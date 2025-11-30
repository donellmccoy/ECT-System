namespace ALOD.Core.Domain.Common.KeyVal
{
    public class KeyValValue
    {
        public KeyValValue()
        {
            this.Key = new KeyValKey();
        }

        public KeyValValue(int id, int valueId, string value, string desc, KeyValKey key)
        {
            this.Id = id;
            this.ValueId = valueId;
            this.Value = value;
            this.ValueDescription = desc;
            this.Key = key;
        }

        public virtual int Id { get; set; }
        public virtual KeyValKey Key { get; set; }
        public virtual string Value { get; set; }
        public virtual string ValueDescription { get; set; }
        public virtual int ValueId { get; set; }
    }
}