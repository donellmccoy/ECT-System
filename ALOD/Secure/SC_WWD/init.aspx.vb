Imports ALOD.Core.Domain.Modules.SpecialCases
Imports ALOD.Core.Domain.Workflow
Imports ALOD.Core.Interfaces.DAOInterfaces
Imports ALOD.Data
Imports ALOD.Web.UserControls
Imports ALODWebUtility.Common

Namespace Web.Special_Case.WWD

    Partial Class Secure_WWD_init
        Inherits System.Web.UI.Page

        Dim dao As ISpecialCaseDAO

        Private sc As SC_WWD = Nothing
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

        Protected ReadOnly Property MasterPage() As SC_WWDMaster
            Get
                Dim master As SC_WWDMaster = CType(Page.Master, SC_WWDMaster)
                Return master
            End Get
        End Property

        Protected ReadOnly Property SpecCase() As SC_WWD
            Get
                If (sc Is Nothing) Then
                    sc = SCDao.GetById(Request.QueryString("RefId"))
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
            Dim userAccess As PageAccess.AccessLevel = SCDao.GetUserAccess(SESSION_USER_ID, refId, ModuleType.SpecCaseWWD)

            Utility.VerifyUserAccess(userAccess, Resources.Messages.ERROR_NO_ACCESS, Resources._Global.StartPage)

            Dim scWWD As SC_WWD = MasterPage.LoadCase(refId)

            Session("refId") = refId

            Utility.UpdateCaseLock(userAccess, refId, ModuleType.SpecCaseWWD)

            MasterPage.InitPageAccess(scWWD)

            Dim startPage As String = MasterPage.GetStartPageTitle(scWWD.Status)

            'remove the scWWD from the rDao so no changes are persisted back
            SCDao.Evict(scWWD)

            Navigator.MoveToPage(startPage)
        End Sub

    End Class

End Namespace