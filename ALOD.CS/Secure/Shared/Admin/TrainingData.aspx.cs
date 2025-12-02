using System;
using System.Web.UI.WebControls;
using ALOD.Core.Utils;
using static ALODWebUtility.Common.Utility;

namespace ALOD.Web.Admin
{
    public partial class Secure_Shared_Admin_TrainingData : System.Web.UI.Page
    {
        protected DropDownList cbCompo;
        protected DropDownList cbRegion;
        protected Button btnView;
        protected Button btnExport;
        protected PlaceHolder plOutput;
        protected Button btnGenerate;
        protected ObjectDataSource ObjectDataSource1;

        protected void btnExport_Click(object sender, EventArgs e)
        {
            int region = 0;
            int.TryParse(cbRegion.SelectedValue, out region);
            DisplayData(region, true);
        }

        protected void btnGenerate_Click(object sender, EventArgs e)
        {
            int uics = 93, soldiers = 23, region = 0;

            if (int.TryParse(cbRegion.SelectedValue, out region))
            {
                // parsed successfully
            }

            if (uics == 0 || soldiers == 0 || region == 0)
            {
                return;
            }

            DisplayData(region, false);
        }

        protected void btnView_Click(object sender, EventArgs e)
        {
            int region = 0;
            int.TryParse(cbRegion.SelectedValue, out region);
            DisplayData(region, false);
        }

        protected void DisplayData(int region, bool export)
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
