Imports ALOD.Core.Domain.Documents
Imports ALOD.Core.Domain.Reports
Imports ALOD.Core.Domain.Workflow
Imports ALOD.Core.Interfaces.DAOInterfaces
Imports ALOD.Data
Imports ALODWebUtility.Common
Imports WebSupergoo.ABCpdf8

Namespace Web.Reports

    Public Class LODProgramStatusReport
        Inherits System.Web.UI.Page

        Protected Const EXCEL_DATA_ROW_STYLE As String = "border: 1px solid #CCC; border-bottom: solid 1px #C0C0C0; background-color:"
        Protected Const EXCEL_HEADER_ROW_STYLE As String = "background-color:navy;text-align: center; border: 1px solid #CCC; border-bottom: solid 1px black; color:white;"
        Protected Const EXCEL_TEXT_ALIGN_STYLE As String = "text-align:right;"
        Protected Const EXCEL_TEXT_BOLD_STYLE As String = "font-weight:bold;"
        Protected Const EXCEL_TEXT_NOWRAP_STYLE As String = "white-space:nowrap;"
        Protected Const EXCEL_TEXT_ORANGE_STYLE As String = "color:orange;"
        Protected Const EXCEL_TEXT_RED_STYLE As String = "color:red;"
        Protected Const GRIDCOMMAND_VIEW_UNIT As String = "VIEW_UNIT"
        Protected Const VWS_CURRENT_UNIT As String = "CURRENT_UNIT"
        Protected Const VWS_UNIT_CHAIN As String = "UNIT_CHAIN"

        Private _report As ALOD.Core.Domain.Reports.LODProgramStatusReport
        Private _reportsDao As IReportsDao
        Private _unitChain As Dictionary(Of Integer, String)
        Private _unitDao As IUnitDao

        Protected Property CurrentUnitId As Integer
            Get
                If (ViewState(VWS_CURRENT_UNIT) Is Nothing) Then
                    ViewState(VWS_CURRENT_UNIT) = SESSION_UNIT_ID
                End If

                Return CStr(ViewState(VWS_CURRENT_UNIT))
            End Get
            Set(value As Integer)
                ViewState(VWS_CURRENT_UNIT) = value
            End Set
        End Property

        Protected ReadOnly Property Report As ALOD.Core.Domain.Reports.LODProgramStatusReport
            Get
                If (_report Is Nothing) Then
                    _report = New ALOD.Core.Domain.Reports.LODProgramStatusReport(New NHibernateDaoFactory())
                End If

                Return _report
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

        Protected Property StoredUnitChain As Dictionary(Of Integer, String)
            Get
                If (ViewState(VWS_UNIT_CHAIN) Is Nothing) Then
                    ViewState(VWS_UNIT_CHAIN) = New Dictionary(Of Integer, String)()
                End If

                Return ViewState(VWS_UNIT_CHAIN)
            End Get
            Set(value As Dictionary(Of Integer, String))
                ViewState(VWS_UNIT_CHAIN) = value
            End Set
        End Property

        Protected Property UnitChain As Dictionary(Of Integer, String)
            Get
                If (_unitChain Is Nothing) Then
                    _unitChain = New Dictionary(Of Integer, String)()
                End If

                Return _unitChain
            End Get
            Set(value As Dictionary(Of Integer, String))
                _unitChain = value
            End Set
        End Property

        Protected ReadOnly Property UnitDao As IUnitDao
            Get
                If (_unitDao Is Nothing) Then
                    _unitDao = New NHibernateDaoFactory().GetUnitDao()
                End If

                Return _unitDao
            End Get
        End Property

        Protected Sub btnRunReport_Click(sender As Object, e As EventArgs) Handles btnRunReport.Click
            CurrentUnitId = ReportUtility.GetUserStartingUnitId(SESSION_GROUP_ID, SESSION_UNIT_ID, UnitDao)

            UpdateUnitChainNavigator(True)

            RunReport()
        End Sub

        Protected Function ConstructReportArguments(ByVal unitId As Integer, ByVal groupByChildUnits As Boolean) As LODProgramStatusReportArgs
            Dim args As LODProgramStatusReportArgs = New LODProgramStatusReportArgs()

            args.Quarter = Integer.Parse(ddlQuarter.SelectedValue)

            If (ddlYear.SelectedValue > 0) Then
                args.Year = Integer.Parse(ddlYear.SelectedValue)
            ElseIf (args.Quarter = 0 OrElse args.Quarter >= GetCurrentQuarter()) Then
                args.Year = DateTime.Now.Year - 1
                ddlYear.SelectedValue = DateTime.Now.Year - 1
                ddlYear_SelectedIndexChanged(Nothing, Nothing)
            Else
                args.Year = DateTime.Now.Year
                ddlYear.SelectedValue = DateTime.Now.Year
                ddlYear_SelectedIndexChanged(Nothing, Nothing)
            End If

            args.UnitId = unitId
            args.ViewType = ReportingView.Non_Medical_Reporting_View
            args.GroupByChildUnits = groupByChildUnits

            Return args
        End Function

        Protected Sub ddlQuarter_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ddlQuarter.SelectedIndexChanged
            Dim currentYearSelection As String = ddlYear.SelectedValue

            If (Integer.Parse(ddlQuarter.SelectedValue) = 0) Then
                PopulateYearDDL(DateTime.Now.Year - 1)
            Else
                PopulateYearDDL(DateTime.Now.Year)
            End If

            If (ddlYear.Items.FindByValue(currentYearSelection) IsNot Nothing) Then
                ddlYear.SelectedValue = currentYearSelection
            Else
                ddlYear.SelectedValue = 0
            End If
        End Sub

        Protected Sub ddlYear_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ddlYear.SelectedIndexChanged
            Dim currentQuarterSelection As String = ddlQuarter.SelectedValue

            PopulateQuarterDDL(Integer.Parse(ddlYear.SelectedValue))

            If (ddlQuarter.Items.FindByValue(currentQuarterSelection) IsNot Nothing) Then
                ddlQuarter.SelectedValue = currentQuarterSelection
            Else
                ddlQuarter.SelectedValue = 0
            End If
        End Sub

        Protected Function GetCurrentQuarter() As Integer
            ' Quarter 1 = Jan(1), Feb(2), Mar(3)
            ' Quarter 2 = Apr(4), May(5), June(6)
            ' Quarter 3 = July(7), Aug(8), Sept(9)
            ' Quarter 4 = Oct(10), Nov(11), Dec(12)

            Dim currentMonth As Integer = DateTime.Now.Month

            If (currentMonth > 9) Then
                Return 4
            ElseIf (currentMonth > 6) Then
                Return 3
            ElseIf (currentMonth > 3) Then
                Return 2
            Else
                Return 1
            End If
        End Function

        Protected Function GetListItemForQuarter(ByVal quarter As Integer) As ListItem
            Select Case quarter
                Case 1
                    Return New ListItem("Quarter 1 (Jan - Mar)", 1)

                Case 2
                    Return New ListItem("Quarter 2 (Apr - Jun)", 2)

                Case 3
                    Return New ListItem("Quarter 3 (Jul - Sept)", 3)

                Case 4
                    Return New ListItem("Quarter 4 (Oct - Dec)", 4)

                Case Else
                    Return New ListItem("ERROR", 9999)
            End Select

        End Function

        Protected Sub InitControls()
            PopulateQuarterDDL(DateTime.Now.Year - 1)
            PopulateYearDDL(DateTime.Now.Year - 1)
        End Sub

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
            If (Not Page.IsPostBack) Then
                InitControls()
            End If
        End Sub

        Protected Sub PopulateQuarterDDL(ByVal forYear As Integer)
            ' Quarter 1 = Jan(1), Feb(2), Mar(3)
            ' Quarter 2 = Apr(4), May(5), June(6)
            ' Quarter 3 = July(7), Aug(8), Sept(9)
            ' Quarter 4 = Oct(10), Nov(11), Dec(12)

            ddlQuarter.Items.Clear()
            ddlQuarter.Items.Add(New ListItem("-- No Quarter Selected --", 0))

            ' Add all quarters for years less then the current year...
            If (forYear < DateTime.Now.Year) Then
                ddlQuarter.Items.Add(GetListItemForQuarter(1))
                ddlQuarter.Items.Add(GetListItemForQuarter(2))
                ddlQuarter.Items.Add(GetListItemForQuarter(3))
                ddlQuarter.Items.Add(GetListItemForQuarter(4))
                Exit Sub
            End If

            Dim currentMonth As Integer = DateTime.Now.Month

            ' Add quarters for the current year only if the quarter has passed...
            If (currentMonth < 4) Then
                Exit Sub
            End If

            ddlQuarter.Items.Add(GetListItemForQuarter(1))

            If (currentMonth < 7) Then
                Exit Sub
            End If

            ddlQuarter.Items.Add(GetListItemForQuarter(2))

            If (currentMonth < 10) Then
                Exit Sub
            End If

            ddlQuarter.Items.Add(GetListItemForQuarter(3))

            ' Quarter 4 can never be added for the current year...that quarter is only available when the current year turns over to the next year...
        End Sub

        Protected Sub PopulateUnitChainRepeater()
            rptUnitNavigation.DataSource = UnitChain
            rptUnitNavigation.DataBind()
        End Sub

        Protected Sub PopulateYearDDL(ByVal maxYear As Integer)
            ddlYear.Items.Clear()
            ddlYear.Items.Add(New ListItem("-- No Year Selected --", 0))

            Dim oldestYear As Integer = 2011

            If (maxYear < 1990 OrElse maxYear > DateTime.Now.Year) Then
                maxYear = DateTime.Now.Year
            End If

            If (maxYear < oldestYear) Then
                maxYear = oldestYear
            End If

            For i As Integer = maxYear To oldestYear Step -1
                ddlYear.Items.Add(New ListItem(i, i))
            Next
        End Sub

        Protected Sub rptUnitNavigation_ItemCommand(source As Object, e As RepeaterCommandEventArgs) Handles rptUnitNavigation.ItemCommand
            If (e.CommandName.Equals(GRIDCOMMAND_VIEW_UNIT)) Then
                Dim args As String() = e.CommandArgument.ToString().Split(";")
                Dim groupByChildUnits As Boolean = True

                ' Rebuild unit chain from the version stored in the view state...
                For Each item As KeyValuePair(Of Integer, String) In StoredUnitChain
                    If (item.Key = Integer.Parse(args(0))) Then
                        Exit For
                    End If

                    AddUnitToUnitChain(item.Key, item.Value)
                Next

                StoredUnitChain = UnitChain

                CurrentUnitId = Integer.Parse(args(0))

                UpdateUnitChainNavigator(False)

                ' Check if the user is attempting to output the case results of a unit which has both sub unit and individual cases associated with it to a file...
                If (args.Count > 2 AndAlso args(2).Equals("cases") AndAlso Not rdbOutputScreen.Checked) Then
                    groupByChildUnits = False
                End If

                'Place the code below here in a function...reduce copy/paste code...
                If (Not Report.ExecuteReport(ConstructReportArguments(CurrentUnitId, groupByChildUnits))) Then
                    bllErrors.Items.Add("The report failed to execute.")
                    trErrors.Visible = True
                    Exit Sub
                End If

                ' Determine how the repot results should be output...
                If (rdbOutputExcel.Checked) Then
                    ResultsToExcel()
                ElseIf (rdbOutputPdf.Checked) Then
                    ResultsToPDF()
                ElseIf (rdbOutputCsv.Checked) Then
                    ResultsToCSV()
                ElseIf (rdbOutputXml.Checked) Then
                    ResultsToXml()
                Else
                    ResultsToBrowser()
                End If
            End If
        End Sub

        Protected Sub rptUnitNavigation_ItemDataBound(sender As Object, e As RepeaterItemEventArgs) Handles rptUnitNavigation.ItemDataBound
            If (e.Item.ItemType <> ListItemType.Item AndAlso e.Item.ItemType <> ListItemType.AlternatingItem) Then
                Exit Sub
            End If

            Dim item As KeyValuePair(Of Integer, String) = CType(e.Item.DataItem, KeyValuePair(Of Integer, String))
            Dim lnk As LinkButton = CType(e.Item.FindControl("lnkUnit"), LinkButton)

            If (item.Value.Contains(";cases")) Then
                lnk.Text = item.Value.Substring(0, item.Value.IndexOf(";"))
            Else
                lnk.Text = item.Value
            End If

            lnk.CommandName = GRIDCOMMAND_VIEW_UNIT
            lnk.CommandArgument = item.Key.ToString() + ";" + item.Value
        End Sub

        Protected Sub RunReport()
            bllErrors.Items.Clear()
            trErrors.Visible = False

            If (Not UserHasPermission("LODProgramStatusReport")) Then
                bllErrors.Items.Add("You do not have permission to run this report.")
                trErrors.Visible = True
                Exit Sub
            End If

            If (Not Report.ExecuteReport(ConstructReportArguments(CurrentUnitId, True))) Then
                bllErrors.Items.Add("The report failed to execute.")
                trErrors.Visible = True
                Exit Sub
            End If

            ' Determine how the repot results should be output...
            If (rdbOutputExcel.Checked) Then
                ResultsToExcel()
            ElseIf (rdbOutputPdf.Checked) Then
                ResultsToPDF()
            ElseIf (rdbOutputCsv.Checked) Then
                ResultsToCSV()
            ElseIf (rdbOutputXml.Checked) Then
                ResultsToXml()
            Else
                ResultsToBrowser()
            End If
        End Sub

        Protected Sub UpdateUnitChainNavigator(ByVal clearChain As Boolean)
            UnitChain = StoredUnitChain

            If (clearChain) Then
                UnitChain.Clear()
            End If

            AddUnitToUnitChain(CurrentUnitId, UnitDao.GetById(CurrentUnitId).NameAndPasCode)

            StoredUnitChain = UnitChain

            PopulateUnitChainRepeater()
        End Sub

        Protected Function ValidateReportInput() As Boolean
            Dim isValid As Boolean = True

            Return isValid
        End Function

        Private Sub AddUnitToUnitChain(ByVal cs_id As Integer, ByVal text As String)
            If (Not UnitChain.Keys.Contains(cs_id)) Then
                UnitChain.Add(cs_id, text)
            Else
                ' This is the sitation where a unit has both sub units and cases associated with it and the user has selected the unit a second time in order to see the individual cases
                ' therefore, we append a string to denote this...
                UnitChain(cs_id) = UnitChain(cs_id) & ";cases"
            End If
        End Sub

