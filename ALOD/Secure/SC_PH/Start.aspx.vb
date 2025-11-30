Imports System.Threading
Imports ALOD.Core.Domain.Documents
Imports ALOD.Core.Domain.Lookup
Imports ALOD.Core.Domain.Modules.Lod
Imports ALOD.Core.Domain.Modules.SpecialCases
Imports ALOD.Core.Domain.Users
Imports ALOD.Core.Domain.Workflow
Imports ALOD.Core.Interfaces.DAOInterfaces
Imports ALOD.Data
Imports ALOD.Data.Services
Imports ALOD.Logging
Imports ALODWebUtility.Common
Imports ALODWebUtility.Permission.Search

Namespace Web.Special_Case.PH

    Public Class Start
        Inherits System.Web.UI.Page

        Protected Const ACTION_CREATE As String = "create"
        Protected Const ACTION_SIGN As String = "sign"
        Protected Const KEY_ACTION_MODE As String = "action_mode"
        Protected Const KEY_REFID As String = "refId"
        Protected Const KEY_REFSTATUS As String = "refStatus"
        Protected CURR_MODULE As String = ""
        Protected URL_SC_INIT As String = ""
        Private Const ASCENDING As String = "Asc"
        Private Const DESCENDING As String = "Desc"
        Private Const SORT_COLUMN_KEY As String = "_SortExp_"
        Private Const SORT_DIR_KEY As String = "_SortDirection_"
        Private _daoFactory As IDaoFactory
        Private _documentDao As IDocumentDao
        Private _dphUser As AppUser
        Private _lookupDao As ILookupDao
        Private _phDao As IPsychologicalHealthDao
        Private _scDao As ISpecialCaseDAO
        Private _userDao As IUserDao
        Private _validationErrors As List(Of String)
        Private _workflowDao As IWorkflowDao

        Protected ReadOnly Property ModuleId() As Integer
            Get
                Return CInt(Request.QueryString("mid"))
            End Get
        End Property

        Private Property ActionMode() As String
            Get
                If (ViewState(KEY_ACTION_MODE) Is Nothing) Then
                    ViewState(KEY_ACTION_MODE) = ACTION_CREATE
                End If
                Return ViewState(KEY_ACTION_MODE)
            End Get
            Set(ByVal value As String)
                ViewState(KEY_ACTION_MODE) = value
            End Set
        End Property

        Private ReadOnly Property DPHUser As AppUser
            Get
                If (_dphUser Is Nothing) Then
                    _dphUser = UserDao.GetById(SESSION_USER_ID)
                End If

                Return _dphUser
            End Get
        End Property

        Private ReadOnly Property SelectedReportingPeriod As Nullable(Of DateTime)
            Get
                If (ddlYear.SelectedValue > 0 AndAlso ddlMonth.SelectedValue > 0) Then
                    Return New DateTime(Integer.Parse(ddlYear.SelectedValue), Integer.Parse(ddlMonth.SelectedValue), 1)
                End If

                Return Nothing
            End Get
        End Property

        Private Property ValidationErrors As List(Of String)
            Get
                If (_validationErrors Is Nothing) Then
                    _validationErrors = New List(Of String)()
                End If

                Return _validationErrors
            End Get
            Set(value As List(Of String))
                _validationErrors = value
            End Set
        End Property

