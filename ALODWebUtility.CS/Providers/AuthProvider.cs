using System;
using System.Configuration;
using System.Text;
using System.Web;
using ALOD.Core.Utils;
using ALODWebUtility.Common;

namespace ALODWebUtility.Providers
{
    public class AuthProvider
    {
        const string _delimiter = "|";
        const short _paramCount = 3;
        const string AUTHCOOKIE_KEY = "authCookieName";
        const string LOGINURL_KEY = "loginUrl";

        /// <summary>
        /// Given an encrypted string from the authentication cookie returns a User object
        /// </summary>
        /// <param name="encrypted">The encrypted cookie value</param>
        /// <returns></returns>
        /// <remarks></remarks>
        public static UserAuthentication Decrypt(string encrypted)
        {
            UserAuthentication user = null;

            try
            {
                string decryptedString = LodCrypto.Decrypt(encrypted);
                string[] parts = decryptedString.Split(new string[] { _delimiter }, StringSplitOptions.None);

                if (parts.Length == _paramCount)
                {
                    bool authed = false;
                    bool.TryParse(parts[2], out authed);

                    user = new UserAuthentication(parts[0], parts[1], authed);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return user;
        }

        /// <summary>
        /// Encrypts a User object to a string for storing in a cookie
        /// </summary>
        /// <param name="user">The User object to store</param>
        /// <returns>The encrypted string</returns>
        /// <remarks>This stores the user name and permissions for the current user</remarks>
        public static string Encrypt(UserAuthentication user)
        {
            string encryptedString = string.Empty;

            try
            {
                StringBuilder buffer = new StringBuilder();

                buffer.Append(user.UserName + _delimiter);
                buffer.Append(user.Roles + _delimiter);
                buffer.Append(user.IsAuthenticated.ToString());

                encryptedString = LodCrypto.Encrypt(buffer.ToString());
            }
            catch (Exception ex)
            {
            }

            return encryptedString;
        }

        public static void SetAuthCookie(UserAuthentication identity)
        {
            string encrypted = Encrypt(identity);
            string cookieName = ConfigurationManager.AppSettings[AUTHCOOKIE_KEY];
            HttpCookie cookie = new HttpCookie(cookieName, encrypted);

            if (Utility.AppMode == DeployMode.Production)
            {
                cookie.Secure = true;
            }

            HttpContext.Current.Response.Cookies.Add(cookie);
        }
    }
}
