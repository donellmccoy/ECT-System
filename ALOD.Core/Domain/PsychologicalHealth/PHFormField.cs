using ALOD.Core.Interfaces;
using ALOD.Core.Interfaces.DAOInterfaces;
using ALOD.Core.Utils;
using ALOD.Logging;
using System;
using System.Data;

namespace ALOD.Core.Domain.PsychologicalHealth
{
    public class PHFormField : IExtractedEntity
    {
        public PHFormField()
        {
            SetPropertiesToDefaultValues();
        }

        private PHFormField(DataRow row)
        { }

        private PHFormField(DataRow row, IPsychologicalHealthDao phDao)
        { }

        public virtual PHField Field { get; set; }
        public virtual int FieldDisplayOrder { get; set; }
        public virtual PHFieldType FieldType { get; set; }
        public virtual int FieldTypeDisplayOrder { get; set; }

        /// <summary>
        /// Returns the PH Form Field's SectionId, FieldId, and FieldTypeId as a 3-tuple. These three values represent the field's unique key Id value.
        /// </summary>
        public virtual Tuple<int, int, int> Key
        {
            get
            {
                return new Tuple<int, int, int>(Section.Id, Field.Id, FieldType.Id);
            }
        }

        public virtual PHSection Section { get; set; }
        public virtual string ToolTip { get; set; }

        /// <inheritdoc/>
        public bool ExtractFromDataRow(DataRow row)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public bool ExtractFromDataRow(DataRow row, IDaoFactory daoFactory)
        {
            try
            {
                IPsychologicalHealthDao phDao = daoFactory.GetPsychologicalHealthDao();

                Section = phDao.GetSectionById(DataHelpers.GetIntFromDataRow("SectionId", row));
                Field = phDao.GetFieldById(DataHelpers.GetIntFromDataRow("FieldId", row));
                FieldType = phDao.GetFieldTypeById(DataHelpers.GetIntFromDataRow("FieldTypeId", row));
                FieldDisplayOrder = DataHelpers.GetIntFromDataRow("FieldDisplayOrder", row);
                FieldTypeDisplayOrder = DataHelpers.GetIntFromDataRow("FieldTypeDisplayOrder", row);
                ToolTip = DataHelpers.GetStringFromDataRow("ToolTip", row);

                return true;
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
            if (Section == null || Field == null || FieldType == null)
            {
                return false;
            }

            return true;
        }

        protected void SetPropertiesToDefaultValues()
        {
            Section = null;
            Field = null;
            FieldType = null;
            FieldDisplayOrder = 0;
            FieldTypeDisplayOrder = 0;
            ToolTip = string.Empty;
        }
    }
}