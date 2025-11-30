Imports ALOD.Core.Domain.Documents
Imports ALOD.Core.Domain.Modules.Lod
Imports ALOD.Core.Domain.Modules.SpecialCases
Imports ALOD.Core.Domain.Workflow
Imports ALOD.Core.Interfaces.DAOInterfaces
Imports ALOD.Data
Imports ALOD.Logging
Imports ALOD.Web.UserControls
Imports ALODWebUtility.Common

Namespace Web.Special_Case.RW

    Partial Class Secure_sc_rw_Documents
        Inherits System.Web.UI.Page

#Region "Fields..."

        Private _documentDao As IDocumentDao
        Private _factory As IDaoFactory
        Private _memoSource As MemoDao2
        Private _specCase As SC_RW
        Private _specialCaseDao As ISpecialCaseDAO

#End Region

#Region "Properties..."

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
                If (_documentDao Is Nothing) Then
                    _documentDao = New SRXDocumentStore(CStr(HttpContext.Current.Session("UserName")))
                End If

                Return _documentDao
            End Get
        End Property

        ReadOnly Property SpecCaseDao() As ISpecialCaseDAO
            Get
                If (_specialCaseDao Is Nothing) Then
                    _specialCaseDao = DaoFactory.GetSpecialCaseDAO()
                End If

                Return _specialCaseDao
            End Get
        End Property

        Public ReadOnly Property Navigator() As TabNavigator
            Get
                Return Master.Navigator
            End Get
        End Property

        Protected ReadOnly Property ModuleType() As ModuleType
            Get
                Return ModuleType.SpecCaseRW
            End Get
        End Property

        Protected ReadOnly Property RequestId() As Integer
            Get
                Return CInt(Request.QueryString("refId"))
            End Get
        End Property

        Protected ReadOnly Property SpecCase() As SC_RW
            Get
                If (_specCase Is Nothing) Then
                    _specCase = SpecCaseDao.GetById(RequestId)
                End If

                Return _specCase
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

