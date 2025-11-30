Imports ALOD.Core.Domain.Workflow
Imports ALOD.Core.Interfaces.DAOInterfaces
Imports ALOD.Core.Utils
Imports ALOD.Core.Utils.RegexValidation
Imports ALOD.Data
Imports ALOD.Data.Services
Imports ALODWebUtility.Common
Imports ALODWebUtility.Permission.Search

Namespace Web.Special_Case

    Partial Class Secure_sc_Search
        Inherits Page

        Private _workflowDao As IWorkflowDao

        ReadOnly Property ModuleId As Integer
            Get
                Return CInt(Request.QueryString("mid"))
            End Get
        End Property

        Protected ReadOnly Property WorkflowDao As IWorkflowDao
            Get
                If (_workflowDao Is Nothing) Then
                    _workflowDao = New NHibernateDaoFactory().GetWorkflowDao()
                End If

                Return _workflowDao
            End Get
        End Property

        Protected Sub ddlLodStatus_DataBound(sender As Object, e As EventArgs) Handles StatusSelect.DataBound
            Utility.InsertDropDownListEmptyValue(StatusSelect, "-- All --")
        End Sub

        Protected Sub ddlModule_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ddlModule.SelectedIndexChanged
            StatusData.SelectParameters("workflow").DefaultValue = WorkflowDao.GetWorkflowFromModule(ddlModule.SelectedValue)
            StatusData.DataBind()
        End Sub

        Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
            If (Not IsPostBack) Then
                InitControls()
                InitInputFormatRestrictions()
            End If
        End Sub

        Protected Sub ResultGrid_RowDataBound(sender As Object, e As GridViewRowEventArgs) Handles ResultGrid.RowDataBound
            HeaderRowBinding(sender, e, "CaseId")

            If (e.Row.RowType = DataControlRowType.DataRow) Then
                Dim data As DataRowView = CType(e.Row.DataItem, DataRowView)
                Dim lockId As Integer = 0
                Dim refId As Integer = data("RefId")
                Integer.TryParse(data("lockId"), lockId)

                If (lockId > 0) Then
                    CType(e.Row.FindControl("LockImage"), Image).Visible = True
                End If

                Dim modType As Integer = 0
                Integer.TryParse(data("ModuleId"), modType)

                If ((modType <> ModuleType.SpecCaseWWD) And (modType <> ModuleType.SpecCaseFT)) Then
                    CType(e.Row.FindControl("PrintImage"), ImageButton).Visible = False
                ElseIf (modType = ModuleType.SpecCaseFT) Then
                    CType(e.Row.FindControl("PrintImage"), ImageButton).OnClientClick = "printForms('" & refId & "', 'SC_FT'); return false;"
                ElseIf (modType = ModuleType.SpecCaseWWD) Then
                    CType(e.Row.FindControl("PrintImage"), ImageButton).OnClientClick = "printForms('" & refId & "', 'SC_WD'); return false;"
                End If
            End If
        End Sub

        Protected Sub SearchButton_Click(sender As Object, e As EventArgs) Handles SearchButton.Click
            ResultGrid.DataBind()
        End Sub

        Protected Sub SearchData_Selecting(sender As Object, e As ObjectDataSourceSelectingEventArgs) Handles SearchData.Selecting
            NoFiltersPanel.Visible = False
            e.InputParameters("sarcpermission") = UserHasPermission(PERMISSION_VIEW_SARC_CASES)
            If (SsnBox.Text.Trim.Length = 0 AndAlso
                NameBox.Text.Trim.Length = 0 AndAlso
                CaseIdBox.Text.Trim.Length = 0 AndAlso
                StatusSelect.SelectedIndex = 0 AndAlso
                ddlModule.SelectedIndex = 0 AndAlso
                UnitSelect.SelectedIndex < 1) Then
                NoFiltersPanel.Visible = True
                e.Cancel = True
                Exit Sub
            End If
        End Sub

        Private Function GetModuleDataSource() As List(Of Core.Domain.Workflow.Module)
            Dim modules As List(Of Core.Domain.Workflow.Module) = DataHelpers.ExtractObjectsFromDataSet(Of Core.Domain.Workflow.Module)(LookupService.GetAllModules())

            Return modules.Where(Function(x) x.IsSpecialCase = True AndAlso HasSearchPermissionForModule(x)).OrderBy(Function(y) y.Name).ToList()
        End Function

        Private Function HasSearchPermissionForModule(m As Core.Domain.Workflow.Module) As Boolean
            Return UserHasPermission(GetSearchPermissionByModuleId(m.IdAsModuleType))
        End Function

        Private Sub InitControls()
            InitModuleDropdownList()
            StatusData.SelectParameters("workflow").DefaultValue = WorkflowDao.GetWorkflowFromModule(ModuleId)
            InitUnitDropdownListAsAsyncPageTask()
            PreloadSearchFilters()
            ResultGrid.Sort("CaseId", SortDirection.Ascending)
        End Sub

        Private Sub InitInputFormatRestrictions()
            SetInputFormatRestriction(Page, SsnBox, FormatRestriction.Numeric, "-")
            SetInputFormatRestriction(Page, NameBox, FormatRestriction.AlphaNumeric, PERMITTED_SPECIAL_CHAR_INPUT)
            SetInputFormatRestriction(Page, CaseIdBox, FormatRestriction.AlphaNumeric, "-")
        End Sub

        Private Sub InitModuleDropdownList()
            ddlModule.DataSource = GetModuleDataSource()
            ddlModule.DataValueField = "Id"
            ddlModule.DataTextField = "Name"
            ddlModule.DataBind()

            Utility.InsertDropDownListZeroValue(ddlModule, "--- ALL Special Cases ---")

            If Session(SESSIONKEY_COMPO) = "5" Then
                For Each currItem In ddlModule.Items
                    If currItem.Text.Contains("Worldwide Duty") OrElse currItem.Value = "9" Then currItem.Text = "Non Duty Disability Evaluation System"
                Next
            End If

            ddlModule.SelectedValue = ModuleId
        End Sub

        Private Sub InitUnitDropdownListAsAsyncPageTask()
            Dim task = New PageAsyncTask(New BeginEventHandler(AddressOf BeginAsyncUnitLookup), New EndEventHandler(AddressOf EndAsyncUnitLookup), Nothing, GetStateObject(UnitSelect))
            RegisterAsyncTask(task)
        End Sub

        Private Sub PreloadSearchFilters()
            If (Request.QueryString("data") Is Nothing) Then
                Exit Sub
            End If

            Dim data As String = Request.QueryString("data")

            If (IsNumeric(data)) Then
                If (data.Length = 4) Then
                    'assume SSN
                    SsnBox.Text = data
                Else
                    'assume caseId
                    CaseIdBox.Text = data
                End If
            Else
                'could be either a name, or caseid
                If (IsValidCaseId(data)) Then
                    CaseIdBox.Text = data
                Else
                    NameBox.Text = data
                End If
            End If
        End Sub

        Private Sub ResultGrid_RowCommand(sender As Object, e As GridViewCommandEventArgs) Handles ResultGrid.RowCommand
            If (e.CommandName = "view") Then
                Dim parts() As String = e.CommandArgument.ToString().Split(";")
                Dim strQuery As New StringBuilder()
                Dim args As New ItemSelectedEventArgs
                args.RefId = CInt(parts(0))

                strQuery.Append("refId=" + CType(args.RefId, String))
                args.Type = CInt(parts(1))

                If (UserHasPermission(GetViewPermissionByModuleId(args.Type))) Then
                    args.Url = GetWorkflowInitPageURL(args.Type, args.RefId)

                    Response.Redirect(args.Url)
                Else
                    args.Url = "~/Secure/Shared/SecureAccessDenied.aspx?deniedType=1"
                End If

                Response.Redirect(args.Url)
            End If
        End Sub

    End Class

End Namespace