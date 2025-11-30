using System;
using System.Collections.Generic;
using System.Security.Principal;
using System.Text;

namespace ALODWebUtility.Providers
{
    [CLSCompliant(false)]
    public class LodPrinciple : IPrincipal
    {
        protected IIdentity _identity;
        protected List<string> _roles;

        public string Permissions
        {
            get
            {
                StringBuilder buffer = new StringBuilder();

                foreach (string perm in _roles)
                {
                    buffer.Append(perm + ",");
                }

                if (buffer.Length > 0)
                {
                    buffer = buffer.Remove(buffer.Length - 1, 1);
                }

                return buffer.ToString();
            }
        }

        #region IPrinciple

        public IIdentity Identity
        {
            get
            {
                return this._identity;
            }
        }

        public bool IsInRole(string role)
        {
            return _roles.Contains(role);
        }

        #endregion

        #region Constructors

        public LodPrinciple(IIdentity identity)
        {
            _identity = identity;
            _roles = new List<string>();
        }

        public LodPrinciple(IIdentity identity, List<string> roles)
        {
            _identity = identity;
            _roles = roles;
        }

        public LodPrinciple(IIdentity identity, string[] roles)
        {
            _identity = identity;
            _roles = new List<string>();

            foreach (string item in roles)
            {
                _roles.Add(item);
            }
        }

        public LodPrinciple(IIdentity identity, string roles, char seperator)
        {
            _identity = identity;
            _roles = new List<string>();

            string[] parts = roles.Split(new char[] { seperator });

            foreach (string item in parts)
            {
                _roles.Add(item);
            }
        }

        #endregion
    }
}
