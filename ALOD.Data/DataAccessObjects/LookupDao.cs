using ALOD.Core.Domain.Common;
using ALOD.Core.Domain.Lookup;
using ALOD.Core.Domain.Modules.Lod;
using ALOD.Core.Domain.ServiceMembers;
using ALOD.Core.Domain.Users;
using ALOD.Core.Domain.Workflow;
using ALOD.Core.Interfaces.DAOInterfaces;
using ALOD.Core.Utils;
using NHibernate;
using NHibernate.Criterion;
using NHibernate.Transform;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;

namespace ALOD.Data
{
    public class LookupDao : ILookupDao
    {
        private SqlDataStore dataSource;

        private SqlDataStore DataSource
        {
            get
            {
                if (dataSource == null)
                {
                    dataSource = new SqlDataStore();
                }
                return dataSource;
            }
        }

        private ISession Session
        {
            get { return NHibernateSessionManager.Instance.GetSession(); }
        }

        /// <summary>
        /// Deletes a case comment by its ID.
        /// </summary>
        /// <param name="id">The ID of the case comment to delete.</param>
        /// <returns>True if the comment was successfully deleted; otherwise, false.</returns>
        public Boolean DeleteCaseComments(int id)
        {
            DbCommand cmd;
            cmd = DataSource.GetSqlStringCommand("Delete from Case_Comments Where ID = @ID");

            DataSource.AddInParameter(cmd, "@ID", DbType.Int32, id);
            DataSource.ExecuteDataSet(cmd);

            return true;
        }

        /// <summary>
        /// Retrieves a list of LOD Program Managers for a specified unit.
        /// Executes stored procedure: form348_sp_GetLODPM
        /// </summary>
        /// <param name="unitId">The ID of the unit to retrieve LOD PMs for.</param>
        /// <returns>A list of LookUpItem objects representing LOD Program Managers, or an empty list if none found.</returns>
        public IList<LookUpItem> Get_LODPMs(int unitId)
        {
            SqlDataStore store = new SqlDataStore();
            DataSet dSet = store.ExecuteDataSet("form348_sp_GetLODPM", unitId);
            IList<LookUpItem> list = new List<LookUpItem>();

            if (dSet == null)
                return list;

            if (dSet.Tables.Count == 0)
                return list;

            DataTable dTable = dSet.Tables[0];

            if (dTable == null)
                return list;

            foreach (DataRow row in dTable.Rows)
            {
                LookUpItem l = new LookUpItem();

                if (l != null)
                {
                    String name = ALOD.Core.Utils.DataHelpers.GetStringFromDataRow("FirstName", row) + " " + ALOD.Core.Utils.DataHelpers.GetStringFromDataRow("LastName", row);
                    l.Name = name;
                    l.Value = ALOD.Core.Utils.DataHelpers.GetIntFromDataRow("userID", row);

                    list.Add(l);
                }
            }

            return list;
        }

        /// <summary>
        /// Retrieves a list of Wing Sexual Assault Response Coordinators (SARCs) for a specified unit.
        /// Executes stored procedure: form348_sp_GetWingSARC
        /// </summary>
        /// <param name="unitId">The ID of the unit to retrieve Wing SARCs for.</param>
        /// <returns>A list of LookUpItem objects representing Wing SARCs, or an empty list if none found.</returns>
        public IList<LookUpItem> Get_WingSARCs(int unitId)
        {
            SqlDataStore store = new SqlDataStore();
            DataSet dSet = store.ExecuteDataSet("form348_sp_GetWingSARC", unitId);
            IList<LookUpItem> list = new List<LookUpItem>();

            if (dSet == null)
                return list;

            if (dSet.Tables.Count == 0)
                return list;

            DataTable dTable = dSet.Tables[0];

            if (dTable == null)
                return list;

            foreach (DataRow row in dTable.Rows)
            {
                LookUpItem l = new LookUpItem();

                if (l != null)
                {
                    String name = ALOD.Core.Utils.DataHelpers.GetStringFromDataRow("FirstName", row) + " " + ALOD.Core.Utils.DataHelpers.GetStringFromDataRow("LastName", row);
                    l.Name = name;
                    l.Value = ALOD.Core.Utils.DataHelpers.GetIntFromDataRow("userID", row);

                    list.Add(l);
                }
            }

            return list;
        }

        /// <summary>
        /// Retrieves all DAWG (Disability Advisory Working Group) recommendations.
        /// Executes stored procedure: core_lookUps_sp_GetAllDAWGRecommendations
        /// </summary>
        /// <returns>A DataSet containing all DAWG recommendations.</returns>
        public DataSet GetAllDAWGRecommendations()
        {
            return DataSource.ExecuteDataSet("core_lookUps_sp_GetAllDAWGRecommendations");
        }

        /// <summary>
        /// Retrieves all finding-by-reason-of entities.
        /// Executes stored procedure: core_workflow_sp_GetAllFindingByReasonOf
        /// </summary>
        /// <returns>A list of FindingByReasonOf objects.</returns>
        public IList<FindingByReasonOf> GetAllFindingByReasonOfs()
        {
            DataSet dSet = DataSource.ExecuteDataSet("core_workflow_sp_GetAllFindingByReasonOf");

            return DataHelpers.ExtractObjectsFromDataSet<FindingByReasonOf>(dSet);
        }

        /// <summary>
        /// Retrieves all follow-up intervals.
        /// Executes stored procedure: core_lookUps_sp_GetAllFollowUpIntervals
        /// </summary>
        /// <returns>A DataSet containing all follow-up intervals.</returns>
        public DataSet GetAllFollowUpIntervals()
        {
            return DataSource.ExecuteDataSet("core_lookUps_sp_GetAllFollowUpIntervals");
        }

        /// <summary>
        /// Retrieves all missed work days lookup values.
        /// Executes stored procedure: core_lookUps_sp_GetAllMissedWorkDays
        /// </summary>
        /// <returns>A DataSet containing all missed work days values.</returns>
        public DataSet GetAllMissedWorkDays()
        {
            return DataSource.ExecuteDataSet("core_lookUps_sp_GetAllMissedWorkDays");
        }

        /// <summary>
        /// Retrieves all application modules.
        /// Executes stored procedure: core_lookups_GetAllModules
        /// </summary>
        /// <returns>A DataSet containing all application modules.</returns>
        public DataSet GetAllModules()
        {
            SqlDataStore store = new SqlDataStore();
            return store.ExecuteDataSet("core_lookups_GetAllModules");
        }

        /// <summary>
        /// Retrieves all Reserve Medical Units (RMUs).
        /// Executes stored procedure: core_lookups_sp_GetAllRMUs
        /// </summary>
        /// <returns>A list of ReserveMedicalUnit objects.</returns>
        public IList<ReserveMedicalUnit> GetAllRMUs()
        {
            SqlDataStore store = new SqlDataStore();
            DataSet dSet = store.ExecuteDataSet("core_lookups_sp_GetAllRMUs");

            return ExtractRMUs(dSet);
        }

        /// <summary>
        /// Retrieves all specialists required for case management.
        /// Executes stored procedure: core_lookUps_sp_GetAllSpecialistsRequiredForManagement
        /// </summary>
        /// <returns>A DataSet containing all required specialist types.</returns>
        public DataSet GetAllSpecialistsRequiredForManagement()
        {
            return DataSource.ExecuteDataSet("core_lookUps_sp_GetAllSpecialistsRequiredForManagement");
        }

        /// <summary>
        /// Retrieves all sudden incapacitation risk levels.
        /// Executes stored procedure: core_lookUps_sp_GetAllSuddenIncapaciationRisks
        /// </summary>
        /// <returns>A DataSet containing all sudden incapacitation risk values.</returns>
        public DataSet GetAllSuddenIncapaciationRisks()
        {
            return DataSource.ExecuteDataSet("core_lookUps_sp_GetAllSuddenIncapaciationRisks");
        }

        /// <summary>
        /// Retrieves all years of satisfactory service lookup values.
        /// Executes stored procedure: core_lookUps_sp_GetAllYearsSatisfactoryService
        /// </summary>
        /// <returns>A DataSet containing all years of satisfactory service values.</returns>
        public DataSet GetAllYearsSatisfactoryService()
        {
            return DataSource.ExecuteDataSet("core_lookUps_sp_GetAllYearsSatisfactoryService");
        }

        /// <summary>
        /// Retrieves the description for a specific cancel reason.
        /// Executes stored procedure: core_lookUps_sp_GetCancelReasonDescriptionById
        /// </summary>
        /// <param name="reasonId">The ID of the cancel reason.</param>
        /// <returns>The cancel reason description, or an empty string if not found.</returns>
        public string GetCancelReasonDescription(int reasonId)
        {
            SqlDataStore store = new SqlDataStore();
            Object result = store.ExecuteScalar("core_lookUps_sp_GetCancelReasonDescriptionById", reasonId);

            if (result != null)
                return result.ToString();
            else
                return string.Empty;
        }

