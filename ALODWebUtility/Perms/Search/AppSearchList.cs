using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Web;
using ALOD.Core.Domain.Workflow;
using ALOD.Data;

namespace ALODWebUtility.Perms.Search
{
    public class AppSearchList : IList<AppSearch>
    {
        protected SqlDataStore _adapter;
        private int _maxResults = 0;
        List<AppSearch> _search = new List<AppSearch>();

        public int MaxResults
        {
            get { return _maxResults; }
            set { _maxResults = value; }
        }

        protected SqlDataStore DataStore
        {
            get
            {
                if (_adapter == null)
                {
                    _adapter = new SqlDataStore();
                }

                return _adapter;
            }
        }

        public AppSearchList GetActiveWorkOrders(short status, byte workflow, short formal, short overdue)
        {
            DbCommand cmd = DataStore.GetStoredProcCommand("form2173_sp_SearchActiveWorkOrders");

            DataStore.AddInParameter(cmd, "@userId", DbType.Int32, Convert.ToInt32(HttpContext.Current.Session["UserId"]));
            DataStore.AddInParameter(cmd, "@status", DbType.Int16, status);
            DataStore.AddInParameter(cmd, "@workflow", DbType.Byte, workflow != 0 ? (object)workflow : DBNull.Value);
            DataStore.AddInParameter(cmd, "@formal", DbType.Int16, formal != -1 ? (object)formal : DBNull.Value);
            DataStore.AddInParameter(cmd, "@overdue", DbType.Int16, overdue != -1 ? (object)overdue : DBNull.Value);

            DataStore.ExecuteReader(CodeReader, cmd);
            return this;
        }

        public DataSets.SearchResultDataTable GetActiveWorkOrdersDataSet(short status, byte workflow, short formal, short overdue)
        {
            return GetActiveWorkOrders(status, workflow, formal, overdue).ToDataSet();
        }

        public DataSets.SearchResultDataTable GetAllLodsAsDataSet(ModuleType type, string caseId = "", string ssn = "", string name = "", int statusId = 0, string workflow = "")
        {
            string scriptName = string.Empty;
            SqlDataStore.RowReader reader = ResultReader;

            switch (type)
            {
                case ModuleType.LOD:
                    scriptName = "form348_sp_Search";
                    break;
            }

            return GetAllLods(reader, scriptName, caseId, ssn, name, statusId, workflow).ToDataSet();
        }

        public AppSearchList GetSearchResultsForGroup(ModuleType type, string caseId = "", string ssn = "", string name = "", int statusId = 0, string AOR = "", string Tricare = "", string workflow = "")
        {
            string scriptName = string.Empty;
            SqlDataStore.RowReader reader = CodeReader;

            switch (type)
            {
                case ModuleType.LOD:
                    scriptName = "form2173_GroupSearch";
                    break;
                default:
                    break;
            }

            return GetSearchResult(reader, scriptName, caseId, ssn, name, statusId, AOR, Tricare, workflow);
        }

        public DataSets.SearchResultDataTable GetSearchResultsForGroupAsDataSet(ModuleType type, string caseId = "", string ssn = "", string name = "", int statusId = 0, string AOR = "", string Tricare = "", string workflow = "")
        {
            return GetSearchResultsForGroup(type, caseId, ssn, name, statusId, AOR, Tricare, workflow).ToDataSet();
        }

        public DataSets.SearchResultDataTable GetSearchResultsForUserAsDataSet(ModuleType type, string caseId = "", string ssn = "", string name = "", int statusId = 0, string AOR = "", string Tricare = "", string workflow = "")
        {
            string scriptName = string.Empty;
            SqlDataStore.RowReader reader = CodeReader;

            switch (type)
            {
                case ModuleType.LOD:
                    scriptName = "form2173_Search";
                    break;
                default:
                    break;
            }

            return GetSearchResult(reader, scriptName, caseId, ssn, name, statusId, AOR, Tricare, workflow).ToDataSet();
        }

