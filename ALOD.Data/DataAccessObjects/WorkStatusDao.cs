using ALOD.Core.Domain.Workflow;
using ALOD.Core.Interfaces.DAOInterfaces;
using NHibernate.Criterion;
using System;
using System.Collections.Generic;
using System.Data;

namespace ALOD.Data
{
    public class WorkStatusDao : AbstractNHibernateDao<WorkStatus, int>, IWorkStatusDao
    {
        //07-17-2019 S32019 - Curt Lucas
        /// <inheritdoc/>
        public IList<WorkStatus> GetByWorkflow(byte workflow)
        {
            // 09-19-2019 Darel Johnson
            IList<WorkStatus> list = NHibernateSession.CreateCriteria(typeof(WorkStatus))
                .Add(Expression.Eq("WorkflowId", workflow))
                .List<WorkStatus>();

            return list;
        }

        /// <inheritdoc/>
        public string GetDescription(int workstatusId)
        {
            SqlDataStore store = new SqlDataStore();
            Object result = store.ExecuteScalar("core_workstatus_sp_GetWorkStatusDescription", workstatusId);

            if (result == null)
                return String.Empty;
            else
                return result.ToString();
        }

        /// <inheritdoc/>
        public DataSet GetStatusDescription(int workstatusId)
        {
            SqlDataStore store = new SqlDataStore();
            return store.ExecuteDataSet("core_workstatus_sp_GetWorkStatusDescription", workstatusId);
        }
    }
}