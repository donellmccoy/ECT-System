using System;

namespace ALOD.Core.Domain.Workflow
{
    [Serializable]
    public class WorkStatusTracking : Entity
    {
        public virtual int? CompletedBy { get; set; }

        public virtual TimeSpan DaysInStep
        {
            get
            {
                DateTime end = EndDate.HasValue ? EndDate.Value : DateTime.Now;
                return end.Subtract(StartDate);
            }
        }

        public virtual DateTime? EndDate { get; set; }
        public virtual byte ModuleId { get; set; }
        public virtual int ReferenceId { get; set; }
        public virtual DateTime StartDate { get; set; }
        public virtual WorkStatus WorkflowStatus { get; set; }
    }
}