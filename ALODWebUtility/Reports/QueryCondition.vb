Namespace Reports

    Public Class QueryCondition

        Private _filter As String = ""
        Private _name As String = ""
        Private _type As DbType
        Private _value As Object

        Public Sub New(ByVal name As String, ByVal value As Object, ByVal valueType As DbType, ByVal filter As String)
            _name = name
            _type = valueType
            _value = value
            _filter = filter
        End Sub

        Public Sub New(ByVal name As String, ByVal filter As String)
            _name = name
            _filter = filter
        End Sub

        Public Property Field() As String
            Get
                Return _name
            End Get
            Set(ByVal value As String)
                _name = value
            End Set
        End Property

        Public Property Filter() As String
            Get
                Return _filter
            End Get
            Set(ByVal value As String)
                _filter = value
            End Set
        End Property

        Public Property Name() As String
            Get
                Return "@" + _name
            End Get
            Set(ByVal value As String)
                _name = value
            End Set
        End Property

        Public Property Value() As Object
            Get
                Return _value
            End Get
            Set(ByVal value As Object)
                _value = value
            End Set
        End Property

        Public Property ValueType() As DbType
            Get
                Return _type
            End Get
            Set(ByVal value As DbType)
                _type = value
            End Set
        End Property

    End Class

End Namespace