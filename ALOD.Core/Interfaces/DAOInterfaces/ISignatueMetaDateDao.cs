using ALOD.Core.Domain.Workflow;
using System.Collections.Generic;

namespace ALOD.Core.Interfaces.DAOInterfaces
{
    public interface ISignatueMetaDateDao : IDao<SignatureMetaData, int>
    {
        void AddForWorkStatus(SignatureMetaData signature);

        void DeleteForWorkStatus(int refId, int workflowId, int workStatus);

        IList<SignatureMetaData> GetAllForCase(int refId, int workflowId);

        SignatureMetaData GetByUserGroup(int refId, int workflowId, int groupId);

        SignatureMetaData GetByWorkStatus(int refId, int workflowId, int workStatus);
    }
}