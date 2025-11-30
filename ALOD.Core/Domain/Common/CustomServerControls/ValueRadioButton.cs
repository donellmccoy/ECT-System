using System;
using System.ComponentModel;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ALOD.Core.Domain.Common.CustomServerControls
{
    [DefaultProperty("Text")]
    [ToolboxData("<{0}:ValueRadioButton runat=server></{0}:ValueRadioButton>")]
    public class ValueRadioButton : RadioButton
    {
        [Bindable(true)]
        [Category("Appearances")]
        [DefaultValue("")]
        [Localizable(true)]
        public string Value
        {
            get
            {
                String s = (String)ViewState["Value"];
                return ((s == null) ? "" : s);
            }

            set
            {
                ViewState["Value"] = value;
                this.Attributes["value"] = value;
            }
        }
    }
}