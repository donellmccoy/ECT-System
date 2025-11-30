Imports ALOD.Core.Domain.Common
Imports ALOD.Core.Domain.Documents
Imports ALOD.Core.Domain.Workflow
Imports ALOD.Data
Imports ALODWebUtility.Common
Imports ALODWebUtility.LookUps
Imports ALODWebUtility.Worklfow

Namespace Web.Sys

    Partial Class Secure_Shared_System_WorkStatusActions
        Inherits System.Web.UI.Page

        Private emailTemplateSource As IList(Of EmailTemplate)

        Private memoSource As IList(Of MemoTemplate)

        Private userGroupSource As LookUp

        Private workFlowSource As WorkFlowList

        'Protected _memos As MemoTemplateList
        Private workStatusCodeSource As WorkStatusList

        Private ReadOnly Property EmailTemplates() As IList(Of EmailTemplate)
            Get
                If (emailTemplateSource Is Nothing) Then
                    Dim list As IQueryable(Of EmailTemplate) = New NHibernateDaoFactory().GetEmailTemplateDao().GetAll()
                    emailTemplateSource = (From l In list Where l.Compo = Session("Compo") Select l).ToList()
                End If
                Return emailTemplateSource
            End Get
        End Property

        Private ReadOnly Property MemoTemplates() As IList(Of MemoTemplate)
            Get
                If (memoSource Is Nothing) Then
                    Dim list As IList(Of MemoTemplate) = New NHibernateDaoFactory().GetMemoDao().GetAllTemplates()
                    memoSource = (From l In list Where l.Component = Session("Compo") Select l Order By l.Id).ToList()
                End If
                Return memoSource
            End Get
        End Property

        Private ReadOnly Property UserGroups() As LookUp
            Get
                If (userGroupSource Is Nothing) Then
                    userGroupSource = New LookUp()
                    userGroupSource.GetGroupsByCompo(Session("Compo"))
                End If
                Return userGroupSource
            End Get
        End Property

        Private ReadOnly Property WorkFlows() As WorkFlowList
            Get
                If (workFlowSource Is Nothing) Then
                    workFlowSource = New WorkFlowList()
                    workFlowSource.GetByCompoAndModule(Session("Compo"), CByte(Request.QueryString("module")))
                End If
                Return workFlowSource
            End Get
        End Property

        Private ReadOnly Property WorkStatusCodes() As WorkStatusList
            Get
                If (workStatusCodeSource Is Nothing) Then
                    workStatusCodeSource = New WorkStatusList()
                    workStatusCodeSource.GetByWorklfow(CType(Request.QueryString("workflow"), Short))
                End If
                Return workStatusCodeSource
            End Get
        End Property

        Protected Sub BindOptions(ByVal workstep As Int32)

            Dim currentoption As Int32 = CType(Request.QueryString("wso"), Int32)
            Dim list As New WorkStatusOptionList()
            list.GetByWorkStatus(workstep, Session("Compo"))

            cbOptions.DataSource = From o In list Where o.Id <> currentoption Select o
            cbOptions.DataTextField = "Text"
            cbOptions.DataValueField = "Id"
            cbOptions.DataBind()

            If cbOptions.Items.Count > 0 Then
                btnCopy.Enabled = True
            Else
                btnCopy.Enabled = False
            End If

        End Sub

        Protected Sub btnAddAction_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnAddAction.Click

            Dim action As New WorkflowAction
            action.OptionId = CInt(Request.QueryString("wso"))
            action.Type = CByte(cbType.SelectedValue)

            If (cbTarget.Items.Count > 0) Then
                action.Target = CInt(cbTarget.SelectedValue)
            End If

            If (cbData.Items.Count > 0) Then
                action.Data = CInt(cbData.SelectedValue)
            End If

            action.Insert()

            Response.Redirect(Request.RawUrl)

        End Sub

        Protected Sub btnCopy_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnCopy.Click
            Dim strOption As String = cbOptions.SelectedValue
            If strOption = "" Then Exit Sub
            Dim dstwso As Int32 = CType(Request.QueryString("wso"), Int32)
            Dim srcwso As Int32 = CType(cbOptions.SelectedValue, Int32)
            WorkflowActionList.CopyAction(srcwso, dstwso)
            Response.Redirect(Request.RawUrl)

        End Sub

        Protected Sub cbSteps_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles cbSteps.SelectedIndexChanged

            Dim workstep As Int32 = CType(cbSteps.SelectedValue, Int32)
            BindOptions(workstep)

        End Sub

        Protected Sub cbType_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles cbType.SelectedIndexChanged

            cbData.Enabled = True
            cbTarget.Enabled = True
            btnAddAction.Enabled = True
            Dim data As New LookUp

            Dim compo As String = Request.QueryString("compo")

            cbTarget.AutoPostBack = False
            cbData.Items.Clear()
            cbTarget.Items.Clear()

            Select Case CShort(cbType.SelectedValue)
                Case 0

                    cbData.Enabled = False
                    cbTarget.Enabled = False
                    btnAddAction.Enabled = False
                Case WorkflowActionType.SignMemo
                    'populate with memo templates
                    cbTarget.DataSource = From t In MemoTemplates Select t.Id, t.Title
                    cbTarget.DataTextField = "Title"
                    cbTarget.DataValueField = "Id"
                    cbTarget.DataBind()
                    cbData.Items.Clear()
                    cbData.Enabled = False
                Case WorkflowActionType.SendEmail
                    'populate data with email options
                    cbData.DataSource = From t In EmailTemplates Where t.Compo = compo Select t.Title, t.Id
                    cbData.DataTextField = "Title"
                    cbData.DataValueField = "Id"
                    cbData.DataBind()
                    'and group to send to
                    cbTarget.DataSource = UserGroups
                    cbTarget.DataTextField = "text"
                    cbTarget.DataValueField = "value"
                    cbTarget.DataBind()
                Case WorkflowActionType.SendLessonsLearnedEmail
                    'populate data with email options
                    cbData.DataSource = From t In EmailTemplates Where t.Compo = compo And t.Title = "Lessons Learned" Select t.Title, t.Id
                    cbData.DataTextField = "Title"
                    cbData.DataValueField = "Id"
                    cbData.DataBind()
                    'No Group
                    cbTarget.Enabled = False
                Case WorkflowActionType.AddSignature
                    'target is the group
                    cbTarget.DataSource = UserGroups
                    cbTarget.DataTextField = "text"
                    cbTarget.DataValueField = "value"
                    cbTarget.DataBind()
                    'data not used
                    cbData.Enabled = False
                Case WorkflowActionType.RemoveSignature
                    'target is the group
                    cbTarget.DataSource = UserGroups
                    cbTarget.DataTextField = "text"
                    cbTarget.DataValueField = "value"
                    cbTarget.DataBind()

                    cbData.DataSource = WorkStatusCodes
                    cbData.DataTextField = "Description"
                    cbData.DataValueField = "Id"
                    cbData.DataBind()
                Case WorkflowActionType.RecommendCancelFormal
                    cbData.DataSource = GetBooleanList()
                    cbData.DataTextField = "text"
                    cbData.DataValueField = "value"
                    cbData.DataBind()
                    cbTarget.Enabled = False
                Case Else
                    cbData.Enabled = False
                    cbTarget.Enabled = False
            End Select

        End Sub

        Protected Sub chkCopy_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles chkCopy.CheckedChanged
            If chkCopy.Checked Then
                divCopy.Visible = True
                BindOptions(CType(Request.QueryString("step"), Int32))
                divAdd.Visible = False
            Else
                divCopy.Visible = False
                divAdd.Visible = True
            End If
        End Sub

        Protected Sub gvActions_RowCommand(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewCommandEventArgs) Handles gvActions.RowCommand

            If e.CommandName = "Delete" Then
                Dim wsaId As Integer = CInt(e.CommandArgument)
                Dim dao As WorkflowOptionActionsDao = New NHibernateDaoFactory().GetOptionActionsDao()
                Dim action As ALOD.Core.Domain.Workflow.WorkflowOptionAction = dao.GetById(wsaId)
                dao.Delete(action)
                dao.CommitChanges()
                Response.Redirect(Request.RawUrl)
            End If

        End Sub

        Protected Sub InitControls()

            Dim wso As Short = CType(Request.QueryString("wso"), Short)
            Dim dao As WorkflowOptionActionsDao = New NHibernateDaoFactory().GetOptionActionsDao()
            gvActions.DataSource = From optionActions In dao.GetAll() Where optionActions.OptionType.Id = wso
            gvActions.DataBind()

            Dim workflow As Short = CType(Request.QueryString("workflow"), Short)
            Dim stepId As Int32 = CType(Request.QueryString("step"), Integer)
            Dim compo As String = CType(Request.QueryString("compo"), String)

            Dim wfname As String = (From w In WorkFlows Where w.Id = workflow Select w.Title).FirstOrDefault
            Dim workstatus As String = (From w In WorkStatusCodes Where w.Id = stepId Select w.Description).FirstOrDefault

            lblCompo.Text = Server.HtmlEncode(GetCompoString(compo))
            lblOption.Text = Server.HtmlEncode(Request.QueryString("text"))
            lblWorkflow.Text = Server.HtmlEncode(wfname)
            lblStep.Text = Server.HtmlEncode(workstatus)
        End Sub

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

            If (Not IsPostBack) Then
                InitControls()
            End If

        End Sub

        Protected Sub RowBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewRowEventArgs) Handles gvActions.RowDataBound

            If (e.Row.RowType <> DataControlRowType.DataRow) Then
                Exit Sub
            End If

            Dim action As ALOD.Core.Domain.Workflow.WorkflowOptionAction = CType(e.Row.DataItem, ALOD.Core.Domain.Workflow.WorkflowOptionAction)
            Dim lblTarget As Label = CType(e.Row.FindControl("lblTarget"), Label)
            Dim lblData As Label = CType(e.Row.FindControl("lblData"), Label)

            Select Case action.ActionType.Id

                Case WorkflowActionType.SendEmail
                    'target is group, data is email template
                    lblTarget.Text = Server.HtmlEncode((From g In UserGroups Where g.value = action.Target.ToString() Select g.text).FirstOrDefault)
                    lblData.Text = Server.HtmlEncode((From email In EmailTemplates Where email.Id = action.Data Select email.Title).FirstOrDefault)
                Case WorkflowActionType.SignMemo
                    'target is memo, data is empty
                    lblTarget.Text = Server.HtmlEncode((From memos In MemoTemplates Where memos.Id = action.Target Select memos.Title).FirstOrDefault)
                    lblData.Text = "--"
                Case WorkflowActionType.AddSignature
                    lblTarget.Text = Server.HtmlEncode((From u In UserGroups Where u.value = action.Target.ToString() Select u.text).FirstOrDefault)
                    lblData.Text = "--"
                Case WorkflowActionType.RemoveSignature
                    lblTarget.Text = Server.HtmlEncode((From u In UserGroups Where u.value = action.Target.ToString() Select u.text).FirstOrDefault)
                    lblData.Text = Server.HtmlEncode((From ws In WorkStatusCodes Where ws.Id = action.Data Select ws.Description).FirstOrDefault)
                Case WorkflowActionType.SendLessonsLearnedEmail
                    'No Group for LL, data is email template
                    lblTarget.Text = "--"
                    lblData.Text = Server.HtmlEncode((From email In EmailTemplates Where email.Id = action.Data Select email.Title).FirstOrDefault)
                Case WorkflowActionType.RecommendCancelFormal
                    lblData.Text = Server.HtmlEncode((From bools In GetBooleanList() Where bools.Value = action.Data Select bools.Text).FirstOrDefault)
            End Select

        End Sub

        Protected Sub TypeDataBound(ByVal sender As Object, ByVal e As System.EventArgs) Handles cbType.DataBound
            cbType.Items.Insert(0, New ListItem("-- Select Action --", "0"))
        End Sub

        Private Function GetBooleanList() As IList(Of ListItem)
            Dim values As New List(Of ListItem)

            values.Add(New ListItem("True", 1))
            values.Add(New ListItem("False", 0))

            Return values
        End Function

    End Class

End Namespace