namespace ALOD.Core.Domain.Log
{
    public class ChangeRow
    {
        public ChangeRow()
        {
            Id = 0;
            Section = "";
            Field = "";
            OldVal = "";
            NewVal = "";
        }

        public ChangeRow(string section, string field, string oldVal, string newVal)
        {
            this.Section = section;
            this.Field = field;
            this.OldVal = oldVal;
            this.NewVal = NewVal;
        }

        public string Field { get; set; }
        public int Id { get; set; }
        public string NewVal { get; set; }
        public string OldVal { get; set; }
        public string Section { get; set; }
    }
}