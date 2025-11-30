Imports ALOD.Data
Imports ALOD.Core.Domain.Users
Imports ALOD.Core.Domain.Lookup
Imports ALOD.Data.Services
Imports ALOD.Core.Domain.Workflow
Imports ALOD.Core.Domain.Modules.Lod
Imports ALOD.Core.Domain.Common
Imports ALOD.Core.Utils
Imports ALOD.Logging
Imports ALOD.Web.UserControls
Imports ALODWebUtility.Common
Imports ALOD.Core.Interfaces.DAOInterfaces
Imports ALOD.Core.Domain.ServiceMembers

Namespace Web.LOD
    Partial Class Secure_lod_WingCC
        Inherits System.Web.UI.Page

        Private _scAccess As Dictionary(Of String, ALOD.Core.Domain.Workflow.PageAccessType)
        Private _daoFactory As IDaoFactory
        Private _lodDao As ILineOfDutyDao
        Private _lod As LineOfDuty
        Protected _v2 As LineOfDuty_v2
        Private _lookupDao As ILookupDao
        Private _userDao As IUserDao
        Private _userGroupDao As IUserGroupDao
        Private _unitDao As IUnitDao
        Protected Const optionalFindings As String = InvestigationDecision.APPROVE + "," + InvestigationDecision.DISAPPROVE
        Protected Const optionalInformalFindings As String = InvestigationDecision.APPROVE + "," + InvestigationDecision.DISAPPROVE + "," + InvestigationDecision.NOT_LOD_MISCONDUCT

#Region "Properties"
        Protected ReadOnly Property DaoFactory As IDaoFactory
            Get
                If (_daoFactory Is Nothing) Then
                    _daoFactory = New NHibernateDaoFactory()
                End If

                Return _daoFactory
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

        Protected ReadOnly Property lod_v2() As LineOfDuty_v2
            Get
                If (_v2 Is Nothing) Then
                    '_lod_v2 = LodService.GetById(CType(Request.QueryString("refId"), Integer))
                    _v2 = CType(LOD, LineOfDuty_v2)
                End If
                Return _v2
            End Get
        End Property

        Protected ReadOnly Property CalendarImage() As String
            Get
                Return Me.ResolveClientUrl("~/App_Themes/" + Page.Theme + "/Images/Calendar.gif")
            End Get
        End Property

        Private ReadOnly Property LodDao() As ILineOfDutyDao
            Get
                If (_lodDao Is Nothing) Then
                    _lodDao = DaoFactory.GetLineOfDutyDao()
                End If
                Return _lodDao
            End Get
        End Property

        Protected ReadOnly Property LOD() As LineOfDuty
            Get
                If (_lod Is Nothing) Then
                    _lod = LodDao.GetById(RefId)
                End If
                Return _lod
            End Get
        End Property

        Protected ReadOnly Property UserDao As IUserDao
            Get
                If (_userDao Is Nothing) Then
                    _userDao = DaoFactory.GetUserDao()
                End If

                Return _userDao
            End Get
        End Property

        Protected ReadOnly Property UserGroupDao As IUserGroupDao
            Get
                If (_userGroupDao Is Nothing) Then
                    _userGroupDao = DaoFactory.GetUserGroupDao()
                End If

                Return _userGroupDao
            End Get
        End Property

        Protected ReadOnly Property UnitDao As IUnitDao
            Get
                If (_unitDao Is Nothing) Then
                    _unitDao = DaoFactory.GetUnitDao()
                End If

                Return _unitDao
            End Get
        End Property

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

        Protected ReadOnly Property MasterPage() As LodMaster
            Get
                Dim master As LodMaster = CType(Page.Master, LodMaster)
                Return master
            End Get
        End Property

        Private ReadOnly Property RefId() As Integer
            Get
                Return CInt(Request.QueryString("refId"))
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

        Protected ReadOnly Property SectionList() As Dictionary(Of String, ALOD.Core.Domain.Workflow.PageAccessType)
            Get
                If (_scAccess Is Nothing) Then
                    _scAccess = LOD.ReadSectionList(CInt(Session("GroupId")))
                End If
                Return _scAccess
            End Get
        End Property

        Protected Sub Page_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
            AddHandler Me.Master.TabClick, AddressOf TabButtonClicked
        End Sub

        Protected Sub Save_Click(ByRef sender As Object, ByRef e As TabNavigationEventArgs)

            If (Navigator.CurrentStep.IsReadOnly) Then
                Exit Sub
            End If

            If (e.ButtonType = NavigatorButtonType.Save OrElse e.ButtonType = NavigatorButtonType.NavigatedAway _
                OrElse e.ButtonType = NavigatorButtonType.NextStep OrElse e.ButtonType = NavigatorButtonType.PreviousStep) Then

                If (LOD.Workflow = 1) Then
                    SaveFindings()
                Else
                    SaveFindings_v2()
                End If
            End If

        End Sub

        Private Sub TabButtonClicked(ByRef sender As Object, ByRef e As TabNavigationEventArgs)

            If (e.ButtonType = NavigatorButtonType.Save OrElse
                e.ButtonType = NavigatorButtonType.NavigatedAway OrElse
                e.ButtonType = NavigatorButtonType.NextStep OrElse
                e.ButtonType = NavigatorButtonType.PreviousStep) Then

                If (LOD.Workflow = 1) Then
                    SaveFindings()
                Else
                    SaveFindings_v2()
                End If
            End If

        End Sub
#End Region

#Region "Page Methods"
        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
            AddHandler ucInformalAndReinvestigationFindings.DecisionSelected, AddressOf DecisionSelected
            AddHandler ucInformalAndReinvestigationFindings.FindingSelected, AddressOf FindingSelected
            AddHandler ucInformalAndReinvestigationFindings_v2.DecisionSelected, AddressOf DecisionSelected_v2
            AddHandler ucInformalAndReinvestigationFindings_v2.FindingSelected, AddressOf FindingSelected_v2

            If (Not IsPostBack) Then

                UserCanEdit = GetAccessLOD(Navigator.PageAccess, True, LOD)

                SetInputFormatRestriction(Page, txtAAPOC, FormatRestriction.AlphaNumeric, PERMITTED_SPECIAL_CHAR_INPUT)
                SetInputFormatRestriction(Page, txtIOInstruction, FormatRestriction.AlphaNumeric, PERMITTED_SPECIAL_CHAR_INPUT)
                SetInputFormatRestriction(Page, txtIOCompletionDate, FormatRestriction.Numeric, "/")
                SetInputFormatRestriction(Page, txtAppointedIOEDIPI, FormatRestriction.AlphaNumeric)
                SetInputFormatRestriction(Page, txtAppointedIOEmail, FormatRestriction.AlphaNumeric, PERMITTED_SPECIAL_CHAR_INPUT)

                btnChangeUnit.Attributes.Add("onclick", "showSearcher('" + "Select New Unit" + "','" + lblNewUnitID.ClientID + "','" + txtAppointedIOUnit.ClientID + "'); return false;")

                If (LOD.Formal = True AndAlso ucInformalAndReinvestigationFindings.Decision.Equals("Y")) Then
                    ucInformalAndReinvestigationFindings.FindingsIndex = -1
                    ucInformalAndReinvestigationFindings.EnableFindings = False
                End If

                InitControls()

                LogManager.LogAction(ModuleType.LOD, UserAction.ViewPage, RefId, "Viewed Page: WingCC")
            End If
        End Sub
        
        Private Sub InitControls()

            If (LOD.Workflow = 1) Then
                LoadIO()
                Original_LOD.Visible = True

                If (UserCanEdit) Then
                    DisplayReadWrite()
                Else
                    DisplayReadOnly(0)
                End If
            Else
                LoadIO_v2()
                LOD_v2_Panel.Visible = True

                If (UserCanEdit) Then
                    DisplayReadWrite_v2()
                Else
                    DisplayReadOnly_v2()
                End If
            End If

        End Sub
#End Region

