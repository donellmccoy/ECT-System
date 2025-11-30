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

Namespace Web.Special_Case.RW

    Partial Class Secure_sc_rw_MedTech
        Inherits System.Web.UI.Page

#Region "Fields..."

        Public FTsubType As String
        Public FTType As String
        Private _associatedSC As SpecialCase
        Private _daoFactory As IDaoFactory
        Private _icd As String = String.Empty
        Private _keyValDao As IKeyValDao
        Private _lookupDao As ILookupDao
        Private _sc As SC_RW = Nothing
        Private _scAccess As Dictionary(Of String, ALOD.Core.Domain.Workflow.PageAccessType)
        Private _scId As Integer = 0
        Private _specCaseDao As ISpecialCaseDAO

#End Region

#Region "Properties..."

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

        Protected ReadOnly Property AssociatedSpecCase As SpecialCase
            Get
                If (_associatedSC Is Nothing) Then
                    If (SpecCase.AssociatedSC.HasValue AndAlso SpecCase.AssociatedSC.Value > 0) Then
                        _associatedSC = SCDao.GetById(SpecCase.AssociatedSC.Value)
                    End If
                End If

                Return _associatedSC
            End Get
        End Property

        Protected ReadOnly Property AssociatedSpecCaseAsFastTrack As SC_FastTrack
            Get
                If (_associatedSC Is Nothing) Then
                    Return Nothing
                End If

                Dim castedAssociatedSpecCase As SC_FastTrack = AssociatedSpecCase

                Return castedAssociatedSpecCase
            End Get
        End Property

        Protected ReadOnly Property AssociatedSpecCaseAsMEB As SC_MEB
            Get
                If (_associatedSC Is Nothing) Then
                    Return Nothing
                End If

                Dim castedAssociatedSpecCase As SC_MEB = AssociatedSpecCase

                Return castedAssociatedSpecCase
            End Get
        End Property

        Protected ReadOnly Property AssociatedSpecCaseAsRS As SC_RS
            Get
                If (_associatedSC Is Nothing) Then
                    Return Nothing
                End If

                Dim castedAssociatedSpecCase As SC_RS = AssociatedSpecCase

                Return castedAssociatedSpecCase
            End Get
        End Property

        Protected ReadOnly Property AssociatedSpecCaseAsWWD As SC_WWD
            Get
                If (_associatedSC Is Nothing) Then
                    Return Nothing
                End If

                Dim castedAssociatedSpecCase As SC_WWD = AssociatedSpecCase

                Return castedAssociatedSpecCase
            End Get
        End Property

        Protected ReadOnly Property CalendarImage() As String
            Get
                Return Me.ResolveClientUrl("~/App_Themes/" + Page.Theme + "/Images/Calendar.gif")
            End Get
        End Property

        Protected ReadOnly Property CaseModuleType() As ModuleType
            Get
                Return ModuleType.SpecCaseRW
            End Get
        End Property

        Protected ReadOnly Property DaoFactory() As IDaoFactory
            Get
                If (_daoFactory Is Nothing) Then
                    _daoFactory = New NHibernateDaoFactory()
                End If

                Return _daoFactory
            End Get
        End Property

        Protected ReadOnly Property KeyValDao() As IKeyValDao
            Get
                If (_keyValDao Is Nothing) Then
                    _keyValDao = DaoFactory.GetKeyValDao()
                End If

                Return _keyValDao
            End Get
        End Property

        Protected ReadOnly Property LookupDao() As ILookupDao
            Get
                If (_lookupDao Is Nothing) Then
                    _lookupDao = DaoFactory.GetLookupDao()
                End If

                Return _lookupDao
            End Get
        End Property

        Protected ReadOnly Property MasterPage() As SC_RWMaster
            Get
                Dim master As SC_RWMaster = CType(Page.Master, SC_RWMaster)
                Return master
            End Get
        End Property

        Protected ReadOnly Property Navigator() As TabNavigator
            Get
                Return MasterPage.Navigator
            End Get
        End Property

        Protected ReadOnly Property RefId() As Integer
            Get
                Return CInt(Request.QueryString("refId"))
            End Get
        End Property

        Protected ReadOnly Property SCDao() As ISpecialCaseDAO
            Get
                If (_specCaseDao Is Nothing) Then
                    _specCaseDao = DaoFactory.GetSpecialCaseDAO()
                End If

                Return _specCaseDao
            End Get
        End Property

        Protected ReadOnly Property SectionList() As Dictionary(Of String, ALOD.Core.Domain.Workflow.PageAccessType)
            Get
                If (_scAccess Is Nothing) Then
                    _scAccess = SpecCase.ReadSectionList(CInt(Session("GroupId")))
                End If
                Return _scAccess
            End Get
        End Property

        Protected ReadOnly Property SpecCase() As SC_RW
            Get
                If (_sc Is Nothing) Then
                    _sc = SCDao.GetById(RefId)
                End If

                Return _sc
            End Get
        End Property

        Protected ReadOnly Property TabControl() As TabControls
            Get
                Return Master.TabControl
            End Get
        End Property

