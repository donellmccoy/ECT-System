using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using ALOD.Data;
using ALODWebUtility.DataAccess;


namespace ALODWebUtility.PageTitles
{
    public class PageTitleList : IList<PageTitle>
    {
        List<PageTitle> _pages = new List<PageTitle>();

        public PageTitleList GetAllPageTitles()
        {
            SqlDataStore adapter = new SqlDataStore();
            adapter.ExecuteReader(CodeReader, "core_pageTitles_sp_GetAllPages");
            return this;
        }

        public DataSets.PageTitlesDataTable GetPagesAsDataSet()
        {
            return GetAllPageTitles().ToDataSet();
        }

        public DataSets.PageTitlesDataTable ToDataSet()
        {
            DataSets.PageTitlesDataTable data = new DataSets.PageTitlesDataTable();
            DataSets.PageTitlesRow row;

            foreach (PageTitle page in _pages)
            {
                row = data.NewPageTitlesRow();
                page.ToDataRow(ref row);
                data.Rows.Add(row);
            }

            return data;
        }

        public void UpdateByPageId(int pageID, string title)
        {
            SqlDataStore adapter = new SqlDataStore();
            adapter.ExecuteNonQuery("core_pageTitles_sp_UpdateByPageId", pageID, title);
        }

        protected void CodeReader(SqlDataStore adapter, IDataReader reader)
        {
            PageTitle pages = new PageTitle();

            pages.PageId = adapter.GetInt16(reader, 0);
            pages.Title = adapter.GetString(reader, 1);

            _pages.Add(pages);
        }

        #region Ilist

        public int Count
        {
            get
            {
                return _pages.Count;
            }
        }

        public bool IsReadOnly
        {
            get
            {
                return false;
            }
        }

        public PageTitle this[int index]
        {
            get
            {
                return _pages[index];
            }
            set
            {
                _pages[index] = value;
            }
        }

        public void Add(PageTitle item)
        {
            _pages.Add(item);
        }

        public void Clear()
        {
            _pages.Clear();
        }

        public bool Contains(PageTitle item)
        {
            return _pages.Contains(item);
        }

        public void CopyTo(PageTitle[] array, int arrayIndex)
        {
            _pages.CopyTo(array, arrayIndex);
        }

        public IEnumerator<PageTitle> GetEnumerator()
        {
            return _pages.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _pages.GetEnumerator();
        }

        public int IndexOf(PageTitle item)
        {
            return _pages.IndexOf(item);
        }

        public void Insert(int index, PageTitle item)
        {
            _pages.Insert(index, item);
        }

        public bool Remove(PageTitle item)
        {
            return _pages.Remove(item);
        }

        public void RemoveAt(int index)
        {
            _pages.RemoveAt(index);
        }

        #endregion
    }
}
