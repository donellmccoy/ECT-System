using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using ALOD.Core.Domain.Workflow;
using ALOD.Data;

namespace ALODWebUtility.Workflow
{
    public class WorkFlowList : IList<Workflow>
    {
        protected List<Workflow> _workflows = new List<Workflow>();
        SqlDataStore _adapter;

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

        public void DeleteWorkflow(byte id)
        {
            DbCommand cmd = Adapter.GetSqlStringCommand(
                "DELETE FROM core_Workflow WHERE workflowId = @workflowId");
            Adapter.AddInParameter(cmd, "@workflowId", System.Data.DbType.Byte, id);
            Adapter.ExecuteNonQuery(cmd);
        }

        public Workflow Find(int id)
        {
            foreach (Workflow flow in _workflows)
            {
                if (flow.Id == id)
                {
                    return flow;
                }
            }
            return null;
        }

        public IList<Workflow> GetAll()
        {
            DbCommand cmd = Adapter.GetSqlStringCommand(
                "SELECT workFlowId, a.compo, title, formal, a.moduleId, active, initialStatus, b.description FROM core_Workflow a LEFT JOIN core_StatusCodes b ON b.statusId = a.initialStatus");
            Adapter.ExecuteReader(WorkFlowReader, cmd);
            return this;
        }

        public IList<Workflow> GetByAllowedCreate(string compo, ModuleType type, byte groupId)
        {
            Adapter.ExecuteReader(WorkFlowReader, "core_workflow_sp_GetCreatableByGroup", compo, (int)type, groupId);
            return this;
        }

        public WorkFlowList GetByAllowedToView(short groupId, ModuleType type)
        {
            Adapter.ExecuteReader(WorkFlowReader, "core_workflow_sp_GetViewableByGroup", groupId, (int)type);
            return this;
        }

        public IList<Workflow> GetByCompo(string compo)
        {
            DbCommand cmd = Adapter.GetSqlStringCommand(
                "SELECT workFlowId, a.compo, title, formal, a.moduleId, active, initialStatus, b.description FROM core_Workflow a LEFT JOIN core_StatusCodes b ON b.statusId = a.initialStatus WHERE a.compo = @compo");
            Adapter.AddInParameter(cmd, "@compo", System.Data.DbType.String, compo);
            Adapter.ExecuteReader(WorkFlowReader, cmd);
            return this;
        }

        public IList<Workflow> GetByCompoAndModule(string compo, ModuleType type)
        {
            DbCommand cmd = Adapter.GetSqlStringCommand(
                "SELECT a.workFlowId, a.compo, title, formal, a.moduleId, active, initialStatus, c.description FROM core_Workflow a LEFT JOIN core_WorkStatus b ON b.ws_id = a.initialStatus LEFT JOIN core_StatusCodes c ON c.statusId = b.statusId WHERE a.compo = @compo AND a.moduleId = @module");
            Adapter.AddInParameter(cmd, "@compo", System.Data.DbType.String, compo);
            Adapter.AddInParameter(cmd, "@module", System.Data.DbType.Byte, (byte)type);
            Adapter.ExecuteReader(WorkFlowReader, cmd);
            return this;
        }

        public IList<Workflow> GetByCompoForLODandPDHRA(string compo)
        {
            string sql = "SELECT workFlowId, a.compo, title, formal, a.moduleId, active, initialStatus, b.description FROM core_Workflow a LEFT JOIN core_StatusCodes b ON b.statusId = a.initialStatus WHERE a.compo = @compo AND a.moduleId IN(2,8)";
            DbCommand cmd = Adapter.GetSqlStringCommand(sql);
            Adapter.AddInParameter(cmd, "@compo", System.Data.DbType.String, compo);
            Adapter.ExecuteReader(WorkFlowReader, cmd);
            return this;
        }

        public IList<Workflow> GetByCompoForLODandPDHRA(string compo, bool restricted)
        {
            string sql = "SELECT workFlowId, a.compo, title, formal, a.moduleId, active, initialStatus, b.description FROM core_Workflow a LEFT JOIN core_StatusCodes b ON b.statusId = a.initialStatus WHERE a.compo = @compo AND a.moduleId IN(2,8)";
            if (!restricted)
            {
                sql += " AND workflowid NOT IN(9,17)";
            }
            DbCommand cmd = Adapter.GetSqlStringCommand(sql);
            Adapter.AddInParameter(cmd, "@compo", System.Data.DbType.String, compo);
            Adapter.ExecuteReader(WorkFlowReader, cmd);
            return this;
        }

        public IList<Workflow> GetByCompoForPDHRA(string compo)
        {
            string sql = "SELECT workFlowId, a.compo, title, formal, a.moduleId, active, initialStatus, b.description FROM core_Workflow a LEFT JOIN core_StatusCodes b ON b.statusId = a.initialStatus WHERE a.compo = @compo AND a.moduleId IN(8)";

            DbCommand cmd = Adapter.GetSqlStringCommand(sql);
            Adapter.AddInParameter(cmd, "@compo", System.Data.DbType.String, compo);
            Adapter.ExecuteReader(WorkFlowReader, cmd);
            return this;
        }

        public void UpdateWorkflow(string title, byte moduleId, bool isFormal, byte id, bool active, int initialStatus)
        {
            Adapter.ExecuteNonQuery("core_workflow_sp_UpdateWorkflow", id, title, moduleId, isFormal, active, initialStatus);
        }

        protected void WorkFlowReader(SqlDataStore adapter, IDataReader reader)
        {
            // 0-workflowid, 1-compo, 2-title, 3-formal, 4-moduleId, 5-active, 6-initialStatus, 7-statusDesc
            Workflow flow = new Workflow();

            flow.Id = adapter.GetByte(reader, 0);
            flow.Compo = adapter.GetString(reader, 1);
            flow.Title = adapter.GetString(reader, 2);
            flow.IsFormal = adapter.GetBoolean(reader, 3);
            flow.ModuleId = adapter.GetByte(reader, 4);
            flow.Active = adapter.GetBoolean(reader, 5);
            flow.InitialStatus = adapter.GetInt32(reader, 6);
            flow.StatusDescription = adapter.GetString(reader, 7);

            _workflows.Add(flow);
        }

        #region IList

        public int Count
        {
            get { return _workflows.Count; }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        public Workflow this[int id]
        {
            get
            {
                foreach (Workflow flow in _workflows)
                {
                    if (flow.Id == id)
                    {
                        return flow;
                    }
                }
                return null;
            }
            set
            {
                for (int i = 0; i < _workflows.Count; i++)
                {
                    if (_workflows[i].Id == id)
                    {
                        _workflows[i] = value;
                        break;
                    }
                }
            }
        }

        public void Add(Workflow item)
        {
            _workflows.Add(item);
        }

        public void Clear()
        {
            _workflows.Clear();
        }

        public bool Contains(Workflow item)
        {
            return _workflows.Contains(item);
        }

        public void CopyTo(Workflow[] array, int arrayIndex)
        {
            _workflows.CopyTo(array, arrayIndex);
        }

        public IEnumerator<Workflow> GetEnumerator()
        {
            return _workflows.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return _workflows.GetEnumerator();
        }

        public int IndexOf(Workflow item)
        {
            return _workflows.IndexOf(item);
        }

        public void Insert(int index, Workflow item)
        {
            _workflows.Insert(index, item);
        }

        public bool Remove(Workflow item)
        {
            return _workflows.Remove(item);
        }

        public void RemoveAt(int index)
        {
            _workflows.RemoveAt(index);
        }

        #endregion
    }
}
