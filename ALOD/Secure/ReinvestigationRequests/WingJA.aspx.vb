Imports ALOD.Core.Domain.Common
Imports ALOD.Core.Domain.Modules.Lod
Imports ALOD.Core.Domain.Modules.Reinvestigations
Imports ALOD.Core.Domain.Users
Imports ALOD.Core.Interfaces.DAOInterfaces
Imports ALOD.Data
Imports ALOD.Data.Services
Imports ALOD.Web.UserControls
Imports ALODWebUtility.Common

Namespace Web.RR

    Partial Class Secure_rr_WingJA
        Inherits System.Web.UI.Page

        Private _scAccess As Dictionary(Of String, ALOD.Core.Domain.Workflow.PageAccessType)
        Dim dao As ILineOfDutyDao
        Private lineOfDuty As LineOfDuty = Nothing
        Private lodId As Integer = 0
        Dim rdao As ILODReinvestigateDAO
        Private ReinvestigationRequest As LODReinvestigation

#Region "Properties"

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
                Return Master.TabControl
            End Get
        End Property

        Public Property UserCanEdit() As Boolean
            Get
                If (ViewState("CanEdit") Is Nothing) Then
                    ViewState("CanEdit") = False
                End If
                Return CBool(ViewState("CanEdit"))
            End Get
            Set(ByVal value As Boolean)
                ViewState("CanEdit") = value
            End Set
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

        Private ReadOnly Property RefId() As Integer
            Get
                Return CInt(Request.QueryString("refId"))
            End Get
        End Property

#End Region

#Region "Page Methods..."

        Protected Function HasReadWriteAccessForSection(ByVal sectionName As RRSectionNames) As Boolean
            If (SectionList(sectionName.ToString()) <> ALOD.Core.Domain.Workflow.PageAccessType.ReadWrite) Then
                Return False
            End If

            Return True
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

            If (cFinding.IsLegacyFinding) Then
                Exit Sub
            End If

            sigCheck.Visible = True
            sigCheck.VerifySignature(ReferenceId, pType)
        End Sub

        Protected Sub Page_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
            AddHandler Me.Master.TabClick, AddressOf TabButtonClicked
        End Sub

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
            If (Not IsPostBack) Then
                UserCanEdit = GetAccess(Navigator.PageAccess, True)
                InitControls()
            End If
        End Sub

        Protected Sub Save_Click(ByRef sender As Object, ByRef e As TabNavigationEventArgs)
            If (Navigator.CurrentStep.IsReadOnly) Then
                Exit Sub
            End If

            If (e.ButtonType = NavigatorButtonType.Save OrElse e.ButtonType = NavigatorButtonType.NavigatedAway _
                OrElse e.ButtonType = NavigatorButtonType.NextStep OrElse e.ButtonType = NavigatorButtonType.PreviousStep) Then
                SaveFindings()
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

        Private Sub InitControls()
            ucWingJAFindings.SetAccess(SectionList(RRSectionNames.RR_WING_JA_REV.ToString()))

            LoadFindingsControl(ucWingJAFindings, PersonnelTypes.WING_JA, UserGroups.WingJudgeAdvocate)
            LoadFindingsSignature(ucSigCheckWingJA, PersonnelTypes.WING_JA)
        End Sub

        Private Sub SaveFindings()
            If (Not UserCanEdit) Then
                Exit Sub
            End If

            SaveFinding(ucWingJAFindings, PersonnelTypes.WING_JA, RRSectionNames.RR_WING_JA_REV)
        End Sub

        Private Sub TabButtonClicked(ByRef sender As Object, ByRef e As TabNavigationEventArgs)

            If (e.ButtonType = NavigatorButtonType.Save OrElse
                e.ButtonType = NavigatorButtonType.NavigatedAway OrElse
                e.ButtonType = NavigatorButtonType.NextStep OrElse
                e.ButtonType = NavigatorButtonType.PreviousStep) Then
                SaveFindings()
            End If

        End Sub

#End Region

    End Class

End Namespace