        public AppSearchList SearchForEligibleMMSO(string searchTerm)
        {
            DataStore.ExecuteReader(CodeReader, "mmso_sp_newCase", searchTerm, Convert.ToInt32(HttpContext.Current.Session["UserId"]));
            return this;
        }

        public DataSets.SearchResultDataTable ToDataSet()
        {
            DataSets.SearchResultDataTable data = new DataSets.SearchResultDataTable();
            DataSets.SearchResultRow row;

            foreach (AppSearch search in _search)
            {
                row = data.NewSearchResultRow();
                search.ToDataRow(ref row);
                data.Rows.Add(row);
            }

            return data;
        }

        private void CodeReader(SqlDataStore adapter, IDataReader reader)
        {
            AppSearch listSearch = new AppSearch();

            listSearch.refID = adapter.GetInt32(reader, 0);
            listSearch.recID = adapter.GetInt32(reader, 1);
            listSearch.parentID = adapter.GetInt32(reader, 2);

            listSearch.CaseId = adapter.GetString(reader, 3);
            listSearch.WorkflowId = adapter.GetByte(reader, 11);
            listSearch.Workflow = adapter.GetString(reader, 12);
            listSearch.Name = adapter.GetString(reader, 7);
            listSearch.Uic = adapter.GetString(reader, 5);
            listSearch.Region = adapter.GetString(reader, 10);
            listSearch.Status = adapter.GetString(reader, 6);
            listSearch.Compo = adapter.GetString(reader, 13);
            listSearch.DateCreated = adapter.GetDateTime(reader, 14, DateTime.Now);
            listSearch.CanView = adapter.GetBoolean(reader, 15);
            listSearch.CanEdit = adapter.GetBoolean(reader, 16);
            listSearch.ModuleId = adapter.GetByte(reader, 17);
            listSearch.BaseType = (ModuleType)adapter.GetByte(reader, 18);
            listSearch.Returned = adapter.GetBoolean(reader, 19);
            listSearch.IsFormal = adapter.GetByte(reader, 20);
            listSearch.DateReceived = adapter.GetDateTime(reader, 21);
            listSearch.IsFinal = adapter.GetBoolean(reader, 22);

            _search.Add(listSearch);
        }

        private AppSearchList GetAllLods(SqlDataStore.RowReader rowReader, string scriptName, string caseId = "", string ssn = "", string name = "", int statusId = 0, string workFlow = "")
        {
            SqlDataStore adapter = DataStore;
            DbCommand cmd = adapter.GetStoredProcCommand(scriptName);

            adapter.AddInParameter(cmd, "@caseID", DbType.String, !string.IsNullOrEmpty(caseId) ? (object)caseId : DBNull.Value);
            adapter.AddInParameter(cmd, "@ssn", DbType.String, !string.IsNullOrEmpty(ssn) ? (object)ssn : DBNull.Value);
            adapter.AddInParameter(cmd, "@name", DbType.String, !string.IsNullOrEmpty(name) ? (object)name : DBNull.Value);
            adapter.AddInParameter(cmd, "@status", DbType.Int32, statusId != 0 ? (object)statusId : DBNull.Value);
            adapter.AddInParameter(cmd, "@userId", DbType.Int32, Convert.ToInt32(HttpContext.Current.Session["UserId"]));
            adapter.AddInParameter(cmd, "@compo", DbType.String, Convert.ToString(HttpContext.Current.Session["Compo"]));
            adapter.AddInParameter(cmd, "@maxCount", DbType.Int32, _maxResults);

            adapter.ExecuteReader(rowReader, cmd);

            return this;
        }

