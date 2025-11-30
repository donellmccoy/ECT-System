using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using ALOD.Data;
using ALODWebUtility.Common;

namespace ALODWebUtility.Perms
{
    public class PermRequestList : IList<PermRequest>
    {
        protected List<PermRequest> _list = new List<PermRequest>();

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

        public PermRequest this[int index]
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

        public void Add(PermRequest item)
        {
            _list.Add(item);
        }

        public void Clear()
        {
            _list.Clear();
        }

        public bool Contains(PermRequest item)
        {
            return _list.Contains(item);
        }

        public void CopyTo(PermRequest[] array, int arrayIndex)
        {
            _list.CopyTo(array, arrayIndex);
        }

        public IList<PermRequest> GetByPermissionId(int permId)
        {
            _list.Clear();
            SqlDataStore adapter = new SqlDataStore();
            adapter.ExecuteReader(RequestReader, "core_permissions_sp_GetRequestByPermId", permId);
            return _list;
        }

        public IEnumerator<PermRequest> GetEnumerator()
        {
            return _list.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _list.GetEnumerator();
        }

        public int IndexOf(PermRequest item)
        {
            return _list.IndexOf(item);
        }

        public void Insert(int index, PermRequest item)
        {
            _list.Insert(index, item);
        }

        public bool Remove(PermRequest item)
        {
            return _list.Remove(item);
        }

        public void RemoveAt(int index)
        {
            _list.RemoveAt(index);
        }

        public void Save()
        {
            if (_list.Count == 0)
            {
                return;
            }

            XMLString xml = new XMLString("List");

            foreach (PermRequest req in _list)
            {
                xml.BeginElement("Request");
                xml.WriteAttribute("id", req.RequestId.ToString());
                xml.WriteAttribute("userId", req.UserId.ToString());
                xml.WriteAttribute("permId", req.PermissionId.ToString());
                xml.WriteAttribute("reqGranted", req.Granted ? "1" : "0");
                xml.EndElement();
            }

            SqlDataStore adapter = new SqlDataStore();
            adapter.ExecuteNonQuery("core_permissions_sp_updatePermissionRequests", xml.Value);
        }

        protected void RequestReader(SqlDataStore adapter, IDataReader reader)
        {
            PermRequest req = new PermRequest();
            req.UserId = adapter.GetInt32(reader, 0);
            req.AkoId = adapter.GetString(reader, 1);
            req.UserName = adapter.GetString(reader, 2) + " " + adapter.GetString(reader, 3) + ", " + adapter.GetString(reader, 4);
            req.DateRequested = adapter.GetDateTime(reader, 5);
            req.RequestId = adapter.GetInt32(reader, 6);
            req.PermissionId = adapter.GetInt16(reader, 7);
            _list.Add(req);
        }
    }
}
