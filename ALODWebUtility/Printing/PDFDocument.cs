using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Web;
using ALOD.Core.Utils;
using WebSupergoo.ABCpdf8;

namespace ALODWebUtility.Printing
{
    public class PDFDocument : IDisposable
    {
        private Doc _doc = null;
        private bool _includeFOUOWatermark;
        private bool _includeWSCompleteWaterMark;
        private List<PDFString> _nullWatermarkBody = null;
        private string _watermark = string.Empty;

        public PDFDocument()
        {
            _doc = new Doc();
            _doc.SaveOptions.Linearize = true;
            _watermark = ConfigurationManager.AppSettings["WaterMark"];
            _nullWatermarkBody = new List<PDFString>();
            _includeFOUOWatermark = true;
        }

        public bool IncludeFOUOWatermark { get; set; }
        public bool IncludeWSCompleteWaterMark { get; set; }

        public string WaterMark
        {
            get { return _watermark; }
            set { _watermark = value; }
        }

        public void AddForm(PDFForm form)
        {
            _doc.Append(form.Document);
        }

        public void AddNullWatermarkString(PDFString str)
        {
            if (_nullWatermarkBody == null || str == null)
            {
                return;
            }

            _nullWatermarkBody.Add(str);
        }

        public void AddPageNumbers(double widthPositionRatio, double heightPositionRatio)
        {
            // Apply text to each page...
            for (int i = 1; i <= _doc.PageCount; i++)
            {
                _doc.PageNumber = i;
                _doc.Pos.X = (_doc.MediaBox.Width * widthPositionRatio);
                _doc.Pos.Y = (_doc.MediaBox.Height * heightPositionRatio);
                _doc.AddText("Page " + i);
                _doc.Flatten();
            }
        }

        public void AddStamp(List<PDFString> stampLines, bool firstPageOnly, double widthRatio, double heightRatio, double tabSize)
        {
            if (stampLines == null || stampLines.Count == 0)
            {
                return;
            }

            int numPages = 0;

            if (firstPageOnly)
            {
                numPages = 1;
            }
            else
            {
                numPages = _doc.PageCount;
            }

            for (short i = 1; i <= numPages; i++)
            {
                _doc.PageNumber = i;

                _doc.Pos.Y = _doc.MediaBox.Height * heightRatio;

                int tabAmount = 0;

                foreach (PDFString s in stampLines)
                {
                    if (!string.IsNullOrEmpty(s.Text))
                    {
                        _doc.Pos.X = (_doc.MediaBox.Width * widthRatio) + tabAmount;

                        _doc.AddHtml(s.GetHTML());

                        tabAmount = tabAmount + (int)tabSize;
                    }
                }
            }
        }

        public void AddWebPage(HttpServerUtility server, string url, int startPage)
        {
            if (startPage < 1 || startPage > _doc.PageCount)
            {
                startPage = 1;
            }

            string renderedPageContents = string.Empty;

            using (StringWriter writer = new StringWriter())
            {
                server.Execute(url, writer, false);
                renderedPageContents = writer.ToString();
            }

            int chainId = 0;

            chainId = _doc.AddImageHtml(renderedPageContents);

            while (true)
            {
                _doc.FrameRect();

                if (!_doc.Chainable(chainId))
                {
                    break;
                }

                _doc.Page = _doc.AddPage();
                chainId = _doc.AddImageToChain(chainId);
            }

            for (int i = startPage; i <= _doc.PageCount; i++)
            {
                _doc.PageNumber = i;
                _doc.Flatten();
            }
        }

        public void Close()
        {
            _nullWatermarkBody.Clear();
            _doc.Clear();
            Dispose();
        }

        public byte[] GetBuffer()
        {
            return _doc.GetData();
        }

