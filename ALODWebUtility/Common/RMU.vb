Imports ALOD.Data

Namespace Common

    Public Class RMU
        Inherits IntValuedLookupItem

        'Protected _csId As Integer
        Protected _pasCode As String

        'Public Property CSId As Integer
        '    Get
        '        Return _csId
        '    End Get
        '    Set(value As Integer)
        '        _csId = value
        '    End Set
        'End Property

        Public Property PAS As String
            Get
                Return _pasCode
            End Get
            Set(value As String)
                _pasCode = value
            End Set
        End Property

        ' Get all RMU records from the lookup table for the Sys Admin
        Public Overrides Function GetRecords() As DataTable
            Dim sqlDS As New SqlDataStore()
            Return sqlDS.ExecuteDataSet("core_lookUps_sp_RMUNames").Tables(0)
        End Function

        ' Insert new RMU into the lookup table
        Public Sub InsertRMU(ByVal Value As Integer, ByVal Name As String, ByVal PAS As String, ByVal Collocated As Integer)
            Dim adapter As New SqlDataStore()
            adapter.ExecuteNonQuery("core_lookups_sp_Insert_RMU", Value, Name, PAS, Collocated)
        End Sub

        ' Edit/Update an existing RMU in the lookup table
        Public Sub UpdateRMU(ByVal Value As Integer, ByVal Name As String, ByVal PAS As String, ByVal Collocated As Integer)
            Dim adapter As New SqlDataStore()
            adapter.ExecuteNonQuery("core_lookups_sp_Insert_RMU", Value, Name, PAS, Collocated)
        End Sub

    End Class

End Namespace