using ALOD.Core.Interfaces;
using ALOD.Core.Interfaces.DAOInterfaces;
using ALOD.Core.Utils;
using ALOD.Logging;
using System;
using System.Data;

namespace ALOD.Core.Domain.Lookup
{
    public class FindingByReasonOf : IExtractedEntity
    {
        public FindingByReasonOf()
        {
            SetPropertiesToDefaultValues();
        }

        private FindingByReasonOf(DataRow row)
        { }

        public virtual string Description { get; set; }
        public virtual int Id { get; set; }

        /// <inheritdoc/>
        public bool ExtractFromDataRow(DataRow row)
        {
            try
            {
                this.Id = DataHelpers.GetIntFromDataRow("Id", row);
                this.Description = DataHelpers.GetStringFromDataRow("Description", row);

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
            return ExtractFromDataRow(row);
        }

        protected void SetPropertiesToDefaultValues()
        {
            this.Id = 0;
            this.Description = string.Empty;
        }
    }
}