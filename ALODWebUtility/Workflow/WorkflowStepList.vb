Imports ALOD.Data

Namespace Worklfow

    <Serializable()>
    Public Class WorkflowStepList
        Implements IList(Of WorkflowStep)

        Protected _steps As New List(Of WorkflowStep)
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

        Public Function GetSteps() As WorkflowStepList
            Adapter.ExecuteReader(AddressOf StepReader, "core_workflow_sp_getStepsByWorkflow", _workflowId)
            Return Me
        End Function

        Public Function GetSteps(ByVal workflowId As Byte) As WorkflowStepList
            _workflowId = workflowId
            Return GetSteps()
        End Function

        Public Function GetStepsAsDataSet() As DataSet
            GetSteps()
            Return Me.ToDataSet()
        End Function

        Public Function GetStepsAsDataSet(ByVal workflowId As Byte) As DataSet
            _workflowId = workflowId
            Return GetStepsAsDataSet()
        End Function

        Public Function GetStepsByStatus(ByVal workflowId As Byte, ByVal status As Integer, ByVal isDeathCase As Boolean) As WorkflowStepList
            _workflowId = workflowId
            Dim deathStatus As Char
            If (isDeathCase) Then
                deathStatus = "Y"
            Else
                deathStatus = "N"
            End If
            Adapter.ExecuteReader(AddressOf StepReader, "core_workflow_sp_getStepsByWorkflowAndStatus", _workflowId, status, deathStatus)
            Return Me
        End Function

        Public Function GetStepsByStatusAsDataSet(ByVal workflowId As Byte, ByVal status As Integer, ByVal isDeathCase As Boolean) As DataSet
            GetStepsByStatus(workflowId, status, isDeathCase)
            Return Me.ToDataSet()
        End Function

        Public Function ToDataSet() As DataSet

            Dim data As New DataSet
            Dim topRow As Boolean
            Dim lastStatus As Byte

            Dim top As New DataSets.WorkflowStepsDataTable
            top.TableName = "top"
            Dim children As New DataSets.WorkflowStepsDataTable

            data.Tables.Add(top)
            data.Tables.Add(children)
            data.Relations.Add("steps", top.Columns("statusIn"), children.Columns("statusIn"), False)
            data.Relations(0).Nested = True

            For Each item As WorkflowStep In _steps

                Dim row As DataSets.WorkflowStepsRow
                topRow = (item.StatusIn <> lastStatus)

                If (topRow) Then
                    row = top.NewWorkflowStepsRow
                    lastStatus = item.StatusIn
                Else
                    row = children.NewWorkflowStepsRow
                End If

                item.ToDataRow(row)

                If (topRow) Then
                    top.Rows.Add(row)
                    children.ImportRow(row)
                Else
                    children.Rows.Add(row)
                End If

            Next

            Return data

        End Function

        Protected Sub StepReader(ByVal adapter As SqlDataStore, ByVal reader As IDataReader)

            '0-stepId, 1-workflowId, 2-statusIn 3-statusOut, 4-displayText
            '5-stepType, 6-active, 7-displayOrder
            '8-statusInDescr, 9-statusOutDescr
            '10-groupInId, 11-groupInName, 12-groupOutId, 13-groupOutName
            '14-dbSignTemplate, 15-actionCount, 16-deathStatus, 17-memoTemplate

            Dim item As New WorkflowStep

            item.Id = adapter.GetInt16(reader, 0)
            item.Workflow = adapter.GetByte(reader, 1)
            item.StatusIn = adapter.GetInt32(reader, 2)
            item.StatusOut = adapter.GetInt32(reader, 3)
            item.Text = adapter.GetString(reader, 4)

            item.Active = adapter.GetBoolean(reader, 6)
            item.DisplayOrder = adapter.GetByte(reader, 7)
            item.StatusInDescription = adapter.GetString(reader, 8)
            item.StatusOutDescription = adapter.GetString(reader, 9)

            item.GroupInId = adapter.GetByte(reader, 10)
            item.GroupInDescr = adapter.GetString(reader, 11)
            item.GroupOutId = adapter.GetByte(reader, 12)
            item.GroupOutDescr = adapter.GetString(reader, 13)
            item.DBSignTemplate = adapter.GetByte(reader, 14)
            item.ActionCount = adapter.GetInt32(reader, 15)
            item.DeathStatus = adapter.GetString(reader, 16, "A").Chars(0)

            item.MemoTemplate = adapter.GetByte(reader, 17, 0)

            _steps.Add(item)

        End Sub

#Region "IList"

        Public ReadOnly Property Count() As Integer Implements System.Collections.Generic.ICollection(Of WorkflowStep).Count
            Get
                Return _steps.Count
            End Get
        End Property

        Public ReadOnly Property IsReadOnly() As Boolean Implements System.Collections.Generic.ICollection(Of WorkflowStep).IsReadOnly
            Get
                Return False
            End Get
        End Property

        Default Public Property Item(ByVal stepId As Integer) As WorkflowStep Implements System.Collections.Generic.IList(Of WorkflowStep).Item
            Get
                For Each workstep As WorkflowStep In _steps
                    If (workstep.Id = stepId) Then
                        Return workstep
                    End If
                Next
                Return Nothing
            End Get
            Set(ByVal value As WorkflowStep)
                For Each workstep As WorkflowStep In _steps
                    If (workstep.Id = stepId) Then
                        workstep = value
                    End If
                Next
            End Set
        End Property

        Public Sub Add(ByVal item As WorkflowStep) Implements System.Collections.Generic.ICollection(Of WorkflowStep).Add
            _steps.Add(item)
        End Sub

        Public Sub Clear() Implements System.Collections.Generic.ICollection(Of WorkflowStep).Clear
            _steps.Clear()
        End Sub

        Public Function Contains(ByVal item As WorkflowStep) As Boolean Implements System.Collections.Generic.ICollection(Of WorkflowStep).Contains
            Return _steps.Contains(item)
        End Function

        Public Sub CopyTo(ByVal array() As WorkflowStep, ByVal arrayIndex As Integer) Implements System.Collections.Generic.ICollection(Of WorkflowStep).CopyTo
            _steps.CopyTo(array, arrayIndex)
        End Sub

        Public Function GetEnumerator() As System.Collections.Generic.IEnumerator(Of WorkflowStep) Implements System.Collections.Generic.IEnumerable(Of WorkflowStep).GetEnumerator
            Return _steps.GetEnumerator
        End Function

        Public Function GetEnumerator1() As System.Collections.IEnumerator Implements System.Collections.IEnumerable.GetEnumerator
            Return _steps.GetEnumerator()
        End Function

        Public Function IndexOf(ByVal item As WorkflowStep) As Integer Implements System.Collections.Generic.IList(Of WorkflowStep).IndexOf
            Return _steps.IndexOf(item)
        End Function

        Public Sub Insert(ByVal index As Integer, ByVal item As WorkflowStep) Implements System.Collections.Generic.IList(Of WorkflowStep).Insert
            _steps.Insert(index, item)
        End Sub

        Public Function Remove(ByVal item As WorkflowStep) As Boolean Implements System.Collections.Generic.ICollection(Of WorkflowStep).Remove
            Return _steps.Remove(item)
        End Function

        Public Sub RemoveAt(ByVal index As Integer) Implements System.Collections.Generic.IList(Of WorkflowStep).RemoveAt
            _steps.RemoveAt(index)
        End Sub

#End Region

    End Class

End Namespace