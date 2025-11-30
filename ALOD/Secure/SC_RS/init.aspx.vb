Imports ALOD.Core.Domain.Modules.SpecialCases
Imports ALOD.Core.Domain.Workflow
Imports ALOD.Core.Interfaces.DAOInterfaces
Imports ALOD.Data
Imports ALOD.Web.UserControls
Imports ALODWebUtility.Common

Namespace Web.Special_Case.RS

    Partial Class Secure_rs_init
        Inherits System.Web.UI.Page

        Dim dao As ISpecialCaseDAO

        Private sc As SC_RS = Nothing
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

        Protected ReadOnly Property MasterPage() As SC_RSMaster
            Get
                Dim master As SC_RSMaster = CType(Page.Master, SC_RSMaster)
                Return master
            End Get
        End Property

        Protected ReadOnly Property SpecCase() As SC_RS
            Get
                If (sc Is Nothing) Then
                    sc = SCDao.GetById(Request.QueryString("refId"))
                End If

                Return sc
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
            Dim userAccess As PageAccess.AccessLevel = SCDao.GetUserAccess(SESSION_USER_ID, refId, ModuleType.SpecCaseRS)

            Utility.VerifyUserAccess(userAccess, Resources.Messages.ERROR_NO_ACCESS, Resources._Global.StartPage)

            Dim scRS As SC_RS = MasterPage.LoadCase(refId)

            Session("refId") = refId

            Utility.UpdateCaseLock(userAccess, refId, ModuleType.SpecCaseRS)

            MasterPage.InitPageAccess(scRS)

            Dim startPage As String = MasterPage.GetStartPageTitle(scRS.Status)

            'remove the scRS from the rDao so no changes are persisted back
            SCDao.Evict(scRS)

            Navigator.MoveToPage(startPage)
        End Sub

    End Class

End Namespace