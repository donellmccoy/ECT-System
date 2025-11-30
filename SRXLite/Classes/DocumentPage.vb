Option Strict On

Imports System.Drawing
Imports System.Drawing.Imaging
Imports System.Data.SqlClient
Imports System.IO
Imports SRXLite.DataAccess
Imports SRXLite.Modules

Namespace Classes

    ''' <summary>
    '''
    ''' </summary>
    ''' <remarks></remarks>
    Public Class DocumentPage
        Implements IDisposable

        Private _bitmap As Bitmap
        Private _browserViewable As Boolean
        Private _contentType As String
        Private _db As AsyncDB
        Private _docPageID As Long
        Private _errorMessage As String
        Private _fileBytes() As Byte
        Private _fileExt As String
        Private _getBitmapUserCallback As AsyncCallback
        Private _hasErrors As Boolean = False
        Private _imageSettings As ImageSettings
        Private _subuserID As Integer
        Private _userCallback As AsyncCallback
        Private _userID As Short
        Private _userStateObject As Object

#Region " Constructors "

        ''' <summary>
        '''
        ''' </summary>
        ''' <param name="user"></param>
        ''' <remarks></remarks>
        Public Sub New(ByVal user As User)
            _db = New AsyncDB(AddressOf HandleError)
            _userID = user.UserID
            _subuserID = user.SubuserID
        End Sub

        ''' <summary>
        '''
        ''' </summary>
        ''' <param name="user"></param>
        ''' <param name="docPageID"></param>
        ''' <remarks></remarks>
        Public Sub New(ByVal user As User, ByVal docPageID As Long)
            _db = New AsyncDB(AddressOf HandleError)
            _userID = user.UserID
            _subuserID = user.SubuserID
            _docPageID = docPageID
        End Sub

        ''' <summary>
        '''
        ''' </summary>
        ''' <param name="data"></param>
        ''' <remarks></remarks>
        Public Sub New(ByVal data As DocumentPageGuid.GuidData)
            _db = New AsyncDB(AddressOf HandleError)
            _userID = data.UserID
            _subuserID = data.SubuserID
            _docPageID = data.DocPageID
        End Sub

#End Region

#Region " Properties "

        Public ReadOnly Property ContentType() As String
            Get
                Return _contentType
            End Get
        End Property

        Public ReadOnly Property ErrorMessage() As String
            Get
                Return _errorMessage
            End Get
        End Property

        Public ReadOnly Property FileExtension() As String
            Get
                Return _fileExt
            End Get
        End Property

        Public ReadOnly Property HasErrors() As Boolean
            Get
                Return _hasErrors
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

#End Region

#Region " ImageSettings "

        Public Structure ImageSettings

            ''' <summary>Height of the bitmap.</summary>
            Public Height As Integer

            '''' <summary>Preserve the original image aspect ratio.</summary>
            'Public PreserveAspectRatio As Boolean

            ''' <summary>RotateFlip setting to apply to the bitmap.</summary>
            Public RotateType As RotateFlipType

            ''' <summary>Scale the height of the bitmap to the specified width.</summary>
            Public ScaleHeight As Boolean

            ''' <summary>Scale the width of the bitmap to the specified height.</summary>
            Public ScaleWidth As Boolean

            ''' <summary>Width of the bitmap.</summary>
            Public Width As Integer

        End Structure

#End Region

