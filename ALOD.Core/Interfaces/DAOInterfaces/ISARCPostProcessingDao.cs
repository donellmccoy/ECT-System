using ALOD.Core.Domain.Modules.SARC;

namespace ALOD.Core.Interfaces.DAOInterfaces
{
    public interface ISARCPostProcessingDao
    {
        RestrictedSARCPostProcessing GetById(int id);

        bool InsertOrUpdate(RestrictedSARCPostProcessing sarcPostProcessing);
    }
}