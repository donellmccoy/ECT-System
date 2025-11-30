Imports ALOD.Core.Domain.Workflow
Imports ALOD.Core.Interfaces.DAOInterfaces
Imports ALOD.Data

Namespace Web.Admin

    Partial Class Secure_Shared_Admin_CaseLocks
        Inherits System.Web.UI.Page

        Protected Sub LockGrid_RowCommand(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewCommandEventArgs) Handles LockGrid.RowCommand

            If (e.CommandName = "DeleteLock") Then

                Dim lockId As Integer = 0
                Integer.TryParse(e.CommandArgument, lockId)

                If (lockId > 0) Then
                    Dim dao As ICaseLockDao = New NHibernateDaoFactory().GetCaseLockDao()
                    Dim lock As CaseLock = dao.GetById(lockId)
                    dao.Delete(lock)
                    dao.CommitChanges()
                    UpdateCaseGrid()
                End If

            End If

        End Sub

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
            If (Not IsPostBack) Then
                UpdateCaseGrid()
            End If
        End Sub

        Protected Sub UpdateCaseGrid()

            Dim dao As ICaseLockDao = New NHibernateDaoFactory().GetCaseLockDao()
            Dim data As DataSet = dao.GetAll()

            LockGrid.DataSource = data
            LockGrid.DataBind()

        End Sub

    End Class

End Namespace