#End Region

#Region "Page Methods..."

        Public Function ValidBoxLength() As Boolean
            Dim IsValid As Boolean = True

            If Not CheckTextLength(txtDiagnosis) Then
                IsValid = False
            End If

            If Not CheckTextLength(txtHospitalizationDetails) Then
                IsValid = False
            End If

            If Not CheckTextLength(txtERorUrgentCareDetails) Then
                IsValid = False
            End If

            If Not CheckTextLength(txtPrognosis) Then
                IsValid = False
            End If

            If Not CheckTextLength(txtTreatment) Then
                IsValid = False
            End If

            If Not CheckTextLength(txtMedicationDosages) Then
                IsValid = False
            End If

            Return IsValid
        End Function

        Protected Sub ddlICDDiagnosis_SelectedIndexChanged(sender As Object, e As EventArgs)
            'SaveFindings()
            'InitControls(True)
            'UpdatePanel2.Update()
        End Sub

        Protected Sub DisplayReadOnly(ByVal isPostBack As Boolean)
            txtRenewalDate.CssClass = String.Empty
            txtRenewalDate.ReadOnly = True
            txtRenewalDate.Enabled = True
            ddlMedGroups.Enabled = False
            ddlRMUs.Enabled = False

            If (Not isPostBack) Then
                ucICDCodeControl.DisplayReadOnly(False)
                ucICD7thCharacterControl.DisplayReadOnly()
            End If

            txtDiagnosis.ReadOnly = True
            txtDiagnosis.Enabled = True
            ddlYearsSatisfactoryService.Enabled = False

            txtDAFSC.ReadOnly = True
            txtDAFSC.Enabled = True
            txtBodyMassIndex.ReadOnly = True
            txtBodyMassIndex.Enabled = True
            ddlMissedWorkDays.Enabled = False
            ddlSpecialistRequired.Enabled = False

            rblEROrUrgentCareVisits.Enabled = False
            txtERorUrgentCareDetails.ReadOnly = True
            txtERorUrgentCareDetails.Enabled = True

            rblHospitalizations.Enabled = False
            txtHospitalizationDetails.ReadOnly = True
            txtHospitalizationDetails.Enabled = True

            ddlIncapacitationRisk.Enabled = False
            ddlFollowUpInterval.Enabled = False
            ddlDAWGRecommendation.Enabled = False

            rblDutyInterference.Enabled = False
            txtPrognosis.ReadOnly = True
            txtPrognosis.Enabled = True
            txtTreatment.ReadOnly = True
            txtTreatment.Enabled = True
            txtMedicationDosages.ReadOnly = True
            txtMedicationDosages.Enabled = True
        End Sub

        Protected Sub DisplayReadWrite(ByVal isPostBack As Boolean)
            txtRenewalDate.ReadOnly = False
            txtRenewalDate.Enabled = True
            ddlMedGroups.Enabled = True
            ddlRMUs.Enabled = True

            If (Not isPostBack) Then
                ucICDCodeControl.DisplayReadWrite(False)
                ucICD7thCharacterControl.DisplayReadWrite()
            End If

            txtDiagnosis.ReadOnly = False
            txtDiagnosis.Enabled = True
            ddlYearsSatisfactoryService.Enabled = True

            txtDAFSC.ReadOnly = False
            txtDAFSC.Enabled = True
            txtBodyMassIndex.ReadOnly = False
            txtBodyMassIndex.Enabled = True
            ddlMissedWorkDays.Enabled = True
            ddlSpecialistRequired.Enabled = True

            rblEROrUrgentCareVisits.Enabled = True
            txtERorUrgentCareDetails.ReadOnly = False
            txtERorUrgentCareDetails.Enabled = True

            rblHospitalizations.Enabled = True
            txtHospitalizationDetails.ReadOnly = False
            txtHospitalizationDetails.Enabled = True

            ddlIncapacitationRisk.Enabled = True
            ddlFollowUpInterval.Enabled = True
            ddlDAWGRecommendation.Enabled = True

            txtDiagnosis.ReadOnly = False
            txtDiagnosis.Enabled = True
            txtPrognosis.ReadOnly = False
            txtPrognosis.Enabled = True
            txtTreatment.ReadOnly = False
            txtTreatment.Enabled = True
            txtMedicationDosages.ReadOnly = False
            txtMedicationDosages.Enabled = True
        End Sub

        Protected Function GetAssociatedCaseExpirationRenewalDate() As Nullable(Of DateTime)
            If (AssociatedSpecCase Is Nothing) Then
                Return Nothing
            End If

            Select Case AssociatedSpecCase.moduleId
                Case ModuleType.SpecCaseFT
                    Return AssociatedSpecCaseAsFastTrack.ExpirationDate

                Case ModuleType.SpecCaseMEB
                    Return AssociatedSpecCaseAsMEB.ExpirationDate

                Case ModuleType.SpecCaseRS
                    Return AssociatedSpecCaseAsRS.ExpirationDate

                Case ModuleType.SpecCaseWWD
                    Return AssociatedSpecCaseAsWWD.ExpirationDate

                Case Else
                    Return Nothing
            End Select
        End Function

        Protected Sub InitControls(ByVal isPostBack As Boolean)
            InitDropdownLists()
            InitICDControls()
            InitMemberInfoControls()
            InitRadioButtonListControls()
            InitTextBoxControls()

            SetInputFormatRestrictionsForPageControls()
            SetMaxLengthLimitsForPageControls()

            If (UserCanEdit) Then
                DisplayReadWrite(isPostBack)
            Else
                DisplayReadOnly(isPostBack)
            End If
        End Sub

        Protected Sub InitDAWGRecommendationsDropdownList()
            ddlDAWGRecommendation.DataSource = LookupDao.GetAllDAWGRecommendations()
            ddlDAWGRecommendation.DataValueField = "Id"
            ddlDAWGRecommendation.DataTextField = "Recommendation"
            ddlDAWGRecommendation.DataBind()

            Utility.InsertDropDownListZeroValue(ddlDAWGRecommendation, "--- Select One ---")

            If (SpecCase.DAWGRecommendation.HasValue) Then
                ddlDAWGRecommendation.SelectedValue = SpecCase.DAWGRecommendation.Value
            Else
                ddlDAWGRecommendation.SelectedValue = 0
            End If
        End Sub

        Protected Sub InitDropdownLists()
            InitMedGroupsDropdownList()
            InitRMUDropdownList()
            InitYearsSatisfactoryServiceDropdownList()
            InitMissedWorkDaysDropdownList()
            InitSpecialistsRequiredForManagementDropdownList()
            InitSuddenIncapacitationRisksDropdownList()
            InitFollowUpIntervalsDropdownList()
            InitDAWGRecommendationsDropdownList()
        End Sub

        Protected Sub InitFollowUpIntervalsDropdownList()
            ddlFollowUpInterval.DataSource = LookupDao.GetAllFollowUpIntervals()
            ddlFollowUpInterval.DataValueField = "Id"
            ddlFollowUpInterval.DataTextField = "Interval"
            ddlFollowUpInterval.DataBind()

            Utility.InsertDropDownListZeroValue(ddlFollowUpInterval, "--- Select One ---")

            If (SpecCase.RecommendedFollowUpInterval.HasValue) Then
                ddlFollowUpInterval.SelectedValue = SpecCase.RecommendedFollowUpInterval.Value
            Else
                ddlFollowUpInterval.SelectedValue = 0
            End If
        End Sub

        Protected Sub InitICDControls()
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

            txtDiagnosis.Text = Server.HtmlDecode(SpecCase.ICD9Diagnosis)
        End Sub

        Protected Sub InitMedGroupsDropdownList()
            ddlMedGroups.DataSource = From n In LookupDao.GetMedGroupNames("") Select n
            ddlMedGroups.DataTextField = "Name"
            ddlMedGroups.DataValueField = "Value"
            ddlMedGroups.DataBind()

            Utility.InsertDropDownListZeroValue(ddlMedGroups, "--- Select One ---")

            Dim NASelection As New ListItem()
            NASelection.Text = "N/A"
            NASelection.Value = -1
            ddlMedGroups.Items.Insert(1, NASelection)

            If (Not IsNothing(SpecCase.MedGroupName)) Then
                ddlMedGroups.SelectedValue = SpecCase.MedGroupName
            Else
                ddlMedGroups.SelectedValue = 0
            End If
        End Sub

        Protected Sub InitMemberInfoControls()
            txtMemberName.Text = SpecCase.MemberName
            txtMemberRank.Text = SpecCase.MemberRank.Grade
            txtMemberProtectedSSN.Text = "*****" & Right(SpecCase.MemberSSN, 4)
        End Sub

        Protected Sub InitMissedWorkDaysDropdownList()
            ddlMissedWorkDays.DataSource = LookupDao.GetAllMissedWorkDays()
            ddlMissedWorkDays.DataValueField = "Id"
            ddlMissedWorkDays.DataTextField = "DayIntervals"
            ddlMissedWorkDays.DataBind()

            Utility.InsertDropDownListZeroValue(ddlMissedWorkDays, "--- Select One ---")

            If (SpecCase.MissedWorkDays.HasValue) Then
                ddlMissedWorkDays.SelectedValue = SpecCase.MissedWorkDays.Value
            Else
                ddlMissedWorkDays.SelectedValue = 0
            End If
        End Sub

        Protected Sub InitRadioButtonListControls()
            If (SpecCase.HasHadERorUrgentCareVisits.HasValue) Then
                rblEROrUrgentCareVisits.SelectedValue = Convert.ToInt32(SpecCase.HasHadERorUrgentCareVisits.Value)
            End If

            If (SpecCase.HasBeenHospitalized.HasValue) Then
                rblHospitalizations.SelectedValue = Convert.ToInt32(SpecCase.HasBeenHospitalized.Value)
            End If

            If (SpecCase.DxInterferenceWithDuties.HasValue) Then
                rblDutyInterference.SelectedValue = Convert.ToInt32(SpecCase.DxInterferenceWithDuties.Value)
            End If

            rblEROrUrgentCareVisits.Attributes.Add("onclick", "updateCSSClassForRadioButtonListDetailsCheckboxes('" & rblEROrUrgentCareVisits.UniqueID & "', '" & tdERorUrgentCareDetails.ClientID & "');")
            rblHospitalizations.Attributes.Add("onclick", "updateCSSClassForRadioButtonListDetailsCheckboxes('" & rblHospitalizations.UniqueID & "', '" & tdHospitalizationsDetails.ClientID & "');")
        End Sub

        Protected Sub InitRenewalDateTextBox()
            If (SpecCase.RenewalDate.HasValue) Then
                WebControlSetters.SetDateTextbox(txtRenewalDate, SpecCase.RenewalDate)
            ElseIf (AssociatedSpecCase IsNot Nothing AndAlso GetAssociatedCaseExpirationRenewalDate() IsNot Nothing) Then
                WebControlSetters.SetDateTextbox(txtRenewalDate, GetAssociatedCaseExpirationRenewalDate())
            End If
        End Sub

        Protected Sub InitRMUDropdownList()

            If Session(SESSIONKEY_COMPO) = "6" Then
                ddlRMUs.DataSource = From n In LookupDao.GetRMUNames("") Where n.Value < 100 Select n
            Else
                ddlRMUs.DataSource = From n In LookupDao.GetRMUNames("") Where n.Value >= 100 Select n
            End If
            ddlRMUs.DataTextField = "Name"
            ddlRMUs.DataValueField = "Value"
            ddlRMUs.DataBind()

            Utility.InsertDropDownListZeroValue(ddlRMUs, "--- Select One ---")

            If (Not IsNothing(SpecCase.RMUName)) Then
                ddlRMUs.SelectedValue = SpecCase.RMUName
            Else
                ddlRMUs.SelectedValue = 0
            End If
        End Sub

        Protected Sub InitSpecialistsRequiredForManagementDropdownList()
            ddlSpecialistRequired.DataSource = LookupDao.GetAllSpecialistsRequiredForManagement()
            ddlSpecialistRequired.DataValueField = "Id"
            ddlSpecialistRequired.DataTextField = "AmountPerYear"
            ddlSpecialistRequired.DataBind()

            Utility.InsertDropDownListZeroValue(ddlSpecialistRequired, "--- Select One ---")

            If (SpecCase.RequiresSpecialistForMgmt.HasValue) Then
                ddlSpecialistRequired.SelectedValue = SpecCase.RequiresSpecialistForMgmt.Value
            Else
                ddlSpecialistRequired.SelectedValue = 0
            End If
        End Sub

        Protected Sub InitSuddenIncapacitationRisksDropdownList()
            ddlIncapacitationRisk.DataSource = LookupDao.GetAllSuddenIncapaciationRisks()
            ddlIncapacitationRisk.DataValueField = "Id"
            ddlIncapacitationRisk.DataTextField = "RiskLevel"
            ddlIncapacitationRisk.DataBind()

            Utility.InsertDropDownListZeroValue(ddlIncapacitationRisk, "--- Select One ---")

            If (SpecCase.RiskForSuddenIncapacitation.HasValue) Then
                ddlIncapacitationRisk.SelectedValue = SpecCase.RiskForSuddenIncapacitation.Value
            Else
                ddlIncapacitationRisk.SelectedValue = 0
            End If
        End Sub

        Protected Sub InitTextBoxControls()
            txtRenewalDate.CssClass = "datePickerAll"

            InitRenewalDateTextBox()

            WebControlSetters.SetTextboxText(txtDAFSC, SpecCase.DAFSC)
            WebControlSetters.SetTextboxText(txtBodyMassIndex, SpecCase.BodyMassIndex)
            WebControlSetters.SetTextboxText(txtERorUrgentCareDetails, SpecCase.ERorUrgentCareVisitList)
            WebControlSetters.SetTextboxText(txtHospitalizationDetails, SpecCase.HospitalizationList)
            WebControlSetters.SetTextboxText(txtPrognosis, SpecCase.Prognosis)
            WebControlSetters.SetTextboxText(txtTreatment, SpecCase.Treatment)
            WebControlSetters.SetTextboxText(txtMedicationDosages, SpecCase.MedicationsAndDosages)
        End Sub

        Protected Sub InitYearsSatisfactoryServiceDropdownList()
            ddlYearsSatisfactoryService.DataSource = LookupDao.GetAllYearsSatisfactoryService()
            ddlYearsSatisfactoryService.DataValueField = "Id"
            ddlYearsSatisfactoryService.DataTextField = "RangeCategory"
            ddlYearsSatisfactoryService.DataBind()

            Utility.InsertDropDownListZeroValue(ddlYearsSatisfactoryService, "--- Select One ---")

            If (SpecCase.YearsSatisfactoryService.HasValue) Then
                ddlYearsSatisfactoryService.SelectedValue = SpecCase.YearsSatisfactoryService.Value
            Else
                ddlYearsSatisfactoryService.SelectedValue = 0
            End If
        End Sub

        Protected Sub Page_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
            AddHandler Me.Master.TabClick, AddressOf TabButtonClicked
        End Sub

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
            AddHandler ucICDCodeControl.ICDDiagnosisChanged, AddressOf ddlICDDiagnosis_SelectedIndexChanged

            ucICDCodeControl.Initialilze(Me)
            ucICD7thCharacterControl.Initialize(ucICDCodeControl)

            If (Not IsPostBack) Then
                UserCanEdit = GetAccess(Navigator.PageAccess, True)
                InitControls(False)

                LogManager.LogAction(CaseModuleType, UserAction.ViewPage, RefId, "Viewed Page: Med Tech")
            End If

            If Session(SESSIONKEY_COMPO) = "6" Then
                lblRMU.Text = "RMU Name: "
            Else
                lblRMU.Text = "GMU Name: "
            End If

        End Sub

        Protected Function RadioButtonListSelectedValueToBoolean(ByVal rbl As RadioButtonList) As Boolean
            Try
                Dim selectedValueAsInt As Integer = 0

                If (String.IsNullOrEmpty(rbl.SelectedValue) OrElse Not Integer.TryParse(rbl.SelectedValue, selectedValueAsInt)) Then
                    Return False
                End If

                Return Convert.ToBoolean(selectedValueAsInt)
            Catch ex As Exception
                Return False
            End Try
        End Function

        Protected Sub Save_Click(ByRef sender As Object, ByRef e As TabNavigationEventArgs)
            If (Navigator.CurrentStep.IsReadOnly) Then
                Exit Sub
            End If

            If (e.ButtonType = NavigatorButtonType.Save OrElse e.ButtonType = NavigatorButtonType.NavigatedAway _
                OrElse e.ButtonType = NavigatorButtonType.NextStep OrElse e.ButtonType = NavigatorButtonType.PreviousStep) Then
                SaveFindings()
            End If

            If Not (ValidBoxLength()) Then
                e.Cancel = True
                Exit Sub
            End If
        End Sub

        Protected Sub SaveDropdownListSelections()
            SpecCase.MedGroupName = Utility.GetDropDownListNullableSelectedValue(ddlMedGroups, 0)
            SpecCase.RMUName = Utility.GetDropDownListNullableSelectedValue(ddlRMUs, 0)
            SpecCase.YearsSatisfactoryService = Utility.GetDropDownListNullableSelectedValue(ddlYearsSatisfactoryService, 0)
            SpecCase.MissedWorkDays = Utility.GetDropDownListNullableSelectedValue(ddlMissedWorkDays, 0)
            SpecCase.RequiresSpecialistForMgmt = Utility.GetDropDownListNullableSelectedValue(ddlSpecialistRequired, 0)
            SpecCase.RiskForSuddenIncapacitation = Utility.GetDropDownListNullableSelectedValue(ddlIncapacitationRisk, 0)
            SpecCase.RecommendedFollowUpInterval = Utility.GetDropDownListNullableSelectedValue(ddlFollowUpInterval, 0)
            SpecCase.DAWGRecommendation = Utility.GetDropDownListNullableSelectedValue(ddlDAWGRecommendation, 0)
        End Sub

        Protected Sub SaveFindings()
            If (Not UserCanEdit) Then
                Exit Sub
            End If

            SaveDropdownListSelections()
            SaveICDFindings()
            SaveRadioButtonListSelections()
            SaveTextBoxEntries()

            SCDao.SaveOrUpdate(SpecCase)
            SCDao.CommitChanges()
        End Sub

        Protected Sub SaveICDFindings()
            If (ucICDCodeControl.IsICDCodeSelected()) Then
                SpecCase.ICD9Code = ucICDCodeControl.SelectedICDCodeID
                SpecCase.ICD9Description = ucICDCodeControl.SelectedICDCodeText
            Else
                'SpecCase.ICD9Code = Nothing
                'SpecCase.ICD9Description = Nothing
            End If

            If (ucICD7thCharacterControl.Is7thCharacterSelected()) Then
                SpecCase.ICD7thCharacter = ucICD7thCharacterControl.Selected7thCharacter
            Else
                SpecCase.ICD7thCharacter = Nothing
            End If

            SpecCase.ICD9Diagnosis = Server.HtmlEncode(txtDiagnosis.Text.Trim())

            If (Not IsNothing(SpecCase.ICD9Code)) Then
                ucICDCodeControl.UpdateICDCodeDiagnosisLabel(SpecCase.ICD9Code)
            End If
        End Sub

        Protected Sub SaveRadioButtonListSelections()
            If (Not String.IsNullOrEmpty(rblEROrUrgentCareVisits.SelectedValue)) Then
                SpecCase.HasHadERorUrgentCareVisits = RadioButtonListSelectedValueToBoolean(rblEROrUrgentCareVisits)
            End If

            If (Not String.IsNullOrEmpty(rblHospitalizations.SelectedValue)) Then
                SpecCase.HasBeenHospitalized = RadioButtonListSelectedValueToBoolean(rblHospitalizations)
            End If

            If (Not String.IsNullOrEmpty(rblDutyInterference.SelectedValue)) Then
                SpecCase.DxInterferenceWithDuties = RadioButtonListSelectedValueToBoolean(rblDutyInterference)
            End If
        End Sub

        Protected Sub SaveTextBoxEntries()
            SpecCase.DAFSC = HTMLEncodeNulls(txtDAFSC.Text)
            SpecCase.BodyMassIndex = HTMLEncodeNulls(txtBodyMassIndex.Text, True)
            SpecCase.Prognosis = HTMLEncodeNulls(txtPrognosis.Text, True)
            SpecCase.Treatment = HTMLEncodeNulls(txtTreatment.Text, True)
            SpecCase.MedicationsAndDosages = HTMLEncodeNulls(txtMedicationDosages.Text, True)

            If (Not String.IsNullOrEmpty(txtRenewalDate.Text)) Then
                SpecCase.RenewalDate = Server.HtmlEncode(txtRenewalDate.Text)
            Else
                SpecCase.RenewalDate = Nothing
            End If

            If (RadioButtonListSelectedValueToBoolean(rblEROrUrgentCareVisits)) Then
                SpecCase.ERorUrgentCareVisitList = HTMLEncodeNulls(txtERorUrgentCareDetails.Text, True)
            End If

            If (RadioButtonListSelectedValueToBoolean(rblHospitalizations)) Then
                SpecCase.HospitalizationList = HTMLEncodeNulls(txtHospitalizationDetails.Text, True)
            End If
        End Sub

        Protected Sub SetInputFormatRestrictionsForPageControls()
            SetInputFormatRestriction(Page, txtRenewalDate, FormatRestriction.Numeric, "/")
            SetInputFormatRestriction(Page, txtDiagnosis, FormatRestriction.AlphaNumeric, PERMITTED_SPECIAL_CHAR_INPUT)
            SetInputFormatRestriction(Page, txtDAFSC, FormatRestriction.AlphaNumeric, PERMITTED_SPECIAL_CHAR_INPUT)
            SetInputFormatRestriction(Page, txtBodyMassIndex, FormatRestriction.Numeric, ".")
            SetInputFormatRestriction(Page, txtERorUrgentCareDetails, FormatRestriction.AlphaNumeric, PERMITTED_SPECIAL_CHAR_INPUT)
            SetInputFormatRestriction(Page, txtHospitalizationDetails, FormatRestriction.AlphaNumeric, PERMITTED_SPECIAL_CHAR_INPUT)
            SetInputFormatRestriction(Page, txtDiagnosis, FormatRestriction.AlphaNumeric, PERMITTED_SPECIAL_CHAR_INPUT)
            SetInputFormatRestriction(Page, txtPrognosis, FormatRestriction.AlphaNumeric, PERMITTED_SPECIAL_CHAR_INPUT)
            SetInputFormatRestriction(Page, txtTreatment, FormatRestriction.AlphaNumeric, PERMITTED_SPECIAL_CHAR_INPUT)
            SetInputFormatRestriction(Page, txtMedicationDosages, FormatRestriction.AlphaNumeric, PERMITTED_SPECIAL_CHAR_INPUT)
        End Sub

        Protected Sub SetMaxLengthLimitsForPageControls()
            SetMaxLength(txtDiagnosis)
            SetMaxLength(txtHospitalizationDetails)
            SetMaxLength(txtERorUrgentCareDetails)
            SetMaxLength(txtPrognosis)
            SetMaxLength(txtTreatment)
            SetMaxLength(txtMedicationDosages)
        End Sub

        Private Sub TabButtonClicked(ByRef sender As Object, ByRef e As TabNavigationEventArgs)
            If (e.ButtonType = NavigatorButtonType.Save OrElse
                e.ButtonType = NavigatorButtonType.NavigatedAway OrElse
                e.ButtonType = NavigatorButtonType.NextStep OrElse
                e.ButtonType = NavigatorButtonType.PreviousStep) Then
                SaveFindings()
            End If

            If Not (ValidBoxLength()) Then
                e.Cancel = True
                Exit Sub
            End If
        End Sub

#End Region

    End Class

End Namespace