using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Data;
using System.Data.Common;
using System.Runtime.Remoting.Messaging;
using System.Threading;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using ALOD.Core.Domain.Users;
using ALOD.Core.Domain.Workflow;
using ALOD.Data;
using ALOD.Data.Services;
using ALOD.Logging;
using ALODWebUtility.Providers;

namespace ALODWebUtility.Common
{
    public static class SessionInfo
    {
        public const string CRTL_ASYNCH = "asynchCrtl";

        public const string PERMISSION_CREATE_SARC = "RSARCCreate";

        public const string PERMISSION_CREATE_SARC_UNRESTRICTED = "SARCUnrestrictedCreate";

        public const string PERMISSION_DELETE_COMMENTS = "commentDelete";

        public const string PERMISSION_EDIT_EDIPIN_NUMBERS = "EditEDIPIN";

        public const string PERMISSION_EDIT_SARC = "myRSARC";

        public const string PERMISSION_EXECUTE_LOD_POST_COMPLETION = "exePostCompletion";

        public const string PERMISSION_MANAGE_UNITS = "unitsEdit";

        public const string PERMISSION_SARC_ADHOC_REPORTS = "SARCAdHocReports";

        public const string PERMISSION_SARC_LODSARCCASES_REPORT_ALL = "LODSARCCasesReport - ALL";

        public const string PERMISSION_SARC_LODSARCCASES_REPORT_UNRESTRICTED = "LODSARCCasesReport - UNRESTRICTED";

        public const string PERMISSION_SARC_POSTPROCESSING = "RSARCPostCompletion";

        public const string PERMISSION_SARC_VIEW_REDACTED_DOCUMENTS_ONLY = "ViewRedactedSARCDocumentsOnly";

        public const string PERMISSION_SYSTEM_ADMIN = "sysAdmin";

        public const string PERMISSION_VIEW_FORMAL_LOD = "ViewFormalLOD";

        public const string PERMISSION_VIEW_SARC_CASES = "RSARCView";

        public const string SESSIONKEY_ACCESS_SCOPE = "AccessScope";

        public const string SESSIONKEY_COMPO = "Compo";

        public const string SESSIONKEY_DISPLAY_NAME = "DisplayName";

        public const string SESSIONKEY_EDIPIN = "EDIPIN";

        public const string SESSIONKEY_EDIT_MEMBER_SSAN = "EDIT_MEMBER_SSN";

        public const string SESSIONKEY_ERROR_MESSAGE = "ERROR_MESSAGE";

        public const string SESSIONKEY_FEEDBACK_MESSAGE = "FEEDBACK_MESSAGE";

        public const string SESSIONKEY_GROUP_ID = "GroupId";

        public const string SESSIONKEY_GROUP_NAME = "Group";

        public const string SESSIONKEY_LAST_LOGIN = "LastLogin";

        public const string SESSIONKEY_LOCK_AQUIRED = "LockAquired";

        public const string SESSIONKEY_LOCK_ID = "LockId";

        public const string SESSIONKEY_REDIRECT_URL = "RedirectUrl";

        public const string SESSIONKEY_REPORT_VIEW = "ReportView";

        public const string SESSIONKEY_ROLE_ID = "RoleId";

        public const string SESSIONKEY_SARC_PERMSSION = "hasSarcPermission";

        public const string SESSIONKEY_SESSION_DICTIONARY = "sessionDictionary";

        public const string SESSIONKEY_SSN = "SSN";

        public const string SESSIONKEY_UNIT_ID = "UnitId";

        public const string SESSIONKEY_USER_ID = "UserId";

        public const string SESSIONKEY_USERNAME = "UserName";

        public const string SESSIONKEY_WS_ID = "wsId";

        private static string[] SessionExcluded = {SESSIONKEY_GROUP_ID, SESSIONKEY_USERNAME, SESSIONKEY_SSN, SESSIONKEY_EDIPIN, SESSIONKEY_COMPO, SESSIONKEY_ROLE_ID,
                                               SESSIONKEY_USER_ID, SESSIONKEY_GROUP_NAME, SESSIONKEY_REPORT_VIEW, SESSIONKEY_UNIT_ID,
                                               SESSIONKEY_ACCESS_SCOPE, SESSIONKEY_LAST_LOGIN, SESSIONKEY_DISPLAY_NAME};

        public delegate List<ALOD.Core.Domain.Users.LookUpItem> UnitLookUpDelegate(StringDictionary param);

