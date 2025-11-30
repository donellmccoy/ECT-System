Imports ALOD.Core.Domain.Users
Imports ALOD.Data
Imports ALOD.Data.Services
Imports ALODWebUtility.Common
Imports ALODWebUtility.Providers

Namespace Web.Login

    Partial Class Public_Login
        Inherits System.Web.UI.Page

        'Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        '    Dim c As CMS.Utility.Security.ICertificateReader

        '    Try
        '        c = CMS.Utility.Security.certificateReaderFactory.getCertificateReader()
        '        c.SelectCertificate(Request.ClientCertificate)

        '        If (c.certificateErrors.Length = 0) AndAlso (c.certificateEDIPIN.Length > 0) Then
        '            Dim loggingUser As AppUser = UserService.GetByEDIPIN(c.certificateEDIPIN)
        '            Dim edipin As String
        '            edipin = c.certificateEDIPIN
        '            If loggingUser Is Nothing Then
        '                SESSION_EDIPIN = edipin
        '                SafeResponseRedirect("~/Public/Register1.aspx?")
        '            Else
        '                ProcessLogin(edipin)
        '            End If

        '        End If

        '    Catch ex As Exception
        '        LogManager.LogError(ex)
        '    Finally
        '        c = Nothing
        '    End Try

        '    GetPhoneNumbers()

        'End Sub

        Protected Function FileRequiresAuth(ByVal extension As String) As Boolean

            Select Case extension
                Case ".jpg", ".gif", ".png", ".axd", ".js", ".vbs", ".css", ".asmx"
                    Return False
            End Select

            Return True

        End Function

        Protected Sub GetPhoneNumbers()

            Dim KeyValueDAO = New NHibernateDaoFactory().GetKeyValDao

            Dim keys = KeyValueDAO.GetAllKeys()

            Dim phoneKey = (From k In keys Where k.Description.ToString().Equals("Phone Numbers")).FirstOrDefault

            Dim phoneValues = KeyValueDAO.GetKeyValuesByKeyId(phoneKey.Id)

            Dim DSN = (From k In phoneValues Where k.ValueDescription.ToString().Equals("Help Desk Phone Number DSN:")).FirstOrDefault
            Dim COMM = (From k In phoneValues Where k.ValueDescription.ToString().Equals("Help Desk Phone Number Comm:")).FirstOrDefault

            phoneNumbers.InnerHtml = DSN.ValueDescription + " " + DSN.Value + "<br />" + COMM.ValueDescription + " " + COMM.Value
        End Sub

        Protected Function IsValidRedirectUrl(ByVal url As String) As Boolean

            Dim request As HttpRequest = Page.Request
            Dim response As HttpResponse = Page.Response

            'allow access to non-protected files
            If (Not FileRequiresAuth(System.IO.Path.GetExtension(request.Path))) Then
                Return False
            End If

            'allow access to public folders
            If (url.ToLower.Contains("/default.aspx") OrElse
                url.ToLower.Contains("/clienttest.aspx") OrElse
                url.ToLower.Contains("/public/") OrElse
                url.ToLower.Contains("/login/login.aspx")) Then
                Return True
            End If

            Dim user As UserAuthentication = Nothing
            Dim cookieName As String = ConfigurationManager.AppSettings(AuthenticationHandler.AUTHCOOKIE_KEY)

            If (request.Cookies(cookieName) IsNot Nothing) Then

                Dim cookie As HttpCookie = request.Cookies(cookieName)

                If (cookie IsNot Nothing) Then
                    user = AuthProvider.Decrypt(cookie.Value)
                End If

            End If

            If (user Is Nothing) Then
                Return False
            End If

            If (url.IndexOf("?") > 0) Then
                url = url.Substring(0, url.IndexOf("?"))
            End If

            Dim rawUrl As String = Page.ResolveUrl(url)
            Dim node As SiteMapNode = SiteMap.Provider.FindSiteMapNode(rawUrl)

            If (node Is Nothing) Then
                'not found
                Return False
            End If

            Dim allowed As Boolean = False

            If (node.Roles.Count > 0) Then
                allowed = SiteMap.Provider.IsAccessibleToUser(HttpContext.Current, node)
            End If

            If (Not allowed) Then
                Return False
            End If

            Return True

        End Function

        Protected Sub ProcessLogin(ByVal edipin As String)

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
                Exit Sub
            End If

            'do we have a redirect url?
            '    Dim redir As String = FormsAuthentication.GetRedirectUrl(ssn, False)
            Dim redir As String = FormsAuthentication.GetRedirectUrl(edipin, False)

            If (IsValidRedirectUrl(redir)) Then
                SESSION_REDIRECT_URL = redir
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

            SafeResponseRedirect(url)

        End Sub

        Protected Sub RedirectToRegistration(ByVal edipin As String)

            'since this is not a registered user, remove the auth ticket.
            'doing this denies them access to /secure parts of the site
            FormsAuthentication.SignOut()

            SESSION_EDIPIN = edipin
            SafeResponseRedirect("~/Public/Register1.aspx?")

        End Sub

    End Class

End Namespace