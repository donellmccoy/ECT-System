Imports ALOD.Core.Domain.Common
Imports ALOD.Core.Domain.Lookup
Imports ALOD.Core.Domain.Modules.Lod
Imports ALOD.Core.Domain.Modules.SpecialCases
Imports ALOD.Core.Domain.Users
Imports ALOD.Core.Domain.Workflow
Imports ALOD.Core.Interfaces.DAOInterfaces
Imports ALOD.Core.Utils
Imports ALOD.Data
Imports ALOD.Data.Services
Imports ALOD.Logging
Imports ALOD.Web.UserControls
Imports ALODWebUtility.Common

Namespace Web.Special_Case.IRILO

    Partial Class Secure_sc_FT_MedTech
        Inherits System.Web.UI.Page

        Private _MedTechSig As SignatureMetaData
        Private _sigDao As ISignatueMetaDateDao

        Public ReadOnly Property Navigator() As TabNavigator
            Get
                Return MasterPage.Navigator
            End Get
        End Property

        Public ReadOnly Property TabControl() As TabControls
            Get
                Return Master.TabControl
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

        Protected ReadOnly Property MasterPage() As SC_FastTrackMaster
            Get
                Dim master As SC_FastTrackMaster = CType(Page.Master, SC_FastTrackMaster)
                Return master
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

        Private ReadOnly Property MedTechSig() As SignatureMetaData
            Get
                If (_MedTechSig Is Nothing) Then
                    _MedTechSig = SigDao.GetByWorkStatus(SpecCase.Id, SpecCase.Workflow, SpecCaseWWDWorkStatus.InitiateCase)
                End If

                Return _MedTechSig
            End Get
        End Property

        Private ReadOnly Property RefId() As Integer
            Get
                Return CInt(Request.QueryString("refId"))
            End Get
        End Property

        Private ReadOnly Property SigDao() As SignatureMetaDataDao
            Get
                If (_sigDao Is Nothing) Then
                    _sigDao = New NHibernateDaoFactory().GetSigMetaDataDao()
                End If

                Return _sigDao
            End Get
        End Property

#Region "SC_FastTrackProperty"

        Public FTsubType As String
        Public FTType As String
        Private _icd As String = String.Empty
        Private _keyValDao As IKeyValDao
        Private _scAccess As Dictionary(Of String, ALOD.Core.Domain.Workflow.PageAccessType)
        Dim dao As ISpecialCaseDAO

        Private sc As SC_FastTrack = Nothing
        Private scId As Integer = 0

        ReadOnly Property KeyValDao() As IKeyValDao
            Get
                If (_keyValDao Is Nothing) Then
                    _keyValDao = New NHibernateDaoFactory().GetKeyValDao()
                End If

                Return _keyValDao
            End Get
        End Property

        ReadOnly Property SCDao() As ISpecialCaseDAO
            Get
                If (dao Is Nothing) Then
                    dao = New NHibernateDaoFactory().GetSpecialCaseDAO()
                End If

                Return dao
            End Get
        End Property

        Public ReadOnly Property ReferenceId() As Integer
            Get
                Return Integer.Parse(Request.QueryString("refId"))
            End Get
        End Property

        Protected ReadOnly Property SpecCase() As SC_FastTrack
            Get
                If (sc Is Nothing) Then
                    sc = SCDao.GetById(ReferenceId)
                End If

                Return sc
            End Get
        End Property