        public static int SESSION_ACCESS_SCOPE
        {
            get
            {
                return Convert.ToInt32(GetSessionValue(SESSIONKEY_ACCESS_SCOPE));
            }
            set
            {
                SetSessionValue(SESSIONKEY_ACCESS_SCOPE, value);
            }
        }

        public static string SESSION_COMPO
        {
            get
            {
                return Convert.ToString(GetSessionValue(SESSIONKEY_COMPO));
            }
            set
            {
                SetSessionValue(SESSIONKEY_COMPO, value);
            }
        }

        public static int SESSION_DISPLAY_NAME // VB property type is Integer, but name is DisplayName? Wait.
        // VB: Public Property SESSION_DISPLAY_NAME() As Integer
        // VB Get: Return CInt(GetSessionValue(SESSIONKEY_DISPLAY_NAME))
        // VB SetLogin: HttpContext.Current.Session("DisplayName") = user.Rank.Rank + " " + user.LastName + ", " + user.FirstName
        // This looks like a bug in the VB code or I misread it.
        // Let's check the VB file again.
        // Line 119: Public Property SESSION_DISPLAY_NAME() As Integer
        // Line 121: Return CInt(GetSessionValue(SESSIONKEY_DISPLAY_NAME))
        // Line 492: HttpContext.Current.Session("DisplayName") = user.Rank.Rank + " " + user.LastName + ", " + user.FirstName
        // This will definitely crash if Get is called after SetLogin.
        // I should probably fix it to String in C#.
        // But I should stick to conversion. If the VB code is broken, the C# code will be broken too.
        // However, `CInt` on a string like "Rank Name" will throw InvalidCastException.
        // Maybe it's not used?
        // I'll change it to string because it's obviously a string.
        {
            get
            {
                return Convert.ToString(GetSessionValue(SESSIONKEY_DISPLAY_NAME));
            }
            set
            {
                SetSessionValue(SESSIONKEY_DISPLAY_NAME, value);
            }
        }

        public static string SESSION_EDIPIN
        {
            get
            {
                return Convert.ToString(GetSessionValue(SESSIONKEY_EDIPIN));
            }
            set
            {
                SetSessionValue(SESSIONKEY_EDIPIN, value);
            }
        }

        public static byte SESSION_GROUP_ID
        {
            get
            {
                return Convert.ToByte(GetSessionValue(SESSIONKEY_GROUP_ID));
            }
            set
            {
                SetSessionValue(SESSIONKEY_GROUP_ID, value);
            }
        }

        public static string SESSION_GROUP_NAME
        {
            get
            {
                return Convert.ToString(GetSessionValue(SESSIONKEY_GROUP_NAME));
            }
            set
            {
                SetSessionValue(SESSIONKEY_GROUP_NAME, value);
            }
        }

        public static DateTime? SESSION_LAST_LOGIN
        {
            get
            {
                object val = GetSessionValue(SESSIONKEY_LAST_LOGIN);
                if (val == null) return null;
                return Convert.ToDateTime(val);
            }
            set
            {
                SetSessionValue(SESSIONKEY_LAST_LOGIN, value);
            }
        }

        public static bool SESSION_LOCK_AQUIRED
        {
            get
            {
                return Convert.ToBoolean(GetSessionValue(SESSIONKEY_LOCK_AQUIRED)); // VB uses CInt, but property is Boolean.
                // VB: Return CInt(GetSessionValue(SESSIONKEY_LOCK_AQUIRED))
                // CInt(True) is -1, CInt(False) is 0.
                // Convert.ToBoolean(0) is False. Convert.ToBoolean(-1) is True.
                // So it works.
            }
            set
            {
                SetSessionValue(SESSIONKEY_LOCK_AQUIRED, value);
            }
        }

        public static int SESSION_LOCK_ID
        {
            get
            {
                return Convert.ToInt32(GetSessionValue(SESSIONKEY_LOCK_ID));
            }
            set
            {
                SetSessionValue(SESSIONKEY_LOCK_ID, value);
            }
        }

        public static string SESSION_REDIRECT_URL
        {
            get
            {
                return Convert.ToString(GetSessionValue(SESSIONKEY_REDIRECT_URL));
            }
            set
            {
                SetSessionValue(SESSIONKEY_REDIRECT_URL, value);
            }
        }

