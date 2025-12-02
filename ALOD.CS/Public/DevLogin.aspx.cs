using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI.WebControls;
using ALOD.Core.Domain.Users;
using ALOD.Core.Utils;
using ALOD.Data.Services;
using ALODWebUtility.Common;
using static ALODWebUtility.Common.SessionInfo;
using static ALODWebUtility.Common.Utility;
using static ALODWebUtility.Common.WebControlSetters;

namespace ALOD.Web
{
    public partial class Public_DevLogin : System.Web.UI.Page
    {
        // Resource string constant - normally from App_GlobalResources
        private const string START_PAGE = "~/Secure/lod/Inbox.aspx";

        protected void btnLogin_Click(object sender, EventArgs e)
        {
            if (EDIPIN.Text.Trim().Length == 0)
            {
                return;
            }

            UserPreferences.SaveSetting("devEDIPIN", EDIPIN.Text.Trim());
            // For dev we assume the ssn and edipin are the same
            ProcessDevLogin(EDIPIN.Text.Trim());
        }

        protected void btnRoleLogin_Click(object sender, EventArgs e)
        {
            if (rblRoles.SelectedIndex == -1)
            {
                return;
            }

            UserPreferences.SaveSetting("devWing", WingSelect.SelectedValue);
            UserPreferences.SaveSetting("devShowBoard", ShowBoard.Checked.ToString());
            // For dev we assume the ssn and edipin are the same
            ProcessDevLogin(rblRoles.SelectedValue);
        }

        protected void rblRoles_SelectedIndexChanged(object sender, EventArgs e)
        {
            btnRoleLogin_Click(sender, e);
        }

        protected void EDIPIN_TextChanged(object sender, EventArgs e)
        {
        }

        protected void lnkNormalLogin_Click(object sender, EventArgs e)
        {
            UserPreferences.SaveSetting("devLogin", "0");
            Response.Redirect("~/Default.aspx", false);
            HttpContext.Current.ApplicationInstance.CompleteRequest();
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Utility.AppMode == DeployMode.Production || ConfigurationManager.AppSettings["DevLoginEnabled"] != "Y")
            {
                UserPreferences.SaveSetting("devLogin", "0");
                Response.Redirect("~/Default.aspx", false);
                HttpContext.Current.ApplicationInstance.CompleteRequest();
            }

            if (!IsPostBack)
            {
                EDIPIN.Text = UserPreferences.GetSetting("devEDIPIN");

                bool show = false;
                bool.TryParse(UserPreferences.GetSetting("devShowBoard"), out show);
                ShowBoard.Checked = show;

                EDIPIN.Focus();
            }
        }

        protected void ProcessDevLogin(string edipin)
        {
            if (Utility.AppMode == DeployMode.Production)
            {
                return;
            }

            IList<AppUser> users = UserService.FindByEDIPIN(edipin);

            // Update any expired accounts
            var expiredAccounts = from u in users
                                  where u.Status == AccessStatus.Approved
                                  && u.AccountExpiration < DateTime.Now
                                  select u;

            foreach (AppUser account in expiredAccounts)
            {
                account.Status = AccessStatus.Disabled;
            }

            // Grab the users valid accounts
            var validAccounts = (from u in users
                                where u.Status == AccessStatus.Approved
                                && u.CurrentRole.Id > 0
                                select u).ToList();

            if (validAccounts.Count == 0)
            {
                RedirectToRegistration(edipin);
            }

            // The user has at least one valid account
            string url = START_PAGE;

            if (validAccounts.Count == 1)
            {
                // If they only have one, use that and log them in
                if (!SetLogin(validAccounts[0]))
                {
                    // User is already logged into a different session; SetLogin has done a redirect to the
                    // AltSession page therefore we need to exit the subprocedure in order to not override that call
                    return;
                }
            }
            else
            {
                // The user has more than one account, redirect to the account selection page
                SESSION_EDIPIN = edipin;
                url = "~/Public/SelectAccount.aspx";
            }

            Response.Redirect(url, false);
            HttpContext.Current.ApplicationInstance.CompleteRequest();
        }

        protected void RedirectToRegistration(string edipin)
        {
            // Since this is not a registered user, remove the auth ticket.
            // Doing this denies them access to /secure parts of the site
            FormsAuthentication.SignOut();

            SESSION_EDIPIN = edipin;
            SESSION_COMPO = cbCompo.SelectedValue;

            Response.Redirect("~/Public/Register1.aspx?", false);
            HttpContext.Current.ApplicationInstance.CompleteRequest();
        }

        protected void WingSelect_DataBound(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                SetDropdownByValue(WingSelect, UserPreferences.GetSetting("devWing"));
            }
        }
    }
}
