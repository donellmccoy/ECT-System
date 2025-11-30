Namespace Web.Admin

    Partial Class Secure_Shared_Admin_UserTracking
        Inherits System.Web.UI.Page

        Protected Sub lnkManageUsers_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles lnkManageUsers.Click
            Session("EditId") = Nothing
            Response.Redirect("~/Secure/Shared/Admin/ManageUsers.aspx")
        End Sub

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

            If (Session("EditId") Is Nothing) Then
                Response.Redirect(Resources._Global.StartPage, True)
            End If

            If (Not IsPostBack) Then
                Page.ClientScript.RegisterClientScriptInclude("ChangeLogScript", Request.ApplicationPath + "/Script/ChangeLog.js")

            End If
        End Sub

        Protected Sub RowBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewRowEventArgs) Handles gvTracking.RowDataBound

            If (e.Row.RowType <> DataControlRowType.DataRow) Then
                Exit Sub
            End If
            Me.chkShowAll.Visible = True
            Dim row As DataRowView = CType(e.Row.DataItem, DataRowView)
            Dim logChanges As Boolean = CBool(row("logChanges"))

            Dim link As HyperLink
            link = CType(e.Row.FindControl("lnkAction"), HyperLink)

            If (Not logChanges) Then
                link.Font.Underline = False
            Else
                link.NavigateUrl = "#"
                link.Attributes.Add("onclick", "showChangeSet('" + row("logId").ToString() + "','0'); return false;")
            End If

        End Sub

    End Class

End Namespace