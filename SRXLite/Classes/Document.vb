Option Strict On

Imports System.Drawing
Imports System.Drawing.Imaging
Imports System.Data.SqlClient
Imports System.IO
Imports SRXLite.DataAccess
Imports SRXLite.DataTypes
Imports SRXLite.Modules

Namespace Classes

    ''' <summary>
    '''
    ''' </summary>
    ''' <remarks></remarks>
    Public Class Document
        Implements IDisposable

        Private _appendable As Boolean
        Private _browserViewable As Boolean
        Private _contentType As String
        Private _context As HttpContext
        Private _currentFrame As Integer = 0
        Private _db As AsyncDB
        Private _docGuid As DocumentGuid
        Private _docID As Long
        Private _errorMessage As String
        Private _fileBytes As Byte()
        Private _fileExt As String

        'Multiframe images
        Private _frameFileBytes As List(Of Byte())

        Private _group As Group
        Private _groupID As Long
        Private _guid As Guid
        Private _hasErrors As Boolean = False
        Private _multiframe As Boolean = False
        Private _multiframeComplete As Boolean = False
        Private _subuserID As Integer
        Private _uploadKeys As UploadKeys
        Private _userCallback As AsyncCallback
        Private _userID As Short
        Private _userStateObject As Object

#Region " Properties "

        Public ReadOnly Property ContentType() As String
            Get
                Return _contentType
            End Get
        End Property

        Public ReadOnly Property DocID() As Long
            Get
                Return _docID
            End Get
        End Property

        Public ReadOnly Property ErrorMessage() As String
            Get
                Return _errorMessage
            End Get
        End Property

        Public ReadOnly Property FileExt() As String
            Get
                Return _fileExt
            End Get
        End Property

        Public ReadOnly Property HasErrors() As Boolean
            Get
                Return _hasErrors
            End Get
        End Property

        Public ReadOnly Property IsAppendable() As Boolean
            Get
                Return _appendable
            End Get
        End Property

        Public ReadOnly Property IsBrowserViewable() As Boolean
            Get
                Return _browserViewable
            End Get
        End Property

        Public ReadOnly Property IsImage() As Boolean
            Get
                Return _contentType.StartsWith("image/")
            End Get
        End Property

        Public ReadOnly Property IsMultiframe() As Boolean
            Get
                Return _multiframe
            End Get
        End Property

        Public Property UploadKeys() As UploadKeys
            Get
                Return _uploadKeys
            End Get
            Set(ByVal value As UploadKeys)
                _uploadKeys = value
            End Set
        End Property

#End Region

