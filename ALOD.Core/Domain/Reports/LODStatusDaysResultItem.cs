using ALOD.Core.Interfaces;
using ALOD.Core.Interfaces.DAOInterfaces;
using ALOD.Core.Utils;
using ALOD.Logging;
using System;
using System.Data;

namespace ALOD.Core.Domain.Reports
{
    public class LODStatusDaysResultItem : IExtractedEntity
    {
        public LODStatusDaysResultItem()
        {
            SetPropertiesToDefaultValues();
        }

        public LODStatusDaysResultItem(LODStatusDaysResultItem item)
        {
            CopyObject(item);
        }

        private LODStatusDaysResultItem(DataRow row)
        { }

        public virtual double? ApprovalAuthorityDays { get; set; }
        public virtual double? BoardAdministratorDays { get; set; }
        public virtual double? BoardJudgeAdvocateDays { get; set; }

        // Board A1
        public virtual double? BoardMedicalOfficerDays { get; set; }

        public virtual ProcessingTimeThresholdStatus BoardStatus { get; set; }
        public virtual double? BoardTechnicianDays { get; set; }
        public virtual string CaseId { get; set; }

        /// <summary>
        /// Gets the summed total of all the days the case has spent in informal Board steps (including the approval authority step).
        /// </summary>
        public virtual double? CombinedBoardDays { get; protected set; }

        /// <summary>
        /// Gets the summed total of all the days the case has spent in formal Board steps (including the approval authority step).
        /// </summary>
        public virtual double? CombinedFormalBoardDays { get; protected set; }

        /// <summary>
        /// Totals day open exlcuding the days spent in the Med Tech Input, Med Off Review, and LOD PM steps.
        /// </summary>
        public virtual double DaysOpen { get; protected set; }

        public virtual int? DaysOpenFromCreation { get; set; }
        public virtual ProcessingTimeThresholdStatus DaysOpenStatus { get; set; }
        public virtual double? FormalApprovalAuthorityDays { get; set; }
        public virtual double? FormalBoardAdministratorDays { get; set; }
        public virtual double? FormalBoardJudgeAdvocateDays { get; set; }

        // Board A1
        public virtual double? FormalBoardMedicalOfficerDays { get; set; }

        public virtual ProcessingTimeThresholdStatus FormalBoardStatus { get; set; }

        // Board SG
        public virtual double? FormalBoardTechnicianDays { get; set; }

        public virtual ProcessingTimeThresholdStatus FormalIOStatus { get; set; }
        public virtual ProcessingTimeThresholdStatus FormalWingCCStatus { get; set; }
        public virtual double? FormalWingCommanderDays { get; set; }
        public virtual ProcessingTimeThresholdStatus FormalWingJAStatus { get; set; }
        public virtual double? FormalWingJudgeAdvocateDays { get; set; }
        public virtual double? InvestigatingOfficerDays { get; set; }
        public virtual bool? IsFormal { get; set; }
        public virtual double? MedicalOfficerDays { get; set; }
        public virtual double? MedicalTechnicianDays { get; set; }
        public virtual ProcessingTimeThresholdStatus MedOffStatus { get; set; }

        // Board SG
        public virtual ProcessingTimeThresholdStatus MedTechStatus { get; set; }

        public virtual string MemberUnit { get; set; }
        public virtual string ParentUnit { get; set; }
        public virtual int ParentUnitId { get; set; }
        public virtual double? ProgramManagerDays { get; set; }
        public virtual int? RefId { get; set; }

        // Total days open from case creation to the current date
        public virtual int? TotalCases { get; set; }

        public virtual ProcessingTimeThresholdStatus UnitCCStatus { get; set; }
        public virtual double? UnitCommanderDays { get; set; }
        public virtual int UnitId { get; set; }
        public virtual ProcessingTimeThresholdStatus WingCCStatus { get; set; }
        public virtual double? WingCommanderDays { get; set; }
        public virtual ProcessingTimeThresholdStatus WingJAStatus { get; set; }
        public virtual double? WingJudgeAdvocateDays { get; set; }
        public virtual double? WingSARCDays { get; set; }
        public virtual ProcessingTimeThresholdStatus WingSARCStatus { get; set; }

