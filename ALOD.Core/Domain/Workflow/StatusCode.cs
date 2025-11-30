using System.Collections.Generic;

namespace ALOD.Core.Domain.Workflow
{
    public class StatusCode : Entity
    {
        public virtual bool CanAppeal { get; set; }
        public virtual char Compo { get; set; }
        public virtual string Description { get; set; }
        public virtual byte DisplayOrder { get; set; }
        public virtual string Filter { get; set; }
        public virtual byte? GroupId { get; set; }
        public virtual bool IsApproved { get; set; }
        public virtual bool IsCancel { get; set; }
        public virtual bool IsDisposition { get; set; }
        public virtual bool IsFinal { get; set; }
        public virtual bool IsFormal { get; set; }
        public virtual ModuleType ModuleId { get; set; }
        public virtual IList<WorkStatus> WorkStatusList { get; set; }
    }
}