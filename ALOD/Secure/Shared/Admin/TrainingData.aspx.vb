Imports ALOD.Core.Utils
Imports ALODWebUtility.Common

Namespace Web.Admin

    Partial Class Secure_Shared_Admin_TrainingData
        Inherits System.Web.UI.Page

        Protected Sub btnExport_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnExport.Click
            DisplayData(cbRegion.SelectedValue, True)
        End Sub

        Protected Sub btnGenerate_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnGenerate.Click

            Dim uics As Integer = 93, soldiers As Integer = 23, region As Integer = 0

            If (IsNumeric(cbRegion.SelectedValue)) Then
                region = Integer.Parse(cbRegion.SelectedValue)
            End If

            If (uics = 0) OrElse (soldiers = 0) OrElse (region = 0) Then
                Exit Sub
            End If

            'Dim data As New TrainingData()
            'data.RebuildTrainingData(region, uics, soldiers)

            DisplayData(region, False)

        End Sub

        Protected Sub btnView_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnView.Click
            DisplayData(cbRegion.SelectedValue, False)
        End Sub

        Protected Sub DisplayData(ByVal region As Integer, ByVal export As Boolean)

            'If (export) Then
            '    Response.Clear()
            '    Response.AddHeader("content-disposition", "attachment;filename=TrainingSoldiers_" + cbRegion.SelectedItem.Text + ".xls")
            '    Response.Charset = ""
            '    Response.ContentType = "application/ms-excel"
            'End If

            'Dim data As New TrainingData
            'Dim users As DataSets.SoldierDataTable = data.GetTrainingSoldiers(region)
            'Dim uic As String = ""

            'Dim table As HtmlTable = Nothing
            'Dim row As HtmlTableRow
            'Dim cell As HtmlTableCell
            'Dim odd As Boolean = False

            'Dim stringWrite As New System.IO.StringWriter
            'Dim htmlWrite As New HtmlTextWriter(stringWrite)

            'For Each user As DataSets.SoldierRow In users

            '    If (uic <> user.UIC) Then

            '        uic = user.UIC

            '        If (table IsNot Nothing) Then
            '            If export Then
            '                table.RenderControl(htmlWrite)
            '            Else
            '                plOutput.Controls.Add(table)
            '            End If
            '        End If

            '        'this is a new unit, so start a new table
            '        table = New HtmlTable()
            '        table.CellPadding = 2
            '        table.CellSpacing = 0
            '        table.Attributes.Add("class", "gridViewMain")
            '        row = New HtmlTableRow()

            '        If (export) Then
            '            row.BgColor = "Gray"
            '            row.BorderColor = "black"
            '        End If

            '        row.Attributes.Add("class", "gridViewHeader")
            '        cell = New HtmlTableCell()
            '        cell.ColSpan = 2
            '        cell.InnerHtml = "Unit: " + user.UIC
            '        row.Align = "center"
            '        row.Cells.Add(cell)
            '        table.Rows.Add(row)
            '        odd = False

            '    End If

            '    row = New HtmlTableRow()
            '    row.Attributes.Add("class", IIf(odd, "gridViewAlternateRow", "gridViewRow").ToString())
            '    odd = Not odd

            '    cell = New HtmlTableCell()
            '    cell.InnerHtml = user.Name
            '    row.Cells.Add(cell)
            '    cell.Width = "300px"

            '    If (export) Then
            '        cell.BorderColor = "black"
            '        cell.Attributes.Add("style", "border-bottom: 1px solid gray;")
            '    End If

            '    cell = New HtmlTableCell()
            '    cell.InnerHtml = IIf(export, "'" + user.SSN + "'", user.SSN).ToString()
            '    row.Cells.Add(cell)
            '    cell.Width = "140px"

            '    If (export) Then
            '        cell.BorderColor = "black"
            '        cell.Attributes.Add("style", "border-bottom: 1px solid gray;")
            '    End If

            '    table.Rows.Add(row)

            'Next

            'If (table IsNot Nothing) Then

            '    'add the last table
            '    If (export) Then
            '        table.RenderControl(htmlWrite)

            '        Response.Write(stringWrite.ToString)
            '        Response.End()
            '    Else
            '        plOutput.Controls.Add(table)
            '    End If

            'End If

        End Sub

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

            'this page is not allowed on prod
            If (AppMode = DeployMode.Production) Then
                Response.Redirect("~/Secure/Welcome.aspx", True)
            End If

        End Sub

    End Class

End Namespace