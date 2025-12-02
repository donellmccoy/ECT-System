using System;
using ALOD.Core.Domain.Modules.Lod;
using ALOD.Core.Domain.Workflow;
using ALOD.Core.Interfaces.DAOInterfaces;
using ALOD.Data;
// TODO: Convert UserControls - using ALOD.Web.UserControls;
using ALODWebUtility.Common;
using static ALODWebUtility.Common.SessionInfo;
using static ALODWebUtility.Common.Utility;

namespace ALOD.Web.LOD
{
    public partial class Secure_lod_init : System.Web.UI.Page
    {
        // Resource string constants - normally from App_GlobalResources
        private const string ERROR_LOADING_CASE = "An error has occurred loading the requested case";
        private const string ERROR_NO_ACCESS = "You do not have access to view the requested case";
        private const string START_PAGE = "~/Secure/lod/Inbox.aspx";

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

        // TODO: Convert TabNavigator user control
        // public TabNavigator Navigator
        // {
        //     get
        //     {
        //         return MasterPage.Navigator();
        //     }
        // }

        // TODO: Convert LodMaster master page
        // protected LodMaster MasterPage
        // {
        //     get
        //     {
        //         LodMaster master = (LodMaster)Page.Master;
        //         return master;
        //     }
        // }

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
                SetErrorMessage(ERROR_LOADING_CASE);
                Response.Redirect(START_PAGE, true);
            }

            // TODO: SESSION_WS_ID requires refId lookup - implement when MasterPage is ready
            // int ws_id = SESSION_WS_ID(refId);
            LineOfDuty lod;
            ILineOfDutyDao dao = DaoFactory.GetLineOfDutyDao();

            // TODO: Implement status-based access check when MasterPage is ready
            // Make sure the user has access to this case
            // switch (ws_id)
            // {
            //     case (int)LodWorkStatus_v3.MedicalOfficerReview_LODV3:
            //     case (int)LodWorkStatus_v3.MedicalTechReview_LODV3:
            //     case (int)LodWorkStatus_v3.UnitCommanderReview_LODV3:
            //     case (int)LodWorkStatus_v3.AppointingAutorityReview_LODV3:
            //     case (int)LodWorkStatus_v3.WingJA_LODV3:
            //         userAccess = dao.GetUserAccess_LOD_V3(SESSION_USER_ID, refId);
            //         break;
            //     default:
            //         userAccess = dao.GetUserAccess(SESSION_USER_ID, refId);
            //         break;
            // }
            userAccess = dao.GetUserAccess(SESSION_USER_ID, refId);

            Utility.VerifyUserAccess(userAccess, ERROR_NO_ACCESS, START_PAGE);

            // TODO: MasterPage functions require conversion
            // lod = MasterPage.LoadCase(refId, dao);

            Session["RefId"] = refId;

            Utility.UpdateCaseLock(userAccess, refId, ModuleType.LOD);
            // MasterPage.InitPageAccess(lod);

            // string startPage = MasterPage.GetStartPageTitle(lod.CurrentStatusCode, lod.Formal);

            // Remove the LOD from the DAO so no changes are persisted back
            // dao.Evict(lod);

            // Navigator.MoveToPage(startPage);
        }
    }
}
