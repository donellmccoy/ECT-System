Imports ALOD.Core.Domain.DBSign
Imports ALOD.Core.Domain.Documents
Imports ALOD.Core.Domain.Modules.Lod
Imports ALOD.Core.Domain.Modules.SARC
Imports ALOD.Core.Domain.Reports
Imports ALOD.Core.Domain.ServiceMembers
Imports ALOD.Core.Domain.Users
Imports ALOD.Core.Domain.Workflow
Imports ALOD.Core.Interfaces.DAOInterfaces
Imports ALOD.Data
Imports ALOD.Data.Services
Imports ALOD.Logging
Imports ALODWebUtility.Common
Imports ALODWebUtility.Worklfow
Imports WebSupergoo.ABCpdf8

Namespace Web.Reports

    Public Class LODSARCCasesReport
        Inherits System.Web.UI.Page

#Region "Fields..."

        Protected Const EXCEL_DATA_ROW_STYLE As String = "border: 1px solid #CCC; border-bottom: solid 1px #C0C0C0; background-color:"
        Protected Const EXCEL_HEADER_ROW_STYLE As String = "background-color:navy;text-align: center; border: 1px solid #CCC; border-bottom: solid 1px black; color:white;"
        Protected Const KEY_OLDREFID As String = "oldRefId"
        Protected Const KEY_REFID As String = "refId"
        Protected Const KEY_REFSTATUS As String = "refStatus"

        Protected Const SUBTRACTDAYS As Double = -30

        Protected _report As ALOD.Core.Domain.Reports.LODSARCCasesReport
        Protected _reportsDao As IReportsDao
        Private _daoFactory As IDaoFactory
        Private _docCatViewDao As IDocCategoryViewDao
        Private _documentDao As IDocumentDao
        Private _lineOfDutyDao As ILineOfDutyDao
        Private _lodAppealDao As ILODAppealDAO
        Private _memoSource As IMemoDao
        Private _memoSource2 As IMemoDao2
        Private _reminderEmailDao As IReminderEmailDao
        Private _sarcAppealDao As ISARCAppealDAO
        Private _SARCdao As ISARCDAO
        Private _serviceMemberDao As IMemberDao
        Private _userDao As IUserDao

#End Region

