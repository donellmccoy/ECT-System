Imports System.Text
Imports ALOD.Data
Imports ALODWebUtility.Common

Namespace Permission

    <Serializable()>
    Public Class PermissionList
        Implements IList(Of Permission)

        Protected _perms As List(Of Permission)
        Protected sqDataStore As SqlDataStore

        Public Sub New()
            _perms = New List(Of Permission)
        End Sub

        Protected ReadOnly Property DataStore() As SqlDataStore
            Get
                If (sqDataStore Is Nothing) Then
                    sqDataStore = New SqlDataStore()
                End If

                Return sqDataStore
            End Get
        End Property

        ''' <summary>
        ''' Assigns the current permission list to the specified group
        ''' </summary>
        ''' <param name="groupId">The ID of the group</param>
        ''' <remarks></remarks>
        Public Sub AssignToGroup(ByVal groupId As Short)

            If (Count = 0) Then
                Exit Sub
            End If

            Dim xml As New XMLString("list")

            For Each perm As Permission In _perms
                xml.BeginElement("item")
                xml.WriteAttribute("permId", perm.Id.ToString())
                xml.EndElement()
            Next

            'now pass it to SQL
            Dim st As String = xml.ToString()
            DataStore.ExecuteNonQuery("core_group_sp_UpdateGroupPermissions", groupId, xml.Value)

        End Sub

        ''' <summary>
        ''' Assigns the current permission list to the specified user
        ''' </summary>
        ''' <param name="userId">The ID of the user</param>
        ''' <remarks></remarks>
        Public Sub AssignToUser(ByVal userId As Short)

            Dim xml As New XMLString("list")

            For Each perm As Permission In _perms
                xml.BeginElement("item")
                xml.WriteAttribute("permId", perm.Id.ToString())
                xml.WriteAttribute("status", perm.Status)
                xml.EndElement()
            Next

            'now pass it to SQL
            Dim st As String = xml.ToString()
            DataStore.ExecuteNonQuery("core_permissions_sp_UpdateUserPermissions", userId, xml.Value)

        End Sub

        ''' <summary>
        ''' Delete a permission
        ''' </summary>
        ''' <param name="id">The permId of the permission to delete</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function DeletePermission(ByVal id As Integer) As Boolean
            Dim count As Integer = DataStore.ExecuteNonQuery("core_permissions_sp_Delete", id)
            Return (count > 0)

        End Function

        Public Function Find(ByVal permId As Integer) As Permission

            For Each perm As Permission In _perms
                If (perm.Id = permId) Then
                    Return perm
                End If
            Next

            Return Nothing

        End Function

        ''' <summary>
        ''' Retrieves all permissions
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetAll() As DataSets.PermissionDataTable
            _perms.Clear()
            DataStore.ExecuteReader(AddressOf PermReader, "core_permissions_sp_GetALL")
            Return Me.ToDataSet()
        End Function

        ''' <summary>
        ''' Loads the permissions list for a user group
        ''' </summary>
        ''' <param name="groupId"></param>
        ''' <remarks>All permissions are returned with the allowed flag indicating if
        ''' the role has access to that permissions
        ''' </remarks>
        Public Function GetByGroupId(ByVal groupId As Integer) As DataSets.PermissionDataTable
            _perms.Clear()
            DataStore.ExecuteReader(AddressOf PermReader, "core_permissions_sp_GetByGroup", groupId)
            Return Me.ToDataSet
        End Function

        ''' <summary>
        ''' Retrieves the current permissions for a user
        ''' </summary>
        ''' <param name="userId"></param>
        ''' <remarks>This returns only allowed permissions for the user</remarks>
        Public Function GetByUserId(ByVal userId As Integer) As DataSets.PermissionDataTable
            _perms.Clear()
            DataStore.ExecuteReader(AddressOf PermReader, "core_permissions_sp_GetByUserId", userId)
            Return Me.ToDataSet
        End Function

        ''' <summary>
        ''' Retrieves the current permissions for a user
        ''' </summary>
        ''' <param name="userName"></param>
        ''' <remarks>This returns only allowed permissions for the user</remarks>
        Public Function GetByUserName(ByVal userName As String) As DataSets.PermissionDataTable
            _perms.Clear()
            DataStore.ExecuteReader(AddressOf PermReader, "core_permissions_sp_GetByUserName", userName)
            Return Me.ToDataSet
        End Function

        Public Function GetUserAssignable() As DataSets.PermissionDataTable
            _perms.Clear()
            DataStore.ExecuteReader(AddressOf PermReader, "core_permissions_sp_GetUserAssignable")
            Return Me.ToDataSet()
        End Function

        Public Function GetUserPermissions(ByVal userId As Integer) As PermissionList
            _perms.Clear()
            DataStore.ExecuteReader(AddressOf UserPermReader, "core_permissions_sp_GetUserPerms", userId)
            Return Me
        End Function

        ''' <summary>
        ''' Insert a new permission
        ''' </summary>
        ''' <param name="name">The name of the permission</param>
        ''' <param name="description">A description of the permission</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function InsertPermission(ByVal name As String, ByVal description As String, ByVal exclude As Boolean) As Boolean
            Dim count As Integer = DataStore.ExecuteNonQuery("core_permissions_sp_Insert", name, description, exclude)
            Return (count > 0)
        End Function

        Public Overrides Function ToString() As String

            Dim buffer As New StringBuilder

            For Each perm As Permission In _perms
                buffer.Append(perm.Name + ",")
            Next

            If (buffer.Length > 0) Then
                buffer = buffer.Remove(buffer.Length - 1, 1)
            End If

            Return buffer.ToString()
        End Function

        ''' <summary>
        ''' Update a permissions
        ''' </summary>
        ''' <param name="id">the permId of the permission to update</param>
        ''' <param name="name">The new name</param>
        ''' <param name="description">The new description</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function UpdatePermission(ByVal id As Integer, ByVal name As String, ByVal description As String, ByVal exclude As Boolean) As Boolean

            Dim count As Integer = DataStore.ExecuteNonQuery("core_permissions_sp_Update", id, name, description, exclude)
            Return (count > 0)

        End Function

        Protected Sub PermReader(ByVal adapter As SqlDataStore, ByVal reader As IDataReader)

            Dim perm As New Permission()
            perm.Id = adapter.GetNumber(reader, 0)
            perm.Name = adapter.GetString(reader, 1)
            perm.Description = adapter.GetString(reader, 2)
            perm.Allowed = adapter.GetBoolean(reader, 3)
            perm.Exclude = adapter.GetBoolean(reader, 4)
            _perms.Add(perm)

        End Sub

        Protected Function ToDataSet() As DataSets.PermissionDataTable

            Dim data As New DataSets.PermissionDataTable
            Dim row As DataSets.PermissionRow

            For Each perm As Permission In _perms
                row = data.NewPermissionRow()
                perm.ToDataRow(row)
                data.Rows.Add(row)
            Next

            Return data

        End Function

        Protected Sub UserPermReader(ByVal adapter As SqlDataStore, ByVal reader As IDataReader)

            Dim perm As New Permission()
            perm.Id = adapter.GetInt16(reader, 0)
            perm.Name = adapter.GetString(reader, 1)
            perm.Description = adapter.GetString(reader, 2)
            perm.Status = adapter.GetString(reader, 3)
            perm.Exclude = adapter.GetBoolean(reader, 4)
            _perms.Add(perm)

        End Sub

