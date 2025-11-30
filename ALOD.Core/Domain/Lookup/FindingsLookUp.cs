using System;

namespace ALOD.Core.Domain.Lookup
{
    [Serializable]
    public class FindingsLookUp : Entity
    {
        public virtual string Description { get; set; }
        public virtual string FindingType { get; set; }
    }
}