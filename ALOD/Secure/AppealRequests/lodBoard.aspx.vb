Imports ALOD.Core.Domain.Common
Imports ALOD.Core.Domain.Modules.Appeals
Imports ALOD.Core.Domain.Modules.Lod
Imports ALOD.Core.Domain.Users
Imports ALOD.Core.Domain.Workflow
Imports ALOD.Core.Interfaces.DAOInterfaces
Imports ALOD.Data
Imports ALOD.Data.Services
Imports ALOD.Logging
Imports ALOD.Web.UserControls
Imports ALODWebUtility.Common

Namespace Web.AP

    Partial Class Secure_ap_lodBoard
        Inherits System.Web.UI.Page

        Private _scAccess As Dictionary(Of String, ALOD.Core.Domain.Workflow.PageAccessType)
        Dim adao As ILODAppealDAO
        Private AppealRequest As LODAppeal
        Private CAN_EDIT_SESSION_KEY As String = "CanEdit"
        Private PAGE_TITLE As String = "AP Board"

#Region "Properties"

        ReadOnly Property aLODDao() As ILODAppealDAO
            Get
                If (adao Is Nothing) Then
                    adao = New NHibernateDaoFactory().GetLODAppealDao()
                End If

                Return adao
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

        Protected ReadOnly Property apLOD() As LODAppeal
            Get
                If (AppealRequest Is Nothing) Then
                    AppealRequest = aLODDao.GetById(ReferenceId, False)

                End If
                Return AppealRequest
            End Get
        End Property

        Protected ReadOnly Property MasterPage() As AppealRequestMaster
            Get
                Dim master As AppealRequestMaster = CType(Page.Master, AppealRequestMaster)
                Return master
            End Get
        End Property

        Protected ReadOnly Property SectionList() As Dictionary(Of String, ALOD.Core.Domain.Workflow.PageAccessType)
            Get
                If (_scAccess Is Nothing) Then
                    _scAccess = apLOD.ReadSectionList(CInt(Session("GroupId")))
                End If
                Return _scAccess
            End Get
        End Property

#End Region

