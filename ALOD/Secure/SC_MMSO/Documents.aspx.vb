Imports ALOD.Core.Domain.Documents
Imports ALOD.Core.Domain.Modules.SpecialCases
Imports ALOD.Core.Domain.Workflow
Imports ALOD.Core.Interfaces.DAOInterfaces
Imports ALOD.Data
Imports ALOD.Logging
Imports ALOD.Web.UserControls
Imports ALODWebUtility.Common
Imports ALODWebUtility.Providers

Namespace Web.Special_Case.MMSO

    Partial Class Secure_sc_mmso_Documents
        Inherits System.Web.UI.Page

        Private Const DATE_FORMAT As String = "your_date_format_here"

        'e.g., "dd-MM-yyyy"
        Private Const VIEWED_PAGE_MESSAGE As String = "Viewed Page: Documents"

        Private _dao As IDocumentDao
        Private _docCatViewDao As IDocCategoryViewDao
        Private _documents As IList(Of ALOD.Core.Domain.Documents.Document)
        Private _factory As IDaoFactory
        Private _memoSource As MemoDao2
        Dim dao As ISpecialCaseDAO
        Private sc As SC_MMSO = Nothing
        Private scId As Integer = 0

        ReadOnly Property DocCatViewDao() As IDocCategoryViewDao
            Get
                If (_docCatViewDao Is Nothing) Then
                    _docCatViewDao = factory.GetDocCategoryViewDao()
                End If

                Return _docCatViewDao
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

        ReadOnly Property DocumentEntityId() As String
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

        ReadOnly Property DocumentGroupId() As String
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

        ReadOnly Property DocumentViewId() As Integer
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

        ReadOnly Property factory() As IDaoFactory
            Get
                If (_factory Is Nothing) Then
                    _factory = New NHibernateDaoFactory()
                End If

                Return _factory
            End Get
        End Property

        ReadOnly Property RefId() As Integer
            Get
                scId = CInt(Request.QueryString("refId"))
                If scId = 0 Then
                    scId = Session("refId")
                End If
                Return scId
            End Get
        End Property

        ReadOnly Property RefIds() As String
            Get
                Return Request.QueryString("refId")
            End Get
        End Property

        ReadOnly Property SCDao() As ISpecialCaseDAO
            Get
                If (dao Is Nothing) Then
                    dao = factory.GetSpecialCaseDAO()
                End If

                Return dao
            End Get
        End Property

        Public ReadOnly Property Navigator() As TabNavigator
            Get
                Return MasterPage.Navigator
            End Get
        End Property

        Public Property UserCanEdit() As Boolean
            Get
                If (ViewState("UserCanEdit") Is Nothing) Then
                    ViewState("UserCanEdit") = False
                End If
                Return CBool(ViewState("UserCanEdit"))
            End Get
            Set(value As Boolean)
                ViewState("UserCanEdit") = value
            End Set
        End Property

        Protected ReadOnly Property CalendarImage() As String
            Get
                Return Me.ResolveClientUrl("~/App_Themes/" + Page.Theme + "/Images/Calendar.gif")
            End Get
        End Property

        Protected ReadOnly Property MasterPage() As SC_MMSOMaster
            Get
                Dim master As SC_MMSOMaster = CType(Page.Master, SC_MMSOMaster)
                Return master
            End Get
        End Property

        Protected ReadOnly Property SpecCase() As SC_MMSO
            Get
                If (sc Is Nothing) Then
                    sc = SCDao.GetById(RefIds)
                End If

                Return sc
            End Get
        End Property

        Private Property MemberSSN() As String
            Get
                Dim key As String = "MemberSSN"
                Return CStr(ViewState(key))
            End Get
            Set(ByVal value As String)
                Dim key As String = "MemberSSN"
                ViewState(key) = value
            End Set
        End Property

        Private ReadOnly Property MemoStore() As MemoDao2
            Get
                If (_memoSource Is Nothing) Then
                    _memoSource = factory.GetMemoDao2()
                End If
                Return _memoSource
            End Get
        End Property

        Protected Sub CategoryRepeater_ItemDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.RepeaterItemEventArgs) Handles CategoryRepeater.ItemDataBound

            If (e.Item.ItemType <> ListItemType.Item AndAlso e.Item.ItemType <> ListItemType.AlternatingItem) Then
                Exit Sub
            End If
            Dim cat As DocumentCategory2 = CType(e.Item.DataItem, DocumentCategory2)

            If (SpecCase.Required.ContainsKey(cat.DocCatId.ToString())) Then
                Dim lblReqd As Label = CType(e.Item.FindControl("lblReqd"), Label)
                lblReqd.Text = "Required"
            End If

            If (UserCanEdit) Then
                Dim url As String = ResolveClientUrl("~/Secure/Shared/CustomDocumentUpload.aspx") +
                                                     "?group=" + DocumentGroupId.ToString() +
                                                     "&id=" + CStr(cat.DocCatId) +
                                                     "&entity=" + Server.UrlEncode(MemberSSN)

                Dim UploadImage = CType(e.Item.FindControl("UploadImage"), Image)
                UploadImage.Attributes.Add("onclick", "uploadDoc('" + url + "');")
                UploadImage.AlternateText = Server.HtmlEncode("Add " + cat.CategoryDescription + " document")
            Else
                CType(e.Item.FindControl("UploadImage"), Image).Visible = False
            End If

            If (_documents Is Nothing) Then
                _documents = DocumentDao.GetDocumentsByGroupId(DocumentGroupId)
            End If

            Dim ChildDocRepeater As Repeater = CType(e.Item.FindControl("ChildDocRepeater"), Repeater)

            Dim docs = From d In _documents Where d.DocType = cat.DocCatId Select d Order By d.DateAdded
            ChildDocRepeater.DataSource = docs
            ChildDocRepeater.DataBind()

        End Sub

        Protected Sub ChildDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.RepeaterItemEventArgs)

            If (e.Item.ItemType <> ListItemType.Item AndAlso e.Item.ItemType <> ListItemType.AlternatingItem) Then
                Exit Sub
            End If

            Dim modId As Integer = ModuleType.SpecCaseMMSO

            Dim doc As Document = CType(e.Item.DataItem, Document)

            Dim link As HyperLink = CType(e.Item.FindControl("ViewDocLink"), HyperLink)
            Dim url As String = Me.ResolveClientUrl("~/Secure/Shared/DocumentViewer.aspx") +
                                                    "?docId=" + doc.Id.ToString() +
                                                    "&modId=" + modId.ToString() +
                                                    "&refId=" + RefId.ToString() +
                                                    "&doc=" + (Server.UrlEncode(Server.HtmlDecode(doc.Description).Replace("'", "")))

            link.Attributes.Add("onclick", "viewDoc('" + url + "'); return false;")

            If (UserCanEdit) Then

                Dim edit As Image = CType(e.Item.FindControl("EditDocument"), Image)

                Dim x As String = Server.HtmlDecode(doc.Description).Replace(ControlChars.Quote, "''").Replace("'", "\'")
                If (doc.UploadedBy Like Session("UserName")) Then
                    edit.Attributes.Add("onclick", "editDocument(" + doc.Id.ToString() + "," _
                                                    + CStr(doc.DocType) + "," _
                                                    + "'" + doc.DocDate.ToString(DATE_FORMAT) + "'," _
                                                    + "'" + Server.HtmlEncode(x) + "'" _
                                                    + ");") '
                Else
                    edit.Visible = False
                    CType(e.Item.FindControl("DeleteDocument"), Image).Visible = False
                    CType(e.Item.FindControl("LockedDocument"), Image).Visible = True
                End If
            Else
                'this is read only
                CType(e.Item.FindControl("EditDocument"), Image).Visible = False
                CType(e.Item.FindControl("DeleteDocument"), Image).Visible = False
                CType(e.Item.FindControl("LockedDocument"), Image).Visible = True
            End If

        End Sub

        Protected Sub Document_ItemCommand(ByVal source As Object, ByVal e As System.Web.UI.WebControls.RepeaterCommandEventArgs)

            If (e.CommandName = "DeleteDocument") Then
                Dim parts() As String = e.CommandArgument.ToString().Split("|")
                Dim docId As Integer = Integer.Parse(parts(0))
                DocumentDao.DeleteDocument(docId)
                LogManager.LogAction(ModuleType.SpecCaseMEB, UserAction.DocumentDeleted, RefId, "Deleted: " + parts(1))
                UpdateDocumentList()
            End If

        End Sub

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

            AddStyleSheet(Page, "~/Styles/Documents.css")

            If (Not IsPostBack) Then
                Dim userName As String = If(Session("UserName"), String.Empty)

                UserCanEdit = GetAccess(Navigator.PageAccess, True)

                MemberSSN = LodCrypto.Encrypt(SpecCase.DocumentEntityId)

                'make sure we have a groupId
                If DocumentGroupId = 0 Then
                    SpecCase.CreateDocumentGroup(DocumentDao)
                    SCDao.SaveOrUpdate(SpecCase)  'Save the new Document Group ID with the Special Case
                End If

                UpdateDocumentList()

                'LogManager.LogAction(ModuleType.SpecCaseMEB, UserAction.ViewPage, RefId, "Viewed Page: Documents")
                LogManager.LogAction(ModuleType.SpecCaseMEB, UserAction.ViewPage, RefId, VIEWED_PAGE_MESSAGE)

            End If

        End Sub

        Protected Sub RefreshButton_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles RefreshButton.Click
            UpdateDocumentList()
        End Sub

        Protected Sub UpdateDocumentButton_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles UpdateDocumentButton.Click

            Dim values() As String = DocumentEditValues.Text.Split("|")
            Dim desc As String = SanitizeInput(values(3))

            If values.Length <> 4 Then
                Exit Sub
            End If

            Dim docId As Integer
            Dim docType As Integer
            Dim docDate As DateTime

            Try
                docId = Integer.Parse(values(0))
                docType = Integer.Parse(values(1))
                docDate = DateTime.Parse(values(2))
                desc = values(3)
            Catch ex As Exception
                LogManager.LogError(ex)
                Exit Sub
            End Try

            If (docId = 0) Then
                Exit Sub
            End If

            'we only allow a limited character set in document names, so filter out anything not in that list
            Dim regex As Regex = New Regex(
                   "([a-zA-Z0-9 ])*",
                RegexOptions.IgnoreCase _
               Or RegexOptions.Singleline _
              Or RegexOptions.CultureInvariant _
             Or RegexOptions.Compiled
             )

            Dim ms As MatchCollection = regex.Matches(desc)
            Dim buffer As New StringBuilder()

            For Each match As Match In ms
                buffer.Append(match.ToString())
            Next

            desc = Server.HtmlEncode(buffer.ToString())

            'get the old values for comparison
            Dim docs As List(Of Document) = DocumentDao.GetDocumentsByGroupId(DocumentGroupId)
            Dim before As Document = (From d In docs Where d.Id = docId Select d).FirstOrDefault()
            Dim changes As New ChangeSet()

            If (docType <> before.DocType) Then
                changes.Add("Details", "Document Type", before.DocTypeName, CType(docType, DocumentType).ToString())
            End If

            If (DateTime.Compare(docDate, before.DocDate) <> 0) Then
                changes.Add("Details", "Document Date", before.DocDate.ToString(DATE_FORMAT), docDate.ToString(DATE_FORMAT))
            End If

            If (desc <> before.Description) Then
                changes.Add("Details", "Description", before.Description, desc)
            End If

            If (changes.Count > 0) Then
                DocumentDao.UpdateDocumentDetails(docId, desc, docDate, DocumentEntityId, DocumentStatus.Pending, docType)
                Dim logId As Integer = LogManager.LogAction(ModuleType.SpecCaseMEB, UserAction.EditDocument, RefId, "Edited: " + desc)
                changes.Save(logId)
                UpdateDocumentList()
            End If

        End Sub

        Protected Sub UpdateDocumentList()
            'update documents
            SpecCase.ProcessDocuments(factory)
            Dim viewCats As List(Of DocumentCategory2) = DocCatViewDao.GetCategoriesByDocumentViewId(DocumentViewId)
            Dim activeDocs1 = From p In SpecCase.AllDocuments Select p.Key

            Dim categories = From p In viewCats Where activeDocs1.Contains(p.DocCatId.ToString())

            CategoryRepeater.DataSource = categories
            CategoryRepeater.DataBind()

            DocumentTypeSelect.DataSource = categories
            DocumentTypeSelect.DataTextField = "CategoryDescription"
            DocumentTypeSelect.DataValueField = "DocCatId"
            DocumentTypeSelect.DataBind()

        End Sub

        Private Function SanitizeInput(ByVal input As String) As String
            ' Simple function to sanitize input - expand as needed
            Dim regex As Regex = New Regex("([a-zA-Z0-9 ])*", RegexOptions.IgnoreCase Or RegexOptions.Singleline Or RegexOptions.CultureInvariant Or RegexOptions.Compiled)
            Dim ms As MatchCollection = regex.Matches(input)
            Dim buffer As New StringBuilder()

            For Each match As Match In ms
                buffer.Append(match.ToString())
            Next

            Return Server.HtmlEncode(buffer.ToString())
        End Function

    End Class

End Namespace