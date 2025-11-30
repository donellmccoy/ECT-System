Imports ALOD.Core.Domain.Workflow
Imports ALOD.Core.Interfaces.DAOInterfaces
Imports ALOD.Data
Imports ALODWebUtility.Common
Imports ALODWebUtility.Permission.Search

Namespace Web.Special_Case.PW

    Partial Class Secure_pw_MyCases

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

        Protected Sub gvDisposition_RowCommand(ByVal sender As Object, ByVal e As GridViewCommandEventArgs) Handles gvDisposition.RowCommand

            If (e.CommandName = "view") Then

                Dim parts() As String = e.CommandArgument.ToString().Split(";")
                Session("RefId") = parts(0)
                Response.Redirect(GetWorkflowInitPageURL(parts(1), parts(0)))

            End If

        End Sub

        Protected Sub gvDisposition_RowDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewRowEventArgs) Handles gvDisposition.RowDataBound
            HeaderRowBinding(sender, e, "CaseId")

            If (e.Row.RowType = DataControlRowType.DataRow) Then
                Dim data As DataRowView = CType(e.Row.DataItem, DataRowView)
                Dim lockId As Integer = 0

                Integer.TryParse(data("lockId"), lockId)

                If (lockId > 0) Then
                    CType(e.Row.FindControl("LockImage"), Image).Visible = True
                End If
            End If

        End Sub

        Protected Sub gvHolding_RowDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewRowEventArgs) Handles gvHolding.RowDataBound
            HeaderRowBinding(sender, e, "CaseId")

            If (e.Row.RowType = DataControlRowType.DataRow) Then
                Dim data As DataRowView = CType(e.Row.DataItem, DataRowView)
                Dim lockId As Integer = 0

                Integer.TryParse(data("lockId"), lockId)

                If (lockId > 0) Then
                    CType(e.Row.FindControl("LockImage"), Image).Visible = True
                End If
            End If

        End Sub

        Protected Sub gvResults10_RowDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewRowEventArgs) Handles gvResults10.RowDataBound
            HeaderRowBinding(sender, e, "CaseId")

            If (e.Row.RowType = DataControlRowType.DataRow) Then
                Dim data As DataRowView = CType(e.Row.DataItem, DataRowView)
                Dim lockId As Integer = 0

                Integer.TryParse(data("lockId"), lockId)

                If (lockId > 0) Then
                    CType(e.Row.FindControl("LockImage"), Image).Visible = True
                End If
            End If

        End Sub

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

            If (Not IsPostBack) Then
                'clear any pending locks this user might have

                Dim dao As ALOD.Core.Interfaces.DAOInterfaces.ICaseLockDao = New ALOD.Data.NHibernateDaoFactory().GetCaseLockDao()

            End If

        End Sub

        Protected Sub SCData_Module10_Selecting(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.ObjectDataSourceSelectingEventArgs) Handles SCData_Module10.Selecting

            e.InputParameters("moduleId") = ModuleType.SpecCasePW
            e.InputParameters("userId") = SESSION_USER_ID

        End Sub

        Private Sub gvHolding_RowCommand(ByVal sender As Object, ByVal e As GridViewCommandEventArgs) Handles gvHolding.RowCommand

            If (e.CommandName = "view") Then

                Dim parts() As String = e.CommandArgument.ToString().Split(";")
                Dim strQuery As New StringBuilder()
                Dim args As New ItemSelectedEventArgs
                args.RefId = CInt(parts(0))

                strQuery.Append("refId=" + CType(args.RefId, String))
                args.Type = CInt(parts(1))

                Select Case args.Type
                    Case ModuleType.SpecCaseBCMR
                        args.Url = "~/Secure/SC_BCMR/init.aspx?" + strQuery.ToString()
                        Response.Redirect(args.Url)
                    Case ModuleType.SpecCaseBMT
                        args.Url = "~/Secure/SC_BMT/init.aspx?" + strQuery.ToString()
                        Response.Redirect(args.Url)
                    Case ModuleType.SpecCaseCMAS
                        args.Url = "~/Secure/SC_CMAS/init.aspx?" + strQuery.ToString()
                        Response.Redirect(args.Url)
                    Case ModuleType.SpecCaseCongress
                        args.Url = "~/Secure/SC_Congress/init.aspx?" + strQuery.ToString()
                        Response.Redirect(args.Url)
                    Case ModuleType.SpecCaseFT
                        args.Url = "~/Secure/SC_FastTrack/init.aspx?" + strQuery.ToString()
                        Response.Redirect(args.Url)
                    Case ModuleType.SpecCaseIncap
                        args.Url = "~/Secure/SC_Incap/init.aspx?" + strQuery.ToString()
                        Response.Redirect(args.Url)
                    Case ModuleType.SpecCaseMEB
                        args.Url = "~/Secure/SC_MEB/init.aspx?" + strQuery.ToString()
                        Response.Redirect(args.Url)
                    Case ModuleType.SpecCaseMEPS
                        args.Url = "~/Secure/SC_MEPS/init.aspx?" + strQuery.ToString()
                        Response.Redirect(args.Url)
                    Case ModuleType.SpecCasePW
                        args.Url = "~/Secure/SC_PWaivers/init.aspx?" + strQuery.ToString()
                        Response.Redirect(args.Url)
                    Case ModuleType.SpecCaseWWD
                        args.Url = "~/Secure/SC_WWD/init.aspx?" + strQuery.ToString()
                        Response.Redirect(args.Url)
                    Case ModuleType.SpecCaseMH
                        args.Url = "~/Secure/SC_MH/init.aspx?" + strQuery.ToString()
                        Response.Redirect(args.Url)
                End Select

            End If

        End Sub

        Private Sub gvResults10_RowCommand(ByVal sender As Object, ByVal e As GridViewCommandEventArgs) Handles gvResults10.RowCommand

            If (e.CommandName = "view") Then

                Dim parts() As String = e.CommandArgument.ToString().Split(";")
                Dim strQuery As New StringBuilder()
                Dim args As New ItemSelectedEventArgs
                args.RefId = CInt(parts(0))

                strQuery.Append("refId=" + CType(args.RefId, String))
                args.Type = CInt(parts(1))

                Select Case args.Type
                    Case ModuleType.SpecCaseBCMR
                        args.Url = "~/Secure/SC_BCMR/init.aspx?" + strQuery.ToString()
                        Response.Redirect(args.Url)
                    Case ModuleType.SpecCaseBMT
                        args.Url = "~/Secure/SC_BMT/init.aspx?" + strQuery.ToString()
                        Response.Redirect(args.Url)
                    Case ModuleType.SpecCaseCMAS
                        args.Url = "~/Secure/SC_CMAS/init.aspx?" + strQuery.ToString()
                        Response.Redirect(args.Url)
                    Case ModuleType.SpecCaseCongress
                        args.Url = "~/Secure/SC_Congress/init.aspx?" + strQuery.ToString()
                        Response.Redirect(args.Url)
                    Case ModuleType.SpecCaseFT
                        args.Url = "~/Secure/SC_FastTrack/init.aspx?" + strQuery.ToString()
                        Response.Redirect(args.Url)
                    Case ModuleType.SpecCaseIncap
                        args.Url = "~/Secure/SC_Incap/init.aspx?" + strQuery.ToString()
                        Response.Redirect(args.Url)
                    Case ModuleType.SpecCaseMEB
                        args.Url = "~/Secure/SC_MEB/init.aspx?" + strQuery.ToString()
                        Response.Redirect(args.Url)
                    Case ModuleType.SpecCaseMEPS
                        args.Url = "~/Secure/SC_MEPS/init.aspx?" + strQuery.ToString()
                        Response.Redirect(args.Url)
                    Case ModuleType.SpecCasePW
                        args.Url = "~/Secure/SC_PWaivers/init.aspx?" + strQuery.ToString()
                        Response.Redirect(args.Url)
                    Case ModuleType.SpecCaseWWD
                        args.Url = "~/Secure/SC_WWD/init.aspx?" + strQuery.ToString()
                        Response.Redirect(args.Url)
                End Select

            End If

        End Sub

    End Class

End Namespace