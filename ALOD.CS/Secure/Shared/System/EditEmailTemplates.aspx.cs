using System;
using System.Web.UI.WebControls;
using ALOD.Core.Domain.Common;
using ALOD.Core.Interfaces.DAOInterfaces;
using ALOD.Data;

namespace ALOD.Web.Sys
{
    public partial class Secure_Shared_System_EditEmailTemplates : System.Web.UI.Page
    {
        protected Label lblMsg;
        protected TextBox txtTitle;
        protected TextBox txtSubject;
        protected TextBox txtBody;
        protected TextBox txtDataSrc;
        protected CheckBox chkActive;
        protected Button btnAdd;
        protected Button btnUpdate;
        protected Button btnCancel;
        protected CustomValidator CustomValidator1;

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            btnAdd.Click += btnAdd_Click;
            btnUpdate.Click += btnUpdate_Click;
            btnCancel.Click += btnCancel_Click;
            CustomValidator1.ServerValidate += BodyMultiLineValidator;
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                SetControls();
            }
        }

        /// <summary>
        /// Description: fills EmailTemplate class from web controls and then pass the class to UpdateEmailTemplateInfo subroutine
        /// </summary>
        public void FillEmailTemplateClass()
        {
            IDaoFactory factory = new NHibernateDaoFactory();
            IEmailTemplateDao dao = factory.GetEmailTemplateDao();

            EmailTemplate email;

            if (Request.QueryString["id"] != null)
            {
                email = dao.GetById(Convert.ToInt32(Request.QueryString["id"]));
            }
            else
            {
                email = new EmailTemplate();
            }

            email.Subject = txtSubject.Text;
            email.Title = txtTitle.Text;
            email.Body = txtBody.Text;
            email.DataProc = txtDataSrc.Text;
            email.Active = chkActive.Checked;
            email.Date = DateTime.Now;
            email.Compo = 6;

            dao.SaveOrUpdate(email);

            Response.Redirect("~/Secure/Shared/System/ManageEmailTemplate.aspx");
        }

        /// <summary>
        /// Called by: SetControls
        /// Description: Loads email template info
        /// </summary>
        /// <param name="id"></param>
        public void LoadEmailTemplateInfo(int id)
        {
            IDaoFactory factory = new NHibernateDaoFactory();
            IEmailTemplateDao dao = factory.GetEmailTemplateDao();

            EmailTemplate email = dao.GetById(id);

            txtSubject.Text = Server.HtmlEncode(email.Subject);
            txtTitle.Text = Server.HtmlEncode(email.Title);
            txtBody.Text = Server.HtmlEncode(email.Body);
            txtDataSrc.Text = Server.HtmlEncode(email.DataProc);
            chkActive.Checked = email.Active;
        }

        public void RedirectPage()
        {
            Response.Redirect("~/Secure/Shared/System/ManageEmailTemplate.aspx", true);
        }

        /// <summary>
        /// This subroutine called on page load to fill web controls from database and
        /// displays appropriate buttons based on query string status.
        /// </summary>
        public void SetControls()
        {
            int id;
            if (Request.QueryString["id"] == null)
            {
                ShowButton(btnAdd);
            }
            else
            {
                id = Convert.ToInt32(Request.QueryString["id"]);
                LoadEmailTemplateInfo(id);
                ShowButton(btnUpdate);
            }
        }

        public void ShowButton(Button btn)
        {
            btnAdd.Visible = false;
            btnUpdate.Visible = false;
            btn.Visible = true;
        }

        protected void BodyMultiLineValidator(object source, ServerValidateEventArgs args)
        {
            if (txtBody.Text.Length < 2001)
            {
                args.IsValid = true;
            }
            else
            {
                args.IsValid = false;
            }
        }

        protected void btnAdd_Click(object sender, EventArgs e)
        {
            if (Page.IsValid)
            {
                FillEmailTemplateClass();
            }
        }

        protected void btnUpdate_Click(object sender, EventArgs e)
        {
            if (Page.IsValid)
            {
                FillEmailTemplateClass();
            }
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            RedirectPage();
        }
    }
}
