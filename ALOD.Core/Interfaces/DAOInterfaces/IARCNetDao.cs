using ALOD.Core.Domain.Reports;
using System;
using System.Data;

namespace ALOD.Core.Interfaces.DAOInterfaces
{
    public interface IARCNetDao
    {
        DateTime? GetARCNetImportLastExecutionDate();

        DataSet GetIAATrainingDataForUsers(ARCNetLookupReportArgs args);
    }
}