using System;

namespace ALOD.Core.Domain.WelcomePageBanner
{
    [Serializable]
    public class HyperLinkType : Entity
    {
        public virtual string Name { get; set; }
    }
}