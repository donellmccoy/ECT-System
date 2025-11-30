Namespace Worklfow

    Public Class WorkStatusLinqExt

        Private _optionValid As Boolean

        Private _optionVisible As Boolean

        Public Property OptionValid() As Boolean
            Get
                Return _optionValid
            End Get
            Set(ByVal value As Boolean)
                _optionValid = value
            End Set
        End Property

        Public Property OptionVisible() As Boolean
            Get
                Return _optionVisible
            End Get
            Set(ByVal value As Boolean)
                _optionVisible = value
            End Set
        End Property

    End Class

End Namespace