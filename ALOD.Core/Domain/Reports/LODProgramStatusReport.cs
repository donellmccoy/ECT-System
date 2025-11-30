using ALOD.Core.Domain.Users;
using ALOD.Core.Interfaces.DAOInterfaces;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace ALOD.Core.Domain.Reports
{
    public class LODProgramStatusReport
    {
        #region Fields...

        private IList<LODProgramStatusReportResultItem> _caseViewResults;
        private Dictionary<int, IList<UnitLookup>> _childUnitsDictionary;
        private string _resultsDataKey;
        private IList<LODProgramStatusReportResultItem> _unitViewResults;

        #endregion Fields...

        #region Properties...

        public virtual bool HasExecuted { get; private set; }

        public virtual LODProgramStatusReportArgs ReportArgs { get; private set; }

        public virtual IReportsDao ReportsDao { get; private set; }

        public virtual IReadOnlyCollection<LODProgramStatusReportResultItem> Results
        {
            get
            {
                // Construction of the ReadOnlyCollection is a O(1) operation...
                if (!HasExecuted)
                    return new ReadOnlyCollection<LODProgramStatusReportResultItem>(new List<LODProgramStatusReportResultItem>());

                if (IsCaseViewResults())
                    return new ReadOnlyCollection<LODProgramStatusReportResultItem>(_caseViewResults);
                else
                    return new ReadOnlyCollection<LODProgramStatusReportResultItem>(_unitViewResults);
            }
        }

        public virtual string ResultsDataKey
        {
            get { return _resultsDataKey; }
            private set { _resultsDataKey = value; }
        }

        public virtual IUnitDao UnitDao { get; private set; }

        #endregion Properties...

        #region Constructors...

        public LODProgramStatusReport(IDaoFactory daoFactory)
        {
            this.ResultsDataKey = "UnitId";
            this.HasExecuted = false;
            this.ReportsDao = daoFactory.GetReportsDao();
            this.UnitDao = daoFactory.GetUnitDao();
            this._caseViewResults = new List<LODProgramStatusReportResultItem>();
        }

        private LODProgramStatusReport()
        { }

        #endregion Constructors...

        #region Report Operations...

        public bool ExecuteReport(LODProgramStatusReportArgs args)
        {
            ReportArgs = null;
            HasExecuted = false;

            if (!CanExecuteReport(args))
                return false;

            ReportArgs = args;

            _caseViewResults = ReportsDao.ExecuteLODProgramStatusReport(args);

            if (_caseViewResults == null)
                return false;

            ProcessResults();

            HasExecuted = true;

            return true;
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

        protected LODProgramStatusReportResultItem CalculateAverageOfCases(UnitLookup unit)
        {
            LODProgramStatusReportResultItem average = new LODProgramStatusReportResultItem();
            int divisor = 0;

            foreach (LODProgramStatusReportResultItem item in _caseViewResults)
            {
                if (item.UnitId != unit.Id)
                    continue;

                if (average.UnitId == 0)
                    CopyUnitDataToItem(unit, average);

                divisor++;
                average.CombineItems(item);
            }

            if (divisor == 0)
                return average;

            average.AverageBy(divisor);

            return average;
        }

        protected LODProgramStatusReportResultItem CalculateAverageOfSet(IList<LODProgramStatusReportResultItem> averages, UnitLookup unit)
        {
            LODProgramStatusReportResultItem average = new LODProgramStatusReportResultItem();

            if (averages.Count == 0)
                return null;

            int divisor = 0;

            foreach (LODProgramStatusReportResultItem item in averages)
            {
                if (average.UnitId == 0)
                    CopyUnitDataToItem(unit, average);

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
            _unitViewResults = new List<LODProgramStatusReportResultItem>();
            BuildChildUnitsDictionary();

            //TraverseDownUnitHierarchyAndBuildAveragesBottomUp stores the results we want for the report in _unitViewResults...
            TraverseDownUnitHierarchyAndBuildAveragesBottomUp(UnitDao.GetById(ReportArgs.UnitId.Value).GetUnitLookup(), _unitViewResults);
        }

        protected bool CanExecuteReport(LODProgramStatusReportArgs args)
        {
            if (!args.IsValid())
                return false;

            if (ReportsDao == null)
                return false;

            return true;
        }

        protected void CopyUnitDataToItem(UnitLookup unit, LODProgramStatusReportResultItem item)
        {
            item.UnitId = unit.Id;
            item.MemberUnit = unit.NameAndPasCode;
        }

        protected bool DoCasesForUnitExist(int unitId)
        {
            foreach (LODProgramStatusReportResultItem item in _caseViewResults)
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
            foreach (LODProgramStatusReportResultItem item in _caseViewResults)
            {
                // Check the threshold limits and mark them appropriately...
                item.MedTechStatus = ReportUtility.GetThresholdStatus(item.MedicalTechnicianDays, ReportUtility.INFORMAL_MEDTECH_THRESHOLD, 0);
                item.MedOffStatus = ReportUtility.GetThresholdStatus(item.MedicalOfficerDays, ReportUtility.INFORMAL_MEDOFF_THRESHOLD, 0);
                item.UnitCCStatus = ReportUtility.GetThresholdStatus(item.UnitCommanderDays, ReportUtility.INFORMAL_UNITCC_THRESHOLD, 0);
                item.WingJAStatus = ReportUtility.GetThresholdStatus(item.WingJudgeAdvocateDays, ReportUtility.INFORMAL_WINGJA_THRESHOLD, 0);
                item.WingCCStatus = ReportUtility.GetThresholdStatus(item.WingCommanderDays, ReportUtility.INFORMAL_WINGCC_THRESHOLD, 0);
                item.WingSARCStatus = ReportUtility.GetThresholdStatus(item.WingSARCDays, ReportUtility.INFORMAL_MEDOFF_THRESHOLD, 0);
                item.BoardStatus = ReportUtility.GetThresholdStatus(item.CombinedBoardDays, ReportUtility.INFORMAL_BOARD_THRESHOLD, 0);

                if (item.IsFormal.HasValue && item.IsFormal.Value == true)
                {
                    item.DaysOpenStatus = ReportUtility.GetThresholdStatus(item.DaysOpen, ReportUtility.FORMAL_TOTAL_THRESHOLD, 0);
                    item.FormalIOStatus = ReportUtility.GetThresholdStatus(item.InvestigatingOfficerDays, ReportUtility.FORMAL_IO_THRESHOLD, 0);
                    item.FormalWingJAStatus = ReportUtility.GetThresholdStatus(item.FormalWingJudgeAdvocateDays, ReportUtility.FORMAL_WINGJA_THRESHOLD, 0);
                    item.FormalWingCCStatus = ReportUtility.GetThresholdStatus(item.FormalWingCommanderDays, ReportUtility.FORMAL_WINGCC_THRESHOLD, 0);
                    item.FormalBoardStatus = ReportUtility.GetThresholdStatus(item.CombinedFormalBoardDays, ReportUtility.FORMAL_BOARD_THRESHOLD, 0);
                }
                else
                {
                    item.DaysOpenStatus = ReportUtility.GetThresholdStatus(item.DaysOpen, ReportUtility.INFORMAL_TOTAL_THRESHOLD, 0);
                }
            }
        }

        protected void ProccessStatusesForUnitLevelResults()
        {
            foreach (LODProgramStatusReportResultItem item in _unitViewResults)
            {
                // Check the threshold limits and mark them appropriately...
                item.MedTechStatus = ReportUtility.GetThresholdStatus(item.MedicalTechnicianDays, ReportUtility.INFORMAL_MEDTECH_THRESHOLD, 0);
                item.MedOffStatus = ReportUtility.GetThresholdStatus(item.MedicalOfficerDays, ReportUtility.INFORMAL_MEDOFF_THRESHOLD, 0);
                item.UnitCCStatus = ReportUtility.GetThresholdStatus(item.UnitCommanderDays, ReportUtility.INFORMAL_UNITCC_THRESHOLD, 0);
                item.WingJAStatus = ReportUtility.GetThresholdStatus(item.WingJudgeAdvocateDays, ReportUtility.INFORMAL_WINGJA_THRESHOLD, 0);
                item.WingCCStatus = ReportUtility.GetThresholdStatus(item.WingCommanderDays, ReportUtility.INFORMAL_WINGCC_THRESHOLD, 0);
                item.WingSARCStatus = ReportUtility.GetThresholdStatus(item.WingSARCDays, ReportUtility.INFORMAL_MEDOFF_THRESHOLD, 0);
                item.BoardStatus = ReportUtility.GetThresholdStatus(item.CombinedBoardDays, ReportUtility.INFORMAL_BOARD_THRESHOLD, 0);

                item.DaysOpenStatus = ReportUtility.GetThresholdStatus(item.DaysOpen, ReportUtility.INFORMAL_TOTAL_THRESHOLD, 0);
                item.FormalIOStatus = ReportUtility.GetThresholdStatus(item.InvestigatingOfficerDays, ReportUtility.FORMAL_IO_THRESHOLD, 0);
                item.FormalWingJAStatus = ReportUtility.GetThresholdStatus(item.FormalWingJudgeAdvocateDays, ReportUtility.FORMAL_WINGJA_THRESHOLD, 0);
                item.FormalWingCCStatus = ReportUtility.GetThresholdStatus(item.FormalWingCommanderDays, ReportUtility.FORMAL_WINGCC_THRESHOLD, 0);
                item.FormalBoardStatus = ReportUtility.GetThresholdStatus(item.CombinedFormalBoardDays, ReportUtility.FORMAL_BOARD_THRESHOLD, 0);
            }
        }

        protected void ProcessResults()
        {
            // Check if results have already been processed...
            if (HasExecuted)
                return;

            // Determine which data key need to be used for the results...
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
        /// it then begins to calculate and bubble up the averages of the values in LODProgramStatusReportResultItem.
        ///
        /// Passing in the list of averages for the child units of the Unit that is also being passed in was done in order to avoid duplicating code where
        /// there would be two functions while identical code except that the non-recursive one would be using the class variable _unitViewResults instead
        /// of the local variable averagesOfChildren.
        /// </summary>
        /// <param name="unit">Unit we are calculating the averages of</param>
        /// <param name="averagesOfChildren">The list of averages of the children of Unit</param>
        /// <returns>The average for Unit</returns>
        protected LODProgramStatusReportResultItem TraverseDownUnitHierarchyAndBuildAveragesBottomUp(UnitLookup unit, IList<LODProgramStatusReportResultItem> averagesOfChildren)
        {
            LODProgramStatusReportResultItem currentAverage;

            foreach (UnitLookup childUnitId in GetImmediateChildrenForUnit(unit.Id))
            {
                currentAverage = TraverseDownUnitHierarchyAndBuildAveragesBottomUp(childUnitId, new List<LODProgramStatusReportResultItem>());

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