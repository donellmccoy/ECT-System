using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using ALOD.Core.Domain.Users;
using ALOD.Data;

namespace ALODWebUtility.Common
{
    [Serializable]
    public class Pascode
    {
        #region OldPasCodeDefinition

        protected bool _canEdit = false;
        protected string _description = string.Empty;
        protected string _designator = string.Empty;
        protected string _installation = string.Empty;
        protected string _majcom = string.Empty;
        protected string _organization = string.Empty;
        protected string _orgnKind = string.Empty;

        //Service Plan Activity
        protected string _parent = string.Empty;

        protected string _pasCode;
        protected string _spa = string.Empty;
        protected AccessStatus _status = AccessStatus.Pending;
        protected string _unit = string.Empty;

        #region PropertiesOld

        public bool CanEdit
        {
            get { return _canEdit; }
            set { _canEdit = value; }
        }

        public string Description
        {
            get { return _description; }
            set { _description = value; }
        }

        public string Designator
        {
            get { return _designator; }
            set { _designator = value; }
        }

        public string Installation
        {
            get { return _installation; }
            set { _installation = value; }
        }

        public string MAJCOM
        {
            get { return _majcom; }
            set { _majcom = value; }
        }

        public string Organization
        {
            get { return _organization; }
            set { _organization = value; }
        }

        public string OrgnKind
        {
            get { return _orgnKind; }
            set { _orgnKind = value; }
        }

        public string Parent
        {
            get { return _parent; }
            set { _parent = value; }
        }

        public string SPA
        {
            get { return _spa; }
            set { _spa = value; }
        }

        public AccessStatus Status
        {
            get { return _status; }
            set { _status = value; }
        }

        public string Unit
        {
            get { return _unit; }
            set { _unit = value; }
        }

        public string Value
        {
            get { return _pasCode; }
            set
            {
                if (value.Trim().Length == 8)
                {
                    _pasCode = value.Substring(4, 4);
                }
                else
                {
                    _pasCode = value;
                }
            }
        }

        #endregion

        #region Constructors

        public Pascode()
        {
        }

        public Pascode(Pascode pCode)
        {
            _pasCode = pCode.Value;
            _description = pCode.Description;
            _status = pCode.Status;
            _canEdit = pCode.CanEdit;

            _spa = pCode.SPA;
            _majcom = pCode.MAJCOM;
            _designator = pCode.Designator;
            _unit = pCode.Unit;
            _organization = pCode.Organization;  //Service Plan Activity
            _installation = pCode.Installation;
            _orgnKind = pCode.OrgnKind;
            _parent = pCode.Parent;
            _orgnKind = pCode.OrgnKind;
            _uic = pCode.uic;
        }

        public Pascode(string pasCode)
        {
            _pasCode = pasCode;
        }

        public Pascode(string strpasCode, AccessStatus status, bool canEdit)
        {
            _status = status;
            _pasCode = strpasCode;
            _canEdit = canEdit;
        }

        public Pascode(string strpasCode, AccessStatus status)
        {
            _status = status;
            _pasCode = strpasCode;
        }

        public bool IsValidPascode(string pascode)
        {
            SqlDataStore adapter = new SqlDataStore();
            DbCommand cmd = adapter.GetStoredProcCommand("core_pascode_sp_Validation");
            adapter.AddInParameter(cmd, "@pascode", DbType.String, pascode);
            adapter.AddOutParameter(cmd, "@valid", DbType.Boolean, 0);
            adapter.ExecuteNonQuery(cmd);
            return Convert.ToBoolean(cmd.Parameters["@valid"].Value);
        }

        #endregion

        #endregion

        #region NewPasCodeDefinition

        #region Members

        protected string _address1 = string.Empty;

        protected string _address2 = string.Empty;

        protected string _base_code = string.Empty;

        protected string _city = string.Empty;

        protected string _component = string.Empty;

        protected string _country = string.Empty;

        //--These are the properties
        protected int _cs_id;

        //These need to be initialized to these values since 0 correspoonds to an existing unit
        protected int _cs_id_parent = -1;

        protected string _cs_level = string.Empty;
        protected string _cs_oper_type = string.Empty;
        protected string _email = string.Empty;
        protected int _gaining_command_cs_id = -1;
        protected string _long_name = string.Empty;
        protected string _pas_code = string.Empty;
        protected string _postal_code = string.Empty;
        protected Dictionary<string, int> _reportingStructure;
        protected string _state = string.Empty;
        protected string _time_zone = string.Empty;
        protected string _uic = string.Empty;
        protected string _unit_det = string.Empty;
        protected string _unit_kind = string.Empty;
        protected string _unit_nbr = string.Empty;
        protected string _unit_type = string.Empty;

        #endregion

        #region Properties

        public string address1
        {
            get { return _address1; }
            set { _address1 = value; }
        }

