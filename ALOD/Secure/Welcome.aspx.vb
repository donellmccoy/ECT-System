Imports ALOD.Core.Domain.Documents
Imports ALOD.Core.Domain.Users
Imports ALOD.Core.Domain.WelcomePageBanner
Imports ALOD.Core.Domain.Workflow
Imports ALOD.Core.Interfaces.DAOInterfaces
Imports ALOD.Core.Utils
Imports ALOD.Data
Imports ALOD.Data.Services
Imports ALODWebUtility.Common
Imports ALODWebUtility.Common.SessionInfo

Namespace Web

    Partial Class Secure_Welcome
        Inherits System.Web.UI.Page

        Private _daoFactory As IDaoFactory
        Private _docDao As IDocumentDao
        Private _hyperLinkDao As IHyperLinkDao
        Private _permissionDao As IALODPermissionDao
        Private _viewHelpDocPerm As ALODPermission
        Dim dataservice As New DataService

        Protected ReadOnly Property DaoFactory As IDaoFactory
            Get
                If (_daoFactory Is Nothing) Then
                    _daoFactory = New NHibernateDaoFactory()
                End If

                Return _daoFactory
            End Get
        End Property

        Protected ReadOnly Property DocumentDao() As IDocumentDao
            Get
                If (_docDao Is Nothing) Then
                    _docDao = New SRXDocumentStore(CStr(HttpContext.Current.Session("UserName")))
                End If

                Return _docDao
            End Get
        End Property

        Protected ReadOnly Property DocumentGroupId() As Integer
            Get
                Return PermissionDao.GetDocGroupIdByPermId(ViewHelpDocPermission.Id)
            End Get
        End Property

        Protected ReadOnly Property HyperLinkDao As IHyperLinkDao
            Get
                If (_hyperLinkDao Is Nothing) Then
                    _hyperLinkDao = DaoFactory.GetHyperLinkDao()
                End If

                Return _hyperLinkDao
            End Get
        End Property

        Protected ReadOnly Property PermissionDao() As IALODPermissionDao
            Get
                If (_permissionDao Is Nothing) Then
                    _permissionDao = DaoFactory.GetPermissionDao()

                End If

                Return _permissionDao
            End Get
        End Property

        Protected ReadOnly Property ViewHelpDocPermission() As ALODPermission
            Get
                If (_viewHelpDocPerm Is Nothing) Then
                    Dim lst As IList(Of ALODPermission) = PermissionDao.GetAll().ToList()
                    If (lst.Count > 0) Then
                        _viewHelpDocPerm = (From p In lst Where p.Name = "viewHelpDocs" Select p).First()
                    End If
                End If

                Return _viewHelpDocPerm
            End Get
        End Property

        Protected Sub Audit_LinkButton_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles Audit_LinkButton.Click
            Response.Redirect("lod/MyLodAudit.aspx")
        End Sub

        Protected Sub btnSearch_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnLodSearch.Click, btnSCSearch.Click
            Dim btn As Button = CType(sender, Button)
            Dim url As String

            Dim btnID As String
            Dim txtSearch As String
            txtSearch = Server.HtmlEncode(Trim(txtLodSearch.Text))
            If String.IsNullOrEmpty(txtSearch) Then
                txtSearch = Server.HtmlEncode(Trim(txtSCSearch.Text))
                If String.IsNullOrEmpty(txtSearch) Then
                    btnID = "" 'No Search term provided
                Else
                    btnID = "btnSCSearch"
                End If
            Else
                If String.IsNullOrEmpty(txtSCSearch.Text) Then
                    btnID = "btnLodSearch"
                Else
                    'Both Search Terms provided - go with the button click
                    btnID = btn.ID
                End If
            End If

            Select Case btnID
                Case "btnLodSearch"
                    url = "~/secure/lod/search.aspx?data=" + Server.HtmlEncode(txtLodSearch.Text)
                Case "btnSCSearch"
                    'url = "~/secure/SpecialCases/search.aspx?data=" + Server.HtmlEncode(txtSCSearch.Text)
                    url = GetSpecialCaseSearchURL()
                Case Else
                    url = ""
            End Select

            If Not String.IsNullOrEmpty(url) Then
                Response.Redirect(url)
            End If
        End Sub

        Protected Function CanSpecialCasePanelBeVisible() As Boolean
            If (UserHasPermission("mySCs") OrElse
                UserHasPermission("scSearch") OrElse
                DoesUserHaveASpecialCaseSearchPermission() OrElse
                UserHasPermission("PHCreate") OrElse
                Session("GroupId") = UserGroups.SystemAdministrator OrElse
                Session("GroupId") = UserGroups.MedicalTechnician) Then
                Return True
            End If

            Return False
        End Function

        Protected Function CanSpecialCaseSearchLinkBeVisible() As Boolean
            If (UserHasPermission("scSearch") OrElse
                UserHasPermission("scSearchMT") OrElse
                Session("GroupId") = UserGroups.SystemAdministrator) Then
                Return True
            End If

            Return False
        End Function

        Protected Sub Consult_LinkButton_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles Consult_LinkButton.Click
            Response.Redirect("lod/MyLodConsult.aspx")
        End Sub

        Protected Sub dataMessages_Selecting(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.ObjectDataSourceSelectingEventArgs) Handles dataMessages.Selecting
            e.InputParameters("userId") = CInt(HttpContext.Current.Session("UserId"))
            e.InputParameters("groupId") = CInt(HttpContext.Current.Session("GroupId"))
        End Sub

        Protected Sub DisplayMessages()
            Dim messages As New MessageList()
            messages = messages.RetrieveMessages(SESSION_USER_ID, SESSION_GROUP_ID, False)

            Dim hyperlinks As IList(Of HyperLink) = HyperLinkDao.GetAll().ToList()
            Dim documents As IList(Of Document) = DocumentDao.GetDocumentsByGroupId(DocumentGroupId)

            If (messages.Count > 0) Then
                For Each i In messages
                    i.Message = Replace((Server.HtmlEncode(i.Message)), vbLf, "<br/>")

                    For Each l In hyperlinks
                        If (l.Type.Name.Equals("Document")) Then
                            i.Message = Regex.Replace(i.Message, "{" + l.Name.ToUpper() + "}", BuildDocumentLink(l, documents), RegexOptions.IgnoreCase)
                        ElseIf (l.Type.Name.Equals("Website")) Then
                            i.Message = Regex.Replace(i.Message, "{" + l.Name.ToUpper() + "}", BuildWebsiteLink(l), RegexOptions.IgnoreCase)
                        End If

                    Next
                Next
                rptMessages.DataSource = messages
                rptMessages.DataBind()
                NoMessagesPanel.Visible = False
            Else
                NoMessagesPanel.Visible = True
            End If
        End Sub

        Protected Sub InitAGRLinks()
            If (UserHasPermission("myAGRCert")) Then
                SpecialCaseLink_AGRCert.Visible = True
                SpecialCaseCount_AGRCert.Text = LodService.GetSpecialCasesByModuleCount(ModuleType.SpecCaseAGR, SESSION_USER_ID)
            ElseIf (UserHasPermission("AGRCertSearch")) Then
                AGRCertSearch.Visible = True
            End If
        End Sub

        Protected Sub InitAppealLinks()
            If (UserHasPermission("myAP") AndAlso Not IsUserSystemAdmin()) Then
                MyAppealLink.Visible = True
                LoadInBox(PendingAppealCount, "GetPendingAppealCount")
            ElseIf (UserHasPermission("APSearch") AndAlso Not IsUserSystemAdmin()) Then
                AppealLink.Visible = True
            End If

            If (UserHasPermission("APCompletion")) Then
                CompletedAppealLink.Visible = True
                LoadInBox(CompletedAppealCount, "GetCompletedAppealCount")
            End If
        End Sub

        Protected Sub InitBCMRLinks()
            If (UserHasPermission("myBcmr")) Then
                SpecialCaseLink_BCMR.Visible = True
                SpecialCaseCount_BCMR.Text = LodService.GetSpecialCasesByModuleCount(ModuleType.SpecCaseBCMR, SESSION_USER_ID)
            ElseIf (UserHasPermission("BCMRSearch")) Then
                BCMR_search.Visible = True
            End If
        End Sub

        Protected Sub InitBMTLinks()
            If (UserHasPermission("myBmt")) Then
                SpecialCaseLink_BMT.Visible = True
                SpecialCaseCount_BMT.Text = LodService.GetSpecialCasesByModuleCount(ModuleType.SpecCaseBMT, SESSION_USER_ID)
            ElseIf (UserHasPermission("BMTSearch")) Then
                BMT_search.Visible = True
            End If
        End Sub

        Protected Sub InitCILinks()
            If (UserHasPermission("myCi")) Then
                SpecialCaseLink_Congress.Visible = True
                SpecialCaseCount_Congress.Text = LodService.GetSpecialCasesByModuleCount(ModuleType.SpecCaseCongress, SESSION_USER_ID)
            ElseIf (UserHasPermission("CISearch")) Then
                CI_search.Visible = True
            End If
        End Sub

        Protected Sub InitCMASLinks()
            If (UserHasPermission("myCmas")) Then
                SpecialCaseLink_CMAS.Visible = True
                SpecialCaseCount_CMAS.Text = LodService.GetSpecialCasesByModuleCount(ModuleType.SpecCaseCMAS, SESSION_USER_ID)
            ElseIf (UserHasPermission("CMASSearch")) Then
                CMAS_search.Visible = True
            End If
        End Sub

        Protected Sub InitDWLinks()
            If (UserHasPermission("myDW")) Then
                SpecialCaseLink_DW.Visible = True
                SpecialCaseCount_DW.Text = LodService.GetSpecialCasesByModuleCount(ModuleType.SpecCaseDW, SESSION_USER_ID)
            ElseIf (UserHasPermission("DWSearch")) Then
                DW_search.Visible = True
            End If
        End Sub

        Protected Sub InitINCAPLinks()
            If (UserHasPermission("myIncap")) Then
                SpecialCaseLink_Incap.Visible = True
                SpecialCaseCount_Incap.Text = LodService.GetSpecialCasesByModuleCount(ModuleType.SpecCaseIncap, SESSION_USER_ID)
            ElseIf (UserHasPermission("INCAPSearch")) Then
                Incap_search.Visible = True
            End If
        End Sub

        Protected Sub InitIRILOLinks()
            If (UserHasPermission("myFTs")) Then
                SpecialCaseLink_FastTrack.Visible = True
                SpecialCaseCount_FastTrack.Text = LodService.GetSpecialCaseNotHoldingCount(ModuleType.SpecCaseFT, SESSION_USER_ID)
            ElseIf (UserHasPermission("FTSearch")) Then
                IRILO_search.Visible = True
            End If
        End Sub

        Protected Sub InitLODLinks()
            If (UserHasPermission("myLod")) Then
                Select Case SESSION_GROUP_ID
                    Case UserGroups.MedicalTechnician, UserGroups.MedicalOfficer, UserGroups.UnitCommander, UserGroups.WingJudgeAdvocate,
                 UserGroups.WingCommander, UserGroups.BoardMedical, UserGroups.BoardTechnician, UserGroups.BoardLegal, UserGroups.BoardAdministrator,
                 UserGroups.BoardApprovalAuthority, UserGroups.WingSarc
                        MyLodLink.Visible = True
                        LoadInBox(PendingLodCount, "GetPendingLodCount")
                    Case UserGroups.LOD_PM, UserGroups.InvestigatingOfficer
                        MyLodLink.Visible = True
                        LoadInBox(PendingLodCount, "GetPendingIOLodCount")
                    Case Else
                        MyLodLink.Visible = True
                        LoadInBox(PendingLodCount, "GetPendingLodCount")
                End Select
            End If

            If (UserHasPermission("lodCreate") OrElse UserHasPermission(PERMISSION_CREATE_SARC)) Then
                NewLodLink.Visible = True
            End If

            If (UserHasPermission("exePostCompletion")) Then
                CompletedLodLink.Visible = True
                LoadInBox(CompletedLodCount, "GetCompletedLodCount")
            End If

            If (SESSION_GROUP_ID = 122) Then 'Appointing Authority (P)
                Consult_Link.Visible = True
                Consult_LinkButton.Text = "LODs awaiting ARC SME Consult"
                Consult_Count.Text = dataservice.GetCaseCount(332, SESSION_COMPO) 'ws_id
            End If

            If (SESSION_GROUP_ID = 9) Then 'Board Medical
                InitLODLinks(SESSION_GROUP_ID, 332, 333)
                SpecialCaseLink_SCAC_Count.Text = dataservice.GetInConsultCaseCount() 'ws_id
            End If

            If (SESSION_GROUP_ID = 8) Then 'Board Legal
                InitLODLinks(SESSION_GROUP_ID, 332, 334)
            End If

            If (SESSION_GROUP_ID = 11) Then 'Approving Authority
                InitLODLinks(SESSION_GROUP_ID, 335)
            End If

            If (SESSION_GROUP_ID = 97) Then 'Board Administrator
                InitLODLinks(SESSION_GROUP_ID, 330)
            End If

            If (UserHasPermission("lodSearch")) Then
                SearchLodLink.Visible = True
            End If
        End Sub

        Protected Sub InitLODLinks(groupId As Int16, wsId As Int16)
            If (wsId = 330 Or wsId = 335) Then
                Audit_Link.Visible = True
                Audit_Count.Text = dataservice.GetCaseCount(wsId, SESSION_COMPO)
            Else
                Consult_Link.Visible = True
                Consult_Count.Text = dataservice.GetCaseCount(wsId, SESSION_COMPO)
            End If

        End Sub

        Protected Sub InitLODLinks(groupId As Int16, cWsId As Int16, aWsid As Int16)
            Consult_Link.Visible = True
            Audit_Link.Visible = True
            Consult_Count.Text = dataservice.GetCaseCount(cWsId, SESSION_COMPO)
            SpecialCaseLink_SCAC_Count.Text = dataservice.GetInConsultCaseCount()
            Audit_Count.Text = dataservice.GetCaseCount(aWsid, SESSION_COMPO)
        End Sub

        Protected Sub InitMEBLinks()
            If (UserHasPermission("myMeb")) Then
                SpecialCaseLink_MEB.Visible = True
                SpecialCaseCount_MEB.Text = LodService.GetSpecialCasesByModuleCount(ModuleType.SpecCaseMEB, SESSION_USER_ID)
            ElseIf (UserHasPermission("MEBSearch")) Then
                MB_search.Visible = True
            End If
        End Sub

        Protected Sub InitMHLinks()
            If (UserHasPermission("myMH")) Then
                SpecialCaseLink_MH.Visible = True
                SpecialCaseCount_MH.Text = LodService.GetSpecialCasesByModuleCount(ModuleType.SpecCaseMH, SESSION_USER_ID)
            ElseIf (UserHasPermission("MHSearch")) Then
                MH_search.Visible = True
            End If
        End Sub

        Protected Sub InitMMSOLinks()
            If ((UserHasPermission("mySCs") Or UserHasPermission("MMSOSearch")) AndAlso Not IsUserSystemAdmin()) Then
                'Removing the count/link from the SysAdmin role/gorup.
                SpecialCaseLink_MMSO.Visible = True
                LoadInBox(SpecialCaseCount, "GetSpecialCasesCount")
                If UserHasPermission("MMSOSearch") And UserHasPermission("mySCs") Then
                    SpecCaseTitle.Text = "Other/Special Cases/MMSO"
                ElseIf UserHasPermission("MMSOSearch") Then
                    SpecCaseTitle.Text = "MMSO Cases"
                Else
                    SpecCaseTitle.Text = "Other/Special Cases"
                End If
            End If
        End Sub

        Protected Sub InitMOLinks()
            If (UserHasPermission("myMO")) Then
                SpecialCaseLink_MO.Visible = True
                SpecialCaseCount_MO.Text = LodService.GetSpecialCasesByModuleCount(ModuleType.SpecCaseMO, SESSION_USER_ID)
            ElseIf (UserHasPermission("MOSearch")) Then
                MO_search.Visible = True
            End If
        End Sub

        Protected Sub InitNELinks()
            If (UserHasPermission("myNE")) Then
                SpecialCaseLink_NE.Visible = True
                SpecialCaseCount_NE.Text = LodService.GetSpecialCasesByModuleCount(ModuleType.SpecCaseNE, SESSION_USER_ID)
            ElseIf (UserHasPermission("NESearch")) Then
                NE_search.Visible = True
            End If
        End Sub

        Protected Sub InitPEPPLinks()
            If (UserHasPermission("myPEPP")) Then
                SpecialCaseLink_PEPP.Visible = True
                SpecialCaseCount_PEPP.Text = LodService.GetSpecialCaseNotHoldingCount(ModuleType.SpecCasePEPP, SESSION_USER_ID)
            ElseIf (UserHasPermission("PEPPSearch")) Then
                PE_search.Visible = True
            End If
        End Sub

        Protected Sub InitPHLinks()
            If (UserHasPermission("myPH")) Then
                SpecialCaseLink_PH.Visible = True
                SpecialCaseCount_PH.Text = LodService.GetSpecialCasesByModuleCount(ModuleType.SpecCasePH, SESSION_USER_ID)
            ElseIf (UserHasPermission("PHSearch")) Then
                ' Currently no link exists for the PH Search page...
            End If

            If (UserHasPermission("PHCreate")) Then
                SpecialCasLink_StartNewPH.Visible = True
            End If
        End Sub

        Protected Sub InitPSCDLinks()
            If (UserHasPermission("myPSCDs")) Then
                SpecialCaseLink_PSCD.Visible = True
                SpecialCaseCount_PSCD.Text = LodService.GetSpecialCasesByModuleCount(ModuleType.SpecCasePSCD, SESSION_USER_ID)
            ElseIf (UserHasPermission("PSCDSearch")) Then
                PSCD_search.Visible = True
            End If
        End Sub

        Protected Sub InitPWLinks()
            If (UserHasPermission("myPwaiver")) Then
                SpecialCaseLink_PWaivers.Visible = True
                SpecialCaseCount_PWaivers.Text = LodService.GetSpecialCasesByModuleCount(ModuleType.SpecCasePW, SESSION_USER_ID)
            ElseIf (UserHasPermission("PWSearch")) Then
                PW_search.Visible = True
            End If
        End Sub

        Protected Sub InitRestrictedSARCLinks()
            If (UserHasPermission(PERMISSION_EDIT_SARC) AndAlso Not IsUserSystemAdmin()) Then
                trRestrictedSARCsLink.Visible = True
                LoadInBox(lblRestrictedSARCsCount, "GetRestrictedSARCsCaseCount")
            End If

            If (UserHasPermission(PERMISSION_SARC_POSTPROCESSING) AndAlso Not IsUserSystemAdmin()) Then
                trComletedRestrictedSARCsLink.Visible = True
                LoadInBox(lblCompletedRestrictedSARCsCount, "GetRestrictedSARCsPostCompletionCaseCount")
            End If
        End Sub

        Protected Sub InitRRLinks()
            If (UserHasPermission("reinvestigate") AndAlso Not IsUserSystemAdmin()) Then
                ReinvestigateLink.Visible = True
                LoadInBox(ReinvestigatedLodCount, "GetReinvestigationRequestCount")
            ElseIf (UserHasPermission("reinvestigateSearch") AndAlso Not IsUserSystemAdmin()) Then
                ReinvestigationLink.Visible = True
            End If
        End Sub

        Protected Sub InitRSLinks()
            If (UserHasPermission("myRS")) Then
                SpecialCaseLink_RS.Visible = True
                SpecialCaseCount_RS.Text = LodService.GetSpecialCaseNotHoldingCount(ModuleType.SpecCaseRS, SESSION_USER_ID)
            ElseIf (UserHasPermission("RSSearch")) Then
                RS_search.Visible = True
            End If
        End Sub

        Protected Sub InitRWLinks()
            If (UserHasPermission("myRW")) Then
                SpecialCaseLink_RW.Visible = True
                SpecialCaseCount_RW.Text = LodService.GetSpecialCasesByModuleCount(ModuleType.SpecCaseRW, SESSION_USER_ID)
            ElseIf (UserHasPermission("RWSearch")) Then
                RW_search.Visible = True
            End If
        End Sub

        Protected Sub InitSARCAppealLinks()
            If (UserHasPermission("myRSARCAppeal") AndAlso Not IsUserSystemAdmin()) Then
                trSARCAppealsLink.Visible = True
                LoadInBox(lblSARCAppealsCount, "GetSARCAppealsCaseCount")
            End If

            If (UserHasPermission("RSARCAppealPostCompletion") AndAlso Not IsUserSystemAdmin()) Then
                trComletedSARCAppealsLink.Visible = True
                LoadInBox(lblCompletedSARCAppealssCount, "GetSARCAppealsPostCompletionCaseCount")
            End If
        End Sub

        Protected Sub InitWWDLinks()
            If (UserHasPermission("myWWDs")) Then
                SpecialCaseLink_WWD.Visible = True
                SpecialCaseCount_WWD.Text = LodService.GetSpecialCasesByModuleCount(ModuleType.SpecCaseWWD, SESSION_USER_ID)
            ElseIf (UserHasPermission("WWDSearch")) Then
                WD_search.Visible = True
            End If
        End Sub

        Protected Function IsUserSystemAdmin() As Boolean
            Return (Session("GroupId") = UserGroups.SystemAdministrator)
        End Function

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
            If (Not IsPostBack) Then
                Dim scm As ScriptManager = ScriptManager.GetCurrent(Page)
                Dim svr As ServiceReference = New ServiceReference("~/Secure/utility/DataService.asmx")
                scm.Services.Add(svr)
                'clear any pending locks this user might have
                Dim dao As ICaseLockDao = New ALOD.Data.NHibernateDaoFactory().GetCaseLockDao()
                dao.ClearLocksForUser(SESSION_USER_ID)
                InitControls()
                DisplayMessages()

                If Session(SESSIONKEY_COMPO) = "5" Then
                    SpecialCaseLink_WWD_hl.InnerText = "Non Duty Disability Evaluation System (NDDES)"
                    WD_search_hl.InnerText = "Non Duty Disability Evaluation System (NDDES)"
                End If
                'clean up any unneeded session vars
                CleanSession()
            End If
        End Sub

        Protected Function UserHasAppealPermissions() As Boolean
            If (UserHasPermission("myAP")) Then
                Return True
            End If

            Return False
        End Function

        Protected Function UserHasLODPermissions() As Boolean
            If (UserHasPermission("lodCreate") OrElse UserHasPermission("myLod") OrElse
                UserHasPermission("MMSOSearch") OrElse UserHasPermission("lodSearch")) Then
                Return True
            End If

            Return False
        End Function

        Protected Function UserHasRRPermissions() As Boolean
            If (UserHasPermission("reinvestigate") OrElse UserHasPermission("reinvestigateSearch")) Then
                Return True
            End If

            Return False
        End Function

        Protected Function UserHasSARCPermissions() As Boolean
            If (UserHasPermission(PERMISSION_CREATE_SARC) OrElse UserHasPermission(PERMISSION_EDIT_SARC) OrElse
                UserHasPermission(PERMISSION_SARC_POSTPROCESSING) OrElse UserHasPermission(PERMISSION_VIEW_SARC_CASES)) Then
                Return True
            End If

            Return False
        End Function

        Private Function BuildDocumentLink(ByVal l As HyperLink, ByVal documents As IList(Of Document)) As String
            If (l Is Nothing) Then
                Return String.Empty
            End If

            If (Not IsValidDocumentLink(l, documents)) Then
                Return "[DOCUMENT NOT FOUND]"
            End If

            Dim logMessage = "Help Document with ID = " + l.Value
            Return "<a onclick=""viewDoc('Shared/DocumentViewer.aspx?docId=" + l.Value + "&amp;modId=1&amp;doc=" + logMessage.Replace(" ", "+") + "'); return false;"" href=""#"">" + l.DisplayText + "</a>"
        End Function

        Private Function BuildWebsiteLink(ByVal l As HyperLink) As String
            If (l Is Nothing) Then
                Return String.Empty
            End If

            Return "<a href=""" + l.Value + """ target=""_blank"">" + l.DisplayText + "</a>"
        End Function

        Private Function DoesUserHaveASpecialCaseSearchPermission() As Boolean
            If (UserHasPermission("BCMRSearch") OrElse
                UserHasPermission("BMTSearch") OrElse
                UserHasPermission("CMASSearch") OrElse
                UserHasPermission("CISearch") OrElse
                UserHasPermission("DWSearch") OrElse
                UserHasPermission("FTSearch") OrElse
                UserHasPermission("INCAPSearch") OrElse
                UserHasPermission("MEBSearch") OrElse
                UserHasPermission("MHSearch") OrElse
                UserHasPermission("MOSearch") OrElse
                UserHasPermission("NESearch") OrElse
                UserHasPermission("PWSearch") OrElse
                UserHasPermission("PEPPSearch") OrElse
                UserHasPermission("PHSearch") OrElse
                UserHasPermission("RSSearch") OrElse
                UserHasPermission("RWSearch") OrElse
                UserHasPermission("AGRCertSearch") OrElse
                UserHasPermission("WWDSearch") OrElse
                UserHasPermission("PSCDSearch")) Then
                Return True
            End If

            Return False
        End Function

        Private Function GetSpecialCaseSearchURL() As String
            If (SESSION_GROUP_ID = UserGroups.UnitPH OrElse SESSION_GROUP_ID = UserGroups.HQAFRCDPH) Then
                Return ("~/secure/SC_PH/Search.aspx?data=" + Server.HtmlEncode(txtSCSearch.Text))
            Else
                Return ("~/secure/SpecialCases/search.aspx?data=" + Server.HtmlEncode(txtSCSearch.Text))
            End If
        End Function

        Private Sub InitAdminPanel()
            If (UserHasPermission("usersModify") Or UserHasPermission("usersApprove")) Then
                AdminPanel.Visible = True
                LoadInBox(PendingRolesLabel, "GetPendingRoleRequestCount")
                LoadInBox(PendingUserCount, "GetPendingUserCount")
            End If
        End Sub

        Private Sub InitControls()
            SetInputFormatRestriction(Page, txtLodSearch, FormatRestriction.AlphaNumeric, PERMITTED_SPECIAL_CHAR_INPUT)
            SetInputFormatRestriction(Page, txtSCSearch, FormatRestriction.AlphaNumeric, PERMITTED_SPECIAL_CHAR_INPUT)

            InitWelcomePageBanner()
            InitMessagePanels()
            InitLODPanel()
            InitSpecialCasePanel()
            InitAdminPanel()

            SetDefaultButton(txtLodSearch, btnLodSearch)
        End Sub

        Private Sub InitLODPanel()
            If (UserHasLODPermissions() OrElse UserHasAppealPermissions() OrElse UserHasRRPermissions() OrElse UserHasSARCPermissions()) Then
                LodPanel.Visible = True

                InitLODLinks()
                InitMMSOLinks()
                InitAppealLinks()
                InitRRLinks()
                InitRestrictedSARCLinks()
                InitSARCAppealLinks()
            End If
        End Sub

        Private Sub InitMessagePanels()
            'do we have an error message to display?
            If (GetErrorMessage().Length > 0) Then
                ErrorMessageLabel.Text = GetErrorMessage()
                ErrorPanel.Visible = True
            Else
                ErrorPanel.Visible = False
            End If

            'do we have a feedback message to display?
            If (GetFeedbackMessage().Length > 0) Then
                FeedbackMessageLabel.Text = GetFeedbackMessage()
                FeedbackPanel.Visible = True
            Else
                FeedbackPanel.Visible = False
            End If
        End Sub

        Private Sub InitSCAwaitingConsult()
            If (UserHasPermission("SCAwaitingConsult")) Then
                Try
                    SpecialCaseLink_SCAC_Panel.Visible = True
                Catch ex As Exception
                    SpecialCaseLink_SCAC_Count.Text = 0
                    SpecialCaseLink_SCAC_Panel.Visible = True
                End Try

            End If
        End Sub

        Private Sub InitSpecialCasePanel()
            If (CanSpecialCasePanelBeVisible()) Then
                SpecCasePanel.Visible = True
            Else
                Exit Sub
            End If

            If (CanSpecialCaseSearchLinkBeVisible()) Then
                SearchSCLink.Visible = True
            End If

            If (SESSION_GROUP_ID = UserGroups.SystemAdministrator) Then
                Exit Sub
            End If

            SpecialCaseLink_SCAC_Panel.Visible = False

            InitSCAwaitingConsult()
            InitBCMRLinks()
            InitBMTLinks()
            InitCMASLinks()
            InitCILinks()
            InitDWLinks()
            InitIRILOLinks()
            InitINCAPLinks()
            InitMEBLinks()
            InitMHLinks()
            InitMOLinks()
            InitNELinks()
            InitPWLinks()
            InitPEPPLinks()
            InitPHLinks()
            InitRSLinks()
            InitRWLinks()
            InitWWDLinks()
            InitAGRLinks()
            InitPSCDLinks()
        End Sub

        Private Sub InitWelcomePageBanner()
            ' Should the welcome banner be displayed?
            Dim welcomePageBanner As WelcomePageBanner = New WelcomePageBanner(DaoFactory.GetKeyValDao(), DaoFactory.GetHyperLinkDao(), DocumentDao, DocumentGroupId)

            If (welcomePageBanner IsNot Nothing AndAlso welcomePageBanner.Enabled) Then
                pnlWelcomeBanner.Visible = True
                lblWelcomeBannerMessage.Text = Server.HtmlDecode(welcomePageBanner.PopulatedBannerText)
            Else
                pnlWelcomeBanner.Visible = False
            End If
        End Sub

        Private Function IsValidDocumentLink(ByVal l As HyperLink, ByVal documents As IList(Of Document)) As Boolean

            Dim documentId As Long = Long.Parse(l.Value)

            If (documentId = 0) Then
                Return False
            End If

            Dim linkedDocument As Document = (From m In documents Select m Where m.Id = documentId).FirstOrDefault

            If (linkedDocument Is Nothing OrElse linkedDocument.DocStatus = DocumentStatus.Deleted) Then
                Return False
            End If

            Return True
        End Function

        Private Sub LoadInBox(ByVal target As Control, ByVal methodName As String)
            Dim imgScript As String = "showBusy('" + target.ClientID + "');"
            ClientScript.RegisterStartupScript(Page.GetType(), target.ClientID, imgScript + "ALOD.DataService." + methodName + "(onInBoxLookUpComplete, onInBoxLookUpFailed,'" + target.ClientID + "');", True)
        End Sub

    End Class

End Namespace