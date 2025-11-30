Imports ALOD.Core.Domain.Modules.SARC
Imports ALOD.Core.Domain.Workflow
Imports ALOD.Core.Interfaces.DAOInterfaces
Imports ALOD.Data
Imports ALOD.Web.UserControls
Imports ALODWebUtility.Common

Namespace Web.SARC

    Partial Class Secure_sarc_init
        Inherits System.Web.UI.Page

        Dim dao As ISARCDAO
        Private sarc As RestrictedSARC = Nothing
        Private sarcId As Integer = 0

        ReadOnly Property SARCDao() As ISARCDAO
            Get
                If (dao Is Nothing) Then
                    dao = New NHibernateDaoFactory().GetSARCDao()
                End If

                Return dao
            End Get
        End Property

        Public ReadOnly Property Navigator() As TabNavigator
            Get
                Return MasterPage.Navigator()
            End Get
        End Property

        Protected ReadOnly Property MasterPage() As SARCMaster
            Get
                Dim master As SARCMaster = CType(Page.Master, SARCMaster)
                Return master
            End Get
        End Property

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
            If (IsPostBack) Then
                Exit Sub
            End If

            Dim sarcId As Integer = 0
            Integer.TryParse(Request.QueryString("refId"), sarcId)

            If (sarcId = 0) Then
                SetErrorMessage(Resources.Messages.ERROR_LOADING_CASE)
                Response.Redirect(Resources._Global.StartPage, True)
            End If

            Dim factory As New NHibernateDaoFactory()
            Dim sarc As New RestrictedSARC()
            Dim sDao As ISARCDAO = factory.GetSARCDao()
            Dim startPage As String = String.Empty
            Dim userAccess As PageAccess.AccessLevel = PageAccess.AccessLevel.None

            userAccess = sDao.GetUserAccess(SESSION_USER_ID, sarcId)

            Utility.VerifyUserAccess(userAccess, Resources.Messages.ERROR_NO_ACCESS, Resources._Global.StartPage)

            sarc = MasterPage.LoadCase(sarcId, sDao)

            Session("refId") = sarcId

            Utility.UpdateCaseLock(userAccess, sarcId, ModuleType.SARC)
            MasterPage.InitPageAccess(sarc)

            startPage = MasterPage.GetStartPageTitle(sarc.Status)

            ' Remove the sarc from the sDao so no changes are persisted back
            sDao.Evict(sarc)

            Navigator.MoveToPage(startPage)
        End Sub

    End Class

End Namespace