        /// <summary>
        /// Retrieves case comments for a specific case, module, and comment type.
        /// </summary>
        /// <param name="caseID">The ID of the case.</param>
        /// <param name="ModuleID">The ID of the module.</param>
        /// <param name="commentType">The type of comments to retrieve.</param>
        /// <param name="SortOrder">True for descending order by created date; false for ascending order.</param>
        /// <returns>A DataSet containing the matching case comments.</returns>
        public System.Data.DataSet GetCaseComments(int caseID, int ModuleID, int commentType, bool SortOrder)
        {
            DbCommand cmd;
            if (SortOrder)
            {
                cmd = DataSource.GetSqlStringCommand("SELECT ID, LodId, comments, created_by, created_date, deleted, moduleID, commentType FROM  Case_Comments Where lodid = @caseID and ModuleID = @ModuleID and CommentType = @commentType Order by created_date desc");
            }
            else
            {
                cmd = DataSource.GetSqlStringCommand("SELECT ID, LodId, comments, created_by, created_date, deleted, moduleID, commentType FROM  Case_Comments Where lodid = @caseID and ModuleID = @ModuleID and CommentType = @commentType Order by created_date asc");
            }

            DataSource.AddInParameter(cmd, "@caseID", DbType.Int32, caseID);
            DataSource.AddInParameter(cmd, "@ModuleID", DbType.Int32, ModuleID);
            DataSource.AddInParameter(cmd, "@commentType", DbType.Int32, commentType);
            return DataSource.ExecuteDataSet(cmd);
        }

        /// <summary>
        /// Retrieves the name of a case type by its ID.
        /// </summary>
        /// <param name="id">The ID of the case type.</param>
        /// <returns>The case type name, or an empty string if not found.</returns>
        public string GetCaseTypeName(int id)
        {
            ICaseTypeDao ctDao = new NHibernateDaoFactory().GetCaseTypeDao();

            CaseType ct = ctDao.GetById(id);

            if (ct != null)
                return ct.Name;

            return string.Empty;
        }

        /// <summary>
        /// Retrieves all member categories.
        /// Executes stored procedure: core_LookUps_sp_MemberCategory
        /// </summary>
        /// <returns>A list of LookUpItem objects representing member categories, or an empty list if none found.</returns>
        public IList<LookUpItem> GetCategory()
        {
            SqlDataStore store = new SqlDataStore();
            DataSet dSet = store.ExecuteDataSet("core_LookUps_sp_MemberCategory");
            IList<LookUpItem> list = new List<LookUpItem>();

            if (dSet == null)
                return list;

            if (dSet.Tables.Count == 0)
                return list;

            DataTable dTable = dSet.Tables[0];

            if (dTable == null)
                return list;

            foreach (DataRow row in dTable.Rows)
            {
                LookUpItem l = new LookUpItem();

                if (l != null)
                {
                    l.Name = ALOD.Core.Utils.DataHelpers.GetStringFromDataRow("description", row);
                    l.Value = ALOD.Core.Utils.DataHelpers.GetIntFromDataRow("Id", row);

                    list.Add(l);
                }
            }

            return list;
        }

        /// <summary>
        /// Retrieves all proximate causes.
        /// Executes stored procedure: core_lookUps_sp_ProximateCause
        /// </summary>
        /// <returns>A list of LookUpItem objects representing proximate causes, or an empty list if none found.</returns>
        public IList<LookUpItem> GetCauses()
        {
            SqlDataStore store = new SqlDataStore();
            DataSet dSet = store.ExecuteDataSet("core_lookUps_sp_ProximateCause");
            IList<LookUpItem> list = new List<LookUpItem>();

            if (dSet == null)
                return list;

            if (dSet.Tables.Count == 0)
                return list;

            DataTable dTable = dSet.Tables[0];

            if (dTable == null)
                return list;

            foreach (DataRow row in dTable.Rows)
            {
                LookUpItem l = new LookUpItem();

                if (l != null)
                {
                    l.Name = ALOD.Core.Utils.DataHelpers.GetStringFromDataRow("description", row);
                    l.Value = ALOD.Core.Utils.DataHelpers.GetIntFromDataRow("id", row);

                    list.Add(l);
                }
            }

            return list;
        }

        /// <summary>
        /// Retrieves all chain types.
        /// Executes named query: GetChainType
        /// </summary>
        /// <returns>A list of LookUpItem objects representing chain types.</returns>
        public IList<LookUpItem> GetChainType()
        {
            IQuery query = Session.GetNamedQuery("GetChainType").SetResultTransformer(Transformers.AliasToBean(typeof(LookUpItem)));
            return query.List<LookUpItem>();
        }

        /// <summary>
        /// Retrieves child units for a specified command structure ID and reporting view.
        /// Executes named query: GetPasCodes
        /// </summary>
        /// <param name="cs_id">The command structure ID.</param>
        /// <param name="rptView">The reporting view type.</param>
        /// <returns>A list of LookUpItem objects representing child units.</returns>
        public IList<LookUpItem> GetChildUnits(int cs_id, ReportingView rptView)
        {
            IQuery query = Session.GetNamedQuery("GetPasCodes").SetResultTransformer(Transformers.AliasToBean(typeof(LookUpItem)));
            query.SetString("type", "CHILD");
            query.SetInt32("cs_id", cs_id);
            query.SetByte("rptView", Convert.ToByte(rptView));
            IList<LookUpItem> list = query.List<LookUpItem>();

            return list;
        }

        /// <summary>
        /// Retrieves the formatted name of the user who created a comment.
        /// </summary>
        /// <param name="created_by">The user ID of the comment creator.</param>
        /// <returns>The formatted name of the user (Title + LastName, FirstName).</returns>
        public String GetCommentsCreatedBy(int created_by)
        {
            DbCommand cmd;

            try
            {
                cmd = DataSource.GetSqlStringCommand("SELECT (clg.Title + ' ' + LastName + ', ' + FirstName) as CreatedBy FROM core_Users cu Inner Join core_lkupGrade clg On cu.Rank_Code = clg.Code Where userID = @created_by");
                DataSource.AddInParameter(cmd, "@created_by", DbType.Int32, created_by);
                return (string)(DataSource.ExecuteScalar(cmd));
            }
            catch (Exception)
            {
                cmd = DataSource.GetSqlStringCommand("SELECT (LastName + ', ' + FirstName) as CreatedBy FROM core_Users Where userID = @created_by");
                DataSource.AddInParameter(cmd, "@created_by", DbType.Int32, created_by);
                return (string)(DataSource.ExecuteScalar(cmd));
            }
        }

        /// <summary>
        /// Retrieves the name of a completed-by group by its ID.
        /// </summary>
        /// <param name="id">The ID of the completed-by group.</param>
        /// <returns>The completed-by group name, or an empty string if not found.</returns>
        public string GetCompletedByGroupName(int id)
        {
            ICompletedByGroupDao cbgDao = new NHibernateDaoFactory().GetCompletedByGroupDao();

            CompletedByGroup cbg = cbgDao.GetById(id);

            if (cbgDao != null)
                return cbg.Name;

            return string.Empty;
        }

        /// <summary>
        /// Retrieves all components.
        /// Executes stored procedure: core_LookUps_sp_Component
        /// </summary>
        /// <returns>A list of LookUpItem objects representing components, or an empty list if none found.</returns>
        public IList<LookUpItem> GetComponents()
        {
            SqlDataStore store = new SqlDataStore();
            DataSet dSet = store.ExecuteDataSet("core_LookUps_sp_Component");
            IList<LookUpItem> list = new List<LookUpItem>();

            if (dSet == null)
                return list;

            if (dSet.Tables.Count == 0)
                return list;

            DataTable dTable = dSet.Tables[0];

            if (dTable == null)
                return list;

            foreach (DataRow row in dTable.Rows)
            {
                LookUpItem l = new LookUpItem();

                if (l != null)
                {
                    l.Name = ALOD.Core.Utils.DataHelpers.GetStringFromDataRow("description", row);
                    l.Value = ALOD.Core.Utils.DataHelpers.GetIntFromDataRow("id", row);

                    list.Add(l);
                }
            }

            return list;
        }

        /// <summary>
        /// Retrieves the list of Air Force components (AFRC and ANG).
        /// </summary>
        /// <returns>A list of LookUpItem objects representing Air Force Reserve and Air National Guard.</returns>
        public IList<LookUpItem> GetCompos()
        {
            LookUpItem lkupItem = new LookUpItem { Name = "Air Force Reserve", Value = "AFRC" };
            LookUpItem lkupItem2 = new LookUpItem { Name = "Air National Guard", Value = "ANG" };
            List<LookUpItem> lst = new List<LookUpItem>();
            lst.Add(lkupItem);
            lst.Add(lkupItem2);
            return lst;
        }

