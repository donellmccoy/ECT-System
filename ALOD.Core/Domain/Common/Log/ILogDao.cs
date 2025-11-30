namespace ALOD.Core.Domain.Log
{
    public interface ILogDao
    {
        ChangeSet GetChangeSetByLogId(int logId);

        ChangeSet GetChangeSetByReferenceId(int refId, byte moduleType);

        ChangeSet GetChangeSetByUserId(int userId);

        ChangeSet GetLastChangeSet(int id);

        void SaveChangeSet(int logId, ChangeSet changes);
    }
}