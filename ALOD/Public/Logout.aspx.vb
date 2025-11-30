Imports ALODWebUtility.Providers

Namespace Web

    Partial Class Logout
        Inherits System.Web.UI.Page

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

            ALOD.Data.Services.UserService.Logout(Session("UserId"))
            Session.Clear()
            Session.Abandon()
            AuthenticationHandler.Logout()
            FormsAuthentication.SignOut()

            'Response.Redirect("~/Default.aspx")

        End Sub

    End Class

End Namespace