Imports ALOD.Core.Domain.Documents
Imports ALOD.Core.Interfaces
Imports ALOD.Core.Utils
Imports ALOD.Data
Imports ALOD.Logging
Imports ALODWebUtility.Common
Imports ALODWebUtility.Documents
Imports ALODWebUtility.Providers

Namespace Web.Docs

    Public Class CustomDocumentUpload
        Inherits System.Web.UI.Page

        Private _docDao As IDocumentDao
        Private _processingFactory As DocumentProcessingFactory
        Private _validationErrors As BulletedList

        Public Property DocEntity() As String
            Get
                If (ViewState("DocEntity") Is Nothing) Then
                    ViewState("DocEntity") = String.Empty
                End If
                Return LodCrypto.Decrypt(ViewState("DocEntity"))
            End Get
            Set(ByVal value As String)
                ViewState("DocEntity") = value
            End Set
        End Property

        Public Property DocGroupId() As Integer
            Get
                If (ViewState("DocGroupId") Is Nothing) Then
                    ViewState("DocGroupId") = 0
                End If
                Return CInt(ViewState("DocGroupId"))
            End Get
            Set(ByVal value As Integer)
                ViewState("DocGroupId") = value
            End Set
        End Property

        Public Property DocType() As Integer
            Get
                If (ViewState("DocType") Is Nothing) Then
                    ViewState("DocType") = 0
                End If
                Return CInt(ViewState("DocType"))
            End Get
            Set(ByVal value As Integer)
                ViewState("DocType") = value
            End Set
        End Property

        Public Property DocTypeTitle() As String
            Get
                If (ViewState("DocTypeTitle") Is Nothing) Then
                    ViewState("DocTypeTitle") = String.Empty
                End If
                Return ViewState("DocTypeTitle")
            End Get
            Set(ByVal value As String)
                ViewState("DocTypeTitle") = value
            End Set
        End Property

        Public ReadOnly Property DocumentDao() As IDocumentDao
            Get
                If (_docDao Is Nothing) Then
                    _docDao = New SRXDocumentStore(SESSION_USERNAME)
                End If

                Return _docDao
            End Get
        End Property

        Public ReadOnly Property ProcessingFactory As DocumentProcessingFactory
            Get
                If (_processingFactory Is Nothing) Then
                    _processingFactory = New DocumentProcessingFactory()
                End If

                Return _processingFactory
            End Get
        End Property

        Public Property ProcessType() As Integer
            Get
                If (ViewState("ProcessType") Is Nothing) Then
                    ViewState("ProcessType") = 1
                End If
                Return CInt(ViewState("ProcessType"))
            End Get
            Set(ByVal value As Integer)
                ViewState("ProcessType") = value
            End Set
        End Property

        Public Property RefId() As Integer
            Get
                If (ViewState("RefId") Is Nothing) Then
                    ViewState("RefId") = 0
                End If
                Return CInt(ViewState("RefId"))
            End Get
            Set(ByVal value As Integer)
                ViewState("RefId") = value
            End Set
        End Property

        Protected ReadOnly Property CalendarImage() As String
            Get
                Return Me.ResolveClientUrl("~/App_Themes/" + Page.Theme + "/Images/Calendar.gif")
            End Get
        End Property

        Protected ReadOnly Property ValidationErrors As BulletedList
            Get
                If (_validationErrors Is Nothing) Then
                    _validationErrors = New BulletedList()
                End If

                Return _validationErrors
            End Get
        End Property

        Protected Sub btnSubmit_ServerClick(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnSubmit.ServerClick
            ValidateDocumentDate()
            ValidateSelectedFile()

            If (ValidationErrors.Items.Count > 0) Then
                SetValidationErrorsControl()
                Exit Sub
            End If

            If (Not ProcessSelectedDocument()) Then
                SetValidationErrorsControl()
                Exit Sub
            End If

            ' Close the window...
            ScriptManager.RegisterStartupScript(Page, Page.GetType(), "UploadCompleteJS", "window.close();", True)
        End Sub

        Protected Function GenerateDocumentMetaData() As Document
            Dim docDate As Date
            Dim fileExt As String

            fileExt = filePicker.FileName.Substring(filePicker.FileName.LastIndexOf(".") + 1)
            docDate = Date.Parse(txtDocDate.Text)

            Dim docMetaData As Document = New Document()

            docMetaData.DateAdded = DateTime.Now
            docMetaData.DocDate = docDate
            docMetaData.Description = Server.HtmlEncode(txtDocDescr.Text.Trim)
            docMetaData.DocStatus = DocumentStatus.Approved
            docMetaData.Extension = fileExt
            docMetaData.SSN = DocEntity
            docMetaData.DocType = DocType
            docMetaData.OriginalFileName = Utility.FormatFileName(filePicker.FileName)

            Return docMetaData
        End Function

        Protected Function GetFileExtension(ByVal fileName As String)
            If (String.IsNullOrEmpty(fileName)) Then
                Return String.Empty
            End If

            Return fileName.Substring(fileName.LastIndexOf(".") + 1)
        End Function

        Protected Sub InitControls()
            DocTypeName.InnerText = DocTypeTitle

            ' XXX-XX-0002
            If (DocEntity.Length = 9) Then
                divSubtitle.InnerText = "for XXX-XX-" & DocEntity.Substring(5, 4)
            End If

            SetInputFormatRestriction(Page, txtDocDate, FormatRestriction.Numeric, "/")
            txtDocDate.CssClass = "datePickerPast"
            txtDocDate.Enabled = True
        End Sub

        Protected Function IsFileExtValid(ByVal ext As String)
            Select Case ext.ToLower()
                Case "bmp", "doc", "docx", "g42", "gif", "jpeg", "jpg", "pdf", "pjpg", "png", "rtf", "tif", "tiff", "txt", "xls", "xlsx"
                    Return True
                Case Else
                    Return False
            End Select
        End Function

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
            WriteHostName(Page)
            RegisterJavaScriptScripts()
            ProcessQueryString()

            If (Not IsPostBack) Then
                InitControls()
            End If
        End Sub

        Protected Sub ProcessQueryString()
            ' Check and assign RefId from the query string
            Dim queryRefId As String = Request.QueryString("refId")
            If Not String.IsNullOrEmpty(queryRefId) AndAlso IsNumeric(queryRefId) Then
                RefId = CInt(queryRefId)
            Else
                ' Handle the case where RefId is not present or not a number
                LogManager.LogError("ProcessQueryString: RefId is missing or invalid in the query string.")
                RefId = 0 ' Or set a default value if appropriate
            End If

            DocGroupId = CInt(Request.QueryString("group"))
            DocType = CInt(Request.QueryString("id"))

            ' Null check for catName
            If Not String.IsNullOrEmpty(Request.QueryString("catName")) Then
                DocTypeTitle = Request.QueryString("catName").ToString()
            Else
                DocTypeTitle = String.Empty
            End If

            DocEntity = Request.QueryString("entity")
            ProcessType = CInt(Request.QueryString("processType"))
        End Sub

        Protected Function ProcessSelectedDocument() As Boolean
            Dim strategy As IDocumentProcessingStrategy = Nothing

            strategy = ProcessingFactory.GetStrategyByType(ProcessType)

            If (strategy Is Nothing) Then
                ValidationErrors.Items.Add("Failed to process document")
                Return False
            End If

            Dim docId As Long = strategy.ProcessDocument(RefId, DocGroupId, DocumentDao, GenerateDocumentMetaData(), filePicker.FileBytes)

            If (docId <= 0) Then
                If (strategy.GetProcessingErrors().Count > 0) Then
                    For Each s As String In strategy.GetProcessingErrors()
                        ValidationErrors.Items.Add(s)
                    Next
                Else
                    ValidationErrors.Items.Add("Failed to process document. Please contact a system administrator for further assistance.")
                End If

                Return False
            End If

            DocumentDao.InsertRecentlyAddedDocument(RefId, DocGroupId, docId, DocType)

            Return True
        End Function

        Protected Sub RegisterJavaScriptScripts()
            Page.ClientScript.RegisterClientScriptInclude("JQueryScript", Request.ApplicationPath + "/Script/jquery-3.6.0.min.js")
            Page.ClientScript.RegisterClientScriptInclude("MigrateScript", Request.ApplicationPath + "/Script/jquery-migrate-3.4.1.min.js")

            Page.ClientScript.RegisterClientScriptInclude("JqueryBlock", Request.ApplicationPath + "/Script/jquery.blockUI.min.js")
            Page.ClientScript.RegisterClientScriptInclude("JqueryDom", Request.ApplicationPath + "/Script/jquery-dom.js")
            Page.ClientScript.RegisterClientScriptInclude("JqueryUI", Request.ApplicationPath + "/Script/jquery-ui-1.13.0.min.js")
            Page.ClientScript.RegisterClientScriptInclude("JQueryModal", Request.ApplicationPath + "/Script/jqModal.js")
            Page.ClientScript.RegisterClientScriptInclude("CommonScript", Request.ApplicationPath + "/Script/common.js")
        End Sub

        Protected Sub SetValidationErrorsControl()
            ValidationErrors.Style.Add("margin-top", "2px")
            ValidationErrors.Style.Add("margin-bottom", "10px")

            divValidation.Controls.Add(New LiteralControl("The following errors have occurred:"))
            divValidation.Controls.Add(ValidationErrors)
        End Sub

        Protected Function ValidateDocumentDate() As Boolean
            Dim docDate As Date

            If (String.IsNullOrEmpty(txtDocDate.Text)) Then
                ValidationErrors.Items.Add("Document Date is required")
                Return False
            End If

            If (Not Date.TryParse(txtDocDate.Text, docDate)) Then
                ValidationErrors.Items.Add("Document Date is invalid")
                Return False
            End If

            If (docDate = Nothing) Then
                ValidationErrors.Items.Add("Document Date is required")
                Return False
            ElseIf (docDate > Today) Then
                ValidationErrors.Items.Add("Document Date cannot be a future date")
                Return False
            ElseIf (docDate.Year < 1754) Then
                ValidationErrors.Items.Add("Document Date is invalid")
                Return False
            End If

            Return True
        End Function

        Protected Function ValidateSelectedFile() As Boolean
            If (Not filePicker.HasFile) Then
                ValidationErrors.Items.Add("Must select a document to upload")
                Return False
            End If

            If (Not IsFileExtValid(GetFileExtension(filePicker.FileName))) Then
                ValidationErrors.Items.Add("Invalid file type")
                Return False
            End If

            If (Not Utility.IsFileSizeValid(filePicker.PostedFile.ContentLength)) Then
                ValidationErrors.Items.Add("File size (" & Utility.GetFileSizeMB(filePicker.PostedFile.ContentLength) & " MB) exceeded the maximum limit. Uploads are limited to " & Utility.GetFileSizeUploadLimitMB() & " MB.")
                Return False
            End If

            Return True
        End Function

    End Class

End Namespace