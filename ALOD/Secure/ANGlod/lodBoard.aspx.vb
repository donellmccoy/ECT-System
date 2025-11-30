Imports ALOD.Core.Domain.Modules.Lod
Imports ALOD.Data.Services
Imports ALOD.Core.Domain.Common
Imports ALOD.Logging
Imports ALOD.Web.UserControls
Imports ALODWebUtility.Common
Imports ALOD.Core.Domain.Users
Imports ALOD.Core.Domain.Workflow
Imports ALOD.Core.Utils

Namespace Web.LOD
    Partial Class Secure_lod_lodBoard
        Inherits System.Web.UI.Page
        Private _scAccess As Dictionary(Of String, ALOD.Core.Domain.Workflow.PageAccessType)
        Protected _lod As LineOfDuty
        Protected _lod_v2 As LineOfDuty_v2
        
        Protected Const optionalFindings As String = InvestigationDecision.APPROVE + "," + InvestigationDecision.DISAPPROVE
        Protected Const optionalInformalFindings As String = InvestigationDecision.APPROVE + "," + InvestigationDecision.DISAPPROVE + "," + InvestigationDecision.NOT_LOD_MISCONDUCT

#Region "Master Property"

        Public ReadOnly Property Navigator() As TabNavigator
            Get
                Return MasterPage.Navigator
            End Get
        End Property

        Public ReadOnly Property TabControl() As TabControls
            Get
                Return MasterPage.TabControl
            End Get
        End Property

#End Region

#Region "PageProperty"
        Protected ReadOnly Property lod_v2() As LineOfDuty_v2
            Get
                If (_lod_v2 Is Nothing) Then
                    '_lod_v2 = LodService.GetById(CType(Request.QueryString("refId"), Integer))
                    _lod_v2 = CType(lod, LineOfDuty_v2)
                End If
                Return _lod_v2
            End Get
        End Property

        Protected ReadOnly Property lod() As LineOfDuty
            Get
                If (_lod Is Nothing) Then
                    _lod = LodService.GetById(CType(Request.QueryString("refId"), Integer))
                End If
                Return _lod
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

        Protected ReadOnly Property MasterPage() As LodMaster
            Get
                Dim master As LodMaster = CType(Page.Master, LodMaster)
                Return master
            End Get
        End Property


        Protected ReadOnly Property RefId() As Integer
            Get
                Return CInt(Request.QueryString("refId"))
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


        Protected Sub Page_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
            AddHandler CType(Page.Master, LodMaster).TabClick, AddressOf TabButtonClicked
            AddHandler bdPersonnelFindings_v2.FindingSelected, AddressOf FindingSelected
            AddHandler bdPersonnelFindings_v2.DecisionSelected, AddressOf DecisionSelected
            AddHandler bdAppAuthFindings_v2.FindingSelected, AddressOf FindingSelected
            AddHandler bdAppAuthFindings_v2.DecisionSelected, AddressOf DecisionSelected
            AddHandler bdfrmlPersonnelFindings_v2.FindingSelected, AddressOf FindingSelected
            AddHandler bdfrmlPersonnelFindings_v2.DecisionSelected, AddressOf DecisionSelected
            AddHandler bdFrmlAppAuthFindings_v2.FindingSelected, AddressOf FindingSelected
            AddHandler bdFrmlAppAuthFindings_v2.DecisionSelected, AddressOf DecisionSelected
        End Sub

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
            If (Not IsPostBack) Then
                UserCanEdit = GetAccessLOD(Navigator.PageAccess, True, lod)
                InitiateControls()

                If (lod.Workflow = 1) Then
                    LoadData()
                    EnableSections()
                    LoadPreviousFindings()
                    LOD_Original.Visible = True
                Else
                    LoadData_v2()
                    EnableSections_v2()
                    LoadPreviousFindings_v2()
                    LOD_v2_panel.Visible = True
                End If
                LogManager.LogAction(ModuleType.LOD, UserAction.ViewPage, RefId, "Viewed Page: LOD Board")

            End If

        End Sub

        Public Sub InitiateControls()
            If (lod.Formal) Then
                ShowFormal = True
            Else
                ShowFormal = False
            End If
        End Sub
#End Region

#Region "BoardProperty"

        Public WriteOnly Property ShowInformalSpaces() As Boolean

            Set(ByVal value As Boolean)

                If (lod.Workflow = 1) Then
                    dvsp04.Visible = True
                    dvsp1.Visible = True
                    dvsp2.Visible = True
                    dvsp3.Visible = True
                    dvsp4.Visible = True
                End If
            End Set

        End Property

        Public WriteOnly Property ShowFormalSpaces() As Boolean

            Set(ByVal value As Boolean)

                If (lod.Workflow = 1) Then
                    dvsp5.Visible = True
                    dvsp6.Visible = True
                    dvsp7.Visible = True
                    dvsp8.Visible = True
                    dvsp9.Visible = True
                End If
            End Set

        End Property


        Public WriteOnly Property ShowFormal() As Boolean

            Set(ByVal value As Boolean)
                If (lod.Workflow = 1) Then
                    If (value) Then
                        dvFormal.Visible = True
                        ShowFormalSpaces = True
                        dvInformal.Visible = False
                    Else
                        dvInformal.Visible = True
                        ShowInformalSpaces = True
                        dvFormal.Visible = False
                    End If
                Else
                    If (value) Then
                        dvFormal_v2.Visible = True
                        ShowFormalSpaces = True
                        dvInformal_v2.Visible = False
                    Else
                        dvInformal_v2.Visible = True
                        ShowInformalSpaces = True
                        dvFormal_v2.Visible = False
                    End If
                End If
            End Set

        End Property

        Public Property BoardEnabled() As Boolean
            Get
                If (lod.Workflow = 1) Then
                    Return bdFindings.SetReadOnly
                Else
                    Return bdFindings_v2.SetReadOnly
                End If
            End Get

            Set(ByVal value As Boolean)
                If (lod.Workflow = 1) Then
                    bdFindings.SetReadOnly = Not (value)
                    lblSelectedApprovingAuthority.Visible = Not (value)
                    ddlApprovingAuthorities.Visible = value
                Else
                    bdFindings_v2.SetReadOnly = Not (value)
                    lblSelectedApprovingAuthority_v2.Visible = Not (value)
                    ddlApprovingAuthorities_v2.Visible = value
                End If
            End Set
        End Property

        Public Property BoardVisible() As Boolean
            Get
                If (lod.Workflow = 1) Then
                    Return AFRCBoard.Visible
                Else
                    Return AFRCBoard_v2.Visible
                End If
            End Get
            Set(ByVal value As Boolean)

                If (lod.Workflow = 1) Then
                    AFRCBoard.Visible = value
                    dvsp04.Visible = value
                Else
                    AFRCBoard_v2.Visible = value
                    dvsp04_v2.Visible = value
                End If
            End Set
        End Property

        Public Property LegalEnabled() As Boolean
            Get
                If (lod.Workflow = 1) Then
                    Return bdLegalFindings.SetReadOnly
                Else
                    Return bdLegalFindings_v2.SetReadOnly
                End If
            End Get
            Set(ByVal value As Boolean)
                If (lod.Workflow = 1) Then
                    bdLegalFindings.SetReadOnly = Not (value)
                Else
                    bdLegalFindings_v2.SetReadOnly = Not (value)
                End If
            End Set
        End Property

        Public Property LegalVisible() As Boolean
            Get
                If (lod.Workflow = 1) Then
                    Return BoardLegal.Visible
                Else
                    Return BoardLegal_v2.Visible
                End If
            End Get
            Set(ByVal value As Boolean)
                If (lod.Workflow = 1) Then
                    BoardLegal.Visible = value
                Else
                    BoardLegal_v2.Visible = value
                End If
            End Set
        End Property

        Public Property FrmlLegalEnabled() As Boolean
            Get
                If (lod.Workflow = 1) Then
                    Return bdfrmlLegalFindings.SetReadOnly
                Else
                    Return bdfrmlLegalFindings_v2.SetReadOnly
                End If
            End Get
            Set(ByVal value As Boolean)
                If (lod.Workflow = 1) Then
                    bdfrmlLegalFindings.SetReadOnly = Not (value)
                    bdfrmlLegalFindings.SetAdditionalCommentsReadOnly = True
                Else
                    bdfrmlLegalFindings_v2.SetReadOnly = Not (value)
                    bdfrmlLegalFindings_v2.SetAdditionalCommentsReadOnly = True
                End If
            End Set
        End Property

        Public Property FrmlLegalVisible() As Boolean
            Get
                If (lod.Workflow = 1) Then
                    Return FrmlBoardLegal.Visible
                Else
                    Return FrmlBoardLegal.Visible
                End If
            End Get
            Set(ByVal value As Boolean)
                If (lod.Workflow = 1) Then
                    FrmlBoardLegal.Visible = value
                Else
                    FrmlBoardLegal_v2.Visible = value
                End If
            End Set
        End Property

        Public Property MedicalEnabled() As Boolean
            Get
                If (lod.Workflow = 1) Then
                    Return bdMedicalFindings.SetReadOnly
                Else
                    Return bdMedicalFindings_v2.SetReadOnly
                End If
            End Get
            Set(ByVal value As Boolean)
                If (lod.Workflow = 1) Then
                    bdMedicalFindings.SetReadOnly = Not (value)
                Else
                    bdMedicalFindings_v2.SetReadOnly = Not (value)
                End If
            End Set

        End Property

        Public Property MedicalVisible() As Boolean
            Get
                If (lod.Workflow = 1) Then
                    Return BoardMedical.Visible
                Else
                    Return BoardMedical_v2.Visible
                End If
            End Get
            Set(ByVal value As Boolean)
                If (lod.Workflow = 1) Then
                    BoardMedical.Visible = value
                Else
                    BoardMedical_v2.Visible = value
                End If
            End Set
        End Property

        Public Property FrmlMedicalEnabled() As Boolean
            Get
                If (lod.Workflow = 1) Then
                    Return bdfrmlMedicalFindings.SetReadOnly
                Else
                    Return bdfrmlMedicalFindings_v2.SetReadOnly
                End If
            End Get
            Set(ByVal value As Boolean)
                If (lod.Workflow = 1) Then
                    bdfrmlMedicalFindings.SetReadOnly = Not (value)
                    bdfrmlMedicalFindings.SetAdditionalCommentsReadOnly = True
                Else
                    bdfrmlMedicalFindings_v2.SetReadOnly = Not (value)
                    bdfrmlMedicalFindings_v2.SetAdditionalCommentsReadOnly = True
                End If
            End Set

        End Property

        Public Property FrmlMedicalVisible() As Boolean
            Get
                If (lod.Workflow = 1) Then
                    Return FrmlBoardMedical.Visible
                Else
                    Return FrmlBoardMedical_v2.Visible
                End If
            End Get
            Set(ByVal value As Boolean)
                If (lod.Workflow = 1) Then
                    FrmlBoardMedical.Visible = value
                Else
                    FrmlBoardMedical_v2.Visible = value
                End If
            End Set
        End Property

        Public Property AAEnabled() As Boolean
            Get
                If (lod.Workflow = 1) Then
                    Return bdAppAuthFindings.SetReadOnly
                Else
                    Return bdAppAuthFindings_v2.SetReadOnly
                End If
            End Get
            Set(ByVal value As Boolean)
                If (lod.Workflow = 1) Then
                    bdAppAuthFindings.SetReadOnly = Not (value)
                Else
                    bdAppAuthFindings_v2.SetReadOnly = Not (value)
                End If
            End Set
        End Property

        Public Property AAVisible() As Boolean
            Get
                If (lod.Workflow = 1) Then
                    Return BoardAAReview.Visible
                Else
                    Return BoardAAReview_v2.Visible
                End If
            End Get
            Set(ByVal value As Boolean)
                If (lod.Workflow = 1) Then
                    BoardAAReview.Visible = value
                Else
                    BoardAAReview_v2.Visible = value
                End If
            End Set
        End Property

        Public Property FrmlAAEnabled() As Boolean
            Get
                If (lod.Workflow = 1) Then
                    Return bdFrmlAppAuthFindings.SetReadOnly
                Else
                    Return bdFrmlAppAuthFindings_v2.SetReadOnly
                End If
            End Get
            Set(ByVal value As Boolean)
                If (lod.Workflow = 1) Then
                    bdFrmlAppAuthFindings.SetReadOnly = Not (value)
                    bdFrmlAppAuthFindings.SetAdditionalCommentsReadOnly = True
                Else
                    bdFrmlAppAuthFindings_v2.SetReadOnly = Not (value)
                    bdFrmlAppAuthFindings_v2.SetAdditionalCommentsReadOnly = True
                End If
            End Set
        End Property

        Public Property FrmlAAVisible() As Boolean
            Get
                If (lod.Workflow = 1) Then
                    Return FrmlBoardAAReview.Visible
                Else
                    Return FrmlBoardAAReview_v2.Visible
                End If
            End Get
            Set(ByVal value As Boolean)
                If (lod.Workflow = 1) Then
                    FrmlBoardAAReview.Visible = value
                Else
                    FrmlBoardAAReview_v2.Visible = value
                End If

            End Set
        End Property

        Public Property RAEnabled() As Boolean
            Get
                If (lod.Workflow = 1) Then
                    Return bdFormalRAFindings.SetReadOnly
                Else
                    Return bdFormalRAFindings_v2.SetReadOnly
                End If
            End Get
            Set(ByVal value As Boolean)
                If (lod.Workflow = 1) Then
                    bdFormalRAFindings.SetReadOnly = Not (value)
                    bdFormalRAFindings.SetAdditionalCommentsReadOnly = True
                    lblSelectedFormalApprovingAuthority.Visible = Not (value)
                    ddlFormalApprovingAuthorities.Visible = value
                Else
                    bdFormalRAFindings_v2.SetReadOnly = Not (value)
                    bdFormalRAFindings_v2.SetAdditionalCommentsReadOnly = True
                    lblSelectedFormalApprovingAuthority_v2.Visible = Not (value)
                    ddlFormalApprovingAuthorities_v2.Visible = value
                End If
            End Set
        End Property

        Public Property RAVisible() As Boolean
            Get
                If (lod.Workflow = 1) Then
                    Return FromalBoardRA.Visible
                Else
                    Return FromalBoardRA_v2.Visible
                End If
            End Get
            Set(ByVal value As Boolean)
                If (lod.Workflow = 1) Then
                    FromalBoardRA.Visible = value
                Else
                    FromalBoardRA_v2.Visible = value
                End If
            End Set
        End Property


        Public Property PersonnelEnabled() As Boolean
            Get
                If (lod.Workflow = 1) Then
                    Return bdPersonnelFindings.SetReadOnly
                Else
                    Return bdPersonnelFindings_v2.SetReadOnly
                End If
            End Get
            Set(ByVal value As Boolean)
                If (lod.Workflow = 1) Then
                    bdPersonnelFindings.SetReadOnly = Not (value)
                Else
                    bdPersonnelFindings_v2.SetReadOnly = Not (value)

                End If
            End Set

        End Property

        Public Property PersonnelVisible() As Boolean
            Get
                If (lod.Workflow = 1) Then
                    Return BoardPersonnel.Visible
                Else
                    Return BoardPersonnel_v2.Visible
                End If
            End Get
            Set(ByVal value As Boolean)
                If (lod.Workflow = 1) Then
                    BoardPersonnel.Visible = value
                Else
                    BoardPersonnel_v2.Visible = value
                End If
            End Set
        End Property

        Public Property FrmlPersonnelEnabled() As Boolean
            Get
                If (lod.Workflow = 1) Then
                    Return bdfrmlPersonnelFindings.SetReadOnly
                Else
                    Return bdfrmlPersonnelFindings_v2.SetReadOnly
                End If
            End Get
            Set(ByVal value As Boolean)
                If (lod.Workflow = 1) Then
                    bdfrmlPersonnelFindings.SetReadOnly = Not (value)
                    bdfrmlPersonnelFindings.SetAdditionalCommentsReadOnly = True
                Else
                    bdfrmlPersonnelFindings_v2.SetReadOnly = Not (value)
                    bdfrmlPersonnelFindings_v2.SetAdditionalCommentsReadOnly = True
                End If
            End Set

        End Property

        Public Property FrmlPersonnelVisible() As Boolean
            Get
                If (lod.Workflow = 1) Then
                    Return FrmlBoardPersonnel.Visible
                Else
                    Return FrmlBoardPersonnel_v2.Visible
                End If
            End Get
            Set(ByVal value As Boolean)
                If (lod.Workflow = 1) Then
                    FrmlBoardPersonnel.Visible = value
                Else
                    FrmlBoardPersonnel_v2.Visible = value
                End If
            End Set
        End Property

