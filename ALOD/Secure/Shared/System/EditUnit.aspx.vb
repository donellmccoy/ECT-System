Imports ALOD.Core.Utils
Imports ALOD.Data
Imports ALODWebUtility.Common

Namespace Web.Sys

    Public Class EditUnit
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
                UpdateDutyStatusGridView()
                UpdateSourceGridView()
                UpdateOccurrenceGridView()
                UpdateProximateGridView()

                SetInputFormatRestriction(Page, txtSortOrderDuty, FormatRestriction.Numeric)
                SetInputFormatRestriction(Page, txtSortOrderSource, FormatRestriction.Numeric)
                SetInputFormatRestriction(Page, txtSortOrderOccurrence, FormatRestriction.Numeric)
                SetInputFormatRestriction(Page, txtSortOrderProximate, FormatRestriction.Numeric)
            End If
        End Sub

#Region "Duty Status"

        Protected Sub btnAddDuty_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnAddDuty.Click
            Dim sort_order As Integer

            If (String.IsNullOrEmpty(txtAddDuty.Text)) Then
                Exit Sub
            End If

            If (String.IsNullOrEmpty(txtTypeDuty.Text)) Then
                Exit Sub
            End If

            Try
                sort_order = CInt(txtSortOrderDuty.Text)
            Catch
                Exit Sub
            End Try

            listEditDao.InsertDropDownList("core_lookUps_sp_InsertDutyStatus", Server.HtmlEncode(txtAddDuty.Text), Server.HtmlEncode(txtTypeDuty.Text), sort_order)

            UpdateDutyStatusGridView()

            txtAddDuty.Text = String.Empty
            txtSortOrderDuty.Text = String.Empty
            txtTypeDuty.Text = String.Empty
        End Sub

        Protected Sub gdvUnitDuty_RowCancelingEdit(sender As Object, e As System.Web.UI.WebControls.GridViewCancelEditEventArgs) Handles gdvUnitDuty.RowCancelingEdit
            gdvUnitDuty.EditIndex = -1
            UpdateDutyStatusGridView()
        End Sub

        Protected Sub gdvUnitDuty_RowEditing(sender As Object, e As System.Web.UI.WebControls.GridViewEditEventArgs) Handles gdvUnitDuty.RowEditing
            gdvUnitDuty.EditIndex = e.NewEditIndex
            UpdateDutyStatusGridView()
        End Sub

        Protected Sub gdvUnitDuty_RowUpdating(sender As Object, e As System.Web.UI.WebControls.GridViewUpdateEventArgs) Handles gdvUnitDuty.RowUpdating
            Dim id As Integer? = Server.HtmlEncode(CInt(gdvUnitDuty.DataKeys(e.RowIndex).Value))
            Dim sort_order As Integer? = Nothing

            Try
                sort_order = Convert.ToInt32(CType(gdvUnitDuty.Rows(e.RowIndex).FindControl("txtDutysort"), TextBox).Text)
            Catch
                Exit Sub
            End Try

            Dim name As String = Server.HtmlEncode(CType(gdvUnitDuty.Rows(e.RowIndex).FindControl("txtDuty"), TextBox).Text)

            If (String.IsNullOrEmpty(name)) Then
                Exit Sub
            End If

            If (sort_order Is Nothing) Then
                Exit Sub
            End If

            If (id Is Nothing) Then
                Exit Sub
            End If

            listEditDao.UpdateDropDownList("core_lookUps_sp_UpdateDutyStatus", id, name, "", sort_order)

            gdvUnitDuty.EditIndex = -1

            UpdateDutyStatusGridView()
        End Sub

        Private Sub UpdateDutyStatusGridView()
            gdvUnitDuty.DataSource = listEditDao.GetDropDownList("core_lookUps_sp_DutyStatuses")
            gdvUnitDuty.DataBind()
        End Sub

#End Region

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

