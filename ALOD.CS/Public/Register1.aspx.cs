using System;

namespace ALOD.Web
{
    public partial class Public_Register1 : System.Web.UI.Page
    {
        protected void btnAck_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/Public/Register2.aspx?");
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/Default.aspx");
        }

        protected void btnSign_Click(object sender, EventArgs e)
        {
            Session["signed"] = "1";
            modalPan.Visible = true;
            modalExtender.Show();
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            modalPan.Visible = false;
        }
    }
}
