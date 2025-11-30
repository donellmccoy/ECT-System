Imports ALOD.Core.Utils
Imports ALOD.Data
Imports ALODWebUtility.Common

Namespace Web.Sys

    Public Class EditMedical
        Inherits System.Web.UI.Page

        Private _listEditDao As DropDownListEditDao

        Public ReadOnly Property listEditDao As DropDownListEditDao
            Get
                If (_listEditDao Is Nothing) Then
                    _listEditDao = New DropDownListEditDao
                End If

                Return _listEditDao
            End Get
        End Property

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
            If (Not IsPostBack) Then
                UpdateMemberComponentGridView()
                UpdateMemberCategoryGridView()
                UpdateMemberStatusGridView()
                UpdateFromLocationGridView()
                UpdateFacilityGridView()
                UpdateInfluenceGridView()
                UpdateSourceGridView()

                SetInputFormatRestriction(Page, txtSortOrderComponent, FormatRestriction.Numeric)
                SetInputFormatRestriction(Page, txtSortOrderCategory, FormatRestriction.Numeric)
                SetInputFormatRestriction(Page, txtSortOrderStatus, FormatRestriction.Numeric)
                SetInputFormatRestriction(Page, txtSortOrderFrom, FormatRestriction.Numeric)
                SetInputFormatRestriction(Page, txtSortOrderFacility, FormatRestriction.Numeric)
                SetInputFormatRestriction(Page, txtSortOrderInfluence, FormatRestriction.Numeric)

            End If
        End Sub

#Region "Source Information"

        Protected Sub btnAddSource_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnAddSource.Click
            Dim sort_order As Integer

            If (String.IsNullOrEmpty(txtAddSource.Text)) Then
                Exit Sub
            End If

            Try
                sort_order = CInt(txtSortOrderSource.Text)
            Catch
                Exit Sub
            End Try

            listEditDao.InsertDropDownList("core_lookUps_sp_InsertInfoSource", Server.HtmlEncode(txtAddSource.Text), "", sort_order)

            UpdateSourceGridView()

            txtAddSource.Text = String.Empty
            txtSortOrderSource.Text = String.Empty
        End Sub

        Protected Sub gdvUnitSource_RowCancelingEdit(sender As Object, e As System.Web.UI.WebControls.GridViewCancelEditEventArgs) Handles gdvUnitSource.RowCancelingEdit
            gdvUnitSource.EditIndex = -1
            UpdateSourceGridView()
        End Sub

        Protected Sub gdvUnitSource_RowEditing(sender As Object, e As System.Web.UI.WebControls.GridViewEditEventArgs) Handles gdvUnitSource.RowEditing
            gdvUnitSource.EditIndex = e.NewEditIndex
            UpdateSourceGridView()
        End Sub

        Protected Sub gdvUnitSource_RowUpdating(sender As Object, e As System.Web.UI.WebControls.GridViewUpdateEventArgs) Handles gdvUnitSource.RowUpdating
            Dim id As Integer? = Server.HtmlEncode(CInt(gdvUnitSource.DataKeys(e.RowIndex).Value))
            Dim sort_order As Integer? = Nothing

            Try
                sort_order = Convert.ToInt32(CType(gdvUnitSource.Rows(e.RowIndex).FindControl("txtSortSource"), TextBox).Text)
            Catch
                Exit Sub
            End Try

            Dim name As String = Server.HtmlEncode(CType(gdvUnitSource.Rows(e.RowIndex).FindControl("txtSource"), TextBox).Text)

            If (String.IsNullOrEmpty(name)) Then
                Exit Sub
            End If

            If (sort_order Is Nothing) Then
                Exit Sub
            End If

            If (id Is Nothing) Then
                Exit Sub
            End If

            listEditDao.UpdateDropDownList("core_lookUps_sp_UpdateInfoSource", id, name, "", sort_order)

            gdvUnitSource.EditIndex = -1

            UpdateSourceGridView()
        End Sub

        Private Sub UpdateSourceGridView()
            gdvUnitSource.DataSource = listEditDao.GetDropDownList("core_lookUps_sp_InfoSources")
            gdvUnitSource.DataBind()
        End Sub

#End Region

