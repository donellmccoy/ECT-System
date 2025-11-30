namespace ALOD.Core.Utils
{
    public interface IUtilityDao
    {
        bool AssignIo(int refId, int ioUserId, int aaUserId, bool isFormal);
    }
}