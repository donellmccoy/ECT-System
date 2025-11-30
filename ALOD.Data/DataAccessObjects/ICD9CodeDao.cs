using ALOD.Core.Domain.Common;
using ALOD.Core.Domain.Modules.Lod;
using ALOD.Core.Interfaces.DAOInterfaces;
using NHibernate;
using System;
using System.Collections.Generic;
using System.Data;

namespace ALOD.Data
{
    /// <summary>
    /// Data access object for managing <see cref="ICD9Code"/> entities.
    /// Handles ICD-9/ICD-10 diagnostic code operations including code validation, hierarchy navigation, 7th character definitions, and nature of incident associations.
    /// </summary>
    public class ICD9CodeDao : AbstractNHibernateDao<ICD9Code, int>, IICD9CodeDao
    {
        /// <summary>
        /// Gets the current NHibernate session.
        /// </summary>
        private ISession Session
        {
            get { return NHibernateSessionManager.Instance.GetSession(); }
        }

        /// <summary>
        /// Determines whether an ICD code exists in the system.
        /// </summary>
        /// <param name="code">The ICD code to check.</param>
        /// <returns>True if the code exists; otherwise, false.</returns>
        public bool DoesCodeExist(string code)
        {
            SqlDataStore store = new SqlDataStore();
            Object result = store.ExecuteScalar("core_ICD_sp_FindCode", code);

            if (result == null)
                return false;
            else
                return true;
        }

        /// <summary>
        /// Retrieves the definition for a specific 7th character of an ICD code.
        /// </summary>
        /// <param name="codeId">The ICD code ID.</param>
        /// <param name="seventhCharacter">The 7th character to look up.</param>
        /// <returns>The definition of the 7th character, or an empty string if not found.</returns>
        public string Get7thCharacterDefinition(int codeId, string seventhCharacter)
        {
            DataSet code7thCharacters = Get7thCharacters(codeId);

            if (code7thCharacters == null || code7thCharacters.Tables.Count == 0)
                return string.Empty;

            foreach (DataRow row in code7thCharacters.Tables[0].Rows)
            {
                if (row["Character"].ToString().Equals(seventhCharacter))
                    return row["Definition"].ToString();
            }

            return string.Empty;
        }

        /// <summary>
        /// Retrieves all available 7th characters for an ICD code.
        /// </summary>
        /// <param name="codeId">The ICD code ID.</param>
        /// <returns>A DataSet containing 7th character definitions.</returns>
        public System.Data.DataSet Get7thCharacters(int codeId)
        {
            SqlDataStore store = new SqlDataStore();
            return store.ExecuteDataSet("core_ICD_sp_Find7thCharacters", codeId);
        }

        /// <summary>
        /// Retrieves nature of incident values associated with an ICD code.
        /// </summary>
        /// <param name="codeId">The ICD code ID.</param>
        /// <returns>A list of nature of incident values.</returns>
        public IList<NatureOfIncident> GetAssociatedNatureOfIncidentValues(int codeId)
        {
            SqlDataStore store = new SqlDataStore();
            DataSet dSet = store.ExecuteDataSet("core_ICD_sp_FindNatureOfIncidentValues", codeId);

            if (dSet == null)
                return null;

            if (dSet.Tables.Count == 0)
                return null;

            DataTable dTable = dSet.Tables[0];

            List<NatureOfIncident> values = new List<NatureOfIncident>();

            foreach (DataRow row in dTable.Rows)
            {
                NatureOfIncident value = ExtractNatureOfIncident(row);

                if (value != null)
                    values.Add(value);
            }

            return values;
        }

        /// <summary>
        /// Retrieves child ICD codes for a given parent code.
        /// </summary>
        /// <param name="parentId">The parent ICD code ID.</param>
        /// <param name="version">The ICD version (e.g., 9 or 10).</param>
        /// <param name="onlyActive">Whether to return only active codes.</param>
        /// <returns>A DataSet containing child codes.</returns>
        public System.Data.DataSet GetChildren(int parentId, int version, bool onlyActive)
        {
            SqlDataStore store = new SqlDataStore();
            return store.ExecuteDataSet("core_ICD_sp_GetChildren", parentId, version, onlyActive);
        }

        /// <summary>
        /// Retrieves the version (ICD-9 or ICD-10) of a code.
        /// </summary>
        /// <param name="codeId">The ICD code ID.</param>
        /// <returns>The code version string, or empty string if not found.</returns>
        public string GetCodeVersion(int codeId)
        {
            SqlDataStore store = new SqlDataStore();
            Object result = store.ExecuteScalar("core_ICD_sp_FindCodeVersion", codeId);

            if (result == null)
                return String.Empty;
            else
                return result.ToString();
        }

        /// <summary>
        /// Determines whether an ICD code has child codes.
        /// </summary>
        /// <param name="parentId">The parent ICD code ID.</param>
        /// <param name="onlyActive">Whether to check only active codes.</param>
        /// <returns>True if the code has children; otherwise, false.</returns>
        public bool HasChildren(int parentId, bool onlyActive)
        {
            System.Data.DataSet results = GetChildren(parentId, 10, onlyActive);

            if (results == null)
                return false;

            if (results.Tables.Count == 0)
                return false;

            foreach (System.Data.DataTable table in results.Tables)
            {
                if (table.Rows.Count > 0)
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Creates a mapping between an ICD code and a nature of incident value.
        /// </summary>
        /// <param name="codeId">The ICD code ID.</param>
        /// <param name="noiValue">The nature of incident value.</param>
        public void InsertNatureOfIncidentMapping(int codeId, string noiValue)
        {
            SqlDataStore store = new SqlDataStore();
            store.ExecuteNonQuery("core_ICD_sp_InsertNatureOfIncidentMapping", codeId, noiValue);
        }

        /// <summary>
        /// Updates an ICD code's properties.
        /// </summary>
        /// <param name="codeId">The ICD code ID.</param>
        /// <param name="code">The code value.</param>
        /// <param name="description">The code description.</param>
        /// <param name="isDisease">Whether the code represents a disease.</param>
        /// <param name="active">Whether the code is active.</param>
        public void UpdateCode(int codeId, string code, string description, bool isDisease, bool active)
        {
            SqlDataStore store = new SqlDataStore();
            store.ExecuteNonQuery("core_ICD_sp_UpdateCode", codeId, code, description, isDisease, active);
        }

        /// <summary>
        /// Extracts a <see cref="NatureOfIncident"/> object from a data row.
        /// </summary>
        /// <param name="row">The data row containing nature of incident data.</param>
        /// <returns>The extracted nature of incident object.</returns>
        private NatureOfIncident ExtractNatureOfIncident(DataRow row)
        {
            return new NatureOfIncident(int.Parse(row["Id"].ToString()), row["Value"].ToString(), row["Text"].ToString());
        }
    }
}