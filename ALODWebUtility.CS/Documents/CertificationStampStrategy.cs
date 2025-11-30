using System;
using System.Collections.Generic;
using System.Linq;
using ALOD.Core.Domain.Documents;
using ALOD.Core.Domain.Lookup;
using ALOD.Core.Domain.Workflow;
using ALOD.Core.Interfaces;
using ALOD.Core.Interfaces.DAOInterfaces;
using ALOD.Data;
using ALOD.Logging;
using ALODWebUtility.Printing;

namespace ALODWebUtility.Documents
{
    public class CertificationStampStrategy : IDocumentProcessingStrategy
    {
        private int _certificateId;
        private List<string> _processingErrors;
        private ISpecialCaseDAO _specCaseDao;
        private ICertificationStampDao _stampDao;

        public CertificationStampStrategy()
        {
            _processingErrors = new List<string>();
        }

        protected ISpecialCaseDAO SpecCaseDao
        {
            get
            {
                if (_specCaseDao == null)
                {
                    _specCaseDao = new NHibernateDaoFactory().GetSpecialCaseDAO();
                }

                return _specCaseDao;
            }
        }

        protected ICertificationStampDao StampDao
        {
            get
            {
                if (_stampDao == null)
                {
                    _stampDao = new NHibernateDaoFactory().GetCertificationStampDao();
                }

                return _stampDao;
            }
        }

        public IList<string> GetProcessingErrors()
        {
            if (_processingErrors == null)
            {
                _processingErrors = new List<string>();
            }

            return _processingErrors;
        }

        public long ProcessDocument(int refId, long groupId, IDocumentDao docDao, Document metaData, byte[] fileData)
        {
            string processingError = string.Empty;

            if (!metaData.Extension.ToLower().Equals("pdf"))
            {
                processingError = "Incorrect file extenstion (" + metaData.Extension + "). Document must be a PDF.";
                _processingErrors.Add(processingError);
                LogManager.LogError("CertificationStampStrategy.ProcessDocument(): Incorrect file extenstion (" + metaData.Extension + "). Document must be a PDF. RefId = " + refId + ".");
                return 0;
            }

            // Get stamp data...
            CertificationStamp stamp = StampDao.GetSpecialCaseStamp(refId, false);

            if (stamp == null)
            {
                processingError = GetGenericErrorMessage(refId);
                _processingErrors.Add(processingError);
                LogManager.LogError("CertificationStampStrategy.ProcessDocument(): Failed to load certification stamp. RefId = " + refId + ".");
                return 0;
            }

            string stampBody = stamp.PopulatedBody(StampDao.GetStampData(refId, false), 50);

            if (string.IsNullOrEmpty(stampBody))
            {
                processingError = GetGenericErrorMessage(refId);
                _processingErrors.Add(processingError);
                LogManager.LogError("CertificationStampStrategy.ProcessDocument(): Failed to load certification stamp body. RefId = " + refId + ". StampId = " + stamp.Id + ".");
                return 0;
            }

            // Get optional secondary stamp if it exists...
            CertificationStamp secondaryStamp = StampDao.GetSpecialCaseStamp(refId, true);
            string secondaryStampBody = string.Empty;

            if (secondaryStamp != null)
            {
                secondaryStampBody = secondaryStamp.PopulatedBody(StampDao.GetStampData(refId, true), 50);
            }

            // Read document data into PDFDocument object
            PDFDocument doc = new PDFDocument();

            if (doc == null)
            {
                processingError = GetGenericErrorMessage(refId);
                _processingErrors.Add(processingError);
                LogManager.LogError("CertificationStampStrategy.ProcessDocument(): Failed to load a PDFDocument object. RefId = " + refId + ". StampId = " + stamp.Id + ".");
                return 0;
            }

            if (!doc.Read(fileData))
            {
                processingError = GetGenericErrorMessage(refId);
                _processingErrors.Add(processingError);
                LogManager.LogError("CertificationStampStrategy.ProcessDocument(): Failed to read document data. RefId = " + refId + ". StampId = " + stamp.Id + ".");
                return 0;
            }

            // Add stamps to the PDF document...
            if (!string.IsNullOrEmpty(secondaryStampBody))
            {
                AddStampToDoc(doc, stampBody, 0.05, 0.5);
                AddStampToDoc(doc, secondaryStampBody, 0.05, 0.2);
            }
            else
            {
                // Add stamp to the PDF document...
                AddStampToDoc(doc, stampBody, 0.05, 0.333);
            }

            // Render to file...
            doc.IncludeFOUOWatermark = false;
            doc.Render(metaData.OriginalFileName);

            // Add document to database...
            long docId = docDao.AddDocument(doc.GetBuffer(), metaData.OriginalFileName, groupId, metaData);

            if (docId <= 0)
            {
                processingError = GetGenericErrorMessage(refId);
                _processingErrors.Add(processingError);
                LogManager.LogError("CertificationStampStrategy.ProcessDocument(): SRXDocumentStore.AddDocument() failed to add document to database. RefId = " + refId + ".");
                return 0;
            }

            LogManager.LogAction(ModuleType.System, UserAction.AddedDocument, "Added Certification Stamped document: " + metaData.OriginalFileName + ". RefId = " + refId + ". DocGroupId = " + groupId + ". DocId = " + docId + ".");

            return docId;
        }

        protected void AddStampToDoc(PDFDocument doc, string stampBody)
        {
            List<string> stampParts = stampBody.Split(new string[] { Environment.NewLine }, StringSplitOptions.None).ToList();
            List<PDFString> stampPDFString = new List<PDFString>();

            foreach (string s in stampParts)
            {
                stampPDFString.Add(new PDFString() { Text = s, Color = "#284780", FontSize = 14, Alignment = "left", Rotation = 30 });
            }

            doc.AddStamp(stampPDFString, true, 0.05, 0.333, 8);
        }

        protected void AddStampToDoc(PDFDocument doc, string stampBody, double widthRatio, double heightRatio)
        {
            List<string> stampParts = stampBody.Split(new string[] { Environment.NewLine }, StringSplitOptions.None).ToList();
            List<PDFString> stampPDFString = new List<PDFString>();

            foreach (string s in stampParts)
            {
                stampPDFString.Add(new PDFString() { Text = s, Color = "#284780", FontSize = 14, Alignment = "left", Rotation = 30 });
            }

            doc.AddStamp(stampPDFString, true, widthRatio, heightRatio, 8);
        }

        private string GetGenericErrorMessage(int refId)
        {
            return "Failed to process document. Please contact a system administrator and provide them with the Reference Id Number " + refId + ".";
        }
    }
}
