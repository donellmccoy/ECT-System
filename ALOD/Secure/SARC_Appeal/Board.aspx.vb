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

Namespace Web.APSA

    Partial Class Secure_apsa_Board
        Inherits System.Web.UI.Page

#Region "Fields..."

        Protected Const PAGE_TITLE As String = "SARC AP Board"

        Private _daoFactory As NHibernateDaoFactory
        Private _pageAccessDictionary As Dictionary(Of String, ALOD.Core.Domain.Workflow.PageAccessType)
        Private _sarc As SARCAppeal
        Private _sarcDao As ISARCAppealDAO

#End Region

#Region "Properties..."

        Public ReadOnly Property ReferenceId() As Integer
            Get
                Return Integer.Parse(Request.QueryString("requestId"))
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

        Protected ReadOnly Property MasterPage() As SARCAppealMaster
            Get
                Dim master As SARCAppealMaster = CType(Page.Master, SARCAppealMaster)
                Return master
            End Get
        End Property

        Protected ReadOnly Property ModuleType() As ModuleType
            Get
                Return ModuleType.SARCAppeal
            End Get
        End Property

        Protected ReadOnly Property Navigator() As TabNavigator
            Get
                Return MasterPage.Navigator
            End Get
        End Property

        Protected ReadOnly Property SARC As SARCAppeal
            Get
                If (_sarc Is Nothing) Then
                    _sarc = SARCDao.GetById(ReferenceId)
                End If

                Return _sarc
            End Get
        End Property

        Protected ReadOnly Property SARCDao As ISARCAppealDAO
            Get
                If (_sarcDao Is Nothing) Then
                    _sarcDao = DAOFactory.GetSARCAppealDao()
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

            If (findingsControl IsNot ucAppellateAuthorityFindings) Then
                Exit Sub
            End If

            If (Short.Parse(findingsControl.Findings) = Finding.Request_Consultation) Then
                tblConsultation.Visible = True
            Else
                tblConsultation.Visible = False
                ddlBoardMembers.SelectedValue = 0
            End If
        End Sub

        Protected Function HasReadWriteAccessForSection(ByVal sectionName As APSASectionNames) As Boolean
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

            If (IsAppellateAuthorityFindingRequestForConsultation()) Then
                tblConsultation.Visible = True
            End If

            If (SectionList(APSASectionNames.APSA_APPELLATE_AUTH_REV.ToString()) = ALOD.Core.Domain.Workflow.PageAccessType.ReadWrite) Then
                ddlBoardMembers.Visible = True
            Else
                lblBoardMember.Visible = True
            End If
        End Sub

        Protected Sub InitiateControls()
            ucBoardMedicalFindings.SetAccess(SectionList(APSASectionNames.APSA_BOARD_MED_REV.ToString()))
            ucBoardJAFindings.SetAccess(SectionList(APSASectionNames.APSA_BOARD_LEGAL_REV.ToString()))
            ucBoardAdminFindings.SetAccess(SectionList(APSASectionNames.APSA_BOARD_ADMIN_REV.ToString()))
            ucAppellateAuthorityFindings.SetAccess(SectionList(APSASectionNames.APSA_APPELLATE_AUTH_REV.ToString()))
            InitConsultationControls()
        End Sub

        Protected Function IsAppellateAuthorityFindingRequestForConsultation() As Boolean
            Dim cFinding As SARCAppealFindings = SARC.FindFindingByPersonnelType(PersonnelTypes.APPELLATE_AUTH)

            If (Not AppealSARCUtility.HasFinding(cFinding) OrElse cFinding.Finding <> Finding.Request_Consultation) Then
                Return False
            End If

            Return True
        End Function

        Protected Sub LoadAppellateAuthorityControls()
            LoadFindingsControl(ucAppellateAuthorityFindings, PersonnelTypes.APPELLATE_AUTH, UserGroups.AppellateAuthority)
            LoadFindingsSignature(ucSigCheckAppellateAuth, PersonnelTypes.APPELLATE_AUTH)

            If (Not IsAppellateAuthorityFindingRequestForConsultation() OrElse Not SARC.ConsultationFromUserGroupId.HasValue) Then
                Exit Sub
            End If

            ddlBoardMembers.SelectedValue = SARC.ConsultationFromUserGroupId.Value
            lblBoardMember.Text = ddlBoardMembers.Items.FindByValue(SARC.ConsultationFromUserGroupId.Value).Text
        End Sub

        Protected Sub LoadData()
            LoadAppellateAuthorityControls()

            LoadFindingsControl(ucBoardMedicalFindings, PersonnelTypes.BOARD_SG, UserGroups.BoardMedical)
            LoadFindingsSignature(ucSigCheckBoardMedical, PersonnelTypes.BOARD_SG)

            LoadFindingsControl(ucBoardJAFindings, PersonnelTypes.BOARD_JA, UserGroups.BoardLegal)
            LoadFindingsSignature(ucSigCheckBoardJA, PersonnelTypes.BOARD_JA)

            LoadFindingsControl(ucBoardAdminFindings, PersonnelTypes.BOARD_A1, UserGroups.BoardAdministrator)
            LoadFindingsSignature(ucSigCheckBoardAdmin, PersonnelTypes.BOARD_A1)
        End Sub

        Protected Sub LoadFindingsControl(ByVal userControl As Secure_Shared_UserControls_FindingsControl, ByVal pType As PersonnelTypes, ByVal groupId As Integer)
            Dim cFinding As SARCAppealFindings

            cFinding = SARC.FindFindingByPersonnelType(pType)

            If (userControl Is ucAppellateAuthorityFindings) Then
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

            userControl.FormFindingsText = cFinding.Remarks
        End Sub

        Protected Sub LoadFindingsSignature(ByVal sigCheck As Secure_Shared_UserControls_SigntureCheck, ByVal pType As PersonnelTypes)
            Dim cFinding As SARCAppealFindings

            cFinding = SARC.FindFindingByPersonnelType(pType)

            sigCheck.Visible = False

            If (Not AppealSARCUtility.HasFinding(cFinding)) Then
                Exit Sub
            End If

            If (cFinding.IsLegacyFinding) Then
                Exit Sub
            End If

            sigCheck.Visible = True
            sigCheck.VerifySignature(ReferenceId, pType)
        End Sub

        Protected Sub Page_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
            AddHandler CType(Page.Master, SARCAppealMaster).TabClick, AddressOf TabButtonClicked
            AddHandler ucAppellateAuthorityFindings.FindingSelected, AddressOf FindingSelected
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

            If (AppealSARCUtility.IsTimeToSaveViaNavigator(e.ButtonType)) Then
                SaveData()
            End If
        End Sub

        Protected Sub SaveApprovingAuthorityFindings()
            SaveFinding(ucAppellateAuthorityFindings, PersonnelTypes.APPELLATE_AUTH, APSASectionNames.APSA_APPELLATE_AUTH_REV)

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

            SaveFinding(ucBoardMedicalFindings, PersonnelTypes.BOARD_SG, APSASectionNames.APSA_BOARD_MED_REV)
            SaveFinding(ucBoardJAFindings, PersonnelTypes.BOARD_JA, APSASectionNames.APSA_BOARD_LEGAL_REV)
            SaveFinding(ucBoardAdminFindings, PersonnelTypes.BOARD_A1, APSASectionNames.APSA_BOARD_ADMIN_REV)
            SaveApprovingAuthorityFindings()
        End Sub

        Protected Sub SaveFinding(ByVal userControl As Secure_Shared_UserControls_FindingsControl, ByVal pType As PersonnelTypes, ByVal sectionName As APSASectionNames)
            If (Not HasReadWriteAccessForSection(sectionName)) Then
                Exit Sub
            End If

            Dim cFinding As SARCAppealFindings
            cFinding = AppealSARCUtility.CreateSARCFinding(SARC.Id)
            cFinding.PType = pType

            If (userControl.Findings IsNot Nothing AndAlso userControl.Findings > 0) Then
                cFinding.Finding = userControl.Findings
            End If

            cFinding.Remarks = userControl.FormFindingsText

            SARC.SetFindingByType(cFinding)

            SARCDao.SaveOrUpdate(SARC)
        End Sub

        Protected Sub TabButtonClicked(ByRef sender As Object, ByRef e As TabNavigationEventArgs)
            If (SARCUtility.IsTimeToSaveViaNavigator(e.ButtonType)) Then
                SaveData()
            End If
        End Sub

#End Region

    End Class

End Namespace