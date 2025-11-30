Imports ALOD.Core.Domain.Common
Imports ALOD.Core.Domain.Modules.Lod
Imports ALOD.Core.Domain.Modules.SpecialCases
Imports ALOD.Core.Domain.Users
Imports ALOD.Core.Domain.Workflow
Imports ALOD.Core.Interfaces.DAOInterfaces
Imports ALOD.Core.Utils
Imports ALOD.Data
Imports ALOD.Logging
Imports ALOD.Web.UserControls
Imports ALODWebUtility.Common

Namespace Web.Special_Case.WWD

    Partial Class Secure_sc_WD_MedOff
        Inherits System.Web.UI.Page

#Region "Fields"

        Private _daoFactory As IDaoFactory
        Private _memoDao As IMemoDao2
        Private _scAccess As Dictionary(Of String, ALOD.Core.Domain.Workflow.PageAccessType)
        Private _specCase As SC_WWD = Nothing
        Private _specCaseDao As ISpecialCaseDAO

#End Region

#Region "Properties"

        Public ReadOnly Property Navigator() As TabNavigator
            Get
                Return MasterPage.Navigator
            End Get
        End Property

        Public ReadOnly Property RefId() As Integer
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

        Protected ReadOnly Property DaoFactory() As IDaoFactory
            Get
                If (_daoFactory Is Nothing) Then
                    _daoFactory = New NHibernateDaoFactory()
                End If

                Return _daoFactory
            End Get
        End Property

        Protected ReadOnly Property MasterPage() As SC_WWDMaster
            Get
                Dim master As SC_WWDMaster = CType(Page.Master, SC_WWDMaster)
                Return master
            End Get
        End Property

        Protected ReadOnly Property MemoDao As IMemoDao2
            Get
                If (_memoDao Is Nothing) Then
                    _memoDao = DaoFactory.GetMemoDao2()
                End If

                Return _memoDao
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

        Protected ReadOnly Property SpecCase() As SC_WWD
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

#End Region

