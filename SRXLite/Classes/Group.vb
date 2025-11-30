Option Strict On

Imports System.Data.SqlClient
Imports SRXLite.DataAccess
Imports SRXLite.DataTypes
Imports SRXLite.Modules

Namespace Classes

    ''' <summary>
    '''
    ''' </summary>
    ''' <remarks></remarks>
    Public Class Group
        Implements IDisposable

        Private _context As HttpContext
        Private _db As AsyncDB
        Private _errorMessage As String
        Private _groupID As Long
        Private _hasErrors As Boolean = False
        Private _subuserID As Integer
        Private _userCallback As AsyncCallback
        Private _userID As Short

#Region " Constructors "

        ''' <summary>
        '''
        ''' </summary>
        ''' <param name="userID">ID of the requesting user.</param>
        ''' <remarks></remarks>
        Public Sub New(ByVal userID As Short)
            _db = New AsyncDB(AddressOf HandleError)
            _userID = userID
            _context = HttpContext.Current
        End Sub

        ''' <summary>
        '''
        ''' </summary>
        ''' <param name="userID">ID of the requesting user.</param>
        ''' <param name="groupID">ID of an existing group.</param>
        ''' <remarks></remarks>
        Public Sub New(ByVal userID As Short, ByVal subuserID As Integer, ByVal groupID As Long)
            _db = New AsyncDB(AddressOf HandleError)
            _userID = userID
            _subuserID = subuserID
            _groupID = groupID
            _context = HttpContext.Current
        End Sub

        ''' <summary>
        '''
        ''' </summary>
        ''' <param name="user"></param>
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
        ''' <param name="user"></param>
        ''' <param name="groupID"></param>
        ''' <remarks></remarks>
        Public Sub New(ByVal user As User, ByVal groupID As Long)
            _db = New AsyncDB(AddressOf HandleError)
            _userID = user.UserID
            _subuserID = user.SubuserID
            _groupID = groupID
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

#Region " AddDocument "

        ''' <summary>
        ''' Starts an asynchronous operation for adding a document to the group.
        ''' </summary>
        ''' <param name="DocID"></param>
        ''' <remarks></remarks>
        Public Function BeginAddDocument(
         ByVal docID As Long,
         ByVal callback As AsyncCallback,
         ByVal stateObject As Object) As IAsyncResult

            Dim command As New SqlCommand
            command.CommandText = "dsp_GroupDocument_Insert"
            command.CommandType = CommandType.StoredProcedure
            With command.Parameters
                .Add(getSqlParameter("@UserID", _userID))
                .Add(getSqlParameter("@SubuserID", _subuserID))
                .Add(getSqlParameter("@GroupID", _groupID))
                .Add(getSqlParameter("@DocID", docID))
                .Add(getSqlParameter("@ClientIP", GetClientIP(_context)))
            End With

            Return _db.BeginExecuteNonQuery(command, callback, stateObject)
        End Function

        ''' <summary>
        ''' Ends the asynchronous operation
        ''' </summary>
        ''' <param name="result"></param>
        ''' <remarks></remarks>
        Public Sub EndAddDocument(ByVal result As IAsyncResult)
            _db.EndExecuteNonQuery(result)
        End Sub

#End Region

#Region " Create "

        ''' <summary>
        ''' Starts an asynchronous operation for creating a new group.
        ''' </summary>
        ''' <param name="groupName">A name for the group.</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function BeginCreate(
         ByVal groupName As String,
         ByVal callback As AsyncCallback,
         ByVal stateObject As Object) As IAsyncResult

            Dim command As New SqlCommand
            command.CommandText = "dsp_Group_Insert"
            command.CommandType = CommandType.StoredProcedure
            With command.Parameters
                .Add(getSqlParameter("@SubuserID", _subuserID))
                .Add(getSqlParameter("@GroupName", groupName))
                .Add(getSqlParameter("@OwnerUserID", _userID))
                .Add(getSqlParameter("@ClientIP", GetClientIP(_context)))
                .Add(getSqlParameter("@GroupID", Nothing, SqlDbType.BigInt, ParameterDirection.Output))
            End With
            'LogError(groupName & " - " & _userID)
            Return _db.BeginExecuteNonQuery(command, callback, stateObject)
        End Function

        ''' <summary>
        ''' Ends the asynchronous operation and returns the group ID.
        ''' </summary>
        ''' <param name="result"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function EndCreate(ByVal result As IAsyncResult) As Long
            _db.EndExecuteNonQuery(result, "@GroupID", _groupID)
            Return _groupID
        End Function

