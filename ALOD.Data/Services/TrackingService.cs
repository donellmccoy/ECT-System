using ALOD.Core.Interfaces.DAOInterfaces;
using System.Data;

namespace ALOD.Data.Services
{
    /// <summary>
    /// Service for managing tracking data and navigation history.
    /// Provides functionality for recording and retrieving user tracking information.
    /// </summary>
    public class TrackingService : DataService
    {
        private static ITrackingDao _dao;

        public static ITrackingDao Dao
        {
            get
            {
                if (_dao == null)
                    _dao = new NHibernateDaoFactory().GetTrackingDao();
                return _dao;
            }
        }

        /// <summary>
        /// Retrieves tracking data for the specified user.
        /// </summary>
        /// <param name="userId">The ID of the user to retrieve tracking data for.</param>
        /// <param name="showAll">Whether to show all tracking records or only active ones.</param>
        /// <returns>A DataSet containing tracking records for the user.</returns>
        public static DataSet GetByUserId(int userId, bool showAll)
        {
            return Dao.GetByUserId(userId, showAll);
        }
    }
}