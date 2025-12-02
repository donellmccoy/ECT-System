using System.Collections.Generic;
using ALOD.Core.Domain.Documents;
using ALOD.Core.Domain.Modules.Lod;
using ALOD.Core.Domain.Workflow;
using ALOD.Core.Interfaces.DAOInterfaces;
using ALOD.Data;

namespace ALODWebUtility.Printing
{
    // Called from lod/Print.aspx.vb and ReinvestigationRequests/Print.aspx.vb
    // Returns link to 348/261 forms for Print button

    public class PrintFinal
    {
        public string GetURL261(string lodid, string curSession, ILineOfDutyDao dao)
        {
            string str261 = "";
            string form261ID = "";

            LineOfDuty instance = dao.GetById(int.Parse(lodid));
            IDocumentDao DocumentDao = new SRXDocumentStore(curSession);
            IList<ALOD.Core.Domain.Documents.Document> _documents = null;

            if (instance.DocumentGroupId == 0)
            {
                instance.CreateDocumentGroup(DocumentDao);
            }

            if (_documents == null)
            {
                _documents = DocumentDao.GetDocumentsByGroupId(instance.DocumentGroupId.Value);
            }

            bool isDoc = false;
            foreach (var docItem in _documents)
            {
                if (docItem.DocType.ToString() == "FinalForm261")
                {
                    form261ID = docItem.Id.ToString();
                    isDoc = true;
                }
            }

            if (isDoc)
            {
                str261 = "~/Secure/Shared/DocumentViewer.aspx?docID=" + form261ID + "&modId=" + ModuleType.LOD;
            }
            else
            {
                str261 = "";
            }

            return str261;
        }

        public string GetURL348(string lodid, string curSession, ILineOfDutyDao dao)
        {
            string str348 = "";
            string form348ID = "";

            LineOfDuty instance = dao.GetById(int.Parse(lodid));
            IDocumentDao DocumentDao = new SRXDocumentStore(curSession);
            IList<ALOD.Core.Domain.Documents.Document> _documents;

            if (instance.DocumentGroupId == 0)
            {
                instance.CreateDocumentGroup(DocumentDao);
            }

            _documents = DocumentDao.GetDocumentsByGroupId(instance.DocumentGroupId.Value);

            if (instance.WorkflowStatus.StatusCodeType.IsFinal)
            {
                // fileSubString is used to get the correct 348 document if multiple 348s are associated with the LOD's group Id.
                // This happens when a case is overridden and recompleted or if a case is reinvestigated.
                string fileSubString = instance.CaseId + "-Generated";

                bool isDoc = false;

                foreach (var docItem in _documents)
                {
                    if (docItem.DocType.ToString() == "FinalForm348" && docItem.OriginalFileName.Contains(fileSubString))
                    {
                        form348ID = docItem.Id.ToString();
                        isDoc = true;
                    }
                }

                if (isDoc)
                {
                    str348 = "~/Secure/Shared/DocumentViewer.aspx?docID=" + form348ID + "&modId=" + ModuleType.LOD;
                }
                else
                {
                    str348 = "";
                }
            }
            else
            {
                str348 = "";
            }

            return str348;
        }
    }
}