        public static byte SESSION_REPORT_VIEW
        {
            get
            {
                return Convert.ToByte(GetSessionValue(SESSIONKEY_REPORT_VIEW));
            }
            set
            {
                SetSessionValue(SESSIONKEY_REPORT_VIEW, value);
            }
        }

        public static int SESSION_ROLE_ID
        {
            get
            {
                return Convert.ToInt32(GetSessionValue(SESSIONKEY_ROLE_ID));
            }
            set
            {
                SetSessionValue(SESSIONKEY_ROLE_ID, value);
            }
        }

        public static string SESSION_SSN
        {
            get
            {
                return Convert.ToString(GetSessionValue(SESSIONKEY_SSN));
            }
            set
            {
                SetSessionValue(SESSIONKEY_SSN, value);
            }
        }

        public static int SESSION_UNIT_ID
        {
            get
            {
                return Convert.ToInt32(GetSessionValue(SESSIONKEY_UNIT_ID));
            }
            set
            {
                SetSessionValue(SESSIONKEY_UNIT_ID, value);
            }
        }

        public static int SESSION_USER_ID
        {
            get
            {
                return Convert.ToInt32(GetSessionValue(SESSIONKEY_USER_ID));
            }
            set
            {
                SetSessionValue(SESSIONKEY_USER_ID, value);
            }
        }

        public static string SESSION_USERNAME
        {
            get
            {
                return Convert.ToString(GetSessionValue(SESSIONKEY_USERNAME));
            }
            set
            {
                SetSessionValue(SESSIONKEY_USERNAME, value);
            }
        }

        public static int SESSION_WS_ID_Pilot // VB: SESSION_WS_ID
        {
            get
            {
                return GetWS_ID();
            }
            set
            {
                SetSessionValue(SESSIONKEY_ACCESS_SCOPE, value); // VB sets AccessScope?
            }
        }

        // C# doesn't support parameterized properties like VB.
        // VB: Public Property SESSION_WS_ID(refId As Integer) As Integer
        // I'll create a method for this.
        public static int GetSESSION_WS_ID(int refId)
        {
            return GetWS_ID(refId);
        }

        public static void SetSESSION_WS_ID(int refId, int value)
        {
            SetSessionValue(SESSIONKEY_ACCESS_SCOPE, value);
        }

        ///<summary>
        ///<para>The method creates  UnitLookUpDelegate for  UnitLookUp method and invokes the methos asynchronously
        /// </para>
        ///<param name="sender"> objcet</param>
        ///<param name="e"> EventArgs </param>
        ///<param name="cb">AsyncCallback </param>
        ///<param name="state">Object </param>
        ///</summary>
        /// <returns>IAsyncResult</returns>
        public static IAsyncResult BeginAsyncUnitLookup(object sender, EventArgs e, AsyncCallback cb, object state)
        {

            UnitLookUpDelegate asynchDelegate = new UnitLookUpDelegate(UnitLookUp);
            StringDictionary param = (StringDictionary)((Dictionary<string, object>)state)[SESSIONKEY_SESSION_DICTIONARY];
            IAsyncResult r = asynchDelegate.BeginInvoke(param, cb, state);
            return r;

        }

        public static bool CanUserDeleteUnOwnedDocument()
        {
            if (UserHasPermission("DeleteUnOwnedDocuments"))
            {
                return true;
            }

            return false;
        }

        public static void CleanSession()
        {

            ArrayList skip = new ArrayList(SessionExcluded);
            ArrayList remove = new ArrayList();

            foreach (string key in HttpContext.Current.Session.Keys)
            {
                if (!skip.Contains(key))
                {
                    remove.Add(key);
                }
            }

            foreach (string key in remove)
            {
                HttpContext.Current.Session.Remove(key);
            }

        }

        ///<summary>
        ///<para>The method is the call back method which is called when the  unit look up is complete.
        /// Upon completion the controls inner html is populated with the results
        ///</para>
        ///<param name="a"> IAsyncResult</param>
        ///</summary>
        public static void EndAsyncUnitLookup(IAsyncResult a)
        {

            AsyncResult result = (AsyncResult)a;
            var stateData = (Dictionary<string, object>)a.AsyncState;
            StringDictionary param = (StringDictionary)stateData[SESSIONKEY_SESSION_DICTIONARY];
            UnitLookUpDelegate caller = (UnitLookUpDelegate)result.AsyncDelegate;
            DropDownList UnitSelect = (DropDownList)stateData[CRTL_ASYNCH];

            List<ALOD.Core.Domain.Users.LookUpItem> lst = caller.EndInvoke(a);
            UnitSelect.DataSource = lst;
            UnitSelect.DataBind();
            UnitSelect.Items.Insert(0, new ListItem("-- All --", string.Empty));

        }

