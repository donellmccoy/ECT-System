Imports ALOD.Core.Domain.Common
Imports ALOD.Core.Domain.Lookup
Imports ALOD.Core.Domain.Modules.Lod
Imports ALOD.Core.Domain.Modules.SARC
Imports ALOD.Core.Domain.Workflow
Imports ALOD.Core.Interfaces.DAOInterfaces
Imports ALOD.Data
Imports ALOD.Logging
Imports ALOD.Web.UserControls
Imports ALODWebUtility
Imports ALODWebUtility.Common
Imports ALODWebUtility.TabNavigation

Namespace Web.APSA

    Partial Class SARCAppealMaster
        Inherits System.Web.UI.MasterPage

        Dim _dao As ISARCAppealDAO
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
            Dim sarcAppealDao As ISARCAppealDAO = factory.GetSARCAppealDao()
            Dim sarcAppeal As New SARCAppeal()

            sarcAppeal = LoadCase(Request.QueryString("requestId"), sarcAppealDao)

            Dim workStatuses As List(Of Integer) = New List(Of Integer)
            workStatuses.Add(SARCAppealWorkStatus.SeniorMedicalReview)

            args.RefId = sarcAppeal.Id
            args.ModuleId = sarcAppeal.ModuleId
            args.WorkStatusIds = workStatuses
            args.TabTitle = "SARC AP Senior Med"
            args.Steps = Navigator.Steps

            Return args
        End Function

        Public Function GetStartPageTitle(ByVal workStatus As SARCAppealWorkStatus) As String
            Dim startPage As String = "SARC AP Member"

            If (Not Navigator(startPage).Visible) Then
                startPage = "SARC AP Member"
            End If

            Return startPage
        End Function

        Public Sub InitPageAccess(ByVal sarc As SARCAppeal)
            Dim accessDao As IPageAccessDao = New NHibernateDaoFactory().GetPageAccessDao()

            Navigator.ClearSession()
            Navigator.InitControl()
            Navigator.SetPageAccess(accessDao.GetByWorkflowGroupAndStatus(sarc.Workflow, GroupId, sarc.Status))

            Dim trackingData = LookupDao.GetStatusTracking(sarc.Id, sarc.ModuleId)

            If (Not trackingData Is Nothing AndAlso trackingData.Count > 1) Then

                For Each wst As WorkStatusTracking In trackingData

                    If (wst.WorkflowStatus.Id = SARCAppealWorkStatus.SeniorMedicalReview) Then
                        Exit Sub
                    End If
                Next
            End If

            For Each item As TabItem In Navigator.Steps
                If (item.Title.Equals("SARC AP Senior Med")) Then
                    item.Visible = False
                End If
            Next
        End Sub

        Public Function LoadCase(ByVal refId As Integer, ByVal sDao As ISARCAppealDAO) As SARCAppeal
            Dim sarcAppeal As New SARCAppeal()

            Try
                sarcAppeal = sDao.GetById(refId, False)
            Catch ex As Exception
                LogManager.LogError("sDao Error loading SARC Appeal Request with sarcId: " + refId.ToString())
                SetErrorMessage(Resources.Messages.ERROR_LOADING_CASE)
                Response.Redirect(Resources._Global.StartPage, True)
            End Try

            If (sarcAppeal Is Nothing) Then
                LogManager.LogError("NULL SARC Appeal Request Error loading SARC Appeal Request with sarcId: " + refId.ToString())
                SetErrorMessage(Resources.Messages.ERROR_LOADING_CASE)
                Response.Redirect(Resources._Global.StartPage, True)
            End If

            Return sarcAppeal
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

            ' The two refId values not being equal is usually indicative of navigating away from a case, and then using the web browser back button to navigate back to the case...
            If (appealId <> sessionId) Then
                InitNavigator(appealId)
            End If

            ModHeader.ModuleHeader = "APSA"
        End Sub

        Private Sub InitNavigator(ByVal refId As Integer)
            Dim factory As New NHibernateDaoFactory()
            Dim userAccess As PageAccess.AccessLevel = PageAccess.AccessLevel.None
            Dim sarcAppealDao As ISARCAppealDAO = factory.GetSARCAppealDao()
            Dim sarcAppeal As New SARCAppeal()

            Session("requestId") = refId

            userAccess = sarcAppealDao.GetUserAccess(SESSION_USER_ID, refId)

            Utility.VerifyUserAccess(userAccess, Resources.Messages.ERROR_NO_ACCESS, Resources._Global.StartPage)

            sarcAppeal = LoadCase(refId, sarcAppealDao)

            Utility.UpdateCaseLock(userAccess, refId, ModuleType.SARCAppeal)
            InitPageAccess(sarcAppeal)

            Navigator.Commit()
            sarcAppealDao.Evict(sarcAppeal)
        End Sub

        Private Sub TabButtonClicked(ByRef sender As Object, ByRef e As TabNavigationEventArgs) Handles TabNavigator1.ButtonClicked
            RaiseEvent TabClick(Me, e)
        End Sub

    End Class

End Namespace