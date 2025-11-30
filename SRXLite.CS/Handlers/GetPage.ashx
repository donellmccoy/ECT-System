<%@ WebHandler Language="C#" Class="SRXLite.Web.Handlers.GetPage" %>

using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Web;
using SRXLite.Classes;
using static SRXLite.Modules.Util;

namespace SRXLite.Web.Handlers
{
    public class GetPage : IHttpAsyncHandler
    {
        private DocumentPageGuid _docPageGuid;
        private DocumentPage _docPage;
        private HttpContext _context;
        private AsyncCallback _endProcessCallback;
        private Parameters _parameters;
        private bool _exception = false;

        private struct Parameters
        {
            public string ID;
            public int Height;
            public int RotateType;
            public bool UseCache;
            public int Width;
        }

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
            _parameters = new Parameters
            {
                ID = NullCheck(context.Request.QueryString["id"]),
                Height = IntCheck(context.Request.QueryString["h"]),
                RotateType = IntCheck(context.Request.QueryString["r"]),
                UseCache = BoolCheck(context.Request.QueryString["c"]),
                Width = IntCheck(context.Request.QueryString["w"])
            };

            try
            {
                _docPageGuid = new DocumentPageGuid(_parameters.ID);
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

            if (_docPageGuid.Data.DocPageID == 0 || _docPageGuid.HasErrors)
            {
                // Guid was not found or has expired
                EndAsyncProcessing(result);
                return;
            }

            // Guid is valid, process the file
            _docPage = new DocumentPage(_docPageGuid.Data);
            _docPage.BeginGetBytes(_endProcessCallback, result.AsyncState);
        }

        public void EndProcessRequest(IAsyncResult result)
        {
            try
            {
                // Skip output if exception occurred
                if (_exception) return;

                byte[] fileBytes = _docPage.EndGetBytes(result);

                // Set output cache settings
                if (_parameters.UseCache)
                {
                    _context.Response.Cache.SetExpires(DateTime.Now.AddSeconds(600));
                    _context.Response.Cache.SetCacheability(HttpCacheability.Public);
                    _context.Response.Cache.VaryByParams["*"] = true;
                }
                else
                {
                    // Load the latest version of the image for each request
                    _context.Response.Cache.SetCacheability(HttpCacheability.NoCache);
                }

                // Output the file data
                _context.Response.ClearContent();
                _context.Response.ClearHeaders();

                if (_docPage.IsImage)
                {
                    // Process the byte array as a bitmap
                    DocumentPage.ImageSettings imageSettings = new DocumentPage.ImageSettings
                    {
                        Height = _parameters.Height,
                        RotateType = (RotateFlipType)_parameters.RotateType,
                        ScaleHeight = true, // TODO: should this be default?
                        Width = _parameters.Width
                    };

                    _context.Response.ContentType = "image/jpeg";

                    // Save the bitmap to the output stream
                    using (Bitmap bmp = _docPage.ProcessFileAsBitmap(fileBytes, imageSettings))
                    {
                        bmp.Save(_context.Response.OutputStream, ImageFormat.Jpeg);
                    }
                }
                else
                {
                    if (!_docPage.IsBrowserViewable)
                    {
                        // Prompt to save or open file
                        _context.Response.AddHeader("content-disposition", "attachment; filename=document." + _docPage.FileExtension);
                    }

                    // Save the byte array to the output stream
                    _context.Response.ContentType = _docPage.ContentType;
                    _context.Response.BinaryWrite(fileBytes);
                    _context.Response.End();
                }
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
