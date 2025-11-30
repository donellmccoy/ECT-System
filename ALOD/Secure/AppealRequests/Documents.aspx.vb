Imports ALOD.Core.Domain.Documents
Imports ALOD.Core.Domain.Modules.Appeals
Imports ALOD.Core.Domain.Modules.Lod
Imports ALOD.Core.Domain.Workflow
Imports ALOD.Core.Interfaces.DAOInterfaces
Imports ALOD.Data
Imports ALOD.Web.UserControls
Imports ALODWebUtility.Common
Imports ALODWebUtility.Printing

Namespace Web.AP

    Partial Class Secure_ap_Documents
        Inherits System.Web.UI.Page

#Region "Fields"

        Private _ap As LODAppeal
        Private _docCatDao As IDocCategoryViewDao
        Private _DocumentDao As IDocumentDao
        Private _DocumentDao2 As IDocumentDao
        Private _factory As IDaoFactory
        Private _lod As LineOfDuty
        Private _LODAppealDAO As ILODAppealDAO
        Private _LODDao As ILineOfDutyDao
        Private _memoSource As MemoDao

#End Region

#Region "Properties"

        ReadOnly Property APDao() As ILODAppealDAO
            Get
                If (_LODAppealDAO Is Nothing) Then
                    _LODAppealDAO = DaoFactory.GetLODAppealDao()
                End If

                Return _LODAppealDAO
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
                    _lod = LODDao.GetById(LODAppeal.InitialLodId)
                End If
                Return _lod
            End Get
        End Property

        ReadOnly Property LODAppeal() As LODAppeal
            Get

                If (_ap Is Nothing) Then
                    _ap = APDao.GetById(RequestId)
                End If
                Return _ap
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

        Public ReadOnly Property ModuleTypeLOD() As ModuleType
            Get
                Return ModuleType.LOD
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

        Private ReadOnly Property MemoStore() As MemoDao
            Get
                If (_memoSource Is Nothing) Then
                    _memoSource = New NHibernateDaoFactory().GetMemoDao()
                End If
                Return _memoSource
            End Get
        End Property

#End Region

#Region "Page Methods"

        Protected Sub DisableAppealMemo()
            ViewAppealLink.Visible = False
            IconA.Visible = False
            AppealDescription.Visible = False
        End Sub

        Protected Sub GetForm348()

            If (LOD.WorkflowStatus.StatusCodeType.IsFinal) Then
                Dim _Documents As IList(Of Document) = DocumentDao.GetDocumentsByGroupId(LOD.DocumentGroupId)
                Dim strAttribute348 As String = ViewForms.LinkAttribute348(LOD.Id.ToString(), _Documents, "lod")
                ViewDocLink348.Attributes.Add("onclick", "" & strAttribute348 & "return false;")
            Else
                ViewDocLink348.Attributes.Add("onclick", "printForms('" & LOD.Id.ToString() & "', 'lod'); return false;")
            End If

            If (LOD.Formal) AndAlso (LOD.LODInvestigation IsNot Nothing) Then
                Description.Text = "AFRC Form 348 and DD Form 261"
            End If

        End Sub

        Protected Sub GetLODAppealMemo()

            If (LODAppeal.Status = AppealWorkStatus.AppealApproved OrElse LODAppeal.Status = AppealWorkStatus.AppealDenied) Then

                Dim AppealMemo = (From m In MemoStore.GetByRefId(LOD.Id)
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
                ViewAppealLink.Attributes.Add("onClick", "displayMemo('" + GetMemoURL(AppealMemo.Id) + "'); return false;")
            Else
                DisableAppealMemo()
            End If

        End Sub

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
            End If

        End Sub

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

            AddStyleSheet(Page, "~/Styles/Documents.css")

            If (Not IsPostBack) Then

                GetDocuments()
                GetForm348()
                GetLODAppealMemo()
                UpdateMemoList()

            End If

        End Sub

        Protected Sub UpdateMemoList()

            Dim AppealMemos As IList(Of MemoType) = New List(Of MemoType)
            AppealMemos.Add(MemoType.ApprovalAppeal)
            AppealMemos.Add(MemoType.DisapprovalAppeal)

            MemoRepeater.DataSource = From m In MemoStore.GetByRefId(LODAppeal.InitialLodId)
                                      Where m.Deleted = False AndAlso Not AppealMemos.Contains(m.Template.Id)
                                      Select m
                                      Order By m.CreatedDate

            MemoRepeater.DataBind()

        End Sub

        Private Sub GetDocuments()

            If (LODAppeal.DocumentGroupId Is Nothing OrElse LODAppeal.DocumentGroupId = 0) Then
                LODAppeal.CreateDocumentGroup(DocumentDao)
                APDao.SaveOrUpdate(LODAppeal)
                APDao.CommitChanges()
            End If

            Documents.Initialize(Me, Navigator, New WorkflowDocument(LODAppeal, 3, DaoFactory))

        End Sub

        Private Function GetMemoURL(memoId As Integer) As String

            Dim url = Me.ResolveClientUrl("~/Secure/Shared/Memos/ViewPdf.aspx") +
                                                    "?id=" + LOD.Id.ToString() +
                                                    "&memo=" + memoId.ToString() +
                                                    "&mod=" + CInt(ModuleTypeLOD).ToString()

            Return url

        End Function

#End Region

    End Class

End Namespace