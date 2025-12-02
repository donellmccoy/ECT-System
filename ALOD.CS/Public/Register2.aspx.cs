using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using ALOD.Core.Domain.Lookup;
using ALOD.Core.Domain.ServiceMembers;
using ALOD.Core.Domain.Users;
using ALOD.Core.Interfaces.DAOInterfaces;
using ALOD.Core.Utils;
using static ALOD.Core.Utils.RegexValidation;
using ALOD.Data;
using ALOD.Data.Services;
using ALODWebUtility.Common;
using static ALODWebUtility.Common.SessionInfo;
using static ALODWebUtility.Common.Utility;
using static ALODWebUtility.Common.WebControlSetters;
using DomainUnit = ALOD.Core.Domain.Users.Unit;

namespace ALOD.Web
{
    public partial class Public_Register2 : System.Web.UI.Page
    {
        protected string _edipin = "";

        protected void NextButton_Click(object sender, EventArgs e)
        {
            bool passed = true;
            if (!IsValidEmail(EmailBox.Text.Trim()))
            {
                EmailBox.CssClass = "fieldRequired";
                passed = false;
            }
            else
            {
                EmailBox.CssClass = "";
            }

            if (!IsValidPhoneNumber(PhoneBox.Text.Trim()))
            {
                PhoneBox.CssClass = "fieldRequired";
                passed = false;
            }
            else
            {
                PhoneBox.CssClass = "";
            }

            if (!passed)
            {
                return;
            }

            if (Page.IsValid)
            {
                SaveUser();
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            VerifyAccess();
            WriteHostName(Page);

            if (!IsPostBack)
            {
                btnFindUnit.Attributes.Add("onclick", "showSearcher('" + "Find Unit" + "','" + SrcUnitIdHdn.ClientID + "','" + SrcNameHdn.ClientID + "'); return false;");

                SetInputFormatRestriction(Page, FirstNameBox, FormatRestriction.AlphaNumeric, PERMITTED_SPECIAL_CHAR_INPUT);
                SetInputFormatRestriction(Page, MIddleNameBox, FormatRestriction.AlphaNumeric, PERMITTED_SPECIAL_CHAR_INPUT);
                SetInputFormatRestriction(Page, LastNameBox, FormatRestriction.AlphaNumeric, PERMITTED_SPECIAL_CHAR_INPUT);
                SetInputFormatRestriction(Page, EmailBox, FormatRestriction.AlphaNumeric, PERMITTED_SPECIAL_CHAR_INPUT);
                SetInputFormatRestriction(Page, Email2Box, FormatRestriction.AlphaNumeric, PERMITTED_SPECIAL_CHAR_INPUT);
                SetInputFormatRestriction(Page, Email3Box, FormatRestriction.AlphaNumeric, PERMITTED_SPECIAL_CHAR_INPUT);
                SetInputFormatRestriction(Page, PhoneBox, FormatRestriction.AlphaNumeric, PERMITTED_SPECIAL_CHAR_INPUT);
                SetInputFormatRestriction(Page, DsnBox, FormatRestriction.AlphaNumeric, PERMITTED_SPECIAL_CHAR_INPUT);
                SetInputFormatRestriction(Page, txtAddress, FormatRestriction.AlphaNumeric, PERMITTED_SPECIAL_CHAR_INPUT);
                SetInputFormatRestriction(Page, txtCity, FormatRestriction.AlphaNumeric, PERMITTED_SPECIAL_CHAR_INPUT);
                SetInputFormatRestriction(Page, txtZip, FormatRestriction.AlphaNumeric, PERMITTED_SPECIAL_CHAR_INPUT);

                PopulateStates();
                PopulateRoles();
                PopulateData();
            }
        }

        protected void PopulateCommonData(AppUser user)
        {
            if (user.Unit != null)
            {
                PasCodeBox.Text = user.Unit.Name;
                SrcUnitIdHdn.Text = user.Unit.Id.ToString();
            }

            if (user.CurrentRole != null)
            {
                SetDropdownByValue(RoleSelect, user.CurrentRole.Group.Id);
            }

            if (!string.IsNullOrEmpty(user.Email))
            {
                EmailBox.Text = Server.HtmlDecode(user.Email);
            }

            if (!string.IsNullOrEmpty(user.Email2))
            {
                Email2Box.Text = Server.HtmlDecode(user.Email2);
            }
            if (!string.IsNullOrEmpty(user.Email3))
            {
                Email3Box.Text = Server.HtmlDecode(user.Email3);
            }

            if (!string.IsNullOrEmpty(user.Phone))
            {
                PhoneBox.Text = Server.HtmlDecode(user.Phone);
            }

            if (!string.IsNullOrEmpty(user.DSN))
            {
                DsnBox.Text = Server.HtmlDecode(user.DSN);
            }

            if (user.Address != null)
            {
                if (!string.IsNullOrEmpty(user.Address.Street))
                {
                    txtAddress.Text = Server.HtmlDecode(user.Address.Street);
                }

                if (!string.IsNullOrEmpty(user.Address.City))
                {
                    txtCity.Text = Server.HtmlDecode(user.Address.City);
                }

                if (!string.IsNullOrEmpty(user.Address.State))
                {
                    SetDropdownByValue(cbState, user.Address.State);
                }

                if (!string.IsNullOrEmpty(user.Address.Zip))
                {
                    txtZip.Text = Server.HtmlDecode(user.Address.Zip);
                }
            }

            chkReceiveEmail.Checked = user.ReceiveEmail;
        }

        protected void PopulateData()
        {
            AppUser user = UserService.GetByEDIPIN(_edipin);
            ServiceMember data = null;

            if (user == null)
            {
                user = new AppUser();

                // The user does not yet have an account, so try and import from MIL
                if (data != null)
                {
                    user.Import(data);
                }
            }

            user.EDIPIN = _edipin;

            if (data != null)
            {
                // user is a member
                PopulateMemberData(user);
            }
            else
            {
                PopulateNonMemberData(user);
            }

            PopulateCommonData(user);
        }

        protected void PopulateMemberData(AppUser user)
        {
            NameEntryPanel.Visible = false;
            NameDisplayPanel.Visible = true;
            FullName.Text = user.FullName;
            RankDisplay.Text = user.Rank.Title;

            FirstNameBox.Visible = false;
            FirstNameBox.Text = Server.HtmlDecode(user.FirstName);
            FirstNameLabel.Text = user.FirstName;

            LastNameBox.Visible = false;
            LastNameBox.Text = Server.HtmlDecode(user.LastName);
            LastNameLabel.Text = user.LastName;

            MIddleNameBox.Visible = false;
            MIddleNameBox.Text = Server.HtmlDecode(user.MiddleName);
            LastNameLabel.Text = user.MiddleName;

            RankLabel.Text = user.Rank.Title;
            RankSelect.Visible = false;
            SetDropdownByValue(RankSelect, user.Rank.Id);

            PasCodeBox.Text = user.Unit.PasCode + " (" + user.Unit.Name + ")";
            PasCodeHidden.Text = user.Unit.PasCode;
            PasCodeLabel.Visible = false;
            SrcUnitIdHdn.Text = user.Unit.Id.ToString();

            if (user.Rank.Id != 0)
            {
                TitleSelect.Visible = false;
                TitleLabel.Visible = false;
            }
        }

        protected void PopulateNonMemberData(AppUser user)
        {
            NameDisplayPanel.Visible = false;
            NameEntryPanel.Visible = true;

            FirstNameLabel.Visible = false;
            LastNameLabel.Visible = false;
            MiddleNameLabel.Visible = false;

            FirstNameBox.Text = Server.HtmlDecode(user.FirstName);
            LastNameBox.Text = Server.HtmlDecode(user.LastName);
            MIddleNameBox.Text = Server.HtmlDecode(user.MiddleName);

            SetDropdownByValue(TitleSelect, user.Title);

            PopulateRanks();
            RankSelect.Visible = true;

            if (user.Rank != null)
            {
                SetDropdownByValue(RankSelect, user.Rank.Id);
            }

            TitleNum.Text = "C";
            FirstNameNum.Text = "D";
            MiddleNameNum.Text = "E";
            LastNameNum.Text = "F";
            RankNumEntry.Text = "G";
            UnitNum.Text = "H";
            UserRoleNum.Text = "I";
            EmailNum.Text = "J";

            EmailNum2.Text = "K";
            Component.Text = "L";
            EmailNum3.Text = "M";

            PhoneNum.Text = "N";
            DSNNum.Text = "O";

            AddressNum.Text = "P";
            CityNum.Text = "Q";
            StateNum.Text = "R";
            ZipNum.Text = "S";
            RecieveEmailNum.Text = "T";
        }

        protected void PopulateRanks()
        {
            RankSelect.DataSource = LookupService.GetRanksAndGrades();
            RankSelect.DataTextField = "Title";
            RankSelect.DataValueField = "Id";
            RankSelect.DataBind();
        }

        protected void PopulateRoles()
        {
            IList<UserGroup> groups = LookupService.GetGroupsByCompo("6");

            RoleSelect.DataSource = from g in groups where g.CanBeRequested == true select g;
            RoleSelect.DataTextField = "Description";
            RoleSelect.DataValueField = "Id";

            RoleSelect.DataBind();
        }

        protected void PopulateStates()
        {
            cbState.DataSource = LookupService.GetStates();
            cbState.DataTextField = "Name";
            cbState.DataValueField = "Id";
            cbState.DataBind();

            cbState.Items.Insert(0, new ListItem("", ""));
        }

        protected void SaveUser()
        {
            AppUser user;

            IUserDao dao = new NHibernateDaoFactory().GetUserDao();

            user = UserService.GetByEDIPIN(_edipin);

            if (user == null)
            {
                user = new AppUser();
            }

            user.EDIPIN = _edipin;
            user.SSN = string.Empty;

            // the user is not a service member, so get all data
            user.FirstName = Server.HtmlEncode(FirstNameBox.Text.Trim());
            user.MiddleName = Server.HtmlEncode(MIddleNameBox.Text.Trim());
            user.LastName = Server.HtmlEncode(LastNameBox.Text.Trim());

            if (TitleSelect.SelectedIndex > 0)
            {
                user.Title = TitleSelect.SelectedValue;
            }
            else
            {
                user.Title = null;
            }

            user.Rank = new UserRank();
            user.Rank.SetId(int.Parse(RankSelect.SelectedValue));

            if (user.Id == 0)
            {
                user.Status = AccessStatus.None;
                user.Username = dao.GetUserName(user.FirstName, user.LastName);
            }

            if (SrcUnitIdHdn.Text != "")
            {
                DomainUnit newUnit = new NHibernateDaoFactory().GetUnitDao().FindById(int.Parse(SrcUnitIdHdn.Text.Trim()));
                if (newUnit != null)
                {
                    user.Unit = newUnit;
                }
            }

            // now get the common data
            user.Phone = Server.HtmlEncode(PhoneBox.Text.Trim());
            user.DSN = Server.HtmlEncode(DsnBox.Text.Trim());

            if (user.Address == null)
            {
                user.Address = new Address();
            }

            user.Address.Street = Server.HtmlEncode(txtAddress.Text.Trim());
            user.Address.City = Server.HtmlEncode(txtCity.Text.Trim());
            user.Address.State = cbState.SelectedValue;
            user.Address.Zip = Server.HtmlEncode(txtZip.Text.Trim());

            user.Component = CompoSelect.SelectedValue;
            user.AccountExpiration = DateTime.Now.AddYears(1);
            user.Email = Server.HtmlEncode(EmailBox.Text.Trim());
            user.Email2 = Server.HtmlEncode(Email2Box.Text.Trim());
            user.Email3 = Server.HtmlEncode(Email3Box.Text.Trim());

            user.ReceiveEmail = chkReceiveEmail.Checked;

            if (user.Status == AccessStatus.None)
            {
                user.CreatedDate = DateTime.Now;
            }

            // create the role
            if (user.CurrentRole == null)
            {
                UserRole role = new UserRole();
                role.Status = AccessStatus.Pending;
                role.Group = new UserGroup(int.Parse(RoleSelect.SelectedValue));
                role.Active = true;
                role.User = user;
                user.AllRoles.Add(role);
                user.CurrentRole = role;
            }
            else
            {
                int newGroup = 0;
                int.TryParse(RoleSelect.SelectedValue, out newGroup);

                if (newGroup != user.CurrentRole.Group.Id)
                {
                    UserGroup group = new NHibernateDaoFactory().GetUserGroupDao().GetById(newGroup);
                    user.CurrentRole.Group = group;
                }
            }

            user.ModifiedDate = DateTime.Now;

            dao.SaveOrUpdate(user);
            dao.CommitChanges();

            if (user.Id > 0)
            {
                Response.Redirect("~/Public/Register3.aspx?", true);
            }
        }

        protected void VerifyAccess()
        {
            if (SESSION_EDIPIN == null || Session["signed"] == null)
            {
                // no ssn on so this is not a legit request
                Response.Redirect("~/Default.aspx");
            }

            _edipin = SESSION_EDIPIN;
        }
    }
}
