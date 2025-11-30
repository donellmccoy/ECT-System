Imports ALOD.Core.Domain.DBSign
Imports ALOD.Core.Domain.Documents
Imports ALOD.Core.Domain.Modules.Appeals
Imports ALOD.Core.Domain.Modules.Lod
Imports ALOD.Core.Domain.Modules.Reinvestigations
Imports ALOD.Core.Domain.Reports
Imports ALOD.Core.Domain.ServiceMembers
Imports ALOD.Core.Domain.Users
Imports ALOD.Core.Domain.Workflow
Imports ALOD.Core.Interfaces.DAOInterfaces
Imports ALOD.Data
Imports ALOD.Data.Services
Imports ALOD.Logging
Imports ALODWebUtility.Common
Imports ALODWebUtility.Printing
Imports ALODWebUtility.Worklfow

Namespace Web.Reports

    Partial Class Secure_Reports_LodDisposition
        Inherits System.Web.UI.Page

#Region "Fields & Properties..."

        Protected Const KEY_CREATEDCASEWORKFLOW As String = "createdCaseWorkflow"
        Protected Const KEY_OLDREFID As String = "oldRefId"
        Protected Const KEY_REDIRECTURL As String = "redirectURL"
        Protected Const KEY_REFID As String = "refId"
        Protected Const KEY_REFSTATUS As String = "refStatus"
        Private Const ACTION_CREATE As String = "create"
        Private Const ACTION_SIGN As String = "sign"
        Private Const ASCENDING As String = "Asc"
        Private Const DESCENDING As String = "Desc"
        Private Const SORT_COLUMN_KEY As String = "_SortExp_"
        Private Const SORT_DIR_KEY As String = "_SortDirection_"
        Private _appealDao As ILODAppealDAO
        Private _dao As IDocumentDao
        Private _daoFactory As IDaoFactory
        Private _docCatViewDao As IDocCategoryViewDao
        Private _documents As IList(Of ALOD.Core.Domain.Documents.Document)
        Private _lineOfDutyDao As ILineOfDutyDao
        Private _reminderEmailDao As IReminderEmailDao
        Private _reportsDao As IReportsDao
        Private _rrDao As ILODReinvestigateDAO
        Private _serviceMemberDao As IMemberDao
        Private _userDao As IUserDao
        Private instance As LineOfDuty

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
                If (_dao Is Nothing) Then
                    _dao = New SRXDocumentStore(CStr(HttpContext.Current.Session("UserName")))
                End If

                Return _dao
            End Get
        End Property

        Protected ReadOnly Property AppealDao As ILODAppealDAO
            Get
                If (_appealDao Is Nothing) Then
                    _appealDao = DaoFactory.GetLODAppealDao()
                End If

                Return _appealDao
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

        Protected ReadOnly Property IsUserWingCC() As Boolean
            Get
                If CInt(Session("GroupId")) = UserGroups.WingCommander Then
                    Return True
                Else
                    Return False
                End If
            End Get
        End Property

        Protected ReadOnly Property IsValidUser() As Boolean
            Get
                If (CInt(Session("GroupId")) = UserGroups.MPF OrElse CInt(Session("GroupId")) = UserGroups.LOD_PM) Then
                    Return True
                Else
                    Return False
                End If
            End Get
        End Property

        Protected ReadOnly Property ReminderEmailDao As IReminderEmailDao
            Get
                If (_reminderEmailDao Is Nothing) Then
                    _reminderEmailDao = DaoFactory.GetReminderEmailDao()
                End If

                Return _reminderEmailDao
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

        Protected ReadOnly Property RRDao As ILODReinvestigateDAO
            Get
                If (_rrDao Is Nothing) Then
                    _rrDao = DaoFactory.GetLODReinvestigationDao()
                End If

                Return _rrDao
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
                Dim key As String = "ActionMode"
                ViewState(key) = value
            End Set
        End Property

        Private ReadOnly Property ApplicationUser As AppUser
            Get
                Return UserDao.FindById(SessionInfo.SESSION_USER_ID)
            End Get
        End Property

        Private ReadOnly Property LodDao() As ILineOfDutyDao
            Get
                If (_lineOfDutyDao Is Nothing) Then
                    _lineOfDutyDao = DaoFactory.GetLineOfDutyDao()
                End If
                Return _lineOfDutyDao
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

        Protected Function ConstructReportArgs(ByVal ssn As String) As LODDispositionReportArgs
            Dim args As LODDispositionReportArgs = New LODDispositionReportArgs()

            args.UnitId = rptTemplate.Unit
            args.SSN = ssn
            args.ReportView = CInt(Session("ReportView"))
            args.BeginDate = rptTemplate.BeginDate
            args.EndDate = rptTemplate.EndDate
            args.IsComplete = rptTemplate.IsComplete
            args.IncludeSubordinateUnits = rptTemplate.IncludeSubordinate

            Return args
        End Function

        Protected Sub CreateAppealCase(ByVal initialLODId As Integer)
            Dim member As ServiceMember = Nothing
            Dim lod As LineOfDuty = LodDao.GetById(initialLODId)

            Dim appealId As Integer = 0
            appealId = AppealDao.GetAppealIdByInitLod(initialLODId)

            ' Disables link if existing appeal for this LOD is still in progress...
            If Not (appealId = 0) Then
                Dim ap As LODAppeal = AppealDao.GetById(appealId)
                If (ap IsNot Nothing AndAlso Not ap.WorkflowStatus.StatusCodeType.IsFinal) Then
                    GridViewErrorLabel.Text = "The selected case (" & lod.CaseId & ") already has an active Appeal Request in progress (" & ap.CaseId & ")."
                    Exit Sub
                End If
            End If

            If (Not HasNotificationMemo(lod.Id)) Then
                GridViewErrorLabel.Text = "The selected case (" & lod.CaseId & ") requires the Member's Signed Notification Memo to process an Appeal. Please contact the Unit Commander."
                Exit Sub
            End If

            member = ServiceMemberDao.GetById(lod.MemberSSN)

            If (member Is Nothing) Then
                GridViewErrorLabel.Text = "Could not find the Service Member of the selected case."
                Exit Sub
            End If

            If (SESSION_GROUP_ID = UserGroups.LOD_PM) Then
                Dim appealRequestArgs As New AppealInitiateCaseArgs(AppealDao)

                appealRequestArgs.OriginalCaseRefId = lod.Id
                appealRequestArgs.OriginalCaseCaseId = lod.CaseId
                appealRequestArgs.Member = member
                appealRequestArgs.CreatedByUserId = SESSION_USER_ID
                appealRequestArgs.InitialWorkStatus = LookupService.GetInitialStatus(SESSION_USER_ID, SESSION_GROUP_ID, AFRCWorkflows.AppealRequest)
                appealRequestArgs.DocumentDao = DocumentDao

                Dim newAppealRequest As New LODAppeal()

                newAppealRequest.InitiateCase(appealRequestArgs)

                AppealDao.Save(newAppealRequest)

                If (newAppealRequest.Id > 0) Then
                    Dim id As Integer = newAppealRequest.Id
                    Dim status As Integer = newAppealRequest.Status

                    LogManager.LogAction(ModuleType.AppealRequest, UserAction.RequestedAppeal, id, "Workflow: Appeal Request", status)

                    ActionMode = ACTION_SIGN
                    ViewState(KEY_OLDREFID) = initialLODId
                    ViewState(KEY_REFID) = id
                    ViewState(KEY_REFSTATUS) = status
                    ViewState(KEY_REDIRECTURL) = "~/Secure/AppealRequests/init.aspx?requestId="
                    ViewState(KEY_CREATEDCASEWORKFLOW) = "AP"

                    SigBlock.StartSignature(id, newAppealRequest.Workflow, 0, "Initiate LOD Appeal", status, status, 0, DBSignTemplateId.Form348AP, "")
                End If
            Else
                GridViewErrorLabel.Text = "Appeal Requests must be initiated by the LOD-PM."
                Exit Sub
            End If
        End Sub

        Protected Sub CreateReinvestigationCase(ByVal initialLODId As Integer)
            Dim member As ServiceMember = Nothing
            Dim lod As LineOfDuty = LodDao.GetById(initialLODId)

            Dim rRequestId As Integer = 0
            rRequestId = RRDao.GetReinvestigationRequestIdByInitLod(initialLODId)

            ' Disables link if existing Reinvestigation Request for this LOD is still in progress...
            If Not (rRequestId = 0) Then
                Dim rr As LODReinvestigation = RRDao.GetById(rRequestId)
                If (rr IsNot Nothing AndAlso Not rr.WorkflowStatus.StatusCodeType.IsFinal) Then
                    GridViewErrorLabel.Text = "The selected case (" & lod.CaseId & ") already has an active Reinvestigation Request in progress (" & rr.CaseId & ")."
                    Exit Sub
                End If
            End If

            member = ServiceMemberDao.GetById(lod.MemberSSN)

            If (member Is Nothing) Then
                GridViewErrorLabel.Text = "Could not find the Service Member of the selected case."
                Exit Sub
            End If

            If (SESSION_GROUP_ID = UserGroups.MPF OrElse SESSION_GROUP_ID = UserGroups.LOD_PM) Then
                Dim reinvestigationRequestArgs As New RRInitiateCaseArgs(RRDao)

                reinvestigationRequestArgs.OriginalCaseRefId = lod.Id
                reinvestigationRequestArgs.OriginalCaseCaseId = lod.CaseId
                reinvestigationRequestArgs.Member = member
                reinvestigationRequestArgs.CreatedByUserId = SESSION_USER_ID
                reinvestigationRequestArgs.DocumentDao = DocumentDao

                Dim newReinvestigationRequest As New LODReinvestigation()

                newReinvestigationRequest.InitiateCase(reinvestigationRequestArgs)

                RRDao.Save(newReinvestigationRequest)

                If (newReinvestigationRequest.Id > 0) Then
                    Dim id As Integer = newReinvestigationRequest.Id
                    Dim status As Integer = newReinvestigationRequest.Status

                    LogManager.LogAction(ModuleType.ReinvestigationRequest, UserAction.RequestedReinvestigation, id, "Workflow: Reinvestigation Request", status)

                    ActionMode = ACTION_SIGN
                    ViewState(KEY_OLDREFID) = initialLODId
                    ViewState(KEY_REFID) = id
                    ViewState(KEY_REFSTATUS) = status
                    ViewState(KEY_REDIRECTURL) = "~/Secure/ReinvestigationRequests/init.aspx?requestId="
                    ViewState(KEY_CREATEDCASEWORKFLOW) = "RR"

                    SigBlock.StartSignature(id, newReinvestigationRequest.Workflow, 0, "Initiate LOD Reinvestigation", status, status, 0, DBSignTemplateId.Form348RR, "")
                End If
            Else
                'Disables link for Wing CC
                GridViewErrorLabel.Text = "Reinvestigation Requests must now be initiated by the LOD-PM."
                Exit Sub
            End If
        End Sub

        Protected Function DetermineAPLinkVisibility(isFinal As Boolean, findingId As Integer, caseId As String, isFormal As Boolean, lodId As Integer) As Boolean
            Return (IsValidUser() AndAlso Not IsMemberTheUser(lodId) AndAlso isFinal AndAlso Not IsLODInFavorOfMember(findingId, lodId) AndAlso Not LookupService.GetIsReinvestigationLod(lodId) AndAlso Not hasReinvestigationInProgress(lodId) AndAlso Not LODHasAppealActive(lodId, True))
        End Function

        Protected Function DetermineRRLinkVisibility(isFinal As Boolean, findingId As Integer, caseId As String, lodId As Integer, isFormal As Boolean) As Boolean
            Return (IsValidUser() AndAlso Not IsMemberTheUser(lodId) AndAlso isFinal AndAlso Not IsLODInFavorOfMember(findingId, lodId) AndAlso Not LookupService.GetIsReinvestigationLod(lodId) AndAlso Not hasReinvestigationInProgress(lodId) AndAlso Not LODHasAppealActive(lodId, False) AndAlso isFormal)
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

            If (e.CommandName = "appeal") Then
                CreateAppealCase(CType(e.CommandArgument.ToString(), Integer))
            ElseIf (e.CommandName = "reinvestigate") Then
                CreateReinvestigationCase(CType(e.CommandArgument.ToString(), Integer))
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

        Protected Function HasNotificationMemo(ByVal lodId As Integer) As Boolean
            instance = LodService.GetById(lodId)
            instance.ProcessDocuments(DaoFactory)
            _documents = DocumentDao.GetDocumentsByGroupId(instance.DocumentGroupId)

            Dim docs = (From d In _documents Where d.DocType = DocumentType.SignedNotificationMemo Select d).Count

            If (docs > 0) Then
                Return True
            End If

            Return False
        End Function

        Protected Function hasReinvestigationInProgress(ByVal lodid As Integer) As Boolean
            If (LookupService.GetInProgressReinvestigation(lodid) > 0 OrElse LookupService.LODHasActiveReinvestigation(lodid)) Then
                Return True
            End If

            Return False
        End Function

        Protected Function IsLODInFavorOfMember(ByVal findingId As Integer, ByVal lodId As Integer) As Boolean
            Dim findingIds As List(Of Integer) = New List(Of Integer)()

            findingIds.Add(findingId)

            ' If the LOD has been reinvestigated then get the findings for the reinvestigations...
            If (LookupService.GetHasReinvestigationLod(lodId)) Then
                Dim reinvestigationFindings As List(Of Integer) = Nothing

                reinvestigationFindings = LookupService.GetReinvestigationLodFindings(lodId)

                If (reinvestigationFindings IsNot Nothing AndAlso reinvestigationFindings.Count > 0) Then
                    findingIds.AddRange(reinvestigationFindings)
                End If
            End If

            ' Check if any of the findings have been found in favor of the member...
            For Each id As Integer In findingIds
                If (id = ALOD.Core.Utils.Finding.In_Line_Of_Duty OrElse id = ALOD.Core.Utils.Finding.Epts_Service_Aggravated) Then
                    Return True
                End If
            Next

            If (LookupService.GetHasAppealLOD(lodId)) Then
                Dim appealId = AppealDao.GetAppealIdByInitLod(lodId)
                Dim appeal = AppealDao.GetById(appealId)

                If (appeal.Status = AppealWorkStatus.AppealApproved) Then
                    Return True
                End If

            End If

            ' No findings were found to be in favor of the member...
            Return False
        End Function

        Protected Function IsReinvestigatedLOD(ByVal caseId As String) As Boolean
            Dim parts As String() = caseId.Split("-")

            ' Check if there are less than three parts of the case id...
            If (parts.Length < 3) Then
                Return False
            End If

            ' Check if the final character is not an alpha character...
            If (Not Char.IsLetter(Convert.ToChar(parts(2).Substring(0, 1)))) Then
                Return False
            End If

            Return True
        End Function

        Protected Function LODHasAppealActive(ByVal lodId As Integer, ByVal forAP As Boolean) As Boolean
            If (LookupService.GetHasAppealLOD(lodId)) Then
                If (forAP) Then
                    Return True
                End If

                Dim appealId = AppealDao.GetAppealIdByInitLod(lodId)
                Dim appeal = AppealDao.GetById(appealId)

                If (appeal.Status = AppealWorkStatus.AppealDenied OrElse appeal.Status = AppealWorkStatus.AppealApproved) Then
                    Return False
                End If

                Return True
            End If

            ' This case does not have an open or completed Appeal case
            Return False
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

            If (e.Row.RowType = DataControlRowType.DataRow) Then
                Dim data As DataRowView = CType(e.Row.DataItem, DataRowView)
                Dim refID As Integer = data("lodid")
                ViewFinal(refID, data, e)
            End If
        End Sub

        Protected Sub SignatureCompleted(ByVal sender As Object, ByVal e As SignCompletedEventArgs) Handles SigBlock.SignCompleted
            If (e.SignaturePassed) Then
                If (ViewState(KEY_CREATEDCASEWORKFLOW).Equals("RR")) Then
                    RRDao.CommitChanges()
                ElseIf (ViewState(KEY_CREATEDCASEWORKFLOW).Equals("AP")) Then
                    AppealDao.CommitChanges()
                End If

                ReminderEmailDao.ReminderEmailInitialStep(ViewState(KEY_REFID), ViewState(KEY_REFSTATUS), ViewState(KEY_CREATEDCASEWORKFLOW))

                Response.Redirect(ViewState(KEY_REDIRECTURL) & ViewState(KEY_REFID).ToString())
            End If
        End Sub

        Protected Sub ViewFinal(ByVal refID As String, ByVal data As DataRowView, ByVal e As System.Web.UI.WebControls.GridViewRowEventArgs)
            Dim form348ID As String = 0
            Dim form261ID As String = 0
            Dim printImageBtn As ImageButton = CType(e.Row.FindControl("PrintImage"), ImageButton)

            If (printImageBtn Is Nothing) Then
                Exit Sub
            End If

            instance = LodService.GetById(CInt(data("lodid")))

            ' ckeck for GroupID; some cases were cancled
            If (instance.DocumentGroupId) Then
                _documents = DocumentDao.GetDocumentsByGroupId(instance.DocumentGroupId)
            End If

            printImageBtn.OnClientClick = ViewForms.LinkAttribute348(refID, _documents, "lod")

            If (data("IsFinal").ToString() = "True") Then
                printImageBtn.AlternateText = "Print Final Forms"
            Else
                printImageBtn.AlternateText = "Print Draft Forms"
            End If
        End Sub

        Private Sub FillGrid(ByVal ssn As String)
            Dim ds As DataSet = ReportsDao.ExecuteLODDispositionReport(ConstructReportArgs(ssn))
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

        Private Function IsMemberTheUser(lodId As Integer) As Boolean
            Dim memberSsn As String = LodDao.GetById(lodId).MemberSSN
            Dim member As ServiceMember = ServiceMemberDao.GetById(memberSsn)

            Return ApplicationUser.IsPersonnalServiceMemberRecord(member)
        End Function

#End Region

    End Class

End Namespace