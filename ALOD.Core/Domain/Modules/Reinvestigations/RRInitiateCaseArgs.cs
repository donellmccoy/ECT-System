using ALOD.Core.Domain.Modules.Common;
using ALOD.Core.Interfaces.DAOInterfaces;

namespace ALOD.Core.Domain.Modules.Reinvestigations
{
    public class RRInitiateCaseArgs : PetitionWorkflowInitiateCaseArgs
    {
        public RRInitiateCaseArgs(ILODReinvestigateDAO rrDao)
        {
            RRDao = rrDao;
        }

        protected RRInitiateCaseArgs()
        { }

        public ILODReinvestigateDAO RRDao { get; set; }

        /// <inheritdoc/>
        public override string GetNextCaseId()
        {
            return BuildCaseId(RRDao.GetReinvestigationRequestsCount(OriginalCaseRefId), "-RR-");
        }
    }
}