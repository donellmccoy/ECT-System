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

Namespace Web.LOD

    Partial Class Secure_lod_c2_Unit
        Inherits System.Web.UI.Page
        Protected Const optionalFindings As String = InvestigationDecision.APPROVE + "," + InvestigationDecision.DISAPPROVE + "," + InvestigationDecision.NOT_LOD_MISCONDUCT
        Protected _lod As LineOfDuty

        Public Property IsReadOnly() As Boolean
            Get
                Return ViewState("LodUnit_IsReadonly")
            End Get
            Set(ByVal value As Boolean)
                ViewState("LodUnit_IsReadonly") = value
            End Set

        End Property

        Public ReadOnly Property Navigator() As TabNavigator
            Get
                Return MasterPage.Navigator
            End Get
        End Property

        Public ReadOnly Property refId() As Integer
            Get
                Return Integer.Parse(Request.QueryString("refId"))
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

        Protected ReadOnly Property lod() As LineOfDuty
            Get
                If (_lod Is Nothing) Then
                    _lod = LodService.GetById(CType(Request.QueryString("refId"), Integer))
                End If
                Return _lod
            End Get
        End Property

        Protected ReadOnly Property MasterPage() As LodMaster
            Get
                Dim master As LodMaster = CType(Page.Master, LodMaster)
                Return master
            End Get
        End Property

        Protected Sub InitSignatureCheck()
            If (UserCanEdit) Then
                SigCheck.Visible = False
            Else
                SigCheck.Visible = True
                SigCheck.VerifySignature(refId)
            End If
        End Sub

        Protected Sub Page_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
            AddHandler Me.Master.TabClick, AddressOf TabButtonClicked
        End Sub

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
            Dim ws_Id As Integer = SESSION_WS_ID(refId)
            If (Not Page.IsPostBack) Then
                Dim lod As LineOfDuty = LodService.GetById(refId)

                UserCanEdit = GetAccessLOD(Navigator.PageAccess, True, lod)

                If (SESSION_GROUP_ID <> 2 And SESSION_GROUP_ID <> 120) Then
                    MStatus.Visible = False
                    MProof.Visible = False
                End If

                If (lod.Workflow = 1) Then
                    InitControls()
                Else
                    InitControls_v2()
                End If

                InitSignatureCheck()
            End If
            '    If (ws_Id > 324) Then
            '        DocumentationQuestionLogic()
            '    End If
        End Sub

        'Private Sub DocumentationQuestionLogic()
        '    DocumentationQuestions.Visible = True
        '    '    'If (UserCanEdit) Then
        '    '    '        rblLODInitiation.Enabled = True
        '    '    '        If (rblLODInitiation.SelectedIndex >= 0) Then
        '    '    '            rblWrittenDiagnosis.Enabled = True
        '    '    '        End If
        '    '    '        If (rblWrittenDiagnosis.SelectedIndex >= 0) Then
        '    '    '            rblMemberRequest.Enabled = True
        '    '    '        End If
        '    '    '        If (rblMemberRequest.SelectedIndex >= 0) Then
        '    '    '            rblIncurredOrAggravated.Enabled = True
        '    '    '        End If

        '    '    '        If (rblIncurredOrAggravated.SelectedIndex >= 0) Then
        '    rblStatusWhenOccured.Enabled = True
        '    '    '        End If
        '    'If (rblStatusWhenOccured.SelectedIndex >= 0) Then
        '    '    If (Not rblStatusWhenOccured.SelectedValue.Equals("Other")) Then
        '    '        txtStatusWhenOccured.Text = ""
        '    '        rblIfNoOrders.Enabled = True
        '    '    ElseIf (rblStatusWhenOccured.SelectedValue.Equals("Other") AndAlso Not txtStatusWhenOccuredTextBox.Text.Equals("")) Then
        '    '        rblIfNoOrders.Enabled = True
        '    '    Else
        '    '        rblIfNoOrders.Enabled = False
        '    '    End If
        '    'End If

        '    '    '        If (rblStatusWhenOccured.SelectedValue.Equals("Other")) Then
        '    '    '            txtStatusWhenOccuredTextBox.Enabled = True
        '    '    '        Else
        '    '    '            txtStatusWhenOccuredTextBox.Enabled = False
        '    '    '            txtStatusWhenOccuredTextBox.Text = ""
        '    '    '        End If

        '    '    '        If (rblIfNoOrders.SelectedIndex >= 0) Then
        '    '    '            rblIDTDocumentAttached.Enabled = True
        '    '    '        End If
        '    '    '        If (rblIDTDocumentAttached.SelectedIndex >= 0) Then
        '    '    '            rblUTAPSAttached.Enabled = True
        '    '    '        End If
        '    '    '        If (rblUTAPSAttached.SelectedIndex >= 0) Then
        '    '    '            rblPCARSAttached.Enabled = True
        '    '    '        End If
        '    If (rblPCARSAttached.SelectedValue.Equals("True")) Then
        '        rblPCARSHistory.Enabled = True
        '        lblPCARSAttached.Text = ""
        '    ElseIf (rblPCARSAttached.SelectedValue.Equals("False")) Then
        '        rblPCARSHistory.Enabled = False
        '        lblPCARSAttached.CssClass = "labelRequired"
        '        lblPCARSAttached.Text = "PCARS Report must be attached to the MilPDS in order for case to continue"
        '    End If
        '    If (rblPCARSHistory.SelectedValue.Equals("True")) Then
        '        rblEightYearRule.Enabled = True
        '        lblPCARSHistory.Text = ""
        '    ElseIf (rblPCARSHistory.SelectedValue.Equals("False")) Then
        '        rblEightYearRule.Enabled = False
        '        lblPCARSHistory.CssClass = "labelRequired"
        '        lblPCARSHistory.Text = "PCARS must reflect the date of injury or illness please see note on Q6"
        '    End If
        '    If (rblEightYearRule.SelectedIndex >= 0) Then
        '        rblPriorToDutytatus.Enabled = True
        '    End If
        '    If (rblPriorToDutytatus.SelectedIndex >= 0) Then
        '        If (rblPriorToDutytatus.SelectedValue.Equals("False")) Then
        '            rblStatusWorsened.Enabled = False
        '            rblStatusWorsened.SelectedValue = "N/A"
        '        Else
        '            rblStatusWorsened.Enabled = True
        '        End If
        '        If (rblPriorToDutytatus.SelectedValue.Equals("True") AndAlso rblStatusWorsened.SelectedValue.Equals("N/A")) Then
        '            rblStatusWorsened.ClearSelection()
        '        End If
        '    End If
        '    'End If
        'End Sub

