using System;
using System.Data;
using System.Data.Common;
using System.Web;
using System.Web.Script.Services;
using System.Web.Services;
using System.Xml;
using ALOD.Data;
using ALODWebUtility.Common;
using static ALODWebUtility.Common.SessionInfo;

namespace ALOD
{
    /// <summary>
    /// Summary description for PublicDataService
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line.
    // [System.Web.Script.Services.ScriptService]
    public class PublicDataService : System.Web.Services.WebService
    {
        [WebMethod(EnableSession = true)]
        [ScriptMethod()]
        public XmlDocument GetUnits(string p, string descr, string active)
        {
            XmlDocument xmlDoc = new XmlDocument();

            // SESSION_EDIPIN is static and requires HttpContext.Current.Session
            string edipin = HttpContext.Current.Session["EDIPIN"]?.ToString();
            if (!string.IsNullOrEmpty(edipin))
            {
                string pascode = p.Trim();
                string longName = descr.Trim();

                DataSet ds = new DataSet();
                SqlDataStore adapter = new SqlDataStore();
                DbCommand dbCommand;

                bool activeOnly;
                bool.TryParse(active, out activeOnly);

                dbCommand = adapter.GetStoredProcCommand("core_pascode_sp_search");
                adapter.AddInParameter(dbCommand, "@pascode", DbType.String, pascode);
                adapter.AddInParameter(dbCommand, "@longName", DbType.String, longName);
                adapter.AddInParameter(dbCommand, "@activeOnly", DbType.Boolean, activeOnly);

                ds = adapter.ExecuteDataSet(dbCommand);

                if (ds.Tables.Count > 0)
                {
                    ds.Tables[0].TableName = "Unit";
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        xmlDoc.LoadXml(ds.GetXml());
                        return xmlDoc;
                    }
                }

                XmlDeclaration xmldecl;
                xmldecl = xmlDoc.CreateXmlDeclaration("1.0", null, null);
                // Add the new node to the document.
                XmlElement root = xmlDoc.DocumentElement;
                xmlDoc.InsertBefore(xmldecl, root);
            }

            return xmlDoc;
        }
    }
}
