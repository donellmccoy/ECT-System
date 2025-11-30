using System;

namespace ALOD.Core.Domain.Reports
{
    public class ARCNetLookupReportArgs
    {
        public ARCNetLookupReportArgs()
        {
            EDIPIN = null;
            FirstName = null;
            LastName = null;
            MiddleNames = null;
            BeginDate = null;
            EndDate = null;
        }

        public DateTime? BeginDate { get; set; }
        public string EDIPIN { get; set; }

        public DateTime? EndDate { get; set; }
        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string MiddleNames { get; set; }
    }
}