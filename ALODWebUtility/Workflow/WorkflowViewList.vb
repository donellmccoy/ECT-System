Imports ALOD.Data
Imports ALODWebUtility.PageTitles

Namespace Worklfow

    Public Class WorkflowViewList
        Implements IList(Of WorkflowView)

        Dim _adapter As SqlDataStore
        Dim _workflow As New List(Of WorkflowView)

        Protected ReadOnly Property Adapter() As SqlDataStore
            Get
                If (_adapter Is Nothing) Then
                    _adapter = New SqlDataStore()
                End If

                Return _adapter
            End Get
        End Property

        Public Sub CodeReader(ByVal adapter As SqlDataStore, ByVal reader As System.Data.IDataReader)

            Dim pages As New WorkflowView

            pages.PageId = adapter.GetInt16(reader, 0)
            pages.Title = adapter.GetString(reader, 1)

            _workflow.Add(pages)

        End Sub

        Public Sub DeleteWorkflowView(ByVal pageId As Integer, ByVal workflowId As Integer)
            Dim cmd As System.Data.Common.DbCommand = Adapter.GetStoredProcCommand("core_workflowView_sp_Delete", pageId, workflowId)
            Adapter.ExecuteNonQuery(cmd)
        End Sub

        Public Function GetPagesAsDataSet(ByVal workflowId As Integer) As DataSets.PageTitlesDataTable
            Return GetPagesByWorkflowId(workflowId).ToDataSet()
        End Function

        Public Function ToDataSet() As DataSets.PageTitlesDataTable

            Dim data As New DataSets.PageTitlesDataTable
            Dim row As DataSets.PageTitlesRow

            For Each page As WorkflowView In _workflow
                row = data.NewPageTitlesRow()
                page.ToDataRow(row)
                data.Rows.Add(row)
            Next

            Return data
        End Function

        Private Function GetPagesByWorkflowId(ByVal workflowId As Integer) As WorkflowViewList
            Dim _list As New PageTitleList
            Adapter.ExecuteReader(AddressOf CodeReader, "core_workflow_sp_GetPagesByWorkflowId", workflowId)
            Return Me

        End Function

#Region "IList"

        Public ReadOnly Property Count() As Integer Implements System.Collections.Generic.ICollection(Of WorkflowView).Count
            Get
                Return (_workflow.Count)
            End Get
        End Property

        Public ReadOnly Property IsReadOnly() As Boolean Implements System.Collections.Generic.ICollection(Of WorkflowView).IsReadOnly
            Get
                Return False
            End Get
        End Property

        Default Public Property Item(ByVal index As Integer) As WorkflowView Implements System.Collections.Generic.IList(Of WorkflowView).Item
            Get
                Return _workflow.Item(index)
            End Get
            Set(ByVal value As WorkflowView)
                _workflow.Item(index) = value
            End Set
        End Property

        Public Sub Add(ByVal item As WorkflowView) Implements System.Collections.Generic.ICollection(Of WorkflowView).Add
            _workflow.Add(item)
        End Sub

        Public Sub Clear() Implements System.Collections.Generic.ICollection(Of WorkflowView).Clear
            _workflow.Clear()
        End Sub

        Public Function Contains(ByVal item As WorkflowView) As Boolean Implements System.Collections.Generic.ICollection(Of WorkflowView).Contains
            Return _workflow.Contains(item)
        End Function

        Public Sub CopyTo(ByVal array() As WorkflowView, ByVal arrayIndex As Integer) Implements System.Collections.Generic.ICollection(Of WorkflowView).CopyTo
            _workflow.CopyTo(array)
        End Sub

        Public Function GetEnumerator() As System.Collections.Generic.IEnumerator(Of WorkflowView) Implements System.Collections.Generic.IEnumerable(Of WorkflowView).GetEnumerator
            Return _workflow.GetEnumerator()
        End Function

        Public Function GetEnumerator1() As System.Collections.IEnumerator Implements System.Collections.IEnumerable.GetEnumerator
            Return _workflow.GetEnumerator()
        End Function

        Public Function IndexOf(ByVal item As WorkflowView) As Integer Implements System.Collections.Generic.IList(Of WorkflowView).IndexOf
            Return _workflow.IndexOf(item)
        End Function

        Public Sub Insert(ByVal index As Integer, ByVal item As WorkflowView) Implements System.Collections.Generic.IList(Of WorkflowView).Insert
            _workflow.Insert(index, item)
        End Sub

        Public Function Remove(ByVal item As WorkflowView) As Boolean Implements System.Collections.Generic.ICollection(Of WorkflowView).Remove
            Return _workflow.Remove(item)
        End Function

        Public Sub RemoveAt(ByVal index As Integer) Implements System.Collections.Generic.IList(Of WorkflowView).RemoveAt
            _workflow.RemoveAt(index)
        End Sub

#End Region

    End Class

End Namespace