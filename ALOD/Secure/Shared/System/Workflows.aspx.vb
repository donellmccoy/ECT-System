Imports System.Reflection
Imports ALOD.Core.Domain.Workflow
Imports ALOD.Core.Interfaces.DAOInterfaces
Imports ALOD.Data
Imports ALOD.Data.Services

Namespace Web.Sys

    Partial Class Secure_Shared_System_Workflows
        Inherits System.Web.UI.Page

        Private _workflowDao As IWorkflowDao

        Protected ReadOnly Property WorkflowDao As IWorkflowDao
            Get
                If (_workflowDao Is Nothing) Then
                    _workflowDao = New NHibernateDaoFactory().GetWorkflowDao()
                End If

                Return _workflowDao
            End Get
        End Property

        Protected Sub CompoDataBound(ByVal sender As Object, ByVal e As System.EventArgs) Handles cbCompo.DataBound

            Dim list As DropDownList = CType(sender, DropDownList)

            For Each item As ListItem In list.Items
                If (item.Value = "0") Then
                    list.Items.Remove(item)
                    Exit Sub
                End If
            Next

        End Sub

        Protected Sub gvWorkflows_RowDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewRowEventArgs) Handles gvWorkflows.RowDataBound
            Dim ddlTemp As New DropDownList
            If e.Row.RowType = DataControlRowType.DataRow Then
                ddlTemp = DirectCast(e.Row.FindControl("DropDownList1"), DropDownList)
                If Not ddlTemp Is Nothing Then
                    For Each currField As FieldInfo In GetType(ModuleType).GetFields
                        Dim myListItem As New ListItem
                        Dim modTypeTemp As ModuleType
                        myListItem.Value = CType(currField.GetValue(modTypeTemp), Byte)
                        myListItem.Text = currField.Name
                        If myListItem.Value <> 0 Then
                            ddlTemp.Items.Add(myListItem)
                        End If
                    Next
                    ddlTemp.SelectedIndex = ddlModule.SelectedIndex
                End If

                If (e.Row.RowIndex = gvWorkflows.EditIndex) Then
                    Dim ddlStatus As DropDownList = e.Row.FindControl("DropDownList3")
                    ddlStatus.SelectedValue = LookupService.GetWorkflowInitialStatusCode(Integer.Parse(cbCompo.SelectedValue), Integer.Parse(ddlModule.SelectedValue), gvWorkflows.DataKeys(e.Row.RowIndex)("Id"))
                End If
            End If
        End Sub

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

            If (Not IsPostBack) Then

                If (Request.QueryString("compo") IsNot Nothing) Then
                    cbCompo.SelectedValue = Request.QueryString("compo")
                End If

                'Populate Module drop down
                For Each currField As FieldInfo In GetType(ModuleType).GetFields
                    Dim myListItem As New ListItem
                    Dim modTypeTemp As ModuleType
                    Dim blnInsert As Boolean = True
                    Dim currValue As Integer
                    currValue = CType(currField.GetValue(modTypeTemp), Byte)
                    myListItem.Value = currValue
                    myListItem.Text = WorkflowDao.GetCaseType(currValue)  'CurrField.Name
                    For Each currItem In ddlModule.Items  'Keep from loading duplicate values on page reload
                        If currItem.ToString() = myListItem.Text.ToString() Then
                            blnInsert = False
                        End If
                    Next
                    If blnInsert Then
                        ddlModule.Items.Add(myListItem)
                    End If
                Next

                If (Request.QueryString("module") IsNot Nothing) Then
                    ddlModule.SelectedValue = Request.QueryString("module")
                Else
                    ddlModule.SelectedValue = ModuleType.LOD
                End If

            End If

        End Sub

        Protected Sub WorkflowCommand(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.CommandEventArgs)
            If (e.CommandName = "EditSteps") Then
                Response.Redirect("~/Secure/Shared/System/WorkStatus.aspx?id=" + e.CommandArgument + "&compo=" + cbCompo.SelectedValue + "&module=" + ddlModule.SelectedValue)
            ElseIf (e.CommandName = "EditPerms") Then
                Response.Redirect("~/Secure/Shared/System/WorkflowPerms.aspx?id=" + e.CommandArgument + "&compo=" + cbCompo.SelectedValue + "&module=" + ddlModule.SelectedValue)
            End If
        End Sub

    End Class

End Namespace