namespace ALODWebUtility.Common
{
    public class childunit
    {
        private string _CHAIN_TYPE = string.Empty;
        private string _childName = string.Empty;
        private string _childPasCode = string.Empty;
        private int _cs_id = -1;
        private int _Level = -1;
        private int _parentCS_ID = -1;
        private int _userUnit = -1;

        private bool in_Active = false;

        public childunit()
        {
        }

        public string CHAIN_TYPE
        {
            get
            {
                return this._CHAIN_TYPE;
            }
            set
            {
                this._CHAIN_TYPE = value;
            }
        }

        public string childName
        {
            get
            {
                return this._childName;
            }
            set
            {
                this._childName = value;
            }
        }

        public string childPasCode
        {
            get
            {
                return this._childPasCode;
            }
            set
            {
                this._childPasCode = value;
            }
        }

        public int cs_id
        {
            get
            {
                return this._cs_id;
            }
            set
            {
                this._cs_id = value;
            }
        }

        public bool InActive
        {
            get
            {
                return this.in_Active;
            }
            set
            {
                this.in_Active = value;
            }
        }

        public int Level
        {
            get
            {
                return this._Level;
            }
            set
            {
                this._Level = value;
            }
        }

        public int parentCS_ID
        {
            get
            {
                return this._parentCS_ID;
            }
            set
            {
                this._parentCS_ID = value;
            }
        }

        public int userUnit
        {
            get
            {
                return this._userUnit;
            }
            set
            {
                this._userUnit = value;
            }
        }
    }
}