        public bool Read(byte[] fileData)
        {
            if (fileData == null)
            {
                return false;
            }

            try
            {
                _doc.Read(fileData);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        /// <summary>
        /// Render the final PDF document to the client response stream
        /// </summary>
        /// <param name="response"></param>
        /// <remarks></remarks>
        public void Render(HttpResponse response)
        {
            if (IncludeFOUOWatermark)
            {
                AddBRBlockNotUsedWatermark();
                AddAABlockNotUsedWatermark();
                AddWJABlockNotUsedWatermark();
                AddFOUOWatermark();
            }

            _doc.Form.Stamp();
            _doc.Flatten();

            AddNullDocumentWatermark();

            byte[] theData = _doc.GetData();

            response.ContentType = "application/pdf";
            response.AddHeader("content-disposition", "inline; filename=output.PDF");
            response.AddHeader("content-length", theData.Length.ToString());
            response.BinaryWrite(theData);
            response.End();
        }

        /// <summary>
        /// Render the final PDF document to a file
        /// </summary>
        /// <param name="fileName">The name of the file to write to</param>
        /// <remarks></remarks>
        public void Render(string fileName)
        {
            if (IncludeFOUOWatermark)
            {
                AddBRBlockNotUsedWatermark();
                AddAABlockNotUsedWatermark();
                AddWJABlockNotUsedWatermark();
                AddFOUOWatermark();
            }

            _doc.Form.Stamp();
            _doc.Flatten();

            AddNullDocumentWatermark();
        }

        public void SetRenderingEngine(EngineType engine)
        {
            _doc.HtmlOptions.Engine = engine;
        }

        private void AddAABlockNotUsedWatermark()
        {
            if (IncludeWSCompleteWaterMark)
            {
                _doc.FontSize = 40;

                _doc.PageNumber = 3;
                _doc.Pos.X = 0;
                _doc.Pos.Y = _doc.Rect.Bottom + 330;
                _doc.AddHtml("<p align=center><StyleRun rotate=11><Font face='arial-bold' color=#FF0000/50>" + "This Section Not Used" + "</Font></StyleRun></p>");
            }
        }

        private void AddBRBlockNotUsedWatermark()
        {
            if (IncludeWSCompleteWaterMark)
            {
                _doc.FontSize = 40;

                _doc.PageNumber = 3;
                _doc.Pos.X = 0;
                _doc.Pos.Y = _doc.Rect.Bottom + 460;
                _doc.AddHtml("<p align=center><StyleRun rotate=11><Font face='arial-bold' color=#FF0000/50>" + "This Section Not Used" + "</Font></StyleRun></p>");
                _doc.Pos.Y = _doc.Rect.Bottom + 600;
                _doc.AddHtml("<p align=center><StyleRun rotate=11><Font face='arial-bold' color=#FF0000/50>" + "This Section Not Used" + "</Font></StyleRun></p>");
            }
        }

        private void AddFOUOWatermark()
        {
            _doc.FontSize = 22;

            for (short i = 1; i <= _doc.PageCount; i++)
            {
                _doc.PageNumber = i;
                _doc.Pos.X = 0;
                _doc.Pos.Y = _doc.Rect.Bottom + 24;
                _doc.AddHtml("<p align=center><StyleRun rotate=0><Font face='arial-bold' color=#FF0000/90>" + ConfigurationManager.AppSettings["WaterMark"] + "</Font></StyleRun></p>");
            }
        }

        private void AddNullDocumentWatermark()
        {
            if (_nullWatermarkBody == null || _nullWatermarkBody.Count == 0)
            {
                return;
            }

            for (short i = 1; i <= _doc.PageCount; i++)
            {
                _doc.PageNumber = i;

                // Add grey overlay
                _doc.Color.String = "128 128 128 a64";
                _doc.Color.Alpha = 255 / 5;
                _doc.FillRect();

                if (Helpers.IsOdd(i))
                {
                    // Fill in grey rectangle
                    _doc.Rect.Inset(38.25, 297);
                    _doc.Color.String = "128 128 128";
                    _doc.Color.Alpha = 255 / 5;
                    _doc.FillRect();
                    _doc.Rect.Inset(-38.25, -297);

                    // Fill in smaller white rectangle over the grey rectangle
                    _doc.Rect.Inset(41.25, 300.5);
                    _doc.Color.String = "255 255 255";
                    _doc.Color.Alpha = 255 / 5;
                    _doc.FillRect();
                    _doc.Rect.Inset(-41.25, -300.5);

                    // Add text via HTML
                    _doc.Rect.Inset(41.25, 0);
                    _doc.Pos.X = 0;
                    _doc.Pos.Y = (_doc.MediaBox.Height / 2) + 75;

                    foreach (PDFString s in _nullWatermarkBody)
                    {
                        if (!string.IsNullOrEmpty(s.Text))
                        {
                            _doc.AddHtml(s.GetHTML());
                        }
                    }
                    _doc.Rect.Inset(-41.25, 0);
                }
            }
        }

        private void AddWJABlockNotUsedWatermark()
        {
            if (IncludeWSCompleteWaterMark)
            {
                _doc.FontSize = 40;

                _doc.PageNumber = 2;
                _doc.Pos.X = 0;
                _doc.Pos.Y = _doc.Rect.Bottom + 143;
                _doc.AddHtml("<p align=center><StyleRun rotate=4><Font face='arial-bold' color=#FF0000/50>" + "This Section Not Used" + "</Font></StyleRun></p>");
            }
        }

        #region IDisposable

        private bool disposedValue = false;

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposedValue)
            {
                if (disposing)
                {
                    _nullWatermarkBody.Clear();
                    _doc.Clear();
                }
            }
            this.disposedValue = true;
        }

        #endregion
    }
}
