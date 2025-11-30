using System;
using System.ComponentModel;
using System.Web.UI;

namespace SRXLite.Web.Controls
{
    public partial class Controls_ModalDialog : System.Web.UI.UserControl
    {
        private ITemplate _contentTemplate = null;
        private bool _showDialog = false;

        #region Properties

        [PersistenceMode(PersistenceMode.InnerDefaultProperty)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        [TemplateContainer(typeof(ControlContainer))]
        [TemplateInstance(TemplateInstance.Single)]
        public ITemplate ContentTemplate
        {
            get { return _contentTemplate; }
            set { _contentTemplate = value; }
        }

        public string BehaviorID
        {
            get { return ModalPopupExtender1.BehaviorID; }
            set { ModalPopupExtender1.BehaviorID = value; }
        }

        public string PanelClientID
        {
            get { return divContent.ClientID; }
        }

        public string Title
        {
            get { return ModalDialogTitle.InnerText; }
            set { ModalDialogTitle.InnerText = value; }
        }

        public string Height
        {
            get { return pnlContent.Style["height"]; }
            set { pnlContent.Style["height"] = value; }
        }

        public string Width
        {
            get { return pnlContent.Style["width"]; }
            set { pnlContent.Style["width"] = value; }
        }

        #endregion

        #region ControlContainer Class

        public class ControlContainer : Control, INamingContainer
        {
            internal ControlContainer()
            {
            }
        }

        #endregion

        protected void Page_Init(object sender, EventArgs e)
        {
            if (_contentTemplate != null)
            {
                ControlContainer container = new ControlContainer();
                _contentTemplate.InstantiateIn(container);
                phContent.Controls.Add(container);
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                imgClose.Attributes.Add("onclick", "$find('" + this.BehaviorID + "').hide();");
            }
        }

        public void Hide()
        {
            ModalPopupExtender1.Hide();
        }

        public void Show()
        {
            ModalPopupExtender1.Show();
        }
    }
}
