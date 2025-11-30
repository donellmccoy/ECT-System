using System;
using System.Web;

namespace ALODWebUtility.Common
{
    /// <summary>
    /// HTTP Module to enforce UTF-8 charset on JavaScript resources served by ASP.NET handlers
    /// </summary>
    public class CharsetEnforcementModule : IHttpModule
    {
        public void Init(HttpApplication context)
        {
            context.PreSendRequestHeaders += OnPreSendRequestHeaders;
        }

        private void OnPreSendRequestHeaders(object sender, EventArgs e)
        {
            HttpApplication app = sender as HttpApplication;
            if (app != null && app.Context != null)
            {
                HttpResponse response = app.Context.Response;
                HttpRequest request = app.Context.Request;

                // Check if this is a WebResource.axd or ScriptResource.axd request
                if (request.Path.EndsWith("WebResource.axd", StringComparison.OrdinalIgnoreCase) ||
                   request.Path.EndsWith("ScriptResource.axd", StringComparison.OrdinalIgnoreCase))
                {
                    // Get the current Content-Type
                    string contentType = response.ContentType;

                    // If it's JavaScript and doesn't have charset, add it
                    if (!string.IsNullOrEmpty(contentType))
                    {
                        if ((contentType.Contains("javascript") || contentType.Contains("ecmascript")) &&
                           !contentType.Contains("charset"))
                        {
                            response.ContentType = contentType + "; charset=utf-8";
                        }
                    }
                }
            }
        }

        public void Dispose()
        {
            // Nothing to dispose
        }
    }
}
