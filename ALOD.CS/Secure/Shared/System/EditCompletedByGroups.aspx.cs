using System;
using System.Collections.Generic;
using System.Web.UI.WebControls;
using ALOD.Core.Domain.Lookup;
using ALOD.Core.Domain.Workflow;
using ALOD.Core.Interfaces.DAOInterfaces;
using ALOD.Data;
using ALODWebUtility.Worklfow;

namespace ALOD.Web.Sys
{
    public partial class Secure_Shared_System_EditCompletedByGroups : System.Web.UI.Page
    {
        protected GridView gdvCompletedByGroups;
        protected GridView gdvCompletedByGroupWorkflows;
        protected Panel pnlWorkflowMaps;
        protected Label lblAddCompletedByGroup;
        protected TextBox txtAddCompletedByGroup;
        protected Button btnAddCompletedByGroup;
        protected Button btnUpdateWorkflowMappings;
        protected Button btnCancelWorkflowMappings;

        private ICompletedByGroupDao _completedByGroupDao;
        private IWorkflowDao _workflowDao;

        public ICompletedByGroupDao CompletedByGroupDao
        {
            get
            {
                if (_completedByGroupDao == null)
                {
                    _completedByGroupDao = new NHibernateDaoFactory().GetCompletedByGroupDao();
                }

                return _completedByGroupDao;
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
            btnAddCompletedByGroup.Click += btnAddCompletedByGroup_Click;
            btnCancelWorkflowMappings.Click += btnCancelWorkflowMappings_Click;
            btnUpdateWorkflowMappings.Click += btnUpdateWorkflowMappings_Click;
            gdvCompletedByGroups.RowCancelingEdit += gdvCompletedByGroups_RowCancelingEdit;
            gdvCompletedByGroups.RowCommand += gdvCompletedByGroups_RowCommand;
            gdvCompletedByGroups.RowEditing += gdvCompletedByGroups_RowEditing;
            gdvCompletedByGroups.RowUpdating += gdvCompletedByGroups_RowUpdating;
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                UpdateCompletedByGroupGridView();
            }
        }

        protected void btnAddCompletedByGroup_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtAddCompletedByGroup.Text))
            {
                return;
            }

            CompletedByGroup cbg = new CompletedByGroup();
            cbg.Name = Server.HtmlEncode(txtAddCompletedByGroup.Text);

            CompletedByGroupDao.Insert(cbg);

            UpdateCompletedByGroupGridView();

            txtAddCompletedByGroup.Text = string.Empty;
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

            foreach (GridViewRow row in gdvCompletedByGroupWorkflows.Rows)
            {
                bool isMapped = ((CheckBox)row.FindControl("chkWorkflowAssociated")).Checked;
                int workflowId = int.Parse(((Label)row.FindControl("txtWorkflowId")).Text);

                if (isMapped)
                {
                    workflows.Add(workflowId);
                }
            }

            CompletedByGroupDao.UpdateCompletedByGroupWorkflowMaps(Convert.ToInt32(ViewState["EditId"]), workflows);

            ClearEdit();
        }

        protected void gdvCompletedByGroups_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
        {
            gdvCompletedByGroups.EditIndex = -1;
            UpdateCompletedByGroupGridView();
        }

        protected void gdvCompletedByGroups_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            string[] parts = e.CommandArgument.ToString().Split('|');

            if (parts.Length < 2)
            {
                return;
            }

            int id = Convert.ToInt32(parts[0]);
            int rowIndex = Convert.ToInt32(parts[1]);

            gdvCompletedByGroups.SelectedIndex = rowIndex;
            ViewState["EditId"] = id;

            if (e.CommandName == "EditWorkflowMaps")
            {
                StartWorkflowMapping(id);
            }
        }

        protected void gdvCompletedByGroups_RowEditing(object sender, GridViewEditEventArgs e)
        {
            gdvCompletedByGroups.EditIndex = e.NewEditIndex;
            UpdateCompletedByGroupGridView();
        }

        protected void gdvCompletedByGroups_RowUpdating(object sender, GridViewUpdateEventArgs e)
        {
            int id = Convert.ToInt32(gdvCompletedByGroups.DataKeys[e.RowIndex].Value);
            string name = ((TextBox)gdvCompletedByGroups.Rows[e.RowIndex].FindControl("txtName")).Text;

            if (string.IsNullOrEmpty(name))
            {
                return;
            }

            CompletedByGroup cbg = new CompletedByGroup();
            cbg.Id = id;
            cbg.Name = Server.HtmlEncode(name);

            CompletedByGroupDao.Update(cbg);

            gdvCompletedByGroups.EditIndex = -1;

            UpdateCompletedByGroupGridView();
        }

        private void ClearEdit()
        {
            gdvCompletedByGroups.SelectedIndex = -1;
            gdvCompletedByGroups.EditIndex = -1;
            ViewState["EditId"] = 0;
            pnlWorkflowMaps.Visible = false;
        }

        private void StartWorkflowMapping(int id)
        {
            pnlWorkflowMaps.Visible = true;
            List<Workflow> associatedWorkflows = CompletedByGroupDao.GetCompletedByGroupWorkflows(id);

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

            gdvCompletedByGroupWorkflows.DataSource = workflowItems;
            gdvCompletedByGroupWorkflows.DataBind();
        }

        private void UpdateCompletedByGroupGridView()
        {
            gdvCompletedByGroups.DataSource = CompletedByGroupDao.GetAll();
            gdvCompletedByGroups.DataBind();
        }
    }
}
