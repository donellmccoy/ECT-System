Imports ALOD.Core.Domain.Common
Imports ALOD.Core.Domain.Modules.Lod
Imports ALOD.Core.Domain.Modules.SpecialCases
Imports ALOD.Core.Domain.Workflow
Imports ALOD.Core.Interfaces.DAOInterfaces
Imports ALOD.Data
Imports ALOD.Web.UserControls
Imports ALODWebUtility.Common

Namespace Web.Special_Case.PSCD

    Partial Class SC_PSCDMaster
        Inherits System.Web.UI.MasterPage
        Private _associatedCaseDao As IAssociatedCaseDao
        Private _dao As ISpecialCaseDAO
        Private _factory As IDaoFactory
        Private _sc As SC_PSCD = Nothing
        Private _scId As Integer = 0
        Private _special As SpecialCase

        Public Delegate Sub TabClickEventHandler(ByRef sender As Object, ByRef e As TabNavigationEventArgs)

        Public Event TabClick As TabClickEventHandler

        ReadOnly Property DaoFactory() As IDaoFactory
            Get
                If (_factory Is Nothing) Then
                    _factory = New NHibernateDaoFactory()
                End If

                Return _factory
            End Get
        End Property

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

        Protected ReadOnly Property Associated() As IAssociatedCaseDao
            Get
                If (_associatedCaseDao Is Nothing) Then
                    _associatedCaseDao = DaoFactory.GetAssociatedCaseDao()
                End If
                Return _associatedCaseDao
            End Get
        End Property

        Protected ReadOnly Property AssociatedCase() As SpecialCase
            Get
                If (_special Is Nothing) Then
                    _special = SCDao.GetById(GetAssociatedRefId(), False)
                End If
                Return _special
            End Get

        End Property

        Protected ReadOnly Property SpecCase() As SC_PSCD
            Get
                If (_sc Is Nothing) Then
                    _sc = SCDao.GetById(Request.QueryString("refId"))
                End If

                Return _sc
            End Get
        End Property

        Public Function GetStartPageTitle(ByVal workStatus As SpecCasePSCDStatusCode) As String
            Dim startPage As String = "PSC Member"

            Return startPage
        End Function

        Public Sub InitPageAccess(ByVal specCase As SC_PSCD)
            Dim accessDao As IPageAccessDao = New NHibernateDaoFactory().GetPageAccessDao()
            Dim associatedRefId As Integer = GetAssociatedRefId()
            Navigator.ClearSession()
            Navigator.InitControl()
            If (associatedRefId > 0) Then
                Navigator.SetPageAccess(accessDao.GetByWorkflowGroupAndStatus(specCase.Workflow, GroupId, specCase.Status), AssociatedCase.Workflow)
            Else
                Navigator.SetPageAccess(accessDao.GetByWorkflowGroupAndStatus(specCase.Workflow, GroupId, specCase.Status))
            End If

        End Sub

        Public Function LoadCase(ByVal refId As Integer) As SC_PSCD
            Dim scPSCD As SC_PSCD = Nothing

            Try
                If _scId <> refId Then
                    _sc = Nothing 'Ensure we grab based on refId
                    _scId = refId
                End If
                scPSCD = SpecCase
            Catch ex As Exception
                'We failed to load the case, nothing we can do
                ALOD.Logging.LogManager.LogError(ex.ToString() + " \ refId: " + refId.ToString())
                SetErrorMessage(Resources.Messages.ERROR_LOADING_CASE)
                Response.Redirect(Resources._Global.StartPage, True)
            End Try

            If (scPSCD Is Nothing) Then
                'We failed to load the case, nothing we can do
                ALOD.Logging.LogManager.LogError("NULL Recruiting Services Error loading Recruiting Services with requestId: " + refId.ToString())
                SetErrorMessage(Resources.Messages.ERROR_LOADING_CASE)
                Response.Redirect(Resources._Global.StartPage, True)
            End If

            Return scPSCD
        End Function

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
            Dim refId As Integer = 0
            Integer.TryParse(Request.QueryString("RefId"), refId)

            If (refId = 0) Then
                Response.Redirect(Resources._Global.StartPage, True)
            End If

            Dim sessionId As Integer = 0
            Integer.TryParse(Session("refId"), sessionId)

            If (sessionId = 0) Then
                Response.Redirect(Resources._Global.StartPage, True)
            End If

            If (refId <> sessionId) Then
                InitNavigator(refId)
            End If

            ModHeader.ModuleHeader = "PSCD"
        End Sub

        Private Function GetAssociatedRefId() As Integer
            Dim refId As Integer = 0
            Dim associatedRefid As Integer = 0
            Integer.TryParse(Request.QueryString("refId"), refId)
            Dim sc = SCDao.GetById(refId)
            Dim cases As IList(Of AssociatedCase) = New List(Of AssociatedCase)
            cases = cases.Concat(Associated.GetAssociatedCasesSC(sc.Id, sc.Workflow)).ToList()
            If (cases.Count > 0) Then
                Return cases.Item(0).associated_RefId
            Else
                Return 0
            End If
        End Function

        Private Sub InitNavigator(ByVal refId As Integer)
            Dim factory As New NHibernateDaoFactory()
            Dim userAccess As PageAccess.AccessLevel = PageAccess.AccessLevel.None
            Dim scPSCD As New SC_PSCD

            Session("refId") = refId

            userAccess = SCDao.GetUserAccess(SESSION_USER_ID, refId, ModuleType.SpecCasePSCD)

            Utility.VerifyUserAccess(userAccess, Resources.Messages.ERROR_NO_ACCESS, Resources._Global.StartPage)

            scPSCD = LoadCase(refId)

            Utility.UpdateCaseLock(userAccess, refId, ModuleType.SpecCasePSCD)
            InitPageAccess(scPSCD)

            Navigator.Commit()
            SCDao.Evict(scPSCD)
        End Sub

        Private Sub TabButtonClicked(ByRef sender As Object, ByRef e As TabNavigationEventArgs) Handles TabNavigator1.ButtonClicked
            RaiseEvent TabClick(Me, e)
        End Sub

    End Class

End Namespace