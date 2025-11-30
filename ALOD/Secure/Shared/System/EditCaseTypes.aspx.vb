Imports ALOD.Core.Domain.Lookup
Imports ALOD.Core.Domain.Workflow
Imports ALOD.Core.Interfaces.DAOInterfaces
Imports ALOD.Data

Namespace Web.Sys

    Public Class EditCaseTypes
        Inherits System.Web.UI.Page

        Private _caseTypeDao As ICaseTypeDao
        Private _workflowDao As IWorkflowDao

        Public ReadOnly Property CaseTypeDao As ICaseTypeDao
            Get
                If (_caseTypeDao Is Nothing) Then
                    _caseTypeDao = New NHibernateDaoFactory().GetCaseTypeDao()
                End If

                Return _caseTypeDao
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

        Protected Sub btnAddCaseType_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnAddCaseType.Click
            If (String.IsNullOrEmpty(txtAddCaseType.Text)) Then
                Exit Sub
            End If

            Dim ct As CaseType = New CaseType()

            ct.Name = Server.HtmlEncode(txtAddCaseType.Text)

            CaseTypeDao.Insert(ct)

            UpdateCaseTypeGridView()

            txtAddCaseType.Text = String.Empty
        End Sub

        Protected Sub btnAddSubCaseType_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnAddSubCaseType.Click
            If (String.IsNullOrEmpty(txtAddSubCaseType.Text)) Then
                Exit Sub
            End If

            Dim ct As CaseType = New CaseType()

            ct.Name = Server.HtmlEncode(txtAddSubCaseType.Text)

            CaseTypeDao.InsertSubCaseType(ct)

            UpdateSubCaseTypeGridView()

            txtAddSubCaseType.Text = String.Empty
        End Sub

        Protected Sub btnCancelSubCaseTypeMappings_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnCancelSubCaseTypeMappings.Click
            ClearEdit()
        End Sub

        Protected Sub btnUpdateSubCaseTypeMappings_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnUpdateSubCaseTypeMappings.Click
            Dim subCaseTypes As List(Of Integer) = New List(Of Integer)()

            If (subCaseTypes Is Nothing) Then
                ClearEdit()
                Exit Sub
            End If

            For Each row As GridViewRow In gdvCaseTypeSubCaseTypes.Rows
                Dim isMapped As Boolean = CType(row.FindControl("chkSubCaseTypeAssociated"), CheckBox).Checked
                Dim subCaseTypeId As Integer = Integer.Parse(CType(row.FindControl("txtSubCaseTypeId"), Label).Text)

                If (isMapped) Then
                    subCaseTypes.Add(subCaseTypeId)
                End If
            Next

            CaseTypeDao.UpdateCaseTypeSubCaseTypeMaps(CInt(ViewState("EditId")), subCaseTypes)

            ClearEdit()
        End Sub

        Protected Sub CancelButton_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnCancelWorkflowMappings.Click
            ClearEdit()
        End Sub

        Protected Sub gdvCaseTypes_RowCancelingEdit(sender As Object, e As System.Web.UI.WebControls.GridViewCancelEditEventArgs) Handles gdvCaseTypes.RowCancelingEdit
            gdvCaseTypes.EditIndex = -1
            UpdateCaseTypeGridView()
        End Sub

        Protected Sub gdvCaseTypes_RowCommand(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewCommandEventArgs) Handles gdvCaseTypes.RowCommand
            Dim parts() As String = e.CommandArgument.ToString().Split("|")

            If (parts.Count < 2) Then
                Exit Sub
            End If

            Dim id As Integer = CInt(parts(0))
            Dim rowIndex As Int16 = CInt(parts(1))

            gdvCaseTypes.SelectedIndex = rowIndex
            ViewState("EditId") = id

            If (e.CommandName = "EditWorkflowMaps") Then
                StartWorkflowMapping(id)
            ElseIf (e.CommandName = "EditSubCaseTypeMaps") Then
                StartSubCaseTypeMapping(id)
            End If
        End Sub

        Protected Sub gdvCaseTypes_RowEditing(sender As Object, e As System.Web.UI.WebControls.GridViewEditEventArgs) Handles gdvCaseTypes.RowEditing
            gdvCaseTypes.EditIndex = e.NewEditIndex
            UpdateCaseTypeGridView()
        End Sub

        Protected Sub gdvCaseTypes_RowUpdating(sender As Object, e As System.Web.UI.WebControls.GridViewUpdateEventArgs) Handles gdvCaseTypes.RowUpdating
            Dim id As Integer = CInt(gdvCaseTypes.DataKeys(e.RowIndex).Value)

            Dim name As String = CType(gdvCaseTypes.Rows(e.RowIndex).FindControl("txtName"), TextBox).Text

            If (String.IsNullOrEmpty(name)) Then
                Exit Sub
            End If

            Dim ct As CaseType = New CaseType()

            ct.Id = id
            ct.Name = Server.HtmlEncode(name)

            CaseTypeDao.Update(ct)

            gdvCaseTypes.EditIndex = -1

            UpdateCaseTypeGridView()
        End Sub

        Protected Sub gdvSubCaseTypes_RowCancelingEdit(sender As Object, e As System.Web.UI.WebControls.GridViewCancelEditEventArgs) Handles gdvSubCaseTypes.RowCancelingEdit
            gdvSubCaseTypes.EditIndex = -1
            UpdateSubCaseTypeGridView()
        End Sub

        Protected Sub gdvSubCaseTypes_RowEditing(sender As Object, e As System.Web.UI.WebControls.GridViewEditEventArgs) Handles gdvSubCaseTypes.RowEditing
            gdvSubCaseTypes.EditIndex = e.NewEditIndex
            UpdateSubCaseTypeGridView()
        End Sub

        Protected Sub gdvSubCaseTypes_RowUpdating(sender As Object, e As System.Web.UI.WebControls.GridViewUpdateEventArgs) Handles gdvSubCaseTypes.RowUpdating
            Dim id As Integer = CInt(gdvSubCaseTypes.DataKeys(e.RowIndex).Value)

            Dim name As String = CType(gdvSubCaseTypes.Rows(e.RowIndex).FindControl("txtSubName"), TextBox).Text

            If (String.IsNullOrEmpty(name)) Then
                Exit Sub
            End If

            Dim ct As CaseType = New CaseType()

            ct.Id = id
            ct.Name = Server.HtmlEncode(name)

            CaseTypeDao.UpdateSubCaseType(ct)

            gdvSubCaseTypes.EditIndex = -1

            UpdateSubCaseTypeGridView()
        End Sub

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
            If (Not IsPostBack) Then
                UpdateCaseTypeGridView()
                UpdateSubCaseTypeGridView()
            End If
        End Sub

        Protected Sub SaveButton_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnUpdateWorkflowMappings.Click
            Dim workflows As List(Of Integer) = New List(Of Integer)()

            If (workflows Is Nothing) Then
                ClearEdit()
                Exit Sub
            End If

            For Each row As GridViewRow In gdvCaseTypeWorkflows.Rows
                Dim isMapped As Boolean = CType(row.FindControl("chkWorkflowAssociated"), CheckBox).Checked
                Dim workflowId As Integer = Integer.Parse(CType(row.FindControl("txtWorkflowId"), Label).Text)

                If (isMapped) Then
                    workflows.Add(workflowId)
                End If
            Next

            CaseTypeDao.UpdateCaseTypeWorkflowMaps(CInt(ViewState("EditId")), workflows)

            ClearEdit()
        End Sub

        Private Sub ClearEdit()
            gdvCaseTypes.SelectedIndex = -1
            gdvCaseTypes.EditIndex = -1
            gdvSubCaseTypes.SelectedIndex = -1
            gdvSubCaseTypes.EditIndex = -1
            ViewState("EditId") = 0
            pnlWorkflowMaps.Visible = False
            pnlSubCaseTypeMaps.Visible = False
        End Sub

        Private Sub StartSubCaseTypeMapping(ByVal id As Integer)
            pnlSubCaseTypeMaps.Visible = True

            Dim ct As CaseType = CaseTypeDao.GetById(id)

            If (ct Is Nothing) Then
                Exit Sub
            End If

            Dim workflowItems As List(Of ALODWebUtility.Worklfow.WorkflowAssociationViewModel) = New List(Of ALODWebUtility.Worklfow.WorkflowAssociationViewModel)()

            For Each sct As CaseType In CaseTypeDao.GetAllSubCaseTypes()
                If (ct.SubCaseTypes.Contains(sct)) Then
                    workflowItems.Add(New ALODWebUtility.Worklfow.WorkflowAssociationViewModel(id, sct.Id, sct.Name, 1))
                Else
                    workflowItems.Add(New ALODWebUtility.Worklfow.WorkflowAssociationViewModel(id, sct.Id, sct.Name, 0))
                End If
            Next

            gdvCaseTypeSubCaseTypes.DataSource = workflowItems
            gdvCaseTypeSubCaseTypes.DataBind()
        End Sub

        Private Sub StartWorkflowMapping(ByVal id As Integer)
            pnlWorkflowMaps.Visible = True
            Dim associatedWorkflows As List(Of Workflow) = CaseTypeDao.GetCaseTypeWorkflows(id)

            Dim workflowItems As List(Of ALODWebUtility.Worklfow.WorkflowAssociationViewModel) = New List(Of ALODWebUtility.Worklfow.WorkflowAssociationViewModel)()

            For Each w As Workflow In WorkflowDao.GetAll()
                If (associatedWorkflows.Contains(w)) Then
                    workflowItems.Add(New ALODWebUtility.Worklfow.WorkflowAssociationViewModel(id, w.Id, w.Title, 1))
                Else
                    workflowItems.Add(New ALODWebUtility.Worklfow.WorkflowAssociationViewModel(id, w.Id, w.Title, 0))
                End If
            Next

            gdvCaseTypeWorkflows.DataSource = workflowItems
            gdvCaseTypeWorkflows.DataBind()

        End Sub

        Private Sub UpdateCaseTypeGridView()
            gdvCaseTypes.DataSource = CaseTypeDao.GetAll()
            gdvCaseTypes.DataBind()
        End Sub

        Private Sub UpdateSubCaseTypeGridView()
            gdvSubCaseTypes.DataSource = CaseTypeDao.GetAllSubCaseTypes()
            gdvSubCaseTypes.DataBind()
        End Sub

    End Class

End Namespace