        /// <summary>
        /// Retrieves the description of a data type by its ID.
        /// Executes stored procedure: core_lookups_sp_GetDataTypeById
        /// </summary>
        /// <param name="id">The ID of the data type.</param>
        /// <returns>The data type description, or an empty string if not found.</returns>
        public string GetDataTypeById(int id)
        {
            SqlDataStore store = new SqlDataStore();
            Object result = store.ExecuteScalar("core_lookups_sp_GetDataTypeById", id);

            if (result != null)
                return result.ToString();
            else
                return string.Empty;
        }

        /// <summary>
        /// Retrieves all data types.
        /// Executes stored procedure: core_lookups_sp_GetDataTypes
        /// </summary>
        /// <returns>A DataSet containing all data types.</returns>
        public DataSet GetDataTypes()
        {
            SqlDataStore store = new SqlDataStore();
            return store.ExecuteDataSet("core_lookups_sp_GetDataTypes");
        }

        /// <summary>
        /// Retrieves all duty statuses.
        /// Executes stored procedure: core_LookUps_sp_DutyStatuses
        /// </summary>
        /// <returns>A list of LookUpItem objects representing duty statuses, or an empty list if none found.</returns>
        public IList<LookUpItem> GetDutyStatuses()
        {
            SqlDataStore store = new SqlDataStore();
            DataSet dSet = store.ExecuteDataSet("core_LookUps_sp_DutyStatuses");
            IList<LookUpItem> list = new List<LookUpItem>();

            if (dSet == null)
                return list;

            if (dSet.Tables.Count == 0)
                return list;

            DataTable dTable = dSet.Tables[0];

            if (dTable == null)
                return list;

            foreach (DataRow row in dTable.Rows)
            {
                LookUpItem l = new LookUpItem();

                if (l != null)
                {
                    l.Name = ALOD.Core.Utils.DataHelpers.GetStringFromDataRow("description", row);
                    l.Value = ALOD.Core.Utils.DataHelpers.GetStringFromDataRow("type", row);

                    list.Add(l);
                }
            }

            return list;
        }

        /// <summary>
        /// Retrieves a specific finding-by-reason-of entity by its ID.
        /// Executes stored procedure: core_workflow_sp_GetFindingByReasonOfById
        /// </summary>
        /// <param name="id">The ID of the finding-by-reason-of entity.</param>
        /// <returns>The FindingByReasonOf object, or null if not found.</returns>
        public FindingByReasonOf GetFindingByReasonOfById(int id)
        {
            DataSet dSet = DataSource.ExecuteDataSet("core_workflow_sp_GetFindingByReasonOfById", id);

            return DataHelpers.ExtractObjectFromDataSet<FindingByReasonOf>(dSet);
        }

        /// <summary>
        /// Retrieves all findings.
        /// Executes stored procedure: core_lookUps_sp_v1_Findings
        /// </summary>
        /// <returns>A list of LookUpItem objects representing findings, or an empty list if none found.</returns>
        public IList<LookUpItem> GetFindings()
        {
            SqlDataStore store = new SqlDataStore();
            DataSet dSet = store.ExecuteDataSet("core_lookUps_sp_v1_Findings");
            IList<LookUpItem> list = new List<LookUpItem>();

            if (dSet == null)
                return list;

            if (dSet.Tables.Count == 0)
                return list;

            DataTable dTable = dSet.Tables[0];

            if (dTable == null)
                return list;

            foreach (DataRow row in dTable.Rows)
            {
                LookUpItem l = new LookUpItem();

                if (l != null)
                {
                    l.Name = ALOD.Core.Utils.DataHelpers.GetStringFromDataRow("Description", row);
                    l.Value = ALOD.Core.Utils.DataHelpers.GetIntFromDataRow("Id", row);

                    list.Add(l);
                }
            }

            return list;
        }

        /// <summary>
        /// Retrieves the content of the FindingsLookUp table.
        /// </summary>
        /// <returns>A list of FindingsLookUp entities.</returns>
        public IList<FindingsLookUp> GetFindingTypes()
        {
            IList<FindingsLookUp> list = Session.CreateCriteria(typeof(FindingsLookUp)).List<FindingsLookUp>();
            return list;
        }

        /// <summary>
        /// Retrieves a snapshot of Form 348 data within a specified date range.
        /// Executes stored procedure: core_lookUps_sp_GetForm348Snapshot
        /// </summary>
        /// <param name="beginDate">The start date of the snapshot range.</param>
        /// <param name="endDate">The end date of the snapshot range.</param>
        /// <returns>A DataSet containing the Form 348 snapshot data.</returns>
        public DataSet GetForm348Snapshot(DateTime beginDate, DateTime endDate)
        {
            return DataSource.ExecuteDataSet("core_lookUps_sp_GetForm348Snapshot", beginDate, endDate);
        }

        /// <summary>
        /// Retrieves the form field parser configuration by its ID.
        /// Executes stored procedure: Print_sp_GetFormFieldParser
        /// </summary>
        /// <param name="id">The ID of the form field parser.</param>
        /// <returns>The form field parser configuration, or an empty string if not found.</returns>
        public string GetFormFieldParserById(int id)
        {
            Object result = DataSource.ExecuteScalar("Print_sp_GetFormFieldParser", id);

            if (result != null)
                return result.ToString();
            else
                return string.Empty;
        }

        /// <summary>
        /// Retrieves all 'from' locations.
        /// Executes stored procedure: core_LookUps_sp_FromLocation
        /// </summary>
        /// <returns>A list of LookUpItem objects representing 'from' locations, or an empty list if none found.</returns>
        public IList<LookUpItem> GetFromLocation()
        {
            SqlDataStore store = new SqlDataStore();
            DataSet dSet = store.ExecuteDataSet("core_LookUps_sp_FromLocation");
            IList<LookUpItem> list = new List<LookUpItem>();

            if (dSet == null)
                return list;

            if (dSet.Tables.Count == 0)
                return list;

            DataTable dTable = dSet.Tables[0];

            if (dTable == null)
                return list;

            foreach (DataRow row in dTable.Rows)
            {
                LookUpItem l = new LookUpItem();

                if (l != null)
                {
                    l.Name = ALOD.Core.Utils.DataHelpers.GetStringFromDataRow("description", row);
                    l.Value = ALOD.Core.Utils.DataHelpers.GetIntFromDataRow("id", row);

                    list.Add(l);
                }
            }

            return list;
        }

        /// <summary>
        /// Retrieves all user groups for a specified component (AFRC or ANG).
        /// </summary>
        /// <param name="compo">The component code (e.g., 'AFRC', 'ANG').</param>
        /// <returns>A list of UserGroup objects for the specified component.</returns>
        public IList<UserGroup> GetGroupsByCompo(string compo)
        {
            IList<UserGroup> list = Session.CreateCriteria(typeof(UserGroup))
                .Add(Expression.Eq("Component", compo))
                .List<UserGroup>();

            return list;
        }

        /// <summary>
        /// Checks if a Line of Duty case has an associated appeal.
        /// Executes stored procedure: core_lod_sp_GetHasAppeal
        /// </summary>
        /// <param name="initialLodId">The ID of the initial LOD case.</param>
        /// <returns>True if the LOD case has an appeal; otherwise, false.</returns>
        public bool GetHasAppealLOD(int initialLodId)
        {
            SqlDataStore store = new SqlDataStore();
            Object result = store.ExecuteScalar("core_lod_sp_GetHasAppeal", initialLodId);

            if (result == null)
                return false;

            int iResult = (int)result;

            if (iResult == 0)
                return false;

            return true;
        }

        /// <summary>
        /// Checks if a SARC case has an associated appeal.
        /// Executes stored procedure: core_sarc_sp_GetHasAppeal
        /// </summary>
        /// <param name="initialLodId">The ID of the initial LOD case.</param>
        /// <param name="workflowId">The workflow ID.</param>
        /// <returns>True if the SARC case has an appeal; otherwise, false.</returns>
        public bool GetHasAppealSARC(int initialLodId, int workflowId)
        {
            SqlDataStore store = new SqlDataStore();
            Object result = store.ExecuteScalar("core_sarc_sp_GetHasAppeal", initialLodId, workflowId);

            if (result == null)
                return false;

            int iResult = (int)result;

            if (iResult == 0)
                return false;

            return true;
        }

