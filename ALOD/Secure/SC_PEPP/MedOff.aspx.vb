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

Namespace Web.Special_Case.PEPP

    Partial Class Secure_sc_pepp_MedOff
        Inherits System.Web.UI.Page

#Region "Fields"

        Private _daoFactory As IDaoFactory
        Private _scAccess As Dictionary(Of String, ALOD.Core.Domain.Workflow.PageAccessType)
        Private _specCase As SC_PEPP = Nothing
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

        Protected ReadOnly Property MasterPage() As SC_PEPPMaster
            Get
                Dim master As SC_PEPPMaster = CType(Page.Master, SC_PEPPMaster)
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

        Protected ReadOnly Property SpecCase() As SC_PEPP
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

        Protected Sub Decision_CheckedChanged(sender As Object, e As EventArgs) Handles DecisionY.CheckedChanged, DecisionN.CheckedChanged
            If (DecisionY.Checked = True) Then
                ExpirationDate.Enabled = True
                ExpirationDate.CssClass = "datePickerPlusFuture"
            Else
                ExpirationDate.Enabled = False
                ExpirationDate.CssClass = String.Empty
            End If
        End Sub

        Protected Sub Page_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
            AddHandler Me.Master.TabClick, AddressOf TabButtonClicked
        End Sub

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
            InitializeUserControls()

            If (Not IsPostBack) Then
                UserCanEdit = GetAccess(Navigator.PageAccess, True)
                InitControls()
                LogManager.LogAction(ModuleType.SpecCasePEPP, UserAction.ViewPage, RefId, "Viewed Page: Board Med")
            End If
        End Sub

        Protected Sub rblSeniorMedicalReviewerDecision_SelectedIndexChanged(sender As Object, e As EventArgs) Handles rblSeniorMedicalReviewerDecision.SelectedIndexChanged
            SetSeniorMedicalReviewerFindingsPanelVisibility()
        End Sub

        Protected Sub rblSeniorMedicalReviewerFindings_SelectedIndexChanged(sender As Object, e As EventArgs) Handles rblSeniorMedicalReviewerFindings.SelectedIndexChanged
            If (rblSeniorMedicalReviewerFindings.SelectedValue.Equals("1")) Then
                txtSeniorMedicalReviewerExpirationDate.Enabled = True
                txtSeniorMedicalReviewerExpirationDate.CssClass = "datePickerPlusFuture"
            Else
                txtSeniorMedicalReviewerExpirationDate.Enabled = False
                txtSeniorMedicalReviewerExpirationDate.CssClass = String.Empty
            End If
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

        Private Sub LoadBoardMedicalFindings()
            If (SpecCase.med_off_approved.HasValue) Then
                If (SpecCase.med_off_approved.Value = 1) Then
                    DecisionY.Checked = True
                End If

                If (SpecCase.med_off_approved.Value = 0) Then
                    DecisionN.Checked = True
                End If
            End If

            If (Not String.IsNullOrEmpty(SpecCase.med_off_approval_comment)) Then
                DecisionComment.Text = Server.HtmlDecode(SpecCase.med_off_approval_comment)
            End If

            If (Not IsNothing(SpecCase.DQParagraph)) Then
                DQParagraph.Text = Server.HtmlDecode(SpecCase.DQParagraph.ToString())
            End If

            If (SpecCase.ExpirationDate.HasValue) Then
                ExpirationDate.Text = Server.HtmlDecode(SpecCase.ExpirationDate.Value.ToString(DATE_FORMAT))
            End If

            If (SpecCase.CertificationDate.HasValue) Then
                txtCertificationDate.Text = Server.HtmlDecode(SpecCase.CertificationDate.Value.ToString(DATE_FORMAT))
            End If

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

            If (SpecCase.AlternateCertificationDate.HasValue) Then
                txtSeniorMedicalReviewerCertificationDate.Text = Server.HtmlDecode(SpecCase.AlternateCertificationDate.Value.ToString(DATE_FORMAT))
            End If

            LoadIcdControls(ucSeniorMedicalReviewerICDCodeControl, txtSeniorMedicalReviewerDiagnosisText, ucSeniorMedicalReviewerICD7thCharacterControl)
        End Sub

        Private Sub SaveBoardMedicalData()
            SaveIcdData(ucICDCodeControl, DiagnosisTextBox, ucICD7thCharacterControl)

            If (DecisionN.Checked) Then
                SpecCase.med_off_approved = 0
                SpecCase.ExpirationDate = Nothing

                If (Not String.IsNullOrEmpty(DQParagraph.Text)) Then
                    SpecCase.DQParagraph = HTMLEncodeNulls(DQParagraph.Text.Trim())
                End If
            End If

            If (DecisionY.Checked) Then
                SpecCase.med_off_approved = 1
                SpecCase.DQParagraph = String.Empty

                If Not String.IsNullOrEmpty(ExpirationDate.Text) Then
                    SpecCase.ExpirationDate = Server.HtmlEncode(ExpirationDate.Text)
                End If
            End If

            SpecCase.med_off_approval_comment = Server.HtmlEncode(DecisionComment.Text)

            If (Not String.IsNullOrEmpty(txtCertificationDate.Text)) Then
                SpecCase.CertificationDate = Server.HtmlEncode(txtCertificationDate.Text)
            End If

            SpecCaseDao.SaveOrUpdate(SpecCase)
            SpecCaseDao.CommitChanges()
        End Sub

        Private Sub SaveData()
            If (Not UserCanEdit) Then
                Exit Sub
            End If

            If (SessionInfo.SESSION_GROUP_ID = UserGroups.BoardMedical) Then
                SaveBoardMedicalData()
            End If

            If (SessionInfo.SESSION_GROUP_ID = UserGroups.SeniorMedicalReviewer) Then
                SaveSeniorMedicalReviewerData()
            End If
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

        Private Sub SaveSeniorMedicalReviewerAdditionalData()
            SaveIcdData(ucSeniorMedicalReviewerICDCodeControl, txtSeniorMedicalReviewerDiagnosisText, ucSeniorMedicalReviewerICD7thCharacterControl)

            If (SpecCase.SeniorMedicalReviewerApproved.HasValue AndAlso SpecCase.SeniorMedicalReviewerApproved.Value = SC_PEPP.DISQUALIFY_FINDING_VALUE) Then
                SpecCase.AlternateExpirationDate = Nothing

                If (Not String.IsNullOrEmpty(txtSeniorMedicalReviewerDQParagraph.Text)) Then
                    SpecCase.AlternateDQParagraph = HTMLEncodeNulls(txtSeniorMedicalReviewerDQParagraph.Text.Trim())
                End If
            End If

            If (SpecCase.SeniorMedicalReviewerApproved.HasValue AndAlso SpecCase.SeniorMedicalReviewerApproved.Value = SC_PEPP.QUALIFY_FINDING_VALUE) Then
                SpecCase.AlternateDQParagraph = String.Empty

                If (Not String.IsNullOrEmpty(txtSeniorMedicalReviewerExpirationDate.Text)) Then
                    SpecCase.AlternateExpirationDate = Server.HtmlEncode(txtSeniorMedicalReviewerExpirationDate.Text)
                End If
            End If

            If (Not String.IsNullOrEmpty(txtSeniorMedicalReviewerCertificationDate.Text)) Then
                SpecCase.AlternateCertificationDate = Server.HtmlEncode(txtSeniorMedicalReviewerCertificationDate.Text)
            End If
        End Sub

        Private Sub SaveSeniorMedicalReviewerData()
            SaveSeniorMedicalReviewerDecision()
            SaveSeniorMedicalReviewerFinding()
            SaveSeniorMedicalReviewerDecisionComment()
            SaveSeniorMedicalReviewerAdditionalData()

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
            SetInputFormatRestriction(Page, DecisionComment, FormatRestriction.AlphaNumeric, PERMITTED_SPECIAL_CHAR_INPUT)
            SetInputFormatRestriction(Page, DiagnosisTextBox, FormatRestriction.AlphaNumeric, PERMITTED_SPECIAL_CHAR_INPUT)
            SetInputFormatRestriction(Page, DQParagraph, FormatRestriction.AlphaNumeric, PERMITTED_SPECIAL_CHAR_INPUT)
            SetInputFormatRestriction(Page, ExpirationDate, FormatRestriction.Numeric, "/")
            SetInputFormatRestriction(Page, txtCertificationDate, FormatRestriction.Numeric, "/")

            SetInputFormatRestriction(Page, txtSeniorMedicalReviewerDecisionComment, FormatRestriction.AlphaNumeric, PERMITTED_SPECIAL_CHAR_INPUT)
            SetInputFormatRestriction(Page, txtSeniorMedicalReviewerDiagnosisText, FormatRestriction.AlphaNumeric, PERMITTED_SPECIAL_CHAR_INPUT)
            SetInputFormatRestriction(Page, txtSeniorMedicalReviewerDQParagraph, FormatRestriction.AlphaNumeric, PERMITTED_SPECIAL_CHAR_INPUT)
            SetInputFormatRestriction(Page, txtSeniorMedicalReviewerExpirationDate, FormatRestriction.Numeric, "/")
            SetInputFormatRestriction(Page, txtSeniorMedicalReviewerCertificationDate, FormatRestriction.Numeric, "/")
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
            DecisionComment.Enabled = False

            DQParagraph.Enabled = False
            DiagnosisTextBox.ReadOnly = True
            ExpirationDate.Enabled = False
            txtCertificationDate.Enabled = False

            ucICDCodeControl.DisplayReadOnly(False)
            ucICD7thCharacterControl.DisplayReadOnly()

            txtCertificationDate.CssClass = ""
            ExpirationDate.CssClass = ""
        End Sub

        Private Sub SetReadOnlyAccessForControls()
            SetReadOnlyAccessForBoardMedicalOfficerControls()
            SetReadOnlyAccessForSeniorMedicalReviewerControls()
        End Sub

        Private Sub SetReadOnlyAccessForSeniorMedicalReviewerControls()
            rblSeniorMedicalReviewerDecision.Enabled = False
            rblSeniorMedicalReviewerFindings.Enabled = False
            txtSeniorMedicalReviewerDecisionComment.Enabled = False

            txtSeniorMedicalReviewerDQParagraph.Enabled = False
            txtSeniorMedicalReviewerExpirationDate.Enabled = False
            txtSeniorMedicalReviewerCertificationDate.Enabled = False

            ucSeniorMedicalReviewerICDCodeControl.DisplayReadOnly(False)
            ucSeniorMedicalReviewerICD7thCharacterControl.DisplayReadOnly()

            txtSeniorMedicalReviewerCertificationDate.CssClass = ""
            txtSeniorMedicalReviewerExpirationDate.CssClass = ""
        End Sub

        Private Sub SetReadWriteAccessForBoardMedicalOfficerControls()
            DecisionY.Enabled = True
            DecisionN.Enabled = True
            DecisionComment.Enabled = True
            DQParagraph.Enabled = True
            txtCertificationDate.Enabled = True

            If (SpecCase.med_off_approved.HasValue AndAlso SpecCase.med_off_approved.Value = 1) Then
                ExpirationDate.Enabled = True
            Else
                ExpirationDate.Enabled = False
                ExpirationDate.CssClass = String.Empty
            End If

            If (SpecCase.IsWaiverRequired.HasValue AndAlso SpecCase.IsWaiverRequired.Value = True) Then
                ucICDCodeControl.DisplayReadWrite(False)
                ucICD7thCharacterControl.DisplayReadWrite()
                DiagnosisTextBox.ReadOnly = False
                DiagnosisTextBox.Enabled = True
            Else
                ucICDCodeControl.DisplayReadOnly(False)
                ucICD7thCharacterControl.DisplayReadOnly()
                DiagnosisTextBox.ReadOnly = True
            End If

            ExpirationDate.CssClass = "datePickerPlusFuture"
            txtCertificationDate.CssClass = "datePickerAll"
        End Sub

        Private Sub SetReadWriteAccessForControls()
            If (SessionInfo.SESSION_GROUP_ID = UserGroups.BoardMedical) Then
                SetReadWriteAccessForBoardMedicalOfficerControls()
            End If

            If (SessionInfo.SESSION_GROUP_ID = UserGroups.SeniorMedicalReviewer) Then
                SetReadWriteAccessForSeniorMedicalReviewerControls()
            End If
        End Sub

        Private Sub SetReadWriteAccessForSeniorMedicalReviewerControls()
            rblSeniorMedicalReviewerDecision.Enabled = True
            rblSeniorMedicalReviewerFindings.Enabled = True
            txtSeniorMedicalReviewerDecisionComment.Enabled = True
            txtSeniorMedicalReviewerDQParagraph.Enabled = True
            txtSeniorMedicalReviewerCertificationDate.Enabled = True

            If (SpecCase.SeniorMedicalReviewerApproved.HasValue AndAlso SpecCase.SeniorMedicalReviewerApproved.Value = 1) Then
                txtSeniorMedicalReviewerExpirationDate.Enabled = True
            Else
                txtSeniorMedicalReviewerExpirationDate.Enabled = False
                txtSeniorMedicalReviewerExpirationDate.CssClass = String.Empty
            End If

            If (SpecCase.IsWaiverRequired.HasValue AndAlso SpecCase.IsWaiverRequired.Value = True) Then
                ucSeniorMedicalReviewerICDCodeControl.DisplayReadWrite(False)
                ucSeniorMedicalReviewerICD7thCharacterControl.DisplayReadWrite()
                txtSeniorMedicalReviewerDiagnosisText.ReadOnly = False
                txtSeniorMedicalReviewerDiagnosisText.Enabled = True
            Else
                ucSeniorMedicalReviewerICDCodeControl.DisplayReadOnly(False)
                ucSeniorMedicalReviewerICD7thCharacterControl.DisplayReadOnly()
                txtSeniorMedicalReviewerDiagnosisText.ReadOnly = True
            End If

            txtSeniorMedicalReviewerExpirationDate.CssClass = "datePickerPlusFuture"
            txtSeniorMedicalReviewerCertificationDate.CssClass = "datePickerAll"
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