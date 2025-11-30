Imports ALOD.Data
Imports ALODWebUtility.Common

Namespace Permission

    Public Class PermRequestList
        Implements IList(Of PermRequest)

        Protected _list As New List(Of PermRequest)

        Public ReadOnly Property Count() As Integer Implements System.Collections.Generic.ICollection(Of PermRequest).Count
            Get
                Return _list.Count
            End Get
        End Property

        Public ReadOnly Property IsReadOnly() As Boolean Implements System.Collections.Generic.ICollection(Of PermRequest).IsReadOnly
            Get
                Return False
            End Get
        End Property

        Default Public Property Item(ByVal index As Integer) As PermRequest Implements System.Collections.Generic.IList(Of PermRequest).Item
            Get
                Return _list(index)
            End Get
            Set(ByVal value As PermRequest)
                _list(index) = value
            End Set
        End Property

        Public Sub Add(ByVal item As PermRequest) Implements System.Collections.Generic.ICollection(Of PermRequest).Add
            _list.Add(item)
        End Sub

        Public Sub Clear() Implements System.Collections.Generic.ICollection(Of PermRequest).Clear
            _list.Clear()
        End Sub

        Public Function Contains(ByVal item As PermRequest) As Boolean Implements System.Collections.Generic.ICollection(Of PermRequest).Contains
            Return _list.Contains(item)
        End Function

        Public Sub CopyTo(ByVal array() As PermRequest, ByVal arrayIndex As Integer) Implements System.Collections.Generic.ICollection(Of PermRequest).CopyTo
            _list.CopyTo(array, arrayIndex)
        End Sub

        Public Function GetByPermissionId(ByVal permId As Integer) As IList(Of PermRequest)
            _list.Clear()
            Dim adapter As New SqlDataStore()
            adapter.ExecuteReader(AddressOf RequestReader, "core_permissions_sp_GetRequestByPermId", permId)
            Return _list
        End Function

        Public Function GetEnumerator() As System.Collections.Generic.IEnumerator(Of PermRequest) Implements System.Collections.Generic.IEnumerable(Of PermRequest).GetEnumerator
            Return _list.GetEnumerator
        End Function

        Public Function GetEnumerator1() As System.Collections.IEnumerator Implements System.Collections.IEnumerable.GetEnumerator
            Return _list.GetEnumerator()
        End Function

        Public Function IndexOf(ByVal item As PermRequest) As Integer Implements System.Collections.Generic.IList(Of PermRequest).IndexOf
            Return _list.IndexOf(item)
        End Function

        Public Sub Insert(ByVal index As Integer, ByVal item As PermRequest) Implements System.Collections.Generic.IList(Of PermRequest).Insert
            _list.Insert(index, item)
        End Sub

        Public Function Remove(ByVal item As PermRequest) As Boolean Implements System.Collections.Generic.ICollection(Of PermRequest).Remove
            Return _list.Remove(item)
        End Function

        Public Sub RemoveAt(ByVal index As Integer) Implements System.Collections.Generic.IList(Of PermRequest).RemoveAt
            _list.RemoveAt(index)
        End Sub

        Public Sub Save()

            If (_list.Count = 0) Then
                Exit Sub
            End If

            Dim xml As New XMLString("List")

            For Each req As PermRequest In _list
                xml.BeginElement("Request")
                xml.WriteAttribute("id", req.RequestId.ToString())
                xml.WriteAttribute("userId", req.UserId.ToString())
                xml.WriteAttribute("permId", req.PermissionId.ToString())
                xml.WriteAttribute("reqGranted", IIf(req.Granted, "1", "0"))  'IIf is fine because this line is only reached if req is not nothing
                xml.EndElement()
            Next

            Dim adapter As New SqlDataStore
            adapter.ExecuteNonQuery("core_permissions_sp_updatePermissionRequests", xml.Value)

        End Sub

        Protected Sub RequestReader(ByVal adapter As SqlDataStore, ByVal reader As IDataReader)

            Dim req As New PermRequest
            req.UserId = adapter.GetInt32(reader, 0)
            req.AkoId = adapter.GetString(reader, 1)
            req.UserName = adapter.GetString(reader, 2) + " " + adapter.GetString(reader, 3) + ", " + adapter.GetString(reader, 4)
            req.DateRequested = adapter.GetDateTime(reader, 5)
            req.RequestId = adapter.GetInt32(reader, 6)
            req.PermissionId = adapter.GetInt16(reader, 7)
            _list.Add(req)

        End Sub

    End Class

End Namespace