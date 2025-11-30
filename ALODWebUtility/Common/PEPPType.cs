using System;
using System.Data;
using ALOD.Core.Domain.Lookup;
using ALOD.Data;

namespace ALODWebUtility.Common
{
    public class PEPPType : IntValuedLookupItem
    {
        protected int _active = 0;            // active

        #region Properties

        public int Active
        {
            get { return _active; }
            set { _active = value; }
        }

        #endregion

        // Get all PEPPType records from the lookup table for the Sys Admin
        public override DataTable GetRecords()
        {
            ILookupDao lkup = new NHibernateDaoFactory().GetLookupDao();
            return lkup.GetPEPPTypes(0, 0).Tables[0];
        }

        // Insert new PEPPType into the lookup table
        public void InsertPEPPType(int Value, string Name, int Active)
        {
            SqlDataStore adapter = new SqlDataStore();
            adapter.ExecuteNonQuery("core_lookups_sp_Insert_PEPPType", Value, Name, Active);
        }

        // Edit/Update an existing PEPPType in the lookup table
        public void UpdatePEPPType(int Value, string Name, int Active)
        {
            SqlDataStore adapter = new SqlDataStore();
            adapter.ExecuteNonQuery("core_lookups_sp_Insert_PEPPType", Value, Name, Active);
        }
    }
}
