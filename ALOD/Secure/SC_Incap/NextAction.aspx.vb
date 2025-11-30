Imports ALOD.Core.Domain.Common
Imports ALOD.Core.Domain.Common.CustomServerControls
Imports ALOD.Core.Domain.Documents
Imports ALOD.Core.Domain.Lookup
Imports ALOD.Core.Domain.Modules.Lod
Imports ALOD.Core.Domain.Modules.SpecialCases
Imports ALOD.Core.Domain.Users
Imports ALOD.Core.Domain.Workflow
Imports ALOD.Core.Interfaces.DAOInterfaces
Imports ALOD.Core.Utils
Imports ALOD.Core.Utils.NextActionHelpers
Imports ALOD.Data
Imports ALOD.Data.Services
Imports ALOD.Logging
Imports ALOD.Web.UserControls
Imports ALODWebUtility.Common
Imports ALODWebUtility.Worklfow

Namespace Web.Special_Case.IN

    Partial Class Secure_sc_in_NextAction
        Inherits System.Web.UI.Page

        Private Const VIEWSTATE_VALIDATIONS As String = "VALIDATION_ITEMS"
        Private _docCatViewDao As IDocCategoryViewDao
        Private _docDao As IDocumentDao
        Private _factory As IDaoFactory
        Private _lookupDao As ILookupDao
        Private _memoDao As IMemoDao2
        Private _scAccess As Dictionary(Of String, ALOD.Core.Domain.Workflow.PageAccessType)
        Private _sdao As ISpecialCaseDAO
        Private _special As SpecialCase
        Private _workstatusDao As IWorkStatusDao
        Private CANCELLATION_REASON As String = "Case Cancelled By "
        Private errors As New List(Of ValidationItem)
        Private sc As SC_Incap
        Private separatorTemplate As System.Web.UI.CompiledTemplateBuilder

