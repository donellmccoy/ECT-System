Imports ALOD.Core.Domain.Modules.Lod
Imports ALOD.Core.Domain.Modules.SpecialCases
Imports ALOD.Core.Domain.Users
Imports ALOD.Core.Domain.Workflow
Imports ALOD.Core.Interfaces.DAOInterfaces
Imports ALOD.Data
Imports ALOD.Web.UserControls
Imports ALODWebUtility.Common

Namespace Web.Special_Case.PH

    Partial Class Secure_ph_init
        Inherits System.Web.UI.Page

        Private _userDao As IUserDao
        Dim dao As ISpecialCaseDAO
        Private sc As SC_PH = Nothing
        Private scId As Integer = 0

        ReadOnly Property SCDao() As ISpecialCaseDAO
            Get
                If (dao Is Nothing) Then
                    dao = New NHibernateDaoFactory().GetSpecialCaseDAO()
                End If

                Return dao
            End Get
        End Property

        Public ReadOnly Property Navigator() As TabNavigator
            Get
                Return MasterPage.Navigator()
            End Get
        End Property

        Protected ReadOnly Property MasterPage() As SC_PHMaster
            Get
                Dim master As SC_PHMaster = CType(Page.Master, SC_PHMaster)
                Return master
            End Get
        End Property

        Protected ReadOnly Property SpecCase() As SC_PH
            Get
                If (sc Is Nothing) Then
                    sc = SCDao.GetById(Request.QueryString("refId"))
                End If

                Return sc
            End Get
        End Property

        Protected ReadOnly Property UserDao() As IUserDao
            Get
                If (_userDao Is Nothing) Then
                    _userDao = New NHibernateDaoFactory().GetUserDao()
                End If

                Return _userDao
            End Get
        End Property

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

            If (IsPostBack) Then
                Exit Sub
            End If

            Dim refId As Integer = 0
            Integer.TryParse(Request.QueryString("refId"), refId)

            If (refId = 0) Then
                SetErrorMessage(Resources.Messages.ERROR_LOADING_CASE)
                Response.Redirect(Resources._Global.StartPage, True)
            End If

            Dim factory As New NHibernateDaoFactory()

            'make sure the user has access to this case
            Dim userAccess As PageAccess.AccessLevel = SCDao.GetUserAccess(SESSION_USER_ID, refId, ModuleType.SpecCasePH)

            If (userAccess = PageAccess.AccessLevel.None) Then
                SetErrorMessage(Resources.Messages.ERROR_NO_ACCESS)
                Response.Redirect(Resources._Global.StartPage, True)
            End If

            Dim scPH As New SC_PH
            Try
                If scId <> refId Then
                    sc = Nothing  'Ensure we grab based on refId
                    scId = refId
                End If
                scPH = SpecCase
            Catch ex As Exception
                'we failed to load the case, nothing we can do
                ALOD.Logging.LogManager.LogError(ex.ToString() + " \ refId: " + refId.ToString())
                SetErrorMessage(Resources.Messages.ERROR_LOADING_CASE)
                Response.Redirect(Resources._Global.StartPage, True)
            End Try

            If (scPH Is Nothing) Then
                'we failed to load the case, nothing we can do
                ALOD.Logging.LogManager.LogError("NULL Recruiting Services Error loading Recruiting Services with requestId: " + refId.ToString())
                SetErrorMessage(Resources.Messages.ERROR_LOADING_CASE)
                Response.Redirect(Resources._Global.StartPage, True)
            End If

            Session("refId") = refId

            'now see if we can get a lock for this case
            Dim lockDao As ICaseLockDao = factory.GetCaseLockDao()

            'start by clearing any locks the user might have
            lockDao.ClearLocksForUser(SESSION_USER_ID)

            'check the case-lock if the user is attempting to edit
            If (userAccess = PageAccess.AccessLevel.ReadWrite) Then

                Dim lock As CaseLock = lockDao.GetByReferenceId(refId, ModuleType.SpecCasePH)

                If (lock Is Nothing) Then
                    'there is not a lock on this case, see if we can get one
                    lock = New CaseLock()
                    lock.UserId = SESSION_USER_ID
                    lock.ReferenceId = refId
                    lock.ModuleType = ModuleType.SpecCasePH
                    lock.LockTime = DateTime.Now

                    lockDao.Save(lock)
                    SESSION_LOCK_ID = lock.Id
                    SESSION_LOCK_AQUIRED = True
                Else
                    'there is already a lock on this case.
                    SESSION_LOCK_ID = lock.Id

                    'does it belong to the current user?
                    If (lock.UserId = SESSION_USER_ID) Then
                        SESSION_LOCK_AQUIRED = True
                    Else
                        SESSION_LOCK_AQUIRED = False
                    End If
                End If
            Else
                'no need to check lock, since it will be read-only anyway
                SESSION_LOCK_ID = 0
                SESSION_LOCK_AQUIRED = False
            End If

            'when this page loads we always want to clear out any previous navigator and start fresh
            Navigator.ClearSession()
            'and initialize our control  -- note: this gets called again [reseting the Navigator] in SMData (so, any exceptions need to be there too)
            Navigator.InitControl()

            'now set our page access for this session
            Dim access As IList(Of PageAccess)
            Dim groupId As Byte = CByte(Session("GroupId"))
            Dim accessDao As IPageAccessDao = New NHibernateDaoFactory().GetPageAccessDao()
            access = accessDao.GetByWorkflowGroupAndStatus(scPH.Workflow, groupId, scPH.Status)
            Navigator.SetPageAccess(access)

            Dim startPage As String = "PH Form"
            Dim group As UserGroups = Session("GroupId")

            Select Case scPH.Status
                Case SpecCasePHWorkStatus.InitiateCase, SpecCasePHWorkStatus.Delinquent
                    If (group = UserGroups.UnitPH) Then
                        startPage = "PH Form"
                    End If
                Case SpecCasePHWorkStatus.AFRCReview, SpecCasePHWorkStatus.DelinquentAFRCReview
                    If (group = UserGroups.HQAFRCDPH) Then
                        startPage = "PH Form"
                    End If
                Case Else
                    startPage = "PH Form"
            End Select

            'make sure the start page we have is visible
            If (Not Navigator(startPage).Visible) Then
                startPage = "PH Form"
            End If

            'remove the scPEPP from the rDao so no changes are persisted back
            SCDao.Evict(scPH)

            Navigator.MoveToPage(startPage)

        End Sub

    End Class

End Namespace