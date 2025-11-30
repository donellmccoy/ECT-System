using ALOD.Core.Domain.Workflow;
using ALOD.Core.Interfaces.DAOInterfaces;
using System.Collections.Generic;
using System.Linq;

namespace ALOD.Data.Services
{
    /// <summary>
    /// Service for managing workflow configurations and workflow history.
    /// Provides functionality for workflow state management, transitions, and tracking.
    /// </summary>
    public class WorkFlowService
    {
        private static IWorkflowOptionDao _dao;
        private static IWorkStatusTrackingDao _wstDao;

        public static IWorkflowOptionDao Dao
        {
            get
            {
                if (_dao == null)
                    _dao = new NHibernateDaoFactory().GetWorkflowOptionDao();

                return _dao;
            }
        }

        public static IWorkStatusTrackingDao wstDao
        {
            get
            {
                if (_wstDao == null)
                    _wstDao = new NHibernateDaoFactory().GetWorkStatusTrackingDao();

                return _wstDao;
            }
        }

        public static int GetLastStatus(int refId, short module)
        {
            return wstDao.GetLastStatus(refId, module);
        }

        public static ALOD.Core.Domain.Workflow.WorkflowStatusOption GetOptionById(int wsoId)
        {
            return Dao.GetById(wsoId);
        }

        public static IList<ALODPermission> GetPermissions()
        {
            IALODPermissionDao dao = new NHibernateDaoFactory().GetPermissionDao();
            IList<ALODPermission> lst = dao.GetAll().ToList();
            return lst;
        }

        public static IList<WorkStatusTracking> GetWorkStatusTracking(int refId, short module)
        {
            return wstDao.GetWorkStatusTracking(refId, module);
        }

        public static string ViewDescription(ReportingView view)
        {
            switch (view)
            {
                case ReportingView.System_Administration_View:
                    return "System Administration View";

                case ReportingView.Total_View:
                    return "Total View";

                case ReportingView.JA_View:
                    return "JA View";

                case ReportingView.Medical_Reporting_View:
                    return "Medical Reporting View";

                case ReportingView.MPF_View:
                    return "MPF View";

                case ReportingView.RMU_View_Physical_Responsibility:
                    return "RMU View Physical Responsibility";

                case ReportingView.Non_Medical_Reporting_View:
                    return "Non Medical Reporting View";

                case ReportingView.Old_RMU_type_view:
                    return "Old RMU type view";

                default:
                    return "";
            }
            ;
        }
    }
}