#Region "SC_IncapProperty"

        ReadOnly Property DaoFactory() As IDaoFactory
            Get
                If (_factory Is Nothing) Then
                    _factory = New NHibernateDaoFactory()
                End If

                Return _factory
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

        ReadOnly Property RefId() As Integer
            Get
                Return Integer.Parse(Request.QueryString("refId"))
            End Get
        End Property

        ReadOnly Property SCDao() As ISpecialCaseDAO
            Get
                If (_sdao Is Nothing) Then
                    _sdao = DaoFactory.GetSpecialCaseDAO()
                End If

                Return _sdao
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

        Protected ReadOnly Property INCAP() As SC_Incap
            Get
                If (sc Is Nothing) Then
                    sc = SCDao.GetById(RefId)
                End If

                Return sc
            End Get
        End Property

        Protected ReadOnly Property MasterPage() As SC_IncapMaster
            Get
                Dim master As SC_IncapMaster = CType(Page.Master, SC_IncapMaster)
                Return master
            End Get
        End Property

        Protected ReadOnly Property ModuleType() As ModuleType
            Get
                Return ModuleType.SpecCaseIncap
            End Get
        End Property

        Protected ReadOnly Property SectionList() As Dictionary(Of String, ALOD.Core.Domain.Workflow.PageAccessType)
            Get
                If (_scAccess Is Nothing) Then
                    _scAccess = INCAP.ReadSectionList(SESSION_GROUP_ID)
                End If
                Return _scAccess
            End Get
        End Property

        Protected ReadOnly Property SpecCase() As SpecialCase
            Get
                If (_special Is Nothing) Then
                    _special = SCDao.GetById(RefId, False)

                End If
                Return _special
            End Get

        End Property

        Private ReadOnly Property MemoDao() As IMemoDao2
            Get
                If (_memoDao Is Nothing) Then
                    _memoDao = DaoFactory.GetMemoDao2()
                End If
                Return _memoDao
            End Get
        End Property

        Public Sub CommitChanges()
            SCDao.CommitChanges()
            SCDao.Evict(INCAP)
            _sdao = Nothing
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

                INCAP.ModifiedBy = SESSION_USER_ID
                INCAP.ModifiedDate = DateTime.Now
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
            INCAP.CaseCancelExplanation = Server.HtmlEncode(CommentsTextBox.Text.Trim) '            If rblCancellation.SelectedValue <> "" Then
            If rblReason.SelectedValue <> "" Then
                INCAP.CaseCancelReason = CShort(rblReason.SelectedValue) '                LOD.phy_cancel_reason = rblCancellation.SelectedValue
            End If

            INCAP.CaseCancelDate = DateTime.Now

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
                CommentsTextBox.MaxLength = 400
                SetMaxLength(CommentsTextBox)

                reasonRow.Visible = True
                reasonLetter.Text = "B"
                commentsRow.Visible = True
                commentLetter.Text = "C"

            ElseIf (workOption.DisplayText Like "Return*") Then
                PopulateReturnrbl()

                reasonTitle.Text = "* Reason for Return:"
                commentsTitle.Text = "* Return Comments:"
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

        Protected Function GetStatusInfo(ByVal statusCodeOut As Integer) As Tuple(Of Integer, String)

            Dim sentTo As String = ""
            Dim sentToId As Integer = 0

            Select Case statusCodeOut
                'Case SpecCaseIncapStatusCode.InitiateCase, SpecCaseIncapStatusCode.FinalReview
                '    sentTo = "HQ Tech"
                '    sentToId = UserGroups.AFRCHQTechnician
                'Case SpecCaseIncapStatusCode.MedicalReview
                '    sentTo = "Board Medical"
                '    sentToId = UserGroups.BoardMedical
                'Case SpecCaseIncapStatusCode.SeniorMedicalReview
                '    sentTo = "Senior Medical Reviewer"
                '    sentToId = UserGroups.SeniorMedicalReviewer
                'Case Else
                '    sentTo = ""
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

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

            Dim eventTarget As String

            If (Not Page.IsPostBack) Then

                If (SpecCase.DocumentGroupId Is Nothing OrElse SpecCase.DocumentGroupId = 0) Then
                    SpecCase.CreateDocumentGroup(DocumentDao)
                    SCDao.SaveOrUpdate(SpecCase)  'Save the new Document Group ID with the Special Case
                    SCDao.CommitChanges()
                End If

                SpecCase.ProcessDocuments(DaoFactory)

                UserCanEdit = GetAccess(Navigator.PageAccess, True)
                InitControls()
                InitData()
                ShowRLB()
                InitNextActionOptions()

                LogManager.LogAction(ModuleType, UserAction.ViewPage, RefId, "Viewed Page: Next Action")
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
            rblReason.DataSource = LookupService.GetWorkflowCancelReasons(INCAP.Workflow, 0)
            rblReason.DataTextField = "Description"
            rblReason.DataValueField = "Id"
            rblReason.DataBind()
        End Sub

        Protected Sub PopulateReturnrbl()
            NextActionHelpers.PopulateRadioButtonListWithReturnOptions(rblReason, INCAP.Workflow, LookupDao)
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

        Protected Sub SignatureCompleted(ByVal sender As Object, ByVal e As SignCompletedEventArgs) Handles SigBlock.SignCompleted

            If (e.SignaturePassed) Then

                'we have a good signature.  take any further actions that are required
                ChangeStatus(e.OptionId, e.StatusOut, e.Text, e.Comments)
                SetFeedbackMessage("Case " + INCAP.CaseId + " successfully signed.  Action applied: " + e.Text)
                Response.Redirect(Resources._Global.StartPage, True)
            End If

        End Sub

        Protected Sub SignButton_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles SignButton.Click

            Dim workOption = GetWorkOption()

            'make sure we have an option selected
            If (workOption Is Nothing) Then
                options.Style.Value = "fieldRequired"
                errors.Add(New ValidationItem("Routing", "Routing", "A Routing option must be selected"))
                Exit Sub
            Else
                options.Style.Value = ""
            End If

            'we have an option selected

            If (workOption.DisplayText Like "Cancel*") Then
                'the user is cancelling, do we have a cancel reason?
                If (Not ValidateReasonControls("Cancellation")) Then
                    Exit Sub
                End If

                'we have a cancel reason, so add save it
                SaveCancelData()

            End If

            If (workOption.DisplayText Like "Return*") Then
                If (Not ValidateReasonControls("Return")) Then
                    Exit Sub
                End If
            End If

            ReasonPanel.CssClass = ""

            If Not IsNothing(INCAP.ReturnToGroup) Then  'Clear the Return Group info when the recipient Replies/Forwards
                If SESSION_GROUP_ID = INCAP.ReturnToGroup Then
                    INCAP.ReturnToGroup = Nothing
                    INCAP.ReturnByGroup = Nothing
                End If
            End If

            Dim template As Integer = workOption.Template

            Dim secId As Integer = CInt(GetPersonnelTypeFromGroup(SESSION_GROUP_ID, False))

            Dim selectedText As String
            If (rblReason.SelectedValue.Length > 0) Then
                selectedText = rblReason.SelectedItem.Text
            Else
                selectedText = String.Empty
            End If

            'Adding RWOA/Cancel Reason to the Log
            Dim LogActionDisplayText = LogText(workOption.DisplayText, selectedText, Server.HtmlEncode(CommentsTextBox.Text.Trim()))

            SCDao.SaveOrUpdate(INCAP)
            CommitChanges()
            SigBlock.StartSignature(RefId, INCAP.Workflow, secId, LogActionDisplayText, INCAP.Status, workOption.wsStatusOut, workOption.Id, template, String.Empty)

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

            If (RefId = 0) Then
                Exit Sub
            End If

            'show the comments on the cancel/return (if there are any)
            CommentPanel.Visible = False
            CommentLabel.Text = ""

            If (Not IsNothing(INCAP.CaseCancelReason)) Then  'Check if Cancelled - show Cancel Reason
                If INCAP.CaseCancelReason > 0 Then
                    InitCancelComments(INCAP.CaseCancelReason, INCAP.CaseCancelExplanation)

                    CommentPanel.Visible = True
                End If
            End If

        End Sub

        Private Sub InitNextActionOptions()

            'as part of this method call the validation errors are generated
            'so we will store them from here for later display
            Dim options = From o In INCAP.GetCurrentINCAPOptions(WorkFlowService.GetLastStatus(INCAP.Id, ModuleType), DaoFactory, INCAP.INCAPFindings.First)
                          Select o
                          Order By o.SortOrder

            'Dim x As String = SpecCase.CaseId
            'Dim z As IList(Of Document) = SpecCase.Documents

            rbtOption.DataSource = options
            rbtOption.DataBind()

            ViewState(VIEWSTATE_VALIDATIONS) = INCAP.Validations

        End Sub

        Private Sub ShowRLB()

            Dim ws As IList(Of WorkStatusTracking) = WorkFlowService.GetWorkStatusTracking(INCAP.Id, ModuleType)

            If (ws IsNot Nothing AndAlso ws.Count > 1) Then

                Dim returnDao As IReturnDao = New NHibernateDaoFactory().GetReturnDao

                Dim rwRec = returnDao.GetRecentReturn(INCAP.Workflow, INCAP.Id)

                If (rwRec IsNot Nothing) AndAlso (rwRec.WorkStatusFrom = INCAP.WorkflowStatus.Id Or rwRec.WorkStatusTo = INCAP.WorkflowStatus.Id) Then
                    RLBVisible = True
                    RLBTitle.Text = "Returned"

                    ucRLB.Initialize(returnDao.GetRecentReturn(INCAP.Workflow, INCAP.Id))

                    If rwRec.WorkStatusTo <> INCAP.WorkflowStatus.Id Then
                        RLBEnabled = False
                    End If
                End If
            End If
        End Sub

