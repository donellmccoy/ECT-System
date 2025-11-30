using ALOD.Core.Domain.Documents;
using System.Collections.Generic;

namespace ALOD.Core.Interfaces
{
    public interface IDocumentProcessingStrategy
    {
        IList<string> GetProcessingErrors();

        long ProcessDocument(int refId, long groupId, IDocumentDao docDao, Document metaData, byte[] fileData);
    }
}