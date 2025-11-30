using ALOD.Core.Domain.Users;
using ALOD.Core.Interfaces.DAOInterfaces;
using ALOD.Core.Utils;
using NHibernate;
using NHibernate.Criterion;
using System;
using System.Collections.Generic;
using System.Data;

namespace ALOD.Data
{
    public class UnitDao : AbstractNHibernateDao<Unit, int>, IUnitDao
    {
        private SqlDataStore DataStore
        {
            get { return new SqlDataStore(); }
        }

        private ISession Session
        {
            get { return NHibernateSessionManager.Instance.GetSession(); }
        }

        /// <inheritdoc/>
        public IList<UnitLookup> GetAllSubUnitsForUnit(int unitId, int viewType)
        {
            DataSet dSet = DataStore.ExecuteDataSet("cmdStruct_sp_GetAllSubUnitsForUnit", unitId, viewType);

            return DataHelpers.ExtractObjectsFromDataSet<UnitLookup>(dSet);
        }

        /// <inheritdoc/>
        public Unit GetByNameAndPASCode(string name, string pasCode)
        {
            IList<Unit> result = Session.CreateCriteria(typeof(Unit))
                .Add(Expression.Eq("Name", name))
                .Add(Expression.Eq("PasCode", pasCode))
                .List<Unit>();

            if (result == null || result.Count == 0)
                return null;

            return result[0];
        }

        /// <inheritdoc/>
        public IList<UnitLookup> GetImmediateChildrenForUnit(int unitId, int viewType)
        {
            DataSet dSet = DataStore.ExecuteDataSet("cmdStruct_sp_GetImmediateChildrenForUnit", unitId, viewType);

            return DataHelpers.ExtractObjectsFromDataSet<UnitLookup>(dSet);
        }

        /// <inheritdoc/>
        public Dictionary<string, Int32> GetUnitReportingStructure(int unitId)
        {
            DataSet dSet = DataStore.ExecuteDataSet("core_pascode_sp_GetReporting", unitId);

            Dictionary<string, Int32> reportingStructure = new Dictionary<string, Int32>();

            if (dSet == null || dSet.Tables.Count == 0)
                return reportingStructure;

            DataTable dTable = dSet.Tables[0];

            if (dTable == null)
                return reportingStructure;

            string currentReportingViewType = string.Empty;
            int? currentUnitId = 0;

            foreach (DataRow row in dTable.Rows)
            {
                currentReportingViewType = DataHelpers.GetStringFromDataRow("Chain_Type", row);
                currentUnitId = DataHelpers.GetNullableIntFromDataRow("parent_cs_Id", row);

                if (!String.IsNullOrEmpty(currentReportingViewType) && currentUnitId.HasValue && currentUnitId.Value != 0)
                {
                    reportingStructure.Add(currentReportingViewType, currentUnitId.Value);
                }
            }

            return reportingStructure;
        }
    }
}