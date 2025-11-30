using ALOD.Core.Domain.Documents;
using System.Collections.Generic;

namespace ALOD.Core.Interfaces.DAOInterfaces
{
    public interface IDocCategoryViewDao : IDao<DocCategoryView, int>
    {
        IList<DocumentCategory2> GetCategoriesByDocumentViewId(int docViewId);

        int GetDocumentViewByWorkflowId(int workflowId);

        IList<DocumentCategory2> GetRedactedCategoriesByDocumentViewId(int docViewId);
    }
}