Option Strict On

Imports System.Data.SqlClient

Namespace DataAccess

    ''' <summary>
    '''
    ''' </summary>
    ''' <remarks></remarks>
    Public Module DB

#Region " getConnectionString "

        ''' <summary>
        '''
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Private Function getConnectionString() As String
            Return ConfigurationManager.ConnectionStrings("SRXLite").ToString
        End Function

#End Region

#Region " getSqlParameter "

        Public Function getSqlParameter(ByVal name As String, ByVal value As Boolean, Optional ByVal direction As ParameterDirection = ParameterDirection.Input) As SqlParameter
            Return getSqlParameter(name, value, SqlDbType.Bit, direction)
        End Function

        Public Function getSqlParameter(ByVal name As String, ByVal value As Byte, Optional ByVal direction As ParameterDirection = ParameterDirection.Input) As SqlParameter
            Return getSqlParameter(name, value, SqlDbType.TinyInt, direction)
        End Function

        Public Function getSqlParameter(ByVal name As String, ByVal value As Byte?, Optional ByVal direction As ParameterDirection = ParameterDirection.Input) As SqlParameter
            Return getSqlParameter(name, value, SqlDbType.TinyInt, direction)
        End Function

        Public Function getSqlParameter(ByVal name As String, ByVal value As Byte(), Optional ByVal direction As ParameterDirection = ParameterDirection.Input) As SqlParameter
            Return getSqlParameter(name, value, SqlDbType.VarBinary, direction)
        End Function

        Public Function getSqlParameter(ByVal name As String, ByVal value As Date, Optional ByVal direction As ParameterDirection = ParameterDirection.Input) As SqlParameter
            Return getSqlParameter(name, value, SqlDbType.DateTime, direction)
        End Function

        Public Function getSqlParameter(ByVal name As String, ByVal value As Long, Optional ByVal direction As ParameterDirection = ParameterDirection.Input) As SqlParameter
            Return getSqlParameter(name, value, SqlDbType.BigInt, direction)
        End Function

        Public Function getSqlParameter(ByVal name As String, ByVal value As Long?, Optional ByVal direction As ParameterDirection = ParameterDirection.Input) As SqlParameter
            Return getSqlParameter(name, value, SqlDbType.BigInt, direction)
        End Function

        Public Function getSqlParameter(ByVal name As String, ByVal value As Short, Optional ByVal direction As ParameterDirection = ParameterDirection.Input) As SqlParameter
            Return getSqlParameter(name, value, SqlDbType.SmallInt, direction)
        End Function

        Public Function getSqlParameter(ByVal name As String, ByVal value As Short?, Optional ByVal direction As ParameterDirection = ParameterDirection.Input) As SqlParameter
            Return getSqlParameter(name, value, SqlDbType.SmallInt, direction)
        End Function

        Public Function getSqlParameter(ByVal name As String, ByVal value As String, Optional ByVal direction As ParameterDirection = ParameterDirection.Input) As SqlParameter
            Return getSqlParameter(name, value, SqlDbType.VarChar, direction)
        End Function

        Public Function getSqlParameter(ByVal name As String, ByVal value As Guid, Optional ByVal direction As ParameterDirection = ParameterDirection.Input) As SqlParameter
            Return getSqlParameter(name, value, SqlDbType.UniqueIdentifier, direction)
        End Function

        Public Function getSqlParameter(ByVal name As String, ByVal value As Object, ByVal dbType As SqlDbType, Optional ByVal direction As ParameterDirection = ParameterDirection.Input) As SqlParameter
            Dim parameter As New SqlParameter(name, dbType)
            parameter.Direction = direction
            parameter.Value = value
            Return parameter
        End Function

        Public Function getSqlParameter(ByVal name As String, ByVal value As Object, ByVal dbType As SqlDbType, ByVal size As Integer, Optional ByVal direction As ParameterDirection = ParameterDirection.Input) As SqlParameter
            Dim parameter As New SqlParameter(name, dbType, size)
            parameter.Direction = direction
            parameter.Value = value
            Return parameter
        End Function

#End Region

