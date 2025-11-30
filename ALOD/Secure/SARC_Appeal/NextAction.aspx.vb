Imports ALOD.Core.Domain.Common
Imports ALOD.Core.Domain.Common.CustomServerControls
Imports ALOD.Core.Domain.DBSign
Imports ALOD.Core.Domain.Documents
Imports ALOD.Core.Domain.Lookup
Imports ALOD.Core.Domain.Modules.Lod
Imports ALOD.Core.Domain.Modules.SARC
Imports ALOD.Core.Domain.Users
Imports ALOD.Core.Domain.Workflow
Imports ALOD.Core.Interfaces
Imports ALOD.Core.Interfaces.DAOInterfaces
Imports ALOD.Core.Utils
Imports ALOD.Core.Utils.NextActionHelpers
Imports ALOD.Data
Imports ALOD.Data.Services
Imports ALOD.Logging
Imports ALOD.Web.UserControls
Imports ALODWebUtility.Common
Imports ALODWebUtility.Worklfow

Namespace Web.APSA

    Partial Class Secure_apsa_NextAction
        Inherits System.Web.UI.Page

        Private Const VIEWSTATE_VALIDATIONS As String = "VALIDATION_ITEMS"
        Private _adao As ISARCAppealDAO
        Private _appeal As SARCAppeal
        Private _daoFactory As IDaoFactory
        Private _docCatViewDao As IDocCategoryViewDao
        Private _docDao As IDocumentDao
        Private _lookupDao As ILookupDao
        Private _memoDao As IMemoDao
        Private _memoDao2 As IMemoDao2
        Private _memoSignature As IMemoSignatureDao
        Private _scAccess As Dictionary(Of String, ALOD.Core.Domain.Workflow.PageAccessType)
        Private _usergroupDao As IUserGroupDao
        Private _workstatusDao As IWorkStatusDao

        Dim boardStages As Short() = New Short() {
            CShort(SARCAppealStatusCode.SARCAdminReview),
            CShort(SARCAppealStatusCode.BoardMedicalReview),
            CShort(SARCAppealStatusCode.SeniorMedicalReview),
            CShort(SARCAppealStatusCode.BoardLegalReview),
            CShort(SARCAppealStatusCode.BoardAdminReview),
            CShort(SARCAppealStatusCode.AppellateAuthorityReview)}

        Private CANCELLATION_REASON As String = "Case Cancelled By "
        Private errors As New List(Of ValidationItem)
        Private separatorTemplate As System.Web.UI.CompiledTemplateBuilder
        Dim unitStages As Short() = New Short() {CShort(SARCAppealStatusCode.Initiate)}

