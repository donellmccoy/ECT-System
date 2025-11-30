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

    Public Class SeniorMedReviewer
        Inherits System.Web.UI.Page

        Private _apDao As ILODAppealDAO
        Private _appealRequest As LODAppeal
        Private _daoFactory As IDaoFactory
        Private _scAccess As Dictionary(Of String, ALOD.Core.Domain.Workflow.PageAccessType)

#Region "Properties"

        ReadOnly Property APDao() As ILODAppealDAO
            Get
                If (_apDao Is Nothing) Then
                    _apDao = DaoFactory.GetLODAppealDao()
                End If

                Return _apDao
            End Get
        End Property

        Public ReadOnly Property Navigator() As TabNavigator
            Get
                Return Master.Navigator
            End Get
        End Property

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

        Protected ReadOnly Property AP() As LODAppeal
            Get
                If (_appealRequest Is Nothing) Then
                    _appealRequest = APDao.GetById(ReferenceId, False)

                End If
                Return _appealRequest
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

        Protected ReadOnly Property SectionList() As Dictionary(Of String, ALOD.Core.Domain.Workflow.PageAccessType)
            Get
                If (_scAccess Is Nothing) Then
                    _scAccess = AP.ReadSectionList(CInt(Session("GroupId")))
                End If
                Return _scAccess
            End Get
        End Property

#End Region

        Public Sub InitiateControls()
            ucSeniorMedicalFindings.SetAccess(SectionList(APSectionNames.AP_BOARD_SENIOR_MED_REV.ToString()))
        End Sub

        Public Sub LoadData()
            LoadFindingsControl(ucSeniorMedicalFindings, PersonnelTypes.SENIOR_MEDICAL_REVIEWER, UserGroups.SeniorMedicalReviewer)
            LoadFindingsSignature(ucSigCheckSeniorMedical, PersonnelTypes.SENIOR_MEDICAL_REVIEWER)
        End Sub

        Public Sub SaveData()
            If (Not UserCanEdit) Then
                Exit Sub
            End If

            SaveFinding(ucSeniorMedicalFindings, PersonnelTypes.SENIOR_MEDICAL_REVIEWER, APSectionNames.AP_BOARD_SENIOR_MED_REV)
        End Sub

        Protected Function HasReadWriteAccessForSection(ByVal sectionName As APSectionNames) As Boolean
            If (SectionList(sectionName.ToString()) <> PageAccessType.ReadWrite) Then
                Return False
            End If

            Return True
        End Function

        Protected Sub LoadFindingsControl(ByVal userControl As Secure_Shared_UserControls_FindingsControl, ByVal pType As PersonnelTypes, ByVal groupId As Integer)
            Dim cFinding As LODAppealFindings = AP.FindFindingByPersonnelType(pType)
            Dim BoardMedicalFinding As LODAppealFindings = AP.FindFindingByPersonnelType(PersonnelTypes.BOARD_SG)

            userControl.LoadFindingOptionsByWorkflow(AP.Workflow, groupId)

            If (cFinding Is Nothing) Then
                Exit Sub
            End If

            If (Not BoardMedicalFinding Is Nothing AndAlso BoardMedicalFinding.Finding.HasValue) Then
                If cFinding.Concur <> String.Empty Then
                    userControl.Decision = cFinding.Concur
                End If
            Else
                userControl.FindingsOnly = True
            End If

            If (cFinding.Finding.HasValue) Then
                userControl.Findings = cFinding.Finding.Value
            End If

            userControl.FormFindingsText = cFinding.Explanation
        End Sub

        Protected Sub LoadFindingsSignature(ByVal sigCheck As Secure_Shared_UserControls_SigntureCheck, ByVal pType As PersonnelTypes)
            Dim cFinding As LODAppealFindings

            cFinding = AP.FindFindingByPersonnelType(pType)

            sigCheck.Visible = False

            If (Not AP.HasAFinding(cFinding)) Then
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

                LogManager.LogAction(ModuleType.AppealRequest, UserAction.ViewPage, ReferenceId, "Viewed Page: AP Senior Medical")
            End If
        End Sub

        Protected Sub SaveFinding(ByVal userControl As Secure_Shared_UserControls_FindingsControl, ByVal pType As PersonnelTypes, ByVal sectionName As APSectionNames)
            If (Not HasReadWriteAccessForSection(sectionName)) Then
                Exit Sub
            End If

            Dim cFinding As LODAppealFindings = AP.CreateEmptyFindingForUser(UserService.CurrentUser)
            Dim BoardMedicalFinding As LODAppealFindings = AP.FindFindingByPersonnelType(PersonnelTypes.BOARD_SG)

            cFinding.PType = pType

            If (Not BoardMedicalFinding Is Nothing AndAlso BoardMedicalFinding.Finding.HasValue) Then

                cFinding.Concur = userControl.Decision

                If (userControl.Decision.Equals("N")) Then
                    cFinding.Finding = userControl.Findings
                End If
            Else
                cFinding.Finding = userControl.Findings
            End If

            cFinding.Explanation = userControl.FormFindingsText

            AP.SetFindingByType(cFinding)
            APDao.SaveOrUpdate(AP)
            APDao.CommitChanges()
        End Sub

        Private Sub TabButtonClicked(ByRef sender As Object, ByRef e As TabNavigationEventArgs)
            If (e.ButtonType = NavigatorButtonType.Save OrElse e.ButtonType = NavigatorButtonType.NavigatedAway _
              OrElse e.ButtonType = NavigatorButtonType.NextStep OrElse e.ButtonType = NavigatorButtonType.PreviousStep) Then
                SaveData()
            End If
        End Sub

    End Class

End Namespace