        /// <summary>
        /// Checks if a LOD case has an associated reinvestigation.
        /// Executes stored procedure: core_lod_sp_GetHasReinvestigationLod
        /// </summary>
        /// <param name="initialLodId">The ID of the initial LOD case.</param>
        /// <returns>True if the LOD case has a reinvestigation; otherwise, false.</returns>
        public bool GetHasReinvestigationLod(int initialLodId)
        {
            SqlDataStore store = new SqlDataStore();
            Object result = store.ExecuteScalar("core_lod_sp_GetHasReinvestigationLod", initialLodId);

            if (result == null)
                return false;

            int iResult = (int)result;

            if (iResult == 0)
                return false;

            return true;
        }

        /// <summary>
        /// Checks if a case type has sub-case types.
        /// </summary>
        /// <param name="id">The ID of the case type.</param>
        /// <returns>True if the case type has sub-case types; otherwise, false.</returns>
        public bool GetHasSubCaseTypes(int id)
        {
            ICaseTypeDao ctDao = new NHibernateDaoFactory().GetCaseTypeDao();

            CaseType ct = ctDao.GetById(id);

            if (ct == null)
                return false;

            if (ct.SubCaseTypes != null && ct.SubCaseTypes.Count > 0)
                return true;

            return false;
        }

        /// <summary>
        /// Retrieves an ICD-9 code entity by its ID.
        /// </summary>
        /// <param name="id">The ID of the ICD-9 code.</param>
        /// <returns>The ICD9Code object.</returns>
        public ICD9Code GetIcd9ById(int id)
        {
            return Session.Load<ICD9Code>(id);
        }

        /// <summary>
        /// Retrieves all information sources.
        /// Executes stored procedure: core_LookUps_sp_InfoSources
        /// </summary>
        /// <returns>A list of LookUpItem objects representing information sources, or an empty list if none found.</returns>
        public IList<LookUpItem> GetInfoSources()
        {
            SqlDataStore store = new SqlDataStore();
            DataSet dSet = store.ExecuteDataSet("core_LookUps_sp_InfoSources");
            IList<LookUpItem> list = new List<LookUpItem>();

            if (dSet == null)
                return list;

            if (dSet.Tables.Count == 0)
                return list;

            DataTable dTable = dSet.Tables[0];

            if (dTable == null)
                return list;

            foreach (DataRow row in dTable.Rows)
            {
                LookUpItem l = new LookUpItem();

                if (l != null)
                {
                    l.Name = ALOD.Core.Utils.DataHelpers.GetStringFromDataRow("description", row);
                    l.Value = ALOD.Core.Utils.DataHelpers.GetIntFromDataRow("id", row);

                    list.Add(l);
                }
            }

            return list;
        }

        /// <summary>
        /// Retrieves the initial status for a user, workflow, and group combination.
        /// Executes named query: GetInitialStatus
        /// </summary>
        /// <param name="userId">The user ID.</param>
        /// <param name="workflow">The workflow type.</param>
        /// <param name="groupId">The group ID.</param>
        /// <returns>The initial status ID, or 0 if not found.</returns>
        public int GetInitialStatus(int userId, byte workflow, int groupId)
        {
            IList<int> list = Session.GetNamedQuery("GetInitialStatus")
                .SetInt32("userId", userId)
                .SetByte("workflow", workflow)
                .SetInt32("groupId", groupId)
                .List<int>();

            if (list.Count > 0)
            {
                return list[0];
            }
            else
            {
                return 0;
            }
        }

        /// <summary>
        /// Retrieves the count of in-progress reinvestigations for a LOD case.
        /// Executes stored procedure: form348_sp_GetInProgressReinvestigationCount
        /// </summary>
        /// <param name="initialLodId">The ID of the initial LOD case.</param>
        /// <returns>The count of in-progress reinvestigations, or 0 if none found.</returns>
        public int GetInProgressReinvestigation(int initialLodId)
        {
            SqlDataStore store = new SqlDataStore();
            Object result = store.ExecuteScalar("form348_sp_GetInProgressReinvestigationCount", initialLodId);

            if (result == null)
                return 0;

            int iResult = (int)result;

            return iResult;
        }

        /// <summary>
        /// Retrieves the list of Investigating Officers (IOs) for a user and reporting view.
        /// Executes named query: GetIOList
        /// </summary>
        /// <param name="userId">The user ID.</param>
        /// <param name="rptView">The reporting view type.</param>
        /// <param name="memberCaseGradeCode">The grade code of the case member.</param>
        /// <returns>A list of LookUpItem objects representing available IOs.</returns>
        public IList<LookUpItem> GetIOList(int userId, short rptView, int memberCaseGradeCode)
        {
            IQuery query = Session.GetNamedQuery("GetIOList").SetResultTransformer(Transformers.AliasToBean(typeof(LookUpItem)));
            query.SetInt32("userId", userId);
            query.SetInt16("rptView", rptView);
            query.SetInt32("caseMemberGradeCode", memberCaseGradeCode);
            return query.List<LookUpItem>();
        }

        /// <summary>
        /// Retrieves ICD code IDs for a specific IR/ILO type.
        /// Executes stored procedure: core_lookups_sp_GetIRILOTypeICDCodeIDs
        /// </summary>
        /// <param name="typeName">The name of the IR/ILO type.</param>
        /// <returns>A list of ICD code IDs, or null if none found.</returns>
        public IList<int> GetIRILOTypeICDCodeIds(string typeName)
        {
            System.Data.DataSet ds = DataSource.ExecuteDataSet("core_lookups_sp_GetIRILOTypeICDCodeIDs", typeName);

            if (ds == null || ds.Tables.Count == 0)
                return null;

            List<int> items = new List<int>();

            foreach (System.Data.DataRow row in ds.Tables[0].Rows)
            {
                items.Add((int)row["ICD_ID"]);
            }

            return items;
        }

        /// <summary>
        /// Checks if a special case is a reassessment case.
        /// </summary>
        /// <param name="refId">The reference ID of the special case.</param>
        /// <returns>True if the case is a reassessment; otherwise, false.</returns>
        public bool GetIsReassessmentSpecialCase(int refId)
        {
            ISpecialCaseDAO specCaseDao = new NHibernateDaoFactory().GetSpecialCaseDAO();

            if (specCaseDao == null)
                return false;

            return specCaseDao.GetIsReassessmentCase(refId);
        }

        /// <summary>
        /// Checks if a LOD case is a reinvestigation.
        /// Executes stored procedure: core_lod_sp_GetIsReinvestigationLod
        /// </summary>
        /// <param name="lodId">The ID of the LOD case.</param>
        /// <returns>True if the LOD case is a reinvestigation; otherwise, false.</returns>
        public bool GetIsReinvestigationLod(int lodId)
        {
            SqlDataStore store = new SqlDataStore();
            Object result = store.ExecuteScalar("core_lod_sp_GetIsReinvestigationLod", lodId);

            if (result == null)
                return false;

            int iResult = (int)result;

            if (iResult == 0)
                return false;

            return true;
        }

        /// <summary>
        /// Retrieves medical group names that begin with a specified search string.
        /// Executes named query: GetMedGroupNames
        /// </summary>
        /// <param name="searchBegin">The beginning text to search for in medical group names.</param>
        /// <returns>A list of LookUpItem objects representing matching medical groups.</returns>
        public IList<LookUpItem> GetMedGroupNames(string searchBegin)
        {
            IQuery query = Session.GetNamedQuery("GetMedGroupNames").SetResultTransformer(Transformers.AliasToBean(typeof(LookUpItem)));

            if (!string.IsNullOrEmpty(searchBegin.Trim()))
            {
                query.SetString("Name", searchBegin);
            }

            IList<LookUpItem> list = query.List<LookUpItem>();

            return list;
        }

        /// <summary>
        /// Retrieves all medical facilities.
        /// Executes stored procedure: core_LookUps_sp_MedicalFacility
        /// </summary>
        /// <returns>A list of LookUpItem objects representing medical facilities, or an empty list if none found.</returns>
        public IList<LookUpItem> GetMedicalFacility()
        {
            SqlDataStore store = new SqlDataStore();
            DataSet dSet = store.ExecuteDataSet("core_LookUps_sp_MedicalFacility");
            IList<LookUpItem> list = new List<LookUpItem>();

            if (dSet == null)
                return list;

            if (dSet.Tables.Count == 0)
                return list;

            DataTable dTable = dSet.Tables[0];

            if (dTable == null)
                return list;

            foreach (DataRow row in dTable.Rows)
            {
                LookUpItem l = new LookUpItem();

                if (l != null)
                {
                    l.Name = ALOD.Core.Utils.DataHelpers.GetStringFromDataRow("description", row);
                    l.Value = ALOD.Core.Utils.DataHelpers.GetStringFromDataRow("type", row);

                    list.Add(l);
                }
            }

            return list;
        }

