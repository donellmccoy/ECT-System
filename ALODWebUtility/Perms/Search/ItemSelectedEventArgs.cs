using System;
using ALOD.Core.Domain.Workflow;

namespace ALODWebUtility.Perms.Search
{
    public class ItemSelectedEventArgs : EventArgs
    {
        private ModuleType _baseType;
        private bool _canEdit;

        // used by appeals
        private string _compo;

        private int _parentId;
        private int _recId;
        private bool _redirect = true;
        private int _refId;
        private int _requestId;
        private string _status = "";
        private ModuleType _type;
        private string _url = "";
        private short _workFlowId;

        /// <summary>
        /// For appeals this is the the base module type being appealed
        /// For non-appeals this will be the same as Type
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        public ModuleType BaseType
        {
            get
            {
                return _baseType;
            }
            set
            {
                _baseType = value;
            }
        }

        public bool CanEdit
        {
            get
            {
                return _canEdit;
            }
            set
            {
                _canEdit = value;
            }
        }

        public string Compo
        {
            get
            {
                return _compo;
            }
            set
            {
                _compo = value;
            }
        }

        public int ParentID
        {
            get
            {
                return this._parentId;
            }
            set
            {
                this._parentId = value;
            }
        }

        public int RecID
        {
            get
            {
                return this._recId;
            }
            set
            {
                this._recId = value;
            }
        }

        public bool Redirect
        {
            get
            {
                return _redirect;
            }
            set
            {
                _redirect = value;
            }
        }

        public int RefId
        {
            get
            {
                return _refId;
            }
            set
            {
                _refId = value;
            }
        }

        public int RequestId
        {
            get
            {
                return _requestId;
            }
            set
            {
                _requestId = value;
            }
        }

        public string Status
        {
            get
            {
                return _status;
            }
            set
            {
                _status = value;
            }
        }

        public ModuleType Type
        {
            get
            {
                return _type;
            }
            set
            {
                _type = value;
            }
        }

        public string Url
        {
            get
            {
                return _url;
            }
            set
            {
                _url = value;
            }
        }

        public short WorkFlowId
        {
            get
            {
                return _workFlowId;
            }
            set
            {
                _workFlowId = value;
            }
        }
    }
}
