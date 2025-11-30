using ALOD.Core.Domain.Reports;
using ALOD.Core.Interfaces.DAOInterfaces;
using ALOD.Core.Utils;
using ALOD.Logging;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace ALOD.Data
{
    public class ReportsDao : IReportsDao
    {
        #region SQL DataSource Property

        private SqlDataStore _dataSource;

        private SqlDataStore DataSource
        {
            get
            {
                if (_dataSource == null)
                {
                    _dataSource = new SqlDataStore();
                }
                return _dataSource;
            }
        }

        #endregion SQL DataSource Property

        #region DataSet Extraction...

        private IList<int> ExtractLODComplianceReportInit(DataSet dSet)
        {
            List<int> extractedObjects = new List<int>();

            if (!DataHelpers.IsValidDataSet(dSet))
                return extractedObjects;

            foreach (DataRow row in dSet.Tables[0].Rows)
            {
                int s = DataHelpers.GetIntFromDataRow("lodId", row);

                extractedObjects.Add(s);
            }

            return extractedObjects;
        }

        #endregion DataSet Extraction...

        #region Report Calls...

        /// <inheritdoc/>
        public DataSet ExecuteLODAvgTimesMetricsReport(LODAvgTimesMetricsReportArgs args)
        {
            return DataSource.ExecuteDataSet("report_sp_GetLodMetrics", args.UnitId.Value, args.ReportView.Value, args.SSN, args.BeginDate.Value.Date, args.EndDate.Value.Date, args.IsComplete.Value, args.IncludeSubordinateUnits.Value);
        }

        /// <inheritdoc/>
        public DataSet ExecuteLODCategoryCountDetailsReport(LODCategoryCountReportArgs args)
        {
            return DataSource.ExecuteDataSet("report_sp_GetDetailLodByUnitId", args.UnitId.Value, args.BeginDate.Value.Date, args.EndDate.Value.Date, args.IsComplete.Value);
        }

        /// <inheritdoc/>
        public DataSet ExecuteLODCategoryCountReport(LODCategoryCountReportArgs args)
        {
            return DataSource.ExecuteDataSet("report_sp_GetLodCategoryCounts", args.UnitId.Value, args.ReportView.Value, args.BeginDate.Value.Date, args.EndDate.Value.Date, args.IsComplete.Value);
        }

        /// <inheritdoc/>
        public IList<LODComplianceAccuracyResultItem> ExecuteLODComplianceAccuracyReport(IList<int> list, int unitId, bool groupByChildUnits)
        {
            try
            {
                if (list == null || list.Count == 0)
                    return new List<LODComplianceAccuracyResultItem>();

                SqlCommand sqlCmd = new SqlCommand("report_sp_LODComplianceAccuracy @refIds, @unitId, @groupByChildUnits");

                SqlParameter sqlParam = new SqlParameter("@refIds", SqlDbType.Structured);
                sqlParam.TypeName = "dbo.tblIntegerList";
                sqlParam.Value = CollectionHelpers.IntListToListOfSQLDataRecords(list);

                sqlCmd.Parameters.Add(sqlParam);

                sqlParam = new SqlParameter("@unitId", unitId);
                sqlParam.DbType = DbType.Int32;

                sqlCmd.Parameters.Add(sqlParam);

                sqlParam = new SqlParameter("@groupByChildUnits", groupByChildUnits);
                sqlParam.DbType = DbType.Boolean;

                sqlCmd.Parameters.Add(sqlParam);

                DataSet dSet = DataSource.ExecuteDataSet(sqlCmd);

                return DataHelpers.ExtractObjectsFromDataSet<LODComplianceAccuracyResultItem>(dSet);
            }
            catch (Exception e)
            {
                LogManager.LogError(e);
                return new List<LODComplianceAccuracyResultItem>();
            }
        }

        /// <inheritdoc/>
        public IList<int> ExecuteLODComplianceInit(LODComplianceReportArgs args)
        {
            try
            {
                if (!args.IsValid())
                    return new List<int>();

                DataSet dSet = DataSource.ExecuteDataSet("report_sp_LODCompliance", args.Quarter, args.Year, args.UnitId, args.ViewType);

                return ExtractLODComplianceReportInit(dSet);
            }
            catch (Exception e)
            {
                LogManager.LogError(e);
                return new List<int>();
            }
        }

        /// <inheritdoc/>
        public IList<LODComplianceQualityReportResultItem> ExecuteLODComplianceQualityReport(IList<int> refIds, int unitId, bool groupByChildUnits)
        {
            try
            {
                if (refIds == null || refIds.Count == 0)
                    return new List<LODComplianceQualityReportResultItem>();

                SqlCommand sqlCmd = new SqlCommand("report_sp_LODCompliance_Quality @refIds, @unitId, @groupByChildUnits");

                SqlParameter sqlParam = new SqlParameter("@refIds", SqlDbType.Structured);
                sqlParam.TypeName = "dbo.tblIntegerList";
                sqlParam.Value = CollectionHelpers.IntListToListOfSQLDataRecords(refIds);

                sqlCmd.Parameters.Add(sqlParam);

                sqlParam = new SqlParameter("@unitId", unitId);
                sqlParam.DbType = DbType.Int32;

                sqlCmd.Parameters.Add(sqlParam);

                sqlParam = new SqlParameter("@groupByChildUnits", groupByChildUnits);
                sqlParam.DbType = DbType.Boolean;

                sqlCmd.Parameters.Add(sqlParam);

                DataSet dSet = DataSource.ExecuteDataSet(sqlCmd);

                return DataHelpers.ExtractObjectsFromDataSet<LODComplianceQualityReportResultItem>(dSet);
            }
            catch (Exception e)
            {
                LogManager.LogError(e);
                return new List<LODComplianceQualityReportResultItem>();
            }
        }

        /// <inheritdoc/>
        public IList<LODProgramStatusReportResultItem> ExecuteLODComplianceTimelinessReport(IList<int> refIds, int unitId, bool groupByChildUnits)
        {
            try
            {
                if (refIds == null || refIds.Count == 0)
                    return new List<LODProgramStatusReportResultItem>();

                SqlCommand sqlCmd = new SqlCommand("report_sp_LODCompliance_Timeliness @refIds, @unitId, @groupByChildUnits");

                SqlParameter sqlParam = new SqlParameter("@refIds", SqlDbType.Structured);
                sqlParam.TypeName = "dbo.tblIntegerList";
                sqlParam.Value = CollectionHelpers.IntListToListOfSQLDataRecords(refIds);

                sqlCmd.Parameters.Add(sqlParam);

                sqlParam = new SqlParameter("@unitId", unitId);
                sqlParam.DbType = DbType.Int32;

                sqlCmd.Parameters.Add(sqlParam);

                sqlParam = new SqlParameter("@groupByChildUnits", groupByChildUnits);
                sqlParam.DbType = DbType.Boolean;

                sqlCmd.Parameters.Add(sqlParam);

                DataSet dSet = DataSource.ExecuteDataSet(sqlCmd);

                return DataHelpers.ExtractObjectsFromDataSet<LODProgramStatusReportResultItem>(dSet);
            }
            catch (Exception e)
            {
                LogManager.LogError(e);
                return new List<LODProgramStatusReportResultItem>();
            }
        }

        /// <inheritdoc/>
        public DataSet ExecuteLODDisapprovedDetailsReport(LODDisapprovedReportArgs args)
        {
            return DataSource.ExecuteDataSet("report_sp_GetDetailDisapprovedLod", args.UserId.Value, args.BeginDate.Value.Date, args.EndDate.Value.Date, args.PType.Value, args.Finding.Value);
        }

        /// <inheritdoc/>
        public DataSet ExecuteLODDisapprovedReport(LODDisapprovedReportArgs args)
        {
            return DataSource.ExecuteDataSet("report_sp_GetDisapprovedLod", args.UserId.Value, args.BeginDate.Value.Date, args.EndDate.Value.Date);
        }

        /// <inheritdoc/>
        public DataSet ExecuteLODDispositionReport(LODDispositionReportArgs args)
        {
            return DataSource.ExecuteDataSet("report_sp_GetDisposition", args.UnitId, args.SSN, args.ReportView, args.BeginDate, args.EndDate, args.IsComplete, args.IncludeSubordinateUnits);
        }

        /// <inheritdoc/>
        public DataSet ExecuteLODGraphReport(LODGraphReportArgs args)
        {
            return DataSource.ExecuteDataSet("report_sp_GetLODDaysToComplete", args.BeginDate, args.EndDate, args.IsFormal);
        }

        /// <inheritdoc/>
        public DataSet ExecuteLODMetricsActivitiesCountReport(LODActivitiesMetricsReportArgs args)
        {
            return DataSource.ExecuteDataSet("report_sp_LODActivity_counts", args.UnitId.Value, args.ReportView.Value, args.BeginDate.Value.Date, args.EndDate.Value.Date, args.IsComplete.Value, args.IncludeSubordinateUnits.Value);
        }

        /// <inheritdoc/>
        public DataSet ExecuteLODMetricsActivitiesDetailsReport(LODActivitiesMetricsReportArgs args)
        {
            DataSet result = DataSource.ExecuteDataSet("report_sp_LODActivity", args.UnitId.Value, args.ReportView.Value, args.BeginDate.Value.Date, args.EndDate.Value.Date, args.IsComplete.Value);

            if (args.IncludeSubordinateUnits.Value)
            {
                return result;
            }
            else
            {
                return FilterDataSet(result, "cs_id=" + args.UnitId.Value.ToString());
            }
        }

        /// <inheritdoc/>
        public DataSet ExecuteLODPhysicianCancelledReport(LODPhysicianCancelledReportArgs args)
        {
            return DataSource.ExecuteDataSet("report_sp_GetPhsicianCanceledLOD", args.UnitId, args.ReportView, args.SSN, args.BeginDate, args.EndDate, args.IncludeSubordinateUnits);
        }

        /// <inheritdoc/>
        public IList<LODProgramStatusReportResultItem> ExecuteLODProgramStatusReport(LODProgramStatusReportArgs args)
        {
            try
            {
                if (!args.IsValid())
                    return new List<LODProgramStatusReportResultItem>();

                DataSet dSet = DataSource.ExecuteDataSet("report_sp_LODProgramStatus", args.Quarter, args.Year, args.UnitId, args.ViewType, args.GroupByChildUnits);

                return DataHelpers.ExtractObjectsFromDataSet<LODProgramStatusReportResultItem>(dSet);
            }
            catch (Exception e)
            {
                LogManager.LogError(e);
                return new List<LODProgramStatusReportResultItem>();
            }
        }

        /// <inheritdoc/>
        public DataSet ExecuteLODRWOACountReport(LODRWOAReportArgs args)
        {
            return DataSource.ExecuteDataSet("report_RowaCount", args.BeginDate, args.EndDate, args.Compo);
        }

        /// <inheritdoc/>
        public DataSet ExecuteLODRWOADetailsReport(LODRWOAReportArgs args)
        {
            return DataSource.ExecuteDataSet("report_RowaDetail", args.RWOAId, args.BeginDate, args.EndDate, args.Compo);
        }

        /// <inheritdoc/>
        public IList<LODSARCCasesReportResultItem> ExecuteLODSARCCasesReport(LODSARCCasesReportArgs args)
        {
            try
            {
                if (!args.IsValid())
                    return new List<LODSARCCasesReportResultItem>();

                DataSet dSet = DataSource.ExecuteDataSet("report_sp_LODSARCCases", args.BeginDate.Value.Date, args.EndDate.Value.Date, args.RestrictionStatus.Value, args.CompletionStatus.Value);

                return DataHelpers.ExtractObjectsFromDataSet<LODSARCCasesReportResultItem>(dSet);
            }
            catch (Exception e)
            {
                LogManager.LogError(e);
                return new List<LODSARCCasesReportResultItem>();
            }
        }

        /// <inheritdoc/>
        public DataSet ExecuteLODStatisticsDetailsReport(LODStatisticsReportArgs args)
        {
            return DataSource.ExecuteDataSet("report_sp_DetailStatistics", args.UserId, args.BeginDate, args.EndDate, args.Category);
        }

        /// <inheritdoc/>
        public DataSet ExecuteLODStatisticsReport(LODStatisticsReportArgs args)
        {
            return DataSource.ExecuteDataSet("report_sp_GetLODStatistics", args.UserId, args.BeginDate, args.EndDate);
        }

        /// <inheritdoc/>
        public IList<LODSuspenseMonitoringReportResultItem> ExecuteLODSuspsenseMonitoringReport(LODSuspenseMonitoringReportArgs args)
        {
            try
            {
                if (!args.IsValid())
                    return new List<LODSuspenseMonitoringReportResultItem>();

                DataSet dSet = DataSource.ExecuteDataSet("report_sp_LODSuspsenseMonitoring", args.UnitId, args.ViewType, args.GroupByChildUnits);

                return DataHelpers.ExtractObjectsFromDataSet<LODSuspenseMonitoringReportResultItem>(dSet);
            }
            catch (Exception e)
            {
                LogManager.LogError(e);
                return new List<LODSuspenseMonitoringReportResultItem>();
            }
        }

        /// <inheritdoc/>
        public DataSet ExecuteLODTotalCountByProcessDetailsReport(LODTotalCountByProcessReportArgs args)
        {
            return DataSource.ExecuteDataSet("report_OpenStatusDetail", args.UnitId, args.ReportView, args.BeginDate, args.EndDate);
        }

        /// <inheritdoc/>
        public DataSet ExecuteLODTotalCountByProcessReport(LODTotalCountByProcessReportArgs args)
        {
            return DataSource.ExecuteDataSet("report_OpenStatusCount", args.UnitId, args.ReportView, args.BeginDate, args.EndDate);
        }

        /// <inheritdoc/>
        public DataSet ExecutePackagesReport(DateTime? beginDate, DateTime? endDate)
        {
            return DataSource.ExecuteDataSet("report_pkgLog", beginDate, endDate);
        }

        /// <inheritdoc/>
        public DataSet ExecutePALDocumentsReport(string lastName, string lastFourSSN)
        {
            return DataSource.ExecuteDataSet("report_sp_GetPALDocumentsForMember", lastName, lastFourSSN);
        }

        /// <inheritdoc/>
        public DataSet ExecutePWaiversReport(PWaiversReportArgs args)
        {
            return DataSource.ExecuteDataSet("report_sp_GetParticipationWaivers", args.UnitId, args.SSN, args.BeginDate, args.EndDate, args.Interval, args.UserGroupId, args.IncludeSubordinateUnits);
        }

        /// <inheritdoc/>
        public DataSet ExecuteRFAByGroupReport(DateTime? beginDate, DateTime? endDate)
        {
            return DataSource.ExecuteDataSet("report_sp_GetSpecialCasesRFAByGroup", beginDate, endDate);
        }

        /// <inheritdoc/>
        public DataSet ExecuteRFAByUnitReport(DateTime? beginDate, DateTime? endDate)
        {
            return DataSource.ExecuteDataSet("report_sp_GetSpecialCasesRFAByUnit", beginDate, endDate);
        }

        /// <inheritdoc/>
        public DataSet ExecuteRSDispositionReport(RSDispositionReportArgs args)
        {
            return DataSource.ExecuteDataSet("report_sp_GetRSDisposition", args.UnitId, args.SSN, args.ReportView, args.BeginDate, args.EndDate, args.IsComplete, args.IncludeSubordinateUnits);
        }

        #endregion Report Calls...

        #region Stored Result Calls...

        /// <inheritdoc/>
        public void DeleteAllStoredResults()
        {
            DataSource.ExecuteNonQuery("report_sp_DeleteAllStoredResult");
        }

        /// <inheritdoc/>
        public string GetStoredResult(int userId, string reportTitle)
        {
            Object result = DataSource.ExecuteScalar("report_sp_GetStoredResult", userId, reportTitle);

            if (result == null)
                return string.Empty;

            return result.ToString();
        }

        /// <inheritdoc/>
        public void SaveResult(int userId, string reportTitle, string results)
        {
            DataSource.ExecuteNonQuery("report_sp_SaveStoredResult", userId, reportTitle, results);
        }

        #endregion Stored Result Calls...

        #region Utility Methods...

        protected DataSet FilterDataSet(DataSet ds, string search)
        {
            DataSet clonedDS = ds.Clone();

            foreach (DataRow row in ds.Tables[0].Select(search))
            {
                clonedDS.Tables[0].ImportRow(row);
            }

            return clonedDS;
        }

        #endregion Utility Methods...
    }
}