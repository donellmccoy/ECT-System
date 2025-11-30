using System;

namespace ALOD.Core.Domain.Common
{
    [Serializable]
    public class NullableFields
    {
        public static Nullable<T> GetNullableField<T>(object value) where T : struct
        {
            if (value == null)
                return null;
            if (Convert.IsDBNull(value))
                return null;
            return (T)value;
        }
    }
}