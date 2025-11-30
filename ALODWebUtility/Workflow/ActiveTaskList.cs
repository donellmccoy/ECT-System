using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Web;
using ALOD.Data;

namespace ALODWebUtility.Workflow
{
    public class ActiveTaskList : IList<ActiveTask>
    {
        private List<ActiveTask> _taskList;

        public ActiveTaskList()
        {
            _taskList = new List<ActiveTask>();
        }

        public ActiveTaskList GetActivities(string workflow)
        {
            SqlDataStore adapter = new SqlDataStore();
            DbCommand cmd;

            cmd = adapter.GetStoredProcCommand("form2173_sp_GetActivityByRegion");
            adapter.AddInParameter(cmd, "@workflow", ALOD.Data.DbType.String, workflow);
            adapter.AddInParameter(cmd, "@userId", ALOD.Data.DbType.Int32, Convert.ToInt32(HttpContext.Current.Session["UserId"]));
            adapter.AddInParameter(cmd, "@compo", ALOD.Data.DbType.String, Convert.ToString(HttpContext.Current.Session["Compo"]));
            adapter.ExecuteReader(ActivityListReader, cmd);

            return this;
        }

        public ActiveTaskList GetNGBAors(string workflow)
        {
            SqlDataStore adapter = new SqlDataStore();
            DbCommand cmd;
            cmd = adapter.GetStoredProcCommand("core_workflow_sp_GetAORActivity");
            adapter.AddInParameter(cmd, "@compo", ALOD.Data.DbType.String, Convert.ToString(HttpContext.Current.Session["Compo"]));
            adapter.AddInParameter(cmd, "@workflow", ALOD.Data.DbType.String, workflow);
            adapter.AddInParameter(cmd, "@userId", ALOD.Data.DbType.Int32, Convert.ToInt32(HttpContext.Current.Session["UserId"]));
            adapter.ExecuteReader(StatusByAorReader, cmd);
            return this;
        }

        public ActiveTaskList GetWorkflowTitles()
        {
            SqlDataStore adapter = new SqlDataStore();
            DbCommand cmd = adapter.GetStoredProcCommand("core_workflow_sp_GetWorkflowByCompo");
            adapter.AddInParameter(cmd, "@compo", ALOD.Data.DbType.String, Convert.ToString(HttpContext.Current.Session["Compo"]));
            adapter.AddInParameter(cmd, "@userId", ALOD.Data.DbType.Int32, Convert.ToInt32(HttpContext.Current.Session["UserId"]));
            adapter.ExecuteReader(WorkflowListReader, cmd);
            return this;
        }

        private void ActivityListReader(SqlDataStore adapter, IDataReader reader)
        {
            ActiveTask activeTask = new ActiveTask();

            activeTask.ActiveTitle = adapter.GetString(reader, 0);
            activeTask.OverdueCount = adapter.GetInt32(reader, 1);
            activeTask.FormalCount = adapter.GetInt32(reader, 2);
            activeTask.InformalCount = adapter.GetInt32(reader, 3);
            activeTask.StatusID = adapter.GetByte(reader, 4);
            _taskList.Add(activeTask);
        }

        private void StatusByAorReader(SqlDataStore adapter, IDataReader reader)
        {
            ActiveTask statusByAorReader = new ActiveTask();
            statusByAorReader.ActiveTitle = adapter.GetString(reader, 0);
            statusByAorReader.RegionId = adapter.GetByte(reader, 2);
            statusByAorReader.OverdueCount = adapter.GetInt32(reader, 3);
            statusByAorReader.FormalCount = adapter.GetInt32(reader, 4);
            statusByAorReader.InformalCount = adapter.GetInt32(reader, 5);
            _taskList.Add(statusByAorReader);
        }

        private void WorkflowListReader(SqlDataStore adapter, IDataReader reader)
        {
            ActiveTask workflowListReader = new ActiveTask();
            workflowListReader.WorkflowId = adapter.GetByte(reader, 0);
            workflowListReader.ActiveTitle = adapter.GetString(reader, 1);
            _taskList.Add(workflowListReader);
        }

        #region IList

        public int Count
        {
            get { return _taskList.Count; }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        public ActiveTask this[int index]
        {
            get { return _taskList[index]; }
            set { _taskList[index] = value; }
        }

        public void Add(ActiveTask item)
        {
            _taskList.Add(item);
        }

        public void Clear()
        {
            _taskList.Clear();
        }

        public bool Contains(ActiveTask item)
        {
            return _taskList.Contains(item);
        }

        public void CopyTo(ActiveTask[] array, int arrayIndex)
        {
            _taskList.CopyTo(array, arrayIndex);
        }

        public IEnumerator<ActiveTask> GetEnumerator()
        {
            return _taskList.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return _taskList.GetEnumerator();
        }

        public int IndexOf(ActiveTask item)
        {
            return _taskList.IndexOf(item);
        }

        public void Insert(int index, ActiveTask item)
        {
            _taskList.Insert(index, item);
        }

        public bool Remove(ActiveTask item)
        {
            return _taskList.Remove(item);
        }

        public void RemoveAt(int index)
        {
            _taskList.RemoveAt(index);
        }

        #endregion
    }
}