#Region " Constructors "

        ''' <summary>
        '''
        ''' </summary>
        ''' <param name="user">User object</param>
        ''' <remarks></remarks>
        Public Sub New(ByVal user As User)
            _db = New AsyncDB(AddressOf HandleError)
            _userID = user.UserID
            _subuserID = user.SubuserID
            _context = HttpContext.Current
        End Sub

        ''' <summary>
        '''
        ''' </summary>
        ''' <param name="user">User object</param>
        ''' <param name="docID">Document ID</param>
        ''' <remarks></remarks>
        Public Sub New(ByVal user As User, ByVal docID As Long)
            _db = New AsyncDB(AddressOf HandleError)
            _userID = user.UserID
            _subuserID = user.SubuserID
            _docID = docID
            _context = HttpContext.Current
        End Sub

        ''' <summary>
        '''
        ''' </summary>
        ''' <param name="user">User requesting the document</param>
        ''' <param name="docID">ID of the document</param>
        ''' <param name="groupID">Group ID of the document</param>
        ''' <remarks></remarks>
        Public Sub New(ByVal user As User, ByVal docID As Long, ByVal groupID As Long)
            _db = New AsyncDB(AddressOf HandleError)
            _userID = user.UserID
            _subuserID = user.SubuserID
            _docID = docID
            _groupID = groupID
            _context = HttpContext.Current
        End Sub

        ''' <summary>
        '''
        ''' </summary>
        ''' <param name="data">Document data associated with a GUID</param>
        ''' <remarks></remarks>
        Public Sub New(ByVal data As DocumentGuid.GuidData, Optional ByVal context As HttpContext = Nothing)
            _db = New AsyncDB(AddressOf HandleError)
            _userID = data.UserID
            _subuserID = data.SubuserID
            _docID = data.DocID
            _groupID = data.GroupID
            _guid = data.DocGuid
            _context = CType(IIf(IsNothing(context), HttpContext.Current, context), HttpContext)
        End Sub

#End Region

#Region " AddPage "

        ''' <summary>
        ''' Starts an asynchronous operation for adding a page to the document.
        ''' If the page is a multiframe image, each frame will be added to the document as a new page.
        ''' </summary>
        ''' <param name="fileBytes"></param>
        ''' <param name="callback"></param>
        ''' <param name="stateObject"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function BeginAddPage(ByVal fileBytes As Byte(), ByVal callback As AsyncCallback, ByVal stateObject As Object) As IAsyncResult
            If fileBytes Is Nothing OrElse _docID <= 0 Then
                _hasErrors = True
                callback.BeginInvoke(Nothing, Nothing, Nothing)
                LogError("BeginAddPage: Invalid data")
                Return Nothing
            End If

            _fileBytes = fileBytes
            _userCallback = callback

            If _multiframe Then
                Try
                    Using ms As New MemoryStream(_fileBytes.Length)
                        ms.Write(_fileBytes, 0, _fileBytes.Length)

                        Using img As Image = Image.FromStream(ms)
                            Dim fGuid As Guid = img.FrameDimensionsList(0)
                            Dim fDimension As New FrameDimension(fGuid)
                            Dim frameCount As Integer = img.GetFrameCount(fDimension)

                            If frameCount > 1 Then
                                _frameFileBytes = New List(Of Byte())

                                For i As Integer = 0 To (frameCount - 1)
                                    Using msPage As New MemoryStream
                                        img.SelectActiveFrame(fDimension, i)
                                        img.Save(msPage, ImageFormat.Tiff)
                                        _frameFileBytes.Add(msPage.ToArray())
                                    End Using 'msPage
                                Next

                                _currentFrame = 0
                                _multiframeComplete = False

                                Return Me.BeginAddPageMultiframe(New AsyncCallback(AddressOf EndAddPageMultiframe), stateObject)

                            End If
                        End Using 'img
                    End Using 'ms
                Catch ex As Exception
                    _hasErrors = True
                    callback.BeginInvoke(Nothing, Nothing, Nothing)
                    LogError("BeginAddPage: " & ex.Message)
                    Return Nothing
                End Try
            End If

            Return Me.BeginAddPage(callback, stateObject)
        End Function

        ''' <summary>
        '''
        ''' </summary>
        ''' <param name="result"></param>
        ''' <remarks></remarks>
        Public Sub EndAddPage(ByVal result As IAsyncResult)
            If _multiframeComplete Then Exit Sub
            _db.EndExecuteNonQuery(result)
        End Sub

        ''' <summary>
        '''
        ''' </summary>
        ''' <param name="callback"></param>
        ''' <param name="stateObject"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Private Function BeginAddPage(ByVal callback As AsyncCallback, ByVal stateObject As Object) As IAsyncResult
            'Insert DocumentPage record
            Dim command As New SqlCommand
            command.CommandText = "dsp_DocumentPage_Insert"
            command.CommandType = CommandType.StoredProcedure
            With command.Parameters
                .Add(getSqlParameter("@UserID", _userID))
                .Add(getSqlParameter("@SubuserID", _subuserID))
                .Add(getSqlParameter("@DocID", _docID))
                .Add(getSqlParameter("@ClientIP", GetClientIP(_context)))
                .Add(getSqlParameter("@DocPageID", Nothing, SqlDbType.BigInt, ParameterDirection.Output))
                .Add(getSqlParameter("@FileData", _fileBytes, SqlDbType.VarBinary))
            End With

            Return _db.BeginExecuteNonQuery(command, callback, stateObject)
        End Function

        ''' <summary>
        '''
        ''' </summary>
        ''' <param name="callback"></param>
        ''' <param name="stateObject"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Private Function BeginAddPageMultiframe(ByVal callback As AsyncCallback, ByVal stateObject As Object) As IAsyncResult
            _fileBytes = _frameFileBytes(_currentFrame)
            Return Me.BeginAddPage(callback, stateObject)
        End Function

        ''' <summary>
        '''
        ''' </summary>
        ''' <param name="result"></param>
        ''' <remarks></remarks>
        Private Sub EndAddPageMultiframe(ByVal result As IAsyncResult)
            Me.EndAddPage(result)
            _currentFrame += 1

            If _currentFrame < (_frameFileBytes.Count) Then
                Me.BeginAddPageMultiframe(New AsyncCallback(AddressOf EndAddPageMultiframe), Nothing)
            Else
                _multiframeComplete = True
                _userCallback.Invoke(result)
            End If
        End Sub