#Region "Properties"

        ReadOnly Property APDao() As ISARCAppealDAO
            Get
                If (_adao Is Nothing) Then
                    _adao = DaoFactory.GetSARCAppealDao()
                End If

                Return _adao
            End Get
        End Property

        ReadOnly Property appealRequest() As SARCAppeal
            Get
                If (_appeal Is Nothing) Then
                    _appeal = APDao.GetById(RequestId)
                End If

                Return _appeal
            End Get
        End Property

        ReadOnly Property DocCatViewDao() As IDocCategoryViewDao
            Get
                If (_docCatViewDao Is Nothing) Then
                    _docCatViewDao = DaoFactory.GetDocCategoryViewDao()
                End If

                Return _docCatViewDao
            End Get
        End Property

        ReadOnly Property DocumentDao() As IDocumentDao
            Get
                If (_docDao Is Nothing) Then
                    _docDao = New SRXDocumentStore(SESSION_USERNAME)
                End If

                Return _docDao
            End Get
        End Property

        ReadOnly Property LookupDao() As ILookupDao
            Get
                If (_lookupDao Is Nothing) Then
                    _lookupDao = DaoFactory.GetLookupDao()
                End If

                Return _lookupDao
            End Get
        End Property

        ReadOnly Property MemoDao() As IMemoDao
            Get
                If (_memoDao Is Nothing) Then
                    _memoDao = DaoFactory.GetMemoDao()
                End If

                Return _memoDao
            End Get
        End Property

        ReadOnly Property MemoDao2() As IMemoDao2
            Get
                If (_memoDao2 Is Nothing) Then
                    _memoDao2 = DaoFactory.GetMemoDao2()
                End If

                Return _memoDao2
            End Get
        End Property

        ReadOnly Property MemoSignatureDao() As IMemoSignatureDao
            Get
                If (_memoSignature Is Nothing) Then
                    _memoSignature = New NHibernateDaoFactory().GetMemoSignatureDao()
                End If

                Return _memoSignature
            End Get
        End Property

        ReadOnly Property RequestId() As Integer
            Get
                Return CInt(Request.QueryString("requestId"))
            End Get
        End Property

        ReadOnly Property UserGroupDao() As IUserGroupDao
            Get
                If (_usergroupDao Is Nothing) Then
                    _usergroupDao = DaoFactory.GetUserGroupDao()
                End If

                Return _usergroupDao
            End Get
        End Property

        ReadOnly Property WorkstatusDao() As IWorkStatusDao
            Get
                If (_workstatusDao Is Nothing) Then
                    _workstatusDao = DaoFactory.GetWorkStatusDao()
                End If

                Return _workstatusDao
            End Get
        End Property

        Public ReadOnly Property Navigator() As TabNavigator
            Get
                Return MasterPage.Navigator
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

        Protected ReadOnly Property DaoFactory As IDaoFactory
            Get
                If (_daoFactory Is Nothing) Then
                    _daoFactory = New NHibernateDaoFactory()
                End If

                Return _daoFactory
            End Get
        End Property

        Protected ReadOnly Property MasterPage() As SARCAppealMaster
            Get
                Dim master As SARCAppealMaster = CType(Page.Master, SARCAppealMaster)
                Return master
            End Get
        End Property

        Protected ReadOnly Property ModuleType() As ModuleType
            Get
                Return ModuleType.SARCAppeal
            End Get
        End Property

        Protected ReadOnly Property SectionList() As Dictionary(Of String, ALOD.Core.Domain.Workflow.PageAccessType)
            Get
                If (_scAccess Is Nothing) Then
                    _scAccess = appealRequest.ReadSectionList(SESSION_GROUP_ID)
                End If
                Return _scAccess
            End Get
        End Property

        Public Sub CommitChanges()
            APDao.CommitChanges()
            APDao.Evict(appealRequest)
            _appeal = Nothing
        End Sub

#End Region

