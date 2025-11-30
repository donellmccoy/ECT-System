using ALOD.Core.Interfaces;
using ALOD.Core.Interfaces.DAOInterfaces;
using ALOD.Core.Utils;
using ALOD.Logging;
using System;
using System.Data;

namespace ALOD.Core.Domain.Common
{
    public class ChildCaseComments : IExtractedEntity
    {
        public virtual int _caseId { get; set; }
        public virtual string _comment { get; set; }
        public virtual int _commentType { get; set; }
        public virtual string _createdBy { get; set; }
        public virtual DateTime _createdDate { get; set; }
        public virtual bool _deleted { get; set; }
        public virtual int _Id { get; protected set; }
        public virtual int _module { get; set; }
        public virtual int _parentCommentId { get; set; }
        public virtual string _role { get; set; }

        public int caseId
        {
            get
            {
                return this._caseId;
            }
            set
            {
                this._caseId = value;
            }
        }

        public string comment
        {
            get
            {
                return this._comment;
            }
            set
            {
                this._comment = value;
            }
        }

        public int commentType
        {
            get
            {
                return this._commentType;
            }
            set
            {
                this._commentType = value;
            }
        }

        public string createdBy
        {
            get
            {
                return this._createdBy;
            }
            set
            {
                this._createdBy = value;
            }
        }

        public DateTime createdDate
        {
            get
            {
                return this._createdDate;
            }
            set
            {
                this._createdDate = value;
            }
        }

        public bool deleted
        {
            get
            {
                return this._deleted;
            }
            set
            {
                this._deleted = value;
            }
        }

        public int Id
        {
            get
            {
                return this._Id;
            }
            set
            {
                this._Id = value;
            }
        }

        public int module
        {
            get
            {
                return this._module;
            }
            set
            {
                this._module = value;
            }
        }

        public int parentCommentId
        {
            get
            {
                return this._parentCommentId;
            }
            set
            {
                this._parentCommentId = value;
            }
        }

        public string role
        {
            get
            {
                return this._role;
            }
            set
            {
                this._role = value;
            }
        }

        /// <inheritdoc/>
        public bool ExtractFromDataRow(DataRow row)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public bool ExtractFromDataRow(DataRow row, IDaoFactory daoFactory)
        {
            try
            {
                this._Id = DataHelpers.GetIntFromDataRow("id", row);
                this._caseId = DataHelpers.GetIntFromDataRow("lodid", row);
                this._comment = DataHelpers.GetStringFromDataRow("comments", row);
                this._createdDate = DataHelpers.GetDateTimeFromDataRow("created_date", row);
                this._deleted = DataHelpers.GetBoolFromDataRow("deleted", row);
                this._module = DataHelpers.GetIntFromDataRow("ModuleID", row);
                this._commentType = DataHelpers.GetIntFromDataRow("CommentType", row);
                this._createdBy = daoFactory.GetUserDao().GetById(DataHelpers.GetIntFromDataRow("created_by", row)).CommentName;
                this._parentCommentId = DataHelpers.GetIntFromDataRow("ParentCommentId", row);
                this._role = DataHelpers.GetStringFromDataRow("role", row);

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