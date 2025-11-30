using System;
using System.Collections.Generic;
using System.Data;
using ALOD.Data;

namespace ALODWebUtility.Workflow
{
    public class WorkStatusOptionList : ICollection<WorkStatusOption>
    {
        protected List<WorkStatusOption> _list = new List<WorkStatusOption>();

        public ICollection<WorkStatusOption> GetByWorkflow(byte workflow, int compo)
        {
            SqlDataStore adapter = new SqlDataStore();
            adapter.ExecuteReader(OptionReader, "core_workstatus_sp_GetOptionsByWorkflow", workflow, compo);
            return _list;
        }

        public ICollection<WorkStatusOption> GetByWorkflowAll(byte workflow)
        {
            SqlDataStore adapter = new SqlDataStore();
            adapter.ExecuteReader(OptionReader, "core_workstatus_sp_GetOptionsByWorkflowAll", workflow);
            return _list;
        }

        public ICollection<WorkStatusOption> GetByWorkStatus(int workStatus, int compo)
        {
            SqlDataStore adapter = new SqlDataStore();
            adapter.ExecuteReader(OptionReader, "core_workstatus_sp_GetOptionsByWorkStatus", workStatus, compo);
            return _list;
        }

        protected void OptionReader(SqlDataStore adapter, IDataReader reader)
        {
            // 0-wso_id, 1-ws_id, 2-statusId, 3-displayText, 4-active, 5-groupId, 6-name, 7-sortOrder, 8-template

            WorkStatusOption opt = new WorkStatusOption();
            opt.Id = adapter.GetInteger(reader, 0);
            opt.WorkStatusId = adapter.GetInteger(reader, 1);
            opt.StatusOut = adapter.GetInteger(reader, 2);
            opt.Text = adapter.GetString(reader, 3);
            opt.Active = adapter.GetBoolean(reader, 4);
            opt.GroupId = adapter.GetByte(reader, 5);
            opt.GroupName = adapter.GetString(reader, 6);
            opt.SortOrder = adapter.GetByte(reader, 7);
            opt.DBSignTemplate = adapter.GetByte(reader, 8);
            opt.Valid = true;
            opt.OptionVisible = true;
            opt.StatusOutText = adapter.GetString(reader, 9);
            opt.Compo = adapter.GetInteger(reader, 10);

            _list.Add(opt);
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

        public void Add(WorkStatusOption item)
        {
            _list.Add(item);
        }

        public void Clear()
        {
            _list.Clear();
        }

        public bool Contains(WorkStatusOption item)
        {
            return _list.Contains(item);
        }

        public void CopyTo(WorkStatusOption[] array, int arrayIndex)
        {
            _list.CopyTo(array, arrayIndex);
        }

        public IEnumerator<WorkStatusOption> GetEnumerator()
        {
            return _list.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return _list.GetEnumerator();
        }

        public bool Remove(WorkStatusOption item)
        {
            return _list.Remove(item);
        }

        #endregion
    }
}
