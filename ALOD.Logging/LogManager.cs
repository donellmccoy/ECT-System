using Microsoft.Practices.EnterpriseLibrary.Data;
using System;
using System.Runtime.Remoting.Messaging;
using System.Web;

namespace ALOD.Logging
{
    public class LogManager
    {
        private static string BrowserName
        {
            get
            {
                if (IsInWebContext())
                {
                    return HttpContext.Current.Request.Browser.Browser.ToString();
                }

                return "";
            }
        }

        private static string BrowserVersion
        {
            get
            {
                if (IsInWebContext())
                {
                    return HttpContext.Current.Request.Browser.Version;
                }

                return "";
            }
        }

        private static string CallerIp
        {
            get
            {
                if (IsInWebContext())
                {
                    return HttpContext.Current.Request.UserHostAddress;
                }

                return "";
            }
        }

        private static string Platform
        {
            get
            {
                if (IsInWebContext())
                {
                    return HttpContext.Current.Request.Browser.Platform;
                }

                return "";
            }
        }

        private static string RequestUrl
        {
            get
            {
                if (IsInWebContext())
                {
                    return HttpContext.Current.Request.RawUrl;
                }

                return "";
            }
        }

        private static int UserId
        {
            get
            {
                string key = "UserId";
                int userId = 0;

                if (IsInWebSession())
                {
                    if (HttpContext.Current.Session[key] != null)
                    {
                        userId = (int)HttpContext.Current.Session[key];
                    }
                }
                else
                {
                    object id = CallContext.GetData(key);
                    if (id != null)
                    {
                        userId = (int)id;
                    }
                }

                return userId;
            }
        }

        private static string UserName
        {
            get
            {
                string key = "UserName";
                string userName = "Unknown";

                if (IsInWebSession())
                {
                    if (HttpContext.Current.Session[key] != null)
                    {
                        userName = (string)HttpContext.Current.Session[key];
                    }
                }
                else
                {
                    object name = CallContext.GetData(key);
                    if (name != null)
                    {
                        userName = (string)name;
                    }
                }

                return userName;
            }
        }

        public static System.Data.DataSet GetErrorById(int Id)
        {
            Database db = DatabaseFactory.CreateDatabase();
            return db.ExecuteDataSet("core_log_sp_GetErrorByID", Id);
        }

        public static System.Data.DataSet GetErrorLog()
        {
            Database db = DatabaseFactory.CreateDatabase();
            return db.ExecuteDataSet("core_log_sp_GetErrors");
        }

        public static int LogAction(int moduleId, UserAction actionId)
        {
            return LogAction(moduleId, actionId, 0, "", 0, 0);
        }

        public static int LogAction(int moduleId, UserAction actionId, string notes)
        {
            return LogAction(moduleId, actionId, 0, notes, 0, 0);
        }

        public static int LogAction(int moduleId, UserAction actionId, int referenceId)
        {
            return LogAction(moduleId, actionId, referenceId, "", 0, 0);
        }

        public static int LogAction(int moduleId, UserAction actionId, string notes, int status)
        {
            return LogAction(moduleId, actionId, 0, notes, status, 0);
        }

        public static int LogAction(int moduleId, UserAction actionId, int referenceId, int status)
        {
            return LogAction(moduleId, actionId, referenceId, "", status, 0);
        }

        public static int LogAction(int moduleId, UserAction actionId, int referenceId, string notes)
        {
            return LogAction(moduleId, actionId, referenceId, notes, 0, 0);
        }

        public static int LogAction(int moduleId, UserAction actionId, int referenceId, string notes, int status)
        {
            return LogAction(moduleId, actionId, referenceId, notes, status, 0);
        }

        public static int LogAction(int moduleId, UserAction actionId, int referenceId, string notes, int status, int statusOut)
        {
            try
            {
                Database db = DatabaseFactory.CreateDatabase();
                int logId = (int)db.ExecuteScalar("core_log_sp_RecordAction",
                    moduleId,
                    actionId,
                    UserId,
                    referenceId == 0 ? DBNull.Value : (object)referenceId,
                    notes,
                    status == 0 ? DBNull.Value : (object)status,
                    0,
                    statusOut == 0 ? DBNull.Value : (object)statusOut,
                    CallerIp
                    );

                return logId;
            }
            catch (Exception ex)
            {
                LogError(ex);
                return 0;
            }
        }

