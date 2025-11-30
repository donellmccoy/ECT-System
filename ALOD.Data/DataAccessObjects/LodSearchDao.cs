using ALOD.Core.Domain.Modules.Lod;
using ALOD.Core.Interfaces.DAOInterfaces;
using NHibernate;
using NHibernate.Criterion;
using System;
using System.Collections.Generic;
using System.Data;

namespace ALOD.Data
{
    internal class LodSearchDao : AbstractNHibernateDao<LodSearch, int>, ILodSearchDao
    {
        /// <summary>
        /// Searches for LOD cases using basic search criteria.
        /// Executes stored procedure: form348_sp_Search
        /// </summary>
        /// <param name="caseID">The case ID to search for.</param>
        /// <param name="ssn">The service member SSN.</param>
        /// <param name="name">The service member name.</param>
        /// <param name="status">The case status ID.</param>
        /// <param name="userId">The user ID executing the search.</param>
        /// <param name="rptView">The report view type.</param>
        /// <param name="compo">The component identifier.</param>
        /// <param name="maxCount">Maximum number of results to return.</param>
        /// <param name="moduleId">The module ID.</param>
        /// <param name="isFormal">Indicates whether to search for formal cases (empty string for all).</param>
        /// <param name="unitId">The unit ID filter.</param>
        /// <param name="sarcpermission">Indicates whether the user has SARC permissions.</param>
        /// <returns>A DataSet containing matching LOD cases.</returns>
        public DataSet GetAll(string caseID, string ssn, string name, int status,
                                              int userId, byte rptView, string compo, int maxCount,
                                              byte moduleId, string isFormal, int unitId, bool sarcpermission)
        {
            if (isFormal == "")
            {
                isFormal = null;
            }
            else
            {
                Convert.ToBoolean(isFormal);
            }

            SqlDataStore source = new SqlDataStore();
            return source.ExecuteDataSet("form348_sp_Search", caseID, ssn, name, status, userId, rptView,
                compo, maxCount, moduleId, isFormal, unitId, sarcpermission, null, null);
        }

        /// <summary>
        /// Searches for LOD cases using full name search criteria.
        /// Executes stored procedure: form348_sp_FullSearch
        /// </summary>
        /// <param name="caseID">The case ID to search for.</param>
        /// <param name="ssn">The service member SSN.</param>
        /// <param name="lastName">The service member last name (trimmed).</param>
        /// <param name="firstName">The service member first name (trimmed).</param>
        /// <param name="middleName">The service member middle name (trimmed).</param>
        /// <param name="status">The case status ID.</param>
        /// <param name="userId">The user ID executing the search.</param>
        /// <param name="rptView">The report view type.</param>
        /// <param name="compo">The component identifier.</param>
        /// <param name="maxCount">Maximum number of results to return.</param>
        /// <param name="moduleId">The module ID.</param>
        /// <param name="isFormal">Indicates whether to search for formal cases (empty string for all).</param>
        /// <param name="unitId">The unit ID filter.</param>
        /// <param name="sarcpermission">Indicates whether the user has SARC permissions.</param>
        /// <returns>A DataSet containing matching LOD cases.</returns>
        public DataSet GetAll(string caseID, string ssn, string lastName, string firstName, string middleName, int status,
                                       int userId, byte rptView, string compo, int maxCount, byte moduleId, string isFormal, int unitId, bool sarcpermission)
        {
            if (isFormal == "")
            {
                isFormal = null;
            }
            else
            {
                Convert.ToBoolean(isFormal);
            }

            if (!String.IsNullOrEmpty(lastName))
                lastName = lastName.Trim();

            if (!String.IsNullOrEmpty(firstName))
                firstName = firstName.Trim();

            if (!String.IsNullOrEmpty(middleName))
                middleName = middleName.Trim();

            SqlDataStore source = new SqlDataStore();
            return source.ExecuteDataSet("form348_sp_FullSearch", caseID, ssn, lastName, firstName, middleName, status, userId, rptView,
                compo, maxCount, moduleId, isFormal, unitId, sarcpermission, null, null);
        }

