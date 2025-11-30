Imports ALOD.Core.Domain.Workflow
Imports ALOD.Core.Utils
Imports ALODWebUtility.Common
Imports ALODWebUtility.Permission.Search

Namespace Web.AP

    Partial Class Secure_ap_PostCompletionAppeal
        Inherits System.Web.UI.Page

        Protected Sub AppealData_Selecting(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.ObjectDataSourceSelectingEventArgs) Handles AppealData.Selecting
            e.InputParameters("moduleId") = CByte(ModuleType.AppealRequest)
            e.InputParameters("sarcpermission") = UserHasPermission(PERMISSION_VIEW_SARC_CASES)

            NoFiltersPanel.Visible = False
            If (CheckBoxSearchAllCases.Checked) Then
                If (SsnBox.Text.Trim.Length = 0 AndAlso NameBox.Text.Trim.Length = 0 AndAlso CaseIdbox.Text.Trim.Length = 0) Then
                    e.Cancel = True
                    NoFiltersPanel.Visible = True

                End If
            End If
        End Sub

        Protected Sub gvResults_RowDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewRowEventArgs) Handles gvResults.RowDataBound
            HeaderRowBinding(sender, e, "CaseId")
        End Sub

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
            If (Not IsPostBack) Then
                Dim task As PageAsyncTask = New PageAsyncTask(New BeginEventHandler(AddressOf BeginAsyncUnitLookup), New EndEventHandler(AddressOf EndAsyncUnitLookup), Nothing, GetStateObject(UnitSelect))
                RegisterAsyncTask(task)
                gvResults.Sort("DaysCompleted", SortDirection.Descending)

                SetInputFormatRestriction(Page, SsnBox, FormatRestriction.Numeric, "-")
                SetInputFormatRestriction(Page, NameBox, FormatRestriction.AlphaNumeric, PERMITTED_SPECIAL_CHAR_INPUT)
                SetInputFormatRestriction(Page, CaseIdbox, FormatRestriction.AlphaNumeric, "-")
            End If
        End Sub

        Private Sub gvResults_RowCommand(ByVal sender As Object, ByVal e As GridViewCommandEventArgs) Handles gvResults.RowCommand
            If (e.CommandName = "view") Then
                Dim parts() As String = e.CommandArgument.ToString().Split(";")
                Dim strQuery As New StringBuilder()
                Dim args As New ItemSelectedEventArgs
                args.RefId = CInt(parts(0))

                strQuery.Append("requestId=" + CType(args.RefId, String))
                args.Type = ModuleType.AppealRequest

                Select Case args.Type
                    Case ModuleType.AppealRequest
                        args.Url = "~/Secure/AppealRequests/init.aspx?" + strQuery.ToString()
                        Response.Redirect(args.Url)

                End Select
            End If
        End Sub

    End Class

End Namespace