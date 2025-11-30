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

Namespace Web.Special_Case.RW

    Partial Class SC_RWMaster
        Inherits System.Web.UI.MasterPage

        Private _dao As ISpecialCaseDAO
        Private _sc As SC_RW = Nothing
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

        Protected ReadOnly Property SpecCase() As SC_RW
            Get
                If (_sc Is Nothing) Then
                    _sc = SCDao.GetById(Request.QueryString("refId"))
                End If

                Return _sc
            End Get
        End Property

        Public Function ConstructSeniorMedicalReviewerTabVisibilityArgs() As SeniorMedicalReviewerTabVisibilityArgs
            Dim args As SeniorMedicalReviewerTabVisibilityArgs

            Dim workStatuses As List(Of Integer) = New List(Of Integer)
            workStatuses.Add(SpecCaseRWWorkStatus.SeniorMedicalReview)

            args.RefId = SpecCase.Id
            args.ModuleId = SpecCase.moduleId
            args.WorkStatusIds = workStatuses
            args.TabTitle = "RW Senior Med"
            args.Steps = Navigator.Steps

            Return args
        End Function

        Public Function GetStartPageTitle(ByVal workStatus As SpecCaseRWWorkStatus) As String
            Dim startPage As String = "RW Member"

            Select Case workStatus
                Case SpecCaseRWWorkStatus.MedTechInitiateCase
                    If (GroupId = UserGroups.MedicalTechnician) Then
                        startPage = "RW Med Tech"
                    End If

                Case SpecCaseRWWorkStatus.HQAFRCTechInitiateCase
                    If (GroupId = UserGroups.AFRCHQTechnician) Then
                        startPage = "RW Med Tech"
                    End If

                Case SpecCaseRWWorkStatus.MedicalReview
                    If (GroupId = UserGroups.BoardMedical) Then
                        startPage = "RW Med Off"
                    End If
                Case Else
                    startPage = "RW Member"
            End Select

            If (Not Navigator(startPage).Visible) Then
                startPage = "RW Member"
            End If

            Return startPage
        End Function

        Public Sub InitPageAccess(ByVal specCase As SC_RW)
            Dim accessDao As IPageAccessDao = New NHibernateDaoFactory().GetPageAccessDao()

            Navigator.ClearSession()
            Navigator.InitControl()
            Navigator.SetPageAccess(accessDao.GetByWorkflowGroupAndStatus(specCase.Workflow, GroupId, specCase.Status))

            Utility.InitSeniorMedicalReviewerTabVisibility(ConstructSeniorMedicalReviewerTabVisibilityArgs())
        End Sub

        Public Function LoadCase(ByVal refId As Integer) As SC_RW
            Dim scRW As SC_RW = Nothing

            Try
                If _scId <> refId Then
                    _sc = Nothing  'Ensure we grab based on refId
                    _scId = refId
                End If
                scRW = SpecCase
            Catch ex As Exception
                'we failed to load the case, nothing we can do
                ALOD.Logging.LogManager.LogError(ex.ToString() + " \ refId: " + refId.ToString())
                SetErrorMessage(Resources.Messages.ERROR_LOADING_CASE)
                Response.Redirect(Resources._Global.StartPage, True)
            End Try

            If (scRW Is Nothing) Then
                'we failed to load the case, nothing we can do
                ALOD.Logging.LogManager.LogError("NULL Retention Waiver Renewal Error loading Retention Waiver Renewal with requestId: " + refId.ToString())
                SetErrorMessage(Resources.Messages.ERROR_LOADING_CASE)
                Response.Redirect(Resources._Global.StartPage, True)
            End If

            Return scRW
        End Function

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
            Dim refId As Integer = 0
            Integer.TryParse(Request.QueryString("refId"), refId)

            If (refId = 0) Then
                Response.Redirect(Resources._Global.StartPage, True)
            End If

            Dim sessionId As Integer = 0
            Integer.TryParse(Session("refId"), sessionId)

            If (sessionId = 0) Then
                Response.Redirect(Resources._Global.StartPage, True)
            End If

            ' The two refId values not being equal is usually indicative of navigating away from a case, and then using the web browser back button to navigate back to the case...
            If (refId <> sessionId) Then
                InitNavigator(refId)
            End If

            ModHeader.ModuleHeader = "RW"
        End Sub

        Private Sub InitNavigator(ByVal refId As Integer)
            Dim factory As New NHibernateDaoFactory()
            Dim userAccess As PageAccess.AccessLevel = PageAccess.AccessLevel.None
            Dim scRW As New SC_RW

            Session("refId") = refId

            userAccess = SCDao.GetUserAccess(SESSION_USER_ID, refId, ModuleType.SpecCaseRW)

            Utility.VerifyUserAccess(userAccess, Resources.Messages.ERROR_NO_ACCESS, Resources._Global.StartPage)

            scRW = LoadCase(refId)

            Utility.UpdateCaseLock(userAccess, refId, ModuleType.SpecCaseRW)
            InitPageAccess(scRW)

            Navigator.Commit()
            SCDao.Evict(scRW)
        End Sub

        Private Sub TabButtonClicked(ByRef sender As Object, ByRef e As TabNavigationEventArgs) Handles TabNavigator1.ButtonClicked

            RaiseEvent TabClick(Me, e)

        End Sub

    End Class

End Namespace