Imports ALOD.Core.Domain.PsychologicalHealth
Imports ALOD.Core.Interfaces.DAOInterfaces
Imports ALOD.Data

Namespace Web.Sys

    Public Class EditPHFields
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

        Protected Sub AddNewFieldButton_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnAddNewField.Click
            ClearEdit()
            pnlAddNewField.Visible = True
        End Sub

        Protected Sub btnAddPHField_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnAddPHField.Click
            If (String.IsNullOrEmpty(txtAddPHField.Text)) Then
                Exit Sub
            End If

            PHDao.InsertField(Server.HtmlEncode(txtAddPHField.Text))

            UpdateViews()

            ClearAddNewField()
        End Sub

        Protected Sub CancelAddPHSectionButton_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnCancelAddPHField.Click
            ClearAddNewField()
            UpdateViews()
        End Sub

        Protected Sub gdvPHFields_PageIndexChanging(sender As Object, e As System.Web.UI.WebControls.GridViewPageEventArgs) Handles gdvPHFields.PageIndexChanging
            gdvPHFields.PageIndex = e.NewPageIndex
            UpdatePHFieldsView()
        End Sub

        Protected Sub gdvPHFields_RowCancelingEdit(sender As Object, e As System.Web.UI.WebControls.GridViewCancelEditEventArgs) Handles gdvPHFields.RowCancelingEdit
            gdvPHFields.EditIndex = -1
            UpdateViews()
        End Sub

        Protected Sub gdvPHFields_RowEditing(sender As Object, e As System.Web.UI.WebControls.GridViewEditEventArgs) Handles gdvPHFields.RowEditing
            gdvPHFields.EditIndex = e.NewEditIndex
            UpdateViews()
        End Sub

        Protected Sub gdvPHFields_RowUpdating(sender As Object, e As System.Web.UI.WebControls.GridViewUpdateEventArgs) Handles gdvPHFields.RowUpdating
            Dim id As Integer = CInt(gdvPHFields.DataKeys(e.RowIndex).Value)
            Dim name As String = CType(gdvPHFields.Rows(e.RowIndex).FindControl("txtName"), TextBox).Text

            If (String.IsNullOrEmpty(name)) Then
                Exit Sub
            End If

            Dim field As PHField = PHDao.GetFieldById(id)

            If (field Is Nothing) Then
                Exit Sub
            End If

            field.Name = Server.HtmlEncode(name)

            PHDao.UpdateField(field)

            gdvPHFields.EditIndex = -1

            UpdateViews()
        End Sub

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
            If (Not IsPostBack) Then
                UpdateViews()
            End If
        End Sub

        Private Sub ClearAddNewField()
            pnlAddNewField.Visible = False
            txtAddPHField.Text = String.Empty
        End Sub

        Private Sub ClearEdit()
            gdvPHFields.SelectedIndex = -1
            gdvPHFields.EditIndex = -1
            ViewState("EditId") = 0
        End Sub

        Private Sub UpdatePHFieldsView()
            gdvPHFields.DataSource = PHDao.GetAllFields()
            gdvPHFields.DataBind()
        End Sub

        Private Sub UpdateViews()
            UpdatePHFieldsView()
        End Sub

    End Class

End Namespace