namespace ALOD.Core.Interfaces
{
    public interface IHasAssignedId<IdT>
    {
        void SetId(IdT assignedId);
    }
}