#Region " Delete "

        ''' <summary>
        ''' Starts an asynchronous operation for deleting a page.
        ''' </summary>
        ''' <param name="callback"></param>
        ''' <param name="stateObject"></param>
        ''' <remarks></remarks>
        Public Function BeginDelete(ByVal callback As AsyncCallback, ByVal stateObject As Object) As IAsyncResult
            If IsNothing(_docPageID) Then Throw New Exception("DocPageID is missing")

            'Delete DB records, return file paths
            Dim command As New SqlCommand
            command.CommandText = "dsp_DocumentPage_Delete"
            command.CommandType = CommandType.StoredProcedure
            With command.Parameters
                .Add(getSqlParameter("@DocPageID", _docPageID))
                .Add(getSqlParameter("@UserID", _userID))
                .Add(getSqlParameter("@SubuserID", _subuserID))
                .Add(getSqlParameter("@ClientIP", GetClientIP()))
            End With
            Return _db.BeginExecuteNonQuery(command, callback, stateObject)
        End Function

        ''' <summary>
        ''' Ends the asynchronous operation for deleting a page.
        ''' </summary>
        ''' <param name="result"></param>
        ''' <remarks></remarks>
        Public Sub EndDelete(ByVal result As IAsyncResult)
            _db.EndExecuteNonQuery(result)
        End Sub

#End Region

#Region " GetBytes "

        ''' <summary>
        ''' Starts an asynchronous operation for retrieving the file.
        ''' </summary>
        ''' <param name="callback"></param>
        ''' <param name="stateObject"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function BeginGetBytes(ByVal callback As AsyncCallback, ByVal stateObject As Object) As IAsyncResult
            Dim command As New SqlCommand
            command.CommandText = "dsp_DocumentPage_GetFile"
            command.CommandType = CommandType.StoredProcedure
            command.Parameters.Add(getSqlParameter("@DocPageID", _docPageID))

            Return _db.BeginExecuteReader(command, callback, stateObject)
        End Function

        ''' <summary>
        ''' Ends the asynchronous operation for retrieving the file.
        ''' </summary>
        ''' <param name="result"></param>
        ''' <remarks></remarks>
        Public Function EndGetBytes(ByVal result As IAsyncResult) As Byte()
            Using reader As SqlDataReader = _db.EndExecuteReader(result)
                While reader.Read()
                    _contentType = NullCheck(reader("ContentType"))
                    _browserViewable = BoolCheck(reader("BrowserViewable"))
                    _fileExt = NullCheck(reader("FileExt"))
                    _fileBytes = DirectCast(reader("FileData"), Byte())
                End While
            End Using 'Closes connection

            Return _fileBytes
        End Function

        ''' <summary>
        '''
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetBytes() As Byte()
            Dim result As IAsyncResult = Me.BeginGetBytes(Nothing, Nothing)
            Return Me.EndGetBytes(result)
        End Function

#End Region

#Region " GetBitmap "

        ''' <summary>
        ''' Starts an asynchronous operation for retrieving an image as a bitmap.
        ''' </summary>
        ''' <param name="imageSettings"></param>
        ''' <param name="callback"></param>
        ''' <param name="stateObject"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function BeginGetBitmap(
         ByVal imageSettings As ImageSettings,
         ByVal callback As AsyncCallback,
         ByVal stateObject As Object) As IAsyncResult

            _getBitmapUserCallback = callback
            _imageSettings = imageSettings
            Return Me.BeginGetBytes(New AsyncCallback(AddressOf BeginGetBitmapCallback), stateObject)
        End Function

        ''' <summary>
        ''' Ends the asynchronous operation and returns the bitmap object.
        ''' </summary>
        ''' <param name="result"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function EndGetBitmap(ByVal result As IAsyncResult) As Bitmap
            Return _bitmap
        End Function

        ''' <summary>
        '''
        ''' </summary>
        ''' <param name="result"></param>
        ''' <remarks></remarks>
        Private Sub BeginGetBitmapCallback(ByVal result As IAsyncResult)
            Me.EndGetBytes(result) 'Sets _fileBytes
            ProcessFileAsBitmap(_imageSettings)
            _getBitmapUserCallback.BeginInvoke(result, Nothing, Nothing)
        End Sub

#End Region