        /// <summary>
        /// Retrieves member components for a specified workflow.
        /// Executes stored procedure: core_lookups_sp_GetMemberComponentsByWorkflow
        /// </summary>
        /// <param name="workflowId">The workflow ID.</param>
        /// <returns>A list of LookUpItem objects representing member components, or an empty list if none found.</returns>
        public IList<LookUpItem> GetMemberComponentsByWorkflow(int workflowId)
        {
            DataSet dSet = DataSource.ExecuteDataSet("core_lookups_sp_GetMemberComponentsByWorkflow", workflowId);
            IList<LookUpItem> list = new List<LookUpItem>();

            if (dSet == null)
                return list;

            if (dSet.Tables.Count == 0)
                return list;

            DataTable dTable = dSet.Tables[0];

            if (dTable == null)
                return list;

            foreach (DataRow row in dTable.Rows)
            {
                LookUpItem l = new LookUpItem();

                if (l != null)
                {
                    l.Name = DataHelpers.GetStringFromDataRow("Name", row);
                    l.Value = DataHelpers.GetIntFromDataRow("Id", row);

                    list.Add(l);
                }
            }

            return list;
        }

        /// <summary>
        /// Retrieves all member influence values.
        /// Executes stored procedure: core_LookUps_sp_MemberInfluence
        /// </summary>
        /// <returns>A list of LookUpItem objects representing member influence values, or an empty list if none found.</returns>
        public IList<LookUpItem> GetMemberInfluence()
        {
            SqlDataStore store = new SqlDataStore();
            DataSet dSet = store.ExecuteDataSet("core_LookUps_sp_MemberInfluence");
            IList<LookUpItem> list = new List<LookUpItem>();

            if (dSet == null)
                return list;

            if (dSet.Tables.Count == 0)
                return list;

            DataTable dTable = dSet.Tables[0];

            if (dTable == null)
                return list;

            foreach (DataRow row in dTable.Rows)
            {
                LookUpItem l = new LookUpItem();

                if (l != null)
                {
                    l.Name = ALOD.Core.Utils.DataHelpers.GetStringFromDataRow("description", row);
                    l.Value = ALOD.Core.Utils.DataHelpers.GetIntFromDataRow("id", row);

                    list.Add(l);
                }
            }

            return list;
        }

        /// <summary>
        /// Retrieves all occurrence types.
        /// Executes stored procedure: core_LookUps_sp_Occurrences
        /// </summary>
        /// <returns>A list of LookUpItem objects representing occurrence types, or an empty list if none found.</returns>
        public IList<LookUpItem> GetOccurrences()
        {
            SqlDataStore store = new SqlDataStore();
            DataSet dSet = store.ExecuteDataSet("core_LookUps_sp_Occurrences");
            IList<LookUpItem> list = new List<LookUpItem>();

            if (dSet == null)
                return list;

            if (dSet.Tables.Count == 0)
                return list;

            DataTable dTable = dSet.Tables[0];

            if (dTable == null)
                return list;

            foreach (DataRow row in dTable.Rows)
            {
                LookUpItem l = new LookUpItem();

                if (l != null)
                {
                    l.Name = ALOD.Core.Utils.DataHelpers.GetStringFromDataRow("description", row);
                    l.Value = ALOD.Core.Utils.DataHelpers.GetIntFromDataRow("id", row);

                    list.Add(l);
                }
            }

            return list;
        }

        /// <summary>
        /// Retrieves all PEPP case types.
        /// Executes stored procedure: core_lookUps_sp_GetPEPPCaseTypes
        /// </summary>
        /// <returns>A DataSet containing all PEPP case types.</returns>
        public DataSet GetPEPPCaseTypes()
        {
            SqlDataStore store = new SqlDataStore();
            return store.ExecuteDataSet("core_lookUps_sp_GetPEPPCaseTypes");
        }

        /// <summary>
        /// Retrieves PEPP dispositions filtered by ID and filter value.
        /// Executes stored procedure: core_lookUps_sp_GetPEPPDispositions
        /// </summary>
        /// <param name="id">The ID to filter by.</param>
        /// <param name="filter">The filter value.</param>
        /// <returns>A DataSet containing the matching PEPP dispositions.</returns>
        public DataSet GetPEPPDispositions(int id, int filter)
        {
            SqlDataStore store = new SqlDataStore();
            return store.ExecuteDataSet("core_lookUps_sp_GetPEPPDispositions", id, filter);
        }

        /// <summary>
        /// Retrieves PEPP ratings filtered by ID and filter value.
        /// Executes stored procedure: core_lookUps_sp_GetPEPPRatings
        /// </summary>
        /// <param name="id">The ID to filter by.</param>
        /// <param name="filter">The filter value.</param>
        /// <returns>A DataSet containing the matching PEPP ratings.</returns>
        public DataSet GetPEPPRatings(int id, int filter)
        {
            SqlDataStore store = new SqlDataStore();
            return store.ExecuteDataSet("core_lookUps_sp_GetPEPPRatings", id, filter);
        }

        /// <summary>
        /// Retrieves PEPP types filtered by ID and filter value.
        /// Executes stored procedure: core_lookUps_sp_GetPEPPTypes
        /// </summary>
        /// <param name="id">The ID to filter by.</param>
        /// <param name="filter">The filter value.</param>
        /// <returns>A DataSet containing the matching PEPP types.</returns>
        public DataSet GetPEPPTypes(int id, int filter)
        {
            SqlDataStore store = new SqlDataStore();
            return store.ExecuteDataSet("core_lookUps_sp_GetPEPPTypes", id, filter);
        }

        /// <summary>
        /// Retrieves all process types.
        /// Executes stored procedure: core_LookUps_sp_Process
        /// </summary>
        /// <returns>A list of LookUpItem objects representing process types, or an empty list if none found.</returns>
        public IList<LookUpItem> GetProcess()
        {
            SqlDataStore store = new SqlDataStore();
            DataSet dSet = store.ExecuteDataSet("core_LookUps_sp_Process");
            IList<LookUpItem> list = new List<LookUpItem>();

            if (dSet == null)
                return list;

            if (dSet.Tables.Count == 0)
                return list;

            DataTable dTable = dSet.Tables[0];

            if (dTable == null)
                return list;

            foreach (DataRow row in dTable.Rows)
            {
                LookUpItem l = new LookUpItem();

                if (l != null)
                {
                    l.Name = ALOD.Core.Utils.DataHelpers.GetStringFromDataRow("description", row);
                    l.Value = ALOD.Core.Utils.DataHelpers.GetIntFromDataRow("id", row);

                    list.Add(l);
                }
            }

            return list;
        }

        /// <summary>
        /// Retrieves all PSCD member statuses (for PSCD workflow).
        /// Executes stored procedure: core_lookUps_sp_GetMemberPSCDStatus
        /// </summary>
        /// <returns>A list of LookUpItem objects representing PSCD member statuses, or an empty list if none found.</returns>
        public IList<LookUpItem> GetPSCDMemberStatus()
        {
            //for PSCD workflow
            SqlDataStore store = new SqlDataStore();
            DataSet dSet = store.ExecuteDataSet("core_lookUps_sp_GetMemberPSCDStatus");
            IList<LookUpItem> list = new List<LookUpItem>();

            if (dSet == null)
                return list;

            if (dSet.Tables.Count == 0)
                return list;

            DataTable dTable = dSet.Tables[0];

            if (dTable == null)
                return list;

            foreach (DataRow row in dTable.Rows)
            {
                LookUpItem l = new LookUpItem();

                if (l != null)
                {
                    l.Name = ALOD.Core.Utils.DataHelpers.GetStringFromDataRow("service", row);
                    l.Value = ALOD.Core.Utils.DataHelpers.GetIntFromDataRow("serviceID", row);

                    list.Add(l);
                }
            }

            return list;
        }

        /// <summary>
        /// Retrieves the memo ID for a PSCD case by reference ID and template ID.
        /// Executes stored procedure: memo_sp_GetPSCDMemo
        /// </summary>
        /// <param name="refId">The reference ID of the PSCD case.</param>
        /// <param name="templetId">The template ID to search for.</param>
        /// <returns>The memo ID if found; otherwise, 0.</returns>
        public virtual Int32 GetPSCDMemoId(Int32 refId, Int32 templetId)
        {
            int memoId = 0;
            int index = 0;
            int num = 0;
            SqlDataStore store = new SqlDataStore();
            DataSet dSet = store.ExecuteDataSet("memo_sp_GetPSCDMemo", refId);
            if (dSet.Tables[0].Rows.Count != 0)
            {
                foreach (DataRow x in dSet.Tables[0].Rows)
                {
                    num = int.Parse(dSet.Tables[0].Rows[index].ItemArray[3].ToString());
                    if (num == templetId)
                    {
                        return int.Parse(dSet.Tables[0].Rows[index].ItemArray[0].ToString());
                    }
                    index++;
                }
            }

            return memoId;
        }

