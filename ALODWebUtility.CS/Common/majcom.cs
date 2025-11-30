using System.Data;
using ALOD.Data;

namespace ALODWebUtility.Common
{
    public class majcom
    {
        protected int _active = 0;
        protected string _name = "";
        protected int _value = 0;

        public string Active
        {
            get
            {
                return _active.ToString();
            }
            set
            {
                _active = int.Parse(value);
            }
        }

        public string Name
        {
            get
            {
                return _name;
            }
            set
            {
                _name = value;
            }
        }

        public int Value
        {
            get
            {
                return _value;
            }
            set
            {
                _value = value;
            }
        }

        // Get all for Sys Admin
        public DataTable GetMAJCOM()
        {
            SqlDataStore sqlDS = new SqlDataStore();
            DataSet ds;
            DataTable dt = new DataTable();
            ds = sqlDS.ExecuteDataSet("core_lookUps_sp_GetMAJCOM");
            dt = ds.Tables[0];

            return dt;
        }

        // Get Active only for user
        public DataTable GetMAJCOM(int Value, int filter)
        {
            SqlDataStore sqlDS = new SqlDataStore();
            DataSet ds;
            DataTable dt = new DataTable();
            ds = sqlDS.ExecuteDataSet("core_lookUps_sp_GetMAJCOM", Value, filter);
            dt = ds.Tables[0];

            return dt;
        }

        // Add or Edit
        public void InsertMAJCOM(int Value, string Name, int Active)
        {
            SqlDataStore adapter = new SqlDataStore();
            adapter.ExecuteNonQuery("core_lookups_sp_Insert_MAJCOM", Value, Name, Active);
        }

        public void UpdateMAJCOM(int Value, string Name, int Active)
        {
            SqlDataStore adapter = new SqlDataStore();
            adapter.ExecuteNonQuery("core_lookups_sp_Insert_MAJCOM", Value, Name, Active);
        }
    }
}