        public static int LogActionIndependant(byte moduleId, UserAction actionId, int idUser, string notes)
        {
            try
            {
                Database db = DatabaseFactory.CreateDatabase();
                int logId = (int)db.ExecuteScalar("core_log_sp_RecordAction",
                    moduleId,
                    actionId,
                    idUser,
                    idUser,
                    notes,
                    null,
                    0,
                    null,
                    CallerIp
                    );

                return logId;
            }
            catch (Exception ex)
            {
                LogError(ex);
                return 0;
            }
        }

        public static int LogActionPermission(int moduleId, UserAction actionId, int status)
        {
            return LogAction(moduleId, actionId, 0, "", status, 0);
        }

        public static void LogApplicationWarmupProcess(string processName, DateTime executionDate, string message)
        {
            try
            {
                Database db = DatabaseFactory.CreateDatabase();
                db.ExecuteNonQuery("ApplicationWarmupProcess_sp_InsertLog", processName, executionDate, message);
            }
            catch (Exception ex)
            {
                LogError(ex);
            }
        }

        public static void LogDebugMessage(string message)
        {
            try
            {
                Database db = DatabaseFactory.CreateDatabase();
                db.ExecuteNonQuery("Debug_sp_InsertLog", message);
            }
            catch (Exception ex)
            {
                LogError(ex);
            }
        }

        public static void LogError(string message)
        {
            LogError(message, "", "");
        }

        public static void LogError(Exception ex)
        {
            LogError(ex.Message, ex.StackTrace, "");
        }

        public static void LogError(Exception ex, string caller)
        {
            LogError(ex.Message, ex.StackTrace, caller);
        }

        public static void LogError(string message, string stackTrace, string caller)
        {
            try
            {
                Database db = DatabaseFactory.CreateDatabase();
                db.ExecuteNonQuery("core_log_sp_RecordError",
                    UserName,
                    RequestUrl,
                    System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString(),
                    BrowserName + " " + BrowserVersion + ", " + Platform,
                    message,
                    stackTrace,
                    caller,
                    CallerIp
                );
            }
            catch
            {
                //we're already in an error situation, nothing more we can do
            }
        }

        public static int LogGenerationTime(DateTime action_date, string measuredTime, string currentPage, string referringPage, string username, string role)
        {
            try
            {
                Database db = DatabaseFactory.CreateDatabase();
                int gId = (int)db.ExecuteScalar("core_log_sp_LogPageGenerationTime",
                             action_date,
                             measuredTime,
                             currentPage,
                             referringPage,
                             username,
                             role
                    );

                return gId;
            }
            catch (Exception ex)
            {
                LogError(ex);
                return 0;
            }
        }

        public static int LogMail(string eTo, string eCC, string eBCC, string subject, string body, string faliledrecipients)
        {
            try
            {
                Database db = DatabaseFactory.CreateDatabase();
                int eId = (int)db.ExecuteScalar("core_log_sp_RecordEmail",
                             UserId,
                             eTo,
                             eCC,
                             eBCC,
                             subject,
                             body,
                             faliledrecipients,
                             null
                    );

                return eId;
            }
            catch (Exception ex)
            {
                LogError(ex);
                return 0;
            }
        }

        public static int LogMail(string eTo, string eCC, string eBCC, string subject, string body, string faliledrecipients, int templateId, int user = 0)
        {
            try
            {
                Database db = DatabaseFactory.CreateDatabase();
                int eId = (int)db.ExecuteScalar("core_log_sp_RecordEmail",
                             user,
                             eTo,
                             eCC,
                             eBCC,
                             subject,
                             body,
                             faliledrecipients,
                             templateId
                    );

                return eId;
            }
            catch (Exception ex)
            {
                LogError(ex);
                return 0;
            }
        }

        private static bool IsInWebContext()
        {
            return HttpContext.Current != null;
        }

        private static bool IsInWebSession()
        {
            return IsInWebContext() && HttpContext.Current.Session != null;
        }
    }
}