namespace ALOD.Core.Domain.Documents
{
    public interface IDocumentSource
    {
        string DocumentEntityId { get; }
        long DocumentGroupId { get; }
        int DocumentViewId { get; }
    }
}