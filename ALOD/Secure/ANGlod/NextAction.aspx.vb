Imports ALOD.Core.Domain.Common
Imports ALOD.Core.Domain.Common.CustomServerControls
Imports ALOD.Core.Domain.DBSign
Imports ALOD.Core.Domain.Documents
Imports ALOD.Core.Domain.Lookup
Imports ALOD.Core.Domain.Modules.Lod
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

Namespace Web.LOD
    Partial Class Secure_lod_NextAction
        Inherits System.Web.UI.Page

        Private _daoFactory As IDaoFactory
        Private _dao As ILineOfDutyDao
        Private _docDao As IDocumentDao
        Private _docCatViewDao As IDocCategoryViewDao
        Private _memoDao As IMemoDao
        Private _lookupDao As ILookupDao
        Private _usergroupDao As IUserGroupDao
        Private _lod As LineOfDuty
        Private _lod_v2 As LineOfDuty_v2
        Private _workstatusDao As IWorkStatusDao
        Private _sigDao As ISignatueMetaDateDao
        Private _scAccess As Dictionary(Of String, ALOD.Core.Domain.Workflow.PageAccessType)

        Private separatorTemplate As System.Web.UI.CompiledTemplateBuilder

        Private Const VIEWSTATE_VALIDATIONS As String = "VALIDATION_ITEMS"
        Private CANCELLATION_REASON As String = "Case Cancelled By "
        Private errors As New List(Of ValidationItem)

        Private unitStages As Short() = New Short() {
            CShort(LodStatusCode.MedTechReview),
            CShort(LodStatusCode.WingSarcInput),
            CShort(LodStatusCode.MedicalOfficerReview),
            CShort(LodStatusCode.UnitCommanderReview),
            CShort(LodStatusCode.AppointingAutorityReview),
            CShort(LodStatusCode.WingJAReview),
            CShort(LodStatusCode.FormalActionByAppointingAuthority),
            CShort(LodStatusCode.FormalActionByWingJA),
            CShort(LodStatusCode.NotifyFormalInvestigator),
            CShort(LodStatusCode.FormalInvestigation)}

        Private boardStages As Short() = New Short() {
            CShort(LodStatusCode.BoardReview),
            CShort(LodStatusCode.FormalBoardReview)
            }

