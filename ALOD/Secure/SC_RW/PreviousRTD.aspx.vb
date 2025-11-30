Imports ALOD.Core.Domain.Documents
Imports ALOD.Core.Domain.Modules.SpecialCases
Imports ALOD.Core.Domain.Workflow
Imports ALOD.Core.Interfaces.DAOInterfaces
Imports ALOD.Data
Imports ALOD.Logging
Imports ALOD.Web.UserControls
Imports ALODWebUtility.Common
Imports ALODWebUtility.Providers

Namespace Web.Special_Case.RW

    Partial Class Secure_sc_rw_PreviousRTD
        Inherits System.Web.UI.Page

#Region "Fields..."

        Private _associatedSC As SpecialCase
        Private _docCatViewDao As IDocCategoryViewDao
        Private _documents As IList(Of ALOD.Core.Domain.Documents.Document)
        Private _documentsDao As IDocumentDao
        Private _factory As IDaoFactory
        Private _memoSource As MemoDao2
        Private _sc As SC_RW = Nothing
        Private _scId As Integer = 0
        Private _specialCaseDao As ISpecialCaseDAO

#End Region

#Region "Properties..."

        Protected ReadOnly Property AssociatedDocumentGroupId As String
            Get
                Dim key As String = "AssociatedDocGroupId"

                If (ViewState(key) Is Nothing) Then
                    If (AssociatedSpecCase Is Nothing) Then
                        Return 0
                    End If

                    If (Not AssociatedSpecCase.DocumentGroupId.HasValue) Then
                        Return 0
                    End If

                    ViewState(key) = AssociatedSpecCase.DocumentGroupId
                End If

                Return CStr(ViewState(key))
            End Get
        End Property

        Protected ReadOnly Property AssociatedDocuments As IList(Of Document)
            Get
                If (_documents Is Nothing) Then
                    If (AssociatedDocumentGroupId > 0) Then
                        _documents = DocumentDao.GetDocumentsByGroupId(AssociatedDocumentGroupId)
                    Else
                        _documents = New List(Of Document)()
                    End If
                End If

                Return _documents
            End Get
        End Property

        Protected ReadOnly Property AssociatedSpecCase As SpecialCase
            Get
                If (_associatedSC Is Nothing) Then
                    If (SpecCase.AssociatedSC.HasValue AndAlso SpecCase.AssociatedSC.Value > 0) Then
                        _associatedSC = SCDao.GetById(SpecCase.AssociatedSC.Value)
                    End If
                End If

                Return _associatedSC
            End Get
        End Property

        Protected ReadOnly Property AssociatedSpecCaseId As Integer
            Get
                If (AssociatedSpecCase Is Nothing) Then
                    Return 0
                End If

                Return AssociatedSpecCase.Id
            End Get
        End Property

        Protected ReadOnly Property DaoFactory() As IDaoFactory
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

        Protected ReadOnly Property DocumentEntityId() As String
            Get
                Dim key As String = "DocEntityId"

                If (ViewState(key) Is Nothing) Then

                    If (SpecCase Is Nothing) Then
                        Return 0
                    End If
                    ViewState(key) = SpecCase.DocumentEntityId
                End If

                Return CStr(ViewState(key))

            End Get
        End Property

        Protected ReadOnly Property DocumentGroupId() As String
            Get
                Dim key As String = "DocGroupId"

                If (ViewState(key) Is Nothing) Then
                    If (SpecCase Is Nothing) Then
                        Return 0
                    End If

                    ViewState(key) = SpecCase.DocumentGroupId
                End If

                Return CStr(ViewState(key))
            End Get
        End Property

        Protected ReadOnly Property DocumentViewId() As Integer
            Get
                Dim key As String = "DocViewId"

                If (ViewState(key) Is Nothing) Then

                    If (SpecCase Is Nothing) Then
                        Return 0
                    End If
                    ViewState(key) = SpecCase.DocumentViewId
                End If

                Return CStr(ViewState(key))

            End Get
        End Property

        Protected ReadOnly Property IndexIncrement() As Integer
            Get
                Return 2
            End Get
        End Property

        Protected ReadOnly Property MasterPage() As SC_RWMaster
            Get
                Dim master As SC_RWMaster = CType(Page.Master, SC_RWMaster)
                Return master
            End Get
        End Property

        Protected Property MemberSSN() As String
            Get
                Dim key As String = "MemberSSN"
                Return CStr(ViewState(key))
            End Get
            Set(ByVal value As String)
                Dim key As String = "MemberSSN"
                ViewState(key) = value
            End Set
        End Property

        Protected ReadOnly Property MemoStore() As MemoDao2
            Get
                If (_memoSource Is Nothing) Then
                    _memoSource = DaoFactory.GetMemoDao2()
                End If
                Return _memoSource
            End Get
        End Property

        Protected ReadOnly Property ModuleType() As ModuleType
            Get
                Return ModuleType.SpecCaseRW
            End Get
        End Property

        Protected ReadOnly Property Navigator() As TabNavigator
            Get
                Return MasterPage.Navigator
            End Get
        End Property

        Protected ReadOnly Property RefId() As Integer
            Get
                _scId = CInt(Request.QueryString("refId"))
                If _scId = 0 Then
                    _scId = Session("refId")
                End If
                Return _scId
            End Get
        End Property

        Protected ReadOnly Property RefIds() As String
            Get
                Return Request.QueryString("refId")
            End Get
        End Property

        Protected ReadOnly Property SCDao() As ISpecialCaseDAO
            Get
                If (_specialCaseDao Is Nothing) Then
                    _specialCaseDao = DaoFactory.GetSpecialCaseDAO()
                End If

                Return _specialCaseDao
            End Get
        End Property

        Protected ReadOnly Property SpecCase() As SC_RW
            Get
                If (_sc Is Nothing) Then
                    _sc = SCDao.GetById(RefId)
                End If

                Return _sc
            End Get
        End Property

        Protected Property UserCanEdit() As Boolean
            Get
                Dim key As String = "CanEdit"
                If (ViewState(key) Is Nothing) Then
                    ViewState(key) = False
                End If
                Return CBool(ViewState(key))
            End Get
            Set(ByVal value As Boolean)
                ViewState("CanEdit") = value
            End Set
        End Property