#Region "Component"

        Protected Sub btnAddComponent_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnAddComponent.Click
            Dim sort_order As Integer

            If (String.IsNullOrEmpty(txtAddComponent.Text)) Then
                Exit Sub
            End If

            Try
                sort_order = CInt(txtSortOrderComponent.Text)
            Catch
                Exit Sub
            End Try

            listEditDao.InsertDropDownList("core_lookUps_sp_InsertComponent", Server.HtmlEncode(txtAddComponent.Text), "", sort_order)

            UpdateMemberComponentGridView()

            txtAddComponent.Text = String.Empty
            txtSortOrderComponent.Text = String.Empty
        End Sub

        Protected Sub gdvMedicalComponent_RowCancelingEdit(sender As Object, e As System.Web.UI.WebControls.GridViewCancelEditEventArgs) Handles gdvMedicalComponent.RowCancelingEdit
            gdvMedicalComponent.EditIndex = -1
            UpdateMemberComponentGridView()
        End Sub

        Protected Sub gdvMedicalComponent_RowEditing(sender As Object, e As System.Web.UI.WebControls.GridViewEditEventArgs) Handles gdvMedicalComponent.RowEditing
            gdvMedicalComponent.EditIndex = e.NewEditIndex
            UpdateMemberComponentGridView()
        End Sub

        Protected Sub gdvMedicalComponent_RowUpdating(sender As Object, e As System.Web.UI.WebControls.GridViewUpdateEventArgs) Handles gdvMedicalComponent.RowUpdating
            Dim id As Integer? = Server.HtmlEncode(CInt(gdvMedicalComponent.DataKeys(e.RowIndex).Value))
            Dim sort_order As Integer? = Nothing

            Try
                sort_order = Convert.ToInt32(CType(gdvMedicalComponent.Rows(e.RowIndex).FindControl("txtComponentsort"), TextBox).Text)
            Catch
                Exit Sub
            End Try

            Dim name As String = Server.HtmlEncode(CType(gdvMedicalComponent.Rows(e.RowIndex).FindControl("txtComponent"), TextBox).Text)

            If (String.IsNullOrEmpty(name)) Then
                Exit Sub
            End If

            If (sort_order Is Nothing) Then
                Exit Sub
            End If

            If (id Is Nothing) Then
                Exit Sub
            End If

            listEditDao.UpdateDropDownList("core_lookUps_sp_UpdateComponent", id, name, "", sort_order)

            gdvMedicalComponent.EditIndex = -1

            UpdateMemberComponentGridView()
        End Sub

        Private Sub UpdateMemberComponentGridView()
            gdvMedicalComponent.DataSource = listEditDao.GetDropDownList("core_lookUps_sp_Component")
            gdvMedicalComponent.DataBind()
        End Sub

#End Region

#Region "Category"

        Protected Sub btnAddCategory_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnAddCategory.Click
            Dim sort_order As Integer

            If (String.IsNullOrEmpty(txtAddCategory.Text)) Then
                Exit Sub
            End If

            Try
                sort_order = CInt(txtSortOrderCategory.Text)
            Catch
                Exit Sub
            End Try

            listEditDao.InsertDropDownList("core_lookUps_sp_InsertCategory", Server.HtmlEncode(txtAddCategory.Text), "", sort_order)

            UpdateMemberCategoryGridView()

            txtAddCategory.Text = String.Empty
            txtSortOrderCategory.Text = String.Empty
        End Sub

        Protected Sub gdvMedicalCategory_RowCancelingEdit(sender As Object, e As System.Web.UI.WebControls.GridViewCancelEditEventArgs) Handles gdvMedicalCategory.RowCancelingEdit
            gdvMedicalCategory.EditIndex = -1
            UpdateMemberCategoryGridView()
        End Sub

        Protected Sub gdvMedicalCategory_RowEditing(sender As Object, e As System.Web.UI.WebControls.GridViewEditEventArgs) Handles gdvMedicalCategory.RowEditing
            gdvMedicalCategory.EditIndex = e.NewEditIndex
            UpdateMemberCategoryGridView()
        End Sub

        Protected Sub gdvMedicalCategory_RowUpdating(sender As Object, e As System.Web.UI.WebControls.GridViewUpdateEventArgs) Handles gdvMedicalCategory.RowUpdating
            Dim sort_order As Integer? = Nothing

            Dim id As Integer? = Server.HtmlEncode(CInt(gdvMedicalCategory.DataKeys(e.RowIndex).Value))

            Try
                sort_order = Server.HtmlEncode(CType(gdvMedicalCategory.Rows(e.RowIndex).FindControl("txtCategorysort"), TextBox).Text)
            Catch
                Exit Sub
            End Try

            Dim name As String = Server.HtmlEncode(CType(gdvMedicalCategory.Rows(e.RowIndex).FindControl("txtCategory"), TextBox).Text)

            If (String.IsNullOrEmpty(name)) Then
                Exit Sub
            End If

            If (sort_order Is Nothing) Then
                Exit Sub
            End If

            If (id Is Nothing) Then
                Exit Sub
            End If

            listEditDao.UpdateDropDownList("core_lookUps_sp_UpdateCategory", id, name, "", sort_order)

            gdvMedicalCategory.EditIndex = -1

            UpdateMemberCategoryGridView()
        End Sub

        Private Sub UpdateMemberCategoryGridView()
            gdvMedicalCategory.DataSource = listEditDao.GetDropDownList("core_lookUps_sp_MemberCategory")
            gdvMedicalCategory.DataBind()
        End Sub

