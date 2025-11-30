using ALOD.Core.Domain.Modules.SpecialCases;
using System.Data;

namespace ALOD.Core.Interfaces.DAOInterfaces
{
    public interface IPALDataDao : IDao<PAL_Data, int>
    {
        DataSet GetPALData(string partialSSN, string partialLastName);

        DataSet GetPALDocuments(string Last4SSN, string LastName);
    }
}