using ALOD.Core.Interfaces;
using System;

namespace ALOD.Core.Domain.Modules.SARC
{
    [Serializable]
    public class SARCAppealFindings : Entity, IAuditable
    {
        public SARCAppealFindings()
        {
            AppealID = null;
            PType = 0;
            SSN = string.Empty;
            Name = string.Empty;
            Grade = string.Empty;
            Rank = string.Empty;
            Pascode = string.Empty;
            Compo = string.Empty;
            CreatedBy = null;
            CreatedDate = null;
            FindingsText = string.Empty;
            Remarks = string.Empty;
        }

        public virtual int? AppealID { get; set; }
        public virtual string Compo { get; set; }
        public virtual string Concur { get; set; }
        public virtual int? CreatedBy { get; set; }
        public virtual DateTime? CreatedDate { get; set; }
        public virtual short? Finding { get; set; }
        public virtual string FindingsText { get; set; }
        public virtual string Grade { get; set; }
        public virtual bool IsLegacyFinding { get; set; }
        public virtual int ModifiedBy { get; set; }

        /// <inheritdoc/>
        public virtual DateTime ModifiedDate { get; set; }

        public virtual string Name { get; set; }
        public virtual string Pascode { get; set; }
        public virtual short PType { get; set; }
        public virtual string Rank { get; set; }
        public virtual string Remarks { get; set; }
        public virtual string SSN { get; set; }

        public virtual bool DoFindingsMatch(SARCAppealFindings fnd)
        {
            if (!Finding.HasValue || !fnd.Finding.HasValue)
                return false;

            return (fnd.Finding.Value == Finding.Value && fnd.Remarks == Remarks && fnd.FindingsText == FindingsText && fnd.Concur == Concur);
        }

        public virtual void Modify(SARCAppealFindings fnd)
        {
            if (DoFindingsMatch(fnd))
                return;

            Finding = fnd.Finding;
            Concur = fnd.Concur;
            Remarks = fnd.Remarks;
            FindingsText = fnd.FindingsText;
            SSN = fnd.SSN;
            Compo = fnd.Compo;
            Rank = fnd.Rank;
            Grade = fnd.Grade;
            Name = fnd.Name;
            Pascode = fnd.Pascode;
            IsLegacyFinding = fnd.IsLegacyFinding;
            ModifiedDate = fnd.ModifiedDate;
            ModifiedBy = fnd.ModifiedBy;
        }

        public virtual void ModifyPersonal(SARCAppealFindings fnd)
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