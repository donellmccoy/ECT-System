Imports ALOD.Core.Domain.Common
Imports ALOD.Core.Domain.Lookup
Imports ALOD.Core.Domain.Modules.Lod
Imports ALOD.Core.Domain.Modules.SpecialCases
Imports ALOD.Core.Domain.Users
Imports ALOD.Core.Domain.Workflow
Imports ALOD.Core.Interfaces.DAOInterfaces
Imports ALOD.Data
Imports ALOD.Web.UserControls
Imports ALODWebUtility
Imports ALODWebUtility.Common

Namespace Web.Special_Case.IN

    Partial Class SC_IncapMaster
        Inherits System.Web.UI.MasterPage

        Private _daoFactory As IDaoFactory
        Private _lookupDao As ILookupDao
        Private _sc As SC_Incap = Nothing
        Private _scId As Integer = 0
        Private _specCaseDao As ISpecialCaseDAO

        Public Delegate Sub TabClickEventHandler(ByRef sender As Object, ByRef e As TabNavigationEventArgs)

        Public Event TabClick As TabClickEventHandler

        ReadOnly Property SCDao() As ISpecialCaseDAO
            Get
                If (_specCaseDao Is Nothing) Then
                    _specCaseDao = New NHibernateDaoFactory().GetSpecialCaseDAO()
                End If

                Return _specCaseDao
            End Get
        End Property

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

        Protected ReadOnly Property LookupDao As ILookupDao
            Get
                If (_lookupDao Is Nothing) Then
                    _lookupDao = DaoFactory.GetLookupDao()
                End If

                Return _lookupDao
            End Get
        End Property

        Protected ReadOnly Property SpecCase() As SC_Incap
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
            'workStatuses.Add(SpecCaseIncapWorkStatus.SeniorMedicalReview)

            args.RefId = SpecCase.Id
            args.ModuleId = SpecCase.moduleId
            args.WorkStatusIds = workStatuses
            args.TabTitle = "IN Senior Med"
            args.Steps = Navigator.Steps

            Return args
        End Function

        Public Function GetStartPageTitle(ByVal workStatus As SpecCaseIncapWorkStatus) As String
            Dim startPage As String = "IN Member"
            Dim refId As Integer = 0
            Integer.TryParse(Request.QueryString("refId"), refId)
            Dim workflowType As String = GetWorkflowType(refId)

            Select Case workflowType
                Case "Initiate", "Ext Wing"
                    startPage = "IN Wing Int"
                Case "Ext HQ", "Appeal"
                    If (GroupId = UserGroups.WingCommander OrElse GroupId = UserGroups.INCAP_PM) Then
                        startPage = "IN Wing Int"
                    Else
                        startPage = "IN HQ Ext/ App"
                    End If
                Case Else
                    startPage = "IN Member"
            End Select

            If (Not Navigator(startPage).Visible) Then
                startPage = "IN Member"
            End If

            Return startPage
        End Function

        Public Sub InitPageAccess(ByVal specCase As SC_Incap)
            Dim accessDao As IPageAccessDao = New NHibernateDaoFactory().GetPageAccessDao()

            Navigator.ClearSession()
            Navigator.InitControl()
            Navigator.SetPageAccess(accessDao.GetByWorkflowGroupAndStatus(specCase.Workflow, GroupId, specCase.Status))

            Utility.InitSeniorMedicalReviewerTabVisibility(ConstructSeniorMedicalReviewerTabVisibilityArgs())
        End Sub

        Public Function LoadCase(ByVal refId As Integer) As SC_Incap
            Dim scIN As SC_Incap = Nothing

            Try
                If _scId <> refId Then
                    _sc = Nothing  'Ensure we grab based on refId
                    _scId = refId
                End If
                scIN = SpecCase
            Catch ex As Exception
                'we failed to load the case, nothing we can do
                ALOD.Logging.LogManager.LogError(ex.ToString() + " \ refId: " + refId.ToString())
                SetErrorMessage(Resources.Messages.ERROR_LOADING_CASE)
                Response.Redirect(Resources._Global.StartPage, True)
            End Try

            If (scIN Is Nothing) Then
                'we failed to load the case, nothing we can do
                ALOD.Logging.LogManager.LogError("NULL SpecCaseIncap Error loading Case of Type (" + "SpecCaseIncap" + ") with refId: " + refId.ToString())
                SetErrorMessage(Resources.Messages.ERROR_LOADING_CASE)
                Response.Redirect(Resources._Global.StartPage, True)
            End If

            Return scIN
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

            ModHeader.ModuleHeader = "IN"
        End Sub

        Private Function GetWorkflowType(ByVal wsid As Integer) As String
            Dim type As String = ""
            Select Case wsid
                Case AFRCWorkflows.INInitiate, AFRCWorkflows.INMedicalReview_WG, AFRCWorkflows.INImmediateCommanderReview,
                     AFRCWorkflows.INWingJAReview, AFRCWorkflows.INFinanceReview, AFRCWorkflows.INWingCCAction, AFRCWorkflows.INApproved
                    type = "Initiate"
                Case AFRCWorkflows.INExtension, AFRCWorkflows.INMedicalReview_WG_Ext, AFRCWorkflows.INImmediateCommanderReview_Ext,
                     AFRCWorkflows.INWingJAReview_Ext, AFRCWorkflows.INFinanceReview_Ext, AFRCWorkflows.INWingCommanderRecommendation_Ext
                    type = "Ext Wing"
                Case AFRCWorkflows.INOCR_Ext_HR_Review, AFRCWorkflows.INOPR_Ext_HR_Review, AFRCWorkflows.INDirectorOfStaffReview,
                     AFRCWorkflows.INDirectorOfPersonnelReview, AFRCWorkflows.INCommandChiefReview, AFRCWorkflows.INViceCommanderReview,
                     AFRCWorkflows.INCAFRAction
                    type = "Ext HQ"
                Case AFRCWorkflows.INWingCCRecommendation_Appeal, AFRCWorkflows.INAppeal, AFRCWorkflows.INOPR_Appeal_HR_Review,
                     AFRCWorkflows.INOCR_Appeal_HR_Review, AFRCWorkflows.INViceCommanderReview_Appeal, AFRCWorkflows.INDirectorOfPersonnelReview_Appeal,
                     AFRCWorkflows.INDirectorOfStaffReview_Appeal, AFRCWorkflows.INCommandChiefReview_Appeal, AFRCWorkflows.INViceCommanderReview_Appeal,
                     AFRCWorkflows.INCAFR_Action_Appeal
                    type = "Appeal"

            End Select
            Return type
        End Function

        Private Sub InitNavigator(ByVal refId As Integer)
            Dim factory As New NHibernateDaoFactory()
            Dim userAccess As PageAccess.AccessLevel = PageAccess.AccessLevel.None
            Dim sc As New SC_Incap

            Session("refId") = refId

            userAccess = SCDao.GetUserAccess(SESSION_USER_ID, refId, ModuleType.SpecCaseIncap)

            Utility.VerifyUserAccess(userAccess, Resources.Messages.ERROR_NO_ACCESS, Resources._Global.StartPage)

            sc = LoadCase(refId)

            Utility.UpdateCaseLock(userAccess, refId, ModuleType.SpecCaseIncap)
            InitPageAccess(sc)

            Navigator.Commit()
            SCDao.Evict(sc)
        End Sub

        Private Sub TabButtonClicked(ByRef sender As Object, ByRef e As TabNavigationEventArgs) Handles TabNavigator1.ButtonClicked
            RaiseEvent TabClick(Me, e)
        End Sub

    End Class

End Namespace