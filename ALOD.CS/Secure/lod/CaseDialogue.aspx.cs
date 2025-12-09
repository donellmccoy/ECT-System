using System;
using System.Web.UI;
using ALOD.Core.Domain.Workflow;
using ALOD.Logging;
using ALOD.Secure.Shared.UserControls;
using ALOD.Web.UserControls;

namespace ALOD.Web.LOD
{
    public partial class Secure_lod_CaseDialogue : Page
    {
        #region Properties

        public int refId => int.Parse(Request.QueryString["refId"]);

        protected ModuleType ModuleType => ModuleType.LOD;

        protected TabNavigator Navigator => ((LodMaster)Master).Navigator;

        protected int RequestId => int.Parse(Request.QueryString["refId"]);

        protected TabControls TabControl => ((LodMaster)Master).TabControl;

        #endregion

        #region Load

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                CaseDialogue.Initialize(this, ModuleType, RequestId, Navigator, false, true);

                LogManager.LogAction(ModuleType, UserAction.ViewPage, refId, "Viewed Page: Case Dialogue");
            }
        }

        #endregion
    }
}
