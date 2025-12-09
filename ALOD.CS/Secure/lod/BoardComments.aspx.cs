using System;
using System.Web.UI;
using ALOD.Core.Domain.Workflow;
using ALOD.Web.UserControls;

namespace ALOD.Web.LOD
{
    public partial class Secure_lod_BoardComments : Page
    {
        #region Properties

        protected ModuleType ModuleType
        {
            get { return ModuleType.LOD; }
        }

        protected TabNavigator Navigator
        {
            get { return Master.Navigator; }
        }

        protected int RequestId
        {
            get { return int.Parse(Request.QueryString["refId"]); }
        }

        protected TabControls TabControl
        {
            get { return Master.TabControl; }
        }

        #endregion

        #region Load

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                CaseComment.Initialize(this, ModuleType, RequestId, Navigator, true);
            }
        }

        #endregion
    }
}
