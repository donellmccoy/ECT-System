using System;
using System.Collections.Generic;

namespace ALOD.Core.Domain.Documents
{
    public interface IDocumentDao
    {
        long AddDocument(byte[] fileData, string fileName, long groupId, Document details);

        void CopyGroupDocuments(int oldGroupId, int newGroupId, int oldDocTypeId, int newDocTypeId);

        // Test
        long CreateGroup();

        void DeleteDocument(long docId);

        IList<Document> GetDocumentsByGroupId(long groupId);

        string GetDocumentUploadUrl(long groupId, DocumentType docType, string docEntity);

        string GetDocumentViewerUrl(long docId);

        IList<KeyValuePair<long, int>> GetRecentlyAddedDocuments(int refId, long docGroupId);

        void InsertRecentlyAddedDocument(int refId, long docGroupId, long docId, DocumentType type);

        void LockDocument(long docId);

        void RemoveRecentlyAddedDocuments(int refId, long docGroupId);

        void UnlockDocument(long docId);

        void UpdateDocumentDetails(long docId, string description, DateTime docDate, string ssn, DocumentStatus status, DocumentType type);
    }
}