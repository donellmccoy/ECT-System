Imports ALOD.Core.Domain.Common
Imports ALOD.Core.Domain.Lookup
Imports ALOD.Core.Domain.Modules.Lod
Imports ALOD.Core.Domain.Modules.SpecialCases
Imports ALOD.Core.Domain.Users
Imports ALOD.Core.Domain.Workflow
Imports ALOD.Core.Interfaces.DAOInterfaces
Imports ALOD.Data
Imports ALOD.Logging
Imports ALOD.Web.UserControls
Imports ALODWebUtility.Common

Namespace Web.Special_Case.PSCD

    Partial Class Secure_PSCD_PSCDBoard
        Inherits System.Web.UI.Page

        Private _dao As ISpecialCaseDAO
        Private _lookupDao As ILookupDao
        Private _pageAccessDictionary As Dictionary(Of String, ALOD.Core.Domain.Workflow.PageAccessType)
        Private sc As SC_PSCD

        ReadOnly Property SCDao() As ISpecialCaseDAO
            Get
                If (_dao Is Nothing) Then
                    _dao = New NHibernateDaoFactory().GetSpecialCaseDAO()
                End If

                Return _dao
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

        Protected ReadOnly Property LookupDao() As ILookupDao
            Get
                If (_lookupDao Is Nothing) Then
                    _lookupDao = New NHibernateDaoFactory().GetLookupDao()
                End If

                Return _lookupDao
            End Get
        End Property

        Protected ReadOnly Property ModuleType() As ModuleType
            Get
                Return ModuleType.SpecCasePSCD
            End Get
        End Property

        Protected ReadOnly Property Navigator() As TabNavigator
            Get
                Return Me.Master.Navigator
            End Get
        End Property

        Protected ReadOnly Property refId() As Integer
            Get
                Return Integer.Parse(Request.QueryString("refId"))
            End Get
        End Property

        Protected ReadOnly Property SectionList() As Dictionary(Of String, ALOD.Core.Domain.Workflow.PageAccessType)
            Get
                ' If (_pageAccessDictionary Is Nothing) Then
                _pageAccessDictionary = SpecCase.ReadSectionList(CInt(Session("GroupId")))
                'End If

                Return _pageAccessDictionary
            End Get
        End Property

        Protected ReadOnly Property SpecCase() As SC_PSCD
            Get
                If (sc Is Nothing) Then
                    sc = SCDao.GetById(refId)
                End If

                Return sc
            End Get
        End Property

        Protected ReadOnly Property TabControl() As TabControls
            Get
                Return Master.TabControl
            End Get
        End Property

        Protected Function HasReadWriteAccessForSection(ByVal sectionName As PSCDSectionNames) As Boolean
            If (SectionList(sectionName.ToString()) <> ALOD.Core.Domain.Workflow.PageAccessType.ReadWrite) Then
                Return False
            End If

            Return True
        End Function

        Protected Sub LoadFindingsControl(ByVal userControl As Secure_Shared_UserControls_FindingsControl, ByVal pType As PersonnelTypes, ByVal groupId As Integer)
            Dim cFinding As SC_PSCD_Findings

            cFinding = SpecCase.FindByType(pType)
            userControl.LoadFindingOptionsByWorkflow(AFRCWorkflows.SpecCasePSCD, groupId)

            If (cFinding Is Nothing) Then
                Exit Sub
            End If

            If (cFinding.Finding.HasValue) Then
                userControl.Findings = cFinding.Finding.Value
            End If

            If (cFinding.ReferToDES) Then
                If (pType = PersonnelTypes.BOARD_SR) Then 'Board Admin
                    BPR_DESCheckBox.Checked = True
                ElseIf (pType = PersonnelTypes.BOARD) Then 'Approving Authority
                    AAR_DESCheckBox.Checked = True
                End If
            End If

            userControl.Remarks = cFinding.Remarks

            userControl.AdditionalRemarks = cFinding.AdditionalRemarks

            If (userControl.ShowFormText) Then
                userControl.FormFindingsText = cFinding.FindingText
            End If
        End Sub

        Protected Sub Page_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
            AddHandler Me.Master.TabClick, AddressOf TabButtonClicked
        End Sub

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
            AAR_DESCheckBox.Enabled = False
            BPR_DESCheckBox.Enabled = False
            If (Not IsPostBack) Then
                UserCanEdit = GetAccess(Navigator.PageAccess, True)
                InitControls()
                LoadData()

                LogManager.LogAction(ModuleType.SpecCasePSCD, UserAction.ViewPage, refId, "View Page: PSCD Board")
            End If

        End Sub

        Protected Sub SaveFinding(ByVal userControl As Secure_Shared_UserControls_FindingsControl, ByVal pType As PersonnelTypes, ByVal sectionName As PSCDSectionNames)
            If (Not HasReadWriteAccessForSection(sectionName)) Then
                Exit Sub
            End If

            Dim cFinding As SC_PSCD_Findings
            cFinding = PSCDUtility.CreatePSCDFinding(SpecCase.Id)
            cFinding.PType = pType

            If (userControl.Findings IsNot Nothing AndAlso userControl.Findings > 0) Then
                cFinding.Finding = userControl.Findings
            End If

            If (userControl.ShowFormText) Then
                cFinding.FindingText = userControl.FormFindingsText
            End If

            If (userControl.ShowRemarks) Then
                cFinding.Remarks = userControl.Remarks
            End If

            If (userControl.ShowAdditionalRemarks) Then
                cFinding.AdditionalRemarks = userControl.AdditionalRemarks
            End If
            If (sectionName = PSCDSectionNames.PSCD_APPROVINGAUTHORITY) Then
                If (AAR_DESCheckBox.Checked) Then
                    cFinding.ReferToDES = True
                Else
                    cFinding.ReferToDES = False
                End If
            End If
            If (sectionName = PSCDSectionNames.PSCD_BOARDADMIN) Then
                If (BPR_DESCheckBox.Checked) Then
                    cFinding.ReferToDES = True
                Else
                    cFinding.ReferToDES = False
                End If
            End If

            SpecCase.SetFindingByType(cFinding)

            SCDao.SaveOrUpdate(SpecCase)

        End Sub

        Protected Sub SetFindingsControlAccess(ByVal userControl As Secure_Shared_UserControls_FindingsControl, ByVal access As ALOD.Core.Domain.Workflow.PageAccessType)

            If (access <> ALOD.Core.Domain.Workflow.PageAccessType.None) Then
                If (access = ALOD.Core.Domain.Workflow.PageAccessType.ReadWrite) Then
                    userControl.SetReadOnly = False
                End If
            End If
            If (userControl.ID.ToString() = "ucApprovingAuthorityFindings" AndAlso Not userControl.SetReadOnly) Then
                AAR_DESCheckBox.Enabled = True
            End If
            If (userControl.ID.ToString() = "ucPersonnelFindings" AndAlso Not userControl.SetReadOnly) Then
                BPR_DESCheckBox.Enabled = True
            End If
        End Sub

        Private Sub InitControls()
            SetFindingsControlAccess(ucHQMedTechFindings, SectionList(PSCDSectionNames.PSCD_HQMED.ToString()))
            SetFindingsControlAccess(ucBoardMedicalFindings, SectionList(PSCDSectionNames.PSCD_BOARDMED.ToString()))
            SetFindingsControlAccess(ucSeniorMedicalFindings, SectionList(PSCDSectionNames.PSCD_SENIORMED.ToString()))
            SetFindingsControlAccess(ucBoardLegalFindings, SectionList(PSCDSectionNames.PSCD_BOARDLEGAL.ToString()))
            SetFindingsControlAccess(ucPersonnelFindings, SectionList(PSCDSectionNames.PSCD_BOARDADMIN.ToString()))
            SetFindingsControlAccess(ucApprovingAuthorityFindings, SectionList(PSCDSectionNames.PSCD_APPROVINGAUTHORITY.ToString()))
        End Sub

        Private Sub LoadData()

            LoadFindingsControl(ucHQMedTechFindings, PersonnelTypes.MED_TECH, UserGroups.AFRCHQTechnician)
            LoadFindingsControl(ucBoardMedicalFindings, PersonnelTypes.BOARD_SG, UserGroups.BoardMedical)
            LoadFindingsControl(ucSeniorMedicalFindings, PersonnelTypes.SENIOR_MEDICAL_REVIEWER, UserGroups.SeniorMedicalReviewer)
            LoadFindingsControl(ucBoardLegalFindings, PersonnelTypes.BOARD_JA, UserGroups.BoardLegal)
            'Board Personal
            LoadFindingsControl(ucPersonnelFindings, PersonnelTypes.BOARD_SR, UserGroups.BoardAdministrator)
            LoadFindingsControl(ucApprovingAuthorityFindings, PersonnelTypes.BOARD, UserGroups.BoardApprovalAuthority)
        End Sub

        Private Sub Save_Click(ByRef sender As Object, ByRef e As TabNavigationEventArgs)
            If (Navigator.CurrentStep.IsReadOnly) Then
                Exit Sub
            End If

            If (e.ButtonType = NavigatorButtonType.Save OrElse
                e.ButtonType = NavigatorButtonType.NavigatedAway OrElse
                e.ButtonType = NavigatorButtonType.NextStep OrElse
                e.ButtonType = NavigatorButtonType.PreviousStep) Then

                SaveData()
            End If
        End Sub

        Private Sub SaveData()
            If (Not UserCanEdit) Then
                Exit Sub
            End If

            'Board Tech
            SaveFinding(ucHQMedTechFindings, PersonnelTypes.MED_TECH, PSCDSectionNames.PSCD_HQMED)
            'Board Med
            SaveFinding(ucBoardMedicalFindings, PersonnelTypes.BOARD_SG, PSCDSectionNames.PSCD_BOARDMED)
            'Sr Med Rev
            SaveFinding(ucSeniorMedicalFindings, PersonnelTypes.SENIOR_MEDICAL_REVIEWER, PSCDSectionNames.PSCD_SENIORMED)
            'Board Legal
            SaveFinding(ucBoardLegalFindings, PersonnelTypes.BOARD_JA, PSCDSectionNames.PSCD_BOARDLEGAL)
            'Board Admin
            SaveFinding(ucPersonnelFindings, PersonnelTypes.BOARD_SR, PSCDSectionNames.PSCD_BOARDADMIN)
            'Approving Authority
            SaveFinding(ucApprovingAuthorityFindings, PersonnelTypes.BOARD, PSCDSectionNames.PSCD_APPROVINGAUTHORITY)
        End Sub

        Private Sub TabButtonClicked(ByRef sender As Object, ByRef e As TabNavigationEventArgs)
            If (Navigator.CurrentStep.IsReadOnly) Then
                Exit Sub
            End If

            If (e.ButtonType = NavigatorButtonType.Save OrElse
                e.ButtonType = NavigatorButtonType.NavigatedAway OrElse
                e.ButtonType = NavigatorButtonType.NextStep OrElse
                e.ButtonType = NavigatorButtonType.PreviousStep) Then

                SaveData()
            End If
        End Sub

    End Class

End Namespace