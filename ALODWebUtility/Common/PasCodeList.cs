using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using ALOD.Core.Domain.Users;
using ALOD.Data;

namespace ALODWebUtility.Common
{
    [Serializable]
    public class PasCodeList : IList<Pascode>
    {
        protected List<Pascode> _pasCodes;

        #region Properties

        public Pascode this[string strpasCode]
        {
            get
            {
                foreach (Pascode iter in _pasCodes)
                {
                    if (iter.Value.ToUpper() == strpasCode.ToUpper())
                    {
                        return iter;
                    }
                }
                return null;
            }
            set
            {
                for (int i = 0; i < _pasCodes.Count; i++)
                {
                    if (_pasCodes[i].Value.ToUpper() == strpasCode.ToUpper())
                    {
                        _pasCodes[i] = value;
                    }
                }
            }
        }

        #endregion

        #region Constructors

        public PasCodeList()
        {
            _pasCodes = new List<Pascode>();
        }

        public PasCodeList(PasCodeList pCodeList)
        {
            _pasCodes = new List<Pascode>();

            foreach (Pascode pCode in pCodeList)
            {
                _pasCodes.Add(new Pascode(pCode));
            }
        }

        #endregion

        #region HelperFunctions

        public string GetPasCodeString(string seperator)
        {
            StringBuilder buffer = new StringBuilder();

            foreach (Pascode item in _pasCodes)
            {
                buffer.Append(item.Value);
                buffer.Append(seperator);
            }

            if (buffer.Length > 0)
            {
                buffer.Remove(buffer.Length - 1, 1);
            }

            return buffer.ToString();
        }

        public void ResetStatus()
        {
            foreach (Pascode pCode in _pasCodes)
            {
                pCode.Status = AccessStatus.Pending;
            }
        }

        #endregion

        #region IList

        public int Count
        {
            get { return _pasCodes.Count; }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        public Pascode this[int index]
        {
            get { return _pasCodes[index]; }
            set { _pasCodes[index] = value; }
        }

        public void Add(string strpasCode, AccessStatus status)
        {
            _pasCodes.Add(new Pascode(strpasCode, status));
        }

        public void Add(Pascode item)
        {
            _pasCodes.Add(item);
        }

        public void Clear()
        {
            _pasCodes.Clear();
        }

        public bool Contains(Pascode item)
        {
            return _pasCodes.Contains(item);
        }

        public void CopyTo(Pascode[] array, int arrayIndex)
        {
            _pasCodes.CopyTo(array, arrayIndex);
        }

        public IEnumerator<Pascode> GetEnumerator()
        {
            return _pasCodes.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _pasCodes.GetEnumerator();
        }

        public int IndexOf(Pascode item)
        {
            return _pasCodes.IndexOf(item);
        }

        public void Insert(int index, Pascode item)
        {
            _pasCodes.Insert(index, item);
        }

        public bool Remove(Pascode item)
        {
            return _pasCodes.Remove(item);
        }

        public void RemoveAt(int index)
        {
            _pasCodes.RemoveAt(index);
        }

        #endregion

        #region Helper

        public bool Contains(string strpasCode)
        {
            foreach (Pascode item in _pasCodes)
            {
                if (strpasCode.ToUpper() == item.Value.ToUpper())
                {
                    return true;
                }
            }
            return false;
        }

        #endregion

        #region Save

        public void Save(int roleId, SqlDataStore adapter)
        {
            XMLString xml = new XMLString("PasCodeList");

            foreach (Pascode item in _pasCodes)
            {
                xml.BeginElement("PasCode");
                xml.WriteAttribute("id", item.Value.ToUpper());
                xml.WriteAttribute("status", item.Status.ToString());
                xml.EndElement();
            }

            adapter.ExecuteNonQuery("core_role_sp_UpdatePasCodes", roleId, xml.Value);
        }

        #endregion
    }
}
