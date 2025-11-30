using System;
using System.Data;
using ALOD.Data;

namespace ALODWebUtility.Perms
{
    public class PermRequest
    {
        protected string _akoId;
        protected DateTime _dateReq;
        protected bool _grant = false;
        protected int _permId;
        protected int _requestId;
        protected int _userId;
        protected string _userName;

        public string AkoId
        {
            get
            {
                return _akoId;
            }
            set
            {
                _akoId = value;
            }
        }

        public DateTime DateRequested
        {
            get
            {
                return _dateReq;
            }
            set
            {
                _dateReq = value;
            }
        }

        public bool Granted
        {
            get
            {
                return _grant;
            }
            set
            {
                _grant = value;
            }
        }

        public int PermissionId
        {
            get
            {
                return _permId;
            }
            set
            {
                _permId = value;
            }
        }

        public int RequestId
        {
            get
            {
                return _requestId;
            }
            set
            {
                _requestId = value;
            }
        }

        public int UserId
        {
            get
            {
                return _userId;
            }
            set
            {
                _userId = value;
            }
        }

        public string UserName
        {
            get
            {
                return _userName;
            }
            set
            {
                _userName = value;
            }
        }

        public void GetByUserId(int userId, short permId)
        {
            _permId = permId;
            _userId = userId;

            SqlDataStore adapter = new SqlDataStore();
            adapter.ExecuteReader(RequestReader, "core_permissions_sp_getRequestForUser", userId, permId);
        }

        public bool Grant(int userId)
        {
            SqlDataStore adapter = new SqlDataStore();
            return Convert.ToInt32(adapter.ExecuteNonQuery("core_permissions_sp_GrantUserPermission", userId, _permId)) > 0;
        }

        public bool Save(int userId)
        {
            if (_permId == 0)
            {
                return false;
            }

            SqlDataStore adapter = new SqlDataStore();
            return Convert.ToInt32(adapter.ExecuteNonQuery("core_permissions_sp_insertRequest", userId, _permId)) > 0;
        }

        protected void RequestReader(SqlDataStore adapter, IDataReader reader)
        {
            UserId = adapter.GetInt32(reader, 0);
            AkoId = adapter.GetString(reader, 1);
            UserName = adapter.GetString(reader, 2) + " " + adapter.GetString(reader, 3) + ", " + adapter.GetString(reader, 4);
            DateRequested = adapter.GetDateTime(reader, 5);
            RequestId = adapter.GetInt32(reader, 6);
            PermissionId = adapter.GetInt16(reader, 7);
        }
    }
}
