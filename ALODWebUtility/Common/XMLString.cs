using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace ALODWebUtility.Common
{
    public class XMLString : IDisposable
    {
        private Stack<string> _sections = new Stack<string>();
        private MemoryStream _stream;
        private XmlTextWriter _writer;

        private bool disposedValue = false;

        public XMLString(string title)
        {
            _stream = new MemoryStream();
            _writer = new XmlTextWriter(_stream, System.Text.Encoding.ASCII);

            // start the document
            BeginElement(title);
        }

        public string Value
        {
            get
            {
                return this.ToString();
            }
        }

        public void BeginElement(string name)
        {
            _sections.Push(name);
            _writer.WriteStartElement(name);
        }

        public void Close()
        {
            // write the end elements for any that weren't closed
            while (_sections.Count > 0)
            {
                EndElement();
            }
        }

        public void EndElement()
        {
            _sections.Pop();
            _writer.WriteEndElement();
        }

        public override string ToString()
        {
            Close();

            _writer.Flush();
            byte[] buffer = new byte[_stream.Length + 1];
            _stream.Seek(0, SeekOrigin.Begin);

            StreamReader reader = new StreamReader(_stream);
            return reader.ReadToEnd();
        }

        public void WriteAttribute(string name, string value)
        {
            _writer.WriteAttributeString(name, value);
        }

        // To detect redundant calls

        // IDisposable
        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposedValue)
            {
                if (disposing)
                {
                    // TODO: free managed resources when explicitly called
                    try
                    {
                        _writer.Close();
                    }
                    catch
                    {
                    }
                }

                // TODO: free shared unmanaged resources
            }
            this.disposedValue = true;
        }

        #region IDisposable Support

        // This code added by Visual Basic to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code.  Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion
    }
}
