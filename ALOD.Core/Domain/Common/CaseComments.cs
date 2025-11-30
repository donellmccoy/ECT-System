using ALOD.Core.Domain.Users;
using ALOD.Core.Interfaces;
using ALOD.Core.Interfaces.DAOInterfaces;
using ALOD.Core.Utils;
using ALOD.Logging;
using System;
using System.Data;

namespace ALOD.Core.Domain.Common
{
    public class CaseComments : IExtractedEntity
    {
        public virtual int caseId { get; set; }
        public virtual string comment { get; set; }
        public virtual int commentType { get; set; }
        public virtual AppUser createdBy { get; set; }
        public virtual DateTime createdDate { get; set; }
        public virtual bool deleted { get; set; }
        public virtual int Id { get; protected set; }
        public virtual int module { get; set; }

        /// <inheritdoc/>
        public virtual bool ExtractFromDataRow(DataRow row)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public bool ExtractFromDataRow(DataRow row, IDaoFactory daoFactory)
        {
            try
            {
                this.Id = DataHelpers.GetIntFromDataRow("id", row);
                this.caseId = DataHelpers.GetIntFromDataRow("lodid", row);
                this.comment = DataHelpers.GetStringFromDataRow("comments", row);
                this.createdDate = DataHelpers.GetDateTimeFromDataRow("created_date", row);
                this.deleted = DataHelpers.GetBoolFromDataRow("deleted", row);
                this.module = DataHelpers.GetIntFromDataRow("ModuleID", row);
                this.commentType = DataHelpers.GetIntFromDataRow("CommentType", row);
                this.createdBy = daoFactory.GetUserDao().GetById(DataHelpers.GetIntFromDataRow("created_by", row));

                return true;
            }
            catch (Exception e)
            {
                LogManager.LogError(e);
                return false;
            }
        }
    }
}