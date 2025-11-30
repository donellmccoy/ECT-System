Imports ALOD.Core.Domain.Common
Imports ALOD.Core.Domain.Lookup
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

    Partial Class Secure_sc_WD_MedTech
        Inherits System.Web.UI.Page

#Region "SC_WWDProperty"

        Private _ACDao As AssociatedCaseDao
        Private _HQTechSig As SignatureMetaData
        Private _MedTechSig As SignatureMetaData
        Private _scAccess As Dictionary(Of String, ALOD.Core.Domain.Workflow.PageAccessType)
        Private _sigDao As ISignatueMetaDateDao
        Dim dao As ISpecialCaseDAO

        Private sc As SC_WWD
        Private scId As Integer = 0

        ReadOnly Property AssociatedCaseDao() As IAssociatedCaseDao
            Get
                If (_ACDao Is Nothing) Then
                    _ACDao = New NHibernateDaoFactory().GetAssociatedCaseDao()
                End If

                Return _ACDao
            End Get
        End Property

        ReadOnly Property SCDao() As ISpecialCaseDAO
            Get
                If (dao Is Nothing) Then
                    dao = New NHibernateDaoFactory().GetSpecialCaseDAO()
                End If

                Return dao
            End Get
        End Property

        Public ReadOnly Property ReferenceId() As Integer
            Get
                Return Integer.Parse(Request.QueryString("refId"))
            End Get
        End Property

        Protected ReadOnly Property SpecCase() As SC_WWD
            Get
                If (sc Is Nothing) Then
                    sc = SCDao.GetById(ReferenceId)
                End If

                Return sc
            End Get
        End Property

        Private ReadOnly Property HQTechSig() As SignatureMetaData
            Get
                If (_HQTechSig Is Nothing) Then
                    _HQTechSig = SigDao.GetByWorkStatus(SpecCase.Id, SpecCase.Workflow, SpecCaseWWDWorkStatus.InitiateCase)
                End If

                Return _HQTechSig
            End Get
        End Property

        Private ReadOnly Property MedTechSig() As SignatureMetaData
            Get
                If (_MedTechSig Is Nothing) Then
                    _MedTechSig = SigDao.GetByWorkStatus(SpecCase.Id, SpecCase.Workflow, SpecCaseWWDWorkStatus.InitiateCase)
                End If

                Return _MedTechSig
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
                If (ViewState("CanEdit") Is Nothing) Then
                    ViewState("CanEdit") = False
                End If
                Return CBool(ViewState("CanEdit"))
            End Get
            Set(ByVal value As Boolean)
                ViewState("CanEdit") = value
            End Set
        End Property

        Protected ReadOnly Property CalendarImage() As String
            Get
                Return Me.ResolveClientUrl("~/App_Themes/" + Page.Theme + "/Images/Calendar.gif")
            End Get
        End Property

        Protected ReadOnly Property MasterPage() As SC_WWDMaster
            Get
                Dim master As SC_WWDMaster = CType(Page.Master, SC_WWDMaster)
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

        Private ReadOnly Property RefId() As Integer
            Get
                Return CInt(Request.QueryString("refId"))
            End Get
        End Property

        Public Sub DisplayReadWrite()
            'Read Only Fields

            If UserCanEdit Then
                'Date Fields - Style
                IPEBSignatureDate.CssClass = "datePicker"
                MUQRequestDate.CssClass = "datePicker"
                MUQUploadDate.CssClass = "datePicker"
                MEFSSignatureDate.CssClass = "datePicker"
                MEFSWaiverDate.CssClass = "datePicker"
                PS3811RequestDate.CssClass = "datePicker"
                PS3811SignatureDate.CssClass = "datePicker"
                FirstClassMailDate.CssClass = "datePicker"
                txtCode37Date.CssClass = "datePicker"

                ddlMedGroups.Enabled = True
                ddlRMUs.Enabled = True
                ucICDCodeControl.DisplayReadWrite(False)
                ucICD7thCharacterControl.DisplayReadWrite()
                DiagnosisTextBox.ReadOnly = False
                DiagnosisTextBox.Enabled = True

                'Documentation
                If Not SpecCase.AssociatedWWD.HasValue Then
                    DocumentsAttachedN.Enabled = True
                    DocumentsAttachedY.Enabled = True
                End If

                ' Cover letter choice commented out for unknown reason, added back in 10/17
                CoverLetterN.Enabled = True
                CoverLetterY.Enabled = True

                AFForm469N.Enabled = True
                AFForm469Y.Enabled = True
                txtCode37Date.Enabled = True
                NarrativeSummaryN.Enabled = True
                NarrativeSummaryY.Enabled = True
                IPEBElectionN.Enabled = True
                IPEBElectionY.Enabled = True
                IPEBRefusalN.Enabled = True
                IPEBRefusalY.Enabled = True
                IPEBSignatureDate.Enabled = True
                MUQRequestDate.Enabled = True
                MUQUploadDate.Enabled = True

                If SpecCase.MUQUploadDate.HasValue Then
                    MemberStatementY.Enabled = True 'change
                    MemberStatementN.Enabled = True 'change
                Else
                    MemberStatementY.Enabled = False 'change
                    MemberStatementN.Enabled = False 'change
                End If

                UnitCommanderMemoN.Enabled = True
                UnitCommanderMemoY.Enabled = True
                MEFSSignatureDate.Enabled = True
                MEFSWaiverDate.Enabled = True
                PrivatePhysicianN.Enabled = True
                PrivatePhysicianY.Enabled = True

                'Mailing Requests
                PS3811RequestDate.Enabled = True
                PS3811SignatureDate.Enabled = True
                PS3811UploadN.Enabled = True
                PS3811UploadY.Enabled = True
                FirstClassMailDate.Enabled = True
                MemberContactN.Enabled = True
                MemberContactY.Enabled = True
                MemberLetterN.Enabled = True
                MemberLetterY.Enabled = True
            Else
                DisplayReadOnly(0)
            End If
        End Sub

        Protected Sub DisplayReadOnly(ByVal ShowStoppers As Integer)
            If ShowStoppers <> 0 Then
                ' Do not hide show stopper questions
            Else
                'Do Nothing
            End If

            ddlMedGroups.Enabled = False
            ddlRMUs.Enabled = False
            ucICDCodeControl.DisplayReadOnly(False)
            ucICD7thCharacterControl.DisplayReadOnly()
            DiagnosisTextBox.ReadOnly = True
            '        DiagnosisTextBox.Enabled = False

            'Date Fields - Style
            IPEBSignatureDate.CssClass = ""
            MUQRequestDate.CssClass = ""
            MUQUploadDate.CssClass = ""
            MEFSSignatureDate.CssClass = ""
            MEFSWaiverDate.CssClass = ""
            PS3811RequestDate.CssClass = ""
            PS3811SignatureDate.CssClass = ""
            FirstClassMailDate.CssClass = ""
            txtCode37Date.CssClass = ""

            'Documentation
            DocumentsAttachedN.Enabled = False
            DocumentsAttachedY.Enabled = False
            CoverLetterN.Enabled = False
            CoverLetterY.Enabled = False
            AFForm469N.Enabled = False
            AFForm469Y.Enabled = False
            txtCode37Date.Enabled = False
            NarrativeSummaryN.Enabled = False
            NarrativeSummaryY.Enabled = False
            IPEBElectionN.Enabled = False
            IPEBElectionY.Enabled = False
            IPEBRefusalN.Enabled = False
            IPEBRefusalY.Enabled = False
            IPEBSignatureDate.Enabled = False
            MUQRequestDate.Enabled = False
            MUQUploadDate.Enabled = False
            MemberStatementN.Enabled = False
            MemberStatementY.Enabled = False
            UnitCommanderMemoN.Enabled = False
            UnitCommanderMemoY.Enabled = False
            MEFSSignatureDate.Enabled = False
            MEFSWaiverDate.Enabled = False
            PrivatePhysicianN.Enabled = False
            PrivatePhysicianY.Enabled = False

            'Mailing Requests
            PS3811RequestDate.Enabled = False
            PS3811SignatureDate.Enabled = False
            PS3811UploadN.Enabled = False
            PS3811UploadY.Enabled = False
            FirstClassMailDate.Enabled = False
            MemberContactN.Enabled = False
            MemberContactY.Enabled = False
            MemberLetterN.Enabled = False
            MemberLetterY.Enabled = False
        End Sub

        Protected Sub DocumentsAttachedN_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles DocumentsAttachedN.CheckedChanged
            If DocumentsAttachedN.Checked Then
                Documentation.Visible = False
                MailingRequests.Visible = True
            End If
        End Sub

        Protected Sub DocumentsAttachedY_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles DocumentsAttachedY.CheckedChanged
            If DocumentsAttachedY.Checked Then
                Documentation.Visible = True
                MailingRequests.Visible = False
            End If
        End Sub

        Protected Sub MemberStatementN_PreRender(sender As Object, e As System.EventArgs) Handles MemberStatementN.PreRender, MemberStatementY.PreRender
            If ((UserCanEdit) AndAlso (Trim(MUQUploadDate.Text) <> String.Empty)) Then 'change
                MemberStatementN.Enabled = True
                MemberStatementY.Enabled = True
            Else
                MemberStatementN.Enabled = False
                MemberStatementY.Enabled = False
            End If
        End Sub

        Protected Sub MUQUploadDate_Unload(sender As Object, e As System.EventArgs) Handles MUQUploadDate.Unload
            If ((UserCanEdit) AndAlso (Trim(MUQUploadDate.Text) <> String.Empty)) Then  'change
                MemberStatementN.Enabled = True
                MemberStatementY.Enabled = True
            Else
                MemberStatementN.Enabled = False
                MemberStatementY.Enabled = False
            End If
        End Sub

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

            ucICDCodeControl.Initialilze(Me)
            ucICD7thCharacterControl.Initialize(ucICDCodeControl)

            If (Not IsPostBack) Then
                SetPageAccess()
                InitControls()
                SetMaxLength(DiagnosisTextBox)

                LogManager.LogAction(ModuleType.SpecCaseWWD, UserAction.ViewPage, ReferenceId, "Viewed Page: Med Tech")

            End If

            If Session(SESSIONKEY_COMPO) = "6" Then
                Label5.Text = "RMU Name: "
            Else
                Label5.Text = "GMU Name: "
                Label15.Text = "Statement of Selection Date (Election or Refusal)"
                Label16.Text = "Commander’s Impact Statement Date"
                Label9.Text = "CIS Upload Date"
                pnlAFRC.Visible = False
                Label10.Text = "Election: "
                Label4.Text = "Refusal: "
                Label15.Text = "Signature Date (Election or Refusal): "
                Label19.Text = "Supporting Medical Documentation"
            End If

        End Sub

        Protected Sub SetPageAccess()

            ''if we don't have a case lock aquired, this will be read-only regardless of pageacess
            If (Not SESSION_LOCK_AQUIRED) Then
                UserCanEdit = False
                Exit Sub
            End If

            'otherwise, we have a case lock, so check page access
            Dim access = (From t In Navigator.PageAccess Where t.PageTitle = "WD Med Tech" Select t).SingleOrDefault()

            If (access Is Nothing) Then
                UserCanEdit = False
            Else
                If (access.Access = ALOD.Core.Domain.Workflow.PageAccess.AccessLevel.ReadWrite) Then
                    UserCanEdit = True
                Else
                    UserCanEdit = False
                    'Check if HQ Tech reopened expired WWD
                    If ((IsNothing(MedTechSig)) And
                        (Not IsNothing(AssociatedCaseDao.GetAssociatedCases(RefId, AFRCWorkflows.SpecCaseWWD))) And
                        (SESSION_GROUP_ID = UserGroups.AFRCHQTechnician) And
                        (SpecCase.Status = SpecCaseWWDWorkStatus.PackageReview)) Then

                        UserCanEdit = True
                    End If
                End If
            End If

        End Sub

        Private Sub InitControls()
            Dim uDao As UserDao = New NHibernateDaoFactory().GetUserDao()
            Dim currUser As AppUser = uDao.GetById(SESSION_USER_ID)
            Documentation.Visible = False
            MailingRequests.Visible = False

            Dim lkupDAO As ILookupDao
            lkupDAO = New NHibernateDaoFactory().GetLookupDao()

            SetInputFormatRestriction(Page, DiagnosisTextBox, FormatRestriction.AlphaNumeric, PERMITTED_SPECIAL_CHAR_INPUT)
            SetInputFormatRestriction(Page, txtCode37Date, FormatRestriction.Numeric, "/")
            SetInputFormatRestriction(Page, IPEBSignatureDate, FormatRestriction.Numeric, "/")
            SetInputFormatRestriction(Page, MUQRequestDate, FormatRestriction.Numeric, "/")
            SetInputFormatRestriction(Page, MUQUploadDate, FormatRestriction.Numeric, "/")
            SetInputFormatRestriction(Page, MEFSSignatureDate, FormatRestriction.Numeric, "/")
            SetInputFormatRestriction(Page, MEFSWaiverDate, FormatRestriction.Numeric, "/")
            SetInputFormatRestriction(Page, PS3811RequestDate, FormatRestriction.Numeric, "/")
            SetInputFormatRestriction(Page, PS3811SignatureDate, FormatRestriction.Numeric, "/")
            SetInputFormatRestriction(Page, FirstClassMailDate, FormatRestriction.Numeric, "/")

            ddlMedGroups.DataSource = From n In lkupDAO.GetMedGroupNames("") Select n
            ddlMedGroups.DataTextField = "Name"
            ddlMedGroups.DataValueField = "Value"
            ddlMedGroups.DataBind()
            Dim TopSelection As New ListItem()
            TopSelection.Text = "--- Select One ---"
            TopSelection.Value = 0
            ddlMedGroups.Items.Insert(0, TopSelection)
            Dim NASelection As New ListItem() ' selection if no Med Group
            NASelection.Text = "N/A"
            NASelection.Value = -1
            ddlMedGroups.Items.Insert(1, NASelection)

            If Not IsNothing(SpecCase.MedGroupName) Then
                ddlMedGroups.ClearSelection()
                ddlMedGroups.SelectedValue = SpecCase.MedGroupName
            End If

            If Session(SESSIONKEY_COMPO) = "6" Then
                ddlRMUs.DataSource = From n In lkupDAO.GetRMUNames("") Where n.Value < 100 Select n
            Else
                ddlRMUs.DataSource = From n In lkupDAO.GetRMUNames("") Where n.Value >= 100 Select n
            End If
            Dim TopSelection2 As New ListItem()
            TopSelection2.Text = "--- Select One ---"
            TopSelection2.Value = 0
            ddlRMUs.DataTextField = "Name"
            ddlRMUs.DataValueField = "Value"
            ddlRMUs.DataBind()
            ddlRMUs.Items.Insert(0, TopSelection2)
            If Not IsNothing(SpecCase.RMUName) Then
                ddlRMUs.ClearSelection()
                ddlRMUs.SelectedValue = SpecCase.RMUName
            End If

            'Load Findings
            If Not String.IsNullOrEmpty(SpecCase.PocUnit) Then
                POCUnitLabel.Text = SpecCase.PocUnit
            Else
                If UserCanEdit Then
                    POCUnitLabel.Text = currUser.CurrentUnitName  ' Need to get User's Unit (name)
                End If
            End If
            If Not String.IsNullOrEmpty(SpecCase.PocEmail) Then
                POCEmailLabel.Text = SpecCase.PocEmail
            Else
                If UserCanEdit Then
                    POCEmailLabel.Text = currUser.Email  ' Need to get User's Email
                End If
            End If
            If Not String.IsNullOrEmpty(SpecCase.PocPhoneDSN) Then
                POCPhoneLabel.Text = SpecCase.PocPhoneDSN
            Else
                If UserCanEdit Then
                    POCPhoneLabel.Text = currUser.Phone  ' Need to get User's Phone
                End If
            End If

            If Not IsNothing(SpecCase.ICD9Code) Then
                ucICDCodeControl.InitializeHierarchy(SpecCase.ICD9Code)

                If (ucICDCodeControl.IsValidICDCode(SpecCase.ICD9Code)) Then
                    ucICDCodeControl.UpdateICDCodeDiagnosisLabel(SpecCase.ICD9Code, True)
                    DiagnosisTextBox.Text = Server.HtmlDecode(SpecCase.ICD9Diagnosis)
                End If

                If (Not String.IsNullOrEmpty(SpecCase.ICD7thCharacter)) Then
                    ucICD7thCharacterControl.InitializeCharacters(SpecCase.ICD9Code, SpecCase.ICD7thCharacter)
                    ucICD7thCharacterControl.Update7thCharacterLabel(SpecCase.ICD9Code, SpecCase.ICD7thCharacter)
                Else
                    ucICD7thCharacterControl.InitializeCharacters(SpecCase.ICD9Code, String.Empty)
                End If
            Else
                ucICD7thCharacterControl.InitializeCharacters(0, String.Empty)
            End If

            Dim NameString As String = ""
            If SpecCase.AssociatedWWD.HasValue Then  'If a reopened case - use HQ Tech signature (if existing)
                Dim sig As SignatureMetaData = SigDao.GetByUserGroup(SpecCase.Id, SpecCase.Workflow, UserGroups.AFRCHQTechnician)
                If (Not sig Is Nothing) Then
                    NameString = sig.NameAndRank
                End If
                DocumentsAttachedY.Checked = True
                DocumentsAttachedN.Enabled = False
                DocumentsAttachedY.Enabled = False
            End If
            If Not IsNothing(MedTechSig) Then 'use Med Tech signature (if existing - overrides HQ Tech signature)
                NameString = MedTechSig.NameAndRank
            End If
            If ((String.IsNullOrEmpty(NameString)) And ((SESSION_GROUP_ID = UserGroups.AFRCHQTechnician) Or (SESSION_GROUP_ID = UserGroups.MedicalTechnician))) Then
                NameString = currUser.RankAndName 'if no signature, use curr User [assuming Med Tech or HQ Tech]
            End If

            POCNameLabel.Text = NameString
            If SpecCase.WWDDocsAttached = 1 Then
                DocumentsAttachedY.Checked = True
            End If
            If SpecCase.WWDDocsAttached = 0 Then
                DocumentsAttachedN.Checked = True
            End If

            'Show appropriate div
            If DocumentsAttachedN.Checked Then
                MailingRequests.Visible = True
            End If
            If DocumentsAttachedY.Checked Then
                Documentation.Visible = True
            End If

            'Documenation Attached
            If SpecCase.CoverLetterUploaded = 1 Then
                CoverLetterY.Checked = True
            End If
            If SpecCase.CoverLetterUploaded = 0 Then
                CoverLetterN.Checked = True
            End If
            If SpecCase.AFForm469Uploaded = 1 Then
                AFForm469Y.Checked = True
            End If
            If SpecCase.AFForm469Uploaded = 0 Then
                AFForm469N.Checked = True
            End If

            If SpecCase.Code37InitDate.HasValue Then
                txtCode37Date.Text = Server.HtmlDecode(SpecCase.Code37InitDate.Value.ToString(DATE_FORMAT))
            End If

            If SpecCase.NarrativeSummaryUploaded = 1 Then
                NarrativeSummaryY.Checked = True
            End If
            If SpecCase.NarrativeSummaryUploaded = 0 Then
                NarrativeSummaryN.Checked = True
            End If
            If SpecCase.IPEBElection = 1 Then
                IPEBElectionY.Checked = True
            End If
            If SpecCase.IPEBElection = 0 Then
                IPEBElectionN.Checked = True
            End If
            If SpecCase.IPEBRefusal = 1 Then
                IPEBRefusalY.Checked = True
            End If
            If SpecCase.IPEBRefusal = 0 Then
                IPEBRefusalN.Checked = True
            End If
            If SpecCase.IPEBSignatureDate.HasValue Then
                IPEBSignatureDate.Text = Server.HtmlDecode(SpecCase.IPEBSignatureDate.Value.ToString(DATE_FORMAT))
            End If
            If SpecCase.MUQRequestDate.HasValue Then
                MUQRequestDate.Text = Server.HtmlDecode(SpecCase.MUQRequestDate.Value.ToString(DATE_FORMAT))
            End If
            If SpecCase.MUQUploadDate.HasValue Then
                MUQUploadDate.Text = Server.HtmlDecode(SpecCase.MUQUploadDate.Value.ToString(DATE_FORMAT))
            End If
            If SpecCase.CoverLtrIncMemberStatement = True Then
                MemberStatementY.Checked = True
            End If
            If SpecCase.CoverLtrIncMemberStatement = False Then
                MemberStatementN.Checked = True
            End If
            If SpecCase.UnitCmdrMemoUploaded = 1 Then
                UnitCommanderMemoY.Checked = True
            End If
            If SpecCase.UnitCmdrMemoUploaded = 0 Then
                UnitCommanderMemoN.Checked = True
            End If
            If SpecCase.MedEvalFactSheetSignDate.HasValue Then
                MEFSSignatureDate.Text = Server.HtmlDecode(SpecCase.MedEvalFactSheetSignDate.Value.ToString(DATE_FORMAT))
            End If
            If SpecCase.MedEvalFSWaiverSignDate.HasValue Then
                MEFSWaiverDate.Text = Server.HtmlDecode(SpecCase.MedEvalFSWaiverSignDate.Value.ToString(DATE_FORMAT))
            End If
            If SpecCase.PrivatePhysicianDocsUploaded = 1 Then
                PrivatePhysicianY.Checked = True
            End If
            If SpecCase.PrivatePhysicianDocsUploaded = 0 Then
                PrivatePhysicianN.Checked = True
            End If

            'Mailing Requests
            If SpecCase.PS3811RequestDate.HasValue Then
                PS3811RequestDate.Text = Server.HtmlDecode(SpecCase.PS3811RequestDate.Value.ToString(DATE_FORMAT))
            End If
            If SpecCase.PS3811SignDate.HasValue Then
                PS3811SignatureDate.Text = Server.HtmlDecode(SpecCase.PS3811SignDate.Value.ToString(DATE_FORMAT))
            End If
            If SpecCase.PS3811Uploaded = 1 Then
                PS3811UploadY.Checked = True
            End If
            If SpecCase.PS3811Uploaded = 0 Then
                PS3811UploadN.Checked = True
            End If
            If SpecCase.FirstClassMailDate.HasValue Then
                FirstClassMailDate.Text = Server.HtmlDecode(SpecCase.FirstClassMailDate.Value.ToString(DATE_FORMAT))
            End If
            If SpecCase.CoverLtrIncContactAttemptDetails = 1 Or SpecCase.CoverLtrIncContactAttemptDetails = True Then
                MemberContactY.Checked = True
            End If
            If SpecCase.CoverLtrIncContactAttemptDetails = 0 Or SpecCase.CoverLtrIncContactAttemptDetails = False Then
                MemberContactN.Checked = True
            End If
            If SpecCase.MemberLetterUploaded = 1 Then
                MemberLetterY.Checked = True
            End If
            If SpecCase.MemberLetterUploaded = 0 Then
                MemberLetterN.Checked = True
            End If

            DisplayReadWrite()
        End Sub

        Private Sub SaveFindings()

            If (Not UserCanEdit) Then
                Exit Sub
            End If

            Dim access As ALOD.Core.Domain.Workflow.PageAccessType
            access = SectionList(WDSectionNames.WD_APPROVED.ToString())

            If (ucICDCodeControl.IsICDCodeSelected()) Then
                SpecCase.ICD9Code = ucICDCodeControl.SelectedICDCodeID
            Else
                'SpecCase.ICD9Code = Nothing
            End If

            If (ucICD7thCharacterControl.Is7thCharacterSelected()) Then
                SpecCase.ICD7thCharacter = ucICD7thCharacterControl.Selected7thCharacter
            Else
                SpecCase.ICD7thCharacter = Nothing
            End If

            SpecCase.ICD9Description = ucICDCodeControl.ICDCodeDiagnosisLabelText
            SpecCase.ICD9Diagnosis = Server.HtmlEncode(DiagnosisTextBox.Text.Trim())

            If (Not IsNothing(SpecCase.ICD9Code)) Then
                ucICDCodeControl.UpdateICDCodeDiagnosisLabel(SpecCase.ICD9Code)
            End If

            '*************************************************************

            'Read-Only values
            If ddlMedGroups.SelectedIndex > 0 Then
                SpecCase.MedGroupName = ddlMedGroups.SelectedValue
            End If

            If ddlRMUs.SelectedIndex > 0 Then
                SpecCase.RMUName = ddlRMUs.SelectedValue
            End If

            SpecCase.PocEmail = POCEmailLabel.Text
            SpecCase.PocUnit = POCUnitLabel.Text
            SpecCase.PocPhoneDSN = POCPhoneLabel.Text

            If DocumentsAttachedY.Checked Then
                SpecCase.WWDDocsAttached = 1
            End If
            If DocumentsAttachedN.Checked Then
                SpecCase.WWDDocsAttached = 0
            End If

            'Documenation Attached
            If CoverLetterY.Checked Then
                SpecCase.CoverLetterUploaded = 1
            End If
            If CoverLetterN.Checked Then
                SpecCase.CoverLetterUploaded = 0
            End If
            If AFForm469Y.Checked Then
                SpecCase.AFForm469Uploaded = 1
            End If
            If AFForm469N.Checked Then
                SpecCase.AFForm469Uploaded = 0
            End If

            If Not String.IsNullOrEmpty(txtCode37Date.Text) Then
                SpecCase.Code37InitDate = Server.HtmlEncode(txtCode37Date.Text)
            End If

            If NarrativeSummaryY.Checked Then
                SpecCase.NarrativeSummaryUploaded = 1
            End If
            If NarrativeSummaryN.Checked Then
                SpecCase.NarrativeSummaryUploaded = 0
            End If
            If IPEBElectionY.Checked Then
                SpecCase.IPEBElection = 1
            End If
            If IPEBElectionN.Checked Then
                SpecCase.IPEBElection = 0
            End If
            If IPEBRefusalY.Checked Then
                SpecCase.IPEBRefusal = 1
            End If
            If IPEBRefusalN.Checked Then
                SpecCase.IPEBRefusal = 0
            End If
            If Not String.IsNullOrEmpty(IPEBSignatureDate.Text) Then
                SpecCase.IPEBSignatureDate = Server.HtmlEncode(IPEBSignatureDate.Text)
            End If
            Dim canSubtractDates As Boolean = True
            If Not String.IsNullOrEmpty(MUQRequestDate.Text) Then
                SpecCase.MUQRequestDate = Server.HtmlEncode(MUQRequestDate.Text)
            Else
                canSubtractDates = False
                SpecCase.MUQ_Valid = False
            End If
            If Not String.IsNullOrEmpty(MUQUploadDate.Text) Then
                SpecCase.MUQUploadDate = Server.HtmlEncode(MUQUploadDate.Text)
            Else
                canSubtractDates = False
                SpecCase.MUQ_Valid = False
            End If

            If canSubtractDates Then
                Dim temp1 As DateTime = SpecCase.MUQUploadDate
                Dim temp2 As DateTime = SpecCase.MUQRequestDate
                Dim diff As TimeSpan = temp1.Subtract(temp2)
                If diff.Days > 60 Then
                    SpecCase.MUQ_Valid = False
                Else
                    SpecCase.MUQ_Valid = True
                End If
            End If

            If MemberStatementY.Checked Then
                SpecCase.CoverLtrIncMemberStatement = 1  'Translates to True
            End If
            If MemberStatementN.Checked Then
                SpecCase.CoverLtrIncMemberStatement = 0  'Translates to False
            End If
            If UnitCommanderMemoY.Checked Then
                SpecCase.UnitCmdrMemoUploaded = 1
            End If
            If UnitCommanderMemoN.Checked Then
                SpecCase.UnitCmdrMemoUploaded = 0
            End If
            If Not String.IsNullOrEmpty(MEFSSignatureDate.Text) Then
                SpecCase.MedEvalFactSheetSignDate = Server.HtmlEncode(MEFSSignatureDate.Text)
            End If
            If Not String.IsNullOrEmpty(MEFSWaiverDate.Text) Then
                SpecCase.MedEvalFSWaiverSignDate = Server.HtmlEncode(MEFSWaiverDate.Text)
            End If
            If PrivatePhysicianY.Checked Then
                SpecCase.PrivatePhysicianDocsUploaded = 1
            End If
            If PrivatePhysicianN.Checked Then
                SpecCase.PrivatePhysicianDocsUploaded = 0
            End If

            'Mailing Requests
            If Not String.IsNullOrEmpty(PS3811RequestDate.Text) Then
                SpecCase.PS3811RequestDate = Server.HtmlEncode(PS3811RequestDate.Text)
            End If
            If Not String.IsNullOrEmpty(PS3811SignatureDate.Text) Then
                SpecCase.PS3811SignDate = Server.HtmlEncode(PS3811SignatureDate.Text)
            End If
            If PS3811UploadY.Checked Then
                SpecCase.PS3811Uploaded = 1
            End If
            If PS3811UploadN.Checked Then
                SpecCase.PS3811Uploaded = 0
            End If
            If Not String.IsNullOrEmpty(FirstClassMailDate.Text) Then
                SpecCase.FirstClassMailDate = Server.HtmlEncode(FirstClassMailDate.Text)
            End If
            If MemberContactY.Checked Then
                SpecCase.CoverLtrIncContactAttemptDetails = 1
            End If
            If MemberContactN.Checked Then
                SpecCase.CoverLtrIncContactAttemptDetails = 0
            End If
            If MemberLetterY.Checked Then
                SpecCase.MemberLetterUploaded = 1
            End If
            If MemberLetterN.Checked Then
                SpecCase.MemberLetterUploaded = 0
            End If
            '***************************************************************
            SCDao.SaveOrUpdate(SpecCase)
            SCDao.CommitChanges()
        End Sub

