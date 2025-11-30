using System;
using System.Data.Common;
using ALOD.Data;
using ALODWebUtility.PageTitles;

namespace ALODWebUtility.Workflow
{
    public class WorkflowView : PageTitle
    {
        #region members and properties

        private int _workflowId = 0;
        private string _workflowTitle = string.Empty;

        public int WorkflowId
        {
            get { return _workflowId; }
            set { _workflowId = value; }
        }

        public string WorkflowTitle
        {
            get { return _workflowTitle; }
            set { _workflowTitle = value; }
        }

        #endregion

        public WorkflowView()
        {
        }

        public void InsertWorkflowView()
        {
            SqlDataStore adapter = new SqlDataStore();
            DbCommand cmd = adapter.GetStoredProcCommand("core_workflowView_sp_Insert", _pageId, _workflowId);
            adapter.ExecuteNonQuery(cmd);
        }
    }
}
