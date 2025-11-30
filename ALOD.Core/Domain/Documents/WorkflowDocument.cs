using ALOD.Core.Domain.Modules.Appeals;
using ALOD.Core.Domain.Modules.Lod;
using ALOD.Core.Domain.Modules.Reinvestigations;
using ALOD.Core.Domain.Modules.SARC;
using ALOD.Core.Domain.Modules.SpecialCases;
using ALOD.Core.Domain.Workflow;
using ALOD.Core.Interfaces.DAOInterfaces;
using System.Collections.Generic;
using System.Linq;

namespace ALOD.Core.Domain.Documents
{
    public class WorkflowDocument
    {
        public WorkflowDocument(SpecialCase SpecCase, int DocStart, IDaoFactory factory)
        {
            InitializeLists();

            refID = SpecCase.Id;
            moduleId = SpecCase.moduleId;
            DocumentViewId.Add(SpecCase.DocumentViewId);
            DocumentEntityId = SpecCase.DocumentEntityId;
            DocumentGroupId.Add(SpecCase.DocumentGroupId.Value);
            GetSpecialCaseDocuments(SpecCase, factory);
            Required = SpecCase.Required;
            DocumentStart = DocStart;
        }

        public WorkflowDocument(LineOfDuty LOD, int DocStart, IDaoFactory factory)
        {
            InitializeLists();

            LOD.ProcessDocuments(factory);

            refID = LOD.Id;
            moduleId = LOD.ModuleType;
            DocumentViewId.Add(LOD.DocumentViewId);
            DocumentEntityId = LOD.DocumentEntityId;
            if (LOD.DocumentGroupId.HasValue)
            {
                DocumentGroupId.Add(LOD.DocumentGroupId.Value);
            }
            else
            {
                // Optionally, you can handle the scenario where LOD.DocumentGroupId is null.
                // For instance, logging an error, throwing an exception, or using a default value.
                // Example:
                // throw new ArgumentException("LOD.DocumentGroupId cannot be null.");
            }
            AllDocuments.Add(LOD.Active);
            Required = LOD.Required;
            LODStatusCode = LOD.CurrentStatusCode;
            DocumentStart = DocStart;

            GetLODDocumentsInfo(LOD, factory);
        }

        public WorkflowDocument(LODAppeal Appeal, int DocStart, IDaoFactory factory)
        {
            InitializeLists();

            Appeal.ProcessDocuments(factory);

            GetAPDocumentsInfo(Appeal, factory);

            refID = Appeal.Id;
            moduleId = Appeal.ModuleId;
            DocumentViewId.Add(Appeal.DocumentViewId);
            DocumentEntityId = Appeal.DocumentEntityId;
            DocumentGroupId.Add(Appeal.DocumentGroupId.Value);
            AllDocuments.Add(Appeal.AllDocuments);
            Required = Appeal.Required;
            DocumentStart = DocStart;
        }

        public WorkflowDocument(LODReinvestigation reinvestigation, int DocStart, IDaoFactory factory)
        {
            InitializeLists();
            GetRRDocumentsInfo(reinvestigation, factory);

            refID = reinvestigation.Id;
            moduleId = reinvestigation.ModuleId;
            DocumentViewId.Add(reinvestigation.DocumentViewId);
            DocumentEntityId = reinvestigation.DocumentEntityId;
            DocumentGroupId.Add(reinvestigation.DocumentGroupId.Value);
            AllDocuments.Add(reinvestigation.AllDocuments);
            Required = reinvestigation.Required;
            DocumentStart = DocStart;
        }

        public WorkflowDocument(RestrictedSARC sarc, int DocStart)
        {
            InitializeLists();

            refID = sarc.Id;
            moduleId = sarc.ModuleId;
            DocumentViewId.Add(sarc.DocumentViewId);
            DocumentEntityId = sarc.DocumentEntityId;
            DocumentGroupId.Add(sarc.DocumentGroupId.Value);
            AllDocuments.Add(sarc.AllDocuments);
            Required = sarc.Required;
            DocumentStart = DocStart;
        }

        public WorkflowDocument(SARCAppeal appeal, int DocStart)
        {
            InitializeLists();

            refID = appeal.Id;
            moduleId = appeal.ModuleId;
            DocumentViewId.Add(appeal.DocumentViewId);
            DocumentEntityId = appeal.DocumentEntityId;
            DocumentGroupId.Add(appeal.DocumentGroupId.Value);
            AllDocuments.Add(appeal.AllDocuments);
            Required = appeal.Required;
            DocumentStart = DocStart;
        }

        public IList<IDictionary<string, bool>> AllDocuments { get; set; }
        public string DocumentEntityId { get; set; }
        public IList<long> DocumentGroupId { get; set; }
        public int DocumentStart { get; set; }
        public IList<int> DocumentViewId { get; set; }
        public int LODStatusCode { get; set; }
        public int moduleId { get; set; }
        public int refID { get; set; }
        public IDictionary<string, bool> Required { get; set; }

