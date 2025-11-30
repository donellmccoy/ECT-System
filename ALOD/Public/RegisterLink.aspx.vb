Imports ALOD.Core.Domain.Users
Imports ALOD.Core.Utils
Imports ALOD.Data.Services
Imports ALODWebUtility.Common

Namespace Web

    Partial Class Public_RegisterLink
        Inherits System.Web.UI.Page

        Public Property CAC_SSN() As String
            Get
                Return Session("CAC_SSN")

            End Get
            Set(ByVal value As String)
                Session("CAC_SSN") = value
            End Set
        End Property

        Protected Sub accountLinkButton_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles accountLinkButton.Click
            VerifyAccess()
            If (SSNMatched() = False) Then
                Exit Sub
            End If
            ProcessLogin(CAC_SSN)
        End Sub

        Protected Sub linkBtn_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles linkBtn.Click
            VerifyAccess()
            If (SSNMatched() = False) Then
                Exit Sub
            End If
            LinkUser()
        End Sub

        Protected Sub LinkUser()
            Dim userId As Integer = 0

            Dim ssn As String = Server.HtmlEncode(SSNTextBox.Text).Trim().Replace("-", "")
            Dim firstName As String = Server.HtmlEncode(FirstNameTextBox.Text).Trim()
            Dim lastName As String = Server.HtmlEncode(LastNameTextBox.Text).Trim()
            Dim userName As String = Server.HtmlEncode(UserNameTextBox.Text).Trim()

            userId = UserService.GetIDByCredentials(firstName, lastName, userName, SESSION_SSN)

            If userId = 0 Then
                accountLinkedDiv.Visible = False
                registerLinkedDiv.Visible = True
            Else
                linkBtn.Enabled = False
                accountLinkedDiv.Visible = True
                registerLinkedDiv.Visible = False
            End If

        End Sub

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
            VerifyAccess()
            SetInputFormatRestriction(Page, SSNTextBox, FormatRestriction.Numeric)
        End Sub

        Protected Sub ProcessLogin(ByVal ssn As String)
            If (AppMode = DeployMode.Production) Then
                Exit Sub
            End If

            Dim appUser As ALOD.Core.Domain.Users.AppUser = UserService.GetBySSN(ssn)

            If (appUser Is Nothing) OrElse
                (appUser.Status <> ALOD.Core.Domain.Users.AccessStatus.Approved) OrElse
                (appUser.CurrentRole.Id = 0) Then

                RedirectToRegistration(appUser, ssn)
            End If

            If (Not SetLogin(appUser)) Then
                ' User is already logged into a different session; SetLogin has done a redirect to the
                'AltSession page therefore we need to exit the subprocedure in order to not override that call
                Exit Sub
            End If

            Response.Redirect("~/Secure/Welcome.aspx")

        End Sub

        Protected Sub RedirectToRegistration(ByVal appUser As ALOD.Core.Domain.Users.AppUser, ByVal ssn As String)

            If (appUser Is Nothing) OrElse (appUser.Id = 0) Then
                'since this is not a registered user, remove the auth ticket.
                'doing this denies them access to /secure parts of the site
                FormsAuthentication.SignOut()
            End If
            Response.Redirect("~/Public/Register1.aspx")

        End Sub

        Protected Sub registerLinkButton_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles registerLinkButton.Click
            VerifyAccess()
            If (SSNMatched() = False) Then
                Exit Sub
            End If
            Response.Redirect("~/Public/Register1.aspx")
        End Sub

        Protected Function SSNMatched() As Boolean

            Dim ssn As String = Server.HtmlEncode(SSNTextBox.Text).Trim().Replace("-", "")
            If ssn.Length <> 9 Then
                InvalidSSNLabel.Visible = True
                Return False
            End If

            InvalidSSNLabel.Visible = False
            If CAC_SSN <> ssn Then
                IncorrectSSNLabel.Visible = True
                Return False
            End If
            IncorrectSSNLabel.Visible = False
            Return True

        End Function

        Protected Sub VerifyAccess()

            If (Session("ssn") Is Nothing) Then
                Response.Redirect("~/Default.aspx")
            End If

            'This is just to make sure that the page is not browsed back and reused
            If Not Page.IsPostBack Then
                CAC_SSN = Session("ssn")
            End If

            If CAC_SSN <> Session("ssn") Then
                Response.Redirect("~/Default.aspx")
            End If

        End Sub

    End Class

End Namespace