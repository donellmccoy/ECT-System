Imports ALOD.Core.Domain.Common.KeyVal
Imports ALOD.Core.Domain.Documents
Imports ALOD.Core.Domain.Workflow
Imports ALOD.Core.Interfaces.DAOInterfaces
Imports ALOD.Core.Utils.RegexValidation
Imports ALOD.Data
Imports ALOD.Logging
Imports ALODWebUtility.Common
Imports ALODWebUtility.Providers

Namespace Web.Help

    Partial Class Secure_help_Documentation
        Inherits System.Web.UI.Page

#Region "Data Members"

        Public Const HELP_MISC_KEY_NAME As String = "Help Miscellaneous Links"
        Public Const HELP_VIDEO_KEY_NAME As String = "Help Video Links"
        Private _docCatViewDao As IDocCategoryViewDao
        Private _docDao As IDocumentDao
        Private _documents As IList(Of ALOD.Core.Domain.Documents.Document)
        Private _docViewDao As IDocumentViewDao
        Private _keyValDao As IKeyValDao
        Private _permissionDao As IALODPermissionDao
        Private _viewHelpDocPerm As ALODPermission

#End Region

#Region "Properties"

        ReadOnly Property DocCatViewDao() As IDocCategoryViewDao
            Get
                If (_docCatViewDao Is Nothing) Then
                    _docCatViewDao = New NHibernateDaoFactory().GetDocCategoryViewDao()
                End If

                Return _docCatViewDao
            End Get
        End Property

        ReadOnly Property DocumentDao() As IDocumentDao
            Get
                If (_docDao Is Nothing) Then
                    _docDao = New SRXDocumentStore(CStr(HttpContext.Current.Session("UserName")))
                End If

                Return _docDao
            End Get
        End Property

        Property DocumentEntity() As String
            Get
                Dim key As String = "DocEntity"
                Return CStr(ViewState(key))
            End Get
            Set(ByVal value As String)
                Dim key As String = "DocEntity"
                ViewState(key) = value
            End Set
        End Property

        ReadOnly Property DocumentEntityId() As String
            Get
                Return "HelpDocumentsEntity"
            End Get
        End Property

        ReadOnly Property DocumentGroupId() As Integer
            Get
                Return PermissionDao.GetDocGroupIdByPermId(ViewHelpDocPermission.Id)
            End Get
        End Property

        ReadOnly Property DocumentViewDao() As IDocumentViewDao
            Get
                If (_docViewDao Is Nothing) Then
                    _docViewDao = New NHibernateDaoFactory().GetDocumentViewDao()
                End If

                Return _docViewDao
            End Get
        End Property

        ReadOnly Property DocumentViewId() As Integer
            Get
                Return DocumentViewDao.GetIdByDescription("Help Documents")
            End Get
        End Property

        ReadOnly Property KeyValDao() As IKeyValDao
            Get
                If (_keyValDao Is Nothing) Then
                    _keyValDao = New NHibernateDaoFactory().GetKeyValDao()
                End If

                Return _keyValDao
            End Get
        End Property

        ReadOnly Property PermissionDao() As IALODPermissionDao
            Get
                If (_permissionDao Is Nothing) Then
                    _permissionDao = New NHibernateDaoFactory().GetPermissionDao()
                End If

                Return _permissionDao
            End Get
        End Property

        ReadOnly Property ViewHelpDocPermission() As ALODPermission
            Get
                If (_viewHelpDocPerm Is Nothing) Then
                    Dim lst As IList(Of ALODPermission) = PermissionDao.GetAll().ToList()
                    If (lst.Count > 0) Then
                        _viewHelpDocPerm = (From p In lst Where p.Name = "viewHelpDocs" Select p).First()
                    End If
                End If

                Return _viewHelpDocPerm
            End Get
        End Property

        Public ReadOnly Property CanUserEdit() As Boolean
            Get
                If (UserHasPermission("editHelpDocs")) Then
                    Return True
                Else
                    Return False
                End If
            End Get
        End Property

        Public ReadOnly Property CanUserView() As Boolean
            Get
                If (UserHasPermission("viewHelpDocs")) Then
                    Return True
                Else
                    Return False
                End If
            End Get
        End Property

        Protected ReadOnly Property CalendarImage() As String
            Get
                Return Me.ResolveClientUrl("~/App_Themes/" + Page.Theme + "/Images/Calendar.gif")
            End Get
        End Property

        Protected ReadOnly Property VideoImage() As String
            Get
                Return Me.ResolveClientUrl("~/App_Themes/" + Page.Theme + "/Images/wmv_icon.ico")
            End Get
        End Property

