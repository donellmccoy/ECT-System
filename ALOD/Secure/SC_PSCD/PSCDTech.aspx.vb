Imports ALOD.Core.Domain.Common
Imports ALOD.Core.Domain.Lookup
Imports ALOD.Core.Domain.Modules.SpecialCases
Imports ALOD.Core.Domain.Workflow
Imports ALOD.Core.Interfaces.DAOInterfaces
Imports ALOD.Core.Utils
Imports ALOD.Data
Imports ALOD.Logging
Imports ALOD.Web.UserControls
Imports ALODWebUtility.Common

Namespace Web.Special_Case.PSCD

    Partial Class Secure_PSCD_PSCDTech
        Inherits System.Web.UI.Page

        Private _dao As ISpecialCaseDAO
        Private _lookupDao As ILookupDao
        Private sc As SC_PSCD

        ReadOnly Property SCDao() As ISpecialCaseDAO
            Get
                If (_dao Is Nothing) Then
                    _dao = New NHibernateDaoFactory().GetSpecialCaseDAO()
                End If

                Return _dao
            End Get
        End Property

        Public Property UserCanEdit() As Boolean
            Get
                If (ViewState("UserCanEdit") Is Nothing) Then
                    ViewState("UserCanEdit") = False
                End If
                Return CBool(ViewState("UserCanEdit"))
            End Get
            Set(value As Boolean)
                ViewState("UserCanEdit") = value
            End Set
        End Property

        Protected ReadOnly Property CalendarImage() As String
            Get
                Return Me.ResolveClientUrl("~/App_Themes/" + Page.Theme + "/Images/Calendar.gif")
            End Get
        End Property

        Protected ReadOnly Property LookupDao() As ILookupDao
            Get
                If (_lookupDao Is Nothing) Then
                    _lookupDao = New NHibernateDaoFactory().GetLookupDao()
                End If

                Return _lookupDao
            End Get
        End Property

        Protected ReadOnly Property ModuleType() As ModuleType
            Get
                Return ModuleType.SpecCasePSCD
            End Get
        End Property

        Protected ReadOnly Property Navigator() As TabNavigator
            Get
                Return Me.Master.Navigator
            End Get
        End Property

        Protected ReadOnly Property refId() As Integer
            Get
                Return Integer.Parse(Request.QueryString("refId"))
            End Get
        End Property

        Protected ReadOnly Property SpecCase() As SC_PSCD
            Get
                If (sc Is Nothing) Then
                    sc = SCDao.GetById(refId)
                End If

                Return sc
            End Get
        End Property

        Protected ReadOnly Property TabControl() As TabControls
            Get
                Return Master.TabControl
            End Get
        End Property

        Protected Sub Page_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
            AddHandler Me.Master.TabClick, AddressOf TabButtonClicked
        End Sub

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
            ucICDCodeControl.Initialilze(Me)
            ucICD7thCharacterControl.Initialize(ucICDCodeControl)
            UserCanEdit = GetAccess(Navigator.PageAccess, True)

            If (Not Page.IsPostBack) Then

                InitControls()

                LogManager.LogAction(ModuleType.SpecCasePSCD, UserAction.ViewPage, refId, "View Page: PS Med Tech")

            End If
            InitMemberCategorySelectDropwdownRule()
            If (Not UserCanEdit) Then
                MemberCategorySelect.Enabled = False
                IAW_AFI_RadioButton.Enabled = False
                page_readOnly.Value = "0"
            Else
                page_readOnly.Value = "1"
            End If
        End Sub

        Private Sub InitControls()

            SetInputFormatRestriction(Page, DiagnosisTextBox, FormatRestriction.AlphaNumeric, PERMITTED_SPECIAL_CHAR_INPUT)
            InitMemberStatusSelectDropwdownList()
            InitMemberCategorySelectDropwdownList()
            InitRMUSelectDropwdownList()
            InitDatePickerCSSClasses()
            LoadData()

            If Not String.IsNullOrEmpty(SpecCase.PocEmail) Then
                txtPOCEmailLabel.Text = SpecCase.PocEmail
            End If
            If Not String.IsNullOrEmpty(SpecCase.PocPhoneDSN) Then
                txtPOCPhoneLabel.Text = SpecCase.PocPhoneDSN
            End If
            If Not String.IsNullOrEmpty(SpecCase.PocRankAndName) Then
                txtPOCNameLabel.Text = SpecCase.PocRankAndName
            End If

            ' Check permission
            If (Not UserCanEdit) Then
                txtPOCNameLabel.Enabled = False
                txtPOCPhoneLabel.Enabled = False
                txtPOCEmailLabel.Enabled = False
                MemberStatusSelect.Enabled = False
                MemberCategorySelect.Enabled = False
                ucICDCodeControl.DisplayReadOnly(True)
                ucICD7thCharacterControl.DisplayReadOnly()
                DiagnosisTextBox.Enabled = False
                DiagnosisTextBox.ReadOnly = True
                medicalOpinionTxtBox.Enabled = False
                medicalOpinionTxtBox.ReadOnly = True
                IAW_AFI_RadioButton.Enabled = True
                RMUSelect.Enabled = False
                DurationOfServiceFromDate.Enabled = False
                DurationOfServiceToDate.Enabled = False
                FacilityLocationTextBox.Enabled = False
                InitialTreatmentDate.Enabled = False
                InitialTreatmentTime.Enabled = False
                AccidentOrHistoryDetailsTextBox.Enabled = False
                DurationOfServiceToTime.Enabled = False
                DurationOfServiceFromTime.Enabled = False
            End If

        End Sub

        Private Sub InitDatePickerCSSClasses()
            DurationOfServiceFromDate.CssClass = "datePicker"
            DurationOfServiceToDate.CssClass = "datePickerFuture"
            InitialTreatmentDate.CssClass = "datePicker"
        End Sub

        Private Sub InitMemberCategorySelectDropwdownList()
            MemberCategorySelect.DataSource = From n In LookupDao.GetCategory() Select n Where n.Value < 5
            MemberCategorySelect.DataTextField = "Name"
            MemberCategorySelect.DataValueField = "Value"
            MemberCategorySelect.DataBind()
            InsertDropDownListEmptyValue(MemberCategorySelect, "--- Select One ---")
        End Sub

        Private Sub InitMemberCategorySelectDropwdownRule()

            If (MemberStatusSelect.SelectedItem.Text.Contains("Active Duty")) Then
                InsertDropDownListZeroValue(MemberCategorySelect, "N/A")
                MemberCategorySelect.SelectedValue = 0
                MemberCategorySelect.Enabled = False
            Else
                RemoveDropDownListValue(MemberCategorySelect, "N/A")
                MemberCategorySelect.Enabled = True
            End If
        End Sub

        Private Sub InitMemberStatusSelectDropwdownList()
            MemberStatusSelect.DataSource = From n In LookupDao().GetPSCDMemberStatus() Select n
            MemberStatusSelect.DataTextField = "Name"
            MemberStatusSelect.DataValueField = "Value"
            MemberStatusSelect.DataBind()
            InsertDropDownListEmptyValue(MemberStatusSelect, "--- Select One ---")
        End Sub

        Private Sub InitRMUSelectDropwdownList()
            RMUSelect.DataSource = From n In LookupDao().GetRMU() Select n
            RMUSelect.DataTextField = "Name"
            RMUSelect.DataValueField = "Value"
            RMUSelect.DataBind()
            InsertDropDownListEmptyValue(RMUSelect, "--- Select One ---")
        End Sub

        Private Sub LoadData()
            'Member Status
            If (Not IsNothing(SpecCase.MemberStatus)) Then
                MemberStatusSelect.SelectedValue = SpecCase.MemberStatus
            End If
            'Member Category
            If (Not IsNothing(SpecCase.MemberCategory)) Then
                MemberCategorySelect.SelectedValue = SpecCase.MemberCategory
            End If

            If (Not IsNothing(SpecCase.RMU)) Then
                RMUSelect.SelectedValue = SpecCase.RMU
            End If

            'ICD9
            If (Not IsNothing(SpecCase.ICD9Code)) Then
                ucICDCodeControl.InitializeHierarchy(SpecCase.ICD9Code)

                If (ucICDCodeControl.IsValidICDCode(SpecCase.ICD9Code)) Then
                    ucICDCodeControl.UpdateICDCodeDiagnosisLabel(SpecCase.ICD9Code, UserCanEdit)

                End If

                If (Not String.IsNullOrEmpty(SpecCase.ICD7thCharacter)) Then
                    ucICD7thCharacterControl.InitializeCharacters(SpecCase.ICD9Code, SpecCase.ICD7thCharacter)
                    ucICD7thCharacterControl.Update7thCharacterLabel(SpecCase.ICD9Code, SpecCase.ICD7thCharacter)
                Else
                    ucICD7thCharacterControl.InitializeCharacters(SpecCase.ICD9Code, String.Empty)
                End If
            Else
                ucICD7thCharacterControl.InitializeCharacters(0, String.Empty)
            End If

            If (Not IsNothing(SpecCase.ICD9Description)) Then
                DiagnosisTextBox.Text = Server.HtmlDecode(SpecCase.ICD9Diagnosis)
            End If

            If (Not IsDBNull(SpecCase.DurationOfServiceFrom)) Then
                If (Not IsDBNull(SpecCase.DurationOfServiceTo)) Then
                    If (SpecCase.DurationOfServiceFrom IsNot Nothing) Then
                        If (SpecCase.DurationOfServiceTo IsNot Nothing) Then
                            If (SpecCase.DurationOfServiceFrom.HasValue) Then
                                DurationOfServiceFromDate.Text = SpecCase.DurationOfServiceFrom.Value.ToString(DATE_FORMAT)
                                DurationOfServiceFromTime.Text = SpecCase.DurationOfServiceFrom.Value.ToString(HOUR_FORMAT)
                            End If
                            If (SpecCase.DurationOfServiceTo.HasValue) Then
                                DurationOfServiceToDate.Text = SpecCase.DurationOfServiceTo.Value.ToString(DATE_FORMAT)
                                DurationOfServiceToTime.Text = SpecCase.DurationOfServiceTo.Value.ToString(HOUR_FORMAT)
                            End If
                        End If
                    End If
                End If
            End If

            If (Not IsDBNull(SpecCase.InitialTreatmentDate)) Then
                If (SpecCase.InitialTreatmentDate IsNot Nothing) Then
                    If (SpecCase.InitialTreatmentDate.HasValue) Then
                        InitialTreatmentDate.Text = SpecCase.InitialTreatmentDate.Value.ToString(DATE_FORMAT)
                        InitialTreatmentTime.Text = SpecCase.InitialTreatmentDate.Value.ToString(HOUR_FORMAT)
                    End If
                End If
            End If

            If (Not SpecCase.AccidentOrHistoryDetails Is Nothing) Then
                AccidentOrHistoryDetailsTextBox.Text = SpecCase.AccidentOrHistoryDetails
            End If

            If (Not SpecCase.FacilityLocation Is Nothing) Then
                FacilityLocationTextBox.Text = SpecCase.FacilityLocation
            End If

            If (Not SpecCase.IAWAFI Is Nothing) Then
                IAW_AFI_RadioButton.SelectedIndex = SpecCase.IAWAFI
            End If
        End Sub

        Private Sub Save_Click(ByRef sender As Object, ByRef e As TabNavigationEventArgs)
            If (Navigator.CurrentStep.IsReadOnly) Then
                Exit Sub
            End If

            If (e.ButtonType = NavigatorButtonType.Save OrElse
                e.ButtonType = NavigatorButtonType.NavigatedAway OrElse
                e.ButtonType = NavigatorButtonType.NextStep OrElse
                e.ButtonType = NavigatorButtonType.PreviousStep) Then

                SaveData()
            End If
        End Sub

        Private Sub SaveData()
            If (Not UserCanEdit) Then
                Exit Sub
            End If

            ' Member Status
            If (Not IsNothing(MemberStatusSelect.SelectedValue)) Then
                SpecCase.MemberStatus = MemberStatusSelect.SelectedValue
            End If
            If (Not IsNothing(MemberCategorySelect.SelectedValue)) Then
                SpecCase.MemberCategory = MemberCategorySelect.SelectedValue
            End If
            If (Not RMUSelect.SelectedValue = "") Then
                If (Not IsNothing(RMUSelect.SelectedValue)) Then
                    SpecCase.RMU = RMUSelect.SelectedValue
                End If
            End If
            ' ICD9
            If (ucICDCodeControl.IsICDCodeSelected()) Then
                SpecCase.ICD9Code = ucICDCodeControl.SelectedICDCodeID
            End If
            If (ucICD7thCharacterControl.Is7thCharacterSelected()) Then
                SpecCase.ICD7thCharacter = ucICD7thCharacterControl.Selected7thCharacter
            Else
                SpecCase.ICD7thCharacter = Nothing
            End If
            If (IAW_AFI_RadioButton.SelectedIndex >= 0) Then
                SpecCase.IAWAFI = IAW_AFI_RadioButton.SelectedIndex
            End If

            If (Not FacilityLocationTextBox.Text = "") Then
                SpecCase.FacilityLocation = FacilityLocationTextBox.Text
            End If

            Try
                If (CheckDate(DurationOfServiceFromDate)) Then
                    If (DurationOfServiceFromTime.Text.Trim.Length > 0) Then
                        SpecCase.DurationOfServiceFrom = Server.HtmlEncode(ParseDateAndTime(DurationOfServiceFromDate.Text.Trim + " " + DurationOfServiceFromTime.Text.Trim))
                    Else
                        SpecCase.DurationOfServiceFrom = Server.HtmlEncode(DateTime.Parse(DurationOfServiceFromDate.Text.Trim))
                    End If
                Else
                    SpecCase.DurationOfServiceFrom = Nothing
                End If
            Catch ex As Exception
                SpecCase.DurationOfServiceFrom = Nothing
            End Try

            If (Not AccidentOrHistoryDetailsTextBox.Text = "") Then
                SpecCase.AccidentOrHistoryDetails = AccidentOrHistoryDetailsTextBox.Text
            End If

            If Not String.IsNullOrEmpty(txtPOCPhoneLabel.Text) Then
                SpecCase.PocPhoneDSN = Server.HtmlEncode(txtPOCPhoneLabel.Text)
            End If

            If Not String.IsNullOrEmpty(txtPOCEmailLabel.Text) Then
                SpecCase.PocEmail = Server.HtmlEncode(txtPOCEmailLabel.Text)
            End If
            If Not String.IsNullOrEmpty(txtPOCNameLabel.Text) Then
                SpecCase.PocRankAndName = Server.HtmlEncode(txtPOCNameLabel.Text)
            End If

            Try
                If (CheckDate(DurationOfServiceToDate)) Then
                    If (DurationOfServiceToTime.Text.Trim.Length > 0) Then
                        SpecCase.DurationOfServiceTo = Server.HtmlEncode(ParseDateAndTime(DurationOfServiceToDate.Text.Trim + " " + DurationOfServiceToTime.Text.Trim))
                    Else
                        SpecCase.DurationOfServiceTo = Server.HtmlEncode(DateTime.Parse(DurationOfServiceToDate.Text.Trim))
                    End If
                Else
                    SpecCase.DurationOfServiceTo = Nothing
                End If
            Catch ex As Exception
                SpecCase.DurationOfServiceTo = Nothing
            End Try

            Try
                If (CheckDate(InitialTreatmentDate)) Then
                    If (InitialTreatmentTime.Text.Trim.Length > 0) Then
                        SpecCase.InitialTreatmentDate = Server.HtmlEncode(ParseDateAndTime(InitialTreatmentDate.Text.Trim + " " + InitialTreatmentTime.Text.Trim))
                    Else
                        SpecCase.InitialTreatmentDate = Server.HtmlEncode(DateTime.Parse(InitialTreatmentDate.Text.Trim))
                    End If
                Else
                    SpecCase.InitialTreatmentDate = Nothing
                End If
            Catch ex As Exception
                SpecCase.InitialTreatmentDate = Nothing
            End Try

            If (Not IsNothing(SpecCase.ICD9Code)) Then
                ucICDCodeControl.UpdateICDCodeDiagnosisLabel(SpecCase.ICD9Code)
            End If

            SpecCase.ICD9Description = ucICDCodeControl.ICDCodeDiagnosisLabelText
            SpecCase.ICD9Diagnosis = Server.HtmlEncode(DiagnosisTextBox.Text.Trim())

            SCDao.SaveOrUpdate(SpecCase)
            SCDao.CommitChanges()
        End Sub

        Private Sub TabButtonClicked(ByRef sender As Object, ByRef e As TabNavigationEventArgs)
            If (Navigator.CurrentStep.IsReadOnly) Then
                Exit Sub
            End If

            If (e.ButtonType = NavigatorButtonType.Save OrElse
                e.ButtonType = NavigatorButtonType.NavigatedAway OrElse
                e.ButtonType = NavigatorButtonType.NextStep OrElse
                e.ButtonType = NavigatorButtonType.PreviousStep) Then

                SaveData()
            End If
        End Sub

    End Class

End Namespace