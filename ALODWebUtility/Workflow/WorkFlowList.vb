Imports System.Data.Common
Imports ALOD.Core.Domain.Workflow
Imports ALOD.Data

Namespace Worklfow

    Public Class WorkFlowList
        Implements IList(Of Workflow)

        Protected _workflows As New List(Of Workflow)
        Dim _adapter As SqlDataStore

        Protected ReadOnly Property Adapter() As SqlDataStore
            Get
                If (_adapter Is Nothing) Then
                    _adapter = New SqlDataStore()
                End If

                Return _adapter
            End Get
        End Property

        Public Sub DeleteWorkflow(ByVal id As Byte)
            Dim cmd As DbCommand = Adapter.GetSqlStringCommand(
                "DELETE FROM core_Workflow WHERE workflowId = @workflowId")
            Adapter.AddInParameter(cmd, "@workflowId", DbType.Byte, id)
            Adapter.ExecuteNonQuery(cmd)
        End Sub

        Public Function Find(ByVal id As Integer) As Workflow

            For Each flow As Workflow In _workflows
                If (flow.Id = id) Then
                    Return flow
                End If
            Next

            Return Nothing

        End Function

        Public Function GetAll() As IList(Of Workflow)
            Dim cmd As DbCommand = Adapter.GetSqlStringCommand(
                "SELECT workFlowId, a.compo, title, formal, a.moduleId, active, initialStatus, b.description FROM core_Workflow a LEFT JOIN core_StatusCodes b ON b.statusId = a.initialStatus")
            Adapter.ExecuteReader(AddressOf WorkFlowReader, cmd)
            Return Me
        End Function

        Public Function GetByAllowedCreate(ByVal compo As String, ByVal type As ModuleType, ByVal groupId As Byte) As IList(Of Workflow)
            Adapter.ExecuteReader(AddressOf WorkFlowReader, "core_workflow_sp_GetCreatableByGroup", compo, type, groupId)
            Return Me
        End Function

        Public Function GetByAllowedToView(ByVal groupId As Int16, ByVal type As ModuleType) As WorkFlowList
            Adapter.ExecuteReader(AddressOf WorkFlowReader, "core_workflow_sp_GetViewableByGroup", groupId, type)
            Return Me
        End Function

        Public Function GetByCompo(ByVal compo As String) As IList(Of Workflow)
            Dim cmd As DbCommand = Adapter.GetSqlStringCommand(
                "SELECT workFlowId, a.compo, title, formal, a.moduleId, active, initialStatus, b.description FROM core_Workflow a LEFT JOIN core_StatusCodes b ON b.statusId = a.initialStatus WHERE a.compo = @compo")
            Adapter.AddInParameter(cmd, "@compo", DbType.String, compo)
            Adapter.ExecuteReader(AddressOf WorkFlowReader, cmd)
            Return Me
        End Function

        Public Function GetByCompoAndModule(ByVal compo As String, ByVal type As ModuleType) As IList(Of Workflow)
            Dim cmd As DbCommand = Adapter.GetSqlStringCommand(
                "SELECT a.workFlowId, a.compo, title, formal, a.moduleId, active, initialStatus, c.description FROM core_Workflow a LEFT JOIN core_WorkStatus b ON b.ws_id = a.initialStatus LEFT JOIN core_StatusCodes c ON c.statusId = b.statusId WHERE a.compo = @compo AND a.moduleId = @module")
            Adapter.AddInParameter(cmd, "@compo", DbType.String, compo)
            Adapter.AddInParameter(cmd, "@module", DbType.Byte, type)
            Adapter.ExecuteReader(AddressOf WorkFlowReader, cmd)
            Return Me
        End Function

        Public Function GetByCompoForLODandPDHRA(ByVal compo As String) As IList(Of Workflow)
            Dim sql As String = "SELECT workFlowId, a.compo, title, formal, a.moduleId, active, initialStatus, b.description FROM core_Workflow a LEFT JOIN core_StatusCodes b ON b.statusId = a.initialStatus WHERE a.compo = @compo AND a.moduleId IN(2,8)"
            Dim cmd As DbCommand = Adapter.GetSqlStringCommand(sql)
            Adapter.AddInParameter(cmd, "@compo", DbType.String, compo)
            Adapter.ExecuteReader(AddressOf WorkFlowReader, cmd)
            Return Me
        End Function

        Public Function GetByCompoForLODandPDHRA(ByVal compo As String, ByVal restricted As Boolean) As IList(Of Workflow)
            Dim sql As String = "SELECT workFlowId, a.compo, title, formal, a.moduleId, active, initialStatus, b.description FROM core_Workflow a LEFT JOIN core_StatusCodes b ON b.statusId = a.initialStatus WHERE a.compo = @compo AND a.moduleId IN(2,8)"
            If Not restricted Then
                sql += " AND workflowid NOT IN(9,17)"
            End If
            Dim cmd As DbCommand = Adapter.GetSqlStringCommand(sql)
            Adapter.AddInParameter(cmd, "@compo", DbType.String, compo)
            Adapter.ExecuteReader(AddressOf WorkFlowReader, cmd)
            Return Me
        End Function

        Public Function GetByCompoForPDHRA(ByVal compo As String) As IList(Of Workflow)
            Dim sql As String = "SELECT workFlowId, a.compo, title, formal, a.moduleId, active, initialStatus, b.description FROM core_Workflow a LEFT JOIN core_StatusCodes b ON b.statusId = a.initialStatus WHERE a.compo = @compo AND a.moduleId IN(8)"

            Dim cmd As DbCommand = Adapter.GetSqlStringCommand(sql)
            Adapter.AddInParameter(cmd, "@compo", DbType.String, compo)
            Adapter.ExecuteReader(AddressOf WorkFlowReader, cmd)
            Return Me
        End Function

        Public Sub UpdateWorkflow(ByVal title As String, ByVal moduleId As Byte, ByVal isFormal As Boolean, ByVal id As Byte, ByVal active As Boolean, ByVal initialStatus As Integer)
            Adapter.ExecuteNonQuery("core_workflow_sp_UpdateWorkflow", id, title, moduleId, isFormal, active, initialStatus)
        End Sub

        Protected Sub WorkFlowReader(ByVal adapter As SqlDataStore, ByVal reader As IDataReader)

            '0-workflowid, 1-compo, 2-title, 3-formal, 4-moduleId, 5-active, 6-initialStatus, 7-statusDesc
            Dim flow As New Workflow

            flow.Id = adapter.GetByte(reader, 0)
            flow.Compo = adapter.GetString(reader, 1)
            flow.Title = adapter.GetString(reader, 2)
            flow.IsFormal = adapter.GetBoolean(reader, 3)
            flow.ModuleId = adapter.GetByte(reader, 4)
            flow.Active = adapter.GetBoolean(reader, 5)
            flow.InitialStatus = adapter.GetInt32(reader, 6)
            flow.StatusDescription = adapter.GetString(reader, 7)

            _workflows.Add(flow)

        End Sub

