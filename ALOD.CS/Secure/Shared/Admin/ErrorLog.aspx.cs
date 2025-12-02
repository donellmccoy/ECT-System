using System;
using System.Data;
using System.Web.UI;
using System.Web.UI.WebControls;
using AjaxControlToolkit;
using ALOD.Logging;

namespace ALOD.Web.Admin
{
    public partial class Secure_Shared_Admin_ErrorLog : System.Web.UI.Page
    {
        protected UpdatePanel updMain;
        protected GridView gvAllErrors;
        protected ObjectDataSource dataAllErrors;
        protected Panel Panel1;
        protected Label lblAppVersion;
        protected Label lblBrowser;
        protected Label lblMessage;
        protected Label lblStack;
        protected Label lblCaller;
        protected ModalPopupExtender mdlPopup;
        protected Button btnInvisible;

        private const int charCount = 65;

        protected void gvAllErrors_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType != DataControlRowType.DataRow)
            {
                return;
            }

            DataRowView row = (DataRowView)e.Row.DataItem;

            Label msgLabel = (Label)e.Row.FindControl("lblMessage");
            if (msgLabel.Text.Length > charCount)
            {
                msgLabel.ToolTip = Server.HtmlEncode(msgLabel.Text);
                msgLabel.Text = Server.HtmlEncode(msgLabel.Text.Substring(0, charCount)) + "<b> ...</b>";
            }

            Label lblPage = (Label)e.Row.FindControl("lblPage");

            if (lblPage.Text.IndexOf("?") == -1)
            {
                lblPage.ToolTip = Server.HtmlEncode(lblPage.Text);
                lblPage.Text = Server.HtmlEncode(lblPage.Text.Substring(lblPage.Text.LastIndexOf("/") + 1));
            }
            else
            {
                lblPage.ToolTip = Server.HtmlEncode(lblPage.Text);
                string pagePart = lblPage.Text.Substring(0, lblPage.Text.IndexOf("?"));
                int startIndex = pagePart.LastIndexOf("/") + 1;
                lblPage.Text = Server.HtmlEncode(pagePart.Substring(startIndex));
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                // FillGrid()
            }
        }

        protected void PopulateDetails(object sender, EventArgs e)
        {
            FillDetails((int)gvAllErrors.SelectedDataKey.Value);
            mdlPopup.Show();
        }

        private void FillDetails(int id)
        {
            DataSet ds = LogManager.GetErrorById(id);

            DataRow result = ds.Tables[0].Rows[0];

            lblAppVersion.Text = Server.HtmlEncode(result["appVersion"].ToString());
            lblBrowser.Text = Server.HtmlEncode(result["browser"].ToString());
            lblCaller.Text = Server.HtmlEncode(result["caller"].ToString().Trim().Length == 0 ? "N/A" : result["caller"].ToString());
            lblMessage.Text = Server.HtmlEncode(result["message"].ToString().Trim().Length == 0 ? "N/A" : result["message"].ToString());
            lblStack.Text = Server.HtmlEncode(result["stackTrace"].ToString().Trim().Length == 0 ? "N/A" : result["stackTrace"].ToString());

            gvAllErrors.SelectedIndex = -1;
        }
    }
}
