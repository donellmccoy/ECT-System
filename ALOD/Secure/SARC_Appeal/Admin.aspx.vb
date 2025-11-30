Imports ALOD.Core.Domain.Common
Imports ALOD.Core.Domain.Modules.Lod
Imports ALOD.Core.Domain.Modules.SARC
Imports ALOD.Core.Domain.Users
Imports ALOD.Core.Domain.Workflow
Imports ALOD.Core.Interfaces.DAOInterfaces
Imports ALOD.Data
Imports ALOD.Logging
Imports ALOD.Web.UserControls
Imports ALODWebUtility.Common

Namespace Web.APSA

    Partial Class Admin
        Inherits System.Web.UI.Page

#Region "Fields..."

        Protected Const PAGE_TITLE As String = "SARC AP Admin"

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

        Protected Function HasReadWriteAccessForSection(ByVal sectionName As APSASectionNames) As Boolean
            If (SectionList(sectionName.ToString()) <> ALOD.Core.Domain.Workflow.PageAccessType.ReadWrite) Then
                Return False
            End If

            Return True
        End Function

        Protected Sub InitiateControls()
            SetFindingsControlAccess(ucSARCAdminFindings, SectionList(APSASectionNames.APSA_SARC_ADMIN.ToString()))
        End Sub

        Protected Sub LoadData()
            LoadFindingsControl(ucSARCAdminFindings, PersonnelTypes.SARC_ADMIN, UserGroups.SARCAdmin)
            LoadFindingsSignature(ucSigCheckSARCAdmin, PersonnelTypes.SARC_ADMIN)
        End Sub

        Protected Sub LoadFindingsControl(ByVal userControl As Secure_Shared_UserControls_FindingsControl, ByVal pType As PersonnelTypes, ByVal groupId As Integer)
            Dim cFinding As SARCAppealFindings

            cFinding = SARC.FindFindingByPersonnelType(pType)

            userControl.LoadFindingOptionsByWorkflow(SARC.Workflow, groupId, ExcludedFindings, False)

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
        End Sub

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
            If (Not IsPostBack) Then
                UserCanEdit = GetAccess(Navigator.PageAccess, True)
                InitiateControls()
                LoadData()

                LogManager.LogAction(ModuleType.SARCAppeal, UserAction.ViewPage, ReferenceId, "Viewed Page: " & PAGE_TITLE)
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

        Protected Sub SaveData()
            If (Not UserCanEdit) Then
                Exit Sub
            End If

            If (Not HasReadWriteAccessForSection(APSASectionNames.APSA_SARC_ADMIN)) Then
                Exit Sub
            End If

            Dim cFinding As SARCAppealFindings
            cFinding = AppealSARCUtility.CreateSARCFinding(SARC.Id)
            cFinding.PType = PersonnelTypes.SARC_ADMIN

            If (ucSARCAdminFindings.Findings IsNot Nothing AndAlso ucSARCAdminFindings.Findings > 0) Then
                cFinding.Finding = ucSARCAdminFindings.Findings
            End If

            cFinding.Remarks = ucSARCAdminFindings.FormFindingsText

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