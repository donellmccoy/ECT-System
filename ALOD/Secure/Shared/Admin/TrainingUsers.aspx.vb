Imports ALOD.Core.Utils
Imports ALODWebUtility.Common

Namespace Web.Admin

    Partial Class Secure_Shared_Admin_TrainingUsers
        Inherits System.Web.UI.Page

        Protected Sub btnExport_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnExport.Click
            DisplayUsers(CInt(cbRegion.SelectedValue), True)
        End Sub

        Protected Sub btnView_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnView.Click
            DisplayUsers(CInt(cbRegion.SelectedValue), False)
        End Sub

        Protected Sub DisplayUsers(ByVal region As Integer, ByVal export As Boolean)

            'If (export) Then
            '    Response.Clear()
            '    Response.AddHeader("content-disposition", "attachment;filename=TrainingUsers_" + cbRegion.SelectedItem.Text + ".xls")
            '    Response.Charset = ""
            '    Response.ContentType = "application/ms-excel"
            'End If

            'Dim training As New TrainingData()
            'Dim users As DataSets.UsersDataTable = training.GetUsersByRegion(region)
            'Dim uic As String = ""

            'Dim table As HtmlTable = Nothing
            'Dim row As HtmlTableRow
            'Dim cell As HtmlTableCell
            'Dim odd As Boolean = False

            'Dim stringWrite As New System.IO.StringWriter
            'Dim htmlWrite As New HtmlTextWriter(stringWrite)

            'For Each user As DataSets.UsersRow In users

            '    If (uic <> user.uic) Then

            '        uic = user.uic

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
            '        cell.ColSpan = 3
            '        cell.InnerHtml = "Unit: " + user.uic
            '        row.Align = "center"
            '        row.Cells.Add(cell)
            '        table.Rows.Add(row)
            '        odd = False

            '    End If

            '    row = New HtmlTableRow()
            '    row.Attributes.Add("class", IIf(odd, "gridViewAlternateRow", "gridViewRow"))
            '    odd = Not odd

            '    cell = New HtmlTableCell()
            '    cell.InnerHtml = user.role
            '    row.Cells.Add(cell)

            '    If (export) Then
            '        cell.BorderColor = "black"
            '        cell.Attributes.Add("style", "border-bottom: 1px solid gray;")
            '    End If

            '    cell = New HtmlTableCell()
            '    cell.InnerHtml = user.ako
            '    row.Cells.Add(cell)

            '    If (export) Then
            '        cell.BorderColor = "black"
            '        cell.Attributes.Add("style", "border-bottom: 1px solid gray;")
            '    End If

            '    cell = New HtmlTableCell()
            '    cell.InnerHtml = user.ssn
            '    row.Cells.Add(cell)

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