#End Region

        Public Sub DisplayReadWrite(ByVal isPostBack As Boolean)
            If UserCanEdit Then

                ddlMedGroups.Enabled = True
                ddlRMUs.Enabled = True

                TMTReceiveDate.Enabled = True
                TMTReceiveDate.CssClass = "datePicker"
                DAFSC.Enabled = True
                YearsSatisfactoryService.Enabled = True
                BodyMassIndex.Enabled = True
                MissedWorkDays.Enabled = True
                SpecialistRequired.Enabled = True
                ERorUrgentCareY.Enabled = True
                ERorUrgentCareN.Enabled = True
                ERorUrgentCareDetails.ReadOnly = False
                ERorUrgentCareDetails.Enabled = True
                HospitalizationDetails.ReadOnly = False
                HospitalizationDetails.Enabled = True
                HospitalizationsN.Enabled = True
                HospitalizationsY.Enabled = True
                IncapacitationRisk.Enabled = True
                FollowUpInterval.Enabled = True
                DAWGRecommendation.Enabled = True
                DutyInterferenceN.Enabled = True
                DutyInterferenceY.Enabled = True

                If SpecCase.ICD9Code.HasValue Then
                    If ((SpecCase.ICD9Code = 0) Or (SpecCase.ICD9Code = SCFastTrackTypes.Asthma) Or
                        (SpecCase.ICD9Code = SCFastTrackTypes.Diabetes) Or
                        (SpecCase.ICD9Code = SCFastTrackTypes.Sleep)) Then
                        DutyInterferenceY.Checked = False  'Uncheck upon re-enabling
                    End If

                    If (SpecCase.ICD9Code <> SCFastTrackTypes.Sleep) Then
                        BIPAPRequiredY.Checked = False  'Uncheck upon re-enabling
                    End If
                End If

                Prognosis.ReadOnly = False
                Prognosis.Enabled = True
                Treatment.ReadOnly = False
                Treatment.Enabled = True
                MedicationDosages.ReadOnly = False
                MedicationDosages.Enabled = True
                DaytimeSomnolenceDetails.ReadOnly = False
                DaytimeSomnolenceDetails.Enabled = True
                DaytimeSomnolenceN.Enabled = True
                DaytimeSomnolenceY.Enabled = True
                ApneaEpisodeDetails.ReadOnly = False
                ApneaEpisodeDetails.Enabled = True
                ApneaEpisodesN.Enabled = True
                ApneaEpisodesY.Enabled = True
                SleepStudyN.Enabled = True
                SleepStudyY.Enabled = True
                OralDevicesN.Enabled = True
                OralDevicesY.Enabled = True
                CPAPRequiredN.Enabled = True
                CPAPRequiredY.Enabled = True
                BIPAPRequiredN.Enabled = True
                BIPAPRequiredY.Enabled = True

                SystemResponseN.Enabled = True
                SystemResponseY.Enabled = True
                FastingBloodSugar.Enabled = True
                HgbA1C.Enabled = True
                OptometryExamN.Enabled = True
                OptometryExamY.Enabled = True
                OtherConditionsList.ReadOnly = False
                OtherConditionsList.Enabled = True
                OtherConditionsN.Enabled = True
                OtherConditionsY.Enabled = True
                OralAgentsList.ReadOnly = False
                OralAgentsList.Enabled = True
                OralAgentsN.Enabled = True
                OralAgentsY.Enabled = True
                RequiresInsulinN.Enabled = True
                RequiresInsulinY.Enabled = True
                InsulinDosageRegime.ReadOnly = False
                InsulinDosageRegime.Enabled = True
                RequiresNonInsulinN.Enabled = True
                RequiresNonInsulinY.Enabled = True
                PFTN.Enabled = True
                PFTY.Enabled = True
                MethacholineChallengeN.Enabled = True
                MethacholineChallengeP.Enabled = True
                MethacholineChallengeY.Enabled = True
                DailySteroidsDosage.ReadOnly = False
                DailySteroidsDosage.Enabled = True
                RequiresSteroidsN.Enabled = True
                RequiresSteroidsY.Enabled = True
                FrequencyOfRescueInhalerUsage.Enabled = True
                ColdExerciseExacerbationDetails.ReadOnly = False
                ColdExerciseExacerbationDetails.Enabled = True
                ColdExerciseExacerbationN.Enabled = True
                ColdExerciseExacerbationY.Enabled = True
                ExacerbatedSymptomsOralSteroidsY.Enabled = True
                ExacerbatedSymptomsOralSteroidsN.Enabled = True
                ExacerbatedSymptomsOralSteroidsDosage.ReadOnly = False
                ExacerbatedSymptomsOralSteroidsDosage.Enabled = True
                NormalPFTWithTreatmentY.Enabled = True
                NormalPFTWithTreatmentN.Enabled = True
                HOIntubationN.Enabled = True
                HOIntubationY.Enabled = True

                If (Not isPostBack) Then
                    ucICDCodeControl.DisplayReadWrite(False)
                    ucICD7thCharacterControl.DisplayReadWrite()
                End If

                DiagnosisTextBox.ReadOnly = False
                DiagnosisTextBox.Enabled = True
            Else
                DisplayReadOnly(0, True)
            End If
        End Sub

        Protected Sub BIPAPRequiredN_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles BIPAPRequiredN.CheckedChanged
            If BIPAPRequiredY.Checked Then
                DisplayReadOnly(1, True)
            Else
                DisplayReadWrite(True)
            End If
            SaveFindings()
            InitControls(True)
            UpdatePanel2.Update()
        End Sub

        Protected Sub BIPAPRequiredY_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles BIPAPRequiredY.CheckedChanged
            If BIPAPRequiredY.Checked Then
                DisplayReadOnly(1, True)
            Else
                DisplayReadWrite(True)
            End If
            SaveFindings()
            InitControls(True)
            UpdatePanel2.Update()
        End Sub

        Protected Sub ChangeContent() 'ByVal sender As Object, ByVal e As System.EventArgs) 'Handles ddlICDContent.SelectedIndexChanged

            'Hide the Divs
            MostCases.Visible = False
            SleepApnea.Visible = False
            Diabetes.Visible = False
            Asthma.Visible = False

            'If InStr(cddContent.SelectedValue, ":") > 0 Then
            If (ucICDCodeControl.IsICDCodeSelected()) Then
                '_icd = Left(cddContent.SelectedValue, InStr(cddContent.SelectedValue, ":") - 1)
                'Dim temp As String = ddlICDContent.SelectedValue
                _icd = ucICDCodeControl.SelectedICDCodeID.ToString()

                If (_icd = 0) Then
                    FTsubType = "IRILO - Not Eligible"
                    'FTsubType = FTType & "Fast Track - Not Eligible, requires Full " & FTType & " Workflow"
                ElseIf (IsICDCodeOfType("Diabetes", _icd)) Then
                    '250
                    Diabetes.Visible = True
                    FTsubType = "IRILO - Type II Diabetes [Non-Insulin Dependent]"
                    'FTsubType = FTType & "Fast Track - Type II Diabetes [Non-Insulin Dependent]"
                    lblDiabetes.Text = FTsubType
                ElseIf (IsICDCodeOfType("Asthma", _icd)) Then
                    '493
                    Asthma.Visible = True
                    FTsubType = "IRILO - Asthma"
                    'FTsubType = FTType & "Fast Track - Asthma"
                    lblAsthma.Text = FTsubType
                ElseIf (IsICDCodeOfType("Sleep", _icd)) Then
                    '327
                    SleepApnea.Visible = True
                    FTsubType = "IRILO - Obstructive Sleep Apnea (OSA)"
                    'FTsubType = FTType & "Fast Track - Obstructive Sleep Apnea (OSA)"
                    lblSleepApnea.Text = FTsubType
                Else
                    MostCases.Visible = True
                    FTsubType = "IRILO - General (" & ucICDCodeControl.ICDCodeDiagnosisLabelText & ")"
                    'FTsubType = FTType & "Fast Track - General (" & Icd9DiagnosisLabel.Text & ")"
                    'FTsubType = FTType & "Fast Track - General (" & cddContent.SelectedValue & ")"
                    lblMostCases.Text = FTsubType
                End If

                'Select Case _icd
                '    Case 0
                '        FTsubType = "IRILO - Not Eligible"
                '        'FTsubType = FTType & "Fast Track - Not Eligible, requires Full " & FTType & " Workflow"
                '    Case 251
                '        '250
                '        Diabetes.Visible = True
                '        FTsubType = "IRILO - Type II Diabetes [Non-Insulin Dependent]"
                '        'FTsubType = FTType & "Fast Track - Type II Diabetes [Non-Insulin Dependent]"
                '        lblDiabetes.Text = FTsubType
                '    Case 1045
                '        '327
                '        SleepApnea.Visible = True
                '        FTsubType = "IRILO - Obstructive Sleep Apnea (OSA)"
                '        'FTsubType = FTType & "Fast Track - Obstructive Sleep Apnea (OSA)"
                '        lblSleepApnea.Text = FTsubType
                '    Case 498
                '        '493
                '        Asthma.Visible = True
                '        FTsubType = "IRILO - Asthma"
                '        'FTsubType = FTType & "Fast Track - Asthma"
                '        lblAsthma.Text = FTsubType
                '    Case Else
                '        MostCases.Visible = True
                '        FTsubType = "IRILO - General (" & ucICDCodeControl.ICDCodeDiagnosisLabelText & ")"
                '        'FTsubType = FTType & "Fast Track - General (" & Icd9DiagnosisLabel.Text & ")"
                '        'FTsubType = FTType & "Fast Track - General (" & cddContent.SelectedValue & ")"
                '        lblMostCases.Text = FTsubType
                'End Select

                lblFastTrackAll.Text = FTsubType
                Session("FTSubType") = FTsubType
                If String.IsNullOrEmpty(_icd) Then
                    _icd = "0"
                End If
                SpecCase.ICD9Code = _icd
            End If
        End Sub

        Protected Sub ddlICDDiagnosis_SelectedIndexChanged(sender As Object, e As EventArgs)
            SaveFindings()
            InitControls(True)
            UpdatePanel2.Update()
        End Sub

        Protected Sub DisplayReadOnly(ByVal ShowStoppers As Integer, ByVal isPostBack As Boolean)
            BIPAPRequiredN.Enabled = False
            BIPAPRequiredY.Enabled = False
            YearsSatisfactoryService.Enabled = True
            Select Case ShowStoppers
                Case 1
                    BIPAPRequiredN.Enabled = True
                    BIPAPRequiredY.Enabled = True
                Case 2
                    YearsSatisfactoryService.Enabled = True
                Case 3
                    YearsSatisfactoryService.Enabled = True
                Case Else
                    ' Do nothing
            End Select

            ddlMedGroups.Enabled = True
            ddlRMUs.Enabled = True

            TMTReceiveDate.Enabled = True
            TMTReceiveDate.CssClass = "datePicker"
            DAFSC.Enabled = False
            BodyMassIndex.Enabled = True
            MissedWorkDays.Enabled = False
            SpecialistRequired.Enabled = False
            ERorUrgentCareY.Enabled = False
            ERorUrgentCareN.Enabled = False
            ERorUrgentCareDetails.ReadOnly = True
            ERorUrgentCareDetails.Enabled = True
            HospitalizationDetails.ReadOnly = True
            HospitalizationDetails.Enabled = True
            HospitalizationsN.Enabled = False
            HospitalizationsY.Enabled = False
            IncapacitationRisk.Enabled = False
            FollowUpInterval.Enabled = False
            DAWGRecommendation.Enabled = False
            Prognosis.ReadOnly = True
            Prognosis.Enabled = True
            Treatment.ReadOnly = True
            Treatment.Enabled = True
            MedicationDosages.ReadOnly = True
            MedicationDosages.Enabled = True
            DaytimeSomnolenceDetails.ReadOnly = True
            DaytimeSomnolenceDetails.Enabled = True
            DaytimeSomnolenceN.Enabled = False
            DaytimeSomnolenceY.Enabled = False
            ApneaEpisodeDetails.ReadOnly = True
            ApneaEpisodeDetails.Enabled = True
            ApneaEpisodesN.Enabled = False
            ApneaEpisodesY.Enabled = False
            SleepStudyN.Enabled = False
            SleepStudyY.Enabled = False
            OralDevicesN.Enabled = False
            OralDevicesY.Enabled = False
            CPAPRequiredN.Enabled = False
            CPAPRequiredY.Enabled = False
            SystemResponseN.Enabled = False
            SystemResponseY.Enabled = False
            FastingBloodSugar.Enabled = False
            HgbA1C.Enabled = False
            OptometryExamN.Enabled = False
            OptometryExamY.Enabled = False
            OtherConditionsList.ReadOnly = True
            OtherConditionsList.Enabled = True
            OtherConditionsN.Enabled = False
            OtherConditionsY.Enabled = False
            OralAgentsList.ReadOnly = True
            OralAgentsList.Enabled = True
            OralAgentsN.Enabled = False
            OralAgentsY.Enabled = False
            RequiresInsulinN.Enabled = False
            RequiresInsulinY.Enabled = False
            InsulinDosageRegime.ReadOnly = True
            InsulinDosageRegime.Enabled = True
            RequiresNonInsulinN.Enabled = False
            RequiresNonInsulinY.Enabled = False
            PFTN.Enabled = False
            PFTY.Enabled = False
            MethacholineChallengeN.Enabled = False
            MethacholineChallengeP.Enabled = False
            MethacholineChallengeY.Enabled = False
            DailySteroidsDosage.ReadOnly = True
            DailySteroidsDosage.Enabled = True
            RequiresSteroidsN.Enabled = False
            RequiresSteroidsY.Enabled = False
            FrequencyOfRescueInhalerUsage.Enabled = False
            ColdExerciseExacerbationDetails.ReadOnly = True
            ColdExerciseExacerbationDetails.Enabled = True
            ColdExerciseExacerbationN.Enabled = False
            ColdExerciseExacerbationY.Enabled = False
            ExacerbatedSymptomsOralSteroidsY.Enabled = False
            ExacerbatedSymptomsOralSteroidsN.Enabled = False
            ExacerbatedSymptomsOralSteroidsDosage.ReadOnly = True
            ExacerbatedSymptomsOralSteroidsDosage.Enabled = True
            NormalPFTWithTreatmentY.Enabled = False
            NormalPFTWithTreatmentN.Enabled = False
            HOIntubationN.Enabled = False
            HOIntubationY.Enabled = False

            If (Not isPostBack) Then
                ucICDCodeControl.DisplayReadOnly(False)
                ucICD7thCharacterControl.DisplayReadOnly()
            End If

            DiagnosisTextBox.ReadOnly = True
            DiagnosisTextBox.Enabled = True

        End Sub

        Protected Sub ERorUrgentCareN_CheckedChanged(sender As Object, e As EventArgs) Handles ERorUrgentCareN.CheckedChanged
            If (ERorUrgentCareN.Checked) Then
                Label11.CssClass = "label"
            Else
                Label11.CssClass = "label labelRequired"
            End If
            UpdatePanel2.Update()
        End Sub

        Protected Sub ERorUrgentCareY_CheckedChanged(sender As Object, e As EventArgs) Handles ERorUrgentCareY.CheckedChanged
            If (ERorUrgentCareY.Checked) Then
                Label11.CssClass = "label labelRequired"
            Else
                Label11.CssClass = "label"
            End If
            UpdatePanel2.Update()
        End Sub

        Protected Sub HospitalizationsN_CheckedChanged(sender As Object, e As EventArgs) Handles HospitalizationsN.CheckedChanged
            If (HospitalizationsN.Checked) Then
                Label13.CssClass = "label"
            Else
                Label13.CssClass = "label labelRequired"
            End If
            UpdatePanel2.Update()
        End Sub

        Protected Sub HospitalizationsY_CheckedChanged(sender As Object, e As EventArgs) Handles HospitalizationsY.CheckedChanged
            If (HospitalizationsY.Checked) Then
                Label13.CssClass = "label labelRequired"
            Else
                Label13.CssClass = "label"
            End If
            UpdatePanel2.Update()
        End Sub

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

            AddHandler ucICDCodeControl.ICDDiagnosisChanged, AddressOf ddlICDDiagnosis_SelectedIndexChanged

            ucICDCodeControl.Initialilze(Me)
            ucICD7thCharacterControl.Initialize(ucICDCodeControl)

            If (Not IsPostBack) Then
                UserCanEdit = GetAccess(Navigator.PageAccess, True)
                InitControls(False)
                SetMaxLength(DiagnosisTextBox)
                SetMaxLength(HospitalizationDetails)
                SetMaxLength(ERorUrgentCareDetails)
                SetMaxLength(Prognosis)
                SetMaxLength(Treatment)
                SetMaxLength(MedicationDosages)
                SetMaxLength(DaytimeSomnolenceDetails)
                SetMaxLength(ApneaEpisodeDetails)
                SetMaxLength(OtherConditionsList)
                SetMaxLength(OralAgentsList)
                SetMaxLength(InsulinDosageRegime)
                SetMaxLength(ColdExerciseExacerbationDetails)
                SetMaxLength(ExacerbatedSymptomsOralSteroidsDosage)

                LogManager.LogAction(ModuleType.SpecCaseFT, UserAction.ViewPage, RefId, "Viewed Page: Med Tech")

            End If

            If Session(SESSIONKEY_COMPO) = "6" Then
                Label34.Text = "RMU Name: "
            Else
                Label34.Text = "GMU Name: "
            End If

        End Sub

        Protected Sub YearsSatisfactoryService_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles YearsSatisfactoryService.SelectedIndexChanged
            If YearsSatisfactoryService.SelectedValue = 1 Then 'Less than 5 years
                DisplayReadOnly(2, True)
            End If
            If (YearsSatisfactoryService.SelectedValue = 2 Or YearsSatisfactoryService.SelectedValue = 3) Then 'And DAFSCSuitableN.Checked
                DisplayReadOnly(3, True)
            Else
                DisplayReadWrite(True)
            End If

            '_icd = Left(cddContent.SelectedValue, InStr(cddContent.SelectedValue, ":") - 1)
            'Dim temp As String = ddlICDContent.SelectedValue
            _icd = ucICDCodeControl.SelectedICDCodeID.ToString()

            Select Case _icd

                Case "", "251", "498", "1045"
                Case Else
                    MostCases.Visible = True
            End Select
        End Sub

        Private Sub InitControls(ByVal isPostBack As Boolean)
            Dim FillValues As Boolean = True
            Dim rdOnly As Boolean = False

            ' Set textbox character restrictions
            SetInputFormatRestriction(Page, TMTReceiveDate, FormatRestriction.Numeric, "/")
            SetInputFormatRestriction(Page, DiagnosisTextBox, FormatRestriction.AlphaNumeric, PERMITTED_SPECIAL_CHAR_INPUT)
            SetInputFormatRestriction(Page, DAFSC, FormatRestriction.AlphaNumeric, PERMITTED_SPECIAL_CHAR_INPUT)
            SetInputFormatRestriction(Page, ERorUrgentCareDetails, FormatRestriction.AlphaNumeric, PERMITTED_SPECIAL_CHAR_INPUT)
            SetInputFormatRestriction(Page, HospitalizationDetails, FormatRestriction.AlphaNumeric, PERMITTED_SPECIAL_CHAR_INPUT)
            SetInputFormatRestriction(Page, Prognosis, FormatRestriction.AlphaNumeric, PERMITTED_SPECIAL_CHAR_INPUT)
            SetInputFormatRestriction(Page, Treatment, FormatRestriction.AlphaNumeric, PERMITTED_SPECIAL_CHAR_INPUT)
            SetInputFormatRestriction(Page, MedicationDosages, FormatRestriction.AlphaNumeric, PERMITTED_SPECIAL_CHAR_INPUT)
            SetInputFormatRestriction(Page, DaytimeSomnolenceDetails, FormatRestriction.AlphaNumeric, PERMITTED_SPECIAL_CHAR_INPUT)
            SetInputFormatRestriction(Page, ApneaEpisodeDetails, FormatRestriction.AlphaNumeric, PERMITTED_SPECIAL_CHAR_INPUT)
            SetInputFormatRestriction(Page, OtherConditionsList, FormatRestriction.AlphaNumeric, PERMITTED_SPECIAL_CHAR_INPUT)
            SetInputFormatRestriction(Page, OralAgentsList, FormatRestriction.AlphaNumeric, PERMITTED_SPECIAL_CHAR_INPUT)
            SetInputFormatRestriction(Page, InsulinDosageRegime, FormatRestriction.AlphaNumeric, PERMITTED_SPECIAL_CHAR_INPUT)
            SetInputFormatRestriction(Page, ColdExerciseExacerbationDetails, FormatRestriction.AlphaNumeric, PERMITTED_SPECIAL_CHAR_INPUT)
            SetInputFormatRestriction(Page, ExacerbatedSymptomsOralSteroidsDosage, FormatRestriction.AlphaNumeric, PERMITTED_SPECIAL_CHAR_INPUT)

            Dim uDao As UserDao = New NHibernateDaoFactory().GetUserDao()
            Dim currUser As AppUser = uDao.GetById(SESSION_USER_ID)

            'Hide the Divs
            MostCases.Visible = False
            SleepApnea.Visible = False
            Diabetes.Visible = False
            Asthma.Visible = False

            Dim lkupDAO As ILookupDao
            lkupDAO = New NHibernateDaoFactory().GetLookupDao()

            ddlMedGroups.DataSource = From n In lkupDAO.GetMedGroupNames("") Select n
            ddlMedGroups.DataTextField = "Name"
            ddlMedGroups.DataValueField = "Value"
            ddlMedGroups.DataBind()
            Dim TopSelection As New ListItem()
            TopSelection.Text = "--- Select One ---"
            TopSelection.Value = 0
            ddlMedGroups.Items.Insert(0, TopSelection)
            Dim NASelection As New ListItem() ' selection if no Med Group
            NASelection.Text = "N/A"
            NASelection.Value = -1
            ddlMedGroups.Items.Insert(1, NASelection)

            If Not IsNothing(SpecCase.MedGroupName) Then
                ddlMedGroups.SelectedValue = SpecCase.MedGroupName
            End If

            If Session(SESSIONKEY_COMPO) = "6" Then
                ddlRMUs.DataSource = From n In lkupDAO.GetRMUNames("") Where n.Value < 100 Select n
            Else
                ddlRMUs.DataSource = From n In lkupDAO.GetRMUNames("") Where n.Value >= 100 Select n
            End If

            ddlRMUs.DataTextField = "Name"
            ddlRMUs.DataValueField = "Value"
            ddlRMUs.DataBind()
            ddlRMUs.Items.Insert(0, TopSelection)
            If Not IsNothing(SpecCase.RMUName) Then
                ddlRMUs.SelectedValue = SpecCase.RMUName
            End If

            DAWGRecommendation.Items(5).Text = "Full WWD"

            If Not IsNothing(SpecCase.ICD9Code) Then
                If (Not isPostBack) Then
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
                End If

                DiagnosisTextBox.Text = HTMLDecodeNulls(SpecCase.FTDiagnosis)

                If (SpecCase.ICD9Code = 0) Then
                    FillValues = False
                    FTsubType = "IRILO - Not Eligible"
                    'FTsubType = FTType & "Fast Track - Not Eligible, requires Full " & FTType & " Workflow"
                ElseIf (IsICDCodeOfType("Diabetes", SpecCase.ICD9Code)) Then
                    '250 is actual ICD Code
                    Diabetes.Visible = True
                    FTsubType = "IRILO - Type II Diabetes [Non-Insulin Dependent]"
                    'FTsubType = FTType & "Fast Track - Type II Diabetes [Non-Insulin Dependent]"
                    lblDiabetes.Text = FTsubType
                ElseIf (IsICDCodeOfType("Asthma", SpecCase.ICD9Code)) Then
                    '493 is actual ICD Code
                    Asthma.Visible = True
                    FTsubType = "IRILO - Asthma"
                    'FTsubType = FTType & "Fast Track - Asthma"
                    lblAsthma.Text = FTsubType
                ElseIf (IsICDCodeOfType("Sleep", SpecCase.ICD9Code)) Then
                    '327 is actual ICD Code
                    SleepApnea.Visible = True
                    FTsubType = "IRILO - Obstructive Sleep Apnea (OSA)"
                    'FTsubType = FTType & "Fast Track - Obstructive Sleep Apnea (OSA)"
                    lblSleepApnea.Text = FTsubType
                Else
                    Dim code As ICD9Code = LookupService.GetIcd9CodeById(SpecCase.ICD9Code)
                    MostCases.Visible = True
                    FTsubType = "IRILO - General (" & Server.HtmlEncode(code.Description) & ")"
                    'FTsubType = FTType & "Fast Track - General (" & DiagnosisTextBox.Text & ")"
                    lblMostCases.Text = FTsubType
                End If

                'Select Case SpecCase.ICD9Code
                '    Case 0
                '        FillValues = False
                '        FTsubType = "IRILO - Not Eligible"
                '        'FTsubType = FTType & "Fast Track - Not Eligible, requires Full " & FTType & " Workflow"
                '    Case 251
                '        '250 is actual ICD Code
                '        Diabetes.Visible = True
                '        FTsubType = "IRILO - Type II Diabetes [Non-Insulin Dependent]"
                '        'FTsubType = FTType & "Fast Track - Type II Diabetes [Non-Insulin Dependent]"
                '        lblDiabetes.Text = FTsubType
                '    Case 1045
                '        '327 is actual ICD Code
                '        SleepApnea.Visible = True
                '        FTsubType = "IRILO - Obstructive Sleep Apnea (OSA)"
                '        'FTsubType = FTType & "Fast Track - Obstructive Sleep Apnea (OSA)"
                '        lblSleepApnea.Text = FTsubType
                '    Case 498
                '        '493 is actual ICD Code
                '        Asthma.Visible = True
                '        FTsubType = "IRILO - Asthma"
                '        'FTsubType = FTType & "Fast Track - Asthma"
                '        lblAsthma.Text = FTsubType
                '    Case Else
                '        MostCases.Visible = True
                '        FTsubType = "IRILO - General (" & Server.HtmlEncode(DiagnosisTextBox.Text) & ")"
                '        'FTsubType = FTType & "Fast Track - General (" & DiagnosisTextBox.Text & ")"
                '        lblMostCases.Text = FTsubType
                'End Select

                lblFastTrackAll.Text = FTsubType
                Session("FTSubType") = FTsubType
            Else
                If (Not isPostBack) Then
                    ucICD7thCharacterControl.InitializeCharacters(0, String.Empty)
                End If
            End If

            'Load Findings
            MemberName.Text = SpecCase.MemberName
            MemberRank.Text = SpecCase.MemberRank.Grade
            ProtectedSSN.Text = "*****" & Right(SpecCase.MemberSSN, 4)

            If FillValues Then
                If (SpecCase.TMTReceiveDate.HasValue) Then
                    TMTReceiveDate.Text = Server.HtmlDecode(SpecCase.TMTReceiveDate.Value.ToString(DATE_FORMAT))
                End If
                DAFSC.Text = HTMLDecodeNulls(SpecCase.DAFSC)
                If SpecCase.YearsSatisfactoryService.HasValue Then
                    YearsSatisfactoryService.SelectedValue = SpecCase.YearsSatisfactoryService
                End If
                If SpecCase.BodyMassIndex.HasValue Then
                    BodyMassIndex.Text = HTMLDecodeNulls(SpecCase.BodyMassIndex)
                End If
                If SpecCase.MissedWorkDays.HasValue Then
                    MissedWorkDays.Text = SpecCase.MissedWorkDays
                End If
                If SpecCase.RequiresSpecialistForMgmt.HasValue Then
                    SpecialistRequired.SelectedValue = SpecCase.RequiresSpecialistForMgmt
                End If
                If SpecCase.HasHadERorUrgentCareVisits = 1 Then
                    ERorUrgentCareY.Checked = True
                    Label11.CssClass = "label labelRequired"
                End If
                If SpecCase.HasHadERorUrgentCareVisits = 0 Then
                    ERorUrgentCareN.Checked = True
                    Label11.CssClass = "label"
                End If
                If Not IsNothing(SpecCase.ERorUrgentCareVisitList) Then
                    ERorUrgentCareDetails.Text = HTMLDecodeNulls(SpecCase.ERorUrgentCareVisitList)
                End If
                HospitalizationDetails.Text = HTMLDecodeNulls(SpecCase.HospitalizationList)
                If SpecCase.HasBeenHospitalized = 1 Then
                    HospitalizationsY.Checked = True
                    Label13.CssClass = "label labelRequired"
                End If
                If SpecCase.HasBeenHospitalized = 0 Then
                    HospitalizationsN.Checked = True
                    Label13.CssClass = "label"
                End If
                If SpecCase.RiskForSuddenIncapacitation.HasValue Then
                    IncapacitationRisk.SelectedValue = SpecCase.RiskForSuddenIncapacitation
                End If

                If (SpecCase.RecommendedFollowUpInterval.HasValue) Then
                    FollowUpInterval.SelectedValue = SpecCase.RecommendedFollowUpInterval
                End If

                If SpecCase.DAWGRecommendation.HasValue Then
                    DAWGRecommendation.SelectedValue = SpecCase.DAWGRecommendation
                End If

                If SpecCase.DxInterferenceWithDuties = 1 Then
                    DutyInterferenceY.Checked = True
                    'rdOnly = True
                End If
                If SpecCase.DxInterferenceWithDuties = 0 Then
                    DutyInterferenceN.Checked = True
                End If
                Prognosis.Text = HTMLDecodeNulls(SpecCase.FTPrognosis)
                Treatment.Text = HTMLDecodeNulls(SpecCase.FTTreatment)
                MedicationDosages.Text = HTMLDecodeNulls(SpecCase.FTMedicationsAndDosages)

                'Sleep Apnea
                If SpecCase.DaytimeSomnolence = 1 Then
                    DaytimeSomnolenceY.Checked = True
                End If
                If SpecCase.DaytimeSomnolence = 0 Then
                    DaytimeSomnolenceN.Checked = True
                End If
                DaytimeSomnolenceDetails.Text = HTMLDecodeNulls(SpecCase.DaySleepDescription)
                ApneaEpisodeDetails.Text = HTMLDecodeNulls(SpecCase.ApneaEpisodeDescription)
                If SpecCase.HasApneaEpisodes = 1 Then
                    ApneaEpisodesY.Checked = True
                End If
                If SpecCase.HasApneaEpisodes = 0 Then
                    ApneaEpisodesN.Checked = True
                End If
                If SpecCase.SleepStudyResults = 1 Then
                    SleepStudyY.Checked = True
                End If
                If SpecCase.SleepStudyResults = 0 Then
                    SleepStudyN.Checked = True
                End If
                If SpecCase.OralDevicesUsed = 1 Then
                    OralDevicesY.Checked = True
                End If
                If SpecCase.OralDevicesUsed = 0 Then
                    OralDevicesN.Checked = True
                End If
                If SpecCase.CPAPRequired = 1 Then
                    CPAPRequiredY.Checked = True
                End If
                If SpecCase.CPAPRequired = 0 Then
                    CPAPRequiredN.Checked = True
                End If

                If SpecCase.BIPAPRequired = 1 And SpecCase.ICD9Code = 1045 Then
                    BIPAPRequiredY.Checked = True
                    rdOnly = True
                End If

                If SpecCase.BIPAPRequired = 0 And SpecCase.ICD9Code = 1045 Then
                    BIPAPRequiredN.Checked = True
                    rdOnly = False
                End If
                If SpecCase.ResponseToDevices = 1 Then
                    SystemResponseY.Checked = True
                End If
                If SpecCase.ResponseToDevices = 0 Then
                    SystemResponseN.Checked = True
                End If

                'Diabetes
                If SpecCase.FastingBloodSugar.HasValue Then
                    FastingBloodSugar.SelectedValue = Convert.ToInt32(SpecCase.FastingBloodSugar).ToString()
                End If
                If SpecCase.HgbA1C.HasValue Then
                    HgbA1C.SelectedValue = Convert.ToInt32(SpecCase.HgbA1C).ToString()
                End If
                If SpecCase.CurrentOptometryExam = 1 Then
                    OptometryExamY.Checked = True
                End If
                If SpecCase.CurrentOptometryExam = 0 Then
                    OptometryExamN.Checked = True
                End If
                If SpecCase.HasOtherSignificantConditions = 1 Then
                    OtherConditionsY.Checked = True
                End If
                If SpecCase.HasOtherSignificantConditions = 0 Then
                    OtherConditionsN.Checked = True
                End If
                OtherConditionsList.Text = HTMLDecodeNulls(SpecCase.OtherSignificantConditionsList)
                If SpecCase.ControlledWithOralAgents = 1 Then
                    OralAgentsY.Checked = True
                End If
                If SpecCase.ControlledWithOralAgents = 0 Then
                    OralAgentsN.Checked = True
                End If
                OralAgentsList.Text = HTMLDecodeNulls(SpecCase.OralAgentsList)
                If SpecCase.RequiresInsulin = 1 Then
                    RequiresInsulinY.Checked = True
                End If
                If SpecCase.RequiresInsulin = 0 Then
                    RequiresInsulinN.Checked = True
                End If
                InsulinDosageRegime.Text = HTMLDecodeNulls(SpecCase.InsulinDosageRegime)
                If SpecCase.RequiresNonInsulinMed = 1 Then
                    RequiresNonInsulinY.Checked = True
                End If
                If SpecCase.RequiresNonInsulinMed = 0 Then
                    RequiresNonInsulinN.Checked = True
                End If

                'Asthma
                If SpecCase.PulmonaryFunctionTest = 1 Then
                    PFTY.Checked = True
                End If
                If SpecCase.PulmonaryFunctionTest = 0 Then
                    PFTN.Checked = True
                End If
                If SpecCase.MethacholineChallenge = 1 Then
                    MethacholineChallengeY.Checked = True
                End If
                If SpecCase.MethacholineChallenge = 2 Then
                    MethacholineChallengeN.Checked = True
                End If
                If SpecCase.MethacholineChallenge = 3 Then
                    MethacholineChallengeP.Checked = True
                End If
                If SpecCase.RequiresDailySteroids = 1 Then
                    RequiresSteroidsY.Checked = True
                End If
                If SpecCase.RequiresDailySteroids = 0 Then
                    RequiresSteroidsN.Checked = True
                End If
                If SpecCase.DailySteroidDosage.HasValue Then
                    DailySteroidsDosage.Text = HTMLDecodeNulls(SpecCase.DailySteroidDosage)
                End If
                If SpecCase.RescueInhalerUsageFrequency.HasValue Then
                    FrequencyOfRescueInhalerUsage.SelectedValue = SpecCase.RescueInhalerUsageFrequency
                End If
                If SpecCase.SymptomsExacerbatedByColdExercise = 1 Then
                    ColdExerciseExacerbationY.Checked = True
                End If
                If SpecCase.SymptomsExacerbatedByColdExercise = 0 Then
                    ColdExerciseExacerbationN.Checked = True
                End If
                ColdExerciseExacerbationDetails.Text = HTMLDecodeNulls(SpecCase.ExerciseColdSymptomDescription)
                If SpecCase.HOIntubation = 1 Then
                    HOIntubationY.Checked = True
                End If
                If SpecCase.ExacerbatedSymptomsRequireOralSteroids = 1 Then
                    ExacerbatedSymptomsOralSteroidsY.Checked = True
                End If
                If SpecCase.ExacerbatedSymptomsRequireOralSteroids = 0 Then
                    ExacerbatedSymptomsOralSteroidsN.Checked = True
                End If
                ExacerbatedSymptomsOralSteroidsDosage.Text = HTMLDecodeNulls(SpecCase.ExacerbatedSymptomsOralSteroidsDosage)
                If SpecCase.NormalPFTwithTreatment = 1 Then
                    NormalPFTWithTreatmentY.Checked = True
                End If
                If SpecCase.NormalPFTwithTreatment = 0 Then
                    NormalPFTWithTreatmentN.Checked = True
                End If
                If SpecCase.HOIntubation = 0 Then
                    HOIntubationN.Checked = True
                End If
            End If

            If rdOnly Then
                DisplayReadOnly(1, isPostBack)
            Else
                DisplayReadWrite(isPostBack)
            End If

            'Load Findings
            If Not String.IsNullOrEmpty(SpecCase.PocEmail) Then
                txtPOCEmailLabel.Text = SpecCase.PocEmail
            End If
            If Not String.IsNullOrEmpty(SpecCase.PocPhoneDSN) Then
                txtPOCPhoneLabel.Text = SpecCase.PocPhoneDSN
            End If
            If Not String.IsNullOrEmpty(SpecCase.PocRankAndName) Then
                txtPOCNameLabel.Text = SpecCase.PocRankAndName
            End If

        End Sub

        Private Function IsICDCodeOfType(ByVal typeName As String, ByVal icdID As Integer) As Boolean
            Dim lkupDAO As ILookupDao = New NHibernateDaoFactory().GetLookupDao()
            Dim ids As List(Of Integer) = lkupDAO.GetIRILOTypeICDCodeIds(typeName)

            If (ids Is Nothing) Then
                Return False
            End If

            If (ids.Contains(icdID)) Then
                Return True
            End If

            Return False
        End Function

        Private Sub SaveFindings()

            If (Not UserCanEdit) Then
                Exit Sub
            End If

            Dim PageAccess As ALOD.Core.Domain.Workflow.PageAccessType
            PageAccess = SectionList(FTSectionNames.FT_RTD.ToString())
            '*************************************************************

            If ddlMedGroups.SelectedIndex > 0 Then
                SpecCase.MedGroupName = ddlMedGroups.SelectedValue
            End If

            If ddlRMUs.SelectedIndex > 0 Then
                SpecCase.RMUName = ddlRMUs.SelectedValue
            End If

            'Load Findings
            If String.IsNullOrEmpty(TMTReceiveDate.Text) Then
                SpecCase.TMTReceiveDate = Nothing
            Else
                SpecCase.TMTReceiveDate = Server.HtmlEncode(TMTReceiveDate.Text)
            End If

            SpecCase.DAFSC = HTMLEncodeNulls(DAFSC.Text)

            SpecCase.YearsSatisfactoryService = YearsSatisfactoryService.SelectedValue
            SpecCase.BodyMassIndex = HTMLEncodeNulls(BodyMassIndex.Text, True)
            SpecCase.MissedWorkDays = MissedWorkDays.Text
            SpecCase.RequiresSpecialistForMgmt = SpecialistRequired.SelectedValue
            If ERorUrgentCareY.Checked Then
                SpecCase.HasHadERorUrgentCareVisits = 1
                SpecCase.ERorUrgentCareVisitList = HTMLEncodeNulls(ERorUrgentCareDetails.Text, True)
            End If
            If ERorUrgentCareN.Checked Then
                SpecCase.HasHadERorUrgentCareVisits = 0
            End If
            If HospitalizationsY.Checked Then
                SpecCase.HasBeenHospitalized = 1
                SpecCase.HospitalizationList = HTMLEncodeNulls(HospitalizationDetails.Text, True)
            End If
            If HospitalizationsN.Checked Then
                SpecCase.HasBeenHospitalized = 0
            End If
            SpecCase.RiskForSuddenIncapacitation = IncapacitationRisk.SelectedValue
            SpecCase.RecommendedFollowUpInterval = FollowUpInterval.SelectedValue
            SpecCase.DAWGRecommendation = DAWGRecommendation.SelectedValue

            If DutyInterferenceY.Checked Then
                SpecCase.DxInterferenceWithDuties = 1
            End If
            If DutyInterferenceN.Checked Then
                SpecCase.DxInterferenceWithDuties = 0
            End If
            SpecCase.FTPrognosis = HTMLEncodeNulls(Prognosis.Text, True)
            SpecCase.FTTreatment = HTMLEncodeNulls(Treatment.Text, True)
            SpecCase.FTMedicationsAndDosages = HTMLEncodeNulls(MedicationDosages.Text, True)

            'Sleep Apnea
            If DaytimeSomnolenceY.Checked Then
                SpecCase.DaytimeSomnolence = 1
                SpecCase.DaySleepDescription = HTMLEncodeNulls(DaytimeSomnolenceDetails.Text, True)
            End If
            If DaytimeSomnolenceN.Checked Then
                SpecCase.DaytimeSomnolence = 0
            End If
            If ApneaEpisodesY.Checked Then
                SpecCase.HasApneaEpisodes = 1
                SpecCase.ApneaEpisodeDescription = HTMLEncodeNulls(ApneaEpisodeDetails.Text, True)
            End If
            If ApneaEpisodesN.Checked Then
                SpecCase.HasApneaEpisodes = 0
            End If
            If SleepStudyY.Checked Then
                SpecCase.SleepStudyResults = 1
            End If
            If SleepStudyN.Checked Then
                SpecCase.SleepStudyResults = 0
            End If
            If OralDevicesY.Checked Then
                SpecCase.OralDevicesUsed = 1
            End If
            If OralDevicesN.Checked Then
                SpecCase.OralDevicesUsed = 0
            End If
            If CPAPRequiredY.Checked Then
                SpecCase.CPAPRequired = 1
            End If
            If CPAPRequiredN.Checked Then
                SpecCase.CPAPRequired = 0
            End If
            If BIPAPRequiredY.Checked Then
                SpecCase.BIPAPRequired = 1
            End If
            If BIPAPRequiredN.Checked Then
                SpecCase.BIPAPRequired = 0
            End If
            If SystemResponseY.Checked Then
                SpecCase.ResponseToDevices = 1
            End If
            If SystemResponseN.Checked Then
                SpecCase.ResponseToDevices = 0
            End If

            'Diabetes
            SpecCase.FastingBloodSugar = FastingBloodSugar.SelectedValue
            SpecCase.HgbA1C = HgbA1C.SelectedValue
            If OptometryExamY.Checked Then
                SpecCase.CurrentOptometryExam = 1
            End If
            If OptometryExamN.Checked Then
                SpecCase.CurrentOptometryExam = 0
            End If
            If OtherConditionsY.Checked Then
                SpecCase.HasOtherSignificantConditions = 1
            End If
            If OtherConditionsN.Checked Then
                SpecCase.HasOtherSignificantConditions = 0
            End If
            SpecCase.OtherSignificantConditionsList = HTMLEncodeNulls(OtherConditionsList.Text, True)
            If OralAgentsY.Checked Then
                SpecCase.ControlledWithOralAgents = 1
            End If
            If OralAgentsN.Checked Then
                SpecCase.ControlledWithOralAgents = 0
            End If
            SpecCase.OralAgentsList = HTMLEncodeNulls(OralAgentsList.Text, True)
            If RequiresInsulinY.Checked Then
                SpecCase.RequiresInsulin = 1
            End If
            If RequiresInsulinN.Checked Then
                SpecCase.RequiresInsulin = 0
            End If
            SpecCase.InsulinDosageRegime = HTMLEncodeNulls(InsulinDosageRegime.Text, True)
            If RequiresNonInsulinY.Checked Then
                SpecCase.RequiresNonInsulinMed = 1
            End If
            If RequiresNonInsulinN.Checked Then
                SpecCase.RequiresNonInsulinMed = 0
            End If

            'Asthma
            If PFTY.Checked Then
                SpecCase.PulmonaryFunctionTest = 1
            End If
            If PFTN.Checked Then
                SpecCase.PulmonaryFunctionTest = 0
            End If
            If MethacholineChallengeY.Checked Then
                SpecCase.MethacholineChallenge = 1
            End If
            If MethacholineChallengeN.Checked Then
                SpecCase.MethacholineChallenge = 2
            End If
            If MethacholineChallengeP.Checked Then
                SpecCase.MethacholineChallenge = 3
            End If
            If RequiresSteroidsY.Checked Then
                SpecCase.RequiresDailySteroids = 1
            End If
            If RequiresSteroidsN.Checked Then
                SpecCase.RequiresDailySteroids = 0
            End If
            SpecCase.DailySteroidDosage = HTMLEncodeNulls(DailySteroidsDosage.Text, True)
            SpecCase.RescueInhalerUsageFrequency = FrequencyOfRescueInhalerUsage.SelectedValue
            If ColdExerciseExacerbationY.Checked Then
                SpecCase.SymptomsExacerbatedByColdExercise = 1
            End If
            If ColdExerciseExacerbationN.Checked Then
                SpecCase.SymptomsExacerbatedByColdExercise = 0
            End If
            SpecCase.ExerciseColdSymptomDescription = HTMLEncodeNulls(ColdExerciseExacerbationDetails.Text, True)
            If ExacerbatedSymptomsOralSteroidsY.Checked Then
                SpecCase.ExacerbatedSymptomsRequireOralSteroids = 1
            End If
            If ExacerbatedSymptomsOralSteroidsN.Checked Then
                SpecCase.ExacerbatedSymptomsRequireOralSteroids = 0
            End If
            SpecCase.ExacerbatedSymptomsOralSteroidsDosage = HTMLEncodeNulls(ExacerbatedSymptomsOralSteroidsDosage.Text, True)
            If NormalPFTWithTreatmentY.Checked Then
                SpecCase.NormalPFTwithTreatment = 1
            End If
            If NormalPFTWithTreatmentN.Checked Then
                SpecCase.NormalPFTwithTreatment = 0
            End If
            If HOIntubationY.Checked Then
                SpecCase.HOIntubation = 1
            End If
            If HOIntubationN.Checked Then
                SpecCase.HOIntubation = 0
            End If

            ChangeContent()

            If String.IsNullOrEmpty(_icd) Then
                'If InStr(cddContent.SelectedValue, ":") > 0 Then
                '    _icd = Left(cddContent.SelectedValue, InStr(cddContent.SelectedValue, ":") - 1)
                '    Dim temp As String = ddlICDContent.SelectedValue
                'Else
                '    _icd = "0"
                'End If
                If (ucICDCodeControl.IsICDCodeSelected()) Then
                    _icd = ucICDCodeControl.SelectedICDCodeID.ToString()
                Else
                    _icd = "0"
                End If
            End If

            'If Not String.IsNullOrEmpty(txtPOCNameLabel.Text) Then
            '	SpecCase.RMUName = Server.HtmlEncode(txtPOCNameLabel.Text)
            'End If

            If Not String.IsNullOrEmpty(txtPOCPhoneLabel.Text) Then
                SpecCase.PocPhoneDSN = Server.HtmlEncode(txtPOCPhoneLabel.Text)
            End If

            If Not String.IsNullOrEmpty(txtPOCEmailLabel.Text) Then
                SpecCase.PocEmail = Server.HtmlEncode(txtPOCEmailLabel.Text)
            End If

            If Not String.IsNullOrEmpty(txtPOCNameLabel.Text) Then
                SpecCase.PocRankAndName = Server.HtmlEncode(txtPOCNameLabel.Text)
            End If

            If _icd = "0" Then
                'SpecCase.ICD9Code = Nothing
            Else
                SpecCase.ICD9Code = _icd
            End If

            If String.IsNullOrEmpty(FTsubType) Then
                FTsubType = Session("FTSubType")
            End If
            SpecCase.ICD9Description = FTsubType

            If (ucICD7thCharacterControl.Is7thCharacterSelected()) Then
                SpecCase.ICD7thCharacter = ucICD7thCharacterControl.Selected7thCharacter
            Else
                SpecCase.ICD7thCharacter = Nothing
            End If

            If (Not IsNothing(SpecCase.ICD9Code)) Then
                ucICDCodeControl.UpdateICDCodeDiagnosisLabel(SpecCase.ICD9Code, UserCanEdit)
            End If

            SpecCase.FTDiagnosis = HTMLEncodeNulls(DiagnosisTextBox.Text, True)

            '***************************************************************
            SCDao.SaveOrUpdate(SpecCase)
            SCDao.CommitChanges()
        End Sub

