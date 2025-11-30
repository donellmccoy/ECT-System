using System;
using System.Data;
using System.Web.UI.WebControls;
using SRXLite.DataAccess;
using static SRXLite.Modules.Util;

namespace SRXLite.Web
{
    public partial class ErrorLog : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                BindGrid(25);
            }
        }

        private void BindGrid(short rowCount)
        {
            gvErrorLog.DataSource = DB.ExecuteDataset("dsp_ErrorLog_Select " + rowCount);
            gvErrorLog.DataBind();
        }

        protected void gvErrorLog_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                string errorMsg = e.Row.Cells[1].Text;
                string errorText = Left(errorMsg, 200);
                if (errorMsg.Length > 200)
                {
                    string spanID = "E" + e.Row.RowIndex;
                    errorText += "...<span id='" + spanID + "' style='display:none'>" + errorMsg.Substring(200) + "</span>";
                    e.Row.Cells[1].Attributes.Add("onclick", "expand('" + spanID + "');");
                    e.Row.Cells[1].Style.Add("cursor", "pointer");
                }
                e.Row.Cells[1].Text = errorText.Replace("\r\n", "<br />");
            }
        }

        protected void ddlTop_SelectedIndexChanged(object sender, EventArgs e)
        {
            BindGrid(ShortCheck(ddlTop.SelectedValue));
        }
    }
}
