Imports ALOD.Logging

Namespace Web.Admin

    Partial Class Secure_Shared_Admin_ErrorLog
        Inherits System.Web.UI.Page

        Private Const charCount As Integer = 65

        Protected Sub gvAllErrors_RowDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewRowEventArgs) Handles gvAllErrors.RowDataBound

            If (e.Row.RowType <> DataControlRowType.DataRow) Then
                Exit Sub
            End If

            Dim row As DataRowView = CType(e.Row.DataItem, DataRowView)

            Dim lblMessage As Label = CType(e.Row.FindControl("lblMessage"), Label)
            If (lblMessage.Text.Length > charCount) Then
                lblMessage.ToolTip = Server.HtmlEncode(lblMessage.Text)
                lblMessage.Text = Server.HtmlEncode(lblMessage.Text.Substring(0, charCount)) + "<b> ...</b>"
            End If

            Dim lblPage As Label = CType(e.Row.FindControl("lblPage"), Label)

            'The following code is trying to find out the page that is having the error. It tries to find this in the error string.
            'Sadly, it chokes from bad logic if there is a slash char in the string after the url path for the page.
            'We can do better than that.
            'If (lblPage.Text.IndexOf("?") = -1) Then
            '    lblPage.ToolTip = Server.HtmlEncode(lblPage.Text)
            '    lblPage.Text = Server.HtmlEncode(lblPage.Text.Substring(lblPage.Text.LastIndexOf("/") + 1))
            'Else
            '    lblPage.ToolTip = Server.HtmlEncode(lblPage.Text)
            '    Dim startIndex As Integer = lblPage.Text.LastIndexOf("/") + 1
            '    Dim charCount As Integer = lblPage.Text.LastIndexOf("?") - startIndex
            '    lblPage.Text = Server.HtmlEncode(lblPage.Text.Substring(startIndex, charCount))
            'End If
            If (lblPage.Text.IndexOf("?") = -1) Then
                lblPage.ToolTip = Server.HtmlEncode(lblPage.Text)
                lblPage.Text = Server.HtmlEncode(lblPage.Text.Substring(lblPage.Text.LastIndexOf("/") + 1))
                'note that the above line will likely fail if an error url has no ? but has a slash after the page url.
                'Unlikely (as it would be a bad url), but possible. Should fix at some point.
            Else
                lblPage.ToolTip = Server.HtmlEncode(lblPage.Text)
                Dim pagePart As String = lblPage.Text.Substring(0, lblPage.Text.IndexOf("?")) 'get the text before the ?, which is where page stops
                Dim startIndex As Integer = pagePart.LastIndexOf("/") + 1 'now find the last slash before that page name
                lblPage.Text = Server.HtmlEncode(pagePart.Substring(startIndex)) 'page name is from that slash till the end (which is the ?)
            End If

        End Sub

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
            If (Not Page.IsPostBack) Then
                ' FillGrid()
            End If
        End Sub

        Protected Sub PopulateDetails(ByVal sender As Object, ByVal e As System.EventArgs) Handles gvAllErrors.SelectedIndexChanged
            FillDetails(gvAllErrors.SelectedDataKey.Value)
            mdlPopup.Show()
        End Sub

        Private Sub FillDetails(ByVal id As Integer)

            Dim ds As DataSet = LogManager.GetErrorById(id)

            Dim result As DataRow = ds.Tables(0).Rows(0)

            lblAppVersion.Text = Server.HtmlEncode(result("appVersion").ToString())
            lblBrowser.Text = Server.HtmlEncode(result("browser").ToString())
            lblCaller.Text = Server.HtmlEncode(IIf(result("caller").ToString.Trim.Length = 0, "N/A", result("caller").ToString()))
            lblMessage.Text = Server.HtmlEncode(IIf(result("message").ToString.Trim.Length = 0, "N/A", result("message").ToString()))
            lblStack.Text = Server.HtmlEncode(IIf(result("stackTrace").ToString.Trim.Length = 0, "N/A", result("stackTrace").ToString()))

            'lbllogID.Text = result("logId").ToString()
            gvAllErrors.SelectedIndex = -1

        End Sub

    End Class

End Namespace