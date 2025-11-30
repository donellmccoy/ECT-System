using ALOD.Core.Domain.Lookup;
using ALOD.Core.Interfaces.DAOInterfaces;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;

namespace ALOD.Core.Domain.PsychologicalHealth
{
    public class PHTotalsReport
    {
        #region Fields...

        public const string WALKABOUTUNITVISITS = "Walkabout/Unit Visits";

        private IList<PHFieldType> _allFieldTypes;
        private IList<PHSection> _allSections;
        private DataSet _comments;
        private IList<PHTotalsReportStringValue> _stringValues;
        private IList<PHFormValue> _suicideMethods;
        private Dictionary<int, long> _suicideMethodTotals;
        private IList<PHFormFieldTotal> _totals;
        private Dictionary<Tuple<int, int, int>, PHFormFieldTotal> _totalsDictionary;

        #endregion Fields...

        #region Properties...

        public virtual DataSet Comments
        {
            get { return _comments; }
        }

        public virtual PHForm Form { get; private set; }

        public virtual bool HasComments
        {
            get
            {
                if (!HasExecuted)
                    return false;

                if (Comments == null || Comments.Tables.Count == 0 || Comments.Tables[0].Rows.Count == 0)
                    return false;

                return true;
            }
        }

        public virtual bool HasExecuted { get; private set; }

        public virtual IPsychologicalHealthDao PHDao { get; private set; }

        public virtual ReadOnlyCollection<PHTotalsReportStringValue> StringValues
        {
            get
            {
                // Construction of the ReadOnlyCollection is a O(1) operation...
                return new ReadOnlyCollection<PHTotalsReportStringValue>(_stringValues);
            }
        }

        public virtual ISuicideMethodDao SuicideMethodDao { get; private set; }

        /// <summary>
        /// The PHFormFieldTotal values calculated in the PHTotalsReport as a read only collection.
        /// </summary>
        public virtual ReadOnlyCollection<PHFormFieldTotal> Totals
        {
            get
            {
                // Construction of the ReadOnlyCollection is a O(1) operation...
                return new ReadOnlyCollection<PHFormFieldTotal>(_totals);
            }
        }

        protected virtual IList<PHFieldType> AllFieldTypes
        {
            get
            {
                if (_allFieldTypes == null)
                    _allFieldTypes = PHDao.GetAllFieldTypes();

                return _allFieldTypes;
            }
        }

        protected virtual IList<PHSection> AllSections
        {
            get
            {
                if (_allSections == null)
                    _allSections = PHDao.GetAllSections();

                return _allSections;
            }
        }

        #endregion Properties...

        #region Constructors...

        public PHTotalsReport()
        {
            this.HasExecuted = false;
            this.Form = null;
            this._totals = new List<PHFormFieldTotal>();
            this._totalsDictionary = new Dictionary<Tuple<int, int, int>, PHFormFieldTotal>();
            this.PHDao = null;
        }

        public PHTotalsReport(IPsychologicalHealthDao phDao, ISuicideMethodDao smDao)
        {
            this.HasExecuted = false;
            this.PHDao = phDao;
            this.SuicideMethodDao = smDao;
            this.Form = new PHForm(0, phDao);               // Using a refId of zero creates a PHForm object without any PHFormFieldValue objects
            this._totals = new List<PHFormFieldTotal>();
        }

        #endregion Constructors...

        #region PHFormFieldTotal Operations...

        /// <summary>
        /// Returns the PHFormFieldTotal associated with the specified id values.
        /// </summary>
        /// <param name="sectionId">Id of the PHSection the PHFormField is associated with.</param>
        /// <param name="fieldId">Id of the PHField the PHFormField is associated with.</param>
        /// <param name="fieldTypeId">Id of the PHFieldType the PHFormField is associated with.</param>
        public virtual PHFormFieldTotal GetFieldTotal(int sectionId, int fieldId, int fieldTypeId)
        {
            if (!HasFieldTotal(sectionId, fieldId, fieldTypeId))
                return null;

            return _totalsDictionary[new Tuple<int, int, int>(sectionId, fieldId, fieldTypeId)];
        }

