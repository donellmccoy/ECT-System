Imports ALOD.Core.Domain.Common
Imports ALOD.Core.Domain.Documents
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

Namespace Web.Special_Case.AGR

    Partial Class Secure_sc_agr_MedOff
        Inherits System.Web.UI.Page

#Region "Fields"

        Private _daoFactory As IDaoFactory
        Private _memoSource As MemoDao2
        Private _scAccess As Dictionary(Of String, ALOD.Core.Domain.Workflow.PageAccessType)
        Private _specCase As SC_AGRCert = Nothing
        Private _specCaseDao As ISpecialCaseDAO

#End Region

#Region "Properties"

        ReadOnly Property ModuleType() As ModuleType
            Get
                Return ModuleType.SpecCaseWWD
            End Get
        End Property

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

        Protected ReadOnly Property MasterPage() As SC_AGRCertMaster
            Get
                Dim master As SC_AGRCertMaster = CType(Page.Master, SC_AGRCertMaster)
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

        Protected ReadOnly Property SpecCase() As SC_AGRCert
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

        Private ReadOnly Property MemoStore() As MemoDao2
            Get
                If (_memoSource Is Nothing) Then
                    _memoSource = DaoFactory.GetMemoDao2()
                End If
                Return _memoSource
            End Get
        End Property

#End Region

