Imports ALOD.Core.Domain.Common
Imports ALOD.Core.Domain.Modules.SpecialCases
Imports ALOD.Core.Domain.Users
Imports ALOD.Core.Domain.Workflow
Imports ALOD.Core.Interfaces.DAOInterfaces
Imports ALOD.Data
Imports ALOD.Logging
Imports ALOD.Web.UserControls
Imports ALODWebUtility.Common

Namespace Web.Special_Case.IN

    Partial Class Secure_sc_in_HQTech
        Inherits System.Web.UI.Page

#Region "IncapProperty"

        Private _daoFactory As IDaoFactory
        Private _scAccess As Dictionary(Of String, ALOD.Core.Domain.Workflow.PageAccessType)
        Private _specCaseDao As ISpecialCaseDAO
        Dim dao As ISpecialCaseDAO
        Private sc As SC_Incap = Nothing
        Private scId As Integer = 0

        ReadOnly Property SCDao() As ISpecialCaseDAO
            Get
                If (dao Is Nothing) Then
                    dao = New NHibernateDaoFactory().GetSpecialCaseDAO()
                End If

                Return dao
            End Get
        End Property

        Public ReadOnly Property ReferenceId() As Integer
            Get
                Return Integer.Parse(Request.QueryString("refId"))
            End Get
        End Property

        Protected ReadOnly Property CalendarImage() As String
            Get
                Return Me.ResolveClientUrl("~/App_Themes/" + Page.Theme + "/Images/Calendar.gif")
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

        Protected ReadOnly Property SpecCase() As SC_Incap
            Get
                If (sc Is Nothing) Then
                    sc = SCDao.GetById(ReferenceId)
                End If

                Return sc
            End Get
        End Property

        Protected ReadOnly Property SpecCaseDao() As ISpecialCaseDAO
            Get
                If (_specCaseDao Is Nothing) Then
                    _specCaseDao = DaoFactory.GetSpecialCaseDAO()
                End If

                Return _specCaseDao
            End Get
        End Property

