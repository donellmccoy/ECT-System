Imports ALOD.Core.Domain.Documents
Imports ALOD.Core.Domain.Lookup
Imports ALOD.Core.Domain.Workflow
Imports ALOD.Core.Interfaces
Imports ALOD.Core.Interfaces.DAOInterfaces
Imports ALOD.Data
Imports ALOD.Logging
Imports ALODWebUtility.Printing

Namespace Documents

    Public Class CertificationStampStrategy
        Implements IDocumentProcessingStrategy

        Private _certificateId As Integer
        Private _processingErrors As List(Of String)
        Private _specCaseDao As ISpecialCaseDAO
        Private _stampDao As ICertificationStampDao

        Public Sub New()
            _processingErrors = New List(Of String)()
        End Sub

        Protected ReadOnly Property SpecCaseDao() As ISpecialCaseDAO
            Get
                If (_specCaseDao Is Nothing) Then
                    _specCaseDao = New NHibernateDaoFactory().GetSpecialCaseDAO()
                End If

                Return _specCaseDao
            End Get
        End Property

        Protected ReadOnly Property StampDao() As ICertificationStampDao
            Get
                If (_stampDao Is Nothing) Then
                    _stampDao = New NHibernateDaoFactory().GetCertificationStampDao()
                End If

                Return _stampDao
            End Get
        End Property

        Public Function GetProcessingErrors() As IList(Of String) Implements IDocumentProcessingStrategy.GetProcessingErrors
            If (_processingErrors Is Nothing) Then
                _processingErrors = New List(Of String)()
            End If

            Return _processingErrors
        End Function

        Public Function ProcessDocument(ByVal refId As Integer, ByVal groupId As Long, ByVal docDao As IDocumentDao, ByVal metaData As Document, ByVal fileData As Byte()) As Long Implements IDocumentProcessingStrategy.ProcessDocument
            Dim processingError As String = String.Empty

            If (Not metaData.Extension.ToLower().Equals("pdf")) Then
                processingError = "Incorrect file extenstion (" & metaData.Extension & "). Document must be a PDF."
                _processingErrors.Add(processingError)
                LogManager.LogError("CertificationStampStrategy.ProcessDocument(): Incorrect file extenstion (" & metaData.Extension & "). Document must be a PDF. RefId = " & refId & ".")
                Return 0
            End If

            ' Get stamp data...
            Dim stamp As CertificationStamp = StampDao.GetSpecialCaseStamp(refId, False)

            If (stamp Is Nothing) Then
                processingError = GetGenericErrorMessage(refId)
                _processingErrors.Add(processingError)
                LogManager.LogError("CertificationStampStrategy.ProcessDocument(): Failed to load certification stamp. RefId = " & refId & ".")
                Return 0
            End If

            Dim stampBody As String = stamp.PopulatedBody(StampDao.GetStampData(refId, False), 50)

            If (String.IsNullOrEmpty(stampBody)) Then
                processingError = GetGenericErrorMessage(refId)
                _processingErrors.Add(processingError)
                LogManager.LogError("CertificationStampStrategy.ProcessDocument(): Failed to load certification stamp body. RefId = " & refId & ". StampId = " & stamp.Id & ".")
                Return 0
            End If

            ' Get optional secondary stamp if it exists...
            Dim secondaryStamp As CertificationStamp = StampDao.GetSpecialCaseStamp(refId, True)
            Dim secondaryStampBody As String = String.Empty

            If (secondaryStamp IsNot Nothing) Then
                secondaryStampBody = secondaryStamp.PopulatedBody(StampDao.GetStampData(refId, True), 50)
            End If

            ' Read document data into PDFDocument object
            Dim doc As PDFDocument = New PDFDocument()

            If (doc Is Nothing) Then
                processingError = GetGenericErrorMessage(refId)
                _processingErrors.Add(processingError)
                LogManager.LogError("CertificationStampStrategy.ProcessDocument(): Failed to load a PDFDocument object. RefId = " & refId & ". StampId = " & stamp.Id & ".")
                Return 0
            End If

            If (Not doc.Read(fileData)) Then
                processingError = GetGenericErrorMessage(refId)
                _processingErrors.Add(processingError)
                LogManager.LogError("CertificationStampStrategy.ProcessDocument(): Failed to read document data. RefId = " & refId & ". StampId = " & stamp.Id & ".")
                Return 0
            End If

            ' Add stamps to the PDF document...
            If (Not String.IsNullOrEmpty(secondaryStampBody)) Then
                AddStampToDoc(doc, stampBody, 0.05, 0.5)
                AddStampToDoc(doc, secondaryStampBody, 0.05, 0.2)
            Else
                ' Add stamp to the PDF document...
                AddStampToDoc(doc, stampBody, 0.05, 0.333)
            End If

            ' Render to file...
            doc.IncludeFOUOWatermark = False
            doc.Render(metaData.OriginalFileName)

            ' Add document to database...
            Dim docId As Long = docDao.AddDocument(doc.GetBuffer(), metaData.OriginalFileName, groupId, metaData)

            If (docId <= 0) Then
                processingError = GetGenericErrorMessage(refId)
                _processingErrors.Add(processingError)
                LogManager.LogError("CertificationStampStrategy.ProcessDocument(): SRXDocumentStore.AddDocument() failed to add document to database. RefId = " & refId & ".")
                Return 0
            End If

            LogManager.LogAction(ModuleType.System, UserAction.AddedDocument, "Added Certification Stamped document: " & metaData.OriginalFileName & ". RefId = " & refId & ". DocGroupId = " & groupId & ". DocId = " & docId & ".")

            Return docId
        End Function

        Protected Sub AddStampToDoc(ByVal doc As PDFDocument, ByVal stampBody As String)
            Dim stampParts As List(Of String) = stampBody.Split(vbNewLine).ToList()
            Dim stampPDFString As List(Of PDFString) = New List(Of PDFString)()

            For Each s As String In stampParts
                stampPDFString.Add(New PDFString() With {.Text = s, .Color = "#284780", .FontSize = 14, .Alignment = "left", .Rotation = 30})
            Next

            doc.AddStamp(stampPDFString, True, 0.05, 0.333, 8)
        End Sub

        Protected Sub AddStampToDoc(ByVal doc As PDFDocument, ByVal stampBody As String, ByVal widthRatio As Double, ByVal heightRatio As Double)
            Dim stampParts As List(Of String) = stampBody.Split(vbNewLine).ToList()
            Dim stampPDFString As List(Of PDFString) = New List(Of PDFString)()

            For Each s As String In stampParts
                stampPDFString.Add(New PDFString() With {.Text = s, .Color = "#284780", .FontSize = 14, .Alignment = "left", .Rotation = 30})
            Next

            doc.AddStamp(stampPDFString, True, widthRatio, heightRatio, 8)
        End Sub

        Private Function GetGenericErrorMessage(ByVal refId As Integer) As String
            Return "Failed to process document. Please contact a system administrator and provide them with the Reference Id Number " & refId & "."
        End Function

    End Class

End Namespace