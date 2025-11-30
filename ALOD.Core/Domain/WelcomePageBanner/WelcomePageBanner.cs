using ALOD.Core.Domain.Common.KeyVal;
using ALOD.Core.Domain.Documents;
using ALOD.Core.Interfaces.DAOInterfaces;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.RegularExpressions;

namespace ALOD.Core.Domain.WelcomePageBanner
{
    public class WelcomePageBanner
    {
        #region Fields...

        private IList<HyperLink> _hyperLinks;

        #endregion Fields...

        #region Properties...

        public virtual IDocumentDao DocumentDao { get; private set; }
        public virtual bool Enabled { get; set; }
        public virtual int HelpDocumentsDocGroupId { get; set; }
        public virtual IHyperLinkDao HyperLinkDao { get; private set; }

        public virtual ReadOnlyCollection<HyperLink> HyperLinks
        {
            // Construction of the ReadOnlyCollection is a O(1) operation...
            get { return new ReadOnlyCollection<HyperLink>(_hyperLinks); }
        }

        public virtual IKeyValDao KeyValDao { get; private set; }

        public virtual string PopulatedBannerText
        { get { return PopulateParameters(); } }

        public virtual string RawBannerText { get; set; }

        #endregion Properties...

        #region Constructors...

        public WelcomePageBanner()
        {
            this.RawBannerText = string.Empty;
            this._hyperLinks = new List<HyperLink>();
        }

        public WelcomePageBanner(IKeyValDao keyValDao, IHyperLinkDao hyperLinkDao, IDocumentDao documentDao, int helpDocumentsDocGroupId)
        {
            this.KeyValDao = keyValDao;
            this.HyperLinkDao = hyperLinkDao;
            this.DocumentDao = documentDao;
            this.HelpDocumentsDocGroupId = helpDocumentsDocGroupId;

            KeyValValue enabledValue = KeyValDao.GetKeyValuesByKeyDesciption("Welcome Page Banner").Where(x => x.ValueDescription.Equals("Banner Enabled")).FirstOrDefault();
            KeyValValue textValue = KeyValDao.GetKeyValuesByKeyDesciption("Welcome Page Banner").Where(x => x.ValueDescription.Equals("Banner Text")).FirstOrDefault();

            if (enabledValue != null && enabledValue.Value.Equals("1"))
                Enabled = true;
            else
                Enabled = false;

            if (textValue != null && !string.IsNullOrEmpty(textValue.Value))
                this.RawBannerText = textValue.Value;
            else
                this.RawBannerText = string.Empty;

            this._hyperLinks = HyperLinkDao.GetAll().ToList();
        }

        #endregion Constructors...

        #region Operations...

        public virtual void Save()
        {
            // Save enabled setting...
            KeyValValue enabledValue = KeyValDao.GetKeyValuesByKeyDesciption("Welcome Page Banner").Where(x => x.ValueDescription.Equals("Banner Enabled")).FirstOrDefault();

            if (enabledValue != null)
            {
                KeyValDao.UpdateKeyValueById(enabledValue.Id, enabledValue.Key.Id, enabledValue.ValueDescription, Convert.ToByte(Enabled).ToString());
            }

            // Save raw banner content text...
            KeyValValue textValue = KeyValDao.GetKeyValuesByKeyDesciption("Welcome Page Banner").Where(x => x.ValueDescription.Equals("Banner Text")).FirstOrDefault();

            if (textValue != null)
            {
                KeyValDao.UpdateKeyValueById(textValue.Id, textValue.Key.Id, textValue.ValueDescription, RawBannerText);
            }
        }

        protected virtual string BuildDocumentLink(HyperLink l, IList<Document> documents)
        {
            if (l == null)
                return string.Empty;

            if (!IsValidDocumentLink(l, documents))
                return "[DOCUMENT NOT FOUND]";

            string logMessage = "Help Document with ID = " + l.Value;
            // <a id="" onclick="viewDoc('../Shared/DocumentViewer.aspx?docId=4074&amp;modId=1&amp;doc=Q3+2016+Release+Notes'); return false;" href="#">Q3 2016 Release Notes</a>
            return "<a onclick=\"viewDoc('Shared/DocumentViewer.aspx?docId=" + l.Value + "&amp;modId=1&amp;doc=" + logMessage.Replace(" ", "+") + "'); return false;\" href=\"#\">" + l.DisplayText + "</a>";
        }

        protected virtual string BuildWebsiteLink(HyperLink l)
        {
            if (l == null)
                return string.Empty;

            // <a href="https://www.google.com">here</a>
            return "<a href=\"" + l.Value + "\" target=\"_blank\">" + l.DisplayText + "</a>";
        }

        protected virtual bool IsValidDocumentLink(HyperLink l, IList<Document> documents)
        {
            long documentId = long.Parse(l.Value);

            if (documentId == 0)
                return false;

            Document linkedDocument = documents.SingleOrDefault(x => x.Id == documentId);

            if (linkedDocument == null || linkedDocument.DocStatus == DocumentStatus.Deleted)
                return false;

            return true;
        }

        protected virtual string PopulateParameters()
        {
            string populatedContent = RawBannerText;
            IList<Document> documents = DocumentDao.GetDocumentsByGroupId(HelpDocumentsDocGroupId);

            foreach (HyperLink l in HyperLinks)
            {
                if (l.Type.Name.Equals("Document"))
                {
                    populatedContent = Regex.Replace(populatedContent, "{" + l.Name.ToUpper() + "}", BuildDocumentLink(l, documents), RegexOptions.IgnoreCase);
                }

                if (l.Type.Name.Equals("Website"))
                {
                    populatedContent = Regex.Replace(populatedContent, "{" + l.Name.ToUpper() + "}", BuildWebsiteLink(l), RegexOptions.IgnoreCase);
                }
            }

            return populatedContent;
        }

        #endregion Operations...
    }
}