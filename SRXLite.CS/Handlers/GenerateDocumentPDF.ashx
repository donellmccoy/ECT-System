<%@ WebHandler Language="C#" Class="SRXLite.Web.Handlers.GenerateDocumentPDF" %>

using System;
using System.Web;
using SRXLite.Classes;

namespace SRXLite.Web.Handlers
{
    public class GenerateDocumentPDF : IHttpAsyncHandler
    {
        private DocumentGuid _docGuid;
        private Document _doc;
        private HttpContext _context;

        public void ProcessRequest(HttpContext context)
        {
        }

        public bool IsReusable
        {
            get { return false; }
        }

        public IAsyncResult BeginProcessRequest(HttpContext context, AsyncCallback cb, object extraData)
        {
            _context = context;

            try
            {
                _docGuid = new DocumentGuid(_context.Request.QueryString["id"]);
                return _docGuid.BeginGetData(cb, extraData);
            }
            catch (Exception)
            {
                throw new Exception("Invalid ID");
            }
        }

        public void EndProcessRequest(IAsyncResult result)
        {
            _docGuid.EndGetData(result);

            if (_docGuid.Data.DocID == 0 || _docGuid.HasErrors)
            {
                // Guid was not found or has expired
                _context.Response.Write("This page has expired.");
                return;
            }

            // Guid is valid, process the file
            _doc = new Document(_docGuid.Data, _context);
            byte[] fileBytes = _doc.GeneratePDF();

            // Check if document is empty
            if (fileBytes.Length == 0)
            {
                _context.Response.Write("No documents found.");
                return;
            }

            // Output PDF file
            _context.Response.ClearContent();
            _context.Response.ClearHeaders();
            _context.Response.AddHeader("content-disposition", "attachment; filename=document.pdf");
            _context.Response.ContentType = "application/pdf";
            _context.Response.BinaryWrite(fileBytes);
            _context.Response.End();
        }
    }
}