#End Region

#Region " CreateCopy "

        ''' <summary>
        ''' Starts an asynchronous operation for creating a new group and
        ''' copying document references from the specified source group.
        ''' </summary>
        ''' <param name="groupName">A name for the group.</param>
        ''' <param name="sourceGroupID">The ID of an existing group from which to copy.</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function BeginCreateCopy(
         ByVal groupName As String,
         ByVal sourceGroupID As Long,
         ByVal callback As AsyncCallback,
         ByVal stateObject As Object) As IAsyncResult

            Dim command As New SqlCommand
            command.CommandText = "dsp_Group_InsertCopy"
            command.CommandType = CommandType.StoredProcedure
            With command.Parameters
                .Add(getSqlParameter("@SubuserID", _subuserID))
                .Add(getSqlParameter("@GroupName", groupName))
                .Add(getSqlParameter("@SourceGroupID", sourceGroupID))
                .Add(getSqlParameter("@OwnerUserID", _userID))
                .Add(getSqlParameter("@ClientIP", GetClientIP(_context)))
                .Add(getSqlParameter("@GroupID", SqlDbType.BigInt, ParameterDirection.Output))
            End With

            Return _db.BeginExecuteNonQuery(command, callback, stateObject)
        End Function

        ''' <summary>
        ''' Ends the asynchronous operation and returns the group ID.
        ''' </summary>
        ''' <param name="result"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function EndCreateCopy(ByVal result As IAsyncResult) As Long
            _db.EndExecuteNonQuery(result, "@GroupID", _groupID)
            Return _groupID
        End Function

#End Region

#Region " GetDocumentCount "

        ''' <summary>
        ''' Returns a count of the number of documents in the group.
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetDocumentCount() As Integer
            Return IntCheck(DB.ExecuteScalar("dsp_Group_GetDocumentCount " & _groupID))
        End Function

#End Region

