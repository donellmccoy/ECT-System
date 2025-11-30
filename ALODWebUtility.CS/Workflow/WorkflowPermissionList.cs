using System;
using System.Collections.Generic;
using System.Data;
using ALOD.Data;
using ALODWebUtility.Common;

namespace ALODWebUtility.Workflow
{
    public class WorkflowPermissionList : IList<WorkflowPermission>
    {
        protected List<WorkflowPermission> _permissions = new List<WorkflowPermission>();
        protected byte _workflowId = 0;
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

        public IList<WorkflowPermission> GetByWorkflow(byte workflowId)
        {
            _permissions.Clear();
            _workflowId = workflowId;
            Adapter.ExecuteReader(PermissionsReader, "core_Workflow_sp_GetPermissions", workflowId);
            return this;
        }

        public IList<WorkflowPermission> GetByWorkflowAndCompo(byte workflowId, string compo)
        {
            _permissions.Clear();
            _workflowId = workflowId;
            Adapter.ExecuteReader(PermissionsReader, "core_Workflow_sp_GetPermissionsByCompo", workflowId, compo);
            return this;
        }

        public bool UpdateWorkflow(byte workflowId, string compo)
        {
            _workflowId = workflowId;

            if (Count == 0)
            {
                return false;
            }

            //generate the XML for the bulk update
            XMLString xml = new XMLString("list");

            foreach (WorkflowPermission perm in _permissions)
            {
                //IIf is fine because these lines are only reached if perm is not nothing
                xml.BeginElement("item");
                xml.WriteAttribute("groupId", perm.GroupId.ToString());
                xml.WriteAttribute("canCreate", perm.CanCreate ? "1" : "0");
                xml.WriteAttribute("canView", perm.CanView ? "1" : "0");
                xml.EndElement();
            }

            //now pass it to SQL
            string st = xml.ToString();
            Adapter.ExecuteNonQuery("core_workflow_sp_UpdatePermissions", _workflowId, compo, xml.Value);
            return true;
        }

        protected void PermissionsReader(SqlDataStore adapter, IDataReader reader)
        {
            //0-groupid, 1-groupname, 2-canView, 3-canCreate,
            WorkflowPermission permission = new WorkflowPermission();
            permission.GroupId = adapter.GetByte(reader, 0);
            permission.GroupName = adapter.GetString(reader, 1);
            permission.CanView = adapter.GetBoolean(reader, 2);
            permission.CanCreate = adapter.GetBoolean(reader, 3);

            _permissions.Add(permission);
        }

        #region IList

        public int Count
        {
            get { return _permissions.Count; }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        public WorkflowPermission this[int groupId]
        {
            get
            {
                foreach (WorkflowPermission permission in _permissions)
                {
                    if (permission.GroupId == groupId)
                    {
                        return permission;
                    }
                }
                return null;
            }
            set
            {
                for (int i = 0; i < _permissions.Count; i++)
                {
                    if (_permissions[i].GroupId == groupId)
                    {
                        _permissions[i] = value;
                        break;
                    }
                }
            }
        }

        public void Add(WorkflowPermission item)
        {
            _permissions.Add(item);
        }

        public void Clear()
        {
            _permissions.Clear();
        }

        public bool Contains(WorkflowPermission item)
        {
            return _permissions.Contains(item);
        }

        public void CopyTo(WorkflowPermission[] array, int arrayIndex)
        {
            _permissions.CopyTo(array, arrayIndex);
        }

        public IEnumerator<WorkflowPermission> GetEnumerator()
        {
            return _permissions.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return _permissions.GetEnumerator();
        }

        public int IndexOf(WorkflowPermission item)
        {
            return _permissions.IndexOf(item);
        }

        public void Insert(int index, WorkflowPermission item)
        {
            _permissions.Insert(index, item);
        }

        public bool Remove(WorkflowPermission item)
        {
            return _permissions.Remove(item);
        }

        public void RemoveAt(int index)
        {
            _permissions.RemoveAt(index);
        }

        #endregion
    }
}
