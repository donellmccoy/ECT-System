using System;
using System.Data;

namespace ALOD.Core.Interfaces.DAOInterfaces
{
    public interface IApplicationWarmupProcessDao
    {
        void DeleteLogById(int logId);

        DateTime? FindProcessLastExecutionDate(string processName);

        DataSet GetAllLogs();

        bool IsProcessActive(string processName);
    }
}