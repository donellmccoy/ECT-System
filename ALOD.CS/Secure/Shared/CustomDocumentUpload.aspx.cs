using System;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using ALOD.Core.Domain.Documents;
using ALOD.Core.Interfaces;
using ALOD.Core.Utils;
using ALOD.Data;
using ALOD.Logging;
using ALODWebUtility.Common;
using ALODWebUtility.Documents;
using ALODWebUtility.Providers;
using static ALODWebUtility.Common.SessionInfo;
using static ALODWebUtility.Common.Utility;

namespace ALOD.Web.Docs
{
    public class CustomDocumentUpload : System.Web.UI.Page
    {
        protected HtmlGenericControl DocTypeName;
        protected HtmlGenericControl divSubtitle;
        protected HtmlGenericControl divValidation;
        protected TextBox txtDocDate;
        protected TextBox txtDocDescr;
        protected TextBox txtKeywords;
        protected FileUpload filePicker;
        protected HtmlInputSubmit btnSubmit;

        private IDocumentDao _docDao;
        private DocumentProcessingFactory _processingFactory;
        private BulletedList _validationErrors;

        public string DocEntity
        {
            get
            {
                if (ViewState["DocEntity"] == null)
                {
                    ViewState["DocEntity"] = string.Empty;
                }
                return LodCrypto.Decrypt((string)ViewState["DocEntity"]);
            }
            set { ViewState["DocEntity"] = value; }
        }

        public int DocGroupId
        {
            get
            {
                if (ViewState["DocGroupId"] == null)
                {
                    ViewState["DocGroupId"] = 0;
                }
                return (int)ViewState["DocGroupId"];
            }
            set { ViewState["DocGroupId"] = value; }
        }

        public int DocType
        {
            get
            {
                if (ViewState["DocType"] == null)
                {
                    ViewState["DocType"] = 0;
                }
                return (int)ViewState["DocType"];
            }
            set { ViewState["DocType"] = value; }
        }

        public string DocTypeTitle
        {
            get
            {
                if (ViewState["DocTypeTitle"] == null)
                {
                    ViewState["DocTypeTitle"] = string.Empty;
                }
                return (string)ViewState["DocTypeTitle"];
            }
            set { ViewState["DocTypeTitle"] = value; }
        }

        public IDocumentDao DocumentDao
        {
            get
            {
                if (_docDao == null)
                {
                    _docDao = new SRXDocumentStore(SESSION_USERNAME);
                }
                return _docDao;
            }
        }

        public DocumentProcessingFactory ProcessingFactory
        {
            get
            {
                if (_processingFactory == null)
                {
                    _processingFactory = new DocumentProcessingFactory();
                }
                return _processingFactory;
            }
        }

        public int ProcessType
        {
            get
            {
                if (ViewState["ProcessType"] == null)
                {
                    ViewState["ProcessType"] = 1;
                }
                return (int)ViewState["ProcessType"];
            }
            set { ViewState["ProcessType"] = value; }
        }

        public int RefId
        {
            get
            {
                if (ViewState["RefId"] == null)
                {
                    ViewState["RefId"] = 0;
                }
                return (int)ViewState["RefId"];
            }
            set { ViewState["RefId"] = value; }
        }

        protected string CalendarImage
        {
            get { return this.ResolveClientUrl("~/App_Themes/" + Page.Theme + "/Images/Calendar.gif"); }
        }

        protected BulletedList ValidationErrors
        {
            get
            {
                if (_validationErrors == null)
                {
                    _validationErrors = new BulletedList();
                }
                return _validationErrors;
            }
        }

        protected void btnSubmit_ServerClick(object sender, EventArgs e)
        {
            ValidateDocumentDate();
            ValidateSelectedFile();

            if (ValidationErrors.Items.Count > 0)
            {
                SetValidationErrorsControl();
                return;
            }

            if (!ProcessSelectedDocument())
            {
                SetValidationErrorsControl();
                return;
            }

            // Close the window...
            ScriptManager.RegisterStartupScript(Page, Page.GetType(), "UploadCompleteJS", "window.close();", true);
        }

        protected Document GenerateDocumentMetaData()
        {
            string fileExt = filePicker.FileName.Substring(filePicker.FileName.LastIndexOf(".") + 1);
            DateTime docDate = DateTime.Parse(txtDocDate.Text);

            Document docMetaData = new Document();

            docMetaData.DateAdded = DateTime.Now;
            docMetaData.DocDate = docDate;
            docMetaData.Description = Server.HtmlEncode(txtDocDescr.Text.Trim());
            docMetaData.DocStatus = DocumentStatus.Approved;
            docMetaData.Extension = fileExt;
            docMetaData.SSN = DocEntity;
            docMetaData.DocType = (DocumentType)DocType;
            docMetaData.OriginalFileName = Utility.FormatFileName(filePicker.FileName);

            return docMetaData;
        }

        protected string GetFileExtension(string fileName)
        {
            if (string.IsNullOrEmpty(fileName))
            {
                return string.Empty;
            }
            return fileName.Substring(fileName.LastIndexOf(".") + 1);
        }

        protected void InitControls()
        {
            DocTypeName.InnerText = DocTypeTitle;

            // XXX-XX-0002
            if (DocEntity.Length == 9)
            {
                divSubtitle.InnerText = "for XXX-XX-" + DocEntity.Substring(5, 4);
            }

            SetInputFormatRestriction(Page, txtDocDate, FormatRestriction.Numeric, "/");
            txtDocDate.CssClass = "datePickerPast";
            txtDocDate.Enabled = true;
        }

