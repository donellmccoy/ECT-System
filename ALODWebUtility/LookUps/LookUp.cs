using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Web.UI.WebControls;
using System.Xml;
using AjaxControlToolkit;
using ALOD.Core.Domain.Common;
using ALOD.Core.Domain.Modules.Lod;
using ALOD.Data;

namespace ALODWebUtility.LookUps
{
    public class LookUp : LookUpTable
    {
        // Public Function GetMemberTypes() As LookUp
        //     Dim cmd As DbCommand = DataStore.GetSqlStringCommand("select  memberType, memberDescr from core_lkupMemberType Order By id")
        //     GetCollection(DataStore, "memberType", cmd)
        //     Return Me
        // End Function

        public IEnumerable<ICD9Code> GetCategory(int headingId)
        {
            ICD9CodeDao icdDao = new NHibernateDaoFactory().GetICD9CodeDao();
            var query = from r in icdDao.GetAll() where r.ParentId == headingId && r.Active select r;
            return query;
        }

        public IEnumerable<ICD9Code> GetChapter()
        {
            ICD9CodeDao icdDao = new NHibernateDaoFactory().GetICD9CodeDao();
            var query = from r in icdDao.GetAll() where r.Code == null && r.ParentId == null && r.Active orderby r.SortOrder select r;

            return query;
        }

        public LookUp GetGroupsByCompo(string compo)
        {
            DbCommand cmd = DataStore.GetStoredProcCommand("core_group_sp_GetByCompo");
            DataStore.AddInParameter(cmd, "@compo", DbType.String, compo);
            GetCollection(DataStore, "groups_compo_" + compo, cmd);
            return this;
        }

        public DataSets.LookupDataTable GetGroupsByCompoAsDataSet(string compo)
        {
            return GetGroupsByCompo(compo).ToDataSet();
        }

        public IEnumerable<ICD9Code> GetICDChildren(int parentId)
        {
            ICD9CodeDao icdDao = new NHibernateDaoFactory().GetICD9CodeDao();
            var query = from r in icdDao.GetAll() where r.ParentId == parentId && r.Active select r;
            return query;
        }

        public CascadingDropDownNameValue[] GetICDCodeCDDValues(StringDictionary categoryValues, string parentCategory)
        {
            if (!categoryValues.ContainsKey(parentCategory))
            {
                return null;
            }

            int selectedParentId = Convert.ToInt32(categoryValues[parentCategory]);
            IEnumerable<ICD9Code> children = GetICDChildren(selectedParentId);
            List<CascadingDropDownNameValue> values = new List<CascadingDropDownNameValue>();
            string text = string.Empty;

            foreach (ICD9Code item in children)
            {
                if (string.IsNullOrEmpty(item.Code))
                {
                    text = item.Description;
                }
                else
                {
                    text = item.Description + " - " + item.Code;
                }

                values.Add(new CascadingDropDownNameValue(text, item.Id.ToString()));
            }

            return values.ToArray();
        }

        public List<ListItem> GetICDIncident(int codeId)
        {
            if (codeId == 0)
            {
                return null;
            }

            ICD9CodeDao icdDao = new NHibernateDaoFactory().GetICD9CodeDao();
            ICD9Code code = icdDao.GetById(codeId);
            List<ListItem> values = new List<ListItem>();

            List<NatureOfIncident> incidentValues = icdDao.GetAssociatedNatureOfIncidentValues(codeId);

            if (incidentValues != null && incidentValues.Count > 0)
            {
                foreach (NatureOfIncident v in incidentValues)
                {
                    values.Add(new ListItem(v.Text, v.Value));
                }
            }
            else
            {
                values.Add(new ListItem("Illness", "Illness"));
                values.Add(new ListItem("Injury", "Injury"));
                values.Add(new ListItem("Injury MVA", "Injury-MVA"));
                values.Add(new ListItem("Disease", "Disease"));
                values.Add(new ListItem("Death", "Death"));
            }

            // Commented out legacy code preserved from VB version
            // If code.Id < 467 Then...

            return values;
        }

        public DataSet GetManagedGroups(int groupId)
        {
            return DataStore.ExecuteDataSet("core_group_sp_GetManaged", groupId);
        }

        public DataSet GetManagedGroupsDropDown(int groupId)
        {
            return DataStore.ExecuteDataSet("core_group_sp_GetManagedDropDown", groupId);
        }

        public DataSet GetStates()
        {
            DbCommand cmd = DataStore.GetSqlStringCommand("select state, state_name from core_lkupStates order by country desc, state_name");
            return DataStore.ExecuteDataSet(cmd);
        }

        public DataSet GetViewedGroups(int groupId)
        {
            return DataStore.ExecuteDataSet("core_group_sp_GetViewBy", groupId);
        }

        public LookUp GetWorkflowActionTypes()
        {
            DbCommand cmd = DataStore.GetSqlStringCommand("SELECT type, text FROM core_lkupWorkflowAction ORDER BY text");
            GetCollection(DataStore, "workflowActions", cmd);
            return this;
        }

        public DataSets.LookupDataTable GetWorkflowActionTypesAsDataSet()
        {
            return GetWorkflowActionTypes().ToDataSet();
        }

        public DataSet SearchUsers(int userId, string ssn, string name, byte status, int role, int unitId, bool showAllUsers)
        {
            return DataStore.ExecuteDataSet("core_user_sp_GetManagedUsers", userId, ssn, name, status, role, unitId, showAllUsers);
        }

        #region Caching

        public DataSets.LookupDataTable ToDataSet()
        {
            DataSets.LookupDataTable data = new DataSets.LookupDataTable();
            DataSets.LookupRow row;

            foreach (LookUpRow item in _rows)
            {
                row = data.NewLookupRow();
                row.key = item.text;
                row.value = item.value;
                data.Rows.Add(row);
            }

            return data;
        }

        public void ToXml(ref XmlNode parentNode)
        {
            XmlDocument doc = parentNode.OwnerDocument;

            string section = "";
            int currentId = 0;

            foreach (LookUpRow row in _rows)
            {
                XmlNode node = doc.CreateElement("Row");

                node.Attributes.Append(doc.CreateAttribute("Text"));
                node.Attributes["Text"].Value = row.text;

                node.Attributes.Append(doc.CreateAttribute("Value"));
                node.Attributes["Value"].Value = row.value;

                parentNode.AppendChild(node);
            }
        }

        protected void GetCollection(string key, string procName, params object[] parameterValues)
        {
            GetCollection(DataStore, key, DataStore.GetStoredProcCommand(procName, parameterValues));
        }

        protected void GetCollection(SqlDataStore adapter, string key, DbCommand cmd)
        {
            if (Cache[key] != null)
            {
                // we have this item in cache
                _rows = (List<LookUpRow>)Cache[key];
            }
            else
            {
                // get it from the db
                adapter.ExecuteReader(LookUpRowReader, cmd);
                Cache.Insert(key, _rows);
            }
        }

        #endregion
    }
}