#Region "ChangeStatusAndActions"

        Public Sub AddEmails(ByRef emails As MailManager, emailService As EmailService, action As WorkflowOptionAction, statusOut As Integer)

            Dim toList As StringCollection

            If ((statusOut = SpecCaseIncapWorkStatus.INComplete) Or (statusOut = SpecCaseIncapWorkStatus.INInitiate)) Then
                'add it to the email manager
                toList = emailService.GetDistributionListByGroupSC(INCAP.Id, action.Target)
            Else
                toList = emailService.GetDistributionListByWorkflow(INCAP.Id, statusOut, INCAP.Workflow, action.Target)
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

                    Case WorkflowActionType.AddSignature
                        AddSignature(actn.Target)
                    Case WorkflowActionType.RemoveSignature
                        INCAP.RemoveSignature(DaoFactory, statusOut)
                    Case WorkflowActionType.SendEmail
                        AddEmails(emails, mailService, actn, statusOut)
                    Case WorkflowActionType.ReturnTo
                        ReturnTo(statusIn, statusOut)
                    Case WorkflowActionType.ReturnBack
                        ReturnBack(statusIn, statusOut, False)
                End Select

            Next

            Dim user As AppUser = UserService.CurrentUser()

            emails.SetField("CASE_NUMBER", INCAP.CaseId)
            emails.SetField("APP_LINK", GetHostName())
            emails.SetField("MEMBER_NAME", INCAP.MemberName)
            emails.SetField("SIGNATURE", String.Format("{0}, {1}", user.LastName, user.Rank.Rank))

            emails.SendAll()

        End Sub

        Public Sub ChangeStatus(optionId As Integer, newStatus As Integer, text As String, comments As String)
            Dim passed As Boolean = True
            Dim statusIn As Integer = INCAP.Status

            If (newStatus <> INCAP.Status) Then
                LogManager.LogAction(ModuleType, UserAction.Signed, INCAP.Id, Server.HtmlEncode(CommentsTextBox.Text.Trim), statusIn, newStatus)
                LogManager.LogAction(ModuleType, UserAction.StatusChanged, INCAP.Id, text, statusIn, newStatus)

                ApplyActions(optionId, statusIn, newStatus)

                INCAP.Status = newStatus
                INCAP.ModifiedBy = SESSION_USER_ID
                INCAP.ModifiedDate = DateTime.Now()

                SCDao.SaveOrUpdate(INCAP)
                CommitChanges()
            End If

            Dim reminderDao = New ReminderEmailsDao()
            reminderDao.ReminderEmailUpdateStatusChange(statusIn, newStatus, INCAP.CaseId, "SC")
        End Sub

        Public Sub ReturnBack(ByVal statusIn As Integer, ByVal statusOut As Integer, ByVal ReturnFlag As Boolean)
            Dim statusCodeIn As Integer = WorkstatusDao.GetById(statusIn).StatusCodeType.Id
            Dim statusCodeOut As Integer = WorkstatusDao.GetById(statusOut).StatusCodeType.Id

            Dim returnDao As IReturnDao = New NHibernateDaoFactory().GetReturnDao

            Dim rwRec = returnDao.GetRecentReturn(INCAP.Workflow, INCAP.Id)

            Dim sentTo As String = GetStatusInfo(statusCodeOut).Item2

            If (rwRec IsNot Nothing AndAlso rwRec.DateSentBack Is Nothing) Then

                If (rwRec.WorkStatusFrom = statusOut) Then                      'Case made it back to where the Return came from

                    If (rwRec.WorkStatusTo = statusIn) Then                     'Direct Reply
                        rwRec.DateSentBack = DateTime.Now
                        rwRec.CommentsBackToSender = ucRLB.MedTechComments
                        rwRec.rerouting = 0
                    Else                                                        'Case was rerouted
                        rwRec.rerouting = 1
                        rwRec.DateSentBack = DateTime.Now
                    End If

                ElseIf (statusCodeOut = SpecCaseIncapStatusCode.IN_Cancelled) Then           'Case was canceled during Return
                    rwRec.DateSentBack = DateTime.Now
                    If (String.IsNullOrEmpty(rwRec.CommentsBackToSender)) Then
                        rwRec.CommentsBackToSender = "Case has been Cancelled: " + ucRLB.MedTechComments
                        rwRec.rerouting = 0
                    End If

                ElseIf (statusCodeOut = SpecCaseIncapStatusCode.IN_Complete) Then            'Case was completed during Return
                    rwRec.DateSentBack = DateTime.Now
                    If (String.IsNullOrEmpty(rwRec.CommentsBackToSender)) Then
                        rwRec.CommentsBackToSender = "Case has been Completed: " + ucRLB.MedTechComments
                        rwRec.rerouting = 0
                    End If

                ElseIf (ReturnFlag) Then                                        'Case was returned again
                    rwRec.DateSentBack = DateTime.Now
                    If (String.IsNullOrEmpty(rwRec.CommentsBackToSender)) Then
                        rwRec.CommentsBackToSender = "Case was Returned to " + sentTo + ": " + ucRLB.MedTechComments
                        rwRec.rerouting = 1
                    End If
                Else                                                            'Case was forwarded
                    rwRec.CommentsBackToSender = "Case was Forwarded to " + sentTo + ": " + ucRLB.MedTechComments
                    rwRec.rerouting = 1
                End If

            End If

        End Sub

        Public Sub ReturnTo(ByVal statusIn As Integer, ByVal statusOut As Integer)
            Dim statusCodeIn As Integer = WorkstatusDao.GetById(statusIn).StatusCodeType.Id
            Dim statusCodeOut As Integer = WorkstatusDao.GetById(statusOut).StatusCodeType.Id

            Dim returnDao As IReturnDao = New NHibernateDaoFactory().GetReturnDao

            Dim oldReturn = returnDao.GetRecentReturn(INCAP.Workflow, INCAP.Id)
            If (oldReturn IsNot Nothing AndAlso oldReturn.DateSentBack Is Nothing) Then
                ReturnBack(statusIn, statusOut, True)
            End If

            If GetStatusInfo(statusCodeOut).Item1 <> 0 Then
                INCAP.ReturnByGroup = SESSION_GROUP_ID
                INCAP.ReturnToGroup = GetStatusInfo(statusCodeOut).Item1
            End If

            Dim rwRec As New ALOD.Core.Domain.Modules.Lod.Return
            rwRec.RefId = INCAP.Id
            rwRec.Workflow = INCAP.Workflow
            rwRec.WorkStatusTo = statusOut
            rwRec.WorkStatusFrom = statusIn
            rwRec.SentTo = GetStatusInfo(statusCodeOut).Item1
            rwRec.DateSent = DateTime.Now
            rwRec.Sender = SESSION_GROUP_ID
            rwRec.ExplantionSendingBack = Server.HtmlEncode(CommentsTextBox.Text)
            rwRec.CreatedDate = DateTime.Now
            rwRec.CreatedBy = SESSION_USER_ID

            rwRec.BoardReturn = 1

            If (Not String.IsNullOrEmpty(rblReason.SelectedValue)) Then
                rwRec.ReasonSentBack = rblReason.SelectedValue
            End If

            returnDao.Save(rwRec)

        End Sub

        Private Sub AddSignature(ByVal groupId As Integer)
            Dim user As AppUser = UserService.CurrentUser()
            INCAP.AddSignature(DaoFactory, groupId, user.SignatureName, user.SignatureTitle, user)
        End Sub

#End Region

    End Class

End Namespace