        /// <summary>
        /// Retrieves all PW (Psychosocial Work) categories.
        /// </summary>
        /// <returns>A DataSet containing all PW categories.</returns>
        public System.Data.DataSet GetPWCategories()
        {
            DbCommand cmd = DataSource.GetSqlStringCommand("SELECT Id, Name FROM core_lkupPWCategories");
            return DataSource.ExecuteDataSet(cmd);
        }

        /// <summary>
        /// Retrieves a user rank by its code.
        /// </summary>
        /// <param name="code">The rank code.</param>
        /// <returns>The UserRank object, or null if not found.</returns>
        public UserRank GetRank(int code)
        {
            return Session.CreateCriteria(typeof(UserRank))
                .Add(Expression.Like("Id", code))
               .UniqueResult<UserRank>();
        }

        /// <summary>
        /// Retrieves the abbreviation for a rank by type and rank code.
        /// Executes stored procedure: core_lookUps_sp_GetGradeAbbreviationByTypeAndCode
        /// </summary>
        /// <param name="rank">The UserRank object.</param>
        /// <param name="typeName">The type name for the abbreviation.</param>
        /// <returns>The rank abbreviation, or an empty string if not found.</returns>
        public string GetRankAbbreviationByType(UserRank rank, string typeName)
        {
            Object result = DataSource.ExecuteScalar("core_lookUps_sp_GetGradeAbbreviationByTypeAndCode", typeName, rank.Id);

            if (result != null)
                return result.ToString();
            else
                return string.Empty;
        }

        /// <summary>
        /// Retrieves all military ranks/grades.
        /// Executes stored procedure: core_lookups_sp_GetGrades
        /// </summary>
        /// <returns>A list of LookUpItem objects representing ranks, or an empty list if none found.</returns>
        public IList<LookUpItem> GetRanks()
        {
            SqlDataStore store = new SqlDataStore();
            DataSet dSet = store.ExecuteDataSet("core_lookups_sp_GetGrades");
            IList<LookUpItem> list = new List<LookUpItem>();

            if (dSet == null)
                return list;

            if (dSet.Tables.Count == 0)
                return list;

            DataTable dTable = dSet.Tables[0];

            if (dTable == null)
                return list;

            foreach (DataRow row in dTable.Rows)
            {
                LookUpItem l = new LookUpItem();

                if (l != null)
                {
                    l.Name = ALOD.Core.Utils.DataHelpers.GetStringFromDataRow("Title", row);
                    l.Value = ALOD.Core.Utils.DataHelpers.GetStringFromDataRow("Code", row);

                    list.Add(l);
                }
            }

            return list;
        }

        /// <summary>
        /// Retrieves all ranks and grades ordered by display order.
        /// </summary>
        /// <returns>A list of UserRank objects ordered by display order.</returns>
        public IList<UserRank> GetRanksAndGrades()
        {
            IList<UserRank> list = Session.CreateCriteria(typeof(UserRank))
                .AddOrder(Order.Asc("DisplayOrder"))
                .List<UserRank>();

            return list;
        }

        /// <summary>
        /// Retrieves the description of a RWOA (Returned Without Official Action) reason.
        /// </summary>
        /// <param name="id">The ID of the RWOA reason.</param>
        /// <returns>The reason description, or an empty string if not found.</returns>
        public string GetReasonDescription(short id)
        {
            IList<RwoaReason> list = Session.CreateCriteria(typeof(RwoaReason))
                .Add(Expression.Eq("Id", Convert.ToInt32(id)))
                 .List<RwoaReason>();

            if (list.Count > 0)
            {
                return list[0].Description;
            }
            else
            {
                return "";
            }
        }

        /// <summary>
        /// Retrieves the final findings from all reinvestigations for a LOD case.
        /// Executes stored procedure: core_lod_sp_GetReinvestigationLodFindings
        /// </summary>
        /// <param name="initialLodId">The ID of the initial LOD case.</param>
        /// <returns>A list of final finding IDs, or null if none found.</returns>
        public IList<int> GetReinvestigationLodFindings(int initialLodId)
        {
            System.Data.DataSet ds = DataSource.ExecuteDataSet("core_lod_sp_GetReinvestigationLodFindings", initialLodId);

            if (ds == null || ds.Tables.Count == 0)
                return null;

            List<int> items = new List<int>();

            foreach (System.Data.DataRow row in ds.Tables[0].Rows)
            {
                items.Add((byte)row["FinalFindings"]);
            }

            return items;
        }

        /// <summary>
        /// Retrieves reporting period Month/Year for PH (Psychological Health) reports.
        /// </summary>
        /// <returns>A DataSet containing distinct reporting periods from PH special cases.</returns>
        public DataSet GetReportPeriod()
        {
            SqlDataStore Datasource = new SqlDataStore();
            string sql = "select distinct DATENAME(MONTH, ph_reporting_period) + ' ' + DATENAME(YEAR, ph_reporting_period) AS PeriodText, " +
                        "CAST(ph_reporting_period AS DATE) AS PeriodValue from Form348_SC " +
                        "where ph_reporting_period is not null order by PeriodValue desc";
            DbCommand cmd = Datasource.GetSqlStringCommand(sql);
            return Datasource.ExecuteDataSet(cmd);
        }

        /// <summary>
        /// Retrieves all Reserve Medical Units (RMUs).
        /// Executes stored procedure: core_LookUps_sp_RMU
        /// </summary>
        /// <returns>A list of LookUpItem objects representing RMUs, or an empty list if none found.</returns>
        public IList<LookUpItem> GetRMU()
        {
            SqlDataStore store = new SqlDataStore();
            DataSet dSet = store.ExecuteDataSet("core_LookUps_sp_RMU");
            IList<LookUpItem> list = new List<LookUpItem>();

            if (dSet == null)
                return list;

            if (dSet.Tables.Count == 0)
                return list;

            DataTable dTable = dSet.Tables[0];

            if (dTable == null)
                return list;

            foreach (DataRow row in dTable.Rows)
            {
                LookUpItem l = new LookUpItem();

                if (l != null)
                {
                    l.Name = ALOD.Core.Utils.DataHelpers.GetStringFromDataRow("description", row);
                    l.Value = ALOD.Core.Utils.DataHelpers.GetIntFromDataRow("Id", row);

                    list.Add(l);
                }
            }

            return list;
        }

        /// <summary>
        /// Retrieves a specific Reserve Medical Unit by its ID.
        /// Executes stored procedure: core_lookups_sp_GetRMUById
        /// </summary>
        /// <param name="id">The ID of the Reserve Medical Unit.</param>
        /// <returns>The ReserveMedicalUnit object, or null if not found or multiple results exist.</returns>
        public ReserveMedicalUnit GetRMUById(int id)
        {
            SqlDataStore store = new SqlDataStore();
            DataSet dSet = store.ExecuteDataSet("core_lookups_sp_GetRMUById", id);

            IList<ReserveMedicalUnit> results = ExtractRMUs(dSet);

            if (results.Count != 1)
                return null;

            return results[0];
        }

        /// <summary>
        /// Retrieves Reserve Medical Unit names that begin with a specified search string.
        /// Executes stored procedure: core_LookUps_sp_RMUNames
        /// </summary>
        /// <param name="searchBegin">The beginning text to search for in RMU names.</param>
        /// <returns>A list of LookUpItem objects representing matching RMUs, or an empty list if none found.</returns>
        public IList<LookUpItem> GetRMUNames(string searchBegin)
        {
            //IQuery query = Session.GetNamedQuery("GetRMUNames").SetResultTransformer(Transformers.AliasToBean(typeof(LookUpItem)));

            //if (!string.IsNullOrEmpty(searchBegin.Trim()))
            //{
            //    query.SetString("Name", searchBegin);
            //}

            //IList<LookUpItem> list = query.List<LookUpItem>();

            //return list;

            SqlDataStore store = new SqlDataStore();
            DataSet dSet = store.ExecuteDataSet("core_LookUps_sp_RMUNames");
            IList<LookUpItem> list = new List<LookUpItem>();

            if (dSet == null)
                return list;

            if (dSet.Tables.Count == 0)
                return list;

            DataTable dTable = dSet.Tables[0];

            if (dTable == null)
                return list;

            foreach (DataRow row in dTable.Rows)
            {
                LookUpItem l = new LookUpItem();

                if (l != null)
                {
                    l.Name = ALOD.Core.Utils.DataHelpers.GetStringFromDataRow("Name", row);
                    l.Value = ALOD.Core.Utils.DataHelpers.GetIntFromDataRow("Value", row);

                    list.Add(l);
                }
            }

            return list;
        }

