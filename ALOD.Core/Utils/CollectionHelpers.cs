using Microsoft.SqlServer.Server;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web.UI.WebControls;

namespace ALOD.Core.Utils
{
    public static class CollectionHelpers
    {
        public static IList<SqlDataRecord> IntListToListOfSQLDataRecords(IList<int> list)
        {
            List<SqlDataRecord> newList = new List<SqlDataRecord>();

            if (list.Count == 0)
                return newList;

            foreach (int item in list)
            {
                SqlMetaData columnInfo = new SqlMetaData("Column1", SqlDbType.Int);
                SqlDataRecord record = new SqlDataRecord(new SqlMetaData[] { columnInfo });

                record.SetInt32(0, item);

                newList.Add(record);
            }

            return newList;
        }

        public static void MoveRadioButtonListItemToEndOfList(RadioButtonList radioButtonList, string desiredItemText)
        {
            if (radioButtonList == null || string.IsNullOrEmpty(desiredItemText))
                return;

            if (radioButtonList.Items.Count == 0)
                return;

            ListItem desiredItem = radioButtonList.Items.FindByText(desiredItemText);

            if (desiredItem == null)
                return;

            radioButtonList.Items.Remove(desiredItem);
            radioButtonList.Items.Add(desiredItem);
        }

        /// <summary>
        /// Compares two IEnumerable collections to determine if the two collections contain the same values regardless of order.
        /// If the lists contain custom reference types then those reference types need to implement the IEquatable interface
        /// and provide implementations for the Equals and GetHashCode functions. This function does not handle collections
        /// that are nullable types.
        /// <para>
        ///     Source: http://stackoverflow.com/questions/3669970/compare-two-listt-objects-for-equality-ignoring-order
        /// </para>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static bool ScrambledEquals<T>(IEnumerable<T> listA, IEnumerable<T> listB)
        {
            if (listA.Count<T>() != listB.Count<T>())
                return false;

            // The dictionary counts the number of equal elements...the hash code of the objects are used
            // which is why the IEquatable interface needs to be implemented...
            Dictionary<T, int> cnt = new Dictionary<T, int>();

            foreach (T item in listA)
            {
                if (cnt.ContainsKey(item))
                {
                    cnt[item]++;
                }
                else
                {
                    cnt.Add(item, 1);
                }
            }

            foreach (T item in listB)
            {
                // Check if the item is in listA...
                if (cnt.ContainsKey(item))
                {
                    cnt[item]--;
                }
                else
                {
                    return false;
                }
            }

            return cnt.Values.All(c => c == 0);
        }

        public static IList<SqlDataRecord> StringListToListOfSQLDataRecords(IList<string> list)
        {
            List<SqlDataRecord> newList = new List<SqlDataRecord>();

            if (list.Count == 0)
                return newList;

            foreach (string item in list)
            {
                SqlMetaData columnInfo = new SqlMetaData("Column1", SqlDbType.VarChar, 100);
                SqlDataRecord record = new SqlDataRecord(new SqlMetaData[] { columnInfo });

                record.SetString(0, item);

                newList.Add(record);
            }

            return newList;
        }
    }
}