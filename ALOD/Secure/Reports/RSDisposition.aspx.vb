Imports ALOD.Core.Domain.DBSign
Imports ALOD.Core.Domain.Documents
Imports ALOD.Core.Domain.Modules.Lod
Imports ALOD.Core.Domain.Modules.SpecialCases
Imports ALOD.Core.Domain.Reports
Imports ALOD.Core.Domain.ServiceMembers
Imports ALOD.Core.Domain.Users
Imports ALOD.Core.Domain.Workflow
Imports ALOD.Core.Interfaces.DAOInterfaces
Imports ALOD.Data
Imports ALOD.Data.Services
Imports ALOD.Logging
Imports ALODWebUtility.Common

Namespace Web.Reports

    Partial Class RSDisposition
        Inherits System.Web.UI.Page

#Region "Fields & Properties..."

        Private Const ACTION_CREATE As String = "create"
        Private Const ACTION_SIGN As String = "sign"
        Private Const ASCENDING As String = "Asc"
        Private Const DESCENDING As String = "Desc"
        Private Const SORT_COLUMN_KEY As String = "_SortExp_"
        Private Const SORT_DIR_KEY As String = "_SortDirection_"
        Private _daoFactory As IDaoFactory
        Private _docCatViewDao As IDocCategoryViewDao
        Private _documentDao As IDocumentDao
        Private _reportsDao As IReportsDao
        Private _scDao As ISpecialCaseDAO
        Private _serviceMemberDao As IMemberDao
        Private _userDao As IUserDao

        ReadOnly Property DocCatViewDao() As IDocCategoryViewDao
            Get
                If (_docCatViewDao Is Nothing) Then
                    _docCatViewDao = DaoFactory.GetDocCategoryViewDao()
                End If

                Return _docCatViewDao
            End Get
        End Property

        ReadOnly Property DocumentDao() As IDocumentDao
            Get
                If (_documentDao Is Nothing) Then
                    _documentDao = New SRXDocumentStore(CStr(HttpContext.Current.Session("UserName")))
                End If

                Return _documentDao
            End Get
        End Property

        Protected ReadOnly Property DaoFactory As IDaoFactory
            Get
                If (_daoFactory Is Nothing) Then
                    _daoFactory = New NHibernateDaoFactory()
                End If

                Return _daoFactory
            End Get
        End Property

        Protected ReadOnly Property IsValidUser() As Boolean
            Get
                If (CInt(Session("GroupId")) = UserGroups.AFRCHQTechnician) Then
                    Return True
                Else
                    Return False
                End If
            End Get
        End Property

        Protected ReadOnly Property ReportsDao As IReportsDao
            Get
                If (_reportsDao Is Nothing) Then
                    _reportsDao = DaoFactory.GetReportsDao()
                End If

                Return _reportsDao
            End Get
        End Property

        Protected ReadOnly Property ServiceMemberDao As IMemberDao
            Get
                If (_serviceMemberDao Is Nothing) Then
                    _serviceMemberDao = New NHibernateDaoFactory().GetMemberDao()
                End If

                Return _serviceMemberDao
            End Get
        End Property

        Private Property ActionMode() As String
            Get
                Dim key As String = "ActionMode"
                If (ViewState(key) Is Nothing) Then
                    ViewState(key) = ACTION_CREATE
                End If
                Return ViewState(key)
            End Get
            Set(ByVal value As String)
                ViewState("ActionMode") = value
            End Set
        End Property

        Private ReadOnly Property ApplicationUser As AppUser
            Get
                Return UserDao.FindById(SessionInfo.SESSION_USER_ID)
            End Get
        End Property

        Private Property SortColumn() As String
            Get
                If (ViewState(SORT_COLUMN_KEY) Is Nothing) Then
                    ViewState(SORT_COLUMN_KEY) = "LastName"
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

        Private ReadOnly Property SpecCaseDao() As ISpecialCaseDAO
            Get
                If (_scDao Is Nothing) Then
                    _scDao = DaoFactory.GetSpecialCaseDAO()
                End If

                Return _scDao
            End Get
        End Property

        Private ReadOnly Property UserDao As IUserDao
            Get
                If (_userDao Is Nothing) Then
                    _userDao = DaoFactory.GetUserDao()
                End If

                Return _userDao
            End Get
        End Property

