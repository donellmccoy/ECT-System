using ALOD.Core.Interfaces;
using ALOD.Core.Interfaces.DAOInterfaces;
using ALOD.Core.Utils;
using ALOD.Logging;
using System;
using System.Data;

namespace ALOD.Core.Domain.Modules.SpecialCases
{
    public class AssociatedCase : IExtractedEntity
    {
        public AssociatedCase()
        {
        }

        public AssociatedCase(int refId, int workflow, int associated_RefId, int associated_workflowId, string caseId)
        {
            this.RefId = refId;
            this.workflowId = workflow;
            this.associated_RefId = associated_RefId;
            this.associated_workflowId = associated_workflowId;
            this.associated_caseId = caseId;
        }

        public virtual string associated_caseId { get; set; }
        public virtual int associated_RefId { get; set; }
        public virtual int associated_workflowId { get; set; }
        public virtual int RefId { get; set; }

        public virtual int workflowId { get; set; }

        /// <inheritdoc/>
        public bool ExtractFromDataRow(DataRow row)
        {
            try
            {
                this.RefId = DataHelpers.GetIntFromDataRow("refId", row);
                this.workflowId = DataHelpers.GetIntFromDataRow("workflowId", row);
                this.associated_RefId = DataHelpers.GetIntFromDataRow("associated_refId", row);
                this.associated_workflowId = DataHelpers.GetIntFromDataRow("associated_workflow", row);
                this.associated_caseId = DataHelpers.GetStringFromDataRow("associated_caseId", row);

                return true;
            }
            catch (Exception e)
            {
                LogManager.LogError(e);
                return false;
            }
        }

        /// <inheritdoc/>
        public bool ExtractFromDataRow(DataRow row, IDaoFactory daoFactory)
        {
            return ExtractFromDataRow(row);
        }
    }
}