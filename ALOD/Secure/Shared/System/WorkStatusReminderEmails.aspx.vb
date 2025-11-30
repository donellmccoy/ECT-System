Imports ALOD.Core.Domain.Common
Imports ALOD.Data
Imports ALODWebUtility.Common
Imports ALODWebUtility.LookUps
Imports ALODWebUtility.Worklfow

Namespace Web.Sys

    Partial Class Secure_Shared_System_WorkStatusReminderEmails
        Inherits System.Web.UI.Page

        Private emailTemplateSource As IList(Of EmailTemplate)
        Private statusCodeSource As WorkStatusList
        Private userGroupSource As LookUp
        Private workFlowSource As WorkFlowList

        Private ReadOnly Property EmailTemplates() As IList(Of EmailTemplate)
            Get
                If (emailTemplateSource Is Nothing) Then
                    Dim list As IQueryable(Of EmailTemplate) = New NHibernateDaoFactory().GetEmailTemplateDao().GetAll()
                    emailTemplateSource = (From l In list Where l.Compo = Session("Compo") Select l).ToList()
                End If
                Return emailTemplateSource
            End Get
        End Property

        Private ReadOnly Property StatusCodes() As WorkStatusList
            Get
                If (statusCodeSource Is Nothing) Then
                    statusCodeSource = New WorkStatusList()
                    statusCodeSource.GetByWorklfow(CType(Request.QueryString("workflow"), Short))
                    'statusCodeSource.GetByWorklfow(1)
                End If
                Return statusCodeSource
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
                    'workFlowSource.GetByCompoAndModule(Session("Compo"), 2)
                End If
                Return workFlowSource
            End Get
        End Property

        Public Sub InitControls()

            Dim wso As Short = CType(Request.QueryString("wso"), Short)

            Dim workflow As Short = CType(Request.QueryString("workflow"), Short)
            Dim statusId As Int32 = CType(Request.QueryString("status"), Integer)
            Dim compo As String = CType(Request.QueryString("compo"), String)
            Dim stepId As Int32 = CType(Request.QueryString("step"), Integer)

            Dim dao = New ReminderEmailsDao()
            gvReminders.DataSource = dao.ReminderEmailGetSettingsByStatus(workflow, statusId, compo)
            gvReminders.DataBind()

            Dim wfname As String = (From w In WorkFlows Where w.Id = workflow Select w.Title).First
            Dim workstatus As String = (From w In StatusCodes Where w.Id = stepId Select w.Description).First

            lblCompo.Text = Server.HtmlEncode(GetCompoString(compo))
            lblOption.Text = Server.HtmlEncode(Request.QueryString("text"))
            lblWorkflow.Text = Server.HtmlEncode(wfname)
            lblStep.Text = Server.HtmlEncode(workstatus)

            cbType.DataBind()

            cbType.SelectedValue = cbType.Items.FindByText("Send Email").Value
            cbType.Enabled = False

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
        End Sub

        Protected Sub btnAddAction_Click(sender As Object, e As System.EventArgs) Handles btnAddAction.Click

            Dim dao = New ReminderEmailsDao()
            Dim workflow As Integer = Request.QueryString("workflow")
            Dim statusId As Integer = Request.QueryString("status")
            Dim compo As Integer = Request.QueryString("compo")
            Dim groupId As Integer = cbTarget.SelectedValue
            Dim templateId As Integer = cbData.SelectedValue
            Dim interval As Integer

            If IsNumeric(intervalTime.Text.ToString()) Then
                If Convert.ToInt32(intervalTime.Text.ToString()) > 0 And Convert.ToInt32(intervalTime.Text.ToString()) <= 999 Then
                    interval = Convert.ToInt32(intervalTime.Text.ToString())
                    intervalError.Visible = False
                Else
                    intervalError.Visible = True
                    Exit Sub
                End If
            Else
                intervalError.Visible = True
                Exit Sub
            End If

            dao.ReminderEmailSettingAddByStatus(workflow, statusId, compo, groupId, templateId, interval)
            Response.Redirect(Request.RawUrl)

        End Sub

        Protected Sub gvReminderss_RowCommand(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewCommandEventArgs) Handles gvReminders.RowCommand

            If e.CommandName = "Delete" Then
                Dim dao = New ReminderEmailsDao()
                dao.ReminderEmailSettingsDelete(e.CommandArgument)
                Response.Redirect(Request.RawUrl)
            End If

        End Sub

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

            If (Not IsPostBack) Then
                InitControls()
            End If

        End Sub

    End Class

End Namespace