#Region "Page Methods..."

        Protected Function DoesUserHaveMemoCreatePermission(ByVal template As MemoTemplate) As Boolean
            If (template Is Nothing) Then
                Return False
            End If

            Dim perms = (From g In template.GroupPermissions Where g.Group.Id = CInt(Session("groupId")) Select g).FirstOrDefault()

            If (perms Is Nothing OrElse Not perms.CanCreate) Then
                Return False
            End If

            Return True
        End Function

        Protected Function GetAllRWMemoTemplates() As IEnumerable(Of MemoTemplate)
            Return (From t In MemoStore.GetAllTemplates()
                    Where t.Component = Session("Compo") And (t.ModuleType = ModuleType)
                    Select t)
        End Function

        Protected Function GetCaseSelectedMemoTemplate(ByVal templates As IEnumerable(Of MemoTemplate)) As MemoTemplate
            Select Case SpecCase.Status
                Case SpecCaseRWWorkStatus.FinalReview
                    If (SpecCase.GetActiveMemoTemplateId().HasValue AndAlso SpecCase.GetActiveMemoTemplateId().Value > 0) Then
                        Return (From t In templates Select t Where t.Id = SpecCase.GetActiveMemoTemplateId().Value).FirstOrDefault()
                    End If
            End Select

            Return Nothing
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
                    edit.Attributes.Add("onClick", "showEditor(" + memo.Id.ToString() + "," + memo.Template.Id.ToString() + ");")
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
                InitMemoCreation()
                UpdateMemoList()
            End If
        End Sub

        Protected Sub RefreshButton_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles RefreshButton.Click
            UpdateMemoList()
        End Sub

        Protected Sub UpdateMemoList()
            MemoRepeater.DataSource = From m In MemoStore.GetByRefnModule(RequestId, ModuleType)
                                      Where (m.Deleted = False)
                                      Select m
                                      Order By m.CreatedDate
            MemoRepeater.DataBind()

            CheckMemoList()
        End Sub

        Private Sub CheckMemoList()
            If (Not Documents.UserCanEdit) Then
                CreateMemo.Visible = False
                Exit Sub
            End If

            Dim rwMemos As IList(Of Memorandum2)
            rwMemos = (From m In MemoStore.GetByRefnModule(RequestId, ModuleType)
                       Where m.Deleted = False AndAlso m.CreatedDate >= SpecCase.CreatedDate
                       Select m).ToList()

            Dim templates = From m In MemoStore.GetAllTemplates()
                            Where m.Component = Session("Compo") _
                            And (m.ModuleType = ModuleType)
                            Select m

            Dim template = (From t In templates Select t Where t.Id = SpecCase.GetActiveMemoTemplateId()).FirstOrDefault()

            If ((rwMemos IsNot Nothing AndAlso rwMemos.Count > 0) OrElse Not DoesUserHaveMemoCreatePermission(template)) Then
                CreateMemo.Visible = False
            Else
                CreateMemo.Visible = True
            End If
        End Sub

        Private Sub GetDocuments()
            If (SpecCase.DocumentGroupId Is Nothing OrElse SpecCase.DocumentGroupId = 0) Then
                SpecCase.CreateDocumentGroup(DocumentDao)
                SpecCaseDao.SaveOrUpdate(SpecCase)  'Save the new Document Group ID with the Special Case
                SpecCaseDao.CommitChanges()
            End If

            SpecCase.ProcessDocuments(DaoFactory)
            Documents.Initialize(Me, Navigator, New WorkflowDocument(SpecCase, 2, DaoFactory))
        End Sub

        Private Function GetMemoToolTipText(ByVal templateId As Integer) As String
            If (templateId = MemoType.RW_4_Non_RW_C) Then
                Return "Create RW Renewal 4 (Non RW-C)"
            ElseIf (templateId = MemoType.RW_4_2_RTD) Then
                Return "Create RW Renewal 4-2 RTD"
            ElseIf (templateId = MemoType.RW_Admin_LOD_AFPC) Then
                Return "Create RW Renewal Admin LOD AFPC"
            ElseIf (templateId = MemoType.RW_HIV) Then
                Return "Create RW Renewal HIV"
            ElseIf (templateId = MemoType.RW_RTD_SAF) Then
                Return "Create RW Renewal RTD SAF"
            ElseIf (templateId = MemoType.RW_RTD_SG) Then
                Return "Create RW Renewal RTD SG"
            ElseIf (templateId = MemoType.RW_DQ_MEB) Then
                Return "Create RW Renewal DQ Letter MEB"
            ElseIf (templateId = MemoType.RW_DQ_WD) Then
                Return "Create RW Renewal DQ Letter WD"
            ElseIf (templateId = MemoType.RW_DQ_Pending_LOD) Then
                Return "Create RW Renewal DQ Pending LOD"
            Else
                Return "Create RW Memo"
            End If
        End Function

        Private Function GetMemoURL(memoId As Integer) As String
            Return Me.ResolveClientUrl("~/Secure/Shared/Memos/ViewPdf.aspx") +
                                                    "?id=" + RequestId.ToString() +
                                                    "&memo=" + memoId.ToString() +
                                                    "&mod=" + CInt(ModuleType).ToString()
        End Function

        Private Sub InitMemoCreation()
            Dim rwTemplateId As Integer = 0
            Dim templates As IEnumerable(Of MemoTemplate) = GetAllRWMemoTemplates()
            Dim rwTemplate As MemoTemplate = GetCaseSelectedMemoTemplate(templates)

            If (DoesUserHaveMemoCreatePermission(rwTemplate)) Then
                rwTemplateId = rwTemplate.Id
            End If

            If (rwTemplateId = 0) Then
                CreateMemo.Visible = False
            Else
                CreateMemo.Attributes.Add("onclick", "showEditor(0," + rwTemplateId.ToString() + ", " + CInt(ModuleType).ToString() + ");")
                CreateMemo.ToolTip = GetMemoToolTipText(rwTemplateId)
            End If
        End Sub

#End Region

    End Class

End Namespace