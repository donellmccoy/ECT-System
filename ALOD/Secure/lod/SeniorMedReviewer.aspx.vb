Imports ALOD.Core.Domain.Common
Imports ALOD.Core.Domain.Modules.Lod
Imports ALOD.Core.Domain.Users
Imports ALOD.Core.Domain.Workflow
Imports ALOD.Core.Interfaces.DAOInterfaces
Imports ALOD.Core.Utils
Imports ALOD.Data
Imports ALOD.Logging
Imports ALOD.Web.UserControls
Imports ALODWebUtility.Common

Namespace Web.LOD

    Public Class SeniorMedReviewer
        Inherits System.Web.UI.Page

        Protected Const optionalFindings As String = InvestigationDecision.APPROVE + "," + InvestigationDecision.DISAPPROVE
        Protected Const optionalInformalFindings As String = InvestigationDecision.APPROVE + "," + InvestigationDecision.DISAPPROVE + "," + InvestigationDecision.NOT_LOD_MISCONDUCT
        Private _daoFactory As IDaoFactory
        Private _lod As LineOfDuty_v2
        Private _lodDao As ILineOfDutyDao
        Private _scAccess As Dictionary(Of String, ALOD.Core.Domain.Workflow.PageAccessType)

#Region "Properties"

        ReadOnly Property LODDao() As ILineOfDutyDao
            Get
                If (_lodDao Is Nothing) Then
                    _lodDao = DaoFactory.GetLineOfDutyDao()
                End If

                Return _lodDao
            End Get
        End Property

        Public ReadOnly Property Navigator() As TabNavigator
            Get
                Return Master.Navigator
            End Get
        End Property

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

        Protected ReadOnly Property DaoFactory As IDaoFactory
            Get
                If (_daoFactory Is Nothing) Then
                    _daoFactory = New NHibernateDaoFactory()
                End If

                Return _daoFactory
            End Get
        End Property

        Protected ReadOnly Property LOD() As LineOfDuty_v2
            Get
                If (_lod Is Nothing) Then
                    _lod = LODDao.GetById(ReferenceId, False)

                End If
                Return _lod
            End Get
        End Property

        Protected ReadOnly Property SectionList() As Dictionary(Of String, ALOD.Core.Domain.Workflow.PageAccessType)
            Get
                If (_scAccess Is Nothing) Then
                    _scAccess = LOD.ReadSectionList(CInt(Session("GroupId")))
                End If
                Return _scAccess
            End Get
        End Property