#Region "Properties..."

        ReadOnly Property DocumentDao() As IDocumentDao
            Get
                If (_documentDao Is Nothing) Then
                    _documentDao = New SRXDocumentStore(CStr(HttpContext.Current.Session("UserName")))
                End If

                Return _documentDao
            End Get
        End Property

        ReadOnly Property LODAppealDao() As ILODAppealDAO
            Get
                If (_lodAppealDao Is Nothing) Then
                    _lodAppealDao = DaoFactory.GetLODAppealDao()
                End If

                Return _lodAppealDao
            End Get
        End Property

        Public ReadOnly Property BeginDate() As Nullable(Of Date)
            Get
                If (txtBeginDate.Text.Length = 0) Then
                    txtBeginDate.Text = Now.AddDays(SUBTRACTDAYS).ToShortDateString()
                End If

                Return Server.HtmlEncode(CDate(txtBeginDate.Text))
            End Get
        End Property

        Public ReadOnly Property EndDate() As Nullable(Of Date)
            Get
                If (txtEndDate.Text.Length = 0) Then
                    txtEndDate.Text = Now.ToShortDateString()
                End If

                Return Server.HtmlEncode(CDate(txtEndDate.Text))
            End Get
        End Property

        Protected ReadOnly Property CalendarImage() As String
            Get
                Return GetCalendarImage(Page)
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

        Protected ReadOnly Property DocCatViewDao() As IDocCategoryViewDao
            Get
                If (_docCatViewDao Is Nothing) Then
                    _docCatViewDao = DaoFactory.GetDocCategoryViewDao()
                End If

                Return _docCatViewDao
            End Get
        End Property

        Protected ReadOnly Property HasRestrictedReportingAccess As Boolean
            Get
                Return UserHasPermission(PERMISSION_SARC_LODSARCCASES_REPORT_ALL)
            End Get
        End Property

        Protected ReadOnly Property IsValidUser() As Boolean
            Get
                If (CInt(Session("GroupId")) = UserGroups.WingSarc OrElse CInt(Session("GroupId")) = UserGroups.RSL) Then
                    Return True
                Else
                    Return False
                End If
            End Get
        End Property

        Protected ReadOnly Property MemoDao() As IMemoDao
            Get
                If (_memoSource Is Nothing) Then
                    _memoSource = DaoFactory.GetMemoDao()
                End If
                Return _memoSource
            End Get
        End Property

        Protected ReadOnly Property MemoDao2() As IMemoDao2
            Get
                If (_memoSource2 Is Nothing) Then
                    _memoSource2 = DaoFactory.GetMemoDao2()
                End If
                Return _memoSource2
            End Get
        End Property

        Protected ReadOnly Property OutputFileName As String
            Get
                Return "LOD_SARC_Cases_Report_" + Date.Now.ToString("yyyyMMdd")
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

        Protected ReadOnly Property Report As ALOD.Core.Domain.Reports.LODSARCCasesReport
            Get
                If (_report Is Nothing) Then
                    _report = New ALOD.Core.Domain.Reports.LODSARCCasesReport(ReportsDao)
                End If

                Return _report
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
                    _serviceMemberDao = DaoFactory.GetMemberDao()
                End If

                Return _serviceMemberDao
            End Get
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

        Private ReadOnly Property SARCAppealDao() As ISARCAppealDAO
            Get
                If (_sarcAppealDao Is Nothing) Then
                    _sarcAppealDao = DaoFactory.GetSARCAppealDao()
                End If
                Return _sarcAppealDao
            End Get
        End Property

        Private ReadOnly Property SARCDao() As ISARCDAO
            Get
                If (_SARCdao Is Nothing) Then
                    _SARCdao = DaoFactory.GetSARCDao()
                End If
                Return _SARCdao
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

        Protected Sub btnRunReport_Click(sender As Object, e As EventArgs) Handles btnRunReport.Click
            RunReport()
        End Sub

        Protected Function CanAppeal(refId As Integer, moduleId As Integer, workflowId As Integer) As Boolean
            If (moduleId = ModuleType.LOD) Then
                If (LookupService.GetHasAppealLOD(refId)) Then
                    Return False
                End If
            End If

            If (LookupService.GetHasAppealSARC(refId, workflowId)) Then
                Return False
            End If

            Return True
        End Function

        Protected Function ConstructReportArguments() As LODSARCCasesReportArgs
            Dim args As LODSARCCasesReportArgs = New LODSARCCasesReportArgs()

            args.BeginDate = BeginDate
            args.EndDate = EndDate
            args.CompletionStatus = Integer.Parse(rblCompletionStatus.SelectedValue)

            Dim restrictionStatusSelection As Integer = Integer.Parse(ddlRestrictionStatus.SelectedValue)

            If ((restrictionStatusSelection = 1 OrElse restrictionStatusSelection = 3) AndAlso Not HasRestrictedReportingAccess) Then
                restrictionStatusSelection = 2
            End If

            args.RestrictionStatus = restrictionStatusSelection

            Return args
        End Function

        Protected Function DetermineAPLinkVisibility(isFinal As Boolean, caseId As String, refId As Integer, moduleId As Integer, workflowId As Integer, isRestricted As Boolean) As Boolean
            If (moduleId = ModuleType.LOD) Then
                Return (IsValidUser() AndAlso Not IsMemberTheUser(refId, moduleId) AndAlso isFinal AndAlso isRestricted AndAlso Not HasFindingInFavorOfMember(refId, moduleId, workflowId) AndAlso Not hasReinvestigationInProgress(refId) AndAlso Not LookupService.GetIsReinvestigationLod(refId) AndAlso CanAppeal(refId, moduleId, workflowId) AndAlso HasDetermination(refId, moduleId))
            Else
                Return (IsValidUser() AndAlso Not IsMemberTheUser(refId, moduleId) AndAlso isFinal AndAlso isRestricted AndAlso Not HasFindingInFavorOfMember(refId, moduleId, workflowId) AndAlso CanAppeal(refId, moduleId, workflowId) AndAlso HasDetermination(refId, moduleId))
            End If
        End Function

        Protected Sub DisplayAppeal(ByVal visible As Boolean)

            If (SESSION_GROUP_ID = UserGroups.WingSarc Or SESSION_GROUP_ID = UserGroups.RSL) Then
                gdvResults.Columns(gdvResults.Columns.Count - 1).Visible = visible
            Else
                gdvResults.Columns(gdvResults.Columns.Count - 1).Visible = False
            End If

        End Sub

        Protected Function HasDetermination(ByVal refId As Integer, ByVal moduleId As Integer) As Boolean
            If (moduleId = ModuleType.SARC) Then
                Dim memos = From m In MemoDao2.GetByRefnModule(refId, ModuleType.SARC)
                            Where m.Deleted = False AndAlso m.Template.Id = MemoType.SARC_Determination_NILOD
                            Select (m)
                If (memos.Count > 0) Then
                    Return True
                End If
            Else
                Dim memos = From m In MemoDao.GetByRefId(refId)
                            Where m.Deleted = False AndAlso (m.Template.Id = MemoType.LodFindingsNLOD OrElse m.Template.Id = MemoType.LodFindingsNLODDeath)
                            Select (m)
                If (memos.Count > 0) Then
                    Return True
                End If
            End If

            Return False
        End Function

        Protected Function HasFindingInFavorOfMember(refId As Integer, moduleId As Integer, workflowId As Integer)
            If (moduleId = ModuleType.LOD) Then
                Return IsLODInFavorOfMember(refId, workflowId)
            Else
                Return IsSARCInFavorOfMember(refId, workflowId)
            End If
        End Function

        Protected Function hasReinvestigationInProgress(ByVal lodid As Integer) As Boolean
            If (LookupService.GetInProgressReinvestigation(lodid) > 0 OrElse LookupService.LODHasActiveReinvestigation(lodid)) Then
                Return True
            End If

            Return False
        End Function

        Protected Sub InitControls()
            InitRestrictionStatusDDL()
        End Sub

        Protected Sub InitRestrictionStatusDDL()
            ddlRestrictionStatus.Items.Clear()

            If (HasRestrictedReportingAccess) Then
                ddlRestrictionStatus.Items.Add(New ListItem("All", 1))
                ddlRestrictionStatus.Items.Add(New ListItem("Unrestricted", 2))
                ddlRestrictionStatus.Items.Add(New ListItem("Restricted", 3))
                ddlRestrictionStatus.Enabled = True
            Else
                ddlRestrictionStatus.Items.Add(New ListItem("Unrestricted", 2))
                ddlRestrictionStatus.Enabled = False
            End If
        End Sub

        Protected Function IsLODInFavorOfMember(lodId As Integer, workflowId As Integer) As Boolean
            Dim findingIds As List(Of Integer) = New List(Of Integer)()

            Dim lod = LodDao.GetById(lodId)

            findingIds.Add(lod.FinalFindings)

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
                Dim appealId = LODAppealDao.GetAppealIdByInitLod(lodId)
                Dim appeal = LODAppealDao.GetById(appealId)

                If (appeal.Status = AppealWorkStatus.AppealApproved) Then
                    Return True
                End If

            End If

            If (LookupService.GetHasAppealSARC(lodId, workflowId)) Then
                Dim appealId = SARCAppealDao.GetAppealIdByInitId(lodId, workflowId)
                Dim appeal = SARCAppealDao.GetById(appealId)

                If (appeal.Status = SARCAppealWorkStatus.Approved) Then
                    Return True
                End If

            End If

            ' No findings were found to be in favor of the member...
            Return False
        End Function

        Protected Function IsSARCInFavorOfMember(lodId As Integer, workflowId As Integer) As Boolean
            Dim findingIds As List(Of Integer) = New List(Of Integer)()

            Dim sarc = SARCDao.GetById(lodId)

            findingIds.Add(sarc.FinalFindings)

            For Each id As Integer In findingIds
                If (id = ALOD.Core.Utils.Finding.In_Line_Of_Duty OrElse id = ALOD.Core.Utils.Finding.Epts_Service_Aggravated) Then
                    Return True
                End If
            Next

            If (LookupService.GetHasAppealSARC(lodId, workflowId)) Then
                Dim appealId = SARCAppealDao.GetAppealIdByInitId(lodId, workflowId)
                Dim appeal = SARCAppealDao.GetById(appealId)

                If (appeal.Status = SARCAppealWorkStatus.Approved) Then
                    Return True
                End If

            End If

            ' No findings were found to be in favor of the member...
            Return False
        End Function

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
            If (Not Page.IsPostBack) Then
                InitControls()
            End If
        End Sub

        Protected Sub RunReport()
            bllErrors.Items.Clear()
            trErrors.Visible = False

            If (Not UserHasPermission(PERMISSION_SARC_LODSARCCASES_REPORT_UNRESTRICTED) AndAlso Not UserHasPermission(PERMISSION_SARC_LODSARCCASES_REPORT_ALL)) Then
                bllErrors.Items.Add("You do not have permission to run this report.")
                trErrors.Visible = True
                Exit Sub
            End If

            If (Not Report.ExecuteReport(ConstructReportArguments())) Then
                bllErrors.Items.Add("The report failed to execute.")
                trErrors.Visible = True
                Exit Sub
            End If

            ' Determine how the repot results should be output...
            If (rdbOutputExcel.Checked) Then
                DisplayAppeal(False)
                ResultsToExcel()
            ElseIf (rdbOutputPdf.Checked) Then
                DisplayAppeal(False)
                ResultsToPDF()
            ElseIf (rdbOutputCsv.Checked) Then
                DisplayAppeal(False)
                ResultsToCSV()
            ElseIf (rdbOutputXml.Checked) Then
                DisplayAppeal(False)
                ResultsToXml()
            Else
                ResultsToBrowser()
                DisplayAppeal(True)
            End If
        End Sub

