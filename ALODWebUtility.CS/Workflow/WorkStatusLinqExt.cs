using System;

namespace ALODWebUtility.Workflow
{
    public class WorkStatusLinqExt
    {
        private bool _optionValid;
        private bool _optionVisible;

        public bool OptionValid
        {
            get { return _optionValid; }
            set { _optionValid = value; }
        }

        public bool OptionVisible
        {
            get { return _optionVisible; }
            set { _optionVisible = value; }
        }
    }
}