        public string address2
        {
            get { return _address2; }
            set { _address2 = value; }
        }

        public string base_code
        {
            get { return _base_code; }
            set { _base_code = value; }
        }

        public string city
        {
            get { return _city; }
            set { _city = value; }
        }

        public string component
        {
            get { return _component; }
            set { _component = value; }
        }

        public string country
        {
            get { return _country; }
            set { _country = value; }
        }

        public int cs_id
        {
            get { return _cs_id; }
            set { _cs_id = value; }
        }

        public int cs_id_parent
        {
            get { return _cs_id_parent; }
            set { _cs_id_parent = value; }
        }

        public string cs_level
        {
            get { return _cs_level; }
            set { _cs_level = value; }
        }

        public string cs_oper_type
        {
            get { return _cs_oper_type; }
            set { _cs_oper_type = value; }
        }

        public string Email
        {
            get { return _email; }
            set { _email = value; }
        }

        public int gaining_command_cs_id
        {
            get { return _gaining_command_cs_id; }
            set { _gaining_command_cs_id = value; }
        }

        public string long_name
        {
            get { return _long_name; }
            set { _long_name = value; }
        }

        public string pas_code
        {
            get { return _pas_code; }
            set { _pas_code = value; }
        }

        public string postal_code
        {
            get { return _postal_code; }
            set { _postal_code = value; }
        }

        public Dictionary<string, int> reportingStructure
        {
            get
            {
                if (_reportingStructure == null)
                {
                    _reportingStructure = new Dictionary<string, int>();
                }
                return _reportingStructure;
            }
            set { _reportingStructure = value; }
        }

        public string state
        {
            get { return _state; }
            set { _state = value; }
        }

        public string time_zone
        {
            get { return _time_zone; }
            set { _time_zone = value; }
        }

        public string uic
        {
            get { return _uic; }
            set { _uic = value; }
        }

        public string unit_det
        {
            get { return _unit_det; }
            set { _unit_det = value; }
        }

        public string unit_kind
        {
            get { return _unit_kind; }
            set { _unit_kind = value; }
        }

        public string unit_nbr
        {
            get { return _unit_nbr; }
            set { _unit_nbr = value; }
        }

        public string unit_type
        {
            get { return _unit_type; }
            set { _unit_type = value; }
        }

        #endregion

        #region Constructor

        public Pascode(int csid)
        {
            _cs_id = csid;
            _reportingStructure = new Dictionary<string, int>();
        }

        #endregion

        #region Load

        public void LoadPasCode()
        {
            // Do not use Iif (evaluates both results [Then and Else] before the conditional [If]) - which causes errors
            UnitDao unitDao = new NHibernateDaoFactory().GetUnitDao();
            Unit pCodeInfo = unitDao.GetById(cs_id);
            if (pCodeInfo.Name == null)
            {
                long_name = "";
            }
            else
            {
                long_name = pCodeInfo.Name;
            }
            if (pCodeInfo.UnitNumber == null)
            {
                unit_nbr = "";
            }
            else
            {
                unit_nbr = pCodeInfo.UnitNumber;
            }
            if (pCodeInfo.UnitKind == null)
            {
                unit_kind = "";
            }
            else
            {
                unit_kind = pCodeInfo.UnitKind;
            }
            if (pCodeInfo.UnitType == null)
            {
                unit_type = "";
            }
            else
            {
                unit_type = pCodeInfo.UnitType;
            }
            if (pCodeInfo.UnitDet == null)
            {
                unit_det = "";
            }
            else
            {
                unit_det = pCodeInfo.UnitDet;
            }
            if (pCodeInfo.Uic == null)
            {
                uic = "";
            }
            else
            {
                uic = pCodeInfo.Uic;
            }
            if (pCodeInfo.CommandStructLevel == null)
            {
                cs_level = "";
            }
            else
            {
                cs_level = pCodeInfo.CommandStructLevel;
            }
            if (pCodeInfo.ParentUnit == null)
            {
                cs_id_parent = -1;
            }
            else
            {
                cs_id_parent = pCodeInfo.ParentUnit.Id;
            }
            if (pCodeInfo.GainingCommand == null)
            {
                gaining_command_cs_id = -1;
            }
            else
            {
                gaining_command_cs_id = pCodeInfo.GainingCommand.Id;
            }
            if (pCodeInfo.PasCode == null)
            {
                pas_code = "";
            }
            else
            {
                pas_code = pCodeInfo.PasCode;
            }
            if (pCodeInfo.BaseCode == null)
            {
                base_code = "";
            }
            else
            {
                base_code = pCodeInfo.BaseCode;
            }
            if (pCodeInfo.CommandStructOperationType == null)
            {
                cs_oper_type = "";
            }
            else
            {
                cs_oper_type = pCodeInfo.CommandStructOperationType;
            }
            if (pCodeInfo.TimeZone == null)
            {
                time_zone = "";
            }
            else
            {
                time_zone = pCodeInfo.TimeZone;
            }
            if (pCodeInfo.Component == null)
            {
                component = "";
            }
            else
            {
                component = pCodeInfo.Component;
            }
            if (pCodeInfo.Address1 == null)
            {
                address1 = "";
            }
            else
            {
                address1 = pCodeInfo.Address1;
            }
            if (pCodeInfo.Address2 == null)
            {
                address2 = "";
            }
            else
            {
                address2 = pCodeInfo.Address2;
            }
            if (pCodeInfo.City == null)
            {
                city = "";
            }
            else
            {
                city = pCodeInfo.City;
            }
            if (pCodeInfo.State == null)
            {
                state = "";
            }
            else
            {
                state = pCodeInfo.State;
            }
            if (pCodeInfo.PostalCode == null)
            {
                postal_code = "";
            }
            else
            {
                postal_code = pCodeInfo.PostalCode;
            }
            if (pCodeInfo.Country == null)
            {
                country = "";
            }
            else
            {
                country = pCodeInfo.Country;
            }
            if (pCodeInfo.Email == null)
            {
                Email = "";
            }
            else
            {
                Email = pCodeInfo.Email;
            }

        }