#End Region

        Public ReadOnly Property Navigator() As TabNavigator
            Get
                Return MasterPage.Navigator
            End Get
        End Property

        Public ReadOnly Property TabControl() As TabControls
            Get
                Return Master.TabControl
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

        Protected ReadOnly Property MasterPage() As SC_IncapMaster
            Get
                Dim master As SC_IncapMaster = CType(Page.Master, SC_IncapMaster)
                Return master
            End Get
        End Property

        Protected ReadOnly Property SectionList() As Dictionary(Of String, ALOD.Core.Domain.Workflow.PageAccessType)
            Get
                If (_scAccess Is Nothing) Then
                    _scAccess = SpecCase.ReadSectionList(CInt(Session("GroupId")))
                End If
                Return _scAccess
            End Get
        End Property

        Protected ReadOnly Property WsId() As Integer
            Get
                Dim ws_id As Integer = SESSION_WS_ID(RefId)
                Return ws_id
            End Get
        End Property

        Private ReadOnly Property RefId() As Integer
            Get
                Return CInt(Request.QueryString("refId"))
            End Get
        End Property

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

            If (Not IsPostBack) Then
                UserCanEdit = GetAccess(Navigator.PageAccess, True)
                InitControls()
                '    SetMaxLength(CommentsTB)

                '    'Validation
                '    If (UserCanEdit) Then
                '        sc.Validate()
                '        If (sc.Validations.Count > 0) Then
                '            ShowPageValidationErrors(sc.Validations, Me)
                '        End If
                '    End If

            End If

            TabControl.Item(NavigatorButtonType.Save).Visible = UserCanEdit
            LogManager.LogAction(ModuleType.SpecCaseIncap, UserAction.ViewPage, RefId, "Viewed Page: IN HQ AFRC Tech")
        End Sub

        'controls which questions are displayed
        Protected Sub ResetAll(ByVal wsId As Integer)
            Dim workflowType As String = GetWorkflowType(wsId)
            Select Case workflowType
                Case "Ext"
                    OPRExtSection.Visible = True
                    OCRExtSection.Visible = True
                    DOSExtSection.Visible = True
                    CCRExtSection.Visible = True
                    VCRExtSection.Visible = True
                    DOPExtSection.Visible = True
                    CAFRExtSection.Visible = True
                Case "Appeal"
                    OPRIntAppealSection.Visible = True
                    OCRIntAppealSection.Visible = True
                    DOSAppealSection.Visible = True
                    CCRAppealSection.Visible = True
                    VCRAppealSection.Visible = True
                    DOPAppealSection.Visible = True
                    CAFRAppealSection.Visible = True
                    'Extension Appeal
                    'Case
            End Select
        End Sub

        Protected Sub ViewEnableControls()
            Dim wsId As Integer = SESSION_WS_ID(RefId)
            ResetAll(wsId)
            'controls which questions are enabled for read/write
            Select Case wsId
                'Extension
                Case AFRCWorkflows.INOPR_Ext_HR_Review
                    If (SESSION_GROUP_ID = UserGroups.HumanResoures_OPR) Then
                        rblOPRExtQuestion.Enabled = True
                    End If
                Case AFRCWorkflows.INOCR_Ext_HR_Review
                    If (SESSION_GROUP_ID = UserGroups.HumanResoures_OCR) Then
                        rblOCRExtQuestion.Enabled = True
                    End If
                Case AFRCWorkflows.INDirectorOfStaffReview
                    If (SESSION_GROUP_ID = UserGroups.AFR_DirectorOfStaff) Then
                        rblDOSExtQuestion.Enabled = True
                    End If
                Case AFRCWorkflows.INCommandChiefReview
                    If (SESSION_GROUP_ID = UserGroups.AFR_CommandChief) Then
                        rblCCRExtQuestion.Enabled = True
                    End If
                Case AFRCWorkflows.INViceCommanderReview
                    If (SESSION_GROUP_ID = UserGroups.AFR_ViceCommander) Then
                        rblVCRExtQuestion.Enabled = True
                    End If
                Case AFRCWorkflows.INDirectorOfPersonnelReview
                    If (SESSION_GROUP_ID = UserGroups.AFR_DirectorOfPersonnel) Then
                        rblDOPExtQuestion.Enabled = True
                    End If
                Case AFRCWorkflows.INCAFRAction
                    If (SESSION_GROUP_ID = UserGroups.ChiefAirForceReserve) Then
                        rblCAFRExtQuestion.Enabled = True
                    End If
                'Appeal
                Case AFRCWorkflows.INOPR_Appeal_HR_Review
                    If (SESSION_GROUP_ID = UserGroups.HumanResoures_OPR) Then
                        rblOPRAppealQuestion.Enabled = True
                    End If
                Case AFRCWorkflows.INOCR_Appeal_HR_Review
                    If (SESSION_GROUP_ID = UserGroups.HumanResoures_OCR) Then
                        rblOCRAppealQuestion.Enabled = True
                    End If
                Case AFRCWorkflows.INDirectorOfStaffReview_Appeal
                    If (SESSION_GROUP_ID = UserGroups.AFR_DirectorOfStaff) Then
                        rblDOSAppealQuestion.Enabled = True
                    End If
                Case AFRCWorkflows.INCommandChiefReview_Appeal
                    If (SESSION_GROUP_ID = UserGroups.AFR_CommandChief) Then
                        rblCCRAppealQuestion.Enabled = True
                    End If
                Case AFRCWorkflows.INViceCommanderReview_Appeal
                    If (SESSION_GROUP_ID = UserGroups.AFR_ViceCommander) Then
                        rblVCRAppealQuestion.Enabled = True
                    End If
                Case AFRCWorkflows.INDirectorOfPersonnelReview_Appeal
                    If (SESSION_GROUP_ID = UserGroups.AFR_DirectorOfPersonnel) Then
                        rblDOPAppealQuestion.Enabled = True
                    End If
                Case AFRCWorkflows.INCAFR_Action_Appeal
                    If (SESSION_GROUP_ID = UserGroups.ChiefAirForceReserve) Then
                        rblCAFRAppealQuestion.Enabled = True
                    End If
            End Select
        End Sub

        Private Function GetWorkflowType(ByVal wsid As Integer) As String
            Dim type As String = ""
            Select Case wsid
                Case AFRCWorkflows.INExtension, AFRCWorkflows.INMedicalReview_WG_Ext, AFRCWorkflows.INImmediateCommanderReview_Ext,
                     AFRCWorkflows.INWingJAReview_Ext, AFRCWorkflows.INFinanceReview_Ext, AFRCWorkflows.INWingCommanderRecommendation_Ext,
                     AFRCWorkflows.INOCR_Ext_HR_Review, AFRCWorkflows.INOPR_Ext_HR_Review, AFRCWorkflows.INDirectorOfStaffReview,
                     AFRCWorkflows.INDirectorOfPersonnelReview, AFRCWorkflows.INCommandChiefReview, AFRCWorkflows.INViceCommanderReview,
                     AFRCWorkflows.INCAFRAction
                    type = "Ext"
                Case AFRCWorkflows.INOPR_Appeal_HR_Review, AFRCWorkflows.INOCR_Appeal_HR_Review, AFRCWorkflows.INDirectorOfStaffReview_Appeal,
                     AFRCWorkflows.INAppeal, AFRCWorkflows.INWingCCRecommendation_Appeal, AFRCWorkflows.INCommandChiefReview_Appeal,
                     AFRCWorkflows.INViceCommanderReview_Appeal, AFRCWorkflows.INDirectorOfPersonnelReview_Appeal, AFRCWorkflows.INCAFR_Action_Appeal
                    type = "Appeal"

            End Select
            Return type
        End Function

        Private Sub InitControls()
            ViewEnableControls()
            LoadFindings()
        End Sub

        Private Sub LoadAppealFindings()
            Dim results As SC_IncapAppeal_Findings = SpecCase.INCAPAppealFindings.First

            If (results.OPR_AppealApproval.HasValue) Then
                rblOPRAppealQuestion.SelectedValue = Convert.ToInt32(results.OPR_AppealApproval.Value)
            End If
            If (results.OCR_AppealApproval.HasValue) Then
                rblOCRAppealQuestion.SelectedValue = Convert.ToInt32(results.OCR_AppealApproval.Value)
            End If
            If (results.DOS_AppealApproval.HasValue) Then
                rblDOSAppealQuestion.SelectedValue = Convert.ToInt32(results.DOS_AppealApproval.Value)
            End If
            If (results.CCR_AppealApproval.HasValue) Then
                rblCCRAppealQuestion.SelectedValue = Convert.ToInt32(results.CCR_AppealApproval.Value)
            End If
            If (results.VCR_AppealApproval.HasValue) Then
                rblVCRAppealQuestion.SelectedValue = Convert.ToInt32(results.VCR_AppealApproval.Value)
            End If
            If (results.DOP_AppealApproval.HasValue) Then
                rblDOPAppealQuestion.SelectedValue = Convert.ToInt32(results.DOP_AppealApproval.Value)
            End If
            If (results.CAFR_AppealApproval.HasValue) Then
                rblCAFRAppealQuestion.SelectedValue = Convert.ToInt32(results.CAFR_AppealApproval.Value)
            End If
        End Sub

        Private Sub LoadExtFindings()
            Dim results As SC_IncapExt_Findings = SpecCase.INCAPExtFindings.First

            If (results.OPR_ExtApproval.HasValue) Then
                rblOPRExtQuestion.SelectedValue = Convert.ToInt32(results.OPR_ExtApproval.Value)
            End If
            If (results.OCR_ExtApproval.HasValue) Then
                rblOCRExtQuestion.SelectedValue = Convert.ToInt32(results.OCR_ExtApproval.Value)
            End If
            If (results.DOS_ExtApproval.HasValue) Then
                rblDOSExtQuestion.SelectedValue = Convert.ToInt32(results.DOS_ExtApproval.Value)
            End If
            If (results.CCR_ExtApproval.HasValue) Then
                rblCCRExtQuestion.SelectedValue = Convert.ToInt32(results.CCR_ExtApproval.Value)
            End If
            If (results.VCR_ExtApproval.HasValue) Then
                rblVCRExtQuestion.SelectedValue = Convert.ToInt32(results.VCR_ExtApproval.Value)
            End If
            If (results.DOP_ExtApproval.HasValue) Then
                rblDOPExtQuestion.SelectedValue = Convert.ToInt32(results.DOP_ExtApproval.Value)
            End If
            If (results.CAFR_ExtApproval.HasValue) Then
                rblCAFRExtQuestion.SelectedValue = Convert.ToInt32(results.CAFR_ExtApproval.Value)
            End If
        End Sub

        Private Sub LoadFindings()
            Dim workflowType As String = GetWorkflowType(WsId)

            Select Case workflowType
                Case "Ext"
                    LoadExtFindings()
                Case "Appeal"
                    LoadAppealFindings()
            End Select
        End Sub

        Private Sub SaveAppealData()
            SpecCaseDao.CreateINCAPAppealFindings(RefId)
            Dim findings As SC_IncapAppeal_Findings = SpecCase.INCAPAppealFindings.Last
            UpdateAppealFindings(findings)
            SpecCaseDao.SaveOrUpdate(SpecCase)
            SpecCaseDao.CommitChanges()
        End Sub

        Private Sub SaveExtData()
            If (SESSION_WS_ID(RefId) = AFRCWorkflows.INExtension) Then
                SpecCaseDao.CreateINCAPExtFindings(RefId)
            End If
            Dim findings As SC_IncapExt_Findings = SpecCase.INCAPExtFindings.Last
            UpdateExtFindings(findings)
            SpecCaseDao.SaveOrUpdate(SpecCase)
            SpecCaseDao.CommitChanges()
        End Sub

        Private Sub SaveFindings()
            If (Not UserCanEdit) Then
                Exit Sub
            End If
            Dim workflowType As String = GetWorkflowType(WsId)

            Select Case workflowType
                Case "Ext"
                    SaveExtData()
                Case "Appeal"
                    SaveAppealData()
            End Select

            SCDao.SaveOrUpdate(SpecCase)
            SCDao.CommitChanges()
            'End If
        End Sub

        Private Function UpdateAppealFindings(ByVal fnd As SC_IncapAppeal_Findings) As SC_IncapAppeal_Findings
            Select Case WsId
                Case AFRCWorkflows.INOPR_Appeal_HR_Review
                    If (Not rblOPRAppealQuestion.SelectedValue = "" AndAlso rblOPRAppealQuestion.SelectedValue >= 0) Then
                        fnd.OPR_AppealApproval = rblOPRAppealQuestion.SelectedItem.Value
                    End If
                Case AFRCWorkflows.INOCR_Appeal_HR_Review
                    If (Not rblOCRAppealQuestion.SelectedValue = "" AndAlso rblOCRAppealQuestion.SelectedValue >= 0) Then
                        fnd.OCR_AppealApproval = rblOCRAppealQuestion.SelectedItem.Value
                    End If

                Case AFRCWorkflows.INDirectorOfStaffReview_Appeal
                    If (Not rblDOSAppealQuestion.SelectedValue = "" AndAlso rblDOSAppealQuestion.SelectedIndex >= 0) Then
                        fnd.DOS_AppealApproval = rblDOSAppealQuestion.SelectedItem.Value
                    End If

                Case AFRCWorkflows.INCommandChiefReview_Appeal
                    If (Not rblCCRAppealQuestion.SelectedValue = "" AndAlso rblCCRAppealQuestion.SelectedIndex >= 0) Then
                        fnd.CCR_AppealApproval = rblCCRAppealQuestion.SelectedItem.Value
                    End If

                Case AFRCWorkflows.INViceCommanderReview_Appeal
                    If (Not rblVCRAppealQuestion.SelectedValue = "" AndAlso rblVCRAppealQuestion.SelectedIndex >= 0) Then
                        fnd.VCR_AppealApproval = rblVCRAppealQuestion.SelectedItem.Value
                    End If

                Case AFRCWorkflows.INDirectorOfPersonnelReview_Appeal
                    If (Not rblDOPAppealQuestion.SelectedValue = "" AndAlso rblDOPAppealQuestion.SelectedIndex >= 0) Then
                        fnd.DOP_AppealApproval = rblDOPAppealQuestion.SelectedItem.Value
                    End If
                Case AFRCWorkflows.INCAFR_Action_Appeal
                    If (Not rblCAFRAppealQuestion.SelectedValue = "" AndAlso rblCAFRAppealQuestion.SelectedIndex >= 0) Then
                        fnd.CAFR_AppealApproval = rblCAFRAppealQuestion.SelectedItem.Value
                    End If

            End Select
            Return fnd
        End Function

        Private Function UpdateExtFindings(ByVal fnd As SC_IncapExt_Findings) As SC_IncapExt_Findings
            Select Case WsId
                Case AFRCWorkflows.INOPR_Ext_HR_Review
                    If (rblOPRExtQuestion.SelectedValue >= 0) Then
                        fnd.OPR_ExtApproval = rblOPRExtQuestion.SelectedItem.Value
                    End If
                Case AFRCWorkflows.INOCR_Ext_HR_Review
                    If (rblOCRExtQuestion.SelectedValue >= 0) Then
                        fnd.OCR_ExtApproval = rblOCRExtQuestion.SelectedItem.Value
                    End If

                Case AFRCWorkflows.INDirectorOfStaffReview
                    If (rblDOSExtQuestion.SelectedIndex >= 0) Then
                        fnd.DOS_ExtApproval = rblDOSExtQuestion.SelectedItem.Value
                    End If

                Case AFRCWorkflows.INCommandChiefReview
                    If (rblCCRExtQuestion.SelectedIndex >= 0) Then
                        fnd.CCR_ExtApproval = rblCCRExtQuestion.SelectedItem.Value
                    End If

                Case AFRCWorkflows.INViceCommanderReview
                    If (rblVCRExtQuestion.SelectedIndex >= 0) Then
                        fnd.VCR_ExtApproval = rblVCRExtQuestion.SelectedItem.Value
                    End If

                Case AFRCWorkflows.INDirectorOfPersonnelReview
                    If (rblDOPExtQuestion.SelectedIndex >= 0) Then
                        fnd.DOP_ExtApproval = rblDOPExtQuestion.SelectedItem.Value
                    End If
                Case AFRCWorkflows.INCAFRAction
                    If (rblCAFRExtQuestion.SelectedIndex >= 0) Then
                        fnd.CAFR_ExtApproval = rblCAFRExtQuestion.SelectedItem.Value
                    End If

            End Select
            Return fnd
        End Function

