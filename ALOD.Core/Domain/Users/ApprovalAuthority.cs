using System;

namespace ALOD.Core.Domain.Users
{
    public class ApprovalAuthority : Entity
    {
        public virtual AppUser CreatedBy { get; set; }
        public virtual DateTime CreatedDate { get; set; }
        public virtual DateTime EffectiveDate { get; set; }
        public virtual AppUser ModifiedBy { get; set; }
        public virtual DateTime ModifiedDate { get; set; }
        public virtual string Name { get; set; }
        public virtual string SignatureBlock { get; set; }
        public virtual string Title { get; set; }
    }
}