Imports ALOD.Core.Domain.Documents
Imports ALOD.Core.Domain.Modules.Lod
Imports ALOD.Core.Domain.Modules.SARC
Imports ALOD.Core.Domain.Workflow
Imports ALOD.Core.Interfaces.DAOInterfaces
Imports ALOD.Data
Imports ALOD.Logging
Imports ALOD.Web.UserControls
Imports ALODWebUtility.Common
Imports ALODWebUtility.Printing

Namespace Web.SARC

    Partial Class Documents
        Inherits System.Web.UI.Page

#Region "Fields..."

        Private _docCatViewDao As IDocCategoryViewDao
        Private _documentsDao As IDocumentDao
        Private _factory As IDaoFactory
        Private _memoSource As MemoDao2
        Private _sarc As RestrictedSARC
        Private _sarcAppeal As SARCAppeal
        Private _sarcAppealDao As ISARCAppealDAO
        Private _sarcDao As ISARCDAO
        Private _sarcPostProcessing As RestrictedSARCPostProcessing
        Private _sarcPostProcessingDao As ISARCPostProcessingDao

#End Region

#Region "Properties..."

        ReadOnly Property SARCAppeal() As SARCAppeal
            Get
                If (_sarcAppeal Is Nothing) Then
                    Dim _appealId As Integer = APSADao.GetAppealIdByInitId(RequestId, SARC.Workflow)

                    If (_appealId <> 0) Then
                        _sarcAppeal = APSADao.GetById(_appealId)
                    Else
                        _sarcAppeal = Nothing
                    End If

                End If
                Return _sarcAppeal

            End Get
        End Property

        Public ReadOnly Property ModuleType() As ModuleType
            Get
                Return ModuleType.SARC
            End Get
        End Property

        Public ReadOnly Property Navigator() As TabNavigator
            Get
                Return Master.Navigator
            End Get
        End Property

        Protected ReadOnly Property APSADao As ISARCAppealDAO
            Get
                If (_sarcAppealDao Is Nothing) Then
                    _sarcAppealDao = DaoFactory.GetSARCAppealDao()
                End If

                Return _sarcAppealDao
            End Get
        End Property

        Protected ReadOnly Property DaoFactory As IDaoFactory
            Get
                If (_factory Is Nothing) Then
                    _factory = New NHibernateDaoFactory()
                End If

                Return _factory
            End Get
        End Property

        Protected ReadOnly Property DocCatViewDao() As IDocCategoryViewDao
            Get
                If (_docCatViewDao Is Nothing) Then
                    _docCatViewDao = DaoFactory.GetDocCategoryViewDao()
                End If

                Return _docCatViewDao
            End Get
        End Property

        Protected ReadOnly Property DocumentDao() As IDocumentDao
            Get
                If (_documentsDao Is Nothing) Then
                    _documentsDao = New SRXDocumentStore(CStr(HttpContext.Current.Session("UserName")))
                End If

                Return _documentsDao
            End Get
        End Property

        Protected ReadOnly Property MemoStore() As MemoDao2
            Get
                If (_memoSource Is Nothing) Then
                    _memoSource = DaoFactory.GetMemoDao2()
                End If
                Return _memoSource
            End Get
        End Property

        Protected ReadOnly Property RequestId() As Integer
            Get
                Return CInt(Request.QueryString("refId"))
            End Get
        End Property

        Protected ReadOnly Property SARC As RestrictedSARC
            Get
                If (_sarc Is Nothing) Then
                    _sarc = SARCDao.GetById(RequestId)
                End If

                Return _sarc
            End Get
        End Property

        Protected ReadOnly Property SARCDao As ISARCDAO
            Get
                If (_sarcDao Is Nothing) Then
                    _sarcDao = DaoFactory.GetSARCDao()
                End If

                Return _sarcDao
            End Get
        End Property

        Protected ReadOnly Property SARCPostProcessing As RestrictedSARCPostProcessing
            Get
                If (_sarcPostProcessing Is Nothing) Then
                    _sarcPostProcessing = SARCPostProcessingDao.GetById(SARC.Id)
                End If

                Return _sarcPostProcessing
            End Get
        End Property

        Protected ReadOnly Property SARCPostProcessingDao As ISARCPostProcessingDao
            Get
                If (_sarcPostProcessingDao Is Nothing) Then
                    _sarcPostProcessingDao = DaoFactory.GetSARCPostProcessingDao()
                End If

                Return _sarcPostProcessingDao
            End Get
        End Property

        Protected ReadOnly Property UserCanOnlyViewRedactedDocuments As Boolean
            Get
                Return UserHasPermission(PERMISSION_SARC_VIEW_REDACTED_DOCUMENTS_ONLY)
            End Get
        End Property

