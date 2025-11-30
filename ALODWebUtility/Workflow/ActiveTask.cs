using System;
using ALOD.Core.Domain.Workflow;

namespace ALODWebUtility.Workflow
{
    public class ActiveTask
    {
        private int _formalCount;
        private int _informalCount;
        private ModuleType _module;
        private int _overdueCount;
        private int _regionId;
        private int _statusId;
        private string _title;
        private int _workflowId;

        public ActiveTask()
        {
            _workflowId = 0;
            _module = ModuleType.LOD;
            _title = string.Empty;
            _regionId = 0;
            _overdueCount = 0;
            _formalCount = 0;
            _informalCount = 0;
            _statusId = 0;
        }

        public string ActiveTitle
        {
            get { return _title; }
            set { _title = value; }
        }

        public int FormalCount
        {
            get { return _formalCount; }
            set { _formalCount = value; }
        }

        public int InformalCount
        {
            get { return _informalCount; }
            set { _informalCount = value; }
        }

        public ModuleType ModuleStr
        {
            get { return _module; }
            set { _module = value; }
        }

        public int OverdueCount
        {
            get { return _overdueCount; }
            set { _overdueCount = value; }
        }

        public int RegionId
        {
            get { return _regionId; }
            set { _regionId = value; }
        }

        public int StatusID
        {
            get { return _statusId; }
            set { _statusId = value; }
        }

        public int WorkflowId
        {
            get { return _workflowId; }
            set { _workflowId = value; }
        }
    }
}
