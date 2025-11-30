Imports System
Imports ALOD.Core.Interfaces
Imports ALOD.Data
Imports ALOD.Data.Services
Imports ALOD.Core.Domain.Modules.Lod
Imports ALOD.Core.Domain.Common
Imports ALOD.Core.Domain.Users
Imports ALOD.Logging
Imports ALODWebUtility.Common
Imports ALOD.Core.Domain.Modules.Appeals
Imports ALOD.Core.Domain.Modules.SARC
Imports ALOD.Core.Interfaces.DAOInterfaces
Imports ALOD.Core.Domain.Modules.Reinvestigations
Imports ALOD.Core.Domain.Workflow
Imports ALOD.Core.Utils

Namespace Web.LOD
    Partial Class Secure_lod_Override
        Inherits System.Web.UI.Page

        Public Const NO_CASE_OR_OVERRIDE_PERMISSION As String = "Case does not exist or you do not have override permissions"

        Private _daoFactory As NHibernateDaoFactory
        Private _lineOfDuty As LineOfDuty = Nothing
        Private _reinvestigationRequest As LODReinvestigation = Nothing
        Private _appeal As LODAppeal = Nothing
        Private _sarc As RestrictedSARC = Nothing
        Private _sarcAppeal As SARCAppeal = Nothing

        Protected ReadOnly Property DaoFactory As NHibernateDaoFactory
            Get
                If (_daoFactory Is Nothing) Then
                    _daoFactory = New NHibernateDaoFactory()
                End If

                Return _daoFactory
            End Get
        End Property

        Protected ReadOnly Property LineOfDutyDao As ILineOfDutyDao
            Get
                Return DaoFactory.GetLineOfDutyDao()
            End Get
        End Property

        Protected ReadOnly Property ReinvestigationRequestDao As ILODReinvestigateDAO
            Get
                Return DaoFactory.GetLODReinvestigationDao()
            End Get
        End Property

        Protected ReadOnly Property AppealDao As ILODAppealDAO
            Get
                Return DaoFactory.GetLODAppealDao
            End Get
        End Property

        Protected ReadOnly Property SARCDao As ISARCDAO
            Get
                Return DaoFactory.GetSARCDao()
            End Get
        End Property

        Protected ReadOnly Property SARCAppealDao As ISARCAppealDAO
            Get
                Return DaoFactory.GetSARCAppealDao()
            End Get
        End Property

        Protected Property LOD As LineOfDuty
            Get
                If (_lineOfDuty Is Nothing) Then
                    _lineOfDuty = DaoFactory.GetLodSearchDao.GetByCaseId(CaseId)
                End If
                Return _lineOfDuty
            End Get
            Set(ByVal value As LineOfDuty)
                _lineOfDuty = value
            End Set
        End Property

        Protected Property RR As LODReinvestigation
            Get
                If (_reinvestigationRequest Is Nothing) Then
                    _reinvestigationRequest = ReinvestigationRequestDao.GetByCaseId(CaseId)
                End If

                Return _reinvestigationRequest
            End Get
            Set(value As LODReinvestigation)
                _reinvestigationRequest = value
            End Set
        End Property

        Protected Property AP As LODAppeal
            Get
                If (_appeal Is Nothing) Then
                    _appeal = AppealDao.GetByCaseId(CaseId)
                End If
                Return _appeal
            End Get
            Set(ByVal value As LODAppeal)
                _appeal = value
            End Set
        End Property

        Protected Property SARC As RestrictedSARC
            Get
                If (_sarc Is Nothing) Then
                    _sarc = SARCDao.GetByCaseId(CaseId)
                End If
                Return _sarc
            End Get
            Set(ByVal value As RestrictedSARC)
                _sarc = value
            End Set
        End Property

        Protected Property SARCAppeal As SARCAppeal
            Get
                If (_sarcAppeal Is Nothing) Then
                    _sarcAppeal = SARCAppealDao.GetByCaseId(CaseId)
                End If

                Return _sarcAppeal
            End Get
            Set(value As SARCAppeal)
                _sarcAppeal = value
            End Set
        End Property

        Public ReadOnly Property CaseId As String
            Get
                Return Server.HtmlEncode(CaseIdbox.Text.Trim)
            End Get
        End Property

        Protected ReadOnly Property SelectedModule As ModuleType
            Get
                Return ddlModules.SelectedValue
            End Get
        End Property

        Protected Sub SearchButton_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles SearchButton.Click
            Search()
        End Sub

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
            If (Not IsPostBack) Then
                SetInputFormatRestriction(Page, CaseIdbox, FormatRestriction.AlphaNumeric, "-")
                InitControls()
            End If
        End Sub

        Protected Sub InitControls()
            PopulateWorklowDDL()
        End Sub

        Protected Sub PopulateWorklowDDL()
            ddlModules.DataSource = GetOverrideableModules()
            ddlModules.DataValueField = "Id"
            ddlModules.DataTextField = "Name"
            ddlModules.DataBind()

            For Each item As ListItem In ddlModules.Items
                If (item.Text.Equals("LOD")) Then
                    item.Text = "Line of Duty"
                End If
            Next

            Utility.InsertDropDownListZeroValue(ddlModules, "-- Select Workflow --")
        End Sub

        Protected Function GetOverrideableModules() As List(Of ALOD.Core.Domain.Workflow.Module)
            Dim modules As List(Of ALOD.Core.Domain.Workflow.Module) = ALOD.Core.Utils.DataHelpers.ExtractObjectsFromDataSet(Of ALOD.Core.Domain.Workflow.Module)(LookupService.GetAllModules())

            Return modules.Where(Function(x) x.IsSpecialCase = False AndAlso Not x.Name.Equals("System")).ToList()
        End Function

        Protected Sub PopulateWorkStatusDDL(ByVal workflowId As Integer)
            Dim workStatusDao As IWorkStatusDao = DaoFactory.GetWorkStatusDao()

            ddlWorkStatus.DataSource = workStatusDao.GetByWorkflow(workflowId).OrderBy(Function(x) x.Description)
            ddlWorkStatus.DataValueField = "Id"
            ddlWorkStatus.DataTextField = "Description"
            ddlWorkStatus.DataBind()
        End Sub

        Public Sub Search()
            ResetCaseControls()

            If (SelectedModule = 0) Then
                errlbl.Text = "Please select a workflow"
                trErrors.Visible = True
                Exit Sub
            End If

            If (CaseId = String.Empty) Then
                errlbl.Text = "Please enter a caseId"
                trErrors.Visible = True
                Exit Sub
            End If

            If (CaseId.Length < GetCaseIdMinimumLengthForModule(SelectedModule)) Then
                errlbl.Text = "Invalid case Id"
                trErrors.Visible = True
                Exit Sub
            End If

            Select Case SelectedModule
                Case ModuleType.LOD
                    If (LOD Is Nothing) Then
                        SetCaseLoadingError()
                        Exit Sub
                    End If

                    PopulateLODData()

                Case ModuleType.ReinvestigationRequest
                    If (RR Is Nothing) Then
                        SetCaseLoadingError()
                        Exit Sub
                    End If

                    PopulateRRData()

                Case ModuleType.AppealRequest
                    If (AP Is Nothing) Then
                        SetCaseLoadingError()
                        Exit Sub
                    End If

                    PopulateAppealData()

                Case ModuleType.SARC
                    If (SARC Is Nothing) Then
                        SetCaseLoadingError()
                        Exit Sub
                    End If

                    PopulateSARCData()

                Case ModuleType.SARCAppeal
                    If (SARCAppeal Is Nothing) Then
                        SetCaseLoadingError()
                        Exit Sub
                    End If

                    PopulateSARCAppealData()
            End Select
        End Sub

        Protected Sub SetCaseLoadingError()
            errlbl.Text = NO_CASE_OR_OVERRIDE_PERMISSION
            trErrors.Visible = True
        End Sub

        Public Sub ResetCaseControls()
            errlbl.Text = ""
            Namelbl.Text = ""
            Ranklbl.Text = ""
            Unitlbl.Text = ""
            Statuslbl.Text = ""
            lblCaseId.Text = ""
            pnlCaseInfo.Visible = False
            trErrors.Visible = False
        End Sub

        Protected Function GetCaseIdMinimumLengthForModule(ByVal modType As ModuleType) As Integer
            Select Case modType
                Case ModuleType.LOD
                    Return 12

                Case ModuleType.ReinvestigationRequest
                    Return 15

                Case ModuleType.SARC
                    Return 15

                Case ModuleType.AppealRequest
                    Return 19

                Case ModuleType.SARCAppeal
                    Return 21
                Case Else
                    Return 12
            End Select
        End Function

        Protected Sub PopulateLODData()
            If Not (LOD Is Nothing) Then
                Namelbl.Text = LOD.MemberName
                Ranklbl.Text = LOD.MemberRank.Title
                Unitlbl.Text = LOD.MemberUnit
                lblCaseId.Text = "for CASE ID: " & CaseId
                Statuslbl.Text = LOD.WorkflowStatus.Description
                PopulateWorkStatusDDL(LOD.Workflow)
                ddlWorkStatus.SelectedValue = LOD.WorkflowStatus.Id
                pnlCaseInfo.Visible = True
            End If
        End Sub

        Protected Sub PopulateRRData()
            If Not (RR Is Nothing) Then
                Namelbl.Text = RR.MemberName
                Ranklbl.Text = RR.MemberRank.Title
                Unitlbl.Text = RR.MemberUnit
                lblCaseId.Text = "for CASE ID: " & CaseId
                Statuslbl.Text = RR.WorkflowStatus.Description
                PopulateWorkStatusDDL(RR.Workflow)
                ddlWorkStatus.SelectedValue = RR.WorkflowStatus.Id
                pnlCaseInfo.Visible = True
            End If
        End Sub

        Protected Sub PopulateAppealData()
            If Not (AP Is Nothing) Then
                Namelbl.Text = AP.MemberName
                Ranklbl.Text = AP.MemberRank.Title
                Unitlbl.Text = AP.MemberUnit
                lblCaseId.Text = "for CASE ID: " & CaseId
                Statuslbl.Text = AP.WorkflowStatus.Description
                PopulateWorkStatusDDL(AP.Workflow)
                ddlWorkStatus.SelectedValue = AP.WorkflowStatus.Id
                pnlCaseInfo.Visible = True
            End If
        End Sub

        Protected Sub PopulateSARCData()
            If Not (SARC Is Nothing) Then
                Namelbl.Text = SARC.MemberName
                Ranklbl.Text = SARC.MemberRank.Title
                Unitlbl.Text = SARC.MemberUnit
                lblCaseId.Text = "for CASE ID: " & CaseId
                Statuslbl.Text = SARC.WorkflowStatus.Description
                PopulateWorkStatusDDL(SARC.Workflow)
                ddlWorkStatus.SelectedValue = SARC.WorkflowStatus.Id
                pnlCaseInfo.Visible = True
            End If
        End Sub

        Protected Sub PopulateSARCAppealData()
            If Not (SARCAppeal Is Nothing) Then
                Namelbl.Text = SARCAppeal.MemberName
                Ranklbl.Text = SARCAppeal.MemberRank.Title
                Unitlbl.Text = SARCAppeal.MemberUnit
                lblCaseId.Text = "for CASE ID: " & CaseId
                Statuslbl.Text = SARCAppeal.WorkflowStatus.Description
                PopulateWorkStatusDDL(SARCAppeal.Workflow)
                ddlWorkStatus.SelectedValue = SARCAppeal.WorkflowStatus.Id
                pnlCaseInfo.Visible = True
            End If
        End Sub

        Public Sub ChangeStatus()
            Dim workStatusDao As IWorkStatusDao = DaoFactory.GetWorkStatusDao()
            Dim reminderEmailDao As ReminderEmailsDao = New ReminderEmailsDao()

            Dim refId As Integer
            Dim oldStatusId As Integer
            Dim newStatusId As Integer

            If ddlWorkStatus.SelectedValue <> "" Then
                newStatusId = CInt(ddlWorkStatus.SelectedValue)

                Select Case SelectedModule
                    Case ModuleType.LOD
                        refId = LOD.Id
                        oldStatusId = LOD.Status
                        LOD.PerformOverride(DaoFactory, newStatusId, oldStatusId)
                        LineOfDutyDao.SaveOrUpdate(LOD)

                    Case ModuleType.ReinvestigationRequest
                        refId = RR.Id
                        oldStatusId = RR.Status
                        RR.PerformOverride(DaoFactory, newStatusId, oldStatusId)
                        ReinvestigationRequestDao.SaveOrUpdate(RR)

                    Case ModuleType.AppealRequest
                        refId = AP.Id
                        oldStatusId = AP.Status
                        AP.PerformOverride(DaoFactory, newStatusId, oldStatusId)
                        AppealDao.SaveOrUpdate(AP)

                    Case ModuleType.SARC
                        refId = SARC.Id
                        oldStatusId = SARC.Status
                        SARC.PerformOverride(DaoFactory, newStatusId, oldStatusId)
                        SARCDao.SaveOrUpdate(SARC)

                    Case ModuleType.SARCAppeal
                        refId = SARCAppeal.Id
                        oldStatusId = SARCAppeal.Status
                        SARCAppeal.PerformOverride(DaoFactory, newStatusId, oldStatusId)
                        SARCAppealDao.SaveOrUpdate(SARCAppeal)
                End Select

                LogManager.LogAction(SelectedModule, UserAction.Override, refId, "Override Status Action", oldStatusId, newStatusId)
            End If
        End Sub

        Protected Sub StatusChangeBtn_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles StatusChangeBtn.Click
            If (SelectedModule = ModuleType.LOD AndAlso LOD IsNot Nothing) Then
                ChangeStatus()
                LOD = Nothing
                PopulateLODData()
            ElseIf (SelectedModule = ModuleType.ReinvestigationRequest AndAlso RR IsNot Nothing) Then
                ChangeStatus()
                RR = Nothing
                PopulateRRData()
            ElseIf (SelectedModule = ModuleType.AppealRequest AndAlso AP IsNot Nothing) Then
                ChangeStatus()
                AP = Nothing
                PopulateAppealData()
            ElseIf (SelectedModule = ModuleType.SARC AndAlso SARC IsNot Nothing) Then
                ChangeStatus()
                SARC = Nothing
                PopulateSARCData()
            ElseIf (SelectedModule = ModuleType.SARCAppeal AndAlso SARCAppeal IsNot Nothing) Then
                ChangeStatus()
                SARCAppeal = Nothing
                PopulateSARCAppealData()
            End If
        End Sub
    End Class
End Namespace
