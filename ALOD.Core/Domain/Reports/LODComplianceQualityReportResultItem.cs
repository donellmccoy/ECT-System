using ALOD.Core.Interfaces;
using ALOD.Core.Interfaces.DAOInterfaces;
using ALOD.Core.Utils;
using ALOD.Logging;
using System;
using System.Data;

namespace ALOD.Core.Domain.Reports
{
    public class LODComplianceQualityReportResultItem : IExtractedEntity
    {
        public LODComplianceQualityReportResultItem()
        {
            SetPropertiesToDefaultValues();
        }

        private LODComplianceQualityReportResultItem(DataRow row)
        { }

        public virtual string CaseId { get; set; }
        public virtual DateTime? DateCompleted { get; set; }
        public virtual bool IsFormal { get; set; }
        public virtual string MemberUnit { get; set; }
        public virtual int? ReasonEightTotal { get; set; }
        public virtual int? ReasonElevenTotal { get; set; }
        public virtual int? ReasonFiveTotal { get; set; }
        public virtual int? ReasonFourTotal { get; set; }
        public virtual int? ReasonNineTotal { get; set; }
        public virtual int? ReasonOneTotal { get; set; }
        public virtual int? ReasonSevenTotal { get; set; }
        public virtual int? ReasonSixTotal { get; set; }
        public virtual int? ReasonTenTotal { get; set; }
        public virtual int? ReasonThirteenTotal { get; set; }
        public virtual int? ReasonThreeTotal { get; set; }
        public virtual int? ReasonTwelveTotal { get; set; }
        public virtual int? ReasonTwoTotal { get; set; }
        public virtual int? RefId { get; set; }
        public virtual int? TotalRWOA { get; set; }
        public virtual int? UnitId { get; set; }

        /// <inheritdoc/>
        public bool ExtractFromDataRow(DataRow row)
        {
            try
            {
                this.RefId = DataHelpers.GetNullableIntFromDataRow("RefId", row);
                this.CaseId = DataHelpers.GetStringFromDataRow("CaseId", row);
                this.IsFormal = DataHelpers.GetBoolFromDataRow("IsFormal", row);
                this.MemberUnit = DataHelpers.GetStringFromDataRow("MemberUnit", row);
                this.UnitId = DataHelpers.GetIntFromDataRow("UnitId", row);
                this.DateCompleted = DataHelpers.GetNullableDateTimeFromDataRow("DateCompleted", row);

                this.ReasonOneTotal = DataHelpers.GetNullableIntFromDataRow("ReasonOneTotal", row);
                this.ReasonTwoTotal = DataHelpers.GetNullableIntFromDataRow("ReasonTwoTotal", row);
                this.ReasonThreeTotal = DataHelpers.GetNullableIntFromDataRow("ReasonThreeTotal", row);
                this.ReasonFourTotal = DataHelpers.GetNullableIntFromDataRow("ReasonFourTotal", row);
                this.ReasonFiveTotal = DataHelpers.GetNullableIntFromDataRow("ReasonFiveTotal", row);
                this.ReasonSixTotal = DataHelpers.GetNullableIntFromDataRow("ReasonSixTotal", row);
                this.ReasonSevenTotal = DataHelpers.GetNullableIntFromDataRow("ReasonSevenTotal", row);
                this.ReasonEightTotal = DataHelpers.GetNullableIntFromDataRow("ReasonEightTotal", row);
                this.ReasonNineTotal = DataHelpers.GetNullableIntFromDataRow("ReasonNineTotal", row);
                this.ReasonTenTotal = DataHelpers.GetNullableIntFromDataRow("ReasonTenTotal", row);
                this.ReasonElevenTotal = DataHelpers.GetNullableIntFromDataRow("ReasonElevenTotal", row);
                this.ReasonTwelveTotal = DataHelpers.GetNullableIntFromDataRow("ReasonTwelveTotal", row);
                this.ReasonThirteenTotal = DataHelpers.GetNullableIntFromDataRow("ReasonThirteenTotal", row);

                this.TotalRWOA = DataHelpers.GetNullableIntFromDataRow("TotalRWOA", row);

                if (!this.TotalRWOA.HasValue)
                    this.TotalRWOA = CalculateTotalRWOA();

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

        protected int? CalculateTotalRWOA()
        {
            int total = 0;

            if (ReasonOneTotal.HasValue)
                total += ReasonOneTotal.Value;

            if (ReasonTwoTotal.HasValue)
                total += ReasonTwoTotal.Value;

            if (ReasonThreeTotal.HasValue)
                total += ReasonThreeTotal.Value;

            if (ReasonFourTotal.HasValue)
                total += ReasonFourTotal.Value;

            if (ReasonFiveTotal.HasValue)
                total += ReasonFiveTotal.Value;

            if (ReasonSixTotal.HasValue)
                total += ReasonSixTotal.Value;

            if (ReasonSevenTotal.HasValue)
                total += ReasonSevenTotal.Value;

            if (ReasonEightTotal.HasValue)
                total += ReasonEightTotal.Value;

            if (ReasonNineTotal.HasValue)
                total += ReasonNineTotal.Value;

            if (ReasonTenTotal.HasValue)
                total += ReasonTenTotal.Value;

            if (ReasonElevenTotal.HasValue)
                total += ReasonElevenTotal.Value;

            if (ReasonTwelveTotal.HasValue)
                total += ReasonTwelveTotal.Value;

            if (ReasonThirteenTotal.HasValue)
                total += ReasonThirteenTotal.Value;

            if (total == 0)
                return null;
            else
                return total;
        }

        protected void SetPropertiesToDefaultValues()
        {
            this.RefId = 0;
            this.CaseId = string.Empty;
            this.IsFormal = false;
            this.MemberUnit = string.Empty;
            this.UnitId = null;
            this.DateCompleted = null;
            this.TotalRWOA = 0;
            this.ReasonOneTotal = null;
            this.ReasonTwoTotal = null;
            this.ReasonThreeTotal = null;
            this.ReasonFourTotal = null;
            this.ReasonFiveTotal = null;
            this.ReasonSixTotal = null;
            this.ReasonSevenTotal = null;
            this.ReasonEightTotal = null;
            this.ReasonNineTotal = null;
            this.ReasonTenTotal = null;
            this.ReasonElevenTotal = null;
            this.ReasonTwelveTotal = null;
            this.ReasonThirteenTotal = null;
        }
    }
}