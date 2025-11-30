Imports System.Web
Imports ALOD.Data

Namespace Worklfow

    Public Class ActiveCaseList
        Implements ICollection(Of ActiveCase)

        Protected _list As New List(Of ActiveCase)

        Public ReadOnly Property Count() As Integer Implements System.Collections.Generic.ICollection(Of ActiveCase).Count
            Get
                Return _list.Count
            End Get
        End Property

        Public ReadOnly Property IsReadOnly() As Boolean Implements System.Collections.Generic.ICollection(Of ActiveCase).IsReadOnly
            Get
                Return False
            End Get
        End Property

        Public Sub Add(ByVal item As ActiveCase) Implements System.Collections.Generic.ICollection(Of ActiveCase).Add
            _list.Add(item)
        End Sub

        Public Sub Clear() Implements System.Collections.Generic.ICollection(Of ActiveCase).Clear
            _list.Clear()
        End Sub

        Public Function Contains(ByVal item As ActiveCase) As Boolean Implements System.Collections.Generic.ICollection(Of ActiveCase).Contains
            Return _list.Contains(item)
        End Function

        Public Sub CopyTo(ByVal array() As ActiveCase, ByVal arrayIndex As Integer) Implements System.Collections.Generic.ICollection(Of ActiveCase).CopyTo
            _list.CopyTo(array, arrayIndex)
        End Sub

        Public Function GetByRefId(ByVal refId As Integer) As ActiveCaseList
            Dim adapter As New SqlDataStore
            adapter.ExecuteReader(AddressOf ActiveReader, "core_workflow_sp_GetActiveCases", refId, CInt(HttpContext.Current.Session("GroupId")))
            Return Me
        End Function

        Public Function GetEnumerator() As System.Collections.Generic.IEnumerator(Of ActiveCase) Implements System.Collections.Generic.IEnumerable(Of ActiveCase).GetEnumerator
            Return _list.GetEnumerator()
        End Function

        Public Function GetEnumerator1() As System.Collections.IEnumerator Implements System.Collections.IEnumerable.GetEnumerator
            Return _list.GetEnumerator()
        End Function

        Public Function Remove(ByVal item As ActiveCase) As Boolean Implements System.Collections.Generic.ICollection(Of ActiveCase).Remove
            Return _list.Remove(item)
        End Function

        Protected Sub ActiveReader(ByVal adapter As SqlDataStore, ByVal reader As System.Data.IDataReader)

            '0-moduleId, 1-refId, 2-description, 3-title, 4-moduleName, 5-parentId

            Dim row As New ActiveCase
            row.Type = adapter.GetByte(reader, 0)
            row.RefId = adapter.GetInt32(reader, 1)
            row.Description = adapter.GetString(reader, 2)
            row.Title = adapter.GetString(reader, 3)
            row.ModuleTitle = adapter.GetString(reader, 4)
            row.ParentId = adapter.GetInt32(reader, 5)

            _list.Add(row)

        End Sub

    End Class

End Namespace