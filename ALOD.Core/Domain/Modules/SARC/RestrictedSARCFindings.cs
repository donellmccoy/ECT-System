using ALOD.Core.Interfaces;
using System;

namespace ALOD.Core.Domain.Modules.SARC
{
    [Serializable]
    public class RestrictedSARCFindings : Entity, IAuditable
    {
        public RestrictedSARCFindings()
        {
            SARCID = null;
            PType = 0;
            Finding = null;
            Remarks = string.Empty;
            FindingsText = string.Empty;
            SSN = string.Empty;
            Compo = string.Empty;
            Rank = string.Empty;
            Grade = string.Empty;
            Name = string.Empty;
            Pascode = string.Empty;
            CreatedBy = null;
            CreatedDate = null;
        }

        public virtual string Compo { get; set; }
        public virtual string Concur { get; set; }
        public virtual int? CreatedBy { get; set; }
        public virtual DateTime? CreatedDate { get; set; }
        public virtual short? Finding { get; set; }
        public virtual string FindingsText { get; set; }
        public virtual string Grade { get; set; }
        public virtual int ModifiedBy { get; set; }

        /// <inheritdoc/>
        public virtual DateTime ModifiedDate { get; set; }

        public virtual string Name { get; set; }
        public virtual string Pascode { get; set; }
        public virtual short PType { get; set; }
        public virtual string Rank { get; set; }
        public virtual string Remarks { get; set; }
        public virtual int? SARCID { get; set; }
        public virtual string SSN { get; set; }

        public virtual bool DoFindingsMatch(RestrictedSARCFindings fnd)
        {
            if (!Finding.HasValue || !fnd.Finding.HasValue)
                return false;

            if (fnd.Finding.Value != Finding.Value || fnd.Remarks != Remarks || fnd.FindingsText != FindingsText)
                return false;

            return true;
        }

        public virtual void Modify(RestrictedSARCFindings fnd)
        {
            if (DoFindingsMatch(fnd))
                return;

            Finding = fnd.Finding;
            Remarks = fnd.Remarks;
            FindingsText = fnd.FindingsText;
            SSN = fnd.SSN;
            Compo = fnd.Compo;
            Rank = fnd.Rank;
            Grade = fnd.Grade;
            Name = fnd.Name;
            Pascode = fnd.Pascode;
            ModifiedDate = fnd.ModifiedDate;
            ModifiedBy = fnd.ModifiedBy;
        }

        public virtual void ModifyPersonal(RestrictedSARCFindings fnd)
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