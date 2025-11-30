Imports System.Collections.ObjectModel
Imports ALOD.Core.Domain.PsychologicalHealth
Imports ALOD.Core.Interfaces.DAOInterfaces
Imports ALOD.Data
Imports ALODWebUtility.Common

Namespace Web.Reports

    Public Class PHTotals
        Inherits System.Web.UI.Page

        Protected Const EXCEL_DATA_ROW_STYLE As String = "border: 1px solid #CCC; border-bottom: solid 1px #C0C0C0; background-color:"
        Protected Const EXCEL_HEADER_ROW_STYLE As String = "background-color:navy;text-align: center; border: 1px solid #CCC; border-bottom: solid 1px black; color:white;"
        Protected Const EXCEL_TEXT_ALIGN_STYLE As String = "vertical-align:middle; text-align:left;"
        Protected Const MSG_NO_SUB_UNITS As String = "No sub units found"
        Private _phDao As IPsychologicalHealthDao
        Private _phTotalsReport As PHTotalsReport
        Private _smDao As ISuicideMethodDao

        Public ReadOnly Property PHDao As IPsychologicalHealthDao
            Get
                If (_phDao Is Nothing) Then
                    _phDao = New NHibernateDaoFactory().GetPsychologicalHealthDao()
                End If

                Return _phDao
            End Get
        End Property

        Public ReadOnly Property SMDao As ISuicideMethodDao
            Get
                If (_smDao Is Nothing) Then
                    _smDao = New NHibernateDaoFactory().GetSuicideMethodDao()
                End If

                Return _smDao
            End Get
        End Property

        Public Property TotalsReport As PHTotalsReport
            Get
                Return _phTotalsReport
            End Get
            Set(value As PHTotalsReport)
                _phTotalsReport = value
            End Set
        End Property

        Public Sub RunReport(ByVal sender As Object, ByVal e As System.EventArgs)
            ResultsPanel.Visible = False
            ReportNavPH.ClearErrors()
            ReportNavPH.HideErrors()

            If (Not UserHasPermission("PHTotalsReport")) Then
                ReportNavPH.AddError("You do not have permission to run this report.")
                ReportNavPH.ShowErrors()
                Exit Sub
            End If

            If (Not Page.IsValid) Then
                ReportNavPH.AddError("An unknown error occured!")
                ReportNavPH.ShowErrors()
                Exit Sub
            End If

            If (Not ValidateReportInput()) Then
                ReportNavPH.ShowErrors()
                Exit Sub
            End If

            Dim report As PHTotalsReport = New PHTotalsReport(PHDao, SMDao)

            If (report Is Nothing) Then
                ReportNavPH.AddError("An unknown error occured!")
                ReportNavPH.ShowErrors()
                Exit Sub
            End If

            Dim reportSuccess As Boolean = report.ExecuteReport(ConstructReportArguments())

            If (Not reportSuccess) Then
                ReportNavPH.AddError("The report failed to execute!")
                ReportNavPH.ShowErrors()
                Exit Sub
            End If

            TotalsReport = report

            ' Determine how the report results should be output and them output them...
            If (ReportNavPH.OutputFormat.Equals("Excel")) Then
                ResultsToExcel()
            ElseIf (ReportNavPH.OutputFormat.Equals("Excel")) Then
                BuildReportResultsUI()
            Else
                BuildReportResultsUI()
            End If
        End Sub

        Protected Function ConstructReportArguments() As PHTotalsReportArgs
            Dim args = New PHTotalsReportArgs()

            args.UnitId = ReportNavPH.Unit
            args.IncludeSubUnits = ReportNavPH.IncludeSubordinate
            args.Collocated = ReportNavPH.Collocated
            args.ViewType = CInt(Session("ReportView"))
            args.BeginReportingPeriod = ReportNavPH.BeginReportingPeriod
            args.EndReportingPeriod = ReportNavPH.EndReportingPeriod

            Return args
        End Function

        Protected Sub Page_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
            ScriptManager.GetCurrent(Page).RegisterPostBackControl(ReportNavPH.FindControl("ReportButton"))
        End Sub

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
            AddHandler ReportNavPH.RptClicked, AddressOf RunReport

            If (Not Page.IsPostBack) Then

            End If
        End Sub

        Protected Function ValidateReportInput() As Boolean
            Dim isValid As Boolean = True

            ' Check to make sure a unit is selected...
            If (Not ReportNavPH.IsUnitSelected()) Then
                ReportNavPH.AddError("A " & ReportNavPH.UnitType & " must be selected.")
                isValid = False
            End If

            ' Check if the reporting period begin date is greater than the end date...
            If (ReportNavPH.BeginReportingPeriod.HasValue AndAlso ReportNavPH.EndReportingPeriod.HasValue AndAlso ReportNavPH.BeginReportingPeriod.Value > ReportNavPH.EndReportingPeriod.Value) Then
                ReportNavPH.AddError("The begin reporting period cannot be more recent than the end reporting period.")
                isValid = False
            End If

            Return isValid
        End Function

