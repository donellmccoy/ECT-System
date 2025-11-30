using System;

namespace ALOD.Core.Domain.Modules.Lod
{
    public class LodSearch
    {
        public virtual string CaseId { get; set; }
        public virtual string Compo { get; set; }
        public virtual DateTime DateCreated { get; set; }
        public virtual bool IsFinal { get; set; }
        public virtual bool IsFormal { get; set; }
        public virtual short ModuleId { get; set; }
        public virtual string Name { get; set; }
        public virtual string Pascode { get; set; }
        public virtual int RefId { get; set; }
        public virtual string SSN { get; set; }
        public virtual string UnitName { get; set; }
        public virtual string Workflow { get; set; }
        public virtual short WorkflowId { get; set; }
        public virtual string WorkStatus { get; set; }
        public virtual short WorkStatusId { get; set; }
    }
}