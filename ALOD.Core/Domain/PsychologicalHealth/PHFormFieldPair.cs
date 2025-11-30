namespace ALOD.Core.Domain.PsychologicalHealth
{
    public class PHFormFieldPair
    {
        public PHFormFieldPair()
        {
            this.Field1 = null;
            this.Field2 = null;
            this.Value1 = null;
            this.Value2 = null;
        }

        public PHFormFieldPair(PHFormField field1, PHFormField field2, string value1, string value2)
        {
            this.Field1 = field1;
            this.Field2 = field2;
            this.Value1 = value1;
            this.Value2 = value2;
        }

        public string CombinedValue
        { get { return ToString(); } }

        public PHFormField Field1 { get; set; }
        public PHFormField Field2 { get; set; }
        public string Value1 { get; set; }
        public string Value2 { get; set; }

        public virtual bool IsValid()
        {
            if (Field1 == null || Field2 == null || Value1 == null || Value2 == null)
                return false;

            if (!Field1.IsValid() || !Field2.IsValid())
                return false;

            return true;
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            return Value1 + " / " + Value2;
        }
    }
}