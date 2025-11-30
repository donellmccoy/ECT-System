using System.Collections.Generic;

namespace ALOD.Core.Domain.Workflow
{
    public class RuleType : Entity
    {
        public virtual bool Active { get; set; }
        public virtual string Name { get; set; }
        public virtual IList<WorkflowOptionRule> OptionList { get; set; }
        public virtual string Prompt { get; set; }
        public virtual int ruleTypeId { get; set; }
        public virtual Workflow Workflow { get; set; }
    }
}