Imports ALODWebUtility.Common

Namespace Web.Sys

    Partial Class Secure_Shared_Admin_EmailTest
        Inherits System.Web.UI.Page

        Protected Sub SendButton_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles SendButton.Click

            Dim passed As Boolean = True

            If (Not ValidateRequiredField(AddressBox)) Then
                passed = False
            End If

            If (Not ValidateRequiredField(SubjectBox)) Then
                passed = False
            End If

            If (Not ValidateRequiredField(MessageBox)) Then
                passed = False
            End If

            If (Not passed) Then
                Exit Sub
            End If

            Dim address As String = AddressBox.Text.Trim
            Dim subject As String = SubjectBox.Text.Trim
            Dim message As String = MessageBox.Text.Trim

            Dim mail As New System.Net.Mail.MailMessage()

            mail.To.Add(address)
            mail.Subject = subject
            mail.Body = message

            mail.From = New System.Net.Mail.MailAddress(ConfigurationManager.AppSettings("EmailFrom"))

            Dim server As New System.Net.Mail.SmtpClient()

            Try
                server.Send(mail)
                Result.Text = "Mail Sent"
            Catch ex As Exception
                Result.Text = "Failed to send: " + ex.ToString()
            End Try

        End Sub

    End Class

End Namespace