using System;
using System.Web.UI.WebControls;
using ALOD.Core.Domain.Reports;
using ALOD.Core.Interfaces.DAOInterfaces;
using ALOD.Data;
using ALODWebUtility.Common;

namespace ALOD.Web.Admin
{
    public partial class ARCNetUserLookup : System.Web.UI.Page
    {
        protected Panel pnlLookupInput;
        protected TextBox txtEDIPI;
        protected TextBox txtUserLastName;
        protected TextBox txtUserFirstName;
        protected TextBox txtUserMiddleName;
        protected TextBox txtBeginDate;
        protected TextBox txtEndDate;
        protected Button btnSearch;
        protected Label lblLastImportExecutionDate;
        protected Panel pnlLookupOutput;
        protected GridView gdvResults;

        protected const double SUBTRACTDAYS = -30;

        private IARCNetDao _arcnetDao;

        protected IARCNetDao ARCNetDao
        {
            get
            {
                if (_arcnetDao == null)
                {
                    _arcnetDao = new NHibernateDaoFactory().GetARCNetDao();
                }
                return _arcnetDao;
            }
        }

        protected DateTime? BeginDate
        {
            get
            {
                if (txtBeginDate.Text.Length == 0)
                {
                    txtBeginDate.Text = DateTime.Now.AddDays(SUBTRACTDAYS).ToShortDateString();
                }
                return DateTime.Parse(Server.HtmlEncode(txtBeginDate.Text));
            }
        }

        protected string CalendarImage
        {
            get { return this.ResolveClientUrl("~/App_Themes/" + Page.Theme + "/Images/Calendar.gif"); }
        }

        protected string EDIPI
        {
            get
            {
                if (txtEDIPI.Text.Length == 0)
                {
                    return string.Empty;
                }
                return Server.HtmlEncode(txtEDIPI.Text);
            }
        }

        protected DateTime? EndDate
        {
            get
            {
                if (txtEndDate.Text.Length == 0)
                {
                    txtEndDate.Text = DateTime.Now.ToShortDateString();
                }

                DateTime theEndDate = DateTime.Parse(Server.HtmlEncode(txtEndDate.Text));

                if (theEndDate < BeginDate.Value)
                {
                    txtEndDate.Text = BeginDate.Value.AddDays(1).ToShortDateString();
                }

                if (theEndDate > BeginDate.Value.AddMonths(1))
                {
                    txtEndDate.Text = BeginDate.Value.AddMonths(1).ToShortDateString();
                }

                return DateTime.Parse(Server.HtmlEncode(txtEndDate.Text));
            }
        }

        protected string FirstName
        {
            get
            {
                if (txtUserFirstName.Text.Length == 0)
                {
                    return string.Empty;
                }
                return Server.HtmlEncode(txtUserFirstName.Text);
            }
        }

        protected string LastName
        {
            get
            {
                if (txtUserLastName.Text.Length == 0)
                {
                    return string.Empty;
                }
                return Server.HtmlEncode(txtUserLastName.Text);
            }
        }

        protected string MiddleNames
        {
            get
            {
                if (txtUserMiddleName.Text.Length == 0)
                {
                    return string.Empty;
                }
                return Server.HtmlEncode(txtUserMiddleName.Text);
            }
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            ExecuteLookup();
        }

        protected bool BypassDates()
        {
            if (!string.IsNullOrEmpty(EDIPI))
            {
                return true;
            }

            if (!string.IsNullOrEmpty(LastName) ||
                !string.IsNullOrEmpty(FirstName) ||
                !string.IsNullOrEmpty(MiddleNames))
            {
                return true;
            }

            return false;
        }

        protected ARCNetLookupReportArgs ConstructReportArguments()
        {
            ARCNetLookupReportArgs args = new ARCNetLookupReportArgs();

            args.EDIPIN = EDIPI;
            args.LastName = LastName;
            args.FirstName = FirstName;
            args.MiddleNames = MiddleNames;

            if (!BypassDates())
            {
                args.BeginDate = BeginDate.Value;
                args.EndDate = EndDate.Value;
            }

            return args;
        }

        protected void ExecuteLookup()
        {
            gdvResults.DataSource = ARCNetDao.GetIAATrainingDataForUsers(ConstructReportArguments());
            gdvResults.DataBind();

            pnlLookupOutput.Visible = true;
        }

        protected void gdvResults_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gdvResults.PageIndex = e.NewPageIndex;
            ExecuteLookup();
        }

        protected void gdvResults_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName.Equals("EditUser"))
            {
                Session["EditId"] = e.CommandArgument;
                Response.Redirect("~/Secure/Shared/Admin/EditUser.aspx?Caller=1", true);
            }
        }

        protected void InitControls()
        {
            InitLastExecutionDateLabel();
        }

        protected void InitLastExecutionDateLabel()
        {
            DateTime? executionDate = ARCNetDao.GetARCNetImportLastExecutionDate();

            if (executionDate.HasValue)
            {
                lblLastImportExecutionDate.Text = executionDate.Value.ToString();
            }
            else
            {
                lblLastImportExecutionDate.Text = "UNKNOWN";
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            InitControls();
            Session["EditId"] = null;
        }
    }
}
