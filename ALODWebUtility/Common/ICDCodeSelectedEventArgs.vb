Namespace Common

    Public Class ICDCodeSelectedEventArgs
        Inherits System.EventArgs

        Private _selectedDropDownLevel As Integer
        Private _selectedICD7thCharacter As String
        Private _selectedICDCodeId As Integer

        Public Property SelectedDropDownLevel() As Integer
            Get
                Return _selectedDropDownLevel
            End Get
            Set(value As Integer)
                _selectedDropDownLevel = value
            End Set
        End Property

        Public Property SelectedICD7thCharacter() As String
            Get
                Return _selectedICD7thCharacter
            End Get
            Set(value As String)
                _selectedICD7thCharacter = value
            End Set
        End Property

        Public Property SelectedICDCodeId() As Integer
            Get
                Return _selectedICDCodeId
            End Get
            Set(value As Integer)
                _selectedICDCodeId = value
            End Set
        End Property

    End Class

End Namespace