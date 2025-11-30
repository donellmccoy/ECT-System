using ALOD.Core.Domain.Documents;
using ALOD.Core.Domain.ServiceMembers;

namespace ALOD.Core.Domain.Modules.Common
{
    public abstract class PetitionWorkflowInitiateCaseArgs
    {
        public int CreatedByUserId { get; set; }
        public IDocumentDao DocumentDao { get; set; }
        public ServiceMember Member { get; set; }
        public string OriginalCaseCaseId { get; set; }
        public int OriginalCaseRefId { get; set; }

        public abstract string GetNextCaseId();

        protected virtual string BuildCaseId(int caseCount, string workflowAbbreviation)
        {
            return BuildCaseId(caseCount, OriginalCaseCaseId, workflowAbbreviation);
        }

        protected virtual string BuildCaseId(int caseCount, string baseCaseId, string workflowAbbreviation)
        {
            caseCount += 1;
            int leadingZeroes = 3 - caseCount.ToString().Length;
            string caseId = baseCaseId + workflowAbbreviation;

            for (int i = 0; i < leadingZeroes; i++)
            {
                caseId += "0";
            }

            caseId += caseCount.ToString();

            return caseId;
        }
    }
}