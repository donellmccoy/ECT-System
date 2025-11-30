Namespace Web.Admin

    Partial Class Secure_Shared_Admin_AppAuth
        Inherits System.Web.UI.Page

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
            odsUsersTitle.SelectParameters("groupId").DefaultValue = ALOD.Core.Domain.Users.UserGroups.BoardApprovalAuthority
            odsUsersTitle.UpdateParameters("groupId").DefaultValue = ALOD.Core.Domain.Users.UserGroups.BoardApprovalAuthority
        End Sub

    End Class

End Namespace