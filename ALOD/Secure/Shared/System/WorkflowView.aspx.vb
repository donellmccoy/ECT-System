Namespace Web.Sys

    Partial Class Secure_Shared_System_WorkflowView
        Inherits System.Web.UI.Page

        Protected Sub WorkFlowSelecting(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.ObjectDataSourceSelectingEventArgs) Handles workflow.Selecting
            e.InputParameters("compo") = CStr(HttpContext.Current.Session("Compo"))
        End Sub

    End Class

End Namespace