#Region "DAO Properties..."

        Protected ReadOnly Property DaoFactory As IDaoFactory
            Get
                If (_daoFactory Is Nothing) Then
                    _daoFactory = New NHibernateDaoFactory()
                End If

                Return _daoFactory
            End Get
        End Property

        Protected ReadOnly Property DocumentDao() As IDocumentDao
            Get
                If (_documentDao Is Nothing) Then
                    _documentDao = New SRXDocumentStore(CStr(HttpContext.Current.Session("UserName")))
                End If

                Return _documentDao
            End Get
        End Property

        Protected ReadOnly Property LookupDao As ILookupDao
            Get
                If (_lookupDao Is Nothing) Then
                    _lookupDao = DaoFactory.GetLookupDao()
                End If

                Return _lookupDao
            End Get
        End Property

        Protected ReadOnly Property PHDao As IPsychologicalHealthDao
            Get
                If (_phDao Is Nothing) Then
                    _phDao = DaoFactory.GetPsychologicalHealthDao()
                End If

                Return _phDao
            End Get
        End Property

        Protected ReadOnly Property SpecCaseDao As ISpecialCaseDAO
            Get
                If (_scDao Is Nothing) Then
                    _scDao = DaoFactory.GetSpecialCaseDAO()
                End If

                Return _scDao
            End Get
        End Property

        Protected ReadOnly Property UserDao As IUserDao
            Get
                If (_userDao Is Nothing) Then
                    _userDao = DaoFactory.GetUserDao()
                End If

                Return _userDao
            End Get
        End Property

        Protected ReadOnly Property WorkflowDao As IWorkflowDao
            Get
                If (_workflowDao Is Nothing) Then
                    _workflowDao = DaoFactory.GetWorkflowDao()
                End If

                Return _workflowDao
            End Get
        End Property

#End Region

#Region "Sort Properties..."

        Private Property SortColumn() As String
            Get
                If (ViewState(SORT_COLUMN_KEY) Is Nothing) Then
                    ViewState(SORT_COLUMN_KEY) = "Case_Id"
                End If

                Return ViewState(SORT_COLUMN_KEY)
            End Get
            Set(ByVal value As String)
                ViewState(SORT_COLUMN_KEY) = value
            End Set
        End Property

        Private Property SortDirection() As String
            Get
                If (ViewState(SORT_DIR_KEY) Is Nothing) Then
                    ViewState(SORT_DIR_KEY) = ASCENDING
                End If

                Return ViewState(SORT_DIR_KEY)
            End Get
            Set(ByVal value As String)
                ViewState(SORT_DIR_KEY) = value
            End Set
        End Property

        Private ReadOnly Property SortExpression() As String
            Get
                Return SortColumn + " " + SortDirection
            End Get
        End Property

