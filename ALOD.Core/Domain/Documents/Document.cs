using System;

namespace ALOD.Core.Domain.Documents
{
    public class Document
    {
        public DocumentClass DocClass = DocumentClass.Document;
        public bool CanAppend { get; set; }
        public DateTime DateAdded { get; set; }
        public string Description { get; set; }
        public DateTime DocDate { get; set; }
        public DocumentStatus DocStatus { get; set; }
        public DocumentType DocType { get; set; }
        public string DocTypeName { get; set; }
        public string Extension { get; set; }
        public string IconUrl { get; set; }
        public long Id { get; set; }
        public string OriginalFileName { get; set; }
        public string SSN { get; set; }
        public string UploadedBy { get; set; }
    }
}