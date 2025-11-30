Imports ALOD.Core.Domain.Workflow

Namespace TabNavigation

    <Serializable()>
    Public Class TabItem

        Dim _access As PageAccessType = PageAccessType.None
        Dim _active As Boolean = False
        Dim _completed As Boolean = False
        Dim _enabled As Boolean = False
        Dim _order As Short = 0
        Dim _page As String = String.Empty
        Dim _readOnly As Boolean = True
        Dim _required As Boolean = False
        Dim _script As String = String.Empty
        Dim _title As String = String.Empty
        Dim _visible As Boolean = True

        Public Sub New(ByVal title As String, ByVal order As Short, ByVal page As String)
            _title = title
            _order = order
            _page = page
        End Sub

        Public Sub New(ByVal title As String)
            _title = title
        End Sub

        Public Property Access() As PageAccessType
            Get
                Return _access
            End Get
            Set(ByVal value As PageAccessType)
                _access = value
            End Set
        End Property

        Public Property Active() As Boolean
            Get
                Return _active
            End Get
            Set(ByVal Value As Boolean)
                _active = Value
            End Set
        End Property

        Public Property ClientScript() As String
            Get
                Return _script
            End Get
            Set(ByVal value As String)
                _script = value
            End Set
        End Property

        Public Property Completed() As Boolean
            Get
                Return _completed
            End Get
            Set(ByVal Value As Boolean)
                _completed = Value
            End Set
        End Property

        Public Property Enabled() As Boolean
            Get
                Return _enabled
            End Get
            Set(ByVal Value As Boolean)
                _enabled = Value
            End Set
        End Property

        Public Property IsReadOnly() As Boolean
            Get
                Return _readOnly
            End Get
            Set(ByVal value As Boolean)
                _readOnly = value
            End Set
        End Property

        Public ReadOnly Property Order() As Short
            Get
                Return _order
            End Get
        End Property

        Public ReadOnly Property Page() As String
            Get
                Return _page
            End Get
        End Property

        Public Property Required() As Boolean
            Get
                Return _required
            End Get
            Set(ByVal value As Boolean)
                _required = value
            End Set
        End Property

        Public Property Title() As String
            Get
                Return _title
            End Get
            Set(value As String)
                _title = value
            End Set
        End Property

        Public Property Visible() As Boolean
            Get
                Return _visible
            End Get
            Set(ByVal value As Boolean)
                If (value = True AndAlso _access = PageAccessType.None) Then
                    Exit Property 'don't allow tabs with no access to become visible
                End If
                _visible = value
            End Set
        End Property

    End Class

End Namespace