#Region " ExecuteDataset "

        ''' <summary>
        '''
        ''' </summary>
        ''' <param name="sql"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function ExecuteDataset(ByVal sql As String) As DataSet
            Return ExecuteDataset(New SqlCommand(sql))
        End Function

        ''' <summary>
        '''
        ''' </summary>
        ''' <param name="command"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function ExecuteDataset(ByVal command As SqlCommand) As DataSet
            Dim da As New SqlDataAdapter(command)
            Dim ds As New DataSet
            Try
                command.Connection = New SqlConnection(getConnectionString())
                da.Fill(ds)
                Return ds
            Finally
                ds.Dispose()
                da.Dispose()
                command.Connection.Dispose()
                command.Dispose()
            End Try
        End Function

#End Region

#Region " ExecuteNonQuery "

        ''' <summary>
        '''
        ''' </summary>
        ''' <param name="sql"></param>
        ''' <remarks></remarks>
        Public Sub ExecuteNonQuery(ByVal sql As String)
            ExecuteNonQuery(New SqlCommand(sql))
        End Sub

        ''' <summary>
        '''
        ''' </summary>
        ''' <param name="command"></param>
        ''' <remarks></remarks>
        Public Sub ExecuteNonQuery(ByVal command As SqlCommand)
            Using command
                Using connection As New SqlConnection(getConnectionString())
                    command.Connection = connection
                    command.Connection.Open()
                    command.ExecuteNonQuery()
                End Using
            End Using
        End Sub

        ''' <summary>
        '''
        ''' </summary>
        ''' <param name="command"></param>
        ''' <param name="outputParamName"></param>
        ''' <param name="outputParamValue"></param>
        ''' <remarks></remarks>
        Public Sub ExecuteNonQuery(ByVal command As SqlCommand, ByVal outputParamName As String, ByRef outputParamValue As Object)
            Using command
                Using connection As New SqlConnection(getConnectionString())
                    command.Connection = connection
                    command.Connection.Open()
                    command.ExecuteNonQuery()
                    outputParamValue = command.Parameters.Item(outputParamName).Value
                End Using
            End Using
        End Sub

        ''' <summary>
        '''
        ''' </summary>
        ''' <param name="command"></param>
        ''' <param name="outputParamName"></param>
        ''' <param name="outputParamValue"></param>
        ''' <remarks></remarks>
        Public Sub ExecuteNonQuery(ByVal command As SqlCommand, ByVal outputParamName As String, ByRef outputParamValue As String)
            Dim value As Object = Nothing
            ExecuteNonQuery(command, outputParamName, value)
            outputParamValue = SRXLite.Modules.Util.NullCheck(value)
        End Sub

        ''' <summary>
        '''
        ''' </summary>
        ''' <param name="command"></param>
        ''' <param name="outputParams"></param>
        ''' <remarks></remarks>
        Public Sub ExecuteNonQuery(ByVal command As SqlCommand, ByRef outputParams As Dictionary(Of String, Object))
            Using command
                Using connection As New SqlConnection(getConnectionString())
                    command.Connection = connection
                    command.Connection.Open()
                    command.ExecuteNonQuery()

                    If outputParams.Count > 0 Then
                        Dim keys(outputParams.Count - 1) As String
                        outputParams.Keys.CopyTo(keys, 0)
                        For Each key As String In keys
                            outputParams.Item(key) = command.Parameters.Item(key).Value
                        Next
                    End If

                End Using
            End Using
        End Sub

#End Region

#Region " ExecuteReader "

        ''' <summary>
        '''
        ''' </summary>
        ''' <param name="command"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function ExecuteReader(ByVal command As SqlCommand) As SqlDataReader
            Try
                command.Connection = New SqlConnection(getConnectionString())
                command.Connection.Open()
                Return command.ExecuteReader(CommandBehavior.CloseConnection)   'Connection is closed when SqlDataReader is closed
            Catch ex As Exception
                command.Connection.Dispose()
                command.Dispose()
                Throw ex
            End Try
        End Function

#End Region

#Region " ExecuteScalar "

        ''' <summary>
        '''
        ''' </summary>
        ''' <param name="sql"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function ExecuteScalar(ByVal sql As String) As Object
            Return ExecuteScalar(New SqlCommand(sql))
        End Function

        ''' <summary>
        '''
        ''' </summary>
        ''' <param name="command"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function ExecuteScalar(ByVal command As SqlCommand) As Object
            Dim connection As New SqlConnection(getConnectionString())
            Try
                command.Connection = connection
                connection.Open()
                Return command.ExecuteScalar()
            Finally
                connection.Dispose()
                command.Dispose()
            End Try
        End Function

#End Region

    End Module

End Namespace