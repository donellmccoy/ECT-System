Imports ALOD.Core.Domain.Workflow
Imports ALOD.Core.Utils
Imports ALODWebUtility.Common

Namespace Web.SARC

    Public Class PostCompletion
        Inherits System.Web.UI.Page

        Protected Function GetInitURL(ByVal moduleId As Integer)
            Select Case moduleId
                Case ModuleType.LOD
                    Return "~/Secure/lod/init.aspx"

                Case ModuleType.SARC
                    Return "~/Secure/SARC/init.aspx"

                Case Else
                    Return String.Empty
            End Select
        End Function

        Protected Sub gvResults_RowCommand(ByVal sender As Object, ByVal e As GridViewCommandEventArgs) Handles gvResults.RowCommand
            If (e.CommandName = "view") Then
                Dim parts() As String = e.CommandArgument.ToString().Split("|")

                If (parts.Count <> 2) Then
                    Exit Sub
                End If

                Dim refId As Integer = CInt(parts(0))
                Dim moduleId As Integer = CInt(parts(1))
                Dim strQuery As New StringBuilder()
                Dim initURL As String = GetInitURL(moduleId)

                If (refId <= 0 OrElse String.IsNullOrEmpty(initURL)) Then
                    Exit Sub
                End If

                strQuery.Append("?refId=" + refId.ToString())
                Response.Redirect(initURL & strQuery.ToString())
            End If
        End Sub

        Protected Sub gvResults_RowDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewRowEventArgs) Handles gvResults.RowDataBound
            HeaderRowBinding(sender, e, "CaseId")
        End Sub

        Protected Sub InitInputRestrictions()
            SetInputFormatRestriction(Page, SsnBox, FormatRestriction.Numeric, "-")
            SetInputFormatRestriction(Page, NameBox, FormatRestriction.AlphaNumeric, PERMITTED_SPECIAL_CHAR_INPUT)
            SetInputFormatRestriction(Page, CaseIdbox, FormatRestriction.AlphaNumeric, "-")
        End Sub

        Protected Sub InitUnitLookupControl()
            Dim task As PageAsyncTask = New PageAsyncTask(New BeginEventHandler(AddressOf BeginAsyncUnitLookup), New EndEventHandler(AddressOf EndAsyncUnitLookup), Nothing, GetStateObject(UnitSelect))
            RegisterAsyncTask(task)
        End Sub

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
            If (Not IsPostBack) Then
                InitInputRestrictions()
                InitUnitLookupControl()
                gvResults.Sort("DaysCompleted", SortDirection.Descending)
            End If
        End Sub

        Protected Sub SearchButton_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles SearchButton.Click
            gvResults.DataBind()
        End Sub

    End Class

End Namespace