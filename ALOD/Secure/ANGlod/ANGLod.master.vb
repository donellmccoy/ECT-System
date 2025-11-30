Imports ALOD.Core.Domain.Common
Imports ALOD.Core.Domain.Lookup
Imports ALOD.Core.Domain.Modules.Lod
Imports ALOD.Core.Domain.Users
Imports ALOD.Core.Domain.Workflow
Imports ALOD.Core.Interfaces.DAOInterfaces
Imports ALOD.Data
Imports ALOD.Web.UserControls
Imports ALODWebUtility
Imports ALODWebUtility.Common
Imports ALODWebUtility.TabNavigation

Namespace Web.LOD
    Partial Class ANGLodMaster
        Inherits System.Web.UI.MasterPage

        Private _daoFactory As IDaoFactory
        Private _lookupDao As ILookupDao
        Public Delegate Sub TabClickEventHandler(ByRef sender As Object, ByRef e As TabNavigationEventArgs)
        Public Event TabClick As TabClickEventHandler

        Public ReadOnly Property Navigator() As TabNavigator
            Get
                Return TabNavigator1
            End Get
        End Property

        Public ReadOnly Property ModHeader() As ModuleHeader
            Get
                Return ModuleHeader1
            End Get
        End Property

        Public ReadOnly Property TabControl() As TabControls
            Get
                Return TabControls1
            End Get
        End Property

        Public ReadOnly Property NestedHolder() As ContentPlaceHolder
            Get
                Return CType(Me.Master.FindControl("ContentMain").FindControl("ContentNested"), ContentPlaceHolder)
            End Get
        End Property

        Public ReadOnly Property GroupId As Byte
            Get
                Return CByte(Session("GroupId"))
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

        Private Sub TabButtonClicked(ByRef sender As Object, ByRef e As TabNavigationEventArgs) Handles TabNavigator1.ButtonClicked
            RaiseEvent TabClick(Me, e)
        End Sub

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

            Dim refId As Integer = 0
            Integer.TryParse(Request.QueryString("refId"), refId)

            If (refId = 0) Then
                Response.Redirect(Resources._Global.StartPage, True)
            End If

            Dim sessionId As Integer = 0
            Integer.TryParse(Session("RefId"), sessionId)

            If (sessionId = 0) Then
                Response.Redirect(Resources._Global.StartPage, True)
            End If

            ' The two refId values not being equal is usually indicative of navigating away from a case, and then using the web browser back button to navigate back to the case...
            If (refId <> sessionId) Then
                InitNavigator(refId)
            End If

            ModHeader.ModuleHeader = "LOD"
        End Sub

        Private Sub InitNavigator(ByVal refId As Integer)
            Dim factory As New NHibernateDaoFactory()
            Dim userAccess As PageAccess.AccessLevel = PageAccess.AccessLevel.None
            Dim lodDao As ANGILineOfDutyDao = factory.GetLineOfDutyDao()
            Dim lod As New ANGLineOfDuty()

            Session("refId") = refId

            If (lodDao.GetWorkflow(refId) = 127) Then
                lod = New ANGLineOfDuty_v2()
            End If

            userAccess = lodDao.GetUserAccess(SESSION_USER_ID, refId)

            Utility.VerifyUserAccess(userAccess, Resources.Messages.ERROR_NO_ACCESS, Resources._Global.StartPage)

            lod = LoadCase(refId, lodDao)

            Utility.UpdateCaseLock(userAccess, refId, ModuleType.ANGLOD)
            InitPageAccess(lod)

            Navigator.Commit()
            lodDao.Evict(lod)
        End Sub

        Public Function LoadCase(ByVal refId As Integer, ByVal lodDao As ANGILineOfDutyDao) As ANGLineOfDuty
            Dim lod As ANGLineOfDuty = Nothing

            Try
                lod = lodDao.GetById(refId, False)
            Catch ex As Exception
                ALOD.Logging.LogManager.LogError("DAO Error loading LOD with refId: " + refId.ToString())
                SetErrorMessage(Resources.Messages.ERROR_LOADING_CASE)
                Response.Redirect(Resources._Global.StartPage, True)
            End Try

            If (lod Is Nothing) Then
                ALOD.Logging.LogManager.LogError("NULL LOD Error loading LOD with refId: " + refId.ToString())
                SetErrorMessage(Resources.Messages.ERROR_LOADING_CASE)
                Response.Redirect(Resources._Global.StartPage, True)
            End If

            If (lod.Formal = True AndAlso Not UserHasPermission(PERMISSION_VIEW_FORMAL_LOD)) Then
                SetErrorMessage("You do not have access to view Formal LOD cases")
                Response.Redirect(Resources._Global.StartPage, True)
            End If

            Return lod
        End Function

        Public Sub InitPageAccess(ByVal lod As ANGLineOfDuty)
            Navigator.ClearSession()
            Navigator.InitControl()

            ' Now set our page access for this session
            Dim access As IList(Of PageAccess)
            Dim accessDao As IPageAccessDao = New NHibernateDaoFactory().GetPageAccessDao()

            access = accessDao.GetByWorkflowGroupAndStatus(lod.Workflow, GroupId, lod.Status)
            Navigator.SetPageAccess(access)


            ' End of Findings stuff being ignored

            If (Not lod.Formal) Then
                Navigator("Investigation").Visible = False
                Navigator("Investigation").Access = PageAccessType.None
            Else
                If (IsUserGroupWithMinimalFormalLODView()) Then
                    ' Hide all of the tabs except for the Member and Tracking tabs...
                    For Each item As TabItem In Navigator.Steps
                        If (Not item.Title.Equals("Member") AndAlso Not item.Title.Equals("Tracking")) Then
                            item.Visible = False
                        Else
                            item.Visible = True
                        End If
                    Next
                End If
            End If

            InitSeniorMedicalReviewerTabVisibility(ConstructSeniorMedicalReviewerTabVisibilityArgs())

        End Sub

        Public Function ConstructSeniorMedicalReviewerTabVisibilityArgs() As SeniorMedicalReviewerTabVisibilityArgs
            Dim args As SeniorMedicalReviewerTabVisibilityArgs
            Dim factory As New NHibernateDaoFactory()
            Dim lodDao As ANGILineOfDutyDao = factory.GetLineOfDutyDao()
            Dim lod As New ANGLineOfDuty()

            lod = LoadCase(Request.QueryString("refId"), lodDao)

            Dim workStatuses As List(Of Integer) = New List(Of Integer)

            args.RefId = lod.Id
            args.ModuleId = lod.ModuleType
            If (lod.Formal) Then
                workStatuses.Add(LodWorkStatus_v2.ANGFormalSeniorMedicalReview)
                args.WorkStatusIds = workStatuses
            Else
                workStatuses.Add(LodWorkStatus_v2.ANGSeniorMedicalReview)
                args.WorkStatusIds = workStatuses
            End If
            args.TabTitle = "Senior Med"
            args.Steps = Navigator.Steps

            Return args
        End Function

        Protected Function IsUserGroupWithMinimalFormalLODView() As Boolean
            If (GroupId = UserGroups.WingSarc OrElse
                GroupId = UserGroups.RSL OrElse
                GroupId = UserGroups.SARCAdmin) Then
                Return True
            Else
                Return False
            End If
        End Function

        Public Function GetStartPageTitle(ByVal statusCode As LodStatusCode, ByVal isFormal As Boolean) As String
            Dim startPage As String = "Member"

            Select Case statusCode
                Case LodStatusCode.ANGMedTechReview, LodStatusCode.ANGMedicalOfficerReview
                    If (GroupId = UserGroups.ANGMedicalTechnician OrElse GroupId = UserGroups.ANGMedicalOfficer OrElse GroupId = UserGroups.ANGLOD_MFP) Then
                        startPage = "Medical"
                    End If

                Case LodStatusCode.ANGUnitCommanderReview
                    If (GroupId = UserGroups.ANGUnitCommander) Then
                        startPage = "Unit CC"
                    End If

                Case LodStatusCode.ANGWingJAReview
                    If (GroupId = UserGroups.ANGWingJudgeAdvocate) Then
                        startPage = "Wing JA"
                    End If

                Case LodStatusCode.ANGAppointingAutorityReview, LodStatusCode.ANGFormalActionByAppointingAuthority
                    If (GroupId = UserGroups.ANGWingCommander) Then
                        startPage = "Wing CC"
                    End If

                Case LodStatusCode.ANGNotifyFormalInvestigator
                    If (GroupId = UserGroups.ANGMPF OrElse GroupId = UserGroups.ANGLOD_PM) Then
                        startPage = "Wing CC"
                    End If

                Case LodStatusCode.ANGFormalInvestigation
                    If (GroupId = UserGroups.ANGInvestigatingOfficer) Then
                        startPage = "Investigation"
                    End If
            End Select

            If (Not Navigator(startPage).Visible) Then
                startPage = "Member"
            End If

            If (isFormal AndAlso IsUserGroupWithMinimalFormalLODView()) Then
                startPage = "Member"
            End If

            Return startPage
        End Function
    End Class
End Namespace

