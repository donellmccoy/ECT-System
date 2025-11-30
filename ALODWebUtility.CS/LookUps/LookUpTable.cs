using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Web.UI.WebControls;
using ALOD.Data;

namespace ALODWebUtility.LookUps
{
    public class LookUpTable : IList<LookUpRow>
    {
        protected SqlDataStore _adapter;
        protected List<LookUpRow> _rows;
        protected System.Web.Caching.Cache cacheSource;

        public LookUpTable()
        {
            _rows = new List<LookUpRow>();
        }

        public List<LookUpRow> rows
        {
            get
            {
                return _rows;
            }
            set
            {
                _rows = value;
            }
        }

        protected System.Web.Caching.Cache Cache
        {
            get
            {
                return System.Web.HttpContext.Current.Cache;
            }
        }

        protected SqlDataStore DataStore
        {
            get
            {
                if (_adapter == null)
                {
                    _adapter = new SqlDataStore();
                }
                return _adapter;
            }
        }

        public void AddRow(string value, string text)
        {
            LookUpRow row = new LookUpRow();
            row.value = value;
            row.text = text;
            _rows.Add(row);
        }

        public LookUpRow Find(string value)
        {
            foreach (LookUpRow row in _rows)
            {
                if (row.value == value)
                {
                    return row;
                }
            }

            return null;
        }

        public void LookUpRowReader(SqlDataStore adapter, IDataReader reader)
        {
            LookUpRow row = new LookUpRow();
            if (!reader.IsDBNull(0))
            {
                row.value = reader[0].ToString();
            }
            else
            {
                row.value = string.Empty;
            }

            if (!reader.IsDBNull(1))
            {
                row.text = reader[1].ToString();
            }
            else
            {
                row.text = string.Empty;
            }
            _rows.Add(row);
        }

        #region IList

        public int Count
        {
            get
            {
                return _rows.Count;
            }
        }

        public bool IsReadOnly
        {
            get
            {
                return false;
            }
        }

        public LookUpRow this[int index]
        {
            get
            {
                return _rows[index];
            }
            set
            {
                _rows[index] = value;
            }
        }

        public void Add(LookUpRow item)
        {
            _rows.Add(item);
        }

        public void Clear()
        {
            _rows.Clear();
        }

        public bool Contains(LookUpRow item)
        {
            return _rows.Contains(item);
        }

        public void CopyTo(LookUpRow[] array, int arrayIndex)
        {
            _rows.CopyTo(array, arrayIndex);
        }

        public IEnumerator<LookUpRow> GetEnumerator()
        {
            return _rows.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _rows.GetEnumerator();
        }

        public int IndexOf(LookUpRow item)
        {
            return _rows.IndexOf(item);
        }

        public void Insert(int index, LookUpRow item)
        {
            _rows.Insert(index, item);
        }

        public bool Remove(LookUpRow item)
        {
            return _rows.Remove(item);
        }

        public void RemoveAt(int index)
        {
            _rows.RemoveAt(index);
        }

        #endregion

        #region Helper

        public string GetList()
        {
            StringBuilder buffer = new StringBuilder();
            if (_rows.Count < 1) return "";
            foreach (LookUpRow row in _rows)
            {
                buffer.Append(row.value.Trim());
                buffer.Append(";");
            }
            if (buffer.Length > 0)
            {
                buffer = buffer.Remove(buffer.Length - 1, 1);
            }

            return buffer.ToString();
        }

        public void SetList(string strVals)
        {
            _rows = new List<LookUpRow>();
            if (strVals.Length != 0)
            {
                string[] strItems = strVals.Split(';');
                int i;
                for (i = 0; i <= strItems.Length - 1; i++)
                {
                    LookUpRow row = new LookUpRow();
                    row.value = strItems[i];
                    row.text = "";
                    _rows.Add(row);
                }
            }
        }

        public void SetList(ListBox lst)
        {
            _rows = new List<LookUpRow>();
            int i;
            for (i = 0; i <= lst.Items.Count - 1; i++)
            {
                LookUpRow row = new LookUpRow();
                row.value = lst.Items[i].Value;
                row.text = lst.Items[i].Text;
                _rows.Add(row);
            }
        }

        #endregion
    }
}
