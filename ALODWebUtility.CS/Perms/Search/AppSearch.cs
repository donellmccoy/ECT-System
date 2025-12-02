using System;
using ALOD.Core.Domain.Workflow;
using ALODWebUtility.DataAccess;


namespace ALODWebUtility.Perms.Search
{
    public class AppSearch
    {
        private ModuleType _baseType;
        private bool _canEdit;
        private bool _canView;
        private string _caseId;
        private string _compo;
        private DateTime _dateCreated;
        private DateTime _dateFinal;
        private DateTime _dateReceived;

        // used by appeals
        private bool _isFinal;

        private byte _isFormal;
        private string _name;
        private int _parentId;
        private int _recId;
        private int _refId;
        private string _region;
        private int _regionId;
        private bool _returned;
        private string _ssn;
        private string _status;
        private int _statusId;
        private ModuleType _type;
        private string _uic;
        private string _workflow;
        private short _workflowId;

        public AppSearch()
        {
            _caseId = string.Empty;
            _workflow = string.Empty;
            _workflowId = 0;
            _name = string.Empty;
            _uic = string.Empty;
            _region = string.Empty;
            _statusId = 0;
            _status = string.Empty;
            _ssn = string.Empty;
            _type = 0;
            _recId = 0;
            _refId = 0;
            _parentId = 0;
            _compo = string.Empty;
            _canView = false;
            _canEdit = false;
            _isFinal = false;
        }

        public ModuleType BaseType { get => _baseType; set => _baseType = value; }
        public bool CanEdit { get => _canEdit; set => _canEdit = value; }
        public bool CanView { get => _canView; set => _canView = value; }
        public string CaseId { get => _caseId; set => _caseId = value; }
        public string Compo { get => this._compo; set => this._compo = value; }
        public DateTime DateCreated { get => _dateCreated; set => _dateCreated = value; }
        public DateTime DateFinal { get => _dateFinal; set => _dateFinal = value; }
        public DateTime DateReceived { get => _dateReceived; set => _dateReceived = value; }
        public bool IsFinal { get => _isFinal; set => _isFinal = value; }
        public byte IsFormal { get => _isFormal; set => _isFormal = value; }
        public byte ModuleId { get => (byte)this._type; set => this._type = (ModuleType)value; }
        public string Name { get => this._name; set => this._name = value; }

        public bool Overdue
        {
            get
            {
                TimeSpan elapsed = DateTime.Now.Subtract(_dateCreated);

                switch (_type)
                {
                    case ModuleType.LOD:
                        if (!_isFinal)
                        {
                            if (_isFormal != 0)
                            {
                                return (elapsed.TotalDays > 70);
                            }
                            else
                            {
                                return (elapsed.TotalDays > 45);
                            }
                        }
                        break;
                    default:
                        return false;
                }
                return false;
            }
        }

        public int parentID { get => this._parentId; set => this._parentId = value; }
        public int recID { get => this._recId; set => this._recId = value; }
        public int refID { get => this._refId; set => this._refId = value; }
        public string Region { get => this._region; set => this._region = value; }
        public int RegionId { get => _regionId; set => _regionId = value; }
        public bool Returned { get => _returned; set => _returned = value; }
        public string SSN { get => this._ssn; set => this._ssn = value; }
        public string Status { get => this._status; set => this._status = value; }
        public int StatusId { get => this._statusId; set => this._statusId = value; }
        public string Uic { get => this._uic; set => this._uic = value; }
        public string Workflow { get => this._workflow; set => this._workflow = value; }
        public short WorkflowId { get => _workflowId; set => _workflowId = value; }

        public void ToDataRow(ref DataSets.SearchResultRow row)
        {
            row.caseId = _caseId;
            row.recId = _recId;
            row.parentId = _parentId;
            row.workflow = _workflow;
            row.workflowId = (byte)_workflowId;
            row.name = _name;
            row.uic = _uic;
            row.regions = _region;
            row.status = _status;
            row.refID = _refId;
            row.compo = _compo;
            row.canEdit = _canEdit;
            row.canView = _canView;
            row.moduleId = (byte)_type;
            row.dateFinal = _dateFinal;
            row.baseType = (byte)_baseType;
            row.baseType = (byte)_baseType;
            row.dateCreated = DateCreated;
            row.returned = _returned;
            row.overdue = Overdue;
            row.isFormal = _isFormal != 0;
            row.dateReceived = _dateReceived;
            row.isFinal = _isFinal;
        }
    }
}
