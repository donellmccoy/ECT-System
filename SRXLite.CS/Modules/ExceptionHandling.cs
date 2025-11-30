using System;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using System.Web;
using System.Web.Services.Protocols;
using System.Xml;
using SRXLite.DataAccess;
using static SRXLite.Modules.Util;
using static SRXLite.Modules.AppSettings;

namespace SRXLite.Modules
{
    public static class ExceptionHandling
    {
        #region CreateSoapException

        /// <summary>
        /// Creates a SoapException with error details.
        /// </summary>
        /// <param name="message">Error message.</param>
        /// <param name="detailMessage">Error message details.</param>
        /// <param name="user">User object.</param>
        /// <param name="source">Source method of the error.</param>
        /// <param name="writeToErrorLog">Write the error to log file/table.</param>
        /// <returns></returns>
        public static SoapException CreateSoapException(string message, string detailMessage, 
            SRXLite.Classes.User user, string source, bool writeToErrorLog)
        {
            int errorID = -1;
            if (writeToErrorLog)
            {
                errorID = LogError(detailMessage, user);
            }

            // Create nodes
            XmlDocument xmlDoc = new XmlDocument();
            XmlNode rootNode = xmlDoc.CreateNode(XmlNodeType.Element, SoapException.DetailElementName.Name, 
                SoapException.DetailElementName.Namespace);
            XmlNode errorNode = xmlDoc.CreateNode(XmlNodeType.Element, "Error", "");
            XmlNode idNode = xmlDoc.CreateNode(XmlNodeType.Element, "ErrorID", "");
            XmlNode messageNode = xmlDoc.CreateNode(XmlNodeType.Element, "Message", "");
            XmlNode sourceNode = xmlDoc.CreateNode(XmlNodeType.Element, "Source", "");

            // Child nodes with error info
            idNode.InnerText = errorID.ToString();
            messageNode.InnerText = SRXLite.Modules.AppSettings.Environment.IsDev() ? message : "";
            sourceNode.InnerText = source;

            // Append the child nodes to the error node
            errorNode.AppendChild(idNode);
            errorNode.AppendChild(messageNode);
            errorNode.AppendChild(sourceNode);

            // Append the error node to the detail node
            rootNode.AppendChild(errorNode);

            // Create new SOAP exception with error details
            XmlQualifiedName faultCode = SoapException.ServerFaultCode;
            SoapException soapEx = new SoapException("SRX Lite Web Service Error", faultCode, source, rootNode);
            return soapEx;
        }

        #endregion

        #region FormatLastError

        /// <summary>
        /// Formats the last error from HttpContext
        /// </summary>
        /// <returns></returns>
        public static string FormatLastError()
        {
            return FormatLastError(HttpContext.Current);
        }

        /// <summary>
        /// Formats the last error from HttpContext
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public static string FormatLastError(HttpContext context)
        {
            StringBuilder errorDetails = new StringBuilder();

            if (context != null)
            {
                string exceptionMessage;

                // Sometimes the InnerException is NULL; therefore, we need to check for this before calling the ToString() method...
                if (context.Server.GetLastError().InnerException != null)
                {
                    exceptionMessage = context.Server.GetLastError().InnerException.ToString();
                }
                else
                {
                    exceptionMessage = context.Server.GetLastError().ToString();
                }

                errorDetails.Append(exceptionMessage);
                errorDetails.Append("\r\n--------------------------------------------------");
                errorDetails.Append("\r\nURL=" + context.Request.Url.ToString());
                errorDetails.Append("\r\nREFERER=" + NullCheck(context.Request.UrlReferrer));
                errorDetails.Append("\r\nBROWSER=" + NullCheck(context.Request.Browser.Browser));
            }

            return errorDetails.ToString();
        }

        #endregion

        #region GetErrorMsg

        /// <summary>
        /// Returns the error message for an error ID.
        /// </summary>
        /// <param name="errorID"></param>
        /// <returns></returns>
        public static string GetErrorMsg(int errorID)
        {
            return NullCheck(DB.ExecuteScalar($"dsp_ErrorLog_GetErrorMsg {errorID}"));
        }

        #endregion

        #region GetLastErrorID

        /// <summary>
        /// Gets the last error ID from HttpContext
        /// </summary>
        /// <returns></returns>
        public static int GetLastErrorID()
        {
            HttpContext context = HttpContext.Current;
            int errorID = -1;
            if (context.Server.GetLastError() != null)
            {
                errorID = IntCheck(context.Server.GetLastError().Data["ErrorID"]);
            }
            return errorID;
        }

        #endregion

        #region LogError

        /// <summary>
        /// Logs an error message
        /// </summary>
        /// <param name="errorMsg"></param>
        /// <returns></returns>
        public static int LogError(string errorMsg)
        {
            return LogError(errorMsg, 0, 0);
        }

        /// <summary>
        /// Logs an error message with user information
        /// </summary>
        /// <param name="errorMsg"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        public static int LogError(string errorMsg, SRXLite.Classes.User user)
        {
            return LogError(errorMsg, user.UserID, user.SubuserID);
        }

        /// <summary>
        /// Inserts a record into the ErrorLog table.
        /// </summary>
        /// <param name="errorMsg"></param>
        /// <param name="userID"></param>
        /// <param name="subuserID"></param>
        /// <returns></returns>
        public static int LogError(string errorMsg, short userID, int subuserID)
        {
            try
            {
                SqlCommand command = new SqlCommand();
                command.CommandText = "dsp_ErrorLog_Insert";
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.Add(DB.GetSqlParameter("@ErrorMsg", errorMsg));
                command.Parameters.Add(DB.GetSqlParameter("@UserID", userID));
                command.Parameters.Add(DB.GetSqlParameter("@SubuserID", subuserID));

                // TODO: write to log file?

                return IntCheck(DB.ExecuteScalar(command)); // Return error ID
            }
            catch
            {
                // TODO: Handle exception
                return -1;
            }
        }

        #endregion
    }
}
