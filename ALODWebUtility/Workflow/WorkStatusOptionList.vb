Imports ALOD.Data

Namespace Worklfow

    Public Class WorkStatusOptionList
        Implements ICollection(Of WorkStatusOption)

        Protected _list As New List(Of WorkStatusOption)

        Public Function GetByWorkflow(ByVal workflow As Byte, compo As Integer) As ICollection(Of WorkStatusOption)
            Dim adapter As New SqlDataStore
            adapter.ExecuteReader(AddressOf OptionReader, "core_workstatus_sp_GetOptionsByWorkflow", workflow, compo)
            Return _list
        End Function

        Public Function GetByWorkflowAll(ByVal workflow As Byte) As ICollection(Of WorkStatusOption)
            Dim adapter As New SqlDataStore
            adapter.ExecuteReader(AddressOf OptionReader, "core_workstatus_sp_GetOptionsByWorkflowAll", workflow)
            Return _list
        End Function

        Public Function GetByWorkStatus(ByVal workStatus As Integer, compo As Integer) As ICollection(Of WorkStatusOption)
            Dim adapter As New SqlDataStore
            adapter.ExecuteReader(AddressOf OptionReader, "core_workstatus_sp_GetOptionsByWorkStatus", workStatus, compo)
            Return _list
        End Function

        Protected Sub OptionReader(ByVal adapter As SqlDataStore, ByVal reader As System.Data.IDataReader)

            '0-wso_id, 1-ws_id, 2-statusId, 3-displayText, 4-active, 5-groupId, 6-name, 7-sortOrder, 8-template

            Dim opt As New WorkStatusOption
            opt.Id = adapter.GetInteger(reader, 0)
            opt.WorkStatusId = adapter.GetInteger(reader, 1)
            opt.StatusOut = adapter.GetInteger(reader, 2)
            opt.Text = adapter.GetString(reader, 3)
            opt.Active = adapter.GetBoolean(reader, 4)
            opt.GroupId = adapter.GetByte(reader, 5)
            opt.GroupName = adapter.GetString(reader, 6)
            opt.SortOrder = adapter.GetByte(reader, 7)
            opt.DBSignTemplate = adapter.GetByte(reader, 8)
            opt.Valid = True
            opt.OptionVisible = True
            opt.StatusOutText = adapter.GetString(reader, 9)
            opt.Compo = adapter.GetInteger(reader, 10)

            _list.Add(opt)

        End Sub

#Region "ICollection"

        Public ReadOnly Property Count() As Integer Implements System.Collections.Generic.ICollection(Of WorkStatusOption).Count
            Get
                Return _list.Count
            End Get
        End Property

        Public ReadOnly Property IsReadOnly() As Boolean Implements System.Collections.Generic.ICollection(Of WorkStatusOption).IsReadOnly
            Get
                Return False
            End Get
        End Property

        Public Sub Add(ByVal item As WorkStatusOption) Implements System.Collections.Generic.ICollection(Of WorkStatusOption).Add
            _list.Add(item)
        End Sub

        Public Sub Clear() Implements System.Collections.Generic.ICollection(Of WorkStatusOption).Clear
            _list.Clear()
        End Sub

        Public Function Contains(ByVal item As WorkStatusOption) As Boolean Implements System.Collections.Generic.ICollection(Of WorkStatusOption).Contains
            Return _list.Contains(item)
        End Function

        Public Sub CopyTo(ByVal array() As WorkStatusOption, ByVal arrayIndex As Integer) Implements System.Collections.Generic.ICollection(Of WorkStatusOption).CopyTo
            _list.CopyTo(array, arrayIndex)
        End Sub

        Public Function GetEnumerator() As System.Collections.Generic.IEnumerator(Of WorkStatusOption) Implements System.Collections.Generic.IEnumerable(Of WorkStatusOption).GetEnumerator
            Return _list.GetEnumerator()
        End Function

        Public Function GetEnumerator1() As System.Collections.IEnumerator Implements System.Collections.IEnumerable.GetEnumerator
            Return _list.GetEnumerator()
        End Function

        Public Function Remove(ByVal item As WorkStatusOption) As Boolean Implements System.Collections.Generic.ICollection(Of WorkStatusOption).Remove
            Return _list.Remove(item)
        End Function

#End Region

    End Class

End Namespace