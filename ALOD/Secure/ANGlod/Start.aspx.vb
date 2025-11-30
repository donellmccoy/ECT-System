Imports System.Collections.Specialized
Imports ALOD.Core.Domain.Users
Imports ALOD.Data.Services
Imports ALOD.Core.Interfaces
Imports ALOD.Core.Domain.Lookup
Imports ALOD.Core.Domain.Common
Imports ALOD.Core.Domain.Modules.Lod
Imports ALOD.Core.Domain.Modules.SARC
Imports ALOD.Data
Imports System.Data
Imports ALOD.Logging
Imports ALODWebUtility.Common
Imports ALODWebUtility.Worklfow
Imports ALOD.Core.Domain.Documents
Imports ALOD.Core.Domain.DBSign
Imports ALOD.Core.Interfaces.DAOInterfaces
Imports ALOD.Core.Domain.ServiceMembers
Imports ALOD.Core.Domain.Workflow
Imports ALOD.Core.Utils

Namespace Web.LOD
    Partial Class Secure_lod_Start
        Inherits System.Web.UI.Page

        Private _daoFactory As IDaoFactory
        Private dao As ILineOfDutyDao
        Private sdao As ISARCDAO
        Private _dao As IDocumentDao
        Private _permissionDao As IALODPermissionDao
        Private _workflowDao As IWorkflowDao
        Private _userDao As IUserDao

        Protected Const COMMAND_VIEW_LOD As String = "VIEW_LOD"

        Protected Const ACTION_CREATE As String = "create"
        Protected Const ACTION_SIGN As String = "sign"

        Protected Const URL_LOD_INIT As String = "~/Secure/Lod/init.aspx?refId="
        Protected Const URL_SARC_INIT As String = "~/Secure/SARC/init.aspx?refId="

        Protected Const PARAM_COMPO As String = "compo"
        Protected Const PARAM_TYPE As String = "type"
        Protected Const PARAM_GROUP_ID As String = "groupId"

        Protected Const KEY_ACTION_MODE As String = "action_mode"
        Protected Const KEY_REFID As String = "refId"
        Protected Const KEY_REFSTATUS As String = "refStatus"


        Private Property ActionMode() As String
            Get
                If (ViewState(KEY_ACTION_MODE) Is Nothing) Then
                    ViewState(KEY_ACTION_MODE) = ACTION_CREATE
                End If
                Return ViewState(KEY_ACTION_MODE)
            End Get
            Set(ByVal value As String)
                ViewState(KEY_ACTION_MODE) = value
            End Set
        End Property

        Private ReadOnly Property DaoFactory As IDaoFactory
            Get
                If (_daoFactory Is Nothing) THen
                    _daoFactory = New NHibernateDaoFactory()
                End If

                Return _daoFactory
            End Get
        End Property

        Private ReadOnly Property LodDao() As ILineOfDutyDao
            Get
                If (dao Is Nothing) Then
                    dao = DaoFactory.GetLineOfDutyDao()
                End If
                Return dao
            End Get
        End Property

        Protected ReadOnly Property WorkflowDao As IWorkflowDao
            Get
                If (_workflowDao Is Nothing) Then
                    _workflowDao = DaoFactory.GetWorkflowDao()
                End If

                Return _workflowDao
            End Get
        End Property

        Private ReadOnly Property SARCDao() As ISARCDAO
            Get
                If (sdao Is Nothing) Then
                    sdao = DaoFactory.GetSARCDao()
                End If
                Return sdao
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

        Protected ReadOnly Property PermissionDao As IALODPermissionDao
            Get
                If (_permissionDao Is Nothing) Then
                    _permissionDao = DaoFactory.GetPermissionDao()
                End If

                Return _permissionDao
            End Get
        End Property

        Private ReadOnly Property UserDao As IUserDao
            Get
                If (_userDao Is Nothing) Then
                    _userDao = DaoFactory.GetUserDao()
                End If

                Return _userDao
            End Get
        End Property

        Private ReadOnly Property ApplicationUser As AppUser
            Get
                Return UserDao.FindById(SessionInfo.SESSION_USER_ID)
            End Get
        End Property

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

            AddHandler SigBlock.SignCompleted, AddressOf SignatureCompleted
            AddHandler ucMemberSelectionGrid.MemberSelected, AddressOf MemberSelected

            If (Not IsPostBack) Then
                SetInputFormatRestriction(Page, SSNTextBox, FormatRestriction.Numeric)
                SetDefaultButton(SSNTextBox, LookupButton)

                If (SESSION_GROUP_ID = UserGroups.WingSarc Or SESSION_GROUP_ID = UserGroups.RSL) Then
                    'for SARC users, they can only initiate SARC cases
                    SarcYes.Checked = True
                    SarcPanel.Enabled = False
                End If

                trSSN.Visible = True
                trSSN2.Visible = True
                trName.Visible = False
            End If

            If (AppMode = DeployMode.Production) Then
                Label1.Visible = True
            Else
                WorkflowSelect.Visible = True
            End If

        End Sub

        Protected Sub SignatureCompleted(ByVal sender As Object, ByVal e As SignCompletedEventArgs)

            If (e.SignaturePassed) Then

                'Run Stored Procedure to add Rows into ReminderEmails table if setting for initiat status exist.
                Dim reminderDao = New ReminderEmailsDao()

                If (SarcYes.Checked And RestrictedYes.Checked) Then


                    If (AppMode = DeployMode.Production) Then
                        SARCDao.CommitChanges()
                        reminderDao.ReminderEmailInitialStep(ViewState(KEY_REFID), ViewState(KEY_REFSTATUS), "SARC")
                        Response.Redirect(URL_SARC_INIT + e.RefId.ToString(), True)
                    Else
                        If (WorkflowSelect.SelectedValue = 1) Then
                            LodDao.CommitChanges()
                            reminderDao.ReminderEmailInitialStep(ViewState(KEY_REFID), ViewState(KEY_REFSTATUS), "LOD")
                            Response.Redirect(URL_LOD_INIT + e.RefId.ToString(), True)
                        Else
                            SARCDao.CommitChanges()
                            reminderDao.ReminderEmailInitialStep(ViewState(KEY_REFID), ViewState(KEY_REFSTATUS), "SARC")
                            Response.Redirect(URL_SARC_INIT + e.RefId.ToString(), True)
                        End If
                    End If

                Else

                    LodDao.CommitChanges()
                    reminderDao.ReminderEmailInitialStep(ViewState(KEY_REFID), ViewState(KEY_REFSTATUS), "LOD")
                    Response.Redirect(URL_LOD_INIT + e.RefId.ToString(), True)


                End If

                NHibernateSessionManager.Instance().CommitTransaction()

            Else
                ErrorLabel.Text = Resources.Messages.ERROR_SIGNING_LOD_START
            End If

        End Sub

        Protected Sub MemberSelected(ByVal sender As Object, ByVal e As MemberSelectedEventArgs)

            Dim resultsTable As DataTable = LookupService.GetServerMembersByName(txtMemberLastName.Text, txtMemberFirstName.Text, txtMemberMiddleName.Text)
            Dim member As ServiceMember = Nothing
            Dim ssn As String = String.Empty

            ssn = resultsTable.Rows(e.SelectedRowIndex).Field(Of String)("SSN")
            member = LookupService.GetServiceMemberBySSN(ssn)

            ProcessSelectedMember(member)

        End Sub

        Protected Sub SearchSelectionRadioButton_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles rdbSSN.CheckedChanged, rdbName.CheckedChanged
            If (rdbSSN.Checked) Then
                trSSN.Visible = True
                trSSN2.Visible = True
                trName.Visible = False
            Else
                trSSN.Visible = False
                trSSN2.Visible = False
                trName.Visible = True
            End If
        End Sub

        Protected Sub LookupButton_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles LookupButton.Click
            ResetErrorLabels() 

            Dim member As ServiceMember = Nothing

            If (rdbSSN.Checked) Then
                member = LookupServiceMemberBySSN()
            ElseIf (rdbName.Checked) Then
                member = LookupServiceMemberByName()
            End If

            ProcessSelectedMember(member)
        End Sub

        Protected Sub ResetErrorLabels()
            NotFoundLabel.Visible = False
            InvalidSSNLabel.Visible = False
            InvalidLabel.Visible = False
            InvalidSSN.Visible = False
            lblMemberNotFound.Visible = False
            lblInvalidMemberName.Visible = False
            lblInvalidMemberForSSN.Visible = False
            lblInvalidMemberForName.Visible = False
        End Sub

        Private Function LookupServiceMemberBySSN() As ServiceMember
            Dim ssn As String = SSNTextBox.Text.Trim.Replace("-", "")
            Dim member As ServiceMember = Nothing

            If (Not AreEnteredSsnsValid())
                Return Nothing
            End If

            member = LookupService.GetServiceMemberBySSN(ssn)

            If (member Is Nothing) Then
                InitVisibilityOfMemberNotFoundControls()
                Return Nothing
            End If

            If (IsMemberTheUser(member))
                lblInvalidMemberForSSN.Visible = True
                Return Nothing
            End If
            
            Return member
        End Function

        Private Function AreEnteredSsnsValid() As Boolean
            Dim ssn As String = SSNTextBox.Text.Trim.Replace("-", "")
            Dim ssn2 As String = SSNTextBox2.Text.Trim.Replace("-", "")
            Dim isSsnValid As Boolean = True

            If (ssn.Length <> STRLEN_SSN) Then
                InvalidSSNLabel.Visible = True
                isSsnValid = False
            End If

            If (Not String.Equals(ssn, ssn2)) Then
                InvalidSSN.Visible = True
                isSsnValid = False
            End If

            If (Not isSsnValid) Then
                InitVisibilityOfSsnNotFoundControls()
                Return False
            End If

            Return True
        End Function

        Private Sub InitVisibilityOfSsnNotFoundControls()
            SSNLabel.Text = String.Empty
            NotFoundLabel.Visible = False
            lblInvalidMemberForSSN.Visible = False
            lblInvalidMemberForName.Visible = False
            MemberDataPanel.Visible = False
            HistoryPanel.Visible = False
        End Sub

        Private Sub InitVisibilityOfMemberNotFoundControls()
            SSNLabel.Text = String.Empty
            NotFoundLabel.Visible = True
            MemberSelectionPanel.Visible = False
            lblInvalidMemberName.Visible = False
            lblMemberNotFound.Visible = False
            InvalidSSNLabel.Visible = False
            lblInvalidMemberForSSN.Visible = False
            lblInvalidMemberForName.Visible = False
            MemberDataPanel.Visible = False
            HistoryPanel.Visible = False
        End Sub

        Private Function LookupServiceMemberByName() As ServiceMember
            Dim ssn As String = String.Empty
            Dim member As ServiceMember = Nothing

            lblInvalidMemberName.Visible = False
            lblMemberNotFound.Visible = False
            lblInvalidMemberForSSN.Visible = False
            lblInvalidMemberForName.Visible = False

            If (Not IsEnteredMemberNameValid()) Then
                lblInvalidMemberName.Visible = True
                Return Nothing
            End If

            Dim resultsTable As DataTable = GetServiceMembersByName(txtMemberLastName.Text, txtMemberFirstName.Text, txtMemberMiddleName.Text)

            If (resultsTable.Rows.Count > 1) Then
                ucMemberSelectionGrid.Initialize(resultsTable)
                MemberSelectionPanel.Visible = True
            ElseIf (resultsTable.Rows.Count = 1) Then
                ssn = resultsTable.Rows(0).Field(Of String)("SSN")
                member = LookupService.GetServiceMemberBySSN(ssn)
            Else
                lblMemberNotFound.Visible = True
                MemberSelectionPanel.Visible = False
            End If

            If (IsMemberTheUser(member))
                lblInvalidMemberForName.Visible = True
                Return Nothing
            End If

            Return member
        End Function

        Private Function IsEnteredMemberNameValid()
            If (String.IsNullOrEmpty(txtMemberLastName.Text) AndAlso 
                String.IsNullOrEmpty(txtMemberFirstName.Text) AndAlso 
                String.IsNullOrEmpty(txtMemberMiddleName.Text)) Then
                Return False
            End If

            Return True
        End Function

        Private Function GetServiceMembersByName(lastName As String, firstName As String, middleName As String) As DataTable
            Dim resultsTable As DataTable = LookupService.GetServerMembersByName(lastName, firstName, middleName)
            Dim rowToRemove As DataRow = GetRowWithAppUserInformation(resultsTable)

            If (rowToRemove IsNot Nothing) Then
                resultsTable.Rows.Remove(rowToRemove)
            End If

            Return resultsTable
        End Function

        Private Function GetRowWithAppUserInformation(table As DataTable) As DataRow
            If (table Is Nothing)
                Return Nothing
            End If

            For Each row As DataRow In table.Rows
                If (IsMemberTheUser(row)) Then
                    Return row
                End If
            Next

            Return Nothing
        End Function

        Private Function IsMemberTheUser(row As Datarow) As Boolean
            Dim member = New ServiceMember(DataHelpers.GetStringFromDataRow("SSN", row))
            member.LastName = DataHelpers.GetStringFromDataRow("LastName", row)
            member.FirstName = DataHelpers.GetStringFromDataRow("FirstName", row)
            member.MiddleName = DataHelpers.GetStringFromDataRow("MiddleName", row)

            Return IsMemberTheUser(member)
        End Function

        Private Function IsMemberTheUser(member As ServiceMember) As Boolean
            Return ApplicationUser.IsPersonnalServiceMemberRecord(member)
        End Function

        Private Sub ProcessSelectedMember(ByRef member As ServiceMember)
            If (member Is Nothing)
                Exit Sub
            End If

            InitVisibilityOfMemberFoundControls()
            InitMemberInformationControls(member)
            InitMemberCaseHistoryGridView(member)
        End Sub

        Private Sub InitVisibilityOfMemberFoundControls()
            InvalidSSNLabel.Visible = False
            NotFoundLabel.Visible = False
            InputPanel.Visible = False
            MemberSelectionPanel.Visible = False
            lblInvalidMemberName.Visible = False
            lblMemberNotFound.Visible = False
            lblInvalidMemberForSSN.Visible = False
            lblInvalidMemberForName.Visible = False
            MemberDataPanel.Visible = True
            HistoryPanel.Visible = True
        End Sub

        Private Sub InitMemberInformationControls(member As ServiceMember)
            SSNLabel.Text = Server.HtmlEncode(member.SSN)
            NameLabel.Text = member.FullName.ToUpper()

            If member.Unit IsNot Nothing Then
                UnitNameLabel.Text = Server.HtmlEncode(member.Unit.Name)
                UnitCodeLabel.Text = Server.HtmlEncode(member.Unit.PasCode)
                UnitIdLabel.Text = Server.HtmlEncode(member.Unit.Id)
            End If

            If (member.AttachUnit IsNot Nothing) Then
                AttachedUnitNameLabel.Text = Server.HtmlEncode(member.AttachUnit.Name)
                AttachedUnitCodeLabel.Text = Server.HtmlEncode(member.AttachUnit.PasCode)
                AttachedUnitIdLabel.Text = Server.HtmlEncode(member.AttachUnit.Id)
            End If

            If (member.DateOfBirth.HasValue) Then
                DoBLabel.Text = Server.HtmlEncode(member.DateOfBirth.Value.ToString(DATE_FORMAT))
            Else
                DoBLabel.Text = Nothing
            End If

            If member.Rank IsNot Nothing Then
                GradeCodeLabel.Text = Server.HtmlEncode(member.Rank.Id)
                RankLabel.Text = Server.HtmlEncode(member.Rank.Title)
            End If

            CompoLabel.Text = Server.HtmlEncode(member.Component)
        End Sub

        Private Sub InitMemberCaseHistoryGridView(member As ServiceMember)
            If (member Is Nothing) Then
                Exit Sub
            End If
            
            HistoryGrid.DataSource = LodService.GetCaseHistory(member.SSN, SESSION_USER_ID, SESSION_REPORT_VIEW, SESSION_COMPO, SESSION_UNIT_ID, UserHasPermission(PERMISSION_VIEW_SARC_CASES), False)
            HistoryGrid.DataBind()
        End Sub

        Protected Sub WorkflowData_Selecting(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.ObjectDataSourceSelectingEventArgs) Handles WorkflowData.Selecting
            e.InputParameters(PARAM_COMPO) = SESSION_COMPO
            e.InputParameters(PARAM_TYPE) = ModuleType.LOD
            e.InputParameters(PARAM_GROUP_ID) = SESSION_GROUP_ID
        End Sub

        Protected Sub CreateButton_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles CreateButton.Click
            ErrorLabel.Text = String.Empty

            If (ActionMode = ACTION_SIGN) Then
                'we are just signing, don't create a new one
                SigBlock.StartSignature(CInt(ViewState(KEY_REFID)), 0, 0, "Initiate LOD",
                                        ViewState(KEY_REFSTATUS), ViewState(KEY_REFSTATUS),
                                        0, DBSignTemplateId.Form348, "")
                Exit Sub
            End If

            If (Not ValidateMemberData()) Then
                Exit Sub
            End If

            If (Not SarcYes.Checked AndAlso Not SarcNo.Checked) Then
                AddCssClass(SarcPanel, CSS_FIELD_REQUIRED)
                Exit Sub
            Else
                RemoveCssClass(SarcPanel, CSS_FIELD_REQUIRED)
            End If

            If (Not TmnYes.Checked AndAlso Not TmnNo.Checked) Then
                AddCssClass(TmnPanel, CSS_FIELD_REQUIRED)
                Exit Sub
            Else
                RemoveCssClass(TmnPanel, CSS_FIELD_REQUIRED)
            End If

            If (Not TmsYes.Checked AndAlso Not TmsNo.Checked) Then
                AddCssClass(TmsPanel, CSS_FIELD_REQUIRED)
                Exit Sub
            Else
                RemoveCssClass(TmsPanel, CSS_FIELD_REQUIRED)
            End If

            If (SarcYes.Checked) Then
                If (Not RestrictedYes.Checked AndAlso Not RestrictedNo.Checked) Then
                    AddCssClass(RestrictedPanel, CSS_FIELD_REQUIRED)
                    Exit Sub
                Else
                    RemoveCssClass(RestrictedPanel, CSS_FIELD_REQUIRED)
                End If

                If (RestrictedYes.Checked) Then

                    If (Not CheckRestrictedSarcPermission()) Then
                        ErrorLabel.Text = Resources.Messages.ERROR_RSARC_REQUIRED
                        Exit Sub
                    End If

                    If (AppMode = DeployMode.Production) Then
                        CreateSARC()
                    Else
                        If (WorkflowSelect.SelectedValue = 1) Then
                            CreateLOD()
                        Else
                            CreateSARC()
                        End If
                    End If


                Else
                    If (Not CheckSarcPermission()) Then
                        ErrorLabel.Text = Resources.Messages.ERROR_SARC_REQUIRED
                        Exit Sub
                    End If

                    CreateLOD()
                End If
            Else
                CreateLOD()
            End If
        End Sub

        Private Sub CreateSARC()

            'the user has permission. make sure it's not the same unit
            Dim ssn As String = SSNLabel.Text.Trim.Replace("-", "")
            Dim member As ServiceMember = LookupService.GetServiceMemberBySSN(ssn)

            If (UserService.CurrentUser().CurrentRole.Group.Id = UserGroups.MedTechSARC) Then
                If (UserService.CurrentUser().Unit.PasCode Like member.Unit.PasCode) Then
                    ErrorLabel.Text = Resources.Messages.ERROR_SARC_SAME_UNIT
                    Exit Sub
                End If
            End If

            Dim sarc As RestrictedSARC

            sarc = New RestrictedSARC()

            sarc.Workflow = WorkflowDao.GetWorkflowFromModule(ModuleType.SARC)
            sarc.CaseId = "sarc"
            sarc.MemberSSN = SSNLabel.Text
            sarc.MemberName = NameLabel.Text
            sarc.MemberRank.SetId(CInt(GradeCodeLabel.Text))
            sarc.MemberUnit = UnitNameLabel.Text
            sarc.MemberUnitId = UnitIdLabel.Text
            sarc.MemberCompo = CompoLabel.Text

            If (DoBLabel.Text.Length > 0) Then
                sarc.MemberDOB = DateTime.Parse(DoBLabel.Text)
            End If

            sarc.CreatedBy = SESSION_USER_ID
            sarc.CreatedDate = Now
            sarc.ModifiedBy = sarc.CreatedBy
            sarc.ModifiedDate = Now

            sarc.Status = LookupService.GetInitialStatus(SESSION_USER_ID, SESSION_GROUP_ID, sarc.Workflow)
            sarc.CreateDocumentGroup(DocumentDao)

            ' sarc.FromUnit = UserService.CurrentUser.Unit.Name

            SARCDao.Save(sarc)
            If (sarc.Id > 0) Then
                LogManager.LogAction(ModuleType.SARC, UserAction.InitiatedSARC,
                                             sarc.Id, "Workflow: Restricted SARC", sarc.Status)
                ActionMode = ACTION_SIGN
                ViewState(KEY_REFID) = sarc.Id
                ViewState(KEY_REFSTATUS) = sarc.Status
                SigBlock.StartSignature(sarc.Id, sarc.Workflow, 0, "Initiate Restricted SARC", sarc.Status, sarc.Status, 0, DBSignTemplateId.Form348SARC, "")

            Else
                ActionMode = ACTION_CREATE
                ValidationList.DataSource = New String() {"An error occured initiating the Restricted SARC"}
                ValidationList.DataBind()
                ValidationList.Visible = True
            End If
        End Sub

        Private Sub CreateLOD()
            Dim lod As LineOfDuty

            'everything checks out,start the LOD
            If (AppMode = DeployMode.Production) Then
                lod = New LineOfDuty_v2()
                lod.Workflow = WorkflowDao.GetWorkflowFromModule(ModuleType.LOD)
            Else
                If (WorkflowSelect.SelectedValue = 1) Then
                    lod = New LineOfDuty()
                    lod.Workflow = WorkflowSelect.SelectedValue
                Else
                    lod = New LineOfDuty_v2()
                    lod.Workflow = WorkflowSelect.SelectedValue
                End If
            End If

            Dim Unitdao As ILineOfDutyUnitDao = DaoFactory.GetLineOfDutyUnitDao()

            lod.MemberSSN = SSNLabel.Text
            lod.MemberName = NameLabel.Text
            lod.MemberRank.SetId(CInt(GradeCodeLabel.Text))
            lod.MemberUnit = UnitNameLabel.Text
            lod.MemberUnitId = UnitIdLabel.Text
            lod.MemberCompo = CompoLabel.Text

            lod.CreateDocumentGroup(DocumentDao)

            lod.SarcCase = SarcYes.Checked
            lod.IsRestricted = RestrictedYes.Checked
            lod.TimelyMemberNotify = TmnYes.Checked
            lod.TimelyMemberSubmit = TmsYes.Checked

            If (DoBLabel.Text.Length > 0) Then
                lod.MemberDob = DateTime.Parse(DoBLabel.Text)
            End If

            lod.CreatedBy = SESSION_USER_ID
            lod.CreatedDate = Now
            lod.ModifiedBy = lod.CreatedBy
            lod.ModifiedDate = Now

            lod.Status = LookupService.GetInitialStatus(SESSION_USER_ID, SESSION_GROUP_ID, lod.Workflow)
            lod.FromUnit = UserService.CurrentUser.Unit.Name

            'Attach PAS CODE
            Dim AttachUnitId As Integer
            Dim isAttachUnit As Boolean = Int32.TryParse(AttachedUnitIdLabel.Text, AttachUnitId)

            If (lod.MemberUnitId <> AttachUnitId) And (isAttachUnit) Then
                lod.isAttachPas = isAttachUnit
                lod.MemberAttachedUnitId = AttachUnitId
            Else
                lod.isAttachPas = False
            End If


            LodDao.Save(lod)

            If (lod.Id > 0) Then
                LogManager.LogAction(ModuleType.LOD, UserAction.InitiatedLOD,
                                     lod.Id, "Workflow: Line Of Duty", lod.Status)
                ActionMode = ACTION_SIGN
                ViewState(KEY_REFID) = lod.Id
                ViewState(KEY_REFSTATUS) = lod.Status
                SigBlock.StartSignature(lod.Id, lod.Workflow, 0, "Initiate LOD", lod.Status, lod.Status, 0, DBSignTemplateId.Form348, "")

            Else
                ActionMode = ACTION_CREATE
                ValidationList.DataSource = New String() {"An error occured initiating the LOD"}
                ValidationList.DataBind()
                ValidationList.Visible = True
            End If
        End Sub

        Private Function CheckSarcPermission() As Boolean
            Dim roles = ApplicationUser.AllRoles

            For Each role In roles
                Dim perms = PermissionDao.GetPermissionsByGroupId(role.Group.Id)

                For Each perm In perms
                    If perm.Name.Equals(PERMISSION_CREATE_SARC_UNRESTRICTED) Then
                        Return True
                    End If
                Next
            Next

            Return False
        End Function

        Private Function CheckRestrictedSarcPermission() As Boolean
            Dim roles = ApplicationUser.AllRoles

            For Each role In roles
                Dim perms = PermissionDao.GetPermissionsByGroupId(role.Group.Id)

                For Each perm In perms
                    If perm.Name.Equals(PERMISSION_CREATE_SARC) Then
                        Return True
                    End If
                Next
            Next

            Return False
        End Function

        Private Function ValidateMemberData() As Boolean
            Dim errors As New StringCollection

            If SSNLabel.Text.Trim.Length = 0 Then
                errors.Add(Resources.Messages.REQUIRED_FIELD_MEMBER_SSN)
            End If

            If NameLabel.Text.Trim.Length = 0 Then
                errors.Add(Resources.Messages.REQUIRED_FIELD_MEMBER_NAME)
            End If

            If DoBLabel.Text.Trim.Length = 0 Then
                errors.Add(Resources.Messages.REQUIRED_FIELD_MEMBER_DOB)
            End If

            If UnitIdLabel.Text.Trim.Length = 0 Then
                errors.Add(Resources.Messages.REQUIRED_FIELD_MEMBER_UNIT)
            End If

            If CompoLabel.Text.Trim.Length = 0 Then
                errors.Add(Resources.Messages.REQUIRED_FIELD_INVALID_COMPO)
            End If

            If GradeCodeLabel.Text.Trim.Length = 0 Then
                errors.Add(Resources.Messages.REQUIRED_FIELD_INVALID_GRADE)
            End If

            If (errors.Count > 0) Then
                ErrorPanel.Visible = True
                ValidationList.Visible = True
                ValidationList.DataSource = errors
                ValidationList.DataBind()
                Return False
            Else
                ErrorPanel.Visible = False
            End If

            ValidationList.Visible = False
            Return True
        End Function

        Protected Sub ChangeSsnButton_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles ChangeSsnButton.Click
            NotFoundLabel.Visible = False
            InvalidSSNLabel.Visible = False
            InputPanel.Visible = True
            MemberDataPanel.Visible = False
            HistoryPanel.Visible = False
            SigBlock.ClearSignatureFrame()
        End Sub

        Protected Sub Page_PreRender(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.PreRender
            ErrorRow.Visible = (ErrorLabel.Text.Length > 0)
        End Sub
    End Class
End Namespace
