using System;

namespace ALOD.Core.Domain.Users
{
    public class UnitChainType : Entity
    {
        public virtual bool Active { get; set; }
        public virtual String CreatedBy { get; set; }
        public virtual DateTime CreatedDate { get; set; }
        public virtual String Description { get; set; }
        public virtual String ModifiedBy { get; set; }
        public virtual DateTime ModifiedDate { get; set; }
        public virtual String Name { get; set; }
    }
}