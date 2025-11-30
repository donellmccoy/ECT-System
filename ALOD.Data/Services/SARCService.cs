using ALOD.Core.Interfaces.DAOInterfaces;
using System.Data;

namespace ALOD.Data.Services
{
    /// <summary>
    /// Service for managing Sexual Assault Response Coordinator (SARC) cases.
    /// Provides functionality for SARC case operations including search and data access.
    /// </summary>
    public class SARCService : DataService
    {
        private static NHibernateDaoFactory _daoFactory;
        private static ISARCDAO _sarcRestrictedDao;

        public static NHibernateDaoFactory DAOFactory
        {
            get
            {
                if (_daoFactory == null)
                    _daoFactory = new NHibernateDaoFactory();

                return _daoFactory;
            }
        }

        public static ISARCDAO SARCDao
        {
            get
            {
                if (_sarcRestrictedDao == null)
                    _sarcRestrictedDao = DAOFactory.GetSARCDao();

                return _sarcRestrictedDao;
            }
        }

        public static DataSet GetPostCompletionSearchResults(int userId, string caseId, string memberSSN, string memberName, int reportView, string compo, int unitId)
        {
            return SARCDao.GetPostCompletionSearchResults(userId, caseId, memberSSN, memberName, reportView, compo, unitId);
        }

        public static int GetRestrictedSARCsCount(int userId)
        {
            return SARCDao.GetCaseCount(userId);
        }

        public static int GetRestrictedSARCsPostCompletionCount(int userId)
        {
            return SARCDao.GetPostCompletionCaseCount(userId);
        }

        public DataSet GetSearchResults(int userId, string caseId, string memberSSN, string memberName, int reportView, string compo, int status, int unitId)
        {
            return SARCDao.GetSearchResults(userId, caseId, memberSSN, memberName, reportView, compo, status, unitId);
        }
    }
}