#Region "Page Methods"

        Public Function ValidBoxLength() As Boolean
            Dim IsValid As Boolean = True
            If Not CheckTextLength(DiagnosisTextBox) Then
                IsValid = False
            End If
            If Not CheckTextLength(DecisionComment) Then
                IsValid = False
            End If
            If Not CheckTextLength(DQParagraph) Then
                IsValid = False
            End If

            If Not CheckTextLength(txtSeniorMedicalReviewerDiagnosisText) Then
                IsValid = False
            End If
            If Not CheckTextLength(txtSeniorMedicalReviewerDecisionComment) Then
                IsValid = False
            End If
            If Not CheckTextLength(txtSeniorMedicalReviewerDQParagraph) Then
                IsValid = False
            End If
            Return IsValid
        End Function

        Protected Sub Page_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
            AddHandler Me.Master.TabClick, AddressOf TabButtonClicked
        End Sub

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
            InitializeUserControls()

            If (Not IsPostBack) Then
                UserCanEdit = GetAccess(Navigator.PageAccess, True)
                InitControls()
                LogManager.LogAction(ModuleType.SpecCaseWWD, UserAction.ViewPage, RefId, "Viewed Page: Board Med")
            End If

            'zach,s et admin lod rb disabled for boardmedical on this tab
            If Session(SESSIONKEY_COMPO) = "5" Then
                If (SessionInfo.SESSION_GROUP_ID = UserGroups.BoardMedical) Then 'UserGroups.BoardMedical
                    DecisionAdminLOD.Enabled = False
                End If
            End If

        End Sub

        Protected Sub rblSeniorMedicalReviewerDecision_SelectedIndexChanged(sender As Object, e As EventArgs) Handles rblSeniorMedicalReviewerDecision.SelectedIndexChanged
            SetSeniorMedicalReviewerFindingsPanelVisibility()
        End Sub

        Protected Sub Save_Click(ByRef sender As Object, ByRef e As TabNavigationEventArgs)
            If (Navigator.CurrentStep.IsReadOnly) Then
                Exit Sub
            End If

            If Not (ValidBoxLength()) Then
                e.Cancel = True
                Exit Sub
            End If

            If (e.ButtonType = NavigatorButtonType.Save OrElse
                e.ButtonType = NavigatorButtonType.NavigatedAway OrElse
                e.ButtonType = NavigatorButtonType.NextStep OrElse
                e.ButtonType = NavigatorButtonType.PreviousStep) Then
                SaveData()
            End If
        End Sub

        Private Sub InitControls()
            SetInputFormatRestrictions()
            SetAccessForControls()
            LoadFindings()
            SetMaxLengthForControls()
            SetControlsVisibility()
            SetFindingsPanelIndentation()
        End Sub

        Private Sub InitializeUserControls()
            ucICDCodeControl.Initialilze(Me)
            ucICD7thCharacterControl.Initialize(ucICDCodeControl)

            ucSeniorMedicalReviewerICDCodeControl.Initialilze(Me)
            ucSeniorMedicalReviewerICD7thCharacterControl.Initialize(ucSeniorMedicalReviewerICDCodeControl)
        End Sub

        Private Sub InitMemosDropDownList(memoDropDownListControl As DropDownList, selectedValue As Nullable(Of Integer))
            memoDropDownListControl.Items.Clear()

            memoDropDownListControl.DataSource = MemoDao.GetTemplatesByModule(ModuleType.SpecCaseWWD)
            memoDropDownListControl.DataTextField = "Title"
            memoDropDownListControl.DataValueField = "Id"
            memoDropDownListControl.DataBind()

            Utility.InsertDropDownListZeroValue(memoDropDownListControl, "-- Select a Memo --")

            If (selectedValue.HasValue AndAlso memoDropDownListControl.Items.FindByValue(selectedValue.Value) IsNot Nothing) Then
                memoDropDownListControl.SelectedValue = selectedValue.Value
            Else
                memoDropDownListControl.SelectedValue = 0
            End If
        End Sub

        Private Function IsCaseInBoardMedicalDispositionStep() As Boolean
            If (SpecCase.CurrentStatusCode = SpecCaseWWDStatusCode.MedicalReviewSAF OrElse
                SpecCase.CurrentStatusCode = SpecCaseWWDStatusCode.MedicalReviewIPEB OrElse
                SpecCase.CurrentStatusCode = SpecCaseWWDStatusCode.MedicalReviewFPEB) Then
                Return True
            End If

            Return False
        End Function

        Private Function IsCaseInHqAfrcTechnicianReviewStep() As Boolean
            If (SpecCase.CurrentStatusCode = SpecCaseWWDStatusCode.FinalReview OrElse
                SpecCase.CurrentStatusCode = SpecCaseWWDStatusCode.FinalReviewSAF OrElse
                SpecCase.CurrentStatusCode = SpecCaseWWDStatusCode.FinalReviewIPEB OrElse
                SpecCase.CurrentStatusCode = SpecCaseWWDStatusCode.FinalReviewFPEB) Then
                Return True
            End If

            Return False
        End Function

        Private Function IsCaseInSeniorMedicalReviewerDispositionStep() As Boolean
            If (SpecCase.CurrentStatusCode = SpecCaseWWDStatusCode.SeniorMedicalReviewSAF OrElse
                SpecCase.CurrentStatusCode = SpecCaseWWDStatusCode.SeniorMedicalReviewIPEB OrElse
                SpecCase.CurrentStatusCode = SpecCaseWWDStatusCode.SeniorMedicalReviewFPEB) Then
                Return True
            End If

            Return False
        End Function

        Private Sub LoadBoardMedicalFindings()
            If (SpecCase.med_off_approved = 1) Then
                DecisionY.Checked = True
            End If

            If (SpecCase.med_off_approved = 0) Then
                DecisionN.Checked = True
            End If

            If (SpecCase.med_off_approved = 3) Then
                DecisionRFA.Checked = True
            End If

            If (SpecCase.med_off_approved = 2) Then
                DecisionAdminLOD.Checked = True
            End If

            If (Not IsNothing(SpecCase.med_off_approval_comment)) Then
                DecisionComment.Text = Server.HtmlDecode(SpecCase.med_off_approval_comment.ToString())
            End If

            If (Not IsNothing(SpecCase.DQParagraph)) Then
                DQParagraph.Text = Server.HtmlDecode(SpecCase.DQParagraph.ToString())
            End If

            If (SpecCase.ExpirationDate.HasValue) Then
                ExpirationDate.Text = Server.HtmlDecode(SpecCase.ExpirationDate.Value.ToString(DATE_FORMAT))
            End If

            If (SpecCase.ReturnToDutyDate.HasValue) Then
                ReturnToDutyDate.Text = Server.HtmlDecode(SpecCase.ReturnToDutyDate.Value.ToString(DATE_FORMAT))
            End If

            If (SpecCase.ALCLetterType.HasValue) Then
                ddlAssignmentLimitationCode.SelectedValue = SpecCase.ALCLetterType
            End If

            If (Not String.IsNullOrEmpty(SpecCase.med_off_approval_comment)) Then
                DecisionComment.Text = HTMLDecodeNulls(SpecCase.med_off_approval_comment)
            End If

            InitMemosDropDownList(ddlMemos, SpecCase.MemoTemplateID)
            LoadIcdControls(ucICDCodeControl, DiagnosisTextBox, ucICD7thCharacterControl)
        End Sub

        Private Sub LoadFindings()
            LoadBoardMedicalFindings()
            LoadSeniorMedicalReviewerFindings()
        End Sub

        Private Sub LoadIcdControls(icdCodeControl As ICDCodeControl, icdDiagnosisTextBox As TextBox, icd7thCharacterControl As ICD7thCharacterControl)
            If (Not IsNothing(SpecCase.ICD9Code)) Then
                icdCodeControl.InitializeHierarchy(SpecCase.ICD9Code)

                If (icdCodeControl.IsValidICDCode(SpecCase.ICD9Code)) Then
                    icdCodeControl.UpdateICDCodeDiagnosisLabel(SpecCase.ICD9Code, True)
                    icdDiagnosisTextBox.Text = Server.HtmlDecode(SpecCase.ICD9Diagnosis)
                End If

                If (Not String.IsNullOrEmpty(SpecCase.ICD7thCharacter)) Then
                    icd7thCharacterControl.InitializeCharacters(SpecCase.ICD9Code, SpecCase.ICD7thCharacter)
                    icd7thCharacterControl.Update7thCharacterLabel(SpecCase.ICD9Code, SpecCase.ICD7thCharacter)
                Else
                    icd7thCharacterControl.InitializeCharacters(SpecCase.ICD9Code, String.Empty)
                End If
            Else
                icd7thCharacterControl.InitializeCharacters(0, String.Empty)
            End If
        End Sub

        Private Sub LoadSeniorMedicalReviewerFindings()
            If (Not String.IsNullOrEmpty(SpecCase.SeniorMedicalReviewerConcur)) Then
                rblSeniorMedicalReviewerDecision.SelectedValue = Server.HtmlDecode(SpecCase.SeniorMedicalReviewerConcur)
            End If

            If (SpecCase.SeniorMedicalReviewerApproved.HasValue) Then
                rblSeniorMedicalReviewerFindings.SelectedValue = SpecCase.SeniorMedicalReviewerApproved.Value
            End If

            If (Not String.IsNullOrEmpty(SpecCase.SeniorMedicalReviewerComment)) Then
                txtSeniorMedicalReviewerDecisionComment.Text = Server.HtmlDecode(SpecCase.SeniorMedicalReviewerComment)
            End If

            If (Not IsNothing(SpecCase.AlternateDQParagraph)) Then
                txtSeniorMedicalReviewerDQParagraph.Text = Server.HtmlDecode(SpecCase.AlternateDQParagraph.ToString())
            End If

            If (SpecCase.AlternateExpirationDate.HasValue) Then
                txtSeniorMedicalReviewerExpirationDate.Text = Server.HtmlDecode(SpecCase.AlternateExpirationDate.Value.ToString(DATE_FORMAT))
            End If

            If (SpecCase.AlternateReturnToDutyDate.HasValue) Then
                txtSeniorMedicalReviewerReturnToDutyDate.Text = Server.HtmlDecode(SpecCase.AlternateReturnToDutyDate.Value.ToString(DATE_FORMAT))
            End If

            If (SpecCase.AlternateALCLetterType.HasValue) Then
                ddlSeniorMedicalReviewerAssignmentLimitationCode.SelectedValue = SpecCase.AlternateALCLetterType
            End If

            InitMemosDropDownList(ddlSeniorMedicalReviewerMemos, SpecCase.AlternateMemoTemplateID)
            LoadIcdControls(ucSeniorMedicalReviewerICDCodeControl, txtSeniorMedicalReviewerDiagnosisText, ucSeniorMedicalReviewerICD7thCharacterControl)
        End Sub

        Private Sub SaveAllBoardMedicalData()
            SaveIcdData(ucICDCodeControl, DiagnosisTextBox, ucICD7thCharacterControl)
            SaveBoardMedicalMemoSelection()

            If (DecisionN.Checked) Then
                SpecCase.med_off_approved = SC_WWD.DISQUALIFY_FINDING_VALUE

                If (Not String.IsNullOrEmpty(DQParagraph.Text)) Then
                    SpecCase.DQParagraph = HTMLEncodeNulls(DQParagraph.Text.Trim())
                End If

                SpecCase.DQCompletionDate = Now
            End If

            If (DecisionY.Checked) Then
                SpecCase.med_off_approved = SC_WWD.QUALIFY_FINDING_VALUE
                SpecCase.DQParagraph = String.Empty
                SpecCase.DQCompletionDate = Nothing 'If the case is "Returned" to the Board Medical and decides to change the determination
            End If

            If (DecisionRFA.Checked) Then
                SpecCase.med_off_approved = SC_WWD.RFA_FINDING_VALUE
                SpecCase.DQParagraph = String.Empty
                SpecCase.DQCompletionDate = Nothing
            End If

            If (DecisionAdminLOD.Checked) Then
                SpecCase.med_off_approved = SC_WWD.ADMINLOD_FINDING_VALUE
                SpecCase.DQParagraph = String.Empty
                SpecCase.DQCompletionDate = Nothing 'If the case is "Returned" to the Board Medical and decides to change the determination
            End If

            If Not String.IsNullOrEmpty(DecisionComment.Text) Then
                SpecCase.med_off_approval_comment = HTMLEncodeNulls(DecisionComment.Text)
            End If

            If (Not String.IsNullOrEmpty(ddlAssignmentLimitationCode.SelectedValue)) Then
                SpecCase.ALCLetterType = ddlAssignmentLimitationCode.SelectedValue
            End If

            If Not String.IsNullOrEmpty(ReturnToDutyDate.Text) Then
                SpecCase.ReturnToDutyDate = Server.HtmlEncode(ReturnToDutyDate.Text)
            End If

            If Not String.IsNullOrEmpty(ExpirationDate.Text) Then
                SpecCase.ExpirationDate = Server.HtmlEncode(ExpirationDate.Text)
            End If
        End Sub

        Private Sub SaveAllSeniorMedicalReviewerAdditionalData()
            SaveIcdData(ucSeniorMedicalReviewerICDCodeControl, txtSeniorMedicalReviewerDiagnosisText, ucSeniorMedicalReviewerICD7thCharacterControl)
            SaveSeniorMedicalReviewerMemoSelection()

            If (SpecCase.SeniorMedicalReviewerApproved.HasValue AndAlso SpecCase.SeniorMedicalReviewerApproved.Value = SC_WWD.DISQUALIFY_FINDING_VALUE) Then
                If (Not String.IsNullOrEmpty(txtSeniorMedicalReviewerDQParagraph.Text)) Then
                    SpecCase.AlternateDQParagraph = HTMLEncodeNulls(txtSeniorMedicalReviewerDQParagraph.Text.Trim())
                End If

                SpecCase.AlternateDQCompletionDate = Now
            End If

            If (SpecCase.SeniorMedicalReviewerApproved.HasValue AndAlso
                (SpecCase.SeniorMedicalReviewerApproved.Value = SC_WWD.QUALIFY_FINDING_VALUE OrElse SpecCase.SeniorMedicalReviewerApproved.Value = SC_WWD.ADMINLOD_FINDING_VALUE)) Then
                SpecCase.AlternateDQParagraph = String.Empty
                SpecCase.AlternateDQCompletionDate = Nothing 'If the case is "Returned" to the Board Medical and decides to change the determination
            End If

            If (Not String.IsNullOrEmpty(ddlSeniorMedicalReviewerAssignmentLimitationCode.SelectedValue)) Then
                SpecCase.AlternateALCLetterType = ddlSeniorMedicalReviewerAssignmentLimitationCode.SelectedValue
            End If

            If (Not String.IsNullOrEmpty(txtSeniorMedicalReviewerReturnToDutyDate.Text)) Then
                SpecCase.AlternateReturnToDutyDate = Server.HtmlEncode(txtSeniorMedicalReviewerReturnToDutyDate.Text)
            End If

            If (Not String.IsNullOrEmpty(txtSeniorMedicalReviewerExpirationDate.Text)) Then
                SpecCase.AlternateExpirationDate = Server.HtmlEncode(txtSeniorMedicalReviewerExpirationDate.Text)
            End If
        End Sub

        Private Sub SaveBoardMedicalData()
            If (IsCaseInBoardMedicalDispositionStep()) Then
                SaveBoardMedicalMemoSelection()
            Else
                SaveAllBoardMedicalData()
            End If

            SpecCaseDao.SaveOrUpdate(SpecCase)
            SpecCaseDao.CommitChanges()
        End Sub

        Private Sub SaveBoardMedicalMemoSelection()
            If (Not String.IsNullOrEmpty(ddlMemos.SelectedValue)) Then
                SpecCase.MemoTemplateID = ddlMemos.SelectedValue
            End If
        End Sub

        Private Sub SaveData()
            If (Not UserCanEdit) Then
                Exit Sub
            End If

            If (SessionInfo.SESSION_GROUP_ID = UserGroups.BoardMedical) Then
                SaveBoardMedicalData()
            End If

            If (SessionInfo.SESSION_GROUP_ID = UserGroups.AFRCHQTechnician) Then
                SaveHqAfrcTechnicianData()
            End If

            If (SessionInfo.SESSION_GROUP_ID = UserGroups.SeniorMedicalReviewer) Then
                SaveSeniorMedicalReviewerData()
            End If
        End Sub

        Private Sub SaveHqAfrcTechnicianData()
            If (IsCaseInHqAfrcTechnicianReviewStep()) Then
                If (SpecCase.ShouldUseSeniorMedicalReviewerFindings()) Then
                    SaveSeniorMedicalReviewerMemoSelection()
                Else
                    SaveBoardMedicalMemoSelection()
                End If
            End If

            If (SpecCase.CurrentStatusCode = SpecCaseWWDStatusCode.FinalReview) Then
                If (SpecCase.ShouldUseSeniorMedicalReviewerFindings()) Then
                    SaveRestrictedSeniorMedicalReviewerFields()
                Else
                    SaveRestrictedBoardMedicalFields()
                End If
            End If

            SpecCaseDao.SaveOrUpdate(SpecCase)
            SpecCaseDao.CommitChanges()
        End Sub

        Private Sub SaveIcdData(icdCodeControl As ICDCodeControl, icdDiagnosisTextBox As TextBox, icd7thCharacterControl As ICD7thCharacterControl)
            If (icdCodeControl.IsICDCodeSelected()) Then
                SpecCase.ICD9Code = icdCodeControl.SelectedICDCodeID
                SpecCase.ICD9Description = icdCodeControl.SelectedICDCodeText
            End If

            SpecCase.ICD9Diagnosis = Server.HtmlEncode(icdDiagnosisTextBox.Text.Trim())

            If (icd7thCharacterControl.Is7thCharacterSelected()) Then
                SpecCase.ICD7thCharacter = icd7thCharacterControl.Selected7thCharacter
            Else
                SpecCase.ICD7thCharacter = Nothing
            End If

            If (Not IsNothing(SpecCase.ICD9Code)) Then
                icdCodeControl.UpdateICDCodeDiagnosisLabel(SpecCase.ICD9Code, True)
            End If
        End Sub

        Private Sub SaveRestrictedBoardMedicalFields()
            If (SpecCase.med_off_approved.HasValue AndAlso SpecCase.med_off_approved.Value = SC_WWD.DISQUALIFY_FINDING_VALUE) Then
                If (Not String.IsNullOrEmpty(ReturnToDutyDate.Text)) Then
                    SpecCase.ReturnToDutyDate = Server.HtmlEncode(ReturnToDutyDate.Text)
                End If

                If (Not String.IsNullOrEmpty(ExpirationDate.Text)) Then
                    SpecCase.ExpirationDate = Server.HtmlEncode(ExpirationDate.Text)
                End If

                If (Not String.IsNullOrEmpty(ddlAssignmentLimitationCode.SelectedValue)) Then
                    SpecCase.ALCLetterType = ddlAssignmentLimitationCode.SelectedValue
                End If
            End If
        End Sub

        Private Sub SaveRestrictedSeniorMedicalReviewerFields()
            If (SpecCase.SeniorMedicalReviewerApproved.HasValue AndAlso SpecCase.SeniorMedicalReviewerApproved.Value = SC_WWD.DISQUALIFY_FINDING_VALUE) Then
                If (Not String.IsNullOrEmpty(txtSeniorMedicalReviewerReturnToDutyDate.Text)) Then
                    SpecCase.AlternateReturnToDutyDate = Server.HtmlEncode(txtSeniorMedicalReviewerReturnToDutyDate.Text)
                End If

                If (Not String.IsNullOrEmpty(txtSeniorMedicalReviewerExpirationDate.Text)) Then
                    SpecCase.AlternateExpirationDate = Server.HtmlEncode(txtSeniorMedicalReviewerExpirationDate.Text)
                End If

                If (Not String.IsNullOrEmpty(ddlSeniorMedicalReviewerAssignmentLimitationCode.SelectedValue)) Then
                    SpecCase.AlternateALCLetterType = ddlSeniorMedicalReviewerAssignmentLimitationCode.SelectedValue
                End If
            End If
        End Sub

        Private Sub SaveSeniorMedicalReviewerData()
            SaveSeniorMedicalReviewerDecision()
            SaveSeniorMedicalReviewerFinding()
            SaveSeniorMedicalReviewerDecisionComment()
            SaveAllSeniorMedicalReviewerAdditionalData()

            SpecCaseDao.SaveOrUpdate(SpecCase)
            SpecCaseDao.CommitChanges()
        End Sub

        Private Sub SaveSeniorMedicalReviewerDecision()
            If (rblSeniorMedicalReviewerDecision.Visible = False OrElse String.IsNullOrEmpty(rblSeniorMedicalReviewerDecision.SelectedValue)) Then
                Exit Sub
            End If

            SpecCase.SeniorMedicalReviewerConcur = Server.HtmlEncode(rblSeniorMedicalReviewerDecision.SelectedValue)
        End Sub

        Private Sub SaveSeniorMedicalReviewerDecisionComment()
            If (txtSeniorMedicalReviewerDecisionComment.Text.Length > txtSeniorMedicalReviewerDecisionComment.MaxLength) Then
                Exit Sub
            End If

            SpecCase.SeniorMedicalReviewerComment = Server.HtmlEncode(txtSeniorMedicalReviewerDecisionComment.Text)
        End Sub

        Private Sub SaveSeniorMedicalReviewerFinding()
            If (rblSeniorMedicalReviewerDecision.Visible = False AndAlso Not String.IsNullOrEmpty(rblSeniorMedicalReviewerFindings.SelectedValue)) Then
                SpecCase.SeniorMedicalReviewerApproved = Integer.Parse(rblSeniorMedicalReviewerFindings.SelectedValue)
            ElseIf (rblSeniorMedicalReviewerDecision.Visible = True AndAlso rblSeniorMedicalReviewerDecision.SelectedValue.Equals(SpecialCase.DECISION_NONCONCUR) AndAlso Not String.IsNullOrEmpty(rblSeniorMedicalReviewerFindings.SelectedValue)) Then
                SpecCase.SeniorMedicalReviewerApproved = Integer.Parse(rblSeniorMedicalReviewerFindings.SelectedValue)
            Else
                SpecCase.SeniorMedicalReviewerApproved = Nothing
            End If
        End Sub

        Private Sub SaveSeniorMedicalReviewerMemoSelection()
            If (Not String.IsNullOrEmpty(ddlSeniorMedicalReviewerMemos.SelectedValue)) Then
                SpecCase.AlternateMemoTemplateID = ddlSeniorMedicalReviewerMemos.SelectedValue
            End If
        End Sub

        Private Sub SetAccessForControls()
            SetReadOnlyAccessForControls()

            If (UserCanEdit) Then
                SetReadWriteAccessForControls()
            End If
        End Sub

        Private Sub SetControlsVisibility()
            SetSeniorMedicalReviewerFindingsPanelVisibility()
        End Sub

        Private Sub SetFindingsPanelIndentation()
            If (rblSeniorMedicalReviewerDecision.Visible) Then
                pnlSeniorMedicalReviewerFindings.CssClass = "FindingsIndent"
            Else
                pnlSeniorMedicalReviewerFindings.CssClass = String.Empty
            End If
        End Sub

        Private Sub SetInputFormatRestrictions()
            SetInputFormatRestriction(Page, DiagnosisTextBox, FormatRestriction.AlphaNumeric, PERMITTED_SPECIAL_CHAR_INPUT)
            SetInputFormatRestriction(Page, DecisionComment, FormatRestriction.AlphaNumeric, PERMITTED_SPECIAL_CHAR_INPUT)
            SetInputFormatRestriction(Page, DQParagraph, FormatRestriction.AlphaNumeric, PERMITTED_SPECIAL_CHAR_INPUT)
            SetInputFormatRestriction(Page, ExpirationDate, FormatRestriction.Numeric, "/")
            SetInputFormatRestriction(Page, ReturnToDutyDate, FormatRestriction.Numeric, "/")

            SetInputFormatRestriction(Page, txtSeniorMedicalReviewerDecisionComment, FormatRestriction.AlphaNumeric, PERMITTED_SPECIAL_CHAR_INPUT)
            SetInputFormatRestriction(Page, txtSeniorMedicalReviewerDiagnosisText, FormatRestriction.AlphaNumeric, PERMITTED_SPECIAL_CHAR_INPUT)
            SetInputFormatRestriction(Page, txtSeniorMedicalReviewerDQParagraph, FormatRestriction.AlphaNumeric, PERMITTED_SPECIAL_CHAR_INPUT)
            SetInputFormatRestriction(Page, txtSeniorMedicalReviewerExpirationDate, FormatRestriction.Numeric, "/")
            SetInputFormatRestriction(Page, txtSeniorMedicalReviewerReturnToDutyDate, FormatRestriction.Numeric, "/")
        End Sub

        Private Sub SetMaxLengthForControls()
            SetMaxLength(DiagnosisTextBox)
            SetMaxLength(DecisionComment)
            SetMaxLength(DQParagraph)

            SetMaxLength(txtSeniorMedicalReviewerDiagnosisText)
            SetMaxLength(txtSeniorMedicalReviewerDecisionComment)
            SetMaxLength(txtSeniorMedicalReviewerDQParagraph)
        End Sub

        Private Sub SetReadOnlyAccessForBoardMedicalOfficerControls()
            DecisionY.Enabled = False
            DecisionN.Enabled = False
            DecisionRFA.Enabled = False
            DecisionAdminLOD.Enabled = False
            DecisionComment.ReadOnly = True
            DQParagraph.ReadOnly = True
            DiagnosisTextBox.ReadOnly = True
            ExpirationDate.Enabled = False
            ReturnToDutyDate.Enabled = False
            ddlMemos.Enabled = False
            ddlAssignmentLimitationCode.Enabled = False

            ucICDCodeControl.DisplayReadOnly(False)
            ucICD7thCharacterControl.DisplayReadOnly()

            ExpirationDate.CssClass = ""
            ReturnToDutyDate.CssClass = ""
        End Sub

        Private Sub SetReadOnlyAccessForControls()
            SetReadOnlyAccessForBoardMedicalOfficerControls()
            SetReadOnlyAccessForSeniorMedicalReviewerControls()
        End Sub

        Private Sub SetReadOnlyAccessForSeniorMedicalReviewerControls()
            ucSeniorMedicalReviewerICDCodeControl.DisplayReadOnly(False)
            ucSeniorMedicalReviewerICD7thCharacterControl.DisplayReadOnly()

            rblSeniorMedicalReviewerDecision.Enabled = False
            rblSeniorMedicalReviewerFindings.Enabled = False
            txtSeniorMedicalReviewerDecisionComment.Enabled = False
            txtSeniorMedicalReviewerDiagnosisText.ReadOnly = True
            txtSeniorMedicalReviewerDiagnosisText.Enabled = False
            txtSeniorMedicalReviewerDQParagraph.ReadOnly = True
            txtSeniorMedicalReviewerDQParagraph.Enabled = False
            txtSeniorMedicalReviewerExpirationDate.Enabled = False
            txtSeniorMedicalReviewerReturnToDutyDate.Enabled = False
            ddlSeniorMedicalReviewerAssignmentLimitationCode.Enabled = False
            ddlSeniorMedicalReviewerMemos.Enabled = False

            txtSeniorMedicalReviewerReturnToDutyDate.CssClass = ""
            txtSeniorMedicalReviewerExpirationDate.CssClass = ""
        End Sub

        Private Sub SetReadWriteAccessForBoardMedicalOfficer()
            If (IsCaseInBoardMedicalDispositionStep()) Then
                If (SpecCase.med_off_approved.HasValue AndAlso SpecCase.med_off_approved.Value = 0) Then
                    SetRestrictedReadWriteAccessForBoardMedicalControls()
                End If
            Else
                SetReadWriteAccessForBoardMedicalReviewStepControls()
            End If
        End Sub

        Private Sub SetReadWriteAccessForBoardMedicalReviewStepControls()
            ucICDCodeControl.DisplayReadWrite(False)
            ucICD7thCharacterControl.DisplayReadWrite()
            DecisionY.Enabled = True
            DecisionN.Enabled = True
            DecisionRFA.Enabled = True
            DecisionAdminLOD.Enabled = True
            DecisionComment.ReadOnly = False
            DecisionComment.Enabled = True
            DQParagraph.ReadOnly = False
            DQParagraph.Enabled = True
            DiagnosisTextBox.ReadOnly = False
            DiagnosisTextBox.Enabled = True
            ReturnToDutyDate.Enabled = True
            ExpirationDate.Enabled = True
            ddlMemos.Enabled = True
            ddlAssignmentLimitationCode.Enabled = True

            ExpirationDate.CssClass = "datePickerPlusFuture"
            ReturnToDutyDate.CssClass = "datePickerFuture"
        End Sub

        Private Sub SetReadWriteAccessForControls()
            If (SessionInfo.SESSION_GROUP_ID = UserGroups.BoardMedical) Then
                SetReadWriteAccessForBoardMedicalOfficer()
            End If

            If (SessionInfo.SESSION_GROUP_ID = UserGroups.AFRCHQTechnician) Then
                SetReadWriteAccessForHqAfrcTechnician()
            End If

            If (SessionInfo.SESSION_GROUP_ID = UserGroups.SeniorMedicalReviewer) Then
                SetReadWriteAccessForSeniorMedicalReviewer()
            End If
        End Sub

        Private Sub SetReadWriteAccessForHqAfrcTechnician()
            If (Not IsCaseInHqAfrcTechnicianReviewStep()) Then
                Exit Sub
            End If

            If (SpecCase.ShouldUseSeniorMedicalReviewerFindings()) Then
                If (SpecCase.SeniorMedicalReviewerApproved.HasValue AndAlso SpecCase.SeniorMedicalReviewerApproved.Value = 0) Then
                    SetRestrictedReadWriteAccessForSeniorMedicalReviewerControls()
                End If
            Else
                If (SpecCase.med_off_approved.HasValue AndAlso SpecCase.med_off_approved.Value = 0) Then
                    SetRestrictedReadWriteAccessForBoardMedicalControls()
                End If
            End If
        End Sub

        Private Sub SetReadWriteAccessForSeniorMedicalReviewer()
            If (IsCaseInSeniorMedicalReviewerDispositionStep()) Then
                If (SpecCase.med_off_approved.HasValue AndAlso SpecCase.med_off_approved.Value = 0) Then
                    SetRestrictedReadWriteAccessForSeniorMedicalReviewerControls()
                End If
            Else
                SetReadWriteAccessForSeniorMedicalReviewerReviewStepControls()
            End If
        End Sub

        Private Sub SetReadWriteAccessForSeniorMedicalReviewerReviewStepControls()
            ucSeniorMedicalReviewerICDCodeControl.DisplayReadWrite(False)
            ucSeniorMedicalReviewerICD7thCharacterControl.DisplayReadWrite()

            rblSeniorMedicalReviewerDecision.Enabled = True
            rblSeniorMedicalReviewerFindings.Enabled = True
            txtSeniorMedicalReviewerDecisionComment.Enabled = True
            txtSeniorMedicalReviewerDiagnosisText.ReadOnly = False
            txtSeniorMedicalReviewerDiagnosisText.Enabled = True
            txtSeniorMedicalReviewerDQParagraph.ReadOnly = False
            txtSeniorMedicalReviewerDQParagraph.Enabled = True
            txtSeniorMedicalReviewerExpirationDate.Enabled = True
            txtSeniorMedicalReviewerReturnToDutyDate.Enabled = True
            ddlSeniorMedicalReviewerAssignmentLimitationCode.Enabled = True
            ddlSeniorMedicalReviewerMemos.Enabled = True

            txtSeniorMedicalReviewerExpirationDate.CssClass = "datePickerPlusFuture"
            txtSeniorMedicalReviewerReturnToDutyDate.CssClass = "datePickerFuture"
        End Sub

        Private Sub SetRestrictedReadWriteAccessForBoardMedicalControls()
            ExpirationDate.Enabled = True
            ReturnToDutyDate.Enabled = True
            ddlAssignmentLimitationCode.Enabled = True
            ddlMemos.Enabled = True

            ExpirationDate.CssClass = "datePickerPlusFuture"
            ReturnToDutyDate.CssClass = "datePickerFuture"
        End Sub

        Private Sub SetRestrictedReadWriteAccessForSeniorMedicalReviewerControls()
            txtSeniorMedicalReviewerExpirationDate.Enabled = True
            txtSeniorMedicalReviewerReturnToDutyDate.Enabled = True
            ddlSeniorMedicalReviewerAssignmentLimitationCode.Enabled = True
            ddlSeniorMedicalReviewerMemos.Enabled = True
            rblSeniorMedicalReviewerDecision.Enabled = True
            rblSeniorMedicalReviewerFindings.Enabled = True
            txtSeniorMedicalReviewerDecisionComment.Enabled = True

            txtSeniorMedicalReviewerExpirationDate.CssClass = "datePickerPlusFuture"
            txtSeniorMedicalReviewerReturnToDutyDate.CssClass = "datePickerFuture"
        End Sub

        Private Sub SetSeniorMedicalReviewerFindingsPanelVisibility()
            rblSeniorMedicalReviewerDecision.Visible = True

            If (String.IsNullOrEmpty(rblSeniorMedicalReviewerDecision.SelectedValue) OrElse Not rblSeniorMedicalReviewerDecision.SelectedValue.Equals(SpecialCase.DECISION_NONCONCUR)) Then
                pnlSeniorMedicalReviewerFindings.Visible = False
                pnlSeniorMedicalReviewerAdditionalData.Visible = False
            Else
                pnlSeniorMedicalReviewerFindings.Visible = True
                pnlSeniorMedicalReviewerAdditionalData.Visible = True
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

#End Region

    End Class

End Namespace