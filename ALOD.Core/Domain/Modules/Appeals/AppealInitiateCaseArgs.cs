using ALOD.Core.Domain.Modules.Common;
using ALOD.Core.Interfaces.DAOInterfaces;

namespace ALOD.Core.Domain.Modules.Appeals
{
    public class AppealInitiateCaseArgs : PetitionWorkflowInitiateCaseArgs
    {
        public AppealInitiateCaseArgs(ILODAppealDAO appealDao)
        {
            AppealDao = appealDao;
        }

        protected AppealInitiateCaseArgs()
        { }

        public ILODAppealDAO AppealDao { get; set; }
        public int InitialWorkStatus { get; set; }

        /// <inheritdoc/>
        public override string GetNextCaseId()
        {
            return BuildCaseId(AppealDao.GetAppealCount(OriginalCaseRefId), "-AP-");
        }
    }
}