using System;
using ALOD.Data;

namespace ALODWebUtility.Workflow
{
    public class WorkStatus
    {
        protected string _descr;
        protected byte _groupId;
        protected string _groupName;
        protected int _id;
        protected byte _sortOrder;
        protected int _status;
        protected byte _workflow;

        public string Description
        {
            get { return _descr; }
            set { _descr = value; }
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

        public int Id
        {
            get { return _id; }
            set { _id = value; }
        }

        public byte SortOrder
        {
            get { return _sortOrder; }
            set { _sortOrder = value; }
        }

        public int Status
        {
            get { return _status; }
            set { _status = value; }
        }

        public byte Workflow
        {
            get { return _workflow; }
            set { _workflow = value; }
        }

        /// <summary>
        /// Saves the status code to the provided workflow
        /// </summary>
        /// <param name="workflow"></param>
        /// <returns></returns>
        public bool Save(byte workflow)
        {
            _workflow = workflow;
            return Save();
        }

        public bool Save()
        {
            SqlDataStore adapter = new SqlDataStore();
            return Convert.ToInt32(adapter.ExecuteNonQuery("core_workstatus_sp_InsertStatus", _workflow, _id)) > 0;
        }

        public bool SetOrder(byte order)
        {
            SqlDataStore adapter = new SqlDataStore();
            if (Convert.ToInt32(adapter.ExecuteNonQuery("core_workstatus_sp_UpdateOrder", _id, order)) > 0)
            {
                _sortOrder = order;
                return true;
            }

            return false;
        }
    }
}
