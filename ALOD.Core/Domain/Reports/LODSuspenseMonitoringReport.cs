using ALOD.Core.Domain.Modules.Lod;
using ALOD.Core.Domain.Users;
using ALOD.Core.Interfaces.DAOInterfaces;
using ALOD.Logging;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace ALOD.Core.Domain.Reports
{
    public class LODSuspenseMonitoringReport
    {
        #region Fields...

        private IList<LODSuspenseMonitoringReportResultItem> _caseViewResults;
        private Dictionary<int, IList<UnitLookup>> _childUnitsDictionary;
        private string _resultsDataKey;
        private IList<LODSuspenseMonitoringReportResultItem> _unitViewResults;

        #endregion Fields...

        #region Properties...

        public virtual bool HasExecuted { get; private set; }

        public virtual LODSuspenseMonitoringReportArgs ReportArgs { get; private set; }

        public virtual IReportsDao ReportsDao { get; private set; }

        public virtual IReadOnlyCollection<LODSuspenseMonitoringReportResultItem> Results
        {
            get
            {
                if (!HasExecuted)
                    return new ReadOnlyCollection<LODSuspenseMonitoringReportResultItem>(new List<LODSuspenseMonitoringReportResultItem>());

                // Construction of the ReadOnlyCollection is a O(1) operation...
                if (IsCaseViewResults())
                    return new ReadOnlyCollection<LODSuspenseMonitoringReportResultItem>(_caseViewResults);
                else
                    return new ReadOnlyCollection<LODSuspenseMonitoringReportResultItem>(_unitViewResults);
            }
        }

        /// <summary>
        /// ResultsDataKey keeps track of if the report return results for the Unit View (list of units returned) or Case View (list of cases returned)
        /// </summary>
        public virtual string ResultsDataKey
        {
            get { return _resultsDataKey; }
            private set { _resultsDataKey = value; }
        }

        public virtual IUnitDao UnitDao { get; private set; }

        #endregion Properties...

        #region Constructors...

        public LODSuspenseMonitoringReport(IDaoFactory daoFactory)
        {
            this.ResultsDataKey = "UnitId";
            this.HasExecuted = false;
            this.ReportsDao = daoFactory.GetReportsDao();
            this.UnitDao = daoFactory.GetUnitDao();
            this._caseViewResults = new List<LODSuspenseMonitoringReportResultItem>();
        }

        private LODSuspenseMonitoringReport()
        { }

        #endregion Constructors...

        #region Report Operations...

        public bool ExecuteReport(LODSuspenseMonitoringReportArgs args)
        {
            try
            {
                ReportArgs = null;
                HasExecuted = false;

                if (!CanExecuteReport(args))
                    return false;

                ReportArgs = args;

                _caseViewResults = ReportsDao.ExecuteLODSuspsenseMonitoringReport(args);

                if (_caseViewResults == null)
                    return false;

                ProcessResults();

                HasExecuted = true;

                return true;
            }
            catch (Exception e)
            {
                LogManager.LogError(e);
                return false;
            }
        }

        protected void BuildChildUnitsDictionary()
        {
            _childUnitsDictionary = new Dictionary<int, IList<UnitLookup>>();

            foreach (UnitLookup unit in UnitDao.GetAllSubUnitsForUnit(ReportArgs.UnitId.Value, ReportArgs.ViewType.Value))
            {
                if (!_childUnitsDictionary.ContainsKey(unit.ParentId))
                {
                    _childUnitsDictionary.Add(unit.ParentId, new List<UnitLookup>());
                }

                _childUnitsDictionary[unit.ParentId].Add(unit);
            }
        }

        protected LODSuspenseMonitoringReportResultItem CalculateAverageOfCases(UnitLookup unit)
        {
            LODSuspenseMonitoringReportResultItem average = new LODSuspenseMonitoringReportResultItem();
            int divisor = 0;

            foreach (LODSuspenseMonitoringReportResultItem item in _caseViewResults)
            {
                if (item.UnitId != unit.Id)
                    continue;

                if (average.UnitId == 0)
                    CopyUnitDataToItem(unit, average);

                if (!item.IsTimeBeingProcessed)
                    continue;

                divisor++;
                average.CombineItems(item);
            }

            if (divisor == 0)
                return average;

            average.AverageBy(divisor);

            return average;
        }

        protected LODSuspenseMonitoringReportResultItem CalculateAverageOfSet(IList<LODSuspenseMonitoringReportResultItem> averages, UnitLookup unit)
        {
            LODSuspenseMonitoringReportResultItem average = new LODSuspenseMonitoringReportResultItem();

            if (averages.Count == 0)
                return null;

            int divisor = 0;

            foreach (LODSuspenseMonitoringReportResultItem item in averages)
            {
                if (average.UnitId == 0)
                    CopyUnitDataToItem(unit, average);

                if (!item.IsTimeBeingProcessed)
                    continue;

                divisor++;
                average.CombineItems(item);
            }

            if (divisor == 0)
                return average;

            average.AverageBy(divisor);

            return average;
        }

        protected void CalculateUnitViewAveragesResults()
        {
            _unitViewResults = new List<LODSuspenseMonitoringReportResultItem>();
            BuildChildUnitsDictionary();

            //TraverseDownUnitHierarchyAndBuildAveragesBottomUp stores the results we want for the report in _unitViewResults...
            TraverseDownUnitHierarchyAndBuildAveragesBottomUp(UnitDao.GetById(ReportArgs.UnitId.Value).GetUnitLookup(), _unitViewResults);
        }

        protected bool CanExecuteReport(LODSuspenseMonitoringReportArgs args)
        {
            if (!args.IsValid())
                return false;

            if (ReportsDao == null)
                return false;

            return true;
        }

        protected void CopyUnitDataToItem(UnitLookup unit, LODSuspenseMonitoringReportResultItem item)
        {
            item.UnitId = unit.Id;
            item.MemberUnit = unit.NameAndPasCode;
        }

        protected bool DoCasesForUnitExist(int unitId)
        {
            foreach (LODSuspenseMonitoringReportResultItem item in _caseViewResults)
            {
                if (item.UnitId == unitId)
                {
                    return true;
                }
            }

            return false;
        }

        protected IList<UnitLookup> GetImmediateChildrenForUnit(int unitId)
        {
            if (!_childUnitsDictionary.ContainsKey(unitId) || _childUnitsDictionary[unitId] == null)
                return new List<UnitLookup>();

            return _childUnitsDictionary[unitId];
        }

        protected int GetThresholdForStatusCode(int statusCode)
        {
            switch (statusCode)
            {
                case (int)LodStatusCode.MedTechReview:
                    return ReportUtility.INFORMAL_MEDTECH_THRESHOLD;

                case (int)LodStatusCode.MedicalOfficerReview:
                    return ReportUtility.INFORMAL_MEDOFF_THRESHOLD;

                case (int)LodStatusCode.NotifyFormalInvestigator:
                    return -1;

                case (int)LodStatusCode.UnitCommanderReview:
                    return ReportUtility.INFORMAL_UNITCC_THRESHOLD;

                case (int)LodStatusCode.WingJAReview:
                    return ReportUtility.INFORMAL_WINGJA_THRESHOLD;

                case (int)LodStatusCode.AppointingAutorityReview:
                    return ReportUtility.INFORMAL_WINGCC_THRESHOLD;

                case (int)LodStatusCode.BoardReview:
                case (int)LodStatusCode.BoardMedicalReview:
                case (int)LodStatusCode.BoardLegalReview:
                case (int)LodStatusCode.BoardPersonnelReview:
                case (int)LodStatusCode.ApprovingAuthorityAction:
                    return ReportUtility.INFORMAL_BOARD_THRESHOLD;

                case (int)LodStatusCode.FormalInvestigation:
                    return ReportUtility.FORMAL_IO_THRESHOLD;

                case (int)LodStatusCode.FormalActionByWingJA:
                    return ReportUtility.FORMAL_WINGJA_THRESHOLD;

                case (int)LodStatusCode.FormalActionByAppointingAuthority:
                    return ReportUtility.FORMAL_WINGCC_THRESHOLD;

                case (int)LodStatusCode.FormalBoardReview:
                case (int)LodStatusCode.FormalBoardMedicalReview:
                case (int)LodStatusCode.FormalBoardLegalReview:
                case (int)LodStatusCode.FormalBoardPersonnelReview:
                case (int)LodStatusCode.FormalApprovingAuthorityAction:
                    return ReportUtility.FORMAL_BOARD_THRESHOLD;

                default:
                    return -1;
            }
        }

        protected int GetToleranceForStatusCode(int statusCode)
        {
            switch (statusCode)
            {
                case (int)LodStatusCode.MedTechReview:
                    return 0;

                case (int)LodStatusCode.MedicalOfficerReview:
                    return 31;

                default:
                    return 5;
            }
        }

        protected bool IsCaseViewResults()
        {
            if (_caseViewResults.Count == 0)
                return false;

            if (!_caseViewResults[0].RefId.HasValue)
                return false;

            return true;
        }

        protected void ProccessStatusesForCaseLevelResults()
        {
            foreach (LODSuspenseMonitoringReportResultItem item in _caseViewResults)
            {
                // Check the threshold limits and mark them appropriately...
                item.DaysInCurrentStatusStatus = ReportUtility.GetThresholdStatus(item.DaysInCurrentStatus, GetThresholdForStatusCode(item.CurrentStatusCodeId), GetToleranceForStatusCode(item.CurrentStatusCodeId));
                item.MedTechStatus = ReportUtility.GetThresholdStatus(item.MedicalTechnicianDays, ReportUtility.INFORMAL_MEDTECH_THRESHOLD, 0);
                item.MedOffStatus = ReportUtility.GetThresholdStatus(item.MedicalOfficerDays, ReportUtility.INFORMAL_MEDOFF_THRESHOLD, 31);
                item.UnitCCStatus = ReportUtility.GetThresholdStatus(item.UnitCommanderDays, ReportUtility.INFORMAL_UNITCC_THRESHOLD, 5);
                item.WingJAStatus = ReportUtility.GetThresholdStatus(item.WingJudgeAdvocateDays, ReportUtility.INFORMAL_WINGJA_THRESHOLD, 5);
                item.WingCCStatus = ReportUtility.GetThresholdStatus(item.WingCommanderDays, ReportUtility.INFORMAL_WINGCC_THRESHOLD, 5);
                item.WingSARCStatus = ReportUtility.GetThresholdStatus(item.WingSARCDays, ReportUtility.INFORMAL_MEDOFF_THRESHOLD, 31);
                item.BoardStatus = ReportUtility.GetThresholdStatus(item.CombinedBoardDays, ReportUtility.INFORMAL_BOARD_THRESHOLD, 5);

                if (item.IsFormal.HasValue && item.IsFormal.Value == true)
                {
                    item.DaysOpenStatus = ReportUtility.GetThresholdStatus(item.DaysOpen, ReportUtility.FORMAL_TOTAL_THRESHOLD, 5);
                    item.FormalIOStatus = ReportUtility.GetThresholdStatus(item.InvestigatingOfficerDays, ReportUtility.FORMAL_IO_THRESHOLD, 5);
                    item.FormalWingJAStatus = ReportUtility.GetThresholdStatus(item.FormalWingJudgeAdvocateDays, ReportUtility.FORMAL_WINGJA_THRESHOLD, 5);
                    item.FormalWingCCStatus = ReportUtility.GetThresholdStatus(item.FormalWingCommanderDays, ReportUtility.FORMAL_WINGCC_THRESHOLD, 5);
                    item.FormalBoardStatus = ReportUtility.GetThresholdStatus(item.CombinedFormalBoardDays, ReportUtility.FORMAL_BOARD_THRESHOLD, 5);
                }
                else
                {
                    item.DaysOpenStatus = ReportUtility.GetThresholdStatus(item.DaysOpen, ReportUtility.INFORMAL_TOTAL_THRESHOLD, 5);
                }
            }
        }

        protected void ProccessStatusesForUnitLevelResults()
        {
            foreach (LODSuspenseMonitoringReportResultItem item in _unitViewResults)
            {
                // Check the threshold limits and mark them appropriately...
                item.MedTechStatus = ReportUtility.GetThresholdStatus(item.MedicalTechnicianDays, ReportUtility.INFORMAL_MEDTECH_THRESHOLD, 0);
                item.MedOffStatus = ReportUtility.GetThresholdStatus(item.MedicalOfficerDays, ReportUtility.INFORMAL_MEDOFF_THRESHOLD, 31);
                item.UnitCCStatus = ReportUtility.GetThresholdStatus(item.UnitCommanderDays, ReportUtility.INFORMAL_UNITCC_THRESHOLD, 5);
                item.WingJAStatus = ReportUtility.GetThresholdStatus(item.WingJudgeAdvocateDays, ReportUtility.INFORMAL_WINGJA_THRESHOLD, 5);
                item.WingCCStatus = ReportUtility.GetThresholdStatus(item.WingCommanderDays, ReportUtility.INFORMAL_WINGCC_THRESHOLD, 5);
                item.WingSARCStatus = ReportUtility.GetThresholdStatus(item.WingSARCDays, ReportUtility.INFORMAL_MEDOFF_THRESHOLD, 31);
                item.BoardStatus = ReportUtility.GetThresholdStatus(item.CombinedBoardDays, ReportUtility.INFORMAL_BOARD_THRESHOLD, 5);

                item.DaysOpenStatus = ReportUtility.GetThresholdStatus(item.DaysOpen, ReportUtility.INFORMAL_TOTAL_THRESHOLD, 5);
                item.FormalIOStatus = ReportUtility.GetThresholdStatus(item.InvestigatingOfficerDays, ReportUtility.FORMAL_IO_THRESHOLD, 5);
                item.FormalWingJAStatus = ReportUtility.GetThresholdStatus(item.FormalWingJudgeAdvocateDays, ReportUtility.FORMAL_WINGJA_THRESHOLD, 5);
                item.FormalWingCCStatus = ReportUtility.GetThresholdStatus(item.FormalWingCommanderDays, ReportUtility.FORMAL_WINGCC_THRESHOLD, 5);
                item.FormalBoardStatus = ReportUtility.GetThresholdStatus(item.CombinedFormalBoardDays, ReportUtility.FORMAL_BOARD_THRESHOLD, 5);
            }
        }

        protected void ProcessResults()
        {
            if (HasExecuted)
                return;

            if (IsCaseViewResults())
            {
                ResultsDataKey = "RefId";
                ProccessStatusesForCaseLevelResults();
            }
            else
            {
                ResultsDataKey = "UnitId";
                CalculateUnitViewAveragesResults();
                ProccessStatusesForUnitLevelResults();
            }
        }

        /// <summary>
        /// TraverseDownUnitHierarchyAndBuildAveragesBottomUp recursively travels down the unit hierarchy. Once it gets to the leafs of the hierarchy
        /// it then begins to calculate and bubble up the averages of the values in LODSuspenseMonitoringReportResultItem.
        ///
        /// Passing in the list of averages for the child units of the Unit that is also being passed in was done in order to avoid duplicating code where
        /// there would be two functions while identical code except that the non-recursive one would be using the class variable _unitViewResults instead
        /// of the local variable averagesOfChildren.
        /// </summary>
        /// <param name="unit">Unit we are calculating the averages of</param>
        /// <param name="averagesOfChildren">The list of averages of the children of Unit</param>
        /// <returns>The average for Unit</returns>
        protected LODSuspenseMonitoringReportResultItem TraverseDownUnitHierarchyAndBuildAveragesBottomUp(UnitLookup unit, IList<LODSuspenseMonitoringReportResultItem> averagesOfChildren)
        {
            LODSuspenseMonitoringReportResultItem currentAverage;

            foreach (UnitLookup childUnitId in GetImmediateChildrenForUnit(unit.Id))
            {
                currentAverage = TraverseDownUnitHierarchyAndBuildAveragesBottomUp(childUnitId, new List<LODSuspenseMonitoringReportResultItem>());

                if (currentAverage != null)
                    averagesOfChildren.Add(currentAverage);
            }

            if (DoCasesForUnitExist(unit.Id))
            {
                averagesOfChildren.Add(CalculateAverageOfCases(unit));
            }

            return CalculateAverageOfSet(averagesOfChildren, unit);
        }

        #endregion Report Operations...
    }
}