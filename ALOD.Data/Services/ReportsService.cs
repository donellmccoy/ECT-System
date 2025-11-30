using ALOD.Core.Domain.Common;
using ALOD.Core.Domain.Users;
using ALOD.Core.Interfaces.DAOInterfaces;
using ALOD.Logging;
using System;
using System.Collections.Specialized;

namespace ALOD.Data.Services
{
    public class ReportsService
    {
        #region Fields...

        private static IApplicationWarmupProcessDao _appWarmupDao;
        private static IEmailDao _emailDao;
        private static IEmailTemplateDao _emailTemplateDao;
        private static IReportsDao _reportsDao;

        #endregion Fields...

        #region Properties...

        protected static IApplicationWarmupProcessDao AppWarmupDao
        {
            get
            {
                if (_appWarmupDao == null)
                    _appWarmupDao = new NHibernateDaoFactory().GetApplicationWarmupProcessDao();

                return _appWarmupDao;
            }
        }

        protected static IEmailDao EmailDao
        {
            get
            {
                if (_emailDao == null)
                    _emailDao = new EmailService();

                return _emailDao;
            }
        }

        protected static IEmailTemplateDao EmailTemplateDao
        {
            get
            {
                if (_emailTemplateDao == null)
                    _emailTemplateDao = new NHibernateDaoFactory().GetEmailTemplateDao();

                return _emailTemplateDao;
            }
        }

        protected static IReportsDao ReportsDao
        {
            get
            {
                if (_reportsDao == null)
                    _reportsDao = new NHibernateDaoFactory().GetReportsDao();

                return _reportsDao;
            }
        }

        #endregion Properties...

        #region Application Warmup Procedures...

        /// <summary>
        /// Executes all scheduled report processes for the specified date.
        /// Generates and distributes automated reports based on configured schedules.
        /// </summary>
        /// <param name="executionDate">The date to execute the report processes for.</param>
        /// <param name="hostName">The application hostname used in email links and notifications.</param>
        public static void ExecuteApplicationWarmupProcesses(DateTime executionDate, string hostName)
        {
            ExecuteQuarterlyProgramStatusNotificationProcess(executionDate, hostName);
            ExecuteAnnualProgramStatusNotificationProcess(executionDate, hostName);
        }

        protected static void ExecuteAnnualProgramStatusNotificationProcess(DateTime executionDate, string hostName)
        {
            const string processName = "AnnualProgramStatusNotification";

            try
            {
                // Check if this is not an active process...
                if (!AppWarmupDao.IsProcessActive(processName))
                {
                    return;
                }

                // This report should only execute on January 1 of each year (i.e. only once a year)...
                if (!(executionDate.Month == 1 && executionDate.Day == 1))
                    return;

                // Check if this has already been done for the current month...
                if (HasProcessAlreadyExecuted(processName, executionDate))
                {
                    return;
                }

                LogManager.LogApplicationWarmupProcess(processName, executionDate, processName + " Process Started...");

                EmailTemplate template = EmailTemplateDao.GetTemplateByTitle("Quarterly-Annual Program Status Notification (LOD PM)");

                if (template == null)
                {
                    return;
                }

                // Generate LOD PM emails...
                MailManager emails = GetEmails(EmailDao.GetEmailListForUsersByGroup((int)UserGroups.LOD_PM), template.Id);

                emails.SetField("PERIOD_TYPE", "yearly");
                emails.SetField("APP_LINK", hostName);

                emails.SendAll(template.Id);

                // Generate Board emails...
                template = EmailTemplateDao.GetTemplateByTitle("Quarterly-Annual Program Status Notification (Board)");

                if (template == null)
                {
                    return;
                }

                emails = GetEmails(EmailDao.GetEmailListForBoardLevelUsers(), template.Id);

                emails.SetField("PERIOD_TYPE", "yearly");
                emails.SetField("APP_LINK", hostName);

                emails.SendAll(template.Id);

                LogManager.LogApplicationWarmupProcess(processName, executionDate, processName + " Process Completed...");
            }
            catch (Exception e)
            {
                LogManager.LogApplicationWarmupProcess(processName, executionDate, processName + " Process Failed...Error Message: \"" + e.Message + "\"...see error log for further details...");
                LogManager.LogError(e);
            }
        }

        protected static void ExecuteQuarterlyProgramStatusNotificationProcess(DateTime executionDate, string hostName)
        {
            const string processName = "QuarterlyProgramStatusNotification";

            try
            {
                // Check if this is not an active process...
                if (!AppWarmupDao.IsProcessActive(processName))
                {
                    return;
                }

                // Quarterly notifications only go out on Apr 1st, Jul 1st, Oct 1st, and Jan 1st...
                if (!((executionDate.Month == 1 || executionDate.Month == 4 || executionDate.Month == 7 || executionDate.Month == 10) && executionDate.Day == 1))
                {
                    return;
                }

                // Check if this has already been done for the current month...
                if (HasProcessAlreadyExecuted(processName, executionDate))
                {
                    return;
                }

                LogManager.LogApplicationWarmupProcess(processName, executionDate, processName + " Process Started...");

                EmailTemplate template = EmailTemplateDao.GetTemplateByTitle("Quarterly-Annual Program Status Notification (LOD PM)");

                if (template == null)
                {
                    return;
                }

                // Generate LOD PM emails...
                MailManager emails = GetEmails(EmailDao.GetEmailListForUsersByGroup((int)UserGroups.LOD_PM), template.Id);

                emails.SetField("PERIOD_TYPE", "quarterly");
                emails.SetField("APP_LINK", hostName);

                emails.SendAll(template.Id);

                // Generate Board emails...
                template = EmailTemplateDao.GetTemplateByTitle("Quarterly-Annual Program Status Notification (Board)");

                if (template == null)
                {
                    return;
                }

                emails = GetEmails(EmailDao.GetEmailListForBoardLevelUsers(), template.Id);

                emails.SetField("PERIOD_TYPE", "quarterly");
                emails.SetField("APP_LINK", hostName);

                emails.SendAll(template.Id);

                LogManager.LogApplicationWarmupProcess(processName, executionDate, processName + " Process Completed...");
            }
            catch (Exception e)
            {
                LogManager.LogApplicationWarmupProcess(processName, executionDate, processName + " Process Failed...Error Message: \"" + e.Message + "\"...see error log for further details...");
                LogManager.LogError(e);
            }
        }

        private static MailManager GetEmails(StringCollection toList, int templateId)
        {
            MailManager emails = new MailManager(EmailDao);

            emails.AddTemplate(templateId, "", toList);

            return emails;
        }

        private static bool HasProcessAlreadyExecuted(string processName, DateTime currentExecutionDate)
        {
            DateTime? lastExecutionDate = null;

            lastExecutionDate = AppWarmupDao.FindProcessLastExecutionDate(processName);

            if (lastExecutionDate.HasValue && lastExecutionDate.Value.Year == currentExecutionDate.Year && lastExecutionDate.Value.Month == currentExecutionDate.Month)
            {
                return true;
            }

            return false;
        }

        #endregion Application Warmup Procedures...
    }
}