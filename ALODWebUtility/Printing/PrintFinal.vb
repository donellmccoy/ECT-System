Imports ALOD.Core.Domain.Documents
Imports ALOD.Core.Domain.Modules.Lod
Imports ALOD.Core.Domain.Workflow
Imports ALOD.Core.Interfaces.DAOInterfaces
Imports ALOD.Data

Namespace Printing
    ' Called from lod/Print.aspx.vb and ReinvestigationRequests/Print.aspx.vb
    ' Returns link to 348/261 forms for Print button

    Public Class PrintFinal

        Public Function GetURL261(ByVal lodid As String, ByVal curSession As String, ByVal dao As ILineOfDutyDao) As String
            Dim str261 As String = ""
            Dim form261ID As String = ""

            Dim instance As LineOfDuty = dao.GetById(lodid)
            Dim DocumentDao As IDocumentDao = New SRXDocumentStore(curSession)
            Dim _documents As IList(Of ALOD.Core.Domain.Documents.Document) = Nothing

            If (instance.DocumentGroupId = 0 Or instance.DocumentGroupId Is Nothing) Then
                instance.CreateDocumentGroup(DocumentDao)
            End If

            If (_documents Is Nothing) Then
                _documents = DocumentDao.GetDocumentsByGroupId(instance.DocumentGroupId)
            End If

            Dim isDoc As Boolean = False
            For Each docItem In _documents

                If (docItem.DocType.ToString() = "FinalForm261") Then
                    form261ID = docItem.Id.ToString()
                    isDoc = True
                End If
            Next

            If (isDoc) Then
                str261 = "~/Secure/Shared/DocumentViewer.aspx?docID=" & form261ID & "&modId=" & ModuleType.LOD
            Else
                str261 = ""
            End If

            Return str261

        End Function

        Public Function GetURL348(ByVal lodid As String, ByVal curSession As String, ByVal dao As ILineOfDutyDao) As String
            Dim str348 As String = ""
            Dim form348ID As String = ""

            Dim instance As LineOfDuty = dao.GetById(lodid)
            Dim DocumentDao As IDocumentDao = New SRXDocumentStore(curSession)
            Dim _documents As IList(Of ALOD.Core.Domain.Documents.Document)

            If (instance.DocumentGroupId Is Nothing Or instance.DocumentGroupId = 0) Then
                instance.CreateDocumentGroup(DocumentDao)
            End If

            _documents = DocumentDao.GetDocumentsByGroupId(instance.DocumentGroupId)

            If (instance.WorkflowStatus.StatusCodeType.IsFinal) Then

                ' fileSubString is used to get the correct 348 document if multiple 348s are associated with the LOD's group Id.
                ' This happens when a case is overridden and recompleted or if a case is reinvestigated.
                Dim fileSubString As String = instance.CaseId & "-Generated"

                Dim isDoc As Boolean = False

                For Each docItem In _documents
                    If (docItem.DocType.ToString() = "FinalForm348" AndAlso docItem.OriginalFileName.Contains(fileSubString)) Then
                        form348ID = docItem.Id.ToString()
                        isDoc = True
                    End If
                Next

                If (isDoc) Then
                    str348 = "~/Secure/Shared/DocumentViewer.aspx?docID=" & form348ID & "&modId=" & ModuleType.LOD
                Else
                    str348 = ""

                End If
            Else
                str348 = ""

            End If

            Return str348

        End Function

    End Class

End Namespace