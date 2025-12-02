using System;
using System.Collections.Generic;
using System.Security.Principal;
using System.Text;
using System.Web.Security;
using ALODWebUtility.Perms;

namespace ALODWebUtility.Providers
{
    public class UserAuthentication : MembershipUser, IIdentity
    {
        #region Members/Properties

        protected bool _isAuthenticated = false;
        protected List<string> _perms = new List<string>();
        protected string _userName = string.Empty;
        const string _delimiter = ",";

        public List<string> Permissions
        {
            get
            {
                return _perms;
            }
        }

        public string Roles
        {
            get
            {
                StringBuilder buffer = new StringBuilder();

                foreach (string item in _perms)
                {
                    buffer.Append(item + _delimiter);
                }

                if (buffer.Length > 0)
                {
                    buffer.Remove(buffer.Length - 1, 1);
                }

                return buffer.ToString();
            }

            set
            {
                foreach (string item in value.Split(new string[] { _delimiter }, StringSplitOptions.RemoveEmptyEntries))
                {
                    _perms.Add(item);
                }
            }
        }

        public override string UserName
        {
            get
            {
                return _userName;
            }
        }

        #endregion

        #region Constructors

        public UserAuthentication(string userName, string roles, bool isAuthed)
        {
            _userName = userName;
            this.Roles = roles;
            _isAuthenticated = isAuthed;
        }

        public UserAuthentication(string userName)
        {
            _userName = userName;
            LoadPermissions();
        }

        protected UserAuthentication()
        {
        }

        #endregion

        #region Loading

        protected void LoadPermissions()
        {
            PermissionList perms = new PermissionList();
            perms.GetByUserId(int.Parse(_userName));

            foreach (Permission perm in perms)
            {
                Permissions.Add(perm.Name);
            }

            _isAuthenticated = true;
        }

        #endregion

        #region IIdentity

        public string AuthenticationType
        {
            get
            {
                return "LodAuth";
            }
        }

        public bool IsAuthenticated
        {
            get
            {
                return _isAuthenticated;
            }
        }

        public string Name
        {
            get
            {
                return _userName;
            }
        }

        #endregion
    }
}
