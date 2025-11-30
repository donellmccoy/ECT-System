using ALOD.Core.Domain.Common;
using ALOD.Core.Interfaces.DAOInterfaces;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Data.SqlClient;

namespace ALOD.Data.Services
{
    /// <summary>
    /// Service for creating and managing email messages and distribution lists.
    /// Implements IEmailDao to provide email template processing, distribution list management, and message sending capabilities.
    /// </summary>
    public class EmailService : IEmailDao
    {
        /// <summary>
        /// Data access object for email templates.
        /// </summary>
        public static IEmailTemplateDao templateDao;
        
        /// <summary>
        /// Data store for executing database operations.
        /// </summary>
        private SqlDataStore dataSource;

        /// <summary>
        /// Initializes a new instance of the EmailService class.
        /// </summary>
        public EmailService()
        {
            if (templateDao == null)
                templateDao = new NHibernateDaoFactory().GetEmailTemplateDao();
            if (dataSource == null)
                dataSource = new SqlDataStore();
        }

        /// <summary>
        /// Creates an email message using a template and sends to multiple recipients.
        /// </summary>
        /// <param name="templateId">The ID of the email template to use.</param>
        /// <param name="from">The sender's email address.</param>
        /// <param name="toList">The collection of recipient email addresses.</param>
        /// <returns>An EMailMessage object configured with the template data.</returns>
        public virtual EMailMessage CreateMessage(int templateId, string from, StringCollection toList)
        {
            EmailTemplate template = templateDao.GetById(templateId);
            if (template.DataProc != null)
            {
                if (template.DataProc.Length != 0)
                {
                    StringDictionary collection = GetTemplateData(template.DataProc);
                    template.PopulateData(collection);
                }
            }

            ALOD.Core.Domain.Common.EMailMessage msg = new EMailMessage(template.Subject, template.Body, from, toList);
            return msg;
        }

        /// <summary>
        /// Creates an email message using a template and sends to a single recipient.
        /// </summary>
        /// <param name="templateId">The ID of the email template to use.</param>
        /// <param name="from">The sender's email address.</param>
        /// <param name="to">The recipient's email address.</param>
        /// <returns>An EMailMessage object configured with the template data.</returns>
        public virtual EMailMessage CreateMessage(int templateId, string from, string to)
        {
            EmailTemplate template = templateDao.GetById(templateId);
            if (template.DataProc != null)
            {
                if (template.DataProc.Length != 0)
                {
                    StringDictionary collection = GetTemplateData(template.DataProc);
                    template.PopulateData(collection);
                }
            }

            ALOD.Core.Domain.Common.EMailMessage msg = new EMailMessage(template.Subject, template.Body, from, to);
            return msg;
        }

        /// <summary>
        /// Creates an email message with custom subject and body.
        /// </summary>
        /// <param name="subject">The email subject.</param>
        /// <param name="body">The email body content.</param>
        /// <param name="from">The sender's email address.</param>
        /// <param name="toList">The collection of recipient email addresses.</param>
        /// <returns>An EMailMessage object with the specified content.</returns>
        public virtual EMailMessage CreateMessage(string subject, string body, string from, StringCollection toList)
        {
            ALOD.Core.Domain.Common.EMailMessage msg = new EMailMessage(subject, body, from, toList);
            return msg;
        }

        /// <summary>
        /// Gets the distribution list for a specific group and calling context.
        /// </summary>
        /// <param name="refId">The reference ID of the case.</param>
        /// <param name="groupId">The ID of the group.</param>
        /// <param name="callingGroup">The name of the calling group.</param>
        /// <param name="isFinal">Whether this is the final notification.</param>
        /// <returns>A collection of email addresses for the specified group.</returns>
        public StringCollection GetDistributionListByGroup(int refId, int groupId, string callingGroup, bool isFinal = true)
        {
            StringCollection collection = new StringCollection();
            SqlDataStore.RowReader rowReader = (SqlDataStore adapter, IDataReader reader) =>
            {
                collection.Add(adapter.GetString(reader, 0));
            };
            dataSource.ExecuteReader(rowReader, "core_user_sp_GetMailingListByGroup", refId, groupId, callingGroup, isFinal);
            return collection;
        }