#Region "UI Procedures..."

        Protected Sub CreateSARCAppealForLineOfDuty(ByVal lod As LineOfDuty)
            Dim member As ServiceMember = Nothing
            Dim appealId As Integer = 0
            appealId = SARCAppealDao.GetAppealIdByInitId(lod.Id, lod.Workflow)

            If Not (appealId = 0) Then
                Dim ap As SARCAppeal = SARCAppealDao.GetById(appealId)
                If (ap IsNot Nothing AndAlso Not ap.WorkflowStatus.StatusCodeType.IsFinal) Then
                    GridViewErrorLabel.Text = "The selected case (" & lod.CaseId & ") already has an active SARC Appeal in progress (" & ap.CaseId & ")."
                    Exit Sub
                End If
            End If

            member = ServiceMemberDao.GetById(lod.MemberSSN)

            If (member Is Nothing) Then
                GridViewErrorLabel.Text = "Could not find the Service Member of the selected case."
                Exit Sub
            End If

            If (SESSION_GROUP_ID = UserGroups.WingSarc OrElse SESSION_GROUP_ID = UserGroups.RSL) Then
                Dim sarcAppealArgs = New SARCAppealInitiateCaseArgs(SARCAppealDao)

                sarcAppealArgs.OriginalCaseRefId = lod.Id
                sarcAppealArgs.OriginalCaseWorkflowId = lod.Workflow
                sarcAppealArgs.OriginalCaseCaseId = lod.CaseId
                sarcAppealArgs.Member = member
                sarcAppealArgs.CreatedByUserId = SESSION_USER_ID
                sarcAppealArgs.DocumentDao = DocumentDao

                Dim newAppealRequest As New SARCAppeal()

                newAppealRequest.InitiateCase(sarcAppealArgs)

                SARCAppealDao.Save(newAppealRequest)

                If (newAppealRequest.Id > 0) Then

                    Dim id As Integer = newAppealRequest.Id
                    Dim status As Integer = newAppealRequest.Status

                    LogManager.LogAction(ModuleType.AppealRequest, UserAction.RequestedAppeal, id, "Workflow: SARC Appeal Request", status)

                    ViewState("oldRefId") = lod.Id
                    ViewState("refId") = id
                    ViewState("refStatus") = status

                    SigBlock.StartSignature(id, newAppealRequest.Workflow, 0, "Initiate SARC Appeal", status, status, 0, DBSignTemplateId.Form348APSARC, "")
                End If
            Else
                GridViewErrorLabel.Text = "SARC Appeal Requests must be initiated by the Wing SARC or RSL."
                Exit Sub
            End If
        End Sub

        Protected Sub CreateSARCAppealForSARC(ByVal sarc As RestrictedSARC)
            Dim member As ServiceMember = Nothing
            Dim appealId As Integer = 0
            appealId = SARCAppealDao.GetAppealIdByInitId(sarc.Id, sarc.Workflow)

            If Not (appealId = 0) Then
                Dim ap As SARCAppeal = SARCAppealDao.GetById(appealId)
                If (ap IsNot Nothing AndAlso Not ap.WorkflowStatus.StatusCodeType.IsFinal) Then
                    GridViewErrorLabel.Text = "The selected case (" & sarc.CaseId & ") already has an active SARC Appeal in progress (" & ap.CaseId & ")."
                    Exit Sub
                End If
            End If

            member = ServiceMemberDao.GetById(sarc.MemberSSN)

            If (member Is Nothing) Then
                GridViewErrorLabel.Text = "Could not find the Service Member of the selected case."
                Exit Sub
            End If

            If (SESSION_GROUP_ID = UserGroups.WingSarc Or SESSION_GROUP_ID = UserGroups.RSL) Then
                Dim sarcAppealArgs = New SARCAppealInitiateCaseArgs(SARCAppealDao)

                sarcAppealArgs.OriginalCaseRefId = sarc.Id
                sarcAppealArgs.OriginalCaseWorkflowId = sarc.Workflow
                sarcAppealArgs.OriginalCaseCaseId = sarc.CaseId
                sarcAppealArgs.Member = member
                sarcAppealArgs.CreatedByUserId = SESSION_USER_ID
                sarcAppealArgs.DocumentDao = DocumentDao

                Dim newAppealRequest As New SARCAppeal()

                newAppealRequest.InitiateCase(sarcAppealArgs)

                SARCAppealDao.Save(newAppealRequest)

                If (newAppealRequest.Id > 0) Then

                    Dim id As Integer = newAppealRequest.Id
                    Dim status As Integer = newAppealRequest.Status

                    LogManager.LogAction(ModuleType.AppealRequest, UserAction.RequestedAppeal, id, "Workflow: SARC Appeal Request", status)

                    ViewState(KEY_OLDREFID) = sarc.Id
                    ViewState(KEY_REFID) = id
                    ViewState(KEY_REFSTATUS) = status

                    SigBlock.StartSignature(id, newAppealRequest.Workflow, 0, "Initiate SARC Appeal", status, status, 0, DBSignTemplateId.Form348APSARC, "")
                End If
            Else
                GridViewErrorLabel.Text = "SARC Appeal Requests must be initiated by the Wing SARC or RSL."
                Exit Sub
            End If
        End Sub

        Protected Sub gdvResults_PageIndexChanging(sender As Object, e As System.Web.UI.WebControls.GridViewPageEventArgs) Handles gdvResults.PageIndexChanging
            gdvResults.PageIndex = e.NewPageIndex
            RunReport()
        End Sub

        Protected Sub gdvResults_RowCommand(sender As Object, e As GridViewCommandEventArgs) Handles gdvResults.RowCommand
            If e.CommandName = "appeal" Then
                Dim args = e.CommandArgument.ToString().Split(";")

                If (args(1) = AFRCWorkflows.LOD) Then
                    CreateSARCAppealForLineOfDuty(LodDao.GetById(args(0)))
                ElseIf (args(1) = AFRCWorkflows.SARCRestricted) Then
                    CreateSARCAppealForSARC(SARCDao.GetById(args(0)))
                End If
            End If
        End Sub

        Protected Sub gdvResults_RowDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewRowEventArgs) Handles gdvResults.RowDataBound
            If (e.Row.RowType <> DataControlRowType.DataRow OrElse Report Is Nothing) Then
                Exit Sub
            End If

            ' Check if this row is being edited or not
            If e.Row.RowIndex <> gdvResults.EditIndex Then
                Dim resultItem As LODSARCCasesReportResultItem = Nothing

                resultItem = Report.Results.Where(Function(x) x.RefId = CInt(gdvResults.DataKeys(e.Row.RowIndex).Value)).First()

                If (resultItem Is Nothing) Then
                    Exit Sub
                End If

                CType(e.Row.FindControl("lblCaseId"), Label).Text = resultItem.CaseId
                CType(e.Row.FindControl("lblIsRestricted"), Label).Text = GetIsRestrictedLabelText(resultItem.IsRestricted)
                CType(e.Row.FindControl("lblStatus"), Label).Text = resultItem.StatusCodeName
                CType(e.Row.FindControl("lblMemberName"), Label).Text = resultItem.MemberName
                CType(e.Row.FindControl("lblSSN"), Label).Text = "***-**-" + resultItem.MemberPartialSSN
                CType(e.Row.FindControl("lblMemberUnit"), Label).Text = resultItem.MemberUnit
            End If
        End Sub

        Protected Function GetIsRestrictedLabelText(ByVal isRestricted As Boolean) As String
            If (isRestricted) Then
                Return "Yes"
            Else
                Return "No"
            End If
        End Function

        Protected Function GetNextAPCaseId(ByVal lodCaseId As String, ByVal lodId As Integer, ByVal workflowId As Integer) As String
            Dim caseId As String = String.Empty
            Dim caseCount As Integer = 0
            Dim leadingZeroes As Integer = 0

            lodCaseId = lodCaseId.Replace("-SA", "")

            ' Get the current number of RR cases which exist with the specificed lodId and then add one...
            caseCount = SARCAppealDao.GetAppealCount(lodId, workflowId) + 1

            ' Figure out how many leading zeros will be needed for the case Id...
            leadingZeroes = 3 - caseCount.ToString().Length

            caseId = lodCaseId + "-APSA-"

            For i As Integer = 1 To leadingZeroes
                caseId = caseId + "0"
            Next

            caseId = caseId + caseCount.ToString()

            Return caseId

        End Function

        Protected Sub ResultsToBrowser()

            If (Report Is Nothing OrElse Not Report.HasExecuted) Then
                bllErrors.Items.Add("The report failed to execute.")
                trErrors.Visible = True
                pnlResults.Visible = False
                Exit Sub
            End If

            If (Report.Results.Count = 0) Then
                lblEmptyResults.Text = "No Results Found."
                pnlResults.Visible = True
                pnlEmptyResults.Visible = True

                gdvResults.DataSource = Report.Results.OrderBy(Function(y) y.CaseId).ToList()
                gdvResults.DataBind()

                Exit Sub
            End If

            pnlResults.Visible = True
            pnlEmptyResults.Visible = False

            gdvResults.DataSource = Report.Results.OrderBy(Function(y) y.CaseId).ToList()
            gdvResults.DataBind()
        End Sub

        Protected Sub SignatureCompleted(ByVal sender As Object, ByVal e As SignCompletedEventArgs) Handles SigBlock.SignCompleted
            If (e.SignaturePassed) Then
                SARCAppealDao.CommitChanges()

                ReminderEmailDao.ReminderEmailInitialStep(ViewState(KEY_REFID), ViewState(KEY_REFSTATUS), "APSA")

                Response.Redirect("~/Secure/SARC_Appeal/init.aspx?requestId=" & ViewState(KEY_REFID).ToString())
            End If
        End Sub