        #endregion

        #region Update

        public void UpdatePasCode()
        {

            SqlDataStore adapter = new SqlDataStore();
            DbCommand dbCommand;

            dbCommand = adapter.GetStoredProcCommand("core_pascode_sp_update");

            adapter.AddInParameter(dbCommand, "@CS_ID", DbType.Int32, cs_id);
            if (long_name != "") adapter.AddInParameter(dbCommand, "@LONG_NAME", DbType.String, long_name);
            if (unit_nbr != "") adapter.AddInParameter(dbCommand, "@UNIT_NBR", DbType.String, unit_nbr);
            if (unit_kind != "") adapter.AddInParameter(dbCommand, "@UNIT_KIND", DbType.String, unit_kind);
            if (unit_type != "") adapter.AddInParameter(dbCommand, "@UNIT_TYPE", DbType.String, unit_type);
            if (unit_det != "") adapter.AddInParameter(dbCommand, "@UNIT_DET", DbType.String, unit_det);
            if (uic != "") adapter.AddInParameter(dbCommand, "@UIC", DbType.String, uic);
            if (cs_level != "") adapter.AddInParameter(dbCommand, "@CS_LEVEL", DbType.String, cs_level);
            if (cs_id_parent != -1) adapter.AddInParameter(dbCommand, "@CS_ID_PARENT", DbType.Int32, cs_id_parent);
            if (gaining_command_cs_id != -1) adapter.AddInParameter(dbCommand, "@GAINING_COMMAND_CS_ID", DbType.Int32, gaining_command_cs_id);
            if (pas_code != "") adapter.AddInParameter(dbCommand, "@PAS_CODE", DbType.String, pas_code);
            if (base_code != "") adapter.AddInParameter(dbCommand, "@BASE_CODE", DbType.String, base_code);
            if (cs_oper_type != "") adapter.AddInParameter(dbCommand, "@CS_OPER_TYPE", DbType.String, cs_oper_type);
            if (time_zone != "") adapter.AddInParameter(dbCommand, "@TIME_ZONE", DbType.String, time_zone);
            if (component != "") adapter.AddInParameter(dbCommand, "@COMPONENT", DbType.String, component);
            if (address1 != "") adapter.AddInParameter(dbCommand, "@ADDRESS1", DbType.String, address1);
            if (address2 != "") adapter.AddInParameter(dbCommand, "@ADDRESS2", DbType.String, address2);
            if (city != "") adapter.AddInParameter(dbCommand, "@CITY", DbType.String, city);
            if (country != "") adapter.AddInParameter(dbCommand, "@COUNTRY", DbType.String, country);
            if (state != "") adapter.AddInParameter(dbCommand, "@STATE", DbType.String, state);
            if (postal_code != "") adapter.AddInParameter(dbCommand, "@POSTAL_CODE", DbType.String, postal_code);
            if (Email != "") adapter.AddInParameter(dbCommand, "@E_MAIL", DbType.String, Email);

            adapter.ExecuteNonQuery(dbCommand);

        }

        public void UpdateReporting(int userId)
        {
            XMLString xml = new XMLString("ReportList");
            foreach (KeyValuePair<string, int> entry in reportingStructure)
            {
                xml.BeginElement("command");
                xml.WriteAttribute("cs_id", cs_id);
                xml.WriteAttribute("chain_type", entry.Key);
                xml.WriteAttribute("parent_cs_id", entry.Value);
                xml.EndElement();
            }

            SqlDataStore adapter = new SqlDataStore();
            adapter.ExecuteNonQuery("core_pascode_sp_updateReporting", userId, xml.ToString());
        }

        #endregion

        #endregion
    }
}
