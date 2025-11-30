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

Namespace Web.Special_Case.MH

    Partial Class Secure_sc_mh_MedOff
        Inherits System.Web.UI.Page

#Region "Fields"

        Private _BoardSGSig As SignatureMetaData
        Private _BoardSMRSig As SignatureMetaData
        Private _daoFactory As IDaoFactory
        Private _scAccess As Dictionary(Of String, PageAccessType)
        Private _sigDao As ISignatueMetaDateDao
        Private _specCase As SC_MH
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

        Protected ReadOnly Property SectionList() As Dictionary(Of String, ALOD.Core.Domain.Workflow.PageAccessType)
            Get
                If (_scAccess Is Nothing) Then
                    _scAccess = SpecCase.ReadSectionList(CInt(Session("GroupId")))
                End If
                Return _scAccess
            End Get
        End Property

        Protected ReadOnly Property SpecCase() As SC_MH
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

        Private ReadOnly Property BoardSGSig() As SignatureMetaData
            Get
                If (_BoardSGSig Is Nothing) Then
                    _BoardSGSig = SigDao.GetByWorkStatus(SpecCase.Id, SpecCase.Workflow, SpecCaseMHWorkStatus.MedicalReview)
                End If

                Return _BoardSGSig
            End Get
        End Property

        Private ReadOnly Property BoardSMRSig() As SignatureMetaData
            Get
                If (_BoardSMRSig Is Nothing) Then
                    _BoardSMRSig = SigDao.GetByWorkStatus(SpecCase.Id, SpecCase.Workflow, SpecCaseMHWorkStatus.SeniorMedicalReview)
                End If

                Return _BoardSMRSig
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
                LogManager.LogAction(ModuleType.SpecCaseMH, UserAction.ViewPage, RefId, "Viewed Page: MH Board Med")
            End If
        End Sub

        Protected Sub rblSeniorMedicalReviewerDecision_SelectedIndexChanged(sender As Object, e As EventArgs) Handles rblSeniorMedicalReviewerDecision.SelectedIndexChanged
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
                ElseIf (SpecCase.med_off_approved.Value = 0) Then
                    DecisionN.Checked = True
                End If

                If ((SpecCase.med_off_approved = 1) And
                    (SpecCase.Status = SpecCaseMHWorkStatus.FinalReview Or SpecCase.Status = SpecCaseMHWorkStatus.Approved Or SpecCase.Status = SpecCaseMHWorkStatus.Disapproved Or SpecCase.Status = SpecCaseMHWorkStatus.Override)) Then
                    If (BoardSGSig IsNot Nothing) Then
                        lblExperationDate.Text = String.Format("Decision Date: {0} - Expiration Date : {1}", Server.HtmlDecode(BoardSGSig.date.ToString(DATE_FORMAT)),
                                                          Server.HtmlDecode(SpecCase.ExpirationDate.Value.ToString(DATE_FORMAT)))
                    End If
                Else
                    If (BoardSGSig IsNot Nothing) Then
                        lblExperationDate.Text = String.Format("Decision Date: {0}", Server.HtmlDecode(BoardSGSig.date.ToString(DATE_FORMAT)))

                    End If

                End If
            End If

            If (SpecCase.ExpirationDate.HasValue) Then
                txtExpirationDate.Text = SpecCase.ExpirationDate
            End If

            If (Not String.IsNullOrEmpty(SpecCase.med_off_approval_comment)) Then
                DecisionComment.Text = Server.HtmlDecode(SpecCase.med_off_approval_comment)
            End If

        End Sub

        Private Sub LoadFindings()
            LoadBoardMedicalFindings()
            LoadSeniorMedicalReviewerFindings()
        End Sub

        Private Sub LoadSeniorMedicalReviewerFindings()
            If (Not String.IsNullOrEmpty(SpecCase.SeniorMedicalReviewerConcur)) Then
                rblSeniorMedicalReviewerDecision.SelectedValue = Server.HtmlDecode(SpecCase.SeniorMedicalReviewerConcur)
            End If

            If (SpecCase.SeniorMedicalReviewerApproved.HasValue) Then
                rblSeniorMedicalReviewerFindings.SelectedValue = SpecCase.SeniorMedicalReviewerApproved.Value

                If ((SpecCase.SeniorMedicalReviewerApproved = SC_MH.APPROVED_FINDING_VALUE) And
                    (SpecCase.Status = SpecCaseMHWorkStatus.FinalReview Or SpecCase.Status = SpecCaseMHWorkStatus.Approved Or SpecCase.Status = SpecCaseMHWorkStatus.Disapproved Or SpecCase.Status = SpecCaseMHWorkStatus.Override)) Then
                    If (BoardSMRSig IsNot Nothing) Then
                        lblSeniorMedicalReviewerExperationDate.Text = String.Format("Decision Date: {0} - Expiration Date : {1}", Server.HtmlDecode(BoardSMRSig.date.ToString(DATE_FORMAT)), Server.HtmlDecode(SpecCase.AlternateExpirationDate.Value.ToString(DATE_FORMAT)))
                    End If
                Else
                    If (BoardSMRSig IsNot Nothing) Then
                        lblSeniorMedicalReviewerExperationDate.Text = String.Format("Decision Date: {0}", Server.HtmlDecode(BoardSMRSig.date.ToString(DATE_FORMAT)))

                    End If

                End If

            End If

            If (SpecCase.AlternateExpirationDate.HasValue) Then
                txtSeniorMedialReviewExperationDate.Text = SpecCase.AlternateExpirationDate
            End If

            If (Not String.IsNullOrEmpty(SpecCase.SeniorMedicalReviewerComment)) Then
                SeniorMedicalReviewerDecisionComment.Text = Server.HtmlDecode(SpecCase.SeniorMedicalReviewerComment)
            End If

        End Sub

        Private Sub LogExpirationDateChangeForOverrideStep(currentExpirationDate As DateTime, expirationDateTextBox As TextBox, userGroupName As String)
            If (SpecCase.CurrentStatusCode = SpecCaseMHStatusCode.Override) Then
                Dim newExpirationDate As DateTime = Nothing

                If (DateTime.TryParse(expirationDateTextBox.Text.Trim, newExpirationDate) AndAlso Not currentExpirationDate.Date.Equals(newExpirationDate.Date)) Then
                    LogManager.LogAction(ModuleType.SpecCaseMH, UserAction.Override, RefId, "Changed " & userGroupName & " Expiration Date from: " & currentExpirationDate & " to " & newExpirationDate)
                End If
            End If
        End Sub

        Private Sub SaveBoardMedicalData()
            If (DecisionN.Checked) Then
                SpecCase.med_off_approved = 0
            End If

            If (DecisionY.Checked) Then
                SpecCase.med_off_approved = 1
            End If

            SpecCase.med_off_approval_comment = Server.HtmlEncode(DecisionComment.Text)

            SaveBoardMedicalOfficerExpirationDate()

            SpecCaseDao.SaveOrUpdate(SpecCase)
            SpecCaseDao.CommitChanges()
        End Sub

        Private Sub SaveBoardMedicalOfficerExpirationDate()
            Dim expireDateValue As DateTime

            If (DateTime.TryParse(txtExpirationDate.Text.Trim, expireDateValue)) Then
                SpecCase.ExpirationDate = Server.HtmlEncode(expireDateValue)
            End If
        End Sub

        Private Sub SaveData()
            If (Not UserCanEdit) Then
                Exit Sub
            End If

            If (SessionInfo.SESSION_GROUP_ID = UserGroups.AFRCHQTechnician) Then
                SaveHqAfrcTechnicianData()
            End If

            If (SessionInfo.SESSION_GROUP_ID = UserGroups.BoardMedical) Then
                SaveBoardMedicalData()
            End If

            If (SessionInfo.SESSION_GROUP_ID = UserGroups.SeniorMedicalReviewer) Then
                SaveSeniorMedicalReviewerData()
            End If
        End Sub

        Private Sub SaveHqAfrcTechnicianData()
            If (SpecCase.ShouldUseSeniorMedicalReviewerFindings()) Then
                LogExpirationDateChangeForOverrideStep(SpecCase.AlternateExpirationDate, txtSeniorMedialReviewExperationDate, "Senior Medical Reviewer")
                SaveSeniorMedicalReviewerExpirationDate()
            Else
                LogExpirationDateChangeForOverrideStep(SpecCase.ExpirationDate, txtExpirationDate, "Board Medical Officer")
                SaveBoardMedicalOfficerExpirationDate()
            End If

            SpecCaseDao.SaveOrUpdate(SpecCase)
            SpecCaseDao.CommitChanges()
        End Sub

        Private Sub SaveSeniorMedicalReviewerAdditionalData()
            If (rblSeniorMedicalReviewerDecision.SelectedValue.Equals(SpecialCase.DECISION_NONCONCUR)) Then
                SaveSeniorMedicalReviewerExpirationDate()
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
            If (SeniorMedicalReviewerDecisionComment.Text.Length > SeniorMedicalReviewerDecisionComment.MaxLength) Then
                Exit Sub
            End If

            SpecCase.SeniorMedicalReviewerComment = Server.HtmlEncode(SeniorMedicalReviewerDecisionComment.Text)
        End Sub

        Private Sub SaveSeniorMedicalReviewerExpirationDate()
            Dim expireDateValue As DateTime

            If (DateTime.TryParse(txtSeniorMedialReviewExperationDate.Text.Trim, expireDateValue)) Then
                SpecCase.AlternateExpirationDate = Server.HtmlEncode(expireDateValue)
            End If
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
            SetInputFormatRestriction(Page, txtExpirationDate, FormatRestriction.Numeric, "/")
            SetInputFormatRestriction(Page, DecisionComment, FormatRestriction.AlphaNumeric, PERMITTED_SPECIAL_CHAR_INPUT)

            SetInputFormatRestriction(Page, txtSeniorMedialReviewExperationDate, FormatRestriction.Numeric, "/")
            SetInputFormatRestriction(Page, SeniorMedicalReviewerDecisionComment, FormatRestriction.AlphaNumeric, PERMITTED_SPECIAL_CHAR_INPUT)
        End Sub

        Private Sub SetMaxLengthForControls()
            SetMaxLength(DecisionComment)
            SetMaxLength(SeniorMedicalReviewerDecisionComment)
        End Sub

        Private Sub SetReadOnlyAccessForBoardMedicalOfficerControls()
            DecisionY.Enabled = False
            DecisionN.Enabled = False
            DecisionComment.Enabled = False
            txtExpirationDate.Visible = False
            lblExperationDate.Visible = True
        End Sub

        Private Sub SetReadOnlyAccessForControls()
            SetReadOnlyAccessForBoardMedicalOfficerControls()
            SetReadOnlyAccessForSeniorMedicalReviewerControls()
        End Sub

        Private Sub SetReadOnlyAccessForSeniorMedicalReviewerControls()
            rblSeniorMedicalReviewerDecision.Enabled = False
            rblSeniorMedicalReviewerFindings.Enabled = False
            SeniorMedicalReviewerDecisionComment.Enabled = False
            txtSeniorMedialReviewExperationDate.Visible = False
            lblSeniorMedicalReviewerExperationDate.Visible = True
        End Sub

        Private Sub SetReadWriteAccessForBoardMedicalOfficerControls()
            DecisionY.Enabled = True
            DecisionN.Enabled = True
            DecisionComment.Enabled = True
            txtExpirationDate.Visible = True
            txtExpirationDate.Enabled = True
            lblExperationDate.Visible = False

            txtExpirationDate.CssClass = "datePicker"
        End Sub

        Private Sub SetReadWriteAccessForControls()
            If (SESSION_GROUP_ID = UserGroups.AFRCHQTechnician) Then
                SetReadWriteAccessForHqAfrcTechnicianControls()
            End If

            If (SESSION_GROUP_ID = UserGroups.BoardMedical) Then
                SetReadWriteAccessForBoardMedicalOfficerControls()
            End If

            If (SESSION_GROUP_ID = UserGroups.SeniorMedicalReviewer) Then
                SetReadWriteAccessForSeniorMedicalReviewerControls()
            End If
        End Sub

        Private Sub SetReadWriteAccessForHqAfrcTechnicianControls()
            If (SpecCase.ShouldUseSeniorMedicalReviewerFindings()) Then
                txtSeniorMedialReviewExperationDate.Visible = True
                txtSeniorMedialReviewExperationDate.Enabled = True
                lblSeniorMedicalReviewerExperationDate.Visible = False

                txtSeniorMedialReviewExperationDate.CssClass = "datePicker"
            Else
                txtExpirationDate.Visible = True
                txtExpirationDate.Enabled = True
                lblExperationDate.Visible = False

                txtExpirationDate.CssClass = "datePicker"
            End If
        End Sub

        Private Sub SetReadWriteAccessForSeniorMedicalReviewerControls()
            rblSeniorMedicalReviewerDecision.Enabled = True
            rblSeniorMedicalReviewerFindings.Enabled = True
            SeniorMedicalReviewerDecisionComment.Enabled = True
            txtSeniorMedialReviewExperationDate.Visible = True
            txtSeniorMedialReviewExperationDate.Enabled = True
            lblSeniorMedicalReviewerExperationDate.Visible = False

            txtSeniorMedialReviewExperationDate.CssClass = "datePicker"
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