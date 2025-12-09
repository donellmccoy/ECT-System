using System;
using System.Collections.Generic;
using System.Web.UI.WebControls;
using ALOD.Core.Domain.Workflow;
using ALOD.Core.Interfaces.DAOInterfaces;
using ALOD.Data;
using ALODWebUtility.Worklfow;

namespace ALOD.Web.Sys
{
    public partial class Secure_Shared_System_EditDispositionLookup : System.Web.UI.Page
    {
        protected GridView gdvDispositions;
        protected GridView gdvDispositionWorkflows;
        protected Panel pnlWorkflowMaps;
        protected Label lblAddDisposition;
        protected TextBox txtAddDisposition;
        protected Button btnAddDisposition;
        protected Button btnUpdateWorkflowMappings;
        protected Button btnCancelWorkflowMappings;

        private ILookupDispositionDao _dispositionDao;
        private IWorkflowDao _workflowDao;

        public ILookupDispositionDao DispositionDao
        {
            get
            {
                if (_dispositionDao == null)
                {
                    _dispositionDao = new NHibernateDaoFactory().GetLookupDispositionDao();
                }

                return _dispositionDao;
            }
        }

        public IWorkflowDao WorkflowDao
        {
            get
            {
                if (_workflowDao == null)
                {
                    _workflowDao = new NHibernateDaoFactory().GetWorkflowDao();
                }

                return _workflowDao;
            }
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            btnAddDisposition.Click += btnAddDisposition_Click;
            btnCancelWorkflowMappings.Click += btnCancelWorkflowMappings_Click;
            btnUpdateWorkflowMappings.Click += btnUpdateWorkflowMappings_Click;
            gdvDispositions.RowCancelingEdit += gdvDispositions_RowCancelingEdit;
            gdvDispositions.RowCommand += gdvDispositions_RowCommand;
            gdvDispositions.RowEditing += gdvDispositions_RowEditing;
            gdvDispositions.RowUpdating += gdvDispositions_RowUpdating;
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                UpdateDispositionGridView();
            }
        }

        protected void btnAddDisposition_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtAddDisposition.Text))
            {
                return;
            }

            DispositionDao.InsertDisposition(Server.HtmlEncode(txtAddDisposition.Text));

            UpdateDispositionGridView();
        }

        protected void btnCancelWorkflowMappings_Click(object sender, EventArgs e)
        {
            ClearEdit();
        }

        protected void btnUpdateWorkflowMappings_Click(object sender, EventArgs e)
        {
            List<int> workflows = new List<int>();

            if (workflows == null)
            {
                ClearEdit();
                return;
            }

            foreach (GridViewRow row in gdvDispositionWorkflows.Rows)
            {
                bool isMapped = ((CheckBox)row.FindControl("chkAssociated")).Checked;
                int workflowId = int.Parse(((Label)row.FindControl("txtWorkflowId")).Text);

                if (isMapped)
                {
                    workflows.Add(workflowId);
                }
            }

            DispositionDao.UpdateDispositionWorkflowsMaps(Convert.ToInt32(ViewState["EditId"]), workflows);

            ClearEdit();
        }

        protected void gdvDispositions_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
        {
            gdvDispositions.EditIndex = -1;
            UpdateDispositionGridView();
        }

        protected void gdvDispositions_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "EditWorkflowMaps")
            {
                string[] parts = e.CommandArgument.ToString().Split('|');
                int id = Convert.ToInt32(parts[0]);
                int rowIndex = Convert.ToInt32(parts[1]);

                gdvDispositions.SelectedIndex = rowIndex;
                ViewState["EditId"] = id;

                StartWorkflowMapping(id);
            }
        }

        protected void gdvDispositions_RowEditing(object sender, GridViewEditEventArgs e)
        {
            gdvDispositions.EditIndex = e.NewEditIndex;
            UpdateDispositionGridView();
        }

        protected void gdvDispositions_RowUpdating(object sender, GridViewUpdateEventArgs e)
        {
            int dispositionId = Convert.ToInt32(gdvDispositions.DataKeys[e.RowIndex].Value);
            string dispositionName = ((TextBox)gdvDispositions.Rows[e.RowIndex].FindControl("txtName")).Text;

            DispositionDao.UpdateDisposition(dispositionId, dispositionName);

            gdvDispositions.EditIndex = -1;

            UpdateDispositionGridView();
        }

        private void ClearEdit()
        {
            gdvDispositions.SelectedIndex = -1;
            gdvDispositions.EditIndex = -1;
            ViewState["EditId"] = 0;
            pnlWorkflowMaps.Visible = false;
        }

        private void StartWorkflowMapping(int id)
        {
            pnlWorkflowMaps.Visible = true;
            List<Workflow> associatedWorkflows = DispositionDao.GetDispositionWorkflows(id);

            List<WorkflowAssociationViewModel> workflowItems = new List<WorkflowAssociationViewModel>();

            foreach (Workflow w in WorkflowDao.GetAll())
            {
                if (associatedWorkflows.Contains(w))
                {
                    workflowItems.Add(new WorkflowAssociationViewModel(id, w.Id, w.Title, 1));
                }
                else
                {
                    workflowItems.Add(new WorkflowAssociationViewModel(id, w.Id, w.Title, 0));
                }
            }

            gdvDispositionWorkflows.DataSource = workflowItems;
            gdvDispositionWorkflows.DataBind();
        }

        private void UpdateDispositionGridView()
        {
            gdvDispositions.DataSource = DispositionDao.GetAll();
            gdvDispositions.DataBind();
        }
    }
}
