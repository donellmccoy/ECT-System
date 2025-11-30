using System;

namespace ALODWebUtility.Common
{
    [Serializable]
    public class ChangeRow
    {
        protected DateTime _actionDate;
        protected int _actionId;
        protected string _actionName;
        protected string _field;
        protected int _id;
        protected string _newVal;
        protected string _oldVal;
        protected string _section;
        protected int _userId;
        protected string _userName;

        public ChangeRow()
        {
            _id = 0;
            _section = string.Empty;
            _field = string.Empty;
            _oldVal = string.Empty;
            _newVal = string.Empty;
        }

        public ChangeRow(int id, string section, string field, string oldVal, string newVal)
        {
            _id = id;
            _section = section;
            _field = field;
            _oldVal = oldVal;
            _newVal = newVal;
        }

        public ChangeRow(string section, string field, string oldVal, string newVal)
        {
            _id = Id; // This looks like a bug in the original VB code, assigning property Id to field _id, but Id uses _id. 
                      // In VB, Id property access inside the class might refer to the property. 
                      // But _id is initialized to 0 in default constructor. 
                      // In the VB code: _id = Id. Id returns _id. So _id = _id.
                      // I will keep it as is, but it's redundant.
            _section = section;
            _field = field;
            _oldVal = oldVal;
            _newVal = newVal;
        }

        public DateTime ActionDate
        {
            get { return _actionDate; }
            set { _actionDate = value; }
        }

        public int ActionId
        {
            get { return _actionId; }
            set { _actionId = value; }
        }

        public string ActionName
        {
            get { return _actionName; }
            set { _actionName = value; }
        }

        public string Field
        {
            get { return _field; }
            set { _field = value; }
        }

        public int Id
        {
            get { return _id; }
            set { _id = value; }
        }

        public string NewVal
        {
            get { return _newVal; }
            set { _newVal = value; }
        }

        public string OldVal
        {
            get { return _oldVal; }
            set { _oldVal = value; }
        }

        public string Section
        {
            get { return _section; }
            set { _section = value; }
        }

        public int UserId
        {
            get { return _userId; }
            set { _userId = value; }
        }

        public string UserName
        {
            get { return _userName; }
            set { _userName = value; }
        }
    }
}
