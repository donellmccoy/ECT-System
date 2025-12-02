using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using ALOD.Core.Domain.Workflow;
using ALOD.Logging;
using ALODWebUtility.DataAccess;


namespace ALODWebUtility.Common
{
    [Serializable]
    public class lodControlList : IList<lodControl>
    {
        protected List<lodControl> _lodcontrols;

        public lodControlList()
        {
            _lodcontrols = new List<lodControl>();
            _lodcontrols.Clear();
        }

        public void Create(System.Web.UI.ControlCollection ctls)
        {
            // Set the Initial Hash Value
            string tmpVal;
            lodControlList self = this;
            tmpVal = ControlValues.GetKeyValues(ctls, ref self, "");
        }

        public bool LogChanges(lodControlList newList, ModuleType modtype, UserAction actionType, int refId, string comment, int status)
        {
            int i;
            string name = "";
            lodControl oldldc;
            ChangeSet changes = new ChangeSet();
            i = 0;

            if (newList.Count != _lodcontrols.Count)
            {
                // There is some error since the control count is not same
                return false;
            }

            foreach (lodControl newldc in newList)
            {
                oldldc = _lodcontrols[i];
                if (newldc.Name != oldldc.Name)
                {
                    // There is some error since the control names are  not same so do not record the change set
                    return false;
                }
                i = i + 1;
            }

            i = 0;
            foreach (lodControl newldc in newList)
            {
                oldldc = _lodcontrols[i];
                if (newldc.Value != oldldc.Value)
                {
                    _lodcontrols[i].IsModified = true;
                    changes.Add(oldldc.Section, oldldc.Field, oldldc.Value, newldc.Value);
                }
                i = i + 1;
            }
            if (changes.Count > 0)
            {
                int actionId = LogManager.LogAction((int)modtype, actionType, refId, comment, status);
                changes.Save(actionId);
            }
            return true;
        }

        public void Read(string strList)
        {
            int i;
            string[] crtls;
            string[] crtlDef;
            lodControl ldc;
            crtls = strList.Split((char)9); // Tab is used as a seperator character since control values can contain this character
            int ncrtls;

            _lodcontrols.Clear();
            ncrtls = crtls.Length;
            if (ncrtls > 0)
            {
                for (i = 0; i <= ncrtls - 1; i++)
                {
                    crtlDef = crtls[i].Split((char)14); // Character -Control+T

                    ldc = new lodControl(crtlDef[0], crtlDef[1], crtlDef[2], crtlDef[3]); // name ,val,sec,field
                    _lodcontrols.Add(ldc);
                }
            }
        }

        public DataSets.ControlLodDataTable ToDataSet()
        {
            DataSets.ControlLodDataTable data = new DataSets.ControlLodDataTable();
            DataSets.ControlLodRow row;

            foreach (lodControl ldc in _lodcontrols)
            {
                row = data.NewControlLodRow();
                ldc.ToDataRow(row);
                data.Rows.Add(row);
            }

            return data;
        }

        public string Write()
        {
            StringBuilder buffer = new StringBuilder();

            foreach (lodControl ldc in _lodcontrols)
            {
                // name ,val,sec,field
                //   buffer.Append(ldc.Name + "|$|" + ldc.Value + "|$|" + ldc.Section + "|$|" + ldc.Field)
                buffer.Append(ldc.Name + (char)14 + ldc.Value + (char)14 + ldc.Section + (char)14 + ldc.Field);

                buffer.Append((char)9); // Tab is used as a seperator character since control values can contain this character
                // buffer.Append(";")
            }

            if (buffer.Length > 0)
            {
                buffer = buffer.Remove(buffer.Length - 1, 1);
            }

            return buffer.ToString();
        }

        #region IList

        public int Count
        {
            get
            {
                return _lodcontrols.Count;
            }
        }

        public bool IsReadOnly
        {
            get
            {
                return false;
            }
        }

        public lodControl this[int index]
        {
            get
            {
                return _lodcontrols[index];
            }
            set
            {
                _lodcontrols[index] = value;
            }
        }

        public void Add(lodControl item)
        {
            _lodcontrols.Add(item);
        }

        public void Clear()
        {
            _lodcontrols.Clear();
        }

        public bool Contains(lodControl item)
        {
            return _lodcontrols.Contains(item);
        }

        public void CopyTo(lodControl[] array, int arrayIndex)
        {
            _lodcontrols.CopyTo(array, arrayIndex);
        }

        public IEnumerator<lodControl> GetEnumerator()
        {
            return _lodcontrols.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _lodcontrols.GetEnumerator();
        }

        public int IndexOf(lodControl item)
        {
            return _lodcontrols.IndexOf(item);
        }

        public void Insert(int index, lodControl item)
        {
            _lodcontrols.Insert(index, item);
        }

        public bool Remove(lodControl item)
        {
            return _lodcontrols.Remove(item);
        }

        public void RemoveAt(int index)
        {
            _lodcontrols.RemoveAt(index);
        }

        #endregion
    }
}
