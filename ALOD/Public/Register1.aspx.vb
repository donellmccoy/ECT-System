Namespace Web

    Partial Class Public_Register1
        Inherits System.Web.UI.Page

        Protected Sub btnAck_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnAck.Click
            Response.Redirect("~/Public/Register2.aspx?")
        End Sub

        Protected Sub btnCancel_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnCancel.Click
            Response.Redirect("~/Default.aspx")
        End Sub

        Protected Sub btnSign_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnSign.Click
            Session("signed") = "1"
            modalPan.Visible = True
            modalExtender.Show()
        End Sub

        Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
            modalPan.Visible = False
        End Sub

    End Class

End Namespace