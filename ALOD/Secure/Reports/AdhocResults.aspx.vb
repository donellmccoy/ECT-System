Imports ALOD.Core.Domain.Documents
Imports ALOD.Core.Domain.Query
Imports ALOD.Core.Interfaces.DAOInterfaces
Imports ALOD.Core.Utils
Imports ALOD.Data
Imports ALODWebUtility.Common
Imports WebSupergoo.ABCpdf8

Namespace Web.Reports

    Partial Class Secure_Reports_AdhocResults
        Inherits System.Web.UI.Page

        Const DISPLAY_MODE_CSV As String = "c"
        Const DISPLAY_MODE_EXCEL As String = "e"
        Const DISPLAY_MODE_PDF As String = "p"
        Const DISPLAY_MODE_SCREEN As String = "s"
        Const DISPLAY_MODE_XML As String = "x"
        Private _phQueryDao As IPHQueryDao = Nothing
        Private dao As IQueryDao = Nothing

        Protected ReadOnly Property QueryId() As Integer
            Get
                If (Request.QueryString("id") Is Nothing) Then
                    Return 0
                End If

                Return CInt(Request.QueryString("id"))
            End Get
        End Property

        Protected ReadOnly Property ReportType() As Integer
            Get
                If (Request.QueryString("report") Is Nothing) Then
                    Return Convert.ToInt32(AdHocReportType.Standard)
                End If

                Return CInt(Request.QueryString("report"))
            End Get
        End Property

        Private ReadOnly Property DisplayMode() As String
            Get
                If (Request.QueryString("mode") Is Nothing) Then
                    Return DISPLAY_MODE_SCREEN
                End If

                Dim mode As String = Request.QueryString("mode")

                If (mode = "s") Then
                    Return DISPLAY_MODE_SCREEN
                ElseIf (mode = "e") Then
                    Return DISPLAY_MODE_EXCEL
                ElseIf (mode = "p") Then
                    Return DISPLAY_MODE_PDF
                ElseIf (mode = "c") Then
                    Return DISPLAY_MODE_CSV
                ElseIf (mode = "x") Then
                    Return DISPLAY_MODE_XML
                End If

                Return DISPLAY_MODE_SCREEN

            End Get
        End Property

        Private ReadOnly Property PHQueryDao() As IPHQueryDao
            Get
                If (_phQueryDao Is Nothing) Then
                    _phQueryDao = New NHibernateDaoFactory().GetPHQueryDao()
                End If
                Return _phQueryDao
            End Get
        End Property

        Private ReadOnly Property QueryDao() As IQueryDao
            Get
                If (dao Is Nothing) Then
                    dao = New NHibernateDaoFactory().GetQueryDao()
                End If
                Return dao
            End Get
        End Property

        Protected Function ConstructQueryArgs() As UserQueryArgs
            Dim queryArgs As UserQueryArgs = New UserQueryArgs()

            queryArgs.QueryId = QueryId
            queryArgs.UserId = SESSION_USER_ID
            queryArgs.HasSarc = UserHasPermission(PERMISSION_VIEW_SARC_CASES)
            queryArgs.Scope = SESSION_ACCESS_SCOPE
            queryArgs.UnitId = SESSION_UNIT_ID
            queryArgs.ViewType = SESSION_REPORT_VIEW

            Return queryArgs
        End Function

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

            If (Not IsPostBack) Then
                DisplayResults()
            End If

        End Sub

        Private Sub AddTableCell(ByVal row As HtmlTableRow, ByVal text As String)
            Dim cell As New HtmlTableCell
            cell.InnerHtml = text
            row.Cells.Add(cell)
        End Sub

        Private Sub DisplayResults()
            If (QueryId = 0) Then
                Exit Sub
            End If

            Dim queryResults As UserQueryResult = Nothing

            Select Case ReportType
                Case AdHocReportType.PH_Totals
                    queryResults = PHQueryDao.ExecuteTotalsQuery(QueryId, SESSION_USER_ID, SESSION_ACCESS_SCOPE, SESSION_UNIT_ID, SESSION_REPORT_VIEW)
                Case AdHocReportType.Standard
                    queryResults = QueryDao.ExecuteQuery(ConstructQueryArgs())
                Case Else
                    queryResults = QueryDao.ExecuteQuery(ConstructQueryArgs())
            End Select

            If (Not queryResults.HasData) Then
                queryResults.Errors.Add("No data was returned by the query including header information.")
            End If

            If (queryResults.HasErrors) Then
                pnlErrors.Visible = True

                lblQueryId.Text = queryResults.QueryId.ToString()

                If (Not String.IsNullOrEmpty(queryResults.QueryTitle)) Then
                    lblQueryTitle.Text = queryResults.QueryTitle
                Else
                    lblQueryTitle.Text = "UNKNOWN"
                End If

                For Each e As String In queryResults.Errors
                    bllErrors.Items.Add(e)
                Next
                Exit Sub
            End If

            Select Case DisplayMode
                Case DISPLAY_MODE_SCREEN
                    ResultsToScreen(queryResults.ResultData)
                Case DISPLAY_MODE_EXCEL
                    ResultsToExcel(queryResults.ResultData)
                Case DISPLAY_MODE_PDF
                    ResultsToPdf(queryResults.ResultData)
                Case DISPLAY_MODE_CSV
                    ResultsToCsv(queryResults.ResultData)
                Case DISPLAY_MODE_XML
                    ResultsToXml(queryResults.ResultData)
            End Select
        End Sub

        Private Sub ResultsToCsv(ByVal data As DataSet)

            Response.Clear()
            Response.AddHeader("content-disposition", "attachment;filename=AdHocReport" + Date.Now.ToString("yyyyMMdd") + ".csv")
            Response.Charset = ""
            Response.ContentType = "text/plain"

            'add the header row
            Dim buffer As New StringBuilder()
            Dim SSNFound As Boolean
            Dim SSNIndex As Integer
            Dim columnIndex = 1
            Dim columnSize = data.Tables(0).Columns.Count()

            'add the header cells
            For Each column As DataColumn In data.Tables(0).Columns
                buffer.Append(column.ColumnName + ",")
                If SSNFound = False Then
                    If column.ColumnName = "Member SSN" Then
                        SSNFound = True
                        SSNIndex = SSNIndex + 1
                    Else
                        SSNIndex = SSNIndex + 1
                    End If

                End If
            Next

            buffer = buffer.Remove(buffer.Length - 1, 1)
            buffer.Append(Environment.NewLine)

            For Each item As System.Data.DataRow In data.Tables(0).Rows
                For Each column As DataColumn In data.Tables(0).Columns
                    If SSNFound = True And columnIndex = SSNIndex Then
                        buffer.Append("***-**-" + item(column.ColumnName).ToString() + ",")
                    Else
                        buffer.Append(item(column.ColumnName).ToString() + ",")
                    End If
                    If columnIndex >= columnSize Then
                        columnIndex = 1
                    Else
                        columnIndex += 1
                    End If
                Next

                buffer = buffer.Remove(buffer.Length - 1, 1)
                buffer.Append(Environment.NewLine)

            Next

            buffer.Remove(buffer.Length - 1, 1)
            Response.Write(buffer.ToString())
            Response.End()

        End Sub

        Private Sub ResultsToExcel(ByVal data As DataSet)

            Response.Clear()
            Response.AddHeader("content-disposition", "attachment;filename=AdHocReport" + Date.Now.ToString("yyyyMMdd") + ".xls")
            Response.Charset = ""
            Response.ContentType = "application/ms-excel"

            Dim table As New HtmlTable

            'add the header row
            Dim row As New HtmlTableRow()
            row.Attributes.Add("style", "background-color:navy;text-align: center; border: 1px solid #CCC; border-bottom: solid 1px black; color:white")

            'Index Member SSN
            Dim SSNIndex = 0
            Dim SSNFound = False
            Dim columnSize = 0

            columnSize = data.Tables(0).Columns.Count()

            'add the header cells
            For Each column As DataColumn In data.Tables(0).Columns
                AddTableCell(row, column.ColumnName)
                If SSNFound = False Then
                    If column.ColumnName = "Member SSN" Then
                        SSNFound = True
                        SSNIndex = SSNIndex + 1
                    Else
                        SSNIndex = SSNIndex + 1
                    End If

                End If
            Next

            If SSNFound = False Then
                SSNIndex = 0
            End If

            table.Rows.Add(row)

            Dim odd As Boolean = False
            Dim columnIndex = 1

            For Each item As System.Data.DataRow In data.Tables(0).Rows

                row = New HtmlTableRow
                Dim bgColor As String = IIf(odd, "#d8d8ff;", "#FFF;")
                odd = Not odd

                row.Attributes.Add("style", "border: 1px solid #CCC; border-bottom: solid 1px #C0C0C0; background-color:" + bgColor)
                table.Rows.Add(row)

                For Each column As DataColumn In data.Tables(0).Columns
                    If SSNFound = True And columnIndex = SSNIndex Then 'If Member SSN is slected, the add *s
                        AddTableCell(row, "***-**-" + item(column.ColumnName).ToString())
                    Else
                        AddTableCell(row, item(column.ColumnName).ToString())
                    End If
                    If columnIndex >= columnSize Then
                        columnIndex = 1
                    Else
                        columnIndex += 1
                    End If

                Next

            Next

            Dim writer As New System.IO.StringWriter
            Dim html As New HtmlTextWriter(writer)
            table.RenderControl(html)
            Response.Write(writer.ToString())
            Response.End()

        End Sub

        Private Sub ResultsToPdf(ByVal data As DataSet)

            Dim pdf As New Doc()

            'we need to be in landscape to allow as much width as possible
            'apply a rotation transform
            Dim w As Double = pdf.MediaBox.Width
            Dim h As Double = pdf.MediaBox.Height
            Dim l As Double = pdf.MediaBox.Left
            Dim b As Double = pdf.MediaBox.Bottom
            pdf.Transform.Rotate(90, l, b)
            pdf.Transform.Translate(w, 0)

            ' rotate our rectangle
            pdf.Rect.Width = h
            pdf.Rect.Height = w

            'rotate the page
            Dim docId As Integer = pdf.GetInfoInt(pdf.Root, "Pages")
            pdf.SetInfo(docId, "/Rotate", "90")

            pdf.FontSize = 10
            pdf.Rect.Inset(20, 40)

            Dim table As New PdfTable(pdf, 6)
            table.CellPadding = 5
            table.RepeatHeader = True

            Dim i As Integer = 0
            Dim page As Integer = 1
            Dim shade As Boolean = False

            'header row
            table.NextRow()

            Dim colNames As New StringCollection()

            For Each col As DataColumn In data.Tables(0).Columns
                colNames.Add(col.ColumnName)
            Next

            Dim header(colNames.Count) As String
            colNames.CopyTo(header, 0)

            table.AddHtml(header)

            For Each row As DataRow In data.Tables(0).Rows

                table.NextRow()

                Dim cols(colNames.Count) As String

                For index As Integer = 0 To colNames.Count - 1
                    cols(index) = row(row.Table.Columns(index).ColumnName).ToString()
                Next

                table.AddHtml(cols)

                If (pdf.PageNumber > page) Then
                    page = pdf.PageNumber
                    shade = True
                End If

                If (shade) Then
                    '      table.FillRow("216 216 255", table.Row)
                End If

                shade = Not shade
                i = i + 1

            Next

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
                pdf.AddText("Ad-Hoc Report")

                'right
                pdf.HPos = 1.0
                pdf.Rect.SetRect(500, 580, 270, 20)
                pdf.AddText("Generated: " + DateTime.Now.ToString(DATE_HOUR_FORMAT))

                'page number
                pdf.HPos = 0.5
                pdf.Rect.SetRect(20, 20, 750, 20)
                pdf.AddText("Page " + ct.ToString() + " of " + pdf.PageCount.ToString())

                'table header
                pdf.AddLine(20, 552, 772, 552)

            Next

            pdf.Flatten()
            Dim buffer() As Byte = pdf.GetData()

            Response.ContentType = "application/pdf"
            Response.AddHeader("content-disposition", "inline; filename=Tracking.PDF")
            Response.AddHeader("content-length", buffer.Length.ToString())
            Response.BinaryWrite(buffer)
            Response.End()

        End Sub

        Private Sub ResultsToScreen(ByVal data As DataSet)
            pnlResults.Visible = True

            CountLabel.Text = data.Tables(0).Rows.Count.ToString("N0")

            ResultsGrid.DataSource = data
            ResultsGrid.DataBind()
        End Sub

        Private Sub ResultsToXml(ByVal data As DataSet)

            Response.Clear()
            Response.AddHeader("content-disposition", "attachment;filename=AdHocReport" + Date.Now.ToString("yyyyMMdd") + ".xml")
            Response.Charset = ""
            Response.ContentType = "text/xml"

            data.Tables(0).TableName = "LOD"

            Response.Write("<?xml version=""1.0"" encoding=""utf-8"" ?>" + Environment.NewLine)
            data.WriteXml(Response.OutputStream)

            Response.End()

        End Sub

    End Class

End Namespace