        /// <summary>
        /// Retrieves the ID of a special case sub-workflow type by its title.
        /// Executes stored procedure: core_lookUps_sp_Get_SCSubTypeIdByWorkflowTitle
        /// </summary>
        /// <param name="workflowTitle">The title of the workflow.</param>
        /// <returns>The sub-workflow type ID, or -1 if not found.</returns>
        public int GetSCSubWorkflowTypeId(string workflowTitle)
        {
            SqlDataStore store = new SqlDataStore();
            Object result = store.ExecuteScalar("core_lookUps_sp_Get_SCSubTypeIdByWorkflowTitle", workflowTitle);

            if (result == null)
                return -1;
            else
                return (int)result;
        }

        /// <summary>
        /// Retrieves all special case sub-workflow types for a specified workflow.
        /// Executes stored procedure: core_lookUps_sp_Get_SCSubTypesByWorkflowId
        /// </summary>
        /// <param name="workflowId">The workflow ID.</param>
        /// <returns>A DataSet containing all sub-workflow types for the specified workflow.</returns>
        public DataSet GetSCSubWorkflowTypes(int workflowId)
        {
            SqlDataStore store = new SqlDataStore();
            return store.ExecuteDataSet("core_lookUps_sp_Get_SCSubTypesByWorkflowId", workflowId);
        }