        /// <summary>
        /// Returns the PHFormFieldTotal associated with the specified 3-int-tuple key.
        /// </summary>
        /// <param name="fieldKey">The 3-int-tuple key associated with the PHFormField.</param>
        public virtual PHFormFieldTotal GetFieldTotal(Tuple<int, int, int> fieldKey)
        {
            if (!HasFieldTotal(fieldKey))
                return null;

            return _totalsDictionary[fieldKey];
        }

        /// <summary>
        /// Returns TRUE if a PHFormFieldTotal associated with the specified PHFormField key Ids exists in this PHForm; otherwise FALSE is returned.
        /// </summary>
        /// <param name="sectionId">Id of the PHSection the PHFormField is associated with.</param>
        /// <param name="fieldId">Id of the PHField the PHFormField is associated with.</param>
        /// <param name="fieldTypeId">Id of the PHFieldType the PHFormField is associated with.</param>
        public virtual bool HasFieldTotal(int sectionId, int fieldId, int fieldTypeId)
        {
            if (_totalsDictionary == null)
                return false;

            return _totalsDictionary.ContainsKey(new Tuple<int, int, int>(sectionId, fieldId, fieldTypeId));
        }

        /// <summary>
        /// Returns TRUE if a PHFormFieldTotal associated with the specified PHFormField key Ids exists in this PHForm; otherwise FALSE is returned.
        /// </summary>
        /// <param name="sectionId">Id of the PHSection the PHFormField is associated with.</param>
        /// <param name="fieldId">Id of the PHField the PHFormField is associated with.</param>
        /// <param name="fieldTypeId">Id of the PHFieldType the PHFormField is associated with.</param>
        public virtual bool HasFieldTotal(Tuple<int, int, int> fieldKey)
        {
            if (_totalsDictionary == null)
                return false;

            return _totalsDictionary.ContainsKey(fieldKey);
        }

        /// <summary>
        /// Builds the _totalsDictionary Dictionary object out of the PHFormFieldTotal objects in the _totals collections
        /// using the PHFormFieldTotal 3-int-Tuple key composed of the SectionId, FieldId, and FieldTypeId values.
        /// </summary>
        private void BuildFieldsDictionary()
        {
            _totalsDictionary = new Dictionary<Tuple<int, int, int>, PHFormFieldTotal>();

            if (Totals == null || Totals.Count == 0)
                return;

            foreach (PHFormFieldTotal t in Totals)
            {
                _totalsDictionary.Add(t.Key, t);
            }
        }

        #endregion PHFormFieldTotal Operations...

        #region Report Operations...

        public bool ExecuteReport(PHTotalsReportArgs args)
        {
            HasExecuted = false;

            if (!args.IsValid())
                return false;

            if (PHDao == null)
                return false;

            if (SuicideMethodDao == null)
                return false;

            // ** EXECUTE BASE TOTALS REPORT ** //
            _totals = PHDao.ExecuteTotalsCannedReport(args);

            if (_totals == null)
                return false;

            // ** EXECUTE TOTALS REPORT ON STRING VALUES ** //
            _stringValues = PHDao.ExecuteTotalsStringValuesCannedReport(args);

            if (_stringValues == null)
                return false;

            // ** EXECUTE TOTALS REPORT ON SUICIDE METHODS ** //
            _suicideMethods = PHDao.ExecuteTotalsSuicideMethodsReport(args);

            if (_suicideMethods == null)
                return false;

            CalculateSuicideMethodTotals();

            // ** EXECUTE TOTALS REPORT ON FORM COMMENTS ** //
            _comments = PHDao.ExecuteTotalsCommentsCannedReport(args);

            if (_comments == null)
                return false;

            HasExecuted = true;

            return true;
        }

