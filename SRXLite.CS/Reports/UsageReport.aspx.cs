using System;
using System.Collections.Generic;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using SRXLite.Classes;
using SRXLite.DataAccess;
using SRXLite.DataTypes;
using static SRXLite.Modules.Util;

namespace SRXLite.Web.Reports
{
    public partial class Reports_UsageReport : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                UsageSummary_DateStart.Value = DateTime.Today.AddDays(-7).ToShortDateString();
                UsageSummary_DateEnd.Value = DateTime.Today.ToShortDateString();

                RequestStats_DateStart.Value = DateTime.Today.AddDays(-7).ToShortDateString();
                RequestStats_DateEnd.Value = DateTime.Today.ToShortDateString();

                UsageSummary_ddlCategory.DataSource = System.Enum.GetNames(typeof(Category));
                UsageSummary_ddlCategory.DataBind();
                UsageSummary_ddlCategory.Items.Insert(0, new ListItem("-- All --", "0"));

                UsageSummary_ddlUser.DataSource = DB.ExecuteDataset("select userid, username from users");
                UsageSummary_ddlUser.DataTextField = "username";
                UsageSummary_ddlUser.DataValueField = "userid";
                UsageSummary_ddlUser.DataBind();
                UsageSummary_ddlUser.Items.Insert(0, new ListItem("-- All --", "0"));
            }
        }

        protected void btnViewUsageSummary_Click(object sender, EventArgs e)
        {
            short userID = ShortCheck(UsageSummary_ddlUser.SelectedValue);
            DateTime startDate, endDate;
            Category catg;
            startDate = UsageSummary_DateStart.SelectedDate;
            endDate = UsageSummary_DateEnd.SelectedDate;
            catg = (Category)System.Enum.Parse(typeof(Category), UsageSummary_ddlCategory.SelectedValue);

            Report report = new Report(userID);
            gvUsageSummary.DataSource = report.GetUsageSummary(startDate, endDate, catg);
            gvUsageSummary.DataBind();

            // TEST chart ---------------------------------------------
            // Populate series data
            chartUsageSummary.DataSource = report.GetUsageSummary(startDate, endDate, catg);
            chartUsageSummary.Series["Series1"].XValueMember = "ActionTypeName";
            chartUsageSummary.Series["Series1"].YValueMembers = "Count";
            chartUsageSummary.DataBind();

            // Set Doughnut chart type
            chartUsageSummary.Series["Series1"].ChartType = System.Web.UI.DataVisualization.Charting.SeriesChartType.Pie;

            // Set labels style
            chartUsageSummary.Series["Series1"]["PieLabelStyle"] = "Disabled";

            // Set Doughnut radius percentage
            chartUsageSummary.Series["Series1"]["DoughnutRadius"] = "30";

            // Explode data point with label "Italy"
            chartUsageSummary.Series["Series1"].Points[4]["Exploded"] = "true";

            // Enable 3D
            chartUsageSummary.ChartAreas["ChartArea1"].Area3DStyle.Enable3D = true;

            // Disable the Legend
            chartUsageSummary.Legends.Add("legend1");
        }

        private int _maxDocCount = 0;
        private int _maxPageCount = 0;

        protected void btnViewRequestStats_Click(object sender, EventArgs e)
        {
            DateTime startDate, endDate;
            startDate = RequestStats_DateStart.SelectedDate;
            endDate = RequestStats_DateEnd.SelectedDate;

            Report report = new Report();
            List<Report.GuidStatsData> list = report.GetRequestStatistics(startDate, endDate);
            Report.GuidStatsData item;
            for (int i = 0; i < list.Count; i++)
            {
                item = list[i];
                if (item.DocGuidCount > _maxDocCount) _maxDocCount = item.DocGuidCount;
                if (item.DocPageGuidCount > _maxPageCount) _maxPageCount = item.DocPageGuidCount;
            }
            gvRequestStats.DataSource = list;
            gvRequestStats.DataBind();
        }

        protected void gvRequestStats_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                HtmlGenericControl divDocs = (HtmlGenericControl)e.Row.Cells[3].FindControl("divDocs");
                HtmlGenericControl divPages = (HtmlGenericControl)e.Row.Cells[3].FindControl("divPages");

                divDocs.Style.Add("width", Math.Ceiling(int.Parse(e.Row.Cells[1].Text) / (double)Math.Max(_maxDocCount, 1) * 100) + "%");
                divPages.Style.Add("width", Math.Ceiling(int.Parse(e.Row.Cells[2].Text) / (double)Math.Max(_maxPageCount, 1) * 100) + "%");
            }
        }
    }
}
