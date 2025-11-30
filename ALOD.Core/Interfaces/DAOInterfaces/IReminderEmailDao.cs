using System.Data;

namespace ALOD.Core.Interfaces.DAOInterfaces
{
    public interface IReminderEmailDao
    {
        DataSet GetReminderEmailsList();

        DataSet ReminderEmailGetSettingsByStatus(int workflowId, int statusId, int compo);

        void ReminderEmailInitialStep(int id, int workStatusId, string caseType);

        void ReminderEmailsAdd(int newStatus, string caseId, int workflowId);

        void ReminderEmailSettingAddByStatus(int workflowId, int statusId, int compo, int groupId, int templateId, int interval);

        void ReminderEmailSettingsDelete(int id);

        void ReminderEmailSettingsDeleteByStatus(int workflowId, int statusId);

        void ReminderEmailUpdate(int id);

        void ReminderEmailUpdateStatusChange(int oldStatus, int newStatus, string caseId, string caseType);
    }
}