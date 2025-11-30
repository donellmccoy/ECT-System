Imports ALOD.Core.Domain.Common
Imports ALOD.Core.Domain.DBSign
Imports ALOD.Core.Domain.Modules.Lod
Imports ALOD.Core.Domain.Modules.Reinvestigations
Imports ALOD.Core.Domain.Users
Imports ALOD.Core.Domain.Workflow
Imports ALOD.Core.Interfaces.DAOInterfaces
Imports ALOD.Data
Imports ALOD.Data.Services
Imports ALOD.Logging
Imports ALOD.Web.UserControls
Imports ALODWebUtility.Common

Namespace Web.RR

    Partial Class Secure_rr_lodBoard
        Inherits System.Web.UI.Page
        Private _scAccess As Dictionary(Of String, ALOD.Core.Domain.Workflow.PageAccessType)
        Private CAN_EDIT_SESSION_KEY As String = "CanEdit"
        Dim dao As ILineOfDutyDao
        Private lineOfDuty As LineOfDuty = Nothing
        Private lodId As Integer = 0
        Private PAGE_TITLE As String = "RR Board Review"
        Dim rdao As ILODReinvestigateDAO
        Private ReinvestigationRequest As LODReinvestigation

#Region "Properties..."

        ReadOnly Property LODDao() As ILineOfDutyDao
            Get
                If (dao Is Nothing) Then
                    dao = New NHibernateDaoFactory().GetLineOfDutyDao()
                End If

                Return dao
            End Get
        End Property

        ReadOnly Property rLODDao() As ILODReinvestigateDAO
            Get
                If (rdao Is Nothing) Then
                    rdao = New NHibernateDaoFactory().GetLODReinvestigationDao()
                End If

                Return rdao
            End Get
        End Property

        Public ReadOnly Property Navigator() As TabNavigator
            Get
                Return MasterPage.Navigator
            End Get
        End Property

        Public ReadOnly Property ReferenceId() As Integer
            Get
                Return Integer.Parse(Request.QueryString("requestId"))
            End Get
        End Property

        Public ReadOnly Property TabControl() As TabControls
            Get
                Return MasterPage.TabControl
            End Get
        End Property

        Protected ReadOnly Property LOD() As LineOfDuty
            Get
                If (lineOfDuty Is Nothing) Then
                    lodId = rLOD().InitialLodId
                    lineOfDuty = LODDao.GetById(lodId, False)
                    Session("RefId") = lodId
                End If

                Return lineOfDuty
            End Get
        End Property

        Protected ReadOnly Property MasterPage() As ReinvestigationRequestMaster
            Get
                Dim master As ReinvestigationRequestMaster = CType(Page.Master, ReinvestigationRequestMaster)
                Return master
            End Get
        End Property

        Protected ReadOnly Property rLOD() As LODReinvestigation
            Get
                If (ReinvestigationRequest Is Nothing) Then
                    ReinvestigationRequest = rLODDao.GetById(ReferenceId, False)

                End If
                Return ReinvestigationRequest
            End Get
        End Property

        Protected ReadOnly Property SectionList() As Dictionary(Of String, ALOD.Core.Domain.Workflow.PageAccessType)
            Get
                If (_scAccess Is Nothing) Then
                    _scAccess = rLOD.ReadSectionList(CInt(Session("GroupId")))
                End If
                Return _scAccess
            End Get
        End Property

        Private Property UserCanEdit() As Boolean
            Get
                If (ViewState(CAN_EDIT_SESSION_KEY) Is Nothing) Then
                    ViewState(CAN_EDIT_SESSION_KEY) = False
                End If
                Return CBool(ViewState(CAN_EDIT_SESSION_KEY))
            End Get
            Set(ByVal value As Boolean)
                ViewState(CAN_EDIT_SESSION_KEY) = value
            End Set
        End Property

#End Region

