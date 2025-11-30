using ALOD.Core.Domain.Query;
using System.Collections.Generic;
using System.Data;

namespace ALOD.Core.Interfaces.DAOInterfaces
{
    public interface IQueryDao : IDao<UserQuery, int>
    {
        void DeleteClause(QueryClause clause);

        void DeleteParameter(QueryParameter parameter);

        UserQueryResult ExecuteQuery(UserQueryArgs args);

        IList<InputSource> GetAllInputSources();

        DataSet GetInputChoices(InputSource source);

        IList<InputSource> GetInputSourcesByTableName(string tableName);

        IList<QueryOutputField> GetOutputFields();

        IList<QueryOutputField> GetOutputFields(string tableName);

        QueryParameter GetParameterById(int id);

        IList<UserQuery> GetSharedUserQueries();

        InputSource GetSourceById(int id);

        IList<UserQuery> GetUserQueries(int userId);

        void SaveOrUpdateClause(QueryClause clause);

        void SaveOrUpdateParameter(QueryParameter parameter);

        void SaveOrUpdateQuery(UserQuery query);

        void SaveOrUpdateSource(InputSource source);

        void UpdateParamExecuteOrders(int clauseId, int paramId, int newOrder, int oldOrder, int filter);
    }
}