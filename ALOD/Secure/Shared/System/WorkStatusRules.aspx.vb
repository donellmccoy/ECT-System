Imports ALOD.Core.Domain.Documents
Imports ALOD.Core.Domain.Workflow
Imports ALOD.Core.Interfaces.DAOInterfaces
Imports ALOD.Data
Imports ALODWebUtility.Common

Namespace Web.Sys

    Partial Class Secure_Shared_System_WorkStatusRules
        Inherits System.Web.UI.Page

#Region "Fields"

        Private _daoFactory As NHibernateDaoFactory
        Private _userGroupDao As IUserGroupDao

#End Region

#Region "Properties"

        Protected ReadOnly Property DaoFactory As NHibernateDaoFactory
            Get
                If (_daoFactory Is Nothing) Then
                    _daoFactory = New NHibernateDaoFactory()
                End If

                Return _daoFactory
            End Get
        End Property

        Protected ReadOnly Property UserGroupDao As IUserGroupDao
            Get
                If (_userGroupDao Is Nothing) Then
                    _userGroupDao = DaoFactory.GetUserGroupDao()
                End If

                Return _userGroupDao
            End Get
        End Property

#End Region

#Region "Load"

        Protected Sub BindRulesGrid()

            Dim wso As Int32 = CType(Request.QueryString("wso"), Int32)
            Dim ruleDao As WorkflowOptionRulesDao = New NHibernateDaoFactory().GetOptionRulesDao()
            Dim rules = From r In ruleDao.GetAll() Where r.OptionType.Id = wso Select wsr_id = r.Id, wso_id = r.OptionType.Id, r.RuleId, ruleData = r.RuleValue, r.RuleTypes.Name, Description = IIf(r.RuleTypes.ruleTypeId = 1, "Visibility", "Validation"), r.CheckAll
            ' Dim rulrt = From r In ctxLod.core_WorkStatus_Rules, rt In ctxLod.core_lkupRuleTypes Where r.core_lkupRule.ruleType = rt.Id And r.wso_id = wso Select r.wsr_id, r.wso_id, r.ruleId, r.ruleData, r.core_lkupRule.name, rt.Description, r.checkAll
            gvRules.DataSource = rules
            gvRules.DataBind()
        End Sub

        Protected Sub gvRules_RowCommand(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewCommandEventArgs) Handles gvRules.RowCommand

            If e.CommandName = "Delete" Then
                Dim wsrId As Integer = e.CommandArgument
                Dim ruleDao As WorkflowOptionRulesDao = New NHibernateDaoFactory().GetOptionRulesDao()
                Dim rule As WorkflowOptionRule = ruleDao.GetById(wsrId)
                ruleDao.Delete(rule)
                ruleDao.CommitChanges()
                '   ctxLod.core_rule_sp_DeleteOptionRule(wsrId)
                Response.Redirect(Request.RawUrl)

            End If

        End Sub

        Protected Sub InitControls()

            Dim wf As Short = CType(Request.QueryString("workflow"), Short)
            Dim stepId As Int32 = CType(Request.QueryString("step"), Integer)
            Dim compo As String = CType(Request.QueryString("compo"), String)

            Dim nhFactory As NHibernateDaoFactory = New NHibernateDaoFactory()
            Dim wsDao As WorkStatusDao = nhFactory.GetWorkStatusDao()
            Dim wfDao As WorkflowDao = nhFactory.GetWorkflowDao()

            Dim wfname As String = (From w In wfDao.GetAll() Where w.Id = wf Select w.Title).First
            Dim workstatus As String = (From w In wsDao.GetAll() Where w.Id = stepId Select w.StatusCodeType.Description).First

            lblCompo.Text = Server.HtmlEncode(GetCompoString(compo))
            lblOption.Text = Server.HtmlEncode(Request.QueryString("text"))
            lblWorkflow.Text = Server.HtmlEncode(wfname)
            lblStep.Text = Server.HtmlEncode(workstatus)

        End Sub

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

            If Not (Page.IsPostBack) Then
                InitControls()
                BindRulesGrid()
                BindRules(1)
                BindSteps()
            End If

        End Sub

        Protected Sub Page_PreRender(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.PreRender

            CheckForStatus()

        End Sub

#End Region

#Region "UserAction"

        Public Function GetData() As String

            Dim i As Short
            Dim ruleName As String = cbRule.SelectedItem.ToString()
            i = cbRule.SelectedValue.Split(";")(0)

            'because some of the rule ids are outdated or incorrect. Example "8YearRule"id = 123 the enum rule 123 is OptionRules.MemoApproveRmuInitial
            If (RulesWithTextBox(ruleName)) Then
                Return txtData.Text.Trim
            End If

            Select Case i

                Case OptionRules.ValidatePreviousStatus, OptionRules.ValidatePostStatus, OptionRules.LastStatusWas,
                    OptionRules.LastStatusWasNot, OptionRules.PrevStatusWas, OptionRules.PrevStatusWasNot,
                    OptionRules.Document, OptionRules.Memo, OptionRules.MemoApproveHqfon, OptionRules.MemoApproveHqInitial,
                    OptionRules.MemoApproveRmufon, OptionRules.MemoApproveRmuInitial, OptionRules.MemoDenied, OptionRules.MemoValidateRmuInitial,
                    OptionRules.MemoValidateRmufon, OptionRules.MemoValidateHqInitial, OptionRules.MemoValidateHqfon, OptionRules.MemoValidateDenied
                    Return GetMultiListSelectedValues(lstData)
                Case Else
                    Return txtData.Text.Trim
            End Select

        End Function

        Protected Sub btnAddRule_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnAddRule.Click

            Dim ruleId As Short
            Dim optionsId As Int32
            Dim DataValue As String

            ruleId = cbRule.SelectedValue.Split(";")(0)
            optionsId = Request.QueryString("wso")
            DataValue = GetData()

            'Dim ruleDao As WorkflowOptionRulesDao = factory.GetOptionRulesDao()

            ' Dim newRule As New OptionRule With {.WSOId = optionsId, .RuleId = ruleId, .RuleData = DataValue}
            Dim factory As NHibernateDaoFactory = New NHibernateDaoFactory()
            Dim ruleDao As WorkflowOptionRulesDao = factory.GetOptionRulesDao()
            Dim optionsDao As WorkflowOptionsDao = factory.GetWorkflowOptionDao()
            Dim rulesDao As RulesTypeDao = factory.GetRuleTypeDao()
            Dim OptionType As WorkflowStatusOption = optionsDao.GetById(optionsId)
            Dim ruleType = rulesDao.GetById(ruleId)

            Dim newRule As New WorkflowOptionRule()

            newRule.RuleId = ruleId

            newRule.OptionId = optionsId
            newRule.RuleValue = DataValue
            newRule.CheckAll = IIf(chkAll.Visible, chkAll.Checked, CType("True", Boolean))
            'if its a multi select value then as per user else true
            ruleDao.SaveOrUpdate(newRule)
            ruleDao.CommitChanges()
            Response.Redirect(Request.RawUrl)

        End Sub

        Protected Sub cbRuleType_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles cbRuleType.SelectedIndexChanged

            Dim ruleType As Short
            ruleType = CType(cbRuleType.SelectedValue, Short)
            BindRules(ruleType)

        End Sub

#End Region

#Region "Helpers"

        Public Sub BindRules(ByVal ruleType As Short)

            Dim wso As Int32 = CType(Request.QueryString("wso"), Int32)
            Dim wf As Short = CType(Request.QueryString("workflow"), Short)

            'Dim existing = From p In ctxLod.core_WorkStatus_Rules Where p.wso_id = wso Select p.ruleId
            'Dim lkuprulesOld = From r In ctxLod.core_lkupRules Where (Not existing.Contains(r.Id)) AndAlso (r.workFlow = wf Or r.workFlow Is Nothing) AndAlso r.ruleType = ruleType Select name = r.name, value = CType(r.Id, String) + ";" + IIf(r.prompt Is Nothing, "", r.prompt)

            Dim factory As NHibernateDaoFactory = New NHibernateDaoFactory()
            Dim ruleDao As WorkflowOptionRulesDao = factory.GetOptionRulesDao()
            Dim ruleTypesDao As RulesTypeDao = factory.GetRuleTypeDao()

            Dim existing = From p In ruleDao.GetAll() Where p.OptionType.Id = wso Select p.RuleId
            Dim all = From p In ruleTypesDao.GetAll() Where (Not existing.Contains(p.Id))
            Dim lkupRules = From r In all Where r.ruleTypeId = ruleType Select name = r.Name, value = CType(r.Id, String) + ";" + IIf(r.Prompt Is Nothing, "", r.Prompt)

            cbRule.DataSource = lkupRules
            cbRule.DataTextField = "name"
            cbRule.DataValueField = "value"
            cbRule.DataBind()

        End Sub

        Public Sub CheckForStatus()

            Dim i As Short

            If cbRule.Items.Count < 1 Then
                ShowDataTextBox()
                lblEmpty.Text = "All Rules have either been  added or not available in this category"
                lblPrompt.Text = ""
                btnAddRule.Enabled = False
                Return
            End If

            Dim wf As Short = CType(Request.QueryString("workflow"), Short)
            Dim compo As String = Request.QueryString("compo")

            btnAddRule.Enabled = True
            Dim selection As String = cbRule.SelectedValue
            Dim ruleName As String = cbRule.SelectedItem.ToString()
            If selection <> "" Then
                i = CType(cbRule.SelectedValue.Split(";")(0), Short)
                lblPrompt.Text = cbRule.SelectedValue.Split(";")(1)
                lblEmpty.Text = ""
                Dim selectedItem = cbRule.SelectedItem
                Select Case i
                    Case OptionRules.ValidatePreviousStatus, OptionRules.ValidatePostStatus, OptionRules.LastStatusWas, OptionRules.LastStatusWasNot, OptionRules.PrevStatusWas, OptionRules.PrevStatusWasNot
                        ShowDataDropDown()
                        Dim wsDao As IWorkStatusDao = New NHibernateDaoFactory().GetWorkStatusDao()
                        Dim ws As IList(Of ALOD.Core.Domain.Workflow.WorkStatus) = wsDao.GetByWorkflow(wf)
                        lstData.DataSource = ws
                        lstData.DataTextField = "Description"
                        lstData.DataValueField = "Id"
                        lstData.DataBind()

                    Case OptionRules.Document
                        ShowDataDropDown()
                        Dim dao As IDocCategoryViewDao = New NHibernateDaoFactory().GetDocCategoryViewDao()
                        Dim docViewId As Integer = dao.GetDocumentViewByWorkflowId(wf)
                        Dim viewCats As List(Of DocumentCategory2) = dao.GetCategoriesByDocumentViewId(docViewId)
                        lstData.DataSource = viewCats
                        lstData.DataTextField = "CategoryDescription"
                        lstData.DataValueField = "DocCatId"
                        lstData.DataBind()

                    Case OptionRules.Memo
                        ShowDataDropDown()
                        'populate with memo templates
                        Dim memDao As IMemoDao = New NHibernateDaoFactory().GetMemoDao()
                        lstData.DataSource = memDao.GetByCompo(compo)
                        lstData.DataTextField = "Title"
                        lstData.DataValueField = "Id"
                        lstData.DataBind()

                    Case OptionRules.MemoApproveHqfon, OptionRules.MemoApproveHqInitial,
                        OptionRules.MemoApproveRmufon, OptionRules.MemoApproveRmuInitial, OptionRules.MemoDenied
                        If (RulesWithTextBox(ruleName)) Then
                            ShowDataTextBox()
                        Else
                            ShowDataDropDown()
                            'populate with memo templates
                            Dim memDao As IMemoDao = New NHibernateDaoFactory().GetMemoDao()
                            lstData.DataSource = memDao.GetByCompo(compo)
                            lstData.DataTextField = "Title"
                            lstData.DataValueField = "Id"
                            lstData.DataBind()
                        End If

                    Case OptionRules.MemoValidateRmuInitial, OptionRules.MemoValidateRmufon,
                        OptionRules.MemoValidateHqInitial, OptionRules.MemoValidateHqfon, OptionRules.MemoValidateDenied
                        If (RulesWithTextBox(ruleName)) Then
                            ShowDataTextBox()
                        Else
                            ShowDataDropDown()
                            'populate with memo templates
                            Dim memDao As IMemoDao = New NHibernateDaoFactory().GetMemoDao()
                            lstData.DataSource = memDao.GetByCompo(compo)
                            lstData.DataTextField = "Title"
                            lstData.DataValueField = "Id"
                            lstData.DataBind()
                        End If
                    Case Else
                        ShowDataTextBox()
                End Select
            Else
                ShowDataTextBox()
                lblEmpty.Text = ""
                lblPrompt.Text = ""
            End If

        End Sub

        Public Function RulesWithTextBox(ByVal rule As String) As Boolean
            Select Case rule
                Case "8YearRule", "IN_INPM_Validations", "IN_MedTech_Validations", "IN_UnitCC_Validations", "IN_WingJa_Validations",
                     "IN_FMTech_Validations", "IN_WingCC_Validations", "IN_HROPR_Validations", "IN_HROCR_Validations", "IN_AFRDOS_Validations",
                     "IN_AFRCC_Validations", "IN_AFRVCR_Validations", "IN_AFRDOP_Validations", "IN_CAFR_Validations"
                    Return True
            End Select
            Return False
        End Function

        Public Sub ShowDataDropDown()

            If lstData.Items.Count > 0 Then
                lstData.Items.Clear()
            End If
            lstData.Visible = True
            txtData.Visible = False
            chkAll.Visible = True

        End Sub

        Public Sub ShowDataTextBox()
            txtData.Visible = True
            lstData.Visible = False
            chkAll.Visible = False
        End Sub

#End Region

#Region "Copy"

        Protected Sub BindOptions(ByVal workstep As Int32)

            Dim currentoption As Int32 = CType(Request.QueryString("wso"), Int32)

            Dim wsDao As WorkflowOptionsDao = New NHibernateDaoFactory().GetWorkflowOptionDao()
            'Dim options = From wso In ctxLod.core_WorkStatus_Options Where wso.ws_id = workstep AndAlso wso.wso_id <> currentoption Select wso.displayText, wso.wso_id
            Dim options = From wso In wsDao.GetAll() Where wso.wsStatus = workstep AndAlso wso.wsStatusOut <> currentoption Select wso.DisplayText, wso_id = wso.wsStatusOut
            cbOptions.DataSource = options
            cbOptions.DataTextField = "displayText"
            cbOptions.DataValueField = "wso_id"
            cbOptions.DataBind()

            If cbOptions.Items.Count > 0 Then
                btnCopy.Enabled = True
            Else
                btnCopy.Enabled = False
            End If

        End Sub

        Protected Sub BindSteps()

            Dim wso As Int32 = CType(Request.QueryString("wso"), Int32)
            Dim wf As Short = CType(Request.QueryString("workflow"), Short)

            Dim factory As NHibernateDaoFactory = New NHibernateDaoFactory()
            Dim wsDao As WorkStatusDao = factory.GetWorkStatusDao()

            Dim steps = From ws In wsDao.GetAll() Where ws.WorkflowId = wf Select ws.StatusCodeType.Description, ws_id = ws.Id
            cbSteps.DataSource = steps
            cbSteps.DataBind()
            cbSteps.SelectedValue = CType(Request.QueryString("step"), Int32)

            ' Dim steps = From ws In ctxLod.core_WorkStatus, s In ctxLod.core_StatusCodes Where ws.statusId = s.statusId And ws.workflowId = wf Select s.description, ws.ws_id
            'e.Result = steps

        End Sub

        Protected Sub btnCopy_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnCopy.Click
            Dim strOption As String = cbOptions.SelectedValue
            If strOption = "" Then Exit Sub
            Dim dstwso As Int32 = CType(Request.QueryString("wso"), Int32)
            Dim srcwso As Int32 = CType(cbOptions.SelectedValue, Int32)
            'NewFindi.core_workflow_sp_CopyRules(dstwso, srcwso)
            Dim factory As NHibernateDaoFactory = New NHibernateDaoFactory()
            Dim wsDao As WorkflowOptionRulesDao = factory.GetOptionRulesDao()
            wsDao.CopyRules(dstwso, srcwso)
            Response.Redirect(Request.RawUrl)

        End Sub

        Protected Sub cbSteps_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles cbSteps.SelectedIndexChanged

            Dim workstep As Int32 = CType(cbSteps.SelectedValue, Int32)
            BindOptions(workstep)

        End Sub

        Protected Sub chkCopy_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles chkCopy.CheckedChanged
            If chkCopy.Checked Then
                divCopy.Visible = True
                divAdd.Visible = False
                BindOptions(CType(Request.QueryString("step"), Int32))
            Else
                divCopy.Visible = False
                divAdd.Visible = True
            End If
        End Sub

#End Region

        Public Function DocTypeContainsSubstring(ByVal objects As IEnumerable, ByVal substring As String) As DocumentType
            Dim strings = objects.OfType(Of DocumentType)()
            For Each str As DocumentType In strings
                If str.ToString().Equals(substring) Then Return str
            Next
            Return Nothing
        End Function

        Protected Sub gvRules_RowDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewRowEventArgs) Handles gvRules.RowDataBound

            If (e.Row.RowType <> DataControlRowType.DataRow) Then
                Exit Sub
            End If

            'Dim rule As core_WorkStatus_Rule = CType(e.Row.DataItem, core_WorkStatus_Rule)
            Dim rulename = CType(DataBinder.Eval(e.Row.DataItem, "name"), String).ToLower()
            Dim lblRuleDataText As Label = CType(e.Row.FindControl("lblRuleData"), Label)
            Dim ruleDataIds As String()
            Dim ruleDataNames As New StringBuilder()
            Dim i As Integer
            Select Case rulename
                Case "document"
                    Dim arrDocTypes() As DocumentType = [Enum].GetValues(GetType(DocumentType))
                    ruleDataIds = CType(DataBinder.Eval(e.Row.DataItem, "ruleData"), String).Split(",")
                    For i = 0 To ruleDataIds.Length - 1
                        Dim tempDocType As New DocumentType
                        tempDocType = DocTypeContainsSubstring(arrDocTypes, ruleDataIds(i))
                        If Not IsNothing(tempDocType) Then
                            ruleDataNames.Append(tempDocType.ToString())
                            ruleDataNames.Append(", ")
                        End If
                    Next
                    ruleDataNames.Remove(ruleDataNames.Length - 2, 2)

                Case "validatepreviousstatus", "validatepoststatus", "laststatuswas", "laststatuswasnot", "prevstatuswas", "prevStatuswasnot", "wasinstatus", "wasnotinstatus", "isalcormajcom", "isnotalcandmajcom", "rfa"
                    ruleDataIds = CType(DataBinder.Eval(e.Row.DataItem, "ruleData"), String).Split(",")
                    Dim wsDao As IWorkStatusDao = New NHibernateDaoFactory().GetWorkStatusDao()
                    For i = 0 To ruleDataIds.Length - 1
                        If (ruleDataIds.Length > 1) Then
                            ruleDataNames.Append(wsDao.GetDescription(ruleDataIds(i)))
                            ruleDataNames.Append(", ")
                        End If
                    Next
                    If (ruleDataNames.Length > 1) Then
                        ruleDataNames.Remove(ruleDataNames.Length - 2, 2)
                    End If

                Case "memo"
                    ruleDataIds = CType(DataBinder.Eval(e.Row.DataItem, "ruleData"), String).Split(",")
                    For i = 0 To ruleDataIds.Length - 1
                        ruleDataNames.Append(CType(ruleDataIds(i), ALOD.Core.Domain.Documents.MemoType).ToString())

                        ruleDataNames.Append(", ")
                    Next
                    ruleDataNames.Remove(ruleDataNames.Length - 2, 2)

                Case "sarcrestricted", "medical", "unit", "isreinvestigationlod", "injurynonmvanonepts", "formalrecommended", "unitccquestions", "wingjaformalfindings",
                        "fullreview", "checksignature", "issarc", "cancomplete", "wingccformalaction", "directreply", "in_inpm_validations"
                    ruleDataNames.Append(CType(DataBinder.Eval(e.Row.DataItem, "ruleData"), String))

                Case "memoapprovermuinitial", "memoapprovermufon", "memoapprovehqinitial", "memoapprovehqfon", "memodenied"
                    ruleDataIds = CType(DataBinder.Eval(e.Row.DataItem, "ruleData"), String).Split(",")
                    For i = 0 To ruleDataIds.Length - 1
                        ruleDataNames.Append(CType(ruleDataIds(i), ALOD.Core.Domain.Documents.MemoType).ToString())

                        ruleDataNames.Append(", ")
                    Next
                    ruleDataNames.Remove(ruleDataNames.Length - 2, 2)

                Case "memovalidatermuinitial", "memovalidatermufon", "memovalidatehqinitial", "memovalidatehqfon", "memovalidatedenied"
                    ruleDataIds = CType(DataBinder.Eval(e.Row.DataItem, "ruleData"), String).Split(",")
                    For i = 0 To ruleDataIds.Length - 1
                        ruleDataNames.Append(CType(ruleDataIds(i), ALOD.Core.Domain.Documents.MemoType).ToString())

                        ruleDataNames.Append(", ")
                    Next
                    ruleDataNames.Remove(ruleDataNames.Length - 2, 2)

                Case "isrequestforconsultto"
                    ruleDataIds = CType(DataBinder.Eval(e.Row.DataItem, "ruleData"), String).Split(",")

                    If (ruleDataIds.Count = 0) Then
                        Exit Select
                    End If

                    ruleDataNames.Append(UserGroupDao.GetById(ruleDataIds(0)).Description)
                Case Else
                    ruleDataNames.Append(CType(DataBinder.Eval(e.Row.DataItem, "ruleData"), String))
            End Select
            lblRuleDataText.Text = ruleDataNames.ToString()

        End Sub

    End Class

End Namespace