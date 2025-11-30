using System;
using ALOD.Data;

namespace ALODWebUtility.Workflow
{
    public class WorkStatusOption
    {
        protected bool _active;
        protected int _compo;
        protected byte _groupId;
        protected string _groupName;
        protected int _id;
        protected byte _order;
        protected int _statusOut;
        protected string _statusOutText;
        protected byte _template;
        protected string _text;
        protected bool _valid;
        protected bool _visible;
        protected int _workstatusId;

        public bool Active
        {
            get { return _active; }
            set { _active = value; }
        }

        public int Compo
        {
            get { return _compo; }
            set { _compo = value; }
        }

        public string CompoName
        {
            get
            {
                switch (Compo)
                {
                    case 5:
                        return "ANG";
                    case 6:
                        return "AFRC";
                    case 0:
                        return "All";
                    default:
                        return "UNKNOWN";
                }
            }
        }

        public byte DBSignTemplate
        {
            get { return _template; }
            set { _template = value; }
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

        public bool OptionVisible
        {
            get { return _visible; }
            set { _visible = value; }
        }

        public byte SortOrder
        {
            get { return _order; }
            set { _order = value; }
        }

        public int StatusOut
        {
            get { return _statusOut; }
            set { _statusOut = value; }
        }

        public string StatusOutText
        {
            get { return _statusOutText; }
            set { _statusOutText = value; }
        }

        public string Text
        {
            get { return _text; }
            set { _text = value; }
        }

        public bool Valid
        {
            get { return _valid; }
            set { _valid = value; }
        }

        public int WorkStatusId
        {
            get { return _workstatusId; }
            set { _workstatusId = value; }
        }

        public bool Delete()
        {
            SqlDataStore adapter = new SqlDataStore();
            return Convert.ToInt32(adapter.ExecuteNonQuery("core_workstatus_sp_DeleteOption", _id)) > 0;
        }

        public bool Save()
        {
            SqlDataStore adapter = new SqlDataStore();
            _id = Convert.ToInt32(adapter.ExecuteNonQuery("core_workstatus_sp_InsertOption", _id, _workstatusId, _statusOut, _text, _active, _order, _template, _compo));
            return _id > 0;
        }
    }
}
