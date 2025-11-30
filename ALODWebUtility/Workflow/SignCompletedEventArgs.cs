using System;

namespace ALODWebUtility.Workflow
{
    public class SignCompletedEventArgs : EventArgs
    {
        private string _comments;
        private int _optionId;
        private bool _redirect = true;
        private int _refId;
        private int _statusIn;
        private int _statusOut;
        private bool _successful = false;
        private string _text;
        private string _url = "";

        public string Comments
        {
            get { return _comments; }
            set { _comments = value; }
        }

        public int OptionId
        {
            get { return _optionId; }
            set { _optionId = value; }
        }

        public bool Redirect
        {
            get { return _redirect; }
            set { _redirect = value; }
        }

        public int RefId
        {
            get { return _refId; }
            set { _refId = value; }
        }

        public bool SignaturePassed
        {
            get { return _successful; }
            set { _successful = value; }
        }

        public int StatusIn
        {
            get { return _statusIn; }
            set { _statusIn = value; }
        }

        public int StatusOut
        {
            get { return _statusOut; }
            set { _statusOut = value; }
        }

        public string Text
        {
            get { return _text; }
            set { _text = value; }
        }

        public string Url
        {
            get { return _url; }
            set { _url = value; }
        }
    }
}
