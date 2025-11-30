Imports ALOD.Core.Domain.Modules.SpecialCases
Imports ALOD.Core.Domain.Workflow
Imports ALOD.Core.Interfaces.DAOInterfaces
Imports ALOD.Data
Imports ALOD.Web.UserControls
Imports ALODWebUtility.Common

Namespace Web.Special_Case.BCMR

    Partial Class Secure_sc_bc_init
        Inherits System.Web.UI.Page

        Public ReadOnly Property Navigator() As TabNavigator
            Get
                Return MasterPage.Navigator()
            End Get
        End Property

        Protected ReadOnly Property MasterPage() As SC_BCMRMaster
            Get
                Dim master As SC_BCMRMaster = CType(Page.Master, SC_BCMRMaster)
                Return master
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
            Dim scDao As ISpecialCaseDAO = factory.GetSpecialCaseDAO()

            'make sure the user has access to this case
            Dim userAccess As PageAccess.AccessLevel = scDao.GetUserAccess(SESSION_USER_ID, refId, ModuleType.SpecCaseBCMR)

            Utility.VerifyUserAccess(userAccess, Resources.Messages.ERROR_NO_ACCESS, Resources._Global.StartPage)

            Dim scBC As SC_BCMR = MasterPage.LoadCase(refId)

            Session("refId") = refId

            Utility.UpdateCaseLock(userAccess, refId, ModuleType.SpecCaseBCMR)

            MasterPage.InitPageAccess(scBC)

            Dim startPage As String = MasterPage.GetStartPageTitle(scBC.Status)

            'remove the scCI from the scDao so no changes are persisted back
            scDao.Evict(scBC)

            Navigator.MoveToPage(startPage)
        End Sub

    End Class

End Namespace