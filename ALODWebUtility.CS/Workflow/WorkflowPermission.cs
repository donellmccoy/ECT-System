using System;

namespace ALODWebUtility.Workflow
{
    public class WorkflowPermission
    {
        protected bool _canCreate;
        protected bool _canView;
        protected byte _groupId;
        protected string _groupName;
        protected byte _workflowId;

        public bool CanCreate
        {
            get { return _canCreate; }
            set { _canCreate = value; }
        }

        public bool CanView
        {
            get { return _canView; }
            set { _canView = value; }
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

        public byte WorkflowId
        {
            get { return _workflowId; }
            set { _workflowId = value; }
        }
    }
}
