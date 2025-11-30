namespace ALOD.Core.Domain.Lookup
{
    public class CompletedByGroup
    {
        public CompletedByGroup()
        {
            Id = 0;
            Name = string.Empty;
        }

        public CompletedByGroup(int id, string name)
        {
            this.Id = id;
            this.Name = name;
        }

        public virtual int Id { get; set; }
        public virtual string Name { get; set; }
    }
}