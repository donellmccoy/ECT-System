Imports ALOD.Core.Domain.PsychologicalHealth
Imports ALOD.Core.Interfaces.DAOInterfaces
Imports ALOD.Core.Utils
Imports ALOD.Data
Imports ALODWebUtility.Common

Namespace Web.Sys

    Public Class EditPHSections
        Inherits System.Web.UI.Page

        Private _phDao As IPsychologicalHealthDao

        Public ReadOnly Property PHDao As IPsychologicalHealthDao
            Get
                If (_phDao Is Nothing) Then
                    _phDao = New NHibernateDaoFactory().GetPsychologicalHealthDao()
                End If

                Return _phDao
            End Get
        End Property

        Protected Sub AddNewMainSectionButton_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnAddNewMainSection.Click
            ClearAddNewSubSection()
            ClearEdit()
            pnlAddNewMainSection.Visible = True
            UpdateAddNewMainSectionView()
        End Sub

        Protected Sub AddNewSubSectionButton_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnAddNewSubSection.Click
            ClearAddMainbSection()
            ClearEdit()
            pnlAddNewSection.Visible = True
            UpdateAddNewSectionView()
        End Sub

        Protected Sub AddSubSectionMappingButton_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnAddSubSectionMapping.Click
            If (ddlSubSections.SelectedValue = 0) Then
                Exit Sub
            End If
        End Sub

        Protected Sub btnAddMainPHSection_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnAddMainPHSection.Click
            If (String.IsNullOrEmpty(txtMainAddPHSection.Text)) Then
                Exit Sub
            End If

            Dim columns As String = txtMainFieldColumns.Text
            Dim columNumber As Integer = 3

            If (String.IsNullOrEmpty(columns) OrElse Integer.TryParse(columns, columNumber) = False) Then
                Exit Sub
            End If

            If (columNumber <> 2 AndAlso columNumber <> 3) Then
                columNumber = 3
            End If

            PHDao.InsertSection(Server.HtmlEncode(txtMainAddPHSection.Text), String.Empty, columNumber, True, chkHasPageBreak.Checked)

            UpdateViews()

            ClearAddMainbSection()
        End Sub

        Protected Sub btnAddPHSection_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnAddPHSection.Click
            If (String.IsNullOrEmpty(txtAddPHSection.Text)) Then
                Exit Sub
            End If

            Dim columns As String = txtFieldColumns.Text
            Dim columNumber As Integer = 3

            If (String.IsNullOrEmpty(columns) OrElse Integer.TryParse(columns, columNumber) = False) Then
                Exit Sub
            End If

            If (columNumber <> 2 AndAlso columNumber <> 3) Then
                columNumber = 3
            End If

            If (ddlParentSections.SelectedValue > 0) Then
                PHDao.InsertSection(Server.HtmlEncode(txtAddPHSection.Text), ddlParentSections.SelectedItem.Text, columNumber, False, False)
            Else
                PHDao.InsertSection(Server.HtmlEncode(txtAddPHSection.Text), String.Empty, columNumber, False, False)
            End If

            UpdateViews()

            ClearAddNewSubSection()
        End Sub

        Protected Sub btnAddSubSectionMapping_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnAddSubSectionMapping.Click
            If (ddlSubSections.SelectedValue = 0) Then
                Exit Sub
            End If

            If (String.IsNullOrEmpty(txtSubSectionDisplayOrder.Text)) Then
                Exit Sub
            End If

            Dim parentId As Integer = CInt(ViewState("EditId"))

            PHDao.AddSectionChild(parentId, Integer.Parse(ddlSubSections.SelectedValue), Integer.Parse(txtSubSectionDisplayOrder.Text))

            StartSubSectionMapping(parentId)
        End Sub

        Protected Sub CancelAddMainPHSectionButton_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnCancelAddMainPHSection.Click
            ClearAddMainbSection()
            UpdateViews()
        End Sub

        Protected Sub CancelAddPHSectionButton_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnCancelAddPHSection.Click
            ClearAddNewSubSection()
            UpdateViews()
        End Sub

        Protected Sub CancelSubSectioMappingButton_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnCancelSubSectionMapping.Click
            ClearEdit()
        End Sub

        Protected Sub gdvPHMainSections_RowCancelingEdit(sender As Object, e As System.Web.UI.WebControls.GridViewCancelEditEventArgs) Handles gdvPHMainSections.RowCancelingEdit
            gdvPHMainSections.EditIndex = -1
            UpdateViews()
        End Sub

        Protected Sub gdvPHMainSections_RowCommand(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewCommandEventArgs) Handles gdvPHMainSections.RowCommand
            If (e.CommandName.Equals("Page")) Then
                Exit Sub
            End If

            Dim parts() As String = e.CommandArgument.ToString().Split("|")

            If (parts.Count <> 2) Then
                Exit Sub
            End If

            ClearEdit()

            Dim id As Integer = CInt(parts(0))
            Dim rowIndex As Int16 = CInt(parts(1)) Mod gdvPHMainSections.PageSize

            gdvPHMainSections.SelectedIndex = rowIndex
            ViewState("EditId") = id

            If (e.CommandName = "EditSubSections") Then
                StartSubSectionMapping(id)
            End If
        End Sub

        Protected Sub gdvPHMainSections_RowDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewRowEventArgs) Handles gdvPHMainSections.RowDataBound
            If (e.Row.RowType <> DataControlRowType.DataRow) Then
                Exit Sub
            End If

            ' Check if this row is being edited or not
            If e.Row.RowIndex = gdvPHMainSections.EditIndex Then
                Dim gdvTxtDisplayOrder As TextBox = e.Row.FindControl("txtDisplayOrder")
                SetInputFormatRestriction(Page, gdvTxtDisplayOrder, FormatRestriction.Numeric)
            End If
        End Sub

        Protected Sub gdvPHMainSections_RowEditing(sender As Object, e As System.Web.UI.WebControls.GridViewEditEventArgs) Handles gdvPHMainSections.RowEditing
            gdvPHMainSections.EditIndex = e.NewEditIndex
            UpdateViews()
        End Sub

        Protected Sub gdvPHMainSections_RowUpdating(sender As Object, e As System.Web.UI.WebControls.GridViewUpdateEventArgs) Handles gdvPHMainSections.RowUpdating
            Dim id As Integer = CInt(gdvPHMainSections.DataKeys(e.RowIndex).Value)
            Dim name As String = CType(gdvPHMainSections.Rows(e.RowIndex).FindControl("txtName"), TextBox).Text
            Dim columns As String = CType(gdvPHMainSections.Rows(e.RowIndex).FindControl("ddlFieldColumns"), DropDownList).SelectedValue
            Dim displayOrder As String = CType(gdvPHMainSections.Rows(e.RowIndex).FindControl("txtDisplayOrder"), TextBox).Text
            Dim hasPageBreak As Boolean = CType(gdvPHMainSections.Rows(e.RowIndex).Cells(4).Controls(0), CheckBox).Checked

            If (String.IsNullOrEmpty(name)) Then
                Exit Sub
            End If

            If (String.IsNullOrEmpty(displayOrder)) Then
                Exit Sub
            End If

            Dim section As PHSection = PHDao.GetSectionById(id)

            If (section Is Nothing) Then
                Exit Sub
            End If

            section.Name = Server.HtmlEncode(name)
            section.FieldColumns = Integer.Parse(columns)
            section.DisplayOrder = Integer.Parse(displayOrder)
            section.HasPageBreak = hasPageBreak

            PHDao.UpdateSection(section)

            gdvPHMainSections.EditIndex = -1

            UpdatePHMainSectionsView()
        End Sub

        Protected Sub gdvPHSubSections_RowCancelingEdit(sender As Object, e As System.Web.UI.WebControls.GridViewCancelEditEventArgs) Handles gdvPHSubSections.RowCancelingEdit
            gdvPHSubSections.EditIndex = -1
            UpdateViews()
        End Sub

        Protected Sub gdvPHSubSections_RowCommand(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewCommandEventArgs) Handles gdvPHSubSections.RowCommand
            If (e.CommandName.Equals("Page")) Then
                Exit Sub
            End If

            Dim parts() As String = e.CommandArgument.ToString().Split("|")

            If (parts.Count <> 2) Then
                Exit Sub
            End If

            ClearEdit()

            Dim id As Integer = CInt(parts(0))
            Dim rowIndex As Int16 = CInt(parts(1)) Mod gdvPHSubSections.PageSize

            gdvPHSubSections.SelectedIndex = rowIndex
            ViewState("EditId") = id

            If (e.CommandName = "EditSubSections") Then
                StartSubSectionMapping(id)
            End If
        End Sub

        Protected Sub gdvPHSubSections_RowEditing(sender As Object, e As System.Web.UI.WebControls.GridViewEditEventArgs) Handles gdvPHSubSections.RowEditing
            gdvPHSubSections.EditIndex = e.NewEditIndex
            UpdateViews()
        End Sub

        Protected Sub gdvPHSubSections_RowUpdating(sender As Object, e As System.Web.UI.WebControls.GridViewUpdateEventArgs) Handles gdvPHSubSections.RowUpdating
            Dim id As Integer = CInt(gdvPHSubSections.DataKeys(e.RowIndex).Value)
            Dim name As String = CType(gdvPHSubSections.Rows(e.RowIndex).FindControl("txtName"), TextBox).Text
            Dim columns As String = CType(gdvPHSubSections.Rows(e.RowIndex).FindControl("ddlFieldColumns"), DropDownList).SelectedValue

            If (String.IsNullOrEmpty(name)) Then
                Exit Sub
            End If

            Dim section As PHSection = PHDao.GetSectionById(id)

            If (section Is Nothing) Then
                Exit Sub
            End If

            section.Name = Server.HtmlEncode(name)
            section.FieldColumns = Integer.Parse(columns)

            PHDao.UpdateSection(section)

            gdvPHSubSections.EditIndex = -1

            UpdatePHSubSectionsView()
        End Sub

        Protected Sub gdvSubSectionMappings_RowCancelingEdit(sender As Object, e As System.Web.UI.WebControls.GridViewCancelEditEventArgs) Handles gdvSubSectionMappings.RowCancelingEdit
            gdvSubSectionMappings.EditIndex = -1
            UpdatePHSubSectionMappingsView()
        End Sub

        Protected Sub gdvSubSectionMappings_RowDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewRowEventArgs) Handles gdvSubSectionMappings.RowDataBound
            If (e.Row.RowType <> DataControlRowType.DataRow) Then
                Exit Sub
            End If

            ' Check if this row is being edited or not
            If e.Row.RowIndex = gdvSubSectionMappings.EditIndex Then
                Dim gdvTxtDisplayOrder As TextBox = e.Row.FindControl("txtDisplayOrder")
                SetInputFormatRestriction(Page, gdvTxtDisplayOrder, FormatRestriction.Numeric)
            End If
        End Sub

        Protected Sub gdvSubSectionMappings_RowDeleting(sender As Object, e As System.Web.UI.WebControls.GridViewDeleteEventArgs)
            Dim parentId As Integer = CInt(ViewState("EditId"))
            Dim childId As Integer = CInt(gdvSubSectionMappings.DataKeys(e.RowIndex).Value)

            If (parentId = 0 OrElse childId = 0) Then
                Exit Sub
            End If

            PHDao.RemoveSectionChild(parentId, childId)

            StartSubSectionMapping(parentId)
        End Sub

        Protected Sub gdvSubSectionMappings_RowEditing(sender As Object, e As System.Web.UI.WebControls.GridViewEditEventArgs) Handles gdvSubSectionMappings.RowEditing
            gdvSubSectionMappings.EditIndex = e.NewEditIndex
            UpdatePHSubSectionMappingsView()
        End Sub

        Protected Sub gdvSubSectionMappings_RowUpdating(sender As Object, e As System.Web.UI.WebControls.GridViewUpdateEventArgs) Handles gdvSubSectionMappings.RowUpdating
            Dim id As Integer = CInt(gdvSubSectionMappings.DataKeys(e.RowIndex).Value)
            Dim displayOrder As String = CType(gdvSubSectionMappings.Rows(e.RowIndex).FindControl("txtDisplayOrder"), TextBox).Text

            If (String.IsNullOrEmpty(displayOrder)) Then
                Exit Sub
            End If

            Dim section As PHSection = PHDao.GetSectionById(id)

            If (section Is Nothing) Then
                Exit Sub
            End If

            section.DisplayOrder = Integer.Parse(displayOrder)

            PHDao.UpdateSection(section)

            gdvSubSectionMappings.EditIndex = -1

            UpdatePHSubSectionMappingsView()
        End Sub

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
            If (Not IsPostBack) Then
                SetInputFormatRestriction(Page, txtSubSectionDisplayOrder, FormatRestriction.Numeric)

                UpdateViews()
            End If
        End Sub

        Private Sub ClearAddMainbSection()
            pnlAddNewMainSection.Visible = False
            txtMainAddPHSection.Text = String.Empty
            txtMainFieldColumns.Text = String.Empty
            chkHasPageBreak.Checked = False
        End Sub

        Private Sub ClearAddNewSubSection()
            pnlAddNewSection.Visible = False
            txtAddPHSection.Text = String.Empty
        End Sub

        Private Sub ClearEdit()
            gdvPHMainSections.SelectedIndex = -1
            gdvPHMainSections.EditIndex = -1
            gdvPHSubSections.SelectedIndex = -1
            gdvPHSubSections.EditIndex = -1
            ViewState("EditId") = 0
            pnlSubSectionMappings.Visible = False
        End Sub

        Private Sub StartSubSectionMapping(ByVal id As Integer)

            Dim sections As List(Of PHSection) = PHDao.GetAllSections()

            If (sections Is Nothing OrElse sections.Count = 0) Then
                Exit Sub
            End If

            Dim parent As PHSection = sections.Where(Function(x) x.Id = id).First()

            If (parent Is Nothing) Then
                Exit Sub
            End If

            ' Restrict options to section which are not top level and which do not already have a parent...
            ddlSubSections.DataSource = sections.Where(Function(x) x.IsTopLevel = False AndAlso x.Name.Equals(parent.Name) = False AndAlso parent.Children.Contains(x) = False AndAlso x.ParentId = 0)
            ddlSubSections.DataValueField = "Id"
            ddlSubSections.DataTextField = "Name"
            ddlSubSections.DataBind()

            Dim firstItem = New ListItem()
            firstItem.Text = "--- Select a Sub Section ---"
            firstItem.Value = 0
            ddlSubSections.Items.Insert(0, firstItem)

            gdvSubSectionMappings.DataSource = parent.Children.OrderBy(Function(x) x.DisplayOrder).ToList()
            gdvSubSectionMappings.DataBind()
            pnlSubSectionMappings.Visible = True

            ClearAddNewSubSection()

        End Sub

        Private Sub UpdateAddNewMainSectionView()

        End Sub

        Private Sub UpdateAddNewSectionView()
            Dim sections As List(Of PHSection) = PHDao.GetAllSections()
            ddlParentSections.DataSource = sections
            ddlParentSections.DataValueField = "Id"
            ddlParentSections.DataTextField = "Name"
            ddlParentSections.DataBind()

            Dim firstItem = New ListItem()
            firstItem.Text = "No Parent"
            firstItem.Value = 0
            ddlParentSections.Items.Insert(0, firstItem)
        End Sub

        Private Sub UpdatePHMainSectionsView()
            gdvPHMainSections.DataSource = PHDao.GetAllSections().Where(Function(x) x.IsTopLevel = True).OrderBy(Function(x) x.DisplayOrder).ToList()
            gdvPHMainSections.DataBind()
        End Sub

        Private Sub UpdatePHSubSectionMappingsView()

            Dim sections As List(Of PHSection) = PHDao.GetAllSections()

            If (sections Is Nothing OrElse sections.Count = 0) Then
                Exit Sub
            End If

            Dim parent As PHSection = sections.Where(Function(x) x.Id = Integer.Parse(ViewState("EditId"))).First()

            If (parent Is Nothing) Then
                Exit Sub
            End If

            gdvSubSectionMappings.DataSource = parent.Children.OrderBy(Function(x) x.DisplayOrder).ToList()
            gdvSubSectionMappings.DataBind()
        End Sub

        Private Sub UpdatePHSubSectionsView()
            gdvPHSubSections.DataSource = PHDao.GetAllSections().Where(Function(x) x.IsTopLevel = False)
            gdvPHSubSections.DataBind()
        End Sub

        Private Sub UpdateViews()
            UpdatePHMainSectionsView()
            UpdatePHSubSectionsView()
        End Sub

    End Class

End Namespace