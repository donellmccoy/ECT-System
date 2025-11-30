using ALOD.Core.Domain.Users;
using ALOD.Core.Interfaces;
using System;
using System.Collections.Generic;

namespace ALOD.Core.Domain.Documents
{
    public class MemoTemplate : Entity, IAuditable
    {
        private const string DATE_FORMAT = "d MMMM yyyy";
        public virtual bool Active { get; set; }
        public virtual bool AddDate { get; set; }
        public virtual bool AddSignature { get; set; }
        public virtual bool AddSuspenseDate { get; set; }
        public virtual string Attachments { get; set; }
        public virtual string Body { get; set; }
        public virtual string Component { get; set; }
        public virtual bool ContainsPhi { get; set; }
        public virtual AppUser CreatedBy { get; set; }
        public virtual DateTime CreatedDate { get; set; }
        public virtual string DataSource { get; set; }

        public virtual string DateFormat
        {
            get { return DATE_FORMAT; }
        }

        public virtual int FontSize { get; set; }
        public virtual IList<MemoGroup> GroupPermissions { get; set; }

        //If a new memo is added MemoTypes Enumeration should be updated
        public virtual string MemoType
        {
            get
            {
                switch (Component)
                {
                    case "6":
                        return ((MemoType)Id).ToString();

                    case "5":
                        return ((MemoType)Id).ToString();
                }
                return "";
            }
        }

        public virtual AppUser ModifiedBy { get; set; }

        /// <inheritdoc/>
        public virtual DateTime ModifiedDate { get; set; }

        public virtual byte ModuleType { get; set; }
        public virtual string SignatureBlock { get; set; }
        public virtual string Title { get; set; }

        // gets the cc value and places it in the Attachments textbox
        public virtual string GetCC(IDictionary<string, string> data)
        {
            string ccText = Attachments;
            foreach (KeyValuePair<string, string> pair in data)
            {
                if (pair.Key == "RMU" || pair.Key == "MED_GROUP")
                {
                    if (String.IsNullOrEmpty(pair.Value))
                    {
                        ccText = "";
                    }
                    else
                    {
                        ccText = ccText.Replace("{" + pair.Key.ToUpper() + "}", pair.Value);
                    }
                }
            }
            return ccText;
        }

        public virtual string PopulatedBody(IDictionary<string, string> data)
        {
            string text = Body;

            foreach (KeyValuePair<string, string> pair in data)
            {
                text = text.Replace("{" + pair.Key.ToUpper() + "}", System.Web.HttpUtility.HtmlDecode(pair.Value));
            }

            return text;
        }
    }
}