using ALOD.Core.Domain.Documents;
using ALOD.Core.Domain.Modules.Lod;
using ALOD.Core.Domain.Users;
using ALOD.Core.Interfaces;
using ALOD.Core.Interfaces.DAOInterfaces;

namespace ALOD.Core.Domain.Modules.Reinvestigations
{
    public struct RRDeterminationMemoArgs
    {
        public IDaoFactory DaoFactory;
        public PersonnelTypes PType;
        public IDigitalSignatureService SigService;
        public MemoTemplate Template;
        public AppUser UserGeneratingMemo;
    }
}