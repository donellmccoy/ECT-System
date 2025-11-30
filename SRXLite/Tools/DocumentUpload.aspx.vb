Option Strict On

'Imports ALOD.Logging
Imports SRXLite.Classes
Imports SRXLite.DataTypes
Imports SRXLite.Modules

Namespace Web.Tools

    Partial Class Tools_DocumentUpload
        Inherits System.Web.UI.Page

        Private _docGuid As DocumentGuid
        Private _doc As Document
        Private _batch As Batch
        Private _docUploadKeys As UploadKeys
        Private _isBatch As Boolean
        Private _batchType As BatchType
        Private _batchLocation As String
        Private _entityDisplayText As String

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
            _docGuid = New DocumentGuid(Request.QueryString("id"))
            _isBatch = Request.QueryString("bid") IsNot Nothing
            _docUploadKeys = New UploadKeys

            'User-defined stylesheet
            Dim stylesheetUrl As String = NullCheck(Request.QueryString("styleUrl"))
            If stylesheetUrl.Length > 0 Then
                lnkStyleSheet.Href = stylesheetUrl
            End If

            'User-defined Entity display text
            _entityDisplayText = NullCheck(Request.QueryString("e"))

            Dim task As New PageAsyncTask(
              New BeginEventHandler(AddressOf BeginGetDocumentGuidData),
              New EndEventHandler(AddressOf EndGetDocumentGuidData),
              New EndEventHandler(AddressOf AsyncTaskTimeout),
              Nothing)

            RegisterAsyncTask(task)
            Page.ExecuteRegisteredAsyncTasks() 'Ensure this task executes first

            If _isBatch Then
                rbUpload.Checked = False
                rbScan.Checked = True
                rbUpload.Disabled = True
                rbScan.Disabled = True

                Dim crypto As New CryptoManager()
                Dim data As Collection = crypto.DecryptNameValuePairs(Request.QueryString("bid"))
                _batchType = CType(ByteCheck(data("bt"), 1), BatchType)
                _batchLocation = NullCheck(data("l"))
            End If

            'Set display of controls on postback
            trUpload.Style("display") = IIf(rbUpload.Checked, "", "none").ToString
            trScan.Style("display") = IIf(rbScan.Checked, "", "none").ToString
            btnUploadScan.Value = IIf(rbUpload.Checked, "Upload", "Scan & Upload").ToString
            ddlMode.Disabled = chkScanCustom.Checked
            ddlResolution.Disabled = chkScanCustom.Checked
            ddlPageSide.Disabled = chkScanCustom.Checked


            If Request.QueryString("test") = "1" Then

                divSubtitle.InnerText = "TEST CONNECTION SUCCESSFUL"
            End If
        End Sub

#Region " AsyncTaskTimeout "
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="result"></param>
        ''' <remarks></remarks>
        Private Sub AsyncTaskTimeout(ByVal result As IAsyncResult)
            'TODO: timeout code
        End Sub
#End Region

#Region " GetDocumentGuidData (Begin/End) "
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>
        ''' <param name="callback"></param>
        ''' <param name="stateObject"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Private Function BeginGetDocumentGuidData(ByVal sender As Object, ByVal e As EventArgs, ByVal callback As AsyncCallback, ByVal stateObject As Object) As IAsyncResult
            Return _docGuid.BeginGetData(callback, stateObject)
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="result"></param>
        ''' <remarks></remarks>
        Private Sub EndGetDocumentGuidData(ByVal result As IAsyncResult)
            Dim data As New DocumentGuid.GuidData
            data = _docGuid.EndGetData(result)

            If data.UserID = 0 Then
                'Guid was not found or has expired
                'TODO: 
                Response.Write("This page has expired.")
                divDocumentUpload.Visible = False

            Else

                _doc = New Document(data)
                DocTypeName.InnerText = data.DocTypeName
                divSubtitle.InnerText = "for " & IIf(_entityDisplayText.Length = 0, data.EntityName, _entityDisplayText).ToString
            End If

            'Testing connection
            If divSubtitle.InnerText = "for XXXXXTEST" Then
                TestLabel.Visible = True
                divDocumentUpload.Visible = False
            End If
        End Sub
