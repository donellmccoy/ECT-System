Imports ALOD.Core.Domain.Workflow
Imports ALOD.Core.Interfaces.DAOInterfaces
Imports ALOD.Data

Namespace Web.Sys

    Public Class EditDispositionLookup
        Inherits System.Web.UI.Page

        Private _dispositionDao As ILookupDispositionDao
        Private _workflowDao As IWorkflowDao

        Public ReadOnly Property DispositionDao As ILookupDispositionDao
            Get
                If (_dispositionDao Is Nothing) Then
                    _dispositionDao = New NHibernateDaoFactory().GetLookupDispositionDao()
                End If

                Return _dispositionDao
            End Get
        End Property

        Public ReadOnly Property WorkflowDao As IWorkflowDao
            Get
                If (_workflowDao Is Nothing) Then
                    _workflowDao = New NHibernateDaoFactory().GetWorkflowDao()
                End If

                Return _workflowDao
            End Get
        End Property

        Protected Sub btnAddDisposition_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnAddDisposition.Click
            If (String.IsNullOrEmpty(txtAddDisposition.Text)) Then
                Exit Sub
            End If

            DispositionDao.InsertDisposition(Server.HtmlEncode(txtAddDisposition.Text))

            UpdateDispositionGridView()
        End Sub

        Protected Sub CancelButton_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnCancelWorkflowMappings.Click
            ClearEdit()
        End Sub

        Protected Sub gdvDispositions_RowCancelingEdit(sender As Object, e As System.Web.UI.WebControls.GridViewCancelEditEventArgs) Handles gdvDispositions.RowCancelingEdit
            gdvDispositions.EditIndex = -1
            UpdateDispositionGridView()
            'ErrorPanel.Visible = False
        End Sub

        Protected Sub gdvDispositions_RowCommand(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewCommandEventArgs) Handles gdvDispositions.RowCommand
            If (e.CommandName = "EditWorkflowMaps") Then
                Dim parts() As String = e.CommandArgument.ToString().Split("|")
                Dim id As Integer = CInt(parts(0))
                Dim rowIndex As Int16 = CInt(parts(1))

                gdvDispositions.SelectedIndex = rowIndex
                ViewState("EditId") = id

                StartWorkflowMapping(id)
            End If
        End Sub

        Protected Sub gdvDispositions_RowEditing(sender As Object, e As System.Web.UI.WebControls.GridViewEditEventArgs) Handles gdvDispositions.RowEditing
            gdvDispositions.EditIndex = e.NewEditIndex
            UpdateDispositionGridView()
            'ErrorPanel.Visible = False
        End Sub

        Protected Sub gdvDispositions_RowUpdating(sender As Object, e As System.Web.UI.WebControls.GridViewUpdateEventArgs) Handles gdvDispositions.RowUpdating
            Dim dispositionId As Integer = CInt(gdvDispositions.DataKeys(e.RowIndex).Value)

            Dim dispositionName As String = CType(gdvDispositions.Rows(e.RowIndex).FindControl("txtName"), TextBox).Text

            DispositionDao.UpdateDisposition(dispositionId, dispositionName)

            gdvDispositions.EditIndex = -1

            UpdateDispositionGridView()
        End Sub

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
            If (Not IsPostBack) Then
                UpdateDispositionGridView()
            End If
        End Sub

        Protected Sub SaveButton_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnUpdateWorkflowMappings.Click
            Dim workflows As List(Of Integer) = New List(Of Integer)()

            If (workflows Is Nothing) Then
                ClearEdit()
                Exit Sub
            End If

            For Each row As GridViewRow In gdvDispositionWorkflows.Rows
                Dim isMapped As Boolean = CType(row.FindControl("chkAssociated"), CheckBox).Checked
                Dim workflowId As Integer = Integer.Parse(CType(row.FindControl("txtWorkflowId"), Label).Text)

                If (isMapped) Then
                    workflows.Add(workflowId)
                End If
            Next

            DispositionDao.UpdateDispositionWorkflowsMaps(CInt(ViewState("EditId")), workflows)

            ClearEdit()
        End Sub

        Private Sub ClearEdit()
            gdvDispositions.SelectedIndex = -1
            gdvDispositions.EditIndex = -1
            ViewState("EditId") = 0
            pnlWorkflowMaps.Visible = False
        End Sub

        Private Sub InitPageControls()
            pnlWorkflowMaps.Visible = False
        End Sub

        Private Sub StartWorkflowMapping(ByVal id As Integer)
            pnlWorkflowMaps.Visible = True
            Dim associatedWorkflows As List(Of Workflow) = DispositionDao.GetDispositionWorkflows(id)

            Dim workflowItems As List(Of ALODWebUtility.Worklfow.WorkflowAssociationViewModel) = New List(Of ALODWebUtility.Worklfow.WorkflowAssociationViewModel)()

            For Each w As Workflow In WorkflowDao.GetAll()
                If (associatedWorkflows.Contains(w)) Then
                    workflowItems.Add(New ALODWebUtility.Worklfow.WorkflowAssociationViewModel(id, w.Id, w.Title, 1))
                Else
                    workflowItems.Add(New ALODWebUtility.Worklfow.WorkflowAssociationViewModel(id, w.Id, w.Title, 0))
                End If
            Next

            gdvDispositionWorkflows.DataSource = workflowItems
            gdvDispositionWorkflows.DataBind()

        End Sub

        Private Sub UpdateDispositionGridView()
            gdvDispositions.DataSource = DispositionDao.GetAll()
            gdvDispositions.DataBind()
        End Sub

    End Class

End Namespace