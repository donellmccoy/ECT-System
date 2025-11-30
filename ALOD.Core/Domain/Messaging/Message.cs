using ALOD.Core.Domain.Users;
using System;

namespace ALOD.Core.Domain.Messaging
{
    public class Message : Entity
    {
        public virtual string Body { get; set; }
        public virtual AppUser CreatedBy { get; set; }
        public virtual DateTime? EndDate { get; set; }
        public virtual bool IsAdminMessage { get; set; }
        public virtual bool IsPopup { get; set; }
        public virtual string Name { get; set; }
        public virtual DateTime StartDate { get; set; }
        public virtual string Title { get; set; }
    }
}