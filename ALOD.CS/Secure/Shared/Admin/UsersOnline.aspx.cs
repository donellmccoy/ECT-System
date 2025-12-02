using System;
using System.Data;
using System.Web.UI.WebControls;
using ALOD.Core.Interfaces.DAOInterfaces;
using ALOD.Data;

namespace ALOD.Web.Admin
{
    public partial class Secure_Shared_Admin_UsersOnline : System.Web.UI.Page
    {
        protected GridView GridView1;
        protected ObjectDataSource dataUsers;

        protected void GridView1_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "ClearUser")
            {
                int userId = 0;
                int.TryParse(e.CommandArgument.ToString(), out userId);

                if (userId == 0)
                {
                    return;
                }

                IUserDao dao = new NHibernateDaoFactory().GetUserDao();
                dao.ClearOnlineUser(userId);

                GridView1.DataBind();
            }
        }

        protected void GridView1_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType != DataControlRowType.DataRow)
            {
                return;
            }

            DataRowView view = (DataRowView)e.Row.DataItem;
            TimeSpan span = DateTime.Now.Subtract((DateTime)view["loginTime"]);
            string text = string.Format("{0:00}:{1:00}", span.Hours, span.Minutes);
            ((Label)e.Row.FindControl("TimeLabel")).Text = text;
        }
    }
}
