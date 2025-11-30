Option Strict On

Imports SRXLite.Classes
Imports SRXLite.Modules

Namespace Web.Tools

    Partial Class Tools_DocumentUploadScan
        Inherits System.Web.UI.Page

        Private _docGuid As DocumentGuid

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
            Dim id As String = Request.QueryString("id")
            Dim id2 As String = Request.QueryString("id2")
            hReturnUrl.Value = Request.QueryString("returnurl")

            Dim crypto As New CryptoManager()
            Dim data As Collection = crypto.DecryptNameValuePairs(id2)

            hActionPage.Value = Request.ApplicationPath & "/handlers/processscan.ashx?id=" & id
            hMode.Value = NullCheck(data("m"))
            hResolution.Value = NullCheck(data("r"))
            hPageSide.Value = NullCheck(data("ps"))
            hScanCustom.Value = NullCheck(data("custom"))

            _docGuid = New DocumentGuid(id)

            Dim task As New PageAsyncTask( _
              New BeginEventHandler(AddressOf BeginGetDocumentGuidData), _
              New EndEventHandler(AddressOf EndGetDocumentGuidData), _
              New EndEventHandler(AddressOf AsyncTaskTimeout), _
              Nothing)

            RegisterAsyncTask(task)
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
                hActionPage.Value = ""
                Response.Write("This page has expired.")
            Else
                'Start scanning
                HtmlBody.Attributes.Add("onload", "Scan();")
            End If
        End Sub
#End Region

    End Class

End Namespace