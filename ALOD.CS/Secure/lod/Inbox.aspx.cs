using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web.UI.WebControls;
using ALOD.Core.Domain.Users;
using ALOD.Core.Domain.Workflow;
using ALOD.Core.Utils;
using ALOD.Data.Services;
using ALODWebUtility.Common;

namespace ALOD.Web.LOD
{
    public partial class Secure_lod_Inbox : System.Web.UI.Page
    {
        const string COLUMN_LOCK_ID = "lockId";
        const string DEFAULT_SORT_COLUMN = "CaseId";
        const string ERROR_LOADING_QUEUE = "An error occured loading the requested queue";
        const string ERROR_NO_MATCHES = "No matches found";
        const string KEY_CLICKED = "SEARCH_CLICKED";
        const string KEY_USERID = "USER_ID";
        const string KEY_VIEW = "REPORT_VIEW";
        const string PARAM_MODULE_ID = "moduleId";
        const string PARAM_REPORT_VIEW = "rptView";
        const string PARAM_SARC = "sarcpermission";
        const string PARAM_USERID = "userId";
        const int VALUE_COUNT = 2;
        const string VALUE_DELIMITER = ";";

        protected bool GetInboxClicked
        {
            get
            {
                if (ViewState[KEY_CLICKED] == null)
                {
                    return false;
                }
                return (bool)ViewState[KEY_CLICKED];
            }
            set
            {
                ViewState[KEY_CLICKED] = value;
            }
        }

        protected int SearchUserId
        {
            get
            {
                if (ViewState[KEY_USERID] == null)
                {
                    return 0;
                }
                return (int)ViewState[KEY_USERID];
            }
            set
            {
                ViewState[KEY_USERID] = value;
            }
        }

        protected byte SearchView
        {
            get
            {
                if (ViewState[KEY_VIEW] == null)
                {
                    return 0;
                }
                return (byte)ViewState[KEY_VIEW];
            }
            set
            {
                ViewState[KEY_VIEW] = value;
            }
        }

        protected void GetInboxView()
        {
            const int INDEX_USERID = 0;
            const int INDEX_VIEW = 1;

            if (UserSelect.Items.Count == 0)
            {
                ErrorMessageLabel.Text = ERROR_LOADING_QUEUE;
                return;
            }

            string[] parts = UserSelect.SelectedValue.Split(VALUE_DELIMITER.ToCharArray());

            if (parts.Length != VALUE_COUNT)
            {
                ErrorMessageLabel.Text = ERROR_LOADING_QUEUE;
                return;
            }

            int.TryParse(parts[INDEX_USERID], out int userId);
            byte.TryParse(parts[INDEX_VIEW], out byte view);

            SearchUserId = userId;
            SearchView = view;

            if (SearchUserId == 0 || SearchView == 0)
            {
                ErrorMessageLabel.Text = ERROR_LOADING_QUEUE;
                return;
            }

            ErrorMessageLabel.Text = string.Empty;
            GetInboxClicked = true;
            ResultGrid.Visible = true;
            ResultGrid.DataBind();
        }

        protected void LodData_Selecting(object sender, ObjectDataSourceSelectingEventArgs e)
        {
            if (!GetInboxClicked)
            {
                e.Cancel = true;
                return;
            }

            if (SearchUserId == 0 || SearchView == 0)
            {
                ErrorMessageLabel.Text = ERROR_LOADING_QUEUE;
                e.Cancel = true;
                return;
            }

            e.InputParameters[PARAM_USERID] = SearchUserId;
            e.InputParameters[PARAM_REPORT_VIEW] = SearchView;
            e.InputParameters[PARAM_MODULE_ID] = (byte)ModuleType.LOD;
            e.InputParameters[PARAM_SARC] = false;
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                SetDefaultButton(UsernameInput, SearchButton);
                SetInputFormatRestriction(Page, UsernameInput, FormatRestriction.AlphaNumeric, PERMITTED_SPECIAL_CHAR_INPUT);
            }
        }

        protected void Page_PreRender(object sender, EventArgs e)
        {
            ErrorPanel.Visible = (ErrorMessageLabel.Text.Length > 0);
        }

        protected void PopulateUserList(string username)
        {
            UserSelect.Items.Clear();
            IList<AppUser> users = UserService.FindByUsername(username);

            if (users.Count == 0)
            {
                ErrorMessageLabel.Text = ERROR_NO_MATCHES;
                UserPanel.Visible = false;
                StartPanel.Visible = true;
                return;
            }
            else
            {
                ErrorMessageLabel.Text = string.Empty;
                UserPanel.Visible = true;
                StartPanel.Visible = false;
            }

            foreach (AppUser row in users)
            {
                string text = string.Format("{0} {1}, {2} ({3}) - {4} ({5}) ",
                        row.Rank.Rank, row.LastName, row.FirstName,
                        row.Username.ToUpper(), row.CurrentRoleName, row.Unit.Name);

                string value = row.Id.ToString() + VALUE_DELIMITER;

                if (row.ReportView.HasValue)
                {
                    value += row.ReportView.Value.ToString();
                }
                else
                {
                    value += row.CurrentRole.Group.ReportView.ToString();
                }

                UserSelect.Items.Add(new ListItem(text, value));
            }
        }

        protected void ResetButton_Click(object sender, EventArgs e)
        {
            StartPanel.Visible = true;
            UserPanel.Visible = false;
            ResultGrid.Visible = false;
            SearchUserId = 0;
            SearchView = 0;
            GetInboxClicked = false;
            UsernameInput.Focus();
        }

        protected void ResultGrid_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            const string CONTROL_LOCK_IMAGE = "LockImage";
            HeaderRowBinding(sender, e, DEFAULT_SORT_COLUMN);

            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                DataRowView data = (DataRowView)e.Row.DataItem;
                int lockId = 0;
                int.TryParse(data[COLUMN_LOCK_ID].ToString(), out lockId);

                if (lockId > 0)
                {
                    ((Image)e.Row.FindControl(CONTROL_LOCK_IMAGE)).Visible = true;
                }

                int refID = (int)data["RefId"];

                ((ImageButton)e.Row.FindControl("PrintImage")).OnClientClick = "printForms('" + refID + "', 'lod'); return false;";
            }
        }

        protected void SearchButton_Click(object sender, EventArgs e)
        {
            GetInboxClicked = false;
            string userName = Server.HtmlEncode(UsernameInput.Text.Trim());

            if (userName.Length == 0)
            {
                AddCssClass(UsernameInput, CSS_FIELD_REQUIRED);
            }
            else
            {
                RemoveCssClass(UsernameInput, CSS_FIELD_REQUIRED);
                PopulateUserList(userName);
            }
        }

        protected void ViewInboxButton_Click(object sender, EventArgs e)
        {
            GetInboxView();
        }
    }
}