#Region "LODProperty"
        Protected ReadOnly Property DaoFactory As IDaoFactory
            Get
                If (_daoFactory Is Nothing) Then
                    _daoFactory = New NHibernateDaoFactory()
                End If

                Return _daoFactory
            End Get
        End Property

        ReadOnly Property LODDao() As ILineOfDutyDao
            Get
                If (_dao Is Nothing) Then
                    _dao = DaoFactory.GetLineOfDutyDao()
                End If

                Return _dao
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

        ReadOnly Property DocCatViewDao() As IDocCategoryViewDao
            Get
                If (_docCatViewDao Is Nothing) Then
                    _docCatViewDao = DaoFactory.GetDocCategoryViewDao()
                End If

                Return _docCatViewDao
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

        ReadOnly Property LookupDao() As ILookupDao
            Get
                If (_lookupDao Is Nothing) Then
                    _lookupDao = DaoFactory.GetLookupDao()
                End If

                Return _lookupDao
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

        ReadOnly Property SigDao() As SignatureMetaDataDao
            Get
                If (_sigDao Is Nothing) Then
                    _sigDao = DaoFactory.GetSigMetaDataDao()
                End If

                Return _sigDao
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

        Protected ReadOnly Property LOD_v2() As LineOfDuty_v2
            Get
                If (_lod_v2 Is Nothing) Then
                    _lod_v2 = CType(LOD, LineOfDuty_v2)

                End If
                Return _lod_v2
            End Get
        End Property

        Public ReadOnly Property refId() As Integer
            Get
                Return Integer.Parse(Request.QueryString("refId"))
            End Get
        End Property

        Protected ReadOnly Property SectionList() As Dictionary(Of String, ALOD.Core.Domain.Workflow.PageAccessType)
            Get
                If (_scAccess Is Nothing) Then
                    _scAccess = LOD.ReadSectionList(SESSION_GROUP_ID)
                End If
                Return _scAccess
            End Get
        End Property

        Protected ReadOnly Property MasterPage() As LodMaster
            Get
                Dim master As LodMaster = CType(Page.Master, LodMaster)
                Return master
            End Get
        End Property


        Protected ReadOnly Property CalendarImage() As String
            Get
                Return Me.ResolveClientUrl("~/App_Themes/" + Page.Theme + "/Images/Calendar.gif")
            End Get
        End Property

        Public ReadOnly Property Navigator() As TabNavigator
            Get
                Return MasterPage.Navigator
            End Get
        End Property

        Public Sub CommitChanges()
            LODDao.CommitChanges()
            LODDao.Evict(_lod)
            _lod = Nothing
        End Sub

        Protected ReadOnly Property ModuleType() As ModuleType
            Get
                Return ModuleType.LOD
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

        Protected Sub Page_PreRender(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.PreRender
            'we have two sources of errors.  
            'those generated by the LOD itself and any errors that occured on this page
            'so we combine the two here for display

            For Each item As ValidationItem In ViewState(VIEWSTATE_VALIDATIONS)
                errors.Add(item)
            Next

            If (LOD.Workflow = 27 AndAlso LOD.WorkflowStatus.StatusCodeType.Id = LodStatusCode.MedTechReview) Then
                Dim unitdao As ILineOfDutyUnitDao = DaoFactory.GetLineOfDutyUnitDao()
                Dim unit As LineOfDutyUnit_v2 = unitdao.GetById(refId, False)

                If (Not unit.DutyFrom.HasValue) Then

                    errors.Add(New ValidationItem("Medical", "ComponentFromDate_v2, ComponentFromTime_v2", "Member component from date is required"))

                End If
                If (Not unit.DutyTo.HasValue) Then

                    errors.Add(New ValidationItem("Medical", "ComponentToDate_v2, ComponentToTime_v2", "Member component to date is required"))

                End If
            End If

            results.DataSource = errors

        End Sub



        Protected Sub Page_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
            AddHandler Me.Master.TabClick, AddressOf TabButtonClicked
        End Sub


        Private Sub TabButtonClicked(ByRef sender As Object, ByRef e As TabNavigationEventArgs)

            If (e.ButtonType = NavigatorButtonType.Save OrElse e.ButtonType = NavigatorButtonType.NavigatedAway) Then

                LOD.ModifiedBy = SESSION_USER_ID
                LOD.ModifiedDate = DateTime.Now
            End If
            If (e.ButtonType = NavigatorButtonType.Save) Then
                Response.Redirect(Request.RawUrl)
            End If
        End Sub

#End Region

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

            Dim eventTarget As String
            If (Not Page.IsPostBack) Then

                UserCanEdit = GetAccessLOD(Navigator.PageAccess, True, LOD)
                InitControls()
                InitData()
                ShowRLB()
                InitNextActionOptions()

                LogManager.LogAction(ModuleType, UserAction.ViewPage, refId, "Viewed Page: Next Action")

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

        Private Sub InitControls()

            SetInputFormatRestriction(Page, CommentsTextBox, FormatRestriction.AlphaNumeric, PERMITTED_SPECIAL_CHAR_INPUT)

        End Sub

        Private Sub InitData()

            If (refId = 0) Then
                Exit Sub
            End If

            'show the comments on the cancel/return (if there are any)
            CommentPanel.Visible = False
            CommentLabel.Text = ""

            Dim isCancel As Boolean = False

            ' Informal - Medical Officer Cancel
            If (Not IsNothing(LOD.LODMedical.PhysicianCancelReason)) Then  'Check if Cancelled - show Cancel Reason
                If LOD.LODMedical.PhysicianCancelReason > 0 Then
                    isCancel = True
                    InitCancelComments(LOD.LODMedical.PhysicianCancelReason, LOD.LODMedical.PhysicianCancelExplanation, False)
                End If
            End If

            ' Formal Cancel
            If (LOD.Formal AndAlso LOD.IsFormalCancelRecommended) Then
                If (LOD.AppointingCancelReasonId.HasValue AndAlso LOD.AppointingCancelReasonId > 0) Then    ' Appointing Authority Cancel
                    isCancel = True
                    InitCancelComments(LOD.AppointingCancelReasonId, LOD.AppointingCancelExplanation, False)
                ElseIf (LOD.ApprovingCancelReasonId.HasValue AndAlso LOD.ApprovingCancelReasonId > 0) Then  ' Approving Authority Cancel Recommendation
                    isCancel = True
                    InitCancelComments(LOD.ApprovingCancelReasonId, LOD.ApprovingCancelExplanation, True)
                End If
            End If

            If (Not String.IsNullOrEmpty(LOD.ReturnComment) And (Not isCancel)) Then  'Check if Returned - show Return Comment
                CommentLabel.Text = NullStringToEmptyString(LOD.ReturnComment).Replace(Environment.NewLine, "<br />")
            End If

            If (LOD.CurrentStatusCode = LodStatusCode.AppointingAutorityReview And (Not String.IsNullOrEmpty(LOD.BoardTechComments)) And (Not isCancel)) Then
                CommentLabel.Text = CommentLabel.Text & Environment.NewLine & NullStringToEmptyString(LOD.BoardTechComments).Replace(Environment.NewLine, "<br />")
                CommentPanel.Visible = True
            End If

            If Len(CommentLabel.Text) > 0 Then
                If (isCancel) Then
                    CommentPanel.Visible = True
                End If

                If Not IsNothing(LOD.ReturnToGroup) Then
                    If SESSION_GROUP_ID = LOD.ReturnToGroup Then
                        CommentPanel.Visible = True
                    End If
                End If
                If Not IsNothing(LOD.ReturnByGroup) Then
                    If SESSION_GROUP_ID = LOD.ReturnByGroup Then
                        CommentPanel.Visible = True
                    End If
                End If
            End If

        End Sub

        Private Sub InitCancelComments(ByVal reason As Integer, ByVal explanation As String, ByVal isRecommended As Boolean)
            Dim title As String = "Case Cancelled: "

            If (isRecommended) Then
                title = "Cancel Case Recommended by Approving Authority: "
            End If

            PopulateCancelrbl()
            rblReason.SelectedValue = reason
            CommentLabel.Text = title & rblReason.SelectedItem.Text & "<br /><br />"

            CommentLabel.Text = CommentLabel.Text & NullStringToEmptyString(explanation).Replace(Environment.NewLine, "<br />")
        End Sub

        Private Sub InitNextActionOptions()
            'as part of this method call the validation errors are generated
            'so we will store them from here for later display
            Dim options = From o In LOD.GetCurrentOptions(WorkFlowService.GetLastStatus(LOD.Id, ModuleType), DaoFactory, SESSION_GROUP_ID)
                          Select o
                          Order By o.SortOrder
            If (LOD.Workflow = AFRCWorkflows.LOD_v2 AndAlso ConfigurationManager.AppSettings("UseDBSign") = "Y") Then
                For Each o In options
                    Dim isShown As Boolean = o.OptionVisible
                    For Each r In o.RuleList
                        If (r.RuleTypes.ruleTypeId = RuleKind.Visibility) Then
                            If (r.RuleTypes.Name.ToLower().Equals("checksignature")) Then
                                Dim data As IList(Of String)
                                data = r.RuleValue.Split(",")

                                Dim sig As Boolean = False
                                Dim signatureService As DBSignService
                                Dim template As DBSignTemplateId = Nothing
                                Dim ptype As PersonnelTypes = Nothing

                                Dim currentStatus As Integer = LOD.CurrentStatusCode
                                Dim sigMetaData As SignatureMetaData = Nothing



                                Select Case currentStatus
                                    Case LodStatusCode.MedTechReview, LodStatusCode.MedicalOfficerReview
                                        If (Not SigDao.GetByWorkStatus(LOD.Id, LOD.Workflow, LodWorkStatus_v2.MedicalOfficerReview) Is Nothing) Then
                                            template = DBSignTemplateId.Form348Medical
                                            ptype = PersonnelTypes.MED_OFF
                                        End If
                                    Case LodStatusCode.UnitCommanderReview
                                        If (Not SigDao.GetByWorkStatus(LOD.Id, LOD.Workflow, LodWorkStatus_v2.UnitCommanderReview) Is Nothing) Then
                                            template = DBSignTemplateId.Form348Unit
                                            ptype = PersonnelTypes.UNIT_CMDR
                                        End If
                                    Case LodStatusCode.WingJAReview
                                        If (Not SigDao.GetByWorkStatus(LOD.Id, LOD.Workflow, LodWorkStatus_v2.WingJAReview) Is Nothing) Then
                                            template = DBSignTemplateId.Form348Findings
                                            ptype = PersonnelTypes.WING_JA
                                        End If
                                    Case LodStatusCode.FormalActionByWingJA
                                        If (Not SigDao.GetByWorkStatus(LOD.Id, LOD.Workflow, LodWorkStatus_v2.FormalActionByWingJA) Is Nothing) Then
                                            template = DBSignTemplateId.Form348Findings
                                            ptype = PersonnelTypes.FORMAL_WING_JA
                                        End If
                                    Case LodStatusCode.AppointingAutorityReview
                                        If (Not SigDao.GetByWorkStatus(LOD.Id, LOD.Workflow, LodWorkStatus_v2.AppointingAutorityReview) Is Nothing) Then
                                            template = DBSignTemplateId.WingCC
                                            ptype = PersonnelTypes.APPOINT_AUTH
                                        End If
                                    Case LodStatusCode.FormalActionByAppointingAuthority
                                        If (Not SigDao.GetByWorkStatus(LOD.Id, LOD.Workflow, LodWorkStatus_v2.FormalActionByAppointingAuthority) Is Nothing) Then
                                            template = DBSignTemplateId.WingCC
                                            ptype = PersonnelTypes.FORMAL_BOARD_AA
                                        End If

                                End Select

                                If (Not template = Nothing AndAlso Not ptype = Nothing) Then
                                    signatureService = New DBSignService(template, LOD.Id, ptype)
                                    Dim signatureStatus As DBSignResult = signatureService.VerifySignature()
                                    If (Not (signatureStatus = DBSignResult.SignatureValid And data(1).Trim().Equals("True")) And Not (signatureStatus = DBSignResult.SignatureInvalid And data(1).Trim().Equals("False"))) Then
                                        o.OptionVisible = False
                                    End If
                                ElseIf (data(1).Trim().Equals("True")) Then
                                    o.OptionVisible = False
                                End If

                            End If
                        End If
                    Next
                Next
            End If

            rbtOption.DataSource = options
            rbtOption.DataBind()


            ViewState(VIEWSTATE_VALIDATIONS) = LOD.Validations
        End Sub

        Private Sub ShowRLB()
            Dim access As ALOD.Core.Domain.Workflow.PageAccessType = SectionList(SectionNames.RLB.ToString())
            Dim ws As IList(Of WorkStatusTracking) = WorkFlowService.GetWorkStatusTracking(LOD.Id, ModuleType)

            If access <> ALOD.Core.Domain.Workflow.PageAccessType.None Then

                If (ws IsNot Nothing AndAlso ws.Count > 1) Then
                    If (IsReturnedToBoard(LOD.CurrentStatusCode) OrElse IsReturnedToUnit(ws(0).WorkflowStatus.StatusCodeType.Id, LOD.CurrentStatusCode)) Then 'get the last status from workflowtransaction class
                        RLBVisible = True
                        RLBTitle.Text = "Returned Without Action"

                        Dim rwoaDao As IRwoaDao = DaoFactory.GetRwoaDao()
                        If (LOD.Workflow = AFRCWorkflows.LOD) Then
                            ucRLB.Initialize(LOD)
                        Else
                            ucRLB.Initialize(rwoaDao.GetRecentRWOA(LOD.Workflow, LOD.Id))
                        End If

                        If access <> ALOD.Core.Domain.Workflow.PageAccessType.ReadWrite Then
                            RLBEnabled = False
                        End If
                        Exit Sub
                    End If

                End If
            End If

            If (ws IsNot Nothing AndAlso ws.Count > 1) Then
                If (LOD.Workflow = AFRCWorkflows.LOD_v2) Then

                    If (CheckReturn(ws(0).WorkflowStatus.StatusCodeType.Id, LOD.CurrentStatusCode)) Then

                        Dim returnDao As IReturnDao = DaoFactory.GetReturnDao()

                        Dim rwRec = returnDao.GetRecentReturn(LOD.Workflow, LOD.Id)

                        If (rwRec IsNot Nothing) AndAlso (rwRec.WorkStatusFrom = LOD.WorkflowStatus.Id Or rwRec.WorkStatusTo = LOD.WorkflowStatus.Id) Then
                            RLBVisible = True
                            RLBTitle.Text = "Returned"

                            ucRLB.Initialize(returnDao.GetRecentReturn(LOD.Workflow, LOD.Id))

                            If rwRec.WorkStatusTo <> LOD.WorkflowStatus.Id Then
                                RLBEnabled = False
                            End If
                        End If
                    End If
                End If
            End If
        End Sub


        Public Sub SaveRLB()
            'Save the rwoa reply comments
            Dim access As ALOD.Core.Domain.Workflow.PageAccessType = SectionList(SectionNames.RLB.ToString())

            If access = ALOD.Core.Domain.Workflow.PageAccessType.ReadWrite Then
                Dim lastWorkStatus As Integer = WorkFlowService.GetLastStatus(LOD.Id, ModuleType)

                If (lastWorkStatus <> 0 AndAlso boardStages.Contains(WorkstatusDao.GetById(lastWorkStatus).StatusCodeType.Id) AndAlso unitStages.Contains(LOD.CurrentStatusCode)) Then
                    LOD.MedTechComments = ucRLB.MedTechComments ' automatically HtmlEncoded by the user control
                End If
            End If
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

        Protected Function MemberHasFindings() As Boolean
            Dim pType As Integer = CInt(GetPersonnelTypeFromGroup(SESSION_GROUP_ID, LOD.Formal))
            Dim finding As LineOfDutyFindings = LOD.FindByType(pType)

            If (finding Is Nothing) Then
                Return False
            End If

            Return True

        End Function

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

            ' Was a cancel option selected?
            ' Currently only Medical Officers and Appointing Authorities can cancel cases (informal and formal respectively)
            ' Approving Authorities can recommend a formal case be cancelled

            If (workOption.DisplayText Like "*Cancel*") Then
                'the user is cancelling, do we have a cancel reason?
                If (Not ValidateReasonControls("Cancellation")) Then
                    Exit Sub
                End If

                ' if cancel recommendation
                SaveCancelRecommendation()
            End If

            If (LOD.Workflow = AFRCWorkflows.LOD_v2) Then
                If (workOption.DisplayText.Equals("Notify Formal Investigator")) Then
                    LOD_v2.DirectReply = False
                End If
            End If


            If (workOption.DisplayText Like "RWOA*") Then
                If (Not ValidateReasonControls("RWOA")) Then
                    Exit Sub
                End If

                If (workOption.DisplayText Like "RWOA for reply*") Then
                    LOD_v2.DirectReply = True
                End If

                If (workOption.DisplayText Like "RWOA for rerouting*") Then
                    LOD_v2.DirectReply = False
                End If

            End If

            If (workOption.DisplayText Like "Direct Formal Investigation*") Then
                If (CommentsTextBox.Text.Length > 0) Then
                    LOD.BoardTechComments = Server.HtmlEncode(CommentsTextBox.Text.Trim)
                End If
            End If

            If Not IsNothing(LOD.ReturnToGroup) Then  'Clear the Return Group info when the recipient Replies/Forwards
                If SESSION_GROUP_ID = LOD.ReturnToGroup Then
                    LOD.ReturnToGroup = Nothing
                    LOD.ReturnByGroup = Nothing
                End If
            End If


            LOD.ReturnComment = ""
            If (workOption.DisplayText Like "Return*") Then


                If (LOD.Workflow = AFRCWorkflows.LOD_v2) Then

                    If (Not ValidateReasonControls("Return")) Then
                        Exit Sub
                    End If

                    LOD_v2.DirectReply = False
                Else

                    If (SESSION_GROUP_ID = UserGroups.BoardTechnician) AndAlso (LOD.CurrentStatusCode = LodStatusCode.BoardReview) Then

                        If rblReason.SelectedValue <> "" Then
                            LOD.RwoaReason = rblReason.SelectedValue
                        End If

                        LOD.RwoaExplanation = Server.HtmlEncode(CommentsTextBox.Text)
                        LOD.RwoaDate = DateTime.Now
                        LOD.MedTechComments = String.Empty

                    End If

                    LOD.ReturnComment = Server.HtmlEncode(CommentsTextBox.Text.Trim)

                    SaveRLB()
                End If

                LOD.ReturnToGroup = GetStatusInfo(WorkstatusDao.GetById(workOption.wsStatusOut).StatusCodeType.Id).Item1
                LOD.ReturnByGroup = SESSION_GROUP_ID

            End If

            ReasonPanel.CssClass = ""

            Dim template As Integer = workOption.Template

            'if this is a board member signing, we set the dbsign template only if they have findings
            If (IsUserBelongsToTheBoard(SESSION_GROUP_ID, False, True)) Then
                'we know they are a board member, do they have findings?
                If (MemberHasFindings()) Then
                    template = DBSignTemplateId.Form348Findings
                Else
                    template = DBSignTemplateId.SignOnly
                End If
            End If

            Dim secId As Integer = CInt(GetPersonnelTypeFromGroup(SESSION_GROUP_ID, LOD.Formal))

            Dim selectedText As String
            If (rblReason.SelectedValue.Length > 0) Then
                selectedText = rblReason.SelectedItem.Text
            Else
                selectedText = String.Empty
            End If

            'Adding RWOA/Cancel Reason to the Log
            Dim LogActionDisplayText = LogText(workOption.DisplayText, selectedText, Server.HtmlEncode(CommentsTextBox.Text.Trim()))

            CommitChanges()
            SigBlock.StartSignature(refId, LOD.Workflow, secId, LogActionDisplayText, LOD.Status, workOption.wsStatusOut, workOption.Id, template, String.Empty)
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

        Protected Sub SignStarted(ByVal sender As Object, ByVal e As SignStartedEventArgs) Handles SigBlock.SignStarted
            Dim statusCodeIn As Integer = WorkstatusDao.GetById(e.StatusIn).StatusCodeType.Id
            Dim statusCodeOut As Integer = WorkstatusDao.GetById(e.StatusOut).StatusCodeType.Id

            If (UserCanEdit) Then
                If (statusCodeIn = LodStatusCode.UnitCommanderReview) AndAlso (SESSION_GROUP_ID = UserGroups.UnitCommander) Then
                    'a unit commander is siging this, populate the 'To' on the 348
                    Dim user As AppUser = UserService.CurrentUser()
                    LOD.ToUnit = user.Unit.Name
                End If

                If (LOD.Workflow = AFRCWorkflows.LOD_v2) Then
                    If (statusCodeIn = LodStatusCode.AppointingAutorityReview OrElse statusCodeIn = LodStatusCode.FormalActionByAppointingAuthority) AndAlso (SESSION_GROUP_ID = UserGroups.WingCommander) Then
                        'a unit commander is siging this, populate the 'To' on the 348
                        Dim user As AppUser = UserService.CurrentUser()
                        LOD_v2.AppointingUnit = user.Unit.Name
                    End If
                End If

                'because this changes data that is signed, it must be done before the signing takes place
                InsertPersonnelInfo(statusCodeIn) 'This updates the person who is actuall forwarding the lod
                CommitChanges()
            End If

            If (LOD.CurrentStatusCode = LodStatusCode.NotifyFormalInvestigator) Then
                If (statusCodeOut = LodStatusCode.FormalInvestigation) Then
                    If (Not SaveIO()) Then
                        LogManager.LogError("An error occured saving IO.  Id: " + LOD.Id.ToString())
                        e.Cancel = True
                    End If
                End If
            End If
        End Sub

        Protected Sub SignatureCompleted(sender As Object, e As SignCompletedEventArgs) Handles SigBlock.SignCompleted
            If (e.SignaturePassed) Then

                'we have a good signature.  take any further actions that are required
                ChangeStatus(e.OptionId, e.StatusOut, e.Text, e.Comments)
                SetFeedbackMessage("Case " + LOD.CaseId + " successfully signed.  Action applied: " + e.Text)

                Response.Redirect(Resources._Global.StartPage, True)
            End If
        End Sub

#Region "AddPersonnelInformation"

        Public Sub InsertPersonnelInfo(ByVal StatusCodeIn As Integer)
            'This updates the personnel info for the actual person who forwarded the lod

            If (GetStatusInfo(StatusCodeIn).Item3 > 0 AndAlso GetStatusInfo(StatusCodeIn).Item4 = ALOD.Core.Domain.Workflow.PageAccessType.ReadWrite) Then
                Dim cFinding As LineOfDutyFindings
                cFinding = CreateFinding(LOD.Id)
                cFinding.PType = GetStatusInfo(StatusCodeIn).Item3

                If (GetStatusInfo(StatusCodeIn).Item3 = PersonnelTypes.FORMAL_APP_AUTH Or GetStatusInfo(StatusCodeIn).Item3 = PersonnelTypes.APPOINT_AUTH) Then
                    cFinding.Name = UserService.CurrentUser.FirstLastName
                    cFinding.Pascode = UserService.CurrentUser.Unit.PasCode
                    LOD.AAUserId = SESSION_USER_ID
                End If

                LOD.SetPersonalByType(cFinding)

            End If

        End Sub

#End Region


        Public Function SaveIO() As Boolean
            Dim ioUserId As Integer = 0
            Dim isFormal As Boolean = False


            If (LOD.AppointedIO IsNot Nothing) Then
                ioUserId = LOD.AppointedIO.Id
            End If

            If ioUserId = 0 Then
                Return False
            End If

            ' This is needed if the selected IO is changed after the case was pushed forward and sent back to the Wing CC step
            ' because the update to the Form261 record in the AssignIO call becomes overridden by an NHibernate commit. 
            If (LOD.LODInvestigation IsNot Nothing) Then
                LOD.LODInvestigation.IoUserId = ioUserId
            End If

            isFormal = LOD.IsReconductFormalInvestigationRequested(LookupDao)

            Return LodService.AssignIo(refId, ioUserId, LOD.AAUserId.Value, isFormal)
        End Function

        Public Sub SaveCancelRecommendation()

            ' Formal - Approving Authority Cancel Recommendation
            If (SESSION_GROUP_ID = UserGroups.BoardApprovalAuthority AndAlso LOD.CurrentStatusCode = LodStatusCode.FormalApprovingAuthorityAction) Then

                LOD.ApprovingCancelReasonId = CShort(rblReason.SelectedValue)
                LOD.ApprovingCancelExplanation = Server.HtmlEncode(CommentsTextBox.Text.Trim)

            End If
        End Sub


#Region "ChangeStatusAndActions"

        Public Sub ChangeStatus(optionId As Integer, newStatus As Integer, text As String, comments As String)
            Dim statusIn As Integer = LOD.Status
            Dim newStatusCode As Integer = WorkstatusDao.GetById(newStatus).StatusCodeType.Id

            If (newStatus <> LOD.Status) Then
                LogManager.LogAction(ModuleType, UserAction.Signed, LOD.Id, Server.HtmlEncode(CommentsTextBox.Text.Trim), statusIn, newStatus)
                LogManager.LogAction(ModuleType, UserAction.StatusChanged, LOD.Id, text, statusIn, newStatus)

                ApplyActions(optionId, statusIn, newStatus)

                LOD.Status = newStatus
                LOD.ModifiedBy = SESSION_USER_ID
                LOD.ModifiedDate = DateTime.Now()

                LODDao.SaveOrUpdate(LOD)
                CommitChanges()

                If (newStatusCode = LodStatusCode.Complete OrElse newStatusCode = LodStatusCode.Cancelled) Then
                    SaveFinalPDFForm()
                    LOD.UpdateIsPostProcessingComplete(DaoFactory)
                    LODDao.SaveOrUpdate(LOD)
                    CommitChanges()
                End If
            End If

            Dim reminderDao = New ReminderEmailsDao()
            reminderDao.ReminderEmailUpdateStatusChange(statusIn, LOD.Status, LOD.CaseId, "LOD")
        End Sub

        Protected Sub SaveFinalPDFForm()
            If (Not LOD.DocumentGroupId.HasValue) Then
                Exit Sub
            End If

            'check for Formal investigation
            '  Dim is261 As Boolean = False
            Dim description As String = ""
            Dim file216 As String = ""

            If (LOD.Formal) Then
                '  is261 = True
                description = "and Form261 "
                file216 = "_Form261"
            End If

            Dim SavePDF As New PDFCreateFactory()
            Dim doc As PDFDocument = SavePDF.GeneratePdf(LOD.Id, LOD.ModuleType)
            Dim groupID As Long = LOD.DocumentGroupId
            Dim fileName As String = "Form348" & file216 & "-Case:" & LOD.CaseId.ToString() & "-Generated.pdf"
            Dim docId As Long = 0

            Dim docMetaData As Document = New Document()
            docMetaData.DateAdded = DateTime.Now
            docMetaData.DocDate = DateTime.Now
            docMetaData.Description = "Form348 " & description & "Generated for Case Id: " & LOD.CaseId.ToString()
            docMetaData.DocStatus = DocumentStatus.Approved
            docMetaData.Extension = "pdf"
            docMetaData.SSN = LOD.MemberSSN
            docMetaData.DocType = DocumentType.FinalForm348

            doc.Render(fileName)
            docId = DocumentDao.AddDocument(doc.GetBuffer(), fileName, groupID, docMetaData)

            LogManager.LogAction(ModuleType, UserAction.AddedDocument, LOD.Id, "Generated Form 348 PDF")
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
                    Case WorkflowActionType.ChangeToFormal
                        LOD.Formal = True
                    Case WorkflowActionType.ChangeToInformal
                        LOD.Formal = False
                    Case WorkflowActionType.SaveRWOA
                        InsertRWOA(statusIn, statusOut)
                    Case WorkflowActionType.ReturnRWOA
                        ReturnRWOA(statusIn)
                    Case WorkflowActionType.ReturnTo
                        ReturnTo(statusIn, statusOut)
                    Case WorkflowActionType.ReturnBack
                        ReturnBack(statusIn, statusOut, False)
                    Case WorkflowActionType.SaveFinalDecision
                        SetFinalDecision(statusIn, statusOut)
                    Case WorkflowActionType.AddSignature
                        AddSignature(actn.Target)
                    Case WorkflowActionType.RemoveSignature
                        RemoveSignature(actn.Data, statusOut)
                    Case WorkflowActionType.SendEmail
                        AddEmails(emails, mailService, actn, statusOut)
                    Case WorkflowActionType.AddApprovalAuthoritySignature
                        AddApprovalAuthoritySignature()
                    Case WorkflowActionType.SendLessonsLearnedEmail
                        AddEmailsForLessonsLearned(emails, mailService, actn, statusOut)
                    Case WorkflowActionType.RecommendCancelFormal
                        If (actn.Data = 1) Then
                            LOD.IsFormalCancelRecommended = True
                        Else
                            LOD.IsFormalCancelRecommended = False
                        End If
                End Select
            Next

            emails.SetField("CASE_NUMBER", LOD.CaseId)
            emails.SetField("APP_LINK", GetHostName())
            emails.SetField("TO_STATUS", WorkstatusDao.GetDescription(statusOut))
            emails.SendAll()
        End Sub

        Private Sub RemoveSignature(deleteWorkStatus As Integer, newStatus As Integer)
            Dim newStatusCode As Integer = WorkstatusDao.GetById(newStatus).StatusCodeType.Id

            If (LOD.Workflow = 1) Then
                'Old LOD only deletes signature meta data for the workstatus it is sent to
                LOD.RemoveSignature(DaoFactory, newStatus, newStatusCode)
            Else
                'LOD v2 deletes signature meta data of the desired workstatus 
                LOD_v2.RemoveSignature(DaoFactory, deleteWorkStatus, newStatusCode)
            End If

        End Sub

        Private Sub AddSignature(ByVal groupId As Integer)
            Dim user As AppUser = UserService.CurrentUser()

            If (LOD.Workflow = 1) Then
                LOD.AddSignature(DaoFactory, groupId, user.SignatureTitle, user)
            Else
                LOD_v2.AddSignature(DaoFactory, groupId, user.SignatureTitle, user)
            End If
        End Sub

        Private Sub AddApprovalAuthoritySignature()

            Dim user As AppUser = UserService.CurrentUser()
            Dim sig As String = String.Empty
            Dim title As String = String.Empty

            If (Not String.IsNullOrEmpty(user.AlternateSignatureName)) Then
                sig = user.AlternateSignatureName & ", USAFR"
            Else
                sig = user.SignatureName & ", USAFR"
            End If

            title = UserService.GetUserAlternateTitle(user.Id, UserGroups.BoardApprovalAuthority)

            LOD.AddSignature(DaoFactory, UserGroups.BoardApprovalAuthority, title, user)

        End Sub

        Public Sub InsertRWOA(ByVal statusIn As Integer, ByVal statusOut As Integer)
            Dim statusCodeIn As Integer = WorkstatusDao.GetById(statusIn).StatusCodeType.Id
            Dim statusCodeOut As Integer = WorkstatusDao.GetById(statusOut).StatusCodeType.Id

            Dim rwoaDao As IRwoaDao = DaoFactory.GetRwoaDao()


            If (IsReturnedToUnit(statusCodeIn, statusCodeOut)) Then


                If GetStatusInfo(statusCodeOut).Item1 <> 0 Then
                    LOD.ReturnByGroup = SESSION_GROUP_ID
                    LOD.ReturnToGroup = GetStatusInfo(statusCodeOut).Item1
                End If

                Dim rwRec As New ALOD.Core.Domain.Modules.Lod.Rwoa
                rwRec.RefId = LOD.Id
                rwRec.Workflow = LOD.Workflow
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


        Public Sub ReturnRWOA(ByVal statusIn As Integer)

            Dim rwoaDao As IRwoaDao = DaoFactory.GetRwoaDao()

            Dim rwRec = rwoaDao.GetRecentRWOA(LOD.Workflow, LOD.Id)

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

            Dim returnDao As IReturnDao = DaoFactory.GetReturnDao()

            Dim oldReturn = returnDao.GetRecentReturn(LOD.Workflow, LOD.Id)
            If (oldReturn IsNot Nothing AndAlso oldReturn.DateSentBack Is Nothing) Then
                ReturnBack(statusIn, statusOut, True)
            End If

            If GetStatusInfo(statusCodeOut).Item1 <> 0 Then
                LOD.ReturnByGroup = SESSION_GROUP_ID
                LOD.ReturnToGroup = GetStatusInfo(statusCodeOut).Item1
            End If

            Dim rwRec As New ALOD.Core.Domain.Modules.Lod.Return
            rwRec.RefId = LOD.Id
            rwRec.Workflow = LOD.Workflow
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


        Public Sub ReturnBack(ByVal statusIn As Integer, ByVal statusOut As Integer, ByVal ReturnFlag As Boolean)
            Dim statusCodeIn As Integer = WorkstatusDao.GetById(statusIn).StatusCodeType.Id
            Dim statusCodeOut As Integer = WorkstatusDao.GetById(statusOut).StatusCodeType.Id

            Dim returnDao As IReturnDao = DaoFactory.GetReturnDao()

            Dim rwRec = returnDao.GetRecentReturn(LOD.Workflow, LOD.Id)

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

                ElseIf (statusCodeOut = LodStatusCode.Cancelled) Then           'Case was canceled during Return
                    rwRec.DateSentBack = DateTime.Now
                    If (String.IsNullOrEmpty(rwRec.CommentsBackToSender)) Then
                        rwRec.CommentsBackToSender = "Case has been Cancelled: " + ucRLB.MedTechComments
                        rwRec.rerouting = 0
                    End If

                ElseIf (statusCodeOut = LodStatusCode.Complete) Then            'Case was completed during Return
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

        ' Currently only Medical Officers and Appointing Authorities can cancel cases (informal and formal respectively)
        ' Approving Authorities can recommend a formal case be cancelled
        Public Sub SaveCancelData()
            If (Not UserCanEdit) Then
                Exit Sub
            End If

            Dim medText As String = ""

            ' Informal - Medical Officer Cancel
            If (SESSION_GROUP_ID = UserGroups.MedicalOfficer AndAlso LOD.CurrentStatusCode = LodStatusCode.MedicalOfficerReview) Then


                LOD.LODMedical.PhysicianCancelReason = CShort(rblReason.SelectedValue)
                LOD.LODMedical.PhysicianCancelExplanation = Server.HtmlEncode(CommentsTextBox.Text.Trim)

                If LOD.LODMedical.PhysicianCancelReason = ALOD.Core.Utils.PhyCancelReason.Other Then
                    medText = LOD.LODMedical.PhysicianCancelExplanation
                Else
                    medText = LOD.LODMedical.CancelDescription
                End If


                LOD.FinalDecision = "Canceled by Medical Officer:  " + medText
            End If

            ' Formal - Appointing Authority Cancel
            If (SESSION_GROUP_ID = UserGroups.WingCommander AndAlso LOD.CurrentStatusCode = LodStatusCode.FormalActionByAppointingAuthority) Then

                LOD.AppointingCancelReasonId = CShort(rblReason.SelectedValue)
                LOD.AppointingCancelExplanation = Server.HtmlEncode(CommentsTextBox.Text.Trim)

                Dim reason As String = LookupService.GetCancelReasonDescription(LOD.AppointingCancelReasonId)
                LOD.FinalDecision = "Canceled by Appointing Authority:  " + reason
            End If
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
        ''' <param name="_statusIn"></param>
        ''' <param name="_statusOut"></param>
        ''' <remarks></remarks>
        Public Sub SetFinalDecision(ByVal _statusIn As Integer, ByVal _statusOut As Integer)
            Dim Approving As AppUser


            Select Case WorkstatusDao.GetById(_statusIn).StatusCodeType.Id

                Case LodStatusCode.MedicalOfficerReview, LodStatusCode.FormalActionByAppointingAuthority
                    SaveCancelData()

                Case LodStatusCode.AppointingAutorityReview
                    Dim appauthfndgs As LineOfDutyFindings
                    appauthfndgs = LOD.FindByType(PersonnelTypes.APPOINT_AUTH)
                    If (Not (appauthfndgs Is Nothing) AndAlso (Not appauthfndgs.Finding Is Nothing)) Then
                        LOD.FinalDecision = "Closed by Appointing Auth:  " + appauthfndgs.Description
                        LOD.FinalFindings = appauthfndgs.Finding.Value
                    End If
                    ResetLODCancelProperties()

                Case LodStatusCode.BoardReview
                    Dim hqaaFinding As LineOfDutyFindings
                    Dim hqbdFinding As LineOfDutyFindings
                    Dim cFinding As LineOfDutyFindings
                    hqaaFinding = LOD.FindByType(PersonnelTypes.BOARD_AA)
                    If (Not (hqaaFinding Is Nothing)) AndAlso (Not hqaaFinding.Finding Is Nothing) AndAlso (hqaaFinding.Finding.Value <> ALOD.Core.Utils.Finding.Recommend_Formal_Investigation) Then
                        LOD.FinalDecision = "Closed by LOD Board Approving Auth:  " + hqaaFinding.Description
                        LOD.FinalFindings = hqaaFinding.Finding.Value
                        LOD.BoardForGeneral = "N"
                    Else
                        hqbdFinding = LOD.FindByType(CShort(PersonnelTypes.BOARD))
                        If (Not (hqbdFinding Is Nothing)) AndAlso (Not (hqbdFinding.Finding Is Nothing)) Then
                            LOD.FinalDecision = "Closed by LOD Board Admin:  " + hqbdFinding.Description
                            LOD.BoardForGeneral = "Y"
                            LOD.FinalFindings = hqbdFinding.Finding.Value
                            cFinding = NewFinding(LOD.Id)
                            Approving = UserService.GetById(LOD.ApprovingAuthorityUserId)
                            cFinding.Name = Approving.FirstLastName
                            cFinding.Rank = Approving.Rank.Rank
                            cFinding.Compo = Approving.Component
                            cFinding.Grade = Approving.Rank.Grade
                            cFinding.PType = CShort(PersonnelTypes.BOARD_AA)
                            cFinding.Finding = hqbdFinding.Finding.Value
                            LOD.SetFindingByType(cFinding)
                        End If
                    End If

                    ResetLODCancelProperties()

                Case LodStatusCode.FormalBoardReview
                    Dim aafrmlFinding As LineOfDutyFindings
                    Dim hqfrmlbdFinding As LineOfDutyFindings
                    Dim cFinding As LineOfDutyFindings
                    'AA Formal Board
                    aafrmlFinding = LOD.FindByType(PersonnelTypes.FORMAL_BOARD_AA)
                    If (Not (aafrmlFinding Is Nothing)) AndAlso (Not aafrmlFinding.Finding Is Nothing) Then
                        LOD.FinalDecision = "Closed by LOD Board Approving Auth:(Formal Inv):" + aafrmlFinding.Description
                        LOD.LODInvestigation.FinalApprovalFindings = aafrmlFinding.Finding
                        LOD.FinalFindings = aafrmlFinding.Finding.Value
                        LOD.BoardForGeneral = "N"
                    Else
                        hqfrmlbdFinding = LOD.FindByType(CShort(PersonnelTypes.FORMAL_BOARD_RA))
                        If (Not (hqfrmlbdFinding Is Nothing)) AndAlso (Not (hqfrmlbdFinding.Finding Is Nothing)) Then
                            LOD.FinalDecision = "Closed by LOD Board Admin:(Formal Inv):" + hqfrmlbdFinding.Description
                            LOD.LODInvestigation.FinalApprovalFindings = hqfrmlbdFinding.Finding
                            LOD.FinalFindings = hqfrmlbdFinding.Finding.Value
                            LOD.BoardForGeneral = "Y"
                            cFinding = NewFinding(LOD.Id)
                            Approving = UserService.GetById(LOD.ApprovingAuthorityUserId)
                            cFinding.Name = Approving.FirstLastName
                            cFinding.Rank = Approving.Rank.Rank
                            cFinding.Compo = Approving.Component
                            cFinding.Grade = Approving.Rank.Grade
                            cFinding.PType = CShort(PersonnelTypes.FORMAL_BOARD_AA)
                            cFinding.Finding = hqfrmlbdFinding.Finding.Value
                            LOD.SetFindingByType(cFinding)
                        End If
                    End If

                    ResetLODCancelProperties()

            End Select

            LOD.Completed_By_Unit = LOD.MemberUnitId

        End Sub

        Public Sub AddEmails(ByRef emails As MailManager, emailService As EmailService, action As WorkflowOptionAction, statusOut As Integer)
            Dim toList As StringCollection
            Dim statusCodeOut As Integer = WorkstatusDao.GetById(statusOut).StatusCodeType.Id

            If (statusCodeOut = LodStatusCode.Complete OrElse statusCodeOut = LodStatusCode.Cancelled) Then
                'add it to the email manager
                toList = emailService.GetDistributionListByGroup(LOD.Id, action.Target, "LOD")
            Else
                toList = emailService.GetDistributionListByWorkflow(LOD.Id, statusOut, LOD.Workflow, action.Target)
            End If

            emails.AddTemplate(action.Data, "", toList)
        End Sub

        ''' <summary>
        ''' Summary for AddEmailsForLessonsLearned Method.
        ''' This method will add the Distribution List to the email message.
        ''' Will also add the template id for the Lessons Learned Email.
        ''' </summary>
        ''' <param name="emails">Mail Manager</param>
        ''' <param name="emailService">EmailService</param>
        ''' <param name="action">WorkflowOptionAction</param>
        ''' <param name="statusOut">LOD Status (Out)</param>
        ''' <remarks>The email will be sent only if one of the Board members added Lessons Learned.</remarks>
        Public Sub AddEmailsForLessonsLearned(emails As MailManager, emailService As EmailService, action As WorkflowOptionAction, statusOut As Integer)
            Dim toList As StringCollection

            'Validate LOD is in Complete Status
            If (WorkstatusDao.GetById(statusOut).StatusCodeType.Id = LodStatusCode.Complete) Then
                'Validate we have LessonsLearned Added
                If (emailService.LODHasLessonsLearned(LOD.Id)) Then
                    'Now, get the distributuion List and set the template
                    toList = emailService.GetEmailListForLessonsLearned(LOD.Id, ModuleType)
                    emails.AddTemplate(action.Data, "", toList)
                End If
            End If
        End Sub

        Public Function IsReturnedToBoard(ByVal statusIn As Short) As Boolean
            'If board- unit -board
            'This will check last two stages before the current status 
            If (boardStages.Contains(statusIn)) Then
                Dim ws As IList(Of WorkStatusTracking) = WorkFlowService.GetWorkStatusTracking(LOD.Id, ModuleType)

                If (ws IsNot Nothing) Then
                    If (ws.Count > 1) Then
                        If (unitStages.Contains(ws(0).WorkflowStatus.StatusCodeType.Id)) Then     'The last stage was unit stage
                            If (boardStages.Contains(ws(1).WorkflowStatus.StatusCodeType.Id)) Then     'The stage before that was board
                                Return True
                            End If
                        End If
                    End If
                End If
            End If

            Return False
        End Function

        Public Function IsReturnedToUnit(ByVal statusIn As Short, ByVal statusOut As Short) As Boolean
            'Board--Unit --Simply check the last status if it  is from board and the current status is one of the units then it has been returned to Unit
            If (boardStages.Contains(statusIn)) Then
                If unitStages.Contains(statusOut) Then
                    Return True
                End If
            End If
            Return False
        End Function

        Public Function CheckReturn(ByVal statusIn As Short, ByVal statusOut As Short) As Boolean
            'Check if the case could be a return
            If (unitStages.Contains(statusOut) AndAlso unitStages.Contains(statusOut)) Then
                Return True
            ElseIf (boardStages.Contains(statusOut) AndAlso boardStages.Contains(statusOut)) Then
                Return True
            End If
            Return False
        End Function

        Private Sub ResetLODCancelProperties()
            LOD.LODMedical.PhysicianCancelReason = Nothing
            LOD.LODMedical.PhysicianCancelExplanation = String.Empty

            LOD.ApprovingCancelReasonId = Nothing
            LOD.ApprovingCancelExplanation = String.Empty

            LOD.AppointingCancelReasonId = Nothing
            LOD.AppointingCancelExplanation = String.Empty
        End Sub

#End Region

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

            ElseIf (workOption.DisplayText Like "RWOA*") Then
                PopulateRWOArbl()

                reasonTitle.Text = "* Reason for RWOA:"
                commentsTitle.Text = "* RWOA Comments:"
                CommentsTextBox.MaxLength = 400
                SetMaxLength(CommentsTextBox)

                CommentsTextBox.Text = ""

                reasonRow.Visible = True
                reasonLetter.Text = "B"
                commentsRow.Visible = True
                commentLetter.Text = "C"

            ElseIf (workOption.DisplayText Like "Return*") Then

                If (LOD.Workflow = AFRCWorkflows.LOD AndAlso SESSION_GROUP_ID = UserGroups.BoardTechnician) Then
                    PopulateRWOArbl()

                    reasonTitle.Text = "* Reason for RWOA:"
                    commentsTitle.Text = "* RWOA Comments:"
                    CommentsTextBox.MaxLength = 400
                    SetMaxLength(CommentsTextBox)

                    CommentsTextBox.Text = ""

                    reasonRow.Visible = True
                    reasonLetter.Text = "B"
                    commentsRow.Visible = True
                    commentLetter.Text = "C"
                ElseIf (LOD.Workflow = AFRCWorkflows.LOD) Then
                    commentsTitle.Text = "* Return Comments:"
                    CommentsTextBox.Text = ""
                    CommentsTextBox.MaxLength = 250
                    SetMaxLength(CommentsTextBox)

                    reasonRow.Visible = False
                    commentsRow.Visible = True
                    commentLetter.Text = "B"
                Else
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
                End If


            ElseIf (workOption.DisplayText Like "Direct Formal Investigation*") Then

                commentsTitle.Text = "Comments:"
                CommentsTextBox.Text = ""
                CommentsTextBox.MaxLength = 1000
                SetMaxLength(CommentsTextBox)

                reasonRow.Visible = False
                commentsRow.Visible = True
                commentLetter.Text = "B"
            Else
                reasonRow.Visible = False
                commentsRow.Visible = False
            End If
        End Sub

        Protected Sub PopulateRWOArbl()
            NextActionHelpers.PopulateRadioButtonListWithRwoaOptions(rblReason, LOD.Workflow, LookupDao)
        End Sub

        Protected Sub PopulateCancelrbl()
            rblReason.DataSource = LookupService.GetWorkflowCancelReasons(LOD.Workflow, LOD.Formal)
            rblReason.DataTextField = "Description"
            rblReason.DataValueField = "Id"
            rblReason.DataBind()
        End Sub

        Protected Sub PopulateReturnrbl()
            NextActionHelpers.PopulateRadioButtonListWithReturnOptions(rblReason, LOD.Workflow, LookupDao)
        End Sub

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

        Protected Function GetStatusInfo(ByVal statusCode As Integer) As Tuple(Of Integer, String, Integer, Integer)


            Dim sentTo As String = ""
            Dim sentToId As Integer = 0
            Dim ptype As Integer = 0
            Dim access As Integer = 0

            Select Case statusCode
                Case LodStatusCode.MedTechReview
                    sentTo = "Med Tech"
                    sentToId = UserGroups.MedicalTechnician

                Case LodStatusCode.MedicalOfficerReview
                    sentTo = "Medical Officer"
                    sentToId = UserGroups.MedicalOfficer

                Case LodStatusCode.WingSarcInput
                    sentTo = "Wing SARC"
                    sentToId = UserGroups.WingSarc

                Case LodStatusCode.UnitCommanderReview
                    sentTo = "Unit Cmdr"
                    sentToId = UserGroups.UnitCommander
                    ptype = PersonnelTypes.UNIT_CMDR
                    access = SectionList(SectionNames.UNIT_CMD_REV.ToString())

                Case LodStatusCode.AppointingAutorityReview
                    sentTo = "Wing CC"
                    sentToId = UserGroups.WingCommander
                    ptype = PersonnelTypes.APPOINT_AUTH
                    access = SectionList(SectionNames.APP_AUTH_REV.ToString())

                Case LodStatusCode.FormalActionByAppointingAuthority
                    sentTo = "Wing CC"
                    sentToId = UserGroups.WingCommander
                    ptype = PersonnelTypes.FORMAL_APP_AUTH
                    access = SectionList(SectionNames.FORMAL_ACTION_APP_AUTH.ToString())

                Case LodStatusCode.WingJAReview
                    sentTo = "Wing JA"
                    sentToId = UserGroups.WingJudgeAdvocate
                    ptype = PersonnelTypes.WING_JA
                    access = SectionList(SectionNames.WING_JA_REV.ToString())

                Case LodStatusCode.FormalActionByWingJA
                    sentTo = "Wing JA"
                    sentToId = UserGroups.WingJudgeAdvocate
                    ptype = PersonnelTypes.FORMAL_WING_JA
                    access = SectionList(SectionNames.FORMAL_ACTION_WING_JA.ToString())

                Case LodStatusCode.BoardReview
                    sentTo = "Board Technician"
                    sentToId = UserGroups.BoardTechnician
                    ptype = PersonnelTypes.BOARD
                    access = SectionList(SectionNames.BOARD_REV.ToString())

                Case LodStatusCode.FormalBoardReview
                    sentTo = "Board Technician"
                    sentToId = UserGroups.BoardTechnician
                    ptype = PersonnelTypes.FORMAL_BOARD_RA
                    access = SectionList(SectionNames.FORMAL_BOARD_REV.ToString())

                Case LodStatusCode.BoardMedicalReview
                    sentTo = "Board Medical"
                    sentToId = UserGroups.BoardMedical
                    ptype = PersonnelTypes.BOARD_SG
                    access = SectionList(SectionNames.BOARD_MED_REV.ToString())

                Case LodStatusCode.FormalBoardMedicalReview
                    sentTo = "Board Medical"
                    sentToId = UserGroups.BoardMedical
                    ptype = PersonnelTypes.FORMAL_BOARD_SG
                    access = SectionList(SectionNames.FORMAL_BOARD_MED_REV.ToString())

                Case LodStatusCode.SeniorMedicalReview
                    sentTo = "Senior Medical Reviewer"
                    sentToId = UserGroups.SeniorMedicalReviewer
                    ptype = PersonnelTypes.SENIOR_MEDICAL_REVIEWER
                    access = SectionList(SectionNames.SENIOR_MED_REV.ToString())

                Case LodStatusCode.FormalSeniorMedicalReview
                    sentTo = "Senior Medical Reviewer"
                    sentToId = UserGroups.SeniorMedicalReviewer
                    ptype = PersonnelTypes.FORMAL_SENIOR_MEDICAL_REVIEWER
                    access = SectionList(SectionNames.FORMAL_SENIOR_MED_REV.ToString())

                Case LodStatusCode.BoardLegalReview
                    sentTo = "Board Legal"
                    sentToId = UserGroups.BoardLegal
                    ptype = PersonnelTypes.BOARD_JA
                    access = SectionList(SectionNames.BOARD_LEGAL_REV.ToString())

                Case LodStatusCode.FormalBoardLegalReview
                    sentTo = "Board Legal"
                    sentToId = UserGroups.BoardLegal
                    ptype = PersonnelTypes.FORMAL_BOARD_JA
                    access = SectionList(SectionNames.FORMAL_BOARD_LEGAL_REV.ToString())

                Case LodStatusCode.BoardPersonnelReview
                    sentTo = "Board Admin"
                    sentToId = UserGroups.BoardAdministrator
                    ptype = PersonnelTypes.BOARD_A1
                    access = SectionList(SectionNames.BOARD_PERSONNEL_REV.ToString())

                Case LodStatusCode.FormalBoardPersonnelReview
                    sentTo = "Board Admin"
                    sentToId = UserGroups.BoardAdministrator
                    ptype = PersonnelTypes.FORMAL_BOARD_A1
                    access = SectionList(SectionNames.FORMAL_BOARD_PERSONNEL_REV.ToString())

                Case LodStatusCode.ApprovingAuthorityAction
                    sentTo = "Approving Authority"
                    sentToId = UserGroups.BoardApprovalAuthority
                    ptype = PersonnelTypes.BOARD_AA
                    access = SectionList(SectionNames.BOARD_APPROVING_AUTH_REV.ToString())

                Case LodStatusCode.FormalActionByAppointingAuthority
                    sentTo = "Approving Authority"
                    sentToId = UserGroups.BoardApprovalAuthority
                    ptype = PersonnelTypes.FORMAL_BOARD_AA
                    access = SectionList(SectionNames.FORMAL_BOARD_APPROVING_AUTH_REV.ToString())

                Case LodStatusCode.NotifyFormalInvestigator
                    sentTo = "LOD-PM"
                    sentToId = UserGroups.LOD_PM
                    ptype = PersonnelTypes.LOD_PM
                    access = SectionList(SectionNames.NOTIFY_IO.ToString())

                Case LodStatusCode.FormalInvestigation
                    sentTo = "IO"
                    sentToId = UserGroups.InvestigatingOfficer
                    ptype = PersonnelTypes.IO
                    access = SectionList(SectionNames.IO.ToString())
                Case Else
                    sentTo = ""
            End Select

            Dim sent As Tuple(Of Integer, String, Integer, Integer) = New Tuple(Of Integer, String, Integer, Integer)(sentToId, sentTo, ptype, access)
            Return sent
        End Function

    End Class
End Namespace
