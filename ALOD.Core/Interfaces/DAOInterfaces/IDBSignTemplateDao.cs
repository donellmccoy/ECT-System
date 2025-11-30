using ALOD.Core.Domain.DBSign;

namespace ALOD.Core.Interfaces.DAOInterfaces
{
    public interface IDBSignTemplateDao
    {
        DBSignTemplate GetById(int id);
    }
}