Imports ALOD.Core.Domain.Workflow
Imports ALOD.Core.Interfaces.DAOInterfaces
Imports ALOD.Data
Imports ALODWebUtility.Common

Namespace Web.Special_Case.BCMR

    Partial Class Secure_bcmr_MyCases
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

        Protected Sub gvResults12_RowDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewRowEventArgs) Handles gvResults12.RowDataBound
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

                Dim dao As ICaseLockDao = New ALOD.Data.NHibernateDaoFactory().GetCaseLockDao()
                dao.ClearLocksForUser(SESSION_USER_ID)

            End If

        End Sub

        Protected Sub SCData_Module12_Selecting(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.ObjectDataSourceSelectingEventArgs) Handles SCData_Module12.Selecting

            e.InputParameters("moduleId") = ModuleType.SpecCaseBCMR

        End Sub

        Private Sub gvHolding_RowCommand(ByVal sender As Object, ByVal e As GridViewCommandEventArgs) Handles gvResults12.RowCommand

            If (e.CommandName = "view") Then

                Dim parts() As String = e.CommandArgument.ToString().Split(";")

                Response.Redirect(GetWorkflowInitPageURL(CInt(parts(1)), CInt(parts(0))))

            End If

        End Sub

        Private Sub gvResults12_RowCommand(ByVal sender As Object, ByVal e As GridViewCommandEventArgs) Handles gvResults12.RowCommand

            If (e.CommandName = "view") Then

                Dim parts() As String = e.CommandArgument.ToString().Split(";")

                Response.Redirect(GetWorkflowInitPageURL(CInt(parts(1)), CInt(parts(0))))

            End If

        End Sub

    End Class

End Namespace