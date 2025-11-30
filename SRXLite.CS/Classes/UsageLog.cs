using System;
using System.Data;
using System.Data.SqlClient;
using System.Web;
using SRXLite.DataAccess;
using SRXLite.DataTypes;
using static SRXLite.Modules.Util;
using static SRXLite.DataAccess.DB;

namespace SRXLite.Classes
{
    public class UsageLog
    {
        private HttpContext _context;

        #region Constructors

        /// <summary>
        /// Constructor with HTTP context
        /// </summary>
        /// <param name="context"></param>
        public UsageLog(HttpContext context)
        {
            _context = context;
        }

        #endregion

        #region UsageData

        public class UsageData
        {
            public ActionType? ActionType { get; set; }
            public int? BatchID { get; set; }
            public long? DocID { get; set; }
            public long? DocPageID { get; set; }
            public DocumentStatus? DocStatus { get; set; }
            public long? GroupID { get; set; }
            public string IPAddress { get; set; }
            public int? SubuserID { get; set; }
            public short UserID { get; set; }
        }

        #endregion

        #region Insert

        public void Insert(UsageData data)
        {
            SqlCommand command = new SqlCommand();
            command.CommandText = "dsp_UsageLog_Insert";
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.Add(GetSqlParameter("@UserID", data.UserID));
            command.Parameters.Add(GetSqlParameter("@SubuserID", data.SubuserID));
            command.Parameters.Add(GetSqlParameter("@ActionTypeID", (int?)data.ActionType));
            command.Parameters.Add(GetSqlParameter("@DocStatusID", (byte?)data.DocStatus));
            command.Parameters.Add(GetSqlParameter("@DocID", data.DocID));
            command.Parameters.Add(GetSqlParameter("@DocPageID", data.DocPageID));
            command.Parameters.Add(GetSqlParameter("@GroupID", data.GroupID));
            command.Parameters.Add(GetSqlParameter("@BatchID", data.BatchID));
            command.Parameters.Add(GetSqlParameter("@ClientIP", GetClientIP(_context)));

            DB.ExecuteNonQuery(command);
        }

        #endregion
    }
}
