using System;

namespace ALOD.Core.Domain.Lookup
{
    [Serializable]
    public class SuicideMethod : Entity
    {
        public virtual bool Active { get; set; }
        public virtual string Name { get; set; }
    }
}