#Region "UI Procedures..."

        Protected Sub ConfigureDaysLabel(ByVal lbl As Label, ByVal value As Nullable(Of Integer), ByVal thresholdStatus As ProcessingTimeThresholdStatus)
            If (Not value.HasValue) Then
                Exit Sub
            End If

            lbl.Text = value.Value

            If (thresholdStatus = ProcessingTimeThresholdStatus.Over) Then
                lbl.CssClass &= " OverThresholdTolerance"
            ElseIf (thresholdStatus = ProcessingTimeThresholdStatus.Near) Then
                lbl.CssClass &= " InThresholdTolerance"
            End If
        End Sub

        Protected Sub ConfigureDaysOpenLabel(ByVal lbl As Label, ByVal resultItem As LODProgramStatusReportResultItem)
            lbl.Text = resultItem.DaysOpen

            If (resultItem.DaysOpenStatus = ProcessingTimeThresholdStatus.Over) Then
                lbl.CssClass &= " OverThresholdTolerance"
            ElseIf (resultItem.DaysOpenStatus = ProcessingTimeThresholdStatus.Near) Then
                lbl.CssClass &= " InThresholdTolerance"
            End If
        End Sub

        Protected Sub gdvResults_DataBound(sender As Object, e As EventArgs) Handles gdvResults.DataBound
            ' Update the visibility of certain columns based on if unit aggregates or individual cases are being displayed...
            If (Report.ResultsDataKey.Equals("UnitId")) Then
                gdvResults.Columns(0).Visible = True      ' Unit
                gdvResults.Columns(1).Visible = False     ' Case Id
                gdvResults.Columns(3).Visible = False     ' Date Completed
                gdvResults.Columns(20).Visible = False    ' Member Unit
            Else
                gdvResults.Columns(0).Visible = False     ' Unit
                gdvResults.Columns(1).Visible = True      ' Case Id
                gdvResults.Columns(3).Visible = True      ' Date Completed
                gdvResults.Columns(20).Visible = True     ' Member Unit
            End If
        End Sub

        Protected Sub gdvResults_RowCommand(sender As Object, e As GridViewCommandEventArgs) Handles gdvResults.RowCommand
            If (e.CommandName.Equals(GRIDCOMMAND_VIEW_UNIT)) Then
                ' Check if the same unit was clicked...if so display individual cases...
                Dim groupByChildUnits As Boolean = True

                If (CurrentUnitId = e.CommandArgument) Then
                    groupByChildUnits = False
                End If

                If (Not Report.ExecuteReport(ConstructReportArguments(e.CommandArgument, groupByChildUnits))) Then
                    bllErrors.Items.Add("The report failed to execute.")
                    trErrors.Visible = True
                    Exit Sub
                End If

                CurrentUnitId = e.CommandArgument

                UpdateUnitChainNavigator(False)

                ' Determine how the repot results should be output...
                If (rdbOutputExcel.Checked) Then
                    ResultsToExcel()
                ElseIf (rdbOutputPdf.Checked) Then
                    ResultsToPDF()
                ElseIf (rdbOutputCsv.Checked) Then
                    ResultsToCSV()
                ElseIf (rdbOutputXml.Checked) Then
                    ResultsToXml()
                Else
                    ResultsToBrowser()
                End If
            End If
        End Sub

        Protected Sub gdvResults_RowDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewRowEventArgs) Handles gdvResults.RowDataBound
            If (e.Row.RowType <> DataControlRowType.DataRow OrElse Report Is Nothing) Then
                Exit Sub
            End If

            ' Check if this row is being edited or not
            If e.Row.RowIndex <> gdvResults.EditIndex Then
                Dim dataKey As Integer = CInt(gdvResults.DataKeys(e.Row.RowIndex).Value)
                Dim resultItem As LODProgramStatusReportResultItem = Nothing

                ' Get the resultItem by the proper key based on if unit aggregates or individual cases are being displayed...
                If (Report.ResultsDataKey.Equals("UnitId")) Then
                    resultItem = Report.Results.Where(Function(x) x.UnitId = dataKey).First()
                Else
                    resultItem = Report.Results.Where(Function(x) x.RefId = dataKey).First()
                End If

                If (resultItem Is Nothing) Then
                    Exit Sub
                End If

                ' Populate certain fields based on if unit aggregates or individual cases are being displayed...
                If (Report.ResultsDataKey.Equals("UnitId")) Then
                    Dim link As LinkButton = CType(e.Row.FindControl("lkbUnit"), LinkButton)
                    link.Text = resultItem.MemberUnit
                    link.CommandArgument = resultItem.UnitId
                Else
                    CType(e.Row.FindControl("lblCaseId"), Label).Text = resultItem.CaseId
                    CType(e.Row.FindControl("lblMemberUnit"), Label).Text = resultItem.MemberUnit
                    CType(e.Row.FindControl("lblDateCompleted"), Label).Text = resultItem.DateCompleted.Value.ToString("MM\/dd\/yyyy HH:mm")
                End If

                ConfigureDaysOpenLabel(CType(e.Row.FindControl("lblDaysOpen"), Label), resultItem)
                ConfigureDaysLabel(CType(e.Row.FindControl("lblMedTechDays"), Label), resultItem.MedicalTechnicianDays, resultItem.MedTechStatus)
                ConfigureDaysLabel(CType(e.Row.FindControl("lblMedOffDays"), Label), resultItem.MedicalOfficerDays, resultItem.MedOffStatus)
                ConfigureDaysLabel(CType(e.Row.FindControl("lblUnitCCDays"), Label), resultItem.UnitCommanderDays, resultItem.UnitCCStatus)
                ConfigureDaysLabel(CType(e.Row.FindControl("lblWingJADays"), Label), resultItem.WingJudgeAdvocateDays, resultItem.WingJAStatus)
                ConfigureDaysLabel(CType(e.Row.FindControl("lblWingCCDays"), Label), resultItem.WingCommanderDays, resultItem.WingCCStatus)
                ConfigureDaysLabel(CType(e.Row.FindControl("lblWingSARCDays"), Label), resultItem.WingSARCDays, resultItem.WingSARCStatus)
                ConfigureDaysLabel(CType(e.Row.FindControl("lblInformalBoardDays"), Label), resultItem.CombinedBoardDays, resultItem.BoardStatus)
                ConfigureDaysLabel(CType(e.Row.FindControl("lblLODPMDays"), Label), resultItem.ProgramManagerDays, ProcessingTimeThresholdStatus.Under)
                ConfigureDaysLabel(CType(e.Row.FindControl("lblIODays"), Label), resultItem.InvestigatingOfficerDays, resultItem.FormalIOStatus)
                ConfigureDaysLabel(CType(e.Row.FindControl("lblFormalWingJADays"), Label), resultItem.FormalWingJudgeAdvocateDays, resultItem.FormalWingJAStatus)
                ConfigureDaysLabel(CType(e.Row.FindControl("lblFormalWingCCDays"), Label), resultItem.FormalWingCommanderDays, resultItem.FormalWingCCStatus)
                ConfigureDaysLabel(CType(e.Row.FindControl("lblBoardTechDays"), Label), resultItem.FormalBoardTechnicianDays, resultItem.FormalBoardStatus)
                ConfigureDaysLabel(CType(e.Row.FindControl("lblBoardJADays"), Label), resultItem.FormalBoardJudgeAdvocateDays, resultItem.FormalBoardStatus)
                ConfigureDaysLabel(CType(e.Row.FindControl("lblBoardA1Days"), Label), resultItem.FormalBoardAdministratorDays, resultItem.FormalBoardStatus)
                ConfigureDaysLabel(CType(e.Row.FindControl("lblBoardMedDays"), Label), resultItem.FormalBoardMedicalOfficerDays, resultItem.FormalBoardStatus)
                ConfigureDaysLabel(CType(e.Row.FindControl("lblApprAuthDays"), Label), resultItem.FormalApprovalAuthorityDays, resultItem.FormalBoardStatus)
            End If
        End Sub

        Protected Sub ResultsToBrowser()
            If (Report Is Nothing OrElse Not Report.HasExecuted) Then
                bllErrors.Items.Add("The report failed to execute.")
                trErrors.Visible = True
                pnlTopScrollPanel.Visible = False
                pnlHorizontalScrollPanel.Visible = False
                pnlAFINotes.Visible = False
                pnlResults.Visible = False
                Exit Sub
            End If

            If (Report.Results.Count = 0) Then
                lblEmptyResults.Text = "No Results Found."
                pnlResults.Visible = True
                pnlTopScrollPanel.Visible = False
                pnlHorizontalScrollPanel.Visible = False
                pnlAFINotes.Visible = False
                pnlEmptyResults.Visible = True
                Exit Sub
            End If

            pnlResults.Visible = True
            pnlTopScrollPanel.Visible = True
            pnlHorizontalScrollPanel.Visible = True
            pnlAFINotes.Visible = True
            pnlEmptyResults.Visible = False

            gdvResults.DataSource = Report.Results.OrderByDescending(Function(x) x.DaysOpen).ThenBy(Function(y) y.CaseId)
            gdvResults.DataKeyNames = New String() {Report.ResultsDataKey}
            gdvResults.DataBind()
        End Sub

