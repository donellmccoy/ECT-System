Imports ALOD.Core.Domain.Common
Imports ALOD.Core.Domain.Common.CustomServerControls
Imports ALOD.Core.Domain.DBSign
Imports ALOD.Core.Domain.Documents
Imports ALOD.Core.Domain.Lookup
Imports ALOD.Core.Domain.Modules.Lod
Imports ALOD.Core.Domain.Modules.SARC
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
Imports ALODWebUtility.Printing
Imports ALODWebUtility.Worklfow

Namespace Web.SARC

    Partial Class NextAction
        Inherits System.Web.UI.Page

#Region "Fields"

        Private Const VIEWSTATE_VALIDATIONS As String = "VALIDATION_ITEMS"
        Private _dao As ISARCDAO
        Private _daoFactory As IDaoFactory
        Private _docCatViewDao As IDocCategoryViewDao
        Private _docDao As IDocumentDao
        Private _lookupDao As ILookupDao
        Private _memoDao As IMemoDao2
        Private _sarc As RestrictedSARC
        Private _scAccess As Dictionary(Of String, ALOD.Core.Domain.Workflow.PageAccessType)
        Private _usergroupDao As IUserGroupDao
        Private _workstatusDao As IWorkStatusDao

        Private boardStages As Short() = New Short() {
            CShort(SARCRestrictedStatusCode.SARCAdminReview),
            CShort(SARCRestrictedStatusCode.SARCBoardMedicalReview),
            CShort(SARCRestrictedStatusCode.SARCSeniorMedicalReview),
            CShort(SARCRestrictedStatusCode.SARCBoardJAReview),
            CShort(SARCRestrictedStatusCode.SARCApprovingAuthorityReview),
            CShort(SARCRestrictedStatusCode.SARCBoardAdminReview)
        }

        Private CANCELLATION_REASON As String = "Case Cancelled By "
        Private errors As New List(Of ValidationItem)
        Private separatorTemplate As System.Web.UI.CompiledTemplateBuilder

        Private unitStages As Short() = New Short() {
            CShort(SARCRestrictedStatusCode.SARCInitiate)}

#End Region

