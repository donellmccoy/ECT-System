using System;
using System.Data;
using System.Data.Common;
using ALOD.Core.Domain.Workflow;
using ALOD.Data;
using ALODWebUtility.Common;

namespace ALODWebUtility.Workflow
{
    [Serializable]
    public class Workflow
    {
        #region Members / Properties

        protected WorkflowActionList _actions;
        protected bool _active;
        protected string _compo;
        protected bool _formal;
        protected byte _id;
        protected int _initialStatus;
        protected byte _module;
        protected string _statusDesc;
        protected int _statusIn;
        protected WorkflowStepList _steps = new WorkflowStepList();
        protected string _title;

        public WorkflowActionList Actions
        {
            get { return _actions; }
            set { _actions = value; }
        }

        public bool Active
        {
            get { return _active; }
            set { _active = value; }
        }

        public string Compo
        {
            get { return _compo; }
            set { _compo = value; }
        }

        public string FullTitle
        {
            get
            {
                return _title + " [" + Utility.GetCompoAbbr(_compo) + "]";
            }
        }

        public byte Id
        {
            get { return _id; }
            set { _id = value; }
        }

        public int InitialStatus
        {
            get { return _initialStatus; }
            set { _initialStatus = value; }
        }

        public bool IsFormal
        {
            get { return _formal; }
            set { _formal = value; }
        }

        public byte ModuleId
        {
            get { return _module; }
            set { _module = value; }
        }

        public string ModuleName
        {
            get { return _module.ToString(); }
        }

        public string StatusDescription
        {
            get { return _statusDesc; }
            set { _statusDesc = value; }
        }

        public int StatusIn
        {
            get { return _statusIn; }
            set { _statusIn = value; }
        }

        public WorkflowStepList Steps
        {
            get { return _steps; }
            set { _steps = value; }
        }

        public string Title
        {
            get { return _title; }
            set { _title = value; }
        }

        #endregion

        #region Constructors

        public Workflow()
        {
            _id = 0;
            _compo = string.Empty;
            _formal = false;
            _title = string.Empty;
        }

        public Workflow(byte workflowId, bool loadSteps = false)
        {
            _id = workflowId;
            _steps = new WorkflowStepList();
            GetDetails();

            if (loadSteps)
            {
                GetAllSteps();
            }
        }

        public Workflow(byte workflowId, int status, bool deathCase)
        {
            _id = workflowId;
            _statusIn = status;
            _steps = new WorkflowStepList();
            GetCurrentSteps(deathCase);
        }

        #endregion

        #region Data methods

        public WorkflowStepList GetAllSteps()
        {
            _steps.Clear();
            _steps.GetSteps(_id);
            return _steps;
        }

        public WorkflowStepList GetCurrentSteps(bool deathCase)
        {
            _steps.Clear();
            _steps.GetStepsByStatus(_id, _statusIn, deathCase);
            return _steps;
        }

        public WorkflowStepList GetCurrentSteps(int status, bool deathCase)
        {
            _statusIn = status;
            _steps.Clear();
            _steps.GetStepsByStatus(_id, _statusIn, deathCase);
            return _steps;
        }

        public void GetDetails()
        {
            SqlDataStore adapter = new SqlDataStore();
            DbCommand cmd = adapter.GetSqlStringCommand(
            "SELECT title, a.compo, formal, active, cast(initialStatus as int) as initialStatus, b.description "
            + "FROM core_Workflow a JOIN core_StatusCodes b ON b.statusId = a.initialStatus WHERE workflowId = @workflowId");
            adapter.AddInParameter(cmd, "@workflowId", ALOD.Data.DbType.Byte, _id);
            adapter.ExecuteReader(DetailsReader, cmd);
        }

        public bool Insert()
        {
            if (_title.Trim().Length == 0 || _compo.Trim().Length != 1)
            {
                return false;
            }

            SqlDataStore adapter = new SqlDataStore();
            return Convert.ToInt32(adapter.ExecuteScalar("core_workflow_sp_InsertWorkflow", _title, _module, _compo, _formal, _initialStatus)) > 0;
        }

        protected void DetailsReader(SqlDataStore adapter, IDataReader reader)
        {
            // 0-title, 1-compo, 2-formal, 3-active
            _title = adapter.GetString(reader, 0);
            _compo = adapter.GetString(reader, 1);
            _formal = adapter.GetBoolean(reader, 2);
            _active = adapter.GetBoolean(reader, 3);
            _initialStatus = adapter.GetInt32(reader, 4);
            _statusDesc = adapter.GetString(reader, 5);
        }

        #endregion
    }
}
