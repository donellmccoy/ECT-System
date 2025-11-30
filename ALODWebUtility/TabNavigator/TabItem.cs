using System;
using ALOD.Core.Domain.Workflow;

namespace ALODWebUtility.TabNavigation
{
    [Serializable]
    public class TabItem
    {
        PageAccessType _access = PageAccessType.None;
        bool _active = false;
        bool _completed = false;
        bool _enabled = false;
        short _order = 0;
        string _page = string.Empty;
        bool _readOnly = true;
        bool _required = false;
        string _script = string.Empty;
        string _title = string.Empty;
        bool _visible = true;

        public TabItem(string title, short order, string page)
        {
            _title = title;
            _order = order;
            _page = page;
        }

        public TabItem(string title)
        {
            _title = title;
        }

        public PageAccessType Access
        {
            get
            {
                return _access;
            }
            set
            {
                _access = value;
            }
        }

        public bool Active
        {
            get
            {
                return _active;
            }
            set
            {
                _active = value;
            }
        }

        public string ClientScript
        {
            get
            {
                return _script;
            }
            set
            {
                _script = value;
            }
        }

        public bool Completed
        {
            get
            {
                return _completed;
            }
            set
            {
                _completed = value;
            }
        }

        public bool Enabled
        {
            get
            {
                return _enabled;
            }
            set
            {
                _enabled = value;
            }
        }

        public bool IsReadOnly
        {
            get
            {
                return _readOnly;
            }
            set
            {
                _readOnly = value;
            }
        }

        public short Order
        {
            get
            {
                return _order;
            }
        }

        public string Page
        {
            get
            {
                return _page;
            }
        }

        public bool Required
        {
            get
            {
                return _required;
            }
            set
            {
                _required = value;
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

        public bool Visible
        {
            get
            {
                return _visible;
            }
            set
            {
                if (value == true && _access == PageAccessType.None)
                {
                    return; // don't allow tabs with no access to become visible
                }
                _visible = value;
            }
        }
    }
}
