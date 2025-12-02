using System;
using System.Text;
using System.Web.UI.WebControls;
using ALOD.Core.Utils;
using ALODWebUtility.Common;
using ALODWebUtility.Providers;
using static ALODWebUtility.Common.Utility;

namespace ALOD.Web
{
    public partial class SessionViewer : System.Web.UI.Page
    {
        protected LinkButton lnkRefresh;
        protected Label lblMode;
        protected Label lblCount;
        protected Label lblNulls;
        protected Label lblPerms;
        protected Literal ltSummary;

        protected string even = "#FFFFFF";
        protected bool isEven = false;
        protected string odd = "#e6e6fa";

        protected string BGColor
        {
            get
            {
                isEven = !isEven;
                return isEven ? even : odd;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (AppMode != DeployMode.Development)
                {
                    // Development-only logic
                }

                lnkRefresh.Attributes.Add("onClick", "DoRefresh();");
            }

            StringBuilder buffer = new StringBuilder();
            int count = 0;

            foreach (string key in Session.Keys)
            {
                buffer.Append("<tr bgcolor='" + BGColor + "'>\r\n");
                buffer.Append("<td>&nbsp;" + key + "</td>\r\n");
                buffer.Append("<td>&nbsp;</td>\r\n");

                if (key == null || Session[key] == null)
                {
                    buffer.Append("<td style='color: blue;'>Nothing</td>\r\n");
                }
                else
                {
                    buffer.Append("<td style='color: blue;'>" + Session[key].GetType().ToString() + "</td>\r\n");
                }

                buffer.Append("<td>&nbsp;</td>\r\n");

                if (Session[key] == null)
                {
                    buffer.Append("<td style='color: blue;'>Nothing</td>\r\n");
                    count += 1;
                }
                else
                {
                    buffer.Append("<td>" + Session[key].ToString() + "</td>\r\n");
                }

                buffer.Append("</tr>");
            }

            ltSummary.Text = buffer.ToString();
            lblCount.Text = Server.HtmlEncode(Session.Count.ToString());
            lblNulls.Text = Server.HtmlEncode(count.ToString());
            lblMode.Text = Server.HtmlEncode(Session.Mode.ToString());

            UserAuthentication auth = (UserAuthentication)User.Identity;
            lblPerms.Text = Server.HtmlEncode(auth.Roles.Replace(",", ", "));
        }
    }
}
