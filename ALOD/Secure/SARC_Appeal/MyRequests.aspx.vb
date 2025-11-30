Imports ALOD.Core.Domain.Workflow
Imports ALOD.Core.Interfaces.DAOInterfaces
Imports ALODWebUtility.Common
Imports ALODWebUtility.Permission.Search

Namespace Web.APSA

    Partial Class Secure_apsa_MyRequests
        Inherits System.Web.UI.Page

        Protected Sub gvResults_RowDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewRowEventArgs) Handles gvResults.RowDataBound
            HeaderRowBinding(sender, e, "CaseId")

            If (e.Row.RowType = DataControlRowType.DataRow) Then

                Dim data As DataRowView = CType(e.Row.DataItem, DataRowView)
            End If

        End Sub

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

            If (Not IsPostBack) Then
                'clear any pending locks this user might have

                Dim dao As ICaseLockDao = New ALOD.Data.NHibernateDaoFactory().GetCaseLockDao()
                dao.ClearLocksForUser(SESSION_USER_ID)

            End If

        End Sub

        Private Sub gvResults_RowCommand(ByVal sender As Object, ByVal e As GridViewCommandEventArgs) Handles gvResults.RowCommand

            If (e.CommandName = "view") Then

                Dim parts() As String = e.CommandArgument.ToString().Split(";")
                Dim strQuery As New StringBuilder()
                Dim args As New ItemSelectedEventArgs
                args.RequestId = CInt(parts(0))

                strQuery.Append("requestId=" + CType(args.RequestId, String))
                args.Type = ModuleType.SARCAppeal

                Select Case args.Type
                    Case ModuleType.SARCAppeal
                        args.Url = "~/Secure/SARC_Appeal/init.aspx?" + strQuery.ToString()
                        Response.Redirect(args.Url)

                End Select

            End If

        End Sub

        Private Sub gvResults2_RowCommand(ByVal sender As Object, ByVal e As GridViewCommandEventArgs) Handles gvResults2.RowCommand

            If (e.CommandName = "view") Then

                Dim parts() As String = e.CommandArgument.ToString().Split(";")
                Dim strQuery As New StringBuilder()
                Dim args As New ItemSelectedEventArgs
                args.RequestId = CInt(parts(0))

                strQuery.Append("requestId=" + CType(args.RequestId, String))
                args.Type = ModuleType.SARCAppeal

                Select Case args.Type
                    Case ModuleType.SARCAppeal
                        args.Url = "~/Secure/SARC_Appeal/init.aspx?" + strQuery.ToString()
                        Response.Redirect(args.Url)

                End Select

            End If

        End Sub

    End Class

End Namespace