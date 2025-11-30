using System;

namespace ALOD.Core.Domain.Reports
{
    public class LODSARCCasesReportArgs
    {
        public virtual DateTime? BeginDate { get; set; }
        public virtual int? CompletionStatus { get; set; }
        public virtual DateTime? EndDate { get; set; }
        public virtual int? RestrictionStatus { get; set; }

        public bool IsValid()
        {
            return (BeginDate.HasValue &&
                    EndDate.HasValue &&
                    RestrictionStatus.HasValue &&
                    CompletionStatus.HasValue
                );
        }
    }
}