#End Region

#Region " Create "

        ''' <summary>
        ''' Starts an asynchronous operation for creating a new document without any pages.
        ''' If a group ID was specified in the constructor, the document will be added to that group.
        ''' </summary>
        ''' <param name="docUploadKeys"></param>
        ''' <param name="callback"></param>
        ''' <param name="stateObject"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function BeginCreate(ByVal docUploadKeys As UploadKeys, ByVal callback As AsyncCallback, ByVal stateObject As Object) As IAsyncResult
            'Save to DB, get docID
            Dim command As New SqlCommand
            command.CommandText = "dsp_Document_Insert"
            command.CommandType = CommandType.StoredProcedure
            With command.Parameters
                .Add(getSqlParameter("@DocDate", docUploadKeys.DocDate, SqlDbType.SmallDateTime))
                .Add(getSqlParameter("@DocDescription", docUploadKeys.DocDescription))
                .Add(getSqlParameter("@DocStatusID", docUploadKeys.DocStatus))
                .Add(getSqlParameter("@DocTypeID", docUploadKeys.DocTypeID))
                .Add(getSqlParameter("@EntityName", docUploadKeys.EntityName))
                .Add(getSqlParameter("@FileExt", FormatFileExt(docUploadKeys.FileName)))
                .Add(getSqlParameter("@OriginalFileName", FormatFileName(docUploadKeys.FileName)))
                .Add(getSqlParameter("@InputTypeID", docUploadKeys.InputType))
                .Add(getSqlParameter("@OwnerUserID", _userID))
                .Add(getSqlParameter("@OwnerSubuserID", _subuserID))
                .Add(getSqlParameter("@DocGuid", _guid))
                .Add(getSqlParameter("@ClientIP", GetClientIP(_context)))
                .Add(getSqlParameter("@DocID", Nothing, SqlDbType.BigInt, ParameterDirection.Output))
                .Add(getSqlParameter("@Multiframe", Nothing, SqlDbType.Bit, ParameterDirection.Output))
            End With

            'Insert a new document record, output the docID
            Return _db.BeginExecuteNonQuery(command, callback, stateObject)
        End Function

        ''' <summary>
        ''' Finishes the asynchronous operation for creating a new document without any pages.
        ''' </summary>
        ''' <param name="result"></param>
        ''' <remarks></remarks>
        Public Sub EndCreate(ByVal result As IAsyncResult)
            Dim outputParams As New Dictionary(Of String, Object)
            outputParams.Add("@DocID", Nothing)
            outputParams.Add("@Multiframe", Nothing)

            _db.EndExecuteNonQuery(result, outputParams)

            _docID = LngCheck(outputParams("@DocID"))
            _multiframe = BoolCheck(outputParams("@Multiframe"))

            'Add the document to the group
            If _groupID > 0 Then
                _group = New Group(_userID, _subuserID, _groupID)
                _group.BeginAddDocument(_docID, New AsyncCallback(AddressOf EndGroupAddDocument), Nothing)
            End If
        End Sub

        ''' <summary>
        '''
        ''' </summary>
        ''' <param name="result"></param>
        ''' <remarks></remarks>
        Private Sub EndGroupAddDocument(ByVal result As IAsyncResult)
            _group.EndAddDocument(result)
        End Sub

#End Region

#Region " Delete "

        ''' <summary>
        ''' Starts an asynchronous operation for permanently deleting a document.
        ''' </summary>
        ''' <param name="callback"></param>
        ''' <param name="stateObject"></param>
        ''' <remarks></remarks>
        Public Function BeginDelete(ByVal callback As AsyncCallback, ByVal stateObject As Object) As IAsyncResult
            Dim command As New SqlCommand
            command.CommandText = "dsp_Document_Delete"
            command.CommandType = CommandType.StoredProcedure
            With command.Parameters
                .Add(getSqlParameter("@DocID", _docID))
                .Add(getSqlParameter("@UserID", _userID))
                .Add(getSqlParameter("@SubuserID", _subuserID))
                .Add(getSqlParameter("@ClientIP", GetClientIP(_context)))
            End With

            Return _db.BeginExecuteNonQuery(command, callback, stateObject)
        End Function

        ''' <summary>
        ''' Ends the asynchronous operation for permanently deleting a document.
        ''' </summary>
        ''' <param name="result"></param>
        ''' <remarks></remarks>
        Public Sub EndDelete(ByVal result As IAsyncResult)
            _db.EndExecuteNonQuery(result)
        End Sub

#End Region

