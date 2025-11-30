Imports ALOD.Core.Domain.Common
Imports ALOD.Core.Domain.Modules.Lod
Imports ALOD.Core.Domain.Modules.SARC
Imports ALOD.Core.Domain.Users
Imports ALOD.Core.Domain.Workflow
Imports ALOD.Core.Interfaces.DAOInterfaces
Imports ALOD.Core.Utils
Imports ALOD.Data
Imports ALOD.Logging
Imports ALOD.Web.UserControls
Imports ALODWebUtility.Common

Namespace Web.SARC

    Public Class Board
        Inherits System.Web.UI.Page

#Region "Fields..."

        Protected Const PAGE_TITLE As String = "SARC Board"

        Private _daoFactory As NHibernateDaoFactory
        Private _pageAccessDictionary As Dictionary(Of String, ALOD.Core.Domain.Workflow.PageAccessType)
        Private _sarc As RestrictedSARC
        Private _sarcDao As ISARCDAO

#End Region

#Region "Properties..."

        Public ReadOnly Property ReferenceId() As Integer
            Get
                Return Integer.Parse(Request.QueryString("refId"))
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

        Protected ReadOnly Property DAOFactory As NHibernateDaoFactory
            Get
                If (_daoFactory Is Nothing) Then
                    _daoFactory = New NHibernateDaoFactory()
                End If

                Return _daoFactory
            End Get
        End Property

        Protected ReadOnly Property ExcludedFindings As String
            Get
                Return "REQUEST_CONSULT"
            End Get
        End Property

        Protected ReadOnly Property MasterPage() As SARCMaster
            Get
                Dim master As SARCMaster = CType(Page.Master, SARCMaster)
                Return master
            End Get
        End Property

        Protected ReadOnly Property ModuleType() As ModuleType
            Get
                Return ModuleType.SARC
            End Get
        End Property

        Protected ReadOnly Property Navigator() As TabNavigator
            Get
                Return MasterPage.Navigator
            End Get
        End Property

        Protected ReadOnly Property SARC As RestrictedSARC
            Get
                If (_sarc Is Nothing) Then
                    _sarc = SARCDao.GetById(ReferenceId)
                End If

                Return _sarc
            End Get
        End Property

        Protected ReadOnly Property SARCDao As ISARCDAO
            Get
                If (_sarcDao Is Nothing) Then
                    _sarcDao = DAOFactory.GetSARCDao()
                End If

                Return _sarcDao
            End Get
        End Property

        Protected ReadOnly Property SectionList() As Dictionary(Of String, ALOD.Core.Domain.Workflow.PageAccessType)
            Get
                If (_pageAccessDictionary Is Nothing) Then
                    _pageAccessDictionary = SARC.ReadSectionList(CInt(Session("GroupId")))
                End If

                Return _pageAccessDictionary
            End Get
        End Property

        Protected ReadOnly Property TabControl() As TabControls
            Get
                Return MasterPage.TabControl
            End Get
        End Property

#End Region

