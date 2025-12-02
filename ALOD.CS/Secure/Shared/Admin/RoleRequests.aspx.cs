using System;
using System.Data;
using System.Web.UI.WebControls;
using ALOD.Data;

namespace ALOD.Web.Admin
{
    public partial class Secure_Shared_Admin_RoleRequests : System.Web.UI.Page
    {
        protected GridView RequestGrid;

        private const string ASCENDING = "Asc";
        private const string DESCENDING = "Desc";
        private const string SORT_COLUMN_KEY = "_SortExp_";
        private const string SORT_DIR_KEY = "_SortDirection_";

        private string SortColumn
        {
            get
            {
                if (ViewState[SORT_COLUMN_KEY] == null)
                {
                    ViewState[SORT_COLUMN_KEY] = "LastName";
                }
                return (string)ViewState[SORT_COLUMN_KEY];
            }
            set { ViewState[SORT_COLUMN_KEY] = value; }
        }

        private string SortDir
        {
            get
            {
                if (ViewState[SORT_DIR_KEY] == null)
                {
                    ViewState[SORT_DIR_KEY] = ASCENDING;
                }
                return (string)ViewState[SORT_DIR_KEY];
            }
            set { ViewState[SORT_DIR_KEY] = value; }
        }

        private string SortExpression
        {
            get { return SortColumn + " " + SortDir; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                Session.Remove("editId");
                InitRequestGrid();
            }
        }

        protected void RequestGrid_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "ProcessRequest")
            {
                Session["editId"] = int.Parse(e.CommandArgument.ToString());
                Response.Redirect("~/Secure/Shared/Admin/EditUser.aspx?caller=3", true);
            }
        }

        protected void RequestGrid_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            GridView grid = (GridView)sender;

            if (e.Row.RowType == DataControlRowType.Header)
            {
                int cellIndex = -1;

                foreach (DataControlField field in grid.Columns)
                {
                    if (field.SortExpression == SortColumn)
                    {
                        cellIndex = grid.Columns.IndexOf(field);
                    }
                }

                if (cellIndex > -1)
                {
                    if (SortDir == ASCENDING)
                    {
                        e.Row.Cells[cellIndex].CssClass = "gridViewHeader sort-asc";
                    }
                    else
                    {
                        e.Row.Cells[cellIndex].CssClass = "gridViewHeader sort-desc";
                    }
                }
            }
        }

        protected void RequestGrid_Sorting(object sender, GridViewSortEventArgs e)
        {
            if (SortColumn != "")
            {
                if (SortColumn == e.SortExpression)
                {
                    if (SortDir == ASCENDING)
                    {
                        SortDir = DESCENDING;
                    }
                    else
                    {
                        SortDir = ASCENDING;
                    }
                }
                else
                {
                    SortDir = ASCENDING;
                }
            }

            SortColumn = e.SortExpression;
            InitRequestGrid();
        }

        private void InitRequestGrid()
        {
            DataSet data = new NHibernateDaoFactory().GetUserRoleRequestDao().GetAllPendingRequests((int)Session["UserId"]);
            DataView view = new DataView(data.Tables[0]);

            if (SortColumn.Length > 0)
            {
                view.Sort = SortExpression;
            }

            RequestGrid.DataSource = view;
            RequestGrid.DataBind();
        }
    }
}