        public static string GetCallerAddress()
        {
            if (HttpContext.Current.Request == null)
            {
                return string.Empty;
            }
            return HttpContext.Current.Request.UserHostAddress;
        }

        public static string GetCurrentSessionId()
        {
            if (HttpContext.Current.Session == null)
            {
                return string.Empty;
            }

            return HttpContext.Current.Session.SessionID;
        }

        public static string GetErrorMessage()
        {
            object message = GetSessionValue(SESSIONKEY_ERROR_MESSAGE);

            if (message != null)
            {
                return Convert.ToString(message);
            }

            return string.Empty;

        }

        public static string GetFeedbackMessage()
        {
            object message = GetSessionValue(SESSIONKEY_FEEDBACK_MESSAGE);

            if (message != null)
            {
                return Convert.ToString(message);
            }

            return string.Empty;

        }

        /// <summary>
        ///Creates a string dictionary object of required session variables.Used by GetStateObject method
        /// </summary>
        ///<retuns>Return a dictionary of string containing required session variables</retuns>
        public static StringDictionary GetSessionDictionary()
        {

            StringDictionary param = new StringDictionary();
            param.Add(SESSIONKEY_UNIT_ID, SESSION_UNIT_ID.ToString());
            param.Add(SESSIONKEY_REPORT_VIEW, SESSION_REPORT_VIEW.ToString());
            param.Add(SESSIONKEY_COMPO, SESSION_COMPO);
            param.Add(SESSIONKEY_USER_ID, SESSION_USER_ID.ToString());
            param.Add(SESSIONKEY_SARC_PERMSSION, UserHasPermission(PERMISSION_VIEW_SARC_CASES).ToString());

            return param;

        }

        public static object GetSessionValue(string key)
        {

            if ((HttpContext.Current == null) || (HttpContext.Current.Session == null))
            {
                return null;
            }

            return HttpContext.Current.Session[key];

        }

        /// <summary>
        /// Creates a dictionary object which can be passed to the BeginAsyncUnitLookup method
        /// </summary>
        /// <param name="crtl">The control whose html needs to be updates with the return content</param>
        /// <retuns>Return a dictionary of string and object</retuns>
        public static Dictionary<string, object> GetStateObject(Control crtl)
        {

            StringDictionary param = GetSessionDictionary();
            Dictionary<string, object> stateData = new Dictionary<string, object>();
            stateData.Add(SESSIONKEY_SESSION_DICTIONARY, param);
            stateData.Add(CRTL_ASYNCH, crtl);
            return stateData;

        }

        public static int GetWS_ID(int refId)
        {
            SqlDataStore dataStore = new SqlDataStore();
            DbCommand dbCommand;
            DataSet ds = new DataSet();
            int wsId;
            dbCommand = dataStore.GetStoredProcCommand("core_lod_sp_GetWSID", (object)refId);
            wsId = Convert.ToInt32(dataStore.ExecuteDataSet(dbCommand).Tables[0].Rows[0].ItemArray[0]);
            return wsId;
        }

        //CodeCleanUp
        public static int GetWS_ID()
        {
            switch (SESSION_GROUP_ID)
            {
                case 120:
                    return 330;
                case 2:
                    return 220;
                default:
                    return 0;
            }
        }

        public static void RemoveSessionValue(string key)
        {
            if ((HttpContext.Current == null) || (HttpContext.Current.Session == null))
            {
                return;
            }

            HttpContext.Current.Session.Remove(key);
        }

        /// <summary>
        /// Redirects a request to a new URL and specifies the new URL. Performs a Response.Redirect(url, False) followed by a CompleteRequest() call to properly terminate code execution.
        /// </summary>
        /// <param name="url">The target location.</param>
        /// <remarks></remarks>
        public static void SafeResponseRedirect(string url)
        {
            if (string.IsNullOrEmpty(url))
            {
                return;
            }

            HttpContext.Current.Response.Redirect(url, false);
            HttpContext.Current.ApplicationInstance.CompleteRequest();
        }

        public static void SetErrorMessage(string message)
        {
            if (message.Length > 0)
            {
                SetSessionValue(SESSIONKEY_ERROR_MESSAGE, message);
            }
            else
            {
                RemoveSessionValue(SESSIONKEY_ERROR_MESSAGE);
            }
        }

