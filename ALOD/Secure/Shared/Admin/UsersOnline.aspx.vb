Imports ALOD.Core.Interfaces.DAOInterfaces
Imports ALOD.Data

Namespace Web.Admin

    Partial Class Secure_Shared_Admin_UsersOnline
        Inherits System.Web.UI.Page

        Protected Sub GridView1_RowCommand(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewCommandEventArgs) Handles GridView1.RowCommand

            If (e.CommandName = "ClearUser") Then

                Dim userId As Integer = 0
                Integer.TryParse(e.CommandArgument, userId)

                If (userId = 0) Then
                    Exit Sub
                End If

                Dim dao As IUserDao = New NHibernateDaoFactory().GetUserDao()
                dao.ClearOnlineUser(userId)

                GridView1.DataBind()

            End If

        End Sub

        Protected Sub GridView1_RowDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewRowEventArgs) Handles GridView1.RowDataBound

            If (e.Row.RowType <> DataControlRowType.DataRow) Then
                Exit Sub
            End If

            Dim view As DataRowView = CType(e.Row.DataItem, DataRowView)
            Dim span As TimeSpan = DateTime.Now.Subtract(CDate(view("loginTime")))
            Dim text As String = String.Format("{0:00}:{1:00}", span.Hours, span.Minutes)
            CType(e.Row.FindControl("TimeLabel"), Label).Text = text

        End Sub

    End Class

End Namespace