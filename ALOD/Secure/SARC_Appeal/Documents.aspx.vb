Imports ALOD.Core.Domain.Documents
Imports ALOD.Core.Domain.Modules.Lod
Imports ALOD.Core.Domain.Modules.SARC
Imports ALOD.Core.Domain.Workflow
Imports ALOD.Core.Interfaces.DAOInterfaces
Imports ALOD.Data
Imports ALOD.Web.UserControls
Imports ALODWebUtility.Common
Imports ALODWebUtility.Printing

Namespace Web.APSA

    Partial Class Secure_apsa_Documents
        Inherits System.Web.UI.Page

#Region "Fields"

        Private _ap As SARCAppeal
        Private _DocumentDao As IDocumentDao
        Private _factory As IDaoFactory
        Private _lod As LineOfDuty
        Private _LODDao As ILineOfDutyDao
        Private _memoSource As MemoDao
        Private _memoSource2 As MemoDao2
        Private _sarc As RestrictedSARC
        Private _SARCAppealDAO As ISARCAppealDAO
        Private _SARCDao As ISARCDAO

#End Region

#Region "Properties"

        ReadOnly Property APSADao() As ISARCAppealDAO
            Get
                If (_SARCAppealDAO Is Nothing) Then
                    _SARCAppealDAO = New NHibernateDaoFactory().GetSARCAppealDao
                End If

                Return _SARCAppealDAO
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

        ReadOnly Property LODDao() As ILineOfDutyDao
            Get
                If (_LODDao Is Nothing) Then
                    _LODDao = DaoFactory.GetLineOfDutyDao()
                End If

                Return _LODDao
            End Get
        End Property

        ReadOnly Property RequestId() As Integer
            Get
                Return CInt(Request.QueryString("requestId"))
            End Get
        End Property

        ReadOnly Property SARCAppeal() As SARCAppeal
            Get
                If (_ap Is Nothing) Then
                    _ap = APSADao.GetById(RequestId)
                End If
                Return _ap
            End Get
        End Property

        ReadOnly Property SARCDao() As ISARCDAO
            Get
                If (_SARCDao Is Nothing) Then
                    _SARCDao = DaoFactory.GetSARCDao()
                End If

                Return _SARCDao
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

        Protected ReadOnly Property InitModuleType() As Byte
            Get
                If (SARCAppeal.InitialWorkflow = 1) Then
                    Return ModuleType.LOD
                Else
                    Return ModuleType.SARC
                End If
            End Get
        End Property

        Protected ReadOnly Property LOD As LineOfDuty
            Get
                If (SARCAppeal.InitialWorkflow = AFRCWorkflows.LOD) Then

                    If (_lod Is Nothing) Then
                        _lod = LODDao.GetById(SARCAppeal.InitialId)
                    End If
                Else
                    _lod = Nothing
                End If
                Return _lod
            End Get
        End Property

        Protected ReadOnly Property MemoStore2() As MemoDao2
            Get
                If (_memoSource2 Is Nothing) Then
                    _memoSource2 = DaoFactory.GetMemoDao2()
                End If
                Return _memoSource2
            End Get
        End Property

        Protected ReadOnly Property SARC As RestrictedSARC
            Get
                If (SARCAppeal.InitialWorkflow = AFRCWorkflows.SARCRestricted) Then
                    If (_sarc Is Nothing) Then
                        _sarc = SARCDao.GetById(SARCAppeal.InitialId)
                    End If
                Else
                    _sarc = Nothing
                End If

                Return _sarc
            End Get
        End Property

        Protected ReadOnly Property UserCanOnlyViewRedactedDocuments As Boolean
            Get
                Return UserHasPermission(PERMISSION_SARC_VIEW_REDACTED_DOCUMENTS_ONLY)
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

        Protected Sub DisableSARCAppealMemo()
            ViewSARCAppealLink.Visible = False
            IconB.Visible = False
            SARCAppealDescription.Visible = False
        End Sub

        Protected Sub GetForm348()

            Dim perm As MemoGroup = Nothing

            If (SARCAppeal.InitialWorkflow = AFRCWorkflows.SARCRestricted) Then
                If (UserCanOnlyViewRedactedDocuments) Then
                    pnlForm348.Visible = False
                    Exit Sub
                End If

                Dim _documentsSARC As IList(Of Document) = DocumentDao.GetDocumentsByGroupId(SARC.DocumentGroupId)

                Form348_title.Text = "1. AF Form 348-R"
                Description.Text = "AF Form 348-R"

                If (SARC.WorkflowStatus.StatusCodeType.IsFinal) Then
                    ViewDocLink348.Attributes.Add("onclick", "" & ViewForms.RestrictedSARCFormPDFLinkAttribute(SARC, _documentsSARC) & "")
                Else
                    ViewDocLink348.Attributes.Add("onclick", "printForms('" & SARC.Id.ToString() & "', 'SARC');")
                End If
            Else
                Dim _documentsLOD As IList(Of Document) = DocumentDao.GetDocumentsByGroupId(LOD.DocumentGroupId)

                Form348_title.Text = "1 - AFRC Form 348 / DD Form 261 "
                Description.Text = "AFRC Form 348"

                If (LOD.WorkflowStatus.StatusCodeType.IsFinal) Then
                    Dim strAttribute348 As String = ViewForms.LinkAttribute348(LOD.Id.ToString(), _documentsLOD, "lod")
                    ViewDocLink348.Attributes.Add("onclick", "" & strAttribute348 & "return false;")
                Else
                    ViewDocLink348.Attributes.Add("onclick", "printForms('" & LOD.Id.ToString() & "', 'lod'); return false;")
                End If

            End If

        End Sub

        Protected Sub GetSARCAppealMemo()

            If (SARCAppeal IsNot Nothing) Then

                If (SARCAppeal.Status = SARCAppealWorkStatus.Approved OrElse SARCAppeal.Status = SARCAppealWorkStatus.Denied) Then

                    Dim AppealMemo = (From m In MemoStore2.GetByRefnModule(SARCAppeal.InitialId, InitModuleType)
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

        Protected Sub MemoRepeater_ItemDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.RepeaterItemEventArgs) Handles MemoRepeater.ItemDataBound
            If (e.Item.ItemType <> ListItemType.Item AndAlso e.Item.ItemType <> ListItemType.AlternatingItem) Then
                Exit Sub
            End If

            If (LOD IsNot Nothing) Then
                Dim memo As Memorandum = CType(e.Item.DataItem, Memorandum)

                Dim link As HyperLink = CType(e.Item.FindControl("ViewMemoLink"), HyperLink)
                link.Attributes.Add("onclick", "displayMemo('" + GetMemoURL(memo.Id) + "'); return false;")

                Dim perm = (From g In memo.Template.GroupPermissions
                            Where g.Group.Id = CInt(Session("groupId"))
                            Select g).FirstOrDefault()

                If (perm Is Nothing OrElse Not perm.CanView) Then
                    e.Item.Visible = False
                    Exit Sub
                End If
            Else
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
            End If

        End Sub

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

            AddStyleSheet(Page, "~/Styles/Documents.css")

            If (Not IsPostBack) Then

                GetDocuments()
                GetForm348()
                GetSARCAppealMemo()
                UpdateMemoList()

            End If

        End Sub

        Protected Sub UpdateMemoList()

            If (SARCAppeal.InitialWorkflow = AFRCWorkflows.LOD) Then
                MemoRepeater.DataSource = From m In MemoStore.GetByRefId(SARCAppeal.InitialId)
                                          Where m.Deleted = False AndAlso Not (m.Template.Id = MemoType.SARC_APPEAL_APPROVED OrElse m.Template.Id = MemoType.SARC_APPEAL_DISAPPROVAL)
                                          Select m
                                          Order By m.CreatedDate

                MemoRepeater.DataBind()
            Else
                MemoRepeater.DataSource = From m In MemoStore2.GetByRefnModule(SARCAppeal.InitialId, ModuleType.SARC)
                                          Where m.Deleted = False AndAlso Not (m.Template.Id = MemoType.SARC_APPEAL_APPROVED OrElse m.Template.Id = MemoType.SARC_APPEAL_DISAPPROVAL)
                                          Select m
                                          Order By m.CreatedDate

                MemoRepeater.DataBind()
            End If

        End Sub

        Private Sub GetDocuments()

            If (SARCAppeal.DocumentGroupId Is Nothing OrElse SARCAppeal.DocumentGroupId = 0) Then
                SARCAppeal.CreateDocumentGroup(DocumentDao)
                APSADao.SaveOrUpdate(SARCAppeal)
                APSADao.CommitChanges()
            End If

            SARCAppeal.ProcessDocuments(DaoFactory)
            Documents.Initialize(Me, Navigator, New WorkflowDocument(SARCAppeal, 3))

        End Sub

        Private Function GetMemoURL(memoId As Integer) As String

            Dim InitialId As Integer = 0
            If (LOD IsNot Nothing) Then
                InitialId = LOD.Id
            Else
                InitialId = SARC.Id
            End If

            Return Me.ResolveClientUrl("~/Secure/Shared/Memos/ViewPdf.aspx") +
                                                    "?id=" + InitialId.ToString() +
                                                    "&memo=" + memoId.ToString() +
                                                    "&mod=" + CInt(InitModuleType).ToString()

        End Function

    End Class

End Namespace