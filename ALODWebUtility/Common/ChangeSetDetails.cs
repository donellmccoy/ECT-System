using System;
using System.Data;
using ALOD.Data;

namespace ALODWebUtility.Common
{
    public class ChangeSetDetails
    {
        private DateTime? _actionDate;
        private string _firstName;
        private string _lastName;
        private int _logId;
        private string _rank;
        private int _userId;

        public DateTime? ActionDate
        {
            get { return _actionDate; }
            set { _actionDate = value; }
        }

        public string FirstName
        {
            get { return _firstName; }
            set { _firstName = value; }
        }

        public string FullNameRank
        {
            get
            {
                if (!string.IsNullOrEmpty(_rank) && _rank.ToLower() != "civ")
                {
                    return _rank + " " + _lastName + ", " + _firstName;
                }

                if (!string.IsNullOrEmpty(_lastName) && !string.IsNullOrEmpty(_firstName))
                {
                    return _lastName + ", " + _firstName;
                }

                return null;
            }
        }

        public string LastName
        {
            get { return _lastName; }
            set { _lastName = value; }
        }

        public int LogId
        {
            get { return _logId; }
            set { _logId = value; }
        }

        public string Rank
        {
            get { return _rank; }
            set { _rank = value; }
        }

        public int UserId
        {
            get { return _userId; }
            set { _userId = value; }
        }

        public ChangeSetDetails GetLastChange(int id)
        {
            SqlDataStore adapter = new SqlDataStore();
            adapter.ExecuteReader(LastChagedReader, "core_log_sp_GetLastChange", id);
            return this;
        }

        protected void LastChagedReader(SqlDataStore adapter, IDataReader reader)
        {
            this.LogId = adapter.GetInt32(reader, 0);
            this.ActionDate = adapter.GetNullableDate(reader, 1, null);
            this.UserId = adapter.GetInt32(reader, 2);
            this.LastName = adapter.GetString(reader, 3);
            this.FirstName = adapter.GetString(reader, 4);
            this.Rank = adapter.GetString(reader, 5);
        }
    }
}
