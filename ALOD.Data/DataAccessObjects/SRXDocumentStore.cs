using ALOD.Core.Domain.Documents;
using ALOD.Core.Utils;
using ALOD.Logging;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

// Add System.Net for ServicePointManager to handle TLS protocols
using System.Net;

namespace ALOD.Data
{
    public class SRXDocumentStore : IDocumentDao
    {
        private const string GroupName = "ALODDocument";
        private SRXExchange.DocumentService DocService;

        public SRXDocumentStore()
        {
            InitWebService(GroupName);
        }

        public SRXDocumentStore(string userName)
        {
            InitWebService(userName);
        }

        private string DocumentUploadStyleSheet
        {
            get { return System.Configuration.ConfigurationManager.AppSettings["DocServiceStylesheet"]; }
        }

        private string WebServicePassword
        {
            get { return System.Configuration.ConfigurationManager.AppSettings["DocServicePassword"]; }
        }

        private string WebServiceUserName
        {
            get { return System.Configuration.ConfigurationManager.AppSettings["DocServiceUsername"]; }
        }

        private void InitWebService(string userName)
        {
            DocService = new SRXExchange.DocumentService();

            // Enable TLS 1.2, 1.1, and 1.0 to ensure compatibility with HTTPS servers
            // This addresses "Could not create SSL/TLS secure channel" errors
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;

            // Temporary: Bypass certificate validation for development/testing
            // WARNING: This is insecure and should ONLY be used in non-production environments.
            // Remove or replace with proper certificate trust handling in production.
            ServicePointManager.ServerCertificateValidationCallback = (sender, cert, chain, sslPolicyErrors) => true;

            SRXExchange.ServiceLogin login = new SRXExchange.ServiceLogin
            {
                // appSettings.config
                UserName = WebServiceUserName,
                Password = WebServicePassword,
                SubuserName = userName
            };
            DocService.ServiceLoginValue = login;
        }

        #region IDocumentDao Members

        /// <inheritdoc/>
        public long AddDocument(byte[] fileData, string fileName, long groupId, Document details)
        {
            SRXExchange.UploadKeys keys = new ALOD.Data.SRXExchange.UploadKeys();
            keys.DocDate = details.DocDate;
            keys.DocDescription = details.Description;
            keys.DocStatus = DocStatusToSrxStatus(details.DocStatus);
            keys.DocTypeID = (int)details.DocType;
            keys.EntityName = details.SSN;
            keys.FileName = fileName;
            keys.InputType = ALOD.Data.SRXExchange.InputType.WebServiceUpload;

            long docId = 0;

            try
            {
                docId = DocService.UploadDocument(fileData, keys, groupId);
            }
            catch (Exception ex)
            {
                LogManager.LogError(ex);
            }

            return docId;
        }

        /// <inheritdoc/>
        public void CopyGroupDocuments(int oldGroupId, int newGroupId, int oldDocTypeId, int newDocTypeId)
        {
            try
            {
                DocService.CopyGroupDocuments(oldGroupId, newGroupId, oldDocTypeId, newDocTypeId);
            }
            catch (Exception ex)
            {
                LogManager.LogError(ex);
            }
        }

        /// <inheritdoc/>
        public long CreateGroup()
        {
            const int maxAttempts = 5;  // you can adjust this value based on your need
            int attempts = 0;

            while (attempts < maxAttempts)
            {
                try
                {
                    long groupId = DocService.CreateGroup(GroupName);

                    if (groupId != 0)
                    {
                        return groupId;  // If a valid groupId is obtained, return it
                    }
                }
                catch (Exception ex)
                {
                    LogManager.LogError(ex);
                }

                attempts++;
            }

            throw new InvalidOperationException("Failed to obtain a valid groupId after multiple attempts.");
        }

        /// <inheritdoc/>
        public void DeleteDocument(long docId)
        {
            UpdateDocumentStatus(docId, DocumentStatus.Deleted);
        }

        /// <inheritdoc/>
        public IList<Document> GetDocumentsByGroupId(long groupId)
        {
            LogManager.LogError($"Fetching documents for groupId: {groupId}");

            List<Document> docs = new List<Document>();
            SRXExchange.DocumentData[] data;

            try
            {
                data = DocService.GetGroupDocumentList(groupId);
            }
            catch (Exception e)
            {
                LogManager.LogError(e);
                return docs;  // Return an empty list if there's an error fetching the documents.
            }

            foreach (SRXExchange.DocumentData row in data)
            {
                Document doc = new Document
                {
                    Id = row.DocID,
                    SSN = row.EntityName,
                    DocStatus = (DocumentStatus)Enum.Parse(typeof(DocumentStatus), row.DocStatus.ToString()),
                    DocType = (DocumentType)row.DocTypeID,
                    DocTypeName = row.DocTypeName,
                    DocDate = row.DocDate,
                    Description = row.DocDescription,
                    Extension = row.FileExt,
                    IconUrl = row.IconUrl,
                    UploadedBy = row.UploadedBySubuserName,
                    DateAdded = row.UploadDate,
                    CanAppend = row.IsAppendable,
                    OriginalFileName = row.OriginalFileName
                };

                docs.Add(doc);
            }

            return docs;
        }

