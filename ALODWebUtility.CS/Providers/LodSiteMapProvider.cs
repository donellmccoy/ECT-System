using System;
using System.Collections;
using System.Web;

namespace ALODWebUtility.Providers
{
    public class LodSiteMapProvider : System.Web.XmlSiteMapProvider
    {
        public override bool IsAccessibleToUser(HttpContext context, SiteMapNode node)
        {
            if (node == null)
            {
                throw new ArgumentNullException("node");
            }

            if (context == null)
            {
                throw new ArgumentNullException("context");
            }

            if (!this.SecurityTrimmingEnabled)
            {
                return true;
            }

            IList nodeRoles = node.Roles;
            System.Security.Principal.IPrincipal user = context.User;

            if (user == null || nodeRoles == null || nodeRoles.Count == 0)
            {
                return true;
            }

            if (nodeRoles.Contains("*"))
            {
                return true;
            }

            foreach (string role in nodeRoles)
            {
                if (user.IsInRole(role))
                {
                    return true;
                }
            }

            return false;
        }
    }
}
