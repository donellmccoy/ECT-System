Imports ALOD.Core.Domain.Common
Imports ALOD.Core.Domain.Documents
Imports ALOD.Core.Domain.Modules.SpecialCases
Imports ALOD.Core.Domain.Workflow
Imports ALOD.Core.Interfaces.DAOInterfaces
Imports ALOD.Data
Imports ALOD.Web.UserControls

Namespace Web.Special_Case.PSCD

    Partial Class Secure_PSCD_WWD_Documents
        Inherits System.Web.UI.Page

#Region "Fields"

        Private _associatedCaseDao As IAssociatedCaseDao
        Private _DocumentDao As IDocumentDao
        Private _factory As IDaoFactory
        Private _memoSource As MemoDao2
        Private _sc As SC_WWD
        Private _scAccess As Dictionary(Of String, ALOD.Core.Domain.Workflow.PageAccessType)
        Private _specCaseDao As ISpecialCaseDAO
        Private _special As SpecialCase
        Private _Specialdao As ISpecialCaseDAO

#End Region

#Region "Properties"

        ReadOnly Property DaoFactory() As IDaoFactory
            Get
                If (_factory Is Nothing) Then
                    _factory = New NHibernateDaoFactory()
                End If

                Return _factory
            End Get
        End Property

        ReadOnly Property DocumentDao() As IDocumentDao
            Get
                If (_DocumentDao Is Nothing) Then
                    _DocumentDao = New SRXDocumentStore(CStr(HttpContext.Current.Session("UserName")))
                End If

                Return _DocumentDao
            End Get
        End Property

        ReadOnly Property ModuleType() As ModuleType
            Get
                Return ModuleType.SpecCaseWWD
            End Get
        End Property

        ReadOnly Property SCDao() As ISpecialCaseDAO
            Get
                If (_Specialdao Is Nothing) Then
                    _Specialdao = DaoFactory.GetSpecialCaseDAO()
                End If

                Return _Specialdao
            End Get
        End Property

        Public ReadOnly Property Navigator() As TabNavigator
            Get
                Return Master.Navigator
            End Get
        End Property

        Protected ReadOnly Property Associated() As IAssociatedCaseDao
            Get
                If (_associatedCaseDao Is Nothing) Then
                    _associatedCaseDao = DaoFactory.GetAssociatedCaseDao()
                End If
                Return _associatedCaseDao
            End Get
        End Property

        Protected ReadOnly Property RequestId() As Integer
            Get
                Return CInt(Request.QueryString("refId"))
            End Get
        End Property

        Protected ReadOnly Property SC() As SC_WWD
            Get
                If (_sc Is Nothing) Then
                    _sc = SCDao.GetById(GetAssociatedRefId())
                End If

                Return _sc
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

        Protected ReadOnly Property SpecCase() As SpecialCase
            Get
                If (_special Is Nothing) Then
                    _special = SCDao.GetById(GetAssociatedRefId(), False)

                End If
                Return _special
            End Get

        End Property

        Protected ReadOnly Property SpecCaseDao As ISpecialCaseDAO
            Get
                If (_specCaseDao Is Nothing) Then
                    _specCaseDao = DaoFactory.GetSpecialCaseDAO()
                End If

                Return _specCaseDao
            End Get
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

        Protected Sub Page_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
            AddHandler Me.Master.TabClick, AddressOf TabButtonClicked
        End Sub

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

            'AddStyleSheet(Page, "~/Styles/Documents.css")

            'If (Not IsPostBack) Then

            '    If Session(SESSIONKEY_COMPO) = "5" Then
            '        Memorandum_Title.Text = "1 - Form 4"
            '    End If

            '    GetDocuments()
            '    InitMemoCreation()
            '    UpdateMemoList()

            'End If

            Dim HasAdminLod As Boolean

            If (SpecCase.HasAdminLOD Is Nothing) Then
                HasAdminLod = False
            Else
                HasAdminLod = SpecCase.HasAdminLOD
            End If

            If (Associated.GetAssociatedCasesLOD(SpecCase.Id, SpecCase.Workflow).Count = 0) Then
                ucWDDocuments.Initialize(Me, Navigator, New WorkflowDocument(SC, 2, DaoFactory))
            Else
                'ucWDDocuments.Initialize(Me, instance.Id, HasAdminLod, Navigator)

            End If

        End Sub

        Private Function GetAssociatedRefId() As Integer
            Dim sc = SpecCaseDao.GetById(RequestId)
            Dim cases As IList(Of AssociatedCase) = New List(Of AssociatedCase)
            cases = cases.Concat(Associated.GetAssociatedCasesSC(sc.Id, sc.Workflow)).ToList()
            Return cases.Item(0).associated_RefId
        End Function

        Private Sub TabButtonClicked(ByRef sender As Object, ByRef e As TabNavigationEventArgs)

            'If (e.ButtonType = NavigatorButtonType.Save OrElse
            '    e.ButtonType = NavigatorButtonType.NavigatedAway OrElse
            '    e.ButtonType = NavigatorButtonType.NextStep OrElse
            '    e.ButtonType = NavigatorButtonType.PreviousStep) Then
            '    SaveFindings()
            'End If

        End Sub

        '        Private Sub GetDocuments()

        '            'make sure we have a groupId
        '            If (SpecCase.DocumentGroupId Is Nothing OrElse SpecCase.DocumentGroupId = 0) Then
        '                SpecCase.CreateDocumentGroup(DocumentDao)
        '                SCDao.SaveOrUpdate(SpecCase)  'Save the new Document Group ID with the Special Case
        '                SCDao.CommitChanges()
        '            End If

        '            SpecCase.ProcessDocuments(DaoFactory)
        '            Documents.Initialize(Me, Navigator, New WorkflowDocument(SC, 2, DaoFactory))

        '        End Sub
        '        Private Sub InitMemoCreation()
        '            Dim associatedRefId As Integer = GetAssociatedRefId()
        '            'get the memo templates
        '            Dim templates = From m In MemoStore.GetAllTemplates()
        '                            Where m.Component = 6 And m.ModuleType = 9
        '                            Select m

        '            templates = (From x In templates Select x Where x.Component = 6 And x.ModuleType = 9)

        '            ' get memo list
        '            Dim memoList As IList(Of Memorandum2)
        '            memoList = (From l In MemoStore.GetByRefnModule(associatedRefId, 13)
        '                        Where l.Deleted = False And l.Template.Id = 57 'SC.GetActiveMemoTemplateId()
        '                        Select l).ToList

        '            ' get disqual memos
        '            Dim memoDisqual As IList(Of Memorandum2) = Nothing
        '            If (SC.GetActiveBoardMedicalFinding() = 2) Then
        '                memoDisqual = (From l In MemoStore.GetByRefnModule(associatedRefId, ModuleType)
        '                               Where l.Deleted = False And l.Template.Id = MemoType.MEB_Disqualification_Letter Select l).ToList

        '            ElseIf (SC.GetActiveBoardMedicalFinding() = 0) Then
        '                memoDisqual = (From l In MemoStore.GetByRefnModule(associatedRefId, ModuleType)
        '                               Where l.Deleted = False And l.Template.Id = MemoType.WWD_Unit_Disqualification_Letter Select l).ToList
        '            End If

        '            ' get RTD memos; if SAF disposition
        '            Dim memoListRTD As IList(Of Memorandum2) = Nothing
        '            If (SC.Status = SpecCaseWWDWorkStatus.FinalReviewSAF OrElse SC.Status = SpecCaseWWDWorkStatus.FinalReviewIPEB OrElse SC.Status = SpecCaseWWDWorkStatus.FinalReviewFPEB) Then
        '                memoListRTD = (From l In MemoStore.GetByRefnModule(associatedRefId, ModuleType)
        '                               Where l.Deleted = False And (l.Template.Id = MemoType.WWD_4_2_RTD Or
        '                                                            l.Template.Id = MemoType.WWD_4_Non_ALC_C Or
        '                                                            l.Template.Id = MemoType.WWD_RTD_SAF Or
        '                                                            l.Template.Id = MemoType.WWD_RTD_SG)
        '                               Select l).ToList
        '            End If

        '            Dim templateId As Integer = 0
        '            Dim templateId2 As Integer = 0
        '            Dim templateId3 As Integer = 0
        '            Dim modType As Integer = ModuleType

        '            Dim template As MemoTemplate = Nothing
        '            Dim template2 As MemoType = Nothing
        '            Dim template3 As MemoTemplate = Nothing 'this will replace 'template' value if case goes to SAF disposition and RTD

        '            Select Case SC.Status

        '                Case SpecCaseWWDWorkStatus.FinalReview, SpecCaseWWDWorkStatus.AdminLOD
        '                    'grab the proper memo

        '                    template = (From t In templates Select t Where t.Component = 6 And t.ModuleType = 9).FirstOrDefault()

        '                    'If (SC.GetActiveBoardMedicalFinding() = 1) Then
        '                    '    template = (From t In templates Select t Where t.Id = SC.GetActiveMemoTemplateId()).FirstOrDefault()
        '                    '    CreateMemo.ToolTip = "Create WWD Return to Duty Memo"

        '                    'ElseIf (SC.GetActiveBoardMedicalFinding() = 2) Or (SC.GetActiveMemoTemplateId() = MemoType.WWD_Admin_LOD_AFPC) Then
        '                    '    template = (From t In templates Select t Where t.Id = SC.GetActiveMemoTemplateId()).FirstOrDefault()
        '                    '    CreateMemo.ToolTip = "Create WWD Admin LOD Letter"

        '                    '    template2 = MemoType.MEB_Disqualification_Letter
        '                    '    CreateMemo2.ToolTip = "Create MEB Disqualificatoin Letter"

        '                    'ElseIf (SC.GetActiveBoardMedicalFinding() = 0) Then
        '                    '    template = (From t In templates Select t Where t.Id = SC.GetActiveMemoTemplateId()).FirstOrDefault()
        '                    '    CreateMemo.ToolTip = "Create WWD Disqualification Letter"

        '                    '    template2 = MemoType.WWD_Unit_Disqualification_Letter
        '                    '    CreateMemo2.ToolTip = "Create WWD Unit Disqualification Letter"

        '                    'End If

        '                    If (template IsNot Nothing) Then
        '                        'now get the user's permissions for this memo
        '                        Dim perms = (From g In template.GroupPermissions Where g.Group.Id = CInt(Session("groupId")) Select g).FirstOrDefault()
        '                        If (perms IsNot Nothing AndAlso perms.CanCreate) Then
        '                            'the user has create permissions for this memo, so set it as the one to be created
        '                            templateId = template.Id
        '                        End If
        '                    End If

        '                Case SpecCaseWWDWorkStatus.FinalReviewSAF, SpecCaseWWDWorkStatus.FinalReviewIPEB, SpecCaseWWDWorkStatus.FinalReviewFPEB

        '                    template3 = (From t In templates Select t Where t.Id = SC.GetActiveMemoTemplateId()).FirstOrDefault()

        '                    If (template3 IsNot Nothing) Then
        '                        'now get the user's permissions for this memo
        '                        Dim perms = (From g In template3.GroupPermissions Where g.Group.Id = CInt(Session("groupId")) Select g).FirstOrDefault()
        '                        If (perms IsNot Nothing AndAlso perms.CanCreate) Then
        '                            'the user has create permissions for this memo, so set it as the one to be created
        '                            templateId3 = template3.Id
        '                        End If
        '                    End If

        '            End Select

        '            'if the user doesn't have access, hide the create button
        '            If (templateId = 0) Then
        '                CreateMemo.Visible = False
        '            Else
        '                CreateMemo.Attributes.Add("onclick", "showEditor(0," + templateId.ToString() + ", " + modType.ToString() + ");")
        '            End If

        '            If (Convert.ToInt32(template2) = 0 Or memoDisqual Is Nothing) Then
        '                CreateMemo2.Visible = False
        '            ElseIf ((SC.Status = SpecCaseWWDWorkStatus.FinalReview And SC.GetActiveMemoTemplateId() = MemoType.WWD_Admin_LOD_AFPC) Or
        '                    (SC.Status = SpecCaseWWDWorkStatus.AdminLOD And SC.GetActiveMemoTemplateId() = MemoType.WWD_Admin_LOD_AFPC) Or
        '                    (SC.Status = SpecCaseWWDWorkStatus.PackageReview And SC.GetActiveMemoTemplateId() = MemoType.WWD_Admin_LOD_AFPC) Or
        '                    (SC.Status = SpecCaseWWDWorkStatus.FinalReview And SC.GetActiveMemoTemplateId() = MemoType.WWD_Disqualification_Letter)) Then

        '                CreateMemo2.Attributes.Add("onclick", "showEditor(0," + Convert.ToInt32(template2).ToString() + ", " + modType.ToString() + ");")

        '            End If

        '            If (templateId3 = 0 Or memoListRTD Is Nothing) Then
        '                CreateRTDMemo.Visible = False
        '            Else
        '                CreateRTDMemo.Visible = True
        '                CreateRTDMemo.Attributes.Add("onclick", "showEditor(0," + templateId3.ToString() + ", " + modType.ToString() + ");")

        '            End If

        '        End Sub

        '        Private Sub CheckMemoList()
        '            Dim associatedRefId As Integer = GetAssociatedRefId()

        '            If (Not Documents.UserCanEdit) Then
        '                CreateMemo.Visible = False
        '                CreateMemo2.Visible = False
        '                CreateRTDMemo.Visible = False
        '            End If

        '            ' get memo list
        '            Dim memoList As IList(Of Memorandum2)
        '            memoList = (From l In MemoStore.GetByRefnModule(associatedRefId, ModuleType)
        '                        Where l.Deleted = False And l.Template.Id = SC.GetActiveMemoTemplateId()
        '                        Select l).ToList

        '            Dim memoDisqual As IList(Of Memorandum2) = New List(Of Memorandum2)
        '            If (SC.GetActiveBoardMedicalFinding() = 2) Then

        '                memoDisqual = (From l In MemoStore.GetByRefnModule(associatedRefId, ModuleType)
        '                               Where l.Deleted = False And l.Template.Id = MemoType.MEB_Disqualification_Letter Select l).ToList

        '                If (memoDisqual.Count > 0) Then
        '                    CreateMemo2.Visible = False
        '                Else
        '                    CreateMemo2.Visible = True
        '                End If

        '            ElseIf (SC.GetActiveBoardMedicalFinding() = 0) Then
        '                memoDisqual = (From l In MemoStore.GetByRefnModule(associatedRefId, ModuleType)
        '                               Where l.Deleted = False And l.Template.Id = MemoType.WWD_Unit_Disqualification_Letter Select l).ToList

        '                If (memoDisqual.Count > 0) Then
        '                    CreateMemo2.Visible = False
        '                Else
        '                    CreateMemo2.Visible = True
        '                End If
        '            Else
        '                CreateMemo2.Visible = False

        '            End If

        '            Dim memoListRTD As IList(Of Memorandum2)
        '            memoListRTD = (From l In MemoStore.GetByRefnModule(associatedRefId, ModuleType)
        '                           Where l.Deleted = False And (l.Template.Id = MemoType.WWD_4_2_RTD Or
        '                                                        l.Template.Id = MemoType.WWD_4_Non_ALC_C Or
        '                                                        l.Template.Id = MemoType.WWD_RTD_SAF Or
        '                                                        l.Template.Id = MemoType.WWD_RTD_SG)
        '                           Select l).ToList

        '            If (SC.Status = SpecCaseWWDWorkStatus.PackageReview OrElse SC.Status = SpecCaseWWDWorkStatus.FinalReview OrElse SC.Status = SpecCaseWWDWorkStatus.AdminLOD) Then
        '                If (memoList.Count > 0) Then
        '                    CreateMemo.Visible = False
        '                Else
        '                    CreateMemo.Visible = True
        '                End If
        '            Else
        '                CreateMemo.Visible = False
        '            End If

        '            If (SC.Status = SpecCaseWWDWorkStatus.FinalReviewSAF OrElse SC.Status = SpecCaseWWDWorkStatus.FinalReviewIPEB OrElse SC.Status = SpecCaseWWDWorkStatus.FinalReviewFPEB) Then
        '                If (memoListRTD.Count > 0) Then
        '                    CreateRTDMemo.Visible = False
        '                Else
        '                    CreateRTDMemo.Visible = True
        '                End If
        '            Else
        '                CreateRTDMemo.Visible = False
        '            End If

        '        End Sub

        '        Protected Sub UpdateMemoList()
        '            Dim associatedRefId As Integer = GetAssociatedRefId()
        '            Dim modType As Integer = ModuleType

        '            MemoRepeater.DataSource = From m In MemoStore.GetByRefnModule(associatedRefId, 13)
        '                                      Where (m.Deleted = False)
        '                                      Select m
        '                                      Order By m.CreatedDate
        '            MemoRepeater.DataBind()

        '            If ((SC.GetActiveBoardMedicalFinding().HasValue AndAlso (SC.Status = SpecCaseWWDWorkStatus.FinalReview OrElse SC.Status = SpecCaseWWDWorkStatus.AdminLOD)) OrElse SC.Status = SpecCaseWWDWorkStatus.FinalReviewSAF OrElse SC.Status = SpecCaseWWDWorkStatus.FinalReviewIPEB OrElse SC.Status = SpecCaseWWDWorkStatus.FinalReviewFPEB) Then
        '                CheckMemoList()
        '            End If

        '        End Sub

        '        Protected Sub MemoRepeater_ItemCommand(ByVal source As Object, ByVal e As System.Web.UI.WebControls.RepeaterCommandEventArgs) Handles MemoRepeater.ItemCommand
        '            Dim associatedRefId As Integer = GetAssociatedRefId()
        '            If (e.CommandName = "DeleteMemo") Then

        '                Dim memoId As Integer = CInt(e.CommandArgument)
        '                Dim memo As Memorandum2 = MemoStore.GetById(memoId)

        '                LogManager.LogAction(ModuleType, UserAction.DocumentDeleted, associatedRefId, "Memo: " + memo.Template.Title)

        '                memo.Deleted = True
        '                MemoStore.SaveOrUpdate(memo)
        '                MemoStore.CommitChanges()
        '                MemoStore.Evict(memo)
        '                UpdateMemoList()
        '            End If

        '        End Sub

        '        Protected Sub MemoRepeater_ItemDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.RepeaterItemEventArgs) Handles MemoRepeater.ItemDataBound

        '            Dim memo As Memorandum2 = CType(e.Item.DataItem, Memorandum2)

        '            Dim link As HyperLink = CType(e.Item.FindControl("ViewMemoLink"), HyperLink)
        '            link.Attributes.Add("onclick", "displayMemo('" + GetMemoURL(memo.Id) + "'); return false;")

        '            Dim perm = (From g In memo.Template.GroupPermissions
        '                        Where g.Group.Id = CInt(Session("groupId"))
        '                        Select g).FirstOrDefault()

        '            If (perm Is Nothing OrElse Not perm.CanView) Then
        '                e.Item.Visible = False
        '                Exit Sub
        '            End If

        '            Dim edit As Image = CType(e.Item.FindControl("EditMemo"), Image)
        '            Dim delete As Image = CType(e.Item.FindControl("DeleteMemo"), Image)

        '            If (perm IsNot Nothing) Then

        '                edit.Visible = perm.CanEdit
        '                delete.Visible = perm.CanDelete

        '                If (perm.CanEdit) Then
        '                    edit.Attributes.Add("onClick", "showEditor(" + memo.Id.ToString() + "," + memo.Template.Id.ToString() + ");")
        '                End If

        '            Else
        '                CType(e.Item.FindControl("DeleteMemo"), Image).Visible = False
        '                CType(e.Item.FindControl("EditMemo"), Image).Visible = False
        '            End If

        '        End Sub

        '        Protected Sub RefreshButton_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles RefreshButton.Click

        '            UpdateMemoList()

        '        End Sub

        '        Private Function GetMemoURL(memoId As Integer) As String

        '            Return Me.ResolveClientUrl("~/Secure/Shared/Memos/ViewPdf.aspx") +
        '                                                    "?id=" + GetAssociatedRefId().ToString() +
        '                                                    "&memo=" + memoId.ToString() +
        '                                                    "&mod=" + CInt(ModuleType).ToString()

        '        End Function

        '#Region "SAF Letter Related"

        '        Private Property SAFLetterUploaded() As Integer
        '            Get
        '                Dim key As String = "SAFLetterUploaded"

        '                If (ViewState(key) Is Nothing) Then

        '                    If (SpecCase Is Nothing) Then
        '                        Return 0
        '                    End If
        '                    ViewState(key) = SC.SAFLetterUploaded
        '                End If

        '                Return CStr(ViewState(key))

        '            End Get

        '            Set(ByVal value As Integer)
        '                ViewState("SAFLetterUploaded") = value.ToString()
        '            End Set
        '        End Property

        '        Private Sub SaveFindings()

        '            If (Not Documents.UserCanEdit) Then
        '                Exit Sub
        '            End If

        '            Dim access As ALOD.Core.Domain.Workflow.PageAccessType
        '            access = SectionList(WDSectionNames.WD_HQT_FINAL_REV.ToString())

        '            If ((access = ALOD.Core.Domain.Workflow.PageAccessType.ReadWrite) And (SpecCase.Status = SpecCaseWWDWorkStatus.FinalReview)) Then

        '                If (SC.SAFLetterUploaded = 1) Then
        '                    SC.SAFLetterUploadDate = Now
        '                End If

        '                If (SC.SAFLetterUploaded = 0) Then
        '                    SC.SAFLetterUploadDate = Nothing 'Reset the Uploaded date, probably the HQ AFRC Tech delete the current SAF Letter
        '                End If

        '            End If

        '            'Save Findings
        '            SCDao.SaveOrUpdate(SpecCase)
        '            SCDao.CommitChanges()

        '        End Sub

        '#End Region

    End Class

End Namespace