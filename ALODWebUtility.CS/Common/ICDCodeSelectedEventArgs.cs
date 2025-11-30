using System;

namespace ALODWebUtility.Common
{
    public class ICDCodeSelectedEventArgs : EventArgs
    {
        private int _selectedDropDownLevel;
        private string _selectedICD7thCharacter;
        private int _selectedICDCodeId;

        public int SelectedDropDownLevel
        {
            get { return _selectedDropDownLevel; }
            set { _selectedDropDownLevel = value; }
        }

        public string SelectedICD7thCharacter
        {
            get { return _selectedICD7thCharacter; }
            set { _selectedICD7thCharacter = value; }
        }

        public int SelectedICDCodeId
        {
            get { return _selectedICDCodeId; }
            set { _selectedICDCodeId = value; }
        }
    }
}
