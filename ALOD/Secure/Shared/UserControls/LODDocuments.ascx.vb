Imports ALOD.Core.Domain.Documents
Imports ALOD.Core.Domain.Modules.Appeals
Imports ALOD.Core.Domain.Modules.Lod
Imports ALOD.Core.Domain.Modules.SARC
Imports ALOD.Core.Domain.Workflow
Imports ALOD.Core.Interfaces.DAOInterfaces
Imports ALOD.Data
Imports ALOD.Data.Services
Imports ALODWebUtility.Common
Imports ALODWebUtility.Printing

Namespace Web.UserControls

    Partial Class LODDocuments
        Inherits System.Web.UI.UserControl

#Region "Fields"

        Private _dao As IDocumentDao
        Private _factory As IDaoFactory
        Private _lod As LineOfDuty
        Private _lodAppeal As LODAppeal
        Private _LODAppealDAO As ILODAppealDAO
        Private _LODDao As ILineOfDutyDao
        Private _lodId As Integer = 0
        Private _memoSource As MemoDao
        Private _navigator As TabNavigator
        Private _page As Page
        Private _sarcAppeal As SARCAppeal
        Private _SARCAppealDAO As ISARCAppealDAO
        Private HasAdminLOD As Boolean = False

#End Region

#Region "Page Properties"

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

        ReadOnly Property DocumentDao() As IDocumentDao
            Get
                If (_dao Is Nothing) Then
                    _dao = New SRXDocumentStore(CStr(HttpContext.Current.Session("UserName")))
                End If

                Return _dao
            End Get
        End Property

        ReadOnly Property LOD() As LineOfDuty
            Get
                If (_lod Is Nothing) Then
                    _lod = LodService.GetById(Lod_Id)
                End If
                Return _lod
            End Get
        End Property

        ReadOnly Property LODAppeal() As LODAppeal
            Get

                If (_lodAppeal Is Nothing And Lod_Id <> 0) Then
                    Dim _appealId = APDao.GetAppealIdByInitLod(Lod_Id)

                    If _appealId <> 0 Then
                        _lodAppeal = APDao.GetById(_appealId)
                    Else
                        _lodAppeal = Nothing
                    End If
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

        ReadOnly Property SARCAppeal() As SARCAppeal
            Get

                If (_sarcAppeal Is Nothing) Then
                    Dim _sarcId = APSADao.GetAppealIdByInitId(LOD.Id, LOD.Workflow)

                    If _sarcId <> 0 Then
                        _sarcAppeal = APSADao.GetById(_sarcId)
                    Else
                        _sarcAppeal = Nothing
                    End If
                End If
                Return _sarcAppeal
            End Get
        End Property

        Public Property Lod_Id() As Integer
            Get
                Return _lodId
            End Get
            Set(value As Integer)
                _lodId = value
            End Set
        End Property

        Protected ReadOnly Property DaoFactory As IDaoFactory
            Get
                If (_factory Is Nothing) Then
                    _factory = New NHibernateDaoFactory()
                End If

                Return _factory
            End Get
        End Property

        Protected ReadOnly Property ModuleType() As ModuleType
            Get
                Return ModuleType.LOD
            End Get
        End Property

        Private Property HostPage() As Page
            Get
                Return _page
            End Get
            Set(value As Page)
                _page = value
            End Set
        End Property

        Private ReadOnly Property MemoStore() As MemoDao
            Get
                If (_memoSource Is Nothing) Then
                    _memoSource = New NHibernateDaoFactory().GetMemoDao()
                End If
                Return _memoSource
            End Get
        End Property

        Private Property Navigator() As TabNavigator
            Get
                Return _navigator
            End Get
            Set(value As TabNavigator)
                _navigator = value
            End Set
        End Property

#End Region

#Region "Page Methods"

        Public Sub Initialize(host As Page, LODId As Integer, AdminLOD As Boolean, Nav As TabNavigator)
            Lod_Id = LODId
            HasAdminLOD = AdminLOD
            Navigator = Nav
            HostPage = host

            ScriptManager.GetCurrent(HostPage).RegisterAsyncPostBackControl(Me)
        End Sub

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

        End Sub

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

            AddStyleSheet(Page, "~/Styles/Documents.css")

            If (Lod_Id = 0) Then
                NonAdminDocs.Visible = False
                If HasAdminLOD = 0 Then
                    AdminDocs.InnerText = "  No Associated LOD."
                End If
                AdminDocs.Visible = True
            Else
                If (Not IsPostBack) Then

                    GetDocuments()

                    GetForm348()

                    GetLODAppealMemo()

                    GetSARCAppealMemo()

                    UpdateMemoList()

                End If
            End If

        End Sub

        Protected Sub UpdateMemoList()

            Dim AppealMemos As IList(Of MemoType) = New List(Of MemoType)
            AppealMemos.Add(MemoType.ApprovalAppeal)
            AppealMemos.Add(MemoType.DisapprovalAppeal)
            AppealMemos.Add(MemoType.SARC_APPEAL_APPROVED)
            AppealMemos.Add(MemoType.SARC_APPEAL_DISAPPROVAL)

            MemoRepeater.DataSource = From m In MemoStore.GetByRefId(Lod_Id)
                                      Where m.Deleted = False AndAlso Not AppealMemos.Contains(m.Template.Id)
                                      Select m
                                      Order By m.CreatedDate

            MemoRepeater.DataBind()

        End Sub

        Private Sub GetDocuments()

            If (LOD.DocumentGroupId Is Nothing OrElse LOD.DocumentGroupId = 0) Then
                LOD.CreateDocumentGroup(DocumentDao)
                LODDao.SaveOrUpdate(LOD)
                LODDao.CommitChanges()
            End If

            Documents.Initialize(HostPage, Navigator, New WorkflowDocument(LOD, 3, DaoFactory))

        End Sub

        Private Sub GetForm348()

            If (LOD.WorkflowStatus.StatusCodeType.IsFinal) Then
                Dim _Documents = DocumentDao.GetDocumentsByGroupId(LOD.DocumentGroupId)
                Dim strAttribute348 As String = ViewForms.LinkAttribute348(Lod_Id.ToString(), _Documents, "lod")
                ViewDocLink348.Attributes.Add("onclick", "" & strAttribute348 & "return false;")
            Else
                ViewDocLink348.Attributes.Add("onclick", "printForms('" & Lod_Id.ToString() & "', 'lod'); return false;")
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
            Else
                DisableAppealMemo()
            End If

        End Sub

        Private Function GetMemoURL(memoId As Integer) As String

            Dim url = HostPage.ResolveClientUrl("~/Secure/Shared/Memos/ViewPdf.aspx") +
                                                    "?id=" + LOD.Id.ToString() +
                                                    "&memo=" + memoId.ToString() +
                                                    "&mod=" + CInt(ModuleType).ToString()

            Return url

        End Function

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
                    ViewSARCAppealLink.Attributes.Add("onClick", "displayMemo('" + GetMemoURL(AppealMemo.Id) + "'); return false;")
                Else
                    DisableSARCAppealMemo()
                End If
            Else
                DisableSARCAppealMemo()
            End If

        End Sub

#End Region

    End Class

End Namespace