#Region "UI Procedures"

        Protected Sub BuildReportResultsUI()
            If (TotalsReport Is Nothing OrElse Not TotalsReport.HasExecuted) Then
                pnlBrowserResults.Visible = False
                ResultsPanel.Visible = False
                Exit Sub
            End If

            ResetStringValuesPanelsVisibility()

            ResultsPanel.Visible = True
            pnlBrowserResults.Visible = True

            ' Bind Human Performance Improvement/Outreach gridviews...
            gdvWalkaboutUnitVisits.DataSource = TotalsReport.GetWalkaboutUnitVisitsPairs()
            gdvWalkaboutUnitVisits.DataBind()

            gdvWalkaboutUnitVisitsHours.DataSource = TotalsReport.GetWalkaboutUnitVisitsHours()
            gdvWalkaboutUnitVisitsHours.DataBind()

            gdvHumanPerformanceImprovement.DataSource = TotalsReport.GetTotalsBySection("Human Performance Improvement/Outreach")
            gdvHumanPerformanceImprovement.DataBind()

            If (ReportNavPH.IncludeTextFields) Then
                gdvHumanPerformanceImprovementStringValues.DataSource = TotalsReport.GetStringValuesForSections("Human Performance Improvement/Outreach")
                gdvHumanPerformanceImprovementStringValues.DataBind()

                If (CType(gdvHumanPerformanceImprovementStringValues.DataSource, ICollection(Of PHTotalsReportStringValue)).Count > 0) Then
                    pnlHumanPerformanceImprovementStringValues.Visible = True
                End If
            End If

            ' Bind Human Performance Sustainment gridviews...
            gdvTrendingActivity_Abuse.DataSource = TotalsReport.GetAbusePairs()
            gdvTrendingActivity_Abuse.DataBind()

            gdvTrendingActivity_FrMs.DataSource = TotalsReport.GetTrendingActivityFrMsPairs()
            gdvTrendingActivity_FrMs.DataBind()

            gdvTrendingActivity_FrFu.DataSource = TotalsReport.GetTrendingActivityFrFuPairs()
            gdvTrendingActivity_FrFu.DataBind()

            gdvTrendingActivity_Misc.DataSource = TotalsReport.GetTrendingActivityMiscTotals()
            gdvTrendingActivity_Misc.DataBind()

            BuildThreeGridViewGroupUI(TotalsReport.GetPresentingProblemsPairs(), gdvPresentingProblems1, gdvPresentingProblems2, gdvPresentingProblems3)
            BuildTwoGridViewGroupUI(TotalsReport.GetTotalsBySection("Deaths/Near Deaths"), gdvDeaths1, gdvDeaths2)

            gdvSuicideMethodTotals.DataSource = TotalsReport.GetSuicideMethodTotals()
            gdvSuicideMethodTotals.DataBind()

            If (ReportNavPH.IncludeTextFields) Then
                gdvTrendingActivityStringValues.DataSource = TotalsReport.GetStringValuesForSections("Trending Activity")
                gdvTrendingActivityStringValues.DataBind()

                If (CType(gdvTrendingActivityStringValues.DataSource, ICollection(Of PHTotalsReportStringValue)).Count > 0) Then
                    pnlTrendingActivityStringValues.Visible = True
                End If

                gdvDeathsStringValues.DataSource = TotalsReport.GetStringValuesForSections("Deaths/Near Deaths")
                gdvDeathsStringValues.DataBind()

                If (CType(gdvDeathsStringValues.DataSource, ICollection(Of PHTotalsReportStringValue)).Count > 0) Then
                    pnlDeathsStringValues.Visible = True
                End If

                gdvPresentingProblemsStringValues.DataSource = TotalsReport.GetStringValuesForSections("Presenting Problems")
                gdvPresentingProblemsStringValues.DataBind()

                If (CType(gdvPresentingProblemsStringValues.DataSource, ICollection(Of PHTotalsReportStringValue)).Count > 0) Then
                    pnlPresentingProblemsStringValues.Visible = True
                End If
            End If

            ' Bind Referrals gridviews...
            BuildThreeGridViewGroupUI(TotalsReport.GetTotalsBySection("Referrals"), gdvReferrals1, gdvReferrals2, gdvReferrals3)

            If (ReportNavPH.IncludeTextFields) Then
                gdvReferralsStringValues.DataSource = TotalsReport.GetStringValuesForSections("Referrals")
                gdvReferralsStringValues.DataBind()

                If (CType(gdvReferralsStringValues.DataSource, ICollection(Of PHTotalsReportStringValue)).Count > 0) Then
                    pnlReferralsStringValues.Visible = True
                End If
            End If

            ' Bind RMU Interaction gridviews...
            BuildThreeGridViewGroupUI(TotalsReport.GetTotalsBySection("RMU Interaction"), gdvRMUInteraction1, gdvRMUInteraction2, gdvRMUInteraction3)

            If (ReportNavPH.IncludeTextFields) Then
                gdvRMUInteractionStringValues.DataSource = TotalsReport.GetStringValuesForSections("RMU Interaction")
                gdvRMUInteractionStringValues.DataBind()

                If (CType(gdvRMUInteractionStringValues.DataSource, ICollection(Of PHTotalsReportStringValue)).Count > 0) Then
                    pnlRMUInteractionStringValues.Visible = True
                End If
            End If

            ' Bind Demographics gridviews...
            gdvDemographics_Status.DataSource = TotalsReport.GetTotalsBySection("Status")
            gdvDemographics_Status.DataBind()

            gdvDemographics_Age.DataSource = TotalsReport.GetTotalsBySection("Age")
            gdvDemographics_Age.DataBind()

            gdvDemographics_Gender.DataSource = TotalsReport.GetTotalsBySection("Gender")
            gdvDemographics_Gender.DataBind()

            gdvDemographics_Rank.DataSource = TotalsReport.GetTotalsBySection("Rank")
            gdvDemographics_Rank.DataBind()

            gdvDemographics_MaritalStatus.DataSource = TotalsReport.GetTotalsBySection("Marital Status")
            gdvDemographics_MaritalStatus.DataBind()

            gdvDemographics_Ethnicity.DataSource = TotalsReport.GetTotalsBySection("Ethnicity")
            gdvDemographics_Ethnicity.DataBind()

            If (ReportNavPH.IncludeTextFields) Then
                gdvDemographicsStringValues.DataSource = TotalsReport.GetStringValuesForSections("Only New Non-Medical//Medical Client Demographics")
                gdvDemographicsStringValues.DataBind()

                If (CType(gdvDemographicsStringValues.DataSource, ICollection(Of PHTotalsReportStringValue)).Count > 0) Then
                    pnlDemographicsStringValues.Visible = True
                End If
            End If

            ' Bind Comments gridview
            If (ReportNavPH.IncludeComments) Then
                gdvComments.DataSource = TotalsReport.Comments
                gdvComments.DataBind()
                pnlCommentsResults.Visible = True
            Else
                pnlCommentsResults.Visible = False
            End If

            ' Set common header row height for all gridviews...
            SetGridViewHeaderHeights()
        End Sub

        Protected Sub BuildThreeGridViewGroupUI(Of T)(ByVal totals As ReadOnlyCollection(Of T), ByVal gridView1 As GridView, ByVal gridView2 As GridView, ByVal gridView3 As GridView)
            Dim group1 As IList(Of T) = New List(Of T)
            Dim group2 As IList(Of T) = New List(Of T)
            Dim group3 As IList(Of T) = New List(Of T)

            If (totals.Count <= 0) Then
                Exit Sub
            End If

            Dim oneThirds As Integer = Math.Ceiling(totals.Count / 3)
            Dim twoThirds As Integer = oneThirds * 2

            For i As Integer = 0 To (totals.Count - 1)
                If (i < oneThirds) Then
                    group1.Add(totals(i))
                ElseIf (i >= oneThirds AndAlso i < twoThirds) Then
                    group2.Add(totals(i))
                Else
                    group3.Add(totals(i))
                End If
            Next

            gridView1.DataSource = group1
            gridView2.DataSource = group2
            gridView3.DataSource = group3

            gridView1.DataBind()
            gridView2.DataBind()
            gridView3.DataBind()
        End Sub

        Protected Sub BuildTwoGridViewGroupUI(Of T)(ByVal totals As ReadOnlyCollection(Of T), ByVal gridView1 As GridView, ByVal gridView2 As GridView)
            Dim group1 As IList(Of T) = New List(Of T)
            Dim group2 As IList(Of T) = New List(Of T)

            If (totals.Count <= 0) Then
                Exit Sub
            End If

            Dim half As Integer = Math.Ceiling(totals.Count / 2)

            For i As Integer = 0 To (totals.Count - 1)
                If (i < half) Then
                    group1.Add(totals(i))
                Else
                    group2.Add(totals(i))
                End If
            Next

            gridView1.DataSource = group1
            gridView2.DataSource = group2

            gridView1.DataBind()
            gridView2.DataBind()
        End Sub

        Protected Sub gdvDemographics_Status_RowDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewRowEventArgs) Handles gdvDemographics_Status.RowDataBound
            If (e.Row.RowType <> DataControlRowType.DataRow OrElse TotalsReport Is Nothing) Then
                Exit Sub
            End If

            Dim fieldId As Integer = Integer.Parse(CType(e.Row.FindControl("hdfFieldId"), HiddenField).Value)
            Dim fieldTypeId As Integer = Integer.Parse(CType(e.Row.FindControl("hdfFieldTypeId"), HiddenField).Value)
            Dim fieldName As String = String.Empty

            ' Check if this row is being edited or not
            If e.Row.RowIndex <> gdvHumanPerformanceImprovement.EditIndex Then
                fieldName = PHDao.GetFieldById(fieldId).Name

                If (fieldName.Equals("AD")) Then
                    CType(e.Row.FindControl("lblFieldName"), Label).Text = fieldName & " - " & PHDao.GetFieldTypeById(fieldTypeId).Name
                Else
                    CType(e.Row.FindControl("lblFieldName"), Label).Text = fieldName
                End If
            End If
        End Sub

        Protected Sub ResetStringValuesPanelsVisibility()
            pnlHumanPerformanceImprovementStringValues.Visible = False
            pnlHumanPerformanceImprovementStringValues.Visible = False
            pnlTrendingActivityStringValues.Visible = False
            pnlDeathsStringValues.Visible = False
            pnlPresentingProblemsStringValues.Visible = False
            pnlReferralsStringValues.Visible = False
            pnlRMUInteractionStringValues.Visible = False
            pnlDemographicsStringValues.Visible = False
        End Sub

        Protected Sub SetGridViewHeaderHeights()
            Dim headerHeight As Unit = New Unit(3, UnitType.Em)

            gdvWalkaboutUnitVisits.HeaderRow.Height = headerHeight
            gdvWalkaboutUnitVisitsHours.HeaderRow.Height = headerHeight
            gdvHumanPerformanceImprovement.HeaderRow.Height = headerHeight
            gdvTrendingActivity_Abuse.HeaderRow.Height = headerHeight
            gdvTrendingActivity_FrMs.HeaderRow.Height = headerHeight
            gdvTrendingActivity_FrFu.HeaderRow.Height = headerHeight
            gdvTrendingActivity_Misc.HeaderRow.Height = headerHeight
            gdvPresentingProblems1.HeaderRow.Height = headerHeight
            gdvPresentingProblems2.HeaderRow.Height = headerHeight
            gdvPresentingProblems3.HeaderRow.Height = headerHeight
            gdvDeaths1.HeaderRow.Height = headerHeight
            gdvDeaths2.HeaderRow.Height = headerHeight
            gdvSuicideMethodTotals.HeaderRow.Height = headerHeight
            gdvReferrals1.HeaderRow.Height = headerHeight
            gdvReferrals2.HeaderRow.Height = headerHeight
            gdvReferrals3.HeaderRow.Height = headerHeight
            gdvRMUInteraction1.HeaderRow.Height = headerHeight
            gdvRMUInteraction2.HeaderRow.Height = headerHeight
            gdvRMUInteraction3.HeaderRow.Height = headerHeight
            gdvDemographics_Status.HeaderRow.Height = headerHeight
            gdvDemographics_Age.HeaderRow.Height = headerHeight
            gdvDemographics_Gender.HeaderRow.Height = headerHeight
            gdvDemographics_Rank.HeaderRow.Height = headerHeight
            gdvDemographics_MaritalStatus.HeaderRow.Height = headerHeight
            gdvDemographics_Ethnicity.HeaderRow.Height = headerHeight

            If (ReportNavPH.IncludeTextFields) Then

            End If

            If (ReportNavPH.IncludeComments AndAlso TotalsReport.HasComments) Then
                gdvComments.HeaderRow.Height = headerHeight
            End If
        End Sub

        Protected Sub SingleFieldGridView_RowDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewRowEventArgs) Handles gdvWalkaboutUnitVisitsHours.RowDataBound, gdvHumanPerformanceImprovement.RowDataBound, gdvDeaths1.RowDataBound, gdvDeaths2.RowDataBound, gdvReferrals1.RowDataBound, gdvReferrals2.RowDataBound, gdvReferrals3.RowDataBound, gdvRMUInteraction1.RowDataBound, gdvRMUInteraction2.RowDataBound, gdvRMUInteraction3.RowDataBound, gdvDemographics_Age.RowDataBound, gdvDemographics_Gender.RowDataBound, gdvDemographics_Rank.RowDataBound, gdvDemographics_MaritalStatus.RowDataBound, gdvDemographics_Ethnicity.RowDataBound, gdvTrendingActivity_Misc.RowDataBound
            If (e.Row.RowType <> DataControlRowType.DataRow OrElse TotalsReport Is Nothing) Then
                Exit Sub
            End If

            Dim fieldId As Integer = Integer.Parse(CType(e.Row.FindControl("hdfFieldId"), HiddenField).Value)

            ' Check if this row is being edited or not
            If e.Row.RowIndex <> gdvHumanPerformanceImprovement.EditIndex Then
                CType(e.Row.FindControl("lblFieldName"), Label).Text = PHDao.GetFieldById(fieldId).Name
            End If
        End Sub

