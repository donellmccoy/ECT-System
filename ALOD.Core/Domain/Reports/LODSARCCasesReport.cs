using ALOD.Core.Interfaces.DAOInterfaces;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace ALOD.Core.Domain.Reports
{
    public class LODSARCCasesReport
    {
        #region Fields...

        private IList<LODSARCCasesReportResultItem> _results;

        #endregion Fields...

        #region Properties...

        public virtual bool HasExecuted { get; private set; }

        public virtual IReportsDao ReportsDao { get; private set; }

        public virtual IReadOnlyCollection<LODSARCCasesReportResultItem> Results
        {
            get
            {
                if (!HasExecuted)
                    return new ReadOnlyCollection<LODSARCCasesReportResultItem>(new List<LODSARCCasesReportResultItem>());

                // Construction of the ReadOnlyCollection is a O(1) operation...
                return new ReadOnlyCollection<LODSARCCasesReportResultItem>(_results);
            }
        }

        #endregion Properties...

        #region Constructors...

        public LODSARCCasesReport(IReportsDao reportsDao)
        {
            this.ReportsDao = reportsDao;
        }

        private LODSARCCasesReport()
        { }

        #endregion Constructors...

        #region Report Operations...

        public bool ExecuteReport(LODSARCCasesReportArgs args)
        {
            HasExecuted = false;

            if (!args.IsValid())
                return false;

            if (ReportsDao == null)
                return false;

            _results = ReportsDao.ExecuteLODSARCCasesReport(args);

            if (_results == null)
                return false;

            ProcessResults();

            HasExecuted = true;

            return true;
        }

        protected void ProcessResults()
        {
            if (HasExecuted)
                return;
        }

        #endregion Report Operations...
    }
}