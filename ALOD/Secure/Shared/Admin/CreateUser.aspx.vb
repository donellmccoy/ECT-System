Imports ALOD.Core.Domain.Lookup
Imports ALOD.Core.Domain.ServiceMembers
Imports ALOD.Core.Domain.Users
Imports ALOD.Core.Domain.Workflow
Imports ALOD.Core.Interfaces.DAOInterfaces
Imports ALOD.Core.Utils
Imports ALOD.Core.Utils.RegexValidation
Imports ALOD.Data
Imports ALOD.Data.Services
Imports ALOD.Logging
Imports ALODWebUtility.Common

Namespace Web.Admin

    Partial Class Secure_Shared_Admin_CreateUser
        Inherits System.Web.UI.Page

        'member: 310429547
        'nonmember: 43243243F

        Protected Const MSG_DIFFERENT_SSN As String = "Different SSN"

        'Protected _compo As Integer = 6 'when we have more that one compo, use dropdown selection to get compo
        Protected Const MSG_INVALID_SSN As String = "Invalid SSN"

        Protected _ssn As String = ""

        Public ReadOnly Property Component() As String
            Get
                Return CompoSelect.SelectedValue ' Session(SESSIONKEY_COMPO) '"6"
            End Get
        End Property

        Public Property NewActSSN() As String
            Get
                Return Session("NewActSSN")

            End Get
            Set(ByVal value As String)
                Session("NewActSSN") = value
            End Set

        End Property

#Region "Load"

        Public Sub PopulateNonMemberData()

            SetNonMemberLabels()
            FullName.Text = ""
            RankDisplay.Text = ""
            TitleSelect.SelectedValue = ""
            FirstNameBox.Text = ""
            MIddleNameBox.Text = ""
            LastNameBox.Text = ""
            RankSelect.SelectedValue = ""
            UnitTextBox.Text = String.Empty
            RoleSelect.SelectedValue = ""
            EmailBox.Text = String.Empty
            Email2Box.Text = String.Empty
            Email3Box.Text = String.Empty
            PhoneBox.Text = String.Empty
            DsnBox.Text = String.Empty
            txtAddress.Text = String.Empty
            txtCity.Text = String.Empty
            StateSelect.SelectedValue = ""
            txtZip.Text = String.Empty
            chkReceiveEmail.Checked = False

        End Sub

        Public Sub SetMemberLabels()

            ChangeUnitButton.Text = "Change"
            Lbl1.Text = "D"
            UnitNum.Text = "E"
            UserRoleNum.Text = "F"
            EmailNum.Text = "G"
            EmailNum2.Text = "H"
            EmailNum3.Text = "I"
            PhoneNum.Text = "J"
            DSNNum.Text = "K"
            AddressNum.Text = "L"
            CityNum.Text = "M"
            StateNum.Text = "N"
            ZipNum.Text = "O"
            RecieveEmailNum.Text = "P"
            ActnNum.Text = "Q"

        End Sub

        Public Sub SetNonMemberLabels()

            ChangeUnitButton.Text = "Select"
            Lbl1.Text = "G"
            UnitNum.Text = "H"
            UserRoleNum.Text = "I"
            EmailNum.Text = "J"
            EmailNum2.Text = "K"
            EmailNum3.Text = "L"
            PhoneNum.Text = "M"
            DSNNum.Text = "N"
            AddressNum.Text = "O"
            CityNum.Text = "P"
            StateNum.Text = "Q"
            ZipNum.Text = "R"
            RecieveEmailNum.Text = "S"
            ActnNum.Text = "T"

        End Sub

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

            If (Not IsPostBack) Then
                SetInputFormatRestriction(Page, SSNTextBox, FormatRestriction.AlphaNumeric, "-")
                SetInputFormatRestriction(Page, SsnSearchBox2, FormatRestriction.AlphaNumeric, "-")
                ChangeUnitButton.Attributes.Add("onclick", "showSearcher('" + "Select New Unit" + "','" + newUnitIDLabel.ClientID + "','" + UnitTextBox.ClientID + "'); return false;")
                PopulateStates()
                PopulateRoles()
                PopulateRanks()
                If UserHasPermission("sysAdmin") Then
                    EDIPINtxt.Enabled = True
                Else
                    EDIPINtxt.Enabled = False
                End If
                If (Session("NewActSSN") IsNot Nothing) Then
                    srchSSN.Visible = False
                    NameDisplayPanel.Visible = True
                    MemberDataPanel.Visible = True
                    PopulateData(NewActSSN)
                Else
                    lnkManageMembers.Visible = False
                End If
            End If

        End Sub

        Protected Sub PopulateCommonData(ByVal user As AppUser)

            If (Not String.IsNullOrEmpty(user.Email)) Then
                EmailBox.Text = user.Email
            End If
            If (Not String.IsNullOrEmpty(user.Email2)) Then
                Email2Box.Text = user.Email2
            End If
            If (Not String.IsNullOrEmpty(user.Email3)) Then
                Email3Box.Text = user.Email3
            End If
            If (Not String.IsNullOrEmpty(user.Phone)) Then
                PhoneBox.Text = user.Phone
            End If

            If (Not String.IsNullOrEmpty(user.DSN)) Then
                DsnBox.Text = user.DSN
            End If

            If (user.Address IsNot Nothing) Then
                If (Not String.IsNullOrEmpty(user.Address.Street)) Then
                    txtAddress.Text = user.Address.Street
                End If

                If (Not String.IsNullOrEmpty(user.Address.City)) Then
                    txtCity.Text = user.Address.City
                End If

                If (Not String.IsNullOrEmpty(user.Address.State)) Then
                    SetDropdownByValue(StateSelect, user.Address.State)
                End If

                If (Not String.IsNullOrEmpty(user.Address.Zip)) Then
                    txtZip.Text = user.Address.Zip
                End If
            End If

            chkReceiveEmail.Checked = user.ReceiveEmail

        End Sub

        Protected Sub PopulateData(ByVal ssn As String)
            Dim user As AppUser = UserService.GetBySSN(ssn)
            Dim data As ServiceMember = LookupService.GetServiceMemberBySSN(ssn)

            If user IsNot Nothing Then
                CleanPanel()
                userFoundLabel.Visible = True
                Exit Sub
            End If
            userFoundLabel.Visible = False
            If (user Is Nothing) Then

                user = New AppUser()

                'The user does not yet have an account, so try and import from MILPDS
                If (data IsNot Nothing) Then
                    user.Import(data)
                End If

            End If

            'we will always have an SSN. The user did not get this far without a CAC and associated SSN
            user.SSN = ssn
            If user.Unit IsNot Nothing Then
                UnitTextBox.Text = user.Unit.Name + " (" + user.Unit.PasCode + ")"
            End If
            SSNLabel.Text = "***-**-" + user.SSN.Substring(5, 4)

            'user will be  a member
            If (data IsNot Nothing) Then
                PopulateMemberData(user)
            Else
                PopulateNonMemberData()
            End If

            PopulateCommonData(user)

        End Sub

        Protected Sub PopulateMemberData(ByVal user As AppUser)
            SetMemberLabels()
            FullName.Text = user.FullName
            RankDisplay.Text = user.Rank.Title
        End Sub

        Protected Sub PopulateRanks()
            RankSelect.DataSource = LookupService.GetRanksAndGrades()
            RankSelect.DataTextField = "Title"
            RankSelect.DataValueField = "Id"
            RankSelect.DataBind()
            RankSelect.Items.Insert(0, New ListItem("-Select-", ""))
        End Sub

        Protected Sub PopulateRoles()

            Dim groups As IList(Of UserGroup) = LookupService.GetGroupsByCompo(6) 'Component)
            RoleSelect.DataSource = From g In groups Where g.CanBeRequested = True Select g
            RoleSelect.DataTextField = "Description"
            RoleSelect.DataValueField = "Id"
            RoleSelect.DataBind()
            RoleSelect.Items.Insert(0, New ListItem("-Select-", ""))

        End Sub

        Protected Sub PopulateStates()
            StateSelect.DataSource = LookupService.GetStates()
            StateSelect.DataBind()
            StateSelect.Items.Insert(0, New ListItem("-Select-", ""))

        End Sub