        public void GetAPDocumentsInfo(LODAppeal appeal, IDaoFactory factory)
        {
            int initialId = appeal.InitialLodId;

            if (initialId > 0)
            {
                LineOfDuty LOD = factory.GetLineOfDutyDao().GetById(initialId);
                IDocCategoryViewDao DocCatViewDao = factory.GetDocCategoryViewDao();

                LOD.ProcessDocuments(factory);

                IDictionary<string, bool> Active = (from Doc in LOD.Active where Doc.Key.Equals(DocumentType.SignedNotificationMemo.ToString()) select Doc).ToList().ToDictionary(x => x.Key, x => x.Value);

                DocumentViewId.Add(LOD.DocumentViewId);
                DocumentGroupId.Add(LOD.DocumentGroupId.Value);
                AllDocuments.Add(Active);
            }
        }

        public void GetLODDocumentsInfo(LineOfDuty lod, IDaoFactory factory)
        {
            GetAPDocumentsInfoForLODDocuments(lod, factory);
            GetSARCAPDocumentsInfoForLODDocuments(lod, factory);
        }

        public void GetRRDocumentsInfo(LODReinvestigation rr, IDaoFactory factory)
        {
            if (rr.InitialLodId > 0)
            {
                LineOfDuty lod = factory.GetLineOfDutyDao().GetById(rr.InitialLodId);
                IDocCategoryViewDao DocCatViewDao = factory.GetDocCategoryViewDao();

                lod.ProcessDocuments(factory);

                DocumentViewId.Add(lod.DocumentViewId);
                DocumentGroupId.Add(lod.DocumentGroupId.Value);
                AllDocuments.Add(lod.Active);
            }
        }