#Region "Page Methods"

        Public Function ValidBoxLength() As Boolean
            Dim IsValid As Boolean = True
            If Not CheckTextLength(DecisionComment) Then
                IsValid = False
            End If
            Return IsValid
        End Function

        Protected Sub Page_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
            AddHandler Me.Master.TabClick, AddressOf TabButtonClicked
        End Sub

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

            If (Not IsPostBack) Then
                UserCanEdit = GetAccess(Navigator.PageAccess, True)
                InitControls()
                LogManager.LogAction(ModuleType.SpecCaseAGR, UserAction.ViewPage, RefId, "Viewed Page: AGR Certification Medical Officer Review")
            End If
        End Sub

        Protected Sub rblSeniorMedicalReviewerDecision_SelectedIndexChanged(sender As Object, e As EventArgs) Handles rblSeniorMedicalReviewerDecision.SelectedIndexChanged
            If (SpecCase.hqt_approval1.HasValue And SpecCase.hqt_approval1 = 1) Then
                rblSeniorMedicalReviewerFindings.SelectedValue = "0"
            Else
                rblSeniorMedicalReviewerFindings.SelectedValue = "1"
            End If
            SetSeniorMedicalReviewerFindingsPanelVisibility()
        End Sub

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

        Private Sub InitControls()
            SetInputFormatRestrictions()
            SetAccessForControls()
            LoadFindings()
            SetMaxLengthForControls()
            SetPanelVisibility()
            SetControlsVisibility()

            'SetFindingsPanelIndentation()
        End Sub

        Private Sub LoadBoardMedicalFindings()
            If (SpecCase.hqt_approval1.HasValue) Then
                If (SpecCase.hqt_approval1.Value = 1) Then
                    DecisionBMedY.Checked = True
                End If

                If (SpecCase.hqt_approval1.Value = 0) Then
                    DecisionBMedN.Checked = True
                End If

                If (SpecCase.hqt_approval1.Value = 3) Then
                    DecisionRFABMed.Checked = True
                End If
            End If

            If (Not String.IsNullOrEmpty(SpecCase.hqt_approval1_comment)) Then
                DecisionCommentBMed.Text = Server.HtmlDecode(SpecCase.hqt_approval1_comment)
            End If

            If (SpecCase.AlternateApprovalDate.HasValue) Then
                AGRAltDateTextBox.Text = Server.HtmlDecode(SpecCase.AlternateApprovalDate.Value.ToString(DATE_FORMAT))
            End If
        End Sub

        Private Sub LoadFindings()
            LoadMedicalOfficerFindings()
            LoadBoardMedicalFindings()
            LoadSeniorMedicalReviewerFindings()
        End Sub

        Private Sub LoadMedicalOfficerFindings()
            If (SpecCase.med_off_approved.HasValue) Then
                If (SpecCase.med_off_approved.Value = 1) Then
                    DecisionY.Checked = True
                End If

                If (SpecCase.med_off_approved.Value = 0) Then
                    DecisionN.Checked = True
                End If

                If (SpecCase.med_off_approved = 3) Then
                    DecisionRFA.Checked = True
                End If
            End If

            If (Not String.IsNullOrEmpty(SpecCase.med_off_approval_comment)) Then
                DecisionComment.Text = Server.HtmlDecode(SpecCase.med_off_approval_comment)
            End If

            If (SpecCase.ApprovalDate.HasValue) Then
                AGRDateTextBox.Text = Server.HtmlDecode(SpecCase.ApprovalDate.Value.ToString(DATE_FORMAT))
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

            If (SpecCase.AlternateExpirationDate.HasValue) Then
                txtSeniorMedicalReviewerRenewalDate.Text = Server.HtmlDecode(SpecCase.AlternateExpirationDate.Value.ToString(DATE_FORMAT))
            End If
        End Sub

        Private Sub SaveBoardMedicalData()

            'get the memo templates
            Dim templates = From m In MemoStore.GetAllTemplates()
                            Where m.Component = Session("Compo") _
                            And (m.ModuleType = ModuleType)
                            Select m

            If (DecisionBMedN.Checked) Then
                SpecCase.hqt_approval1 = 0
                SpecCase.hqt_approval1_comment = Server.HtmlEncode(DecisionCommentBMed.Text)
                SpecCase.AlternateApprovalDate = Nothing

                If (templates.Any() And templates.FirstOrDefault().Title = "AGR Certiication Denied") Then
                    SpecCase.MemoTemplateID = MemoType.AGR_Certiication_Denied
                End If
            End If

            If (DecisionBMedY.Checked) Then
                SpecCase.hqt_approval1 = 1
                SpecCase.hqt_approval1_comment = Server.HtmlEncode(DecisionCommentBMed.Text)

                Dim dateValue As DateTime

                If (DateTime.TryParse(AGRAltDateTextBox.Text.Trim, dateValue)) Then
                    SpecCase.AlternateApprovalDate = Server.HtmlEncode(dateValue)
                End If

                ' Board Memo
                If ((SpecCase.ALC.HasValue And SpecCase.ALC = 1 Or SpecCase.MAJCOM.HasValue And SpecCase.MAJCOM = 1)) Then
                    'Initial Tour
                    If SpecCase.InitialTour.HasValue And SpecCase.InitialTour = 1 Then
                        If (templates.Any() And templates.FirstOrDefault().Title = "AGR Certiication Approved-HQ-Intial") Then
                            SpecCase.MemoTemplateID = MemoType.AGR_Approved_HQ_INIT
                        End If

                    End If
                    'FollowOn Tour
                    If SpecCase.FollowOnTour.HasValue And SpecCase.FollowOnTour = 1 Then
                        If (templates.Any() And templates.FirstOrDefault().Title = "AGR Certiication Approved-HQ-FON") Then
                            SpecCase.MemoTemplateID = MemoType.AGR_Approved_HQ_FON
                        End If

                    End If
                End If
            End If

            If (DecisionRFABMed.Checked) Then
                SpecCase.hqt_approval1 = SC_AGRCert.RFA_FINDING_VALUE
                SpecCase.hqt_approval1_comment = " "
                SpecCase.AlternateApprovalDate = Nothing
            End If

            SpecCaseDao.SaveOrUpdate(SpecCase)
            SpecCaseDao.CommitChanges()
        End Sub

        Private Sub SaveData()
            'If (Not UserCanEdit) Then
            '    Exit Sub
            'End If

            If (SessionInfo.SESSION_GROUP_ID = UserGroups.BoardMedical) Then
                SaveBoardMedicalData()
            End If

            If (SessionInfo.SESSION_GROUP_ID = UserGroups.MedicalOfficer) Then
                SaveMedicalOfficerData()
            End If

            If (SessionInfo.SESSION_GROUP_ID = UserGroups.SeniorMedicalReviewer) Then
                SaveSeniorMedicalReviewerData()
            End If
        End Sub

        'Private Sub SetFindingsPanelIndentation()
        '    If (rblSeniorMedicalReviewerDecision.Visible) Then
        '        pnlMedicalOfficerReviewerFindings.CssClass = "FindingsIndent"
        '    Else
        '        pnlSeniorMedicalReviewerFindings.CssClass = String.Empty
        '    End If
        'End Sub
        Private Sub SaveMedicalOfficerData()
            If (DecisionN.Checked) Then
                SpecCase.med_off_approved = 0
                SpecCase.ApprovalDate = Nothing
            End If

            If (DecisionY.Checked) Then
                SpecCase.med_off_approved = 1

                Dim dateValue As DateTime

                If (DateTime.TryParse(AGRDateTextBox.Text.Trim, dateValue)) Then
                    SpecCase.ApprovalDate = Server.HtmlEncode(dateValue)
                End If
            End If

            If (DecisionRFA.Checked) Then
                SpecCase.med_off_approved = SC_AGRCert.RFA_FINDING_VALUE
                SpecCase.ApprovalDate = Nothing
            End If

            SpecCase.med_off_approval_comment = Server.HtmlEncode(DecisionComment.Text)

            SpecCaseDao.SaveOrUpdate(SpecCase)
            SpecCaseDao.CommitChanges()
        End Sub

        Private Sub SaveSeniorMedicalReviewerData()
            SaveSeniorMedicalReviewerDecision()
            SaveSeniorMedicalReviewerFinding()
            SaveSeniorMedicalReviewerDecisionComment()
            SaveSeniorMedicalReviewerDecisionApprovalDate()

            SpecCaseDao.SaveOrUpdate(SpecCase)
            SpecCaseDao.CommitChanges()
        End Sub

        Private Sub SaveSeniorMedicalReviewerDecision()
            If (rblSeniorMedicalReviewerDecision.Visible = False OrElse String.IsNullOrEmpty(rblSeniorMedicalReviewerDecision.SelectedValue)) Then
                Exit Sub
            End If

            SpecCase.SeniorMedicalReviewerConcur = Server.HtmlEncode(rblSeniorMedicalReviewerDecision.SelectedValue)
        End Sub

        Private Sub SaveSeniorMedicalReviewerDecisionApprovalDate()
            Dim dateValue As DateTime
            If (DateTime.TryParse(txtSeniorMedicalReviewerRenewalDate.Text.Trim, dateValue)) Then
                SpecCase.AlternateExpirationDate = Server.HtmlEncode(dateValue)
            End If
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

        Private Sub SetAccessForControls()
            SetReadOnlyAccessForControls()

            If (UserCanEdit) Then
                SetReadWriteAccessForControls()
            End If
        End Sub

        Private Sub SetBoardMedicalOfficerReviewerFindingsPanelVisibility()
            pnlBoardMedialReviewerInformation.Visible = False
            If (SpecCase.hqt_approval1.HasValue) Then
                pnlBoardMedialReviewerInformation.Visible = True
            End If
            If (UserCanEdit) Then
                DecisionBMedY.Enabled = True
                DecisionBMedN.Enabled = True
                DecisionRFABMed.Enabled = True
                DecisionCommentBMed.Enabled = True
            End If
        End Sub

        Private Sub SetControlsVisibility()
            SetMedicalOfficerReviewerFindingsPanelVisibility()
            SetBoardMedicalOfficerReviewerFindingsPanelVisibility()
            SetSeniorMedicalReviewerFindingsPanelVisibility()
        End Sub

        Private Sub SetInputFormatRestrictions()
            SetInputFormatRestriction(Page, DecisionComment, FormatRestriction.AlphaNumeric, PERMITTED_SPECIAL_CHAR_INPUT)
            SetInputFormatRestriction(Page, DecisionCommentBMed, FormatRestriction.AlphaNumeric, PERMITTED_SPECIAL_CHAR_INPUT)
            SetInputFormatRestriction(Page, AGRDateTextBox, FormatRestriction.Numeric, "/")
            SetInputFormatRestriction(Page, AGRAltDateTextBox, FormatRestriction.Numeric, "/")
            SetInputFormatRestriction(Page, txtSeniorMedicalReviewerDecisionComment, FormatRestriction.AlphaNumeric, PERMITTED_SPECIAL_CHAR_INPUT)
            SetInputFormatRestriction(Page, txtSeniorMedicalReviewerRenewalDate, FormatRestriction.Numeric, "/")
        End Sub

        Private Sub SetMaxLengthForControls()
            SetMaxLength(DecisionComment)
            SetMaxLength(DecisionCommentBMed)
            SetMaxLength(txtSeniorMedicalReviewerDecisionComment)
        End Sub

        Private Sub SetMedicalOfficerReviewerFindingsPanelVisibility()
            pnlMedicalOfficerInformation.Visible = False
            If (SpecCase.med_off_approved.HasValue) Then
                pnlMedicalOfficerInformation.Visible = True
            End If
        End Sub

        Private Sub SetPanelVisabilityMedTechLocalFinalReview()

            If (SessionInfo.SESSION_GROUP_ID = UserGroups.MedicalTechnician And SpecCase.Status = SpecCaseAGRWorkStatus.LocalFinalReview) Then
                pnlMedicalOfficerInformation.Visible = True
                pnlBoardMedialReviewerInformation.Visible = False
                pnlSeniorMedialReviewerInformation.Visible = False
                If SpecCase.med_off_approved IsNot Nothing And SpecCase.med_off_approved = 1 Then
                    DecisionY.Checked = True
                End If
                If SpecCase.med_off_approved IsNot Nothing And SpecCase.med_off_approved = 0 Then
                    DecisionN.Checked = True
                End If
                DecisionY.Enabled = False
                DecisionN.Enabled = False
                DecisionRFA.Enabled = False
                DecisionComment.Enabled = False
                AGRDateTextBox.Visible = True
                AGRDateTextBox.ReadOnly = True
                AGRDateLabel.Visible = True
            End If
        End Sub

        Private Sub SetPanelVisibility()

            pnlMedicalOfficerInformation.Visible = False
            pnlBoardMedialReviewerInformation.Visible = False
            pnlSeniorMedialReviewerInformation.Visible = False

            DecisionY.Enabled = False
            DecisionN.Enabled = False
            DecisionRFA.Enabled = False
            DecisionComment.Enabled = False
            AGRDateTextBox.Enabled = False
            AGRDateLabel.Enabled = False
            AGRDateLabel.Visible = False

            DecisionBMedY.Enabled = False
            DecisionBMedN.Enabled = False
            DecisionRFABMed.Enabled = False
            DecisionCommentBMed.Enabled = False
            AGRAltDateTextBox.Visible = False
            AGRAltDateLabel.Visible = False

            rblSeniorMedicalReviewerDecision.Enabled = False
            txtSeniorMedicalReviewerDecisionComment.Enabled = False
            txtSeniorMedicalReviewerRenewalDate.Visible = False

            ' Wing flow -------------------------------------------------------------------->>

            ' Local Medical Technician Close out (Final Review)
            SetPanelVisabilityMedTechLocalFinalReview()

            ' Local Medical Technician Close out (Approved, Denied)
            SetPanelVisibilityMedTechApprovedDenied()

            ' Local Medical Officer wing
            SetPanelVisibilityLocalMedOfficerReview()

            ' Board flow -------------------------------------------------------------------->>

            '' HQ Medical Technician Initiate
            SetPanelVisibilityHqInitiateCase()

            '' HQ Medical Technician Package Review
            SetPanelVisibilityHqPackageReview()

            ' Medical Officer HQ
            SetPanelVisibilityHqMedOfficer()

            ' Board Medical HQ
            SetPanelVisibilityHqBoardMedical()

            ' Senior Medical Officer HQ
            SetPanelVisibilityHqSeniorMedOfficer()

            ' HQ Medical Technician Close out (Final Review)
            SetPanelVisibilityHqMedTechFinalReview()

            ' HQ Medical Technician Close out  (Approved, Denied)
            SetPanelVisibilityHqMedTechApproveDenied()

        End Sub

        Private Sub SetPanelVisibilityHqBoardMedical()

            If ((SessionInfo.SESSION_GROUP_ID = UserGroups.BoardMedical Or
                 SessionInfo.SESSION_GROUP_ID = UserGroups.SeniorMedicalReviewer Or SessionInfo.SESSION_GROUP_ID = UserGroups.AFRCHQTechnician) _
                And SpecCase.Status = SpecCaseAGRWorkStatus.MedicalReview) Then
                pnlMedicalOfficerInformation.Visible = True
                pnlBoardMedialReviewerInformation.Visible = True
                pnlSeniorMedialReviewerInformation.Visible = True
                DecisionY.Enabled = False
                DecisionN.Enabled = False

                DecisionRFA.Visible = True
                DecisionComment.Enabled = False
                AGRDateTextBox.Visible = True
                AGRDateTextBox.ReadOnly = True
                AGRDateLabel.Visible = True

                DecisionBMedY.Enabled = False
                DecisionBMedN.Enabled = False
                DecisionRFABMed.Enabled = False
                DecisionCommentBMed.Enabled = False

                AGRAltDateTextBox.Visible = True
                AGRAltDateLabel.Visible = True
                AGRAltDateTextBox.Enabled = False

                If (SessionInfo.SESSION_GROUP_ID = UserGroups.BoardMedical) Then
                    DecisionBMedY.Enabled = True
                    DecisionBMedN.Enabled = True
                    DecisionRFABMed.Enabled = True
                    DecisionCommentBMed.Enabled = True

                    AGRAltDateTextBox.Enabled = True
                    AGRAltDateTextBox.Visible = True
                    AGRAltDateLabel.Visible = True
                    AGRAltDateTextBox.ReadOnly = False
                    AGRAltDateTextBox.CssClass = "datePicker"
                End If

                AGRAlt2DateLabel.Visible = False
                txtSeniorMedicalReviewerRenewalDate.Visible = True
                txtSeniorMedicalReviewerRenewalDate.Enabled = False

                If (SessionInfo.SESSION_GROUP_ID = UserGroups.SeniorMedicalReviewer) Then
                    AGRAlt2DateLabel.Visible = True
                    txtSeniorMedicalReviewerRenewalDate.Visible = True
                    txtSeniorMedicalReviewerRenewalDate.Enabled = True
                End If

            End If
        End Sub

        Private Sub SetPanelVisibilityHqInitiateCase()

            If (SessionInfo.SESSION_GROUP_ID = UserGroups.AFRCHQTechnician And SpecCase.Status = SpecCaseAGRWorkStatus.HqInitiateCase) Then
                pnlMedicalOfficerInformation.Visible = True
                pnlBoardMedialReviewerInformation.Visible = True
                pnlSeniorMedialReviewerInformation.Visible = True

                AGRDateTextBox.Visible = True
                AGRDateTextBox.ReadOnly = True
                AGRDateLabel.Visible = True

                DecisionY.Enabled = False
                DecisionN.Enabled = False
                DecisionRFA.Enabled = False
                DecisionComment.Enabled = False

                AGRAltDateTextBox.Visible = True
                AGRAltDateTextBox.ReadOnly = True
                AGRAltDateLabel.Visible = True
                DecisionRFABMed.Enabled = False

                AGRAlt2DateLabel.Visible = True
                txtSeniorMedicalReviewerRenewalDate.Visible = True
                txtSeniorMedicalReviewerRenewalDate.ReadOnly = True
            End If
        End Sub

        Private Sub SetPanelVisibilityHqMedOfficer()

            If ((SessionInfo.SESSION_GROUP_ID = UserGroups.MedicalOfficer Or SessionInfo.SESSION_GROUP_ID = UserGroups.AFRCHQTechnician Or
                 SessionInfo.SESSION_GROUP_ID = UserGroups.BoardMedical Or
                 SessionInfo.SESSION_GROUP_ID = UserGroups.SeniorMedicalReviewer) _
                And SpecCase.Status = SpecCaseAGRWorkStatus.MedicalReview) Then
                pnlMedicalOfficerInformation.Visible = True
                pnlBoardMedialReviewerInformation.Visible = False
                pnlSeniorMedialReviewerInformation.Visible = False
                DecisionY.Enabled = True
                DecisionN.Enabled = True
                DecisionRFA.Enabled = True
                DecisionComment.Enabled = True

                AGRDateTextBox.Visible = True
                AGRDateLabel.Visible = True
                AGRDateTextBox.CssClass = "datePicker"

            End If
        End Sub

        Private Sub SetPanelVisibilityHqMedTechApproveDenied()

            If ((SessionInfo.SESSION_GROUP_ID = UserGroups.BoardMedical Or
                SessionInfo.SESSION_GROUP_ID = UserGroups.SeniorMedicalReviewer Or
                SessionInfo.SESSION_GROUP_ID = UserGroups.AFRCHQTechnician) _
                And (SpecCase.Status = SpecCaseAGRWorkStatus.Approved Or SpecCase.Status = SpecCaseAGRWorkStatus.Denied)) Then
                pnlMedicalOfficerInformation.Visible = True
                pnlBoardMedialReviewerInformation.Visible = True
                pnlSeniorMedialReviewerInformation.Visible = True
                If SpecCase.med_off_approved IsNot Nothing And SpecCase.med_off_approved = 1 Then
                    DecisionY.Checked = True
                End If
                If SpecCase.med_off_approved IsNot Nothing And SpecCase.med_off_approved = 0 Then
                    DecisionN.Checked = True
                End If
                DecisionY.Enabled = False
                DecisionN.Enabled = False
                DecisionRFA.Enabled = False
                DecisionComment.Enabled = False
                AGRDateTextBox.Visible = False
                AGRDateTextBox.ReadOnly = True
                AGRDateLabel.Visible = False

                AGRAltDateTextBox.Visible = True
                AGRAltDateTextBox.ReadOnly = True
                AGRAltDateLabel.Visible = True
                DecisionRFABMed.Enabled = False

                AGRAlt2DateLabel.Visible = True
                txtSeniorMedicalReviewerRenewalDate.Visible = True
                txtSeniorMedicalReviewerRenewalDate.ReadOnly = True
            End If
        End Sub

        Private Sub SetPanelVisibilityHqMedTechFinalReview()

            If ((SessionInfo.SESSION_GROUP_ID = UserGroups.BoardMedical Or
                SessionInfo.SESSION_GROUP_ID = UserGroups.SeniorMedicalReviewer Or
                SessionInfo.SESSION_GROUP_ID = UserGroups.AFRCHQTechnician) And SpecCase.Status = SpecCaseAGRWorkStatus.FinalReview) Then
                pnlMedicalOfficerInformation.Visible = True
                pnlBoardMedialReviewerInformation.Visible = True
                pnlSeniorMedialReviewerInformation.Visible = True

                'If SpecCase.med_off_approved IsNot Nothing And SpecCase.med_off_approved = 1 Then
                '    AGRDateTextBox.Visible = True
                '    AGRDateTextBox.ReadOnly = True
                '    AGRDateLabel.Visible = True
                'End If

                AGRDateTextBox.Visible = True
                AGRDateTextBox.ReadOnly = True
                AGRDateLabel.Visible = True

                DecisionY.Enabled = False
                DecisionN.Enabled = False
                DecisionRFA.Enabled = False
                DecisionComment.Enabled = False

                DecisionRFABMed.Enabled = False

                'If SpecCase.hqt_approval1 IsNot Nothing And SpecCase.hqt_approval1 = 1 Then
                '    AGRAltDateTextBox.Visible = True
                '    AGRAltDateTextBox.ReadOnly = True
                '    AGRAltDateLabel.Visible = True
                'End If
                AGRAltDateTextBox.Visible = True
                AGRAltDateTextBox.ReadOnly = True
                AGRAltDateLabel.Visible = True

                'If SpecCase.SeniorMedicalReviewerApproved IsNot Nothing And SpecCase.SeniorMedicalReviewerApproved = 1 Then
                '    AGRAlt2DateLabel.Visible = True
                '    txtSeniorMedicalReviewerRenewalDate.Visible = True
                '    txtSeniorMedicalReviewerRenewalDate.ReadOnly = True
                'End If

                AGRAlt2DateLabel.Visible = True
                txtSeniorMedicalReviewerRenewalDate.Visible = True
                txtSeniorMedicalReviewerRenewalDate.ReadOnly = True
            End If
        End Sub

        Private Sub SetPanelVisibilityHqPackageReview()

            If ((SessionInfo.SESSION_GROUP_ID = UserGroups.AFRCHQTechnician Or SessionInfo.SESSION_GROUP_ID = UserGroups.BoardMedical Or
                SessionInfo.SESSION_GROUP_ID = UserGroups.SeniorMedicalReviewer) And SpecCase.Status = SpecCaseAGRWorkStatus.PackageReview) Then
                pnlMedicalOfficerInformation.Visible = True
                pnlBoardMedialReviewerInformation.Visible = True
                pnlSeniorMedialReviewerInformation.Visible = True
                If SpecCase.med_off_approved IsNot Nothing And SpecCase.med_off_approved = 1 Then
                    DecisionY.Checked = True
                End If
                If SpecCase.med_off_approved IsNot Nothing And SpecCase.med_off_approved = 0 Then
                    DecisionN.Checked = True
                End If
                DecisionY.Enabled = False
                DecisionN.Enabled = False
                DecisionRFA.Enabled = False
                DecisionRFA.Visible = True
                DecisionComment.Enabled = False
                AGRDateLabel.Enabled = False
                AGRDateLabel.Visible = False

                AGRDateTextBox.Visible = True
                AGRDateTextBox.ReadOnly = True
                AGRDateLabel.Visible = True
                AGRDateLabel.Enabled = True

                AGRAltDateTextBox.Visible = True
                AGRAltDateLabel.Visible = True
                AGRAltDateTextBox.ReadOnly = True
                DecisionRFABMed.Enabled = False

                AGRAlt2DateLabel.Visible = True
                txtSeniorMedicalReviewerRenewalDate.Visible = True
                txtSeniorMedicalReviewerRenewalDate.ReadOnly = True

            End If
        End Sub

        Private Sub SetPanelVisibilityHqSeniorMedOfficer()

            If ((SessionInfo.SESSION_GROUP_ID = UserGroups.BoardMedical Or
                 SessionInfo.SESSION_GROUP_ID = UserGroups.SeniorMedicalReviewer Or SessionInfo.SESSION_GROUP_ID = UserGroups.AFRCHQTechnician) _
                And SpecCase.Status = SpecCaseAGRWorkStatus.SeniorMedicalReview) Then
                pnlMedicalOfficerInformation.Visible = True
                pnlBoardMedialReviewerInformation.Visible = True
                pnlSeniorMedialReviewerInformation.Visible = True

                DecisionBMedY.Enabled = False
                DecisionBMedN.Enabled = False
                DecisionRFABMed.Enabled = False
                DecisionRFABMed.Visible = True

                DecisionCommentBMed.Enabled = False
                AGRAltDateLabel.Visible = True
                AGRAltDateTextBox.ReadOnly = True
                AGRAltDateTextBox.Visible = True

                rblSeniorMedicalReviewerDecision.Enabled = False
                pnlSeniorMedicalReviewerFindings.Enabled = False
                rblSeniorMedicalReviewerFindings.Enabled = False

                txtSeniorMedicalReviewerDecisionComment.Enabled = False

                AGRAlt2DateLabel.Visible = True
                txtSeniorMedicalReviewerRenewalDate.Visible = True
                txtSeniorMedicalReviewerRenewalDate.Enabled = False

                If (SessionInfo.SESSION_GROUP_ID = UserGroups.SeniorMedicalReviewer) Then

                    rblSeniorMedicalReviewerDecision.Enabled = True
                    pnlSeniorMedicalReviewerFindings.Enabled = True
                    rblSeniorMedicalReviewerFindings.Enabled = True

                    txtSeniorMedicalReviewerDecisionComment.Enabled = True

                    AGRAlt2DateLabel.Visible = True
                    txtSeniorMedicalReviewerRenewalDate.Visible = True
                    txtSeniorMedicalReviewerRenewalDate.Enabled = True
                    txtSeniorMedicalReviewerRenewalDate.CssClass = "datePicker"
                End If

            End If
        End Sub

        Private Sub SetPanelVisibilityLocalMedOfficerReview()

            If (SessionInfo.SESSION_GROUP_ID = UserGroups.MedicalOfficer And (SpecCase.Status = SpecCaseAGRWorkStatus.MedicalOfficerReview _
                                                                              Or SpecCase.Status = SpecCaseAGRWorkStatus.LocalFinalReview)) Then
                pnlMedicalOfficerInformation.Visible = True
                pnlBoardMedialReviewerInformation.Visible = False
                pnlSeniorMedialReviewerInformation.Visible = False

                DecisionY.Enabled = True
                DecisionN.Enabled = True
                DecisionRFA.Enabled = True
                DecisionComment.Enabled = True
                AGRDateTextBox.Enabled = True
                AGRDateTextBox.Visible = True
                AGRDateLabel.Visible = True
                AGRDateTextBox.CssClass = "datePicker"
            End If
        End Sub

        Private Sub SetPanelVisibilityMedTechApprovedDenied()

            If (SessionInfo.SESSION_GROUP_ID = UserGroups.MedicalTechnician _
                And (SpecCase.Status = SpecCaseAGRWorkStatus.Approved Or SpecCase.Status = SpecCaseAGRWorkStatus.Denied)) Then
                pnlMedicalOfficerInformation.Visible = True
                pnlBoardMedialReviewerInformation.Visible = False
                pnlSeniorMedialReviewerInformation.Visible = False
                DecisionY.Enabled = False
                DecisionN.Enabled = False
                DecisionRFA.Enabled = False
                DecisionComment.Enabled = False
                AGRDateTextBox.Visible = True
                AGRDateTextBox.ReadOnly = True
                AGRDateLabel.Visible = True
            End If
        End Sub

        Private Sub SetReadOnlyAccessForBoardMedicalOfficerControls()
            DecisionBMedY.Enabled = False
            DecisionBMedN.Enabled = False
            DecisionCommentBMed.Enabled = False
            AGRAltDateTextBox.Visible = False
            AGRAltDateLabel.Visible = False
        End Sub

        Private Sub SetReadOnlyAccessForControls()
            SetReadOnlyAccessForBoardMedicalOfficerControls()
            SetReadOnlyAccessForMedicalOfficerControls()
            SetReadOnlyAccessForSeniorMedicalReviewerControls()
            'rblSeniorMedicalReviewerDecision.Enabled = False
            'rblSeniorMedicalReviewerFindings.Enabled = False
            'txtSeniorMedicalReviewerDecisionComment.Enabled = False
        End Sub

        Private Sub SetReadOnlyAccessForMedicalOfficerControls()
            DecisionY.Enabled = False
            DecisionN.Enabled = False
            DecisionComment.Enabled = False
            AGRDateTextBox.Visible = False
            AGRDateLabel.Visible = False
        End Sub

        Private Sub SetReadOnlyAccessForSeniorMedicalReviewerControls()
            rblSeniorMedicalReviewerDecision.Enabled = False
            rblSeniorMedicalReviewerFindings.Enabled = False
            txtSeniorMedicalReviewerDecisionComment.Enabled = False
            txtSeniorMedicalReviewerRenewalDate.Enabled = False
        End Sub

        Private Sub SetReadWriteAccessForBoardMedicalOfficerControls()
            DecisionBMedY.Enabled = True
            DecisionBMedN.Enabled = True
            DecisionCommentBMed.Enabled = True
            AGRAltDateLabel.Visible = False
            AGRAltDateTextBox.Visible = True

            AGRAltDateTextBox.CssClass = "datePicker"
        End Sub

        Private Sub SetReadWriteAccessForControls()
            If (SessionInfo.SESSION_GROUP_ID = UserGroups.BoardMedical) Then
                SetReadWriteAccessForBoardMedicalOfficerControls()
            End If

            If (SessionInfo.SESSION_GROUP_ID = UserGroups.MedicalOfficer) Then
                SetReadWriteAccessForMedicalOfficerControls()
            End If

            If (SessionInfo.SESSION_GROUP_ID = UserGroups.SeniorMedicalReviewer) Then
                SetReadWriteAccessForSeniorMedicalReviewerControls()
            End If
        End Sub

        Private Sub SetReadWriteAccessForMedicalOfficerControls()
            DecisionY.Enabled = True
            DecisionN.Enabled = True
            DecisionComment.Enabled = True
            AGRDateLabel.Visible = False
            AGRDateTextBox.Visible = True

            AGRDateTextBox.CssClass = "datePicker"
            'pnlMedicalOfficerInformation.Visible = True
        End Sub

        Private Sub SetReadWriteAccessForSeniorMedicalReviewerControls()
            rblSeniorMedicalReviewerDecision.Enabled = True
            rblSeniorMedicalReviewerFindings.Enabled = True
            txtSeniorMedicalReviewerDecisionComment.Enabled = True
            txtSeniorMedicalReviewerRenewalDate.Enabled = True

            txtSeniorMedicalReviewerRenewalDate.CssClass = "datePickerPlusFuture"
        End Sub

        Private Sub SetSeniorMedicalReviewerFindingsPanelVisibility()
            rblSeniorMedicalReviewerDecision.Visible = True

            If (String.IsNullOrEmpty(rblSeniorMedicalReviewerDecision.SelectedValue) OrElse Not rblSeniorMedicalReviewerDecision.SelectedValue.Equals(SpecialCase.DECISION_NONCONCUR)) Then
                pnlSeniorMedicalReviewerFindings.Visible = False
            Else
                pnlSeniorMedicalReviewerFindings.Visible = True
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