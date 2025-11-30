Imports ALODWebUtility.Common

Namespace Web

    Partial Class Secure_Shared_ChangeLog
        Inherits System.Web.UI.Page

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

            If (Not IsPostBack) Then
                InitControls()
            End If
        End Sub

        Private Sub InitControls()

            Dim changes As New ChangeSet()
            changes.GetByLogId(CInt(Request.QueryString("logId")))
            gvChanges.DataSource = changes
            gvChanges.DataBind()

        End Sub

    End Class

End Namespace