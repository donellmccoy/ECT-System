using System;
using System.Collections.Generic;
using System.Web;
using ALOD.Data;
using ALOD.Core.Domain.Workflow;

namespace ALODWebUtility.Workflow
{
    public class ActiveCaseList : ICollection<ActiveCase>
    {
        protected List<ActiveCase> _list = new List<ActiveCase>();

        public int Count
        {
            get { return _list.Count; }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        public void Add(ActiveCase item)
        {
            _list.Add(item);
        }

        public void Clear()
        {
            _list.Clear();
        }

        public bool Contains(ActiveCase item)
        {
            return _list.Contains(item);
        }

        public void CopyTo(ActiveCase[] array, int arrayIndex)
        {
            _list.CopyTo(array, arrayIndex);
        }

        public ActiveCaseList GetByRefId(int refId)
        {
            SqlDataStore adapter = new SqlDataStore();
            adapter.ExecuteReader(ActiveReader, "core_workflow_sp_GetActiveCases", refId, Convert.ToInt32(HttpContext.Current.Session["GroupId"]));
            return this;
        }

        public IEnumerator<ActiveCase> GetEnumerator()
        {
            return _list.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return _list.GetEnumerator();
        }

        public bool Remove(ActiveCase item)
        {
            return _list.Remove(item);
        }

        protected void ActiveReader(SqlDataStore adapter, System.Data.IDataReader reader)
        {
            // 0-moduleId, 1-refId, 2-description, 3-title, 4-moduleName, 5-parentId

            ActiveCase row = new ActiveCase();
            row.Type = (ModuleType)adapter.GetByte(reader, 0);
            row.RefId = adapter.GetInt32(reader, 1);
            row.Description = adapter.GetString(reader, 2);
            row.Title = adapter.GetString(reader, 3);
            row.ModuleTitle = adapter.GetString(reader, 4);
            row.ParentId = adapter.GetInt32(reader, 5);

            _list.Add(row);
        }
    }
}
