using ALOD.Core.Domain.Modules.Lod;
using ALOD.Core.Domain.ServiceMembers;
using ALOD.Core.Domain.Users;
using ALOD.Core.Domain.Workflow;
using ALOD.Core.Interfaces.DAOInterfaces;
using ALOD.Core.Utils;
using System;
using System.Data;

namespace ALOD.Data.Services
{
    /// <summary>
    /// Service layer for Line of Duty (LOD) operations including LOD cases, appeals, special cases, and SARC cases.
    /// Provides a unified interface for accessing LOD-related data access objects and business logic.
    /// </summary>
    public class LodService : DataService
    {
        private static ILODAppealDAO _appealDao;
        private static ILineOfDutyDao _dao;
        private static IDaoFactory _factory;
        private static IPALDataDao _palDao;
        private static ILODReinvestigateDAO _requestDao;
        private static ISARCAppealDAO _SARCappealDao;
        private static ISARCDAO _sarcRestrictedDao;
        private static ISpecialCaseDAO _SCdao;
        private static ILodSearchDao _srchDao;
        private static IUtilityDao _utilDao;

        public static ILODAppealDAO appealDao
        {
            get
            {
                if (_appealDao == null)
                    _appealDao = DaoFactocy.GetLODAppealDao();

                return _appealDao;
            }
        }

        public static IDaoFactory DaoFactocy
        {
            get
            {
                if (_factory == null)
                    _factory = new NHibernateDaoFactory();

                return _factory;
            }
        }

        public static ISARCDAO GetSARCDao
        {
            get
            {
                if (_sarcRestrictedDao == null)
                    _sarcRestrictedDao = DaoFactocy.GetSARCDao();

                return _sarcRestrictedDao;
            }
        }

        public static ILineOfDutyDao LODDao
        {
            get
            {
                if (_dao == null)
                    _dao = DaoFactocy.GetLineOfDutyDao();

                return _dao;
            }
        }

        public static IPALDataDao palDao
        {
            get
            {
                if (_palDao == null)
                    _palDao = DaoFactocy.GetPALDataDao();

                return _palDao;
            }
        }

        public static ILODReinvestigateDAO requestDao
        {
            get
            {
                if (_requestDao == null)
                    _requestDao = DaoFactocy.GetLODReinvestigationDao();

                return _requestDao;
            }
        }

        public static ISARCAppealDAO SARCappealDao
        {
            get
            {
                if (_SARCappealDao == null)
                    _SARCappealDao = DaoFactocy.GetSARCAppealDao();

                return _SARCappealDao;
            }
        }

        public static ISpecialCaseDAO scDao
        {
            get
            {
                if (_SCdao == null)
                    _SCdao = DaoFactocy.GetSpecialCaseDAO();

                return _SCdao;
            }
        }

        public static ILodSearchDao srchDao
        {
            get
            {
                if (_srchDao == null)
                    _srchDao = new NHibernateDaoFactory().GetLodSearchDao();

                return _srchDao;
            }
        }

        public static IUtilityDao utilDao
        {
            get
            {
                if (_utilDao == null)
                    _utilDao = DaoFactocy.GetUtilityDao();

                return _utilDao;
            }
        }

        public static DataSet AppealRequestSearch(string caseID, string ssn, string name, int status, int userId, byte rptView, string compo, int maxCount, byte moduleId, int unitId)
        {
            return appealDao.AppealRequestSearch(caseID, ssn, name, status, userId, rptView, compo, maxCount, moduleId, unitId);
        }

        public static DataSet AppealRequestSearch(string caseID, string ssn, string lastName, string firstName, string middleName, int status, int userId, byte rptView, string compo, int maxCount, byte moduleId, int unitId)
        {
            return appealDao.AppealRequestSearch(caseID, ssn, lastName, firstName, middleName, status, userId, rptView, compo, maxCount, moduleId, unitId);
        }

        public static string AppointedIONameAndRank(LineOfDuty lod)
        {
            ServiceMember ioMember = null;
            String nameRank = String.Empty;
            AppUser io = null;
            String ioSSN = string.Empty;
            if (lod.IoSsn != null)
            {
                ioSSN = lod.IoSsn.Trim();
            }

            if (lod.AppointedIO != null)
            {
                io = lod.AppointedIO;
            }
            else
            {
                if (ioSSN != string.Empty)
                {
                    io = UserService.GetBySSN(ioSSN);
                    if (io == null)
                    {
                        ioMember = LookupService.GetServiceMemberBySSN(ioSSN);
                    }
                }
            }

            if (io != null)
            {
                return nameRank = io.Rank.Rank + " " + io.LastName + ", " + io.FirstName;
            }
            else
            {
                if (ioMember != null)
                {
                    nameRank = ioMember.Rank.Rank + " " + ioMember.LastName + ", " + ioMember.FirstName;
                }
            }

            return nameRank;
        }