#Region "IList"

        Public ReadOnly Property Count() As Integer Implements System.Collections.Generic.ICollection(Of Workflow).Count
            Get
                Return _workflows.Count
            End Get
        End Property

        Public ReadOnly Property IsReadOnly() As Boolean Implements System.Collections.Generic.ICollection(Of Workflow).IsReadOnly
            Get
                Return False
            End Get
        End Property

        Default Public Property Item(ByVal id As Integer) As Workflow Implements System.Collections.Generic.IList(Of Workflow).Item
            Get
                For Each flow As Workflow In _workflows
                    If flow.Id = id Then
                        Return flow
                    End If
                Next
                Return Nothing
            End Get
            Set(ByVal value As Workflow)
                For Each flow As Workflow In _workflows
                    If flow.Id = id Then
                        flow = value
                    End If
                Next
            End Set
        End Property

        Public Sub Add(ByVal item As Workflow) Implements System.Collections.Generic.ICollection(Of Workflow).Add
            _workflows.Add(item)
        End Sub

        Public Sub Clear() Implements System.Collections.Generic.ICollection(Of Workflow).Clear
            _workflows.Clear()
        End Sub

        Public Function Contains(ByVal item As Workflow) As Boolean Implements System.Collections.Generic.ICollection(Of Workflow).Contains
            Return _workflows.Contains(item)
        End Function

        Public Sub CopyTo(ByVal array() As Workflow, ByVal arrayIndex As Integer) Implements System.Collections.Generic.ICollection(Of Workflow).CopyTo
            _workflows.CopyTo(array, arrayIndex)
        End Sub

        Public Function GetEnumerator() As System.Collections.Generic.IEnumerator(Of Workflow) Implements System.Collections.Generic.IEnumerable(Of Workflow).GetEnumerator
            Return _workflows.GetEnumerator
        End Function

        Public Function GetEnumerator1() As System.Collections.IEnumerator Implements System.Collections.IEnumerable.GetEnumerator
            Return _workflows.GetEnumerator()
        End Function

        Public Function IndexOf(ByVal item As Workflow) As Integer Implements System.Collections.Generic.IList(Of Workflow).IndexOf
            Return _workflows.IndexOf(item)
        End Function

        Public Sub Insert(ByVal index As Integer, ByVal item As Workflow) Implements System.Collections.Generic.IList(Of Workflow).Insert
            _workflows.Insert(index, item)
        End Sub

        Public Function Remove(ByVal item As Workflow) As Boolean Implements System.Collections.Generic.ICollection(Of Workflow).Remove
            Return _workflows.Remove(item)
        End Function

        Public Sub RemoveAt(ByVal index As Integer) Implements System.Collections.Generic.IList(Of Workflow).RemoveAt
            _workflows.RemoveAt(index)
        End Sub

#End Region

    End Class

End Namespace