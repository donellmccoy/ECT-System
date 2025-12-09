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
    public partial class Secure_Shared_System_EditCaseTypes : System.Web.UI.Page
    {
        protected GridView gdvCaseTypes;
        protected GridView gdvSubCaseTypes;
        protected GridView gdvCaseTypeWorkflows;
        protected GridView gdvCaseTypeSubCaseTypes;
        protected Panel pnlSubCaseTypes;
        protected Panel pnlWorkflowMaps;
        protected Panel pnlSubCaseTypeMaps;
        protected Label lblAddCaseType;
        protected TextBox txtAddCaseType;
        protected Button btnAddCaseType;
        protected Label lblAddSubCaseType;
        protected TextBox txtAddSubCaseType;
        protected Button btnAddSubCaseType;
        protected Button btnUpdateWorkflowMappings;
        protected Button btnCancelWorkflowMappings;
        protected Button btnUpdateSubCaseTypeMappings;
        protected Button btnCancelSubCaseTypeMappings;

        private ICaseTypeDao _caseTypeDao;
        private IWorkflowDao _workflowDao;

        public ICaseTypeDao CaseTypeDao
        {
            get
            {
                if (_caseTypeDao == null)
                {
                    _caseTypeDao = new NHibernateDaoFactory().GetCaseTypeDao();
                }

                return _caseTypeDao;
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
            btnAddCaseType.Click += btnAddCaseType_Click;
            btnAddSubCaseType.Click += btnAddSubCaseType_Click;
            btnCancelSubCaseTypeMappings.Click += btnCancelSubCaseTypeMappings_Click;
            btnUpdateSubCaseTypeMappings.Click += btnUpdateSubCaseTypeMappings_Click;
            btnCancelWorkflowMappings.Click += btnCancelWorkflowMappings_Click;
            btnUpdateWorkflowMappings.Click += btnUpdateWorkflowMappings_Click;
            gdvCaseTypes.RowCancelingEdit += gdvCaseTypes_RowCancelingEdit;
            gdvCaseTypes.RowCommand += gdvCaseTypes_RowCommand;
            gdvCaseTypes.RowEditing += gdvCaseTypes_RowEditing;
            gdvCaseTypes.RowUpdating += gdvCaseTypes_RowUpdating;
            gdvSubCaseTypes.RowCancelingEdit += gdvSubCaseTypes_RowCancelingEdit;
            gdvSubCaseTypes.RowEditing += gdvSubCaseTypes_RowEditing;
            gdvSubCaseTypes.RowUpdating += gdvSubCaseTypes_RowUpdating;
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                UpdateCaseTypeGridView();
                UpdateSubCaseTypeGridView();
            }
        }

        protected void btnAddCaseType_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtAddCaseType.Text))
            {
                return;
            }

            CaseType ct = new CaseType();
            ct.Name = Server.HtmlEncode(txtAddCaseType.Text);

            CaseTypeDao.Insert(ct);

            UpdateCaseTypeGridView();

            txtAddCaseType.Text = string.Empty;
        }

        protected void btnAddSubCaseType_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtAddSubCaseType.Text))
            {
                return;
            }

            CaseType ct = new CaseType();
            ct.Name = Server.HtmlEncode(txtAddSubCaseType.Text);

            CaseTypeDao.InsertSubCaseType(ct);

            UpdateSubCaseTypeGridView();

            txtAddSubCaseType.Text = string.Empty;
        }

        protected void btnCancelSubCaseTypeMappings_Click(object sender, EventArgs e)
        {
            ClearEdit();
        }

        protected void btnUpdateSubCaseTypeMappings_Click(object sender, EventArgs e)
        {
            List<int> subCaseTypes = new List<int>();

            if (subCaseTypes == null)
            {
                ClearEdit();
                return;
            }

            foreach (GridViewRow row in gdvCaseTypeSubCaseTypes.Rows)
            {
                bool isMapped = ((CheckBox)row.FindControl("chkSubCaseTypeAssociated")).Checked;
                int subCaseTypeId = int.Parse(((Label)row.FindControl("txtSubCaseTypeId")).Text);

                if (isMapped)
                {
                    subCaseTypes.Add(subCaseTypeId);
                }
            }

            CaseTypeDao.UpdateCaseTypeSubCaseTypeMaps(Convert.ToInt32(ViewState["EditId"]), subCaseTypes);

            ClearEdit();
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

            foreach (GridViewRow row in gdvCaseTypeWorkflows.Rows)
            {
                bool isMapped = ((CheckBox)row.FindControl("chkWorkflowAssociated")).Checked;
                int workflowId = int.Parse(((Label)row.FindControl("txtWorkflowId")).Text);

                if (isMapped)
                {
                    workflows.Add(workflowId);
                }
            }

            CaseTypeDao.UpdateCaseTypeWorkflowMaps(Convert.ToInt32(ViewState["EditId"]), workflows);

            ClearEdit();
        }

        protected void gdvCaseTypes_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
        {
            gdvCaseTypes.EditIndex = -1;
            UpdateCaseTypeGridView();
        }

        protected void gdvCaseTypes_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            string[] parts = e.CommandArgument.ToString().Split('|');

            if (parts.Length < 2)
            {
                return;
            }

            int id = Convert.ToInt32(parts[0]);
            int rowIndex = Convert.ToInt32(parts[1]);

            gdvCaseTypes.SelectedIndex = rowIndex;
            ViewState["EditId"] = id;

            if (e.CommandName == "EditWorkflowMaps")
            {
                StartWorkflowMapping(id);
            }
            else if (e.CommandName == "EditSubCaseTypeMaps")
            {
                StartSubCaseTypeMapping(id);
            }
        }

        protected void gdvCaseTypes_RowEditing(object sender, GridViewEditEventArgs e)
        {
            gdvCaseTypes.EditIndex = e.NewEditIndex;
            UpdateCaseTypeGridView();
        }

        protected void gdvCaseTypes_RowUpdating(object sender, GridViewUpdateEventArgs e)
        {
            int id = Convert.ToInt32(gdvCaseTypes.DataKeys[e.RowIndex].Value);
            string name = ((TextBox)gdvCaseTypes.Rows[e.RowIndex].FindControl("txtName")).Text;

            if (string.IsNullOrEmpty(name))
            {
                return;
            }

            CaseType ct = new CaseType();
            ct.Id = id;
            ct.Name = Server.HtmlEncode(name);

            CaseTypeDao.Update(ct);

            gdvCaseTypes.EditIndex = -1;

            UpdateCaseTypeGridView();
        }

        protected void gdvSubCaseTypes_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
        {
            gdvSubCaseTypes.EditIndex = -1;
            UpdateSubCaseTypeGridView();
        }

        protected void gdvSubCaseTypes_RowEditing(object sender, GridViewEditEventArgs e)
        {
            gdvSubCaseTypes.EditIndex = e.NewEditIndex;
            UpdateSubCaseTypeGridView();
        }

        protected void gdvSubCaseTypes_RowUpdating(object sender, GridViewUpdateEventArgs e)
        {
            int id = Convert.ToInt32(gdvSubCaseTypes.DataKeys[e.RowIndex].Value);
            string name = ((TextBox)gdvSubCaseTypes.Rows[e.RowIndex].FindControl("txtSubName")).Text;

            if (string.IsNullOrEmpty(name))
            {
                return;
            }

            CaseType ct = new CaseType();
            ct.Id = id;
            ct.Name = Server.HtmlEncode(name);

            CaseTypeDao.UpdateSubCaseType(ct);

            gdvSubCaseTypes.EditIndex = -1;

            UpdateSubCaseTypeGridView();
        }

        private void ClearEdit()
        {
            gdvCaseTypes.SelectedIndex = -1;
            gdvCaseTypes.EditIndex = -1;
            gdvSubCaseTypes.SelectedIndex = -1;
            gdvSubCaseTypes.EditIndex = -1;
            ViewState["EditId"] = 0;
            pnlWorkflowMaps.Visible = false;
            pnlSubCaseTypeMaps.Visible = false;
        }

        private void StartSubCaseTypeMapping(int id)
        {
            pnlSubCaseTypeMaps.Visible = true;

            CaseType ct = CaseTypeDao.GetById(id);

            if (ct == null)
            {
                return;
            }

            List<WorkflowAssociationViewModel> workflowItems = new List<WorkflowAssociationViewModel>();

            foreach (CaseType sct in CaseTypeDao.GetAllSubCaseTypes())
            {
                if (ct.SubCaseTypes.Contains(sct))
                {
                    workflowItems.Add(new WorkflowAssociationViewModel(id, sct.Id, sct.Name, 1));
                }
                else
                {
                    workflowItems.Add(new WorkflowAssociationViewModel(id, sct.Id, sct.Name, 0));
                }
            }

            gdvCaseTypeSubCaseTypes.DataSource = workflowItems;
            gdvCaseTypeSubCaseTypes.DataBind();
        }

        private void StartWorkflowMapping(int id)
        {
            pnlWorkflowMaps.Visible = true;
            List<Workflow> associatedWorkflows = CaseTypeDao.GetCaseTypeWorkflows(id);

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

            gdvCaseTypeWorkflows.DataSource = workflowItems;
            gdvCaseTypeWorkflows.DataBind();
        }

        private void UpdateCaseTypeGridView()
        {
            gdvCaseTypes.DataSource = CaseTypeDao.GetAll();
            gdvCaseTypes.DataBind();
        }

        private void UpdateSubCaseTypeGridView()
        {
            gdvSubCaseTypes.DataSource = CaseTypeDao.GetAllSubCaseTypes();
            gdvSubCaseTypes.DataBind();
        }
    }
}
