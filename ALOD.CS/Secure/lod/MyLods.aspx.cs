using System;
using System.Data;
using System.Text;
using System.Web.UI.WebControls;
using ALOD.Core.Domain.Users;
using ALOD.Core.Domain.Workflow;
using ALOD.Core.Interfaces.DAOInterfaces;
using ALOD.Core.Utils;
using ALOD.Data;
using ALODWebUtility.Common;
using ALODWebUtility.Permission.Search;

namespace ALOD.Web.LOD
{
    public partial class Secure_lod_MyLods : System.Web.UI.Page
    {
        private NHibernateDaoFactory _daoFactory;
        private ILineOfDutyDao _lodDao;

        protected NHibernateDaoFactory DaoFactory
        {
            get
            {
                if (_daoFactory == null)
                {
                    _daoFactory = new NHibernateDaoFactory();
                }

                return _daoFactory;
            }
        }

        protected ILineOfDutyDao LODDao
        {
            get
            {
                if (_lodDao == null)
                {
                    _lodDao = DaoFactory.GetLineOfDutyDao();
                }

                return _lodDao;
            }
        }

        protected void gvResults_LODV3_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            HeaderRowBinding(sender, e, "CaseId");

            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                DataRowView data = (DataRowView)e.Row.DataItem;
                int lockId = 0;
                int.TryParse(data["lockId"].ToString(), out lockId);

                if (lockId > 0)
                {
                    ((Image)e.Row.FindControl("LockImage")).Visible = true;
                }

                if (IsSessionUserABoardTechnician())
                {
                    ((Label)e.Row.FindControl("lblReceivedFrom")).Text = LODDao.GetFromAndDirection(int.Parse(data["refId"].ToString()));
                }
            }
        }

        protected void gvResults_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            HeaderRowBinding(sender, e, "CaseId");

            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                DataRowView data = (DataRowView)e.Row.DataItem;
                int lockId = 0;
                int.TryParse(data["lockId"].ToString(), out lockId);

                if (lockId > 0)
                {
                    ((Image)e.Row.FindControl("LockImage")).Visible = true;
                }

                if (IsSessionUserABoardTechnician())
                {
                    ((Label)e.Row.FindControl("lblReceivedFrom")).Text = LODDao.GetFromAndDirection(int.Parse(data["refId"].ToString()));
                }
            }
        }

        protected bool IsSessionUserABoardTechnician()
        {
            if ((int)Session["GroupId"] == (int)ALOD.Core.Domain.Users.UserGroups.BoardTechnician)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        protected void LOD_V3()
        {
            switch (SESSION_GROUP_ID)
            {
                case UserGroups.MedicalTechnician:
                case UserGroups.MedicalOfficer:
                case UserGroups.UnitCommander:
                case UserGroups.WingJudgeAdvocate:
                case UserGroups.WingCommander:
                case UserGroups.WingSarc:
                case UserGroups.BoardLegal:
                case UserGroups.BoardMedical:
                case UserGroups.BoardTechnician:
                case UserGroups.BoardAdministrator:
                case UserGroups.BoardApprovalAuthority:
                    resultsUpdatePanel.Visible = false;
                    resultsUpdatePanel_IO.Visible = false;
                    break;
                case UserGroups.InvestigatingOfficer:
                    resultsUpdatePanel.Visible = false;
                    resultsUpdatePanel_LODV3.Visible = false;
                    break;
                default:
                    resultsUpdatePanel_LODV3.Visible = false;
                    resultsUpdatePanel_IO.Visible = false;
                    break;
            }
        }

        protected void LodData_LODV3_Selecting(object sender, ObjectDataSourceSelectingEventArgs e)
        {
            e.InputParameters["moduleId"] = (byte)ModuleType.LOD;
            e.InputParameters["sarcpermission"] = UserHasPermission(PERMISSION_VIEW_SARC_CASES);
        }

        protected void LodData_Selecting(object sender, ObjectDataSourceSelectingEventArgs e)
        {
            e.InputParameters["moduleId"] = (byte)ModuleType.LOD;
            e.InputParameters["sarcpermission"] = UserHasPermission(PERMISSION_VIEW_SARC_CASES);
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                // Clear any pending locks this user might have
                ICaseLockDao dao = new ALOD.Data.NHibernateDaoFactory().GetCaseLockDao();
                dao.ClearLocksForUser(SESSION_USER_ID);
                PageAsyncTask task = new PageAsyncTask(new BeginEventHandler(BeginAsyncUnitLookup), new EndEventHandler(EndAsyncUnitLookup), null, GetStateObject(UnitSelect));
                RegisterAsyncTask(task);
                LOD_V3();
                SetInputFormatRestriction(Page, SsnBox, FormatRestriction.Numeric, "-");
                SetInputFormatRestriction(Page, NameBox, FormatRestriction.AlphaNumeric, PERMITTED_SPECIAL_CHAR_INPUT);
                SetInputFormatRestriction(Page, CaseIdbox, FormatRestriction.AlphaNumeric, "-");
            }
            if (IsSessionUserABoardTechnician())
            {
                gvResults.Columns[5].Visible = true;
            }
            else
            {
                gvResults.Columns[5].Visible = false;
            }
        }

        private void gvResults_IO_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "view")
            {
                string[] parts = e.CommandArgument.ToString().Split(';');
                StringBuilder strQuery = new StringBuilder();
                ItemSelectedEventArgs args = new ItemSelectedEventArgs();
                args.RefId = int.Parse(parts[0]);

                strQuery.Append("refId=" + args.RefId.ToString());
                args.Type = ModuleType.LOD;

                switch (args.Type)
                {
                    case ModuleType.LOD:
                        args.Url = "~/Secure/Lod/init.aspx?" + strQuery.ToString();
                        Response.Redirect(args.Url);
                        break;
                }
            }
        }

        private void gvResults_LODV3_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "view")
            {
                string[] parts = e.CommandArgument.ToString().Split(';');
                StringBuilder strQuery = new StringBuilder();
                ItemSelectedEventArgs args = new ItemSelectedEventArgs();
                args.RefId = int.Parse(parts[0]);

                strQuery.Append("refId=" + args.RefId.ToString());
                args.Type = ModuleType.LOD;

                switch (args.Type)
                {
                    case ModuleType.LOD:
                        args.Url = "~/Secure/Lod/init.aspx?" + strQuery.ToString();
                        Response.Redirect(args.Url);
                        break;
                }
            }
        }

        private void gvResults_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "view")
            {
                string[] parts = e.CommandArgument.ToString().Split(';');
                StringBuilder strQuery = new StringBuilder();
                ItemSelectedEventArgs args = new ItemSelectedEventArgs();
                args.RefId = int.Parse(parts[0]);

                strQuery.Append("refId=" + args.RefId.ToString());
                args.Type = ModuleType.LOD;

                switch (args.Type)
                {
                    case ModuleType.LOD:
                        args.Url = "~/Secure/Lod/init.aspx?" + strQuery.ToString();
                        Response.Redirect(args.Url);
                        break;
                }
            }
        }
    }
}
