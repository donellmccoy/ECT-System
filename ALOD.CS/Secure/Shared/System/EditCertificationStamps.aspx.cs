using System;
using System.Collections.Generic;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using ALOD.Core.Domain.Lookup;
using ALOD.Core.Domain.Workflow;
using ALOD.Core.Interfaces.DAOInterfaces;
using ALOD.Data;
using ALODWebUtility.Worklfow;

namespace ALOD.Web.Sys
{
    public partial class Secure_Shared_System_EditCertificationStamps : System.Web.UI.Page
    {
        protected GridView gdvCertificationStamps;
        protected GridView gdvCertificationStampWorkflows;
        protected Panel pnlWorkflowMaps;
        protected Panel pnlEditCertificationStamp;
        protected Button btnAddCertificationStamp;
        protected Button btnUpdateWorkflowMappings;
        protected Button btnCancelWorkflowMappings;
        protected Button btnInsertCertificationStamp;
        protected Button btnUpdateCertificationStamp;
        protected Button btnCancelCertificationStamp;
        protected Label lblEditBlockLabel;
        protected TextBox txtStampName;
        protected TextBox txtStampBody;
        protected CheckBox chkStampIsQualified;
        protected HtmlTableRow trValidationErrors;
        protected BulletedList bllValidationErrors;

        private ICertificationStampDao _certificationStampDao;
        private IWorkflowDao _workflowDao;

        public ICertificationStampDao CertificationStampDao
        {
            get
            {
                if (_certificationStampDao == null)
                {
                    _certificationStampDao = new NHibernateDaoFactory().GetCertificationStampDao();
                }

                return _certificationStampDao;
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
            btnAddCertificationStamp.Click += btnAddCertificationStamp_Click;
            btnCancelCertificationStamp.Click += btnCancelCertificationStamp_Click;
            btnCancelWorkflowMappings.Click += btnCancelWorkflowMappings_Click;
            btnInsertCertificationStamp.Click += btnInsertCertificationStamp_Click;
            btnUpdateCertificationStamp.Click += btnUpdateCertificationStamp_Click;
            btnUpdateWorkflowMappings.Click += btnUpdateWorkflowMappings_Click;
            gdvCertificationStamps.RowCommand += gdvCertificationStamps_RowCommand;
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                UpdateCertificationStampGridView();
            }
        }

        protected void btnAddCertificationStamp_Click(object sender, EventArgs e)
        {
            StartAddingCertificationStamp();
        }

        protected void btnCancelCertificationStamp_Click(object sender, EventArgs e)
        {
            ClearEdit();
        }

        protected void btnCancelWorkflowMappings_Click(object sender, EventArgs e)
        {
            ClearEdit();
        }

        protected void btnInsertCertificationStamp_Click(object sender, EventArgs e)
        {
            if (!ValidateCertificationStampInput())
            {
                return;
            }

            CertificationStamp cs = new CertificationStamp();

            if (cs == null)
            {
                return;
            }

            cs.Name = Server.HtmlEncode(txtStampName.Text);
            cs.Body = Server.HtmlEncode(txtStampBody.Text);
            cs.IsQualified = chkStampIsQualified.Checked;

            CertificationStampDao.Insert(cs);

            ClearEdit();

            UpdateCertificationStampGridView();
        }

        protected void btnUpdateCertificationStamp_Click(object sender, EventArgs e)
        {
            if (!ValidateCertificationStampInput())
            {
                return;
            }

            CertificationStamp cs = new CertificationStamp();

            if (cs == null)
            {
                return;
            }

            cs.Id = Convert.ToInt32(ViewState["EditId"]);
            cs.Name = Server.HtmlEncode(txtStampName.Text);
            cs.Body = Server.HtmlEncode(txtStampBody.Text);
            cs.IsQualified = chkStampIsQualified.Checked;

            CertificationStampDao.Update(cs);

            ClearEdit();

            UpdateCertificationStampGridView();
        }

