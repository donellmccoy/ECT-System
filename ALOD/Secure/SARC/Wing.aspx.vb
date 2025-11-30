Imports ALOD.Core.Domain.Common
Imports ALOD.Core.Domain.Lookup
Imports ALOD.Core.Domain.Modules.Lod
Imports ALOD.Core.Domain.Modules.SARC
Imports ALOD.Core.Domain.Workflow
Imports ALOD.Core.Interfaces.DAOInterfaces
Imports ALOD.Core.Utils
Imports ALOD.Data
Imports ALOD.Logging
Imports ALOD.Web.UserControls
Imports ALODWebUtility.Common
Imports ALODWebUtility.Common.WebControlSetters

Namespace Web.SARC

    Public Class Wing
        Inherits System.Web.UI.Page

#Region "Fields..."

        Protected Const INDEX_E968 As Integer = 0
        Protected Const INDEX_E969 As Integer = 1
        Protected Const INDEX_OTHER As Integer = 2
        Protected Const PAGE_TITLE As String = "Wing SARC"
        Private _daoFactory As NHibernateDaoFactory
        Private _lookupDao As ILookupDao
        Private _pageAccessDictionary As Dictionary(Of String, ALOD.Core.Domain.Workflow.PageAccessType)
        Private _sarc As RestrictedSARC
        Private _sarcDao As ISARCDAO
        Private icdDao As IICD9CodeDao

#End Region

#Region "Properties..."

        Public ReadOnly Property ReferenceId() As Integer
            Get
                Return Integer.Parse(Request.QueryString("refId"))
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

        Protected ReadOnly Property DAOFactory As NHibernateDaoFactory
            Get
                If (_daoFactory Is Nothing) Then
                    _daoFactory = New NHibernateDaoFactory()
                End If

                Return _daoFactory
            End Get
        End Property

        Protected ReadOnly Property ICDCodeDao As IICD9CodeDao
            Get
                If (icdDao Is Nothing) Then
                    icdDao = DAOFactory.GetICD9CodeDao()
                End If

                Return icdDao
            End Get
        End Property

        Protected ReadOnly Property LookupDao As ILookupDao
            Get
                If (_lookupDao Is Nothing) Then
                    _lookupDao = DAOFactory.GetLookupDao()
                End If

                Return _lookupDao
            End Get
        End Property

        Protected ReadOnly Property MasterPage() As SARCMaster
            Get
                Dim master As SARCMaster = CType(Page.Master, SARCMaster)
                Return master
            End Get
        End Property

        Protected ReadOnly Property Navigator() As TabNavigator
            Get
                Return MasterPage.Navigator
            End Get
        End Property

        Protected ReadOnly Property SARC As RestrictedSARC
            Get
                If (_sarc Is Nothing) Then
                    _sarc = SARCDao.GetById(ReferenceId)
                End If

                Return _sarc
            End Get
        End Property

        Protected ReadOnly Property SARCDao As ISARCDAO
            Get
                If (_sarcDao Is Nothing) Then
                    _sarcDao = DAOFactory.GetSARCDao()
                End If

                Return _sarcDao
            End Get
        End Property

        Protected ReadOnly Property SectionList() As Dictionary(Of String, ALOD.Core.Domain.Workflow.PageAccessType)
            Get
                If (_pageAccessDictionary Is Nothing) Then
                    _pageAccessDictionary = SARC.ReadSectionList(CInt(Session("GroupId")))
                End If

                Return _pageAccessDictionary
            End Get
        End Property

        Protected ReadOnly Property TabControl() As TabControls
            Get
                Return MasterPage.TabControl
            End Get
        End Property

#End Region