#End Region

#Region "LOD"
#Region "Load"

        Public Sub EnableSections()

            BoardEnabled = False

            Dim access As ALOD.Core.Domain.Workflow.PageAccessType
            access = SectionList(SectionNames.BOARD_REV.ToString())

            If access <> ALOD.Core.Domain.Workflow.PageAccessType.None Then
                BoardVisible = True
                If access = ALOD.Core.Domain.Workflow.PageAccessType.ReadWrite Then
                    BoardEnabled = True
                End If
            End If

            access = SectionList(SectionNames.BOARD_MED_REV.ToString())
            If access <> ALOD.Core.Domain.Workflow.PageAccessType.None Then
                MedicalVisible = True
                If access = ALOD.Core.Domain.Workflow.PageAccessType.ReadWrite Then
                    MedicalEnabled = True
                End If
            End If

            access = SectionList(SectionNames.FORMAL_BOARD_MED_REV.ToString())
            If access <> ALOD.Core.Domain.Workflow.PageAccessType.None Then
                FrmlMedicalVisible = True
                If access = ALOD.Core.Domain.Workflow.PageAccessType.ReadWrite Then
                    FrmlMedicalEnabled = True
                End If
            End If

            access = SectionList(SectionNames.BOARD_LEGAL_REV.ToString())
            If access <> ALOD.Core.Domain.Workflow.PageAccessType.None Then
                LegalVisible = True
                If access = ALOD.Core.Domain.Workflow.PageAccessType.ReadWrite Then

                    LegalEnabled = True
                End If
            End If

            access = SectionList(SectionNames.FORMAL_BOARD_LEGAL_REV.ToString())
            If access <> ALOD.Core.Domain.Workflow.PageAccessType.None Then
                FrmlLegalVisible = True
                If access = ALOD.Core.Domain.Workflow.PageAccessType.ReadWrite Then

                    FrmlLegalEnabled = True
                End If
            End If

            access = SectionList(SectionNames.BOARD_PERSONNEL_REV.ToString())
            If access <> ALOD.Core.Domain.Workflow.PageAccessType.None Then
                PersonnelVisible = True

                rblCredibleService.Visible = False
                lblCredibleService.Visible = True
                rblWasMemberOnOrders.Visible = False
                lblWasMemberOnOrders.Visible = True

                If access = ALOD.Core.Domain.Workflow.PageAccessType.ReadWrite Then
                    PersonnelEnabled = True
                    rblCredibleService.Visible = True
                    lblCredibleService.Visible = False
                    rblWasMemberOnOrders.Visible = True
                    lblWasMemberOnOrders.Visible = False
                End If
            End If

            access = SectionList(SectionNames.FORMAL_BOARD_PERSONNEL_REV.ToString())
            If access <> ALOD.Core.Domain.Workflow.PageAccessType.None Then
                FrmlPersonnelVisible = True

                rblFrmlCredibleService.Visible = False
                lblFrmlCredibleService.Visible = True
                rblFrmlWasMemberOnOrders.Visible = False
                lblFrmlWasMemberOnOrders.Visible = True

                If access = ALOD.Core.Domain.Workflow.PageAccessType.ReadWrite Then
                    FrmlPersonnelEnabled = True
                    rblFrmlCredibleService.Visible = True
                    lblFrmlCredibleService.Visible = False
                    rblFrmlWasMemberOnOrders.Visible = True
                    lblFrmlWasMemberOnOrders.Visible = False
                End If
            End If

            access = SectionList(SectionNames.BOARD_APPROVING_AUTH_REV.ToString())
            If access <> ALOD.Core.Domain.Workflow.PageAccessType.None Then
                AAVisible = True
                If access = ALOD.Core.Domain.Workflow.PageAccessType.ReadWrite Then

                    AAEnabled = True
                End If
            End If

            access = SectionList(SectionNames.FORMAL_BOARD_APPROVING_AUTH_REV.ToString())
            If access <> ALOD.Core.Domain.Workflow.PageAccessType.None Then
                FrmlAAVisible = True
                If access = ALOD.Core.Domain.Workflow.PageAccessType.ReadWrite Then
                    FrmlAAEnabled = True
                End If
            End If

            RAEnabled = False

            access = SectionList(SectionNames.FORMAL_BOARD_REV.ToString())
            If access <> ALOD.Core.Domain.Workflow.PageAccessType.None Then
                RAVisible = True
                If access = ALOD.Core.Domain.Workflow.PageAccessType.ReadWrite Then
                    RAEnabled = True
                End If
            End If

        End Sub

        Public Sub LoadData()

            Dim cFinding As LineOfDutyFindings

            'Load the formal and informal appointing authority for readonly
            cFinding = lod.FindByType(PersonnelTypes.FORMAL_APP_AUTH)
            Dim hasFinding As Boolean = False

            fcfrmlAppAuth.LoadFindingOptionsByWorkflow(lod.Workflow, UserGroups.WingCommander)

            If Not (cFinding Is Nothing) Then

                If cFinding.DecisionYN <> String.Empty Then
                    fcfrmlAppAuth.Decision = cFinding.DecisionYN
                    hasFinding = True
                End If

                If (Not cFinding.Finding Is Nothing AndAlso cFinding.Finding > 0) Then
                    fcfrmlAppAuth.Findings = cFinding.Finding.Value
                    hasFinding = True
                End If

                fcfrmlAppAuth.Remarks = cFinding.Explanation
                fcfrmlAppAuth.FormFindingsText = cFinding.FindingsText

                If (hasFinding) Then
                    SigCheckAppointing.VerifySignature(RefId, PersonnelTypes.FORMAL_APP_AUTH)
                Else
                    SigCheckAppointing.Visible = False
                End If
            End If

            fcfrmlAppAuth.SetReadOnly = True
            hasFinding = False

            'appointing authority
            cFinding = lod.FindByType(PersonnelTypes.APPOINT_AUTH)

            fcAppAuth.LoadFindingOptionsByWorkflow(lod.Workflow, UserGroups.WingCommander)

            If Not (cFinding Is Nothing) Then
                If (Not cFinding.Finding Is Nothing AndAlso cFinding.Finding > 0) Then
                    fcAppAuth.Findings = cFinding.Finding.Value
                    hasFinding = True
                End If
                fcAppAuth.Remarks = cFinding.Explanation
                fcAppAuth.FormFindingsText = cFinding.FindingsText
            End If

            fcAppAuth.SetReadOnly = True

            'Board technician
            hasFinding = False
            cFinding = lod.FindByType(PersonnelTypes.BOARD)

            bdFindings.LoadFindingOptionsByWorkflow(lod.Workflow, UserGroups.BoardTechnician, InvestigationDecision.LINE_OF_DUTY + "," + InvestigationDecision.EPTS_SERVICE_AGGRAVATE, True)

            If Not (cFinding Is Nothing) Then

                If (Not cFinding.Finding Is Nothing AndAlso cFinding.Finding > 0) Then
                    bdFindings.Findings = cFinding.Finding.Value
                    hasFinding = True
                End If

                bdFindings.Remarks = cFinding.Explanation
                bdFindings.FormFindingsText = cFinding.FindingsText

                bdFormalRAFindings.AdditionalRemarks = cFinding.Explanation

                If hasFinding Then
                    SigCheckBoardTech.VerifySignature(RefId, PersonnelTypes.BOARD)
                Else
                    SigCheckBoardTech.Visible = False
                End If
            Else
                SigCheckBoardTech.Visible = False
            End If

            ddlApprovingAuthorities.DataSource = UserService.GetUsersAlternateTitleByGroup(ALOD.Core.Domain.Users.UserGroups.BoardApprovalAuthority)
            ddlApprovingAuthorities.DataValueField = "userID"
            ddlApprovingAuthorities.DataTextField = "Name"
            ddlApprovingAuthorities.DataBind()

            ddlApprovingAuthorities.Items.Insert(0, New ListItem("-- Select Approving Authority --", ""))
            ddlApprovingAuthorities.SelectedIndex = 0

            If (lod.ApprovingAuthorityUserId.HasValue) Then
                If (ddlApprovingAuthorities.Items.FindByValue(lod.ApprovingAuthorityUserId.Value.ToString()) IsNot Nothing) Then
                    ddlApprovingAuthorities.SelectedValue = lod.ApprovingAuthorityUserId.Value.ToString()
                End If

                Dim u As ALOD.Core.Domain.Users.AppUser = Nothing
                u = UserService.GetById(lod.ApprovingAuthorityUserId.Value.ToString())

                If (u IsNot Nothing) Then
                    lblSelectedApprovingAuthority.Text = u.FirstName + " " + u.LastName
                End If
            End If

            'Board Legal
            hasFinding = False
            cFinding = lod.FindByType(PersonnelTypes.BOARD_JA)

            bdLegalFindings.LoadFindingOptionsByWorkflow(lod.Workflow, UserGroups.BoardLegal)

            If Not (cFinding Is Nothing) Then

                If cFinding.DecisionYN <> String.Empty Then
                    bdLegalFindings.Decision = cFinding.DecisionYN
                    hasFinding = True
                End If

                If (Not cFinding.Finding Is Nothing AndAlso cFinding.Finding > 0) Then
                    bdLegalFindings.Findings = cFinding.Finding.Value
                    hasFinding = True
                End If

                bdLegalFindings.Remarks = cFinding.Explanation
                bdLegalFindings.FormFindingsText = cFinding.FindingsText

                bdfrmlLegalFindings.AdditionalRemarks = cFinding.Explanation

                If (hasFinding) Then
                    SigCheckLegal.VerifySignature(RefId, PersonnelTypes.BOARD_JA)
                Else
                    SigCheckLegal.Visible = False
                End If
            Else
                SigCheckLegal.Visible = False
            End If


            'Board Medical
            hasFinding = False
            cFinding = lod.FindByType(PersonnelTypes.BOARD_SG)

            bdMedicalFindings.LoadFindingOptionsByWorkflow(lod.Workflow, UserGroups.BoardMedical)

            If Not (cFinding Is Nothing) Then

                If cFinding.DecisionYN <> String.Empty Then
                    bdMedicalFindings.Decision = cFinding.DecisionYN
                    hasFinding = True
                End If

                If (Not cFinding.Finding Is Nothing AndAlso cFinding.Finding > 0) Then
                    bdMedicalFindings.Findings = cFinding.Finding.Value
                    hasFinding = True
                End If

                bdMedicalFindings.Remarks = cFinding.Explanation
                bdMedicalFindings.FormFindingsText = cFinding.FindingsText

                bdfrmlMedicalFindings.AdditionalRemarks = cFinding.Explanation

                If (hasFinding) Then
                    SigCheckMedical.VerifySignature(RefId, PersonnelTypes.BOARD_SG)
                Else
                    SigCheckMedical.Visible = False
                End If
            Else
                SigCheckMedical.Visible = False
            End If

            'Board Personnel
            hasFinding = False
            cFinding = lod.FindByType(PersonnelTypes.BOARD_A1)

            bdPersonnelFindings.LoadFindingOptionsByWorkflow(lod.Workflow, UserGroups.BoardAdministrator)

            If Not (cFinding Is Nothing) Then

                If cFinding.DecisionYN <> String.Empty Then
                    bdPersonnelFindings.Decision = cFinding.DecisionYN
                    hasFinding = True
                End If

                If (Not cFinding.Finding Is Nothing AndAlso cFinding.Finding > 0) Then
                    bdPersonnelFindings.Findings = cFinding.Finding.Value
                    hasFinding = True
                End If

                bdPersonnelFindings.Remarks = cFinding.Explanation
                bdPersonnelFindings.FormFindingsText = cFinding.FindingsText

                bdfrmlPersonnelFindings.AdditionalRemarks = cFinding.Explanation

                If (hasFinding) Then
                    SigCheckPersonnel.VerifySignature(RefId, PersonnelTypes.BOARD_A1)
                Else
                    SigCheckPersonnel.Visible = False
                End If
            Else
                SigCheckPersonnel.Visible = False
            End If

            'Approving Authority 
            hasFinding = False
            cFinding = lod.FindByType(PersonnelTypes.BOARD_AA)

            bdAppAuthFindings.LoadFindingOptionsByWorkflow(lod.Workflow, UserGroups.BoardApprovalAuthority)

            If Not (cFinding Is Nothing) Then

                If (Not cFinding.Finding Is Nothing AndAlso cFinding.Finding > 0) Then
                    hasFinding = True
                    bdAppAuthFindings.Findings = cFinding.Finding.Value
                End If

                bdAppAuthFindings.Remarks = cFinding.Explanation
                '       bdAppAuthFindings.FormFindingsText = cFinding.FindingsText

                bdFrmlAppAuthFindings.AdditionalRemarks = cFinding.Explanation

                If (hasFinding) Then
                    SigCheckAA.VerifySignature(RefId, PersonnelTypes.BOARD_AA)
                Else
                    SigCheckAA.Visible = False
                End If
            Else
                SigCheckAA.Visible = False
            End If


            'Formal Board Legal
            hasFinding = False
            cFinding = lod.FindByType(PersonnelTypes.FORMAL_BOARD_JA)

            bdfrmlLegalFindings.LoadFindingOptionsByWorkflow(lod.Workflow, UserGroups.BoardLegal)

            If Not (cFinding Is Nothing) Then

                If cFinding.DecisionYN <> String.Empty Then
                    bdfrmlLegalFindings.Decision = cFinding.DecisionYN
                    hasFinding = True
                End If

                bdfrmlLegalFindings.Remarks = cFinding.Explanation
                bdfrmlLegalFindings.FormFindingsText = cFinding.FindingsText

                If (hasFinding) Then
                    SigCheckLegalFormal.VerifySignature(RefId, PersonnelTypes.FORMAL_BOARD_JA)
                Else
                    SigCheckLegalFormal.Visible = False
                End If

            Else
                SigCheckLegalFormal.Visible = False
            End If

            'Formal Board Medical
            hasFinding = False
            cFinding = lod.FindByType(PersonnelTypes.FORMAL_BOARD_SG)

            bdfrmlMedicalFindings.LoadFindingOptionsByWorkflow(lod.Workflow, UserGroups.BoardMedical)

            If Not (cFinding Is Nothing) Then

                If cFinding.DecisionYN <> String.Empty Then
                    bdfrmlMedicalFindings.Decision = cFinding.DecisionYN
                    hasFinding = True
                End If

                bdfrmlMedicalFindings.Remarks = cFinding.Explanation
                bdfrmlMedicalFindings.FormFindingsText = cFinding.FindingsText

                If (hasFinding) Then
                    SigCheckMedicalFormal.VerifySignature(RefId, PersonnelTypes.FORMAL_BOARD_SG)
                Else
                    SigCheckMedicalFormal.Visible = False
                End If
            Else
                SigCheckMedicalFormal.Visible = False
            End If

            'Formal Board Personnel
            hasFinding = False
            cFinding = lod.FindByType(PersonnelTypes.FORMAL_BOARD_A1)

            bdfrmlPersonnelFindings.LoadFindingOptionsByWorkflow(lod.Workflow, UserGroups.BoardAdministrator)

            If Not (cFinding Is Nothing) Then

                If cFinding.DecisionYN <> String.Empty Then
                    bdfrmlPersonnelFindings.Decision = cFinding.DecisionYN
                    hasFinding = True
                End If

                bdfrmlPersonnelFindings.Remarks = cFinding.Explanation
                bdfrmlPersonnelFindings.FormFindingsText = cFinding.FindingsText

                If (hasFinding) Then
                    SigCheckPersonnelFormal.VerifySignature(RefId, PersonnelTypes.FORMAL_BOARD_A1)
                Else
                    SigCheckPersonnelFormal.Visible = False
                End If
            Else
                SigCheckPersonnelFormal.Visible = False
            End If

            'Formal Approving Authority 
            hasFinding = False
            cFinding = lod.FindByType(PersonnelTypes.FORMAL_BOARD_AA)

            bdFrmlAppAuthFindings.LoadFindingOptionsByWorkflow(lod.Workflow, UserGroups.BoardApprovalAuthority, InvestigationDecision.FORMAL_INVESTIGATION, False)

            If Not (cFinding Is Nothing) Then
                If (Not cFinding.Finding Is Nothing AndAlso cFinding.Finding > 0) Then
                    bdFrmlAppAuthFindings.Findings = cFinding.Finding.Value
                    hasFinding = True
                End If
                bdFrmlAppAuthFindings.Remarks = cFinding.Explanation
                bdFrmlAppAuthFindings.FormFindingsText = cFinding.FindingsText

                If (hasFinding) Then
                    SigCheckAAFormal.VerifySignature(RefId, PersonnelTypes.FORMAL_BOARD_AA)
                Else
                    SigCheckAAFormal.Visible = False
                End If
            Else
                SigCheckAAFormal.Visible = False
            End If

            'Formal Board Reviewing Authority 
            hasFinding = False
            cFinding = lod.FindByType(PersonnelTypes.FORMAL_BOARD_RA)

            bdFormalRAFindings.LoadFindingOptionsByWorkflow(lod.Workflow, UserGroups.BoardTechnician, InvestigationDecision.FORMAL_INVESTIGATION, False)

            If Not (cFinding Is Nothing) Then
                If (Not cFinding.Finding Is Nothing AndAlso cFinding.Finding > 0) Then
                    bdFormalRAFindings.Findings = cFinding.Finding.Value
                    hasFinding = True
                End If
                bdFormalRAFindings.FormFindingsText = cFinding.FindingsText
                bdFormalRAFindings.Remarks = cFinding.Explanation
            End If


            ' Set Board Personnel additional questions
            If (lod.HasCredibleService.HasValue) Then
                If (lod.HasCredibleService) Then
                    rblCredibleService.SelectedValue = "Y"
                    rblFrmlCredibleService.SelectedValue = "Y"
                    lblCredibleService.Text = "Yes"
                    lblFrmlCredibleService.Text = "Yes"
                Else
                    rblCredibleService.SelectedValue = "N"
                    rblFrmlCredibleService.SelectedValue = "N"
                    lblCredibleService.Text = "No"
                    lblFrmlCredibleService.Text = "No"
                End If
            End If

            If (lod.WasMemberOnOrders.HasValue) Then
                If (lod.WasMemberOnOrders) Then
                    rblWasMemberOnOrders.SelectedValue = "Y"
                    rblFrmlWasMemberOnOrders.SelectedValue = "Y"
                    lblWasMemberOnOrders.Text = "Yes"
                    lblFrmlWasMemberOnOrders.Text = "Yes"
                Else
                    rblWasMemberOnOrders.SelectedValue = "N"
                    rblFrmlWasMemberOnOrders.SelectedValue = "N"
                    lblWasMemberOnOrders.Text = "No"
                    lblFrmlWasMemberOnOrders.Text = "No"
                End If
            End If

            ddlFormalApprovingAuthorities.DataSource = UserService.GetUsersAlternateTitleByGroup(ALOD.Core.Domain.Users.UserGroups.BoardApprovalAuthority)
            ddlFormalApprovingAuthorities.DataValueField = "userID"
            ddlFormalApprovingAuthorities.DataTextField = "Name"
            ddlFormalApprovingAuthorities.DataBind()

            ddlFormalApprovingAuthorities.Items.Insert(0, New ListItem("-- Select Approving Authority --", ""))
            ddlFormalApprovingAuthorities.SelectedIndex = 0

            If (lod.ApprovingAuthorityUserId.HasValue) Then
                If (ddlFormalApprovingAuthorities.Items.FindByValue(lod.ApprovingAuthorityUserId.Value.ToString()) IsNot Nothing) Then
                    ddlFormalApprovingAuthorities.SelectedValue = lod.ApprovingAuthorityUserId.Value.ToString()
                End If

                Dim u As ALOD.Core.Domain.Users.AppUser = Nothing
                u = UserService.GetById(lod.ApprovingAuthorityUserId.Value.ToString())

                If (u IsNot Nothing) Then
                    lblSelectedFormalApprovingAuthority.Text = u.FirstName + " " + u.LastName
                End If
            End If

        End Sub

        Public Sub LoadPreviousFindings()
            fcfrmlAppAuth.PrevFindingsLableText = "IO Findings:"
            fcfrmlAppAuth.PrevFindingsText = "Not found"
            Dim ioFinding As LineOfDutyFindings
            ioFinding = lod.FindByType(PersonnelTypes.IO)
            If ioFinding IsNot Nothing Then
                If (ioFinding.Finding IsNot Nothing AndAlso ioFinding.Finding > 0) Then
                    fcfrmlAppAuth.PrevFindingsText = ioFinding.Description
                End If
            End If
        End Sub
