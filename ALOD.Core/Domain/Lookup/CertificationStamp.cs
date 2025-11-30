using ALOD.Core.Utils;
using System.Collections.Generic;

namespace ALOD.Core.Domain.Lookup
{
    public class CertificationStamp
    {
        public CertificationStamp()
        {
            Id = 0;
            Name = string.Empty;
            Body = string.Empty;
            IsQualified = false;
        }

        public CertificationStamp(int id, string name, string body, bool isQualified)
        {
            this.Id = id;
            this.Name = name;
            this.Body = body;
            this.IsQualified = isQualified;
        }

        public virtual string Body { get; set; }
        public virtual int Id { get; set; }
        public virtual bool IsQualified { get; set; }
        public virtual string Name { get; set; }

        public virtual string PopulatedBody(IDictionary<string, string> data)
        {
            string text = Body;

            foreach (KeyValuePair<string, string> pair in data)
            {
                text = text.Replace("{" + pair.Key.ToUpper() + "}", System.Web.HttpUtility.HtmlDecode(pair.Value));
            }

            return text;
        }

        public virtual string PopulatedBody(IDictionary<string, string> data, int wordWrapLength)
        {
            string text = Body;

            foreach (KeyValuePair<string, string> pair in data)
            {
                text = text.Replace("{" + pair.Key.ToUpper() + "}", System.Web.HttpUtility.HtmlDecode(pair.Value));
            }

            text = Helpers.WordWrapByCharacterLength(text, wordWrapLength);

            return text;
        }
    }
}