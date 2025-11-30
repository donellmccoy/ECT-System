Imports ALOD.Core.Domain.Users
Imports ALOD.Data.Services

Imports ALODWebUtility.Common

Namespace Web

    Partial Class login_SelectAccount
        Inherits System.Web.UI.Page

        Protected Sub AccountRepeater_ItemCommand(ByVal source As Object, ByVal e As System.Web.UI.WebControls.RepeaterCommandEventArgs) Handles AccountRepeater.ItemCommand

            Dim userId As Integer = 0
            Integer.TryParse(e.CommandArgument, userId)

            If (userId = 0) Then
                Response.Redirect("~/Default.aspx")
            End If

            Dim user As AppUser = UserService.GetById(userId)

            If (Not SetLogin(user)) Then
                ' User is already logged into a different session; SetLogin has done a redirect to the
                'AltSession page therefore we need to exit the subprocedure in order to not override that call
                Exit Sub
            End If

            Dim url As String = Resources._Global.StartPage

            If (SESSION_REDIRECT_URL IsNot Nothing) Then
                url = SESSION_REDIRECT_URL
            End If

            Response.Redirect(url)

        End Sub

        Protected Sub AccountRepeater_ItemDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.RepeaterItemEventArgs) Handles AccountRepeater.ItemDataBound

            Dim user As AppUser = DirectCast(e.Item.DataItem, AppUser)
            Dim link As LinkButton = DirectCast(e.Item.FindControl("AccountLink"), LinkButton)

            link.CommandArgument = user.Id.ToString()
            link.Text = user.Username + " - " +
                        user.CurrentRole.Group.Description + " - " +
                        user.Unit.Name

        End Sub

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

            If (Not IsPostBack) Then
                InitControls()
            End If

        End Sub

        Private Sub InitControls()

            '  If (SESSION_SSN Is Nothing OrElse SESSION_SSN.Length = 0) Then
            'Response.Redirect("~/Default.aspx")
            'End If

            If (SESSION_EDIPIN Is Nothing OrElse SESSION_EDIPIN.Length = 0) Then
                Response.Redirect("~/Default.aspx")
            End If

            Dim users As IList(Of AppUser)

            users = UserService.FindByEDIPIN(SESSION_EDIPIN)
            AccountRepeater.DataSource = From u In users
                                         Where u.Status = AccessStatus.Approved _
                                         And u.AccountExpiration >= DateTime.Now
                                         Select u
                                         Order By u.Username

            AccountRepeater.DataBind()

        End Sub

    End Class

End Namespace