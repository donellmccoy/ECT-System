using ALOD.Core.Domain.Users;
using System;

namespace ALOD.Core.Domain.Modules.Lod
{
    [Serializable]
    public class LODComment : Entity
    {
        public LODComment()
        { }

        public virtual string Comments { get; set; }
        public virtual int CommentType { get; set; }
        public virtual AppUser CreatedBy { get; set; }
        public virtual DateTime? CreatedDate { get; set; }
        public virtual bool Deleted { get; set; }
        public virtual int LodId { get; set; }
        public virtual int ModuleID { get; set; }
    }
}