using System;
using System.Data;
using System.Data.Common;
using ALOD.Data;

namespace ALODWebUtility.Common
{
    [Serializable]
    public class Message
    {
        #region Members

        protected byte _rows;
        private bool _assigned = false;
        private DateTime _endTime = new DateTime();
        private byte _groupId = 0;
        private string _groupName = string.Empty;
        private bool _isAdmin = false;
        private string _message = string.Empty;
        private short _messageId = 0;
        private string _name = string.Empty;
        private bool _popup = false;
        private DateTime _startTime = new DateTime();
        private string _title = string.Empty;

        #endregion

        #region Properties

        public bool Assigned
        {
            get { return _assigned; }
            set { _assigned = value; }
        }

        public DateTime EndTime
        {
            get { return _endTime; }
            set { _endTime = value; }
        }

        public byte GroupId
        {
            get { return _groupId; }
            set { _groupId = value; }
        }

        public string GroupName
        {
            get { return _groupName; }
            set { _groupName = value; }
        }

        public bool IsAdmin
        {
            get { return _isAdmin; }
            set { _isAdmin = value; }
        }

        public string MessageText // Renamed from Message to MessageText to avoid conflict with class name
        {
            get { return _message; }
            set { _message = value; }
        }

        public short MessageId
        {
            get { return _messageId; }
            set { _messageId = value; }
        }

        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        public bool Popup
        {
            get { return _popup; }
            set { _popup = value; }
        }

        public DateTime StartTime
        {
            get { return _startTime; }
            set { _startTime = value; }
        }

        public string Title
        {
            get { return _title; }
            set { _title = value; }
        }

        #endregion

        public void DeleteMessage(short messageId)
        {
            SqlDataStore adapter = new SqlDataStore();
            DbCommand cmd;
            cmd = adapter.GetStoredProcCommand("core_messages_sp_DeleteMessage");
            adapter.AddInParameter(cmd, "@messageId", DbType.Int16, messageId);
            adapter.ExecuteNonQuery(cmd);
        }

        /// <summary>
        /// Returns a Message details row
        /// </summary>
        /// <param name="messageId">The ID of the Message</param>
        /// <returns>A data row containing the results.</returns>
        /// <remarks></remarks>
        public Message RetrieveMessageDetails(int messageId)
        {
            SqlDataStore adapter = new SqlDataStore();
            DbCommand cmd = adapter.GetStoredProcCommand("core_messages_sp_GetMessagesDetails", messageId);
            adapter.ExecuteReader(RetrieveMessageDetailsReader, cmd);
            return this;
        }

        protected void RetrieveMessageDetailsReader(SqlDataStore adapter, IDataReader reader)
        {
            _rows += 1;

            //0-Title, 1-Message, 2-Name, 3-Popup, 4-StartTime,5-EndTime
            //Dim message As New Message
            _title = adapter.GetString(reader, 0);
            _message = adapter.GetString(reader, 1);
            _name = adapter.GetString(reader, 2);
            _popup = adapter.GetBoolean(reader, 3);
            _startTime = DateTime.Parse(adapter.GetDateTime(reader, 4, DateTime.Now).ToShortDateString());
            _endTime = DateTime.Parse(adapter.GetDateTime(reader, 5, DateTime.Now).ToShortDateString());
        }
    }
}