        public virtual void AverageBy(int divisor)
        {
            if (divisor <= 0)
                return;

            if (MedicalTechnicianDays.HasValue)
                MedicalTechnicianDays = Helpers.NormalRound(MedicalTechnicianDays.Value / divisor);

            if (MedicalOfficerDays.HasValue)
                MedicalOfficerDays = Helpers.NormalRound(MedicalOfficerDays.Value / divisor);

            if (UnitCommanderDays.HasValue)
                UnitCommanderDays = Helpers.NormalRound(UnitCommanderDays.Value / divisor);

            if (WingJudgeAdvocateDays.HasValue)
                WingJudgeAdvocateDays = Helpers.NormalRound(WingJudgeAdvocateDays.Value / divisor);

            if (WingCommanderDays.HasValue)
                WingCommanderDays = Helpers.NormalRound(WingCommanderDays.Value / divisor);

            if (WingSARCDays.HasValue)
                WingSARCDays = Helpers.NormalRound(WingSARCDays.Value / divisor);

            if (ProgramManagerDays.HasValue)
                ProgramManagerDays = Helpers.NormalRound(ProgramManagerDays.Value / divisor);

            if (InvestigatingOfficerDays.HasValue)
                InvestigatingOfficerDays = Helpers.NormalRound(InvestigatingOfficerDays.Value / divisor);

            if (FormalWingJudgeAdvocateDays.HasValue)
                FormalWingJudgeAdvocateDays = Helpers.NormalRound(FormalWingJudgeAdvocateDays.Value / divisor);

            if (FormalWingCommanderDays.HasValue)
                FormalWingCommanderDays = Helpers.NormalRound(FormalWingCommanderDays.Value / divisor);

            if (BoardTechnicianDays.HasValue)
                BoardTechnicianDays = Helpers.NormalRound(BoardTechnicianDays.Value / divisor);

            if (BoardJudgeAdvocateDays.HasValue)
                BoardJudgeAdvocateDays = Helpers.NormalRound(BoardJudgeAdvocateDays.Value / divisor);

            if (BoardAdministratorDays.HasValue)
                BoardAdministratorDays = Helpers.NormalRound(BoardAdministratorDays.Value / divisor);

            if (BoardMedicalOfficerDays.HasValue)
                BoardMedicalOfficerDays = Helpers.NormalRound(BoardMedicalOfficerDays.Value / divisor);

            if (ApprovalAuthorityDays.HasValue)
                ApprovalAuthorityDays = Helpers.NormalRound(ApprovalAuthorityDays.Value / divisor);

            if (FormalBoardTechnicianDays.HasValue)
                FormalBoardTechnicianDays = Helpers.NormalRound(FormalBoardTechnicianDays.Value / divisor);

            if (FormalBoardJudgeAdvocateDays.HasValue)
                FormalBoardJudgeAdvocateDays = Helpers.NormalRound(FormalBoardJudgeAdvocateDays.Value / divisor);

            if (FormalBoardAdministratorDays.HasValue)
                FormalBoardAdministratorDays = Helpers.NormalRound(FormalBoardAdministratorDays.Value / divisor);

            if (FormalBoardMedicalOfficerDays.HasValue)
                FormalBoardMedicalOfficerDays = Helpers.NormalRound(FormalBoardMedicalOfficerDays.Value / divisor);

            if (FormalApprovalAuthorityDays.HasValue)
                FormalApprovalAuthorityDays = Helpers.NormalRound(FormalApprovalAuthorityDays.Value / divisor);

            if (CombinedBoardDays.HasValue)
                CombinedBoardDays = Helpers.NormalRound(CombinedBoardDays.Value / divisor);

            if (CombinedFormalBoardDays.HasValue)
                CombinedFormalBoardDays = Helpers.NormalRound(CombinedFormalBoardDays.Value / divisor);

            DaysOpen = Helpers.NormalRound(DaysOpen / divisor);
        }

