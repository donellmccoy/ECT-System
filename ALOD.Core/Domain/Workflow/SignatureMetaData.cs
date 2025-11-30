using ALOD.Core.Interfaces;
using ALOD.Core.Interfaces.DAOInterfaces;
using ALOD.Core.Utils;
using ALOD.Logging;
using System;
using System.Data;

namespace ALOD.Core.Domain.Workflow
{
    public class SignatureMetaData : IExtractedEntity
    {
        public virtual DateTime date { get; set; }
        public virtual string NameAndRank { get; set; }
        public virtual int refId { get; set; }
        public virtual int sigId { get; set; }
        public virtual string Title { get; set; }
        public virtual int userGroup { get; set; }
        public virtual int userId { get; set; }
        public virtual int workflowId { get; set; }

        public virtual int workStatus { get; set; }

        /// <inheritdoc/>
        public bool ExtractFromDataRow(DataRow row)
        {
            try
            {
                this.sigId = DataHelpers.GetIntFromDataRow("Id", row);
                this.refId = DataHelpers.GetIntFromDataRow("refId", row);
                this.workflowId = DataHelpers.GetIntFromDataRow("workflowId", row);
                this.workStatus = DataHelpers.GetIntFromDataRow("workStatus", row);
                this.userGroup = DataHelpers.GetIntFromDataRow("userGroup", row);
                this.userId = DataHelpers.GetIntFromDataRow("userId", row);
                this.date = DataHelpers.GetDateTimeFromDataRow("date", row);
                this.NameAndRank = DataHelpers.GetStringFromDataRow("nameAndRank", row);
                this.Title = DataHelpers.GetStringFromDataRow("title", row);

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