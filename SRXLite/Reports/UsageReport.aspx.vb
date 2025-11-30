Option Strict On

Imports SRXLite.Classes
Imports SRXLite.DataAccess
Imports SRXLite.DataTypes
Imports SRXLite.Modules

Namespace Web.Reports

    Partial Class Reports_UsageReport
        Inherits System.Web.UI.Page

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

            If Not IsPostBack Then
                UsageSummary_DateStart.Value = Today.AddDays(-7).ToShortDateString
                UsageSummary_DateEnd.Value = Today.ToShortDateString

                RequestStats_DateStart.Value = Today.AddDays(-7).ToShortDateString
                RequestStats_DateEnd.Value = Today.ToShortDateString

                UsageSummary_ddlCategory.DataSource = System.Enum.GetNames(GetType(Category))
                UsageSummary_ddlCategory.DataBind()
                UsageSummary_ddlCategory.Items.Insert(0, New ListItem("-- All --", "0"))

                UsageSummary_ddlUser.DataSource = DB.ExecuteDataset("select userid, username from users")
                UsageSummary_ddlUser.DataTextField = "username"
                UsageSummary_ddlUser.DataValueField = "userid"
                UsageSummary_ddlUser.DataBind()
                UsageSummary_ddlUser.Items.Insert(0, New ListItem("-- All --", "0"))
            End If
        End Sub

        Protected Sub btnViewUsageSummary_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnViewUsageSummary.Click
            Dim userID As Short = ShortCheck(UsageSummary_ddlUser.SelectedValue)
            Dim startDate, endDate As Date
            Dim catg As Category
            startDate = UsageSummary_DateStart.SelectedDate
            endDate = UsageSummary_DateEnd.SelectedDate
            catg = CType(System.Enum.Parse(GetType(Category), UsageSummary_ddlCategory.SelectedValue), Category)

            Dim report As New Report(userID)
            gvUsageSummary.DataSource = report.GetUsageSummary(startDate, endDate, catg)
            gvUsageSummary.DataBind()


            'TEST chart ---------------------------------------------
            ' Populate series data
            chartUsageSummary.DataSource = report.GetUsageSummary(startDate, endDate, catg)
            chartUsageSummary.Series("Series1").XValueMember = "ActionTypeName"
            chartUsageSummary.Series("Series1").YValueMembers = "Count"
            chartUsageSummary.DataBind()

            ' Set Doughnut chart type
            'chartUsageSummary.Series("Default").Type = SeriesChartType.Doughnut
            chartUsageSummary.Series("Series1").ChartType = DataVisualization.Charting.SeriesChartType.Pie

            ' Set labels style
            chartUsageSummary.Series("Series1")("PieLabelStyle") = "Disabled"

            ' Set Doughnut radius percentage
            chartUsageSummary.Series("Series1")("DoughnutRadius") = "30"

            ' Explode data point with label "Italy"
            chartUsageSummary.Series("Series1").Points(4)("Exploded") = "true"

            ' Enable 3D
            chartUsageSummary.ChartAreas("ChartArea1").Area3DStyle.Enable3D = True
            'chartUsageSummary.Height = 500
            'chartUsageSummary.Width = 500

            ' Disable the Legend
            chartUsageSummary.Legends.Add("legend1")

        End Sub

        Private _maxDocCount As Integer = 0
        Private _maxPageCount As Integer = 0
        Protected Sub btnViewRequestStats_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnViewRequestStats.Click
            Dim startDate, endDate As Date
            startDate = RequestStats_DateStart.SelectedDate
            endDate = RequestStats_DateEnd.SelectedDate

            Dim report As New Report()
            Dim list As List(Of Report.GuidStatsData) = report.GetRequestStatistics(startDate, endDate)
            Dim item As Report.GuidStatsData
            For i As Integer = 0 To list.Count - 1
                item = list(i)
                If item.DocGuidCount > _maxDocCount Then _maxDocCount = item.DocGuidCount
                If item.DocPageGuidCount > _maxPageCount Then _maxPageCount = item.DocPageGuidCount
            Next
            gvRequestStats.DataSource = list
            gvRequestStats.DataBind()
        End Sub

        Protected Sub gvRequestStats_RowDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewRowEventArgs) Handles gvRequestStats.RowDataBound
            If e.Row.RowType = DataControlRowType.DataRow Then
                Dim divDocs As HtmlGenericControl = CType(e.Row.Cells(3).FindControl("divDocs"), HtmlGenericControl)
                Dim divPages As HtmlGenericControl = CType(e.Row.Cells(3).FindControl("divPages"), HtmlGenericControl)

                divDocs.Style.Add("width", Math.Ceiling(CInt(e.Row.Cells(1).Text) / Math.Max(_maxDocCount, 1) * 100) & "%")
                divPages.Style.Add("width", Math.Ceiling(CInt(e.Row.Cells(2).Text) / Math.Max(_maxPageCount, 1) * 100) & "%")

            End If
        End Sub

    End Class

End Namespace