Namespace Worklfow

    Public Class SignStartedEventArgs
        Inherits EventArgs

        Private _cancel As Boolean = False
        Private _comments As String
        Private _dbSign As Byte
        Private _optionId As Short
        Private _refId As Integer
        Private _secondaryId As Integer
        Private _statusIn As Integer
        Private _statusOut As Integer

        Public Property Cancel() As Boolean
            Get
                Return _cancel
            End Get
            Set(ByVal value As Boolean)
                _cancel = value
            End Set
        End Property

        Public Property Comments() As String
            Get
                Return _comments
            End Get
            Set(ByVal value As String)
                _comments = value
            End Set
        End Property

        Public Property DBSign() As Byte
            Get
                Return _dbSign
            End Get
            Set(ByVal value As Byte)
                _dbSign = value
            End Set
        End Property

        Public Property OptionId() As Short
            Get
                Return _optionId
            End Get
            Set(ByVal value As Short)
                _optionId = value
            End Set
        End Property

        Public Property RefId() As Integer
            Get
                Return _refId
            End Get
            Set(ByVal value As Integer)
                _refId = value
            End Set
        End Property

        Public Property SecondaryId() As Integer
            Get
                Return _secondaryId
            End Get
            Set(ByVal value As Integer)
                _secondaryId = value
            End Set
        End Property

        Public Property StatusIn() As Integer
            Get
                Return _statusIn
            End Get
            Set(ByVal value As Integer)
                _statusIn = value
            End Set
        End Property

        Public Property StatusOut() As Integer
            Get
                Return _statusOut
            End Get
            Set(ByVal value As Integer)
                _statusOut = value
            End Set
        End Property

    End Class

End Namespace