Imports ALOD.Core.Domain.Common
Imports ALOD.Core.Domain.Lookup
Imports ALOD.Core.Domain.Modules.Lod
Imports ALOD.Core.Domain.Users
Imports ALOD.Core.Domain.Workflow
Imports ALOD.Core.Interfaces.DAOInterfaces
Imports ALOD.Core.Utils
Imports ALOD.Data
Imports ALOD.Data.Services
Imports ALOD.Logging
Imports ALOD.Web.UserControls
Imports ALODWebUtility.Common
Imports ALODWebUtility.LookUps

Namespace Web.LOD
    Partial Class Secure_lod_c2_Medical
        Inherits System.Web.UI.Page

#Region "fields/properties"
        Private _provider As New LookUp
        Private _daoFactory As IDaoFactory
        Private _lookupDao As ILookupDao
        Private _lineOfDutyMedicalDao As ILineOfDutyMedicalDao
        Private _lineOfDutyUnitDao As ILineOfDutyUnitDao

        Protected ReadOnly Property DaoFactory() As IDaoFactory
            Get
                If (_daoFactory Is Nothing) Then
                    _daoFactory = New NHibernateDaoFactory()
                End If

                Return _daoFactory
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

        Protected ReadOnly Property LineOfDutyMedicalDao() As ILineOfDutyMedicalDao
            Get
                If (_lineOfDutyMedicalDao Is Nothing) Then
                    _lineOfDutyMedicalDao = DaoFactory.GetLineOfDutyMedicalDao()
                End If

                Return _lineOfDutyMedicalDao
            End Get
        End Property

        Protected ReadOnly Property LineOfDutyUnitDao() As ILineOfDutyUnitDao
            Get
                If (_lineOfDutyUnitDao Is Nothing) Then
                    _lineOfDutyUnitDao = DaoFactory.GetLineOfDutyUnitDao()
                End If

                Return _lineOfDutyUnitDao
            End Get
        End Property

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

        Public ReadOnly Property refId() As Integer
            Get
                Return Integer.Parse(Request.QueryString("refId"))
            End Get
        End Property

        Protected ReadOnly Property MasterPage() As LodMaster
            Get
                Dim master As LodMaster = CType(Page.Master, LodMaster)
                Return master
            End Get
        End Property

        Public Property IsReadOnly() As Boolean
            Get
                Return ViewState("LodMedical_IsReadonly")
            End Get
            Set(ByVal value As Boolean)
                ViewState("LodMedical_IsReadonly") = value
            End Set

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
#End Region

        Protected Sub Page_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
            AddHandler Me.Master.TabClick, AddressOf TabButtonClicked
            AddHandler ucICDCodeControl.ICDCodeSelectionChanged, AddressOf DataBindNatureOfIncidentDDLEventHandler
            AddHandler ucICDCodeControl_v2.ICDCodeSelectionChanged, AddressOf ICDCodeSelectionChangedEventHandler_v2
        End Sub

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
            InitializeUserControls()

            If (Not Page.IsPostBack) Then
                Dim lod As LineOfDuty = LodService.GetById(refId)

                UserCanEdit = GetAccessLOD(Navigator.PageAccess, True, lod)

                If UserCanEdit Then
                    page_readOnly.Value = "1"
                Else
                    page_readOnly.Value = "0"
                End If

                If (lod.Workflow = 1) Then
                    InitControls()
                Else
                    InitControls_v2()
                End If
            End If
        End Sub

        Private Sub InitializeUserControls()
            ucICDCodeControl.Initialilze(Me)
            ucICD7thCharacterControl.Initialize(ucICDCodeControl)
            ucICDCodeControl_v2.Initialilze(Me)
            ucICD7thCharacterControl_v2.Initialize(ucICDCodeControl_v2)
        End Sub

