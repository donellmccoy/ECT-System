namespace ALOD.Core.Domain.Workflow
{
    public class ActionTypes : Entity
    {
        public virtual string ActionName { get; set; }
        public virtual bool Hide { get; set; }
        public virtual bool LogChanges { get; set; }
    }
}