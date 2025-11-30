Namespace Web.UserControls

    Partial Class Secure_Shared_UserControls_FeedbackPanel
        Inherits System.Web.UI.UserControl

        Protected _text As String = String.Empty

        Public Property Text() As String
            Get
                Return _text
            End Get
            Set(ByVal value As String)
                _text = value
            End Set
        End Property

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

            _text = ""
            Page.ClientScript.RegisterClientScriptInclude("jQueryColor", Request.ApplicationPath + "/Script/jquery.color.js")
            Page.ClientScript.RegisterClientScriptInclude("feedbackPanel", Request.ApplicationPath + "/Script/FeedbackPanel.js")

        End Sub

        Protected Sub Page_PreRender(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.PreRender

            If (Me.Text.Length > 0) Then
                lblText.Text = Server.HtmlEncode(_text)
            Else
                lblText.Text = ""
            End If

        End Sub

    End Class

End Namespace