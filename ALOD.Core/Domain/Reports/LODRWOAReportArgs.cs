using System;

namespace ALOD.Core.Domain.Reports
{
    public class LODRWOAReportArgs
    {
        public virtual DateTime? BeginDate { get; set; }
        public virtual string Compo { get; set; }
        public virtual DateTime? EndDate { get; set; }
        public virtual int RWOAId { get; set; }
    }
}