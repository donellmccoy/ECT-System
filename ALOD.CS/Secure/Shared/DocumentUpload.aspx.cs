using System;
using System.Diagnostics;
using System.Web;
using System.Web.UI.WebControls;
using ALOD.Core.Domain.Documents;
using ALOD.Data;
using ALOD.Logging;
using ALODWebUtility.Providers;

namespace ALOD.Web.Docs
{
    public partial class Secure_Shared_DocumentUpload : System.Web.UI.Page
    {
        protected Label ErrorLabel;

        protected void Page_Load(object sender, EventArgs e)
        {
            // Capture and log query string parameters
            string groupIdString = Request.QueryString["groupId"];
            if (string.IsNullOrWhiteSpace(groupIdString))
            {
                groupIdString = Request.QueryString["group"]; // Fallback to "group" if "groupId" is not found
            }

            string docTypeString = Request.QueryString["docType"];
            if (string.IsNullOrWhiteSpace(docTypeString))
            {
                docTypeString = Request.QueryString["id"]; // Fallback to "id" if "docType" is not found
            }

            // Decrypting the docEntity parameter
            string docEntity = LodCrypto.Decrypt(Request.QueryString["entity"]);

            // Logging the values for debugging
            Debug.WriteLine("groupId: " + groupIdString);
            Debug.WriteLine("docType: " + docTypeString);
            Debug.WriteLine("docEntity: " + docEntity);

            // Validate groupId
            long groupId;
            if (string.IsNullOrWhiteSpace(groupIdString) || !long.TryParse(groupIdString, out groupId) || groupId == 0)
            {
                LogManager.LogError("Invalid or missing groupId in Secure_Shared_DocumentUpload.Page_Load");
                ErrorLabel.Text = "Invalid or missing group ID. Please check your input and try again.";
                return;
            }

            // Validate and Parse docType
            DocumentType docType;
            if (string.IsNullOrWhiteSpace(docTypeString) || !Enum.TryParse(docTypeString, true, out docType) || !Enum.IsDefined(typeof(DocumentType), docType))
            {
                LogManager.LogError($"Invalid docType value '{docTypeString}' in Secure_Shared_DocumentUpload.Page_Load");
                ErrorLabel.Text = "Invalid document type value. Please check your input and try again.";
                return;
            }

            // Specific condition check from C# code
            if (docEntity == "XXXXXTEST")
            {
                ErrorLabel.Text = "An error occurred initializing the document transfer system. Please check the Error Log for more details.";
                return;
            }

            try
            {
                // Proceed with the document store operations
                IDocumentDao store = new SRXDocumentStore(Session["UserName"].ToString());
                string url = store.GetDocumentUploadUrl(groupId, docType, docEntity);

                if (!string.IsNullOrEmpty(url))
                {
                    Response.Redirect(url, false);
                    HttpContext.Current.ApplicationInstance.CompleteRequest();
                }
                else
                {
                    // Handle the case where the URL is empty
                    LogManager.LogError("Document upload URL is empty in Secure_Shared_DocumentUpload.Page_Load");
                    ErrorLabel.Text = "Failed to initialize document upload. URL is empty.";
                }
            }
            catch (Exception ex)
            {
                // Log the exception details
                LogManager.LogError("Exception in Secure_Shared_DocumentUpload.Page_Load: " + ex.ToString());
                ErrorLabel.Text = "An exception occurred during document upload initialization: " + ex.Message;
            }
        }
    }
}
