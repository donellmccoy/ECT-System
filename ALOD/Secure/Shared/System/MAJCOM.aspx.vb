Imports ALODWebUtility.Common

Namespace Web.Sys

    Partial Class Secure_Shared_System_MAJCOM
        Inherits System.Web.UI.Page

        Protected Sub btnAdd_Click(sender As Object, e As System.EventArgs) Handles btnAdd.Click

            If (Page.IsValid) Then
                Dim mc As New majcom()
                mc.InsertMAJCOM(0, txtName.Text.Trim, 0)
                GridView1.DataBind()

            End If

        End Sub

    End Class

End Namespace