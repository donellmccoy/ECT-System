<%@ WebHandler Language="VB" Class="Web.Handlers.GenerateDocumentPDF" %>
Option Strict On

Imports SRXLiteWebUtility.Classes

Namespace Web.Handlers

    Public Class GenerateDocumentPDF : Implements IHttpAsyncHandler
	
        Private _docGuid As DocumentGuid
        Private _doc As Document
        Private _context As HttpContext

        Public Sub ProcessRequest(ByVal context As HttpContext) Implements IHttpHandler.ProcessRequest
        End Sub
	 
        Public ReadOnly Property IsReusable() As Boolean Implements IHttpHandler.IsReusable
            Get
                Return False
            End Get
        End Property

        Public Function BeginProcessRequest(ByVal context As System.Web.HttpContext, ByVal cb As System.AsyncCallback, ByVal extraData As Object) As System.IAsyncResult Implements System.Web.IHttpAsyncHandler.BeginProcessRequest
            _context = context
		
            Try
                _docGuid = New DocumentGuid(_context.Request.QueryString("id"))
                Return _docGuid.BeginGetData(cb, extraData)

            Catch ex As Exception
                Throw New Exception("Invalid ID")
            End Try
        End Function
	
        Public Sub EndProcessRequest(ByVal result As System.IAsyncResult) Implements System.Web.IHttpAsyncHandler.EndProcessRequest
            _docGuid.EndGetData(result)
		
            If _docGuid.Data.DocID = 0 OrElse _docGuid.HasErrors Then
                'Guid was not found or has expired
                _context.Response.Write("This page has expired.")
                Exit Sub
            End If
		
            'Guid is valid, process the file
            _doc = New Document(_docGuid.Data, _context)
            Dim fileBytes() As Byte = _doc.GeneratePDF()
			
            'Check if document is empty
            If fileBytes.Length = 0 Then
                _context.Response.Write("No documents found.")
                Exit Sub
            End If

            'Output PDF file
            _context.Response.ClearContent()
            _context.Response.ClearHeaders()
            _context.Response.AddHeader("content-disposition", "attachment; filename=document.pdf")
            _context.Response.ContentType = "application/pdf"
            _context.Response.BinaryWrite(fileBytes)
            _context.Response.End()
        End Sub
	
    End Class
    
End Namespace