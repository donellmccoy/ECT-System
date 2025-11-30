using ALOD.Core.Domain.Documents;
using ALOD.Core.Domain.Workflow;
using ALOD.Core.Interfaces.DAOInterfaces;
using NHibernate.Transform;
using System.Collections.Generic;

namespace ALOD.Data
{
    /// <summary>
    /// Data access object for managing <see cref="DocCategoryView"/> entities.
    /// Handles document category view operations including category retrieval by document view ID and workflow-based document view type mapping.
    /// </summary>
    public class DocCategoryViewDao : AbstractNHibernateDao<DocCategoryView, int>, IDocCategoryViewDao
    {
        /// <summary>
        /// Retrieves all document categories associated with a specific document view ID.
        /// </summary>
        /// <param name="docViewId">The document view ID.</param>
        /// <returns>A list of document categories.</returns>
        public IList<DocumentCategory2> GetCategoriesByDocumentViewId(int docViewId)
        {
            return NHibernateSession.GetNamedQuery("GetCategoriesByViewID")
                .SetInt32("viewId", docViewId)
                .SetResultTransformer(Transformers.AliasToBean(typeof(DocumentCategory2)))
                .List<DocumentCategory2>();
        }

        /// <summary>
        /// Maps a workflow ID to its corresponding document view type.
        /// </summary>
        /// <param name="workflowId">The workflow ID.</param>
        /// <returns>The document view type ID.</returns>
        public int GetDocumentViewByWorkflowId(int workflowId)
        {
            int docType;

            switch (workflowId)
            {
                case (int)AFRCWorkflows.SpecCaseBCMR:
                    docType = (int)DocumentViewType.BCMR;
                    break;

                case (int)AFRCWorkflows.SpecCaseBMT:
                    docType = (int)DocumentViewType.BMT;
                    break;

                case (int)AFRCWorkflows.SpecCaseCongress:
                    docType = (int)DocumentViewType.Congress;
                    break;

                case (int)AFRCWorkflows.SpecCaseCMAS:
                    docType = (int)DocumentViewType.CMAS;
                    break;

                case (int)AFRCWorkflows.SpecCaseFT:
                    docType = (int)DocumentViewType.FastTrack;
                    break;

                case (int)AFRCWorkflows.SpecCaseIncap:
                    docType = (int)DocumentViewType.Incap;
                    break;

                case (int)AFRCWorkflows.SpecCaseMEB:
                    docType = (int)DocumentViewType.MEB;
                    break;

                case (int)AFRCWorkflows.SpecCaseMEPS:
                    docType = (int)DocumentViewType.MEPS;
                    break;

                case (int)AFRCWorkflows.SpecCasePW:
                    docType = (int)DocumentViewType.PWaivers;
                    break;

                // new workflow: medical hold
                case (int)AFRCWorkflows.SpecCaseMH:
                    docType = (int)DocumentViewType.MH;
                    break;

                // new workflow: non-emergent surgery
                case (int)AFRCWorkflows.SpecCaseNE:
                    docType = (int)DocumentViewType.NE;
                    break;

                // new workflow: Deployment Waivers
                case (int)AFRCWorkflows.SpecCaseDW:
                    docType = (int)DocumentViewType.DW;
                    break;

                // new workflow: Modifications
                case (int)AFRCWorkflows.SpecCaseMO:
                    docType = (int)DocumentViewType.MO;
                    break;

                // new workflow: Physical Examination Processing Program
                case (int)AFRCWorkflows.SpecCasePEPP:
                    docType = (int)DocumentViewType.PEPP;
                    break;

                case (int)AFRCWorkflows.ReinvestigationRequest:
                    docType = (int)DocumentViewType.ReinvestigationRequest;
                    break;

                case (int)AFRCWorkflows.AppealRequest:
                    docType = (int)DocumentViewType.AppealRequest;
                    break;

                case (int)AFRCWorkflows.SpecCaseRS:
                    docType = (int)DocumentViewType.RS;
                    break;

                case (int)AFRCWorkflows.SpecCaseWWD:
                    docType = (int)DocumentViewType.WWD;
                    break;

                case (int)AFRCWorkflows.SARCRestricted:
                    docType = (int)DocumentViewType.SARC;
                    break;

                case (int)AFRCWorkflows.SARCRestrictedAppeal:
                    docType = (int)DocumentViewType.Appeal_SARC;
                    break;

                case (int)AFRCWorkflows.LOD_v2:
                    docType = (int)DocumentViewType.LineOfDuty_v2;
                    break;

                case (int)AFRCWorkflows.SpecCaseRW:
                    docType = (int)DocumentViewType.RW;
                    break;

                case (int)AFRCWorkflows.SpecCaseAGR:
                    docType = (int)DocumentViewType.AGRCert;
                    break;

                case (int)AFRCWorkflows.SpecCasePSCD:
                    docType = (int)DocumentViewType.PSCD;
                    break;

                /* -------- ANG ------- */
                case (int)AFRCWorkflows.ANGSpecCaseBCMR:
                    docType = (int)DocumentViewType.BCMR;
                    break;

                case (int)AFRCWorkflows.ANGSpecCaseBMT:
                    docType = (int)DocumentViewType.BMT;
                    break;

                case (int)AFRCWorkflows.ANGSpecCaseCongress:
                    docType = (int)DocumentViewType.Congress;
                    break;

                case (int)AFRCWorkflows.ANGSpecCaseCMAS:
                    docType = (int)DocumentViewType.CMAS;
                    break;

                case (int)AFRCWorkflows.ANGSpecCaseFT:
                    docType = (int)DocumentViewType.FastTrack;
                    break;

                case (int)AFRCWorkflows.ANGSpecCaseIncap:
                    docType = (int)DocumentViewType.Incap;
                    break;

                case (int)AFRCWorkflows.ANGSpecCaseMEB:
                    docType = (int)DocumentViewType.MEB;
                    break;

                case (int)AFRCWorkflows.ANGSpecCaseMEPS:
                    docType = (int)DocumentViewType.MEPS;
                    break;

                case (int)AFRCWorkflows.ANGSpecCasePW:
                    docType = (int)DocumentViewType.PWaivers;
                    break;

                // new workflow: medical hold
                case (int)AFRCWorkflows.ANGSpecCaseMH:
                    docType = (int)DocumentViewType.MH;
                    break;

                // new workflow: non-emergent surgery
                case (int)AFRCWorkflows.ANGSpecCaseNE:
                    docType = (int)DocumentViewType.NE;
                    break;

                // new workflow: Deployment Waivers
                case (int)AFRCWorkflows.ANGSpecCaseDW:
                    docType = (int)DocumentViewType.DW;
                    break;

                // new workflow: Modifications
                case (int)AFRCWorkflows.ANGSpecCaseMO:
                    docType = (int)DocumentViewType.MO;
                    break;

                // new workflow: Physical Examination Processing Program
                case (int)AFRCWorkflows.ANGSpecCasePEPP:
                    docType = (int)DocumentViewType.PEPP;
                    break;

                case (int)AFRCWorkflows.ANGReinvestigationRequest:
                    docType = (int)DocumentViewType.ReinvestigationRequest;
                    break;

                case (int)AFRCWorkflows.ANGAppealRequest:
                    docType = (int)DocumentViewType.AppealRequest;
                    break;

                case (int)AFRCWorkflows.ANGSpecCaseRS:
                    docType = (int)DocumentViewType.RS;
                    break;

                case (int)AFRCWorkflows.ANGSpecCaseWWD:
                    docType = (int)DocumentViewType.WWD;
                    break;

                case (int)AFRCWorkflows.ANGSARCRestricted:
                    docType = (int)DocumentViewType.SARC;
                    break;

                case (int)AFRCWorkflows.ANGSARCRestrictedAppeal:
                    docType = (int)DocumentViewType.Appeal_SARC;
                    break;

                case (int)AFRCWorkflows.ANGLOD_v2:
                    docType = (int)DocumentViewType.LineOfDuty_v2;
                    break;

                case (int)AFRCWorkflows.ANGSpecCaseRW:
                    docType = (int)DocumentViewType.RW;
                    break;

                default:
                    docType = (int)DocumentViewType.LineOfDuty;
                    break;
            }
            return docType;
        }

        /// <summary>
        /// Retrieves redacted document categories associated with a specific document view ID.
        /// </summary>
        /// <param name="docViewId">The document view ID.</param>
        /// <returns>A list of redacted document categories.</returns>
        public IList<DocumentCategory2> GetRedactedCategoriesByDocumentViewId(int docViewId)
        {
            return NHibernateSession.GetNamedQuery("GetRedactedCategoriesByViewID")
                .SetInt32("viewId", docViewId)
                .SetResultTransformer(Transformers.AliasToBean(typeof(DocumentCategory2)))
                .List<DocumentCategory2>();
        }
    }
}