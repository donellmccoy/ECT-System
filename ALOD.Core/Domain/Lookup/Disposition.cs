namespace ALOD.Core.Domain.Lookup
{
    public class Disposition
    {
        public Disposition()
        {
            Id = 0;
            Name = string.Empty;
        }

        public Disposition(int id, string name)
        {
            this.Id = id;
            this.Name = name;
        }

        public virtual int Id { get; set; }
        public virtual string Name { get; set; }
    }
}