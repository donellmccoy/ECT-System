Imports ALOD.Data
Imports ALODWebUtility.PageTitles

Namespace Worklfow

    Public Class WorkflowView
        Inherits PageTitle

#Region "members and properties"

        Private _workflowId As Integer = 0
        Private _workflowTitle As String = String.Empty

        Public Property WorkflowId() As Integer
            Get
                Return _workflowId
            End Get
            Set(ByVal value As Integer)
                _workflowId = value
            End Set
        End Property

        Public Property WorkflowTitle() As String
            Get
                Return _workflowTitle
            End Get
            Set(ByVal value As String)
                _workflowTitle = value
            End Set
        End Property

#End Region

        Public Sub New()
        End Sub

        Public Sub InsertWorkflowView()
            Dim adapter As New SqlDataStore()
            Dim cmd As System.Data.Common.DbCommand = adapter.GetStoredProcCommand("core_workflowView_sp_Insert", _pageId, _workflowId)
            adapter.ExecuteNonQuery(cmd)
        End Sub

    End Class

End Namespace