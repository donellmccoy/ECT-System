Imports ALOD.Core.Domain.Users
Imports ALOD.Core.Utils
Imports ALOD.Data.Services
Imports ALODWebUtility.Common
Imports ALODWebUtility.LookUps

Namespace Web.Admin

    Partial Class Secure_Shared_Admin_ManageUsers
        Inherits System.Web.UI.Page

        Public Function AdminCanEdit(ByVal groupId As Integer) As Boolean

            Dim userGroupId As Integer = CInt(Session("groupId"))
            'Dim roleName As String = CStr(Session("RoleName"))

            If (userGroupId = 1) Then
                Return True
            End If

            'End If
            If (userGroupId = groupId) Then
                Return False
            End If

            Return True

        End Function

        Public Function AdminCanEdit(ByVal groupId As Integer, ByVal permission As Boolean) As Boolean

            Dim userGroupId As Integer = CInt(Session("groupId"))

            If (userGroupId = 1) Then
                Return True
            End If

            If (userGroupId = groupId) Then
                Return False
            End If
            '-----------permission: 0 is view; 1 is managed------------
            If (permission = 0) Then
                Return False
            End If

            Return True

        End Function

        Protected Sub AddTableCell(ByVal row As HtmlTableRow, ByVal text As String)
            Dim cell As New HtmlTableCell
            cell.InnerHtml = text
            row.Cells.Add(cell)
        End Sub

        Protected Sub btnExport_Click(ByVal sender As Object, ByVal e As System.EventArgs)

            Response.Clear()
            Response.AddHeader("content-disposition", "attachment;filename=LodUsers_" + Date.Now.ToString("yyyyMMdd") + ".xls")
            Response.Charset = ""
            Response.ContentType = "application/ms-excel"

            Dim table As New HtmlTable

            'add the header row
            Dim row As New HtmlTableRow()
            row.Attributes.Add("style", "background-color:navy;text-align: center; border: 1px solid #CCC; border-bottom: solid 1px black; color:white")

            'add the header cells
            AddTableCell(row, "STATUS")

            AddTableCell(row, "NAME")
            AddTableCell(row, "SSN")
            AddTableCell(row, "ExpirationDate")
            AddTableCell(row, "ROLE")
            AddTableCell(row, "UNIT")

            table.Rows.Add(row)

            Dim odd As Boolean = False
            Dim data As System.Data.DataSet = GetSearchData()

            For Each item As System.Data.DataRow In data.Tables(0).Rows

                row = New HtmlTableRow
                Dim bgColor As String = IIf(odd, "#d8d8ff;", "#FFF;")
                odd = Not odd

                row.Attributes.Add("style", "border: 1px solid #CCC; border-bottom: solid 1px #C0C0C0; background-color:" + bgColor)
                table.Rows.Add(row)

                AddTableCell(row, item("AccessStatusText").ToString())
                AddTableCell(row, item("LastName").ToString() + ", " + item("FirstName").ToString())
                AddTableCell(row, item("LastFour").ToString())
                AddTableCell(row, item("expirationDate").ToString())
                AddTableCell(row, item("RoleName").ToString())
                AddTableCell(row, item("CurrentUnitName").ToString())

            Next

            Dim writer As New System.IO.StringWriter
            Dim html As New HtmlTextWriter(writer)

            table.RenderControl(html)
            Response.Write(writer.ToString())
            Response.End()
        End Sub

        Protected Function GetSearchData() As DataSet

            Dim status As Byte = 0, unit As Integer = 0, role As Integer = 0, workCompo As Integer = 0

            If (StatusSelect.SelectedValue <> "0") Then
                status = CByte(StatusSelect.SelectedValue)
            End If

            If (UnitSelect.SelectedValue <> "0") Then
                unit = CInt(UnitSelect.SelectedValue)
            End If

            If (RoleSelect.SelectedValue <> "0") Then
                role = CInt(RoleSelect.SelectedValue)
            End If

            Return New LookUp().SearchUsers(Session("UserId"), SsnText.Text.Trim(), NameText.Text.Trim(), status, role, unit, CheckBoxShowAllUsers.Checked)

        End Function

        Protected Function GetSearchParameters() As Dictionary(Of String, String)

            Dim status As String = String.Empty
            Dim unit As String = String.Empty
            Dim role As String = String.Empty

            Dim ssn As String = Server.HtmlEncode(SsnText.Text.Trim())
            Dim name As String = Server.HtmlEncode(NameText.Text.Trim())
            status = Server.HtmlEncode(StatusSelect.SelectedValue)
            unit = Server.HtmlEncode(UnitSelect.SelectedValue)
            role = Server.HtmlEncode(RoleSelect.SelectedValue)

            Dim srchParam As New Dictionary(Of String, String)
            srchParam.Add("ssn", ssn)
            srchParam.Add("name", name)
            srchParam.Add("role", role)
            srchParam.Add("unit", unit)
            srchParam.Add("status", status)
            Return srchParam

        End Function

        Protected Sub InitControls()

            'check for passed in params
            If (Request.QueryString("status") IsNot Nothing) Then
                Dim status As Integer = 0
                Integer.TryParse(Request.QueryString("status"), status)
                SetDropdownByValue(StatusSelect, status.ToString())
            End If

            'populate the roles dropdown
            Dim lookup As New LookUp()
            Dim list As New List(Of DataSet) From {lookup.GetManagedGroupsDropDown(CInt(Session("groupId")))}
            RoleSelect.DataSource = New LookUp().GetManagedGroupsDropDown(CInt(Session("groupId")))
            For Each x As DataSet In list
                Convert.ToString(lookup.GetManagedGroupsDropDown(CInt(Session("groupId"))).Tables(0).Columns(0))

            Next
            Convert.ToString(lookup.GetManagedGroupsDropDown(CInt(Session("groupId"))).Tables(0).Columns(1))

            If (Request.QueryString("role") IsNot Nothing) Then
                Dim role As Integer = 0
                Integer.TryParse(Request.QueryString("role"), role)
                SetDropdownByValue(StatusSelect, role.ToString())
            End If
            RoleSelect.DataBind()
            RoleSelect.Items.Insert(0, New ListItem("All", "0"))
            Dim t As String = CStr(Session("RoleName"))

            Dim lst As List(Of ALOD.Core.Domain.Users.LookUpItem) = LookupService.GetChildUnits(CInt(HttpContext.Current.Session("UnitId")), CByte(HttpContext.Current.Session("ReportView")))
            UnitSelect.DataSource = lst
            UnitSelect.DataTextField = "Name"
            UnitSelect.DataValueField = "Value"
            UnitSelect.DataBind()
            UnitSelect.Items.Insert(0, New ListItem("All", "0"))

        End Sub

        Protected Sub LoadPrevSearch()

            Dim srchParam As Dictionary(Of String, String) = CType(Session("ManagedUserSrchPara"), Dictionary(Of String, String))
            StatusSelect.SelectedValue = srchParam("status")
            UnitSelect.SelectedValue = srchParam("unit")
            RoleSelect.SelectedValue = srchParam("role")
            SsnText.Text = srchParam("ssn")
            NameText.Text = srchParam("name")
            UsersGrid.DataBind()
        End Sub

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

            If (Not IsPostBack) Then
                InitControls()
                If Not (Session("ManagedUserSrchPara") Is Nothing) Then
                    LoadPrevSearch()
                End If
                CheckBoxShowAllUsers.Visible = False
                If UserHasPermission("sysAdmin") AndAlso AppMode <> DeployMode.Production Then
                    CheckBoxShowAllUsers.Visible = True
                End If
                'set the default sort
                UsersGrid.Sort("LastName", SortDirection.Ascending)

            End If
            Session("EditId") = Nothing
            Session("ManagedUserSrchPara") = Nothing
        End Sub

        Protected Sub UserData_Selecting(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.ObjectDataSourceSelectingEventArgs) Handles UserData.Selecting

            Dim ssn As String = SsnText.Text.Trim
            Dim name As String = NameText.Text.Trim
            Dim status As String = StatusSelect.SelectedValue
            Dim role As String = RoleSelect.SelectedValue
            Dim unitId As String = UnitSelect.SelectedValue

            If (ssn.Length = 0 AndAlso name.Length = 0 AndAlso status = "0" AndAlso role = "0" AndAlso unitId = "0") Then
                e.Cancel = True
                SearchMessage.Visible = True
                UsersGrid.CssClass = "hidden"
            Else
                e.Cancel = False
                SearchMessage.Visible = False
                UsersGrid.CssClass = "gridViewMain"
            End If

        End Sub

        Protected Sub UsersGrid_DataBound(ByVal sender As Object, ByVal e As System.EventArgs) Handles UsersGrid.DataBound
            If Not (UserHasPermission("sysAdmin")) Then
                'Only sys admin can change the permission
                'UsersGrid.Columns(5).Visible = False
            End If
            UsersGrid.Columns(3).Visible = False
        End Sub

        Protected Sub UsersGrid_RowCommand(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewCommandEventArgs) Handles UsersGrid.RowCommand

            If (e.CommandName = "EditInfo") Then
                Session("EditId") = e.CommandArgument
                Session("ManagedUserSrchPara") = GetSearchParameters()
                Response.Redirect("~/Secure/Shared/Admin/EditUser.aspx?Caller=1", True)
            ElseIf (e.CommandName = "Perms") Then
                Session("EditId") = e.CommandArgument
                Session("ManagedUserSrchPara") = GetSearchParameters()
                Response.Redirect("~/Secure/Shared/System/UserPermissions.aspx", True)
            ElseIf (e.CommandName = "Activity") Then
                Session("EditId") = e.CommandArgument
                Session("ManagedUserSrchPara") = GetSearchParameters()
                Response.Redirect("~/Secure/Shared/Admin/UserTracking.aspx?", True)
            End If

        End Sub

        Protected Sub UsersGrid_RowDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewRowEventArgs) Handles UsersGrid.RowDataBound

            HeaderRowBinding(sender, e, "LastName")

            If (e.Row.RowType <> DataControlRowType.DataRow) Then
                Exit Sub
            End If

            Dim StatusLabel As Label = CType(e.Row.FindControl("StatusLabel"), Label)
            Dim data As DataRowView = CType(e.Row.DataItem, DataRowView)
            Dim status As AccessStatus = data("Status")

            Select Case status
                Case AccessStatus.Approved
                    StatusLabel.ForeColor = Drawing.Color.Green
                Case AccessStatus.Pending
                    StatusLabel.ForeColor = Drawing.Color.OrangeRed
                Case Else
                    StatusLabel.ForeColor = Drawing.Color.Red
            End Select

        End Sub

    End Class

End Namespace