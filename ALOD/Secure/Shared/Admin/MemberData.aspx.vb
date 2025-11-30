Imports ALOD.Data.Services
Imports ALODWebUtility.Common

Namespace Web.Admin

    Partial Class Secure_Shared_Admin_MemberData
        Inherits System.Web.UI.Page

        Public ReadOnly Property ReportView() As Integer
            Get
                Return CInt(Session("ReportView"))
            End Get

        End Property

        Protected Sub AddTableCell(ByVal row As HtmlTableRow, ByVal text As String)
            Dim cell As New HtmlTableCell
            cell.InnerHtml = text
            row.Cells.Add(cell)
        End Sub

        Protected Sub btnExport_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles ExportButton.Click

            Response.Clear()
            Response.AddHeader("content-disposition", "attachment;filename=LODMembers_" + Date.Now.ToString("yyyyMMdd") + ".xls")
            Response.Charset = ""
            Response.ContentType = "application/ms-excel"

            Dim table As New HtmlTable

            'add the header row
            Dim row As New HtmlTableRow()
            row.Attributes.Add("style", "background-color:navy;text-align: center; border: 1px solid #CCC; border-bottom: solid 1px black; color:white")

            'add the header cells

            AddTableCell(row, "NAME")
            AddTableCell(row, "SSN")
            AddTableCell(row, "UNIT")
            AddTableCell(row, "ROLE")

            table.Rows.Add(row)

            Dim odd As Boolean = False
            Dim data As System.Data.DataSet = GetSearchData()

            For Each item As System.Data.DataRow In data.Tables(0).Rows

                row = New HtmlTableRow
                Dim bgColor As String = IIf(odd, "#d8d8ff;", "#FFF;")
                odd = Not odd

                row.Attributes.Add("style", "border: 1px solid #CCC; border-bottom: solid 1px #C0C0C0; background-color:" + bgColor)
                table.Rows.Add(row)

                AddTableCell(row, item("LastName").ToString() + ", " + item("FirstName").ToString() + " " + item("MiddleName").ToString())
                AddTableCell(row, item("LastFour").ToString())

                AddTableCell(row, item("CurrentUnitName").ToString())
                AddTableCell(row, item("RoleName").ToString())

            Next

            Dim writer As New System.IO.StringWriter
            Dim html As New HtmlTextWriter(writer)

            table.RenderControl(html)
            Response.Write(writer.ToString())
            Response.End()
        End Sub

        Protected Function GetSearchData() As DataSet

            Dim unit As Integer = 0

            If (UnitSelect.SelectedValue <> "0") Then
                unit = CInt(UnitSelect.SelectedValue)
            End If

            Return UserService.SearchMemberData(CInt(Session("UserId")), Server.HtmlEncode(SsnText.Text.Trim()), Server.HtmlEncode(txtLastName.Text.Trim()), Server.HtmlEncode(txtFirstName.Text.Trim()), Server.HtmlEncode(txtMiddleName.Text.Trim()), unit, ReportView)

        End Function

        Protected Function GetSearchParameters() As Dictionary(Of String, String)

            Dim status As String = String.Empty
            Dim unit As String = String.Empty
            Dim role As String = String.Empty

            Dim ssn As String = Server.HtmlEncode(SsnText.Text.Trim())
            Dim lastName As String = Server.HtmlEncode(txtLastName.Text.Trim())
            Dim firstName As String = Server.HtmlEncode(txtFirstName.Text.Trim())
            Dim middleName As String = Server.HtmlEncode(txtMiddleName.Text.Trim())
            unit = Server.HtmlEncode(UnitSelect.SelectedValue)

            Dim srchParam As New Dictionary(Of String, String)
            srchParam.Add("ssn", ssn)
            srchParam.Add("lastName", lastName)
            srchParam.Add("firstName", firstName)
            srchParam.Add("middleName", middleName)
            srchParam.Add("unit", unit)
            Return srchParam

        End Function

        Protected Sub InitControls()

            Dim lst As List(Of ALOD.Core.Domain.Users.LookUpItem) = LookupService.GetChildUnits(CInt(HttpContext.Current.Session("UnitId")), CByte(HttpContext.Current.Session("ReportView")))
            UnitSelect.DataSource = lst
            UnitSelect.DataTextField = "Name"
            UnitSelect.DataValueField = "Value"
            UnitSelect.DataBind()
            UnitSelect.Items.Insert(0, New ListItem("All", "0"))

        End Sub

        Protected Sub LoadPrevSearch()

            Dim srchParam As Dictionary(Of String, String) = CType(Session("MemberDataSrchPara"), Dictionary(Of String, String))

            UnitSelect.SelectedValue = srchParam("unit")
            SsnText.Text = srchParam("ssn")
            txtLastName.Text = srchParam("lastName")
            txtFirstName.Text = srchParam("firstName")
            txtMiddleName.Text = srchParam("middleName")
            MembersGrid.DataBind()
        End Sub

        Protected Sub MemberData_Selecting(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.ObjectDataSourceSelectingEventArgs) Handles MemberData.Selecting

            Dim ssn As String = SsnText.Text.Trim
            'Dim name As String = NameText.Text.Trim
            Dim lastName As String = txtLastName.Text.Trim()
            Dim firstName As String = txtFirstName.Text.Trim()
            Dim middleName As String = txtMiddleName.Text.Trim()

            Dim unitId As String = UnitSelect.SelectedValue

            If (ssn.Length = 0 AndAlso lastName.Length = 0 AndAlso firstName.Length = 0 AndAlso middleName.Length = 0 AndAlso unitId = "0") Then
                e.Cancel = True
                SearchMessage.Visible = True
                MembersGrid.CssClass = "hidden"
            Else
                e.Cancel = False
                SearchMessage.Visible = False
                MembersGrid.CssClass = "gridViewMain"
            End If

        End Sub

        Protected Sub MembersGrid_DataBound(ByVal sender As Object, ByVal e As System.EventArgs) Handles MembersGrid.DataBound
            If Not (UserHasPermission("sysAdmin")) Then
                'Create user link should be invisible
                MembersGrid.Columns(4).Visible = False
            End If

        End Sub

        Protected Sub MembersGrid_RowCommand(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewCommandEventArgs) Handles MembersGrid.RowCommand
            Dim index As Integer
            If (e.CommandName = "CreateUser") Then
                ' Convert the row index stored in the CommandArgument
                ' property to an Integer.
                index = Convert.ToInt32(e.CommandArgument)
                ' Retrieve the row that contains the button clicked
                ' by the user from the Rows collection.
                Session("NewActSSN") = MembersGrid.DataKeys(index)(0)
                Session("MemberDataSrchPara") = GetSearchParameters()
                Response.Redirect("~/Secure/Shared/Admin/CreateUser.aspx", True)
            ElseIf (e.CommandName = "EditUserInfo") Then
                Session("EditId") = e.CommandArgument
                Session("MemberDataSrchPara") = GetSearchParameters()
                Response.Redirect("~/Secure/Shared/Admin/EditUser.aspx?Caller=2", True)
            ElseIf (e.CommandName = "EditMemberInfo") Then
                index = Convert.ToInt32(e.CommandArgument)
                Session(SESSIONKEY_EDIT_MEMBER_SSAN) = MembersGrid.DataKeys(index)(0)
                Session("MemberDataSrchPara") = GetSearchParameters()
                Response.Redirect("~/Secure/Shared/Admin/EditMember.aspx", True)
            End If
        End Sub

        Protected Sub MembersGrid_RowDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewRowEventArgs) Handles MembersGrid.RowDataBound
            HeaderRowBinding(sender, e, "RoleName")
        End Sub

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

            If (Not IsPostBack) Then
                InitControls()

                If Not (Session("MemberDataSrchPara") Is Nothing) Then
                    LoadPrevSearch()
                End If

                MembersGrid.Sort("RoleName", SortDirection.Descending)

            End If
            Session("EditId") = Nothing
            Session("MemberDataSrchPara") = Nothing
            Session("NewActSSN") = Nothing
        End Sub

        Protected Sub SearchButton_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles SearchButton.Click

            MembersGrid.DataBind()
        End Sub

    End Class

End Namespace