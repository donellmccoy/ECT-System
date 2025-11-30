using System.Data;

namespace ALODWebUtility.Reports
{
    public class QueryCondition
    {
        private string _filter = "";
        private string _name = "";
        private DbType _type;
        private object _value;

        public QueryCondition(string name, object value, DbType valueType, string filter)
        {
            _name = name;
            _type = valueType;
            _value = value;
            _filter = filter;
        }

        public QueryCondition(string name, string filter)
        {
            _name = name;
            _filter = filter;
        }

        public string Field
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

        public string Filter
        {
            get
            {
                return _filter;
            }
            set
            {
                _filter = value;
            }
        }

        public string Name
        {
            get
            {
                return "@" + _name;
            }
            set
            {
                _name = value;
            }
        }

        public object Value
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

        public DbType ValueType
        {
            get
            {
                return _type;
            }
            set
            {
                _type = value;
            }
        }
    }
}
