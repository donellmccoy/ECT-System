Imports ALOD.Core.Domain.Documents
Imports ALOD.Core.Domain.Modules.Lod
Imports ALOD.Core.Domain.Modules.SpecialCases
Imports ALOD.Core.Domain.Workflow
Imports ALOD.Core.Interfaces.DAOInterfaces
Imports ALOD.Data
Imports ALOD.Logging
Imports ALOD.Web.UserControls
Imports ALODWebUtility.Common

Namespace Web.Special_Case.AGR

    Partial Class Secure_sc_agr_Documents
        Inherits System.Web.UI.Page

#Region "Fields"

        Private _dao As IDocumentDao
        Private _dispositionDao As ILookupDispositionDao
        Private _factory As IDaoFactory
        Private _memoSource As MemoDao2
        Private _sc As SC_AGRCert
        Private _specialCase As SC_AGRCert
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
                If (_dao Is Nothing) Then
                    _dao = New SRXDocumentStore(CStr(HttpContext.Current.Session("UserName")))
                End If

                Return _dao
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

        Protected ReadOnly Property ModuleType() As ModuleType
            Get
                Return ModuleType.SpecCaseAGR
            End Get
        End Property

        Protected ReadOnly Property RequestId() As Integer
            Get
                Return CInt(Request.QueryString("refId"))
            End Get
        End Property

        Protected ReadOnly Property SC() As SC_AGRCert
            Get
                If (_sc Is Nothing) Then
                    _sc = SCDao.GetById(RequestId)
                End If

                Return _sc
            End Get
        End Property

        Protected ReadOnly Property SpecCase() As SpecialCase
            Get
                If (_specialCase Is Nothing) Then
                    _specialCase = SCDao.GetById(RequestId, False)

                End If

                Return _specialCase
            End Get
        End Property

        Private ReadOnly Property DispositionDao() As ILookupDispositionDao
            Get
                If (_dispositionDao Is Nothing) Then
                    _dispositionDao = DaoFactory.GetLookupDispositionDao()
                End If

                Return _dispositionDao
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