#Region "LOD"

        Public Sub LoadFindings(ByVal unit As LineOfDutyUnit)

            Dim cFinding As LineOfDutyFindings
            'Unit Findings

            cFinding = lod.FindByType(PersonnelTypes.UNIT_CMDR)
            unit.UnitFinding = cFinding

            rblInLOD.DataSource = New LookupDao().GetWorkflowFindings(lod.Workflow, UserGroups.UnitCommander)
            rblInLOD.DataValueField = "Id"
            rblInLOD.DataTextField = "Description"
            rblInLOD.DataBind()

            If Not (cFinding Is Nothing) Then
                If Not cFinding.Finding Is Nothing Then
                    rblInLOD.SelectedValue = cFinding.Finding
                End If
            End If
            If Not (UserCanEdit) Then
                rblInLOD.Visible = False
                If Not (cFinding Is Nothing) Then

                    If (cFinding.Finding.HasValue) Then
                        InLodLabel.Text = cFinding.Description
                    End If
                End If
            End If

        End Sub

        Public Sub SaveFindings()

            Dim cFinding As LineOfDutyFindings
            'Board
            cFinding = CreateFinding(lod.Id)
            cFinding.PType = PersonnelTypes.UNIT_CMDR
            If rblInLOD.SelectedValue <> "" Then
                cFinding.Finding = rblInLOD.SelectedValue
            End If
            lod.SetFindingByType(cFinding)

        End Sub

        Protected Sub ddlDutyStatus_DataBound(ByVal sender As Object, ByVal e As System.EventArgs) Handles DutyStatusSelect.DataBound
            DutyStatusSelect.Items.Insert(0, New ListItem("-- Select --", String.Empty))
        End Sub

        Private Sub DisplayReadOnly(ByVal unit As LineOfDutyUnit)

            'hide the edit controls
            rblcmdractivated.Visible = False
            DutyStatusSelect.Visible = False
            AbsenceFromBox.Visible = False
            AbsenceHourFromBox.Visible = False
            AbsenceToBox.Visible = False
            AbsenceHourToBox.Visible = False
            OtherDutyBox.Visible = False
            DetailsBox.Visible = False

            lblcmdractivated.Text = unit.Activated
            DutyStatusLabel.Text = unit.DutyStatusDescription
            OtherDutyLabel.Text = "N/A"

            LoadFindings(unit)

            If (unit.DutyDetermination = "active") Then
                If (unit.DutyFrom.HasValue) Then
                    AbsenceFromLabel.Text = unit.DutyFrom.Value.ToString(DATE_HOUR_FORMAT)
                End If

                If (unit.DutyTo.HasValue) Then
                    AbsenceToLabel.Text = unit.DutyTo.Value.ToString(DATE_HOUR_FORMAT)
                End If
            Else
                AbsenceFromLabel.Text = "N/A"
                AbsenceToLabel.Text = "N/A"

                If unit.DutyDetermination = "other" Then
                    OtherDutyLabel.Text = unit.OtherDutyStatus
                End If
            End If

            If (Not String.IsNullOrEmpty(unit.AccidentDetails)) Then
                DetailsLabel.Text = unit.AccidentDetails.Replace(vbCrLf, "<br />") + "<br /><br /><br />"
            End If

        End Sub

        Private Sub DisplayReadWrite(ByVal unit As LineOfDutyUnit)

            DutyStatusSelect.DataBind()
            'rblInLOD.DataBind()

            LoadFindings(unit)

            SetRadioList(rblcmdractivated, unit.Activated)
            DetailsBox.Text = Server.HtmlDecode(unit.AccidentDetails)

            If (unit.DutyFrom.HasValue) Then
                AbsenceFromBox.Text = Server.HtmlDecode(unit.DutyFrom.Value.ToString(DATE_FORMAT))
                AbsenceHourFromBox.Text = Server.HtmlDecode(unit.DutyFrom.Value.ToString(HOUR_FORMAT))
            End If

            If (unit.DutyTo.HasValue) Then
                AbsenceToBox.Text = Server.HtmlDecode(unit.DutyTo.Value.ToString(DATE_FORMAT))
                AbsenceHourToBox.Text = Server.HtmlDecode(unit.DutyTo.Value.ToString(HOUR_FORMAT))
            End If

            OtherDutyBox.Text = Server.HtmlDecode(unit.OtherDutyStatus)
            SetDropdownByValue(DutyStatusSelect, unit.DutyDetermination)

        End Sub

        Private Sub InitControls()

            DutyStatusSelect.Attributes.Add("OnChange", "CheckDutyStatus();")
            SetMaxLength(DetailsBox)
            SetInputFormatRestriction(Page, AbsenceFromBox, FormatRestriction.Numeric, "/")
            SetInputFormatRestriction(Page, AbsenceToBox, FormatRestriction.Numeric, "/")
            SetInputFormatRestriction(Page, AbsenceHourFromBox, FormatRestriction.Numeric)
            SetInputFormatRestriction(Page, AbsenceHourToBox, FormatRestriction.Numeric)
            SetInputFormatRestriction(Page, OtherDutyBox, FormatRestriction.AlphaNumeric, PERMITTED_SPECIAL_CHAR_INPUT)
            SetInputFormatRestriction(Page, DetailsBox, FormatRestriction.AlphaNumeric, PERMITTED_SPECIAL_CHAR_INPUT)

            If (UserCanEdit) Then
                AbsenceFromBox.CssClass = "datePicker"
                AbsenceToBox.CssClass = "datePickerFuture"
            End If

            OriginalLOD.Visible = True

            LoadData()
            TabControl.Item(NavigatorButtonType.Save).Visible = UserCanEdit
            LogManager.LogAction(ModuleType.LOD, UserAction.ViewPage, refId, "Viewed Page: Unit")

        End Sub

        Private Sub LoadData()

            Dim dao As ILineOfDutyUnitDao = New NHibernateDaoFactory().GetLineOfDutyUnitDao()
            Dim unit As LineOfDutyUnit = dao.GetById(refId, False)

            If (UserCanEdit) Then
                DisplayReadWrite(unit)
            Else
                DisplayReadOnly(unit)
            End If

        End Sub

        Private Sub SaveData()

            If (Not UserCanEdit) Then
                Exit Sub
            End If

            Dim dao As ILineOfDutyUnitDao = New NHibernateDaoFactory().GetLineOfDutyUnitDao()
            Dim unitInfo = dao.GetById(refId)

            Select Case DutyStatusSelect.SelectedValue.ToString()
                Case DutyStatus.Active_Duty_Status
                    Try
                        If (Trim(AbsenceHourFromBox.Text) <> "") Then
                            unitInfo.DutyFrom = Server.HtmlEncode(ParseDateAndTime(Trim(AbsenceFromBox.Text) + " " + Trim(AbsenceHourFromBox.Text)))
                        Else
                            unitInfo.DutyFrom = Server.HtmlEncode(DateTime.Parse(Trim(AbsenceFromBox.Text)))
                        End If
                    Catch ex As Exception
                        unitInfo.DutyFrom = Nothing
                    End Try

                    Try
                        If (Trim(AbsenceHourToBox.Text) <> "") Then
                            unitInfo.DutyTo = Server.HtmlEncode(ParseDateAndTime(Trim(AbsenceToBox.Text) + " " + Trim(AbsenceHourToBox.Text)))
                        Else
                            unitInfo.DutyTo = Server.HtmlEncode(DateTime.Parse(Trim(AbsenceToBox.Text)))
                        End If
                    Catch ex As Exception
                        unitInfo.DutyTo = Nothing
                    End Try

                Case DutyStatus.Other
                    unitInfo.OtherDutyStatus = Server.HtmlEncode(OtherDutyBox.Text)

            End Select

            unitInfo.Activated = rblcmdractivated.SelectedValue
            unitInfo.AccidentDetails = Server.HtmlEncode(DetailsBox.Text)
            unitInfo.DutyDetermination = DutyStatusSelect.SelectedValue

            unitInfo.ModifiedBy = CInt(HttpContext.Current.Session("UserId"))
            unitInfo.ModifiedDate = Now
            SaveFindings()
            dao.SaveOrUpdate(unitInfo)

        End Sub

