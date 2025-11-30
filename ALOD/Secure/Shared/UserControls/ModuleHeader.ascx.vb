Imports ALOD.Core.Domain.Modules.Appeals
Imports ALOD.Core.Domain.Modules.Lod
Imports ALOD.Core.Domain.Modules.Reinvestigations
Imports ALOD.Core.Domain.Modules.SARC
Imports ALOD.Core.Domain.Modules.SpecialCases
Imports ALOD.Core.Domain.Workflow
Imports ALOD.Core.Interfaces.DAOInterfaces
Imports ALOD.Data
Imports ALOD.Data.Services
Imports ALODWebUtility.Common
Imports ALODWebUtility.Permission.Search

Namespace Web.UserControls

    Partial Class ModuleHeader
        Inherits System.Web.UI.UserControl

#Region "Fields"

        Public Const MSG_REINVESTIGATION As String = "This LOD is a reinvestigation of a closed case.  The orignal case number was: "

        Public sKeyWord As String
        Public sReplacement As String
        Private _associatedCaseDao As IAssociatedCaseDao
        Private _daoFactory As IDaoFactory
        Private _lodAppealDao As ILODAppealDAO
        Private _modHeader As String = "LOD"
        Private _reinvestigationDao As ILODReinvestigateDAO
        Private _sarcAppealDao As ISARCAppealDAO
        Private _sarcDao As ISARCDAO
        Private _specCaseDao As ISpecialCaseDAO
        Private _workflowDao As IWorkflowDao

#End Region

#Region "Properties"

        Public Property ModuleHeader As String
            Get
                Return _modHeader
            End Get
            Set(ByVal value As String)
                _modHeader = value
            End Set
        End Property

        Protected ReadOnly Property Associated() As IAssociatedCaseDao
            Get
                If (_associatedCaseDao Is Nothing) Then
                    _associatedCaseDao = DaoFactory.GetAssociatedCaseDao()
                End If
                Return _associatedCaseDao
            End Get
        End Property

        Protected ReadOnly Property DaoFactory() As IDaoFactory
            Get
                If (_daoFactory Is Nothing) Then
                    _daoFactory = New NHibernateDaoFactory()
                End If

                Return _daoFactory
            End Get
        End Property

        Protected ReadOnly Property LODAppealDao As ILODAppealDAO
            Get
                If (_lodAppealDao Is Nothing) Then
                    _lodAppealDao = DaoFactory.GetLODAppealDao()
                End If

                Return _lodAppealDao
            End Get
        End Property

        Protected ReadOnly Property ReinvestigationDao As ILODReinvestigateDAO
            Get
                If (_reinvestigationDao Is Nothing) Then
                    _reinvestigationDao = DaoFactory.GetLODReinvestigationDao()
                End If

                Return _reinvestigationDao
            End Get
        End Property

        Protected ReadOnly Property SARCAppealDao As ISARCAppealDAO
            Get
                If (_sarcAppealDao Is Nothing) Then
                    _sarcAppealDao = DaoFactory.GetSARCAppealDao()
                End If

                Return _sarcAppealDao
            End Get
        End Property

        Protected ReadOnly Property SARCDao As ISARCDAO
            Get
                If (_sarcDao Is Nothing) Then
                    _sarcDao = DaoFactory.GetSARCDao()
                End If

                Return _sarcDao
            End Get
        End Property

        Protected ReadOnly Property SpecCaseDao As ISpecialCaseDAO
            Get
                If (_specCaseDao Is Nothing) Then
                    _specCaseDao = DaoFactory.GetSpecialCaseDAO()
                End If

                Return _specCaseDao
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

