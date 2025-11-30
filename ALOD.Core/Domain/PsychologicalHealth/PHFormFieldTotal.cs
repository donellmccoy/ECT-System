using ALOD.Core.Interfaces;
using ALOD.Core.Interfaces.DAOInterfaces;
using ALOD.Core.Utils;
using ALOD.Logging;
using System;
using System.Data;

namespace ALOD.Core.Domain.PsychologicalHealth
{
    public class PHFormFieldTotal : IExtractedEntity
    {
        public PHFormFieldTotal()
        {
            SetPropertiesToDefaultValues();
        }

        private PHFormFieldTotal(DataRow row)
        { }

        public Tuple<int, int, int> Key { get; set; }
        public long Total { get; set; }

        /// <inheritdoc/>
        public bool ExtractFromDataRow(DataRow row)
        {
            try
            {
                Key = new Tuple<int, int, int>(DataHelpers.GetIntFromDataRow("SectionId", row), DataHelpers.GetIntFromDataRow("FieldId", row), DataHelpers.GetIntFromDataRow("FieldTypeId", row));
                Total = DataHelpers.GetLongFromDataRow("Total", row);

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

        protected void SetPropertiesToDefaultValues()
        {
            Key = new Tuple<int, int, int>(0, 0, 0);
            Total = 0;
        }
    }
}