        /// <inheritdoc/>
        public string GetDocumentUploadUrl(long groupId, DocumentType docType, string docEntity)
        {
            Check.Require(groupId > 0, "groupId cannot be 0");
            Check.Require((int)docType > 0, "DocType is required");
            Check.Require(docEntity.Length > 0, "docEntity cannot be empty");

            string entityDisplayText = docEntity;
            bool isSSN = Regex.IsMatch(docEntity, @"^\d{9}$", RegexOptions.Compiled | RegexOptions.CultureInvariant);
            bool isOtherIdNumber = Regex.IsMatch(docEntity, @"^\d{10,}$", RegexOptions.Compiled | RegexOptions.CultureInvariant);

            if (isSSN)
            {
                entityDisplayText = "XXX-XX-" + docEntity.Substring(5);
            }

            if (isOtherIdNumber)
            {
                entityDisplayText = new string('X', docEntity.Length - 4) + docEntity.Substring(docEntity.Length - 4);
            }

            try
            {
                return DocService.GetDocumentUploadUrl(docEntity, (int)docType, groupId, DocumentUploadStyleSheet, entityDisplayText);
            }
            catch (Exception ex)
            {
                LogManager.LogError(ex);
                return string.Empty;
            }
        }

        /// <inheritdoc/>
        public string GetDocumentViewerUrl(long docId)
        {
            Check.Require(docId > 0, "docId cannot be 0");

            try
            {
                return DocService.GetDocumentViewerUrl(docId, false);
            }
            catch (Exception ex)
            {
                LogManager.LogError(ex);
                return string.Empty;
            }
        }

        /// <inheritdoc/>
        public IList<KeyValuePair<long, int>> GetRecentlyAddedDocuments(int refId, long docGroupId)
        {
            List<KeyValuePair<long, int>> docs = new List<KeyValuePair<long, int>>();

            SqlDataStore store = new SqlDataStore();
            System.Data.DataSet dSet = store.ExecuteDataSet("RecentlyAddedDocs_sp_GetByIds", refId, docGroupId);

            if (dSet == null)
                return docs;

            if (dSet.Tables.Count == 0)
                return docs;

            System.Data.DataTable dTable = dSet.Tables[0];

            foreach (System.Data.DataRow row in dTable.Rows)
            {
                long docId = long.Parse(row["DocId"].ToString());
                int docTypeId = int.Parse(row["DocTypeId"].ToString());

                docs.Add(new KeyValuePair<long, int>(docId, docTypeId));
            }

            return docs;
        }

        /// <inheritdoc/>
        public void InsertRecentlyAddedDocument(int refId, long docGroupId, long docId, DocumentType type)
        {
            SqlDataStore store = new SqlDataStore();
            store.ExecuteNonQuery("RecentlyAddedDocs_sp_Insert", refId, docGroupId, docId, (int)type);
        }

        /// <inheritdoc/>
        public void LockDocument(long docId)
        {
            UpdateDocumentStatus(docId, DocumentStatus.Approved);
        }

        /// <inheritdoc/>
        public void RemoveRecentlyAddedDocuments(int refId, long docGroupId)
        {
            SqlDataStore store = new SqlDataStore();
            store.ExecuteNonQuery("RecentlyAddedDocs_sp_Remove", refId, docGroupId);
        }

        /// <inheritdoc/>
        public void UnlockDocument(long docId)
        {
            UpdateDocumentStatus(docId, DocumentStatus.Pending);
        }

        /// <inheritdoc/>
        public void UpdateDocumentDetails(long docId, string description, DateTime docDate, string ssn, DocumentStatus status, DocumentType type)
        {
            SRXExchange.DocumentKeys keys = new ALOD.Data.SRXExchange.DocumentKeys();
            keys.DocDate = docDate;
            keys.DocDescription = description;
            keys.DocStatus = DocStatusToSrxStatus(status);
            keys.DocTypeID = (int)type;
            keys.EntityName = ssn;

            try
            {
                DocService.UpdateDocumentKeys(docId, keys);
            }
            catch (Exception ex)
            {
                LogManager.LogError(ex);
            }
        }

        private SRXExchange.DocumentStatus DocStatusToSrxStatus(DocumentStatus status)
        {
            return (SRXExchange.DocumentStatus)Enum.Parse(typeof(SRXExchange.DocumentStatus), status.ToString());
        }

        private void UpdateDocumentStatus(long docId, DocumentStatus status)
        {
            Check.Require(docId > 0, "docId cannot be 0");

            try
            {
                DocService.UpdateDocumentStatus(docId, (SRXExchange.DocumentStatus)Enum.Parse(typeof(SRXExchange.DocumentStatus), status.ToString()));
            }
            catch (Exception ex)
            {
                LogManager.LogError(ex);
            }
        }

        #endregion IDocumentDao Members
    }
}