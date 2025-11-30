Namespace Common

    Public Class MemberSelectedEventArgs
        Inherits System.EventArgs

        Private _selectedRowIndex As Integer

        Public Property SelectedRowIndex() As Integer
            Get
                Return _selectedRowIndex
            End Get
            Set(value As Integer)
                _selectedRowIndex = value
            End Set
        End Property

    End Class

End Namespace