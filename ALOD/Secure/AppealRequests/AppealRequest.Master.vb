Imports ALOD.Core.Domain.Common
Imports ALOD.Core.Domain.Lookup
Imports ALOD.Core.Domain.Modules.Appeals
Imports ALOD.Core.Domain.Modules.Lod
Imports ALOD.Core.Domain.Workflow
Imports ALOD.Core.Interfaces.DAOInterfaces
Imports ALOD.Data
Imports ALOD.Web.UserControls
Imports ALODWebUtility
Imports ALODWebUtility.Common

Namespace Web.AP

    Partial Class AppealRequestMaster
        Inherits System.Web.UI.MasterPage

        Dim _dao As ILODAppealDAO
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
            Dim sDao As ILODAppealDAO = factory.GetLODAppealDao()
            Dim appeal As New LODAppeal()
            appeal = LoadCase(Session("requestId"), sDao)

            Dim workStatuses As List(Of Integer) = New List(Of Integer)
            workStatuses.Add(SpecCaseMHWorkStatus.SeniorMedicalReview)

            args.RefId = appeal.Id
            args.ModuleId = appeal.ModuleId
            args.WorkStatusIds = workStatuses
            args.TabTitle = "MH Senior Med"
            args.Steps = Navigator.Steps

            Return args
        End Function

        Public Function GetStartPageTitle() As String
            Dim startPage As String = "AP Member"

            If (Not Navigator(startPage).Visible) Then
                startPage = "AP Member"
            End If

            Return startPage
        End Function

        Public Sub InitPageAccess(ByVal ap As LODAppeal)
            Dim accessDao As IPageAccessDao = New NHibernateDaoFactory().GetPageAccessDao()

            Navigator.ClearSession()
            Navigator.InitControl()
            Navigator.SetPageAccess(accessDao.GetByWorkflowGroupAndStatus(ap.Workflow, GroupId, ap.Status))

            InitSeniorMedicalReviewerTabVisibility(ConstructSeniorMedicalReviewerTabVisibilityArgs())
        End Sub

        Public Function LoadCase(ByVal requestId As Integer, ByVal aDao As ILODAppealDAO) As LODAppeal
            Dim apLod As New LODAppeal()

            Try
                apLod = aDao.GetById(requestId, False)
            Catch ex As Exception
                'we failed to load the case, nothing we can do
                ALOD.Logging.LogManager.LogError("aDao Error loading AppealRequest with appealId: " + requestId.ToString())
                SetErrorMessage(Resources.Messages.ERROR_LOADING_CASE)
                Response.Redirect(Resources._Global.StartPage, True)
            End Try

            If (apLod Is Nothing) Then
                'we failed to load the case, nothing we can do
                ALOD.Logging.LogManager.LogError("NULL AppealRequest Error loading AppealRequest with appealId: " + requestId.ToString())
                SetErrorMessage(Resources.Messages.ERROR_LOADING_CASE)
                Response.Redirect(Resources._Global.StartPage, True)
            End If

            Return apLod
        End Function

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
            Dim appealId As Integer = 0
            Integer.TryParse(Request.QueryString("requestId"), appealId)

            If (appealId = 0) Then
                Response.Redirect(Resources._Global.StartPage, True)
            End If

            Dim sessionId As Integer = 0
            Integer.TryParse(Session("requestId"), sessionId)

            If (sessionId = 0) Then
                Response.Redirect(Resources._Global.StartPage, True)
            End If

            If (appealId <> sessionId) Then
                InitNavigator(appealId)
            End If

            ModHeader.ModuleHeader = "AP"
        End Sub

        Private Sub InitNavigator(ByVal requestId As Integer)
            Dim factory As New NHibernateDaoFactory()
            Dim sDao As ILODAppealDAO = factory.GetLODAppealDao()
            Dim appeal As New LODAppeal()
            Dim userAccess As PageAccess.AccessLevel = PageAccess.AccessLevel.None

            Session("requestId") = requestId

            userAccess = sDao.GetUserAccess(SESSION_USER_ID, requestId)

            Utility.VerifyUserAccess(userAccess, Resources.Messages.ERROR_NO_ACCESS, Resources._Global.StartPage)

            appeal = LoadCase(requestId, sDao)

            Utility.UpdateCaseLock(userAccess, requestId, ModuleType.AppealRequest)
            InitPageAccess(appeal)

            Navigator.Commit()
            sDao.Evict(appeal)
        End Sub

        Private Sub TabButtonClicked(ByRef sender As Object, ByRef e As TabNavigationEventArgs) Handles TabNavigator1.ButtonClicked
            RaiseEvent TabClick(Me, e)
        End Sub

    End Class

End Namespace