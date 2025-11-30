Option Strict On

Imports System.Data.SqlClient
Imports SRXLite.DataAccess
Imports SRXLite.Modules

Namespace Classes

    Public Class UsageLog

        Private _context As HttpContext

#Region " Constructors "

        ''' <summary>
        '''
        ''' </summary>
        ''' <param name="context"></param>
        ''' <remarks></remarks>
        Public Sub New(ByVal context As HttpContext)
            _context = context
        End Sub

#End Region

#Region " UsageData "

        Public Class UsageData
            Public ActionType As DataTypes.ActionType?
            Public BatchID As Integer?
            Public DocID As Long?
            Public DocPageID As Long?
            Public DocStatus As DataTypes.DocumentStatus?
            Public GroupID As Long?
            Public IPAddress As String
            Public SubuserID As Integer?
            Public UserID As Short
        End Class

#End Region

#Region " Insert "

        Public Sub Insert(ByVal data As UsageData)
            Dim command As New SqlCommand
            command.CommandText = "dsp_UsageLog_Insert"
            command.CommandType = CommandType.StoredProcedure
            With command.Parameters
                .Add(getSqlParameter("@UserID", data.UserID))
                .Add(getSqlParameter("@SubuserID", data.SubuserID))
                .Add(getSqlParameter("@ActionTypeID", data.ActionType))
                .Add(getSqlParameter("@DocStatusID", data.DocStatus))
                .Add(getSqlParameter("@DocID", data.DocID))
                .Add(getSqlParameter("@DocPageID", data.DocPageID))
                .Add(getSqlParameter("@GroupID", data.GroupID))
                .Add(getSqlParameter("@BatchID", data.BatchID))
                .Add(getSqlParameter("@ClientIP", GetClientIP(_context)))
            End With

            DB.ExecuteNonQuery(command)
        End Sub

#End Region

    End Class

End Namespace