Imports ALOD.Core.Domain.Common
Imports ALOD.Core.Domain.Lookup
Imports ALOD.Core.Domain.Modules.Lod
Imports ALOD.Core.Domain.Users
Imports ALOD.Core.Interfaces.DAOInterfaces
Imports ALOD.Core.Utils
Imports ALOD.Data
Imports ALOD.Web.UserControls
Imports ALODWebUtility.Common

Namespace Web.LOD

    Partial Class Secure_lod_WingJA
        Inherits System.Web.UI.Page

        Protected Const optionalFindings As String = InvestigationDecision.APPROVE + "," + InvestigationDecision.DISAPPROVE
        Protected Const optionalInformalFindings As String = InvestigationDecision.APPROVE + "," + InvestigationDecision.DISAPPROVE + "," + InvestigationDecision.NOT_LOD_MISCONDUCT
        Private _lod As LineOfDuty
        Private _lodDao As ILineOfDutyDao
        Private _scAccess As Dictionary(Of String, ALOD.Core.Domain.Workflow.PageAccessType)

        Public ReadOnly Property Navigator() As TabNavigator
            Get
                Return MasterPage.Navigator
            End Get
        End Property

        Public ReadOnly Property TabControl() As TabControls
            Get
                Return Master.TabControl
            End Get
        End Property

        Public Property UserCanEdit() As Boolean
            Get
                If (ViewState("UserCanEdit") Is Nothing) Then
                    ViewState("UserCanEdit") = False
                End If
                Return CBool(ViewState("UserCanEdit"))
            End Get
            Set(value As Boolean)
                ViewState("UserCanEdit") = value
            End Set
        End Property

        Protected ReadOnly Property lod() As LineOfDuty
            Get
                If (_lod Is Nothing) Then
                    _lod = LodDao.GetById(RefId)
                End If
                Return _lod
            End Get
        End Property

        Protected ReadOnly Property MasterPage() As LodMaster
            Get
                Dim master As LodMaster = CType(Page.Master, LodMaster)
                Return master
            End Get
        End Property

        Protected ReadOnly Property SectionList() As Dictionary(Of String, ALOD.Core.Domain.Workflow.PageAccessType)
            Get
                If (_scAccess Is Nothing) Then
                    _scAccess = lod.ReadSectionList(CInt(Session("GroupId")))
                End If
                Return _scAccess
            End Get
        End Property

        Private ReadOnly Property LodDao() As ILineOfDutyDao
            Get
                If (_lodDao Is Nothing) Then
                    _lodDao = New NHibernateDaoFactory().GetLineOfDutyDao()
                End If
                Return _lodDao
            End Get
        End Property

        Private ReadOnly Property RefId() As Integer
            Get
                Return CInt(Request.QueryString("refId"))
            End Get
        End Property

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
            If (Not IsPostBack) Then
                UserCanEdit = GetAccessLOD(Navigator.PageAccess, True, lod)
                InitControls()
            End If
        End Sub

        Private Sub InitControls()

            Dim formal As Boolean = lod.Formal

            If (lod.Workflow = 1) Then
                OriginalLOD.Visible = True
                FormalPanel.Visible = formal
                LoadFindings(formal)
            Else
                LOD_v2.Visible = True
                If (SESSION_WS_ID(RefId) = 213) Then
                    FormalPanel_v2.Visible = True
                    FormalPanel_v2.Enabled = True
                    FormalFindings_v2.Visible = True
                    'WJAFormalTitle.Text = "test"
                Else
                    FormalPanel_v2.Visible = formal
                End If
                LoadFindings_v2(formal)
            End If

        End Sub

