Imports ALOD.Data

Namespace Worklfow

    Public Class WorkStatusList
        Implements ICollection(Of WorkStatus)

        Protected _list As New List(Of WorkStatus)

        ''' <summary>
        ''' Returns all active WorkStatus codes for the given workflow
        ''' </summary>
        ''' <param name="workflow"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetByWorklfow(ByVal workflow As Integer) As ICollection(Of WorkStatus)
            Dim adapter As New SqlDataStore
            adapter.ExecuteReader(AddressOf WorkStatusReader, "core_workstatus_sp_GetByWorkflow", workflow)
            Return _list
        End Function

        Protected Sub WorkStatusReader(ByVal adapter As SqlDataStore, ByVal reader As System.Data.IDataReader)

            '0-ws_id, 1-workflowId, 2-statusId, 3-sortOrder
            '4-descr, 5-groupId, 6-groupName

            Dim status As New WorkStatus
            status.Id = adapter.GetInt32(reader, 0)
            status.Workflow = adapter.GetByte(reader, 1)
            status.Status = adapter.GetInt32(reader, 2)
            status.SortOrder = adapter.GetByte(reader, 3)
            status.Description = adapter.GetString(reader, 4)
            status.GroupId = adapter.GetByte(reader, 5)
            status.GroupName = adapter.GetString(reader, 6)

            _list.Add(status)

        End Sub

#Region "ICollection"

        Public ReadOnly Property Count() As Integer Implements System.Collections.Generic.ICollection(Of WorkStatus).Count
            Get
                Return _list.Count
            End Get
        End Property

        Public ReadOnly Property IsReadOnly() As Boolean Implements System.Collections.Generic.ICollection(Of WorkStatus).IsReadOnly
            Get
                Return False
            End Get
        End Property

        Public Sub Add(ByVal item As WorkStatus) Implements System.Collections.Generic.ICollection(Of WorkStatus).Add
            _list.Add(item)
        End Sub

        Public Sub Clear() Implements System.Collections.Generic.ICollection(Of WorkStatus).Clear
            _list.Clear()
        End Sub

        Public Function Contains(ByVal item As WorkStatus) As Boolean Implements System.Collections.Generic.ICollection(Of WorkStatus).Contains
            Return _list.Contains(item)
        End Function

        Public Sub CopyTo(ByVal array() As WorkStatus, ByVal arrayIndex As Integer) Implements System.Collections.Generic.ICollection(Of WorkStatus).CopyTo
            _list.CopyTo(array, arrayIndex)
        End Sub

        Public Function GetEnumerator() As System.Collections.Generic.IEnumerator(Of WorkStatus) Implements System.Collections.Generic.IEnumerable(Of WorkStatus).GetEnumerator
            Return _list.GetEnumerator
        End Function

        Public Function GetEnumerator1() As System.Collections.IEnumerator Implements System.Collections.IEnumerable.GetEnumerator
            Return _list.GetEnumerator
        End Function

        Public Function Remove(ByVal item As WorkStatus) As Boolean Implements System.Collections.Generic.ICollection(Of WorkStatus).Remove
            Return _list.Remove(item)
        End Function

#End Region

    End Class

End Namespace