        /// <summary>
        /// Retrieves a service member by their Social Security Number.
        /// </summary>
        /// <param name="ssn">The Social Security Number of the service member.</param>
        /// <returns>The ServiceMember object, or null if not found.</returns>
        public ServiceMember GetServiceMemberBySSN(string ssn)
        {
            IList<ServiceMember> list = Session.CreateCriteria(typeof(ServiceMember))
                .Add(Expression.Eq("Id", ssn))
                .List<ServiceMember>();

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
        /// Retrieves service members by their name components.
        /// Executes stored procedure: member_sp_GetMembersByName
        /// </summary>
        /// <param name="lastName">The last name of the service member.</param>
        /// <param name="firstName">The first name of the service member.</param>
        /// <param name="middleName">The middle name of the service member.</param>
        /// <returns>A DataTable containing matching service members.</returns>
        public DataTable GetServiceMembersByName(string lastName, string firstName, string middleName)
        {
            SqlDataStore store = new SqlDataStore();
            return store.ExecuteDataSet("member_sp_GetMembersByName", lastName, firstName, middleName).Tables[0];
        }

        /// <summary>
        /// Retrieves PEPP types for a specific special case.
        /// </summary>
        /// <param name="refId">The reference ID of the special case.</param>
        /// <returns>A list of LookUpItem objects representing PEPP types, or null if the DAO is unavailable.</returns>
        public IList<LookUpItem> GetSpecialCasePEPPTypes(int refId)
        {
            ISpecialCaseDAO specCaseDao = new NHibernateDaoFactory().GetSpecialCaseDAO();

            if (specCaseDao == null)
                return null;

            return specCaseDao.GetSpecialCasePEPPTypes(refId);
        }

        /// <summary>
        /// Retrieves all states ordered by country (descending) and state ID (ascending).
        /// </summary>
        /// <returns>A list of State objects.</returns>
        public IList<State> GetStates()
        {
            IList<State> list = Session.CreateCriteria(typeof(State))
                .AddOrder(Order.Desc("Country"))
                .AddOrder(Order.Asc("Id"))
                .List<State>();

            return list;
        }

        /// <summary>
        /// Retrieves all member statuses.
        /// Executes stored procedure: core_LookUps_sp_MemberStatus
        /// </summary>
        /// <returns>A list of LookUpItem objects representing member statuses, or an empty list if none found.</returns>
        public IList<LookUpItem> GetStatus()
        {
            SqlDataStore store = new SqlDataStore();
            DataSet dSet = store.ExecuteDataSet("core_LookUps_sp_MemberStatus");
            IList<LookUpItem> list = new List<LookUpItem>();

            if (dSet == null)
                return list;

            if (dSet.Tables.Count == 0)
                return list;

            DataTable dTable = dSet.Tables[0];

            if (dTable == null)
                return list;

            foreach (DataRow row in dTable.Rows)
            {
                LookUpItem l = new LookUpItem();

                if (l != null)
                {
                    l.Name = ALOD.Core.Utils.DataHelpers.GetStringFromDataRow("Description", row);
                    l.Value = ALOD.Core.Utils.DataHelpers.GetStringFromDataRow("Type", row);

                    list.Add(l);
                }
            }

            return list;
        }

        /// <summary>
        /// Retrieves all status codes for a specified module, ordered by description.
        /// </summary>
        /// <param name="moduleId">The module ID.</param>
        /// <returns>A list of StatusCode objects for the specified module.</returns>
        public IList<StatusCode> GetStatusCodesByModule(int moduleId)
        {
            IList<StatusCode> results = Session.CreateCriteria(typeof(StatusCode))
                .Add(Expression.Eq("ModuleId", (ModuleType)moduleId))
                .List<StatusCode>().OrderBy(x => x.Description).ToList<StatusCode>();

            return results;
        }

        /// <summary>
        /// Retrieves status tracking history for a case, ordered by ID descending.
        /// </summary>
        /// <param name="refId">The reference ID of the case.</param>
        /// <param name="moduleType">The module type.</param>
        /// <returns>A list of WorkStatusTracking objects for the specified case.</returns>
        public IList<WorkStatusTracking> GetStatusTracking(int refId, byte moduleType)
        {
            return Session.CreateCriteria(typeof(WorkStatusTracking))
                .Add(Expression.Eq("ReferenceId", refId))
                .Add(Expression.Eq("ModuleId", moduleType))
                .AddOrder(Order.Desc("Id"))
                .List<WorkStatusTracking>();
        }

        /// <summary>
        /// Retrieves the name of a sub-case type by its ID.
        /// </summary>
        /// <param name="id">The ID of the sub-case type.</param>
        /// <returns>The sub-case type name, or an empty string if not found.</returns>
        public string GetSubCaseTypeName(int id)
        {
            ICaseTypeDao ctDao = new NHibernateDaoFactory().GetCaseTypeDao();

            CaseType ct = ctDao.GetAllSubCaseTypes().Where(c => c.Id == id).First();

            if (ct != null)
                return ct.Name;

            return string.Empty;
        }

        /// <summary>
        /// Retrieves a unit by its ID.
        /// </summary>
        /// <param name="unitId">The ID of the unit.</param>
        /// <returns>The Unit object, or null if not found.</returns>
        public Unit GetUnitById(int unitId)
        {
            IList<Unit> list = Session.CreateCriteria(typeof(Unit))
                .Add(Expression.Eq("Id", unitId))
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

        /// <summary>
        /// Retrieves the PAS (Personnel Accounting Symbol) code for a unit by its ID and name.
        /// Executes stored procedure: cmdStruct_sp_GetUnitPasCode
        /// </summary>
        /// <param name="id">The unit ID.</param>
        /// <param name="name">The unit name.</param>
        /// <returns>The PAS code, or an empty string if not found.</returns>
        public string GetUnitPasByIdAndName(int id, string name)
        {
            SqlDataStore store = new SqlDataStore();
            Object result = store.ExecuteScalar("cmdStruct_sp_GetUnitPasCode", id, name);

            if (result == null)
                return String.Empty;
            else
                return result.ToString();
        }

        /// <summary>
        /// Retrieves the display text for a unit by its command structure ID.
        /// </summary>
        /// <param name="cs_id">The command structure ID.</param>
        /// <returns>The unit display text.</returns>
        public string GetUnitText(int cs_id)
        {
            Object result = DataSource.ExecuteScalar("core_sp_GetUnitText", cs_id);

            if (result == null)
                return String.Empty;
            else
                return result.ToString();
        }

        /// <summary>
        /// Retrieves cancel reasons for a specific workflow.
        /// Executes stored procedure: core_workflow_sp_GetCancelReasons
        /// </summary>
        /// <param name="workflowId">The workflow ID.</param>
        /// <param name="isFormal">True for formal cases; false for informal cases.</param>
        /// <returns>A DataSet containing cancel reasons for the specified workflow.</returns>
        public DataSet GetWorkflowCancelReasons(int workflowId, bool isFormal)
        {
            SqlDataStore store = new SqlDataStore();
            return store.ExecuteDataSet("core_workflow_sp_GetCancelReasons", workflowId, isFormal);
        }

        /// <summary>
        /// Retrieves workflow findings for a specific workflow and group.
        /// Executes named query: GetWorkflowFindings
        /// </summary>
        /// <param name="workflowId">The workflow ID.</param>
        /// <param name="groupId">The group ID.</param>
        /// <returns>A list of FindingsLookUp objects for the specified workflow and group.</returns>
        public IList<FindingsLookUp> GetWorkflowFindings(int workflowId, int groupId)
        {
            IQuery query = Session.GetNamedQuery("GetWorkflowFindings").SetResultTransformer(Transformers.AliasToBean(typeof(FindingsLookUp))); ;
            query.SetInt32("groupId", groupId);
            query.SetInt32("workflowId", workflowId);
            IList<FindingsLookUp> list = query.List<FindingsLookUp>();
            return list;
        }

        /// <summary>
        /// Retrieves the initial status code for a workflow based on component, module, and workflow IDs.
        /// Executes stored procedure: core_workflow_sp_GetWorkflowInitialStatusCode
        /// </summary>
        /// <param name="compo">The component ID.</param>
        /// <param name="moduleId">The module ID.</param>
        /// <param name="workflowId">The workflow ID.</param>
        /// <returns>The initial status code.</returns>
        public int GetWorkflowInitialStatusCode(int compo, int moduleId, int workflowId)
        {
            SqlDataStore store = new SqlDataStore();
            return Convert.ToInt32(store.ExecuteScalar("core_workflow_sp_GetWorkflowInitialStatusCode", compo, moduleId, workflowId));
        }

        /// <summary>
        /// Retrieves return reasons for a specific workflow.
        /// Executes stored procedure: core_workflow_sp_GetReturnReasons
        /// </summary>
        /// <param name="workflowId">The workflow ID.</param>
        /// <returns>A DataSet containing return reasons for the specified workflow.</returns>
        public DataSet GetWorkflowReturnReasons(int workflowId)
        {
            SqlDataStore store = new SqlDataStore();
            return store.ExecuteDataSet("core_workflow_sp_GetReturnReasons", workflowId);
        }

        /// <summary>
        /// Retrieves RWOA (Returned Without Official Action) reasons for a specific workflow.
        /// Executes stored procedure: core_workflow_sp_GetRwoaReasons
        /// </summary>
        /// <param name="workflowId">The workflow ID.</param>
        /// <returns>A DataSet containing RWOA reasons for the specified workflow.</returns>
        public DataSet GetWorkflowRwoaReasons(int workflowId)
        {
            SqlDataStore store = new SqlDataStore();
            return store.ExecuteDataSet("core_workflow_sp_GetRwoaReasons", workflowId);
        }

        /// <summary>
        /// Retrieves all workflows for a specified component (AFRC or ANG).
        /// </summary>
        /// <param name="compo">The component code (e.g., 'AFRC', 'ANG').</param>
        /// <returns>A list of Workflow objects for the specified component.</returns>
        public IList<ALOD.Core.Domain.Workflow.Workflow> GetWorkflowsByCompo(string compo)
        {
            return Session.CreateCriteria(typeof(Workflow))
                .Add(Expression.Eq("Compo", compo))
                .List<Workflow>();
        }

        /// <summary>
        /// Retrieves work statuses for a specific workflow, ordered by description.
        /// </summary>
        /// <param name="workflow">The workflow ID.</param>
        /// <returns>A DataSet containing work statuses for the specified workflow.</returns>
        public System.Data.DataSet GetWorkStatusByWorkflow(byte workflow)
        {
            DbCommand cmd = DataSource.GetSqlStringCommand("SELECT ws_id Id, Description FROM vw_workstatus WHERE workflowId = @workflow ORDER BY description");
            DataSource.AddInParameter(cmd, "@workflow", DbType.Byte, workflow);
            return DataSource.ExecuteDataSet(cmd);
        }

        /// <summary>
        /// Retrieves a list of WWD (Warrior Wellness Days) cases by member SSN.
        /// Executes named query: GetWWDsByMemberSSN
        /// </summary>
        /// <param name="memberSSN">The Social Security Number of the member.</param>
        /// <param name="searchType">The type of search to perform.</param>
        /// <param name="userId">The user ID performing the search.</param>
        /// <returns>A list of LookUpItem objects representing WWD cases.</returns>
        public IList<LookUpItem> GetWWDListByMemberSSN(string memberSSN, int searchType, int userId)
        {
            return Session.GetNamedQuery("GetWWDsByMemberSSN")
                .SetString("memberSSN", memberSSN)
                .SetInt32("searchType", searchType)
                .SetInt32("userId", userId)
                .SetResultTransformer(Transformers.AliasToBean(typeof(LookUpItem)))
                .List<LookUpItem>();
        }

        /// <summary>
        /// Checks if a LOD case has an active reinvestigation.
        /// Executes stored procedure: core_lod_sp_GetHasActiveReinvestigation
        /// </summary>
        /// <param name="initialLodId">The ID of the initial LOD case.</param>
        /// <returns>True if the LOD case has an active reinvestigation; otherwise, false.</returns>
        public bool LODHasActiveReinvestigation(int initialLodId)
        {
            SqlDataStore store = new SqlDataStore();
            Object result = store.ExecuteScalar("core_lod_sp_GetHasActiveReinvestigation", initialLodId);

            if (result == null)
                return false;

            int iResult = (int)result;

            if (iResult == 0)
                return false;

            return true;
        }

        /// <summary>
        /// Inserts a new case comment.
        /// </summary>
        /// <param name="caseID">The ID of the case.</param>
        /// <param name="comments">The comment text.</param>
        /// <param name="created_by">The user ID who created the comment.</param>
        /// <param name="created_date">The date the comment was created.</param>
        /// <param name="ModuleID">The module ID.</param>
        /// <param name="commentType">The type of comment.</param>
        /// <param name="deleted">Whether the comment is deleted.</param>
        /// <returns>True if the comment was successfully inserted; otherwise, false.</returns>
        public Boolean SetCaseComments(int caseID, string comments, int created_by, DateTime created_date, int ModuleID, int commentType, bool deleted)
        {
            DbCommand cmd;
            cmd = DataSource.GetSqlStringCommand("Insert into Case_Comments (LodId, comments, created_by, created_date, deleted, moduleID, commentType) Values (@caseID, @comments, @created_by, @created_date, @deleted, @ModuleID, @commentType)");

            DataSource.AddInParameter(cmd, "@caseID", DbType.Int32, caseID);
            DataSource.AddInParameter(cmd, "@comments", DbType.String, comments);
            DataSource.AddInParameter(cmd, "@created_by", DbType.Int32, created_by);
            DataSource.AddInParameter(cmd, "@created_date", DbType.DateTime, created_date);
            DataSource.AddInParameter(cmd, "@deleted", DbType.Boolean, deleted);
            DataSource.AddInParameter(cmd, "@ModuleID", DbType.Int32, ModuleID);
            DataSource.AddInParameter(cmd, "@commentType", DbType.Int32, commentType);
            DataSource.ExecuteDataSet(cmd);
            return true;
        }

        /// <summary>
        /// Updates an existing case comment.
        /// </summary>
        /// <param name="id">The ID of the comment to update.</param>
        /// <param name="caseID">The ID of the case.</param>
        /// <param name="comments">The updated comment text.</param>
        /// <param name="created_by">The user ID who created the comment.</param>
        /// <param name="created_date">The date the comment was created.</param>
        /// <param name="ModuleID">The module ID.</param>
        /// <param name="commentType">The type of comment.</param>
        /// <param name="deleted">Whether the comment is deleted.</param>
        /// <returns>True if the comment was successfully updated; otherwise, false.</returns>
        public Boolean UpdateCaseComments(int id, int caseID, string comments, int created_by, DateTime created_date, int ModuleID, int commentType, bool deleted)
        {
            DbCommand cmd;
            cmd = DataSource.GetSqlStringCommand("Update Case_Comments set LodId = @caseID, comments = @comments, created_by = @created_by, created_date = @created_date, deleted = @deleted, moduleID = @ModuleID, commentType = @commentType Where ID = @ID");

            DataSource.AddInParameter(cmd, "@ID", DbType.Int32, id);
            DataSource.AddInParameter(cmd, "@caseID", DbType.Int32, caseID);
            DataSource.AddInParameter(cmd, "@comments", DbType.String, comments);
            DataSource.AddInParameter(cmd, "@created_by", DbType.Int32, created_by);
            DataSource.AddInParameter(cmd, "@created_date", DbType.DateTime, created_date);
            DataSource.AddInParameter(cmd, "@deleted", DbType.Boolean, deleted);
            DataSource.AddInParameter(cmd, "@ModuleID", DbType.Int32, ModuleID);
            DataSource.AddInParameter(cmd, "@commentType", DbType.Int32, commentType);
            DataSource.ExecuteDataSet(cmd);

            return true;
        }

        private IList<ReserveMedicalUnit> ExtractRMUs(DataSet dSet)
        {
            if (dSet == null)
                return null;

            if (dSet.Tables.Count == 0)
                return null;

            DataTable dTable = dSet.Tables[0];

            if (dTable == null)
                return null;

            IList<ReserveMedicalUnit> results = new List<ReserveMedicalUnit>();

            foreach (DataRow row in dTable.Rows)
            {
                ReserveMedicalUnit rmu = new ReserveMedicalUnit(row);

                if (rmu != null)
                {
                    results.Add(rmu);
                }
            }

            return results;
        }
    }
}