#Region " ProcessFileAsBitmap "

        ''' <summary>
        ''' Returns the file as a bitmap with the specified image settings applied.
        ''' </summary>
        ''' <param name="imageSettings"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function ProcessFileAsBitmap(ByVal imageSettings As ImageSettings) As Bitmap
            If _fileBytes Is Nothing Then Throw New NotImplementedException("File data has not been initialized.")
            Return ProcessFileAsBitmap(_fileBytes, imageSettings)
        End Function

        ''' <summary>
        ''' Returns the file as a bitmap with the specified image settings applied.
        ''' </summary>
        ''' <param name="fileBytes"></param>
        ''' <param name="imageSettings"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function ProcessFileAsBitmap(ByVal fileBytes() As Byte, ByVal imageSettings As ImageSettings) As Bitmap
            Dim aspectRatio As Double
            Dim height, width As Integer

            Using ms As New MemoryStream(fileBytes, 0, fileBytes.Length)
                'Must keep stream open to access the image

                Using img As Image = Image.FromStream(ms)
                    aspectRatio = (img.Width / img.Height)

                    'Limit resolution to that of the actual image
                    height = Math.Min(imageSettings.Height, img.Height)
                    width = Math.Min(imageSettings.Width, img.Width)

                    If (height = 0) Then height = img.Height
                    If (width = 0) Then width = img.Width

                    'Scale the height or width
                    If imageSettings.ScaleHeight Then
                        height = CInt(Math.Round(width / aspectRatio))
                    ElseIf imageSettings.ScaleWidth Then
                        width = CInt(Math.Round(height * aspectRatio))
                    End If

                    _bitmap = New Bitmap(width, height, Imaging.PixelFormat.Format32bppArgb)

                    Using g As Graphics = Graphics.FromImage(_bitmap)
                        g.FillRectangle(New SolidBrush(Color.White), 0, 0, width, height)
                        g.DrawImage(img, 0, 0, width, height)
                        _bitmap.RotateFlip(imageSettings.RotateType)
                    End Using 'g

                    Return _bitmap '_bitmap disposed in Dispose method

                End Using 'img
            End Using 'ms
        End Function

#End Region

#Region " RotateFlip "

        Private _rotateFlipType As RotateFlipType
        Private _rotateFlipUserCallback As AsyncCallback

        ''' <summary>
        ''' Starts an asynchronous operation for saving RotateFlip settings to an image.
        ''' </summary>
        ''' <param name="rotateFlipType"></param>
        ''' <param name="callback"></param>
        ''' <param name="stateObject"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function BeginRotateFlip(ByVal rotateFlipType As RotateFlipType, ByVal callback As AsyncCallback, ByVal stateObject As Object) As IAsyncResult
            _rotateFlipUserCallback = callback
            _rotateFlipType = rotateFlipType
            Return Me.BeginGetBytes(New AsyncCallback(AddressOf BeginRotateFlipCallback), stateObject)
        End Function

        ''' <summary>
        ''' Ends the asynchronous operation.
        ''' </summary>
        ''' <param name="result"></param>
        ''' <remarks></remarks>
        Public Sub EndRotateFlip(ByVal result As IAsyncResult)
            Me.EndUpdate(result)
        End Sub

        ''' <summary>
        '''
        ''' </summary>
        ''' <param name="result"></param>
        ''' <remarks></remarks>
        Private Sub BeginRotateFlipCallback(ByVal result As IAsyncResult)
            Dim fileBytes() As Byte = Me.EndGetBytes(result)

            Using ms As New MemoryStream(fileBytes, 0, fileBytes.Length)
                'Must keep stream open to access the image
                Using bmp As New Bitmap(ms)
                    'Save RotateType to the original file
                    Try
                        bmp.RotateFlip(_rotateFlipType)

                        Dim enc As Encoder = Encoder.SaveFlag
                        Dim encParams As New EncoderParameters(1)
                        Dim encParam As New EncoderParameter(enc, EncoderValue.CompressionNone)
                        encParams.Param(0) = encParam

                        Using ms2 As New MemoryStream
                            bmp.Save(ms2, GetEncoderInfo(_contentType), encParams)
                            Me.BeginUpdate(ms2.ToArray(), _rotateFlipUserCallback, result.AsyncState)
                        End Using
                    Catch ex As Exception
                        LogError(ex.ToString, _userID, _subuserID)
                        HandleError(result, ex.ToString, _rotateFlipUserCallback)
                    End Try

                End Using 'bmp
            End Using 'ms
        End Sub

#End Region

#Region " Update "

        Public Function BeginUpdate(ByVal fileData As Byte(), ByVal callback As AsyncCallback, ByVal stateObject As Object) As IAsyncResult
            Dim command As New SqlCommand
            command.CommandText = "dsp_DocumentPage_UpdateFile"
            command.CommandType = CommandType.StoredProcedure
            With command.Parameters
                .Add(getSqlParameter("@DocPageID", _docPageID))
                .Add(getSqlParameter("@FileData", fileData))
            End With

            Return _db.BeginExecuteNonQuery(command, callback, stateObject)
        End Function

        ''' <summary>
        ''' Ends the asynchronous operation for retrieving the file.
        ''' </summary>
        ''' <param name="result"></param>
        ''' <remarks></remarks>
        Public Sub EndUpdate(ByVal result As IAsyncResult)
            _db.EndExecuteNonQuery(result)
        End Sub

#End Region

#Region " HandleError "

        Public Sub HandleError(ByVal result As IAsyncResult, ByVal errorMessage As String)
            HandleError(result, errorMessage, _userCallback)
        End Sub

        Public Sub HandleError(ByVal result As IAsyncResult, ByVal errorMessage As String, ByVal callback As AsyncCallback)
            _hasErrors = True
            _errorMessage = errorMessage
            If callback IsNot Nothing Then
                callback.Invoke(result)
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
                End If

                ' TODO: free shared unmanaged resources
                If _db IsNot Nothing Then _db.Dispose()
                If _bitmap IsNot Nothing Then _bitmap.Dispose()

            End If
            Me.disposedValue = True
        End Sub

#End Region

    End Class

End Namespace