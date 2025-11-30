Imports ALOD.Core.Domain.Workflow
Imports ALOD.Logging
Imports ALODWebUtility.Common

Namespace Web.Sys

    Partial Class Secure_Shared_System_Permissions
        Inherits System.Web.UI.Page

        Protected Sub InsertPermission(ByVal sender As Object, ByVal e As System.EventArgs)
            FeedbackPanel1.Text = "Inserted Permission"
        End Sub

        Protected Sub PermRowUpdated(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewUpdatedEventArgs)
            Dim changes As New ChangeSet

            If (e.OldValues("Name") <> e.NewValues("Name")) Then
                changes.Add("Edit Permission", "Name", e.OldValues("Name"), e.NewValues("Name"))
            End If

            If (e.OldValues("Description") <> e.NewValues("Description")) Then
                changes.Add("Edit Permission", "Description", e.OldValues("Description"), e.NewValues("Description"))
            End If

            If (changes.Count > 0) Then
                Dim actionId As Integer = LogManager.LogActionPermission(ModuleType.System, UserAction.ModifyPermission, e.Keys("Id"))
                changes.Save(actionId)
            End If

        End Sub

    End Class

End Namespace