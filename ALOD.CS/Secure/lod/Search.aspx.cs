using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using ALOD.Core.Domain.Documents;
using ALOD.Core.Domain.Modules.Lod;
using ALOD.Core.Domain.Workflow;
using ALOD.Core.Utils;
using static ALOD.Core.Utils.RegexValidation;
using ALOD.Data;
using ALOD.Data.Services;
using ALODWebUtility.Common;
using ALODWebUtility.Perms.Search;
using static ALODWebUtility.Common.SessionInfo;
using static ALODWebUtility.Common.Utility;
using Microsoft.VisualBasic;

namespace ALOD.Web.LOD
{
    public partial class Secure_lod_Search : System.Web.UI.Page
    {
        private IDocumentDao _dao;
        private IList<Document> _documents;
        private LineOfDuty _lod = null;
        private LineOfDuty instance;

        protected IDocumentDao DocumentDao
        {
            get
            {
                if (_dao == null)
                {
                    _dao = new SRXDocumentStore(HttpContext.Current.Session["UserName"].ToString());
                }

                return _dao;
            }
        }

        protected void ddlLodStatus_DataBound(object sender, EventArgs e)
        {
            StatusSelect.Items.Insert(0, new ListItem("-- All --", string.Empty));
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                PageAsyncTask task = new PageAsyncTask(new BeginEventHandler(BeginAsyncUnitLookup), new EndEventHandler(EndAsyncUnitLookup), null, GetStateObject(UnitSelect));
                RegisterAsyncTask(task);
                PreloadSearchFilters();
                ResultGrid.Sort("CaseId", SortDirection.Ascending);

                SetInputFormatRestriction(Page, SsnBox, FormatRestriction.Numeric, "-");
                SetInputFormatRestriction(Page, NameBox, FormatRestriction.AlphaNumeric, PERMITTED_SPECIAL_CHAR_INPUT);
                SetInputFormatRestriction(Page, CaseIdBox, FormatRestriction.AlphaNumeric, "-");
            }
        }

        protected void ResultGrid_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            HeaderRowBinding((GridView)sender, e, "CaseId");

            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                DataRowView data = (DataRowView)e.Row.DataItem;
                int lockId = 0;
                int.TryParse(data["lockId"].ToString(), out lockId);

                if (lockId > 0)
                {
                    ((Image)e.Row.FindControl("LockImage")).Visible = true;
                }

                int refID = (int)data["RefId"];
                ViewFinal(refID.ToString(), data, e);
            }
        }

        protected void SearchButton_Click(object sender, EventArgs e)
        {
            CaseIdBox.Text = CaseIdBox.Text.Trim();
            SsnBox.Text = SsnBox.Text.Trim();
            NameBox.Text = NameBox.Text.Trim();

            ResultGrid.DataBind();
        }

        protected void SearchData_Selecting(object sender, ObjectDataSourceSelectingEventArgs e)
        {
            NoFiltersPanel.Visible = false;
            e.InputParameters["sarcpermission"] = UserHasPermission(PERMISSION_VIEW_SARC_CASES);
            
            if (SsnBox.Text.Trim().Length == 0 &&
                NameBox.Text.Trim().Length == 0 &&
                CaseIdBox.Text.Trim().Length == 0 &&
                StatusSelect.SelectedIndex == 0 &&
                FormalSelect.SelectedIndex == 0 &&
                UnitSelect.SelectedIndex < 1)
            {
                NoFiltersPanel.Visible = true;
                e.Cancel = true;
                return;
            }
        }

        // Handles Printing 348/261 forms:
        // If IsFinal then get pdf from database, else build pdf on the fly
        protected void ViewFinal(string refID, DataRowView data, GridViewRowEventArgs e)
        {
            string form348ID = "0";
            string form261ID = "0";
            instance = LodService.GetById(int.Parse(data["RefId"].ToString()));

            // Check for GroupID; some cases were cancelled
            if (instance.DocumentGroupId.HasValue && instance.DocumentGroupId.Value > 0)
            {
                _documents = DocumentDao.GetDocumentsByGroupId(instance.DocumentGroupId.Value);
            }

            if ((bool)data["IsFinal"])
            {
                bool isDoc = false;
                if (_documents != null)
                {
                    // fileSubString is used to get the correct 348 document if multiple 348s are associated with the LOD's group Id.
                    // This happens when a case is overridden and recompleted or if a case is reinvestigated.
                    string fileSubString = instance.CaseId + "-Generated";

                    foreach (var docItem in _documents)
                    {
                        if (docItem.DocType.ToString() == "FinalForm348" && docItem.OriginalFileName.Contains(fileSubString))
                        {
                            form348ID = docItem.Id.ToString();
                            isDoc = true;
                        }
                    }
                }

                if (isDoc)
                {
                    string url348 = this.ResolveClientUrl("~/Secure/Shared/DocumentViewer.aspx?docID=" + form348ID + "&modId=" + (int)ModuleType.LOD);
                    ((ImageButton)e.Row.FindControl("PrintImage")).OnClientClick = "viewDoc('" + url348 + "'); return false;";
                    ((ImageButton)e.Row.FindControl("PrintImage")).AlternateText = "Print Final Forms";
                }
                else
                {
                    ((ImageButton)e.Row.FindControl("PrintImage")).OnClientClick = "printForms('" + refID + "', 'lod'); return false;";
                    ((ImageButton)e.Row.FindControl("PrintImage")).AlternateText = "Print Final Forms";
                }
            }
            else
            {
                ((ImageButton)e.Row.FindControl("PrintImage")).OnClientClick = "printForms('" + refID + "', 'lod'); return false;";
                ((ImageButton)e.Row.FindControl("PrintImage")).AlternateText = "Print Draft Forms";
            }
        }

        private void PreloadSearchFilters()
        {
            if (Request.QueryString["data"] == null)
            {
                return;
            }

            string data = Request.QueryString["data"];

            if (Information.IsNumeric(data))
            {
                if (data.Length == 4)
                {
                    int num = int.Parse(data);
                    if (num >= 2005 && num <= 2026)
                    {
                        // Assumes it is the year portion of the caseID
                        CaseIdBox.Text = data;
                    }
                    else
                    {
                        // Assume SSN
                        SsnBox.Text = data;
                    }
                }
                else
                {
                    // Assume caseId
                    CaseIdBox.Text = data;
                }
            }
            else
            {
                // Could be either a name, or caseid
                if (IsValidCaseId(data))
                {
                    CaseIdBox.Text = data;
                }
                else
                {
                    NameBox.Text = data;
                }
            }
        }

        private void ResultGrid_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "view")
            {
                string[] parts = e.CommandArgument.ToString().Split(';');
                StringBuilder strQuery = new StringBuilder();
                ItemSelectedEventArgs args = new ItemSelectedEventArgs();
                args.RefId = int.Parse(parts[0]);

                strQuery.Append("refId=" + args.RefId.ToString());
                args.Type = ModuleType.LOD;

                if (UserHasPermission(GetViewPermissionByModuleId(args.Type)))
                {
                    switch (args.Type)
                    {
                        case ModuleType.LOD:
                            args.Url = "~/Secure/Lod/init.aspx?" + strQuery.ToString();
                            break;
                    }
                }
                else
                {
                    args.Url = "~/Secure/Shared/SecureAccessDenied.aspx?deniedType=1";
                }

                Response.Redirect(args.Url);
            }
        }
    }
}
