namespace ALOD.Core.Domain.Modules.Lod
{
    public class NatureOfIncident
    {
        public NatureOfIncident(int id, string value, string text)
        {
            this.Id = id;
            this.Value = value;
            this.Text = text;
        }

        public virtual int Id { get; set; }
        public virtual string Text { get; set; }
        public virtual string Value { get; set; }
    }
}