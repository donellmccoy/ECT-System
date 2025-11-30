using ALOD.Core.Domain.Modules.Common;
using ALOD.Core.Interfaces.DAOInterfaces;

namespace ALOD.Core.Domain.Modules.SARC
{
    public class SARCAppealInitiateCaseArgs : PetitionWorkflowInitiateCaseArgs
    {
        public SARCAppealInitiateCaseArgs(ISARCAppealDAO sarcAppealDao)
        {
            SARCAppealDao = sarcAppealDao;
        }

        protected SARCAppealInitiateCaseArgs()
        { }

        public int OriginalCaseWorkflowId { get; set; }

        public ISARCAppealDAO SARCAppealDao { get; set; }

        /// <inheritdoc/>
        public override string GetNextCaseId()
        {
            return BuildCaseId(SARCAppealDao.GetAppealCount(OriginalCaseRefId, OriginalCaseWorkflowId), OriginalCaseCaseId.Replace("-SA", string.Empty), "-APSA-");
        }
    }
}