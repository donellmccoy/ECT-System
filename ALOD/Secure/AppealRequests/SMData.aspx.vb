Imports ALOD.Core.Domain.Common
Imports ALOD.Core.Domain.DBSign
Imports ALOD.Core.Domain.Modules.Appeals
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
Imports ALODWebUtility.Common.WebControlSetters
Imports ALODWebUtility.Worklfow

Namespace Web.AP

    Partial Class Secure_ap_SMData
        Inherits System.Web.UI.Page

        Private _adao As ILODAppealDAO
        Private _appeal As LODAppeal
        Private _dao As ILineOfDutyDao
        Private _daoFactory As IDaoFactory
        Private _lod As LineOfDuty
        Private _postdao As IAppealPostProcessingDAO
        Private _postprocessing As AppealPostProcessing
        Private _signatureMetaDataDao As ISignatueMetaDateDao

#Region "Properties"

        Protected ReadOnly Property APDao() As ILODAppealDAO
            Get
                If (_adao Is Nothing) Then
                    _adao = DaoFactory.GetLODAppealDao()
                End If

                Return _adao
            End Get
        End Property

        Protected ReadOnly Property AppealPostDao() As IAppealPostProcessingDAO
            Get
                If (_postdao Is Nothing) Then
                    _postdao = DaoFactory.GetAppealPostProcessingDao()
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
                    _lod = Dao.GetById(LODAppealRequest.InitialLodId, False)

                End If
                Return _lod
            End Get
        End Property

        Protected ReadOnly Property LODAppealRequest() As LODAppeal
            Get
                If (_appeal Is Nothing) Then
                    _appeal = APDao.GetById(RequestId, False)

                End If
                Return _appeal
            End Get
        End Property

        Protected ReadOnly Property ModuleType() As ModuleType
            Get
                Return ModuleType.AppealRequest
            End Get
        End Property

        Protected ReadOnly Property Navigator() As TabNavigator
            Get
                Return Me.Master.Navigator
            End Get
        End Property

        Protected ReadOnly Property PostProcessing() As AppealPostProcessing
            Get
                If (_postprocessing Is Nothing) Then
                    _postprocessing = AppealPostDao.GetById(LODAppealRequest.Id)
                End If

                If (_postprocessing Is Nothing) Then
                    _postprocessing = New AppealPostProcessing(LODAppealRequest.Id)
                    _postprocessing.InitialLodId = LODAppealRequest.InitialLodId
                End If

                Return _postprocessing
            End Get
        End Property

        Protected ReadOnly Property RequestId() As Integer
            Get
                Return Integer.Parse(Request.QueryString("requestId"))
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

            LODAppealRequest.AddSignature(DaoFactory, groupId, currentUser.SignatureTitle, currentUser)
        End Sub

        Protected Sub btnSavePostProcessingData_Click(sender As Object, e As EventArgs) Handles btnSavePostProcessingData.Click
            SaveData()
        End Sub

        Protected Function CanDigitallySign() As Boolean
            If (LODAppealRequest.IsPostProcessingComplete) Then
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
            If (Not LODAppealRequest.WorkflowStatus.StatusCodeType.IsFinal OrElse LODAppealRequest.WorkflowStatus.StatusCodeType.IsCancel) Then
                Return False
            End If

            If (Not LODAppealRequest.DoesFindingExist(PersonnelTypes.APPELLATE_AUTH)) Then
                Return False
            End If

            Return True
        End Function

        Protected Function CanSavePostProcessingData() As Boolean
            If (Not LODAppealRequest.WorkflowStatus.StatusCodeType.IsFinal AndAlso Not LODAppealRequest.WorkflowStatus.StatusCodeType.IsCancel) Then
                Return False
            End If

            If (Not UserHasPermission("APCompletion")) Then
                Return False
            End If

            If (LODAppealRequest.IsPostProcessingComplete) Then
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
            SaveData()
            ucPostProcessingSigBlock.StartSignature(LODAppealRequest.Id, LODAppealRequest.Workflow, 0, "Sign Post Processing Information", LODAppealRequest.Status, LODAppealRequest.Status, 0, DBSignTemplateId.Form348AppealPostProcessing, String.Empty)
        End Sub

        Protected Sub GetData()
            SetLabelText(lblName, LODAppealRequest.MemberName)
            If (LODAppealRequest.MemberRank IsNot Nothing) Then
                SetLabelText(lblRank, LODAppealRequest.MemberRank.Title)
            End If
            SetLabelText(lblCompo, Utility.GetCompoString(LODAppealRequest.MemberCompo))
            SetLabelText(lblUnit, LODAppealRequest.MemberUnit)
            SetDateLabel(lbldob, LOD.MemberDob)

            InitPostProcessingControls()
        End Sub

        Protected Sub InitPostProcessingControls()
            If (Not CanInitPostProcessingControls()) Then
                Exit Sub
            End If

            SetPostProcessingControlsInputRestrictions()
            LoadPostProcessingData()
            LoadPostProcessingSignature()
            SetPostProcessingControlsVisibility()
            InitLODPMDropdownList()
            btnDigitallySign.Enabled = CanDigitallySign()
        End Sub

        Protected Sub InitSigCheckControl()
            If (Not LODAppealRequest.IsNonDBSignCase) Then
                SigCheck.VerifySignature(RequestId)
                SigCheck.Visible = True
            Else
                SigCheck.Visible = False
            End If
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

            If (LODAppealRequest.MemberNotified) Then
                SetCheckBox(MemberInformedCheckBox, LODAppealRequest.MemberNotified)
                MemberInformedCheckBox.Enabled = False
                SetLabelText(MemberNotifiedLabel, "Yes")
            Else
                SetLabelText(MemberNotifiedLabel, "No")
            End If
        End Sub

        Protected Sub MemberInformedCheckBox_CheckedChanged(sender As Object, e As System.EventArgs) Handles MemberInformedCheckBox.CheckedChanged
            If (Not UserHasPermission("APCompletion")) Then
                Exit Sub
            End If

            If (MemberInformedCheckBox.Checked) Then
                LogManager.LogAction(ModuleType, UserAction.PostCompletion, LODAppealRequest.Id, "Post Completion: " + SESSION_GROUP_NAME + " Informed Member")
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
                CaseHistory.Initialize(Me, LODAppealRequest.MemberSSN, LODAppealRequest.CaseId, False)

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

        Protected Sub SavePostProcessingData()
            Dim input As Double = 0
            lblError.Text = ""

            PostProcessing.HelpExtensionNumber = Server.HtmlEncode(txtHelpExtensionNumber.Text)
            If (String.IsNullOrEmpty(txtHelpExtensionNumber.Text)) Then
                lblError.Text = lblError.Text + "No LOD PM Phone Number <br />"
            Else
                input += 1
            End If

            PostProcessing.AppealAddress.Street = Server.HtmlEncode(txtAppealStreet.Text)
            If (String.IsNullOrEmpty(txtAppealStreet.Text)) Then
                lblError.Text = lblError.Text + "No LOD PM Address Street name <br />"
            Else
                input += 0.2
            End If

            PostProcessing.AppealAddress.City = Server.HtmlEncode(txtAppealCity.Text)
            If (String.IsNullOrEmpty(txtAppealCity.Text)) Then
                lblError.Text = lblError.Text + "No LOD PM Address City name <br />"
            Else
                input += 0.2
            End If

            PostProcessing.AppealAddress.State = Server.HtmlEncode(txtAppealState.Text)
            If (String.IsNullOrEmpty(txtAppealState.Text)) Then
                lblError.Text = lblError.Text + "No LOD PM Address State name <br />"
            Else
                input += 0.2
            End If

            PostProcessing.AppealAddress.Zip = Server.HtmlEncode(txtAppealZip.Text)
            If (String.IsNullOrEmpty(txtAppealZip.Text)) Then
                lblError.Text = lblError.Text + "No LOD PM Address Zip number <br />"
            Else
                input += 0.2
            End If

            PostProcessing.AppealAddress.Country = Server.HtmlEncode(txtAppealCountry.Text)
            If (String.IsNullOrEmpty(txtAppealCountry.Text)) Then
                lblError.Text = lblError.Text + "No LOD PM Address Country name <br />"
            Else
                input += 0.2
            End If

            PostProcessing.email = Server.HtmlEncode(txtEmail.Text)
            If (String.IsNullOrEmpty(txtEmail.Text)) Then
                lblError.Text = lblError.Text + "No LOD PM Email <br />"
            Else
                input += 1
            End If

            If (Not String.IsNullOrEmpty(NotificationDate.Text)) Then
                PostProcessing.NotificationDate = Server.HtmlEncode(DateTime.Parse(NotificationDate.Text.Trim))
            End If

            If (input > 0) Then
                trError.Visible = False

                If (LODAppealRequest.MemberNotified = False) Then
                    LODAppealRequest.MemberNotified = MemberInformedCheckBox.Checked()
                End If
            Else
                trError.Visible = True
            End If

            AppealPostDao.InsertOrUpdate(PostProcessing)
            LOD.UpdateIsPostProcessingComplete(DaoFactory)
            APDao.SaveOrUpdate(LODAppealRequest)
        End Sub

        Protected Sub SetPostProcessingControlsInputRestrictions()
            SetInputFormatRestriction(Page, txtHelpExtensionNumber, FormatRestriction.Numeric)
            SetInputFormatRestriction(Page, txtAppealStreet, FormatRestriction.AlphaNumeric, PERMITTED_SPECIAL_CHAR_INPUT)
            SetInputFormatRestriction(Page, txtAppealCity, FormatRestriction.AlphaNumeric, PERMITTED_SPECIAL_CHAR_INPUT)
            SetInputFormatRestriction(Page, txtAppealState, FormatRestriction.AlphaNumeric, PERMITTED_SPECIAL_CHAR_INPUT)
            SetInputFormatRestriction(Page, txtAppealZip, FormatRestriction.Numeric, "-")
            SetInputFormatRestriction(Page, txtAppealCountry, FormatRestriction.AlphaNumeric, PERMITTED_SPECIAL_CHAR_INPUT)
        End Sub

        Protected Sub SetPostProcessingControlsVisibility()
            pnlPostCompletion.Visible = True

            If (UserHasPermission("APCompletion") AndAlso Not LODAppealRequest.IsPostProcessingComplete) Then
                trLODPM.Visible = True
                txtAppealStreet.Visible = True
                txtAppealCity.Visible = True
                txtAppealState.Visible = True
                txtAppealZip.Visible = True
                txtAppealCountry.Visible = True
                MemberInformedCheckBox.Visible = True
                NotificationDate.Visible = True
                trSave.Visible = True
                txtEmail.Visible = True
                txtHelpExtensionNumber.Visible = True
                trDigitallySign.Visible = True
                ucPostProcessingSigBlock.Visible = True
            Else
                lblAppealAddress.Visible = True
                lblEmail.Visible = True
                lblHelpExtensionNumber.Visible = True
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
            LODAppealRequest.UpdateIsPostProcessingComplete(DaoFactory)
            APDao.SaveOrUpdate(LODAppealRequest)

            LogManager.LogAction(ModuleType, UserAction.PostCompletion, LODAppealRequest.Id, "Post Completion: " + SESSION_GROUP_NAME + " Digital Signature Generated")

            SetFeedbackMessage("Case " + LODAppealRequest.CaseId + " successfully signed.  Action applied: " + e.Text)
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

        Private Sub InitLODPMDropdownList()
            Dim availableLODPMs = DaoFactory.GetLookupDao().Get_LODPMs(LOD.MemberUnitId)

            If (availableLODPMs.Count = 0) Then
                availableLODPMs.Add(New LookUpItem() With {.Name = "No LOD PMs available", .Value = 0})
            End If

            ddlLODPM.DataSource = availableLODPMs
            ddlLODPM.DataTextField = "Name"
            ddlLODPM.DataValueField = "Value"
            ddlLODPM.DataBind()

            InsertDropDownListZeroValue(ddlLODPM, "--- Select One ---")
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
            ucPostCompletionSigCheck.VerifySignature(LODAppealRequest.Id)
        End Sub

#End Region

    End Class

End Namespace