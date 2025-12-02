using System;
using System.Web.UI.WebControls;
using ALOD.Core.Utils;
using static ALODWebUtility.Common.Utility;

namespace ALOD.Web.Admin
{
    public partial class Secure_Shared_Admin_TrainingUsers : System.Web.UI.Page
    {
        protected DropDownList cbCompo;
        protected DropDownList cbRegion;
        protected Button btnView;
        protected Button btnExport;
        protected PlaceHolder plOutput;
        protected ObjectDataSource ObjectDataSource1;

        protected void btnExport_Click(object sender, EventArgs e)
        {
            int region = 0;
            int.TryParse(cbRegion.SelectedValue, out region);
            DisplayUsers(region, true);
        }

        protected void btnView_Click(object sender, EventArgs e)
        {
            int region = 0;
            int.TryParse(cbRegion.SelectedValue, out region);
            DisplayUsers(region, false);
        }

        protected void DisplayUsers(int region, bool export)
        {
            // Method implementation commented out in original
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            // This page is not allowed on prod
            if (AppMode == DeployMode.Production)
            {
                Response.Redirect("~/Secure/Welcome.aspx", true);
            }
        }
    }
}
