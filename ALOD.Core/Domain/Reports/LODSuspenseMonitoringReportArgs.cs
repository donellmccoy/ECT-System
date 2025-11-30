namespace ALOD.Core.Domain.Reports
{
    public class LODSuspenseMonitoringReportArgs
    {
        public virtual bool? GroupByChildUnits { get; set; }
        public virtual int? UnitId { get; set; }
        public virtual int? ViewType { get; set; }

        public bool IsValid()
        {
            return (
                UnitId.HasValue &&
                ViewType.HasValue &&
                GroupByChildUnits.HasValue
                );
        }
    }
}