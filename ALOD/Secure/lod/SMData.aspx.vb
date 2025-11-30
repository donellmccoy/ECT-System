Imports ALOD.Core.Domain.Common
Imports ALOD.Core.Domain.DBSign
Imports ALOD.Core.Domain.Modules.Lod
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

Namespace Web.LOD

    Partial Class Secure_lod_SMData
        Inherits System.Web.UI.Page

        Private _dao As ILineOfDutyDao
        Private _daoFactory As IDaoFactory
        Private _lod As LineOfDuty
        Private _postdao As ILineOfDutyPostProcessingDao
        Private _postprocessing As LineOfDutyPostProcessing
        Private _rdao As ILODReinvestigateDAO
        Private _reinvestigation As Integer = 0
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

        Protected ReadOnly Property LOD() As LineOfDuty
            Get
                If (_lod Is Nothing) Then
                    _lod = LODDao.GetById(refId, False)

                End If
                Return _lod
            End Get
        End Property

        Protected ReadOnly Property LODDao() As ILineOfDutyDao
            Get
                If (_dao Is Nothing) Then
                    _dao = DaoFactory.GetLineOfDutyDao()
                End If

                Return _dao
            End Get
        End Property

        Protected ReadOnly Property ModuleType() As ModuleType
            Get
                Return ModuleType.LOD
            End Get
        End Property

        Protected ReadOnly Property Navigator() As TabNavigator
            Get
                Return Me.Master.Navigator
            End Get
        End Property

        Protected ReadOnly Property PostProcessing() As LineOfDutyPostProcessing
            Get
                If (_postprocessing Is Nothing) Then
                    _postprocessing = PostProcessingDao.GetById(LOD.Id)
                End If

                If (_postprocessing Is Nothing) Then
                    _postprocessing = New LineOfDutyPostProcessing(LOD.Id)
                End If

                Return _postprocessing
            End Get
        End Property

        Protected ReadOnly Property PostProcessingDao() As ILineOfDutyPostProcessingDao
            Get
                If (_postdao Is Nothing) Then
                    _postdao = DaoFactory.GetLineOfDutyPostProcessingDao()
                End If

                Return _postdao
            End Get
        End Property

        Protected ReadOnly Property refId() As Integer
            Get
                Return Integer.Parse(Request.QueryString("refId"))
            End Get
        End Property

        Protected ReadOnly Property reinvestigationRequestId() As Integer
            Get
                If (_reinvestigation = 0) Then
                    _reinvestigation = RRDao.GetReinvestigationRequestIdByRLod(LOD.Id)

                End If
                Return _reinvestigation
            End Get
        End Property

        Protected ReadOnly Property RRDao() As ILODReinvestigateDAO
            Get
                If (_rdao Is Nothing) Then
                    _rdao = DaoFactory.GetLODReinvestigationDao()
                End If

                Return _rdao
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

            LOD.AddSignature(DaoFactory, groupId, currentUser.SignatureTitle, currentUser)
        End Sub

        Protected Sub btnSavePostProcessingData_Click(sender As Object, e As EventArgs) Handles btnSavePostProcessingData.Click
            SaveData()
        End Sub

        Protected Function CanDigitallySign() As Boolean
            If (LOD.IsPostProcessingComplete) Then
                Return False
            End If

            If (Not MemberInformedCheckBox.Checked) Then
                Return False
            End If

            If (String.IsNullOrEmpty(NotificationDate.Text)) Then
                Return False
            End If

            If Session(SESSIONKEY_COMPO) = "6" Then
                If (Not LOD.AFRCNotificationMemoCreated(DaoFactory.GetMemoDao(), SESSIONKEY_COMPO)) Then
                    Return False
                End If
            ElseIf Session(SESSIONKEY_COMPO) = "5" Then
                If (Not LOD.ANGNotificationMemoCreated(DaoFactory.GetMemoDao(), SESSIONKEY_COMPO)) Then
                    Return False
                End If
            End If

            Return True
        End Function

        Protected Function CanInitPostProcessingControls() As Boolean
            If (Not LOD.WorkflowStatus.StatusCodeType.IsFinal OrElse LOD.WorkflowStatus.StatusCodeType.IsCancel) Then
                Return False
            End If

            If (Not LOD.FinalFindings.HasValue) Then
                Return False
            End If

            Return True
        End Function

        Protected Function CanSavePostProcessingData() As Boolean
            If (Not LOD.WorkflowStatus.StatusCodeType.IsFinal OrElse LOD.WorkflowStatus.StatusCodeType.IsCancel) Then
                Return False
            End If

            If (Not UserHasPostProcessingPermission()) Then
                Return False
            End If

            If (LOD.IsPostProcessingComplete) Then
                Return False
            End If

            Return True
        End Function

        Protected Sub ddlLODPM_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ddlLODPM.SelectedIndexChanged
            ResetContactInfoFields()

            If (ddlLODPM.SelectedValue < 1) Then
                Exit Sub
            End If

            Dim PM As AppUser = UserService.GetById(ddlLODPM.SelectedValue)

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
            Dim postProcessing As New LineOfDutyPostProcessingDao
            SaveData()
            ucPostProcessingSigBlock.StartSignature(LOD.Id, LOD.Workflow, 0, "Sign Post Processing Information", LOD.Status, LOD.Status, 0, DBSignTemplateId.Form348PostProcessing, String.Empty)
            postProcessing.SetPostProcessingComplete(refId)
        End Sub

        Protected Sub GetData()
            SetLabelText(lblName, LOD.MemberName)

            If LOD.MemberRank IsNot Nothing Then
                SetLabelText(lblRank, LOD.MemberRank.Title)
            End If

            SetLabelText(lblCompo, Utility.GetCompoString(LOD.MemberCompo))
            SetLabelText(lblUnit, LOD.MemberUnit)
            SetDateLabel(lbldob, LOD.MemberDob)

            InitPostProcessingControls()
        End Sub

        Protected Sub InitPostProcessingControls()

            If (Not CanInitPostProcessingControls() And Not SESSION_WS_ID(refId) = 330) Then  'LOD.WorkflowStatus.Description.Equals("Complete (Pilot)")
                Exit Sub
            End If

            SetPostProcessingControlsInputRestrictions()
            LoadPostProcessingData()
            LoadPostProcessingSignature()
            SetPostProcessingControlsVisibility()
            InitLODPMDropdownList()
            btnDigitallySign.Enabled = CanDigitallySign()
        End Sub

        Protected Sub LoadPostProcessingData()
            Dim rowLabel As String = "B"

            If (PostProcessing IsNot Nothing) Then
                SetTextboxText(txtHelpExtensionNumber, PostProcessing.HelpExtensionNumber)
                SetLabelText(lblHelpExtensionNumber, PostProcessing.HelpExtensionNumber)
                SetLabelText(lblAppealAddress, PostProcessing.AppealAddress.FullAddress)
                SetTextboxText(txtAppealStreet, PostProcessing.AppealAddress.Street)
                SetTextboxText(txtAppealCity, PostProcessing.AppealAddress.City)
                SetTextboxText(txtAppealState, PostProcessing.AppealAddress.State)
                SetTextboxText(txtAppealZip, PostProcessing.AppealAddress.Zip)
                SetTextboxText(txtAppealCountry, PostProcessing.AppealAddress.Country)
                SetLabelText(lblEmail, PostProcessing.email)
                SetTextboxText(txtEmail, PostProcessing.email)
                SetCheckBox(chkPhone, PostProcessing.chkPhone)
                SetCheckBox(chkEmail, PostProcessing.chkEmail)
                SetCheckBox(chkAddress, PostProcessing.chkAddress)
                ddlLODPM.SelectedValue = LOD.LODPM
                SetDateTextbox(NotificationDate, PostProcessing.NotificationDate)
                SetDateLabel(NotificationDatelbl, PostProcessing.NotificationDate)
            End If

            If (Not String.IsNullOrEmpty(LOD.LODMedical.DeathInvolved) AndAlso LOD.LODMedical.DeathInvolved = "Yes") Then
                trNOK.Visible = True
                SetLabelText(lblNOKRow, rowLabel)
                rowLabel = "C"

                If (PostProcessing IsNot Nothing) Then
                    SetLabelText(lblNOK, PostProcessing.NextOfKinFullName)
                    SetTextboxText(txtNOKFirstName, PostProcessing.NextOfKinFirstName)
                    SetTextboxText(txtNOKLastName, PostProcessing.NextOfKinLastName)
                    SetTextboxText(txtNOKMiddleName, PostProcessing.NextOfKinMiddleName)
                End If
            End If

            SetLabelText(lblMemberInformedRow, rowLabel)
            SetLabelText(lblMemberMemoCreatedRow, ENGLISH_ALPHABET_UPPERCASE(ENGLISH_ALPHABET_UPPERCASE.IndexOf(rowLabel) + 1))
            SetLabelText(lblMemberDateRow, ENGLISH_ALPHABET_UPPERCASE(ENGLISH_ALPHABET_UPPERCASE.IndexOf(lblMemberMemoCreatedRow.Text) + 1))
            SetLabelText(lblSaveRow, ENGLISH_ALPHABET_UPPERCASE(ENGLISH_ALPHABET_UPPERCASE.IndexOf(lblMemberDateRow.Text) + 1))
            SetLabelText(lblErrorRow, ENGLISH_ALPHABET_UPPERCASE(ENGLISH_ALPHABET_UPPERCASE.IndexOf(lblSaveRow.Text) + 1))

            MemberInformedCheckBox.Checked = LOD.memberNotified
            If (LOD.memberNotified) Then
                MemberInformedCheckBox.Enabled = False
                SetLabelText(MemberNotifiedLabel, "Yes")
            Else
                SetLabelText(MemberNotifiedLabel, "No")
            End If

            If Session(SESSIONKEY_COMPO) = "6" Then
                If (LOD.AFRCNotificationMemoCreated(DaoFactory.GetMemoDao(), SESSIONKEY_COMPO)) Then
                    SetLabelText(NotificationMemoCreatedLabel, "Yes")
                Else
                    SetLabelText(NotificationMemoCreatedLabel, "No")
                End If
            ElseIf Session(SESSIONKEY_COMPO) = "5" Then
                If (LOD.ANGNotificationMemoCreated(DaoFactory.GetMemoDao(), SESSIONKEY_COMPO)) Then
                    SetLabelText(NotificationMemoCreatedLabel, "Yes")
                Else
                    SetLabelText(NotificationMemoCreatedLabel, "No")
                End If
            End If

        End Sub

        Protected Sub MemberInformedCheckBox_CheckedChanged(sender As Object, e As System.EventArgs) Handles MemberInformedCheckBox.CheckedChanged
            If (Not UserHasPostProcessingPermission()) Then
                Exit Sub
            End If

            If (MemberInformedCheckBox.Checked) Then
                LogManager.LogAction(ModuleType, UserAction.PostCompletion, LOD.Id, "Post Completion: " + SESSION_GROUP_NAME + " Informed Member")

                MemberInformedCheckBox.Enabled = False
                btnDigitallySign.Enabled = CanDigitallySign()
            End If
        End Sub

        Protected Sub NotificationDate_Filled(sender As Object, e As EventArgs) Handles NotificationDate.TextChanged
            btnDigitallySign.Enabled = CanDigitallySign()
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
                CaseHistory.Initialize(Me, LOD.MemberSSN, LOD.CaseId, False)

                LogManager.LogAction(ModuleType, UserAction.ViewPage, refId, "Viewed Page: Member")
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
            Dim errorVisible As Boolean = False
            lblError.Text = ""

            PostProcessing.chkAddress = chkAddress.Checked
            PostProcessing.chkEmail = chkEmail.Checked
            PostProcessing.chkPhone = chkPhone.Checked

            If (Not (PostProcessing.chkAddress Or PostProcessing.chkEmail Or PostProcessing.chkPhone)) Then
                errorVisible = True
                lblError.Text = lblError.Text + "Need LOD PM Contact information to merge <br />"
            End If

            PostProcessing.HelpExtensionNumber = Server.HtmlEncode(txtHelpExtensionNumber.Text)
            If (PostProcessing.chkPhone AndAlso String.IsNullOrEmpty(txtHelpExtensionNumber.Text)) Then
                errorVisible = True
                lblError.Text = lblError.Text + "No LOD PM Phone Number <br />"
            End If

            PostProcessing.AppealAddress.Street = Server.HtmlEncode(txtAppealStreet.Text)
            If (PostProcessing.chkAddress AndAlso String.IsNullOrEmpty(txtAppealStreet.Text)) Then
                errorVisible = True
                lblError.Text = lblError.Text + "No LOD PM Address Street name <br />"
            End If

            PostProcessing.AppealAddress.City = Server.HtmlEncode(txtAppealCity.Text)
            If (PostProcessing.chkAddress AndAlso String.IsNullOrEmpty(txtAppealCity.Text)) Then
                errorVisible = True
                lblError.Text = lblError.Text + "No LOD PM Address City name <br />"
            End If

            PostProcessing.AppealAddress.State = Server.HtmlEncode(txtAppealState.Text)
            If (PostProcessing.chkAddress AndAlso String.IsNullOrEmpty(txtAppealState.Text)) Then
                errorVisible = True
                lblError.Text = lblError.Text + "No LOD PM Address State name <br />"
            End If

            PostProcessing.AppealAddress.Zip = Server.HtmlEncode(txtAppealZip.Text)
            If (PostProcessing.chkAddress AndAlso String.IsNullOrEmpty(txtAppealZip.Text)) Then
                errorVisible = True
                lblError.Text = lblError.Text + "No LOD PM Address Zip number <br />"
            End If

            PostProcessing.AppealAddress.Country = Server.HtmlEncode(txtAppealCountry.Text)
            If (PostProcessing.chkAddress AndAlso String.IsNullOrEmpty(txtAppealCountry.Text)) Then
                errorVisible = True
                lblError.Text = lblError.Text + "No LOD PM Address Country name <br />"
            End If

            PostProcessing.email = Server.HtmlEncode(txtEmail.Text)
            If (PostProcessing.chkEmail AndAlso String.IsNullOrEmpty(txtEmail.Text)) Then
                errorVisible = True
                lblError.Text = lblError.Text + "No LOD PM Email <br />"
            End If

            If (trNOK.Visible) Then
                PostProcessing.NextOfKinFirstName = Server.HtmlEncode(txtNOKFirstName.Text)
                If (String.IsNullOrEmpty(txtNOKFirstName.Text)) Then
                    errorVisible = True
                    lblError.Text = lblError.Text + "No First Name for Next of Kin <br />"
                End If

                PostProcessing.NextOfKinLastName = Server.HtmlEncode(txtNOKLastName.Text)
                If (String.IsNullOrEmpty(txtNOKLastName.Text)) Then
                    errorVisible = True
                    lblError.Text = lblError.Text + "No Last Name for Next of Kin <br />"
                End If

                PostProcessing.NextOfKinMiddleName = Server.HtmlEncode(txtNOKMiddleName.Text)
            End If

            If (Not String.IsNullOrEmpty(NotificationDate.Text)) Then
                PostProcessing.NotificationDate = Server.HtmlEncode(DateTime.Parse(NotificationDate.Text.Trim))
            End If

            If (LOD.memberNotified = False) Then
                LOD.memberNotified = MemberInformedCheckBox.Checked()
            End If

            If (ddlLODPM.SelectedIndex > 0) Then
                LOD.LODPM = ddlLODPM.SelectedValue
            End If

            If (errorVisible) Then
                trError.Visible = True
            Else
                trError.Visible = False
            End If

            PostProcessingDao.InsertOrUpdate(PostProcessing)
            LOD.UpdateIsPostProcessingComplete(DaoFactory)
            LODDao.SaveOrUpdate(LOD)
        End Sub

        Protected Sub SaveUnitButton_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles SaveUnitButton.Click
            If (Not UserHasPermissionToChangeCaseUnit()) Then
                Exit Sub
            End If

            Dim newUnitId As Integer = 0

            If (Not Integer.TryParse(newUnitIDLabel.Text.Trim, newUnitId)) Then
                Exit Sub
            End If

            Dim newUnit As Unit = DaoFactory.GetUnitDao().FindById(newUnitId)

            If (newUnit Is Nothing OrElse LOD Is Nothing) Then
                Exit Sub
            End If

            ChangeLodUnit(newUnit)
        End Sub

        Protected Sub SetPostProcessingControlsInputRestrictions()
            SetInputFormatRestriction(Page, txtHelpExtensionNumber, FormatRestriction.Numeric)
            SetInputFormatRestriction(Page, txtAppealStreet, FormatRestriction.AlphaNumeric, PERMITTED_SPECIAL_CHAR_INPUT)
            SetInputFormatRestriction(Page, txtAppealCity, FormatRestriction.AlphaNumeric, PERMITTED_SPECIAL_CHAR_INPUT)
            SetInputFormatRestriction(Page, txtAppealState, FormatRestriction.AlphaNumeric, PERMITTED_SPECIAL_CHAR_INPUT)
            SetInputFormatRestriction(Page, txtAppealZip, FormatRestriction.Numeric, "-")
            SetInputFormatRestriction(Page, txtAppealCountry, FormatRestriction.AlphaNumeric, PERMITTED_SPECIAL_CHAR_INPUT)
            SetInputFormatRestriction(Page, txtNOKFirstName, FormatRestriction.Alpha, PERMITTED_SPECIAL_CHAR_INPUT)
            SetInputFormatRestriction(Page, txtNOKLastName, FormatRestriction.Alpha, PERMITTED_SPECIAL_CHAR_INPUT)
            SetInputFormatRestriction(Page, txtNOKMiddleName, FormatRestriction.Alpha, PERMITTED_SPECIAL_CHAR_INPUT)
        End Sub

        Protected Sub SetPostProcessingControlsVisibility()
            pnlPostCompletion.Visible = True

            If (UserHasPostProcessingPermission() AndAlso Not LOD.IsPostProcessingComplete) Then
                trLODPM.Visible = True
                chkAddress.Enabled = True
                chkEmail.Enabled = True
                chkPhone.Enabled = True
                txtAppealStreet.Visible = True
                txtAppealCity.Visible = True
                txtAppealState.Visible = True
                txtAppealZip.Visible = True
                txtAppealCountry.Visible = True
                txtEmail.Visible = True
                txtHelpExtensionNumber.Visible = True
                lblNOK.Visible = False
                txtNOKFirstName.Visible = True
                txtNOKLastName.Visible = True
                txtNOKMiddleName.Visible = True
                MemberInformedCheckBox.Visible = True
                NotificationDate.Visible = True
                trSave.Visible = True
                trDigitallySign.Visible = True
                ucPostProcessingSigBlock.Visible = True
            Else
                lblAppealAddress.Visible = True
                lblEmail.Visible = True
                lblHelpExtensionNumber.Visible = True
                lblNOK.Visible = True
                txtNOKFirstName.Visible = False
                txtNOKLastName.Visible = False
                txtNOKMiddleName.Visible = False
                MemberNotifiedLabel.Visible = True
                NotificationDatelbl.Visible = True
                ucPostProcessingSigBlock.Visible = False
            End If
        End Sub

        Protected Sub SignatureCompleted(ByVal sender As Object, ByVal e As SignCompletedEventArgs) Handles ucPostProcessingSigBlock.SignCompleted
            If (Not e.SignaturePassed) Then
                Exit Sub
            End If

            AddPostCompletionDigitalSignatureMetaData()
            LOD.UpdateIsPostProcessingComplete(DaoFactory)
            LODDao.SaveOrUpdate(LOD)

            LogManager.LogAction(ModuleType, UserAction.PostCompletion, LOD.Id, "Post Completion: " + SESSION_GROUP_NAME + " Digital Signature Generated")

            SetFeedbackMessage("Case " + LOD.CaseId + " successfully signed.  Action applied: " + e.Text)
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

        Protected Function UserHasPostProcessingPermission() As Boolean
            If (LOD.SarcCase AndAlso LOD.IsRestricted) Then
                ' This is a legacy restricted SARC case, so use the Restricted SARC workflow post processing permission...
                Return UserHasPermission(PERMISSION_SARC_POSTPROCESSING)
            Else
                Return UserHasPermission(PERMISSION_EXECUTE_LOD_POST_COMPLETION)
            End If
        End Function

        Private Sub ChangeLodUnit(newUnit As Unit)
            If (LOD.MemberUnitId = newUnit.Id) Then
                Exit Sub
            End If

            LogManager.LogAction(ModuleType, UserAction.UpdateCaseUnit, LOD.Id, "Unit changed from: " + LOD.MemberUnit + " to " + newUnit.Name, LOD.Status)

            LOD.MemberUnitId = newUnit.Id
            LOD.MemberUnit = newUnit.Name
            lblUnit.Text = newUnit.Name

            LODDao.SaveOrUpdate(LOD)

            ucUnitChangeSigBlock.StartSignature(refId, LOD.Workflow, 0, "Updated Member unit", LOD.Status, LOD.Status, 0, DBSignTemplateId.Form348, "")
        End Sub

        Private Sub InitChangeUnitButton()
            If (UserHasPermissionToChangeCaseUnit()) Then
                ChangeUnitButton.Attributes.Add("onclick", "showSearcher('Select New Unit'); return false;")
                ucUnitChangeSigBlock.Visible = True
            Else
                ChangeUnitButton.Visible = False
                ucUnitChangeSigBlock.Visible = False
            End If
        End Sub

        Private Sub InitLODPMDropdownList()
            Dim availableLODPMs = DaoFactory.GetLookupDao().Get_LODPMs(LOD.MemberUnitId)

            If (availableLODPMs.Count = 0) Then
                availableLODPMs.Add(New LookUpItem() With {.Name = "No LOD PMs available", .Value = 0})
            End If

            ddlLODPM.DataSource = availableLODPMs
            ddlLODPM.DataTextField = "Name"
            ddlLODPM.DataValueField = "Value"
            Try
                ddlLODPM.DataBind()
            Catch
            End Try

            InsertDropDownListZeroValue(ddlLODPM, "--- Select One ---")
        End Sub

        Private Sub LoadPostProcessingSignature()
            ucPostCompletionSigCheck.VerifySignature(LOD.Id)
        End Sub

        Private Function UserHasPermissionToChangeCaseUnit() As Boolean
            If (Not UserHasPermission(PERMISSION_SYSTEM_ADMIN)) Then
                Return False
            End If

            If (reinvestigationRequestId > 0) Then
                Return False
            End If

            Return True
        End Function

#End Region

    End Class

End Namespace