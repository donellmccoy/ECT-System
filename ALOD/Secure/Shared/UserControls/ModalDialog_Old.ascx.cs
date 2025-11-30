using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

namespace Web.UserControls
{
    public partial class UserControl_ModalDialog : System.Web.UI.UserControl
    {
        private string _redirectURL;
        private int _timeout;

        public string RedirectURL
        {
            get
            {
                return _redirectURL;
            }
            set
            {
                _redirectURL = value;
            }
        }
        public int Timeout
        {
            get
            {
                return _timeout;
            }
            set
            {
                _timeout = value;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            AddKeepAlive();

            if (Page.IsPostBack == false)
            {
                //ButtonContinue.Attributes.Add("onClick", "startTimer();");
                //btnExit.Attributes.Add("onclick", "return exitWebsite('" + Page.ResolveUrl("~/public/logout.aspx") + "');");
            }
        }

        private void AddKeepAlive()
        {
            int timeOut = int.Parse(ConfigurationManager.AppSettings["timeoutDialog"]) * 60000;
            int milliSecondsTimeOut = (this.Session.Timeout * 60000) - timeOut;
            int delay = (timeOut / 1000) - 60;

            System.Text.StringBuilder buffer = new System.Text.StringBuilder();

            buffer.Append(Environment.NewLine);
            buffer.Append(@"<script type='text/javascript'>" + Environment.NewLine);
            buffer.Append("$_TD_DELAY=" + delay.ToString() + ";" + Environment.NewLine);
            buffer.Append("$_TD_MS_TIMEOUT=" + milliSecondsTimeOut.ToString() + ";" + Environment.NewLine);
            buffer.Append("$_TD_CONTINUE_BUTTON='" + ButtonContinue.ClientID + "';" + Environment.NewLine);
            buffer.Append(@"</script>");

            Page.ClientScript.RegisterClientScriptBlock(this.GetType().BaseType, "TimeoutVars", buffer.ToString());
            Page.ClientScript.RegisterClientScriptInclude("TimeoutDialog", Request.ApplicationPath + "/Script/TimeoutDialog.js");
        }

        protected void btnExit_Click(object sender, EventArgs e)
        {
            //Response.Redirect(RedirectURL);
        }
    }
}