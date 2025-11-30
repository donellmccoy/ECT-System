Imports ALOD.Core.Domain.DBSign
Imports ALODWebUtility.Common
Imports ALODWebUtility.Providers
Imports ALODWebUtility.Worklfow

Namespace Web.Sys

    Partial Class Secure_Shared_Admin_DBSignTest
        Inherits System.Web.UI.Page

        Protected Sub Page_Load(ByVal Sender As System.Object, ByVal e As System.EventArgs) Handles Me.Load
            If Not IsPostBack Then
                SSNHidden.Text = ""
                SSNHidden.Text = Server.UrlEncode(LodCrypto.Encrypt("XXXXXTEST"))
            End If
        End Sub

        Protected Sub RefreshButton_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles RefreshButton.Click
        End Sub

        Protected Sub SignatureCompleted(ByVal sender As Object, ByVal e As SignCompletedEventArgs) Handles SigBlock.SignCompleted
            If (e.SignaturePassed) Then

                '        'we have a good signature.  take any further actions that are required
                '        If (ChangeStatus(e.OptionId, e.StatusOut, e.Text, e.Comments)) Then
                SetFeedbackMessage("Test Signing Successful.  Action applied: " + e.Text)
                DBSignMessageText.Text = "Test Signing Successful.  Action applied: " + e.Text
                'Response.Redirect(Resources.Global.StartPage, True)
                '        End If

            End If
        End Sub

        Protected Sub SignButton_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles SignButton.Click
            Dim scId = 0 'Is used if it NOT SignOnly
            Dim secId = 0 'Also used if NOt SignOnly
            Dim LogActionDisplayText = "Test Signing"
            DBSignMessageText.Text = ""

            SigBlock.StartSignature(scId, 0, secId, LogActionDisplayText, Nothing, Nothing,
                                                     Nothing, DBSignTemplateId.SignOnly, Nothing)
        End Sub

        Protected Sub SignStarted(ByVal sender As Object, ByVal e As SignStartedEventArgs) Handles SigBlock.SignStarted

            '    ' Status is changed !!
            '    If (UserCanEdit) Then
            '        'because this changes data that is signed, it must be done before the signing takes place
            '        'InsertPersonnelInfo() 'This updates the person who is actuall forwarding the lod
            '        CommitChanges()
            '    End If

        End Sub

    End Class

End Namespace