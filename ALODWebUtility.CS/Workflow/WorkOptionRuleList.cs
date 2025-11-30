using System;
using System.Collections.Generic;
using System.Data;
using ALOD.Data;

namespace ALODWebUtility.Workflow
{
    public class WorkOptionRuleList : IList<WorkOptionRule>
    {
        protected List<WorkOptionRule> _list;
        SqlDataStore _adapter;

        public WorkOptionRuleList()
        {
            _list = new List<WorkOptionRule>();
        }

        protected SqlDataStore Adapter
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

        /// <summary>
        /// Get list of rules by workflowid and status in
        /// </summary>
        /// <param name="workflowId"></param>
        /// <param name="statusIn"></param>
        /// <returns></returns>
        public WorkOptionRuleList GetListOfRules(int workflowId, int statusIn)
        {
            Adapter.ExecuteReader(RuleReader, "core_workflowRules_sp_GetRules", workflowId, statusIn);
            return this;
        }

        protected void RuleReader(SqlDataStore adapter, IDataReader reader)
        {
            WorkOptionRule _rules = new WorkOptionRule();

            _rules.OptionId = adapter.GetInt32(reader, 0);
            _rules.RuleKey = adapter.GetString(reader, 1);
            _rules.RuleValue = adapter.GetString(reader, 2);
            _rules.RuleTypeID = (ALOD.Core.Utils.RuleType)adapter.GetByte(reader, 3);

            _list.Add(_rules);
        }

        #region IList

        public int Count
        {
            get { return _list.Count; }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        public WorkOptionRule this[int index]
        {
            get { return _list[index]; }
            set { _list[index] = value; }
        }

        public void Add(WorkOptionRule item)
        {
            _list.Add(item);
        }

        public void Clear()
        {
            _list.Clear();
        }

        public bool Contains(WorkOptionRule item)
        {
            return _list.Contains(item);
        }

        public void CopyTo(WorkOptionRule[] array, int arrayIndex)
        {
            _list.CopyTo(array, arrayIndex);
        }

        public IEnumerator<WorkOptionRule> GetEnumerator()
        {
            return _list.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return _list.GetEnumerator();
        }

        public int IndexOf(WorkOptionRule item)
        {
            return _list.IndexOf(item);
        }

        public void Insert(int index, WorkOptionRule item)
        {
            _list.Insert(index, item);
        }

        public bool Remove(WorkOptionRule item)
        {
            return _list.Remove(item);
        }

        public void RemoveAt(int index)
        {
            _list.RemoveAt(index);
        }

        #endregion
    }
}
