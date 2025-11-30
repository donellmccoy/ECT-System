using System;

namespace ALOD.Core.Domain.Documents
{
    public class DocumentCreatedEventArgs : EventArgs
    {
        public Workflow.PageAccess Access { get; set; }
        public bool AllowDelete { get; set; }
        public bool AllowEdit { get; set; }
        public bool AllowUnlock { get; set; }
        public bool AllowUpload { get; set; }
        public Document Document { get; set; }
    }
}