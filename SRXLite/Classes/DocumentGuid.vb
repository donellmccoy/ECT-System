Option Strict On

Imports System.Data.SqlClient
Imports SRXLite.DataAccess
Imports SRXLite.Modules

Namespace Classes

    Public Class DocumentGuid
        Implements IDisposable

        Private _data As GuidData
        Private _db As AsyncDB
        Private _errorMessage As String
        Private _guid As Guid
        Private _hasErrors As Boolean = False

#Region " Constructors "

        ''' <summary>
        '''
        ''' </summary>
        ''' <remarks></remarks>
        Public Sub New()
            _db = New AsyncDB(AddressOf HandleError)
        End Sub

        ''' <summary>
        '''
        ''' </summary>
        ''' <param name="encryptedGuid"></param>
        ''' <remarks></remarks>
        Public Sub New(ByVal encryptedGuid As String)
            Dim crypto As New CryptoManager()
            _guid = New Guid(crypto.Decrypt(encryptedGuid))
            _db = New AsyncDB(AddressOf HandleError)
        End Sub

#End Region

#Region " Properties "

        Public ReadOnly Property Data() As GuidData
            Get
                Return _data
            End Get
        End Property

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

#Region " GuidData "

        Public Structure GuidData
            Public DocGuid As Guid
            Public DocID As Long
            Public DocTypeID As Integer
            Public DocTypeName As String
            Public EntityID As Integer
            Public EntityName As String
            Public GroupID As Long
            Public IsReadOnly As Boolean
            Public SubuserID As Integer
            Public UserID As Short
        End Structure

#End Region

#Region " Create "

        ''' <summary>
        ''' Starts an asynchronous operation for creating a new GUID for a document.
        ''' </summary>
        ''' <param name="data"></param>
        ''' <param name="callback"></param>
        ''' <param name="stateObject"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function BeginCreate(ByVal data As GuidData, ByVal callback As AsyncCallback, ByVal stateObject As Object) As IAsyncResult
            Dim command As New SqlCommand
            command.CommandText = "dsp_DocumentGuid_Create"
            command.CommandType = CommandType.StoredProcedure
            With command.Parameters
                .Add(getSqlParameter("@UserID", data.UserID))
                .Add(getSqlParameter("@SubuserID", data.SubuserID))
                .Add(getSqlParameter("@DocID", data.DocID))
                .Add(getSqlParameter("@EntityName", data.EntityName))
                .Add(getSqlParameter("@DocTypeID", data.DocTypeID))
                .Add(getSqlParameter("@GroupID", data.GroupID))
                .Add(getSqlParameter("@ReadOnly", BoolCheck(data.IsReadOnly)))
                .Add(getSqlParameter("@DocGuid", Nothing, SqlDbType.UniqueIdentifier, ParameterDirection.Output))
            End With

            Return _db.BeginExecuteNonQuery(command, callback, stateObject)
        End Function

        ''' <summary>
        ''' Ends the asynchronous operation and returns the GUID.
        ''' </summary>
        ''' <param name="result"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function EndCreate(ByVal result As IAsyncResult) As Guid
            _db.EndExecuteNonQuery(result, "@DocGuid", _guid)
            Return _guid
        End Function

#End Region

#Region " GetData "

        ''' <summary>
        ''' Starts an asynchronous operation for retrieving data referenced by the GUID.
        ''' </summary>
        ''' <param name="callback"></param>
        ''' <param name="stateObject"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function BeginGetData(ByVal callback As AsyncCallback, ByVal stateObject As Object) As IAsyncResult
            Dim command As New SqlCommand
            command.CommandText = "dsp_DocumentGuid_GetData"
            command.CommandType = CommandType.StoredProcedure
            command.Parameters.Add(getSqlParameter("@DocGuid", _guid))

            Return _db.BeginExecuteReader(command, callback, stateObject)
        End Function

        ''' <summary>
        ''' Ends the asynchronous operation and returns a GuidData object.
        ''' </summary>
        ''' <param name="result"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function EndGetData(ByVal result As IAsyncResult) As GuidData
            Using reader As SqlDataReader = _db.EndExecuteReader(result)
                While reader.Read()
                    With _data
                        .UserID = ShortCheck(reader("UserID"))
                        .SubuserID = IntCheck(reader("SubuserID"))
                        .DocID = LngCheck(reader("DocID"))
                        .EntityID = IntCheck(reader("EntityID"))
                        .EntityName = NullCheck(reader("EntityName"))
                        .DocTypeID = IntCheck(reader("DocTypeID"))
                        .DocTypeName = NullCheck(reader("DocTypeName"))
                        .GroupID = LngCheck(reader("GroupID"))
                        .IsReadOnly = BoolCheck(reader("ReadOnly"), True)
                        .DocGuid = _guid
                    End With
                End While
            End Using

            Return _data
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