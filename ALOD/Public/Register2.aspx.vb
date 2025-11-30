Imports ALOD.Core.Domain.Lookup
Imports ALOD.Core.Domain.ServiceMembers
Imports ALOD.Core.Domain.Users
Imports ALOD.Core.Interfaces.DAOInterfaces
Imports ALOD.Core.Utils
Imports ALOD.Core.Utils.RegexValidation
Imports ALOD.Data
Imports ALOD.Data.Services
Imports ALODWebUtility.Common

Namespace Web

    Partial Class Public_Register2
        Inherits System.Web.UI.Page

        'member: 310429547
        'nonmember: 43243243F

        Protected _edipin As String = ""

        Protected Sub NextButton_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles NextButton.Click

            Dim passed As Boolean = True
            If (Not IsValidEmail(EmailBox.Text.Trim)) Then
                EmailBox.CssClass = "fieldRequired"
                passed = False
            Else
                EmailBox.CssClass = ""
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

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
            VerifyAccess()
            WriteHostName(Page)

            If (Not IsPostBack) Then
                btnFindUnit.Attributes.Add("onclick", "showSearcher('" + "Find Unit" + "','" + SrcUnitIdHdn.ClientID + "','" + SrcNameHdn.ClientID + "'); return false;")

                SetInputFormatRestriction(Page, FirstNameBox, FormatRestriction.AlphaNumeric, PERMITTED_SPECIAL_CHAR_INPUT)
                SetInputFormatRestriction(Page, MIddleNameBox, FormatRestriction.AlphaNumeric, PERMITTED_SPECIAL_CHAR_INPUT)
                SetInputFormatRestriction(Page, LastNameBox, FormatRestriction.AlphaNumeric, PERMITTED_SPECIAL_CHAR_INPUT)
                SetInputFormatRestriction(Page, EmailBox, FormatRestriction.AlphaNumeric, PERMITTED_SPECIAL_CHAR_INPUT)
                SetInputFormatRestriction(Page, Email2Box, FormatRestriction.AlphaNumeric, PERMITTED_SPECIAL_CHAR_INPUT)
                SetInputFormatRestriction(Page, Email3Box, FormatRestriction.AlphaNumeric, PERMITTED_SPECIAL_CHAR_INPUT)
                SetInputFormatRestriction(Page, PhoneBox, FormatRestriction.AlphaNumeric, PERMITTED_SPECIAL_CHAR_INPUT)
                SetInputFormatRestriction(Page, DsnBox, FormatRestriction.AlphaNumeric, PERMITTED_SPECIAL_CHAR_INPUT)
                SetInputFormatRestriction(Page, txtAddress, FormatRestriction.AlphaNumeric, PERMITTED_SPECIAL_CHAR_INPUT)
                SetInputFormatRestriction(Page, txtCity, FormatRestriction.AlphaNumeric, PERMITTED_SPECIAL_CHAR_INPUT)
                SetInputFormatRestriction(Page, txtZip, FormatRestriction.AlphaNumeric, PERMITTED_SPECIAL_CHAR_INPUT)

                PopulateStates()
                PopulateRoles()
                PopulateData()
            End If

        End Sub

        Protected Sub PopulateCommonData(ByVal user As AppUser)

            If (user.Unit IsNot Nothing) Then
                PasCodeBox.Text = user.Unit.Name
                SrcUnitIdHdn.Text = user.Unit.Id.ToString()
            End If

            If (user.CurrentRole IsNot Nothing) Then
                SetDropdownByValue(RoleSelect, user.CurrentRole.Group.Id)
            End If

            If (Not String.IsNullOrEmpty(user.Email)) Then
                EmailBox.Text = Server.HtmlDecode(user.Email)
            End If

            If (Not String.IsNullOrEmpty(user.Email2)) Then
                Email2Box.Text = Server.HtmlDecode(user.Email2)
            End If
            If (Not String.IsNullOrEmpty(user.Email3)) Then
                Email3Box.Text = Server.HtmlDecode(user.Email3)
            End If

            If (Not String.IsNullOrEmpty(user.Phone)) Then
                PhoneBox.Text = Server.HtmlDecode(user.Phone)
            End If

            If (Not String.IsNullOrEmpty(user.DSN)) Then
                DsnBox.Text = Server.HtmlDecode(user.DSN)
            End If

            If (user.Address IsNot Nothing) Then
                If (Not String.IsNullOrEmpty(user.Address.Street)) Then
                    txtAddress.Text = Server.HtmlDecode(user.Address.Street)
                End If

                If (Not String.IsNullOrEmpty(user.Address.City)) Then
                    txtCity.Text = Server.HtmlDecode(user.Address.City)
                End If

                If (Not String.IsNullOrEmpty(user.Address.State)) Then
                    SetDropdownByValue(cbState, user.Address.State)
                End If

                If (Not String.IsNullOrEmpty(user.Address.Zip)) Then
                    txtZip.Text = Server.HtmlDecode(user.Address.Zip)
                End If
            End If

            chkReceiveEmail.Checked = user.ReceiveEmail

        End Sub

        Protected Sub PopulateData()

            Dim user As AppUser = UserService.GetByEDIPIN(_edipin)
            Dim data As ServiceMember = Nothing

            If (user Is Nothing) Then

                user = New AppUser()

                'The user does not yet have an account, so try and import from MIL
                '
                If (data IsNot Nothing) Then
                    user.Import(data)
                End If

            End If

            user.EDIPIN = _edipin

            If (data IsNot Nothing) Then
                'user is a member
                PopulateMemberData(user)
            Else
                PopulateNonMemberData(user)
            End If

            PopulateCommonData(user)

        End Sub

        Protected Sub PopulateMemberData(ByVal user As AppUser)

            NameEntryPanel.Visible = False
            NameDisplayPanel.Visible = True
            FullName.Text = user.FullName
            RankDisplay.Text = user.Rank.Title

            FirstNameBox.Visible = False
            FirstNameBox.Text = Server.HtmlDecode(user.FirstName)
            FirstNameLabel.Text = user.FirstName

            LastNameBox.Visible = False
            LastNameBox.Text = Server.HtmlDecode(user.LastName)
            LastNameLabel.Text = user.LastName

            MIddleNameBox.Visible = False
            MIddleNameBox.Text = Server.HtmlDecode(user.MiddleName)
            LastNameLabel.Text = user.MiddleName

            RankLabel.Text = user.Rank.Title
            RankSelect.Visible = False
            SetDropdownByValue(RankSelect, user.Rank.Id)

            PasCodeBox.Text = user.Unit.PasCode + " (" + user.Unit.Name + ")"
            PasCodeHidden.Text = user.Unit.PasCode
            PasCodeLabel.Visible = False
            SrcUnitIdHdn.Text = user.Unit.Id

            If (user.Rank.Id <> 0) Then
                TitleSelect.Visible = False
                TitleLabel.Visible = False
            End If

        End Sub

        Protected Sub PopulateNonMemberData(ByVal user As AppUser)

            NameDisplayPanel.Visible = False
            NameEntryPanel.Visible = True

            FirstNameLabel.Visible = False
            LastNameLabel.Visible = False
            MiddleNameLabel.Visible = False

            FirstNameBox.Text = Server.HtmlDecode(user.FirstName)
            LastNameBox.Text = Server.HtmlDecode(user.LastName)
            MIddleNameBox.Text = Server.HtmlDecode(user.MiddleName)

            SetDropdownByValue(TitleSelect, user.Title)

            PopulateRanks()
            RankSelect.Visible = True

            If (user.Rank IsNot Nothing) Then
                SetDropdownByValue(RankSelect, user.Rank.Id)
            End If

            TitleNum.Text = "C"
            FirstNameNum.Text = "D"
            MiddleNameNum.Text = "E"
            LastNameNum.Text = "F"
            RankNumEntry.Text = "G"
            UnitNum.Text = "H"
            UserRoleNum.Text = "I"
            EmailNum.Text = "J"

            EmailNum2.Text = "K"
            Component.Text = "L"
            EmailNum3.Text = "M"

            PhoneNum.Text = "N"
            DSNNum.Text = "O"

            AddressNum.Text = "P"
            CityNum.Text = "Q"
            StateNum.Text = "R"
            ZipNum.Text = "S"
            RecieveEmailNum.Text = "T"

        End Sub

        Protected Sub PopulateRanks()
            RankSelect.DataSource = LookupService.GetRanksAndGrades()
            RankSelect.DataTextField = "Title"
            RankSelect.DataValueField = "Id"
            RankSelect.DataBind()
        End Sub

        Protected Sub PopulateRoles()

            Dim groups As IList(Of UserGroup) = LookupService.GetGroupsByCompo(6)

            RoleSelect.DataSource = From g In groups Where g.CanBeRequested = True Select g
            RoleSelect.DataTextField = "Description"
            RoleSelect.DataValueField = "Id"

            RoleSelect.DataBind()

        End Sub

        Protected Sub PopulateStates()
            cbState.DataSource = LookupService.GetStates()
            cbState.DataTextField = "Name"
            cbState.DataValueField = "Id"
            cbState.DataBind()

            cbState.Items.Insert(0, New ListItem("", ""))
        End Sub

        Protected Sub SaveUser()

            Dim user As AppUser

            Dim dao As IUserDao = New NHibernateDaoFactory().GetUserDao()

            user = UserService.GetByEDIPIN(_edipin)

            If (user Is Nothing) Then
                user = New AppUser()
            End If

            user.EDIPIN = _edipin
            user.SSN = String.Empty

            'the user is a not service member, so get all data
            user.FirstName = Server.HtmlEncode(FirstNameBox.Text.Trim)
            user.MiddleName = Server.HtmlEncode(MIddleNameBox.Text.Trim)
            user.LastName = Server.HtmlEncode(LastNameBox.Text.Trim)

            If (TitleSelect.SelectedIndex > 0) Then
                user.Title = TitleSelect.SelectedValue
            Else
                user.Title = Nothing
            End If

            user.Rank = New UserRank()
            user.Rank.SetId(RankSelect.SelectedValue)

            If (user.Id = 0) Then
                user.Status = AccessStatus.None
                user.Username = dao.GetUserName(user.FirstName, user.LastName)
            End If

            If SrcUnitIdHdn.Text <> "" Then
                Dim newUnit As Unit = New NHibernateDaoFactory().GetUnitDao().FindById(CInt(SrcUnitIdHdn.Text.Trim))
                If newUnit IsNot Nothing Then
                    user.Unit = newUnit
                End If
            End If

            'now get the common data
            user.Phone = Server.HtmlEncode(PhoneBox.Text.Trim)
            user.DSN = Server.HtmlEncode(DsnBox.Text.Trim)

            If (user.Address Is Nothing) Then
                user.Address = New Address
            End If

            user.Address.Street = Server.HtmlEncode(txtAddress.Text.Trim)
            user.Address.City = Server.HtmlEncode(txtCity.Text.Trim)
            user.Address.State = cbState.SelectedValue
            user.Address.Zip = Server.HtmlEncode(txtZip.Text.Trim)

            user.Component = CompoSelect.SelectedValue
            user.AccountExpiration = DateTime.Now.AddYears(1)
            user.Email = Server.HtmlEncode(EmailBox.Text.Trim)
            user.Email2 = Server.HtmlEncode(Email2Box.Text.Trim)
            user.Email3 = Server.HtmlEncode(Email3Box.Text.Trim)

            user.ReceiveEmail = chkReceiveEmail.Checked

            If (user.Status = AccessStatus.None) Then
                user.CreatedDate = DateTime.Now
            End If

            'create the role
            If (user.CurrentRole Is Nothing) Then
                Dim role As New UserRole()
                role.Status = AccessStatus.Pending
                role.Group = New UserGroup(CInt(RoleSelect.SelectedValue))
                role.Active = True
                role.User = user
                user.AllRoles.Add(role)
                user.CurrentRole = role
            Else
                Dim newGroup As Integer = 0
                Integer.TryParse(RoleSelect.SelectedValue, newGroup)

                If (newGroup <> user.CurrentRole.Group.Id) Then
                    Dim group As UserGroup = New NHibernateDaoFactory().GetUserGroupDao().GetById(newGroup)
                    user.CurrentRole.Group = group
                End If
            End If

            user.ModifiedDate = DateTime.Now

            dao.SaveOrUpdate(user)
            dao.CommitChanges()

            If (user.Id > 0) Then
                Response.Redirect("~/Public/Register3.aspx?", True)
            End If

        End Sub

        Protected Sub VerifyAccess()

            If (SESSION_EDIPIN Is Nothing OrElse Session("signed") Is Nothing) Then
                'no ssn on so this is not a legit request
                Response.Redirect("~/Default.aspx")
            End If

            _edipin = SESSION_EDIPIN

        End Sub

    End Class

End Namespace