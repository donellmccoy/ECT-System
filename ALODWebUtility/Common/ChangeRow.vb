Namespace Common

    <Serializable()>
    Public Class ChangeRow

        Protected _actionDate As DateTime
        Protected _actionId As Integer
        Protected _actionName As String
        Protected _field As String
        Protected _id As Integer
        Protected _newVal As String
        Protected _oldVal As String
        Protected _section As String
        Protected _userId As Integer
        Protected _userName As String

        Public Sub New()
            _id = 0
            _section = String.Empty
            _field = String.Empty
            _oldVal = String.Empty
            _newVal = String.Empty
        End Sub

        Public Sub New(ByVal id As Integer, ByVal section As String, ByVal field As String, ByVal oldVal As String, ByVal newVal As String)
            _id = id
            _section = section
            _field = field
            _oldVal = oldVal
            _newVal = newVal
        End Sub

        Public Sub New(ByVal section As String, ByVal field As String, ByVal oldVal As String, ByVal newVal As String)
            _id = Id
            _section = section
            _field = field
            _oldVal = oldVal
            _newVal = newVal
        End Sub

        Public Property ActionDate() As DateTime
            Get
                Return _actionDate
            End Get
            Set(ByVal value As DateTime)
                _actionDate = value
            End Set
        End Property

        Public Property ActionId() As Integer
            Get
                Return _actionId
            End Get
            Set(ByVal value As Integer)
                _actionId = value
            End Set
        End Property

        Public Property ActionName() As String
            Get
                Return _actionName
            End Get
            Set(ByVal value As String)
                _actionName = value
            End Set
        End Property

        Public Property Field() As String
            Get
                Return _field
            End Get
            Set(ByVal value As String)
                _field = value
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

        Public Property NewVal() As String
            Get
                Return _newVal
            End Get
            Set(ByVal value As String)
                _newVal = value
            End Set
        End Property

        Public Property OldVal() As String
            Get
                Return _oldVal
            End Get
            Set(ByVal value As String)
                _oldVal = value
            End Set
        End Property

        Public Property Section() As String
            Get
                Return _section
            End Get
            Set(ByVal value As String)
                _section = value
            End Set
        End Property

        Public Property UserId() As Integer
            Get
                Return _userId
            End Get
            Set(ByVal value As Integer)
                _userId = value
            End Set
        End Property

        Public Property UserName() As String
            Get
                Return _userName
            End Get
            Set(ByVal value As String)
                _userName = value
            End Set
        End Property

    End Class

End Namespace