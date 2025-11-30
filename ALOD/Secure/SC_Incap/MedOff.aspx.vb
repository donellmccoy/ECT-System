Imports ALOD.Core.Domain.Common
Imports ALOD.Core.Domain.Modules.SpecialCases
Imports ALOD.Core.Domain.Users
Imports ALOD.Core.Domain.Workflow
Imports ALOD.Core.Interfaces.DAOInterfaces
Imports ALOD.Core.Utils
Imports ALOD.Data
Imports ALOD.Logging
Imports ALOD.Web.UserControls
Imports ALODWebUtility.Common

Namespace Web.Special_Case.IN

    Partial Class Secure_sc_in_MedOff
        Inherits System.Web.UI.Page

#Region "Fields"

        Private _daoFactory As IDaoFactory
        Private _scAccess As Dictionary(Of String, ALOD.Core.Domain.Workflow.PageAccessType)
        Private _specCase As SC_Incap = Nothing
        Private _specCaseDao As ISpecialCaseDAO

#End Region

#Region "Properties"

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

        Protected ReadOnly Property DaoFactory() As IDaoFactory
            Get
                If (_daoFactory Is Nothing) Then
                    _daoFactory = New NHibernateDaoFactory()
                End If

                Return _daoFactory
            End Get
        End Property

        Protected ReadOnly Property INCAP_Ext() As SC_Incap
            Get
                If (_specCase Is Nothing) Then
                    _specCase = SpecCaseDao.GetById(RefId)
                End If

                Return _specCase
            End Get
        End Property

        Protected ReadOnly Property MasterPage() As SC_IncapMaster
            Get
                Dim master As SC_IncapMaster = CType(Page.Master, SC_IncapMaster)
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

        Protected ReadOnly Property SpecCase() As SC_Incap
            Get
                If (_specCase Is Nothing) Then
                    _specCase = SpecCaseDao.GetById(RefId)
                End If

                Return _specCase
            End Get
        End Property

        Protected ReadOnly Property SpecCaseDao() As ISpecialCaseDAO
            Get
                If (_specCaseDao Is Nothing) Then
                    _specCaseDao = DaoFactory.GetSpecialCaseDAO()
                End If

                Return _specCaseDao
            End Get
        End Property

        Protected ReadOnly Property WsId() As Integer
            Get
                Dim ws_id As Integer = SESSION_WS_ID(RefId)
                Return ws_id
            End Get
        End Property

        Private ReadOnly Property RefId() As Integer
            Get
                Return Integer.Parse(Request.QueryString("refId"))
            End Get
        End Property

#End Region

