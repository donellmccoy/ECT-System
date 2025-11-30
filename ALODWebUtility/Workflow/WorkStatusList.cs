using System;
using System.Collections.Generic;
using System.Data;
using ALOD.Data;

namespace ALODWebUtility.Workflow
{
    public class WorkStatusList : ICollection<WorkStatus>
    {
        protected List<WorkStatus> _list = new List<WorkStatus>();

        /// <summary>
        /// Returns all active WorkStatus codes for the given workflow
        /// </summary>
        /// <param name="workflow"></param>
        /// <returns></returns>
        public ICollection<WorkStatus> GetByWorklfow(int workflow)
        {
            SqlDataStore adapter = new SqlDataStore();
            adapter.ExecuteReader(WorkStatusReader, "core_workstatus_sp_GetByWorkflow", workflow);
            return _list;
        }

        protected void WorkStatusReader(SqlDataStore adapter, IDataReader reader)
        {
            // 0-ws_id, 1-workflowId, 2-statusId, 3-sortOrder
            // 4-descr, 5-groupId, 6-groupName

            WorkStatus status = new WorkStatus();
            status.Id = adapter.GetInt32(reader, 0);
            status.Workflow = adapter.GetByte(reader, 1);
            status.Status = adapter.GetInt32(reader, 2);
            status.SortOrder = adapter.GetByte(reader, 3);
            status.Description = adapter.GetString(reader, 4);
            status.GroupId = adapter.GetByte(reader, 5);
            status.GroupName = adapter.GetString(reader, 6);

            _list.Add(status);
        }

        #region ICollection

        public int Count
        {
            get { return _list.Count; }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        public void Add(WorkStatus item)
        {
            _list.Add(item);
        }

        public void Clear()
        {
            _list.Clear();
        }

        public bool Contains(WorkStatus item)
        {
            return _list.Contains(item);
        }

        public void CopyTo(WorkStatus[] array, int arrayIndex)
        {
            _list.CopyTo(array, arrayIndex);
        }

        public IEnumerator<WorkStatus> GetEnumerator()
        {
            return _list.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return _list.GetEnumerator();
        }

        public bool Remove(WorkStatus item)
        {
            return _list.Remove(item);
        }

        #endregion
    }
}
