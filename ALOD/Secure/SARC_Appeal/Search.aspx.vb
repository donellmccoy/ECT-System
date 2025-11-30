Imports ALOD.Core.Domain.Workflow
Imports ALOD.Core.Utils
Imports ALOD.Core.Utils.RegexValidation
Imports ALODWebUtility.Common
Imports ALODWebUtility.Permission.Search

Namespace Web.APSA

    Public Class Search
        Inherits System.Web.UI.Page

        Protected Sub ddlLodStatus_DataBound(ByVal sender As Object, ByVal e As System.EventArgs) Handles StatusSelect.DataBound
            StatusSelect.Items.Insert(0, New ListItem("-- All --", String.Empty))
        End Sub

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
            If (Not IsPostBack) Then

                Dim task As PageAsyncTask = New PageAsyncTask(New BeginEventHandler(AddressOf BeginAsyncUnitLookup), New EndEventHandler(AddressOf EndAsyncUnitLookup), Nothing, GetStateObject(UnitSelect))
                RegisterAsyncTask(task)
                PreloadSearchFilters()
                ResultGrid.Sort("CaseId", SortDirection.Ascending)

                SetInputFormatRestriction(Page, SsnBox, FormatRestriction.Numeric, "-")
                SetInputFormatRestriction(Page, NameBox, FormatRestriction.AlphaNumeric, PERMITTED_SPECIAL_CHAR_INPUT)
                'SetInputFormatRestriction(Page, txtLastName, FormatRestriction.AlphaNumeric, PERMITTED_SPECIAL_CHAR_INPUT)
                'SetInputFormatRestriction(Page, txtFirstName, FormatRestriction.AlphaNumeric, PERMITTED_SPECIAL_CHAR_INPUT)
                'SetInputFormatRestriction(Page, txtMiddleName, FormatRestriction.AlphaNumeric, PERMITTED_SPECIAL_CHAR_INPUT)
                SetInputFormatRestriction(Page, CaseIdBox, FormatRestriction.AlphaNumeric, "-")

            End If
        End Sub

        Protected Sub ResultGrid_RowDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewRowEventArgs) Handles ResultGrid.RowDataBound
            HeaderRowBinding(sender, e, "CaseId")

            If (e.Row.RowType = DataControlRowType.DataRow) Then

                Dim data As DataRowView = CType(e.Row.DataItem, DataRowView)
                Dim lockId As Integer = 0
                Integer.TryParse(data("lockId"), lockId)

                If (lockId > 0) Then
                    CType(e.Row.FindControl("LockImage"), Image).Visible = True
                End If

            End If

        End Sub

        Protected Sub SearchButton_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles SearchButton.Click
            ResultGrid.DataBind()
        End Sub

        Protected Sub SearchData_Selecting(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.ObjectDataSourceSelectingEventArgs) Handles SearchData.Selecting
            NoFiltersPanel.Visible = False

            If (SsnBox.Text.Trim.Length = 0 AndAlso
                NameBox.Text.Trim.Length = 0 AndAlso
                CaseIdBox.Text.Trim.Length = 0 AndAlso
                StatusSelect.SelectedIndex = 0 AndAlso
                UnitSelect.SelectedIndex < 1) Then
                NoFiltersPanel.Visible = True
                e.Cancel = True
                Exit Sub
            End If

        End Sub

        Private Sub PreloadSearchFilters()

            If (Request.QueryString("data") Is Nothing) Then
                Exit Sub
            End If

            Dim data As String = Request.QueryString("data")

            If (IsNumeric(data)) Then
                If (data.Length = 4) Then
                    'assume SSN
                    SsnBox.Text = data
                Else
                    'assume caseId
                    CaseIdBox.Text = data
                End If
            Else
                'could be either a name, or caseid

                If (IsValidCaseId(data)) Then
                    CaseIdBox.Text = data
                Else
                    'txtLastName.Text = data
                    NameBox.Text = data
                End If

            End If

        End Sub

        Private Sub ResultGrid_RowCommand(ByVal sender As Object, ByVal e As GridViewCommandEventArgs) Handles ResultGrid.RowCommand

            If (e.CommandName = "view") Then

                ' Dim query As New SecureQueryString()
                Dim parts() As String = e.CommandArgument.ToString().Split(";")
                Dim strQuery As New StringBuilder()
                Dim args As New ItemSelectedEventArgs
                args.RefId = CInt(parts(0))

                strQuery.Append("requestId=" + CType(args.RefId, String))
                args.Type = ModuleType.SARCAppeal

                If (UserHasPermission(GetViewPermissionByModuleId(args.Type))) Then
                    Select Case args.Type
                        Case ModuleType.SARCAppeal
                            args.Url = "~/Secure/SARC_Appeal/init.aspx?" + strQuery.ToString()
                    End Select
                Else
                    args.Url = "~/Secure/Shared/SecureAccessDenied.aspx?deniedType=1"
                End If

                Response.Redirect(args.Url)
            End If

        End Sub

    End Class

End Namespace