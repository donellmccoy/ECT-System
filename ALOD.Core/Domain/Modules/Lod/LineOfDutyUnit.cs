using ALOD.Core.Domain.Common;
using ALOD.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ALOD.Core.Domain.Modules.Lod
{
    [Serializable]
    public class LineOfDutyUnit : Entity, IValidatable, IAuditable
    {
        protected LineOfDutyFindings _unitFinding = null;
        protected IList<ValidationItem> validations = new List<ValidationItem>();

        public LineOfDutyUnit()
        { }

        public LineOfDutyUnit(int id)
        {
            this.Id = id;
        }

        public virtual string AccidentDetails { get; set; }
        public virtual string Activated { get; set; }
        public virtual string DutyDetermination { get; set; }
        public virtual DateTime? DutyFrom { get; set; }

        public virtual string DutyStatusDescription
        {
            get
            {
                switch (DutyDetermination)
                {
                    case "active":
                        return "Active Duty Status";

                    case "uta":
                        return "UTA";

                    case "aftp":
                        return "AFTP";

                    case "snr":
                        return "Saturday night rule";

                    case "travel":
                        return "Travel to/from duty";

                    case "use":
                        return "Unit sponsored event";

                    case "other":
                        return "Other (provide details below)";

                    case "rmp":
                        return "RMP";

                    default:
                        return "";
                }
            }
        }

        public virtual DateTime? DutyTo { get; set; }
        public virtual bool MemberProof { get; set; }
        public virtual bool MemberStatus { get; set; }
        public virtual int ModifiedBy { get; set; }

        /// <inheritdoc/>
        public virtual DateTime ModifiedDate { get; set; }

        public virtual string OtherDutyStatus { get; set; }

        public virtual LineOfDutyFindings UnitFinding
        {
            get
            {
                return _unitFinding;
            }
            set
            {
                _unitFinding = value;
            }
        }

        public virtual int Workflow { get; set; }

        #region IValidatable Members

        /// <inheritdoc/>
        public virtual IList<ValidationItem> ValidationItems
        {
            get { return validations; }
        }

        public virtual void AddValidationItem(ValidationItem item)
        {
            IList<ValidationItem> lst = (from p in validations where p.Section == item.Section && p.Field == item.Field select p).ToList<ValidationItem>();
            if (lst.Count == 0)
            {
                validations.Add(item);
            }
        }

        /// <inheritdoc/>
        public virtual bool Validate(int userid)
        {
            string Section = "Unit";
            bool isValid = true;
            validations.Clear();

            if (string.IsNullOrEmpty(DutyDetermination))
            {
                AddValidationItem(new ValidationItem(Section, "ddlDutyStatus", "Duty Status is required"));
                isValid = false;
            }
            else
            {
                if ((string.IsNullOrEmpty(OtherDutyStatus) && (DutyDetermination == "other")))
                {
                    AddValidationItem(new ValidationItem(Section, "txtOtherDuty", "Other duty status is required"));
                    isValid = false;
                }
            }

            if (string.IsNullOrEmpty(AccidentDetails))
            {
                AddValidationItem(new ValidationItem(Section, "txtCircDetails", "Details Of Accident is required"));
                isValid = false;
            }

            bool valueValid = (_unitFinding != null && _unitFinding.Finding.HasValue && _unitFinding.Finding.Value != 0);

            if (!valueValid)
            {
                AddValidationItem(new ValidationItem(Section, "unitFindings", "Result of Investigation is required."));
                isValid = false;
            }
            if (DutyDetermination == "active" && !DutyFrom.HasValue)
            {
                AddValidationItem(new ValidationItem(Section, "AbsenceFromBox", "Start Date not specified."));
                isValid = false;
            }
            if (DutyDetermination == "active" && !DutyTo.HasValue)
            {
                AddValidationItem(new ValidationItem(Section, "AbsenceToBox", "End Date not specified."));
                isValid = false;
            }

            if (DutyDetermination == "active" && DutyFrom.HasValue && DutyTo.HasValue)
            {
                if (DutyFrom.Value >= DutyTo.Value)
                {
                    AddValidationItem(new ValidationItem(Section, "AbsenceFromBox", "Start Date must be before End Date."));
                    isValid = false;
                }
            }

            if (DutyDetermination == "active" && DutyFrom.HasValue)
            {
                if (DutyFrom.Value >= DateTime.Now)
                {
                    AddValidationItem(new ValidationItem(Section, "AbsenceFromBox", "Start Date  can not be future date."));
                    isValid = false;
                }
            }

            return isValid;
        }

        #endregion IValidatable Members
    }
}