#Region "Page Methods..."

        Protected Sub AssignCaseData()
            Try
                SARC.DutyStatus = Integer.Parse(rdblstMembershipStatus.SelectedValue)
                SARC.ICDE968 = chklstICDCodes.Items(INDEX_E968).Selected
                SARC.ICDE969 = chklstICDCodes.Items(INDEX_E969).Selected
                SARC.ICDOther = chklstICDCodes.Items(INDEX_OTHER).Selected

                If (txtDateOfIncident.Text.Trim.Length > 0) Then
                    SARC.IncidentDate = Server.HtmlEncode(DateTime.Parse(txtDateOfIncident.Text.Trim))
                Else
                    SARC.IncidentDate = Nothing
                End If

                If (txtSexualAssaultDatabaseCaseNumber.Text.Trim.Length > 0) Then
                    SARC.DefenseSexualAssaultDBCaseNumber = Server.HtmlEncode(txtSexualAssaultDatabaseCaseNumber.Text.Trim)
                Else
                    SARC.DefenseSexualAssaultDBCaseNumber = Nothing
                End If

                If (CheckDate(txtDurationStartDate)) Then
                    If (txtDurationStartTime.Text.Trim.Length > 0) Then
                        SARC.DurationStart = Server.HtmlEncode(ParseDateAndTime(txtDurationStartDate.Text.Trim + " " + txtDurationStartTime.Text.Trim))
                    Else
                        SARC.DurationStart = Server.HtmlEncode(DateTime.Parse(txtDurationStartDate.Text.Trim))
                    End If
                Else
                    SARC.DurationStart = Nothing
                End If

                If (CheckDate(txtDurationEndDate)) Then
                    If (txtDurationEndTime.Text.Trim.Length > 0) Then
                        SARC.DurationEnd = Server.HtmlEncode(ParseDateAndTime(txtDurationEndDate.Text.Trim + " " + txtDurationEndTime.Text.Trim))
                    Else
                        SARC.DurationEnd = Server.HtmlEncode(DateTime.Parse(txtDurationEndDate.Text.Trim))
                    End If
                Else
                    SARC.DurationEnd = Nothing
                End If

                If (rdblDutyStatusDetermination.SelectedIndex <> -1) Then
                    SARC.InDutyStatus = GetInDutyStatusFromText(rdblDutyStatusDetermination.SelectedItem.Text)
                End If
            Catch ex As Exception
                LogManager.LogError(ex)
                Throw New Exception("Error: AssignCaseData() in SARC.Wing.aspx.vb generated an exception.")
            End Try

        End Sub

        Protected Sub btnAddICDCode_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnAddICDCode.Click
            ucICDCodeModulePopup.Show()
        End Sub

        Protected Function CanSaveData() As Boolean
            If (Not UserCanEdit) Then
                Return False
            End If

            If (SARC.Status <> SARCRestrictedWorkStatus.SARCInitiate OrElse Not HasReadWriteAccessForSection(SARCSectionNames.SARC_INIT)) Then
                Return False
            End If

            Return True
        End Function

        Protected Sub chklstICDCodes_SelectedIndexChanged(sender As Object, e As EventArgs) Handles chklstICDCodes.SelectedIndexChanged
            Try
                ucICDCodeModulePopup.Hide()
                ICDCodeOtherCheckboxChangedHandler()
            Catch ex As Exception
                LogManager.LogError(ex)
                Throw New Exception("Error: chklstICDCodes_SelectedIndexChanged() in SARC.Wing.aspx.vb generated an exception.")
            End Try
        End Sub

        Protected Function GetInDutyStatusFromText(ByVal text As String) As Boolean
            If (text.Equals("Yes")) Then
                Return True
            Else
                Return False
            End If
        End Function

        Protected Function GetInDutyStatusText(ByVal dutyStatus As Boolean) As String
            If (dutyStatus) Then
                Return "Yes"
            Else
                Return "No"
            End If
        End Function

        Protected Function GetLastUserSelectedICDCodeCheckboxIndex() As Integer
            Try
                Dim result As String = Request.Form("__EVENTTARGET")
                Dim checkbox As String() = result.Split("$")
                Return Integer.Parse(checkbox(checkbox.Length - 1))
            Catch ex As Exception
                LogManager.LogError(ex)
                Throw New Exception("Error: GetLastUserSelectedICDCodeCheckboxIndex() in SARC.Wing.aspx.vb generated an exception.")
            End Try
        End Function

        Protected Function HasReadWriteAccessForSection(ByVal sectionName As SARCSectionNames) As Boolean
            If (SectionList(sectionName.ToString()) <> ALOD.Core.Domain.Workflow.PageAccessType.ReadWrite) Then
                Return False
            End If

            Return True
        End Function

        Protected Sub ICDCodeOtherCheckboxChangedHandler()
            Try
                If (GetLastUserSelectedICDCodeCheckboxIndex() = INDEX_OTHER) Then
                    If (chklstICDCodes.Items(INDEX_OTHER).Selected) Then
                        trOtherICDCodes.Visible = True
                        lblICDCodesRowId.Text = "D.1"
                        LoadICDOthersList()
                    Else
                        trOtherICDCodes.Visible = False
                        lblICDCodesRowId.Text = "D"
                        SARC.RemoveAllOtherICDCodes()
                        SARCDao.SaveOrUpdate(SARC)
                    End If
                End If
            Catch ex As Exception
                LogManager.LogError(ex)
                Throw New Exception("Error: ICDCodeOtherCheckboxChangedHandler() in SARC.Wing.aspx.vb generated an exception.")
            End Try
        End Sub

        Protected Sub InitiateControlInputRestrictions()
            SetInputFormatRestriction(Page, txtSexualAssaultDatabaseCaseNumber, FormatRestriction.AlphaNumeric, PERMITTED_SPECIAL_CHAR_INPUT)
            SetInputFormatRestriction(Page, txtDateOfIncident, FormatRestriction.Numeric, "/")
            SetInputFormatRestriction(Page, txtDurationStartDate, FormatRestriction.Numeric, "/")
            SetInputFormatRestriction(Page, txtDurationEndDate, FormatRestriction.Numeric, "/")
            SetInputFormatRestriction(Page, txtDurationStartTime, FormatRestriction.Numeric)
            SetInputFormatRestriction(Page, txtDurationEndTime, FormatRestriction.Numeric)
        End Sub

        Protected Sub InitiateControls()
            If (UserCanEdit) Then
                InitiateReadWriteControls()
            Else
                InitiateReadOnlyControls()
            End If
        End Sub

        Protected Sub InitiateMembershipRadioButtons()
            If (SARC.MemberRank.Title.Equals("Cadet")) Then
                rdblstMembershipStatus.DataSource = LookupDao.GetMemberComponentsByWorkflow(SARC.Workflow).Where(Function(x) x.Name.ToLower().Contains("cadet")).ToList()
            Else
                rdblstMembershipStatus.DataSource = LookupDao.GetMemberComponentsByWorkflow(SARC.Workflow).Where(Function(x) Not x.Name.ToLower().Contains("cadet")).ToList()
            End If

            rdblstMembershipStatus.DataValueField = "Value"
            rdblstMembershipStatus.DataTextField = "Name"
            rdblstMembershipStatus.Visible = True
            rdblstMembershipStatus.DataBind()
        End Sub

        Protected Sub InitiateReadOnlyControls()
            InitiateReadOnlyControlVisibility()

            lblICDCodesRowId.Text = "D"
        End Sub

        Protected Sub InitiateReadOnlyControlVisibility()
            lblDateOfIncident.Visible = True
            lblSexualAssaultDatabaseCaseNumber.Visible = True
            lblDurationStart.Visible = True
            lblDurationEnd.Visible = True
            lblICDCodes.Visible = True
            lblDutyStatusDetermination.Visible = True
            lblMembershipStatus.Visible = True
        End Sub

        Protected Sub InitiateReadWriteControls()
            InitiateControlInputRestrictions()
            InitiateReadWriteControlVisibility()
            InitiateMembershipRadioButtons()

            If (SARC.ICDOther.HasValue AndAlso SARC.ICDOther.Value) Then
                lblICDCodesRowId.Text = "D.1"
            Else
                lblICDCodesRowId.Text = "D"
            End If
        End Sub

        Protected Sub InitiateReadWriteControlVisibility()
            txtDateOfIncident.Visible = True
            txtSexualAssaultDatabaseCaseNumber.Visible = True
            txtDurationStartDate.Visible = True
            txtDurationStartTime.Visible = True
            txtDurationEndDate.Visible = True
            txtDurationEndTime.Visible = True
            chklstICDCodes.Visible = True
            rdblDutyStatusDetermination.Visible = True

            If (SARC.ICDOther.HasValue AndAlso SARC.ICDOther.Value) Then
                trOtherICDCodes.Visible = True
            End If
        End Sub

        Protected Sub LoadCaseData()
            If (UserCanEdit) Then
                LoadReadWriteData()
            Else
                LoadReadOnlyData()
            End If
        End Sub

        Protected Sub LoadData()
            LoadCaseData()
            LoadSignatureCheck(ucSigCheckWingSARCRSL, PersonnelTypes.WING_SARC_RSL)
        End Sub

        Protected Sub LoadICDCodeReadOnlyData()
            If (SARC.ICDE968.HasValue AndAlso SARC.ICDE968.Value) Then
                lblICDCodes.Text &= (chklstICDCodes.Items(INDEX_E968).Text & "<br />")
            End If

            If (SARC.ICDE969.HasValue AndAlso SARC.ICDE969.Value) Then
                lblICDCodes.Text &= (chklstICDCodes.Items(INDEX_E969).Text & "<br />")
            End If

            If (SARC.ICDOther.HasValue AndAlso SARC.ICDOther.Value AndAlso SARC.ICDList.Count > 0) Then
                lblICDCodes.Text &= ("Other ICD Code(s):" & "<br />")

                For Each code As RestrictedSARCOtherICDCode In SARC.ICDList
                    lblICDCodes.Text &= ("&nbsp;&nbsp;&nbsp;&nbsp;" & code.ICDCode.Code & " (" & code.ICDCode.Description & ")" & "<br />")
                Next
            End If

            ' Remove trailing "<br />" substring...
            If (lblICDCodes.Text.Length >= 6) Then
                lblICDCodes.Text = lblICDCodes.Text.Substring(0, lblICDCodes.Text.Length - 6)
            End If

        End Sub

        Protected Sub LoadICDCodeReadWriteData()
            If (SARC.ICDE968.HasValue) Then
                chklstICDCodes.Items(INDEX_E968).Selected = SARC.ICDE968.Value
            End If

            If (SARC.ICDE969.HasValue) Then
                chklstICDCodes.Items(INDEX_E969).Selected = SARC.ICDE969.Value
            End If

            If (SARC.ICDOther.HasValue) Then
                chklstICDCodes.Items(INDEX_OTHER).Selected = SARC.ICDOther.Value
            End If

            LoadICDOthersList()
        End Sub

        Protected Sub LoadICDOthersList()
            rptrICDOthers.DataSource = SARC.ICDList
            rptrICDOthers.DataBind()
        End Sub

        Protected Sub LoadInDutyStatus(ByVal isReadOnly As Boolean)
            Try
                If (Not SARC.InDutyStatus.HasValue) Then
                    Exit Sub
                End If

                If (isReadOnly) Then
                    lblDutyStatusDetermination.Text = GetInDutyStatusText(SARC.InDutyStatus.Value)
                Else
                    rdblDutyStatusDetermination.Items.FindByText(GetInDutyStatusText(SARC.InDutyStatus.Value)).Selected = True
                End If
            Catch ex As Exception
                LogManager.LogError(ex)
                Throw New Exception("Error: LoadInDutyStatus() in SARC.Wing.aspx.vb generated an exception.")
            End Try
        End Sub

        Protected Sub LoadMembershipStatus(ByVal isReadOnly As Boolean)
            If (isReadOnly) Then
                LoadMembershipStatusLabel()
            Else
                LoadMembershipStatusRadioButtons()
            End If
        End Sub

        Protected Sub LoadMembershipStatusLabel()
            Try
                If (Not SARC.DutyStatus.HasValue) Then
                    lblMembershipStatus.Text = "AFR"
                    Exit Sub
                End If

                Dim aList As RadioButtonList = New RadioButtonList()

                If (SARC.MemberRank.Title.Equals("Cadet")) Then
                    aList.DataSource = LookupDao.GetMemberComponentsByWorkflow(SARC.Workflow).Where(Function(x) x.Name.ToLower().Contains("cadet")).ToList()
                Else
                    aList.DataSource = LookupDao.GetMemberComponentsByWorkflow(SARC.Workflow).Where(Function(x) Not x.Name.ToLower().Contains("cadet")).ToList()
                End If

                aList.DataValueField = "Value"
                aList.DataTextField = "Name"
                aList.DataBind()

                If (aList.Items.FindByValue(SARC.DutyStatus.Value) IsNot Nothing) Then
                    lblMembershipStatus.Text = aList.Items.FindByValue(SARC.DutyStatus.Value).Text
                End If
            Catch ex As Exception
                LogManager.LogError(ex)
                Throw New Exception("Error: LoadMembershipStatusLabel() in SARC.Wing.aspx.vb generated an exception.")
            End Try
        End Sub

        Protected Sub LoadMembershipStatusRadioButtons()
            Try
                Dim defaultStatusName As String = "AFR"

                If (SARC.MemberRank.Title.Equals("Cadet")) Then
                    defaultStatusName = "AFROTC Cadet"
                End If

                If (Not SARC.DutyStatus.HasValue) Then
                    rdblstMembershipStatus.Items.FindByText(defaultStatusName).Selected = True
                    Exit Sub
                End If

                If (rdblstMembershipStatus.Items.FindByValue(SARC.DutyStatus.Value) Is Nothing) Then
                    rdblstMembershipStatus.Items.FindByText(defaultStatusName).Selected = True
                    Exit Sub
                End If

                rdblstMembershipStatus.Items.FindByValue(SARC.DutyStatus.Value).Selected = True
            Catch ex As Exception
                LogManager.LogError(ex)
                Throw New Exception("Error: LoadMembershipStatus() in SARC.Wing.aspx.vb generated an exception.")
            End Try
        End Sub

        Protected Sub LoadReadOnlyData()
            Try
                SetDateLabel(lblDateOfIncident, SARC.IncidentDate)
                SetLabelText(lblSexualAssaultDatabaseCaseNumber, SARC.DefenseSexualAssaultDBCaseNumber)
                SetDateTimeLabel(lblDurationStart, SARC.DurationStart)
                SetDateTimeLabel(lblDurationEnd, SARC.DurationEnd)

                LoadInDutyStatus(True)
                LoadMembershipStatus(True)
                LoadICDCodeReadOnlyData()
            Catch ex As Exception
                LogManager.LogError(ex)
                Throw New Exception("Error: LoadReadOnlyData() in SARC.Wing.aspx.vb generated an exception.")
            End Try
        End Sub

        Protected Sub LoadReadWriteData()
            Try
                SetDateTextbox(txtDateOfIncident, SARC.IncidentDate)
                SetTextboxText(txtSexualAssaultDatabaseCaseNumber, SARC.DefenseSexualAssaultDBCaseNumber)
                SetDateTextbox(txtDurationStartDate, SARC.DurationStart)
                SetTimeTextbox(txtDurationStartTime, SARC.DurationStart)
                SetDateTextbox(txtDurationEndDate, SARC.DurationEnd)
                SetTimeTextbox(txtDurationEndTime, SARC.DurationEnd)

                LoadInDutyStatus(False)
                LoadMembershipStatus(False)
                LoadICDCodeReadWriteData()
            Catch ex As Exception
                LogManager.LogError(ex)
                Throw New Exception("Error: LoadReadWriteData() in SARC.Wing.aspx.vb generated an exception.")
            End Try
        End Sub

        Protected Sub LoadSignatureCheck(ByVal sigCheck As Secure_Shared_UserControls_SigntureCheck, ByVal pType As PersonnelTypes)
            If (UserCanEdit) Then
                Exit Sub
            End If

            sigCheck.Visible = True
            sigCheck.VerifySignature(ReferenceId, pType)
        End Sub

        Protected Sub OtherICDCodeSubmittedHandler(ByVal sender As Object, ByVal e As ICDCodeSelectedEventArgs)
            Try
                If (e.SelectedICDCodeId <= 0) Then
                    Exit Sub
                End If

                Dim selectedICDCode As ICD9Code = ICDCodeDao.GetById(e.SelectedICDCodeId)

                If (selectedICDCode Is Nothing) Then
                    Exit Sub
                End If

                SARC.AddOtherICDCode(selectedICDCode, e.SelectedICD7thCharacter)
                SARCDao.SaveOrUpdate(SARC)
                LoadICDOthersList()
            Catch ex As Exception
                LogManager.LogError(ex)
                Throw New Exception("Error: OtherICDCodeSubmittedHandler() in SARC.Wing.aspx.vb generated an exception.")
            End Try
        End Sub

        Protected Sub Page_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
            AddHandler CType(Page.Master, SARCMaster).TabClick, AddressOf TabButtonClicked
            AddHandler ucICDCodeModulePopup.ICDCodeSubmitted, AddressOf OtherICDCodeSubmittedHandler
        End Sub

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
            If (Not IsPostBack) Then
                UserCanEdit = GetAccess(Navigator.PageAccess, True)
                InitiateControls()
                LoadData()

                LogManager.LogAction(ModuleType.SARC, UserAction.ViewPage, ReferenceId, "Viewed Page: " & PAGE_TITLE)
            End If
        End Sub

        Protected Sub rptrICDOthers_ItemCommand(ByVal source As Object, ByVal e As RepeaterCommandEventArgs) Handles rptrICDOthers.ItemCommand
            Try
                ucICDCodeModulePopup.Hide()

                If (e.CommandName = "Remove") Then
                    Dim icdCodeId As Integer = Integer.Parse(e.CommandArgument.ToString())
                    SARC.RemoveOtherICDCode(icdCodeId)
                    SARCDao.SaveOrUpdate(SARC)
                    LoadICDOthersList()
                End If
            Catch ex As Exception
                LogManager.LogError(ex)
                Throw New Exception("Error: rptrICDOthers_ItemCommand() in SARC.Wing.aspx.vb generated an exception.")
            End Try
        End Sub

        Protected Sub rptrICDOthers_ItemDataBound(ByVal sender As Object, ByVal e As RepeaterItemEventArgs) Handles rptrICDOthers.ItemDataBound
            Try
                If (e.Item.ItemType <> ListItemType.Item AndAlso e.Item.ItemType <> ListItemType.AlternatingItem) Then
                    Exit Sub
                End If

                Dim icdOtherCode As RestrictedSARCOtherICDCode = CType(e.Item.DataItem, RestrictedSARCOtherICDCode)

                CType(e.Item.FindControl("lblOtherICDFullName"), Label).Text = (icdOtherCode.ICDCode.GetFullCode(icdOtherCode.ICD7thCharacter) & " (" & icdOtherCode.ICDCode.Description & ")")

                Dim removeButton As LinkButton = CType(e.Item.FindControl("lkbtnRemove"), LinkButton)

                removeButton.CommandName = "Remove"
                removeButton.CommandArgument = icdOtherCode.ICDCode.Id
            Catch ex As Exception
                LogManager.LogError(ex)
                Throw New Exception("Error: rptrICDOthers_ItemDataBound() in SARC.Wing.aspx.vb generated an exception.")
            End Try
        End Sub

        Protected Sub SaveData()
            Try
                If (Not CanSaveData()) Then
                    Exit Sub
                End If

                AssignCaseData()

                SARCDao.SaveOrUpdate(SARC)
            Catch ex As Exception
                LogManager.LogError(ex)
                Throw New Exception("Error: SaveData() in SARC.Wing.aspx.vb generated an exception.")
            End Try
        End Sub

        Protected Sub TabButtonClicked(ByRef sender As Object, ByRef e As TabNavigationEventArgs)
            ucICDCodeModulePopup.Hide()

            If (Navigator.CurrentStep.IsReadOnly) Then
                Exit Sub
            End If

            If (SARCUtility.IsTimeToSaveViaNavigator(e.ButtonType)) Then
                SaveData()
            End If
        End Sub

#End Region

    End Class

End Namespace