#Region "Occurrence"

        Protected Sub btnAddOccurrence_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnAddOccurrence.Click
            Dim sort_order As Integer

            If (String.IsNullOrEmpty(txtAddOccurrence.Text)) Then
                Exit Sub
            End If

            Try
                sort_order = CInt(txtSortOrderOccurrence.Text)
            Catch
                Exit Sub
            End Try

            listEditDao.InsertDropDownList("core_lookUps_sp_InsertOccurrence", Server.HtmlEncode(txtAddOccurrence.Text), "", sort_order)

            UpdateOccurrenceGridView()

            txtAddOccurrence.Text = String.Empty
            txtSortOrderOccurrence.Text = String.Empty
        End Sub

        Protected Sub gdvUnitOccurrence_RowCancelingEdit(sender As Object, e As System.Web.UI.WebControls.GridViewCancelEditEventArgs) Handles gdvUnitOccurrence.RowCancelingEdit
            gdvUnitOccurrence.EditIndex = -1
            UpdateOccurrenceGridView()
        End Sub

        Protected Sub gdvUnitOccurrence_RowEditing(sender As Object, e As System.Web.UI.WebControls.GridViewEditEventArgs) Handles gdvUnitOccurrence.RowEditing
            gdvUnitOccurrence.EditIndex = e.NewEditIndex
            UpdateOccurrenceGridView()
        End Sub

        Protected Sub gdvUnitOccurrence_RowUpdating(sender As Object, e As System.Web.UI.WebControls.GridViewUpdateEventArgs) Handles gdvUnitOccurrence.RowUpdating
            Dim id As Integer? = Server.HtmlEncode(CInt(gdvUnitOccurrence.DataKeys(e.RowIndex).Value))
            Dim sort_order As Integer? = Nothing

            Try
                sort_order = Convert.ToInt32(CType(gdvUnitOccurrence.Rows(e.RowIndex).FindControl("txtSortOccurrence"), TextBox).Text)
            Catch
                Exit Sub
            End Try

            Dim name As String = Server.HtmlEncode(CType(gdvUnitOccurrence.Rows(e.RowIndex).FindControl("txtOccurrence"), TextBox).Text)

            If (String.IsNullOrEmpty(name)) Then
                Exit Sub
            End If

            If (sort_order Is Nothing) Then
                Exit Sub
            End If

            If (id Is Nothing) Then
                Exit Sub
            End If

            listEditDao.UpdateDropDownList("core_lookUps_sp_UpdateOccurrence", id, name, "", sort_order)

            gdvUnitOccurrence.EditIndex = -1

            UpdateOccurrenceGridView()
        End Sub

        Private Sub UpdateOccurrenceGridView()
            gdvUnitOccurrence.DataSource = listEditDao.GetDropDownList("core_lookUps_sp_Occurrences")
            gdvUnitOccurrence.DataBind()
        End Sub

#End Region

#Region "Proximate Cause"

        Protected Sub btnAddProximate_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnAddProximate.Click
            Dim sort_order As Integer

            If (String.IsNullOrEmpty(txtAddProximate.Text)) Then
                Exit Sub
            End If

            Try
                sort_order = CInt(txtSortOrderProximate.Text)
            Catch
                Exit Sub
            End Try

            listEditDao.InsertDropDownList("core_lookUps_sp_InsertProximate", Server.HtmlEncode(txtAddProximate.Text), "", sort_order)

            UpdateProximateGridView()

            txtAddProximate.Text = String.Empty
            txtSortOrderProximate.Text = String.Empty
        End Sub

        Protected Sub gdvUnitProximate_RowCancelingEdit(sender As Object, e As System.Web.UI.WebControls.GridViewCancelEditEventArgs) Handles gdvUnitProximate.RowCancelingEdit
            gdvUnitProximate.EditIndex = -1
            UpdateProximateGridView()
        End Sub

        Protected Sub gdvUnitProximate_RowEditing(sender As Object, e As System.Web.UI.WebControls.GridViewEditEventArgs) Handles gdvUnitProximate.RowEditing
            gdvUnitProximate.EditIndex = e.NewEditIndex
            UpdateProximateGridView()
        End Sub

        Protected Sub gdvUnitProximate_RowUpdating(sender As Object, e As System.Web.UI.WebControls.GridViewUpdateEventArgs) Handles gdvUnitProximate.RowUpdating
            Dim id As Integer? = Server.HtmlEncode(CInt(gdvUnitProximate.DataKeys(e.RowIndex).Value))
            Dim sort_order As Integer? = Nothing

            Try
                sort_order = Convert.ToInt32(CType(gdvUnitProximate.Rows(e.RowIndex).FindControl("txtSortProximate"), TextBox).Text)
            Catch
                Exit Sub
            End Try

            Dim name As String = Server.HtmlEncode(CType(gdvUnitProximate.Rows(e.RowIndex).FindControl("txtProximate"), TextBox).Text)

            If (String.IsNullOrEmpty(name)) Then
                Exit Sub
            End If

            If (sort_order Is Nothing) Then
                Exit Sub
            End If

            If (id Is Nothing) Then
                Exit Sub
            End If

            listEditDao.UpdateDropDownList("core_lookUps_sp_UpdateProximate", id, name, "", sort_order)

            gdvUnitProximate.EditIndex = -1

            UpdateProximateGridView()
        End Sub

        Private Sub UpdateProximateGridView()
            gdvUnitProximate.DataSource = listEditDao.GetDropDownList("core_lookUps_sp_ProximateCause")
            gdvUnitProximate.DataBind()
        End Sub

#End Region

    End Class

End Namespace