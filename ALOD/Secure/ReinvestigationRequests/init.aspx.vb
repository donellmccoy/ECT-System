Imports ALOD.Core.Domain.Modules.Reinvestigations
Imports ALOD.Core.Domain.Workflow
Imports ALOD.Core.Interfaces.DAOInterfaces
Imports ALOD.Data
Imports ALOD.Web.UserControls
Imports ALODWebUtility.Common

Namespace Web.RR

    Partial Class Secure_rr_init
        Inherits System.Web.UI.Page

        Public ReadOnly Property Navigator() As TabNavigator
            Get
                Return MasterPage.Navigator()
            End Get
        End Property

        Protected ReadOnly Property MasterPage() As ReinvestigationRequestMaster
            Get
                Dim master As ReinvestigationRequestMaster = CType(Page.Master, ReinvestigationRequestMaster)
                Return master
            End Get
        End Property

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
            If (IsPostBack) Then
                Exit Sub
            End If

            Dim requestId As Integer = 0
            Integer.TryParse(Request.QueryString("requestId"), requestId)

            If (requestId = 0) Then
                SetErrorMessage(Resources.Messages.ERROR_LOADING_CASE)
                Response.Redirect(Resources._Global.StartPage, True)
            End If

            Dim factory As New NHibernateDaoFactory()
            Dim startPage As String = String.Empty
            Dim rrLod As New LODReinvestigation()
            Dim rDao As ILODReinvestigateDAO = factory.GetLODReinvestigationDao()

            'make sure the user has access to this case
            Dim userAccess As PageAccess.AccessLevel = rDao.GetUserAccess(SESSION_USER_ID, requestId)

            Utility.VerifyUserAccess(userAccess, Resources.Messages.ERROR_NO_ACCESS, Resources._Global.StartPage)

            rrLod = MasterPage.LoadCase(requestId, rDao)

            Session("requestId") = requestId

            Utility.UpdateCaseLock(userAccess, requestId, ModuleType.ReinvestigationRequest)

            MasterPage.InitPageAccess(rrLod)

            startPage = MasterPage.GetStartPageTitle(rrLod.Status)

            'remove the rrLod from the rDao so no changes are persisted back
            rDao.Evict(rrLod)

            Navigator.MoveToPage(startPage)
        End Sub

    End Class

End Namespace