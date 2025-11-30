using ALOD.Core.Interfaces.DAOInterfaces;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace ALOD.Core.Domain.PsychologicalHealth
{
    /// <summary>
    /// PHForm represents a single instance of a special case Psychological Health form.
    /// </summary>
    public class PHForm
    {
        #region Fields...

        private IList<PHFormField> _fields;
        private Dictionary<Tuple<int, int, int>, PHFormField> _fieldsDictionary;
        private IList<PHFormValue> _values;
        private Dictionary<Tuple<int, int, int, int>, PHFormValue> _valuesDictionary;

        #endregion Fields...

        #region Properties...

        /// <summary>
        /// The PHFormField values associated with this PHForm object as a read only collection.
        /// </summary>
        public virtual ReadOnlyCollection<PHFormField> Fields
        {
            get
            {
                // Construction of the ReadOnlyCollection is a O(1) operation...
                return new ReadOnlyCollection<PHFormField>(_fields);
            }
        }

        public virtual IPsychologicalHealthDao PHDao { get; private set; }
        public virtual int RefId { get; private set; }

        /// <summary>
        /// The PHFormValue values associated with this PHForm object as a read only collection. To manipulate this
        /// collection of objects use the PHForm's Insert/Update methods.
        /// </summary>
        public virtual ReadOnlyCollection<PHFormValue> Values
        {
            get
            {
                // Construction of the ReadOnlyCollection is a O(1) operation...
                return new ReadOnlyCollection<PHFormValue>(_values);
            }
        }

        #endregion Properties...

        #region Constructors...

        /// <summary>
        /// Constructs an empty PHForm object.
        /// </summary>
        public PHForm()
        {
            this.RefId = 0;
            this._fields = new List<PHFormField>();
            this._values = new List<PHFormValue>();
            this._fieldsDictionary = new Dictionary<Tuple<int, int, int>, PHFormField>();
            this._valuesDictionary = new Dictionary<Tuple<int, int, int, int>, PHFormValue>();
            this.PHDao = null;
        }

        /// <summary>
        /// Constructs a PHForm objects associated with a specific PH case.
        /// </summary>
        /// <param name="refId">The reference ID of the case the PHForm is associated with.</param>
        /// <param name="PHDao">The Data Access Object which will allow the PHForm to grab all of its required data from the database.</param>
        public PHForm(int refId, IPsychologicalHealthDao PHDao)
        {
            this.RefId = refId;
            this.PHDao = PHDao;
            this._fields = PHDao.GetAllFormFields();
            this._values = PHDao.GetFormValuesByRefId(refId);
            BuildFieldsDictionary();
            BuildValuesDictionary();
        }

        #endregion Constructors...

        #region PHFormField Operations...

        /// <summary>
        /// Returns the PHFormField associated with the specified id values.
        /// </summary>
        /// <param name="sectionId">Id of the PHSection the PHFormField is associated with.</param>
        /// <param name="fieldId">Id of the PHField the PHFormField is associated with.</param>
        /// <param name="fieldTypeId">Id of the PHFieldType the PHFormField is associated with.</param>
        public virtual PHFormField GetField(int sectionId, int fieldId, int fieldTypeId)
        {
            if (!HasField(sectionId, fieldId, fieldTypeId))
                return null;

            return _fieldsDictionary[new Tuple<int, int, int>(sectionId, fieldId, fieldTypeId)];
        }

        /// <summary>
        /// Returns the PHFormField associated with the specified 3-int-tuple key.
        /// </summary>
        /// <param name="fieldKey">The 3-int-tuple key associated with the PHFormField.</param>
        public virtual PHFormField GetField(Tuple<int, int, int> fieldKey)
        {
            if (!HasField(fieldKey))
                return null;

            return _fieldsDictionary[fieldKey];
        }

        public virtual ReadOnlyCollection<PHFormField> GetFieldsBySection(int sectionId)
        {
            return new ReadOnlyCollection<PHFormField>(Fields.Where(x => x.Section.Id == sectionId).ToList());
        }

        public virtual IList<PHField> GetFieldsForSection(int sectionId)
        {
            IList<PHField> results = new List<PHField>();
            int currentFieldId = 0;

            foreach (PHFormField ff in Fields.Where(x => x.Section.Id == sectionId).ToList())
            {
                if (ff.Field.Id != currentFieldId)
                {
                    results.Add(ff.Field);
                    currentFieldId = ff.Field.Id;
                }
            }

            return results;
        }

        public virtual PHSection GetTopLevelSectionForChildSection(PHSection child)
        {
            if (child == null)
                return null;

            int count = 0; // Used to avoid an infinite loop...
            PHSection currentSection = child;

            while (currentSection != null && currentSection.IsTopLevel == false && count < 100)
            {
                currentSection = PHDao.GetSectionById(currentSection.ParentId);
                count++;
            }

            if (currentSection == null || currentSection.IsTopLevel == false)
                return null;

            return currentSection;
        }

        /// <summary>
        /// Returns all of the top level PHSection objects associated with the PHForm.
        /// </summary>
        public virtual IList<PHSection> GetTopLevelSections()
        {
            IList<PHSection> results = new List<PHSection>();
            PHSection topLevelSection = null;

            foreach (PHFormField field in Fields)
            {
                topLevelSection = field.Section;

                while (topLevelSection.IsTopLevel == false)
                {
                    topLevelSection = PHDao.GetSectionById(topLevelSection.ParentId);
                }

                if (results.Where(x => x.Id == topLevelSection.Id).ToList().Count == 0)
                {
                    results.Add(topLevelSection);
                }
            }

            return results.OrderBy(x => x.DisplayOrder).ToList();
        }

        /// <summary>
        /// Returns TRUE if a PHFormField associated with the specified PHFormField key Ids exists in this PHForm; otherwise FALSE is returned.
        /// </summary>
        /// <param name="sectionId">Id of the PHSection the PHFormField is associated with.</param>
        /// <param name="fieldId">Id of the PHField the PHFormField is associated with.</param>
        /// <param name="fieldTypeId">Id of the PHFieldType the PHFormField is associated with.</param>
        public virtual bool HasField(int sectionId, int fieldId, int fieldTypeId)
        {
            if (_fieldsDictionary == null)
                return false;

            return _fieldsDictionary.ContainsKey(new Tuple<int, int, int>(sectionId, fieldId, fieldTypeId));
        }

        /// <summary>
        /// Returns TRUE if a PHFormField associated with the specified PHFormField key Ids exists in this PHForm; otherwise FALSE is returned.
        /// </summary>
        /// <param name="sectionId">Id of the PHSection the PHFormField is associated with.</param>
        /// <param name="fieldId">Id of the PHField the PHFormField is associated with.</param>
        /// <param name="fieldTypeId">Id of the PHFieldType the PHFormField is associated with.</param>
        public virtual bool HasField(Tuple<int, int, int> fieldKey)
        {
            if (_fieldsDictionary == null)
                return false;

            return _fieldsDictionary.ContainsKey(fieldKey);
        }

        /// <summary>
        /// Builds the _fieldsDictionary Dictionary object out of the PHFormField objects in the _fields collections
        /// using the PHFormField 3-int-Tuple key composed of the SectionId, FieldId, and FieldTypeId values.
        /// </summary>
        private void BuildFieldsDictionary()
        {
            _fieldsDictionary = new Dictionary<Tuple<int, int, int>, PHFormField>();

            if (Fields == null || Fields.Count == 0)
                return;

            foreach (PHFormField f in Fields)
            {
                _fieldsDictionary.Add(f.Key, f);
            }
        }

        #endregion PHFormField Operations...

        #region PHFormValue Operations...

        /// <summary>
        /// Returns the PHFormValue associated with the specified id values.
        /// </summary>
        /// <param name="sectionId">Id of the PHSection the PHFormValue is associated with.</param>
        /// <param name="fieldId">Id of the PHField the PHFormValue is associated with.</param>
        /// <param name="fieldTypeId">Id of the PHFieldType the PHFormValue is associated with.</param>
        public virtual PHFormValue GetValue(int sectionId, int fieldId, int fieldTypeId)
        {
            return GetValue(new Tuple<int, int, int, int>(RefId, sectionId, fieldId, fieldTypeId));
        }

        /// <summary>
        /// Returns the PHFormValue associated with the specified 3-int-tuple key (SectionId, FieldId, FieldTypeId) plus
        /// the RefId assigned to this PHForm.
        /// </summary>
        /// <param name="valueKey">The 4-int-tuple key associated with the PHFormValue.</param>
        public virtual PHFormValue GetValue(Tuple<int, int, int> valueKey)
        {
            return GetValue(new Tuple<int, int, int, int>(RefId, valueKey.Item1, valueKey.Item2, valueKey.Item3));
        }

        /// <summary>
        /// Returns the PHFormValue associated with the specified 4-int-tuple key (RefId, SectionId, FieldId, FieldTypeId).
        /// </summary>
        /// <param name="valueKey">The 4-int-tuple key associated with the PHFormValue.</param>
        public virtual PHFormValue GetValue(Tuple<int, int, int, int> valueKey)
        {
            if (!HasValue(valueKey.Item2, valueKey.Item3, valueKey.Item4))
                return null;

            return _valuesDictionary[valueKey];
        }

        /// <summary>
        /// Returns TRUE if a PHFormValue associated with the specified PHFormValue key Ids exists in this PHForm; otherwise FALSE is returned.
        /// </summary>
        /// <param name="sectionId">Id of the PHSection the PHFormValue is associated with.</param>
        /// <param name="fieldId">Id of the PHField the PHFormValue is associated with.</param>
        /// <param name="fieldTypeId">Id of the PHFieldType the PHFormValue is associated with.</param>
        public virtual bool HasValue(int sectionId, int fieldId, int fieldTypeId)
        {
            if (_valuesDictionary == null)
                return false;

            return _valuesDictionary.ContainsKey(new Tuple<int, int, int, int>(RefId, sectionId, fieldId, fieldTypeId));
        }

        /// <summary>
        /// Returns TRUE if the specified PHFormValue exists in this PHForm; otherwise FALSE is returned.
        /// </summary>
        /// <param name="value">The PHFormValue being search for in the PHForm.</param>
        public virtual bool HasValue(PHFormValue value)
        {
            if (_valuesDictionary == null)
                return false;

            return HasValue(value.SectionId, value.FieldId, value.FieldTypeId);
        }

        /// <summary>
        /// Adds a new PHFormValue to the PHForm. If this value already exists in
        /// the PHForm values collection then the PHFormValue will be updated.
        /// TRUE will be returned if the insert was successfull; FALSE otherwise.
        /// </summary>
        /// <param name="newValue">The new PHFormVAlue value to be added to the PHForm.</param>
        public virtual bool InsertValue(PHFormValue newValue)
        {
            if (PHDao == null)
                return false;

            if (newValue == null || !newValue.IsValid())
                return false;

            if (newValue.RefId != RefId)
                return false;

            // If this value alreay exists then update it instead...
            if (HasValue(newValue))
            {
                return UpdateValue(newValue);
            }

            if (string.IsNullOrEmpty(newValue.RawValue))
            {
                return false;
            }

            // Attempt to insert the new Form Value into the database...
            bool results = PHDao.InsertFormValue(newValue.RefId, newValue.SectionId, newValue.FieldId, newValue.FieldTypeId, newValue.RawValue);

            if (!results)
                return false;

            // If database insert was successfull then update the PHForm collection objects...
            _values.Add(newValue);
            _valuesDictionary.Add(newValue.Key, newValue);

            return true;
        }

        /// <summary>
        /// Updates the PHFormValue in the PHForm with the specified updateValue.
        /// If the specified PHFormValue exists in the PHForm's value collection then the
        /// value will be updated. If the PHFormValue does not exist then that value will
        /// be inserted.
        /// TRUE will be returned if the update was successfull; FALSE otherwise.
        /// </summary>
        /// <param name="updatedValue">The PHForm's PHFormValue value to update.</param>
        public virtual bool UpdateValue(PHFormValue updatedValue)
        {
            if (PHDao == null)
                return false;

            if (updatedValue == null || !updatedValue.IsValid())
                return false;

            if (updatedValue.RefId != RefId)
                return false;

            // If this value does not exist then insert it instead...
            if (!HasValue(updatedValue))
            {
                return InsertValue(updatedValue);
            }

            // Check if the new value is the same...if so then an update is not needed...
            if (GetValue(updatedValue.Key).RawValue.Equals(updatedValue.RawValue))
                return true;

            bool results = PHDao.UpdateFormValue(updatedValue);

            // Check if the value is null or an empty string...
            if (string.IsNullOrEmpty(updatedValue.RawValue))
            {
                // Delete the value record from the database...there is no need to have a null/empty form value record
                results = PHDao.RemoveFormValue(updatedValue);

                if (!results)
                    return false;

                // If database delete was successfull then remove the value from the PHForm collection objects...
                _valuesDictionary.Remove(updatedValue.Key);

                return true;
            }
            else
            {
                // Attempt to update the Form Value record in the database...
                results = PHDao.UpdateFormValue(updatedValue);

                if (!results)
                    return false;

                // If database update was successfull then update the PHForm collection objects...
                _valuesDictionary[updatedValue.Key].RawValue = updatedValue.RawValue;

                return true;
            }
        }

        /// <summary>
        /// Updates the PHForm with the values from updatedValues. If the PHForm contains the values then the values will be updated.
        /// If the PHForm does not contain a value in updatedValues then that value will be inserted into the PHForm's PHFormValues
        /// collection.
        /// Any PHFormValues unable to be updated will be returned.
        /// </summary>
        /// <param name="updatedValues">The PHFormValue values to update the PHForm with.</param>
        public virtual IList<PHFormValue> UpdateValues(IList<PHFormValue> updatedValues)
        {
            if (PHDao == null)
                return updatedValues;

            IList<PHFormValue> failedUpdates = new List<PHFormValue>();

            foreach (PHFormValue v in updatedValues)
            {
                if (!v.IsValid())
                {
                    failedUpdates.Add(v);
                    continue;   // Move onto the next value...
                }

                if (!UpdateValue(v))
                {
                    failedUpdates.Add(v);
                    continue;   // Move onto the next value...
                }
            }

            return failedUpdates;
        }

        /// <summary>
        /// Builds the _valuesDictionary Dictionary object out of the PHFormValue objects in the _values collection
        /// using the PHFormValue 4-int-Tuple key composed of the RefId, SectionId, FieldId, and FieldTypeId values.
        /// </summary>
        private void BuildValuesDictionary()
        {
            _valuesDictionary = new Dictionary<Tuple<int, int, int, int>, PHFormValue>();

            if (Values == null || Values.Count == 0)
                return;

            foreach (PHFormValue v in Values)
            {
                _valuesDictionary.Add(v.Key, v);
            }
        }

        #endregion PHFormValue Operations...

        #region Misc Operations...

        public virtual bool DoFieldsExistForSection(int sectionId)
        {
            foreach (PHFormField ff in Fields)
            {
                if (ff.Section.Id == sectionId)
                {
                    return true;
                }
            }

            return false;
        }

        #endregion Misc Operations...
    }
}