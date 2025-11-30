Imports ALOD.Core.Domain.Common
Imports ALOD.Core.Domain.Common.CustomServerControls
Imports ALOD.Core.Domain.DBSign
Imports ALOD.Core.Domain.Documents
Imports ALOD.Core.Domain.Lookup
Imports ALOD.Core.Domain.Modules.Lod
Imports ALOD.Core.Domain.Modules.Reinvestigations
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

Namespace Web.RR

    Partial Class Secure_rr_NextAction
        Inherits System.Web.UI.Page

        Private Const VIEWSTATE_VALIDATIONS As String = "VALIDATION_ITEMS"
        Private _daoFactory As IDaoFactory
        Private _docCatViewDao As IDocCategoryViewDao
        Private _docDao As IDocumentDao
        Private _ldao As ILineOfDutyDao
        Private _lod As LineOfDuty
        Private _lookupDao As ILookupDao
        Private _memoDao As IMemoDao2
        Private _rdao As ILODReinvestigateDAO
        Private _request As LODReinvestigation
        Private _scAccess As Dictionary(Of String, ALOD.Core.Domain.Workflow.PageAccessType)
        Private _usergroupDao As IUserGroupDao
        Private _workstatusDao As IWorkStatusDao

        Private boardStages As Short() = New Short() {
            CShort(ReinvestigationRequestStatusCode.BoardTechRequestReview),
            CShort(ReinvestigationRequestStatusCode.BoardMedicalRequestReview),
            CShort(ReinvestigationRequestStatusCode.SeniorMedicalReview),
            CShort(ReinvestigationRequestStatusCode.BoardLegalRequestReview),
            CShort(ReinvestigationRequestStatusCode.ApprovingAuthorityRequestAction),
            CShort(ReinvestigationRequestStatusCode.BoardA1RequestReview),
            CShort(ReinvestigationRequestStatusCode.BoardTechRequestFinalReview)}

        Private CANCELLATION_REASON As String = "Case Cancelled By "
        Private errors As New List(Of ValidationItem)
        Private separatorTemplate As System.Web.UI.CompiledTemplateBuilder

        Private unitStages As Short() = New Short() {
            CShort(ReinvestigationRequestStatusCode.InitiateRequest),
            CShort(ReinvestigationRequestStatusCode.WingJARequestReview),
            CShort(ReinvestigationRequestStatusCode.WingCCRequestReview)}

