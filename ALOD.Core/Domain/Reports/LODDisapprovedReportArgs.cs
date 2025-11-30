using System;

namespace ALOD.Core.Domain.Reports
{
    public class LODDisapprovedReportArgs
    {
        public virtual DateTime? BeginDate { get; set; }
        public virtual DateTime? EndDate { get; set; }
        public virtual int? Finding { get; set; }
        public virtual int? PType { get; set; }
        public virtual int? UserId { get; set; }
    }
}