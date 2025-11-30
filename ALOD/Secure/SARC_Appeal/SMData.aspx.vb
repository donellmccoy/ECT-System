Imports ALOD.Core.Domain.Common
Imports ALOD.Core.Domain.DBSign
Imports ALOD.Core.Domain.Modules.Lod
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

Namespace Web.APSA

    Partial Class Secure_apsa_SMData
        Inherits System.Web.UI.Page

        Private _adao As ISARCAppealDAO
        Private _appeal As SARCAppeal
        Private _dao As ILineOfDutyDao
        Private _daoFactory As IDaoFactory
        Private _lod As LineOfDuty
        Private _postdao As ISARCAppealPostProcessingDAO
        Private _postprocessing As SARCAppealPostProcessing
        Private _sarc As RestrictedSARC
        Private _sarcdao As ISARCDAO
        Private _signatureMetaDataDao As ISignatueMetaDateDao

#Region "Properties"

        Protected ReadOnly Property APDao() As ISARCAppealDAO
            Get
                If (_adao Is Nothing) Then
                    _adao = DaoFactory.GetSARCAppealDao()
                End If

                Return _adao
            End Get
        End Property

        Protected ReadOnly Property AppealPostDao() As ISARCAppealPostProcessingDAO
            Get
                If (_postdao Is Nothing) Then
                    _postdao = DaoFactory.GetSARCAppealPostProcessingDao()
                End If

                Return _postdao
            End Get
        End Property

        Protected ReadOnly Property CalendarImage() As String
            Get
                Return GetCalendarImage(Me)
            End Get
        End Property

        Protected ReadOnly Property Dao() As ILineOfDutyDao
            Get
                If (_dao Is Nothing) Then
                    _dao = DaoFactory.GetLineOfDutyDao()
                End If

                Return _dao
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
                    _lod = Dao.GetById(SARCAppealRequest.InitialId, False)

                End If
                Return _lod
            End Get
        End Property

        Protected ReadOnly Property ModuleType() As ModuleType
            Get
                Return ModuleType.SARCAppeal
            End Get
        End Property

        Protected ReadOnly Property Navigator() As TabNavigator
            Get
                Return Me.Master.Navigator
            End Get
        End Property

        Protected ReadOnly Property PostProcessing() As SARCAppealPostProcessing
            Get
                If (_postprocessing Is Nothing) Then
                    _postprocessing = AppealPostDao.GetById(SARCAppealRequest.Id)
                End If

                If (_postprocessing Is Nothing) Then
                    _postprocessing = New SARCAppealPostProcessing(SARCAppealRequest.Id)
                End If

                Return _postprocessing
            End Get
        End Property

        Protected ReadOnly Property RequestId() As Integer
            Get
                Return Integer.Parse(Request.QueryString("requestId"))
            End Get
        End Property

        Protected ReadOnly Property SARC() As RestrictedSARC
            Get
                If (_sarc Is Nothing) Then
                    _sarc = SARCDao.GetById(SARCAppealRequest.InitialId, False)

                End If
                Return _sarc
            End Get
        End Property

        Protected ReadOnly Property SARCAppealRequest() As SARCAppeal
            Get
                If (_appeal Is Nothing) Then
                    _appeal = APDao.GetById(RequestId, False)

                End If
                Return _appeal
            End Get
        End Property

        Protected ReadOnly Property SARCDao() As ISARCDAO
            Get
                If (_sarcdao Is Nothing) Then
                    _sarcdao = DaoFactory.GetSARCDao()
                End If

                Return _sarcdao
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

            SARCAppealRequest.AddSignature(DaoFactory, groupId, currentUser.SignatureTitle, currentUser)
        End Sub

        Protected Sub btnSavePostProcessingData_Click(sender As Object, e As EventArgs) Handles btnSavePostProcessingData.Click
            SaveData()
        End Sub

        Protected Function CanDigitallySign() As Boolean
            If (SARCAppealRequest.IsPostProcessingComplete) Then
                Return False
            End If

            If (Not MemberInformedCheckBox.Checked) Then
                Return False
            End If

            If (String.IsNullOrEmpty(NotificationDate.Text)) Then
                Return False
            End If

            If (IsEmptyContactInfo()) Then
                Return False
            End If

            Return True
        End Function

        Protected Function CanInitPostProcessingControls() As Boolean
            If (Not SARCAppealRequest.WorkflowStatus.StatusCodeType.IsFinal OrElse SARCAppealRequest.WorkflowStatus.StatusCodeType.IsCancel) Then
                Return False
            End If

            Return True
        End Function

        Protected Function CanSavePostProcessingData() As Boolean
            If (Not SARCAppealRequest.WorkflowStatus.StatusCodeType.IsFinal AndAlso Not SARCAppealRequest.WorkflowStatus.StatusCodeType.IsCancel) Then
                Return False
            End If

            If (Not UserHasPermission("RSARCAppealPostCompletion")) Then
                Return False
            End If

            If (SARCAppealRequest.IsPostProcessingComplete) Then
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
            ucPostProcessingSigBlock.StartSignature(SARCAppealRequest.Id, SARCAppealRequest.Workflow, 0, "Sign Post Processing Information", SARCAppealRequest.Status, SARCAppealRequest.Status, 0, DBSignTemplateId.Form348SARCAppealPostProcessing, String.Empty)
        End Sub

        Protected Function GetMemberUnitFromOriginalCase() As Integer
            If (SARCAppealRequest.InitialWorkflow = AFRCWorkflows.LOD) Then
                Return LOD.MemberUnitId
            Else
                Return SARC.MemberUnitId
            End If
        End Function

        Protected Sub InitSigCheckControl()
            If (Not SARCAppealRequest.IsNonDBSignCase) Then
                SigCheck.VerifySignature(RequestId)
                SigCheck.Visible = True
            Else
                SigCheck.Visible = False
            End If
        End Sub

        Protected Sub InitWingSARCDropdownList()
            Dim availableWingSARCS = DaoFactory.GetLookupDao().Get_WingSARCs(GetMemberUnitFromOriginalCase())

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
            If (PostProcessing IsNot Nothing) Then
                SetTextboxText(txtAppealStreet, PostProcessing.AppealAddress.Street)
                SetTextboxText(txtAppealCity, PostProcessing.AppealAddress.City)
                SetTextboxText(txtAppealState, PostProcessing.AppealAddress.State)
                SetTextboxText(txtAppealZip, PostProcessing.AppealAddress.Zip)
                SetTextboxText(txtAppealCountry, PostProcessing.AppealAddress.Country)
                SetLabelText(lblAppealAddress, PostProcessing.AppealAddress.FullAddress)
                SetDateTextbox(NotificationDate, PostProcessing.NotificationDate)
                SetDateLabel(NotificationDatelbl, PostProcessing.NotificationDate)
                SetLabelText(lblEmail, PostProcessing.email)
                SetTextboxText(txtEmail, PostProcessing.email)
                SetTextboxText(txtHelpExtensionNumber, PostProcessing.HelpExtensionNumber)
                SetLabelText(lblHelpExtensionNumber, PostProcessing.HelpExtensionNumber)
            End If

            If (SARCAppealRequest.MemberNotified) Then
                MemberInformedCheckBox.Checked = SARCAppealRequest.MemberNotified
                MemberInformedCheckBox.Enabled = False
                SetLabelText(MemberNotifiedLabel, "Yes")
            Else
                SetLabelText(MemberNotifiedLabel, "No")
            End If
        End Sub

        Protected Sub MemberInformedCheckBox_CheckedChanged(sender As Object, e As System.EventArgs) Handles MemberInformedCheckBox.CheckedChanged
            If (Not UserHasPermission("RSARCAppealPostCompletion")) Then
                Exit Sub
            End If

            If (MemberInformedCheckBox.Checked) Then
                LogManager.LogAction(ModuleType, UserAction.PostCompletion, SARCAppealRequest.Id, "Post Completion: " + SESSION_GROUP_NAME + " Informed Member")

                MemberInformedCheckBox.Enabled = False
                btnDigitallySign.Enabled = CanDigitallySign()
            End If
        End Sub

        Protected Sub Page_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
            AddHandler Me.Master.TabClick, AddressOf TabButtonClicked
        End Sub

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
            If (Not IsPostBack) Then
                InitSigCheckControl()
                GetData()
                TabControl.Item(NavigatorButtonType.Save).Visible = False
                CaseHistory.Initialize(Me, SARCAppealRequest.MemberSSN, SARCAppealRequest.CaseId, False)

                LogManager.LogAction(ModuleType, UserAction.ViewPage, RequestId, "Viewed Page: Member")
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
                btnDigitallySign.Enabled = CanDigitallySign()
            End If
        End Sub

        Protected Sub SetPostProcessingControlsInputRestrictions()
            SetInputFormatRestriction(Page, txtAppealStreet, FormatRestriction.AlphaNumeric, PERMITTED_SPECIAL_CHAR_INPUT)
            SetInputFormatRestriction(Page, txtAppealCity, FormatRestriction.AlphaNumeric, PERMITTED_SPECIAL_CHAR_INPUT)
            SetInputFormatRestriction(Page, txtAppealState, FormatRestriction.AlphaNumeric, PERMITTED_SPECIAL_CHAR_INPUT)
            SetInputFormatRestriction(Page, txtAppealZip, FormatRestriction.Numeric, "-")
            SetInputFormatRestriction(Page, txtAppealCountry, FormatRestriction.AlphaNumeric, PERMITTED_SPECIAL_CHAR_INPUT)
            SetInputFormatRestriction(Page, txtHelpExtensionNumber, FormatRestriction.Numeric)
        End Sub

        Protected Sub SetPostProcessingControlsVisibility()
            pnlPostCompletion.Visible = True

            If (UserHasPermission("RSARCAppealPostCompletion") AndAlso Not SARCAppealRequest.IsPostProcessingComplete) Then
                trWingSARC.Visible = True
                txtAppealStreet.Visible = True
                txtAppealCity.Visible = True
                txtAppealState.Visible = True
                txtAppealZip.Visible = True
                txtAppealCountry.Visible = True
                txtEmail.Visible = True
                MemberInformedCheckBox.Visible = True
                NotificationDate.Visible = True
                trSave.Visible = True
                trDigitallySign.Visible = True
                txtHelpExtensionNumber.Visible = True
                ucPostProcessingSigBlock.Visible = True
            Else
                lblAppealAddress.Visible = True
                MemberNotifiedLabel.Visible = True
                NotificationDatelbl.Visible = True
                lblEmail.Visible = True
                lblHelpExtensionNumber.Visible = True
                ucPostProcessingSigBlock.Visible = False
            End If
        End Sub

        Protected Sub SignatureCompleted(ByVal sender As Object, ByVal e As SignCompletedEventArgs) Handles ucPostProcessingSigBlock.SignCompleted
            If (Not e.SignaturePassed) Then
                Exit Sub
            End If

            AddPostCompletionDigitalSignatureMetaData()
            SARCAppealRequest.UpdateIsPostProcessingComplete(DaoFactory)
            APDao.SaveOrUpdate(SARCAppealRequest)

            LogManager.LogAction(ModuleType, UserAction.PostCompletion, SARCAppealRequest.Id, "Post Completion: " + SESSION_GROUP_NAME + " Digital Signature Generated")

            SetFeedbackMessage("Case " + SARCAppealRequest.CaseId + " successfully signed.  Action applied: " + e.Text)
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

        Private Sub GetData()
            SetLabelText(lblName, SARCAppealRequest.MemberName)
            SetLabelText(lblCompo, Utility.GetCompoString(SARCAppealRequest.MemberCompo))
            SetLabelText(lblUnit, SARCAppealRequest.MemberUnit)

            If (SARCAppealRequest.MemberRank IsNot Nothing) Then
                SetLabelText(lblRank, SARCAppealRequest.MemberRank.Title)
            End If

            If (SARCAppealRequest.InitialWorkflow = AFRCWorkflows.LOD) Then
                SetDateLabel(lbldob, LOD.MemberDob)
            Else
                SetDateLabel(lbldob, SARC.MemberDOB)
            End If

            InitPostProcessingControls()
        End Sub

        Private Sub InitPostProcessingControls()
            If (Not CanInitPostProcessingControls()) Then
                Exit Sub
            End If

            SetPostProcessingControlsInputRestrictions()
            LoadPostProcessingData()
            LoadPostProcessingSignature()
            SetPostProcessingControlsVisibility()
            InitWingSARCDropdownList()
            btnDigitallySign.Enabled = CanDigitallySign()
        End Sub

        Private Function IsContactAddressEmpty() As Boolean
            If (String.IsNullOrEmpty(txtAppealStreet.Text) AndAlso
                String.IsNullOrEmpty(txtAppealCity.Text) AndAlso
                String.IsNullOrEmpty(txtAppealState.Text) AndAlso
                String.IsNullOrEmpty(txtAppealZip.Text) AndAlso
                String.IsNullOrEmpty(txtAppealCountry.Text)) Then
                Return True
            End If

            Return False
        End Function

        Private Function IsEmptyContactInfo() As Boolean
            If (IsContactAddressEmpty() AndAlso String.IsNullOrEmpty(txtEmail.Text) AndAlso String.IsNullOrEmpty(txtHelpExtensionNumber.Text)) Then
                Return True
            End If

            Return False
        End Function

        Private Sub LoadPostProcessingSignature()
            ucPostCompletionSigCheck.VerifySignature(SARCAppealRequest.Id)
        End Sub

        Private Sub SavePostProcessingData()
            Dim input As Double = 0
            lblError.Text = ""

            PostProcessing.HelpExtensionNumber = Server.HtmlEncode(txtHelpExtensionNumber.Text)
            If (String.IsNullOrEmpty(txtHelpExtensionNumber.Text)) Then
                lblError.Text = lblError.Text + "No Wing SARC Phone Number <br />"
            Else
                input += 1
            End If

            PostProcessing.AppealAddress.Street = Server.HtmlEncode(txtAppealStreet.Text)
            If (String.IsNullOrEmpty(txtAppealStreet.Text)) Then
                lblError.Text = lblError.Text + "No Wing SARC Address Street name <br />"
            Else
                input += 0.2
            End If

            PostProcessing.AppealAddress.City = Server.HtmlEncode(txtAppealCity.Text)
            If (String.IsNullOrEmpty(txtAppealCity.Text)) Then
                lblError.Text = lblError.Text + "No Wing SARC Address City name <br />"
            Else
                input += 0.2
            End If

            PostProcessing.AppealAddress.State = Server.HtmlEncode(txtAppealState.Text)
            If (String.IsNullOrEmpty(txtAppealState.Text)) Then
                lblError.Text = lblError.Text + "No Wing SARC Address State name <br />"
            Else
                input += 0.2
            End If

            PostProcessing.AppealAddress.Zip = Server.HtmlEncode(txtAppealZip.Text)
            If (String.IsNullOrEmpty(txtAppealZip.Text)) Then
                lblError.Text = lblError.Text + "No Wing SARC Address Zip number <br />"
            Else
                input += 0.2
            End If

            PostProcessing.AppealAddress.Country = Server.HtmlEncode(txtAppealCountry.Text)
            If (String.IsNullOrEmpty(txtAppealCountry.Text)) Then
                lblError.Text = lblError.Text + "No Wing SARC Address Country name <br />"
            Else
                input += 0.2
            End If

            PostProcessing.email = Server.HtmlEncode(txtEmail.Text)
            If (String.IsNullOrEmpty(txtEmail.Text)) Then
                lblError.Text = lblError.Text + "No Wing SARC Email <br />"
            Else
                input += 1
            End If

            If (Not String.IsNullOrEmpty(NotificationDate.Text)) Then
                PostProcessing.NotificationDate = Server.HtmlEncode(DateTime.Parse(NotificationDate.Text.Trim))
            End If

            If (input > 0) Then
                trError.Visible = False

                If (SARCAppealRequest.MemberNotified = False) Then
                    SARCAppealRequest.MemberNotified = MemberInformedCheckBox.Checked()
                End If
            Else
                trError.Visible = True
            End If

            AppealPostDao.InsertOrUpdate(PostProcessing)
            SARCAppealRequest.UpdateIsPostProcessingComplete(DaoFactory)
            APDao.SaveOrUpdate(SARCAppealRequest)
        End Sub

#End Region

    End Class

End Namespace