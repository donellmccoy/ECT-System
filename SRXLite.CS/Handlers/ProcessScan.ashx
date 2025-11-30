<%@ WebHandler Language="C#" Class="SRXLite.Web.Handlers.ProcessScan" %>

using System;
using System.Web;
using SRXLite.Classes;
using static SRXLite.Modules.ExceptionHandling;

namespace SRXLite.Web.Handlers
{
    public class ProcessScan : IHttpAsyncHandler
    {
        private DocumentGuid _docGuid;
        private Document _doc;
        private HttpContext _context;
        private AsyncCallback _endProcessCallback;
        private bool _exception = false;

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
            _endProcessCallback = cb;

            try
            {
                _docGuid = new DocumentGuid(_context.Request.QueryString["id"]);
                return _docGuid.BeginGetData(BeginProcessRequestCallback, extraData);
            }
            catch (Exception ex)
            {
                LogError(ex.ToString());
                throw new Exception("Invalid ID");
            }
        }

        private void BeginProcessRequestCallback(IAsyncResult result)
        {
            DocumentGuid.GuidData data = _docGuid.EndGetData(result);

            if (data.UserID == 0)
            {
                // Guid was not found or has expired
                EndAsyncProcessing(result);
                return;
            }

            // Guid is valid, process the file
            _doc = new Document(data);

            // Get byte array from the posted file
            HttpPostedFile postedFile = _context.Request.Files[0];
            byte[] fileBytes = new byte[postedFile.ContentLength];
            postedFile.InputStream.Read(fileBytes, 0, fileBytes.Length);

            _doc.BeginAddPage(fileBytes, _endProcessCallback, result.AsyncState);
        }

        public void EndProcessRequest(IAsyncResult result)
        {
            try
            {
                // Skip output if exception occurred
                if (_exception) return;

                _doc.EndAddPage(result);
            }
            finally
            {
                // Dispose objects
                if (_doc != null) _doc.Dispose();
                if (_docGuid != null) _docGuid.Dispose();
            }
        }

        private IAsyncResult EndAsyncProcessing(IAsyncResult result)
        {
            _exception = true;
            return _endProcessCallback.BeginInvoke(result, null, null);
        }
    }
}
