using ALOD.Core.Domain.Lookup;
using System.Collections.Generic;

namespace ALOD.Core.Interfaces.DAOInterfaces
{
    public interface ILookupCancelReasonsDao
    {
        IList<CancelReasons> GetAll();

        CancelReasons GetById(int id);

        void UpdateCancelReasons(int id, string description, int DisplayOrder);
    }
}