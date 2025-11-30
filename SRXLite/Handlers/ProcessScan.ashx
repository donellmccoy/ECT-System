<%@ WebHandler Language="VB" Class="Web.Handlers.ProcessScan" %>
Option Strict On

Imports SRXLiteWebUtility.Classes
Imports SRXLiteWebUtility.Modules

Namespace Web.Handlers

    Public Class ProcessScan : Implements IHttpAsyncHandler
	    
        Private _docGuid As DocumentGuid
        Private _doc As Document
        Private _context As HttpContext
        Private _endProcessCallback As AsyncCallback
        Private _exception As Boolean = False

        Public Sub ProcessRequest(ByVal context As HttpContext) Implements IHttpHandler.ProcessRequest
        End Sub
	
        Public ReadOnly Property IsReusable() As Boolean Implements IHttpHandler.IsReusable
            Get
                Return False
            End Get
        End Property

        Public Function BeginProcessRequest(ByVal context As System.Web.HttpContext, ByVal cb As System.AsyncCallback, ByVal extraData As Object) As System.IAsyncResult Implements System.Web.IHttpAsyncHandler.BeginProcessRequest
            _context = context
            _endProcessCallback = cb
		
            Try
                _docGuid = New DocumentGuid(_context.Request.QueryString("id"))
                Return _docGuid.BeginGetData(New AsyncCallback(AddressOf BeginProcessRequestCallback), extraData)

            Catch ex As Exception
                LogError(ex.ToString)
                Throw New Exception("Invalid ID")
            End Try
        End Function
	
        Private Sub BeginProcessRequestCallback(ByVal result As IAsyncResult)
            Dim data As New DocumentGuid.GuidData
            data = _docGuid.EndGetData(result)
		
            If data.UserID = 0 Then
                'Guid was not found or has expired
                EndAsyncProcessing(result)
                Exit Sub
            End If
		
            'Guid is valid, process the file
            _doc = New Document(data)
		
            'Get byte array from the posted file
            Dim postedFile As HttpPostedFile = _context.Request.Files(0)
            Dim fileBytes(postedFile.ContentLength - 1) As Byte
            postedFile.InputStream.Read(fileBytes, 0, fileBytes.Length)
				
            _doc.BeginAddPage(fileBytes, _endProcessCallback, result.AsyncState)
        End Sub

        Public Sub EndProcessRequest(ByVal result As System.IAsyncResult) Implements System.Web.IHttpAsyncHandler.EndProcessRequest
            Try
                'Skip output if exception occurred
                If _exception Then Exit Try
			
                _doc.EndAddPage(result)
			
            Finally
                'Dispose objects
                If _doc IsNot Nothing Then _doc.Dispose()
                If _docGuid IsNot Nothing Then _docGuid.Dispose()
            End Try
        End Sub
	
        Private Function EndAsyncProcessing(ByVal result As IAsyncResult) As IAsyncResult
            _exception = True
            Return _endProcessCallback.BeginInvoke(result, Nothing, Nothing)
        End Function
	
    End Class
    
End Namespace