#Region "TabEvent"

        Public Function ValidBoxLength() As Boolean

            Dim IsValid As Boolean = True

            'If Not CheckTextLength(SuspenseDate) Then
            '    IsValid = False
            'End If
            'If Not CheckTextLength(TMTRecDate) Then
            '    IsValid = False
            'End If
            'If Not CheckTextLength(CommentsTB) Then
            '    IsValid = False
            'End If

            Return IsValid

        End Function

        Protected Sub Page_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
            AddHandler Me.Master.TabClick, AddressOf TabButtonClicked
        End Sub

        Protected Sub Save_Click(ByRef sender As Object, ByRef e As TabNavigationEventArgs)

            If (Navigator.CurrentStep.IsReadOnly) Then
                Exit Sub
            End If

            If (e.ButtonType = NavigatorButtonType.Save OrElse e.ButtonType = NavigatorButtonType.NavigatedAway _
                OrElse e.ButtonType = NavigatorButtonType.NextStep OrElse e.ButtonType = NavigatorButtonType.PreviousStep) Then
                SaveFindings()
            End If

            If Not (ValidBoxLength()) Then
                e.Cancel = True
                Exit Sub
            End If

        End Sub

        Private Sub TabButtonClicked(ByRef sender As Object, ByRef e As TabNavigationEventArgs)

            If (e.ButtonType = NavigatorButtonType.Save OrElse
                e.ButtonType = NavigatorButtonType.NavigatedAway OrElse
                e.ButtonType = NavigatorButtonType.NextStep OrElse
                e.ButtonType = NavigatorButtonType.PreviousStep) Then
                SaveFindings()
            End If

            If Not (ValidBoxLength()) Then
                e.Cancel = True
                Exit Sub
            End If

        End Sub

#End Region

    End Class

End Namespace