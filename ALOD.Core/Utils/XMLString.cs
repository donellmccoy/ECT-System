using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace ALOD.Core.Utils
{
    public class XMLString : IDisposable
    {
        private Stack<String> _sections;
        private MemoryStream _stream;
        private XmlTextWriter _writer;

        public XMLString(string title)
        {
            _sections = new Stack<string>();
            _stream = new MemoryStream();
            _writer = new XmlTextWriter(_stream, System.Text.Encoding.ASCII);

            BeginElement(title);
        }

        public string Value
        {
            get { return this.ToString(); }
        }

        public void BeginElement(string name)
        {
            _sections.Push(name);
            _writer.WriteStartElement(name);
        }

        public void Close()
        {
            while (_sections.Count > 0)
                EndElement();
        }

        public void EndElement()
        {
            _sections.Pop();
            _writer.WriteEndElement();
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            Close();

            _writer.Flush();
            _stream.Seek(0, SeekOrigin.Begin);
            StreamReader reader = new StreamReader(_stream);
            return reader.ReadToEnd();
        }

        public void WriteAttribute(string name, string value)
        {
            _writer.WriteAttributeString(name, value);
        }

        #region IDisposable Members

        private bool disposedValue = false;

        /// <inheritdoc/>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposedValue && disposing)
            {
                try
                {
                    _writer.Close();
                }
                catch
                {
                }
            }
        }

        #endregion IDisposable Members
    }
}