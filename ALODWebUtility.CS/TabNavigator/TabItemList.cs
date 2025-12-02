using System;
using System.Collections;
using System.Collections.Generic;

namespace ALODWebUtility.TabNavigation
{
    [Serializable]
    public class TabItemList : IList<TabItem>
    {
        protected List<TabItem> _list = new List<TabItem>();

        public int Count
        {
            get
            {
                return _list.Count;
            }
        }

        public bool IsReadOnly
        {
            get
            {
                return false;
            }
        }

        public TabItem this[int index]
        {
            get
            {
                return _list[index];
            }
            set
            {
                _list[index] = value;
            }
        }

        public TabItem this[string page]
        {
            get
            {
                foreach (TabItem row in _list)
                {
                    if (string.Equals(row.Page, page, StringComparison.InvariantCultureIgnoreCase))
                    {
                        return row;
                    }
                }
                return null;
            }
            set
            {
                for (int i = 0; i < _list.Count; i++)
                {
                    if (_list[i].Page.CompareTo(page) == 0)
                    {
                        _list[i] = value;
                    }
                }
            }
        }

        public void Add(TabItem item)
        {
            _list.Add(item);
        }

        public void Clear()
        {
            _list.Clear();
        }

        public bool Contains(TabItem item)
        {
            return _list.Contains(item);
        }

        public void CopyTo(TabItem[] array, int arrayIndex)
        {
            _list.CopyTo(array, arrayIndex);
        }

        public IEnumerator<TabItem> GetEnumerator()
        {
            return _list.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _list.GetEnumerator();
        }

        public int IndexOf(TabItem item)
        {
            return _list.IndexOf(item);
        }

        public void Insert(int index, TabItem item)
        {
            _list.Insert(index, item);
        }

        public bool Remove(TabItem item)
        {
            return _list.Remove(item);
        }

        public void RemoveAt(int index)
        {
            _list.RemoveAt(index);
        }
    }
}
