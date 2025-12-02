using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using ALOD.Data.Services;
using static ALODWebUtility.Common.SessionInfo;
using static ALODWebUtility.Common.Utility;

namespace ALOD.Web.Admin
{
    public partial class Secure_Shared_Admin_PermissionReport : System.Web.UI.Page
    {
        protected DropDownList PermissionsSelect;
        protected Button SearchButton;
        protected Button ExportButton;
        protected Panel SearchMessage;
        protected GridView UsersGrid;
        protected ObjectDataSource UserData;
        protected ObjectDataSource Permissions;

        #region Load

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                PopulatePermissions();

                if (Session["PermissionUserSrchPara"] != null)
                {
                    Dictionary<string, string> srchParam = (Dictionary<string, string>)Session["PermissionUserSrchPara"];
                    PermissionsSelect.SelectedValue = srchParam["perm"];
                }

                UsersGrid.Sort("Name", SortDirection.Ascending);
            }

            Session["EditId"] = null;
            Session["PermissionUserSrchPara"] = null;
        }

        protected void Permissions_Selecting(object sender, ObjectDataSourceSelectingEventArgs e)
        {
            string perm = Server.HtmlEncode(PermissionsSelect.SelectedValue);

            if (perm.Length == 0)
            {
                e.Cancel = true;
                SearchMessage.Visible = true;
                UsersGrid.CssClass = "hidden";
            }
            else
            {
                e.Cancel = false;
                SearchMessage.Visible = false;
                UsersGrid.CssClass = "gridViewMain";
            }
        }

        protected void PopulatePermissions()
        {
            var perms = from w in WorkFlowService.GetPermissions() where w.Exclude == false orderby w.Description select w;
            PermissionsSelect.DataSource = perms;
            PermissionsSelect.DataBind();
            PermissionsSelect.Items.Insert(0, new ListItem("-Select-", ""));
        }

        protected void SearchButton_Click(object sender, EventArgs e)
        {
            UsersGrid.DataBind();
        }

        #endregion

        #region EditUser

        protected Dictionary<string, string> GetSearchParameters()
        {
            string perm = Server.HtmlEncode(PermissionsSelect.SelectedValue);
            Dictionary<string, string> srchParam = new Dictionary<string, string>();
            srchParam.Add("perm", perm);
            return srchParam;
        }

        protected void UsersGrid_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            // Method implementation commented out in original
        }

        #endregion

        #region Sorting

        protected void UsersGrid_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            HeaderRowBinding((GridView)sender, e, "Name");
        }

        #endregion

        #region Export

        protected void AddTableCell(HtmlTableRow row, string text)
        {
            HtmlTableCell cell = new HtmlTableCell();
            cell.InnerHtml = text;
            row.Cells.Add(cell);
        }

        protected void btnExport_Click(object sender, EventArgs e)
        {
            string perm = Server.HtmlEncode(PermissionsSelect.SelectedValue);

            DataSet ds;
            if (perm.Length > 0)
            {
                Response.Clear();

                bool odd = false;
                string permName = Server.HtmlEncode(PermissionsSelect.SelectedItem.Text);
                ds = UserService.GetUsersWithPermission(short.Parse(perm));
                Response.AddHeader("content-disposition", "attachment;filename=ALodUsersWithPerm_" + permName + "_" + DateTime.Now.ToString("yyyyMMdd") + ".xls");
                Response.Charset = "";
                Response.ContentType = "application/ms-excel";

                HtmlTable table = new HtmlTable();

                // add the header row
                HtmlTableRow row = new HtmlTableRow();
                row.Attributes.Add("style", "background-color:navy;text-align: center; border: 1px solid #CCC; border-bottom: solid 1px black; color:white");

                // add the header cells
                AddTableCell(row, "NAME");
                AddTableCell(row, "SSN");
                AddTableCell(row, "ROLE");
                AddTableCell(row, "UNIT");
                AddTableCell(row, "STATUS");

                table.Rows.Add(row);

                foreach (DataRow item in ds.Tables[0].Rows)
                {
                    row = new HtmlTableRow();
                    string bgColor = odd ? "#d8d8ff;" : "#FFF;";
                    odd = !odd;

                    row.Attributes.Add("style", "border: 1px solid #CCC; border-bottom: solid 1px #C0C0C0; background-color:" + bgColor);
                    table.Rows.Add(row);
                    AddTableCell(row, item["Name"].ToString());
                    AddTableCell(row, item["LastFour"].ToString());
                    AddTableCell(row, item["RoleName"].ToString());
                    AddTableCell(row, item["CurrentUnitName"].ToString());
                    AddTableCell(row, item["AccessStatusDescr"].ToString());
                }

                StringWriter writer = new StringWriter();
                HtmlTextWriter html = new HtmlTextWriter(writer);

                table.RenderControl(html);
                Response.Write(writer.ToString());
                Response.End();
            }
        }

        #endregion
    }
}
