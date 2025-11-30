using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Xml;
using ALOD.Core.Domain.Workflow;
using ALOD.Data;

namespace ALODWebUtility.Common
{
    public class ChangeSet : IList<ChangeRow>
    {
        protected Dictionary<int, ChangeSetDetails> _details = new Dictionary<int, ChangeSetDetails>();
        protected List<ChangeRow> _sets = new List<ChangeRow>();

        public void Add(string section, string field, string oldVal, string newVal)
        {
            ChangeRow change = new ChangeRow(section, field, oldVal, newVal);
            _sets.Add(change);
        }

        public void GetByLogId(int logId)
        {
            _sets.Clear();
            SqlDataStore adapter = new SqlDataStore();
            adapter.ExecuteReader(SetReader, "core_log_sp_GetChangeSetByLogId", logId);
            GetDetails();
        }

        public void GetByReferenceId(int refId, ModuleType type)
        {
            _sets.Clear();
            SqlDataStore adapter = new SqlDataStore();
            adapter.ExecuteReader(SetReader, "core_log_sp_GetChangeSetsByRefId", refId, type);
            GetDetails();
        }

        public void GetByUserID(int userID)
        {
            _sets.Clear();
            SqlDataStore adapter = new SqlDataStore();
            adapter.ExecuteReader(SetReader, "core_log_sp_GetChangeSetByUserId", userID);
            GetDetailsByUserId(userID);
        }

        public void GetDetails()
        {
            if (_sets.Count == 0)
            {
                return;
            }

            //get a list of unique log ids
            List<int> list = new List<int>();

            foreach (ChangeRow row in _sets)
            {
                if (!list.Contains(row.Id))
                {
                    list.Add(row.Id);
                }
            }

            StringBuilder buffer = new StringBuilder();

            foreach (int id in list)
            {
                buffer.Append(id.ToString() + ",");
            }

            if (buffer.Length > 0)
            {
                buffer.Remove(buffer.Length - 1, 1);
            }

            //now get the details for this
            SqlDataStore adapter = new SqlDataStore();
            adapter.ExecuteReader(DetailsReader, "core_log_sp_GetChangeSetDetails", buffer.ToString());
        }

        public void Save(int id)
        {
            XMLString xml = new XMLString("XML_Array");

            foreach (ChangeRow item in _sets)
            {
                xml.BeginElement("XMLList");
                xml.WriteAttribute("ID", id.ToString());
                xml.WriteAttribute("section", item.Section);
                xml.WriteAttribute("field", item.Field);
                xml.WriteAttribute("oldVal", item.OldVal);
                xml.WriteAttribute("newVal", item.NewVal);
                xml.EndElement();
            }

            SqlDataStore adapter = new SqlDataStore();
            adapter.ExecuteNonQuery("core_log_sp_InsertChangeSet", xml.Value);
        }

        public void ToXml(ref XmlNode parentNode, bool repeatHeading = true)
        {
            XmlDocument doc = parentNode.OwnerDocument;

            string section = "";
            int currentId = 0;
            bool noRepeat = true;
            XmlNode groupNode = doc.CreateElement("ChangeSet");

            foreach (ChangeRow row in _sets)
            {
                if (currentId != row.Id && noRepeat)
                {
                    currentId = row.Id;
                    groupNode = doc.CreateElement("ChangeSet");

                    if (_details.ContainsKey(row.Id))
                    {
                        ChangeSetDetails details = _details[row.Id];
                        noRepeat = repeatHeading;
                        
                        XmlAttribute logIdAttr = doc.CreateAttribute("logId");
                        logIdAttr.Value = details.LogId.ToString();
                        groupNode.Attributes.Append(logIdAttr);

                        XmlAttribute actionDateAttr = doc.CreateAttribute("ActionDate");
                        actionDateAttr.Value = details.ActionDate.ToString();
                        groupNode.Attributes.Append(actionDateAttr);

                        XmlAttribute lastNameAttr = doc.CreateAttribute("LastName");
                        lastNameAttr.Value = details.LastName;
                        groupNode.Attributes.Append(lastNameAttr);

                        XmlAttribute firstNameAttr = doc.CreateAttribute("FirstName");
                        firstNameAttr.Value = details.FirstName;
                        groupNode.Attributes.Append(firstNameAttr);

                        XmlAttribute rankAttr = doc.CreateAttribute("Rank");
                        rankAttr.Value = details.Rank;
                        groupNode.Attributes.Append(rankAttr);

                        XmlAttribute userIdAttr = doc.CreateAttribute("UserId");
                        userIdAttr.Value = details.UserId.ToString();
                        groupNode.Attributes.Append(userIdAttr);
                    }

                    parentNode.AppendChild(groupNode);
                }

                XmlNode node = doc.CreateElement("ChangeRow");

                XmlAttribute sectionAttr = doc.CreateAttribute("Section");
                sectionAttr.Value = row.Section;
                node.Attributes.Append(sectionAttr);

                XmlAttribute fieldAttr = doc.CreateAttribute("Field");
                fieldAttr.Value = row.Field;
                node.Attributes.Append(fieldAttr);

                XmlAttribute oldValueAttr = doc.CreateAttribute("OldValue");
                oldValueAttr.Value = row.OldVal;
                node.Attributes.Append(oldValueAttr);

                XmlAttribute newValueAttr = doc.CreateAttribute("NewValue");
                newValueAttr.Value = row.NewVal;
                node.Attributes.Append(newValueAttr);

                groupNode.AppendChild(node);
            }
        }

