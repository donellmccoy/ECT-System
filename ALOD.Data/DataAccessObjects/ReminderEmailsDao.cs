using ALOD.Core.Interfaces.DAOInterfaces;
using System;
using System.Data;

namespace ALOD.Data
{
    /// <summary>
    /// Data access object for reminder email operations.
    /// Manages automated email reminders for case status changes, inactive accounts, and workflow-based notifications.
    /// </summary>
    public class ReminderEmailsDao : IReminderEmailDao
    {
        /// <summary>
        /// Disables an inactive user account.
        /// </summary>
        /// <param name="userid">The user ID to disable.</param>
        /// <returns>The number of rows affected.</returns>
        public int DisableAccount(int userid)
        {
            SqlDataStore store = new SqlDataStore();
            return store.ExecuteNonQuery("ReminderDisableInactiveAccount", userid);
        }

        /// <summary>
        /// Retrieves a list of inactive user accounts that should be disabled.
        /// </summary>
        /// <returns>A dataset containing inactive account information.</returns>
        public DataSet GetInactiveAccounts()
        {
            SqlDataStore store = new SqlDataStore();
            return store.ExecuteDataSet("ReminderGetUsersToDisable");
        }

        /// <summary>
        /// Retrieves a list of email addresses for inactive account notifications.
        /// </summary>
        /// <returns>A dataset containing email addresses for inactive accounts.</returns>
        public DataSet GetInactiveEmailList()
        {
            SqlDataStore store = new SqlDataStore();
            return store.ExecuteDataSet("ReminderGetInactiveEmails");
        }

        /// <inheritdoc/>
        public DataSet GetReminderEmailsList()
        {
            SqlDataStore store = new SqlDataStore();
            return store.ExecuteDataSet("ReminderEmailGetList");
        }

        /// <inheritdoc/>
        public DataSet ReminderEmailGetSettingsByStatus(int workflowId, int statusId, int compo)
        {
            SqlDataStore store = new SqlDataStore();
            return store.ExecuteDataSet("ReminderEmailGetSettingsByStatus", workflowId, statusId, compo);
        }

        /// <inheritdoc/>
        public void ReminderEmailInitialStep(int id, int workStatusId, string caseType)
        {
            SqlDataStore store = new SqlDataStore();
            store.ExecuteNonQuery("ReminderEmailInitialStep", id, workStatusId, caseType);
        }

        /// <inheritdoc/>
        public void ReminderEmailsAdd(int newStatus, string caseId, int workflowId)
        {
            SqlDataStore store = new SqlDataStore();
            store.ExecuteNonQuery("ReminderEmailsAdd", newStatus, caseId, workflowId);
        }

        /// <inheritdoc/>
        public void ReminderEmailSettingAddByStatus(int workflowId, int statusId, int compo, int groupId, int templateId, int interval)
        {
            SqlDataStore store = new SqlDataStore();
            store.ExecuteNonQuery("ReminderEmailAddSettingsByStatus", workflowId, statusId, compo, groupId, templateId, interval);
        }

        /// <inheritdoc/>
        public void ReminderEmailSettingsDelete(int id)
        {
            SqlDataStore store = new SqlDataStore();
            store.ExecuteNonQuery("ReminderEmailSettingsDelete", id);
        }

        /// <inheritdoc/>
        public void ReminderEmailSettingsDeleteByStatus(int workflowId, int statusId)
        {
            SqlDataStore store = new SqlDataStore();
            store.ExecuteNonQuery("ReminderEmailSettingsDeleteByStatus", workflowId, statusId);
        }

        /// <inheritdoc/>
        public void ReminderEmailUpdate(int id)
        {
            SqlDataStore store = new SqlDataStore();
            store.ExecuteNonQuery("ReminderEmailUpdate", id);
        }

        /// <inheritdoc/>
        public void ReminderEmailUpdateStatusChange(int oldStatus, int newStatus, string caseId, string caseType)
        {
            SqlDataStore store = new SqlDataStore();
            store.ExecuteNonQuery("ReminderEmailUpdateStatusChange", oldStatus, newStatus, caseId, caseType);
        }

        /// <summary>
        /// Retrieves inactive account reminder settings.
        /// </summary>
        /// <returns>A dataset containing inactive reminder settings.</returns>
        public DataSet ReminderGetInactiveSettings()
        {
            SqlDataStore store = new SqlDataStore();
            return store.ExecuteDataSet("ReminderGetInactiveSettings");
        }

        /// <summary>
        /// Updates the inactive account reminder settings.
        /// </summary>
        /// <param name="interv">The inactivity interval before disabling.</param>
        /// <param name="notifcInterv">The notification interval.</param>
        /// <param name="templateId">The email template ID to use.</param>
        /// <param name="active">Whether inactive account processing is active.</param>
        /// <returns>The number of rows affected.</returns>
        public int ReminderUpdateInactiveSettings(int interv, int notifcInterv, int templateId, Boolean active)
        {
            SqlDataStore store = new SqlDataStore();
            return store.ExecuteNonQuery("ReminderUpdateInactiveSettings", interv, notifcInterv, templateId, active);
        }
    }
}