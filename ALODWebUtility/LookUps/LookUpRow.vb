Namespace LookUps

    Public Class LookUpRow
        Protected _text As String
        Protected _value As String

        Public Sub New()

            _text = String.Empty
            _value = String.Empty
        End Sub

        Public Sub New(ByVal text As String, ByVal value As String)
            _text = text
            _value = value
        End Sub

        Public Property text() As String
            Get
                Return _text
            End Get
            Set(ByVal value As String)
                _text = value
            End Set
        End Property

        Public Property value() As String
            Get
                Return _value
            End Get
            Set(ByVal value As String)
                _value = value
            End Set
        End Property

    End Class

End Namespace