        public ReadOnlyCollection<PHFormFieldPair> GetAbusePairs()
        {
            List<PHFormFieldPair> pairs = new List<PHFormFieldPair>();

            if (!HasExecuted)
                return new ReadOnlyCollection<PHFormFieldPair>(pairs);

            int sectionId = AllSections.Where(x => x.Name == "Abuse").ToList().FirstOrDefault().Id;
            int freqFieldTypeId = AllFieldTypes.Where(x => x.Name == "Frequency").ToList().FirstOrDefault().Id;
            int msFieldTypeId = AllFieldTypes.Where(x => x.Name == "Members Seen").ToList().FirstOrDefault().Id;
            PHFormFieldPair p = null;

            // Loop through all frequency values
            foreach (PHFormFieldTotal t in Totals.Where(x => x.Key.Item1 == sectionId && x.Key.Item3 == freqFieldTypeId).ToList())
            {
                p = new PHFormFieldPair();

                p.Field1 = Form.GetField(t.Key);
                p.Field2 = Form.GetField(sectionId, t.Key.Item2, msFieldTypeId);

                p.Value1 = t.Total.ToString();
                p.Value2 = Totals.Where(x => x.Key.Item1 == sectionId && x.Key.Item2 == t.Key.Item2 && x.Key.Item3 == msFieldTypeId).FirstOrDefault().Total.ToString();

                pairs.Add(p);
            }

            return new ReadOnlyCollection<PHFormFieldPair>(pairs);
        }

        public ReadOnlyCollection<PHFormFieldPair> GetPresentingProblemsPairs()
        {
            List<PHFormFieldPair> pairs = new List<PHFormFieldPair>();

            if (!HasExecuted)
                return new ReadOnlyCollection<PHFormFieldPair>(pairs);

            int sectionId = AllSections.Where(x => x.Name == "Presenting Problems").ToList().FirstOrDefault().Id;
            int msFieldTypeId = AllFieldTypes.Where(x => x.Name == "Members Seen").ToList().FirstOrDefault().Id;
            int fouFieldTypeId = AllFieldTypes.Where(x => x.Name == "Follow-Ups").ToList().FirstOrDefault().Id;
            PHFormFieldPair p = null;

            // Loop through all frequency values
            foreach (PHFormFieldTotal t in Totals.Where(x => x.Key.Item1 == sectionId && x.Key.Item3 == msFieldTypeId).ToList())
            {
                p = new PHFormFieldPair();

                p.Field1 = Form.GetField(t.Key);
                p.Field2 = Form.GetField(sectionId, t.Key.Item2, fouFieldTypeId);

                p.Value1 = t.Total.ToString();
                p.Value2 = Totals.Where(x => x.Key.Item1 == sectionId && x.Key.Item2 == t.Key.Item2 && x.Key.Item3 == fouFieldTypeId).FirstOrDefault().Total.ToString();

                pairs.Add(p);
            }

            return new ReadOnlyCollection<PHFormFieldPair>(pairs);
        }

        public ReadOnlyCollection<PHTotalsReportStringValue> GetStringValuesForSections(string parentSectionName)
        {
            if (!HasExecuted || string.IsNullOrEmpty(parentSectionName))
                return new ReadOnlyCollection<PHTotalsReportStringValue>(new List<PHTotalsReportStringValue>());

            PHSection mainSection = AllSections.Where(x => x.Name == parentSectionName).ToList().FirstOrDefault();

            if (mainSection == null)
                return new ReadOnlyCollection<PHTotalsReportStringValue>(new List<PHTotalsReportStringValue>());

            List<PHTotalsReportStringValue> values = new List<PHTotalsReportStringValue>();

            List<int> sectionIds = GetSectionIds(mainSection);

            foreach (PHTotalsReportStringValue value in _stringValues)
            {
                if (sectionIds.Contains(value.Key.Item1))
                {
                    values.Add(value);
                }
            }

            return new ReadOnlyCollection<PHTotalsReportStringValue>(values.OrderBy(x => x.WingRMU).ThenBy(x => x.RawReportingPeriod).ThenBy(x => x.SectionName).ThenBy(x => x.FieldName).ToList());
        }

        public Dictionary<string, long> GetSuicideMethodTotals()
        {
            if (!HasExecuted)
                return new Dictionary<string, long>();

            Dictionary<string, long> totals = new Dictionary<string, long>();

            foreach (SuicideMethod s in SuicideMethodDao.GetAll())
            {
                // Check if there exists a caluclated total for the current suicide method...
                if (_suicideMethodTotals.ContainsKey(s.Id))
                {
                    totals.Add(s.Name, _suicideMethodTotals[s.Id]);
                }
                else
                {
                    totals.Add(s.Name, 0);
                }
            }

            return totals;
        }