#End Region

#Region "Excel Procedures..."

        Protected Sub ResultsToExcel()
            If (Report Is Nothing OrElse Not Report.HasExecuted) Then
                bllErrors.Items.Add("The report failed to execute.")
                trErrors.Visible = True
                Exit Sub
            End If

            ResultsToBrowser()

            If (Report.Results.Count = 0) Then
                lblEmptyResults.Text &= "..No results to export to EXCEL."
                Exit Sub
            End If

            ' Populate excel gridviews with report results data...
            Dim table As New HtmlTable

            BuildExcelTable(table)
            BuildAFINotes(table)

            ' Render HTML and send as an excel file...
            Dim writer As New System.IO.StringWriter
            Dim html As New HtmlTextWriter(writer)
            table.RenderControl(html)

            Response.Clear()
            Response.AddHeader("content-disposition", "attachment;filename=LOD_PM_Quarterly-Annual_Program_Status_Report_" + Date.Now.ToString("yyyyMMdd") + ".xls")
            Response.Charset = ""
            Response.ContentType = "application/ms-excel"
            Response.Write(writer.ToString())
            Response.End()
        End Sub

        Private Sub AddTableCell(ByVal row As HtmlTableRow, ByVal text As String, ByVal style As String)
            Dim cell As New HtmlTableCell
            cell.InnerHtml = text

            If (Not String.IsNullOrEmpty(style)) Then
                cell.Attributes.Add("style", style)
            End If

            row.Cells.Add(cell)
        End Sub

        Private Sub BuildAFINotes(ByVal table As HtmlTable)
            ' Add empty row...
            table.Rows.Add(New HtmlTableRow)

            Dim row As New HtmlTableRow()

            AddTableCell(row, "**: " & lblDoubleAsteriskMsg.Text, EXCEL_TEXT_NOWRAP_STYLE)
            table.Rows.Add(row)

            row = New HtmlTableRow()

            AddTableCell(row, "***: " & lblTripleAsteriskMsg.Text, EXCEL_TEXT_NOWRAP_STYLE)
            table.Rows.Add(row)
        End Sub

        Private Sub BuildExcelTable(ByVal table As HtmlTable)
            Dim odd As Boolean = False
            Dim row As New HtmlTableRow()
            Dim currentLabel As Label
            Dim currentLinkButton As LinkButton
            Dim currentColumnId As Integer = -1

            ' Add header row...
            row.Attributes.Add("style", EXCEL_HEADER_ROW_STYLE)

            For Each cell As TableCell In gdvResults.HeaderRow.Cells
                currentColumnId = currentColumnId + 1

                If (Not gdvResults.Columns(currentColumnId).Visible) Then
                    Continue For
                End If

                AddTableCell(row, cell.Text, String.Empty)
            Next

            table.Rows.Add(row)

            ' Add data rows...
            For Each gridRow As GridViewRow In gdvResults.Rows
                row = New HtmlTableRow
                Dim bgColor As String = IIf(odd, "#d8d8ff;", "#FFF;")
                odd = Not odd

                row.Attributes.Add("style", EXCEL_DATA_ROW_STYLE + bgColor)
                table.Rows.Add(row)

                currentColumnId = -1

                For Each cell As TableCell In gridRow.Cells
                    currentColumnId = currentColumnId + 1

                    If (Not gdvResults.Columns(currentColumnId).Visible) Then
                        Continue For
                    End If

                    If (cell.Controls.Count > 0) Then
                        If (TypeOf cell.Controls(1) Is Label) Then
                            currentLabel = CType(cell.Controls(1), Label)

                            AddTableCell(row, currentLabel.Text, GetCellStyle(currentLabel.CssClass, currentLabel.Text))

                            currentLabel = Nothing
                        ElseIf (TypeOf cell.Controls(1) Is LinkButton) Then
                            currentLinkButton = CType(cell.Controls(1), LinkButton)

                            AddTableCell(row, currentLinkButton.Text, GetCellStyle(currentLinkButton.CssClass, currentLinkButton.Text))

                            currentLinkButton = Nothing
                        End If
                    Else
                        AddTableCell(row, cell.Text, String.Empty)
                    End If
                Next
            Next
        End Sub

        Private Function GetCellStyle(ByVal cssClasses As String, ByVal cellValue As String) As String
            Dim style As String = String.Empty

            If (cssClasses.Contains("InThresholdTolerance")) Then
                style = EXCEL_TEXT_ORANGE_STYLE
            ElseIf (cssClasses.Contains("OverThresholdTolerance")) Then
                style = EXCEL_TEXT_RED_STYLE
            Else
                style = String.Empty
            End If

            If (cssClasses.Contains("numericResult")) Then
                style &= EXCEL_TEXT_BOLD_STYLE
            End If

            If (cellValue.Equals("*")) Then
                style = style + EXCEL_TEXT_ALIGN_STYLE
            End If

            Return style
        End Function