        public virtual void CombineItems(LODStatusDaysResultItem item)
        {
            if (MedicalTechnicianDays.HasValue && item.MedicalTechnicianDays.HasValue)
                MedicalTechnicianDays += item.MedicalTechnicianDays;

            if (MedicalOfficerDays.HasValue && item.MedicalOfficerDays.HasValue)
                MedicalOfficerDays += item.MedicalOfficerDays;

            if (UnitCommanderDays.HasValue && item.UnitCommanderDays.HasValue)
                UnitCommanderDays += item.UnitCommanderDays;

            if (WingJudgeAdvocateDays.HasValue && item.WingJudgeAdvocateDays.HasValue)
                WingJudgeAdvocateDays += item.WingJudgeAdvocateDays;

            if (WingCommanderDays.HasValue && item.WingCommanderDays.HasValue)
                WingCommanderDays += item.WingCommanderDays;

            if (WingSARCDays.HasValue && item.WingSARCDays.HasValue)
                WingSARCDays += item.WingSARCDays;

            if (ProgramManagerDays.HasValue && item.ProgramManagerDays.HasValue)
                ProgramManagerDays += item.ProgramManagerDays;

            if (InvestigatingOfficerDays.HasValue && item.InvestigatingOfficerDays.HasValue)
                InvestigatingOfficerDays += item.InvestigatingOfficerDays;

            if (FormalWingJudgeAdvocateDays.HasValue && item.FormalWingJudgeAdvocateDays.HasValue)
                FormalWingJudgeAdvocateDays += item.FormalWingJudgeAdvocateDays;

            if (FormalWingCommanderDays.HasValue && item.FormalWingCommanderDays.HasValue)
                FormalWingCommanderDays += item.FormalWingCommanderDays;

            if (BoardTechnicianDays.HasValue && item.BoardTechnicianDays.HasValue)
                BoardTechnicianDays += item.BoardTechnicianDays;

            if (BoardJudgeAdvocateDays.HasValue && item.BoardJudgeAdvocateDays.HasValue)
                BoardJudgeAdvocateDays += item.BoardJudgeAdvocateDays;

            if (BoardAdministratorDays.HasValue && item.BoardAdministratorDays.HasValue)
                BoardAdministratorDays += item.BoardAdministratorDays;

            if (BoardMedicalOfficerDays.HasValue && item.BoardMedicalOfficerDays.HasValue)
                BoardMedicalOfficerDays += item.BoardMedicalOfficerDays;

            if (ApprovalAuthorityDays.HasValue && item.ApprovalAuthorityDays.HasValue)
                ApprovalAuthorityDays += item.ApprovalAuthorityDays;

            if (FormalBoardTechnicianDays.HasValue && item.FormalBoardTechnicianDays.HasValue)
                FormalBoardTechnicianDays += item.FormalBoardTechnicianDays;

            if (FormalBoardJudgeAdvocateDays.HasValue && item.FormalBoardJudgeAdvocateDays.HasValue)
                FormalBoardJudgeAdvocateDays += item.FormalBoardJudgeAdvocateDays;

            if (FormalBoardAdministratorDays.HasValue && item.FormalBoardAdministratorDays.HasValue)
                FormalBoardAdministratorDays += item.FormalBoardAdministratorDays;

            if (FormalBoardMedicalOfficerDays.HasValue && item.FormalBoardMedicalOfficerDays.HasValue)
                FormalBoardMedicalOfficerDays += item.FormalBoardMedicalOfficerDays;

            if (FormalApprovalAuthorityDays.HasValue && item.FormalApprovalAuthorityDays.HasValue)
                FormalApprovalAuthorityDays += item.FormalApprovalAuthorityDays;

            DaysOpen += item.DaysOpen;

            CombineBoardDays(item);
            CombineFormalBoardDays(item);
        }