#End Region

#Region "Save"

        Public Sub SaveData()

            If (Not UserCanEdit) Then
                Exit Sub
            End If

            Dim lodCurrStatus As Short = lod.Status
            Dim cFinding As LineOfDutyFindings

            'Board
            cFinding = CreateFinding(lod.Id)

            Dim access As ALOD.Core.Domain.Workflow.PageAccessType
            access = SectionList(SectionNames.BOARD_REV.ToString())

            If access = ALOD.Core.Domain.Workflow.PageAccessType.ReadWrite Then
                cFinding.PType = PersonnelTypes.BOARD
                If (bdFindings.Findings IsNot Nothing AndAlso bdFindings.Findings IsNot Nothing AndAlso bdFindings.Findings > 0) Then
                    cFinding.Finding = bdFindings.Findings
                End If
                cFinding.Explanation = bdFindings.Remarks

                lod.SetFindingByType(cFinding)

                If (Not String.IsNullOrEmpty(ddlApprovingAuthorities.SelectedValue)) Then
                    lod.ApprovingAuthorityUserId = Integer.Parse(ddlApprovingAuthorities.SelectedValue)
                End If
            End If

            access = SectionList(SectionNames.BOARD_LEGAL_REV.ToString())

            If access = ALOD.Core.Domain.Workflow.PageAccessType.ReadWrite Then
                cFinding.PType = PersonnelTypes.BOARD_JA
                'Legal
                cFinding.DecisionYN = bdLegalFindings.Decision
                If bdLegalFindings.Decision <> "Y" AndAlso bdLegalFindings.Findings IsNot Nothing AndAlso bdLegalFindings.Findings > 0 Then
                    cFinding.Finding = bdLegalFindings.Findings
                End If
                cFinding.Explanation = bdLegalFindings.Remarks
                cFinding.FindingsText = bdLegalFindings.FormFindingsText
                lod.SetFindingByType(cFinding)
            End If

            access = SectionList(SectionNames.BOARD_MED_REV.ToString())
            If access = ALOD.Core.Domain.Workflow.PageAccessType.ReadWrite Then
                'medical
                cFinding.PType = PersonnelTypes.BOARD_SG
                cFinding.DecisionYN = bdMedicalFindings.Decision
                If bdMedicalFindings.Decision <> "Y" AndAlso bdMedicalFindings.Findings IsNot Nothing AndAlso bdMedicalFindings.Findings > 0 Then
                    cFinding.Finding = bdMedicalFindings.Findings
                End If
                cFinding.FindingsText = bdMedicalFindings.FormFindingsText
                cFinding.Explanation = bdMedicalFindings.Remarks

                lod.SetFindingByType(cFinding)
            End If

            access = SectionList(SectionNames.BOARD_PERSONNEL_REV.ToString())
            If access = ALOD.Core.Domain.Workflow.PageAccessType.ReadWrite Then
                ' Personnel
                cFinding.PType = PersonnelTypes.BOARD_A1
                cFinding.DecisionYN = bdPersonnelFindings.Decision
                If bdPersonnelFindings.Decision <> "Y" AndAlso bdPersonnelFindings.Findings IsNot Nothing AndAlso bdPersonnelFindings.Findings > 0 Then
                    cFinding.Finding = bdPersonnelFindings.Findings
                End If
                cFinding.FindingsText = bdPersonnelFindings.FormFindingsText
                cFinding.Explanation = bdPersonnelFindings.Remarks

                lod.SetFindingByType(cFinding)

                If (String.Equals(rblCredibleService.SelectedValue, "Y")) Then
                    lod.HasCredibleService = True
                ElseIf (String.Equals(rblCredibleService.SelectedValue, "N")) Then
                    lod.HasCredibleService = False
                End If

                If (String.Equals(rblWasMemberOnOrders.SelectedValue, "Y")) Then
                    lod.WasMemberOnOrders = True
                ElseIf (String.Equals(rblWasMemberOnOrders.SelectedValue, "N")) Then
                    lod.WasMemberOnOrders = False
                End If
            End If

            access = SectionList(SectionNames.BOARD_APPROVING_AUTH_REV.ToString())
            If access = ALOD.Core.Domain.Workflow.PageAccessType.ReadWrite Then
                'Approving Authority
                cFinding.PType = PersonnelTypes.BOARD_AA
                If bdAppAuthFindings.Findings IsNot Nothing AndAlso bdAppAuthFindings.Findings > 0 Then
                    cFinding.Finding = bdAppAuthFindings.Findings
                End If
                '  cFinding.FindingsText = bdAppAuthFindings.FormFindingsText
                cFinding.Explanation = bdAppAuthFindings.Remarks

                lod.SetFindingByType(cFinding)
            End If

            access = SectionList(SectionNames.FORMAL_BOARD_LEGAL_REV.ToString())
            If access = ALOD.Core.Domain.Workflow.PageAccessType.ReadWrite Then
                'Formal Legal
                cFinding.PType = PersonnelTypes.FORMAL_BOARD_JA
                cFinding.DecisionYN = bdfrmlLegalFindings.Decision
                cFinding.Explanation = bdfrmlLegalFindings.Remarks
                cFinding.FindingsText = bdfrmlLegalFindings.FormFindingsText
                lod.SetFindingByType(cFinding)
            End If

            access = SectionList(SectionNames.FORMAL_BOARD_MED_REV.ToString())
            If access = ALOD.Core.Domain.Workflow.PageAccessType.ReadWrite Then
                ' Formal Medical
                cFinding.PType = PersonnelTypes.FORMAL_BOARD_SG
                cFinding.DecisionYN = bdfrmlMedicalFindings.Decision
                cFinding.Explanation = bdfrmlMedicalFindings.Remarks
                cFinding.FindingsText = bdfrmlMedicalFindings.FormFindingsText
                lod.SetFindingByType(cFinding)
            End If

            access = SectionList(SectionNames.FORMAL_BOARD_PERSONNEL_REV.ToString())
            If access = ALOD.Core.Domain.Workflow.PageAccessType.ReadWrite Then
                ' Formal Personnel
                cFinding.PType = PersonnelTypes.FORMAL_BOARD_A1
                cFinding.DecisionYN = bdfrmlPersonnelFindings.Decision
                cFinding.Explanation = bdfrmlPersonnelFindings.Remarks
                cFinding.FindingsText = bdfrmlPersonnelFindings.FormFindingsText
                lod.SetFindingByType(cFinding)

                If (String.Equals(rblFrmlCredibleService.SelectedValue, "Y")) Then
                    lod.HasCredibleService = True
                ElseIf (String.Equals(rblFrmlCredibleService.SelectedValue, "N")) Then
                    lod.HasCredibleService = False
                End If

                If (String.Equals(rblFrmlWasMemberOnOrders.SelectedValue, "Y")) Then
                    lod.WasMemberOnOrders = True
                ElseIf (String.Equals(rblFrmlWasMemberOnOrders.SelectedValue, "N")) Then
                    lod.WasMemberOnOrders = False
                End If
            End If

            access = SectionList(SectionNames.FORMAL_BOARD_APPROVING_AUTH_REV.ToString())
            If access = ALOD.Core.Domain.Workflow.PageAccessType.ReadWrite Then
                'Formal approving authority
                cFinding.PType = PersonnelTypes.FORMAL_BOARD_AA
                If bdFrmlAppAuthFindings.Findings IsNot Nothing AndAlso bdFrmlAppAuthFindings.Findings > 0 Then
                    cFinding.Finding = bdFrmlAppAuthFindings.Findings
                    'set the final approval findings also 
                    lod.LODInvestigation.FinalApprovalFindings = bdFrmlAppAuthFindings.Findings
                End If
                cFinding.FindingsText = bdFrmlAppAuthFindings.FormFindingsText
                cFinding.Explanation = bdFrmlAppAuthFindings.Remarks
                lod.SetFindingByType(cFinding)
            End If

            access = SectionList(SectionNames.FORMAL_BOARD_REV.ToString())
            If access = ALOD.Core.Domain.Workflow.PageAccessType.ReadWrite Then

                'Formal Board Review (reviewing authority)
                cFinding.PType = PersonnelTypes.FORMAL_BOARD_RA
                If bdFormalRAFindings.Findings IsNot Nothing AndAlso bdFormalRAFindings.Findings > 0 Then
                    cFinding.Finding = bdFormalRAFindings.Findings
                    'set the final approval findings also 
                    lod.LODInvestigation.FinalApprovalFindings = bdFormalRAFindings.Findings
                End If
                cFinding.Explanation = bdFormalRAFindings.Remarks
                lod.SetFindingByType(cFinding)

                If (Not String.IsNullOrEmpty(ddlFormalApprovingAuthorities.SelectedValue)) Then
                    lod.ApprovingAuthorityUserId = Integer.Parse(ddlFormalApprovingAuthorities.SelectedValue)
                End If
            End If

            LodService.SaveUpdate(lod)
        End Sub