#End Region

#Region "LOD_v2"

        Public Sub LoadFindings_v2(ByVal unit As LineOfDutyUnit)

            Dim cFinding As LineOfDutyFindings
            Dim userGroupId As Integer = CInt(Session("groupId"))
            'Unit Findings
            Dim wsId As Integer = SESSION_WS_ID(refId)

            If (unit.MemberProof And
                (wsId = LodWorkStatus_v3.UnitCommanderReview_LODV3 Or wsId = LodWorkStatus_v2.UnitCommanderReview) And
                (userGroupId = UserGroups.UnitCommander Or userGroupId = UserGroups.UnitCC_LODV3)) Then
                SetCheckBox(MProofCheckBox, unit.MemberProof)
                MStatus.Visible = True
            ElseIf (unit.MemberProof) Then
                MStatus.Visible = False
                MVStatusText.Visible = True
                'MVStatusText.ForeColor = Drawing.Color.Green
            End If

            If (unit.MemberStatus And
                (wsId = LodWorkStatus_v3.UnitCommanderReview_LODV3 Or wsId = wsId = LodWorkStatus_v2.UnitCommanderReview) And
                (userGroupId = UserGroups.UnitCommander Or userGroupId = UserGroups.UnitCC_LODV3)) Then
                SetCheckBox(MStatusCheckBox, unit.MemberStatus)
                MProof.Visible = True
            ElseIf (unit.MemberStatus) Then
                MProof.Visible = False
                MVProofText.Visible = True
                'MVProofText.ForeColor = Drawing.Color.Green
            End If

            cFinding = lod.FindByType(PersonnelTypes.UNIT_CMDR)
            unit.UnitFinding = cFinding

            rblInLOD_v2.DataSource = From p In New LookupDao().GetWorkflowFindings(lod.Workflow, UserGroups.UnitCommander) Where Not optionalFindings.Split(",").Contains(p.FindingType) Select p
            rblInLOD_v2.DataValueField = "Id"
            rblInLOD_v2.DataTextField = "Description"
            rblInLOD_v2.DataBind()

            If Not (cFinding Is Nothing) Then
                If Not cFinding.Finding Is Nothing Then
                    rblInLOD_v2.SelectedValue = cFinding.Finding
                End If
            End If
            If Not (UserCanEdit) Then
                rblInLOD_v2.Visible = False
                If Not (cFinding Is Nothing) Then

                    If (cFinding.Finding.HasValue) Then
                        InLodLabel_v2.Text = cFinding.Description
                    End If
                End If
            End If

        End Sub

        Public Sub SaveFindings_v2()

            Dim cFinding As LineOfDutyFindings

            cFinding = CreateFinding(lod.Id)
            cFinding.PType = PersonnelTypes.UNIT_CMDR

            If rblInLOD_v2.SelectedValue <> "" Then
                cFinding.Finding = rblInLOD_v2.SelectedValue
            End If
            lod.SetFindingByType(cFinding)

        End Sub

        Protected Sub DutyStatusSelect_v2_SelectedIndexChanged(sender As Object, e As EventArgs) Handles DutyStatusSelect_v2.SelectedIndexChanged

            Dim lkupDAO As ILookupDao
            lkupDAO = New NHibernateDaoFactory().GetLookupDao()

            If (DutyStatusSelect_v2.SelectedValue.Equals("travel")) Then
                TravelOccurrence_v2.Visible = True
            Else
                TravelOccurrence_v2.Visible = False
                TravelOccurrenceSelect_v2.SelectedIndex = 0
            End If

            If (DutyStatusSelect_v2.SelectedValue.Equals("other")) Then
                OtherDutyBox_v2.Enabled = True
            End If
        End Sub

        Protected Sub InfoSourceSelect_v2_SelectedIndexChanged(sender As Object, e As EventArgs) Handles InfoSourceSelect_v2.SelectedIndexChanged
            If (InfoSourceSelect_v2.SelectedValue = InfoSource.Other) Then
                InfoSourceOtherTextBox_v2.Visible = True
            Else
                InfoSourceOtherTextBox_v2.Visible = False
                InfoSourceOtherTextBox_v2.Text = ""
            End If
        End Sub

        Protected Sub MemberOccurrenceSelect_v2_SelectedIndexChanged(sender As Object, e As EventArgs) Handles MemberOccurrenceSelect_v2.SelectedIndexChanged
            If (MemberOccurrenceSelect_v2.SelectedValue = Occurrence.AbsentWithoutAuthority) Then
                MemberOccurrence_v2.Visible = True
            Else
                MemberOccurrence_v2.Visible = False
            End If
        End Sub

        Protected Sub proximateCause_v2_SelectedIndexChanged(sender As Object, e As EventArgs) Handles proximateCause_v2.SelectedIndexChanged
            If (proximateCause_v2.SelectedValue = ProximateCause.FreeText) Then
                otherCauseTextBox_v2.Visible = True
            Else
                If (proximateCause_v2.SelectedValue = ProximateCause.Misconduct) Then
                    rblInLOD_v2.SelectedValue = LodFinding.RecommendFormalInvestigation
                    rblInLOD_v2.Enabled = False
                Else
                    rblInLOD_v2.Enabled = True
                End If

                otherCauseTextBox_v2.Text = ""
                otherCauseTextBox_v2.Visible = False
            End If
        End Sub

        Private Sub DisplayReadOnly_v2(ByVal unit As LineOfDutyUnit_v2)

            Dim lkupDAO As ILookupDao
            Dim ws_Id As Integer = SESSION_WS_ID(refId)
            Dim readWrite As Boolean = False
            lkupDAO = New NHibernateDaoFactory().GetLookupDao()

            'hide the edit controls
            rblcmdractivated_v2.Visible = False
            DutyStatusSelect_v2.Visible = False
            DutyStatusFromDate_v2.Visible = False
            DutyStatusFromTime_v2.Visible = False
            DutyStatusToDate_v2.Visible = False
            DutyStatusToTime_v2.Visible = False
            OtherDutyBox_v2.Visible = False
            InfoSourceSelect_v2.Visible = False
            InfoSourceOtherTextBox_v2.Visible = False
            WitnessName1.Visible = False
            WitnessAddress1.Visible = False
            WitnessPhoneNumber1.Visible = False
            WitnessName2.Visible = False
            WitnessAddress2.Visible = False
            WitnessPhoneNumber2.Visible = False
            WitnessName3.Visible = False
            WitnessAddress3.Visible = False
            WitnessPhoneNumber3.Visible = False
            WitnessName4.Visible = False
            WitnessAddress4.Visible = False
            WitnessPhoneNumber4.Visible = False
            WitnessName5.Visible = False
            WitnessAddress5.Visible = False
            WitnessPhoneNumber5.Visible = False
            MemberOccurrenceSelect_v2.Visible = False
            TravelOccurrenceSelect_v2.Visible = False
            AbsenceFromDate_v2.Visible = False
            AbsenceFromTime_v2.Visible = False
            AbsenceToDate_v2.Visible = False
            AbsenceToTime_v2.Visible = False
            DetailsBox_v2.Visible = False
            rblCredibleService_v2.Visible = False
            rblMemberOnOrders_v2.Visible = False
            proximateCause_v2.Visible = False
            otherCauseTextBox_v2.Visible = False
            rblInLOD_v2.Visible = False
            'rblLODInitiation.Visible = False
            'rblWrittenDiagnosis.Visible = False
            'rblMemberRequest.Visible = False
            'rblPriorToDutytatus.Visible = False
            'rblStatusWorsened.Visible = False
            'rblIncurredOrAggravated.Visible = False
            'rblStatusWhenOccured.Visible = False
            'rblIfNoOrders.Visible = False
            'rblIDTDocumentAttached.Visible = False
            'rblUTAPSAttached.Visible = False
            'rblPCARSAttached.Visible = False
            'rblPCARSHistory.Visible = False
            'rblEightYearRule.Visible = False

            lblcmdractivated_v2.Text = unit.Activated
            DutyStatusLabel_v2.Text = unit.DutyStatusDescription
            OtherDutyLabel_v2.Text = "N/A"

            If (unit.DutyFrom.HasValue) Then
                DutyStatusFromLabel_v2.Text = unit.DutyFrom.Value.ToString(DATE_HOUR_FORMAT)
            End If

            If (unit.DutyTo.HasValue) Then
                DutyStatusToLabel_v2.Text = unit.DutyTo.Value.ToString(DATE_HOUR_FORMAT)
            End If

            If unit.DutyDetermination = "other" Then
                OtherDutyLabel_v2.Text = unit.OtherDutyStatus
            End If

            If (unit.SourceInformation.HasValue) Then
                Dim infosource = (From n In lkupDAO.GetInfoSources() Where n.Value = unit.SourceInformation Select n).FirstOrDefault
                If (Not infosource Is Nothing) Then
                    InfoSourcelbl_v2.Text = infosource.Name

                    If (infosource.Name.Equals("Other")) Then
                        InfoSourceOtherlbl_v2.Text = unit.SourceInformationSpecify
                    End If
                End If
            Else
                InfoSourcelbl_v2.Text = "No witnesses presented"
            End If

            If (unit.Witnesses IsNot Nothing AndAlso unit.Witnesses.Count > 0) Then
                If (unit.Witnesses.Count >= 1) Then
                    Dim wit As WitnessData = unit.Witnesses(0)
                    WitnessName1lbl.Text = Server.HtmlDecode(wit.Name)
                    WitnessAddress1lbl.Text = Server.HtmlDecode(wit.Address)
                    WitnessPhoneNumber1lbl.Text = Server.HtmlDecode(wit.PhoneNumber)
                Else
                    witness1.Visible = False
                End If

                If (unit.Witnesses.Count >= 2) Then
                    Dim wit As WitnessData = unit.Witnesses(1)
                    WitnessName2lbl.Text = Server.HtmlDecode(wit.Name)
                    WitnessAddress2lbl.Text = Server.HtmlDecode(wit.Address)
                    WitnessPhoneNumber2lbl.Text = Server.HtmlDecode(wit.PhoneNumber)
                Else
                    witness2.Visible = False
                End If

                If (unit.Witnesses.Count >= 3) Then
                    Dim wit As WitnessData = unit.Witnesses(2)
                    WitnessName3lbl.Text = Server.HtmlDecode(wit.Name)
                    WitnessAddress3lbl.Text = Server.HtmlDecode(wit.Address)
                    WitnessPhoneNumber3lbl.Text = Server.HtmlDecode(wit.PhoneNumber)
                Else
                    witness3.Visible = False
                End If
                If (unit.Witnesses.Count >= 4) Then
                    Dim wit As WitnessData = unit.Witnesses(3)
                    WitnessName4lbl.Text = Server.HtmlDecode(wit.Name)
                    WitnessAddress4lbl.Text = Server.HtmlDecode(wit.Address)
                    WitnessPhoneNumber4lbl.Text = Server.HtmlDecode(wit.PhoneNumber)
                Else
                    witness4.Visible = False
                End If
                If (unit.Witnesses.Count >= 5) Then
                    Dim wit As WitnessData = unit.Witnesses(4)
                    WitnessName5lbl.Text = Server.HtmlDecode(wit.Name)
                    WitnessAddress5lbl.Text = Server.HtmlDecode(wit.Address)
                    WitnessPhoneNumber5lbl.Text = Server.HtmlDecode(wit.PhoneNumber)
                Else
                    witness5.Visible = False
                End If
            Else
                witness1.Visible = False
                witness2.Visible = False
                witness3.Visible = False
                witness4.Visible = False
                witness5.Visible = False
                trWitnessesHeader.Visible = False
                lblNoWitnesses.Visible = True
            End If

            Dim MemberOccurrence = (From n In lkupDAO.GetOccurrences() Where n.Value = unit.MemberOccurrence Select n).FirstOrDefault
            If (Not MemberOccurrence Is Nothing) Then
                MemberOccurrencelbl_v2.Text = MemberOccurrence.Name
            End If

            If (unit.MemberOccurrence.HasValue AndAlso unit.MemberOccurrence = Occurrence.AbsentWithoutAuthority) Then
                If (unit.AbsentFrom.HasValue) Then
                    AbsenceFromlbl_v2.Text = unit.AbsentFrom.Value.ToString(DATE_HOUR_FORMAT)
                End If

                If (unit.AbsentTo.HasValue) Then
                    AbsenceTolbl_v2.Text = unit.AbsentTo.Value.ToString(DATE_HOUR_FORMAT)
                End If
            Else
                AbsenceFromlbl_v2.Text = "N/A"
                AbsenceTolbl_v2.Text = "N/A"

            End If

            If (Not String.IsNullOrEmpty(unit.DutyDetermination) AndAlso unit.DutyDetermination.Equals("travel")) Then
                Dim TravelOccurrence = (From n In lkupDAO.GetOccurrences() Where n.Value = unit.TravelOccurrence Select n).FirstOrDefault
                If (Not TravelOccurrence Is Nothing) Then
                    TravelOccurrencelbl_v2.Text = "In Addition: " + TravelOccurrence.Name
                End If
            Else
                TravelOccurrence_v2.Visible = False
            End If

            If (Not String.IsNullOrEmpty(unit.AccidentDetails)) Then
                DetailsLabel_v2.Text = unit.AccidentDetails.Replace(vbCrLf, "<br />") + "<br /><br /><br />"
            End If

            lblCredibleService_v2.Text = unit.MemberCredible
            lblMemberOnOrders_v2.Text = unit.MemberOnOrders

            Dim proximate = (From n In lkupDAO.GetCauses() Where n.Value = unit.ProximateCause Select n).FirstOrDefault
            If (Not proximate Is Nothing) Then
                proximateCauselbl_v2.Text = proximate.Name
            End If

            otherCauseTextlbl_v2.Text = unit.ProximateCauseSpecify
            'SetDocumentionRadioButtons(unit, ws_Id, readWrite)

            LoadFindings_v2(unit)

        End Sub

        Private Sub DisplayReadWrite_v2(ByVal unit As LineOfDutyUnit_v2)

            Dim lkupDAO As ILookupDao
            Dim ws_Id As Integer = SESSION_WS_ID(refId)
            Dim readWrite As Boolean = True
            lkupDAO = New NHibernateDaoFactory().GetLookupDao()

            SetRadioList(rblcmdractivated_v2, unit.Activated)

            DutyStatusSelect_v2.SelectedValue = unit.DutyDetermination

            If (unit.DutyFrom.HasValue) Then
                DutyStatusFromDate_v2.Text = Server.HtmlDecode(unit.DutyFrom.Value.ToString(DATE_FORMAT))
                DutyStatusFromTime_v2.Text = Server.HtmlDecode(unit.DutyFrom.Value.ToString(HOUR_FORMAT))
            End If

            If (unit.DutyTo.HasValue) Then
                DutyStatusToDate_v2.Text = Server.HtmlDecode(unit.DutyTo.Value.ToString(DATE_FORMAT))
                DutyStatusToTime_v2.Text = Server.HtmlDecode(unit.DutyTo.Value.ToString(HOUR_FORMAT))
            End If

            If (DutyStatusSelect_v2.SelectedValue.Equals("other")) Then
                OtherDutyBox_v2.Text = Server.HtmlDecode(unit.OtherDutyStatus)
            Else
                OtherDutyBox_v2.Enabled = False
            End If

            If (unit.SourceInformation.HasValue()) Then
                InfoSourceSelect_v2.SelectedValue = unit.SourceInformation
            End If

            If (unit.SourceInformation = InfoSource.Other) Then
                InfoSourceOtherTextBox_v2.Text = unit.SourceInformationSpecify
            Else
                InfoSourceOtherTextBox_v2.Visible = False
            End If

            If (unit.Witnesses IsNot Nothing) Then
                If (unit.Witnesses.Count >= 1) Then
                    Dim wit As WitnessData = unit.Witnesses(0)
                    WitnessName1.Text = Server.HtmlDecode(wit.Name)
                    WitnessAddress1.Text = Server.HtmlDecode(wit.Address)
                    WitnessPhoneNumber1.Text = Server.HtmlDecode(wit.PhoneNumber)
                End If
                If (unit.Witnesses.Count >= 2) Then
                    Dim wit As WitnessData = unit.Witnesses(1)
                    WitnessName2.Text = Server.HtmlDecode(wit.Name)
                    WitnessAddress2.Text = Server.HtmlDecode(wit.Address)
                    WitnessPhoneNumber2.Text = Server.HtmlDecode(wit.PhoneNumber)
                End If
                If (unit.Witnesses.Count >= 3) Then
                    Dim wit As WitnessData = unit.Witnesses(2)
                    WitnessName3.Text = Server.HtmlDecode(wit.Name)
                    WitnessAddress3.Text = Server.HtmlDecode(wit.Address)
                    WitnessPhoneNumber3.Text = Server.HtmlDecode(wit.PhoneNumber)
                End If
                If (unit.Witnesses.Count >= 4) Then
                    Dim wit As WitnessData = unit.Witnesses(3)
                    WitnessName4.Text = Server.HtmlDecode(wit.Name)
                    WitnessAddress4.Text = Server.HtmlDecode(wit.Address)
                    WitnessPhoneNumber4.Text = Server.HtmlDecode(wit.PhoneNumber)
                End If
                If (unit.Witnesses.Count >= 5) Then
                    Dim wit As WitnessData = unit.Witnesses(4)
                    WitnessName5.Text = Server.HtmlDecode(wit.Name)
                    WitnessAddress5.Text = Server.HtmlDecode(wit.Address)
                    WitnessPhoneNumber5.Text = Server.HtmlDecode(wit.PhoneNumber)
                End If

            End If

            If (Not String.IsNullOrEmpty(unit.DutyDetermination) AndAlso unit.DutyDetermination.Equals("travel") AndAlso unit.TravelOccurrence.HasValue) Then
                TravelOccurrence_v2.Visible = True
                TravelOccurrenceSelect_v2.SelectedValue = unit.TravelOccurrence
            Else
                TravelOccurrence_v2.Visible = False
            End If

            If (unit.MemberOccurrence.HasValue()) Then
                MemberOccurrenceSelect_v2.SelectedValue = unit.MemberOccurrence
            End If

            If (unit.MemberOccurrence = Occurrence.AbsentWithoutAuthority) Then
                If (unit.AbsentFrom.HasValue) Then
                    AbsenceFromDate_v2.Text = Server.HtmlDecode(unit.AbsentFrom.Value.ToString(DATE_FORMAT))
                    AbsenceFromTime_v2.Text = Server.HtmlDecode(unit.AbsentFrom.Value.ToString(HOUR_FORMAT))
                End If

                If (unit.AbsentTo.HasValue) Then
                    AbsenceToDate_v2.Text = Server.HtmlDecode(unit.AbsentTo.Value.ToString(DATE_FORMAT))
                    AbsenceToTime_v2.Text = Server.HtmlDecode(unit.AbsentTo.Value.ToString(HOUR_FORMAT))
                End If
            Else
                MemberOccurrence_v2.Visible = False
            End If

            DetailsBox_v2.Text = Server.HtmlDecode(unit.AccidentDetails)
            SetRadioList(rblCredibleService_v2, unit.MemberCredible)
            SetRadioList(rblMemberOnOrders_v2, unit.MemberOnOrders)
            SetCheckBox(MProofCheckBox, unit.MemberProof)
            SetCheckBox(MStatusCheckBox, unit.MemberStatus)
            If (unit.ProximateCause.HasValue()) Then
                proximateCause_v2.SelectedValue = unit.ProximateCause
            End If

            If (unit.ProximateCause = ProximateCause.FreeText) Then
                otherCauseTextBox_v2.Text = unit.ProximateCauseSpecify
            Else
                otherCauseTextBox_v2.Visible = False
            End If

            'SetDocumentionRadioButtons(unit, ws_Id, readWrite)

            LoadFindings_v2(unit)

        End Sub

        Private Sub InitControls_v2()

            Dim lkupDAO As ILookupDao
            lkupDAO = New NHibernateDaoFactory().GetLookupDao()

            DutyStatusSelect_v2.DataSource = From n In lkupDAO.GetDutyStatuses() Select n
            DutyStatusSelect_v2.DataTextField = "Name"
            DutyStatusSelect_v2.DataValueField = "Value"
            DutyStatusSelect_v2.DataBind()
            InsertDropDownListEmptyValue(DutyStatusSelect_v2, "--- Select One ---")

            InfoSourceSelect_v2.DataSource = From n In lkupDAO.GetInfoSources() Select n
            InfoSourceSelect_v2.DataTextField = "Name"
            InfoSourceSelect_v2.DataValueField = "Value"
            InfoSourceSelect_v2.DataBind()
            InsertDropDownListZeroValue(InfoSourceSelect_v2, "--- Select One ---")

            MemberOccurrenceSelect_v2.DataSource = From n In lkupDAO.GetOccurrences() Select n Where Not n.Value = Occurrence.InactiveDutyTraining AndAlso Not n.Value = Occurrence.DutyOrTraining Select n
            MemberOccurrenceSelect_v2.DataTextField = "Name"
            MemberOccurrenceSelect_v2.DataValueField = "Value"
            MemberOccurrenceSelect_v2.DataBind()
            InsertDropDownListZeroValue(MemberOccurrenceSelect_v2, "--- Select One ---")

            TravelOccurrenceSelect_v2.DataSource = From n In lkupDAO.GetOccurrences() Where n.Value = Occurrence.InactiveDutyTraining Or n.Value = Occurrence.DutyOrTraining Select n
            TravelOccurrenceSelect_v2.DataTextField = "Name"
            TravelOccurrenceSelect_v2.DataValueField = "Value"
            TravelOccurrenceSelect_v2.DataBind()
            InsertDropDownListZeroValue(TravelOccurrenceSelect_v2, "--- Select ---")

            proximateCause_v2.DataSource = From n In lkupDAO.GetCauses() Select n
            proximateCause_v2.DataTextField = "Name"
            proximateCause_v2.DataValueField = "Value"
            proximateCause_v2.DataBind()
            InsertDropDownListZeroValue(proximateCause_v2, "--- Select One ---")

            DutyStatusFromDate_v2.CssClass = "datePicker"
            DutyStatusToDate_v2.CssClass = "datePickerFuture"
            AbsenceFromDate_v2.CssClass = "datePicker"
            AbsenceToDate_v2.CssClass = "datePicker"

            SetInputFormatRestriction(Page, WitnessName1, FormatRestriction.AlphaNumeric, PERMITTED_SPECIAL_CHAR_INPUT)
            SetInputFormatRestriction(Page, WitnessAddress1, FormatRestriction.AlphaNumeric, PERMITTED_SPECIAL_CHAR_INPUT)
            SetInputFormatRestriction(Page, WitnessPhoneNumber1, FormatRestriction.Numeric, PHONE_NUMBER_CHARACTERS)

            SetInputFormatRestriction(Page, WitnessName2, FormatRestriction.AlphaNumeric, PERMITTED_SPECIAL_CHAR_INPUT)
            SetInputFormatRestriction(Page, WitnessAddress2, FormatRestriction.AlphaNumeric, PERMITTED_SPECIAL_CHAR_INPUT)
            SetInputFormatRestriction(Page, WitnessPhoneNumber2, FormatRestriction.Numeric, PHONE_NUMBER_CHARACTERS)

            SetInputFormatRestriction(Page, WitnessName3, FormatRestriction.AlphaNumeric, PERMITTED_SPECIAL_CHAR_INPUT)
            SetInputFormatRestriction(Page, WitnessAddress3, FormatRestriction.AlphaNumeric, PERMITTED_SPECIAL_CHAR_INPUT)
            SetInputFormatRestriction(Page, WitnessPhoneNumber3, FormatRestriction.Numeric, PHONE_NUMBER_CHARACTERS)

            SetInputFormatRestriction(Page, WitnessName4, FormatRestriction.AlphaNumeric, PERMITTED_SPECIAL_CHAR_INPUT)
            SetInputFormatRestriction(Page, WitnessAddress4, FormatRestriction.AlphaNumeric, PERMITTED_SPECIAL_CHAR_INPUT)
            SetInputFormatRestriction(Page, WitnessPhoneNumber4, FormatRestriction.Numeric, PHONE_NUMBER_CHARACTERS)

            SetInputFormatRestriction(Page, WitnessName5, FormatRestriction.AlphaNumeric, PERMITTED_SPECIAL_CHAR_INPUT)
            SetInputFormatRestriction(Page, WitnessAddress5, FormatRestriction.AlphaNumeric, PERMITTED_SPECIAL_CHAR_INPUT)
            SetInputFormatRestriction(Page, WitnessPhoneNumber5, FormatRestriction.Numeric, PHONE_NUMBER_CHARACTERS)

            SetMaxLength(DetailsBox_v2)
            SetInputFormatRestrictionNoReturn(Page, DetailsBox_v2, FormatRestriction.AlphaNumeric, PERMITTED_SPECIAL_CHAR_INPUT)

            SetMaxLength(otherCauseTextBox_v2)
            SetInputFormatRestrictionNoReturn(Page, otherCauseTextBox_v2, FormatRestriction.AlphaNumeric, PERMITTED_SPECIAL_CHAR_INPUT)

            LOD_v2.Visible = True

            LoadData_v2()
            TabControl.Item(NavigatorButtonType.Save).Visible = UserCanEdit
            LogManager.LogAction(ModuleType.LOD, UserAction.ViewPage, refId, "Viewed Page: Unit")

        End Sub

        Private Sub LoadData_v2()

            Dim dao As ILineOfDutyUnitDao = New NHibernateDaoFactory().GetLineOfDutyUnitDao()
            Dim unit As LineOfDutyUnit_v2 = dao.GetById(refId, False)
            'Dim unit As LineOfDutyUnit = dao.GetById(refId, False)
            Dim lod_v2 As LineOfDuty_v2 = LodService.GetById(refId)

            If (UserCanEdit AndAlso Not lod_v2.Formal) Then
                DisplayReadWrite_v2(unit)
            Else
                DisplayReadOnly_v2(unit)
            End If

        End Sub

        'Private Sub SetDocumentionRadioButtons(ByVal unit As LineOfDutyUnit_v2, ByVal ws_Id As Integer, ByVal readWrite As Boolean)
        '    If (ws_Id > 324) Then
        '        If (readWrite) Then
        '            ' SetRadioList(rblStatusWhenOccured, unit.StatusWhenInjuryed)
        '            ' SetRadioList(rblIfNoOrders, unit.OrdersAttached)
        '            ' SetRadioList(rblIDTDocumentAttached, unit.IDTStatus)
        '            ' SetRadioList(rblUTAPSAttached, unit.UTAPSAttached)
        '            SetRadioList(rblPCARSAttached, unit.PCARSAttached)
        '            SetRadioList(rblPCARSHistory, unit.PCARSHistory)
        '            SetRadioList(rblEightYearRule, unit.EightYearRule)

        '            ' SetRadioList(rblIncurredOrAggravated, unit.IncurredOrAggravated)

        '            ' SetRadioList(rblLODInitiation, unit.LODInitiation)
        '            ' SetRadioList(rblWrittenDiagnosis, unit.WrittenDiagnosis)
        '            ' SetRadioList(rblMemberRequest, unit.MemberRequest)
        '            SetRadioList(rblPriorToDutytatus, unit.PriorToDutytatus)
        '            SetRadioList(rblStatusWorsened, unit.StatusWorsened)
        '        Else
        '            ' SetRadioLables(unit.LODInitiation, lblLODInitiation)
        '            ' SetRadioLables(unit.WrittenDiagnosis, lblWrittenDiagnosis)
        '            ' SetRadioLables(unit.MemberRequest, lblMemberRequest)
        '            SetRadioLables(unit.PriorToDutytatus, lblPriorToDutytatus)
        '            SetRadioLables(unit.StatusWorsened, lblStatusWorsened, True)

        '            ' SetRadioLables(unit.IncurredOrAggravated, lblIncurredOrAggravated)

        '            ' lblStatusWhenOccured.Text = unit.StatusWhenInjuryed
        '            ' lblIfNoOrders.Text = unit.OrdersAttached
        '            ' lblIDTDocumentAttached.Text = unit.IDTStatus
        '            ' lblUTAPSAttached.Text = unit.UTAPSAttached

        '            SetRadioLables(unit.PCARSAttached, lblPCARSAttached)
        '            SetRadioLables(unit.PCARSHistory, lblPCARSHistory)
        '            SetRadioLables(unit.EightYearRule, lblEightYearRule)
        '        End If

        '        ' If (unit.StatusWhenInjuryedExplanation IsNot Nothing) Then
        '        '     If (unit.StatusWhenInjuryedExplanation.Length > 0) Then
        '        '         txtStatusWhenOccuredTextBox.Text = unit.StatusWhenInjuryedExplanation.ToString()
        '        '     End If
        '        ' End If
        '    End If
        'End Sub

        Private Sub SaveData_v2()
            Dim lod_v2 As LineOfDuty_v2 = LodService.GetById(refId)
            'Dim lodUnit As LineOfDutyUnit
            If (Not UserCanEdit) Then
                Exit Sub
            ElseIf (UserCanEdit AndAlso lod_v2.Formal) Then
                Exit Sub
            End If

            Dim dao As ILineOfDutyUnitDao = New NHibernateDaoFactory().GetLineOfDutyUnitDao()
            Dim unitInfo As LineOfDutyUnit_v2 = dao.GetById(refId)

            unitInfo.Activated = rblcmdractivated_v2.SelectedValue
            unitInfo.DutyDetermination = DutyStatusSelect_v2.SelectedValue

            Try
                If (Trim(DutyStatusFromTime_v2.Text) <> "") Then
                    unitInfo.DutyFrom = Server.HtmlEncode(ParseDateAndTime(Trim(DutyStatusFromDate_v2.Text) + " " + Trim(DutyStatusFromTime_v2.Text)))
                Else
                    unitInfo.DutyFrom = Server.HtmlEncode(DateTime.Parse(Trim(DutyStatusFromDate_v2.Text)))
                End If
            Catch ex As Exception
                unitInfo.DutyFrom = Nothing
            End Try

            Try
                If (Trim(DutyStatusToTime_v2.Text) <> "") Then
                    unitInfo.DutyTo = Server.HtmlEncode(ParseDateAndTime(Trim(DutyStatusToDate_v2.Text) + " " + Trim(DutyStatusToTime_v2.Text)))
                Else
                    unitInfo.DutyTo = Server.HtmlEncode(DateTime.Parse(Trim(DutyStatusToDate_v2.Text)))
                End If
            Catch ex As Exception
                unitInfo.DutyTo = Nothing
            End Try

            unitInfo.OtherDutyStatus = Server.HtmlEncode(OtherDutyBox_v2.Text)

            unitInfo.SourceInformation = InfoSourceSelect_v2.SelectedValue
            unitInfo.SourceInformationSpecify = InfoSourceOtherTextBox_v2.Text

            unitInfo.Witnesses = New List(Of ALOD.Core.Domain.Common.WitnessData)
            unitInfo.Witnesses.Clear()

            If (Not String.IsNullOrEmpty(WitnessName1.Text.Trim)) Then
                Dim wit As New WitnessData()
                wit.Name = Server.HtmlEncode(WitnessName1.Text.Trim)
                wit.Address = Server.HtmlEncode(WitnessAddress1.Text.Trim)
                wit.PhoneNumber = Server.HtmlEncode(WitnessPhoneNumber1.Text.Trim)
                unitInfo.Witnesses.Add(wit)
            End If

            If (Not String.IsNullOrEmpty(WitnessName2.Text.Trim)) Then
                Dim wit As New WitnessData()
                wit.Name = Server.HtmlEncode(WitnessName2.Text.Trim)
                wit.Address = Server.HtmlEncode(WitnessAddress2.Text.Trim)
                wit.PhoneNumber = Server.HtmlEncode(WitnessPhoneNumber2.Text.Trim)
                unitInfo.Witnesses.Add(wit)
            End If

            If (Not String.IsNullOrEmpty(WitnessName3.Text.Trim)) Then
                Dim wit As New WitnessData()
                wit.Name = Server.HtmlEncode(WitnessName3.Text.Trim)
                wit.Address = Server.HtmlEncode(WitnessAddress3.Text.Trim)
                wit.PhoneNumber = Server.HtmlEncode(WitnessPhoneNumber3.Text.Trim)
                unitInfo.Witnesses.Add(wit)
            End If

            If (Not String.IsNullOrEmpty(WitnessName4.Text.Trim)) Then
                Dim wit As New WitnessData()
                wit.Name = Server.HtmlEncode(WitnessName4.Text.Trim)
                wit.Address = Server.HtmlEncode(WitnessAddress4.Text.Trim)
                wit.PhoneNumber = Server.HtmlEncode(WitnessPhoneNumber4.Text.Trim)
                unitInfo.Witnesses.Add(wit)
            End If

            If (Not String.IsNullOrEmpty(WitnessName5.Text.Trim)) Then
                Dim wit As New WitnessData()
                wit.Name = Server.HtmlEncode(WitnessName5.Text.Trim)
                wit.Address = Server.HtmlEncode(WitnessAddress5.Text.Trim)
                wit.PhoneNumber = Server.HtmlEncode(WitnessPhoneNumber5.Text.Trim)
                unitInfo.Witnesses.Add(wit)
            End If

            unitInfo.MemberOccurrence = MemberOccurrenceSelect_v2.SelectedValue

            If (MemberOccurrenceSelect_v2.SelectedValue = Occurrence.AbsentWithoutAuthority) Then
                Try
                    If (CheckDate(AbsenceFromDate_v2)) Then
                        If (AbsenceFromTime_v2.Text.Trim.Length > 0) Then
                            unitInfo.AbsentFrom = Server.HtmlEncode(ParseDateAndTime(AbsenceFromDate_v2.Text.Trim + " " + Trim(AbsenceFromTime_v2.Text)))
                        Else
                            unitInfo.AbsentFrom = Server.HtmlEncode(DateTime.Parse(AbsenceFromDate_v2.Text.Trim))
                        End If
                    Else
                        unitInfo.AbsentFrom = Nothing
                    End If
                Catch ex As Exception
                    unitInfo.AbsentFrom = Nothing
                End Try

                Try
                    If (CheckDate(AbsenceToDate_v2)) Then
                        If (AbsenceToTime_v2.Text.Trim.Length > 0) Then
                            unitInfo.AbsentTo = Server.HtmlEncode(ParseDateAndTime(AbsenceToDate_v2.Text.Trim + " " + Trim(AbsenceToTime_v2.Text)))
                        Else
                            unitInfo.AbsentTo = Server.HtmlEncode(DateTime.Parse(AbsenceToDate_v2.Text.Trim))
                        End If
                    Else
                        unitInfo.AbsentTo = Nothing
                    End If
                Catch ex As Exception
                    unitInfo.AbsentTo = Nothing
                End Try
            End If

            If (DutyStatusSelect_v2.SelectedValue.Equals("travel")) Then
                unitInfo.TravelOccurrence = TravelOccurrenceSelect_v2.SelectedValue()
            End If

            unitInfo.AccidentDetails = Server.HtmlEncode(DetailsBox_v2.Text)
            unitInfo.MemberCredible = rblCredibleService_v2.SelectedValue
            unitInfo.MemberProof = MProofCheckBox.Checked
            unitInfo.MemberStatus = MStatusCheckBox.Checked
            unitInfo.MemberOnOrders = rblMemberOnOrders_v2.SelectedValue
            unitInfo.ProximateCause = proximateCause_v2.SelectedValue
            unitInfo.ProximateCauseSpecify = otherCauseTextBox_v2.Text
            'If (Not rblLODInitiation.SelectedValue.Equals("")) Then
            '    unitInfo.LODInitiation = Convert.ToBoolean(rblLODInitiation.SelectedValue)
            'End If
            'If (Not rblWrittenDiagnosis.SelectedValue.Equals("")) Then
            '    unitInfo.WrittenDiagnosis = Convert.ToBoolean(rblWrittenDiagnosis.SelectedValue)
            'End If
            'If (Not rblMemberRequest.SelectedValue.Equals("")) Then
            '    unitInfo.MemberRequest = Convert.ToBoolean(rblMemberRequest.SelectedValue)
            'End If
            'If (Not rblPriorToDutytatus.SelectedValue.Equals("")) Then
            '    unitInfo.PriorToDutytatus = Convert.ToBoolean(rblPriorToDutytatus.SelectedValue)
            'End If
            'If (Not rblStatusWorsened.SelectedValue.Equals("")) Then
            '    unitInfo.StatusWorsened = rblStatusWorsened.SelectedValue.ToString()
            'End If
            'If (Not rblIncurredOrAggravated.SelectedValue.Equals("")) Then
            '    unitInfo.IncurredOrAggravated = Convert.ToBoolean(rblIncurredOrAggravated.SelectedValue)
            'End If

            'If (Not rblStatusWhenOccured.SelectedValue.Equals("")) Then
            '    unitInfo.StatusWhenInjuryed = rblStatusWhenOccured.SelectedValue.ToString()
            'End If
            'If (Not txtStatusWhenOccuredTextBox.Text.Equals("")) Then
            '    unitInfo.StatusWhenInjuryedExplanation = txtStatusWhenOccuredTextBox.Text.ToString()
            'End If
            'If (Not rblIfNoOrders.SelectedValue.Equals("")) Then
            '    unitInfo.OrdersAttached = rblIfNoOrders.SelectedValue.ToString()
            'End If
            'If (Not rblIDTDocumentAttached.SelectedValue.Equals("")) Then
            '    unitInfo.IDTStatus = rblIDTDocumentAttached.SelectedValue.ToString()
            'End If
            'If (Not rblUTAPSAttached.SelectedValue.Equals("")) Then
            '    unitInfo.UTAPSAttached = rblUTAPSAttached.SelectedValue.ToString()
            'End If
            'If (Not rblPCARSAttached.SelectedValue.Equals("")) Then
            '    unitInfo.PCARSAttached = Convert.ToBoolean(rblPCARSAttached.SelectedValue)
            'End If
            'If (Not rblPCARSHistory.SelectedValue.Equals("")) Then
            '    unitInfo.PCARSHistory = Convert.ToBoolean(rblPCARSHistory.SelectedValue)
            'End If
            'If (Not rblEightYearRule.SelectedValue.Equals("")) Then
            '    unitInfo.EightYearRule = Convert.ToBoolean(rblEightYearRule.SelectedValue)
            'End If

            unitInfo.ModifiedBy = CInt(HttpContext.Current.Session("UserId"))
            unitInfo.ModifiedDate = Now
            SaveFindings_v2()
            dao.SaveOrUpdate(unitInfo)

        End Sub

        Private Sub SetRadioLables(ByVal rbutton As Boolean, ByVal label As Label)
            Select Case rbutton
                Case 0
                    label.Text = "No"
                Case 1
                    label.Text = "Yes"

                Case Else
                    label.Text = ""
            End Select
        End Sub

        Private Sub SetRadioLables(ByVal rbutton As String, ByVal label As Label, ByVal x As Boolean)
            Try
                label.Text = rbutton
            Catch ex As Exception

            End Try
        End Sub

#End Region

#Region "TabEvent"

        Protected Sub Save_Click(ByRef sender As Object, ByRef e As TabNavigationEventArgs)
            If (Navigator.CurrentStep.IsReadOnly) Then
                Exit Sub
            End If

            Dim lod As LineOfDuty = LodService.GetById(refId)

            If (lod.Workflow = 1) Then
                SaveData()
            Else
                SaveData_v2()
            End If
        End Sub

        Private Sub TabButtonClicked(ByRef sender As Object, ByRef e As TabNavigationEventArgs)

            If (e.ButtonType = NavigatorButtonType.Delete) Then

            End If

            Dim lod As LineOfDuty = LodService.GetById(refId)

            If (e.ButtonType = NavigatorButtonType.Save OrElse e.ButtonType = NavigatorButtonType.NavigatedAway _
                OrElse e.ButtonType = NavigatorButtonType.NextStep OrElse e.ButtonType = NavigatorButtonType.PreviousStep) Then
                If (lod.Workflow = 1) Then
                    SaveData()
                Else
                    SaveData_v2()
                End If
            End If
        End Sub

#End Region

    End Class

End Namespace