using ALOD.Core.Interfaces;
using System;

namespace ALOD.Core.Domain.Modules.Common
{
    public abstract class AbstractWorkflowCaseFindings : Entity, IAuditable
    {
        public AbstractWorkflowCaseFindings()
        {
            RefId = 0;
            PType = 0;
            CreatedBy = 0;
            CreatedDate = DateTime.Now;
            ModifiedDate = DateTime.Now;
        }

        public virtual string Compo { get; set; }
        public virtual string Concur { get; set; }
        public virtual int CreatedBy { get; set; }
        public virtual DateTime CreatedDate { get; set; }
        public virtual string Explanation { get; set; }
        public virtual short? Finding { get; set; }
        public virtual string Grade { get; set; }
        public virtual int ModifiedBy { get; set; }

        /// <inheritdoc/>
        public virtual DateTime ModifiedDate { get; set; }

        public virtual string PasCode { get; set; }
        public virtual short PType { get; set; }
        public virtual string Rank { get; set; }
        public virtual int RefId { get; set; }

        public virtual bool DoFindingsMatch(AbstractWorkflowCaseFindings finding)
        {
            if (!Finding.HasValue || !finding.Finding.HasValue)
                return false;

            if (finding.Finding.Value != Finding.Value || finding.Explanation != Explanation)
                return false;

            if (String.IsNullOrEmpty(finding.Concur) != String.IsNullOrEmpty(Concur))
                return false;
            else if (!String.IsNullOrEmpty(finding.Concur))
            {
                if (!finding.Concur.Equals(Concur))
                    return false;
            }

            return true;
        }

        public virtual void Modify(AbstractWorkflowCaseFindings finding)
        {
            if (DoFindingsMatch(finding))
                return;

            Concur = finding.Concur;
            Finding = finding.Finding;
            Explanation = finding.Explanation;
            Compo = finding.Compo;
            Rank = finding.Rank;
            Grade = finding.Grade;
            PasCode = finding.PasCode;
            ModifiedDate = finding.ModifiedDate;
            ModifiedBy = finding.ModifiedBy;
        }

        public virtual void ModifyPersonnel(AbstractWorkflowCaseFindings finding)
        {
            Compo = finding.Compo;
            Rank = finding.Rank;
            Grade = finding.Grade;
            PasCode = finding.PasCode;
            ModifiedDate = finding.ModifiedDate;
            ModifiedBy = finding.ModifiedBy;
        }
    }
}