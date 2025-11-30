using System;

namespace ALOD.Core.Domain.Reports
{
    public class LODTotalCountByProcessReportArgs
    {
        public virtual DateTime? BeginDate { get; set; }
        public virtual DateTime? EndDate { get; set; }
        public virtual int ReportView { get; set; }
        public virtual int UnitId { get; set; }
    }
}