        /// <summary>
        /// Gets the distribution list for special cases by group.
        /// </summary>
        /// <param name="refId">The reference ID of the case.</param>
        /// <param name="groupId">The ID of the group.</param>
        /// <returns>A collection of email addresses for the specified group.</returns>
        public StringCollection GetDistributionListByGroupSC(int refId, int groupId)
        {
            StringCollection collection = new StringCollection();
            SqlDataStore.RowReader rowReader = (SqlDataStore adapter, IDataReader reader) =>
            {
                collection.Add(adapter.GetString(reader, 0));
            };
            dataSource.ExecuteReader(rowReader, "core_user_sp_GetMailingListByGroup_SC", refId, groupId);
            return collection;
        }

        /// <summary>
        /// Gets the distribution list for users with specific roles at a unit.
        /// </summary>
        /// <param name="compo">The component identifier.</param>
        /// <param name="unitId">The unit ID.</param>
        /// <param name="roles">Comma-separated list of role names.</param>
        /// <returns>A collection of email addresses for users with the specified roles.</returns>
        public StringCollection GetDistributionListByRoles(string compo, int unitId, string roles)
        {
            StringCollection collection = new StringCollection();
            SqlDataStore.RowReader rowReader = (SqlDataStore adapter, IDataReader reader) =>
            {
                collection.Add(adapter.GetString(reader, 0));
            };
            dataSource.ExecuteReader(rowReader, "core_user_sp_GetMailingListByRoles", compo, unitId, roles);
            return collection;
        }

        /// <summary>
        /// Gets the distribution list based on work status for a LOD case.
        /// </summary>
        /// <param name="refId">The reference ID of the LOD case.</param>
        /// <param name="workStatus">The work status code.</param>
        /// <returns>A collection of email addresses for users with the specified status.</returns>
        public StringCollection GetDistributionListByStatus(int refId, short workStatus)
        {
            StringCollection collection = new StringCollection();
            SqlDataStore.RowReader rowReader = (SqlDataStore adapter, IDataReader reader) =>
            {
                collection.Add(adapter.GetString(reader, 0));
            };
            dataSource.ExecuteReader(rowReader, "core_user_sp_GetMailingListByStatus", refId, workStatus);
            return collection;
        }

        /// <summary>
        /// Gets the distribution list by status for SARC cases.
        /// </summary>
        /// <param name="refId">The reference ID of the case.</param>
        /// <param name="workStatus">The work status code.</param>
        /// <returns>A collection of email addresses for users with the specified status.</returns>
        public StringCollection GetDistributionListByStatusSARC(int refId, short workStatus)
        {
            StringCollection collection = new StringCollection();
            SqlDataStore.RowReader rowReader = (SqlDataStore adapter, IDataReader reader) =>
            {
                collection.Add(adapter.GetString(reader, 0));
            };
            dataSource.ExecuteReader(rowReader, "core_user_sp_GetMailingListByStatus_SARC", refId, workStatus);
            return collection;
        }

        /// <summary>
        /// Gets the distribution list by status for special cases.
        /// </summary>
        /// <param name="refId">The reference ID of the special case.</param>
        /// <param name="workStatus">The work status code.</param>
        /// <returns>A collection of email addresses for users with the specified status.</returns>
        public StringCollection GetDistributionListByStatusSC(int refId, short workStatus)
        {
            StringCollection collection = new StringCollection();
            SqlDataStore.RowReader rowReader = (SqlDataStore adapter, IDataReader reader) =>
            {
                collection.Add(adapter.GetString(reader, 0));
            };
            dataSource.ExecuteReader(rowReader, "core_user_sp_GetMailingListByStatus_SC", refId, workStatus);
            return collection;
        }

