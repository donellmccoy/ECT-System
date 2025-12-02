using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;
using ALOD.Core.Domain.Users;
using ALOD.Data.Services;
using ALODWebUtility.Common;
using static ALODWebUtility.Common.SessionInfo;
using static ALODWebUtility.Common.Utility;

namespace ALOD.Web
{
    public partial class login_SelectAccount : System.Web.UI.Page
    {
        // Resource string constant - normally from App_GlobalResources
        private const string START_PAGE = "~/Secure/lod/Inbox.aspx";

        protected void AccountRepeater_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            int userId = 0;
            int.TryParse(e.CommandArgument.ToString(), out userId);

            if (userId == 0)
            {
                Response.Redirect("~/Default.aspx");
            }

            AppUser user = UserService.GetById(userId);

            if (!SetLogin(user))
            {
                // User is already logged into a different session; SetLogin has done a redirect to the
                // AltSession page therefore we need to exit the subprocedure in order to not override that call
                return;
            }

            string url = START_PAGE;

            if (SESSION_REDIRECT_URL != null)
            {
                url = SESSION_REDIRECT_URL;
            }

            Response.Redirect(url);
        }

        protected void AccountRepeater_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            AppUser user = (AppUser)e.Item.DataItem;
            LinkButton link = (LinkButton)e.Item.FindControl("AccountLink");

            link.CommandArgument = user.Id.ToString();
            link.Text = user.Username + " - " +
                        user.CurrentRole.Group.Description + " - " +
                        user.Unit.Name;
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                InitControls();
            }
        }

        private void InitControls()
        {
            if (SESSION_EDIPIN == null || SESSION_EDIPIN.Length == 0)
            {
                Response.Redirect("~/Default.aspx");
            }

            IList<AppUser> users;

            users = UserService.FindByEDIPIN(SESSION_EDIPIN);
            AccountRepeater.DataSource = (from u in users
                                         where u.Status == AccessStatus.Approved
                                         && u.AccountExpiration >= DateTime.Now
                                         orderby u.Username
                                         select u).ToList();

            AccountRepeater.DataBind();
        }
    }
}
