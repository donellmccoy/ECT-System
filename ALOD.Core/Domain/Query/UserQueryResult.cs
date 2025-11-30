using System.Collections.Generic;
using System.Data;

namespace ALOD.Core.Domain.Query
{
    public class UserQueryResult
    {
        public UserQueryResult()
        {
            this.QueryId = 0;
            this.QueryTitle = string.Empty;
            this.ResultData = new DataSet();
            this.Errors = new List<string>();
        }

        public virtual IList<string> Errors { get; set; }

        public virtual bool HasData
        {
            get
            {
                if (ResultData == null)
                    return false;

                if (ResultData.Tables.Count == 0)
                    return false;

                return true;
            }
        }

        public virtual bool HasErrors
        {
            get
            {
                if (Errors.Count > 0)
                    return true;
                else
                    return false;
            }
        }

        public virtual int QueryId { get; set; }
        public virtual string QueryTitle { get; set; }
        public virtual DataSet ResultData { get; set; }
    }
}