        /// <summary>
        /// Gets the distribution list based on system-wide email preferences and user groups.
        /// </summary>
        /// <param name="includeWork">Whether to include work email addresses.</param>
        /// <param name="includePersonal">Whether to include personal email addresses.</param>
        /// <param name="includeUnit">Whether to include unit-based email addresses.</param>
        /// <param name="userGroups">A collection of user group records to filter by.</param>
        /// <returns>A collection of email addresses matching the specified parameters.</returns>
        public StringCollection GetDistributionListBySystemParameters(bool includeWork, bool includePersonal, bool includeUnit, IList<Microsoft.SqlServer.Server.SqlDataRecord> userGroups)
        {
            StringCollection collection = new StringCollection();

            // Had to circumvent the SqlDataStore in order to get table valued parameters to work...
            using (var con = dataSource.GetSqlConnection())
            {
                con.Open();

                string execCmd = String.Empty;

                if (userGroups.Count > 0)
                {
                    execCmd = "EXEC core_user_sp_GetMailingListBySystemParams @includeWork, @includePersonal, @includeUnit, @userGroupList";
                }
                else
                {
                    execCmd = "EXEC core_user_sp_GetMailingListBySystemParams @includeWork, @includePersonal, @includeUnit";
                }

                using (SqlCommand cmd = new SqlCommand(execCmd, con))
                {
                    SqlParameter sqlParam = new SqlParameter("@includeWork", includeWork);
                    sqlParam.DbType = DbType.Boolean;
                    cmd.Parameters.Add(sqlParam);

                    sqlParam = new SqlParameter("@includePersonal", includePersonal);
                    sqlParam.DbType = DbType.Boolean;
                    cmd.Parameters.Add(sqlParam);

                    sqlParam = new SqlParameter("@includeUnit", includeUnit);
                    sqlParam.DbType = DbType.Boolean;
                    cmd.Parameters.Add(sqlParam);

                    if (userGroups.Count > 0)
                    {
                        sqlParam = new SqlParameter("@userGroupList", SqlDbType.Structured);
                        sqlParam.TypeName = "dbo.tblIntegerList";
                        sqlParam.Value = userGroups;
                        cmd.Parameters.Add(sqlParam);
                    }

                    using (var dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            collection.Add(dr["EmailAddress"].ToString());
                        }
                    }
                }

                con.Close();
            }

            return collection;
        }

        /// <summary>
        /// Gets the distribution list by workflow, work status, and group.
        /// </summary>
        /// <param name="refId">The reference ID of the case.</param>
        /// <param name="workstatus">The work status code.</param>
        /// <param name="workflowId">The workflow ID.</param>
        /// <param name="groupId">The group ID.</param>
        /// <returns>A collection of email addresses for users matching the workflow criteria.</returns>
        public StringCollection GetDistributionListByWorkflow(int refId, int workstatus, int workflowId, int groupId)
        {
            StringCollection collection = new StringCollection();
            SqlDataStore.RowReader rowReader = (SqlDataStore adapter, IDataReader reader) =>
            {
                collection.Add(adapter.GetString(reader, 0));
            };
            dataSource.ExecuteReader(rowReader, "core_user_sp_GetMailingListByWorkflow", refId, workstatus, workflowId, groupId);
            return collection;
        }

        /// <summary>
        /// Gets the distribution list for a LOD based on group, work status, and calling context.
        /// </summary>
        /// <param name="refId">The reference ID of the LOD case.</param>
        /// <param name="groupId">The ID of the group.</param>
        /// <param name="workStatus">The work status code.</param>
        /// <param name="callingGroup">The name of the calling group.</param>
        /// <returns>A collection of email addresses for the LOD distribution.</returns>
        public StringCollection GetDistributionListForLOD(int refId, int groupId, int workStatus, string callingGroup)
        {
            StringCollection collection = new StringCollection();
            SqlDataStore.RowReader rowReader = (SqlDataStore adapter, IDataReader reader) =>
            {
                collection.Add(adapter.GetString(reader, 0));
            };
            dataSource.ExecuteReader(rowReader, "core_user_sp_GetMailingListForLOD", refId, groupId, workStatus, callingGroup);
            return collection;
        }

        /// <summary>
        /// Gets the email list for all board-level users in the system.
        /// </summary>
        /// <returns>A collection of email addresses for board-level users.</returns>
        public StringCollection GetEmailListForBoardLevelUsers()
        {
            StringCollection collection = new StringCollection();

            SqlDataStore.RowReader rowReader = (SqlDataStore adapter, IDataReader reader) =>
            {
                collection.Add(adapter.GetString(reader, 0));
            };

            dataSource.ExecuteReader(rowReader, "core_user_sp_GetMailingListForBoardLevelUsers");

            return collection;
        }

