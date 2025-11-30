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
Imports ALODWebUtility.LookUps

Namespace Web.Admin

    Partial Class Secure_Shared_Admin_EditUser
        Inherits System.Web.UI.Page

        Protected Const COMMAND_DISABLE_ACCOUNT As String = "DISABLE_ACCCOUNT"
        Protected Const COMMAND_VIEW_ACCOUNT As String = "VIEW_ACCOUNT"
        Protected Const KEY_CALLER As String = "Caller"
        Protected Const KEY_EDIT_ID As String = "EditId"
        Protected edituser As AppUser
        Dim _dao As IUserDao
        Dim accessStatus As AccessStatus

        ReadOnly Property Caller() As Integer
            Get
                If Request.QueryString(KEY_CALLER) IsNot Nothing Then
                    Dim k As Integer = 0
                    Integer.TryParse(Request.QueryString(KEY_CALLER), k)
                    Return k
                Else
                    Return 0
                End If
            End Get
        End Property

        ReadOnly Property UserDao() As IUserDao
            Get
                If (_dao Is Nothing) Then
                    _dao = New NHibernateDaoFactory().GetUserDao()
                End If

                Return _dao
            End Get
        End Property

        Protected ReadOnly Property CalendarImage() As String
            Get
                Return Me.ResolveClientUrl("~/App_Themes/" + Page.Theme + "/Images/Calendar.gif")
            End Get
        End Property

        Protected ReadOnly Property currentUser() As AppUser
            Get
                If (edituser Is Nothing) Then
                    If Not (Session(KEY_EDIT_ID) Is Nothing) Then
                        edituser = UserDao.FindById(EditId)
                    Else
                        edituser = Nothing
                    End If

                End If
                Return edituser
            End Get
        End Property

        Protected Property EditId() As Integer
            Get
                If (Session(KEY_EDIT_ID) Is Nothing) Then
                    Response.Redirect(Resources._Global.StartPage)
                End If
                Return CInt(Session(KEY_EDIT_ID))
            End Get
            Set(ByVal value As Integer)
                Session(KEY_EDIT_ID) = value
            End Set
        End Property

        Public Sub CommitChanges()
            UserDao.CommitChanges()
            UserDao.Evict(edituser)
            edituser = Nothing
        End Sub

        'LODPM Enable button For Investigating Officer
        Public Function DisableLODPm(ByVal groupId As Integer) As Boolean
            Dim userGroupId As Integer = CInt(Session("groupId"))
            Dim status As AccessStatus = accessStatus
            If (userGroupId = 96 And groupId = 14 And status = AccessStatus.Disabled) Then
                Return True
            End If
            Return False
        End Function

        'Disable LODPM Edit button except For Investigating Officer
        Public Function DisableLODPmIO(ByVal groupId As Integer) As String
            Dim userGroupId As Integer = CInt(Session("groupId"))
            Dim show As String = "visible"
            Dim hide As String = "hidden"
            If (userGroupId = 96 And groupId = 14) Then
                Return show
            End If

            If (userGroupId = 1) Then
                Return show
            End If
            Return hide
        End Function

        Public Sub SaveUser()

            Dim passed As Boolean = True
            Dim expires As DateTime

            DateTime.TryParse(ExpirationBox.Text.Trim, expires)

            If (expires.Ticks = 0) OrElse (expires < DateTime.Now) Then
                AddCssClass(ExpirationBox, CSS_FIELD_REQUIRED)
                passed = False
            Else
                RemoveCssClass(ExpirationBox, CSS_FIELD_REQUIRED)
            End If

            If (Not ValidateRequiredField(EmailBox)) Then
                passed = False
            Else
                If (Not IsValidEmail(EmailBox.Text.Trim)) Then
                    AddCssClass(EmailBox, CSS_FIELD_REQUIRED)
                    passed = False
                Else
                    RemoveCssClass(EmailBox, CSS_FIELD_REQUIRED)
                End If
            End If

            If (Email2Box.Text.Trim.Length > 0) Then
                'they entered a second (personnal) email, make sure it's valid
                If (Not IsValidEmail(Email2Box.Text.Trim)) Then
                    AddCssClass(Email2Box, CSS_FIELD_REQUIRED)
                    passed = False
                Else
                    RemoveCssClass(Email2Box, CSS_FIELD_REQUIRED)
                End If
            End If

            If (Email3Box.Text.Trim.Length > 0) Then
                'they entered a third (unit) email, make sure it's valid
                If (Not IsValidEmail(Email3Box.Text.Trim)) Then
                    AddCssClass(Email3Box, CSS_FIELD_REQUIRED)
                    passed = False
                Else
                    RemoveCssClass(Email3Box, CSS_FIELD_REQUIRED)
                End If
            End If

            If (Not IsValidPhoneNumber(PhoneBox.Text.Trim)) Then
                AddCssClass(PhoneBox, CSS_FIELD_REQUIRED)
                passed = False
            Else
                RemoveCssClass(PhoneBox, CSS_FIELD_REQUIRED)
            End If

            If (Not passed) Then
                Exit Sub
            End If

            'Everything looks valid, save it
            Dim user As AppUser = currentUser

            If Page.IsValid() Then

                Dim changes As New ChangeSet()
                Const SECTION_USER_DATA As String = "User Data"

                Dim rankId As Integer = 0
                Integer.TryParse(RankSelect.SelectedValue, rankId)

                If (rankId <> user.Rank.Id) Then
                    Dim dao As ILookupDao = New NHibernateDaoFactory().GetLookupDao()
                    Dim rank As UserRank = dao.GetRank(rankId)
                    changes.Add(SECTION_USER_DATA, "Rank", user.Rank.Title, rank.Title)
                    user.Rank = rank
                    RankLabel.Text = rank.Title + "  (" + rank.Grade + ")"
                End If

                Dim currentView As Byte = 0
                Dim view As Byte = 0
                Byte.TryParse(ViewSelect.SelectedValue, view)

                If (user.ReportView.HasValue) Then
                    currentView = CByte(user.ReportView.Value)
                End If

                If (currentView <> view) Then

                    If (view = 0) Then
                        user.ReportView = Nothing
                    Else
                        user.ReportView = view
                    End If

                    changes.Add(SECTION_USER_DATA, "Report View",
                                GetReportingViewDescription(currentView),
                                GetReportingViewDescription(view))
                End If

                Dim ediPin As String = Server.HtmlEncode(EDIPINtxt.Text.Trim)

                If ediPin.Length > 0 AndAlso UserHasPermission(PERMISSION_EDIT_EDIPIN_NUMBERS) Then

                    If (ediPin <> user.EDIPIN) Then
                        changes.Add(SECTION_USER_DATA, "EDIPIN", user.EDIPIN, ediPin)
                        user.EDIPIN = ediPin
                    End If

                End If

                If (ReceiveEmailCheck.Checked <> user.ReceiveEmail) Then
                    changes.Add(SECTION_USER_DATA, "Receive Email", user.ReceiveEmail.ToString(), ReceiveEmailCheck.Checked.ToString())
                    user.ReceiveEmail = ReceiveEmailCheck.Checked
                End If

                If (ReceiveReminderEmailCheck.Checked <> user.ReceiveReminderEmail) Then
                    changes.Add(SECTION_USER_DATA, "Recieve Reminder Email", user.ReceiveReminderEmail.ToString(), ReceiveReminderEmailCheck.Checked.ToString())
                    user.ReceiveReminderEmail = ReceiveReminderEmailCheck.Checked
                End If
                Dim comments As String = Server.HtmlEncode(CommentsBox.Text.Trim)
                If (user.AccountComment <> comments) Then
                    changes.Add(SECTION_USER_DATA, "Comments", user.AccountComment, comments)
                    user.AccountComment = comments
                End If

                Dim phone As String = Server.HtmlEncode(PhoneBox.Text.Trim)

                If (user.Phone <> phone) Then
                    changes.Add(SECTION_USER_DATA, "Phone", user.Phone, phone)
                    user.Phone = phone
                End If

                Dim dsn As String = Server.HtmlEncode(DsnBox.Text.Trim)

                If (user.DSN <> dsn) Then
                    changes.Add(SECTION_USER_DATA, "DSN", user.DSN, dsn)
                    user.DSN = dsn
                End If

                Dim email1 As String = Server.HtmlEncode(EmailBox.Text.Trim)
                Dim email2 As String = Server.HtmlEncode(Email2Box.Text.Trim)
                Dim email3 As String = Server.HtmlEncode(Email3Box.Text.Trim)

                If (user.Email <> email1) Then
                    changes.Add(SECTION_USER_DATA, "Work Email", user.Email, email1)
                    user.Email = email1
                End If

                If (user.Email2 <> email2) Then
                    changes.Add(SECTION_USER_DATA, "Personal Email", user.Email2, email2)
                    user.Email2 = email2
                End If

                If (user.Email3 <> email3) Then
                    changes.Add(SECTION_USER_DATA, "Unit Email", user.Email3, email3)
                    user.Email3 = email3
                End If

                Dim street As String = Server.HtmlEncode(StreetBox.Text.Trim)
                Dim city As String = Server.HtmlEncode(CityBox.Text.Trim)
                Dim state As String = StateSelect.SelectedValue
                Dim zip As String = Server.HtmlEncode(ZipBox.Text.Trim)

                If (user.Address Is Nothing) Then
                    user.Address = New ALOD.Core.Domain.Lookup.Address()
                End If

                If (user.Address IsNot Nothing) AndAlso (user.Address.Street <> street _
                   OrElse user.Address.City <> city _
                   OrElse user.Address.State <> state _
                   OrElse user.Address.Zip <> zip) Then

                    Dim currentAddress As String = user.Address.Street + ", " +
                     user.Address.City + ", " + user.Address.State + " " +
                     user.Address.Zip

                    Dim newAddress As String = street + ", " +
                        city + ", " + state + " " +
                        zip

                    changes.Add(SECTION_USER_DATA, "Address", currentAddress, newAddress)

                    If (user.Address Is Nothing) Then
                        user.Address = New ALOD.Core.Domain.Lookup.Address()
                    End If

                    user.Address.Street = street
                    user.Address.City = city
                    user.Address.State = state
                    user.Address.Zip = zip

                End If

                Dim compoValue As String = CompoSelect.SelectedValue
                Dim compo As String = "none"
                If (compoValue = "ANG") Then
                    compo = "5"
                ElseIf (compoValue = "AFRC") Then
                    compo = "6"
                End If
                If (user.Component <> compo) Then
                    changes.Add(SECTION_USER_DATA, "Component", user.Component, compo)
                    user.Component = compo
                End If

                Dim expireDate As DateTime = Date.Parse(Server.HtmlEncode(ExpirationBox.Text.Trim))

                If (expires.Date <> user.AccountExpiration.Value.Date) Then
                    changes.Add(SECTION_USER_DATA, "Expiration Date", user.AccountExpiration.Value.ToString(DATE_FORMAT), expireDate.ToString(DATE_FORMAT))
                    user.AccountExpiration = expireDate
                End If

                Dim unitId As Integer = -1
                Integer.TryParse(newUnitIDLabel.Value.Trim, unitId)

                If newUnitIDLabel.Value.Trim <> "" Then
                    Dim newUnit As Unit = New NHibernateDaoFactory().GetUnitDao().FindById(unitId)
                    If (newUnit IsNot Nothing) AndAlso (newUnit.Id <> user.Unit.Id) Then
                        changes.Add(SECTION_USER_DATA, "Unit", user.Unit.Name, newUnit.Name)
                        user.Unit = newUnit
                    End If
                End If

                Dim adUnitId As Integer = -1
                Integer.TryParse(adUnitIDLabel.Value.Trim, adUnitId)
                Dim currentADUnit As String = String.Empty
                If user.ActiveDutyUnit IsNot Nothing Then
                    currentADUnit = user.ActiveDutyUnit.Name

                End If

                If (adUnitIDLabel.Value.Trim = String.Empty) Then

                    If currentADUnit <> String.Empty Then
                        changes.Add(SECTION_USER_DATA, "Attachment Unit", user.ActiveDutyUnit.Name, "")
                    End If
                    user.ActiveDutyUnit = Nothing
                Else
                    Dim newAdUnit As Unit = New NHibernateDaoFactory().GetUnitDao().FindById(adUnitId)
                    If (newAdUnit IsNot Nothing) Then

                        Select Case (currentADUnit)
                            Case String.Empty
                                changes.Add(SECTION_USER_DATA, "Attachment Unit", "", newAdUnit.Name)
                            Case Else
                                If (newAdUnit.Id <> user.ActiveDutyUnit.Id) Then
                                    changes.Add(SECTION_USER_DATA, "Attachment Unit", user.ActiveDutyUnit.Name, newAdUnit.Name)
                                End If
                        End Select
                        user.ActiveDutyUnit = newAdUnit
                    End If

                End If

                If (changes.Count > 0) Then
                    user.ModifiedBy = UserService.CurrentUser
                    user.ModifiedDate = DateTime.Now
                    CommitChanges()
                    Dim logId As Integer = LogManager.LogAction(ModuleType.System, UserAction.ChangedFormData, currentUser.Id)
                    changes.Save(logId)
                    UpdateUserData()
                    UpdateAccountHistory(currentUser.Id)

                End If

            End If

        End Sub

        Public Sub UpdateUserData()
            Dim user As AppUser = currentUser
            Dim userGroupId As Integer = CInt(Session("groupId"))
            If (user Is Nothing) Then
                Response.Redirect(Resources._Global.StartPage, True)
            End If

            'account status
            UpdateButtonStates(user.Status)
            SetStatusLabel(user.Status)
            NameLabel.Text = user.FullName

            'account settings
            If (user.AccountExpiration.HasValue) Then
                ExpirationBox.Text = Server.HtmlDecode(user.AccountExpiration.Value.ToString(DATE_FORMAT))
            End If

            ReceiveEmailCheck.Checked = user.ReceiveEmail
            ReceiveReminderEmailCheck.Checked = user.ReceiveReminderEmail
            CommentsBox.Text = Server.HtmlDecode(user.AccountComment)

            UpdateUserRoleDisplay(user.Id)
            UpdateAccountHistory(user.Id)

            'user information
            If (user.SSN IsNot Nothing) AndAlso (user.SSN.Length = 9) Then
                SsnLabel.Text = user.SSN.Substring(user.SSN.Length - 4, 4)
            End If

            UsernameLabel.Text = user.Username
            RankLabel.Text = user.Rank.Title + "  (" + user.Rank.Grade + ")"
            UnitTextBox.Text = Server.HtmlDecode(user.Unit.Name + " (" + user.Unit.PasCode + ")")
            If user.ActiveDutyUnit IsNot Nothing Then
                adUnitTextBoX.Text = Server.HtmlDecode(user.ActiveDutyUnit.Name + " (" + user.ActiveDutyUnit.PasCode + ")")
            Else
                adUnitTextBoX.Text = ""

            End If
            Try
                If (user.ReportView.HasValue) Then
                    MsgBox(CByte(user.ReportView.Value).ToString(),
                   MsgBoxStyle.OkCancel, "Lessons Objectives")
                    SetDropdownByValue(ViewSelect, CByte(user.ReportView.Value).ToString())
                End If
            Catch
            End Try

            If (userGroupId = 96) Then
                ViewSelect.Visible = False
                ViewSelectLODPm.Visible = True
            Else
                ViewSelectLODPm.Visible = False
            End If

            If (userGroupId = 1) Then
                ViewSelect.Visible = True
            End If

            Dim lkupDAO As ILookupDao
            lkupDAO = New NHibernateDaoFactory().GetLookupDao()

            RankSelect.DataSource = From n In lkupDAO.GetRanks() Select n Where Not n.Name.Equals("Unknown")
            RankSelect.DataTextField = "Name"
            RankSelect.DataValueField = "Value"
            RankSelect.DataBind()

            SetDropdownByValue(RankSelect, user.Rank.Id)

            If UserHasPermission(PERMISSION_EDIT_EDIPIN_NUMBERS) Then
                EDIPINtxt.Text = Server.HtmlDecode(user.EDIPIN)
            Else
                If Not user.EDIPIN Is Nothing Then
                    If user.EDIPIN.Length > 0 Then
                        EDIPINtxt.Text = "XXXXXXXXXXXXXXXXXXXXX"
                    End If
                Else
                    EDIPINtxt.Text = ""
                End If
            End If

            PhoneBox.Text = Server.HtmlDecode(user.Phone)
            DsnBox.Text = Server.HtmlDecode(user.DSN)
            EmailBox.Text = Server.HtmlDecode(user.Email)
            Email2Box.Text = Server.HtmlDecode(user.Email2)
            Email3Box.Text = Server.HtmlDecode(user.Email3)

            Dim lookup As New LookUp()
            StateSelect.DataSource = lookup.GetStates()
            StateSelect.DataBind()
            StateSelect.Items.Insert(0, New ListItem("-Select-", ""))

            If Not user.Address Is Nothing Then
                StreetBox.Text = Server.HtmlDecode(user.Address.Street)
                CityBox.Text = Server.HtmlDecode(user.Address.City)
                SetDropdownByValue(StateSelect, user.Address.State)
                ZipBox.Text = Server.HtmlDecode(user.Address.Zip)
            End If

            CompoSelect.DataSource = From n In lkupDAO.GetCompos()
            CompoSelect.DataTextField = "Name"
            CompoSelect.DataValueField = "Value"
            CompoSelect.DataBind()
            If user.Component.ToString() = "5" Then
                SetDropdownByValue(CompoSelect, "ANG")
            ElseIf user.Component.ToString() = "6" Then
                SetDropdownByValue(CompoSelect, "AFRC")
            End If

            'history printing
            If Not Page.IsPostBack Then
                PrintHistory.Attributes.Add("onclick", "printHistory(" + user.Id.ToString() + "); return false;")
            End If

        End Sub

        Protected Sub AccountsGrid_RowCommand(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewCommandEventArgs) Handles AccountsGrid.RowCommand

            Dim userId As Integer = 0
            Integer.TryParse(e.CommandArgument, userId)

            If (userId = 0) Then
                Exit Sub
            End If

            If (e.CommandName = COMMAND_VIEW_ACCOUNT) Then
                EditId = userId
                Response.Redirect(Request.Url.OriginalString)
            ElseIf (e.CommandName = COMMAND_DISABLE_ACCOUNT) Then

                Dim user As AppUser = UserDao.GetById(userId)
                user.AccountExpiration = DateTime.Now.AddDays(-1)
                user.Status = AccessStatus.Disabled
                user.ModifiedBy = UserService.CurrentUser
                user.ModifiedDate = DateTime.Now
                UserDao.CommitChanges()
                UserDao.Evict(user)

                Dim logid As Integer = LogManager.LogAction(ModuleType.System, UserAction.ModifyAccountStatus, userId)

                Dim changes As New ChangeSet()
                changes.Add("Account Status", "Status", "Approved", "Disabled")
                changes.Save(logid)

                UpdateAccountListing()

            End If

        End Sub

        Protected Sub AccountsGrid_RowDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewRowEventArgs) Handles AccountsGrid.RowDataBound

            If (e.Row.RowType <> DataControlRowType.DataRow) Then
                Exit Sub
            End If

            Dim user As AppUser = DirectCast(e.Row.DataItem, AppUser)
            Dim statusLabel As Label = CType(e.Row.FindControl("StatusLabel"), Label)
            Dim disableLink As LinkButton = CType(e.Row.FindControl("DisableLink"), LinkButton)

            Select Case user.Status
                Case AccessStatus.Approved
                    statusLabel.ForeColor = Drawing.Color.Green
                Case AccessStatus.Pending
                    statusLabel.ForeColor = Drawing.Color.OrangeRed
                Case Else
                    statusLabel.ForeColor = Drawing.Color.Red
                    disableLink.Enabled = False
                    disableLink.Text = ""
            End Select

        End Sub

        Protected Sub CreateAccountButton_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles CreateAccountButton.Click

            If (Not UserHasPermission(PERMISSION_SYSTEM_ADMIN)) Then
                Exit Sub
            End If

            'We are duplicating the current user account
            Dim newId As Integer = UserService.DuplicateAccount(currentUser.Id, SESSION_USER_ID)

            If (newId > 0) Then
                'the new account was created, log this action
                Dim changes As New ChangeSet()
                changes.Add("User Account", "Account Created", "", "")
                Dim logId As Integer = LogManager.LogAction(ModuleType.System, UserAction.CreatedNewAccount, newId)
                changes.Save(logId)

                UpdateAccountListing()
            End If

        End Sub

        Protected Sub DisableButton_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles DisableButton.Click

            Dim oldStatus As String = currentUser.StatusDescription

            currentUser.Status = AccessStatus.Disabled
            UpdateButtonStates(AccessStatus.Disabled)
            SetStatusLabel(AccessStatus.Disabled)
            hdButtonClicked.Value = "Account Disabled"
            currentUser.DisabledBy = SESSION_GROUP_ID

            'Dim currentAction As String
            'currentAction = "Account Disabled "
            'UserService.SendAccountModifiedEmail(Session("UserId"), currentUser.Id, GetHostName(), currentAction)

            'log the change
            Dim logId As Integer = LogManager.LogAction(ModuleType.System, UserAction.ModifyAccountStatus, currentUser.Id)

            Dim changes As New ChangeSet()
            changes.Add("Account Status", "Status", oldStatus, "Disabled")
            changes.Save(logId)

            UpdateAccountHistory(currentUser.Id)

        End Sub

        Protected Sub EnableButton_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles EnableButton.Click

            Dim oldStatus As String = currentUser.StatusDescription
            currentUser.Status = AccessStatus.Approved
            UpdateButtonStates(AccessStatus.Approved)
            SetStatusLabel(AccessStatus.Approved)
            hdButtonClicked.Value = "Account Approved"
            currentUser.DisabledBy = 0

            'Dim currentAction As String
            'currentAction = "Account Approved "
            'UserService.SendAccountModifiedEmail(Session("UserId"), currentUser.Id, GetHostName(), currentAction)

            'log the change
            Dim logId As Integer = LogManager.LogAction(ModuleType.System, UserAction.ModifyAccountStatus, currentUser.Id)

            Dim changes As New ChangeSet()
            changes.Add("Account Status", "Status", oldStatus, "Approved")
            changes.Save(logId)

            UpdateAccountHistory(currentUser.Id)

        End Sub

        Protected Sub EnableIOButton_Click(ByVal sender As Object, ByVal e As System.EventArgs)

            Dim userGroupId As Integer = CInt(Session("groupId"))
            Dim oldStatus As String = currentUser.StatusDescription
            currentUser.Status = AccessStatus.Approved
            UpdateButtonStates(AccessStatus.Approved)
            SetStatusLabel(AccessStatus.Approved)
            hdButtonClicked.Value = "Account Approved"
            currentUser.DisabledBy = 0

            'Dim currentAction As String
            'currentAction = "Account Approved "
            'UserService.SendAccountModifiedEmail(Session("UserId"), currentUser.Id, GetHostName(), currentAction)

            'log the change
            Dim logId As Integer = LogManager.LogAction(ModuleType.System, UserAction.ModifyAccountStatus, currentUser.Id)

            Dim changes As New ChangeSet()
            changes.Add("Account Status", "Status", oldStatus, "Approved")
            changes.Save(logId)

            UpdateAccountHistory(currentUser.Id)
            DisableLODPm(userGroupId)

        End Sub

        Protected Sub lnkManageMembers_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles lnkManageMembers.Click
            Session(KEY_EDIT_ID) = Nothing
            Response.Redirect("~/Secure/Shared/Admin/MemberData.aspx")
        End Sub

        Protected Sub lnkManageRoles_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles lnkManageRoles.Click
            Session(KEY_EDIT_ID) = Nothing
            Response.Redirect("~/Secure/Shared/Admin/RoleRequests.aspx")
        End Sub

        Protected Sub lnkManageUsers_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles lnkManageUsers.Click
            Session(KEY_EDIT_ID) = Nothing
            Response.Redirect("~/Secure/Shared/Admin/ManageUsers.aspx")
        End Sub

        Protected Sub lnkPermissionReport_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles lnkPermissionReport.Click
            Session(KEY_EDIT_ID) = Nothing
            Response.Redirect("~/Secure/Shared/Admin/PermissionReport.aspx")
        End Sub

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

            If (Session(KEY_EDIT_ID) Is Nothing) Then
                Response.Redirect(Resources._Global.StartPage, True)
            End If

            If (Not IsPostBack) Then
                ChangeUnitButton.Attributes.Add("onclick", "showSearcher('" + "Select New Unit" + "','" + newUnitIDLabel.ClientID + "','" + UnitTextBox.ClientID + "'); return false;")
                ChangeAdUnitBtn.Attributes.Add("onclick", "showSearcher('" + "Select New Unit" + "','" + adUnitIDLabel.ClientID + "','" + adUnitTextBoX.ClientID + "'); return false;")

                SetInputFormatRestriction(Page, EDIPINtxt, FormatRestriction.AlphaNumeric, PERMITTED_SPECIAL_CHAR_INPUT)
                SetInputFormatRestriction(Page, UnitTextBox, FormatRestriction.AlphaNumeric, PERMITTED_SPECIAL_CHAR_INPUT)
                SetInputFormatRestriction(Page, adUnitTextBoX, FormatRestriction.AlphaNumeric, PERMITTED_SPECIAL_CHAR_INPUT)
                SetInputFormatRestriction(Page, ExpirationBox, FormatRestriction.Numeric, "/")
                SetInputFormatRestriction(Page, CommentsBox, FormatRestriction.AlphaNumeric, PERMITTED_SPECIAL_CHAR_INPUT)
                SetInputFormatRestriction(Page, PhoneBox, FormatRestriction.Numeric, "-")
                SetInputFormatRestriction(Page, DsnBox, FormatRestriction.Numeric)
                SetInputFormatRestriction(Page, EmailBox, FormatRestriction.AlphaNumeric, PERMITTED_SPECIAL_CHAR_INPUT)
                SetInputFormatRestriction(Page, Email2Box, FormatRestriction.AlphaNumeric, PERMITTED_SPECIAL_CHAR_INPUT)
                SetInputFormatRestriction(Page, Email3Box, FormatRestriction.AlphaNumeric, PERMITTED_SPECIAL_CHAR_INPUT)
                SetInputFormatRestriction(Page, StreetBox, FormatRestriction.AlphaNumeric, PERMITTED_SPECIAL_CHAR_INPUT)
                SetInputFormatRestriction(Page, CityBox, FormatRestriction.AlphaNumeric, PERMITTED_SPECIAL_CHAR_INPUT)
                SetInputFormatRestriction(Page, ZipBox, FormatRestriction.Numeric, "-")

                UpdateUserData()

                If UserHasPermission(PERMISSION_EDIT_EDIPIN_NUMBERS) Then
                    EDIPINtxt.Enabled = True
                    MultipleAccountsPanel.Visible = True
                    UpdateAccountListing()
                Else
                    EDIPINtxt.Enabled = False
                    MultipleAccountsPanel.Visible = False
                End If

                If Caller = 1 Then
                    lnkManageUsers.Visible = True
                ElseIf Caller = 2 Then
                    lnkManageMembers.Visible = True
                ElseIf Caller = 3 Then
                    lnkManageRoles.Visible = True
                ElseIf Caller = 4 Then
                    lnkPermissionReport.Visible = True
                End If

            End If

        End Sub

        Protected Sub RequestRoleButton_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles RequestRoleButton.Click

            Dim roleId As Integer = 0
            Integer.TryParse(ChangeRoleSelect.SelectedValue, roleId)
            If roleId = 0 Then
                Exit Sub
            End If

            Dim NUM_VALUES As Integer = 3

            'If (RequestDialogValues.Text.Length = 0) Then
            '    Exit Sub
            'End If

            'Dim vals As String() = RequestDialogValues.Text.Split("|")

            'If (vals.Length <> NUM_VALUES) Then
            '    Exit Sub
            'End If

            Dim id As Integer = 0
            Dim approved As Boolean = False
            'Dim notes As String

            'Integer.TryParse(vals(0), id)
            'Boolean.TryParse(vals(1), approved)
            'notes = vals(2)

            Dim factory As New NHibernateDaoFactory()
            'Dim dao As IUserRoleRequestDao = factory.GetUserRoleRequestDao()
            'Dim request As UserRoleRequest = dao.GetById(id)
            Dim role As New UserRole()
            Dim group As UserGroup = factory.GetUserGroupDao().GetById(roleId)
            role.Group = group
            role.User = currentUser
            role.Status = AccessStatus.Approved

            factory.GetUserRoleDao().Save(role)
            'request.CompletedNotes = notes

            If (role.Id > 0) Then
                Dim logId As Integer = LogManager.LogAction(ModuleType.System, UserAction.CreatedUserRole, currentUser.Id)
                Dim change As New ChangeSet()
                change.Add("Role Added", "Added Role", "", role.Group.Description)
                change.Save(logId)
                Dim currentAction As String

                currentAction = "Role of '" + role.Group.Description + "' has been created"
                'UserService.SendAccountModifiedEmail(Session("UserId"), currentUser.Id, GetHostName(), currentAction)

            End If

            UpdateUserRoleDisplay(currentUser.Id)
            UpdateAccountHistory(currentUser.Id)

        End Sub

        Protected Sub RoleDialogButton_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles RoleDialogButton.Click

            Dim NUM_VALUES As Integer = 3

            If (RoleDialogValues.Text.Length = 0) Then
                Exit Sub
            End If

            Dim vals As String() = RoleDialogValues.Text.Split("|")

            If (vals.Length <> NUM_VALUES) Then
                Exit Sub
            End If

            Dim id As Integer = 0
            Dim approved As Boolean = False
            Dim comments As String

            Integer.TryParse(vals(0), id)
            Boolean.TryParse(vals(1), approved)
            comments = vals(2)

            If (id = 0) Then
                Exit Sub
            End If

            Dim factory As New NHibernateDaoFactory()
            Dim dao As IUserRoleRequestDao = factory.GetUserRoleRequestDao()
            Dim request As UserRoleRequest = dao.GetById(id)

            request.CompletedBy = UserService.CurrentUser
            request.DateCompleted = DateTime.Now
            request.CompletedComments = comments

            If (approved) Then
                request.Status = AccessStatus.Approved
            Else
                request.Status = AccessStatus.Disabled 'we don't have a 'denied' status, so we use disabled
            End If

            'first we update the request itself
            dao.SaveOrUpdate(request)
            Dim currentAction As String
            'now we do whatever the request was for
            Dim type As String = IIf(request.IsNewRole, "New Role", "Role Change")
            If (Not approved) Then
                'the request was denied
                Dim logId As Integer = LogManager.LogAction(ModuleType.System, UserAction.DeniedRoleChange, currentUser.Id)
                Dim change As New ChangeSet()
                change.Add("Change denied", type, request.RequestedGroup.Description, "")
                currentAction = type + " request for '" + request.RequestedGroup.Description + "' has been denied"
                change.Save(logId)
            Else
                currentAction = type + " request for '" + request.RequestedGroup.Description + "' has been approved"
                'the request was approved, so process the request
                If (request.IsNewRole) Then
                    'this is to add a new role
                    Dim role As New UserRole()
                    role.Group = request.RequestedGroup
                    role.User = request.User
                    role.Status = AccessStatus.Approved
                    role.Active = False

                    factory.GetUserRoleDao().Save(role)

                    If (role.Id > 0) Then
                        'log the change
                        Dim logId As Integer = LogManager.LogAction(ModuleType.System, UserAction.ApprovedRoleChange, currentUser.Id)
                        Dim change As New ChangeSet()
                        change.Add("Change Approved", "Added Role", "", role.Group.Description)
                        change.Save(logId)
                    End If
                Else
                    'this is a change request, so change the role

                    'find the role that matches the request
                    Dim role As UserRole = Nothing

                    For Each currentRole As UserRole In currentUser.AllRoles
                        If (currentRole.Group.Id = request.CurrentGroup.Id) Then
                            role = currentRole
                            Exit For
                        End If
                    Next

                    If (role Is Nothing) Then
                        'we didn't find a matching role, it might have gotten changed since the request
                        'so grab the active role and use that
                        role = currentUser.CurrentRole
                    End If

                    'now update it
                    role.Group = request.RequestedGroup
                    factory.GetUserRoleDao().SaveOrUpdate(role)

                    'log the change
                    Dim logId As Integer = LogManager.LogAction(ModuleType.System, UserAction.ApprovedRoleChange, currentUser.Id)
                    Dim change As New ChangeSet()
                    change.Add("Change Approved", "Changed Role", request.CurrentGroup.Description, role.Group.Description)
                    change.Save(logId)

                End If

            End If

            'finally, update the displays
            UpdateUserRoleDisplay(currentUser.Id)
            UpdateAccountHistory(currentUser.Id)

            'UserService.SendAccountModifiedEmail(Session("UserId"), currentUser.Id, GetHostName(), currentAction)

        End Sub

        Protected Sub RolesGrid_RowCommand(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewCommandEventArgs) Handles RolesGrid.RowCommand

            If (e.CommandName = "DeleteRole") Then
                Dim id As Integer = 0
                Integer.TryParse(e.CommandArgument, id)

                If (id = 0) Then
                    Exit Sub
                End If

                Dim roleDao As IUserRoleDao = New NHibernateDaoFactory().GetUserRoleDao()
                Dim role As UserRole = roleDao.GetById(id)

                Dim logId As Integer = LogManager.LogAction(ModuleType.System, UserAction.DeletedUserRole, currentUser.Id)
                Dim change As New ChangeSet()
                change.Add("Role Added", "Deleted Role", role.Group.Description, "")
                change.Save(logId)

                roleDao.Delete(role)
                roleDao.CommitChanges()

                UpdateUserRoleDisplay(currentUser.Id)
                UpdateAccountHistory(currentUser.Id)
                Dim currentAction As String
                currentAction = "Role of '" + role.Group.Description + "' has been deleted"
                '   UserService.SendAccountModifiedEmail(Session("UserId"), currentUser.Id, 0(), currentAction)

            End If

        End Sub

        Protected Sub RolesGrid_RowDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewRowEventArgs) Handles RolesGrid.RowDataBound

            If (e.Row.RowType <> DataControlRowType.DataRow) Then
                Exit Sub
            End If

            Dim role As UserRole = CType(e.Row.DataItem, UserRole)
            CType(e.Row.FindControl("RoleTitle"), Label).Text = role.Group.Description
            CType(e.Row.FindControl("RoleActive"), Image).Visible = role.Active

        End Sub

        Protected Sub RoleUpdateButton_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles RoleUpdateButton.Click

            Dim NUM_VALUES As Integer = 4

            If (RoleDialogValues.Text.Length = 0) Then
                Exit Sub
            End If

            Dim vals As String() = RoleDialogValues.Text.Split("|")

            If (vals.Length <> NUM_VALUES) Then
                Exit Sub
            End If

            Dim id As Integer = 0
            Dim oldGroup As Integer = 0
            Dim newGroup As Integer = 0
            Dim active As Boolean = False

            Integer.TryParse(vals(0), id)
            Integer.TryParse(vals(1), oldGroup)
            Integer.TryParse(vals(2), newGroup)
            Boolean.TryParse(vals(3), active)

            If (id = 0 OrElse oldGroup = 0 OrElse newGroup = 0) Then
                Exit Sub
            End If

            Dim factory As New NHibernateDaoFactory()
            Dim roleDao As IUserRoleDao = factory.GetUserRoleDao()
            Dim role As UserRole = roleDao.GetById(id)

            Dim changes As New ChangeSet()
            Dim currentAction As String

            If (oldGroup <> newGroup) Then
                Dim group As UserGroup = factory.GetUserGroupDao().GetById(newGroup)
                changes.Add("User Role", "Changed Role", role.Group.Description, group.Description)
                role.Group = group
                ' currentUser.CurrentRole.Group = group
                currentAction = "User role has been changed from '" + role.Group.Description + "' to '" + group.Description + "'"
            End If

            If (role.Active <> active AndAlso active = True) Then

                changes.Add("User Role", "Active Role", currentUser.CurrentRole.Group.Description, role.Group.Description)
                currentUser.CurrentRole = role
                For Each currentRole As UserRole In currentUser.AllRoles
                    If currentRole.Id = role.Id Then
                        currentRole.Active = True
                    Else
                        currentRole.Active = False
                    End If
                Next
            End If

            roleDao.SaveOrUpdate(role)
            roleDao.CommitChanges()
            UserDao.SaveOrUpdate(currentUser)

            If (changes.Count > 0) Then
                'log the change
                Dim logId As Integer = LogManager.LogAction(ModuleType.System, UserAction.ModifyUserRole, currentUser.Id)
                changes.Save(logId)
            End If

            UpdateUserRoleDisplay(currentUser.Id)
            UpdateAccountHistory(currentUser.Id)

        End Sub

        Protected Sub SetStatusLabel(ByVal status As AccessStatus)

            StatusLabel.Text = status.ToString()

            If (status = AccessStatus.Approved) Then
                StatusLabel.ForeColor = Drawing.Color.DarkGreen
            Else
                StatusLabel.ForeColor = Drawing.Color.OrangeRed
            End If

        End Sub

        Protected Sub updateButton_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles updateButton.Click
            SaveUser()
        End Sub

        Protected Sub UpdateButtonStates(ByVal status As AccessStatus)
            Dim userGroupId As Integer = CInt(Session("groupId"))
            accessStatus = status
            Select Case status

                Case AccessStatus.Pending
                    EnableButton.Visible = True
                    EnableButton.Enabled = True
                    DisableButton.Visible = True
                    DisableButton.Enabled = True
                    PendingPanel.Visible = True
                    DisabledPanel.Visible = False

                    CommentsBox.Visible = False
                Case AccessStatus.Approved
                    EnableButton.Visible = False
                    DisableButton.Visible = True
                    DisableButton.Enabled = True
                    PendingPanel.Visible = False
                    DisabledPanel.Visible = False
                    CommentsBox.Visible = True
                Case AccessStatus.Disabled

                    If currentUser.DisabledBy.Equals(Nothing) Then
                        EnableButton.Visible = True
                        EnableButton.Enabled = True
                        DisableButton.Enabled = False
                        DisableButton.Visible = False
                        PendingPanel.Visible = False
                        DisabledPanel.Visible = True
                        CommentsBox.Visible = True

                    ElseIf (currentUser.DisabledBy = 1 And SESSION_GROUP_ID = 1) Or Not currentUser.DisabledBy = 1 Then

                        EnableButton.Visible = True
                        EnableButton.Enabled = True
                        DisableButton.Enabled = False
                        DisableButton.Visible = False
                        PendingPanel.Visible = False
                        DisabledPanel.Visible = True
                        CommentsBox.Visible = True
                    Else
                        AdminDisabledLabel.Visible = "True"
                        PendingPanel.Visible = False

                    End If

                Case Else
                    EnableButton.Visible = False
                    DisableButton.Visible = False
                    PendingPanel.Visible = False
                    DisabledPanel.Visible = False
                    CommentsBox.Visible = True
            End Select
            Select Case userGroupId
                Case 96

                    EnableButton.Visible = False
                    DisableButton.Visible = False
                    ActionLabel.Visible = False
                    PendingPanel.Visible = False
            End Select

        End Sub

        Private Sub UpdateAccountHistory(ByVal userId As Integer)

            Dim changes As New ChangeSet()
            changes.GetByUserID(userId)
            HistoryGrid.DataSource = changes
            HistoryGrid.DataBind()

        End Sub

        Private Sub UpdateAccountListing()

            ' Dim users As IList(Of AppUser) = UserService.FindBySSN(currentUser.SSN)

            If (currentUser.EDIPIN IsNot Nothing) AndAlso (currentUser.EDIPIN.Length > 0) Then
                Dim users As IList(Of AppUser) = UserService.FindByEDIPIN(currentUser.EDIPIN)

                AccountsGrid.DataSource = From u In users
                                          Where u.Id <> currentUser.Id
                                          Select u
                                          Order By u.Username
            Else
                AccountsGrid.DataSource = Nothing
            End If

            AccountsGrid.DataBind()

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
            Dim groups As IList(Of UserGroup) = factory.GetUserGroupDao().GetById(SESSION_GROUP_ID).ManagedGroups

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