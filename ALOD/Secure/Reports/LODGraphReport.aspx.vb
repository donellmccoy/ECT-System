Imports System.Web.UI.DataVisualization.Charting
Imports ALOD.Core.Domain.Reports
Imports ALOD.Core.Interfaces.DAOInterfaces
Imports ALOD.Core.Utils
Imports ALOD.Data
Imports ALODWebUtility.Common

Namespace Web.Reports

    Partial Class Secure_Reports_ScatteredGraphReport
        Inherits System.Web.UI.Page

        Public interval As Integer
        Public targetOffset As Integer
        Public targetValue As Integer
        Public years(3) As String

        Private _reportsDao As IReportsDao

        Protected ReadOnly Property CalendarImage() As String
            Get
                Return Me.ResolveClientUrl("~/App_Themes/" + Page.Theme + "/Images/Calendar.gif")
            End Get
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

        Protected Sub BeginGraph()
            Dim data As DataSet
            Dim isFormal As Boolean = IsFormalCheckbox.Checked
            Dim stripWidth As Double
            Dim BeginDate As Date = ReportBeginDate()
            Dim EndDate As Date = ReportEndDate()

            ResultChart1.ChartAreas("ChartArea").AxisX.MajorGrid.Enabled = False

            'Default Settings
            If isFormal = False Then
                targetValue = 50
                targetOffset = 200
                stripWidth = 1.3
            Else
                targetValue = 70
                targetOffset = 230
                stripWidth = 1.7
            End If

            If RB1.Checked Then
                interval = 1
            ElseIf RB5.Checked Then
                interval = 5
            ElseIf RB10.Checked Then
                interval = 10
            ElseIf RB20.Checked Then
                interval = 20
            End If

            'Year Entered
            years(0) = EndDate.Year.ToString()

            ResultChart1.ChartAreas("ChartArea").AxisX.Interval = 20
            ResultChart1.Series("Year1").Name = years(0).ToString()

            Try
                data = ReportsDao.ExecuteLODGraphReport(ConstructReportArgs(ReportBeginDate(), ReportEndDate(), isFormal))
            Catch ex As Exception
                ErrorMessageLabel.Text = "Please make sure the dates are valid."
                MaxValueLabel.Text = ""
                AverageLabel.Text = ""
                TotalCasesLabel.Text = ""
                TargetLabel.Text = ""
                Return
            End Try

            If data.Tables(0).Rows.Count <= 0 Then
                ErrorMessageLabel.Text = "No result returned. Please make sure dates are valid"
                MaxValueLabel.Text = ""
                AverageLabel.Text = ""
                TotalCasesLabel.Text = ""
                TargetLabel.Text = ""
                Return
            End If

            ErrorMessageLabel.Text = ""

            ProcessData(data, years(0))

            If PrevYears.Checked Then

                years(1) = EndDate.AddYears(-1).Year.ToString()
                years(2) = EndDate.AddYears(-2).Year.ToString()

                ResultChart1.Series("Year2").Name = years(1)
                ResultChart1.Series("Year3").Name = years(2)

                For index = 1 To 2 Step 1
                    Dim bDate As Date = BeginDate.AddYears(-index)
                    Dim eDate As Date = EndDate.AddYears(-index)

                    Try
                        data = ReportsDao.ExecuteLODGraphReport(ConstructReportArgs(bDate, eDate, isFormal))
                    Catch ex As Exception
                        ErrorMessageLabel.Text = "Please make sure the dates are valid."
                        MaxValueLabel.Text = ""
                        AverageLabel.Text = ""
                        TotalCasesLabel.Text = ""
                        TargetLabel.Text = ""
                        Return
                    End Try

                    ProcessData(data, years(index))
                Next
            End If

            'Display Target vertical line
            Dim stripLine As DataVisualization.Charting.StripLine = New DataVisualization.Charting.StripLine
            stripLine.Interval = 0
            stripLine.IntervalOffset = targetValue
            stripLine.StripWidth = stripWidth
            stripLine.BackColor = Drawing.Color.Red
            ResultChart1.ChartAreas("ChartArea").AxisX.StripLines.Add(stripLine)

            'Show Legend
            ResultChart1.Legends.Add(New Legend("Legend"))
            ResultChart1.Legends("Legend").DockedToChartArea = "ChartArea"
            ResultChart1.Series(years(0).ToString()).Legend = "Legend"
            ResultChart1.Series(years(0).ToString()).IsVisibleInLegend = True

            If PrevYears.Checked = False Then
                ResultChart1.Series("Year2").IsVisibleInLegend = False
                ResultChart1.Series("Year3").IsVisibleInLegend = False
            End If
        End Sub

        Protected Function ConstructReportArgs(ByVal beginDate As Date, ByVal endDate As Date, ByVal isFormal As Boolean) As LODGraphReportArgs
            Dim args As LODGraphReportArgs = New LODGraphReportArgs()

            args.BeginDate = beginDate
            args.EndDate = endDate
            args.IsFormal = isFormal

            Return args
        End Function

        Protected Sub InitControls()
            SetInputFormatRestriction(Page, BeginDateBox, FormatRestriction.Numeric, "/")
            SetInputFormatRestriction(Page, EndDateBox, FormatRestriction.Numeric, "/")
        End Sub

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
            If (Not IsPostBack) Then
                InitControls()
            End If
        End Sub

        Protected Sub PopulateGraph(arr As Array, intervl As Integer, series As String)
            Dim intervalAverage(intervl - 1) As Integer

            'Plot Point 0
            ResultChart1.Series(series).Points.AddXY(0, arr(0))

            'Plotting points. Also avergaing if Interval is not 1
            For index As Integer = 1 To arr.Length - 1 Step 1
                If intervl = 1 Then
                    ResultChart1.Series(series).Points.AddXY(index, arr(index))
                ElseIf index Mod intervl <> 0 Then
                    intervalAverage((index Mod intervl) - 1) = arr(index)
                Else
                    intervalAverage(intervl - 1) = arr(index)
                    ResultChart1.Series(series).Points.AddXY(index, intervalAverage.Average())
                End If
            Next
        End Sub

        Protected Sub ProcessData(data As DataSet, series As String)
            Dim targetCounter As Integer = 0
            Dim maxValue As Integer
            Dim indexOfMax As Integer
            Dim averageIndex As Integer
            Dim totalNumber As Integer
            Dim totalForAveraging As Integer
            Dim averageCount As Integer
            Dim arraySize As Integer

            'Figure out what should be the Horizontal range of the Graph based on input
            If XAll.Checked = True And series = years(0).ToString() Then
                arraySize = (Convert.ToInt32(data.Tables(1).Rows(0)("maxNumber")))
                XRange.Text = (Convert.ToInt32(data.Tables(1).Rows(0)("maxNumber")))
            Else
                If String.IsNullOrWhiteSpace(XRange.Text) Then
                    arraySize = targetValue + targetOffset
                    If series = years(0).ToString() Then
                        XRange.Text = targetValue + targetOffset
                    End If
                Else
                    If (Convert.ToInt32(XRange.Text.ToString()) >= 50) Then
                        arraySize = Convert.ToInt32(XRange.Text.ToString())
                    Else
                        arraySize = targetValue + targetOffset
                        If series = years(0).ToString() Then
                            XRange.Text = targetValue + targetOffset
                        End If
                    End If
                End If
            End If

            If arraySize Mod interval <> 0 And series = years(0).ToString() Then
                arraySize = (interval - arraySize Mod interval) + arraySize
                XRange.Text = arraySize
            End If

            Dim yValues(arraySize) As Integer
            Dim findMax(arraySize) As Integer

            'Populate yValues() with data
            For Each dataRow As DataRow In data.Tables(0).Rows
                If Convert.ToInt32(dataRow("daysToComplete").ToString()) <= arraySize Then

                    yValues(Convert.ToInt32(dataRow("daysToComplete").ToString())) += 1

                    findMax(Convert.ToInt32(dataRow("daysToComplete").ToString())) += 1
                    totalForAveraging += Convert.ToInt32(dataRow("daysToComplete").ToString())
                    averageCount += 1

                    If Convert.ToInt32(dataRow("daysToComplete").ToString()) <= targetValue Then
                        targetCounter += 1
                    End If
                    totalNumber += 1
                End If
            Next

            'Information only for first year
            If series = years(0).ToString() Then

                maxValue = findMax.Max()
                indexOfMax = Array.IndexOf(findMax, maxValue)
                averageIndex = totalForAveraging / averageCount

                MaxValueLabel.Text = "Peak: <b>" & maxValue & "</b> at " & indexOfMax & " Days"
                AverageLabel.Text = "Average Days to Complete: <b>" & averageIndex & "</b>"
                TotalCasesLabel.Text = "Total Cases Found in Report: <b>" & totalNumber & "</b>"
                TargetLabel.Text = "Percentage That Met Target: <b>%" & (targetCounter / totalNumber * 100).ToString("####0.")
            End If

            PopulateGraph(yValues, interval, series)
        End Sub

        Protected Sub ReportButton_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles ReportButton.Click
            BeginGraph()
        End Sub

    End Class

End Namespace