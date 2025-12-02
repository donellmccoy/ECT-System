using System;
using ALOD.Core.Domain.Workflow;
using ALOD.Data;
using System.Data;
using ALODWebUtility.DataAccess;


namespace ALODWebUtility.Workflow
{
    [Serializable]
    public class StatusCode
    {
        protected byte _accessScope = 0;
        protected bool _canAppeal;
        protected string _compo = string.Empty;
        protected string _compoDescr = string.Empty;
        protected string _description = string.Empty;
        protected byte _displayOrder = 0;
        protected string _fullDescr = string.Empty;
        protected byte _groupId;
        protected string _groupName = string.Empty;
        protected bool _isApproved;
        protected bool _isDisposition;
        protected bool _isFinal;
        protected bool _isFormal;
        protected ModuleType _moduleId;
        protected string _moduleName = string.Empty;
        protected int _statusId = 0;

        public StatusCode()
        {
        }

        public byte AccessScope
        {
            get { return _accessScope; }
            set { _accessScope = value; }
        }

        public bool CanAppeal
        {
            get { return _canAppeal; }
            set { _canAppeal = value; }
        }

        public string Compo
        {
            get { return _compo; }
            set { _compo = value; }
        }

        public string CompoDescr
        {
            get { return _compoDescr; }
            set { _compoDescr = value; }
        }

        public string Description
        {
            get { return _description; }
            set { _description = value; }
        }

        public byte DisplayOrder
        {
            get { return _displayOrder; }
            set { _displayOrder = value; }
        }

        public string FullDescription
        {
            get { return _fullDescr; }
            set { _fullDescr = value; }
        }

        public byte GroupId
        {
            get { return _groupId; }
            set { _groupId = value; }
        }

        public string GroupName
        {
            get { return _groupName; }
            set { _groupName = value; }
        }

        public bool IsApproved
        {
            get { return _isApproved; }
            set { _isApproved = value; }
        }

        public bool IsDisposition
        {
            get { return _isDisposition; }
            set { _isDisposition = value; }
        }

        public bool IsFinal
        {
            get { return _isFinal; }
            set { _isFinal = value; }
        }

        public bool IsFormal
        {
            get { return _isFormal; }
            set { _isFormal = value; }
        }

        public ModuleType ModuleId
        {
            get { return _moduleId; }
            set { _moduleId = value; }
        }

        public string ModuleName
        {
            get { return _moduleName; }
            set { _moduleName = value; }
        }

        public int StatusId
        {
            get { return _statusId; }
            set { _statusId = value; }
        }

        public bool Insert()
        {
            if (_description.Trim().Length == 0 || _compo.Trim().Length == 0 || _moduleId == 0)
            {
                return false;
            }

            SqlDataStore adapter = new SqlDataStore();
            _statusId = Convert.ToInt32(adapter.ExecuteScalar("core_workflow_sp_InsertStatusCode", _description, _isFinal, _isApproved, _canAppeal, _groupId != 0 ? (object)_groupId : DBNull.Value, _moduleId, _compo, _isDisposition, _isFormal));

            return _statusId > 0;
        }

        public void SetAccessScope()
        {
            SqlDataStore adapter = new SqlDataStore();
            _accessScope = Convert.ToByte(adapter.ExecuteScalar("core_workflow_sp_GetStatusCodeScope", _statusId));
        }

        public void ToDataRow(ref DataSets.StatusCodeRow row)
        {
            row.canAppeal = _canAppeal;
            row.compo = _compo;
            row.description = _description;
            row.groupId = _groupId;
            row.isApproved = _isApproved;
            row.isFinal = _isFinal;
            row.moduleId = (byte)_moduleId;
            row.statusId = _statusId;
            row.groupName = _groupName;
            row.moduleName = _moduleName;
            row.compoDescr = _compoDescr;
            row.FullDescription = _fullDescr;
            row.displayOrder = _displayOrder;
            row.isDisposition = _isDisposition;
            row.isFormal = _isFormal.ToString();
        }
    }
}