#End Region

#End Region


#Region "LOD_v2"
#Region "Load"

        Public Sub EnableSections_v2()

            BoardEnabled = False

            Dim access As ALOD.Core.Domain.Workflow.PageAccessType
            access = SectionList(SectionNames.BOARD_REV.ToString())

            If access <> ALOD.Core.Domain.Workflow.PageAccessType.None Then
                BoardVisible = True
                If access = ALOD.Core.Domain.Workflow.PageAccessType.ReadWrite Then
                    BoardEnabled = True
                End If
            End If

            access = SectionList(SectionNames.BOARD_MED_REV.ToString())
            If access <> ALOD.Core.Domain.Workflow.PageAccessType.None Then
                MedicalVisible = True
                If access = ALOD.Core.Domain.Workflow.PageAccessType.ReadWrite Then
                    MedicalEnabled = True
                End If
            End If

            access = SectionList(SectionNames.FORMAL_BOARD_MED_REV.ToString())
            If access <> ALOD.Core.Domain.Workflow.PageAccessType.None Then
                FrmlMedicalVisible = True
                If access = ALOD.Core.Domain.Workflow.PageAccessType.ReadWrite Then
                    FrmlMedicalEnabled = True
                End If
            End If

            access = SectionList(SectionNames.BOARD_LEGAL_REV.ToString())
            If access <> ALOD.Core.Domain.Workflow.PageAccessType.None Then
                LegalVisible = True
                If access = ALOD.Core.Domain.Workflow.PageAccessType.ReadWrite Then

                    LegalEnabled = True
                End If
            End If

            access = SectionList(SectionNames.FORMAL_BOARD_LEGAL_REV.ToString())
            If access <> ALOD.Core.Domain.Workflow.PageAccessType.None Then
                FrmlLegalVisible = True
                If access = ALOD.Core.Domain.Workflow.PageAccessType.ReadWrite Then

                    FrmlLegalEnabled = True
                End If
            End If

            access = SectionList(SectionNames.BOARD_PERSONNEL_REV.ToString())
            If access <> ALOD.Core.Domain.Workflow.PageAccessType.None Then
                PersonnelVisible = True
                bdPersonnelChecklbl_v2.Visible = True
                bdPersonnelREFER_v2.Visible = False

                If access = ALOD.Core.Domain.Workflow.PageAccessType.ReadWrite Then
                    PersonnelEnabled = True

                    bdPersonnelChecklbl_v2.Visible = False
                    bdPersonnelREFER_v2.Visible = True
                End If
            End If

            access = SectionList(SectionNames.FORMAL_BOARD_PERSONNEL_REV.ToString())
            If access <> ALOD.Core.Domain.Workflow.PageAccessType.None Then
                FrmlPersonnelVisible = True
                bdFrmlPersonnelChecklbl_v2.Visible = True
                bdfrmlPersonnelREFER_v2.Visible = False

                If access = ALOD.Core.Domain.Workflow.PageAccessType.ReadWrite Then
                    FrmlPersonnelEnabled = True

                    bdFrmlPersonnelChecklbl_v2.Visible = False
                    bdfrmlPersonnelREFER_v2.Visible = True
                End If
            End If

            access = SectionList(SectionNames.BOARD_APPROVING_AUTH_REV.ToString())
            If access <> ALOD.Core.Domain.Workflow.PageAccessType.None Then
                AAVisible = True

                bdAppAuthREFER_v2.Visible = False
                bdAppAuthChecklbl_v2.Visible = True
                If access = ALOD.Core.Domain.Workflow.PageAccessType.ReadWrite Then
                    AAEnabled = True

                    bdAppAuthREFER_v2.Visible = True
                    bdAppAuthChecklbl_v2.Visible = False
                End If
            End If

            access = SectionList(SectionNames.FORMAL_BOARD_APPROVING_AUTH_REV.ToString())
            If access <> ALOD.Core.Domain.Workflow.PageAccessType.None Then
                FrmlAAVisible = True

                FrmlAppAuthREFER_v2.Visible = False
                FrmlAppAuthChecklbl_v2.Visible = True
                If access = ALOD.Core.Domain.Workflow.PageAccessType.ReadWrite Then
                    FrmlAAEnabled = True

                    FrmlAppAuthREFER_v2.Visible = True
                    FrmlAppAuthChecklbl_v2.Visible = False
                End If
            End If

            RAEnabled = False

            access = SectionList(SectionNames.FORMAL_BOARD_REV.ToString())
            If access <> ALOD.Core.Domain.Workflow.PageAccessType.None Then
                RAVisible = True
                If access = ALOD.Core.Domain.Workflow.PageAccessType.ReadWrite Then
                    RAEnabled = True
                End If
            End If

            SetDESProcessingTooltips()

        End Sub

        Protected Sub SetDESProcessingTooltips()
            Dim tooltipText As String = "This box should only be used and members referred for DES processing when all three criteria of the 8 year rule are met; 1) medical authority noted the condition is potentially disqualifying for continued military service 2) member has 8 years of active duty and 3) member was on 30 days of orders or more when the disqualifying condition became unfitting."
            bdFRMLPersonnellbl_v2.ToolTip = tooltipText
            FrmlAppAuthlbl_v2.ToolTip = tooltipText
            bdPersonnellbl_v2.ToolTip = tooltipText
            bdAppAuthlbl_v2.ToolTip = tooltipText
        End Sub

        Public Sub LoadData_v2()

            Dim cFinding As LineOfDutyFindings

            'Load the formal and informal appointing authority for readonly
            cFinding = lod_v2.FindByType(PersonnelTypes.FORMAL_APP_AUTH)
            Dim hasFinding As Boolean = False

            fcfrmlAppAuth_v2.LoadFindingOptionsByWorkflow(lod_v2.Workflow, UserGroups.WingCommander, optionalFindings, False)

            If Not (cFinding Is Nothing) Then

                If cFinding.DecisionYN <> String.Empty Then
                    fcfrmlAppAuth_v2.Decision = cFinding.DecisionYN
                    hasFinding = True
                End If

                If (Not cFinding.Finding Is Nothing AndAlso cFinding.Finding > 0) Then
                    fcfrmlAppAuth_v2.Findings = cFinding.Finding.Value
                    hasFinding = True
                End If

                fcfrmlAppAuth_v2.Remarks = cFinding.Explanation
                fcfrmlAppAuth_v2.FormFindingsText = cFinding.FindingsText

                If (hasFinding) Then
                    SigCheckAppointing_v2.VerifySignature(RefId, PersonnelTypes.FORMAL_APP_AUTH)
                Else
                    SigCheckAppointing_v2.Visible = False
                End If
            End If

            fcfrmlAppAuth_v2.SetReadOnly = True
            hasFinding = False

            'appointing authority
            cFinding = lod_v2.FindByType(PersonnelTypes.APPOINT_AUTH)

            fcAppAuth_v2.LoadFindingOptionsByWorkflow(lod_v2.Workflow, UserGroups.WingCommander, optionalInformalFindings, False)

            If Not (cFinding Is Nothing) Then
                If (Not cFinding.Finding Is Nothing AndAlso cFinding.Finding > 0) Then
                    fcAppAuth_v2.Findings = cFinding.Finding.Value
                    hasFinding = True
                End If
                fcAppAuth_v2.Remarks = cFinding.Explanation
                fcAppAuth_v2.FormFindingsText = cFinding.FindingsText
            End If

            fcAppAuth_v2.SetReadOnly = True

            'Board technician
            hasFinding = False
            cFinding = lod_v2.FindByType(PersonnelTypes.BOARD)

            bdFindings_v2.LoadFindingOptionsByWorkflow(lod_v2.Workflow, UserGroups.BoardTechnician, optionalInformalFindings, False)

            If Not (cFinding Is Nothing) Then

                If (Not cFinding.Finding Is Nothing AndAlso cFinding.Finding > 0) Then
                    bdFindings_v2.Findings = cFinding.Finding.Value
                    hasFinding = True
                End If

                bdFindings_v2.Remarks = cFinding.Explanation
                bdFindings_v2.FormFindingsText = cFinding.FindingsText

                bdFormalRAFindings_v2.AdditionalRemarks = cFinding.Explanation

                If hasFinding Then
                    SigCheckBoardTech_v2.VerifySignature(RefId, PersonnelTypes.BOARD)
                Else
                    SigCheckBoardTech_v2.Visible = False
                End If
            Else
                SigCheckBoardTech_v2.Visible = False
            End If

            ddlApprovingAuthorities_v2.DataSource = UserService.GetUsersAlternateTitleByGroup(ALOD.Core.Domain.Users.UserGroups.BoardApprovalAuthority)
            ddlApprovingAuthorities_v2.DataValueField = "userID"
            ddlApprovingAuthorities_v2.DataTextField = "Name"
            ddlApprovingAuthorities_v2.DataBind()

            ddlApprovingAuthorities_v2.Items.Insert(0, New ListItem("-- Select Approving Authority --", ""))
            ddlApprovingAuthorities_v2.SelectedIndex = 0

            If (lod_v2.ApprovingAuthorityUserId.HasValue) Then
                If (ddlApprovingAuthorities_v2.Items.FindByValue(lod_v2.ApprovingAuthorityUserId.Value.ToString()) IsNot Nothing) Then
                    ddlApprovingAuthorities_v2.SelectedValue = lod_v2.ApprovingAuthorityUserId.Value.ToString()
                End If

                Dim u As ALOD.Core.Domain.Users.AppUser = Nothing
                u = UserService.GetById(lod_v2.ApprovingAuthorityUserId.Value.ToString())

                If (u IsNot Nothing) Then
                    lblSelectedApprovingAuthority_v2.Text = u.FirstName + " " + u.LastName
                End If
            End If

            'Board Legal
            hasFinding = False
            cFinding = lod_v2.FindByType(PersonnelTypes.BOARD_JA)

            bdLegalFindings_v2.LoadFindingOptionsByWorkflow(lod_v2.Workflow, UserGroups.BoardLegal, optionalInformalFindings, False)

            If Not (cFinding Is Nothing) Then

                If cFinding.DecisionYN <> String.Empty Then
                    bdLegalFindings_v2.Decision = cFinding.DecisionYN
                    hasFinding = True
                End If

                If (Not cFinding.Finding Is Nothing AndAlso cFinding.Finding > 0) Then
                    bdLegalFindings_v2.Findings = cFinding.Finding.Value
                    hasFinding = True
                End If

                bdLegalFindings_v2.Remarks = cFinding.Explanation
                bdLegalFindings_v2.FormFindingsText = cFinding.FindingsText

                bdfrmlLegalFindings_v2.AdditionalRemarks = cFinding.Explanation

                If (hasFinding) Then
                    SigCheckLegal_v2.VerifySignature(RefId, PersonnelTypes.BOARD_JA)
                Else
                    SigCheckLegal_v2.Visible = False
                End If
            Else
                SigCheckLegal_v2.Visible = False
            End If


            'Board Medical
            hasFinding = False
            cFinding = lod_v2.FindByType(PersonnelTypes.BOARD_SG)

            bdMedicalFindings_v2.LoadFindingOptionsByWorkflow(lod_v2.Workflow, UserGroups.BoardMedical, optionalInformalFindings, False)

            If Not (cFinding Is Nothing) Then

                If cFinding.DecisionYN <> String.Empty Then
                    bdMedicalFindings_v2.Decision = cFinding.DecisionYN
                    hasFinding = True
                End If

                If (Not cFinding.Finding Is Nothing AndAlso cFinding.Finding > 0) Then
                    bdMedicalFindings_v2.Findings = cFinding.Finding.Value
                    hasFinding = True
                End If

                bdMedicalFindings_v2.Remarks = cFinding.Explanation
                bdMedicalFindings_v2.FormFindingsText = cFinding.FindingsText

                bdfrmlMedicalFindings_v2.AdditionalRemarks = cFinding.Explanation

                If (hasFinding) Then
                    SigCheckMedical_v2.VerifySignature(RefId, PersonnelTypes.BOARD_SG)
                Else
                    SigCheckMedical_v2.Visible = False
                End If
            Else
                SigCheckMedical_v2.Visible = False
            End If

            'Board Personnel
            hasFinding = False
            cFinding = lod_v2.FindByType(PersonnelTypes.BOARD_A1)

            bdPersonnelFindings_v2.LoadFindingOptionsByWorkflow(lod_v2.Workflow, UserGroups.BoardAdministrator, optionalInformalFindings, False)

            If Not (cFinding Is Nothing) Then

                If (cFinding.DecisionYN <> String.Empty) Then
                    bdPersonnelFindings_v2.Decision = cFinding.DecisionYN
                    hasFinding = True
                End If

                If (Not cFinding.Finding Is Nothing AndAlso cFinding.Finding > 0) Then
                    bdPersonnelFindings_v2.Findings = cFinding.Finding.Value
                    hasFinding = True
                End If

                bdPersonnelFindings_v2.Remarks = cFinding.Explanation
                bdPersonnelFindings_v2.FormFindingsText = cFinding.FindingsText

                bdfrmlPersonnelFindings_v2.AdditionalRemarks = cFinding.Explanation

                If (cFinding.DecisionYN.Equals("Y")) Then
                    bdPersonnelFindings_v2.EnableFindings = False

                    If (Not HasDESReferFindings(lod_v2.FindByType(PersonnelTypes.APPOINT_AUTH))) Then
                        bdPersonnelREFER_v2.Enabled = False
                    End If
                ElseIf (Not cFinding.Finding Is Nothing AndAlso cFinding.Finding.Value <> LodFinding.NotInLineOfDutyNotDueToOwnMisconduct) Then
                    bdPersonnelREFER_v2.Enabled = False
                End If

                If (bdPersonnelREFER_v2.Enabled AndAlso cFinding.ReferDES.HasValue()) Then
                    bdPersonnelREFER_v2.Checked = cFinding.ReferDES.Value

                    If (cFinding.ReferDES.Value) Then
                        bdPersonnelChecklbl_v2.Text = "Yes"
                    Else
                        bdPersonnelChecklbl_v2.Text = "No"
                    End If
                End If

                If (hasFinding) Then
                    SigCheckPersonnel_v2.VerifySignature(RefId, PersonnelTypes.BOARD_A1)
                Else
                    SigCheckPersonnel_v2.Visible = False
                End If
            Else
                SigCheckPersonnel_v2.Visible = False
                bdPersonnelREFER_v2.Enabled = False
                bdPersonnelFindings_v2.EnableFindings = False
            End If


            'Approving Authority 
            hasFinding = False
            cFinding = lod_v2.FindByType(PersonnelTypes.BOARD_AA)

            bdAppAuthFindings_v2.LoadFindingOptionsByWorkflow(lod_v2.Workflow, UserGroups.BoardApprovalAuthority, optionalInformalFindings, False)

            If Not (cFinding Is Nothing) Then

                If (Not cFinding.Finding Is Nothing AndAlso cFinding.Finding > 0) Then
                    hasFinding = True
                    bdAppAuthFindings_v2.Findings = cFinding.Finding.Value

                    If (cFinding.Finding.Value <> LodFinding.NotInLineOfDutyNotDueToOwnMisconduct) Then
                        bdAppAuthREFER_v2.Enabled = False
                    End If

                    bdFindings_v2.DisableAllFindingsExcept(cFinding.Finding.Value)
                End If

                bdAppAuthFindings_v2.Remarks = cFinding.Explanation
                bdAppAuthFindings.FormFindingsText = cFinding.FindingsText

                bdFrmlAppAuthFindings_v2.AdditionalRemarks = cFinding.Explanation

                If (bdAppAuthREFER_v2.Enabled AndAlso cFinding.ReferDES.HasValue()) Then
                    bdAppAuthREFER_v2.Checked = cFinding.ReferDES.Value

                    If (cFinding.ReferDES.Value) Then
                        bdAppAuthChecklbl_v2.Text = "Yes"
                    Else
                        bdAppAuthChecklbl_v2.Text = "No"
                    End If
                End If

                If (hasFinding) Then
                    SigCheckAA_v2.VerifySignature(RefId, PersonnelTypes.BOARD_AA)
                Else
                    SigCheckAA_v2.Visible = False
                End If
            Else
                SigCheckAA_v2.Visible = False
                bdAppAuthREFER_v2.Enabled = False
            End If


            'Formal Board Legal
            hasFinding = False
            cFinding = lod_v2.FindByType(PersonnelTypes.FORMAL_BOARD_JA)

            bdfrmlLegalFindings_v2.LoadFindingOptionsByWorkflow(lod_v2.Workflow, UserGroups.BoardLegal, optionalFindings, False)

            If Not (cFinding Is Nothing) Then

                If cFinding.DecisionYN <> String.Empty Then
                    bdfrmlLegalFindings_v2.Decision = cFinding.DecisionYN
                    hasFinding = True
                End If

                bdfrmlLegalFindings_v2.Remarks = cFinding.Explanation
                bdfrmlLegalFindings_v2.FormFindingsText = cFinding.FindingsText

                If (hasFinding) Then
                    SigCheckLegalFormal_v2.VerifySignature(RefId, PersonnelTypes.FORMAL_BOARD_JA)
                Else
                    SigCheckLegalFormal_v2.Visible = False
                End If

            Else
                SigCheckLegalFormal_v2.Visible = False
            End If

            'Formal Board Medical
            hasFinding = False
            cFinding = lod_v2.FindByType(PersonnelTypes.FORMAL_BOARD_SG)

            bdfrmlMedicalFindings_v2.LoadFindingOptionsByWorkflow(lod_v2.Workflow, UserGroups.BoardMedical, optionalFindings, False)

            If Not (cFinding Is Nothing) Then

                If cFinding.DecisionYN <> String.Empty Then
                    bdfrmlMedicalFindings_v2.Decision = cFinding.DecisionYN
                    hasFinding = True
                End If

                bdfrmlMedicalFindings_v2.Remarks = cFinding.Explanation
                bdfrmlMedicalFindings_v2.FormFindingsText = cFinding.FindingsText

                If (hasFinding) Then
                    SigCheckMedicalFormal_v2.VerifySignature(RefId, PersonnelTypes.FORMAL_BOARD_SG)
                Else
                    SigCheckMedicalFormal_v2.Visible = False
                End If
            Else
                SigCheckMedicalFormal_v2.Visible = False
            End If

            'Formal Board Personnel
            hasFinding = False
            cFinding = lod_v2.FindByType(PersonnelTypes.FORMAL_BOARD_A1)

            bdfrmlPersonnelFindings_v2.LoadFindingOptionsByWorkflow(lod_v2.Workflow, UserGroups.BoardAdministrator, optionalFindings, False)

            If Not (cFinding Is Nothing) Then

                If cFinding.DecisionYN <> String.Empty Then
                    bdfrmlPersonnelFindings_v2.Decision = cFinding.DecisionYN
                    hasFinding = True
                End If

                If (Not cFinding.Finding Is Nothing AndAlso cFinding.Finding > 0) Then
                    bdfrmlPersonnelFindings_v2.Findings = cFinding.Finding.Value
                    hasFinding = True
                End If

                bdfrmlPersonnelFindings_v2.Remarks = cFinding.Explanation
                bdfrmlPersonnelFindings_v2.FormFindingsText = cFinding.FindingsText

                If (cFinding.DecisionYN.Equals("Y")) Then
                    bdPersonnelFindings_v2.EnableFindings = False

                    If (Not HasDESReferFindings(lod_v2.FindByType(PersonnelTypes.FORMAL_APP_AUTH))) Then
                        bdfrmlPersonnelREFER_v2.Enabled = False
                    End If
                ElseIf (Not cFinding.Finding Is Nothing AndAlso cFinding.Finding.Value <> LodFinding.NotInLineOfDutyNotDueToOwnMisconduct) Then
                    bdfrmlPersonnelREFER_v2.Enabled = False
                End If

                If (cFinding.ReferDES.HasValue()) Then
                    bdfrmlPersonnelREFER_v2.Checked = cFinding.ReferDES.Value

                    If (cFinding.ReferDES.Value) Then
                        bdFrmlPersonnelChecklbl_v2.Text = "Yes"
                    Else
                        bdFrmlPersonnelChecklbl_v2.Text = "No"
                    End If
                End If

                If (hasFinding) Then
                    SigCheckPersonnelFormal_v2.VerifySignature(RefId, PersonnelTypes.FORMAL_BOARD_A1)
                Else
                    SigCheckPersonnelFormal_v2.Visible = False
                End If
            Else
                SigCheckPersonnelFormal_v2.Visible = False
                bdfrmlPersonnelREFER_v2.Enabled = False
                bdPersonnelFindings_v2.EnableFindings = False
            End If



            'Formal Board Technician (aka Reviewing authority)
            hasFinding = False
            cFinding = lod_v2.FindByType(PersonnelTypes.FORMAL_BOARD_RA)

            bdFormalRAFindings_v2.LoadFindingOptionsByWorkflow(lod_v2.Workflow, UserGroups.BoardTechnician, InvestigationDecision.FORMAL_INVESTIGATION + "," + optionalFindings, False)

            If Not (cFinding Is Nothing) Then
                If (Not cFinding.Finding Is Nothing AndAlso cFinding.Finding > 0) Then
                    bdFormalRAFindings_v2.Findings = cFinding.Finding.Value
                    hasFinding = True
                End If
                bdFormalRAFindings_v2.FormFindingsText = cFinding.FindingsText
                bdFormalRAFindings_v2.Remarks = cFinding.Explanation
            End If

            ddlFormalApprovingAuthorities_v2.DataSource = UserService.GetUsersAlternateTitleByGroup(ALOD.Core.Domain.Users.UserGroups.BoardApprovalAuthority)
            ddlFormalApprovingAuthorities_v2.DataValueField = "userID"
            ddlFormalApprovingAuthorities_v2.DataTextField = "Name"
            ddlFormalApprovingAuthorities_v2.DataBind()

            ddlFormalApprovingAuthorities_v2.Items.Insert(0, New ListItem("-- Select Approving Authority --", ""))
            ddlFormalApprovingAuthorities_v2.SelectedIndex = 0

            If (lod_v2.ApprovingAuthorityUserId.HasValue) Then
                If (ddlFormalApprovingAuthorities_v2.Items.FindByValue(lod_v2.ApprovingAuthorityUserId.Value.ToString()) IsNot Nothing) Then
                    ddlFormalApprovingAuthorities_v2.SelectedValue = lod_v2.ApprovingAuthorityUserId.Value.ToString()
                End If

                Dim u As ALOD.Core.Domain.Users.AppUser = Nothing
                u = UserService.GetById(lod_v2.ApprovingAuthorityUserId.Value.ToString())

                If (u IsNot Nothing) Then
                    lblSelectedFormalApprovingAuthority_v2.Text = u.FirstName + " " + u.LastName
                End If
            End If



            'Formal Approving Authority 
            hasFinding = False
            cFinding = lod_v2.FindByType(PersonnelTypes.FORMAL_BOARD_AA)

            bdFrmlAppAuthFindings_v2.LoadFindingOptionsByWorkflow(lod_v2.Workflow, UserGroups.BoardApprovalAuthority, InvestigationDecision.FORMAL_INVESTIGATION + "," + optionalFindings, False)

            If Not (cFinding Is Nothing) Then
                If (Not cFinding.Finding Is Nothing AndAlso cFinding.Finding > 0) Then
                    bdFrmlAppAuthFindings_v2.Findings = cFinding.Finding.Value
                    hasFinding = True

                    bdFormalRAFindings_v2.DisableAllFindingsExcept(cFinding.Finding.Value)

                    If (cFinding.Finding.Value <> LodFinding.NotInLineOfDutyNotDueToOwnMisconduct) Then
                        FrmlAppAuthREFER_v2.Enabled = False
                    End If
                End If

                bdFrmlAppAuthFindings_v2.Remarks = cFinding.Explanation
                bdFrmlAppAuthFindings_v2.FormFindingsText = cFinding.FindingsText

                If (FrmlAppAuthREFER_v2.Enabled AndAlso cFinding.ReferDES.HasValue()) Then
                    FrmlAppAuthREFER_v2.Checked = cFinding.ReferDES.Value

                    If (cFinding.ReferDES.Value) Then
                        FrmlAppAuthChecklbl_v2.Text = "Yes"
                    Else
                        FrmlAppAuthChecklbl_v2.Text = "No"
                    End If
                End If

                If (hasFinding) Then
                    SigCheckAAFormal_v2.VerifySignature(RefId, PersonnelTypes.FORMAL_BOARD_AA)
                Else
                    SigCheckAAFormal_v2.Visible = False
                End If
            Else
                SigCheckAAFormal_v2.Visible = False
                FrmlAppAuthREFER_v2.Enabled = False
            End If
        End Sub

        Public Sub LoadPreviousFindings_v2()
            fcfrmlAppAuth_v2.PrevFindingsLableText = "IO Findings:"
            fcfrmlAppAuth_v2.PrevFindingsText = "Not found"
            Dim ioFinding As LineOfDutyFindings
            ioFinding = lod_v2.FindByType(PersonnelTypes.IO)
            If ioFinding IsNot Nothing Then
                If (ioFinding.Finding IsNot Nothing AndAlso ioFinding.Finding > 0) Then
                    fcfrmlAppAuth_v2.PrevFindingsText = ioFinding.Description
                End If
            End If
        End Sub

