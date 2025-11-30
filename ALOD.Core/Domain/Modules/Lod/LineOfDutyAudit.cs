using ALOD.Core.Domain.Common;
using ALOD.Core.Domain.Lookup;
using ALOD.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ALOD.Core.Domain.Modules.Lod
{
    [Serializable]
    public class LineOfDutyAudit : Entity, IValidatable
    {
        protected IList<ValidationItem> validations = new List<ValidationItem>();

        public LineOfDutyAudit()
        { }

        public LineOfDutyAudit(int id)
        {
            this.Id = id;
        }

        public virtual string A1_Comment { get; set; }
        public virtual Boolean A1_EPTS { get; set; }
        public virtual string A1_Other { get; set; }
        public virtual Boolean Activites { get; set; }
        public virtual string ApprovalComments { get; set; }
        public virtual int AuditId { get; set; }

        public virtual string CancelDescription
        {
            get
            {
                if (PhysicianCancelReason == 0)
                    return "";

                switch (PhysicianCancelReason)
                {
                    case 1:
                        return "Duplicate LOD";

                    case 2:
                        return "LOD Started in error by MedTech";

                    case 3:
                        return "Annotation made in medical record";

                    case 4:
                        return "Other please explain below";

                    default:
                        return "";
                }
                ;
            }
        }

        public virtual string DeathInvolved { get; set; }
        public virtual string DiagnosisText { get; set; }
        public virtual Boolean EightYearRule { get; set; }
        public virtual short? Epts { get; set; }
        public virtual string EventDetails { get; set; }
        public virtual string ICD7thCharacter { get; set; }
        public virtual int? ICD9Id { get; set; }
        public virtual Boolean IDT { get; set; }
        public virtual Boolean IllnessOrDisease { get; set; }
        public virtual Boolean IncurredOrAggravated { get; set; }
        public virtual Boolean JA_Aggravation { get; set; }
        public virtual string JA_Comment { get; set; }
        public virtual Boolean JA_CorrectStandard { get; set; }
        public virtual Boolean JA_DX { get; set; }
        public virtual Boolean JA_EPTS { get; set; }
        public virtual Boolean JA_Evidence { get; set; }
        public virtual Boolean JA_FormalInvestigation { get; set; }
        public virtual Boolean JA_ISupport { get; set; }
        public virtual Boolean JA_Misconduct { get; set; }
        public virtual string JA_Other { get; set; }
        public virtual Boolean JA_Principles { get; set; }
        public virtual Boolean JA_ProofApplied { get; set; }
        public virtual Boolean JA_ProofMet { get; set; }
        public virtual Boolean LegallySufficient { get; set; }
        public virtual Boolean LODInitiation { get; set; }
        public virtual string MedicalFacility { get; set; }
        public virtual string MedicalFacilityType { get; set; }
        public virtual Boolean MedicallyAppropriate { get; set; }
        public virtual Boolean MemberRequest { get; set; }

        //Not needed keep for now for logic below
        public virtual string MemberStatus { get; set; }

        public virtual int ModifiedBy { get; set; }
        public virtual DateTime? ModifiedDate { get; set; }
        public virtual string MvaInvolved { get; set; }
        public virtual string NatureOfEvent { get; set; }

        public virtual string NatureOfIncidentDescription
        {
            get
            {
                if (string.IsNullOrEmpty(NatureOfEvent))
                    return "";

                switch (NatureOfEvent)
                {
                    case "Disease":
                        return "Disease";

                    case "Illness":
                        return "Illness";

                    case "Death":
                        return "Death";

                    case "Injury":
                        return "Injury (non MVA)";

                    case "Injury_MVA":
                        return "Injury (MVA)";

                    default:
                        return "";
                }
                ;
            }
        }

        public virtual Boolean Orders { get; set; }
        public virtual string PhysicianCancelExplanation { get; set; }
        public virtual short PhysicianCancelReason { get; set; }
        public virtual Boolean SG_Aggravation { get; set; }
        public virtual string SG_Comment { get; set; }
        public virtual Boolean SG_CorrectStandard { get; set; }
        public virtual Boolean SG_DX { get; set; }
        public virtual Boolean SG_EPTS { get; set; }
        public virtual Boolean SG_Evidence { get; set; }
        public virtual Boolean SG_FormalInvestigation { get; set; }
        public virtual Boolean SG_ISupport { get; set; }
        public virtual Boolean SG_Misconduct { get; set; }
        public virtual string SG_Other { get; set; }
        public virtual Boolean SG_Principles { get; set; }
        public virtual Boolean SG_ProofApplied { get; set; }
        public virtual Boolean SG_ProofMet { get; set; }
        public virtual Boolean StatusOfMember { get; set; }
        public virtual Boolean StatusValidated { get; set; }
        public virtual DateTime? TreatmentDate { get; set; }
        public virtual int Workflow { get; set; }
        public virtual Boolean WrittenDiagnosis { get; set; }

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

        public virtual bool IsSelectedICDCodeADisease(ILookupDao lookupDao)
        {
            if (lookupDao == null)
                throw new ArgumentNullException(nameof(lookupDao));

            if (!ICD9Id.HasValue)
                return false;

            ICD9Code code = lookupDao.GetIcd9ById(ICD9Id.Value);

            if (code == null)
                return false;

            return code.IsDisease;
        }

        public virtual void SaveAudit(int x)
        {
        }

        /// <inheritdoc/>
        public virtual bool Validate(int userid)
        {
            string section = "Medical";
            bool isValid = true;
            validations.Clear();

            if ((String.IsNullOrEmpty(DiagnosisText)))
            {
                AddValidationItem(new ValidationItem(section, "DiagnosisTextBox", "Diagnosis Text is required"));
                isValid = false;
            }

            if (!(ICD9Id.HasValue))
            {
                AddValidationItem(new ValidationItem(section, "ddlICDChapter,ddlICDSection,ddlICDDiagnosisLevel1,ddlICDDiagnosisLevel2,ddlICDDiagnosisLevel3,ddlICDDiagnosisLevel4", "ICD code  is required"));
                isValid = false;
            }

            if (String.IsNullOrEmpty(NatureOfEvent))
            {
                AddValidationItem(new ValidationItem(section, "ddlIncidentType", "Nature of Incident is required"));
                isValid = false;
            }

            if (String.IsNullOrEmpty(MemberStatus))
            {
                AddValidationItem(new ValidationItem(section, "ddlMemberType", "Member status is required"));
                isValid = false;
            }

            if (String.IsNullOrEmpty(MedicalFacilityType))
            {
                AddValidationItem(new ValidationItem(section, "ddlHospitalType", "Type of Medical Facility is required"));
                isValid = false;
            }

            if (String.IsNullOrEmpty(MedicalFacility))
            {
                AddValidationItem(new ValidationItem(section, "txtHospitalName", "Name of Medical Facility is required"));
                isValid = false;
            }

            if ((TreatmentDate == null) || (!TreatmentDate.HasValue))
            {
                AddValidationItem(new ValidationItem(section, "txtDateTreatment, txtHrTreatment", "Treatment date is required"));
                isValid = false;
            }

            if ((TreatmentDate != null && TreatmentDate.HasValue))
            {
                if (TreatmentDate.Value > DateTime.Now)
                {
                    AddValidationItem(new ValidationItem(section, "txtDateTreatment, txtHrTreatment", "Treatment date can not be future date"));
                    isValid = false;
                }
            }

            if (String.IsNullOrEmpty(EventDetails))
            {
                AddValidationItem(new ValidationItem(section, "txtEventDetails", "Details of Accident is required"));
                isValid = false;
            }

            return isValid;
        }

        #endregion IValidatable Members
    }
}