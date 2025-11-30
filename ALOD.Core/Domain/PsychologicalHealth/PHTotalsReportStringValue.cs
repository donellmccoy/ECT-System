using ALOD.Core.Interfaces;
using ALOD.Core.Interfaces.DAOInterfaces;
using ALOD.Core.Utils;
using ALOD.Logging;
using System;
using System.Data;

namespace ALOD.Core.Domain.PsychologicalHealth
{
    public class PHTotalsReportStringValue : IExtractedEntity
    {
        public PHTotalsReportStringValue()
        {
            SetPropertiesToDefaultValues();
        }

        private PHTotalsReportStringValue(DataRow row)
        { }

        public string FieldName { get; set; }
        public Tuple<int, int, int> Key { get; set; }
        public DateTime RawReportingPeriod { get; set; }
        public string ReportingPeriod { get; set; }
        public string SectionName { get; set; }
        public string Value { get; set; }
        public string WingRMU { get; set; }

        /// <inheritdoc/>
        public bool ExtractFromDataRow(DataRow row)
        {
            try
            {
                Key = new Tuple<int, int, int>(DataHelpers.GetIntFromDataRow("SectionId", row), DataHelpers.GetIntFromDataRow("FieldId", row), DataHelpers.GetIntFromDataRow("FieldTypeId", row));
                RawReportingPeriod = DataHelpers.GetNullableDateTimeFromDataRow("RawReportingPeriod", row).Value;
                WingRMU = DataHelpers.GetStringFromDataRow("PH Wing RMU", row);
                ReportingPeriod = DataHelpers.GetStringFromDataRow("Reporting Period", row);
                SectionName = DataHelpers.GetStringFromDataRow("Section Name", row);
                FieldName = DataHelpers.GetStringFromDataRow("Field Name", row);
                Value = DataHelpers.GetStringFromDataRow("Value", row);

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
            WingRMU = string.Empty;
            ReportingPeriod = string.Empty;
            SectionName = string.Empty;
            FieldName = string.Empty;
            Value = string.Empty;
        }
    }
}