#Region " GeneratePDF "

        ''' <summary>
        '''
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GeneratePDF() As Byte()

            Dim fileBytes() As Byte = Nothing
            Dim fileExt As String = ""
            Dim contentType As String = ""
            Dim docTypeName As String = ""
            Dim docDate As Date
            Dim pageNum As Integer = 0
            Dim lastDocTypeName As String = ""
            Dim lastPagePath As String = ""
            Dim pagePath As String
            Dim pageID As Long
            Dim user As New User(_userID, _subuserID)

            Dim command As New SqlCommand
            command.CommandText = "dsp_Document_GetPageIDs"
            command.CommandType = CommandType.StoredProcedure
            command.Parameters.AddWithValue("@DocID", _docID)

            Using pdfDoc As New PDF()
                Using reader As SqlDataReader = DB.ExecuteReader(command)
                    If Not reader.HasRows Then
                        'Check if documents were found
                        Return pdfDoc.GetBytes()
                    End If

                    While reader.Read()
                        fileExt = NullCheck(reader("FileExt")).ToLower
                        contentType = NullCheck(reader("ContentType"))
                        docTypeName = NullCheck(reader("DocTypeName"))
                        docDate = DateCheck(reader("DocDate"), Nothing)
                    End While

                    reader.NextResult()

                    While reader.Read()
                        Using ms As New MemoryStream

                            pageNum += 1
                            pageID = LngCheck(reader("DocPageID"))

                            Using docPage As New DocumentPage(user, pageID)
                                fileBytes = docPage.GetBytes()
                            End Using

                            pagePath = docTypeName & "\" & docDate

                            'Add document and bookmark to PDF file
                            pdfDoc.AddPage(fileBytes, PDF.PageType.Image, docTypeName, docDate, pageNum)

                            If docTypeName <> lastDocTypeName Then
                                'Link the top-level bookmark to the first page of each doc type
                                pdfDoc.AddBookmark(docTypeName, False)
                            End If

                            If pageNum = 1 AndAlso fileExt <> "pdf" AndAlso pagePath <> lastPagePath Then
                                'Link the DocDate bookmark to the first page of the document
                                pdfDoc.AddBookmark(pagePath, False)
                            End If

                            lastDocTypeName = docTypeName
                            lastPagePath = pagePath

                        End Using 'ms
                    End While
                End Using 'reader

                'Log usage
                Dim usageLog As New UsageLog(_context)
                Dim usageData As New UsageLog.UsageData
                With usageData
                    .UserID = _userID
                    .SubuserID = _subuserID
                    .ActionType = ActionType.GenerateDocumentPDF
                    .DocID = _docID
                End With
                usageLog.Insert(usageData)

                Return pdfDoc.GetBytes()

            End Using 'pdfDoc
        End Function

#End Region

