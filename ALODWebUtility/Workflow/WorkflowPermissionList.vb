Imports ALOD.Data
Imports ALODWebUtility.Common

Namespace Worklfow

    Public Class WorkflowPermissionList
        Implements IList(Of WorkflowPermission)

        Protected _permissions As New List(Of WorkflowPermission)
        Protected _workflowId As Byte = 0
        Dim _adapter As SqlDataStore

        Protected ReadOnly Property Adapter() As SqlDataStore
            Get
                If (_adapter Is Nothing) Then
                    _adapter = New SqlDataStore()
                End If

                Return _adapter
            End Get
        End Property

        Public Function GetByWorkflow(ByVal workflowId As Byte) As IList(Of WorkflowPermission)

            _permissions.Clear()
            _workflowId = workflowId
            Adapter.ExecuteReader(AddressOf PermissionsReader, "core_Workflow_sp_GetPermissions", workflowId)
            Return Me

        End Function

        Public Function GetByWorkflowAndCompo(ByVal workflowId As Byte, ByVal compo As String) As IList(Of WorkflowPermission)

            _permissions.Clear()
            _workflowId = workflowId
            Adapter.ExecuteReader(AddressOf PermissionsReader, "core_Workflow_sp_GetPermissionsByCompo", workflowId, compo)
            Return Me

        End Function

        Public Function UpdateWorkflow(ByVal workflowId As Byte, ByVal compo As String) As Boolean

            _workflowId = workflowId

            If (Count = 0) Then
                Return False
            End If

            'generate the XML for the bulk update
            Dim xml As New XMLString("list")

            For Each perm As WorkflowPermission In _permissions
                'IIf is fine because these lines are only reached if perm is not nothing
                xml.BeginElement("item")
                xml.WriteAttribute("groupId", perm.GroupId.ToString())
                xml.WriteAttribute("canCreate", IIf(perm.CanCreate, "1", "0"))
                xml.WriteAttribute("canView", IIf(perm.CanView, "1", "0"))
                xml.EndElement()
            Next

            'now pass it to SQL
            Dim st As String = xml.ToString()
            Adapter.ExecuteNonQuery("core_workflow_sp_UpdatePermissions", _workflowId, compo, xml.Value)
            Return True

        End Function

        Protected Sub PermissionsReader(ByVal adapter As SqlDataStore, ByVal reader As IDataReader)

            '0-groupid, 1-groupname, 2-canView, 3-canCreate,
            Dim permission As New WorkflowPermission
            permission.GroupId = adapter.GetByte(reader, 0)
            permission.GroupName = adapter.GetString(reader, 1)
            permission.CanView = adapter.GetBoolean(reader, 2)
            permission.CanCreate = adapter.GetBoolean(reader, 3)

            _permissions.Add(permission)

        End Sub

#Region "IList"

        Public ReadOnly Property Count() As Integer Implements System.Collections.Generic.ICollection(Of WorkflowPermission).Count
            Get
                Return _permissions.Count
            End Get
        End Property

        Public ReadOnly Property IsReadOnly() As Boolean Implements System.Collections.Generic.ICollection(Of WorkflowPermission).IsReadOnly
            Get
                Return False
            End Get
        End Property

        Default Public Property Item(ByVal groupId As Integer) As WorkflowPermission Implements System.Collections.Generic.IList(Of WorkflowPermission).Item
            Get
                For Each permission As WorkflowPermission In _permissions
                    If (permission.GroupId = groupId) Then
                        Return permission
                    End If
                Next
                Return Nothing
            End Get
            Set(ByVal value As WorkflowPermission)
                For Each permission As WorkflowPermission In _permissions
                    If (permission.GroupId = groupId) Then
                        permission = value
                    End If
                Next
            End Set
        End Property

        Public Sub Add(ByVal item As WorkflowPermission) Implements System.Collections.Generic.ICollection(Of WorkflowPermission).Add
            _permissions.Add(item)
        End Sub

        Public Sub Clear() Implements System.Collections.Generic.ICollection(Of WorkflowPermission).Clear
            _permissions.Clear()
        End Sub

        Public Function Contains(ByVal item As WorkflowPermission) As Boolean Implements System.Collections.Generic.ICollection(Of WorkflowPermission).Contains
            Return _permissions.Contains(item)
        End Function

        Public Sub CopyTo(ByVal array() As WorkflowPermission, ByVal arrayIndex As Integer) Implements System.Collections.Generic.ICollection(Of WorkflowPermission).CopyTo
            _permissions.CopyTo(array, arrayIndex)
        End Sub

        Public Function GetEnumerator() As System.Collections.Generic.IEnumerator(Of WorkflowPermission) Implements System.Collections.Generic.IEnumerable(Of WorkflowPermission).GetEnumerator
            Return _permissions.GetEnumerator()
        End Function

        Public Function GetEnumerator1() As System.Collections.IEnumerator Implements System.Collections.IEnumerable.GetEnumerator
            Return _permissions.GetEnumerator()
        End Function

        Public Function IndexOf(ByVal item As WorkflowPermission) As Integer Implements System.Collections.Generic.IList(Of WorkflowPermission).IndexOf
            Return _permissions.IndexOf(item)
        End Function

        Public Sub Insert(ByVal index As Integer, ByVal item As WorkflowPermission) Implements System.Collections.Generic.IList(Of WorkflowPermission).Insert
            _permissions.Insert(index, item)
        End Sub

        Public Function Remove(ByVal item As WorkflowPermission) As Boolean Implements System.Collections.Generic.ICollection(Of WorkflowPermission).Remove
            Return _permissions.Remove(item)
        End Function

        Public Sub RemoveAt(ByVal index As Integer) Implements System.Collections.Generic.IList(Of WorkflowPermission).RemoveAt
            _permissions.RemoveAt(index)
        End Sub

#End Region

    End Class

End Namespace