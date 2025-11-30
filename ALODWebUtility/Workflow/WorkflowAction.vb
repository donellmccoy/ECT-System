Imports ALOD.Core.Domain.Workflow
Imports ALOD.Data

Namespace Worklfow

    <Serializable()>
    Public Class WorkflowAction
        Protected _data As Integer
        Protected _id As Short
        Protected _optionId As Integer
        Protected _stepId As Short
        Protected _target As Integer
        Protected _text As String
        Protected _type As WorkflowActionType

        Public Property Data() As Integer
            Get
                Return _data
            End Get
            Set(ByVal value As Integer)
                _data = value
            End Set
        End Property

        Public Property Id() As Short
            Get
                Return _id
            End Get
            Set(ByVal value As Short)
                _id = value
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

        Public Property StepId() As Short
            Get
                Return _stepId
            End Get
            Set(ByVal value As Short)
                _stepId = value
            End Set
        End Property

        Public Property Target() As Integer
            Get
                Return _target
            End Get
            Set(ByVal value As Integer)
                _target = value
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

        Public Property Type() As WorkflowActionType
            Get
                Return _type
            End Get
            Set(ByVal value As WorkflowActionType)
                _type = value
            End Set
        End Property

        Public Sub Insert()
            Dim adapter As New SqlDataStore()
            adapter.ExecuteNonQuery("core_workflow_sp_InsertOptionAction", _type, _optionId, _target, _data)
        End Sub

    End Class

End Namespace