        private AppSearchList GetSearchResult(SqlDataStore.RowReader rowReader, string scriptName, string caseId = "", string ssn = "", string name = "", int statusId = 0, string AOR = "", string Tricare = "", string workFlow = "")
        {
            SqlDataStore adapter = DataStore;
            DbCommand cmd = adapter.GetStoredProcCommand(scriptName);

            adapter.AddInParameter(cmd, "@caseID", DbType.String, !string.IsNullOrEmpty(caseId) ? (object)caseId : DBNull.Value);
            adapter.AddInParameter(cmd, "@ssn", DbType.String, !string.IsNullOrEmpty(ssn) ? (object)ssn : DBNull.Value);
            adapter.AddInParameter(cmd, "@name", DbType.String, !string.IsNullOrEmpty(name) ? (object)name : DBNull.Value);
            adapter.AddInParameter(cmd, "@status", DbType.Int32, statusId != 0 ? (object)statusId : DBNull.Value);
            adapter.AddInParameter(cmd, "@userId", DbType.Int32, Convert.ToInt32(HttpContext.Current.Session["UserId"]));
            adapter.AddInParameter(cmd, "@compo", DbType.String, Convert.ToString(HttpContext.Current.Session["Compo"]));
            adapter.AddInParameter(cmd, "@maxCount", DbType.Int32, _maxResults);

            adapter.AddInParameter(cmd, "@aor", DbType.String, AOR != string.Empty ? (object)AOR : DBNull.Value);
            adapter.AddInParameter(cmd, "@tricare", DbType.String, Tricare != string.Empty ? (object)Tricare : DBNull.Value);

            if ((scriptName == "form2173_GroupSearch") || (scriptName == "form2173_Search"))
            {
                adapter.AddInParameter(cmd, "@workflow", DbType.Byte, workFlow != "0" ? (object)workFlow : DBNull.Value);
            }
            adapter.ExecuteReader(rowReader, cmd);

            return this;
        }

        private void ResultReader(SqlDataStore adapter, IDataReader reader)
        {
            AppSearch listSearch = new AppSearch();

            listSearch.refID = adapter.GetInt32(reader, 0);
            listSearch.recID = adapter.GetInt32(reader, 1);
            listSearch.parentID = adapter.GetInt32(reader, 2);
            listSearch.CaseId = adapter.GetString(reader, 3);
            listSearch.Name = adapter.GetString(reader, 4);
            listSearch.Compo = adapter.GetString(reader, 5);
            listSearch.Status = adapter.GetString(reader, 7);
            listSearch.IsFinal = adapter.GetBoolean(reader, 8);
            listSearch.WorkflowId = adapter.GetByte(reader, 9);
            listSearch.Workflow = adapter.GetString(reader, 10);
            listSearch.ModuleId = adapter.GetByte(reader, 11);
            listSearch.IsFormal = adapter.GetByte(reader, 12);
            listSearch.Uic = "";
            listSearch.Region = "";
            listSearch.DateCreated = adapter.GetDateTime(reader, 13, DateTime.Now);
            listSearch.CanView = adapter.GetBoolean(reader, 14);
            listSearch.CanEdit = adapter.GetBoolean(reader, 15);

            _search.Add(listSearch);
        }

        #region IList

        public int Count
        {
            get { return _search.Count; }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        public AppSearch this[int index]
        {
            get { return _search[index]; }
            set { _search[index] = value; }
        }

        public void Add(AppSearch item)
        {
            _search.Add(item);
        }

        public void Clear()
        {
            _search.Clear();
        }

        public bool Contains(AppSearch item)
        {
            return _search.Contains(item);
        }

        public void CopyTo(AppSearch[] array, int arrayIndex)
        {
            _search.CopyTo(array, arrayIndex);
        }

        public IEnumerator<AppSearch> GetEnumerator()
        {
            return _search.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _search.GetEnumerator();
        }

        public int IndexOf(AppSearch item)
        {
            return _search.IndexOf(item);
        }

        public void Insert(int index, AppSearch item)
        {
            _search.Insert(index, item);
        }

        public bool Remove(AppSearch item)
        {
            return _search.Remove(item);
        }

        public void RemoveAt(int index)
        {
            _search.RemoveAt(index);
        }

        #endregion
    }
}
