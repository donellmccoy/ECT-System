Imports ALOD.Core.Domain.Common
Imports ALOD.Core.Domain.Documents
Imports ALOD.Core.Domain.Lookup
Imports ALOD.Core.Domain.Modules.SpecialCases
Imports ALOD.Core.Domain.Users
Imports ALOD.Core.Domain.Workflow
Imports ALOD.Core.Interfaces.DAOInterfaces
Imports ALOD.Core.Utils
Imports ALOD.Core.Utils.DataTransferObjects
Imports ALOD.Data
Imports ALOD.Logging
Imports ALOD.Web.UserControls
Imports ALODWebUtility.Common

Namespace Web.Special_Case.IRILO

    Partial Class Secure_sc_FT_MedOff
        Inherits System.Web.UI.Page

#Region "Fields"

        Private _daoFactory As IDaoFactory
        Private _lookupDao As ILookupDao
        Private _memoDao As IMemoDao2
        Private _scAccess As Dictionary(Of String, ALOD.Core.Domain.Workflow.PageAccessType)
        Private _specCase As SC_FastTrack = Nothing
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

        Protected ReadOnly Property LookupDao As ILookupDao
            Get
                If (_lookupDao Is Nothing) Then
                    _lookupDao = DaoFactory.GetLookupDao()
                End If

                Return _lookupDao
            End Get
        End Property

        Protected ReadOnly Property MasterPage() As SC_FastTrackMaster
            Get
                Dim master As SC_FastTrackMaster = CType(Page.Master, SC_FastTrackMaster)
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

        Protected ReadOnly Property SpecCase() As SC_FastTrack
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

        Private ReadOnly Property RefId() As Integer
            Get
                Return CInt(Request.QueryString("refId"))
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

        Protected Sub ddlProcess_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ddlProcess.SelectedIndexChanged
            InitMemosDropDownList(GetInitMemoDropdownListDtoForBoardMedical(True))
        End Sub

        Protected Sub ddlSeniorMedicalReviewerProcessAs_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ddlSeniorMedicalReviewerProcessAs.SelectedIndexChanged
            InitMemosDropDownList(GetInitMemoDropdownListDtoForSeniorMedicalReviewer(True))
        End Sub

        Protected Sub Decision_SelectedIndexChanged(sender As Object, e As EventArgs) Handles rblDecision.SelectedIndexChanged
            SetReadWriteAccessForBoardMedicalOfficerFindingsDependentControls(rblDecision.SelectedValue)
            InitMemosDropDownList(GetInitMemoDropdownListDtoForBoardMedical(True))
        End Sub

        Protected Sub InitFindingRadioButtonsList(groupId As UserGroups, decisionsControl As RadioButtonList, findingId As Nullable(Of Integer))
            decisionsControl.DataSource = LookupDao.GetWorkflowFindings(SpecCase.Workflow, groupId)
            decisionsControl.DataValueField = "Id"
            decisionsControl.DataTextField = "Description"
            decisionsControl.DataBind()

            If (findingId.HasValue) Then
                decisionsControl.SelectedValue = findingId.Value
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
                LogManager.LogAction(ModuleType.SpecCaseFT, UserAction.ViewPage, RefId, "Viewed Page: Board Med")
            End If
        End Sub

        Protected Sub rblSeniorMedicalReviewerDecision_SelectedIndexChanged(sender As Object, e As EventArgs) Handles rblSeniorMedicalReviewerDecision.SelectedIndexChanged
            SetSeniorMedicalReviewerFindingsPanelVisibility()
        End Sub

        Protected Sub rblSeniorMedicalReviewerFindings_SelectedIndexChanged(sender As Object, e As EventArgs) Handles rblSeniorMedicalReviewerFindings.SelectedIndexChanged
            SetReadWriteAccessForSeniorMedicalReviewerFindingsDependentControls(rblSeniorMedicalReviewerFindings.SelectedValue)
            InitMemosDropDownList(GetInitMemoDropdownListDtoForSeniorMedicalReviewer(True))
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

        Private Function GetInitMemoDropdownListDtoForBoardMedical(usePageControlValues As Boolean) As IRILOMedicalOfficerTabInitMemosDropdownListDto
            Dim dto As New IRILOMedicalOfficerTabInitMemosDropdownListDto()

            dto.MemosDropDownList = ddlMemos

            If (usePageControlValues) Then
                If (Not String.IsNullOrEmpty(rblDecision.SelectedValue)) Then
                    dto.BoardMedicalFindingId = rblDecision.SelectedValue
                Else
                    dto.BoardMedicalFindingId = Nothing
                End If

                If (Not String.IsNullOrEmpty(ddlMemos.SelectedValue)) Then
                    dto.MemoTemplateId = ddlMemos.SelectedValue
                Else
                    dto.MemoTemplateId = Nothing
                End If

                If (Not String.IsNullOrEmpty(ddlProcess.SelectedValue)) Then
                    dto.ProcessAs = ddlProcess.SelectedValue
                Else
                    dto.ProcessAs = Nothing
                End If
            Else
                dto.BoardMedicalFindingId = SpecCase.med_off_approved
                dto.MemoTemplateId = SpecCase.MemoTemplateID
                dto.ProcessAs = SpecCase.Process
            End If

            Return dto
        End Function

        Private Function GetInitMemoDropdownListDtoForSeniorMedicalReviewer(usePageControlValues As Boolean) As IRILOMedicalOfficerTabInitMemosDropdownListDto
            Dim dto As New IRILOMedicalOfficerTabInitMemosDropdownListDto()

            dto.MemosDropDownList = ddlSeniorMedicalReviewerMemos

            If (usePageControlValues) Then
                If (Not String.IsNullOrEmpty(rblSeniorMedicalReviewerFindings.SelectedValue)) Then
                    dto.BoardMedicalFindingId = rblSeniorMedicalReviewerFindings.SelectedValue
                Else
                    dto.BoardMedicalFindingId = Nothing
                End If

                If (Not String.IsNullOrEmpty(ddlSeniorMedicalReviewerMemos.SelectedValue)) Then
                    dto.MemoTemplateId = ddlSeniorMedicalReviewerMemos.SelectedValue
                Else
                    dto.MemoTemplateId = Nothing
                End If

                If (Not String.IsNullOrEmpty(ddlSeniorMedicalReviewerProcessAs.SelectedValue)) Then
                    dto.ProcessAs = ddlSeniorMedicalReviewerProcessAs.SelectedValue
                Else
                    dto.ProcessAs = Nothing
                End If
            Else
                dto.BoardMedicalFindingId = SpecCase.SeniorMedicalReviewerApproved
                dto.MemoTemplateId = SpecCase.AlternateMemoTemplateID
                dto.ProcessAs = SpecCase.AlternateProcessAs
            End If

            Return dto
        End Function

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

        Private Sub InitMemosDropDownList(args As IRILOMedicalOfficerTabInitMemosDropdownListDto)
            If (Not args.BoardMedicalFindingId.HasValue) Then
                Exit Sub
            End If

            args.MemosDropDownList.Items.Clear()

            Dim memoTemplates As IList(Of MemoTemplate) = MemoDao.GetTemplatesByModule(ModuleType.SpecCaseFT)

            If ((args.BoardMedicalFindingId.Value = Core.Utils.Finding.Disqualify OrElse args.BoardMedicalFindingId.Value = Core.Utils.Finding.Admin_Disqualified) AndAlso (args.ProcessAs.HasValue AndAlso args.ProcessAs.Value = 1)) Then
                args.MemosDropDownList.DataSource = From memo In memoTemplates Where memo.Title.Equals("IRILO DQ MEB") Select memo
            ElseIf ((args.BoardMedicalFindingId.Value = Core.Utils.Finding.Disqualify OrElse args.BoardMedicalFindingId.Value = Core.Utils.Finding.Admin_Disqualified) AndAlso (args.ProcessAs.HasValue AndAlso args.ProcessAs.Value = 2)) Then
                args.MemosDropDownList.DataSource = From memo In memoTemplates Where memo.Title.Equals("IRILO DQ WWD") Select memo
            ElseIf ((args.BoardMedicalFindingId.Value = Core.Utils.Finding.Disqualify OrElse args.BoardMedicalFindingId.Value = Core.Utils.Finding.Admin_Disqualified) AndAlso (args.ProcessAs.HasValue AndAlso args.ProcessAs.Value = 3)) Then
                args.MemosDropDownList.DataSource = From memo In memoTemplates Where memo.Title.Equals("IRILO DQ LOD Pending") Select memo
            Else
                args.MemosDropDownList.DataSource = memoTemplates
            End If

            args.MemosDropDownList.DataTextField = "Title"
            args.MemosDropDownList.DataValueField = "Id"
            args.MemosDropDownList.DataBind()

            Utility.InsertDropDownListZeroValue(args.MemosDropDownList, "-- Select a Memo --")

            If (args.MemoTemplateId.HasValue AndAlso args.MemosDropDownList.Items.FindByValue(args.MemoTemplateId.Value) IsNot Nothing) Then
                args.MemosDropDownList.SelectedValue = args.MemoTemplateId.Value
            Else
                args.MemosDropDownList.SelectedValue = 0
            End If
        End Sub

        Private Sub InitProcessAsDropDownList(processAsDropDownListControl As DropDownList, selectedValue As Nullable(Of Integer))
            processAsDropDownListControl.Items.Clear()

            processAsDropDownListControl.DataSource = LookupDao.GetProcess()
            processAsDropDownListControl.DataTextField = "Name"
            processAsDropDownListControl.DataValueField = "Value"
            processAsDropDownListControl.DataBind()

            Utility.InsertDropDownListZeroValue(processAsDropDownListControl, "-- Select One --")

            If (selectedValue.HasValue AndAlso processAsDropDownListControl.Items.FindByValue(selectedValue.Value) IsNot Nothing) Then
                processAsDropDownListControl.SelectedValue = selectedValue.Value
            Else
                processAsDropDownListControl.SelectedValue = 0
            End If
        End Sub

        Private Sub LoadBoardMedicalFindings()
            InitProcessAsDropDownList(ddlProcess, SpecCase.Process)
            InitFindingRadioButtonsList(UserGroups.BoardMedical, rblDecision, SpecCase.med_off_approved)
            InitMemosDropDownList(GetInitMemoDropdownListDtoForBoardMedical(False))
            LoadIcdControls(ucICDCodeControl, DiagnosisTextBox, ucICD7thCharacterControl)

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

            If (SpecCase.MemoTemplateID.HasValue) Then
                ddlMemos.SelectedValue = SpecCase.MemoTemplateID
            End If
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
                    icdDiagnosisTextBox.Text = Server.HtmlDecode(SpecCase.FTDiagnosis)
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

            InitProcessAsDropDownList(ddlSeniorMedicalReviewerProcessAs, SpecCase.AlternateProcessAs)
            InitFindingRadioButtonsList(UserGroups.SeniorMedicalReviewer, rblSeniorMedicalReviewerFindings, SpecCase.SeniorMedicalReviewerApproved)
            InitMemosDropDownList(GetInitMemoDropdownListDtoForSeniorMedicalReviewer(False))
            LoadIcdControls(ucSeniorMedicalReviewerICDCodeControl, txtSeniorMedicalReviewerDiagnosisText, ucSeniorMedicalReviewerICD7thCharacterControl)

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

            If (Not String.IsNullOrEmpty(SpecCase.SeniorMedicalReviewerComment)) Then
                txtSeniorMedicalReviewerDecisionComment.Text = HTMLDecodeNulls(SpecCase.SeniorMedicalReviewerComment)
            End If

            If (SpecCase.AlternateMemoTemplateID.HasValue) Then
                ddlSeniorMedicalReviewerMemos.SelectedValue = SpecCase.AlternateMemoTemplateID
            End If
        End Sub

        Private Sub SaveBoardMedicalData()
            SaveIcdData(ucICDCodeControl, DiagnosisTextBox, ucICD7thCharacterControl)

            If (Not String.IsNullOrEmpty(rblDecision.SelectedValue)) Then
                SpecCase.med_off_approved = rblDecision.SelectedValue
            End If

            If (Not String.IsNullOrEmpty(DecisionComment.Text)) Then
                SpecCase.med_off_approval_comment = HTMLEncodeNulls(DecisionComment.Text)
            End If

            If (Not String.IsNullOrEmpty(rblDecision.SelectedValue)) Then
                SpecCase.Process = ddlProcess.SelectedValue
            Else
                SpecCase.Process = 0
            End If

            SaveBoardMedicalFindingDependentData()
            SaveHqAfrcTechEditableBoardMedicalData()
            SaveBoardMedicalMemoSelection()

            SpecCaseDao.SaveOrUpdate(SpecCase)
            SpecCaseDao.CommitChanges()
        End Sub

        Private Sub SaveBoardMedicalFindingDependentData()
            If (String.IsNullOrEmpty(rblDecision.SelectedValue)) Then
                Exit Sub
            End If

            If (rblDecision.SelectedValue = Core.Utils.Finding.Disqualify Or rblDecision.SelectedValue = Core.Utils.Finding.Admin_Disqualified) Then
                If (Not String.IsNullOrEmpty(DQParagraph.Text)) Then
                    SpecCase.DQParagraph = HTMLEncodeNulls(DQParagraph.Text.Trim())
                End If

                SpecCase.DQCompletionDate = Now
            End If

            If (rblDecision.SelectedValue = Core.Utils.Finding.Qualify_RTD Or rblDecision.SelectedValue = Core.Utils.Finding.Admin_Qualified) Then
                SpecCase.DQParagraph = String.Empty
                SpecCase.DQCompletionDate = Nothing 'If the case is "Returned" to the Board Medical and decides to change the determination
            End If

            If (rblDecision.SelectedValue = Core.Utils.Finding.Admin_LOD) Then
                SpecCase.DQParagraph = String.Empty
                SpecCase.DQCompletionDate = Nothing 'If the case is "Returned" to the Board Medical and decides to change the determination
            End If
        End Sub

        Private Sub SaveBoardMedicalMemoSelection()
            If (String.IsNullOrEmpty(ddlMemos.SelectedValue)) Then
                Exit Sub
            End If

            If (ddlMemos.SelectedValue = 0) Then
                SpecCase.MemoTemplateID = Nothing
            Else
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

        Private Sub SaveHqAfrcTechEditableBoardMedicalData()
            If (Not String.IsNullOrEmpty(ReturnToDutyDate.Text)) Then
                SpecCase.ReturnToDutyDate = Server.HtmlEncode(ReturnToDutyDate.Text)
            End If

            If (Not String.IsNullOrEmpty(ExpirationDate.Text)) Then
                SpecCase.ExpirationDate = Server.HtmlEncode(ExpirationDate.Text)
            End If

            If (Not String.IsNullOrEmpty(ddlAssignmentLimitationCode.SelectedValue)) Then
                SpecCase.ALCLetterType = ddlAssignmentLimitationCode.SelectedValue
            End If
        End Sub

        Private Sub SaveHqAfrcTechEditableSeniorMedicalReviewerData()
            If (Not String.IsNullOrEmpty(txtSeniorMedicalReviewerReturnToDutyDate.Text)) Then
                SpecCase.AlternateReturnToDutyDate = Server.HtmlEncode(txtSeniorMedicalReviewerReturnToDutyDate.Text)
            End If

            If (Not String.IsNullOrEmpty(txtSeniorMedicalReviewerExpirationDate.Text)) Then
                SpecCase.AlternateExpirationDate = Server.HtmlEncode(txtSeniorMedicalReviewerExpirationDate.Text)
            End If

            If (Not String.IsNullOrEmpty(ddlSeniorMedicalReviewerAssignmentLimitationCode.SelectedValue)) Then
                SpecCase.AlternateALCLetterType = ddlSeniorMedicalReviewerAssignmentLimitationCode.SelectedValue
            End If
        End Sub

        Private Sub SaveHqAfrcTechnicianData()
            If (SpecCase.ShouldUseSeniorMedicalReviewerFindings()) Then
                SaveHqAfrcTechEditableSeniorMedicalReviewerData()
                SaveSeniorMedicalReviewerMemoSelection()
            Else
                SaveHqAfrcTechEditableBoardMedicalData()
                SaveBoardMedicalMemoSelection()
            End If

            SpecCaseDao.SaveOrUpdate(SpecCase)
            SpecCaseDao.CommitChanges()
        End Sub

        Private Sub SaveIcdData(icdCodeControl As ICDCodeControl, icdDiagnosisTextBox As TextBox, icd7thCharacterControl As ICD7thCharacterControl)
            If (icdCodeControl.IsICDCodeSelected()) Then
                SpecCase.ICD9Code = icdCodeControl.SelectedICDCodeID
                SpecCase.ICD9Description = icdCodeControl.SelectedICDCodeText
            End If

            SpecCase.FTDiagnosis = Server.HtmlEncode(icdDiagnosisTextBox.Text.Trim())

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

            If (Not String.IsNullOrEmpty(rblSeniorMedicalReviewerFindings.SelectedValue)) Then
                SpecCase.AlternateProcessAs = ddlSeniorMedicalReviewerProcessAs.SelectedValue
            Else
                SpecCase.AlternateProcessAs = 0
            End If

            SaveSeniorMedicalReviewerFindingDependentData()
            SaveHqAfrcTechEditableSeniorMedicalReviewerData()
            SaveSeniorMedicalReviewerMemoSelection()
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

        Private Sub SaveSeniorMedicalReviewerFindingDependentData()
            If (String.IsNullOrEmpty(rblSeniorMedicalReviewerFindings.SelectedValue)) Then
                Exit Sub
            End If

            If (rblSeniorMedicalReviewerFindings.SelectedValue = Core.Utils.Finding.Disqualify Or rblSeniorMedicalReviewerFindings.SelectedValue = Core.Utils.Finding.Admin_Disqualified) Then
                If (Not String.IsNullOrEmpty(txtSeniorMedicalReviewerDQParagraph.Text)) Then
                    SpecCase.AlternateDQParagraph = HTMLEncodeNulls(txtSeniorMedicalReviewerDQParagraph.Text.Trim())
                End If

                SpecCase.AlternateDQCompletionDate = Now
            End If

            If (rblSeniorMedicalReviewerFindings.SelectedValue = Core.Utils.Finding.Qualify_RTD Or rblSeniorMedicalReviewerFindings.SelectedValue = Core.Utils.Finding.Admin_Qualified) Then
                SpecCase.AlternateDQParagraph = String.Empty
                SpecCase.AlternateDQCompletionDate = Nothing 'If the case is "Returned" to the Board Medical and decides to change the determination
            End If

            If (rblSeniorMedicalReviewerFindings.SelectedValue = Core.Utils.Finding.Admin_LOD) Then
                SpecCase.AlternateDQParagraph = String.Empty
                SpecCase.AlternateDQCompletionDate = Nothing 'If the case is "Returned" to the Board Medical and decides to change the determination
            End If
        End Sub

        Private Sub SaveSeniorMedicalReviewerMemoSelection()
            If (String.IsNullOrEmpty(ddlSeniorMedicalReviewerMemos.SelectedValue)) Then
                Exit Sub
            End If

            If (ddlSeniorMedicalReviewerMemos.SelectedValue = 0) Then
                SpecCase.AlternateMemoTemplateID = Nothing
            Else
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
            rblDecision.Enabled = False
            DecisionComment.ReadOnly = True
            DQParagraph.ReadOnly = True
            DiagnosisTextBox.ReadOnly = True
            ExpirationDate.Enabled = False
            ReturnToDutyDate.Enabled = False
            ddlMemos.Enabled = False
            ddlAssignmentLimitationCode.Enabled = False
            ddlProcess.Enabled = False

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
            ddlSeniorMedicalReviewerProcessAs.Enabled = False

            txtSeniorMedicalReviewerReturnToDutyDate.CssClass = ""
            txtSeniorMedicalReviewerExpirationDate.CssClass = ""
        End Sub

        Private Sub SetReadWriteAccessForBoardMedicalOfficer()
            SetReadWriteAccessForBoardMedicalReviewStepControls()
        End Sub

        Private Sub SetReadWriteAccessForBoardMedicalOfficerFindingsDependentControls(boardMedicalFinding As Nullable(Of Integer))
            If (boardMedicalFinding.HasValue) Then
                If (boardMedicalFinding.Value = Core.Utils.Finding.Disqualify OrElse boardMedicalFinding.Value = Core.Utils.Finding.Admin_Disqualified) Then
                    decisionlbl.CssClass = "label labelRequired"
                    DQlbl.CssClass = "label labelRequired"
                    ProcessRow.CssClass = "label labelRequired"
                    ddlProcess.Enabled = True
                Else
                    decisionlbl.CssClass = "label"
                    DQlbl.CssClass = "label"
                    ProcessRow.CssClass = "label"
                    ddlProcess.Enabled = False
                End If

                If (boardMedicalFinding.Value = 1) Then
                    lblReturnToDutyDateRow.CssClass = "label labelRequired"
                Else
                    lblReturnToDutyDateRow.CssClass = "label"
                End If
            End If
        End Sub

        Private Sub SetReadWriteAccessForBoardMedicalReviewStepControls()
            ucICDCodeControl.DisplayReadWrite(False)
            ucICD7thCharacterControl.DisplayReadWrite()

            rblDecision.Enabled = True
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
            ddlProcess.Enabled = True

            ExpirationDate.CssClass = "datePickerPlusFuture"
            ReturnToDutyDate.CssClass = "datePickerFuture"

            SetReadWriteAccessForBoardMedicalOfficerFindingsDependentControls(SpecCase.med_off_approved)
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
            If (SpecCase.ShouldUseSeniorMedicalReviewerFindings()) Then
                SetRestrictedReadWriteAccessForSeniorMedicalReviewerControls()
            Else
                SetRestrictedReadWriteAccessForBoardMedicalControls()
            End If
        End Sub

        Private Sub SetReadWriteAccessForSeniorMedicalReviewer()
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
            ddlSeniorMedicalReviewerProcessAs.Enabled = True

            txtSeniorMedicalReviewerExpirationDate.CssClass = "datePickerPlusFuture"
            txtSeniorMedicalReviewerReturnToDutyDate.CssClass = "datePickerFuture"

            SetReadWriteAccessForSeniorMedicalReviewerFindingsDependentControls(SpecCase.SeniorMedicalReviewerApproved)
        End Sub

        Private Sub SetReadWriteAccessForSeniorMedicalReviewerFindingsDependentControls(seniorMedicalReviewerFinding As Nullable(Of Integer))
            If (seniorMedicalReviewerFinding.HasValue) Then
                If (seniorMedicalReviewerFinding.Value = Core.Utils.Finding.Disqualify OrElse seniorMedicalReviewerFinding.Value = Core.Utils.Finding.Admin_Disqualified) Then
                    lblSeniorMedicalReviewerDecisionExplanationRow.CssClass = "label labelRequired"
                    lblSeniorMedicalReviewerDqParagraphRow.CssClass = "label labelRequired"
                    lblSeniorMedicalReviewerProcessAsRow.CssClass = "label labelRequired"
                    ddlSeniorMedicalReviewerProcessAs.Enabled = True
                Else
                    lblSeniorMedicalReviewerDecisionExplanationRow.CssClass = "label"
                    lblSeniorMedicalReviewerDqParagraphRow.CssClass = "label"
                    lblSeniorMedicalReviewerProcessAsRow.CssClass = "label"
                    ddlSeniorMedicalReviewerProcessAs.Enabled = False
                End If

                If (seniorMedicalReviewerFinding.Value = 1) Then
                    lblSeniorMedicalReviewerReturnToDutyDateRow.CssClass = "label labelRequired"
                Else
                    lblSeniorMedicalReviewerReturnToDutyDateRow.CssClass = "label"
                End If
            End If
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