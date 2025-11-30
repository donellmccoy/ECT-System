Imports ALOD.Core.Interfaces.DAOInterfaces
Imports ALOD.Data

Namespace Web.Sys

    Partial Class Secure_Shared_System_ManagedBy
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
            'get all the data for each row and pass the whole list to the DAO for a bulk update.
            Dim data As New DataSet
            Dim table As DataTable = data.Tables.Add()
            table.Columns.Add("groupId")
            table.Columns.Add("Manages")
            table.Columns.Add("Notify")
            table.Columns.Add("ViewBy")

            Dim group As DataRow

            For Each row As GridViewRow In GroupsGrid.Rows

                group = table.NewRow()
                group("groupId") = CShort(GroupsGrid.DataKeys(row.RowIndex)(0))
                group("Manages") = IIf(CType(row.FindControl("cbManaged"), CheckBox).Checked, "1", "0")
                group("Notify") = IIf(CType(row.FindControl("cbNotify"), CheckBox).Checked, "1", "0")
                group("ViewBy") = IIf(CType(row.FindControl("cbViewBy"), CheckBox).Checked, "1", "0")

                table.Rows.Add(group)

            Next

            GroupDao.UpdateManagedBy(CInt(GroupSelect.SelectedValue), data)

        End Sub

        Protected Sub GroupSelect_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles GroupSelect.SelectedIndexChanged
            DisplayManagedGroups()
        End Sub

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
            If (Not IsPostBack) Then
                InitControls()
            End If
        End Sub

        Private Sub DisplayManagedGroups()
            Dim groupid As Integer = CInt(GroupSelect.SelectedValue)
            GroupsGrid.DataSource = GroupDao.GetAllWithManaged(groupid)
            DisplayViewByGroups()
            GroupsGrid.DataBind()
        End Sub

        Private Sub DisplayViewByGroups()
            Dim groupid As Integer = CInt(GroupSelect.SelectedValue)

        End Sub

        Private Sub InitControls()
            GroupSelect.DataSource = From g In GroupDao.GetAll() Select g Order By g.Description
            GroupSelect.DataBind()
            DisplayManagedGroups()
        End Sub

    End Class

End Namespace