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

Namespace Web.Special_Case.DW

    Partial Class Secure_SC_DW_MedTech
        Inherits System.Web.UI.Page

        Private _MedTechSig As SignatureMetaData
        Private _sigDao As ISignatueMetaDateDao

#Region "DWProperty"

        Private _icd As String
        Private _scAccess As Dictionary(Of String, ALOD.Core.Domain.Workflow.PageAccessType)
        Dim dao As ISpecialCaseDAO

        Private sc As SC_DW = Nothing
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

        Protected ReadOnly Property CalendarImage() As String
            Get
                Return Me.ResolveClientUrl("~/App_Themes/" + Page.Theme + "/Images/Calendar.gif")
            End Get
        End Property

        Protected ReadOnly Property SpecCase() As SC_DW
            Get
                If (sc Is Nothing) Then
                    sc = SCDao.GetById(ReferenceId)
                End If

                Return sc
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
                If (ViewState("UserCanEdit") Is Nothing) Then
                    ViewState("UserCanEdit") = False
                End If
                Return CBool(ViewState("UserCanEdit"))
            End Get
            Set(value As Boolean)
                ViewState("UserCanEdit") = value
            End Set
        End Property

        Protected ReadOnly Property MasterPage() As SC_DWMaster
            Get
                Dim master As SC_DWMaster = CType(Page.Master, SC_DWMaster)
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

        Private ReadOnly Property MedTechSig() As SignatureMetaData
            Get
                If (_MedTechSig Is Nothing) Then
                    _MedTechSig = SigDao.GetByWorkStatus(SpecCase.Id, SpecCase.Workflow, SpecCaseWWDWorkStatus.InitiateCase)
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

        Protected Sub DisplayReadOnly(ByVal ShowStoppers As Integer)

            txtDeployStart.Enabled = False
            txtDeployEnd.Enabled = False
            txtDeployLocation.Enabled = False
            txtLineNumber.Enabled = False

            txtLineRemarks.Enabled = True
            txtLineRemarks.ReadOnly = True

            txtPOCEmailLabel.Enabled = False
            txtPOCPhoneLabel.Enabled = False
            txtPOCNameLabel.Enabled = False
        End Sub

        Protected Sub DisplayReadWrite()

            txtDeployStart.CssClass = "datePickerAll"
            txtDeployEnd.CssClass = "datePickerAll"

        End Sub

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

            If (Not IsPostBack) Then
                UserCanEdit = GetAccess(Navigator.PageAccess, True)
                InitControls()
                SetMaxLength(txtLineRemarks)
                SetMaxLength(txtLineNumber)
                SetMaxLength(txtDeployLocation)
                SetMaxLength(txtPOCNameLabel)
                SetMaxLength(txtPOCPhoneLabel)
                SetMaxLength(txtPOCEmailLabel)

                SetInputFormatRestriction(Page, txtLineNumber, FormatRestriction.AlphaNumeric, "-, ")
            End If

            LogManager.LogAction(ModuleType.SpecCaseDW, UserAction.ViewPage, RefId, "Viewed Page: DW Med Tech")

        End Sub

        Private Sub InitControls()

            Dim lkupDAO As ILookupDao
            lkupDAO = New NHibernateDaoFactory().GetLookupDao()

            SetInputFormatRestriction(Page, txtDeployStart, FormatRestriction.Numeric, "/")
            SetInputFormatRestriction(Page, txtDeployEnd, FormatRestriction.Numeric, "/")
            SetInputFormatRestriction(Page, txtDeployLocation, FormatRestriction.AlphaNumeric, PERMITTED_SPECIAL_CHAR_INPUT)
            SetInputFormatRestriction(Page, txtLineRemarks, FormatRestriction.AlphaNumeric, PERMITTED_SPECIAL_CHAR_INPUT)

            If Not IsNothing(SpecCase.deploy_start_date) Then
                txtDeployStart.Text = Server.HtmlDecode(SpecCase.deploy_start_date)
            End If

            If Not IsNothing(SpecCase.deploy_end_date) Then
                txtDeployEnd.Text = Server.HtmlDecode(SpecCase.deploy_end_date)
            End If

            If Not IsNothing(SpecCase.deploy_location) Then
                txtDeployLocation.Text = Server.HtmlDecode(SpecCase.deploy_location)
            End If

            If Not IsNothing(SpecCase.line_number) Then
                txtLineNumber.Text() = Server.HtmlDecode(SpecCase.line_number)
            End If

            If Not IsNothing(txtLineRemarks.Text) Then
                txtLineRemarks.Text() = Server.HtmlDecode(SpecCase.line_remarks)
            End If

            Dim uDao As UserDao = New NHibernateDaoFactory().GetUserDao()
            Dim currUser As AppUser = uDao.GetById(SESSION_USER_ID)

            If Not String.IsNullOrEmpty(SpecCase.PocEmail) Then
                txtPOCEmailLabel.Text = SpecCase.PocEmail
            End If
            If Not String.IsNullOrEmpty(SpecCase.PocPhoneDSN) Then
                txtPOCPhoneLabel.Text = SpecCase.PocPhoneDSN
            End If
            If Not String.IsNullOrEmpty(SpecCase.PocRankAndName) Then
                txtPOCNameLabel.Text = SpecCase.PocRankAndName
            End If

            DisplayReadWrite()

            If Not UserCanEdit Then
                DisplayReadOnly(0)
            End If

        End Sub

        Private Sub SaveFindings()

            If (Not UserCanEdit) Then
                Exit Sub
            End If

            If Not String.IsNullOrEmpty(txtDeployStart.Text) Then
                SpecCase.deploy_start_date = Server.HtmlEncode(txtDeployStart.Text)
            End If

            If Not String.IsNullOrEmpty(txtDeployEnd.Text) Then
                SpecCase.deploy_end_date = Server.HtmlEncode(txtDeployEnd.Text)
            End If

            If Not String.IsNullOrEmpty(txtDeployLocation.Text) Then
                SpecCase.deploy_location = Server.HtmlEncode(txtDeployLocation.Text)
            End If

            If Not String.IsNullOrEmpty(txtLineNumber.Text) Then
                SpecCase.line_number = Server.HtmlEncode(txtLineNumber.Text())
            End If

            If Not String.IsNullOrEmpty(txtLineRemarks.Text) Then
                SpecCase.line_remarks = Server.HtmlEncode(txtLineRemarks.Text())
            End If

            If Not String.IsNullOrEmpty(txtPOCPhoneLabel.Text) Then
                SpecCase.PocPhoneDSN = Server.HtmlEncode(txtPOCPhoneLabel.Text)
            End If

            If Not String.IsNullOrEmpty(txtPOCEmailLabel.Text) Then
                SpecCase.PocEmail = Server.HtmlEncode(txtPOCEmailLabel.Text)
            End If

            If Not String.IsNullOrEmpty(txtPOCNameLabel.Text) Then
                SpecCase.PocRankAndName = Server.HtmlEncode(txtPOCNameLabel.Text)
            End If

            SCDao.SaveOrUpdate(SpecCase)
            SCDao.CommitChanges()

        End Sub

#Region "TabEvent"

        Public Function ValidBoxLength() As Boolean
            Dim IsValid As Boolean = True
            If Not CheckTextLength(txtLineRemarks) Then
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