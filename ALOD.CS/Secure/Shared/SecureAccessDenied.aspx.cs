using System;
using System.Collections.Generic;
using System.Web.UI.WebControls;
using ALOD.Core.Domain.Common.KeyVal;
using ALOD.Core.Interfaces.DAOInterfaces;
using ALOD.Core.Utils;
using ALOD.Data;

namespace ALOD.Web
{
    public partial class SecureAccessDenied : System.Web.UI.Page
    {
        protected Label lblDeniedSubject;
        protected Label lblAdditionalComments;

        private IKeyValDao _keyValDao;

        protected IKeyValDao KeyValDao
        {
            get
            {
                if (_keyValDao == null)
                {
                    _keyValDao = new NHibernateDaoFactory().GetKeyValDao();
                }
                return _keyValDao;
            }
        }

        protected string AdditionalComments
        {
            get { return lblAdditionalComments.Text; }
            set { lblAdditionalComments.Text = value; }
        }

        protected string DeniedSubject
        {
            get { return lblDeniedSubject.Text; }
            set { lblDeniedSubject.Text = value; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                int deniedType = 0;
                int.TryParse(Request.QueryString["deniedType"], out deniedType);

                InitData(deniedType);
            }
        }

        private void InitData(int deniedType)
        {
            switch (deniedType)
            {
                case (int)AccessDeniedType.CaseDetails:
                    IList<KeyValValue> values = KeyValDao.GetKeyValuesByKeyDesciption("Access Denied - Case Details");
                    DeniedSubject = values[0].Value;
                    AdditionalComments = values[1].Value;
                    break;
                default:
                    DeniedSubject = "You do not have access to this page for unknown reasons.";
                    AdditionalComments = "Please contact the help desk for further assistance.";
                    break;
            }
        }
    }
}
