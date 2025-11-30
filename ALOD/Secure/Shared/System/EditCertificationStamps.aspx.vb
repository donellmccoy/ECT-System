Imports ALOD.Core.Domain.Lookup
Imports ALOD.Core.Domain.Workflow
Imports ALOD.Core.Interfaces.DAOInterfaces
Imports ALOD.Data

Namespace Web.Sys

    Public Class EditCertificationStamps
        Inherits System.Web.UI.Page

        Private _certificationStampDao As ICertificationStampDao
        Private _workflowDao As IWorkflowDao

        Public ReadOnly Property CertificationStampDao As ICertificationStampDao
            Get
                If (_certificationStampDao Is Nothing) Then
                    _certificationStampDao = New NHibernateDaoFactory().GetCertificationStampDao()
                End If

                Return _certificationStampDao
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

        Protected Sub btnAddDisposition_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnAddCertificationStamp.Click
            StartAddingCertificationStamp()
        End Sub

        Protected Sub btnCancelCertificationStamp_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnCancelCertificationStamp.Click
            ClearEdit()
        End Sub

        Protected Sub btnCancelWorkflowMappings_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnCancelWorkflowMappings.Click
            ClearEdit()
        End Sub

        Protected Sub btnInsertCertificationStamp_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnInsertCertificationStamp.Click
            If (Not ValidateCertificationStampInput()) Then
                Exit Sub
            End If

            Dim cs As CertificationStamp = New CertificationStamp()

            If (cs Is Nothing) Then
                Exit Sub
            End If

            cs.Name = Server.HtmlEncode(txtStampName.Text)
            cs.Body = Server.HtmlEncode(txtStampBody.Text)
            cs.IsQualified = chkStampIsQualified.Checked

            CertificationStampDao.Insert(cs)

            ClearEdit()

            UpdateCertificationStampGridView()
        End Sub

        Protected Sub btnUpdateCertificationStamp_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnUpdateCertificationStamp.Click
            If (Not ValidateCertificationStampInput()) Then
                Exit Sub
            End If

            Dim cs As CertificationStamp = New CertificationStamp()

            If (cs Is Nothing) Then
                Exit Sub
            End If

            cs.Id = CInt(ViewState("EditId"))
            cs.Name = Server.HtmlEncode(txtStampName.Text)
            cs.Body = Server.HtmlEncode(txtStampBody.Text)
            cs.IsQualified = chkStampIsQualified.Checked

            CertificationStampDao.Update(cs)

            ClearEdit()

            UpdateCertificationStampGridView()
        End Sub

        Protected Sub btnUpdateWorkflowMappings_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnUpdateWorkflowMappings.Click
            Dim workflows As List(Of Integer) = New List(Of Integer)()

            If (workflows Is Nothing) Then
                ClearEdit()
                Exit Sub
            End If

            For Each row As GridViewRow In gdvCertificationStampWorkflows.Rows
                Dim isMapped As Boolean = CType(row.FindControl("chkAssociated"), CheckBox).Checked
                Dim workflowId As Integer = Integer.Parse(CType(row.FindControl("txtWorkflowId"), Label).Text)

                If (isMapped) Then
                    workflows.Add(workflowId)
                End If
            Next

            CertificationStampDao.UpdateCertificationStampWorkflowsMaps(CInt(ViewState("EditId")), workflows)

            ClearEdit()
        End Sub

        Protected Sub gdvCertificationStamps_RowCommand(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewCommandEventArgs) Handles gdvCertificationStamps.RowCommand
            Dim parts() As String = e.CommandArgument.ToString().Split("|")

            If (parts.Count < 2) Then
                Exit Sub
            End If

            Dim id As Integer = CInt(parts(0))
            Dim rowIndex As Int16 = CInt(parts(1))

            gdvCertificationStamps.SelectedIndex = rowIndex
            ViewState("EditId") = id

            If (e.CommandName = "EditWorkflowMaps") Then
                StartWorkflowMapping(id)
            ElseIf (e.CommandName = "EditCertificationStamp") Then
                StartEditingCertificationStamp(id)
            End If
        End Sub

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
            If (Not IsPostBack) Then
                UpdateCertificationStampGridView()
            End If
        End Sub

        Private Sub ClearEdit()
            gdvCertificationStamps.SelectedIndex = -1
            gdvCertificationStamps.EditIndex = -1
            ViewState("EditId") = 0
            pnlWorkflowMaps.Visible = False
            pnlEditCertificationStamp.Visible = False
            bllValidationErrors.Items.Clear()
            trValidationErrors.Visible = False
        End Sub

        Private Sub StartAddingCertificationStamp()
            pnlEditCertificationStamp.Visible = True
            btnInsertCertificationStamp.Visible = True
            btnUpdateCertificationStamp.Visible = False

            lblEditBlockLabel.Text = "2 - Add Certification Stamp"

            txtStampName.Text = String.Empty
            txtStampBody.Text = String.Empty
            chkStampIsQualified.Checked = False
        End Sub

        Private Sub StartEditingCertificationStamp(ByVal id As Integer)
            pnlEditCertificationStamp.Visible = True
            btnUpdateCertificationStamp.Visible = True
            btnInsertCertificationStamp.Visible = False

            lblEditBlockLabel.Text = "2 - Edit Certification Stamp"

            Dim stamp As CertificationStamp = CertificationStampDao.GetById(id)

            If (stamp Is Nothing) Then
                ClearEdit()
                Exit Sub
            End If

            txtStampName.Text = stamp.Name
            txtStampBody.Text = stamp.Body
            chkStampIsQualified.Checked = stamp.IsQualified
        End Sub

        Private Sub StartWorkflowMapping(ByVal id As Integer)
            pnlWorkflowMaps.Visible = True
            Dim associatedWorkflows As List(Of Workflow) = CertificationStampDao.GetCertificationStampWorkflows(id)

            Dim workflowItems As List(Of ALODWebUtility.Worklfow.WorkflowAssociationViewModel) = New List(Of ALODWebUtility.Worklfow.WorkflowAssociationViewModel)()

            For Each w As Workflow In WorkflowDao.GetAll()
                If (associatedWorkflows.Contains(w)) Then
                    workflowItems.Add(New ALODWebUtility.Worklfow.WorkflowAssociationViewModel(id, w.Id, w.Title, 1))
                Else
                    workflowItems.Add(New ALODWebUtility.Worklfow.WorkflowAssociationViewModel(id, w.Id, w.Title, 0))
                End If
            Next

            gdvCertificationStampWorkflows.DataSource = workflowItems
            gdvCertificationStampWorkflows.DataBind()

        End Sub

        Private Sub UpdateCertificationStampGridView()
            gdvCertificationStamps.DataSource = CertificationStampDao.GetAll()
            gdvCertificationStamps.DataBind()
        End Sub

        Private Function ValidateCertificationStampInput() As Boolean
            Dim isValid As Boolean = True

            trValidationErrors.Visible = False
            bllValidationErrors.Items.Clear()

            If (String.IsNullOrEmpty(txtStampName.Text)) Then
                isValid = False
                bllValidationErrors.Items.Add("Name cannot be empty")
            End If

            If (String.IsNullOrEmpty(txtStampBody.Text)) Then
                isValid = False
                bllValidationErrors.Items.Add("Body cannot be empty")
            End If

            If (Not isValid) Then
                trValidationErrors.Visible = True
            End If

            Return isValid
        End Function

    End Class

End Namespace