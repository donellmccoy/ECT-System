using ALOD.Core.Domain.Workflow;
using System.Collections.Generic;
using System.Data;

namespace ALOD.Core.Interfaces.DAOInterfaces
{
    public interface ICaseLockDao : IDao<CaseLock, int>
    {
        void ClearAllLocks();

        void ClearLocksForUser(int userId);

        new DataSet GetAll();

        CaseLock GetByReferenceId(int refId, byte module);

        IList<CaseLock> GetByUserId(int userId);
    }
}