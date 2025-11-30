using System;
using ALOD.Core.Domain.Users;
using ALOD.Core.Utils;
using ALOD.Data.Services;
using ALODWebUtility.Common;

namespace ALOD.Web
{
    public partial class Public_RegisterLink : System.Web.UI.Page
    {
        public string CAC_SSN
        {
            get
            {
                return (string)Session["CAC_SSN"];
            }
            set
            {
                Session["CAC_SSN"] = value;
            }
        }

        protected void accountLinkButton_Click(object sender, EventArgs e)
        {
            VerifyAccess();
            if (SSNMatched() == false)
            {
                return;
            }
            ProcessLogin(CAC_SSN);
        }

        protected void linkBtn_Click(object sender, EventArgs e)
        {
            VerifyAccess();
            if (SSNMatched() == false)
            {
                return;
            }
            LinkUser();
        }

        protected void LinkUser()
        {
            int userId = 0;

            string ssn = Server.HtmlEncode(SSNTextBox.Text).Trim().Replace("-", "");
            string firstName = Server.HtmlEncode(FirstNameTextBox.Text).Trim();
            string lastName = Server.HtmlEncode(LastNameTextBox.Text).Trim();
            string userName = Server.HtmlEncode(UserNameTextBox.Text).Trim();

            userId = UserService.GetIDByCredentials(firstName, lastName, userName, SESSION_SSN);

            if (userId == 0)
            {
                accountLinkedDiv.Visible = false;
                registerLinkedDiv.Visible = true;
            }
            else
            {
                linkBtn.Enabled = false;
                accountLinkedDiv.Visible = true;
                registerLinkedDiv.Visible = false;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            VerifyAccess();
            SetInputFormatRestriction(Page, SSNTextBox, FormatRestriction.Numeric);
        }

        protected void ProcessLogin(string ssn)
        {
            if (AppMode == DeployMode.Production)
            {
                return;
            }

            AppUser appUser = UserService.GetBySSN(ssn);

            if (appUser == null ||
                appUser.Status != AccessStatus.Approved ||
                appUser.CurrentRole.Id == 0)
            {
                RedirectToRegistration(appUser, ssn);
            }

            if (!SetLogin(appUser))
            {
                // User is already logged into a different session; SetLogin has done a redirect to the
                // AltSession page therefore we need to exit the subprocedure in order to not override that call
                return;
            }

            Response.Redirect("~/Secure/Welcome.aspx");
        }

        protected void RedirectToRegistration(AppUser appUser, string ssn)
        {
            if (appUser == null || appUser.Id == 0)
            {
                // since this is not a registered user, remove the auth ticket.
                // doing this denies them access to /secure parts of the site
                FormsAuthentication.SignOut();
            }
            Response.Redirect("~/Public/Register1.aspx");
        }

        protected void registerLinkButton_Click(object sender, EventArgs e)
        {
            VerifyAccess();
            if (SSNMatched() == false)
            {
                return;
            }
            Response.Redirect("~/Public/Register1.aspx");
        }

        protected bool SSNMatched()
        {
            string ssn = Server.HtmlEncode(SSNTextBox.Text).Trim().Replace("-", "");
            if (ssn.Length != 9)
            {
                InvalidSSNLabel.Visible = true;
                return false;
            }

            InvalidSSNLabel.Visible = false;
            if (CAC_SSN != ssn)
            {
                IncorrectSSNLabel.Visible = true;
                return false;
            }
            IncorrectSSNLabel.Visible = false;
            return true;
        }

        protected void VerifyAccess()
        {
            if (Session["ssn"] == null)
            {
                Response.Redirect("~/Default.aspx");
            }

            // This is just to make sure that the page is not browsed back and reused
            if (!Page.IsPostBack)
            {
                CAC_SSN = (string)Session["ssn"];
            }

            if (CAC_SSN != (string)Session["ssn"])
            {
                Response.Redirect("~/Default.aspx");
            }
        }
    }
}
