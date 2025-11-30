Imports ALOD.Data.Services

Imports ALODWebUtility.Common

Namespace Web.Admin

    Partial Class Secure_Shared_Admin_PermissionReport
        Inherits System.Web.UI.Page

#Region "Load"

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

            If Not Page.IsPostBack Then
                PopulatePermissions()

                If Not (Session("PermissionUserSrchPara") Is Nothing) Then
                    Dim srchParam As Dictionary(Of String, String) = CType(Session("PermissionUserSrchPara"), Dictionary(Of String, String))
                    PermissionsSelect.SelectedValue = srchParam("perm")
                End If

                UsersGrid.Sort("Name", SortDirection.Ascending)

            End If

            Session("EditId") = Nothing
            Session("PermissionUserSrchPara") = Nothing
        End Sub

        Protected Sub Permissions_Selecting(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.ObjectDataSourceSelectingEventArgs) Handles Permissions.Selecting

            Dim perm As String = Server.HtmlEncode(PermissionsSelect.SelectedValue)

            If (perm.Length = 0) Then
                e.Cancel = True
                SearchMessage.Visible = True
                UsersGrid.CssClass = "hidden"
            Else
                e.Cancel = False
                SearchMessage.Visible = False
                UsersGrid.CssClass = "gridViewMain"
            End If

        End Sub

        Protected Sub PopulatePermissions()
            Dim perms = From w In WorkFlowService.GetPermissions() Where w.Exclude = False Select w Order By w.Description
            PermissionsSelect.DataSource = perms
            PermissionsSelect.DataBind()
            PermissionsSelect.Items.Insert(0, New ListItem("-Select-", ""))
        End Sub

        Protected Sub SearchButton_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles SearchButton.Click
            UsersGrid.DataBind()
        End Sub

#End Region

#Region "EditUser"

        Protected Function GetSearchParameters() As Dictionary(Of String, String)

            Dim perm As String = Server.HtmlEncode(PermissionsSelect.SelectedValue)
            Dim srchParam As New Dictionary(Of String, String)
            srchParam.Add("perm", perm)
            Return srchParam

        End Function

        Protected Sub UsersGrid_RowCommand(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewCommandEventArgs) Handles UsersGrid.RowCommand

            'If (e.CommandName = "EditInfo") Then
            '    Session("EditId") = e.CommandArgument
            '    Session("PermissionUserSrchPara") = GetSearchParameters()
            '    Response.Redirect("~/Secure/Shared/System/UserPermissions.aspx?Caller=4", True)
            'End If
        End Sub

#End Region

#Region "Sorting"

        Protected Sub UsersGrid_RowDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewRowEventArgs) Handles UsersGrid.RowDataBound
            HeaderRowBinding(sender, e, "Name")
        End Sub

#End Region

#Region "Export"

        Protected Sub AddTableCell(ByVal row As HtmlTableRow, ByVal text As String)
            Dim cell As New HtmlTableCell
            cell.InnerHtml = text
            row.Cells.Add(cell)
        End Sub

        Protected Sub btnExport_Click(ByVal sender As Object, ByVal e As System.EventArgs)

            Dim perm As String = Server.HtmlEncode(PermissionsSelect.SelectedValue)

            Dim ds As System.Data.DataSet
            If perm.Length > 0 Then
                Response.Clear()

                Dim odd As Boolean = False
                Dim permName As String = Server.HtmlEncode(PermissionsSelect.SelectedItem.Text)
                ds = UserService.GetUsersWithPermission(CType(perm, Short))
                Response.AddHeader("content-disposition", "attachment;filename=ALodUsersWithPerm_" + permName + "_" + Date.Now.ToString("yyyyMMdd") + ".xls")
                Response.Charset = ""
                Response.ContentType = "application/ms-excel"

                Dim table As New HtmlTable

                'add the header row
                Dim row As New HtmlTableRow()
                row.Attributes.Add("style", "background-color:navy;text-align: center; border: 1px solid #CCC; border-bottom: solid 1px black; color:white")

                'add the header cells

                AddTableCell(row, "NAME")
                AddTableCell(row, "SSN")
                AddTableCell(row, "ROLE")
                AddTableCell(row, "UNIT")
                AddTableCell(row, "STATUS")

                table.Rows.Add(row)

                For Each item As System.Data.DataRow In ds.Tables(0).Rows

                    row = New HtmlTableRow
                    Dim bgColor As String = IIf(odd, "#d8d8ff;", "#FFF;")
                    odd = Not odd

                    row.Attributes.Add("style", "border: 1px solid #CCC; border-bottom: solid 1px #C0C0C0; background-color:" + bgColor)
                    table.Rows.Add(row)
                    AddTableCell(row, item("Name").ToString())
                    AddTableCell(row, item("LastFour").ToString())
                    AddTableCell(row, item("RoleName").ToString())
                    AddTableCell(row, item("CurrentUnitName").ToString())
                    AddTableCell(row, item("AccessStatusDescr").ToString())

                Next

                Dim writer As New System.IO.StringWriter
                Dim html As New HtmlTextWriter(writer)

                table.RenderControl(html)
                Response.Write(writer.ToString())
                Response.End()

            End If
        End Sub

#End Region

    End Class

End Namespace