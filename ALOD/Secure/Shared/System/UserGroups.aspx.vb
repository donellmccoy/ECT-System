Imports System.Linq.Dynamic
Imports ALOD.Core.Domain.Users
Imports ALOD.Core.Interfaces.DAOInterfaces
Imports ALOD.Core.Utils
Imports ALOD.Data
Imports ALOD.Data.Services
Imports ALODWebUtility.Common

Namespace Web.Sys

    Partial Class Secure_Shared_System_UserGroups
        Inherits System.Web.UI.Page

        Private _daoFactory As IDaoFactory
        Private _userGroupDao As IUserGroupDao
        Private _userGroupLevelDao As IUserGroupLevelDao

        Protected ReadOnly Property DaoFactory As IDaoFactory
            Get
                If (_daoFactory Is Nothing) Then
                    _daoFactory = New NHibernateDaoFactory
                End If

                Return _daoFactory
            End Get
        End Property

        Protected Property SortBy() As Boolean
            Get
                Return CBool(ViewState("Sort"))
            End Get
            Set(ByVal value As Boolean)
                If value Then
                    ViewState("Sort") = False
                Else
                    ViewState("Sort") = True
                End If
            End Set
        End Property

        Protected Property SortField() As String
            Get
                Return ViewState("SortField")
            End Get
            Set(ByVal value As String)
                ViewState("SortField") = value
            End Set
        End Property

        Protected ReadOnly Property UserGroupDao As IUserGroupDao
            Get
                If (_userGroupDao Is Nothing) Then
                    _userGroupDao = DaoFactory.GetUserGroupDao()
                End If

                Return _userGroupDao
            End Get
        End Property

        Protected ReadOnly Property UserGroupLevelDao As IUserGroupLevelDao
            Get
                If (_userGroupLevelDao Is Nothing) Then
                    _userGroupLevelDao = DaoFactory.GetUserGroupLevelDao()
                End If

                Return _userGroupLevelDao
            End Get
        End Property

        Protected Sub cmdSubmit_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles cmdSubmit.Click
            Dim group As New UserGroup

            group.Description = txtDescription.Text.Trim
            group.Abbreviation = txtAbbreviation.Text.Trim
            group.Component = cbCompo.SelectedValue
            group.SortOrder = txtSortOrder.Text.Trim
            group.Scope = cbScope.SelectedValue
            group.ReportView = ReportViewList.SelectedValue
            group.GroupLevel = UserGroupLevelDao.GetById(ddlUserGroupLevels.SelectedValue)
            UserGroupDao.Save(group)

            PopulateRoles()
        End Sub

        Protected Sub GroupsView_RowCancelingEdit(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewCancelEditEventArgs) Handles GroupsView.RowCancelingEdit
            GroupsView.EditIndex = -1
            PopulateRoles()
        End Sub

        Protected Sub GroupsView_RowDeleting(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewDeleteEventArgs) Handles GroupsView.RowDeleting
            Dim group As UserGroup

            group = UserGroupDao.GetById(GroupsView.DataKeys(e.RowIndex)("Id"))
            UserGroupDao.Delete(group)
            PopulateRoles()
        End Sub

        Protected Sub GroupsView_RowEditing(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewEditEventArgs) Handles GroupsView.RowEditing
            GroupsView.EditIndex = e.NewEditIndex
            Dim groups As IList(Of UserGroup) = LookupService.GetGroupsByCompo(HttpContext.Current.Session("Compo"))

            GroupsView.DataSource = From g In groups Order By g.SortOrder Select g
            GroupsView.DataBind()

            Dim ddlGridViewUserGroupLevels As DropDownList = CType(GroupsView.Rows(e.NewEditIndex).FindControl("ddlGridViewUserGroupLevels"), DropDownList)

            InitUserGroupLevelDropdownList(ddlGridViewUserGroupLevels)

            CType(GroupsView.Rows(e.NewEditIndex).FindControl("cbScope"), DropDownList).SelectedValue = groups.Item(GroupsView.EditIndex).Scope
            CType(GroupsView.Rows(e.NewEditIndex).FindControl("ReportViewList"), DropDownList).SelectedValue = groups.Item(GroupsView.EditIndex).ReportView
            ddlGridViewUserGroupLevels.SelectedValue = groups.Item(GroupsView.EditIndex).GroupLevel.Id
        End Sub

        Protected Sub GroupsView_RowUpdating(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewUpdateEventArgs) Handles GroupsView.RowUpdating
            Dim group As UserGroup
            Dim row As GridViewRow = GroupsView.Rows(e.RowIndex)

            group = UserGroupDao.GetById(GroupsView.DataKeys(e.RowIndex)("Id"))
            group.Description = CType(row.FindControl("txtDescription"), TextBox).Text
            group.Abbreviation = CType(row.FindControl("txtAbbreviation"), TextBox).Text
            group.Component = CType(row.FindControl("cbCompo"), DropDownList).SelectedValue
            group.SortOrder = CType(row.FindControl("txtSortOrder"), TextBox).Text
            group.Scope = CType(row.FindControl("cbScope"), DropDownList).SelectedValue
            group.ReportView = CType(row.FindControl("ReportViewList"), DropDownList).SelectedValue
            group.GroupLevel = UserGroupLevelDao.GetById(CType(row.FindControl("ddlGridViewUserGroupLevels"), DropDownList).SelectedValue)

            UserGroupDao.SaveOrUpdate(group)
            UserGroupDao.CommitChanges()
            UserGroupDao.Evict(group)

            GroupsView.EditIndex = -1
            PopulateRoles()
        End Sub

        Protected Sub GroupsView_Sorting(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewSortEventArgs) Handles GroupsView.Sorting
            SortField = e.SortExpression
            PopulateRoles(True)
        End Sub

        Protected Sub InitControls()
            PopulateRoles()
            InitUserGroupLevelDropdownList(ddlUserGroupLevels)
        End Sub

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
            SetInputFormatRestriction(Page, txtSortOrder, FormatRestriction.Numeric)

            If Not IsPostBack Then
                InitControls()
            End If
        End Sub

        Protected Sub PopulateRoles(Optional ByVal isEdit As Boolean = False)
            UpdateSortProperties(isEdit)

            GroupsView.DataSource = LookupService.GetGroupsByCompo(Session("Compo")).AsQueryable().OrderBy(SortField)
            'GroupsView.DataSource = LookupService.GetGroupsByCompo(Session("Scope")).AsQueryable().OrderBy(SortField)
            GroupsView.DataBind()
        End Sub

        Private Sub InitUserGroupLevelDropdownList(ddl As DropDownList)
            ddl.DataSource = UserGroupLevelDao.GetAll()
            ddl.DataValueField = "Id"
            ddl.DataTextField = "Name"
            ddl.DataBind()
        End Sub

        Private Sub UpdateSortProperties(Optional ByVal isEdit As Boolean = False)
            If (SortField = String.Empty) Then
                SortField = "SortOrder"
            Else
                If (isEdit) Then
                    If (SortBy) Then
                        SortField = SortField + " DESC"
                        SortBy = True
                    Else
                        SortBy = False
                    End If
                End If
            End If
        End Sub

    End Class

End Namespace