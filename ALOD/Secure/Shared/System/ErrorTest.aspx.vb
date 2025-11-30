Namespace Web.Sys

    Partial Class Secure_Shared_System_ErrorTest
        Inherits System.Web.UI.Page

        Protected Sub ErrorButton_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles ErrorButton.Click
            Throw New Exception("Test Exception")
        End Sub

    End Class

End Namespace