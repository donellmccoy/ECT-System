Imports ALOD.Core.Domain.Common
Imports ALOD.Core.Domain.DBSign
Imports ALOD.Core.Domain.Modules.SARC
Imports ALOD.Core.Domain.Users
Imports ALOD.Core.Domain.Workflow
Imports ALOD.Core.Interfaces.DAOInterfaces
Imports ALOD.Core.Utils
Imports ALOD.Data
Imports ALOD.Data.Services
Imports ALOD.Logging
Imports ALOD.Web.UserControls
Imports ALODWebUtility.Common
Imports ALODWebUtility.Worklfow

Namespace Web.SARC

    Partial Class SMData
        Inherits System.Web.UI.Page

        Private _daoFactory As IDaoFactory
        Private _postdao As ISARCPostProcessingDao
        Private _postprocessing As RestrictedSARCPostProcessing
        Private _sarc As RestrictedSARC
        Private _sarcDao As ISARCDAO
        Private _signatureMetaDataDao As ISignatueMetaDateDao

#Region "Properties"

        Protected ReadOnly Property CalendarImage() As String
            Get
                Return GetCalendarImage(Me)
            End Get
        End Property

        Protected ReadOnly Property DaoFactory As IDaoFactory
            Get
                If (_daoFactory Is Nothing) Then
                    _daoFactory = New NHibernateDaoFactory()
                End If

                Return _daoFactory
            End Get
        End Property

        Protected ReadOnly Property ModuleType() As ModuleType
            Get
                Return ModuleType.SARC
            End Get
        End Property

        Protected ReadOnly Property Navigator() As TabNavigator
            Get
                Return Me.Master.Navigator
            End Get
        End Property

        Protected ReadOnly Property refId() As Integer
            Get
                Return Integer.Parse(Request.QueryString("refId"))
            End Get
        End Property

        Protected ReadOnly Property SARC As RestrictedSARC
            Get
                If (_sarc Is Nothing) Then
                    _sarc = SARCDao.GetById(refId)
                End If

                Return _sarc
            End Get
        End Property

        Protected ReadOnly Property SARCDao As ISARCDAO
            Get
                If (_sarcDao Is Nothing) Then
                    _sarcDao = DaoFactory.GetSARCDao()
                End If

                Return _sarcDao
            End Get
        End Property

        Protected Property SARCPostProcessing As RestrictedSARCPostProcessing
            Get
                If (_postprocessing Is Nothing) Then
                    _postprocessing = SARCPostProcessingDao.GetById(SARC.Id)
                End If

                Return _postprocessing
            End Get
            Set(value As RestrictedSARCPostProcessing)
                _postprocessing = value
            End Set
        End Property

        Protected ReadOnly Property SARCPostProcessingDao As ISARCPostProcessingDao
            Get
                If (_postdao Is Nothing) Then
                    _postdao = DaoFactory.GetSARCPostProcessingDao()
                End If

                Return _postdao
            End Get
        End Property

        Protected ReadOnly Property SigMetaDataDao As ISignatueMetaDateDao
            Get
                If (_signatureMetaDataDao Is Nothing) Then
                    _signatureMetaDataDao = DaoFactory.GetSigMetaDataDao()
                End If

                Return _signatureMetaDataDao
            End Get
        End Property

        Protected ReadOnly Property TabControl() As TabControls
            Get
                Return Master.TabControl
            End Get
        End Property

#End Region

