Imports System.Reflection
Imports System.Runtime.Remoting.Messaging
Imports ALOD.Core.Domain.DBSign
Imports ALOD.Core.Domain.Lookup
Imports ALOD.Core.Domain.Modules.SpecialCases
Imports ALOD.Core.Domain.ServiceMembers
Imports ALOD.Core.Domain.Users
Imports ALOD.Core.Domain.Workflow
Imports ALOD.Core.Interfaces.DAOInterfaces
Imports ALOD.Core.Utils
Imports ALOD.Data
Imports ALOD.Data.Services
Imports ALOD.Logging
Imports ALODWebUtility.Common
Imports ALODWebUtility.Permission.Search
Imports ALODWebUtility.Worklfow

Namespace Web.Special_Case

    Partial Class Secure_sc_ci_Start
        Inherits System.Web.UI.Page

#Region "Fields..."

        Protected Const ACTION_CREATE As String = "create"
        Protected Const ACTION_SIGN As String = "sign"
        Protected Const COMMAND_VIEW_LOD As String = "VIEW_LOD"
        Protected Const KEY_ACTION_MODE As String = "action_mode"
        Protected Const KEY_REFID As String = "refId"
        Protected Const KEY_REFSTATUS As String = "refStatus"
        Protected Const PARAM_COMPO As String = "compo"
        Protected Const PARAM_GROUP_ID As String = "groupId"
        Protected Const PARAM_TYPE As String = "type"
        Protected CURR_MODULE As String = ""
        Protected CURR_MODULE_ID As Integer = 0
        Protected URL_SC_INIT As String = ""
        Private Const ASCENDING As String = "Asc"
        Private Const DESCENDING As String = "Desc"
        Private Const MEMBER_SSN_KEY As String = "MemberSSN"
        Private Const SORT_COLUMN_KEY_LOd As String = "_LODSortExp_"
        Private Const SORT_COLUMN_KEY_SC As String = "_SCSortExp_"
        Private Const SORT_DIR_KEY_LOD As String = "_LODSortDirection_"
        Private Const SORT_DIR_KEY_SC As String = "_SCSortDirection_"
        Private _case As IAssociatedCaseDao
        Private _caseTypeDao As ICaseTypeDao
        Private _daoFactory As IDaoFactory
        Private _reminderEmailsDao As IReminderEmailDao
        Private _userDao As IUserDao
        Private _workflowDao As IWorkflowDao
        Private _workstatusDao As IWorkStatusDao
        Private dao As ILineOfDutyDao
        Private lkup As ILookupDao
        Private scdao As ISpecialCaseDAO

        Public Delegate Function GetPALDataDelegate(ByVal param As StringDictionary) As DataSet

        Public Delegate Function GetSpecialCasesDelegate(ByVal param As StringDictionary) As DataSet

        Public Delegate Function GetUnDeletedLodsDelegate(ByVal param As StringDictionary) As DataSet

#End Region

#Region "Properties..."

        ReadOnly Property ModuleId() As Integer
            Get
                Return CInt(Request.QueryString("mid"))
            End Get
        End Property

        Protected ReadOnly Property DaoFactory() As IDaoFactory
            Get
                If (_daoFactory Is Nothing) Then
                    _daoFactory = New NHibernateDaoFactory()
                End If

                Return _daoFactory
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

        Private ReadOnly Property ApplicationUser As AppUser
            Get
                Return UserDao.FindById(SessionInfo.SESSION_USER_ID)
            End Get
        End Property

        Private ReadOnly Property AssociatedDao() As IAssociatedCaseDao
            Get
                If (_case Is Nothing) Then
                    _case = DaoFactory.GetAssociatedCaseDao()
                End If
                Return _case
            End Get
        End Property

        Private ReadOnly Property CaseTypeDao() As ICaseTypeDao
            Get
                If (_caseTypeDao Is Nothing) Then
                    _caseTypeDao = DaoFactory.GetCaseTypeDao()
                End If

                Return _caseTypeDao
            End Get
        End Property

        Private ReadOnly Property LOD_Dao() As ILineOfDutyDao
            Get
                If (dao Is Nothing) Then
                    dao = DaoFactory.GetLineOfDutyDao()
                End If
                Return dao
            End Get
        End Property

        Private ReadOnly Property Lookup_Dao() As ILookupDao
            Get
                If (lkup Is Nothing) Then
                    lkup = DaoFactory.GetLookupDao()
                End If
                Return lkup
            End Get
        End Property

        Private ReadOnly Property ReminderEmailDao() As IReminderEmailDao
            Get
                If (_reminderEmailsDao Is Nothing) Then
                    _reminderEmailsDao = DaoFactory.GetReminderEmailDao()
                End If

                Return _reminderEmailsDao
            End Get
        End Property

        Private ReadOnly Property SC_Dao() As ISpecialCaseDAO
            Get
                If (scdao Is Nothing) Then
                    scdao = DaoFactory.GetSpecialCaseDAO()
                End If
                Return scdao
            End Get
        End Property

        Private Property SortColumnLOD() As String
            Get
                Return ViewState(SORT_COLUMN_KEY_LOd)
            End Get
            Set(ByVal value As String)
                ViewState(SORT_COLUMN_KEY_LOd) = value
            End Set
        End Property

        Private Property SortColumnSC() As String
            Get
                Return ViewState(SORT_COLUMN_KEY_SC)
            End Get
            Set(ByVal value As String)
                ViewState(SORT_COLUMN_KEY_SC) = value
            End Set
        End Property

        Private Property SortDirectionLOD() As String
            Get
                If (ViewState(SORT_DIR_KEY_LOD) Is Nothing) Then
                    ViewState(SORT_DIR_KEY_LOD) = ASCENDING
                End If
                Return ViewState(SORT_DIR_KEY_LOD)
            End Get
            Set(ByVal value As String)
                ViewState(SORT_DIR_KEY_LOD) = value
            End Set
        End Property

        Private Property SortDirectionSC() As String
            Get
                If (ViewState(SORT_DIR_KEY_SC) Is Nothing) Then
                    ViewState(SORT_DIR_KEY_SC) = ASCENDING
                End If
                Return ViewState(SORT_DIR_KEY_SC)
            End Get
            Set(ByVal value As String)
                ViewState(SORT_DIR_KEY_SC) = value
            End Set
        End Property

        Private ReadOnly Property SortExpressionLOD() As String
            Get
                Return SortColumnLOD + " " + SortDirectionLOD
            End Get
        End Property

        Private ReadOnly Property SortExpressionSC() As String
            Get
                Return SortColumnSC + " " + SortDirectionSC
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

        Private ReadOnly Property WorkStatusDao() As IWorkStatusDao
            Get
                If (_workstatusDao Is Nothing) Then
                    _workstatusDao = DaoFactory.GetWorkStatusDao()
                End If

                Return _workstatusDao
            End Get
        End Property

#End Region

