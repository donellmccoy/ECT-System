Imports ALOD.Core.Domain.Common
Imports ALOD.Core.Domain.Lookup
Imports ALOD.Core.Domain.Modules.Lod
Imports ALOD.Core.Interfaces.DAOInterfaces
Imports ALOD.Data
Imports ALOD.Data.Services
Imports ALOD.Web.UserControls
Imports ALODWebUtility.Common
Imports ALODWebUtility.LookUps

Namespace Web.LOD

    Partial Class Secure_lod_c2_Audit
        Inherits System.Web.UI.Page

#Region "fields/properties"

        Private _daoFactory As IDaoFactory
        Private _lineOfDutyAuditDao As ILineOfDutyAuditDao
        Private _lookupDao As ILookupDao
        Private _provider As New LookUp
        'Private _lineOfDutyUnitDao As ILineOfDutyUnitDao

        Public Property IsReadOnly() As Boolean
            Get
                Return ViewState("LodMedical_IsReadonly")
            End Get
            Set(ByVal value As Boolean)
                ViewState("LodMedical_IsReadonly") = value
            End Set

        End Property

        Public ReadOnly Property Navigator() As TabNavigator
            Get
                Return MasterPage.Navigator
            End Get
        End Property

        Public ReadOnly Property refId() As Integer
            Get
                Return Integer.Parse(Request.QueryString("refId"))
            End Get
        End Property

        '                Return _lineOfDutyUnitDao
        '            End Get
        '        End Property
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

        Protected ReadOnly Property DaoFactory() As IDaoFactory
            Get
                If (_daoFactory Is Nothing) Then
                    _daoFactory = New NHibernateDaoFactory()
                End If

                Return _daoFactory
            End Get
        End Property

        '        Protected ReadOnly Property LookupDao() As ILookupDao
        '            Get
        '                If (_lookupDao Is Nothing) Then
        '                    _lookupDao = DaoFactory.GetLookupDao()
        '                End If

        '                Return _lookupDao
        '            End Get
        '        End Property

        Protected ReadOnly Property LineOfDutyAuditDao() As ILineOfDutyAuditDao
            Get
                If (_lineOfDutyAuditDao Is Nothing) Then
                    _lineOfDutyAuditDao = DaoFactory.GetLineOfDutyAuditDao()
                End If

                Return _lineOfDutyAuditDao
            End Get
        End Property

        '        Protected ReadOnly Property LineOfDutyUnitDao() As ILineOfDutyUnitDao
        '            Get
        '                If (_lineOfDutyUnitDao Is Nothing) Then
        '                    _lineOfDutyUnitDao = DaoFactory.GetLineOfDutyUnitDao()
        '                End If
        Protected ReadOnly Property MasterPage() As LodMaster
            Get
                Dim master As LodMaster = CType(Page.Master, LodMaster)
                Return master
            End Get
        End Property

        '        Protected ReadOnly Property CalendarImage() As String
        '            Get
        '                Return Me.ResolveClientUrl("~/App_Themes/" + Page.Theme + "/Images/Calendar.gif")
        '            End Get
        '        End Property

