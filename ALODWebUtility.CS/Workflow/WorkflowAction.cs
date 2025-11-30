using System;
using ALOD.Core.Domain.Workflow;
using ALOD.Data;
using System.Data.Common;

namespace ALODWebUtility.Workflow
{
    [Serializable]
    public class WorkflowAction
    {
        protected int _data;
        protected short _id;
        protected int _optionId;
        protected short _stepId;
        protected int _target;
        protected string _text;
        protected WorkflowActionType _type;

        public int Data
        {
            get { return _data; }
            set { _data = value; }
        }

        public short Id
        {
            get { return _id; }
            set { _id = value; }
        }

        public int OptionId
        {
            get { return _optionId; }
            set { _optionId = value; }
        }

        public short StepId
        {
            get { return _stepId; }
            set { _stepId = value; }
        }

        public int Target
        {
            get { return _target; }
            set { _target = value; }
        }

        public string Text
        {
            get { return _text; }
            set { _text = value; }
        }

        public WorkflowActionType Type
        {
            get { return _type; }
            set { _type = value; }
        }

        public void Insert()
        {
            SqlDataStore adapter = new SqlDataStore();
            adapter.ExecuteNonQuery("core_workflow_sp_InsertOptionAction", _type, _optionId, _target, _data);
        }
    }
}
