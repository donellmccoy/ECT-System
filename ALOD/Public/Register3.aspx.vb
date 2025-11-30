Imports ALOD.Core.Domain.Users
Imports ALOD.Core.Interfaces.DAOInterfaces
Imports ALOD.Data
Imports ALOD.Data.Services
Imports ALODWebUtility.Common

Namespace Web

    Partial Class Public_Register3
        Inherits System.Web.UI.Page

        Protected _edipin As String = ""

        Protected Sub NextButton_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles NextButton.Click

            Dim dao As IUserDao = New NHibernateDaoFactory().GetUserDao()
            Dim user As AppUser = dao.GetByEDIPIN(_edipin)

            If (user Is Nothing) Then
                Exit Sub
            End If

            'set user status to pending after user confirms information
            user.Status = 2
            dao.SaveOrUpdate(user)
            UserService.SendAccountRegisteredEmail(user.Component, user.Username, user.CurrentRoleName, user.CurrentUnitId, GetHostName())
            Response.Redirect("~/public/register4.aspx", True)

        End Sub

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
            VerifyAccess()
            populatedata()
        End Sub

        Protected Sub populatedata()

            Dim user As AppUser = UserService.GetByEDIPIN(_edipin)

            FullName.Text = user.FullName
            RankDisplay.Text = user.Rank.Title
            UnitLabel.Text = user.Unit.Name + " (" + user.Unit.PasCode + ")"

            If (user.AllRoles.Count > 1) Then
                For Each role As ALOD.Core.Domain.Users.UserRole In user.AllRoles
                    UserRoleLabel.Text = role.Group.Description + ", " + UserRoleLabel.Text
                Next
            Else
                If (user.AllRoles.Count > 0) Then
                    UserRoleLabel.Text = user.AllRoles(0).Group.Description
                End If
            End If

            ' UserRoleLabel.Text = user.CurrentRole

            EmailLabel.Text = user.Email
            Email2Label.Text = user.Email2
            Email3Label.Text = user.Email3
            PhoneLabel.Text = user.Phone
            DsnLabel.Text = user.DSN

            AddressLabel.Text = user.Address.Street
            CityLabel.Text = user.Address.City
            StateLabel.Text = user.Address.State
            ZipLabel.Text = user.Address.Zip

            If (user.ReceiveEmail) Then
                RecieveEmailLabel.Text = "Yes"
            Else
                RecieveEmailLabel.Text = "No"
            End If

        End Sub

        Protected Sub PrevButton_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles PrevButton.Click
            Response.Redirect("~/Public/Register2.aspx", True)
        End Sub

        Protected Sub VerifyAccess()

            If (SESSION_EDIPIN Is Nothing OrElse Session("signed") Is Nothing) Then

                '  If (Not query.Contains("ssn") OrElse Not query.Contains("signed")) Then
                'no ssn on so this is not a legit request
                Response.Redirect("~/Default.aspx")
            End If

            _edipin = SESSION_EDIPIN

        End Sub

    End Class

End Namespace