#Region "BoardProperty"

        Protected ReadOnly Property RefId() As Integer
            Get
                Return CInt(Request.QueryString("refId"))
            End Get
        End Property

        Public Sub InitiateControls()
            ucBoardMedicalFindings.SetAccess(SectionList(APSectionNames.AP_BOARD_MED_REV.ToString()))
            ucBoardJAFindings.SetAccess(SectionList(APSectionNames.AP_BOARD_LEGAL_REV.ToString()))
            ucBoardAdminFindings.SetAccess(SectionList(APSectionNames.AP_BOARD_ADMIN_REV.ToString()))
            ucApprovingAuthorityFindings.SetAccess(SectionList(APSectionNames.AP_APPROVING_AUTH_REV.ToString()))
            ucAppellateAuthorityFindings.SetAccess(SectionList(APSectionNames.AP_APPEALLATE_AUTH_REV.ToString()))
        End Sub

        Public Sub LoadData()
            LoadFindingsControl(ucBoardMedicalFindings, PersonnelTypes.BOARD_SG, UserGroups.BoardMedical)
            LoadFindingsSignature(ucSigCheckBoardMedical, PersonnelTypes.BOARD_SG)

            LoadFindingsControl(ucBoardJAFindings, PersonnelTypes.BOARD_JA, UserGroups.BoardLegal)
            LoadFindingsSignature(ucSigCheckBoardJA, PersonnelTypes.BOARD_JA)

            LoadFindingsControl(ucBoardAdminFindings, PersonnelTypes.BOARD_A1, UserGroups.BoardAdministrator)
            LoadFindingsSignature(ucSigCheckBoardAdmin, PersonnelTypes.BOARD_A1)

            LoadFindingsControl(ucApprovingAuthorityFindings, PersonnelTypes.BOARD_AA, UserGroups.BoardApprovalAuthority)
            LoadFindingsSignature(ucSigCheckApprovingAuthority, PersonnelTypes.BOARD_AA)

            LoadFindingsControl(ucAppellateAuthorityFindings, PersonnelTypes.APPELLATE_AUTH, UserGroups.AppellateAuthority)
            LoadFindingsSignature(ucSigCheckAppellateAuthority, PersonnelTypes.APPELLATE_AUTH)
        End Sub

        Public Sub SaveData()
            If (Not UserCanEdit) Then
                Exit Sub
            End If

            SaveFinding(ucBoardMedicalFindings, PersonnelTypes.BOARD_SG, APSectionNames.AP_BOARD_MED_REV)
            SaveFinding(ucBoardJAFindings, PersonnelTypes.BOARD_JA, APSectionNames.AP_BOARD_LEGAL_REV)
            SaveFinding(ucBoardAdminFindings, PersonnelTypes.BOARD_A1, APSectionNames.AP_BOARD_ADMIN_REV)
            SaveFinding(ucApprovingAuthorityFindings, PersonnelTypes.BOARD_AA, APSectionNames.AP_APPROVING_AUTH_REV)
            SaveFinding(ucAppellateAuthorityFindings, PersonnelTypes.APPELLATE_AUTH, APSectionNames.AP_APPEALLATE_AUTH_REV)
        End Sub

        Protected Function HasReadWriteAccessForSection(ByVal sectionName As APSectionNames) As Boolean
            If (SectionList(sectionName.ToString()) <> ALOD.Core.Domain.Workflow.PageAccessType.ReadWrite) Then
                Return False
            End If

            Return True
        End Function

        Protected Sub LoadFindingsControl(ByVal userControl As Secure_Shared_UserControls_FindingsControl, ByVal pType As PersonnelTypes, ByVal groupId As Integer)
            Dim cFinding As LODAppealFindings

            cFinding = apLOD.FindFindingByPersonnelType(pType)

            userControl.LoadFindingOptionsByWorkflow(apLOD.Workflow, groupId)

            If (cFinding Is Nothing) Then
                Exit Sub
            End If

            If (cFinding.Finding.HasValue) Then
                userControl.Findings = cFinding.Finding.Value
            End If

            userControl.FormFindingsText = cFinding.Explanation
        End Sub

        Protected Sub LoadFindingsSignature(ByVal sigCheck As Secure_Shared_UserControls_SigntureCheck, ByVal pType As PersonnelTypes)
            Dim cFinding As LODAppealFindings

            cFinding = apLOD.FindFindingByPersonnelType(pType)

            sigCheck.Visible = False

            If (Not apLOD.HasAFinding(cFinding)) Then
                Exit Sub
            End If

            If (cFinding.IsLegacyFinding) Then
                Exit Sub
            End If

            sigCheck.Visible = True
            sigCheck.VerifySignature(ReferenceId, pType)
        End Sub

        Protected Sub Page_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
            AddHandler CType(Page.Master, AppealRequestMaster).TabClick, AddressOf TabButtonClicked
        End Sub

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
            If (Not IsPostBack) Then
                UserCanEdit = GetAccess(Navigator.PageAccess, True)
                InitiateControls()
                LoadData()

                LogManager.LogAction(ModuleType.AppealRequest, UserAction.ViewPage, ReferenceId, "Viewed Page: AP Board")
            End If
        End Sub

        Protected Sub SaveFinding(ByVal userControl As Secure_Shared_UserControls_FindingsControl, ByVal pType As PersonnelTypes, ByVal sectionName As APSectionNames)
            If (Not HasReadWriteAccessForSection(sectionName)) Then
                Exit Sub
            End If

            Dim cFinding As LODAppealFindings = apLOD.CreateEmptyFindingForUser(UserService.CurrentUser)

            cFinding.PType = pType

            If (userControl.Findings IsNot Nothing AndAlso userControl.Findings > 0) Then
                cFinding.Finding = userControl.Findings
            End If

            cFinding.Explanation = userControl.FormFindingsText

            apLOD.SetFindingByType(cFinding)
            adao.SaveOrUpdate(apLOD)
            adao.CommitChanges()
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