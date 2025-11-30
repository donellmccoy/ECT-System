using System;
using System.Collections.Generic;
using System.Xml;

namespace ALOD.Core.Domain.Log
{
    public class ChangeSet : IList<ChangeRow>
    {
        public ChangeSet()
        {
            LogId = 0;
            ActionDate = DateTime.Now;
            UserId = 0;
            LastName = "";
            FirstName = "";
            Rank = "";
            Changes = new List<ChangeRow>();
        }

        public DateTime ActionDate { get; set; }
        public IList<ChangeRow> Changes { get; set; }
        public string FirstName { get; set; }

        public string FullNameRank
        {
            get
            {
                if (!String.IsNullOrEmpty(this.Rank) && this.Rank.ToLower() != "civ")
                {
                    return this.Rank + " " + this.LastName + ", " + this.FirstName;
                }

                return this.LastName + ", " + this.FirstName;
            }
        }

        public string LastName { get; set; }
        public int LogId { get; set; }
        public string Rank { get; set; }
        public int UserId { get; set; }

        public void Add(string section, string field, string oldVal, string newVal)
        {
            ChangeRow change = new ChangeRow(section, field, oldVal, newVal);
            Changes.Add(change);
        }

        public void ToXml(ref XmlNode parentNode)
        {
            XmlDocument doc = parentNode.OwnerDocument;

            int currentId = 0;
            XmlNode groupNode = doc.CreateElement("ChangeSet");

            foreach (ChangeRow row in Changes)
            {
                if ((currentId != row.Id))
                {
                    currentId = row.Id;
                    groupNode = doc.CreateElement("ChangeSet");

                    groupNode.Attributes.Append(doc.CreateAttribute("logId"));
                    groupNode.Attributes["logId"].Value = this.LogId.ToString();

                    groupNode.Attributes.Append(doc.CreateAttribute("ActionDate"));
                    groupNode.Attributes["ActionDate"].Value = this.ActionDate.ToString();

                    groupNode.Attributes.Append(doc.CreateAttribute("LastName"));
                    groupNode.Attributes["LastName"].Value = this.LastName;

                    groupNode.Attributes.Append(doc.CreateAttribute("FirstName"));
                    groupNode.Attributes["FirstName"].Value = this.FirstName;

                    groupNode.Attributes.Append(doc.CreateAttribute("Rank"));
                    groupNode.Attributes["Rank"].Value = this.Rank;

                    groupNode.Attributes.Append(doc.CreateAttribute("UserId"));
                    groupNode.Attributes["UserId"].Value = this.UserId.ToString();

                    parentNode.AppendChild(groupNode);
                }

                XmlNode node = doc.CreateElement("ChangeRow");

                node.Attributes.Append(doc.CreateAttribute("Section"));
                node.Attributes["Section"].Value = row.Section;

                node.Attributes.Append(doc.CreateAttribute("Field"));
                node.Attributes["Field"].Value = row.Field;

                node.Attributes.Append(doc.CreateAttribute("OldValue"));
                node.Attributes["OldValue"].Value = row.OldVal;

                node.Attributes.Append(doc.CreateAttribute("NewValue"));
                node.Attributes["NewValue"].Value = row.NewVal;

                groupNode.AppendChild(node);
            }
        }

        #region IList<ChangeRow> Members

        /// <inheritdoc/>
        public ChangeRow this[int index]
        {
            get
            {
                return Changes[index];
            }
            set
            {
                Changes[index] = value;
            }
        }

        /// <inheritdoc/>
        public int IndexOf(ChangeRow item)
        {
            return Changes.IndexOf(item);
        }

        /// <inheritdoc/>
        public void Insert(int index, ChangeRow item)
        {
            Changes.Insert(index, item);
        }

        /// <inheritdoc/>
        public void RemoveAt(int index)
        {
            Changes.RemoveAt(index);
        }

        #endregion IList<ChangeRow> Members

        #region ICollection<ChangeRow> Members

        /// <inheritdoc/>
        public int Count
        {
            get { return Changes.Count; }
        }

        /// <inheritdoc/>
        public bool IsReadOnly
        {
            get { return false; }
        }

        /// <inheritdoc/>
        public void Add(ChangeRow item)
        {
            Changes.Add(item);
        }

        /// <inheritdoc/>
        public void Clear()
        {
            Changes.Clear();
        }

        /// <inheritdoc/>
        public bool Contains(ChangeRow item)
        {
            return Changes.Contains(item);
        }

        /// <inheritdoc/>
        public void CopyTo(ChangeRow[] array, int arrayIndex)
        {
            Changes.CopyTo(array, arrayIndex);
        }

        /// <inheritdoc/>
        public bool Remove(ChangeRow item)
        {
            return Changes.Remove(item);
        }

        #endregion ICollection<ChangeRow> Members

        #region IEnumerable<ChangeRow> Members

        /// <inheritdoc/>
        public IEnumerator<ChangeRow> GetEnumerator()
        {
            return Changes.GetEnumerator();
        }

        #endregion IEnumerable<ChangeRow> Members

        #region IEnumerable Members

        /// <inheritdoc/>
        global::System.Collections.IEnumerator global::System.Collections.IEnumerable.GetEnumerator()
        {
            return Changes.GetEnumerator();
        }

        #endregion IEnumerable Members
    }
}