#End Region

#Region "Status"

        Protected Sub btnAddStatus_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnAddStatus.Click
            Dim sort_order As Integer

            If (String.IsNullOrEmpty(txtAddStatus.Text)) Then
                Exit Sub
            End If

            If (String.IsNullOrEmpty(txtTypeStatus.Text)) Then
                Exit Sub
            End If

            Try
                sort_order = CInt(txtSortOrderStatus.Text)
            Catch
                Exit Sub
            End Try

            listEditDao.InsertDropDownList("core_lookUps_sp_InsertStatus", Server.HtmlEncode(txtAddStatus.Text), Server.HtmlEncode(txtTypeStatus.Text), sort_order)

            UpdateMemberStatusGridView()

            txtAddStatus.Text = String.Empty
            txtSortOrderStatus.Text = String.Empty
            txtTypeStatus.Text = String.Empty
        End Sub

        Protected Sub gdvMedicalStatus_RowCancelingEdit(sender As Object, e As System.Web.UI.WebControls.GridViewCancelEditEventArgs) Handles gdvMedicalStatus.RowCancelingEdit
            gdvMedicalStatus.EditIndex = -1
            UpdateMemberStatusGridView()
        End Sub

        Protected Sub gdvMedicalStatus_RowEditing(sender As Object, e As System.Web.UI.WebControls.GridViewEditEventArgs) Handles gdvMedicalStatus.RowEditing
            gdvMedicalStatus.EditIndex = e.NewEditIndex
            UpdateMemberStatusGridView()
        End Sub

        Protected Sub gdvMedicalStatus_RowUpdating(sender As Object, e As System.Web.UI.WebControls.GridViewUpdateEventArgs) Handles gdvMedicalStatus.RowUpdating
            Dim sort_order As Integer? = Nothing

            Dim id As Integer? = Server.HtmlEncode(CInt(gdvMedicalStatus.DataKeys(e.RowIndex).Value))

            Try
                sort_order = Server.HtmlEncode(CType(gdvMedicalStatus.Rows(e.RowIndex).FindControl("txtStatussort"), TextBox).Text)
            Catch
                Exit Sub
            End Try

            Dim name As String = Server.HtmlEncode(CType(gdvMedicalStatus.Rows(e.RowIndex).FindControl("txtStatus"), TextBox).Text)

            If (String.IsNullOrEmpty(name)) Then
                Exit Sub
            End If

            If (sort_order Is Nothing) Then
                Exit Sub
            End If

            If (id Is Nothing) Then
                Exit Sub
            End If

            listEditDao.UpdateDropDownList("core_lookUps_sp_UpdateStatus", id, name, "", sort_order)

            gdvMedicalStatus.EditIndex = -1

            UpdateMemberStatusGridView()
        End Sub

        Private Sub UpdateMemberStatusGridView()
            gdvMedicalStatus.DataSource = listEditDao.GetDropDownList("core_lookUps_sp_MemberStatus")
            gdvMedicalStatus.DataBind()
        End Sub

#End Region