#Region "PageProperty"

        Public Property RLBEnabled() As Boolean
            Get
                Return ucRLB.Enabled
            End Get
            Set(ByVal value As Boolean)
                ucRLB.Enabled = value
            End Set
        End Property

        Public Property RLBVisible() As Boolean
            Get
                Return divRLB.Visible
            End Get
            Set(ByVal value As Boolean)
                divRLB.Visible = value

            End Set
        End Property

        Protected Sub Page_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
            AddHandler Me.Master.TabClick, AddressOf TabButtonClicked
        End Sub

        Protected Sub Page_PreRender(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.PreRender
            'we have two sources of errors.
            'those generated by the LOD itself and any errors that occured on this page
            'so we combine the two here for display

            For Each item As ValidationItem In ViewState(VIEWSTATE_VALIDATIONS)
                errors.Add(item)
            Next

            results.DataSource = errors

        End Sub

        Private Sub TabButtonClicked(ByRef sender As Object, ByRef e As TabNavigationEventArgs)

            If (e.ButtonType = NavigatorButtonType.Save OrElse e.ButtonType = NavigatorButtonType.NavigatedAway) Then

                appealRequest.ModifiedBy = SESSION_USER_ID
                appealRequest.ModifiedDate = DateTime.Now
            End If
            If (e.ButtonType = NavigatorButtonType.Save) Then
                Response.Redirect(Request.RawUrl)
            End If
        End Sub

#End Region

        Public Sub SaveCancelData()

            If (Not UserCanEdit) Then
                Exit Sub
            End If

            'Cancellation Reasons
            appealRequest.Cancel_Explanation = Server.HtmlEncode(CommentsTextBox.Text.Trim)
            If rblReason.SelectedValue <> "" Then
                appealRequest.Cancel_Reason = CShort(rblReason.SelectedValue)
            End If
            appealRequest.Cancel_Date = DateTime.Now

        End Sub

        Protected Sub CheckRoutingSelection()
            Dim workOption = GetWorkOption()

            commentsTitle.CssClass = String.Empty

            If (workOption.DisplayText Like "*Cancel*") Then
                PopulateCancelrbl()

                reasonTitle.Text = "* Reason for Cancellation:"
                commentsTitle.Text = "* Cancel Comments:"
                commentsTitle.CssClass = "labelRequired"

                CommentsTextBox.Text = ""
                CommentsTextBox.MaxLength = 250
                SetMaxLength(CommentsTextBox)

                reasonRow.Visible = True
                reasonLetter.Text = "B"
                commentsRow.Visible = True
                commentLetter.Text = "C"

                'Curt Lucas
                '7-02-2019 - CR-016 - Added OrElse for RFA
            ElseIf (workOption.DisplayText Like "RWOA*") OrElse (workOption.DisplayText Like "RFA*") Then
                PopulateRWOArbl()

                reasonTitle.Text = "* Reason for RFA:"
                commentsTitle.Text = "* RFA Comments:"
                CommentsTextBox.MaxLength = 400
                SetMaxLength(CommentsTextBox)

                CommentsTextBox.Text = ""

                reasonRow.Visible = True
                reasonLetter.Text = "B"
                commentsRow.Visible = True
                commentLetter.Text = "C"
            Else
                reasonRow.Visible = False
                commentsRow.Visible = False
            End If
        End Sub

        Protected Function GetSentToInfo(ByVal statusCodeOut As Integer) As Tuple(Of Integer, String)

            Dim sentTo As String = ""
            Dim sentToId As Integer = 0

            Select Case statusCodeOut
                Case SARCAppealStatusCode.Initiate
                    sentTo = "Wing SARC"
                    sentToId = UserGroups.WingSarc
                Case SARCAppealStatusCode.SARCAdminReview
                    sentTo = "SARC Admin"
                    sentToId = UserGroups.SARCAdmin
                Case SARCAppealStatusCode.BoardMedicalReview
                    sentTo = "Board Medical"
                    sentToId = UserGroups.BoardMedical
                Case SARCAppealStatusCode.SeniorMedicalReview
                    sentTo = "Senior Medical Reviewer"
                    sentToId = UserGroups.SeniorMedicalReviewer
                Case SARCAppealStatusCode.BoardLegalReview
                    sentTo = "Board Legal"
                    sentToId = UserGroups.BoardLegal
                Case SARCAppealStatusCode.BoardAdminReview
                    sentTo = "Board Admin"
                    sentToId = UserGroups.BoardAdministrator
                Case SARCAppealStatusCode.AppellateAuthorityReview
                    sentTo = "Appellate Authority"
                    sentToId = UserGroups.AppellateAuthority
                Case Else
                    sentTo = ""
            End Select

            Dim sent As Tuple(Of Integer, String) = New Tuple(Of Integer, String)(sentToId, sentTo)
            Return sent
        End Function

        Protected Function GetWorkOption() As WorkflowStatusOption
            Dim radio As ValueRadioButton
            Dim workOption As WorkflowStatusOption = Nothing

            'get the selected option
            For Each item As RepeaterItem In rbtOption.Items
                radio = CType(item.FindControl("rblOptions"), ValueRadioButton)

                If (radio.Checked) Then
                    workOption = StringToOption(radio.Value)
                    workOption.DisplayText = radio.Text
                    Exit For
                End If

            Next

            Return workOption
        End Function

        Protected Function MemberHasFindings() As Boolean
            Dim pType As Integer = CInt(GetPersonnelTypeFromGroup(SESSION_GROUP_ID, False))
            Dim finding As SARCAppealFindings = appealRequest.FindFindingByPersonnelType(pType)

            If (finding Is Nothing) Then
                Return False
            End If

            Return True
        End Function

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

            Dim eventTarget As String

            If (Not Page.IsPostBack) Then

                UserCanEdit = GetAccess(Navigator.PageAccess, True)
                InitControls()
                InitData()
                ShowRLB()
                InitNextActionOptions()

                LogManager.LogAction(ModuleType, UserAction.ViewPage, RequestId, "Viewed Page: Next Action")

            End If

            If ((Me.Request("__EVENTTARGET") Is Nothing)) Then
                eventTarget = String.Empty
            Else
                eventTarget = Me.Request("__EVENTTARGET")
            End If

            If (eventTarget = "CheckRoutingSelection") Then
                CheckRoutingSelection()
            End If

            If Not (UserCanEdit) Then
                SignButton.Enabled = False
            End If
        End Sub

        Protected Sub PopulateCancelrbl()
            rblReason.DataSource = LookupService.GetWorkflowCancelReasons(appealRequest.Workflow, False)
            rblReason.DataTextField = "Description"
            rblReason.DataValueField = "Id"
            rblReason.DataBind()
        End Sub

        Protected Sub PopulateRWOArbl()
            NextActionHelpers.PopulateRadioButtonListWithRwoaOptions(rblReason, appealRequest.Workflow, LookupDao)
        End Sub

        Protected Sub rbtOption_ItemDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.RepeaterItemEventArgs) Handles rbtOption.ItemDataBound

            If (e.Item.ItemType = ListItemType.Item) OrElse (e.Item.ItemType = ListItemType.AlternatingItem) Then

                Dim rb As ValueRadioButton = CType(e.Item.FindControl("rblOptions"), ValueRadioButton)
                Dim options As ALOD.Core.Domain.Workflow.WorkflowStatusOption

                options = e.Item.DataItem
                rb.Text = Server.HtmlEncode(options.DisplayText)
                rb.CssClass = "option-routing"
                rb.Value = OptionToString(options)
                rb.Visible = options.OptionVisible
                rb.Enabled = options.OptionValid
                Dim script As String = "SetUniqueRadioButton('rbtOption.*option', this)"
                rb.Attributes.Add("onclick", script)

                If (separatorTemplate Is Nothing) Then
                    separatorTemplate = rbtOption.SeparatorTemplate
                End If

                If (Not options.OptionVisible) Then
                    rbtOption.SeparatorTemplate = Nothing
                Else
                    rbtOption.SeparatorTemplate = separatorTemplate
                End If

            End If
        End Sub

        Protected Sub SignatureCompleted(sender As Object, e As SignCompletedEventArgs) Handles SigBlock.SignCompleted

            If (e.SignaturePassed) Then

                'we have a good signature.  take any further actions that are required
                ChangeStatus(e.OptionId, e.StatusOut, e.Text, e.Comments)
                SetFeedbackMessage("Case " + appealRequest.CaseId + " successfully signed.  Action applied: " + e.Text)
                Response.Redirect(Resources._Global.StartPage, True)

            End If

        End Sub

        Protected Sub SignButton_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles SignButton.Click

            Dim workOption = GetWorkOption()

            'make sure we have an option selected
            If (workOption Is Nothing) Then
                OptionPanel.CssClass = "fieldRequired"
                errors.Add(New ValidationItem("Routing", "Routing", "A Routing option must be selected"))
                Exit Sub
            Else
                OptionPanel.CssClass = ""
            End If

            'we have an option selected

            If (workOption.DisplayText Like "*Cancel*") Then
                'the user is cancelling, do we have a cancel reason?
                If (Not ValidateReasonControls("Cancellation")) Then
                    Exit Sub
                End If

                'we have a cancel reason, so add save it
                SaveCancelData()

            End If

            'Curt Lucas
            '7-02-2019 - CR-016 - Added OrElse for RFA
            If (workOption.DisplayText Like "RWOA*") OrElse (workOption.DisplayText Like "RFA*") Then
                If (Not ValidateReasonControls("RWOA")) Then
                    Exit Sub
                End If
            End If

            ReasonPanel.CssClass = ""

            If Not IsNothing(appealRequest.ReturnToGroup) Then  'Clear the Return Group info when the recipient Replies/Forwards
                If SESSION_GROUP_ID = appealRequest.ReturnToGroup Then
                    appealRequest.ReturnToGroup = Nothing
                    appealRequest.ReturnByGroup = Nothing
                End If
            End If

            Dim template As Integer = workOption.Template

            If (template = DBSignTemplateId.Form348APSARCFindings AndAlso Not MemberHasFindings()) Then
                template = DBSignTemplateId.SignOnly
            End If

            Dim secId As Integer = CInt(GetPersonnelTypeFromGroup(SESSION_GROUP_ID, False))

            Dim selectedText As String
            If (rblReason.SelectedValue.Length > 0) Then
                selectedText = rblReason.SelectedItem.Text
            Else
                selectedText = String.Empty
            End If

            'Adding RWOA/Cancel Reason to the Log
            Dim LogActionDisplayText = LogText(workOption.DisplayText, selectedText, Server.HtmlEncode(CommentsTextBox.Text.Trim()))

            CommitChanges()
            SigBlock.StartSignature(RequestId, appealRequest.Workflow, secId, LogActionDisplayText, appealRequest.Status, workOption.wsStatusOut, workOption.Id, template, String.Empty)
        End Sub

        Protected Function ValidateReasonControls(ByVal typeOfReturn As String) As Boolean
            Dim areValid As Boolean = True

            If (rblReason.SelectedValue.Length = 0) Then
                ReasonPanel.CssClass = "fieldRequired"
                errors.Add(New ValidationItem("Routing", "Reason For Return", "A Reason for " & typeOfReturn & " must be selected"))
                areValid = False
            Else
                ReasonPanel.CssClass = String.Empty
            End If

            If (String.IsNullOrEmpty(CommentsTextBox.Text)) Then
                CommentsTextBox.CssClass = "fieldRequired"
                errors.Add(New ValidationItem("Routing", "Reason For Return", "An explanation for " & typeOfReturn & " must be entered"))
                areValid = False
            Else
                CommentsTextBox.CssClass = String.Empty
            End If

            Return areValid
        End Function

        Private Sub InitCancelComments(ByVal reason As Integer, ByVal explanation As String)
            Dim title As String = "Case Cancelled: "

            PopulateCancelrbl()
            rblReason.SelectedValue = reason
            CommentLabel.Text = title & rblReason.SelectedItem.Text & "<br /><br />"

            CommentLabel.Text = CommentLabel.Text & NullStringToEmptyString(explanation).Replace(Environment.NewLine, "<br />")
        End Sub

        Private Sub InitControls()

            SetInputFormatRestriction(Page, CommentsTextBox, FormatRestriction.AlphaNumeric, PERMITTED_SPECIAL_CHAR_INPUT)

        End Sub

        Private Sub InitData()

            If (RequestId = 0) Then
                Exit Sub
            End If

            'show the comments on the cancel/return (if there are any)
            CommentPanel.Visible = False
            CommentLabel.Text = ""

            If (Not IsNothing(appealRequest.Cancel_Reason)) Then  'Check if Cancelled - show Cancel Reason
                If appealRequest.Cancel_Reason > 0 Then
                    InitCancelComments(appealRequest.Cancel_Reason, appealRequest.Cancel_Explanation)

                    CommentPanel.Visible = True
                End If
            End If

        End Sub

        Private Sub InitNextActionOptions()

            'as part of this method call the validation errors are generated
            'so we will store them from here for later display
            Dim options = From o In appealRequest.GetCurrentOptions(WorkFlowService.GetLastStatus(appealRequest.Id, ModuleType.SARCAppeal), DaoFactory)
                          Select o
                          Order By o.SortOrder

            rbtOption.DataSource = options
            rbtOption.DataBind()

            ViewState(VIEWSTATE_VALIDATIONS) = appealRequest.Validations

        End Sub

        Private Sub ShowRLB()

            Dim access As ALOD.Core.Domain.Workflow.PageAccessType = SectionList(APSASectionNames.APSA_RLB.ToString())

            If access <> ALOD.Core.Domain.Workflow.PageAccessType.None Then
                Dim ws As IList(Of WorkStatusTracking) = WorkFlowService.GetWorkStatusTracking(appealRequest.Id, ModuleType.SARCAppeal)

                If (ws IsNot Nothing AndAlso ws.Count > 1) Then
                    If ((IsReturnedToBoard(appealRequest.CurrentStatusCode) OrElse IsReturnedToUnit(ws(0).WorkflowStatus.StatusCodeType.Id, appealRequest.CurrentStatusCode))) Then
                        'get the last status from workflowtransaction class
                        RLBVisible = True
                        RLBTitle.Text = "Returned Without Action"

                        appealRequest.RWOA_Reply = String.Empty
                        Dim rwoaDao As IRwoaDao = DaoFactory.GetRwoaDao()
                        ucRLB.Initialize(rwoaDao.GetRecentRWOA(appealRequest.Workflow, appealRequest.Id))
                        If access <> ALOD.Core.Domain.Workflow.PageAccessType.ReadWrite Then
                            RLBEnabled = False
                        End If
                    End If
                End If
            End If
        End Sub

