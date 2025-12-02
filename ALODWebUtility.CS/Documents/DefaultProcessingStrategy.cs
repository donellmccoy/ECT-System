using System;
using System.Collections.Generic;
using ALOD.Core.Domain.Documents;
using ALOD.Core.Domain.Workflow;
using ALOD.Core.Interfaces;
using ALOD.Logging;

namespace ALODWebUtility.Documents
{
    public class DefaultProcessingStrategy : IDocumentProcessingStrategy
    {
        private List<string> _processingErrors = new List<string>();

        // Implementing the GetProcessingErrors method from the IDocumentProcessingStrategy interface
        public IList<string> GetProcessingErrors()
        {
            return _processingErrors;
        }

        // Implementing the ProcessDocument method from the IDocumentProcessingStrategy interface
        public long ProcessDocument(int refId, long groupId, IDocumentDao docDao, Document metaData, byte[] fileData)
        {
            if (refId <= 0)
            {
                // Handle invalid refId
                LogManager.LogError("ProcessDocument called with invalid refId: " + refId);
                _processingErrors.Add("Invalid reference ID provided.");
                return 0;
            }

            long docId = 0;
            try
            {
                // Attempt to add the document
                docId = docDao.AddDocument(fileData, metaData.OriginalFileName, groupId, metaData);

                // Check if the document ID is valid
                if (docId <= 0)
                {
                    // Log error with specific details
                    LogManager.LogError("ProcessDocument: Failed with docId=" + docId + ", refId=" + refId + ", groupId=" + groupId);
                    _processingErrors.Add("Failed to upload document. Please contact a system administrator and provide them with the Reference Id Number " + refId + ".");
                    return 0;
                }

                // Log successful action
                LogManager.LogAction((int)ModuleType.System, UserAction.AddedDocument, "Added document: " + metaData.OriginalFileName + ". RefId = " + refId + ". DocGroupId = " + groupId + ". DocId = " + docId + ".");

                return docId;
            }
            catch (Exception ex)
            {
                // Log exception details
                LogManager.LogError("ProcessDocument Exception: " + ex.ToString());
                _processingErrors.Add("An error occurred while processing the document: " + ex.Message);
                return 0;
            }
        }
    }
}
