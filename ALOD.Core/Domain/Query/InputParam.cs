using System.Data;

namespace ALOD.Core.Domain.Query
{
    public class InputParam
    {
        public InputParam()
        {
        }

        public InputParam(string name, object value, DbType type)
        {
            this.Name = name;
            this.Value = value;
            this.Type = type;
        }

        public string Name { get; set; }
        public DbType Type { get; set; }
        public object Value { get; set; }
    }
}