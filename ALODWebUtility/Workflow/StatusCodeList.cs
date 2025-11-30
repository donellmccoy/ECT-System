using System;
using System.Collections.Generic;
using System.Web;
using ALOD.Core.Domain.Workflow;
using ALOD.Data;
using System.Data;

namespace ALODWebUtility.Workflow
{
    public class StatusCodeList : System.Web.UI.Page, IList<StatusCode>
    {
        protected SqlDataStore _adapter;
        protected List<StatusCode> _codes = new List<StatusCode>();

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

        public void DeleteStatusCode(int statusId)
        {
            DataStore.ExecuteNonQuery("core_workflow_sp_DeleteStatusCode", statusId);
        }

        public StatusCode Find(byte statusId)
        {
            foreach (StatusCode code in _codes)
            {
                if (code.StatusId == statusId)
                {
                    return code;
                }
            }
            return null;
        }

        public StatusCodeList GetByCompo(string compo)
        {
            DataStore.ExecuteReader(CodeReader, "core_workflow_sp_GetStatusCodesByCompo", compo);
            return this;
        }

        public StatusCodeList GetByCompoAndModule(string compo, byte type)
        {
            DataStore.ExecuteReader(CodeReader, "core_workflow_sp_GetStatusCodesByCompoAndModule", compo, type);
            return this;
        }

        public DataSets.StatusCodeDataTable GetByCompoAndModuleAsDataSet(string compo, byte type)
        {
            return this.GetByCompoAndModule(compo, type).ToDataSet();
        }

        public StatusCodeList GetByCompoAndModuleWithGroup(string compo, byte type)
        {
            DataStore.ExecuteReader(CodeReader, "core_workflow_sp_GetStatusCodesByCompoAndModuleWithGroup", compo, type);
            return this;
        }

        public DataSets.StatusCodeDataTable GetByCompoAsDataSet(string compo)
        {
            return GetByCompo(compo).ToDataSet();
        }

        public StatusCodeList GetByCompoForPDHRAandLOD(string compo, bool justPdhra = false)
        {
            DataStore.ExecuteReader(CodeReader, "core_workflow_sp_GetStatusCodesByCompoForPDHRAandLOD", compo, justPdhra);
            return this;
        }

        public StatusCodeList GetBySignCode(short groupId, byte moduleId)
        {
            DataStore.ExecuteReader(CodeReaderShort, "core_workflow_sp_GetStatusCodesBySignCode", groupId, moduleId);
            return this;
        }

        public StatusCodeList GetByWorkflow(short workflow)
        {
            DataStore.ExecuteReader(CodeReader, "core_workflow_sp_GetStatusCodesByWorkflow", workflow);
            return this;
        }

        public DataSets.StatusCodeDataTable GetByWorkflowDataSet(short workflow)
        {
            return GetByWorkflow(workflow).ToDataSet();
        }

        public DataSets.StatusCodeDataTable ToDataSet()
        {
            DataSets.StatusCodeDataTable data = new DataSets.StatusCodeDataTable();
            DataSets.StatusCodeRow row;

            foreach (StatusCode code in _codes)
            {
                row = data.NewStatusCodeRow();
                code.ToDataRow(ref row);
                data.Rows.Add(row);
            }

            return data;
        }

        public void UpdateStatusCode(int statusId, string description, ModuleType moduleId, byte groupId, bool isFinal, bool isApproved, bool canAppeal, byte displayOrder, bool isDisposition, bool isFormal)
        {
            if (moduleId == 0)
            {
                moduleId = (ModuleType)Session["moduleId"];
            }
            DataStore.ExecuteNonQuery("core_workflow_sp_UpdateStatusCode",
                statusId, description, isFinal, isApproved, canAppeal, groupId != 0 ? (object)groupId : DBNull.Value, moduleId, displayOrder, isDisposition, isFormal);
        }

        protected void CodeReader(SqlDataStore adapter, IDataReader reader)
        {
            // 0-statusId, 1-description, 2-moduleId, 3-compo,
            // 4-groupId, 5-isFinal, 6-isApproved, 7-canAppeal, 8-groupName
            // 9-moduleName, 10-compoDescr, 11-fullDescription, 12-DisplayOrder
            // 13-isDisposition, 14-isFormal
            StatusCode code = new StatusCode();

            code.StatusId = adapter.GetInt32(reader, 0);
            code.Description = adapter.GetString(reader, 1);
            code.ModuleId = (ModuleType)adapter.GetByte(reader, 2);
            code.Compo = adapter.GetString(reader, 3);
            code.GroupId = adapter.GetByte(reader, 4);
            code.IsFinal = adapter.GetBoolean(reader, 5);
            code.IsApproved = adapter.GetBoolean(reader, 6);
            code.CanAppeal = adapter.GetBoolean(reader, 7);
            code.GroupName = adapter.GetString(reader, 8);
            code.ModuleName = adapter.GetString(reader, 9);
            code.CompoDescr = adapter.GetString(reader, 10);
            code.FullDescription = adapter.GetString(reader, 11);
            code.DisplayOrder = adapter.GetByte(reader, 12);
            code.IsDisposition = adapter.GetBoolean(reader, 13);
            code.IsFormal = adapter.GetBoolean(reader, 14);

            _codes.Add(code);
        }

        protected void CodeReaderShort(SqlDataStore adapter, IDataReader reader)
        {
            StatusCode code = new StatusCode();
            code.StatusId = adapter.GetInt32(reader, 0);
            code.Description = adapter.GetString(reader, 1);
            _codes.Add(code);
        }

        #region IList

        public int Count
        {
            get { return _codes.Count; }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        public StatusCode this[int index]
        {
            get
            {
                foreach (StatusCode code in _codes)
                {
                    if (code.StatusId == index)
                    {
                        return code;
                    }
                }
                return null;
            }
            set
            {
                for (int i = 0; i < _codes.Count; i++)
                {
                    if (_codes[i].StatusId == index)
                    {
                        _codes[i] = value;
                        break;
                    }
                }
            }
        }

        public void Add(StatusCode item)
        {
            _codes.Add(item);
        }

        public void Clear()
        {
            _codes.Clear();
        }

        public bool Contains(StatusCode item)
        {
            return _codes.Contains(item);
        }

        public void CopyTo(StatusCode[] array, int arrayIndex)
        {
            _codes.CopyTo(array, arrayIndex);
        }

        public IEnumerator<StatusCode> GetEnumerator()
        {
            return _codes.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return _codes.GetEnumerator();
        }

        public int IndexOf(StatusCode item)
        {
            return _codes.IndexOf(item);
        }

        public void Insert(int index, StatusCode item)
        {
            _codes.Insert(index, item);
        }

        public bool Remove(StatusCode item)
        {
            return _codes.Remove(item);
        }

        public void RemoveAt(int index)
        {
            _codes.RemoveAt(index);
        }

        #endregion
    }
}
