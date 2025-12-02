using System;
using System.Data;
using System.Web.UI.WebControls;
using ALOD.Core.Domain.Workflow;
using ALOD.Core.Interfaces.DAOInterfaces;
using ALOD.Data;

namespace ALOD.Web.Admin
{
    public partial class Secure_Shared_Admin_CaseLocks : System.Web.UI.Page
    {
        protected GridView LockGrid;

        protected void LockGrid_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "DeleteLock")
            {
                int lockId = 0;
                int.TryParse(e.CommandArgument.ToString(), out lockId);

                if (lockId > 0)
                {
                    ICaseLockDao dao = new NHibernateDaoFactory().GetCaseLockDao();
                    CaseLock caseLock = dao.GetById(lockId);
                    dao.Delete(caseLock);
                    dao.CommitChanges();
                    UpdateCaseGrid();
                }
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                UpdateCaseGrid();
            }
        }

        protected void UpdateCaseGrid()
        {
            ICaseLockDao dao = new NHibernateDaoFactory().GetCaseLockDao();
            DataSet data = dao.GetAll();

            LockGrid.DataSource = data;
            LockGrid.DataBind();
        }
    }
}
