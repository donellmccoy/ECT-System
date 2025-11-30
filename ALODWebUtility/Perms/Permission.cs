using System;

namespace ALODWebUtility.Perms
{
    [Serializable]
    public class Permission
    {
        protected bool _allowed;
        protected string _description;
        protected bool _exclude = false;
        protected int _id;
        protected string _name;
        protected string _status;

        public Permission()
        {
            _id = 0;
            _name = string.Empty;
            _description = string.Empty;
        }

        public Permission(int id, string name, string description)
        {
            _id = id;
            _name = name;
            _description = description;
            _allowed = true;
        }

        public Permission(int id, string name, string description, bool allowed)
        {
            _id = id;
            _name = name;
            _description = description;
            _allowed = allowed;
        }

        public bool Allowed
        {
            get
            {
                return _allowed;
            }
            set
            {
                _allowed = value;
            }
        }

        public string Description
        {
            get
            {
                return _description;
            }
            set
            {
                _description = value;
            }
        }

        public bool Exclude
        {
            get
            {
                return _exclude;
            }
            set
            {
                _exclude = value;
            }
        }

        public int Id
        {
            get
            {
                return _id;
            }
            set
            {
                _id = value;
            }
        }

        public string Name
        {
            get
            {
                return _name;
            }
            set
            {
                _name = value;
            }
        }

        public string Status
        {
            get
            {
                return _status;
            }
            set
            {
                _status = value;
            }
        }

        public void ToDataRow(DataSets.PermissionRow row)
        {
            row.id = _id;
            row.name = _name;
            row.description = _description;
            row.allowed = _allowed;
            row.exclude = _exclude;
        }
    }
}
