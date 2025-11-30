Imports ALOD.Data

Namespace Common

    Public Class majcom

        Protected _active As Integer = 0
        Protected _name As String = ""
        Protected _value As Integer = 0

        Public Property Active() As String
            Get
                Return _active
            End Get
            Set(value As String)
                _active = value
            End Set
        End Property

        Public Property Name() As String
            Get
                Return _name
            End Get
            Set(value As String)
                _name = value
            End Set
        End Property

        Public Property Value() As Integer
            Get
                Return _value
            End Get
            Set(value As Integer)
                _value = value
            End Set
        End Property

        ' Get all for Sys Admin
        Public Function GetMAJCOM() As DataTable
            Dim sqlDS As New SqlDataStore()
            Dim ds As DataSet
            Dim dt As New DataTable()
            ds = sqlDS.ExecuteDataSet("core_lookUps_sp_GetMAJCOM")
            dt = ds.Tables(0)

            Return dt

        End Function

        ' Get Active only for user
        Public Function GetMAJCOM(ByVal Value As Integer, ByVal filter As Integer) As DataTable
            Dim sqlDS As New SqlDataStore()
            Dim ds As DataSet
            Dim dt As New DataTable()
            ds = sqlDS.ExecuteDataSet("core_lookUps_sp_GetMAJCOM", Value, filter)
            dt = ds.Tables(0)

            Return dt

        End Function

        ' Add or Edit
        Public Sub InsertMAJCOM(ByVal Value As Integer, ByVal Name As String, ByVal Active As Integer)
            Dim adapter As New SqlDataStore()
            adapter.ExecuteNonQuery("core_lookups_sp_Insert_MAJCOM", Value, Name, Active)

        End Sub

        Public Sub UpdateMAJCOM(ByVal Value As Integer, ByVal Name As String, ByVal Active As Integer)
            Dim adapter As New SqlDataStore()
            adapter.ExecuteNonQuery("core_lookups_sp_Insert_MAJCOM", Value, Name, Active)

        End Sub

    End Class

End Namespace