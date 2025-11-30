using System;

namespace ALOD.Core.Domain.Users
{
    [Serializable]
    public class UserRole : Entity
    {
        public virtual bool Active { get; set; }
        public virtual UserGroup Group { get; set; }
        public virtual AccessStatus Status { get; set; }
        public virtual AppUser User { get; set; }
    }
}