Namespace TabNavigation

    <Serializable()>
    Public Class TabItemList
        Implements IList(Of TabItem)

        Protected _list As New List(Of TabItem)

        Public ReadOnly Property Count() As Integer Implements System.Collections.Generic.ICollection(Of TabItem).Count
            Get
                Return _list.Count
            End Get
        End Property

        Public ReadOnly Property IsReadOnly() As Boolean Implements System.Collections.Generic.ICollection(Of TabItem).IsReadOnly
            Get
                Return False
            End Get
        End Property

        Default Public Property Item(ByVal index As Integer) As TabItem Implements System.Collections.Generic.IList(Of TabItem).Item
            Get
                Return _list(index)
            End Get
            Set(ByVal value As TabItem)
                _list(index) = value
            End Set
        End Property

        Default Public Property Item(ByVal page As String) As TabItem
            Get
                For Each row As TabItem In _list
                    If (String.Equals(row.Page, page, StringComparison.InvariantCultureIgnoreCase)) Then
                        Return row
                    End If
                Next
                Return Nothing
            End Get
            Set(ByVal value As TabItem)
                For Each row As TabItem In _list
                    If (row.Page.CompareTo(page)) Then
                        row = value
                    End If
                Next
            End Set
        End Property

        Public Sub Add(ByVal item As TabItem) Implements System.Collections.Generic.ICollection(Of TabItem).Add
            _list.Add(item)
        End Sub

        Public Sub Clear() Implements System.Collections.Generic.ICollection(Of TabItem).Clear
            _list.Clear()
        End Sub

        Public Function Contains(ByVal item As TabItem) As Boolean Implements System.Collections.Generic.ICollection(Of TabItem).Contains
            Return _list.Contains(item)
        End Function

        Public Sub CopyTo(ByVal array() As TabItem, ByVal arrayIndex As Integer) Implements System.Collections.Generic.ICollection(Of TabItem).CopyTo
            _list.CopyTo(array, arrayIndex)
        End Sub

        Public Function GetEnumerator() As System.Collections.Generic.IEnumerator(Of TabItem) Implements System.Collections.Generic.IEnumerable(Of TabItem).GetEnumerator
            Return _list.GetEnumerator()
        End Function

        Public Function GetEnumerator1() As System.Collections.IEnumerator Implements System.Collections.IEnumerable.GetEnumerator
            Return _list.GetEnumerator()
        End Function

        Public Function IndexOf(ByVal item As TabItem) As Integer Implements System.Collections.Generic.IList(Of TabItem).IndexOf
            Return _list.IndexOf(item)
        End Function

        Public Sub Insert(ByVal index As Integer, ByVal item As TabItem) Implements System.Collections.Generic.IList(Of TabItem).Insert
            _list.Insert(index, item)
        End Sub

        Public Function Remove(ByVal item As TabItem) As Boolean Implements System.Collections.Generic.ICollection(Of TabItem).Remove
            Return _list.Remove(item)
        End Function

        Public Sub RemoveAt(ByVal index As Integer) Implements System.Collections.Generic.IList(Of TabItem).RemoveAt
            _list.RemoveAt(index)
        End Sub

    End Class

End Namespace