#End Region

#Region "Save"

        Public Sub SaveData_v2()

            If (Not UserCanEdit) Then
                Exit Sub
            End If

            Dim lodCurrStatus As Short = lod_v2.Status
            Dim cFinding As LineOfDutyFindings
            Dim access As ALOD.Core.Domain.Workflow.PageAccessType
            Dim invalid As Boolean = False

            cFinding = CreateFinding(lod_v2.Id)

            access = SectionList(SectionNames.BOARD_LEGAL_REV.ToString())

            If access = ALOD.Core.Domain.Workflow.PageAccessType.ReadWrite Then
                cFinding.PType = PersonnelTypes.BOARD_JA
                'Legal
                cFinding.DecisionYN = bdLegalFindings_v2.Decision
                If bdLegalFindings_v2.Decision <> "Y" AndAlso bdLegalFindings_v2.Findings IsNot Nothing AndAlso bdLegalFindings_v2.Findings > 0 Then
                    cFinding.Finding = bdLegalFindings_v2.Findings
                End If
                cFinding.Explanation = bdLegalFindings_v2.Remarks
                cFinding.FindingsText = bdLegalFindings_v2.FormFindingsText
                lod_v2.SetFindingByType(cFinding)
            End If

            access = SectionList(SectionNames.BOARD_MED_REV.ToString())
            If access = ALOD.Core.Domain.Workflow.PageAccessType.ReadWrite Then
                'medical
                cFinding.PType = PersonnelTypes.BOARD_SG
                cFinding.DecisionYN = bdMedicalFindings_v2.Decision
                If bdMedicalFindings_v2.Decision <> "Y" AndAlso bdMedicalFindings_v2.Findings IsNot Nothing AndAlso bdMedicalFindings_v2.Findings > 0 Then
                    cFinding.Finding = bdMedicalFindings_v2.Findings
                End If
                cFinding.FindingsText = bdMedicalFindings_v2.FormFindingsText
                cFinding.Explanation = bdMedicalFindings_v2.Remarks

                lod_v2.SetFindingByType(cFinding)
            End If

            access = SectionList(SectionNames.BOARD_PERSONNEL_REV.ToString())
            If access = ALOD.Core.Domain.Workflow.PageAccessType.ReadWrite Then
                ' Personnel
                cFinding.PType = PersonnelTypes.BOARD_A1
                cFinding.DecisionYN = bdPersonnelFindings_v2.Decision
                If bdPersonnelFindings_v2.Decision <> "Y" AndAlso bdPersonnelFindings_v2.Findings IsNot Nothing AndAlso bdPersonnelFindings_v2.Findings > 0 Then
                    cFinding.Finding = bdPersonnelFindings_v2.Findings
                End If
                cFinding.FindingsText = bdPersonnelFindings_v2.FormFindingsText
                cFinding.Explanation = bdPersonnelFindings_v2.Remarks

                cFinding.ReferDES = bdPersonnelREFER_v2.Checked

                lod_v2.SetFindingByType(cFinding)

            End If

            access = SectionList(SectionNames.BOARD_APPROVING_AUTH_REV.ToString())
            If access = ALOD.Core.Domain.Workflow.PageAccessType.ReadWrite Then
                'Approving Authority
                cFinding.PType = PersonnelTypes.BOARD_AA

                If (bdAppAuthFindings_v2.Findings IsNot Nothing AndAlso bdAppAuthFindings_v2.Findings > 0) Then
                    cFinding.Finding = bdAppAuthFindings_v2.Findings

                    If (bdFindings_v2.Findings IsNot Nothing AndAlso Not bdFindings_v2.Findings = bdAppAuthFindings_v2.Findings) Then

                        invalid = True
                    End If

                    lod.BoardForGeneral = "N"
                Else
                    lod.BoardForGeneral = "Y"
                End If

                cFinding.FindingsText = bdAppAuthFindings.FormFindingsText
                cFinding.Explanation = bdAppAuthFindings_v2.Remarks

                cFinding.ReferDES = bdAppAuthREFER_v2.Checked

                lod_v2.SetFindingByType(cFinding)


            End If

            If (invalid) Then
                cFinding.PType = PersonnelTypes.BOARD

                cFinding.Finding = 0
                cFinding.Explanation = bdFindings_v2.Remarks

                lod_v2.SetFindingByType(cFinding)

                invalid = False
            End If

            access = SectionList(SectionNames.BOARD_REV.ToString())

            If access = ALOD.Core.Domain.Workflow.PageAccessType.ReadWrite Then
                cFinding.PType = PersonnelTypes.BOARD
                'Board
                If (bdFindings_v2.Findings IsNot Nothing AndAlso bdFindings_v2.Findings > 0) Then
                    cFinding.Finding = bdFindings_v2.Findings

                End If
                cFinding.Explanation = bdFindings_v2.Remarks

                lod_v2.SetFindingByType(cFinding)

                If (Not String.IsNullOrEmpty(ddlApprovingAuthorities_v2.SelectedValue)) Then
                    lod_v2.ApprovingAuthorityUserId = Integer.Parse(ddlApprovingAuthorities_v2.SelectedValue)
                End If
            End If

            access = SectionList(SectionNames.FORMAL_BOARD_LEGAL_REV.ToString())
            If access = ALOD.Core.Domain.Workflow.PageAccessType.ReadWrite Then
                'Formal Legal
                cFinding.PType = PersonnelTypes.FORMAL_BOARD_JA
                cFinding.DecisionYN = bdfrmlLegalFindings_v2.Decision
                cFinding.Explanation = bdfrmlLegalFindings_v2.Remarks
                cFinding.FindingsText = bdfrmlLegalFindings_v2.FormFindingsText
                lod_v2.SetFindingByType(cFinding)
            End If

            access = SectionList(SectionNames.FORMAL_BOARD_MED_REV.ToString())
            If access = ALOD.Core.Domain.Workflow.PageAccessType.ReadWrite Then
                ' Formal Medical
                cFinding.PType = PersonnelTypes.FORMAL_BOARD_SG
                cFinding.DecisionYN = bdfrmlMedicalFindings_v2.Decision
                cFinding.Explanation = bdfrmlMedicalFindings_v2.Remarks
                cFinding.FindingsText = bdfrmlMedicalFindings_v2.FormFindingsText
                lod_v2.SetFindingByType(cFinding)
            End If

            access = SectionList(SectionNames.FORMAL_BOARD_PERSONNEL_REV.ToString())
            If access = ALOD.Core.Domain.Workflow.PageAccessType.ReadWrite Then
                ' Formal Personnel
                cFinding.PType = PersonnelTypes.FORMAL_BOARD_A1
                cFinding.DecisionYN = bdfrmlPersonnelFindings_v2.Decision

                If bdfrmlPersonnelFindings_v2.Decision <> "Y" AndAlso bdfrmlPersonnelFindings_v2.Findings IsNot Nothing AndAlso bdfrmlPersonnelFindings_v2.Findings > 0 Then
                    cFinding.Finding = bdfrmlPersonnelFindings_v2.Findings
                End If

                cFinding.Explanation = bdfrmlPersonnelFindings_v2.Remarks
                cFinding.FindingsText = bdfrmlPersonnelFindings_v2.FormFindingsText
                cFinding.ReferDES = bdfrmlPersonnelREFER_v2.Checked
                lod_v2.SetFindingByType(cFinding)
            End If

            access = SectionList(SectionNames.FORMAL_BOARD_APPROVING_AUTH_REV.ToString())
            If access = ALOD.Core.Domain.Workflow.PageAccessType.ReadWrite Then
                'Formal approving authority
                cFinding.PType = PersonnelTypes.FORMAL_BOARD_AA
                If (bdFrmlAppAuthFindings_v2.Findings IsNot Nothing AndAlso bdFrmlAppAuthFindings_v2.Findings > 0) Then
                    cFinding.Finding = bdFrmlAppAuthFindings_v2.Findings
                    'set the final approval findings also 
                    lod_v2.LODInvestigation.FinalApprovalFindings = bdFrmlAppAuthFindings_v2.Findings

                    If (bdFormalRAFindings_v2.Findings IsNot Nothing AndAlso Not bdFormalRAFindings_v2.Findings = bdAppAuthFindings_v2.Findings) Then

                        invalid = True
                    End If

                    lod.BoardForGeneral = "N"
                Else
                    lod.BoardForGeneral = "Y"

                End If
                cFinding.FindingsText = bdFrmlAppAuthFindings_v2.FormFindingsText
                cFinding.Explanation = bdFrmlAppAuthFindings_v2.Remarks
                cFinding.ReferDES = FrmlAppAuthREFER_v2.Checked
                lod_v2.SetFindingByType(cFinding)
            End If

            If (invalid) Then
                cFinding.PType = PersonnelTypes.FORMAL_BOARD_RA

                cFinding.Finding = 0
                cFinding.Explanation = bdFormalRAFindings_v2.Remarks

                lod_v2.SetFindingByType(cFinding)

                invalid = False
            End If

            access = SectionList(SectionNames.FORMAL_BOARD_REV.ToString())
            If access = ALOD.Core.Domain.Workflow.PageAccessType.ReadWrite Then

                'Formal Board Review (reviewing authority)
                cFinding.PType = PersonnelTypes.FORMAL_BOARD_RA
                If (bdFormalRAFindings_v2.Findings IsNot Nothing AndAlso bdFormalRAFindings_v2.Findings > 0) Then
                    cFinding.Finding = bdFormalRAFindings_v2.Findings
                    'set the final approval findings also 
                    lod_v2.LODInvestigation.FinalApprovalFindings = bdFormalRAFindings_v2.Findings
                End If
                cFinding.Explanation = bdFormalRAFindings_v2.Remarks
                lod_v2.SetFindingByType(cFinding)

                If (Not String.IsNullOrEmpty(ddlFormalApprovingAuthorities_v2.SelectedValue)) Then
                    lod_v2.ApprovingAuthorityUserId = Integer.Parse(ddlFormalApprovingAuthorities_v2.SelectedValue)
                End If
            End If

            LodService.SaveUpdate(lod_v2)
        End Sub