#Region " GetPageList "

        ''' <summary>
        ''' Starts an asynchronous operation for retrieving data for all pages in the document.
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function BeginGetPageList(ByVal callback As AsyncCallback, ByVal stateObject As Object) As IAsyncResult
            Dim command As New SqlCommand
            command.CommandText = "dsp_Document_GetPageList"
            command.CommandType = CommandType.StoredProcedure
            With command.Parameters
                .Add(getSqlParameter("@UserID", _userID))
                .Add(getSqlParameter("@SubuserID", _subuserID))
                .Add(getSqlParameter("@DocID", _docID))
            End With

            Return _db.BeginExecuteReader(command, callback, stateObject)
        End Function

        ''' <summary>
        ''' Finishes the asynchronous operation and returns a list of data about each page.
        ''' </summary>
        ''' <param name="result"></param>
        ''' <remarks></remarks>
        Public Function EndGetPageList(ByVal result As IAsyncResult) As List(Of DocumentPageData)
            Dim list As New List(Of DocumentPageData)
            Dim docPageData As DocumentPageData
            Dim appRootUrl As String = GetURL(_context)
            Dim crypto As New CryptoManager(_context)

            Using reader As SqlDataReader = _db.EndExecuteReader(result)
                While reader.Read() 'First resultset
                    docPageData = New DocumentPageData()
                    With docPageData
                        .PageUrl = appRootUrl & "/Handlers/GetPage.ashx?id=" & crypto.EncryptForUrl(NullCheck(reader("DocPageGuid")))
                        .DocPageID = LngCheck(reader("DocPageID"))
                        .PageNumber = ShortCheck(reader("PageNumber"))
                    End With

                    list.Add(docPageData)
                End While

                reader.NextResult()
                While reader.Read() 'Second resultset
                    _contentType = NullCheck(reader("ContentType"))
                    _fileExt = NullCheck(reader("FileExt"))
                    _appendable = BoolCheck(reader("Appendable"))
                    _multiframe = BoolCheck(reader("Multiframe"))
                    _browserViewable = BoolCheck(reader("BrowserViewable"))
                End While
            End Using 'Closes connection

            Return list
        End Function

#End Region

#Region " GetUploadUrl "

        Private _entityDisplayText As String
        Private _stylesheetUrl As String

        ''' <summary>
        ''' Starts an asynchronous operation for generating a URL to upload a document.
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function BeginGetUploadUrl(
         ByVal entityName As String,
         ByVal docTypeID As Integer,
         ByVal groupID As Long,
         ByVal stylesheetUrl As String,
         ByVal entityDisplayText As String,
         ByVal callback As AsyncCallback,
         ByVal stateObject As Object) As IAsyncResult

            _stylesheetUrl = stylesheetUrl
            _entityDisplayText = entityDisplayText
            _docGuid = New DocumentGuid()
            Dim data As New DocumentGuid.GuidData
            data.UserID = _userID
            data.SubuserID = _subuserID
            data.EntityName = entityName
            data.DocTypeID = docTypeID
            data.GroupID = groupID

            Return _docGuid.BeginCreate(data, callback, stateObject)
        End Function

        ''' <summary>
        ''' Finishes the asynchronous operation and returns a URL to upload a document.
        ''' </summary>
        ''' <param name="result"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function EndGetUploadUrl(ByVal result As IAsyncResult) As String
            _guid = _docGuid.EndCreate(result)
            Dim crypto As New CryptoManager(_context)
            Return GetURL(_context, "/Tools/DocumentUpload.aspx", "id=" & crypto.EncryptForUrl(_guid.ToString) & "&styleurl=" & _context.Server.UrlEncode(_stylesheetUrl) & "&e=" & _context.Server.UrlEncode(_entityDisplayText))
        End Function

#End Region

#Region " GetViewerUrl "

        ''' <summary>
        ''' Starts an asynchronous operation for generating a URL to view the document.
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function BeginGetViewerUrl(
         ByVal docID As Long,
         ByVal isReadOnly As Boolean,
         ByVal callback As AsyncCallback,
         ByVal stateObject As Object) As IAsyncResult

            _docGuid = New DocumentGuid()
            Dim data As New DocumentGuid.GuidData
            data.DocID = docID
            data.UserID = _userID
            data.SubuserID = _subuserID
            data.IsReadOnly = isReadOnly
            data.EntityName = ""

            Return _docGuid.BeginCreate(data, callback, stateObject)
        End Function

        ''' <summary>
        ''' Finishes the asynchronous operation and returns a URL to view the document.
        ''' </summary>
        ''' <param name="result"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function EndGetViewerUrl(ByVal result As IAsyncResult) As String
            _guid = _docGuid.EndCreate(result)
            Dim crypto As New CryptoManager(_context)
            Return GetURL(_context, "/Tools/DocumentViewer.aspx", "id=" & crypto.EncryptForUrl(_guid.ToString))
        End Function

#End Region

#Region " UpdateStatus "

        ''' <summary>
        ''' Starts an asynchronous operation for updating the status of the document.
        ''' </summary>
        ''' <param name="docStatus"></param>
        ''' <param name="callback"></param>
        ''' <param name="stateObject"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function BeginUpdateStatus(ByVal docStatus As DocumentStatus, ByVal callback As AsyncCallback, ByVal stateObject As Object) As IAsyncResult
            Dim command As New SqlCommand
            command.CommandText = "dsp_Document_UpdateStatus"
            command.CommandType = CommandType.StoredProcedure
            With command.Parameters
                .Add(getSqlParameter("@UserID", _userID))
                .Add(getSqlParameter("@SubuserID", _subuserID))
                .Add(getSqlParameter("@DocID", _docID))
                .Add(getSqlParameter("@DocStatusID", docStatus))
                .Add(getSqlParameter("@ClientIP", GetClientIP(_context)))
            End With

            Return _db.BeginExecuteNonQuery(command, callback, stateObject)
        End Function

        ''' <summary>
        ''' Finishes the asynchronous operation for updating the status of the document.
        ''' </summary>
        ''' <param name="result"></param>
        ''' <remarks></remarks>
        Public Sub EndUpdateStatus(ByVal result As IAsyncResult)
            _db.EndExecuteNonQuery(result)
        End Sub

