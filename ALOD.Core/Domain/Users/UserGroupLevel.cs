using ALOD.Core.Domain.Modules.Lod;
using System;

namespace ALOD.Core.Domain.Users
{
    [Serializable]
    public class UserGroupLevel : Entity
    {
        public virtual string Name { get; set; }

        public virtual CommentsTypes GetAssociatedCommentsType()
        {
            switch (Name)
            {
                case "Unit":
                    return CommentsTypes.UnitComments;

                case "Board":
                    return CommentsTypes.BoardComments;

                case "System Admin":
                    return CommentsTypes.SysAdminComments;

                default:
                    return CommentsTypes.UnitComments;
            }
        }
    }
}