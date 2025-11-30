using System;
using System.Collections.Generic;
using System.Data;

namespace ALOD.Core.Utils
{
    public static class AdHocQueryHelpers
    {
        public static int ConverToDbBool(bool value)
        {
            if (value)
                return 1;
            else
                return 0;
        }

        public static string ConvertToValidDbValue(string sourceType, string value)
        {
            switch (sourceType)
            {
                case "B":
                    return ConverToDbBool(bool.Parse(value)).ToString();

                case "D":
                case "T":
                case "C":
                    return "'" + value + "'";

                default:
                    return value;
            }
        }

        public static string GetUnitNameSourceFieldName(string sourceName)
        {
            if (sourceName.Equals("Unit Name"))
                return "member_unit_id";
            else if (sourceName.Equals("PH Wing RMU") || sourceName.Equals("PH Case Unit") || sourceName.Equals("Case Unit"))
                return "PH Wing RMU ID";
            else
                return "member_unit_id";
        }

        public static bool IsUnitNameSource(string sourceName)
        {
            List<string> unitNameSources = new List<string>();
            unitNameSources.Add("Unit Name");
            unitNameSources.Add("PH Wing RMU");
            unitNameSources.Add("Case Unit");
            unitNameSources.Add("PH Case Unit");

            if (unitNameSources.Contains(sourceName))
                return true;
            else
                return false;
        }

        public static DbType SourceTypeToDbType(string sourceType)
        {
            switch (sourceType)
            {
                case "T":
                    return DbType.String;

                case "N":
                    return DbType.Int32;

                case "B":
                    return DbType.Boolean;

                case "D":
                    return DbType.DateTime;

                case "C":
                    return DbType.String;

                default:
                    return DbType.String;
            }
        }

        public static string SourceValueToParamValue(string sourceType, string value)
        {
            switch (sourceType)
            {
                case "T":
                    return value;

                case "N":
                    return value;

                case "B":
                    return value;

                case "D":

                    string output = value;

                    if (!char.IsNumber(value[value.Length - 1]))
                    {
                        //this is a time span, so grab from the current date
                        string spanType = value.Substring(value.Length - 1);
                        int count = 0;

                        int.TryParse(value.Substring(0, value.Length - 1), out count);

                        switch (spanType)
                        {
                            case "D": //days
                                output = DateTime.Now.AddDays(-1 * count).ToString();
                                break;

                            case "W": //weeks
                                output = DateTime.Now.AddDays(-1 * 7 * count).ToString();
                                break;

                            case "M": //months
                                output = DateTime.Now.AddMonths(-1 * count).ToString();
                                break;

                            case "Y": //years
                                output = DateTime.Now.AddYears(-1 * count).ToString();
                                break;

                            default:
                                output = DateTime.Now.ToString();
                                break;
                        }
                    }
                    else
                    {
                        //this is an exact date, so use it
                    }

                    return output;

                case "C":
                    return value;

                default:
                    return value;
            }
        }
    }
}