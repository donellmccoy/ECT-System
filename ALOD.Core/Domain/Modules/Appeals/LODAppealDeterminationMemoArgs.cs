using ALOD.Core.Domain.Documents;
using ALOD.Core.Domain.Users;
using ALOD.Core.Interfaces;
using ALOD.Core.Interfaces.DAOInterfaces;

namespace ALOD.Core.Domain.Modules.Appeals
{
    public struct LODAppealDeterminationMemoArgs
    {
        public IDaoFactory DaoFactory;
        public IDigitalSignatureService SigService;
        public MemoTemplate Template;
        public AppUser UserGeneratingMemo;
    }
}