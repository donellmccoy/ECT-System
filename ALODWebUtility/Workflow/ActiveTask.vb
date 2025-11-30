Imports ALOD.Core.Domain.Workflow

Namespace Worklfow

    Public Class ActiveTask

        Private _formalCount As Integer
        Private _informalCount As Integer
        Private _module As ModuleType
        Private _overdueCount As Integer
        Private _regionId As Integer
        Private _statusId As Integer
        Private _title As String
        Private _workflowId As Integer

        Public Sub New()
            _workflowId = 0
            _module = ModuleType.LOD
            _title = String.Empty
            _regionId = 0
            _overdueCount = 0
            _formalCount = 0
            _informalCount = 0
            _statusId = 0
        End Sub

        Public Property ActiveTitle() As String
            Get
                Return _title
            End Get
            Set(ByVal value As String)
                _title = value
            End Set
        End Property

        Public Property FormalCount() As Integer
            Get
                Return _formalCount
            End Get
            Set(ByVal value As Integer)
                _formalCount = value
            End Set
        End Property

        Public Property InformalCount() As Integer
            Get
                Return _informalCount
            End Get
            Set(ByVal value As Integer)
                _informalCount = value
            End Set

        End Property

        Public Property ModuleStr() As ModuleType
            Get
                Return _module
            End Get
            Set(ByVal value As ModuleType)
                _module = value
            End Set
        End Property

        Public Property OverdueCount() As Integer
            Get
                Return _overdueCount
            End Get
            Set(ByVal value As Integer)
                _overdueCount = value
            End Set
        End Property

        Public Property RegionId() As Integer
            Get
                Return _regionId
            End Get
            Set(ByVal value As Integer)
                _regionId = value
            End Set
        End Property

        Public Property StatusID() As Integer
            Get
                Return _statusId
            End Get
            Set(ByVal value As Integer)
                _statusId = value
            End Set
        End Property

        Public Property WorkflowId() As Integer
            Get
                Return _workflowId
            End Get
            Set(ByVal value As Integer)
                _workflowId = value
            End Set
        End Property

    End Class

End Namespace