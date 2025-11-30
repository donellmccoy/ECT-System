using ALOD.Core.Interfaces;
using System;

namespace ALOD.Core.Domain.Modules.SpecialCases
{
    [Serializable]
    public class SC_PSCD_Findings : Entity, IAuditable
    {
        public SC_PSCD_Findings()
        {
            PSCDId = null;
            Name = string.Empty;
            PType = 0;
            Remarks = string.Empty;
            CreatedBy = null;
            CreatedDate = null;
            Finding = null;
            FindingText = string.Empty;
            AdditionalRemarks = string.Empty;
        }

        public virtual string AdditionalRemarks { get; set; }
        public virtual int? CreatedBy { get; set; }
        public virtual DateTime? CreatedDate { get; set; }
        public virtual short? Finding { get; set; }
        public virtual string FindingText { get; set; }
        public virtual int ModifiedBy { get; set; }

        /// <inheritdoc/>
        public virtual DateTime ModifiedDate { get; set; }

        public virtual string Name { get; set; }
        public virtual int? PSCDId { get; set; }
        public virtual short PType { get; set; }
        public virtual bool ReferToDES { get; set; }
        public virtual string Remarks { get; set; }

        public virtual bool DoFindingsMatch(SC_PSCD_Findings fnd)
        {
            if (!Finding.HasValue || !fnd.Finding.HasValue) return false;

            if (fnd.Finding.Value != Finding.Value || fnd.FindingText != FindingText) return false;

            return true;
        }

        public virtual void Modify(SC_PSCD_Findings fnd)
        {
            //if (DoFindingsMatch(fnd)) return;

            Name = fnd.Name;
            Remarks = fnd.Remarks;
            ModifiedDate = fnd.ModifiedDate;
            ModifiedBy = fnd.ModifiedBy;
            Finding = fnd.Finding;
            FindingText = fnd.FindingText;
            AdditionalRemarks = fnd.AdditionalRemarks;
            ReferToDES = fnd.ReferToDES;
        }

        public virtual void ModifyPersonal(SC_PSCD_Findings fnd)
        {
            Name = fnd.Name;
            ModifiedDate = fnd.ModifiedDate;
            ModifiedBy = fnd.ModifiedBy;
        }
    }
}