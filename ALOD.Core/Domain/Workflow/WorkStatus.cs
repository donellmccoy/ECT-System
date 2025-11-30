using System;
using System.Collections.Generic;

namespace ALOD.Core.Domain.Workflow
{
    [Serializable]
    public class WorkStatus : Entity
    {
        //07-17-2019 S32019 - Curt Lucas
        public virtual string Compo { get; set; }

        // 09-19-2019 Darel Johnson
        public virtual string Description => !string.IsNullOrEmpty(DisplayText) ? DisplayText : StatusCodeType.Description;

        public virtual string DisplayText { get; set; }
        public virtual bool IsBoardStatus { get; set; }
        public virtual byte SortOrder { get; set; }
        public virtual StatusCode StatusCodeType { get; set; }
        public virtual byte WorkflowId { get; set; }
        public virtual IList<WorkflowStatusOption> WorkStatusOptionList { get; set; }
    }
}