Imports ALOD.Core.Domain.Common
Imports ALOD.Core.Utils
Imports ALOD.Data
Imports ALOD.Data.Services
Imports ALOD.Logging
Imports ALODWebUtility.Common

Namespace Web.Sys

    Public Class EditICDCodes
        Inherits System.Web.UI.Page

        Public Property SelectedParentId() As Integer
            Get
                Return CInt(ViewState("SelectedParentId"))
            End Get
            Set(ByVal value As Integer)
                ViewState("SelectedParentId") = value
            End Set
        End Property

        Protected Sub gvResults_RowCancelingEdit(sender As Object, e As System.Web.UI.WebControls.GridViewCancelEditEventArgs) Handles gvResults.RowCancelingEdit
            gvResults.EditIndex = -1
            BindGridView(SelectedParentId)
        End Sub

        Protected Sub gvResults_RowDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewRowEventArgs) Handles gvResults.RowDataBound

            If (e.Row.RowType <> DataControlRowType.DataRow) Then
                Exit Sub
            End If

            ' Check if this row is being edited or not
            If e.Row.RowIndex = gvResults.EditIndex Then
                Dim txtValue As TextBox = e.Row.FindControl("txtValue")
                Dim txtDescription As TextBox = e.Row.FindControl("txtDescription")

                SetInputFormatRestriction(Page, txtValue, FormatRestriction.AlphaNumeric, PERMITTED_SPECIAL_CHAR_INPUT)
                SetInputFormatRestriction(Page, txtDescription, FormatRestriction.AlphaNumeric, PERMITTED_SPECIAL_CHAR_INPUT)
            End If
        End Sub

        Protected Sub gvResults_RowEditing(sender As Object, e As System.Web.UI.WebControls.GridViewEditEventArgs) Handles gvResults.RowEditing
            gvResults.EditIndex = e.NewEditIndex

            'ScriptManager.GetCurrent(Me).RegisterAsyncPostBackControl(gvResults.Rows(e.NewEditIndex).Controls(4))

            BindGridView(SelectedParentId)
        End Sub

        Protected Sub gvResults_RowUpdating(sender As Object, e As System.Web.UI.WebControls.GridViewUpdateEventArgs) Handles gvResults.RowUpdating
            lblError.Visible = False

            Dim row As GridViewRow = gvResults.Rows(e.RowIndex)

            If (row Is Nothing) Then
                lblError.Text = "* An error has occured! Edit Aborted!<br />"
                lblError.Visible = True
                LogManager.LogError("Edit ICD Codes page failed to find the row being edited.")
                gvResults.EditIndex = -1
                BindGridView(SelectedParentId)
                Exit Sub
            End If

            ' Validate the inputs before we do anything
            If (Not ValidateGridViewEditInput(e.RowIndex)) Then
                Exit Sub
            End If

            Dim internalError As Boolean = False
            Dim nullControlName As String = String.Empty
            Dim txtValue As TextBox = row.FindControl("txtValue")
            Dim txtDescription As TextBox = row.FindControl("txtDescription")
            Dim chkIsDisease As CheckBox = row.FindControl("chkIsDisease")
            Dim chkActive As CheckBox = row.FindControl("chkActive")
            Dim lblIdControl As Label = row.FindControl("lblId")

            If (txtValue Is Nothing) Then
                internalError = True
                nullControlName = "txtValue"
            ElseIf (txtDescription Is Nothing) Then
                internalError = True
                nullControlName = "txtDescription"
            ElseIf (chkIsDisease Is Nothing) Then
                internalError = True
                nullControlName = "chkIsDisease"
            ElseIf (chkActive Is Nothing) Then
                internalError = True
                nullControlName = "chkActive"
            ElseIf (lblIdControl Is Nothing) Then
                internalError = True
                nullControlName = "lblIdControl"
            Else
                internalError = False
            End If

            If (internalError) Then
                lblError.Text = "* An error has occured! Edit Aborted! <br />"
                lblError.Visible = True
                LogManager.LogError("Edit ICD Codes page failed to find the " + nullControlName + " gridview control.")
                gvResults.EditIndex = -1
                BindGridView(SelectedParentId)
                Exit Sub
            End If

            Dim newValue As String = Server.HtmlEncode(txtValue.Text)
            Dim newDescription As String = Server.HtmlEncode(txtDescription.Text)

            Dim icdDao As ICD9CodeDao = New NHibernateDaoFactory().GetICD9CodeDao()

            'If (Not icdDao.DoesCodeExist(newValue)) Then
            '    icdDao.UpdateCode(CInt(lblIdControl.Text), newValue, newDescription, chkIsDisease.Checked, chkActive.Checked)
            'End If

            icdDao.UpdateCode(CInt(lblIdControl.Text), newValue, newDescription, chkIsDisease.Checked, chkActive.Checked)

            gvResults.EditIndex = -1
            BindGridView(SelectedParentId)

            ' Rebind the DDLs in case the edited code has been activated or deactivated...
            ucICDCodeControl.ForceFullRebind(SelectedParentId)
        End Sub

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
            ucICDCodeControl.Initialilze(Me)

            AddHandler ucICDCodeControl.ICDCodeSelectionChanged, AddressOf UpdateGridViewEventHandler

            If (Not IsPostBack) Then
                ucICDCodeControl.DisplayReadWrite(True)
                ucICDCodeControl.DisableLastLevel(True)
                SelectedParentId = 0
                BindGridView(0)
            End If
        End Sub

        Protected Sub UpdateGridViewEventHandler(ByVal sender As Object, ByVal e As ICDCodeSelectedEventArgs)
            UpdateGridView(e.SelectedICDCodeId, e.SelectedDropDownLevel)
        End Sub

        Private Sub BindGridView(ByVal icdCodeId As Integer)
            Dim icdDao As ICD9CodeDao = New NHibernateDaoFactory().GetICD9CodeDao()
            Dim dataSource As DataSet = icdDao.GetChildren(icdCodeId, 10, False)

            If (dataSource Is Nothing) Then
                Exit Sub
            End If

            SelectedParentId = icdCodeId

            gvResults.DataSource = dataSource
            gvResults.DataBind()
        End Sub

        Private Sub UpdateGridView(ByVal icdCodeId As Integer, ByVal selectedLevel As Integer)
            If (icdCodeId < 1 AndAlso selectedLevel <> 1) Then
                lblResultsPanelTitle.Text = "INVALID CODE!"
                Exit Sub
            End If

            Dim codeDescription As String = "NONE SELECTED"

            If (icdCodeId > 0) Then
                Dim code As ICD9Code = LookupService.GetIcd9CodeById(icdCodeId)

                If (code Is Nothing) Then
                    lblResultsPanelTitle.Text = "INVALID CODE!"
                    Exit Sub
                End If

                codeDescription = code.Description
            End If

            lblResultsPanelTitle.Text = "Children of ICD Code - " + codeDescription

            lblError.Visible = False

            BindGridView(icdCodeId)
        End Sub

        Private Function ValidateGridViewEditInput(ByVal editIndex As Integer) As Boolean

            Dim row As GridViewRow = gvResults.Rows(editIndex)

            If (row Is Nothing) Then
                Return False
            End If

            Dim txtDescription As TextBox = row.FindControl("txtDescription")

            If (txtDescription Is Nothing OrElse String.IsNullOrEmpty(txtDescription.Text)) Then
                txtDescription.CssClass = "fieldRequired"
                Return False
            Else
                txtDescription.CssClass = ""
            End If

            Return True
        End Function

    End Class

End Namespace