#End Region

        Protected Sub btnSubmit_ServerClick(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnSubmit.ServerClick
            'Validation ---------------------------------------

            'LogManager.LogError("Test btnSubmit_ServerClick in SRX")

            Dim Msg As New StringBuilder
            Dim list As New BulletedList
            Dim docDate As Date = tcDocDate.SelectedDate

            If docDate = Nothing Then
                list.Items.Add("Document Date")
            ElseIf docDate > Today Then
                list.Items.Add("Document Date cannot be a future date")
            ElseIf docDate.Year < 1754 Then
                list.Items.Add("Document Date is invalid")
            End If

            If rbUpload.Checked Then
                If FileUpload1.HasFile Then
                    'Check file type
                    Dim fileName As String = FileUpload1.FileName
                    Dim fileExt As String = fileName.Substring(fileName.LastIndexOf(".") + 1)
                    If Not IsFileExtValid(fileExt) Then
                        list.Items.Add("File type (" & fileExt & ") is not supported. Please upload a common image type (jpg, tif, etc.) or pdf file.")
                    End If

                    'Check file size
                    If Not IsInitialFileSizeValid(FileUpload1.PostedFile.ContentLength) Then
                        list.Items.Add("File size (" & GetFileSizeMB(FileUpload1.PostedFile.ContentLength) & " MB) exceeded the maximum limit. Uploads are limited to " & GetInitialFileSizeUploadLimitMB() & " MB.")
                    End If
                Else
                    list.Items.Add("File Name")
                End If
            End If

            'Display message
            If list.Items.Count > 0 Then
                list.Style.Add("margin-top", "2px")
                list.Style.Add("margin-bottom", "10px")

                divValidation.Controls.Add(New LiteralControl("The following fields are required:"))
                divValidation.Controls.Add(list)
                Exit Sub
            End If

            With _docUploadKeys
                .DocDate = docDate
                .DocDescription = Server.HtmlEncode(txtDocDescr.Text.Trim)
                .DocStatus = CType(IIf(_isBatch, DocumentStatus.Batch, DocumentStatus.Pending), DocumentStatus)
                .DocTypeID = _docGuid.Data.DocTypeID
                .EntityName = _docGuid.Data.EntityName
                .FileName = IIf(rbUpload.Checked, FormatFileName(FileUpload1.FileName), "scan.tif").ToString
                .InputType = CType(IIf(rbUpload.Checked, InputType.Upload, InputType.Scan), InputType)
            End With

            'LogManager.LogError("Test btnSubmit_ServerClick")

            Dim task As New PageAsyncTask( _
             New BeginEventHandler(AddressOf BeginCreateDocument), _
             New EndEventHandler(AddressOf EndCreateDocument), _
             New EndEventHandler(AddressOf AsyncTaskTimeout), _
             Nothing)

            RegisterAsyncTask(task)
        End Sub

#Region " CreateDocument (Begin/End) "
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>
        ''' <param name="callback"></param>
        ''' <param name="stateObject"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Private Function BeginCreateDocument(ByVal sender As Object, ByVal e As EventArgs, ByVal callback As AsyncCallback, ByVal stateObject As Object) As IAsyncResult
            Return _doc.BeginCreate(_docUploadKeys, callback, stateObject)
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="result"></param>
        ''' <remarks></remarks>
        Private Sub EndCreateDocument(ByVal result As IAsyncResult)
            Try
                _doc.EndCreate(result)

                If _doc.HasErrors Then
                    Throw New Exception(_doc.ErrorMessage)
                End If

                If rbUpload.Checked Then
                    'Upload -----------------------------
                    'Add page
                    Dim task As New PageAsyncTask(
                      New BeginEventHandler(AddressOf BeginAddPage),
                      New EndEventHandler(AddressOf EndAddPage),
                      New EndEventHandler(AddressOf AsyncTaskTimeout),
                      Nothing)

                    RegisterAsyncTask(task)

                Else
                    If _isBatch Then
                        'Create batch
                        Dim task As New PageAsyncTask(
                          New BeginEventHandler(AddressOf BeginCreateBatch),
                          New EndEventHandler(AddressOf EndCreateBatch),
                          New EndEventHandler(AddressOf AsyncTaskTimeout),
                          Nothing)

                        RegisterAsyncTask(task)
                    Else

                        RegisterScanRedirectJS()
                    End If
                End If

            Catch ex As Exception
                LogError(ex.ToString, _docGuid.Data.UserID, _docGuid.Data.SubuserID)
                divValidation.InnerHtml = "An error occurred while processing your request. Please try again. If the error continues to occur, please contact the help desk.<br/><br/>"
            End Try
        End Sub
#End Region

#Region " CreateBatch (Begin/End) "
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>
        ''' <param name="callback"></param>
        ''' <param name="stateObject"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Private Function BeginCreateBatch(ByVal sender As Object, ByVal e As EventArgs, ByVal callback As AsyncCallback, ByVal stateObject As Object) As IAsyncResult
            _batch = New Batch(_doc.DocID, _docGuid.Data.UserID, _docGuid.Data.SubuserID)
            Return _batch.BeginCreate(_batchType, _batchLocation, _docUploadKeys, callback, stateObject)
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="result"></param>
        ''' <remarks></remarks>
        Private Sub EndCreateBatch(ByVal result As IAsyncResult)
            Try
                _batch.EndCreate(result)

                If _batch.HasErrors Then
                    Throw New Exception(_batch.ErrorMessage)
                End If

                RegisterScanRedirectJS()

            Catch ex As Exception
                LogError(ex.ToString, _docGuid.Data.UserID, _docGuid.Data.SubuserID)
                divValidation.InnerHtml = "Test2 An error occurred while processing your request. Please try again. If the error continues to occur, please contact the help desk.<br/><br/>"
            End Try
        End Sub
#End Region

#Region " AddPage (Begin/End) "
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>
        ''' <param name="callback"></param>
        ''' <param name="stateObject"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Private Function BeginAddPage(ByVal sender As Object, ByVal e As EventArgs, ByVal callback As AsyncCallback, ByVal stateObject As Object) As IAsyncResult
            Return _doc.BeginAddPage(FileUpload1.FileBytes, callback, stateObject)
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="result"></param>
        ''' <remarks></remarks>
        Private Sub EndAddPage(ByVal result As IAsyncResult)
            Try
                If Not _doc.IsMultiframe Then 'TODO: remove if statement
                    _doc.EndAddPage(result)
                End If

                If _doc.HasErrors Then
                    Throw New Exception(_doc.ErrorMessage)
                End If

                'Close window
                ScriptManager.RegisterStartupScript(Page, Page.GetType(), "UploadCompleteJS", "window.close();", True)


            Catch ex As Exception
                LogError(ex.ToString)
                divValidation.InnerHtml = "Test3 An error occurred while processing your request. Please try again. If the error continues to occur, please contact the help desk.<br/><br/>"
            End Try
        End Sub
#End Region

        Private Sub RegisterScanRedirectJS()
            Dim values(5) As String
            values(0) = _doc.DocID.ToString
            values(1) = ddlMode.Value
            values(2) = ddlResolution.Value
            values(3) = ddlPageSide.Value
            values(4) = IIf(chkScanCustom.Checked, "1", "0").ToString

            Dim queryString As String
            queryString = "docid={0}&m={1}&r={2}&ps={3}&custom={4}"
            queryString = String.Format(queryString, values)

            Dim crypto As New CryptoManager()
            Dim scanUrl As String = "DocumentUploadScan.aspx?id=" & NullCheck(Request.QueryString("id")) & "&id2=" & crypto.EncryptForUrl(queryString) & _
             "&returnurl=" & Server.UrlEncode(Request.Url.ToString)
            ScriptManager.RegisterClientScriptBlock(Page, Page.GetType(), "ScanJS", String.Format("window.location='{0}';", scanUrl), True)
        End Sub

        Protected Sub Page_Error(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Error
            divValidation.InnerText = "An error occurred while processing your request. Please try again."
        End Sub

    End Class

End Namespace