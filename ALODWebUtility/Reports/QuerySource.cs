using System.Collections;
using System.Collections.Specialized;
using System.Text;
using ALOD.Core.Utils;

namespace ALODWebUtility.Reports
{
    public class QuerySource
    {
        private string _alias = "";
        private StringDictionary _fields = new StringDictionary();
        private string _join = "";
        private JoinType _joinType;
        private StringCollection _orderFields = new StringCollection();
        private string _table = "";

        public string FieldList
        {
            get
            {
                StringBuilder buffer = new StringBuilder();

                foreach (DictionaryEntry pair in _fields)
                {
                    if (pair.Value.ToString().Length > 0)
                    {
                        buffer.Append(_alias + "." + pair.Key.ToString() + " AS '" + pair.Value.ToString() + "', ");
                    }
                    else
                    {
                        buffer.Append(_alias + "." + pair.Key.ToString() + ", ");
                    }
                }

                return buffer.ToString();
            }
        }

        public StringDictionary Fields
        {
            get
            {
                return _fields;
            }
            set
            {
                _fields = value;
            }
        }

        public bool IsJoin
        {
            get
            {
                return _join.Length > 0;
            }
        }

        public string JoinCondition
        {
            get
            {
                return _join;
            }
            set
            {
                _join = value;
            }
        }

        public JoinType JoinType
        {
            get
            {
                return _joinType;
            }
            set
            {
                _joinType = value;
            }
        }

        public string Name
        {
            get
            {
                return _table;
            }
            set
            {
                _table = value;
            }
        }

        public string OrderFieldList
        {
            get
            {
                StringBuilder buffer = new StringBuilder();

                foreach (string pair in _orderFields)
                {
                    buffer.Append(_alias + "." + pair + ", ");
                }

                return buffer.ToString();
            }
        }

        public StringCollection OrderFields
        {
            get
            {
                return _orderFields;
            }
            set
            {
                _orderFields = value;
            }
        }

        public string TableAlias
        {
            get
            {
                return _alias;
            }
            set
            {
                _alias = value;
            }
        }

        public void AddField(string name)
        {
            _fields.Add(name, "");
        }

        public void AddField(string name, string display)
        {
            _fields.Add(name, display);
        }

        public void AddFields(params string[] name)
        {
            foreach (string item in name)
            {
                AddField(item);
            }
        }

        public void AddOrderField(string name)
        {
            _orderFields.Add(name);
        }

        public void AddOrderFields(params string[] name)
        {
            foreach (string item in name)
            {
                AddOrderField(item);
            }
        }
    }
}
