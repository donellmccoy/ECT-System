Imports ALOD.Core.Domain.Common
Imports ALOD.Core.Domain.Common.CustomServerControls
Imports ALOD.Core.Domain.DBSign
Imports ALOD.Core.Domain.Documents
Imports ALOD.Core.Domain.Lookup
Imports ALOD.Core.Domain.Modules.Lod
Imports ALOD.Core.Domain.Modules.SpecialCases
Imports ALOD.Core.Domain.ServiceMembers
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

Namespace Web.Special_Case.MMSO

    Partial Class Secure_sc_mmso_NextAction
        Inherits System.Web.UI.Page

        Private Const VIEWSTATE_VALIDATIONS As String = "VALIDATION_ITEMS"
        Private _assocaiated As IAssociatedCaseDao
        Private _dao As IDocumentDao
        Private _docCatViewDao As IDocCategoryViewDao
        Private _documents As IList(Of ALOD.Core.Domain.Documents.Document)
        Private _factory As IDaoFactory
        Private _lod As LineOfDuty = Nothing
        Dim _lookupDao As ILookupDao
        Private _memoSource As MemoDao2
        Private _scAccess As Dictionary(Of String, ALOD.Core.Domain.Workflow.PageAccessType)
        Private _separator As String = ";"
        Dim dao As ISpecialCaseDAO
        Private docsUploaded As Boolean

        'Private Const MEDICAL_OFFICER_CANCELLING As String = "Medical Officer cancelling Lod"
        Private errors As New List(Of ValidationItem)

        Private sc As SC_MMSO = Nothing
        Private scId As Integer = 0
        Private separatorTemplate As System.Web.UI.CompiledTemplateBuilder

#Region "SC_MMSOProperty"

        ReadOnly Property associated() As IAssociatedCaseDao
            Get
                If (_assocaiated Is Nothing) Then
                    _assocaiated = DaoFactory.GetAssociatedCaseDao()
                End If

                Return _assocaiated
            End Get
        End Property

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
                If (_dao Is Nothing) Then
                    _dao = New SRXDocumentStore(CStr(HttpContext.Current.Session("UserName")))
                End If

                Return _dao
            End Get
        End Property

        ReadOnly Property DocumentEntityId() As String
            Get
                Dim key As String = "DocEntityId"

                If (ViewState(key) Is Nothing) Then

                    If (SpecCase Is Nothing) Then
                        Return 0
                    End If
                    ViewState(key) = SpecCase.DocumentEntityId
                End If

                Return CStr(ViewState(key))

            End Get
        End Property

        ReadOnly Property DocumentGroupId() As String
            Get
                Dim key As String = "DocGroupId"

                If (ViewState(key) Is Nothing) Then

                    If (LOD Is Nothing) Then
                        Return 0
                    End If

                    ViewState(key) = LOD.DocumentGroupId
                End If

                Return CStr(ViewState(key))

            End Get
        End Property

        ReadOnly Property DocumentViewId() As Integer
            Get
                Dim key As String = "DocViewId"

                If (ViewState(key) Is Nothing) Then

                    If (SpecCase Is Nothing) Then
                        Return 0
                    End If
                    ViewState(key) = SpecCase.DocumentViewId
                End If

                Return CStr(ViewState(key))

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
                scId = CInt(Request.QueryString("refId"))
                If scId = 0 Then
                    scId = Session("refId")
                End If
                Return scId
            End Get
        End Property

        ReadOnly Property RefIds() As String
            Get
                Return Request.QueryString("refId")
            End Get
        End Property

        ReadOnly Property SCDao() As ISpecialCaseDAO
            Get
                If (dao Is Nothing) Then
                    dao = DaoFactory.GetSpecialCaseDAO()
                End If

                Return dao
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

        Protected ReadOnly Property MasterPage() As SC_MMSOMaster
            Get
                Dim master As SC_MMSOMaster = CType(Page.Master, SC_MMSOMaster)
                Return master
            End Get
        End Property

        Protected ReadOnly Property SectionList() As Dictionary(Of String, ALOD.Core.Domain.Workflow.PageAccessType)
            Get
                If (_scAccess Is Nothing) Then
                    _scAccess = SpecCase.ReadSectionList(SESSION_GROUP_ID)
                End If
                Return _scAccess
            End Get
        End Property

        Protected ReadOnly Property SpecCase() As SC_MMSO
            Get
                If (sc Is Nothing) Then
                    sc = SCDao.GetById(RefIds)
                End If

                Return sc
            End Get
        End Property

        Private ReadOnly Property LOD() As LineOfDuty
            Get
                If (_lod Is Nothing) Then
                    _lod = LodService.GetById(associated.GetAssociatedCasesLOD(SpecCase.Id, SpecCase.Workflow).First.associated_RefId)
                End If
                Return _lod
            End Get
        End Property

        Private Property MemberSSN() As String
            Get
                Dim key As String = "MemberSSN"
                Return CStr(ViewState(key))
            End Get
            Set(ByVal value As String)
                Dim key As String = "MemberSSN"
                ViewState(key) = value
            End Set
        End Property

        Private ReadOnly Property MemoStore() As MemoDao2
            Get
                If (_memoSource Is Nothing) Then
                    _memoSource = DaoFactory.GetMemoDao2()
                End If
                Return _memoSource
            End Get
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