#Region "LOD"
        Private Sub InitControls()
            If (UserCanEdit) Then
                TreatmentDateBox.CssClass = "datePicker"
                SigCheck.Visible = False
            Else
                SigCheck.VerifySignature(refId)
            End If

            SetMaxLengthForControls()
            SetInputFormatRestrictions()
            LoadData()

            TabControl.Item(NavigatorButtonType.Save).Visible = UserCanEdit
            LogManager.LogAction(ModuleType.LOD, UserAction.ViewPage, refId, "Viewed Page: Medical")

            OriginalLOD.Visible = True
        End Sub

        Private Sub SetMaxLengthForControls()
            SetMaxLength(EventDetailsBox)
            SetMaxLength(HospitalNameBox)
            SetMaxLength(DiagnosisTextBox)
        End Sub

        Private Sub SetInputFormatRestrictions()
            SetInputFormatRestriction(Page, EventDetailsBox, FormatRestriction.AlphaNumeric, PERMITTED_SPECIAL_CHAR_INPUT)
            SetInputFormatRestriction(Page, HospitalNameBox, FormatRestriction.AlphaNumeric, PERMITTED_SPECIAL_CHAR_INPUT)
            SetInputFormatRestriction(Page, DiagnosisTextBox, FormatRestriction.AlphaNumeric, PERMITTED_SPECIAL_CHAR_INPUT)
            SetInputFormatRestriction(Page, txtApprovalComments, FormatRestriction.AlphaNumeric, PERMITTED_SPECIAL_CHAR_INPUT)
            SetInputFormatRestriction(Page, TreatmentDateBox, FormatRestriction.Numeric, "/")
            SetInputFormatRestriction(Page, TreatmentHourBox, FormatRestriction.Numeric)
        End Sub

        Private Sub LoadData()
            Dim medicalInfo As LineOfDutyMedical = LineOfDutyMedicalDao.GetById(refId, False)

            If (UserCanEdit) Then
                DisplayReadWrite(medicalInfo)
            Else
                DisplayReadOnly(medicalInfo)
            End If
        End Sub

        Protected Sub DisplayReadOnly(ByVal medical As LineOfDutyMedical)
            If (IsPostBack) Then
                Exit Sub
            End If

            HideEditableControls()

            'populate the labels
            MemberStatusLabel.Text = medical.MemberStatus
            IncidentTypeLabel.Text = medical.NatureOfEvent
            HospitalTypeLabel.Text = medical.MedicalFacilityType
            If (Not String.IsNullOrEmpty(medical.MedicalFacility)) Then
                HospitalNameLabel.Text = medical.MedicalFacility.Replace(vbCrLf, "<br />")
            End If

            If (medical.ICD9Id.HasValue) Then
                If (ucICDCodeControl.IsValidICDCode(medical.ICD9Id)) Then
                    ucICDCodeControl.UpdateICDCodeDiagnosisLabel(medical.ICD9Id, False)
                End If

                If (Not String.IsNullOrEmpty(medical.ICD7thCharacter)) Then
                    ucICD7thCharacterControl.Update7thCharacterLabel(medical.ICD9Id, medical.ICD7thCharacter)
                End If
            End If

            DiagnosisLabel.Text = medical.DiagnosisText

            If (medical.TreatmentDate.HasValue) Then
                TreatmentDateLabel.Text = medical.TreatmentDate.Value.ToString(DATE_HOUR_FORMAT)
            End If

            DeathCaseLabel.Text = medical.DeathInvolved
            MvaLabel.Text = medical.MvaInvolved

            If medical.Epts.HasValue Then
                Select Case medical.Epts.Value
                    Case 0
                        lblEPTS.Text = "No"
                    Case 1
                        lblEPTS.Text = "EPTS Yes/Service Aggravated"
                    Case 2
                        lblEPTS.Text = "EPTS Yes/Not Service Aggravated"
                    Case Else

                End Select
            End If

            rblEPTS.Visible = False

            txtApprovalComments.Visible = False
            lblApprovalComments.Text = medical.ApprovalComments

            If (Not String.IsNullOrEmpty(medical.EventDetails)) Then
                EventDetailsLabel.Text = medical.EventDetails.Replace(vbCrLf, "<br />") + "<br /><br /><br />"
            End If
        End Sub

        Private Sub HideEditableControls()
            MemberStatusSelect.Visible = False
            IncidentTypeSelect.Visible = False
            ucICDCodeControl.DisplayReadOnly(True)
            ucICD7thCharacterControl.DisplayReadOnly()
            HospitalTypeSelect.Visible = False
            HospitalNameBox.Visible = False
            TreatmentDateBox.Visible = False
            TreatmentHourBox.Visible = False
            EventDetailsBox.Visible = False
            DeathCaseChoice.Visible = False
            MvaInvolvedChoice.Visible = False
            DiagnosisTextBox.Visible = False
        End Sub

        Protected Sub DisplayReadWrite(ByVal medical As LineOfDutyMedical)
            ucICDCodeControl.DisplayReadWrite(False)
            ucICD7thCharacterControl.DisplayReadWrite()

            If (medical.ICD9Id.HasValue) Then
                ucICDCodeControl.InitializeHierarchy(medical.ICD9Id)

                If (ucICDCodeControl.IsValidICDCode(medical.ICD9Id)) Then
                    ucICDCodeControl.UpdateICDCodeDiagnosisLabel(medical.ICD9Id, True)
                End If

                If (Not String.IsNullOrEmpty(medical.ICD7thCharacter)) Then
                    ucICD7thCharacterControl.InitializeCharacters(medical.ICD9Id, medical.ICD7thCharacter)
                Else
                    ucICD7thCharacterControl.InitializeCharacters(medical.ICD9Id, String.Empty)
                End If

                DataBindNatureOfIncidentDDL(medical.ICD9Id)
            Else
                ucICD7thCharacterControl.InitializeCharacters(0, String.Empty)
                DataBindNatureOfIncidentDDL(0)
            End If

            SetDropdownByValue(HospitalTypeSelect, medical.MedicalFacilityType)
            SetDropdownByValue(MemberStatusSelect, medical.MemberStatus)
            HospitalNameBox.Text = Server.HtmlDecode(medical.MedicalFacility)

            If (medical.TreatmentDate.HasValue) Then
                TreatmentDateBox.Text = Server.HtmlDecode(medical.TreatmentDate.Value.ToString(DATE_FORMAT))
                TreatmentHourBox.Text = Server.HtmlDecode(medical.TreatmentDate.Value.ToString(HOUR_FORMAT))
            End If

            DiagnosisTextBox.Text = Server.HtmlDecode(medical.DiagnosisText)
            EventDetailsBox.Text = Server.HtmlDecode(medical.EventDetails)

            SetRadioList(DeathCaseChoice, medical.DeathInvolved)
            SetRadioList(MvaInvolvedChoice, medical.MvaInvolved)

            If medical.Epts.HasValue Then
                rblEPTS.SelectedValue = medical.Epts.Value
            End If

            txtApprovalComments.Text = Server.HtmlDecode(medical.ApprovalComments)

            If ((CInt(Session("groupId")) <> UserGroups.MedicalOfficer) And
                (CInt(Session("groupId")) <> UserGroups.MedOffSARC)) Then

                ApprovalCommentsRow.Visible = False
                EptsRow.Visible = False
            End If

            If (CInt(Session("groupId")) = UserGroups.MedicalTechnician) Then
                ApprovalCommentsRow.Visible = True
                txtApprovalComments.Visible = False
                lblApprovalComments.Text = medical.ApprovalComments
            End If

            ValidBoxLength()
        End Sub

        Private Sub SaveData()
            If (Not UserCanEdit) Then
                Exit Sub
            End If

            Dim medicalInfo As LineOfDutyMedical = LineOfDutyMedicalDao.GetById(refId, False)

            medicalInfo.NatureOfEvent = IncidentTypeSelect.SelectedValue
            medicalInfo.MedicalFacilityType = HospitalTypeSelect.SelectedValue
            medicalInfo.MedicalFacility = Server.HtmlEncode(HospitalNameBox.Text)

            If (ucICDCodeControl.IsICDCodeSelected()) Then
                medicalInfo.ICD9Id = ucICDCodeControl.SelectedICDCodeID
            End If

            medicalInfo.DiagnosisText = Server.HtmlEncode(DiagnosisTextBox.Text.Trim())

            If (Not IsNothing(medicalInfo.ICD9Id)) Then
                ucICDCodeControl.UpdateICDCodeDiagnosisLabel(medicalInfo.ICD9Id, True)
            End If

            If (ucICD7thCharacterControl.Is7thCharacterSelected()) Then
                medicalInfo.ICD7thCharacter = ucICD7thCharacterControl.Selected7thCharacter
            Else
                medicalInfo.ICD7thCharacter = Nothing
            End If

            Try
                If (CheckDate(TreatmentDateBox)) Then
                    If (TreatmentHourBox.Text.Trim.Length > 0) Then
                        medicalInfo.TreatmentDate = Server.HtmlEncode(ParseDateAndTime(TreatmentDateBox.Text.Trim + " " + TreatmentHourBox.Text.Trim))
                    Else
                        medicalInfo.TreatmentDate = Server.HtmlEncode(DateTime.Parse(TreatmentDateBox.Text.Trim))
                    End If
                Else
                    medicalInfo.TreatmentDate = Nothing
                End If
            Catch ex As Exception
                medicalInfo.TreatmentDate = Nothing
            End Try

            medicalInfo.EventDetails = Server.HtmlEncode(EventDetailsBox.Text.Trim)
            medicalInfo.MemberStatus = MemberStatusSelect.SelectedValue
            medicalInfo.DeathInvolved = DeathCaseChoice.SelectedValue
            medicalInfo.MvaInvolved = MvaInvolvedChoice.SelectedValue

            medicalInfo.ModifiedBy = CInt(HttpContext.Current.Session("UserId"))
            medicalInfo.ModifiedDate = Now

            If rblEPTS.SelectedValue <> "" Then
                medicalInfo.Epts = rblEPTS.SelectedValue
            End If
            medicalInfo.ApprovalComments = Server.HtmlEncode(txtApprovalComments.Text.Trim())

            LineOfDutyMedicalDao.SaveOrUpdate(medicalInfo)
        End Sub

        Protected Sub DataBindNatureOfIncidentDDLEventHandler(ByVal sender As Object, ByVal e As ICDCodeSelectedEventArgs)
            DataBindNatureOfIncidentDDL(e.SelectedICDCodeId)
        End Sub

        Private Sub DataBindNatureOfIncidentDDL(ByVal codeId As Integer)
            IncidentTypeSelect.Items.Clear()
            IncidentTypeSelect.DataValueField = "Value"
            IncidentTypeSelect.DataTextField = "Text"
            IncidentTypeSelect.DataSource = _provider.GetICDIncident(codeId)
            IncidentTypeSelect.DataBind()

            IncidentTypeSelect.Items.Insert(0, New ListItem("-- Select Incident Code --", ""))

            Dim medicalInfo As LineOfDutyMedical = LineOfDutyMedicalDao.GetById(refId, False)

            If (IncidentTypeSelect.Items.Count > 0 AndAlso IncidentTypeSelect.Items.FindByValue(medicalInfo.NatureOfEvent) IsNot Nothing) Then
                IncidentTypeSelect.SelectedValue = medicalInfo.NatureOfEvent
            Else
                IncidentTypeSelect.SelectedIndex = 0
            End If

            If (IncidentTypeSelect.Items.Count > 1) Then
                IncidentTypeSelect.Enabled = True
            Else
                IncidentTypeSelect.Enabled = False
            End If
        End Sub