#End Region

#Region "PDF Procedures..."

        Protected Sub ResultsToPDF()
            If (Report Is Nothing OrElse Not Report.HasExecuted) Then
                bllErrors.Items.Add("The report failed to execute.")
                trErrors.Visible = True
                Exit Sub
            End If

            ResultsToBrowser()

            If (Report.Results.Count = 0) Then
                lblEmptyResults.Text &= "..No results to export to PDF."
                Exit Sub
            End If

            ' Populate PDF file with report results data...
            Dim pdf As New Doc()
            Dim currentLabel As Label
            Dim currentLinkButton As LinkButton
            Dim currentColumnId As Integer = -1
            Dim visibleColumnsCount As Integer = 0
            Dim colNames As New StringCollection()
            Dim table As PdfTable
            Dim i As Integer = 0
            Dim page As Integer = 1

            ' Determine how many columns of the results gridview are visible
            For Each c As DataControlField In gdvResults.Columns
                If (c.Visible) Then
                    visibleColumnsCount = visibleColumnsCount + 1
                End If
            Next

            ' We need to be in landscape to allow as much width as possible; therefor, apply a rotation transform (http://www.websupergoo.com/helppdfnet/source/4-examples/08-landscape.htm)...
            Dim w As Double = pdf.MediaBox.Width
            Dim h As Double = pdf.MediaBox.Height
            Dim l As Double = pdf.MediaBox.Left
            Dim b As Double = pdf.MediaBox.Bottom

            pdf.Transform.Rotate(90, l, b)
            pdf.Transform.Translate(w, 0)

            ' Rotate PDF rectangle...
            pdf.Rect.Width = h
            pdf.Rect.Height = w

            ' Rotate the page...
            Dim docId As Integer = pdf.GetInfoInt(pdf.Root, "Pages")
            pdf.SetInfo(docId, "/Rotate", "90")

            pdf.FontSize = 7
            pdf.Rect.Inset(10, 40)

            table = New PdfTable(pdf, visibleColumnsCount)
            table.CellPadding = 1
            table.RepeatHeader = True

            ' Header rows....
            table.NextRow()
            For Each cell As TableCell In gdvResults.HeaderRow.Cells
                currentColumnId = currentColumnId + 1

                If (Not gdvResults.Columns(currentColumnId).Visible) Then
                    Continue For
                End If

                colNames.Add(cell.Text)
            Next

            Dim header(colNames.Count) As String
            colNames.CopyTo(header, 0)

            table.AddHtml(header)

            ' Add data rows...
            For Each gridRow As GridViewRow In gdvResults.Rows
                table.NextRow()

                Dim rowCells As New List(Of PDFTableHTMLData)()

                currentColumnId = -1

                For Each cell As TableCell In gridRow.Cells
                    currentColumnId = currentColumnId + 1

                    If (Not gdvResults.Columns(currentColumnId).Visible) Then
                        Continue For
                    End If

                    If (cell.Controls.Count > 0) Then
                        If (TypeOf cell.Controls(1) Is Label) Then
                            currentLabel = CType(cell.Controls(1), Label)

                            rowCells.Add(New PDFTableHTMLData(currentLabel.Text, GetCellColor(currentLabel.CssClass), GetCellBold(currentLabel.CssClass), GetCellTextAlignment(currentLabel.CssClass)))

                            currentLabel = Nothing
                        ElseIf (TypeOf cell.Controls(1) Is LinkButton) Then
                            currentLinkButton = CType(cell.Controls(1), LinkButton)

                            rowCells.Add(New PDFTableHTMLData(currentLinkButton.Text, GetCellColor(currentLinkButton.CssClass), GetCellBold(currentLinkButton.CssClass), GetCellTextAlignment(currentLinkButton.CssClass)))

                            currentLinkButton = Nothing
                        End If
                    Else
                        rowCells.Add(New PDFTableHTMLData(cell.Text, String.Empty, False, 0.0))
                    End If
                Next

                table.AddHtml(rowCells)

                If (pdf.PageNumber > page) Then
                    page = pdf.PageNumber
                End If
            Next

            table.FrameRows()
            table.FrameColumns()

            pdf.VPos = 0.5

            For ct As Integer = 1 To pdf.PageCount
                pdf.PageNumber = ct

                'left side
                pdf.HPos = 0.0
                pdf.Rect.SetRect(20, 580, 280, 20)
                pdf.AddText("")

                'middle
                pdf.HPos = 0.5
                pdf.Rect.SetRect(20, 580, 750, 20)
                pdf.AddText("LOD PM Monthly Suspense Monitoring Report")

                'right
                pdf.HPos = 1.0
                pdf.Rect.SetRect(500, 580, 270, 20)
                pdf.AddText("Generated: " + DateTime.Now.ToString(DATE_HOUR_FORMAT))

                'page number
                pdf.HPos = 0.5
                pdf.Rect.SetRect(20, 10, 750, 10)
                pdf.AddText("Page " + ct.ToString() + " of " + pdf.PageCount.ToString())

                'table header
                'pdf.AddLine(10, 550, 782, 550)

                ' Asterisk messages...
                If (ct = pdf.PageCount) Then
                    pdf.HPos = 0.0

                    pdf.Rect.SetRect(20, 23, 750, 23)
                    pdf.AddText("**: " + lblDoubleAsteriskMsg.Text)

                    pdf.Rect.SetRect(20, 15, 750, 15)
                    pdf.AddText("***: " + lblTripleAsteriskMsg.Text)
                End If
            Next

            pdf.Flatten()
            Dim buffer() As Byte = pdf.GetData()

            Response.ContentType = "application/pdf"
            Response.AddHeader("content-disposition", "attachment;filename=LOD_PM_Quarterly-Annual_Program_Status_Report_" + Date.Now.ToString("yyyyMMdd") + ".pdf")
            Response.AddHeader("content-length", buffer.Length.ToString())
            Response.BinaryWrite(buffer)
            Response.End()
        End Sub

        Private Function GetCellBold(ByVal cssClasses As String) As String
            If (cssClasses.Contains("numericResult")) Then
                Return True
            Else
                Return False
            End If
        End Function

        Private Function GetCellColor(ByVal cssClasses As String) As String
            Dim color As String = String.Empty

            If (cssClasses.Contains("InThresholdTolerance")) Then
                color = "255 127 0"
            ElseIf (cssClasses.Contains("OverThresholdTolerance")) Then
                color = "255 0 0"
            Else
                color = String.Empty
            End If

            Return color
        End Function

        Private Function GetCellTextAlignment(ByVal cssClasses As String) As Double
            If (cssClasses.Contains("numericResult")) Then
                Return 1.0
            Else
                Return 0.0
            End If
        End Function

