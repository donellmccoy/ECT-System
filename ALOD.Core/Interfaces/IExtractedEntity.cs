using ALOD.Core.Interfaces.DAOInterfaces;
using System.Data;

namespace ALOD.Core.Interfaces
{
    public interface IExtractedEntity
    {
        bool ExtractFromDataRow(DataRow row);

        bool ExtractFromDataRow(DataRow row, IDaoFactory daoFactory);
    }
}