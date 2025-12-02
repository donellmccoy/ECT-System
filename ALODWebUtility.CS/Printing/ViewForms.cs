using System.Collections.Generic;
using ALOD.Core.Domain.Documents;
using ALOD.Core.Domain.Modules.Lod;
using ALOD.Core.Domain.Modules.SARC;
using ALOD.Core.Domain.Modules.SpecialCases;
using ALOD.Core.Domain.Workflow;
using ALOD.Data.Services;

namespace ALODWebUtility.Printing
{
    public static class ViewForms
    {
        public static string LinkAttribute261(string refID, IList<Document> _documents)
        {
            string strAttribute261 = "";
            string form261ID = "0";

            bool isDoc = false;

            if (_documents != null)
            {
                foreach (Document docItem in _documents)
                {
                    if (docItem.DocType.ToString() == "FinalForm261")
                    {
                        form261ID = docItem.Id.ToString();
                        isDoc = true;
                    }
                }
            }

            if (isDoc)
            {
                string url261 = "../Shared/DocumentViewer.aspx?docID=" + form261ID + "&modId=" + ModuleType.LOD;
                strAttribute261 = "viewDoc('" + url261 + "'); return false;";
            }
            else
            {
                // handles legacy data for now. See file Search.aspx.vb line 213 for comments
                strAttribute261 = "printForm(" + refID + ",'261');";
            }

            return strAttribute261;
        }

        public static string LinkAttribute348(string refID, IList<Document> _documents, string casetype)
        {
            string strAttribute348 = "";
            string form348ID = "0";
            bool isDoc = false;

            LineOfDuty lineOfDuty = null;

            lineOfDuty = LodService.GetById(int.Parse(refID));

            if (lineOfDuty != null && _documents != null)
            {
                // fileSubString is used to get the correct 348 document if multiple 348s are associated with the LOD's group Id.
                // This happens when a case is overridden and recompleted or if a case is reinvestigated.
                string fileSubString = lineOfDuty.CaseId + "-Generated";

                foreach (Document docItem in _documents)
                {
                    if (docItem.DocType.ToString() == "FinalForm348" && docItem.OriginalFileName.Contains(fileSubString))
                    {
                        form348ID = docItem.Id.ToString();
                        isDoc = true;
                    }
                }
            }

            if (isDoc)
            {
                string url348 = "../Shared/DocumentViewer.aspx?docID=" + form348ID + "&modId=" + ModuleType.LOD;
                strAttribute348 = "viewDoc('" + url348 + "'); return false;";
            }
            else
            {
                // handles legacy data for now.
                if (lineOfDuty.Formal == true)
                {
                    strAttribute348 = "printForm('" + casetype + "', " + refID + ");";
                }
                else
                {
                    strAttribute348 = "printForm('" + casetype + "', " + refID + ",'348');";
                }
            }

            return strAttribute348;
        }

        public static string PHFormPDFLinkAttribute(SpecialCase specCase, IList<Document> documents)
        {
            string strAttribute = "";
            string phFormDocId = "0";
            bool isDoc = false;

            if (specCase == null)
            {
                return "return false;";
            }

            if (documents != null)
            {
                // fileSubString is used to get the correct PH form document if multiple PH forms are associated with the case's group Id.
                // This happens when a case is overridden and recompleted or if a case is reinvestigated.
                string fileSubString = specCase.CaseId + "-Generated";

                foreach (Document docItem in documents)
                {
                    if (docItem.DocType == DocumentType.PHForm && docItem.OriginalFileName.Contains(fileSubString))
                    {
                        phFormDocId = docItem.Id.ToString();
                        isDoc = true;
                    }
                }
            }

            if (isDoc)
            {
                string docViewerURL = "../Shared/DocumentViewer.aspx?docID=" + phFormDocId + "&modId=" + ModuleType.SpecCasePH;
                strAttribute = "viewDoc('" + docViewerURL + "'); return false;";
            }
            else
            {
                strAttribute = "printForms('" + specCase.Id.ToString() + "', 'SC_PH');";
            }

            return strAttribute;
        }

        public static string RestrictedSARCFormPDFLinkAttribute(RestrictedSARC sarc, IList<Document> documents)
        {
            string strAttribute = "";
            string sarcFormDocId = "0";
            bool isDoc = false;

            if (sarc == null)
            {
                return "return false;";
            }

            if (documents != null)
            {
                // fileSubString is used to get the correct SARC document if multiple SARC documents are associated with the case's group Id.
                // This happens when a case is overridden and recompleted or if a case is reinvestigated.
                string fileSubString = sarc.CaseId + "-Generated";

                foreach (Document docItem in documents)
                {
                    if (docItem.DocType == DocumentType.Form348R && docItem.OriginalFileName.Contains(fileSubString))
                    {
                        sarcFormDocId = docItem.Id.ToString();
                        isDoc = true;
                    }
                }
            }

            if (isDoc)
            {
                string docViewerURL = "../Shared/DocumentViewer.aspx?docID=" + sarcFormDocId + "&modId=" + ModuleType.SARC;
                strAttribute = "viewDoc('" + docViewerURL + "'); return false;";
            }
            else
            {
                strAttribute = "printForms('" + sarc.Id.ToString() + "', 'SARC'); return false;";
            }

            return strAttribute;
        }
    }
}
