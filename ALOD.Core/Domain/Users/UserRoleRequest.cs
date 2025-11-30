using System;

namespace ALOD.Core.Domain.Users
{
    [Serializable]
    public class UserRoleRequest : Entity
    {
        public virtual AppUser CompletedBy { get; set; }
        public virtual string CompletedComments { get; set; }
        public virtual UserGroup CurrentGroup { get; set; }
        public virtual DateTime? DateCompleted { get; set; }
        public virtual DateTime DateRequested { get; set; }
        public virtual Boolean IsNewRole { get; set; }
        public virtual UserGroup RequestedGroup { get; set; }
        public virtual string RequestorComments { get; set; }
        public virtual AccessStatus Status { get; set; }
        public virtual AppUser User { get; set; }
    }
}