#End Region

#Region "Methods"

        Protected Sub CategoryRepeater_ItemDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.RepeaterItemEventArgs) Handles CategoryRepeater.ItemDataBound

            If (e.Item.ItemType <> ListItemType.Item AndAlso e.Item.ItemType <> ListItemType.AlternatingItem) Then
                Exit Sub
            End If
            Dim cat As DocumentCategory2 = CType(e.Item.DataItem, DocumentCategory2)

            If (CanUserEdit) Then
                Dim url As String = ResolveClientUrl("~/Secure/Shared/DocumentUpload.aspx") +
                                                     "?group=" + DocumentGroupId.ToString() +
                                                     "&id=" + CStr(cat.DocCatId) +
                                                     "&entity=" + Server.UrlEncode(DocumentEntity)

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

            Dim modId As Integer = ModuleType.System

            Dim doc As Document = CType(e.Item.DataItem, Document)

            Dim link As HyperLink = CType(e.Item.FindControl("ViewDocLink"), HyperLink)
            Dim url As String = Me.ResolveClientUrl("~/Secure/Shared/DocumentViewer.aspx") +
                                                    "?docId=" + doc.Id.ToString() +
                                                    "&modId=" + modId.ToString() +
                                                    "&doc=" + (Server.UrlEncode(Server.HtmlDecode(doc.Description).Replace("'", "")))

            link.Attributes.Add("onclick", "viewDoc('" + url + "'); return false;")

            If (CanUserEdit) Then

                Dim edit As Image = CType(e.Item.FindControl("EditDocument"), Image)

                Dim x As String = Server.HtmlDecode(doc.Description).Replace(ControlChars.Quote, "''").Replace("'", "\'")
                If (doc.UploadedBy Like Session("UserName")) Then
                    edit.Attributes.Add("onclick", "editDocument(" + doc.Id.ToString() + "," _
                                                    + "'" + doc.DocType.ToString() + "'," _
                                                    + "'" + doc.DocDate.ToString(DATE_FORMAT) + "'," _
                                                    + "'" + Server.HtmlEncode(x) + "'" _
                                                    + ");") '
                    '+ CStr(doc.DocType) + "," _
                Else
                    edit.Visible = False
                    CType(e.Item.FindControl("DeleteDocument"), Image).Visible = True
                    CType(e.Item.FindControl("LockedDocument"), Image).Visible = False
                End If
            Else
                'this is read only
                CType(e.Item.FindControl("EditDocument"), Image).Visible = True
                CType(e.Item.FindControl("DeleteDocument"), Image).Visible = True
                CType(e.Item.FindControl("LockedDocument"), Image).Visible = False
            End If

        End Sub

        Protected Sub Document_ItemCommand(ByVal source As Object, ByVal e As System.Web.UI.WebControls.RepeaterCommandEventArgs)

            If (e.CommandName = "DeleteDocument") Then
                Dim parts() As String = e.CommandArgument.ToString().Split("|")
                Dim docId As Integer = Integer.Parse(parts(0))
                DocumentDao.DeleteDocument(docId)
                LogManager.LogAction(ModuleType.System, UserAction.DocumentDeleted, "Deleted: " + parts(1))
                UpdateDocumentList()
            End If

        End Sub

        Protected Sub KeyVal_ItemCommand(ByVal source As Object, ByVal e As System.Web.UI.WebControls.RepeaterCommandEventArgs)

            If (e.CommandName = "DeleteKeyVal") Then
                Dim parts() As String = e.CommandArgument.ToString().Split("|")
                Dim id As Integer

                If (Integer.TryParse(parts(0), id)) Then
                    KeyValDao.DeleteKeyValueById(id)
                    LogManager.LogAction(ModuleType.System, UserAction.KeyValDeleted, "Deleted KeyVal Value with a description of " + parts(1) + ".")
                    UpdateDocumentList()
                End If
            End If

        End Sub

        Protected Sub MiscLinksDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.RepeaterItemEventArgs)

            If (e.Item.ItemType <> ListItemType.Item AndAlso e.Item.ItemType <> ListItemType.AlternatingItem) Then
                Exit Sub
            End If

            Dim value As KeyValValue = CType(e.Item.DataItem, KeyValValue)

            If (CanUserEdit) Then

                Dim edit As Image = CType(e.Item.FindControl("EditMiscLink"), Image)

                Dim x As String = Server.HtmlDecode(value.ValueDescription).Replace(ControlChars.Quote, "''").Replace("'", "\'")

                edit.Attributes.Add("onclick", "editKeyVal(" + value.Id.ToString() + "," _
                    + "'" + value.Key.Id.ToString() + "'," _
                    + "'" + Server.HtmlEncode(x) + "'," _
                    + "'" + Server.HtmlDecode(value.Value) + "'" _
                    + ");")
            Else
                'this is read only
                CType(e.Item.FindControl("EditMiscLink"), Image).Visible = True
                CType(e.Item.FindControl("DeleteMiscLink"), Image).Visible = True
            End If

        End Sub

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

            AddStyleSheet(Page, "~/Styles/Documents.css")

            AddHandler ucEditKeyVal.KeyValValueEdited, AddressOf UpdateDocumentList
            AddHandler ucAddKeyVal.KeyValValueAdded, AddressOf UpdateDocumentList

            If (Not IsPostBack) Then

                DocumentEntity = LodCrypto.Encrypt(DocumentEntityId)

                ' Make sure a groupId exists for the viewHelpDocs permission
                If (DocumentGroupId = 0) Then
                    Dim newDocId As Integer = DocumentDao.CreateGroup()

                    If (newDocId > 0) Then
                        PermissionDao.InsertNewDocGroup(ViewHelpDocPermission.Id, newDocId)
                    End If

                End If

                UpdateDocumentList()

            End If

        End Sub

        Protected Sub RefreshButton_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles RefreshButton.Click

            UpdateDocumentList()

        End Sub

        Protected Sub UpdateDocumentButton_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles UpdateDocumentButton.Click

            Dim values() As String = DocumentEditValues.Text.Split("|")

            If values.Length <> 4 Then
                Exit Sub
            End If

            Dim docId As Integer

            'Dim docType As Integer
            Dim docTypeStr As String
            Dim docDate As DateTime
            Dim desc As String

            Try
                docId = Integer.Parse(values(0))
                'docType = Integer.Parse(values(1))
                docTypeStr = values(1)
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
            Dim ms As MatchCollection = GetMatchesDocumentDesc(desc)
            Dim buffer As New StringBuilder()

            For Each match As Match In ms
                buffer.Append(match.ToString())
            Next

            desc = Server.HtmlEncode(buffer.ToString())

            'get the old values for comparison
            Dim docs As List(Of Document) = DocumentDao.GetDocumentsByGroupId(DocumentGroupId)
            Dim before As Document = (From d In docs Where d.Id = docId Select d).FirstOrDefault()
            Dim changes As New ChangeSet()

            If (docTypeStr <> before.DocType.ToString()) Then
                'changes.Add("Details", "Document Type", before.DocTypeName, CType(docTypeStr, DocumentType).ToString())
                changes.Add("Details", "Document Type", before.DocTypeName, docTypeStr)
            End If

            If (DateTime.Compare(docDate, before.DocDate) <> 0) Then
                changes.Add("Details", "Document Date", before.DocDate.ToString(DATE_FORMAT), docDate.ToString(DATE_FORMAT))
            End If

            If (desc <> before.Description) Then
                changes.Add("Details", "Description", before.Description, desc)
            End If

            If (changes.Count > 0) Then
                DocumentDao.UpdateDocumentDetails(docId, desc, docDate, DocumentEntityId, DocumentStatus.Pending, [WorkingEnum](Of DocumentType).Parse(docTypeStr))
                Dim logId As Integer = LogManager.LogAction(ModuleType.System, UserAction.EditDocument, "Edited: " + desc)
                changes.Save(logId)
                UpdateDocumentList()
            End If

        End Sub

        Protected Sub UpdateDocumentList()
            Dim viewCats As List(Of DocumentCategory2) = DocCatViewDao.GetCategoriesByDocumentViewId(DocumentViewId)

            CategoryRepeater.DataSource = From p In viewCats Where p.CategoryDescription <> "Miscellaneous"
            CategoryRepeater.DataBind()

            VideoRepeater.DataSource = KeyValDao.GetKeyValuesByKeyDesciption(HELP_VIDEO_KEY_NAME)
            VideoRepeater.DataBind()

            MiscLinksRepeater.DataSource = KeyValDao.GetKeyValuesByKeyDesciption(HELP_MISC_KEY_NAME)
            MiscLinksRepeater.DataBind()

            DocumentTypeSelect.DataSource = From p In viewCats Where p.CategoryDescription <> "Miscellaneous"
            DocumentTypeSelect.DataTextField = "CategoryDescription"
            DocumentTypeSelect.DataValueField = "DocCatId"
            DocumentTypeSelect.DataBind()

            ucEditKeyVal.FillKeySelectControl(KeyValDao.GetHelpKeys())
            ucAddKeyVal.FillKeySelectControl(KeyValDao.GetHelpKeys())

            InitializeKeyValUploadLinks()

        End Sub

        Protected Sub VideoDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.RepeaterItemEventArgs)

            If (e.Item.ItemType <> ListItemType.Item AndAlso e.Item.ItemType <> ListItemType.AlternatingItem) Then
                Exit Sub
            End If

            Dim value As KeyValValue = CType(e.Item.DataItem, KeyValValue)

            If (CanUserEdit) Then

                Dim edit As Image = CType(e.Item.FindControl("EditVideo"), Image)

                Dim x As String = Server.HtmlDecode(value.ValueDescription).Replace(ControlChars.Quote, "''").Replace("'", "\'")

                edit.Attributes.Add("onclick", "editKeyVal(" + value.Id.ToString() + "," _
                    + "'" + value.Key.Id.ToString() + "'," _
                    + "'" + Server.HtmlEncode(x) + "'," _
                    + "'" + Server.HtmlDecode(value.Value) + "'" _
                    + ");")
            Else
                'this is read only
                CType(e.Item.FindControl("EditVideo"), Image).Visible = True
                CType(e.Item.FindControl("DeleteVideo"), Image).Visible = True
            End If

        End Sub

        Private Sub InitializeKeyValUploadLinks()
            If (CanUserEdit) Then
                Dim helpKeys As List(Of KeyValKey) = KeyValDao.GetHelpKeys()
                Dim keyId As Integer = (From p In helpKeys Where p.Description = HELP_VIDEO_KEY_NAME).First().Id

                ' Set the attributes for the video related image
                Dim UploadImage = CType(imgUploadVidKeyVal, Image)
                UploadImage.Attributes.Add("onclick", "addKeyVal(" + keyId.ToString() + ");")
                UploadImage.AlternateText = Server.HtmlEncode("Add Video Link")

                keyId = (From p In helpKeys Where p.Description = HELP_MISC_KEY_NAME).First().Id

                ' Set the attributes for the misc link related image
                UploadImage = CType(imgUploadMiscLinkValue, Image)
                UploadImage.Attributes.Add("onclick", "addKeyVal(" + keyId.ToString() + ");")
                UploadImage.AlternateText = Server.HtmlEncode("Add Miscellaneous Link")
            Else
                CType(imgUploadVidKeyVal, Image).Visible = False
                CType(imgUploadMiscLinkValue, Image).Visible = False
            End If
        End Sub

#End Region

    End Class

End Namespace