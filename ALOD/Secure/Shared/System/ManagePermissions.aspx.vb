Imports ALOD.Core.Domain.Workflow
Imports ALOD.Core.Interfaces.DAOInterfaces
Imports ALOD.Data
Imports ALOD.Logging
Imports ALODWebUtility.Permission

Namespace Web.Sys

    Partial Class Secure_Shared_System_ManagePermissions
        Inherits System.Web.UI.Page

        Private groupSource As IUserGroupDao

        Private ReadOnly Property GroupDao() As IUserGroupDao
            Get
                If (groupSource Is Nothing) Then
                    groupSource = New NHibernateDaoFactory().GetUserGroupDao()
                End If
                Return groupSource
            End Get
        End Property

        Protected Sub btnUpdate_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnUpdate.Click

            'we send all of these rows as a bulk update, so iterate through our data grid
            'get all the data for each row and pass the whole list to SQL for a bulk update
            Dim list As New PermissionList()
            Dim perm As Permission

            For Each row As GridViewRow In PermissionGrid.Rows

                If CType(row.FindControl("cbPerms"), CheckBox).Checked Then
                    perm = New Permission()
                    perm.Id = CShort(PermissionGrid.DataKeys(row.RowIndex)(0))
                    list.Add(perm)
                End If

            Next

            Dim groupId As Short = CShort(GroupSelect.SelectedValue)
            list.AssignToGroup(groupId)
            LogManager.LogAction(ModuleType.System, UserAction.ModifyGroupPermissions, groupId)
        End Sub

        Protected Sub GroupSelect_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles GroupSelect.SelectedIndexChanged
            PermissionGrid.DataBind()
        End Sub

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
            If (Not IsPostBack) Then
                InitControls()
            End If
        End Sub

        Private Sub InitControls()
            GroupSelect.DataSource = From g In GroupDao.GetAll() Select g Order By g.Description
            GroupSelect.DataBind()
        End Sub

    End Class

End Namespace