#End Region

#Region "UserAction"

        Public Sub CreateUser()

            Dim user As AppUser = UserService.GetBySSN(NewActSSN)
            Dim data As ServiceMember = LookupService.GetServiceMemberBySSN(NewActSSN)
            Dim dao As IUserDao = New NHibernateDaoFactory().GetUserDao()

            If user IsNot Nothing Then
                CleanPanel()
                userFoundLabel.Visible = True
                Exit Sub
            End If
            If (user Is Nothing) Then
                user = New AppUser()
                'The user does not yet have an account, so try and import from MILPDS
                If (data IsNot Nothing) Then
                    user.Import(data)
                Else
                    user.FirstName = Server.HtmlEncode(FirstNameBox.Text.Trim)
                    user.MiddleName = Server.HtmlEncode(MIddleNameBox.Text.Trim)
                    user.LastName = Server.HtmlEncode(LastNameBox.Text.Trim)
                    If (TitleSelect.SelectedIndex > 0) Then
                        user.Title = Server.HtmlEncode(TitleSelect.SelectedValue)
                    Else
                        user.Title = Nothing
                    End If

                    If (user.Rank Is Nothing) Then
                        user.Rank = New UserRank()
                    End If

                    user.Rank.SetId(Server.HtmlEncode(RankSelect.SelectedValue))

                End If
            End If
            'we will always have an SSN. The user did not get this far without a CAC and associated SSN
            user.SSN = NewActSSN
            If EDIPINtxt.Text.Trim.Length > 0 AndAlso UserHasPermission("sysAdmin") Then
                user.EDIPIN = EDIPINtxt.Text.Trim
            End If
            If (user.Id = 0) Then
                user.Status = AccessStatus.Pending
                user.Username = dao.GetUserName(user.FirstName, user.LastName)
                user.LastLoginDate = Now
                user.AccountExpiration = Now.AddMonths(1)
                'now get the common data
                user.Phone = PhoneBox.Text.Trim
                user.DSN = DsnBox.Text.Trim

                If (user.Address Is Nothing) Then
                    user.Address = New Address
                End If
                user.Address.Street = Server.HtmlEncode(txtAddress.Text.Trim)
                user.Address.City = Server.HtmlEncode(txtCity.Text.Trim)
                user.Address.State = Server.HtmlEncode(StateSelect.SelectedValue)
                user.Address.Zip = Server.HtmlEncode(txtZip.Text.Trim)

                user.Component = Component
                user.AccountExpiration = DateTime.Now.AddYears(1)
                user.Email = Server.HtmlEncode(EmailBox.Text.Trim)
                user.Email2 = Server.HtmlEncode(Email2Box.Text.Trim)
                user.Email3 = Server.HtmlEncode(Email3Box.Text.Trim)

                user.UnitView = 1

                user.ReceiveEmail = chkReceiveEmail.Checked
                If newUnitIDLabel.Text.Trim <> "" Then
                    Dim newUnit As Unit = New NHibernateDaoFactory().GetUnitDao().FindById(CInt(newUnitIDLabel.Text.Trim))
                    If newUnit IsNot Nothing Then
                        user.Unit = newUnit
                    End If
                End If
                Dim role As New UserRole()
                role.Status = AccessStatus.Pending
                role.Group = New UserGroup(CInt(Server.HtmlEncode(RoleSelect.SelectedValue)))
                role.Active = True
                role.User = user
                user.AllRoles.Add(role)
                user.CurrentRole = role

                'log this action
                Dim logId As Integer = LogManager.LogAction(ModuleType.System, UserAction.CreatedNewAccount, user.Id)

                Dim changes As New ChangeSet()
                changes.Add("User Account", "Account Created", "", "")
                changes.Save(logId)

            End If

            dao.SaveOrUpdate(user)
            Session("NewActSSN") = Nothing
            Response.Redirect("~/Secure/Welcome.aspx")

        End Sub

        Protected Sub Create_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles Create.Click
            If NewActSSN IsNot Nothing Then
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
                    CreateUser()
                End If
            Else

                Response.Redirect("~/Secure/Shared/Admin/CreateUser.aspx")
            End If

        End Sub

        Protected Sub lnkManageMembers_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles lnkManageMembers.Click
            Session("NewActSSN") = Nothing
            Response.Redirect("~/Secure/Shared/Admin/MemberData.aspx")
        End Sub

