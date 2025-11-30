Imports ALOD.Core.Domain.Modules.SpecialCases
Imports ALOD.Core.Domain.Workflow
Imports ALOD.Core.Interfaces.DAOInterfaces
Imports ALOD.Core.Utils
Imports ALOD.Data
Imports ALOD.Data.Services
Imports ALOD.Logging
Imports ALODWebUtility.Common

Namespace Web.Special_Case

    Partial Class Secure_SpecialCases_Override
        Inherits System.Web.UI.Page

        Private _userDao As IUserDao
        Dim dao As ISpecialCaseDAO = Nothing
        Private moduleId As Integer
        Private specialCase As SpecialCase = Nothing

        ReadOnly Property SCDao() As ISpecialCaseDAO
            Get
                If (dao Is Nothing) Then
                    dao = New NHibernateDaoFactory().GetSpecialCaseDAO()
                End If

                Return dao
            End Get
        End Property

        Public ReadOnly Property CaseId() As String
            Get
                Return Server.HtmlEncode(CaseIdbox.Text.Trim)
            End Get

        End Property

        Protected Property SpecCase() As SpecialCase
            Get
                If (specialCase Is Nothing) Then
                    specialCase = GetCorrespondingSpecialCase(SCDao.GetSpecialCaseByCaseId(CaseId))
                End If
                Return specialCase
            End Get
            Set(ByVal value As SpecialCase)
                specialCase = value
            End Set
        End Property

        Protected Property SpecCaseModuleType() As Integer
            Get
                Return moduleId
            End Get
            Set(value As Integer)
                moduleId = value
            End Set
        End Property

        Protected ReadOnly Property UserDao As IUserDao
            Get
                If (_userDao Is Nothing) Then
                    _userDao = New NHibernateDaoFactory().GetUserDao()
                End If

                Return _userDao
            End Get
        End Property

        Public Sub ChangeStatus()

            Dim workStatusDao As IWorkStatusDao
            Dim reminderEmailDao As IReminderEmailDao
            Dim oldStatusId As Integer
            Dim newStatusId As Integer

            workStatusDao = New NHibernateDaoFactory().GetWorkStatusDao()
            reminderEmailDao = New ReminderEmailsDao()

            If StatusSelect.SelectedValue <> "" Then
                oldStatusId = SpecCase.Status
                newStatusId = CInt(StatusSelect.SelectedValue)

                SpecCase.PerformOverride(workStatusDao, reminderEmailDao, newStatusId, oldStatusId)
                SCDao.SaveOrUpdate(SpecCase)
                LogManager.LogAction(SpecCaseModuleType, UserAction.Override, SpecCase.Id, "Override Status Action", oldStatusId, newStatusId)

            End If

        End Sub

        Public Sub PopulateData()

            If Not (SpecCase Is Nothing) Then

                ' Check which type of case it is...
                If (SpecCase.Workflow = AFRCWorkflows.SpecCasePH) Then
                    ' PH cases don't revolve around a member so grab the case data from the DPH User who created the case...
                    Dim phCase As SC_PH = CType(SpecCase, SC_PH)

                    Namelbl.Text = phCase.DPHUser.FullName
                    Ranklbl.Text = phCase.DPHUser.Rank.Title
                    Unitlbl.Text = phCase.DPHUser.CurrentUnitName
                Else
                    Namelbl.Text = SpecCase.MemberName
                    Ranklbl.Text = SpecCase.MemberRank.Title
                    Unitlbl.Text = SpecCase.MemberUnit
                End If

                caseIdLbl.Text = CaseId
                Statuslbl.Text = SpecCase.WorkflowStatus.Description

                ' Populate the status DDL with statuses specific to the workflow of the case being overriden...
                StatusSelect.DataSource = LookupService.GetWorkstatusByWorkflow(SpecCase.Workflow)
                StatusSelect.DataBind()
                StatusSelect.SelectedValue = SpecCase.WorkflowStatus.Id

                memberPanel.Visible = True
            End If
        End Sub

        Public Sub Search()
            errlbl.Text = ""
            Namelbl.Text = ""
            Ranklbl.Text = ""
            Unitlbl.Text = ""
            Statuslbl.Text = ""
            caseIdLbl.Text = ""
            memberPanel.Visible = False

            If CaseId = String.Empty Then
                errlbl.Text = "Please enter a caseId"
                Exit Sub
            End If

            If CaseId.Length < 15 Then
                errlbl.Text = "Invalid case Id"
                Exit Sub
            End If

            If (SpecCase Is Nothing) Then
                errlbl.Text = "Case does not exist or you do not have override permissions"
                Exit Sub
            End If

            PopulateData()

        End Sub

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
            If (Not IsPostBack) Then
                SetInputFormatRestriction(Page, CaseIdbox, FormatRestriction.AlphaNumeric, "-")
            End If
        End Sub

        Protected Sub SearchButton_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles SearchButton.Click
            Search()

        End Sub

        Protected Sub StatusChangeBtn_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles StatusChangeBtn.Click

            If Not (SpecCase Is Nothing) Then
                ChangeStatus()
                'SpecCase = Nothing
                PopulateData()
            End If
        End Sub

        Private Function GetCorrespondingSpecialCase(ByRef sc As SpecialCase) As SpecialCase
            If (sc Is Nothing) Then
                Return Nothing
            End If

            Select Case sc.Workflow
                Case AFRCWorkflows.SpecCaseBCMR
                    Dim scTemp As SC_BCMR = sc
                    SpecCaseModuleType = ModuleType.SpecCaseBCMR
                    Return scTemp

                Case AFRCWorkflows.SpecCaseBMT
                    Dim scTemp As SC_BMT = sc
                    SpecCaseModuleType = ModuleType.SpecCaseBMT
                    Return scTemp

                Case AFRCWorkflows.SpecCaseCMAS
                    Dim scTemp As SC_CMAS = sc
                    SpecCaseModuleType = ModuleType.SpecCaseCMAS
                    Return scTemp

                Case AFRCWorkflows.SpecCaseCongress
                    Dim scTemp As SC_Congress = sc
                    SpecCaseModuleType = ModuleType.SpecCaseCongress
                    Return scTemp

                Case AFRCWorkflows.SpecCaseDW
                    Dim scTemp As SC_DW = sc
                    SpecCaseModuleType = ModuleType.SpecCaseDW
                    Return scTemp

                Case AFRCWorkflows.SpecCaseFT
                    Dim scTemp As SC_FastTrack = sc
                    SpecCaseModuleType = ModuleType.SpecCaseFT
                    Return scTemp

                Case AFRCWorkflows.SpecCaseIncap
                    Dim scTemp As SC_Incap = sc
                    SpecCaseModuleType = ModuleType.SpecCaseIncap
                    Return scTemp

                Case AFRCWorkflows.SpecCaseMEB
                    Dim scTemp As SC_MEB = sc
                    SpecCaseModuleType = ModuleType.SpecCaseMEB
                    Return scTemp

                Case AFRCWorkflows.SpecCaseMH
                    Dim scTemp As SC_MH = sc
                    SpecCaseModuleType = ModuleType.SpecCaseMH
                    Return scTemp

                Case AFRCWorkflows.SpecCaseMMSO
                    Dim scTemp As SC_MMSO = sc
                    SpecCaseModuleType = ModuleType.SpecCaseMMSO
                    Return scTemp

                Case AFRCWorkflows.SpecCaseMO
                    Dim scTemp As SC_MO = sc
                    SpecCaseModuleType = ModuleType.SpecCaseMO
                    Return scTemp

                Case AFRCWorkflows.SpecCaseNE
                    Dim scTemp As SC_NE = sc
                    SpecCaseModuleType = ModuleType.SpecCaseNE
                    Return scTemp

                Case AFRCWorkflows.SpecCasePEPP
                    Dim scTemp As SC_PEPP = sc
                    SpecCaseModuleType = ModuleType.SpecCasePEPP
                    Return scTemp

                Case AFRCWorkflows.SpecCasePH
                    Dim scTemp As SC_PH = sc
                    SpecCaseModuleType = ModuleType.SpecCasePH
                    Return scTemp

                Case AFRCWorkflows.SpecCasePW
                    Dim scTemp As SC_PWaivers = sc
                    SpecCaseModuleType = ModuleType.SpecCasePW
                    Return scTemp

                Case AFRCWorkflows.SpecCaseRS
                    Dim scTemp As SC_RS = sc
                    SpecCaseModuleType = ModuleType.SpecCaseRS
                    Return scTemp

                Case AFRCWorkflows.SpecCaseRW
                    Dim scTemp As SC_RW = sc
                    SpecCaseModuleType = ModuleType.SpecCaseRW
                    Return scTemp

                Case AFRCWorkflows.SpecCaseWWD
                    Dim scTemp As SC_WWD = sc
                    SpecCaseModuleType = ModuleType.SpecCaseWWD
                    Return scTemp

                Case Else
                    Return Nothing
            End Select
        End Function

    End Class

End Namespace