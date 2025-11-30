using System;

namespace ALOD.Core.Domain.Common
{
    [Serializable]
    public class TrackingItem : Entity
    {
        public TrackingItem()
        { }

        public virtual DateTime ActionDate { get; set; }
        public virtual byte ActionId { get; set; }
        public virtual string ActionName { get; set; }
        public virtual string Description { get; set; }
        public virtual string LastName { get; set; }
        public virtual bool LogChanges { get; set; }
        public virtual byte ModuleType { get; set; }
        public virtual string Notes { get; set; }
        public virtual int ParentId { get; set; }
        public virtual int UserId { get; set; }
    }
}