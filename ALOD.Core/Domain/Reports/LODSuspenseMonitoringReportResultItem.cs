using ALOD.Core.Utils;
using ALOD.Logging;
using System;
using System.Data;

namespace ALOD.Core.Domain.Reports
{
    public class LODSuspenseMonitoringReportResultItem : LODStatusDaysResultItem
    {
        public LODSuspenseMonitoringReportResultItem()
        {
            SetPropertiesToDefaultValues();
        }

        public LODSuspenseMonitoringReportResultItem(LODSuspenseMonitoringReportResultItem item)
        {
            CopyObject(item);
        }

        private LODSuspenseMonitoringReportResultItem(DataRow row)
        { }

        public virtual string CurrentStatus { get; set; }
        public virtual int CurrentStatusCodeId { get; set; }
        public virtual double? DaysInCurrentStatus { get; set; }        // Total days the case has been in its current status
        public virtual ProcessingTimeThresholdStatus DaysInCurrentStatusStatus { get; set; }

        /// <summary>
        /// Returns TRUE if the case has moved beyond the Med Tech & Med Officer steps.
        /// </summary>
        public virtual bool IsTimeBeingProcessed { get; protected set; }

        public virtual string MemberName { get; set; }

        /// <inheritdoc/>
        public override void AverageBy(int divisor)
        {
            base.AverageBy(divisor);

            if (divisor <= 0)
                return;

            if (DaysInCurrentStatus.HasValue)
                DaysInCurrentStatus = Helpers.NormalRound(DaysInCurrentStatus.Value / divisor);
        }

        /// <inheritdoc/>
        public override void CombineItems(LODStatusDaysResultItem item)
        {
            base.CombineItems(item);

            LODSuspenseMonitoringReportResultItem castedItem = (LODSuspenseMonitoringReportResultItem)item;

            if (DaysInCurrentStatus.HasValue && castedItem.DaysInCurrentStatus.HasValue)
                DaysInCurrentStatus += castedItem.DaysInCurrentStatus;

            UpdateIfTimeIsBeingProcessed();
        }

        /// <inheritdoc/>
        public override bool ExtractFromDataRow(DataRow row)
        {
            try
            {
                bool baseResult = base.ExtractFromDataRow(row);

                this.DaysInCurrentStatus = DataHelpers.GetNullableIntFromDataRow("DaysInCurrentStatus", row);
                this.CurrentStatusCodeId = DataHelpers.GetIntFromDataRow("StatusId", row);
                this.CurrentStatus = DataHelpers.GetStringFromDataRow("Status", row);
                this.MemberName = DataHelpers.GetStringFromDataRow("MemberName", row);
                this.DaysInCurrentStatusStatus = ProcessingTimeThresholdStatus.Under;
                UpdateIfTimeIsBeingProcessed();

                return (baseResult && true);
            }
            catch (Exception e)
            {
                SetPropertiesToDefaultValues();
                LogManager.LogError(e);
                return false;
            }
        }

        public void UpdateIfTimeIsBeingProcessed()
        {
            IsTimeBeingProcessed = DetermineIfTimeIsBeingProcessed();
        }

        /// <inheritdoc/>
        protected override void CopyObject(LODStatusDaysResultItem item)
        {
            base.CopyObject(item);

            LODSuspenseMonitoringReportResultItem castedItem = (LODSuspenseMonitoringReportResultItem)item;

            this.DaysInCurrentStatus = castedItem.DaysInCurrentStatus;
            this.CurrentStatusCodeId = castedItem.CurrentStatusCodeId;
            this.CurrentStatus = castedItem.CurrentStatus;
            this.MemberName = castedItem.MemberName;
            this.DaysInCurrentStatusStatus = castedItem.DaysInCurrentStatusStatus;
            this.IsTimeBeingProcessed = castedItem.IsTimeBeingProcessed;
        }

        protected bool DetermineIfTimeIsBeingProcessed()
        {
            if (UnitCommanderDays.HasValue ||
                WingJudgeAdvocateDays.HasValue ||
                WingCommanderDays.HasValue ||
                InvestigatingOfficerDays.HasValue ||
                FormalWingJudgeAdvocateDays.HasValue ||
                FormalWingCommanderDays.HasValue ||
                BoardTechnicianDays.HasValue ||
                BoardJudgeAdvocateDays.HasValue ||
                BoardAdministratorDays.HasValue ||
                BoardMedicalOfficerDays.HasValue ||
                ApprovalAuthorityDays.HasValue ||
                FormalBoardTechnicianDays.HasValue ||
                FormalBoardJudgeAdvocateDays.HasValue ||
                FormalBoardAdministratorDays.HasValue ||
                FormalBoardMedicalOfficerDays.HasValue ||
                FormalApprovalAuthorityDays.HasValue)
            {
                return true;
            }

            return false;
        }

        /// <inheritdoc/>
        protected override void SetPropertiesToDefaultValues()
        {
            base.SetPropertiesToDefaultValues();

            this.DaysInCurrentStatus = 0;
            this.CurrentStatusCodeId = 0;
            this.CurrentStatus = string.Empty;
            this.MemberName = string.Empty;
            this.DaysInCurrentStatusStatus = ProcessingTimeThresholdStatus.Under;
            UpdateIfTimeIsBeingProcessed();
        }
    }
}