#Region "Page Methods"

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
            End If
        End Sub

        Protected Sub MemoRepeater_ItemDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.RepeaterItemEventArgs) Handles MemoRepeater.ItemDataBound
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

            Dim edit As Image = CType(e.Item.FindControl("EditMemo"), Image)
            Dim delete As Image = CType(e.Item.FindControl("DeleteMemo"), Image)

            If (perm IsNot Nothing) Then
                edit.Visible = perm.CanEdit
                delete.Visible = perm.CanDelete

                If (perm.CanEdit) Then
                    edit.Attributes.Add("onClick", "showEditor(" + memo.Id.ToString() + "," + memo.Template.Id.ToString() + "," + memo.ModuleId.ToString() + ");")
                End If
            Else
                CType(e.Item.FindControl("DeleteMemo"), Image).Visible = False
                CType(e.Item.FindControl("EditMemo"), Image).Visible = False
            End If

        End Sub

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
            AddStyleSheet(Page, "~/Styles/Documents.css")

            If (Not IsPostBack) Then
                GetDocuments()
                SetCurrentMemoTemplate()
                InitMemoCreation()
                UpdateMemoList()
            End If
        End Sub

        Protected Sub RefreshButton_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles RefreshButton.Click
            UpdateMemoList()
        End Sub

        Protected Sub UpdateMemoList()
            Dim modType As Integer = ModuleType
            MemoRepeater.DataSource = From m In MemoStore.GetByRefnModule(RequestId, modType)
                                      Where (m.Deleted = False)
                                      Select m
                                      Order By m.CreatedDate
            MemoRepeater.DataBind()
            CheckMemoList()
        End Sub

        '    Return Nothing
        'End Function
        Private Function CanCreateMemos() As Boolean
            If Not (SpecCase.Status = SpecCaseAGRWorkStatus.FinalReview Or SpecCase.Status = SpecCaseAGRWorkStatus.LocalFinalReview) AndAlso SC.GetActiveMemoTemplateId().HasValue Then
                Return False
            End If

            Dim templates = From m In MemoStore.GetAllTemplates()
                            Where m.Component = Session("Compo") _
                            And (m.ModuleType = ModuleType)
                            Select m

            Dim template = (From t In templates Select t Where t.Id = SC.GetActiveMemoTemplateId()).FirstOrDefault()

            Return HasPermission(template)
        End Function

        Private Sub CheckMemoList()
            If (Not CanCreateMemos()) Then
                CreateMemo.Visible = False
                CreateMemo2.Visible = False
                Exit Sub
            End If

            Dim memoList As IList(Of Memorandum2)
            memoList = (From l In MemoStore.GetByRefnModule(RequestId, ModuleType)
                        Where l.Deleted = False And (l.Template.Id = MemoType.AGR_Approved_HQ_FON Or
                                                        l.Template.Id = MemoType.AGR_Approved_HQ_INIT Or
                                                        l.Template.Id = MemoType.AGR_Approved_WING_FON Or
                                                        l.Template.Id = MemoType.AGR_Approved_WING_INIT Or
                                                        l.Template.Id = MemoType.AGR_Certiication_Denied)
                        Select l).ToList

            Dim memoDisqual As IList(Of Memorandum2)
            memoDisqual = (From l In MemoStore.GetByRefnModule(RequestId, ModuleType)
                           Select l).ToList

            If (Not Documents.UserCanEdit Or memoList.Count > 0) Then
                CreateMemo.Visible = False
            Else
                CreateMemo.Visible = True
            End If
            If (Not Documents.UserCanEdit Or memoDisqual.Count > 0) Then
                CreateMemo2.Visible = False
            End If
        End Sub

        Private Sub GetDocuments()
            'make sure we have a groupId
            If (SpecCase.DocumentGroupId Is Nothing OrElse SpecCase.DocumentGroupId = 0) Then
                SpecCase.CreateDocumentGroup(DocumentDao)
                SCDao.SaveOrUpdate(SpecCase)  'Save the new Document Group ID with the Special Case
                SCDao.CommitChanges()
            End If

            SpecCase.ProcessDocuments(DaoFactory)
            Documents.Initialize(Me, Navigator, New WorkflowDocument(SC, 2, DaoFactory))
        End Sub

        Private Function GetMemoURL(memoId As Integer) As String
            Return Me.ResolveClientUrl("~/Secure/Shared/Memos/ViewPdf.aspx") +
                                                    "?id=" + RequestId.ToString() +
                                                    "&memo=" + memoId.ToString() +
                                                    "&mod=" + CInt(ModuleType).ToString()
        End Function

        'Protected Function GetCaseSelectedMemoTemplate(ByVal templates As IEnumerable(Of MemoTemplate)) As MemoTemplate
        '    Select Case SpecCase.Status
        '        Case SpecCaseAGRWorkStatus.FinalReview
        '            If (SC.GetActiveMemoTemplateId().HasValue AndAlso SC.GetActiveMemoTemplateId().Value > 0) Then
        '                Return (From t In templates Select t Where t.Id = SC.GetActiveMemoTemplateId().Value).FirstOrDefault()
        '            End If
        '    End Select
        Private Function HasPermission(template As MemoTemplate) As Boolean
            If (template IsNot Nothing) Then
                Dim perms = (From g In template.GroupPermissions Where g.Group.Id = CInt(Session("groupId")) Select g).FirstOrDefault()
                If (perms IsNot Nothing AndAlso perms.CanCreate) Then
                    Return True
                End If
            End If

            Return False
        End Function

        Private Sub InitMemoCreation()

            If (Not CanCreateMemos()) Then
                CreateMemo.Visible = False
                CreateMemo2.Visible = False
                Exit Sub
            End If

            Dim templates = From t In MemoStore.GetAllTemplates()
                            Where t.Component = Session("Compo") And (t.ModuleType = ModuleType)
                            Select t

            Dim templateId As Integer = 0
            Dim templateId2 As Integer = 0
            Dim template As MemoTemplate = Nothing

            ' Get which memo(s) have been selected...
            Select Case SC.Status
                ' Wing Initiate -> Wing Close out
                Case SpecCaseAGRWorkStatus.LocalFinalReview
                    'Wing follow on
                    If ((SC.ShouldUseWingMedicalReviewerindings() = 1 Or SC.GetActiveMemoTemplateId() = MemoType.AGR_Approved_WING_FON)) Then
                        template = (From t In templates Select t Where t.Id = SC.GetActiveMemoTemplateId()).FirstOrDefault()
                        CreateMemo.ToolTip = "Create AGR Follow On RMU Memo"

                        'Wing Initial
                    ElseIf ((SC.ShouldUseWingMedicalReviewerindings() = 1 Or SC.GetActiveMemoTemplateId() = MemoType.AGR_Approved_WING_INIT)) Then
                        template = (From t In templates Select t Where t.Id = SC.GetActiveMemoTemplateId()).FirstOrDefault()
                        CreateMemo.ToolTip = "Create AGR Inital RMU Memo"

                        ' Denial
                    ElseIf (SC.ShouldUseWingMedicalReviewerindings() = 0) Or (SC.GetActiveMemoTemplateId() = MemoType.AGR_Certiication_Denied) Then
                        template = (From t In templates Select t Where t.Id = SC.GetActiveMemoTemplateId()).FirstOrDefault()
                        CreateMemo.ToolTip = "Create AGR Denial Memo"
                    End If

                    'HQ Initiate -> HQ Close out
                Case SpecCaseAGRWorkStatus.FinalReview
                    'HQ follow on
                    If ((SC.ShouldUseSeniorMedicalReviewerFindings() = 1 Or SC.GetActiveMemoTemplateId() = MemoType.AGR_Approved_HQ_FON)) Then
                        template = (From t In templates Select t Where t.Id = SC.GetActiveMemoTemplateId()).FirstOrDefault()
                        CreateMemo.ToolTip = "Create AGR Follow On HQ Memo"

                        'HQ Initial
                    ElseIf ((SC.ShouldUseSeniorMedicalReviewerFindings() = 1 Or SC.GetActiveMemoTemplateId() = MemoType.AGR_Approved_HQ_INIT)) Then
                        template = (From t In templates Select t Where t.Id = SC.GetActiveMemoTemplateId()).FirstOrDefault()
                        CreateMemo.ToolTip = "Create AGR Inital HQ Memo"

                    ElseIf (SC.ShouldUseSeniorMedicalReviewerFindings() = 0) Or (SC.GetActiveMemoTemplateId() = MemoType.AGR_Certiication_Denied) Then
                        template = (From t In templates Select t Where t.Id = SC.GetActiveMemoTemplateId()).FirstOrDefault()
                        CreateMemo.ToolTip = "Create AGR Denial Memo"

                    End If
            End Select

            'if the user doesn't have access, hide the create button
            If (template.Id = 0) Then
                CreateMemo.Visible = False
            Else
                CreateMemo.Attributes.Add("onclick", "showEditor(0," + CInt(template.Id).ToString() + ", " + CInt(ModuleType).ToString() + ");")
            End If

            If (templateId2 = 0) Then
                CreateMemo2.Visible = False
            Else
                CreateMemo2.Attributes.Add("onclick", "showEditor(0," + CInt(templateId2).ToString() + ", " + CInt(ModuleType).ToString() + ");")
            End If

        End Sub

        Private Sub SetCurrentMemoTemplate()
            Select Case SC.Status
            ' If WorkStatus in LocalFinalReview Then
                '    If ALC is 0 and MAJCOM is 0 (WING, Initial Tour)
                '       If InitialTour Is 1
                '           Set Active Template to
                Case SpecCaseAGRWorkStatus.LocalFinalReview
                    If (SC.ALC.HasValue And SC.ALC = 0 And SC.MAJCOM.HasValue And SC.MAJCOM = 0) Then
                        If (SC.InitialTour.HasValue And SC.InitialTour = 1 And SC.med_off_approved.HasValue And SC.med_off_approved = 1) Then
                            SC.MemoTemplateID = MemoType.AGR_Approved_WING_INIT
                        ElseIf (SC.FollowOnTour.HasValue And SC.FollowOnTour = 1 And SC.med_off_approved.HasValue And SC.med_off_approved = 1) Then
                            SC.MemoTemplateID = MemoType.AGR_Approved_WING_FON
                        Else
                            SC.MemoTemplateID = MemoType.AGR_Certiication_Denied
                        End If
                    End If

                    ' If WorkStatus in FinalReview Then
                    '    If ALC is 1 or MAJCOM is 1 (HQ, Initial Tour)
                    '       If InitialTour Is 1
                    '           Set Active Template to
                Case SpecCaseAGRWorkStatus.FinalReview
                    If (SC.ALC IsNot Nothing And SC.ALC = 1 Or SC.MAJCOM IsNot Nothing And SC.MAJCOM = 1) Then
                        If ((SC.InitialTour IsNot Nothing And SC.InitialTour = 1 And SC.hqt_approval1 IsNot Nothing And SC.hqt_approval1 = 1) Or
                            (SC.InitialTour IsNot Nothing And SC.InitialTour = 1 And SC.SeniorMedicalReviewerApproved IsNot Nothing And SC.SeniorMedicalReviewerApproved = 1)) Then
                            SC.MemoTemplateID = MemoType.AGR_Approved_HQ_INIT
                        ElseIf ((SC.FollowOnTour IsNot Nothing And SC.FollowOnTour = 1 And SC.hqt_approval1 IsNot Nothing And SC.hqt_approval1 = 1) Or
                                (SC.FollowOnTour IsNot Nothing And SC.FollowOnTour = 1 And SC.SeniorMedicalReviewerApproved IsNot Nothing And SC.SeniorMedicalReviewerApproved = 1)) Then
                            SC.MemoTemplateID = MemoType.AGR_Approved_HQ_FON
                        Else
                            SC.MemoTemplateID = MemoType.AGR_Certiication_Denied
                        End If
                    End If

            End Select
            SCDao.SaveOrUpdate(SC)
            SCDao.CommitChanges()
        End Sub

#End Region

    End Class

End Namespace