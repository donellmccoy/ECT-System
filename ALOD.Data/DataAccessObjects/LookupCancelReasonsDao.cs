using ALOD.Core.Domain.Lookup;
using ALOD.Core.Interfaces.DAOInterfaces;
using System.Collections.Generic;
using System.Data;

namespace ALOD.Data
{
    public class LookupCancelReasonsDao : ILookupCancelReasonsDao
    {
        private SqlDataStore _dataSource;

        private SqlDataStore DataSource
        {
            get
            {
                if (_dataSource == null)
                {
                    _dataSource = new SqlDataStore();
                }
                return _dataSource;
            }
        }

        /// <inheritdoc/>
        public IList<CancelReasons> GetAll()
        {
            DataSet dSet = DataSource.ExecuteDataSet("core_lookups_sp_GetCancelReasons");
            return GetCancelReasons(dSet);
        }

        /// <inheritdoc/>
        public CancelReasons GetById(int id)
        {
            DataSet dSet = DataSource.ExecuteDataSet("core_lookUps_sp_GetCancelReasonDescriptionById");
            IList<CancelReasons> results = GetCancelReasons(dSet);

            if (results.Count == 0)
                return null;

            return results[0];
        }

        /// <inheritdoc/>
        public void UpdateCancelReasons(int id, string description, int DisplayOrder)
        {
            if (id <= 0 || DisplayOrder <= 0)
                return;

            if (string.IsNullOrEmpty(description))
                return;

            DataSource.ExecuteNonQuery("core_lookups_sp_UpdateCancelReasons", id, description, DisplayOrder);
        }

        private CancelReasons ExtractCancelReasons(DataRow row)
        {
            return new CancelReasons(int.Parse(row["Id"].ToString()), row["Description"].ToString(), int.Parse(row["DisplayOrder"].ToString()));
        }

        private IList<CancelReasons> GetCancelReasons(DataSet dSet)
        {
            if (dSet == null)
                return null;

            if (dSet.Tables.Count == 0)
                return null;

            DataTable dTable = dSet.Tables[0];

            List<CancelReasons> values = new List<CancelReasons>();

            foreach (DataRow row in dTable.Rows)
            {
                CancelReasons value = ExtractCancelReasons(row);

                if (value != null)
                    values.Add(value);
            }

            return values;
        }
    }
}