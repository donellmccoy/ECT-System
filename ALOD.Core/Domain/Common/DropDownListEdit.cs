using ALOD.Core.Interfaces;
using ALOD.Core.Interfaces.DAOInterfaces;
using ALOD.Core.Utils;
using ALOD.Logging;
using System;
using System.Data;

namespace ALOD.Core.Domain.Common
{
    public class DropDownListEdit : IExtractedEntity
    {
        public virtual string description { get; set; }
        public virtual int id { get; set; }
        public virtual int sort_order { get; set; }
        public virtual string type { get; set; }

        /// <inheritdoc/>
        public bool ExtractFromDataRow(DataRow row)
        {
            try
            {
                this.id = DataHelpers.GetIntFromDataRow("Id", row);
                this.description = DataHelpers.GetStringFromDataRow("description", row);
                this.type = DataHelpers.GetStringFromDataRow("type", row);
                this.sort_order = DataHelpers.GetIntFromDataRow("sort_order", row);

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
            this.id = 0;
            this.description = "";
            this.type = "";
            this.sort_order = 0;
        }
    }
}