#Region "TabEvent"

        Public Function ValidBoxLength() As Boolean
            Dim IsValid As Boolean = True
            If Not CheckTextLength(DiagnosisTextBox) Then
                IsValid = False
            End If
            If Not CheckTextLength(HospitalizationDetails) Then
                IsValid = False
            End If
            If Not CheckTextLength(ERorUrgentCareDetails) Then
                IsValid = False
            End If
            If Not CheckTextLength(Prognosis) Then
                IsValid = False
            End If
            If Not CheckTextLength(Treatment) Then
                IsValid = False
            End If
            If Not CheckTextLength(MedicationDosages) Then
                IsValid = False
            End If
            If Not CheckTextLength(DaytimeSomnolenceDetails) Then
                IsValid = False
            End If
            If Not CheckTextLength(ApneaEpisodeDetails) Then
                IsValid = False
            End If
            If Not CheckTextLength(OtherConditionsList) Then
                IsValid = False
            End If
            If Not CheckTextLength(OralAgentsList) Then
                IsValid = False
            End If
            If Not CheckTextLength(InsulinDosageRegime) Then
                IsValid = False
            End If
            If Not CheckTextLength(ColdExerciseExacerbationDetails) Then
                IsValid = False
            End If
            If Not CheckTextLength(ExacerbatedSymptomsOralSteroidsDosage) Then
                IsValid = False
            End If
            Return IsValid
        End Function

        Protected Sub Page_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
            AddHandler Me.Master.TabClick, AddressOf TabButtonClicked
        End Sub

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