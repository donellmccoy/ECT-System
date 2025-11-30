Imports ALODWebUtility.Worklfow

Namespace Web.Sys

    Partial Class Secure_Shared_System_WorkflowPerms
        Inherits System.Web.UI.Page

        Protected Sub btnCancel_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnCancel.Click
            Response.Redirect("~/Secure/Shared/System/Workflows.aspx?compo=" + Request.QueryString("compo") + "&module=" + Request.QueryString("module"))
        End Sub

        Protected Sub Button1_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles Button1.Click

            Dim workflowId As Byte = CByte(Request.QueryString("id"))
            Dim list As New WorkflowPermissionList
            Dim perm As WorkflowPermission
            Dim view As CheckBox
            Dim create As CheckBox

            For Each row As GridViewRow In GridView1.Rows

                view = CType(row.FindControl("cbView"), CheckBox)
                create = CType(row.FindControl("cbCreate"), CheckBox)

                perm = New WorkflowPermission()
                perm.GroupId = CShort(GridView1.DataKeys(row.RowIndex)(0))
                perm.CanCreate = create.Checked
                perm.CanView = view.Checked

                If (perm.CanView Or perm.CanCreate) Then
                    list.Add(perm)
                End If

            Next

            list.UpdateWorkflow(workflowId, cbCompo.SelectedValue)

        End Sub

        Protected Sub CompoBound(ByVal sender As Object, ByVal e As System.EventArgs) Handles cbCompo.DataBound

            If (Not IsPostBack) Then
                cbCompo.SelectedValue = Request.QueryString("compo")
            End If

        End Sub

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

            If (Not IsPostBack) Then

                Dim flow As New Workflow(CByte(Request.QueryString("id")))
                lblWorkflow.Text = Server.HtmlEncode(flow.Title)

            End If

        End Sub

    End Class

End Namespace