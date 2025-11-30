using System;

namespace ALOD.Core.Domain.Reports
{
    public class LODGraphReportArgs
    {
        public virtual DateTime? BeginDate { get; set; }
        public virtual DateTime? EndDate { get; set; }
        public virtual bool IsFormal { get; set; }
    }
}