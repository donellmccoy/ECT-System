using System;
using System.Data;

namespace ALODWebUtility.Common
{
    public abstract class IntValuedLookupItem
    {
        protected string _name = string.Empty;
        protected int _value = 0;             // Unique identifier of the lookup item
        // Name or description of the lookup item

        #region Properties

        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        public int Value
        {
            get { return _value; }
            set { _value = value; }
        }

        #endregion

        // Get all LookupItem records from the lookup table for the Sys Admin
        public abstract DataTable GetRecords();
    }
}
