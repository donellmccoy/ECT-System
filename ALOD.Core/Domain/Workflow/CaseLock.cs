using System;

namespace ALOD.Core.Domain.Workflow
{
    public class CaseLock : Entity
    {
        public virtual DateTime LockTime { get; set; }
        public virtual byte ModuleType { get; set; }
        public virtual int ReferenceId { get; set; }
        public virtual int UserId { get; set; }
    }
}