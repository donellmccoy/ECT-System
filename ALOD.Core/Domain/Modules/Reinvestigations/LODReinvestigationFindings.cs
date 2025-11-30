using ALOD.Core.Domain.Modules.Common;

namespace ALOD.Core.Domain.Modules.Reinvestigations
{
    public class LODReinvestigationFindings : AbstractWorkflowCaseFindings
    {
        public virtual string FirstName { get; set; }
        public virtual bool IsLegacyFinding { get; set; }
        public virtual string LastName { get; set; }
        public virtual string MiddleName { get; set; }

        /// <inheritdoc/>
        public override void Modify(AbstractWorkflowCaseFindings finding)
        {
            base.Modify(finding);

            if (DoFindingsMatch(finding))
                return;

            LODReinvestigationFindings appealFinding = (LODReinvestigationFindings)finding;

            LastName = appealFinding.LastName;
            FirstName = appealFinding.FirstName;
            MiddleName = appealFinding.MiddleName;
            IsLegacyFinding = appealFinding.IsLegacyFinding;
        }

        /// <inheritdoc/>
        public override void ModifyPersonnel(AbstractWorkflowCaseFindings finding)
        {
            base.ModifyPersonnel(finding);

            LODReinvestigationFindings appealFinding = (LODReinvestigationFindings)finding;

            LastName = appealFinding.LastName;
            FirstName = appealFinding.FirstName;
            MiddleName = appealFinding.MiddleName;
        }
    }
}