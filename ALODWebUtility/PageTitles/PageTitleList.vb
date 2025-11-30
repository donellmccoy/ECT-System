Imports ALOD.Data

Namespace PageTitles

    Public Class PageTitleList
        Implements IList(Of PageTitle)

        Dim _pages As New List(Of PageTitle)

        Public Function GetAllPageTitles() As PageTitleList

            Dim adapter As New SqlDataStore()
            adapter.ExecuteReader(AddressOf CodeReader, "core_pageTitles_sp_GetAllPages")
            Return Me

        End Function

        Public Function GetPagesAsDataSet() As DataSets.PageTitlesDataTable
            Return GetAllPageTitles().ToDataSet()
        End Function

        Public Function ToDataSet() As DataSets.PageTitlesDataTable

            Dim data As New DataSets.PageTitlesDataTable
            Dim row As DataSets.PageTitlesRow

            For Each page As PageTitle In _pages
                row = data.NewPageTitlesRow()
                page.ToDataRow(row)
                data.Rows.Add(row)
            Next

            Return data

        End Function

        Public Sub UpdateByPageId(ByVal pageID As Integer, ByVal title As String)
            Dim adapter As New SqlDataStore()
            adapter.ExecuteNonQuery("core_pageTitles_sp_UpdateByPageId", pageID, title)
        End Sub

        Protected Sub CodeReader(ByVal adapter As SqlDataStore, ByVal reader As System.Data.IDataReader)

            Dim pages As New PageTitle

            pages.PageId = adapter.GetInt16(reader, 0)
            pages.Title = adapter.GetString(reader, 1)

            _pages.Add(pages)

        End Sub

#Region "Ilist"

        Public ReadOnly Property Count() As Integer Implements System.Collections.Generic.ICollection(Of PageTitle).Count
            Get
                Return _pages.Count
            End Get
        End Property

        Public ReadOnly Property IsReadOnly() As Boolean Implements System.Collections.Generic.ICollection(Of PageTitle).IsReadOnly
            Get
                Return False
            End Get
        End Property

        Default Public Property Item(ByVal index As Integer) As PageTitle Implements System.Collections.Generic.IList(Of PageTitle).Item
            Get
                Return _pages(index)
            End Get
            Set(ByVal value As PageTitle)
                _pages(index) = value
            End Set
        End Property

        Public Sub Add(ByVal item As PageTitle) Implements System.Collections.Generic.ICollection(Of PageTitle).Add
            _pages.Add(item)
        End Sub

        Public Sub Clear() Implements System.Collections.Generic.ICollection(Of PageTitle).Clear
            _pages.Clear()
        End Sub

        Public Function Contains(ByVal item As PageTitle) As Boolean Implements System.Collections.Generic.ICollection(Of PageTitle).Contains
            Return _pages.Contains(item)
        End Function

        Public Sub CopyTo(ByVal array() As PageTitle, ByVal arrayIndex As Integer) Implements System.Collections.Generic.ICollection(Of PageTitle).CopyTo
            _pages.CopyTo(array, arrayIndex)
        End Sub

        Public Function GetEnumerator() As System.Collections.Generic.IEnumerator(Of PageTitle) Implements System.Collections.Generic.IEnumerable(Of PageTitle).GetEnumerator
            Return _pages.GetEnumerator()
        End Function

        Public Function GetEnumerator1() As System.Collections.IEnumerator Implements System.Collections.IEnumerable.GetEnumerator
            Return _pages.GetEnumerator()
        End Function

        Public Function IndexOf(ByVal item As PageTitle) As Integer Implements System.Collections.Generic.IList(Of PageTitle).IndexOf
            Return _pages.IndexOf(item)
        End Function

        Public Sub Insert(ByVal index As Integer, ByVal item As PageTitle) Implements System.Collections.Generic.IList(Of PageTitle).Insert
            _pages.Insert(index, item)
        End Sub

        Public Function Remove(ByVal item As PageTitle) As Boolean Implements System.Collections.Generic.ICollection(Of PageTitle).Remove
            Return _pages.Remove(item)
        End Function

        Public Sub RemoveAt(ByVal index As Integer) Implements System.Collections.Generic.IList(Of PageTitle).RemoveAt
            _pages.RemoveAt(index)
        End Sub

#End Region

    End Class

End Namespace