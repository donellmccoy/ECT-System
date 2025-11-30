using System;

namespace ALOD.Core.Domain.WelcomePageBanner
{
    [Serializable]
    public class HyperLink : Entity
    {
        public virtual string DisplayText { get; set; }
        public virtual string Name { get; set; }
        public virtual HyperLinkType Type { get; set; }
        public virtual string Value { get; set; }
    }
}