#Region "IList"

        Public ReadOnly Property Count() As Integer Implements System.Collections.Generic.ICollection(Of Permission).Count
            Get
                Return _perms.Count
            End Get
        End Property

        Public ReadOnly Property IsReadOnly() As Boolean Implements System.Collections.Generic.ICollection(Of Permission).IsReadOnly
            Get
                Return False
            End Get
        End Property

        Default Public Property Item(ByVal index As Integer) As Permission Implements System.Collections.Generic.IList(Of Permission).Item
            Get
                Return _perms(index)
            End Get
            Set(ByVal value As Permission)
                _perms(index) = value
            End Set
        End Property

        Public Sub Add(ByVal item As Permission) Implements System.Collections.Generic.ICollection(Of Permission).Add
            _perms.Add(item)
        End Sub

        Public Sub Clear() Implements System.Collections.Generic.ICollection(Of Permission).Clear
            _perms.Clear()
        End Sub

        Public Function Contains(ByVal item As Permission) As Boolean Implements System.Collections.Generic.ICollection(Of Permission).Contains
            Return _perms.Contains(item)
        End Function

        Public Sub CopyTo(ByVal array() As Permission, ByVal arrayIndex As Integer) Implements System.Collections.Generic.ICollection(Of Permission).CopyTo
            _perms.CopyTo(array, arrayIndex)
        End Sub

        Public Function GetEnumerator() As System.Collections.Generic.IEnumerator(Of Permission) Implements System.Collections.Generic.IEnumerable(Of Permission).GetEnumerator
            Return _perms.GetEnumerator()
        End Function

        Public Function GetEnumerator1() As System.Collections.IEnumerator Implements System.Collections.IEnumerable.GetEnumerator
            Return _perms.GetEnumerator()
        End Function

        Public Function IndexOf(ByVal item As Permission) As Integer Implements System.Collections.Generic.IList(Of Permission).IndexOf
            Return _perms.IndexOf(item)
        End Function

        Public Sub Insert(ByVal index As Integer, ByVal item As Permission) Implements System.Collections.Generic.IList(Of Permission).Insert
            _perms.Insert(index, item)
        End Sub

        Public Function Remove(ByVal item As Permission) As Boolean Implements System.Collections.Generic.ICollection(Of Permission).Remove
            Return _perms.Remove(item)
        End Function

        Public Sub RemoveAt(ByVal index As Integer) Implements System.Collections.Generic.IList(Of Permission).RemoveAt
            _perms.RemoveAt(index)
        End Sub

#End Region

    End Class

End Namespace