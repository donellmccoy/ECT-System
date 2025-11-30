Imports ALOD.Core.Domain.Reports
Imports ALOD.Core.Interfaces.DAOInterfaces
Imports ALOD.Core.Utils
Imports ALOD.Data
Imports ALOD.Data.Services
Imports ALODWebUtility.Common

Namespace Web.Reports

    Partial Class Secure_Reports_MetricsReport
        Inherits System.Web.UI.Page
        Protected Const COMMAND_VIEW_UNIT As String = "VIEW_UNIT"
        Protected Const MSG_NO_SUB_UNITS As String = "No sub units found"
        Protected _reportsDao As IReportsDao

        Protected ReadOnly Property CalendarImage() As String
            Get
                Return Me.ResolveClientUrl("~/App_Themes/" + Page.Theme + "/Images/Calendar.gif")
            End Get
        End Property

        Protected Property FeedBackMessage() As String
            Get
                If (ViewState(KEY_NO_SUB_UNITS_MESSAGE) Is Nothing) Then
                    Return String.Empty
                End If
                Return CStr(ViewState(KEY_NO_SUB_UNITS_MESSAGE))
            End Get
            Set(ByVal value As String)
                ViewState(KEY_NO_SUB_UNITS_MESSAGE) = value
            End Set
        End Property

        Protected ReadOnly Property ReportBeginDate() As Date
            Get
                Dim theDate As Date
                Date.TryParse(BeginDateBox.Text.Trim.Trim, theDate)

                If (theDate.Ticks = 0) Then
                    theDate = Now.AddDays(-30)
                    BeginDateBox.Text = theDate.ToString(DATE_FORMAT)
                End If

                Return Server.HtmlEncode(theDate)

            End Get
        End Property

        Protected ReadOnly Property ReportEndDate() As Date
            Get
                Dim theDate As Date
                Date.TryParse(EndDateBox.Text.Trim.Trim, theDate)

                If (theDate.Ticks = 0) Then
                    theDate = Now
                    EndDateBox.Text = theDate.ToString(DATE_FORMAT)
                End If

                Return Server.HtmlEncode(theDate)

            End Get
        End Property

        Protected ReadOnly Property ReportsDao As IReportsDao
            Get
                If (_reportsDao Is Nothing) Then
                    _reportsDao = New NHibernateDaoFactory().GetReportsDao()
                End If

                Return _reportsDao
            End Get
        End Property

        Protected Function ConstructReportArguments() As LODAvgTimesMetricsReportArgs
            Dim args As LODAvgTimesMetricsReportArgs = New LODAvgTimesMetricsReportArgs()

            args.SSN = String.Empty
            args.UnitId = Integer.Parse(UnitSelect.SelectedValue)
            args.BeginDate = ReportBeginDate
            args.EndDate = ReportEndDate
            args.ReportView = CByte(SESSION_REPORT_VIEW)
            args.IsComplete = Byte.Parse(StatusSelect.SelectedValue)
            args.IncludeSubordinateUnits = IncludeSubordinatesCheck.Checked

            Return args
        End Function

        Protected Function GetReportData() As DataSet
            Return ReportsDao.ExecuteLODAvgTimesMetricsReport(ConstructReportArguments())
        End Function

        Protected Sub InitControls()
            SetInputFormatRestriction(Page, BeginDateBox, FormatRestriction.Numeric, "/")
            SetInputFormatRestriction(Page, EndDateBox, FormatRestriction.Numeric, "/")
            InitUnitSelect()
        End Sub

        Protected Sub InitUnitSelect()

            Dim units = From u In LookupService.GetChildUnits(SESSION_UNIT_ID, SESSION_REPORT_VIEW)
                        Select u
                        Order By u.Name

            UnitSelect.DataSource = units
            UnitSelect.DataBind()

            SetDropdownByValue(UnitSelect, SESSION_UNIT_ID)

        End Sub

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

            If (Not IsPostBack) Then
                InitControls()
            End If

        End Sub

        Protected Sub Page_PreRender(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.PreRender

            FeedbackPanel.Visible = FeedBackMessage.Length > 0
            FeedbackMessageLabel.Text = FeedBackMessage

        End Sub

        Protected Sub ReportButton_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles ReportButton.Click
            UpdateReportGrid()
        End Sub

        Protected Sub ResultsGrid_RowCommand(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewCommandEventArgs) Handles ResultsGrid.RowCommand

            If (e.CommandName = COMMAND_VIEW_UNIT) Then
                SetDropdownByValue(UnitSelect, e.CommandArgument)
                UpdateReportGrid()
            End If

        End Sub

        Protected Sub ResultsGrid_RowDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewRowEventArgs) Handles ResultsGrid.RowDataBound

            If (e.Row.RowType <> DataControlRowType.DataRow) Then
                Exit Sub
            End If

            Dim data As DataRowView = CType(e.Row.DataItem, DataRowView)

            If CInt(data("cs_id")) = UnitSelect.SelectedValue Then
                'e.Row.Visible = False
            End If
            Dim total As Decimal = 0

            Dim days As Decimal = 0
            Decimal.TryParse(data("med_tech"), days)
            total += days

            Decimal.TryParse(data("med_off"), days)
            total += days

            Decimal.TryParse(data("unit_cmdr"), days)
            total += days

            Decimal.TryParse(data("wing_ja"), days)
            total += days

            Decimal.TryParse(data("wing_cc"), days)
            total += days

            Decimal.TryParse(data("board"), days)
            total += days

            Decimal.TryParse(data("invest"), days)
            total += days

            Dim totalLabel As Label = CType(e.Row.FindControl("TotalLabel"), Label)
            totalLabel.Text = total.ToString("N2")

        End Sub

        Protected Sub UpdateReportGrid()
            ResultsPanel.Visible = True
            ResultsGrid.DataSource = GetReportData()
            ResultsGrid.DataBind()

            ResultsGrid.Visible = True
            FeedBackMessage = String.Empty
        End Sub

    End Class

End Namespace