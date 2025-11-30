using System;

namespace ALOD.Core.Domain.Reports
{
    public class RSDispositionReportArgs
    {
        public virtual DateTime? BeginDate { get; set; }
        public virtual DateTime? EndDate { get; set; }
        public virtual bool IncludeSubordinateUnits { get; set; }
        public virtual short IsComplete { get; set; }
        public virtual int ReportView { get; set; }
        public virtual string SSN { get; set; }
        public virtual int UnitId { get; set; }
    }
}