#Region "LOD"

        Protected Sub LoadFindings(ByVal formal As Boolean)

            Dim userFinding As LineOfDutyFindings
            userFinding = lod.FindByType(PersonnelTypes.WING_JA)

            ''**************************************
            ''load the informal
            ''**************************************
            'InformalFindings.LoadFindingOptionsByWorkflow(lod.Workflow, UserGroups.WingJudgeAdvocate)

            'If Not (userFinding Is Nothing) Then

            '    'set the wing JA findings
            '    If userFinding.DecisionYN <> String.Empty Then
            '        InformalFindings.Decision = userFinding.DecisionYN
            '    End If

            '    If userFinding.Finding IsNot Nothing Then
            '        InformalFindings.Findings = userFinding.Finding.Value
            '    End If

            '    InformalFindings.Remarks = userFinding.Explanation
            'End If

            ''set the previous findings (for concur/noncur with)
            'InformalFindings.PrevFindingsLableText = "Unit Commander Findings:"
            'InformalFindings.PrevFindingsText = "Not found"

            'Dim cmdrFinding As LineOfDutyFindings = lod.FindByType(PersonnelTypes.UNIT_CMDR)
            'If Not (cmdrFinding Is Nothing) Then
            '    If Not cmdrFinding.Finding Is Nothing Then
            '        InformalFindings.PrevFindingsText = cmdrFinding.Description
            '    End If
            'End If

            'If (lod.CurrentStatusCode = LodStatusCode.WingJAReview AndAlso UserCanEdit) Then
            '    InformalFindings.SetReadOnly = False
            '    SigCheckInformal.Visible = False
            'Else
            '    InformalFindings.SetReadOnly = True
            '    SigCheckInformal.VerifySignature(RefId, PersonnelTypes.WING_JA)
            'End If

            '**************************************
            'load the formal
            '**************************************
            '   If (formal) Then
            FormalFindings.LoadFindingOptionsByWorkflow(lod.Workflow, UserGroups.WingJudgeAdvocate)

            userFinding = lod.FindByType(PersonnelTypes.FORMAL_WING_JA)

            If (userFinding IsNot Nothing) Then

                'set the wing JA findings
                If userFinding.DecisionYN <> String.Empty Then
                    FormalFindings.Decision = userFinding.DecisionYN
                End If

                If userFinding.Finding IsNot Nothing Then
                    FormalFindings.Findings = userFinding.Finding.Value
                End If

                FormalFindings.Remarks = userFinding.Explanation

            End If

            FormalFindings.PrevFindingsLableText = "IO Findings:"
            FormalFindings.PrevFindingsText = "Not found"
            Dim ioFinding As LineOfDutyFindings = lod.FindByType(PersonnelTypes.IO)
            If Not (ioFinding Is Nothing) Then
                If Not ioFinding.Finding Is Nothing Then
                    FormalFindings.PrevFindingsText = ioFinding.Description
                End If
            End If

            If (lod.CurrentStatusCode = LodStatusCode.FormalActionByWingJA AndAlso UserCanEdit) Then
                FormalFindings.SetReadOnly = False
                SigCheckFormal.Visible = False
            Else
                FormalFindings.SetReadOnly = True
                SigCheckFormal.VerifySignature(RefId, PersonnelTypes.FORMAL_WING_JA)
            End If

            '   End If 'end formal

        End Sub

        Private Sub SaveFindings()

            If (Not UserCanEdit) Then
                Exit Sub
            End If

            Dim finding As LineOfDutyFindings
            finding = CreateFinding(RefId)

            Dim PageAccess As ALOD.Core.Domain.Workflow.PageAccessType
            PageAccess = SectionList(SectionNames.WING_JA_REV.ToString())

            'If lod.CurrentStatusCode = LodStatusCode.WingJAReview Then
            '    finding.PType = PersonnelTypes.WING_JA
            '    finding.DecisionYN = InformalFindings.Decision
            '    If InformalFindings.Decision <> "Y" AndAlso InformalFindings.Findings <> 0 Then
            '        finding.Finding = InformalFindings.Findings
            '    End If
            '    finding.Explanation = InformalFindings.Remarks
            '    lod.SetFindingByType(finding)
            If lod.CurrentStatusCode = LodStatusCode.FormalActionByWingJA Then
                finding.PType = PersonnelTypes.FORMAL_WING_JA
                finding.DecisionYN = FormalFindings.Decision
                If FormalFindings.Decision <> "Y" AndAlso FormalFindings.Findings <> 0 Then
                    finding.Finding = FormalFindings.Findings
                End If
                finding.Explanation = FormalFindings.Remarks
                lod.SetFindingByType(finding)
            End If

        End Sub

#End Region