        public ReadOnlyCollection<PHFormFieldTotal> GetTotalsBySection(string sectionName)
        {
            List<PHFormFieldTotal> sectionTotals = new List<PHFormFieldTotal>();

            if (!HasExecuted || string.IsNullOrEmpty(sectionName))
                return new ReadOnlyCollection<PHFormFieldTotal>(sectionTotals);

            int sectionId = AllSections.Where(x => x.Name == sectionName).ToList().FirstOrDefault().Id;

            return new ReadOnlyCollection<PHFormFieldTotal>(Totals.Where(x => x.Key.Item1 == sectionId).ToList());
        }

        public ReadOnlyCollection<PHFormFieldPair> GetTrendingActivityFrFuPairs()
        {
            List<PHFormFieldPair> pairs = new List<PHFormFieldPair>();

            if (!HasExecuted)
                return new ReadOnlyCollection<PHFormFieldPair>(pairs);

            int sectionId = AllSections.Where(x => x.Name == "Trending Activity").ToList().FirstOrDefault().Id;
            int freqFieldTypeId = AllFieldTypes.Where(x => x.Name == "Frequency").ToList().FirstOrDefault().Id;
            int fouFieldTypeId = AllFieldTypes.Where(x => x.Name == "Follow-Ups").ToList().FirstOrDefault().Id;
            PHFormFieldPair p = null;

            // Loop through all frequency values
            foreach (PHFormFieldTotal t in Totals.Where(x => x.Key.Item1 == sectionId && x.Key.Item3 == freqFieldTypeId).ToList())
            {
                p = new PHFormFieldPair();

                p.Field1 = Form.GetField(t.Key);
                p.Field2 = Form.GetField(sectionId, t.Key.Item2, fouFieldTypeId);

                p.Value1 = t.Total.ToString();

                if (Totals.Where(x => x.Key.Item1 == sectionId && x.Key.Item2 == t.Key.Item2 && x.Key.Item3 == fouFieldTypeId).ToList().Count < 1)
                    continue;

                p.Value2 = Totals.Where(x => x.Key.Item1 == sectionId && x.Key.Item2 == t.Key.Item2 && x.Key.Item3 == fouFieldTypeId).FirstOrDefault().Total.ToString();

                pairs.Add(p);
            }

            return new ReadOnlyCollection<PHFormFieldPair>(pairs);
        }

        public ReadOnlyCollection<PHFormFieldPair> GetTrendingActivityFrMsPairs()
        {
            List<PHFormFieldPair> pairs = new List<PHFormFieldPair>();

            if (!HasExecuted)
                return new ReadOnlyCollection<PHFormFieldPair>(pairs);

            int sectionId = AllSections.Where(x => x.Name == "Trending Activity").ToList().FirstOrDefault().Id;
            int freqFieldTypeId = AllFieldTypes.Where(x => x.Name == "Frequency").ToList().FirstOrDefault().Id;
            int msFieldTypeId = AllFieldTypes.Where(x => x.Name == "Members Seen").ToList().FirstOrDefault().Id;
            PHFormFieldPair p = null;

            // Loop through all frequency values
            foreach (PHFormFieldTotal t in Totals.Where(x => x.Key.Item1 == sectionId && x.Key.Item3 == freqFieldTypeId).ToList())
            {
                p = new PHFormFieldPair();

                p.Field1 = Form.GetField(t.Key);
                p.Field2 = Form.GetField(sectionId, t.Key.Item2, msFieldTypeId);

                p.Value1 = t.Total.ToString();

                if (Totals.Where(x => x.Key.Item1 == sectionId && x.Key.Item2 == t.Key.Item2 && x.Key.Item3 == msFieldTypeId).ToList().Count < 1)
                    continue;

                p.Value2 = Totals.Where(x => x.Key.Item1 == sectionId && x.Key.Item2 == t.Key.Item2 && x.Key.Item3 == msFieldTypeId).FirstOrDefault().Total.ToString();

                pairs.Add(p);
            }

            return new ReadOnlyCollection<PHFormFieldPair>(pairs);
        }

