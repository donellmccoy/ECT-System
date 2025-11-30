Imports ALOD.Core.Domain.Workflow
Imports ALOD.Data

Namespace Worklfow

    Public Class StatusCodeList
        Inherits System.Web.UI.Page
        Implements IList(Of StatusCode)

        Protected _adapter As SqlDataStore

        Protected _codes As New List(Of StatusCode)

        Protected ReadOnly Property DataStore() As SqlDataStore
            Get
                If (_adapter Is Nothing) Then
                    _adapter = New SqlDataStore()
                End If

                Return _adapter
            End Get
        End Property

        Public Sub DeleteStatusCode(ByVal statusId As Integer)
            DataStore.ExecuteNonQuery("core_workflow_sp_DeleteStatusCode", statusId)
        End Sub

        Public Function Find(ByVal statusId As Byte) As StatusCode

            For Each code As StatusCode In _codes
                If (code.StatusId = statusId) Then
                    Return code
                End If
            Next

            Return Nothing

        End Function

        Public Function GetByCompo(ByVal compo As String) As StatusCodeList
            DataStore.ExecuteReader(AddressOf CodeReader, "core_workflow_sp_GetStatusCodesByCompo", compo)
            Return Me
        End Function

        Public Function GetByCompoAndModule(ByVal compo As String, ByVal type As Byte) As StatusCodeList
            DataStore.ExecuteReader(AddressOf CodeReader, "core_workflow_sp_GetStatusCodesByCompoAndModule", compo, type)
            Return Me
        End Function

        Public Function GetByCompoAndModuleAsDataSet(ByVal compo As String, ByVal type As Byte) As DataSets.StatusCodeDataTable
            Return Me.GetByCompoAndModule(compo, type).ToDataSet()
        End Function

        Public Function GetByCompoAndModuleWithGroup(ByVal compo As String, ByVal type As Byte) As StatusCodeList
            DataStore.ExecuteReader(AddressOf CodeReader, "core_workflow_sp_GetStatusCodesByCompoAndModuleWithGroup", compo, type)
            Return Me
        End Function

        Public Function GetByCompoAsDataSet(ByVal compo As String) As DataSets.StatusCodeDataTable
            Return GetByCompo(compo).ToDataSet()
        End Function

        Public Function GetByCompoForPDHRAandLOD(ByVal compo As String, Optional ByVal justPdhra As Boolean = False) As StatusCodeList
            DataStore.ExecuteReader(AddressOf CodeReader, "core_workflow_sp_GetStatusCodesByCompoForPDHRAandLOD", compo, justPdhra)
            Return Me
        End Function

        Public Function GetBySignCode(ByVal groupId As Short, ByVal moduleId As Byte) As StatusCodeList
            DataStore.ExecuteReader(AddressOf CodeReaderShort, "core_workflow_sp_GetStatusCodesBySignCode", groupId, moduleId)
            Return Me
        End Function

        Public Function GetByWorkflow(ByVal workflow As Short) As StatusCodeList

            DataStore.ExecuteReader(AddressOf CodeReader, "core_workflow_sp_GetStatusCodesByWorkflow", workflow)
            Return Me

        End Function

        Public Function GetByWorkflowDataSet(ByVal workflow As Short) As DataSets.StatusCodeDataTable
            Return GetByWorkflow(workflow).ToDataSet()
        End Function

        Public Function ToDataSet() As DataSets.StatusCodeDataTable

            Dim data As New DataSets.StatusCodeDataTable
            Dim row As DataSets.StatusCodeRow

            For Each code As StatusCode In _codes
                row = data.NewStatusCodeRow
                code.ToDataRow(row)
                data.Rows.Add(row)
            Next

            Return data

        End Function

        Public Sub UpdateStatusCode(ByVal statusId As Integer, ByVal description As String, ByVal moduleId As ModuleType, ByVal groupId As Byte, ByVal isFinal As Boolean, ByVal isApproved As Boolean, ByVal canAppeal As Boolean, ByVal displayOrder As Byte, ByVal isDisposition As Boolean, ByVal isFormal As Boolean)
            If moduleId = 0 Then
                moduleId = Session("moduleId")
            End If
            DataStore.ExecuteNonQuery("core_workflow_sp_UpdateStatusCode",
                statusId, description, isFinal, isApproved, canAppeal, IIf(groupId <> 0, groupId, DBNull.Value), moduleId, displayOrder, isDisposition, isFormal)
        End Sub

        Protected Sub CodeReader(ByVal adapter As SqlDataStore, ByVal reader As System.Data.IDataReader)

            '0-statusId, 1-description, 2-moduleId, 3-compo,
            '4-groupId, 5-isFinal, 6-isApproved, 7-canAppeal, 8-groupName
            '9-moduleName, 10-compoDescr, 11-fullDescription, 12-DisplayOrder
            '13-isDisposition, 14-isFormal
            Dim code As New StatusCode

            code.StatusId = adapter.GetInt32(reader, 0)
            code.Description = adapter.GetString(reader, 1)
            code.ModuleId = adapter.GetByte(reader, 2)
            code.Compo = adapter.GetString(reader, 3)
            code.GroupId = adapter.GetByte(reader, 4)
            code.IsFinal = adapter.GetBoolean(reader, 5)
            code.IsApproved = adapter.GetBoolean(reader, 6)
            code.CanAppeal = adapter.GetBoolean(reader, 7)
            code.GroupName = adapter.GetString(reader, 8)
            code.ModuleName = adapter.GetString(reader, 9)
            code.CompoDescr = adapter.GetString(reader, 10)
            code.FullDescription = adapter.GetString(reader, 11)
            code.DisplayOrder = adapter.GetByte(reader, 12)
            code.IsDisposition = adapter.GetBoolean(reader, 13)
            code.IsFormal = adapter.GetBoolean(reader, 14)

            _codes.Add(code)

        End Sub

        Protected Sub CodeReaderShort(ByVal adapter As SqlDataStore, ByVal reader As System.Data.IDataReader)

            Dim code As New StatusCode
            code.StatusId = adapter.GetInt32(reader, 0)
            code.Description = adapter.GetString(reader, 1)
            _codes.Add(code)

        End Sub

