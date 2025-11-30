namespace ALOD.Core.Domain.Reports
{
    public class LODComplianceReportArgs
    {
        public virtual bool? GroupByChildUnits { get; set; }
        public virtual int? Quarter { get; set; }
        public virtual int? UnitId { get; set; }
        public virtual int? UserId { get; set; }
        public virtual int? ViewType { get; set; }
        public virtual int? Year { get; set; }

        public bool IsValid()
        {
            return (
                Quarter.HasValue &&
                Year.HasValue &&
                UnitId.HasValue &&
                ViewType.HasValue &&
                GroupByChildUnits.HasValue
                );
        }
    }
}