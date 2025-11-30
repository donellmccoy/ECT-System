Imports System.Web
Imports ALOD.Data

Namespace Common

    <Serializable()>
    Public Class childUnitsList
        Implements IList(Of childunit)
        Protected _adapter As SqlDataStore
        Protected _childUnits As List(Of childunit)

        Public Sub New()
            _childUnits = New List(Of childunit)
            _childUnits.Clear()
        End Sub

        Protected ReadOnly Property DataStore() As SqlDataStore
            Get
                If (_adapter Is Nothing) Then
                    _adapter = New SqlDataStore()
                End If

                Return _adapter
            End Get
        End Property

        Public Function Read(ByVal unit As Integer, ByVal chainType As String, ByVal userUnit As Integer) As childUnitsList
            Dim userId As Integer = CInt(HttpContext.Current.Session("UserId"))
            DataStore.ExecuteReader(AddressOf unitReader, "core_pascodes_GetChildUnits", unit, chainType, userUnit, userId)
            Return Me
        End Function

        Private Sub unitReader(ByVal adapter As SqlDataStore, ByVal reader As System.Data.IDataReader)

            Dim _newunit As New childunit()

            _newunit.cs_id = adapter.GetInt32(reader, 0, -1)
            _newunit.childName = adapter.GetString(reader, 1)
            _newunit.childPasCode = adapter.GetString(reader, 2)

            _newunit.parentCS_ID = adapter.GetInt32(reader, 3, -1)
            _newunit.CHAIN_TYPE = adapter.GetString(reader, 4)
            _newunit.Level = adapter.GetInt32(reader, 5, -1)
            _newunit.userUnit = adapter.GetInt32(reader, 6, -1)
            _newunit.InActive = adapter.GetBoolean(reader, 7, 0)
            _childUnits.Add(_newunit)

        End Sub

#Region "IList"

        Public ReadOnly Property Count() As Integer Implements System.Collections.Generic.ICollection(Of childunit).Count
            Get
                Return _childUnits.Count
            End Get
        End Property

        Public ReadOnly Property IsReadOnly() As Boolean Implements System.Collections.Generic.ICollection(Of childunit).IsReadOnly
            Get
                Return False
            End Get
        End Property

        Default Public Property Item(ByVal index As Integer) As childunit Implements System.Collections.Generic.IList(Of childunit).Item
            Get
                Return _childUnits(index)
            End Get
            Set(ByVal value As childunit)
                _childUnits(index) = value
            End Set
        End Property

        Public Sub Add(ByVal item As childunit) Implements System.Collections.Generic.ICollection(Of childunit).Add
            _childUnits.Add(item)
        End Sub

        Public Sub Clear() Implements System.Collections.Generic.ICollection(Of childunit).Clear
            _childUnits.Clear()
        End Sub

        Public Function Contains(ByVal item As childunit) As Boolean Implements System.Collections.Generic.ICollection(Of childunit).Contains
            Return _childUnits.Contains(item)
        End Function

        Public Sub CopyTo(ByVal array() As childunit, ByVal arrayIndex As Integer) Implements System.Collections.Generic.ICollection(Of childunit).CopyTo
            _childUnits.CopyTo(array, arrayIndex)
        End Sub

        Public Function GetEnumerator() As System.Collections.Generic.IEnumerator(Of childunit) Implements System.Collections.Generic.IEnumerable(Of childunit).GetEnumerator
            Return _childUnits.GetEnumerator()
        End Function

        Public Function GetEnumerator1() As System.Collections.IEnumerator Implements System.Collections.IEnumerable.GetEnumerator
            Return _childUnits.GetEnumerator()
        End Function

        Public Function IndexOf(ByVal item As childunit) As Integer Implements System.Collections.Generic.IList(Of childunit).IndexOf
            Return _childUnits.IndexOf(item)
        End Function

        Public Sub Insert(ByVal index As Integer, ByVal item As childunit) Implements System.Collections.Generic.IList(Of childunit).Insert
            _childUnits.Insert(index, item)
        End Sub

        Public Function Remove(ByVal item As childunit) As Boolean Implements System.Collections.Generic.ICollection(Of childunit).Remove
            Return _childUnits.Remove(item)
        End Function

        Public Sub RemoveAt(ByVal index As Integer) Implements System.Collections.Generic.IList(Of childunit).RemoveAt
            _childUnits.RemoveAt(index)
        End Sub

#End Region

    End Class

End Namespace