Imports ALOD.Core.Domain.Common
Imports ALOD.Core.Domain.Documents
Imports ALOD.Core.Domain.Lookup
Imports ALOD.Core.Domain.Modules.Lod
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

Namespace Web.Special_Case.RW

    Partial Class Secure_sc_rw_MedOff
        Inherits System.Web.UI.Page

#Region "Fields..."

        Private _daoFactory As IDaoFactory
        Private _lookupDao As ILookupDao
        Private _memoDao As IMemoDao2
        Private _scAccess As Dictionary(Of String, ALOD.Core.Domain.Workflow.PageAccessType)
        Private _specCase As SC_RW = Nothing
        Private _specCaseDao As ISpecialCaseDAO

#End Region

#Region "Properties..."

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

        Protected ReadOnly Property AdminDisqualifiedDecisionValue As String
            Get
                Return CType(Finding.Admin_Disqualified, Integer).ToString()
            End Get
        End Property

        Protected ReadOnly Property AdminLODDecisionValue As String
            Get
                Return CType(Finding.Admin_LOD, Integer).ToString()
            End Get
        End Property

        Protected ReadOnly Property AdminQualifiedDecisionValue As String
            Get
                Return CType(Finding.Admin_Qualified, Integer).ToString()
            End Get
        End Property

        Protected ReadOnly Property CalendarImage() As String
            Get
                Return Me.ResolveClientUrl("~/App_Themes/" + Page.Theme + "/Images/Calendar.gif")
            End Get
        End Property

        Protected ReadOnly Property CanBoardMedEdit As Boolean
            Get
                If (UserCanEdit AndAlso SessionInfo.SESSION_GROUP_ID = UserGroups.BoardMedical) Then
                    Return True
                Else
                    Return False
                End If
            End Get
        End Property

        Protected ReadOnly Property CanHQAFRCTechEdit As Boolean
            Get
                If (UserCanEdit AndAlso
                    SessionInfo.SESSION_GROUP_ID = UserGroups.AFRCHQTechnician AndAlso
                    SpecCase.CurrentStatusCode = SpecCaseRWStatusCode.FinalReview AndAlso
                    (SpecCase.GetActiveBoardMedicalFinding().HasValue() AndAlso
                    (SpecCase.GetActiveBoardMedicalFinding().Value = Finding.Disqualify OrElse SpecCase.GetActiveBoardMedicalFinding().Value = Finding.Admin_Disqualified))) Then
                    Return True
                Else
                    Return False
                End If
            End Get
        End Property

        Protected ReadOnly Property CanSeniorMedicalReviewerEdit As Boolean
            Get
                If (UserCanEdit AndAlso SessionInfo.SESSION_GROUP_ID = UserGroups.SeniorMedicalReviewer) Then
                    Return True
                Else
                    Return False
                End If
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

        Protected ReadOnly Property DisqualifyDecisionValue As String
            Get
                Return CType(Finding.Disqualify, Integer).ToString()
            End Get
        End Property

        Protected ReadOnly Property InfiniteDateString As String
            Get
                Return "12/31/2100"
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

        Protected ReadOnly Property MasterPage() As SC_RWMaster
            Get
                Dim master As SC_RWMaster = CType(Page.Master, SC_RWMaster)
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

        Protected ReadOnly Property ModuleType() As ModuleType
            Get
                Return ModuleType.SpecCaseRW
            End Get
        End Property

        Protected ReadOnly Property Navigator() As TabNavigator
            Get
                Return MasterPage.Navigator
            End Get
        End Property

        Protected ReadOnly Property QualifyDecisionValue As String
            Get
                Return CType(Finding.Qualify_RTD, Integer).ToString()
            End Get
        End Property

        Protected ReadOnly Property RefId() As Integer
            Get
                Return CInt(Request.QueryString("refId"))
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

        Protected ReadOnly Property SpecCase() As SC_RW
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

        Protected ReadOnly Property TabControl() As TabControls
            Get
                Return Master.TabControl
            End Get
        End Property

#End Region

