using System;

namespace ALODWebUtility.Workflow
{
    public class WorkflowAssociationViewModel
    {
        private int _associateId;
        private bool _isAssociated;
        private int _workflowId;
        private string _workflowTitle;

        public WorkflowAssociationViewModel()
        {
            AssociateId = 0;
            WorkflowId = 0;
            WorkflowTitle = string.Empty;
            IsAssociated = false;
        }

        public WorkflowAssociationViewModel(int associateId, int workflowId, string workflowTitle, bool isAssociated)
        {
            _associateId = associateId;
            _workflowId = workflowId;
            _workflowTitle = workflowTitle;
            _isAssociated = isAssociated;
        }

        public int AssociateId
        {
            get { return _associateId; }
            set { _associateId = value; }
        }

        public bool IsAssociated
        {
            get { return _isAssociated; }
            set { _isAssociated = value; }
        }

        public int WorkflowId
        {
            get { return _workflowId; }
            set { _workflowId = value; }
        }

        public string WorkflowTitle
        {
            get { return _workflowTitle; }
            set { _workflowTitle = value; }
        }
    }
}
