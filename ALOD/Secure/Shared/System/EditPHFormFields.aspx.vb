Imports ALOD.Core.Domain.PsychologicalHealth
Imports ALOD.Core.Interfaces.DAOInterfaces
Imports ALOD.Core.Utils
Imports ALOD.Data
Imports ALODWebUtility.Common

Namespace Web.Sys

    Public Class EditPHFormFields
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

        Protected Sub AddNewFieldButton_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnAddNewFormField.Click
            ClearEdit()
            UpdateViews()
            PopulateAddNewFormFieldsDDLs()
            txtFieldDisplayOrder.Text = 1
            txtFieldTypeDisplayOrder.Text = 1
            pnlAddNewFormField.Visible = True
        End Sub

        Protected Sub btnAddPHField_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnAddPHFormField.Click
            If (ddlSection.SelectedValue = 0 OrElse ddlField.SelectedValue = 0 OrElse ddlFieldType.SelectedValue = 0) Then
                Exit Sub
            End If

            If (String.IsNullOrEmpty(txtFieldDisplayOrder.Text) OrElse String.IsNullOrEmpty(txtFieldTypeDisplayOrder.Text)) Then
                Exit Sub
            End If

            Dim fieldDO As Integer = 1
            Dim fieldTypeDO As Integer = 1

            Integer.TryParse(Server.HtmlEncode(txtFieldDisplayOrder.Text), fieldDO)
            Integer.TryParse(Server.HtmlEncode(txtFieldTypeDisplayOrder.Text), fieldTypeDO)

            If (fieldDO < 1 OrElse fieldTypeDO < 1) Then
                Exit Sub
            End If

            PHDao.InsertFormField(ddlSection.SelectedValue, ddlField.SelectedValue, ddlFieldType.SelectedValue, fieldDO, fieldTypeDO, Server.HtmlEncode(txtToolTip.Text))

            UpdateViews()

            ClearAddNewField()
        End Sub

        Protected Sub btnCancelEditFormField_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnCancelEditFormField.Click
            ClearEdit()
            UpdateViews()
        End Sub

        Protected Sub btnEditFormField_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnEditFormField.Click
            If (ddlEditSection.SelectedValue = 0 OrElse ddlEditField.SelectedValue = 0 OrElse ddlEditFieldType.SelectedValue = 0) Then
                Exit Sub
            End If

            If (String.IsNullOrEmpty(txtEditFieldDisplayOrder.Text) OrElse String.IsNullOrEmpty(txtEditFieldTypeDisplayOrder.Text)) Then
                Exit Sub
            End If

            Dim newFieldDisplayOrder = Server.HtmlEncode(txtEditFieldDisplayOrder.Text)
            Dim newFieldTypeDisplayOrder = Server.HtmlEncode(txtEditFieldTypeDisplayOrder.Text)
            Dim newToolTip As String = Server.HtmlEncode(txtEditToolTip.Text)

            Dim oldSectionId As Integer = CInt(ViewState("EditSectionId"))
            Dim oldFieldId As Integer = CInt(ViewState("EditFieldId"))
            Dim oldFieldTypeId As Integer = CInt(ViewState("EditFieldTypeId"))

            Dim oldFormField As PHFormField = PHDao.GetFormFieldByIds(oldSectionId, oldFieldId, oldFieldTypeId)

            If (oldFormField Is Nothing OrElse Not oldFormField.IsValid()) Then
                Exit Sub
            End If

            ' Check if any changes were actually made to this form field...
            If (oldFormField.Section.Id = ddlEditSection.SelectedValue AndAlso
                oldFormField.Field.Id = ddlEditField.SelectedValue AndAlso
                oldFormField.FieldType.Id = ddlEditFieldType.SelectedValue AndAlso
                oldFormField.FieldDisplayOrder = newFieldDisplayOrder AndAlso
                oldFormField.FieldTypeDisplayOrder = newFieldTypeDisplayOrder AndAlso
                oldFormField.ToolTip.Equals(newToolTip)) Then
                Exit Sub
            End If

            Dim editArgs As PHFormFieldUpdateEventArgs = New PHFormFieldUpdateEventArgs()

            editArgs.OldSectionId = oldSectionId
            editArgs.OldFieldId = oldFieldId
            editArgs.OldFieldTypeId = oldFieldTypeId
            editArgs.OldFieldDisplayOrder = oldFormField.FieldDisplayOrder
            editArgs.OldFieldTypeDisplayOrder = oldFormField.FieldTypeDisplayOrder
            editArgs.OldToolTip = oldFormField.ToolTip

            editArgs.NewSectionId = ddlEditSection.SelectedValue
            editArgs.NewFieldId = ddlEditField.SelectedValue
            editArgs.NewFieldTypeId = ddlEditFieldType.SelectedValue
            editArgs.NewFieldDisplayOrder = newFieldDisplayOrder
            editArgs.NewFieldTypeDisplayOrder = newFieldTypeDisplayOrder
            editArgs.NewToolTip = newToolTip

            PHDao.UpdateFormField(editArgs)

            ClearEdit()
            UpdateViews()
        End Sub

        Protected Sub CancelAddPHSectionButton_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnCancelAddPHFormField.Click
            ClearAddNewField()
            UpdateViews()
        End Sub

        Protected Sub ddlEditField_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ddlEditField.SelectedIndexChanged, ddlEditSection.SelectedIndexChanged
            If (ddlEditSection.SelectedValue = 0 OrElse ddlEditField.SelectedValue = 0) Then
                txtEditFieldDisplayOrder.Text = 1
                txtEditFieldTypeDisplayOrder.Text = 1
            Else
                Dim formFields As IList(Of PHFormField) = PHDao.GetFormFieldFieldTypes(Integer.Parse(ddlEditSection.SelectedValue), Integer.Parse(ddlEditField.SelectedValue))

                If (formFields.Count > 0) Then
                    txtEditFieldDisplayOrder.Text = formFields(formFields.Count - 1).FieldDisplayOrder
                    txtEditFieldTypeDisplayOrder.Text = formFields(formFields.Count - 1).FieldTypeDisplayOrder + 1
                Else
                    txtEditFieldDisplayOrder.Text = 1
                    txtEditFieldTypeDisplayOrder.Text = 1
                End If
            End If
        End Sub

        Protected Sub ddlField_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ddlField.SelectedIndexChanged
            If (ddlField.SelectedValue = 0) Then
                txtFieldDisplayOrder.Text = 1
                txtFieldTypeDisplayOrder.Text = 1
                ddlFieldType.SelectedValue = 0
                ddlFieldType.Enabled = False
            Else
                Dim formFields As IList(Of PHFormField) = PHDao.GetFormFieldFieldTypes(Integer.Parse(ddlSection.SelectedValue), Integer.Parse(ddlField.SelectedValue))
                ddlFieldType.Enabled = True

                If (formFields.Count > 0) Then
                    txtFieldDisplayOrder.Text = formFields(0).FieldDisplayOrder
                Else
                    txtFieldDisplayOrder.Text = 1
                End If
            End If
        End Sub

        Protected Sub ddlFieldType_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ddlFieldType.SelectedIndexChanged
            If (ddlFieldType.SelectedValue = 0) Then
                txtFieldTypeDisplayOrder.Text = 1
            Else
                Dim formFields As IList(Of PHFormField) = PHDao.GetFormFieldFieldTypes(Integer.Parse(ddlSection.SelectedValue), Integer.Parse(ddlField.SelectedValue))

                If (formFields.Count > 0) Then
                    txtFieldTypeDisplayOrder.Text = formFields(formFields.Count - 1).FieldTypeDisplayOrder + 1
                Else
                    txtFieldTypeDisplayOrder.Text = 1
                End If
            End If
        End Sub

        Protected Sub ddlFilters_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ddlSectionFilter.SelectedIndexChanged, ddlFieldFilter.SelectedIndexChanged, ddlFieldTypeFilter.SelectedIndexChanged
            UpdatePHFormFieldsView()
        End Sub

        Protected Sub ddlSection_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ddlSection.SelectedIndexChanged
            If (ddlSection.SelectedValue = 0) Then

                ddlField.SelectedValue = 0
                ddlFieldType.SelectedValue = 0

                ddlField.Enabled = False
                ddlFieldType.Enabled = False

                txtFieldDisplayOrder.Text = 1
                txtFieldTypeDisplayOrder.Text = 1
            Else
                ddlField.Enabled = True
            End If
        End Sub

        Protected Sub gdvPHFormFields_PageIndexChanging(sender As Object, e As System.Web.UI.WebControls.GridViewPageEventArgs) Handles gdvPHFormFields.PageIndexChanging
            gdvPHFormFields.PageIndex = e.NewPageIndex
            UpdatePHFormFieldsView()
        End Sub

        Protected Sub gdvPHFormFields_RowCommand(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewCommandEventArgs) Handles gdvPHFormFields.RowCommand
            If (e.CommandName.Equals("Page")) Then
                Exit Sub
            End If

            Dim parts() As String = e.CommandArgument.ToString().Split("|")

            If (parts.Count <> 1) Then
                Exit Sub
            End If

            Dim rowIndex As Int16 = CInt(parts(0)) Mod gdvPHFormFields.PageSize
            Dim sectionId As Integer = Integer.Parse(CType(gdvPHFormFields.Rows(rowIndex).FindControl("hdfSectionId"), HiddenField).Value)
            Dim fieldId As Integer = Integer.Parse(CType(gdvPHFormFields.Rows(rowIndex).FindControl("hdfFieldId"), HiddenField).Value)
            Dim fieldTypeId As Integer = Integer.Parse(CType(gdvPHFormFields.Rows(rowIndex).FindControl("hdfFieldTypeId"), HiddenField).Value)

            Dim formField As PHFormField = PHDao.GetFormFieldByIds(sectionId, fieldId, fieldTypeId)

            If (formField Is Nothing OrElse Not formField.IsValid()) Then
                Exit Sub
            End If

            If (e.CommandName = "EditFormField") Then
                ViewState("EditSectionId") = sectionId
                ViewState("EditFieldId") = fieldId
                ViewState("EditFieldTypeId") = fieldTypeId

                gdvPHFormFields.SelectedIndex = rowIndex
                StartEditFormField(formField)
                UpdateViews()
            ElseIf (e.CommandName = "DeleteFormField") Then
                PHDao.RemoveFormField(formField)
                UpdateViews()
            End If
        End Sub

        Protected Sub gdvPHFormFields_RowDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewRowEventArgs) Handles gdvPHFormFields.RowDataBound
            If (e.Row.RowType <> DataControlRowType.DataRow) Then
                Exit Sub
            End If

            Dim sectionId As Integer = Integer.Parse(CType(e.Row.FindControl("hdfSectionId"), HiddenField).Value)
            Dim fieldId As Integer = Integer.Parse(CType(e.Row.FindControl("hdfFieldId"), HiddenField).Value)
            Dim fieldTypeId As Integer = Integer.Parse(CType(e.Row.FindControl("hdfFieldTypeId"), HiddenField).Value)

            Dim formField As PHFormField = PHDao.GetFormFieldByIds(sectionId, fieldId, fieldTypeId)

            If (formField Is Nothing OrElse Not formField.IsValid()) Then
                Exit Sub
            End If

            ' Check if this row is being edited or not
            If e.Row.RowIndex <> gdvPHFormFields.EditIndex Then
                CType(e.Row.FindControl("lblSectionName"), Label).Text = formField.Section.Name
                CType(e.Row.FindControl("lblFieldName"), Label).Text = formField.Field.Name
                CType(e.Row.FindControl("lblFieldTypeName"), Label).Text = formField.FieldType.Name
            End If
        End Sub

        Protected Sub gdvPHFormFields_RowDeleting(sender As Object, e As System.Web.UI.WebControls.GridViewDeleteEventArgs) Handles gdvPHFormFields.RowDeleting
            Dim sectionId As Integer = Integer.Parse(CType(gdvPHFormFields.Rows(e.RowIndex).FindControl("hdfSectionId"), HiddenField).Value)
            Dim fieldId As Integer = Integer.Parse(CType(gdvPHFormFields.Rows(e.RowIndex).FindControl("hdfFieldId"), HiddenField).Value)
            Dim fieldTypeId As Integer = Integer.Parse(CType(gdvPHFormFields.Rows(e.RowIndex).FindControl("hdfFieldTypeId"), HiddenField).Value)

            Dim formField As PHFormField = PHDao.GetFormFieldByIds(sectionId, fieldId, fieldTypeId)

            If (formField Is Nothing OrElse Not formField.IsValid()) Then
                Exit Sub
            End If

            PHDao.RemoveFormField(formField)

            UpdateViews()
        End Sub

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
            If (Not IsPostBack) Then
                SetInputFormatRestriction(Page, txtFieldDisplayOrder, FormatRestriction.Numeric)
                SetInputFormatRestriction(Page, txtFieldTypeDisplayOrder, FormatRestriction.Numeric)
                SetInputFormatRestriction(Page, txtEditFieldDisplayOrder, FormatRestriction.Numeric)
                SetInputFormatRestriction(Page, txtEditFieldTypeDisplayOrder, FormatRestriction.Numeric)
                SetInputFormatRestriction(Page, txtToolTip, FormatRestriction.AlphaNumeric, PERMITTED_SPECIAL_CHAR_INPUT)
                SetInputFormatRestriction(Page, txtEditToolTip, FormatRestriction.AlphaNumeric, PERMITTED_SPECIAL_CHAR_INPUT)

                txtToolTip.MaxLength = 100
                txtEditToolTip.MaxLength = 100

                ddlField.Enabled = False
                ddlFieldType.Enabled = False

                PopulateFilterDDLs()
                UpdateViews()
            End If
        End Sub

        Private Function ApplyFilters(ByVal formFields As IList(Of PHFormField)) As IList(Of PHFormField)
            Dim results As List(Of PHFormField) = formFields

            ' Apply Section filter if necessary...
            If (ddlSectionFilter.SelectedValue <> 0) Then
                results = (From ff In results
                           Where ff.Section.Id = ddlSectionFilter.SelectedValue
                           Select ff).ToList()
            End If

            ' Apply Field filter if necessary...
            If (ddlFieldFilter.SelectedValue <> 0) Then
                results = (From ff In results
                           Where ff.Field.Id = ddlFieldFilter.SelectedValue
                           Select ff).ToList()
            End If

            ' Apply Field Type filter if necessary...
            If (ddlFieldTypeFilter.SelectedValue <> 0) Then
                results = (From ff In results
                           Where ff.FieldType.Id = ddlFieldTypeFilter.SelectedValue
                           Select ff).ToList()
            End If

            Return results
        End Function

        Private Sub ClearAddNewField()
            pnlAddNewFormField.Visible = False
            ddlSection.SelectedValue = 0
            ddlField.SelectedValue = 0
            ddlFieldType.SelectedValue = 0
            ddlField.Enabled = False
            ddlFieldType.Enabled = False
            txtFieldDisplayOrder.Text = 1
            txtFieldTypeDisplayOrder.Text = 1
            txtToolTip.Text = String.Empty
        End Sub

        Private Sub ClearEdit()
            gdvPHFormFields.SelectedIndex = -1
            gdvPHFormFields.EditIndex = -1
            pnlEditFormField.Visible = False
            txtEditFieldDisplayOrder.Text = String.Empty
            txtEditFieldTypeDisplayOrder.Text = String.Empty
            txtEditToolTip.Text = String.Empty
            ViewState("EditSectionId") = 0
            ViewState("EditFieldId") = 0
            ViewState("EditFieldTypeId") = 0
        End Sub

        Private Sub PopulateAddNewFormFieldsDDLs()
            ' PH Sections
            ddlSection.DataSource = PHDao.GetAllSections()
            ddlSection.DataValueField = "Id"
            ddlSection.DataTextField = "Name"
            ddlSection.DataBind()

            InsertDropDownListZeroValue(ddlSection, "--- Select a Section ---")

            ' PH Fields
            ddlField.DataSource = PHDao.GetAllFields()
            ddlField.DataValueField = "Id"
            ddlField.DataTextField = "Name"
            ddlField.DataBind()

            InsertDropDownListZeroValue(ddlField, "--- Select a Field Name ---")

            ' PH Field Types
            ddlFieldType.DataSource = PHDao.GetAllFieldTypes()
            ddlFieldType.DataValueField = "Id"
            ddlFieldType.DataTextField = "Name"
            ddlFieldType.DataBind()

            InsertDropDownListZeroValue(ddlFieldType, "--- Select a Field Type ---")
        End Sub

        Private Sub PopulateEditFormFieldDDLs()
            ' PH Sections
            ddlEditSection.DataSource = PHDao.GetAllSections()
            ddlEditSection.DataValueField = "Id"
            ddlEditSection.DataTextField = "Name"
            ddlEditSection.DataBind()

            ' PH Fields
            ddlEditField.DataSource = PHDao.GetAllFields()
            ddlEditField.DataValueField = "Id"
            ddlEditField.DataTextField = "Name"
            ddlEditField.DataBind()

            ' PH Field Types
            ddlEditFieldType.DataSource = PHDao.GetAllFieldTypes()
            ddlEditFieldType.DataValueField = "Id"
            ddlEditFieldType.DataTextField = "Name"
            ddlEditFieldType.DataBind()
        End Sub

        Private Sub PopulateFilterDDLs()
            ' PH Sections
            ddlSectionFilter.DataSource = PHDao.GetAllSections()
            ddlSectionFilter.DataValueField = "Id"
            ddlSectionFilter.DataTextField = "Name"
            ddlSectionFilter.DataBind()

            InsertDropDownListZeroValue(ddlSectionFilter, "--- All Sections ---")

            ' PH Fields
            ddlFieldFilter.DataSource = PHDao.GetAllFields()
            ddlFieldFilter.DataValueField = "Id"
            ddlFieldFilter.DataTextField = "Name"
            ddlFieldFilter.DataBind()

            InsertDropDownListZeroValue(ddlFieldFilter, "--- All Field Names ---")

            ' PH Field Types
            ddlFieldTypeFilter.DataSource = PHDao.GetAllFieldTypes()
            ddlFieldTypeFilter.DataValueField = "Id"
            ddlFieldTypeFilter.DataTextField = "Name"
            ddlFieldTypeFilter.DataBind()

            InsertDropDownListZeroValue(ddlFieldTypeFilter, "--- All Field Types ---")
        End Sub

        Private Sub StartEditFormField(ByVal formField As PHFormField)
            ClearAddNewField()

            PopulateEditFormFieldDDLs()

            pnlEditFormField.Visible = True

            ddlEditSection.SelectedValue = formField.Section.Id
            ddlEditField.SelectedValue = formField.Field.Id
            ddlEditFieldType.SelectedValue = formField.FieldType.Id
            txtEditFieldDisplayOrder.Text = formField.FieldDisplayOrder
            txtEditFieldTypeDisplayOrder.Text = formField.FieldTypeDisplayOrder
            txtEditToolTip.Text = formField.ToolTip
        End Sub

        Private Sub UpdatePHFormFieldsView()
            gdvPHFormFields.DataSource = ApplyFilters(PHDao.GetAllFormFields())
            gdvPHFormFields.DataBind()
        End Sub

        Private Sub UpdateViews()
            UpdatePHFormFieldsView()
        End Sub

    End Class

End Namespace