Namespace Web.Sys

    Partial Class Secure_Shared_System_Rules
        Inherits System.Web.UI.Page

        Protected Sub DetailsView1_ItemInserted(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.DetailsViewInsertedEventArgs) Handles DetailsView1.ItemInserted
            Response.Redirect(Request.RawUrl)
        End Sub

        Protected Sub gvRules_RowDeleted(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewDeletedEventArgs) Handles gvRules.RowDeleted
            Response.Redirect(Request.RawUrl)
        End Sub

        Protected Sub workflowDataSource_Selecting(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.LinqDataSourceSelectEventArgs) Handles workflowDataSource.Selecting

            Dim ctxLOD As AFLODDataContext = New AFLODDataContext()
            Dim compo As String = CStr(HttpContext.Current.Session("Compo"))
            Dim wfs = From t In ctxLOD.core_Workflows Where t.compo = compo Select t
            Dim qList As List(Of core_Workflow) = wfs.ToList()
            Dim w As New core_Workflow With {.workflowId = 0, .title = "All"}
            qList.Add(w)
            e.Result = qList

        End Sub

    End Class

End Namespace