Imports ALOD.Core.Domain.DBSign
Imports ALOD.Data
Imports ALODWebUtility.Worklfow

Namespace Web.Sys

    Partial Class Secure_Shared_System_WorkStatus
        Inherits System.Web.UI.Page

        Protected _compo As String
        Protected _module As Byte
        Protected _options As WorkStatusOptionList
        Protected _workflow As Integer

        Protected Sub btnMoveDown_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnMoveDown.Click

            Dim order As Integer = CInt(txtMoveOrder.Text)
            Dim status As New WorkStatus()
            status.Id = CInt(txtMoveId.Text)
            status.SetOrder(order + 1)
            InitControls()

        End Sub

        Protected Sub btnMoveUp_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnMoveUp.Click

            Dim order As Integer = CInt(txtMoveOrder.Text)

            If (order = 0) Then
                Exit Sub 'already at the top
            End If

            Dim status As New WorkStatus()
            status.Id = CInt(txtMoveId.Text)
            status.SetOrder(order - 1)
            InitControls()

        End Sub

        Protected Sub btnOptionGo_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnOptionGo.Click

            'do some validation
            If (txtEditValue.Text.Trim.Length = 0) Then
                Exit Sub
            End If

            Dim parts As String() = txtEditValue.Text.Split("|")

            If (parts.Length <> 8) Then
                Exit Sub
            End If

            Dim opt As New WorkStatusOption
            opt.WorkStatusId = CInt(parts(0))
            opt.Id = CInt(parts(1))
            opt.StatusOut = CInt(parts(2))
            opt.SortOrder = CInt(parts(3))
            opt.Active = Boolean.Parse(parts(4))
            opt.Text = parts(5)
            opt.DBSignTemplate = CByte(parts(6))
            opt.Compo = CInt(parts(7))

            If (opt.Save()) Then
                InitControls()
            End If

        End Sub

        Protected Sub btnRefresh_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnRefresh.Click
            InitControls()
        End Sub

        Protected Sub DeleteOption(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.CommandEventArgs)

            If (e.CommandName = "Delete") Then
                Dim opt As New WorkStatusOption
                opt.Id = CInt(e.CommandArgument)
                opt.Delete()
                InitControls()

            End If

        End Sub

        Protected Function GetOptionCount(ByVal workstatus As Integer) As Integer
            Return GetOptions(workstatus).Count()
        End Function

        Protected Function GetOptions(ByVal workstatus As Integer) As List(Of WorkStatusOption)

            If (_options Is Nothing) Then
                _options = New WorkStatusOptionList
                _options.GetByWorkflowAll(_workflow)
            End If

            Return (From t In _options Where t.WorkStatusId = workstatus Select t Order By t.SortOrder).ToList()

        End Function

        Protected Sub InitControls()

            'populate DBSign templates
            For Each item As DBSignTemplateId In [Enum].GetValues(GetType(DBSignTemplateId))
                TemplateDropDown.Items.Add(New ListItem(item.ToString, CStr(item)))
            Next

            'get current steps (workstatus)
            Dim steps As New WorkStatusList
            steps.GetByWorklfow(_workflow)

            rptSteps.DataSource = steps
            rptSteps.DataBind()

            'get options (next action options)

            'get status codes for adding new steps
            Dim codes As New StatusCodeList
            codes.GetByCompoAndModule(_compo, _module)

            'get the current steps
            Dim curSteps =
                From t In steps
                Select t.Status

            'bind our add/edit status in/out lists
            cbOptionStatusIn.Items.Clear()
            cbOptionStatusIn.DataSource = steps
            cbOptionStatusIn.DataTextField = "Description"
            cbOptionStatusIn.DataValueField = "Id"
            cbOptionStatusIn.DataBind()

            cbOptionStatusOut.Items.Clear()
            cbOptionStatusOut.DataSource = steps
            cbOptionStatusOut.DataTextField = "Description"
            cbOptionStatusOut.DataValueField = "Id"
            cbOptionStatusOut.DataBind()

        End Sub

        Protected Sub InitData()
            If (Not IsPostBack) Then
                _workflow = CInt(Request.QueryString("id"))
                _module = CByte(Request.QueryString("module"))
                _compo = Request.QueryString("compo")

                ViewState("workflow") = _workflow
                ViewState("module") = _module
                ViewState("compo") = _compo
            Else
                _workflow = CInt(ViewState("workflow"))
                _module = CInt(ViewState("module"))
                _compo = CInt(ViewState("compo"))
            End If
        End Sub

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

            InitData()

            If (Not IsPostBack) Then
                InitControls()
            End If

        End Sub

        Protected Function RowStyle(ByVal index As Integer) As String
            Return IIf(index Mod 2, "workPanelAlt", "workPanel")
        End Function

        Protected Sub rptChild_ItemDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.RepeaterItemEventArgs)

            Dim item As WorkStatusOption = CType(e.Item.DataItem, WorkStatusOption)
            Dim workStatusItem As WorkStatus = CType((CType(e.Item.Parent.Parent, RepeaterItem)).DataItem, WorkStatus)
            StatusHiddenField.Value = workStatusItem.Status

            If (e.Item.ItemType = ListItemType.Item Or e.Item.ItemType = ListItemType.AlternatingItem) Then

                'handle the edit link

                Dim link As LinkButton = e.Item.FindControl("lnkEdit")
                link.Attributes.Add("onclick",
                                    "editOption(" +
                                    item.WorkStatusId.ToString() + "," +
                                    item.Id.ToString() + "," +
                                    item.StatusOut.ToString() + "," +
                                    item.SortOrder.ToString() + "," +
                                    "'" + item.Text + "'," +
                                    item.DBSignTemplate.ToString() + "," +
                                    item.Compo.ToString() +
                                    "); return false;")

                'the rules link
                Dim rules As HyperLink = CType(e.Item.FindControl("lnkRules"), HyperLink)
                Dim url As New StringBuilder
                url.Append(Page.ResolveUrl("~/Secure/Shared/System/WorkStatusRules.aspx"))
                url.Append("?wso=" + item.Id.ToString())
                url.Append("&workflow=" + Request.QueryString("id").ToString())
                url.Append("&step=" + item.WorkStatusId.ToString())
                url.Append("&module=" + Request.QueryString("module").ToString())
                url.Append("&compo=" + Request.QueryString("compo").ToString())
                url.Append("&text=" + Server.UrlEncode(item.Text))

                rules.Attributes.Add("onclick", "showEditor('" + url.ToString() + "'); return false;")

                Dim actions As HyperLink = CType(e.Item.FindControl("lnkActions"), HyperLink)

                actions.Attributes.Add("onclick", "showEditor('" + url.Replace("WorkStatusRules", "WorkStatusActions").ToString() + "'); return false;")

                Dim lblRule As Label = CType(e.Item.FindControl("lblRules"), Label)
                Dim lblAction As Label = CType(e.Item.FindControl("lblActions"), Label)

                lblRule.Text = Server.HtmlEncode((From t In New NHibernateDaoFactory().GetOptionRulesDao().GetAll Where t.OptionType.Id = item.Id Select t.Id).Count.ToString())
                lblAction.Text = Server.HtmlEncode((From t In New NHibernateDaoFactory().GetOptionActionsDao().GetAll Where t.OptionType.Id = item.Id Select t.Id).Count.ToString())

                Dim TemplateLabel As Label = CType(e.Item.FindControl("TemplateLabel"), Label)
                Dim template As DBSignTemplateId = item.DBSignTemplate
                TemplateLabel.Text = Server.HtmlEncode(template.ToString())

                WorkStatusHiddenField.Value = item.WorkStatusId.ToString()
                WsoHiddenField.Value = item.Id.ToString()

            ElseIf e.Item.ItemType = ListItemType.Footer Then

                Dim dao = New ReminderEmailsDao()
                Dim reminder As DataSet = dao.ReminderEmailGetSettingsByStatus(Request.QueryString("id").ToString(),
                                                                                      Server.UrlEncode(StatusHiddenField.Value.ToString()),
                                                                                      Request.QueryString("compo").ToString())

                Dim reminderCount As Label = CType(e.Item.FindControl("lblReminderCount"), Label)
                reminderCount.Text = reminder.Tables(0).Rows.Count

                If e.Item.Parent.Controls.Count <= 1 Then
                    e.Item.Visible = False

                    If Convert.ToInt32(reminderCount.Text.ToString()) > 0 Then
                        dao.ReminderEmailSettingsDeleteByStatus(Request.QueryString("id").ToString(), StatusHiddenField.Value.ToString())
                    End If
                    Exit Sub

                End If

                Dim url As New StringBuilder
                url.Append(Page.ResolveUrl("~/Secure/Shared/System/WorkStatusReminderEmails.aspx"))
                url.Append("?wso=" + WsoHiddenField.Value.ToString())
                url.Append("&workflow=" + Request.QueryString("id").ToString())
                'url.Append("&step=" + item.WorkStatusId.ToString())
                url.Append("&step=" + Server.UrlEncode(WorkStatusHiddenField.Value.ToString()))
                url.Append("&module=" + Request.QueryString("module").ToString())
                url.Append("&compo=" + Request.QueryString("compo").ToString())
                url.Append("&text=" + Server.UrlEncode("Reminder Emails").ToString())
                url.Append("&status=" + Server.UrlEncode(StatusHiddenField.Value.ToString()))

                Dim emailActions As HyperLink = CType(e.Item.FindControl("linkEmailActions"), HyperLink)
                emailActions.Attributes.Add("onclick", "showEditor('" + url.ToString() + "'); return false;")
            Else

            End If

        End Sub

        Protected Sub rptSteps_ItemDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.RepeaterItemEventArgs) Handles rptSteps.ItemDataBound

            If (e.Item.ItemType <> ListItemType.Item AndAlso e.Item.ItemType <> ListItemType.AlternatingItem) Then
                Exit Sub
            End If

            Dim item As WorkStatus = CType(e.Item.DataItem, WorkStatus)
            Dim img As Image = e.Item.FindControl("imgMoveUp")

            'move up and down
            'moveStatus(false, <%#Eval("Id") %>, <%# Eval("SortOrder") %>); return false;
            img.Attributes.Add("onclick", "moveStatus(true, " + item.Id.ToString() + "," + item.SortOrder.ToString() + "); return false;")
            img = e.Item.FindControl("imgMoveDown")
            img.Attributes.Add("onclick", "moveStatus(false, " + item.Id.ToString() + "," + item.SortOrder.ToString() + "); return false;")

            'add option
            'editOption(<%# Eval("Id") %>,0, 0, <%# GetOptionCount(Eval("Id")) + 1 %>,"", 0); return false;'
            img = e.Item.FindControl("imgAddOption")
            img.Attributes.Add("onclick", "editOption(" + item.Id.ToString() + ",0, 0," + (GetOptionCount(item.Id) + 1).ToString() + ",'', 0, 0); return false;")
        End Sub

    End Class

End Namespace