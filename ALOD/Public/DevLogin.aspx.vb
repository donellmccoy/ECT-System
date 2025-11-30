Imports ALOD.Core.Domain.Users
Imports ALOD.Core.Utils
Imports ALOD.Data.Services
Imports ALODWebUtility.Common

Namespace Web

    Partial Class Public_DevLogin
        Inherits System.Web.UI.Page

        Protected Sub btnLogin_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnLogin.Click

            If (EDIPIN.Text.Trim.Length = 0) Then
                Exit Sub
            End If

            UserPreferences.SaveSetting("devEDIPIN", EDIPIN.Text.Trim)
            'For dev we assume the ssn and edipin are the same
            ProcessDevLogin(EDIPIN.Text.Trim)

        End Sub

        Protected Sub btnRoleLogin_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnRoleLogin.Click, rblRoles.SelectedIndexChanged

            If (rblRoles.SelectedIndex = -1) Then
                Exit Sub
            End If

            UserPreferences.SaveSetting("devWing", WingSelect.SelectedValue)
            UserPreferences.SaveSetting("devShowBoard", ShowBoard.Checked.ToString())
            'For dev we assume the ssn and edipin are the same
            ProcessDevLogin(rblRoles.SelectedValue)

        End Sub

        Protected Sub EDIPIN_TextChanged(sender As Object, e As EventArgs)

        End Sub

        Protected Sub lnkNormalLogin_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles lnkNormalLogin.Click
            UserPreferences.SaveSetting("devLogin", "0")
            Response.Redirect("~/Default.aspx", False)
            HttpContext.Current.ApplicationInstance.CompleteRequest()
        End Sub

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

            'SetDefaultButton(SSN, btnLogin)

            If (AppMode = DeployMode.Production) OrElse (ConfigurationManager.AppSettings("DevLoginEnabled") <> "Y") Then
                UserPreferences.SaveSetting("devLogin", "0")
                Response.Redirect("~/Default.aspx", False)
                HttpContext.Current.ApplicationInstance.CompleteRequest()
            End If

            If (Not IsPostBack) Then
                EDIPIN.Text = UserPreferences.GetSetting("devEDIPIN")

                Dim show As Boolean = False
                Boolean.TryParse(UserPreferences.GetSetting("devShowBoard"), show)
                ShowBoard.Checked = show

                EDIPIN.Focus()
            End If

        End Sub

        Protected Sub ProcessDevLogin(ByVal edipin As String)

            If (AppMode = DeployMode.Production) Then
                Exit Sub
            End If

            Dim users As IList(Of AppUser) = UserService.FindByEDIPIN(edipin)

            'update any expired accounts
            Dim expiredAccounts = From u In users
                                  Where u.Status = AccessStatus.Approved _
                                  AndAlso u.AccountExpiration < DateTime.Now
                                  Select u

            For Each account As AppUser In expiredAccounts
                account.Status = AccessStatus.Disabled
            Next

            'grab the users valid accounts
            Dim validAccounts = From u In users
                                Where u.Status = AccessStatus.Approved _
                                AndAlso u.CurrentRole.Id > 0
                                Select u

            If (validAccounts.Count = 0) Then
                RedirectToRegistration(edipin)
            End If

            'the user has at least one valid account
            Dim url As String = Resources._Global.StartPage

            If (validAccounts.Count = 1) Then
                'if they only have one, use that and log them in
                If (Not SetLogin(validAccounts(0))) Then
                    ' User is already logged into a different session; SetLogin has done a redirect to the
                    'AltSession page therefore we need to exit the subprocedure in order to not override that call
                    Exit Sub
                End If
            Else
                'the user has more than one acccount, redirect to the account selection page
                SESSION_EDIPIN = edipin
                url = "~/Public/SelectAccount.aspx"
            End If

            Response.Redirect(url, False)
            HttpContext.Current.ApplicationInstance.CompleteRequest()

        End Sub

        Protected Sub RedirectToRegistration(ByVal edipin As String)

            'since this is not a registered user, remove the auth ticket.
            'doing this denies them access to /secure parts of the site
            FormsAuthentication.SignOut()

            SESSION_EDIPIN = edipin
            SESSION_COMPO = cbCompo.SelectedValue

            Response.Redirect("~/Public/Register1.aspx?", False)
            HttpContext.Current.ApplicationInstance.CompleteRequest()
        End Sub

        Protected Sub WingSelect_DataBound(ByVal sender As Object, ByVal e As System.EventArgs) Handles WingSelect.DataBound

            If (Not IsPostBack) Then
                SetDropdownByValue(WingSelect, UserPreferences.GetSetting("devWing"))
            End If

        End Sub

    End Class

End Namespace