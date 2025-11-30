using System;

namespace ALOD.Data.Services
{
    /// <summary>
    /// Service for managing system messages and notifications.
    /// Provides functionality for inserting and managing user-facing messages.
    /// </summary>
    public class MessagesService
    {
        private const string SP_INSERT_MESSAGE = "core_messages_sp_InsertMessages";

        /// <summary>
        /// Inserts a new message into the system.
        /// </summary>
        /// <param name="userId">The ID of the user creating the message.</param>
        /// <param name="name">The name of the message.</param>
        /// <param name="title">The title of the message.</param>
        /// <param name="startDate">The start date for message display.</param>
        /// <param name="endDate">The end date for message display.</param>
        /// <param name="popUp">Whether the message should be displayed as a popup.</param>
        /// <param name="message">The message content.</param>
        public static void InsertMessage(int userId, String name, String title, DateTime startDate, DateTime endDate, bool popUp, string message)
        {
            SqlDataStore DataSource = new SqlDataStore();
            DataSource.ExecuteNonQuery(SP_INSERT_MESSAGE, title, name, startDate, endDate, popUp, message, userId);
        }
    }
}