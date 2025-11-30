Namespace Web.UserControls

    Public Class UserControl_ModalDialog

        Inherits System.Web.UI.UserControl

        'Private _redirectURL As String
        'Private _timeout As Integer

        Public Property RedirectURL As String
        Public Property Timeout As Integer

        Protected Sub btnExit_Click(sender As Object, e As EventArgs)
            'Response.Redirect(RedirectURL)
        End Sub

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
            AddKeepAlive()

            If Page.IsPostBack = False Then

                'ButtonContinue.Attributes.Add("onClick", "startTimer();");
                'btnExit.Attributes.Add("onclick", "return exitWebsite('" + Page.ResolveUrl("~/public/logout.aspx") + "');");
            End If

        End Sub

        Private Sub AddKeepAlive()
            'How long will popup be up in seconds
            Dim timeout As Integer = Integer.Parse(ConfigurationManager.AppSettings("timeoutDialog")) * 60
            'How long idle time before pop up appears
            Dim milliSecondsTimeout As Integer = Integer.Parse(ConfigurationManager.AppSettings("timeoutPrompt")) * 60000

            Dim Buffer As New System.Text.StringBuilder()

            Buffer.Append(Environment.NewLine)
            Buffer.Append("<script type='text/javascript'>" + Environment.NewLine)
            Buffer.Append("$_TD_DELAY=" + timeout.ToString() + ";" + Environment.NewLine)
            Buffer.Append("$_TD_MS_TIMEOUT=" + milliSecondsTimeout.ToString() + ";" + Environment.NewLine)
            Buffer.Append("$_TD_CONTINUE_BUTTON='" + ButtonContinue.ClientID + "';" + Environment.NewLine)
            Buffer.Append("</script>")

            Page.ClientScript.RegisterClientScriptBlock(Me.GetType().BaseType, "TimeoutVars", Buffer.ToString())
            'Page.ClientScript.RegisterClientScriptInclude("TimeoutDialog", Request.ApplicationPath + "/Script/TimeoutDialog.js")
            Page.ClientScript.RegisterClientScriptInclude("TimeoutDialog", ResolveClientUrl("~/Script/TimeoutDialog.js"))

        End Sub

    End Class

End Namespace