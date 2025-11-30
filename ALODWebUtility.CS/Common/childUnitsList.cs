using System;
using System.Collections;
using System.Collections.Generic;
using System.Web;
using ALOD.Data;

namespace ALODWebUtility.Common
{
    [Serializable]
    public class childUnitsList : IList<childunit>
    {
        protected SqlDataStore _adapter;
        protected List<childunit> _childUnits;

        public childUnitsList()
        {
            _childUnits = new List<childunit>();
            _childUnits.Clear();
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

        public childUnitsList Read(int unit, string chainType, int userUnit)
        {
            int userId = Convert.ToInt32(HttpContext.Current.Session["UserId"]);
            DataStore.ExecuteReader(unitReader, "core_pascodes_GetChildUnits", unit, chainType, userUnit, userId);
            return this;
        }

        private void unitReader(SqlDataStore adapter, System.Data.IDataReader reader)
        {
            childunit _newunit = new childunit();

            _newunit.cs_id = adapter.GetInt32(reader, 0, -1);
            _newunit.childName = adapter.GetString(reader, 1);
            _newunit.childPasCode = adapter.GetString(reader, 2);

            _newunit.parentCS_ID = adapter.GetInt32(reader, 3, -1);
            _newunit.CHAIN_TYPE = adapter.GetString(reader, 4);
            _newunit.Level = adapter.GetInt32(reader, 5, -1);
            _newunit.userUnit = adapter.GetInt32(reader, 6, -1);
            _newunit.InActive = adapter.GetBoolean(reader, 7, false);
            _childUnits.Add(_newunit);
        }

        #region IList

        public int Count
        {
            get
            {
                return _childUnits.Count;
            }
        }

        public bool IsReadOnly
        {
            get
            {
                return false;
            }
        }

        public childunit this[int index]
        {
            get
            {
                return _childUnits[index];
            }
            set
            {
                _childUnits[index] = value;
            }
        }

        public void Add(childunit item)
        {
            _childUnits.Add(item);
        }

        public void Clear()
        {
            _childUnits.Clear();
        }

        public bool Contains(childunit item)
        {
            return _childUnits.Contains(item);
        }

        public void CopyTo(childunit[] array, int arrayIndex)
        {
            _childUnits.CopyTo(array, arrayIndex);
        }

        public IEnumerator<childunit> GetEnumerator()
        {
            return _childUnits.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _childUnits.GetEnumerator();
        }

        public int IndexOf(childunit item)
        {
            return _childUnits.IndexOf(item);
        }

        public void Insert(int index, childunit item)
        {
            _childUnits.Insert(index, item);
        }

        public bool Remove(childunit item)
        {
            return _childUnits.Remove(item);
        }

        public void RemoveAt(int index)
        {
            _childUnits.RemoveAt(index);
        }

        #endregion
    }
}
