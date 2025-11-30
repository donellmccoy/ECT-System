using System;
using System.Web;
using ALOD.Core.Utils;

namespace ALODWebUtility.Common
{
    public class UserPreferences
    {
        public static void DeleteSetting(string key)
        {
            HttpRequest request = HttpContext.Current.Request;

            HttpCookie cookie;

            if (request.Cookies["userPrefs"] == null)
            {
                cookie = new HttpCookie("userPrefs");
            }
            else
            {
                cookie = request.Cookies["userPrefs"];
            }

            cookie[key] = "";

            if (Utility.AppMode == DeployMode.Production)
            {
                cookie.Secure = true;
            }

            HttpContext.Current.Response.Cookies.Add(cookie);
        }

        public static string GetSetting(string key, string defaultValue = "")
        {
            HttpRequest request = HttpContext.Current.Request;

            if (request.Cookies["userPrefs"] != null)
            {
                if (request.Cookies["userPrefs"][key] != null)
                {
                    return request.Cookies["userPrefs"][key];
                }
            }

            return defaultValue;
        }

        public static void SaveSetting(string key, string value)
        {
            HttpRequest request = HttpContext.Current.Request;

            HttpCookie cookie;

            if (request.Cookies["userPrefs"] == null)
            {
                cookie = new HttpCookie("userPrefs");
            }
            else
            {
                cookie = request.Cookies["userPrefs"];
            }

            cookie.HttpOnly = true;
            cookie.Expires = DateTime.Now.AddYears(1);

            if (Utility.AppMode == DeployMode.Production)
            {
                cookie.Secure = true;
            }

            cookie[key] = value;

            HttpContext.Current.Response.Cookies.Add(cookie);
        }
    }
}
