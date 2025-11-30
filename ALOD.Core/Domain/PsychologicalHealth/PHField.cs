using ALOD.Core.Interfaces;
using ALOD.Core.Interfaces.DAOInterfaces;
using ALOD.Core.Utils;
using ALOD.Logging;
using System;
using System.Collections.Generic;
using System.Data;

namespace ALOD.Core.Domain.PsychologicalHealth
{
    public class PHField : IExtractedEntity
    {
        public PHField()
        {
            SetPropertiesToDefaultValues();
        }

        private PHField(DataRow row)
        { }

        public virtual IList<PHFieldType> FieldTypes { get; set; }
        public virtual int Id { get; set; }
        public virtual string Name { get; set; }

        /// <inheritdoc/>
        public bool ExtractFromDataRow(DataRow row)
        {
            try
            {
                Id = DataHelpers.GetIntFromDataRow("Id", row);
                Name = DataHelpers.GetStringFromDataRow("Name", row);
                FieldTypes = new List<PHFieldType>();

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
            Id = 0;
            Name = string.Empty;
            FieldTypes = new List<PHFieldType>();
        }
    }
}