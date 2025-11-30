Option Strict On

Imports System.Data.SqlClient
Imports SRXLite.Modules

Namespace DataAccess

    ''' <summary>
    '''
    ''' </summary>
    ''' <remarks></remarks>
    Public Class AsyncDB
        Implements IDisposable

        Private _command As SqlCommand
        Private _connectionString As String
        Private _onError As OnErrorDelegate

        Public Delegate Sub OnErrorDelegate(ByVal result As IAsyncResult, ByVal errorMessage As String)

#Region " Constructors "

        '''' <summary>
        ''''
        '''' </summary>
        '''' <remarks></remarks>
        ''Public Sub New()
        ''	_connectionString = ConfigurationManager.ConnectionStrings("SRXLite").ToString
        ''End Sub

        ''' <summary>
        '''
        ''' </summary>
        ''' <param name="onError"></param>
        ''' <remarks></remarks>
        Public Sub New(ByVal onError As OnErrorDelegate)
            _onError = onError
            _connectionString = ConfigurationManager.ConnectionStrings("SRXLite").ToString
        End Sub

#End Region

#Region " Properties "

        Public Property OnError() As OnErrorDelegate
            Get
                Return _onError
            End Get
            Set(ByVal value As OnErrorDelegate)
                _onError = value
            End Set
        End Property

#End Region

#Region " ExecuteNonQuery "

        ''' <summary>
        '''
        ''' </summary>
        ''' <param name="command"></param>
        ''' <param name="callback"></param>
        ''' <param name="stateObject"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function BeginExecuteNonQuery(ByVal command As SqlCommand, ByVal callback As AsyncCallback, ByVal stateObject As Object) As IAsyncResult
            Try
                If _command IsNot Nothing Then
                    _command.Connection.Dispose()
                End If

                _command = command
                _command.Connection = New SqlConnection(_connectionString)
                _command.Connection.Open()

                Return _command.BeginExecuteNonQuery(callback, stateObject) 'Keep connection open
            Catch
                _command.Connection.Dispose()
                Throw
            End Try
        End Function

        ''' <summary>
        '''
        ''' </summary>
        ''' <param name="result"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function EndExecuteNonQuery(ByVal result As IAsyncResult) As Integer
            Try
                Using _command.Connection
                    Return _command.EndExecuteNonQuery(result)
                End Using
            Catch ex As Exception
                If _onError IsNot Nothing Then _onError.Invoke(result, ex.ToString)
                Return -1
            End Try
        End Function

        ''' <summary>
        '''
        ''' </summary>
        ''' <param name="result"></param>
        ''' <param name="outputParamName"></param>
        ''' <param name="outputParamValue"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function EndExecuteNonQuery(ByVal result As IAsyncResult, ByVal outputParamName As String, ByRef outputParamValue As Guid) As Integer
            Dim value As Object = Nothing
            Dim rowCount As Integer = EndExecuteNonQuery(result, outputParamName, value)
            outputParamValue = New Guid(value.ToString)
            Return rowCount
        End Function

        ''' <summary>
        '''
        ''' </summary>
        ''' <param name="result"></param>
        ''' <param name="outputParamName"></param>
        ''' <param name="outputParamValue"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function EndExecuteNonQuery(ByVal result As IAsyncResult, ByVal outputParamName As String, ByRef outputParamValue As Integer) As Integer
            Dim value As Object = Nothing
            Dim rowCount As Integer = EndExecuteNonQuery(result, outputParamName, value)
            outputParamValue = IntCheck(value.ToString)
            Return rowCount
        End Function

        ''' <summary>
        '''
        ''' </summary>
        ''' <param name="result"></param>
        ''' <param name="outputParamName"></param>
        ''' <param name="outputParamValue"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function EndExecuteNonQuery(ByVal result As IAsyncResult, ByVal outputParamName As String, ByRef outputParamValue As Long) As Integer
            Dim value As Object = Nothing
            Dim rowCount As Integer = EndExecuteNonQuery(result, outputParamName, value)
            outputParamValue = LngCheck(value)
            Return rowCount
        End Function

        ''' <summary>
        '''
        ''' </summary>
        ''' <param name="result"></param>
        ''' <param name="outputParamName"></param>
        ''' <param name="outputParamValue"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function EndExecuteNonQuery(ByVal result As IAsyncResult, ByVal outputParamName As String, ByRef outputParamValue As String) As Integer
            Dim value As Object = Nothing
            Dim rowCount As Integer = EndExecuteNonQuery(result, outputParamName, value)
            outputParamValue = NullCheck(value)
            Return rowCount
        End Function

        ''' <summary>
        '''
        ''' </summary>
        ''' <param name="result"></param>
        ''' <param name="outputParamName"></param>
        ''' <param name="outputParamValue"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function EndExecuteNonQuery(ByVal result As IAsyncResult, ByVal outputParamName As String, ByRef outputParamValue As Object) As Integer
            Try
                Using _command.Connection
                    Dim rowCount As Integer = _command.EndExecuteNonQuery(result)
                    outputParamValue = _command.Parameters.Item(outputParamName).Value

                    Return rowCount
                End Using
            Catch ex As Exception
                If _onError IsNot Nothing Then _onError.Invoke(result, ex.ToString)
                Return -1
            End Try
        End Function

        Public Function EndExecuteNonQuery(ByVal result As IAsyncResult, ByRef outputParams As Dictionary(Of String, Object)) As Integer
            Try
                Using _command.Connection
                    Dim rowCount As Integer = _command.EndExecuteNonQuery(result)

                    If outputParams.Count > 0 Then
                        Dim keys(outputParams.Count - 1) As String
                        outputParams.Keys.CopyTo(keys, 0)
                        For Each key As String In keys
                            outputParams.Item(key) = _command.Parameters.Item(key).Value
                        Next
                    End If

                    Return rowCount
                End Using
            Catch ex As Exception
                If _onError IsNot Nothing Then _onError.Invoke(result, ex.ToString)
                Return -1
            End Try
        End Function

#End Region

#Region " ExecuteReader "

        ''' <summary>
        '''
        ''' </summary>
        ''' <param name="command"></param>
        ''' <param name="callback"></param>
        ''' <param name="stateObject"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function BeginExecuteReader(ByVal command As SqlCommand, ByVal callback As AsyncCallback, ByVal stateObject As Object) As IAsyncResult
            Try
                If _command IsNot Nothing Then
                    _command.Connection.Dispose()
                End If

                _command = command
                _command.Connection = New SqlConnection(_connectionString)
                _command.Connection.Open()
                Return _command.BeginExecuteReader(callback, stateObject, CommandBehavior.CloseConnection) 'Connection is closed when SqlDataReader is closed
            Catch
                _command.Connection.Dispose()
                Throw
            End Try
        End Function

        ''' <summary>
        '''
        ''' </summary>
        ''' <param name="result"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function EndExecuteReader(ByVal result As IAsyncResult) As SqlDataReader
            Try
                Return _command.EndExecuteReader(result) 'Calling code must close reader
            Catch ex As Exception
                _command.Connection.Dispose()
                If _onError IsNot Nothing Then _onError.Invoke(result, ex.ToString)
                Return Nothing
            End Try
        End Function

#End Region

#Region " IDisposable Support "

        Private disposedValue As Boolean = False 'To detect redundant calls

        ''' <summary>
        ''' This code added by Visual Basic to correctly implement the disposable pattern.
        ''' </summary>
        ''' <remarks></remarks>
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
                If _command IsNot Nothing Then
                    _command.Connection.Dispose()
                    _command.Dispose()
                End If

            End If
            Me.disposedValue = True
        End Sub

#End Region

    End Class

End Namespace