#Region "Page Methods"

        Public Function ValidBoxLength() As Boolean
            Dim IsValid As Boolean = True
            'If Not CheckTextLength(DecisionComment) Then
            '    IsValid = False
            'End If
            Return IsValid
        End Function

        Protected Function GetAvailabilityCode() As DataSet
            Return SpecCaseDao.GetAvailabilityCode()
        End Function

        Protected Function GetIRILOStatus() As DataSet
            Return SpecCaseDao.GetIRILOStatus()
        End Function

        Protected Function GetMedDisposition() As DataSet
            Return SpecCaseDao.GetMedDisposition()
        End Function

        Protected Sub Page_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
            AddHandler Me.Master.TabClick, AddressOf TabButtonClicked
        End Sub

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

            If (Not IsPostBack) Then
                UserCanEdit = GetAccess(Navigator.PageAccess, False)
                InitControls()
                LogManager.LogAction(ModuleType.SpecCaseIncap, UserAction.ViewPage, RefId, "Viewed Page: MedOff")
            End If
            If (Not UserCanEdit) Then
                page_readOnly.Value = "0"
            Else
                page_readOnly.Value = "1"
            End If
        End Sub

        'CodeCleanUp use for Med Input section
        'Protected Sub rblSeniorMedicalReviewerDecision_SelectedIndexChanged(sender As Object, e As EventArgs) Handles rblSeniorMedicalReviewerDecision.SelectedIndexChanged
        '    SetSeniorMedicalReviewerFindingsPanelVisibility()
        'End Sub
        Protected Sub Save_Click(ByRef sender As Object, ByRef e As TabNavigationEventArgs)
            If (Navigator.CurrentStep.IsReadOnly) Then
                Exit Sub
            End If

            If (e.ButtonType = NavigatorButtonType.Save OrElse
                e.ButtonType = NavigatorButtonType.NavigatedAway OrElse
                e.ButtonType = NavigatorButtonType.NextStep OrElse
                e.ButtonType = NavigatorButtonType.PreviousStep) Then
                SaveData()
            End If

            If Not (ValidBoxLength()) Then
                e.Cancel = True
                Exit Sub
            End If
        End Sub

        Private Function GetWorkflowType(ByVal wsid As Integer) As String
            Dim type As String = ""
            Select Case wsid
                Case AFRCWorkflows.INInitiate, AFRCWorkflows.INMedicalReview_WG, AFRCWorkflows.INImmediateCommanderReview,
                     AFRCWorkflows.INWingJAReview, AFRCWorkflows.INFinanceReview, AFRCWorkflows.INWingCCAction, AFRCWorkflows.INApproved,
                     AFRCWorkflows.INHolding_PM, AFRCWorkflows.INHolding_RMU
                    type = "Initiate"
                Case AFRCWorkflows.INExtension, AFRCWorkflows.INMedicalReview_WG_Ext, AFRCWorkflows.INImmediateCommanderReview_Ext,
                     AFRCWorkflows.INWingJAReview_Ext, AFRCWorkflows.INFinanceReview_Ext, AFRCWorkflows.INWingCommanderRecommendation_Ext,
                     AFRCWorkflows.INOCR_Ext_HR_Review, AFRCWorkflows.INOPR_Ext_HR_Review, AFRCWorkflows.INDirectorOfStaffReview,
                     AFRCWorkflows.INDirectorOfPersonnelReview, AFRCWorkflows.INCommandChiefReview, AFRCWorkflows.INViceCommanderReview,
                     AFRCWorkflows.INCAFRAction, AFRCWorkflows.INDisposition
                    type = "Ext"
                Case AFRCWorkflows.INWingCCRecommendation_Appeal, AFRCWorkflows.INAppeal, AFRCWorkflows.INOPR_Appeal_HR_Review,
                     AFRCWorkflows.INOCR_Appeal_HR_Review, AFRCWorkflows.INViceCommanderReview_Appeal, AFRCWorkflows.INDirectorOfPersonnelReview_Appeal,
                     AFRCWorkflows.INDirectorOfStaffReview_Appeal, AFRCWorkflows.INCommandChiefReview_Appeal, AFRCWorkflows.INViceCommanderReview_Appeal,
                     AFRCWorkflows.INCAFR_Action_Appeal, AFRCWorkflows.INDisapproved
                    type = "Appeal"

            End Select
            Return type
        End Function

        Private Sub InitControls()
            'CodeCleanUp step through and clean up all methods and commentted out work
            ' SetInputFormatRestrictions()
            SetAccessForControls()
            ViewEnableControls()
            LoadFindings()
            SetMaxLengthForControls()
        End Sub

        Private Sub InitDatePickerCSSClasses(ByVal item As TextBox, ByVal type As Int16)
            Select Case type
                Case 0
                    item.CssClass = "datePickerPlusFuture" 'today and beyond
                Case 1
                    item.CssClass = "datePicker" 'past + today
                Case 2
                    item.CssClass = "datePickerFuture" 'any date
            End Select
        End Sub

        Private Sub LoadAppealFindings() 'error
            Dim results As SC_IncapAppeal_Findings = SpecCase.INCAPAppealFindings.Last
            If (results.WCC_AppealApproval.HasValue) Then
                rblWCC_AppealPAY.SelectedValue = Convert.ToInt32(results.WCC_AppealApproval.Value)
            End If
        End Sub

        Private Sub LoadExtFindings()
            'Initial results
            Dim init_results As SC_Incap_Findings = SpecCase.INCAPFindings.First
            If (init_results.Init_StartDate.HasValue) Then
                InitialStartDate_Date.Text = init_results.Init_StartDate.Value.ToString(DATE_FORMAT)
                InitialStartDate_Time.Text = init_results.Init_StartDate.Value.ToString(HOUR_FORMAT)
            End If
            If (init_results.Init_EndDate.HasValue) Then
                InitialEndDate_Date.Text = init_results.Init_EndDate.Value.ToString(DATE_FORMAT)
                InitialEndDate_Time.Text = init_results.Init_EndDate.Value.ToString(HOUR_FORMAT)
            End If
            If (Not IsDBNull(init_results.Med_ReportType)) Then
                If (init_results.Med_ReportType.Trim.Length > 0) Then
                    ddlMedAvailabilityCode.SelectedValue = init_results.Med_ReportType
                    'If (results.Med_ReportType = "AAC 37") Then
                    '    rblMedRevreport.SelectedValue = "37"
                    'Else
                    '    rblMedRevreport.SelectedValue = "31"
                    'End If
                End If
            End If
            If (init_results.Med_AbilityToPreform.HasValue) Then
                rblMedRevDecision.SelectedValue = Convert.ToInt32(init_results.Med_AbilityToPreform.Value)
            End If
            If (init_results.Init_LateSubmission.HasValue) Then
                rblLateSubmission.SelectedValue = Convert.ToInt32(init_results.Init_LateSubmission.Value)
            End If
            If (init_results.Fin_SelfEmployed.HasValue) Then
                rblFinSelfEmployed.SelectedValue = Convert.ToInt32(init_results.Fin_SelfEmployed.Value)
            End If

            Try
                'EXT results
                Dim results As SC_IncapExt_Findings = SpecCase.INCAPExtFindings.Last
                If (Not IsDBNull(results.med_AMRODisposition)) Then
                    If (results.med_AMRODisposition.Trim.Length > 0) Then
                        ddlMedDisposition.SelectedValue = results.med_AMRODisposition
                    End If
                End If
                If (Not results.MED_IRILOStatus.Equals("")) Then
                    ddlMedIRILO.SelectedValue = results.MED_IRILOStatus
                End If
                If (results.EXT_StartDate.HasValue) Then
                    ExtensionStartDate_Date.Text = results.EXT_StartDate.Value.ToString(DATE_FORMAT)
                    ExtensionStartDate_Time.Text = results.EXT_StartDate.Value.ToString(HOUR_FORMAT)
                End If
                If (results.EXT_EndDate.HasValue) Then
                    ExtensionEndDate_Date.Text = results.EXT_EndDate.Value.ToString(DATE_FORMAT)
                    ExtensionEndDate_Time.Text = results.EXT_EndDate.Value.ToString(HOUR_FORMAT)
                End If

                If (results.MED_AMROStartDate.HasValue) Then
                    AMROStartDate_Date.Text = results.MED_AMROStartDate.Value.ToString(DATE_FORMAT)
                    AMROStartDate_Time.Text = results.MED_AMROStartDate.Value.ToString(HOUR_FORMAT)
                End If
                If (results.MED_AMROEndDate.HasValue) Then
                    AMROEndDate_Date.Text = results.MED_AMROEndDate.Value.ToString(DATE_FORMAT)
                    AMROEndDate_Time.Text = results.MED_AMROEndDate.Value.ToString(HOUR_FORMAT)
                End If
                If (results.MED_NextAMROStartDate.HasValue) Then
                    NextAMROStartDate_Date.Text = results.MED_NextAMROStartDate.Value.ToString(DATE_FORMAT)
                    NextAMROStartDate_Time.Text = results.MED_NextAMROStartDate.Value.ToString(HOUR_FORMAT)
                End If
                If (results.MED_NextAMROEndDate.HasValue) Then
                    NextAMROEndDate_Date.Text = results.MED_NextAMROEndDate.Value.ToString(DATE_FORMAT)
                    NextAMROEndDate_Time.Text = results.MED_NextAMROEndDate.Value.ToString(HOUR_FORMAT)
                End If

                If (Not IsDBNull(results.EXT_Number)) Then
                    If (results.EXT_Number > 0) Then
                        txtExtCount.Text = results.EXT_Number.ToString()
                    Else
                        txtExtCount.Text = 1
                    End If
                Else
                    txtExtCount.Text = 1
                End If
                If (results.MED_ExtRecommendation.HasValue) Then
                    rblMedExtRecommendation.SelectedValue = Convert.ToInt32(results.MED_ExtRecommendation.Value)
                End If
                If (results.IC_ExtRecommendation.HasValue) Then
                    rblICExtRecommendation.SelectedValue = Convert.ToInt32(results.IC_ExtRecommendation.Value)
                End If
                If (results.WJA_ConcurWithIC.HasValue) Then
                    rblWJAConcur.SelectedValue = Convert.ToInt32(results.WJA_ConcurWithIC.Value)
                End If
                If (results.FIN_ExtIncomeLost.HasValue) Then
                    rblFinExtLossEarning.SelectedValue = Convert.ToInt32(results.FIN_ExtIncomeLost.Value)
                End If
                If (results.WCC_ExtApproval.HasValue) Then
                    rblWCC_EXTPAY.SelectedValue = Convert.ToInt32(results.WCC_ExtApproval.Value)
                End If
            Catch
                Exit Sub
            End Try
        End Sub

        Private Sub LoadFindings()
            Dim workflowType As String = GetWorkflowType(WsId)

            Select Case workflowType
                Case "Initiate"
                    LoadInitialFindings()
                Case "Ext"
                    LoadExtFindings()
                Case "Appeal"
                    LoadInitialFindings()
                    LoadAppealFindings()
            End Select
            'LoadSeniorMedicalReviewerFindings()
        End Sub

        Private Sub LoadInitialFindings()
            Dim results As SC_Incap_Findings
            Try
                results = SpecCase.INCAPFindings.First
            Catch
                SpecCaseDao.CreateINCAPFindings(RefId)
                Exit Sub
            End Try
            If (results.Init_StartDate.HasValue) Then
                InitialStartDate_Date.Text = results.Init_StartDate.Value.ToString(DATE_FORMAT)
                InitialStartDate_Time.Text = results.Init_StartDate.Value.ToString(HOUR_FORMAT)
            End If
            If (results.Init_EndDate.HasValue) Then
                InitialEndDate_Date.Text = results.Init_EndDate.Value.ToString(DATE_FORMAT)
                InitialEndDate_Time.Text = results.Init_EndDate.Value.ToString(HOUR_FORMAT)
            End If
            If (Not IsDBNull(results.Med_ReportType)) Then
                If (results.Med_ReportType IsNot Nothing) Then
                    If (results.Med_ReportType.Trim.Length > 0) Then
                        ddlMedAvailabilityCode.SelectedValue = results.Med_ReportType
                        'If (results.Med_ReportType = "AAC 37") Then
                        '    rblMedRevreport.SelectedValue = "37"
                        'Else
                        '    rblMedRevreport.SelectedValue = "31"
                        'End If
                    End If
                End If
            End If
            If (results.Med_AbilityToPreform.HasValue) Then
                rblMedRevDecision.SelectedValue = Convert.ToInt32(results.Med_AbilityToPreform.Value)
            End If
            If (results.Init_LateSubmission.HasValue) Then
                rblLateSubmission.SelectedValue = Convert.ToInt32(results.Init_LateSubmission.Value)
            End If
            If (results.IC_Recommendation.HasValue) Then
                rblICINRecommendation.SelectedValue = Convert.ToInt32(results.IC_Recommendation.Value)
            End If
            If (results.Wing_Ja_Concur.HasValue) Then
                rblWJAConcur.SelectedValue = Convert.ToInt32(results.Wing_Ja_Concur.Value)
            End If
            If (results.Fin_IncomeLost.HasValue) Then
                rblFinLossEarning.SelectedValue = Convert.ToInt32(results.Fin_IncomeLost.Value)
            End If
            If (results.Fin_SelfEmployed.HasValue) Then
                rblFinSelfEmployed.SelectedValue = Convert.ToInt32(results.Fin_SelfEmployed.Value)
            End If
            If (results.WCC_InitApproval.HasValue) Then
                rblWCC_INPAY.SelectedValue = Convert.ToInt32(results.WCC_InitApproval.Value)
            End If
            If (results.Init_AppealOrComplete.HasValue) Then
                rblAppealorComplete.SelectedValue = Convert.ToInt32(results.Init_AppealOrComplete.Value)
            End If
            If (results.Init_ExtOrComplete.HasValue) Then
                rblExtorComplete.SelectedValue = Convert.ToInt32(results.Init_ExtOrComplete.Value)
            End If
        End Sub

        Private Sub PopulateMedExtDDL()
            Dim ac_ds As DataSet = New DataSet()
            Dim md_ds As DataSet = New DataSet()
            Dim ir_ds As DataSet = New DataSet()
            ddlMedAvailabilityCode.AppendDataBoundItems = True
            ddlMedDisposition.AppendDataBoundItems = True
            ddlMedIRILO.AppendDataBoundItems = True
            ddlMedAvailabilityCode.Items.Add("--- Select Availability Code ---")
            ac_ds = GetAvailabilityCode()

            For Each ac As DataRow In ac_ds.Tables.Item(0).Rows
                ddlMedAvailabilityCode.Items.Add(ac.ItemArray(1).ToString())
            Next
            ddlMedAvailabilityCode.DataBind()
            ddlMedDisposition.Items.Add("--- Select AMRO Disposition ---")
            md_ds = GetMedDisposition()
            For Each md As DataRow In md_ds.Tables.Item(0).Rows
                ddlMedDisposition.Items.Add(md.ItemArray(1).ToString())
            Next
            ddlMedIRILO.Items.Add("--- Select I-RILO Status ---")
            ir_ds = GetIRILOStatus()

            For Each ir As DataRow In ir_ds.Tables.Item(0).Rows
                ddlMedIRILO.Items.Add(ir.ItemArray(1).ToString())
            Next
        End Sub

        Private Sub ResetAll()
            'Initial
            InitialStartDate_Date.Enabled = False
            InitialStartDate_Time.Enabled = False
            InitialEndDate_Date.Enabled = False
            InitialEndDate_Time.Enabled = False
            'Extentions
            ExtensionStartDate_Date.Enabled = False
            ExtensionStartDate_Time.Enabled = False
            ExtensionEndDate_Date.Enabled = False
            ExtensionEndDate_Time.Enabled = False
            'PMExtDateSection.Visible = False
            'PMExtNumSection.Visible = False
        End Sub

        Private Sub SaveAppealData()
            SpecCaseDao.CreateINCAPAppealFindings(RefId)
            Dim findings As SC_IncapAppeal_Findings = SpecCase.INCAPAppealFindings.Last
            UpdateAppealFindings(findings)
            SpecCaseDao.SaveOrUpdate(SpecCase)
            SpecCaseDao.CommitChanges()
        End Sub

        Private Sub SaveData()
            If (Not UserCanEdit) Then
                Exit Sub
            End If
            Dim workflowType As String = GetWorkflowType(WsId)

            Select Case workflowType
                Case "Initiate"
                    SaveInitialData()
                Case "Ext"
                    SaveExtData()
                Case "Appeal"
                    SaveAppealData()
            End Select
        End Sub

        Private Sub SaveExtData()
            If (SESSION_WS_ID(RefId) = AFRCWorkflows.INExtension) Then
                SpecCaseDao.CreateINCAPExtFindings(RefId)
            End If
            Dim findings As SC_IncapExt_Findings = SpecCase.INCAPExtFindings.Last
            UpdateExtFindings(findings)
            SpecCaseDao.SaveOrUpdate(SpecCase)
            SpecCaseDao.CommitChanges()
        End Sub

        Private Sub SaveInitialData()
            Dim findings As SC_Incap_Findings = SpecCase.INCAPFindings.First
            UpdateInitiateFindings(findings)
            SpecCaseDao.SaveOrUpdate(SpecCase)
            SpecCaseDao.CommitChanges()
        End Sub

        Private Sub SaveSeniorMedicalReviewerData()
            SaveSeniorMedicalReviewerDecision()
            SaveSeniorMedicalReviewerFinding()
            SaveSeniorMedicalReviewerDecisionComment()

            SpecCaseDao.SaveOrUpdate(SpecCase)
            SpecCaseDao.CommitChanges()
        End Sub

        Private Sub SaveSeniorMedicalReviewerDecision()
            'If (rblSeniorMedicalReviewerDecision.Visible = False OrElse String.IsNullOrEmpty(rblSeniorMedicalReviewerDecision.SelectedValue)) Then
            '    Exit Sub
            'End If

            'SpecCase.SeniorMedicalReviewerConcur = Server.HtmlEncode(rblSeniorMedicalReviewerDecision.SelectedValue)
        End Sub

        Private Sub SaveSeniorMedicalReviewerDecisionComment()
            'If (txtSeniorMedicalReviewerDecisionComment.Text.Length > txtSeniorMedicalReviewerDecisionComment.MaxLength) Then
            '    Exit Sub
            'End If

            'SpecCase.SeniorMedicalReviewerComment = Server.HtmlEncode(txtSeniorMedicalReviewerDecisionComment.Text)
        End Sub

        Private Sub SaveSeniorMedicalReviewerFinding()
            'If (rblSeniorMedicalReviewerDecision.Visible = False AndAlso Not String.IsNullOrEmpty(rblSeniorMedicalReviewerFindings.SelectedValue)) Then
            '    SpecCase.SeniorMedicalReviewerApproved = Integer.Parse(rblSeniorMedicalReviewerFindings.SelectedValue)
            'ElseIf (rblSeniorMedicalReviewerDecision.Visible = True AndAlso rblSeniorMedicalReviewerDecision.SelectedValue.Equals(SpecialCase.DECISION_NONCONCUR) AndAlso Not String.IsNullOrEmpty(rblSeniorMedicalReviewerFindings.SelectedValue)) Then
            '    SpecCase.SeniorMedicalReviewerApproved = Integer.Parse(rblSeniorMedicalReviewerFindings.SelectedValue)
            'Else
            '    SpecCase.SeniorMedicalReviewerApproved = Nothing
            'End If
        End Sub

        Private Sub SetAccessForControls()
            SetReadOnlyAccessForControls()

            If (UserCanEdit) Then
                SetReadWriteAccessForControls()
            End If
        End Sub

        Private Sub SetInputFormatRestrictions()
            'SetInputFormatRestriction(Page, EventDetailsBox, FormatRestriction.AlphaNumeric, PERMITTED_SPECIAL_CHAR_INPUT)
            'SetInputFormatRestriction(Page, HospitalNameBox, FormatRestriction.AlphaNumeric, PERMITTED_SPECIAL_CHAR_INPUT)
            'SetInputFormatRestriction(Page, DiagnosisTextBox, FormatRestriction.AlphaNumeric, PERMITTED_SPECIAL_CHAR_INPUT)
            'SetInputFormatRestriction(Page, txtApprovalComments, FormatRestriction.AlphaNumeric, PERMITTED_SPECIAL_CHAR_INPUT)
            SetInputFormatRestriction(Page, InitialStartDate_Date, FormatRestriction.Numeric, "/")
            SetInputFormatRestriction(Page, InitialStartDate_Time, FormatRestriction.Numeric)
        End Sub

        Private Sub SetMaxLengthForControls()
            'SetMaxLength(DecisionComment)
            'SetMaxLength(txtSeniorMedicalReviewerDecisionComment)
        End Sub

        'Private Function GetICTypeText() As String
        '    Dim wsId As Integer = SESSION_WS_ID(RefId)
        '    Select Case wsId
        '        Case AFRCWorkflows.INExtension, AFRCWorkflows.INMedicalReview_WG_Ext,
        '             AFRCWorkflows.INImmediateCommanderReview_Ext, AFRCWorkflows.INFinanceReview_Ext, AFRCWorkflows.INWingJAReview_Ext
        '            ICTypeText.Text = "Extension"
        '        Case Else
        '            ICTypeText.Text = "Initial"
        '    End Select
        'End Function
        Private Sub SetReadOnlyAccessForControls()
            'DecisionY.Enabled = False
            'DecisionN.Enabled = False
            'DecisionComment.Enabled = False
            'rblSeniorMedicalReviewerDecision.Enabled = False
            'rblSeniorMedicalReviewerFindings.Enabled = False
            'txtSeniorMedicalReviewerDecisionComment.Enabled = False
        End Sub

        'Private Sub SetInputFormatRestrictions()
        '    SetInputFormatRestriction(Page, DecisionComment, FormatRestriction.AlphaNumeric, PERMITTED_SPECIAL_CHAR_INPUT)
        '    SetInputFormatRestriction(Page, txtSeniorMedicalReviewerDecisionComment, FormatRestriction.AlphaNumeric, PERMITTED_SPECIAL_CHAR_INPUT)
        'End Sub
        Private Sub SetReadWriteAccessForControls()
            If (SessionInfo.SESSION_GROUP_ID = UserGroups.BoardMedical) Then
                'DecisionY.Enabled = True
                'DecisionN.Enabled = True
                'DecisionComment.Enabled = True
            End If

            If (SessionInfo.SESSION_GROUP_ID = UserGroups.SeniorMedicalReviewer) Then
                'rblSeniorMedicalReviewerDecision.Enabled = True
                'rblSeniorMedicalReviewerFindings.Enabled = True
                'txtSeniorMedicalReviewerDecisionComment.Enabled = True
            End If
        End Sub

        Private Sub TabButtonClicked(ByRef sender As Object, ByRef e As TabNavigationEventArgs)
            If (e.ButtonType = NavigatorButtonType.Save OrElse
                e.ButtonType = NavigatorButtonType.NavigatedAway OrElse
                e.ButtonType = NavigatorButtonType.NextStep OrElse
                e.ButtonType = NavigatorButtonType.PreviousStep) Then
                SaveData()
            End If

            If Not (ValidBoxLength()) Then
                e.Cancel = True
                Exit Sub
            End If
        End Sub

        Private Function UpdateAppealFindings(ByVal fnd As SC_IncapAppeal_Findings) As SC_IncapAppeal_Findings
            If (WsId = AFRCWorkflows.INWingCCRecommendation_Appeal) Then
                If (Not rblWCC_AppealPAY.SelectedValue = "" AndAlso rblWCC_AppealPAY.SelectedIndex >= 0) Then
                    fnd.WCC_AppealApproval = rblWCC_AppealPAY.SelectedItem.Value
                End If
            End If

            Return fnd
        End Function

        Private Function UpdateExtFindings(ByVal fnd As SC_IncapExt_Findings) As SC_IncapExt_Findings
            Select Case WsId
                Case AFRCWorkflows.INExtension
                    If (ExtensionStartDate_Time.Text.Trim.Length > 0) Then
                        fnd.EXT_StartDate = ParseDateAndTime(ExtensionStartDate_Date.Text.Trim + " " + ExtensionStartDate_Time.Text.Trim)
                    ElseIf (ExtensionStartDate_Date.Text.Trim.Length > 0) Then
                        fnd.EXT_StartDate = Date.Parse(ExtensionStartDate_Date.Text.ToString())
                    End If
                    If (ExtensionEndDate_Time.Text.Trim.Length > 0) Then
                        fnd.EXT_EndDate = ParseDateAndTime(ExtensionEndDate_Date.Text.Trim + " " + ExtensionEndDate_Time.Text.Trim)
                    ElseIf (ExtensionEndDate_Date.Text.Trim.Length > 0) Then
                        fnd.EXT_EndDate = Date.Parse(ExtensionEndDate_Date.Text.ToString())
                    End If
                Case AFRCWorkflows.INMedicalReview_WG_Ext
                    If (Not rblMedExtRecommendation.SelectedValue = "" AndAlso rblMedExtRecommendation.SelectedIndex >= 0) Then
                        fnd.MED_ExtRecommendation = rblMedExtRecommendation.SelectedItem.Value
                    End If
                    If (AMROStartDate_Time.Text.Trim.Length > 0) Then
                        fnd.MED_AMROStartDate = ParseDateAndTime(AMROStartDate_Date.Text.Trim + " " + AMROStartDate_Time.Text.Trim)
                    ElseIf (AMROStartDate_Date.Text.Trim.Length > 0) Then
                        fnd.MED_AMROStartDate = Date.Parse(AMROStartDate_Date.Text.ToString())
                    End If
                    If (AMROEndDate_Time.Text.Trim.Length > 0) Then
                        fnd.MED_AMROEndDate = ParseDateAndTime(AMROEndDate_Date.Text.Trim + " " + AMROEndDate_Time.Text.Trim)
                    ElseIf (AMROEndDate_Date.Text.Trim.Length > 0) Then
                        fnd.MED_AMROEndDate = Date.Parse(AMROEndDate_Date.Text.ToString())
                    End If
                    If (NextAMROStartDate_Time.Text.Trim.Length > 0) Then
                        fnd.MED_NextAMROStartDate = ParseDateAndTime(NextAMROStartDate_Date.Text.Trim + " " + NextAMROStartDate_Time.Text.Trim)
                    ElseIf (NextAMROStartDate_Date.Text.Trim.Length > 0) Then
                        fnd.MED_NextAMROStartDate = Date.Parse(NextAMROStartDate_Date.Text.ToString())
                    End If
                    If (NextAMROEndDate_Time.Text.Trim.Length > 0) Then
                        fnd.MED_NextAMROEndDate = ParseDateAndTime(NextAMROEndDate_Date.Text.Trim + " " + NextAMROEndDate_Time.Text.Trim)
                    ElseIf (NextAMROEndDate_Date.Text.Trim.Length > 0) Then
                        fnd.MED_NextAMROEndDate = Date.Parse(NextAMROEndDate_Date.Text.ToString())
                    End If
                    If (Not ddlMedDisposition.SelectedValue = "" AndAlso Not ddlMedDisposition.SelectedValue = "--- Select AMRO Disposition ---" AndAlso ddlMedDisposition.SelectedIndex >= 0) Then
                        fnd.med_AMRODisposition = ddlMedDisposition.SelectedItem.Value
                    End If
                    If (Not ddlMedIRILO.SelectedValue = "" AndAlso Not ddlMedIRILO.SelectedValue = "--- Select I-RILO Status ---" AndAlso ddlMedIRILO.SelectedIndex >= 0) Then
                        fnd.MED_IRILOStatus = ddlMedIRILO.SelectedItem.Value
                    End If
                Case AFRCWorkflows.INImmediateCommanderReview_Ext
                    If (Not rblICExtRecommendation.SelectedValue = "" AndAlso rblICExtRecommendation.SelectedIndex >= 0) Then
                        fnd.IC_ExtRecommendation = rblICExtRecommendation.SelectedItem.Value
                    End If

                Case AFRCWorkflows.INWingJAReview_Ext
                    If (Not rblWJAConcur.SelectedValue = "" AndAlso rblWJAConcur.SelectedIndex >= 0) Then
                        fnd.WJA_ConcurWithIC = rblWJAConcur.SelectedItem.Value
                    End If

                Case AFRCWorkflows.INFinanceReview_Ext
                    If (Not rblFinExtLossEarning.SelectedValue = "" AndAlso rblFinExtLossEarning.SelectedIndex >= 0) Then
                        fnd.FIN_ExtIncomeLost = rblFinExtLossEarning.SelectedItem.Value
                    End If

                Case AFRCWorkflows.INWingCommanderRecommendation_Ext
                    If (Not rblWCC_EXTPAY.SelectedValue = "" AndAlso rblWCC_EXTPAY.SelectedIndex >= 0) Then
                        fnd.WCC_ExtApproval = rblWCC_EXTPAY.SelectedItem.Value
                    End If

            End Select
            Return fnd
        End Function

        Private Function UpdateInitiateFindings(ByVal fnd As SC_Incap_Findings) As SC_Incap_Findings
            Select Case WsId
                Case AFRCWorkflows.INInitiate
                    If (InitialStartDate_Time.Text.Trim.Length > 0) Then
                        fnd.Init_StartDate = ParseDateAndTime(InitialStartDate_Date.Text.Trim + " " + InitialStartDate_Time.Text.Trim)
                    ElseIf (InitialStartDate_Date.Text.Trim.Length > 0) Then
                        fnd.Init_StartDate = Date.Parse(InitialStartDate_Date.Text.ToString())
                    End If
                    If (InitialEndDate_Time.Text.Trim.Length > 0) Then
                        fnd.Init_EndDate = ParseDateAndTime(InitialEndDate_Date.Text.Trim + " " + InitialEndDate_Time.Text.Trim)
                    ElseIf (InitialEndDate_Date.Text.Trim.Length > 0) Then
                        fnd.Init_EndDate = InitialEndDate_Date.Text
                    End If
                    If (Not rblExtorComplete.SelectedValue = "" AndAlso rblExtorComplete.SelectedIndex >= 0) Then
                        fnd.Init_ExtOrComplete = rblExtorComplete.SelectedItem.Value
                    End If
                    If (Not rblLateSubmission.SelectedValue = "" AndAlso rblLateSubmission.SelectedIndex >= 0) Then
                        fnd.Init_LateSubmission = rblLateSubmission.SelectedItem.Value
                    End If
                Case AFRCWorkflows.INMedicalReview_WG
                    If (Not ddlMedAvailabilityCode.SelectedValue = "" AndAlso ddlMedAvailabilityCode.SelectedIndex >= 0) Then
                        fnd.Med_ReportType = ddlMedAvailabilityCode.SelectedItem.Text
                    End If
                    If (Not rblMedRevDecision.SelectedValue = "" AndAlso rblMedRevDecision.SelectedIndex >= 0) Then
                        fnd.Med_AbilityToPreform = rblMedRevDecision.SelectedItem.Value
                    End If
                Case AFRCWorkflows.INImmediateCommanderReview
                    If (Not rblICINRecommendation.SelectedValue = "" AndAlso rblICINRecommendation.SelectedIndex >= 0) Then
                        fnd.IC_Recommendation = rblICINRecommendation.SelectedItem.Value
                    End If
                Case AFRCWorkflows.INWingJAReview
                    If (Not rblWJAConcur.SelectedValue = "" AndAlso rblWJAConcur.SelectedIndex >= 0) Then
                        fnd.Wing_Ja_Concur = rblWJAConcur.SelectedItem.Value
                    End If
                Case AFRCWorkflows.INFinanceReview
                    If (Not rblFinLossEarning.SelectedValue = "" AndAlso rblFinLossEarning.SelectedIndex >= 0) Then
                        fnd.Fin_IncomeLost = rblFinLossEarning.SelectedItem.Value
                    End If
                    If (Not rblFinSelfEmployed.SelectedValue = "" AndAlso rblFinSelfEmployed.SelectedIndex >= 0) Then
                        fnd.Fin_SelfEmployed = rblFinSelfEmployed.SelectedItem.Value
                    End If
                Case AFRCWorkflows.INWingCCAction
                    If (Not rblWCC_INPAY.SelectedValue = "" AndAlso rblWCC_INPAY.SelectedIndex >= 0) Then
                        fnd.WCC_InitApproval = rblWCC_INPAY.SelectedItem.Value
                    End If
                Case AFRCWorkflows.INApproved
                    If (Not rblExtorComplete.SelectedValue = "" AndAlso rblExtorComplete.SelectedIndex >= 0) Then
                        fnd.Init_ExtOrComplete = rblExtorComplete.SelectedItem.Value
                    End If
                Case AFRCWorkflows.INAppeal
                    If (Not rblAppealorComplete.SelectedValue = "" AndAlso rblAppealorComplete.SelectedIndex >= 0) Then
                        fnd.Init_AppealOrComplete = rblAppealorComplete.SelectedItem.Value
                    End If
            End Select
            Return fnd
        End Function

        Private Sub ViewEnableControls()
            Dim workflowType As String = GetWorkflowType(WsId)
            ResetAll()
            'GetICTypeText()
            PopulateMedExtDDL()
            Select Case workflowType
                'Initiate and Extention
                Case "Initiate"
                    WingCC_INTSection.Visible = True
                    FinIntSection.Visible = True
                    ICINSection.Visible = True
                    FinSelfEmployedSection.Visible = True
                    MedAvailabilityCodeSection.Visible = True
                    Select Case WsId
                        Case AFRCWorkflows.INInitiate
                            If (SESSION_GROUP_ID = UserGroups.INCAP_PM) Then
                                InitialStartDate_Date.Enabled = True
                                InitialStartDate_Time.Enabled = True
                                InitialEndDate_Date.Enabled = True
                                InitialEndDate_Time.Enabled = True
                                InitDatePickerCSSClasses(InitialStartDate_Date, 2)
                                InitDatePickerCSSClasses(InitialEndDate_Date, 0)
                                'InitialStartDate_Date.CssClass = False
                                'pnlICInformation.Enabled = True
                                rblLateSubmission.Enabled = True
                            End If
                        Case AFRCWorkflows.INMedicalReview_WG
                            If (SESSION_GROUP_ID = UserGroups.MedicalOfficer OrElse SESSION_GROUP_ID = UserGroups.MedicalTechnician) Then
                                ddlMedAvailabilityCode.Enabled = True
                                rblMedRevDecision.Enabled = True
                            End If
                        Case AFRCWorkflows.INImmediateCommanderReview
                            ICINSection.Visible = True
                            If (SESSION_GROUP_ID = UserGroups.UnitCommander) Then
                                rblICINRecommendation.Enabled = True
                            End If
                        Case AFRCWorkflows.INWingJAReview
                            If (SESSION_GROUP_ID = UserGroups.WingJudgeAdvocate) Then
                                rblWJAConcur.Enabled = True
                            End If
                        Case AFRCWorkflows.INFinanceReview
                            FinIntSection.Visible = True
                            FinSelfEmployedSection.Visible = True
                            If (SESSION_GROUP_ID = UserGroups.FMTech) Then
                                rblFinLossEarning.Enabled = True
                                rblFinSelfEmployed.Enabled = True
                            End If
                        Case AFRCWorkflows.INWingCCAction
                            If (SESSION_GROUP_ID = UserGroups.WingCommander) Then
                                rblWCC_INPAY.Enabled = True
                            End If
                        Case AFRCWorkflows.INWingCCRecommendation_Appeal
                            WingCC_AppealSection.Visible = True
                            If (SESSION_GROUP_ID = UserGroups.WingCommander) Then
                                rblWCC_AppealPAY.Enabled = True
                            End If
                        Case AFRCWorkflows.INApproved
                            If (SESSION_GROUP_ID = UserGroups.INCAP_PM) Then
                                PMExtOrCompleteSection.Visible = True
                            End If
                        Case AFRCWorkflows.INAppeal
                            If (SESSION_GROUP_ID = UserGroups.INCAP_PM) Then
                                PMAppealOrCompleteSection.Visible = True
                            End If
                    End Select

                'Extention
                Case "Ext"
                    PMExtNumSection.Visible = True
                    PMExtDateSection.Visible = True
                    MedExtRecommendationSection.Visible = True
                    ICExtSection.Visible = True
                    FinExtSection.Visible = True
                    WingCC_EXTSection.Visible = True
                    FinSelfEmployedSection.Visible = True
                    FinSelfEmployedSection_text.InnerText = "A"
                    MedAvailabilityCodeSection.Visible = True
                    MedAMROSection.Visible = True
                    MedDispositionSection.Visible = True
                    MedNextAMROSection.Visible = True
                    MedIRILOSection.Visible = True
                    Select Case WsId
                        Case AFRCWorkflows.INExtension
                            If (SESSION_GROUP_ID = UserGroups.INCAP_PM) Then
                                txtExtCount.Enabled = True
                                InitDatePickerCSSClasses(ExtensionStartDate_Date, 2)
                                InitDatePickerCSSClasses(ExtensionEndDate_Date, 0)
                                ExtensionStartDate_Date.Enabled = True
                                ExtensionStartDate_Time.Enabled = True
                                ExtensionEndDate_Date.Enabled = True
                                ExtensionEndDate_Time.Enabled = True
                            End If
                        Case AFRCWorkflows.INMedicalReview_WG_Ext
                            If (SESSION_GROUP_ID = UserGroups.MedicalOfficer OrElse SESSION_GROUP_ID = UserGroups.MedicalTechnician) Then
                                rblMedExtRecommendation.Enabled = True
                                InitDatePickerCSSClasses(NextAMROStartDate_Date, 0)
                                InitDatePickerCSSClasses(NextAMROEndDate_Date, 0)
                                NextAMROStartDate_Date.Enabled = True
                                NextAMROStartDate_Time.Enabled = True
                                NextAMROEndDate_Date.Enabled = True
                                NextAMROEndDate_Time.Enabled = True
                                InitDatePickerCSSClasses(AMROStartDate_Date, 1)
                                InitDatePickerCSSClasses(AMROEndDate_Date, 2)
                                AMROStartDate_Date.Enabled = True
                                AMROStartDate_Time.Enabled = True
                                AMROEndDate_Date.Enabled = True
                                AMROEndDate_Time.Enabled = True
                                'ddlMedAvailabilityCode.Enabled = True
                                ddlMedDisposition.Enabled = True
                                ddlMedIRILO.Enabled = True
                                rblMedExtRecommendation.Enabled = True
                            End If
                        Case AFRCWorkflows.INImmediateCommanderReview_Ext
                            If (SESSION_GROUP_ID = UserGroups.UnitCommander) Then
                                rblICExtRecommendation.Enabled = True
                            End If
                        Case AFRCWorkflows.INWingJAReview_Ext
                            If (SESSION_GROUP_ID = UserGroups.WingJudgeAdvocate) Then
                                rblWJAConcur.Enabled = True
                            End If
                        Case AFRCWorkflows.INFinanceReview_Ext
                            If (SESSION_GROUP_ID = UserGroups.FMTech) Then
                                rblFinExtLossEarning.Enabled = True
                            End If
                        Case AFRCWorkflows.INWingCommanderRecommendation_Ext
                            If (SESSION_GROUP_ID = UserGroups.WingCommander) Then
                                rblWCC_EXTPAY.Enabled = True
                            End If
                        Case AFRCWorkflows.INDisposition
                            If (SESSION_GROUP_ID = UserGroups.INCAP_PM) Then
                                PMExtOrCompleteSection.Visible = True
                                rblExtorComplete.Enabled = True
                                PMExtOrCompleteTitle.InnerText = "E"
                            End If
                    End Select
                Case "Appeal"
                    WingCC_INTSection.Visible = True
                    FinIntSection.Visible = True
                    FinSelfEmployedSection.Visible = True
                    ICINSection.Visible = True
                    WingCC_AppealSection.Visible = True
                    PMAppealOrCompleteSection.Visible = True
                    If (AFRCWorkflows.INWingCCRecommendation_Appeal AndAlso SESSION_GROUP_ID = UserGroups.WingCommander) Then
                        rblWCC_AppealPAY.Enabled = True
                    End If

            End Select
            SetInputFormatRestrictions()
        End Sub

#End Region

    End Class

End Namespace