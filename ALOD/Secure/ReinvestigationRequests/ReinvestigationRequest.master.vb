Imports ALOD.Core.Domain.Common
Imports ALOD.Core.Domain.Lookup
Imports ALOD.Core.Domain.Modules.Lod
Imports ALOD.Core.Domain.Modules.Reinvestigations
Imports ALOD.Core.Domain.Users
Imports ALOD.Core.Domain.Workflow
Imports ALOD.Core.Interfaces.DAOInterfaces
Imports ALOD.Data
Imports ALOD.Web.UserControls
Imports ALODWebUtility
Imports ALODWebUtility.Common

Namespace Web.RR

    Partial Class ReinvestigationRequestMaster
        Inherits System.Web.UI.MasterPage

        Dim _dao As ILODReinvestigateDAO
        Private _daoFactory As IDaoFactory
        Private _lookupDao As ILookupDao

        Public Delegate Sub TabClickEventHandler(ByRef sender As Object, ByRef e As TabNavigationEventArgs)

        Public Event TabClick As TabClickEventHandler

        Public ReadOnly Property GroupId As Byte
            Get
                Return CByte(Session("GroupId"))
            End Get
        End Property

        Public ReadOnly Property ModHeader() As ModuleHeader
            Get
                Return ModuleHeader1
            End Get
        End Property

        Public ReadOnly Property Navigator() As TabNavigator
            Get
                Return TabNavigator1
            End Get
        End Property

        Public ReadOnly Property NestedHolder() As ContentPlaceHolder
            Get
                Return CType(Me.Master.FindControl("ContentMain").FindControl("ContentNested"), ContentPlaceHolder)
            End Get
        End Property

        Public ReadOnly Property TabControl() As TabControls
            Get
                Return TabControls1
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

        Protected ReadOnly Property LookupDao As LookupDao
            Get
                If (_lookupDao Is Nothing) Then
                    _lookupDao = DaoFactory.GetLookupDao()
                End If

                Return _lookupDao
            End Get
        End Property

        Public Function ConstructSeniorMedicalReviewerTabVisibilityArgs() As SeniorMedicalReviewerTabVisibilityArgs
            Dim args As SeniorMedicalReviewerTabVisibilityArgs
            Dim factory As New NHibernateDaoFactory()
            Dim sDao As ILODReinvestigateDAO = factory.GetLODReinvestigationDao()
            Dim rr As New LODReinvestigation()

            rr = LoadCase(Request.QueryString("requestId"), sDao)

            Dim workStatuses As List(Of Integer) = New List(Of Integer)
            workStatuses.Add(ReinvestigationRequestWorkStatus.SeniorMedicalReview)

            args.RefId = rr.Id
            args.ModuleId = rr.ModuleId
            args.WorkStatusIds = workStatuses
            args.TabTitle = "RR Senior Med"
            args.Steps = Navigator.Steps

            Return args
        End Function

        Public Function GetStartPageTitle(ByVal workStatus As ReinvestigationRequestWorkStatus) As String
            Dim startPage As String = "RR Member"

            Select Case workStatus
                Case ReinvestigationRequestWorkStatus.WingJARequestReview
                    If (GroupId = UserGroups.WingJudgeAdvocate) Then
                        startPage = "RR Wing JA"
                    End If

                Case ReinvestigationRequestWorkStatus.WingCCRequestReview
                    If (GroupId = UserGroups.WingCommander) Then
                        startPage = "RR Wing CC"
                    End If

                Case Else
                    startPage = "RR Member"
            End Select

            If (Not Navigator(startPage).Visible) Then
                startPage = "RR Member"
            End If

            Return startPage
        End Function

        Public Sub InitPageAccess(ByVal rr As LODReinvestigation)
            Dim accessDao As IPageAccessDao = New NHibernateDaoFactory().GetPageAccessDao()

            Navigator.ClearSession()
            Navigator.InitControl()
            Navigator.SetPageAccess(accessDao.GetByWorkflowGroupAndStatus(rr.Workflow, GroupId, rr.Status))

            InitSeniorMedicalReviewerTabVisibility(ConstructSeniorMedicalReviewerTabVisibilityArgs())
        End Sub

        Public Function LoadCase(ByVal requestId As Integer, ByVal rDao As ILODReinvestigateDAO) As LODReinvestigation
            Dim rrLod As New LODReinvestigation()

            Try
                rrLod = rDao.GetById(requestId, False)
            Catch ex As Exception
                'we failed to load the case, nothing we can do
                ALOD.Logging.LogManager.LogError("rDao Error loading ReinvestigationRequest with requestId: " + requestId.ToString())
                SetErrorMessage(Resources.Messages.ERROR_LOADING_CASE)
                Response.Redirect(Resources._Global.StartPage, True)
            End Try

            If (rrLod Is Nothing) Then
                'we failed to load the case, nothing we can do
                ALOD.Logging.LogManager.LogError("NULL ReinvestigationRequest Error loading ReinvestigationRequest with requestId: " + requestId.ToString())
                SetErrorMessage(Resources.Messages.ERROR_LOADING_CASE)
                Response.Redirect(Resources._Global.StartPage, True)
            End If

            Return rrLod
        End Function

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
            Dim requestId As Integer = 0
            Integer.TryParse(Request.QueryString("requestId"), requestId)

            If (requestId = 0) Then
                Response.Redirect(Resources._Global.StartPage, True)
            End If

            Dim sessionId As Integer = 0
            Integer.TryParse(Session("RequestId"), sessionId)

            If (sessionId = 0) Then
                Response.Redirect(Resources._Global.StartPage, True)
            End If

            If (requestId <> sessionId) Then
                InitNavigator(requestId)
            End If

            ModHeader.ModuleHeader = "RR"
        End Sub

        Private Sub InitNavigator(ByVal requestId As Integer)
            Dim factory As New NHibernateDaoFactory()
            Dim sDao As ILODReinvestigateDAO = factory.GetLODReinvestigationDao()
            Dim rr As New LODReinvestigation()
            Dim userAccess As PageAccess.AccessLevel = PageAccess.AccessLevel.None

            Session("requestId") = requestId

            userAccess = sDao.GetUserAccess(SESSION_USER_ID, requestId)

            Utility.VerifyUserAccess(userAccess, Resources.Messages.ERROR_NO_ACCESS, Resources._Global.StartPage)

            rr = LoadCase(requestId, sDao)

            Utility.UpdateCaseLock(userAccess, requestId, ModuleType.ReinvestigationRequest)
            InitPageAccess(rr)

            Navigator.Commit()
            sDao.Evict(rr)
        End Sub

        Private Sub TabButtonClicked(ByRef sender As Object, ByRef e As TabNavigationEventArgs) Handles TabNavigator1.ButtonClicked
            RaiseEvent TabClick(Me, e)
        End Sub

    End Class

End Namespace