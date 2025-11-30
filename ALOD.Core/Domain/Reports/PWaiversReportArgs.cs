using System;

namespace ALOD.Core.Domain.Reports
{
    public class PWaiversReportArgs
    {
        public virtual DateTime? BeginDate { get; set; }
        public virtual DateTime? EndDate { get; set; }
        public virtual bool? IncludeSubordinateUnits { get; set; }
        public virtual short Interval { get; set; }
        public virtual string SSN { get; set; }
        public virtual int UnitId { get; set; }
        public virtual int UserGroupId { get; set; }
    }
}