using ALOD.Core.Domain.Common;
using ALOD.Core.Utils;
using ALOD.Logging;
using System;
using System.Collections.Generic;
using System.Data;

namespace ALOD.Data
{
    /// <summary>
    /// Data access object for managing dropdown list edit operations.
    /// Handles retrieval, insertion, and updates of dropdown list items using stored procedures.
    /// </summary>
    public class DropDownListEditDao
    {
        private SqlDataStore _dataSource;

        /// <summary>
        /// Gets the SQL data store instance for database operations.
        /// </summary>
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

        /// <summary>
        /// Retrieves dropdown list items using a specified stored procedure.
        /// </summary>
        /// <param name="StoreProcedure">The name of the stored procedure to execute.</param>
        /// <returns>A list of dropdown list items, or an empty list on error.</returns>
        public IList<DropDownListEdit> GetDropDownList(string StoreProcedure)
        {
            try
            {
                if (String.IsNullOrEmpty(StoreProcedure))
                    return new List<DropDownListEdit>();

                DataSet dSet = DataSource.ExecuteDataSet(StoreProcedure);

                return DataHelpers.ExtractObjectsFromDataSet<DropDownListEdit>(dSet);
            }
            catch (Exception e)
            {
                LogManager.LogError(e);
                return new List<DropDownListEdit>();
            }
        }

        /// <summary>
        /// Inserts a new dropdown list item using a specified stored procedure.
        /// </summary>
        /// <param name="StoreProcedure">The name of the stored procedure to execute.</param>
        /// <param name="description">The description of the item.</param>
        /// <param name="type">The type of the item.</param>
        /// <param name="sort_order">The sort order for the item.</param>
        public void InsertDropDownList(string StoreProcedure, string description, string type, int sort_order)
        {
            DataSource.ExecuteNonQuery(StoreProcedure, description, type, sort_order);
        }

        /// <summary>
        /// Updates an existing dropdown list item using a specified stored procedure.
        /// </summary>
        /// <param name="StoreProcedure">The name of the stored procedure to execute.</param>
        /// <param name="id">The ID of the item to update.</param>
        /// <param name="description">The updated description.</param>
        /// <param name="type">The updated type.</param>
        /// <param name="sort_order">The updated sort order.</param>
        public void UpdateDropDownList(string StoreProcedure, int? id, string description, string type, int sort_order)
        {
            if (id == null)
                return;

            DataSource.ExecuteNonQuery(StoreProcedure, id, description, type, sort_order);
        }
    }
}