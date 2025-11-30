Imports ALOD.Core.Domain.Documents
Imports ALOD.Core.Domain.Lookup
Imports ALOD.Data
Imports ALOD.Data.Services
Imports ALOD.Core.Domain.Modules.Lod
Imports ALOD.Core.Domain.Modules.Appeals
Imports ALOD.Core.Domain.Modules.SARC
Imports ALOD.Logging
Imports ALOD.Web.UserControls
Imports ALODWebUtility.Common
Imports ALODWebUtility.Printing
Imports ALOD.Core.Interfaces.DAOInterfaces
Imports ALOD.Core.Domain.Workflow
Imports NHibernate.Criterion

Namespace Web.LOD
    Partial Public Class Secure_lod_Documents
        Inherits System.Web.UI.Page

#Region "Fields"
        Private _factory As IDaoFactory
        Private _LODDao As ILineOfDutyDao
        Private _postDao As ILineOfDutyPostProcessingDao
        Private _LODAppealDAO As ILODAppealDAO
        Private _SARCAppealDAO As ISARCAppealDAO
        Private _DocumentDao As IDocumentDao
        Private _DocumentDao2 As IDocumentDao
        Private _docCatDao As IDocCategoryViewDao
        Private _lookupDao As ILookupDao
        Private _memoSource As MemoDao
        Private _lod As LineOfDuty
        Private _ap As LODAppeal
        Private _apsa As SARCAppeal
#End Region

#Region "Properties"
        Protected ReadOnly Property DaoFactory As IDaoFactory
            Get
                If (_factory Is Nothing) Then
                    _factory = New NHibernateDaoFactory()
                End If

                Return _factory
            End Get
        End Property

        ReadOnly Property LODDao() As ILineOfDutyDao
            Get
                If (_LODDao Is Nothing) Then
                    _LODDao = DaoFactory.GetLineOfDutyDao()
                End If

                Return _LODDao
            End Get
        End Property

        ReadOnly Property PostDao() As ILineOfDutyPostProcessingDao
            Get
                If (_postDao Is Nothing) Then
                    _postDao = DaoFactory.GetLineOfDutyPostProcessingDao()
                End If

                Return _postDao
            End Get
        End Property

        ReadOnly Property APDao() As ILODAppealDAO
            Get
                If (_LODAppealDAO Is Nothing) Then
                    _LODAppealDAO = DaoFactory.GetLODAppealDao()
                End If

                Return _LODAppealDAO
            End Get
        End Property

        ReadOnly Property APSADao() As ISARCAppealDAO
            Get
                If (_SARCAppealDAO Is Nothing) Then
                    _SARCAppealDAO = DaoFactory.GetSARCAppealDao()
                End If

                Return _SARCAppealDAO
            End Get
        End Property

        Protected ReadOnly Property RequestId() As Integer
            Get
                Return CInt(Request.QueryString("refId"))
            End Get
        End Property

        ReadOnly Property LOD() As LineOfDuty
            Get
                If (_lod Is Nothing) Then
                    _lod = LODDao.GetById(RequestId)
                End If
                Return _lod
            End Get
        End Property

        ReadOnly Property LODAppeal() As LODAppeal
            Get

                If (_ap Is Nothing) Then
                    Dim _appealId = APDao.GetAppealIdByInitLod(RequestId)

                    If _appealId <> 0 Then
                        _ap = APDao.GetById(_appealId)
                    Else
                        _ap = Nothing
                    End If
                End If
                Return _ap
            End Get
        End Property

        ReadOnly Property SARCAppeal() As SARCAppeal
            Get

                If (_apsa Is Nothing) Then
                    Dim _sarcId = APSADao.GetAppealIdByInitId(RequestId, LOD.Workflow)

                    If _sarcId <> 0 Then
                        _apsa = APSADao.GetById(_sarcId)
                    Else
                        _apsa = Nothing
                    End If
                End If
                Return _apsa
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

        Private ReadOnly Property MemoStore() As MemoDao
            Get
                If (_memoSource Is Nothing) Then
                    _memoSource = DaoFactory.GetMemoDao()
                End If
                Return _memoSource
            End Get
        End Property

        Protected ReadOnly Property LookupDao As ILookupDao
            Get
                If (_lookupDao Is Nothing) Then
                    _lookupDao = DaoFactory.GetLookupDao()
                End If

                Return _lookupDao
            End Get
        End Property

        Public ReadOnly Property Navigator() As TabNavigator
            Get
                Return Master.Navigator
            End Get
        End Property

        Protected ReadOnly Property ModuleType() As ModuleType
            Get
                Return ModuleType.LOD
            End Get
        End Property


