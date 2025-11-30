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

Namespace Web.Special_Case.AGR

    Partial Class Secure_sc_agr_MedTech
        Inherits System.Web.UI.Page

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

#Region "LODProperty"

        Private _HQTechSig As SignatureMetaData
        Private _MedTechSig As SignatureMetaData
        Private _scAccess As Dictionary(Of String, ALOD.Core.Domain.Workflow.PageAccessType)
        Private _sigDao As ISignatueMetaDateDao
        Dim dao As ISpecialCaseDAO

        Private sc As SC_AGRCert = Nothing
        Private scId As Integer = 0

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

        Protected ReadOnly Property SpecCase() As SC_AGRCert
            Get
                If (sc Is Nothing) Then
                    sc = SCDao.GetById(ReferenceId)
                End If

                Return sc
            End Get
        End Property

#End Region

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
                    _MedTechSig = SigDao.GetByWorkStatus(SpecCase.Id, SpecCase.Workflow, SpecCaseAGRWorkStatus.InitiateCase)
                End If

                Return _MedTechSig
            End Get
        End Property

        Private ReadOnly Property RefId() As Integer
            Get
                Return CInt(Request.QueryString("refId"))
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

        Protected Sub DisplayReadOnly(ByVal ShowStoppers As Integer, ByVal isPostBack As Boolean)
            ddlRMUs.Enabled = False
            AFSCTextBox.Enabled = False
            PHADate.Enabled = False
            ALCN.Enabled = False
            ALCY.Enabled = False
            MAJCOMY.Enabled = False
            MAJCOMN.Enabled = False
            InitialTourN.Enabled = False
            InitialTourY.Enabled = False
            FollowOnN.Enabled = False
            FollowOnY.Enabled = False
            POCUnitLabel.Enabled = False
            POCAddressLabel.Enabled = False
            POCAddressLabelCityStateZip.Enabled = False
            POCNameLabel.Enabled = False
            POCEmailLabel.Enabled = False
            POCPhoneLabel.Enabled = False
        End Sub

        Protected Sub FollowOnN_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles FollowOnN.CheckedChanged
            InitialTourY.Checked = True
            InitialTourN.Checked = False
        End Sub

        Protected Sub FollowOnY_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles FollowOnY.CheckedChanged
            InitialTourY.Checked = False
            InitialTourN.Checked = True
        End Sub

        Protected Sub InitialTourN_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles InitialTourN.CheckedChanged
            FollowOnY.Checked = True
            FollowOnN.Checked = False
        End Sub

        Protected Sub InitialTourY_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles InitialTourY.CheckedChanged
            FollowOnY.Checked = False
            FollowOnN.Checked = True
        End Sub

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

            If (Not IsPostBack) Then
                UserCanEdit = GetAccess(Navigator.PageAccess, True)
                InitControls()
                'SetMaxLength(AFSCTextBox)

                'Validation
                If (UserCanEdit) Then
                    If (sc.Validations.Count > 0) Then
                        sc.Validate()
                        If (sc.Validations.Count > 0) Then
                            ShowPageValidationErrors(sc.Validations, Me)
                        End If
                    End If
                End If

                LogManager.LogAction(ModuleType.SpecCaseAGR, UserAction.ViewPage, RefId, "Viewed Page: AGR Certification Medical Technician")

            End If

        End Sub

        Private Sub InitControls()
            Dim uDao As UserDao = New NHibernateDaoFactory().GetUserDao()
            Dim currUser As AppUser = uDao.GetById(SESSION_USER_ID)

            ' Set textbox character restrictions
            SetInputFormatRestriction(Page, PHADate, FormatRestriction.Numeric, "/")
            SetInputFormatRestriction(Page, AFSCTextBox, FormatRestriction.AlphaNumeric, PERMITTED_SPECIAL_CHAR_INPUT)

            ALCLabel.Style("display") = "block"
            ALCLabel.Style("width") = "250px"
            MAJCOMLabel.Style("display") = "block"
            MAJCOMLabel.Style("width") = "250px"

            Dim lkupDAO As ILookupDao
            lkupDAO = New NHibernateDaoFactory().GetLookupDao()

            Dim TopSelection As New ListItem()
            TopSelection.Text = "--- Select One ---"
            TopSelection.Value = 0

            If Session(SESSIONKEY_COMPO) = "6" Then
                ddlRMUs.DataSource = From n In lkupDAO.GetRMUNames("") Where n.Value < 100 Select n
            Else
                ddlRMUs.DataSource = From n In lkupDAO.GetRMUNames("") Where n.Value >= 100 Select n
            End If

            ddlRMUs.DataTextField = "Name"
            ddlRMUs.DataValueField = "Value"
            ddlRMUs.DataBind()
            ddlRMUs.Items.Insert(0, TopSelection)

            If Not IsNothing(SpecCase.RMUName) Then
                ddlRMUs.SelectedValue = SpecCase.RMUName
            End If

            'Load Findings
            Dim NameString As String = ""
            Dim sig As SignatureMetaData = SigDao.GetByUserGroup(SpecCase.Id, SpecCase.Workflow, UserGroups.AFRCHQTechnician)
            If (Not sig Is Nothing) Then
                NameString = sig.NameAndRank
            End If
            If Not IsNothing(MedTechSig) Then 'use Med Tech signature (if existing - overrides HQ Tech signature)
                NameString = MedTechSig.NameAndRank
            End If
            If ((String.IsNullOrEmpty(NameString)) And ((SESSION_GROUP_ID = UserGroups.AFRCHQTechnician) Or (SESSION_GROUP_ID = UserGroups.MedicalTechnician))) Then
                NameString = currUser.RankAndName 'if no signature, use curr User [assuming Med Tech or HQ Tech]
            End If

            'POC
            If Not String.IsNullOrEmpty(SpecCase.PocUnit) Then
                POCUnitLabel.Text = SpecCase.PocUnit
            Else
                If UserCanEdit Then
                    POCUnitLabel.Text = currUser.CurrentUnitName  ' Need to get User's Unit (name)
                End If
            End If

            If Not String.IsNullOrEmpty(SpecCase.MTFInitialFacility) Then
                POCAddressLabel.Text = SpecCase.MTFInitialFacility
            Else
                If UserCanEdit Then
                    POCAddressLabel.Text = String.Empty  ' Need to get MTF Unit Address (name)
                End If
            End If

            If Not String.IsNullOrEmpty(SpecCase.MTFInitialFacilityCityStateZip) Then
                POCAddressLabelCityStateZip.Text = SpecCase.MTFInitialFacilityCityStateZip
            Else
                If UserCanEdit Then
                    POCAddressLabelCityStateZip.Text = String.Empty  ' Need to get MTF Unit Address City State Zip (name)
                End If
            End If

            If Not String.IsNullOrEmpty(SpecCase.PocEmail) Then
                POCEmailLabel.Text = SpecCase.PocEmail
                If UserCanEdit Then
                    POCEmailLabel.Text = SpecCase.PocEmail
                End If
            End If

            If Not String.IsNullOrEmpty(SpecCase.POCRankAndName) Then
                POCNameLabel.Text = SpecCase.POCRankAndName
            Else
                If UserCanEdit Then
                    If UserCanEdit Then
                        POCNameLabel.Text = currUser.RankAndName  ' Need to get User's Name
                    End If
                End If
            End If
            If Not String.IsNullOrEmpty(SpecCase.PocPhoneDSN) Then
                POCPhoneLabel.Text = SpecCase.PocPhoneDSN
            Else
                If UserCanEdit Then
                    POCPhoneLabel.Text = currUser.Phone  ' Need to get User's Phone
                End If
            End If

            If SpecCase.ALC IsNot Nothing And SpecCase.ALC = 1 Then
                ALCY.Checked = True
                If UserCanEdit Then
                    ALCY.Checked = True
                End If
            End If
            If SpecCase.ALC = 0 Then
                ALCN.Checked = True
            End If

            If Not IsNothing(SpecCase.AFSC) Then
                AFSCTextBox.Text = SpecCase.AFSC
                If UserCanEdit Then
                    AFSCTextBox.Text = SpecCase.AFSC
                End If
            End If

            If Not IsNothing(SpecCase.PHADate) Then
                PHADate.Text = SpecCase.PHADate
                If UserCanEdit Then
                    PHADate.Text = SpecCase.PHADate
                End If
            End If

            If SpecCase.MAJCOM IsNot Nothing And SpecCase.MAJCOM = 1 Then
                MAJCOMY.Checked = True
                If UserCanEdit Then
                    MAJCOMY.Checked = True
                End If
            End If
            If SpecCase.MAJCOM = 0 Then
                MAJCOMN.Checked = True
            End If

            ' Initial Tour/ Follow On Tour
            If SpecCase.InitialTour IsNot Nothing And SpecCase.InitialTour = 1 Then
                InitialTourY.Checked = True
            End If
            If SpecCase.InitialTour = 0 Then
                InitialTourN.Checked = True
            End If

            If SpecCase.FollowOnTour IsNot Nothing And SpecCase.FollowOnTour = 1 Then
                FollowOnY.Checked = True
            End If
            If SpecCase.FollowOnTour = 0 Then
                FollowOnN.Checked = True
            End If

            If UserCanEdit Then
                'Read/Write

                ddlRMUs.Enabled = True

                PHADate.Enabled = True
                PHADate.CssClass = "datePicker"
                AFSCTextBox.Enabled = True

                ALCN.Enabled = True
                ALCY.Enabled = True
                MAJCOMY.Enabled = True
                MAJCOMN.Enabled = True
                InitialTourN.Enabled = True
                InitialTourY.Enabled = True
                FollowOnN.Enabled = True
                FollowOnY.Enabled = True
                POCUnitLabel.Enabled = True
                POCAddressLabel.Enabled = True
                POCAddressLabelCityStateZip.Enabled = True
                POCNameLabel.Enabled = True
                POCEmailLabel.Enabled = True
                POCPhoneLabel.Enabled = True
            Else
                'Read Only
                DisplayReadOnly(0, True)
            End If

            'Load Findings
            If Not String.IsNullOrEmpty(SpecCase.PocEmail) Then
                POCEmailLabel.Text = SpecCase.PocEmail
            End If
            If Not String.IsNullOrEmpty(SpecCase.PocPhoneDSN) Then
                POCPhoneLabel.Text = SpecCase.PocPhoneDSN
            End If
            If Not String.IsNullOrEmpty(SpecCase.POCRankAndName) Then
                POCNameLabel.Text = SpecCase.POCRankAndName
            End If

        End Sub

        Private Sub SaveFindings()

            If (Not UserCanEdit) Then
                Exit Sub
            End If

            Dim PageAccess As ALOD.Core.Domain.Workflow.PageAccessType
            PageAccess = SectionList(AGRSectionNames.AGR_BOARD_SG_REV.ToString())   'ToDo !!!

            If (SpecCase.Status = SpecCaseAGRWorkStatus.InitiateCase Or SpecCase.Status = SpecCaseAGRWorkStatus.HqInitiateCase Or
                    SpecCase.Status = SpecCaseAGRWorkStatus.PackageReview) Then

                'RMU
                If ddlRMUs.SelectedIndex > 0 Then
                    SpecCase.RMUName = ddlRMUs.SelectedValue
                End If

                SpecCase.AFSC = HTMLEncodeNulls(AFSCTextBox.Text)

                If String.IsNullOrEmpty(PHADate.Text) Then
                    SpecCase.PHADate = Nothing
                Else
                    SpecCase.PHADate = Server.HtmlEncode(PHADate.Text)
                End If

                If ALCY.Checked Then
                    SpecCase.ALC = 1
                End If
                If ALCN.Checked Then
                    SpecCase.ALC = 0
                End If

                If MAJCOMY.Checked Then
                    SpecCase.MAJCOM = 1
                End If
                If MAJCOMN.Checked Then
                    SpecCase.MAJCOM = 0
                End If

                ' Initial Tour/ Follow On Tour
                If InitialTourY.Checked Then
                    SpecCase.InitialTour = 1
                End If

                If InitialTourN.Checked Then
                    SpecCase.InitialTour = 0
                End If

                If FollowOnY.Checked Then
                    SpecCase.FollowOnTour = 1
                End If

                If FollowOnN.Checked Then
                    SpecCase.FollowOnTour = 0
                End If

                SpecCase.PocEmail = HTMLEncodeNulls(POCEmailLabel.Text)
                SpecCase.PocUnit = HTMLEncodeNulls(POCUnitLabel.Text)
                SpecCase.MTFInitialFacility = HTMLEncodeNulls(POCAddressLabel.Text)
                SpecCase.MTFInitialFacilityCityStateZip = HTMLEncodeNulls(POCAddressLabelCityStateZip.Text)
                SpecCase.PocPhoneDSN = HTMLEncodeNulls(POCPhoneLabel.Text)
                SpecCase.POCRankAndName = HTMLEncodeNulls(POCNameLabel.Text)

            End If

            SCDao.SaveOrUpdate(SpecCase)
            SCDao.CommitChanges()

        End Sub

#Region "TabEvent"

        Public Function ValidBoxLength() As Boolean
            Dim IsValid As Boolean = True
            'If Not CheckTextLength(DiagnosisTextBox) Then
            '    IsValid = False
            'End If

            Return IsValid
        End Function

        Protected Sub Page_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
            AddHandler Me.Master.TabClick, AddressOf TabButtonClicked
        End Sub

        Protected Sub Save_Click(ByRef sender As Object, ByRef e As TabNavigationEventArgs)

            If (Navigator.CurrentStep.IsReadOnly) Then
                Exit Sub
            End If

            If (e.ButtonType = NavigatorButtonType.Save OrElse e.ButtonType = NavigatorButtonType.NavigatedAway _
                OrElse e.ButtonType = NavigatorButtonType.NextStep OrElse e.ButtonType = NavigatorButtonType.PreviousStep) Then
                SaveFindings()
            End If

            If Not (ValidBoxLength()) Then
                e.Cancel = True
                Exit Sub
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