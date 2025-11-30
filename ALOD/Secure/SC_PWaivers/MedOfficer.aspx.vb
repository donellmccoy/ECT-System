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

Namespace Web.Special_Case.PW

    Partial Class Secure_sc_pw_MedOff
        Inherits System.Web.UI.Page

#Region "Fields"

        Private _daoFactory As IDaoFactory
        Private _scAccess As Dictionary(Of String, ALOD.Core.Domain.Workflow.PageAccessType)
        Private _specCase As SC_PWaivers = Nothing
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

        Protected ReadOnly Property MasterPage() As SC_PWaiverMaster
            Get
                Dim master As SC_PWaiverMaster = CType(Page.Master, SC_PWaiverMaster)
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

        Protected ReadOnly Property SpecCase() As SC_PWaivers
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
                LogManager.LogAction(ModuleType.SpecCasePW, UserAction.ViewPage, RefId, "Viewed Page: Participation Waiver Medical Officer Review")
            End If
        End Sub

        Protected Sub rblSeniorMedicalReviewerDecisionY_CheckedChanged(sender As Object, e As EventArgs) Handles rblSeniorMedicalReviewerDecisionY.CheckedChanged
            SetSeniorMedicalReviewerFindingsPanelVisibility()
        End Sub

        Protected Sub rblSeniorMedicalReviewerDecisionN_CheckedChanged(sender As Object, e As EventArgs) Handles rblSeniorMedicalReviewerDecisionN.CheckedChanged
            SetSeniorMedicalReviewerFindingsPanelVisibility()
        End Sub

        Protected Sub rblSeniorMedicalReviewerDecisionR_CheckedChanged(sender As Object, e As EventArgs) Handles rblSeniorMedicalReviewerDecisionR.CheckedChanged
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
            SetControlsVisibility()
            SetFindingsPanelIndentation()
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

            If (SpecCase.ApprovalDate.HasValue) Then
                PWaiverDateTextBox.Text = Server.HtmlDecode(SpecCase.ApprovalDate.Value.ToString(DATE_FORMAT))

                If (SpecCase.ExpirationDate.HasValue AndAlso (SpecCase.Status = SpecCasePWWorkStatus.FinalReview OrElse SpecCase.Status = SpecCasePWWorkStatus.Approved)) Then
                    PWaiverDateLabel.Text = String.Format(": {0} - Expiration Date : {1}", SpecCase.ApprovalDate.Value.ToString(DATE_FORMAT), SpecCase.ExpirationDate.Value.ToString(DATE_FORMAT))
                Else
                    PWaiverDateLabel.Text = String.Format(": {0}", SpecCase.ApprovalDate.Value.ToString(DATE_FORMAT))
                End If
            End If

            If (SpecCase.ExpirationDate.HasValue) Then
                PWaiverExpirationDateTextBox.Text = Server.HtmlDecode(SpecCase.ExpirationDate.Value.ToString(DATE_FORMAT))
            End If
        End Sub

        Private Sub LoadFindings()
            LoadBoardMedicalFindings()
            LoadSeniorMedicalReviewerFindings()
        End Sub

        Private Sub LoadSeniorMedicalReviewerFindings()
            If (Not String.IsNullOrEmpty(SpecCase.SeniorMedicalReviewerConcur)) Then
                Dim selectedValue As String = Server.HtmlDecode(SpecCase.SeniorMedicalReviewerConcur)
                rblSeniorMedicalReviewerDecisionY.Checked = (selectedValue = "Y")
                rblSeniorMedicalReviewerDecisionN.Checked = (selectedValue = "N")
                rblSeniorMedicalReviewerDecisionR.Checked = (selectedValue = "R")
            End If

            If (SpecCase.SeniorMedicalReviewerApproved.HasValue) Then
                rblSeniorMedicalReviewerFindings.SelectedValue = SpecCase.SeniorMedicalReviewerApproved.Value
            End If

            If (Not String.IsNullOrEmpty(SpecCase.SeniorMedicalReviewerComment)) Then
                txtSeniorMedicalReviewerDecisionComment.Text = Server.HtmlDecode(SpecCase.SeniorMedicalReviewerComment)
            End If

            If (SpecCase.AlternateApprovalDate.HasValue) Then
                txtSeniorMedicalReviewerPWaiverDate.Text = Server.HtmlDecode(SpecCase.AlternateApprovalDate.Value.ToString(DATE_FORMAT))

                If (SpecCase.AlternateExpirationDate.HasValue AndAlso (SpecCase.Status = SpecCasePWWorkStatus.FinalReview OrElse SpecCase.Status = SpecCasePWWorkStatus.Approved)) Then
                    lblSeniorMedicalReviewerPWaiverDate.Text = String.Format(": {0} - Expiration Date : {1}", SpecCase.AlternateApprovalDate.Value.ToString(DATE_FORMAT), SpecCase.AlternateExpirationDate.Value.ToString(DATE_FORMAT))
                Else
                    lblSeniorMedicalReviewerPWaiverDate.Text = String.Format(": {0}", SpecCase.AlternateApprovalDate.Value.ToString(DATE_FORMAT))
                End If
            End If

            If (SpecCase.AlternateExpirationDate.HasValue) Then
                txtSeniorMedicalReviewerExpirationDate.Text = Server.HtmlDecode(SpecCase.AlternateExpirationDate.Value.ToString(DATE_FORMAT))
            End If
        End Sub

        Private Sub SaveBoardMedicalData()
            If (DecisionN.Checked) Then
                SpecCase.med_off_approved = 0
                SpecCase.ApprovalDate = Nothing
                SpecCase.ExpirationDate = Nothing
                SpecCase.PWaiverLength = Nothing
            End If

            If (DecisionY.Checked) Then
                SpecCase.med_off_approved = 1

                Dim dateValue As DateTime
                Dim expireDateValue As DateTime

                If (DateTime.TryParse(PWaiverDateTextBox.Text.Trim, dateValue)) And (DateTime.TryParse(PWaiverExpirationDateTextBox.Text.Trim, expireDateValue)) Then
                    SpecCase.ApprovalDate = Server.HtmlEncode(dateValue)
                    SpecCase.ExpirationDate = Server.HtmlEncode(expireDateValue)
                    SpecCase.PWaiverLength = expireDateValue.Subtract(dateValue).Days
                End If
            End If

            SpecCase.med_off_approval_comment = Server.HtmlEncode(DecisionComment.Text)

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

        Private Sub SaveSeniorMedicalReviewerAdditionalData()
            SpecCase.AlternateApprovalDate = Nothing
            SpecCase.AlternateExpirationDate = Nothing
            SpecCase.AlternatePWaiverLength = Nothing

            If (rblSeniorMedicalReviewerDecisionN.Checked) Then
                If (Not String.IsNullOrEmpty(rblSeniorMedicalReviewerFindings.SelectedValue) AndAlso Integer.Parse(rblSeniorMedicalReviewerFindings.SelectedValue) = SC_PWaivers.APPROVED_FINDING_VALUE) Then
                    Dim dateValue As DateTime
                    Dim expireDateValue As DateTime

                    If (DateTime.TryParse(txtSeniorMedicalReviewerPWaiverDate.Text.Trim, dateValue)) And (DateTime.TryParse(txtSeniorMedicalReviewerExpirationDate.Text.Trim, expireDateValue)) Then
                        SpecCase.AlternateApprovalDate = Server.HtmlEncode(dateValue)
                        SpecCase.AlternateExpirationDate = Server.HtmlEncode(expireDateValue)
                        SpecCase.AlternatePWaiverLength = expireDateValue.Subtract(dateValue).Days
                    End If
                End If
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
            If (rblSeniorMedicalReviewerDecisionY.Visible = False) Then
                Exit Sub
            End If

            Dim selectedValue As String = String.Empty
            If (rblSeniorMedicalReviewerDecisionY.Checked) Then
                selectedValue = "Y"
            ElseIf (rblSeniorMedicalReviewerDecisionN.Checked) Then
                selectedValue = "N"
            ElseIf (rblSeniorMedicalReviewerDecisionR.Checked) Then
                selectedValue = "R"
            End If

            If (Not String.IsNullOrEmpty(selectedValue)) Then
                SpecCase.SeniorMedicalReviewerConcur = Server.HtmlEncode(selectedValue)
            End If
        End Sub

        Private Sub SaveSeniorMedicalReviewerDecisionComment()
            If (txtSeniorMedicalReviewerDecisionComment.Text.Length > txtSeniorMedicalReviewerDecisionComment.MaxLength) Then
                Exit Sub
            End If

            SpecCase.SeniorMedicalReviewerComment = Server.HtmlEncode(txtSeniorMedicalReviewerDecisionComment.Text)
        End Sub

        Private Sub SaveSeniorMedicalReviewerFinding()
            If (rblSeniorMedicalReviewerDecisionY.Visible = False AndAlso Not String.IsNullOrEmpty(rblSeniorMedicalReviewerFindings.SelectedValue)) Then
                SpecCase.SeniorMedicalReviewerApproved = Integer.Parse(rblSeniorMedicalReviewerFindings.SelectedValue)
            ElseIf (rblSeniorMedicalReviewerDecisionY.Visible = True AndAlso rblSeniorMedicalReviewerDecisionN.Checked AndAlso Not String.IsNullOrEmpty(rblSeniorMedicalReviewerFindings.SelectedValue)) Then
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
            If (rblSeniorMedicalReviewerDecisionY.Visible) Then
                pnlSeniorMedicalReviewerFindings.CssClass = "FindingsIndent"
            Else
                pnlSeniorMedicalReviewerFindings.CssClass = String.Empty
            End If
        End Sub

        Private Sub SetInputFormatRestrictions()
            SetInputFormatRestriction(Page, DecisionComment, FormatRestriction.AlphaNumeric, PERMITTED_SPECIAL_CHAR_INPUT)
            SetInputFormatRestriction(Page, PWaiverDateTextBox, FormatRestriction.Numeric, "/")
            SetInputFormatRestriction(Page, PWaiverExpirationDateTextBox, FormatRestriction.Numeric, "/")

            SetInputFormatRestriction(Page, txtSeniorMedicalReviewerDecisionComment, FormatRestriction.AlphaNumeric, PERMITTED_SPECIAL_CHAR_INPUT)
            SetInputFormatRestriction(Page, txtSeniorMedicalReviewerPWaiverDate, FormatRestriction.Numeric, "/")
            SetInputFormatRestriction(Page, txtSeniorMedicalReviewerExpirationDate, FormatRestriction.Numeric, "/")
        End Sub

        Private Sub SetMaxLengthForControls()
            SetMaxLength(DecisionComment)
            SetMaxLength(txtSeniorMedicalReviewerDecisionComment)
        End Sub

        Private Sub SetReadOnlyAccessForBoardMedicalOfficerControls()
            DecisionY.Enabled = False
            DecisionN.Enabled = False
            DecisionComment.Enabled = False
            PWaiverDateTextBox.Visible = False
            PWaiverExpirationDateLabel.Visible = False
            PWaiverExpirationDateTextBox.Visible = False
            PWaiverDateLabel.Visible = True
            PWaiverDateTextBox.Visible = False
        End Sub

        Private Sub SetReadOnlyAccessForControls()
            SetReadOnlyAccessForBoardMedicalOfficerControls()
            SetReadOnlyAccessForSeniorMedicalReviewerControls()
        End Sub

        Private Sub SetReadOnlyAccessForSeniorMedicalReviewerControls()
            rblSeniorMedicalReviewerDecisionY.Enabled = False
            rblSeniorMedicalReviewerDecisionN.Enabled = False
            rblSeniorMedicalReviewerDecisionR.Enabled = False
            rblSeniorMedicalReviewerFindings.Enabled = False
            txtSeniorMedicalReviewerDecisionComment.Enabled = False
            txtSeniorMedicalReviewerPWaiverDate.Visible = False
            lblSeniorMedicalReviewerPWaiverDate.Visible = True
            lblSeniorMedicalReviewerExpirationDate.Visible = False
            txtSeniorMedicalReviewerExpirationDate.Visible = False
        End Sub

        Private Sub SetReadWriteAccessForBoardMedicalOfficerControls()
            DecisionY.Enabled = True
            DecisionN.Enabled = True
            DecisionComment.Enabled = True
            PWaiverDateLabel.Visible = False
            PWaiverDateTextBox.Visible = True
            PWaiverExpirationDateLabel.Visible = True
            PWaiverExpirationDateTextBox.Visible = True

            PWaiverDateTextBox.CssClass = "datePicker"
            PWaiverExpirationDateTextBox.CssClass = "datePicker"
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
            rblSeniorMedicalReviewerDecisionY.Enabled = True
            rblSeniorMedicalReviewerDecisionN.Enabled = True
            rblSeniorMedicalReviewerDecisionR.Enabled = True
            rblSeniorMedicalReviewerFindings.Enabled = True
            txtSeniorMedicalReviewerDecisionComment.Enabled = True
            lblSeniorMedicalReviewerPWaiverDate.Visible = False
            lblSeniorMedicalReviewerExpirationDate.Visible = True
            txtSeniorMedicalReviewerPWaiverDate.Visible = True
            txtSeniorMedicalReviewerExpirationDate.Visible = True

            txtSeniorMedicalReviewerPWaiverDate.CssClass = "datePicker"
            txtSeniorMedicalReviewerExpirationDate.CssClass = "datePicker"
        End Sub

        Private Sub SetSeniorMedicalReviewerFindingsPanelVisibility()
            rblSeniorMedicalReviewerDecisionY.Visible = True
            rblSeniorMedicalReviewerDecisionN.Visible = True
            rblSeniorMedicalReviewerDecisionR.Visible = True

            If (rblSeniorMedicalReviewerDecisionN.Checked) Then
                pnlSeniorMedicalReviewerFindings.Visible = True
                pnlSeniorMedicalReviewerAdditionalData.Visible = True
            Else
                pnlSeniorMedicalReviewerFindings.Visible = False
                pnlSeniorMedicalReviewerAdditionalData.Visible = False
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