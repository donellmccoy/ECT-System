using ALOD.Core.Domain.Query;
using System.Data;

namespace ALOD.Core.Interfaces.DAOInterfaces
{
    public interface IPHQueryDao : IDao<PHUserQuery, int>
    {
        void DeleteClause(PHQueryClause clause);

        void DeleteOutputField(PHQueryOutputField outputField);

        void DeleteParameter(PHQueryParameter parameter);

        DataSet ExecuteCasesQuery(int queryId, int userId, byte scope, int unitId, int viewType);

        UserQueryResult ExecuteTotalsQuery(int queryId, int userId, byte scope, int unitId, int viewType);

        PHUserQuery GetByBaseQueryId(int queryId);

        PHQueryOutputField GetOutputFieldById(int id);

        PHQueryParameter GetParameterById(int id);

        void SaveOrUpdateClause(PHQueryClause clause);

        void SaveOrUpdateOutputField(PHQueryOutputField outputField);

        void SaveOrUpdateParameter(PHQueryParameter parameter);

        void SaveOrUpdateQuery(PHUserQuery query);

        void UpdateParamExecuteOrders(int clauseId, int paramId, int newOrder, int oldOrder, int filter);
    }
}