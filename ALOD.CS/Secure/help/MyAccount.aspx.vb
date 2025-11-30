Imports ALOD.Core.Domain.Lookup
Imports ALOD.Core.Domain.Users
Imports ALOD.Core.Domain.Workflow
Imports ALOD.Core.Interfaces.DAOInterfaces
Imports ALOD.Core.Utils
Imports ALOD.Core.Utils.RegexValidation
Imports ALOD.Data
Imports ALOD.Data.Services
Imports ALOD.Logging
Imports ALODWebUtility.Common

Namespace Web.Help

    Partial Class Secure_Shared_Admin_MyAccount
        Inherits System.Web.UI.Page
        Dim hide As String = "hidden"

#Region "ValueCheck"

        Private errMsg As String
        Dim newControlList As lodControlList
        Dim oldControlList As lodControlList

        Public Property InitialHash() As String
            Get
                Return hdnInitialHash.Value

            End Get
            Set(ByVal Value As String)
                hdnInitialHash.Value = Value
            End Set
        End Property

        Public ReadOnly Property MyControls() As System.Web.UI.ControlCollection
            Get
                Return Page.Master.FindControl("ContentMain").FindControl("ControlsPanel").Controls

            End Get

        End Property

        Public Property OldList() As lodControlList
            Get
                Dim crtlList As lodControlList = New lodControlList()
                If (hdnOldControlList.Value <> "") Then
                    crtlList.Read(hdnOldControlList.Value)
                End If
                Return crtlList
            End Get
            Set(ByVal Value As lodControlList)

                hdnOldControlList.Value = Value.Write()
            End Set
        End Property

        'This is to log the change set
        Public Sub ApplySections()

            PhoneBox.Attributes.Add("Section", "User Account Settings")
            ReceiveEmailCheck.Attributes.Add("Section", "User Account Settings")
            ReceiveReminderEmailCheck.Attributes.Add("Section", "User Account Settings")
            DsnBox.Attributes.Add("Section", "User Account Settings")
            EmailBox.Attributes.Add("Section", "User Account Settings")
            StreetBox.Attributes.Add("Section", "User Account Settings")
            CityBox.Attributes.Add("Section", "User Account Settings")
            StateSelect.Attributes.Add("Section", "User Account Settings")
            ZipBox.Attributes.Add("Section", "User Account Settings")

            PhoneBox.Attributes.Add("Field", "Comm Phone")
            ReceiveEmailCheck.Attributes.Add("Field", "Recieve Email")
            ReceiveReminderEmailCheck.Attributes.Add("Field", "Recieve Email")
            DsnBox.Attributes.Add("Field", "Dsn Phone")
            EmailBox.Attributes.Add("Field", "Email")
            Email2Box.Attributes.Add("Field", "Addiotinal Email(1)")
            Email3Box.Attributes.Add("Field", "Addiotinal Email(2)")
            StreetBox.Attributes.Add("Field", "Street")
            CityBox.Attributes.Add("Field", "City")
            StateSelect.Attributes.Add("Field", "Work Region/State")
            ZipBox.Attributes.Add("Field", "User Account Summery")

        End Sub

        Public Sub LogChanges(ByVal refId As Integer, ByVal status As Byte)
            Try
                '********************************************
                'Code for Change Set
                If (ControlValues.HasChanged(MyControls, InitialHash)) Then
                    newControlList = New lodControlList
                    newControlList.Create(MyControls)
                    oldControlList = OldList
                    If (oldControlList.LogChanges(newControlList, ModuleType.System, UserAction.ChangedFormData, refId, "Modified (User Account Form)", status)) Then
                    End If
                End If
            Catch
            Finally
                CleanHashValues()
            End Try
            '********************************************
        End Sub

        Protected Sub Page_SaveStateComplete(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.SaveStateComplete
            Try
                If (Not (IsPostBack)) Then
                    InitialHash = ControlValues.GetInitialHash(MyControls).ToString() 'Stores the initial Hash Value in the session state
                    oldControlList = New lodControlList
                    oldControlList.Create(MyControls)  'Stores the initial values for controls the session state
                    OldList = oldControlList  'Stores the initial values for controls the session state
                End If
            Catch
            Finally

            End Try
        End Sub

#End Region

        Public Sub InitControls()

            SetMaxLength(RoleChangeComments)

            ChangeUnitButton.Attributes.Add("onclick", "showSearcher('" + "Select New Unit" + "','" + newUnitIDLabel.ClientID + "','" + UnitTextBox.ClientID + "'); return false;")

            SetInputFormatRestriction(Page, RoleChangeComments, FormatRestriction.AlphaNumeric, PERMITTED_SPECIAL_CHAR_INPUT)
            SetInputFormatRestriction(Page, PhoneBox, FormatRestriction.Numeric, "-")
            SetInputFormatRestriction(Page, DsnBox, FormatRestriction.Numeric)
            SetInputFormatRestriction(Page, EmailBox, FormatRestriction.AlphaNumeric, PERMITTED_SPECIAL_CHAR_INPUT)
            SetInputFormatRestriction(Page, Email2Box, FormatRestriction.AlphaNumeric, PERMITTED_SPECIAL_CHAR_INPUT)
            SetInputFormatRestriction(Page, Email3Box, FormatRestriction.AlphaNumeric, PERMITTED_SPECIAL_CHAR_INPUT)
            SetInputFormatRestriction(Page, StreetBox, FormatRestriction.AlphaNumeric, PERMITTED_SPECIAL_CHAR_INPUT)
            SetInputFormatRestriction(Page, CityBox, FormatRestriction.AlphaNumeric, PERMITTED_SPECIAL_CHAR_INPUT)
            SetInputFormatRestriction(Page, ZipBox, FormatRestriction.Numeric, "-")
            SetInputFormatRestriction(Page, UnitTextBox, FormatRestriction.AlphaNumeric, PERMITTED_SPECIAL_CHAR_INPUT)

            Dim user As AppUser = New NHibernateDaoFactory().GetUserDao().FindById(CInt(Session("UserId")))
            If (user Is Nothing) Then
                Response.Redirect(Resources._Global.StartPage, True)
            End If

            'account status
            UserNameLabel.Text = user.Username
            NameLabel.Text = user.FullName
            StatusLabel.Text = user.Status.ToString()
            RoleLabel.Text = user.CurrentRole.Group.Description
            If (user.AccountExpiration.HasValue) Then
                ExpirationDateLabel.Text = user.AccountExpiration.Value.ToString(DATE_FORMAT)
            End If

            If (user.UnitView = 1) Then
                chkSubUnitView.Checked = True
            Else
                chkSubUnitView.Checked = False
            End If

            UpdateUserRoleDisplay(user.Id)

            'Role information if there are more then one roles for the user
            If user.AllRoles.Count > 1 Then
                CurrentRoleSelect.Visible = True
                ChangeRoleButton.Visible = True
                CurrentRoleSelect.DataSource = From t In user.AllRoles
                                               Order By t.Group.Description
                                               Select t.Id, t.Group.Description
                CurrentRoleSelect.DataBind()
                SetDropdownByValue(CurrentRoleSelect, user.CurrentRole.Group.Id)
            Else
                CurrentRoleLabel.Text = user.CurrentRole.Group.Description
                CurrentRoleSelect.Visible = False
                ChangeRoleButton.Visible = False
            End If

            'user information
            ReceiveEmailCheck.Checked = user.ReceiveEmail
            ReceiveReminderEmailCheck.Checked = user.ReceiveReminderEmail

            If (user.SSN IsNot Nothing) AndAlso (user.SSN.Length = 9) Then
                SsnLabel.Text = user.SSN.Substring(user.SSN.Length - 4, 4)
            End If

            RankLabel.Text = user.Rank.Title + "  (" + user.Rank.Grade + ")"
            PhoneBox.Text = Server.HtmlDecode(user.Phone)
            DsnBox.Text = Server.HtmlDecode(user.DSN)
            EmailBox.Text = Server.HtmlDecode(user.Email)
            Email2Box.Text = Server.HtmlDecode(user.Email2)
            Email3Box.Text = Server.HtmlDecode(user.Email3)

            If (user.Address IsNot Nothing) Then
                StreetBox.Text = Server.HtmlDecode(user.Address.Street)
                CityBox.Text = Server.HtmlDecode(user.Address.City)
                SetDropdownByValue(StateSelect, user.Address.State)
                ZipBox.Text = Server.HtmlDecode(user.Address.Zip)
            End If

            If (CanChangeUnit(user)) Then
                UnitLabel.Visible = False
                UnitTextBox.Visible = True
                ChangeUnitButton.Visible = True
                ChangeUnitButton.Enabled = True

                UnitLabel.Text = String.Empty
                UnitTextBox.Text = Server.HtmlDecode(user.Unit.Name + " (" + user.Unit.PasCode + ")")
            Else
                UnitLabel.Visible = True
                UnitTextBox.Visible = False
                ChangeUnitButton.Visible = False
                ChangeUnitButton.Enabled = False

                UnitLabel.Text = user.Unit.Name + " (" + user.Unit.PasCode + ")"
                UnitTextBox.Text = String.Empty
            End If

        End Sub

        Public Sub SaveUser()

            Dim user As AppUser = UserService.CurrentUser

            If (user Is Nothing) Then
                Response.Redirect(Resources._Global.StartPage, True)
            End If

            If Page.IsValid() Then
                user.ReceiveEmail = ReceiveEmailCheck.Checked
                user.ReceiveReminderEmail = ReceiveReminderEmailCheck.Checked
                user.Phone = Server.HtmlEncode(PhoneBox.Text.Trim)
                user.DSN = Server.HtmlEncode(DsnBox.Text.Trim)
                user.Email = Server.HtmlEncode(EmailBox.Text.Trim)
                user.Email2 = Server.HtmlEncode(Email2Box.Text.Trim)
                user.Email3 = Server.HtmlEncode(Email3Box.Text.Trim)

                If (user.Address Is Nothing) Then
                    user.Address = New Address()
                End If

                user.Address.Street = Server.HtmlEncode(StreetBox.Text.Trim)
                user.Address.City = Server.HtmlEncode(CityBox.Text.Trim)
                user.Address.State = StateSelect.SelectedValue.Trim
                user.Address.Zip = Server.HtmlEncode(ZipBox.Text.Trim)

                If (CanChangeUnit(user)) Then
                    Dim unitId As Integer = -1
                    Integer.TryParse(newUnitIDLabel.Value.Trim, unitId)

                    If newUnitIDLabel.Value.Trim <> "" Then
                        Dim newUnit As Unit = New NHibernateDaoFactory().GetUnitDao().FindById(unitId)
                        If (newUnit IsNot Nothing) AndAlso (newUnit.Id <> user.Unit.Id) Then
                            user.Unit = newUnit
                            UnitTextBox.Text = newUnit.Name + " (" + newUnit.PasCode + ")"
                        End If
                    End If
                End If

                LogChanges(user.Id, user.Status)
            End If

        End Sub

        Protected Sub btnAccountStatusUpdate_Click(sender As Object, e As System.EventArgs) Handles btnAccountStatusUpdate.Click
            Dim user As AppUser = UserService.CurrentUser

            If (user Is Nothing) Then
                Response.Redirect(Resources._Global.StartPage, True)
            End If

            If (chkSubUnitView.Checked = True) Then
                user.UnitView = 1
            Else
                user.UnitView = 0
            End If
        End Sub

        Protected Sub ChangeRoleButton_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles ChangeRoleButton.Click

            Dim factory As New NHibernateDaoFactory()
            Dim userDao As IUserDao = factory.GetUserDao()
            Dim user As AppUser = userDao.GetById(CInt(Session("UserId")))

            Dim roleId As Integer = CInt(CurrentRoleSelect.SelectedValue)

            If (user.CurrentRole.Id = roleId) Then
                'no change
                Exit Sub
            End If

            Dim role As UserRole = factory.GetUserRoleDao().GetById(roleId)

            For Each current As UserRole In user.AllRoles
                current.Active = False
            Next

            role.Active = True
            user.CurrentRole = role

            userDao.SaveOrUpdate(user)
            userDao.CommitChanges()

            If (Not SetLogin(user)) Then
                ' User is already logged into a different session; SetLogin has done a redirect to the
                'AltSession page therefore we need to exit the subprocedure in order to not override that call
                Exit Sub
            End If

            'since we just changed role, we need to reload everything
            Response.Redirect(Request.RawUrl)

        End Sub

        Protected Sub InitLookups()
            StateSelect.DataSource = LookupService.GetStates()
            StateSelect.DataTextField = "Name"
            StateSelect.DataValueField = "Id"
            StateSelect.DataBind()
        End Sub

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

            If (Session("UserId") Is Nothing) Then
                Response.Redirect(Resources._Global.StartPage, True)
            End If

            If (Not IsPostBack) Then
                ApplySections()
                InitLookups()
                InitControls()
            End If
            PendingGrid.Visible = False

        End Sub

        Protected Sub PendingGrid_RowCommand(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewCommandEventArgs) Handles PendingGrid.RowCommand

            If (e.CommandName = "DeleteRequest") Then
                Dim id As Integer = 0
                Integer.TryParse(e.CommandArgument, id)

                If (id = 0) Then
                    Exit Sub
                End If

                Dim dao As IUserRoleRequestDao = New NHibernateDaoFactory().GetUserRoleRequestDao()
                Dim request As UserRoleRequest = dao.GetById(id)
                dao.Delete(request)
                dao.CommitChanges()
                UpdateUserRoleDisplay(CInt(Session("UserId")))

            End If

        End Sub

        Protected Sub RequestRoleButton_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles RequestRoleButton.Click

            Dim request As New UserRoleRequest
            Dim user As AppUser = UserService.CurrentUser()
            Dim group As UserGroup = New UserGroup()

            Dim factory As New NHibernateDaoFactory()
            Dim groupDao As IUserGroupDao = factory.GetUserGroupDao()
            group = groupDao.GetById(CInt(ChangeRoleSelect.SelectedValue))

            request.User = user
            request.RequestedGroup = group

            If (ChangeRole.Checked) Then
                'they are requesting a change to their current role
                request.CurrentGroup = user.CurrentRole.Group
                request.IsNewRole = False
            Else
                request.IsNewRole = True
            End If

            request.RequestorComments = Server.HtmlEncode(RoleChangeComments.Text.Trim)
            request.DateRequested = DateTime.Now
            request.Status = AccessStatus.Pending

            Dim roleDao As IUserRoleRequestDao = factory.GetUserRoleRequestDao()
            roleDao.Save(request)
            UpdateUserRoleDisplay(user.Id)
            UserService.SendModifyRequestEmail(user.Component, user.Username, user.CurrentUnitId, GetHostName())

        End Sub

        Protected Sub UpdateButton_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles updateButton.Click

            Dim passed As Boolean = True
            If (Not IsValidEmail(EmailBox.Text.Trim)) Then
                EmailBox.CssClass = "fieldRequired"
                passed = False
            Else
                EmailBox.CssClass = ""
            End If

            If (Email2Box.Text.Trim.Length > 0) AndAlso (Not IsValidEmail(Email2Box.Text.Trim)) Then
                Email2Box.CssClass = "fieldRequired"
                passed = False
            Else
                Email2Box.CssClass = ""
            End If

            If (Email3Box.Text.Trim.Length > 0) AndAlso (Not IsValidEmail(Email3Box.Text.Trim)) Then
                Email3Box.CssClass = "fieldRequired"
                passed = False
            Else
                Email3Box.CssClass = ""
            End If

            If (Not IsValidPhoneNumber(PhoneBox.Text.Trim)) Then
                PhoneBox.CssClass = "fieldRequired"
                passed = False
            Else
                PhoneBox.CssClass = ""
            End If

            If (Not passed) Then
                Exit Sub
            End If

            If Page.IsValid() Then
                SaveUser()
            End If

        End Sub

        Private Function CanChangeUnit(ByVal appUser As AppUser) As Boolean

            If (appUser Is Nothing) Then
                Return False
            End If

            If (String.IsNullOrEmpty(appUser.EDIPIN)) Then
                Return False
            End If

            Dim dao As IUserDao = New NHibernateDaoFactory().GetUserDao()

            If (dao Is Nothing) Then
                Return False
            End If

            ' The following user groups are allowed to change their unit from the MyAccount page:
            '   * Med Techs that have another account with a HQ AFRC Tech role associated with it.

            If (appUser.CurrentRole.Group.Id <> UserGroups.MedicalTechnician) Then
                Return False
            End If

            ' If this user is a Med Tech and has another account which has a HQ AFRC Tech role associated with it...
            If (appUser.CurrentRole.Group.Id = UserGroups.MedicalTechnician AndAlso dao.HasHQTechAccount(appUser.Id, appUser.EDIPIN)) Then
                Return True
            End If

            Return False

        End Function

        Private Sub CleanHashValues()
            '*****************************
            hdnInitialHash.Value = ""
            hdnOldControlList.Value = ""
            '*****************************
        End Sub

        Private Sub UpdateUserRoleDisplay(ByVal userId As Integer)

            'show current roles
            Dim factory As New NHibernateDaoFactory()
            Dim roleDao As IUserRoleDao = factory.GetUserRoleDao()
            Dim currentRoles As IList(Of UserRole) = roleDao.GetByUserId(userId)
            RolesGrid.DataSource = currentRoles
            RolesGrid.DataBind()

            Dim current = From r In currentRoles Select r.Group

            'show pending requests
            Dim reqDao As IUserRoleRequestDao = factory.GetUserRoleRequestDao()
            Dim openReqs = reqDao.GetRequestsByUser(userId)

            PendingGrid.DataSource = From op In openReqs
                                     Where op.Status = AccessStatus.Pending
                                     Select op
                                     Order By op.DateCompleted

            PendingGrid.DataBind()

            'get the combined list of groups in pending and current so that we can remove them from the
            'available list of groups to pick from
            current = current.Union(From og In openReqs
                                    Where og.Status = AccessStatus.Pending
                                    Select og.RequestedGroup)

            'show completed requests
            CompletedGrid.DataSource = From op In openReqs
                                       Where op.Status <> AccessStatus.Pending _
                                       And op.Status <> AccessStatus.None
                                       Select op
                                       Order By op.DateCompleted

            CompletedGrid.DataBind()

            'show available roles for new role change requests
            Dim groups As IQueryable(Of UserGroup) = factory.GetUserGroupDao().GetAll()

            'allow the user to pick from roles that they don't already have
            ChangeRoleSelect.DataSource = From g In groups
                                          Where g.CanBeRequested = True _
                                          And Not current.Contains(g)
                                          Select g
                                          Order By g.Description

            ChangeRoleSelect.DataBind()
        End Sub

    End Class

End Namespace