#Region "LOD_v2"

        Protected Sub LoadFindings_v2(ByVal formal As Boolean)
            Dim userFinding As LineOfDutyFindings
            userFinding = lod.FindByType(PersonnelTypes.WING_JA)

            ' Load the informal findings
            InformalFindings_v2.LoadFindingOptionsByWorkflow(lod.Workflow, UserGroups.WingJudgeAdvocate, optionalInformalFindings, False)

            If userFinding IsNot Nothing Then
                ' Set the wing JA findings
                If userFinding.DecisionYN <> String.Empty Then
                    InformalFindings_v2.Decision = userFinding.DecisionYN
                End If

                If userFinding.Finding IsNot Nothing Then
                    InformalFindings_v2.Findings = userFinding.Finding.Value
                End If

                InformalFindings_v2.Remarks = userFinding.Explanation
            End If

            InformalFindings_v2.PrevFindingsLableText = "Unit Commander Findings:"
            InformalFindings_v2.PrevFindingsText = "Not found"
            Dim cmdrFinding As LineOfDutyFindings = lod.FindByType(PersonnelTypes.UNIT_CMDR)
            If cmdrFinding IsNot Nothing Then
                If cmdrFinding.Finding IsNot Nothing Then
                    InformalFindings_v2.PrevFindingsText = cmdrFinding.Description
                End If
            End If

            If (lod.Status = LodWorkStatus_v3.WingJA_LODV3 AndAlso UserCanEdit) Then
                InformalFindings_v2.SetReadOnly = False
                SigCheckInformal_v2.Visible = False
            Else
                InformalFindings_v2.SetReadOnly = True
                SigCheckInformal_v2.VerifySignature(RefId, PersonnelTypes.WING_JA)
            End If

            ' Load the formal findings
            Try
                FormalFindings_v2.LoadFindingOptionsByWorkflow(lod.Workflow, UserGroups.WingJudgeAdvocate, optionalFindings, False)
                userFinding = lod.FindByType(PersonnelTypes.FORMAL_WING_JA)

                If (userFinding IsNot Nothing) Then
                    ' Set the wing JA findings
                    If userFinding.DecisionYN <> String.Empty Then
                        FormalFindings_v2.Decision = userFinding.DecisionYN
                    End If

                    If userFinding.Finding IsNot Nothing Then
                        FormalFindings_v2.Findings = userFinding.Finding.Value
                    End If

                    FormalFindings_v2.Remarks = userFinding.Explanation
                End If

                FormalFindings_v2.PrevFindingsLableText = "IO Findings:"
                FormalFindings_v2.PrevFindingsText = "Not found"
                Dim ioFinding As LineOfDutyFindings = lod.FindByType(PersonnelTypes.IO)
                If Not (ioFinding Is Nothing) Then
                    If ioFinding.Finding IsNot Nothing Then
                        For Each f As FindingsLookUp In New LookupDao().GetWorkflowFindings(lod.Workflow, UserGroups.InvestigatingOfficer)
                            If (f.Id = ioFinding.Finding.Value) Then
                                FormalFindings_v2.PrevFindingsText = f.Description
                            End If
                        Next
                    End If
                End If

                If (lod.Status >= LodWorkStatus_v2.FormalActionByWingJA And
                    lod.Status < LodWorkStatus_v3.MedicalTechReview_LODV3 AndAlso UserCanEdit) Then
                    FormalFindings_v2.SetReadOnly = False
                    SigCheckFormal_v2.Visible = False
                Else
                    FormalFindings_v2.SetReadOnly = True
                    SigCheckFormal_v2.VerifySignature(RefId, PersonnelTypes.FORMAL_WING_JA)
                End If

                ' Disable Findings if "Concur with the action of Unit Commander" is selected
                If FormalFindings_v2.Decision = "Y" Then
                    FormalFindings_v2.FindingsEnabled = False
                End If
            Catch
            End Try
        End Sub

        Private Sub SaveFindings_v2()

            If (Not UserCanEdit) Then
                Exit Sub
            End If

            Dim finding As LineOfDutyFindings
            finding = CreateFinding(RefId)

            Dim PageAccess As ALOD.Core.Domain.Workflow.PageAccessType
            PageAccess = SectionList(SectionNames.WING_JA_REV.ToString())

            If lod.Status = LodWorkStatus_v3.WingJA_LODV3 Then

                Try
                    finding.PType = PersonnelTypes.WING_JA
                    finding.DecisionYN = InformalFindings_v2.Decision
                    If InformalFindings_v2.Decision <> "Y" AndAlso InformalFindings_v2.Findings <> 0 Then
                        finding.Finding = InformalFindings_v2.Findings
                    End If
                    finding.Explanation = InformalFindings_v2.Remarks
                    lod.SetFindingByType(finding)
                Catch
                End Try

            ElseIf lod.Status = LodWorkStatus_v2.FormalActionByWingJA Then

                Try
                    finding.PType = PersonnelTypes.FORMAL_WING_JA
                    finding.DecisionYN = FormalFindings_v2.Decision
                    If FormalFindings_v2.Decision <> "Y" AndAlso FormalFindings_v2.Findings <> 0 Then
                        finding.Finding = FormalFindings_v2.Findings
                    End If
                    finding.Explanation = FormalFindings_v2.Remarks
                    lod.SetFindingByType(finding)
                Catch
                End Try

            End If

        End Sub

#End Region

#Region "TabEvent"

        Protected Sub Page_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
            AddHandler Me.Master.TabClick, AddressOf TabButtonClicked
            FormalFindings_v2.DoDecisionAutoPostBack = True
            FormalFindings_v2.DeselectFinding()
        End Sub

        Protected Sub Save_Click(ByRef sender As Object, ByRef e As TabNavigationEventArgs)

            If (Navigator.CurrentStep.IsReadOnly) Then
                Exit Sub
            End If

            If (e.ButtonType = NavigatorButtonType.Save OrElse e.ButtonType = NavigatorButtonType.NavigatedAway _
                OrElse e.ButtonType = NavigatorButtonType.NextStep OrElse e.ButtonType = NavigatorButtonType.PreviousStep) Then
                If (lod.Workflow = 1) Then
                    SaveFindings()
                Else
                    SaveFindings_v2()
                End If
            End If

        End Sub

        Private Sub TabButtonClicked(ByRef sender As Object, ByRef e As TabNavigationEventArgs)

            If (e.ButtonType = NavigatorButtonType.Save OrElse
                e.ButtonType = NavigatorButtonType.NavigatedAway OrElse
                e.ButtonType = NavigatorButtonType.NextStep OrElse
                e.ButtonType = NavigatorButtonType.PreviousStep) Then

                If (lod.Workflow = 1) Then
                    SaveFindings()
                Else
                    SaveFindings_v2()
                End If
            End If

        End Sub

#End Region

    End Class

End Namespace