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

Namespace Web.Special_Case.MEB

    Partial Class Secure_sc_meb_MedOff
        Inherits System.Web.UI.Page

#Region "Fields"

        Private _daoFactory As IDaoFactory
        Private _scAccess As Dictionary(Of String, PageAccessType)
        Private _specCase As SC_MEB
        Private _specCaseDao As ISpecialCaseDAO

#End Region

#Region "Properties"

        Public ReadOnly Property Navigator() As TabNavigator
            Get
                Return Master.Navigator
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

        Protected ReadOnly Property SectionList() As Dictionary(Of String, PageAccessType)
            Get
                If (_scAccess Is Nothing) Then
                    _scAccess = SpecCase.ReadSectionList(CInt(Session("GroupId")))
                End If
                Return _scAccess
            End Get
        End Property

        Protected ReadOnly Property SpecCase() As SC_MEB
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

            If Not CheckTextLength(txtDiagnosis) Then
                IsValid = False
            End If
            If Not CheckTextLength(txtDecisionComment) Then
                IsValid = False
            End If

            If Not CheckTextLength(txtSeniorMedicalReviewerDiagnosis) Then
                IsValid = False
            End If
            If Not CheckTextLength(txtSeniorMedicalReviewerDecisionComment) Then
                IsValid = False
            End If

            Return IsValid
        End Function

        Protected Sub ddlSelectMemos_DataBound(ByVal sender As Object, ByVal e As System.EventArgs) Handles ddlMemos.DataBound, ddlSeniorMedicalReviewerMemos.DataBound
            ddlMemos.Items.Insert(0, New ListItem("-- Select a Memo --", String.Empty))
        End Sub

        Protected Sub Page_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
            AddHandler Me.Master.TabClick, AddressOf TabButtonClicked
        End Sub

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
            InitializeUserControls()

            If (Not IsPostBack) Then
                UserCanEdit = GetAccess(Navigator.PageAccess, True)
                InitControls()

                If (UserCanEdit) Then
                    SpecCase.Validate()
                    If (SpecCase.Validations.Count > 0) Then
                        ShowPageValidationErrors(SpecCase.Validations, Me)
                    End If
                End If

                LogManager.LogAction(ModuleType.SpecCaseMEB, UserAction.ViewPage, RefId, "Viewed Page: Board Med")
            End If
        End Sub

        Protected Sub rblSeniorMedicalReviewerConcur_SelectedIndexChanged(sender As Object, e As EventArgs) Handles rblSeniorMedicalReviewerConcur.SelectedIndexChanged
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

        Private Sub LoadBoardMedicalFindings()
            If (SpecCase.MedOffConcur.HasValue) Then
                rblDecision.SelectedValue = SpecCase.MedOffConcur
            End If

            If (Not String.IsNullOrEmpty(SpecCase.med_off_approval_comment)) Then
                txtDecisionComment.Text = Server.HtmlDecode(SpecCase.med_off_approval_comment)
            End If

            If (SpecCase.med_off_approved.HasValue) Then
                rblDetermination.SelectedValue = SpecCase.med_off_approved
            End If

            If Not IsNothing(SpecCase.ALCLetterType) Then
                ddlAssignmentLimitation.SelectedValue = SpecCase.ALCLetterType
            End If

            If SpecCase.MemoTemplateID.HasValue Then
                ddlMemos.SelectedValue = SpecCase.MemoTemplateID
            End If

            LoadIcdControls(ucICDCodeControl, txtDiagnosis, ucICD7thCharacterControl)

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
                rblSeniorMedicalReviewerConcur.SelectedValue = SpecCase.SeniorMedicalReviewerConcur
            End If

            If (SpecCase.SeniorMedicalReviewerApproved.HasValue) Then
                rblSeniorMedicalReviewerFindings.SelectedValue = SpecCase.SeniorMedicalReviewerApproved
            End If

            If (Not String.IsNullOrEmpty(SpecCase.SeniorMedicalReviewerComment)) Then
                txtSeniorMedicalReviewerDecisionComment.Text = SpecCase.SeniorMedicalReviewerComment
            End If

            If (SpecCase.AlternateMedOffConcur.HasValue) Then
                rblSeniorMedicalReviewerDecision.SelectedValue = SpecCase.AlternateMedOffConcur
            End If

            If (SpecCase.AlternateALCLetterType.HasValue) Then
                ddlSeniorMedicalReviewerAssignmentLimitation.SelectedValue = SpecCase.AlternateALCLetterType
            End If

            If SpecCase.AlternateMemoTemplateId.HasValue Then
                ddlSeniorMedicalReviewerMemos.SelectedValue = SpecCase.AlternateMemoTemplateId
            End If

            LoadIcdControls(ucSeniorMedicalReviewerICDCodeControl, txtSeniorMedicalReviewerDiagnosis, ucSeniorMedicalReviewerICD7thCharacterControl)

        End Sub

        Private Sub SaveBoardMedicalData()

            If ((SpecCase.Status = SpecCaseMEBWorkStatus.MedicalReviewAFPC OrElse SpecCase.Status = SpecCaseMEBWorkStatus.MedicalReviewIPEB OrElse SpecCase.Status = SpecCaseMEBWorkStatus.MedicalReviewFPEB OrElse SpecCase.Status = SpecCaseMEBWorkStatus.MedicalReviewTransitionPhase) And SESSION_GROUP_ID = UserGroups.BoardMedical) Then

                If Not String.IsNullOrEmpty(ddlMemos.SelectedValue) Then
                    SpecCase.MemoTemplateID = ddlMemos.SelectedValue
                End If
            ElseIf (SpecCase.Status = SpecCaseMEBWorkStatus.MedicalReview) Then

                If (Not String.IsNullOrEmpty(rblDecision.SelectedValue)) Then
                    SpecCase.MedOffConcur = rblDecision.SelectedValue
                End If

                SpecCase.med_off_approval_comment = Server.HtmlEncode(txtDecisionComment.Text)
                SaveIcdData(ucICDCodeControl, txtDiagnosis, ucICD7thCharacterControl)

                If (Not String.IsNullOrEmpty(rblDetermination.SelectedValue)) Then
                    SpecCase.med_off_approved = rblDetermination.SelectedValue
                End If

                If ddlAssignmentLimitation.SelectedValue > 0 Then
                    SpecCase.ALCLetterType = ddlAssignmentLimitation.SelectedValue
                End If

                If Not String.IsNullOrEmpty(ddlMemos.SelectedValue) Then
                    SpecCase.MemoTemplateID = ddlMemos.SelectedValue
                End If

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

            If ((SpecCase.Status = SpecCaseMEBWorkStatus.SeniorMedicalReviewAFPC OrElse SpecCase.Status = SpecCaseMEBWorkStatus.SeniorMedicalReviewIPEB OrElse SpecCase.Status = SpecCaseMEBWorkStatus.SeniorMedicalReviewFPEB OrElse SpecCase.Status = SpecCaseMEBWorkStatus.SeniorMedicalReviewTransitionPhase) And SESSION_GROUP_ID = UserGroups.SeniorMedicalReviewer) Then

                If (rblSeniorMedicalReviewerConcur.Visible = True AndAlso rblSeniorMedicalReviewerConcur.SelectedValue.Equals(SpecialCase.DECISION_NONCONCUR) AndAlso Not String.IsNullOrEmpty(ddlSeniorMedicalReviewerMemos.SelectedValue)) Then
                    SpecCase.AlternateMemoTemplateId = ddlSeniorMedicalReviewerMemos.SelectedValue
                End If
            ElseIf (SpecCase.Status = SpecCaseMEBWorkStatus.SeniorMedicalReview) Then

                If (rblSeniorMedicalReviewerConcur.Visible = True AndAlso rblSeniorMedicalReviewerConcur.SelectedValue.Equals(SpecialCase.DECISION_NONCONCUR)) Then

                    If (Not String.IsNullOrEmpty(rblSeniorMedicalReviewerDecision.SelectedValue)) Then
                        SpecCase.AlternateMedOffConcur = rblSeniorMedicalReviewerDecision.SelectedValue
                    End If

                    SaveIcdData(ucSeniorMedicalReviewerICDCodeControl, txtSeniorMedicalReviewerDiagnosis, ucSeniorMedicalReviewerICD7thCharacterControl)

                    If ddlSeniorMedicalReviewerAssignmentLimitation.SelectedValue > 0 Then
                        SpecCase.AlternateALCLetterType = ddlSeniorMedicalReviewerAssignmentLimitation.SelectedValue
                    End If

                    If Not String.IsNullOrEmpty(ddlSeniorMedicalReviewerMemos.SelectedValue) Then
                        SpecCase.AlternateMemoTemplateId = ddlSeniorMedicalReviewerMemos.SelectedValue
                    End If

                End If

            End If

            SpecCaseDao.SaveOrUpdate(SpecCase)
            SpecCaseDao.CommitChanges()
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
            If (rblSeniorMedicalReviewerConcur.Visible = False OrElse String.IsNullOrEmpty(rblSeniorMedicalReviewerConcur.SelectedValue)) Then
                Exit Sub
            End If

            SpecCase.SeniorMedicalReviewerConcur = Server.HtmlEncode(rblSeniorMedicalReviewerConcur.SelectedValue)
        End Sub

        Private Sub SaveSeniorMedicalReviewerDecisionComment()
            If (txtSeniorMedicalReviewerDecisionComment.Text.Length > txtSeniorMedicalReviewerDecisionComment.MaxLength) Then
                Exit Sub
            End If

            SpecCase.SeniorMedicalReviewerComment = Server.HtmlEncode(txtSeniorMedicalReviewerDecisionComment.Text)
        End Sub

        Private Sub SaveSeniorMedicalReviewerFinding()
            If (rblSeniorMedicalReviewerConcur.Visible = False AndAlso Not String.IsNullOrEmpty(rblSeniorMedicalReviewerFindings.SelectedValue)) Then
                SpecCase.SeniorMedicalReviewerApproved = Integer.Parse(rblSeniorMedicalReviewerFindings.SelectedValue)
            ElseIf (rblSeniorMedicalReviewerConcur.Visible = True AndAlso rblSeniorMedicalReviewerConcur.SelectedValue.Equals(SpecialCase.DECISION_NONCONCUR) AndAlso Not String.IsNullOrEmpty(rblSeniorMedicalReviewerFindings.SelectedValue)) Then
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
            If (rblSeniorMedicalReviewerConcur.Visible) Then
                pnlSeniorMedicalReviewerFindings.CssClass = "FindingsIndent"
            Else
                pnlSeniorMedicalReviewerFindings.CssClass = String.Empty
            End If
        End Sub

        Private Sub SetInputFormatRestrictions()
            SetInputFormatRestriction(Page, txtDecisionComment, FormatRestriction.AlphaNumeric, PERMITTED_SPECIAL_CHAR_INPUT)
            SetInputFormatRestriction(Page, txtDiagnosis, FormatRestriction.AlphaNumeric, PERMITTED_SPECIAL_CHAR_INPUT)

            SetInputFormatRestriction(Page, txtSeniorMedicalReviewerDecisionComment, FormatRestriction.AlphaNumeric, PERMITTED_SPECIAL_CHAR_INPUT)
            SetInputFormatRestriction(Page, txtSeniorMedicalReviewerDiagnosis, FormatRestriction.AlphaNumeric, PERMITTED_SPECIAL_CHAR_INPUT)
        End Sub

        Private Sub SetMaxLengthForControls()
            SetMaxLength(txtDiagnosis)
            SetMaxLength(txtDecisionComment)

            SetMaxLength(txtSeniorMedicalReviewerDiagnosis)
            SetMaxLength(txtSeniorMedicalReviewerDecisionComment)
        End Sub

        Private Sub SetReadOnlyAccessForBoardMedicalOfficerControls()
            rblDecision.Enabled = False
            txtDecisionComment.ReadOnly = True
            ucICDCodeControl.DisplayReadOnly(False)
            ucICD7thCharacterControl.DisplayReadOnly()
            txtDiagnosis.ReadOnly = True
            rblDetermination.Enabled = False
            ddlAssignmentLimitation.Enabled = False
            ddlMemos.Enabled = False
        End Sub

        Private Sub SetReadOnlyAccessForControls()
            SetReadOnlyAccessForBoardMedicalOfficerControls()
            SetReadOnlyAccessForSeniorMedicalReviewerControls()
        End Sub

        Private Sub SetReadOnlyAccessForSeniorMedicalReviewerControls()
            rblSeniorMedicalReviewerDecision.Enabled = False
            rblSeniorMedicalReviewerFindings.Enabled = False
            txtSeniorMedicalReviewerDecisionComment.Enabled = False
            ucSeniorMedicalReviewerICDCodeControl.DisplayReadOnly(False)
            ucSeniorMedicalReviewerICD7thCharacterControl.DisplayReadOnly()
            txtSeniorMedicalReviewerDiagnosis.ReadOnly = True
            rblSeniorMedicalReviewerConcur.Enabled = False
            ddlSeniorMedicalReviewerAssignmentLimitation.Enabled = False
            ddlSeniorMedicalReviewerMemos.Enabled = False
        End Sub

        Private Sub SetReadWriteAccessForBoardMedicalOfficerControls()

            If (SpecCase.CurrentStatusCode = SpecCaseMEBStatusCode.MedicalReview) Then
                rblDecision.Enabled = True
                txtDecisionComment.ReadOnly = False
                ucICDCodeControl.DisplayReadWrite(False)
                ucICD7thCharacterControl.DisplayReadWrite()
                txtDiagnosis.ReadOnly = False
                rblDetermination.Enabled = True
                ddlAssignmentLimitation.Enabled = True
            End If

            ddlMemos.Enabled = True

        End Sub

        Private Sub SetReadWriteAccessForControls()
            If (SESSION_GROUP_ID = UserGroups.BoardMedical) Then
                SetReadWriteAccessForBoardMedicalOfficerControls()
            End If

            If (SESSION_GROUP_ID = UserGroups.SeniorMedicalReviewer) Then
                SetReadWriteAccessForSeniorMedicalReviewerControls()
            End If
        End Sub

        Private Sub SetReadWriteAccessForSeniorMedicalReviewerControls()
            If (SpecCase.CurrentStatusCode = SpecCaseMEBStatusCode.SeniorMedicalReview) Then
                rblSeniorMedicalReviewerConcur.Enabled = True
                rblSeniorMedicalReviewerDecision.Enabled = True
                rblSeniorMedicalReviewerFindings.Enabled = True
                txtSeniorMedicalReviewerDecisionComment.Enabled = True
                ucSeniorMedicalReviewerICDCodeControl.DisplayReadWrite(False)
                ucSeniorMedicalReviewerICD7thCharacterControl.DisplayReadWrite()
                txtSeniorMedicalReviewerDiagnosis.ReadOnly = False
                ddlSeniorMedicalReviewerAssignmentLimitation.Enabled = True
            End If

            ddlSeniorMedicalReviewerMemos.Enabled = True

        End Sub

        Private Sub SetSeniorMedicalReviewerFindingsPanelVisibility()
            rblSeniorMedicalReviewerConcur.Visible = True

            If (String.IsNullOrEmpty(rblSeniorMedicalReviewerConcur.SelectedValue) OrElse Not rblSeniorMedicalReviewerConcur.SelectedValue.Equals(SpecialCase.DECISION_NONCONCUR)) Then
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