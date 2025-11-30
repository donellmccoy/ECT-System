using System;

namespace ALODWebUtility.Workflow
{
    public class SignStartedEventArgs : EventArgs
    {
        private bool _cancel = false;
        private string _comments;
        private byte _dbSign;
        private short _optionId;
        private int _refId;
        private int _secondaryId;
        private int _statusIn;
        private int _statusOut;

        public bool Cancel
        {
            get { return _cancel; }
            set { _cancel = value; }
        }

        public string Comments
        {
            get { return _comments; }
            set { _comments = value; }
        }

        public byte DBSign
        {
            get { return _dbSign; }
            set { _dbSign = value; }
        }

        public short OptionId
        {
            get { return _optionId; }
            set { _optionId = value; }
        }

        public int RefId
        {
            get { return _refId; }
            set { _refId = value; }
        }

        public int SecondaryId
        {
            get { return _secondaryId; }
            set { _secondaryId = value; }
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
    }
}
