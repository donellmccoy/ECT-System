Imports ALOD.Core.Domain.Lookup
Imports ALOD.Core.Domain.PsychologicalHealth
Imports ALOD.Core.Interfaces.DAOInterfaces
Imports ALOD.Core.Utils
Imports ALOD.Data
Imports ALODWebUtility.Common

Namespace Web.Sys

    Public Class EditPHFieldTypes
        Inherits System.Web.UI.Page

        Private _lookupDao As ILookupDao
        Private _phDao As IPsychologicalHealthDao

        Public ReadOnly Property LookupDao As ILookupDao
            Get
                If (_lookupDao Is Nothing) Then
                    _lookupDao = New NHibernateDaoFactory().GetLookupDao()
                End If

                Return _lookupDao
            End Get
        End Property

        Public ReadOnly Property PHDao As IPsychologicalHealthDao
            Get
                If (_phDao Is Nothing) Then
                    _phDao = New NHibernateDaoFactory().GetPsychologicalHealthDao()
                End If

                Return _phDao
            End Get
        End Property

        Protected Sub AddNewFieldTypeButton_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnAddNewFieldType.Click
            ClearEdit()
            pnlAddNewFieldType.Visible = True
        End Sub

        Protected Sub btnAddPHFieldType_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnAddPHFieldType.Click
            If (String.IsNullOrEmpty(txtAddPHFieldType.Text)) Then
                Exit Sub
            End If

            If (ddlDataTypes.SelectedValue = 0) Then
                Exit Sub
            End If

            PHDao.InsertFieldType(ConstructNewFieldTypeObject())
            UpdateViews()
            ClearAddNewFieldType()
        End Sub

        Protected Sub btnCancelEdit_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnCancelEdit.Click
            ClearEdit()
            UpdateViews()
        End Sub

        Protected Sub btnEditFormFieldbtnEdit_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnEdit.Click
            If (Not AreEditsValid()) Then
                Exit Sub
            End If

            Dim fieldType As PHFieldType = PHDao.GetFieldTypeById(CInt(ViewState("EditId")))

            If (fieldType Is Nothing) Then
                Exit Sub
            End If

            UpdateFieldType(fieldType)
            ClearEdit()
            UpdateViews()
        End Sub

        Protected Sub CancelAddPHFieldType_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnCancelAddPHFieldType.Click
            ClearAddNewFieldType()
            UpdateViews()
        End Sub

        Protected Sub gdvPHFieldTypes_PageIndexChanging(sender As Object, e As System.Web.UI.WebControls.GridViewPageEventArgs) Handles gdvPHFieldTypes.PageIndexChanging
            gdvPHFieldTypes.PageIndex = e.NewPageIndex
            UpdatePHFieldTypesView()
        End Sub

        Protected Sub gdvPHFieldTypes_RowCancelingEdit(sender As Object, e As System.Web.UI.WebControls.GridViewCancelEditEventArgs) Handles gdvPHFieldTypes.RowCancelingEdit
            gdvPHFieldTypes.EditIndex = -1
            UpdateViews()
        End Sub

        Protected Sub gdvPHFieldTypes_RowCommand(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewCommandEventArgs) Handles gdvPHFieldTypes.RowCommand
            If (e.CommandName.Equals("Page")) Then
                Exit Sub
            End If

            Dim parts() As String = e.CommandArgument.ToString().Split("|")

            If (parts.Count <> 2) Then
                Exit Sub
            End If

            Dim rowIndex As Int16 = CInt(parts(1)) Mod gdvPHFieldTypes.PageSize

            Dim fieldType As PHFieldType = PHDao.GetFieldTypeById(parts(0))

            If (fieldType Is Nothing) Then
                Exit Sub
            End If

            If (e.CommandName = "EditFieldType") Then
                ViewState("EditId") = fieldType.Id

                gdvPHFieldTypes.SelectedIndex = rowIndex
                StartEditFieldType(fieldType)
                UpdateViews()
            End If
        End Sub

        Protected Sub gdvPHFieldTypes_RowDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewRowEventArgs) Handles gdvPHFieldTypes.RowDataBound
            If (e.Row.RowType <> DataControlRowType.DataRow) Then
                Exit Sub
            End If

            Dim fieldType As PHFieldType = PHDao.GetFieldTypeById(CInt(gdvPHFieldTypes.DataKeys(e.Row.RowIndex).Value))

            If (fieldType Is Nothing) Then
                Exit Sub
            End If

            CType(e.Row.FindControl("lblDataType"), Label).Text = LookupDao.GetDataTypeById(fieldType.DataTypeId)
        End Sub

        Protected Sub gdvPHFieldTypes_RowEditing(sender As Object, e As System.Web.UI.WebControls.GridViewEditEventArgs) Handles gdvPHFieldTypes.RowEditing
            gdvPHFieldTypes.EditIndex = e.NewEditIndex
            UpdateViews()
        End Sub

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
            If (Not IsPostBack) Then
                SetInputRestrictionsForControls()
                UpdateViews()
                PopulateDataTypes()
            End If
        End Sub

        Private Function AreEditsValid() As Boolean
            If (String.IsNullOrEmpty(txtEditFieldTypeName.Text)) Then
                Return False
            End If

            If (txtEditColor.Text.Length > 6 OrElse DoesStringContainSpecialCharacters(txtEditColor.Text)) Then
                Return False
            End If

            If (ddlEditDataTypes.SelectedValue = 0) Then
                Return False
            End If

            If (txtEditMaxLength.Text.Length > 6 OrElse DoesStringContainSpecialCharacters(txtEditMaxLength.Text)) Then
                Return False
            End If

            Return True
        End Function

        Private Sub ClearAddNewFieldType()
            pnlAddNewFieldType.Visible = False
            txtAddPHFieldType.Text = String.Empty
            txtDataSource.Text = String.Empty
            txtPlaceholder.Text = String.Empty
            txtMaxLength.Text = String.Empty
            ddlDataTypes.SelectedIndex = 0
        End Sub

        Private Sub ClearEdit()
            gdvPHFieldTypes.SelectedIndex = -1
            gdvPHFieldTypes.EditIndex = -1
            pnlEditFieldType.Visible = False
            txtEditDataSource.Text = String.Empty
            txtEditFieldTypeName.Text = String.Empty
            txtEditPlaceholder.Text = String.Empty
            ddlEditDataTypes.SelectedIndex = 0
            txtEditColor.Text = String.Empty
            txtEditMaxLength.Text = String.Empty
            ViewState("EditId") = 0
        End Sub

        Private Function ConstructNewFieldTypeObject() As PHFieldType
            Dim newFieldType As New PHFieldType()

            newFieldType.Name = Server.HtmlEncode(txtAddPHFieldType.Text)
            newFieldType.DataTypeId = ddlDataTypes.SelectedValue
            newFieldType.Datasource = Server.HtmlEncode(txtDataSource.Text)
            newFieldType.Placeholder = Server.HtmlEncode(txtPlaceholder.Text)

            If (Not String.IsNullOrEmpty(txtColor.Text)) Then
                newFieldType.Color = System.Drawing.Color.FromName("#" & Server.HtmlEncode(txtColor.Text))
            End If

            If (Not String.IsNullOrEmpty(txtMaxLength.Text)) Then
                newFieldType.Length = Server.HtmlEncode(txtMaxLength.Text)
            End If

            Return newFieldType
        End Function

        Private Sub PopulateDataTypes()
            ddlDataTypes.DataSource = LookupDao.GetDataTypes()
            ddlDataTypes.DataValueField = "Id"
            ddlDataTypes.DataTextField = "Name"
            ddlDataTypes.DataBind()

            InsertDropDownListZeroValue(ddlDataTypes, "--- Select a Data Type ---")

            ddlEditDataTypes.DataSource = LookupDao.GetDataTypes()
            ddlEditDataTypes.DataValueField = "Id"
            ddlEditDataTypes.DataTextField = "Name"
            ddlEditDataTypes.DataBind()

            InsertDropDownListZeroValue(ddlEditDataTypes, "--- Select a Data Type ---")
        End Sub

        Private Sub SetInputRestrictionsForControls()
            SetInputFormatRestriction(Page, txtEditColor, FormatRestriction.AlphaNumeric)
            SetInputFormatRestriction(Page, txtEditMaxLength, FormatRestriction.Numeric)
            SetInputFormatRestriction(Page, txtMaxLength, FormatRestriction.Numeric)
        End Sub

        Private Sub StartEditFieldType(ByVal fieldType As PHFieldType)
            ClearAddNewFieldType()

            pnlEditFieldType.Visible = True

            txtEditFieldTypeName.Text = fieldType.Name
            txtEditDataSource.Text = fieldType.Datasource
            txtEditPlaceholder.Text = fieldType.Placeholder
            ddlEditDataTypes.SelectedValue = fieldType.DataTypeId

            If (fieldType.Color.HasValue) Then
                txtEditColor.Text = fieldType.Color.Value.Name.Substring(1)
            End If

            If (fieldType.Length.HasValue) Then
                txtEditMaxLength.Text = fieldType.Length.Value
            End If
        End Sub

        Private Sub UpdateFieldType(fieldType As PHFieldType)
            If (fieldType Is Nothing) Then
                Exit Sub
            End If

            fieldType.Name = Server.HtmlEncode(txtEditFieldTypeName.Text)
            fieldType.DataTypeId = ddlEditDataTypes.SelectedValue

            If (Not String.IsNullOrEmpty(txtEditDataSource.Text)) Then
                fieldType.Datasource = Server.HtmlEncode(txtEditDataSource.Text)
            End If

            If (Not String.IsNullOrEmpty(txtEditPlaceholder.Text)) Then
                fieldType.Placeholder = Server.HtmlEncode(txtEditPlaceholder.Text)
            End If

            If (Not String.IsNullOrEmpty(txtEditColor.Text)) Then
                fieldType.Color = System.Drawing.Color.FromName("#" & Server.HtmlEncode(txtEditColor.Text))
            End If

            If (Not String.IsNullOrEmpty(txtEditMaxLength.Text)) Then
                fieldType.Length = Server.HtmlEncode(txtEditMaxLength.Text)
            End If

            PHDao.UpdateFieldType(fieldType)
        End Sub

        Private Sub UpdatePHFieldTypesView()
            gdvPHFieldTypes.DataSource = PHDao.GetAllFieldTypes()
            gdvPHFieldTypes.DataBind()
        End Sub

        Private Sub UpdateViews()
            UpdatePHFieldTypesView()
        End Sub

    End Class

End Namespace