Imports ALOD.Core.Domain.Workflow
Imports ALODWebUtility
Imports ALODWebUtility.Permission.Search

Namespace Web.UserControls

    Partial Class Secure_Shared_UserControls_SearcherControl
        Inherits System.Web.UI.UserControl

#Region "Events"

        Public Event ItemSelected(ByVal sender As Object, ByVal e As ItemSelectedEventArgs)

#End Region

        Sub DisplaySearchResults()

            Dim ssn As String = txtSSN.Text.Trim
            Dim name As String = txtName.Text.Trim
            Dim caseID As String = txtCaseID.Text.Trim
            Dim status As String = "0"

            Dim ds As DataSets.SearchResultDataTable

            Dim search As New AppSearchList

            ds = search.GetAllLodsAsDataSet(ModuleType.LOD, caseID, ssn, name, status, "")

            lblMessage.Text = "No records found"
            lblMessage.Visible = True
            gvResults.Visible = False
            Dim dv As New DataView
            If ds.Rows.Count > 0 Then
                dv = New DataView(ds)

                gvResults.DataSource = dv
                gvResults.DataBind()
                gvResults.Visible = True
                lblMessage.Visible = False

            End If

        End Sub

        Public Sub SearchButton()
            cmdSearch_Click(Nothing, Nothing)
        End Sub

        Protected Sub cmdSearch_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles cmdSearch.Click

            DisplaySearchResults()
        End Sub

        Private Sub gvResults_RowCommand(ByVal sender As Object, ByVal e As GridViewCommandEventArgs) Handles gvResults.RowCommand

            If (e.CommandName = "view") Then

                ' Dim query As New SecureQueryString()
                Dim parts() As String = e.CommandArgument.ToString().Split(";")
                Dim strQuery As New StringBuilder()
                Dim args As New ItemSelectedEventArgs
                args.RefId = CInt(parts(0))

                strQuery.Append("refId=" + CType(args.RefId, String))
                args.Type = ModuleType.LOD

                Select Case args.Type
                    Case ModuleType.LOD
                        args.Url = "~/Secure/Lod/init.aspx?" + strQuery.ToString()
                        Response.Redirect(args.Url)

                End Select

            End If

        End Sub

    End Class

End Namespace