#Region "SARCProperty"

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

        ReadOnly Property SARCDao() As ISARCDAO
            Get
                If (_dao Is Nothing) Then
                    _dao = DaoFactory.GetSARCDao()
                End If

                Return _dao
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

        Protected ReadOnly Property MasterPage() As SARCMaster
            Get
                Dim master As SARCMaster = CType(Page.Master, SARCMaster)
                Return master
            End Get
        End Property

        Protected ReadOnly Property ModuleType() As ModuleType
            Get
                Return ModuleType.SARC
            End Get
        End Property

        Protected ReadOnly Property SARCCase As RestrictedSARC
            Get
                If (_sarc Is Nothing) Then
                    _sarc = SARCDao.GetById(RefId)
                End If

                Return _sarc
            End Get
        End Property

        Protected ReadOnly Property SectionList() As Dictionary(Of String, ALOD.Core.Domain.Workflow.PageAccessType)
            Get
                If (_scAccess Is Nothing) Then
                    _scAccess = SARCCase.ReadSectionList(SESSION_GROUP_ID)
                End If
                Return _scAccess
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
            SARCDao.CommitChanges()
            SARCDao.Evict(SARCCase)
            _sarc = Nothing
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

                SARCCase.ModifiedBy = SESSION_USER_ID
                SARCCase.ModifiedDate = DateTime.Now
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

            SARCCase.Cancel_Explanation = Server.HtmlEncode(CommentsTextBox.Text.Trim)

            If rblReason.SelectedValue <> "" Then
                SARCCase.Cancel_Reason = CShort(rblReason.SelectedValue)
            End If

            SARCCase.Cancel_Date = DateTime.Now

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
                Case SARCRestrictedStatusCode.SARCInitiate
                    sentTo = "Wing SARC"
                    sentToId = UserGroups.WingSarc
                Case SARCRestrictedStatusCode.SARCAdminReview
                    sentTo = "SARC Admin"
                    sentToId = UserGroups.SARCAdmin
                Case SARCRestrictedStatusCode.SARCBoardMedicalReview
                    sentTo = "Board Medical"
                    sentToId = UserGroups.BoardMedical
                Case SARCRestrictedStatusCode.SARCSeniorMedicalReview
                    sentTo = "Senior Medical Review"
                    sentToId = UserGroups.SeniorMedicalReviewer
                Case SARCRestrictedStatusCode.SARCBoardJAReview
                    sentTo = "Board Legal"
                    sentToId = UserGroups.BoardLegal
                Case SARCRestrictedStatusCode.SARCApprovingAuthorityReview
                    sentTo = "Approving Authority"
                    sentToId = UserGroups.BoardApprovalAuthority
                Case SARCRestrictedStatusCode.SARCBoardAdminReview
                    sentTo = "Board Admin"
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
            Dim finding As RestrictedSARCFindings = SARCCase.FindByType(pType)

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
            rblReason.DataSource = LookupService.GetWorkflowCancelReasons(SARCCase.Workflow, False)
            rblReason.DataTextField = "Description"
            rblReason.DataValueField = "Id"
            rblReason.DataBind()
        End Sub

        Protected Sub PopulateRWOArbl()
            NextActionHelpers.PopulateRadioButtonListWithRwoaOptions(rblReason, SARCCase.Workflow, LookupDao)
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

                ChangeStatus(e.OptionId, e.StatusOut, e.Text)
                SetFeedbackMessage("Case " + SARCCase.CaseId + " successfully signed.  Action applied: " + e.Text)
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

            If (workOption.DisplayText Like "*Cancel*") Then
                If (Not ValidateReasonControls("Cancellation")) Then
                    Exit Sub
                End If

                SaveCancelData()
            End If

            If (workOption.DisplayText Like "RWOA*") OrElse (workOption.DisplayText Like "RFA*") Then
                If (Not ValidateReasonControls("RWOA")) Then
                    Exit Sub
                End If
            End If

            ReasonPanel.CssClass = ""

            If Not IsNothing(SARCCase.ReturnToGroup) Then  'Clear the Return Group info when the recipient Replies/Forwards
                If SESSION_GROUP_ID = SARCCase.ReturnToGroup Then
                    SARCCase.ReturnToGroup = Nothing
                    SARCCase.ReturnByGroup = Nothing
                End If
            End If

            Dim template As Integer = workOption.Template

            If (template = DBSignTemplateId.Form348SARCFindings AndAlso Not MemberHasFindings()) Then
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
            SigBlock.StartSignature(RefId, SARCCase.Workflow, secId, LogActionDisplayText, SARCCase.Status, workOption.wsStatusOut, workOption.Id, template, String.Empty)

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

            If (Not IsNothing(SARCCase.Cancel_Reason)) Then  'Check if Cancelled - show Cancel Reason
                If SARCCase.Cancel_Reason > 0 Then
                    InitCancelComments(SARCCase.Cancel_Reason, SARCCase.Cancel_Explanation)

                    CommentPanel.Visible = True
                End If
            End If

        End Sub

        Private Sub InitNextActionOptions()

            Dim options = From o In SARCCase.GetCurrentOptions(WorkFlowService.GetLastStatus(SARCCase.Id, ModuleType), DaoFactory)
                          Select o
                          Order By o.SortOrder

            rbtOption.DataSource = options
            rbtOption.DataBind()

            ViewState(VIEWSTATE_VALIDATIONS) = SARCCase.Validations

        End Sub

        Private Sub ShowRLB()

            Dim access As ALOD.Core.Domain.Workflow.PageAccessType = SectionList(SARCSectionNames.SARC_RLB.ToString())
            Dim ws As IList(Of WorkStatusTracking) = WorkFlowService.GetWorkStatusTracking(SARCCase.Id, ModuleType)

            If access <> ALOD.Core.Domain.Workflow.PageAccessType.None Then

                If (ws IsNot Nothing AndAlso ws.Count > 1) Then
                    If (IsReturnedToBoard(SARCCase.CurrentStatusCode) OrElse (IsReturnedToUnit(ws(0).WorkflowStatus.StatusCodeType.Id, SARCCase.CurrentStatusCode))) Then
                        'get the last status from workflowtransaction class
                        RLBVisible = True
                        RLBTitle.Text = "Returned Without Action"

                        Dim rwoaDao As IRwoaDao = DaoFactory.GetRwoaDao()
                        ucRLB.Initialize(rwoaDao.GetRecentRWOA(SARCCase.Workflow, SARCCase.Id))
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

            If (statusCodeOut = SARCRestrictedStatusCode.SARCComplete OrElse statusCodeOut = SARCRestrictedStatusCode.SARCCancelled) Then
                toList = emailService.GetDistributionListByGroup(SARCCase.Id, action.Target, "SARC")
            Else
                toList = emailService.GetDistributionListByWorkflow(SARCCase.Id, statusOut, SARCCase.Workflow, action.Target)
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
                        SARCCase.RemoveSignature(DaoFactory, statusOut)
                    Case WorkflowActionType.SendEmail
                        AddEmails(emails, mailService, actn, statusOut)
                End Select
            Next

            emails.SetField("CASE_NUMBER", SARCCase.CaseId)
            emails.SetField("APP_LINK", GetHostName())
            emails.SendAll()
        End Sub

        Public Sub ChangeStatus(optionId As Integer, newStatus As Integer, text As String)
            Dim statusIn As Integer = SARCCase.Status
            Dim newStatusCode As Integer = WorkstatusDao.GetById(newStatus).StatusCodeType.Id

            If (newStatus <> SARCCase.Status) Then
                LogManager.LogAction(ModuleType, UserAction.Signed, SARCCase.Id, Server.HtmlEncode(CommentsTextBox.Text.Trim), statusIn, newStatus)
                LogManager.LogAction(ModuleType, UserAction.StatusChanged, SARCCase.Id, text, statusIn, newStatus)

                ApplyActions(optionId, statusIn, newStatus)

                SARCCase.Status = newStatus
                SARCCase.ModifiedBy = SESSION_USER_ID
                SARCCase.ModifiedDate = DateTime.Now()

                SARCDao.SaveOrUpdate(SARCCase)
                CommitChanges()

                If (newStatusCode = SARCRestrictedStatusCode.SARCComplete OrElse newStatusCode = SARCRestrictedStatusCode.SARCCancelled) Then
                    SaveFinalPDFForm()
                    SARCCase.UpdateIsPostProcessingComplete(DaoFactory)
                    SARCDao.SaveOrUpdate(SARCCase)
                    CommitChanges()
                End If

                Dim reminderDao = New ReminderEmailsDao()
                reminderDao.ReminderEmailUpdateStatusChange(statusIn, newStatus, SARCCase.CaseId, "SARC")
            End If
        End Sub

        Public Sub InsertRWOA(ByVal statusIn As Integer, ByVal statusOut As Integer)
            Dim statusCodeIn As Integer = WorkstatusDao.GetById(statusIn).StatusCodeType.Id
            Dim statusCodeOut As Integer = WorkstatusDao.GetById(statusOut).StatusCodeType.Id

            Dim rwoaDao As IRwoaDao = DaoFactory.GetRwoaDao()

            If (IsReturnedToUnit(statusCodeIn, statusCodeOut)) Then

                If GetSentToInfo(statusCodeOut).Item1 <> 0 Then
                    SARCCase.ReturnByGroup = SESSION_GROUP_ID
                    SARCCase.ReturnToGroup = GetSentToInfo(statusCodeOut).Item1
                End If

                Dim rwRec As New ALOD.Core.Domain.Modules.Lod.Rwoa
                rwRec.RefId = SARCCase.Id
                rwRec.Workflow = SARCCase.Workflow
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
                Dim ws As IList(Of WorkStatusTracking) = WorkFlowService.GetWorkStatusTracking(SARCCase.Id, ModuleType)
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

            Dim rwRec = rwoaDao.GetRecentRWOA(SARCCase.Workflow, SARCCase.Id)

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

        Protected Sub SaveFinalPDFForm()
            If (Not SARCCase.DocumentGroupId.HasValue) Then
                Exit Sub
            End If

            Dim pdfFactory As PDFCreateFactory = New PDFCreateFactory()
            Dim doc As PDFDocument = pdfFactory.GeneratePdf(SARCCase.Id, ModuleType)
            Dim groupID As Long = SARCCase.DocumentGroupId
            Dim fileName As String = "Form348-R_Case:" & SARCCase.CaseId.ToString() & "-Generated.pdf"
            Dim docId As Long = 0

            Dim docMetaData As Document = New Document()
            docMetaData.DateAdded = DateTime.Now
            docMetaData.DocDate = DateTime.Now
            docMetaData.Description = "Form348-R Generated for Case Id: " & SARCCase.CaseId.ToString()
            docMetaData.DocStatus = DocumentStatus.Approved
            docMetaData.Extension = "pdf"
            docMetaData.SSN = SARCCase.DocumentEntityId
            docMetaData.DocType = DocumentType.Form348R

            doc.Render(fileName)
            docId = DocumentDao.AddDocument(doc.GetBuffer(), fileName, groupID, docMetaData)

            LogManager.LogAction(ModuleType, UserAction.AddedDocument, SARCCase.Id, "Generated Form348-R PDF with Document Id = " & docId.ToString())
        End Sub

        Private Sub AddSignature(ByVal groupId As Integer)
            Dim user As AppUser = UserService.CurrentUser()
            SARCCase.AddSignature(DaoFactory, groupId, user.SignatureTitle, user)
        End Sub

#End Region

    End Class

End Namespace