        public ReadOnlyCollection<PHFormFieldTotal> GetTrendingActivityMiscTotals()
        {
            List<PHFormFieldTotal> sectionTotals = new List<PHFormFieldTotal>();

            if (!HasExecuted)
                return new ReadOnlyCollection<PHFormFieldTotal>(sectionTotals);

            int sectionId = AllSections.Where(x => x.Name == "Trending Activity").ToList().FirstOrDefault().Id;

            // Loop through all fields for this section...
            foreach (PHFormFieldTotal t in Totals.Where(x => x.Key.Item1 == sectionId).ToList())
            {
                // Check if this field has more than one fieldtype associated with it...if so then don't include it in the selected totals...
                if (Totals.Where(x => x.Key.Item1 == sectionId && x.Key.Item2 == t.Key.Item2 && x.Key.Item3 != t.Key.Item3).ToList().Count > 0)
                    continue;

                sectionTotals.Add(t);
            }

            return new ReadOnlyCollection<PHFormFieldTotal>(sectionTotals);
        }

        public ReadOnlyCollection<PHFormFieldTotal> GetWalkaboutUnitVisitsHours()
        {
            List<PHFormFieldTotal> hours = new List<PHFormFieldTotal>();

            if (!HasExecuted)
                return new ReadOnlyCollection<PHFormFieldTotal>(hours);

            int sectionId = AllSections.Where(x => x.Name == WALKABOUTUNITVISITS).ToList().FirstOrDefault().Id;
            int fieldTypeId = AllFieldTypes.Where(x => x.Name == "Hours").ToList().FirstOrDefault().Id;

            return new ReadOnlyCollection<PHFormFieldTotal>(Totals.Where(x => x.Key.Item1 == sectionId && x.Key.Item3 == fieldTypeId).ToList());
        }

        public ReadOnlyCollection<PHFormFieldPair> GetWalkaboutUnitVisitsPairs()
        {
            List<PHFormFieldPair> pairs = new List<PHFormFieldPair>();

            if (!HasExecuted)
                return new ReadOnlyCollection<PHFormFieldPair>(pairs);

            int sectionId = AllSections.Where(x => x.Name == WALKABOUTUNITVISITS).ToList().FirstOrDefault().Id;
            int freqFieldTypeId = AllFieldTypes.Where(x => x.Name == "Frequency").ToList().FirstOrDefault().Id;
            int msFieldTypeId = AllFieldTypes.Where(x => x.Name == "Members Seen").ToList().FirstOrDefault().Id;
            PHFormFieldPair p = null;

            // Loop through all frequency values
            foreach (PHFormFieldTotal t in Totals.Where(x => x.Key.Item1 == sectionId && x.Key.Item3 == freqFieldTypeId).ToList())
            {
                p = new PHFormFieldPair();

                p.Field1 = Form.GetField(t.Key);
                p.Field2 = Form.GetField(sectionId, t.Key.Item2, msFieldTypeId);

                p.Value1 = t.Total.ToString();
                p.Value2 = Totals.Where(x => x.Key.Item1 == sectionId && x.Key.Item2 == t.Key.Item2 && x.Key.Item3 == msFieldTypeId).FirstOrDefault().Total.ToString();

                pairs.Add(p);
            }

            return new ReadOnlyCollection<PHFormFieldPair>(pairs);
        }

        private void CalculateSuicideMethodTotals()
        {
            _suicideMethodTotals = new Dictionary<int, long>();

            if (_suicideMethodTotals == null)
                return;

            if (_suicideMethods == null || _suicideMethods.Count == 0)
                return;

            string[] delimiter = new string[] { "," };

            foreach (PHFormValue v in _suicideMethods)
            {
                // Parse and process each suicide method which was selected for the current PH form value...
                foreach (string s in v.RawValue.Split(delimiter, StringSplitOptions.RemoveEmptyEntries))
                {
                    UpdateSuicideMethodTotal(int.Parse(s));
                }
            }
        }

        private List<int> GetSectionIds(PHSection section)
        {
            if (section == null)
                return new List<int>();

            List<int> rawIds = new List<int>();

            rawIds.Add(section.Id);

            foreach (PHSection c in section.Children)
            {
                rawIds.AddRange(GetSectionIds(c));
            }

            return rawIds.Distinct().ToList();
        }

        private void UpdateSuicideMethodTotal(int methodId)
        {
            if (_suicideMethodTotals == null)
                return;

            // Add initial value or update the existing total for the specified suicide method...
            if (_suicideMethodTotals.ContainsKey(methodId))
            {
                _suicideMethodTotals[methodId]++;
            }
            else
            {
                _suicideMethodTotals.Add(methodId, 1);
            }
        }

        #endregion Report Operations...
    }
}