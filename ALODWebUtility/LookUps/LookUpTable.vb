Imports System.Text
Imports System.Web.UI.WebControls
Imports ALOD.Data

Namespace LookUps

    Public Class LookUpTable
        Implements IList(Of LookUpRow)

        Protected _adapter As SqlDataStore
        Protected _rows As List(Of LookUpRow)
        Protected cacheSource As System.Web.Caching.Cache

        Public Sub New()
            _rows = New List(Of LookUpRow)
        End Sub

        Public Property rows() As List(Of LookUpRow)
            Get
                Return _rows
            End Get
            Set(ByVal value As List(Of LookUpRow))
                _rows = value
            End Set
        End Property

        Protected ReadOnly Property Cache() As System.Web.Caching.Cache
            Get
                Return System.Web.HttpContext.Current.Cache
            End Get
        End Property

        Protected ReadOnly Property DataStore() As SqlDataStore
            Get
                If (_adapter Is Nothing) Then
                    _adapter = New SqlDataStore()
                End If
                Return _adapter
            End Get
        End Property

        Public Sub AddRow(ByVal value As String, ByVal text As String)

            Dim row As New LookUpRow()
            row.value = value
            row.text = text
            _rows.Add(row)

        End Sub

        Public Function Find(ByVal value As String) As LookUpRow

            For Each row As LookUpRow In _rows
                If (row.value = value) Then
                    Return row
                End If
            Next

            Return Nothing

        End Function

        Public Sub LookUpRowReader(ByVal adapter As SqlDataStore, ByVal reader As IDataReader)

            Dim row As New LookUpRow()
            If (Not reader.IsDBNull(0)) Then
                row.value = reader(0).ToString()
            Else
                row.value = String.Empty
            End If

            If (Not reader.IsDBNull(1)) Then
                row.text = reader(1).ToString()
            Else
                row.text = String.Empty
            End If
            _rows.Add(row)

        End Sub

#Region "IList"

        Public ReadOnly Property Count() As Integer Implements System.Collections.Generic.ICollection(Of LookUpRow).Count
            Get
                Return _rows.Count
            End Get
        End Property

        Public ReadOnly Property IsReadOnly() As Boolean Implements System.Collections.Generic.ICollection(Of LookUpRow).IsReadOnly
            Get
                Return False
            End Get
        End Property

        Default Public Property Item(ByVal index As Integer) As LookUpRow Implements System.Collections.Generic.IList(Of LookUpRow).Item
            Get
                Return _rows(index)
            End Get
            Set(ByVal value As LookUpRow)
                _rows(index) = value
            End Set
        End Property

        Public Sub Add(ByVal item As LookUpRow) Implements System.Collections.Generic.ICollection(Of LookUpRow).Add
            _rows.Add(item)
        End Sub

        Public Sub Clear() Implements System.Collections.Generic.ICollection(Of LookUpRow).Clear
            _rows.Clear()
        End Sub

        Public Function Contains(ByVal item As LookUpRow) As Boolean Implements System.Collections.Generic.ICollection(Of LookUpRow).Contains
            Return _rows.Contains(item)
        End Function

        Public Sub CopyTo(ByVal array() As LookUpRow, ByVal arrayIndex As Integer) Implements System.Collections.Generic.ICollection(Of LookUpRow).CopyTo
            _rows.CopyTo(array, arrayIndex)
        End Sub

        Public Function GetEnumerator() As System.Collections.Generic.IEnumerator(Of LookUpRow) Implements System.Collections.Generic.IEnumerable(Of LookUpRow).GetEnumerator
            Return _rows.GetEnumerator()
        End Function

        Public Function GetEnumerator1() As System.Collections.IEnumerator Implements System.Collections.IEnumerable.GetEnumerator
            Return _rows.GetEnumerator()
        End Function

        Public Function IndexOf(ByVal item As LookUpRow) As Integer Implements System.Collections.Generic.IList(Of LookUpRow).IndexOf
            Return _rows.IndexOf(item)
        End Function

        Public Sub Insert(ByVal index As Integer, ByVal item As LookUpRow) Implements System.Collections.Generic.IList(Of LookUpRow).Insert
            _rows.Insert(index, item)
        End Sub

        Public Function Remove(ByVal item As LookUpRow) As Boolean Implements System.Collections.Generic.ICollection(Of LookUpRow).Remove
            Return _rows.Remove(item)
        End Function

        Public Sub RemoveAt(ByVal index As Integer) Implements System.Collections.Generic.IList(Of LookUpRow).RemoveAt
            _rows.RemoveAt(index)
        End Sub

#End Region

#Region "Helper"

        Public Function GetList() As String

            Dim buffer As New StringBuilder
            If _rows.Count < 1 Then Return ""
            For Each row As LookUpRow In _rows
                buffer.Append(Trim(row.value))
                buffer.Append(";")
            Next
            If (buffer.Length > 0) Then
                buffer = buffer.Remove(buffer.Length - 1, 1)
            End If

            Return buffer.ToString()

        End Function

        Public Sub SetList(ByVal strVals As String)

            _rows = New List(Of LookUpRow)
            If (strVals.Length <> 0) Then
                Dim strItems() As String = strVals.Split(";")
                Dim i As Integer
                For i = 0 To strItems.Length - 1
                    Dim row As New LookUpRow()
                    row.value = strItems(i)
                    row.text = ""
                    _rows.Add(row)

                Next

            End If

        End Sub

        Public Sub SetList(ByVal lst As ListBox)

            _rows = New List(Of LookUpRow)
            Dim i As Integer
            For i = 0 To lst.Items.Count() - 1

                Dim row As New LookUpRow()
                row.value = lst.Items(i).Value
                row.text = lst.Items(i).Text
                _rows.Add(row)

            Next

        End Sub

#End Region

    End Class

End Namespace