using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using ALOD.Data;
using ALODWebUtility.PageTitles;
using ALODWebUtility.DataAccess;


namespace ALODWebUtility.Workflow
{
    public class WorkflowViewList : IList<WorkflowView>
    {
        SqlDataStore _adapter;
        List<WorkflowView> _workflow = new List<WorkflowView>();

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

        public void CodeReader(SqlDataStore adapter, IDataReader reader)
        {
            WorkflowView pages = new WorkflowView();

            pages.PageId = adapter.GetInt16(reader, 0);
            pages.Title = adapter.GetString(reader, 1);

            _workflow.Add(pages);
        }

        public void DeleteWorkflowView(int pageId, int workflowId)
        {
            DbCommand cmd = Adapter.GetStoredProcCommand("core_workflowView_sp_Delete", pageId, workflowId);
            Adapter.ExecuteNonQuery(cmd);
        }

        public DataSets.PageTitlesDataTable GetPagesAsDataSet(int workflowId)
        {
            return GetPagesByWorkflowId(workflowId).ToDataSet();
        }

        public DataSets.PageTitlesDataTable ToDataSet()
        {
            DataSets.PageTitlesDataTable data = new DataSets.PageTitlesDataTable();
            DataSets.PageTitlesRow row;

            foreach (WorkflowView page in _workflow)
            {
                row = data.NewPageTitlesRow();
                page.ToDataRow(row);
                data.Rows.Add(row);
            }

            return data;
        }

        private WorkflowViewList GetPagesByWorkflowId(int workflowId)
        {
            Adapter.ExecuteReader(CodeReader, "core_workflow_sp_GetPagesByWorkflowId", workflowId);
            return this;
        }

        #region IList

        public int Count
        {
            get { return _workflow.Count; }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        public WorkflowView this[int index]
        {
            get { return _workflow[index]; }
            set { _workflow[index] = value; }
        }

        public void Add(WorkflowView item)
        {
            _workflow.Add(item);
        }

        public void Clear()
        {
            _workflow.Clear();
        }

        public bool Contains(WorkflowView item)
        {
            return _workflow.Contains(item);
        }

        public void CopyTo(WorkflowView[] array, int arrayIndex)
        {
            _workflow.CopyTo(array, arrayIndex);
        }

        public IEnumerator<WorkflowView> GetEnumerator()
        {
            return _workflow.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return _workflow.GetEnumerator();
        }

        public int IndexOf(WorkflowView item)
        {
            return _workflow.IndexOf(item);
        }

        public void Insert(int index, WorkflowView item)
        {
            _workflow.Insert(index, item);
        }

        public bool Remove(WorkflowView item)
        {
            return _workflow.Remove(item);
        }

        public void RemoveAt(int index)
        {
            _workflow.RemoveAt(index);
        }

        #endregion
    }
}