        public static bool AssignIo(int refId, int ioUserId, int aaUserId, bool isFormal)
        {
            return utilDao.AssignIo(refId, ioUserId, aaUserId, isFormal);
        }

        public static DataSet GetAllLods(string caseID, string ssn, string name, int status,
                                                   int userId, byte rptView, string compo, int maxCount, byte moduleId, string IsFomal, int unitId, bool sarcpermission)
        {
            return srchDao.GetAll(caseID, ssn, name, status, userId, rptView, compo, maxCount, moduleId, IsFomal, unitId, sarcpermission);
        }

        public static DataSet GetAllLods(string caseID, string ssn, string lastName, string firstName, string middleInitial, int status,
                                                   int userId, byte rptView, string compo, int maxCount, byte moduleId, string IsFomal, int unitId, bool sarcpermission)
        {
            return srchDao.GetAll(caseID, ssn, lastName, firstName, middleInitial, status, userId, rptView, compo, maxCount, moduleId, IsFomal, unitId, sarcpermission);
        }

        public static DataSet GetAppealRequests(int userId, bool sarc)
        {
            return appealDao.GetAppealRequests(userId, sarc);
        }

        public static LineOfDuty GetById(int userId)
        {
            return LODDao.GetById(userId);
        }

        public static DataSet GetByPilotUser(int wsId, int compo)
        {
            return srchDao.GetByPilotUser(wsId, compo);
        }

        public static DataSet GetByUser(string caseID, string ssn, string name, int status,
                                                  int userId, byte rptView, string compo, int maxCount, byte moduleId, string IsFomal, int unitId, bool sarcpermission)
        {
            return srchDao.GetByUser(caseID, ssn, name, status, userId, rptView, compo, maxCount, moduleId, IsFomal, unitId, sarcpermission);
        }

        public static DataSet GetByUser(string caseID, string ssn, string lastName, string firstName, string middleName, int status,
                                                  int userId, byte rptView, string compo, int maxCount, byte moduleId, string IsFomal, int unitId, bool sarcpermission)
        {
            return srchDao.GetByUser(caseID, ssn, lastName, firstName, middleName, status, userId, rptView, compo, maxCount, moduleId, IsFomal, unitId, sarcpermission);
        }

        public static DataSet GetByUserLOD_IO(string caseID, string ssn, string name, int status,
                                                  int userId, byte rptView, string compo, int maxCount, byte moduleId, string IsFomal, int unitId, bool sarcpermission)
        {
            return srchDao.GetByUserLOD_IO(caseID, ssn, name, status, userId, rptView, compo, maxCount, moduleId, IsFomal, unitId, sarcpermission);
        }

        public static DataSet GetByUserLODV3(string caseID, string ssn, string name, int status,
                                                  int userId, byte rptView, string compo, int maxCount, byte moduleId, string IsFomal, int unitId, bool sarcpermission)
        {
            return srchDao.GetByUserLODV3(caseID, ssn, name, status, userId, rptView, compo, maxCount, moduleId, IsFomal, unitId, sarcpermission);
        }

        public static DataSet GetCaseHistory(string ssn, int userId, byte rptView, string compo, int unitId, bool sarcpermission, bool ovreridescope)
        {
            return srchDao.GetUndeletedCases(ssn, userId, rptView, compo, unitId, sarcpermission, ovreridescope);
        }

        public static int GetCompletedAppealCount(int userId, byte reportView, int unitId, bool sarc)
        {
            DataSet data = GetPostAppealCompletion("", "", "", userId, reportView, "", 0, (int)ModuleType.AppealRequest, "", unitId, sarc);

            if (data == null || data.Tables.Count == 0 || data.Tables[0].Rows.Count == 0)
            {
                return 0;
            }

            return data.Tables[0].Rows.Count;
        }

        public static DataSet GetCompletedAPs(int userId, bool sarc)
        {
            return appealDao.GetCompletedAPs(userId, sarc);
        }

        public static int GetCompletedCount(int userId, byte reportView, int unitId, bool sarc, bool searchAllCases, int wsId, string compo)
        {
            DataSet data = GetPostCompletion("", "", "", userId, reportView, compo, 0, (int)ModuleType.LOD, "", unitId, sarc, searchAllCases, wsId);

            if (data == null || data.Tables.Count == 0 || data.Tables[0].Rows.Count == 0)
            {
                return 0;
            }

            return data.Tables[0].Rows.Count;
        }

