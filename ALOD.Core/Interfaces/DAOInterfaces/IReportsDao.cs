using ALOD.Core.Domain.Reports;
using System;
using System.Collections.Generic;
using System.Data;

namespace ALOD.Core.Interfaces.DAOInterfaces
{
    public interface IReportsDao
    {
        void DeleteAllStoredResults();

        DataSet ExecuteLODAvgTimesMetricsReport(LODAvgTimesMetricsReportArgs args);

        DataSet ExecuteLODCategoryCountDetailsReport(LODCategoryCountReportArgs args);

        DataSet ExecuteLODCategoryCountReport(LODCategoryCountReportArgs args);

        IList<LODComplianceAccuracyResultItem> ExecuteLODComplianceAccuracyReport(IList<int> list, int unitId, bool groupByChildUnits);

        IList<int> ExecuteLODComplianceInit(LODComplianceReportArgs args);

        IList<LODComplianceQualityReportResultItem> ExecuteLODComplianceQualityReport(IList<int> refIds, int unitId, bool groupByChildUnits);

        IList<LODProgramStatusReportResultItem> ExecuteLODComplianceTimelinessReport(IList<int> refIds, int unitId, bool groupByChildUnits);

        DataSet ExecuteLODDisapprovedDetailsReport(LODDisapprovedReportArgs args);

        DataSet ExecuteLODDisapprovedReport(LODDisapprovedReportArgs args);

        DataSet ExecuteLODDispositionReport(LODDispositionReportArgs args);

        DataSet ExecuteLODGraphReport(LODGraphReportArgs args);

        DataSet ExecuteLODMetricsActivitiesCountReport(LODActivitiesMetricsReportArgs args);

        DataSet ExecuteLODMetricsActivitiesDetailsReport(LODActivitiesMetricsReportArgs args);

        DataSet ExecuteLODPhysicianCancelledReport(LODPhysicianCancelledReportArgs args);

        IList<LODProgramStatusReportResultItem> ExecuteLODProgramStatusReport(LODProgramStatusReportArgs args);

        DataSet ExecuteLODRWOACountReport(LODRWOAReportArgs args);

        DataSet ExecuteLODRWOADetailsReport(LODRWOAReportArgs args);

        IList<LODSARCCasesReportResultItem> ExecuteLODSARCCasesReport(LODSARCCasesReportArgs args);

        DataSet ExecuteLODStatisticsDetailsReport(LODStatisticsReportArgs args);

        DataSet ExecuteLODStatisticsReport(LODStatisticsReportArgs args);

        IList<LODSuspenseMonitoringReportResultItem> ExecuteLODSuspsenseMonitoringReport(LODSuspenseMonitoringReportArgs args);

        DataSet ExecuteLODTotalCountByProcessDetailsReport(LODTotalCountByProcessReportArgs args);

        DataSet ExecuteLODTotalCountByProcessReport(LODTotalCountByProcessReportArgs args);

        DataSet ExecutePackagesReport(DateTime? beginDate, DateTime? endDate);

        DataSet ExecutePALDocumentsReport(string lastName, string lastFourSSN);

        DataSet ExecutePWaiversReport(PWaiversReportArgs args);

        DataSet ExecuteRFAByGroupReport(DateTime? beginDate, DateTime? endDate);

        DataSet ExecuteRFAByUnitReport(DateTime? beginDate, DateTime? endDate);

        DataSet ExecuteRSDispositionReport(RSDispositionReportArgs args);

        string GetStoredResult(int userId, string reportTitle);

        void SaveResult(int userId, string reportTitle, string results);
    }
}