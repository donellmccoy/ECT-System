using System;
using System.Data;
using System.Drawing.Imaging;
using System.Web;
using System.Web.Caching;
using SRXLite.DataAccess;

namespace SRXLite.Modules
{
    public static class Util
    {
        #region GetURL

        /// <summary>
        /// Returns the full URL of the website
        /// </summary>
        /// <param name="relativePath"></param>
        /// <param name="query"></param>
        /// <returns></returns>
        public static string GetURL(string relativePath = "", string query = "")
        {
            return GetURL(HttpContext.Current, relativePath, query);
        }

        /// <summary>
        /// Returns the full URL of the website
        /// </summary>
        /// <param name="context"></param>
        /// <param name="relativePath"></param>
        /// <param name="query"></param>
        /// <returns></returns>
        public static string GetURL(HttpContext context, string relativePath = "", string query = "")
        {
            Uri currentUri = context.Request.Url;
            UriBuilder uriBuild = new UriBuilder();
            uriBuild.Scheme = currentUri.Scheme;
            uriBuild.Host = currentUri.Host;
            uriBuild.Port = currentUri.Port;
            uriBuild.Path = context.Request.ApplicationPath + relativePath;
            uriBuild.Query = query;

            return uriBuild.Uri.AbsoluteUri;
        }

        #endregion

        #region BoolCheck

        /// <summary>
        /// Safely converts a value to boolean
        /// </summary>
        /// <param name="value"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static bool BoolCheck(object value, bool defaultValue = false)
        {
            if (DBNull.Value.Equals(value) || value == null || value.ToString() == "")
                return defaultValue;
            return Convert.ToBoolean(value);
        }

        #endregion

        #region ByteCheck

        /// <summary>
        /// Safely converts a value to byte
        /// </summary>
        /// <param name="value"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static byte ByteCheck(object value, byte defaultValue = 0)
        {
            if (!IsNumeric(value))
                return defaultValue;
            return Convert.ToByte(value);
        }

        #endregion

        #region DateCheck

        /// <summary>
        /// Safely converts a value to DateTime
        /// </summary>
        /// <param name="value"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static DateTime DateCheck(object value, DateTime defaultValue)
        {
            if (!IsDate(value))
                return defaultValue;
            return Convert.ToDateTime(value);
        }

        #endregion

        #region IntCheck

        /// <summary>
        /// Safely converts a value to integer
        /// </summary>
        /// <param name="value"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static int IntCheck(object value, int defaultValue = 0)
        {
            if (!IsNumeric(value))
                return defaultValue;
            return Convert.ToInt32(value);
        }

        #endregion

        #region LngCheck

        /// <summary>
        /// Safely converts a value to long
        /// </summary>
        /// <param name="value"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static long LngCheck(object value, long defaultValue = 0)
        {
            if (!IsNumeric(value))
                return defaultValue;
            return Convert.ToInt64(value);
        }

        #endregion

        #region NullCheck

        /// <summary>
        /// Handles null string
        /// </summary>
        /// <param name="value"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static string NullCheck(object value, string defaultValue = "")
        {
            if (DBNull.Value.Equals(value) || value == null)
                return defaultValue;
            return value.ToString().Trim();
        }

        #endregion

        #region ShortCheck

        /// <summary>
        /// Safely converts a value to short
        /// </summary>
        /// <param name="value"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static short ShortCheck(object value, short defaultValue = 0)
        {
            if (!IsNumeric(value))
                return defaultValue;
            return Convert.ToInt16(value);
        }

        #endregion

        #region FormatFileExt

        /// <summary>
        /// Extracts file extension from filename
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static string FormatFileExt(string fileName)
        {
            return fileName.Substring(fileName.LastIndexOf(".") + 1).ToLower();
        }

        #endregion

        #region FormatFileName

        /// <summary>
        /// Extracts filename from full path
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static string FormatFileName(string fileName)
        {
            if (fileName.IndexOf("\\") == -1)
                return fileName;
            return fileName.Substring(fileName.LastIndexOf("\\") + 1);
        }

        #endregion

        #region GetCachedFileExtTable

        /// <summary>
        /// Returns cached file extension table
        /// </summary>
        /// <returns></returns>
        public static DataTable GetCachedFileExtTable()
        {
            return GetCachedFileExtTable(HttpContext.Current);
        }

        /// <summary>
        /// Returns cached file extension table
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public static DataTable GetCachedFileExtTable(HttpContext context)
        {
            DataTable dt = context.Cache["LkupFileExtension"] as DataTable;
            if (dt == null)
            {
                dt = DB.ExecuteDataset("exec dsp_GetFileExtensions").Tables[0];
                context.Cache.Insert("LkupFileExtension", dt, null, Cache.NoAbsoluteExpiration, TimeSpan.FromHours(1));
            }
            return dt;
        }

        #endregion

        #region IsFileExtValid

        public static bool IsFileExtValid(string fileExt)
        {
            if (fileExt.IndexOf(".") > -1)
                fileExt = fileExt.Substring(fileExt.LastIndexOf(".") + 1);
            DataTable dt = GetCachedFileExtTable();
            DataRow[] dr = dt.Select($"FileExt='{fileExt}'");
            return dr.Length > 0;
        }

        #endregion

        #region IsFileSizeValid

        public static bool IsFileSizeValid(int length)
        {
            return length <= AppSettings.FileSizeUploadLimit();
        }

        public static bool IsInitialFileSizeValid(int length)
        {
            return length <= AppSettings.InitialFileSizeUploadLimit();
        }

        #endregion

        #region GetFileSizeMB

        public static double GetFileSizeMB(int fileSizeBytes)
        {
            return Math.Round(fileSizeBytes / 1048576.0, 2);
        }

        #endregion

        #region GetEncoderInfo

        /// <summary>
        /// Gets the encoder info for a given MIME type
        /// </summary>
        /// <param name="mimeType"></param>
        /// <returns></returns>
        public static ImageCodecInfo GetEncoderInfo(string mimeType)
        {
            foreach (ImageCodecInfo codecInfo in ImageCodecInfo.GetImageEncoders())
            {
                if (codecInfo.MimeType == mimeType)
                {
                    return codecInfo;
                }
            }
            return null;
        }

        #endregion

        #region GetClientIP

        /// <summary>
        /// Gets the client IP address
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public static string GetClientIP(HttpContext context = null)
        {
            if (context == null)
            {
                if (HttpContext.Current != null)
                {
                    context = HttpContext.Current;
                }
                else
                {
                    return "";
                }
            }

            string ipAddress = context.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
            if (ipAddress != null)
            {
                // Get last IP address in comma-delimited list
                ipAddress = ipAddress.Substring(ipAddress.LastIndexOf(",") + 1).Trim();
            }
            else
            {
                ipAddress = context.Request.ServerVariables["REMOTE_ADDR"];
            }

            return ipAddress;
        }

        #endregion

        #region Helper Methods

        private static bool IsNumeric(object value)
        {
            if (value == null || DBNull.Value.Equals(value))
                return false;

            double result;
            return double.TryParse(value.ToString(), out result);
        }

        private static bool IsDate(object value)
        {
            if (value == null || DBNull.Value.Equals(value))
                return false;

            DateTime result;
            return DateTime.TryParse(value.ToString(), out result);
        }

        #endregion
    }
}
