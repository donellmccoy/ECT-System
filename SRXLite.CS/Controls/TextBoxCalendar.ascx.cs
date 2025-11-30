using System;
using System.Web.UI.WebControls;
using static SRXLite.Modules.Util;

namespace SRXLite.Web.Controls
{
    public partial class Controls_TextBoxCalendar : System.Web.UI.UserControl
    {
        private DateTime _selectedDate;
        private string _textBoxOnChange;
        private string _textBoxValue;

        #region Properties

        public DateTime SelectedDate
        {
            get { return DateCheck(txtDate.Text, DateTime.MinValue); }
            set { txtDate.Text = value.ToShortDateString(); }
        }

        public TextBox TextBox
        {
            get { return txtDate; }
        }

        public string TextBoxOnChange
        {
            get { return _textBoxOnChange; }
            set
            {
                _textBoxOnChange = value;
                txtDate.Attributes.Add("onchange", _textBoxOnChange);
            }
        }

        public string Value
        {
            get { return txtDate.Text.Trim(); }
            set { txtDate.Text = value; }
        }

        #endregion

        protected void Page_Load(object sender, EventArgs e)
        {
        }

        protected void txtDate_TextChanged(object sender, EventArgs e)
        {
        }
    }
}
