using System;

namespace ALODWebUtility.Common
{
    [Serializable]
    public class lodControl
    {
        private string _field;
        private bool _IsModified;
        private string _name;
        private string _section;
        private string _value;

        public lodControl(string name, string val, string sec, string fld)
        {
            _name = name;
            _value = val;
            _section = sec;
            _field = fld;
        }

        public string Field
        {
            get
            {
                return _field;
            }
            set
            {
                _field = value;
            }
        }

        public bool IsModified
        {
            get
            {
                return _IsModified;
            }
            set
            {
                _IsModified = value;
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

        public string Section
        {
            get
            {
                return _section;
            }
            set
            {
                _section = value;
            }
        }

        public string Value
        {
            get
            {
                return _value;
            }
            set
            {
                _value = value;
            }
        }

        public void ToDataRow(DataSets.ControlLodRow row)
        {
            row.name = _name;
            row.section = _section;
            row.field = _field;
            row.value = _value;
        }
    }
}
