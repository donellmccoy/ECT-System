Imports ALOD.Core.Domain.Documents
Imports ALOD.Core.Domain.Workflow
Imports ALOD.Data
Imports ALOD.Logging

Namespace Web.Docs

    Partial Class Secure_Shared_DocumentViewer
        Inherits System.Web.UI.Page

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

            Dim store As IDocumentDao = New SRXDocumentStore()
            Dim url As String = ""
            Dim docId As Long = 0
            Dim refId As Integer = 0
            Dim modId As Byte = ModuleType.System

            If (Request.QueryString("docId") IsNot Nothing) Then
                Long.TryParse(Request.QueryString("docId"), docId)
            End If

            If (Request.QueryString("refId") IsNot Nothing) Then
                Integer.TryParse(Request.QueryString("refId"), refId)
            End If

            If (Request.QueryString("modId") IsNot Nothing) Then
                Byte.TryParse(Request.QueryString("modId"), modId)
            End If

            If (docId <> 0) Then
                url = store.GetDocumentViewerUrl(docId)
            End If

            Dim des As String = Server.UrlDecode(Request.QueryString("doc"))

            If (url.Length > 0) Then

                If (refId <> 0) Then
                    LogManager.LogAction(modId, UserAction.ViewDocument, refId, "Document: " + des)
                Else
                    LogManager.LogAction(modId, UserAction.ViewDocument, "Document: " + des)
                End If

                Response.Redirect(url)
            End If

        End Sub

    End Class

End Namespace