        public static DataSet GetCompletedRRs(int userId, bool sarc)
        {
            return requestDao.GetCompletedRRs(userId, sarc);
        }

        public static int GetCompletedSARCAppealCount(int userId, byte reportView, int unitId, bool sarc)
        {
            DataSet data = GetPostSARCAppealCompletion("", "", "", userId, reportView, "", 0, (int)ModuleType.SARCAppeal, unitId, sarc);

            if (data == null || data.Tables.Count == 0 || data.Tables[0].Rows.Count == 0)
            {
                return 0;
            }

            return data.Tables[0].Rows.Count;
        }

        public static DataSet GetCompletedSARCAPs(int userId, bool sarc)
        {
            return SARCappealDao.GetCompletedAPs(userId, sarc);
        }

        public static LodWorkStatus GetInitialNextStep(int refId, int inStatus)
        {
            SqlDataStore dataSource = new SqlDataStore();
            LodWorkStatus outStatus;
            try
            {
                outStatus = (LodWorkStatus)dataSource.ExecuteScalar("form348_GetInitialNextStep", ModuleType.LOD, refId, inStatus);
                return outStatus;
            }
            catch (Exception)
            {
                return LodWorkStatus.UnKnown;
            }
        }

        public static DataSet GetLodsBySM(string ssn, bool sarcpermission)
        {
            return srchDao.GetLodsBySM(ssn, sarcpermission);
        }

        public static DataSet GetMemberSpecialCaseHistory(string memberSSN, int userId)
        {
            return scDao.GetMemberSpecialCaseHistory(memberSSN, userId);
        }

        public static DataSet GetOpenRestrictedSARCCasesForUser(int userId)
        {
            return GetSARCDao.GetOpenCasesForUser(userId);
        }

        public static DataSet GetPALDataByMemberSSN(string partialSSN, string partialLastName)
        {
            return palDao.GetPALData(partialSSN, partialLastName);
        }

        public static int GetPendingAppealCount(int userId, bool sarc)
        {
            DataSet data = appealDao.GetAppealRequests(userId, sarc);

            if (data == null || data.Tables.Count == 0 || data.Tables[0].Rows.Count == 0)
            {
                return 0;
            }

            return data.Tables[0].Rows.Count;
        }

        public static int GetPendingCount(int userId, bool sarc)
        {
            return LODDao.GetPendingCount(userId, sarc);
        }

        public static int GetPendingIOCount(int userId, bool sarc)
        {
            return LODDao.GetPendingIOCount(userId, sarc);
        }

        //public static int GetPendingLegacyLodCount(int userId, bool sarc)
        //{
        //    return LODDao.GetPendingLegacyLodCount(userId, sarc);
        //}
        public static int GetPendingSARCAppealCount(int userId, bool sarc)
        {
            DataSet data = SARCappealDao.GetAppealRequests(userId, sarc);

            if (data == null || data.Tables.Count == 0 || data.Tables[0].Rows.Count == 0)
            {
                return 0;
            }

            return data.Tables[0].Rows.Count;
        }

        public static DataSet GetPostAppealCompletion(string caseID, string ssn, string name,
                                                  int userId, byte rptView, string compo, int maxCount, byte moduleId, string IsFomal, int unitId, bool sarcpermission)
        {
            return appealDao.GetPostAppealCompletion(caseID, ssn, name, userId, rptView, compo, maxCount, moduleId, IsFomal, unitId, sarcpermission);
        }

        public static DataSet GetPostCompletion(string caseID, string ssn, string name,
                                                  int userId, byte rptView, string compo, int maxCount, byte moduleId, string IsFomal, int unitId, bool sarcpermission, bool searchAllCases, int wsId)
        {
            return srchDao.GetPostCompletion(caseID, ssn, name, userId, rptView, compo, maxCount, moduleId, IsFomal, unitId, sarcpermission, searchAllCases, wsId);
        }

        public static DataSet GetPostSARCAppealCompletion(string caseID, string ssn, string name,
                                                  int userId, byte rptView, string compo, int maxCount, byte moduleId, int unitId, bool sarcpermission)
        {
            return SARCappealDao.GetPostSARCAppealCompletion(caseID, ssn, name, userId, rptView, compo, maxCount, moduleId, unitId, sarcpermission);
        }

        public static int GetReinvestigationRequestCount(int userId, bool sarc)
        {
            return requestDao.GetReinvestigationRequestCount(userId, sarc);
        }

        public static DataSet GetReinvestigationRequests(int userId, bool sarc)
        {
            return requestDao.GetReinvestigationRequests(userId, sarc);
        }

