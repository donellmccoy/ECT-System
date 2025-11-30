using System;
using System.Data;
using System.Data.Common;
using ALOD.Core.Domain.DBSign;
using ALOD.Data;

namespace ALODWebUtility.Workflow
{
    [Serializable]
    public class WorkflowStep
    {
        #region Members/Properties

        private const string _sep = ";";
        private int _actionCount = 0;
        private bool _active;
        SqlDataStore _adapter;
        private DBSignTemplateId _dbSignTemplate;
        private char _deathStatus;
        private byte _displayOrder = 0;
        private string _groupInDescr = string.Empty;
        private byte _groupInId;
        private string _groupOutDescr = string.Empty;
        private byte _groupOutId;
        private short _id = 0;
        private byte _memoTemplate = 0;
        private string _memoTemplateName = string.Empty;
        private int _statusIn = 0;
        private string _statusInDescr = string.Empty;
        private int _statusOut = 0;
        private string _statusOutDescr = string.Empty;
        private string _text = string.Empty;
        private byte _workflow = 0;
        private string _workflowDescr = string.Empty;

        public int ActionCount
        {
            get { return _actionCount; }
            set { _actionCount = value; }
        }

        public bool Active
        {
            get { return _active; }
            set { _active = value; }
        }

        public DBSignTemplateId DBSignTemplate
        {
            get { return _dbSignTemplate; }
            set { _dbSignTemplate = value; }
        }

        public char DeathStatus
        {
            get { return _deathStatus; }
            set { _deathStatus = value; }
        }

        public byte DisplayOrder
        {
            get { return _displayOrder; }
            set { _displayOrder = value; }
        }

        public string GroupInDescr
        {
            get { return _groupInDescr; }
            set { _groupInDescr = value; }
        }

        public byte GroupInId
        {
            get { return _groupInId; }
            set { _groupInId = value; }
        }

        public string GroupOutDescr
        {
            get { return _groupOutDescr; }
            set { _groupOutDescr = value; }
        }

        public byte GroupOutId
        {
            get { return _groupOutId; }
            set { _groupOutId = value; }
        }

        public short Id
        {
            get { return _id; }
            set { _id = value; }
        }

        public byte MemoTemplate
        {
            get { return _memoTemplate; }
            set { _memoTemplate = value; }
        }

        public string MemoTemplateName
        {
            get { return _memoTemplateName; }
            set { _memoTemplateName = value; }
        }

        public int StatusIn
        {
            get { return _statusIn; }
            set { _statusIn = value; }
        }

        public string StatusInDescription
        {
            get { return _statusInDescr; }
            set { _statusInDescr = value; }
        }

        public int StatusOut
        {
            get { return _statusOut; }
            set { _statusOut = value; }
        }

        public string StatusOutDescription
        {
            get { return _statusOutDescr; }
            set { _statusOutDescr = value; }
        }

        public string Text
        {
            get { return _text; }
            set { _text = value; }
        }

        public byte Workflow
        {
            get { return _workflow; }
            set { _workflow = value; }
        }

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

        #endregion

        #region Constructors

        public WorkflowStep()
        {
        }

        public WorkflowStep(string value)
        {
            string[] parts = value.Split(_sep.ToCharArray());

            if (parts.Length != 4)
            {
                return;
            }

            short.TryParse(parts[0], out _id);
            int.TryParse(parts[1], out _statusOut);
            byte tempDbSignTemplate;
            if (byte.TryParse(parts[2], out tempDbSignTemplate))
            {
                _dbSignTemplate = (DBSignTemplateId)tempDbSignTemplate;
            }
            byte.TryParse(parts[3], out _memoTemplate);
        }

        #endregion

        public void Delete()
        {
            if (_id == 0)
            {
                return;
            }

            DbCommand cmd = Adapter.GetSqlStringCommand(
                "DELETE FROM core_workflowSteps WHERE stepId = @stepId");
            Adapter.AddInParameter(cmd, "@stepId", ALOD.Data.DbType.Int16, _id);
            Adapter.ExecuteNonQuery(cmd);
        }

        public bool Save()
        {
            if (_id == 0)
            {
                return Insert();
            }
            else
            {
                return Update();
            }
        }

        public void ToDataRow(DataSets.WorkflowStepsRow row)
        {
            row.id = _id;
            row.active = _active;
            row.displayOrder = _displayOrder;
            row.statusIn = _statusIn;
            row.statusInDescription = _statusInDescr;
            row.statusOut = _statusOut;
            row.statusOutDescription = _statusOutDescr;
            row.text = _text;
            row.workflow = _workflow;
            row.workflowDescription = _workflowDescr;
            row.groupInId = _groupInId;
            row.groupInDescr = _groupInDescr;
            row.groupOutId = _groupOutId;
            row.groupOutDescr = _groupOutDescr;
            row.dbSignTemplate = (byte)_dbSignTemplate;
            row.actionCount = _actionCount;
            row.deathStatus = _deathStatus.ToString();
            row.memoTemplate = _memoTemplate;
        }

        public string ToValueString()
        {
            return ToValueString(";");
        }

        public string ToValueString(string seperator)
        {
            return _id.ToString() + seperator + _statusOut.ToString() + seperator + ((byte)_dbSignTemplate).ToString() + seperator + _memoTemplate.ToString();
        }

        protected bool Insert()
        {
            return (Convert.ToInt32(Adapter.ExecuteScalar("core_workflow_sp_InsertStep",
                _workflow, _statusIn, _statusOut, _text,
                0, _active, _displayOrder, _dbSignTemplate, _deathStatus,
                _memoTemplate != 0 ? (object)_memoTemplate : DBNull.Value)) > 0);
        }

        protected bool Update()
        {
            return (Convert.ToInt32(Adapter.ExecuteNonQuery("core_workflow_sp_UpdateStep",
                _id, _workflow, _statusIn, _statusOut, _text,
                0, _active, _displayOrder, _dbSignTemplate, _deathStatus,
                _memoTemplate != 0 ? (object)_memoTemplate : DBNull.Value)) > 0);
        }
    }
}
