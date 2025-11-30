Imports ALOD.Core.Domain.Documents
Imports ALOD.Core.Domain.Modules.Appeals
Imports ALOD.Core.Domain.Modules.Lod
Imports ALOD.Core.Domain.Modules.Reinvestigations
Imports ALOD.Core.Domain.Modules.SARC
Imports ALOD.Core.Domain.Workflow
Imports ALOD.Core.Interfaces.DAOInterfaces
Imports ALOD.Data
Imports ALOD.Web.UserControls
Imports ALODWebUtility.Common
Imports ALODWebUtility.Printing

Namespace Web.RR

    Partial Class Secure_rr_Documents
        Inherits System.Web.UI.Page

#Region "Fields"

        Private _APDao As ILODAppealDAO
        Private _APSADao As ISARCAppealDAO
        Private _DocumentDao As IDocumentDao
        Private _factory As IDaoFactory
        Private _lod As LineOfDuty
        Private _lodAppeal As LODAppeal
        Private _LODDao As ILineOfDutyDao
        Private _memoSource As MemoDao2
        Private _reinvestigation As LODReinvestigation
        Private _RRDao As ILODReinvestigateDAO
        Private _sarcAppeal As SARCAppeal

#End Region

        ReadOnly Property APDao() As ILODAppealDAO
            Get
                If (_APDao Is Nothing) Then
                    _APDao = DaoFactory.GetLODAppealDao()
                End If

                Return _APDao
            End Get
        End Property

        ReadOnly Property APSADao() As ISARCAppealDAO
            Get
                If (_APSADao Is Nothing) Then
                    _APSADao = DaoFactory.GetSARCAppealDao()
                End If

                Return _APSADao
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

        ReadOnly Property LOD() As LineOfDuty
            Get
                If (_lod Is Nothing) Then
                    _lod = LODDao.GetById(RR.InitialLodId)
                End If
                Return _lod
            End Get
        End Property

        ReadOnly Property LODAppeal() As LODAppeal
            Get
                Dim AppealId = APDao.GetAppealIdByInitLod(LOD.Id)
                If (_lodAppeal Is Nothing AndAlso AppealId > 0) Then
                    _lodAppeal = APDao.GetById(AppealId)
                End If
                Return _lodAppeal
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

        ReadOnly Property RR() As LODReinvestigation
            Get
                If (_reinvestigation Is Nothing) Then
                    _reinvestigation = RRDao.GetById(RequestId)
                End If
                Return _reinvestigation

            End Get
        End Property

        ReadOnly Property RRDao() As ILODReinvestigateDAO
            Get
                If (_RRDao Is Nothing) Then
                    _RRDao = DaoFactory.GetLODReinvestigationDao()
                End If

                Return _RRDao
            End Get
        End Property

        ReadOnly Property SARCAppeal() As SARCAppeal
            Get
                Dim AppealId = APSADao.GetAppealIdByInitId(LOD.Id, LOD.Workflow)
                If (_sarcAppeal Is Nothing AndAlso AppealId > 0) Then
                    _sarcAppeal = APSADao.GetById(AppealId)
                End If
                Return _sarcAppeal
            End Get
        End Property

        Public ReadOnly Property ModuleType() As ModuleType
            Get
                Return ModuleType.ReinvestigationRequest
            End Get
        End Property

        Public ReadOnly Property Navigator() As TabNavigator
            Get
                Return Master.Navigator
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

        Protected ReadOnly Property RequestId() As Integer
            Get
                Return CInt(Request.QueryString("requestId"))
            End Get
        End Property

        Private ReadOnly Property MemoStore() As MemoDao2
            Get
                If (_memoSource Is Nothing) Then
                    _memoSource = New NHibernateDaoFactory().GetMemoDao2()
                End If
                Return _memoSource
            End Get
        End Property

        Protected Sub DisableAppealMemo()
            ViewAppealLink.Visible = False
            IconA.Visible = False
            AppealDescription.Visible = False
        End Sub

        Protected Sub DisableSARCAppealMemo()
            ViewSARCAppealLink.Visible = False
            IconB.Visible = False
            SARCAppealDescription.Visible = False
        End Sub

        Protected Sub GetForm348()

            If (LOD.WorkflowStatus.StatusCodeType.IsFinal) Then
                Dim _Documents As IList(Of Document) = DocumentDao.GetDocumentsByGroupId(LOD.DocumentGroupId)
                Dim strAttribute348 As String = ViewForms.LinkAttribute348(RR.InitialLodId.ToString(), _Documents, "reinvestigation")
                ViewDocLink348.Attributes.Add("onclick", "" & strAttribute348 & "")
            Else
                ViewDocLink348.Attributes.Add("onclick", "printForms('" & RR.InitialLodId.ToString() & "', 'lod'); return false;")
            End If

            If (LOD.Formal) AndAlso (LOD.LODInvestigation IsNot Nothing) Then
                Description.Text = "AFRC Form 348 and DD Form 261"
            End If
        End Sub

        Protected Sub MemoRepeater_ItemDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.RepeaterItemEventArgs) Handles MemoRepeater.ItemDataBound
            If (e.Item.ItemType <> ListItemType.Item AndAlso e.Item.ItemType <> ListItemType.AlternatingItem) Then
                Exit Sub
            End If

            Dim memo As Memorandum2 = CType(e.Item.DataItem, Memorandum2)

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

        End Sub

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

            AddStyleSheet(Page, "~/Styles/Documents.css")

            If (Not IsPostBack) Then

                GetDocuments()
                GetForm348()
                GetLODAppealMemo()
                GetSARCAppealMemo()
                UpdateMemoList()

            End If

        End Sub

        Protected Sub UpdateMemoList()

            Dim AppealMemos As IList(Of MemoType) = New List(Of MemoType)
            AppealMemos.Add(MemoType.ApprovalAppeal)
            AppealMemos.Add(MemoType.DisapprovalAppeal)
            AppealMemos.Add(MemoType.SARC_APPEAL_APPROVED)
            AppealMemos.Add(MemoType.SARC_APPEAL_DISAPPROVAL)

            MemoRepeater.DataSource = From m In MemoStore.GetByRefnModule(RequestId, ModuleType)
                                      Where m.Deleted = False AndAlso Not AppealMemos.Contains(m.Template.Id)
                                      Select m
                                      Order By m.CreatedDate

            MemoRepeater.DataBind()

        End Sub

        Private Sub GetDocuments()

            If (RR.DocumentGroupId Is Nothing OrElse RR.DocumentGroupId = 0) Then
                RR.CreateDocumentGroup(DocumentDao)
                RRDao.SaveOrUpdate(RR)
                RRDao.CommitChanges()
            End If

            RR.ProcessDocuments(DaoFactory)
            Documents.Initialize(Me, Navigator, New WorkflowDocument(RR, 3, DaoFactory))

        End Sub

        Private Sub GetLODAppealMemo()

            If (LODAppeal IsNot Nothing) Then
                If (LODAppeal.CurrentStatusCode = AppealStatusCode.AppealApproved OrElse LODAppeal.CurrentStatusCode = AppealStatusCode.AppealDenied) Then

                    Dim AppealMemo = (From m In MemoStore.GetByRefnModule(RequestId, ModuleType)
                                      Where m.Deleted = False AndAlso (m.Template.Id = MemoType.ApprovalAppeal OrElse m.Template.Id = MemoType.DisapprovalAppeal)
                                      Select m
                                      Order By m.CreatedDate Descending).FirstOrDefault

                    If (AppealMemo Is Nothing) Then
                        DisableAppealMemo()
                        RowAppeal.Visible = True
                        AppealMemoError.Visible = True
                        Exit Sub
                    End If

                    Dim perm = (From g In AppealMemo.Template.GroupPermissions
                                Where g.Group.Id = CInt(Session("groupId"))
                                Select g).FirstOrDefault()

                    If (perm Is Nothing OrElse Not perm.CanView) Then
                        DisableAppealMemo()
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
                    DisableAppealMemo()
                End If
            Else
                DisableAppealMemo()
            End If

        End Sub

        Private Function GetMemoURL(memoId As Integer) As String

            Return Me.ResolveClientUrl("~/Secure/Shared/Memos/ViewPdf.aspx") +
                                                    "?id=" + RequestId.ToString() +
                                                    "&memo=" + memoId.ToString() +
                                                    "&mod=" + CInt(ModuleType).ToString()

        End Function

        Private Sub GetSARCAppealMemo()

            If (SARCAppeal IsNot Nothing) Then
                If (SARCAppeal.Status = SARCAppealWorkStatus.Approved OrElse SARCAppeal.Status = SARCAppealWorkStatus.Denied) Then

                    Dim AppealMemo = (From m In MemoStore.GetByRefnModule(RequestId, ModuleType)
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

    End Class

End Namespace