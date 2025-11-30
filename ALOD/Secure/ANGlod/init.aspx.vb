Imports ALOD.Data
Imports ALOD.Core.Domain.Users
Imports ALOD.Core.Domain.Workflow
Imports ALOD.Core.Interfaces
Imports ALOD.Core.Domain.Modules.Lod
Imports ALOD.Web.UserControls
Imports ALODWebUtility.Common
Imports ALODWebUtility.TabNavigation

Namespace Web.LOD
    Partial Class Secure_lod_init
        Inherits System.Web.UI.Page

        Private _daoFactory As NHibernateDaoFactory

        Public ReadOnly Property DaoFactory
            Get
                If (_daoFactory Is Nothing) Then
                    _daoFactory = New NHibernateDaoFactory()
                End If

                Return _daoFactory
            End Get
        End Property

        Protected ReadOnly Property MasterPage() As LodMaster
            Get
                Dim master As LodMaster = CType(Page.Master, LodMaster)
                Return master
            End Get
        End Property

        Public ReadOnly Property Navigator() As TabNavigator
            Get
                Return MasterPage.Navigator()
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

            Dim lod As LineOfDuty
            Dim dao As LineOfDutyDao = DaoFactory.GetLineOfDutyDao()

            ' Make sure the user has access to this case
            Dim userAccess As PageAccess.AccessLevel = dao.GetUserAccess(SESSION_USER_ID, refId)

            Utility.VerifyUserAccess(userAccess, Resources.Messages.ERROR_NO_ACCESS, Resources._Global.StartPage)

            lod = MasterPage.LoadCase(refId, dao)

            Session("RefId") = refId

            Utility.UpdateCaseLock(userAccess, refId, ModuleType.LOD)
            MasterPage.InitPageAccess(lod)

            Dim startPage As String = MasterPage.GetStartPageTitle(lod.CurrentStatusCode, lod.Formal)

            ' Remove the LOD from the DAO so no changes are persisted back
            dao.Evict(lod)

            Navigator.MoveToPage(startPage)
        End Sub
    End Class
End Namespace