#End Region

        Protected Sub DecisionSelected(sender As Object, e As RadioButtonSelectedEventArgs)
            Dim findingsControl As Secure_Shared_UserControls_FindingsControl = CType(sender, Secure_Shared_UserControls_FindingsControl)

            ' Determine if the finding options need to be enabled or disabled...
            If (findingsControl.Decision.Equals("Y")) Then
                DisableDESReferControls(findingsControl)
            End If
        End Sub

        Protected Sub FindingSelected(sender As Object, e As RadioButtonSelectedEventArgs)
            Dim findingsControl As Secure_Shared_UserControls_FindingsControl = CType(sender, Secure_Shared_UserControls_FindingsControl)

            If (Short.Parse(findingsControl.Findings) = LodFinding.NotInLineOfDutyNotDueToOwnMisconduct) Then
                If (findingsControl Is bdPersonnelFindings_v2) Then
                    bdPersonnelREFER_v2.Enabled = True
                End If

                If (findingsControl Is bdAppAuthFindings_v2) Then
                    bdAppAuthREFER_v2.Enabled = True
                End If

                If (findingsControl Is bdfrmlPersonnelFindings_v2) Then
                    bdfrmlPersonnelREFER_v2.Enabled = True
                End If

                If (findingsControl Is bdFrmlAppAuthFindings_v2) Then
                    FrmlAppAuthREFER_v2.Enabled = True
                End If

            Else
                DisableDESReferControls(findingsControl)
            End If

        End Sub

        Protected Sub DisableDESReferControls(ByVal findingsControl As Secure_Shared_UserControls_FindingsControl)
            If (findingsControl Is bdPersonnelFindings_v2) Then
                If (findingsControl.Decision.Equals("Y")) Then
                    If (Not HasDESReferFindings(lod_v2.FindByType(PersonnelTypes.APPOINT_AUTH))) Then
                        bdPersonnelREFER_v2.Enabled = False
                        bdPersonnelREFER_v2.Checked = False
                    End If
                Else
                    bdPersonnelREFER_v2.Checked = False
                    bdPersonnelREFER_v2.Enabled = False
                End If
            End If

            If (findingsControl Is bdAppAuthFindings_v2) Then
                bdAppAuthREFER_v2.Checked = False
                bdAppAuthREFER_v2.Enabled = False
            End If

            If (findingsControl Is bdfrmlPersonnelFindings_v2) Then
                If (findingsControl.Decision.Equals("Y")) Then
                    If (Not HasDESReferFindings(lod_v2.FindByType(PersonnelTypes.FORMAL_APP_AUTH))) Then
                        bdfrmlPersonnelREFER_v2.Enabled = False
                        bdfrmlPersonnelREFER_v2.Checked = False
                    End If
                Else
                    bdfrmlPersonnelREFER_v2.Enabled = False
                    bdfrmlPersonnelREFER_v2.Checked = False
                End If
            End If

            If (findingsControl Is bdFrmlAppAuthFindings_v2) Then
                FrmlAppAuthREFER_v2.Checked = False
                FrmlAppAuthREFER_v2.Enabled = False
            End If
        End Sub

        Protected Function HasDESReferFindings(ByVal findings As LineOfDutyFindings) As Boolean
            If (findings Is Nothing) Then
                Return False
            End If

            If (Not findings.Finding.HasValue() OrElse (findings.Finding.HasValue() AndAlso findings.Finding <> LodFinding.NotInLineOfDutyNotDueToOwnMisconduct)) Then
                Return False
            End If

            Return True
        End Function

#End Region

#Region "UserAction"


        Private Sub TabButtonClicked(ByRef sender As Object, ByRef e As TabNavigationEventArgs)

            If (e.ButtonType = NavigatorButtonType.Delete) Then
                If (_lod Is Nothing) Then
                    _lod = LodService.GetById(CType(Request.QueryString("refId"), Integer))
                End If
            End If
            If (e.ButtonType = NavigatorButtonType.Save OrElse e.ButtonType = NavigatorButtonType.NavigatedAway _
              OrElse e.ButtonType = NavigatorButtonType.NextStep OrElse e.ButtonType = NavigatorButtonType.PreviousStep) Then

                If (lod.Workflow = 1) Then
                    SaveData()
                Else
                    SaveData_v2()
                End If
            End If

        End Sub
#End Region


    End Class
End Namespace
