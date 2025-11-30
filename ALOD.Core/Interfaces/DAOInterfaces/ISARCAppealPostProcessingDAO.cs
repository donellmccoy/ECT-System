using ALOD.Core.Domain.Modules.SARC;

namespace ALOD.Core.Interfaces.DAOInterfaces
{
    public interface ISARCAppealPostProcessingDAO
    {
        SARCAppealPostProcessing GetById(int id);

        bool hasPostProcess(int appealId);

        bool InsertOrUpdate(SARCAppealPostProcessing SARCappealPostProcessing);
    }
}