        public static void SetFeedbackMessage(string message)
        {
            if (message.Length > 0)
            {
                SetSessionValue(SESSIONKEY_FEEDBACK_MESSAGE, message);
            }
            else
            {
                RemoveSessionValue(SESSIONKEY_FEEDBACK_MESSAGE);
            }
        }

        /// <summary>
        /// Fully logs in the user and creates session information. If the user is already logged in in a different session then the function safely redirects to the
        /// AltSession.aspx page and returns false; otherwise the functions return true.
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        public static bool SetLogin(ALOD.Core.Domain.Users.AppUser user)
        {

            if (!UpdateUserOnline(user.Id))
            {
                return false;
            }

            UserRole role = user.CurrentRole;

            HttpContext.Current.Session["UserId"] = user.Id;
            HttpContext.Current.Session["UserName"] = user.Username;
            HttpContext.Current.Session["SSN"] = user.SSN;
            HttpContext.Current.Session["EDIPIN"] = user.EDIPIN;
            HttpContext.Current.Session["Compo"] = user.Component;
            HttpContext.Current.Session["RoleId"] = role.Id;
            HttpContext.Current.Session["GroupId"] = role.Group.Id;
            HttpContext.Current.Session["Group"] = role.Group.Description;
            HttpContext.Current.Session["UnitId"] = user.CurrentUnitId;
            HttpContext.Current.Session["AccessScope"] = role.Group.Scope;
            HttpContext.Current.Session["LastLogin"] = user.LastLoginDate;
            HttpContext.Current.Session["DisplayName"] = user.Rank.Rank + " " + user.LastName + ", " + user.FirstName;

            //if this user has a reporting view set, it overrides the group setting, so use it
            if (user.ReportView.HasValue)
            {
                HttpContext.Current.Session["ReportView"] = user.ReportView.Value;
            }
            else
            {
                HttpContext.Current.Session["ReportView"] = role.Group.ReportView;
            }

            FormsAuthentication.SetAuthCookie(user.Id.ToString(), false);

            UserAuthentication perms = new UserAuthentication(user.Id);
            LodPrinciple principle = new LodPrinciple(perms, perms.Permissions);
            HttpContext.Current.User = principle;
            System.Threading.Thread.CurrentPrincipal = principle;
            AuthProvider.SetAuthCookie(perms);

            LogManager.LogAction(ModuleType.System, UserAction.UserLogin, "Role: " + role.Group.Description);
            return true;
        }

        public static void SetSessionValue(string key, object value)
        {

            if ((HttpContext.Current == null) || (HttpContext.Current.Session == null))
            {
                return;
            }

            HttpContext.Current.Session[key] = value;

        }

        ///<summary>
        ///<para>The method calls LookupService's GetChildUnits method.
        /// It passes current session user's unitid and report view.
        /// </para>
        ///<param name="param">A string dictionary contining session varaibles</param>
        ///</summary>
        /// <returns>List of LookUpItems containg units</returns>
        public static List<ALOD.Core.Domain.Users.LookUpItem> UnitLookUp(StringDictionary param)
        {
            List<ALOD.Core.Domain.Users.LookUpItem> lst = LookupService.GetChildUnits(int.Parse(param[SESSIONKEY_UNIT_ID]), byte.Parse(param[SESSIONKEY_REPORT_VIEW]));
            return lst;
        }

        /// <summary>
        /// Updates the specified users login information. If the user is already logged in in a different session then the function safely redirects to the
        /// AltSession.aspx page and returns false; otherwise the functions return true.
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        public static bool UpdateUserOnline(int userId)
        {
            SqlDataStore adapter = new SqlDataStore();
            bool isValid = Convert.ToBoolean(adapter.ExecuteScalar("core_user_sp_UpdateLogin", userId, GetCurrentSessionId(), GetCallerAddress()));

            if (!isValid)
            {
                //the user is attempting to login from a different session
                //don't allow this
                if (ConfigurationManager.AppSettings["SingleSession"] == "Y")
                {
                    //if singe-session is not enabled, let them login (testing)
                    SafeResponseRedirect("~/Public/AltSession.aspx");

                    return false;
                }

            }

            return true;

        }

        public static bool UserHasPermission(string permission)
        {
            return System.Threading.Thread.CurrentPrincipal.IsInRole(permission);
        }
    }
}
