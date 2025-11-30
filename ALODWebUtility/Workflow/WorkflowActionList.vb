Imports ALOD.Data

Namespace Worklfow

    <Serializable()>
    Public Class WorkflowActionList
        Implements IList(Of WorkflowAction)

        Protected _list As New List(Of WorkflowAction)
        Protected _stepId As Short

        Public Sub New()
            _stepId = 0
        End Sub

        Public Sub New(ByVal stepId As Short)
            _stepId = stepId
            GetActions()
        End Sub

        Public Property StepId() As Short
            Get
                Return _stepId
            End Get
            Set(ByVal value As Short)
                _stepId = value
            End Set
        End Property

        Public Shared Sub CopyAction(ByVal fromId As Integer, ByVal toId As Integer)
            Dim store As New SqlDataStore()
            store.ExecuteNonQuery("core_workflow_sp_CopyActions", fromId, toId)
        End Sub

        Public Sub DeleteAction(ByVal id As Short)
            Dim adapter As New SqlDataStore()
            Dim cmd As System.Data.Common.DbCommand = adapter.GetSqlStringCommand(
            "DELETE FROM core_WorkStatus_Actions WHERE wsa_id = @actionId")
            adapter.AddInParameter(cmd, "@actionId", Data.DbType.Int32, CType(id, Int32))
            adapter.ExecuteNonQuery(cmd)
        End Sub

        Public Function GetActions(ByVal stepId As Integer) As WorkflowActionList
            _stepId = stepId
            Return GetActions()
        End Function

        Public Function GetActions() As WorkflowActionList
            _list.Clear()
            Dim adapter As New SqlDataStore()
            adapter.ExecuteReader(AddressOf ActionReader, "core_workflow_sp_GetActionsByStep", _stepId)
            Return Me
        End Function

        Protected Sub ActionReader(ByVal adapter As SqlDataStore, ByVal reader As System.Data.IDataReader)

            '0-stepId, 1-actionId, 2-type, 3-target, 4-data, 5-text
            Dim action As New WorkflowAction

            action.StepId = _stepId
            action.Id = CType(adapter.GetInteger(reader, 1), Int16)
            action.Type = adapter.GetByte(reader, 2)
            action.Target = adapter.GetInteger(reader, 3)
            action.Data = adapter.GetInteger(reader, 4)
            action.Text = adapter.GetString(reader, 5)

            _list.Add(action)

        End Sub

#Region "IList"

        Public ReadOnly Property Count() As Integer Implements System.Collections.Generic.ICollection(Of WorkflowAction).Count
            Get
                Return _list.Count
            End Get
        End Property

        Public ReadOnly Property IsReadOnly() As Boolean Implements System.Collections.Generic.ICollection(Of WorkflowAction).IsReadOnly
            Get
                Return False
            End Get
        End Property

        Default Public Property Item(ByVal index As Integer) As WorkflowAction Implements System.Collections.Generic.IList(Of WorkflowAction).Item
            Get
                Return _list(index)
            End Get
            Set(ByVal value As WorkflowAction)
                _list(index) = value
            End Set
        End Property

        Public Sub Add(ByVal item As WorkflowAction) Implements System.Collections.Generic.ICollection(Of WorkflowAction).Add
            _list.Add(item)
        End Sub

        Public Sub Clear() Implements System.Collections.Generic.ICollection(Of WorkflowAction).Clear
            _list.Clear()
        End Sub

        Public Function Contains(ByVal item As WorkflowAction) As Boolean Implements System.Collections.Generic.ICollection(Of WorkflowAction).Contains
            Return _list.Contains(item)
        End Function

        Public Sub CopyTo(ByVal array() As WorkflowAction, ByVal arrayIndex As Integer) Implements System.Collections.Generic.ICollection(Of WorkflowAction).CopyTo
            _list.CopyTo(array)
        End Sub

        Public Function GetEnumerator() As System.Collections.Generic.IEnumerator(Of WorkflowAction) Implements System.Collections.Generic.IEnumerable(Of WorkflowAction).GetEnumerator
            Return _list.GetEnumerator
        End Function

        Public Function GetEnumerator1() As System.Collections.IEnumerator Implements System.Collections.IEnumerable.GetEnumerator
            Return _list.GetEnumerator()
        End Function

        Public Function IndexOf(ByVal item As WorkflowAction) As Integer Implements System.Collections.Generic.IList(Of WorkflowAction).IndexOf
            Return _list.IndexOf(item)
        End Function

        Public Sub Insert(ByVal index As Integer, ByVal item As WorkflowAction) Implements System.Collections.Generic.IList(Of WorkflowAction).Insert
            _list.Insert(index, item)
        End Sub

        Public Function Remove(ByVal item As WorkflowAction) As Boolean Implements System.Collections.Generic.ICollection(Of WorkflowAction).Remove
            Return _list.Remove(item)
        End Function

        Public Sub RemoveAt(ByVal index As Integer) Implements System.Collections.Generic.IList(Of WorkflowAction).RemoveAt
            _list.RemoveAt(index)
        End Sub

#End Region

    End Class

End Namespace