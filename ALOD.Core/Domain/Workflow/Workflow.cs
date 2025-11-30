namespace ALOD.Core.Domain.Workflow
{
    public class Workflow : Entity
    {
        public virtual bool Active { get; set; }
        public virtual string Compo { get; set; }
        public virtual bool Formal { get; set; }
        public virtual int InitialStatus { get; set; }
        public virtual ModuleType ModuleId { get; set; }
        public virtual string Title { get; set; }
    }
}