#End Region

#Region "Excel Procedures..."

        Protected Sub ResultsToExcel()
            If (Report Is Nothing OrElse Not Report.HasExecuted) Then
                bllErrors.Items.Add("The report failed to execute.")
                trErrors.Visible = True
                Exit Sub
            End If

            ResultsToBrowser()

            If (Report.Results.Count = 0) Then
                lblEmptyResults.Text &= "..No results to export to EXCEL."
                Exit Sub
            End If

            ' Populate excel gridviews with report results data...
            Dim table As New HtmlTable

            BuildExcelTable(table)

            ' Render HTML and send as an excel file...
            Dim writer As New System.IO.StringWriter
            Dim html As New HtmlTextWriter(writer)
            table.RenderControl(html)

            Response.Clear()
            Response.AddHeader("content-disposition", "attachment;filename=" + OutputFileName + ".xls")
            Response.Charset = ""
            Response.ContentType = "application/ms-excel"
            Response.Write(writer.ToString())
            Response.End()
        End Sub

        Private Sub AddTableCell(ByVal row As HtmlTableRow, ByVal text As String, ByVal style As String)
            Dim cell As New HtmlTableCell
            cell.InnerHtml = text

            If (Not String.IsNullOrEmpty(style)) Then
                cell.Attributes.Add("style", style)
            End If

            row.Cells.Add(cell)
        End Sub

        Private Sub BuildExcelTable(ByVal table As HtmlTable)
            Dim odd As Boolean = False
            Dim row As New HtmlTableRow()
            Dim currentLabel As Label
            Dim currentLinkButton As LinkButton
            Dim currentColumnId As Integer = -1

            ' Add header row...
            row.Attributes.Add("style", EXCEL_HEADER_ROW_STYLE)

            For Each cell As TableCell In gdvResults.HeaderRow.Cells
                currentColumnId = currentColumnId + 1

                If (Not gdvResults.Columns(currentColumnId).Visible) Then
                    Continue For
                End If

                AddTableCell(row, cell.Text, String.Empty)
            Next

            table.Rows.Add(row)

            ' Add data rows...
            For Each gridRow As GridViewRow In gdvResults.Rows
                row = New HtmlTableRow
                Dim bgColor As String = IIf(odd, "#d8d8ff;", "#FFF;")
                odd = Not odd

                row.Attributes.Add("style", EXCEL_DATA_ROW_STYLE + bgColor)
                table.Rows.Add(row)

                currentColumnId = -1

                For Each cell As TableCell In gridRow.Cells
                    currentColumnId = currentColumnId + 1

                    If (Not gdvResults.Columns(currentColumnId).Visible) Then
                        Continue For
                    End If

                    If (cell.Controls.Count > 0) Then
                        If (TypeOf cell.Controls(1) Is Label) Then
                            currentLabel = CType(cell.Controls(1), Label)

                            AddTableCell(row, currentLabel.Text, GetCellStyle(currentLabel.CssClass, currentLabel.Text))

                            currentLabel = Nothing
                        ElseIf (TypeOf cell.Controls(1) Is LinkButton) Then
                            currentLinkButton = CType(cell.Controls(1), LinkButton)

                            AddTableCell(row, currentLinkButton.Text, GetCellStyle(currentLinkButton.CssClass, currentLinkButton.Text))

                            currentLinkButton = Nothing
                        End If
                    Else
                        AddTableCell(row, cell.Text, String.Empty)
                    End If
                Next
            Next
        End Sub

        Private Function GetCellStyle(ByVal cssClasses As String, ByVal cellValue As String) As String
            Dim style As String = String.Empty

            'If (cssClasses.Contains("numericResult")) Then
            '    style &= EXCEL_TEXT_BOLD_STYLE
            'End If

            'If (cellValue.Equals("*")) Then
            '    style = style + EXCEL_TEXT_ALIGN_STYLE
            'End If

            Return style
        End Function

