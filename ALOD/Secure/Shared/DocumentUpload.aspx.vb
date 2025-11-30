Imports ALOD.Core.Domain.Documents
Imports ALOD.Data
Imports ALOD.Logging
Imports ALODWebUtility.Providers ' Ensure this is included for VB.NET compatibility

Namespace Web.Docs

    Partial Class Secure_Shared_DocumentUpload
        Inherits System.Web.UI.Page

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
            ' Capture and log query string parameters
            Dim groupIdString As String = Request.QueryString("groupId")
            If String.IsNullOrWhiteSpace(groupIdString) Then
                groupIdString = Request.QueryString("group") ' Fallback to "group" if "groupId" is not found
            End If

            Dim docTypeString As String = Request.QueryString("docType")
            If String.IsNullOrWhiteSpace(docTypeString) Then
                docTypeString = Request.QueryString("id") ' Fallback to "id" if "docType" is not found
            End If

            ' Decrypting the docEntity parameter
            Dim docEntity As String = LodCrypto.Decrypt(Request.QueryString("entity"))

            ' Logging the values for debugging
            Debug.WriteLine("groupId: " & groupIdString)
            Debug.WriteLine("docType: " & docTypeString)
            Debug.WriteLine("docEntity: " & docEntity)

            ' Validate groupId
            Dim groupId As Long
            If String.IsNullOrWhiteSpace(groupIdString) OrElse Not Long.TryParse(groupIdString, groupId) OrElse groupId = 0 Then
                LogManager.LogError("Invalid or missing groupId in Secure_Shared_DocumentUpload.Page_Load")
                ErrorLabel.Text = "Invalid or missing group ID. Please check your input and try again."
                Return
            End If

            ' Validate and Parse docType
            Dim docType As DocumentType
            If String.IsNullOrWhiteSpace(docTypeString) OrElse Not [Enum].TryParse(docTypeString, True, docType) OrElse Not [Enum].IsDefined(GetType(DocumentType), docType) Then
                LogManager.LogError($"Invalid docType value '{docTypeString}' in Secure_Shared_DocumentUpload.Page_Load")
                ErrorLabel.Text = "Invalid document type value. Please check your input and try again."
                Return
            End If

            ' Specific condition check from C# code
            If docEntity = "XXXXXTEST" Then
                ErrorLabel.Text = "An error occurred initializing the document transfer system. Please check the Error Log for more details."
                Return
            End If

            Try
                ' Proceed with the document store operations
                Dim store As IDocumentDao = New SRXDocumentStore(Session("UserName").ToString())
                Dim url As String = store.GetDocumentUploadUrl(groupId, docType, docEntity)

                If Not String.IsNullOrEmpty(url) Then
                    Response.Redirect(url, False)
                    HttpContext.Current.ApplicationInstance.CompleteRequest()
                Else
                    ' Handle the case where the URL is empty
                    LogManager.LogError("Document upload URL is empty in Secure_Shared_DocumentUpload.Page_Load")
                    ErrorLabel.Text = "Failed to initialize document upload. URL is empty."
                End If
            Catch ex As Exception
                ' Log the exception details
                LogManager.LogError("Exception in Secure_Shared_DocumentUpload.Page_Load: " & ex.ToString())
                ErrorLabel.Text = "An exception occurred during document upload initialization: " & ex.Message
            End Try
        End Sub

    End Class

End Namespace