#Region " GetDocumentList "

        ''' <summary>
        ''' Returns document data for all documents in the group.
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function BeginGetDocumentList(ByVal callback As AsyncCallback, ByVal stateObject As Object) As IAsyncResult
            Dim command As New SqlCommand
            command.CommandText = "dsp_Group_GetDocumentList"
            command.CommandType = CommandType.StoredProcedure
            With command.Parameters
                .Add(getSqlParameter("@UserID", _userID))
                .Add(getSqlParameter("@SubuserID", _subuserID))
                .Add(getSqlParameter("@GroupID", _groupID))
            End With

            Return _db.BeginExecuteReader(command, callback, stateObject)
        End Function

        ''' <summary>
        '''
        ''' </summary>
        ''' <param name="result"></param>
        ''' <remarks></remarks>
        Public Function EndGetDocumentList(ByVal result As IAsyncResult) As List(Of DocumentData)
            Dim list As New List(Of DocumentData)
            Dim docData As DocumentData
            Dim appRootUrl As String = GetURL(_context)
            Dim crypto As New CryptoManager(_context)

            Using reader As SqlDataReader = _db.EndExecuteReader(result)
                While reader.Read()
                    docData = New DocumentData()
                    With docData
                        .DocDate = DateCheck(reader("DocDate"), Nothing)
                        .DocDescription = NullCheck(reader("DocDescription"))
                        .DocID = LngCheck(reader("DocID"))
                        .DocStatus = CType(reader("DocStatusID"), DocumentStatus)
                        .DocTypeID = IntCheck(reader("DocTypeID"))
                        .DocTypeName = NullCheck(reader("DocTypeName"))
                        .EntityName = NullCheck(reader("EntityName"))
                        .FileExt = NullCheck(reader("FileExt"))
                        .PageCount = ShortCheck(reader("PageCount"))
                        .IconUrl = appRootUrl & "/icons/" & NullCheck(reader("IconFileName"))
                        .UploadedBySubuserName = NullCheck(reader("SubuserName"))
                        .UploadDate = DateCheck(reader("UploadDate"), Nothing)
                        .IsAppendable = BoolCheck(reader("Appendable"))
                        .OriginalFileName = NullCheck(reader("OriginalFileName"))
                    End With

                    list.Add(docData)
                End While
            End Using 'Closes connection

            Return list
        End Function

#End Region

#Region " MoveDocument "

        ''' <summary>
        ''' Starts an asynchronous operation for moving a document to a different group.
        ''' </summary>
        ''' <param name="docID"></param>
        ''' <param name="targetGroupID"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function BeginMoveDocument(
         ByVal docID As Long,
         ByVal targetGroupID As Long,
         ByVal callback As AsyncCallback,
         ByVal stateObject As Object) As IAsyncResult

            Dim command As New SqlCommand
            command.CommandText = "dsp_GroupDocument_Move"
            command.CommandType = CommandType.StoredProcedure
            With command.Parameters
                .Add(getSqlParameter("@UserID", _userID))
                .Add(getSqlParameter("@SubuserID", _subuserID))
                .Add(getSqlParameter("@DocID", docID))
                .Add(getSqlParameter("@SourceGroupID", _groupID))
                .Add(getSqlParameter("@TargetGroupID", targetGroupID))
            End With

            Return _db.BeginExecuteNonQuery(command, callback, stateObject)
        End Function

        ''' <summary>
        ''' Ends the asynchronous operation.
        ''' </summary>
        ''' <param name="result"></param>
        ''' <remarks></remarks>
        Public Sub EndMoveDocument(ByVal result As IAsyncResult)
            _db.EndExecuteNonQuery(result)
        End Sub

#End Region

#Region " RemoveDocument "

        ''' <summary>
        ''' Starts an asynchronous operation for removing a document from the group.
        ''' </summary>
        ''' <param name="DocID"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function BeginRemoveDocument(
         ByVal docID As Long,
         ByVal callback As AsyncCallback,
         ByVal stateObject As Object) As IAsyncResult

            Dim command As New SqlCommand
            command.CommandText = "dsp_GroupDocument_Delete"
            command.CommandType = CommandType.StoredProcedure
            With command.Parameters
                .Add(getSqlParameter("@UserID", _userID))
                .Add(getSqlParameter("@SubuserID", _subuserID))
                .Add(getSqlParameter("@GroupID", _groupID))
                .Add(getSqlParameter("@DocID", docID))
                .Add(getSqlParameter("@ClientIP", GetClientIP(_context)))
                '.Add(getSqlParameter("@DocCount", Nothing, SqlDbType.Int, ParameterDirection.Output))
            End With

            Return _db.BeginExecuteNonQuery(command, callback, stateObject)
        End Function

        ''' <summary>
        ''' Ends the asynchronous operation.
        ''' </summary>
        ''' <param name="result"></param>
        ''' <remarks></remarks>
        Public Sub EndRemoveDocument(ByVal result As IAsyncResult)
            _db.EndExecuteNonQuery(result)
        End Sub

#End Region

#Region " CopyGroupDocuments "

        ''' <summary>
        '''
        ''' </summary>
        ''' <param name="oldGroupId"></param>
        ''' <param name="newGroupId"></param>
        ''' <param name="oldDocTypeId"></param>
        ''' <param name="newDocTypeId"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function BeginCopyGroupDocuments(
         ByVal oldGroupId As Long,
         ByVal newGroupId As Long,
         ByVal oldDocTypeId As Long,
         ByVal newDocTypeId As Long,
         ByVal callback As AsyncCallback,
         ByVal stateObject As Object) As IAsyncResult

            Dim command As New SqlCommand
            command.CommandText = "DSP_CopyGroupDocuments"
            command.CommandType = CommandType.StoredProcedure
            With command.Parameters
                .Add(getSqlParameter("@oldGroupId", oldGroupId))
                .Add(getSqlParameter("@newGroupId", newGroupId))
                .Add(getSqlParameter("@oldDocTypeId", oldDocTypeId))
                .Add(getSqlParameter("@newDocTypeId", newDocTypeId))
            End With

            Return _db.BeginExecuteNonQuery(command, callback, stateObject)
        End Function

        ''' <summary>
        ''' Ends the asynchronous operation.
        ''' </summary>
        ''' <param name="result"></param>
        ''' <remarks></remarks>
        Public Sub EndCopyGroupDocuments(ByVal result As IAsyncResult)
            _db.EndExecuteNonQuery(result)
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
                End If

                ' TODO: free shared unmanaged resources
                If _db IsNot Nothing Then _db.Dispose()

            End If
            Me.disposedValue = True
        End Sub

#End Region

    End Class

End Namespace