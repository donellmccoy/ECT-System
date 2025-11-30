using System;
using System.Data;
using ALOD.Core.Domain.Lookup;
using ALOD.Data;

namespace ALODWebUtility.Common
{
    public class PEPPDisposition : IntValuedLookupItem
    {
        protected int _active = 0;            // active

        #region Properties

        public int Active
        {
            get { return _active; }
            set { _active = value; }
        }

        #endregion

        // Get all PEPPDisposition records from the lookup table for the Sys Admin
        public override DataTable GetRecords()
        {
            ILookupDao lkup = new NHibernateDaoFactory().GetLookupDao();
            return lkup.GetPEPPDispositions(0, 0).Tables[0];
        }

        // Insert new PEPPDisposition into the lookup table
        public void InsertPEPPDisposition(int Value, string Name, int Active)
        {
            SqlDataStore adapter = new SqlDataStore();
            adapter.ExecuteNonQuery("core_lookups_sp_Insert_PEPPDisposition", Value, Name, Active);
        }

        // Edit/Update an existing PEPPDisposition in the lookup table
        public void UpdatePEPPDisposition(int Value, string Name, int Active)
        {
            SqlDataStore adapter = new SqlDataStore();
            adapter.ExecuteNonQuery("core_lookups_sp_Insert_PEPPDisposition", Value, Name, Active);
        }
    }
}
