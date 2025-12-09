using System;
using System.Data;
using System.Web.UI.WebControls;
using ALOD.Core.Domain.Common;
using ALOD.Core.Utils;
using ALOD.Data;
using ALOD.Data.Services;
using ALOD.Logging;
using ALODWebUtility.Common;
using static ALODWebUtility.Common.Utility;

namespace ALOD.Web.Sys
{
    public partial class Secure_Shared_System_EditICDCodes : System.Web.UI.Page
    {
        protected Panel pnlSearch;
        protected Panel pnlResults;
        protected Label lblResultsPanelTitle;
        protected Label lblError;
        protected GridView gvResults;
        // TODO: Convert ICDCodeControl user control
        // protected ICDCodeControl ucICDCodeControl;

        public int SelectedParentId
        {
            get
            {
                return Convert.ToInt32(ViewState["SelectedParentId"]);
            }
            set
            {
                ViewState["SelectedParentId"] = value;
            }
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            gvResults.RowCancelingEdit += gvResults_RowCancelingEdit;
            gvResults.RowDataBound += gvResults_RowDataBound;
            gvResults.RowEditing += gvResults_RowEditing;
            gvResults.RowUpdating += gvResults_RowUpdating;
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            // TODO: Initialize ICDCodeControl when converted
            // ucICDCodeControl.Initialize(this);
            // ucICDCodeControl.ICDCodeSelectionChanged += UpdateGridViewEventHandler;

            if (!IsPostBack)
            {
                // TODO: Uncomment when ICDCodeControl is converted
                // ucICDCodeControl.DisplayReadWrite(true);
                // ucICDCodeControl.DisableLastLevel(true);
                SelectedParentId = 0;
                BindGridView(0);
            }
        }

        protected void gvResults_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
        {
            gvResults.EditIndex = -1;
            BindGridView(SelectedParentId);
        }

        protected void gvResults_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType != DataControlRowType.DataRow)
            {
                return;
            }

            // Check if this row is being edited or not
            if (e.Row.RowIndex == gvResults.EditIndex)
            {
                TextBox txtValue = (TextBox)e.Row.FindControl("txtValue");
                TextBox txtDescription = (TextBox)e.Row.FindControl("txtDescription");

                SetInputFormatRestriction(Page, txtValue, FormatRestriction.AlphaNumeric, PERMITTED_SPECIAL_CHAR_INPUT);
                SetInputFormatRestriction(Page, txtDescription, FormatRestriction.AlphaNumeric, PERMITTED_SPECIAL_CHAR_INPUT);
            }
        }

        protected void gvResults_RowEditing(object sender, GridViewEditEventArgs e)
        {
            gvResults.EditIndex = e.NewEditIndex;
            BindGridView(SelectedParentId);
        }

        protected void gvResults_RowUpdating(object sender, GridViewUpdateEventArgs e)
        {
            lblError.Visible = false;

            GridViewRow row = gvResults.Rows[e.RowIndex];

            if (row == null)
            {
                lblError.Text = "* An error has occurred! Edit Aborted!<br />";
                lblError.Visible = true;
                LogManager.LogError("Edit ICD Codes page failed to find the row being edited.");
                gvResults.EditIndex = -1;
                BindGridView(SelectedParentId);
                return;
            }

            // Validate the inputs before we do anything
            if (!ValidateGridViewEditInput(e.RowIndex))
            {
                return;
            }

            bool internalError = false;
            string nullControlName = string.Empty;
            TextBox txtValue = (TextBox)row.FindControl("txtValue");
            TextBox txtDescription = (TextBox)row.FindControl("txtDescription");
            CheckBox chkIsDisease = (CheckBox)row.FindControl("chkIsDisease");
            CheckBox chkActive = (CheckBox)row.FindControl("chkActive");
            Label lblIdControl = (Label)row.FindControl("lblId");

            if (txtValue == null)
            {
                internalError = true;
                nullControlName = "txtValue";
            }
            else if (txtDescription == null)
            {
                internalError = true;
                nullControlName = "txtDescription";
            }
            else if (chkIsDisease == null)
            {
                internalError = true;
                nullControlName = "chkIsDisease";
            }
            else if (chkActive == null)
            {
                internalError = true;
                nullControlName = "chkActive";
            }
            else if (lblIdControl == null)
            {
                internalError = true;
                nullControlName = "lblIdControl";
            }
            else
            {
                internalError = false;
            }

            if (internalError)
            {
                lblError.Text = "* An error has occurred! Edit Aborted! <br />";
                lblError.Visible = true;
                LogManager.LogError("Edit ICD Codes page failed to find the " + nullControlName + " gridview control.");
                gvResults.EditIndex = -1;
                BindGridView(SelectedParentId);
                return;
            }

            string newValue = Server.HtmlEncode(txtValue.Text);
            string newDescription = Server.HtmlEncode(txtDescription.Text);

            ICD9CodeDao icdDao = new NHibernateDaoFactory().GetICD9CodeDao();

            icdDao.UpdateCode(Convert.ToInt32(lblIdControl.Text), newValue, newDescription, chkIsDisease.Checked, chkActive.Checked);

            gvResults.EditIndex = -1;
            BindGridView(SelectedParentId);

            // TODO: Rebind the DDLs when ICDCodeControl is converted
            // ucICDCodeControl.ForceFullRebind(SelectedParentId);
        }

        protected void UpdateGridViewEventHandler(object sender, ICDCodeSelectedEventArgs e)
        {
            UpdateGridView(e.SelectedICDCodeId, e.SelectedDropDownLevel);
        }

        private void BindGridView(int icdCodeId)
        {
            ICD9CodeDao icdDao = new NHibernateDaoFactory().GetICD9CodeDao();
            DataSet dataSource = icdDao.GetChildren(icdCodeId, 10, false);

            if (dataSource == null)
            {
                return;
            }

            SelectedParentId = icdCodeId;

            gvResults.DataSource = dataSource;
            gvResults.DataBind();
        }

        private void UpdateGridView(int icdCodeId, int selectedLevel)
        {
            if (icdCodeId < 1 && selectedLevel != 1)
            {
                lblResultsPanelTitle.Text = "INVALID CODE!";
                return;
            }

            string codeDescription = "NONE SELECTED";

            if (icdCodeId > 0)
            {
                ICD9Code code = LookupService.GetIcd9CodeById(icdCodeId);

                if (code == null)
                {
                    lblResultsPanelTitle.Text = "INVALID CODE!";
                    return;
                }

                codeDescription = code.Description;
            }

            lblResultsPanelTitle.Text = "Children of ICD Code - " + codeDescription;

            lblError.Visible = false;

            BindGridView(icdCodeId);
        }

        private bool ValidateGridViewEditInput(int editIndex)
        {
            GridViewRow row = gvResults.Rows[editIndex];

            if (row == null)
            {
                return false;
            }

            TextBox txtDescription = (TextBox)row.FindControl("txtDescription");

            if (txtDescription == null || string.IsNullOrEmpty(txtDescription.Text))
            {
                txtDescription.CssClass = "fieldRequired";
                return false;
            }
            else
            {
                txtDescription.CssClass = "";
            }

            return true;
        }
    }
}
