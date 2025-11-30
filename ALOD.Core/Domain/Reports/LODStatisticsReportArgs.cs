using System;

namespace ALOD.Core.Domain.Reports
{
    public class LODStatisticsReportArgs
    {
        public virtual DateTime? BeginDate { get; set; }
        public virtual string Category { get; set; }
        public virtual DateTime? EndDate { get; set; }
        public virtual int UserId { get; set; }
    }
}