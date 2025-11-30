using System;

namespace ALOD.Core.Domain.Users
{
    public class UnitChainEntry : Entity
    {
        public virtual string ChainType { get; set; }
        public virtual int CreatedBy { get; set; }
        public virtual DateTime CreatedDate { get; set; }
        public virtual int ModifiedBy { get; set; }
        public virtual DateTime ModifiedDate { get; set; }
        public virtual int ParentCSCID { get; set; }
        public virtual short ReportView { get; set; }
        public virtual int unitId { get; set; }
        public virtual bool UserModified { get; set; }
    }
}