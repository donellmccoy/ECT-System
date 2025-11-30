using System;
using System.Collections.Generic;

namespace ALOD.Core.Domain.Workflow
{
    [Serializable]
    public class WorkflowStatusOption : Entity
    {
        private bool _optionValid = true;
        private bool _optionVisbile = true;
        public virtual IList<WorkflowOptionAction> ActionList { get; set; }
        public virtual bool Active { get; set; }
        public virtual string Compo { get; set; }
        public virtual string DisplayText { get; set; }

        public virtual bool OptionValid
        {
            get { return _optionValid; }
            set { _optionValid = value; }
        }

        public virtual bool OptionVisible
        {
            get { return _optionVisbile; }
            set { _optionVisbile = value; }
        }

        public virtual IList<WorkflowOptionRule> RuleList { get; set; }
        public virtual byte SortOrder { get; set; }
        public virtual byte Template { get; set; }
        public virtual WorkStatus WorkstatusType { get; set; }
        public virtual int wsStatus { get; set; }
        public virtual int wsStatusOut { get; set; }
    }
}