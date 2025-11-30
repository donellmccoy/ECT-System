namespace ALOD.Core.Domain.Common.KeyVal
{
    public class KeyValKey
    {
        public KeyValKey()
        {
            this.KeyType = new KeyValKeyType();
        }

        public KeyValKey(int id, string desc, KeyValKeyType type)
        {
            this.Id = id;
            this.Description = desc;
            this.KeyType = type;
        }

        public virtual string Description { get; set; }
        public virtual int Id { get; set; }
        public virtual KeyValKeyType KeyType { get; set; }
    }
}