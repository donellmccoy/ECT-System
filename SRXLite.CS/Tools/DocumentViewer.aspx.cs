using System;
using System.Collections.Generic;
using System.Configuration;
using System.Web;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using SRXLite.Classes;
using SRXLite.DataTypes;
using static SRXLite.Modules.ExceptionHandling;

namespace SRXLite.Web.Tools
{
    public partial class Tools_DocumentViewer : System.Web.UI.Page
    {
        private DocumentGuid _docGuid;
        private Document _doc;
        private static DocumentPage _docPage;
        private static DocumentPageGuid _docPageGuid;

        protected void Page_Load(object sender, EventArgs e)
        {
            _docGuid = new DocumentGuid(Request.QueryString["id"]);

            PageAsyncTask task = new PageAsyncTask(
                new BeginEventHandler(BeginGetDocumentGuidData),
                new EndEventHandler(EndGetDocumentGuidData),
                new EndEventHandler(AsyncTaskTimeout),
                null);
            bool printButton = bool.Parse(ConfigurationManager.AppSettings["PrintButton"]);
            divPrint.Visible = printButton;

            RegisterAsyncTask(task);
        }

        #region AsyncTaskTimeout

        private void AsyncTaskTimeout(IAsyncResult result)
        {
            // TODO: timeout code
        }

        #endregion

        #region GetDocumentGuidData (Begin/End)

        private IAsyncResult BeginGetDocumentGuidData(object sender, EventArgs e, AsyncCallback callback, object stateObject)
        {
            return _docGuid.BeginGetData(callback, stateObject);
        }

        private void EndGetDocumentGuidData(IAsyncResult result)
        {
            DocumentGuid.GuidData data = _docGuid.EndGetData(result);

            if (data.DocID == 0)
            {
                // Guid was not found or has expired
                divContent.InnerText = "This page has expired.";
            }
            else
            {
                _doc = new Document(data);
                lblDocType.Text = data.DocTypeName;

                PageAsyncTask task = new PageAsyncTask(
                    new BeginEventHandler(BeginGetDocumentPageList),
                    new EndEventHandler(EndGetDocumentPageList),
                    new EndEventHandler(AsyncTaskTimeout),
                    null);

                RegisterAsyncTask(task);
            }
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
                    HtmlBody.Attributes.Add("onload", "DocumentOnLoad();");
                    menuDelete.Visible = (!_docGuid.Data.IsReadOnly) && (list.Count > 1);
                    ScriptManager1.EnablePageMethods = !_docGuid.Data.IsReadOnly;
                    divPDF.Attributes.Add("onclick", string.Format("window.location='{0}';", "../Handlers/GenerateDocumentPDF.ashx?id=" + Request.QueryString["id"]));

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

                                // Images
                                img.Attributes.Add("class", "page");
                                img.Attributes.Add("thumbid", imgThumbnail.ClientID);
                                img.Attributes.Add("xsrc", data.PageUrl + "&c=0&r=&h=&w=");    // Set src and width in JS to fit window
                                img.Attributes.Add("onclick", "onClickImg(event,this);");
                                img.Attributes.Add("oncontextmenu", "return onContextMenuImg(event,this);");
                                divImages.Controls.Add(img);

                                // Thumbnail page number labels
                                using (HtmlGenericControl lbl = new HtmlGenericControl())
                                {
                                    lbl.TagName = "span";
                                    lbl.Attributes.Add("class", "pagenum");
                                    lbl.InnerHtml = (i + 1).ToString();
                                    divThumbnails.Controls.Add(lbl);
                                }

                                // Thumbnails
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
                    divContent.Controls.Clear();
                    ScriptManager1.EnablePageMethods = false;

                    using (HtmlGenericControl iframe = new HtmlGenericControl())
                    {
                        iframe.TagName = "iframe";
                        iframe.ID = "ifrDocument";
                        iframe.Attributes.Add("src", data.PageUrl);
                        iframe.Attributes.Add("style", "width:100%; height:100%");
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

        #region Page Methods

        [WebMethod]
        public static void DeletePage(string ID)
        {
            _docPageGuid = new DocumentPageGuid(ID);
            IAsyncResult result = _docPageGuid.BeginGetData(null, null);
            _docPageGuid.EndGetData(result);

            _docPage = new DocumentPage(_docPageGuid.Data);
            _docPage.BeginDelete(null, null);
        }

        #endregion

        #region Dispose

        public override void Dispose()
        {
            if (_docGuid != null) _docGuid.Dispose();
            if (_doc != null) _doc.Dispose();
            if (_docPage != null) _docPage.Dispose();
            if (_docPageGuid != null) _docPageGuid.Dispose();
            base.Dispose();
        }

        #endregion
    }
}