#End Region

        Public Sub InitiateControls()
            If (LOD.Formal) Then
                ucSeniorMedicalFindings.SetAccess(SectionList(SectionNames.FORMAL_SENIOR_MED_REV.ToString()))
            Else
                ucSeniorMedicalFindings.SetAccess(SectionList(SectionNames.SENIOR_MED_REV.ToString()))
            End If
        End Sub

        Public Sub LoadData()
            If (LOD.Formal) Then
                LoadFindingsControl(ucSeniorMedicalFindings, PersonnelTypes.FORMAL_SENIOR_MEDICAL_REVIEWER, UserGroups.SeniorMedicalReviewer)
                LoadFindingsSignature(ucSigCheckSeniorMedical, PersonnelTypes.FORMAL_SENIOR_MEDICAL_REVIEWER)
            Else
                LoadFindingsControl(ucSeniorMedicalFindings, PersonnelTypes.SENIOR_MEDICAL_REVIEWER, UserGroups.SeniorMedicalReviewer)
                LoadFindingsSignature(ucSigCheckSeniorMedical, PersonnelTypes.SENIOR_MEDICAL_REVIEWER)
            End If
        End Sub

        Public Sub SaveData()
            If (Not UserCanEdit) Then
                Exit Sub
            End If

            If (LOD.Formal) Then
                SaveFinding(ucSeniorMedicalFindings, PersonnelTypes.FORMAL_SENIOR_MEDICAL_REVIEWER, SectionNames.FORMAL_SENIOR_MED_REV)
            Else
                SaveFinding(ucSeniorMedicalFindings, PersonnelTypes.SENIOR_MEDICAL_REVIEWER, SectionNames.SENIOR_MED_REV)
            End If
        End Sub

        Protected Function HasReadWriteAccessForSection(ByVal sectionName As SectionNames) As Boolean
            If (SectionList(sectionName.ToString()) <> PageAccessType.ReadWrite) Then
                Return False
            End If

            Return True
        End Function

        Protected Sub LoadFindingsControl(ByVal userControl As Secure_Shared_UserControls_FindingsControl, ByVal pType As PersonnelTypes, ByVal groupId As Integer)
            Dim cFinding As LineOfDutyFindings = LOD.FindByType(pType)

            Dim BoardMedicalFinding As LineOfDutyFindings
            If (LOD.Formal) Then
                BoardMedicalFinding = LOD.FindByType(PersonnelTypes.FORMAL_BOARD_SG)
                userControl.ConcurWith = "Formal Board Medical"
                FindingsTitle.Text = "Formal Senior Medical Reviewer LOD Decision"
                userControl.LoadFindingOptionsByWorkflow(LOD.Workflow, groupId, optionalFindings, True)
            Else
                BoardMedicalFinding = LOD.FindByType(PersonnelTypes.BOARD_SG)
                userControl.ConcurWith = "Board Medical"
                FindingsTitle.Text = "Senior Medical Reviewer LOD Decision"
                userControl.LoadFindingOptionsByWorkflow(LOD.Workflow, groupId, optionalInformalFindings, False)
            End If

            If (cFinding Is Nothing) Then
                Exit Sub
            End If

            If (Not BoardMedicalFinding Is Nothing AndAlso BoardMedicalFinding.Finding.HasValue) Then
                If cFinding.DecisionYN <> String.Empty Then
                    userControl.Decision = cFinding.DecisionYN
                End If
            ElseIf (LOD.Formal AndAlso Not BoardMedicalFinding Is Nothing AndAlso Not String.IsNullOrEmpty(BoardMedicalFinding.DecisionYN)) Then
                If cFinding.DecisionYN <> String.Empty Then
                    userControl.Decision = cFinding.DecisionYN
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
            Dim cFinding As LineOfDutyFindings

            cFinding = LOD.FindByType(pType)

            sigCheck.Visible = False

            If (Not LOD.HasAFinding(cFinding)) Then
                Exit Sub
            End If

            sigCheck.Visible = True
            sigCheck.VerifySignature(ReferenceId, pType)
        End Sub

        Protected Sub Page_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
            AddHandler CType(Page.Master, LodMaster).TabClick, AddressOf TabButtonClicked
        End Sub

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
            If (Not IsPostBack) Then
                UserCanEdit = GetAccess(Navigator.PageAccess, True)
                InitiateControls()
                LoadData()

                LogManager.LogAction(ModuleType.LOD, UserAction.ViewPage, ReferenceId, "Viewed Page: LOD Senior Medical")
            End If
        End Sub

        Protected Sub SaveFinding(ByVal userControl As Secure_Shared_UserControls_FindingsControl, ByVal pType As PersonnelTypes, ByVal sectionName As SectionNames)
            If (Not HasReadWriteAccessForSection(sectionName)) Then
                Exit Sub
            End If

            Dim cFinding As LineOfDutyFindings = CreateFinding(LOD.Id)
            Dim BoardMedicalFinding As LineOfDutyFindings = LOD.FindByType(PersonnelTypes.BOARD_SG)

            cFinding.PType = pType

            If (Not BoardMedicalFinding Is Nothing AndAlso BoardMedicalFinding.Finding.HasValue) Then

                cFinding.DecisionYN = userControl.Decision

                If (userControl.Decision.Equals("N")) Then
                    cFinding.Finding = userControl.Findings
                End If
            Else
                cFinding.Finding = userControl.Findings
            End If

            cFinding.Explanation = userControl.FormFindingsText

            LOD.SetFindingByType(cFinding)
            LODDao.SaveOrUpdate(LOD)
            LODDao.CommitChanges()
        End Sub

        Private Sub TabButtonClicked(ByRef sender As Object, ByRef e As TabNavigationEventArgs)
            If (e.ButtonType = NavigatorButtonType.Save OrElse e.ButtonType = NavigatorButtonType.NavigatedAway _
              OrElse e.ButtonType = NavigatorButtonType.NextStep OrElse e.ButtonType = NavigatorButtonType.PreviousStep) Then
                SaveData()
            End If
        End Sub

    End Class

End Namespace