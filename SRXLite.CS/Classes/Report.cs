using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using SRXLite.DataAccess;
using SRXLite.DataTypes;
using static SRXLite.Modules.Util;
using static SRXLite.DataAccess.DB;

namespace SRXLite.Classes
{
    public class Report
    {
        private short _userID;

        #region Constructors

        public Report()
        {
        }

        public Report(short userID)
        {
            _userID = userID;
        }

        #endregion

        #region UsageSummaryData

        public class UsageSummaryData
        {
            public string ActionTypeName { get; set; }
            public string CategoryName { get; set; }
            public int Count { get; set; }
            public string UserName { get; set; }
        }

        #endregion

        #region GuidStatsData

        public class GuidStatsData
        {
            public int DocGuidCount { get; set; }
            public int DocPageGuidCount { get; set; }
            public DateTime ReportDate { get; set; }
        }

        #endregion

        #region GetUsageSummary

        /// <summary>
        /// Get usage summary report
        /// </summary>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <param name="catg"></param>
        /// <returns></returns>
        public List<UsageSummaryData> GetUsageSummary(DateTime startDate, DateTime endDate, Category catg)
        {
            SqlCommand command = new SqlCommand();
            command.CommandText = "dsp_Report_GetUsageSummary";
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.Add(GetSqlParameter("@UserID", _userID));
            command.Parameters.Add(GetSqlParameter("@StartDate", startDate));
            command.Parameters.Add(GetSqlParameter("@EndDate", endDate));
            command.Parameters.Add(GetSqlParameter("@CategoryID", (int)catg));

            UsageSummaryData usageSummaryData;
            List<UsageSummaryData> list = new List<UsageSummaryData>();

            using (SqlDataReader reader = DB.ExecuteReader(command))
            {
                while (reader.Read())
                {
                    usageSummaryData = new UsageSummaryData();
                    usageSummaryData.ActionTypeName = NullCheck(reader["ActionTypeName"]);
                    usageSummaryData.CategoryName = NullCheck(reader["CategoryName"]);
                    usageSummaryData.Count = IntCheck(reader["Count"]);
                    usageSummaryData.UserName = NullCheck(reader["UserName"]);

                    list.Add(usageSummaryData);
                }
            }

            return list;
        }

        #endregion

        #region GetRequestStatistics

        /// <summary>
        /// Get request/GUID statistics
        /// </summary>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <returns></returns>
        public List<GuidStatsData> GetRequestStatistics(DateTime startDate, DateTime endDate)
        {
            SqlCommand command = new SqlCommand();
            command.CommandText = "dsp_Report_GetGuidStatistics";
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.Add(GetSqlParameter("@StartDate", startDate));
            command.Parameters.Add(GetSqlParameter("@EndDate", endDate));

            GuidStatsData guidStatsData;
            List<GuidStatsData> list = new List<GuidStatsData>();

            using (SqlDataReader reader = DB.ExecuteReader(command))
            {
                while (reader.Read())
                {
                    guidStatsData = new GuidStatsData();
                    guidStatsData.ReportDate = DateCheck(reader["ReportDate"], DateTime.MinValue);
                    guidStatsData.DocGuidCount = IntCheck(reader["DocGuidCount"]);
                    guidStatsData.DocPageGuidCount = IntCheck(reader["DocPageGuidCount"]);

                    list.Add(guidStatsData);
                }
            }

            return list;
        }

        #endregion
    }
}