#End Region

        Sub SetRedirectURL()
            URL_SC_INIT = "~/Secure/SC_PH/init.aspx?refId="
        End Sub

        Protected Sub CreateButton_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles CreateButton.Click
            Try
                Dim currStatus As Integer
                Dim newId As Integer = 0

                ResetValidationControls()

                If (ValidateCaseInitiateInput()) Then
                    Dim sc As New SC_PH

                    SetMemberRelatedFields(sc)
                    SetSpecialCaseCommonFields(sc)
                    SetPHSpecificFields(sc)

                    currStatus = sc.Status

                    SpecCaseDao.Save(sc)

                    newId = sc.Id
                End If

                If (newId > 0) Then
                    SubmitNewCase(newId, currStatus)
                Else
                    ActionMode = ACTION_CREATE
                    UpdateValidationControls()
                End If
            Catch ex As ThreadAbortException
                ' This exception is thrown by Response.Redirect(param1, True)...exception is thrown by design...can safely ignore...see https://support.microsoft.com/en-us/help/312629/prb-threadabortexception-occurs-if-you-use-response-end--response-redi
            Catch ex As Exception
                LogManager.LogError(ex)
                ValidationErrors.Add("An unexpected error has occured. Please contact the ECT Help Desk for assistance.")
                UpdateValidationControls()
            End Try
        End Sub

        Protected Sub gdvPHHistory_PageIndexChanging(sender As Object, e As System.Web.UI.WebControls.GridViewPageEventArgs) Handles gdvPHHistory.PageIndexChanging
            gdvPHHistory.PageIndex = e.NewPageIndex
            PopulateWingPHHistoryGridView(DPHUser.CurrentUnitId)
        End Sub

        Protected Sub gdvPHHistory_Sorting(sender As Object, e As System.Web.UI.WebControls.GridViewSortEventArgs) Handles gdvPHHistory.Sorting
            gdvPHHistory.PageIndex = 0

            If (SortColumn <> "") Then
                If (SortColumn = e.SortExpression) Then
                    If SortDirection = ASCENDING Then
                        SortDirection = DESCENDING
                    Else
                        SortDirection = ASCENDING
                    End If
                Else
                    SortDirection = ASCENDING
                End If
            End If

            SortColumn = e.SortExpression

            PopulateWingPHHistoryGridView(DPHUser.CurrentUnitId)
        End Sub

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
            If (Not IsPostBack) Then
                InitControls()
            End If
        End Sub

        Protected Sub SubmitNewCase(ByVal newId As Integer, ByVal currentStatus As Integer)
            LogManager.LogAction(ModuleType.SpecCasePH, UserAction.InitiatedSpecialCase, newId, "Workflow: " + WorkflowDao.GetById(AFRCWorkflows.SpecCasePH).Title, currentStatus)
            ActionMode = ACTION_SIGN

            SpecCaseDao.CommitChanges()
            NHibernateSessionManager.Instance().CommitTransaction()
            SetRedirectURL()

            ' Update Reminder Emails
            Dim reminderDao = New ReminderEmailsDao()
            reminderDao.ReminderEmailInitialStep(newId, currentStatus, "SC")

            Response.Redirect(URL_SC_INIT + newId.ToString(), True)
        End Sub

        Private Sub gdvPHHistory_RowCommand(ByVal sender As Object, ByVal e As GridViewCommandEventArgs) Handles gdvPHHistory.RowCommand
            If (e.CommandName = "view") Then
                Dim parts() As String = e.CommandArgument.ToString().Split(";")
                Dim strQuery As New StringBuilder()
                Dim args As New ItemSelectedEventArgs()
                args.RefId = parts(0)

                strQuery.Append("refId=" + CType(args.RefId, String))
                args.Type = parts(1)

                args.Url = GetWorkflowInitPageURL(args.Type, args.RefId)

                Session("RefId") = args.RefId
                Response.Redirect(args.Url)
            End If
        End Sub

        Private Sub InitControls()
            If (DPHUser Is Nothing) Then
                CreateButton.Enabled = False
                pnlWingPHHistory.Visible = False
                Exit Sub
            End If

            PopulateReportingPeriodDDLs()

            lblDPHName.Text = DPHUser.FullName
            lblDPHUnit.Text = DPHUser.CurrentUnitName
            lblHistoryPanelTitle.Text = (WorkflowDao.GetById(AFRCWorkflows.SpecCasePH).Title & " History for the " & DPHUser.CurrentUnitName)
            PopulateWingPHHistoryGridView(DPHUser.CurrentUnitId)
        End Sub

        Private Function IsSelectedReportingPeriodBeyondCurrentMonth() As Boolean
            If (ddlYear.SelectedValue > DateTime.Now.Year OrElse (ddlYear.SelectedValue = DateTime.Now.Year AndAlso ddlMonth.SelectedValue > DateTime.Now.Month)) Then
                Return True
            End If

            Return False
        End Function

        Private Sub PopulateReportingPeriodDDLs()
            ddlYear.Items.Add(New ListItem("-- Select Year --", 0))
            ddlYear.Items.Add(New ListItem((DateTime.Now.Year - 1).ToString(), DateTime.Now.Year - 1))
            ddlYear.Items.Add(New ListItem(DateTime.Now.Year.ToString(), DateTime.Now.Year))

            ddlMonth.Items.Add(New ListItem("-- Select Month --", 0))

            For i As Integer = 1 To 12
                ddlMonth.Items.Add(New ListItem(System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(i), i))
            Next
        End Sub

        Private Sub PopulateWingPHHistoryGridView(ByVal wingId As Integer)
            If (wingId <= 0) Then
                gdvPHHistory.Visible = False
                lblNoHistory.Visible = True
                Exit Sub
            End If

            Dim dSet As DataSet = PHDao.GetCasesByCaseUnit(wingId)

            If (dSet Is Nothing OrElse dSet.Tables.Count = 0) Then
                gdvPHHistory.Visible = False
                lblNoHistory.Visible = True
                Exit Sub
            End If

            gdvPHHistory.Visible = True
            lblNoHistory.Visible = False

            Dim dvSort As DataView = dSet.Tables(0).DefaultView
            dvSort.Sort = SortExpression

            gdvPHHistory.DataSource = dvSort
            gdvPHHistory.DataBind()
        End Sub

        Private Sub ResetValidationControls()
            ValidationErrors = New List(Of String)()
            ValidationErrorsRow.Visible = False
        End Sub

        Private Sub SetMemberRelatedFields(sc As SC_PH)
            ' The PH workflow does not revolve around a specific Member so set the required member fields to empty and the non required member fields to NULL...
            sc.MemberSSN = String.Empty
            sc.MemberName = String.Empty
            sc.MemberUnit = String.Empty
            sc.MemberCompo = Integer.Parse(DPHUser.Component)
            sc.MemberUnitId = DPHUser.CurrentUnitId
            sc.MemberDOB = Nothing
            sc.MemberRank = Nothing
        End Sub

        Private Sub SetPHSpecificFields(sc As SC_PH)
            sc.DPHUser = DPHUser
            sc.CaseUnitId = DPHUser.CurrentUnitId
            sc.ReportingPeriod = SelectedReportingPeriod.Value
            sc.FormLastModified = DateTime.Now

            If (ShouldNewCaseBeDelinquent()) Then
                sc.IsDelinquent = True
                sc.Status = SpecCasePHWorkStatus.Delinquent
            End If
        End Sub

        Private Sub SetSpecialCaseCommonFields(sc As SC_PH)
            sc.moduleId = ModuleType.SpecCasePH
            sc.CreatedBy = SESSION_USER_ID
            sc.CreatedDate = Now
            sc.ModifiedBy = sc.CreatedBy
            sc.ModifiedDate = sc.CreatedDate
            sc.Workflow = AFRCWorkflows.SpecCasePH
            sc.Status = LookupService.GetInitialStatus(SESSION_USER_ID, SESSION_GROUP_ID, sc.Workflow)
            sc.SubWorkflowType = 0
            sc.CreateDocumentGroup(DocumentDao)
        End Sub

        Private Function ShouldNewCaseBeDelinquent() As Boolean
            Dim previousMonth As DateTime = New DateTime(DateTime.Now.Year, DateTime.Now.Month, 1).AddMonths(-1)

            If (SelectedReportingPeriod.Value < previousMonth) Then
                Return True
            ElseIf (SelectedReportingPeriod.Value = previousMonth AndAlso DateTime.Now.Day > 3) Then
                Return True
            End If

            Return False
        End Function

        Private Sub UpdateValidationControls()
            If (ValidationErrors.Count < 1) Then
                ValidationErrors.Add("An error occured initiating the Case of Type " & CURR_MODULE)
            End If

            ValidationList.DataSource = ValidationErrors
            ValidationList.DataBind()
            ValidationList.Visible = True
            ValidationErrorsRow.Visible = True
        End Sub

        Private Function ValidateCaseInitiateInput() As Boolean
            Dim isInputValid As Boolean = True

            If (ddlYear.SelectedValue = 0) Then
                isInputValid = False
                ValidationErrors.Add("Must select a Reporting Period Year.")
            End If

            If (ddlMonth.SelectedValue = 0) Then
                isInputValid = False
                ValidationErrors.Add("Must select a Reporting Period Month.")
            End If

            If (IsSelectedReportingPeriodBeyondCurrentMonth()) Then
                isInputValid = False
                ValidationErrors.Add("The selected Reporting Period cannot be a future date.")
            End If

            If (SelectedReportingPeriod.HasValue) Then
                Dim existingRefId As Integer = PHDao.GetCaseIdByReportingPeriod(SelectedReportingPeriod.Value, DPHUser.CurrentUnitId)

                If (existingRefId > 0) Then
                    Dim existingCase As SpecialCase = SpecCaseDao.GetById(existingRefId)
                    isInputValid = False
                    ValidationErrors.Add("A " & WorkflowDao.GetById(AFRCWorkflows.SpecCasePH).Title & " case (" & existingCase.CaseId & ") has already been created for the reporting period of " & SelectedReportingPeriod.Value.ToString("y") & ".")
                End If
            End If

            Return isInputValid
        End Function

    End Class

End Namespace