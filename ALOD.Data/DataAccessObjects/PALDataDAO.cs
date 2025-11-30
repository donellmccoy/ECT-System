using ALOD.Core.Domain.Modules.SpecialCases;
using ALOD.Core.Interfaces.DAOInterfaces;
using System;
using System.Data;

namespace ALOD.Data
{
    /// <summary>
    /// Data access object for <see cref="PAL_Data"/> entities.
    /// Provides operations for managing Participation Activity Limitation (PAL) data including member search and document retrieval.
    /// </summary>
    internal class PALDataDao : AbstractNHibernateDao<PAL_Data, int>, IPALDataDao
    {
        /// <inheritdoc/>
        public DataSet GetPALData(string partialSSN, string partialLastName)
        {
            SqlDataStore store = new SqlDataStore();
            if ((partialSSN.Length == 9) && (partialSSN.IndexOf("-") < 0))
            {
                string fullSSN = partialSSN.Substring(0, 3) + "-" + partialSSN.Substring(3, 2) + "-" + partialSSN.Substring(5);
                partialSSN = fullSSN;
            }
            DataSet dsSingleRow = store.ExecuteDataSet("core_lod_sp_GetPALDataByMemberSSN", partialSSN, partialLastName);

            // flip the one row into multiple rows
            DataTable dt = new DataTable();
            DataRow dr;

            // Since we know there is only 1 row and we want to assign the columns names
            dt.Columns.Add("Field");
            dt.Columns.Add("Value");

            //get all the columns and make it as rows

            //Decided to use the values for first row because column headers were not uniform
            String[] fieldNames = {"SSN", "Type", "Rating", "ASC Code", "Base Assign", "Completed By", "Disposistion", "Sent To", "Date Out", " Date Recieved",
                                 "FICHE", "Expiration Date", "Indefinite Waiver", "Waiver No Longer Required", "Diagnosis", "Remarks", "RWOA Reason", "Discharged?", "Retired?" };

            for (int j = 0; j < dsSingleRow.Tables[0].Columns.Count; j++)
            {
                dr = dt.NewRow();
                dr[0] = fieldNames[j];
                for (int k = 1; k <= dsSingleRow.Tables[0].Rows.Count; k++)
                    dr[k] = dsSingleRow.Tables[0].Rows[k - 1][j];

                dt.Rows.Add(dr);
            }

            DataSet dsReturn = new DataSet();
            if (dsSingleRow.Tables[0].Rows.Count > 0)
                dsReturn.Tables.Add(dt);
            return dsReturn;
        }

        /// <inheritdoc/>
        public DataSet GetPALDocuments(string Last4SSN, string LastName)
        {
            SqlDataStore store = new SqlDataStore();
            return store.ExecuteDataSet("report_sp_GetPALDocumentsForMember", LastName, Last4SSN);
        }
    }
}