#Region "LOD"
        Protected Sub FindingSelected(sender As Object, e As RadioButtonSelectedEventArgs)
            ' Determine if the Formal Investigating Officer fields need to be displayed...
            Dim isFormalRecommended As Boolean = False

            If (LOD.CurrentStatusCode = LodStatusCode.AppointingAutorityReview AndAlso
                (ucInformalAndReinvestigationFindings.Findings = LodFinding.RecommendFormalInvestigation OrElse
                ucInformalAndReinvestigationFindings.Findings = LodFinding.NotInLineOfDutyDueToOwnMisconduct OrElse
                ucInformalAndReinvestigationFindings.Findings = LodFinding.NotInLineOfDutyNotDueToOwnMisconduct)) Then
                isFormalRecommended = True

            ElseIf (LOD.CurrentStatusCode = LodStatusCode.FormalActionByAppointingAuthority AndAlso
                    LookupService.GetIsReinvestigationLod(LOD.Id) = True AndAlso
                    ucInformalAndReinvestigationFindings.Findings = LodFinding.RecommendFormalInvestigation) Then
                isFormalRecommended = True
            End If

            If (isFormalRecommended) Then
                pnlIOControls.Visible = True
                lblIOLabel.CssClass = "labelRequired"
            Else
                pnlIOControls.Visible = False
                lblIOLabel.CssClass = String.Empty
            End If
        End Sub

        Protected Sub DecisionSelected(sender As Object, e As RadioButtonSelectedEventArgs)
            ' Determine if the finding options need to be enabled or disabled...
            If (ucInformalAndReinvestigationFindings.Decision.Equals("Y")) Then
                ucInformalAndReinvestigationFindings.EnableFindings = False
                ucInformalAndReinvestigationFindings.FindingsIndex = -1
                pnlIOControls.Visible = False
                lblIOLabel.CssClass = String.Empty
            Else
                ucInformalAndReinvestigationFindings.EnableFindings = True
            End If
        End Sub

        Public Sub LoadIO()

            SetMaxLength(txtAAPOC)

            If (UserCanEdit) Then
                Dim ioList As List(Of LookUpItem)
                ioList = LookupService.GetIOList(CInt(HttpContext.Current.Session("UserId")), CByte(HttpContext.Current.Session("ReportView")), LOD.MemberRank.Id)

                cbAssignIo.Items.Insert(0, New ListItem("-- Select Investigating Officer --", "-1"))

                Dim selectionValue As String = LOD.IOSelectionString
                Dim rowValue As Integer = 1
                'Appointed IO
                For Each row As LookUpItem In ioList
                    Dim item As New ListItem(row.Name, rowValue)
                    If (Not String.IsNullOrEmpty(selectionValue) AndAlso selectionValue = row.Value) Then
                        item.Selected = True
                    End If
                    cbAssignIo.Items.Add(item)
                    rowValue = rowValue + 1
                Next
            End If


            If (LOD.LODInvestigation IsNot Nothing) Then
                If LOD.LODInvestigation.IoUserId.HasValue Then
                    Dim ioUser As AppUser = UserService.GetById(LOD.LODInvestigation.IoUserId.Value)
                    lblIO.Text = Server.HtmlEncode("   ( Current IO:" + ioUser.FullName + ")")
                End If
            End If

            If (Not String.IsNullOrEmpty(LOD.InstructionsToInvestigator)) Then
                txtIOInstruction.Text = Server.HtmlDecode(LOD.InstructionsToInvestigator)
            End If

            If (Not String.IsNullOrEmpty(LOD.AppointingAuthorityPoc)) Then
                txtAAPOC.Text = Server.HtmlDecode(LOD.AppointingAuthorityPoc)
            End If

            If (LOD.IoCompletionDate.HasValue) Then
                txtIOCompletionDate.Text = Server.HtmlDecode(LOD.IoCompletionDate.Value.ToString(DATE_FORMAT))
            End If

        End Sub

        Private Sub DisplayReadWrite()

            SigCheckFormal.Visible = False
            pnlAppointedIOReview.Visible = False

            If (LOD.CurrentStatusCode = LodStatusCode.AppointingAutorityReview) Then
                AAEditPanel.Visible = True
                AAReviewPanel.Visible = False
                FormalPanel.Visible = False
                LoadAppointingAuthority()
                SigCheckInformal.Visible = False

                If (Not String.IsNullOrEmpty(ucInformalAndReinvestigationFindings.Findings) AndAlso
                    (ucInformalAndReinvestigationFindings.Findings = LodFinding.RecommendFormalInvestigation OrElse
                    ucInformalAndReinvestigationFindings.Findings = LodFinding.NotInLineOfDutyDueToOwnMisconduct OrElse
                    ucInformalAndReinvestigationFindings.Findings = LodFinding.NotInLineOfDutyNotDueToOwnMisconduct)) Then
                    pnlIOControls.Visible = True
                    lblIOLabel.CssClass = "labelRequired"
                Else
                    pnlIOControls.Visible = False
                    lblIOLabel.CssClass = String.Empty
                End If
            ElseIf (LOD.CurrentStatusCode = LodStatusCode.FormalActionByAppointingAuthority) Then
                ' Check if this LOD is a Reinvestigation LOD...
                If (LookupService.GetIsReinvestigationLod(LOD.Id) = False) Then
                    ' This is a normal formal LOD; therefore, display formal findings controls...
                    AAEditPanel.Visible = False
                    AAReviewPanel.Visible = True
                    FormalPanel.Visible = True
                    LoadAppointingAuthorityReadOnly()
                    LoadFormalAppointingAuthority(False)
                    SigCheckInformal.VerifySignature(RefId)
                Else
                    ' This is a reinvestigation LOD; therefore, display the edit panel which allows the Wing CC to choose a new IO
                    ' as well as additional controls specific to this situation...
                    AAEditPanel.Visible = True
                    AAReviewPanel.Visible = False
                    FormalPanel.Visible = False
                    SigCheckInformal.Visible = False

                    LoadRLodFormalAppointingAuthority()
                    LoadPreviousFindings(True)

                    ucInformalAndReinvestigationFindings.FindingsOnly = False
                    ucInformalAndReinvestigationFindings.ConcurWith = "Investigation Officer"
                    ucInformalAndReinvestigationFindings.RenameFindingText("Recommend Formal LOD Investigation", "Conduct Formal Investigation")
                    ucInformalAndReinvestigationFindings.FindingsLableText = "Substituted Findings:"
                    ucInformalAndReinvestigationFindings.ReasonsLabelText = "Reasons:"
                    ucInformalAndReinvestigationFindings.ShownOnText = "(Shown on Form 261)"
                    ucInformalAndReinvestigationFindings.ShowRemarks = False
                    ucInformalAndReinvestigationFindings.ShowFormText = True
                    ucInformalAndReinvestigationFindings.SetDecisionToggle = True
                    ucInformalAndReinvestigationFindings.FindingsRequired = False

                    lblAAEditPanelHeader.Text = "Formal Appointing Authority Review"
                    lblIORow.Text = "D"
                    lblIOCompletedByRow.Text = "E"
                    lblIOInstructionsRow.Text = "F"
                    lblIOPOCRow.Text = "G"

                    If (ucInformalAndReinvestigationFindings.Decision.Equals("Y")) Then
                        ucInformalAndReinvestigationFindings.EnableFindings = False
                        ucInformalAndReinvestigationFindings.FindingsIndex = -1
                    Else
                        ucInformalAndReinvestigationFindings.EnableFindings = True
                    End If

                    If (ucInformalAndReinvestigationFindings.Decision.Equals("N") AndAlso ucInformalAndReinvestigationFindings.Findings = LodFinding.RecommendFormalInvestigation) Then
                        pnlIOControls.Visible = True
                        lblIOLabel.CssClass = "labelRequired"
                    Else
                        pnlIOControls.Visible = False
                        lblIOLabel.CssClass = String.Empty
                    End If
                End If
            ElseIf (LOD.CurrentStatusCode = LodStatusCode.NotifyFormalInvestigator) Then
                SigCheckFormal.Visible = False
                AAEditPanel.Visible = False
                AAReviewPanel.Visible = True
                FormalPanel.Visible = False
                DisplayReadOnly(0)
                'DisplayAppointedIOReviewControls()
            End If

        End Sub

        Private Sub DisplayReadOnly(ByVal ShowStoppers As Integer)

            SigCheckInformal.VerifySignature(RefId)
            SigCheckFormal.VerifySignature(RefId, PersonnelTypes.FORMAL_APP_AUTH)

            Dim isRLod As Boolean = LookupService.GetIsReinvestigationLod(LOD.Id)

            AAEditPanel.Visible = False
            AAReviewPanel.Visible = True

            If (isRLod = False) Then
                LoadAppointingAuthorityReadOnly()
            Else
                trIOFindings.Visible = True
                trDecision.Visible = True
                lblAAReviewPanelHeader.Text = "Formal Appointing Authority Review"
                lblAARevFindings.Text = "Substituted Findings:"
                lblAARevRemarks.Text = "Reasons:<br />(Shown on Form261)"
                lblAARevFindingsRow.Text = "B"
                lblAARevRemarksRow.Text = "C"
                lblAARevIORow.Text = "D"
                lblAARevIOCompletedByRow.Text = "E"
                lblAARevIOInstructionsRow.Text = "F"
                lblAARevIOPOCRow.Text = "G"
                LoadRLodFormalAppointingAuthorityReadOnly()
            End If


            If (Session("GroupId") = UserGroups.WingCommander) OrElse (Session("AccessScope") = AccessScope.Compo) Then
                If (LOD.Formal AndAlso isRLod = False) Then
                    LoadFormalAppointingAuthority(True)
                    FormalPanel.Visible = True
                Else
                    FormalPanel.Visible = False
                End If
            Else
                FormalPanel.Visible = False
            End If

            DisplayAppointedIOReviewControls()

        End Sub

        Private Sub DisplayAppointedIOReviewControls()
            pnlAppointedIOReview.Visible = True

            If (Session("GroupId") = UserGroups.LOD_PM AndAlso LOD.CurrentStatusCode = LodStatusCode.NotifyFormalInvestigator) Then
                lblAppointedIOEDIPI.Visible = False
                txtAppointedIOEDIPI.Visible = True
                txtAppointedIOUnit.Visible = True
                btnChangeUnit.Visible = True
                lblAppointedIOUnit.Visible = False
                lblAppointedIOIATraining.Visible = False
                txtAppointedIOIATraining.Visible = True
                txtAppointedIOEmail.Visible = True
                RequiredFieldValidator1.Visible = True

                LoadAppointedIO(False)
            Else
                lblAppointedIOEDIPI.Visible = True
                txtAppointedIOEDIPI.Visible = False
                txtAppointedIOUnit.Visible = False
                btnChangeUnit.Visible = False
                lblAppointedIOUnit.Visible = True
                lblAppointedIOIATraining.Visible = True
                txtAppointedIOIATraining.Visible = False
                lblAppointedIOEmail.Visible = True
                RequiredFieldValidator1.Visible = False

                LoadAppointedIO(True)
            End If
        End Sub

        Public Sub LoadAppointingAuthority()

            Dim finding As LineOfDutyFindings
            finding = LOD.FindByType(PersonnelTypes.APPOINT_AUTH)

            ucInformalAndReinvestigationFindings.LoadFindingOptionsByWorkflow(LOD.Workflow, UserGroups.WingCommander)
            ucInformalAndReinvestigationFindings.RenameFindingText("Recommend Formal LOD Investigation", "Recommend Formal Investigation")

            If (finding IsNot Nothing) Then

                If (finding.Finding.HasValue AndAlso finding.Finding > 0) Then
                    ucInformalAndReinvestigationFindings.Findings = finding.Finding.Value
                End If

                If (Not String.IsNullOrEmpty(finding.Explanation)) Then
                    ucInformalAndReinvestigationFindings.Remarks = Server.HtmlDecode(finding.Explanation)
                End If

                If (Not String.IsNullOrEmpty(LOD.InstructionsToInvestigator)) Then
                    txtIOInstruction.Text = Server.HtmlDecode(LOD.InstructionsToInvestigator)
                End If

                If (Not String.IsNullOrEmpty(LOD.AppointingAuthorityPoc)) Then
                    txtAAPOC.Text = Server.HtmlDecode(LOD.AppointingAuthorityPoc)
                End If

                If (LOD.IoCompletionDate.HasValue) Then
                    txtIOCompletionDate.Text = Server.HtmlDecode(LOD.IoCompletionDate.Value.ToString(DATE_FORMAT))
                End If

            End If
        End Sub

        Public Sub LoadAppointingAuthorityReadOnly()

            Dim finding As LineOfDutyFindings
            finding = LOD.FindByType(PersonnelTypes.APPOINT_AUTH)
            Dim formal As Boolean = False

            If (finding IsNot Nothing) Then

                If (finding.Finding.HasValue AndAlso finding.Finding > 0) Then
                    For Each f As FindingsLookUp In New LookupDao().GetWorkflowFindings(LOD.Workflow, UserGroups.WingCommander)
                        If (f.Id = finding.Finding.Value) Then
                            FindingsLabel.Text = f.Description
                        End If
                    Next

                    If (finding.Finding.Value = LodFinding.NotInLineOfDutyDueToOwnMisconduct OrElse
                        LodFinding.NotInLineOfDutyNotDueToOwnMisconduct OrElse
                        LodFinding.RecommendFormalInvestigation) Then
                        formal = True
                    End If

                    If (formal) Then

                        Dim scope As AccessScope = Session("AccessScope")
                        Dim group As Integer = CInt(Session("GroupId"))

                        If (scope = AccessScope.Compo) OrElse (group = UserGroups.WingCommander) OrElse UserHasPermission("exePostCompletion") Then

                            IoPanel.Visible = True

                            CurrentIoLabel.Text = LodService.AppointedIONameAndRank(LOD)

                            If (Not String.IsNullOrEmpty(LOD.InstructionsToInvestigator)) Then
                                InstructionsLabel.Text = LOD.InstructionsToInvestigator.Replace(vbCrLf, "<br />") + "<br /><br /><br />"
                            End If

                            If (Not String.IsNullOrEmpty(LOD.AppointingAuthorityPoc)) Then
                                PocLabel.Text = LOD.AppointingAuthorityPoc.Replace(vbCrLf, "<br />") + "<br /><br /><br />"
                            End If

                            If (LOD.IoCompletionDate.HasValue) Then
                                CompleteByLabel.Text = LOD.IoCompletionDate.Value.ToString(DATE_FORMAT)
                            End If

                        Else
                            'this is a unit/wing level, hide the IO info
                            IoPanel.Visible = False
                        End If

                    End If

                End If

                If (Not String.IsNullOrEmpty(finding.Explanation)) Then
                    RemarksLabel.Text = finding.Explanation.Replace(vbCrLf, "<br />") + "<br /><br /><br />"
                End If

            End If
        End Sub

        Public Sub LoadFormalAppointingAuthority(ByVal isReadOnly As Boolean)
            ' Load findings for a Formal LOD that is not a Reinvestigation LOD
            LoadPreviousFindings(False)
            FormalFindings.SetReadOnly = isReadOnly

            Dim cFinding As LineOfDutyFindings
            cFinding = LOD.FindByType(PersonnelTypes.FORMAL_APP_AUTH)

            FormalFindings.LoadFindingOptionsByWorkflow(LOD.Workflow, UserGroups.WingCommander)

            If Not (cFinding Is Nothing) Then
                If cFinding.DecisionYN <> String.Empty Then FormalFindings.Decision = cFinding.DecisionYN
                If (Not cFinding.Finding Is Nothing AndAlso cFinding.Finding > 0) Then
                    FormalFindings.Findings = cFinding.Finding.Value
                Else
                    FormalFindings.Findings = Nothing
                End If
                FormalFindings.Remarks = cFinding.Explanation
                FormalFindings.FormFindingsText = cFinding.FindingsText
            End If

        End Sub

        Public Sub LoadRLodFormalAppointingAuthority()
            ' Load findings for a Formal LOD that is a Reinvestigation LOD
            Dim finding As LineOfDutyFindings
            finding = LOD.FindByType(PersonnelTypes.FORMAL_APP_AUTH)

            ucInformalAndReinvestigationFindings.LoadFindingOptionsByWorkflow(LOD.Workflow, UserGroups.WingCommander)

            If (finding IsNot Nothing) Then
                If finding.DecisionYN <> String.Empty Then
                    ucInformalAndReinvestigationFindings.Decision = finding.DecisionYN
                End If

                If (finding.Finding.HasValue AndAlso finding.Finding > 0) Then
                    ucInformalAndReinvestigationFindings.Findings = finding.Finding.Value
                End If

                If (Not String.IsNullOrEmpty(finding.FindingsText)) Then
                    ucInformalAndReinvestigationFindings.FormFindingsText = Server.HtmlDecode(finding.FindingsText)
                End If

                If (Not String.IsNullOrEmpty(LOD.InstructionsToInvestigator)) Then
                    txtIOInstruction.Text = Server.HtmlDecode(LOD.InstructionsToInvestigator)
                End If

                If (Not String.IsNullOrEmpty(LOD.AppointingAuthorityPoc)) Then
                    txtAAPOC.Text = Server.HtmlDecode(LOD.AppointingAuthorityPoc)
                End If

                If (LOD.IoCompletionDate.HasValue) Then
                    txtIOCompletionDate.Text = Server.HtmlDecode(LOD.IoCompletionDate.Value.ToString(DATE_FORMAT))
                End If

            End If

            Dim ioFinding As LineOfDutyFindings
            ioFinding = LOD.FindByType(PersonnelTypes.IO)

            If (ioFinding Is Nothing) Then
                ' There is no IO findings associated with this reinvestigation LOD which means a new investigation was never conducted;
                ' therefore, it makes no sense to allow the user to decide if they concur or don't concur with the IO...
                ucInformalAndReinvestigationFindings.Decision = "N"
                ucInformalAndReinvestigationFindings.EnableDecision = False

                ' Also force the user to select the option to conduct an investigation...
                ucInformalAndReinvestigationFindings.DisableAllFindingsExcept("Recommend Formal LOD Investigation")
                ucInformalAndReinvestigationFindings.Findings = LodFinding.RecommendFormalInvestigation
            End If
        End Sub

        Public Sub LoadRLodFormalAppointingAuthorityReadOnly()
            Dim ioFinding As LineOfDutyFindings
            ioFinding = LOD.FindByType(PersonnelTypes.IO)

            If (ioFinding IsNot Nothing) Then
                If (ioFinding.Finding.HasValue AndAlso ioFinding.Finding > 0) Then
                    lblAARevIOFindings.Text = ioFinding.Description
                Else
                    lblAARevIOFindings.Text = "Not Found"
                End If
            Else
                lblAARevIOFindings.Text = "Not Found"
            End If

            Dim finding As LineOfDutyFindings
            finding = LOD.FindByType(PersonnelTypes.FORMAL_APP_AUTH)

            If (finding IsNot Nothing) Then

                If (Not String.IsNullOrEmpty(finding.DecisionYN)) Then
                    If (finding.DecisionYN = "Y") Then
                        lblAARevDecision.Text = "Concur with the action of Investigation Officer"
                    Else
                        lblAARevDecision.Text = "Non Concur with the action of Investigation Officer"
                    End If
                End If

                If (finding.Finding.HasValue AndAlso finding.Finding > 0) Then
                    For Each f As FindingsLookUp In New LookupDao().GetWorkflowFindings(LOD.Workflow, UserGroups.WingCommander)
                        If (f.Id = finding.Finding.Value) Then
                            FindingsLabel.Text = f.Description
                        End If
                    Next
                End If

                If (Not String.IsNullOrEmpty(finding.FindingsText)) Then
                    RemarksLabel.Text = finding.FindingsText.Replace(vbCrLf, "<br />") + "<br /><br /><br />"
                End If

                Dim scope As AccessScope = Session("AccessScope")
                Dim group As Integer = CInt(Session("GroupId"))

                If (scope = AccessScope.Compo) OrElse (group = UserGroups.WingCommander) OrElse UserHasPermission("exePostCompletion") Then
                    IoPanel.Visible = True

                    CurrentIoLabel.Text = LodService.AppointedIONameAndRank(LOD)

                    If (Not String.IsNullOrEmpty(LOD.InstructionsToInvestigator)) Then
                        InstructionsLabel.Text = LOD.InstructionsToInvestigator.Replace(vbCrLf, "<br />") + "<br /><br /><br />"
                    End If

                    If (Not String.IsNullOrEmpty(LOD.AppointingAuthorityPoc)) Then
                        PocLabel.Text = LOD.AppointingAuthorityPoc.Replace(vbCrLf, "<br />") + "<br /><br /><br />"
                    End If

                    If (LOD.IoCompletionDate.HasValue) Then
                        CompleteByLabel.Text = LOD.IoCompletionDate.Value.ToString(DATE_FORMAT)
                    End If
                Else
                    'this is a unit/wing level, hide the IO info
                    IoPanel.Visible = False
                End If
            End If

        End Sub

        Public Sub LoadPreviousFindings(ByVal isRLod As Boolean)
            Dim findingsText As String = ""

            Dim ioFinding As LineOfDutyFindings
            ioFinding = LOD.FindByType(PersonnelTypes.IO)

            ' Check if this LOD is a Reinvestigation LOD...
            If (Not isRLod) Then
                ' This is not a reinvestigation LOD; therefore, load the previous findings into the formal findings control...
                FormalFindings.PrevFindingsLableText = "IO Findings:"
                FormalFindings.PrevFindingsText = "Not found"

                If (ioFinding IsNot Nothing AndAlso ioFinding.Finding > 0) Then
                    FormalFindings.PrevFindingsText = ioFinding.Description
                End If
            Else
                ' This is a reinvestigation LOD; therefore, load previous findings into the controls on the edit panel...
                ucInformalAndReinvestigationFindings.ShowPrevFindings = True
                ucInformalAndReinvestigationFindings.PrevFindingsLableText = "IO Findings:"
                ucInformalAndReinvestigationFindings.PrevFindingsText = "Not found"

                If (ioFinding IsNot Nothing) Then
                    If (ioFinding.Finding IsNot Nothing AndAlso ioFinding.Finding > 0) Then
                        ucInformalAndReinvestigationFindings.PrevFindingsText = ioFinding.Description
                    End If
                End If
            End If
        End Sub

        Private Sub LoadAppointedIO(isReadOnly As Boolean)
            If (LOD.AppointedIO IsNot Nothing) Then
                LoadAppointedIO(LOD.AppointedIO, isReadOnly)
            ElseIf (Not String.IsNullOrEmpty(LOD.IoSsn)) Then
                LoadAppointedIO(LookupService.GetServiceMemberBySSN(LOD.IoSsn), isReadOnly)
            Else
                Exit Sub
            End If
        End Sub

        Private Sub LoadAppointedIO(appointedIo AS AppUser, isReadOnly As Boolean)
            If (appointedIo Is Nothing)
                Exit Sub
            End If

            lblAppointedIOName.Text = appointedIo.FullName
            lblAppointedIORank.Text = appointedIo.Rank.Rank
            lblAppointedIOComponent.Text = GetCompoString(appointedIo.Component)

            If (isReadOnly) Then
                lblAppointedIOEDIPI.Text = appointedIo.EDIPIN
                lblAppointedIOEmail.Text = appointedIo.Email
            Else
                txtAppointedIOEDIPI.Text = appointedIo.EDIPIN
                txtAppointedIOEmail.Text = appointedIo.Email
            End If

            If (appointedIo.AccountExpiration.HasValue) Then
                If (isReadOnly) Then
                    lblAppointedIOIATraining.Text = appointedIo.AccountExpiration.Value.ToString(DATE_FORMAT)
                Else
                    txtAppointedIOIATraining.Text = appointedIo.AccountExpiration.Value.ToString(DATE_FORMAT)
                End If
            End If

            If (appointedIo.Unit IsNot Nothing) Then
                If (isReadOnly) Then
                    lblAppointedIOUnit.Text = appointedIo.Unit.NameAndPasCode
                Else
                    txtAppointedIOUnit.Text = appointedIo.Unit.NameAndPasCode
                    lblNewUnitID.Text = appointedIo.Unit.Id.ToString()
                End If
            End If
        End Sub

        Private Sub LoadAppointedIO(memberData As ServiceMember, isReadOnly As Boolean)
            If (memberData Is nothing) Then
                Exit Sub
            End If

            lblAppointedIOName.Text = memberData.FullName
            lblAppointedIORank.Text = memberData.Rank.Rank
            lblAppointedIOComponent.Text = GetCompoString(memberData.Component)

            If (memberData.Unit IsNot Nothing) Then
                If (isReadOnly)
                    lblAppointedIOUnit.Text = memberData.Unit.NameAndPasCode
                Else
                    txtAppointedIOUnit.Text = memberData.Unit.NameAndPasCode
                    lblNewUnitID.Text = memberData.Unit.Id.ToString()
                End If
            End If
        End Sub

        Private Sub SaveFindings()

            'Save the decision data 

            If (Not UserCanEdit) Then
                Exit Sub
            End If

            If (LOD.CurrentStatusCode = LodStatusCode.AppointingAutorityReview) Then
                SaveAppointingAuthority()
            ElseIf (LOD.CurrentStatusCode = LodStatusCode.FormalActionByAppointingAuthority) Then
                ' Check if this LOD is a Reinvestigation LOD...
                If (LookupService.GetIsReinvestigationLod(LOD.Id) = False) Then
                    SaveFormalAction()
                Else
                    SaveRLodFormalAction()
                End If
            ElseIf (LOD.CurrentStatusCode = LodStatusCode.NotifyFormalInvestigator) Then
                SaveAppointedIO()
            End If

        End Sub

        Private Sub SaveAppointingAuthority()

            'save the findings
            Dim finding As LineOfDutyFindings = CreateAppointingAuthorityFinding(PersonnelTypes.APPOINT_AUTH)

            finding.Explanation = ucInformalAndReinvestigationFindings.Remarks.Trim()

            Dim formal As Boolean = False

            If (ucInformalAndReinvestigationFindings.Findings Is Nothing) Then
                finding.Finding = Nothing
            Else
                finding.Finding = ucInformalAndReinvestigationFindings.Findings
            End If

            If (finding.Finding = LodFinding.NotInLineOfDutyDueToOwnMisconduct OrElse
                finding.Finding = LodFinding.RecommendFormalInvestigation OrElse
                finding.Finding = LodFinding.NotInLineOfDutyNotDueToOwnMisconduct) Then
                formal = True
            End If

            If (formal) Then
                LOD.AppointingAuthorityPoc = Server.HtmlEncode(txtAAPOC.Text.Trim)
                LOD.InstructionsToInvestigator = Server.HtmlEncode(txtIOInstruction.Text.Trim)
                If (CheckDate(txtIOCompletionDate)) Then
                    LOD.IoCompletionDate = Server.HtmlEncode(DateTime.Parse(txtIOCompletionDate.Text))
                End If

                'save the IO data
                LOD.IoSsn = Nothing
                Dim selectedValue As String = String.Empty
                If (cbAssignIo.SelectedIndex > 0) Then
                    Dim ioList As List(Of LookUpItem) = LookupService.GetIOList(CInt(HttpContext.Current.Session("UserId")), CByte(HttpContext.Current.Session("ReportView")), LOD.MemberRank.Id)
                    selectedValue = ioList.Item(cbAssignIo.SelectedIndex - 1).Value.Trim()
                    If (selectedValue <> String.Empty) Then
                        If InStr(selectedValue, "ssn") > 0 Then
                            LOD.IoSsn = selectedValue.Split(":")(1)
                            LOD.AppointedIO = Nothing
                        End If
                        If InStr(selectedValue, "uid") > 0 Then
                            LOD.AppointedIO = UserService.GetById(CInt(selectedValue.Split(":")(1)))
                            LOD.IoSsn = Nothing
                        End If
                    End If

                End If
            End If

            ucInformalAndReinvestigationFindings.Remarks = finding.Explanation
            LOD.SetFindingByType(finding)
        End Sub

        Private Sub SaveFormalAction()

            Dim finding As LineOfDutyFindings = CreateAppointingAuthorityFinding(PersonnelTypes.FORMAL_APP_AUTH)

            finding.DecisionYN = FormalFindings.Decision

            If (FormalFindings.Decision <> "Y" AndAlso Not FormalFindings.Findings Is Nothing) Then
                finding.Finding = FormalFindings.Findings
            End If

            finding.Explanation = FormalFindings.Remarks            ' findings control automatically html encodes properties
            finding.FindingsText = FormalFindings.FormFindingsText

            LOD.SetFindingByType(finding)

        End Sub

        Private Sub SaveRLodFormalAction()
            Dim isReconductFormalIO As Boolean = False
            Dim finding As LineOfDutyFindings = CreateAppointingAuthorityFinding(PersonnelTypes.FORMAL_APP_AUTH)

            finding.FindingsText = ucInformalAndReinvestigationFindings.FormFindingsText

            finding.DecisionYN = ucInformalAndReinvestigationFindings.Decision

            If (ucInformalAndReinvestigationFindings.Decision <> String.Empty AndAlso ucInformalAndReinvestigationFindings.Decision <> "Y") Then
                ' Save selected finding...
                If (ucInformalAndReinvestigationFindings.Findings Is Nothing) Then
                    finding.Finding = Nothing
                Else
                    finding.Finding = ucInformalAndReinvestigationFindings.Findings
                End If

                If (finding.Finding = LodFinding.RecommendFormalInvestigation) Then
                    isReconductFormalIO = True
                End If
            End If


            ' Save IO information if necessary....
            If (isReconductFormalIO) Then
                LOD.AppointingAuthorityPoc = Server.HtmlEncode(txtAAPOC.Text.Trim)
                LOD.InstructionsToInvestigator = Server.HtmlEncode(txtIOInstruction.Text.Trim)

                If (CheckDate(txtIOCompletionDate)) Then
                    LOD.IoCompletionDate = Server.HtmlEncode(DateTime.Parse(txtIOCompletionDate.Text))
                End If

                LOD.IoSsn = Nothing
                Dim selectedValue As String = String.Empty

                If (cbAssignIo.SelectedIndex > 0) Then
                    Dim ioList As List(Of LookUpItem) = LookupService.GetIOList(CInt(HttpContext.Current.Session("UserId")), CByte(HttpContext.Current.Session("ReportView")), LOD.MemberRank.Id)

                    selectedValue = ioList.Item(cbAssignIo.SelectedIndex - 1).Value.Trim()

                    If (selectedValue <> String.Empty) Then
                        If InStr(selectedValue, "ssn") > 0 Then
                            LOD.IoSsn = selectedValue.Split(":")(1)
                            LOD.AppointedIO = Nothing
                        End If
                        If InStr(selectedValue, "uid") > 0 Then
                            LOD.AppointedIO = UserService.GetById(CInt(selectedValue.Split(":")(1)))
                            LOD.IoSsn = Nothing
                        End If
                    End If
                End If
            Else
                'LOD.AppointingAuthorityPoc = String.Empty
                'LOD.InstructionsToInvestigator = String.Empty
                'LOD.IoCompletionDate = Nothing
                'LOD.IoSsn = Nothing
                'LOD.AppointedIO = Nothing
            End If

            LOD.SetFindingByType(finding)
        End Sub

        Private Sub SaveAppointedIO()
            ValidateAppointedIoInput()

            If (AreTheirAppointedIoInputErrors()) Then
                trAppointedIOError.Visible = True
                Exit Sub
            End If

            Dim wasModified As Boolean = False

            If (LOD.AppointedIO IsNot Nothing) Then
                wasModified = UpdateExistingAccountWithIoRole(LOD.AppointedIO, False)
                AssignSubmittedDataToInvestigatingOfficer(LOD.AppointedIO, wasModified)
                UpdateLodInvestigationAssignedIo(LOD.AppointedIO)
            ElseIf (Not String.IsNullOrEmpty(LOD.IoSsn)) Then
                Dim ioUser As AppUser = FindExistingUserAccount(txtAppointedIOEDIPI.Text.Trim, LOD.IoSsn)

                If (ioUser IsNot Nothing) Then
                    wasModified = UpdateExistingAccountWithIoRole(ioUser, True)
                Else
                    ioUser = CreateNewInvestigatingOfficerUserAccount(bllAppointedIOErrors)
                    wasModified = True
                End If

                If (ioUser IsNot Nothing) Then
                    AssignSubmittedDataToInvestigatingOfficer(ioUser, wasModified)
                    UserDao.SaveOrUpdate(ioUser)
                    LOD.AppointedIO = ioUser
                    UpdateLodInvestigationAssignedIo(ioUser)
                End If
            End If

            trAppointedIOError.Visible = AreTheirAppointedIoInputErrors()
        End Sub

        Private Sub ValidateAppointedIoInput()
            bllAppointedIOErrors.Items.Clear()

            If (AreAnyAppointedIOControlsEmpty()) Then
                bllAppointedIOErrors.Items.Add("The fields displayed in RED must have values entered into them.")
            End If

            If (Not String.IsNullOrEmpty(txtAppointedIOEDIPI.Text) AndAlso Not DoesEDIPIMeetFormatRequirements(txtAppointedIOEDIPI.Text)) Then
                bllAppointedIOErrors.Items.Add(txtAppointedIOEDIPI.Text.Trim + " does not meet the format requirements for an EDIPI number.")
            End If

            If (Not String.IsNullOrEmpty(txtAppointedIOEmail.Text) AndAlso Not DoesEmailMeetFormatRequirements(txtAppointedIOEmail.Text)) Then
                bllAppointedIOErrors.Items.Add(txtAppointedIOEmail.Text.Trim + " does not meet the format requirements for an email address.")
            End If

            If (Not String.IsNullOrEmpty(txtAppointedIOIATraining.Text) AndAlso Date.Parse(txtAppointedIOIATraining.Text.Trim) <= Date.Now) Then
                bllAppointedIOErrors.Items.Add("IA Training date must be greater than the current date.")
            End If
        End Sub

        Private Function AreAnyAppointedIOControlsEmpty() As Boolean
            Dim result As Boolean = False

            If (Not ValidateWebServerTextControl(txtAppointedIOEDIPI, txtAppointedIOEDIPI.Text)) Then
                result = True
            End If

            If (Not ValidateWebServerTextControl(txtAppointedIOIATraining, txtAppointedIOIATraining.Text)) Then
                result = True
            End If

            If (Not ValidateWebServerTextControl(txtAppointedIOUnit, lblNewUnitID.Text)) Then
                result = True
            End If

            If (Not ValidateWebServerTextControl(txtAppointedIOEmail, txtAppointedIOEmail.Text)) Then
                result = True
            End If

            Return result
        End Function

        Private Function AreTheirAppointedIoInputErrors() As Boolean
            Return (bllAppointedIOErrors.Items.Count > 0)
        End Function

        Private Sub AssignSubmittedDataToInvestigatingOfficer(ioUser As AppUser, hasAlreadyBeenModified As Boolean)
            Dim wasModified As Boolean = hasAlreadyBeenModified

            If (String.IsNullOrEmpty(ioUser.EDIPIN) OrElse Not ioUser.EDIPIN.Equals(txtAppointedIOEDIPI.Text.Trim)) Then
                If (UserDao.IsEDIPINAvailable(txtAppointedIOEDIPI.Text.Trim)) Then
                    ioUser.EDIPIN = Server.HtmlEncode(txtAppointedIOEDIPI.Text.Trim)
                    wasModified = True
                Else
                    bllAppointedIOErrors.Items.Add("A user account with an EDIPI number of " + txtAppointedIOEDIPI.Text.Trim + " already exists in the system.")
                    txtAppointedIOEDIPI.Text = ioUser.EDIPIN
                End If
            End If

            If (Not ioUser.AccountExpiration.Value.Date.Equals(Date.Parse(txtAppointedIOIATraining.Text.Trim))) Then
                ioUser.AccountExpiration = Server.HtmlEncode(Date.Parse(txtAppointedIOIATraining.Text.Trim))
                wasModified = True
            End If

            If (Not ioUser.Email.Equals(txtAppointedIOEmail.Text.Trim())) Then
                ioUser.Email = txtAppointedIOEmail.Text.Trim()
                wasModified = True
            End If

            Dim newUnit As Unit = UnitDao.FindById(CInt(lblNewUnitID.Text.Trim))
            If (newUnit IsNot Nothing AndAlso newUnit.Id <> ioUser.Unit.Id) Then
                ioUser.Unit = newUnit
                wasModified = True
                txtAppointedIOUnit.Text = newUnit.NameAndPasCode
            End If

            If (wasModified) Then
                ioUser.ModifiedDate = DateTime.Now
                ioUser.ModifiedBy = UserDao.GetById(CInt(HttpContext.Current.Session("UserId")))
            End If
        End Sub
