Imports ALOD.Core.Domain.Lookup
Imports ALOD.Core.Domain.ServiceMembers
Imports ALOD.Core.Domain.Workflow
Imports ALOD.Core.Interfaces.DAOInterfaces
Imports ALOD.Core.Utils
Imports ALOD.Core.Utils.RegexValidation
Imports ALOD.Data
Imports ALOD.Data.Services
Imports ALOD.Logging
Imports ALODWebUtility.Common

Namespace Web.Admin

    Partial Class Secure_Shared_Admin_AddMember
        Inherits System.Web.UI.Page

        Protected Const KEY_ERROR_MESSAGE As String = "ERROR_MESSAGE"
        Protected Const KEY_FEEDBACK_MESSAGE As String = "FEEDBACK_MESSAGE"
        Protected Const MSG_DIFFERENT_SSN As String = "Different SSN"
        Protected Const MSG_ERROR_ADDING_MEMBER As String = "An error occured saving the member"
        Protected Const MSG_INVALID_SSN As String = "Invalid SSN"
        Protected Const MSG_MEMBER_ADDED As String = "Member Added"
        Protected Const MSG_SSN_FOUND As String = "SSN already exists in the system.  Please check the SSN and try again"

        Protected ReadOnly Property CalendarImage() As String
            Get
                Return Me.ResolveClientUrl("~/App_Themes/" + Page.Theme + "/Images/Calendar.gif")
            End Get
        End Property

        Protected Property ErrorMessage() As String
            Get
                If (ViewState(KEY_ERROR_MESSAGE) Is Nothing) Then
                    Return String.Empty
                End If
                Return CStr(ViewState(KEY_ERROR_MESSAGE))
            End Get
            Set(ByVal value As String)
                ViewState(KEY_ERROR_MESSAGE) = value
            End Set
        End Property

        Protected Property FeedbackMessage() As String
            Get
                If (ViewState(KEY_FEEDBACK_MESSAGE) Is Nothing) Then
                    Return String.Empty
                End If
                Return CStr(ViewState(KEY_FEEDBACK_MESSAGE))
            End Get
            Set(ByVal value As String)
                ViewState(KEY_FEEDBACK_MESSAGE) = value
            End Set
        End Property

        Protected Sub InitUnitList()

            If (UnitSelect.Items.Count > 0) Then
                Exit Sub
            End If

            Dim units = From u In LookupService.GetChildUnits(SESSION_UNIT_ID, SESSION_REPORT_VIEW)
                        Select u
                        Order By u.Name

            UnitSelect.DataSource = units
            UnitSelect.DataBind()

        End Sub

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

            Dim lkupDAO As ILookupDao
            lkupDAO = New NHibernateDaoFactory().GetLookupDao()

            SetInputFormatRestriction(Page, SsnSearchBox, FormatRestriction.Numeric)
            SetInputFormatRestriction(Page, SsnSearchBox2, FormatRestriction.Numeric)
            SetDefaultButton(SsnSearchBox, SearchButton)
            DobValidator.MaximumValue = DateTime.Now.ToString("MM/dd/") & (DateTime.Now.Year() - 17)

            If (Not IsPostBack) Then
                RankSelect.DataSource = From n In lkupDAO.GetRanks() Select n Where Not n.Name.Equals("Unknown")
                RankSelect.DataTextField = "Name"
                RankSelect.DataValueField = "Value"
                RankSelect.DataBind()
            End If

        End Sub

        Protected Sub Page_PreRender(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.PreRender

            ErrorPanel.Visible = ErrorMessage.Length > 0
            ErrorMessageLabel.Text = ErrorMessage

            FeedbackMessageLabel.Text = FeedbackMessage
            FeedbackPanel.Visible = FeedbackMessage.Length > 0

        End Sub

        Protected Sub ResetButton_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles ResetButton.Click
            ResetInputPanel()
        End Sub

        Protected Sub ResetInputPanel()
            SsnLabel.Text = String.Empty
            FirstNameTextBox.Text = String.Empty
            LastNameTextBox.Text = String.Empty
            MiddleNameTextBox.Text = String.Empty
            DobTextBox.Text = String.Empty
            RankSelect.SelectedIndex = -1
            UnitSelect.SelectedIndex = -1

            InputPanel.Visible = False
            SearchPanel.Visible = True
        End Sub

        Protected Sub SaveMemberButton_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles SaveMemberButton.Click

            Dim errors As New StringCollection()

            Dim lastName As String = LastNameTextBox.Text.Trim.ToUpper()
            Dim firstName As String = FirstNameTextBox.Text.Trim.ToUpper()
            Dim middleName As String = MiddleNameTextBox.Text.Trim.ToUpper()
            Dim unitId As Integer = 0
            Dim rankId As Integer = 0
            Dim dob As Date
            Dim current = DateTime.Now
            Dim MaxDate = New Date(current.Year - 17, current.Month, current.Day)

            Date.TryParse(DobTextBox.Text.Trim, dob)
            Integer.TryParse(UnitSelect.SelectedValue, unitId)
            Integer.TryParse(RankSelect.SelectedValue, rankId)

            If (lastName.Length = 0) OrElse (firstName.Length = 0) Then
                errors.Add(Resources.Messages.REQUIRED_FIELD_MEMBER_NAME)
            End If

            If (unitId = 0) Then
                errors.Add(Resources.Messages.REQUIRED_FIELD_MEMBER_UNIT)
            End If

            If (rankId < 0) Then
                errors.Add(Resources.Messages.REQUIRED_FIELD_INVALID_GRADE)
            End If

            If (dob.Ticks = 0) Then
                errors.Add(Resources.Messages.REQUIRED_FIELD_MEMBER_DOB)
            End If

            If (dob > MaxDate) Then
                errors.Add(Resources.Messages.REQUIRED_VALID_MEMBER_DOB)
            End If

            If (errors.Count > 0) Then
                ErrorListPanel.Visible = True
                ValidationList.Visible = True
                ValidationList.DataSource = errors
                ValidationList.DataBind()
                Exit Sub
            Else
                ErrorListPanel.Visible = False
            End If

            Dim dao As IMemberDao = New NHibernateDaoFactory().GetMemberDao()
            Dim member As New ServiceMember(SsnLabel.Text)

            member.LastName = lastName
            member.FirstName = firstName
            member.MiddleName = middleName
            member.Component = CompoSelect.SelectedValue
            member.DateOfBirth = dob

            Dim unit As ALOD.Core.Domain.Users.Unit = LookupService.GetUnitById(unitId)
            Dim rank As UserRank = LookupService.GetRank(rankId)

            If (unit Is Nothing) OrElse (rank Is Nothing) Then
                Exit Sub
            End If

            member.Unit = unit
            member.Rank = rank

            Dim changes As New ChangeSet()
            changes.Add("Member Data", "Full Member Record", String.Empty, member.FullName)

            Try
                dao.Save(member)
                dao.CommitChanges()
                Dim logId As Integer = LogManager.LogAction(ModuleType.System, UserAction.ModifyMemberData, SESSION_USER_ID, "Added member data for  " + member.FullName + "  SSAN " + member.SSN)
                changes.Save(logId)
                ErrorMessage = String.Empty
                FeedbackMessage = MSG_MEMBER_ADDED
                ResetInputPanel()
                SsnSearchBox.Text = String.Empty
                SsnSearchBox2.Text = String.Empty
            Catch ex As Exception
                FeedbackMessage = String.Empty
                ErrorMessage = MSG_ERROR_ADDING_MEMBER
            End Try

        End Sub

        Protected Sub SearchButton_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles SearchButton.Click

            Dim ssn As String = SsnSearchBox.Text.Trim.Replace("-", "")
            Dim ssn2 As String = SsnSearchBox2.Text.Trim.Replace("-", "")

            If (Not IsValidSSN(ssn)) Then
                ErrorMessage = MSG_INVALID_SSN
                Exit Sub
            End If

            If (Not String.Equals(ssn, ssn2)) Then
                ErrorMessage = MSG_DIFFERENT_SSN
                Exit Sub
            End If

            'we have a valid ssn, does this member already exist in the system?
            Dim member As ServiceMember = LookupService.GetServiceMemberBySSN(ssn)

            If (member IsNot Nothing) Then
                'we did find this member, don't add them again
                ErrorMessage = MSG_SSN_FOUND
                Exit Sub
            End If

            ErrorMessage = String.Empty

            SsnLabel.Text = ssn
            SearchPanel.Visible = False
            InputPanel.Visible = True

            InitUnitList()

        End Sub

    End Class

End Namespace