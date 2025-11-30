<%@ WebHandler Language="VB" Class="Web.Handlers.RotateImage" %>
Option Strict On

Imports SRXLiteWebUtility.Classes
Imports SRXLiteWebUtility.Modules

Namespace Web.Handlers

    Public Class RotateImage : Implements IHttpAsyncHandler
        Private _docPageGuid As DocumentPageGuid
        Private _docPage As DocumentPage
        Private _rotate90 As Integer
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
		
            'Get query string parameters
            Dim id As String = NullCheck(context.Request.QueryString("id"))
            _rotate90 = IntCheck(context.Request.QueryString("r"))
            If _rotate90 < 0 Then _rotate90 += 4 'Rotate counterclockwise if number is negative	
		
            Try
                _docPageGuid = New DocumentPageGuid(id)
                Return _docPageGuid.BeginGetData(New AsyncCallback(AddressOf BeginProcessRequestCallback), extraData)

            Catch ex As Exception
                Throw New Exception("Invalid ID")
            End Try
        End Function

        Private Sub BeginProcessRequestCallback(ByVal result As IAsyncResult)
            _docPageGuid.EndGetData(result)
		
            If _docPageGuid.Data.DocPageID = 0 Then
                'Guid was not found or has expired
                EndAsyncProcessing(result)
                Exit Sub
            End If
		
            'Guid is valid, process the file
            _docPage = New DocumentPage(_docPageGuid.Data)
            _docPage.BeginRotateFlip(CType(_rotate90 Mod 4, RotateFlipType), _endProcessCallback, result.AsyncState)
        End Sub

        Public Sub EndProcessRequest(ByVal result As System.IAsyncResult) Implements System.Web.IHttpAsyncHandler.EndProcessRequest
            Try
                If _exception OrElse _docPage.HasErrors Then
                    _context.Response.StatusCode = 500
                    Exit Try
                End If
					
                _docPage.EndRotateFlip(result)

            Finally
                'Dispose objects
                If _docPage IsNot Nothing Then _docPage.Dispose()
                If _docPageGuid IsNot Nothing Then _docPageGuid.Dispose()
            End Try
        End Sub
	
        Private Function EndAsyncProcessing(ByVal result As IAsyncResult) As IAsyncResult
            _exception = True
            Return _endProcessCallback.BeginInvoke(result, Nothing, Nothing)
        End Function
	
    End Class
    
End Namespace