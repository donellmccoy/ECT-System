Imports ALOD.Core.Domain.Common
Imports ALOD.Core.Domain.Modules.Lod
Imports ALOD.Core.Domain.Modules.SpecialCases
Imports ALOD.Core.Domain.Users
Imports ALOD.Core.Domain.Workflow
Imports ALOD.Core.Interfaces.DAOInterfaces
Imports ALOD.Data
Imports ALOD.Web.UserControls
Imports ALODWebUtility
Imports ALODWebUtility.Common

Namespace Web.Special_Case.AGR

    Partial Class SC_AGRCertMaster
        Inherits System.Web.UI.MasterPage

        Private _dao As ISpecialCaseDAO
        Private _sc As SC_AGRCert = Nothing
        Private _scId As Integer = 0

        Public Delegate Sub TabClickEventHandler(ByRef sender As Object, ByRef e As TabNavigationEventArgs)

        Public Event TabClick As TabClickEventHandler

        ReadOnly Property SCDao() As ISpecialCaseDAO
            Get
                If (_dao Is Nothing) Then
                    _dao = New NHibernateDaoFactory().GetSpecialCaseDAO()
                End If

                Return _dao
            End Get
        End Property

        Public ReadOnly Property GroupId As Byte
            Get
                Return CByte(Session("GroupId"))
            End Get
        End Property

        Public ReadOnly Property ModHeader() As ModuleHeader
            Get
                Return ModuleHeader_agr
            End Get
        End Property

        Public ReadOnly Property Navigator() As TabNavigator
            Get
                Return TabNavigator_agr
            End Get
        End Property

        Public ReadOnly Property NestedHolder() As ContentPlaceHolder
            Get
                Return CType(Me.Master.FindControl("ContentMain").FindControl("ContentNested"), ContentPlaceHolder)
            End Get
        End Property

        Public ReadOnly Property TabControl() As TabControls
            Get
                Return TabControls_agr
            End Get
        End Property

        Protected ReadOnly Property SpecCase() As SC_AGRCert
            Get
                If (_sc Is Nothing) Then
                    _sc = SCDao.GetById(Request.QueryString("RefId"))
                End If

                Return _sc
            End Get
        End Property

        Public Function ConstructSeniorMedicalReviewerTabVisibilityArgs() As SeniorMedicalReviewerTabVisibilityArgs
            Dim args As SeniorMedicalReviewerTabVisibilityArgs

            Dim workStatuses As List(Of Integer) = New List(Of Integer)
            workStatuses.Add(SpecCaseAGRWorkStatus.SeniorMedicalReview)

            args.RefId = SpecCase.Id
            args.ModuleId = SpecCase.moduleId
            args.WorkStatusIds = workStatuses
            args.TabTitle = "AGR Senior Med"
            args.Steps = Navigator.Steps

            Return args
        End Function

        Public Function GetStartPageTitle(ByVal workStatus As SpecCaseAGRWorkStatus) As String
            Dim startPage As String = "AGR Member"

            Select Case workStatus
                Case SpecCaseAGRWorkStatus.InitiateCase
                    If (GroupId = UserGroups.MedicalTechnician) Then
                        startPage = "AGR Med Tech"
                    End If

                Case SpecCaseAGRWorkStatus.PackageReview
                    If (GroupId = UserGroups.AFRCHQTechnician) Then
                        startPage = "AGR Documents"
                    End If

                Case SpecCaseAGRWorkStatus.MedicalReview
                    If (GroupId = UserGroups.BoardMedical) Then
                        startPage = "AGR Medical Review"
                    End If

                Case SpecCaseAGRWorkStatus.FinalReview
                    If (GroupId = UserGroups.AFRCHQTechnician) Then
                        startPage = "AGR Medical Review"
                    End If

                Case SpecCaseAGRWorkStatus.MedicalOfficerReview
                    If (GroupId = UserGroups.MedicalOfficer) Then
                        startPage = "AGR Medical Review"
                    End If
                Case Else
                    startPage = "AGR Member"
            End Select

            If (Not Navigator(startPage).Visible) Then
                startPage = "AGR Member"
            End If

            Return startPage
        End Function

        Public Sub InitPageAccess(ByVal specCase As SC_AGRCert)
            Dim accessDao As IPageAccessDao = New NHibernateDaoFactory().GetPageAccessDao()

            Navigator.ClearSession()
            Navigator.InitControl()
            Navigator.SetPageAccess(accessDao.GetByWorkflowGroupAndStatus(specCase.Workflow, GroupId, specCase.Status))

            Utility.InitSeniorMedicalReviewerTabVisibility(ConstructSeniorMedicalReviewerTabVisibilityArgs())
        End Sub

        Public Function LoadCase(ByVal refId As Integer) As SC_AGRCert
            Dim scPW As SC_AGRCert = Nothing

            Try
                If _scId <> refId Then
                    _sc = Nothing  'Ensure we grab based on refId
                    _scId = refId
                End If
                scPW = SpecCase
            Catch ex As Exception
                'we failed to load the case, nothing we can do
                ALOD.Logging.LogManager.LogError(ex.ToString() + " \ refId: " + refId.ToString())
                SetErrorMessage(Resources.Messages.ERROR_LOADING_CASE)
                Response.Redirect(Resources._Global.StartPage, True)
            End Try

            If (scPW Is Nothing) Then
                'we failed to load the case, nothing we can do
                ALOD.Logging.LogManager.LogError("NULL SpecCaseAGRCert Error loading Case of Type (" + "SpecCaseAGRCert" + ") with refId: " + refId.ToString())
                SetErrorMessage(Resources.Messages.ERROR_LOADING_CASE)
                Response.Redirect(Resources._Global.StartPage, True)
            End If

            Return scPW
        End Function

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
            Dim refId As Integer = 0
            Integer.TryParse(Request.QueryString("RefId"), refId)

            If (refId = 0) Then
                Response.Redirect(Resources._Global.StartPage, True)
            End If

            Dim sessionId As Integer = 0
            Integer.TryParse(Session("RefId"), sessionId)

            If (sessionId = 0) Then
                Response.Redirect(Resources._Global.StartPage, True)
            End If

            If (refId <> sessionId) Then
                InitNavigator(refId)
            End If

            ModHeader.ModuleHeader = "AGR"
        End Sub

        Private Sub InitNavigator(ByVal refId As Integer)
            Dim factory As New NHibernateDaoFactory()
            Dim userAccess As PageAccess.AccessLevel = PageAccess.AccessLevel.None
            Dim sc As New SC_AGRCert

            Session("refId") = refId

            userAccess = SCDao.GetUserAccess(SESSION_USER_ID, refId, ModuleType.SpecCaseAGR)

            Utility.VerifyUserAccess(userAccess, Resources.Messages.ERROR_NO_ACCESS, Resources._Global.StartPage)

            sc = LoadCase(refId)

            Utility.UpdateCaseLock(userAccess, refId, ModuleType.SpecCaseAGR)
            InitPageAccess(sc)

            Navigator.Commit()
            SCDao.Evict(sc)

        End Sub

        Private Sub TabButtonClicked(ByRef sender As Object, ByRef e As TabNavigationEventArgs) Handles TabNavigator_agr.ButtonClicked
            RaiseEvent TabClick(Me, e)
        End Sub

    End Class

End Namespace