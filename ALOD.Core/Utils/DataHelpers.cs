using ALOD.Core.Interfaces;
using ALOD.Core.Interfaces.DAOInterfaces;
using ALOD.Logging;
using System;
using System.Collections.Generic;
using System.Data;

namespace ALOD.Core.Utils
{
    public static class DataHelpers
    {
        public static T ExtractObjectFromDataSet<T>(DataSet dSet) where T : IExtractedEntity, new()
        {
            try
            {
                if (!IsValidDataSet(dSet))
                    return default(T);

                IList<T> extractedObjects = ExtractObjectsFromDataSet<T>(dSet);

                if (extractedObjects == null || extractedObjects.Count == 0)
                    return default(T);

                return extractedObjects[0];
            }
            catch (Exception e)
            {
                LogManager.LogError(e);
                return default(T);
            }
        }

        public static T ExtractObjectFromDataSet<T>(DataSet dSet, IDaoFactory daoFactory) where T : IExtractedEntity, new()
        {
            try
            {
                if (!IsValidDataSet(dSet))
                    return default(T);

                IList<T> extractedObjects = ExtractObjectsFromDataSet<T>(dSet, daoFactory);

                if (extractedObjects == null || extractedObjects.Count == 0)
                    return default(T);

                return extractedObjects[0];
            }
            catch (Exception e)
            {
                LogManager.LogError(e);
                return default(T);
            }
        }

        public static IList<T> ExtractObjectsFromDataSet<T>(DataSet dSet) where T : IExtractedEntity, new()
        {
            try
            {
                List<T> extractedObjects = new List<T>();

                if (!IsValidDataSet(dSet))
                    return extractedObjects;

                foreach (DataRow row in dSet.Tables[0].Rows)
                {
                    IExtractedEntity s = new T();
                    s.ExtractFromDataRow(row);

                    if (s != null)
                    {
                        extractedObjects.Add((T)s);
                    }
                }

                return extractedObjects;
            }
            catch (Exception e)
            {
                LogManager.LogError(e);
                return new List<T>();
            }
        }

        public static IList<T> ExtractObjectsFromDataSet<T>(DataSet dSet, IDaoFactory daoFactory) where T : IExtractedEntity, new()
        {
            try
            {
                List<T> extractedObjects = new List<T>();

                if (!IsValidDataSet(dSet))
                    return extractedObjects;

                foreach (DataRow row in dSet.Tables[0].Rows)
                {
                    IExtractedEntity s = new T();
                    s.ExtractFromDataRow(row, daoFactory);

                    if (s != null)
                    {
                        extractedObjects.Add((T)s);
                    }
                }

                return extractedObjects;
            }
            catch (Exception e)
            {
                LogManager.LogError(e);
                return new List<T>();
            }
        }

        public static bool GetBoolFromDataRow(string fieldName, DataRow row)
        {
            try
            {
                return bool.Parse(row[fieldName].ToString());
            }
            catch
            {
                return false;
            }
        }

        public static DateTime GetDateTimeFromDataRow(string fieldName, DataRow row)
        {
            try
            {
                return DateTime.Parse(row[fieldName].ToString());
            }
            catch
            {
                return new DateTime();
            }
        }

        public static int GetIntFromDataRow(string fieldName, DataRow row)
        {
            try
            {
                return int.Parse(row[fieldName].ToString());
            }
            catch
            {
                return 0;
            }
        }

        public static long GetLongFromDataRow(string fieldName, DataRow row)
        {
            try
            {
                return long.Parse(row[fieldName].ToString());
            }
            catch
            {
                return 0;
            }
        }

        public static bool? GetNullableBoolFromDataRow(string fieldName, DataRow row)
        {
            try
            {
                return bool.Parse(row[fieldName].ToString());
            }
            catch
            {
                return null;
            }
        }

        public static DateTime? GetNullableDateTimeFromDataRow(string fieldName, DataRow row)
        {
            try
            {
                return DateTime.Parse(row[fieldName].ToString());
            }
            catch
            {
                return null;
            }
        }

        public static int? GetNullableIntFromDataRow(string fieldName, DataRow row)
        {
            try
            {
                return int.Parse(row[fieldName].ToString());
            }
            catch
            {
                return null;
            }
        }

        public static string GetStringFromDataRow(string fieldName, DataRow row)
        {
            try
            {
                return row[fieldName].ToString();
            }
            catch
            {
                return string.Empty;
            }
        }

        public static bool IsValidDataSet(DataSet dSet)
        {
            if (dSet == null)
                return false;

            if (dSet.Tables.Count == 0)
                return false;

            if (dSet.Tables[0] == null)
                return false;

            return true;
        }
    }
}