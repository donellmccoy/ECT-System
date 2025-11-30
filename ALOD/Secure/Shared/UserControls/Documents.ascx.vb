Imports ALOD.Core.Domain.Documents
Imports ALOD.Core.Domain.Lookup
Imports ALOD.Core.Domain.Modules.Lod
Imports ALOD.Core.Domain.Modules.SpecialCases
Imports ALOD.Core.Domain.Workflow
Imports ALOD.Core.Interfaces.DAOInterfaces
Imports ALOD.Core.Utils.RegexValidation
Imports ALOD.Data
Imports ALOD.Logging
Imports ALODWebUtility.Common
Imports ALODWebUtility.Providers

Namespace Web.UserControls

    Partial Class Secure_Shared_UserControls_Documents
        Inherits System.Web.UI.UserControl

#Region "Fields"

        Private _dao As IDocumentDao
        Private _daoFactory As IDaoFactory
        Private _dispositionDao As ILookupDispositionDao
        Private _docCatViewDao As IDocCategoryViewDao
        Private _documents As IList(Of Document)
        Private _info As WorkflowDocument
        Private _lodCurrentStatusCode As Integer
        Private _memoSource As MemoDao2
        Private _rs As SC_RS
        Private _special As SpecialCase
        Private _SpecialCaseDao As ISpecialCaseDAO
        Private _stampDao As ICertificationStampDao

#End Region

#Region "Properties"

        ReadOnly Property DaoFactory() As IDaoFactory
            Get
                If (_daoFactory Is Nothing) Then
                    _daoFactory = New NHibernateDaoFactory()
                End If

                Return _daoFactory
            End Get
        End Property

        ReadOnly Property DocCatViewDao() As IDocCategoryViewDao
            Get
                If (_docCatViewDao Is Nothing) Then
                    _docCatViewDao = DaoFactory.GetDocCategoryViewDao()
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

        ReadOnly Property SCDao() As ISpecialCaseDAO
            Get
                If (_SpecialCaseDao Is Nothing) Then
                    _SpecialCaseDao = DaoFactory.GetSpecialCaseDAO()
                End If

                Return _SpecialCaseDao
            End Get
        End Property

        Public Property AllDocuments() As IList(Of IDictionary(Of String, Boolean))
            Get
                If (ViewState("AllDocuments") Is Nothing) Then
                    ViewState("AllDocuments") = New List(Of IDictionary(Of String, Boolean))()
                End If
                Return CType(ViewState("AllDocuments"), IList(Of IDictionary(Of String, Boolean)))
            End Get
            Set(value As IList(Of IDictionary(Of String, Boolean)))
                ViewState("AllDocuments") = value
            End Set
        End Property

        Public Property DocStart() As Integer
            Get
                If (ViewState("DocStart") Is Nothing) Then
                    ViewState("DocStart") = 0
                End If
                Return CInt(ViewState("DocStart"))
            End Get
            Set(value As Integer)
                ViewState("DocStart") = value
            End Set
        End Property

        Public Property Required() As IDictionary(Of String, Boolean)
            Get
                If (ViewState("Required") Is Nothing) Then
                    ViewState("Required") = Nothing
                End If
                Return CType(ViewState("Required"), IDictionary(Of String, Boolean))
            End Get
            Set(value As IDictionary(Of String, Boolean))
                ViewState("Required") = value
            End Set
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

        Protected Property ModuleId() As Integer
            Get
                If (ViewState("moduleId") Is Nothing) Then
                    ViewState("moduleId") = ModuleType.System
                End If
                Return CInt(ViewState("moduleId"))
            End Get
            Set(value As Integer)
                ViewState("moduleId") = value
            End Set
        End Property

        Protected Property refID() As Integer
            Get
                If (ViewState("refID") Is Nothing) Then
                    ViewState("refID") = 0
                End If
                Return CInt(ViewState("refID"))
            End Get
            Set(value As Integer)
                ViewState("refID") = value
            End Set
        End Property

        Protected ReadOnly Property RSCase() As SC_RS
            Get
                If (_rs Is Nothing) Then
                    _rs = SCDao.GetById(refID)
                End If

                Return _rs
            End Get
        End Property

        Protected ReadOnly Property UserCanOnlyViewRedactedDocuments As Boolean
            Get
                Return UserHasPermission(PERMISSION_SARC_VIEW_REDACTED_DOCUMENTS_ONLY)
            End Get
        End Property

        Private ReadOnly Property CertificationStampDao() As ICertificationStampDao
            Get
                If (_stampDao Is Nothing) Then
                    _stampDao = DaoFactory.GetCertificationStampDao()
                End If

                Return _stampDao
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

        Private Property DocumentEntityId() As String
            Get
                If (ViewState("DocumentEntityId") Is Nothing) Then
                    ViewState("DocumentEntityId") = ""
                End If
                Return CInt(ViewState("DocumentEntityId"))
            End Get
            Set(value As String)
                ViewState("DocumentEntityId") = value
            End Set
        End Property

        Private Property DocumentGroupId() As IList(Of Long)
            Get
                If (ViewState("DocumentGroupId") Is Nothing) Then
                    ViewState("DocumentGroupId") = New List(Of Long)({0})
                End If
                Return CType(ViewState("DocumentGroupId"), IList(Of Long))
            End Get
            Set(value As IList(Of Long))
                ViewState("DocumentGroupId") = value
            End Set
        End Property

        Private Property DocumentViewId() As IList(Of Integer)
            Get
                If (ViewState("DocumentViewId") Is Nothing) Then
                    ViewState("DocumentViewId") = New List(Of Integer)({0})
                End If
                Return CType(ViewState("DocumentViewId"), IList(Of Integer))
            End Get
            Set(value As IList(Of Integer))
                ViewState("DocumentViewId") = value
            End Set
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

