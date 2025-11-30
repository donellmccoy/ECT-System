using ALOD.Core.Domain.Users;
using ALOD.Core.Interfaces;
using System;

namespace ALOD.Core.Domain.Modules.Lod
{
    [Serializable]
    public class LineOfDutyFindings : Entity, IAuditable
    {
        public LineOfDutyFindings()
        {
            Finding = null;
            Explanation = "";
            DecisionYN = "";
            FindingsText = "";
            SSN = " ";
            Compo = " ";
            Rank = " ";
            Grade = " ";
            Name = " ";
            Pascode = " ";
            CreatedBy = null;
            CreatedDate = null;
            ReferDES = null;
        }

        public virtual string Compo { get; set; }
        public virtual string CorrectlyIdentified { get; set; }
        public virtual int? CreatedBy { get; set; }
        public virtual DateTime? CreatedDate { get; set; }
        public virtual string DecisionYN { get; set; }

        public virtual string Description
        {
            get
            {
                if (!Finding.HasValue)
                    return "";

                switch ((LodFinding)Finding.Value)
                {
                    case LodFinding.InLineOfDuty:
                        return "In Line of Duty";

                    case LodFinding.EptsLodNotApplicable:
                        return "EPTS  - LOD Not Applicable";

                    case LodFinding.EptsServiceAgravated:
                        return "EPTS - Service aggravated";

                    case LodFinding.NotInLineOfDutyDueToOwnMisconduct:
                        return "Not In Line Of Duty - Due To Own Misconduct";

                    case LodFinding.NotInLineOfDutyNotDueToOwnMisconduct:
                        return "Not ILOD - Not Due To Own Misconduct";

                    case LodFinding.RecommendFormalInvestigation:
                        return "Recommend Formal LOD Investigation";

                    default:
                        return "";
                }
            }
        }

        public virtual bool? EightYearRule { get; set; }
        public virtual string Explanation { get; set; }
        public virtual short? Finding { get; set; }

        //Findings text
        public virtual string FindingsText { get; set; }

        public virtual string Grade { get; set; }
        public virtual string IDTStatus { get; set; }
        public virtual bool? IPCARSAttached { get; set; }
        public virtual int? LODID { get; set; }
        public virtual bool? LODInitiation { get; set; }
        public virtual bool? MemberRequest { get; set; }
        public virtual int ModifiedBy { get; set; }

        /// <inheritdoc/>
        public virtual DateTime ModifiedDate { get; set; }

        public virtual string Name { get; set; }
        public virtual string Pascode { get; set; }
        public virtual bool? PriorToDutytatus { get; set; }
        public virtual short PType { get; set; }
        public virtual string Rank { get; set; }
        public virtual bool? ReferDES { get; set; }
        public virtual string SSN { get; set; }
        public virtual string StatusWorsened { get; set; }
        public virtual Unit Unit { get; set; }
        public virtual string VerifiedAndAttached { get; set; }
        public virtual bool? WrittenDiagnosis { get; set; }

        public virtual void Modify(LineOfDutyFindings fnd)
        {
            if (fnd.Finding != Finding || fnd.Explanation != Explanation || fnd.DecisionYN != DecisionYN || fnd.FindingsText != FindingsText || fnd.ReferDES != ReferDES)
            {
                Finding = fnd.Finding;
                Explanation = fnd.Explanation;
                FindingsText = fnd.FindingsText;
                DecisionYN = fnd.DecisionYN;
                SSN = fnd.SSN;
                Compo = fnd.Compo;
                Rank = fnd.Rank;
                Grade = fnd.Grade;
                Name = fnd.Name;
                Pascode = fnd.Pascode;
                ModifiedDate = fnd.ModifiedDate;
                ModifiedBy = fnd.ModifiedBy;
                ReferDES = fnd.ReferDES;
            }

            LODInitiation = fnd.LODInitiation;
            WrittenDiagnosis = fnd.WrittenDiagnosis;
            MemberRequest = fnd.MemberRequest;
            CorrectlyIdentified = fnd.CorrectlyIdentified;
            VerifiedAndAttached = fnd.VerifiedAndAttached;
            IDTStatus = fnd.IDTStatus;
            IPCARSAttached = fnd.IPCARSAttached;
            EightYearRule = fnd.EightYearRule;
            PriorToDutytatus = fnd.PriorToDutytatus;
            StatusWorsened = fnd.StatusWorsened;
        }

        public virtual void ModifyPersonal(LineOfDutyFindings fnd)
        {
            SSN = fnd.SSN;
            Compo = fnd.Compo;
            Rank = fnd.Rank;
            Grade = fnd.Grade;
            Name = fnd.Name;
            Pascode = fnd.Pascode;
            ModifiedDate = fnd.ModifiedDate;
            ModifiedBy = fnd.ModifiedBy;
        }
    }
}