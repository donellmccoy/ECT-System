Namespace Web.Admin

    Partial Class Secure_Shared_Admin_MappedValues
        Inherits System.Web.UI.Page

        Public Sub ddlKeyTypes_dataBound(ByVal sender As Object, ByVal e As System.EventArgs) Handles ddlKeyTypes.DataBound

        End Sub

        Public Sub ddlKeyTypes_selectedValues(ByVal sender As Object, ByVal e As System.EventArgs) Handles ddlKeyTypes.SelectedIndexChanged
            Values.Update()
        End Sub

    End Class

End Namespace