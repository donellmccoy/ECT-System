Option Strict On

Imports System.Data.SqlClient
Imports SRXLite.DataAccess
Imports SRXLite.DataTypes
Imports SRXLite.Modules

Namespace Classes

    Public Class Batch
        Implements IDisposable

        Private _batchID As Integer
        Private _batchType As BatchType
        Private _context As HttpContext
        Private _db As AsyncDB
        Private _doc As Document
        Private _docID As Long
        Private _errorMessage As String
        Private _hasErrors As Boolean = False
        Private _location As String
        Private _subuserID As Integer
        Private _user As User
        Private _userID As Short

#Region " Constructors "

        ''' <summary>
        '''
        ''' </summary>
        ''' <remarks></remarks>
        Public Sub New()
            _context = HttpContext.Current
        End Sub

        ''' <summary>
        '''
        ''' </summary>
        ''' <param name="user"></param>
        ''' <remarks></remarks>
        Public Sub New(ByVal user As User)
            _user = user
            _db = New AsyncDB(AddressOf HandleError)
            _context = HttpContext.Current
        End Sub

        ''' <summary>
        '''
        ''' </summary>
        ''' <param name="userID"></param>
        ''' <param name="subuserID"></param>
        ''' <param name="docID"></param>
        ''' <remarks></remarks>
        Public Sub New(ByVal docID As Long, ByVal userID As Short, ByVal subuserID As Integer)
            _docID = docID
            _userID = userID
            _subuserID = subuserID
            _db = New AsyncDB(AddressOf HandleError)
            _context = HttpContext.Current
        End Sub

#End Region

#Region " Properties "

        Public ReadOnly Property ErrorMessage() As String
            Get
                Return _errorMessage
            End Get
        End Property

        Public ReadOnly Property HasErrors() As Boolean
            Get
                Return _hasErrors
            End Get
        End Property

#End Region

#Region " Create "

        ''' <summary>
        ''' Starts an asynchronous operation for creating a new batch.
        ''' </summary>
        ''' <param name="docUploadKeys"></param>
        ''' <param name="callback"></param>
        ''' <param name="stateObject"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function BeginCreate(ByVal batchType As BatchType, ByVal location As String, ByVal docUploadKeys As UploadKeys, ByVal callback As AsyncCallback, ByVal stateObject As Object) As IAsyncResult
            If _docID = 0 Then Throw New Exception("DocID is missing")

            'Save to DB, get docID
            Dim command As New SqlCommand
            command.CommandText = "dsp_Batch_Insert"
            command.CommandType = CommandType.StoredProcedure
            With command.Parameters
                .Add(getSqlParameter("@DocID", _docID))
                .Add(getSqlParameter("@BatchTypeID", batchType))
                .Add(getSqlParameter("@Location", location))
                .Add(getSqlParameter("@EntityName", docUploadKeys.EntityName))
                .Add(getSqlParameter("@DocTypeID", docUploadKeys.DocTypeID))
                .Add(getSqlParameter("@OwnerUserID", _userID))
                .Add(getSqlParameter("@OwnerSubuserID", _subuserID))
                .Add(getSqlParameter("@ClientIP", GetClientIP(_context)))
                .Add(getSqlParameter("@BatchID", Nothing, SqlDbType.BigInt, ParameterDirection.Output))
            End With

            'Insert a new document record, output the docID
            Return _db.BeginExecuteNonQuery(command, callback, stateObject)
        End Function

        ''' <summary>
        ''' Finishes the asynchronous operation for creating a new batch.
        ''' </summary>
        ''' <param name="result"></param>
        ''' <remarks></remarks>
        Public Sub EndCreate(ByVal result As IAsyncResult)
            _db.EndExecuteNonQuery(result, "@BatchID", _batchID)
        End Sub

#End Region

#Region " GetUploadUrl "

        ''' <summary>
        ''' Starts an asynchronous operation for generating a URL to upload a batch document.
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function BeginGetUploadUrl(
         ByVal batchType As BatchType,
         ByVal location As String,
         ByVal entityName As String,
         ByVal docTypeID As Integer,
         ByVal stylesheetUrl As String,
         ByVal entityDisplayText As String,
         ByVal callback As AsyncCallback,
         ByVal stateObject As Object) As IAsyncResult

            _batchType = batchType
            _location = location

            Select Case _batchType
                Case DataTypes.BatchType.Entity : docTypeID = DataTypes.BatchDocumentType.BatchImport
                Case DataTypes.BatchType.DocType : entityName = "BatchImport"
                Case Else
            End Select

            _doc = New Document(_user)
            Return _doc.BeginGetUploadUrl(entityName, docTypeID, 0, stylesheetUrl, entityDisplayText, callback, stateObject)
        End Function

        ''' <summary>
        ''' Finishes the asynchronous operation and returns a URL to upload a batch document.
        ''' </summary>
        ''' <param name="result"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function EndGetUploadUrl(ByVal result As IAsyncResult) As String
            Dim values() As String = {ByteCheck(_batchType).ToString, _location}
            Dim id As String = String.Format("bt={0}&l={1}", values)
            Dim crypto As New CryptoManager(_context)
            Return _doc.EndGetUploadUrl(result) & "&bid=" & crypto.EncryptForUrl(id)
        End Function

#End Region

#Region " HandleError "

        Public Sub HandleError(ByVal result As IAsyncResult, ByVal errorMessage As String)
            _hasErrors = True
            _errorMessage = errorMessage
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
                End If

                ' TODO: free shared unmanaged resources
                If _db IsNot Nothing Then _db.Dispose()
                If _doc IsNot Nothing Then _doc.Dispose()

            End If
            Me.disposedValue = True
        End Sub

#End Region

    End Class

End Namespace