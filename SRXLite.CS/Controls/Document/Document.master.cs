using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using SRXLite.Classes;
using SRXLite.DataTypes;
using static SRXLite.Modules.ExceptionHandling;

namespace SRXLite.Web.Controls.Document
{
    public partial class DocumentMaster : System.Web.UI.MasterPage
    {
        private SRXLite.Classes.Document _doc;
        private static DocumentPage _docPage;
        private static DocumentPageGuid _docPageGuid;

        private long _docID;
        private User _user;
        private bool _readOnly = false;
        private DocumentMode _mode;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                // Add stylesheet link
                HtmlLink cssLink = new HtmlLink();
                cssLink.Href = this.TemplateSourceDirectory + "/Document.css";
                cssLink.Attributes.Add("type", "text/css");
                cssLink.Attributes.Add("rel", "stylesheet");
                Page.Header.Controls.Add(cssLink);
            }
        }

        public enum DocumentMode
        {
            Batch,
            Normal,
            Thumbnail
        }

        #region Properties

        public long DocID
        {
            get { return _docID; }
            set { _docID = value; }
        }

        public string DocTypeName
        {
            get { return lblDocType.Text; }
            set { lblDocType.Text = value; }
        }

        public User User
        {
            get { return _user; }
            set { _user = value; }
        }

        public bool IsReadOnly
        {
            get { return _readOnly; }
            set { _readOnly = value; }
        }

        public DocumentMode Mode
        {
            get { return _mode; }
            set { _mode = value; }
        }

        public string Width
        {
            get { return divContent.Style["width"]; }
            set
            {
                if (divContent.Style["width"] == null) divContent.Style.Add("width", "");
                divContent.Style["width"] = value;
            }
        }

        public string BackgroundColor
        {
            get { return divContent.Style["background-color"]; }
            set
            {
                if (divContent.Style["background-color"] == null) divContent.Style.Add("background-color", "");
                divContent.Style["background-color"] = value;
            }
        }

        #endregion

        #region AsyncTaskTimeout

        private void AsyncTaskTimeout(IAsyncResult result)
        {
            // TODO: timeout code
        }

        #endregion

        #region BindDocument

        public void BindDocument()
        {
            _doc = new SRXLite.Classes.Document(this.User, this.DocID);

            PageAsyncTask task = new PageAsyncTask(
                new BeginEventHandler(BeginGetDocumentPageList),
                new EndEventHandler(EndGetDocumentPageList),
                new EndEventHandler(AsyncTaskTimeout),
                null);

            Page.RegisterAsyncTask(task);
        }

        #endregion

        #region GetDocumentPageList (Begin/End)

        private IAsyncResult BeginGetDocumentPageList(object sender, EventArgs e, AsyncCallback callback, object stateObject)
        {
            return _doc.BeginGetPageList(callback, stateObject);
        }

        private void EndGetDocumentPageList(IAsyncResult result)
        {
            ProcessPageList(_doc.EndGetPageList(result));
        }

        #endregion

        #region ProcessPageList

        protected void ProcessPageList(List<DocumentPageData> list)
        {
            try
            {
                DocumentPageData data;

                if (_doc.IsImage && _doc.IsAppendable)
                {
                    CryptoManager crypto = new CryptoManager();
                    menuDelete.Visible = (!this.IsReadOnly) && (list.Count > 1);
                    ScriptManager1.EnablePageMethods = !this.IsReadOnly;
                    divContent.Attributes.Add("onkeydown", "CheckHotKeys(event);");
                    divContent.Attributes.Add("mode", this.Mode.ToString());

                    for (int i = 0; i < list.Count; i++)
                    {
                        data = list[i];
                        using (HtmlImage img = new HtmlImage())
                        {
                            using (HtmlImage imgThumbnail = new HtmlImage())
                            {
                                img.ID = "p" + i;
                                img.Alt = "";
                                imgThumbnail.ID = "tp" + i;
                                imgThumbnail.Alt = "Page " + (i + 1);

                                img.Attributes.Add("parentid", this.ClientID);
                                img.Attributes.Add("class", "page");
                                img.Attributes.Add("thumbid", this.ClientID + "_" + imgThumbnail.ID);
                                img.Attributes.Add("xsrc", data.PageUrl + "&c=0&r=&h=&w=");    // Set src and width in JS to fit window
                                img.Attributes.Add("onclick", "onClickImg(event,this);");
                                img.Attributes.Add("oncontextmenu", "return onContextMenuImg(event,this);");
                                divContent.Controls.Add(img);

                                imgThumbnail.Attributes.Add("parentid", this.ClientID);
                                imgThumbnail.Attributes.Add("for", img.ClientID);
                                imgThumbnail.Attributes.Add("class", "page");
                                imgThumbnail.Attributes.Add("tsrc", data.PageUrl + "&c=1&r=&h=&w=120");
                                imgThumbnail.Attributes.Add("onclick", "onClickImg(event,this);");
                                divThumbnails.Controls.Add(imgThumbnail);
                            }
                        }
                    }
                }
                else
                {
                    data = list[0];

                    using (HtmlGenericControl iframe = new HtmlGenericControl())
                    {
                        iframe.TagName = "iframe";
                        iframe.ID = "ifrDocument";
                        iframe.Attributes.Add("src", data.PageUrl);
                        iframe.Attributes.Add("style", "width:100%; height:100%");
                        divContent.Style.Add("height", "100%");
                        divContent.Controls.Add(iframe);
                    }

                    menu.Visible = false; // Image context menu not needed

                    if (!_doc.IsBrowserViewable)
                    {
                        // Close the window automatically
                        ScriptManager.RegisterClientScriptBlock(Page, Page.GetType(), "CloseWindowJS", "setTimeout('window.close();',5000);", true);
                    }
                }
            }
            catch (Exception ex)
            {
                // TODO: error msg
                LogError(ex.ToString());
                Response.Write("error");
            }
        }

        #endregion

        #region Dispose

        public override void Dispose()
        {
            if (_doc != null) _doc.Dispose();
            if (_docPage != null) _docPage.Dispose();
            if (_docPageGuid != null) _docPageGuid.Dispose();
            base.Dispose();
        }

        #endregion
    }
}