        protected void btnUpdateWorkflowMappings_Click(object sender, EventArgs e)
        {
            List<int> workflows = new List<int>();

            if (workflows == null)
            {
                ClearEdit();
                return;
            }

            foreach (GridViewRow row in gdvCertificationStampWorkflows.Rows)
            {
                bool isMapped = ((CheckBox)row.FindControl("chkAssociated")).Checked;
                int workflowId = int.Parse(((Label)row.FindControl("txtWorkflowId")).Text);

                if (isMapped)
                {
                    workflows.Add(workflowId);
                }
            }

            CertificationStampDao.UpdateCertificationStampWorkflowsMaps(Convert.ToInt32(ViewState["EditId"]), workflows);

            ClearEdit();
        }

        protected void gdvCertificationStamps_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            string[] parts = e.CommandArgument.ToString().Split('|');

            if (parts.Length < 2)
            {
                return;
            }

            int id = Convert.ToInt32(parts[0]);
            int rowIndex = Convert.ToInt32(parts[1]);

            gdvCertificationStamps.SelectedIndex = rowIndex;
            ViewState["EditId"] = id;

            if (e.CommandName == "EditWorkflowMaps")
            {
                StartWorkflowMapping(id);
            }
            else if (e.CommandName == "EditCertificationStamp")
            {
                StartEditingCertificationStamp(id);
            }
        }

        private void ClearEdit()
        {
            gdvCertificationStamps.SelectedIndex = -1;
            gdvCertificationStamps.EditIndex = -1;
            ViewState["EditId"] = 0;
            pnlWorkflowMaps.Visible = false;
            pnlEditCertificationStamp.Visible = false;
            bllValidationErrors.Items.Clear();
            trValidationErrors.Visible = false;
        }

        private void StartAddingCertificationStamp()
        {
            pnlEditCertificationStamp.Visible = true;
            btnInsertCertificationStamp.Visible = true;
            btnUpdateCertificationStamp.Visible = false;

            lblEditBlockLabel.Text = "2 - Add Certification Stamp";

            txtStampName.Text = string.Empty;
            txtStampBody.Text = string.Empty;
            chkStampIsQualified.Checked = false;
        }

        private void StartEditingCertificationStamp(int id)
        {
            pnlEditCertificationStamp.Visible = true;
            btnUpdateCertificationStamp.Visible = true;
            btnInsertCertificationStamp.Visible = false;

            lblEditBlockLabel.Text = "2 - Edit Certification Stamp";

            CertificationStamp stamp = CertificationStampDao.GetById(id);

            if (stamp == null)
            {
                ClearEdit();
                return;
            }

            txtStampName.Text = stamp.Name;
            txtStampBody.Text = stamp.Body;
            chkStampIsQualified.Checked = stamp.IsQualified;
        }

        private void StartWorkflowMapping(int id)
        {
            pnlWorkflowMaps.Visible = true;
            List<Workflow> associatedWorkflows = CertificationStampDao.GetCertificationStampWorkflows(id);

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

            gdvCertificationStampWorkflows.DataSource = workflowItems;
            gdvCertificationStampWorkflows.DataBind();
        }

        private void UpdateCertificationStampGridView()
        {
            gdvCertificationStamps.DataSource = CertificationStampDao.GetAll();
            gdvCertificationStamps.DataBind();
        }

        private bool ValidateCertificationStampInput()
        {
            bool isValid = true;

            trValidationErrors.Visible = false;
            bllValidationErrors.Items.Clear();

            if (string.IsNullOrEmpty(txtStampName.Text))
            {
                isValid = false;
                bllValidationErrors.Items.Add("Name cannot be empty");
            }

            if (string.IsNullOrEmpty(txtStampBody.Text))
            {
                isValid = false;
                bllValidationErrors.Items.Add("Body cannot be empty");
            }

            if (!isValid)
            {
                trValidationErrors.Visible = true;
            }

            return isValid;
        }
    }
}