#Region "Page Methods"

        Public Sub RenameStatus()
            lblStatus.Text = lblStatus.Text.Replace(sKeyWord, sReplacement)
        End Sub

        Public Sub UpdateDisplay()
            Dim refId As Integer = 0
            Dim requestId As Integer = 0

            refId = CInt(Session("RefId"))
            requestId = CInt(Session("RequestId"))

            If ((requestId = 0) And (refId <> 0)) Then
                requestId = refId
            End If

            Select Case ModuleHeader
                Case "LOD"
                    ProcessLOD(refId)

                Case "RR"
                    ProcessRR(requestId)

                Case "AP"
                    ProcessAP(requestId)

                Case "SARC"
                    ProcessSARC(refId)

                Case "APSA"
                    ProcessAPSA(requestId)

                Case Else
                    ProcessSpecialCase(refId)

            End Select
            LockImage.Visible = Not SESSION_LOCK_AQUIRED

            If sKeyWord <> "" Then RenameStatus()

        End Sub

        Protected Function AdminCaseLbl(ByVal moduleId As Integer) As String

            If (moduleId = ModuleType.LOD) Then
                Return "Associated LOD: Admin LOD"
            ElseIf (moduleId = ModuleType.SARC) Then
                Return "Associated SARC: Admin SARC"
            Else
                Return "Associated SC: Admin Special Case"
            End If

        End Function

        Protected Sub CanViewLOD(ByVal LOD As LineOfDuty, ByVal e As System.Web.UI.WebControls.GridViewRowEventArgs)

            If (LOD.Formal) Then
                If (UserHasPermission(PERMISSION_VIEW_FORMAL_LOD)) Then
                    CType(e.Row.FindControl("lnkRefID"), LinkButton).Visible = True
                Else
                    CType(e.Row.FindControl("RefIdlbl"), Label).Visible = True
                End If
            Else
                CType(e.Row.FindControl("lnkRefID"), LinkButton).Visible = True
            End If

        End Sub

        Protected Function CaseLbl(ByVal moduleId As Integer) As String
            If (moduleId = ModuleType.LOD) Then
                Return "Associated LOD: "
            ElseIf (moduleId = ModuleType.SARC) Then
                Return "Associated SARC: "
            Else
                Return "Associated SC: "
            End If
        End Function

        Protected Sub DisplayAssociatedCase(ByVal CaseId As String, ByVal refId As Integer, ByVal workflowId As Integer)

            Dim moduleId As Integer = WorkflowDao.GetModuleFromWorkflow(workflowId)

            If refId <> 0 Then  'Gets LODs for Special Cases with Associated LODs, for LODs, and Reinvestigation Requests

                If (WorkflowDao.CanViewWorkflow(SESSION_GROUP_ID, workflowId, GetIsFormal(refId, workflowId))) Then
                    lblAssociatedCase.Text = CaseLbl(moduleId)
                    lnkAssociatedCase.Text = CaseId
                    lnkAssociatedCase.CommandArgument = moduleId & "," & refId.ToString()
                    lnkAssociatedCase.Visible = True
                Else
                    lblAssociatedCase.Text = CaseLbl(moduleId) + CaseId
                End If
            Else
                lblAssociatedCase.Text = AdminCaseLbl(moduleId)
            End If

            divAssociatedCase.Style.Remove("visibility")

        End Sub

        Protected Sub DisplayAssociatedCase2(ByVal CaseId As String, ByVal refId As Integer, ByVal workflowId As Integer)

            Dim moduleId As Integer = WorkflowDao.GetModuleFromWorkflow(workflowId)

            If refId <> 0 Then  'Gets LODs for Special Cases with Associated LODs, for LODs, and Reinvestigation Requests

                If (WorkflowDao.CanViewWorkflow(SESSION_GROUP_ID, workflowId, False)) Then
                    lblAssociatedCase2.Text = CaseLbl(moduleId)
                    lnkAssociatedCase2.Text = CaseId
                    lnkAssociatedCase2.CommandArgument = moduleId & "," & refId.ToString()
                    lnkAssociatedCase2.Visible = True
                Else
                    lblAssociatedCase2.Text = CaseLbl(moduleId) + CaseId
                End If
            Else
                lblAssociatedCase2.Text = AdminCaseLbl(moduleId)
            End If

            divAssociatedCase2.Style.Remove("visibility")

        End Sub

        Protected Function GetIsFormal(refId As Integer, workflowId As Integer) As Boolean
            Dim moduleId As Integer = WorkflowDao.GetModuleFromWorkflow(workflowId)

            If (moduleId = ModuleType.LOD) Then

                Dim LOD = LodService.GetById(refId)

                Return LOD.Formal

            End If

            Return False

        End Function

        Protected Function GetOriginalCaseId(ByVal caseId As String) As String

            If (caseId Is Nothing) OrElse (caseId.Length = 0) Then
                Return String.Empty
            End If

            Dim index As Integer = caseId.LastIndexOf("-")

            If (index = 0) Then
                Return caseId
            End If

            Return caseId.Substring(0, index)

        End Function

        Protected Sub GridView1_ItemDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewRowEventArgs) Handles GridView1.RowDataBound, GridView2.RowDataBound

            If (e.Row.RowType = DataControlRowType.DataRow) Then
                Dim data As AssociatedCase = e.Row.DataItem

                If (Not data.associated_RefId > 0) Then
                    CType(e.Row.FindControl("RefIdlbl"), Label).Visible = True
                Else

                    If (WorkflowDao.CanViewWorkflow(SESSION_GROUP_ID, data.associated_workflowId, GetIsFormal(data.associated_RefId, data.associated_workflowId))) Then
                        If (data.associated_workflowId = AFRCWorkflows.LOD OrElse data.associated_workflowId = AFRCWorkflows.LOD_v2) Then
                            CanViewLOD(LodService.GetById(data.associated_RefId), e)
                        Else
                            CType(e.Row.FindControl("lnkRefID"), LinkButton).Visible = True
                        End If
                    Else
                        CType(e.Row.FindControl("RefIdlbl"), Label).Visible = True
                    End If

                End If
            End If
        End Sub

        Protected Sub GridView1_RowCommand(sender As Object, e As GridViewCommandEventArgs) Handles GridView1.RowCommand, GridView2.RowCommand
            If (e.CommandName = "view") Then
                Dim parts() As String = e.CommandArgument.ToString().Split(",")
                Dim strQuery As New StringBuilder()
                Dim args As New ItemSelectedEventArgs

                args.RefId = parts(0)
                args.Type = WorkflowDao.GetModuleFromWorkflow(parts(1))

                args.Url = GetWorkflowInitPageURL(args.Type, args.RefId)
                Session("RequestId") = -1
                Response.Redirect(args.Url)

            End If
        End Sub

        Protected Sub lnkAssociatedCase_Command(sender As Object, e As CommandEventArgs) Handles lnkAssociatedCase.Command, lnkAssociatedCase2.Command
            Dim args As String() = e.CommandArgument.ToString().Split(",")

            Session("RequestId") = -1
            Response.Redirect(GetWorkflowInitPageURL(Integer.Parse(args(0)), Integer.Parse(args(1))))
        End Sub

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
            If (Not IsPostBack) Then
                UpdateDisplay()
            End If
        End Sub

        Protected Sub ProcessAP(ByVal requestId As Integer)
            Dim AP As LODAppeal = LODAppealDao.GetById(requestId)
            Dim refId As Integer = AP.InitialLodId
            SetLabels(AP.CaseId, AP.WorkflowStatus.Description, AP.MemberName + " (" + AP.MemberSSN.Substring(5) + ")")

            If (refId <> 0) Then
                Dim LOD = LodService.GetById(refId)
                DisplayAssociatedCase(LOD.CaseId, LOD.Id, LOD.Workflow)
            End If
        End Sub

        Protected Sub ProcessAPSA(ByVal requestId As Integer)
            Dim APSA As SARCAppeal = SARCAppealDao.GetById(requestId)
            Dim refId As Integer = APSA.InitialId
            SetLabels(APSA.CaseId, APSA.WorkflowStatus.Description, APSA.MemberName + " (" + APSA.MemberSSN.Substring(5) + ")")

            If (refId <> 0) Then
                If (APSA.InitialWorkflow = AFRCWorkflows.LOD) Then
                    Dim LOD = LodService.GetById(refId)
                    DisplayAssociatedCase(LOD.CaseId, LOD.Id, LOD.Workflow)
                Else
                    Dim SARC = SARCDao.GetById(refId)
                    DisplayAssociatedCase(SARC.CaseId, SARC.Id, SARC.Workflow)
                End If

            End If

        End Sub

        Protected Sub ProcessLOD(ByVal refId As Integer)
            Dim LOD As LineOfDuty = LodService.GetById(refId)

            SetLabels(LOD.CaseId, LOD.WorkflowStatus.Description, LOD.MemberName + " (" + LOD.MemberSSN.Substring(5) + ")")

            If Not LOD Is Nothing Then

                If (LOD.SarcCase AndAlso LOD.IsRestricted) Then
                    lblCaseId.Text = HTMLEncodeNulls("Case # " + LOD.CaseId + " [RESTRICTED]")
                End If

                If (LOD.IsDeleted) Then
                    pnlDeleted.Visible = True
                    lblRestore.Visible = False
                    btnRestore.Visible = False

                End If

                If (LOD.ParentId.HasValue) Then
                    FeedbackMessageLabel.Text = MSG_REINVESTIGATION
                    FeedbackLink.Text = GetOriginalCaseId(LOD.CaseId)
                    FeedbackLink.NavigateUrl = "~/Secure/lod/init.aspx?refId=" + LOD.ParentId.Value.ToString()
                    FeedbackPanel.Visible = True
                Else
                    FeedbackLink.Text = String.Empty
                    FeedbackLink.NavigateUrl = String.Empty
                    FeedbackMessageLabel.Text = String.Empty
                    FeedbackPanel.Visible = False
                End If
            End If

        End Sub

        Protected Sub ProcessRR(ByVal requestId As Integer)
            Dim RR As LODReinvestigation = ReinvestigationDao.GetById(requestId)
            Dim refId As Integer = RR.InitialLodId
            SetLabels(RR.CaseId, RR.WorkflowStatus.Description, RR.MemberName + " (" + RR.MemberSSN.Substring(5) + ")")

            If (refId <> 0) Then
                Dim LOD = LodService.GetById(refId)
                DisplayAssociatedCase(LOD.CaseId, LOD.Id, LOD.Workflow)
            End If

        End Sub

        Protected Sub ProcessSARC(ByVal refId As Integer)
            Dim SARC As RestrictedSARC = SARCDao.GetById(refId)
            SetLabels(SARC.CaseId, SARC.WorkflowStatus.Description, SARC.MemberName + " (" + SARC.MemberSSN.Substring(5) + ")")

        End Sub

        Protected Sub ProcessSpecialCase(ByVal refId As Integer)

            Dim sc = SpecCaseDao.GetById(refId)
            Dim moduleId As Integer = WorkflowDao.GetModuleFromWorkflow(sc.Workflow)

            'For LOD associated Cases
            Dim cases As IList(Of AssociatedCase) = New List(Of AssociatedCase)

            If sc.HasAdminLOD = 1 Then
                Dim AdminLOD As AssociatedCase = New AssociatedCase(refId, sc.Workflow, -1, -1, "Admin LOD")
                cases.Add(AdminLOD)
            End If

            cases = cases.Concat(Associated.GetAssociatedCasesLOD(sc.Id, sc.Workflow)).ToList()

            'Display multiple associated LOD cases in the case header
            If (cases.Count > 1) Then
                GridView1.DataSource = cases
                GridView1.DataBind()

                lblAssociatedCase.Text = CaseLbl(ModuleType.LOD)
                divAssociatedCase.Style.Remove("visibility")
                MultipleCases.Visible = True

                refId = 0
            ElseIf (cases.Count = 1) Then

                If sc.HasAdminLOD = 1 Then
                    DisplayAssociatedCase("", 0, AFRCWorkflows.LOD_v2)
                Else
                    DisplayAssociatedCase(cases.First.associated_caseId, cases.First.associated_RefId, cases.First.associated_workflowId)
                End If

            End If

            cases.Clear()

            'For SC associated Cases
            If sc.HasAdminSC = 1 Then
                Dim AdminSC As AssociatedCase = New AssociatedCase(refId, sc.Workflow, -1, -1, "Admin Mod")
                cases.Add(AdminSC)
            End If

            cases = cases.Concat(Associated.GetAssociatedCasesSC(sc.Id, sc.Workflow)).ToList()

            'Display multiple associated Special cases in the case header
            If (cases.Count > 1) Then
                GridView2.DataSource = cases
                GridView2.DataBind()

                lblAssociatedCase2.Text = CaseLbl(sc.Workflow)
                divAssociatedCase2.Style.Remove("visibility")
                MultipleCases2.Visible = True

                refId = 0
            ElseIf (cases.Count = 1) Then

                If sc.HasAdminSC = 1 Then
                    DisplayAssociatedCase2("", 0, sc.Workflow)
                Else
                    DisplayAssociatedCase2(cases.First.associated_caseId, cases.First.associated_RefId, cases.First.associated_workflowId)
                End If
            End If

            Dim workStatus = sc.WorkflowStatus.Description

            If ModuleHeader = "BT" Then
                If Session("SubWorkflowType") = "" Then
                    Session("SubWorkflowType") = "(BMT) "
                End If
                workStatus = Session("SubWorkflowType") & workStatus
            End If

            If ModuleHeader = "PE" Then
                If Session("SubWorkflowType") = "" Then
                    Session("SubWorkflowType") = "(PEPP)"
                End If
                workStatus = Session("SubWorkflowType") & workStatus
            End If

            Dim MemberInfo As String = ""

            If (Not String.IsNullOrEmpty(sc.MemberName)) Then
                MemberInfo = sc.MemberName + " (" + sc.MemberSSN.Substring(5) + ")"
            End If

            SetLabels(sc.CaseId, sc.WorkflowStatus.Description, MemberInfo)

        End Sub

        Protected Sub SetLabels(ByVal CaseId As String, ByVal WorkStatus As String, ByVal NameAndSSn As String)
            lblCaseId.Text = HTMLEncodeNulls("Case # " + CaseId)
            lblSmName.Text = HTMLEncodeNulls(NameAndSSn)
            lblStatus.Text = HTMLEncodeNulls(WorkStatus)

            If (lblStatus.Text.Length > 40) Then
                lblStatus.Text = HTMLEncodeNulls(lblStatus.Text.Substring(0, 40) + "...)")
            End If

        End Sub

#End Region

    End Class

End Namespace