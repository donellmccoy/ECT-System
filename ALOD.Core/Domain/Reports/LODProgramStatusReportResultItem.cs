using ALOD.Core.Utils;
using ALOD.Logging;
using System;
using System.Data;

namespace ALOD.Core.Domain.Reports
{
    public class LODProgramStatusReportResultItem : LODStatusDaysResultItem
    {
        public LODProgramStatusReportResultItem()
        {
            SetPropertiesToDefaultValues();
        }

        private LODProgramStatusReportResultItem(DataRow row)
        { }

        public virtual DateTime? DateCompleted { get; set; }

        /// <inheritdoc/>
        public override bool ExtractFromDataRow(DataRow row)
        {
            try
            {
                bool baseResult = base.ExtractFromDataRow(row);

                this.DateCompleted = DataHelpers.GetNullableDateTimeFromDataRow("DateCompleted", row);

                return (baseResult && true);
            }
            catch (Exception e)
            {
                SetPropertiesToDefaultValues();
                LogManager.LogError(e);
                return false;
            }
        }

        /// <inheritdoc/>
        protected override void SetPropertiesToDefaultValues()
        {
            base.SetPropertiesToDefaultValues();

            this.DateCompleted = null;
        }
    }
}