#Region "From"

        Protected Sub btnAddFrom_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnAddFrom.Click
            Dim sort_order As Integer

            If (String.IsNullOrEmpty(txtAddFrom.Text)) Then
                Exit Sub
            End If

            Try
                sort_order = CInt(txtSortOrderFrom.Text)
            Catch
                Exit Sub
            End Try

            listEditDao.InsertDropDownList("core_lookUps_sp_InsertFromLocation", Server.HtmlEncode(txtAddFrom.Text), "", sort_order)

            UpdateFromLocationGridView()

            txtAddFrom.Text = String.Empty
            txtSortOrderFrom.Text = String.Empty
        End Sub

        Protected Sub gdvMedicalFrom_RowCancelingEdit(sender As Object, e As System.Web.UI.WebControls.GridViewCancelEditEventArgs) Handles gdvMedicalFrom.RowCancelingEdit
            gdvMedicalFrom.EditIndex = -1
            UpdateFromLocationGridView()
        End Sub

        Protected Sub gdvMedicalFrom_RowEditing(sender As Object, e As System.Web.UI.WebControls.GridViewEditEventArgs) Handles gdvMedicalFrom.RowEditing
            gdvMedicalFrom.EditIndex = e.NewEditIndex
            UpdateFromLocationGridView()
        End Sub

        Protected Sub gdvMedicalFrom_RowUpdating(sender As Object, e As System.Web.UI.WebControls.GridViewUpdateEventArgs) Handles gdvMedicalFrom.RowUpdating
            Dim sort_order As Integer? = Nothing

            Dim id As Integer? = Server.HtmlEncode(CInt(gdvMedicalFrom.DataKeys(e.RowIndex).Value))
            Try
                sort_order = Server.HtmlEncode(CType(gdvMedicalFrom.Rows(e.RowIndex).FindControl("txtFromsort"), TextBox).Text)
            Catch
                Exit Sub
            End Try

            Dim name As String = Server.HtmlEncode(CType(gdvMedicalFrom.Rows(e.RowIndex).FindControl("txtFrom"), TextBox).Text)

            If (String.IsNullOrEmpty(name)) Then
                Exit Sub
            End If

            If (sort_order Is Nothing) Then
                Exit Sub
            End If

            If (id Is Nothing) Then
                Exit Sub
            End If

            listEditDao.UpdateDropDownList("core_lookUps_sp_UpdateFromLocation", id, name, "", sort_order)

            gdvMedicalFrom.EditIndex = -1

            UpdateFromLocationGridView()
        End Sub

        Private Sub UpdateFromLocationGridView()
            gdvMedicalFrom.DataSource = listEditDao.GetDropDownList("core_lookUps_sp_FromLocation")
            gdvMedicalFrom.DataBind()
        End Sub

#End Region

#Region "Facility"

        Protected Sub btnAddFacility_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnAddFacility.Click
            Dim sort_order As Integer

            If (String.IsNullOrEmpty(txtAddFacility.Text)) Then
                Exit Sub
            End If

            If (String.IsNullOrEmpty(txtTypeFacility.Text)) Then
                Exit Sub
            End If

            Try
                sort_order = CInt(txtSortOrderFacility.Text)
            Catch
                Exit Sub
            End Try

            listEditDao.InsertDropDownList("core_lookUps_sp_InsertFacility", Server.HtmlEncode(txtAddFacility.Text), Server.HtmlEncode(txtTypeFacility.Text), sort_order)

            UpdateFacilityGridView()

            txtAddFacility.Text = String.Empty
            txtSortOrderFacility.Text = String.Empty
            txtTypeFacility.Text = String.Empty
        End Sub

        Protected Sub gdvMedicalFacility_RowCancelingEdit(sender As Object, e As System.Web.UI.WebControls.GridViewCancelEditEventArgs) Handles gdvMedicalFacility.RowCancelingEdit
            gdvMedicalFacility.EditIndex = -1
            UpdateFacilityGridView()
        End Sub

        Protected Sub gdvMedicalFacility_RowEditing(sender As Object, e As System.Web.UI.WebControls.GridViewEditEventArgs) Handles gdvMedicalFacility.RowEditing
            gdvMedicalFacility.EditIndex = e.NewEditIndex
            UpdateFacilityGridView()
        End Sub

        Protected Sub gdvMedicalFacility_RowUpdating(sender As Object, e As System.Web.UI.WebControls.GridViewUpdateEventArgs) Handles gdvMedicalFacility.RowUpdating
            Dim sort_order As Integer? = Nothing

            Dim id As Integer? = Server.HtmlEncode(CInt(gdvMedicalFacility.DataKeys(e.RowIndex).Value))

            Try
                sort_order = Server.HtmlEncode(CType(gdvMedicalFacility.Rows(e.RowIndex).FindControl("txtFacilitysort"), TextBox).Text)
            Catch
                Exit Sub
            End Try

            Dim name As String = Server.HtmlEncode(CType(gdvMedicalFacility.Rows(e.RowIndex).FindControl("txtFacility"), TextBox).Text)

            If (String.IsNullOrEmpty(name)) Then
                Exit Sub
            End If

            If (sort_order Is Nothing) Then
                Exit Sub
            End If

            If (id Is Nothing) Then
                Exit Sub
            End If

            listEditDao.UpdateDropDownList("core_lookUps_sp_UpdateFacility", id, name, "", sort_order)

            gdvMedicalFacility.EditIndex = -1

            UpdateFacilityGridView()
        End Sub

        Private Sub UpdateFacilityGridView()
            gdvMedicalFacility.DataSource = listEditDao.GetDropDownList("core_lookUps_sp_MedicalFacility")
            gdvMedicalFacility.DataBind()
        End Sub