#End Region

#Region "Page Methods..."

        Protected Sub DisableSARCAppealMemo()
            ViewSARCAppealLink.Visible = False
            IconB.Visible = False
            SARCAppealDescription.Visible = False
        End Sub

        Protected Sub GetForm348()
            If (UserCanOnlyViewRedactedDocuments) Then
                pnlForm348R.Visible = False
                Exit Sub
            End If

            If (SARC.WorkflowStatus.StatusCodeType.IsFinal) Then
                Dim _Documents As IList(Of Document) = DocumentDao.GetDocumentsByGroupId(SARC.DocumentGroupId)
                Dim strAttribute348 As String = ViewForms.RestrictedSARCFormPDFLinkAttribute(SARC, _Documents)
                ViewDocLink348.Attributes.Add("onclick", "" & strAttribute348 & "")
            Else
                ViewDocLink348.Attributes.Add("onclick", "printForms('" & RequestId.ToString() & "', 'SARC');")
            End If

        End Sub

        Protected Sub GetSARCAppealMemo()

            If (SARCAppeal IsNot Nothing) Then

                If (SARCAppeal.Status = SARCAppealWorkStatus.Approved OrElse SARCAppeal.Status = SARCAppealWorkStatus.Denied) Then

                    Dim AppealMemo = (From m In MemoStore.GetByRefnModule(SARCAppeal.InitialId, ModuleType)
                                      Where m.Deleted = False AndAlso (m.Template.Id = MemoType.SARC_APPEAL_APPROVED OrElse m.Template.Id = MemoType.SARC_APPEAL_DISAPPROVAL)
                                      Select m
                                      Order By m.CreatedDate Descending).FirstOrDefault

                    If (AppealMemo Is Nothing) Then
                        DisableSARCAppealMemo()
                        RowSARC.Visible = True
                        SARCAppealMemoError.Visible = True
                        Exit Sub
                    End If

                    Dim perm = (From g In AppealMemo.Template.GroupPermissions
                                Where g.Group.Id = CInt(Session("groupId"))
                                Select g).FirstOrDefault()

                    If (perm Is Nothing OrElse Not perm.CanView) Then
                        DisableSARCAppealMemo()
                        Exit Sub
                    End If

                    If (AppealMemo.Template.Id = MemoType.SARC_APPEAL_APPROVED) Then
                        SARCAppealDescription.Text = "AFRC CV Approval of SARC Appeal Memorandum"
                    Else
                        SARCAppealDescription.Text = "AFRC CV Disapproval of SARC Appeal Memorandum"
                    End If

                    RowSARC.Visible = True
                    ViewSARCAppealLink.Attributes.Add("onClick", "displayMemo('" + GetMemoURL(AppealMemo.Id) + "'); return false;")
                Else
                    DisableSARCAppealMemo()
                End If
            Else
                DisableSARCAppealMemo()
            End If

        End Sub

        Protected Function HasPostCompletionProcessingData() As Boolean
            If (SARC.FinalFindings = 0) Then
                Return False
            End If

            If (SARCPostProcessing Is Nothing) Then
                Return False
            End If

            If (Not String.IsNullOrEmpty(SARCPostProcessing.email)) Then
                Return True
            End If

            If (Not String.IsNullOrEmpty(SARCPostProcessing.HelpExtensionNumber)) Then
                Return True
            End If

            If (Not String.IsNullOrEmpty(SARCPostProcessing.AppealAddress.Street) _
                    AndAlso Not String.IsNullOrEmpty(SARCPostProcessing.AppealAddress.City) _
                    AndAlso Not String.IsNullOrEmpty(SARCPostProcessing.AppealAddress.State) _
                    AndAlso Not String.IsNullOrEmpty(SARCPostProcessing.AppealAddress.Zip) _
                    AndAlso Not String.IsNullOrEmpty(SARCPostProcessing.AppealAddress.Country)) Then
                Return True
            End If

            Return False
        End Function

        Protected Sub InitMemoCreation()
            ' The Restricted SARC Workflow currently makes use of the same findings and determination memo templates used by the base LOD Workflow...
            Dim sarcMemoTemplates = From m In MemoStore.GetAllTemplates()
                                    Where (m.Component = Session("Compo") And m.ModuleType = ModuleType)
                                    Select m

            Dim templateId As Integer = 0
            Dim isDeterminationTemplate As Boolean = False
            Dim refId As String = RequestId.ToString()
            If (IsDeterminationMemoGenerationPossible()) Then
                Dim template As MemoTemplate = Nothing

                'ForTesting88
                If (SARC.FinalFindings = ALOD.Core.Utils.Finding.In_Line_Of_Duty) Then
                    If (Session("Compo") = "6") Then
                        template = (From t In sarcMemoTemplates Select t Where t.Id = MemoType.SARC_Determination_ILOD).FirstOrDefault()
                        'Test
                        templateId = 72
                    ElseIf (Session("Compo") = "5") Then
                        template = (From t In sarcMemoTemplates Select t Where t.Id = MemoType.ANGSARC_Determination_ILOD).FirstOrDefault()
                        'Test
                        templateId = 172
                    End If
                    CreateMemo.ToolTip = "Create Member SARC LOD Determination (ILOD)"
                ElseIf (SARC.FinalFindings = ALOD.Core.Utils.Finding.Nlod_Not_Due_To_OwnMisconduct) Then
                    If (Session("Compo") = "6") Then
                        template = (From t In sarcMemoTemplates Select t Where t.Id = MemoType.SARC_Determination_NILOD).FirstOrDefault()
                        'Test
                        templateId = 73
                    ElseIf (Session("Compo") = "5") Then
                        template = (From t In sarcMemoTemplates Select t Where t.Id = MemoType.ANGSARC_Determination_NILOD).FirstOrDefault()
                        'Test
                        templateId = 173
                    End If
                    CreateMemo.ToolTip = "Create Member SARC LOD Determination (NILOD)"
                End If

                If (template IsNot Nothing) Then
                    Dim perms = (From g In template.GroupPermissions Where g.Group.Id = CInt(Session("groupId")) Select g).FirstOrDefault()

                    If (perms IsNot Nothing AndAlso perms.CanCreate) Then
                        'where templateId was originally set
                        'templateId = template.Id
                        isDeterminationTemplate = True
                    End If
                End If
            End If

            'refId = 0

            If (templateId = 0) Then
                CreateMemo.Visible = False
            Else
                CreateMemo.Attributes.Add("onclick", "showEditor(0," + templateId.ToString() + "," + isDeterminationTemplate.ToString().ToLower() + "," + refId + ");")
            End If
        End Sub

        Protected Function IsDeterminationMemo(ByVal templateId As Integer) As Boolean
            If (templateId = MemoType.SARC_Determination_ILOD OrElse templateId = MemoType.SARC_Determination_NILOD) Then
                Return True
            End If

            Return False
        End Function

        Protected Function IsDeterminationMemoGenerationPossible() As Boolean
            If (Not Documents.UserCanEdit) Then
                Return False
            End If

            If (Not SARC.IsInPostCompletionProcessing) Then
                Return False
            End If

            If (Not UserHasPermission(PERMISSION_SARC_POSTPROCESSING)) Then
                Return False
            End If

            If (UserCanOnlyViewRedactedDocuments) Then
                Return False
            End If

            If (Not HasPostCompletionProcessingData()) Then
                Return False
            End If

            Return True
        End Function

        Protected Sub MemoRepeater_ItemCommand(ByVal source As Object, ByVal e As System.Web.UI.WebControls.RepeaterCommandEventArgs) Handles MemoRepeater.ItemCommand
            If (e.CommandName = "DeleteMemo") Then
                Dim memoId As Integer = CInt(e.CommandArgument)
                Dim memo As Memorandum2 = MemoStore.GetById(memoId)

                LogManager.LogAction(ModuleType, UserAction.DocumentDeleted, RequestId, "Memo: " + memo.Template.Title)

                memo.Deleted = True
                MemoStore.SaveOrUpdate(memo)
                MemoStore.CommitChanges()
                MemoStore.Evict(memo)
                UpdateMemoList()

                If (IsDeterminationMemo(memo.Template.Id)) Then
                    SARC.UpdateIsPostProcessingComplete(DaoFactory)
                    SARCDao.SaveOrUpdate(SARC)
                End If
            End If
        End Sub

        Protected Sub MemoRepeater_ItemDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.RepeaterItemEventArgs) Handles MemoRepeater.ItemDataBound
            If (e.Item.ItemType <> ListItemType.Item AndAlso e.Item.ItemType <> ListItemType.AlternatingItem) Then
                Exit Sub
            End If
            Dim refId As String = RequestId.ToString()
            Dim memo As Memorandum2 = CType(e.Item.DataItem, Memorandum2)

            Dim link As HyperLink = CType(e.Item.FindControl("ViewMemoLink"), HyperLink)
            link.Attributes.Add("onclick", "displayMemo('" + GetMemoURL(memo.Id) + "'); return false;")

            Dim perm = (From g In memo.Template.GroupPermissions
                        Where g.Group.Id = CInt(Session("groupId"))
                        Select g).FirstOrDefault()

            If (perm Is Nothing OrElse Not perm.CanView) Then
                e.Item.Visible = False
                Exit Sub
            End If

            If (Documents.UserCanEdit AndAlso (perm.CanEdit OrElse perm.CanDelete)) Then
                If (perm.CanEdit) Then
                    Dim edit As Image = CType(e.Item.FindControl("EditMemo"), Image)
                    edit.Visible = perm.CanEdit
                    edit.Attributes.Add("onClick", "showEditor(" + memo.Id.ToString() + "," + memo.Template.Id.ToString() + "," + refId + ");")
                End If

                If (perm.CanDelete) Then
                    Dim delete As Image = CType(e.Item.FindControl("DeleteMemo"), Image)
                    delete.Visible = perm.CanDelete
                End If
            Else
                CType(e.Item.FindControl("EditMemo"), Image).Visible = False
                CType(e.Item.FindControl("DeleteMemo"), Image).Visible = False
            End If
        End Sub

        Protected Sub NewMemoButton_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles NewMemoButton.Click
            LogManager.LogAction(ModuleType, UserAction.PostCompletion, RequestId, "Post Completion: " + SESSION_GROUP_NAME + " Generated Determination Memo")
            UpdateMemoList()

            SARC.UpdateIsPostProcessingComplete(DaoFactory)
            SARCDao.SaveOrUpdate(SARC)
        End Sub

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

            AddStyleSheet(Page, "~/Styles/Documents.css")

            If (Not IsPostBack) Then

                GetDocuments()
                GetForm348()
                GetSARCAppealMemo()
                InitMemoCreation()

                UpdateMemoList()

            End If
        End Sub

        Protected Sub RefreshButton_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles RefreshButton.Click
            UpdateMemoList()
        End Sub

        Protected Sub SetCreateMemoButtonVisibility()
            CreateMemo.Visible = (IsDeterminationMemoGenerationPossible() AndAlso Not SARC.NotificationMemoCreated(MemoStore))
        End Sub

        Protected Sub UpdateMemoList()
            If (UserCanOnlyViewRedactedDocuments) Then
                pnlMemo.Visible = False
                Exit Sub
            End If

            Dim AppealMemos As IList(Of MemoType) = New List(Of MemoType)
            AppealMemos.Add(MemoType.SARC_APPEAL_APPROVED)
            AppealMemos.Add(MemoType.SARC_APPEAL_DISAPPROVAL)

            MemoRepeater.DataSource = From m In MemoStore.GetByRefnModule(RequestId, ModuleType)
                                      Where m.Deleted = False AndAlso Not AppealMemos.Contains(m.Template.Id)
                                      Select m
                                      Order By m.CreatedDate

            MemoRepeater.DataBind()
            SetCreateMemoButtonVisibility()

        End Sub

        Private Function GetDocumentCategoryStartIndex() As Integer
            If (UserCanOnlyViewRedactedDocuments) Then
                Return 1
            Else
                Return 3
            End If
        End Function

        Private Sub GetDocuments()

            If (SARC.DocumentGroupId Is Nothing OrElse SARC.DocumentGroupId = 0) Then
                SARC.CreateDocumentGroup(DocumentDao)
                SARCDao.SaveOrUpdate(SARC)
                SARCDao.CommitChanges()
            End If

            SARC.ProcessDocuments(DaoFactory)
            Documents.Initialize(Me, Navigator, New WorkflowDocument(SARC, GetDocumentCategoryStartIndex()))

        End Sub

        Private Function GetMemoURL(memoId As Integer) As String

            Return Me.ResolveClientUrl("~/Secure/Shared/Memos/ViewPdf.aspx") +
                                                    "?id=" + RequestId.ToString() +
                                                    "&memo=" + memoId.ToString() +
                                                    "&mod=" + CInt(ModuleType).ToString()

        End Function

#End Region

    End Class

End Namespace