#End Region

        Public Function GetUserId(ByVal ssan As String) As Integer
            Dim user As AppUser = UserService.GetBySSN(ssan)

            If (user Is Nothing) Then
                Dim data As ServiceMember = LookupService.GetServiceMemberBySSN(ssan)

                user = New AppUser()
                'we will consider the  ssn as valid.The ssn itself has  been obtained from milpds
                'The user does not yet have an account, so try and import from MILPDS
                If (data IsNot Nothing) Then
                    user.Import(data)
                End If
                'The following code is to make sure that Most of the cases first and last name will exist if   relevent data is not obtained from member data we still have the non null values set
                user.SSN = ssan
                user.Status = AccessStatus.Pending

                If user.Email = "" Then
                    If user.Unit.Email <> "" Then
                        user.Email = user.Unit.Email
                    Else
                        user.Email = ""
                    End If
                End If
                If user.Component = "" Then
                    user.Component = "6"
                End If
                user.Username = UserService.GetUserName(user.FirstName, user.LastName)
                user.LastLoginDate = Now
                user.AccountExpiration = Now.AddYears(1)

                Dim role As New UserRole()
                role.Status = AccessStatus.Approved
                role.Group = New UserGroup(UserGroups.InvestigatingOfficer)
                role.Active = True
                role.User = user
                user.AllRoles.Add(role)

                user.CurrentRole = role
            Else

                Dim extRole = From t In user.AllRoles Where t.Group.Id = UserGroups.InvestigatingOfficer Select t
                'In case the user already existed the accessStatus should be chaged to pending in case it was null else the status should stay the same
                If user.Status = AccessStatus.None Then
                    user.Status = AccessStatus.Pending
                End If
                If extRole.Count < 1 Then
                    Dim role As New UserRole()
                    role.Status = user.Status
                    role.Group = New UserGroup(CInt(UserGroups.InvestigatingOfficer))
                    role.Active = False
                    role.User = user
                    user.AllRoles.Add(role)
                End If

            End If
            UserService.Update(user)
            Return user.Id

        End Function

        Protected Function IsBoardMember() As Boolean

            Dim group As UserGroups = SESSION_GROUP_ID

            Select Case group
                'we don't include board tech in this because they do not make findings
                Case UserGroups.BoardApprovalAuthority, UserGroups.BoardLegal, UserGroups.BoardMedical ', UserGroups.BoardSeniorReviewer
                    Return True

                Case Else
                    Return False
            End Select

        End Function

        Protected Function MemberHasFindings() As Boolean
            Return False
        End Function

        Protected Function OptionToString(ByVal item As ALOD.Core.Domain.Workflow.WorkflowStatusOption) As String

            Dim buffer As New StringBuilder
            buffer.Append(item.Id.ToString() + _separator)
            buffer.Append(item.wsStatusOut.ToString() + _separator)
            buffer.Append(item.Template.ToString())

            Return buffer.ToString()

        End Function

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

            If (Not Page.IsPostBack) Then

                SetMaxLength(CommentBox)
                SetMaxLength(txtCancel)

                SetInputFormatRestriction(Page, txtSendersComments, FormatRestriction.AlphaNumeric, PERMITTED_SPECIAL_CHAR_INPUT)
                SetInputFormatRestriction(Page, txtCancel, FormatRestriction.AlphaNumeric, PERMITTED_SPECIAL_CHAR_INPUT)
                SetInputFormatRestriction(Page, CommentBox, FormatRestriction.AlphaNumeric, PERMITTED_SPECIAL_CHAR_INPUT)

                UserCanEdit = GetAccess(Navigator.PageAccess, True)

                InitData()
                ShowRLB()
                InitNextActionOptions()

            End If

            If Not (UserCanEdit) Then
                SignButton.Enabled = False
            End If
        End Sub

        Protected Sub rbtOption_ItemDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.RepeaterItemEventArgs) Handles rbtOption.ItemDataBound

            If (e.Item.ItemType = ListItemType.Item) OrElse (e.Item.ItemType = ListItemType.AlternatingItem) Then

                Dim rb As ValueRadioButton = CType(e.Item.FindControl("rblOptions"), ValueRadioButton)
                Dim options As ALOD.Core.Domain.Workflow.WorkflowStatusOption

                options = e.Item.DataItem
                rb.Text = Server.HtmlEncode(options.DisplayText)
                Dim returnClass As String = "option-return"

                If (options.DisplayText Like "Return*") Then
                    rb.CssClass = returnClass
                ElseIf (options.DisplayText Like "Cancel*") Then
                    rb.CssClass = "option-cancel"
                Else
                    rb.CssClass = "option-forward"
                End If

                Dim sep As String = ";"
                rb.Value = OptionToString(options)
                rb.Visible = options.OptionVisible
                rb.Enabled = options.OptionValid
                If Not docsUploaded Then
                    If rb.CssClass = "option-forward" Then
                        rb.Enabled = False
                    End If

                End If
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
                If (ChangeStatus(e.OptionId, e.StatusOut, e.Text, e.Comments)) Then
                    SetFeedbackMessage("Case " + SpecCase.CaseId + " successfully signed.  Action applied: " + e.Text)
                    Response.Redirect(Resources._Global.StartPage, True)
                End If
            End If

        End Sub

        Protected Sub SignButton_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles SignButton.Click

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

            'make sure we have an option selected
            If (workOption Is Nothing) Then
                OptionPanel.CssClass = "fieldRequired"
                errors.Add(New ValidationItem("Routing", "Routing", "A Routing option must be selected"))
                Exit Sub
            Else
                OptionPanel.CssClass = ""
            End If

            'we have an option selected

            Dim template As Integer = workOption.Template

            If (SESSION_GROUP_ID = UserGroups.SeniorMedicalReviewer) Then
                template = DBSignTemplateId.MMSO_Unit
            Else
                template = DBSignTemplateId.SignOnly
            End If

            Dim secId As Integer = 0

            secId = CInt(GetPersonnelTypeFromGroup(SESSION_GROUP_ID, False))

            SigBlock.StartSignature(RefId, SpecCase.Workflow, secId, workOption.DisplayText, SpecCase.Status, workOption.wsStatusOut,
                                           workOption.Id, template, Server.HtmlEncode(CommentBox.Text.Trim))

        End Sub

        Protected Sub SignStarted(ByVal sender As Object, ByVal e As SignStartedEventArgs) Handles SigBlock.SignStarted

        End Sub

        Protected Function StringToOption(ByVal input As String) As ALOD.Core.Domain.Workflow.WorkflowStatusOption
            Dim item As ALOD.Core.Domain.Workflow.WorkflowStatusOption

            Dim parts() As String = input.Split(_separator)
            item = WorkFlowService.GetOptionById(CInt(parts(0)))

            Return item

        End Function

        Private Sub InitData()

            If ((RefId = 0) And (scId = 0)) Then
                Exit Sub
            End If

            'show the comments on the return (if there are any)
            If (Not String.IsNullOrEmpty(SpecCase.Return_Comment)) Then
                CommentPanel.Visible = True
                CommentLabel.Text = SpecCase.Return_Comment.Replace(Environment.NewLine, "<br />")
            Else
                CommentPanel.Visible = False
            End If

        End Sub

        Private Sub InitNextActionOptions()

            'as part of this method call the validation errors are generated
            'so we will store them from here for later display
            If WorkFlowService.GetLastStatus(SpecCase.Id, ModuleType.SpecCaseMMSO) <> 0 Then
                Dim options = From o In SpecCase.GetCurrentOptions(WorkFlowService.GetLastStatus(SpecCase.Id, ModuleType.SpecCaseMMSO), DaoFactory)
                              Select o
                              Order By o.SortOrder

                docsUploaded = ValidateLODDocuments(SpecCase.Validations)
                rbtOption.DataSource = options
                rbtOption.DataBind()
            Else
                'No actions have been tracked against this Reinvestigation Request, probably brand new
                Dim options = From o In SpecCase.GetCurrentOptions(SpecCase.Status, DaoFactory)
                              Select o
                              Order By o.SortOrder

                docsUploaded = ValidateLODDocuments(SpecCase.Validations)
                rbtOption.DataSource = options
                rbtOption.DataBind()
            End If
            ViewState(VIEWSTATE_VALIDATIONS) = SpecCase.Validations

        End Sub

        Private Sub ShowRLB()

            RLBEnabled = False

        End Sub

        Private Sub TabButtonClicked(ByRef sender As Object, ByRef e As TabNavigationEventArgs)

            If (e.ButtonType = NavigatorButtonType.Save) Then
                Response.Redirect(Request.RawUrl)
            End If
        End Sub

        ''' <summary>
        ''' Check that required documents have been uploaded on the associated LOD
        ''' </summary>
        ''' <param name="validations"></param>
        ''' <remarks>Proof of Military Status is required</remarks>
        ''' <returns>False if required documents not uploaded</returns>
        Private Function ValidateLODDocuments(ByVal validations As IList(Of ValidationItem)) As Boolean
            ValidateLODDocuments = True
            If (_documents Is Nothing) Then
                _documents = DocumentDao.GetDocumentsByGroupId(DocumentGroupId)
            End If
            Dim docs = From d In _documents Where d.DocType = DocumentType.ProofOfMilitaryStatus Select d
            Dim docCount As Integer = docs.Count()
            If docCount = 0 Then
                validations.Add(New ValidationItem("LOD Documents", "Orders", "Proof of Military Status is required"))
                ValidateLODDocuments = False
            End If

        End Function