        /// <summary>
        /// Summary for GetEmailListForLessonsLearned Method:
        /// Get the Distribution List to send an email notifying those users that an LOD has Lessons Learned.
        /// </summary>
        /// <param name="refId">Lod Id - Primary Key.</param>
        /// <returns>StringCollection - Mailing List.</returns>
        public StringCollection GetEmailListForLessonsLearned(int refId, int moduleId)
        {
            StringCollection collection = new StringCollection();
            SqlDataStore.RowReader rowReader = (SqlDataStore adapter, IDataReader reader) =>
            {
                collection.Add(adapter.GetString(reader, 0));
            };

            dataSource.ExecuteReader(rowReader, "core_user_sp_GetMailingListForLessonsLearned", refId, moduleId);

            return collection;
        }

        /// <summary>
        /// Gets the email list for a specific user.
        /// </summary>
        /// <param name="userId">The ID of the user.</param>
        /// <returns>A collection of email addresses for the user.</returns>
        public StringCollection GetEmailListForUser(int userId)
        {
            StringCollection collection = new StringCollection();
            SqlDataStore.RowReader rowReader = (SqlDataStore adapter, IDataReader reader) =>
            {
                collection.Add(adapter.GetString(reader, 0));
            };
            dataSource.ExecuteReader(rowReader, "core_user_sp_GetMailingListByUser", userId);
            return collection;
        }

        /// <summary>
        /// Gets the email list for all users in a specific group.
        /// </summary>
        /// <param name="groupId">The ID of the group.</param>
        /// <returns>A collection of email addresses for users in the group.</returns>
        public StringCollection GetEmailListForUsersByGroup(int groupId)
        {
            StringCollection collection = new StringCollection();

            SqlDataStore.RowReader rowReader = (SqlDataStore adapter, IDataReader reader) =>
            {
                collection.Add(adapter.GetString(reader, 0));
            };

            dataSource.ExecuteReader(rowReader, "core_user_sp_GetMailingListByGroupId", groupId);

            return collection;
        }

        /// <summary>
        /// Retrieves template data from a stored procedure to populate email template variables.
        /// </summary>
        /// <param name="storeproc">The name of the stored procedure to execute.</param>
        /// <returns>A StringDictionary containing key-value pairs of template data.</returns>
        public StringDictionary GetTemplateData(string storeproc)
        {
            StringDictionary collection = new StringDictionary();
            SqlDataStore.RowReader rowReader = (SqlDataStore adapter, IDataReader reader) =>
            {
                collection.Add(adapter.GetString(reader, 0), adapter.GetString(reader, 1));
            };
            dataSource.ExecuteReader(rowReader, storeproc);
            return collection;
        }

        /// <summary>
        /// Verifies if any of the Board Members has added a Lessons Learned to the LOD.
        /// Uses the stored procedure core_user_sp_GetMailingListByUser.
        /// </summary>
        /// <param name="refId">LOD ID - Primary Key.</param>
        /// <returns>True if LOD has Lessons Learned; otherwise, false.</returns>
        public bool LODHasLessonsLearned(int refId)
        {
            bool returnValue = false;
            int countLessonsLearned = 0;
            SqlDataStore.RowReader rowReader = (SqlDataStore adapter, IDataReader reader) =>
            {
                countLessonsLearned = adapter.GetInt32(reader, 0);
            };
            dataSource.ExecuteReader(rowReader, "lod_sp_GetNumberOfLessonsLearned", refId);

            if (countLessonsLearned > 0)
            {
                returnValue = true;
            }

            return returnValue;
        }

        /// <summary>
        /// Sends an email using the specified template to a list of recipients.
        /// </summary>
        /// <param name="templateId">The ID of the email template to use.</param>
        /// <param name="from">The sender's email address.</param>
        /// <param name="toList">The collection of recipient email addresses.</param>
        public virtual void SendEmail(int templateId, string from, StringCollection toList)
        {
            ALOD.Core.Domain.Common.EMailMessage msg = CreateMessage(templateId, from, toList);
            msg.Send();
        }
    }
}