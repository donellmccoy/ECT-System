using System;

namespace ALOD.Core.Domain.Common
{
    [Serializable]
    public class ValidationItem
    {
        public ValidationItem(string section, string field, string message)
        {
            Field = field;
            Section = section;
            Message = message;
            IsError = true;
        }

        public ValidationItem(string section, string field, string message, bool isError)
        {
            Field = field;
            Section = section;
            Message = message;
            IsError = isError;
        }

        public string Field { get; set; }
        public bool IsError { get; set; }
        public string Message { get; set; }
        public string Section { get; set; }
    }
}