#End Region

#Region " UpdateKeys "

        ''' <summary>
        ''' Starts an asynchronous operation for updating the keys of the document.
        ''' </summary>
        ''' <param name="docKeys"></param>
        ''' <param name="callback"></param>
        ''' <param name="stateObject"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function BeginUpdateKeys(ByVal docKeys As DocumentKeys, ByVal callback As AsyncCallback, ByVal stateObject As Object) As IAsyncResult
            Dim command As New SqlCommand
            command.CommandText = "dsp_Document_UpdateKeys"
            command.CommandType = CommandType.StoredProcedure
            With command.Parameters
                .Add(getSqlParameter("@UserID", _userID))
                .Add(getSqlParameter("@SubuserID", _subuserID))
                .Add(getSqlParameter("@DocID", _docID))
                .Add(getSqlParameter("@DocDate", docKeys.DocDate))
                .Add(getSqlParameter("@DocDescription", docKeys.DocDescription))
                .Add(getSqlParameter("@DocStatusID", docKeys.DocStatus))
                .Add(getSqlParameter("@DocTypeID", docKeys.DocTypeID))
                .Add(getSqlParameter("@EntityName", docKeys.EntityName))
                .Add(getSqlParameter("@ClientIP", GetClientIP(_context)))
            End With

            Return _db.BeginExecuteNonQuery(command, callback, stateObject)
        End Function

        ''' <summary>
        ''' Finishes the asynchronous operation for updating the keys of the document.
        ''' </summary>
        ''' <param name="result"></param>
        ''' <remarks></remarks>
        Public Sub EndUpdateKeys(ByVal result As IAsyncResult)
            _db.EndExecuteNonQuery(result)
        End Sub

#End Region

#Region " Upload "

        ''' <summary>
        ''' Starts an asynchronous operation for uploading a new document.
        ''' </summary>
        ''' <param name="fileBytes"></param>
        ''' <param name="docUploadKeys"></param>
        ''' <param name="callback"></param>
        ''' <param name="stateObject"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function BeginUpload(ByVal fileBytes As Byte(), ByVal docUploadKeys As UploadKeys, ByVal callback As AsyncCallback, ByVal stateObject As Object) As IAsyncResult
            _fileBytes = fileBytes
            _userCallback = callback
            _userStateObject = stateObject
            Return Me.BeginCreate(docUploadKeys, New AsyncCallback(AddressOf BeginUploadCallback), stateObject)
        End Function

        ''' <summary>
        ''' Finishes the asynchronous operation for uploading a new document.
        ''' </summary>
        ''' <param name="result"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function EndUpload(ByVal result As IAsyncResult) As Long
            Me.EndAddPage(result)
            Return _docID
        End Function

        ''' <summary>
        '''
        ''' </summary>
        ''' <param name="result"></param>
        ''' <remarks></remarks>
        Private Sub BeginUploadCallback(ByVal result As IAsyncResult)
            Me.EndCreate(result)
            Me.BeginAddPage(_fileBytes, _userCallback, _userStateObject)
        End Sub

#End Region

#Region " HandleError "

        Public Sub HandleError(ByVal result As IAsyncResult, ByVal errorMessage As String)
            _hasErrors = True
            _errorMessage = errorMessage
            If _userCallback IsNot Nothing Then
                _userCallback.Invoke(result)
                System.Threading.Thread.CurrentThread.Abort()
            End If
        End Sub

#End Region

#Region " IDisposable Support "

        Private disposedValue As Boolean = False 'To detect redundant calls

        ' This code added by Visual Basic to correctly implement the disposable pattern.
        Public Sub Dispose() Implements IDisposable.Dispose
            ' Do not change this code.  Put cleanup code in Dispose(ByVal disposing As Boolean) above.
            Dispose(True)
            GC.SuppressFinalize(Me)
        End Sub

        Protected Overridable Sub Dispose(ByVal disposing As Boolean)
            If Not Me.disposedValue Then
                If disposing Then
                    ' TODO: free managed resources when explicitly called
                    Erase _fileBytes
                    If _frameFileBytes IsNot Nothing Then _frameFileBytes.Clear()
                End If

                ' TODO: free shared unmanaged resources
                If _db IsNot Nothing Then _db.Dispose()
                If _group IsNot Nothing Then _group.Dispose()
                If _docGuid IsNot Nothing Then _docGuid.Dispose()

            End If
            Me.disposedValue = True
        End Sub

#End Region

    End Class

End Namespace