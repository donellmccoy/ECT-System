using ALOD.Core.Interfaces;
using ALOD.Core.Interfaces.DAOInterfaces;
using ALOD.Core.Utils;
using ALOD.Logging;
using System;
using System.Data;

namespace ALOD.Core.Domain.Reports
{
    public class LODComplianceAccuracyResultItem : IExtractedEntity
    {
        public LODComplianceAccuracyResultItem()
        {
            SetPropertiesToDefaultValues();
        }

        private LODComplianceAccuracyResultItem(DataRow row)
        { }

        public virtual int Accuracy { get; set; }
        public virtual String Appointing { get; set; }
        public virtual String Approving { get; set; }
        public virtual String CaseID { get; set; }
        public virtual DateTime? DateCompleted { get; set; }
        public virtual String FormalAppointing { get; set; }
        public virtual String FormalApproving { get; set; }
        public virtual String MemberUnit { get; set; }
        public virtual int? RefId { get; set; }
        public virtual int? UnitId { get; set; }
        public virtual bool WasClosedAtWing { get; set; }

        /// <inheritdoc/>
        public bool ExtractFromDataRow(DataRow row)
        {
            try
            {
                this.RefId = DataHelpers.GetNullableIntFromDataRow("refId", row);
                this.CaseID = DataHelpers.GetStringFromDataRow("caseID", row);
                this.DateCompleted = DataHelpers.GetNullableDateTimeFromDataRow("DateCompleted", row);
                this.MemberUnit = DataHelpers.GetStringFromDataRow("MemberUnit", row);
                this.UnitId = DataHelpers.GetNullableIntFromDataRow("UnitId", row);
                this.Appointing = DataHelpers.GetStringFromDataRow("Appointing", row);
                this.Approving = DataHelpers.GetStringFromDataRow("Approving", row);
                this.FormalAppointing = DataHelpers.GetStringFromDataRow("FormalAppointing", row);
                this.FormalApproving = DataHelpers.GetStringFromDataRow("FormalApproving", row);
                this.Accuracy = DataHelpers.GetIntFromDataRow("Accuracy", row);
                this.WasClosedAtWing = false;

                //string approve = null;
                //string appoint = null;

                //if (!string.IsNullOrEmpty(this.FormalAppointing))
                //{
                //    appoint = this.FormalAppointing;
                //}
                //else if (!string.IsNullOrEmpty(this.Appointing))
                //{
                //    appoint = this.Appointing;
                //}

                //if (!string.IsNullOrEmpty(this.FormalApproving))
                //{
                //    approve = this.FormalApproving;
                //}
                //else if (!string.IsNullOrEmpty(this.Approving))
                //{
                //    approve = this.Approving;
                //}

                //if (!string.IsNullOrEmpty(approve) && !string.IsNullOrEmpty(appoint))
                //{
                //    if (approve.Equals(appoint))
                //    {
                //        this.Accuracy = 100;
                //    }
                //    else
                //    {
                //        this.Accuracy = 0;
                //    }
                //}
                //else
                //{
                //    this.Accuracy = 0;
                //}

                if (this.RefId.HasValue && !string.IsNullOrEmpty(this.Appointing) && string.IsNullOrEmpty(Approving) && string.IsNullOrEmpty(FormalAppointing) && string.IsNullOrEmpty(FormalApproving))
                {
                    this.WasClosedAtWing = true;
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
            this.RefId = null;
            this.CaseID = string.Empty;
            this.DateCompleted = null;
            this.MemberUnit = string.Empty;
            this.UnitId = null;
            this.Appointing = string.Empty;
            this.Approving = string.Empty;
            this.FormalAppointing = string.Empty;
            this.FormalApproving = string.Empty;
            this.Accuracy = 0;
            this.WasClosedAtWing = false;
        }
    }
}