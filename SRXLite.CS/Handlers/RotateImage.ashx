<%@ WebHandler Language="C#" Class="SRXLite.Web.Handlers.RotateImage" %>

using System;
using System.Drawing.Imaging;
using System.Web;
using SRXLite.Classes;
using static SRXLite.Modules.Util;

namespace SRXLite.Web.Handlers
{
    public class RotateImage : IHttpAsyncHandler
    {
        private DocumentPageGuid _docPageGuid;
        private DocumentPage _docPage;
        private int _rotate90;
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

            // Get query string parameters
            string id = NullCheck(context.Request.QueryString["id"]);
            _rotate90 = IntCheck(context.Request.QueryString["r"]);
            if (_rotate90 < 0) _rotate90 += 4; // Rotate counterclockwise if number is negative

            try
            {
                _docPageGuid = new DocumentPageGuid(id);
                return _docPageGuid.BeginGetData(BeginProcessRequestCallback, extraData);
            }
            catch (Exception)
            {
                throw new Exception("Invalid ID");
            }
        }

        private void BeginProcessRequestCallback(IAsyncResult result)
        {
            _docPageGuid.EndGetData(result);

            if (_docPageGuid.Data.DocPageID == 0)
            {
                // Guid was not found or has expired
                EndAsyncProcessing(result);
                return;
            }

            // Guid is valid, process the file
            _docPage = new DocumentPage(_docPageGuid.Data);
            _docPage.BeginRotateFlip((RotateFlipType)(_rotate90 % 4), _endProcessCallback, result.AsyncState);
        }

        public void EndProcessRequest(IAsyncResult result)
        {
            try
            {
                if (_exception || _docPage.HasErrors)
                {
                    _context.Response.StatusCode = 500;
                    return;
                }

                _docPage.EndRotateFlip(result);
            }
            finally
            {
                // Dispose objects
                if (_docPage != null) _docPage.Dispose();
                if (_docPageGuid != null) _docPageGuid.Dispose();
            }
        }

        private IAsyncResult EndAsyncProcessing(IAsyncResult result)
        {
            _exception = true;
            return _endProcessCallback.BeginInvoke(result, null, null);
        }
    }
}
