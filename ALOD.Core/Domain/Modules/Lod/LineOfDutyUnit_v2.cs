using ALOD.Core.Domain.Common;
using System;
using System.Collections.Generic;

namespace ALOD.Core.Domain.Modules.Lod
{
    [Serializable]
    public class LineOfDutyUnit_v2 : LineOfDutyUnit
    {
        public LineOfDutyUnit_v2() : base()
        {
            Witnesses = new List<WitnessData>();
        }

        public LineOfDutyUnit_v2(int id) : base(id)
        {
            Witnesses = new List<WitnessData>();
        }

        public virtual DateTime? AbsentFrom { get; set; }
        public virtual DateTime? AbsentTo { get; set; }
        public virtual bool? EightYearRule { get; set; }
        public virtual String IDTStatus { get; set; }
        public virtual bool? IncurredOrAggravated { get; set; }
        public virtual bool? LODInitiation { get; set; }
        public virtual string MemberCredible { get; set; }
        public virtual int? MemberOccurrence { get; set; }
        public virtual string MemberOnOrders { get; set; }
        public virtual bool? MemberRequest { get; set; }
        public virtual String OrdersAttached { get; set; }
        public virtual bool? PCARSAttached { get; set; }
        public virtual bool? PCARSHistory { get; set; }
        public virtual bool? PriorToDutytatus { get; set; }
        public virtual int? ProximateCause { get; set; }
        public virtual string ProximateCauseSpecify { get; set; }
        public virtual int? SourceInformation { get; set; }
        public virtual string SourceInformationSpecify { get; set; }
        public virtual String StatusWhenInjuryed { get; set; }
        public virtual String StatusWhenInjuryedExplanation { get; set; }
        public virtual string StatusWorsened { get; set; }
        public virtual int? TravelOccurrence { get; set; }
        public virtual String UTAPSAttached { get; set; }
        public virtual IList<WitnessData> Witnesses { get; set; }
        public virtual bool? WrittenDiagnosis { get; set; }

