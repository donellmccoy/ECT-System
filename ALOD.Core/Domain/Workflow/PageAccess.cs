using System;

namespace ALOD.Core.Domain.Workflow
{
    [Serializable]
    public class PageAccess : Entity
    {
        public enum AccessLevel
        {
            None = 0,
            ReadOnly = 1,
            ReadWrite = 2
        }

        public virtual AccessLevel Access { get; set; }
        public virtual byte GroupId { get; set; }
        public virtual short PageId { get; set; }
        public virtual string PageTitle { get; set; }
        public virtual byte WorkflowId { get; set; }
        public virtual int WorkStatusId { get; set; }
    }
}