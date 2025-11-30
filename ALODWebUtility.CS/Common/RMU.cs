using System;
using System.Data;
using ALOD.Data;

namespace ALODWebUtility.Common
{
    public class RMU : IntValuedLookupItem
    {
        //Protected _csId As Integer
        protected string _pasCode;

        //Public Property CSId As Integer
        //    Get
        //        Return _csId
        //    End Get
        //    Set(value As Integer)
        //        _csId = value
        //    End Set
        //End Property

        public string PAS
        {
            get { return _pasCode; }
            set { _pasCode = value; }
        }

        // Get all RMU records from the lookup table for the Sys Admin
        public override DataTable GetRecords()
        {
            SqlDataStore sqlDS = new SqlDataStore();
            return sqlDS.ExecuteDataSet("core_lookUps_sp_RMUNames").Tables[0];
        }

        // Insert new RMU into the lookup table
        public void InsertRMU(int Value, string Name, string PAS, int Collocated)
        {
            SqlDataStore adapter = new SqlDataStore();
            adapter.ExecuteNonQuery("core_lookups_sp_Insert_RMU", Value, Name, PAS, Collocated);
        }

        // Edit/Update an existing RMU in the lookup table
        public void UpdateRMU(int Value, string Name, string PAS, int Collocated)
        {
            SqlDataStore adapter = new SqlDataStore();
            adapter.ExecuteNonQuery("core_lookups_sp_Insert_RMU", Value, Name, PAS, Collocated);
        }
    }
}
