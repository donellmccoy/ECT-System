Namespace Common

    Public Class RadioButtonSelectedEventArgs
        Private _buttonText As String
        Private _buttonValue As String

        Sub New()
            _buttonValue = String.Empty
            _buttonText = String.Empty
        End Sub

        Public Property ButtonText As String
            Get
                Return _buttonText
            End Get
            Set(value As String)
                _buttonText = value
            End Set
        End Property

        Public Property ButtonValue As String
            Get
                Return _buttonValue
            End Get
            Set(value As String)
                _buttonValue = value
            End Set
        End Property

    End Class

End Namespace