#End Region

#Region "LOD_v2"
        Private Sub InitControls_v2()
            If (UserCanEdit) Then
                InitDatePickerCSSClasses()
                SigCheck.Visible = False
            Else
                SigCheck.VerifySignature(refId)
            End If

            lblApprovalComments_v2.ToolTip = "Local Medical Opinion should be focused on the medical case for EPTS and if applicable for or against Service Aggravation.  If 1500 characters is not enough please attach your analysis to the documents tab and note ""See Documents for Local Medical Opinion"""

            InitComponentSelectDropdownList_v2()
            InitCategorySelectDropdownList_v2()
            InitMemberStatusSelectDropdownList_v2()
            InitFromSelectDropdownList_v2()
            InitHospitalTypeSelectDropdownList_v2()
            InitInfluenceSelectDropdownList_v2()

            SetMaxLengthForControls_v2()
            SetInputFormatRestrictions_v2()
            LoadData_v2()

            TabControl.Item(NavigatorButtonType.Save).Visible = UserCanEdit
            LogManager.LogAction(ModuleType.LOD, UserAction.ViewPage, refId, "Viewed Page: Medical")

            LOD_v2.Visible = True
        End Sub

        Private Sub InitDatePickerCSSClasses()
            TreatmentDateBox_v2.CssClass = "datePicker"
            PsychiatricDatePicker_v2.CssClass = "datePicker"
            OtherTestDatePicker_v2.CssClass = "datePicker"
            ComponentFromDate_v2.CssClass = "datePicker"
            ComponentToDate_v2.CssClass = "datePickerFuture"
        End Sub

        Private Sub InitComponentSelectDropdownList_v2()
            Dim lod As LineOfDuty_v2 = LodService.GetById(refId)

            If (lod.MemberRank.Title.Equals("Cadet")) Then
                ComponentSelect_v2.DataSource = From n In LookupDao.GetComponents() Select n Where n.Name.Contains("Cadet")
                ComponentSelect_v2.DataTextField = "Name"
                ComponentSelect_v2.DataValueField = "Value"
                ComponentSelect_v2.DataBind()
            Else
                ComponentSelect_v2.DataSource = From n In LookupDao.GetComponents() Select n Where Not (n.Name.Contains("Cadet"))
                ComponentSelect_v2.DataTextField = "Name"
                ComponentSelect_v2.DataValueField = "Value"
                ComponentSelect_v2.DataBind()
            End If
        End Sub

        Private Sub InitCategorySelectDropdownList_v2()
            CategorySelect_v2.DataSource = From n In LookupDao.GetCategory() Select n
            CategorySelect_v2.DataTextField = "Name"
            CategorySelect_v2.DataValueField = "Value"
            CategorySelect_v2.DataBind()
            InsertDropDownListZeroValue(CategorySelect_v2, "--- Select One ---")
        End Sub

        Private Sub InitMemberStatusSelectDropdownList_v2()
            MemberStatusSelect_v2.DataSource = From n In LookupDao.GetStatus() Select n
            MemberStatusSelect_v2.DataTextField = "Name"
            MemberStatusSelect_v2.DataValueField = "Value"
            MemberStatusSelect_v2.DataBind()
            InsertDropDownListEmptyValue(MemberStatusSelect_v2, "--- Select One ---")
        End Sub

        Private Sub InitFromSelectDropdownList_v2()
            FromSelect_v2.DataSource = From n In LookupDao.GetFromLocation() Select n
            FromSelect_v2.DataTextField = "Name"
            FromSelect_v2.DataValueField = "Value"
            FromSelect_v2.DataBind()
            InsertDropDownListZeroValue(FromSelect_v2, "--- Select One ---")
        End Sub

        Private Sub InitHospitalTypeSelectDropdownList_v2()
            HospitalTypeSelect_v2.DataSource = From n In LookupDao.GetMedicalFacility() Select n
            HospitalTypeSelect_v2.DataTextField = "Name"
            HospitalTypeSelect_v2.DataValueField = "Value"
            HospitalTypeSelect_v2.DataBind()
            InsertDropDownListEmptyValue(HospitalTypeSelect_v2, "--- Select One ---")
        End Sub

        Private Sub InitInfluenceSelectDropdownList_v2()
            InfluenceSelect_v2.DataSource = From n In LookupDao.GetMemberInfluence() Select n
            InfluenceSelect_v2.DataTextField = "Name"
            InfluenceSelect_v2.DataValueField = "Value"
            InfluenceSelect_v2.DataBind()
            InsertDropDownListZeroValue(InfluenceSelect_v2, "--- Select One ---")
        End Sub

        Private Sub SetMaxLengthForControls_v2()
            SetMaxLength(DiagnosisTextBox_v2)
            SetMaxLength(HospitalNameTextBox_v2)
            SetMaxLength(EventDetailsBox_v2)
            SetMaxLength(RelevantConditionTextBox_v2)
            SetMaxLength(txtApprovalComments_v2)
        End Sub

        Private Sub SetInputFormatRestrictions_v2()
            SetInputFormatRestrictionNoReturn(Page, DiagnosisTextBox_v2, FormatRestriction.AlphaNumeric, PERMITTED_SPECIAL_CHAR_INPUT)
            SetInputFormatRestrictionNoReturn(Page, HospitalNameTextBox_v2, FormatRestriction.AlphaNumeric, PERMITTED_SPECIAL_CHAR_INPUT)
            SetInputFormatRestrictionNoReturn(Page, EventDetailsBox_v2, FormatRestriction.AlphaNumeric, PERMITTED_SPECIAL_CHAR_INPUT)
            SetInputFormatRestrictionNoReturn(Page, RelevantConditionTextBox_v2, FormatRestriction.AlphaNumeric, PERMITTED_SPECIAL_CHAR_INPUT)
            SetInputFormatRestriction(Page, txtApprovalComments_v2, FormatRestriction.AlphaNumeric, PERMITTED_SPECIAL_CHAR_INPUT)
        End Sub

        Private Sub LoadData_v2()
            Dim medicalInfo As LineOfDutyMedical_v2 = LineOfDutyMedicalDao.GetById(refId, False)
            Dim lod_v2 As LineOfDuty_v2 = LodService.GetById(refId)

            If (UserCanEdit AndAlso Not lod_v2.Formal) Then
                DisplayReadWrite_v2(medicalInfo)
            Else
                DisplayReadOnly_v2(medicalInfo)
            End If
        End Sub

        Protected Sub DisplayReadOnly_v2(ByVal medical As LineOfDutyMedical_v2)
            If (IsPostBack) Then
                Exit Sub
            End If

            Dim unit As LineOfDutyUnit_v2 = LineOfDutyUnitDao.GetById(refId, False)

            MedOfficer_v2.Visible = True

            HideEditableControls_v2()

            'populate the labels
            Dim component = (From n In LookupDao.GetComponents() Where n.Value = medical.MemberComponent Select n).FirstOrDefault
            If (Not component Is Nothing) Then
                ComponentSelectlbl_v2.Text = component.Name
            End If

            If (unit.DutyFrom.HasValue) Then
                ComponentFromlbl_v2.Text = unit.DutyFrom.Value.ToString(DATE_HOUR_FORMAT)
            End If
            If (unit.DutyTo.HasValue) Then
                ComponentTolbl_v2.Text = unit.DutyTo.Value.ToString(DATE_HOUR_FORMAT)
            End If


            Dim category = (From n In LookupDao.GetCategory() Where n.Value = medical.MemberCategory Select n).FirstOrDefault
            If (Not category Is Nothing) Then
                CategorySelectlbl_v2.Text = category.Name
            End If

            Dim status = (From n In LookupDao.GetStatus() Where n.Value = medical.MemberStatus Select n).FirstOrDefault
            If (Not status Is Nothing) Then
                MemberStatusSelectlbl_v2.Text = status.Name
            End If

            Dim MemberFrom = (From n In LookupDao.GetFromLocation() Where n.Value.Equals(medical.MemberFrom) Select n).FirstOrDefault
            If (Not MemberFrom Is Nothing) Then
                FromSelectlbl_v2.Text = MemberFrom.Name
            End If

            If (medical.ICD9Id.HasValue) Then
                If (ucICDCodeControl_v2.IsValidICDCode(medical.ICD9Id)) Then
                    ucICDCodeControl_v2.UpdateICDCodeDiagnosisLabel(medical.ICD9Id, False)
                End If

                If (Not String.IsNullOrEmpty(medical.ICD7thCharacter)) Then
                    ucICD7thCharacterControl_v2.Update7thCharacterLabel(medical.ICD9Id, medical.ICD7thCharacter)
                End If
            End If

            IncidentTypelbl_v2.Text = medical.NatureOfEvent
            Diagnosislbl_v2.Text = medical.DiagnosisText

            Dim facility = (From n In LookupDao.GetMedicalFacility() Where n.Value = medical.MedicalFacilityType Select n).FirstOrDefault
            If (Not facility Is Nothing) Then
                HospitalTypeSelectlbl_v2.Text = facility.Name
            End If

            If (Not String.IsNullOrEmpty(medical.MedicalFacility)) Then
                HospitalNamelbl_v2.Text = medical.MedicalFacility.Replace(vbCrLf, "<br />")
            End If

            If (medical.TreatmentDate.HasValue) Then
                TreatmentDateLabel_v2.Text = medical.TreatmentDate.Value.ToString(DATE_HOUR_FORMAT)
            End If

            If (Not String.IsNullOrEmpty(medical.EventDetails)) Then
                EventDetailsLabel_v2.Text = medical.EventDetails.Replace(vbCrLf, "<br />") + "<br /><br /><br />"

            End If

            BoardFinalizationlbl_v2.Text = medical.BoardFinalization
            DeathCaseLabel_v2.Text = medical.DeathInvolved
            MvaLabel_v2.Text = medical.MvaInvolved

            Dim MemberCondition As String = ""
            If (Not String.IsNullOrEmpty(medical.MemberCondition)) Then
                If (medical.MemberCondition.Equals("was")) Then
                    MemberCondition = "was under the influence of alcohol or drugs"
                ElseIf (medical.MemberCondition.Equals("was not")) Then
                    MemberCondition = "was not under the influence of alcohol or drugs"
                End If
            End If
            MemberConditionlbl_v2.Text = MemberCondition


            Dim influence = (From n In LookupDao.GetMemberInfluence() Where n.Value = medical.Influence Select n).FirstOrDefault
            If (Not influence Is Nothing) Then
                Influencelbl_v2.Text = influence.Name
            End If


            AlcoholTestlbl_v2.Text = medical.AlcoholTestDone
            DrugTestlbl_v2.Text = medical.DrugTestDone
            MentallyResponsiblelbl_v2.Text = medical.MemberResponsible
            If (Not String.IsNullOrEmpty(medical.PsychiatricEval)) Then
                PsychiatricEvallbl_v2.Text = medical.PsychiatricEval
                If (medical.PsychiatricEval.Equals("Yes")) Then
                    If (medical.PsychiatricDate.HasValue()) Then
                        PsychiatricDatelbl_v2.Text = medical.PsychiatricDate
                    End If
                Else
                    PsychiatricDate_v2.Visible = False
                End If
            End If

            If (Not String.IsNullOrEmpty(medical.RelevantCondition)) Then
                RelevantConditionlbl_v2.Text = medical.RelevantCondition
            Else
                RelevantConditionlbl_v2.Text = "None"
            End If
            If (Not String.IsNullOrEmpty(medical.OtherTest)) Then
                OtherTestlbl_v2.Text = medical.OtherTest
                If (medical.OtherTest.Equals("Yes")) Then
                    If (medical.OtherTestDate.HasValue()) Then
                        OtherTestDatelbl_v2.Text = medical.OtherTestDate
                    End If
                Else
                    OtherTestDate_v2.Visible = False
                End If
            End If
            DeployedLocationlbl_v2.Text = medical.DeployedLocation


            If medical.Epts.HasValue Then
                Select Case medical.Epts.Value
                    Case 0
                        lblEPTS_v2.Text = "No"
                    Case 1
                        lblEPTS_v2.Text = "EPTS Yes/Service Aggravated"
                    Case 2
                        lblEPTS_v2.Text = "EPTS Yes/Not Service Aggravated"
                    Case Else

                End Select
            End If

            MobilityStandardslbl_v2.Text = medical.MobilityStandards
            lblApprovalComments_v2.Text = medical.ApprovalComments
        End Sub

        Private Sub HideEditableControls_v2()
            ComponentSelect_v2.Visible = False
            ComponentFromDate_v2.Visible = False
            ComponentFromTime_v2.Visible = False
            ComponentToDate_v2.Visible = False
            ComponentToTime_v2.Visible = False
            CategorySelect_v2.Visible = False
            MemberStatusSelect_v2.Visible = False
            FromSelect_v2.Visible = False
            ucICDCodeControl_v2.DisplayReadOnly(True)
            ucICD7thCharacterControl_v2.DisplayReadOnly()
            IncidentTypeSelect_v2.Visible = False
            DiagnosisTextBox_v2.Visible = False
            HospitalTypeSelect_v2.Visible = False
            HospitalNameTextBox_v2.Visible = False
            TreatmentDateBox_v2.Visible = False
            TreatmentHourBox_v2.Visible = False
            EventDetailsBox_v2.Visible = False
            BoardFinalizationrbl_v2.Visible = False
            DeathCaseChoice_v2.Visible = False
            MvaInvolvedChoice_v2.Visible = False
            MemberConditionSelect_v2.Visible = False
            InfluenceSelect_v2.Visible = False
            AlcoholTest_v2.Visible = False
            DrugTest_v2.Visible = False
            MentallyResponsiblerbl_v2.Visible = False
            PsychiatricEval_v2.Visible = False
            PsychiatricDatePicker_v2.Visible = False
            RelevantConditionTextBox_v2.Visible = False
            OtherTest_v2.Visible = False
            OtherTestDatePicker_v2.Visible = False
            DeployedLocation_v2.Visible = False
            rblEPTS_v2.Visible = False
            MobilityStandards_v2.Visible = False
            txtApprovalComments_v2.Visible = False
            ApprovalCommentsRow_v2.Visible = True
        End Sub

        Protected Sub DisplayReadWrite_v2(ByVal medical As LineOfDutyMedical_v2)
            Dim unit As LineOfDutyUnit_v2 = LineOfDutyUnitDao.GetById(refId, False)

            If (medical.MemberComponent IsNot Nothing) Then
                ComponentSelect_v2.SelectedValue = medical.MemberComponent
            End If

            If (unit.DutyFrom.HasValue) Then
                ComponentFromDate_v2.Text = Server.HtmlDecode(unit.DutyFrom.Value.ToString(DATE_FORMAT))
                ComponentFromTime_v2.Text = Server.HtmlDecode(unit.DutyFrom.Value.ToString(HOUR_FORMAT))
            End If
            If (unit.DutyTo.HasValue) Then
                ComponentToDate_v2.Text = Server.HtmlDecode(unit.DutyTo.Value.ToString(DATE_FORMAT))
                ComponentToTime_v2.Text = Server.HtmlDecode(unit.DutyTo.Value.ToString(HOUR_FORMAT))
            End If

            If (medical.MemberCategory IsNot Nothing) Then
                CategorySelect_v2.SelectedValue = medical.MemberCategory
            End If

            If (medical.MemberStatus IsNot Nothing) Then
                MemberStatusSelect_v2.SelectedValue = medical.MemberStatus
            End If

            If (medical.MemberFrom IsNot Nothing) Then
                FromSelect_v2.SelectedValue = medical.MemberFrom
            End If


            ucICDCodeControl_v2.DisplayReadWrite(False)
            ucICD7thCharacterControl_v2.DisplayReadWrite()

            If (medical.ICD9Id.HasValue) Then
                ucICDCodeControl_v2.InitializeHierarchy(medical.ICD9Id)

                If (ucICDCodeControl_v2.IsValidICDCode(medical.ICD9Id)) Then
                    ucICDCodeControl_v2.UpdateICDCodeDiagnosisLabel(medical.ICD9Id, True)
                End If

                If (Not String.IsNullOrEmpty(medical.ICD7thCharacter)) Then
                    ucICD7thCharacterControl_v2.InitializeCharacters(medical.ICD9Id, medical.ICD7thCharacter)
                Else
                    ucICD7thCharacterControl_v2.InitializeCharacters(medical.ICD9Id, String.Empty)
                End If

                DataBindNatureOfIncidentDDL_v2(medical.ICD9Id)
            Else
                ucICD7thCharacterControl_v2.InitializeCharacters(0, String.Empty)
                DataBindNatureOfIncidentDDL_v2(0)
            End If

            If (medical.NatureOfEvent IsNot Nothing) Then
                IncidentTypeSelect_v2.SelectedValue = medical.NatureOfEvent
            End If

            If (medical.DiagnosisText IsNot Nothing) Then
                DiagnosisTextBox_v2.Text = Server.HtmlDecode(medical.DiagnosisText)
            End If

            If (medical.MedicalFacilityType IsNot Nothing) Then
                HospitalTypeSelect_v2.SelectedValue = medical.MedicalFacilityType
            End If

            If (medical.MedicalFacility IsNot Nothing) Then
                HospitalNameTextBox_v2.Text = Server.HtmlDecode(medical.MedicalFacility)
            End If

            If (medical.TreatmentDate.HasValue) Then
                TreatmentDateBox_v2.Text = Server.HtmlDecode(medical.TreatmentDate.Value.ToString(DATE_FORMAT))
                TreatmentHourBox_v2.Text = Server.HtmlDecode(medical.TreatmentDate.Value.ToString(HOUR_FORMAT))
            End If

            If (medical.EventDetails IsNot Nothing) Then
                EventDetailsBox_v2.Text = Server.HtmlDecode(medical.EventDetails)
            End If

            SetRadioList(BoardFinalizationrbl_v2, medical.BoardFinalization)

            If (medical.IsSelectedICDCodeADisease(LookupDao)) Then
                BoardFinalizationrbl_v2.Enabled = False
            End If

            SetRadioList(DeathCaseChoice_v2, medical.DeathInvolved)
            SetRadioList(MvaInvolvedChoice_v2, medical.MvaInvolved)
            SetRadioList(MemberConditionSelect_v2, medical.MemberCondition)

            If (medical.MemberCondition IsNot Nothing) Then
                If (String.IsNullOrEmpty(medical.MemberCondition) Or medical.MemberCondition.Equals("was")) Then
                    If (medical.Influence.HasValue) Then
                        InfluenceSelect_v2.SelectedValue = medical.Influence.Value
                    End If

                    If (InfluenceSelect_v2.SelectedItem.Text.Equals("Drugs")) Then
                        AlcoholTest_v2.Enabled = False
                        SetRadioList(DrugTest_v2, medical.DrugTestDone)

                    ElseIf (InfluenceSelect_v2.SelectedItem.Text.Equals("Alcohol")) Then
                        DrugTest_v2.Enabled = False
                        SetRadioList(AlcoholTest_v2, medical.AlcoholTestDone)
                    Else
                        SetRadioList(DrugTest_v2, medical.DrugTestDone)
                        SetRadioList(AlcoholTest_v2, medical.AlcoholTestDone)
                    End If

                Else
                    InfluenceSelect_v2.Enabled = False
                    AlcoholTest_v2.SelectedValue = "No"
                    AlcoholTest_v2.Enabled = False
                    DrugTest_v2.SelectedValue = "No"
                    DrugTest_v2.Enabled = False

                End If
            End If
            SetRadioList(MentallyResponsiblerbl_v2, medical.MemberResponsible)
            SetRadioList(PsychiatricEval_v2, medical.PsychiatricEval)
            If (PsychiatricEval_v2.SelectedValue.Equals("Yes")) Then
                If (medical.PsychiatricDate IsNot Nothing) Then
                    PsychiatricDatePicker_v2.Text = medical.PsychiatricDate
                End If
            Else
                PsychiatricDate_v2.Visible = False
            End If

            If (medical.RelevantCondition IsNot Nothing) Then
                RelevantConditionTextBox_v2.Text = Server.HtmlDecode(medical.RelevantCondition)
            End If

            SetRadioList(OtherTest_v2, medical.OtherTest)
            If (OtherTest_v2.SelectedValue.Equals("Yes")) Then
                If (medical.OtherTestDate.HasValue()) Then
                    OtherTestDatePicker_v2.Text = medical.OtherTestDate
                End If
            Else
                OtherTestDate_v2.Visible = False
            End If

            If (Not String.IsNullOrEmpty(medical.DeployedLocation)) Then
                SetRadioList(DeployedLocation_v2, medical.DeployedLocation)
            Else
                SetRadioList(DeployedLocation_v2, "No")
            End If

            If (DeployedLocation_v2.SelectedValue.Equals("No")) Then
                If medical.Epts.HasValue Then
                    rblEPTS_v2.SelectedValue = medical.Epts.Value
                End If
                SetRadioList(MobilityStandards_v2, medical.MobilityStandards)
            Else
                rblEPTS_v2.Enabled = False
                MobilityStandards_v2.Enabled = False
                BoardFinalizationrbl_v2.Enabled = False
                LocationWarning_v2.Visible = True
            End If

            If (medical.ApprovalComments IsNot Nothing) Then
                txtApprovalComments_v2.Text = Server.HtmlDecode(medical.ApprovalComments)
            End If

            If CInt(Session("groupId")) = UserGroups.MedicalOfficer Or (CInt(Session("groupId")) = UserGroups.MedOfficer(P)) Then 'needs to be 119 for medp

                ApprovalCommentsRow_v2.Visible = True
                MedOfficer_v2.Visible = True
            End If

            If (CInt(Session("groupId")) = UserGroups.MedicalTechnician OrElse
                CInt(Session("groupId")) = UserGroups.WingSarc) Then
                ApprovalCommentsRow_v2.Visible = True
                txtApprovalComments_v2.Visible = False
                lblApprovalComments_v2.Text = medical.ApprovalComments
            End If

            ValidBoxLength_v2()
        End Sub

        Private Sub SaveData_v2()
            Dim lod_v2 As LineOfDuty_v2 = LodService.GetById(refId)

            If (Not UserCanEdit) Then
                Exit Sub
            ElseIf (UserCanEdit AndAlso lod_v2.Formal) Then
                Exit Sub
            End If

            Dim medicalInfo As LineOfDutyMedical_v2 = LineOfDutyMedicalDao.GetById(refId, False)
            Dim unitInfo As LineOfDutyUnit_v2 = LineOfDutyUnitDao.GetById(refId, False)

            medicalInfo.MemberComponent = ComponentSelect_v2.SelectedValue

            Try
                If (CheckDate(ComponentFromDate_v2)) Then
                    If (ComponentFromTime_v2.Text.Trim.Length > 0) Then
                        unitInfo.DutyFrom = Server.HtmlEncode(ParseDateAndTime(ComponentFromDate_v2.Text.Trim + " " + ComponentFromTime_v2.Text.Trim))
                    Else
                        unitInfo.DutyFrom = Server.HtmlEncode(DateTime.Parse(ComponentFromDate_v2.Text.Trim))
                    End If
                Else
                    unitInfo.DutyFrom = Nothing
                End If
            Catch ex As Exception
                unitInfo.DutyFrom = Nothing
            End Try

            Try
                If (CheckDate(ComponentToDate_v2)) Then
                    If (ComponentToTime_v2.Text.Trim.Length > 0) Then
                        unitInfo.DutyTo = Server.HtmlEncode(ParseDateAndTime(ComponentToDate_v2.Text.Trim + " " + ComponentToTime_v2.Text.Trim))
                    Else
                        unitInfo.DutyTo = Server.HtmlEncode(DateTime.Parse(ComponentToDate_v2.Text.Trim))
                    End If
                Else
                    unitInfo.DutyTo = Nothing
                End If
            Catch ex As Exception
                unitInfo.DutyTo = Nothing
            End Try

            medicalInfo.MemberCategory = CategorySelect_v2.SelectedValue
            medicalInfo.MemberStatus = MemberStatusSelect_v2.SelectedValue
            medicalInfo.MemberFrom = FromSelect_v2.SelectedValue

            If (ucICDCodeControl_v2.IsICDCodeSelected()) Then
                medicalInfo.ICD9Id = ucICDCodeControl_v2.SelectedICDCodeID
            End If

            If (ucICD7thCharacterControl_v2.Is7thCharacterSelected()) Then
                medicalInfo.ICD7thCharacter = ucICD7thCharacterControl_v2.Selected7thCharacter
            Else
                medicalInfo.ICD7thCharacter = Nothing
            End If

            medicalInfo.NatureOfEvent = IncidentTypeSelect_v2.SelectedValue
            medicalInfo.DiagnosisText = Server.HtmlEncode(DiagnosisTextBox_v2.Text.Trim())
            medicalInfo.MedicalFacilityType = HospitalTypeSelect_v2.SelectedValue
            medicalInfo.MedicalFacility = Server.HtmlEncode(HospitalNameTextBox_v2.Text)

            Try
                If (CheckDate(TreatmentDateBox_v2)) Then
                    If (TreatmentHourBox_v2.Text.Trim.Length > 0) Then
                        medicalInfo.TreatmentDate = Server.HtmlEncode(ParseDateAndTime(TreatmentDateBox_v2.Text.Trim + " " + TreatmentHourBox_v2.Text.Trim))
                    Else
                        medicalInfo.TreatmentDate = Server.HtmlEncode(DateTime.Parse(TreatmentDateBox_v2.Text.Trim))
                    End If
                Else
                    medicalInfo.TreatmentDate = Nothing
                End If
            Catch ex As Exception
                medicalInfo.TreatmentDate = Nothing
            End Try

            medicalInfo.EventDetails = Server.HtmlEncode(EventDetailsBox_v2.Text.Trim)
            medicalInfo.BoardFinalization = BoardFinalizationrbl_v2.SelectedValue
            medicalInfo.DeathInvolved = DeathCaseChoice_v2.SelectedValue
            medicalInfo.MvaInvolved = MvaInvolvedChoice_v2.SelectedValue
            medicalInfo.MemberCondition = MemberConditionSelect_v2.SelectedValue
            If (medicalInfo.MemberCondition.Equals("was")) Then
                medicalInfo.Influence = InfluenceSelect_v2.SelectedValue
                medicalInfo.AlcoholTestDone = AlcoholTest_v2.SelectedValue
                medicalInfo.DrugTestDone = DrugTest_v2.SelectedValue
            Else
                medicalInfo.Influence = Nothing
                medicalInfo.AlcoholTestDone = Nothing
                medicalInfo.DrugTestDone = Nothing
            End If
            medicalInfo.MemberResponsible = MentallyResponsiblerbl_v2.SelectedValue
            medicalInfo.PsychiatricEval = PsychiatricEval_v2.SelectedValue

            If (PsychiatricEval_v2.SelectedValue.Equals("Yes")) Then
                Try
                    If (CheckDate(PsychiatricDatePicker_v2)) Then
                        medicalInfo.PsychiatricDate = Server.HtmlEncode(DateTime.Parse(PsychiatricDatePicker_v2.Text.Trim))
                    Else
                        medicalInfo.PsychiatricDate = Nothing
                    End If
                Catch ex As Exception
                    medicalInfo.PsychiatricDate = Nothing
                End Try
            End If

            medicalInfo.RelevantCondition = RelevantConditionTextBox_v2.Text
            medicalInfo.OtherTest = OtherTest_v2.SelectedValue

            If (OtherTest_v2.SelectedValue.Equals("Yes")) Then
                Try
                    If (CheckDate(OtherTestDatePicker_v2)) Then
                        medicalInfo.OtherTestDate = Server.HtmlEncode(DateTime.Parse(OtherTestDatePicker_v2.Text.Trim))
                    Else
                        medicalInfo.OtherTestDate = Nothing
                    End If
                Catch ex As Exception
                    medicalInfo.OtherTestDate = Nothing
                End Try
            End If

            medicalInfo.DeployedLocation = DeployedLocation_v2.SelectedValue

            If (Not IsNothing(medicalInfo.ICD9Id)) Then
                ucICDCodeControl_v2.UpdateICDCodeDiagnosisLabel(medicalInfo.ICD9Id, True)
            End If

            If (medicalInfo.DeployedLocation.Equals("No")) Then
                If rblEPTS_v2.SelectedValue <> "" Then
                    medicalInfo.Epts = rblEPTS_v2.SelectedValue
                    If (medicalInfo.Epts = 0) Then
                        medicalInfo.ConditionEPTS = False
                        medicalInfo.ServiceAggravated = False
                    Else
                        medicalInfo.ConditionEPTS = True
                        If (medicalInfo.Epts = 1) Then
                            medicalInfo.ServiceAggravated = True
                        Else
                            medicalInfo.ServiceAggravated = False
                        End If
                    End If
                End If

                medicalInfo.MobilityStandards = MobilityStandards_v2.SelectedValue
            Else
                medicalInfo.Epts = Nothing
                medicalInfo.ConditionEPTS = Nothing
                medicalInfo.ServiceAggravated = Nothing
                medicalInfo.MobilityStandards = Nothing
            End If

            medicalInfo.ApprovalComments = Server.HtmlEncode(txtApprovalComments_v2.Text.Trim())

            medicalInfo.ModifiedBy = CInt(HttpContext.Current.Session("UserId"))
            medicalInfo.ModifiedDate = Now

            LineOfDutyMedicalDao.SaveOrUpdate(medicalInfo)
            LineOfDutyUnitDao.SaveOrUpdate(unitInfo)
        End Sub

        Protected Sub ICDCodeSelectionChangedEventHandler_v2(ByVal sender As Object, ByVal e As ICDCodeSelectedEventArgs)
            DataBindNatureOfIncidentDDL_v2(e.SelectedICDCodeId)
            UpdateRequireBoardFinalizationControlReadWriteAccess(e.SelectedICDCodeId)
        End Sub

        Private Sub DataBindNatureOfIncidentDDL_v2(ByVal codeId As Integer)
            IncidentTypeSelect_v2.Items.Clear()
            IncidentTypeSelect_v2.DataValueField = "Value"
            IncidentTypeSelect_v2.DataTextField = "Text"
            IncidentTypeSelect_v2.DataSource = _provider.GetICDIncident(codeId)
            IncidentTypeSelect_v2.DataBind()

            IncidentTypeSelect_v2.Items.Insert(0, New ListItem("-- Select Incident Code --", ""))

            Dim medicalInfo As LineOfDutyMedical = LineOfDutyMedicalDao.GetById(refId, False)

            If (IncidentTypeSelect_v2.Items.Count > 0 AndAlso IncidentTypeSelect_v2.Items.FindByValue(medicalInfo.NatureOfEvent) IsNot Nothing) Then
                IncidentTypeSelect_v2.SelectedValue = medicalInfo.NatureOfEvent
            Else
                IncidentTypeSelect_v2.SelectedIndex = 0
            End If

            If (IncidentTypeSelect_v2.Items.Count > 1) Then
                IncidentTypeSelect_v2.Enabled = True
            Else
                IncidentTypeSelect_v2.Enabled = False
            End If
        End Sub

        Private Sub UpdateRequireBoardFinalizationControlReadWriteAccess(icdId As Integer)
            Dim selectedIcdCode As ICD9Code = LookupDao.GetIcd9ById(icdId)

            If (selectedIcdCode IsNot Nothing AndAlso selectedIcdCode.IsDisease) Then
                SetRadioList(BoardFinalizationrbl_v2, "Yes")
                BoardFinalizationrbl_v2.Enabled = False
            Else
                SetRadioList(BoardFinalizationrbl_v2, "No")
                BoardFinalizationrbl_v2.Enabled = True
            End If
        End Sub

        Protected Sub DeployedLocation_v2_SelectedIndexChanged(sender As Object, e As EventArgs) Handles DeployedLocation_v2.SelectedIndexChanged
            If (DeployedLocation_v2.SelectedValue.Equals("Yes")) Then
                rblEPTS_v2.Enabled = False
                MobilityStandards_v2.Enabled = False
                BoardFinalizationrbl_v2.Enabled = False
                LocationWarning_v2.Visible = True

                rblEPTS_v2.SelectedValue = Nothing
                MobilityStandards_v2.SelectedValue = Nothing
            Else
                rblEPTS_v2.Enabled = True
                MobilityStandards_v2.Enabled = True
                BoardFinalizationrbl_v2.Enabled = True
                LocationWarning_v2.Visible = False
            End If
        End Sub

        Protected Sub PsychiatricEval_v2_SelectedIndexChanged(sender As Object, e As EventArgs) Handles PsychiatricEval_v2.SelectedIndexChanged
            If (PsychiatricEval_v2.SelectedValue.Equals("Yes")) Then
                PsychiatricDate_v2.Visible = True
            Else
                PsychiatricDate_v2.Visible = False
            End If
        End Sub

        Protected Sub OtherTest_v2_SelectedIndexChanged(sender As Object, e As EventArgs) Handles OtherTest_v2.SelectedIndexChanged
            If (OtherTest_v2.SelectedValue.Equals("Yes")) Then
                OtherTestDate_v2.Visible = True
            Else
                OtherTestDate_v2.Visible = False
            End If
        End Sub

        Protected Sub MemberConditionSelect_v2_SelectedIndexChanged(sender As Object, e As EventArgs) Handles MemberConditionSelect_v2.SelectedIndexChanged
            If (MemberConditionSelect_v2.SelectedValue.Equals("was not")) Then
                AlcoholTest_v2.SelectedValue = "No"
                AlcoholTest_v2.Enabled = False
                DrugTest_v2.SelectedValue = "No"
                DrugTest_v2.Enabled = False
                InfluenceSelect_v2.Enabled = False
                InfluenceSelect_v2.ClearSelection()
            Else
                AlcoholTest_v2.ClearSelection()
                AlcoholTest_v2.Enabled = True
                DrugTest_v2.ClearSelection()
                DrugTest_v2.Enabled = True
                InfluenceSelect_v2.Enabled = True
            End If
        End Sub
