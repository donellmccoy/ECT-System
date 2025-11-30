using ALOD.Core.Interfaces;
using ALOD.Core.Interfaces.DAOInterfaces;
using ALOD.Core.Utils;
using ALOD.Logging;
using System;
using System.Data;

namespace ALOD.Core.Domain.PsychologicalHealth
{
    public class PHFormValue : IExtractedEntity
    {
        public PHFormValue()
        {
            SetPropertiesToDefaultValues();
        }

        public PHFormValue(PHFormValue v) : this()
        {
            if (v == null)
                return;

            this.RawValue = v.RawValue;
            this.RefId = v.RefId;
            this.SectionId = v.SectionId;
            this.FieldId = v.FieldId;
            this.FieldTypeId = v.FieldTypeId;
        }

        public PHFormValue(int refId, int sectionId, int fieldId, int fieldTypeId, string value) : this()
        {
            this.RawValue = value;
            this.RefId = refId;
            this.SectionId = sectionId;
            this.FieldId = fieldId;
            this.FieldTypeId = fieldTypeId;
        }

        private PHFormValue(DataRow row) : this()
        {
        }

        public virtual int FieldId { get; set; }
        public virtual int FieldTypeId { get; set; }

        /// <summary>
        /// Returns the PH Form Value's RefId, SectionId, FieldId, and FieldTypeId as a 4-tuple. These four values represent the value's unique key value.
        /// </summary>
        public virtual Tuple<int, int, int, int> Key
        {
            get
            {
                return new Tuple<int, int, int, int>(RefId, SectionId, FieldId, FieldTypeId);
            }
        }

        public virtual string RawValue { get; set; }
        public virtual int RefId { get; set; }
        public virtual int SectionId { get; set; }

        /// <inheritdoc/>
        public bool ExtractFromDataRow(DataRow row)
        {
            try
            {
                this.RawValue = DataHelpers.GetStringFromDataRow("Value", row);
                this.RefId = DataHelpers.GetIntFromDataRow("RefId", row);
                this.SectionId = DataHelpers.GetIntFromDataRow("SectionId", row);
                this.FieldId = DataHelpers.GetIntFromDataRow("FieldId", row);
                this.FieldTypeId = DataHelpers.GetIntFromDataRow("FieldTypeId", row);

                return true;
            }
            catch (Exception e)
            {
                SetPropertiesToDefaultValues();
                LogManager.LogError(e);
                return false;
            }
        }

        /// <inheritdoc/>
        public bool ExtractFromDataRow(DataRow row, IDaoFactory daoFactory)
        {
            try
            {
                return ExtractFromDataRow(row);
            }
            catch (Exception e)
            {
                SetPropertiesToDefaultValues();
                LogManager.LogError(e);
                return false;
            }
        }

        public bool IsValid()
        {
            if (RefId == 0 || SectionId == 0 || FieldId == 0 || FieldTypeId == 0 || RawValue == null)
            {
                return false;
            }

            return true;
        }

        protected void SetPropertiesToDefaultValues()
        {
            this.RawValue = string.Empty;
            this.RefId = 0;
            this.SectionId = 0;
            this.FieldId = 0;
            this.FieldTypeId = 0;
        }
    }
}