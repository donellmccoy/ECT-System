Imports ALOD.Data.Services
Imports ALOD.Core.Domain.Users
Imports ALODWebUtility.Common
Imports ALOD.Core.Domain.Workflow
Imports ALOD.Core.Utils

Namespace Web.LOD
    Partial Class Secure_lod_Inbox
        Inherits System.Web.UI.Page

        Const ERROR_LOADING_QUEUE As String = "An error occured loading the requested queue"
        Const ERROR_NO_MATCHES As String = "No matches found"
        Const VALUE_DELIMITER As String = ";"
        Const VALUE_COUNT As Integer = 2

        Const PARAM_USERID As String = "userId"
        Const PARAM_REPORT_VIEW As String = "rptView"
        Const PARAM_MODULE_ID As String = "moduleId"
        Const PARAM_SARC As String = "sarcpermission"

        Const DEFAULT_SORT_COLUMN As String = "CaseId"
        Const COLUMN_LOCK_ID As String = "lockId"

        Const KEY_USERID As String = "USER_ID"
        Const KEY_VIEW As String = "REPORT_VIEW"
        Const KEY_CLICKED As String = "SEARCH_CLICKED"

        Protected Property SearchUserId() As Integer
            Get
                If (ViewState(KEY_USERID) Is Nothing) Then
                    Return 0
                End If
                Return CInt(ViewState(KEY_USERID))
            End Get
            Set(ByVal value As Integer)
                ViewState(KEY_USERID) = value
            End Set
        End Property

        Protected Property SearchView() As Byte
            Get
                If (ViewState(KEY_VIEW) Is Nothing) Then
                    Return 0
                End If
                Return CByte(ViewState(KEY_VIEW))
            End Get
            Set(ByVal value As Byte)
                ViewState(KEY_VIEW) = value
            End Set
        End Property

        Protected Property GetInboxClicked() As Boolean
            Get
                If (ViewState(KEY_CLICKED) Is Nothing) Then
                    Return False
                End If
                Return CBool(ViewState(KEY_CLICKED))
            End Get
            Set(ByVal value As Boolean)
                ViewState(KEY_CLICKED) = value
            End Set
        End Property

        Protected Sub SearchButton_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles SearchButton.Click

            GetInboxClicked = False
            Dim userName As String = Server.HtmlEncode(UsernameInput.Text.Trim)

            If (userName.Length = 0) Then
                AddCssClass(UsernameInput, CSS_FIELD_REQUIRED)
            Else
                RemoveCssClass(UsernameInput, CSS_FIELD_REQUIRED)
                PopulateUserList(userName)
            End If

        End Sub

        Protected Sub PopulateUserList(ByVal username As String)

            UserSelect.Items.Clear()
            Dim users As IList(Of AppUser) = UserService.FindByUsername(username)

            If (users.Count = 0) Then
                ErrorMessageLabel.Text = ERROR_NO_MATCHES
                UserPanel.Visible = False
                StartPanel.Visible = True
                Exit Sub
            Else
                ErrorMessageLabel.Text = String.Empty
                UserPanel.Visible = True
                StartPanel.Visible = False
            End If

            For Each row As AppUser In users

                Dim text As String = String.Format("{0} {1}, {2} ({3}) - {4} ({5}) ", _
                        row.Rank.Rank, row.LastName, row.FirstName, _
                        row.Username.ToUpper(), row.CurrentRoleName, row.Unit.Name)

                Dim value As String = row.Id.ToString() + VALUE_DELIMITER

                If (row.ReportView.HasValue) Then
                    value += CStr(row.ReportView.Value)
                Else
                    value += CStr(row.CurrentRole.Group.ReportView)
                End If

                UserSelect.Items.Add(New ListItem(text, value))
            Next

        End Sub

        Protected Sub LodData_Selecting(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.ObjectDataSourceSelectingEventArgs) Handles LodData.Selecting

            If (Not GetInboxClicked) Then
                e.Cancel = True
                Exit Sub
            End If

            If (SearchUserId = 0 OrElse SearchView = 0) Then
                ErrorMessageLabel.Text = ERROR_LOADING_QUEUE
                e.Cancel = True
                Exit Sub
            End If

            e.InputParameters(PARAM_USERID) = SearchUserId
            e.InputParameters(PARAM_REPORT_VIEW) = SearchView
            e.InputParameters(PARAM_MODULE_ID) = CByte(ModuleType.LOD)
            e.InputParameters(PARAM_SARC) = False

        End Sub

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

            If (Not IsPostBack) Then
                SetDefaultButton(UsernameInput, SearchButton)
                SetInputFormatRestriction(Page, UsernameInput, FormatRestriction.AlphaNumeric, PERMITTED_SPECIAL_CHAR_INPUT)
            End If

        End Sub

        Protected Sub Page_PreRender(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.PreRender
            ErrorPanel.Visible = (ErrorMessageLabel.Text.Length > 0)
        End Sub

        Protected Sub ViewInboxButton_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles ViewInboxButton.Click
            GetInboxView()
        End Sub

        Protected Sub GetInboxView()

            Const INDEX_USERID As Integer = 0
            Const INDEX_VIEW As Integer = 1

            If (UserSelect.Items.Count = 0) Then
                ErrorMessageLabel.Text = ERROR_LOADING_QUEUE
                Exit Sub
            End If

            Dim parts() As String = UserSelect.SelectedValue.Split(VALUE_DELIMITER)

            If (parts.Count <> VALUE_COUNT) Then
                ErrorMessageLabel.Text = ERROR_LOADING_QUEUE
                Exit Sub
            End If

            Integer.TryParse(parts(INDEX_USERID), SearchUserId)
            Byte.TryParse(parts(INDEX_VIEW), SearchView)

            If (SearchUserId = 0 OrElse SearchView = 0) Then
                ErrorMessageLabel.Text = ERROR_LOADING_QUEUE
                Exit Sub
            End If

            ErrorMessageLabel.Text = String.Empty
            GetInboxClicked = True
            ResultGrid.Visible = True
            ResultGrid.DataBind()

        End Sub

        Protected Sub ResetButton_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles ResetButton.Click
            StartPanel.Visible = True
            UserPanel.Visible = False
            ResultGrid.Visible = False
            SearchUserId = Nothing
            SearchView = Nothing
            GetInboxClicked = False
            UsernameInput.Focus()
        End Sub

        Protected Sub ResultGrid_RowDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewRowEventArgs) Handles ResultGrid.RowDataBound

            Const CONTROL_LOCK_IMAGE As String = "LockImage"
            HeaderRowBinding(sender, e, DEFAULT_SORT_COLUMN)

            If (e.Row.RowType = DataControlRowType.DataRow) Then

                Dim data As DataRowView = CType(e.Row.DataItem, DataRowView)
                Dim lockId As Integer = 0
                Integer.TryParse(data(COLUMN_LOCK_ID), lockId)

                If (lockId > 0) Then
                    CType(e.Row.FindControl(CONTROL_LOCK_IMAGE), Image).Visible = True
                End If

                Dim refID As Integer = data("RefId")

                CType(e.Row.FindControl("PrintImage"), ImageButton).OnClientClick = "printForms('" & refID & "', 'lod'); return false;"
            End If

        End Sub
    End Class
End Namespace
