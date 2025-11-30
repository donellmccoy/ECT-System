Imports ALOD.Core.Domain.Common
Imports ALOD.Core.Domain.Lookup
Imports ALOD.Core.Domain.Modules.SpecialCases
Imports ALOD.Core.Domain.Workflow
Imports ALOD.Core.Interfaces.DAOInterfaces
Imports ALOD.Core.Utils
Imports ALOD.Data
Imports ALOD.Logging
Imports ALOD.Web.UserControls
Imports ALODWebUtility.Common

Namespace Web.Special_Case.MEB

    Partial Class Secure_sc_meb_HQTech
        Inherits System.Web.UI.Page

#Region "MEBProperty"

        Private _scAccess As Dictionary(Of String, ALOD.Core.Domain.Workflow.PageAccessType)
        Dim dao As ISpecialCaseDAO

        Private sc As SC_MEB = Nothing
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

        Protected ReadOnly Property SpecCase() As SC_MEB
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

        Protected ReadOnly Property MasterPage() As SC_MEBMaster
            Get
                Dim master As SC_MEBMaster = CType(Page.Master, SC_MEBMaster)
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

        Protected Sub ddlSelectMemos_DataBound(ByVal sender As Object, ByVal e As System.EventArgs) Handles ddlMemos.DataBound
            ddlMemos.Items.Insert(0, New ListItem("-- Select a Memo --", String.Empty))
        End Sub

        Protected Sub DisplayReadOnly(ByVal ShowStoppers As Integer)

            ' There is no data entry on this page
            ddlMedGroups.Enabled = False
            ddlRMUs.Enabled = False
            ddlMemberStatus.Enabled = False
            ucICDCodeControl.DisplayReadOnly(False)
            ucICD7thCharacterControl.DisplayReadOnly()
            DiagnosisTextBox.ReadOnly = True
            DAWGRecommendation.Enabled = False
            ddlAssignmentLimitation.Enabled = False
            ddlApprovingAuthority.Enabled = False
            ddlMemos.Enabled = False
            
            ' Remove datepicker CSS classes to hide calendar icons
            txtCode37Date.CssClass = ""
            txtCode37Date.Enabled = False
            txtExpirationDate.CssClass = ""
            txtExpirationDate.Enabled = False
            txtEffectiveDate.CssClass = ""
            txtEffectiveDate.Enabled = False
            txtForwardDate.CssClass = ""
            txtForwardDate.Enabled = False
            txtRTD.CssClass = ""
            txtRTD.Enabled = False
            
            ' Disable Indefinite checkbox
            chbIndefinite.Enabled = False
        End Sub

        Protected Sub DisplayReadWrite()

            ' There is no data entry on this page
            txtEffectiveDate.CssClass = "datePickerAll"
            txtExpirationDate.CssClass = "datePickerAll"
            txtForwardDate.CssClass = "datePicker"
            txtRTD.CssClass = "datePickerAll"
            txtCode37Date.CssClass = "datePicker"
            ucICDCodeControl.DisplayReadWrite(False)
            ucICD7thCharacterControl.DisplayReadWrite()

        End Sub

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

            ucICDCodeControl.Initialilze(Me)
            ucICD7thCharacterControl.Initialize(ucICDCodeControl)

            If (Not IsPostBack) Then
                UserCanEdit = GetAccess(Navigator.PageAccess, True)
                InitControls()
                SetMaxLength(DiagnosisTextBox)

            End If

            LogManager.LogAction(ModuleType.SpecCaseMEB, UserAction.ViewPage, RefId, "Viewed Page: MB HQ AFRC Tech")

            If Session(SESSIONKEY_COMPO) = "6" Then
                Label2.Text = "RMU Name: "
            Else
                Label2.Text = "GMU Name: "
            End If

        End Sub

        Private Sub InitControls()

            Dim lkupDAO As ILookupDao
            lkupDAO = New NHibernateDaoFactory().GetLookupDao()

            SetInputFormatRestriction(Page, DiagnosisTextBox, FormatRestriction.AlphaNumeric, PERMITTED_SPECIAL_CHAR_INPUT)
            SetInputFormatRestriction(Page, txtCode37Date, FormatRestriction.AlphaNumeric, PERMITTED_SPECIAL_CHAR_INPUT)
            SetInputFormatRestriction(Page, txtExpirationDate, FormatRestriction.Numeric, "/")
            SetInputFormatRestriction(Page, txtRTD, FormatRestriction.Numeric, "/")
            SetInputFormatRestriction(Page, txtEffectiveDate, FormatRestriction.Numeric, "/")
            SetInputFormatRestriction(Page, txtForwardDate, FormatRestriction.Numeric, "/")

            'cboxMedGroup.WhichMethod = "GetMedGroupNames"
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
                ddlMedGroups.SelectedValue = SpecCase.MedGroupName
            End If

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

            If Not IsNothing(SpecCase.DAWGRecommendation) Then
                DAWGRecommendation.SelectedValue = SpecCase.DAWGRecommendation
            End If

            If Not IsNothing(SpecCase.MemberStatus) Then
                ddlMemberStatus.SelectedValue = SpecCase.MemberStatus
            End If

            If Not IsNothing(SpecCase.ICD9Code) Then
                ucICDCodeControl.InitializeHierarchy(SpecCase.ICD9Code)

                If (ucICDCodeControl.IsValidICDCode(SpecCase.ICD9Code)) Then
                    ucICDCodeControl.UpdateICDCodeDiagnosisLabel(SpecCase.ICD9Code)
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

            If Not IsNothing(SpecCase.ALCLetterType) Then
                ddlAssignmentLimitation.SelectedValue = SpecCase.ALCLetterType
            End If

            If SpecCase.MemoTemplateID.HasValue Then
                ddlMemos.SelectedValue = SpecCase.MemoTemplateID
            End If

            If Not IsNothing(SpecCase.ApprovingAuthorityType) Then
                ddlApprovingAuthority.SelectedValue = SpecCase.ApprovingAuthorityType
            End If

            If Not IsNothing(SpecCase.Code37InitDate) Then
                txtCode37Date.Text = Server.HtmlDecode(SpecCase.Code37InitDate.Value.ToString(DATE_FORMAT))
            End If

            ' Experiation Date revision by skennedy; 10/2013
            ' Checks if date is indefinite; Set in UI by javascript function;
            If Not IsNothing(SpecCase.ExpirationDate) Then
                txtExpirationDate.Text = Server.HtmlDecode(SpecCase.ExpirationDate)
            End If
            chbIndefinite.Attributes.Add("onclick", "SetIndefinite(this)")

            If txtExpirationDate.Text = "12/31/2100" Then
                txtExpirationDate.CssClass = ""
                txtExpirationDate.Enabled = False
                chbIndefinite.Checked = True
            Else
                txtExpirationDate.CssClass = "datePickerAll"
                txtExpirationDate.Enabled = True
                chbIndefinite.Checked = False

            End If
            ' ''''''''''''''''''''''''''''''''''''''

            If Not IsNothing(SpecCase.EffectiveDate) Then
                txtEffectiveDate.Text = Server.HtmlDecode(SpecCase.EffectiveDate)
            End If

            If Not IsNothing(SpecCase.ForwardDate) Then
                txtForwardDate.Text = Server.HtmlDecode(SpecCase.ForwardDate)
            End If

            If Not IsNothing(SpecCase.ReturnToDutyDate) Then
                txtRTD.Text = Server.HtmlDecode(SpecCase.ReturnToDutyDate)
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

            ' There originally was no data entry on this page  -- there is now
            If ddlMedGroups.SelectedIndex > 0 Then
                SpecCase.MedGroupName = ddlMedGroups.SelectedValue
            End If
            If ddlRMUs.SelectedIndex > 0 Then
                SpecCase.RMUName = ddlRMUs.SelectedValue
            End If
            If ddlMemberStatus.SelectedIndex > 0 Then
                SpecCase.MemberStatus = ddlMemberStatus.SelectedValue
            End If
            If DAWGRecommendation.SelectedIndex > 0 Then
                SpecCase.DAWGRecommendation = DAWGRecommendation.SelectedValue
            End If

            If (ucICDCodeControl.IsICDCodeSelected()) Then
                SpecCase.ICD9Code = ucICDCodeControl.SelectedICDCodeID
                SpecCase.ICD9Description = ucICDCodeControl.SelectedICDCodeText
            Else
                'SpecCase.ICD9Code = Nothing
                'SpecCase.ICD9Description = Nothing
            End If

            SpecCase.ICD9Diagnosis = Server.HtmlEncode(DiagnosisTextBox.Text.Trim())

            If (ucICD7thCharacterControl.Is7thCharacterSelected()) Then
                SpecCase.ICD7thCharacter = ucICD7thCharacterControl.Selected7thCharacter
            Else
                SpecCase.ICD7thCharacter = Nothing
            End If

            If (Not IsNothing(SpecCase.ICD9Code)) Then
                ucICDCodeControl.UpdateICDCodeDiagnosisLabel(SpecCase.ICD9Code)
            End If

            If ddlAssignmentLimitation.SelectedValue > 0 Then
                SpecCase.ALCLetterType = ddlAssignmentLimitation.SelectedValue
            End If

            If Not String.IsNullOrEmpty(ddlMemos.SelectedValue) Then
                SpecCase.MemoTemplateID = ddlMemos.SelectedValue
            End If

            If ddlApprovingAuthority.SelectedValue > 0 Then
                SpecCase.ApprovingAuthorityType = ddlApprovingAuthority.SelectedValue
            End If

            If Not String.IsNullOrEmpty(txtCode37Date.Text) Then
                SpecCase.Code37InitDate = Server.HtmlEncode(txtCode37Date.Text)
            End If

            If Not String.IsNullOrEmpty(txtExpirationDate.Text) Then
                txtExpirationDate.Enabled = True
                SpecCase.ExpirationDate = Server.HtmlEncode(txtExpirationDate.Text)
            End If

            If Not String.IsNullOrEmpty(txtRTD.Text) Then
                SpecCase.ReturnToDutyDate = Server.HtmlEncode(txtRTD.Text)
            End If

            If Not String.IsNullOrEmpty(txtEffectiveDate.Text) Then
                SpecCase.EffectiveDate = Server.HtmlEncode(txtEffectiveDate.Text)
            End If

            If Not String.IsNullOrEmpty(txtForwardDate.Text) Then
                SpecCase.ForwardDate = Server.HtmlEncode(txtForwardDate.Text)
            End If

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