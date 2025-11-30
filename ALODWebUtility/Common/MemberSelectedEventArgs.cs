using System;

namespace ALODWebUtility.Common
{
    public class MemberSelectedEventArgs : EventArgs
    {
        private int _selectedRowIndex;

        public int SelectedRowIndex
        {
            get { return _selectedRowIndex; }
            set { _selectedRowIndex = value; }
        }
    }
}
