Namespace Common

    Public MustInherit Class IntValuedLookupItem

        Protected _name As String = String.Empty
        Protected _value As Integer = 0             ' Unique identifier of the lookup item
        ' Name or description of the lookup item

#Region "Properties"

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

#End Region

        ' Get all LookupItem records from the lookup table for the Sys Admin
        Public MustOverride Function GetRecords() As DataTable

    End Class

End Namespace