#End Region

        Protected Sub A1_DeterminationRadioList_Radio(sender As Object, e As EventArgs) Handles A1_DeterminationRadioList.SelectedIndexChanged
            If (A1_DeterminationRadioList.SelectedValue.Equals("0")) Then
                A1_DeterminationNotCorrectRadioList.Enabled = True
            ElseIf (A1_DeterminationRadioList.SelectedValue.Equals("1")) Then
                A1_DeterminationNotCorrectRadioList.Enabled = False
                A1_DeterminationNotCorrectRadioList.ClearSelection()
            End If

        End Sub

        Protected Sub A1_IllnessOrDiseaseRadioList_Radio(sender As Object, e As EventArgs) Handles A1_IllnessOrDiseaseRadioList.SelectedIndexChanged
            If (A1_IllnessOrDiseaseRadioList.SelectedValue.Equals("0")) Then
                A1_ActivitesRadioList.Enabled = False
                SetRadioList(A1_ActivitesRadioList, "2")
            ElseIf (A1_IllnessOrDiseaseRadioList.SelectedValue.Equals("1")) Then
                A1_ActivitesRadioList.Enabled = True
                Dim na As ListItem = A1_ActivitesRadioList.Items.FindByText("N/A")
                na.Enabled = False
                A1_ActivitesRadioList.ClearSelection()
            End If

        End Sub

        Protected Sub A1_LegallySelect_Radio(sender As Object, e As EventArgs) Handles A1_ValidatedSelect.SelectedIndexChanged
            If (A1_ValidatedSelect.SelectedValue.Equals("0")) Then
                A1_StatusCheckBox.Enabled = True
                A1_OrdersCheckBox.Enabled = True
                A1_EPTSCheckBox.Enabled = True
                A1_IDTCheckBox.Enabled = True
                A1_PCARSCheckBox.Enabled = True
                A1_8YearRuleCheckBox.Enabled = True
                A1_OtherTextBox.Enabled = True
            Else
                A1_StatusCheckBox.Checked = False
                A1_OrdersCheckBox.Checked = False
                A1_EPTSCheckBox.Checked = False
                A1_IDTCheckBox.Checked = False
                A1_PCARSCheckBox.Checked = False
                A1_8YearRuleCheckBox.Checked = False
                A1_OtherTextBox.Text = ""
                HideAllReasons()
                InitializeUserControls()
            End If

        End Sub

        Protected Function CheckIfAuditInitiated()
            Dim auditInfo As LineOfDutyAuditDao = New LineOfDutyAuditDao
            Dim data As New DataSet
            data = auditInfo.CheckIfAuditInitiated(refId)
            Dim dataTable As DataTable = data.Tables.Item(0)
            Dim initiated As Boolean = dataTable.Rows.Item(0).Item("Column1").ToString()
            Return initiated
        End Function

        Protected Function ConvertCheckBox(value As Object)
            If (IsDBNull(value)) Then
                Return 0
            ElseIf (Not value) Then
                Return 0
            Else
                Return 1
            End If
        End Function

        Protected Function ConvertRadioBox(value As Object)
            'Because values are saved in the DB as bits
            If (IsDBNull(value)) Then
                Return ""

            End If
            If (value.ToString() = "") Then
                Return ""
            End If
            Return value.ToString()
        End Function

        Protected Sub DataMapping(data As DataTable) 'CodeCleanUp create a Mapping to marry the Dao to the database column name
            'A1 Approving Authority/SAF MRR Questions
            SetRadioList(A1_ValidatedSelect, ConvertRadioBox(data.Rows.Item(0).Item("StatusValidated").ToString()))
            SetCheckBox(A1_StatusCheckBox, Integer.Parse(ConvertCheckBox(data.Rows.Item(0).Item("StatusOfMember"))))
            SetCheckBox(A1_OrdersCheckBox, Integer.Parse(ConvertCheckBox(data.Rows.Item(0).Item("Orders"))))
            SetCheckBox(A1_EPTSCheckBox, Integer.Parse(ConvertCheckBox(data.Rows.Item(0).Item("A1_EPTS"))))
            SetCheckBox(A1_IDTCheckBox, Integer.Parse(ConvertCheckBox(data.Rows.Item(0).Item("IDT"))))
            SetCheckBox(A1_PCARSCheckBox, Integer.Parse(ConvertCheckBox(data.Rows.Item(0).Item("PCARS"))))
            SetCheckBox(A1_8YearRuleCheckBox, Integer.Parse(ConvertCheckBox(data.Rows.Item(0).Item("EightYearRule"))))
            SetTextboxText(A1_OtherTextBox, data.Rows.Item(0).Item("A1_Other").ToString())
            SetRadioList(A1_DeterminationRadioList, ConvertRadioBox(data.Rows.Item(0).Item("Determination").ToString()))
            SetRadioList(A1_DeterminationNotCorrectRadioList, ConvertRadioBox(data.Rows.Item(0).Item("A1_DeterminationNotCorrect").ToString()))
            SetRadioList(A1_LODInitRadioList, ConvertRadioBox(data.Rows.Item(0).Item("LODInitiation").ToString()))
            SetRadioList(A1_WrittenDiagnosisRadioList, ConvertRadioBox(data.Rows.Item(0).Item("WrittenDiagnosis").ToString()))
            SetRadioList(A1_MemberRequestRadioList, ConvertRadioBox(data.Rows.Item(0).Item("MemberRequest").ToString()))
            SetRadioList(A1_IncurredOrAggravatedRadioList, ConvertRadioBox(data.Rows.Item(0).Item("IncurredOrAggravated").ToString()))
            SetRadioList(A1_IllnessOrDiseaseRadioList, ConvertRadioBox(data.Rows.Item(0).Item("IllnessOrDisease").ToString()))
            SetRadioList(A1_ActivitesRadioList, ConvertRadioBox(data.Rows.Item(0).Item("Activites").ToString()))
            SetTextboxText(A1_CommentTextBox, data.Rows.Item(0).Item("A1_Comment").ToString())
            'JA Board Legal
            SetRadioList(JA_LegallySelect, ConvertRadioBox(data.Rows.Item(0).Item("LegallySufficient").ToString()))
            SetCheckBox(JA_StandardOfProofCheckBox, Integer.Parse(ConvertCheckBox(data.Rows.Item(0).Item("JA_StandardOfProof"))))
            SetCheckBox(JA_DeathAndMVACheckBox, Integer.Parse(ConvertCheckBox(data.Rows.Item(0).Item("JA_DeathAndMVA"))))
            SetCheckBox(JA_FormalPolicyCheckBox, Integer.Parse(ConvertCheckBox(data.Rows.Item(0).Item("JA_FormalPolicy"))))
            SetCheckBox(JA_AFICheckBox, Integer.Parse(ConvertCheckBox(data.Rows.Item(0).Item("JA_AFI"))))
            SetTextboxText(JA_OtherTextBox, data.Rows.Item(0).Item("JA_Other").ToString())
            SetRadioList(JA_ProofRadioList, ConvertRadioBox(data.Rows.Item(0).Item("JA_ProofApplied").ToString()))
            SetRadioList(JA_StandardOfProofRadioList, ConvertRadioBox(data.Rows.Item(0).Item("JA_CorrectStandard").ToString()))
            SetRadioList(JA_ProofMetRadioList, ConvertRadioBox(data.Rows.Item(0).Item("JA_ProofMet").ToString()))
            SetRadioList(JA_EvidenceConditionRadioList, ConvertRadioBox(data.Rows.Item(0).Item("JA_Evidence").ToString()))
            SetRadioList(JA_MisconductRadioList, ConvertRadioBox(data.Rows.Item(0).Item("JA_Misconduct").ToString()))
            SetRadioList(JA_FormalInvestigationRadioList, ConvertRadioBox(data.Rows.Item(0).Item("JA_FormalInvestigation").ToString()))
            SetTextboxText(JA_CommentTextBox, data.Rows.Item(0).Item("JA_Comment").ToString())
            'SG Board Medical
            SetRadioList(SG_AppropriateStatusSelect, ConvertRadioBox(data.Rows.Item(0).Item("MedicallyAppropriate")))
            SetCheckBox(SG_DXCheckBox, Integer.Parse(ConvertCheckBox(data.Rows.Item(0).Item("SG_DX"))))
            SetCheckBox(SG_ISupportCheckBox, Integer.Parse(ConvertCheckBox(data.Rows.Item(0).Item("SG_ISupport"))))
            SetCheckBox(SG_EPTSCheckBox, Integer.Parse(ConvertCheckBox(data.Rows.Item(0).Item("SG_EPTS"))))
            SetCheckBox(SG_AggravationCheckBox, Integer.Parse(ConvertCheckBox(data.Rows.Item(0).Item("SG_Aggravation"))))
            SetCheckBox(SG_MedicalPrincipleCheckBox, Integer.Parse(ConvertCheckBox(data.Rows.Item(0).Item("SG_Principles"))))
            SetTextboxText(SG_OtherTextBox, data.Rows.Item(0).Item("SG_Other").ToString())
            SetRadioList(SG_ProofRadioList, ConvertRadioBox(data.Rows.Item(0).Item("SG_ProofApplied").ToString()))
            SetRadioList(SG_StandardOfProofRadioList, ConvertRadioBox(data.Rows.Item(0).Item("SG_CorrectStandard").ToString()))
            SetRadioList(SG_ProofMetRadioList, ConvertRadioBox(data.Rows.Item(0).Item("SG_ProofMet").ToString()))
            SetRadioList(SG_EvidenceConditionRadioList, ConvertRadioBox(data.Rows.Item(0).Item("SG_Evidence").ToString()))
            SetRadioList(SG_MisconductRadioList, ConvertRadioBox(data.Rows.Item(0).Item("SG_Misconduct").ToString()))
            SetRadioList(SG_FormalInvestigationRadioList, ConvertRadioBox(data.Rows.Item(0).Item("SG_FormalInvestigation").ToString()))
            SetTextboxText(SG_CommentTextBox, data.Rows.Item(0).Item("SG_Comment").ToString())
            'CodeCleanUp call one method to do the following and also call it in InitializeUserControls()
            If (SESSION_WS_ID(refId) = 333 And Not IsDBNull(data.Rows.Item(0).Item("MedicallyAppropriate"))) Then
                If (Not data.Rows.Item(0).Item("MedicallyAppropriate")) Then
                    SG_DXCheckBox.Enabled = True
                    SG_ISupportCheckBox.Enabled = True
                    SG_EPTSCheckBox.Enabled = True
                    SG_AggravationCheckBox.Enabled = True
                    SG_MedicalPrincipleCheckBox.Enabled = True
                    SG_OtherTextBox.Enabled = True
                    SG_Section.Enabled = True
                End If
            End If
            If (SESSION_WS_ID(refId) = 334 And Not IsDBNull(data.Rows.Item(0).Item("LegallySufficient"))) Then

                If (Not data.Rows.Item(0).Item("LegallySufficient")) Then
                    JA_StandardOfProofCheckBox.Enabled = True
                    JA_DeathAndMVACheckBox.Enabled = True
                    JA_FormalPolicyCheckBox.Enabled = True
                    JA_AFICheckBox.Enabled = True
                    JA_OtherTextBox.Enabled = True
                    JA_Section.Enabled = True
                End If
            End If
            If (SESSION_WS_ID(refId) = 335 And Not IsDBNull(data.Rows.Item(0).Item("StatusValidated"))) Then
                If (Not data.Rows.Item(0).Item("StatusValidated")) Then
                    A1_StatusCheckBox.Enabled = True
                    A1_OrdersCheckBox.Enabled = True
                    A1_EPTSCheckBox.Enabled = True
                    A1_IDTCheckBox.Enabled = True
                    A1_PCARSCheckBox.Enabled = True
                    A1_8YearRuleCheckBox.Enabled = True
                    A1_OtherTextBox.Enabled = True
                    A1_Section.Enabled = True
                    A1_DeterminationRadioList.Enabled = True
                End If
                If (Not IsDBNull(data.Rows.Item(0).Item("Determination"))) Then
                    If (data.Rows.Item(0).Item("Determination") = 0) Then
                        A1_DeterminationNotCorrectRadioList.Enabled = True
                    End If
                End If
            End If

        End Sub

        Protected Sub JA_LegallySelect_Radio(sender As Object, e As EventArgs) Handles JA_LegallySelect.SelectedIndexChanged
            If (JA_LegallySelect.SelectedValue.Equals("0")) Then
                JA_StandardOfProofCheckBox.Enabled = True
                JA_DeathAndMVACheckBox.Enabled = True
                JA_FormalPolicyCheckBox.Enabled = True
                JA_AFICheckBox.Enabled = True
                JA_OtherTextBox.Enabled = True
            Else
                JA_StandardOfProofCheckBox.Checked = False
                JA_DeathAndMVACheckBox.Checked = False
                JA_FormalPolicyCheckBox.Checked = False
                JA_AFICheckBox.Checked = False
                JA_OtherTextBox.Text = ""
                HideAllReasons()
                InitializeUserControls()
            End If

        End Sub

        Protected Sub LoadData()
            Dim auditInfo As LineOfDutyAuditDao = New LineOfDutyAuditDao
            Dim data As New DataSet
            data = auditInfo.GetAuditInfo(refId)
            Dim dataTable As DataTable = data.Tables.Item(0) 'x.DataSet.Tables.SyncRoot
            Dim initiated As Boolean = CheckIfAuditInitiated()
            If (initiated) Then
                DataMapping(dataTable)
            End If

        End Sub

        Protected Sub Page_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
            AddHandler Me.Master.TabClick, AddressOf TabButtonClicked
        End Sub

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

            If (Not Page.IsPostBack) Then
                Dim lod As LineOfDuty = LodService.GetById(refId)
                InitControls()
            End If
            If (JA_ProofMetRadioList.SelectedIndex = 2) Then
                JA_UnableToDetermineText.Visible = True
                JA_DetermineTextBox.Visible = True
            Else
                JA_UnableToDetermineText.Visible = False
                JA_DetermineTextBox.Visible = False
            End If
        End Sub

        Protected Sub Save_Click(ByRef sender As Object, ByRef e As TabNavigationEventArgs)
            If (Navigator.CurrentStep.IsReadOnly) Then
                Exit Sub
            End If

            If Not (ValidBoxLength()) Then
                e.Cancel = True
                Exit Sub
            End If

            If (e.ButtonType = NavigatorButtonType.Save OrElse
                e.ButtonType = NavigatorButtonType.NavigatedAway OrElse
                e.ButtonType = NavigatorButtonType.NextStep OrElse
                e.ButtonType = NavigatorButtonType.PreviousStep) Then

                SaveData()
            End If
        End Sub

        Protected Sub SG_AppropriateStatusSelect_Radio(sender As Object, e As EventArgs) Handles SG_AppropriateStatusSelect.SelectedIndexChanged
            If (SG_AppropriateStatusSelect.SelectedValue.Equals("0")) Then
                SG_DXCheckBox.Enabled = True
                SG_ISupportCheckBox.Enabled = True
                SG_EPTSCheckBox.Enabled = True
                SG_AggravationCheckBox.Enabled = True
                SG_MedicalPrincipleCheckBox.Enabled = True
                SG_OtherTextBox.Enabled = True
            Else
                SG_DXCheckBox.Checked = False
                SG_ISupportCheckBox.Checked = False
                SG_EPTSCheckBox.Checked = False
                SG_AggravationCheckBox.Checked = False
                SG_MedicalPrincipleCheckBox.Checked = False
                SG_OtherTextBox.Text = ""
                HideAllReasons()
                InitializeUserControls()
            End If

        End Sub

        Private Sub HideAllReasons()
            Dim na As ListItem = A1_ActivitesRadioList.Items.FindByText("N/A")
            na.Enabled = False
            A1_StatusCheckBox.Enabled = False
            A1_OrdersCheckBox.Enabled = False
            A1_EPTSCheckBox.Enabled = False
            A1_IDTCheckBox.Enabled = False
            A1_PCARSCheckBox.Enabled = False
            A1_8YearRuleCheckBox.Enabled = False
            SG_DXCheckBox.Enabled = False
            SG_ISupportCheckBox.Enabled = False
            SG_EPTSCheckBox.Enabled = False
            SG_AggravationCheckBox.Enabled = False
            SG_MedicalPrincipleCheckBox.Enabled = False
            SG_OtherTextBox.Enabled = False
            JA_StandardOfProofCheckBox.Enabled = False
            JA_DeathAndMVACheckBox.Enabled = False
            JA_FormalPolicyCheckBox.Enabled = False
            JA_AFICheckBox.Enabled = False
            JA_OtherTextBox.Enabled = False
            A1_OtherTextBox.Enabled = False
            SG_Section.Enabled = False
            A1_Section.Enabled = False
            JA_Section.Enabled = False
            SG_Section.Enabled = False
            A1_DeterminationNotCorrectRadioList.Enabled = False
            If (SG_AppropriateStatusSelect.SelectedValue.Equals("0")) Then
                SG_DXCheckBox.Enabled = True
                SG_ISupportCheckBox.Enabled = True
                SG_EPTSCheckBox.Enabled = True
                SG_AggravationCheckBox.Enabled = True
                SG_MedicalPrincipleCheckBox.Enabled = True
                SG_OtherTextBox.Enabled = True
            End If
            If (JA_LegallySelect.SelectedValue.Equals("0")) Then
                JA_StandardOfProofCheckBox.Enabled = True
                JA_DeathAndMVACheckBox.Enabled = True
                JA_FormalPolicyCheckBox.Enabled = True
                JA_AFICheckBox.Enabled = True
                JA_OtherTextBox.Enabled = True
            End If
            If (A1_ValidatedSelect.SelectedValue.Equals("0")) Then
                A1_StatusCheckBox.Enabled = True
                A1_OrdersCheckBox.Enabled = True
                A1_EPTSCheckBox.Enabled = True
                A1_IDTCheckBox.Enabled = True
                A1_PCARSCheckBox.Enabled = True
                A1_8YearRuleCheckBox.Enabled = True
                A1_OtherTextBox.Enabled = True
            End If
            If (A1_DeterminationRadioList.SelectedValue.Equals("0")) Then
                A1_DeterminationNotCorrectRadioList.Enabled = True
            End If

        End Sub

        '#Region "LOD"
        Private Sub InitControls()
            'Controls the flow of the questions "if NO then this"
            HideAllReasons()                'Hides options
            InitializeUserControls()        'Displays different Sections
            LoadData()                  'Displays saved Data
            TabControl.Item(NavigatorButtonType.Save).Visible = True ' UserCanEdit

            SetMaxLength(SG_CommentTextBox, False)
            SetMaxLength(SG_OtherTextBox)
            SetMaxLength(JA_OtherTextBox)
            SetMaxLength(JA_CommentTextBox, False)
            SetMaxLength(A1_OtherTextBox)
            SetMaxLength(A1_CommentTextBox)
        End Sub

        Private Sub InitializeUserControls()
            'Dim refId As Integer = SESSION_WS_ID
            Dim lod As LineOfDuty = LodService.GetById(refId)
            Dim wsId As Integer = SESSION_WS_ID(lod.Id)
            HideAllReasons()
            'CodeCleanUp call one method to do the following and also call it in DataMapping()
            Select Case wsId
                Case 335 'LOD Audit (A1)
                    A1_Section.Enabled = True
                Case 334 'LOD Audit (JA)
                    JA_Section.Enabled = True
                Case 333 'LOD Audit (SG)
                    SG_Section.Enabled = True
            End Select
        End Sub

        Private Sub SaveData()
            Dim auditInfo As LineOfDutyAuditDao = New LineOfDutyAuditDao
            Dim wsId As Integer = SESSION_WS_ID(refId)
            Dim a1Determination As Integer
            If (A1_DeterminationNotCorrectRadioList.SelectedIndex >= 0) Then
                a1Determination = Integer.Parse(A1_DeterminationNotCorrectRadioList.SelectedValue)
            End If

            Select Case wsId
                Case 335  'LOD Audit (A1) Approving Authority/SAF MRR Questions
                    auditInfo.SaveAuditA1(refId, A1_ValidatedSelect.SelectedIndex, A1_StatusCheckBox.Checked, A1_OrdersCheckBox.Checked, A1_EPTSCheckBox.Checked, A1_IDTCheckBox.Checked,
                                        A1_PCARSCheckBox.Checked, A1_8YearRuleCheckBox.Checked, A1_OtherTextBox.Text, A1_LODInitRadioList.SelectedIndex, A1_WrittenDiagnosisRadioList.SelectedIndex,
                                        A1_MemberRequestRadioList.SelectedIndex, A1_IncurredOrAggravatedRadioList.SelectedIndex, A1_IllnessOrDiseaseRadioList.SelectedIndex, A1_ActivitesRadioList.SelectedIndex,
                                        A1_DeterminationRadioList.SelectedIndex, a1Determination, A1_CommentTextBox.Text)
                Case 334 'LOD Audit (JA) Board Legal
                    auditInfo.SaveAuditJA(refId, JA_LegallySelect.SelectedIndex, JA_StandardOfProofCheckBox.Checked, JA_DeathAndMVACheckBox.Checked, JA_FormalPolicyCheckBox.Checked, JA_AFICheckBox.Checked,
                                        JA_OtherTextBox.Text, JA_ProofRadioList.SelectedIndex, JA_StandardOfProofRadioList.SelectedIndex,
                                        JA_ProofMetRadioList.SelectedIndex, JA_EvidenceConditionRadioList.SelectedIndex, JA_MisconductRadioList.SelectedIndex, JA_FormalInvestigationRadioList.SelectedIndex,
                                        JA_CommentTextBox.Text)
                Case 333 'LOD Audit (SG) Board Medical

                    auditInfo.SaveAuditSG(refId, SG_AppropriateStatusSelect.SelectedIndex, SG_DXCheckBox.Checked, SG_ISupportCheckBox.Checked, SG_EPTSCheckBox.Checked, SG_AggravationCheckBox.Checked,
                                        SG_MedicalPrincipleCheckBox.Checked, SG_OtherTextBox.Text, SG_ProofRadioList.SelectedIndex, SG_StandardOfProofRadioList.SelectedIndex,
                                        SG_ProofMetRadioList.SelectedIndex, SG_EvidenceConditionRadioList.SelectedIndex, SG_MisconductRadioList.SelectedIndex, SG_FormalInvestigationRadioList.SelectedIndex,
                                        SG_CommentTextBox.Text)
            End Select
        End Sub

        Private Sub TabButtonClicked(ByRef sender As Object, ByRef e As TabNavigationEventArgs)
            If (Navigator.CurrentStep.IsReadOnly) Then
                Exit Sub
            End If

            If Not (ValidBoxLength()) Then
                e.Cancel = True
                Exit Sub
            End If

            '            Dim lod As LineOfDuty = LodService.GetById(refId)

            If (e.ButtonType = NavigatorButtonType.Save OrElse
                e.ButtonType = NavigatorButtonType.NavigatedAway OrElse
                e.ButtonType = NavigatorButtonType.NextStep OrElse
                e.ButtonType = NavigatorButtonType.PreviousStep) Then
                SaveData()
            End If
        End Sub

#Region "TabEvent"

        Public Function ValidBoxLength() As Boolean
            Dim IsValid As Boolean = True

            If Not CheckTextLength(JA_CommentTextBox) Then
                IsValid = False
            End If

            If Not CheckTextLength(SG_CommentTextBox) Then
                IsValid = False
            End If
            If Not CheckTextLength(A1_CommentTextBox) Then
                IsValid = False
            End If

            If Not CheckTextLength(A1_OtherTextBox) Then
                IsValid = False
            End If
            If Not CheckTextLength(SG_OtherTextBox) Then
                IsValid = False
            End If
            If Not CheckTextLength(JA_OtherTextBox) Then
                IsValid = False
            End If

            Return IsValid
        End Function

#End Region

    End Class

End Namespace