using System;
using System.Web.UI.WebControls;
using ALOD.Core.Domain.Users;

namespace ALOD.Web.Admin
{
    public partial class Secure_Shared_Admin_AppAuth : System.Web.UI.Page
    {
        protected GridView gvValues;
        protected ObjectDataSource odsUsersTitle;

        protected void Page_Load(object sender, EventArgs e)
        {
            odsUsersTitle.SelectParameters["groupId"].DefaultValue = ((int)UserGroups.BoardApprovalAuthority).ToString();
            odsUsersTitle.UpdateParameters["groupId"].DefaultValue = ((int)UserGroups.BoardApprovalAuthority).ToString();
        }
    }
}