#Region "Page Methods..."

        Protected Sub FindingSelected(sender As Object, e As RadioButtonSelectedEventArgs)
            Dim findingsControl As Secure_Shared_UserControls_FindingsControl = CType(sender, Secure_Shared_UserControls_FindingsControl)

            If (findingsControl IsNot ucApprovingAuthorityFindings) Then
                Exit Sub
            End If

            If (Short.Parse(findingsControl.Findings) = Finding.Request_Consultation) Then
                tblConsultation.Visible = True
            Else
                tblConsultation.Visible = False
                ddlBoardMembers.SelectedValue = 0
            End If
        End Sub

        Protected Function HasReadWriteAccessForSection(ByVal sectionName As SARCSectionNames) As Boolean
            If (SectionList(sectionName.ToString()) <> ALOD.Core.Domain.Workflow.PageAccessType.ReadWrite) Then
                Return False
            End If

            Return True
        End Function

        Protected Sub InitBoardMembersDDL()
            ddlBoardMembers.Items.Clear()

            ddlBoardMembers.Items.Add(New ListItem("-- Select Board Member --", 0))
            ddlBoardMembers.Items.Add(New ListItem("Board Administrator", UserGroups.BoardAdministrator))
            ddlBoardMembers.Items.Add(New ListItem("Board Medical", UserGroups.BoardMedical))
            ddlBoardMembers.Items.Add(New ListItem("Board JA", UserGroups.BoardLegal))
            ddlBoardMembers.Items.Add(New ListItem("Senior Medical Reviewer", UserGroups.SeniorMedicalReviewer))

            ddlBoardMembers.SelectedValue = 0
        End Sub

        Protected Sub InitConsultationControls()
            InitBoardMembersDDL()

            If (IsApprovingAuthorityFindingRequestForConsultation()) Then
                tblConsultation.Visible = True
            End If

            If (SectionList(SARCSectionNames.SARC_BOARD_APPROVING_AUTH_REV.ToString()) = ALOD.Core.Domain.Workflow.PageAccessType.ReadWrite) Then
                ddlBoardMembers.Visible = True
            Else
                lblBoardMember.Visible = True
            End If
        End Sub

        Protected Sub InitiateControls()
            SetFindingsControlAccess(ucBoardMedicalFindings, SectionList(SARCSectionNames.SARC_BOARD_MED_REV.ToString()))
            SetFindingsControlAccess(ucBoardJAFindings, SectionList(SARCSectionNames.SARC_BOARD_JA_REV.ToString()))
            SetFindingsControlAccess(ucApprovingAuthorityFindings, SectionList(SARCSectionNames.SARC_BOARD_APPROVING_AUTH_REV.ToString()))
            SetFindingsControlAccess(ucBoardAdminFindings, SectionList(SARCSectionNames.SARC_BOARD_ADMIN_REV.ToString()))
            InitConsultationControls()
        End Sub

        Protected Function IsApprovingAuthorityFindingRequestForConsultation() As Boolean
            Dim cFinding As RestrictedSARCFindings = SARC.FindByType(PersonnelTypes.BOARD_AA)

            If (Not SARCUtility.HasFinding(cFinding) OrElse cFinding.Finding <> Finding.Request_Consultation) Then
                Return False
            End If

            Return True
        End Function

        Protected Sub LoadApprovingAuthorityControls()
            LoadFindingsControl(ucApprovingAuthorityFindings, PersonnelTypes.BOARD_AA, UserGroups.BoardApprovalAuthority)
            LoadFindingsSignature(ucSigCheckApprovingAuth, PersonnelTypes.BOARD_AA)

            If (Not IsApprovingAuthorityFindingRequestForConsultation() OrElse Not SARC.ConsultationFromUserGroupId.HasValue) Then
                Exit Sub
            End If

            ddlBoardMembers.SelectedValue = SARC.ConsultationFromUserGroupId.Value
            lblBoardMember.Text = ddlBoardMembers.Items.FindByValue(SARC.ConsultationFromUserGroupId.Value).Text
        End Sub

        Protected Sub LoadData()
            LoadApprovingAuthorityControls()

            LoadFindingsControl(ucBoardMedicalFindings, PersonnelTypes.BOARD_SG, UserGroups.BoardMedical)
            LoadFindingsSignature(ucSigCheckBoardMedical, PersonnelTypes.BOARD_SG)

            LoadFindingsControl(ucBoardJAFindings, PersonnelTypes.BOARD_JA, UserGroups.BoardLegal)
            LoadFindingsSignature(ucSigCheckBoardJA, PersonnelTypes.BOARD_JA)

            LoadFindingsControl(ucBoardAdminFindings, PersonnelTypes.BOARD_A1, UserGroups.BoardAdministrator)
            LoadFindingsSignature(ucSigCheckBoardAdmin, PersonnelTypes.BOARD_A1)
        End Sub

        Protected Sub LoadFindingsControl(ByVal userControl As Secure_Shared_UserControls_FindingsControl, ByVal pType As PersonnelTypes, ByVal groupId As Integer)
            Dim cFinding As RestrictedSARCFindings

            cFinding = SARC.FindByType(pType)

            If (userControl Is ucApprovingAuthorityFindings) Then
                userControl.LoadFindingOptionsByWorkflow(SARC.Workflow, groupId)
            Else
                userControl.LoadFindingOptionsByWorkflow(SARC.Workflow, groupId, ExcludedFindings, False)
            End If

            If (cFinding Is Nothing) Then
                Exit Sub
            End If

            If (cFinding.Finding.HasValue) Then
                userControl.Findings = cFinding.Finding.Value
            End If

            userControl.Remarks = cFinding.Remarks

            If (userControl.ShowFormText) Then
                userControl.FormFindingsText = cFinding.FindingsText
            End If
        End Sub

        Protected Sub LoadFindingsSignature(ByVal sigCheck As Secure_Shared_UserControls_SigntureCheck, ByVal pType As PersonnelTypes)
            Dim cFinding As RestrictedSARCFindings

            cFinding = SARC.FindByType(pType)

            sigCheck.Visible = False

            If (Not SARCUtility.HasFinding(cFinding)) Then
                Exit Sub
            End If

            sigCheck.Visible = True
            sigCheck.VerifySignature(ReferenceId, pType)
        End Sub

        Protected Sub Page_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
            AddHandler CType(Page.Master, SARCMaster).TabClick, AddressOf TabButtonClicked
            AddHandler ucApprovingAuthorityFindings.FindingSelected, AddressOf FindingSelected
        End Sub

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
            If (Not IsPostBack) Then
                UserCanEdit = GetAccess(Navigator.PageAccess, True)
                InitiateControls()
                LoadData()

                LogManager.LogAction(ModuleType, UserAction.ViewPage, ReferenceId, "Viewed Page: " & PAGE_TITLE)
            End If
        End Sub

        Protected Sub Save_Click(ByRef sender As Object, ByRef e As TabNavigationEventArgs)
            If (Navigator.CurrentStep.IsReadOnly) Then
                Exit Sub
            End If

            If (SARCUtility.IsTimeToSaveViaNavigator(e.ButtonType)) Then
                SaveData()
            End If
        End Sub

        Protected Sub SaveApprovingAuthorityFindings()
            SaveFinding(ucApprovingAuthorityFindings, PersonnelTypes.BOARD_AA, SARCSectionNames.SARC_BOARD_APPROVING_AUTH_REV)

            If (tblConsultation.Visible AndAlso ddlBoardMembers.SelectedValue > 0) Then
                SARC.ConsultationFromUserGroupId = ddlBoardMembers.SelectedValue
            Else
                SARC.ConsultationFromUserGroupId = Nothing
            End If

            SARCDao.SaveOrUpdate(SARC)
        End Sub

        Protected Sub SaveData()
            If (Not UserCanEdit) Then
                Exit Sub
            End If

            SaveApprovingAuthorityFindings()
            SaveFinding(ucBoardMedicalFindings, PersonnelTypes.BOARD_SG, SARCSectionNames.SARC_BOARD_MED_REV)
            SaveFinding(ucBoardJAFindings, PersonnelTypes.BOARD_JA, SARCSectionNames.SARC_BOARD_JA_REV)
            SaveFinding(ucBoardAdminFindings, PersonnelTypes.BOARD_A1, SARCSectionNames.SARC_BOARD_ADMIN_REV)
        End Sub

        Protected Sub SaveFinding(ByVal userControl As Secure_Shared_UserControls_FindingsControl, ByVal pType As PersonnelTypes, ByVal sectionName As SARCSectionNames)
            If (Not HasReadWriteAccessForSection(sectionName)) Then
                Exit Sub
            End If

            Dim cFinding As RestrictedSARCFindings
            cFinding = SARCUtility.CreateSARCFinding(SARC.Id)
            cFinding.PType = pType

            If (userControl.Findings IsNot Nothing AndAlso userControl.Findings > 0) Then
                cFinding.Finding = userControl.Findings
            End If

            If (userControl.ShowFormText) Then
                cFinding.FindingsText = userControl.FormFindingsText
            End If

            If (userControl.ShowRemarks) Then
                cFinding.Remarks = userControl.Remarks
            End If

            SARC.SetFindingByType(cFinding)

            SARCDao.SaveOrUpdate(SARC)
        End Sub

        Protected Sub SetFindingsControlAccess(ByVal userControl As Secure_Shared_UserControls_FindingsControl, ByVal access As ALOD.Core.Domain.Workflow.PageAccessType)
            If (access <> ALOD.Core.Domain.Workflow.PageAccessType.None) Then
                If (access = ALOD.Core.Domain.Workflow.PageAccessType.ReadWrite) Then
                    userControl.SetReadOnly = False
                End If
            End If
        End Sub

        Protected Sub TabButtonClicked(ByRef sender As Object, ByRef e As TabNavigationEventArgs)
            If (SARCUtility.IsTimeToSaveViaNavigator(e.ButtonType)) Then
                SaveData()
            End If
        End Sub

#End Region

    End Class

End Namespace