#Region "Page Methods"

        Protected Sub AddPostCompletionDigitalSignatureMetaData()
            Dim groupId As Byte = CByte(Session("GroupId"))
            Dim currentUser As AppUser = UserService.CurrentUser()

            SARC.AddSignature(DaoFactory, groupId, currentUser.SignatureTitle, currentUser)
        End Sub

        Protected Sub btnSavePostProcessingData_Click(sender As Object, e As EventArgs) Handles btnSavePostProcessingData.Click
            SaveData()
        End Sub

        Protected Function CanDigitallySign() As Boolean
            'If (SARC.IsPostProcessingComplete OrElse SARC.DetermineIfPostProcessingIsComplete(DaoFactory)) Then
            If (SARC.IsPostProcessingComplete) Then
                Return False
            End If

            If (Not MemberInformedCheckBox.Checked) Then
                Return False
            End If

            If (String.IsNullOrEmpty(NotificationDate.Text)) Then
                Return False
            End If

            If (Not SARC.NotificationMemoCreated(DaoFactory.GetMemoDao2())) Then
                Return False
            End If

            Return True
        End Function

        Protected Function CanSavePostProcessingData() As Boolean
            If (Not SARC.WorkflowStatus.StatusCodeType.IsFinal OrElse SARC.WorkflowStatus.StatusCodeType.IsCancel) Then
                Return False
            End If

            If (SARC.Cancel_Reason.HasValue AndAlso SARC.Cancel_Reason.Value <> 0) Then
                Return False
            End If

            If (Not UserHasPermission("RSARCPostCompletion")) Then
                Return False
            End If

            If (SARC.IsPostProcessingComplete) Then
                Return False
            End If

            Return True
        End Function

        Protected Function CanShowPostProcessingData() As Boolean
            If (Not SARC.WorkflowStatus.StatusCodeType.IsFinal OrElse SARC.WorkflowStatus.StatusCodeType.IsCancel) Then
                Return False
            End If

            If (SARC.FinalFindings = 0) Then
                Return False
            End If

            Return True
        End Function

        Protected Sub ddlWingSARC_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ddlWingSARC.SelectedIndexChanged
            ResetContactInfoFields()

            If (ddlWingSARC.SelectedValue < 1) Then
                Exit Sub
            End If

            Dim PM As AppUser = UserService.GetById(ddlWingSARC.SelectedValue)

            If (PM Is Nothing) Then
                Exit Sub
            End If

            If (PM.Address IsNot Nothing) Then
                If (Not String.IsNullOrEmpty(PM.Address.Street)) Then
                    SetTextboxText(txtAppealStreet, PM.Address.Street)
                End If

                If (Not String.IsNullOrEmpty(PM.Address.City)) Then
                    SetTextboxText(txtAppealCity, PM.Address.City)
                End If

                If (Not String.IsNullOrEmpty(PM.Address.State)) Then
                    SetTextboxText(txtAppealState, PM.Address.State)
                End If

                If (Not String.IsNullOrEmpty(PM.Address.State)) Then
                    SetTextboxText(txtAppealZip, PM.Address.Zip)
                End If

                If (String.IsNullOrEmpty(PM.Address.Country)) Then
                    SetTextboxText(txtAppealCountry, "US")
                Else
                    SetTextboxText(txtAppealCountry, PM.Address.Country)
                End If
            End If

            If (Not String.IsNullOrEmpty(PM.Email)) Then
                SetTextboxText(txtEmail, PM.Email)
            End If

            If (Not String.IsNullOrEmpty(PM.Phone)) Then
                SetTextboxText(txtHelpExtensionNumber, PM.Phone)
            End If
        End Sub

        Protected Sub DigitiallySignButton_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnDigitallySign.Click
            SaveData()
            ucPostProcessingSigBlock.StartSignature(SARC.Id, SARC.Workflow, 0, "Sign Post Processing Information", SARC.Status, SARC.Status, 0, DBSignTemplateId.Form348SARCPostProcessing, String.Empty)
        End Sub

        Protected Sub GetData()
            SetLabelText(lblName, SARC.MemberName)

            If SARC.MemberRank IsNot Nothing Then
                SetLabelText(lblRank, SARC.MemberRank.Title)
            End If

            SetLabelText(lblCompo, Utility.GetCompoString(SARC.MemberCompo))
            SetLabelText(lblUnit, SARC.MemberUnit)
            SetDateLabel(lbldob, SARC.MemberDOB)

            InitPostProcessingControls()
        End Sub

        Protected Sub InitChangeUnitButton()
            If (UserHasPermission("sysAdmin") AndAlso CByte(Session("GroupId")) <> UserGroups.WingSarc) Then
                ChangeUnitButton.Attributes.Add("onclick", "showSearcher('Select New Unit'); return false;")
                ucUnitChangeSigBlock.Visible = False
            Else
                ChangeUnitButton.Visible = False
                ucUnitChangeSigBlock.Visible = False
            End If
        End Sub

        Protected Sub InitPostProcessingControls()
            If (Not CanShowPostProcessingData()) Then
                Exit Sub
            End If

            SetPostProcessingInputRestrictions()
            LoadPostProcessingData()
            LoadWingSARCPostProcessingSignature()
            SetPostProcessingControlsVisibility()
            InitWingSARCDropdownList()
            btnDigitallySign.Enabled = CanDigitallySign()
        End Sub

        Protected Sub InitWingSARCDropdownList()
            Dim availableWingSARCS = DaoFactory.GetLookupDao().Get_WingSARCs(SARC.MemberUnitId)

            If (availableWingSARCS.Count = 0) Then
                availableWingSARCS.Add(New LookUpItem() With {.Name = "No Wing SARCs available", .Value = 0})
            End If

            ddlWingSARC.DataSource = availableWingSARCS
            ddlWingSARC.DataTextField = "Name"
            ddlWingSARC.DataValueField = "Value"
            ddlWingSARC.DataBind()

            InsertDropDownListZeroValue(ddlWingSARC, "--- Select One ---")
        End Sub

        Protected Sub LoadPostProcessingData()
            If (SARCPostProcessing IsNot Nothing) Then
                SetTextboxText(txtHelpExtensionNumber, SARCPostProcessing.HelpExtensionNumber)
                SetLabelText(lblHelpExtensionNumber, SARCPostProcessing.HelpExtensionNumber)
                SetLabelText(lblAppealAddress, SARCPostProcessing.AppealAddress.FullAddress)
                SetTextboxText(txtAppealStreet, SARCPostProcessing.AppealAddress.Street)
                SetTextboxText(txtAppealCity, SARCPostProcessing.AppealAddress.City)
                SetTextboxText(txtAppealState, SARCPostProcessing.AppealAddress.State)
                SetTextboxText(txtAppealZip, SARCPostProcessing.AppealAddress.Zip)
                SetTextboxText(txtAppealCountry, SARCPostProcessing.AppealAddress.Country)
                SetTextboxText(txtEmail, SARCPostProcessing.email)
                SetLabelText(lblEmail, SARCPostProcessing.email)

                If (SARCPostProcessing.MemberNotified.HasValue) Then
                    MemberInformedCheckBox.Checked = SARCPostProcessing.MemberNotified.Value
                End If

                SetDateTextbox(NotificationDate, SARCPostProcessing.MemberNotificationDate)
                SetDateLabel(NotificationDatelbl, SARCPostProcessing.MemberNotificationDate)

                If (SARCPostProcessing.MemberNotified.HasValue AndAlso SARCPostProcessing.MemberNotified.Value) Then
                    SetLabelText(MemberNotifiedLabel, "Yes")
                    MemberInformedCheckBox.Enabled = False
                Else
                    SetLabelText(MemberNotifiedLabel, "No")
                End If
            End If

            If (SARC.NotificationMemoCreated(DaoFactory.GetMemoDao2())) Then
                SetLabelText(NotificationMemoCreatedLabel, "Yes")
            Else
                SetLabelText(NotificationMemoCreatedLabel, "No")
            End If
        End Sub

        Protected Sub MemberInformedCheckBox_CheckedChanged(sender As Object, e As System.EventArgs) Handles MemberInformedCheckBox.CheckedChanged
            If (Not UserHasPermission("RSARCPostCompletion")) Then
                Exit Sub
            End If

            If (MemberInformedCheckBox.Checked) Then
                LogManager.LogAction(ModuleType, UserAction.PostCompletion, SARC.Id, "Post Completion: " + SESSION_GROUP_NAME + " Informed Member")
                MemberInformedCheckBox.Enabled = False
                btnDigitallySign.Enabled = CanDigitallySign()
            End If
        End Sub

        Protected Sub Page_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
            AddHandler Me.Master.TabClick, AddressOf TabButtonClicked
        End Sub

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
            If (Not IsPostBack) Then
                SigCheck.VerifySignature(refId)
                GetData()
                TabControl.Item(NavigatorButtonType.Save).Visible = False
                InitChangeUnitButton()
                CaseHistory.Initialize(Me, SARC.MemberSSN, SARC.CaseId, False)

                LogManager.LogAction(ModuleType, UserAction.ViewPage, refId, "Viewed Page: SARC Member")
            End If
        End Sub

        Protected Sub ResetContactInfoFields()
            txtAppealStreet.Text = ""
            txtAppealCity.Text = ""
            txtAppealState.Text = ""
            txtAppealZip.Text = ""
            txtAppealCountry.Text = ""
            txtEmail.Text = ""
            txtHelpExtensionNumber.Text = ""
        End Sub

        Protected Sub SaveData()
            If (CanSavePostProcessingData()) Then
                SavePostProcessingData()
            End If
        End Sub

        Protected Sub SavePostProcessingData()
            Dim input As Double = 0
            lblError.Text = ""

            If (SARCPostProcessing Is Nothing) Then
                SARCPostProcessing = New RestrictedSARCPostProcessing()
                SARCPostProcessing.Id = SARC.Id
            End If

            SARCPostProcessing.HelpExtensionNumber = Server.HtmlEncode(txtHelpExtensionNumber.Text)
            If (String.IsNullOrEmpty(txtHelpExtensionNumber.Text)) Then
                lblError.Text = lblError.Text + "No Wing SARC Phone Number <br />"
            Else
                input += 1.0F
            End If

            SARCPostProcessing.AppealAddress.Street = Server.HtmlEncode(txtAppealStreet.Text)
            If (String.IsNullOrEmpty(txtAppealStreet.Text)) Then
                lblError.Text = lblError.Text + "No Wing SARC Address Street name <br />"
            Else
                input += 0.2
            End If

            SARCPostProcessing.AppealAddress.City = Server.HtmlEncode(txtAppealCity.Text)
            If (String.IsNullOrEmpty(txtAppealCity.Text)) Then
                lblError.Text = lblError.Text + "No Wing SARC Address City name <br />"
            Else
                input += 0.2
            End If

            SARCPostProcessing.AppealAddress.State = Server.HtmlEncode(txtAppealState.Text)
            If (String.IsNullOrEmpty(txtAppealState.Text)) Then
                lblError.Text = lblError.Text + "No Wing SARC Address State name <br />"
            Else
                input += 0.2
            End If

            SARCPostProcessing.AppealAddress.Zip = Server.HtmlEncode(txtAppealZip.Text)
            If (String.IsNullOrEmpty(txtAppealZip.Text)) Then
                lblError.Text = lblError.Text + "No Wing SARC Address Zip number <br />"
            Else
                input += 0.2
            End If

            SARCPostProcessing.AppealAddress.Country = Server.HtmlEncode(txtAppealCountry.Text)
            If (String.IsNullOrEmpty(txtAppealCountry.Text)) Then
                lblError.Text = lblError.Text + "No Wing SARC Address Country name <br />"
            Else
                input += 0.2
            End If

            SARCPostProcessing.email = Server.HtmlEncode(txtEmail.Text)
            If (String.IsNullOrEmpty(txtEmail.Text)) Then
                lblError.Text = lblError.Text + "No Wing SARC Email <br />"
            Else
                input += 1
            End If

            If (Not String.IsNullOrEmpty(NotificationDate.Text)) Then
                SARCPostProcessing.MemberNotificationDate = Server.HtmlEncode(DateTime.Parse(NotificationDate.Text.Trim))
            End If

            If (input > 0) Then
                trError.Visible = False

                If (Not SARCPostProcessing.MemberNotified.HasValue OrElse SARCPostProcessing.MemberNotified.Value = False) Then
                    SARCPostProcessing.MemberNotified = MemberInformedCheckBox.Checked()
                End If
            Else
                trError.Visible = True
            End If

            SARCPostProcessingDao.InsertOrUpdate(SARCPostProcessing)
            SARC.UpdateIsPostProcessingComplete(DaoFactory)
            SARCDao.SaveOrUpdate(SARC)
        End Sub

        Protected Sub SaveUnitButton_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles SaveUnitButton.Click
            If (Not UserHasPermission(PERMISSION_SYSTEM_ADMIN)) Then
                Exit Sub
            End If

            Dim newUnitId As Integer = 0

            If (Not Integer.TryParse(newUnitIDLabel.Text.Trim, newUnitId)) Then
                Exit Sub
            End If

            Dim newUnit As Unit = DaoFactory.GetUnitDao().FindById(newUnitId)

            If (newUnit Is Nothing) Then
                Exit Sub
            End If

            If (SARC Is Nothing) Then
                Exit Sub
            End If

            If (SARC.MemberUnitId <> newUnit.Id) Then
                LogManager.LogAction(ModuleType, UserAction.UpdateCaseUnit, SARC.Id, "Unit changed from: " + SARC.MemberUnit + " to " + newUnit.Name, SARC.Status)

                SARC.MemberUnitId = newUnit.Id
                SARC.MemberUnit = newUnit.Name
                lblUnit.Text = newUnit.Name

                SARCDao.SaveOrUpdate(SARC)

                ucUnitChangeSigBlock.StartSignature(refId, SARC.Workflow, 0, "Updated Member unit", SARC.Status, SARC.Status, 0, DBSignTemplateId.SignOnly, String.Empty)
            End If
        End Sub

        Protected Sub SetPostProcessingControlsVisibility()
            pnlPostCompletion.Visible = True

            If (UserHasPermission("RSARCPostCompletion") AndAlso Not SARC.IsPostProcessingComplete) Then
                trWingSARC.Visible = True
                txtHelpExtensionNumber.Visible = True
                txtAppealStreet.Visible = True
                txtAppealCity.Visible = True
                txtAppealState.Visible = True
                txtAppealZip.Visible = True
                txtAppealCountry.Visible = True
                MemberInformedCheckBox.Visible = True
                NotificationDate.Visible = True
                trSave.Visible = True
                trDigitallySign.Visible = True
                txtEmail.Visible = True
                ucPostProcessingSigBlock.Visible = True
            Else
                lblHelpExtensionNumber.Visible = True
                lblAppealAddress.Visible = True
                lblEmail.Visible = True
                MemberNotifiedLabel.Visible = True
                NotificationDatelbl.Visible = True
                ucPostProcessingSigBlock.Visible = False
            End If
        End Sub

        Protected Sub SetPostProcessingInputRestrictions()
            SetMaxLength(txtHelpExtensionNumber)

            SetInputFormatRestriction(Page, txtHelpExtensionNumber, FormatRestriction.AlphaNumeric, PERMITTED_SPECIAL_CHAR_INPUT)
            SetInputFormatRestriction(Page, txtAppealStreet, FormatRestriction.AlphaNumeric, PERMITTED_SPECIAL_CHAR_INPUT)
            SetInputFormatRestriction(Page, txtAppealCity, FormatRestriction.AlphaNumeric, PERMITTED_SPECIAL_CHAR_INPUT)
            SetInputFormatRestriction(Page, txtAppealState, FormatRestriction.AlphaNumeric, PERMITTED_SPECIAL_CHAR_INPUT)
            SetInputFormatRestriction(Page, txtAppealZip, FormatRestriction.Numeric, "-")
        End Sub

        Protected Sub SignatureCompleted(ByVal sender As Object, ByVal e As SignCompletedEventArgs) Handles ucPostProcessingSigBlock.SignCompleted
            If (Not e.SignaturePassed) Then
                Exit Sub
            End If

            AddPostCompletionDigitalSignatureMetaData()
            SARC.UpdateIsPostProcessingComplete(DaoFactory)
            SARCDao.SaveOrUpdate(SARC)

            LogManager.LogAction(ModuleType, UserAction.PostCompletion, SARC.Id, "Post Completion: " + SESSION_GROUP_NAME + " Digital Signature Generated")

            SetFeedbackMessage("Case " + SARC.CaseId + " successfully signed.  Action applied: " + e.Text)
            Response.Redirect(Resources._Global.StartPage, True)
        End Sub

        Protected Sub TabButtonClicked(ByRef sender As Object, ByRef e As TabNavigationEventArgs)
            If (e.ButtonType = NavigatorButtonType.Save OrElse
                e.ButtonType = NavigatorButtonType.NavigatedAway OrElse
                e.ButtonType = NavigatorButtonType.NextStep OrElse
                e.ButtonType = NavigatorButtonType.PreviousStep) Then
                SaveData()
            End If
        End Sub

        Private Sub LoadWingSARCPostProcessingSignature()
            ucSigCheckWingSARCPostCompletion.VerifySignature(SARC.Id)
        End Sub

#End Region

    End Class

End Namespace