using System;

namespace ALOD.Core.Domain.PsychologicalHealth
{
    public class PHTotalsReportArgs
    {
        public virtual DateTime? BeginReportingPeriod { get; set; }
        public virtual short? Collocated { get; set; }
        public virtual DateTime? EndReportingPeriod { get; set; }
        public virtual bool? IncludeSubUnits { get; set; }
        public virtual int? UnitId { get; set; }
        public virtual int? ViewType { get; set; }

        public bool IsValid()
        {
            return (
                IncludeSubUnits.HasValue
                && Collocated.HasValue
                && UnitId.HasValue
                && ViewType.HasValue
                && BeginReportingPeriod.HasValue
                && EndReportingPeriod.HasValue
                );
        }
    }
}