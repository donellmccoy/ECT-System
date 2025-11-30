Imports ALOD.Core.Utils
Imports ALODWebUtility.Common

Namespace Web

    Partial Class _Default
        Inherits System.Web.UI.Page

        Protected Sub btnLoginCac_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnLoginCac.Click
            Response.Redirect("~/login/Login.aspx")
        End Sub

        Protected Sub lnkDevLogin_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles lnkDevLogin.Click
            UserPreferences.SaveSetting("devLogin", "1")
            Response.Redirect("~/public/devlogin.aspx")
            ' this is a test
        End Sub

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

            If (Not IsPostBack) Then

                If (AppMode <> DeployMode.Production AndAlso ConfigurationManager.AppSettings("DevLoginEnabled") = "Y") Then
                    lnkDevLogin.Visible = True
                Else
                    lnkDevLogin.Visible = False
                End If

                btnLoginCac.Attributes.Add("onclick", "showDialog(); return false;")
                If (UserPreferences.GetSetting("devLogin") = "1") Then
                    Dim url As String = "~/public/devlogin.aspx"
                    Response.Redirect(url)
                End If

            End If

        End Sub

    End Class

End Namespace