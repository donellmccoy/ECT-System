using ALOD.Core.Interfaces;
using ALOD.Core.Interfaces.DAOInterfaces;
using ALOD.Core.Utils;
using ALOD.Logging;
using System;
using System.Data;

namespace ALOD.Core.Domain.Common
{
    public class MemoSignature : IExtractedEntity
    {
        public virtual int ptype { get; set; }
        public virtual int refId { get; set; }
        public virtual String sig_date { get; set; }
        public virtual String Signature { get; set; }
        public virtual int userId { get; set; }
        public virtual int workflow { get; set; }

        /// <inheritdoc/>
        public bool ExtractFromDataRow(DataRow row)
        {
            try
            {
                this.refId = DataHelpers.GetIntFromDataRow("refId", row);
                this.workflow = DataHelpers.GetIntFromDataRow("workflow", row);
                this.Signature = DataHelpers.GetStringFromDataRow("signature", row);
                this.sig_date = DataHelpers.GetStringFromDataRow("sig_date", row);
                this.userId = DataHelpers.GetIntFromDataRow("user_id", row);
                this.ptype = DataHelpers.GetIntFromDataRow("ptype", row);

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