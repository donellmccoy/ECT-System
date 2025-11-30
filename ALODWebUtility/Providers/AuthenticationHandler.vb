Imports System.Configuration
Imports System.Text
Imports System.Threading
Imports System.Web
Imports ALOD.Core.Utils
Imports ALOD.Logging
Imports ALODWebUtility.Common

Namespace Providers

    Public Class AuthenticationHandler
        Implements IHttpModule

        Public Const AUTHCOOKIE_KEY As String = "authCookieName"

        Public Shared Sub Logout()

            'expire our perm cookie
            Dim cookieName As String = ConfigurationManager.AppSettings(AUTHCOOKIE_KEY)
            Dim cookie As HttpCookie = New HttpCookie(cookieName.ToUpper(), Nothing)

            If (AppMode = DeployMode.Production) Then
                cookie.Secure = True
            End If

            cookie.Expires = DateTime.Now.AddDays(-1)
            HttpContext.Current.Response.Cookies.Add(cookie)

        End Sub

        Public Sub Dispose() Implements System.Web.IHttpModule.Dispose

        End Sub

        Public Sub Init(ByVal context As System.Web.HttpApplication) Implements System.Web.IHttpModule.Init
            AddHandler context.AuthorizeRequest, AddressOf OnAuthorizeRequest
        End Sub

        Protected Function FileRequiresAuth(ByVal extension As String) As Boolean

            Select Case extension
                Case ".jpg", ".gif", ".png", ".axd", ".js", ".vbs", ".css", ".asmx"
                    Return False
            End Select

            Return True

        End Function

        Protected Sub OnAuthorizeRequest(ByVal sender As Object, ByVal e As EventArgs)

            Dim app As HttpApplication = CType(sender, HttpApplication)
            Dim request As HttpRequest = app.Request
            Dim response As HttpResponse = app.Response

            'allow access to non-protected files
            If (Not FileRequiresAuth(System.IO.Path.GetExtension(request.Path))) Then
                Exit Sub
            End If

            'allow access to public folders
            If (request.Url.AbsolutePath.ToLower.Contains("/default.aspx") OrElse
                request.Url.AbsolutePath.ToLower.Contains("/clienttest.aspx") OrElse
                request.Url.AbsolutePath.ToLower.Contains("/public/") OrElse
                request.Url.AbsolutePath.ToLower.Contains("/login/login.aspx")) Then
                Exit Sub
            End If

            Dim user As UserAuthentication = Nothing
            Dim cookieName As String = ConfigurationManager.AppSettings(AUTHCOOKIE_KEY)

            If (request.Cookies(cookieName) IsNot Nothing) Then

                Dim cookie As HttpCookie = request.Cookies(cookieName)

                If (cookie IsNot Nothing) Then
                    user = AuthProvider.Decrypt(cookie.Value)

                    If (user Is Nothing) Then
                        Throw New Exception("GARY: user Is Nothing")
                    End If

                    If (user.Permissions Is Nothing) Then
                        Throw New Exception("GARY: user.Permissions Is Nothing")
                    End If

                    Dim principle As New LodPrinciple(user, user.Permissions)
                    app.Context.User = principle
                    Thread.CurrentPrincipal = principle
                End If

            End If

            If (user Is Nothing) Then
                RedirectToAccessDeniedPage("Unknown", request, "NULL user object")
                Exit Sub
            End If

            If (Not user.IsAuthenticated) Then
                RedirectToAccessDeniedPage(user.UserName, request, "User not authenticated")
                Exit Sub
            End If

            Dim node As SiteMapNode = SiteMap.Provider.FindSiteMapNode(HttpContext.Current)

            If (node Is Nothing) Then
                'not found
                RedirectToAccessDeniedPage(user.UserName, request, "NULL SiteMapNode")
                Exit Sub
            End If

            Dim allowed As Boolean = False

            If (node.Roles.Count > 0) Then
                allowed = SiteMap.Provider.IsAccessibleToUser(HttpContext.Current, node)
            End If

            If (Not allowed) Then
                RedirectToAccessDeniedPage(user.UserName, request, "Not allowed")
            End If

        End Sub

        Private Sub RedirectToAccessDeniedPage(ByVal userName As String, ByVal request As HttpRequest, ByVal reason As String)

            Dim msg As New StringBuilder()
            msg.Append("Access Denied" + System.Environment.NewLine)
            msg.Append("User: " + userName + System.Environment.NewLine)
            msg.Append("Request: " + request.Url.ToString() + System.Environment.NewLine)

            If (request.UrlReferrer IsNot Nothing) Then
                msg.Append("Referrer: " + request.UrlReferrer.ToString() + System.Environment.NewLine)
            End If

            msg.Append("Reason: " + reason)

            LogManager.LogError(msg.ToString())

            Dim ctrl As New System.Web.UI.Control
            Dim url As String = ctrl.ResolveUrl(ConfigurationManager.AppSettings("AccessDeniedUrl"))
            'HttpContext.Current.Response.Redirect(url, True)
            HttpContext.Current.Response.Redirect(url, False)
            HttpContext.Current.ApplicationInstance.CompleteRequest()
        End Sub

    End Class

End Namespace