        /// <summary>
        /// Retrieves a LineOfDuty by case ID.
        /// </summary>
        /// <param name="caseId">The case ID to search for.</param>
        /// <returns>The LineOfDuty object matching the case ID, or null if not found.</returns>
        public LineOfDuty GetByCaseId(string caseId)
        {
            ISession session = NHibernateSessionManager.Instance.GetSession();

            IList<LineOfDuty> list = session.CreateCriteria(typeof(LineOfDuty))
                .Add(Expression.Eq("CaseId", caseId))
                .List<LineOfDuty>();

            if (list.Count > 0)
            {
                return list[0];
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Retrieves LOD cases for a pilot user.
        /// Executes stored procedure: form348_sp_PilotCaseSearch
        /// </summary>
        /// <param name="wsId">The workstation ID.</param>
        /// <param name="compo">The component ID.</param>
        /// <returns>A DataSet containing LOD cases for the pilot user.</returns>
        public DataSet GetByPilotUser(int wsId, int compo)
        {
            SqlDataStore source = new SqlDataStore();
            DataSet ds = source.ExecuteDataSet("form348_sp_PilotCaseSearch", wsId, compo);
            return ds;
        }

        /// <summary>
        /// Searches for LOD cases accessible to a specific user.
        /// Executes stored procedure: form348_sp_GroupSearch_ray (non-SARC) or form348_sp_GroupSearch_SARC_Legacy (SARC).
        /// </summary>
        /// <param name="caseID">The case ID to search for.</param>
        /// <param name="ssn">The service member SSN.</param>
        /// <param name="name">The service member name.</param>
        /// <param name="status">The case status ID.</param>
        /// <param name="userId">The user ID executing the search.</param>
        /// <param name="rptView">The report view type.</param>
        /// <param name="compo">The component identifier.</param>
        /// <param name="maxCount">Maximum number of results to return.</param>
        /// <param name="moduleId">The module ID.</param>
        /// <param name="isFormal">Indicates whether to search for formal cases (empty string for all).</param>
        /// <param name="unitId">The unit ID filter.</param>
        /// <param name="sarcpermission">Indicates whether the user has SARC permissions.</param>
        /// <returns>A DataSet containing LOD cases accessible to the user.</returns>
        public DataSet GetByUser(string caseID, string ssn, string name, int status,
                                               int userId, byte rptView, string compo, int maxCount,
                                               byte moduleId, string isFormal, int unitId, bool sarcpermission)
        {
            if (isFormal == "")
            {
                isFormal = null;
            }
            else
            {
                Convert.ToBoolean(isFormal);
            }

            SqlDataStore source = new SqlDataStore();
            if (!sarcpermission)
            {
                DataSet ds = source.ExecuteDataSet("form348_sp_GroupSearch_ray", caseID, ssn, name, status, userId, rptView,
                       compo, maxCount, moduleId, isFormal, unitId, sarcpermission);
                return ds;
            }
            else
            {
                DataSet ds = source.ExecuteDataSet("form348_sp_GroupSearch_SARC_Legacy", caseID, ssn, name, status, userId, rptView,
                       compo, maxCount, moduleId, isFormal, unitId, sarcpermission);
                return ds;
            }
        }

        /// <summary>
        /// Searches for LOD cases accessible to a specific user using full name criteria.
        /// Executes stored procedure: form348_sp_GroupSearch_FullSearch
        /// </summary>
        /// <param name="caseID">The case ID to search for.</param>
        /// <param name="ssn">The service member SSN.</param>
        /// <param name="lastName">The service member last name.</param>
        /// <param name="firstName">The service member first name.</param>
        /// <param name="middleName">The service member middle name.</param>
        /// <param name="status">The case status ID.</param>
        /// <param name="userId">The user ID executing the search.</param>
        /// <param name="rptView">The report view type.</param>
        /// <param name="compo">The component identifier.</param>
        /// <param name="maxCount">Maximum number of results to return.</param>
        /// <param name="moduleId">The module ID.</param>
        /// <param name="isFormal">Indicates whether to search for formal cases (empty string for all).</param>
        /// <param name="unitId">The unit ID filter.</param>
        /// <param name="sarcpermission">Indicates whether the user has SARC permissions.</param>
        /// <returns>A DataSet containing LOD cases accessible to the user.</returns>
        public DataSet GetByUser(string caseID, string ssn, string lastName, string firstName, string middleName, int status,
                                               int userId, byte rptView, string compo, int maxCount,
                                               byte moduleId, string isFormal, int unitId, bool sarcpermission)
        {
            if (isFormal == "")
            {
                isFormal = null;
            }
            else
            {
                Convert.ToBoolean(isFormal);
            }

            SqlDataStore source = new SqlDataStore();
            DataSet ds = source.ExecuteDataSet("form348_sp_GroupSearch_FullSearch", caseID, ssn, lastName, firstName, middleName, status, userId, rptView,
                   compo, maxCount, moduleId, isFormal, unitId, sarcpermission);
            return ds;
        }

        /// <summary>
        /// Searches for LOD cases accessible to an Investigating Officer.
        /// Executes stored procedure: form348_sp_InvestigatingOfficerGroupSearchForLODV3
        /// </summary>
        /// <param name="caseID">The case ID to search for.</param>
        /// <param name="ssn">The service member SSN.</param>
        /// <param name="name">The service member name.</param>
        /// <param name="status">The case status ID.</param>
        /// <param name="userId">The user ID executing the search.</param>
        /// <param name="rptView">The report view type.</param>
        /// <param name="compo">The component identifier.</param>
        /// <param name="maxCount">Maximum number of results to return.</param>
        /// <param name="moduleId">The module ID.</param>
        /// <param name="isFormal">Indicates whether to search for formal cases (empty string for all).</param>
        /// <param name="unitId">The unit ID filter.</param>
        /// <param name="sarcpermission">Indicates whether the user has SARC permissions.</param>
        /// <returns>A DataSet containing LOD cases accessible to the IO.</returns>
        public DataSet GetByUserLOD_IO(string caseID, string ssn, string name, int status,
                                               int userId, byte rptView, string compo, int maxCount,
                                               byte moduleId, string isFormal, int unitId, bool sarcpermission)
        {
            if (isFormal == "")
            {
                isFormal = null;
            }
            else
            {
                Convert.ToBoolean(isFormal);
            }
            SqlDataStore source = new SqlDataStore();
            DataSet ds = source.ExecuteDataSet("form348_sp_InvestigatingOfficerGroupSearchForLODV3", caseID, ssn, name, status, userId, rptView,
                   compo, maxCount, moduleId, isFormal, unitId, sarcpermission);
            return ds;
        }

        /// <summary>
        /// Searches for LOD V3 cases accessible to a specific user.
        /// Executes stored procedure: form348_sp_GroupSearchForLODV3_ray (non-SARC) or form348_sp_GroupSearch_SARC_LOD_V3 (SARC).
        /// </summary>
        /// <param name="caseID">The case ID to search for.</param>
        /// <param name="ssn">The service member SSN.</param>
        /// <param name="name">The service member name.</param>
        /// <param name="status">The case status ID.</param>
        /// <param name="userId">The user ID executing the search.</param>
        /// <param name="rptView">The report view type.</param>
        /// <param name="compo">The component identifier.</param>
        /// <param name="maxCount">Maximum number of results to return.</param>
        /// <param name="moduleId">The module ID.</param>
        /// <param name="isFormal">Indicates whether to search for formal cases (empty string for all).</param>
        /// <param name="unitId">The unit ID filter.</param>
        /// <param name="sarcpermission">Indicates whether the user has SARC permissions.</param>
        /// <returns>A DataSet containing LOD V3 cases accessible to the user.</returns>
        public DataSet GetByUserLODV3(string caseID, string ssn, string name, int status,
                                               int userId, byte rptView, string compo, int maxCount,
                                               byte moduleId, string isFormal, int unitId, bool sarcpermission)
        {
            if (isFormal == "")
            {
                isFormal = null;
            }
            else
            {
                Convert.ToBoolean(isFormal);
            }
            if (!sarcpermission)
            {
                SqlDataStore source = new SqlDataStore();
                DataSet ds = source.ExecuteDataSet("form348_sp_GroupSearchForLODV3_ray", caseID, ssn, name, status, userId, rptView,
                       compo, maxCount, moduleId, isFormal, unitId, sarcpermission);
                return ds;
            }
            else
            {
                SqlDataStore source = new SqlDataStore();
                DataSet ds = source.ExecuteDataSet("form348_sp_GroupSearch_SARC_LOD_V3", caseID, ssn, name, status, userId, rptView,
                       compo, maxCount, moduleId, isFormal, unitId, sarcpermission);
                return ds;
            }
        }

        /// <summary>
        /// Retrieves all LOD cases for a specific service member by SSN.
        /// Executes stored procedure: form348_sp_GetLodsBySM
        /// </summary>
        /// <param name="ssn">The service member SSN.</param>
        /// <param name="sarcpermission">Indicates whether the user has SARC permissions.</param>
        /// <returns>A DataSet containing all LOD cases for the service member (excluding deleted cases).</returns>
        public DataSet GetLodsBySM(string ssn, bool sarcpermission)
        {
            bool deleted = false;

            SqlDataStore source = new SqlDataStore();
            return source.ExecuteDataSet("form348_sp_GetLodsBySM", ssn, sarcpermission, deleted);
        }

        /// <summary>
        /// Searches for post-completion LOD cases.
        /// Executes stored procedure: form348_sp_PostCompletionSearch
        /// </summary>
        /// <param name="caseID">The case ID to search for.</param>
        /// <param name="ssn">The service member SSN.</param>
        /// <param name="name">The service member name.</param>
        /// <param name="userId">The user ID executing the search.</param>
        /// <param name="rptView">The report view type.</param>
        /// <param name="compo">The component identifier.</param>
        /// <param name="maxCount">Maximum number of results to return.</param>
        /// <param name="moduleId">The module ID.</param>
        /// <param name="IsFomal">Indicates whether to search for formal cases (empty string for all).</param>
        /// <param name="unitId">The unit ID filter.</param>
        /// <param name="sarcpermission">Indicates whether the user has SARC permissions.</param>
        /// <param name="searchAllCases">Indicates whether to search all cases regardless of status.</param>
        /// <param name="wsId">The workstation ID.</param>
        /// <returns>A DataSet containing matching post-completion LOD cases.</returns>
        public DataSet GetPostCompletion(string caseID, string ssn, string name,
                                         int userId, byte rptView, string compo, int maxCount, byte moduleId,
                                         string IsFomal, int unitId, bool sarcpermission, bool searchAllCases, int wsId)
        {
            //Insert If statement for the different wsid

            if (IsFomal == "")
            {
                IsFomal = null;
            }
            else
            {
                Convert.ToBoolean(IsFomal);
            }

            SqlDataStore source = new SqlDataStore();

            return source.ExecuteDataSet("form348_sp_PostCompletionSearch", caseID, ssn, name, userId, rptView,
                  compo, maxCount, moduleId, IsFomal, unitId, sarcpermission, searchAllCases
                  , wsId
                  );
        }

        /// <summary>
        /// Searches for undeleted LOD cases.
        /// Executes stored procedure: form348_sp_Search with deleted = false
        /// </summary>
        /// <param name="caseID">The case ID to search for.</param>
        /// <param name="ssn">The service member SSN.</param>
        /// <param name="name">The service member name.</param>
        /// <param name="status">The case status ID.</param>
        /// <param name="userId">The user ID executing the search.</param>
        /// <param name="rptView">The report view type.</param>
        /// <param name="compo">The component identifier.</param>
        /// <param name="maxCount">Maximum number of results to return.</param>
        /// <param name="moduleId">The module ID.</param>
        /// <param name="isFormal">Indicates whether to search for formal cases (empty string for all).</param>
        /// <param name="unitId">The unit ID filter.</param>
        /// <param name="sarcpermission">Indicates whether the user has SARC permissions.</param>
        /// <param name="ovreridescope">Indicates whether to override the default scope restrictions.</param>
        /// <returns>A DataSet containing matching undeleted LOD cases.</returns>
        public DataSet GetUndeleted(string caseID, string ssn, string name, int status,
                                                    int userId, byte rptView, string compo, int maxCount,
                                                    byte moduleId, string isFormal, int unitId, bool sarcpermission
                                                            , bool ovreridescope)
        {
            if (isFormal == "")
            {
                isFormal = null;
            }
            else
            {
                Convert.ToBoolean(isFormal);
            }

            bool deleted = false;

            SqlDataStore source = new SqlDataStore();
            return source.ExecuteDataSet("form348_sp_Search", caseID, ssn, name, status, userId, rptView,
                compo, maxCount, moduleId, isFormal, unitId, sarcpermission, deleted, ovreridescope);
        }

        /// <summary>
        /// Searches for undeleted LOD cases by service member SSN.
        /// Executes stored procedure: form348_sp_CaseSearch with deleted = false
        /// </summary>
        /// <param name="ssn">The service member SSN.</param>
        /// <param name="userId">The user ID executing the search.</param>
        /// <param name="rptView">The report view type.</param>
        /// <param name="compo">The component identifier.</param>
        /// <param name="unitId">The unit ID filter.</param>
        /// <param name="sarcpermission">Indicates whether the user has SARC permissions.</param>
        /// <param name="ovreridescope">Indicates whether to override the default scope restrictions.</param>
        /// <returns>A DataSet containing matching undeleted LOD cases.</returns>
        public DataSet GetUndeletedCases(string ssn, int userId, byte rptView, string compo,
                                                     int unitId, bool sarcpermission
                                                            , bool ovreridescope)
        {
            bool deleted = false;

            SqlDataStore source = new SqlDataStore();
            return source.ExecuteDataSet("form348_sp_CaseSearch", ssn, userId, rptView,
                compo, unitId, sarcpermission, deleted, ovreridescope);
        }
    }
}