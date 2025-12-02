using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Web;
using ALOD.Core.Domain.Users;
using ALOD.Data;

namespace ALODWebUtility.Common
{
    public class MessageList : IList<Message>
    {
        protected List<Message> _message = new List<Message>();

        #region Messages

        /// <summary>
        /// Insert Message into Table
        /// </summary>
        /// <param name="title">The Message's Title</param>
        /// <param name="name"></param>
        /// <param name="startDate">StartDate of the Message</param>
        /// <param name="endDate">EndDate of the Message</param>
        /// <param name="popUp">If message is a popup</param>
        /// <param name="message">Message</param>
        /// <returns></returns>
        /// <remarks></remarks>
        public int InsertMessage(string title, string name, DateTime startDate, DateTime endDate, bool popUp, string message)
        {
            SqlDataStore adapter = new SqlDataStore();
            int userId = Convert.ToInt32(HttpContext.Current.Session["UserId"]);
            return Convert.ToInt32(adapter.ExecuteScalar("core_messages_sp_InsertMessages", title, name, startDate, endDate, popUp, message, userId));
        }

        /// <summary>
        /// Returns a list of all messages for editing in the MsgAdmin page.
        /// </summary>
        /// <param name="compo"></param>
        /// <returns>A data table containing the results.</returns>
        /// <remarks></remarks>
        public MessageList RetrieveAllMessages(int compo, bool isAdmin)
        {
            SqlDataStore adapter = new SqlDataStore();
            DbCommand cmd = adapter.GetStoredProcCommand("core_messages_sp_GetAllMessages", compo, isAdmin);
            adapter.ExecuteReader(RetrieveAllMessagesReader, cmd);
            return this;
        }

        /// <summary>
        /// Returns a list of UserGroups assigned and unassigned.
        /// </summary>
        /// <param name="messageid"></param>
        /// <param name="compo"></param>
        /// <returns>A data table containing the results.</returns>
        /// <remarks></remarks>
        public MessageList RetrieveMessageGroups(short messageId, string compo)
        {
            SqlDataStore adapter = new SqlDataStore();
            AppUser user = ALOD.Data.Services.UserService.CurrentUser();
            DbCommand cmd = adapter.GetStoredProcCommand("core_messages_sp_GetMessagesGroups", messageId, compo, user.CurrentRole.Id);
            adapter.ExecuteReader(RetrieveMessageGroupsReader, cmd);
            return this;
        }

        /// <summary>
        /// Returns a list of new messages.
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="groupId"></param>
        /// <param name="popup"></param>
        /// <returns>A data table containing the results.</returns>
        /// <remarks></remarks>
        public MessageList RetrieveMessages(int userId, int groupId, bool popup)
        {
            SqlDataStore adapter = new SqlDataStore();
            DbCommand cmd = adapter.GetStoredProcCommand("core_messages_sp_GetMessages", userId, groupId, popup);
            adapter.ExecuteReader(RetrieveMessagesReader, cmd);

            return this;
        }

        public void SetMessagesRead(int userId, byte groupId)
        {
            SqlDataStore adapter = new SqlDataStore();
            adapter.ExecuteNonQuery("core_messages_sp_UpdateMessagesRead", userId, groupId);
        }

        /// <summary>
        /// Updates the Message Details.
        /// </summary>
        /// <param name="messageId">ID of the Message</param>
        /// <param name="title">The Message's Title</param>
        /// <param name="name">Name to show the Message is From</param>
        /// <param name="startdate">StartDate of the Message</param>
        /// <param name="enddate">EndDate of the Message</param>
        /// <param name="popup">If message is a popup</param>
        /// <param name="message">Message</param>
        /// <returns></returns>
        /// <remarks></remarks>
        public bool UpdateMessage(int messageId, string title, string name, DateTime startdate, DateTime enddate, bool popup, string message)
        {
            SqlDataStore adapter = new SqlDataStore();
            return Convert.ToInt32(adapter.ExecuteNonQuery("core_messages_sp_UpdateMessages", messageId, title, name, startdate, enddate, popup, message)) > 0;
        }

        /// <summary>
        /// Remove all associates groups with a message and repopulate them using an XML string.
        /// </summary>
        /// <param name="messageId">ID of the Message</param>
        /// <param name="xmlGroups">XML String of the Groups now associated with the Message</param>
        /// <returns></returns>
        /// <remarks></remarks>
        public bool UpdateMessageGroups(int messageId, string xmlGroups)
        {
            SqlDataStore adapter = new SqlDataStore();
            return adapter.ExecuteNonQuery("core_messages_sp_UpdateMessageGroups", messageId, xmlGroups) > 0;
        }

        protected void RetrieveAllMessagesReader(SqlDataStore adapter, IDataReader reader)
        {
            //1-MessageId, 1-Title, 2-StartTime, 3-EndTime, 4-Popup
            Message message = new Message();
            message.MessageId = adapter.GetInt16(reader, 0);
            message.Title = adapter.GetString(reader, 1);
            message.StartTime = DateTime.Parse(adapter.GetDateTime(reader, 2, DateTime.Now).ToShortDateString());
            message.EndTime = DateTime.Parse(adapter.GetDateTime(reader, 3, DateTime.Now).ToShortDateString());
            message.Popup = adapter.GetBoolean(reader, 4);

            _message.Add(message);
        }

        protected void RetrieveMessageGroupsReader(SqlDataStore adapter, IDataReader reader)
        {
            //0-Title, 1-Message, 2-Name, 3-Popup, 4-StartTime,5-EndTime
            Message message = new Message();

            message.GroupId = adapter.GetByte(reader, 0);
            message.GroupName = adapter.GetString(reader, 1);
            message.Assigned = adapter.GetInteger(reader, 2) != 0; // Assuming GetInteger returns int, and Assigned is bool. Wait, Assigned is bool in Message.cs. But GetInteger returns int.
            // In VB: message.Assigned = adapter.GetInteger(reader, 2)
            // VB does automatic conversion from int to bool (0 is false, non-zero is true).
            // So I should check != 0.
            
            _message.Add(message);
        }

        protected void RetrieveMessagesReader(SqlDataStore adapter, IDataReader reader)
        {
            //0-Message, 1-Name, 2-StartTime, 3-Title
            Message message = new Message();
            message.MessageText = adapter.GetString(reader, 0); // Using MessageText property
            message.Name = adapter.GetString(reader, 1);
            message.StartTime = DateTime.Parse(adapter.GetDateTime(reader, 2, DateTime.Now).ToShortDateString());
            message.Title = adapter.GetString(reader, 3);
            message.IsAdmin = adapter.GetBoolean(reader, 4);

            _message.Add(message);
        }

        #endregion

        #region IList

        public int Count
        {
            get { return _message.Count; }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        public Message this[int index]
        {
            get { return _message[index]; }
            set { _message[index] = value; }
        }

        public void Add(Message item)
        {
            _message.Add(item);
        }

        public void Clear()
        {
            _message.Clear();
        }

        public bool Contains(Message item)
        {
            return _message.Contains(item);
        }

        public void CopyTo(Message[] array, int arrayIndex)
        {
            _message.CopyTo(array, arrayIndex);
        }

        public IEnumerator<Message> GetEnumerator()
        {
            return _message.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _message.GetEnumerator();
        }

        public int IndexOf(Message item)
        {
            return _message.IndexOf(item);
        }

        public void Insert(int index, Message item)
        {
            _message.Insert(index, item);
        }

        public bool Remove(Message item)
        {
            return _message.Remove(item);
        }

        public void RemoveAt(int index)
        {
            _message.RemoveAt(index);
        }

        #endregion
    }
}