#End Region

#Region "LookUp"

        Public Sub CleanPanel()
            Session("NewActSSN") = Nothing
            srchSSN.Visible = True
            userFoundLabel.Visible = False
            MemberDataPanel.Visible = False
            InvalidSSNLabel.Visible = False
            NotFoundLabel.Visible = False
        End Sub

        Protected Sub LookupButton_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles LookupButton.Click

            NewActSSN = Nothing

            Dim ssn As String = Server.HtmlEncode(SSNTextBox.Text.Trim).Replace("-", "")
            Dim ssn2 As String = Server.HtmlEncode(SsnSearchBox2.Text.Trim).Replace("-", "")

            If (Not IsValidSSN(ssn)) Then
                InvalidSSNLabel.Text = MSG_INVALID_SSN
                InvalidSSNLabel.Visible = True
                Exit Sub
            End If

            If (Not String.Equals(ssn, ssn2)) Then
                InvalidSSNLabel.Text = MSG_DIFFERENT_SSN
                InvalidSSNLabel.Visible = True
                Exit Sub
            End If

            If ssn.Length < 9 Then
                'nothing found
                SSNLabel.Text = ""
                InvalidSSNLabel.Visible = True
                NotFoundLabel.Visible = False
                MemberDataPanel.Visible = False
                Exit Sub
            End If
            Dim user As AppUser = UserService.GetBySSN(ssn)
            If user IsNot Nothing Then
                CleanPanel()
                userFoundLabel.Visible = True
                Exit Sub
            End If

            userFoundLabel.Visible = False
            InvalidSSNLabel.Visible = False

            Dim member As ServiceMember = LookupService.GetServiceMemberBySSN(ssn)
            MemberDataPanel.Visible = True

            If (member Is Nothing) Then
                NotFoundLabel.Visible = True
                NameEntryPanel.Visible = True
                NameDisplayPanel.Visible = False
                NewActSSN = ssn
            Else

                NameEntryPanel.Visible = False
                NameDisplayPanel.Visible = True
                NotFoundLabel.Visible = False
                NewActSSN = ssn
            End If

            PopulateData(NewActSSN)

        End Sub

#End Region

    End Class

End Namespace