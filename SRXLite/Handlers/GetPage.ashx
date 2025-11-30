<%@ WebHandler Language="VB" Class="Web.Handlers.GetPage" %>

Option Strict On

Imports System.Drawing.Imaging
Imports SRXLiteWebUtility.Classes
Imports SRXLiteWebUtility.Modules

Namespace Web.Handlers

    Public Class GetPage : Implements IHttpAsyncHandler
		
        Private _docPageGuid As DocumentPageGuid
        Private _docPage As DocumentPage
        Private _context As HttpContext
        Private _endProcessCallback As AsyncCallback
        Private _parameters As Parameters
        Private _exception As Boolean = False
			
        Private Structure Parameters
            Public ID As String
            Public Height As Integer
            Public RotateType As Integer
            Public UseCache As Boolean
            Public Width As Integer
        End Structure
				
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
            With _parameters
                .ID = NullCheck(context.Request.QueryString("id"))
                .Height = IntCheck(_context.Request.QueryString("h"))
                .RotateType = IntCheck(_context.Request.QueryString("r"))
                .UseCache = BoolCheck(_context.Request.QueryString("c"))
                .Width = IntCheck(_context.Request.QueryString("w"))
            End With
		
            Try
                _docPageGuid = New DocumentPageGuid(_parameters.ID)
                Return _docPageGuid.BeginGetData(New AsyncCallback(AddressOf BeginProcessRequestCallback), extraData)

            Catch ex As Exception
                Throw New Exception("Invalid ID")
            End Try
        End Function
	
        Private Sub BeginProcessRequestCallback(ByVal result As IAsyncResult)
            _docPageGuid.EndGetData(result)
		
            If _docPageGuid.Data.DocPageID = 0 OrElse _docPageGuid.HasErrors Then
                'Guid was not found or has expired
                EndAsyncProcessing(result)
                Exit Sub
            End If
		
            'Guid is valid, process the file
            _docPage = New DocumentPage(_docPageGuid.Data)
            _docPage.BeginGetBytes(_endProcessCallback, result.AsyncState)
        End Sub

        Public Sub EndProcessRequest(ByVal result As System.IAsyncResult) Implements System.Web.IHttpAsyncHandler.EndProcessRequest
            Try
                'Skip output if exception occurred
                If _exception Then Exit Try
			
                Dim fileBytes() As Byte = _docPage.EndGetBytes(result)
		
                'Set output cache settings -----------------------------------
                If _parameters.UseCache Then
                    _context.Response.Cache.SetExpires(Now().AddSeconds(600))
                    _context.Response.Cache.SetCacheability(HttpCacheability.Public)
                    _context.Response.Cache.VaryByParams("*") = True
                Else
                    'Load the latest version of the image for each request
                    _context.Response.Cache.SetCacheability(HttpCacheability.NoCache)
                End If

                'Output the file data ----------------------------------------
                _context.Response.ClearContent()
                _context.Response.ClearHeaders()
								
                If _docPage.IsImage Then
                    'Process the byte array as a bitmap
                    Dim imageSettings As DocumentPage.ImageSettings
                    With imageSettings
                        .Height = _parameters.Height
                        .RotateType = CType(_parameters.RotateType, RotateFlipType)
                        .ScaleHeight = True 'TODO: should this be default?
                        .Width = _parameters.Width
                    End With
				
                    _context.Response.ContentType = "image/jpeg"
		
                    'Save the bitmap to the output stream
                    Using bmp As Bitmap = _docPage.ProcessFileAsBitmap(fileBytes, imageSettings)
                        bmp.Save(_context.Response.OutputStream, ImageFormat.Jpeg)
                    End Using

                Else

                    If Not _docPage.IsBrowserViewable Then
                        'Prompt to save or open file
                        _context.Response.AddHeader("content-disposition", "attachment; filename=document." & _docPage.FileExtension)
                    End If
				
                    'Save the byte array to the output stream
                    _context.Response.ContentType = _docPage.ContentType
                    _context.Response.BinaryWrite(fileBytes)
                    _context.Response.End()
                End If
		
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