#Region "IList"

        Public ReadOnly Property Count() As Integer Implements System.Collections.Generic.ICollection(Of StatusCode).Count
            Get
                Return _codes.Count
            End Get
        End Property

        Public ReadOnly Property IsReadOnly() As Boolean Implements System.Collections.Generic.ICollection(Of StatusCode).IsReadOnly
            Get
                Return False
            End Get
        End Property

        Default Public Property Item(ByVal index As Integer) As StatusCode Implements System.Collections.Generic.IList(Of StatusCode).Item
            Get

                For Each code As StatusCode In _codes
                    If (code.StatusId = index) Then
                        Return code
                    End If
                Next

                Return Nothing

            End Get

            Set(ByVal value As StatusCode)
                For Each code As StatusCode In _codes
                    If (code.StatusId = index) Then
                        code = value
                    End If
                Next
            End Set
        End Property

        Public Sub Add(ByVal item As StatusCode) Implements System.Collections.Generic.ICollection(Of StatusCode).Add
            _codes.Add(item)
        End Sub

        Public Sub Clear() Implements System.Collections.Generic.ICollection(Of StatusCode).Clear
            _codes.Clear()
        End Sub

        Public Function Contains(ByVal item As StatusCode) As Boolean Implements System.Collections.Generic.ICollection(Of StatusCode).Contains
            Return _codes.Contains(item)
        End Function

        Public Sub CopyTo(ByVal array() As StatusCode, ByVal arrayIndex As Integer) Implements System.Collections.Generic.ICollection(Of StatusCode).CopyTo
            _codes.CopyTo(array, arrayIndex)
        End Sub

        Public Function GetEnumerator() As System.Collections.Generic.IEnumerator(Of StatusCode) Implements System.Collections.Generic.IEnumerable(Of StatusCode).GetEnumerator
            Return _codes.GetEnumerator()
        End Function

        Public Function GetEnumerator1() As System.Collections.IEnumerator Implements System.Collections.IEnumerable.GetEnumerator
            Return _codes.GetEnumerator()
        End Function

        Public Function IndexOf(ByVal item As StatusCode) As Integer Implements System.Collections.Generic.IList(Of StatusCode).IndexOf
            Return _codes.IndexOf(item)
        End Function

        Public Sub Insert(ByVal index As Integer, ByVal item As StatusCode) Implements System.Collections.Generic.IList(Of StatusCode).Insert
            _codes.Insert(index, item)
        End Sub

        Public Function Remove(ByVal item As StatusCode) As Boolean Implements System.Collections.Generic.ICollection(Of StatusCode).Remove
            Return _codes.Remove(item)
        End Function

        Public Sub RemoveAt(ByVal index As Integer) Implements System.Collections.Generic.IList(Of StatusCode).RemoveAt
            _codes.RemoveAt(index)
        End Sub

#End Region

    End Class

End Namespace