#End Region

#Region "PDF Procedures..."

        Protected Sub ResultsToPDF()
            If (Report Is Nothing OrElse Not Report.HasExecuted) Then
                bllErrors.Items.Add("The report failed to execute.")
                trErrors.Visible = True
                Exit Sub
            End If

            ResultsToBrowser()

            If (Report.Results.Count = 0) Then
                lblEmptyResults.Text &= "..No results to export to PDF."
                Exit Sub
            End If

            ' Populate PDF file with report results data...
            Dim pdf As New Doc()
            Dim currentLabel As Label
            Dim currentLinkButton As LinkButton
            Dim currentColumnId As Integer = -1
            Dim visibleColumnsCount As Integer = 0
            Dim colNames As New StringCollection()
            Dim table As PdfTable
            Dim i As Integer = 0
            Dim page As Integer = 1

            ' Determine how many columns of the results gridview are visible
            For Each c As DataControlField In gdvResults.Columns
                If (c.Visible) Then
                    visibleColumnsCount = visibleColumnsCount + 1
                End If
            Next

            ' We need to be in landscape to allow as much width as possible; therefor, apply a rotation transform (http://www.websupergoo.com/helppdfnet/source/4-examples/08-landscape.htm)...
            Dim w As Double = pdf.MediaBox.Width
            Dim h As Double = pdf.MediaBox.Height
            Dim l As Double = pdf.MediaBox.Left
            Dim b As Double = pdf.MediaBox.Bottom

            pdf.Transform.Rotate(90, l, b)
            pdf.Transform.Translate(w, 0)

            ' Rotate PDF rectangle...
            pdf.Rect.Width = h
            pdf.Rect.Height = w

            ' Rotate the page...
            Dim docId As Integer = pdf.GetInfoInt(pdf.Root, "Pages")
            pdf.SetInfo(docId, "/Rotate", "90")

            pdf.FontSize = 7
            pdf.Rect.Inset(10, 40)

            table = New PdfTable(pdf, visibleColumnsCount)
            table.CellPadding = 1
            table.RepeatHeader = True

            ' Header rows....
            table.NextRow()
            For Each cell As TableCell In gdvResults.HeaderRow.Cells
                currentColumnId = currentColumnId + 1

                If (Not gdvResults.Columns(currentColumnId).Visible) Then
                    Continue For
                End If

                colNames.Add(cell.Text)
            Next

            Dim header(colNames.Count) As String
            colNames.CopyTo(header, 0)

            table.AddHtml(header)

            ' Add data rows...
            For Each gridRow As GridViewRow In gdvResults.Rows
                table.NextRow()

                Dim rowCells As New List(Of PDFTableHTMLData)()

                currentColumnId = -1

                For Each cell As TableCell In gridRow.Cells
                    currentColumnId = currentColumnId + 1

                    If (Not gdvResults.Columns(currentColumnId).Visible) Then
                        Continue For
                    End If

                    If (cell.Controls.Count > 0) Then
                        If (TypeOf cell.Controls(1) Is Label) Then
                            currentLabel = CType(cell.Controls(1), Label)

                            rowCells.Add(New PDFTableHTMLData(currentLabel.Text, GetCellColor(currentLabel.CssClass), GetCellBold(currentLabel.CssClass)))

                            currentLabel = Nothing
                        ElseIf (TypeOf cell.Controls(1) Is LinkButton) Then
                            currentLinkButton = CType(cell.Controls(1), LinkButton)

                            rowCells.Add(New PDFTableHTMLData(currentLinkButton.Text, GetCellColor(currentLinkButton.CssClass), GetCellBold(currentLinkButton.CssClass)))

                            currentLinkButton = Nothing
                        End If
                    Else
                        rowCells.Add(New PDFTableHTMLData(cell.Text, String.Empty, False, 0.0))
                    End If
                Next

                table.AddHtml(rowCells)

                If (pdf.PageNumber > page) Then
                    page = pdf.PageNumber
                End If
            Next

            table.FrameRows()
            table.FrameColumns()

            pdf.VPos = 0.5

            For ct As Integer = 1 To pdf.PageCount
                pdf.PageNumber = ct

                'left side
                pdf.HPos = 0.0
                pdf.Rect.SetRect(20, 580, 280, 20)
                pdf.AddText("")

                'middle
                pdf.HPos = 0.5
                pdf.Rect.SetRect(20, 580, 750, 20)
                pdf.AddText("LOD SARC Cases Report")

                'right
                pdf.HPos = 1.0
                pdf.Rect.SetRect(500, 580, 270, 20)
                pdf.AddText("Generated: " + DateTime.Now.ToString(DATE_HOUR_FORMAT))

                'page number
                pdf.HPos = 0.5
                pdf.Rect.SetRect(20, 10, 750, 10)
                pdf.AddText("Page " + ct.ToString() + " of " + pdf.PageCount.ToString())

                'table header
                'pdf.AddLine(10, 550, 782, 550)
            Next

            pdf.Flatten()
            Dim buffer() As Byte = pdf.GetData()

            Response.ContentType = "application/pdf"
            Response.AddHeader("content-disposition", "attachment;filename=" + OutputFileName + ".pdf")
            Response.AddHeader("content-length", buffer.Length.ToString())
            Response.BinaryWrite(buffer)
            Response.End()
        End Sub

        Private Function GetCellBold(ByVal cssClasses As String) As String
            Return False
        End Function

        Private Function GetCellColor(ByVal cssClasses As String) As String
            Dim color As String = String.Empty

            Return color
        End Function