#End Region

#Region "LOD_v2"
        Protected Sub FindingSelected_v2(sender As Object, e As RadioButtonSelectedEventArgs)
            ' Determine if the Formal Investigating Officer fields need to be displayed...
            Dim isFormalRecommended As Boolean = False
            Dim subFindings As Boolean = False

            If (LOD.CurrentStatusCode = LodStatusCode.AppointingAutorityReview AndAlso
                (ucInformalAndReinvestigationFindings_v2.Findings = LodFinding.NotInLineOfDutyNotDueToOwnMisconduct)) Then
                subFindings = True


            ElseIf (LOD.CurrentStatusCode = LodStatusCode.AppointingAutorityReview AndAlso
                (ucInformalAndReinvestigationFindings_v2.Findings = LodFinding.RecommendFormalInvestigation)) Then

                isFormalRecommended = True

            ElseIf (LOD.CurrentStatusCode = LodStatusCode.FormalActionByAppointingAuthority AndAlso
                    LookupService.GetIsReinvestigationLod(LOD.Id) = True AndAlso
                    ucInformalAndReinvestigationFindings_v2.Findings = LodFinding.RecommendFormalInvestigation) Then
                isFormalRecommended = True
            End If

            If (isFormalRecommended) Then
                pnlIOControls_v2.Visible = True
                lblIOLabel_v2.CssClass = "labelRequired"
            Else
                pnlIOControls_v2.Visible = False
                lblIOLabel_v2.CssClass = String.Empty
            End If

            If (subFindings) Then
                PopulateSubFindings()
                NILOD_subFindings.Visible = True

                lblIORow_v2.Text = "D"
                lblIOCompletedByRow_v2.Text = "E"
                lblIOInstructionsRow_v2.Text = "F"
                lblIOPOCRow_v2.Text = "G"

            Else

                NILOD_subFindings.Visible = False

                lblIORow_v2.Text = "C"
                lblIOCompletedByRow_v2.Text = "D"
                lblIOInstructionsRow_v2.Text = "E"
                lblIOPOCRow_v2.Text = "F"
            End If
        End Sub
        
        Protected Sub PopulateSubFindings()
            rblSubFindings.DataSource = LookupDao.GetAllFindingByReasonOfs()
            rblSubFindings.DataTextField = "Description"
            rblSubFindings.DataValueField = "Id"
            rblSubFindings.DataBind()

        End Sub

        Protected Sub DecisionSelected_v2(sender As Object, e As RadioButtonSelectedEventArgs)
            ' Determine if the finding options need to be enabled or disabled...
            If (ucInformalAndReinvestigationFindings_v2.Decision.Equals("Y")) Then
                ucInformalAndReinvestigationFindings_v2.EnableFindings = False
                ucInformalAndReinvestigationFindings_v2.FindingsIndex = -1
                pnlIOControls_v2.Visible = False
                lblIOLabel_v2.CssClass = String.Empty
            Else
                ucInformalAndReinvestigationFindings_v2.EnableFindings = True
            End If
        End Sub

        Public Sub LoadIO_v2()

            SetMaxLength(txtAAPOC_v2)

            If (UserCanEdit) Then
                Dim ioList As List(Of LookUpItem)
                ioList = LookupService.GetIOList(CInt(HttpContext.Current.Session("UserId")), CByte(HttpContext.Current.Session("ReportView")), LOD.MemberRank.Id)

                cbAssignIo_v2.Items.Insert(0, New ListItem("-- Select Investigating Officer --", "-1"))

                Dim selectionValue As String = LOD.IOSelectionString
                Dim rowValue As Integer = 1
                'Appointed IO
                For Each row As LookUpItem In ioList
                    Dim item As New ListItem(row.Name, rowValue)
                    If (Not String.IsNullOrEmpty(selectionValue) AndAlso selectionValue = row.Value) Then
                        item.Selected = True
                    End If
                    cbAssignIo_v2.Items.Add(item)
                    rowValue = rowValue + 1
                Next
            End If


            If (LOD.LODInvestigation IsNot Nothing) Then
                If LOD.LODInvestigation.IoUserId.HasValue Then
                    Dim ioUser As AppUser = UserService.GetById(LOD.LODInvestigation.IoUserId.Value)
                    lblIO_v2.Text = Server.HtmlEncode("   ( Current IO:" + ioUser.FullName + ")")
                End If
            End If

            If (Not String.IsNullOrEmpty(LOD.InstructionsToInvestigator)) Then
                txtIOInstruction_v2.Text = Server.HtmlDecode(LOD.InstructionsToInvestigator)
            End If

            If (Not String.IsNullOrEmpty(LOD.AppointingAuthorityPoc)) Then
                txtAAPOC_v2.Text = Server.HtmlDecode(LOD.AppointingAuthorityPoc)
            End If

            If (LOD.IoCompletionDate.HasValue) Then
                txtIOCompletionDate_v2.Text = Server.HtmlDecode(LOD.IoCompletionDate.Value.ToString(DATE_FORMAT))
            End If

        End Sub

        Private Sub DisplayReadWrite_v2()

            SigCheckFormal_v2.Visible = False
            pnlAppointedIOReview_v2.Visible = False

            If (LOD.CurrentStatusCode = LodStatusCode.AppointingAutorityReview) Then
                AAEditPanel_v2.Visible = True
                AAReviewPanel_v2.Visible = False
                FormalPanel_v2.Visible = False
                LoadAppointingAuthority_v2()
                SigCheckInformal_v2.Visible = False

                If (Not String.IsNullOrEmpty(ucInformalAndReinvestigationFindings_v2.Findings) AndAlso
                    (ucInformalAndReinvestigationFindings_v2.Findings = LodFinding.RecommendFormalInvestigation)) Then
                    pnlIOControls_v2.Visible = True
                    lblIOLabel_v2.CssClass = "labelRequired"
                ElseIf (Not String.IsNullOrEmpty(ucInformalAndReinvestigationFindings_v2.Findings) AndAlso
                    (ucInformalAndReinvestigationFindings_v2.Findings = LodFinding.NotInLineOfDutyNotDueToOwnMisconduct)) Then

                    NILOD_subFindings.Visible = True
                    PopulateSubFindings()

                    If (lod_v2.NILODsubFinding.HasValue AndAlso lod_v2.NILODsubFinding = NILODSubFindings.AbsentwithoutAuthority) Then

                        rblSubFindings.SelectedValue = lod_v2.NILODsubFinding
                        pnlIOControls_v2.Visible = True
                        lblIOLabel_v2.CssClass = "labelRequired"

                    ElseIf (lod_v2.NILODsubFinding.HasValue AndAlso lod_v2.NILODsubFinding = NILODSubFindings.EPTS_NSA) Then

                        rblSubFindings.SelectedValue = lod_v2.NILODsubFinding

                    End If

                Else
                    pnlIOControls_v2.Visible = False
                    lblIOLabel_v2.CssClass = String.Empty
                End If
            ElseIf (LOD.CurrentStatusCode = LodStatusCode.FormalActionByAppointingAuthority) Then
                ' Check if this LOD is a Reinvestigation LOD...
                If (LookupService.GetIsReinvestigationLod(LOD.Id) = False) Then
                    ' This is a normal formal LOD; therefore, display formal findings controls...
                    AAEditPanel_v2.Visible = False
                    AAReviewPanel_v2.Visible = True
                    FormalPanel_v2.Visible = True
                    LoadAppointingAuthorityReadOnly_v2()
                    LoadFormalAppointingAuthority_v2(False)
                    SigCheckInformal_v2.VerifySignature(RefId)
                Else
                    ' This is a reinvestigation LOD; therefore, display the edit panel which allows the Wing CC to choose a new IO
                    ' as well as additional controls specific to this situation...
                    AAEditPanel_v2.Visible = True
                    AAReviewPanel_v2.Visible = False
                    FormalPanel_v2.Visible = False
                    SigCheckInformal_v2.Visible = False

                    LoadRLodFormalAppointingAuthority_v2()
                    LoadPreviousFindings_v2(True)

                    ucInformalAndReinvestigationFindings_v2.FindingsOnly = False
                    ucInformalAndReinvestigationFindings_v2.ConcurWith = "Investigation Officer"
                    ucInformalAndReinvestigationFindings_v2.RenameFindingText("Recommend Formal LOD Investigation", "Conduct Formal Investigation")
                    ucInformalAndReinvestigationFindings_v2.FindingsLableText = "Substituted Findings:"
                    ucInformalAndReinvestigationFindings_v2.ReasonsLabelText = "Reasons:"
                    ucInformalAndReinvestigationFindings_v2.ShownOnText = "(Shown on Form 261)"
                    ucInformalAndReinvestigationFindings_v2.ShowRemarks = False
                    ucInformalAndReinvestigationFindings_v2.ShowFormText = True
                    ucInformalAndReinvestigationFindings_v2.SetDecisionToggle = True
                    ucInformalAndReinvestigationFindings_v2.FindingsRequired = False

                    lblAAEditPanelHeader_v2.Text = "Formal Appointing Authority Review"
                    lblIORow_v2.Text = "D"
                    lblIOCompletedByRow_v2.Text = "E"
                    lblIOInstructionsRow_v2.Text = "F"
                    lblIOPOCRow_v2.Text = "G"

                    If (ucInformalAndReinvestigationFindings_v2.Decision.Equals("Y")) Then
                        ucInformalAndReinvestigationFindings_v2.EnableFindings = False
                        ucInformalAndReinvestigationFindings_v2.FindingsIndex = -1
                    Else
                        ucInformalAndReinvestigationFindings_v2.EnableFindings = True
                    End If

                    If (ucInformalAndReinvestigationFindings_v2.Decision.Equals("N") AndAlso ucInformalAndReinvestigationFindings_v2.Findings = LodFinding.RecommendFormalInvestigation) Then
                        pnlIOControls_v2.Visible = True
                        lblIOLabel_v2.CssClass = "labelRequired"
                    Else
                        pnlIOControls_v2.Visible = False
                        lblIOLabel_v2.CssClass = String.Empty
                    End If
                End If
            ElseIf (LOD.CurrentStatusCode = LodStatusCode.NotifyFormalInvestigator) Then
                SigCheckFormal_v2.Visible = False
                AAEditPanel_v2.Visible = False
                AAReviewPanel_v2.Visible = True
                FormalPanel_v2.Visible = False
                DisplayReadOnly_v2()
                'DisplayAppointedIOReviewControls()
            End If

        End Sub

        Private Sub DisplayReadOnly_v2()

            SigCheckInformal_v2.VerifySignature(RefId)
            SigCheckFormal_v2.VerifySignature(RefId, PersonnelTypes.FORMAL_APP_AUTH)

            Dim isRLod As Boolean = LookupService.GetIsReinvestigationLod(LOD.Id)

            AAEditPanel_v2.Visible = False
            AAReviewPanel_v2.Visible = True

            If (isRLod = False) Then
                LoadAppointingAuthorityReadOnly_v2()
            Else
                trIOFindings_v2.Visible = True
                trDecision_v2.Visible = True
                lblAAReviewPanelHeader_v2.Text = "Formal Appointing Authority Review"
                lblAARevFindings_v2.Text = "Substituted Findings:"
                lblAARevRemarks_v2.Text = "Reasons:<br />(Shown on Form261)"
                lblAARevFindingsRow_v2.Text = "B"
                lblAARevRemarksRow_v2.Text = "C"
                lblAARevIORow_v2.Text = "D"
                lblAARevIOCompletedByRow_v2.Text = "E"
                lblAARevIOInstructionsRow_v2.Text = "F"
                lblAARevIOPOCRow_v2.Text = "G"
                LoadRLodFormalAppointingAuthorityReadOnly_v2()
            End If


            If (Session("GroupId") = UserGroups.WingCommander) OrElse (Session("AccessScope") = AccessScope.Compo) Then
                If (LOD.Formal AndAlso isRLod = False) Then
                    LoadFormalAppointingAuthority_v2(True)
                    FormalPanel_v2.Visible = True
                Else
                    FormalPanel_v2.Visible = False
                End If
            Else
                FormalPanel_v2.Visible = False
            End If

            DisplayAppointedIOReviewControls_v2()

        End Sub

        Private Sub DisplayAppointedIOReviewControls_v2()
            pnlAppointedIOReview_v2.Visible = True

            If (Session("GroupId") = UserGroups.LOD_PM AndAlso LOD.CurrentStatusCode = LodStatusCode.NotifyFormalInvestigator) Then
                lblAppointedIOEDIPI_v2.Visible = False
                txtAppointedIOEDIPI_v2.Visible = True
                txtAppointedIOUnit_v2.Visible = True
                btnChangeUnit_v2.Visible = True
                lblAppointedIOUnit_v2.Visible = False
                lblAppointedIOIATraining_v2.Visible = False
                txtAppointedIOIATraining_v2.Visible = True
                txtAppointedIOEmail_v2.Visible = True
                RequiredFieldValidator1_v2.Visible = True

                LoadAppointedIO_v2(False)
            Else
                lblAppointedIOEDIPI_v2.Visible = True
                txtAppointedIOEDIPI_v2.Visible = False
                txtAppointedIOUnit_v2.Visible = False
                btnChangeUnit_v2.Visible = False
                lblAppointedIOUnit_v2.Visible = True
                lblAppointedIOIATraining_v2.Visible = True
                txtAppointedIOIATraining_v2.Visible = False
                lblAppointedIOEmail_v2.Visible = True
                RequiredFieldValidator1_v2.Visible = False

                LoadAppointedIO_v2(True)
            End If
        End Sub

        Public Sub LoadAppointingAuthority_v2()

            Dim finding As LineOfDutyFindings
            finding = LOD.FindByType(PersonnelTypes.APPOINT_AUTH)


            ucInformalAndReinvestigationFindings_v2.LoadFindingOptionsByWorkflow(LOD.Workflow, UserGroups.WingCommander, optionalInformalFindings, False)
            ucInformalAndReinvestigationFindings_v2.RenameFindingText("Recommend Formal LOD Investigation", "Recommend Formal Investigation")

            If (finding IsNot Nothing) Then

                If (finding.Finding.HasValue AndAlso finding.Finding > 0) Then
                    ucInformalAndReinvestigationFindings_v2.Findings = finding.Finding.Value
                End If

                If (Not String.IsNullOrEmpty(finding.Explanation)) Then
                    ucInformalAndReinvestigationFindings_v2.Remarks = Server.HtmlDecode(finding.Explanation)
                End If

                If (Not String.IsNullOrEmpty(LOD.InstructionsToInvestigator)) Then
                    txtIOInstruction_v2.Text = Server.HtmlDecode(LOD.InstructionsToInvestigator)
                End If

                If (Not String.IsNullOrEmpty(LOD.AppointingAuthorityPoc)) Then
                    txtAAPOC_v2.Text = Server.HtmlDecode(LOD.AppointingAuthorityPoc)
                End If

                If (LOD.IoCompletionDate.HasValue) Then
                    txtIOCompletionDate_v2.Text = Server.HtmlDecode(LOD.IoCompletionDate.Value.ToString(DATE_FORMAT))
                End If

            End If
        End Sub

        Public Sub LoadAppointingAuthorityReadOnly_v2()

            Dim finding As LineOfDutyFindings
            finding = LOD.FindByType(PersonnelTypes.APPOINT_AUTH)
            Dim formal As Boolean = False

            If (finding IsNot Nothing) Then

                If (finding.Finding.HasValue AndAlso finding.Finding > 0) Then
                    For Each f As FindingsLookUp In New LookupDao().GetWorkflowFindings(LOD.Workflow, UserGroups.WingCommander)
                        If (f.Id = finding.Finding.Value) Then
                            FindingsLabel_v2.Text = f.Description
                        End If
                    Next

                    If (finding.Finding.Value = LodFinding.NotInLineOfDutyNotDueToOwnMisconduct OrElse
                        finding.Finding.Value = LodFinding.RecommendFormalInvestigation) Then
                        formal = True
                    End If

                    If (formal) Then

                        Dim scope As AccessScope = Session("AccessScope")
                        Dim group As Integer = CInt(Session("GroupId"))

                        If (scope = AccessScope.Compo) OrElse (group = UserGroups.WingCommander) OrElse UserHasPermission("exePostCompletion") Then

                            IoPanel_v2.Visible = True

                            CurrentIoLabel_v2.Text = LodService.AppointedIONameAndRank(LOD)

                            If (Not String.IsNullOrEmpty(LOD.InstructionsToInvestigator)) Then
                                InstructionsLabel_v2.Text = LOD.InstructionsToInvestigator.Replace(vbCrLf, "<br />") + "<br /><br /><br />"
                            End If

                            If (Not String.IsNullOrEmpty(LOD.AppointingAuthorityPoc)) Then
                                PocLabel_v2.Text = LOD.AppointingAuthorityPoc.Replace(vbCrLf, "<br />") + "<br /><br /><br />"
                            End If

                            If (LOD.IoCompletionDate.HasValue) Then
                                CompleteByLabel_v2.Text = LOD.IoCompletionDate.Value.ToString(DATE_FORMAT)
                            End If

                        Else
                            'this is a unit/wing level, hide the IO info
                            IoPanel_v2.Visible = False
                        End If

                    End If

                End If

                If (Not String.IsNullOrEmpty(finding.Explanation)) Then
                    RemarksLabel_v2.Text = finding.Explanation.Replace(vbCrLf, "<br />") + "<br /><br /><br />"
                End If

            End If
        End Sub

        Public Sub LoadFormalAppointingAuthority_v2(ByVal isReadOnly As Boolean)
            ' Load findings for a Formal LOD that is not a Reinvestigation LOD
            LoadPreviousFindings_v2(False)
            FormalFindings_v2.SetReadOnly = isReadOnly

            Dim cFinding As LineOfDutyFindings
            cFinding = LOD.FindByType(PersonnelTypes.FORMAL_APP_AUTH)

            FormalFindings_v2.LoadFindingOptionsByWorkflow(LOD.Workflow, UserGroups.WingCommander, optionalFindings, False)

            If Not (cFinding Is Nothing) Then
                If cFinding.DecisionYN <> String.Empty Then FormalFindings_v2.Decision = cFinding.DecisionYN
                If (Not cFinding.Finding Is Nothing AndAlso cFinding.Finding > 0) Then
                    FormalFindings_v2.Findings = cFinding.Finding.Value
                End If
                FormalFindings_v2.Remarks = cFinding.Explanation
                FormalFindings_v2.FormFindingsText = cFinding.FindingsText
            End If

        End Sub

        Public Sub LoadRLodFormalAppointingAuthority_v2()
            ' Load findings for a Formal LOD that is a Reinvestigation LOD
            Dim finding As LineOfDutyFindings
            finding = LOD.FindByType(PersonnelTypes.FORMAL_APP_AUTH)

            ucInformalAndReinvestigationFindings_v2.LoadFindingOptionsByWorkflow(LOD.Workflow, UserGroups.WingCommander, optionalInformalFindings, False)

            If (finding IsNot Nothing) Then
                If finding.DecisionYN <> String.Empty Then
                    ucInformalAndReinvestigationFindings_v2.Decision = finding.DecisionYN
                End If

                If (finding.Finding.HasValue AndAlso finding.Finding > 0) Then
                    ucInformalAndReinvestigationFindings_v2.Findings = finding.Finding.Value
                End If

                If (Not String.IsNullOrEmpty(finding.FindingsText)) Then
                    ucInformalAndReinvestigationFindings_v2.FormFindingsText = Server.HtmlDecode(finding.FindingsText)
                End If

                If (Not String.IsNullOrEmpty(LOD.InstructionsToInvestigator)) Then
                    txtIOInstruction_v2.Text = Server.HtmlDecode(LOD.InstructionsToInvestigator)
                End If

                If (Not String.IsNullOrEmpty(LOD.AppointingAuthorityPoc)) Then
                    txtAAPOC_v2.Text = Server.HtmlDecode(LOD.AppointingAuthorityPoc)
                End If

                If (LOD.IoCompletionDate.HasValue) Then
                    txtIOCompletionDate_v2.Text = Server.HtmlDecode(LOD.IoCompletionDate.Value.ToString(DATE_FORMAT))
                End If

            End If

            Dim ioFinding As LineOfDutyFindings
            ioFinding = LOD.FindByType(PersonnelTypes.IO)

            If (ioFinding Is Nothing) Then
                ' There is no IO findings associated with this reinvestigation LOD which means a new investigation was never conducted;
                ' therefore, it makes no sense to allow the user to decide if they concur or don't concur with the IO...
                ucInformalAndReinvestigationFindings_v2.Decision = "N"
                ucInformalAndReinvestigationFindings_v2.EnableDecision = False

                ' Also force the user to select the option to conduct an investigation...
                ucInformalAndReinvestigationFindings_v2.DisableAllFindingsExcept("Recommend Formal LOD Investigation")
                ucInformalAndReinvestigationFindings_v2.Findings = LodFinding.RecommendFormalInvestigation
            End If
        End Sub

        Public Sub LoadRLodFormalAppointingAuthorityReadOnly_v2()
            Dim ioFinding As LineOfDutyFindings
            ioFinding = LOD.FindByType(PersonnelTypes.IO)

            If (ioFinding IsNot Nothing) Then
                If (ioFinding.Finding.HasValue AndAlso ioFinding.Finding > 0) Then
                    For Each f As FindingsLookUp In New LookupDao().GetWorkflowFindings(LOD.Workflow, UserGroups.InvestigatingOfficer)
                        If (f.Id = ioFinding.Finding.Value) Then
                            lblAARevIOFindings_v2.Text = f.Description
                        End If
                    Next
                Else
                    lblAARevIOFindings_v2.Text = "Not Found"
                End If
            Else
                lblAARevIOFindings_v2.Text = "Not Found"
            End If

            Dim finding As LineOfDutyFindings
            finding = LOD.FindByType(PersonnelTypes.FORMAL_APP_AUTH)

            If (finding IsNot Nothing) Then

                If (Not String.IsNullOrEmpty(finding.DecisionYN)) Then
                    If (finding.DecisionYN = "Y") Then
                        lblAARevDecision_v2.Text = "Concur with the action of Investigation Officer"
                    Else
                        lblAARevDecision_v2.Text = "Non Concur with the action of Investigation Officer"
                    End If
                End If

                If (finding.Finding.HasValue AndAlso finding.Finding > 0) Then
                    For Each f As FindingsLookUp In New LookupDao().GetWorkflowFindings(LOD.Workflow, UserGroups.WingCommander)
                        If (f.Id = finding.Finding.Value) Then
                            FindingsLabel_v2.Text = f.Description
                        End If
                    Next
                End If

                If (Not String.IsNullOrEmpty(finding.FindingsText)) Then
                    RemarksLabel_v2.Text = finding.FindingsText.Replace(vbCrLf, "<br />") + "<br /><br /><br />"
                End If

                Dim scope As AccessScope = Session("AccessScope")
                Dim group As Integer = CInt(Session("GroupId"))

                If (scope = AccessScope.Compo) OrElse (group = UserGroups.WingCommander) OrElse UserHasPermission("exePostCompletion") Then
                    IoPanel_v2.Visible = True

                    CurrentIoLabel_v2.Text = LodService.AppointedIONameAndRank(LOD)

                    If (Not String.IsNullOrEmpty(LOD.InstructionsToInvestigator)) Then
                        InstructionsLabel_v2.Text = LOD.InstructionsToInvestigator.Replace(vbCrLf, "<br />") + "<br /><br /><br />"
                    End If

                    If (Not String.IsNullOrEmpty(LOD.AppointingAuthorityPoc)) Then
                        PocLabel_v2.Text = LOD.AppointingAuthorityPoc.Replace(vbCrLf, "<br />") + "<br /><br /><br />"
                    End If

                    If (LOD.IoCompletionDate.HasValue) Then
                        CompleteByLabel_v2.Text = LOD.IoCompletionDate.Value.ToString(DATE_FORMAT)
                    End If
                Else
                    'this is a unit/wing level, hide the IO info
                    IoPanel_v2.Visible = False
                End If
            End If

        End Sub

        Public Sub LoadPreviousFindings_v2(ByVal isRLod As Boolean)
            Dim findingsText As String = ""

            Dim ioFinding As LineOfDutyFindings
            ioFinding = LOD.FindByType(PersonnelTypes.IO)

            If (ioFinding IsNot Nothing AndAlso ioFinding.Finding.HasValue) Then
                For Each f As FindingsLookUp In New LookupDao().GetWorkflowFindings(LOD.Workflow, UserGroups.InvestigatingOfficer)
                    If (f.Id = ioFinding.Finding.Value) Then
                        findingsText = f.Description
                    End If
                Next
            End If

            ' Check if this LOD is a Reinvestigation LOD...
            If (Not isRLod) Then
                ' This is not a reinvestigation LOD; therefore, load the previous findings into the formal findings control...
                FormalFindings_v2.PrevFindingsLableText = "IO Findings:"
                FormalFindings_v2.PrevFindingsText = "Not found"

                If Not (ioFinding Is Nothing) Then

                    If (ioFinding.DecisionYN = "Y") Then
                        If (Not ioFinding.Finding Is Nothing AndAlso ioFinding.Finding > 0) Then
                            FormalFindings_v2.PrevFindingsText = findingsText
                        End If
                    Else
                        If (Not ioFinding.Finding Is Nothing AndAlso ioFinding.Finding > 0) Then
                            FormalFindings_v2.PrevFindingsText = findingsText
                        End If
                    End If

                End If
            Else
                ' This is a reinvestigation LOD; therefore, load previous findings into the controls on the edit panel...
                ucInformalAndReinvestigationFindings_v2.ShowPrevFindings = True
                ucInformalAndReinvestigationFindings_v2.PrevFindingsLableText = "IO Findings:"
                ucInformalAndReinvestigationFindings_v2.PrevFindingsText = "Not found"

                If (Not (ioFinding Is Nothing)) Then
                    If (Not ioFinding.Finding Is Nothing AndAlso ioFinding.Finding > 0) Then
                        ucInformalAndReinvestigationFindings_v2.PrevFindingsText = findingsText
                    End If
                End If
            End If
        End Sub

        Private Sub LoadAppointedIO_v2(isReadOnly As Boolean)
            If (LOD.AppointedIO IsNot Nothing) Then
                LoadAppointedIO_v2(LOD.AppointedIO, isReadOnly)
            ElseIf (Not String.IsNullOrEmpty(LOD.IoSsn)) Then
                LoadAppointedIO_v2(LookupService.GetServiceMemberBySSN(LOD.IoSsn), isReadOnly)
            Else
                Exit Sub
            End If
        End Sub

        Private Sub LoadAppointedIO_v2(appointedIo As AppUser, isReadOnly As Boolean)
            If (appointedIo Is Nothing) Then
                Exit Sub
            End If

            lblAppointedIOName_v2.Text = appointedIo.FullName
            lblAppointedIORank_v2.Text = appointedIo.Rank.Rank
            lblAppointedIOComponent_v2.Text = GetCompoString(appointedIo.Component)

            If (isReadOnly) Then
                lblAppointedIOEDIPI_v2.Text = appointedIo.EDIPIN
                lblAppointedIOEmail_v2.Text = appointedIo.Email
            Else
                txtAppointedIOEDIPI_v2.Text = appointedIo.EDIPIN
                txtAppointedIOEmail_v2.Text = appointedIo.Email
            End If

            If (appointedIo.AccountExpiration.HasValue) Then
                If (isReadOnly) Then
                    lblAppointedIOIATraining_v2.Text = appointedIo.AccountExpiration.Value.ToString(DATE_FORMAT)
                Else
                    txtAppointedIOIATraining_v2.Text = appointedIo.AccountExpiration.Value.ToString(DATE_FORMAT)
                End If
            End If

            If (appointedIo.Unit IsNot Nothing) Then
                If (isReadOnly) Then
                    lblAppointedIOUnit_v2.Text = appointedIo.Unit.NameAndPasCode
                Else
                    txtAppointedIOUnit_v2.Text = appointedIo.Unit.NameAndPasCode
                    lblNewUnitID_v2.Text = appointedIo.Unit.Id.ToString()
                End If
            End If
        End Sub

        Private Sub LoadAppointedIO_v2(memberData As ServiceMember, isReadOnly As Boolean)
            If (memberData Is Nothing) Then
                Exit Sub
            End If

            lblAppointedIOName_v2.Text = memberData.FullName
            lblAppointedIORank_v2.Text = memberData.Rank.Rank
            lblAppointedIOComponent_v2.Text = GetCompoString(memberData.Component)

            If (memberData.Unit IsNot Nothing) Then
                If (isReadOnly) Then
                    lblAppointedIOUnit_v2.Text = memberData.Unit.NameAndPasCode
                Else
                    txtAppointedIOUnit_v2.Text = memberData.Unit.NameAndPasCode
                    lblNewUnitID_v2.Text = memberData.Unit.Id.ToString()
                End If
            End If
        End Sub

        Private Sub SaveFindings_v2()

            'Save the decision data 

            If (Not UserCanEdit) Then
                Exit Sub
            End If

            If (LOD.CurrentStatusCode = LodStatusCode.AppointingAutorityReview) Then
                SaveAppointingAuthority_v2()
            ElseIf (LOD.CurrentStatusCode = LodStatusCode.FormalActionByAppointingAuthority) Then
                ' Check if this LOD is a Reinvestigation LOD...
                If (LookupService.GetIsReinvestigationLod(LOD.Id) = False) Then
                    SaveFormalAction_v2()
                Else
                    SaveRLodFormalAction_v2()
                End If
            ElseIf (LOD.CurrentStatusCode = LodStatusCode.NotifyFormalInvestigator) Then
                SaveAppointedIO_v2()
            End If

        End Sub

        Private Sub SaveAppointingAuthority_v2()

            'save the findings
            Dim finding As LineOfDutyFindings = CreateAppointingAuthorityFinding(PersonnelTypes.APPOINT_AUTH)

            finding.Explanation = ucInformalAndReinvestigationFindings_v2.Remarks.Trim()

            Dim formal As Boolean = False

            If (ucInformalAndReinvestigationFindings_v2.Findings Is Nothing) Then
                finding.Finding = Nothing
            Else
                finding.Finding = ucInformalAndReinvestigationFindings_v2.Findings
            End If

            If (LOD.Workflow = AFRCWorkflows.LOD) Then
                If (finding.Finding = LodFinding.RecommendFormalInvestigation OrElse
                    finding.Finding = LodFinding.NotInLineOfDutyNotDueToOwnMisconduct) Then
                    formal = True
                End If
            Else

                If (finding.Finding = LodFinding.RecommendFormalInvestigation) Then
                    formal = True
                End If

                If (finding.Finding = LodFinding.NotInLineOfDutyNotDueToOwnMisconduct AndAlso rblSubFindings.SelectedIndex >= 0) Then

                    lod_v2.NILODsubFinding = rblSubFindings.SelectedValue

                    If (lod_v2.NILODsubFinding.HasValue AndAlso lod_v2.NILODsubFinding = NILODSubFindings.AbsentwithoutAuthority) Then
                        formal = True
                    End If
                End If
            End If

            If (formal) Then
                LOD.AppointingAuthorityPoc = Server.HtmlEncode(txtAAPOC_v2.Text.Trim)
                LOD.InstructionsToInvestigator = Server.HtmlEncode(txtIOInstruction_v2.Text.Trim)
                If (CheckDate(txtIOCompletionDate_v2)) Then
                    LOD.IoCompletionDate = Server.HtmlEncode(DateTime.Parse(txtIOCompletionDate_v2.Text))
                End If

                'save the IO data
                LOD.IoSsn = Nothing
                Dim selectedValue As String = String.Empty
                If (cbAssignIo_v2.SelectedIndex > 0) Then
                    Dim ioList As List(Of LookUpItem) = LookupService.GetIOList(CInt(HttpContext.Current.Session("UserId")), CByte(HttpContext.Current.Session("ReportView")), LOD.MemberRank.Id)
                    selectedValue = ioList.Item(cbAssignIo_v2.SelectedIndex - 1).Value.Trim()
                    If (selectedValue <> String.Empty) Then
                        If InStr(selectedValue, "ssn") > 0 Then
                            LOD.IoSsn = selectedValue.Split(":")(1)
                            LOD.AppointedIO = Nothing
                        End If
                        If InStr(selectedValue, "uid") > 0 Then
                            LOD.AppointedIO = UserService.GetById(CInt(selectedValue.Split(":")(1)))
                            LOD.IoSsn = Nothing
                        End If
                    End If

                End If
            End If

            ucInformalAndReinvestigationFindings_v2.Remarks = finding.Explanation
            LOD.SetFindingByType(finding)
        End Sub

        Private Sub SaveFormalAction_v2()

            Dim finding As LineOfDutyFindings = CreateAppointingAuthorityFinding(PersonnelTypes.FORMAL_APP_AUTH)

            finding.DecisionYN = FormalFindings_v2.Decision

            If (FormalFindings_v2.Decision <> "Y" AndAlso Not FormalFindings_v2.Findings Is Nothing) Then
                finding.Finding = FormalFindings_v2.Findings
            End If

            finding.Explanation = FormalFindings_v2.Remarks            ' findings control automatically html encodes properties
            finding.FindingsText = FormalFindings_v2.FormFindingsText

            LOD.SetFindingByType(finding)

        End Sub

        Private Sub SaveRLodFormalAction_v2()
            Dim isReconductFormalIO As Boolean = False
            Dim finding As LineOfDutyFindings = CreateAppointingAuthorityFinding(PersonnelTypes.FORMAL_APP_AUTH)

            finding.FindingsText = ucInformalAndReinvestigationFindings_v2.FormFindingsText

            finding.DecisionYN = ucInformalAndReinvestigationFindings_v2.Decision

            If (ucInformalAndReinvestigationFindings_v2.Decision <> String.Empty AndAlso ucInformalAndReinvestigationFindings_v2.Decision <> "Y") Then
                ' Save selected finding...
                If (ucInformalAndReinvestigationFindings_v2.Findings Is Nothing) Then
                    finding.Finding = Nothing
                Else
                    finding.Finding = ucInformalAndReinvestigationFindings_v2.Findings
                End If

                If (finding.Finding = LodFinding.RecommendFormalInvestigation) Then
                    isReconductFormalIO = True
                End If
            End If


            ' Save IO information if necessary....
            If (isReconductFormalIO) Then
                LOD.AppointingAuthorityPoc = Server.HtmlEncode(txtAAPOC_v2.Text.Trim)
                LOD.InstructionsToInvestigator = Server.HtmlEncode(txtIOInstruction_v2.Text.Trim)

                If (CheckDate(txtIOCompletionDate_v2)) Then
                    LOD.IoCompletionDate = Server.HtmlEncode(DateTime.Parse(txtIOCompletionDate_v2.Text))
                End If

                LOD.IoSsn = Nothing
                Dim selectedValue As String = String.Empty

                If (cbAssignIo_v2.SelectedIndex > 0) Then
                    Dim ioList As List(Of LookUpItem) = LookupService.GetIOList(CInt(HttpContext.Current.Session("UserId")), CByte(HttpContext.Current.Session("ReportView")), LOD.MemberRank.Id)

                    selectedValue = ioList.Item(cbAssignIo_v2.SelectedIndex - 1).Value.Trim()

                    If (selectedValue <> String.Empty) Then
                        If InStr(selectedValue, "ssn") > 0 Then
                            LOD.IoSsn = selectedValue.Split(":")(1)
                            LOD.AppointedIO = Nothing
                        End If
                        If InStr(selectedValue, "uid") > 0 Then
                            LOD.AppointedIO = UserService.GetById(CInt(selectedValue.Split(":")(1)))
                            LOD.IoSsn = Nothing
                        End If
                    End If
                End If
            Else
                'LOD.AppointingAuthorityPoc = String.Empty
                'LOD.InstructionsToInvestigator = String.Empty
                'LOD.IoCompletionDate = Nothing
                'LOD.IoSsn = Nothing
                'LOD.AppointedIO = Nothing
            End If

            LOD.SetFindingByType(finding)
        End Sub

        Private Sub SaveAppointedIO_v2()
            ValidateAppointedIoInput_v2()

            If (AreTheirAppointedIoInputErrors_v2()) Then
                trAppointedIOError_v2.Visible = True
                Exit Sub
            End If

            Dim wasModified As Boolean = False

            If (LOD.AppointedIO IsNot Nothing) Then
                wasModified = UpdateExistingAccountWithIoRole(LOD.AppointedIO, False)
                AssignSubmittedDataToInvestigatingOfficer_v2(LOD.AppointedIO, wasModified)
                UpdateLodInvestigationAssignedIo(LOD.AppointedIO)
            ElseIf (Not String.IsNullOrEmpty(LOD.IoSsn)) Then
                Dim ioUser As AppUser = FindExistingUserAccount(txtAppointedIOEDIPI_v2.Text.Trim, LOD.IoSsn)

                If (ioUser IsNot Nothing) Then
                    wasModified = UpdateExistingAccountWithIoRole(ioUser, True)
                Else
                    ioUser = CreateNewInvestigatingOfficerUserAccount(bllAppointedIOErrors_v2)
                    wasModified = True
                End If

                If (ioUser IsNot Nothing)
                    AssignSubmittedDataToInvestigatingOfficer_v2(ioUser, wasModified)
                    UserDao.SaveOrUpdate(ioUser)
                    LOD.AppointedIO = ioUser
                    UpdateLodInvestigationAssignedIo(ioUser)
                End If
            End If

            trAppointedIOError_v2.Visible = AreTheirAppointedIoInputErrors_v2()
        End Sub

        Private Sub ValidateAppointedIoInput_v2()
            bllAppointedIOErrors_v2.Items.Clear()

            If (AreAnyAppointedIOControlsEmpty_v2()) Then
                bllAppointedIOErrors_v2.Items.Add("The fields displayed in RED must have values entered into them.")
            End If

            If (Not String.IsNullOrEmpty(txtAppointedIOEDIPI_v2.Text) AndAlso Not DoesEDIPIMeetFormatRequirements(txtAppointedIOEDIPI_v2.Text)) Then
                bllAppointedIOErrors_v2.Items.Add(txtAppointedIOEDIPI_v2.Text.Trim + " does not meet the format requirements for an EDIPI number.")
            End If

            If (Not String.IsNullOrEmpty(txtAppointedIOEmail_v2.Text) AndAlso Not DoesEmailMeetFormatRequirements(txtAppointedIOEmail_v2.Text)) Then
                bllAppointedIOErrors_v2.Items.Add(txtAppointedIOEmail_v2.Text.Trim + " does not meet the format requirements for an email address.")
            End If

            If (Not String.IsNullOrEmpty(txtAppointedIOIATraining_v2.Text) AndAlso Date.Parse(txtAppointedIOIATraining_v2.Text.Trim) <= Date.Now) Then
                bllAppointedIOErrors_v2.Items.Add("IA Training date must be greater than the current date.")
            End If
        End Sub

        Private Function AreAnyAppointedIOControlsEmpty_v2() As Boolean
            Dim result As Boolean = False

            If (Not ValidateWebServerTextControl(txtAppointedIOEDIPI_v2, txtAppointedIOEDIPI_v2.Text)) Then
                result = True
            End If

            If (Not ValidateWebServerTextControl(txtAppointedIOIATraining_v2, txtAppointedIOIATraining_v2.Text)) Then
                result = True
            End If

            If (Not ValidateWebServerTextControl(txtAppointedIOUnit_v2, lblNewUnitID_v2.Text)) Then
                result = True
            End If

            If (Not ValidateWebServerTextControl(txtAppointedIOEmail_v2, txtAppointedIOEmail_v2.Text)) Then
                result = True
            End If

            Return result
        End Function

        Private Function AreTheirAppointedIoInputErrors_v2() As Boolean
            Return (bllAppointedIOErrors_v2.Items.Count > 0)
        End Function

        Private Sub AssignSubmittedDataToInvestigatingOfficer_v2(ioUser As AppUser, hasAlreadyBeenModified As Boolean)
            Dim wasModified As Boolean = hasAlreadyBeenModified

            If (String.IsNullOrEmpty(ioUser.EDIPIN) OrElse Not ioUser.EDIPIN.Equals(txtAppointedIOEDIPI_v2.Text.Trim)) Then
                If (UserDao.IsEDIPINAvailable(txtAppointedIOEDIPI_v2.Text.Trim)) Then
                    ioUser.EDIPIN = Server.HtmlEncode(txtAppointedIOEDIPI_v2.Text.Trim)
                    wasModified = True
                Else
                    bllAppointedIOErrors_v2.Items.Add("A user account with an EDIPI number of " + txtAppointedIOEDIPI_v2.Text.Trim + " already exists in the system.")
                    txtAppointedIOEDIPI_v2.Text = ioUser.EDIPIN
                End If
            End If

            If (Not ioUser.AccountExpiration.Value.Date.Equals(Date.Parse(txtAppointedIOIATraining_v2.Text.Trim))) Then
                ioUser.AccountExpiration = Server.HtmlEncode(Date.Parse(txtAppointedIOIATraining_v2.Text.Trim))
                wasModified = True
            End If

            If (Not ioUser.Email.Equals(txtAppointedIOEmail_v2.Text.Trim())) Then
                ioUser.Email = txtAppointedIOEmail_v2.Text.Trim()
                wasModified = True
            End If

            Dim newUnit As Unit = UnitDao.FindById(CInt(lblNewUnitID_v2.Text.Trim))
            If (newUnit IsNot Nothing AndAlso newUnit.Id <> ioUser.Unit.Id) Then
                ioUser.Unit = newUnit
                wasModified = True
                txtAppointedIOUnit_v2.Text = newUnit.NameAndPasCode
            End If

            If (wasModified) Then
                ioUser.ModifiedDate = DateTime.Now
                ioUser.ModifiedBy = UserDao.GetById(CInt(HttpContext.Current.Session("UserId")))
            End If
        End Sub

        Protected Sub rblSubFindings_SelectedIndexChanged(sender As Object, e As EventArgs) Handles rblSubFindings.SelectedIndexChanged
            If (rblSubFindings.SelectedValue = NILODSubFindings.AbsentwithoutAuthority) Then
                pnlIOControls_v2.Visible = True
                lblIOLabel_v2.CssClass = "labelRequired"
            Else
                pnlIOControls_v2.Visible = False
                lblIOLabel_v2.CssClass = String.Empty
            End If
        End Sub
