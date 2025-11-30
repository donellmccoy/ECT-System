using ALOD.Core.Domain.Common;
using ALOD.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ALOD.Core.Domain.Modules.Lod
{
    [Serializable]
    public class LineOfDutyInvestigation : Entity, IValidatable
    {
        private LineOfDutyFindings _ioFinding = null;
        private IList<ValidationItem> validations = new List<ValidationItem>();

        public LineOfDutyInvestigation()
        {
            OtherPersonnel = new List<PersonnelData>();
        }

        public LineOfDutyInvestigation(int refId)
        {
            OtherPersonnel = new List<PersonnelData>();
            this.Id = refId;
        }

        public virtual bool? AbsentWithAuthority { get; set; }
        public virtual DateTime? DateSignedAppointing { get; set; }
        public virtual DateTime? DateSignedIO { get; set; }
        public virtual DateTime? DurationEnd { get; set; }
        public virtual DateTime? DurationStart { get; set; }
        public virtual short? FinalApprovalFindings { get; set; }
        public virtual DateTime? FindingsDate { get; set; }
        public virtual string HowSustained { get; set; }
        public virtual string InactiveDutyTraining { get; set; }
        public virtual bool? IntentionalMisconduct { get; set; }

        public virtual string InvestigationDescription
        {
            get
            {
                if (InvestigationOf == null)
                    return "";
                if (!InvestigationOf.HasValue)
                    return "";

                switch (InvestigationOf.Value)
                {
                    case 1:
                        return "Disease";

                    case 2:
                        return "Illness";

                    case 3:
                        return "Death";

                    case 4:
                        return "Injury (non MVA)";

                    case 5:
                        return "Injury (MVA)";
                }

                return "";
            }
        }

        public virtual short? InvestigationOf { get; set; }

        public virtual LineOfDutyFindings IOFinding
        {
            get
            {
                return _ioFinding;
            }
            set
            {
                _ioFinding = value;
            }
        }

        public virtual int? IoUserId { get; set; }

        public virtual bool IsSignedByAppointingAuthority
        {
            get
            {
                if (DateSignedAppointing == null)
                { return false; }
                else
                { return DateSignedAppointing.HasValue; }
            }
        }

        //Signatures
        public virtual bool IsSignedByIO
        {
            get
            {
                if (DateSignedIO == null)
                { return false; }
                else
                { return DateSignedIO.HasValue; }
            }
        }

        public virtual string MedicalDiagnosis { get; set; }
        public virtual bool? MentallySound { get; set; }
        public virtual int ModifiedBy { get; set; }
        public virtual DateTime? ModifiedDate { get; set; }
        public virtual IList<PersonnelData> OtherPersonnel { get; set; }
        public virtual string Place { get; set; }
        public virtual bool? PresentForDuty { get; set; }
        public virtual string Remarks { get; set; }
        public virtual DateTime? ReportDate { get; set; }
        public virtual PersonnelData SignatureInfoAppointing { get; set; }
        public virtual PersonnelData SignatureInfoIO { get; set; }
        public virtual int? Status { get; set; }

        #region IValidatable Members

        /// <inheritdoc/>
        public virtual IList<ValidationItem> ValidationItems
        {
            get { return validations; }
        }

        /// <inheritdoc/>
        public virtual bool Validate(int userid)
        {
            string section = "Investigation";
            bool valueValid = true;
            bool isValid = true;
            validations.Clear();

            //Check if report date entered(required). if yes check if report date is a valid date
            valueValid = ReportDate.HasValue;
            if (!valueValid)
            {
                AddValidationItem(new ValidationItem(section, "txtDateReport", "Report Date is required"));
                isValid = false;
            }
            if ((valueValid) && ReportDate > DateTime.Now)
            {
                AddValidationItem(new ValidationItem(section, "txtDateReport", "Report Date can not be beyond today's date."));
                isValid = false;
            }

            valueValid = (InvestigationOf.HasValue && InvestigationOf.Value != 0);
            if (!valueValid)
            {
                AddValidationItem(new ValidationItem(section, "rblInvestigationOf", "Type of Investigation not selected"));
                isValid = false;
            }

            valueValid = (Status.HasValue && Status.Value != 0);
            if (!valueValid)
            {
                AddValidationItem(new ValidationItem(section, "rbInactive,rbAdMore,rbAdLess,rbRegularOrEad,rbShortTour", "Duty Status not selected"));

                isValid = false;
            }
            if (valueValid)
            {
                if ((Status == 4) || (Status == 5))
                {
                    if (!DurationStart.HasValue)
                    {
                        AddValidationItem(new ValidationItem(section, "txtDateStart,txtHrStart", "Start Date Reqd"));
                        isValid = false;
                    }
                    if (DurationStart.HasValue && DurationStart > DateTime.Now)
                    {
                        AddValidationItem(new ValidationItem(section, "txtDateStart,txtHrStart", "Duration Start Date can not be beyond today's date."));
                        isValid = false;
                    }
                    if (!DurationEnd.HasValue)
                    {
                        AddValidationItem(new ValidationItem(section, "txtDateFinish,txtHrFinish", "Finish Date Reqd"));
                        isValid = false;
                    }

                    if ((DurationStart.HasValue && DurationStart <= DateTime.Now) && (DurationEnd.HasValue && DurationEnd <= DateTime.Now) && DurationStart > DurationEnd)
                    {
                        AddValidationItem(new ValidationItem(section, "txtDateStart,txtHrStart,txtDateFinish,txtHrFinish", "Duration Start date is greater then Duration Start Date"));
                        isValid = false;
                    }
                }
                if ((Status == 4) && String.IsNullOrEmpty(InactiveDutyTraining))
                {
                    AddValidationItem(new ValidationItem(section, "txtInactiveDutyTraining", "Inactive Duty Training not entered."));
                    isValid = false;
                }
            }

            valueValid = FindingsDate.HasValue;
            if (!valueValid)
            {
                AddValidationItem(new ValidationItem(section, "txtDateCircumstance,txtHrCircumstance", "Finding Date is required."));
                isValid = false;
            }
            if ((valueValid) && FindingsDate > DateTime.Now)
            {
                AddValidationItem(new ValidationItem(section, "txtDateCircumstance,txtHrCircumstance", "Findings Date can not be beyond today's date."));
                isValid = false;
            }
            if (String.IsNullOrEmpty(Place))
            {
                AddValidationItem(new ValidationItem(section, "txtCircumstancePlace", "Circumstances Place not entered."));

                isValid = false;
            }
            if (String.IsNullOrEmpty(HowSustained))
            {
                AddValidationItem(new ValidationItem(section, "txtCircumstanceSustained", "How sustained not entered."));
                isValid = false;
            }

            if (!PresentForDuty.HasValue)
            {
                AddValidationItem(new ValidationItem(section, "rblPresentForDuty", "Present for duty not selected."));
                isValid = false;
            }
            if ((PresentForDuty == false) && !(AbsentWithAuthority.HasValue))
            {
                AddValidationItem(new ValidationItem(section, "rblAbsentWithAuthority", "Type of Absence  not selected."));
                isValid = false;
            }

            valueValid = (InvestigationOf.HasValue && InvestigationOf.Value != 0);
            if (valueValid && InvestigationOf != 3)
            {
                if (!IntentionalMisconduct.HasValue)
                {
                    AddValidationItem(new ValidationItem(section, "rblIntentionalMisconduct", "Intentional misconduct not selected."));
                    isValid = false;
                }

                if (!MentallySound.HasValue)
                {
                    AddValidationItem(new ValidationItem(section, "rblMentallySound", "Mentally Sound not selected."));
                    isValid = false;
                }
            }

            if (String.IsNullOrEmpty(Remarks))
            {
                AddValidationItem(new ValidationItem(section, "txtRemarks", "Remarks not entered."));
                isValid = false;
            }
            valueValid = (_ioFinding != null && _ioFinding.Finding.HasValue && _ioFinding.Finding.Value != 0);

            if (!valueValid)
            {
                AddValidationItem(new ValidationItem(section, "rblFindings", "LOD Findings not selected."));
                isValid = false;
            }
            return isValid;
        }

        private void AddValidationItem(ValidationItem item)
        {
            IList<ValidationItem> lst = (from p in validations where p.Section == item.Section && p.Field == item.Field select p).ToList<ValidationItem>();
            if (lst.Count == 0)
            {
                validations.Add(item);
            }
        }

        #endregion IValidatable Members
    }
}