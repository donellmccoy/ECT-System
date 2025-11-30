namespace ALOD.Core.Domain.Workflow
{
    public class ALODPermission : Entity
    {
        public virtual string Description { get; set; }
        public virtual bool Exclude { get; set; }
        public virtual string Name { get; set; }
    }
}