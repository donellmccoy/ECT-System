using System;
using System.Configuration;
using System.Data;
using System.Net;
using System.Web;
using System.Web.Caching;
using System.Web.Optimization;
using System.Web.Security;
using ALOD.Core;
using ALOD.Core.Interfaces;
using ALOD.Core.Interfaces.DAOInterfaces;
using ALOD.Core.Utils;
using ALOD.Data;
using ALOD.Data.Services;
using ALODWebUtility.Common;
using WebSupergoo.ABCpdf12;

namespace ALOD
{
    public class Global : System.Web.HttpApplication
    {
        private const string EmailReminderCache = "EmailReminderCache";
        private const string AppWarmupCacheKey = "DailyAutomaticProcessesCache";

        void Application_Start(object sender, EventArgs e)
        {
            // Code that runs on application startup

            // Register script and style bundles for optimization
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            if (HttpContext.Current != null)
            {
                NHibernateSessionManager.Instance.GetSession();

                // Register Cache Entry That Will Send Reminder Emails When the Cache is removed
                RegisterCache();

                // Initialize ABCpdf if needed
                // if (!XSettings.InstallLicense(""))
                // {
                //     Response.Write("Could not install trial license. ");
                //     Response.Write("You have: " + XSettings.LicenseDescription);
                // }
            }
        }

        private bool RegisterCache()
        {
            if (HttpContext.Current.Cache[AppWarmupCacheKey] != null)
            {
                return false;
            }

            DateTime sendTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 0, 1, 0).AddDays(1);
            TimeSpan sendTimeFromNow = TimeSpan.FromTicks(sendTime.Ticks - DateTime.Now.Ticks);

            HttpContext.Current.Cache.Add(
                AppWarmupCacheKey,
                "ECTWarmupCache",
                null,
                sendTime,
                System.Web.Caching.Cache.NoSlidingExpiration,
                CacheItemPriority.NotRemovable,
                new CacheItemRemovedCallback(DailyAutomaticProcessesCacheItemRemovedCallback)
            );

            return true;
        }

        void DailyAutomaticProcessesCacheItemRemovedCallback(string key, object value, CacheItemRemovedReason reason)
        {
            if (reason == CacheItemRemovedReason.Expired)
            {
                // Hit a Dummy Page to reinsert the Cache
                HitPage();

                IApplicationWarmupProcessDao appWarmupDao = new NHibernateDaoFactory().GetApplicationWarmupProcessDao();
                string hostname = ConfigurationManager.AppSettings["Hostname"];
                var reminder = new ReminderEmailsService();

                // Call to send email reminders
                if (ConfigurationManager.AppSettings["ReminderEmailIsEnabled"] == "Y" && appWarmupDao.IsProcessActive("ReminderEmails") == true)
                {
                    reminder.SendEmailReminders(hostname);
                }

                if (appWarmupDao.IsProcessActive("InactiveAccounts") == true)
                {
                    reminder.DisableInactiveAccounts();
                }

                PsychologicalHealthService.ExecuteApplicationWarmupProcesses(DateTime.Now, hostname);
                ReportsService.ExecuteApplicationWarmupProcesses(DateTime.Now, hostname);
            }
        }

        void HitPage()
        {
            var client = new WebClient();
            client.DownloadData(ConfigurationManager.AppSettings["KeepAliveURL"]);
        }

        void Application_BeginRequest(object sender, EventArgs e)
        {
            // If the dummy page is hit, then it means we want to add another item in cache
            string requestUrl = HttpContext.Current.Request.Url.ToString().ToLower();
            string keepAliveUrl = ConfigurationManager.AppSettings["KeepAliveURL"];
            string keepAliveUrl2 = ConfigurationManager.AppSettings["KeepAliveURL2"];

            if (requestUrl == keepAliveUrl || requestUrl == keepAliveUrl2)
            {
                // Add the item in cache and when successful, do the work.
                RegisterCache();
                return;
            }
        }

        void Application_End(object sender, EventArgs e)
        {
            // Code that runs on application shutdown
        }

        void Application_Error(object sender, EventArgs e)
        {
            // Code that runs when an unhandled error occurs
            string errorDescription = "";
            string errMsg = Server.GetLastError().ToString();

            // Remove extra message up to "---->"
            int loc = errMsg.IndexOf("--->");

            if (loc >= 0)
            {
                errMsg = errMsg.Substring(loc + 4);
                loc = errMsg.IndexOf("--- End of inner exception stack trace ---");
                if (loc > 2)
                {
                    errMsg = errMsg.Substring(0, loc - 2);
                }
            }

            errorDescription += errMsg;
            errorDescription += Environment.NewLine + "=========================";
            errorDescription += Environment.NewLine + "URL=" + Request.Url.ToString();
            errorDescription += Environment.NewLine + "REFERER=" + Request.ServerVariables["HTTP_REFERER"];
            errorDescription += Environment.NewLine + "BROWSER=" + Request.ServerVariables["HTTP_USER_AGENT"];
            errorDescription += Environment.NewLine + "--------------------------------------------------";

            // Save passed values to aid reconstruction
            errorDescription += Environment.NewLine + ">> QUERY STRING";
            foreach (string key in Request.QueryString.AllKeys)
            {
                if (key != null)
                {
                    errorDescription += Environment.NewLine + key + "=" + Request.QueryString[key];
                }
            }

            errorDescription += Environment.NewLine + ">> FORM COLLECTION";
            foreach (string key in Request.Form.AllKeys)
            {
                if (key != null && !key.StartsWith("__"))
                {
                    errorDescription += Environment.NewLine + key + "=" + Request.Form[key];
                }
            }

            try
            {
                ALOD.Logging.LogManager.LogError(errorDescription);
            }
            catch (Exception)
            {
                // since we are the error handler, let's ignore this one
            }

            // since we encountered an error, rollback any pending database actions
            NHibernateSessionManager.Instance.RollbackTransaction();

            if (AppMode != DeployMode.Development)
            {
                if (Server.GetLastError() is HttpRequestValidationException)
                {
                    HandleRequestValidationException();
                }
                else
                {
                    Server.ClearError();
                    Response.Redirect("~/Public/AppError.htm");
                }
            }

            if (Server.GetLastError() is HttpRequestValidationException)
            {
                HandleRequestValidationException();
            }
        }

        private void HandleRequestValidationException()
        {
            Server.ClearError();
            if (!HttpContext.Current.Request.Url.ToString().ToLower().Contains("logout.aspx"))
            {
                Response.Redirect("~/Public/AppValidationError.htm");
            }
            else
            {
                Response.Redirect(HttpContext.Current.Request.Url.ToString());
            }
        }

        void Session_Start(object sender, EventArgs e)
        {
            // Code that runs when a new session is started
            if (Request.Cookies[FormsAuthentication.FormsCookieName] != null)
            {
                // session has timed out
                // Response.Redirect("~/Public/Logout.aspx");
            }
        }

        void Session_End(object sender, EventArgs e)
        {
            // Code that runs when a session ends. 
            // Note: The Session_End event is raised only when the sessionstate mode
            // is set to InProc in the Web.config file. If session mode is set to StateServer 
            // or SQLServer, the event is not raised.
        }
    }
}