#End Region


#Region "Page Methods"
        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

            AddStyleSheet(Page, "~/Styles/Documents.css")

            If (Not IsPostBack) Then


                GetDocuments()
                GetForm348()
                GetLODAppealMemo()
                GetSARCAppealMemo()
                InitMemoCreation()
                UpdateMemoList()
            End If

        End Sub

        Private Sub GetDocuments()

            If (LOD.DocumentGroupId Is Nothing OrElse LOD.DocumentGroupId = 0) Then
                LOD.CreateDocumentGroup(DocumentDao)
                LODDao.SaveOrUpdate(LOD)
                LODDao.CommitChanges()
            End If

            Documents.Initialize(Me, Navigator, New WorkflowDocument(LOD, 3, DaoFactory))

        End Sub

        Public Sub GetForm348()
            If (LOD.WorkflowStatus.StatusCodeType.IsFinal) Then
                Dim _Documents As IList(Of Document) = DocumentDao.GetDocumentsByGroupId(LOD.DocumentGroupId)
                Dim strAttribute348 As String = ViewForms.LinkAttribute348(RequestId.ToString(), _Documents, "lod")
                ViewDocLink348.Attributes.Add("onclick", "" & strAttribute348 & "")
            Else
                ViewDocLink348.Attributes.Add("onclick", "printForms('" & RequestId.ToString() & "', 'lod');")
            End If

            If (LOD.Formal) AndAlso (LOD.LODInvestigation IsNot Nothing) Then
                Description.Text = "AFRC Form 348 and DD Form 261"
            End If
        End Sub

        Private Sub GetLODAppealMemo()

            If (LODAppeal IsNot Nothing) Then
                If (LODAppeal.CurrentStatusCode = AppealStatusCode.AppealApproved OrElse LODAppeal.CurrentStatusCode = AppealStatusCode.AppealDenied) Then


                    Dim AppealMemo = (From m In MemoStore.GetByRefId(LOD.Id)
                                      Where m.Deleted = False AndAlso (m.Template.Id = MemoType.ApprovalAppeal OrElse m.Template.Id = MemoType.DisapprovalAppeal)
                                      Select m
                                      Order By m.CreatedDate Descending).FirstOrDefault

                    If (AppealMemo Is Nothing) Then
                        DisableLODAppealMemo()
                        RowAppeal.Visible = True
                        AppealMemoError.Visible = True
                        Exit Sub
                    End If

                    Dim perm = (From g In AppealMemo.Template.GroupPermissions
                                Where g.Group.Id = CInt(Session("groupId"))
                                Select g).FirstOrDefault()

                    If (perm Is Nothing OrElse Not perm.CanView) Then
                        DisableLODAppealMemo()
                        Exit Sub
                    End If


                    If (AppealMemo.Template.Id = MemoType.ApprovalAppeal) Then
                        AppealDescription.Text = "AFRC CV Approval of Appeal Memorandum"
                    Else
                        AppealDescription.Text = "AFRC CV Disapproval of Appeal Memorandum"
                    End If

                    RowAppeal.Visible = True
                    ViewAppealLink.Attributes.Add("onclick", "displayMemo('" + GetMemoURL(AppealMemo.Id) + "'); return false;")

                Else
                    DisableLODAppealMemo()
                End If
            Else
                DisableLODAppealMemo()
            End If

        End Sub

        Protected Sub DisableLODAppealMemo()
            ViewAppealLink.Visible = False
            IconA.Visible = False
            AppealDescription.Visible = False
        End Sub

        Private Sub GetSARCAppealMemo()

            If (SARCAppeal IsNot Nothing) Then
                If (SARCAppeal.Status = SARCAppealWorkStatus.Approved OrElse SARCAppeal.Status = SARCAppealWorkStatus.Denied) Then


                    Dim AppealMemo = (From m In MemoStore.GetByRefId(LOD.Id)
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
                    ViewSARCAppealLink.Attributes.Add("onClick", "displayMemo(" + GetMemoURL(AppealMemo.Id) + "); return false;")
                Else
                    DisableSARCAppealMemo()
                End If
            Else
                DisableSARCAppealMemo()
            End If

        End Sub

        Protected Sub DisableSARCAppealMemo()
            ViewSARCAppealLink.Visible = False
            IconB.Visible = False
            SARCAppealDescription.Visible = False
        End Sub

        Private Sub InitMemoCreation()

            If (LOD.Workflow = 1) Then
                Form348_title.Text = "1 - AFRC Form 348 / DD Form 261"
                Description.Text = "AFRC Form 348"
            Else
                Form348_title.Text = "1 - AF Form 348 / DD Form 261"
                Description.Text = "AF Form 348"
            End If

            'get the memo templates
            Dim templates = From m In MemoStore.GetAllTemplates()
                            Where m.Component = Session("Compo") _
                            And m.ModuleType = ModuleType
                            Select m

            Dim templateId As Integer = 0
            Dim isDeterminationTemplate As Boolean = False

            Select Case LOD.CurrentStatusCode
                Case LodStatusCode.NotifyFormalInvestigator ' ALODWebUtility.Common.LodStatus.NotifyFormalInvestigator
                    If (LOD.AppointedIO IsNot Nothing) Then
                        'grab the io appointment memo 
                        Dim template As MemoTemplate = (From t In templates Select t Where t.Id = MemoType.LodAppointIo).FirstOrDefault()

                        If (template IsNot Nothing) Then
                            'now get the user's permissions for this memo
                            Dim perms = (From g In template.GroupPermissions Where g.Group.Id = CInt(Session("groupId")) Select g).FirstOrDefault()

                            If (perms IsNot Nothing AndAlso perms.CanCreate) Then
                                'the user has create permissions for this memo, so set it as the one to be created
                                templateId = template.Id
                                CreateMemo.ToolTip = "Create new IO Appointment memo"
                            End If

                        End If
                    End If

                Case LodStatusCode.Complete ' ALODWebUtility.Common.LodStatus.Complete
                    If Not (CheckPostCompletionData()) Then
                        Exit Select
                    End If

                    'grab the proper memo 
                    Dim template As MemoTemplate

                    If LOD.FinalFindings = ALOD.Core.Utils.Finding.In_Line_Of_Duty OrElse LOD.FinalFindings = ALOD.Core.Utils.Finding.Epts_Service_Aggravated Then
                        If (LOD.LODMedical.DeathInvolved = "Yes") Then
                            template = (From t In templates Select t Where t.Id = MemoType.LodFindingsILODDeath).FirstOrDefault()
                            CreateMemo.ToolTip = "Create Member Notification Memo (ILOD Death)"
                        Else
                            template = (From t In templates Select t Where t.Id = MemoType.LodFindingsILOD).FirstOrDefault()
                            CreateMemo.ToolTip = "Create Member Notification Memo (ILOD)"
                        End If
                    Else
                        If (LOD.LODMedical.DeathInvolved = "Yes") Then
                            template = (From t In templates Select t Where t.Id = MemoType.LodFindingsNLODDeath).FirstOrDefault()
                            CreateMemo.ToolTip = "Create Member Notification Memo (NILOD Death)"
                        Else
                            template = (From t In templates Select t Where t.Id = MemoType.LodFindingsNLOD).FirstOrDefault()
                            CreateMemo.ToolTip = "Create Member Notification Memo (NILOD)"
                        End If
                    End If

                    If (template IsNot Nothing) Then
                        'now get the user's permissions for this memo
                        Dim perms = (From g In template.GroupPermissions Where g.Group.Id = CInt(Session("groupId")) Select g).FirstOrDefault()

                        If (perms IsNot Nothing AndAlso perms.CanCreate) Then
                            'the user has create permissions for this memo, so set it as the one to be created
                            templateId = template.Id
                            isDeterminationTemplate = True
                        End If

                    End If
            End Select

            'if the user doesn't have access, hide the create button
            If (templateId = 0) Then
                CreateMemo.Visible = False
            Else
                CreateMemo.Attributes.Add("onclick", "showEditor(0," + templateId.ToString() + "," + isDeterminationTemplate.ToString().ToLower() + ");")
            End If

        End Sub

        Protected Sub UpdateMemoList()

            If (Not Documents.UserCanEdit) Then
                CreateMemo.Visible = False
            End If

            Dim AppealMemos As IList(Of MemoType) = New List(Of MemoType)
            AppealMemos.Add(MemoType.ApprovalAppeal)
            AppealMemos.Add(MemoType.DisapprovalAppeal)
            AppealMemos.Add(MemoType.SARC_APPEAL_APPROVED)
            AppealMemos.Add(MemoType.SARC_APPEAL_DISAPPROVAL)

            MemoRepeater.DataSource = From m In MemoStore.GetByRefId(LOD.Id)
                                      Where m.Deleted = False AndAlso Not AppealMemos.Contains(m.Template.Id)
                                      Select m
                                      Order By m.CreatedDate

            MemoRepeater.DataBind()

            CheckPostCompletionAccess()
            CheckIOAppointmentMemo()

        End Sub

        Protected Sub MemoRepeater_ItemCommand(ByVal source As Object, ByVal e As System.Web.UI.WebControls.RepeaterCommandEventArgs) Handles MemoRepeater.ItemCommand
            If (e.CommandName = "DeleteMemo") Then
                Dim memoId As Integer = CInt(e.CommandArgument)
                Dim memo As Memorandum = MemoStore.GetById(memoId)

                LogManager.LogAction(ModuleType, UserAction.DocumentDeleted, RequestId, "Memo: " + memo.Template.Title)

                memo.Deleted = True
                MemoStore.SaveOrUpdate(memo)
                MemoStore.CommitChanges()
                MemoStore.Evict(memo)
                UpdateMemoList()

                If (IsDeterminationMemo(memo.Template.Id)) Then
                    LOD.UpdateIsPostProcessingComplete(DaoFactory)
                    LodService.SaveUpdate(LOD)
                End If
            End If
        End Sub

        Protected Function IsDeterminationMemo(ByVal templateId As Integer) As Boolean
            If (templateId = MemoType.LodFindingsILOD OrElse templateId = MemoType.LodFindingsILODDeath OrElse templateId = MemoType.LodFindingsNLOD OrElse templateId = MemoType.LodFindingsNLODDeath) Then
                Return True
            End If

            Return False
        End Function

        Protected Sub MemoRepeater_ItemDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.RepeaterItemEventArgs) Handles MemoRepeater.ItemDataBound
            If (e.Item.ItemType <> ListItemType.Item AndAlso e.Item.ItemType <> ListItemType.AlternatingItem) Then
                Exit Sub
            End If

            Dim memo As Memorandum = CType(e.Item.DataItem, Memorandum)

            Dim link As HyperLink = CType(e.Item.FindControl("ViewMemoLink"), HyperLink)
            link.Attributes.Add("onclick", "displayMemo('" + GetMemoURL(memo.Id) + "'); return false;")

            Dim perm = (From g In memo.Template.GroupPermissions
                        Where g.Group.Id = CInt(Session("groupId"))
                        Select g).FirstOrDefault()

            If (perm Is Nothing OrElse Not perm.CanView) Then
                'if they can't view it, they can't do anything else with it so just exit
                e.Item.Visible = False
                Exit Sub
            End If

            If (Documents.UserCanEdit OrElse perm IsNot Nothing) Then

                Dim edit As Image = CType(e.Item.FindControl("EditMemo"), Image)
                Dim delete As Image = CType(e.Item.FindControl("DeleteMemo"), Image)

                edit.Visible = perm.CanEdit
                delete.Visible = perm.CanDelete

                If (perm.CanEdit) Then
                    edit.Attributes.Add("onClick", "showEditor(" + memo.Id.ToString() + "," + memo.Template.Id.ToString() + ");")
                End If
            Else
                'this is read only
                CType(e.Item.FindControl("EditMemo"), Image).Visible = False
                CType(e.Item.FindControl("DeleteMemo"), Image).Visible = False
            End If
        End Sub

        Protected Sub RefreshButton_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles RefreshButton.Click

            UpdateMemoList()
        End Sub

        Protected Sub NewMemoButton_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles NewMemoButton.Click
            ' Update tracking data
            Dim actionId As Integer = LogManager.LogAction(ModuleType, UserAction.PostCompletion, RequestId, "Post Completion: " + SESSION_GROUP_NAME + " Generated Determination Memo")


            UpdateMemoList()

            LOD.UpdateIsPostProcessingComplete(DaoFactory)
            LodService.SaveUpdate(LOD)
        End Sub

        Private Sub CheckIOAppointmentMemo()
            If (LOD.WorkflowStatus.StatusCodeType.Id = LodStatusCode.NotifyFormalInvestigator) Then
                Dim memoCount As Integer

                If (LookupDao.GetIsReinvestigationLod(LOD.Id)) Then
                    memoCount = GetIOAppointmentMemos(True).Count
                Else
                    memoCount = GetIOAppointmentMemos(False).Count
                End If

                CreateMemo.Visible = Not (memoCount > 0)
            End If
        End Sub

        Private Function GetIOAppointmentMemos(filterOutOriginalLODMemos) As IList(Of Memorandum)
            If (filterOutOriginalLODMemos) Then
                Return MemoStore.GetByRefId(RequestId).Where(Function(m) m.Deleted = False AndAlso m.Template.Id = MemoType.LodAppointIo AndAlso Not IsOriginalLODMemo(m)).OrderBy(Function(m) m.CreatedDate).ToList()
            Else
                Return MemoStore.GetByRefId(RequestId).Where(Function(m) m.Deleted = False AndAlso m.Template.Id = MemoType.LodAppointIo).OrderBy(Function(m) m.CreatedDate).ToList()
            End If
        End Function

        Private Function IsOriginalLODMemo(memo As Memorandum)
            If (memo.Contents(0) IsNot Nothing AndAlso memo.Contents(0).CreatedDate < LOD.CreatedDate) Then
                Return True
            End If

            Return False
        End Function

        Private Sub CheckPostCompletionAccess()
            If (IsLODInPostCompletionProcessing() AndAlso UserHasPostProcessingPermission()) Then
                If (LOD.Formal) Then

                    Dim RepeaterItems As RepeaterItemCollection = MemoRepeater.Items

                    If (RepeaterItems.Count > 0) Then
                        'By This point, there should be always one IO Appoint Letter

                        Dim memoCount As Integer = 0
                        Dim foundLodDeterminationLetter As Boolean = False

                        While (foundLodDeterminationLetter = False) And (memoCount < RepeaterItems.Count)
                            Dim InternalText As Label = RepeaterItems(memoCount).Controls(1).Controls(1)

                            If InternalText.Text.ToLower().Contains("lod determination") Then
                                foundLodDeterminationLetter = True
                            End If

                            memoCount += 1

                        End While

                        If (Not foundLodDeterminationLetter AndAlso CheckPostCompletionData()) Then
                            CreateMemo.Visible = True
                        Else
                            CreateMemo.Visible = False
                        End If

                    End If

                Else 'Informal

                    If Not (MemoRepeater.Items.Count > 0 AndAlso CheckPostCompletionData()) Then 'Original statement
                        CreateMemo.Visible = True
                    Else
                        CreateMemo.Visible = False
                    End If

                End If

            End If

        End Sub

        Private Function CheckPostCompletionData() As Boolean
            If (IsLODInPostCompletionProcessing() AndAlso UserHasPostProcessingPermission()) Then
                Dim lodPostProcessing As LineOfDutyPostProcessing = Nothing
                Dim chk As Boolean = False

                lodPostProcessing = PostDao.GetById(LOD.Id)

                If (lodPostProcessing Is Nothing) Then
                    Return False
                End If

                If (lodPostProcessing.chkPhone) Then
                    chk = True

                    If (String.IsNullOrEmpty(lodPostProcessing.HelpExtensionNumber)) Then
                        Return False
                    End If

                End If

                If (lodPostProcessing.chkEmail) Then
                    chk = True

                    If (String.IsNullOrEmpty(lodPostProcessing.email)) Then
                        Return False
                    End If

                End If

                If (lodPostProcessing.chkAddress) Then
                    chk = True

                    If (lodPostProcessing.AppealAddress Is Nothing OrElse
                        String.IsNullOrEmpty(lodPostProcessing.AppealAddress.Street) OrElse
                        String.IsNullOrEmpty(lodPostProcessing.AppealAddress.City) OrElse
                        String.IsNullOrEmpty(lodPostProcessing.AppealAddress.State) OrElse
                        String.IsNullOrEmpty(lodPostProcessing.AppealAddress.Zip) OrElse
                        String.IsNullOrEmpty(lodPostProcessing.AppealAddress.Country)) Then
                        Return False
                    End If

                End If



                If (Not String.IsNullOrEmpty(LOD.LODMedical.DeathInvolved) AndAlso LOD.LODMedical.DeathInvolved = "Yes") Then
                    If (String.IsNullOrEmpty(lodPostProcessing.NextOfKinFirstName) OrElse
                            String.IsNullOrEmpty(lodPostProcessing.NextOfKinLastName)) Then
                        Return False
                    End If
                End If

                If (Not chk) Then
                    Return False
                End If


                Return True
            End If

            Return False
        End Function

        Private Function IsLODInPostCompletionProcessing() As Boolean
            If (LOD.WorkflowStatus.StatusCodeType.IsFinal AndAlso Not LOD.WorkflowStatus.StatusCodeType.IsCancel) Then
                Return True
            End If

            Return False
        End Function

        Private Function UserHasPostProcessingPermission() As Boolean
            If (LOD.SarcCase AndAlso LOD.IsRestricted) Then
                ' This is a legacy restricted SARC case, so use the Restricted SARC workflow post processing permission...
                Return UserHasPermission(PERMISSION_SARC_POSTPROCESSING)
            Else
                Return UserHasPermission(PERMISSION_EXECUTE_LOD_POST_COMPLETION)
            End If
        End Function

        Private Function GetMemoURL(memoId As Integer) As String

            Return Me.ResolveClientUrl("~/Secure/Shared/Memos/ViewPdf.aspx") +
                                                    "?id=" + RequestId.ToString() +
                                                    "&memo=" + memoId.ToString() +
                                                    "&mod=" + CInt(ModuleType).ToString()

        End Function


#End Region
    End Class
End Namespace

