Imports ALOD.Core.Domain.Users
Imports ALOD.Core.Domain.Workflow
Imports ALOD.Data.Services
Imports ALOD.Logging
Imports ALODWebUtility.Common
Imports ALODWebUtility.Permission

Namespace Web.Sys

    Partial Class Secure_Shared_System_UserPermissions
        Inherits System.Web.UI.Page

        Private _permList As PermissionList

        Private PAGE_MANAGE_USERS As Integer = 1
        Private PAGE_PERM_REPORT As Integer = 4

        Private ReadOnly Property Caller() As Integer
            Get
                If (Request.QueryString("caller") Is Nothing) Then
                    Return PAGE_MANAGE_USERS
                Else
                    If (Request.QueryString("caller") = "4") Then
                        Return PAGE_PERM_REPORT
                    Else
                        Return PAGE_MANAGE_USERS
                    End If
                End If
            End Get
        End Property

        Protected Sub btnUpdate_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnUpdate.Click

            'we send all of these rows as a bulk update, so iterate through our data grid
            'get all the data for each row and pass the whole list to SQL for a bulk update

            GetCurrentPermissions()

            Dim changes As New ChangeSet
            Dim change As ChangeRow
            Dim list As New PermissionList()
            Dim perm As Permission
            Dim current As Permission
            Dim permId As Short
            Dim oldStatus As String = ""

            Dim grant As RadioButton
            Dim revoke As RadioButton
            Dim name As Label

            For Each row As GridViewRow In gvPerms.Rows

                grant = CType(row.FindControl("rbGrant"), RadioButton)
                revoke = CType(row.FindControl("rbRevoke"), RadioButton)
                permId = gvPerms.DataKeys(row.RowIndex)(0)
                current = _permList.Find(permId)

                If (grant.Checked) OrElse (revoke.Checked) Then

                    perm = New Permission()
                    perm.Id = permId
                    perm.Status = IIf(grant.Checked, "G", "R")
                    list.Add(perm)

                    If (current IsNot Nothing) Then
                        oldStatus = current.Status
                    Else
                        oldStatus = ""
                    End If

                    If (oldStatus <> perm.Status) Then
                        'only log the change if it is different
                        change = New ChangeRow
                        change.Section = "Permission"
                        name = CType(row.FindControl("lblName"), Label)
                        change.Field = name.Text
                        change.NewVal = IIf(perm.Status = "G", "Grant", "Revoke")

                        If (current IsNot Nothing) Then
                            change.OldVal = IIf(current.Status = "G", "Grant", "Revoke")
                        Else
                            change.OldVal = "Default"
                        End If

                        changes.Add(change)
                    End If

                ElseIf (current IsNot Nothing) Then

                    'here the perm was set to grant/revoke but is now default
                    change = New ChangeRow
                    change.Section = "Permission"
                    name = CType(row.FindControl("lblName"), Label)
                    change.Field = name.Text
                    change.NewVal = "Default"
                    change.OldVal = IIf(current.Status = "G", "Grant", "Revoke")

                    changes.Add(change)

                End If

            Next

            Dim userId As Integer = CInt(CInt(Session("EditId")))
            list.AssignToUser(userId)

            Dim logId As Integer = LogManager.LogAction(ModuleType.System, UserAction.ModifyUserPermissions, userId)

            If (changes.Count > 0) Then
                changes.Save(logId)
            End If

            Redirect()

        End Sub

        Protected Sub Cancel_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles CancelButton.Click
            Redirect()
        End Sub

        Protected Sub gvPerms_DataBinding(ByVal sender As Object, ByVal e As System.EventArgs) Handles gvPerms.DataBinding
            GetCurrentPermissions()
        End Sub

        Protected Sub gvPerms_RowDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewRowEventArgs) Handles gvPerms.RowDataBound

            If (e.Row.RowType <> DataControlRowType.DataRow) Then
                Exit Sub
            End If

            Dim data As DataRowView = CType(e.Row.DataItem, DataRowView)
            Dim permId As Integer = CInt(data("Id"))
            Dim perm As Permission = _permList.Find(permId)

            If (perm IsNot Nothing) Then
                Dim radio As RadioButton

                If (perm.Status = "G") Then
                    radio = CType(e.Row.FindControl("rbGrant"), RadioButton)
                Else
                    radio = CType(e.Row.FindControl("rbRevoke"), RadioButton)
                End If

                radio.Checked = True
            End If

        End Sub

        Protected Sub lnkManageUsers_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles lnkManageUsers.Click, lnkPermReport.Click
            Redirect()
        End Sub

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

            If (Not IsPostBack) Then

                Dim user As AppUser = UserService.GetById(Session("EditId"))
                UserName.Text = Server.HtmlEncode(user.NameRankAndRole)

                If (Request.QueryString("caller") Is Nothing) Then
                    lnkPermReport.Visible = False
                    lnkManageUsers.Visible = True
                Else
                    If (Request.QueryString("caller") = "4") Then
                        lnkPermReport.Visible = True
                        lnkManageUsers.Visible = False
                    Else
                        lnkPermReport.Visible = False
                        lnkManageUsers.Visible = True
                    End If
                End If
            End If

        End Sub

        Protected Sub Redirect()
            Session("EditId") = Nothing

            If (Caller = PAGE_MANAGE_USERS) Then
                Response.Redirect("~/Secure/Shared/Admin/ManageUsers.aspx")
            Else
                Response.Redirect("~/Secure/Shared/Admin/PermissionReport.aspx")
            End If
        End Sub

        Private Sub GetCurrentPermissions()
            _permList = New PermissionList()
            _permList.GetUserPermissions(CInt(Session("EditId")))
        End Sub

    End Class

End Namespace