#Region "Page Methods..."

        Public Function ValidBoxLength() As Boolean
            Dim IsValid As Boolean = True

            If (Not CheckTextLength(txtDiagnosisTextBox)) Then
                IsValid = False
            End If

            If (Not CheckTextLength(txtDecisionComment)) Then
                IsValid = False
            End If

            If (Not CheckTextLength(txtDQParagraph)) Then
                IsValid = False
            End If

            If (Not CheckTextLength(txtSeniorMedicalReviewerDiagnosisText)) Then
                IsValid = False
            End If

            If (Not CheckTextLength(txtSeniorMedicalReviewerDecisionComment)) Then
                IsValid = False
            End If

            If (Not CheckTextLength(txtSeniorMedicalReviewerDQParagraph)) Then
                IsValid = False
            End If

            Return IsValid
        End Function

        Protected Sub ddlProcessAs_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ddlProcessAs.SelectedIndexChanged
            InitMemoDropdownList(GetInitDropdownListDtoForMedicalOfficer())
        End Sub

        Protected Sub ddlSeniorMedicalReviewerProcessAs_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ddlSeniorMedicalReviewerProcessAs.SelectedIndexChanged
            InitMemoDropdownList(GetInitDropdownListDtoForSeniorMedicalReviewer())
        End Sub

        Protected Sub InitControls()
            SetInputFormatRestrictions()
            SetAccessForControls()
            LoadFindings()
            SetMaxLengthForControls()
            SetControlsVisibility()
            SetFindingsPanelIndentation()
        End Sub

        Protected Sub InitDateTextBoxes()
            InitExpriationDateTextbox()

            txtReturnToDutyDate.CssClass = "datePickerFuture"

            WebControlSetters.SetDateTextbox(txtReturnToDutyDate, SpecCase.ReturnToDutyDate)
        End Sub

        Protected Sub InitExpriationDateTextbox()
            txtExpirationDate.CssClass = "datePickerPlusFuture"
            WebControlSetters.SetDateTextbox(txtExpirationDate, SpecCase.ExpirationDate)

            chbIndefinite.Attributes.Add("onclick", "SetIndefinite('" & txtExpirationDate.UniqueID & "')")

            If txtExpirationDate.Text = "12/31/2100" Then
                chbIndefinite.Checked = True
            Else
                chbIndefinite.Checked = False
            End If
        End Sub

        Protected Sub InitFindingRadioButtonsList(groupId As UserGroups, decisionsControl As RadioButtonList, findingId As Nullable(Of Integer))
            decisionsControl.DataSource = LookupDao.GetWorkflowFindings(SpecCase.Workflow, groupId)
            decisionsControl.DataValueField = "Id"
            decisionsControl.DataTextField = "Description"
            decisionsControl.DataBind()

            If (findingId.HasValue) Then
                decisionsControl.SelectedValue = findingId.Value
            End If

            Dim controlIdInfo = String.Empty

            If (groupId = UserGroups.SeniorMedicalReviewer) Then
                controlIdInfo = "SeniorMedicalReviewer"
            End If

            decisionsControl.Attributes.Add("onclick", "updateCSSClassForDecisionDependentLabels('" & decisionsControl.UniqueID & "', '" & controlIdInfo & "');")
        End Sub

        Protected Sub InitMedicalOfficerTextBoxes()
            WebControlSetters.SetTextboxText(txtDecisionComment, SpecCase.med_off_approval_comment)
            WebControlSetters.SetTextboxText(txtDQParagraph, SpecCase.DQParagraph)
        End Sub

        Protected Sub InitSeniorMedicalReviewerDateTextBoxes()
            InitSeniorMedicalReviewerExpriationDateTextbox()

            txtSeniorMedicalReviewerReturnToDutyDate.CssClass = "datePickerFuture"

            WebControlSetters.SetDateTextbox(txtSeniorMedicalReviewerReturnToDutyDate, SpecCase.AlternateReturnToDutyDate)
        End Sub

        Protected Sub InitSeniorMedicalReviewerExpriationDateTextbox()
            txtSeniorMedicalReviewerExpirationDate.CssClass = "datePickerPlusFuture"
            WebControlSetters.SetDateTextbox(txtSeniorMedicalReviewerExpirationDate, SpecCase.AlternateExpirationDate)

            chbSeniorMedicalReviewerIndefinite.Attributes.Add("onclick", "SetIndefinite('" & txtSeniorMedicalReviewerExpirationDate.UniqueID & "')")

            If (txtSeniorMedicalReviewerExpirationDate.Text = "12/31/2100") Then
                chbSeniorMedicalReviewerIndefinite.Checked = True
            Else
                chbSeniorMedicalReviewerIndefinite.Checked = False
            End If
        End Sub

        Protected Sub InitSeniorMedicalReviewerTextBoxes()
            WebControlSetters.SetTextboxText(txtSeniorMedicalReviewerDecisionComment, SpecCase.SeniorMedicalReviewerComment)
            WebControlSetters.SetTextboxText(txtSeniorMedicalReviewerDQParagraph, SpecCase.AlternateDQParagraph)
        End Sub

        Protected Sub Page_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
            AddHandler Me.Master.TabClick, AddressOf TabButtonClicked
        End Sub

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
            InitializeUserControls()

            If (Not IsPostBack) Then
                UserCanEdit = GetAccess(Navigator.PageAccess, True)
                InitControls()
                LogManager.LogAction(ModuleType, UserAction.ViewPage, RefId, "Viewed Page: Board Med")
            End If
        End Sub

        Protected Sub rblDecision_SelectedIndexChanged(sender As Object, e As EventArgs) Handles rblDecision.SelectedIndexChanged
            Dim args As RwMedicalOfficerTabInitDropdownListDto = GetInitDropdownListDtoForMedicalOfficer()
            InitProcessAsDropdownList(False, args)
            InitMemoDropdownList(args)
        End Sub

        Protected Sub rblSeniorMedicalReviewerDecision_SelectedIndexChanged(sender As Object, e As EventArgs) Handles rblSeniorMedicalReviewerDecision.SelectedIndexChanged
            SetSeniorMedicalReviewerFindingsPanelVisibility()
        End Sub

        Protected Sub rblSeniorMedicalReviewerFindings_SelectedIndexChanged(sender As Object, e As EventArgs) Handles rblSeniorMedicalReviewerFindings.SelectedIndexChanged
            Dim args As RwMedicalOfficerTabInitDropdownListDto = GetInitDropdownListDtoForSeniorMedicalReviewer()
            InitProcessAsDropdownList(False, args)
            InitMemoDropdownList(args)
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

        Protected Sub SetReadWriteAccessForHqAfrcControlsControls()
            If (SpecCase.ShouldUseSeniorMedicalReviewerFindings()) Then
                txtSeniorMedicalReviewerExpirationDate.Enabled = True
                txtSeniorMedicalReviewerReturnToDutyDate.Enabled = True
                ddlSeniorMedicalReviewerAssignmentLimitationCode.Enabled = True
                ddlSeniorMedicalReviewerMemos.Enabled = True

                txtSeniorMedicalReviewerExpirationDate.CssClass = "datePickerPlusFuture"
                txtSeniorMedicalReviewerReturnToDutyDate.CssClass = "datePickerFuture"
            Else
                txtExpirationDate.Enabled = True
                txtReturnToDutyDate.Enabled = True
                ddlAssignmentLimitationCode.Enabled = True
                ddlMemos.Enabled = True

                txtExpirationDate.CssClass = "datePickerPlusFuture"
                txtReturnToDutyDate.CssClass = "datePickerFuture"
            End If
        End Sub

        Protected Sub TabButtonClicked(ByRef sender As Object, ByRef e As TabNavigationEventArgs)
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

        Private Function ExtractDQMemoTemplateByTitle(memoTemplates As IList(Of MemoTemplate), templateTitle As String) As IList(Of MemoTemplate)
            If (String.IsNullOrEmpty(templateTitle)) Then
                Return memoTemplates
            End If

            Return (From memoTemplate In memoTemplates
                    Where memoTemplate.Title.Equals(templateTitle)
                    Select memoTemplate).ToList()
        End Function

        Private Function ExtractDQMemoTemplates(memoTemplates As IList(Of MemoTemplate), args As RwMedicalOfficerTabInitDropdownListDto) As IList(Of MemoTemplate)
            If (args.ProcessAsDropDownList.SelectedValue = SC_RW.PROCESS_AS_FULLWWD) Then
                Return ExtractDQMemoTemplateByTitle(memoTemplates, SC_RW.WORKFLOW_TITLE & " DQ Letter WD")
            End If

            If (args.ProcessAsDropDownList.SelectedValue = SC_RW.PROCESS_AS_FULLMEB) Then
                Return ExtractDQMemoTemplateByTitle(memoTemplates, SC_RW.WORKFLOW_TITLE & " DQ Letter MEB")
            End If

            If (args.ProcessAsDropDownList.SelectedValue = SC_RW.PROCESS_AS_FULLCASEDIRECTED) Then
                Return ExtractDQMemoTemplateByTitle(memoTemplates, SC_RW.WORKFLOW_TITLE & " DQ Pending LOD")
            End If

            Return Nothing
        End Function

        Private Function ExtractNonDQMemoTemplates(memoTemplates As IList(Of MemoTemplate)) As IList(Of MemoTemplate)
            Return (From memoTemplate In memoTemplates
                    Where (Not memoTemplate.Title.Contains("DQ"))
                    Select memoTemplate).ToList()
        End Function

        Private Function GetInitDropdownListDtoForMedicalOfficer() As RwMedicalOfficerTabInitDropdownListDto
            Dim dto As New RwMedicalOfficerTabInitDropdownListDto()

            dto.DecisionRadioButtonList = rblDecision

            dto.AssignmentLimitationCodeDropDownList = ddlAssignmentLimitationCode
            dto.MemosDropDownList = ddlMemos
            dto.ProcessAsDropDownList = ddlProcessAs

            dto.AlcLetterType = SpecCase.ALCLetterType
            dto.MemoTemplateId = SpecCase.MemoTemplateID
            dto.ProcessAs = SpecCase.ProcessAs

            Return dto
        End Function

        Private Function GetInitDropdownListDtoForSeniorMedicalReviewer() As RwMedicalOfficerTabInitDropdownListDto
            Dim dto As New RwMedicalOfficerTabInitDropdownListDto()

            dto.DecisionRadioButtonList = rblSeniorMedicalReviewerFindings

            dto.AssignmentLimitationCodeDropDownList = ddlSeniorMedicalReviewerAssignmentLimitationCode
            dto.MemosDropDownList = ddlSeniorMedicalReviewerMemos
            dto.ProcessAsDropDownList = ddlSeniorMedicalReviewerProcessAs

            dto.AlcLetterType = SpecCase.AlternateALCLetterType
            dto.MemoTemplateId = SpecCase.AlternateMemoTemplateID
            dto.ProcessAs = SpecCase.AlternateProcessAs

            Return dto
        End Function

        Private Function GetMemoTemplatesDataSource(args As RwMedicalOfficerTabInitDropdownListDto) As IList(Of MemoTemplate)
            If (String.IsNullOrEmpty(args.DecisionRadioButtonList.SelectedValue)) Then
                Return Nothing
            End If

            Dim allRWMemoTemplates As IList(Of MemoTemplate) = MemoDao.GetTemplatesByModule(ModuleType)

            If (args.DecisionRadioButtonList.SelectedValue = Finding.Disqualify OrElse args.DecisionRadioButtonList.SelectedValue = Finding.Admin_Disqualified) Then
                If (String.IsNullOrEmpty(args.ProcessAsDropDownList.SelectedValue)) Then
                    Return Nothing
                End If

                Return ExtractDQMemoTemplates(allRWMemoTemplates, args)
            End If

            Return ExtractNonDQMemoTemplates(allRWMemoTemplates)
        End Function

        Private Sub InitDropdownLists(args As RwMedicalOfficerTabInitDropdownListDto)
            InitProcessAsDropdownList(True, args)

            If (args.AlcLetterType.HasValue) Then
                args.AssignmentLimitationCodeDropDownList.SelectedValue = args.AlcLetterType.Value
            End If

            InitMemoDropdownList(args)
        End Sub

        Private Sub InitializeUserControls()
            ucICDCodeControl.Initialilze(Me)
            ucICD7thCharacterControl.Initialize(ucICDCodeControl)

            ucSeniorMedicalReviewerICDCodeControl.Initialilze(Me)
            ucSeniorMedicalReviewerICD7thCharacterControl.Initialize(ucSeniorMedicalReviewerICDCodeControl)
        End Sub

        Private Sub InitMemoDropdownList(args As RwMedicalOfficerTabInitDropdownListDto)
            args.MemosDropDownList.Items.Clear()

            Dim memoTemplateDataSource As IList(Of MemoTemplate) = GetMemoTemplatesDataSource(args)

            If (memoTemplateDataSource IsNot Nothing) Then
                args.MemosDropDownList.DataSource = memoTemplateDataSource
                args.MemosDropDownList.DataValueField = "Id"
                args.MemosDropDownList.DataTextField = "Title"
                args.MemosDropDownList.DataBind()
            End If

            Utility.InsertDropDownListZeroValue(args.MemosDropDownList, "-- Select a Memo --")

            If (args.MemoTemplateId.HasValue AndAlso args.MemosDropDownList.Items.FindByValue(args.MemoTemplateId.Value) IsNot Nothing) Then
                args.MemosDropDownList.SelectedValue = args.MemoTemplateId.Value
            Else
                args.MemosDropDownList.SelectedValue = 0
            End If
        End Sub

        Private Sub InitProcessAsDropdownList(setSelectedValue As Boolean, args As RwMedicalOfficerTabInitDropdownListDto)
            If (ShouldProcessAsDropdownListBeEnabled(args)) Then
                args.ProcessAsDropDownList.Enabled = True
            Else
                args.ProcessAsDropDownList.SelectedValue = 0
                args.ProcessAsDropDownList.Enabled = False
            End If

            If (setSelectedValue AndAlso args.ProcessAs.HasValue) Then
                args.ProcessAsDropDownList.SelectedValue = args.ProcessAs.Value
            End If
        End Sub

        Private Sub InitSeniorMedicalReviewerConcurDecisionControl()
            If (Not String.IsNullOrEmpty(SpecCase.SeniorMedicalReviewerConcur)) Then
                rblSeniorMedicalReviewerDecision.SelectedValue = Server.HtmlDecode(SpecCase.SeniorMedicalReviewerConcur)
            End If
        End Sub

        Private Sub LoadBoardMedicalFindings()
            InitFindingRadioButtonsList(UserGroups.BoardMedical, rblDecision, SpecCase.med_off_approved)
            InitMedicalOfficerTextBoxes()
            InitDateTextBoxes()
            InitDropdownLists(GetInitDropdownListDtoForMedicalOfficer())
            LoadIcdControls(ucICDCodeControl, txtDiagnosisTextBox, ucICD7thCharacterControl)
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

            icdDiagnosisTextBox.Text = Server.HtmlDecode(SpecCase.ICD9Diagnosis)
        End Sub

        Private Sub LoadSeniorMedicalReviewerFindings()
            InitSeniorMedicalReviewerConcurDecisionControl()
            InitFindingRadioButtonsList(UserGroups.SeniorMedicalReviewer, rblSeniorMedicalReviewerFindings, SpecCase.SeniorMedicalReviewerApproved)
            InitSeniorMedicalReviewerTextBoxes()
            InitSeniorMedicalReviewerDateTextBoxes()
            InitDropdownLists(GetInitDropdownListDtoForSeniorMedicalReviewer())
            LoadIcdControls(ucSeniorMedicalReviewerICDCodeControl, txtSeniorMedicalReviewerDiagnosisText, ucSeniorMedicalReviewerICD7thCharacterControl)
        End Sub

        Private Sub SaveBoardMedicalData()
            SaveIcdData(ucICDCodeControl, txtDiagnosisTextBox, ucICD7thCharacterControl)
            SaveBoardMedicalDecision()
            SaveBoardMedicalDates()
            SaveBoardMedicalDropdownListData()

            SpecCaseDao.SaveOrUpdate(SpecCase)
            SpecCaseDao.CommitChanges()
        End Sub

        Private Sub SaveBoardMedicalDates()
            If (Not String.IsNullOrEmpty(txtExpirationDate.Text)) Then
                SpecCase.ExpirationDate = Server.HtmlEncode(txtExpirationDate.Text)
            End If

            If (txtExpirationDate.Text.Equals(InfiniteDateString)) Then
                chbIndefinite.Checked = True
            Else
                chbIndefinite.Checked = False
            End If

            If (Not String.IsNullOrEmpty(txtReturnToDutyDate.Text)) Then
                SpecCase.ReturnToDutyDate = Server.HtmlEncode(txtReturnToDutyDate.Text)
            End If
        End Sub

        Private Sub SaveBoardMedicalDecision()
            If (Not String.IsNullOrEmpty(rblDecision.SelectedValue)) Then
                SpecCase.med_off_approved = rblDecision.SelectedValue

                If (rblDecision.SelectedValue = Finding.Disqualify OrElse rblDecision.SelectedValue = Finding.Admin_Disqualified) Then
                    If (Not String.IsNullOrEmpty(txtDQParagraph.Text)) Then
                        SpecCase.DQParagraph = HTMLEncodeNulls(txtDQParagraph.Text.Trim())
                    End If

                    SpecCase.DQCompletionDate = Now
                End If

                If (rblDecision.SelectedValue = Finding.Qualify_RTD OrElse rblDecision.SelectedValue = Finding.Admin_LOD OrElse rblDecision.SelectedValue = Finding.Admin_Qualified) Then
                    SpecCase.DQParagraph = String.Empty
                    SpecCase.DQCompletionDate = Nothing 'If the case is "Returned" to the Board Medical and decides to change the determination
                End If
            End If

            If (Not String.IsNullOrEmpty(txtDecisionComment.Text)) Then
                SpecCase.med_off_approval_comment = HTMLEncodeNulls(txtDecisionComment.Text)
            End If
        End Sub

        Private Sub SaveBoardMedicalDropdownListData()
            If (Not String.IsNullOrEmpty(ddlAssignmentLimitationCode.SelectedValue)) Then
                SpecCase.ALCLetterType = ddlAssignmentLimitationCode.SelectedValue
            End If

            SpecCase.ProcessAs = Utility.GetDropDownListNullableSelectedValue(ddlProcessAs, 0)
            SpecCase.MemoTemplateID = Utility.GetDropDownListNullableSelectedValue(ddlMemos, 0)
        End Sub

        Private Sub SaveData()
            If (Not UserCanEdit) Then
                Exit Sub
            End If

            If (CanBoardMedEdit) Then
                SaveBoardMedicalData()
            End If

            If (CanHQAFRCTechEdit) Then
                SaveHqAfrcTechnicianData()
            End If

            If (CanSeniorMedicalReviewerEdit) Then
                SaveSeniorMedicalReviewerData()
            End If
        End Sub

        Private Sub SaveHqAfrcTechnicianData()
            If (SpecCase.ShouldUseSeniorMedicalReviewerFindings()) Then
                SaveSeniorMedicalReviewerDates()
                SaveSeniorMedicalReviewerDropdownListData()
            Else
                SaveBoardMedicalDates()
                SaveBoardMedicalDropdownListData()
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

        Private Sub SaveSeniorMedicalReviewerAdditionalData()
            SaveSeniorMedicalReviewerDQData()
            SaveIcdData(ucSeniorMedicalReviewerICDCodeControl, txtSeniorMedicalReviewerDiagnosisText, ucSeniorMedicalReviewerICD7thCharacterControl)
            SaveSeniorMedicalReviewerDates()
            SaveSeniorMedicalReviewerDropdownListData()
        End Sub

        Private Sub SaveSeniorMedicalReviewerData()
            SaveSeniorMedicalReviewerDecision()
            SaveSeniorMedicalReviewerFinding()
            SaveSeniorMedicalReviewerDecisionComment()
            SaveSeniorMedicalReviewerAdditionalData()

            SpecCaseDao.SaveOrUpdate(SpecCase)
            SpecCaseDao.CommitChanges()
        End Sub

        Private Sub SaveSeniorMedicalReviewerDates()
            If (Not String.IsNullOrEmpty(txtSeniorMedicalReviewerExpirationDate.Text)) Then
                SpecCase.AlternateExpirationDate = Server.HtmlEncode(txtSeniorMedicalReviewerExpirationDate.Text)
            End If

            If (txtSeniorMedicalReviewerExpirationDate.Text.Equals(InfiniteDateString)) Then
                chbSeniorMedicalReviewerIndefinite.Checked = True
            Else
                chbSeniorMedicalReviewerIndefinite.Checked = False
            End If

            If (Not String.IsNullOrEmpty(txtSeniorMedicalReviewerReturnToDutyDate.Text)) Then
                SpecCase.AlternateReturnToDutyDate = Server.HtmlEncode(txtSeniorMedicalReviewerReturnToDutyDate.Text)
            End If
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

        Private Sub SaveSeniorMedicalReviewerDQData()
            If (Not SpecCase.SeniorMedicalReviewerApproved.HasValue) Then
                Exit Sub
            End If

            If (SpecCase.SeniorMedicalReviewerApproved.Value = Finding.Disqualify OrElse SpecCase.SeniorMedicalReviewerApproved.Value = Finding.Admin_Disqualified) Then
                If (Not String.IsNullOrEmpty(txtSeniorMedicalReviewerDQParagraph.Text)) Then
                    SpecCase.AlternateDQParagraph = HTMLEncodeNulls(txtSeniorMedicalReviewerDQParagraph.Text.Trim())
                End If

                SpecCase.AlternateDQCompletionDate = Now
            End If

            If (SpecCase.SeniorMedicalReviewerApproved.Value = Finding.Qualify_RTD OrElse SpecCase.SeniorMedicalReviewerApproved.Value = Finding.Admin_LOD OrElse SpecCase.SeniorMedicalReviewerApproved.Value = Finding.Admin_Qualified) Then
                SpecCase.AlternateDQParagraph = String.Empty
                SpecCase.AlternateDQCompletionDate = Nothing 'If the case is "Returned" to the Senior Medical Reviewer and decides to change the determination
            End If
        End Sub

        Private Sub SaveSeniorMedicalReviewerDropdownListData()
            If (Not String.IsNullOrEmpty(ddlSeniorMedicalReviewerAssignmentLimitationCode.SelectedValue)) Then
                SpecCase.AlternateALCLetterType = ddlSeniorMedicalReviewerAssignmentLimitationCode.SelectedValue
            End If

            SpecCase.AlternateProcessAs = Utility.GetDropDownListNullableSelectedValue(ddlSeniorMedicalReviewerProcessAs, 0)
            SpecCase.AlternateMemoTemplateID = Utility.GetDropDownListNullableSelectedValue(ddlSeniorMedicalReviewerMemos, 0)
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
            SetInputFormatRestriction(Page, txtDiagnosisTextBox, FormatRestriction.AlphaNumeric, PERMITTED_SPECIAL_CHAR_INPUT)
            SetInputFormatRestriction(Page, txtDecisionComment, FormatRestriction.AlphaNumeric, PERMITTED_SPECIAL_CHAR_INPUT)
            SetInputFormatRestriction(Page, txtDQParagraph, FormatRestriction.AlphaNumeric, PERMITTED_SPECIAL_CHAR_INPUT)
            SetInputFormatRestriction(Page, txtExpirationDate, FormatRestriction.Numeric, "/")
            SetInputFormatRestriction(Page, txtReturnToDutyDate, FormatRestriction.Numeric, "/")

            SetInputFormatRestriction(Page, txtSeniorMedicalReviewerDecisionComment, FormatRestriction.AlphaNumeric, PERMITTED_SPECIAL_CHAR_INPUT)
            SetInputFormatRestriction(Page, txtSeniorMedicalReviewerDiagnosisText, FormatRestriction.AlphaNumeric, PERMITTED_SPECIAL_CHAR_INPUT)
            SetInputFormatRestriction(Page, txtSeniorMedicalReviewerDQParagraph, FormatRestriction.AlphaNumeric, PERMITTED_SPECIAL_CHAR_INPUT)
            SetInputFormatRestriction(Page, txtSeniorMedicalReviewerExpirationDate, FormatRestriction.Numeric, "/")
            SetInputFormatRestriction(Page, txtSeniorMedicalReviewerReturnToDutyDate, FormatRestriction.Numeric, "/")
        End Sub

        Private Sub SetMaxLengthForControls()
            SetMaxLength(txtDiagnosisTextBox)
            SetMaxLength(txtDecisionComment)
            SetMaxLength(txtDQParagraph)

            SetMaxLength(txtSeniorMedicalReviewerDiagnosisText)
            SetMaxLength(txtSeniorMedicalReviewerDecisionComment)
            SetMaxLength(txtSeniorMedicalReviewerDQParagraph)
        End Sub

        Private Sub SetReadOnlyAccessForBoardMedicalOfficerControls()
            ucICDCodeControl.DisplayReadOnly(False)
            ucICD7thCharacterControl.DisplayReadOnly()

            txtDiagnosisTextBox.ReadOnly = True
            txtDiagnosisTextBox.Enabled = False
            rblDecision.Enabled = False
            txtDecisionComment.ReadOnly = True
            txtDecisionComment.Enabled = False
            txtDQParagraph.ReadOnly = True
            txtDQParagraph.Enabled = False
            txtExpirationDate.Enabled = False
            txtReturnToDutyDate.Enabled = False
            ddlAssignmentLimitationCode.Enabled = False
            ddlMemos.Enabled = False

            txtReturnToDutyDate.CssClass = ""
            txtExpirationDate.CssClass = ""
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

        Private Sub SetReadWriteAccessForBoardMedicalOfficerControls()
            ucICDCodeControl.DisplayReadWrite(False)
            ucICD7thCharacterControl.DisplayReadWrite()
            txtDiagnosisTextBox.ReadOnly = False
            txtDiagnosisTextBox.Enabled = True
            rblDecision.Enabled = True
            txtDecisionComment.ReadOnly = False
            txtDecisionComment.Enabled = True
            txtDQParagraph.ReadOnly = False
            txtDQParagraph.Enabled = True
            txtExpirationDate.Enabled = True
            txtReturnToDutyDate.Enabled = True
            ddlAssignmentLimitationCode.Enabled = True
            ddlMemos.Enabled = True
        End Sub

        Private Sub SetReadWriteAccessForControls()
            If (CanBoardMedEdit) Then
                SetReadWriteAccessForBoardMedicalOfficerControls()
            End If

            If (CanHQAFRCTechEdit) Then
                SetReadWriteAccessForHqAfrcControlsControls()
            End If

            If (CanSeniorMedicalReviewerEdit) Then
                SetReadWriteAccessForSeniorMedicalReviewerControls()
            End If
        End Sub

        Private Sub SetReadWriteAccessForSeniorMedicalReviewerControls()
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

        Private Function ShouldProcessAsDropdownListBeEnabled(args As RwMedicalOfficerTabInitDropdownListDto) As Boolean
            If (Not String.IsNullOrEmpty(args.DecisionRadioButtonList.SelectedValue) AndAlso (args.DecisionRadioButtonList.SelectedValue = Finding.Disqualify OrElse args.DecisionRadioButtonList.SelectedValue = Finding.Admin_Disqualified)) Then
                Return True
            End If

            Return False
        End Function

#End Region

    End Class

End Namespace