        /// <inheritdoc/>
        public override bool Validate(int userid)
        {
            string Section = "Unit";
            bool isValid = true;
            validations.Clear();

            if (string.IsNullOrEmpty(DutyDetermination))
            {
                AddValidationItem(new ValidationItem(Section, "DutyStatusSelect_v2", "Duty Status is required"));
                isValid = false;
            }
            else
            {
                if ((string.IsNullOrEmpty(OtherDutyStatus) && (DutyDetermination == "other")))
                {
                    AddValidationItem(new ValidationItem(Section, "OtherDutyBox_v2", "Other duty status is required"));
                    isValid = false;
                }
            }
            if (!DutyFrom.HasValue)
            {
                AddValidationItem(new ValidationItem(Section, "DutyStatusFromDate_v2, DutyStatusFromTime_v2", "Start Date not specified"));
                isValid = false;
            }
            if (!DutyTo.HasValue)
            {
                AddValidationItem(new ValidationItem(Section, "DutyStatusToDate_v2, DutyStatusToTime_v2", "End Date not specified"));
                isValid = false;
            }

            if (MemberStatus != true)
            {
                AddValidationItem(new ValidationItem(Section, "MStatusCheckBox", "Member’s Signed: Substantiating documentation required"));
                isValid = false;
            }

            if (MemberProof != true)
            {
                AddValidationItem(new ValidationItem(Section, "MProofCheckBox", "Member’s Signed: Proof of status required"));
                isValid = false;
            }
            if (MemberOccurrence == 0)
            {
                AddValidationItem(new ValidationItem(Section, "MemberOccurrenceSelect_v2", "Member Occurrence selection is required"));
                isValid = false;
            }
            else if (MemberOccurrence == 3)
            {
                if (!AbsentFrom.HasValue)
                {
                    AddValidationItem(new ValidationItem(Section, "AbsenceFromDate_v2, AbsenceFromTime_v2", "Absent without authority from date is required"));
                    isValid = false;
                }
                if (!AbsentTo.HasValue)
                {
                    AddValidationItem(new ValidationItem(Section, "AbsenceToDate_v2, AbsenceToTime_v2", "Absent without authority to date is required"));
                    isValid = false;
                }
            }
            if (string.IsNullOrEmpty(AccidentDetails))
            {
                AddValidationItem(new ValidationItem(Section, "DetailsBox_v2", "Details Of Accident is required"));
                isValid = false;
            }
            if (string.IsNullOrEmpty(MemberCredible))
            {
                AddValidationItem(new ValidationItem(Section, "rblCredibleService_v2", "Member has credible service selection is required"));
                isValid = false;
            }
            if (string.IsNullOrEmpty(MemberOnOrders))
            {
                AddValidationItem(new ValidationItem(Section, "rblMemberOnOrders_v2", "Member on orders selection is required"));
                isValid = false;
            }
            if (ProximateCause == 0)
            {
                AddValidationItem(new ValidationItem(Section, "proximateCause_v2", "Proximate cause is required"));
                isValid = false;
            }
            else if (ProximateCause == 13 && string.IsNullOrEmpty(ProximateCauseSpecify))
            {
                AddValidationItem(new ValidationItem(Section, "otherCauseTextBox_v2", "Other proximate cause is required"));
                isValid = false;
            }
            //if (String.IsNullOrEmpty(LODInitiation.ToString()))
            //{
            //    AddValidationItem(new ValidationItem(Section, "rblLODInitiation", "Did member sign the Member LOD"));
            //    isValid = false;
            //}
            //if (String.IsNullOrEmpty(WrittenDiagnosis.ToString()))
            //{
            //    AddValidationItem(new ValidationItem(Section, "rblWrittenDiagnosis", "Is there a written diagnosis from a medical provider"));
            //    isValid = false;
            //}
            //if (String.IsNullOrEmpty(MemberRequest.ToString()))
            //{
            //    AddValidationItem(new ValidationItem(Section, "rblMemberRequest", "Did the member request the LOD within 180 days"));
            //    isValid = false;
            //}

            // Bypassing Unit CC validation
            //if (String.IsNullOrEmpty(PriorToDutytatus.ToString()))
            //{
            //    AddValidationItem(new ValidationItem(Section, "rblPriorToDutytatus", "Is there medical evidence the illness or disease"));
            //    isValid = false;
            //}
            //if (String.IsNullOrEmpty(StatusWorsened))
            //{
            //    AddValidationItem(new ValidationItem(Section, "rblStatusWorsened", "Is there any medical evidence during qualified duty status"));
            //    isValid = false;
            //}

            //if (String.IsNullOrEmpty(IncurredOrAggravated.ToString()))
            //{
            //    AddValidationItem(new ValidationItem(Section, "rblIncurredOrAggravated", "Was the injury incurred or aggravated during duty status"));
            //    isValid = false;
            //}

            //if (String.IsNullOrEmpty(StatusWhenInjuryed))
            //{
            //    AddValidationItem(new ValidationItem(Section, "rblStatusWhenOccured", "What was the status of the injury or illness occurred"));
            //    isValid = false;
            //}
            //if (!String.IsNullOrEmpty(StatusWhenInjuryed))
            //{
            //    if (StatusWhenInjuryed.ToString().Equals("Other") && StatusWhenInjuryedExplanation.ToString().Equals(""))
            //    {
            //        AddValidationItem(new ValidationItem(Section, "txtStatusWhenOccuredTextBox", "What was the status of the injury or illness occurred"));
            //        isValid = false;
            //    }
            //}
            //if (String.IsNullOrEmpty(OrdersAttached))
            //{
            //    AddValidationItem(new ValidationItem(Section, "rblIfNoOrders", "Are the orders verified and attached"));                          --  Task one changes Diamante Lawwson 08/2023--
            //    isValid = false;
            //}

            //if (String.IsNullOrEmpty(IDTStatus))
            //{
            //    AddValidationItem(new ValidationItem(Section, "rblIDTDocumentAttached", "Is a certified or approved AF 40A or ANG equivalent attached"));
            //    isValid = false;
            //}

            //if (String.IsNullOrEmpty(UTAPSAttached))
            //{
            //    AddValidationItem(new ValidationItem(Section, "rblUTAPSAttached", "Is a UTAPS attached"));
            //    isValid = false;
            //}

            // Bypassing Unit CC validation
            //if (String.IsNullOrEmpty(PCARSAttached.ToString()) || PCARSAttached == false)
            //{
            //    AddValidationItem(new ValidationItem(Section, "rblPCARSAttached", "PCARS Report must be attached to the MilPDS in order for case to continue"));
            //    isValid = false;
            //}

            //if (String.IsNullOrEmpty(PCARSHistory.ToString()) || PCARSHistory == false)
            //{
            //    AddValidationItem(new ValidationItem(Section, "rblPCARSHistory", "PCARS must reflect the date of injury or illness please see note on Q6"));
            //    isValid = false;
            //}

            //if (String.IsNullOrEmpty(EightYearRule.ToString()))
            //{
            //    AddValidationItem(new ValidationItem(Section, "rblEightYearRule", "Does the member's TAFMS meet the Eight Year Rule"));
            //    isValid = false;
            //}

            bool valueValid = (_unitFinding != null && _unitFinding.Finding.HasValue && _unitFinding.Finding.Value != 0);

            if (!valueValid)
            {
                AddValidationItem(new ValidationItem(Section, "unitFindings", "Is a UTAPS attached"));
                isValid = false;
            }

            return isValid;
        }
    }
}