#End Region

#Region "Page Methods..."

        Public Sub RunReport(ByVal sender As Object, ByVal e As System.EventArgs)
            If (Page.IsValid) Then
                ReportGrid.Sort(rptTemplate.SortOrder, WebControls.SortDirection.Ascending)
            End If

            GridViewErrorLabel.Text = ""
        End Sub

        Protected Function ConstructReportArgs(ByVal ssn As String) As RSDispositionReportArgs
            Dim args As RSDispositionReportArgs = New RSDispositionReportArgs()

            args.UnitId = rptTemplate.Unit
            args.SSN = ssn
            args.ReportView = CInt(Session("ReportView"))
            args.BeginDate = rptTemplate.BeginDate
            args.EndDate = rptTemplate.EndDate
            args.IsComplete = rptTemplate.IsComplete
            args.IncludeSubordinateUnits = rptTemplate.IncludeSubordinate

            Return args
        End Function

        Protected Function DetermineReassessmentLinkVisbility(ByVal isFinal As Boolean, ByVal status As Integer, ByVal caseId As String, ByVal refId As Integer) As Boolean
            Return (IsValidUser() AndAlso Not IsMemberTheUser(refId) AndAlso isFinal AndAlso IsValidStatus(status) AndAlso Not IsReassessmentCase(caseId) AndAlso Not IsReassessmentInProgress(refId))
        End Function

        Protected Function GetNextRSCaseId(ByVal originalCaseId As String, ByVal originalRefId As Integer) As String
            Dim caseCount As Integer = 0

            ' Get the number of reassessment cases alread created for the original case...
            caseCount = SpecCaseDao.GetReassessmentCount(originalRefId)

            Return (originalCaseId & "-" & Utility.ENGLISH_ALPHABET_UPPERCASE(caseCount))
        End Function

        Protected Function GetServiceMembersByName() As DataTable
            Return LookupService.GetServerMembersByName(rptTemplate.MemberLastName, rptTemplate.MemberFirstName, rptTemplate.MemberMiddleName)
        End Function

        Protected Sub GridView1_PageIndexChanging(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewPageEventArgs) Handles ReportGrid.PageIndexChanging
            ReportGrid.PageIndex = e.NewPageIndex
            FillGrid(GetSelectedMemberSSN())
        End Sub

        Protected Sub GridView1_RowCommand(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewCommandEventArgs) Handles ReportGrid.RowCommand
            GridViewErrorLabel.Text = ""

            If (e.CommandName.Equals("reassessment") AndAlso IsValidUser()) Then
                Dim arguments As String() = e.CommandArgument.ToString().Split("|")

                If (arguments.Count <> 2) Then
                    GridViewErrorLabel.Text = "Failed to create reassessment RS case: Invalid case arguments."
                    Exit Sub
                End If

                Dim refId As Integer = CType(arguments(0), Integer)
                Dim newRefId As Integer = 0
                Dim newCaseId As String = GetNextRSCaseId(arguments(1), refId)

                ' Create the new Reassessment RS case...
                If (refId > 0 AndAlso Not String.IsNullOrEmpty(newCaseId)) Then
                    newRefId = SpecCaseDao.CreateReassessRSCase(SESSION_USER_ID, refId, newCaseId, AFRCWorkflows.SpecCaseRS)
                End If

                If (newRefId <= 0) Then
                    GridViewErrorLabel.Text = "Failed to create reassessment RS case: Case initialization failed."
                    Exit Sub
                End If

                Dim oldCase As SpecialCase = SpecCaseDao.GetById(refId)
                Dim newCase As SpecialCase = SpecCaseDao.GetById(newRefId)
                Dim isDocGroupCreated As Boolean = True

                ' Create document group Id for the new Reassessment case...
                If (newCase.DocumentGroupId = 0) Then
                    isDocGroupCreated = newCase.CreateDocumentGroup(DocumentDao)
                    SpecCaseDao.SaveOrUpdate(newCase)
                End If

                If (Not isDocGroupCreated) Then
                    GridViewErrorLabel.Text = "Failed to create reassessment RS case: Failed to create case document group."
                    SpecCaseDao.Delete(newCase)
                    Exit Sub
                End If

                ' Copy the documents from the original case to the new Reassessment case, and place them in the "Original Documents" category...
                Dim viewCats As List(Of DocumentCategory2) = DocCatViewDao.GetCategoriesByDocumentViewId(DocumentViewType.RS)

                For Each cat As DocumentCategory2 In viewCats
                    DocumentDao.CopyGroupDocuments(oldCase.DocumentGroupId, newCase.DocumentGroupId, cat.DocCatId, DocumentType.OriginalDocuments)
                Next

                ' Update Reminder Emails...
                Dim reminderDao = New ReminderEmailsDao()
                reminderDao.ReminderEmailInitialStep(newCase.Id, newCase.Status, "SC")

                LogManager.LogAction(ModuleType.SpecCaseRS, UserAction.InitiatedSpecialCase, newRefId, "Workflow: Recruiting Services (RS)", newCase.Status)

                ActionMode = ACTION_SIGN
                ViewState("oldRefId") = refId
                ViewState("refId") = newRefId
                ViewState("refStatus") = newCase.Status

                SigBlock.StartSignature(newRefId, newCase.Workflow, 0, "Initiate RS Reassessment", newCase.Status, newCase.Status, 0, DBSignTemplateId.SignOnly, "")

                Response.Redirect("~/Secure/SC_RS/init.aspx?refId=" + newRefId.ToString())
            End If
        End Sub

        Protected Sub GridView1_Sorting(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewSortEventArgs) Handles ReportGrid.Sorting
            ReportGrid.PageIndex = 0

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

            If (rptTemplate.SSNRadioButtonChecked) Then
                FillGrid(rptTemplate.SSN)
            ElseIf (rptTemplate.MemberNameRadioButtonChecked) Then
                rptTemplate.InvalidMemberNameErrorLabelVisibility = False
                rptTemplate.MemberNotFoundErrorLabelVisibility = False

                If (rptTemplate.IsMemberNameInvalid) Then
                    rptTemplate.InvalidMemberNameErrorLabelVisibility = True
                    MemberSelectionPanel.Visible = False
                    ResultsPanel.Visible = False
                    Exit Sub
                End If

                Dim resultsTable As DataTable = GetServiceMembersByName()

                If (resultsTable.Rows.Count > 1) Then
                    ucMemberSelectionGrid.Initialize(resultsTable)
                    MemberSelectionPanel.Visible = True
                    ResultsPanel.Visible = False
                    Exit Sub
                ElseIf (resultsTable.Rows.Count = 1) Then
                    Session("rowIndex") = 0
                    FillGrid(resultsTable.Rows(0).Field(Of String)("SSN"))
                Else
                    rptTemplate.MemberNotFoundErrorLabelVisibility = True
                    Exit Sub
                End If
            End If
        End Sub

        Protected Function IsReassessmentCase(ByVal caseId As String) As Boolean
            Dim parts As String() = caseId.Split("-")

            ' Check if there are less than four parts of the case id...
            If (parts.Length < 4) Then
                Return False
            End If

            ' Check if the final character is not an alpha character...
            If (Not Char.IsLetter(Convert.ToChar(parts(3).Substring(0, 1)))) Then
                Return False
            End If

            Return True
        End Function

        Protected Function IsReassessmentInProgress(ByVal refId As Integer) As Boolean
            Dim sc As SpecialCase = SpecCaseDao.GetReassessmentByOriginalId(refId)

            If (sc Is Nothing) Then
                Return False
            End If

            If (sc.WorkflowStatus.StatusCodeType.IsFinal AndAlso sc.WorkflowStatus.Id <> SpecCaseRSWorkStatus.Qualified) Then
                Return False
            End If

            Return True
        End Function

        Protected Function IsValidStatus(ByVal workStatusId As Integer) As Boolean
            ' Only fully adjudicated RS cases which have a disposition of Disqualified or Canclled can be reassessed
            If (workStatusId = SpecCaseRSWorkStatus.Disqualified OrElse workStatusId = SpecCaseRSWorkStatus.Cancelled) Then
                Return True
            Else
                Return False
            End If
        End Function

        Protected Sub MemberSelected(ByVal sender As Object, ByVal e As MemberSelectedEventArgs)
            Dim resultsTable As DataTable = GetServiceMembersByName()
            Dim ssn As String = String.Empty

            Session("rowIndex") = e.SelectedRowIndex
            ssn = resultsTable.Rows(e.SelectedRowIndex).Field(Of String)("SSN")

            FillGrid(ssn)
        End Sub

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
            AddHandler rptTemplate.RptClicked, AddressOf RunReport
            AddHandler ucMemberSelectionGrid.MemberSelected, AddressOf MemberSelected

            If (Not Page.IsPostBack) Then

            End If
        End Sub

        Protected Sub ReportGrid_RowDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewRowEventArgs) Handles ReportGrid.RowDataBound
            Dim grid As GridView = CType(sender, GridView)

            If (e.Row.RowType = DataControlRowType.Header) Then
                Dim cellIndex As Integer = -1

                For Each field As DataControlField In grid.Columns
                    If (field.SortExpression = SortColumn) Then
                        cellIndex = grid.Columns.IndexOf(field)
                    End If
                Next

                If (cellIndex > -1) Then
                    If (SortDirection = ASCENDING) Then
                        e.Row.Cells(cellIndex).CssClass = "gridViewHeader sort-asc"
                    Else
                        e.Row.Cells(cellIndex).CssClass = "gridViewHeader sort-desc"
                    End If

                End If
            End If
        End Sub

        Private Sub FillGrid(ByVal ssn As String)
            Dim ds As DataSet = ReportsDao.ExecuteRSDispositionReport(ConstructReportArgs(ssn))
            Dim view As DataView = New DataView(ds.Tables(0))

            If (SortColumn.Length > 0) Then
                view.Sort = SortExpression
            End If

            ReportGrid.DataSource = view
            ReportGrid.DataBind()

            ResultsPanel.Visible = True
            MemberSelectionPanel.Visible = False
        End Sub

        Private Function GetSelectedMemberSSN() As String
            If (rptTemplate.SSNRadioButtonChecked) Then
                Return rptTemplate.SSN
            ElseIf (rptTemplate.MemberNameRadioButtonChecked) Then
                If (rptTemplate.IsMemberNameInvalid) Then
                    Return String.Empty
                End If

                If (Session("rowIndex") Is Nothing) Then
                    Return String.Empty
                End If

                Dim resultsTable As DataTable = GetServiceMembersByName()

                If (resultsTable.Rows.Count > 0) Then
                    Return resultsTable.Rows(Session("rowIndex")).Field(Of String)("SSN")
                Else
                    Return String.Empty
                End If
            End If

            Return String.Empty
        End Function

        Private Function IsMemberTheUser(refId As Integer) As Boolean
            Dim memberSsn As String = SpecCaseDao.GetById(refId).MemberSSN
            Dim member As ServiceMember = ServiceMemberDao.GetById(memberSsn)

            Return ApplicationUser.IsPersonnalServiceMemberRecord(member)
        End Function

#End Region

    End Class

End Namespace