#Region "Load"

        Public Sub InitiateControls()
            ucWingJAFindings.SetAccess(SectionList(RRSectionNames.RR_WING_JA_REV.ToString()))
            ucWingCCFindings.SetAccess(SectionList(RRSectionNames.RR_WING_CC_REV.ToString()))
            ucBoardAdminFindings.SetAccess(SectionList(RRSectionNames.RR_BOARD_A1_REV.ToString()))
            ucBoardMedicalFindings.SetAccess(SectionList(RRSectionNames.RR_BOARD_MED_REV.ToString()))
            ucBoardJAFindings.SetAccess(SectionList(RRSectionNames.RR_BOARD_LEGAL_REV.ToString()))
            ucApprovingAuthorityFindings.SetAccess(SectionList(RRSectionNames.RR_BOARD_APPROVING_AUTH_REV.ToString()))
        End Sub

        Public Sub LoadData()
            LoadFindingsControl(ucWingJAFindings, PersonnelTypes.WING_JA, UserGroups.WingJudgeAdvocate)
            LoadFindingsSignature(ucSigCheckWingJA, PersonnelTypes.WING_JA)

            LoadFindingsControl(ucWingCCFindings, PersonnelTypes.APPOINT_AUTH, UserGroups.WingCommander)
            LoadFindingsSignature(ucSigCheckWingCC, PersonnelTypes.APPOINT_AUTH)

            LoadFindingsControl(ucBoardAdminFindings, PersonnelTypes.BOARD_A1, UserGroups.BoardAdministrator)
            LoadFindingsSignature(ucSigCheckBoardAdmin, PersonnelTypes.BOARD_A1)

            LoadFindingsControl(ucBoardMedicalFindings, PersonnelTypes.BOARD_SG, UserGroups.BoardMedical)
            LoadFindingsSignature(ucSigCheckBoardMedical, PersonnelTypes.BOARD_SG)

            LoadFindingsControl(ucBoardJAFindings, PersonnelTypes.BOARD_JA, UserGroups.BoardLegal)
            LoadFindingsSignature(ucSigCheckBoardJA, PersonnelTypes.BOARD_JA)

            LoadFindingsControl(ucApprovingAuthorityFindings, PersonnelTypes.BOARD_AA, UserGroups.BoardApprovalAuthority)
            LoadFindingsSignature(ucSigCheckApprovingAuthority, PersonnelTypes.BOARD_AA)
        End Sub

        Public Sub SaveData()
            If (Not UserCanEdit) Then
                Exit Sub
            End If

            SaveFinding(ucWingJAFindings, PersonnelTypes.WING_JA, RRSectionNames.RR_WING_JA_REV)
            SaveFinding(ucWingCCFindings, PersonnelTypes.APPOINT_AUTH, RRSectionNames.RR_WING_CC_REV)
            SaveFinding(ucBoardAdminFindings, PersonnelTypes.BOARD_A1, RRSectionNames.RR_BOARD_A1_REV)
            SaveFinding(ucBoardMedicalFindings, PersonnelTypes.BOARD_SG, RRSectionNames.RR_BOARD_MED_REV)
            SaveFinding(ucBoardJAFindings, PersonnelTypes.BOARD_JA, RRSectionNames.RR_BOARD_LEGAL_REV)
            SaveFinding(ucApprovingAuthorityFindings, PersonnelTypes.BOARD_AA, RRSectionNames.RR_BOARD_APPROVING_AUTH_REV)
        End Sub

        Protected Function GetLegacyTemplateForPersonnelType(pType As PersonnelTypes) As DBSignTemplateId
            If (pType = PersonnelTypes.APPOINT_AUTH) Then
                Return DBSignTemplateId.WingCCRR
            ElseIf (pType = PersonnelTypes.BOARD_AA) Then
                Return DBSignTemplateId.Form348RRAA
            Else
                Return DBSignTemplateId.SignOnly
            End If
        End Function

        Protected Function HasReadWriteAccessForSection(ByVal sectionName As RRSectionNames) As Boolean
            If (SectionList(sectionName.ToString()) <> ALOD.Core.Domain.Workflow.PageAccessType.ReadWrite) Then
                Return False
            End If

            Return True
        End Function

        Protected Function IsPersonnelTypeWithLegacySignatureInformation(ByVal pType As PersonnelTypes)
            If (pType = PersonnelTypes.APPOINT_AUTH OrElse pType = PersonnelTypes.BOARD_AA) Then
                Return True
            End If

            Return False
        End Function

        Protected Sub LoadFindingsControl(ByVal userControl As Secure_Shared_UserControls_FindingsControl, ByVal pType As PersonnelTypes, ByVal groupId As Integer)
            Dim cFinding As LODReinvestigationFindings

            cFinding = rLOD.FindFindingByPersonnelType(pType)

            userControl.LoadFindingOptionsByWorkflow(rLOD.Workflow, groupId)

            If (cFinding Is Nothing) Then
                Exit Sub
            End If

            If (cFinding.Finding.HasValue) Then
                userControl.Findings = cFinding.Finding.Value
            End If

            userControl.FormFindingsText = cFinding.Explanation
        End Sub

        Protected Sub LoadFindingsSignature(ByVal sigCheck As Secure_Shared_UserControls_SigntureCheck, ByVal pType As PersonnelTypes)
            Dim cFinding As LODReinvestigationFindings

            cFinding = rLOD.FindFindingByPersonnelType(pType)

            sigCheck.Visible = False

            If (Not rLOD.HasAFinding(cFinding)) Then
                Exit Sub
            End If

            If (cFinding.IsLegacyFinding AndAlso IsPersonnelTypeWithLegacySignatureInformation(pType)) Then
                sigCheck.Template = GetLegacyTemplateForPersonnelType(pType)
            ElseIf (cFinding.IsLegacyFinding) Then
                Exit Sub
            End If

            sigCheck.Visible = True
            sigCheck.VerifySignature(ReferenceId, pType)
        End Sub

        Protected Sub Page_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
            AddHandler CType(Page.Master, ReinvestigationRequestMaster).TabClick, AddressOf TabButtonClicked
        End Sub

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

            If (Not IsPostBack) Then
                UserCanEdit = GetAccess(Navigator.PageAccess, True)
                InitiateControls()
                LoadData()

                LogManager.LogAction(ModuleType.ReinvestigationRequest, UserAction.ViewPage, ReferenceId, "Viewed Page: LOD Board")
            End If
        End Sub

        Protected Sub SaveFinding(ByVal userControl As Secure_Shared_UserControls_FindingsControl, ByVal pType As PersonnelTypes, ByVal sectionName As RRSectionNames)
            If (Not HasReadWriteAccessForSection(sectionName)) Then
                Exit Sub
            End If

            Dim cFinding As LODReinvestigationFindings = rLOD.CreateEmptyFindingForUser(UserService.CurrentUser)

            cFinding.PType = pType

            If (userControl.Findings IsNot Nothing AndAlso userControl.Findings > 0) Then
                cFinding.Finding = userControl.Findings
            End If

            cFinding.Explanation = userControl.FormFindingsText

            rLOD.SetFindingByType(cFinding)
            rdao.SaveOrUpdate(rLOD)
            rdao.CommitChanges()
        End Sub

        Private Sub TabButtonClicked(ByRef sender As Object, ByRef e As TabNavigationEventArgs)

            If (e.ButtonType = NavigatorButtonType.Save OrElse e.ButtonType = NavigatorButtonType.NavigatedAway _
              OrElse e.ButtonType = NavigatorButtonType.NextStep OrElse e.ButtonType = NavigatorButtonType.PreviousStep) Then
                SaveData()
            End If

        End Sub

#End Region

    End Class

End Namespace