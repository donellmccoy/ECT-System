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

Namespace Web.SARC

    Public Class SeniorMedReviewer
        Inherits System.Web.UI.Page

        Private _daoFactory As IDaoFactory
        Private _saDao As ISARCDAO
        Private _sarc As RestrictedSARC

        Private _scAccess As Dictionary(Of String, ALOD.Core.Domain.Workflow.PageAccessType)

#Region "Properties"

        ReadOnly Property SADao() As ISARCDAO
            Get
                If (_saDao Is Nothing) Then
                    _saDao = DaoFactory.GetSARCDao()
                End If

                Return _saDao
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

        Protected ReadOnly Property ExcludedFindings As String
            Get
                Return "REQUEST_CONSULT"
            End Get
        End Property

        Protected ReadOnly Property SARC() As RestrictedSARC
            Get
                If (_sarc Is Nothing) Then
                    _sarc = SADao.GetById(ReferenceId, False)

                End If
                Return _sarc
            End Get
        End Property

        Protected ReadOnly Property SectionList() As Dictionary(Of String, ALOD.Core.Domain.Workflow.PageAccessType)
            Get
                If (_scAccess Is Nothing) Then
                    _scAccess = SARC.ReadSectionList(CInt(Session("GroupId")))
                End If
                Return _scAccess
            End Get
        End Property

#End Region

        Public Sub InitiateControls()
            ucSeniorMedicalFindings.SetAccess(SectionList(SARCRestrictedSectionNames.SARC_SENIOR_MED_REV.ToString()))
        End Sub

        Public Sub LoadData()
            LoadFindingsControl(ucSeniorMedicalFindings, PersonnelTypes.SENIOR_MEDICAL_REVIEWER, UserGroups.SeniorMedicalReviewer)
            LoadFindingsSignature(ucSigCheckSeniorMedical, PersonnelTypes.SENIOR_MEDICAL_REVIEWER)
        End Sub

        Public Sub SaveData()
            If (Not UserCanEdit) Then
                Exit Sub
            End If

            SaveFinding(ucSeniorMedicalFindings, PersonnelTypes.SENIOR_MEDICAL_REVIEWER, SARCRestrictedSectionNames.SARC_SENIOR_MED_REV)
        End Sub

        Protected Function HasReadWriteAccessForSection(ByVal sectionName As SARCRestrictedSectionNames) As Boolean
            If (SectionList(sectionName.ToString()) <> PageAccessType.ReadWrite) Then
                Return False
            End If

            Return True
        End Function

        Protected Sub LoadFindingsControl(ByVal userControl As Secure_Shared_UserControls_FindingsControl, ByVal pType As PersonnelTypes, ByVal groupId As Integer)
            Dim cFinding As RestrictedSARCFindings = SARC.FindByType(pType)
            Dim BoardMedicalFinding As RestrictedSARCFindings = SARC.FindByType(PersonnelTypes.BOARD_SG)

            userControl.LoadFindingOptionsByWorkflow(SARC.Workflow, groupId, ExcludedFindings, False)

            If (cFinding Is Nothing) Then
                Exit Sub
            End If

            If (Not BoardMedicalFinding Is Nothing) Then
                If (BoardMedicalFinding.Finding.HasValue) Then
                    If cFinding.Concur <> String.Empty Then
                        userControl.Decision = cFinding.Concur
                    End If
                Else
                    userControl.FindingsOnly = True
                End If
            Else
                userControl.FindingsOnly = True
            End If

            If (cFinding.Finding.HasValue) Then
                userControl.Findings = cFinding.Finding.Value
            End If

            userControl.Remarks = cFinding.Remarks
        End Sub

        Protected Sub LoadFindingsSignature(ByVal sigCheck As Secure_Shared_UserControls_SigntureCheck, ByVal pType As PersonnelTypes)
            Dim cFinding As RestrictedSARCFindings

            cFinding = SARC.FindByType(pType)

            sigCheck.Visible = False

            If (Not SARC.HasAFinding(cFinding)) Then
                Exit Sub
            End If

            sigCheck.Visible = True
            sigCheck.VerifySignature(ReferenceId, pType)
        End Sub

        Protected Sub Page_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
            AddHandler CType(Page.Master, SARCMaster).TabClick, AddressOf TabButtonClicked
        End Sub

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
            If (Not IsPostBack) Then
                UserCanEdit = GetAccess(Navigator.PageAccess, True)
                InitiateControls()
                LoadData()

                LogManager.LogAction(ModuleType.SARC, UserAction.ViewPage, ReferenceId, "Viewed Page: SA Senior Medical")
            End If
        End Sub

        Protected Sub SaveFinding(ByVal userControl As Secure_Shared_UserControls_FindingsControl, ByVal pType As PersonnelTypes, ByVal sectionName As SARCRestrictedSectionNames)
            If (Not HasReadWriteAccessForSection(sectionName)) Then
                Exit Sub
            End If

            Dim cFinding As RestrictedSARCFindings = SARCUtility.CreateSARCFinding(SARC.Id)
            Dim BoardMedicalFinding As RestrictedSARCFindings = SARC.FindByType(PersonnelTypes.BOARD_SG)

            cFinding.PType = pType

            If (Not BoardMedicalFinding Is Nothing AndAlso BoardMedicalFinding.Finding.HasValue) Then

                cFinding.Concur = userControl.Decision

                If (userControl.Decision.Equals("N")) Then
                    cFinding.Finding = userControl.Findings
                End If
            Else
                cFinding.Finding = userControl.Findings
            End If

            If (userControl.ShowFormText) Then
                cFinding.FindingsText = userControl.FormFindingsText
            End If

            If (userControl.ShowRemarks) Then
                cFinding.Remarks = userControl.Remarks
            End If

            SARC.SetFindingByType(cFinding)
            SADao.SaveOrUpdate(SARC)
            SADao.CommitChanges()
        End Sub

        Private Sub TabButtonClicked(ByRef sender As Object, ByRef e As TabNavigationEventArgs)
            If (e.ButtonType = NavigatorButtonType.Save OrElse e.ButtonType = NavigatorButtonType.NavigatedAway _
              OrElse e.ButtonType = NavigatorButtonType.NextStep OrElse e.ButtonType = NavigatorButtonType.PreviousStep) Then
                SaveData()
            End If
        End Sub

    End Class

End Namespace