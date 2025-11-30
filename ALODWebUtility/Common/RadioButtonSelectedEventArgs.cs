using System;

namespace ALODWebUtility.Common
{
    public class RadioButtonSelectedEventArgs : EventArgs
    {
        private string _buttonText;
        private string _buttonValue;

        public RadioButtonSelectedEventArgs()
        {
            _buttonValue = string.Empty;
            _buttonText = string.Empty;
        }

        public string ButtonText
        {
            get { return _buttonText; }
            set { _buttonText = value; }
        }

        public string ButtonValue
        {
            get { return _buttonValue; }
            set { _buttonValue = value; }
        }
    }
}
