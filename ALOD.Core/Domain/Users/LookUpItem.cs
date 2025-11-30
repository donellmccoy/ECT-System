using System;

namespace ALOD.Core.Domain.Users
{
    [Serializable]
    public class LookUpItem
    {
        public virtual String Name { get; set; }
        public virtual object Value { get; set; }
    }
}