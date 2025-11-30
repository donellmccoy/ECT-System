using ALOD.Core.Domain.Lookup;
using ALOD.Core.Domain.Workflow;
using System.Collections.Generic;

namespace ALOD.Core.Interfaces.DAOInterfaces
{
    public interface ICertificationStampDao
    {
        IList<CertificationStamp> GetAll();

        CertificationStamp GetById(int id);

        IList<Workflow> GetCertificationStampWorkflows(int stampId);

        CertificationStamp GetSpecialCaseStamp(int refId, bool selectSecondary);

        IDictionary<string, string> GetStampData(int refId, bool selectSecondary);

        IList<CertificationStamp> GetWorkflowCertificationStamps(int workflowId);

        IList<CertificationStamp> GetWorkflowCertificationStampsByDisposition(int workflowId, bool isQualified);

        void Insert(CertificationStamp cs);

        void Update(CertificationStamp cs);

        void UpdateCertificationStampWorkflowsMaps(int dispositionId, IList<int> workflowIds);
    }
}