#Region "TabEvent"

        Public Function ValidBoxLength() As Boolean
            Dim IsValid As Boolean = True
            If Not CheckTextLength(DiagnosisTextBox) Then
                IsValid = False
            End If
            Return IsValid
        End Function

        Protected Sub Page_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
            AddHandler Me.Master.TabClick, AddressOf TabButtonClicked
        End Sub

        Protected Sub Save_Click(ByRef sender As Object, ByRef e As TabNavigationEventArgs)

            If (Navigator.CurrentStep.IsReadOnly) Then
                Exit Sub
            End If

            If Not (ValidBoxLength()) Then
                e.Cancel = True
                Exit Sub
            End If

            If (e.ButtonType = NavigatorButtonType.Save OrElse e.ButtonType = NavigatorButtonType.NavigatedAway _
                OrElse e.ButtonType = NavigatorButtonType.NextStep OrElse e.ButtonType = NavigatorButtonType.PreviousStep) Then
                SaveFindings()
            End If

        End Sub

        Private Sub TabButtonClicked(ByRef sender As Object, ByRef e As TabNavigationEventArgs)

            If (e.ButtonType = NavigatorButtonType.Save OrElse
                e.ButtonType = NavigatorButtonType.NavigatedAway OrElse
                e.ButtonType = NavigatorButtonType.NextStep OrElse
                e.ButtonType = NavigatorButtonType.PreviousStep) Then
                SaveFindings()
            End If

            If Not (ValidBoxLength()) Then
                e.Cancel = True
                Exit Sub
            End If

        End Sub

#End Region

    End Class

End Namespace