#Region "Page Methods..."

        '''<summary>
        '''<para>The method creates  GetSpecialCasesDelegate for  GetSpecialCases method and invokes it asynchronously
        ''' </para>
        '''<param name="sender"> objcet</param>
        '''<param name="e"> EventArgs </param>
        '''<param name="cb">AsyncCallback </param>
        '''<param name="state">Object </param>
        '''</summary>
        ''' <returns>IAsyncResult</returns>
        Function BeginAsyncOperation(ByVal sender As Object, ByVal e As EventArgs, ByVal cb As AsyncCallback, ByVal state As Object) As IAsyncResult
            Dim asynchDelegate As GetSpecialCasesDelegate = New GetSpecialCasesDelegate(AddressOf GetSpecialCases)
            Dim param As StringDictionary = CType(state, StringDictionary)
            Dim r As IAsyncResult = asynchDelegate.BeginInvoke(param, cb, state)
            Return r
        End Function

        '''<summary>
        '''<para>The method creates  GetSpecialCasesDelegate for  GetSpecialCases method and invokes it asynchronously
        ''' </para>
        '''<param name="sender"> objcet</param>
        '''<param name="e"> EventArgs </param>
        '''<param name="cb">AsyncCallback </param>
        '''<param name="state">Object </param>
        '''</summary>
        ''' <returns>IAsyncResult</returns>
        Function BeginAsyncOperationLOD(ByVal sender As Object, ByVal e As EventArgs, ByVal cb As AsyncCallback, ByVal state As Object) As IAsyncResult
            Dim asynchDelegateLOD As GetUnDeletedLodsDelegate = New GetUnDeletedLodsDelegate(AddressOf GetUnDeletedLods)
            Dim param As StringDictionary = CType(state, StringDictionary)
            Dim r As IAsyncResult = asynchDelegateLOD.BeginInvoke(param, cb, state)
            Return r
        End Function

        '''<summary>
        '''<para>The method is the call back method which is called when the  GetUnDeletedLods is complete.
        ''' Upon completion History Grid's inner html is populated with the results
        '''</para>
        '''<param name="a"> IAsyncResult</param>
        '''</summary>
        Public Sub EndAsyncOperation(ByVal a As IAsyncResult)
            Dim result As AsyncResult = CType(a, AsyncResult)
            Dim caller As GetSpecialCasesDelegate = CType(result.AsyncDelegate, GetSpecialCasesDelegate)
            Dim history As DataSet = caller.EndInvoke(a)
            If history.Tables.Count = 0 Then
                HistoryGrid.Visible = False
                NoHistoryLbl.Visible = True
            Else
                HistoryGrid.Visible = True
                NoHistoryLbl.Visible = False
                HistoryGrid.DataSource = history
                HistoryGrid.DataBind()

            End If
            Session("HistoryGridSC") = history
        End Sub

        '''<summary>
        '''<para>The method is the call back method which is called when the  GetUnDeletedLods is complete.
        ''' Upon completion History Grid's inner html is populated with the results
        '''</para>
        '''<param name="a"> IAsyncResult</param>
        '''</summary>
        Public Sub EndAsyncOperationLOD(ByVal a As IAsyncResult)
            Dim result As AsyncResult = CType(a, AsyncResult)
            Dim caller As GetUnDeletedLodsDelegate = CType(result.AsyncDelegate, GetUnDeletedLodsDelegate)
            Dim history As DataSet = caller.EndInvoke(a)
            If history.Tables.Count = 0 Then
                HistoryGridLOD.Visible = False
                NoHistoryLODLbl.Visible = True
            Else
                HistoryGridLOD.Visible = True
                NoHistoryLODLbl.Visible = False
                HistoryGridLOD.DataSource = history
                HistoryGridLOD.DataBind()
            End If
            Session("HistoryGridLOD") = history
        End Sub

        Public Function GetPALData(ByVal param As StringDictionary) As DataSet
            Return LodService.GetPALDataByMemberSSN(SSNLabel.Text, "")
        End Function

        Public Function GetSpecialCases(ByVal param As StringDictionary) As DataSet
            'Return LodService.GetSpecialCasesByMemberSSN(SSNTextBox.Text, SESSION_USER_ID)
            Return LodService.GetSpecialCasesByMemberSSN(param(MEMBER_SSN_KEY), SESSION_USER_ID)
        End Function

        Public Function GetUnDeletedLods(ByVal param As StringDictionary) As DataSet
            'Return LodService.GetUndeletedlLods("", SSNTextBox.Text, "", 0, Integer.Parse(param(SESSIONKEY_USER_ID)), Byte.Parse(param(SESSIONKEY_REPORT_VIEW)), param(SESSIONKEY_COMPO), 0, ModuleType.LOD, Nothing, Integer.Parse(param(SESSIONKEY_UNIT_ID)), Boolean.Parse(param(SESSIONKEY_SARC_PERMSSION)), True)
            Return LodService.GetUndeletedlLods("", param(MEMBER_SSN_KEY), "", 0, Integer.Parse(param(SESSIONKEY_USER_ID)), Byte.Parse(param(SESSIONKEY_REPORT_VIEW)), param(SESSIONKEY_COMPO), 0, ModuleType.LOD, Nothing, Integer.Parse(param(SESSIONKEY_UNIT_ID)), Boolean.Parse(param(SESSIONKEY_SARC_PERMSSION)), True)
        End Function

        Sub SetRedirectURL()
            Select Case CURR_MODULE_ID
                Case ModuleType.SpecCaseBCMR
                    URL_SC_INIT = "~/Secure/SC_BCMR/init.aspx?refId="
                Case ModuleType.SpecCaseBMT
                    URL_SC_INIT = "~/Secure/SC_BMT/init.aspx?refId="
                Case ModuleType.SpecCaseCMAS
                    URL_SC_INIT = "~/Secure/SC_CMAS/init.aspx?refId="
                Case ModuleType.SpecCaseCongress
                    URL_SC_INIT = "~/Secure/SC_Congress/init.aspx?refId="
                Case ModuleType.SpecCaseFT
                    URL_SC_INIT = "~/Secure/SC_FastTrack/init.aspx?refId="
                Case ModuleType.SpecCaseIncap
                    URL_SC_INIT = "~/Secure/SC_Incap/init.aspx?refId="
                Case ModuleType.SpecCaseMEB
                    URL_SC_INIT = "~/Secure/SC_MEB/init.aspx?refId="
                Case ModuleType.SpecCaseMEPS
                    URL_SC_INIT = "~/Secure/SC_MEPS/init.aspx?refId="
                Case ModuleType.SpecCasePW
                    URL_SC_INIT = "~/Secure/SC_PWaivers/init.aspx?refId="
                Case ModuleType.SpecCaseWWD
                    URL_SC_INIT = "~/Secure/SC_WWD/init.aspx?refId="
                Case ModuleType.SpecCaseMMSO
                    URL_SC_INIT = "~/Secure/SC_MMSO/init.aspx?refId="
                Case ModuleType.SpecCaseMH
                    URL_SC_INIT = "~/Secure/SC_MH/init.aspx?refId="
                Case ModuleType.SpecCaseNE
                    URL_SC_INIT = "~/Secure/SC_NE/init.aspx?refId="
                Case ModuleType.SpecCaseDW
                    URL_SC_INIT = "~/Secure/SC_DW/init.aspx?refId="
                Case ModuleType.SpecCaseMO
                    URL_SC_INIT = "~/Secure/SC_MO/init.aspx?refId="
                Case ModuleType.SpecCasePEPP
                    URL_SC_INIT = "~/Secure/SC_PEPP/init.aspx?refId="
                Case ModuleType.SpecCaseRS
                    URL_SC_INIT = "~/Secure/SC_RS/init.aspx?refId="
                Case ModuleType.SpecCaseRW
                    URL_SC_INIT = "~/Secure/SC_RW/init.aspx?refId="
                Case ModuleType.SpecCaseAGR
                    URL_SC_INIT = "~/Secure/SC_AGRCert/init.aspx?refId="
                Case ModuleType.SpecCasePSCD
                    URL_SC_INIT = "~/Secure/SC_PSCD/init.aspx?refId="

            End Select
        End Sub

        Protected Sub ChangeSsnButton_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles ChangeSsnButton.Click
            NotFoundLabel.Visible = False
            InvalidSSNLabel.Visible = False
            InputPanel.Visible = True
            MemberDataPanel.Visible = False
            HistoryPanelLOD.Visible = False
            HistoryPanel.Visible = False
            SigBlock.ClearSignatureFrame()
        End Sub

        Protected Function CreateAGRWorkflow(ByVal baseSpecCase As SpecialCase, ByVal validationErrors As List(Of String)) As Integer
            Dim newId As Integer = 0
            Dim sc As New SC_AGRCert

            sc.CopyBaseObjectInstance(baseSpecCase)
            SC_Dao.Evict(baseSpecCase)

            SC_Dao.Save(sc)
            newId = sc.Id

            Return newId
        End Function

        Protected Function CreateBaseSpecCaseObject(ByVal workflowId As Integer) As SpecialCase
            Dim baseSpecCase As New SpecialCase

            baseSpecCase.MemberSSN = SSNLabel.Text
            baseSpecCase.MemberName = NameLabel.Text
            baseSpecCase.MemberUnit = UnitNameLabel.Text
            baseSpecCase.MemberCompo = CompoLabel.Text
            baseSpecCase.MemberUnitId = UnitIdLabel.Text
            baseSpecCase.MemberDOB = DoBLabel.Text
            baseSpecCase.MemberRank = LookupService.GetRank(GradeCodeLabel.Text)
            baseSpecCase.moduleId = CURR_MODULE_ID
            baseSpecCase.CreatedBy = SESSION_USER_ID
            baseSpecCase.CreatedDate = DateTime.Now
            baseSpecCase.ModifiedBy = baseSpecCase.CreatedBy
            baseSpecCase.ModifiedDate = baseSpecCase.CreatedDate
            baseSpecCase.Workflow = workflowId
            baseSpecCase.Status = LookupService.GetInitialStatus(SESSION_USER_ID, SESSION_GROUP_ID, workflowId)
            baseSpecCase.SubWorkflowType = 0
            'If (baseSpecCase.CurrentStatusCode) Then
            '    baseSpecCase.CurrentStatusCode = baseSpecCase.Status
            'End If

            Return baseSpecCase
        End Function

        Protected Function CreateBCMRWorkflow(ByVal baseSpecCase As SpecialCase, ByVal validationErrors As List(Of String)) As Integer
            Dim newId As Integer = 0
            Dim sc As New SC_BCMR

            sc.CopyBaseObjectInstance(baseSpecCase)
            SC_Dao.Evict(baseSpecCase)

            SC_Dao.Save(sc)
            newId = sc.Id

            Return newId
        End Function

        Protected Function CreateBMTWorkflow(ByVal baseSpecCase As SpecialCase, ByVal validationErrors As List(Of String)) As Integer
            Dim newId As Integer = 0
            Dim sc As New SC_BMT

            sc.CopyBaseObjectInstance(baseSpecCase)
            SC_Dao.Evict(baseSpecCase)

            If ddlSubModule.SelectedValue > 0 Then
                sc.SubWorkflowType = ddlSubModule.SelectedValue

                If sc.SubWorkflowType = 1 Then
                    Session("SubWorkflowType") = "(BMT) "
                Else
                    Session("SubWorkflowType") = "(MEPS) "
                End If
            End If

            SC_Dao.Save(sc)
            newId = sc.Id

            Return newId
        End Function

        Protected Sub CreateButton_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles CreateButton.Click
            Dim validationErrors As List(Of String) = New List(Of String)()
            Dim ValidationString As String = ""
            ErrorLabel.Text = ValidationString
            ValidationErrorsRow.Visible = False

            If (ActionMode = ACTION_SIGN) Then
                'we are just signing, don't create a new one
                SigBlock.StartSignature(CInt(ViewState(KEY_REFID)), 0, 0, "Initiate Special Case: " & CURR_MODULE,
                                        ViewState(KEY_REFSTATUS), ViewState(KEY_REFSTATUS),
                                        0, DBSignTemplateId.SignOnly, "")
                Exit Sub
            End If

            'validate the member
            If (Not ValidateMemberData()) Then
                Exit Sub
            End If

            'check workflow
            Dim workflow As Integer = 0
            Integer.TryParse(ddlWorkflow.SelectedValue, workflow)

            If (workflow = 0) Then
                ErrorLabel.Text = "Please select a Workflow"
                AddCssClass(ddlWorkflow, CSS_FIELD_REQUIRED)
                Exit Sub
            End If

            'everything checks out,start the Special Case
            Dim currStatus As Integer
            Dim newId As Integer = 0
            Dim sc As SpecialCase = CreateBaseSpecCaseObject(workflow)

            currStatus = sc.Status

            Select Case workflow
                Case AFRCWorkflows.SpecCaseIncap
                    newId = CreateIncapWorkflow(sc, validationErrors)

                Case AFRCWorkflows.SpecCaseMMSO
                    newId = CreateMMSOWorkflow(sc, validationErrors)

                Case AFRCWorkflows.SpecCaseCongress
                    newId = CreateCongressWorkflow(sc, validationErrors)

                Case AFRCWorkflows.SpecCaseCMAS
                    newId = CreateCMASWorkflow(sc, validationErrors)

                Case AFRCWorkflows.SpecCaseBMT
                    newId = CreateBMTWorkflow(sc, validationErrors)

                Case AFRCWorkflows.SpecCaseBCMR
                    newId = CreateBCMRWorkflow(sc, validationErrors)

                Case AFRCWorkflows.SpecCasePW
                    newId = CreatePWWorkflow(sc, validationErrors)

                Case AFRCWorkflows.SpecCaseFT
                    newId = CreateIRILOWorkflow(sc, validationErrors)

                Case AFRCWorkflows.SpecCaseWWD
                    newId = CreateWWDWorkflow(sc, validationErrors)

                Case AFRCWorkflows.SpecCaseMEB
                    newId = CreateMEBWorkflow(sc, validationErrors)

                Case AFRCWorkflows.SpecCaseMEPS
                    newId = 0

                Case AFRCWorkflows.SpecCaseMH
                    newId = CreateMHWorkflow(sc, validationErrors)

                Case AFRCWorkflows.SpecCaseNE
                    newId = CreateNEWorkflow(sc, validationErrors)

                Case AFRCWorkflows.SpecCaseDW
                    newId = CreateDWWorkflow(sc, validationErrors)

                Case AFRCWorkflows.SpecCaseMO
                    newId = CreateMOWorkflow(sc, validationErrors)

                Case AFRCWorkflows.SpecCasePEPP
                    newId = CreatePEPPWorkflow(sc, validationErrors)

                Case AFRCWorkflows.SpecCaseRS
                    newId = CreateRSWorkflow(sc, validationErrors)

                Case AFRCWorkflows.SpecCaseRW
                    newId = CreateRWWorkflow(sc, validationErrors)

                Case AFRCWorkflows.SpecCaseAGR
                    newId = CreateAGRWorkflow(sc, validationErrors)

                Case AFRCWorkflows.SpecCasePSCD
                    'sc.CurrentStatusCode = currStatus
                    newId = CreatePSCDWorkflow(sc, validationErrors)

                Case Else
                    sc.moduleId = vbNull

                    SC_Dao.Save(sc)
                    newId = sc.Id
            End Select

            If (newId > 0) Then
                LogManager.LogAction(CURR_MODULE_ID, UserAction.InitiatedSpecialCase, newId, "Workflow: " + ddlWorkflow.SelectedItem.Text, currStatus)
                ActionMode = ACTION_SIGN
                ViewState(KEY_REFID) = newId
                ViewState(KEY_REFSTATUS) = currStatus
                SigBlock.StartSignature(newId, sc.Workflow, 0, "Initiate Case of Type: " & CURR_MODULE, currStatus, currStatus, 0, DBSignTemplateId.SignOnly, "")
            Else
                ActionMode = ACTION_CREATE

                If (validationErrors.Count < 1) Then
                    validationErrors.Add("An error occured initiating the Case of Type " & CURR_MODULE)
                End If

                ValidationList.DataSource = validationErrors
                ValidationList.DataBind()
                ValidationList.Visible = True
                ValidationErrorsRow.Visible = True
            End If
        End Sub

        Protected Function CreateCMASWorkflow(ByVal baseSpecCase As SpecialCase, ByVal validationErrors As List(Of String)) As Integer
            Dim newId As Integer = 0
            Dim sc As New SC_CMAS

            sc.CopyBaseObjectInstance(baseSpecCase)
            SC_Dao.Evict(baseSpecCase)

            SC_Dao.Save(sc)
            newId = sc.Id

            Return newId
        End Function

        Protected Function CreateCongressWorkflow(ByVal baseSpecCase As SpecialCase, ByVal validationErrors As List(Of String)) As Integer
            Dim newId As Integer = 0
            Dim sc As New SC_Congress

            sc.CopyBaseObjectInstance(baseSpecCase)
            SC_Dao.Evict(baseSpecCase)

            SC_Dao.Save(sc)
            newId = sc.Id

            Return newId
        End Function

        Protected Function CreateDWWorkflow(ByVal baseSpecCase As SpecialCase, ByVal validationErrors As List(Of String)) As Integer
            Dim newId As Integer = 0
            Dim sc As New SC_DW

            If (rbNo.Checked = False And rbYes.Checked = False) Then
                validationErrors.Add("Is this a Simulated Deployment?")
            ElseIf (ddlMAJCOM.SelectedIndex = 0) Then
                validationErrors.Add("Select a MAJCOM.")
            Else

                sc.CopyBaseObjectInstance(baseSpecCase)
                SC_Dao.Evict(baseSpecCase)

                sc.MAJCOM = ddlMAJCOM.SelectedValue

                If (rbYes.Checked = True) Then
                    sc.Sim_Deployment = 1
                ElseIf (rbNo.Checked = True) Then
                    sc.Sim_Deployment = 0
                End If

                SC_Dao.Save(sc)
                newId = sc.Id
            End If

            Return newId
        End Function

        Protected Function CreateIncapWorkflow(ByVal baseSpecCase As SpecialCase, ByVal validationErrors As List(Of String)) As Integer
            Dim newId As Integer = 0
            Dim sc As New SC_Incap
            Dim refIds As New List(Of Integer)
            Dim workflowIds As New List(Of Integer)
            Dim CaseIds As New List(Of String)

            If (ddlLODs.SelectedValue <> 0) Then
                sc.CopyBaseObjectInstance(baseSpecCase)
                SC_Dao.Evict(baseSpecCase)

                If ddlLODs.SelectedValue = -1 Then
                    sc.HasAdminLOD = 1
                Else
                    sc.HasAdminLOD = 0
                End If

                SC_Dao.Save(sc)

                If ddlLODs.SelectedValue <> -1 Then
                    refIds.Add(ddlLODs.SelectedValue)
                    workflowIds.Add(LOD_Dao.GetById(ddlLODs.SelectedValue).Workflow)
                    CaseIds.Add(LOD_Dao.GetById(ddlLODs.SelectedValue).CaseId)
                    AssociatedDao.Save(sc.Id, sc.Workflow, refIds, workflowIds, CaseIds)
                End If

                newId = sc.Id
            Else
                validationErrors.Add("Case Type (" & CURR_MODULE & ") requires an Associated LOD")
            End If
            SC_Dao.CreateINCAPFindings(sc.Id)
            Return newId
        End Function

        Protected Function CreateIRILOWorkflow(ByVal baseSpecCase As SpecialCase, ByVal validationErrors As List(Of String)) As Integer
            Dim newId As Integer = 0
            Dim LODCount As Integer = 0
            Dim include As CheckBox = Nothing
            Dim ref As Label = Nothing
            Dim wf As Label = Nothing
            Dim caseId As Label = Nothing
            Dim refIds As New List(Of Integer)
            Dim workflowIds As New List(Of Integer)
            Dim caseIds As New List(Of String)

            For Each row As GridViewRow In MultipleLODs.Rows
                include = row.FindControl("AddLOD")
                ref = row.FindControl("RefId")
                wf = row.FindControl("workflowId")
                caseId = row.FindControl("lblCaseId")

                If (include Is Nothing OrElse ref Is Nothing OrElse wf Is Nothing) Then
                    Continue For
                End If

                If include.Checked Then
                    refIds.Add(Int(ref.Text.ToString()))
                    workflowIds.Add(Int(wf.Text.ToString()))
                    caseIds.Add(caseId.Text.ToString())
                    LODCount += 1
                End If
            Next

            If (refIds.Contains(0) AndAlso LODCount > 1) Then
                Dim index As Integer = refIds.IndexOf(0)

                refIds.Remove(0)
                workflowIds.Remove(0)
                caseIds.RemoveAt(index)
                LODCount -= 1
            End If

            If (LODCount > 25) Then
                validationErrors.Add("More then 25 LODs have been selected for association")
                Return 0
            End If

            Dim sc As New SC_FastTrack
            sc.CopyBaseObjectInstance(baseSpecCase)
            SC_Dao.Evict(baseSpecCase)

            SC_Dao.Save(sc)
            newId = sc.Id

            If (refIds.Contains(-1) AndAlso workflowIds.Contains(-1)) Then
                sc.HasAdminLOD = 1

                Dim index As Integer = refIds.IndexOf(-1)

                refIds.Remove(-1)
                workflowIds.Remove(-1)
                caseIds.RemoveAt(index)
            End If

            If (refIds.Count > 0 AndAlso workflowIds.Count > 0) Then
                AssociatedDao.Save(sc.Id, sc.Workflow, refIds, workflowIds, caseIds)
            End If

            Return newId
        End Function

        Protected Function CreateMEBWorkflow(ByVal baseSpecCase As SpecialCase, ByVal validationErrors As List(Of String)) As Integer
            Dim newId As Integer = 0
            Dim sc As New SC_MEB
            Dim refIds As New List(Of Integer)
            Dim workflowIds As New List(Of Integer)
            Dim caseIds As New List(Of String)

            If (ddlLODs.SelectedValue = -1) Then
                sc.CopyBaseObjectInstance(baseSpecCase)
                SC_Dao.Evict(baseSpecCase)

                sc.HasAdminLOD = 1

                SC_Dao.Save(sc)
                newId = sc.Id
            ElseIf (ddlLODs.SelectedValue > 0) Then
                sc.CopyBaseObjectInstance(baseSpecCase)
                SC_Dao.Evict(baseSpecCase)

                refIds.Add(ddlLODs.SelectedValue)
                workflowIds.Add(LOD_Dao.GetById(ddlLODs.SelectedValue).Workflow)
                caseIds.Add(LOD_Dao.GetById(ddlLODs.SelectedValue).CaseId)

                sc.HasAdminLOD = 0
                If ddlWWDs.SelectedValue > 0 Then
                    sc.AssociatedWWD = ddlWWDs.SelectedValue
                End If

                SC_Dao.Save(sc)

                AssociatedDao.Save(sc.Id, sc.Workflow, refIds, workflowIds, caseIds)

                newId = sc.Id
            Else
                validationErrors.Add("Case Type (" & CURR_MODULE & ") requires an Associated LOD")
            End If

            Return newId
        End Function

        Protected Function CreateMHWorkflow(ByVal baseSpecCase As SpecialCase, ByVal validationErrors As List(Of String)) As Integer
            Dim newId As Integer = 0
            Dim sc As New SC_MH

            sc.CopyBaseObjectInstance(baseSpecCase)
            SC_Dao.Evict(baseSpecCase)

            SC_Dao.Save(sc)
            newId = sc.Id

            Return newId
        End Function

        Protected Function CreateMMSOWorkflow(ByVal baseSpecCase As SpecialCase, ByVal validationErrors As List(Of String)) As Integer
            Dim newId As Integer = 0
            Dim sc As New SC_MMSO
            Dim refIds As New List(Of Integer)
            Dim workflowIds As New List(Of Integer)
            Dim CaseIds As New List(Of String)

            If (ddlLODs.SelectedValue > 0) Then
                sc.CopyBaseObjectInstance(baseSpecCase)
                SC_Dao.Evict(baseSpecCase)

                refIds.Add(ddlLODs.SelectedValue)
                workflowIds.Add(LOD_Dao.GetById(ddlLODs.SelectedValue).Workflow)
                CaseIds.Add(LOD_Dao.GetById(ddlLODs.SelectedValue).CaseId)
                SC_Dao.Save(sc)

                AssociatedDao.Save(sc.Id, sc.Workflow, refIds, workflowIds, CaseIds)

                newId = sc.Id
            Else
                validationErrors.Add("Case Type (" & CURR_MODULE & ") requires an Associated LOD")
            End If

            Return newId
        End Function

        Protected Function CreateMOWorkflow(ByVal baseSpecCase As SpecialCase, ByVal validationErrors As List(Of String)) As Integer
            Dim newId As Integer = 0
            Dim sc As New SC_MO

            Dim refIds As New List(Of Integer)
            Dim workflowIds As New List(Of Integer)
            Dim caseIds As New List(Of String)

            ' Make sure a special case was selected
            If ddlSCs.SelectedIndex <> 0 Then
                sc.CopyBaseObjectInstance(baseSpecCase)
                SC_Dao.Evict(baseSpecCase)

                If ddlSCs.SelectedValue > 0 Then
                    sc.HasAdminSC = 0
                    sc.AssociatedSC = ddlSCs.SelectedValue

                    refIds.Add(ddlSCs.SelectedValue)
                    workflowIds.Add(scdao.GetById(ddlSCs.SelectedValue).Workflow)
                    caseIds.Add(ddlSCs.SelectedItem.Text)

                    SC_Dao.Save(sc)

                    AssociatedDao.Save(sc.Id, sc.Workflow, refIds, workflowIds, caseIds)
                End If

                If ddlSCs.SelectedValue = -1 Then
                    sc.HasAdminSC = 1
                    sc.AssociatedSC = Nothing
                End If

                SC_Dao.Save(sc)
                newId = sc.Id
            Else
                validationErrors.Add("Case Type (" & CURR_MODULE & ") requires an Associated Special Case")
            End If

            Return newId
        End Function

        Protected Function CreateNEWorkflow(ByVal baseSpecCase As SpecialCase, ByVal validationErrors As List(Of String)) As Integer
            Dim newId As Integer = 0
            Dim sc As New SC_NE

            sc.CopyBaseObjectInstance(baseSpecCase)
            SC_Dao.Evict(baseSpecCase)

            SC_Dao.Save(sc)
            newId = sc.Id

            Return newId
        End Function

        Protected Function CreatePEPPWorkflow(ByVal baseSpecCase As SpecialCase, ByVal validationErrors As List(Of String)) As Integer
            Dim newId As Integer = 0
            Dim sc As New SC_PEPP

            sc.CopyBaseObjectInstance(baseSpecCase)
            SC_Dao.Evict(baseSpecCase)

            ' Set the sub type of the case
            If ddlPEPPSubType.SelectedValue > 0 Then
                sc.SubWorkflowType = ddlPEPPSubType.SelectedValue

                If sc.SubWorkflowType = 1 Then
                    Session("SubWorkflowType") = "(PEPP) "
                Else
                    Session("SubWorkflowType") = "(AIMWITS) "
                End If
            End If

            SC_Dao.Save(sc)
            newId = sc.Id

            Return newId
        End Function

        Protected Function CreatePSCDWorkflow(ByVal baseSpecCase As SpecialCase, ByVal validationErrors As List(Of String)) As Integer
            Dim newId As Integer = 0
            Dim sc As New SC_PSCD
            Dim refIds As New List(Of Integer)
            Dim workflowIds As New List(Of Integer)
            Dim CaseIds As New List(Of String)
            Dim canOpen As Boolean = True
            Dim LODCount As Integer = 0
            Dim include As CheckBox = Nothing
            Dim ref As Label = Nothing
            Dim wf As Label = Nothing
            Dim caseId As Label = Nothing
            'Dim ddList

            'Dim scTest As SC_PSCD_Findings
            ''scTest = New SC_PSCD

            'sc.CopyBaseObjectInstance(baseSpecCase)
            'SC_Dao.Evict(baseSpecCase)
            'Dim msgSC As New StringBuilder()
            'Dim msgBS As New StringBuilder()

            'msgBS.Append("/baseSpecCase\  Member: " + baseSpecCase.MemberName + " with lodId: " + baseSpecCase.Id.ToString() + ". Case status is " + baseSpecCase.Status.ToString() + " in workflow: " + baseSpecCase.Workflow.ToString() + " and moduleId: " + baseSpecCase.moduleId.ToString() + ".")

            'If (sc IsNot Nothing) Then
            '    msgBS.Append("/sc\  Member: " + sc.MemberName + " with lodId: " + sc.Id.ToString() + ". Case status is " + sc.Status.ToString() + " in workflow: " + sc.Workflow.ToString() + " and moduleId: " + sc.moduleId.ToString() + ".")

            'End If
            'LogManager.LogError(msgBS.ToString())
            'LogManager.LogError(msgSC.ToString())

            'SC_Dao.Save(sc)
            'newId = sc.Id

            Select Case AssociatedCaseTypeDropDownList.SelectedValue
                Case "WD"

                    If (ddlWWDs.SelectedValue <> 0) Then
                        sc.CopyBaseObjectInstance(baseSpecCase)
                        SC_Dao.Evict(baseSpecCase)

                        If ddlWWDs.SelectedValue = -1 Then
                            sc.HasAdminLOD = 1
                        Else
                            sc.HasAdminLOD = 0
                        End If

                        SC_Dao.Save(sc)

                        If (ddlWWDs.SelectedValue > 0) Then
                            refIds.Add(ddlWWDs.SelectedValue)
                            workflowIds.Add(scdao.GetById(ddlWWDs.SelectedValue).Workflow)
                            CaseIds.Add(scdao.GetById(ddlWWDs.SelectedValue).CaseId)
                            AssociatedDao.Save(sc.Id, sc.Workflow, refIds, workflowIds, CaseIds)
                        End If

                        newId = sc.Id
                        validationErrors.Add("Case Type (" & CURR_MODULE & ") requires an Associated LOD")
                    End If
                Case "RW"
                    If (ddlRWs.SelectedValue <> 0) Then
                        sc.CopyBaseObjectInstance(baseSpecCase)
                        SC_Dao.Evict(baseSpecCase)

                        If ddlRWs.SelectedValue = -1 Then
                            sc.HasAdminLOD = 1
                        Else
                            sc.HasAdminLOD = 0
                        End If

                        SC_Dao.Save(sc)

                        If (ddlRWs.SelectedValue > 0) Then
                            refIds.Add(ddlRWs.SelectedValue)
                            workflowIds.Add(scdao.GetById(ddlRWs.SelectedValue).Workflow)
                            CaseIds.Add(scdao.GetById(ddlRWs.SelectedValue).CaseId)
                            AssociatedDao.Save(sc.Id, sc.Workflow, refIds, workflowIds, CaseIds)
                        End If

                        newId = sc.Id
                        validationErrors.Add("Case Type (" & CURR_MODULE & ") requires an Associated Retention Waiver")
                    End If
                Case "IR"
                    If (ddlIR.SelectedValue = -1) Then
                        sc.CopyBaseObjectInstance(baseSpecCase)
                        SC_Dao.Evict(baseSpecCase)

                        sc.HasAdminLOD = 1

                        SC_Dao.Save(sc)
                        newId = sc.Id
                    ElseIf (ddlIR.SelectedValue > 0) Then
                        sc.CopyBaseObjectInstance(baseSpecCase)
                        SC_Dao.Evict(baseSpecCase)

                        refIds.Add(ddlIR.SelectedValue)
                        workflowIds.Add(scdao.GetById(ddlIR.SelectedValue).Workflow)
                        CaseIds.Add(scdao.GetById(ddlIR.SelectedValue).CaseId)

                        sc.HasAdminLOD = 0
                        'If ddlWWDs.SelectedValue > 0 Then
                        '    sc.AssociatedWWD = ddlWWDs.SelectedValue
                        'End If

                        SC_Dao.Save(sc)

                        AssociatedDao.Save(sc.Id, sc.Workflow, refIds, workflowIds, CaseIds)

                        newId = sc.Id
                    Else
                        validationErrors.Add("Case Type (" & CURR_MODULE & ") requires an Associated IRILO")
                    End If

                    'If the want to chose multiple LODs
                    '        For Each row As GridViewRow In MultipleLODs.Rows

                    '            include = row.FindControl("AddLOD")
                    '            ref = row.FindControl("RefId")
                    '            wf = row.FindControl("workflowId")
                    '            caseId = row.FindControl("lblCaseId")

                    '            If (include Is Nothing OrElse ref Is Nothing OrElse wf Is Nothing) Then
                    '                Continue For
                    '            End If

                    '            If include.Checked Then
                    '                refIds.Add(Int(ref.Text.ToString()))
                    '                workflowIds.Add(Int(wf.Text.ToString()))
                    '                CaseIds.Add(caseId.Text.ToString())
                    '                LODCount += 1
                    '            End If
                    '        Next

                    '        If (refIds.Contains(0) AndAlso LODCount > 1) Then
                    '            Dim index As Integer = refIds.IndexOf(0)

                    '            refIds.Remove(0)
                    '            workflowIds.Remove(0)
                    '            CaseIds.RemoveAt(index)
                    '            LODCount -= 1
                    '        End If

                    '        If (LODCount > 25) Then
                    '            validationErrors.Add("More then 25 LODs have been selected for association")
                    '            Return 0
                    '        End If

                    '        'Dim sc As New SC_FastTrack
                    '        sc.CopyBaseObjectInstance(baseSpecCase)
                    '        SC_Dao.Evict(baseSpecCase)

                    '        SC_Dao.Save(sc)
                    '        newId = sc.Id

                    '        If (refIds.Contains(-1) AndAlso workflowIds.Contains(-1)) Then
                    '            sc.HasAdminLOD = 1

                    '            Dim index As Integer = refIds.IndexOf(-1)

                    '            refIds.Remove(-1)
                    '            workflowIds.Remove(-1)
                    '            CaseIds.RemoveAt(index)
                    '        End If

                    '        If (refIds.Count > 0 AndAlso workflowIds.Count > 0) Then
                    '            AssociatedDao.Save(sc.Id, sc.Workflow, refIds, workflowIds, CaseIds)
                    '        End If
            End Select
            Return newId
        End Function

        Protected Function CreatePWWorkflow(ByVal baseSpecCase As SpecialCase, ByVal validationErrors As List(Of String)) As Integer
            Dim newId As Integer = 0
            Dim sc As New SC_PWaivers

            sc.CopyBaseObjectInstance(baseSpecCase)
            SC_Dao.Evict(baseSpecCase)

            SC_Dao.Save(sc)
            newId = sc.Id

            Return newId
        End Function

        Protected Function CreateRSWorkflow(ByVal baseSpecCase As SpecialCase, ByVal validationErrors As List(Of String)) As Integer
            Dim newId As Integer = 0
            Dim sc As New SC_RS
            Dim isInputValid As Boolean = True

            ' Validate Case Type selection...
            If (ddlCaseType.SelectedValue <= 0) Then
                isInputValid = False
                validationErrors.Add("Case Type (" & CURR_MODULE & ") requires a Speciality Case Type to be selected.")
            Else
                If (ddlCaseType.SelectedItem.Text.ToLower().Equals("other") AndAlso String.IsNullOrEmpty(txtCaseTypeName.Text.Trim())) Then
                    isInputValid = False
                    validationErrors.Add("Case Type (" & CURR_MODULE & ") requires a Specialty Case Type to be entered.")
                End If
            End If

            ' Validate Sub Case Type selection if necessary...
            If (ddlCaseType.SelectedValue > 0) Then
                ' If the selected Case Type has Sub Case Types then one of those sub types MUST be selected...
                If (HasSubCaseTypes(Integer.Parse(ddlCaseType.SelectedValue))) Then
                    If (ddlSubCaseType.SelectedValue <= 0) Then
                        isInputValid = False
                        validationErrors.Add("Case Type (" & CURR_MODULE & ") requires a Case Type to be selected.")
                    Else
                        If (ddlSubCaseType.SelectedItem.Text.ToLower().Equals("other") AndAlso String.IsNullOrEmpty(txtSubCaseTypeName.Text.Trim())) Then
                            isInputValid = False
                            validationErrors.Add("Case Type (" & CURR_MODULE & ") requires a Case Type to be entered.")
                        End If
                    End If
                End If
            End If

            If (isInputValid) Then
                sc.CopyBaseObjectInstance(baseSpecCase)
                SC_Dao.Evict(baseSpecCase)

                sc.CaseType = ddlCaseType.SelectedValue

                If (ddlSubCaseType.SelectedValue > 0) Then
                    sc.SubCaseType = ddlSubCaseType.SelectedValue
                End If

                If (GetIsCaseTypeOtherSelected()) Then
                    sc.CaseTypeName = Server.HtmlEncode(txtCaseTypeName.Text)
                End If

                If (GetIsSubCaseTypeOtherSelected()) Then
                    sc.SubCaseTypeName = Server.HtmlEncode(txtSubCaseTypeName.Text)
                End If

                If (ddlCompo.SelectedValue = "AFRC") Then
                    sc.MemberCompo = 6
                Else
                    sc.MemberCompo = 5

                End If

                SC_Dao.Save(sc)
                newId = sc.Id
            End If

            Return newId
        End Function

        Protected Function CreateRWWorkflow(ByVal baseSpecCase As SpecialCase, ByVal validationErrors As List(Of String)) As Integer
            Dim newId As Integer = 0
            Dim refIds As New List(Of Integer)
            Dim workflowIds As New List(Of Integer)
            Dim caseIds As New List(Of String)
            Dim LODCount As Integer = 0
            Dim include As CheckBox = Nothing
            Dim ref As Label = Nothing
            Dim wf As Label = Nothing
            Dim caseId As Label = Nothing

            For Each row As GridViewRow In MultipleLODs.Rows
                include = row.FindControl("AddLOD")
                ref = row.FindControl("RefId")
                wf = row.FindControl("workflowId")
                caseId = row.FindControl("lblCaseId")

                If (include Is Nothing OrElse ref Is Nothing OrElse wf Is Nothing) Then
                    Continue For
                End If

                If (include.Checked) Then
                    refIds.Add(Int(ref.Text.ToString()))
                    workflowIds.Add(Int(wf.Text.ToString()))
                    caseIds.Add(caseId.Text.ToString())
                    LODCount += 1
                End If
            Next

            If (refIds.Contains(0) AndAlso LODCount > 1) Then
                Dim index As Integer = refIds.IndexOf(0)

                refIds.Remove(0)
                workflowIds.Remove(0)
                caseIds.RemoveAt(index)
                LODCount -= 1
            End If

            If (LODCount > 25) Then
                validationErrors.Add("More then 25 LODs have been selected for association")
                Return 0
            End If

            If (ddlSCs.SelectedValue = 0) Then
                validationErrors.Add("Must select a special case to associate with the RW case OR Admin Special Case")
                Return 0
            End If

            Dim sc As New SC_RW
            sc.CopyBaseObjectInstance(baseSpecCase)
            SC_Dao.Evict(baseSpecCase)

            sc.AssociatedSC = ddlSCs.SelectedValue

            If (ddlSCs.SelectedValue = -1) Then
                sc.HasAdminSC = 1
            Else
                sc.HasAdminSC = 0

                refIds.Add(ddlSCs.SelectedValue)
                workflowIds.Add(scdao.GetById(ddlSCs.SelectedValue).Workflow)
                caseIds.Add(ddlSCs.SelectedItem.Text)
            End If

            SC_Dao.Save(sc)

            newId = sc.Id

            If (refIds.Contains(-1) AndAlso workflowIds.Contains(-1)) Then
                sc.HasAdminLOD = 1
                Dim index As Integer = refIds.IndexOf(-1)

                refIds.Remove(-1)
                workflowIds.Remove(-1)
                caseIds.RemoveAt(index)
            End If

            If (refIds.Count > 0 AndAlso workflowIds.Count > 0) Then
                AssociatedDao.Save(sc.Id, sc.Workflow, refIds, workflowIds, caseIds)
            End If

            Return newId
        End Function

        Protected Function CreateWWDWorkflow(ByVal baseSpecCase As SpecialCase, ByVal validationErrors As List(Of String)) As Integer
            Dim newId As Integer = 0
            Dim sc As New SC_WWD
            Dim canOpen As Boolean = True
            Dim refIds As New List(Of Integer)
            Dim workflowIds As New List(Of Integer)
            Dim CaseIds As New List(Of String)

            sc.CopyBaseObjectInstance(baseSpecCase)
            SC_Dao.Evict(baseSpecCase)

            'Validate Associated LOD dropdownlist
            If (ddlLODs.SelectedValue = 0) Then
                validationErrors.Add("Case Type (" & CURR_MODULE & ") requires an Associated LOD option.")
                canOpen = False
            End If

            If (ddlLODs.SelectedValue = 1) Then
                validationErrors.Add("Stop WWD Processing. You will need to process this case as an MEB.")
                canOpen = False
            End If

            sc.ExpirationDate = Nothing

            If canOpen Then
                SC_Dao.Save(sc)
                newId = sc.Id

                If (ddlWWDs.SelectedValue > 0) Then
                    refIds.Add(ddlWWDs.SelectedValue)
                    workflowIds.Add(scdao.GetById(ddlWWDs.SelectedValue).Workflow)
                    CaseIds.Add(scdao.GetById(ddlWWDs.SelectedValue).CaseId)
                    AssociatedDao.Save(sc.Id, sc.Workflow, refIds, workflowIds, CaseIds)
                End If
            Else
                SC_Dao.Evict(sc)
            End If

            Return newId
        End Function

        Protected Sub ddlCaseType_SelectedIndexChanged(sender As Object, e As System.EventArgs) Handles ddlCaseType.SelectedIndexChanged
            Dim selectedValue As Integer = Integer.Parse(ddlCaseType.SelectedValue)

            If (HasSubCaseTypes(selectedValue)) Then
                TurnSubCaseTypeRowOnOff(True)
            Else
                TurnSubCaseTypeRowOnOff(False)
            End If

            FillSubCaseTypeList(selectedValue)

            OtherCaseTypeRow.Visible = GetIsCaseTypeOtherSelected()
            OtherSubCaseTypeRow.Visible = GetIsSubCaseTypeOtherSelected()
        End Sub

        Protected Sub ddlModule_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles ddlModule.SelectedIndexChanged
            CURR_MODULE = ddlModule.SelectedItem.Text
            CURR_MODULE_ID = ddlModule.SelectedValue
        End Sub

        Protected Sub ddlSubCaseType_SelectedIndexChanged(sender As Object, e As System.EventArgs) Handles ddlSubCaseType.SelectedIndexChanged
            OtherSubCaseTypeRow.Visible = GetIsSubCaseTypeOtherSelected()
        End Sub

        Protected Sub ddlSubModule_SelectedIndexChanged(sender As Object, e As EventArgs)

        End Sub

        Protected Function GetAssociateableLODsByWorkflowType(ByVal workflowType As Integer, ByVal memberSSN As String) As List(Of Tuple(Of String, Integer, Integer))
            If (workflowType = AFRCWorkflows.SpecCaseIncap OrElse workflowType = AFRCWorkflows.SpecCaseMMSO OrElse workflowType = AFRCWorkflows.SpecCasePSCD) Then
                Return AssociatedDao.GetLODListByMemberSSN(memberSSN, 1, SESSION_USER_ID).ToList() ' ALL (including cancelled cases)
            End If

            If (workflowType = AFRCWorkflows.SpecCaseMEB OrElse workflowType = AFRCWorkflows.SpecCaseWWD) Then
                Return AssociatedDao.GetLODListByMemberSSN(memberSSN, 2, SESSION_USER_ID).ToList()  'Completed and In ILOD/EPTS-SA
            End If

            If (workflowType = AFRCWorkflows.SpecCaseFT OrElse workflowType = AFRCWorkflows.SpecCaseRW) Then
                Return AssociatedDao.GetLODListByMemberSSN(memberSSN, 4, SESSION_USER_ID).ToList()  ' All open and completed cases (excluding cancelled cases)
            End If

            Return New List(Of Tuple(Of String, Integer, Integer))
        End Function

        Protected Sub HistoryGrid_PageIndexChanging(sender As Object, e As System.Web.UI.WebControls.GridViewPageEventArgs) Handles HistoryGrid.PageIndexChanging
            HistoryGrid.PageIndex = e.NewPageIndex
            Dim dsSort As DataSet = Session("HistoryGridSC")
            Dim dvSort As DataView = dsSort.Tables(0).DefaultView
            HistoryGrid.DataSource = dvSort
            HistoryGrid.DataBind()
        End Sub

        Protected Sub HistoryGrid_RowDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewRowEventArgs) Handles HistoryGrid.RowDataBound
            Dim caseId As String
            Dim data As System.Data.DataRowView

            If (e.Row.RowType = DataControlRowType.DataRow) Then
                data = e.Row.DataItem
                caseId = data("Case_Id")
            End If
        End Sub

        Protected Sub HistoryGrid_Sorting(sender As Object, e As System.Web.UI.WebControls.GridViewSortEventArgs) Handles HistoryGrid.Sorting
            HistoryGrid.PageIndex = 0
            If (SortColumnSC <> "") Then
                If (SortColumnSC = e.SortExpression) Then
                    If SortDirectionSC = ASCENDING Then
                        SortDirectionSC = DESCENDING
                    Else
                        SortDirectionSC = ASCENDING
                    End If
                Else
                    SortDirectionSC = ASCENDING
                End If
            End If

            SortColumnSC = e.SortExpression
            Dim dsSort As DataSet = Session("HistoryGridSC")
            Dim dvSort As DataView = dsSort.Tables(0).DefaultView
            dvSort.Sort = SortExpressionSC
            HistoryGrid.DataSource = dvSort
            HistoryGrid.DataBind()
        End Sub

        Protected Sub HistoryGridLOD_PageIndexChanging(sender As Object, e As System.Web.UI.WebControls.GridViewPageEventArgs) Handles HistoryGridLOD.PageIndexChanging
            HistoryGridLOD.PageIndex = e.NewPageIndex
            Dim dsSort As DataSet = Session("HistoryGridLOD")
            Dim dvSort As DataView = dsSort.Tables(0).DefaultView
            HistoryGridLOD.DataSource = dvSort
            HistoryGridLOD.DataBind()
        End Sub

        Protected Sub HistoryGridLOD_RowDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewRowEventArgs) Handles HistoryGridLOD.RowDataBound
            Dim caseId As String
            Dim data As System.Data.DataRowView
            Dim status As Integer
            Dim wsStatus As ALOD.Core.Domain.Workflow.WorkStatus
            Dim completeDateLbl As Label

            If (e.Row.RowType = DataControlRowType.DataRow) Then
                data = e.Row.DataItem
                status = data("WorkStatusId")
                wsStatus = WorkStatusDao.GetById(CInt(status))
                caseId = data("CaseId")
                If wsStatus.StatusCodeType.IsFinal Then
                    completeDateLbl = CType(e.Row.FindControl("completeDateLbl"), Label)
                    completeDateLbl.Text = data("ReceiveDate")
                End If
            End If
        End Sub

        Protected Sub HistoryGridLOD_Sorting(sender As Object, e As System.Web.UI.WebControls.GridViewSortEventArgs) Handles HistoryGridLOD.Sorting
            HistoryGridLOD.PageIndex = 0
            If (SortColumnLOD <> "") Then
                If (SortColumnLOD = e.SortExpression) Then
                    If SortDirectionLOD = ASCENDING Then
                        SortDirectionLOD = DESCENDING
                    Else
                        SortDirectionLOD = ASCENDING
                    End If
                Else
                    SortDirectionLOD = ASCENDING
                End If
            End If

            SortColumnLOD = e.SortExpression
            Dim dsSort As DataSet = Session("HistoryGridLOD")
            Dim dvSort As DataView = dsSort.Tables(0).DefaultView
            dvSort.Sort = SortExpressionLOD
            HistoryGridLOD.DataSource = dvSort
            HistoryGridLOD.DataBind()
        End Sub

        Protected Sub InitDWRows()
            sectionAction.Text = "J"
            sectionErrors.Text = "I"
        End Sub

        Protected Sub InitIncapRows(ByVal memberSSN As String)
            AssociatedLODList.Visible = True
            AssociatedLODListHeader.Visible = True
            AssociatedLODListLabel.Visible = True
            lodRequired.Attributes.Add("class", "label labelRequired")
            FillLodList(False, memberSSN)
            sectionAssociatedLOD.Text = "H"
            sectionAction.Text = "I"
            sectionErrors.Text = "J"
            AssociatedLODRow.Visible = True
        End Sub

        Protected Sub InitIRILORows(ByVal memberSSN As String)
            AssociatedLODMultiple.Visible = True
            AssociatedLODListHeader.Visible = True
            AssociatedLODListLabel.Visible = True
            lodRequired.Attributes.Add("class", "label labelRequired")
            FillMultipleLodList(False, memberSSN)
            lodRequired.Attributes.Remove("class")
            lodRequired.Attributes.Add("class", "label")
            sectionAssociatedLOD.Text = "H"
            sectionAction.Text = "I"
            sectionErrors.Text = "J"
            AssociatedLODRow.Visible = True
        End Sub

        Protected Sub InitMEBRows(ByVal memberSSN As String)
            AssociatedLODList.Visible = True
            AssociatedLODListHeader.Visible = True
            lodRequired.Attributes.Add("class", "label labelRequired")
            AssociatedLODListLabel.Visible = True
            FillLodList(False, memberSSN)
            sectionAssociatedLOD.Text = "H"
            sectionAction.Text = "I"
            sectionErrors.Text = "J"
            AssociatedLODRow.Visible = True
        End Sub

        Protected Sub InitMMSORows(ByVal memberSSN As String)
            AssociatedLODList.Visible = True
            AssociatedLODListHeader.Visible = True
            AssociatedLODListLabel.Visible = True
            lodRequired.Attributes.Add("class", "label labelRequired")
            FillLodList(False, memberSSN)
            sectionAssociatedLOD.Text = "H"
            sectionAction.Text = "H"
            sectionErrors.Text = "J"
            AssociatedLODRow.Visible = True
        End Sub

        Protected Sub InitMORows(ByVal memberSSN As String)
            AssociatedSCList.Visible = True
            AssociatedSCListHeader.Visible = True
            AssociatedSCListLabel.Visible = True
            AssociatedSCRow.Visible = True
            FillSpecCaseList(memberSSN, False)
            scRequired.Attributes.Add("class", "label labelRequired")
            sectionAssociatedSC.Text = "H"
            sectionAction.Text = "H"
            sectionErrors.Text = "J"
            CreateButton.Enabled = True
        End Sub

        Protected Sub InitNERows()
            nesRequestEligibleRow.Visible = True
            nesRequestEligibleHeader.Visible = True
            nesRequestEligibleLabel.Visible = True
            nesRequestEligibleRadioButtons.Visible = True
            nesRequestEligible.Text = "H"
            sectionAction.Text = "H"
            sectionErrors.Text = "J"
            CreateButton.Enabled = False
        End Sub

        Protected Sub InitPEPPRows()
            PEPPSubTypeRow.Visible = True
            PEPPSubTypeListHeader.Visible = True
            PEPPSubTypeListLabel.Visible = True
            PEPPSubTypeList.Visible = True
            sectionAction.Text = "H"
            sectionErrors.Text = "I"
            CreateButton.Enabled = True
        End Sub

        Protected Sub InitPSCDIRILORows(ByVal memberSSN As String)
            FillAssociatedIRList(memberSSN)
        End Sub

        Protected Sub InitPSCDRows(ByVal memberSSN As String)
            Dim associatedCaseType As String = AssociatedCaseTypeDropDownList.SelectedValue
            PSCDAssociatedCaseType.Visible = True
            ResetRows()
            Select Case associatedCaseType
                Case "WD"
                    ddlIR.Items.Clear()
                    ddlRWs.Items.Clear()
                    AssociatedWWDList.Visible = True
                    AssociatedWWDListHeader.Visible = True
                    AssociatedWWDListLabel.Visible = True
                    FillWWDList(False, memberSSN)
                    sectionAssociatedWWD.Text = "I"
                    sectionAction.Text = "J"
                    sectionErrors.Text = "K"
                    AssociatedWWDRow.Visible = True
                Case "RW"
                    ddlWWDs.Items.Clear()
                    ddlIR.Items.Clear()
                    FillSpecCaseListForPSCDRW(memberSSN)
                    AssociatedRWRow.Visible = True
                    AssociatedRWListHeader.Visible = True
                    AssociatedRWListLabel.Visible = True
                    AssociatedRWList.Visible = True
                    sectionAssociatedRW.Text = "I"
                    sectionAction.Text = "J"
                    sectionErrors.Text = "K"
                Case "IR"
                    ddlWWDs.Items.Clear()
                    ddlRWs.Items.Clear()
                    AssociatedIRRow.Visible = True
                    AssociatedIRListHeader.Visible = True
                    AssociatedIRListLabel.Visible = True
                    AssociatedIRList.Visible = True
                    InitPSCDIRILORows(memberSSN)
                    sectionAssociatedIR.Text = "I"
                    sectionAction.Text = "J"
                    sectionErrors.Text = "K"
            End Select
        End Sub

        Protected Sub InitRSRows()
            FillCaseTypeList()
            CaseTypeRow.Visible = True
            CaseTypeListHeader.Visible = True
            caseTypeListLabel.Visible = True
            caseTypeList.Visible = True

            lblSectionCaseType.Text = "G"
            lblSectionSubCaseType.Text = "H"
            sectionAction.Text = "H"
            sectionErrors.Text = "I"

            CreateButton.Enabled = True
        End Sub

        Protected Sub InitRWRows(ByVal memberSSN As String)
            AssociatedLODRow.Visible = True
            AssociatedSCRow.Visible = True
            AssociatedLODMultiple.Visible = True
            AssociatedLODListHeader.Visible = True
            AssociatedLODListLabel.Visible = True
            AssociatedSCList.Visible = True
            AssociatedSCListHeader.Visible = True
            AssociatedSCListLabel.Visible = True

            lodRequired.Attributes.Remove("class")
            lodRequired.Attributes.Add("class", "label")

            scRequired.Attributes.Remove("class")
            scRequired.Attributes.Add("class", "label")

            sectionAssociatedLOD.Text = "H"
            sectionAssociatedSC.Text = "H"
            sectionAction.Text = "J"
            sectionErrors.Text = "K"
            If (CURR_MODULE_ID <> 30) Then
                scRequired.Attributes.Add("class", "label labelRequired")
                lodRequired.Attributes.Add("class", "label labelRequired")
            Else
                lblAssociatedSCList.CssClass = "label"
            End If
            FillMultipleLodList(False, memberSSN)
            FillSpecCaseListForRW(memberSSN)
        End Sub

        Protected Sub InitWWDRows(ByVal memberSSN As String)
            AssociatedLODRow.Visible = True
            AssociatedLODList.Visible = True
            AssociatedLODListHeader.Visible = True
            lodRequired.Attributes.Add("class", "label")
            AssociatedLODListLabel.Visible = True
            FillLodListForWWD()
            AssociatedWWDList.Visible = True
            AssociatedWWDListHeader.Visible = True
            AssociatedWWDListLabel.Visible = True
            FillWWDList(False, memberSSN)
            sectionAssociatedWWD.Text = "I"
            sectionAction.Text = "J"
            sectionErrors.Text = "K"
            AssociatedWWDRow.Visible = True
        End Sub

        Protected Sub LookupButton_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles LookupButton.Click
            ResetErrorLabels()

            Dim member As ServiceMember = Nothing

            If rdbSSN.Checked Then
                member = LookupServiceMemberBySSN()
            ElseIf rdbName.Checked Then
                member = LookupServiceMemberByName()
            End If

            If (member Is Nothing) Then
                Exit Sub
            End If

            ProcessSelectedMember(member)
            PrepareDropDownAreas(member.SSN)
        End Sub

        Protected Sub MemberSelected(ByVal sender As Object, ByVal e As MemberSelectedEventArgs)
            Dim resultsTable As DataTable = LookupService.GetServerMembersByName(txtMemberLastName.Text, txtMemberFirstName.Text, txtMemberMiddleName.Text)
            Dim member As ServiceMember = Nothing
            Dim ssn As String = String.Empty

            ssn = resultsTable.Rows(e.SelectedRowIndex).Field(Of String)("SSN")
            member = LookupService.GetServiceMemberBySSN(ssn)

            ProcessSelectedMember(member)
            PrepareDropDownAreas(ssn)
            PopulateCompoDropdownListbox(member)
        End Sub

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
            AddHandler SigBlock.SignCompleted, AddressOf SignatureCompleted
            AddHandler ucMemberSelectionGrid.MemberSelected, AddressOf MemberSelected

            PopulateModuleDropdownListbox()
            PopulateWorkflowDropdownListbox()

            If (Request.QueryString("module") IsNot Nothing) Then
                CURR_MODULE_ID = Request.QueryString("module")
            Else
                ddlModule.SelectedValue = ModuleId
                CURR_MODULE_ID = ddlModule.SelectedValue
            End If
            CURR_MODULE = ddlModule.SelectedItem.Text

            'added 6/14/2012 - per email response
            'moduleLabel.Text = CURR_MODULE
            ddlModule.Visible = False
            ddlSubModule.Visible = False
            If CURR_MODULE_ID = ModuleType.SpecCaseBMT Then
                ddlSubModule.Visible = True
                BMTModuleSubType.Visible = True
                rowModuleType.Visible = False
            End If

            ' DW workflow
            If CURR_MODULE_ID = ModuleType.SpecCaseDW Then
                rowSimDeploy.Visible = True
                rowMAJCOM.Visible = True
                If (Not Page.IsPostBack) Then
                    FillMAJCOMddl()
                End If

            End If

            ' PEPP workflow
            ddlModule.Visible = False
            ddlPEPPSubType.Visible = False
            If CURR_MODULE_ID = ModuleType.SpecCasePEPP Then
                rowModuleType.Visible = False
                ddlPEPPSubType.Visible = True
            End If

            ' RS workflow
            If CURR_MODULE_ID = ModuleType.SpecCaseRS Then
                rowModuleType.Visible = False
            End If

            If (CURR_MODULE_ID = ModuleType.SpecCasePSCD) Then
                InitPSCDRows(SSNLabel.Text)
                If (IsPostBack AndAlso ddlWWDs.SelectedIndex = 0) Then
                    'FillWWDList(False, SSNLabel.Text)

                ElseIf (IsPostBack AndAlso ddlSCs.SelectedIndex = 0) Then
                    FillSpecCaseListForRW(SSNLabel.Text)
                End If

            End If

            If (Not IsPostBack) Then
                SetMaxLength(txtCaseTypeName)
                SetMaxLength(txtSubCaseTypeName)

                SetInputFormatRestriction(Page, SSNTextBox, FormatRestriction.Numeric)
                SetInputFormatRestriction(Page, txtCaseTypeName, FormatRestriction.AlphaNumeric, PERMITTED_SPECIAL_CHAR_INPUT)
                SetInputFormatRestriction(Page, txtSubCaseTypeName, FormatRestriction.AlphaNumeric, PERMITTED_SPECIAL_CHAR_INPUT)
                SetDefaultButton(SSNTextBox, LookupButton)

                trSSN.Visible = True
                trSSN2.Visible = True
                trName.Visible = False
            End If
        End Sub

        Protected Sub Page_PreRender(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.PreRender
            ErrorRow.Visible = (ErrorLabel.Text.Length > 0)
        End Sub

        Protected Sub PopulateCompoDropdownListbox(ByRef member As ServiceMember)
            Dim lkupDAO As LookupDao
            lkupDAO = New NHibernateDaoFactory().GetLookupDao()

            ddlCompo.DataSource = From n In lkupDAO.GetCompos()
            ddlCompo.DataTextField = "Name"
            ddlCompo.DataValueField = "Value"
            ddlCompo.DataBind()
            If member.Component.ToString() = 5 Then
                SetDropdownByValue(ddlCompo, "ANG")
            ElseIf member.Component.ToString() = 6 Then
                SetDropdownByValue(ddlCompo, "AFRC")

            End If

        End Sub

        Protected Sub PopulateModuleDropdownListbox()
            For Each currField As FieldInfo In GetType(ModuleType).GetFields
                Dim myListItem As New ListItem
                Dim modTypeTemp As ModuleType
                Dim blnInsert As Boolean = True
                Dim currValue As Integer
                currValue = CType(currField.GetValue(modTypeTemp), Byte)
                myListItem.Value = currValue
                myListItem.Text = WorkflowDao.GetCaseType(currValue)  'CurrField.Name

                If Session(SESSIONKEY_COMPO) = "5" Then
                    If myListItem.Text.Contains("Worldwide Duty") OrElse myListItem.Value = "9" Then myListItem.Text = "Non Duty Disability Evaluation System"
                End If

                If myListItem.Value > ModuleType.ReinvestigationRequest Then   'Only Show Special Cases
                    For Each currItem In ddlModule.Items  'Keep from loading duplicate values on page reload
                        If currItem.ToString() = myListItem.Text.ToString() Then
                            blnInsert = False
                        End If
                    Next
                    If blnInsert Then
                        ddlModule.Items.Add(myListItem)
                    End If
                End If
            Next
        End Sub

        Protected Sub PopulateWorkflowDropdownListbox()
            For Each currField2 As FieldInfo In GetType(AFRCWorkflows).GetFields
                Dim myListItem2 As New ListItem
                Dim workTypeTemp As AFRCWorkflows
                Dim blnInsert2 As Boolean = True
                myListItem2.Value = CType(currField2.GetValue(workTypeTemp), UShort) 'had to change the data type from Byte to UShort to include WSID > 255
                myListItem2.Text = currField2.Name
                If myListItem2.Value > CInt(AFRCWorkflows.ReinvestigationRequest) Then   'Only Show Special Cases
                    For Each currItem2 In ddlWorkflow.Items  'Keep from loading duplicate values on page reload
                        If currItem2.ToString() = myListItem2.Text.ToString() Then
                            blnInsert2 = False
                        End If
                    Next
                    If blnInsert2 Then
                        ddlWorkflow.Items.Add(myListItem2)
                    End If
                End If
            Next
        End Sub

        Protected Sub PrepareDropDownAreas(ByVal memberSSN As String)
            If (Len(memberSSN.Trim()) < 1) Then
                Exit Sub  'Otherwise tries to return ALL LODs ever created
            End If

            ddlWorkflow.SelectedValue = WorkflowDao.GetWorkflowFromModule(CURR_MODULE_ID)
            FillWWDList(True, memberSSN)

            ResetRows()

            Select Case CURR_MODULE_ID
                Case ModuleType.SpecCaseIncap
                    InitIncapRows(memberSSN)

                Case ModuleType.SpecCaseFT
                    InitIRILORows(memberSSN)

                Case ModuleType.SpecCaseWWD
                    InitWWDRows(memberSSN)

                Case ModuleType.SpecCaseMEB
                    InitMEBRows(memberSSN)

                Case ModuleType.SpecCaseMMSO
                    InitMMSORows(memberSSN)

                Case ModuleType.SpecCaseNE
                    InitNERows()

                Case ModuleType.SpecCaseMO
                    InitMORows(memberSSN)

                Case ModuleType.SpecCaseDW
                    InitDWRows()

                Case ModuleType.SpecCasePEPP
                    InitPEPPRows()

                Case ModuleType.SpecCaseRS
                    InitRSRows()

                Case ModuleType.SpecCaseRW
                    InitRWRows(memberSSN)

                    'Case ModuleType.SpecCaseAGR
                    '    InitAGRRows(memberSSN)
                Case ModuleType.SpecCasePSCD
                    InitPSCDRows(memberSSN)
                Case Else
                    lodRequired.Attributes.Add("class", "label")
            End Select

            SetRedirectURL()
        End Sub

        Protected Sub rdbNESNo_CheckedChanged(sender As Object, e As System.EventArgs) Handles rdbNESNo.CheckedChanged
            CreateButton.Enabled = False
            nesRequestEligibieDecisionLabel.Text = "Member does not meet eligibility requirement for Non Emergent Surgery Request."
        End Sub

        Protected Sub rdbNESYes_CheckedChanged(sender As Object, e As System.EventArgs) Handles rdbNESYes.CheckedChanged
            CreateButton.Enabled = True
            nesRequestEligibieDecisionLabel.Text = String.Empty
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

        Protected Sub ResetRows()
            ValidationErrorsRow.Visible = False
            AssociatedLODList.Visible = False
            AssociatedLODListHeader.Visible = False
            AssociatedLODListLabel.Visible = False
            AssociatedWWDList.Visible = False
            AssociatedWWDListHeader.Visible = False
            AssociatedWWDListLabel.Visible = False
            AssociatedLODRow.Visible = False
            AssociatedWWDRow.Visible = False
            nesRequestEligibleRow.Visible = False
            nesRequestEligibleHeader.Visible = False
            nesRequestEligibleLabel.Visible = False
            nesRequestEligibleRadioButtons.Visible = False
            AssociatedSCList.Visible = False
            AssociatedSCListHeader.Visible = False
            AssociatedSCListLabel.Visible = False
            AssociatedSCRow.Visible = False
            PEPPSubTypeRow.Visible = False
            PEPPSubTypeListHeader.Visible = False
            PEPPSubTypeListLabel.Visible = False
            PEPPSubTypeList.Visible = False
            CaseTypeRow.Visible = False
            CaseTypeListHeader.Visible = False
            caseTypeListLabel.Visible = False
            caseTypeList.Visible = False
            SubCaseTypeRow.Visible = False
            SubCaseTypeListHeader.Visible = False
            subCaseTypeListLabel.Visible = False
            subCaseTypeList.Visible = False
            AssociatedIRRow.Visible = False
            AssociatedRWRow.Visible = False
            AssociatedRWListHeader.Visible = False
            AssociatedRWListLabel.Visible = False
            sectionAction.Text = "I"
            sectionErrors.Text = "J"
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

        Protected Sub SignatureCompleted(ByVal sender As Object, ByVal e As SignCompletedEventArgs)
            If (e.SignaturePassed) Then
                SC_Dao.CommitChanges()
                NHibernateSessionManager.Instance().CommitTransaction()
                SetRedirectURL()

                ReminderEmailDao.ReminderEmailInitialStep(ViewState(KEY_REFID), ViewState(KEY_REFSTATUS), "SC")
                Response.Redirect(URL_SC_INIT + e.RefId.ToString(), True)
            Else
                ErrorLabel.Text = Resources.Messages.ERROR_SIGNING_LOD_START
            End If
        End Sub

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

        Private Sub FillAssociatedIRList(ByVal ssn As String)
            If (Len(ssn.Trim()) < 1) Then
                Exit Sub  'Otherwise tries to return ALL LODs ever created
            End If

            Dim lstHeader As New ListItem
            Dim blnHeader As Boolean = True
            lstHeader.Value = 0
            lstHeader.Text = "--- NO IRILO Case Associated ---"
            ddlIR.Items.Add(lstHeader)
            ddlIR.AppendDataBoundItems = True
            ddlIR.DataSource = SC_Dao.GetPSCDAssociableIRSpecialCases(ssn)
            ddlIR.DataTextField = "CaseId"
            ddlIR.DataValueField = "RefId"
            ddlIR.DataBind()
        End Sub

        Private Sub FillCaseTypeList()
            ddlCaseType.DataSource = CaseTypeDao.GetWorkflowCaseTypes(WorkflowDao.GetWorkflowFromModule(CURR_MODULE_ID))
            ddlCaseType.DataTextField = "Name"
            ddlCaseType.DataValueField = "Id"
            ddlCaseType.DataBind()

            ' Med Tech only gets these options
            If (String.Equals(ApplicationUser.CurrentRoleName, "Medical Technician")) Then
                For i As Integer = ddlCaseType.Items.Count - 1 To 0 Step -1
                    If String.Compare(ddlCaseType.Items(i).Value, "17") And String.Compare(ddlCaseType.Items(i).Value, "16") And String.Compare(ddlCaseType.Items(i).Value, "18") Then
                        ddlCaseType.Items.RemoveAt(i)
                    End If
                Next
            End If

            Utility.InsertDropDownListZeroValue(ddlCaseType, "--- Select Speciality Case Type test---")
        End Sub

        Private Sub FillLodList(ByVal blnClear As Boolean, ByVal memberSSN As String)
            If (Len(memberSSN.Trim()) < 1) Then
                Exit Sub  'Otherwise tries to return ALL LODs ever created
            End If

            ddlLODs.Items.Clear()

            ' Add Header Rows
            Dim lstHeader As New ListItem
            Dim secondHeader As New ListItem
            Dim blnHeader As Boolean = True
            lstHeader.Value = 0
            lstHeader.Text = "--- NO LOD Associated ---"
            secondHeader.Value = -1
            secondHeader.Text = "--- Admin LOD ---"
            For Each currItemH In ddlLODs.Items  'Keep from loading duplicate values on page reload
                If currItemH.ToString() = lstHeader.Text.ToString() Then
                    blnHeader = False
                End If
                If currItemH.ToString() = secondHeader.Text.ToString() Then
                    blnHeader = False
                End If
            Next
            If blnHeader Then
                ddlLODs.Items.Add(lstHeader)
                ddlLODs.Items.Add(secondHeader)
            End If
            'Add Data Rows
            If (Not blnClear) Then
                ddlLODs.AppendDataBoundItems = True

                ddlLODs.DataSource = GetAssociateableLODsByWorkflowType(ddlWorkflow.SelectedValue, memberSSN)

                ddlLODs.DataValueField = "item2"
                ddlLODs.DataTextField = "item1"
                ddlLODs.DataBind()
            End If
        End Sub

        ''' <summary>
        ''' Method to add only 3 options to the Associated LOD Dropdownlist.
        ''' 1) -- Select Option --  | Validate, user needs to select one option.
        ''' 2) Yes                  | Validate, Message "Stop WWD Processing. You will need to process this case as an MEB."
        ''' 3) No LOD Associated    | Continue WWD workflow.
        ''' </summary>
        ''' <remarks>Phase I changes, WWD</remarks>
        Private Sub FillLodListForWWD()
            ddlLODs.Items.Clear()

            Dim listItem As New ListItem
            listItem.Value = 0
            listItem.Text = "--- Select an option ---"
            ddlLODs.Items.Add(listItem)

            listItem = New ListItem
            listItem.Value = 1
            listItem.Text = "Yes"
            ddlLODs.Items.Add(listItem)

            listItem = New ListItem
            listItem.Value = 2
            listItem.Text = "No Associated LOD"
            ddlLODs.Items.Add(listItem)
        End Sub

        Private Sub FillMAJCOMddl()

            Dim mj As New majcom()
            ddlMAJCOM.Items.Clear()
            ddlMAJCOM.DataSource = mj.GetMAJCOM(0, 1)

            ddlMAJCOM.DataTextField = "Name"
            ddlMAJCOM.DataValueField = "Value"
            ddlMAJCOM.DataBind()

            Utility.InsertDropDownListZeroValue(ddlMAJCOM, "--- Select a MAJCOM ---")
        End Sub

        Private Sub FillMultipleLodList(ByVal blnClear As Boolean, ByVal memberSSN As String)
            If (Len(memberSSN.Trim()) < 1) Then
                Exit Sub  'Otherwise tries to return ALL LODs ever created
            End If

            Dim cases As IList(Of Tuple(Of String, Integer, Integer)) = New List(Of Tuple(Of String, Integer, Integer))

            Dim AdminLOD As Tuple(Of String, Integer, Integer) = New Tuple(Of String, Integer, Integer)("--- Admin LOD ---", -1, -1)
            Dim NoLOD As Tuple(Of String, Integer, Integer) = New Tuple(Of String, Integer, Integer)("--- NO LOD Associated ---", 0, 0)

            cases.Add(AdminLOD)

            'Add Data Rows
            If (Not blnClear) Then
                cases = cases.Concat(GetAssociateableLODsByWorkflowType(ddlWorkflow.SelectedValue, memberSSN)).ToList()

                MultipleLODs.DataSource = cases
                MultipleLODs.DataBind()
            End If
        End Sub

        Private Sub FillSpecCaseList(ByVal memberSSN As String, ByVal includeAdminLOD As Boolean)
            If (Len(memberSSN.Trim()) < 1) Then
                Exit Sub
            End If

            ddlSCs.Items.Clear()

            Utility.InsertDropDownListZeroValue(ddlSCs, "--- No Special Case Associated ---")
            If (CURR_MODULE_ID <> 30) Then
                If (includeAdminLOD) Then
                    ddlSCs.Items.Add(New ListItem("Admin Special Case", -1))
                End If
            End If
            ddlSCs.AppendDataBoundItems = True
            ddlSCs.DataSource = SC_Dao.GetCompletedSpecialCasesByMemberSSN(memberSSN, SESSION_USER_ID)
            ddlSCs.DataTextField = "Case_Id"
            ddlSCs.DataValueField = "RefId"
            ddlSCs.DataBind()
        End Sub

        Private Sub FillSpecCaseListForPSCDRW(ByVal memberSSN As String)
            If (Len(memberSSN.Trim()) < 1) Then
                Exit Sub
            End If

            Utility.InsertDropDownListZeroValue(ddlRWs, "--- No RW Case Associated ---")
            If (CURR_MODULE_ID <> 30) Then
                ddlRWs.Items.Add(New ListItem("Admin Special Case", -1))
            Else
                'AddCssClass(ddlRWs, CSS_FIELD_REQUIRED)
            End If
            ddlRWs.AppendDataBoundItems = True
            ddlRWs.DataSource = SC_Dao.GetPSCDAssociableRWSpecialCases(memberSSN)
            ddlRWs.DataTextField = "CaseId"
            ddlRWs.DataValueField = "RefId"
            ddlRWs.DataBind()
        End Sub

        Private Sub FillSpecCaseListForRW(ByVal memberSSN As String)
            If (Len(memberSSN.Trim()) < 1) Then
                Exit Sub
            End If

            ddlSCs.Items.Clear()

            Utility.InsertDropDownListZeroValue(ddlSCs, "--- No Special Case Associated ---")
            ddlSCs.Items.Add(New ListItem("Admin Special Case", -1))
            ddlSCs.AppendDataBoundItems = True
            ddlSCs.DataSource = SC_Dao.GetRWAssociableSpecialCases(memberSSN)
            ddlSCs.DataTextField = "CaseId"
            ddlSCs.DataValueField = "RefId"
            ddlSCs.DataBind()
        End Sub

        Private Sub FillSubCaseTypeList(ByVal parentCaseTypeId As Integer)
            If (parentCaseTypeId < 1 OrElse Not HasSubCaseTypes(parentCaseTypeId)) Then
                ddlSubCaseType.DataSource = New List(Of CaseType)()
            Else
                Dim ct As CaseType = CaseTypeDao.GetById(parentCaseTypeId)

                If (ct Is Nothing) Then
                    ddlSubCaseType.DataSource = New List(Of CaseType)()
                Else
                    ddlSubCaseType.DataSource = ct.SubCaseTypes
                End If
            End If

            ddlSubCaseType.DataTextField = "Name"
            ddlSubCaseType.DataValueField = "Id"
            ddlSubCaseType.DataBind()

            Utility.InsertDropDownListZeroValue(ddlSubCaseType, "--- Select Case Type ---")
        End Sub

        Private Sub FillWWDList(ByVal blnClear As Boolean, ByVal memberSSN As String)
            If Len(memberSSN.Trim()) < 1 Then
                Exit Sub  'Otherwise tries to return ALL LODs ever created
            End If

            ' Add Header Rows
            Dim lstHeader As New ListItem
            Dim blnHeader As Boolean = True
            lstHeader.Value = 0
            lstHeader.Text = "--- NO WWD Case Associated ---"
            For Each currItemH In ddlWWDs.Items  'Keep from loading duplicate values on page reload
                If currItemH.ToString() = lstHeader.Text.ToString() Then
                    blnHeader = False
                End If
            Next
            If blnHeader Then
                ddlWWDs.Items.Add(lstHeader)
            End If
            'Add Data Rows
            If Not blnClear Then
                ddlWWDs.AppendDataBoundItems = True
                If ddlWorkflow.SelectedValue = AFRCWorkflows.SpecCaseMEB Then
                    ddlWWDs.DataSource = Lookup_Dao.GetWWDListByMemberSSN(memberSSN, 1, SESSION_USER_ID)  'ALL

                    ' Next lines (commented out) are WRONG as they cause system to blow up when large number of records exist
                    'ddlWWDs.DataSource = From WWD In SC_Dao.GetAll() Where WWD.MemberSSN = SSNTextBox.Text _
                    '                     And WWD.Workflow = WorkflowType.SpecCaseWWD _
                    '                     Select WWD
                End If
                If (ddlWorkflow.SelectedValue = AFRCWorkflows.SpecCaseWWD OrElse CURR_MODULE_ID = ModuleType.SpecCasePSCD) Then  'WWDs can only be associated with initial expired/expiring WWDs
                    'Utility.InsertDropDownListZeroValue(ddlWWDs, "--- Select WWD Associated ---")
                    ddlWWDs.DataSource = Lookup_Dao.GetWWDListByMemberSSN(memberSSN, 2, SESSION_USER_ID)  'No Associated WWD with Expiration Date

                    ' Next lines (commented out) are WRONG as they cause system to blow up when large number of records exist
                    'ddlWWDs.DataSource = From WWD In SC_Dao.GetAll() Where WWD.MemberSSN = SSNTextBox.Text _
                    '                     And WWD.Workflow = WorkflowType.SpecCaseWWD _
                    '                     And Not IsNothing(WWD.ExpirationDate) _
                    '                     And WWD.AssociatedWWD Is Nothing _
                    '                     Select WWD
                End If
                ddlWWDs.DataValueField = "Value"
                ddlWWDs.DataTextField = "Name"
                'ddlWWDs.DataValueField = "ID"
                'ddlWWDs.DataTextField = "CaseID"
                '            ddlWWDs.DataTextField = "CaseID"
                ddlWWDs.DataBind()
            End If
        End Sub

        Private Function GetIsCaseTypeOtherSelected() As Boolean
            If ddlCaseType.SelectedItem.Text.ToLower() = "other" Then
                Return True
            End If

            Return False
        End Function

        Private Function GetIsSubCaseTypeOtherSelected() As Boolean
            If ddlSubCaseType.SelectedItem.Text.ToLower() = "other" Then
                Return True
            End If

            Return False
        End Function

        Private Function GetRowWithAppUserInformation(table As DataTable) As DataRow
            If (table Is Nothing) Then
                Return Nothing
            End If

            For Each row As DataRow In table.Rows
                If (IsMemberTheUser(row)) Then
                    Return row
                End If
            Next

            Return Nothing
        End Function

        Private Function GetServiceMembersByName(lastName As String, firstName As String, middleName As String) As DataTable
            Dim resultsTable As DataTable = LookupService.GetServerMembersByName(lastName, firstName, middleName)
            Dim rowToRemove As DataRow = GetRowWithAppUserInformation(resultsTable)

            If (rowToRemove IsNot Nothing) Then
                resultsTable.Rows.Remove(rowToRemove)
            End If

            Return resultsTable
        End Function

        Private Function HasSubCaseTypes(ByVal caseTypeId As Integer) As Boolean
            Dim ct As CaseType = CaseTypeDao.GetById(caseTypeId)

            If (ct Is Nothing) Then
                Return False
            End If

            If (ct.SubCaseTypes.Count > 0) Then
                Return True
            End If

            Return False
        End Function

        Private Sub InitMemberCaseHistoryGridView(member As ServiceMember)
            Dim searchParams As StringDictionary = GetSessionDictionary()
            searchParams.Add(MEMBER_SSN_KEY, member.SSN)

            Dim task As PageAsyncTask = New PageAsyncTask(New BeginEventHandler(AddressOf BeginAsyncOperation), New EndEventHandler(AddressOf EndAsyncOperation), Nothing, searchParams)
            RegisterAsyncTask(task)

            Dim taskLOD As PageAsyncTask = New PageAsyncTask(New BeginEventHandler(AddressOf BeginAsyncOperationLOD), New EndEventHandler(AddressOf EndAsyncOperationLOD), Nothing, searchParams)
            RegisterAsyncTask(taskLOD)
        End Sub

        Private Sub InitMemberInformationControls(member As ServiceMember)
            SSNLabel.Text = HTMLEncodeNulls(member.SSN)
            NameLabel.Text = member.FullName.ToUpper()

            If member.Unit IsNot Nothing Then
                UnitNameLabel.Text = HTMLEncodeNulls(member.Unit.Name)
                UnitCodeLabel.Text = HTMLEncodeNulls(member.Unit.PasCode)
                UnitIdLabel.Text = HTMLEncodeNulls(member.Unit.Id)
            End If

            If (member.DateOfBirth.HasValue) Then
                DoBLabel.Text = HTMLEncodeNulls(member.DateOfBirth.Value.ToString(DATE_FORMAT))
            Else
                DoBLabel.Text = Nothing
            End If

            If (member.DateOfBirth.HasValue) Then
                DoBLabel.Text = HTMLEncodeNulls(member.DateOfBirth.Value.ToString(DATE_FORMAT))
            End If

            If member.Rank IsNot Nothing Then
                GradeCodeLabel.Text = HTMLEncodeNulls(member.Rank.Id)
                RankLabel.Text = HTMLEncodeNulls(member.Rank.Title)
            End If

            CompoLabel.Text = HTMLEncodeNulls(member.Component)
        End Sub

        Private Sub InitModuleControls()
            ddlModule.SelectedValue = ModuleId
            ddlModule_SelectedIndexChanged(Nothing, Nothing)
            ddlModule.Enabled = False

            If CURR_MODULE_ID = ModuleType.SpecCaseBMT Then
                ddlSubModule.Visible = True
                BMTModuleSubType.Visible = True
                rowModuleType.Visible = False
            Else
                moduleLabel.Text = ddlModule.SelectedItem.Text
            End If

            If CURR_MODULE_ID = ModuleType.SpecCasePEPP Then
                rowModuleType.Visible = False
                ddlPEPPSubType.Visible = True
            Else
                moduleLabel.Text = ddlModule.SelectedItem.Text
            End If
        End Sub

        Private Sub InitVisibilityOfMemberFoundControls()
            InvalidSSNLabel.Visible = False
            NotFoundLabel.Visible = False
            InputPanel.Visible = False
            MemberSelectionPanel.Visible = False
            lblInvalidMemberName.Visible = False
            lblMemberNotFound.Visible = False
            MemberDataPanel.Visible = True
            HistoryPanelLOD.Visible = True
            HistoryPanel.Visible = True
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

        Private Sub InitVisibilityOfSsnNotFoundControls()
            SSNLabel.Text = String.Empty
            NotFoundLabel.Visible = False
            lblInvalidMemberForSSN.Visible = False
            lblInvalidMemberForName.Visible = False
            MemberDataPanel.Visible = False
            HistoryPanel.Visible = False
        End Sub

        Private Function IsEnteredMemberNameValid()
            If (String.IsNullOrEmpty(txtMemberLastName.Text) AndAlso
                String.IsNullOrEmpty(txtMemberFirstName.Text) AndAlso
                String.IsNullOrEmpty(txtMemberMiddleName.Text)) Then
                Return False
            End If

            Return True
        End Function

        Private Function IsMemberTheUser(row As DataRow) As Boolean
            Dim member = New ServiceMember(DataHelpers.GetStringFromDataRow("SSN", row))
            member.LastName = DataHelpers.GetStringFromDataRow("LastName", row)
            member.FirstName = DataHelpers.GetStringFromDataRow("FirstName", row)
            member.MiddleName = DataHelpers.GetStringFromDataRow("MiddleName", row)

            Return IsMemberTheUser(member)
        End Function

        Private Function IsMemberTheUser(member As ServiceMember) As Boolean
            Return ApplicationUser.IsPersonnalServiceMemberRecord(member)
        End Function

        Private Function LookupServiceMemberByName() As ServiceMember
            Dim ssn As String = String.Empty
            Dim member As ServiceMember = Nothing

            lblInvalidMemberName.Visible = False
            lblMemberNotFound.Visible = False
            lblInvalidMemberForSSN.Visible = False
            lblInvalidMemberForName.Visible = False

            If (Not IsEnteredMemberNameValid()) Then
                lblInvalidMemberName.Visible = True
                MemberSelectionPanel.Visible = False
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

            If (IsMemberTheUser(member)) Then
                lblInvalidMemberForName.Visible = True
                Return Nothing
            End If

            Return member
        End Function

        Private Function LookupServiceMemberBySSN() As ServiceMember
            Dim ssn As String = SSNTextBox.Text.Trim.Replace("-", "")
            Dim member As ServiceMember = Nothing

            If (Not AreEnteredSsnsValid()) Then
                Return Nothing
            End If

            member = LookupService.GetServiceMemberBySSN(ssn)

            If (member Is Nothing) Then
                InitVisibilityOfMemberNotFoundControls()
                Return Nothing
            End If

            If (IsMemberTheUser(member)) Then
                lblInvalidMemberForSSN.Visible = True
                Return Nothing
            End If

            Return member
        End Function

        Private Sub ProcessSelectedMember(ByRef member As ServiceMember)
            If (member Is Nothing) Then
                Exit Sub
            End If

            InitVisibilityOfMemberFoundControls()
            InitMemberInformationControls(member)
            InitModuleControls()
            InitMemberCaseHistoryGridView(member)
        End Sub

        Private Sub ResultGrid_RowCommand(ByVal sender As Object, ByVal e As GridViewCommandEventArgs) Handles HistoryGrid.RowCommand
            If (e.CommandName = "view") Then
                Dim parts() As String = e.CommandArgument.ToString().Split(";")
                Dim strQuery As New StringBuilder()
                Dim args As New ItemSelectedEventArgs
                args.RefId = parts(0)

                strQuery.Append("refId=" + CType(args.RefId, String))
                args.Type = parts(1)

                args.Url = GetWorkflowInitPageURL(args.Type, args.RefId)

                Session("RefId") = args.RefId
                Response.Redirect(args.Url)
            End If
        End Sub

        Private Sub ResultGridLOD_RowCommand(ByVal sender As Object, ByVal e As GridViewCommandEventArgs) Handles HistoryGridLOD.RowCommand
            If (e.CommandName = "view") Then
                Dim parts() As String = e.CommandArgument.ToString().Split(";")
                Dim strQuery As New StringBuilder()
                Dim args As New ItemSelectedEventArgs
                args.RefId = parts(0)

                strQuery.Append("refId=" + CType(args.RefId, String))
                args.Type = ModuleType.LOD

                Select Case args.Type
                    Case ModuleType.LOD
                        args.Url = "~/Secure/LOD/init.aspx?" + strQuery.ToString()
                        Session("RefId") = args.RefId
                        Response.Redirect(args.Url)
                End Select
            End If
        End Sub

        Private Sub TurnSubCaseTypeRowOnOff(ByVal isOn As Boolean)
            SubCaseTypeRow.Visible = isOn
            SubCaseTypeListHeader.Visible = isOn
            subCaseTypeListLabel.Visible = isOn
            subCaseTypeList.Visible = isOn

            If (isOn) Then
                sectionAction.Text = "I"
                sectionErrors.Text = "J"
            Else
                sectionAction.Text = "H"
                sectionErrors.Text = "I"

            End If
        End Sub

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
                ValidationErrorsRow.Visible = True
                ValidationList.Visible = True
                ValidationList.DataSource = errors
                ValidationList.DataBind()
                Return False
            Else
                ValidationErrorsRow.Visible = False
            End If

            ValidationList.Visible = False
            Return True
        End Function

#End Region

    End Class

End Namespace