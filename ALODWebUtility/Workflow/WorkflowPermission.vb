Namespace Worklfow

    Public Class WorkflowPermission

        Protected _canCreate As Boolean
        Protected _canView As Boolean
        Protected _groupId As Byte
        Protected _groupName As String
        Protected _workflowId As Byte

        Public Property CanCreate() As Boolean
            Get
                Return _canCreate
            End Get
            Set(ByVal value As Boolean)
                _canCreate = value
            End Set
        End Property

        Public Property CanView() As Boolean
            Get
                Return _canView
            End Get
            Set(ByVal value As Boolean)
                _canView = value
            End Set
        End Property

        Public Property GroupId() As Byte
            Get
                Return _groupId
            End Get
            Set(ByVal value As Byte)
                _groupId = value
            End Set
        End Property

        Public Property GroupName() As String
            Get
                Return _groupName
            End Get
            Set(ByVal value As String)
                _groupName = value
            End Set
        End Property

        Public Property WorkflowId() As Byte
            Get
                Return _workflowId
            End Get
            Set(ByVal value As Byte)
                _workflowId = value
            End Set
        End Property

    End Class

End Namespace