#End Region

#Region "Page Methods..."

        Protected Sub CategoryRepeater_ItemDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.RepeaterItemEventArgs) Handles CategoryRepeater.ItemDataBound
            If (Not ShouldDatabindCategoryChildRepeater(e)) Then
                Exit Sub
            End If

            Dim cat As DocumentCategory2 = CType(e.Item.DataItem, DocumentCategory2)
            Dim ChildDocRepeater As Repeater = CType(e.Item.FindControl("ChildDocRepeater"), Repeater)

            Dim docs = From d In AssociatedDocuments Where d.DocType = cat.DocCatId Select d Order By d.DateAdded
            ChildDocRepeater.DataSource = docs
            ChildDocRepeater.DataBind()
        End Sub

        Protected Sub ChildDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.RepeaterItemEventArgs)
            If (e.Item.ItemType <> ListItemType.Item AndAlso e.Item.ItemType <> ListItemType.AlternatingItem) Then
                Exit Sub
            End If

            Dim doc As Document = CType(e.Item.DataItem, Document)
            Dim link As HyperLink = CType(e.Item.FindControl("ViewDocLink"), HyperLink)
            Dim url As String = Me.ResolveClientUrl("~/Secure/Shared/DocumentViewer.aspx") +
                                                    "?docId=" + doc.Id.ToString() +
                                                    "&modId=" + AssociatedSpecCase.moduleId.ToString() +
                                                    "&refId=" + AssociatedSpecCase.Id.ToString() +
                                                    "&doc=" + (Server.UrlEncode(Server.HtmlDecode(doc.Description).Replace("'", "")))

            link.Attributes.Add("onclick", "viewDoc('" + url + "'); return false;")
        End Sub

        Protected Sub CreateDocGroupIdIfNecessary()
            If (DocumentGroupId = 0) Then
                SpecCase.CreateDocumentGroup(DocumentDao)
                SCDao.SaveOrUpdate(SpecCase)  'Save the new Document Group ID with the Special Case
            End If
        End Sub

        Protected Sub InitControls()
            If (AssociatedSpecCase IsNot Nothing) Then
                pnlAssociatedDocumentation.Visible = True
                lblAdminSpecialCase.Visible = False

                pnlMemos.Visible = True
                UpdateMemoList()
                UpdateDocumentList()
            Else
                pnlAssociatedDocumentation.Visible = False
                lblAdminSpecialCase.Visible = True
            End If
        End Sub

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
            AddStyleSheet(Page, "~/Styles/Documents.css")

            If (Not IsPostBack) Then
                SetPageAccess()

                MemberSSN = LodCrypto.Encrypt(SpecCase.DocumentEntityId)

                CreateDocGroupIdIfNecessary()
                InitControls()

                LogManager.LogAction(ModuleType, UserAction.ViewPage, RefId, "Viewed Page: Documents")
            End If
        End Sub

        Protected Sub RefreshButton_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles RefreshButton.Click
            UpdateDocumentList()
            UpdateMemoList()
        End Sub

        Protected Sub SetPageAccess()
            'Sys Admin can always upload/delete documents for this case (not Associated LOD Document tab)
            If (UserHasPermission("sysAdmin")) Then
                UserCanEdit = True
                Exit Sub
            End If

            Dim access = (From t In Navigator.PageAccess Where t.PageTitle = "RW Previous RTD" Select t).SingleOrDefault()

            If (access Is Nothing) Then
                UserCanEdit = False
            Else
                If (access.Access = ALOD.Core.Domain.Workflow.PageAccess.AccessLevel.ReadWrite) Then
                    UserCanEdit = True
                Else
                    UserCanEdit = False
                End If
            End If
        End Sub

        Protected Function ShouldDatabindCategoryChildRepeater(ByVal e As System.Web.UI.WebControls.RepeaterItemEventArgs)
            If (e.Item.ItemType <> ListItemType.Item AndAlso e.Item.ItemType <> ListItemType.AlternatingItem) Then
                Return False
            End If

            If (AssociatedSpecCase Is Nothing) Then
                Return False
            End If

            Return True
        End Function

        Protected Sub UpdateDocumentList()
            SpecCase.ProcessDocuments(DaoFactory)

            Dim viewCats As List(Of DocumentCategory2) = DocCatViewDao.GetCategoriesByDocumentViewId(DocumentViewId)
            Dim activeDocs1 = From p In SpecCase.AllDocuments Select p.Key
            Dim categories As IEnumerable(Of DocumentCategory2)

            categories = From p In viewCats Where activeDocs1.Contains(p.DocCatId.ToString())

            CategoryRepeater.DataSource = categories
            CategoryRepeater.DataBind()
        End Sub

        Protected Sub UpdateMemoList()
            If (AssociatedSpecCase Is Nothing) Then
                Exit Sub
            End If

            MemoRepeater.DataSource = From m In MemoStore.GetByRefnModule(AssociatedSpecCase.Id, AssociatedSpecCase.moduleId)
                                      Where (m.Deleted = False)
                                      Select m
                                      Order By m.CreatedDate
            MemoRepeater.DataBind()
        End Sub

#End Region

    End Class

End Namespace