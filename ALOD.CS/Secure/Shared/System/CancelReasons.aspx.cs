using System;
using System.Web.UI.WebControls;
using ALOD.Core.Interfaces.DAOInterfaces;
using ALOD.Data;

namespace ALOD.Web.Sys
{
    public partial class Secure_Shared_System_CancelReasons : System.Web.UI.Page
    {
        protected GridView gdvCancelReasons;

        private ILookupCancelReasonsDao _cancelReasonsDao;

        public ILookupCancelReasonsDao CancelReasonsDao
        {
            get
            {
                if (_cancelReasonsDao == null)
                {
                    _cancelReasonsDao = new NHibernateDaoFactory().GetLookupCancelReasonsDao();
                }

                return _cancelReasonsDao;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                UpdateCancelReasonsGridView();
            }
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            gdvCancelReasons.RowUpdating += gdvCancelReasons_RowUpdating;
            gdvCancelReasons.RowCancelingEdit += gdvCancelReasons_RowCancelingEdit;
            gdvCancelReasons.RowEditing += gdvCancelReasons_RowEditing;
        }

        protected void gdvCancelReasons_RowUpdating(object sender, GridViewUpdateEventArgs e)
        {
            int cancelReasonsId = Convert.ToInt32(gdvCancelReasons.DataKeys[e.RowIndex].Value);
            string cancelReasonsDesc = ((TextBox)gdvCancelReasons.Rows[e.RowIndex].FindControl("txtDescription")).Text;
            int cancelReasonsOrder = int.Parse(((TextBox)gdvCancelReasons.Rows[e.RowIndex].FindControl("txtDisplay")).Text);

            CancelReasonsDao.UpdateCancelReasons(cancelReasonsId, cancelReasonsDesc, cancelReasonsOrder);

            gdvCancelReasons.EditIndex = -1;

            UpdateCancelReasonsGridView();
        }

        protected void gdvCancelReasons_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
        {
            gdvCancelReasons.EditIndex = -1;
            UpdateCancelReasonsGridView();
        }

        protected void gdvCancelReasons_RowEditing(object sender, GridViewEditEventArgs e)
        {
            gdvCancelReasons.EditIndex = e.NewEditIndex;
            UpdateCancelReasonsGridView();
        }

        private void UpdateCancelReasonsGridView()
        {
            gdvCancelReasons.DataSource = CancelReasonsDao.GetAll();
            gdvCancelReasons.DataBind();
        }
    }
}
