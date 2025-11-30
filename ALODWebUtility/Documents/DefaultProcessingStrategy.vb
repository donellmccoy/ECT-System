Imports ALOD.Core.Domain.Documents
Imports ALOD.Core.Domain.Workflow
Imports ALOD.Core.Interfaces
Imports ALOD.Logging

Namespace Documents

    Public Class DefaultProcessingStrategy
        Implements IDocumentProcessingStrategy

        Private _processingErrors As List(Of String) = New List(Of String)()

        ' Implementing the GetProcessingErrors method from the IDocumentProcessingStrategy interface
        Public Function GetProcessingErrors() As IList(Of String) Implements IDocumentProcessingStrategy.GetProcessingErrors
            Return _processingErrors
        End Function

        ' Implementing the ProcessDocument method from the IDocumentProcessingStrategy interface
        Public Function ProcessDocument(ByVal refId As Integer, ByVal groupId As Long, ByVal docDao As IDocumentDao, ByVal metaData As Document, ByVal fileData As Byte()) As Long Implements IDocumentProcessingStrategy.ProcessDocument
            If refId <= 0 Then
                ' Handle invalid refId
                LogManager.LogError("ProcessDocument called with invalid refId: " & refId)
                _processingErrors.Add("Invalid reference ID provided.")
                Return 0
            End If

            Dim docId As Long = 0
            Try
                ' Attempt to add the document
                docId = docDao.AddDocument(fileData, metaData.OriginalFileName, groupId, metaData)

                ' Check if the document ID is valid
                If (docId <= 0) Then
                    ' Log error with specific details
                    LogManager.LogError("ProcessDocument: Failed with docId=" & docId & ", refId=" & refId & ", groupId=" & groupId)
                    _processingErrors.Add("Failed to upload document. Please contact a system administrator and provide them with the Reference Id Number " & refId & ".")
                    Return 0
                End If

                ' Log successful action
                LogManager.LogAction(ModuleType.System, UserAction.AddedDocument, "Added document: " & metaData.OriginalFileName & ". RefId = " & refId & ". DocGroupId = " & groupId & ". DocId = " & docId & ".")

                Return docId
            Catch ex As Exception
                ' Log exception details
                LogManager.LogError("ProcessDocument Exception: " & ex.ToString())
                _processingErrors.Add("An error occurred while processing the document: " & ex.Message)
                Return 0
            End Try
        End Function

    End Class

End Namespace