#Region "ChangeStatusAndActions"

        Public Sub AddEmails(ByRef emails As MailManager, emailService As EmailService, action As WorkflowOptionAction, statusOut As Integer)

            Dim toList As StringCollection
            Dim statusCodeOut As Integer = WorkstatusDao.GetById(statusOut).StatusCodeType.Id

            If (statusCodeOut = SARCAppealStatusCode.Canceled OrElse statusCodeOut = SARCAppealStatusCode.Approved OrElse statusCodeOut = SARCAppealStatusCode.Denied) Then
                'add it to the email manager
                toList = emailService.GetDistributionListByGroup(appealRequest.Id, action.Target, "SARCAppeal")
            Else
                toList = emailService.GetDistributionListByWorkflow(appealRequest.Id, statusOut, appealRequest.Workflow, action.Target)
            End If

            emails.AddTemplate(action.Data, "", toList)

        End Sub

        Public Sub ApplyActions(ByVal optionId As Integer, ByVal statusIn As Integer, ByVal statusOut As Integer)
            Dim optn As ALOD.Core.Domain.Workflow.WorkflowStatusOption = WorkFlowService.GetOptionById(optionId)

            If (optn Is Nothing) Then
                Exit Sub
            End If

            Dim mailService As EmailService = New EmailService()
            Dim emails As New ALOD.Core.Domain.Common.MailManager(mailService)
            For Each actn As WorkflowOptionAction In optn.ActionList

                Select Case actn.ActionType.Id

                    Case WorkflowActionType.SaveRWOA
                        InsertRWOA(statusIn, statusOut)
                    Case WorkflowActionType.ReturnRWOA
                        ReturnRWOA(statusIn)
                    Case WorkflowActionType.AddSignature
                        AddSignature(actn.Target)
                    Case WorkflowActionType.RemoveSignature
                        appealRequest.RemoveSignature(DaoFactory, statusOut)
                    Case WorkflowActionType.SendEmail
                        AddEmails(emails, mailService, actn, statusOut)
                End Select

            Next

            emails.SetField("CASE_NUMBER", appealRequest.CaseId)
            emails.SetField("APP_LINK", GetHostName())
            emails.SendAll()

        End Sub

        Public Sub ChangeStatus(optionId As Integer, newStatus As Integer, text As String, comments As String)
            Dim statusIn As Integer = appealRequest.Status
            Dim newStatusCode As Integer = WorkstatusDao.GetById(newStatus).StatusCodeType.Id

            If (newStatus <> appealRequest.Status) Then
                LogManager.LogAction(ModuleType, UserAction.Signed, appealRequest.Id, Server.HtmlEncode(CommentsTextBox.Text.Trim), statusIn, newStatus)
                LogManager.LogAction(ModuleType, UserAction.StatusChanged, appealRequest.Id, text, statusIn, newStatus)

                ApplyActions(optionId, statusIn, newStatus)

                appealRequest.Status = newStatus
                appealRequest.ModifiedBy = SESSION_USER_ID
                appealRequest.ModifiedDate = DateTime.Now()

                APDao.SaveOrUpdate(appealRequest)
                CommitChanges()

                If (newStatusCode = SARCAppealStatusCode.Approved OrElse newStatusCode = SARCAppealStatusCode.Denied) Then
                    If (appealRequest.InitialWorkflow = AFRCWorkflows.SARCRestricted) Then
                        GenerateMemoSARC()
                    Else
                        GenerateMemoLOD()
                    End If

                    appealRequest.UpdateIsPostProcessingComplete(DaoFactory)
                    APDao.SaveOrUpdate(appealRequest)
                    CommitChanges()
                End If

            End If

            Dim reminderDao = New ReminderEmailsDao()
            reminderDao.ReminderEmailUpdateStatusChange(statusIn, newStatus, appealRequest.CaseId, "APSA")
        End Sub

        '<summary>
        '<mod tfsid="5125" date="02/22/2011">
        'Med off was not able to cancel lod if this was an override case or in case it was not returned by board and the form348 records rwoa reason and date were not set
        '</mod>
        '</summary>
        Public Sub InsertRWOA(ByVal statusIn As Integer, ByVal statusOut As Integer)

            Dim statusCodeIn As Integer = WorkstatusDao.GetById(statusIn).StatusCodeType.Id
            Dim statusCodeOut As Integer = WorkstatusDao.GetById(statusOut).StatusCodeType.Id

            Dim rwoaDao As IRwoaDao = DaoFactory.GetRwoaDao()

            If (IsReturnedToUnit(statusCodeIn, statusCodeOut)) Then

                If GetSentToInfo(statusCodeOut).Item1 <> 0 Then
                    appealRequest.ReturnByGroup = SESSION_GROUP_ID
                    appealRequest.ReturnToGroup = GetSentToInfo(statusCodeOut).Item1
                End If

                Dim rwRec As New ALOD.Core.Domain.Modules.Lod.Rwoa
                rwRec.RefId = appealRequest.Id
                rwRec.Workflow = AFRCWorkflows.SARCRestrictedAppeal
                rwRec.WorkStatus = statusOut
                rwRec.SentTo = GetSentToInfo(statusCodeOut).Item2
                rwRec.DateSent = DateTime.Now
                rwRec.Sender = UserGroupDao.GetNameById(SESSION_GROUP_ID, SESSION_COMPO)
                rwRec.ExplantionSendingBack = Server.HtmlEncode(CommentsTextBox.Text)
                rwRec.CreatedDate = DateTime.Now
                rwRec.CreatedBy = SESSION_USER_ID

                If (Not String.IsNullOrEmpty(rblReason.SelectedValue)) Then
                    rwRec.ReasonSentBack = rblReason.SelectedValue
                End If

                rwoaDao.Save(rwRec)

            End If

        End Sub

        Public Function IsReturnedToBoard(ByVal statusIn As Short) As Boolean
            'If board- unit -board
            'This will check last two stages before the current status
            If boardStages.Contains(statusIn) Then
                Dim ws As IList(Of WorkStatusTracking) = WorkFlowService.GetWorkStatusTracking(appealRequest.Id, ModuleType.AppealRequest)
                If ws IsNot Nothing Then
                    If ws.Count > 1 Then
                        If unitStages.Contains(ws(0).WorkflowStatus.StatusCodeType.Id) Then     'The last stage was unit stage
                            If boardStages.Contains(ws(1).WorkflowStatus.StatusCodeType.Id) Then     'The stage before that was board
                                Return True
                            End If
                        End If
                    End If
                End If
            End If
            Return False
        End Function

        Public Function IsReturnedToUnit(ByVal statusIn As Short, ByVal statusOut As Short) As Boolean
            'Board--Unit --SImply check the last status if it  is from board and the current status is one of the units then it has been returned to Unit
            If (boardStages.Contains(statusIn)) Then
                If unitStages.Contains(statusOut) Then
                    Return True
                End If
            End If
            Return False
        End Function

        Public Sub ReturnRWOA(ByVal statusIn As Integer)

            Dim rwoaDao As IRwoaDao = DaoFactory.GetRwoaDao()

            Dim rwRec = rwoaDao.GetRecentRWOA(appealRequest.Workflow, appealRequest.Id)

            If (rwRec IsNot Nothing) Then
                If (rwRec.DateSentBack Is Nothing AndAlso rwRec.WorkStatus = statusIn) Then
                    rwRec.DateSentBack = DateTime.Now
                    rwRec.CommentsBackToSender = ucRLB.MedTechComments
                    rwRec.rerouting = 0
                ElseIf (rwRec.DateSentBack Is Nothing AndAlso Not rwRec.WorkStatus = statusIn) Then
                    rwRec.rerouting = 1
                    rwRec.DateSentBack = DateTime.Now
                End If
            End If

        End Sub

        Protected Sub GenerateMemoLOD()
            Dim memo As Memorandum = appealRequest.GetDeterminationMemorandumFromLOD(UserService.CurrentUser(), New DBSignService(DBSignTemplateId.Form348APSARCFindings, appealRequest.Id, PersonnelTypes.APPELLATE_AUTH), DaoFactory)

            If (memo Is Nothing) Then
                LogManager.LogError("Failed to generate determination memo for SARC Appeal case " & appealRequest.CaseId)
                Exit Sub
            End If

            MemoDao.SaveOrUpdate(memo)

            LogManager.LogAction(ModuleType, UserAction.MemoAdded, appealRequest.Id, "Generated " & memo.Template.Title)
        End Sub

        Protected Sub GenerateMemoSARC()
            Dim memo As Memorandum2 = appealRequest.GetDeterminationMemorandumFromSARC(UserService.CurrentUser(), New DBSignService(DBSignTemplateId.Form348APSARCFindings, appealRequest.Id, PersonnelTypes.APPELLATE_AUTH), DaoFactory)

            If (memo Is Nothing) Then
                LogManager.LogError("Failed to generate determination memo for SARC Appeal case " & appealRequest.CaseId)
                Exit Sub
            End If

            MemoDao2.SaveOrUpdate(memo)

            LogManager.LogAction(ModuleType, UserAction.MemoAdded, appealRequest.Id, "Generated " & memo.Template.Title)
        End Sub

        Private Sub AddSignature(ByVal groupId As Integer)
            Dim user As AppUser = UserService.CurrentUser()
            appealRequest.AddSignature(DaoFactory, groupId, user.SignatureTitle, user)
        End Sub

#End Region

    End Class

End Namespace