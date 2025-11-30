Imports ALOD.Core.Domain.Documents
Imports ALOD.Core.Domain.Users
Imports ALOD.Core.Interfaces.DAOInterfaces
Imports ALOD.Data
Imports ALOD.Data.Services
Imports ALODWebUtility.Common

Namespace Web.Admin

    Partial Class Secure_Shared_Admin_MemoTemplates
        Inherits System.Web.UI.Page

        Private dao As IMemoDao

        Private ReadOnly Property MemoStore() As IMemoDao
            Get
                If (dao Is Nothing) Then
                    dao = New NHibernateDaoFactory().GetMemoDao()
                End If
                Return dao
            End Get
        End Property

        Protected Sub CancelEditButton_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles CancelEditButton.Click, CancelRolesButton.Click
            ClearEdit()
        End Sub

        Protected Sub CreateTemplateButton_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles CreateTemplateButton.Click

            ClearEdit()
            EditPanel.Visible = True

            'reset the controls
            MemoTitle.Text = ""
            SourceSelect.SelectedIndex = -1
            Active.Checked = True
            AddDate.Checked = True
            AddSuspenseDate.Checked = False
            AddSignature.Checked = False
            SignatureBlock.Text = ""
            MemoBody.Text = ""
            MemoAttachments.Text = ""

        End Sub

        Protected Sub InitSourceSelect()

            Dim prefixLen As Integer = 8
            Dim sources = MemoStore.GetDataSources()

            For Each source As String In sources
                Dim name As String = source
                name = name.Substring(prefixLen, name.Length - prefixLen)
                name = name.Replace("_", " ")
                SourceSelect.Items.Add(New ListItem(name, source))
            Next

            SourceSelect.Items.Insert(0, New ListItem("None", ""))

        End Sub

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

            If (Not IsPostBack) Then
                SetMaxLength(SignatureBlock)
                InitSourceSelect()
                UpdateTemplateGrid()
            End If
        End Sub

        Protected Sub StartRoleEditing(ByVal id As Integer)

            EditPanel.Visible = False
            RolePanel.Visible = True

            RolesGrid.DataSource = MemoStore.GetRolePermissions(id, Session("Compo"))
            RolesGrid.DataBind()

        End Sub

        Protected Sub StartTemplateEditing(ByVal id As Integer)

            EditPanel.Visible = True
            RolePanel.Visible = False

            Dim memo As MemoTemplate = MemoStore.GetTemplateById(id)

            MemoTitle.Text = memo.Title
            SetDropdownByValue(SourceSelect, memo.DataSource)
            SetDropdownByValue(ModuleSelect, memo.ModuleType)
            Active.Checked = memo.Active
            AddDate.Checked = memo.AddDate
            AddSuspenseDate.Checked = memo.AddSuspenseDate
            AddSignature.Checked = memo.AddSignature
            SignatureBlock.Text = memo.SignatureBlock
            MemoBody.Text = memo.Body
            MemoAttachments.Text = memo.Attachments

        End Sub

        Protected Sub TemplateGrid_RowCommand(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewCommandEventArgs) Handles TemplateGrid.RowCommand

            Dim parts() As String = e.CommandArgument.ToString().Split("|")
            Dim id As Integer = CInt(parts(0))
            Dim rowIndex As Int16 = CInt(parts(1))

            TemplateGrid.SelectedIndex = rowIndex
            ViewState("EditId") = id

            If (e.CommandName = "EditTemplate") Then
                StartTemplateEditing(id)
            ElseIf (e.CommandName = "EditRoles") Then
                StartRoleEditing(id)
            End If

        End Sub

        Protected Sub UpdateRolesButton_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles UpdateRolesButton.Click

            Dim editId As Integer = CInt(ViewState("EditId"))
            Dim data As New DataSet
            Dim table As DataTable = data.Tables.Add("RoleList")
            table.Columns.Add("templateId")
            table.Columns.Add("groupId")
            table.Columns.Add("CanView")
            table.Columns.Add("CanCreate")
            table.Columns.Add("CanEdit")
            table.Columns.Add("CanDelete")

            For Each row As GridViewRow In RolesGrid.Rows
                Dim add As DataRow = table.NewRow()
                add("templateId") = editId
                add("groupId") = RolesGrid.DataKeys(row.RowIndex)("groupId")
                add("CanView") = CType(row.FindControl("CanView"), CheckBox).Checked
                add("CanCreate") = CType(row.FindControl("CanCreate"), CheckBox).Checked
                add("CanEdit") = CType(row.FindControl("CanEdit"), CheckBox).Checked
                add("CanDelete") = CType(row.FindControl("CanDelete"), CheckBox).Checked

                table.Rows.Add(add)
            Next

            MemoStore.UpdateRolePermissions(editId, data)
            MemoStore.CommitChanges()
            ClearEdit()
            UpdateTemplateGrid()

        End Sub

        Protected Sub UpdateTemplateButton_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles UpdateTemplateButton.Click

            If (Not ValidateInputs()) Then
                Exit Sub
            End If

            Dim editId As Integer = 0

            If (ViewState("EditId") IsNot Nothing) Then
                editId = CInt(ViewState("EditId"))
            End If

            Dim user As AppUser = UserService.CurrentUser()
            Dim memo As MemoTemplate

            If (editId = 0) Then
                'new memo
                memo = New MemoTemplate()
                memo.CreatedBy = user
                memo.CreatedDate = DateTime.Now
            Else
                memo = MemoStore.GetTemplateById(editId)
            End If

            memo.Title = MemoTitle.Text.Trim
            memo.Active = Active.Checked
            memo.AddDate = AddDate.Checked
            memo.AddSuspenseDate = AddSuspenseDate.Checked
            memo.AddSignature = AddSignature.Checked
            memo.DataSource = SourceSelect.SelectedValue
            memo.ModuleType = CByte(ModuleSelect.SelectedValue)
            memo.SignatureBlock = SignatureBlock.Text.Trim
            memo.Body = MemoBody.Text
            memo.Attachments = MemoAttachments.Text.Trim()
            If (memo.ModuleType = 30 Or memo.ModuleType = 2) Then 'module 30 is PSCD
                memo.Component = 6
            End If

            memo.ModifiedBy = user
            memo.ModifiedDate = DateTime.Now

            MemoStore.SaveOrUpdateTemplate(memo)

            'because we are rebinding to our updated templates we need to commit them
            'to the db and then flush the updated object to prevent re-binding errors
            MemoStore.CommitChanges()
            MemoStore.EvictTemplate(memo)

            ClearEdit()
            UpdateTemplateGrid()

        End Sub

        Protected Sub UpdateTemplateGrid()

            TemplateGrid.DataSource = From m In MemoStore.GetAllTemplates() Select m Order By m.Id
            TemplateGrid.DataBind()

        End Sub

        Private Sub ClearEdit()
            TemplateGrid.SelectedIndex = -1
            TemplateGrid.EditIndex = -1
            ViewState("EditId") = 0
            EditPanel.Visible = False
            RolePanel.Visible = False
        End Sub

        Private Function ValidateInputs() As Boolean

            Dim passed As Boolean = True

            If (MemoTitle.Text.Trim.Length = 0) Then
                MemoTitle.CssClass = "fieldRequired"
                passed = False
            Else
                MemoTitle.CssClass = ""
            End If

            Return passed

        End Function

    End Class

End Namespace