using System;
using System.Collections.Generic;
using System.Linq;

namespace ALODWebUtility.Common
{
    public sealed class WorkingEnum<T> where T : struct
    {
        private static readonly IEnumerable<T> All = Enum.GetValues(typeof(T)).Cast<T>();

        private static readonly Dictionary<string, T> InsensitiveNames = All.ToDictionary(k => Enum.GetName(typeof(T), k).ToLowerInvariant());

        private static readonly Dictionary<T, string> Names = All.ToDictionary(k => k, v => v.ToString());

        private static readonly Dictionary<string, T> SensitiveNames = All.ToDictionary(k => Enum.GetName(typeof(T), k));

        private static readonly Dictionary<int, T> Values = All.ToDictionary(k => Convert.ToInt32(k));

        private WorkingEnum()
        {
        }

        public static T? CastOrNull(int value)
        {
            // 6/21/19
            T foundValue = default(T);

            if (Values.TryGetValue(value, out foundValue))
            {
                return foundValue;
            }

            return null;
        }

        public static string GetName(T value)
        {
            string name = "";
            Names.TryGetValue(value, out name);
            return name;
        }

        public static string[] GetNames()
        {
            return Names.Values.ToArray();
        }

        public static IEnumerable<T> GetValues()
        {
            return All;
        }

        public static bool IsDefined(T value)
        {
            return Names.Keys.Contains(value);
        }

        public static bool IsDefined(string value)
        {
            return SensitiveNames.Keys.Contains(value);
        }

        public static bool IsDefined(int value)
        {
            return Values.Keys.Contains(value);
        }

        public static T Parse(string value)
        {
            T parsed = default(T);
            if (!SensitiveNames.TryGetValue(value, out parsed))
            {
                throw new ArgumentException("Value is not one of the named constants defined for the enumeration", "value");
            }
            return parsed;
        }

        public static T Parse(string value, bool ignoreCase)
        {
            if (!ignoreCase)
            {
                return Parse(value);
            }

            T parsed = default(T);
            if (!InsensitiveNames.TryGetValue(value.ToLowerInvariant(), out parsed))
            {
                throw new ArgumentException("Value is not one of the named constants defined for the enumeration", "value");
            }
            return parsed;
        }

        public static T? ParseOrNull(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return null;
            }
            // 6/21/19
            T foundValue = default(T);

            if (InsensitiveNames.TryGetValue(value.ToLowerInvariant(), out foundValue))
            {
                return foundValue;
            }

            return null;
        }

        public static bool TryParse(string value, T returnValue)
        {
            return SensitiveNames.TryGetValue(value, out returnValue);
        }

        public static bool TryParse(string value, bool ignoreCase, T returnValue)
        {
            if (!ignoreCase)
            {
                return TryParse(value, returnValue);
            }

            return InsensitiveNames.TryGetValue(value.ToLowerInvariant(), out returnValue);
        }
    }
}
