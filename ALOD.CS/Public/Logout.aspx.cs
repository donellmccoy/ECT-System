using System;
using System.Web.Security;
using ALODWebUtility.Providers;

namespace ALOD.Web
{
    public partial class Logout : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            ALOD.Data.Services.UserService.Logout(Session["UserId"] != null ? (int)Session["UserId"] : 0);
            Session.Clear();
            Session.Abandon();
            AuthenticationHandler.Logout();
            FormsAuthentication.SignOut();

            // Response.Redirect("~/Default.aspx");
        }
    }
}