#End Region

#Region "XML Procedures"

        Private Sub BuildXmlData(ByVal data As DataTable)
            Dim currentColumnId As Integer = -1

            For Each cell As TableCell In gdvResults.HeaderRow.Cells
                currentColumnId = currentColumnId + 1

                If (Not gdvResults.Columns(currentColumnId).Visible) Then
                    Continue For
                End If

                data.Columns.Add(cell.Text)
            Next

            Dim row As DataRow
            Dim i As Integer = 0

            For Each gridRow As GridViewRow In gdvResults.Rows
                row = data.NewRow()

                i = 0
                currentColumnId = -1

                For Each cell As TableCell In gridRow.Cells
                    currentColumnId = currentColumnId + 1

                    If (Not gdvResults.Columns(currentColumnId).Visible) Then
                        Continue For
                    End If

                    If (cell.Controls.Count > 0) Then
                        If (TypeOf cell.Controls(1) Is Label) Then
                            row(i) = CType(cell.Controls(1), Label).Text
                        ElseIf (TypeOf cell.Controls(1) Is LinkButton) Then
                            row(i) = CType(cell.Controls(1), LinkButton).Text
                        End If
                    Else
                        row(i) = cell.Text
                    End If

                    i = i + 1
                Next

                data.Rows.Add(row)
            Next
        End Sub

        Private Sub ResultsToXml()
            If (Report Is Nothing OrElse Not Report.HasExecuted) Then
                bllErrors.Items.Add("The report failed to execute.")
                trErrors.Visible = True
                Exit Sub
            End If

            ResultsToBrowser()

            If (Report.Results.Count = 0) Then
                lblEmptyResults.Text &= "..No results to export to XML."
                Exit Sub
            End If

            ' Populate XML with report results data...
            Dim data As DataTable = New DataTable()

            BuildXmlData(data)

            Response.Clear()
            Response.AddHeader("content-disposition", "attachment;filename=LOD_PM_Quarterly-Annual_Program_Status_Report_" + Date.Now.ToString("yyyyMMdd") + ".xml")
            Response.Charset = ""
            Response.ContentType = "text/xml"
            Response.Write("<?xml version=""1.0"" encoding=""utf-8"" ?>" + Environment.NewLine)

            data.TableName = "ReportResults"
            data.WriteXml(Response.OutputStream)

            Response.End()
        End Sub