#End Region


#Region "TabEvent"
        Public Function ValidBoxLength_v2() As Boolean
            Dim IsValid As Boolean = True

            If Not CheckTextLength(HospitalNameTextBox_v2) Then
                IsValid = False
            End If

            If Not CheckTextLength(DiagnosisTextBox_v2) Then
                IsValid = False
            End If
            If Not CheckTextLength(EventDetailsBox_v2) Then
                IsValid = False
            End If

            If Not CheckTextLength(TreatmentDateBox_v2) Then
                IsValid = False
            End If
            If Not CheckTextLength(TreatmentHourBox_v2) Then
                IsValid = False
            End If
            If Not CheckTextLength(RelevantConditionTextBox_v2) Then
                IsValid = False
            End If

            Return IsValid
        End Function

        Public Function ValidBoxLength() As Boolean
            Dim IsValid As Boolean = True

            If Not CheckTextLength(HospitalNameBox) Then
                IsValid = False
            End If

            If Not CheckTextLength(DiagnosisTextBox) Then
                IsValid = False
            End If
            If Not CheckTextLength(EventDetailsBox) Then
                IsValid = False
            End If

            If Not CheckTextLength(TreatmentDateBox) Then
                IsValid = False
            End If
            If Not CheckTextLength(TreatmentHourBox) Then
                IsValid = False
            End If

            Return IsValid
        End Function

        Protected Sub Save_Click(ByRef sender As Object, ByRef e As TabNavigationEventArgs)
            If (Navigator.CurrentStep.IsReadOnly) Then
                Exit Sub
            End If

            If Not (ValidBoxLength()) Then
                e.Cancel = True
                Exit Sub
            End If

            Dim lod As LineOfDuty = LodService.GetById(refId)

            If (e.ButtonType = NavigatorButtonType.Save OrElse
                e.ButtonType = NavigatorButtonType.NavigatedAway OrElse
                e.ButtonType = NavigatorButtonType.NextStep OrElse
                e.ButtonType = NavigatorButtonType.PreviousStep) Then

                If (lod.Workflow = 1) Then
                    SaveData()
                Else
                    SaveData_v2()
                End If
            End If
        End Sub

        Private Sub TabButtonClicked(ByRef sender As Object, ByRef e As TabNavigationEventArgs)
            If (Navigator.CurrentStep.IsReadOnly) Then
                Exit Sub
            End If

            If Not (ValidBoxLength()) Then
                e.Cancel = True
                Exit Sub
            End If

            Dim lod As LineOfDuty = LodService.GetById(refId)

            If (e.ButtonType = NavigatorButtonType.Save OrElse
                e.ButtonType = NavigatorButtonType.NavigatedAway OrElse
                e.ButtonType = NavigatorButtonType.NextStep OrElse
                e.ButtonType = NavigatorButtonType.PreviousStep) Then
                If (lod.Workflow = 1) Then
                    SaveData()
                Else
                    SaveData_v2()
                End If
            End If
        End Sub
#End Region

        Protected Sub InfluenceSelect_v2_SelectedIndexChanged(sender As Object, e As EventArgs) Handles InfluenceSelect_v2.SelectedIndexChanged
            If (InfluenceSelect_v2.SelectedItem.Text.Equals("Drugs")) Then
                AlcoholTest_v2.ClearSelection()
                AlcoholTest_v2.Enabled = False
                DrugTest_v2.Enabled = True
            ElseIf (InfluenceSelect_v2.SelectedItem.Text.Equals("Alcohol")) Then
                DrugTest_v2.ClearSelection()
                DrugTest_v2.Enabled = False
                AlcoholTest_v2.Enabled = True
            Else
                AlcoholTest_v2.Enabled = True
                DrugTest_v2.Enabled = True
            End If
        End Sub
    End Class
End Namespace
