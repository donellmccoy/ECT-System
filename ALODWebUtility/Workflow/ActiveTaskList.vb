Imports System.Data.Common
Imports System.Web
Imports ALOD.Data

Namespace Worklfow

    Public Class ActiveTaskList
        Implements IList(Of ActiveTask)

        Private _taskList As List(Of ActiveTask)

        Public Sub New()
            _taskList = New List(Of ActiveTask)
        End Sub

        Public Function GetActivities(ByVal workflow As String) As ActiveTaskList
            Dim adapter As New SqlDataStore()
            Dim cmd As DbCommand

            cmd = adapter.GetStoredProcCommand("form2173_sp_GetActivityByRegion")
            adapter.AddInParameter(cmd, "@workflow", Data.DbType.String, workflow)
            adapter.AddInParameter(cmd, "@userId", Data.DbType.Int32, CInt(HttpContext.Current.Session("UserId")))
            adapter.AddInParameter(cmd, "@compo", Data.DbType.String, CStr(HttpContext.Current.Session("Compo")))
            adapter.ExecuteReader(AddressOf ActivityListReader, cmd)

            Return Me

        End Function

        Public Function GetNGBAors(ByVal workflow As String) As ActiveTaskList
            Dim adapter As New SqlDataStore()
            Dim cmd As DbCommand
            cmd = adapter.GetStoredProcCommand("core_workflow_sp_GetAORActivity")
            adapter.AddInParameter(cmd, "@compo", Data.DbType.String, CStr(HttpContext.Current.Session("Compo")))
            adapter.AddInParameter(cmd, "@workflow", Data.DbType.String, workflow)
            adapter.AddInParameter(cmd, "@userId", Data.DbType.Int32, CInt(HttpContext.Current.Session("UserId")))
            adapter.ExecuteReader(AddressOf StatusByAorReader, cmd)
            Return Me
        End Function

        Public Function GetWorkflowTitles() As ActiveTaskList
            Dim adapter As New SqlDataStore()
            Dim cmd As DbCommand = adapter.GetStoredProcCommand("core_workflow_sp_GetWorkflowByCompo")
            adapter.AddInParameter(cmd, "@compo", Data.DbType.String, CStr(HttpContext.Current.Session("Compo")))
            adapter.AddInParameter(cmd, "@userId", Data.DbType.Int32, CInt(HttpContext.Current.Session("UserId")))
            adapter.ExecuteReader(AddressOf WorkflowListReader, cmd)
            Return Me
        End Function

        Private Sub ActivityListReader(ByVal adapter As SqlDataStore, ByVal reader As System.Data.IDataReader)
            Dim activeTask As New ActiveTask()

            activeTask.ActiveTitle = adapter.GetString(reader, 0)
            activeTask.OverdueCount = adapter.GetInt32(reader, 1)
            activeTask.FormalCount = adapter.GetInt32(reader, 2)
            activeTask.InformalCount = adapter.GetInt32(reader, 3)
            activeTask.StatusID = adapter.GetByte(reader, 4)
            _taskList.Add(activeTask)
        End Sub

        Private Sub StatusByAorReader(ByVal adapter As SqlDataStore, ByVal reader As System.Data.IDataReader)
            Dim statusByAorReader As New ActiveTask()
            statusByAorReader.ActiveTitle = adapter.GetString(reader, 0)
            statusByAorReader.RegionId = adapter.GetByte(reader, 2)
            statusByAorReader.OverdueCount = adapter.GetInt32(reader, 3)
            statusByAorReader.FormalCount = adapter.GetInt32(reader, 4)
            statusByAorReader.InformalCount = adapter.GetInt32(reader, 5)
            _taskList.Add(statusByAorReader)
        End Sub

        Private Sub WorkflowListReader(ByVal adapter As SqlDataStore, ByVal reader As System.Data.IDataReader)
            Dim workflowListReader As New ActiveTask()
            workflowListReader.WorkflowId = adapter.GetByte(reader, 0)
            workflowListReader.ActiveTitle = adapter.GetString(reader, 1)
            _taskList.Add(workflowListReader)
        End Sub

#Region "iList"

        Public ReadOnly Property Count() As Integer Implements System.Collections.Generic.ICollection(Of ActiveTask).Count
            Get
                Return _taskList.Count
            End Get
        End Property

        Public ReadOnly Property IsReadOnly() As Boolean Implements System.Collections.Generic.ICollection(Of ActiveTask).IsReadOnly
            Get
                Return False
            End Get
        End Property

        Default Public Property Item(ByVal index As Integer) As ActiveTask Implements System.Collections.Generic.IList(Of ActiveTask).Item
            Get
                Return _taskList.Item(index)
            End Get
            Set(ByVal value As ActiveTask)
                _taskList.Item(index) = value
            End Set
        End Property

        Public Sub Add(ByVal item As ActiveTask) Implements System.Collections.Generic.ICollection(Of ActiveTask).Add
            _taskList.Add(item)
        End Sub

        Public Sub Clear() Implements System.Collections.Generic.ICollection(Of ActiveTask).Clear
            _taskList.Clear()
        End Sub

        Public Function Contains(ByVal item As ActiveTask) As Boolean Implements System.Collections.Generic.ICollection(Of ActiveTask).Contains
            Return _taskList.Contains(item)
        End Function

        Public Sub CopyTo(ByVal array() As ActiveTask, ByVal arrayIndex As Integer) Implements System.Collections.Generic.ICollection(Of ActiveTask).CopyTo
            _taskList.CopyTo(array, arrayIndex)
        End Sub

        Public Function GetEnumerator() As System.Collections.Generic.IEnumerator(Of ActiveTask) Implements System.Collections.Generic.IEnumerable(Of ActiveTask).GetEnumerator
            Return _taskList.GetEnumerator()
        End Function

        Public Function GetEnumerator1() As System.Collections.IEnumerator Implements System.Collections.IEnumerable.GetEnumerator
            Return _taskList.GetEnumerator()
        End Function

        Public Function IndexOf(ByVal item As ActiveTask) As Integer Implements System.Collections.Generic.IList(Of ActiveTask).IndexOf
            Return _taskList.IndexOf(item)
        End Function

        Public Sub Insert(ByVal index As Integer, ByVal item As ActiveTask) Implements System.Collections.Generic.IList(Of ActiveTask).Insert
            _taskList.Insert(index, item)
        End Sub

        Public Function Remove(ByVal item As ActiveTask) As Boolean Implements System.Collections.Generic.ICollection(Of ActiveTask).Remove
            Return _taskList.Remove(item)
        End Function

        Public Sub RemoveAt(ByVal index As Integer) Implements System.Collections.Generic.IList(Of ActiveTask).RemoveAt
            _taskList.RemoveAt(index)
        End Sub

#End Region

    End Class

End Namespace