using ALOD.Core.Domain.Users;
using ALOD.Core.Interfaces.DAOInterfaces;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace ALOD.Core.Domain.Reports
{
    public class LODComplianceReport
    {
        #region Fields...

        private const string REPORT_TITLE = "LODComplianceReport";

        private IList<LODComplianceAccuracyResultItem> _accuracyResults;
        private string _accuracyResultsDataKey;
        private Dictionary<int, IList<UnitLookup>> _childUnitsDictionary;
        private IList<int> _percentageResults;
        private IList<LODComplianceQualityReportResultItem> _qualityResults;
        private string _qualityResultsDataKey;
        private double _selectionRatio = 0.1;
        private IList<LODProgramStatusReportResultItem> _timelinessCaseResults;
        private string _timelinessResultsDataKey;
        private IList<LODProgramStatusReportResultItem> _timelinessUnitViewResults;

        #endregion Fields...

        #region Properties...

        public virtual IReadOnlyCollection<LODComplianceAccuracyResultItem> AccuracyResults
        {
            get
            {
                // Construction of the ReadOnlyCollection is a O(1) operation...
                return new ReadOnlyCollection<LODComplianceAccuracyResultItem>(_accuracyResults);
            }
        }

        public virtual string AccuracyResultsDataKey
        {
            get { return _accuracyResultsDataKey; }
            private set { _accuracyResultsDataKey = value; }
        }

        public virtual bool HasExecuted { get; private set; }

        public virtual double OverallAccuracy { get; private set; }

        public virtual IReadOnlyCollection<LODComplianceQualityReportResultItem> QualityResults
        {
            get
            {
                // Construction of the ReadOnlyCollection is a O(1) operation...
                return new ReadOnlyCollection<LODComplianceQualityReportResultItem>(_qualityResults);
            }
        }

        public virtual string QualityResultsDataKey
        {
            get { return _qualityResultsDataKey; }
            private set { _qualityResultsDataKey = value; }
        }

        public virtual LODComplianceReportArgs ReportArgs { get; private set; }

        public virtual IReportsDao ReportsDao { get; private set; }

        public virtual double SelectionRatio
        {
            get
            {
                return _selectionRatio;
            }
            set
            {
                _selectionRatio = value;
            }
        }

        public virtual IReadOnlyCollection<LODProgramStatusReportResultItem> TimelinessResults
        {
            get
            {
                // Construction of the ReadOnlyCollection is a O(1) operation...
                if (IsTimelinessCaseViewResults())
                    return new ReadOnlyCollection<LODProgramStatusReportResultItem>(_timelinessCaseResults);
                else
                    return new ReadOnlyCollection<LODProgramStatusReportResultItem>(_timelinessUnitViewResults);
            }
        }

        public virtual string TimelinessResultsDataKey
        {
            get { return _timelinessResultsDataKey; }
            private set { _timelinessResultsDataKey = value; }
        }

        public virtual IUnitDao UnitDao { get; private set; }

        #endregion Properties...

        #region Constructors...

        public LODComplianceReport(IDaoFactory daoFactory)
        {
            this.QualityResultsDataKey = "UnitId";
            this.TimelinessResultsDataKey = "UnitId";
            this.AccuracyResultsDataKey = "UnitId";
            this.HasExecuted = false;
            this.ReportsDao = daoFactory.GetReportsDao();
            this.UnitDao = daoFactory.GetUnitDao();
            this._qualityResults = new List<LODComplianceQualityReportResultItem>();
            this._timelinessCaseResults = new List<LODProgramStatusReportResultItem>();
            this._accuracyResults = new List<LODComplianceAccuracyResultItem>();
            this._percentageResults = new List<int>();
        }

        private LODComplianceReport()
        {
            this.QualityResultsDataKey = "UnitId";
            this.TimelinessResultsDataKey = "UnitId";
            this.AccuracyResultsDataKey = "UnitId";
            this.HasExecuted = false;
            this.ReportsDao = null;
            this._qualityResults = new List<LODComplianceQualityReportResultItem>();
            this._timelinessCaseResults = new List<LODProgramStatusReportResultItem>();
            this._accuracyResults = new List<LODComplianceAccuracyResultItem>();
            this._percentageResults = new List<int>();
        }

        #endregion Constructors...

        #region Report Operations...

        public bool ExecuteReport(LODComplianceReportArgs args)
        {
            ReportArgs = null;
            HasExecuted = false;

            if (!CanExecuteReport(args))
                return false;

            IList<int> extractedObjects = ReportsDao.ExecuteLODComplianceInit(args);

            if (extractedObjects == null)
                return false;

            ReportArgs = args;

            if (extractedObjects.Count == 0)
            {
                HasExecuted = true;
                return true;
            }

            ExtractPercentageResults(extractedObjects);
            SavePercentageResults(args.UserId.Value, REPORT_TITLE);

            _qualityResults = ReportsDao.ExecuteLODComplianceQualityReport(_percentageResults, args.UnitId.Value, args.GroupByChildUnits.Value);
            _timelinessCaseResults = ReportsDao.ExecuteLODComplianceTimelinessReport(_percentageResults, args.UnitId.Value, args.GroupByChildUnits.Value);
            _accuracyResults = ReportsDao.ExecuteLODComplianceAccuracyReport(_percentageResults, args.UnitId.Value, args.GroupByChildUnits.Value);

            ProcessResults();

            HasExecuted = true;

            return true;
        }

        public bool ExecuteReportAgainstStoredResults(LODComplianceReportArgs args)
        {
            ReportArgs = null;
            HasExecuted = false;

            if (!CanExecuteReport(args))
                return false;

            ReportArgs = args;

            ExtractStoredResult(args.UserId.Value);

            _qualityResults = ReportsDao.ExecuteLODComplianceQualityReport(_percentageResults, args.UnitId.Value, args.GroupByChildUnits.Value);
            _timelinessCaseResults = ReportsDao.ExecuteLODComplianceTimelinessReport(_percentageResults, args.UnitId.Value, args.GroupByChildUnits.Value);
            _accuracyResults = ReportsDao.ExecuteLODComplianceAccuracyReport(_percentageResults, args.UnitId.Value, args.GroupByChildUnits.Value);

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

            foreach (LODProgramStatusReportResultItem item in _timelinessCaseResults)
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

        protected double CalculateOverallAccuracyOfFindings()
        {
            double totalAccuracy = 0;
            double count = 0;

            foreach (LODComplianceAccuracyResultItem item in _accuracyResults)
            {
                if (item.WasClosedAtWing)
                    continue;

                totalAccuracy += item.Accuracy;
                count++;
            }

            return Math.Round((totalAccuracy / count), 1, MidpointRounding.AwayFromZero);
        }

        protected void CalculateUnitViewAveragesResults()
        {
            _timelinessUnitViewResults = new List<LODProgramStatusReportResultItem>();
            BuildChildUnitsDictionary();

            //TraverseDownUnitHierarchyAndBuildAveragesBottomUp stores the results we want for the report in _unitViewResults...
            TraverseDownUnitHierarchyAndBuildAveragesBottomUp(UnitDao.GetById(ReportArgs.UnitId.Value).GetUnitLookup(), _timelinessUnitViewResults);
        }

        protected bool CanExecuteReport(LODComplianceReportArgs args)
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

        protected void DetermineAccuracyResultsDataKey()
        {
            if (IsAccuracyCaseViewResults())
                AccuracyResultsDataKey = "RefId";
            else
                AccuracyResultsDataKey = "UnitId";
        }

        protected void DetermineQualityResultsDataKey()
        {
            if (IsQualityCaseViewResults())
                QualityResultsDataKey = "RefId";
            else
                QualityResultsDataKey = "UnitId";
        }

        protected bool DoCasesForUnitExist(int unitId)
        {
            foreach (LODProgramStatusReportResultItem item in _timelinessCaseResults)
            {
                if (item.UnitId == unitId)
                {
                    return true;
                }
            }

            return false;
        }

        protected void ExtractPercentageResults(IList<int> extractedObjects)
        {
            // Randomly select a X% portion of the extracted objects...
            var rand = new Random();
            int percent = (int)(extractedObjects.Count * SelectionRatio);

            if (percent < 1)
                percent = 1;

            for (int count = percent; count != 0; count--)
            {
                int other = rand.Next(0, extractedObjects.Count - 1);
                _percentageResults.Add(extractedObjects[other]);
                extractedObjects.Remove(extractedObjects[other]);
            }
        }

        protected void ExtractStoredResult(int userId)
        {
            _percentageResults = new List<int>();
            string storedResults = ReportsDao.GetStoredResult(userId, REPORT_TITLE);

            if (string.IsNullOrEmpty(storedResults))
                return;

            int currentValue = 0;

            foreach (string s in storedResults.Split(','))
            {
                if (int.TryParse(s, out currentValue) && currentValue != 0)
                {
                    _percentageResults.Add(currentValue);
                    currentValue = 0;
                }
            }
        }

        protected IList<UnitLookup> GetImmediateChildrenForUnit(int unitId)
        {
            if (!_childUnitsDictionary.ContainsKey(unitId) || _childUnitsDictionary[unitId] == null)
                return new List<UnitLookup>();

            return _childUnitsDictionary[unitId];
        }

        protected bool IsAccuracyCaseViewResults()
        {
            if (_accuracyResults.Count == 0)
                return false;

            if (!_accuracyResults[0].RefId.HasValue)
                return false;

            return true;
        }

        protected bool IsQualityCaseViewResults()
        {
            if (_qualityResults.Count == 0)
                return false;

            if (!_qualityResults[0].RefId.HasValue)
                return false;

            return true;
        }

        protected bool IsTimelinessCaseViewResults()
        {
            if (_timelinessCaseResults.Count == 0)
                return false;

            if (!_timelinessCaseResults[0].RefId.HasValue)
                return false;

            return true;
        }

        protected bool IsValidRWOA(LODComplianceQualityReportResultItem item)
        {
            if (!item.TotalRWOA.HasValue || item.TotalRWOA.Value == 0)
                return false;

            return true;
        }

        protected void ProccessStatusesForCaseLevelResults()
        {
            foreach (LODProgramStatusReportResultItem item in _timelinessCaseResults)
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
            foreach (LODProgramStatusReportResultItem item in _timelinessUnitViewResults)
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

        protected void ProcessAccuracyResults()
        {
            OverallAccuracy = CalculateOverallAccuracyOfFindings();
            DetermineAccuracyResultsDataKey();
        }

        protected void ProcessQualityResults()
        {
            RemoveInvalidRWOAs();
            DetermineQualityResultsDataKey();
        }

        protected void ProcessResults()
        {
            if (HasExecuted)
                return;

            ProcessQualityResults();
            ProcessTimelinessResults();
            ProcessAccuracyResults();
        }

        protected void ProcessTimelinessResults()
        {
            // Check if results have already been processed...
            if (HasExecuted)
                return;

            // Determine which data key need to be used for the results...
            if (IsTimelinessCaseViewResults())
            {
                TimelinessResultsDataKey = "RefId";
                ProccessStatusesForCaseLevelResults();
            }
            else
            {
                TimelinessResultsDataKey = "UnitId";
                CalculateUnitViewAveragesResults();
                ProccessStatusesForUnitLevelResults();
            }
        }

        protected void RemoveInvalidRWOAs()
        {
            List<LODComplianceQualityReportResultItem> toRemove = new List<LODComplianceQualityReportResultItem>();

            foreach (LODComplianceQualityReportResultItem item in _qualityResults)
            {
                if (!IsValidRWOA(item))
                    toRemove.Add(item);
            }

            foreach (LODComplianceQualityReportResultItem item in toRemove)
            {
                _qualityResults.Remove(item);
            }
        }

        protected void SavePercentageResults(int userId, string reportTitle)
        {
            string resultsCSV = string.Empty;

            for (int i = 0; i < _percentageResults.Count; i++)
            {
                if (i != 0)
                    resultsCSV += ",";

                resultsCSV += _percentageResults[i].ToString();
            }

            if (!string.IsNullOrEmpty(resultsCSV))
                ReportsDao.SaveResult(userId, reportTitle, resultsCSV);
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