#End Region

#Region "CSV Procedures"

        Private Sub BuildCSVData(ByVal buffer As StringBuilder)
            Dim isFirst As Boolean = True
            Dim currentColumnId As Integer = -1

            For Each cell As TableCell In gdvResults.HeaderRow.Cells
                currentColumnId = currentColumnId + 1

                If (Not gdvResults.Columns(currentColumnId).Visible) Then
                    Continue For
                End If

                If (Not isFirst) Then
                    buffer.Append(",")
                Else
                    isFirst = False
                End If

                buffer.Append(cell.Text)
            Next

            buffer.Append(Environment.NewLine)

            For Each gridRow As GridViewRow In gdvResults.Rows
                isFirst = True

                currentColumnId = -1

                For Each cell As TableCell In gridRow.Cells
                    currentColumnId = currentColumnId + 1

                    If (Not gdvResults.Columns(currentColumnId).Visible) Then
                        Continue For
                    End If

                    If (Not isFirst) Then
                        buffer.Append(",")
                    Else
                        isFirst = False
                    End If

                    If (cell.Controls.Count > 0) Then
                        If (TypeOf cell.Controls(1) Is Label) Then
                            buffer.Append(CType(cell.Controls(1), Label).Text)
                        ElseIf (TypeOf cell.Controls(1) Is LinkButton) Then
                            buffer.Append(CType(cell.Controls(1), LinkButton).Text)
                        End If
                    Else
                        buffer.Append(cell.Text)
                    End If
                Next

                buffer.Append(Environment.NewLine)
            Next
        End Sub

        Private Sub ResultsToCSV()
            If (Report Is Nothing OrElse Not Report.HasExecuted) Then
                bllErrors.Items.Add("The report failed to execute.")
                trErrors.Visible = True
                Exit Sub
            End If

            ResultsToBrowser()

            If (Report.Results.Count = 0) Then
                lblEmptyResults.Text &= "..No results to export to CSV."
                Exit Sub
            End If

            ' Populate CSV with report results data...
            Dim buffer As New StringBuilder()

            BuildCSVData(buffer)

            Response.Clear()
            Response.AddHeader("content-disposition", "attachment;filename=LOD_PM_Quarterly-Annual_Program_Status_Report_" + Date.Now.ToString("yyyyMMdd") + ".csv")
            Response.Charset = ""
            Response.ContentType = "text/plain"
            Response.Write(buffer.ToString())
            Response.End()
        End Sub

#End Region

    End Class

End Namespace