using ALOD.Core.Interfaces;
using ALOD.Core.Interfaces.DAOInterfaces;
using ALOD.Core.Utils;
using ALOD.Logging;
using System;
using System.Data;
using System.Drawing;

namespace ALOD.Core.Domain.PsychologicalHealth
{
    public class PHFieldType : IExtractedEntity
    {
        public PHFieldType()
        {
            SetPropertiesToDefaultValues();
        }

        private PHFieldType(DataRow row)
        { }

        public virtual Color? Color { get; set; }
        public virtual string Datasource { get; set; }
        public virtual int DataTypeId { get; set; }
        public virtual int Id { get; set; }
        public virtual int? Length { get; set; }
        public virtual string Name { get; set; }
        public virtual string Placeholder { get; set; }

        /// <inheritdoc/>
        public bool ExtractFromDataRow(DataRow row)
        {
            try
            {
                Id = DataHelpers.GetIntFromDataRow("Id", row);
                Name = DataHelpers.GetStringFromDataRow("Name", row);
                DataTypeId = DataHelpers.GetIntFromDataRow("DataTypeId", row);
                Datasource = DataHelpers.GetStringFromDataRow("Datasource", row);
                Placeholder = DataHelpers.GetStringFromDataRow("Placeholder", row);
                Length = DataHelpers.GetNullableIntFromDataRow("Length", row);

                if (String.IsNullOrEmpty(DataHelpers.GetStringFromDataRow("Color", row)))
                {
                    Color = null;
                }
                else
                {
                    Color = System.Drawing.Color.FromName(DataHelpers.GetStringFromDataRow("Color", row));
                }

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
            DataTypeId = 0;
            Datasource = string.Empty;
            Color = null;
            Length = null;
        }
    }
}