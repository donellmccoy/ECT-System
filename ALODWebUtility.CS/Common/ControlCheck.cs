using System;
using System.Collections;
using System.Data;
using System.Web;

namespace ALODWebUtility.Common
{
    #region Contol Class

    public class ModControl
    {
        #region Private Variables

        private bool mboolIsMultiList;
        private string mstrProperty1;
        private string mstrProperty2;
        private string mstrPropertyToCheck;
        private string mstrPropertyToCheckValue;
        private string mstrType;

        #endregion

        #region Public Properties

        public bool IsMultiList
        {
            get { return mboolIsMultiList; }
            set { mboolIsMultiList = value; }
        }

        public string Property1
        {
            get { return mstrProperty1; }
            set { mstrProperty1 = value; }
        }

        public string Property2
        {
            get { return mstrProperty2; }
            set { mstrProperty2 = value; }
        }

        public string PropertyToCheck
        {
            get { return mstrPropertyToCheck; }
            set { mstrPropertyToCheck = value; }
        }

        public string PropertyToCheckValue
        {
            get { return mstrPropertyToCheckValue; }
            set { mstrPropertyToCheckValue = value; }
        }

        public string Type
        {
            get { return mstrType; }
            set { mstrType = value; }
        }

        // The rest of the property definitions go here

        #endregion
    }

    #endregion

    #region Control Collections Class

    public class ModControls : ArrayList
    {
        private string mstrType = string.Empty;

        #region Load Method

        public void LoadControls(string FileName)
        {
            DataSet ds = new DataSet();
            ModControl dc;

            ds.ReadXml(FileName);
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                dc = new ModControl();
                dc.Type = dr["Type"].ToString();
                dc.Property1 = dr["Property1"].ToString();
                dc.Property2 = dr["Property2"].ToString();
                dc.IsMultiList = ToBoolean(dr["IsMultiList"].ToString());
                dc.PropertyToCheck = dr["PropertyToCheck"].ToString();
                dc.PropertyToCheckValue = dr["PropertyToCheckValue"].ToString();
                this.Add(dc);
            }
        }

        private bool ToBoolean(string value)
        {
            bool boolRet = false;

            value = value.ToString().ToLower().Trim();

            if (value == "yes")
            {
                boolRet = true;
            }

            return boolRet;
        }

        #endregion

        #region Get Methods

        public virtual ModControl GetByType(string Type)
        {
            mstrType = Type;

            foreach (ModControl dc in this)
            {
                if (dc.Type == mstrType) return dc;
            }
            return null;
        }

        private bool FindByType(ModControl dc)
        {
            if (dc.Type == mstrType)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        #endregion
    }

    #endregion

    #region Class for Loading Controls

    public class ControlLoad
    {
        #region Private Shared Variables

        private static ModControls mControlsList = null;

        #endregion

        #region Public Shared Properties

        public static ModControls mControlList
        {
            get
            {
                if (mControlsList == null)
                {
                    LoadMControls();
                }
                return mControlsList;
            }
            set
            {
                mControlsList = value;
            }
        }

        #endregion

        #region LoadMControls Method

        private static void LoadMControls()
        {
            // Load up the DirtyControls List
            mControlsList = new ModControls();
            string strDir;
            strDir = GetCurrentDirectory();
            mControlsList.LoadControls(strDir + "Script\\ControlsXML.xml");
        }

        #endregion

        #region GetCurrentDirectory Method

        public static string GetCurrentDirectory()
        {
            string strPath;

            strPath = HttpContext.Current.Request.PhysicalApplicationPath;
            return strPath;
        }

        #endregion
    }

    #endregion
}