        public static DataSet GetSARCAppealRequests(int userId, bool sarc)
        {
            return SARCappealDao.GetAppealRequests(userId, sarc);
        }

        public static DataSet GetSpecailCaseDisposition(int moduleId, int userId)
        {
            return scDao.GetSpecailCaseDisposition(moduleId, userId);
        }

        public static DataSet GetSpecailCaseNoDisposition(int moduleId, int userId)
        {
            return scDao.GetSpecailCaseNoDisposition(moduleId, userId);
        }

        public static DataSet GetSpecialAwaitingConsult(int wsId)
        {
            return scDao.GetSpecialAwaitingConsult(wsId);
        }

        public static DataSet GetSpecialCaseById(int scId)
        {
            return scDao.GetSpecialCaseById(scId);
        }

        public static int GetSpecialCaseNotHoldingCount(int moduleId, int userId)
        {
            return scDao.GetSpecialCaseNotHoldingCount(moduleId, userId);
        }

        public static DataSet GetSpecialCaseProperties(int moduleId, int scId)
        {
            return scDao.GetSubClassProperties(moduleId, scId);
        }

        public static DataSet GetSpecialCases(int roleId)
        {
            return scDao.GetSpecialCases(roleId);
        }

        public static DataSet GetSpecialCasesByMemberSSN(string ssn, int userId)
        {
            return scDao.GetSpecialCasesByMemberSSN(ssn, userId);
        }

        public static DataSet GetSpecialCasesByModule(int userId, int moduleId)
        {
            return scDao.GetSpecialCasesByModule(moduleId, userId);
        }

        public static int GetSpecialCasesByModuleCount(int moduleId, int userId)
        {
            return scDao.GetSpecialCasesByModuleCount(moduleId, userId);
        }

        public static int GetSpecialCasesCount(int userId)
        {
            return scDao.GetSpecialCasesCount(userId);
        }

        public static DataSet GetSpecialCasesHolding(int moduleId, int userId)
        {
            return scDao.GetSpecialCasesHolding(moduleId, userId);
        }

        public static DataSet GetSpecialCasesNotHolding(int moduleId, int userId)
        {
            return scDao.GetSpecialCasesNotHolding(moduleId, userId);
        }

        public static DataSet GetUndeletedlLods(string caseID, string ssn, string name, int status,
                                                   int userId, byte rptView, string compo, int maxCount, byte moduleId, string IsFomal, int unitId, bool sarcpermission, bool ovreridescope)
        {
            return srchDao.GetUndeleted(caseID, ssn, name, status, userId, rptView, compo, maxCount, moduleId, IsFomal, unitId, sarcpermission, ovreridescope);
        }

        public static DataSet ReinvestigationRequestSearch(string caseID, string ssn, string name, int status, int userId, byte rptView, string compo, int maxCount, byte moduleId, int unitId)
        {
            return requestDao.ReinvestigationRequestSearch(caseID, ssn, name, status, userId, rptView, compo, maxCount, moduleId, unitId);
        }

        public static DataSet ReinvestigationRequestSearch(string caseID, string ssn, string lastName, string firstName, string middleName, int status, int userId, byte rptView, string compo, int maxCount, byte moduleId, int unitId)
        {
            return requestDao.ReinvestigationRequestSearch(caseID, ssn, lastName, firstName, middleName, status, userId, rptView, compo, maxCount, moduleId, unitId);
        }

        public static DataSet SARCAppealRequestSearch(string caseID, string ssn, string name, int status, int userId, byte rptView, string compo, int maxCount, byte moduleId, int unitId)
        {
            return SARCappealDao.SARCAppealRequestSearch(caseID, ssn, name, status, userId, rptView, compo, maxCount, moduleId, unitId);
        }

        public static void SaveUpdate(LineOfDuty lod)
        {
            LODDao.SaveOrUpdate(lod);
        }

        public static DataSet SpecialCaseSearch(string caseId, string ssn, string name, int status, int userId, int rptView, int compo, int maxCount, int module, int unitId, bool sarcpermission)
        {
            return scDao.SpecialCaseSearch(caseId, ssn, name, status, userId, compo, unitId, rptView, module, maxCount, sarcpermission);
        }

        public static DataSet SpecialCaseSearch(string caseId, string ssn, string lastName, string firstName, string middleName, int status, int userId, int rptView, int compo, int maxCount, int module, int unitId, bool sarcpermission)
        {
            return scDao.SpecialCaseSearch(caseId, ssn, lastName, firstName, middleName, status, userId, compo, unitId, rptView, module, maxCount, sarcpermission);
        }
    }
}