        protected bool IsFileExtValid(string ext)
        {
            switch (ext.ToLower())
            {
                case "bmp":
                case "doc":
                case "docx":
                case "g42":
                case "gif":
                case "jpeg":
                case "jpg":
                case "pdf":
                case "pjpg":
                case "png":
                case "rtf":
                case "tif":
                case "tiff":
                case "txt":
                case "xls":
                case "xlsx":
                    return true;
                default:
                    return false;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            WriteHostName(Page);
            RegisterJavaScriptScripts();
            ProcessQueryString();

            if (!IsPostBack)
            {
                InitControls();
            }
        }

        protected void ProcessQueryString()
        {
            // Check and assign RefId from the query string
            string queryRefId = Request.QueryString["refId"];
            if (!string.IsNullOrEmpty(queryRefId) && int.TryParse(queryRefId, out int refIdResult))
            {
                RefId = refIdResult;
            }
            else
            {
                LogManager.LogError("ProcessQueryString: RefId is missing or invalid in the query string.");
                RefId = 0;
            }

            int.TryParse(Request.QueryString["group"], out int groupId);
            DocGroupId = groupId;

            int.TryParse(Request.QueryString["id"], out int docTypeId);
            DocType = docTypeId;

            // Null check for catName
            if (!string.IsNullOrEmpty(Request.QueryString["catName"]))
            {
                DocTypeTitle = Request.QueryString["catName"].ToString();
            }
            else
            {
                DocTypeTitle = string.Empty;
            }

            DocEntity = Request.QueryString["entity"];

            int.TryParse(Request.QueryString["processType"], out int processType);
            ProcessType = processType == 0 ? 1 : processType;
        }

        protected bool ProcessSelectedDocument()
        {
            IDocumentProcessingStrategy strategy = ProcessingFactory.GetStrategyByType((ProcessingStrategyType)ProcessType);

            if (strategy == null)
            {
                ValidationErrors.Items.Add("Failed to process document");
                return false;
            }

            long docId = strategy.ProcessDocument(RefId, DocGroupId, DocumentDao, GenerateDocumentMetaData(), filePicker.FileBytes);

            if (docId <= 0)
            {
                if (strategy.GetProcessingErrors().Count > 0)
                {
                    foreach (string s in strategy.GetProcessingErrors())
                    {
                        ValidationErrors.Items.Add(s);
                    }
                }
                else
                {
                    ValidationErrors.Items.Add("Failed to process document. Please contact a system administrator for further assistance.");
                }

                return false;
            }

            DocumentDao.InsertRecentlyAddedDocument(RefId, DocGroupId, docId, (DocumentType)DocType);

            return true;
        }

        protected void RegisterJavaScriptScripts()
        {
            Page.ClientScript.RegisterClientScriptInclude("JQueryScript", Request.ApplicationPath + "/Script/jquery-3.6.0.min.js");
            Page.ClientScript.RegisterClientScriptInclude("MigrateScript", Request.ApplicationPath + "/Script/jquery-migrate-3.4.1.min.js");

            Page.ClientScript.RegisterClientScriptInclude("JqueryBlock", Request.ApplicationPath + "/Script/jquery.blockUI.min.js");
            Page.ClientScript.RegisterClientScriptInclude("JqueryDom", Request.ApplicationPath + "/Script/jquery-dom.js");
            Page.ClientScript.RegisterClientScriptInclude("JqueryUI", Request.ApplicationPath + "/Script/jquery-ui-1.13.0.min.js");
            Page.ClientScript.RegisterClientScriptInclude("JQueryModal", Request.ApplicationPath + "/Script/jqModal.js");
            Page.ClientScript.RegisterClientScriptInclude("CommonScript", Request.ApplicationPath + "/Script/common.js");
        }

        protected void SetValidationErrorsControl()
        {
            ValidationErrors.Style.Add("margin-top", "2px");
            ValidationErrors.Style.Add("margin-bottom", "10px");

            divValidation.Controls.Add(new LiteralControl("The following errors have occurred:"));
            divValidation.Controls.Add(ValidationErrors);
        }

        protected bool ValidateDocumentDate()
        {
            if (string.IsNullOrEmpty(txtDocDate.Text))
            {
                ValidationErrors.Items.Add("Document Date is required");
                return false;
            }

            if (!DateTime.TryParse(txtDocDate.Text, out DateTime docDate))
            {
                ValidationErrors.Items.Add("Document Date is invalid");
                return false;
            }

            if (docDate == default(DateTime))
            {
                ValidationErrors.Items.Add("Document Date is required");
                return false;
            }
            else if (docDate > DateTime.Today)
            {
                ValidationErrors.Items.Add("Document Date cannot be a future date");
                return false;
            }
            else if (docDate.Year < 1754)
            {
                ValidationErrors.Items.Add("Document Date is invalid");
                return false;
            }

            return true;
        }

        protected bool ValidateSelectedFile()
        {
            if (!filePicker.HasFile)
            {
                ValidationErrors.Items.Add("Must select a document to upload");
                return false;
            }

            if (!IsFileExtValid(GetFileExtension(filePicker.FileName)))
            {
                ValidationErrors.Items.Add("Invalid file type");
                return false;
            }

            if (!Utility.IsFileSizeValid(filePicker.PostedFile.ContentLength))
            {
                ValidationErrors.Items.Add("File size (" + Utility.GetFileSizeMB(filePicker.PostedFile.ContentLength) + " MB) exceeded the maximum limit. Uploads are limited to " + Utility.GetFileSizeUploadLimitMB() + " MB.");
                return false;
            }

            return true;
        }
    }
}
