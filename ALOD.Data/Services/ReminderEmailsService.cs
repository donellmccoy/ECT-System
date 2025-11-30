using ALOD.Core.Domain.Common;
using ALOD.Core.Utils;
using System;
using System.Data;

namespace ALOD.Data.Services
{
    /// <summary>
    /// Service for managing automated reminder emails and account disabling processes.
    /// Handles scheduled email reminders and automatic account deactivation based on inactivity.
    /// </summary>
    public class ReminderEmailsService
    {
        private ReminderEmailsDao dao = new ALOD.Data.ReminderEmailsDao();

        /// <summary>
        /// Automatically disables user accounts that have been inactive for a defined period.
        /// Logs the disable action and records the change set for audit purposes.
        /// </summary>
        public virtual void DisableInactiveAccounts()
        {
            DataSet disableAccounts = new DataSet();
            disableAccounts = dao.GetInactiveAccounts();

            foreach (DataTable table in disableAccounts.Tables)
            {
                foreach (DataRow row in table.Rows)
                {
                    int userId = Convert.ToInt32(row["userId"]);
                    int logId;
                    dao.DisableAccount(userId);

                    // Due to circular dependecies, could not get object Changeset or Changerow
                    //Code in those methods are duplicated here
                    XMLString xml = new XMLString("XML_Array");

                    logId = ALOD.Logging.LogManager.LogActionIndependant(1, Logging.UserAction.AutoDisable, userId, "Disabled Automatically Due to Inactivity");

                    xml.BeginElement("XMLList");
                    xml.WriteAttribute("ID", logId.ToString());
                    xml.WriteAttribute("section", "Account Status");
                    xml.WriteAttribute("field", "Status");
                    xml.WriteAttribute("oldVal", "Approved");
                    xml.WriteAttribute("newVal", "Disabled");
                    xml.EndElement();

                    SqlDataStore adapter = new SqlDataStore();

                    adapter.ExecuteNonQuery("core_log_sp_InsertChangeSet", xml.Value);
                }
            }
        }

        public virtual void SendEmailReminders(string hostname)
        {
            SendReminderNotification(hostname);
            SendInactivityNotification(hostname);
        }

        public virtual void SendInactivityNotification(string hostname)
        {
            DataSet reminderEmails = new DataSet();
            reminderEmails = dao.GetInactiveEmailList();

            foreach (DataTable table in reminderEmails.Tables)
            {
                foreach (DataRow row in table.Rows)
                {
                    EmailService mailService = new EmailService();
                    MailManager emails = new MailManager(mailService);

                    emails.AddTemplate(Convert.ToInt32(row["templateId"]), "", row["email"].ToString());

                    emails.SetField("DAYS_INACTIVE", Convert.ToString(row["daysInactive"]));
                    emails.SetField("DATE_TO_DISABLE", Convert.ToString(row["dateToDisable"]));
                    emails.SetField("APP_LINK", hostname);
                    emails.SendAll(Convert.ToInt32(row["templateId"]), Convert.ToInt32(row["userId"]));
                }
            }
        }

        public virtual void SendReminderNotification(string hostname)
        {
            DataSet reminderEmails = new DataSet();
            reminderEmails = dao.GetReminderEmailsList();

            foreach (DataTable table in reminderEmails.Tables)
            {
                foreach (DataRow row in table.Rows)
                {
                    EmailService mailService = new EmailService();
                    MailManager emails = new MailManager(mailService);

                    emails.AddTemplate(Convert.ToInt32(row["templateId"]), "", row["email"].ToString());

                    emails.SetField("MODULE_NAME", row["moduleName"].ToString());
                    emails.SetField("CASE_NUMBER", row["caseId"].ToString());
                    emails.SetField("PENDING_DAYS", row["daysPending"].ToString());
                    emails.SetField("APP_LINK", hostname);
                    emails.SendAll(Convert.ToInt32(row["templateId"]));

                    dao.ReminderEmailUpdate(Convert.ToInt32(row["id"].ToString()));
                }
            }
        }
    }
}