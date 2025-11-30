using ALOD.Core.Domain.Documents;

namespace ALOD.Core.Interfaces.DAOInterfaces
{
    public interface IDocumentViewDao : IDao<DocumentView, int>
    {
        int GetIdByDescription(string description);
    }
}