using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using ALOD.Data;
using ALOD.Core.Domain.Workflow;

namespace ALODWebUtility.Workflow
{
    [Serializable]
    public class WorkflowActionList : IList<WorkflowAction>
    {
        protected List<WorkflowAction> _list = new List<WorkflowAction>();
        protected short _stepId;

        public WorkflowActionList()
        {
            _stepId = 0;
        }

        public WorkflowActionList(short stepId)
        {
            _stepId = stepId;
            GetActions();
        }

        public short StepId
        {
            get { return _stepId; }
            set { _stepId = value; }
        }

        public static void CopyAction(int fromId, int toId)
        {
            SqlDataStore store = new SqlDataStore();
            store.ExecuteNonQuery("core_workflow_sp_CopyActions", fromId, toId);
        }

        public void DeleteAction(short id)
        {
            SqlDataStore adapter = new SqlDataStore();
            DbCommand cmd = adapter.GetSqlStringCommand(
            "DELETE FROM core_WorkStatus_Actions WHERE wsa_id = @actionId");
            adapter.AddInParameter(cmd, "@actionId", ALOD.Data.DbType.Int32, (int)id);
            adapter.ExecuteNonQuery(cmd);
        }

        public WorkflowActionList GetActions(int stepId)
        {
            _stepId = (short)stepId;
            return GetActions();
        }

        public WorkflowActionList GetActions()
        {
            _list.Clear();
            SqlDataStore adapter = new SqlDataStore();
            adapter.ExecuteReader(ActionReader, "core_workflow_sp_GetActionsByStep", _stepId);
            return this;
        }

        protected void ActionReader(SqlDataStore adapter, IDataReader reader)
        {
            // 0-stepId, 1-actionId, 2-type, 3-target, 4-data, 5-text
            WorkflowAction action = new WorkflowAction();

            action.StepId = _stepId;
            action.Id = (short)adapter.GetInteger(reader, 1);
            action.Type = (WorkflowActionType)adapter.GetByte(reader, 2);
            action.Target = adapter.GetInteger(reader, 3);
            action.Data = adapter.GetInteger(reader, 4);
            action.Text = adapter.GetString(reader, 5);

            _list.Add(action);
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

        public WorkflowAction this[int index]
        {
            get { return _list[index]; }
            set { _list[index] = value; }
        }

        public void Add(WorkflowAction item)
        {
            _list.Add(item);
        }

        public void Clear()
        {
            _list.Clear();
        }

        public bool Contains(WorkflowAction item)
        {
            return _list.Contains(item);
        }

        public void CopyTo(WorkflowAction[] array, int arrayIndex)
        {
            _list.CopyTo(array, arrayIndex);
        }

        public IEnumerator<WorkflowAction> GetEnumerator()
        {
            return _list.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return _list.GetEnumerator();
        }

        public int IndexOf(WorkflowAction item)
        {
            return _list.IndexOf(item);
        }

        public void Insert(int index, WorkflowAction item)
        {
            _list.Insert(index, item);
        }

        public bool Remove(WorkflowAction item)
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