#Region "AddPersonnelInformation"

        Public Sub InsertPersonnelInfo()
        End Sub

#End Region

#Region "ChangeStatusAndActions"

        Public Sub ApplyActions(ByVal actionId As Integer, ByVal optionId As Integer, ByVal statusIn As Integer, ByVal statusOut As Integer)
            '

            Dim optn As ALOD.Core.Domain.Workflow.WorkflowStatusOption
            optn = WorkFlowService.GetOptionById(optionId)

            Dim mailService As EmailService = New EmailService()
            Dim emails As New ALOD.Core.Domain.Common.MailManager(mailService)
            For Each actn As WorkflowOptionAction In optn.ActionList

                Select Case actn.ActionType.Id

                    Case WorkflowActionType.ChangeToFormal
                        SpecCase.ModifiedBy = SESSION_USER_ID
                        SpecCase.ModifiedDate = DateTime.Now()
                    Case WorkflowActionType.AddSignature
                        AddSignature(actn.Target)
                    Case WorkflowActionType.RemoveSignature
                        SpecCase.RemoveSignature(DaoFactory, statusOut)
                    Case WorkflowActionType.SendEmail
                        AddEmails(emails, mailService, actn, statusOut)
                End Select

            Next

            emails.SetField("CASE_NUMBER", SpecCase.CaseId)
            emails.SetField("MEMBER_NAME", SpecCase.MemberName)
            emails.SetField("APP_LINK", GetHostName())
            emails.SendAll()

        End Sub

        Public Function ChangeStatus(optionId As Integer, newStatus As Integer, text As String, comments As String) As Boolean

            Dim passed As Boolean = True
            Dim statusIn As Integer = SpecCase.Status

            If (newStatus <> SpecCase.Status) Then

                'log our action
                Dim actionId As Integer = LogManager.LogAction(ModuleType.SpecCaseMMSO, UserAction.Signed, SpecCase.Id, comments, statusIn, newStatus)
                LogManager.LogAction(ModuleType.SpecCaseMMSO, UserAction.StatusChanged, SpecCase.Id, text, statusIn, newStatus)
                'now take any other actions that go with this
                ApplyActions(actionId, optionId, statusIn, newStatus)
                AddSignature(SESSION_GROUP_ID)
            End If

            SpecCase.Status = newStatus
            SpecCase.Return_Comment = Server.HtmlEncode(comments)
            SpecCase.ModifiedBy = SESSION_USER_ID
            SpecCase.ModifiedDate = DateTime.Now()

            SCDao.SaveOrUpdate(SpecCase)
            CommitChanges()

            ' Update Reminder Emails
            Dim reminderDao = New ReminderEmailsDao()
            reminderDao.ReminderEmailUpdateStatusChange(statusIn, newStatus, SpecCase.CaseId, "SC")

            Return passed

        End Function

        Public Sub CommitChanges()
            dao.CommitChanges()
            dao.Evict(SpecCase)
            dao = Nothing
        End Sub

        Private Sub AddEmails(ByRef emails As MailManager, emailService As EmailService, action As WorkflowOptionAction, statusOut As Integer)
            If action.Data = EMailTemplates.MMSOApproved Or action.Data = EMailTemplates.MMSODenied Then
                'Find the medical officer/tech who initiated the case
                Dim wsTrackingList As List(Of WorkStatusTracking) = LookupService.GetWorkStatusTracking(RefId, ModuleType.SpecCaseMMSO)
                Dim msgSent As Boolean = False
                Dim initiatingUser As Integer? = (From w In wsTrackingList Where w.WorkflowStatus.Id = SpecCaseMMSOWorkStatus.InitiateCase Select w.CompletedBy).SingleOrDefault
                If initiatingUser.HasValue Then
                    Dim toList As StringCollection
                    toList = emailService.GetEmailListForUser(initiatingUser)
                    If toList.Count > 0 Then
                        msgSent = True
                        emails.AddTemplate(action.Data, "", toList)
                    End If
                End If
                If Not msgSent Then
                    ClientScript.RegisterStartupScript(Me.GetType, "emailmsg", "alert('Email message not sent to originator (email address not found)')", True)
                End If
            ElseIf action.Data = EMailTemplates.MMSOAwaitingAction Then
                Dim toList As StringCollection
                If (statusOut = SpecCaseMMSOWorkStatus.Approved Or statusOut = SpecCaseMMSOWorkStatus.Denied) Then
                    'add it to the email manager
                    toList = emailService.GetDistributionListByGroupSC(SpecCase.Id, action.Target)
                Else
                    toList = emailService.GetDistributionListByStatusSC(SpecCase.Id, statusOut)
                End If

                emails.AddTemplate(action.Data, "", toList)
            Else
                Throw New ApplicationException("Incorrect email type for MMSO") ' workflow not set up correctly
            End If

        End Sub

        Private Sub AddSignature(ByVal groupId As Integer)
            Dim user As AppUser = UserService.CurrentUser()
            SpecCase.AddSignature(DaoFactory, groupId, user.SignatureName, user.SignatureTitle, user)
        End Sub

#End Region

    End Class

End Namespace