using ALOD.Data;
using ALODWebUtility.DataAccess;


namespace ALODWebUtility.PageTitles
{
    public class PageTitle
    {
        protected int _pageId = 0;
        protected string _title = string.Empty;

        public PageTitle()
        {
        }

        public int PageId
        {
            get
            {
                return _pageId;
            }
            set
            {
                _pageId = value;
            }
        }

        public string Title
        {
            get
            {
                return _title;
            }
            set
            {
                _title = value;
            }
        }

        public void Insert()
        {
            SqlDataStore adapter = new SqlDataStore();
            adapter.ExecuteNonQuery("core_pageTitles_sp_Insert", _title);
        }

        public void ToDataRow(ref DataSets.PageTitlesRow row)
        {
            row.pageId = _pageId;
            row.title = _title;
        }
    }
}