#End Region

#Region "Excel Procedures..."

        Protected Sub gdvExcelResults_RowDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewRowEventArgs) Handles gdvExcelResults.RowDataBound
            If (e.Row.RowType <> DataControlRowType.DataRow OrElse TotalsReport Is Nothing) Then
                Exit Sub
            End If

            Dim sectionId As Integer = Integer.Parse(CType(e.Row.FindControl("hdfSectionId"), HiddenField).Value)
            Dim fieldId As Integer = Integer.Parse(CType(e.Row.FindControl("hdfFieldId"), HiddenField).Value)
            Dim fieldTypeId As Integer = Integer.Parse(CType(e.Row.FindControl("hdfFieldTypeId"), HiddenField).Value)

            Dim formField As PHFormField = TotalsReport.Form.GetField(sectionId, fieldId, fieldTypeId) ' PHDao.GetFormFieldByIds(sectionId, fieldId, fieldTypeId)

            If (formField Is Nothing OrElse Not formField.IsValid()) Then
                Exit Sub
            End If

            ' Check if this row is being edited or not
            If e.Row.RowIndex <> gdvExcelResults.EditIndex Then
                CType(e.Row.FindControl("lblSectionName"), Label).Text = formField.Section.Name
                CType(e.Row.FindControl("lblFieldName"), Label).Text = formField.Field.Name
                CType(e.Row.FindControl("lblFieldTypeName"), Label).Text = formField.FieldType.Name
            End If
        End Sub

        Private Sub AddTableCell(ByVal row As HtmlTableRow, ByVal text As String)
            Dim cell As New HtmlTableCell
            cell.InnerHtml = text
            row.Cells.Add(cell)
        End Sub

        Private Sub BuildExcelBaseTotalsRows(ByVal table As HtmlTable)
            Dim first As Boolean = True
            Dim odd As Boolean = False
            Dim controls() As String = {"lblSectionName", "lblFieldName", "lblFieldTypeName"}
            Dim row As New HtmlTableRow()

            ' Add header row...
            row.Attributes.Add("style", EXCEL_HEADER_ROW_STYLE)

            For Each cell As TableCell In gdvExcelResults.HeaderRow.Cells
                If (first) Then
                    first = False
                    Continue For
                End If

                AddTableCell(row, cell.Text)
            Next

            table.Rows.Add(row)

            ' Add data rows...
            For Each gridRow As GridViewRow In gdvExcelResults.Rows
                row = New HtmlTableRow
                Dim bgColor As String = IIf(odd, "#d8d8ff;", "#FFF;")
                odd = Not odd

                row.Attributes.Add("style", EXCEL_DATA_ROW_STYLE + bgColor)
                table.Rows.Add(row)

                Dim i As Integer = -1
                For Each cell As TableCell In gridRow.Cells
                    If (i >= 0 AndAlso i < 3) Then
                        AddTableCell(row, CType(cell.FindControl(controls(i)), Label).Text)
                    ElseIf (i > 2) Then
                        AddTableCell(row, cell.Text)
                    End If

                    i = i + 1
                Next
            Next
        End Sub

        Private Sub BuildExcelCommentsRows(ByVal table As HtmlTable)
            If (ReportNavPH.IncludeComments AndAlso TotalsReport.HasComments) Then
                Dim odd As Boolean = False
                Dim row As New HtmlTableRow()

                ' Add empty row...
                table.Rows.Add(New HtmlTableRow)

                ' Add header row...
                row.Attributes.Add("style", EXCEL_HEADER_ROW_STYLE)

                For Each cell As TableCell In gdvExcelCommentResults.HeaderRow.Cells
                    AddTableCell(row, cell.Text)
                Next

                table.Rows.Add(row)

                ' Add data rows...
                For Each gridRow As GridViewRow In gdvExcelCommentResults.Rows
                    row = New HtmlTableRow
                    Dim bgColor As String = IIf(odd, "#d8d8ff;", "#FFF;")
                    odd = Not odd

                    row.Attributes.Add("style", EXCEL_TEXT_ALIGN_STYLE & EXCEL_DATA_ROW_STYLE & bgColor)
                    table.Rows.Add(row)

                    Dim i As Integer = -1
                    For Each cell As TableCell In gridRow.Cells
                        AddTableCell(row, cell.Text)
                    Next
                Next
            End If
        End Sub

        Private Sub BuildExcelStringValuesRows(ByVal table As HtmlTable)
            If (ReportNavPH.IncludeTextFields AndAlso TotalsReport.StringValues.Count > 0) Then
                Dim odd As Boolean = False
                Dim row As New HtmlTableRow()

                ' Add empty row...
                table.Rows.Add(New HtmlTableRow)

                ' Add header row...
                row.Attributes.Add("style", EXCEL_HEADER_ROW_STYLE)

                For Each cell As TableCell In gdvExcelStringValueResults.HeaderRow.Cells
                    AddTableCell(row, cell.Text)
                Next

                table.Rows.Add(row)

                ' Add data rows...
                For Each gridRow As GridViewRow In gdvExcelStringValueResults.Rows
                    row = New HtmlTableRow
                    Dim bgColor As String = IIf(odd, "#d8d8ff;", "#FFF;")
                    odd = Not odd

                    row.Attributes.Add("style", EXCEL_TEXT_ALIGN_STYLE & EXCEL_DATA_ROW_STYLE & bgColor)
                    table.Rows.Add(row)

                    Dim i As Integer = -1
                    For Each cell As TableCell In gridRow.Cells
                        AddTableCell(row, cell.Text)
                    Next
                Next
            End If
        End Sub

        Private Sub BuildExcelSuicideMethodTotalsRows(ByVal table As HtmlTable)
            Dim odd As Boolean = False
            Dim row As New HtmlTableRow()

            ' Add empty row...
            table.Rows.Add(New HtmlTableRow)

            ' Add header row...
            row.Attributes.Add("style", EXCEL_HEADER_ROW_STYLE)

            For Each cell As TableCell In gdvExcelSuicideMethodTotals.HeaderRow.Cells
                AddTableCell(row, cell.Text)
            Next

            table.Rows.Add(row)

            ' Add data rows...
            For Each gridRow As GridViewRow In gdvExcelSuicideMethodTotals.Rows
                row = New HtmlTableRow
                Dim bgColor As String = IIf(odd, "#d8d8ff;", "#FFF;")
                odd = Not odd

                row.Attributes.Add("style", EXCEL_DATA_ROW_STYLE & bgColor)
                table.Rows.Add(row)

                Dim i As Integer = -1
                For Each cell As TableCell In gridRow.Cells
                    AddTableCell(row, cell.Text)
                Next
            Next
        End Sub

        Private Sub ResultsToExcel()
            If (TotalsReport Is Nothing OrElse Not TotalsReport.HasExecuted) Then
                ResultsPanel.Visible = True
                Exit Sub
            End If

            ResultsPanel.Visible = True

            ' Populate excel gridviews with report results data...
            gdvExcelResults.DataSource = TotalsReport.Totals
            gdvExcelResults.DataBind()

            gdvExcelSuicideMethodTotals.DataSource = TotalsReport.GetSuicideMethodTotals()
            gdvExcelSuicideMethodTotals.DataBind()

            If (ReportNavPH.IncludeTextFields) Then
                gdvExcelStringValueResults.DataSource = TotalsReport.StringValues
                gdvExcelStringValueResults.DataBind()
            End If

            If (ReportNavPH.IncludeComments) Then
                gdvExcelCommentResults.DataSource = TotalsReport.Comments
                gdvExcelCommentResults.DataBind()
            End If

            Dim table As New HtmlTable

            BuildExcelBaseTotalsRows(table)
            BuildExcelSuicideMethodTotalsRows(table)
            BuildExcelStringValuesRows(table)
            BuildExcelCommentsRows(table)

            ' Render HTML and send as an excel file...
            Dim writer As New System.IO.StringWriter
            Dim html As New HtmlTextWriter(writer)
            table.RenderControl(html)

            Response.Clear()
            Response.AddHeader("content-disposition", "attachment;filename=PHTotalsReport" + Date.Now.ToString("yyyyMMdd") + ".xls")
            Response.Charset = ""
            Response.ContentType = "application/ms-excel"
            Response.Write(writer.ToString())
            Response.End()
        End Sub

#End Region

    End Class

End Namespace