using ALOD.Core.Domain.Modules.Appeals;

namespace ALOD.Core.Interfaces.DAOInterfaces
{
    public interface IAppealPostProcessingDAO
    {
        AppealPostProcessing GetById(int id);

        bool hasPostProcess(int appealId);

        bool InsertOrUpdate(AppealPostProcessing appealPostProcessing);
    }
}