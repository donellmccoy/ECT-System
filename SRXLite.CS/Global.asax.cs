using System;
using System.Web;
using System.Web.Optimization;
using SRXLite.App_Start;
using SRXLite.Modules;

namespace SRXLite
{
    public class Global : HttpApplication
    {
        void Application_Start(object sender, EventArgs e)
        {
            // Code that runs on application startup
            
            // Register script and style bundles for optimization
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }
        
        void Application_End(object sender, EventArgs e)
        {
            // Code that runs on application shutdown
        }
            
        void Application_Error(object sender, EventArgs e)
        {
            // Code that runs when an unhandled error occurs
            int errorID = SRXLite.Modules.ExceptionHandling.LogError(
                SRXLite.Modules.ExceptionHandling.FormatLastError());
            
            // Error will be cleared on the redirect page
            // Redirect set in web.config customErrors section
            if (HttpContext.Current != null && HttpContext.Current.Server.GetLastError() != null)
            {
                HttpContext.Current.Server.GetLastError().Data.Add("ErrorID", errorID);
            }
        }

        void Session_Start(object sender, EventArgs e)
        {
            // Code that runs when a new session is started
        }
    }
}
