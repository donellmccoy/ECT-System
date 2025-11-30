using System;
using ALOD.Core.Domain.Modules.Lod;
using ALOD.Core.Domain.Workflow;
using ALOD.Data;
using ALOD.Web.UserControls;
using ALODWebUtility.Common;

namespace ALOD.Web.LOD
{
    public partial class Secure_lod_init : System.Web.UI.Page
    {
        private NHibernateDaoFactory _daoFactory;

        public NHibernateDaoFactory DaoFactory
        {
            get
            {
                if (_daoFactory == null)
                {
                    _daoFactory = new NHibernateDaoFactory();
                }

                return _daoFactory;
            }
        }

        public TabNavigator Navigator
        {
            get
            {
                return MasterPage.Navigator();
            }
        }

        protected LodMaster MasterPage
        {
            get
            {
                LodMaster master = (LodMaster)Page.Master;
                return master;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (IsPostBack)
            {
                return;
            }

            int refId = 0;

            PageAccess.AccessLevel userAccess;
            int.TryParse(Request.QueryString["refId"], out refId);

            if (refId == 0)
            {
                SetErrorMessage(Resources.Messages.ERROR_LOADING_CASE);
                Response.Redirect(Resources._Global.StartPage, true);
            }

            int ws_id = SESSION_WS_ID(refId);
            LineOfDuty lod;
            LineOfDutyDao dao = DaoFactory.GetLineOfDutyDao();

            // Make sure the user has access to this case
            switch (ws_id)
            {
                case LodWorkStatus_v3.MedicalOfficerReview_LODV3:
                case LodWorkStatus_v3.MedicalTechReview_LODV3:
                case LodWorkStatus_v3.UnitCommanderReview_LODV3:
                case LodWorkStatus_v3.AppointingAutorityReview_LODV3:
                case LodWorkStatus_v3.WingJA_LODV3:
                    userAccess = dao.GetUserAccess_LOD_V3(SESSION_USER_ID, refId);
                    break;
                default:
                    userAccess = dao.GetUserAccess(SESSION_USER_ID, refId);
                    break;
            }

            Utility.VerifyUserAccess(userAccess, Resources.Messages.ERROR_NO_ACCESS, Resources._Global.StartPage);

            lod = MasterPage.LoadCase(refId, dao);

            Session["RefId"] = refId;

            Utility.UpdateCaseLock(userAccess, refId, ModuleType.LOD);
            MasterPage.InitPageAccess(lod);

            string startPage = MasterPage.GetStartPageTitle(lod.CurrentStatusCode, lod.Formal);

            // Remove the LOD from the DAO so no changes are persisted back
            dao.Evict(lod);

            Navigator.MoveToPage(startPage);
        }
    }
}
