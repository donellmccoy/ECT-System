using System;
using System.Configuration;
using System.Web.UI;
using ALOD.Core.Utils;
using ALODWebUtility.Common;

namespace ALOD.Web
{
    public partial class _Default : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (AppMode != DeployMode.Production && ConfigurationManager.AppSettings["DevLoginEnabled"] == "Y")
                {
                    lnkDevLogin.Visible = true;
                }
                else
                {
                    lnkDevLogin.Visible = false;
                }

                btnLoginCac.Attributes.Add("onclick", "showDialog(); return false;");
                
                if (UserPreferences.GetSetting("devLogin") == "1")
                {
                    string url = "~/public/devlogin.aspx";
                    Response.Redirect(url);
                }
            }
        }

        protected void btnLoginCac_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/login/Login.aspx");
        }

        protected void lnkDevLogin_Click(object sender, EventArgs e)
        {
            UserPreferences.SaveSetting("devLogin", "1");
            Response.Redirect("~/public/devlogin.aspx");
        }
    }
}
