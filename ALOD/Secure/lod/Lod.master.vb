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

    Partial Class LodMaster
        Inherits System.Web.UI.MasterPage

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
            Dim lodDao As ILineOfDutyDao = factory.GetLineOfDutyDao()
            Dim lod As New LineOfDuty()

            lod = LoadCase(Request.QueryString("refId"), lodDao)

            Dim workStatuses As List(Of Integer) = New List(Of Integer)

            args.RefId = lod.Id
            args.ModuleId = lod.ModuleType
            If (lod.Formal) Then
                workStatuses.Add(LodWorkStatus_v2.FormalSeniorMedicalReview)
                args.WorkStatusIds = workStatuses
            Else
                workStatuses.Add(LodWorkStatus_v2.SeniorMedicalReview)
                args.WorkStatusIds = workStatuses
            End If
            args.TabTitle = "Senior Med"
            args.Steps = Navigator.Steps

            Return args
        End Function

        Public Function GetStartPageTitle(ByVal statusCode As LodStatusCode, ByVal isFormal As Boolean) As String
            Dim startPage As String = "Member"

            Select Case statusCode
                Case LodStatusCode.MedTechReview, LodStatusCode.MedicalOfficerReview, LodStatusCode.MedicalOfficer_LODV3, LodStatusCode.MedicalTech_LODV3
                    If (GroupId = UserGroups.MedicalTechnician OrElse GroupId = UserGroups.MedicalOfficer OrElse GroupId = UserGroups.LOD_MFP) Then
                        startPage = "Medical"
                    End If

                Case LodStatusCode.UnitCommanderReview
                    If (GroupId = UserGroups.UnitCommander) Then
                        startPage = "Unit CC"
                    End If

                Case LodStatusCode.WingJAReview
                    If (GroupId = UserGroups.WingJudgeAdvocate) Then
                        startPage = "Wing JA"
                    End If

                Case LodStatusCode.WingJA_LODV3
                    If (GroupId = UserGroups.WingJudgeAdvocate) Then
                        startPage = "Member"
                    End If

                Case LodStatusCode.AppointingAutorityReview, LodStatusCode.FormalActionByAppointingAuthority, LodStatusCode.AppointingAutority_LODV3
                    If (GroupId = UserGroups.WingCommander Or SESSION_GROUP_ID = 122) Then
                        startPage = "Wing CC"
                    End If

                Case LodStatusCode.NotifyFormalInvestigator
                    If (GroupId = UserGroups.MPF OrElse GroupId = UserGroups.LOD_PM) Then
                        startPage = "Wing CC"
                    End If

                Case LodStatusCode.FormalInvestigation
                    If (GroupId = UserGroups.InvestigatingOfficer) Then
                        startPage = "Investigation"
                    End If
                Case LodStatusCode.Complete_LODV3
                    startPage = "Next Action"
                Case LodStatusCode.UnitCC_LODV3
                    If (SESSION_GROUP_ID = 120) Then
                        startPage = "Unit CC"
                    End If
                Case LodStatusCode.LODAudit_A1, LodStatusCode.LODAudit_JA, LodStatusCode.LODAudit_SG
                    'If (SESSION_GROUP_ID = 11) Then
                    startPage = "Audit"
                    'End If
                Case LodStatusCode.AppointingAutority_LODV3
                    startPage = ""
            End Select

            If (Not Navigator(startPage).Visible) Then
                startPage = "Member"
            End If

            If (isFormal AndAlso IsUserGroupWithMinimalFormalLODView()) Then
                startPage = "Member"
            End If

            Return startPage
        End Function

        Public Sub InitPageAccess(ByVal lod As LineOfDuty)
            Navigator.ClearSession()
            Navigator.InitControl()

            ' Now set our page access for this session
            Dim access As IList(Of PageAccess)
            Dim accessDao As IPageAccessDao = New NHibernateDaoFactory().GetPageAccessDao()

            access = accessDao.GetByWorkflowGroupAndStatus(lod.Workflow, GroupId, lod.Status)
            Navigator.SetPageAccess(access)

            ' End of Findings stuff being ignored
            '      System.Diagnostics.Debugger.Launch()
            '' System.Console.WriteLine(lod.Status)

            ' Find the Wing JA tab once and set its visibility based on the status
            Dim wingJaTab As TabItem = Navigator("Wing JA")
            If wingJaTab IsNot Nothing Then
                wingJaTab.Visible = (lod.Status >= LodWorkStatus_v2.FormalActionByWingJA)
            End If

            If (Not lod.Formal) Then
                If (lod.CurrentStatusCode < LodStatusCode.FormalInvestigation) Then
                    Navigator("Investigation").Visible = False
                    Navigator("Investigation").Access = PageAccessType.None
                End If
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

            ' Find the Investigation tab once and set its visibility based on the status
            Dim investigationTab As TabItem = Navigator("Investigation")
            If investigationTab IsNot Nothing Then
                investigationTab.Visible = (lod.Status >= LodWorkStatus_v2.FormalInvestigation)
            End If

            InitSeniorMedicalReviewerTabVisibility(ConstructSeniorMedicalReviewerTabVisibilityArgs())

        End Sub

        Public Function LoadCase(ByVal refId As Integer, ByVal lodDao As ILineOfDutyDao) As LineOfDuty
            Dim lod As LineOfDuty = Nothing

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

        Protected Function IsUserGroupWithMinimalFormalLODView() As Boolean
            If (GroupId = UserGroups.WingSarc OrElse
                GroupId = UserGroups.RSL OrElse
                GroupId = UserGroups.SARCAdmin) Then
                Return True
            Else
                Return False
            End If
        End Function

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
            Dim lodDao As ILineOfDutyDao = factory.GetLineOfDutyDao()
            Dim lod As New LineOfDuty()

            Session("refId") = refId

            If (lodDao.GetWorkflow(refId) = 27) Then
                lod = New LineOfDuty_v2()
            End If

            userAccess = lodDao.GetUserAccess(SESSION_USER_ID, refId)

            Utility.VerifyUserAccess(userAccess, Resources.Messages.ERROR_NO_ACCESS, Resources._Global.StartPage)

            lod = LoadCase(refId, lodDao)

            Utility.UpdateCaseLock(userAccess, refId, ModuleType.LOD)
            InitPageAccess(lod)

            Navigator.Commit()
            lodDao.Evict(lod)
        End Sub

        Private Sub TabButtonClicked(ByRef sender As Object, ByRef e As TabNavigationEventArgs) Handles TabNavigator1.ButtonClicked
            RaiseEvent TabClick(Me, e)
        End Sub

    End Class

End Namespace