#Region "LODProperty"

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

        ReadOnly Property LODDao() As ILineOfDutyDao
            Get
                If (_ldao Is Nothing) Then
                    _ldao = DaoFactory.GetLineOfDutyDao()
                End If

                Return _ldao
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

        ReadOnly Property MemoDao() As IMemoDao2
            Get
                If (_memoDao Is Nothing) Then
                    _memoDao = DaoFactory.GetMemoDao2()
                End If

                Return _memoDao
            End Get
        End Property

        ReadOnly Property RefId() As Integer
            Get
                Return reinvestigationRequest.InitialLodId
            End Get
        End Property

        ReadOnly Property reinvestigationRequest() As LODReinvestigation
            Get
                If (_request Is Nothing) Then

                    If RequestId <> 0 Then
                        _request = RRDao.GetById(RequestId)
                    Else
                        _request = Nothing
                    End If
                End If
                Return _request
            End Get
        End Property

        ReadOnly Property RequestId() As Integer
            Get
                Return CInt(Request.QueryString("requestId"))
            End Get
        End Property

        ReadOnly Property RRDao() As ILODReinvestigateDAO
            Get
                If (_rdao Is Nothing) Then
                    _rdao = DaoFactory.GetLODReinvestigationDao()
                End If

                Return _rdao
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

        Protected ReadOnly Property LOD() As LineOfDuty
            Get
                If (_lod Is Nothing) Then
                    _lod = LODDao.GetById(RefId, False)

                End If
                Return _lod
            End Get
        End Property

        Protected ReadOnly Property MasterPage() As ReinvestigationRequestMaster
            Get
                Dim master As ReinvestigationRequestMaster = CType(Page.Master, ReinvestigationRequestMaster)
                Return master
            End Get
        End Property

        Protected ReadOnly Property ModuleType() As ModuleType
            Get
                Return ModuleType.ReinvestigationRequest
            End Get
        End Property

        Protected ReadOnly Property SectionList() As Dictionary(Of String, ALOD.Core.Domain.Workflow.PageAccessType)
            Get
                If (_scAccess Is Nothing) Then
                    _scAccess = reinvestigationRequest.ReadSectionList(SESSION_GROUP_ID)
                End If
                Return _scAccess
            End Get
        End Property

        Public Sub CommitChanges()
            RRDao.CommitChanges()
            RRDao.Evict(reinvestigationRequest)
            _request = Nothing
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

                reinvestigationRequest.ModifiedBy = SESSION_USER_ID
                reinvestigationRequest.ModifiedDate = DateTime.Now
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
            reinvestigationRequest.Cancel_Explanation = Server.HtmlEncode(CommentsTextBox.Text.Trim)
            If rblReason.SelectedValue <> "" Then
                reinvestigationRequest.Cancel_Reason = CShort(rblReason.SelectedValue)
            End If
            reinvestigationRequest.Cancel_Date = DateTime.Now

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
                Case ReinvestigationRequestStatusCode.InitiateRequest
                    sentTo = "LOD-PM"
                    sentToId = UserGroups.LOD_PM
                Case ReinvestigationRequestStatusCode.WingJARequestReview
                    sentTo = "Wing JA"
                    sentToId = UserGroups.WingJudgeAdvocate
                Case ReinvestigationRequestStatusCode.WingCCRequestReview
                    sentTo = "Wing CC"
                    sentToId = UserGroups.WingCommander
                Case ReinvestigationRequestStatusCode.BoardTechRequestReview, ReinvestigationRequestStatusCode.BoardTechRequestFinalReview
                    sentTo = "Board Technician"
                    sentToId = UserGroups.BoardTechnician
                Case ReinvestigationRequestStatusCode.BoardMedicalRequestReview
                    sentTo = "Board Medical"
                    sentToId = UserGroups.BoardMedical
                Case ReinvestigationRequestStatusCode.SeniorMedicalReview
                    sentTo = "Senior Medical Reviewer"
                    sentToId = UserGroups.SeniorMedicalReviewer
                Case ReinvestigationRequestStatusCode.BoardLegalRequestReview
                    sentTo = "Board Legal"
                    sentToId = UserGroups.BoardLegal
                Case ReinvestigationRequestStatusCode.ApprovingAuthorityRequestAction
                    sentTo = "Approving Authority"
                    sentToId = UserGroups.BoardApprovalAuthority
                Case ReinvestigationRequestStatusCode.BoardA1RequestReview
                    sentTo = "Board Administrator"
                    sentToId = UserGroups.BoardAdministrator

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
            Dim finding As LODReinvestigationFindings = reinvestigationRequest.FindFindingByPersonnelType(pType)

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

                LogManager.LogAction(ModuleType, UserAction.ViewPage, RequestId, "Viewed Page: RR Next Action")

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
            rblReason.DataSource = LookupService.GetWorkflowCancelReasons(reinvestigationRequest.Workflow, 0)
            rblReason.DataTextField = "Description"
            rblReason.DataValueField = "Id"
            rblReason.DataBind()
        End Sub

        Protected Sub PopulateReturnrbl()
            NextActionHelpers.PopulateRadioButtonListWithReturnOptions(rblReason, reinvestigationRequest.Workflow, LookupDao)
        End Sub

        Protected Sub PopulateRWOArbl()
            NextActionHelpers.PopulateRadioButtonListWithRwoaOptions(rblReason, reinvestigationRequest.Workflow, LookupDao)
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
                ChangeStatus(e.OptionId, e.StatusOut, e.Text)
                SetFeedbackMessage("Case " + reinvestigationRequest.CaseId + " successfully signed.  Action applied: " + e.Text)
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

            If (workOption.DisplayText Like "Cancel*") Then
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

            If (workOption.DisplayText Like "Return*") Then
                If (Not ValidateReasonControls("Return")) Then
                    Exit Sub
                End If
            End If

            ReasonPanel.CssClass = ""

            If Not IsNothing(reinvestigationRequest.ReturnToGroup) Then  'Clear the Return Group info when the recipient Replies/Forwards
                If SESSION_GROUP_ID = reinvestigationRequest.ReturnToGroup Then
                    reinvestigationRequest.ReturnToGroup = Nothing
                    reinvestigationRequest.ReturnByGroup = Nothing
                End If
            End If

            Dim template As Integer = workOption.Template

            If (template = DBSignTemplateId.Form348RRFindings AndAlso Not MemberHasFindings()) Then
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
            SigBlock.StartSignature(RequestId, reinvestigationRequest.Workflow, secId, LogActionDisplayText, reinvestigationRequest.Status, workOption.wsStatusOut, workOption.Id, template, String.Empty)
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

            If ((RefId = 0) And (RequestId = 0)) Then
                Exit Sub
            End If

            'show the comments on the cancel/return (if there are any)
            CommentPanel.Visible = False
            CommentLabel.Text = ""

            If (Not IsNothing(reinvestigationRequest.Cancel_Reason)) Then  'Check if Cancelled - show Cancel Reason
                If reinvestigationRequest.Cancel_Reason > 0 Then
                    InitCancelComments(reinvestigationRequest.Cancel_Reason, reinvestigationRequest.Cancel_Explanation)

                    CommentPanel.Visible = True
                End If
            End If

        End Sub

        Private Sub InitNextActionOptions()

            'as part of this method call the validation errors are generated
            'so we will store them from here for later display
            Dim options = From o In reinvestigationRequest.GetCurrentOptions(WorkFlowService.GetLastStatus(reinvestigationRequest.Id, ModuleType), DaoFactory)
                          Select o
                          Order By o.SortOrder

            ''zach
            'If Session(SESSIONKEY_COMPO) = "5" Then

            '    For Each o In options

            '        '135 141
            '        If o.Id = 135 OrElse o.Id = 141 OrElse o.Id = 94 Then
            '            o.OptionValid = False
            '            o.OptionVisible = False
            '        End If

            '    Next

            'ElseIf Session(SESSIONKEY_COMPO) = "6" Then

            '    For Each o In options

            '        '1981 1980 1979
            '        If o.Id >= 962 Then
            '            o.OptionValid = False
            '            o.OptionVisible = False
            '        End If

            '    Next

            'End If

            rbtOption.DataSource = options
            rbtOption.DataBind()

            ViewState(VIEWSTATE_VALIDATIONS) = reinvestigationRequest.Validations

        End Sub

        Private Sub ShowRLB()

            Dim access As ALOD.Core.Domain.Workflow.PageAccessType = SectionList(RRSectionNames.RR_RLB.ToString())
            Dim ws As IList(Of WorkStatusTracking) = WorkFlowService.GetWorkStatusTracking(reinvestigationRequest.Id, ModuleType)

            If access <> ALOD.Core.Domain.Workflow.PageAccessType.None Then

                If (ws IsNot Nothing AndAlso ws.Count > 1) Then
                    If ((IsReturnedToBoard(reinvestigationRequest.CurrentStatusCode) OrElse IsReturnedToUnit(ws(0).WorkflowStatus.StatusCodeType.Id, reinvestigationRequest.CurrentStatusCode))) Then
                        'get the last status from workflowtransaction class
                        RLBVisible = True
                        RLBTitle.Text = "Returned Without Action"

                        Dim rwoaDao As IRwoaDao = New NHibernateDaoFactory().GetRwoaDao
                        ucRLB.Initialize(rwoaDao.GetRecentRWOA(reinvestigationRequest.Workflow, reinvestigationRequest.Id))
                        If access <> ALOD.Core.Domain.Workflow.PageAccessType.ReadWrite Then
                            RLBEnabled = False
                        End If
                        Exit Sub
                    End If
                End If
            End If

            If (ws IsNot Nothing AndAlso ws.Count > 1) Then

                If (CheckReturn(ws(0).WorkflowStatus.StatusCodeType.Id, reinvestigationRequest.CurrentStatusCode)) Then

                    Dim returnDao As IReturnDao = New NHibernateDaoFactory().GetReturnDao

                    Dim rwRec = returnDao.GetRecentReturn(reinvestigationRequest.Workflow, reinvestigationRequest.Id)

                    If (rwRec IsNot Nothing) AndAlso (rwRec.WorkStatusFrom = reinvestigationRequest.WorkflowStatus.Id Or rwRec.WorkStatusTo = reinvestigationRequest.WorkflowStatus.Id) Then
                        RLBVisible = True
                        RLBTitle.Text = "Returned"

                        ucRLB.Initialize(returnDao.GetRecentReturn(reinvestigationRequest.Workflow, reinvestigationRequest.Id))

                        If rwRec.WorkStatusTo <> reinvestigationRequest.WorkflowStatus.Id Then
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

            If (statusCodeOut = ReinvestigationRequestStatusCode.Cancelled OrElse statusCodeOut = ReinvestigationRequestStatusCode.RequestApproved OrElse statusCodeOut = ReinvestigationRequestStatusCode.RequestDenied) Then
                'add it to the email manager
                toList = emailService.GetDistributionListByGroup(reinvestigationRequest.Id, action.Target, "reinvestigationRequest")
            Else
                toList = emailService.GetDistributionListByWorkflow(reinvestigationRequest.Id, statusOut, reinvestigationRequest.Workflow, action.Target)
            End If

            emails.AddTemplate(action.Data, "", toList)
        End Sub

        Public Sub ApplyActions(optionId As Integer, statusIn As Integer, statusOut As Integer)
            Dim optn As ALOD.Core.Domain.Workflow.WorkflowStatusOption = WorkFlowService.GetOptionById(optionId)

            If (optn Is Nothing) Then
                Exit Sub
            End If

            Dim mailService As EmailService = New EmailService()
            Dim emails As New ALOD.Core.Domain.Common.MailManager(mailService)

            For Each actn As WorkflowOptionAction In optn.ActionList
                Select Case actn.ActionType.Id
                    Case WorkflowActionType.SaveFinalDecision
                        SetFinalDecision(statusIn, statusOut)
                    Case WorkflowActionType.AddSignature
                        AddSignature(actn.Target)
                    Case WorkflowActionType.RemoveSignature
                        reinvestigationRequest.RemoveSignature(DaoFactory, statusOut)
                    Case WorkflowActionType.SendEmail
                        AddEmails(emails, mailService, actn, statusOut)
                    Case WorkflowActionType.AddApprovalAuthoritySignature
                        AddApprovalAuthoritySignature()
                    Case WorkflowActionType.SaveRWOA
                        InsertRWOA(statusIn, statusOut)
                    Case WorkflowActionType.ReturnRWOA
                        ReturnRWOA(statusIn)
                    Case WorkflowActionType.ReturnTo
                        ReturnTo(statusIn, statusOut)
                    Case WorkflowActionType.ReturnBack
                        ReturnBack(statusIn, statusOut, False)
                End Select
            Next

            emails.SetField("CASE_NUMBER", reinvestigationRequest.CaseId)
            emails.SetField("APP_LINK", GetHostName())
            emails.SendAll()

        End Sub

        Public Sub ChangeStatus(optionId As Integer, newStatus As Integer, text As String)
            Dim statusIn As Integer = reinvestigationRequest.Status

            If (newStatus <> reinvestigationRequest.Status) Then
                LogManager.LogAction(ModuleType, UserAction.Signed, reinvestigationRequest.Id, Server.HtmlEncode(CommentsTextBox.Text.Trim), statusIn, newStatus)
                LogManager.LogAction(ModuleType, UserAction.StatusChanged, reinvestigationRequest.Id, text, statusIn, newStatus)

                ApplyActions(optionId, statusIn, newStatus)

                reinvestigationRequest.Status = newStatus
                reinvestigationRequest.ModifiedBy = SESSION_USER_ID
                reinvestigationRequest.ModifiedDate = DateTime.Now()

                RRDao.SaveOrUpdate(reinvestigationRequest)
                CommitChanges()
            End If

            Dim reminderDao = New ReminderEmailsDao()
            reminderDao.ReminderEmailUpdateStatusChange(statusIn, newStatus, reinvestigationRequest.CaseId, "RR")
        End Sub

        Public Function CheckReturn(ByVal statusIn As Short, ByVal statusOut As Short) As Boolean
            'Check if the case could be a return
            If (unitStages.Contains(statusOut) AndAlso unitStages.Contains(statusOut)) Then
                Return True
            ElseIf (boardStages.Contains(statusOut) AndAlso boardStages.Contains(statusOut)) Then
                Return True
            End If
            Return False
        End Function

        '<summary>
        '<mod tfsid="5125" date="02/22/2011">
        'Med off was not able to cancel lod if this was an override case or in case it was not returned by board and the form348 records rwoa reason and date were not set
        '</mod>
        '</summary>
        Public Sub InsertRWOA(ByVal statusIn As Integer, ByVal statusOut As Integer)
            Dim statusCodeIn As Integer = WorkstatusDao.GetById(statusIn).StatusCodeType.Id
            Dim statusCodeOut As Integer = WorkstatusDao.GetById(statusOut).StatusCodeType.Id

            Dim rwoaDao As IRwoaDao = New NHibernateDaoFactory().GetRwoaDao

            If (IsReturnedToUnit(statusCodeIn, statusCodeOut)) Then

                If GetStatusInfo(statusCodeOut).Item1 <> 0 Then
                    reinvestigationRequest.ReturnByGroup = SESSION_GROUP_ID
                    reinvestigationRequest.ReturnToGroup = GetStatusInfo(statusCodeOut).Item1
                End If

                Dim rwRec As New ALOD.Core.Domain.Modules.Lod.Rwoa
                rwRec.RefId = reinvestigationRequest.Id
                rwRec.Workflow = reinvestigationRequest.Workflow
                rwRec.WorkStatus = statusOut
                rwRec.SentTo = GetStatusInfo(statusCodeOut).Item2
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
                Dim ws As IList(Of WorkStatusTracking) = WorkFlowService.GetWorkStatusTracking(reinvestigationRequest.Id, ModuleType)
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

        Public Sub ReturnBack(ByVal statusIn As Integer, ByVal statusOut As Integer, ByVal ReturnFlag As Boolean)
            Dim statusCodeIn As Integer = WorkstatusDao.GetById(statusIn).StatusCodeType.Id
            Dim statusCodeOut As Integer = WorkstatusDao.GetById(statusOut).StatusCodeType.Id

            Dim returnDao As IReturnDao = New NHibernateDaoFactory().GetReturnDao

            Dim rwRec = returnDao.GetRecentReturn(reinvestigationRequest.Workflow, reinvestigationRequest.Id)

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

                ElseIf (statusCodeOut = ReinvestigationRequestStatusCode.Cancelled) Then           'Case was canceled during Return
                    rwRec.DateSentBack = DateTime.Now
                    If (String.IsNullOrEmpty(rwRec.CommentsBackToSender)) Then
                        rwRec.CommentsBackToSender = "Case has been Cancelled: " + ucRLB.MedTechComments
                        rwRec.rerouting = 0
                    End If

                ElseIf (statusCodeOut = ReinvestigationRequestStatusCode.RequestApproved OrElse statusCodeOut = ReinvestigationRequestStatusCode.RequestDenied) Then            'Case was completed during Return
                    rwRec.DateSentBack = DateTime.Now
                    If (String.IsNullOrEmpty(rwRec.CommentsBackToSender)) Then
                        rwRec.CommentsBackToSender = "Case has been Completed: " + ucRLB.MedTechComments
                        rwRec.rerouting = 0
                    End If

                ElseIf (IsReturnedToUnit(statusCodeIn, statusCodeOut)) Then                                        'Case was returned again
                    rwRec.DateSentBack = DateTime.Now
                    If (String.IsNullOrEmpty(rwRec.CommentsBackToSender)) Then
                        rwRec.CommentsBackToSender = "Case was RFA to " + sentTo + ": " + ucRLB.MedTechComments
                        rwRec.rerouting = 1
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

        Public Sub ReturnRWOA(ByVal statusIn As Integer)

            Dim rwoaDao As IRwoaDao = New NHibernateDaoFactory().GetRwoaDao

            Dim rwRec = rwoaDao.GetRecentRWOA(reinvestigationRequest.Workflow, reinvestigationRequest.Id)

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

        Public Sub ReturnTo(ByVal statusIn As Integer, ByVal statusOut As Integer)
            Dim statusCodeIn As Integer = WorkstatusDao.GetById(statusIn).StatusCodeType.Id
            Dim statusCodeOut As Integer = WorkstatusDao.GetById(statusOut).StatusCodeType.Id

            Dim returnDao As IReturnDao = New NHibernateDaoFactory().GetReturnDao

            Dim oldReturn = returnDao.GetRecentReturn(reinvestigationRequest.Workflow, reinvestigationRequest.Id)
            If (oldReturn IsNot Nothing AndAlso oldReturn.DateSentBack Is Nothing) Then
                ReturnBack(statusIn, statusOut, True)
            End If

            If GetStatusInfo(statusCodeOut).Item1 <> 0 Then
                reinvestigationRequest.ReturnByGroup = SESSION_GROUP_ID
                reinvestigationRequest.ReturnToGroup = GetStatusInfo(statusCodeOut).Item1
            End If

            Dim rwRec As New ALOD.Core.Domain.Modules.Lod.Return
            rwRec.RefId = reinvestigationRequest.Id
            rwRec.Workflow = reinvestigationRequest.Workflow
            rwRec.WorkStatusTo = statusOut
            rwRec.WorkStatusFrom = statusIn
            rwRec.SentTo = GetStatusInfo(statusCodeOut).Item1
            rwRec.DateSent = DateTime.Now
            rwRec.Sender = SESSION_GROUP_ID
            rwRec.ExplantionSendingBack = Server.HtmlEncode(CommentsTextBox.Text)
            rwRec.CreatedDate = DateTime.Now
            rwRec.CreatedBy = SESSION_USER_ID

            If (boardStages.Contains(statusCodeOut) AndAlso boardStages.Contains(statusCodeIn)) Then
                rwRec.BoardReturn = 1
            Else
                rwRec.BoardReturn = 0
            End If

            If (Not String.IsNullOrEmpty(rblReason.SelectedValue)) Then
                rwRec.ReasonSentBack = rblReason.SelectedValue
            End If

            returnDao.Save(rwRec)

        End Sub

        '''<summary>
        '''<case status="LodStatus.BoardReview">
        '''From the aa the case is always returned to board for completion.
        '''so If the aa  findings are present then  the case is considered to be closed by
        ''' approving authority.If the case is completed by board without going to approving authority
        '''the case is considered to be closed by board and a record is created for approving authority
        ''' A flag BoardforGeneral is set to "Y".
        ''' When the case is overwritten to board status and board complets it again the exiting approving
        ''' authority findings are also overwritten however if the case was closed by approving authority then
        ''' the case has to be overwritten to be approving authority status .Overwriting to board status will have no effect
        '''</case>
        ''' </summary>
        ''' <param name="statusIn"></param>
        ''' <param name="statusOut"></param>
        ''' <remarks></remarks>
        Public Sub SetFinalDecision(ByVal statusIn As Integer, ByVal statusOut As Integer)
            Select Case statusIn
                Case ReinvestigationRequestWorkStatus.WingCCRequestReview
                    If (HasWingCCApprovedRequest()) Then
                        GenerateDeterminationMemo(PersonnelTypes.APPOINT_AUTH)
                        CopyRRDocumentsToLOD()
                    End If

                Case ReinvestigationRequestWorkStatus.ApprovingAuthorityRequestAction
                    GenerateDeterminationMemo(PersonnelTypes.BOARD_AA)

                Case ReinvestigationRequestWorkStatus.BoardTechRequestFinalReview
                    CopyRRDocumentsToLOD()
            End Select
        End Sub

        Protected Sub CopyRRDocumentsToLOD()
            DocumentDao.CopyGroupDocuments(reinvestigationRequest.DocumentGroupId, LOD.DocumentGroupId, DocumentType.MemberReinvestigationRequest, DocumentType.Miscellaneous)
            DocumentDao.CopyGroupDocuments(reinvestigationRequest.DocumentGroupId, LOD.DocumentGroupId, DocumentType.ReinvestigationSupportingDocs, DocumentType.Miscellaneous)
        End Sub

        Protected Sub GenerateDeterminationMemo(pType As PersonnelTypes)
            Dim memo As Memorandum2 = reinvestigationRequest.GetDeterminationMemorandum(GetDeterminationMemoArgs(pType))

            If (memo Is Nothing) Then
                LogManager.LogError("Failed to generate determination memo for Reinvestigation Request case " & reinvestigationRequest.CaseId)
                Exit Sub
            End If

            MemoDao.SaveOrUpdate(memo)

            LogManager.LogAction(ModuleType, UserAction.MemoAdded, reinvestigationRequest.Id, "Generated " & memo.Template.Title)
        End Sub

        Protected Function GetDeterminationMemoArgs(pType As PersonnelTypes) As RRDeterminationMemoArgs
            Dim args = New RRDeterminationMemoArgs()

            args.PType = pType
            args.UserGeneratingMemo = UserService.CurrentUser()
            args.DaoFactory = DaoFactory
            args.SigService = New DBSignService(DBSignTemplateId.Form348RRFindings, RequestId, pType)

            Return args
        End Function

        Protected Function HasWingCCApprovedRequest()
            If (reinvestigationRequest.DoesFindingExist(PersonnelTypes.APPOINT_AUTH) AndAlso reinvestigationRequest.FindFindingByPersonnelType(PersonnelTypes.APPOINT_AUTH).Finding = Finding.Approve) Then
                Return True
            End If

            Return False
        End Function

        Private Sub AddApprovalAuthoritySignature()
            Dim user As AppUser = UserService.CurrentUser()
            Dim sig As String = String.Empty
            Dim title As String = String.Empty

            Dim strSig As String = "USAF"
            If SESSION_COMPO = "5" Then
                strSig = "ANG"
            End If

            If (Not String.IsNullOrEmpty(user.AlternateSignatureName)) Then
                sig = user.AlternateSignatureName & ", " & strSig
            Else
                sig = user.SignatureName & ", " & strSig
            End If

            title = UserService.GetUserAlternateTitle(user.Id, UserGroups.BoardApprovalAuthority)

            reinvestigationRequest.AddSignature(DaoFactory, UserGroups.BoardApprovalAuthority, title, user)
        End Sub

        Private Sub AddSignature(ByVal groupId As Integer)
            Dim user As AppUser = UserService.CurrentUser()
            reinvestigationRequest.AddSignature(DaoFactory, groupId, user.SignatureTitle, user)
        End Sub

#End Region

    End Class

End Namespace