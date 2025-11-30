namespace ALOD.Core.Domain.Workflow
{
    public class WorkflowOptionRule : Entity
    {
        public virtual bool? CheckAll { get; set; }
        public virtual int OptionId { get; set; }
        public virtual WorkflowStatusOption OptionType { get; set; }
        public virtual byte RuleId { get; set; }
        public virtual RuleType RuleTypes { get; set; }
        public virtual string RuleValue { get; set; }
    }
}