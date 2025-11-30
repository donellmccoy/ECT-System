Namespace Worklfow

    Public Class SignCompletedEventArgs
        Inherits EventArgs

        Private _comments As String
        Private _optionId As Integer
        Private _redirect As Boolean = True
        Private _refId As Integer
        Private _statusIn As Integer
        Private _statusOut As Integer
        Private _successful As Boolean = False
        Private _text As String
        Private _url As String = ""

        Public Property Comments() As String
            Get
                Return _comments
            End Get
            Set(ByVal value As String)
                _comments = value
            End Set
        End Property

        Public Property OptionId() As Integer
            Get
                Return _optionId
            End Get
            Set(ByVal value As Integer)
                _optionId = value
            End Set
        End Property

        Public Property Redirect() As Boolean
            Get
                Return _redirect
            End Get
            Set(ByVal value As Boolean)
                _redirect = value
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

        Public Property SignaturePassed() As Boolean
            Get
                Return _successful
            End Get
            Set(ByVal value As Boolean)
                _successful = value
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

        Public Property Text() As String
            Get
                Return _text
            End Get
            Set(ByVal value As String)
                _text = value
            End Set
        End Property

        Public Property Url() As String
            Get
                Return _url
            End Get
            Set(ByVal value As String)
                _url = value
            End Set
        End Property

    End Class

End Namespace