#End Region

#Region "XML Procedures..."

        Private Sub BuildXmlData(ByVal data As DataTable)
            Dim currentColumnId As Integer = -1

            For Each cell As TableCell In gdvResults.HeaderRow.Cells
                currentColumnId = currentColumnId + 1

                If (Not gdvResults.Columns(currentColumnId).Visible) Then
                    Continue For
                End If

                data.Columns.Add(cell.Text)
            Next

            Dim row As DataRow
            Dim i As Integer = 0

            For Each gridRow As GridViewRow In gdvResults.Rows
                row = data.NewRow()

                i = 0
                currentColumnId = -1

                For Each cell As TableCell In gridRow.Cells
                    currentColumnId = currentColumnId + 1

                    If (Not gdvResults.Columns(currentColumnId).Visible) Then
                        Continue For
                    End If

                    If (cell.Controls.Count > 0) Then
                        If (TypeOf cell.Controls(1) Is Label) Then
                            row(i) = CType(cell.Controls(1), Label).Text
                        ElseIf (TypeOf cell.Controls(1) Is LinkButton) Then
                            row(i) = CType(cell.Controls(1), LinkButton).Text
                        End If
                    Else
                        row(i) = cell.Text
                    End If

                    i = i + 1
                Next

                data.Rows.Add(row)
            Next
        End Sub

        Private Sub ResultsToXml()
            If (Report Is Nothing OrElse Not Report.HasExecuted) Then
                bllErrors.Items.Add("The report failed to execute.")
                trErrors.Visible = True
                Exit Sub
            End If

            ResultsToBrowser()

            If (Report.Results.Count = 0) Then
                lblEmptyResults.Text &= "..No results to export to XML."
                Exit Sub
            End If

            ' Populate XML with report results data...
            Dim data As DataTable = New DataTable()

            BuildXmlData(data)

            Response.Clear()
            Response.AddHeader("content-disposition", "attachment;filename=" + OutputFileName + ".xml")
            Response.Charset = ""
            Response.ContentType = "text/xml"
            Response.Write("<?xml version=""1.0"" encoding=""utf-8"" ?>" + Environment.NewLine)

            data.TableName = "ReportResults"
            data.WriteXml(Response.OutputStream)

            Response.End()
        End Sub

