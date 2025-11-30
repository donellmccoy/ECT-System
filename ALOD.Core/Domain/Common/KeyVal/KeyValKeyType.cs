namespace ALOD.Core.Domain.Common.KeyVal
{
    public class KeyValKeyType
    {
        public KeyValKeyType()
        {
        }

        public KeyValKeyType(int id, string name)
        {
            this.Id = id;
            this.TypeName = name;
        }

        public virtual int Id { get; set; }
        public virtual string TypeName { get; set; }
    }
}