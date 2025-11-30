using System;
using System.Configuration;
using System.IO;
using System.Web;
using WebSupergoo.ABCpdf12;
using static SRXLite.Modules.AppSettings;

namespace SRXLite.Classes
{
    public class PDF : IDisposable
    {
        private Doc _doc;
        private Doc _officeDoc;

        #region Constructors

        public PDF()
        {
            _doc = new Doc();
            _officeDoc = new Doc();
        }

        #endregion

        #region PageType enum

        public enum PageType
        {
            Image,
            MSWord,
            PDF
        }

        #endregion

        #region AddBookmark

        public void AddBookmark(string path, bool expanded)
        {
            _doc.AddBookmark(path, false);
        }

        #endregion

        #region AddCoverPage

        public void AddCoverPage(string entityID)
        {
            _doc.Page = _doc.AddPage(1);

            _doc.HPos = 0.5; // Center text
            _doc.Font = _doc.AddFont("Helvetica");
            _doc.TextStyle.ParaSpacing = 20;
            _doc.FontSize = 36;
            _doc.AddText("\r\n\r\nDocument Archive\r\n");

            _doc.FontSize = 18;
            _doc.AddText("for\r\n");
            _doc.AddText(entityID + "\r\n");

            _doc.VPos = 0.98;
            _doc.HPos = 0.5;
            _doc.AddHtml("<font color='gray'>Generated: " + DateTime.Now.ToString("yyyyMMdd HH:mm ET") + "</font>");

            AddWatermark();
        }

        #endregion

        #region AddPage

        /// <summary>
        /// Adds a page and bookmark to the PDF file.
        /// </summary>
        /// <param name="fileBytes"></param>
        /// <param name="type"></param>
        /// <param name="docTypeName"></param>
        /// <param name="docDate"></param>
        /// <param name="pageNum"></param>
        /// <returns></returns>
        public bool AddPage(byte[] fileBytes, PageType type, string docTypeName, DateTime docDate, int pageNum)
        {
            string pagePath = docTypeName + "\\" + docDate;
            string bookmarkPath = pagePath + "\\Page ";

            bool isAdded = true;
            _doc.Transform.Reset();

            switch (type)
            {
                case PageType.Image:
                    XImage image = new XImage();
                    try
                    {
                        image.SetData(fileBytes);
                        _doc.MediaBox.String = "0 0 612 792";
                        _doc.Rect.String = _doc.MediaBox.String;

                        if (image.Width > image.Height)
                        {
                            double aspectRatio = image.Width / (double)image.Height;
                            int w = Math.Min(792, image.Width);
                            int h = Math.Min(612, (int)Math.Floor(w / aspectRatio)); // Scaled height

                            _doc.MediaBox.String = "0 0 792 612";
                            _doc.Rect.String = "0 0 " + w + " " + h;
                        }

                        _doc.Page = _doc.AddPage(); // Add new page
                        _doc.AddImageObject(image, false);
                        _doc.AddBookmark(bookmarkPath + pageNum, false);
                        AddWatermark();
                    }
                    catch (Exception)
                    {
                        isAdded = false;
                    }
                    finally
                    {
                        image.Dispose();
                    }
                    break;

                case PageType.PDF:
                    Doc pdfDoc = new Doc();

                    try
                    {
                        int lastPageNumber = _doc.PageCount;
                        pdfDoc.Read(fileBytes);

                        // Remove existing bookmarks
                        while (pdfDoc.Bookmark.Count > 0)
                        {
                            pdfDoc.Bookmark.RemoveAt(0);
                        }

                        pdfDoc.Form.Stamp(); // Stamp form fields into document
                        pdfDoc.Flatten(); // Combine and compress layers
                        AddWatermarkToDoc(pdfDoc);

                        _doc.Append(pdfDoc); // Append pages

                        _doc.PageNumber = lastPageNumber + 1;
                        _doc.AddBookmark(pagePath, false);

                        // Add bookmark for each page
                        for (int i = 1; i <= pdfDoc.PageCount; i++)
                        {
                            _doc.PageNumber = lastPageNumber + i;
                            _doc.AddBookmark(bookmarkPath + i, false);
                        }
                    }
                    catch (Exception)
                    {
                        isAdded = false;
                    }
                    finally
                    {
                        pdfDoc.Clear();
                        pdfDoc.Dispose();
                    }
                    break;

                default:
                    isAdded = false;
                    break;
            }

            return isAdded;
        }

        #endregion

        #region AddWatermark

        public void AddWatermark(ref Doc pdfDoc, string text)
        {
            // Add watermark to current page
            pdfDoc.VPos = 0;
            pdfDoc.HPos = 0;
            pdfDoc.Font = pdfDoc.AddFont("Helvetica");
            pdfDoc.Pos.X = pdfDoc.MediaBox.Width;
            pdfDoc.Pos.Y = (pdfDoc.MediaBox.Height / 2) - 175;
            pdfDoc.FontSize = 62;
            pdfDoc.TextStyle.Bold = true;
            pdfDoc.TextStyle.Italic = true;
            pdfDoc.AddHtml("<font color='#FFC0CB'>" + text + "</font>");
            pdfDoc.Transform.Reset();
        }

        private void AddWatermark()
        {
            AddWatermark(_doc);
        }

        private void AddWatermark(Doc pdfDoc)
        {
            if (SRXLite.Modules.AppSettings.Environment.IsProd()) return;
            if (ConfigurationManager.AppSettings["WaterMark"] != null && ConfigurationManager.AppSettings["WaterMark"] != "")
            {
                AddWatermark(ref pdfDoc, ConfigurationManager.AppSettings["WaterMark"]);
            }
        }

        #endregion

        #region AddWatermarkToDoc

        public void AddWatermarkToDoc(string text)
        {
            for (int i = 1; i <= _doc.PageCount; i++)
            {
                _doc.PageNumber = i;
                AddWatermark(ref _doc, text);
            }
        }

        public void AddWatermarkToDoc(Doc pdfDoc)
        {
            for (int i = 1; i <= pdfDoc.PageCount; i++)
            {
                pdfDoc.PageNumber = i;
                AddWatermark(pdfDoc);
            }
        }

        #endregion

        #region GetBytes

        public byte[] GetBytes()
        {
            return _doc.GetData();
        }

        #endregion

        #region Render

        /// <summary>
        /// Render the final PDF document to the client response stream
        /// </summary>
        /// <param name="response"></param>
        public void Render(HttpResponse response)
        {
            response.ClearHeaders();
            response.ClearContent();
            response.ContentType = "application/pdf";
            response.OutputStream.Write(_doc.GetData(), 0, _doc.GetData().Length);
            response.Flush();
            response.Close();
        }

        /// <summary>
        /// Render the final PDF document to a file
        /// </summary>
        /// <param name="fileName">The name of the file to write to</param>
        public void Render(string fileName)
        {
            using (FileStream outStream = new FileStream(fileName, FileMode.Create))
            {
                outStream.Write(_doc.GetData(), 0, _doc.GetData().Length);
            }
        }

        #endregion

        #region IDisposable Support

        private bool disposedValue = false;

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // Free managed resources
                }

                // Free unmanaged resources
                if (_doc != null) _doc.Dispose();
                if (_officeDoc != null) _officeDoc.Dispose();

                disposedValue = true;
            }
        }

        #endregion
    }
}
