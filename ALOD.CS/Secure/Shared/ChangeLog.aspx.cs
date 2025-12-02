using System;
using System.Web.UI.WebControls;
using ALODWebUtility.Common;

namespace ALOD.Web
{
    public partial class Secure_Shared_ChangeLog : System.Web.UI.Page
    {
        protected GridView gvChanges;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                InitControls();
            }
        }

        private void InitControls()
        {
            var changes = new ChangeSet();
            changes.GetByLogId(int.Parse(Request.QueryString["logId"]));
            gvChanges.DataSource = changes;
            gvChanges.DataBind();
        }
    }
}
