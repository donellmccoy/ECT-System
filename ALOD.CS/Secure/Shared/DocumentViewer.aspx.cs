using System;
using ALOD.Core.Domain.Documents;
using ALOD.Core.Domain.Workflow;
using ALOD.Data;
using ALOD.Logging;

namespace ALOD.Web.Docs
{
    public partial class Secure_Shared_DocumentViewer : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            IDocumentDao store = new SRXDocumentStore();
            string url = "";
            long docId = 0;
            int refId = 0;
            byte modId = (byte)ModuleType.System;

            if (Request.QueryString["docId"] != null)
            {
                long.TryParse(Request.QueryString["docId"], out docId);
            }

            if (Request.QueryString["refId"] != null)
            {
                int.TryParse(Request.QueryString["refId"], out refId);
            }

            if (Request.QueryString["modId"] != null)
            {
                byte.TryParse(Request.QueryString["modId"], out modId);
            }

            if (docId != 0)
            {
                url = store.GetDocumentViewerUrl(docId);
            }

            string des = Server.UrlDecode(Request.QueryString["doc"]);

            if (url.Length > 0)
            {
                if (refId != 0)
                {
                    LogManager.LogAction(modId, UserAction.ViewDocument, refId, "Document: " + des);
                }
                else
                {
                    LogManager.LogAction(modId, UserAction.ViewDocument, "Document: " + des);
                }

                Response.Redirect(url);
            }
        }
    }
}