        public void GetSpecialCaseDocuments(SpecialCase SpecCase, IDaoFactory factory)
        {
            if (SpecCase.moduleId == (int)ModuleType.SpecCaseRS)
            {
                ISpecialCaseDAO SCDao = factory.GetSpecialCaseDAO();

                if (SCDao.GetIsReassessmentCase(SpecCase.Id))
                {
                    AllDocuments.Add(SpecCase.AllDocuments);
                }
                else
                {
                    AllDocuments.Add((from docs in SpecCase.AllDocuments where !docs.Key.Equals(DocumentType.OriginalDocuments.ToString()) select docs).ToList().ToDictionary(x => x.Key, x => x.Value));
                }
            }
            else if (SpecCase.moduleId == (int)ModuleType.SpecCaseIncap)
            {
                ISpecialCaseDAO SCDao = factory.GetSpecialCaseDAO();

                if (SpecCase.Status == (int)AFRCWorkflows.INInitiate)
                {
                    AllDocuments.Add((from docs in SpecCase.AllDocuments
                                      where
                                                            !docs.Key.Equals(DocumentType.Form1971.ToString()) &&
                                                            !docs.Key.Equals(DocumentType.Medical_1971.ToString())
                                                            && !docs.Key.Equals(DocumentType.UnitCC_1971.ToString())
                                                            && !docs.Key.Equals(DocumentType.Legal_1971.ToString())
                                                            && !docs.Key.Equals(DocumentType.Finance_1971.ToString())
                                                            && !docs.Key.Equals(DocumentType.WingCC_1971.ToString())
                                      select docs).ToList().ToDictionary(x => x.Key, x => x.Value));
                }
                else if (SpecCase.Status == (int)AFRCWorkflows.INMedicalReview_WG)
                {
                    AllDocuments.Add((from docs in SpecCase.AllDocuments
                                      where !docs.Key.Equals(DocumentType.Form1971.ToString())
                                                            && !docs.Key.Equals(DocumentType.UnitCC_1971.ToString())
                                                            && !docs.Key.Equals(DocumentType.Legal_1971.ToString())
                                                            && !docs.Key.Equals(DocumentType.Finance_1971.ToString())
                                                            && !docs.Key.Equals(DocumentType.WingCC_1971.ToString())
                                      select docs).ToList().ToDictionary(x => x.Key, x => x.Value));
                }
                else if (SpecCase.Status == (int)AFRCWorkflows.INImmediateCommanderReview) //Incap Unit CC or Incap Immediate Commander Review
                {
                    AllDocuments.Add((from docs in SpecCase.AllDocuments
                                      where !docs.Key.Equals(DocumentType.Form1971.ToString())
                                                            && !docs.Key.Equals(DocumentType.INCAPPayPM_1971.ToString())
                                                            && !docs.Key.Equals(DocumentType.Legal_1971.ToString())
                                                            && !docs.Key.Equals(DocumentType.Finance_1971.ToString())
                                                            && !docs.Key.Equals(DocumentType.WingCC_1971.ToString())
                                      select docs).ToList().ToDictionary(x => x.Key, x => x.Value));
                }
                else if (SpecCase.Status == (int)AFRCWorkflows.INWingJAReview) //Incap Wing Legal or IN Wing JA Review
                {
                    AllDocuments.Add((from docs in SpecCase.AllDocuments
                                      where !docs.Key.Equals(DocumentType.Form1971.ToString())
                                                            && !docs.Key.Equals(DocumentType.Medical_1971.ToString())
                                                            && !docs.Key.Equals(DocumentType.INCAPPayPM_1971.ToString())
                                                            && !docs.Key.Equals(DocumentType.Finance_1971.ToString())
                                                            && !docs.Key.Equals(DocumentType.WingCC_1971.ToString())
                                      select docs).ToList().ToDictionary(x => x.Key, x => x.Value));
                }
                else if (SpecCase.Status == (int)AFRCWorkflows.INFinanceReview) //Incap Finance Review
                {
                    AllDocuments.Add((from docs in SpecCase.AllDocuments
                                      where !docs.Key.Equals(DocumentType.Form1971.ToString())
                                                            && !docs.Key.Equals(DocumentType.Medical_1971.ToString())
                                                            && !docs.Key.Equals(DocumentType.INCAPPayPM_1971.ToString())
                                                            && !docs.Key.Equals(DocumentType.UnitCC_1971.ToString())
                                                            && !docs.Key.Equals(DocumentType.WingCC_1971.ToString())
                                      select docs).ToList().ToDictionary(x => x.Key, x => x.Value));
                }
                else if (SpecCase.Status == (int)AFRCWorkflows.INWingCCAction) //Incap Wing CC Action
                {
                    AllDocuments.Add((from docs in SpecCase.AllDocuments
                                      where !docs.Key.Equals(DocumentType.Form1971.ToString())
                                                            && !docs.Key.Equals(DocumentType.Medical_1971.ToString())
                                                            && !docs.Key.Equals(DocumentType.INCAPPayPM_1971.ToString())
                                                            && !docs.Key.Equals(DocumentType.UnitCC_1971.ToString())
                                                            && !docs.Key.Equals(DocumentType.Legal_1971.ToString())
                                      select docs).ToList().ToDictionary(x => x.Key, x => x.Value));
                }
                else
                {
                    AllDocuments.Add((from docs in SpecCase.AllDocuments
                                      where !docs.Key.Equals(DocumentType.Form1971.ToString())
                                                            && !docs.Key.Equals(DocumentType.Medical_1971.ToString())
                                                            && !docs.Key.Equals(DocumentType.INCAPPayPM_1971.ToString())
                                                            && !docs.Key.Equals(DocumentType.UnitCC_1971.ToString())
                                                            && !docs.Key.Equals(DocumentType.Legal_1971.ToString())
                                                            && !docs.Key.Equals(DocumentType.Finance_1971.ToString())
                                      select docs).ToList().ToDictionary(x => x.Key, x => x.Value));
                }
            }
            else
            {
                AllDocuments.Add(SpecCase.AllDocuments);
            }
        }

        public void InitializeLists()
        {
            DocumentViewId = new List<int>();
            DocumentGroupId = new List<long>();
            AllDocuments = new List<IDictionary<string, bool>>();
        }

        private void GetAPDocumentsInfoForLODDocuments(LineOfDuty lod, IDaoFactory factory)
        {
            int LODAppealId = factory.GetLODAppealDao().GetAppealIdByInitLod(lod.Id);

            if (LODAppealId <= 0)
                return;

            LODAppeal LODAppeal = factory.GetLODAppealDao().GetById(LODAppealId);

            if (LODAppeal == null || !LODAppeal.DocumentGroupId.HasValue || LODAppeal.DocumentGroupId.Value == 0)
                return;

            DocumentViewId.Add(LODAppeal.DocumentViewId);
            DocumentGroupId.Add(LODAppeal.DocumentGroupId.Value);
            AllDocuments.Add(LODAppeal.AllDocuments);
        }

        private void GetSARCAPDocumentsInfoForLODDocuments(LineOfDuty lod, IDaoFactory factory)
        {
            int SARCappealId = factory.GetSARCAppealDao().GetAppealIdByInitId(lod.Id, lod.Workflow);

            if (SARCappealId <= 0)
                return;

            SARCAppeal SARCAppeal = factory.GetSARCAppealDao().GetById(SARCappealId);

            if (SARCAppeal == null || !SARCAppeal.DocumentGroupId.HasValue || SARCAppeal.DocumentGroupId.Value == 0)
                return;

            DocumentViewId.Add(SARCAppeal.DocumentViewId);
            DocumentGroupId.Add(SARCAppeal.DocumentGroupId.Value);
            AllDocuments.Add(SARCAppeal.AllDocuments);
        }
    }
}