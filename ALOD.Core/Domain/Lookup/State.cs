namespace ALOD.Core.Domain.Lookup
{
    public class State : EntityWithTypedId<string>
    {
        public virtual string Country { get; set; }
        public virtual string Name { get; set; }
    }
}