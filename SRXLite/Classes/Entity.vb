Option Strict On

Imports System.Data.SqlClient
Imports SRXLite.DataAccess
Imports SRXLite.DataTypes
Imports SRXLite.Modules

Namespace Classes

    Public Class Entity
        Implements IDisposable

        Private _context As HttpContext
        Private _db As AsyncDB
        Private _entityName As String
        Private _errorMessage As String
        Private _hasErrors As Boolean = False
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
        ''' <param name="user"></param>
        ''' <param name="entityName"></param>
        ''' <remarks></remarks>
        Public Sub New(ByVal user As User, ByVal entityName As String)
            _user = user
            _userID = user.UserID
            _subuserID = user.SubuserID
            _entityName = entityName
            _db = New AsyncDB(AddressOf HandleError)
            _context = HttpContext.Current
        End Sub

        ''' <summary>
        '''
        ''' </summary>
        ''' <param name="entityName"></param>
        ''' <param name="userID"></param>
        ''' <param name="subuserID"></param>
        ''' <remarks></remarks>
        Public Sub New(ByVal entityName As String, ByVal userID As Short, ByVal subuserID As Integer)
            _entityName = entityName
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

#Region " GetBatchList "

        ''' <summary>
        ''' Starts an asynchronous operation for retrieving a list of all batches for an Entity.
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function BeginGetBatchList(ByVal callback As AsyncCallback, ByVal stateObject As Object) As IAsyncResult
            Dim command As New SqlCommand
            command.CommandText = "dsp_Entity_GetBatchList"
            command.CommandType = CommandType.StoredProcedure
            command.Parameters.Add(getSqlParameter("@EntityName", _entityName))

            Return _db.BeginExecuteReader(command, callback, stateObject)
        End Function

        ''' <summary>
        ''' Finishes the asynchronous operation and returns the list.
        ''' </summary>
        ''' <param name="result"></param>
        ''' <remarks></remarks>
        Public Function EndGetBatchList(ByVal result As IAsyncResult) As List(Of BatchData)
            Dim list As New List(Of BatchData)
            Dim data As BatchData
            Dim appRootUrl As String = GetURL(_context)

            Using reader As SqlDataReader = _db.EndExecuteReader(result)
                While reader.Read()
                    data = New BatchData()
                    data.EntityName = _entityName
                    data.BatchID = IntCheck(reader("BatchID"))
                    data.BatchType = CType(reader("BatchTypeID"), BatchType)
                    data.DateCreated = DateCheck(reader("DateCreated"), Nothing)
                    data.DocTypeID = IntCheck(reader("DocTypeID"))
                    data.DocTypeName = NullCheck(reader("DocTypeName"))
                    data.DocDescription = NullCheck(reader("DocDescription"))
                    data.Location = NullCheck(reader("Location"))
                    data.PageCount = IntCheck(reader("PageCount"))
                    data.UploadedBySubuserName = NullCheck(reader("SubuserName"))

                    list.Add(data)
                End While
            End Using

            Return list
        End Function

#End Region

#Region " GetDocumentList "

        ''' <summary>
        ''' Starts an asynchronous operation for retrieving a list of all documents for an Entity
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function BeginGetDocumentList(ByVal callback As AsyncCallback, ByVal stateObject As Object) As IAsyncResult
            Dim command As New SqlCommand
            command.CommandText = "dsp_Entity_GetDocumentList"
            command.CommandType = CommandType.StoredProcedure
            command.Parameters.Add(getSqlParameter("@EntityName", _entityName))

            Return _db.BeginExecuteReader(command, callback, stateObject)
        End Function

        ''' <summary>
        ''' Finishes the asynchronous operation and returns the list.
        ''' </summary>
        ''' <param name="result"></param>
        ''' <remarks></remarks>
        Public Function EndGetDocumentList(ByVal result As IAsyncResult) As List(Of DocumentData)
            Dim list As New List(Of DocumentData)
            Dim data As DocumentData
            Dim appRootUrl As String = GetURL(_context)

            Using reader As SqlDataReader = _db.EndExecuteReader(result)
                While reader.Read()
                    data = New DocumentData()
                    With data
                        .EntityName = _entityName
                        .DocDate = DateCheck(reader("DocDate"), Nothing)
                        .DocDescription = NullCheck(reader("DocDescription"))
                        .DocID = LngCheck(reader("DocID"))
                        .DocStatus = CType(reader("DocStatusID"), DocumentStatus)
                        .DocTypeID = IntCheck(reader("DocTypeID"))
                        .DocTypeName = NullCheck(reader("DocTypeName"))
                        .FileExt = NullCheck(reader("FileExt"))
                        .PageCount = ShortCheck(reader("PageCount"))
                        .IconUrl = appRootUrl & "/icons/" & NullCheck(reader("IconFileName"))
                        .UploadedBySubuserName = NullCheck(reader("SubuserName"))
                        .UploadDate = DateCheck(reader("UploadDate"), Nothing)
                        .IsAppendable = BoolCheck(reader("Appendable"))
                        .OriginalFileName = NullCheck(reader("OriginalFileName"))
                    End With

                    list.Add(data)
                End While
            End Using

            Return list
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

            End If
            Me.disposedValue = True
        End Sub

#End Region

    End Class

End Namespace