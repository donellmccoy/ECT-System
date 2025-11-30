using ALOD.Core.Domain.Common;
using ALOD.Core.Domain.Modules.Lod;
using ALOD.Core.Domain.Users;
using ALOD.Core.Interfaces.DAOInterfaces;
using ALOD.Logging;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;

namespace ALOD.Data.Services
{
    /// <summary>
    /// Service for managing Psychological Health (PH) cases and related automated processes.
    /// Provides functionality for case search, application warmup processes, and email notifications.
    /// </summary>
    public class PsychologicalHealthService
    {
        private static IApplicationWarmupProcessDao _appWarmupDao;
        private static IEmailTemplateDao _emailTemplateDao;
        private static IPsychologicalHealthDao _phDao;
        private static ISpecialCaseDAO _specCaseDao;

        protected static IApplicationWarmupProcessDao AppWarmupDao
        {
            get
            {
                if (_appWarmupDao == null)
                    _appWarmupDao = new NHibernateDaoFactory().GetApplicationWarmupProcessDao();

                return _appWarmupDao;
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

        protected static IPsychologicalHealthDao PHDao
        {
            get
            {
                if (_phDao == null)
                    _phDao = new NHibernateDaoFactory().GetPsychologicalHealthDao();

                return _phDao;
            }
        }

        protected static ISpecialCaseDAO SpecCaseDao
        {
            get
            {
                if (_specCaseDao == null)
                    _specCaseDao = new NHibernateDaoFactory().GetSpecialCaseDAO();

                return _specCaseDao;
            }
        }

        /// <summary>
        /// Executes all application warmup processes for Psychological Health cases on the specified date.
        /// </summary>
        /// <param name="executionDate">The date to execute the processes for.</param>
        /// <param name="hostName">The application hostname for generating links in emails.</param>
        public static void ExecuteApplicationWarmupProcesses(DateTime executionDate, string hostName)
        {
            ExecutePushReportProcess(executionDate, hostName);
            ExecuteSevenDayWarningProcess(executionDate, hostName);
            ExecuteCollectionProcess(executionDate, hostName);
        }

        /// <summary>
        /// Searches for Psychological Health cases based on multiple criteria.
        /// </summary>
        /// <param name="caseId">The case ID to search for.</param>
        /// <param name="status">The status code to filter by.</param>
        /// <param name="userId">The ID of the user performing the search.</param>
        /// <param name="compo">The component to filter by.</param>
        /// <param name="unitId">The unit ID to filter by.</param>
        /// <param name="rptView">The reporting view type.</param>
        /// <param name="module">The module ID.</param>
        /// <param name="maxCount">Maximum number of results to return.</param>
        /// <param name="reportingMonth">The reporting month to filter by.</param>
        /// <param name="reportingYear">The reporting year to filter by.</param>
        /// <returns>A DataSet containing search results.</returns>
        public static DataSet PHCaseSearch(string caseId, int status, int userId, int compo, int unitId, int rptView, int module, int maxCount, int reportingMonth, int reportingYear)
        {
            return PHDao.PHCaseSearch(caseId, status, userId, compo, unitId, rptView, module, maxCount, reportingMonth, reportingYear);
        }

        protected static void ExecuteCollectionProcess(DateTime executionDate, string hostName)
        {
            const string processName = "PH_CollectionPeriod";

            try
            {
                EmailService mailService = new EmailService();

                // Check if this is not an active process...
                if (!AppWarmupDao.IsProcessActive(processName))
                {
                    return;
                }

                // Check if this is not the 4th day of the month...
                if (executionDate.Day != 4)
                {
                    return;
                }

                // Check if this has already been done for the current month...
                if (HasProcessAlreadyExecuted(processName, executionDate))
                {
                    return;
                }

                LogManager.LogApplicationWarmupProcess(processName, executionDate, processName + " Process Started...");

                EmailTemplate template = EmailTemplateDao.GetTemplateByTitle("SC PH Awaiting Action");

                if (template == null)
                {
                    return;
                }

                // Perform processing on appropriate open cases...send them to the Delinquency status...get the list of case reference Id this occured to...
                DateTime prevMonth = executionDate.AddMonths(-1);

                IList<int> referenceIds = PHDao.ExecuteCollectionProcess(prevMonth);
                MailManager emails;

                // Send awaiting actione mails to appropriate users for cases which were updated in the collection process...
                foreach (int i in referenceIds)
                {
                    emails = GetEmails(mailService.GetDistributionListByStatusSC(i, (byte)SpecCasePHWorkStatus.Delinquent), template.Id);

                    emails.SetField("CASE_NUMBER", SpecCaseDao.GetById(i).CaseId);
                    emails.SetField("APP_LINK", hostName);

                    emails.SendAll(template.Id);
                }

                LogManager.LogApplicationWarmupProcess(processName, executionDate, processName + " Process Completed...");
            }
            catch (Exception e)
            {
                LogManager.LogApplicationWarmupProcess(processName, executionDate, processName + " Process Failed...Error Message: \"" + e.Message + "\"...see error log for further details...");
                LogManager.LogError(e);
            }
        }

        protected static void ExecutePushReportProcess(DateTime executionDate, string hostName)
        {
            const string processName = "PH_PushReport";

            try
            {
                StringCollection unitCollection = null;
                string unitList = String.Empty;

                // Check if this is not an active process...
                if (!AppWarmupDao.IsProcessActive(processName))
                {
                    return;
                }

                // Check if this is not the last day of the month...
                if (executionDate.Day != DateTime.DaysInMonth(executionDate.Year, executionDate.Month))
                {
                    return;
                }

                // Check if this has already been done for the current month...
                if (HasProcessAlreadyExecuted(processName, executionDate))
                {
                    return;
                }

                LogManager.LogApplicationWarmupProcess(processName, executionDate, processName + " Process Started...");

                EmailTemplate template = EmailTemplateDao.GetTemplateByTitle("SC PH Push Report (Units)");

                if (template == null)
                {
                    return;
                }

                // Get the units which have not completed or stareted the current months PH form...
                unitCollection = PHDao.GetPushReportUnitsList(executionDate);

                foreach (string s in unitCollection)
                {
                    if (!string.IsNullOrEmpty(unitList))
                    {
                        unitList = unitList + Environment.NewLine;
                    }

                    unitList = unitList + s;
                }

                // Generate HQ AFRC DPH emails...
                MailManager emails = GetEmails(PHDao.GetPushReportEmailList(executionDate, (int)UserGroups.HQAFRCDPH), template.Id);

                emails.SetField("DELINQUENT_UNITS", unitList);
                emails.SetField("APP_LINK", hostName);

                emails.SendAll(template.Id);

                // Generate Unit PH emails...
                template = EmailTemplateDao.GetTemplateByTitle("SC PH Push Report (Reminder)");

                if (template == null)
                {
                    return;
                }

                emails = GetEmails(PHDao.GetPushReportEmailList(executionDate, (int)UserGroups.UnitPH), template.Id);

                emails.SetField("REPORTING_PERIOD", executionDate.ToString("y"));
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

        protected static void ExecuteSevenDayWarningProcess(DateTime executionDate, string hostName)
        {
            const string processName = "PH_SevenDayWarning";

            try
            {
                // Check if this is not an active process...
                if (!AppWarmupDao.IsProcessActive(processName))
                {
                    return;
                }

                // Check if this is not the seventh to last day of the month...
                if (executionDate.Day != (DateTime.DaysInMonth(executionDate.Year, executionDate.Month) - 6))
                {
                    return;
                }

                // Check if this has already been done for the current month...
                if (HasProcessAlreadyExecuted(processName, executionDate))
                {
                    return;
                }

                LogManager.LogApplicationWarmupProcess(processName, executionDate, processName + " Process Started...");

                EmailTemplate template = EmailTemplateDao.GetTemplateByTitle("SC PH Close Out");

                if (template == null)
                {
                    return;
                }

                // Generate emails...
                MailManager emails = GetEmails(PHDao.GetSevenDayWarningEmailList(executionDate), template.Id);

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
            EmailService mailService = new EmailService();
            MailManager emails = new MailManager(mailService);

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
    }
}