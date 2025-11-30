Imports ALOD.Core.Domain.Modules.Appeals
Imports ALOD.Core.Domain.Workflow
Imports ALOD.Core.Interfaces.DAOInterfaces
Imports ALOD.Data
Imports ALOD.Web.UserControls
Imports ALODWebUtility.Common

Namespace Web.AP

    Partial Class Secure_ap_init
        Inherits System.Web.UI.Page

        Public ReadOnly Property Navigator() As TabNavigator
            Get
                Return MasterPage.Navigator()
            End Get
        End Property

        Protected ReadOnly Property MasterPage() As AppealRequestMaster
            Get
                Dim master As AppealRequestMaster = CType(Page.Master, AppealRequestMaster)
                Return master
            End Get
        End Property

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
            If (IsPostBack) Then
                Exit Sub
            End If

            Dim appealId As Integer = 0
            Integer.TryParse(Request.QueryString("requestId"), appealId)

            If (appealId = 0) Then
                SetErrorMessage(Resources.Messages.ERROR_LOADING_CASE)
                Response.Redirect(Resources._Global.StartPage, True)
            End If

            Dim factory As New NHibernateDaoFactory()
            Dim startPage As String = String.Empty
            Dim apLod As New LODAppeal()
            Dim aDao As ILODAppealDAO = factory.GetLODAppealDao()

            'make sure the user has access to this case
            Dim userAccess As PageAccess.AccessLevel = aDao.GetUserAccess(SESSION_USER_ID, appealId)

            Utility.VerifyUserAccess(userAccess, Resources.Messages.ERROR_NO_ACCESS, Resources._Global.StartPage)

            apLod = MasterPage.LoadCase(appealId, aDao)

            Session("requestId") = appealId

            Utility.UpdateCaseLock(userAccess, appealId, ModuleType.AppealRequest)

            MasterPage.InitPageAccess(apLod)

            startPage = MasterPage.GetStartPageTitle()

            'remove the apLod from the aDao so no changes are persisted back
            aDao.Evict(apLod)

            Navigator.MoveToPage(startPage)
        End Sub

    End Class

End Namespace