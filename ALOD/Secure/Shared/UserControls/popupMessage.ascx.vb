Namespace Web.UserControls

    Partial Class Secure_Shared_UserControls_popupMessage
        Inherits System.Web.UI.UserControl

        Delegate Sub CancelledEvent()

        Delegate Sub SavedEvent(ByVal UserID As Integer, ByVal FullName As String)

        Public Event Cancel_Click As CancelledEvent

        Public Event Saved_Click As SavedEvent

    End Class

End Namespace