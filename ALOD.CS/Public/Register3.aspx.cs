using System;
using System.Linq;
using ALOD.Core.Domain.Users;
using ALOD.Core.Interfaces.DAOInterfaces;
using ALOD.Data;
using ALOD.Data.Services;
using ALODWebUtility.Common;

namespace ALOD.Web
{
    public partial class Public_Register3 : System.Web.UI.Page
    {
        protected string _edipin = "";

        protected void NextButton_Click(object sender, EventArgs e)
        {
            IUserDao dao = new NHibernateDaoFactory().GetUserDao();
            AppUser user = dao.GetByEDIPIN(_edipin);

            if (user == null)
            {
                return;
            }

            // set user status to pending after user confirms information
            user.Status = (AccessStatus)2;
            dao.SaveOrUpdate(user);
            UserService.SendAccountRegisteredEmail(user.Component, user.Username, user.CurrentRoleName, user.CurrentUnitId, GetHostName());
            Response.Redirect("~/public/register4.aspx", true);
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            VerifyAccess();
            populatedata();
        }

        protected void populatedata()
        {
            AppUser user = UserService.GetByEDIPIN(_edipin);

            FullName.Text = user.FullName;
            RankDisplay.Text = user.Rank.Title;
            UnitLabel.Text = user.Unit.Name + " (" + user.Unit.PasCode + ")";

            if (user.AllRoles.Count > 1)
            {
                foreach (UserRole role in user.AllRoles)
                {
                    UserRoleLabel.Text = role.Group.Description + ", " + UserRoleLabel.Text;
                }
            }
            else
            {
                if (user.AllRoles.Count > 0)
                {
                    UserRoleLabel.Text = user.AllRoles[0].Group.Description;
                }
            }

            EmailLabel.Text = user.Email;
            Email2Label.Text = user.Email2;
            Email3Label.Text = user.Email3;
            PhoneLabel.Text = user.Phone;
            DsnLabel.Text = user.DSN;

            AddressLabel.Text = user.Address.Street;
            CityLabel.Text = user.Address.City;
            StateLabel.Text = user.Address.State;
            ZipLabel.Text = user.Address.Zip;

            if (user.ReceiveEmail)
            {
                RecieveEmailLabel.Text = "Yes";
            }
            else
            {
                RecieveEmailLabel.Text = "No";
            }
        }

        protected void PrevButton_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/Public/Register2.aspx", true);
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
