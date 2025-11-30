using ALOD.Core.Domain.Users;
using ALOD.Core.Utils;
using NHibernate;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;

namespace ALOD.Data.Services
{
    /// <summary>
    /// Service for managing military unit operations including unit chains and unit hierarchies.
    /// Provides functionality for unit data access, chain of command management, and unit relationships.
    /// </summary>
    public class UnitService
    {
        private const string ASYNCH = ";async=true";
        private const string CSID = "@cs_id";
        private const string KEY_CONNECTIONSTRING_NAME = "lod";
        private const string SP_UPDATE_CHAIN = "cmdStruct_sp_UpdateAffected";
        private const string USERID = "@userId";

        ///<summary>
        ///<para>The method returns the child units for given view type and parent unit
        ///</para>
        ///<param name="cs_id">parentunitId</param>
        ///<param name="view">chainType</param>
        ///</summary>
        ///<returns>dataset</returns>
        public static DataSet GetChildChain(int cs_id, Byte view)
        {
            SqlDataStore DataSource = new SqlDataStore();

            DbCommand cmd = DataSource.GetSqlStringCommand("Select tree.child_id as Id, tree.child_pas as Pascode,cs.Long_Name as Name from command_struct_tree tree left join command_struct cs   on  tree.child_id= cs.cs_id  where tree.parent_id = @parentUnit and tree.view_type=@viewType  ORDER BY  tree.child_pas");
            DataSource.AddInParameter(cmd, "@parentUnit", DbType.Int32, cs_id);
            DataSource.AddInParameter(cmd, "@viewType", DbType.Byte, view);
            DataSet ds = DataSource.ExecuteDataSet(cmd);
            return ds;
        }

        public static Dictionary<int, String> GetParentChain(int cs_id, int view, string unitName)
        {
            SqlDataStore DataSource = new SqlDataStore();
            Dictionary<int, String> cmdChain = new Dictionary<int, string>();
            SqlDataStore.RowReader rowReader = (SqlDataStore adapter, IDataReader reader) =>
            {
                cmdChain.Add(adapter.GetInteger(reader, 0), adapter.GetString(reader, 1));
            };
            DataSource.ExecuteReader(rowReader, "core_pascodes_GetParentChain", cs_id, view);

            if (cmdChain.Count < 1)
            {
                cmdChain.Add(cs_id, unitName);
            }
            return cmdChain;
        }

        //Used for getting chain on EditPaseCode
        public static DataSet GetReportingUnits(int csId)
        {
            SqlDataStore DataSource = new SqlDataStore();
            return DataSource.ExecuteDataSet("core_pascode_sp_GetReporting", csId);
        }

        //Used on Add Memeber Page to srch for member unit
        public static Unit GetUnitByID(int cs_id)
        {
            ISession session = NHibernateSessionManager.Instance.GetSession();

            IList<Unit> list = session.CreateQuery(
                "from Unit   cs where cs.id=:cs_id")
                .SetInt32("cs_id", cs_id)
                .List<Unit>();

            if (list.Count > 0)
            {
                return list[0];
            }
            else
            {
                return null;
            }
        }

        //Used to search for existing unit on create unit
        public static Unit GetUnitByPasCode(string pascode)
        {
            Check.Require(pascode.Length == 4, "Invalid pascode length");

            ISession session = NHibernateSessionManager.Instance.GetSession();

            IList<Unit> list = session.CreateQuery(
                "from Unit   cs where cs.PasCode=:PasCode")
                .SetString("PasCode", pascode)
                .List<Unit>();

            if (list.Count > 0)
            {
                return list[0];
            }
            else
            {
                return null;
            }
        }

        public static DataSet GetUnitLevelTypes()
        {
            SqlDataStore DataSource = new SqlDataStore();
            DbCommand cmd = DataSource.GetSqlStringCommand("Select * from core_lkupUnitLevelType");
            return DataSource.ExecuteDataSet(cmd);
        }

        public static DataSet GetUnitOperationTypes()
        {
            SqlDataStore DataSource = new SqlDataStore();
            DbCommand cmd = DataSource.GetSqlStringCommand("Select * from core_lkupOperationType");
            return DataSource.ExecuteDataSet(cmd);
        }

        //Used for drop downs on Edit and CreateUnit
        public static DataSet GetUnits()
        {
            SqlDataStore DataSource = new SqlDataStore();
            DbCommand cmd = DataSource.GetSqlStringCommand("Select CS_ID, LONG_NAME from command_struct where Inactive=0");
            return DataSource.ExecuteDataSet(cmd);
        }

        //Used to search for existing unit on create unit
        public static IList<Unit> GetUnitsByPasCode(string pascode)
        {
            Check.Require(pascode.Length == 4, "Invalid pascode length");

            ISession session = NHibernateSessionManager.Instance.GetSession();

            IList<Unit> list = session.CreateQuery(
                "from Unit   cs where cs.PasCode=:PasCode")
                .SetString("PasCode", pascode)
                .List<Unit>();

            return list;
        }

        public static void SetDefaultChain(Unit unit)
        {
            SqlDataStore dataSource = new SqlDataStore();
            dataSource.ExecuteNonQuery("core_pascode_sp_SetDefalutChain", unit.Id, unit.PasCode, UserService.CurrentUser().Id);
        }

        public static void UpdateAffectedUnits(int unitId, int userId)
        {
            string dsn = ConfigurationManager.ConnectionStrings[KEY_CONNECTIONSTRING_NAME].ConnectionString;
            dsn = dsn + ASYNCH;  // Allow for asynchronous operations
            SqlConnection sqlConn = null;
            SqlCommand command;
            try
            {
                sqlConn = new SqlConnection(dsn);
                command = new SqlCommand(SP_UPDATE_CHAIN, sqlConn);
                command.CommandType = CommandType.StoredProcedure;
                sqlConn.Open();
                command.Parameters.AddWithValue(CSID, unitId);
                command.Parameters.AddWithValue(USERID, userId);
                command.BeginExecuteReader(new AsyncCallback(HandleChainUpdateEnd), command, CommandBehavior.CloseConnection);
            }
            catch (Exception)
            {
                if (sqlConn != null && sqlConn.State == ConnectionState.Open)
                {
                    sqlConn.Close();
                }
            }
        }

        public static void UpdateReportingChain(Unit editUnit, int userId)
        {
            XMLString xml = new XMLString("ReportList");
            foreach (KeyValuePair<string, Int32> entry in editUnit.ReportingStructure)
            {
                xml.BeginElement("command");
                xml.WriteAttribute("cs_id", editUnit.Id.ToString());
                xml.WriteAttribute("chain_type", entry.Key);
                xml.WriteAttribute("parent_cs_id", entry.Value.ToString());
                xml.EndElement();
            }

            SqlDataStore DataSource = new SqlDataStore();
            DataSource.ExecuteNonQuery("core_pascode_sp_updateReporting", userId, xml.ToString());
        }

        private static void HandleChainUpdateEnd(IAsyncResult asyncResult)
        {
            SqlDataReader reader = null;
            SqlCommand command;
            command = asyncResult.AsyncState as SqlCommand;

            try
            {
                if (command != null)
                {
                    reader = command.EndExecuteReader(asyncResult);
                    int unitCount = 0;
                    while (reader.Read())
                    {
                        unitCount = reader.GetInt32(0);
                    }
                }
            }
            catch (Exception)
            {
            }
            finally
            {
                if (reader != null)
                {
                    reader.Close();
                }
            }
        }
    }
}