#End Region

#Region "Page Methods"

        Public Sub Initialize(ByVal hostPage As Page, ByVal navigator As TabNavigator, ByVal info As WorkflowDocument)
            refID = info.refID
            ModuleId = info.moduleId
            DocumentViewId = info.DocumentViewId
            DocumentEntityId = info.DocumentEntityId
            MemberSSN = LodCrypto.Encrypt(info.DocumentEntityId)
            DocumentGroupId = info.DocumentGroupId
            AllDocuments = info.AllDocuments
            Required = info.Required
            DocStart = info.DocumentStart
            _lodCurrentStatusCode = info.LODStatusCode

            If (navigator Is Nothing) Then
                UserCanEdit = False
            Else
                If Not (ModuleId = ModuleType.LOD) Then
                    UserCanEdit = GetAccess(navigator.PageAccess, False)
                Else
                    UserCanEdit = GetAccessLOD(navigator.PageAccess, False, Services.LodService.GetById(refID))
                End If
            End If

            ScriptManager.GetCurrent(hostPage).RegisterAsyncPostBackControl(Me)
        End Sub

        Protected Function CanUpload(ByVal cat As DocumentCategory2) As Boolean
            'Documents that cannot be uploaded
            If (UserCanEdit AndAlso cat.DocCatId <> DocumentType.OriginalDocuments) Then
                Return True
            End If

            Return False

        End Function

        Protected Sub CategoryRepeater_ItemDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.RepeaterItemEventArgs) Handles CategoryRepeater.ItemDataBound
            If (e.Item.ItemType <> ListItemType.Item AndAlso e.Item.ItemType <> ListItemType.AlternatingItem) Then
                Exit Sub
            End If
            Dim cat As DocumentCategory2 = CType(e.Item.DataItem, DocumentCategory2)

            SetUploadImage(cat, e)

            Dim count As Integer = 0

            _documents = New List(Of Document)

            While (count < DocumentGroupId.Count)

                For Each currDoc In DocumentDao.GetDocumentsByGroupId(DocumentGroupId(count))
                    _documents.Add(currDoc)
                Next

                ' Ensure count is a valid index for AllDocuments
                If count >= 0 AndAlso count < AllDocuments.Count Then
                    Dim currentDocument As Dictionary(Of String, Boolean) = TryCast(AllDocuments(count), Dictionary(Of String, Boolean))

                    If currentDocument IsNot Nothing AndAlso currentDocument.ContainsKey(cat.DocCatId.ToString()) AndAlso currentDocument(cat.DocCatId.ToString()) Then
                        Dim lblReqd As Label = CType(e.Item.FindControl("lblReqd"), Label)
                        lblReqd.Text = SetRequiredLabel(cat.DocCatId)
                    End If
                End If

                count += 1

            End While

            Dim ChildDocRepeater As Repeater = CType(e.Item.FindControl("ChildDocRepeater"), Repeater)

            Dim docs = From d In _documents Where d.DocType = cat.DocCatId Select d Order By d.DateAdded
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
                                                    "&modId=" + ModuleId.ToString() +
                                                    "&refId=" + refID.ToString() +
                                                    "&doc=" + (Server.UrlEncode(Server.HtmlDecode(doc.Description).Replace("'", "")))

            link.Attributes.Add("onclick", "viewDoc('" + url + "'); return false;")

            If (UserCanEdit) Then

                Dim edit As Image = CType(e.Item.FindControl("EditDocument"), Image)

                Dim x As String = Server.HtmlDecode(doc.Description).Replace(ControlChars.Quote, "''").Replace("'", "\'")
                If (doc.UploadedBy Like Session("UserName")) Then
                    edit.Attributes.Add("onclick", "editDocument(" + doc.Id.ToString() + "," _
                                                    + "'" + doc.DocType.ToString() + "'," _
                                                    + "'" + doc.DocDate.ToString(DATE_FORMAT) + "'," _
                                                    + "'" + Server.HtmlEncode(x) + "'" _
                                                    + ");")
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
                LogManager.LogAction(CByte(ModuleId), UserAction.DocumentDeleted, refID, "Deleted: " + parts(1))

                If (DoesRSStampDocIdNeedToBeUpdated(docId)) Then
                    SetRSCaseStampedDocId(Nothing)
                End If

                UpdateDocumentList()
            End If
        End Sub

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
            AddStyleSheet(Page, "~/Styles/Documents.css")

            If (Not IsPostBack) Then
                UpdateDocumentList()
                LogManager.LogAction(CByte(ModuleId), UserAction.ViewPage, refID, "Viewed Page: " + SiteMap.CurrentNode.Title)
            End If
        End Sub

        Protected Function SetRequiredLabel(ByVal DocCatId As Integer) As String

            If DocCatId = DocumentType.SleepStudy Then
                Return "Required for Sleep Apnea"
            ElseIf DocCatId = DocumentType.PFTResults Then
                Return "Required for Asthma"
            ElseIf DocCatId = DocumentType.OptometryExam Then
                Return "Required for Diabetes if Indicated on Form"
            ElseIf DocCatId = DocumentType.Labs Then
                Return "Required for Diabetes: Fasting Blood Sugar, HqbA1C"
            Else
                Return "Required"
            End If

        End Function

        Protected Sub SetUploadImage(ByVal cat As DocumentCategory2, ByVal e As System.Web.UI.WebControls.RepeaterItemEventArgs)
            Dim control As Image = CType(e.Item.FindControl("UploadImage"), Image)
            If Me.CanUpload(cat) Then
                If cat.DocCatId = DocumentType.Form2808 AndAlso Me.ModuleId = 22 Then
                    If Me.IsValidCertificationStampSelected() Then
                        Dim processType As String = "1"
                        If Not Me.RSCase.StampedDocId.HasValue OrElse Me.RSCase.StampedDocId.Value = 0 Then
                            processType = "2"
                        End If
                        Dim url As String = Me.ResolveClientUrl("~/Secure/Shared/CustomDocumentUpload.aspx") & "?refId=" & Me.refID.ToString() & "&group=" & Me.DocumentGroupId(0).ToString() & "&id=" & CStr(cat.DocCatId) & "&entity=" & Me.Server.UrlEncode(Me.MemberSSN) & "&catName=" & cat.CategoryDescription & "&processType=" & processType
                        Me.SetUploadImageAttributes(control, url, cat)
                    Else
                        control.Visible = False
                    End If
                Else
                    Dim index As Integer = Me.DocumentViewId.Count - 1
                    While index >= 0
                        For Each documentCategory2 As DocumentCategory2 In Me.DocCatViewDao.GetCategoriesByDocumentViewId(Me.DocumentViewId(index))
                            If documentCategory2.DocCatId = cat.DocCatId Then
                                Dim url As String = Me.ResolveClientUrl("~/Secure/Shared/DocumentUpload.aspx") & "?group=" & Me.DocumentGroupId(index).ToString() & "&id=" & CStr(cat.DocCatId) & "&entity=" & Me.Server.UrlEncode(Me.MemberSSN)
                                Me.SetUploadImageAttributes(control, url, cat)
                                Exit While
                            End If
                        Next
                        index -= 1
                    End While
                End If
            Else
                control.Visible = False
            End If
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
            Dim docs As List(Of Document) = DocumentDao.GetDocumentsByGroupId(DocumentGroupId(0))
            Dim before As Document = (From d In docs Where d.Id = docId Select d).FirstOrDefault()
            Dim changes As New ChangeSet()

            If (docTypeStr <> before.DocType.ToString()) Then
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
                Dim logId As Integer = LogManager.LogAction(CByte(ModuleId), UserAction.EditDocument, refID, "Edited: " + desc)
                changes.Save(logId)
                UpdateDocumentList()
            End If

        End Sub

        Protected Sub UpdateDocumentList()
            Dim count As Integer = 0
            Dim categories As List(Of DocumentCategory2) = New List(Of DocumentCategory2)

            ProcessRecentlyAddedDocuments()

            While (DocumentViewId.Count > count AndAlso AllDocuments.Count > count)
                'Dim viewCats2 As List(Of DocumentCategory2) = DocCatViewDao.GetCategoriesByDocumentViewId(DocumentViewId(count))
                Dim viewCats2 As List(Of DocumentCategory2) = GetDocumentViewCategories(DocumentViewId(count))
                Dim activeDocs2 = From p In AllDocuments(count) Select p.Key

                Dim categories2 As List(Of DocumentCategory2) = (From p In viewCats2 Where activeDocs2.Contains(p.DocCatId.ToString())).ToList()

                For Each category In categories2
                    If (_lodCurrentStatusCode <> LodStatusCode.Complete AndAlso category.DocCatId = DocumentType.AmendedDocuments) Then
                        Continue For
                    End If

                    If (activeDocs2.Contains(category.DocCatId.ToString())) Then
                        If (category.CategoryDescription.Equals("Wing CC (1971)") AndAlso Not SESSION_WS_ID(refID) = AFRCWorkflows.INWingCCAction) Then
                            category.CategoryDescription = "Completed Form 1971"
                        End If
                        categories.Add(category)
                    End If
                Next

                count += 1
            End While

            categories = categories.GroupBy(Function(x) x.CategoryDescription).Select(Function(x) x.First).ToList

            'zach
            If Session(SESSIONKEY_COMPO) = "5" Then

                If ModuleId = 9 Then

                    For Each liI As DocumentCategory2 In categories

                        Select Case liI.CategoryDescription
                            Case "WWD Cover Letter"
                                liI.CategoryDescription = "Commanders Impact Statement (CIS)"
                            Case "AF Form 469"
                                liI.CategoryDescription = "Narrative Summary (NARSUM)"
                            Case "Narrative Summary"
                                liI.CategoryDescription = "DAWG Checklist"
                            Case "IPEB Forms"
                                liI.CategoryDescription = "AF Form 469"
                            Case "Member Utilization Questionnaire (MUQ)"
                                liI.CategoryDescription = "Supporting Medical Documentation"
                            Case "Medical Evaluation (ME) Fact Sheet"
                                liI.CategoryDescription = "NDDES Coversheet"
                            Case "Private Physician Docs"
                                liI.CategoryDescription = "Statement of Selection (Elects to Enter DES Process)"
                            Case "PS 3811 (Postal Proof of Delivery)"
                                liI.CategoryDescription = "MILDPS Reports (RSAA04, RSAR02, RSLOSS, RSRBTH)"
                            Case "WWD Disposition"
                                liI.CategoryDescription = "Documentation Requests"
                            Case "WWD Letter to Member (Documentation Request)"
                                liI.CategoryDescription = "PS 3811 (Postal Proof of Delivery)"
                            Case "Return To Duty Letter"
                                liI.CategoryDescription = "AF Form 356"
                            Case "Disqualification Letter"
                                liI.CategoryDescription = "Statement of Request (Individual accepts/rejects FEB findings and/or requests FPEB)"
                        End Select

                    Next

                End If

            End If

            CategoryRepeater.DataSource = categories
            CategoryRepeater.DataBind()

            DocumentTypeSelect.DataSource = categories
            DocumentTypeSelect.DataTextField = "CategoryDescription"
            DocumentTypeSelect.DataValueField = "DocCatId"
            DocumentTypeSelect.DataBind()
        End Sub

        Private Function DoesRSStampDocIdNeedToBeUpdated(docId As Integer)
            If (ModuleId <> ModuleType.SpecCaseRS) Then
                Return False
            End If

            If (RSCase Is Nothing OrElse Not RSCase.StampedDocId.HasValue) Then
                Return False
            End If

            If (RSCase.StampedDocId.Value <> docId) Then
                Return False
            End If

            Return True
        End Function

        Private Function GetDocumentViewCategories(docViewId As Integer) As List(Of DocumentCategory2)
            If (ModuleId = ModuleType.SARC) Then
                Return GetRestrictedSARCDocumentViewCategories(docViewId)
            Else
                Return DocCatViewDao.GetCategoriesByDocumentViewId(docViewId)
            End If
        End Function

        Private Function GetRestrictedSARCDocumentViewCategories(docViewId As Integer) As List(Of DocumentCategory2)
            If (UserCanOnlyViewRedactedDocuments) Then
                Return DocCatViewDao.GetRedactedCategoriesByDocumentViewId(docViewId)
            Else
                Return DocCatViewDao.GetCategoriesByDocumentViewId(docViewId).Where(Function(x) Not x.CategoryDescription.Equals("Miscellaneous")).ToList()
            End If
        End Function

        Private Function IsValidCertificationStampSelected() As Boolean
            If (Not RSCase.CertificationStamp.HasValue OrElse RSCase.CertificationStamp.Value = 0) Then
                Return False
            End If

            Dim stamp As CertificationStamp = CertificationStampDao.GetById(RSCase.CertificationStamp.Value)

            If (stamp Is Nothing) Then
                Return False
            End If

            ' Determine the active disposition for the case (either the Board Med's disposition or the HQ Tech's disposition...Board Med's has higher precedence)...
            ' Qualified = 1; Disqualified = 0
            Dim activeDisposition As Integer = -1

            activeDisposition = RSCase.GetActiveDispositionValue(DispositionDao)

            If (activeDisposition < 0) Then
                Return False
            End If

            ' Check to see if the type of stamp chosen does not match the active disposition for the case...
            If (activeDisposition = 1 AndAlso stamp.IsQualified = False) Then
                Return False
            ElseIf (activeDisposition = 0 AndAlso stamp.IsQualified = True) Then
                Return False
            End If

            Return True
        End Function

        Private Sub ProcessRecentlyAddedDocuments()
            If (ModuleId = ModuleType.SpecCaseRS) Then
                ProcessRSRecentlyAddedDocuments()
            End If
        End Sub

        Private Sub ProcessRSRecentlyAddedDocuments()
            If (RSCase Is Nothing OrElse Not RSCase.DocumentGroupId.HasValue OrElse RSCase.DocumentGroupId.Value = 0) Then
                Exit Sub
            End If

            Dim docs As List(Of KeyValuePair(Of Long, Integer)) = DocumentDao.GetRecentlyAddedDocuments(RSCase.Id, RSCase.DocumentGroupId)

            For Each p As KeyValuePair(Of Long, Integer) In docs
                If (p.Value = DocumentType.Form2808 AndAlso Not RSCase.StampedDocId.HasValue) Then
                    SetRSCaseStampedDocId(p.Key)
                End If
            Next

            DocumentDao.RemoveRecentlyAddedDocuments(RSCase.Id, RSCase.DocumentGroupId)
        End Sub

        Private Sub SetRSCaseStampedDocId(id As Nullable(Of Long))
            If (RSCase Is Nothing) Then
                Exit Sub
            End If

            RSCase.StampedDocId = id
            SCDao.SaveOrUpdate(RSCase)
            SCDao.CommitChanges()
        End Sub

        Private Sub SetUploadImageAttributes(uploadImage As Image, url As String, cat As DocumentCategory2)
            uploadImage.Attributes.Add("onclick", "uploadDoc('" + url + "');")
            uploadImage.AlternateText = Server.HtmlEncode("Add " + cat.CategoryDescription + " document")
        End Sub

#End Region

    End Class

End Namespace