        private void DetailsReader(SqlDataStore adapter, IDataReader reader)
        {
            //0-logId, 1-actionDate, 2-userId, 3-lastName, 4-firstName, 5-rank

            ChangeSetDetails details = new ChangeSetDetails();

            details.LogId = adapter.GetInteger(reader, 0);
            details.ActionDate = adapter.GetDateTime(reader, 1);
            details.UserId = adapter.GetInteger(reader, 2);
            details.LastName = adapter.GetString(reader, 3);
            details.FirstName = adapter.GetString(reader, 4);
            details.Rank = adapter.GetString(reader, 5);

            _details.Add(details.LogId, details);
        }

        private void GetDetailsByUserId(int userId)
        {
            SqlDataStore adapter = new SqlDataStore();
            adapter.ExecuteReader(DetailsReader, "core_log_sp_GetLastChange", userId);
        }

        private void SetReader(SqlDataStore adapter, IDataReader reader)
        {
            //0-logId, 1-section, 2-field, 3-old, 4-new, 5-actionId, 6-actionName
            ChangeRow change = new ChangeRow();

            change.Id = adapter.GetInteger(reader, 0);
            change.Section = adapter.GetString(reader, 1);
            change.Field = adapter.GetString(reader, 2);
            change.OldVal = adapter.GetString(reader, 3);
            change.NewVal = adapter.GetString(reader, 4);
            change.ActionId = (int)adapter.GetNumber(reader, 5); // GetNumber likely returns decimal or double, casting to int
            change.ActionName = adapter.GetString(reader, 6);
            change.UserId = (int)adapter.GetNumber(reader, 7);
            change.UserName = adapter.GetString(reader, 8);
            change.ActionDate = adapter.GetDateTime(reader, 9);

            _sets.Add(change);
        }

        #region IList

        public int Count
        {
            get { return _sets.Count; }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        public ChangeRow this[int index]
        {
            get { return _sets[index]; }
            set { _sets[index] = value; }
        }

        public void Add(ChangeRow item)
        {
            _sets.Add(item);
        }

        public void Clear()
        {
            _sets.Clear();
        }

        public bool Contains(ChangeRow item)
        {
            return _sets.Contains(item);
        }

        public void CopyTo(ChangeRow[] array, int arrayIndex)
        {
            _sets.CopyTo(array, arrayIndex);
        }

        public IEnumerator<ChangeRow> GetEnumerator()
        {
            return _sets.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _sets.GetEnumerator();
        }

        public int IndexOf(ChangeRow item)
        {
            return _sets.IndexOf(item);
        }

        public void Insert(int index, ChangeRow item)
        {
            _sets.Insert(index, item);
        }

        public bool Remove(ChangeRow item)
        {
            return _sets.Remove(item);
        }

        public void RemoveAt(int index)
        {
            _sets.RemoveAt(index);
        }

        #endregion
    }
}
