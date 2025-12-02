using System;
using ALOD.Core.Domain.Workflow;
// TODO: Convert UserControls - using ALOD.Web.UserControls;

namespace ALOD.Web.LOD
{
    public partial class UnitComments : System.Web.UI.Page
    {
        #region Properties

        protected ModuleType ModuleType
        {
            get
            {
                return ModuleType.LOD;
            }
        }

        // TODO: Convert TabNavigator user control
        // protected TabNavigator Navigator
        // {
        //     get
        //     {
        //         return Master.Navigator;
        //     }
        // }

        protected int RequestId
        {
            get
            {
                return int.Parse(Request.QueryString["refId"]);
            }
        }

        // TODO: Convert TabControls user control
        // protected TabControls TabControl
        // {
        //     get
        //     {
        //         return Master.TabControl;
        //     }
        // }

        #endregion

        #region Load

        protected void Page_Load(object sender, EventArgs e)
        {
            // TODO: Uncomment when CaseComment control is converted
            // if (!Page.IsPostBack)
            // {
            //     CaseComment.Initialize(this, ModuleType, RequestId, Navigator, false);
            // }
        }

        #endregion
    }
}
