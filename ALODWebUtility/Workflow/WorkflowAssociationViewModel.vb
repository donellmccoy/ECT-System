Namespace Worklfow

    Public Class WorkflowAssociationViewModel
        Private _associateId As Integer
        Private _isAssociated As Boolean
        Private _workflowId As Integer
        Private _workflowTitle As String

        Public Sub New()
            AssociateId = 0
            WorkflowId = 0
            WorkflowTitle = String.Empty
            IsAssociated = False
        End Sub

        Public Sub New(ByVal associateId As Integer, ByVal workflowId As Integer, ByVal workflowTitle As String, ByVal isAssociated As Boolean)
            _associateId = associateId
            _workflowId = workflowId
            _workflowTitle = workflowTitle
            _isAssociated = isAssociated
        End Sub

        Public Property AssociateId As Integer
            Get
                Return _associateId
            End Get
            Set(value As Integer)
                _associateId = value
            End Set
        End Property

        Public Property IsAssociated As Boolean
            Get
                Return _isAssociated
            End Get
            Set(value As Boolean)
                _isAssociated = value
            End Set
        End Property

        Public Property WorkflowId As Integer
            Get
                Return _workflowId
            End Get
            Set(value As Integer)
                _workflowId = value
            End Set
        End Property

        Public Property WorkflowTitle As String
            Get
                Return _workflowTitle
            End Get
            Set(value As String)
                _workflowTitle = value
            End Set
        End Property

    End Class

End Namespace