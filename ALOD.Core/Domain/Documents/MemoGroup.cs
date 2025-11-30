using ALOD.Core.Domain.Users;

namespace ALOD.Core.Domain.Documents
{
    public class MemoGroup : Entity
    {
        public virtual bool CanCreate { get; set; }
        public virtual bool CanDelete { get; set; }
        public virtual bool CanEdit { get; set; }
        public virtual bool CanView { get; set; }
        public virtual UserGroup Group { get; set; }
        public virtual MemoTemplate Template { get; set; }
    }
}