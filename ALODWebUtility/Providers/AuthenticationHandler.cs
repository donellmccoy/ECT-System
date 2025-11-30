using System;
using System.Configuration;
using System.Text;
using System.Threading;
using System.Web;
using ALOD.Core.Utils;
using ALOD.Logging;
using ALODWebUtility.Common;

namespace ALODWebUtility.Providers
{
    public class AuthenticationHandler : IHttpModule
    {
        public const string AUTHCOOKIE_KEY = "authCookieName";

        public static void Logout()
        {
            // expire our perm cookie
            string cookieName = ConfigurationManager.AppSettings[AUTHCOOKIE_KEY];
            HttpCookie cookie = new HttpCookie(cookieName.ToUpper(), null);

            if (Utility.AppMode == DeployMode.Production)
            {
                cookie.Secure = true;
            }

            cookie.Expires = DateTime.Now.AddDays(-1);
            HttpContext.Current.Response.Cookies.Add(cookie);
        }

        public void Dispose()
        {
        }

        public void Init(HttpApplication context)
        {
            context.AuthorizeRequest += OnAuthorizeRequest;
        }

        protected bool FileRequiresAuth(string extension)
        {
            switch (extension)
            {
                case ".jpg":
                case ".gif":
                case ".png":
                case ".axd":
                case ".js":
                case ".vbs":
                case ".css":
                case ".asmx":
                    return false;
            }

            return true;
        }

        protected void OnAuthorizeRequest(object sender, EventArgs e)
        {
            HttpApplication app = (HttpApplication)sender;
            HttpRequest request = app.Request;
            HttpResponse response = app.Response;

            // allow access to non-protected files
            if (!FileRequiresAuth(System.IO.Path.GetExtension(request.Path)))
            {
                return;
            }

            // allow access to public folders
            if (request.Url.AbsolutePath.ToLower().Contains("/default.aspx") ||
                request.Url.AbsolutePath.ToLower().Contains("/clienttest.aspx") ||
                request.Url.AbsolutePath.ToLower().Contains("/public/") ||
                request.Url.AbsolutePath.ToLower().Contains("/login/login.aspx"))
            {
                return;
            }

            UserAuthentication user = null;
            string cookieName = ConfigurationManager.AppSettings[AUTHCOOKIE_KEY];

            if (request.Cookies[cookieName] != null)
            {
                HttpCookie cookie = request.Cookies[cookieName];

                if (cookie != null)
                {
                    user = AuthProvider.Decrypt(cookie.Value);

                    if (user == null)
                    {
                        throw new Exception("GARY: user Is Nothing");
                    }

                    if (user.Permissions == null)
                    {
                        throw new Exception("GARY: user.Permissions Is Nothing");
                    }

                    LodPrinciple principle = new LodPrinciple(user, user.Permissions);
                    app.Context.User = principle;
                    Thread.CurrentPrincipal = principle;
                }
            }

            if (user == null)
            {
                RedirectToAccessDeniedPage("Unknown", request, "NULL user object");
                return;
            }

            if (!user.IsAuthenticated)
            {
                RedirectToAccessDeniedPage(user.UserName, request, "User not authenticated");
                return;
            }

            SiteMapNode node = SiteMap.Provider.FindSiteMapNode(HttpContext.Current);

            if (node == null)
            {
                // not found
                RedirectToAccessDeniedPage(user.UserName, request, "NULL SiteMapNode");
                return;
            }

            bool allowed = false;

            if (node.Roles.Count > 0)
            {
                allowed = SiteMap.Provider.IsAccessibleToUser(HttpContext.Current, node);
            }

            if (!allowed)
            {
                RedirectToAccessDeniedPage(user.UserName, request, "Not allowed");
            }
        }

        private void RedirectToAccessDeniedPage(string userName, HttpRequest request, string reason)
        {
            StringBuilder msg = new StringBuilder();
            msg.Append("Access Denied" + System.Environment.NewLine);
            msg.Append("User: " + userName + System.Environment.NewLine);
            msg.Append("Request: " + request.Url.ToString() + System.Environment.NewLine);

            if (request.UrlReferrer != null)
            {
                msg.Append("Referrer: " + request.UrlReferrer.ToString() + System.Environment.NewLine);
            }

            msg.Append("Reason: " + reason);

            LogManager.LogError(msg.ToString());

            System.Web.UI.Control ctrl = new System.Web.UI.Control();
            string url = ctrl.ResolveUrl(ConfigurationManager.AppSettings["AccessDeniedUrl"]);
            // HttpContext.Current.Response.Redirect(url, True)
            HttpContext.Current.Response.Redirect(url, false);
            HttpContext.Current.ApplicationInstance.CompleteRequest();
        }
    }
}