#End Region

#Region "CSV Procedures..."

        Private Sub BuildCSVData(ByVal buffer As StringBuilder)
            Dim isFirst As Boolean = True
            Dim currentColumnId As Integer = -1

            For Each cell As TableCell In gdvResults.HeaderRow.Cells
                currentColumnId = currentColumnId + 1

                If (Not gdvResults.Columns(currentColumnId).Visible) Then
                    Continue For
                End If

                If (Not isFirst) Then
                    buffer.Append(",")
                Else
                    isFirst = False
                End If

                buffer.Append(cell.Text)
            Next

            buffer.Append(Environment.NewLine)

            For Each gridRow As GridViewRow In gdvResults.Rows
                isFirst = True

                currentColumnId = -1

                For Each cell As TableCell In gridRow.Cells
                    currentColumnId = currentColumnId + 1

                    If (Not gdvResults.Columns(currentColumnId).Visible) Then
                        Continue For
                    End If

                    If (Not isFirst) Then
                        buffer.Append(",")
                    Else
                        isFirst = False
                    End If

                    If (cell.Controls.Count > 0) Then
                        If (TypeOf cell.Controls(1) Is Label) Then
                            buffer.Append(CType(cell.Controls(1), Label).Text)
                        ElseIf (TypeOf cell.Controls(1) Is LinkButton) Then
                            buffer.Append(CType(cell.Controls(1), LinkButton).Text)
                        End If
                    Else
                        buffer.Append(cell.Text)
                    End If
                Next

                buffer.Append(Environment.NewLine)
            Next
        End Sub

        Private Sub ResultsToCSV()
            If (Report Is Nothing OrElse Not Report.HasExecuted) Then
                bllErrors.Items.Add("The report failed to execute.")
                trErrors.Visible = True
                Exit Sub
            End If

            ResultsToBrowser()

            If (Report.Results.Count = 0) Then
                lblEmptyResults.Text &= "..No results to export to CSV."
                Exit Sub
            End If

            ' Populate CSV with report results data...
            Dim buffer As New StringBuilder()

            BuildCSVData(buffer)

            Response.Clear()
            Response.AddHeader("content-disposition", "attachment;filename=" + OutputFileName + ".csv")
            Response.Charset = ""
            Response.ContentType = "text/plain"
            Response.Write(buffer.ToString())
            Response.End()
        End Sub

#End Region

        Private Function IsMemberTheUser(refId As Integer, moduleId As Integer) As Boolean
            Dim memberSsn As String

            If (moduleId = ModuleType.LOD) Then
                memberSsn = LodDao.GetById(refId).MemberSSN
            Else
                memberSsn = SARCDao.GetById(refId).MemberSSN
            End If

            Dim member As ServiceMember = ServiceMemberDao.GetById(memberSsn)

            Return ApplicationUser.IsPersonnalServiceMemberRecord(member)
        End Function

    End Class

End Namespace