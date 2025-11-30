using ALOD.Core.Domain.Modules.Lod;

namespace ALOD.Core.Interfaces.DAOInterfaces
{
    public interface ILineOfDutyPostProcessingDao
    {
        LineOfDutyPostProcessing GetById(int id);

        bool InsertOrUpdate(LineOfDutyPostProcessing lodPostProcessing);
    }
}