#End Region

#Region "Influence"

        Protected Sub btnAddInfluence_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnAddInfluence.Click
            Dim sort_order As Integer

            If (String.IsNullOrEmpty(txtAddInfluence.Text)) Then
                Exit Sub
            End If

            Try
                sort_order = CInt(txtSortOrderInfluence.Text)
            Catch
                Exit Sub
            End Try

            listEditDao.InsertDropDownList("core_lookUps_sp_InsertInfluence", Server.HtmlEncode(txtAddInfluence.Text), "", sort_order)

            UpdateInfluenceGridView()

            txtAddInfluence.Text = String.Empty
            txtSortOrderInfluence.Text = String.Empty
        End Sub

        Protected Sub gdvMedicalInfluence_RowCancelingEdit(sender As Object, e As System.Web.UI.WebControls.GridViewCancelEditEventArgs) Handles gdvMedicalInfluence.RowCancelingEdit
            gdvMedicalInfluence.EditIndex = -1
            UpdateInfluenceGridView()
        End Sub

        Protected Sub gdvMedicalInfluence_RowEditing(sender As Object, e As System.Web.UI.WebControls.GridViewEditEventArgs) Handles gdvMedicalInfluence.RowEditing
            gdvMedicalInfluence.EditIndex = e.NewEditIndex
            UpdateInfluenceGridView()
        End Sub

        Protected Sub gdvMedicalInfluence_RowUpdating(sender As Object, e As System.Web.UI.WebControls.GridViewUpdateEventArgs) Handles gdvMedicalInfluence.RowUpdating
            Dim sort_order As Integer? = Nothing

            Dim id As Integer? = Server.HtmlEncode(CInt(gdvMedicalInfluence.DataKeys(e.RowIndex).Value))

            Try
                sort_order = Server.HtmlEncode(CType(gdvMedicalInfluence.Rows(e.RowIndex).FindControl("txtInfluencesort"), TextBox).Text)
            Catch
                Exit Sub
            End Try

            Dim name As String = Server.HtmlEncode(CType(gdvMedicalInfluence.Rows(e.RowIndex).FindControl("txtInfluence"), TextBox).Text)

            If (String.IsNullOrEmpty(name)) Then
                Exit Sub
            End If

            If (sort_order Is Nothing) Then
                Exit Sub
            End If

            If (id Is Nothing) Then
                Exit Sub
            End If

            listEditDao.UpdateDropDownList("core_lookUps_sp_UpdateInfluence", id, name, "", sort_order)

            gdvMedicalInfluence.EditIndex = -1

            UpdateInfluenceGridView()
        End Sub

        Private Sub UpdateInfluenceGridView()
            gdvMedicalInfluence.DataSource = listEditDao.GetDropDownList("core_lookUps_sp_MemberInfluence")
            gdvMedicalInfluence.DataBind()
        End Sub

#End Region

    End Class

End Namespace