#End Region

#Region "Shared Methods"
        Private Function CreateAppointingAuthorityFinding(ByVal pType As PersonnelTypes)
            Dim finding As LineOfDutyFindings

            finding = CreateFinding(LOD.Id)
            finding.Name = UserService.CurrentUser.FirstLastName
            finding.Pascode = UserService.CurrentUser.Unit.PasCode
            finding.PType = pType

            Return finding
        End Function

        Private Function ValidateWebServerTextControl(ByVal control As WebControl, ByVal text As String) As Boolean
            Dim requiredCssClass As String = "fieldRequired"

            If (String.IsNullOrEmpty(text.Trim)) Then
                control.CssClass = control.CssClass + " " + requiredCssClass
                Return False
            End If

            If (control.CssClass.Contains(requiredCssClass)) Then
                control.CssClass = control.CssClass.Remove(control.CssClass.IndexOf(requiredCssClass), requiredCssClass.Length).Trim
            End If

            Return True
        End Function

        Private Function DoesEDIPIMeetFormatRequirements(edipi As String) As Boolean
            Dim regex As Regex = New Regex("^[0-9a-zA-Z]+$", RegexOptions.IgnoreCase Or RegexOptions.CultureInvariant Or RegexOptions.IgnorePatternWhitespace Or RegexOptions.Compiled)

            Return regex.IsMatch(edipi.Trim)
        End Function

        Private Function DoesEmailMeetFormatRequirements(email As String) As Boolean
            Dim regex As Regex = New Regex("\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*", RegexOptions.IgnoreCase Or RegexOptions.CultureInvariant Or RegexOptions.IgnorePatternWhitespace Or RegexOptions.Compiled)

            Return regex.IsMatch(email.Trim)
        End Function

        Private Function CreateNewInvestigatingOfficerUserAccount(bllErrors As BulletedList) As AppUser
            Dim memberData As ServiceMember = LookupService.GetServiceMemberBySSN(LOD.IoSsn)

            If (memberData Is Nothing) Then
                bllErrors.Items.Add("Failed to find member data with SSN = " & LOD.IoSsn & ". IO user account not created.")
                Return Nothing
            End If

            Dim ioUser As AppUser = CreateNewUserAccountFromServiceMember(memberData)

            If (ioUser Is Nothing) Then
                bllErrors.Items.Add("Failed to create new IO account from service member record with SSN = " & LOD.IoSsn & ".")
                Return Nothing
            End If

            AddInvestigatingOfficerRoleToUser(ioUser, True)
            LogNewAccountCreatedForUser(ioUser)

            Return ioUser
        End Function

        Private Function CreateNewUserAccountFromServiceMember(memberData As ServiceMember)
            If (memberData Is Nothing) Then
                Return Nothing
            End If

            Dim newUser As New AppUser()

            newUser.Import(memberData)
            newUser.Status = AccessStatus.Approved
            newUser.Username = UserDao.GetUserName(newUser.FirstName, newUser.LastName)
            newUser.ReceiveEmail = True
            newUser.ReceiveReminderEmail = True
            newUser.Component = "6"
            newUser.AccountExpiration = DateTime.Now.Date
            newUser.UnitView = 1
            newUser.Email = String.Empty

            Return newUser
        End Function

        Private Function UpdateExistingAccountWithIoRole(userAccount As AppUser, shouldUpdateStatus As Boolean) As Boolean
            Dim wasModified As Boolean = False

            If (shouldUpdateStatus)
                LogAccountStatusChangeForUser(userAccount)
                userAccount.Status = AccessStatus.Approved
                wasModified = True
            End If
            
            If (Not DoesUserHaveAssignedRoleForUserGroup(userAccount, UserGroups.InvestigatingOfficer)) Then
                AddInvestigatingOfficerRoleToUser(userAccount, False)
                LogNewRoleAddedToUser(userAccount, UserGroups.InvestigatingOfficer)
                wasModified = True
            End If

            Return wasModified
        End Function

        Private Sub AddInvestigatingOfficerRoleToUser(selectedUser As AppUser, setAsActiveRole As Boolean)
            Dim role As New UserRole()
            role.Status = AccessStatus.Approved
            role.Group = New UserGroup(UserGroups.InvestigatingOfficer)
            role.Active = True
            role.User = selectedUser

            selectedUser.AllRoles.Add(role)

            If (setAsActiveRole) Then
                selectedUser.CurrentRole = role
            End If
        End Sub

        Private Sub UpdateLodInvestigationAssignedIo(ioUser As AppUser)
            If (ioUser Is Nothing OrElse LOD Is Nothing OrElse LOD.LODInvestigation Is Nothing)
                Exit Sub
            End If

            LOD.LODInvestigation.IoUserId = ioUser.Id
            LOD.LODInvestigation.ModifiedBy = SESSION_USER_ID
            LOD.LODInvestigation.ModifiedDate = DateTime.UtcNow
        End Sub

        Private Sub LogNewAccountCreatedForUser(newUser As AppUser)
            Dim changes As New ChangeSet()
            changes.Add("User Account", "Account Created", "", "")
            changes.Save(LogManager.LogAction(ModuleType.System, UserAction.CreatedNewAccount, newUser.Id))
        End Sub

        Private Sub LogNewRoleAddedToUser(selectedUser As AppUser, roleUserGroup As UserGroups)
            Dim changes As New ChangeSet()
            changes.Add("Role Added", "Added Role", "", UserGroupDao.GetById(roleUserGroup).Description)
            changes.Save(LogManager.LogAction(ModuleType.System, UserAction.CreatedUserRole, selectedUser.Id))
        End Sub

        Private Sub LogAccountStatusChangeForUser(selectedUser As AppUser)
            Dim changes As New ChangeSet()
            changes.Add("Account Status", "Status", selectedUser.StatusDescription, "Approved")
            changes.Save(LogManager.LogAction(ModuleType.System, UserAction.ModifyAccountStatus, selectedUser.Id))
        End Sub

        Private Function DoesUserHaveAssignedRoleForUserGroup(selectedUser As AppUser, roleUserGroup As UserGroups) As Boolean
            For Each role As UserRole In selectedUser.AllRoles
                If (role.Group.Id = roleUserGroup) Then
                    Return True
                End If
            Next

            Return False
        End Function

        Private Function FindExistingUserAccount(edipin As String, memberSsn As String) As AppUser
            Dim userAccount As AppUser

            userAccount = UserDao.GetByEDIPIN(edipin)

            If (userAccount Is Nothing) Then
                userAccount = UserDao.GetByServiceMemberSSN(memberSsn)
            End If

            Return userAccount
        End Function
#End Region
    End Class
End Namespace
