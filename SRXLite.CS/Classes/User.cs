using System;
using System.Data;
using System.Data.SqlClient;
using System.Web.Services.Protocols;
using SRXLite.DataAccess;
using static SRXLite.Modules.Util;
using static SRXLite.DataAccess.DB;

namespace SRXLite.Classes
{
    /// <summary>
    /// SOAP header for service authentication
    /// </summary>
    public class ServiceLogin : SoapHeader
    {
        public string Password { get; set; }
        public string SubuserName { get; set; }
        public string UserName { get; set; }
    }

    /// <summary>
    /// Service user with authentication
    /// </summary>
    public class ServiceUser : User
    {
        private bool _authenticated = false;

        public ServiceUser()
        {
        }

        #region Properties

        public bool IsAuthenticated
        {
            get { return _authenticated; }
        }

        #endregion

        #region Authenticate

        /// <summary>
        /// Validates the user's credentials (username/password). Throws an
        /// UnauthorizedAccessException if authentication fails.
        /// </summary>
        /// <param name="login"></param>
        public void Authenticate(ServiceLogin login)
        {
            if (_authenticated) return;

            if (login == null || login.UserName == null || login.Password == null)
            {
                throw new UnauthorizedAccessException("Login information is missing.");
            }

            // Validate username/password
            // TODO: store password securely with hash/salt?
            SqlCommand command = new SqlCommand();
            command.CommandText = "dsp_User_Authenticate";
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.Add(GetSqlParameter("@UserName", login.UserName));
            command.Parameters.Add(GetSqlParameter("@Password", login.Password));
            command.Parameters.Add(GetSqlParameter("@SubuserName", login.SubuserName));
            command.Parameters.Add(GetSqlParameter("@UserID", null, SqlDbType.SmallInt, ParameterDirection.Output));
            command.Parameters.Add(GetSqlParameter("@SubuserID", null, SqlDbType.Int, ParameterDirection.Output));

            DB.ExecuteNonQuery(command);

            short userID = ShortCheck(command.Parameters["@UserID"].Value);
            int subuserID = IntCheck(command.Parameters["@SubuserID"].Value);

            // Check for valid userID
            if (userID == 0)
            {
                throw new UnauthorizedAccessException(string.Format("Login failed for user \"{0}\".", login.UserName));
            }

            // User validated, set properties
            this._userID = userID;
            this._userName = login.UserName;
            this._authenticated = true;
            this._subuserID = subuserID;
            this._subuserName = login.SubuserName;
        }

        #endregion
    }

    /// <summary>
    /// User class
    /// </summary>
    public class User
    {
        protected int _subuserID;
        protected string _subuserName;
        protected short _userID;
        protected string _userName;

        #region Properties

        public int SubuserID
        {
            get { return _subuserID; }
        }

        public string SubuserName
        {
            get { return _subuserName; }
        }

        public short UserID
        {
            get { return _userID; }
        }

        public string UserName
        {
            get { return _userName; }
        }

        #endregion

        #region Constructors

        public User()
        {
        }

        /// <summary>
        /// Constructor with user ID
        /// </summary>
        /// <param name="userID"></param>
        public User(short userID)
        {
            _userID = userID;
        }

        /// <summary>
        /// Constructor with user ID and subuser ID
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="subuserID"></param>
        public User(short userID, int subuserID)
        {
            _userID = userID;
            _subuserID = subuserID;
        }

        #endregion
    }
}