        /// <inheritdoc/>
        public virtual bool ExtractFromDataRow(DataRow row)
        {
            try
            {
                this.RefId = DataHelpers.GetNullableIntFromDataRow("RefId", row);
                this.CaseId = DataHelpers.GetStringFromDataRow("CaseId", row);
                this.IsFormal = DataHelpers.GetNullableBoolFromDataRow("IsFormal", row);
                this.DaysOpenFromCreation = DataHelpers.GetNullableIntFromDataRow("DaysOpen", row);
                this.TotalCases = DataHelpers.GetNullableIntFromDataRow("TotalCaseCount", row);
                this.MemberUnit = DataHelpers.GetStringFromDataRow("MemberUnit", row);
                this.UnitId = DataHelpers.GetIntFromDataRow("UnitId", row);

                this.MedicalTechnicianDays = DataHelpers.GetNullableIntFromDataRow("MedTechDays", row);
                this.MedicalOfficerDays = DataHelpers.GetNullableIntFromDataRow("MedOffDays", row);
                this.UnitCommanderDays = DataHelpers.GetNullableIntFromDataRow("UnitCCDays", row);
                this.WingJudgeAdvocateDays = DataHelpers.GetNullableIntFromDataRow("WingJADays", row);
                this.WingCommanderDays = DataHelpers.GetNullableIntFromDataRow("WingCCDays", row);
                this.WingSARCDays = DataHelpers.GetNullableIntFromDataRow("WingSARCDays", row);
                this.ProgramManagerDays = DataHelpers.GetNullableIntFromDataRow("PMDays", row);
                this.InvestigatingOfficerDays = DataHelpers.GetNullableIntFromDataRow("IODays", row);
                this.FormalWingJudgeAdvocateDays = DataHelpers.GetNullableIntFromDataRow("FormalWingJADays", row);
                this.FormalWingCommanderDays = DataHelpers.GetNullableIntFromDataRow("FormalWingCCDays", row);
                this.BoardTechnicianDays = DataHelpers.GetNullableIntFromDataRow("BoardTechDays", row);
                this.BoardJudgeAdvocateDays = DataHelpers.GetNullableIntFromDataRow("BoardJADays", row);
                this.BoardAdministratorDays = DataHelpers.GetNullableIntFromDataRow("BoardAdminDays", row);
                this.BoardMedicalOfficerDays = DataHelpers.GetNullableIntFromDataRow("BoardMedOffDays", row);
                this.ApprovalAuthorityDays = DataHelpers.GetNullableIntFromDataRow("ApprAuthDays", row);

                this.FormalBoardTechnicianDays = DataHelpers.GetNullableIntFromDataRow("FormalBoardTechDays", row);
                this.FormalBoardJudgeAdvocateDays = DataHelpers.GetNullableIntFromDataRow("FormalBoardJADays", row);
                this.FormalBoardAdministratorDays = DataHelpers.GetNullableIntFromDataRow("FormalBoardAdminDays", row);
                this.FormalBoardMedicalOfficerDays = DataHelpers.GetNullableIntFromDataRow("FormalBoardMedOffDays", row);
                this.FormalApprovalAuthorityDays = DataHelpers.GetNullableIntFromDataRow("FormalApprAuthDays", row);

                this.DaysOpenStatus = ProcessingTimeThresholdStatus.Under;
                this.MedTechStatus = ProcessingTimeThresholdStatus.Under;
                this.MedOffStatus = ProcessingTimeThresholdStatus.Under;
                this.UnitCCStatus = ProcessingTimeThresholdStatus.Under;
                this.WingJAStatus = ProcessingTimeThresholdStatus.Under;
                this.WingCCStatus = ProcessingTimeThresholdStatus.Under;
                this.WingSARCStatus = ProcessingTimeThresholdStatus.Under;
                this.BoardStatus = ProcessingTimeThresholdStatus.Under;
                this.FormalBoardStatus = ProcessingTimeThresholdStatus.Under;
                this.FormalIOStatus = ProcessingTimeThresholdStatus.Under;
                this.FormalWingJAStatus = ProcessingTimeThresholdStatus.Under;
                this.FormalWingCCStatus = ProcessingTimeThresholdStatus.Under;

                UpdateDaysOpen();
                UpdateCombinedBoardDays();
                UpdateCombinedFormalBoardDays();

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

        public void UpdateCombinedBoardDays()
        {
            CombinedBoardDays = CalculateCombinedBoardDays();
        }

        public void UpdateCombinedFormalBoardDays()
        {
            CombinedFormalBoardDays = CalculateCombinedFormalBoardDays();
        }

        public void UpdateDaysOpen()
        {
            DaysOpen = CalculateDaysOpen();
        }

        protected double? CalculateCombinedBoardDays()
        {
            // If case has not been to the board then return null...
            if (!BoardTechnicianDays.HasValue &&
                !BoardJudgeAdvocateDays.HasValue &&
                !BoardAdministratorDays.HasValue &&
                !BoardMedicalOfficerDays.HasValue &&
                !ApprovalAuthorityDays.HasValue)
                return null;

            double total = 0;

            if (BoardTechnicianDays.HasValue)
                total += BoardTechnicianDays.Value;

            if (BoardJudgeAdvocateDays.HasValue)
                total += BoardJudgeAdvocateDays.Value;

            if (BoardAdministratorDays.HasValue)
                total += BoardAdministratorDays.Value;

            if (BoardMedicalOfficerDays.HasValue)
                total += BoardMedicalOfficerDays.Value;

            if (ApprovalAuthorityDays.HasValue)
                total += ApprovalAuthorityDays.Value;

            return total;
        }

        protected double CalculateCombinedFormalBoardDays()
        {
            double total = 0;

            if (FormalBoardTechnicianDays.HasValue)
                total += FormalBoardTechnicianDays.Value;

            if (FormalBoardJudgeAdvocateDays.HasValue)
                total += FormalBoardJudgeAdvocateDays.Value;

            if (FormalBoardAdministratorDays.HasValue)
                total += FormalBoardAdministratorDays.Value;

            if (FormalBoardMedicalOfficerDays.HasValue)
                total += FormalBoardMedicalOfficerDays.Value;

            if (FormalApprovalAuthorityDays.HasValue)
                total += FormalApprovalAuthorityDays.Value;

            return total;
        }

        protected double CalculateDaysOpen()
        {
            double days = 0;

            if (UnitCommanderDays.HasValue)
                days += UnitCommanderDays.Value;

            if (WingJudgeAdvocateDays.HasValue)
                days += WingJudgeAdvocateDays.Value;

            if (WingCommanderDays.HasValue)
                days += WingCommanderDays.Value;

            if (InvestigatingOfficerDays.HasValue)
                days += InvestigatingOfficerDays.Value;

            if (FormalWingJudgeAdvocateDays.HasValue)
                days += FormalWingJudgeAdvocateDays.Value;

            if (FormalWingCommanderDays.HasValue)
                days += FormalWingCommanderDays.Value;

            if (BoardTechnicianDays.HasValue)
                days += BoardTechnicianDays.Value;

            if (BoardJudgeAdvocateDays.HasValue)
                days += BoardJudgeAdvocateDays.Value;

            if (BoardAdministratorDays.HasValue)
                days += BoardAdministratorDays.Value;

            if (BoardMedicalOfficerDays.HasValue)
                days += BoardMedicalOfficerDays.Value;

            if (ApprovalAuthorityDays.HasValue)
                days += ApprovalAuthorityDays.Value;

            if (FormalBoardTechnicianDays.HasValue)
                days += FormalBoardTechnicianDays.Value;

            if (FormalBoardJudgeAdvocateDays.HasValue)
                days += FormalBoardJudgeAdvocateDays.Value;

            if (FormalBoardAdministratorDays.HasValue)
                days += FormalBoardAdministratorDays.Value;

            if (FormalBoardMedicalOfficerDays.HasValue)
                days += FormalBoardMedicalOfficerDays.Value;

            if (FormalApprovalAuthorityDays.HasValue)
                days += FormalApprovalAuthorityDays.Value;

            return days;
        }

        protected virtual void CombineBoardDays(LODStatusDaysResultItem item)
        {
            if (!CombinedBoardDays.HasValue && !item.CombinedBoardDays.HasValue)
            {
                UpdateCombinedBoardDays();
            }
            else
            {
                double total = 0;

                if (CombinedBoardDays.HasValue)
                    total += CombinedBoardDays.Value;

                if (item.CombinedBoardDays.HasValue)
                    total += item.CombinedBoardDays.Value;

                CombinedBoardDays = total;
            }
        }

        protected virtual void CombineFormalBoardDays(LODStatusDaysResultItem item)
        {
            if (!CombinedFormalBoardDays.HasValue && !item.CombinedFormalBoardDays.HasValue)
            {
                UpdateCombinedBoardDays();
            }
            else
            {
                double total = 0;

                if (CombinedFormalBoardDays.HasValue)
                    total += CombinedFormalBoardDays.Value;

                if (item.CombinedFormalBoardDays.HasValue)
                    total += item.CombinedFormalBoardDays.Value;

                CombinedFormalBoardDays = total;
            }
        }

        protected virtual void CopyObject(LODStatusDaysResultItem item)
        {
            this.RefId = item.RefId;
            this.CaseId = item.CaseId;
            this.IsFormal = item.IsFormal;
            this.DaysOpenFromCreation = item.DaysOpenFromCreation;
            this.TotalCases = item.TotalCases;
            this.MemberUnit = item.MemberUnit;
            this.UnitId = item.UnitId;

            this.MedicalTechnicianDays = item.MedicalTechnicianDays;
            this.MedicalOfficerDays = item.MedicalOfficerDays;
            this.UnitCommanderDays = item.UnitCommanderDays;
            this.WingJudgeAdvocateDays = item.WingJudgeAdvocateDays;
            this.WingCommanderDays = item.WingCommanderDays;
            this.WingSARCDays = item.WingSARCDays;
            this.ProgramManagerDays = item.ProgramManagerDays;
            this.InvestigatingOfficerDays = item.InvestigatingOfficerDays;
            this.FormalWingJudgeAdvocateDays = item.FormalWingJudgeAdvocateDays;
            this.FormalWingCommanderDays = item.FormalWingCommanderDays;
            this.BoardTechnicianDays = item.BoardTechnicianDays;
            this.BoardJudgeAdvocateDays = item.BoardJudgeAdvocateDays;
            this.BoardAdministratorDays = item.BoardAdministratorDays;
            this.BoardMedicalOfficerDays = item.BoardMedicalOfficerDays;
            this.ApprovalAuthorityDays = item.ApprovalAuthorityDays;

            this.FormalBoardTechnicianDays = item.FormalBoardTechnicianDays;
            this.FormalBoardJudgeAdvocateDays = item.FormalBoardJudgeAdvocateDays;
            this.FormalBoardAdministratorDays = item.FormalBoardAdministratorDays;
            this.FormalBoardMedicalOfficerDays = item.FormalBoardMedicalOfficerDays;
            this.FormalApprovalAuthorityDays = item.FormalApprovalAuthorityDays;

            this.DaysOpenStatus = item.DaysOpenStatus;
            this.MedTechStatus = item.MedTechStatus;
            this.MedOffStatus = item.MedOffStatus;
            this.UnitCCStatus = item.UnitCCStatus;
            this.WingJAStatus = item.WingJAStatus;
            this.WingCCStatus = item.WingCCStatus;
            this.WingSARCStatus = item.WingSARCStatus;
            this.BoardStatus = item.BoardStatus;
            this.FormalBoardStatus = item.FormalBoardStatus;
            this.FormalIOStatus = item.FormalIOStatus;
            this.FormalWingJAStatus = item.FormalWingJAStatus;
            this.FormalWingCCStatus = item.FormalWingCCStatus;

            this.DaysOpen = item.DaysOpen;
            this.CombinedBoardDays = item.CombinedBoardDays;
            this.CombinedFormalBoardDays = item.CombinedFormalBoardDays;
        }

        protected virtual void SetPropertiesToDefaultValues()
        {
            this.RefId = 0;
            this.CaseId = string.Empty;
            this.IsFormal = false;
            this.DaysOpenFromCreation = 0;
            this.MemberUnit = string.Empty;
            this.UnitId = 0;

            this.MedicalTechnicianDays = 0;
            this.MedicalOfficerDays = 0;
            this.UnitCommanderDays = 0;
            this.WingJudgeAdvocateDays = 0;
            this.WingCommanderDays = 0;
            this.WingSARCDays = 0;
            this.ProgramManagerDays = 0;
            this.InvestigatingOfficerDays = 0;
            this.FormalWingJudgeAdvocateDays = 0;
            this.FormalWingCommanderDays = 0;
            this.BoardTechnicianDays = 0;
            this.BoardJudgeAdvocateDays = 0;
            this.BoardAdministratorDays = 0;
            this.BoardMedicalOfficerDays = 0;
            this.ApprovalAuthorityDays = 0;
            this.FormalBoardTechnicianDays = 0;
            this.FormalBoardJudgeAdvocateDays = 0;
            this.FormalBoardAdministratorDays = 0;
            this.FormalBoardMedicalOfficerDays = 0;
            this.FormalApprovalAuthorityDays = 0;

            this.DaysOpenStatus = ProcessingTimeThresholdStatus.Under;
            this.MedTechStatus = ProcessingTimeThresholdStatus.Under;
            this.MedOffStatus = ProcessingTimeThresholdStatus.Under;
            this.UnitCCStatus = ProcessingTimeThresholdStatus.Under;
            this.WingJAStatus = ProcessingTimeThresholdStatus.Under;
            this.WingCCStatus = ProcessingTimeThresholdStatus.Under;
            this.WingSARCStatus = ProcessingTimeThresholdStatus.Under;
            this.BoardStatus = ProcessingTimeThresholdStatus.Under;
            this.FormalBoardStatus = ProcessingTimeThresholdStatus.Under;
            this.FormalIOStatus = ProcessingTimeThresholdStatus.Under;
            this.FormalWingJAStatus = ProcessingTimeThresholdStatus.Under;
            this.FormalWingCCStatus = ProcessingTimeThresholdStatus.Under;

            UpdateDaysOpen();
            UpdateCombinedBoardDays();
            UpdateCombinedFormalBoardDays();
        }
    }
}