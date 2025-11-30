namespace ALOD.Core.Domain.Documents
{
    public class DocumentCategory2 : Entity
    {
        public virtual string CategoryDescription { get; set; }
        public virtual DocumentType DocCatId { get; set; }
    }
}