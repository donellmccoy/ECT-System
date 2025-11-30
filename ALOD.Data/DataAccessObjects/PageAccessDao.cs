using ALOD.Core.Domain.Workflow;
using ALOD.Core.Interfaces.DAOInterfaces;
using ALOD.Core.Utils;
using NHibernate;
using NHibernate.Transform;
using System.Collections.Generic;

namespace ALOD.Data
{
    /// <summary>
    /// Data access object for <see cref="PageAccess"/> entities.
    /// Manages page access permissions for user groups by workflow and status.
    /// </summary>
    public class PageAccessDao : AbstractNHibernateDao<PageAccess, int>, IPageAccessDao
    {
        /// <inheritdoc/>
        public IList<PageAccess> GetByWorkflowAndStatus(string compo, byte workflowId, int statusId)
        {
            return NHibernateSession.GetNamedQuery("GetByWorkflowAndStatus")
                .SetString("compo", compo)
                .SetByte("workflow", workflowId)
                .SetInt32("status", statusId)
                .SetResultTransformer(Transformers.AliasToBean(typeof(PageAccess)))
                .List<PageAccess>();
        }

        /// <inheritdoc/>
        public IList<PageAccess> GetByWorkflowGroupAndStatus(byte workflowId, byte groupId, int workstatusId)
        {
            return NHibernateSession.GetNamedQuery("GetByWorkflowStatusAndGroup")
                .SetByte("workflow", workflowId)
                .SetInt32("status", workstatusId)
                .SetByte("group", groupId)
                .SetResultTransformer(Transformers.AliasToBean(typeof(PageAccess)))
                .List<PageAccess>();
        }

        /// <inheritdoc/>
        public void UpdateList(string compo, byte workflow, int status, IList<PageAccess> list)
        {
            XMLString xml = new XMLString("pages");

            foreach (PageAccess item in list)
            {
                if (item.Access != PageAccess.AccessLevel.None)
                {
                    xml.BeginElement("group");
                    xml.WriteAttribute("groupId", item.GroupId.ToString());
                    xml.WriteAttribute("pageId", item.PageId.ToString());
                    xml.WriteAttribute("access", ((byte)item.Access).ToString());
                    xml.EndElement();
                }
            }

            IQuery query = NHibernateSession.GetNamedQuery("UpdatePageAccess")
                .SetString("compo", compo)
                .SetByte("workflow", workflow)
                .SetInt32("status", status)
                .SetString("xml", xml.ToString());

            query.ExecuteUpdate();
        }
    }
}