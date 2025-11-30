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

Namespace Web.Special_Case.RS

    Partial Class Secure_sc_rs_MedOff
        Inherits System.Web.UI.Page

#Region "Fields"

        Private _daoFactory As IDaoFactory
        Private _scAccess As Dictionary(Of String, ALOD.Core.Domain.Workflow.PageAccessType)
        Private _specCase As SC_RS = Nothing
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

        Protected ReadOnly Property MasterPage() As SC_RSMaster
            Get
                Dim master As SC_RSMaster = CType(Page.Master, SC_RSMaster)
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

        Protected ReadOnly Property SpecCase() As SC_RS
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
                LogManager.LogAction(ModuleType.SpecCaseRS, UserAction.ViewPage, RefId, "Viewed Page: Medical Officer Desicion")
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
                End If

                If (SpecCase.med_off_approved.Value = 0) Then
                    DecisionN.Checked = True
                End If
            End If

            If (Not String.IsNullOrEmpty(SpecCase.med_off_approval_comment)) Then
                DecisionComment.Text = Server.HtmlDecode(SpecCase.med_off_approval_comment)
            End If

            If (SpecCase.AlternateALCLetterType.HasValue) Then
                ddlALC.SelectedValue = SpecCase.AlternateALCLetterType
            End If

            If (SpecCase.ExpirationDate.HasValue) Then
                txtRenewalDate.Text = Server.HtmlDecode(SpecCase.ExpirationDate.Value.ToString(DATE_FORMAT))
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
            End If

            If (Not String.IsNullOrEmpty(SpecCase.SeniorMedicalReviewerComment)) Then
                txtSeniorMedicalReviewerDecisionComment.Text = Server.HtmlDecode(SpecCase.SeniorMedicalReviewerComment)
            End If

            If (SpecCase.AlternateAlternateALCLetterType.HasValue) Then
                ddlSeniorMedicalReviewerALC.SelectedValue = SpecCase.AlternateAlternateALCLetterType
            End If

            If (SpecCase.AlternateExpirationDate.HasValue) Then
                txtSeniorMedicalReviewerRenewalDate.Text = Server.HtmlDecode(SpecCase.AlternateExpirationDate.Value.ToString(DATE_FORMAT))
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

            If (Not String.IsNullOrEmpty(ddlALC.SelectedValue)) Then
                SpecCase.AlternateALCLetterType = ddlALC.SelectedValue
            End If

            If (Not String.IsNullOrEmpty(txtRenewalDate.Text)) Then
                SpecCase.ExpirationDate = Server.HtmlEncode(txtRenewalDate.Text)
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

        Private Sub SaveSeniorMedicalReviewerAdditionalData()
            If (Not String.IsNullOrEmpty(ddlSeniorMedicalReviewerALC.SelectedValue)) Then
                SpecCase.AlternateAlternateALCLetterType = ddlSeniorMedicalReviewerALC.SelectedValue
            End If

            If (Not String.IsNullOrEmpty(txtSeniorMedicalReviewerRenewalDate.Text)) Then
                SpecCase.AlternateExpirationDate = Server.HtmlEncode(txtSeniorMedicalReviewerRenewalDate.Text)
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
            SetInputFormatRestriction(Page, txtRenewalDate, FormatRestriction.Numeric, "/")

            SetInputFormatRestriction(Page, txtSeniorMedicalReviewerDecisionComment, FormatRestriction.AlphaNumeric, PERMITTED_SPECIAL_CHAR_INPUT)
            SetInputFormatRestriction(Page, txtSeniorMedicalReviewerRenewalDate, FormatRestriction.Numeric, "/")
        End Sub

        Private Sub SetMaxLengthForControls()
            SetMaxLength(DecisionComment)
            SetMaxLength(txtSeniorMedicalReviewerDecisionComment)
        End Sub

        Private Sub SetReadOnlyAccessForBoardMedicalOfficerControls()
            DecisionY.Enabled = False
            DecisionN.Enabled = False
            DecisionComment.Enabled = False
            ddlALC.Enabled = False
            txtRenewalDate.Enabled = False
        End Sub

        Private Sub SetReadOnlyAccessForControls()
            SetReadOnlyAccessForBoardMedicalOfficerControls()
            SetReadOnlyAccessForSeniorMedicalReviewerControls()
        End Sub

        Private Sub SetReadOnlyAccessForSeniorMedicalReviewerControls()
            rblSeniorMedicalReviewerDecision.Enabled = False
            rblSeniorMedicalReviewerFindings.Enabled = False
            txtSeniorMedicalReviewerDecisionComment.Enabled = False
            ddlSeniorMedicalReviewerALC.Enabled = False
            txtSeniorMedicalReviewerRenewalDate.Enabled = False
        End Sub

        Private Sub SetReadWriteAccessForBoardMedicalOfficerControls()
            DecisionY.Enabled = True
            DecisionN.Enabled = True
            DecisionComment.Enabled = True
            ddlALC.Enabled = True
            txtRenewalDate.Enabled = True

            txtRenewalDate.CssClass = "datePickerPlusFuture"
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
            ddlSeniorMedicalReviewerALC.Enabled = True
            txtSeniorMedicalReviewerRenewalDate.Enabled = True

            txtSeniorMedicalReviewerRenewalDate.CssClass = "datePickerPlusFuture"
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