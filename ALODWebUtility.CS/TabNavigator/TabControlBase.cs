using System.Web.UI;

namespace ALODWebUtility.TabNavigation
{
    public abstract class TabControlBase : UserControl
    {
        public abstract bool BackEnabled { get; set; }
        public abstract bool NextEnabled { get; set; }
        public abstract System.Web.UI.WebControls.Button this[ALOD.Core.Domain.Common.NavigatorButtonType type] { get; }
    }
}
