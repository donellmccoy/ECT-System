using System;
using System.Collections.Specialized;

namespace ALOD.Core.Domain.Common
{
    [Serializable]
    public class EmailTemplate : Entity
    {
        public virtual bool Active { get; set; }
        public virtual string Body { get; set; }
        public virtual string Compo { get; set; }
        public virtual string DataProc { get; set; }
        public virtual DateTime Date { get; set; }
        public virtual string Subject { get; set; }
        public virtual string Title { get; set; }

        public virtual void PopulateData(StringDictionary collection)
        {
            foreach (string key in collection.Keys)
            {
                Body = Body.Replace("{" + key.ToUpper() + "}", collection[key]);
            }
        }
    }
}