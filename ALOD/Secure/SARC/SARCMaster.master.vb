Imports ALOD.Core.Domain.Common
Imports ALOD.Core.Domain.Lookup
Imports ALOD.Core.Domain.Modules.Lod
Imports ALOD.Core.Domain.Modules.SARC
Imports ALOD.Core.Domain.Users
Imports ALOD.Core.Domain.Workflow
Imports ALOD.Core.Interfaces.DAOInterfaces
Imports ALOD.Data
Imports ALOD.Logging
Imports ALOD.Web.UserControls
Imports ALODWebUtility
Imports ALODWebUtility.Common

Namespace Web.SARC

    Partial Class SARCMaster
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
            Dim sDao As ISARCDAO = factory.GetSARCDao()
            Dim sarc As New RestrictedSARC()

            sarc = LoadCase(Request.QueryString("refId"), sDao)

            Dim workStatuses As List(Of Integer) = New List(Of Integer)
            workStatuses.Add(SARCRestrictedWorkStatus.SARCSeniorMedicalReview)

            args.RefId = sarc.Id
            args.ModuleId = sarc.ModuleId
            args.WorkStatusIds = workStatuses
            args.TabTitle = "SARC Senior Med"
            args.Steps = Navigator.Steps

            Return args
        End Function

        Public Function GetStartPageTitle(ByVal workStatus As SARCRestrictedWorkStatus) As String
            Dim startPage As String = "SARC Member"

            Select Case workStatus
                Case SARCRestrictedWorkStatus.SARCInitiate
                    If (GroupId = UserGroups.WingSarc OrElse GroupId = UserGroups.RSL) Then
                        startPage = "Wing SARC"
                    End If

                    If (GroupId = UserGroups.SARCAdmin) Then
                        startPage = "SARC Admin"
                    End If
                Case Else
                    startPage = "SARC Member"
            End Select

            If (Not Navigator(startPage).Visible) Then
                startPage = "SARC Member"
            End If

            Return startPage
        End Function

        Public Sub InitPageAccess(ByVal sarc As RestrictedSARC)
            Dim accessDao As IPageAccessDao = New NHibernateDaoFactory().GetPageAccessDao()

            Navigator.ClearSession()
            Navigator.InitControl()
            Navigator.SetPageAccess(accessDao.GetByWorkflowGroupAndStatus(sarc.Workflow, GroupId, sarc.Status))

            InitSeniorMedicalReviewerTabVisibility(ConstructSeniorMedicalReviewerTabVisibilityArgs())
        End Sub

        Public Function LoadCase(ByVal refId As Integer, ByVal sDao As ISARCDAO) As RestrictedSARC
            Dim sarc As New RestrictedSARC()

            Try
                sarc = sDao.GetById(refId, False)
            Catch ex As Exception
                LogManager.LogError("sDao Error loading SARC with sarcId: " + refId.ToString())
                SetErrorMessage(Resources.Messages.ERROR_LOADING_CASE)
                Response.Redirect(Resources._Global.StartPage, True)
            End Try

            If (sarc Is Nothing) Then
                LogManager.LogError("NULL SARC Error loading SARC with sarcId: " + refId.ToString())
                SetErrorMessage(Resources.Messages.ERROR_LOADING_CASE)
                Response.Redirect(Resources._Global.StartPage, True)
            End If

            Return sarc
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

            ModHeader.ModuleHeader = "SARC"
        End Sub

        Private Sub InitNavigator(ByVal refId As Integer)
            Dim factory As New NHibernateDaoFactory()
            Dim sDao As ISARCDAO = factory.GetSARCDao()
            Dim sarc As New RestrictedSARC()
            Dim groupId As Byte = CByte(Session("GroupId"))
            Dim userAccess As PageAccess.AccessLevel = PageAccess.AccessLevel.None

            Session("refId") = refId

            userAccess = sDao.GetUserAccess(SESSION_USER_ID, refId)

            Utility.VerifyUserAccess(userAccess, Resources.Messages.ERROR_NO_ACCESS, Resources._Global.StartPage)

            sarc = LoadCase(refId, sDao)

            Utility.UpdateCaseLock(userAccess, refId, ModuleType.SARC)
            InitPageAccess(sarc)

            Navigator.Commit()
            sDao.Evict(sarc)
        End Sub

        Private Sub TabButtonClicked(ByRef sender As Object, ByRef e As TabNavigationEventArgs) Handles TabNavigator1.ButtonClicked
            RaiseEvent TabClick(Me, e)
        End Sub

    End Class

End Namespace