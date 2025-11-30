Imports ALOD.Core.Domain.Lookup
Imports ALOD.Core.Domain.Workflow
Imports ALOD.Core.Interfaces.DAOInterfaces
Imports ALOD.Data

Namespace Web.Sys

    Public Class EditCompletedByGroups
        Inherits System.Web.UI.Page

        Private _completedByGroupDao As ICompletedByGroupDao
        Private _workflowDao As IWorkflowDao

        Public ReadOnly Property CompletedByGroupDao As ICompletedByGroupDao
            Get
                If (_completedByGroupDao Is Nothing) Then
                    _completedByGroupDao = New NHibernateDaoFactory().GetCompletedByGroupDao()
                End If

                Return _completedByGroupDao
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

        Protected Sub btnAddCaseType_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnAddCompletedByGroup.Click
            If (String.IsNullOrEmpty(txtAddCompletedByGroup.Text)) Then
                Exit Sub
            End If

            Dim cbg As CompletedByGroup = New CompletedByGroup()

            cbg.Name = Server.HtmlEncode(txtAddCompletedByGroup.Text)

            CompletedByGroupDao.Insert(cbg)

            UpdateCompletedByGroupGridView()

            txtAddCompletedByGroup.Text = String.Empty
        End Sub

        Protected Sub CancelButton_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnCancelWorkflowMappings.Click
            ClearEdit()
        End Sub

        Protected Sub gdvCaseTypes_RowCancelingEdit(sender As Object, e As System.Web.UI.WebControls.GridViewCancelEditEventArgs) Handles gdvCompletedByGroups.RowCancelingEdit
            gdvCompletedByGroups.EditIndex = -1
            UpdateCompletedByGroupGridView()
        End Sub

        Protected Sub gdvCaseTypes_RowCommand(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewCommandEventArgs) Handles gdvCompletedByGroups.RowCommand
            Dim parts() As String = e.CommandArgument.ToString().Split("|")

            If (parts.Count < 2) Then
                Exit Sub
            End If

            Dim id As Integer = CInt(parts(0))
            Dim rowIndex As Int16 = CInt(parts(1))

            gdvCompletedByGroups.SelectedIndex = rowIndex
            ViewState("EditId") = id

            If (e.CommandName = "EditWorkflowMaps") Then
                StartWorkflowMapping(id)
            End If
        End Sub

        Protected Sub gdvCaseTypes_RowEditing(sender As Object, e As System.Web.UI.WebControls.GridViewEditEventArgs) Handles gdvCompletedByGroups.RowEditing
            gdvCompletedByGroups.EditIndex = e.NewEditIndex
            UpdateCompletedByGroupGridView()
        End Sub

        Protected Sub gdvCaseTypes_RowUpdating(sender As Object, e As System.Web.UI.WebControls.GridViewUpdateEventArgs) Handles gdvCompletedByGroups.RowUpdating
            Dim id As Integer = CInt(gdvCompletedByGroups.DataKeys(e.RowIndex).Value)

            Dim name As String = CType(gdvCompletedByGroups.Rows(e.RowIndex).FindControl("txtName"), TextBox).Text

            If (String.IsNullOrEmpty(name)) Then
                Exit Sub
            End If

            Dim cbg As CompletedByGroup = New CompletedByGroup()

            cbg.Id = id
            cbg.Name = Server.HtmlEncode(name)

            CompletedByGroupDao.Update(cbg)

            gdvCompletedByGroups.EditIndex = -1

            UpdateCompletedByGroupGridView()
        End Sub

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
            If (Not IsPostBack) Then
                UpdateCompletedByGroupGridView()
            End If
        End Sub

        Protected Sub SaveButton_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnUpdateWorkflowMappings.Click
            Dim workflows As List(Of Integer) = New List(Of Integer)()

            If (workflows Is Nothing) Then
                ClearEdit()
                Exit Sub
            End If

            For Each row As GridViewRow In gdvCompletedByGroupWorkflows.Rows
                Dim isMapped As Boolean = CType(row.FindControl("chkWorkflowAssociated"), CheckBox).Checked
                Dim workflowId As Integer = Integer.Parse(CType(row.FindControl("txtWorkflowId"), Label).Text)

                If (isMapped) Then
                    workflows.Add(workflowId)
                End If
            Next

            CompletedByGroupDao.UpdateCompletedByGroupWorkflowMaps(CInt(ViewState("EditId")), workflows)

            ClearEdit()
        End Sub

        Private Sub ClearEdit()
            gdvCompletedByGroups.SelectedIndex = -1
            gdvCompletedByGroups.EditIndex = -1
            ViewState("EditId") = 0
            pnlWorkflowMaps.Visible = False
        End Sub

        Private Sub StartWorkflowMapping(ByVal id As Integer)
            pnlWorkflowMaps.Visible = True
            Dim associatedWorkflows As List(Of Workflow) = CompletedByGroupDao.GetCompletedByGroupWorkflows(id)

            Dim workflowItems As List(Of ALODWebUtility.Worklfow.WorkflowAssociationViewModel) = New List(Of ALODWebUtility.Worklfow.WorkflowAssociationViewModel)()

            For Each w As Workflow In WorkflowDao.GetAll()
                If (associatedWorkflows.Contains(w)) Then
                    workflowItems.Add(New ALODWebUtility.Worklfow.WorkflowAssociationViewModel(id, w.Id, w.Title, 1))
                Else
                    workflowItems.Add(New ALODWebUtility.Worklfow.WorkflowAssociationViewModel(id, w.Id, w.Title, 0))
                End If
            Next

            gdvCompletedByGroupWorkflows.DataSource = workflowItems
            gdvCompletedByGroupWorkflows.DataBind()

        End Sub

        Private Sub UpdateCompletedByGroupGridView()
            gdvCompletedByGroups.DataSource = CompletedByGroupDao.GetAll()
            gdvCompletedByGroups.DataBind()
        End Sub

    End Class

End Namespace