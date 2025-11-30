using System;
using System.Data;
using ALOD.Core.Domain.Lookup;
using ALOD.Data;

namespace ALODWebUtility.Common
{
    public class PEPPRating : IntValuedLookupItem
    {
        protected int _active = 0;            // active

        #region Properties

        public int Active
        {
            get { return _active; }
            set { _active = value; }
        }

        #endregion

        // Get all PEPPRating records from the lookup table for the Sys Admin
        public override DataTable GetRecords()
        {
            ILookupDao lkup = new NHibernateDaoFactory().GetLookupDao();
            return lkup.GetPEPPRatings(0, 0).Tables[0];
        }

        // Insert new PEPPRating into the lookup table
        public void InsertPEPPRating(int Value, string Name, int Active)
        {
            SqlDataStore adapter = new SqlDataStore();
            adapter.ExecuteNonQuery("core_lookups_sp_Insert_PEPPRating", Value, Name, Active);
        }

        // Edit/Update an existing PEPPRating in the lookup table
        public void UpdatePEPPRating(int Value, string Name, int Active)
        {
            SqlDataStore adapter = new SqlDataStore();
            adapter.ExecuteNonQuery("core_lookups_sp_Insert_PEPPRating", Value, Name, Active);
        }
    }
}
