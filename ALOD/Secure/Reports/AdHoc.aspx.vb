Imports ALOD.Core.Domain.PsychologicalHealth
Imports ALOD.Core.Domain.Query
Imports ALOD.Core.Domain.Users
Imports ALOD.Core.Interfaces.DAOInterfaces
Imports ALOD.Core.Utils
Imports ALOD.Data
Imports ALOD.Data.Services
Imports ALODWebUtility.Common

Namespace Web.Reports

    Partial Class Secure_Reports_AdHoc
        Inherits System.Web.UI.Page

        Const ALL_SOURCE As String = "vw_all_cases"
        Const AP_SOURCE As String = "vw_appeal"
        Const APSA_SOURCE As String = "vw_appeal_sarc"
        Const DATACHOICE_TEXTBOX_CSSCLASS As String = "DataChoiceTextbox"
        Const EDIT_ID_KEY As String = "EDIT_ID_KEY"
        Const ERROR_CRITERIA_REQUIRED As String = "Please select at least one Query Criteria"
        Const ERROR_OUTPUT_REQUIRED As String = "Please select at least one Output Field"
        Const GRID_EDIT_OPERATOR_CSSCLASS As String = "gridEditOperator"
        Const GRID_EDIT_TEXTBOX_CSSCLASS As String = "gridEditText"
        Const LOD_SOURCE As String = "vw_lod"
        Const LOD_SOURCE_AUDIT As String = "vw_lod_348_audit"
        Const PAGE_MODE_EDIT As String = "PAGE_EDIT"
        Const PAGE_MODE_KEY As String = "PAGE_MODE_KEY"
        Const PAGE_MODE_VIEW As String = "PAGE_VIEW"
        Const PH_SOURCE As String = "vw_sc_ph_reporting"
        Const RR_SOURCE As String = "vw_rr"
        Const SARC_SOURCE As String = "vw_sarc"
        Const SC_SOURCE As String = "vw_sc"
        Const UNIT_NAME_PARAM As String = "Unit Name"
        Private _phDao As IPsychologicalHealthDao
        Private _phQueryDao As IPHQueryDao
        Private dao As IQueryDao = Nothing
        Dim SaveAsNew As Boolean = False

        Public ReadOnly Property PHDao As IPsychologicalHealthDao
            Get
                If (_phDao Is Nothing) Then
                    _phDao = New NHibernateDaoFactory().GetPsychologicalHealthDao()
                End If

                Return _phDao
            End Get
        End Property

        Public ReadOnly Property PHQueryDao As IPHQueryDao
            Get
                If (_phQueryDao Is Nothing) Then
                    _phQueryDao = New NHibernateDaoFactory().GetPHQueryDao()
                End If

                Return _phQueryDao
            End Get
        End Property

        Protected ReadOnly Property CalendarImage() As String
            Get
                Return Me.ResolveClientUrl("~/App_Themes/" + Page.Theme + "/Images/Calendar.gif")
            End Get
        End Property

        Protected Property EditId() As Integer
            Get
                If (ViewState(EDIT_ID_KEY) Is Nothing) Then
                    ViewState(EDIT_ID_KEY) = 0
                End If
                Return CInt(ViewState(EDIT_ID_KEY))
            End Get
            Set(ByVal value As Integer)
                ViewState(EDIT_ID_KEY) = value
            End Set
        End Property

        Protected Property PageMode() As String
            Get
                If (ViewState(PAGE_MODE_KEY) Is Nothing) Then
                    ViewState(PAGE_MODE_KEY) = PAGE_MODE_VIEW
                End If
                Return ViewState(PAGE_MODE_KEY)
            End Get
            Set(ByVal value As String)
                ViewState(PAGE_MODE_KEY) = value
            End Set
        End Property

        Private ReadOnly Property QueryDao() As IQueryDao
            Get
                If (dao Is Nothing) Then
                    dao = New NHibernateDaoFactory().GetQueryDao()
                End If
                Return dao
            End Get
        End Property

        Protected Sub AddParamButton_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles AddParamButton.Click
            ' Add a PH Form Field parameter if and only if the PH Form case type is selected and the PH Form Field criteria type is selected; otherwise, add a workflow parameter...
            If (IsPHFormCaseTypeSelected() AndAlso rblCriteriaType.SelectedValue.Equals("2")) Then
                AddPHFormFieldParameter()
            Else
                AddWorkflowParameter()
            End If
        End Sub

        ' Adds all of the PH Form Fields currently configured for the PH Form.
        Protected Sub btnAddAllPHFormFields_Click(sender As Object, e As EventArgs) Handles btnAddAllPHFormFields.Click
            If (EditId = 0) Then
                Exit Sub
            End If

            ' Get all of the currently configured PH Form Fields...
            Dim formFields As List(Of PHFormField) = PHDao.GetAllFormFields().OrderBy(Function(x) x.Section.Name).ThenBy(Function(y) y.Field.Name).ThenBy(Function(z) z.FieldType.Name).ToList()

            If (formFields Is Nothing) Then
                Exit Sub
            End If

            Dim phQuery As PHUserQuery = PHQueryDao.GetByBaseQueryId(EditId)

            If (phQuery Is Nothing) Then
                Exit Sub
            End If

            Dim outputField As PHQueryOutputField = Nothing

            ' Create an output field for each PH Form Field not already used as an output field for the PH Query...
            For Each formField As PHFormField In formFields
                outputField = New PHQueryOutputField()
                outputField.SectionId = formField.Section.Id
                outputField.FieldId = formField.Field.Id
                outputField.FieldTypeId = formField.FieldType.Id

                If (phQuery.ContainsOutputField(outputField)) Then
                    Continue For
                End If

                ' Update database...
                phQuery.AddOutputField(outputField)
                PHQueryDao.CommitChanges()

                ' Update UI...
                lsbOutputPHFormFields.Items.Add(New ListItem(formField.Section.Name + " | " + formField.Field.Name + " | " + formField.FieldType.Name, outputField.Id))
            Next

            PHQueryDao.Evict(phQuery)
        End Sub

        ' Adds the PH Form Field specified by the three dropdown listbox selections (Section, Field, & FieldType).
        Protected Sub btnAddPHFormField_Click(sender As Object, e As EventArgs) Handles btnAddPHFormField.Click
            If (EditId = 0) Then
                Exit Sub
            End If

            ' At minimum make sure a section was selected...
            If (ddlOutputPHSection.SelectedIndex = 0) Then
                Exit Sub
            End If

            Dim phQuery As PHUserQuery = PHQueryDao.GetByBaseQueryId(EditId)

            If (phQuery Is Nothing) Then
                Exit Sub
            End If

            Dim pullFromConfiguredFields As Boolean = True
            Dim sectionId As Integer = Integer.Parse(ddlOutputPHSection.SelectedValue)
            Dim fieldId As Integer = Integer.Parse(ddlOutputPHField.SelectedValue)
            Dim fieldTypeId As Integer = Integer.Parse(ddlOutputPHFieldType.SelectedValue)
            Dim formFields As List(Of PHFormField) = New List(Of PHFormField)()

            ' Get all of the currently configured PH Form Fields whose section, field, and fieldtype matches the selected section, field, and fieldtype...
            If (sectionId > 0 AndAlso fieldId > 0 AndAlso fieldTypeId > 0) Then
                pullFromConfiguredFields = False
            ElseIf (sectionId > 0 AndAlso fieldId > 0) Then
                formFields = PHDao.GetAllFormFields().Where(Function(x) x.Section.Id = sectionId AndAlso x.Field.Id = fieldId).OrderBy(Function(y) y.Field.Name).ThenBy(Function(z) z.FieldType.Name).ToList()
            ElseIf (sectionId > 0) Then
                formFields = PHDao.GetAllFormFields().Where(Function(x) x.Section.Id = sectionId).OrderBy(Function(y) y.Field.Name).ThenBy(Function(z) z.FieldType.Name).ToList()
            End If

            Dim outputField As PHQueryOutputField = Nothing

            If (pullFromConfiguredFields) Then
                ' Create an output field for each PH Form Field not already used as an output field for the PH Query...
                For Each formField As PHFormField In formFields
                    outputField = New PHQueryOutputField()
                    outputField.SectionId = formField.Section.Id
                    outputField.FieldId = formField.Field.Id
                    outputField.FieldTypeId = formField.FieldType.Id

                    If (phQuery.ContainsOutputField(outputField)) Then
                        Continue For
                    End If

                    ' Update database...
                    phQuery.AddOutputField(outputField)
                    PHQueryDao.CommitChanges()

                    ' Update UI...
                    lsbOutputPHFormFields.Items.Add(New ListItem(formField.Section.Name + " | " + formField.Field.Name + " | " + formField.FieldType.Name, outputField.Id))
                Next
            Else
                outputField = New PHQueryOutputField()
                outputField.SectionId = sectionId
                outputField.FieldId = fieldId
                outputField.FieldTypeId = fieldTypeId

                If (Not phQuery.ContainsOutputField(outputField)) Then
                    ' Update database...
                    phQuery.AddOutputField(outputField)
                    PHQueryDao.CommitChanges()

                    ' Update UI...
                    Dim selectField As ListItem = New ListItem(ddlOutputPHSection.SelectedItem.Text + " | " + ddlOutputPHField.SelectedItem.Text + " | " + ddlOutputPHFieldType.SelectedItem.Text, outputField.Id)
                    lsbOutputPHFormFields.Items.Add(selectField)
                End If
            End If

            PHQueryDao.Evict(phQuery)
        End Sub

        ' Removes all of the PH Form Fields.
        Protected Sub btnRemoveAllPHFormFields_Click(sender As Object, e As EventArgs) Handles btnRemoveAllPHFormFields.Click
            If (EditId = 0) Then
                Exit Sub
            End If

            Dim phQuery As PHUserQuery = PHQueryDao.GetByBaseQueryId(EditId)

            If (phQuery Is Nothing) Then
                Exit Sub
            End If

            ' Remove from database...
            For Each outputField As PHQueryOutputField In phQuery.OutputFields
                PHQueryDao.DeleteOutputField(outputField)
            Next

            phQuery.OutputFields.Clear()

            PHQueryDao.CommitChanges()

            ' Remove all output fields from UI...
            lsbOutputPHFormFields.Items.Clear()

            PHQueryDao.Evict(phQuery)
        End Sub

        ' Removes the selecte PH Form Fields
        Protected Sub btnRemovePHFormField_Click(sender As Object, e As EventArgs) Handles btnRemovePHFormField.Click
            If (EditId = 0) Then
                Exit Sub
            End If

            If (lsbOutputPHFormFields.GetSelectedIndices().Count = 0) Then
                Exit Sub
            End If

            Dim phQuery As PHUserQuery = PHQueryDao.GetByBaseQueryId(EditId)

            If (phQuery Is Nothing) Then
                Exit Sub
            End If

            Dim outputField As PHQueryOutputField
            Dim deletedFields As List(Of PHQueryOutputField) = New List(Of PHQueryOutputField)()
            Dim deletedItems As List(Of ListItem) = New List(Of ListItem)()

            ' Remove from database...
            For Each item As ListItem In lsbOutputPHFormFields.Items
                If (Not item.Selected) Then
                    Continue For
                End If

                outputField = PHQueryDao.GetOutputFieldById(item.Value)

                If (outputField Is Nothing) Then
                    Continue For
                End If

                deletedItems.Add(item)
                deletedFields.Add(outputField)

                PHQueryDao.DeleteOutputField(outputField)
            Next

            ' Remove from Query object...
            For Each deletedField As PHQueryOutputField In deletedFields
                phQuery.RemoveOutputField(deletedField)
            Next

            ' Remove from UI...
            For Each deletedItem As ListItem In deletedItems
                lsbOutputPHFormFields.Items.Remove(deletedItem)
            Next

            PHQueryDao.CommitChanges()
            PHQueryDao.Evict(phQuery)
        End Sub

        Protected Sub ddlCaseType_SelectedIndexChanged(sender As Object, e As System.EventArgs) Handles ddlCaseType.SelectedIndexChanged
            MyQuerySelect.SelectedIndex = 0
            ddlSharedReports.SelectedIndex = 0
            ErrorPanel.Visible = False
            pnlWorkflowOutputUI.Visible = True
            pnlPHFormTotalsOutputUI.Visible = False
            lblWorkflowParams.Visible = False
            lblPHParams.Visible = False
            'ClearCurrentTransient()
            InitInputSources()
            InitSavedQueries()
            ResetCriteriaPanel()
            InitPHFormReportControls(IsPHFormCaseTypeSelected())
            UpdateExecuteOrderTextbox()
        End Sub

        Protected Sub ddlOutputPHField_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ddlOutputPHField.SelectedIndexChanged
            If (ddlOutputPHField.SelectedIndex = 0) Then
                ddlOutputPHFieldType.Enabled = False
                ddlOutputPHFieldType.SelectedIndex = 0
            Else
                ddlOutputPHFieldType.Enabled = True
            End If
        End Sub

        Protected Sub ddlOutputPHSection_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ddlOutputPHSection.SelectedIndexChanged
            If (ddlOutputPHSection.SelectedIndex = 0) Then
                ddlOutputPHField.Enabled = False
                ddlOutputPHFieldType.Enabled = False

                ddlOutputPHField.SelectedIndex = 0
                ddlOutputPHFieldType.SelectedIndex = 0
            Else
                ddlOutputPHField.Enabled = True
            End If
        End Sub

        Protected Sub ddlPHFieldType_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ddlPHFieldType.SelectedIndexChanged
            ErrorPanel.Visible = False
            InitPHSourceValueControls(CInt(ddlPHFieldType.SelectedValue))
        End Sub

        Protected Sub DeleteReportButton_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles DeleteReportButton.Click
            ' If no reports have been selected then don't delete anything
            If (MyQuerySelect.SelectedValue = "0" AndAlso ddlSharedReports.SelectedValue = "0") Then
                Exit Sub
            End If

            ErrorPanel.Visible = False

            Dim id As Integer

            ' If the 0th index is selected in the Saved Reports dropdownlist then a value has been selected in the Shared Reports dropdownlist
            If (MyQuerySelect.SelectedValue = "0") Then
                id = CInt(ddlSharedReports.SelectedValue)
            End If

            ' If the 0th index is selected in the Shared Reports dropdownlist then a value has been selected in the Saved Reports dropdownlist
            If (ddlSharedReports.SelectedValue = "0") Then
                id = CInt(MyQuerySelect.SelectedValue)
            End If

            Dim query As UserQuery = QueryDao.GetById(id)

            If (query IsNot Nothing AndAlso Not query.IsTransient) Then
                ' If the query has an associated PHUserQuery then delete that query as well...
                Dim phQuery As PHUserQuery = PHQueryDao.GetByBaseQueryId(id)

                If (phQuery IsNot Nothing) Then
                    ' For some reason a NHibernate threadsafe exception occurs when attempting to delete the PHUserQuery unless we explicitly delete
                    ' each PHQueryClause and PHQueryOutputField from the PHUserQuery...this does not need to be done when deleting just the UserQuery
                    ' for some reason...
                    For Each clause As PHQueryClause In phQuery.Clauses
                        PHQueryDao.DeleteClause(clause)
                    Next

                    For Each outputField As PHQueryOutputField In phQuery.OutputFields
                        PHQueryDao.DeleteOutputField(outputField)
                    Next

                    phQuery.OutputFields.Clear()
                    phQuery.Clauses.Clear()

                    PHQueryDao.Delete(phQuery)
                    PHQueryDao.CommitChanges()
                    PHQueryDao.Evict(phQuery)
                End If

                ' Delete query from database...
                QueryDao.Delete(query)
                QueryDao.CommitChanges()

                InitSavedQueries()
            End If

            ' Reset to a default user query
            ClearCurrentTransient()
        End Sub

        Protected Sub EditReportButton_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles EditReportButton.Click
            ' If no reports have been selected then don't load anything
            If (MyQuerySelect.SelectedValue = "0" AndAlso ddlSharedReports.SelectedValue = "0") Then
                Exit Sub
            End If

            ' If the 0th index is selected in the Saved Reports dropdownlist then a value has been selected in the Shared Reports dropdownlist
            If (MyQuerySelect.SelectedValue = "0") Then
                Dim sharedReport As UserQuery = QueryDao.GetById(CInt(ddlSharedReports.SelectedValue))

                If (sharedReport IsNot Nothing) Then
                    ' Check if the user is the owner of the shared report
                    If (sharedReport.User.Id = SESSION_USER_ID) Then
                        EditId = CInt(ddlSharedReports.SelectedValue)
                    Else
                        EditId = CopySharedReportToTransientReport(sharedReport)
                    End If
                Else
                    EditId = 0
                End If
            End If

            ' If the 0th index is selected in the Shared Reports dropdownlist then a value has been selected in the Saved Reports dropdownlist
            If (ddlSharedReports.SelectedValue = "0") Then
                EditId = CInt(MyQuerySelect.SelectedValue)
            End If

            ErrorPanel.Visible = False

            LoadUserQuery()
        End Sub

        Protected Sub gdvPHQuery_RowCancelingEdit(sender As Object, e As System.Web.UI.WebControls.GridViewCancelEditEventArgs) Handles gdvPHQuery.RowCancelingEdit
            gdvPHQuery.EditIndex = -1
            UpdateCurrentPHFormFieldQueryGrid()
            ErrorPanel.Visible = False
        End Sub

        Protected Sub gdvPHQuery_RowCommand(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewCommandEventArgs) Handles gdvPHQuery.RowCommand
            If (e.CommandName = "DeleteParam") Then
                Dim param As PHQueryParameter = PHQueryDao.GetParameterById(CInt(e.CommandArgument))
                Dim query As PHUserQuery = PHQueryDao.GetByBaseQueryId(EditId)
                Dim oldOrder As Integer = param.ExecuteOrder

                query.RemoveParameter(param)
                PHQueryDao.DeleteParameter(param)
                PHQueryDao.CommitChanges()
                PHQueryDao.Evict(query)

                PHQueryDao.UpdateParamExecuteOrders(param.Clause.Id, param.Id, oldOrder, oldOrder, 2) ' filter = 2 means a delete occured
                UpdateExecuteOrderTextbox()
                UpdateCurrentPHFormFieldQueryGrid()
            End If
        End Sub

        Protected Sub gdvPHQuery_RowDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewRowEventArgs) Handles gdvPHQuery.RowDataBound
            If (e.Row.RowType <> DataControlRowType.DataRow) Then
                Exit Sub
            End If

            ' Disable the Edit link and the delete button for each row if the report being loaded is a shared report the logged in user does not own...
            If (EditId > 0) Then
                Dim query As UserQuery = QueryDao.GetById(EditId)

                If (query.Shared AndAlso query.User.Id <> SESSION_USER_ID) Then
                    CType(e.Row.Cells(5).Controls(0), LinkButton).Visible = False
                    CType(e.Row.FindControl("DeleteParamButton"), ImageButton).Visible = False
                End If
            End If

            Dim param As PHQueryParameter = PHQueryDao.GetParameterById(CInt(gdvPHQuery.DataKeys(e.Row.RowIndex).Value))

            ' Check if this row is being edited or not
            If e.Row.RowIndex <> gdvPHQuery.EditIndex Then
                ' Format the operator output
                Dim label As Label = CType(e.Row.FindControl("OperatorLabel"), Label)
                Dim oper As String = label.Text

                Select Case oper
                    Case QueryOperator.BETWEEN
                        oper = "Is Between"
                    Case QueryOperator.EQUALS
                        oper = "Equals"
                    Case QueryOperator.GREATER_THAN
                        oper = "Is Greater Than"
                    Case QueryOperator.LESS_THAN
                        oper = "Is Less Than"
                    Case QueryOperator.LIKE
                        oper = "Contains"
                    Case QueryOperator.NOT_EQUAL
                        oper = "Does Not Equal"
                    Case Else
                        oper = "Equals"
                End Select

                label.Text = oper

                ' Now format the second (end) value
                CType(e.Row.FindControl("AndLabel"), Label).Visible = IIf(CType(e.Row.FindControl("EndValueLabel"), Label).Text.Length > 0, True, False)
            Else
                Dim gridExecuteOrderTxt As TextBox = e.Row.FindControl("txtGridExecuteOrder")
                Dim gridTypeDDL As DropDownList = e.Row.FindControl("ddlGridType")
                Dim gridFieldDDL As DropDownList = e.Row.FindControl("ddlGridField")
                Dim gridOperatorDDL As DropDownList = e.Row.FindControl("ddlGridOperator")

                SetInputFormatRestriction(Page, gridExecuteOrderTxt, FormatRestriction.Numeric)
                InitPHFormGridViewOperators(param, gridOperatorDDL)
                InitPHFormGridViewValues(param, e)

                gridExecuteOrderTxt.Text = Server.HtmlDecode(param.ExecuteOrder)
                gridTypeDDL.SelectedValue = param.WhereType
                gridOperatorDDL.SelectedValue = param.Operator
            End If
        End Sub

        Protected Sub gdvPHQuery_RowEditing(sender As Object, e As System.Web.UI.WebControls.GridViewEditEventArgs) Handles gdvPHQuery.RowEditing
            gdvPHQuery.EditIndex = e.NewEditIndex
            UpdateCurrentPHFormFieldQueryGrid()
            ErrorPanel.Visible = False
        End Sub

        Protected Sub gdvPHQuery_RowUpdating(sender As Object, e As System.Web.UI.WebControls.GridViewUpdateEventArgs) Handles gdvPHQuery.RowUpdating
            Dim param As PHQueryParameter = PHQueryDao.GetParameterById(CInt(gdvPHQuery.DataKeys(e.RowIndex).Value))

            ' Validate the inputs before we do anything
            If (Not ValidatePHFormGridViewParamInput(e.RowIndex, param)) Then
                Exit Sub
            End If

            gdvPHQuery.EditIndex = -1

            Dim phQuery As PHUserQuery = PHQueryDao.GetByBaseQueryId(EditId)
            Dim oldExecuteOrder As Integer = param.ExecuteOrder
            Dim oldWhereType As String = param.WhereType
            Dim oldOperator As String = param.Operator
            Dim oldStartDisplay As String = param.StartDisplay
            Dim oldStartValue As String = param.StartValue
            Dim oldEndDisplay As String = param.EndDisplay
            Dim oldEndValue As String = param.EndValue
            Dim oldModifiedDate As Date = phQuery.UserQuery.ModifiedDate

            ' Update param values
            Dim txtExecuteOrder As TextBox = gdvPHQuery.Rows(e.RowIndex).FindControl("txtGridExecuteOrder")
            param.ExecuteOrder = Server.HtmlEncode(txtExecuteOrder.Text)

            Dim ddl As DropDownList = gdvPHQuery.Rows(e.RowIndex).FindControl("ddlGridType")
            param.WhereType = ddl.SelectedValue

            Dim gridOperatorDDL As DropDownList = gdvPHQuery.Rows(e.RowIndex).FindControl("ddlGridOperator")
            param.Operator = gridOperatorDDL.SelectedValue

            GetPHFormGridViewSourceValues(e.RowIndex, param)

            phQuery.UserQuery.ModifiedDate = DateTime.Now
            PHQueryDao.SaveOrUpdateParameter(param)
            PHQueryDao.CommitChanges()
            PHQueryDao.Evict(phQuery)

            ' Check if the parameter should be removed and replaced with the old one...
            If (phQuery.UserQuery.Shared = True) Then
                phQuery = PHQueryDao.GetByBaseQueryId(EditId)

                ' Get all shared user queries
                Dim queries = QueryDao.GetSharedUserQueries()
                Dim currentPHQuery As PHUserQuery = Nothing

                For Each q As UserQuery In queries
                    If (q.Id = phQuery.UserQuery.Id) Then
                        Continue For
                    End If

                    currentPHQuery = PHQueryDao.GetByBaseQueryId(q.Id)

                    If (currentPHQuery Is Nothing) Then
                        Continue For
                    End If

                    ' Check if there already exists a query that matches this query...
                    If (q.Equals(phQuery.UserQuery) = True AndAlso currentPHQuery.Equals(phQuery) = True) Then
                        Dim errors As New StringCollection()
                        errors.Add("A shared report with these exact criteria already exists. That report is titled """ & q.Title & """. The edit to the criteria was not saved.")
                        ErrorPanel.Visible = True
                        ErrorList.DataSource = errors
                        ErrorList.DataBind()

                        ' Reassign old parameter values...
                        param.ExecuteOrder = oldExecuteOrder
                        param.WhereType = oldWhereType
                        param.Operator = oldOperator
                        param.StartDisplay = oldStartDisplay
                        param.StartValue = oldStartValue
                        param.EndDisplay = oldEndDisplay
                        param.EndValue = oldEndValue
                        phQuery.UserQuery.ModifiedDate = oldModifiedDate

                        PHQueryDao.SaveOrUpdateParameter(param)
                        PHQueryDao.CommitChanges()
                        PHQueryDao.Evict(phQuery)

                        UpdateCurrentPHFormFieldQueryGrid()
                        Exit Sub
                    End If
                Next
            End If

            ' Check if the execute order was changed
            If (oldExecuteOrder <> param.ExecuteOrder) Then
                PHQueryDao.UpdateParamExecuteOrders(param.Clause.Id, param.Id, param.ExecuteOrder, oldExecuteOrder, 0)   ' filter = 0 means an edit occured
            End If

            UpdateCurrentPHFormFieldQueryGrid()
        End Sub

        Protected Sub NewReportButton_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles NewReportButton.Click
            MyQuerySelect.SelectedIndex = 0
            ddlSharedReports.SelectedIndex = 0
            ErrorPanel.Visible = False

            ' Reset PH related controls...
            If (IsPHFormCaseTypeSelected()) Then
                rblCriteriaType.SelectedIndex = 0
                rblCriteriaType_SelectedIndexChanged(rblCriteriaType, New EventArgs())
            End If

            ClearCurrentTransient()
        End Sub

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

            If (Not IsPostBack) Then
                PageMode = PAGE_MODE_VIEW
                InitCaseTypeDropDown()
                InitInputSources()
                InitSavedQueries()

                SetInputFormatRestriction(Page, txtExecuteOrder, FormatRestriction.Numeric)
                SetInputFormatRestriction(Page, NumberStart, FormatRestriction.Numeric)
                SetInputFormatRestriction(Page, NumberStop, FormatRestriction.Numeric)
                SetInputFormatRestriction(Page, DateStart, FormatRestriction.Numeric, "/")
                SetInputFormatRestriction(Page, DateStop, FormatRestriction.Numeric, "/")
                SetInputFormatRestriction(Page, DataChoiceText, FormatRestriction.AlphaNumeric, PERMITTED_SPECIAL_CHAR_INPUT)
                SetInputFormatRestriction(Page, RangeStart, FormatRestriction.Numeric)
                SetInputFormatRestriction(Page, RangeEnd, FormatRestriction.Numeric)
                SetInputFormatRestriction(Page, SaveTitlebox, FormatRestriction.AlphaNumeric, PERMITTED_SPECIAL_CHAR_INPUT)

                btnFindUnit.Attributes.Add("onclick", "showSearcher('" + "Find Unit" + "','" + SrcUnitIdHdn.ClientID + "','" + SrcNameHdn.ClientID + "'); return false;")

                If (CByte(Session("GroupId")) = UserGroups.SystemAdministrator) Then
                    chkIsAdmin.Checked = True
                Else
                    chkIsAdmin.Checked = False
                End If

            End If

            ' New code

            ' Check if the find unit components should be shown
            If (SourceSelect.SelectedItem IsNot Nothing AndAlso Not IsUnitNameSource(SourceSelect.SelectedItem.Text)) Then
                SrcNameHdn.Value = ""
                btnFindUnit.Visible = False
                cbSubUnit.Visible = False
                lblSubUnitInfo.Visible = False
            Else
                btnFindUnit.Visible = True
                cbSubUnit.Visible = True
                lblSubUnitInfo.Visible = True
            End If

            ' Check if SSN components should be modified and shown
            If (SourceSelect.SelectedItem IsNot Nothing AndAlso SourceSelect.SelectedItem.Text <> "Member SSN") Then
                DataChoiceText.MaxLength = 100
                DataChoiceText.Width = 250
                lblSSNMsg.Visible = False
            Else
                DataChoiceText.MaxLength = 4
                DataChoiceText.Width = 50
                lblSSNMsg.Visible = True
            End If

            ' Check if the report editing control needs to be enabled and visible or not
            If (EditId > 0) Then
                Dim query As UserQuery = QueryDao.GetById(EditId)

                ' If the loaded query is shared and NOT owned by the logged in user then disable the editing controls...
                If (query.Shared AndAlso query.User.Id <> SESSION_USER_ID) Then
                    TurnReportEditingControlsOnOff(False)
                Else
                    TurnReportEditingControlsOnOff(True)
                End If
            Else
                TurnReportEditingControlsOnOff(True)
            End If

            ' End new code
        End Sub

        Protected Sub QueryButton_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles QueryButton.Click

            Dim valid As Boolean = True
            Dim errors As New StringCollection()

            If (IsPHFormCaseTypeSelected()) Then
                Dim phQuery As PHUserQuery = PHQueryDao.GetByBaseQueryId(EditId)

                If (phQuery Is Nothing) Then
                    valid = False
                Else
                    ' Make sure the query has at least one output form field specified...
                    If (phQuery.OutputFields Is Nothing OrElse phQuery.OutputFields.Count = 0) Then
                        errors.Add(ERROR_OUTPUT_REQUIRED)
                        valid = False
                    End If

                    phQuery.SortField = GetPHFormSortField()
                    PHQueryDao.CommitChanges()
                End If
            Else
                Dim query As UserQuery = QueryDao.GetById(EditId)

                'make sure we have output fields selected
                If (SelectedFields.Text.Length = 0) Then
                    errors.Add(ERROR_OUTPUT_REQUIRED)
                    valid = False
                End If

                'update the output fields

                'make sure we have some input criteria
                If (query.Clauses.Count = 0) OrElse
                (query.Clauses(0).Parameters Is Nothing) OrElse
                (query.Clauses(0).Parameters.Count = 0) Then
                    'we have to have at least 1 search parameter
                    errors.Add(ERROR_CRITERIA_REQUIRED)
                    valid = False
                End If

                ' Make sure that the output fields of the query are associated to the selected case type
                Dim fields As IList(Of QueryOutputField) = QueryDao.GetOutputFields(ddlCaseType.SelectedValue)

                For Each field As String In query.OutputFields
                    Dim currField As String = field

                    ' Query the current field name against all of the fields associated with the selecte case type.
                    Dim matches = From f In fields
                                  Where f.FieldName = currField
                                  Select f

                    ' If no matches found, then this field is not associated with the selected case type
                    If matches.Count = 0 Then
                        errors.Add("The " & field & " output field is not supported by the selected Case Type")
                        valid = False
                    End If
                Next

                query.FieldList = SelectedFields.Text
                query.SortFields = SortFields.Text
                query.ModifiedDate = DateTime.Now
            End If

            If (Not valid) Then
                ErrorPanel.Visible = True
                ErrorList.DataSource = errors
                ErrorList.DataBind()
                Exit Sub
            Else
                ErrorPanel.Visible = False
            End If

            Dim mode As String = "s" 'default to browser
            Dim popup As Boolean = True 'show results in a new popup window
            Dim reportType As AdHocReportType = AdHocReportType.Standard

            'get the type of output
            If (OutputExcel.Checked) Then
                mode = "e"
                popup = False
            ElseIf (OutputPdf.Checked) Then
                mode = "p"
                popup = True
            ElseIf (OutputCsv.Checked) Then
                mode = "c"
                popup = False
            ElseIf (OutputXml.Checked) Then
                mode = "x"
                popup = False
            End If

            If (IsPHFormCaseTypeSelected()) Then
                reportType = AdHocReportType.PH_Totals
            End If

            Dim url As String = ResolveClientUrl("~/Secure/reports/AdhocResults.aspx?id=" + EditId.ToString() + "&mode=" + mode + "&report=" + Convert.ToInt32(reportType).ToString())

            If (popup) Then
                ResultsUrl.Text = url
            Else
                Response.Redirect(url)
            End If

        End Sub

        Protected Sub QueryGrid_RowCancelingEdit(sender As Object, e As System.Web.UI.WebControls.GridViewCancelEditEventArgs) Handles QueryGrid.RowCancelingEdit
            QueryGrid.EditIndex = -1
            UpdateCurrentWorkflowQueryGrid()
            ErrorPanel.Visible = False
        End Sub

        Protected Sub QueryGrid_RowCommand(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewCommandEventArgs) Handles QueryGrid.RowCommand

            If (e.CommandName = "DeleteParam") Then
                Dim param As QueryParameter = QueryDao.GetParameterById(CInt(e.CommandArgument))
                Dim query As UserQuery = QueryDao.GetById(EditId)
                Dim oldOrder As Integer = param.ExecuteOrder

                query.RemoveParameter(param)
                QueryDao.DeleteParameter(param)
                QueryDao.CommitChanges()
                QueryDao.Evict(query)

                QueryDao.UpdateParamExecuteOrders(param.Clause.Id, param.Id, oldOrder, oldOrder, 2) ' filter = 2 means a delete occured
                UpdateExecuteOrderTextbox()

                UpdateCurrentWorkflowQueryGrid()
            End If

        End Sub

        Protected Sub QueryGrid_RowDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewRowEventArgs) Handles QueryGrid.RowDataBound

            If (e.Row.RowType <> DataControlRowType.DataRow) Then
                Exit Sub
            End If

            ' Disable the Edit link and the delete button for each row if the report being loaded is a shared report the logged in user does not own...
            If (EditId > 0) Then
                Dim query As UserQuery = QueryDao.GetById(EditId)

                If (query.Shared AndAlso query.User.Id <> SESSION_USER_ID) Then
                    CType(e.Row.Cells(5).Controls(0), LinkButton).Visible = False
                    CType(e.Row.FindControl("DeleteParamButton"), ImageButton).Visible = False
                End If

            End If

            Dim param As QueryParameter = QueryDao.GetParameterById(CInt(QueryGrid.DataKeys(e.Row.RowIndex).Value))

            ' Check if this row is being edited or not
            If e.Row.RowIndex <> QueryGrid.EditIndex Then

                'format the operator output
                Dim label As Label = CType(e.Row.FindControl("OperatorLabel"), Label)
                Dim oper As String = label.Text

                Select Case oper
                    Case QueryOperator.BETWEEN
                        oper = "Is Between"
                    Case QueryOperator.EQUALS
                        oper = "Equals"
                    Case QueryOperator.GREATER_THAN
                        oper = "Is Greater Than"
                    Case QueryOperator.LESS_THAN
                        oper = "Is Less Than"
                    Case QueryOperator.LIKE
                        oper = "Contains"
                    Case QueryOperator.NOT_EQUAL
                        oper = "Does Not Equal"
                    Case Else
                        oper = "Equals"
                End Select

                label.Text = oper

                'now format the second (end) value
                If (IsUnitNameSource(param.Source.DisplayName)) Then
                    CType(e.Row.FindControl("AndLabel"), Label).Visible = False
                    CType(e.Row.FindControl("EndValueLabel"), Label).Visible = False
                Else
                    CType(e.Row.FindControl("AndLabel"), Label).Visible =
                        (IIf(CType(e.Row.FindControl("EndValueLabel"), Label).Text.Length > 0, True, False))
                End If
            Else
                Dim gridExecuteOrderTxt As TextBox = e.Row.FindControl("txtGridExecuteOrder")
                Dim gridTypeDDL As DropDownList = e.Row.FindControl("ddlGridType")
                Dim gridFieldDDL As DropDownList = e.Row.FindControl("ddlGridField")
                Dim gridOperatorDDL As DropDownList = e.Row.FindControl("ddlGridOperator")

                SetInputFormatRestriction(Page, gridExecuteOrderTxt, FormatRestriction.Numeric)

                'InitGridViewInputSources(gridFieldDDL)
                InitWorkflowGridViewOperators(param, gridOperatorDDL)
                InitWorkflowGridViewValues(param, e)

                gridExecuteOrderTxt.Text = Server.HtmlDecode(param.ExecuteOrder)
                gridTypeDDL.SelectedValue = param.WhereType
                'gridFieldDDL.SelectedValue = param.Source.Id
                gridOperatorDDL.SelectedValue = param.Operator
            End If
        End Sub

        Protected Sub QueryGrid_RowEditing(sender As Object, e As System.Web.UI.WebControls.GridViewEditEventArgs) Handles QueryGrid.RowEditing
            QueryGrid.EditIndex = e.NewEditIndex
            UpdateCurrentWorkflowQueryGrid()
            ErrorPanel.Visible = False
        End Sub

        Protected Sub QueryGrid_RowUpdating(sender As Object, e As System.Web.UI.WebControls.GridViewUpdateEventArgs) Handles QueryGrid.RowUpdating

            Dim param As QueryParameter = QueryDao.GetParameterById(CInt(QueryGrid.DataKeys(e.RowIndex).Value))

            'validate the inputs before we do anything
            If (Not ValidateWorkflowGridViewParamInput(e.RowIndex, param)) Then
                Exit Sub
            End If

            QueryGrid.EditIndex = -1

            Dim query As UserQuery = QueryDao.GetById(EditId)
            Dim oldExecuteOrder As Integer = param.ExecuteOrder
            Dim oldWhereType As String = param.WhereType
            Dim oldOperator As String = param.Operator
            Dim oldStartDisplay As String = param.StartDisplay
            Dim oldStartValue As String = param.StartValue
            Dim oldEndDisplay As String = param.EndDisplay
            Dim oldEndValue As String = param.EndValue
            Dim oldModifiedDate As Date = query.ModifiedDate

            ' Update param values
            Dim txtExecuteOrder As TextBox = QueryGrid.Rows(e.RowIndex).FindControl("txtGridExecuteOrder")
            param.ExecuteOrder = Server.HtmlEncode(txtExecuteOrder.Text)

            Dim ddl As DropDownList = QueryGrid.Rows(e.RowIndex).FindControl("ddlGridType")
            param.WhereType = ddl.SelectedValue

            Dim gridOperatorDDL As DropDownList = QueryGrid.Rows(e.RowIndex).FindControl("ddlGridOperator")
            param.Operator = gridOperatorDDL.SelectedValue

            GetWorkflowGridViewSourceValues(e.RowIndex, param)

            query.ModifiedDate = DateTime.Now
            QueryDao.SaveOrUpdateParameter(param)
            QueryDao.CommitChanges()
            QueryDao.Evict(query)

            ' Check if the parameter should be removed and replaced with the old one...
            If (query.Shared = True) Then
                query = QueryDao.GetById(EditId)

                ' Get all shared user queries
                Dim queries = QueryDao.GetSharedUserQueries()

                For Each q As UserQuery In queries
                    ' Check if there already exists a query that matches this query...
                    If (q.Id <> query.Id AndAlso q.Equals(query) = True) Then
                        Dim errors As New StringCollection()
                        errors.Add("A shared report with these exact criteria already exists. That report is titled """ & q.Title & """. The edit to the """ & param.Source.DisplayName & """ criteria was not saved.")
                        ErrorPanel.Visible = True
                        ErrorList.DataSource = errors
                        ErrorList.DataBind()

                        ' Reassign old parameter values...
                        param.ExecuteOrder = oldExecuteOrder
                        param.WhereType = oldWhereType
                        param.Operator = oldOperator
                        param.StartDisplay = oldStartDisplay
                        param.StartValue = oldStartValue
                        param.EndDisplay = oldEndDisplay
                        param.EndValue = oldEndValue
                        query.ModifiedDate = oldModifiedDate

                        QueryDao.SaveOrUpdateParameter(param)
                        QueryDao.CommitChanges()
                        QueryDao.Evict(query)

                        UpdateCurrentWorkflowQueryGrid()

                        Exit Sub
                    End If
                Next
            End If

            ' Check if the execute order was changed
            If (oldExecuteOrder <> param.ExecuteOrder) Then
                QueryDao.UpdateParamExecuteOrders(param.Clause.Id, param.Id, param.ExecuteOrder, oldExecuteOrder, 0)   ' filter = 0 means an edit occured
            End If

            UpdateCurrentWorkflowQueryGrid()
        End Sub

        Protected Sub rblCriteriaType_SelectedIndexChanged(sender As Object, e As EventArgs) Handles rblCriteriaType.SelectedIndexChanged
            SourcePanel.Visible = False

            ' Init input sources based on users criteria type selection...
            If (rblCriteriaType.SelectedValue.Equals("1")) Then         ' Workflow
                InitInputSources()
            ElseIf (rblCriteriaType.SelectedValue.Equals("2")) Then     ' PH Form Field
                InitPHInputSources()
            End If

            UpdateExecuteOrderTextbox()
        End Sub

        Protected Sub ResetCriteriaPanel()
            SourceSelect.SelectedIndex = 0
            InitSourceValue(0)
        End Sub

        Protected Sub SaveQueryNewButton_Click(sender As Object, e As System.EventArgs) Handles SaveQueryNewButton.Click
            ErrorPanel.Visible = False

            ' Load query using current EditId
            Dim currentQuery As UserQuery = QueryDao.GetById(EditId)
            Dim currentPHQuery As PHUserQuery = PHQueryDao.GetByBaseQueryId(EditId)

            ' Check to make sure that if a shared report is being saved it has a different name than the other
            ' shared reports.
            If (chkSaveShared.Checked = True) Then
                ' Get all shared user queries
                Dim queries = QueryDao.GetSharedUserQueries()
                Dim phQ As PHUserQuery = Nothing
                Dim foundMatch As Boolean = False

                For Each q As UserQuery In queries
                    If (q.Id = currentQuery.Id) Then
                        Continue For
                    End If

                    foundMatch = False
                    phQ = PHQueryDao.GetByBaseQueryId(q.Id)

                    ' Check if there already exists a query with the same title as this new query...
                    If (q.Title.Equals(SaveTitlebox.Text.Trim)) Then
                        Dim errors As New StringCollection()
                        errors.Add("Report with title """ & q.Title & """ already exists! Please save report with a new title.")
                        ErrorPanel.Visible = True
                        ErrorList.DataSource = errors
                        ErrorList.DataBind()
                        Exit Sub
                    End If

                    ' Check if there already exists a query that matches this new query...
                    If (q.Equals(currentQuery) = True) Then
                        If (currentPHQuery Is Nothing AndAlso phQ Is Nothing) Then
                            foundMatch = True
                        ElseIf (currentPHQuery IsNot Nothing AndAlso phQ IsNot Nothing AndAlso phQ.Equals(currentPHQuery) = True) Then
                            foundMatch = True
                        End If
                    End If

                    If (foundMatch) Then
                        Dim errors As New StringCollection()
                        errors.Add("A shared report with these exact criteria already exists. That report is titled """ & q.Title & """. The """ & SaveTitlebox.Text.Trim() & """ report has not been saved.")
                        ErrorPanel.Visible = True
                        ErrorList.DataSource = errors
                        ErrorList.DataBind()
                        Exit Sub
                    End If
                Next
            End If

            Dim isExistingQuery As Boolean = False

            ' If that query is NOT transient, then the query being saved as new is an existing query.
            If (currentQuery.Transient = False) Then
                isExistingQuery = True
            Else
                dao.Evict(currentQuery)
            End If

            SaveAsNew = True
            ClearCurrentTransient() ' Creates a new query and assigns its ID to EditId
            SaveAsNew = False

            Dim query As UserQuery = QueryDao.GetById(EditId)
            query.FieldList = SelectedFields.Text
            query.SortFields = SortFields.Text
            query.Title = Server.HtmlEncode(SaveTitlebox.Text.Trim)
            query.Transient = False
            query.Shared = chkSaveShared.Checked
            query.ModifiedDate = DateTime.Now

            Dim phQuery As PHUserQuery = PHQueryDao.GetByBaseQueryId(EditId)

            If (phQuery IsNot Nothing) Then
                phQuery.UserQuery = query
                phQuery.ReportType = 1
                phQuery.SortField = GetPHFormSortField()
            End If

            ' Check if an existing query is being saved as new
            If (isExistingQuery = True) Then
                ' Copy the parameters of the existing query into the new query being created
                For Each clause As QueryClause In currentQuery.Clauses
                    For Each param As QueryParameter In clause.Parameters
                        AddExistingParameter(query, param)
                    Next
                Next

                ' Copy the PH query parameters of the existing PH query into the new PH query being created
                If (currentPHQuery IsNot Nothing) Then
                    phQuery = PHQueryDao.GetByBaseQueryId(EditId)

                    If (phQuery Is Nothing) Then
                        phQuery = New PHUserQuery()
                        phQuery.UserQuery = query
                        phQuery.ReportType = 1 'Integer.Parse(rblReportType.SelectedValue)
                        phQuery.SortField = currentPHQuery.SortField
                        PHQueryDao.SaveOrUpdateQuery(phQuery)
                    End If

                    For Each clause As PHQueryClause In currentPHQuery.Clauses
                        For Each param As PHQueryParameter In clause.Parameters
                            AddExistingPHFormParameter(phQuery, param)
                        Next
                    Next

                    ' Copy the PH Form output fields
                    For Each outputField As PHQueryOutputField In currentPHQuery.OutputFields
                        AddExistingPHFormOutputfield(phQuery, outputField)
                    Next

                    PHQueryDao.Evict(currentPHQuery)
                End If

                dao.Evict(currentQuery)
            End If

            ReportTitleLabel.Text = query.Title

            QueryDao.CommitChanges()
            dao.Evict(query)

            If (phQuery IsNot Nothing) Then
                PHQueryDao.CommitChanges()
                PHQueryDao.Evict(phQuery)
            End If

            InitSavedQueries()
        End Sub

        Protected Sub SaveReportButton_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles SaveQueryButton.Click
            ErrorPanel.Visible = False

            Dim query As UserQuery = QueryDao.GetById(EditId)
            Dim phQuery As PHUserQuery = PHQueryDao.GetByBaseQueryId(EditId)

            ' Check to make sure that if a shared report is being saved it has a different name than the other
            ' shared reports.
            If (chkSaveShared.Checked = True) Then
                ' Get all shared user queries
                Dim queries = QueryDao.GetSharedUserQueries()
                Dim currentPHQuery As PHUserQuery = Nothing
                Dim foundMatch As Boolean = False

                For Each q As UserQuery In queries
                    If (q.Id = query.Id) Then
                        Continue For
                    End If

                    foundMatch = False
                    currentPHQuery = PHQueryDao.GetByBaseQueryId(q.Id)

                    If (q.Title.Equals(SaveTitlebox.Text.Trim)) Then
                        Dim errors As New StringCollection()
                        errors.Add("Report with title """ & q.Title & """ already exists! Please save report with a new title.")
                        ErrorPanel.Visible = True
                        ErrorList.DataSource = errors
                        ErrorList.DataBind()
                        Exit Sub
                    End If

                    ' Check if there already exists a query that matches this query...
                    If (q.Equals(query) = True) Then
                        If (phQuery Is Nothing AndAlso currentPHQuery Is Nothing) Then
                            foundMatch = True
                        ElseIf (phQuery IsNot Nothing AndAlso currentPHQuery IsNot Nothing AndAlso currentPHQuery.Equals(phQuery) = True) Then
                            foundMatch = True
                        End If
                    End If

                    If (foundMatch) Then
                        Dim errors As New StringCollection()
                        errors.Add("A shared report with these exact criteria already exists. That report is titled """ & q.Title & """. The """ & SaveTitlebox.Text.Trim() & """ report has not been saved.")
                        ErrorPanel.Visible = True
                        ErrorList.DataSource = errors
                        ErrorList.DataBind()
                        Exit Sub
                    End If
                Next
            End If

            query.FieldList = SelectedFields.Text
            query.SortFields = SortFields.Text
            query.Title = Server.HtmlEncode(SaveTitlebox.Text.Trim)
            query.Transient = False
            query.Shared = chkSaveShared.Checked
            query.ModifiedDate = DateTime.Now

            ReportTitleLabel.Text = query.Title

            QueryDao.CommitChanges()

            If (phQuery IsNot Nothing) Then
                phQuery.SortField = GetPHFormSortField()
                PHQueryDao.CommitChanges()
                PHQueryDao.Evict(phQuery)
            End If

            dao.Evict(query)

            InitSavedQueries()
        End Sub

        Protected Sub SourceSelect_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles SourceSelect.SelectedIndexChanged
            ErrorPanel.Visible = False
            InitSourceValue(CInt(SourceSelect.SelectedValue))
        End Sub

        Protected Sub UpdateOutputFields()

            Dim SEPERATOR As String = "|"
            Dim query As UserQuery = Nothing

            ' Check if there is a query to load
            If (EditId = 0) Then
                UnselectedFields.Text = ""
                SelectedFields.Text = ""
                'ElseIf () Then
            Else
                query = QueryDao.GetById(EditId)
                SelectedFields.Text = query.FieldList
            End If

            'the unselected list is the total field list, minus the selected fields
            Dim fields As IList(Of QueryOutputField) = QueryDao.GetOutputFields(ddlCaseType.SelectedValue)
            Dim unselected As String

            If (query IsNot Nothing) AndAlso (Not String.IsNullOrEmpty(query.FieldList)) Then
                Dim tmp = From t In fields Select t.FieldName
                Dim selected = From t In query.FieldList.Split(New String() {SEPERATOR}, StringSplitOptions.RemoveEmptyEntries).ToList() Select t
                Dim data = tmp.Except(selected)
                unselected = String.Join(SEPERATOR, (From d In data Select d Order By d).ToArray())
            Else
                unselected = String.Join(SEPERATOR, (From t In fields Select t.FieldName Order By FieldName).ToArray())
            End If

            'now we want to sort the remaining fields
            If (query IsNot Nothing) Then
                SortFields.Text = query.SortFields
            Else
                SortFields.Text = ""
            End If

            UnselectedFields.Text = unselected

            ' Update PH Form Field Output Field controls...
            lsbOutputPHFormFields.Items.Clear()

            If (EditId <> 0 AndAlso IsPHFormCaseTypeSelected()) Then
                Dim phQuery As PHUserQuery = PHQueryDao.GetByBaseQueryId(EditId)

                If (phQuery IsNot Nothing) Then
                    ' Update the outputfield list...
                    For Each outputField As PHQueryOutputField In phQuery.OutputFields
                        lsbOutputPHFormFields.Items.Add(New ListItem(PHDao.GetSectionById(outputField.SectionId).Name + " | " + PHDao.GetFieldById(outputField.FieldId).Name + " | " + PHDao.GetFieldTypeById(outputField.FieldTypeId).Name, outputField.Id))
                    Next

                    ' Update the sort field radio buttons
                    UpdatePHSortFieldControls(phQuery.SortField)
                End If
            End If
        End Sub

        ' This method adds a copy of a QueryParameter to a UserQuery. Meant to be used for adding a copy of a parameter
        ' from one query into a different query.
        Private Sub AddExistingParameter(ByRef query As UserQuery, ByRef param As QueryParameter)

            If (query Is Nothing OrElse param Is Nothing) Then
                Exit Sub
            End If

            Dim newParam As New QueryParameter()

            ' Copy values over from the existing parameter to the new one
            newParam.Source = param.Source
            newParam.ExecuteOrder = param.ExecuteOrder
            newParam.WhereType = param.WhereType
            newParam.Operator = param.Operator
            newParam.StartDisplay = param.StartDisplay
            newParam.StartValue = param.StartValue
            newParam.EndDisplay = param.EndDisplay
            newParam.EndValue = param.EndValue

            query.ModifiedDate = DateTime.Now
            query.AddParameter(newParam)
            QueryDao.CommitChanges()

            QueryDao.UpdateParamExecuteOrders(newParam.Clause.Id, newParam.Id, newParam.ExecuteOrder, newParam.ExecuteOrder, 1)   ' filter = 1 means an insert occured

        End Sub

        Private Sub AddExistingPHFormOutputfield(ByRef phQuery As PHUserQuery, ByRef outputField As PHQueryOutputField)
            If (phQuery Is Nothing OrElse outputField Is Nothing) Then
                Exit Sub
            End If

            Dim newOutputField As New PHQueryOutputField()

            ' Copy the properties from the existing output field to the new one
            newOutputField.SectionId = outputField.SectionId
            newOutputField.FieldId = outputField.FieldId
            newOutputField.FieldTypeId = outputField.FieldTypeId

            If (phQuery.ContainsOutputField(newOutputField)) Then
                Exit Sub
            End If

            phQuery.AddOutputField(newOutputField)

            phQuery.UserQuery.ModifiedDate = DateTime.Now
            PHQueryDao.CommitChanges()
            QueryDao.CommitChanges()
        End Sub

        ' This method adds a copy of a PHQueryParameter to a PHUserQuery. Meant to be used for adding a copy of a parameter
        ' from one query into a different query.
        Private Sub AddExistingPHFormParameter(ByRef query As PHUserQuery, ByRef param As PHQueryParameter)
            If (query Is Nothing OrElse param Is Nothing) Then
                Exit Sub
            End If

            Dim newParam As New PHQueryParameter()

            ' Copy values over from the existing parameter to the new one
            newParam.ExecuteOrder = param.ExecuteOrder
            newParam.WhereType = param.WhereType
            newParam.Operator = param.Operator
            newParam.StartDisplay = param.StartDisplay
            newParam.StartValue = param.StartValue
            newParam.EndDisplay = param.EndDisplay
            newParam.EndValue = param.EndValue

            query.UserQuery.ModifiedDate = DateTime.Now
            query.AddParameter(newParam)
            PHQueryDao.CommitChanges()
            QueryDao.CommitChanges()

            PHQueryDao.UpdateParamExecuteOrders(newParam.Clause.Id, newParam.Id, newParam.ExecuteOrder, newParam.ExecuteOrder, 1)   ' filter = 1 means an insert occured
        End Sub

        Private Sub AddPHFormFieldParameter()
            If (EditId = 0) Then
                Exit Sub
            End If

            ' Validate the inputs before we do anything
            If (Not ValidateParamInput()) Then
                Exit Sub
            End If

            Dim phQuery As PHUserQuery = PHQueryDao.GetByBaseQueryId(EditId)

            If (phQuery Is Nothing) Then
                Exit Sub
            End If

            Dim phParameter As PHQueryParameter = New PHQueryParameter()

            If (phParameter Is Nothing) Then
                Exit Sub
            End If

            ' Assign parameter values...
            phParameter.ExecuteOrder = Server.HtmlEncode(txtExecuteOrder.Text)
            phParameter.Operator = GetSourceOperator()

            If (ParamTypeAnd.Checked) Then
                phParameter.WhereType = WhereType.AND
            Else
                phParameter.WhereType = WhereType.OR
            End If

            GetPHFormFieldSourceValues(phParameter)

            phQuery.AddParameter(phParameter)

            ' Check if the parameter should be removed...
            If (phQuery.UserQuery.Shared = True) Then
                ' Get all shared user queries
                Dim queries = QueryDao.GetSharedUserQueries()
                Dim currentPHQuery As PHUserQuery = Nothing

                For Each q As UserQuery In queries
                    If (q.Id = phQuery.UserQuery.Id) Then
                        Continue For
                    End If

                    currentPHQuery = PHQueryDao.GetByBaseQueryId(q.Id)

                    If (currentPHQuery Is Nothing) Then
                        Continue For
                    End If

                    ' Check if there already exists a query that matches this query...
                    If (q.Equals(phQuery.UserQuery) = True AndAlso currentPHQuery.Equals(phQuery) = True) Then
                        Dim errors As New StringCollection()
                        errors.Add("A shared report with these exact criteria already exists. That report is titled """ & q.Title & """.  Criteria not added to the """ & SaveTitlebox.Text.Trim() & """ report.")
                        ErrorPanel.Visible = True
                        ErrorList.DataSource = errors
                        ErrorList.DataBind()

                        phQuery.RemoveParameter(phParameter)
                        PHQueryDao.DeleteParameter(phParameter)
                        PHQueryDao.CommitChanges()
                        Exit Sub
                    End If
                Next
            End If

            ' Commit changes to database...
            phQuery.UserQuery.ModifiedDate = DateTime.Now

            PHQueryDao.CommitChanges()
            QueryDao.CommitChanges()

            PHQueryDao.UpdateParamExecuteOrders(phParameter.Clause.Id, phParameter.Id, phParameter.ExecuteOrder, phParameter.ExecuteOrder, 1)   ' filter = 1 means an insert occured

            ' Update UI controls...
            UpdateExecuteOrderTextbox()
            UpdateCurrentPHFormFieldQueryGrid()
            ResetCriteriaPanel()
        End Sub

        Private Sub AddWorkflowParameter()

            If (EditId = 0) Then
                Exit Sub
            End If

            'validate the inputs before we do anything
            If (Not ValidateParamInput()) Then
                Exit Sub
            End If

            'we have valid inputs, save it to the collection
            Dim query As UserQuery = QueryDao.GetById(EditId)
            Dim param As New QueryParameter()
            Dim source As InputSource = QueryDao.GetSourceById(CInt(SourceSelect.SelectedValue))
            param.Source = source

            param.ExecuteOrder = Server.HtmlEncode(txtExecuteOrder.Text)

            If (ParamTypeAnd.Checked) Then
                param.WhereType = WhereType.AND
            Else
                param.WhereType = WhereType.OR
            End If

            param.Operator = GetSourceOperator()
            GetSourceValues(param)

            query.AddParameter(param)

            ' Check if the parameter should be removed...
            If (query.Shared = True) Then
                ' Get all shared user queries
                Dim queries = QueryDao.GetSharedUserQueries()

                For Each q As UserQuery In queries
                    ' Check if there already exists a query that matches this query...
                    If (q.Id <> query.Id AndAlso q.Equals(query) = True) Then
                        Dim errors As New StringCollection()
                        errors.Add("A shared report with these exact criteria already exists. That report is titled """ & q.Title & """.   """ & param.Source.DisplayName & """ criteria not added to the """ & SaveTitlebox.Text.Trim() & """ report.")
                        ErrorPanel.Visible = True
                        ErrorList.DataSource = errors
                        ErrorList.DataBind()

                        query.RemoveParameter(param)
                        QueryDao.DeleteParameter(param)
                        QueryDao.CommitChanges()
                        Exit Sub
                    End If
                Next
            End If

            query.ModifiedDate = DateTime.Now

            QueryDao.CommitChanges()

            QueryDao.UpdateParamExecuteOrders(param.Clause.Id, param.Id, param.ExecuteOrder, param.ExecuteOrder, 1)   ' filter = 1 means an insert occured

            UpdateExecuteOrderTextbox()
            UpdateCurrentWorkflowQueryGrid()
            ResetCriteriaPanel()

        End Sub

        Private Sub ClearCurrentTransient()
            'the user is starting a new report.  If they have a transient report, clear it out
            'otherwise, start a new transient report
            Dim reports As IList(Of UserQuery) = QueryDao.GetUserQueries(SESSION_USER_ID)

            Dim transient As UserQuery = (From r In reports
                                          Where r.Transient = True
                                          Select r Take 1).SingleOrDefault()

            Dim phTransient As PHUserQuery = Nothing

            If (transient Is Nothing) Then
                'this user doesn't have a transient report, start a new one
                transient = New UserQuery()
                transient.User = UserService.CurrentUser()
                transient.Title = "User Query"
                transient.Transient = True
                transient.Shared = False
                transient.CreatedDate = DateTime.Now
                transient.ModifiedDate = DateTime.Now
                QueryDao.SaveOrUpdateQuery(transient)
            End If

            phTransient = PHQueryDao.GetByBaseQueryId(transient.Id)

            ' Check if the PH Transient query needs to be deleted or if one needs to be created...
            If (phTransient IsNot Nothing AndAlso IsPHFormCaseTypeSelected() = False) Then
                For Each clause As PHQueryClause In phTransient.Clauses
                    PHQueryDao.DeleteClause(clause)
                Next

                For Each outputField As PHQueryOutputField In phTransient.OutputFields
                    PHQueryDao.DeleteOutputField(outputField)
                Next

                phTransient.OutputFields.Clear()
                phTransient.Clauses.Clear()

                PHQueryDao.Delete(phTransient)
                PHQueryDao.CommitChanges()
                PHQueryDao.Evict(phTransient)
            ElseIf (phTransient Is Nothing AndAlso IsPHFormCaseTypeSelected() = True) Then
                phTransient = New PHUserQuery()
                phTransient.UserQuery = transient
                phTransient.ReportType = 1 'Integer.Parse(rblReportType.SelectedValue)
                PHQueryDao.SaveOrUpdateQuery(phTransient)
            End If

            EditId = transient.Id

            If SaveAsNew = False Then
                'so we now have a query, clear out any existing clauses (which will cascade delete any parameters)
                For Each clause As QueryClause In transient.Clauses
                    QueryDao.DeleteClause(clause)
                Next

                ' If editing PH Form query then clear out existing clauses and output fields...
                If (IsPHFormCaseTypeSelected() AndAlso phTransient IsNot Nothing) Then
                    For Each clause As PHQueryClause In phTransient.Clauses
                        PHQueryDao.DeleteClause(clause)
                    Next

                    For Each outputField As PHQueryOutputField In phTransient.OutputFields
                        PHQueryDao.DeleteOutputField(outputField)
                    Next

                    phTransient.OutputFields.Clear()
                    phTransient.Clauses.Clear()
                    phTransient.ReportType = 1 'Integer.Parse(rblReportType.SelectedValue)
                End If

                'EditId = transient.Id

                transient.Clauses.Clear()
                transient.Title = "User Query"
                transient.FieldList = ""
                transient.SortFields = ""
                transient.ModifiedDate = DateTime.Now

                QueryDao.CommitChanges()
                QueryDao.Evict(transient)

                If (IsPHFormCaseTypeSelected() AndAlso phTransient IsNot Nothing) Then
                    PHQueryDao.CommitChanges()
                    PHQueryDao.Evict(phTransient)
                End If

                LoadUserQuery()
                'new added
                'SrcNameHdn.Value = ""
            Else
                AddWorkflowParameter()
                QueryDao.CommitChanges()
                QueryDao.Evict(transient)
            End If

            ' Make sure the report editing controls are turned on
            TurnReportEditingControlsOnOff(True)
        End Sub

        ' The user has chosen to load a shared report that they do not own. Copy the clauses and parameters
        ' of the shared report into a transient report.
        Private Function CopySharedReportToTransientReport(ByVal sharedReport As UserQuery) As Integer
            If (sharedReport Is Nothing) Then
                Return 0
            End If

            Dim sharedPHReport As PHUserQuery = PHQueryDao.GetByBaseQueryId(sharedReport.Id)

            ' If they have a transient report, clear it out otherwise, start a new transient report
            Dim reports As IList(Of UserQuery) = QueryDao.GetUserQueries(SESSION_USER_ID)

            Dim transient As UserQuery = (From r In reports
                                          Where r.Transient = True
                                          Select r Take 1).SingleOrDefault()

            Dim phTransient As PHUserQuery = Nothing

            If (transient Is Nothing) Then
                'this user doesn't have a transient report, start a new one
                transient = New UserQuery()
                transient.User = UserService.CurrentUser()
                transient.Title = "User Query"
                transient.Transient = True
                transient.Shared = False
                transient.CreatedDate = DateTime.Now
                transient.ModifiedDate = DateTime.Now
                QueryDao.SaveOrUpdateQuery(transient)
            End If

            'so we now have a query, clear out any existing clauses (which will cascade delete any parameters)
            For Each clause As QueryClause In transient.Clauses
                QueryDao.DeleteClause(clause)
            Next

            transient.Clauses.Clear()

            phTransient = PHQueryDao.GetByBaseQueryId(transient.Id)

            ' Check if the PH Transient query needs to be deleted, if it needs to be cleared out, or if one needs to be created...
            If (phTransient IsNot Nothing AndAlso sharedReport Is Nothing) Then
                For Each clause As PHQueryClause In phTransient.Clauses
                    PHQueryDao.DeleteClause(clause)
                Next

                For Each outputField As PHQueryOutputField In phTransient.OutputFields
                    PHQueryDao.DeleteOutputField(outputField)
                Next
                phTransient.OutputFields.Clear()
                phTransient.Clauses.Clear()

                PHQueryDao.Delete(phTransient)
                PHQueryDao.CommitChanges()
                PHQueryDao.Evict(phTransient)
            ElseIf (phTransient IsNot Nothing AndAlso sharedPHReport IsNot Nothing) Then
                For Each clause As PHQueryClause In phTransient.Clauses
                    PHQueryDao.DeleteClause(clause)
                Next

                For Each outputField As PHQueryOutputField In phTransient.OutputFields
                    PHQueryDao.DeleteOutputField(outputField)
                Next

                phTransient.OutputFields.Clear()
                phTransient.Clauses.Clear()
                phTransient.ReportType = 1 'Integer.Parse(rblReportType.SelectedValue)
            ElseIf (phTransient Is Nothing AndAlso sharedPHReport IsNot Nothing) Then
                phTransient = New PHUserQuery()
                phTransient.UserQuery = transient
                phTransient.ReportType = 1 'Integer.Parse(rblReportType.SelectedValue)
                PHQueryDao.SaveOrUpdateQuery(phTransient)
            End If

            ' Set the properties of the transient report to be the same as the shared report with a few small changes
            If (sharedReport IsNot Nothing) Then
                transient.Title = sharedReport.Title & " (copy)"
                transient.FieldList = sharedReport.FieldList
                transient.SortFields = sharedReport.SortFields
                transient.ModifiedDate = DateTime.Now

                ' Copy the parameters of the shared report to the transient report
                For Each clause As QueryClause In sharedReport.Clauses
                    For Each param As QueryParameter In clause.Parameters
                        AddExistingParameter(transient, param)
                    Next
                Next

                If (sharedPHReport IsNot Nothing AndAlso phTransient IsNot Nothing) Then
                    ' Copy the PH Form parameters
                    For Each clause As PHQueryClause In sharedPHReport.Clauses
                        For Each param As PHQueryParameter In clause.Parameters
                            AddExistingPHFormParameter(phTransient, param)
                        Next
                    Next

                    ' Copy the PH Form output fields
                    For Each outputField As PHQueryOutputField In sharedPHReport.OutputFields
                        AddExistingPHFormOutputfield(phTransient, outputField)
                    Next

                    phTransient.SortField = sharedPHReport.SortField
                End If
            End If

            QueryDao.CommitChanges()

            If (phTransient IsNot Nothing) Then
                PHQueryDao.CommitChanges()
                PHQueryDao.Evict(phTransient)
            End If

            QueryDao.Evict(transient)

            Return transient.Id
        End Function

        Private Sub GetPHFormFieldSourceValues(ByVal param As PHQueryParameter)
            ' At the moment only the Total criteria exists for PH Form Field source options, so only values from the DataNumber fields needs to be accessed...
            If (DataNumber.Visible) Then
                param.StartValue = Server.HtmlEncode(NumberStart.Text.Trim)
                param.StartDisplay = Server.HtmlEncode(NumberStart.Text.Trim)

                If (OperatorNumber.SelectedValue = QueryOperator.BETWEEN) Then
                    param.EndValue = Server.HtmlEncode(NumberStop.Text.Trim)
                    param.EndDisplay = Server.HtmlEncode(NumberStop.Text.Trim)
                End If
            End If
        End Sub

        Private Sub GetPHFormGridViewSourceValues(ByVal editIndex As Integer, ByRef param As PHQueryParameter)
            Dim row As GridViewRow = gdvPHQuery.Rows(editIndex)
            Dim gridOperatorDDL As DropDownList = row.FindControl("ddlGridOperator")

            ' Get the edit controls needed to figure out what kind of param is being edited.
            Dim txtNumberStart As TextBox = row.FindControl("txtGridNumberStart")

            If (txtNumberStart.Visible) Then
                ' Get the controls specific to a Number parameter
                Dim txtNumberEnd As TextBox = row.FindControl("txtGridNumberEnd")

                param.StartValue = Server.HtmlEncode(txtNumberStart.Text.Trim)
                param.StartDisplay = param.StartValue

                If (gridOperatorDDL.SelectedValue = QueryOperator.BETWEEN) Then
                    If (txtNumberEnd.Visible = True) Then
                        param.EndValue = Server.HtmlEncode(txtNumberEnd.Text.Trim)
                        param.EndDisplay = param.EndValue
                    Else
                        param.EndValue = Server.HtmlEncode(txtNumberStart.Text.Trim)
                        param.EndDisplay = param.EndValue
                    End If
                Else
                    param.EndValue = String.Empty
                    param.EndDisplay = String.Empty
                End If

            End If
        End Sub

        Private Function GetPHFormSortField() As String
            If (rblPHFormSortByField.SelectedIndex = -1 OrElse rblPHFormSortByOrder.SelectedIndex = -1) Then
                Return String.Empty
            End If

            Return "[" + rblPHFormSortByField.SelectedItem.Text + "] " + rblPHFormSortByOrder.SelectedItem.Text
        End Function

        Private Function GetSourceOperator() As String
            'new added
            If (Len(SrcNameHdn.Value) > 0 AndAlso SrcNameHdn.Value.Equals(DataChoiceText.Text) = True AndAlso cbSubUnit.Visible = True AndAlso cbSubUnit.Checked = True) Then
                Return QueryOperator.EQUALS
            End If

            If (DataText.Visible) Then
                Return QueryOperator.LIKE
            End If

            If (DataNumber.Visible) Then
                Return OperatorNumber.SelectedValue
            End If

            If (DataDates.Visible) Then
                Return OperatorDate.SelectedValue
            End If

            If (DataChoice.Visible) Then
                Return ChoiceOperator.SelectedValue
            End If

            Return QueryOperator.EQUALS

        End Function

        Private Sub GetSourceValues(ByRef param As QueryParameter)

            If (DataText.Visible) Then
                param.StartDisplay = Server.HtmlEncode(DataChoiceText.Text.Trim)
                param.StartValue = param.StartDisplay

                ' If the new parameter is a Unit Name criteria perform additional steps
                If (IsUnitNameSource(param.Source.DisplayName)) Then

                    ' If the user used the "Find Unit" control get the unit name from that
                    If (Len(SrcNameHdn.Value) > 0) Then
                        param.StartDisplay = Server.HtmlEncode(DataChoiceText.Text.Trim.Substring(0, DataChoiceText.Text.Length - 6))
                        param.StartValue = param.StartDisplay
                    End If

                    ' If the user checked the "Include Subordinate Unit" checkbox then set the end value to the ID of selected unit unit
                    If (cbSubUnit.Checked) Then
                        param.EndValue = SrcUnitIdHdn.Value     ' EQUALS Parent unit ID
                        cbSubUnit.Checked = False
                    End If

                End If
            End If

            If (SourceSelect.SelectedItem.Text = "Last Updated") Then
                param.Source.FieldName = "Date Modified"
            End If

            If (DataNumber.Visible) Then
                param.StartValue = Server.HtmlEncode(NumberStart.Text.Trim)
                param.StartDisplay = Server.HtmlEncode(NumberStart.Text.Trim)

                If (OperatorNumber.SelectedValue = QueryOperator.BETWEEN) Then
                    param.EndValue = Server.HtmlEncode(NumberStop.Text.Trim)
                    param.EndDisplay = Server.HtmlEncode(NumberStop.Text.Trim)
                End If
            End If

            If (DataDates.Visible) Then
                'if these are exact dates, grab those
                If (DateExact.Checked) Then

                    param.StartValue = Server.HtmlEncode(DateStart.Text.Trim)
                    param.StartDisplay = Server.HtmlEncode(DateStart.Text.Trim)

                    If (OperatorDate.SelectedValue = QueryOperator.BETWEEN) Then
                        param.EndValue = Server.HtmlEncode(DateStop.Text.Trim)
                        param.EndDisplay = Server.HtmlEncode(DateStop.Text.Trim)
                    End If
                    'end exact
                Else

                    param.StartValue = Server.HtmlEncode(RangeStart.Text.Trim) + RangeStartSelect.SelectedValue
                    param.StartDisplay = Server.HtmlEncode(RangeStart.Text.Trim) + " " + RangeStartSelect.SelectedItem.Text

                    If (OperatorDate.SelectedValue = QueryOperator.BETWEEN) Then
                        param.EndValue = Server.HtmlEncode(RangeEnd.Text.Trim) + RangeEndSelect.SelectedValue
                        param.EndDisplay = Server.HtmlEncode(RangeEnd.Text.Trim) + " " + RangeEndSelect.SelectedItem.Text
                    End If

                    'end span
                End If

            End If 'end dates

            If (DataChoice.Visible) Then
                param.StartValue = DataChoiceSelect.SelectedValue
                param.StartDisplay = DataChoiceSelect.SelectedItem.Text
            End If

            If (DataBool.Visible) Then
                param.StartDisplay = DataBoolSelect.SelectedItem.Text
                param.StartValue = DataBoolSelect.SelectedValue
            End If

        End Sub

        Private Sub GetWorkflowGridViewSourceValues(ByVal editIndex As Integer, ByRef param As QueryParameter)
            Dim row As GridViewRow = QueryGrid.Rows(editIndex)
            Dim gridOperatorDDL As DropDownList = row.FindControl("ddlGridOperator")

            ' Get the edit controls needed to figure out what kind of param is being edited.
            Dim ddlChoice As DropDownList = row.FindControl("ddlGridChoice")
            Dim txtText As TextBox = row.FindControl("txtGridText")
            Dim ddlBool As DropDownList = row.FindControl("ddlGridBool")
            Dim txtDateStart As TextBox = row.FindControl("txtGridDateStart")
            Dim txtRangeStart As TextBox = row.FindControl("txtGridRangeStart")
            Dim txtNumberStart As TextBox = row.FindControl("txtGridNumberStart")

            If (txtText.Visible) Then

                param.StartDisplay = Server.HtmlEncode(txtText.Text.Trim)
                param.StartValue = param.StartDisplay

                ' If the parameter is a Unit Name criteria perform additional steps
                If (IsUnitNameSource(param.Source.DisplayName)) Then

                    ' Get the controls specific to a "Unit Name" Text parameter
                    Dim cbGridSubUnit As CheckBox = row.FindControl("cbGridSubUnit")
                    Dim hdnSrcId As HtmlInputHidden = row.FindControl("hdnGridUnitIdClient")
                    Dim hdnSrcName As HtmlInputHidden = row.FindControl("hdnGridUnitNameClient")

                    ' If the user used the "Find Unit" control get the unit name from that
                    If (Len(hdnSrcName.Value) > 0) Then
                        param.StartDisplay = hdnSrcName.Value.Substring(0, hdnSrcName.Value.Length - 6)
                        param.StartValue = param.StartDisplay
                    End If

                    ' If the "Include Subordinate Unit" checkbox has been checked save some additional data
                    If (cbGridSubUnit.Checked) Then

                        ' If a new unit has been selected from the "Find Unit" control
                        If (String.IsNullOrEmpty(hdnSrcId.Value) = False) Then
                            param.EndValue = hdnSrcId.Value
                        Else ' This is for the case where an include sub unit param is being edited, but a new unit was not selected, therefore we need to save the old unit
                            param.EndValue = param.EndValue ' This is technically a redundant assignment, but it is done for clarity on what is happening in this else clause
                            param.StartDisplay = Server.HtmlEncode(txtText.Text.Substring(0, txtText.Text.Length - 6))
                            param.StartValue = param.StartDisplay
                        End If
                    Else
                        param.EndValue = String.Empty
                    End If

                End If

            End If

            If (txtNumberStart.Visible) Then

                ' Get the controls specific to a Number parameter
                Dim txtNumberEnd As TextBox = row.FindControl("txtGridNumberEnd")

                param.StartValue = Server.HtmlEncode(txtNumberStart.Text.Trim)
                param.StartDisplay = param.StartValue

                If (gridOperatorDDL.SelectedValue = QueryOperator.BETWEEN) Then
                    If (txtNumberEnd.Visible = True) Then
                        param.EndValue = Server.HtmlEncode(txtNumberEnd.Text.Trim)
                        param.EndDisplay = param.EndValue
                    Else
                        param.EndValue = Server.HtmlEncode(txtNumberStart.Text.Trim)
                        param.EndDisplay = param.EndValue
                    End If
                Else
                    param.EndValue = String.Empty
                    param.EndDisplay = String.Empty
                End If

            End If

            If (txtDateStart.Visible = True Or txtRangeStart.Visible = True) Then

                'if these are exact dates, grab those
                If (txtDateStart.Visible) Then

                    ' Get controls specific to a specific Date parameter
                    Dim txtDateEnd As TextBox = row.FindControl("txtGridDateEnd")

                    param.StartValue = Server.HtmlEncode(txtDateStart.Text.Trim)
                    param.StartDisplay = param.StartValue

                    If (gridOperatorDDL.SelectedValue = QueryOperator.BETWEEN) Then
                        If (txtDateEnd.Visible = True) Then
                            param.EndValue = Server.HtmlEncode(txtDateEnd.Text.Trim)
                            param.EndDisplay = param.EndValue
                        Else
                            param.EndValue = Server.HtmlEncode(txtDateStart.Text.Trim)
                            param.EndDisplay = param.EndValue
                        End If
                    Else
                        param.EndValue = String.Empty
                        param.EndDisplay = String.Empty
                    End If
                    'end exact
                Else

                    ' Get controls specific to a range Date parameter
                    Dim ddlRangeStartSelect As DropDownList = row.FindControl("ddlGridRangeStartSelect")
                    Dim txtRangeEnd As TextBox = row.FindControl("txtGridRangeEnd")
                    Dim ddlRangeEndSelect As DropDownList = row.FindControl("ddlGridRangeEndSelect")

                    param.StartValue = Server.HtmlEncode(txtRangeStart.Text.Trim) + ddlRangeStartSelect.SelectedValue
                    param.StartDisplay = Server.HtmlEncode(txtRangeStart.Text.Trim) + " " + ddlRangeStartSelect.SelectedItem.Text

                    If (gridOperatorDDL.SelectedValue = QueryOperator.BETWEEN) Then
                        If (txtRangeEnd.Visible = True) Then
                            param.EndValue = Server.HtmlEncode(txtRangeEnd.Text.Trim) + ddlRangeEndSelect.SelectedValue
                            param.EndDisplay = Server.HtmlEncode(txtRangeEnd.Text.Trim) + " " + ddlRangeEndSelect.SelectedItem.Text
                        Else
                            param.EndValue = Server.HtmlEncode(txtRangeStart.Text.Trim) + ddlRangeStartSelect.SelectedValue
                            param.EndDisplay = Server.HtmlEncode(txtRangeStart.Text.Trim) + " " + ddlRangeStartSelect.SelectedItem.Text
                        End If
                    Else
                        param.EndValue = String.Empty
                        param.EndDisplay = String.Empty
                    End If

                End If

            End If 'end dates

            If (ddlChoice.Visible) Then
                param.StartValue = ddlChoice.SelectedValue
                param.StartDisplay = ddlChoice.SelectedItem.Text
            End If

            If (ddlBool.Visible) Then
                param.StartDisplay = ddlBool.SelectedItem.Text
                param.StartValue = ddlBool.SelectedValue
            End If
        End Sub

        Private Sub HideCriteriaValuePanels()
            DataChoice.Visible = False
            DataText.Visible = False
            DataDates.Visible = False
            DataNumber.Visible = False
            DataBool.Visible = False
            pnlPHFormFieldUnsupported.Visible = False
        End Sub

        Private Sub InitCaseTypeDropDown()
            ' The value of the list items is the view table that needs to be used as the source
            ' for the items in the case criteria dropdown and the output fields listbox.
            If (UserHasPermission("lodAdHocReports") = True) Then
                ddlCaseType.Items.Add(New ListItem("Line of Duty", LOD_SOURCE))
            End If

            If (UserHasPermission("lodAdHocAuditReport") = True) Then
                ddlCaseType.Items.Add(New ListItem("Line of Duty Audit", LOD_SOURCE_AUDIT))
            End If

            If (UserHasPermission("reinvestigate") = True) Then
                ddlCaseType.Items.Add(New ListItem("Reinvestigation Request", RR_SOURCE))
            End If

            If (UserHasPermission("APSearch") = True) Then
                ddlCaseType.Items.Add(New ListItem("Appeal Request", AP_SOURCE))
            End If

            If (UserHasPermission(PERMISSION_SARC_ADHOC_REPORTS)) Then
                ddlCaseType.Items.Add(New ListItem("Restricted SARC", SARC_SOURCE))
                ddlCaseType.Items.Add(New ListItem("SARC Appeals", APSA_SOURCE))
            End If

            If (UserHasPermission("scSearch") = True) Then
                ddlCaseType.Items.Add(New ListItem("Special Case", SC_SOURCE))
            End If

            If (ddlCaseType.Items.Count > 1) Then
                ddlCaseType.Items.Add(New ListItem("All Cases", ALL_SOURCE))
            End If

            If (UserHasPermission("PHAdHocReports") = True) Then
                ddlCaseType.Items.Add(New ListItem("PH Form", PH_SOURCE))
            End If

            ddlCaseType.SelectedIndex = 0   ' Default to first item
        End Sub

        Private Sub InitGridViewInputSources(ByRef ddl As DropDownList)

            'populate our list of input sources (fields)
            Dim sources As IList(Of InputSource) = QueryDao.GetInputSourcesByTableName(ddlCaseType.SelectedValue)
            ddl.DataSource = From s In sources Select s Order By s.DisplayName
            ddl.DataBind()

        End Sub

        Private Sub InitInputSources()
            SourceSelect.Visible = True
            ddlPHSection.Visible = False
            ddlPHField.Visible = False
            ddlPHFieldType.Visible = False

            'populate our list of input sources (fields)
            Dim sources As IList(Of InputSource) = QueryDao.GetInputSourcesByTableName(ddlCaseType.SelectedValue)
            SourceSelect.DataSource = From s In sources Select s Order By s.DisplayName
            SourceSelect.DataBind()
            SourceSelect.Items.Insert(0, New ListItem("-- Select Field --", 0))

            SourceSelect.SelectedIndex = 0
        End Sub

        Private Sub InitPHFormGridViewOperators(ByRef param As PHQueryParameter, ByRef ddl As DropDownList)
            ' Currently only the numeric totals parameter is supported, so only use the numeric related operators...
            ddl.Items.Add(New ListItem("Equals", QueryOperator.EQUALS))
            ddl.Items.Add(New ListItem("Less Than", QueryOperator.LESS_THAN))
            ddl.Items.Add(New ListItem("Greater Than", QueryOperator.GREATER_THAN))
            ddl.Items.Add(New ListItem("Between", QueryOperator.BETWEEN))
        End Sub

        Private Sub InitPHFormGridViewValues(ByRef param As PHQueryParameter, ByRef e As System.Web.UI.WebControls.GridViewRowEventArgs)
            Dim hasEndValue As Boolean = False

            If (Not String.IsNullOrEmpty(param.EndValue)) Then
                hasEndValue = True
            End If

            ' Get all of the edit controls for the param values
            Dim txtNumberStart As TextBox = e.Row.FindControl("txtGridNumberStart")
            Dim txtNumberEnd As TextBox = e.Row.FindControl("txtGridNumberEnd")
            Dim lblNumAnd As Label = e.Row.FindControl("lblGridNumAnd")

            ' Reset visibilities of edit controls
            txtNumberStart.Visible = False
            txtNumberEnd.Visible = True
            lblNumAnd.Visible = True

            ' Configure our options based on the data type of the selected field...currently only the numeric Total criteria is available
            SetInputFormatRestriction(Page, txtNumberStart, FormatRestriction.Numeric)
            txtNumberStart.Visible = True
            txtNumberStart.Text = Server.HtmlDecode(param.StartValue)

            If (hasEndValue = True) Then
                SetInputFormatRestriction(Page, txtNumberEnd, FormatRestriction.Numeric)
                lblNumAnd.Visible = True
                txtNumberEnd.Visible = True
                txtNumberEnd.Text = Server.HtmlDecode(param.EndValue)
            End If
        End Sub

        Private Sub InitPHFormReportControls(ByVal initControls As Boolean)
            If (initControls) Then
                'trReportType.Visible = True
                trCriteriaType.Visible = True
                lblCriteriaRow.Text = "C-0"
                'rblReportType.SelectedIndex = 0
                'rblReportType_SelectedIndexChanged(rblReportType, New EventArgs())
                pnlWorkflowOutputUI.Visible = False
                pnlPHFormTotalsOutputUI.Visible = True
                PopulatePHDropdownLists(ddlOutputPHSection, ddlOutputPHField, ddlOutputPHFieldType)
                ddlOutputPHField.Enabled = False
                ddlOutputPHFieldType.Enabled = False
            Else
                'trReportType.Visible = False
                pnlWorkflowOutputUI.Visible = True
                pnlPHFormTotalsOutputUI.Visible = False
                trCriteriaType.Visible = False
                lblCriteriaRow.Text = "C"
            End If
        End Sub

        Private Sub InitPHInputSources()
            SourceSelect.Items.Clear()
            SourceSelect.Items.Add(New ListItem("Total", 1))
            SourceSelect.Items.Insert(0, New ListItem("-- Select Field --", 0))
            SourceSelect.Visible = True
            ddlPHSection.Visible = False
            ddlPHField.Visible = False
            ddlPHFieldType.Visible = False

            ' Determine which type of PH input sources to use...
            'If (rblReportType.SelectedValue.Equals("1")) Then       ' Totals...uses the existing SourceSelect dropdownlist and associated functions...
            '    SourceSelect.Items.Add(New ListItem("Total", 1))
            '    SourceSelect.Items.Insert(0, New ListItem("-- Select Field --", 0))
            '    SourceSelect.Visible = True
            '    ddlPHSection.Visible = False
            '    ddlPHField.Visible = False
            '    ddlPHFieldType.Visible = False
            'ElseIf (rblReportType.SelectedValue.Equals("2")) Then   ' Cases...uses a new set of three dropdownlists...
            '    PopulatePHDropdownLists(ddlPHSection, ddlPHField, ddlPHFieldType)
            '    SourceSelect.Visible = False
            '    ddlPHSection.Visible = True
            '    ddlPHField.Visible = True
            '    ddlPHFieldType.Visible = True
            'End If
        End Sub

        Private Sub InitPHSourceValueControls(ByVal id As Integer)
            If (id = 0) Then
                SourcePanel.Visible = False
                Exit Sub
            End If

            SourcePanel.Visible = True
            AddParamButton.Enabled = True
            HideCriteriaValuePanels()

            Dim fieldType As PHFieldType = PHDao.GetFieldTypeById(id)

            If (fieldType Is Nothing) Then
                SourcePanel.Visible = False
                Exit Sub
            End If

            Select Case fieldType.DataTypeId
                Case DataType.DT_Integer, DataType.DT_Numeric
                    DataNumber.Visible = True

                Case DataType.DT_String, DataType.DT_CSV
                    DataText.Visible = True
                    btnFindUnit.Visible = False
                    cbSubUnit.Visible = False
                    lblSubUnitInfo.Visible = False
                    lblSSNMsg.Visible = False

                    If ((fieldType.Length.HasValue) AndAlso (fieldType.DataTypeId = DataType.DT_String OrElse fieldType.DataTypeId = DataType.DT_CSV)) Then
                        DataChoiceText.MaxLength = fieldType.Length
                    End If

                    DataChoiceText.Width = 250

                Case Else
                    pnlPHFormFieldUnsupported.Visible = True
                    AddParamButton.Enabled = False
            End Select
        End Sub

        Private Sub InitSavedQueries()

            ' Get all of the queries created by the logged in user
            Dim queries = QueryDao.GetUserQueries(SESSION_USER_ID)

            ' Populate the Saved Reports dropdownlist
            InitUserQueries(queries)

            ' Populate the Shared Reports dropdownlist
            InitSharedUserQueries()

            ' Set the EditId to the last modified report
            SelectLastModifiedQueryToLoad(queries)

            ' Load the selected report
            LoadUserQuery()

            ' Set the selected values for the two saved user query dropdownlists
            If (QueryDao.GetById(EditId).Transient = False) Then
                If (MyQuerySelect.Items.FindByValue(EditId.ToString()) IsNot Nothing) Then
                    MyQuerySelect.SelectedValue = EditId.ToString()
                    ddlSharedReports.SelectedValue = "0"
                End If

                If (ddlSharedReports.Items.FindByValue(EditId.ToString()) IsNot Nothing) Then
                    ddlSharedReports.SelectedValue = EditId.ToString()
                    MyQuerySelect.SelectedValue = "0"
                End If
            End If

        End Sub

        Private Sub InitSharedUserQueries()
            ' Get all shared user queries
            Dim queries = QueryDao.GetSharedUserQueries()

            ' Load saved queries that are associated with the selected case type or queries
            ' that do not have any clauses.
            Dim selectedQueries As New List(Of UserQuery)
            Dim foundQuery As Boolean = False

            For Each query As UserQuery In queries

                ' Check if this query does not have any clauses
                ' If the query does not have any clauses, then it has not been associated with
                ' any particular case type yet.
                If (query.Clauses.Count = 0) Then
                    selectedQueries.Add(query)
                    Continue For
                End If

                ' If the query does have one or more clauses, search through them digging down into
                ' the parameters looking for the source table name associated with the parameters.
                For Each clause As QueryClause In query.Clauses

                    ' Check if this clause does not have any parameters
                    ' If the clause does not have any parmeters, then it means that that the criteria for the query
                    ' was deleted, but the clause still exists in the database and is connected to the clause.
                    ' If this is the case we still want to see the query regardless
                    ' of the selected case type, because it does not have a criteria that is associated with a case type.
                    ' This can lead to a problem if one of the queries output fields is associated with an incompatible
                    ' case type when a criteria is eventaully added. This possiblity is check for when the user attempts
                    ' to run the query.
                    If (clause.Parameters.Count = 0) Then
                        selectedQueries.Add(query)
                        Exit For 'clause for each
                    End If

                    For Each param As QueryParameter In clause.Parameters

                        ' Check if the source table name matches the selected case type
                        If String.Compare(param.Source.TableName, ddlCaseType.SelectedValue) = 0 Then
                            selectedQueries.Add(query)
                            foundQuery = True
                            Exit For ' param for each
                        End If

                    Next

                    If (foundQuery = True) Then
                        Exit For ' clause for each
                    End If
                Next

                foundQuery = False
            Next

            ' Sort the selected queries by title
            selectedQueries = selectedQueries.OrderBy(Function(x) x.Title).ToList()

            ddlSharedReports.Items.Clear()

            ' Add the queries to the DDL and mark the queries that the logged in user owns.
            If (selectedQueries.Count > 0) Then
                For Each q As UserQuery In selectedQueries
                    If (q.User.Id = SESSION_USER_ID) Then
                        ddlSharedReports.Items.Add(New ListItem(Server.HtmlDecode(q.Title) & " [Owner]", q.Id.ToString()))
                    Else
                        ddlSharedReports.Items.Add(New ListItem(Server.HtmlDecode(q.Title), q.Id.ToString()))
                    End If
                Next
            End If

            ddlSharedReports.Items.Insert(0, New ListItem("-- Select Report --", "0"))

        End Sub

        Private Sub InitSourceValue(ByVal id As Integer)

            If (id = 0) Then
                'no selection
                SourcePanel.Visible = False
                Exit Sub
            End If

            SourcePanel.Visible = True
            AddParamButton.Enabled = True
            HideCriteriaValuePanels()

            ' Determine which type of source was selected...
            If (ddlCaseType.SelectedItem.Value.Equals(PH_SOURCE) AndAlso rblCriteriaType.SelectedValue.Equals("2")) Then
                DataNumber.Visible = True
            Else
                'get the details for this source
                Dim source As InputSource = QueryDao.GetSourceById(id)

                'configure our options based on the data type of the selected field
                Select Case source.DataType

                    Case "C" 'choice
                        DataChoice.Visible = True
                        DataChoiceSelect.DataSource = QueryDao.GetInputChoices(source)
                        DataChoiceSelect.DataTextField = source.LookupText
                        DataChoiceSelect.DataValueField = source.LookupValue
                        DataChoiceSelect.DataBind()

                        ModifyChoiceSelectItems(source, DataChoiceSelect)
                    Case "N" 'number
                        DataNumber.Visible = True
                    Case "B" 'boolean
                        DataBool.Visible = True
                    Case "T" 'text
                        DataText.Visible = True
                    Case "D" 'dates
                        DataDates.Visible = True
                End Select
            End If
        End Sub

        Private Sub InitUserQueries(ByRef queries As List(Of UserQuery))

            ' Load saved queries that are associated with the selected case type or queries
            ' that do not have any clauses.
            Dim selectedQueries As New List(Of UserQuery)
            Dim foundQuery As Boolean = False

            For Each query As UserQuery In queries

                ' Check if this query is temporary
                If (query.Transient = True) Then
                    Continue For
                End If

                ' Check if this query is a shared query.
                ' If so then do not include it.
                If (query.Shared = True) Then
                    Continue For
                End If

                ' Check if this query does not have any clauses
                ' If the query does not have any clauses, then it has not been associated with
                ' any particular case type yet.
                If (query.Clauses.Count = 0) Then
                    selectedQueries.Add(query)
                    Continue For
                End If

                ' If the query does have one or more clauses, search through them digging down into
                ' the parameters looking for the source table name associated with the parameters.
                For Each clause As QueryClause In query.Clauses

                    ' Check if this clause does not have any parameters
                    ' If the clause does not have any parmeters, then it means that that the criteria for the query
                    ' was deleted, but the clause still exists in the database and is connected to the query.
                    ' If this is the case we still want to see the query regardless
                    ' of the selected case type, because it does not have a criteria that is associated with a case type.
                    ' This can lead to a problem if one of the queries output fields is associated with an incompatible
                    ' case type when a criteria is eventaully added. This possiblity is check for when the user attempts
                    ' to run the query.
                    If (clause.Parameters.Count = 0) Then
                        selectedQueries.Add(query)
                        Exit For 'clause for each
                    End If

                    For Each param As QueryParameter In clause.Parameters

                        ' Check if the source table name matches the selected case type
                        If String.Compare(param.Source.TableName, ddlCaseType.SelectedValue) = 0 Then
                            selectedQueries.Add(query)
                            foundQuery = True
                            Exit For ' param for each
                        End If

                    Next

                    If (foundQuery = True) Then
                        Exit For ' clause for each
                    End If
                Next

                foundQuery = False
            Next

            ' Sort the selected queries by title
            selectedQueries = selectedQueries.OrderBy(Function(x) x.Title).ToList()

            ' end new code

            ' Add the reports to the reports dropdown list
            MyQuerySelect.Items.Clear()

            If (selectedQueries.Count > 0) Then
                For Each q As UserQuery In selectedQueries
                    MyQuerySelect.Items.Add(New ListItem(Server.HtmlDecode(q.Title), q.Id.ToString()))
                Next
            End If

            MyQuerySelect.Items.Insert(0, New ListItem("-- Select Report --", "0"))

        End Sub

        Private Sub InitWorkflowGridViewOperators(ByRef param As QueryParameter, ByRef ddl As DropDownList)
            Select Case param.Source.DataType
                Case "B", "C" 'boolean or choice
                    ddl.Items.Add(New ListItem("Is", QueryOperator.EQUALS))
                    ddl.Items.Add(New ListItem("Is Not", QueryOperator.NOT_EQUAL))

                Case "T" 'text
                    If (IsUnitNameSource(param.Source.DisplayName) = True) Then
                        ddl.Items.Add(New ListItem("Is", QueryOperator.EQUALS))
                    End If
                    ddl.Items.Add(New ListItem("Contains", QueryOperator.LIKE))

                Case "D", "N" 'dates or number
                    ddl.Items.Add(New ListItem("Equals", QueryOperator.EQUALS))
                    ddl.Items.Add(New ListItem("Less Than", QueryOperator.LESS_THAN))
                    ddl.Items.Add(New ListItem("Greater Than", QueryOperator.GREATER_THAN))
                    ddl.Items.Add(New ListItem("Between", QueryOperator.BETWEEN))
            End Select
        End Sub

        Private Sub InitWorkflowGridViewValues(ByRef param As QueryParameter, ByRef e As System.Web.UI.WebControls.GridViewRowEventArgs)
            Dim hasEndValue As Boolean = False

            If (Not String.IsNullOrEmpty(param.EndValue)) Then
                hasEndValue = True
            End If

            ' Get all of the edit controls for the param values
            Dim ddlChoice As DropDownList = e.Row.FindControl("ddlGridChoice")
            Dim txtText As TextBox = e.Row.FindControl("txtGridText")
            Dim cbGridSubUnit As CheckBox = e.Row.FindControl("cbGridSubUnit")
            Dim lblGridSubUnitInfo As Label = e.Row.FindControl("lblGridSubUnitInfo")
            Dim ddlBool As DropDownList = e.Row.FindControl("ddlGridBool")
            Dim txtDateStart As TextBox = e.Row.FindControl("txtGridDateStart")
            Dim txtDateEnd As TextBox = e.Row.FindControl("txtGridDateEnd")
            Dim lblDateAnd As Label = e.Row.FindControl("lblGridDateAnd")
            Dim txtRangeStart As TextBox = e.Row.FindControl("txtGridRangeStart")
            Dim ddlRangeStartSelect As DropDownList = e.Row.FindControl("ddlGridRangeStartSelect")
            Dim lblRangeAnd As Label = e.Row.FindControl("lblGridRangeAnd")
            Dim txtRangeEnd As TextBox = e.Row.FindControl("txtGridRangeEnd")
            Dim ddlRangeEndSelect As DropDownList = e.Row.FindControl("ddlGridRangeEndSelect")
            Dim txtNumberStart As TextBox = e.Row.FindControl("txtGridNumberStart")
            Dim txtNumberEnd As TextBox = e.Row.FindControl("txtGridNumberEnd")
            Dim lblNumAnd As Label = e.Row.FindControl("lblGridNumAnd")
            Dim txtGridFindUnit As HtmlInputButton = e.Row.FindControl("btnGridFindUnit")

            ' Reset visibilities of edit controls
            ddlChoice.Visible = False
            txtText.Visible = False
            cbGridSubUnit.Visible = False
            lblGridSubUnitInfo.Visible = False
            ddlBool.Visible = False
            txtDateStart.Visible = False
            txtDateEnd.Visible = True
            lblDateAnd.Visible = True
            txtRangeStart.Visible = False
            ddlRangeStartSelect.Visible = False
            lblRangeAnd.Visible = True
            txtRangeEnd.Visible = True
            ddlRangeEndSelect.Visible = True
            txtNumberStart.Visible = False
            txtNumberEnd.Visible = True
            lblNumAnd.Visible = True
            txtGridFindUnit.Visible = False

            'configure our options based on the data type of the selected field
            Select Case param.Source.DataType

                Case "C" 'choice
                    ddlChoice.Visible = True
                    ddlChoice.DataSource = QueryDao.GetInputChoices(param.Source)
                    ddlChoice.DataTextField = param.Source.LookupText
                    ddlChoice.DataValueField = param.Source.LookupValue
                    ddlChoice.DataBind()

                    ModifyChoiceSelectItems(param.Source, ddlChoice)
                    ddlChoice.SelectedValue = param.StartValue

                Case "N" 'number
                    SetInputFormatRestriction(Page, txtNumberStart, FormatRestriction.Numeric)
                    txtNumberStart.Visible = True
                    txtNumberStart.Text = Server.HtmlDecode(param.StartValue)

                    If (hasEndValue = True) Then
                        SetInputFormatRestriction(Page, txtNumberEnd, FormatRestriction.Numeric)
                        lblNumAnd.Visible = True
                        txtNumberEnd.Visible = True
                        txtNumberEnd.Text = Server.HtmlDecode(param.EndValue)
                    End If

                Case "B" 'boolean
                    ddlBool.Visible = True
                    ddlBool.SelectedValue = param.StartValue

                Case "T" 'text
                    SetInputFormatRestriction(Page, txtText, FormatRestriction.AlphaNumeric, PERMITTED_SPECIAL_CHAR_INPUT)
                    txtText.Visible = True
                    txtText.Text = Server.HtmlDecode(param.StartValue)

                    ' In the case of a Unit Name parameter then perform additional steps
                    If (IsUnitNameSource(param.Source.DisplayName)) Then
                        txtGridFindUnit.Visible = True
                        cbGridSubUnit.Visible = True
                        lblGridSubUnitInfo.Visible = True
                        txtText.Width = 175

                        ' Check if the parameter has the "Include Subordinate Unit" condition attached to it
                        If (Not String.IsNullOrEmpty(param.EndValue) AndAlso Integer.Parse(param.EndValue) > -1) Then
                            cbGridSubUnit.Checked = True
                            txtText.Text = txtText.Text + "(" + LookupService.GetUnitPasByIdAndName(Integer.Parse(Server.HtmlDecode(param.EndValue)), txtText.Text) + ")"
                            txtText.ReadOnly = True
                        End If
                    End If

                Case "D" 'dates
                    If (param.StartValue.Contains("D") Or param.StartValue.Contains("W") Or param.StartValue.Contains("M") Or param.StartValue.Contains("Y")) Then
                        SetInputFormatRestriction(Page, txtRangeStart, FormatRestriction.Numeric)
                        txtRangeStart.Visible = True
                        ddlRangeStartSelect.Visible = True
                        txtRangeStart.Text = Server.HtmlDecode(param.StartValue).Substring(0, Server.HtmlDecode(param.StartValue).Length - 1)
                        ddlRangeStartSelect.SelectedValue = Server.HtmlDecode(param.StartValue).Substring(Server.HtmlDecode(param.StartValue).Length - 1)

                        If (hasEndValue = True) Then
                            SetInputFormatRestriction(Page, txtRangeEnd, FormatRestriction.Numeric)
                            lblRangeAnd.Visible = True
                            txtRangeEnd.Visible = True
                            ddlRangeEndSelect.Visible = True
                            txtRangeEnd.Text = Server.HtmlDecode(param.EndValue).Substring(0, Server.HtmlDecode(param.EndValue).Length - 1)
                            ddlRangeEndSelect.SelectedValue = Server.HtmlDecode(param.EndValue).Substring(Server.HtmlDecode(param.EndValue).Length - 1)
                        End If
                    Else
                        SetInputFormatRestriction(Page, txtDateStart, FormatRestriction.Numeric, "/")
                        txtDateStart.Visible = True
                        txtDateStart.Text = Server.HtmlDecode(param.StartValue)

                        If (hasEndValue = True) Then
                            SetInputFormatRestriction(Page, txtDateEnd, FormatRestriction.Numeric, "/")
                            lblDateAnd.Visible = True
                            txtDateEnd.Visible = True
                            txtDateEnd.Text = Server.HtmlDecode(param.EndValue)
                        End If
                    End If

            End Select
        End Sub

        Private Function IsPHFormCaseTypeSelected() As Boolean
            Return ddlCaseType.SelectedItem.Value.Equals(PH_SOURCE)
        End Function

        Private Function IsUnitNameSource(ByVal sourceName As String) As Boolean
            Return AdHocQueryHelpers.IsUnitNameSource(sourceName)
        End Function

        Private Sub LoadUserQuery()
            UpdateExecuteOrderTextbox()
            UpdateCurrentWorkflowQueryGrid()
            UpdateCurrentPHFormFieldQueryGrid()
            UpdateOutputFields()
        End Sub

        Private Sub ModifyChoiceSelectItems(ByRef source As InputSource, ByRef ddl As DropDownList)

            ' Remove the MMSO and MP items. MMSO is an inactive workflow and MP is combined with the BMT workflow.
            If source.DisplayName.Equals("Status") Then
                Dim items As New List(Of ListItem)

                ' Find all of the MMSO and MP items
                For Each item As ListItem In ddl.Items
                    If item.Text.Contains("MMSO") Or item.Text.Contains("MP") Then
                        items.Add(item)
                    End If
                Next

                ' Remove the MMSO and MP items
                For Each item As ListItem In items
                    ddl.Items.Remove(item)
                Next
            End If

        End Sub

        Private Sub PopulatePHDropdownLists(ByVal sectionDDL As DropDownList, ByVal fieldDDL As DropDownList, ByVal fieldTypeDDL As DropDownList)
            ' PH Sections
            sectionDDL.DataSource = PHDao.GetAllSections()
            sectionDDL.DataValueField = "Id"
            sectionDDL.DataTextField = "Name"
            sectionDDL.DataBind()

            InsertDropDownListZeroValue(sectionDDL, "--- Select Section ---")

            ' PH Fields
            fieldDDL.DataSource = PHDao.GetAllFields()
            fieldDDL.DataValueField = "Id"
            fieldDDL.DataTextField = "Name"
            fieldDDL.DataBind()

            InsertDropDownListZeroValue(fieldDDL, "--- Select Field Name ---")

            ' PH Field Types
            fieldTypeDDL.DataSource = PHDao.GetAllFieldTypes().Where(Function(x) x.DataTypeId = DataType.DT_Integer Or x.DataTypeId = DataType.DT_Numeric).ToList()
            fieldTypeDDL.DataValueField = "Id"
            fieldTypeDDL.DataTextField = "Name"
            fieldTypeDDL.DataBind()

            InsertDropDownListZeroValue(fieldTypeDDL, "--- Select Field Type ---")
        End Sub

        Private Sub SelectLastModifiedQueryToLoad(ByRef queries As List(Of UserQuery))
            ' Get all save queries created by the logged in user
            'Dim queries = QueryDao.GetUserQueries(SESSION_USER_ID)

            ' Load saved queries that are associated with the selected case type or queries
            ' that do not have any clauses.
            Dim selectedQueries As New List(Of UserQuery)
            Dim foundQuery As Boolean = False

            For Each query As UserQuery In queries

                ' Check if this query is temporary
                If (query.Transient = True) Then
                    Continue For
                End If

                ' Check if this query does not have any clauses
                ' If the query does not have any clauses, then it has not been associated with
                ' any particular case type yet.
                If (query.Clauses.Count = 0) Then
                    selectedQueries.Add(query)
                    Continue For
                End If

                ' If the query does have one or more clauses, search through them digging down into
                ' the parameters looking for the source table name associated with the parameters.
                For Each clause As QueryClause In query.Clauses

                    ' Check if this clause does not have any parameters
                    ' If the clause does not have any parmeters, then it means that that the criteria for the query
                    ' was deleted, but the clause still exists in the database and is connected to the query.
                    ' If this is the case we still want to see the query regardless
                    ' of the selected case type, because it does not have a criteria that is associated with a case type.
                    ' This can lead to a problem if one of the queries output fields is associated with an incompatible
                    ' case type when a criteria is eventaully added. This possiblity is check for when the user attempts
                    ' to run the query.
                    If (clause.Parameters.Count = 0) Then
                        selectedQueries.Add(query)
                        Exit For 'clause for each
                    End If

                    For Each param As QueryParameter In clause.Parameters

                        ' Check if the source table name matches the selected case type
                        If String.Compare(param.Source.TableName, ddlCaseType.SelectedValue) = 0 Then
                            selectedQueries.Add(query)
                            foundQuery = True
                            Exit For ' param for each
                        End If

                    Next

                    If (foundQuery = True) Then
                        Exit For ' clause for each
                    End If
                Next

                foundQuery = False
            Next

            ' Now load up the current query (last modified query)
            If (selectedQueries.Count = 0 Or Not IsPostBack) Then
                'this user doesn't have any queries, so start a new one
                'this will also set the editId to the new one
                ClearCurrentTransient()
            Else

                Dim current As UserQuery = (From q In selectedQueries
                                            Select q
                                            Order By q.ModifiedDate Descending
                                            Take 1).SingleOrDefault()

                EditId = current.Id

            End If
        End Sub

        Private Sub TurnOutputFieldArrowsOnOff(ByVal turnOn As Boolean)
            If (turnOn) Then
                imgAddOutputField.Disabled = False
                imgAddOutputField.Visible = True
                imgRemoveOutputField.Disabled = False
                imgRemoveOutputField.Visible = True
                imgAddToSort.Disabled = False
                imgAddToSort.Visible = True
                imgRemoveFromSort.Disabled = False
                imgRemoveFromSort.Visible = True
                imgMoveItemUp.Disabled = False
                imgMoveItemUp.Visible = True
                imgMoveItemDown.Disabled = False
                imgMoveItemDown.Visible = True
                imgFlipSort.Disabled = False
                imgFlipSort.Visible = True
            Else
                imgAddOutputField.Disabled = True
                imgAddOutputField.Visible = False
                imgRemoveOutputField.Disabled = True
                imgRemoveOutputField.Visible = False
                imgAddToSort.Disabled = True
                imgAddToSort.Visible = False
                imgRemoveFromSort.Disabled = True
                imgRemoveFromSort.Visible = False
                imgMoveItemUp.Disabled = True
                imgMoveItemUp.Visible = False
                imgMoveItemDown.Disabled = True
                imgMoveItemDown.Visible = False
                imgFlipSort.Disabled = True
                imgFlipSort.Visible = False
            End If
        End Sub

        Private Sub TurnReportEditingControlsOnOff(ByVal turnOn As Boolean)
            If (turnOn) Then
                SaveQueryButton.Enabled = True
                ClientSaveButton.Enabled = True
                SourceSelect.Enabled = True
                TurnOutputFieldArrowsOnOff(True)
            Else
                SaveQueryButton.Enabled = False
                ClientSaveButton.Enabled = False
                SourceSelect.Enabled = False
                TurnOutputFieldArrowsOnOff(False)
            End If
        End Sub

        Private Sub UpdateCurrentPHFormFieldQueryGrid()
            If (Not IsPHFormCaseTypeSelected()) Then
                pnlPHParams.Visible = False
                gdvPHQuery.Visible = False
                lblNoPHParams.Visible = False
                lblWorkflowParams.Visible = False
                lblPHParams.Visible = False
                Exit Sub
            End If

            If (EditId = 0) Then
                Exit Sub
            End If

            Dim phQuery As PHUserQuery = PHQueryDao.GetByBaseQueryId(EditId)

            If (phQuery Is Nothing) Then
                Exit Sub
            End If

            ' If we don't have any clauses or parameters then don't display the parameter gridview...
            If (phQuery.Clauses Is Nothing OrElse phQuery.Clauses.Count = 0 OrElse phQuery.Clauses(0).Parameters Is Nothing OrElse phQuery.Clauses(0).Parameters.Count = 0) Then
                pnlPHParams.Visible = True
                gdvPHQuery.Visible = False
                lblNoPHParams.Visible = True
                lblPHParams.Visible = False
                Exit Sub
            End If

            pnlPHParams.Visible = True
            gdvPHQuery.Visible = True
            lblNoPHParams.Visible = False
            lblPHParams.Visible = True

            ' If the base user query is shared and if the user does not own it then disable all the controls that allow the user to edit the report...
            If (phQuery.UserQuery.Shared AndAlso phQuery.UserQuery.User.Id <> SESSION_USER_ID) Then
                TurnReportEditingControlsOnOff(False)
            Else
                TurnReportEditingControlsOnOff(True)
            End If

            ' Databind parameters to the appropriate gridview...Default to the first clause for now
            gdvPHQuery.DataSource = From p In phQuery.Clauses(0).Parameters
                                    Select p.Id, p.WhereType, p.Operator, p.ExecuteOrder, p.StartDisplay, p.StartValue, p.EndDisplay, p.EndValue
                                    Order By ExecuteOrder

            gdvPHQuery.DataBind()
        End Sub

        Private Sub UpdateCurrentWorkflowQueryGrid()

            If (EditId = 0) Then
                Exit Sub
            End If

            Dim query As UserQuery = QueryDao.GetById(EditId)

            If (query Is Nothing) Then
                Exit Sub
            End If

            ReportTitleLabel.Text = query.Title
            SaveTitlebox.Text = Server.HtmlDecode(query.Title)
            chkSaveShared.Checked = query.Shared

            If (query.Clauses Is Nothing) OrElse (query.Clauses.Count = 0) Then
                'if we don't have a clause, we can't have an params
                QueryGrid.Visible = False
                NoParamLabel.Visible = True
                QueryButton.Enabled = False
                SaveQueryButton.Enabled = False
                ClientSaveButton.Enabled = False
                lblWorkflowParams.Visible = False
                Exit Sub
            End If

            If (query.Clauses(0).Parameters Is Nothing) OrElse (query.Clauses(0).Parameters.Count = 0) Then
                'no parameters, we've got nothing to show here
                QueryGrid.Visible = False
                NoParamLabel.Visible = True
                QueryButton.Enabled = False
                SaveQueryButton.Enabled = False
                ClientSaveButton.Enabled = False
                lblWorkflowParams.Visible = False
                Exit Sub
            End If

            QueryGrid.Visible = True
            NoParamLabel.Visible = False
            QueryButton.Enabled = True

            If (IsPHFormCaseTypeSelected()) Then
                lblWorkflowParams.Visible = True
            End If

            ' If the query is shared and if the user does not own it then disable all the controls that allow the user to edit the report...
            If (query.Shared AndAlso query.User.Id <> SESSION_USER_ID) Then
                TurnReportEditingControlsOnOff(False)
            Else
                TurnReportEditingControlsOnOff(True)
            End If

            ' Databind parameters to the appropriate gridview...Default to the first clause for now
            QueryGrid.DataSource = From p In query.Clauses(0).Parameters
                                   Select p.Id, p.Source.DisplayName, p.WhereType, p.StartDisplay,
                                   p.StartValue, p.EndDisplay, p.EndValue, p.Operator, p.ExecuteOrder
                                   Order By ExecuteOrder

            QueryGrid.DataBind()

        End Sub

        Private Sub UpdateExecuteOrderTextbox()
            If (EditId = 0) Then
                txtExecuteOrder.Text = 1
                Exit Sub
            End If
            Dim executeOrder As Integer = 1

            ' Determine if we are updated the execute order for hte workflow parameters or the PH form parameters..
            If (IsPHFormCaseTypeSelected() AndAlso rblCriteriaType.SelectedValue.Equals("2")) Then
                Dim phQuery As PHUserQuery = PHQueryDao.GetByBaseQueryId(EditId)

                If (phQuery Is Nothing) Then
                    txtExecuteOrder.Text = 1
                    Exit Sub
                End If

                ' Check to see if the clause doesn't exist or if any parameters exist.
                If (phQuery.Clauses.Count = 0 OrElse phQuery.Clauses(0).Parameters.Count = 0) Then
                    txtExecuteOrder.Text = 1
                    Exit Sub
                End If

                ' Order the parameters by their execute order then get the execute order of the last parameter in the list
                executeOrder = phQuery.Clauses(0).Parameters.OrderBy(Function(x) x.ExecuteOrder).ToList()(phQuery.Clauses(0).Parameters.Count - 1).ExecuteOrder
            Else
                Dim query As UserQuery = QueryDao.GetById(EditId)

                If (query Is Nothing) Then
                    txtExecuteOrder.Text = 1
                    Exit Sub
                End If

                ' Check to see if the clause doesn't exist or if any parameters exist.
                If (query.Clauses.Count = 0 OrElse query.Clauses(0).Parameters.Count = 0) Then
                    txtExecuteOrder.Text = 1
                    Exit Sub
                End If

                ' Order the parameters by their execute order then get the execute order of the last parameter in the list
                executeOrder = query.Clauses(0).Parameters.OrderBy(Function(x) x.ExecuteOrder).ToList()(query.Clauses(0).Parameters.Count - 1).ExecuteOrder
            End If

            ' Increment the found execute order by one and set the textbox to the resulting value
            txtExecuteOrder.Text = (Integer.Parse(Server.HtmlDecode(executeOrder)) + 1)
        End Sub

        Private Sub UpdatePHSortFieldControls(ByVal sortField As String)
            ' Set sort column radio button...
            For Each item As ListItem In rblPHFormSortByField.Items
                If (Not String.IsNullOrEmpty(sortField) AndAlso sortField.Contains(item.Text)) Then
                    item.Selected = True
                Else
                    item.Selected = False
                End If
            Next

            ' Set sort order radio button...
            For Each item As ListItem In rblPHFormSortByOrder.Items
                If (Not String.IsNullOrEmpty(sortField) AndAlso sortField.Contains(item.Text)) Then
                    item.Selected = True
                Else
                    item.Selected = False
                End If
            Next
        End Sub

        Private Function ValidateParamInput() As Boolean

            'do we have a valid selection?
            If (SourceSelect.SelectedValue = "0") Then
                Return False
            End If

            ' Validate execute order
            If (String.IsNullOrEmpty(txtExecuteOrder.Text) OrElse Integer.Parse(Server.HtmlEncode(txtExecuteOrder.Text)) < 1) Then
                txtExecuteOrder.CssClass = "fieldRequired"
                Return False
            Else
                txtExecuteOrder.CssClass = ""
            End If

            'check the panels, the visible one will tell us what type of data we're dealing with

            'Text
            If (DataText.Visible) Then
                If (DataChoiceText.Text.Trim.Length = 0) Then
                    DataChoiceText.CssClass = DATACHOICE_TEXTBOX_CSSCLASS + " fieldRequired"
                    Return False
                End If

                DataChoiceText.CssClass = DATACHOICE_TEXTBOX_CSSCLASS

                ' Validate the subordinate unit checkbox conditions
                If (cbSubUnit.Visible = True AndAlso cbSubUnit.Checked = True) Then
                    If (Len(SrcNameHdn.Value) = 0) Then
                        DataChoiceText.CssClass = DATACHOICE_TEXTBOX_CSSCLASS + " fieldRequired"
                        Return False
                    End If
                End If

                Return True
            End If

            'dates
            If (DataDates.Visible) Then

                Dim passed As Boolean = True

                If (DateExact.Checked) Then

                    RangeStart.CssClass = ""
                    RangeEnd.CssClass = ""

                    'check the start date
                    If (DateStart.Text.Trim.Length = 0) Then
                        DateStart.CssClass = "fieldRequired datePicker"
                        passed = False
                    Else
                        Dim start As Date
                        Date.TryParse(Server.HtmlEncode(DateStart.Text), start)
                        If (start.Ticks = 0) Then
                            DateStart.CssClass = "fieldRequired datePicker"
                            passed = False
                        End If
                        'start date is good
                        DateStart.CssClass = "datePicker"
                    End If

                    If (OperatorDate.SelectedValue = QueryOperator.BETWEEN) Then
                        'the end date is also required
                        If (DateStop.Text.Trim.Length = 0) Then
                            DateStop.CssClass = "fieldRequired datePicker"
                            passed = False
                        Else
                            Dim start As Date
                            Date.TryParse(Server.HtmlEncode(DateStop.Text), start)
                            If (start.Ticks = 0) Then
                                DateStop.CssClass = "fieldRequired datePicker"
                                passed = False
                            End If
                            'start date is good
                            DateStop.CssClass = "datePicker"
                        End If
                    Else
                        DateStop.CssClass = "datePicker"
                    End If

                    'end exact
                Else
                    DateStart.CssClass = "datePicker"
                    DateStop.CssClass = "datePicker"

                    'check the start date
                    If (RangeStart.Text.Trim.Length = 0) Then
                        RangeStart.CssClass = "fieldRequired"
                        passed = False
                    Else
                        Dim start As Integer = 0
                        Integer.TryParse(Server.HtmlEncode(RangeStart.Text), start)
                        If (start = 0) Then
                            RangeStart.CssClass = "fieldRequired"
                            passed = False
                        End If
                        'start date is good
                        RangeStart.CssClass = ""
                    End If

                    If (OperatorDate.SelectedValue = QueryOperator.BETWEEN) Then
                        'the end date is also required
                        If (RangeEnd.Text.Trim.Length = 0) Then
                            RangeEnd.CssClass = "fieldRequired"
                            passed = False
                        Else
                            Dim start As Integer = 0
                            Integer.TryParse(Server.HtmlEncode(RangeEnd.Text), start)
                            If (start = 0) Then
                                DateStop.CssClass = "fieldRequired"
                                passed = False
                            End If
                            'start date is good
                            RangeEnd.CssClass = ""
                        End If
                    Else
                        RangeEnd.CssClass = ""
                    End If

                    'end span
                End If

                Return passed

            End If

            'number
            If (DataNumber.Visible) Then

                Dim passed As Boolean = True

                'check the start number
                If (NumberStart.Text.Trim.Length = 0) Then
                    NumberStart.CssClass = "fieldRequired"
                    passed = False
                Else
                    Dim start As Integer
                    Try
                        start = Integer.Parse(Server.HtmlEncode(NumberStart.Text.Trim))
                        'start number is good
                        NumberStart.CssClass = ""
                    Catch ex As Exception
                        NumberStart.CssClass = "fieldRequired"
                        passed = False
                    End Try
                End If

                If (OperatorNumber.SelectedValue = QueryOperator.BETWEEN) Then
                    'the end number is also required
                    If (NumberStop.Text.Trim.Length = 0) Then
                        NumberStop.CssClass = "fieldRequired"
                        passed = False
                    Else
                        Dim start As Integer
                        Try
                            start = Integer.Parse(Server.HtmlEncode(NumberStop.Text.Trim))
                            'start number is good
                            NumberStop.CssClass = ""
                        Catch ex As Exception
                            NumberStop.CssClass = "fieldRequired"
                            passed = False
                        End Try
                    End If
                Else
                    NumberStop.CssClass = ""
                End If

                Return passed

            End If

            Return True

        End Function

        Private Function ValidatePHFormGridViewParamInput(ByVal editIndex As Integer, ByRef param As PHQueryParameter) As Boolean
            Dim row As GridViewRow = gdvPHQuery.Rows(editIndex)
            Dim gridExecuteOrderTxt As TextBox = row.FindControl("txtGridExecuteOrder")
            Dim gridOperatorDDL As DropDownList = row.FindControl("ddlGridOperator")

            ' Validate execute order
            If (String.IsNullOrEmpty(gridExecuteOrderTxt.Text) OrElse Integer.Parse(gridExecuteOrderTxt.Text) < 1) Then
                gridExecuteOrderTxt.CssClass = "fieldRequired"
                Return False
            Else
                gridExecuteOrderTxt.CssClass = ""
            End If

            ' Get the controls needed to determine what type of parameter is being validated
            Dim txtNumberStart As TextBox = row.FindControl("txtGridNumberStart")

            ' Number
            If (txtNumberStart.Visible) Then

                ' Get controls specific to a Number parameter that also need to be validated
                Dim txtNumberEnd As TextBox = row.FindControl("txtGridNumberEnd")
                Dim passed As Boolean = True

                'check the start number
                If (txtNumberStart.Text.Trim.Length = 0) Then
                    txtNumberStart.CssClass = "fieldRequired"
                    passed = False
                Else
                    Dim start As Integer
                    Try
                        start = Integer.Parse(Server.HtmlEncode(txtNumberStart.Text.Trim))
                        'start number is good
                        txtNumberStart.CssClass = ""
                    Catch ex As Exception
                        txtNumberStart.CssClass = "fieldRequired"
                        passed = False
                    End Try
                End If

                If (gridOperatorDDL.SelectedValue = QueryOperator.BETWEEN And txtNumberEnd.Visible = True) Then
                    'the end number is also required
                    If (txtNumberEnd.Text.Trim.Length = 0) Then
                        txtNumberEnd.CssClass = "fieldRequired"
                        passed = False
                    Else
                        Dim start As Integer
                        Try
                            start = Integer.Parse(Server.HtmlEncode(txtNumberEnd.Text.Trim))
                            'end number is good
                            txtNumberEnd.CssClass = ""
                        Catch ex As Exception
                            txtNumberEnd.CssClass = "fieldRequired"
                            passed = False
                        End Try
                    End If
                Else
                    txtNumberEnd.CssClass = ""
                End If

                Return passed
            End If

            Return True
        End Function

        Private Function ValidateWorkflowGridViewParamInput(ByVal editIndex As Integer, ByRef param As QueryParameter) As Boolean
            Dim row As GridViewRow = QueryGrid.Rows(editIndex)
            Dim gridExecuteOrderTxt As TextBox = row.FindControl("txtGridExecuteOrder")
            Dim gridOperatorDDL As DropDownList = row.FindControl("ddlGridOperator")

            ' Validate execute order
            If (String.IsNullOrEmpty(gridExecuteOrderTxt.Text) OrElse Integer.Parse(gridExecuteOrderTxt.Text) < 1) Then
                gridExecuteOrderTxt.CssClass = "fieldRequired"
                Return False
            Else
                gridExecuteOrderTxt.CssClass = ""
            End If

            ' Get the controls needed to determine what type of parameter is being validated
            Dim txtText As TextBox = row.FindControl("txtGridText")
            Dim txtDateStart As TextBox = row.FindControl("txtGridDateStart")
            Dim txtRangeStart As TextBox = row.FindControl("txtGridRangeStart")
            Dim txtNumberStart As TextBox = row.FindControl("txtGridNumberStart")

            ' Text
            If (txtText.Visible) Then

                ' Get controls specific to a Text parameter that also need to be validated
                Dim cbGridSubUnit As CheckBox = row.FindControl("cbGridSubUnit")
                Dim hdnSrcName As HtmlInputHidden = row.FindControl("hdnGridUnitNameClient")

                ' Validate the checkbox
                If (txtText.Text.Trim.Length = 0) Then
                    txtText.CssClass = GRID_EDIT_TEXTBOX_CSSCLASS + " fieldRequired"
                    Return False
                End If

                txtText.CssClass = GRID_EDIT_TEXTBOX_CSSCLASS

                ' Validate the subordinate unit checkbox conditions
                If (cbGridSubUnit.Visible = True AndAlso cbGridSubUnit.Checked = True) Then
                    If (Len(hdnSrcName.Value) = 0 AndAlso String.IsNullOrEmpty(param.EndValue) = True) Then
                        txtText.CssClass = GRID_EDIT_TEXTBOX_CSSCLASS + " fieldRequired"
                        Return False
                    End If

                    If (gridOperatorDDL.SelectedValue <> QueryOperator.EQUALS) Then
                        gridOperatorDDL.CssClass = GRID_EDIT_OPERATOR_CSSCLASS + " fieldRequired"
                        Return False
                    End If

                    gridOperatorDDL.CssClass = GRID_EDIT_OPERATOR_CSSCLASS
                End If

                Return True

            End If

            ' Dates
            If (txtDateStart.Visible = True Or txtRangeStart.Visible = True) Then

                ' Get controls specific to a Date parameter that also need to be validated
                Dim txtDateEnd As TextBox = row.FindControl("txtGridDateEnd")
                Dim txtRangeEnd As TextBox = row.FindControl("txtGridRangeEnd")
                Dim passed As Boolean = True

                If (txtDateStart.Visible) Then

                    txtRangeStart.CssClass = ""
                    txtRangeEnd.CssClass = ""

                    'check the start date
                    If (txtDateStart.Text.Trim.Length = 0) Then
                        txtDateStart.CssClass = "fieldRequired datePicker"
                        passed = False
                    Else
                        Dim start As Date
                        Date.TryParse(Server.HtmlEncode(txtDateStart.Text), start)
                        If (start.Ticks = 0) Then
                            txtDateStart.CssClass = "fieldRequired datePicker"
                            passed = False
                        End If
                        'start date is good
                        txtDateStart.CssClass = "datePicker"
                    End If

                    If (gridOperatorDDL.SelectedValue = QueryOperator.BETWEEN And txtDateEnd.Visible = True) Then
                        'the end date is also required
                        If (txtDateEnd.Text.Trim.Length = 0) Then
                            txtDateEnd.CssClass = "fieldRequired datePicker"
                            passed = False
                        Else
                            Dim start As Date
                            Date.TryParse(Server.HtmlEncode(txtDateEnd.Text), start)
                            If (start.Ticks = 0) Then
                                txtDateEnd.CssClass = "fieldRequired datePicker"
                                passed = False
                            End If
                            'end date is good
                            txtDateEnd.CssClass = "datePicker"
                        End If
                    Else
                        txtDateEnd.CssClass = "datePicker"
                    End If

                    'end exact
                Else
                    txtDateStart.CssClass = "datePicker"
                    txtDateEnd.CssClass = "datePicker"

                    'check the start date
                    If (txtRangeStart.Text.Trim.Length = 0) Then
                        txtRangeStart.CssClass = "fieldRequired"
                        passed = False
                    Else
                        Dim start As Integer = 0
                        Integer.TryParse(Server.HtmlEncode(txtRangeStart.Text), start)
                        If (start = 0) Then
                            txtRangeStart.CssClass = "fieldRequired"
                            passed = False
                        End If
                        'start date is good
                        txtRangeStart.CssClass = ""
                    End If

                    If (gridOperatorDDL.SelectedValue = QueryOperator.BETWEEN And txtRangeEnd.Visible = True) Then
                        'the end date is also required
                        If (txtRangeEnd.Text.Trim.Length = 0) Then
                            txtRangeEnd.CssClass = "fieldRequired"
                            passed = False
                        Else
                            Dim start As Integer = 0
                            Integer.TryParse(Server.HtmlEncode(txtRangeEnd.Text), start)
                            If (start = 0) Then
                                txtRangeEnd.CssClass = "fieldRequired"
                                passed = False
                            End If
                            'end date is good
                            txtRangeEnd.CssClass = ""
                        End If
                    Else
                        txtRangeEnd.CssClass = ""
                    End If

                    'end span
                End If

                Return passed

            End If

            ' Number
            If (txtNumberStart.Visible) Then

                ' Get controls specific to a Number parameter that also need to be validated
                Dim txtNumberEnd As TextBox = row.FindControl("txtGridNumberEnd")
                Dim passed As Boolean = True

                'check the start number
                If (txtNumberStart.Text.Trim.Length = 0) Then
                    txtNumberStart.CssClass = "fieldRequired"
                    passed = False
                Else
                    Dim start As Integer
                    Try
                        start = Integer.Parse(Server.HtmlEncode(txtNumberStart.Text.Trim))
                        'start number is good
                        txtNumberStart.CssClass = ""
                    Catch ex As Exception
                        txtNumberStart.CssClass = "fieldRequired"
                        passed = False
                    End Try
                End If

                If (gridOperatorDDL.SelectedValue = QueryOperator.BETWEEN And txtNumberEnd.Visible = True) Then
                    'the end number is also required
                    If (txtNumberEnd.Text.Trim.Length = 0) Then
                        txtNumberEnd.CssClass = "fieldRequired"
                        passed = False
                    Else
                        Dim start As Integer
                        Try
                            start = Integer.Parse(Server.HtmlEncode(txtNumberEnd.Text.Trim))
                            'end number is good
                            txtNumberEnd.CssClass = ""
                        Catch ex As Exception
                            txtNumberEnd.CssClass = "fieldRequired"
                            passed = False
                        End Try
                    End If
                Else
                    txtNumberEnd.CssClass = ""
                End If

                Return passed

            End If

            Return True
        End Function

        'Protected Sub rblReportType_SelectedIndexChanged(sender As Object, e As EventArgs) Handles rblReportType.SelectedIndexChanged
        '    rblCriteriaType.SelectedIndex = 0

        '    rblCriteriaType_SelectedIndexChanged(rblCriteriaType, New EventArgs())

        '    ' update the output UI
        '    If (rblReportType.SelectedValue.Equals("1")) Then       ' Totals
        '        pnlWorkflowOutputUI.Visible = False
        '        pnlPHFormTotalsOutputUI.Visible = True
        '        PopulatePHDropdownLists(ddlOutputPHSection, ddlOutputPHField, ddlOutputPHFieldType)
        '    ElseIf (rblReportType.SelectedValue.Equals("2")) Then   ' Cases
        '        pnlWorkflowOutputUI.Visible = True
        '        pnlPHFormTotalsOutputUI.Visible = False
        '    End If
        'End Sub
    End Class

End Namespace