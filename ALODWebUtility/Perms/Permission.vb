Namespace Permission

    <Serializable()>
    Public Class Permission

        Protected _allowed As Boolean
        Protected _description As String
        Protected _exclude As Boolean = False
        Protected _id As Integer
        Protected _name As String
        Protected _status As String

        Public Sub New()
            _id = 0
            _name = String.Empty
            _description = String.Empty
        End Sub

        Public Sub New(ByVal id As Integer, ByVal name As String, ByVal description As String)
            _id = id
            _name = name
            _description = description
            _allowed = True
        End Sub

        Public Sub New(ByVal id As Integer, ByVal name As String, ByVal description As String, ByVal allowed As Boolean)
            _id = id
            _name = name
            _description = description
            _allowed = allowed
        End Sub

        Public Property Allowed() As Boolean
            Get
                Return _allowed
            End Get
            Set(ByVal value As Boolean)
                _allowed = value
            End Set
        End Property

        Public Property Description() As String
            Get
                Return _description
            End Get
            Set(ByVal value As String)
                _description = value
            End Set
        End Property

        Public Property Exclude() As Boolean
            Get
                Return _exclude
            End Get
            Set(ByVal value As Boolean)
                _exclude = value
            End Set
        End Property

        Public Property Id() As Integer
            Get
                Return _id
            End Get
            Set(ByVal value As Integer)
                _id = value
            End Set
        End Property

        Public Property Name() As String
            Get
                Return _name
            End Get
            Set(ByVal value As String)
                _name = value
            End Set
        End Property

        Public Property Status() As String
            Get
                Return _status
            End Get
            Set(ByVal value As String)
                _status = value
            End Set
        End Property

        Public Sub ToDataRow(ByVal row As DataSets.PermissionRow)

            row.id = _id
            row.name = _name
            row.description = _description
            row.allowed = _allowed
            row.exclude = _exclude

        End Sub

    End Class

End Namespace