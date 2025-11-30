namespace ALOD.Core.Domain.Workflow
{
    public class WorkflowOptionAction : Entity
    {
        public virtual WorkflowActionTypes ActionType { get; set; }
        public virtual int Data { get; set; }
        public virtual WorkflowStatusOption OptionType { get; set; }
        public virtual int Target { get; set; }
    }
}