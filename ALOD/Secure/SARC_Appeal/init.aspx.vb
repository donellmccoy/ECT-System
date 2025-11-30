Imports ALOD.Core.Domain.Modules.SARC
Imports ALOD.Core.Domain.Workflow
Imports ALOD.Core.Interfaces.DAOInterfaces
Imports ALOD.Data
Imports ALOD.Web.UserControls
Imports ALODWebUtility.Common

Namespace Web.APSA

    Partial Class Secure_apsa_init
        Inherits System.Web.UI.Page

        Public ReadOnly Property Navigator() As TabNavigator
            Get
                Return MasterPage.Navigator()
            End Get
        End Property

        Protected ReadOnly Property MasterPage() As SARCAppealMaster
            Get
                Dim master As SARCAppealMaster = CType(Page.Master, SARCAppealMaster)
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
            Dim apSARC As New SARCAppeal()
            Dim aDao As ISARCAppealDAO = factory.GetSARCAppealDao()
            Dim startPage As String
            Dim userAccess As PageAccess.AccessLevel = PageAccess.AccessLevel.None

            userAccess = aDao.GetUserAccess(SESSION_USER_ID, appealId)

            Utility.VerifyUserAccess(userAccess, Resources.Messages.ERROR_NO_ACCESS, Resources._Global.StartPage)

            apSARC = MasterPage.LoadCase(appealId, aDao)

            Session("requestId") = appealId

            Utility.UpdateCaseLock(userAccess, appealId, ModuleType.SARCAppeal)

            MasterPage.InitPageAccess(apSARC)

            startPage = MasterPage.GetStartPageTitle(apSARC.Status)

            ' Remove the sarc from the sDao so no changes are persisted back
            aDao.Evict(apSARC)

            Navigator.MoveToPage(startPage)
        End Sub

    End Class

End Namespace