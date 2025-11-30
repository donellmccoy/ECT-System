Option Strict On

Imports System.Data.SqlClient
Imports SRXLite.DataAccess
Imports SRXLite.Modules

Namespace Classes

    Public Class DocumentPageGuid
        Implements IDisposable

        Private _data As GuidData
        Private _db As AsyncDB
        Private _errorMessage As String
        Private _guid As Guid
        Private _hasErrors As Boolean = False
        'Private _onError As OnErrorDelegate
        'Public Delegate Sub OnErrorDelegate(ByVal errorMessage As String)

#Region " Constructors "

        Public Sub New(ByVal encryptedGuid As String)
            Dim crypto As New CryptoManager()
            _guid = New Guid(crypto.Decrypt(encryptedGuid))
            _db = New AsyncDB(AddressOf HandleError)
        End Sub

        'Public Sub New(ByVal encryptedGuid As String, ByVal onError As OnErrorDelegate)
        '	Dim crypto As New CryptoManager()
        '	_guid = New Guid(crypto.Decrypt(encryptedGuid))
        '	_db = New AsyncDB(AddressOf HandleError)
        '	_onError = onError
        'End Sub

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
            Public DocPageID As Long
            Public SubuserID As Integer
            Public UserID As Short
        End Structure

#End Region

#Region " GetData "

        ''' <summary>
        ''' Starts an asynchronous operation for creating a new GUID for a page.
        ''' </summary>
        ''' <param name="callback"></param>
        ''' <param name="stateObject"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function BeginGetData(ByVal callback As AsyncCallback, ByVal stateObject As Object) As IAsyncResult
            Dim command As New SqlCommand
            command.CommandText = "dsp_DocumentPageGuid_GetData"
            command.CommandType = CommandType.StoredProcedure
            With command.Parameters
                .Add(getSqlParameter("@DocPageGuid", _guid))
                .Add(getSqlParameter("@UserID", Nothing, SqlDbType.SmallInt, ParameterDirection.Output))
                .Add(getSqlParameter("@SubuserID", Nothing, SqlDbType.Int, ParameterDirection.Output))
                .Add(getSqlParameter("@DocPageID", Nothing, SqlDbType.BigInt, ParameterDirection.Output))
            End With

            Return _db.BeginExecuteNonQuery(command, callback, stateObject)
        End Function

        ''' <summary>
        ''' Ends the asynchronous operation and sets the GuidData object.
        ''' </summary>
        ''' <param name="result"></param>
        ''' <remarks></remarks>
        Public Sub EndGetData(ByVal result As IAsyncResult)
            Dim outputParams As New Dictionary(Of String, Object)
            outputParams.Add("@UserID", Nothing)
            outputParams.Add("@SubuserID", Nothing)
            outputParams.Add("@DocPageID", Nothing)

            _db.EndExecuteNonQuery(result, outputParams)

            _data = New GuidData
            _data.UserID = ShortCheck(outputParams("@UserID"))
            _data.SubuserID = IntCheck(outputParams("@SubuserID"))
            _data.DocPageID = LngCheck(outputParams("@DocPageID"))
        End Sub

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