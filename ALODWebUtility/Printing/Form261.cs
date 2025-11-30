using System;
using System.Text;
using System.Web.UI;
using ALOD.Core.Domain.Common;
using ALOD.Core.Domain.DBSign;
using ALOD.Core.Domain.Documents;
using ALOD.Core.Domain.Modules.Lod;
using ALOD.Core.Domain.Workflow;
using ALOD.Core.Interfaces.DAOInterfaces;
using ALOD.Core.Utils;
using ALOD.Data;
using ALOD.Data.Services;
using ALOD.Logging;
using ALODWebUtility.Common;
using ALODWebUtility.PrintingUtil;

namespace ALODWebUtility.Printing
{
    public class Form261 : Page, IDocumentCreate
    {
        protected static readonly DateTime EpochDate = new DateTime(2010, 1, 29);
        private const string BRANCH_AFRC = "AFRC";
        private const string BRANCH_ANG = "ANG";
        private const string DIGITAL_SIGNATURE_DATE_FORMAT = "yyyy.MM.dd HH:mm:ss zz'00'";
        private const string SIGNED_TEXT = "//SIGNED//";
        private ISignatueMetaDateDao _sigDao;
        private int lodid;
        private string remarksField = "";
        private bool replaceIOSig = false;
        private DBSignService signatureService;

        // Constants for formats that might be defined in Utility or elsewhere, assuming standard strings if not found
        private const string DATE_FORMAT = "ddMMMyyyy"; // Assuming standard format used in VB code
        private const string HOUR_FORMAT = "HHmm"; // Assuming standard format

        // Assuming SESSION_COMPO is available via session or utility
        private string SESSION_COMPO
        {
            get { return SessionInfo.Session_Compo; } // Using SessionInfo from Common
        }

        SignatureMetaDataDao SigDao
        {
            get
            {
                if (_sigDao == null)
                {
                    _sigDao = new NHibernateDaoFactory().GetSigMetaDataDao();
                }

                return (SignatureMetaDataDao)_sigDao;
            }
        }

        public DBSignService VerifySource
        {
            get
            {
                return signatureService;
            }
            set
            {
                signatureService = value;
            }
        }

        public PDFForm GeneratePDFForm(int refId, bool replaceIOsig)
        {
            this.replaceIOSig = replaceIOsig;
            string strComments = "Generated Form 261 PDF";
            NHibernateDaoFactory factory = new NHibernateDaoFactory();
            LineOfDutyDao dao = factory.GetLineOfDutyDao();
            LineOfDuty lod = new LineOfDuty();

            if (dao.GetWorkflow(refId) == 27)
            {
                lod = new LineOfDuty_v2();
            }

            lod = dao.GetById(refId);
            this.lodid = lod.Id; // Set class level lodid

            WorkStatusDao wsdao = new WorkStatusDao();
            ALOD.Core.Domain.Workflow.WorkStatus lodCurrStatus = wsdao.GetById(lod.Status);

            PDFForm form261 = new PDFForm(PrintDocuments.FormDD261);

            PrintingUtil.PrintingUtil.SetFormField(form261, "lodCaseNumberP1", lod.CaseId);
            PrintingUtil.PrintingUtil.SetFormField(form261, "lodCaseNumberP2", lod.CaseId);

            if (lod.Formal && lod.LODInvestigation != null)
            {
                if (lod.LODInvestigation.ReportDate != null)
                {
                    if (lod.LODInvestigation.ReportDate.HasValue)
                    {
                        PrintingUtil.PrintingUtil.SetFormField(form261, "REPORT_DATE", Server.HtmlDecode(lod.LODInvestigation.ReportDate.Value.ToString(DATE_FORMAT)));
                    }
                }
                if (lod.LODInvestigation.InvestigationOf != null)
                {
                    switch (lod.LODInvestigation.InvestigationOf)
                    {
                        case 4:
                        case 5:
                            PrintingUtil.PrintingUtil.SetFormField(form261, "invest_aa", "Yes");
                            break;
                        case 1:
                            PrintingUtil.PrintingUtil.SetFormField(form261, "invest_bb", "Yes");
                            break;
                        case 2:
                            PrintingUtil.PrintingUtil.SetFormField(form261, "invest_cc", "Yes");
                            break;
                        case 3:
                            PrintingUtil.PrintingUtil.SetFormField(form261, "invest_dd", "Yes");
                            break;
                    }
                }
                if (lod.LODInvestigation.Status != null)
                {
                    switch (lod.LODInvestigation.Status)
                    {
                        case 1:
                            PrintingUtil.PrintingUtil.SetFormField(form261, "status_aa", "Yes");
                            break;
                        case 2:
                            PrintingUtil.PrintingUtil.SetFormField(form261, "status_bb", "Yes");
                            PrintingUtil.PrintingUtil.SetFormField(form261, "status_b_aa", "Yes");
                            break;
                        case 3:
                            PrintingUtil.PrintingUtil.SetFormField(form261, "status_bb", "Yes");
                            PrintingUtil.PrintingUtil.SetFormField(form261, "status_b_bb", "Yes");
                            break;
                        case 4:
                            PrintingUtil.PrintingUtil.SetFormField(form261, "status_cc", "Yes");
                            PrintingUtil.PrintingUtil.SetFormField(form261, "status_ca", Server.HtmlDecode(lod.LODInvestigation.InactiveDutyTraining.Trim().ToString()));
                            break;
                        case 5:
                            PrintingUtil.PrintingUtil.SetFormField(form261, "status_dd", "Yes");
                            break;
                    }
                }

                if (lod.LODInvestigation.DurationStart != null)
                {
                    if (lod.LODInvestigation.DurationStart.HasValue)
                    {
                        PrintingUtil.PrintingUtil.SetFormField(form261, "startD", Server.HtmlDecode(lod.LODInvestigation.DurationStart.Value.ToString(DATE_FORMAT)));
                        PrintingUtil.PrintingUtil.SetFormField(form261, "startH", Server.HtmlDecode(lod.LODInvestigation.DurationStart.Value.ToString(HOUR_FORMAT)));
                    }
                }
                if (lod.LODInvestigation.DurationEnd != null)
                {
                    if (lod.LODInvestigation.DurationEnd.HasValue)
                    {
                        PrintingUtil.PrintingUtil.SetFormField(form261, "finishD", Server.HtmlDecode(lod.LODInvestigation.DurationEnd.Value.ToString(DATE_FORMAT)));
                        PrintingUtil.PrintingUtil.SetFormField(form261, "finishH", Server.HtmlDecode(lod.LODInvestigation.DurationEnd.Value.ToString(HOUR_FORMAT)));
                    }
                }

                // TODO: Make sure this hardcoded value is nothing to be changed
                if (lod.MemberCompo == "6")
                {
                    PrintingUtil.PrintingUtil.SetFormField(form261, "4_TO_Major_Army", "HQ AFRC/ACV");
                }
                else
                {
                    PrintingUtil.PrintingUtil.SetFormField(form261, "4_TO_Major_Army", "NGB/CV");
                }

                PrintingUtil.PrintingUtil.SetFormField(form261, "individual_name", lod.MemberName.ToUpper());
                PrintingUtil.PrintingUtil.SetFormField(form261, "smLastFirstMiddle", lod.MemberName.ToUpper());
                PrintingUtil.PrintingUtil.SetFormField(form261, "individual_ssn", Utility.FormatSSN(lod.MemberSSN, false));
                PrintingUtil.PrintingUtil.SetFormField(form261, "smSSN", Utility.FormatSSN(lod.MemberSSN, false));
                PrintingUtil.PrintingUtil.SetFormField(form261, "individual_grade", lod.MemberGrade);
                PrintingUtil.PrintingUtil.SetFormField(form261, "smGrade", lod.MemberGrade);
                PrintingUtil.PrintingUtil.SetFormField(form261, "organization_station", lod.MemberUnit);

                if (lod.LODInvestigation.OtherPersonnel != null)
                {
                    if (lod.LODInvestigation.OtherPersonnel.Count != 0)
                    {
                        if (lod.LODInvestigation.OtherPersonnel[0] != null)
                        {
                            PrintingUtil.PrintingUtil.SetFormField(form261, "txtName_1", Server.HtmlDecode(lod.LODInvestigation.OtherPersonnel[0].Name));
                            PrintingUtil.PrintingUtil.SetFormField(form261, "txtGrade_1", lod.LODInvestigation.OtherPersonnel[0].Grade);

                            if (lod.LODInvestigation.OtherPersonnel[0].InvestigationMade == true)
                            {
                                PrintingUtil.PrintingUtil.SetFormField(form261, "lodMadeYes_1", "Yes");
                            }
                            else
                            {
                                PrintingUtil.PrintingUtil.SetFormField(form261, "lodMadeYes_1", "No");
                            }
                        }
                        if (lod.LODInvestigation.OtherPersonnel.Count > 1)
                        {
                            if (lod.LODInvestigation.OtherPersonnel[1] != null)
                            {
                                PrintingUtil.PrintingUtil.SetFormField(form261, "txtName_2", Server.HtmlDecode(lod.LODInvestigation.OtherPersonnel[1].Name));
                                PrintingUtil.PrintingUtil.SetFormField(form261, "txtGrade_2", lod.LODInvestigation.OtherPersonnel[1].Grade);

                                if (lod.LODInvestigation.OtherPersonnel[1].InvestigationMade == true)
                                {
                                    PrintingUtil.PrintingUtil.SetFormField(form261, "lodMadeYes_2", "Yes");
                                }
                                else
                                {
                                    PrintingUtil.PrintingUtil.SetFormField(form261, "lodMadeYes_2", "No");
                                }
                            }
                        }
                        if (lod.LODInvestigation.OtherPersonnel.Count > 2)
                        {
                            if (lod.LODInvestigation.OtherPersonnel[2] != null)
                            {
                                PrintingUtil.PrintingUtil.SetFormField(form261, "txtName_3", Server.HtmlDecode(lod.LODInvestigation.OtherPersonnel[2].Name));
                                PrintingUtil.PrintingUtil.SetFormField(form261, "txtGrade_3", lod.LODInvestigation.OtherPersonnel[2].Grade);

                                if (lod.LODInvestigation.OtherPersonnel[2].InvestigationMade == true)
                                {
                                    PrintingUtil.PrintingUtil.SetFormField(form261, "lodMadeYes_3", "Yes");
                                }
                                else
                                {
                                    PrintingUtil.PrintingUtil.SetFormField(form261, "lodMadeYes_3", "No");
                                }
                            }
                        }
                    }
                }

                // Basis for findings
                if (lod.LODInvestigation.FindingsDate != null)
                {
                    if (lod.LODInvestigation.FindingsDate.HasValue)
                    {
                        if (lod.LODInvestigation.FindingsDate.HasValue)
                        {
                            PrintingUtil.PrintingUtil.SetFormField(form261, "2DATE_YYMMDD", Server.HtmlDecode(lod.LODInvestigation.FindingsDate.Value.ToString(DATE_FORMAT)));
                            PrintingUtil.PrintingUtil.SetFormField(form261, "1HOUR", Server.HtmlDecode(lod.LODInvestigation.FindingsDate.Value.ToString(HOUR_FORMAT)));
                        }
                    }
                }
                PrintingUtil.PrintingUtil.SetFormField(form261, "3PLACE", Server.HtmlDecode(lod.LODInvestigation.Place));

                string[] ary = PrintingUtil.PrintingUtil.SplitString(Server.HtmlDecode(lod.LODInvestigation.HowSustained), 75);

                PrintingUtil.PrintingUtil.SetFormField(form261, "4HOW_SUSTAINED1", ary[0]);
                PrintingUtil.PrintingUtil.SetFormField(form261, "4HOW_SUSTAINED2", ary[1]);

                ary = null;

                ary = PrintingUtil.PrintingUtil.SplitString(Server.HtmlDecode(lod.LODInvestigation.MedicalDiagnosis), 73);

                PrintingUtil.PrintingUtil.SetFormField(form261, "bMEDICAL_DIAGNOSIS1", ary[0]);
                PrintingUtil.PrintingUtil.SetFormField(form261, "bMEDICAL_DIAGNOSIS2", ary[1]);
                ary = null;

                if (lod.LODInvestigation.PresentForDuty != null)
                {
                    switch (lod.LODInvestigation.PresentForDuty)
                    {
                        case true: PrintingUtil.PrintingUtil.SetFormField(form261, "Comb1a", "Yes"); break;
                        case false: PrintingUtil.PrintingUtil.SetFormField(form261, "Comb1b", "No"); break;
                    }
                }
                if (lod.LODInvestigation.AbsentWithAuthority != null)
                {
                    switch (lod.LODInvestigation.AbsentWithAuthority)
                    {
                        case true: PrintingUtil.PrintingUtil.SetFormField(form261, "Comb2a", "Yes"); break;
                        case false: PrintingUtil.PrintingUtil.SetFormField(form261, "Comb2b", "No"); break;
                    }
                }
                if (lod.LODInvestigation.IntentionalMisconduct != null)
                {
                    switch (lod.LODInvestigation.IntentionalMisconduct)
                    {
                        case true: PrintingUtil.PrintingUtil.SetFormField(form261, "Comb3a", "Yes"); break;
                        case false: PrintingUtil.PrintingUtil.SetFormField(form261, "Comb3b", "No"); break;
                    }
                }
                if (lod.LODInvestigation.MentallySound != null)
                {
                    switch (lod.LODInvestigation.MentallySound)
                    {
                        case true: PrintingUtil.PrintingUtil.SetFormField(form261, "Comb4a", "Yes"); break;
                        case false: PrintingUtil.PrintingUtil.SetFormField(form261, "Comb4b", "No"); break;
                    }
                }

                if (!string.IsNullOrEmpty(lod.LODInvestigation.Remarks))
                {
                    ary = PrintingUtil.PrintingUtil.SplitString(Server.HtmlDecode(lod.LODInvestigation.Remarks), 80);

                    if (ary != null)
                    {
                        if (ary.Length > 0)
                        {
                            PrintingUtil.PrintingUtil.SetFormField(form261, "g_REMARKS1", ary[0]);
                        }
                        if (ary.Length > 1)
                        {
                            PrintingUtil.PrintingUtil.SetFormField(form261, "g_REMARKS2", ary[1]);
                        }
                    }
                    ary = null;
                }

                LineOfDutyFindings ioFinding;
                ioFinding = lod.FindByType(PersonnelTypes.IO);

                if (ioFinding != null)
                {
                    if (ioFinding.Finding != null)
                    {
                        switch (ioFinding.Finding)
                        {
                            case Finding.In_Line_Of_Duty: PrintingUtil.PrintingUtil.SetFormField(form261, "findinga", "Yes"); break; // ILD
                            case Finding.Nlod_Not_Due_To_OwnMisconduct: PrintingUtil.PrintingUtil.SetFormField(form261, "findingb", "Yes"); break; // NDOM
                            case Finding.Nlod_Due_To_Own_Misconduct: PrintingUtil.PrintingUtil.SetFormField(form261, "findingc", "Yes"); break; // DOM
                        }
                    }
                }

                if (lod.LODInvestigation.IsSignedByIO)
                {
                    AddFormalSignatureToForm_v2(form261, lod.LODInvestigation.SignatureInfoIO,
                    lod.LODInvestigation.DateSignedIO,
                    "", "fSIGNATURE", "aTYPED_NAME_Last_First_M",
                    "bGRADE", "cBRANCH_OF_SERVICE", "eORGANIZATION_AND_STATIO",
                    DBSignTemplateId.Form261, PersonnelTypes.IO);
                }

                // Appointing Authority ---------------------------------------------------------------------

                LineOfDutyFindings appointingFormalFinding;
                appointingFormalFinding = lod.FindByType(PersonnelTypes.FORMAL_APP_AUTH);

                if (appointingFormalFinding != null)
                {
                    AddFormalFinding_v2(form261, appointingFormalFinding, "appointingSubstitutedFindings", "appointingFindings");

                    switch (appointingFormalFinding.DecisionYN)
                    {
                        case "Y": PrintingUtil.PrintingUtil.SetFormField(form261, "a_approve_yes_a", "Yes"); break;
                        case "N": PrintingUtil.PrintingUtil.SetFormField(form261, "a_approve_no_a", "No"); break;
                    }
                }

                if (lod.LODInvestigation != null)
                {
                    if (lod.LODInvestigation.IsSignedByAppointingAuthority)
                    {
                        AddFormalSignatureToForm_v2(form261, lod.LODInvestigation.SignatureInfoAppointing,
                        lod.LODInvestigation.DateSignedAppointing,
                        "a_date_a", "a_signature_a", "a_name_a",
                        "a_grade_a", "a_station_a", "a_headquarter_a",
                        DBSignTemplateId.Form348Findings, PersonnelTypes.FORMAL_APP_AUTH);
                    }

                    if (lodCurrStatus.StatusCodeType.IsFinal)
                    {
                        // Final/Approving Authority ----------------------------------------------------------------------

                        SignatureMetaData sig = SigDao.GetByWorkStatus(lod.Id, lod.Workflow, LodWorkStatus_v2.FormalApprovingAuthorityAction);

                        if (sig != null)
                        {
                            if (lod.LODInvestigation.FinalApprovalFindings != null)
                            {
                                switch (lod.LODInvestigation.FinalApprovalFindings)
                                {
                                    case Finding.In_Line_Of_Duty:
                                        PrintingUtil.PrintingUtil.SetFormField(form261, "final_approval_findings", "In Line of Duty");
                                        break;
                                    case Finding.Epts_Service_Aggravated:
                                        PrintingUtil.PrintingUtil.SetFormField(form261, "final_approval_findings", "EPTS-Service Aggravated");
                                        break;
                                    case Finding.Nlod_Due_To_Own_Misconduct:
                                        PrintingUtil.PrintingUtil.SetFormField(form261, "final_approval_findings", "Not ILOD-Due to Own Misconduct");
                                        break;
                                    case Finding.Epts_Lod_Not_Applicable:
                                        PrintingUtil.PrintingUtil.SetFormField(form261, "final_approval_findings", "EPTS-LOD Not Applicable");
                                        break;
                                    case Finding.Nlod_Not_Due_To_OwnMisconduct:
                                        PrintingUtil.PrintingUtil.SetFormField(form261, "final_approval_findings", "Not ILOD-Not Due to Own Misconduct");
                                        break;
                                    case Finding.Recommend_Formal_Investigation:
                                        PrintingUtil.PrintingUtil.SetFormField(form261, "final_approval_findings", "Formal Investigation");
                                        break;
                                }

                                AddSignatureInformationToForm(lodid, form261, sig,
                                            "approval_date", "finalapprovalsig",
                                            "finalNameRank",
                                        DBSignTemplateId.Form348Findings,
                                        PersonnelTypes.FORMAL_BOARD_AA);
                            }
                        }
                        // need to determine how much of the endless findings field will go here
                        LineOfDutyFindings approvingFormalFinding;
                        approvingFormalFinding = lod.FindByType(PersonnelTypes.FORMAL_BOARD_AA);
                        if (approvingFormalFinding != null)
                        {
                            AddFormalFinding_v2(form261, approvingFormalFinding, "approvingSubstitutedFindings", "approvingFindings");
                        }
                    }
                }

                if (lod.CurrentStatusCode != LodStatusCode.Complete)
                {
                    // Suppress the page
                    form261.SuppressSecondPage();
                }
            }

            LogManager.LogAction(ModuleType.LOD, UserAction.ViewDocument, lodid, strComments);

            return form261;
        }

        protected bool AddFormalFinding_v2(PDFForm doc, LineOfDutyFindings boardFinding, string concurField, string findingField)
        {
            if (boardFinding == null)
            {
                PrintingUtil.PrintingUtil.SetFormField(doc, concurField, string.Empty);
                PrintingUtil.PrintingUtil.SetFormField(doc, findingField, string.Empty);
                return false;
            }

            string concurText = "";
            string newFinding = "";
            bool valid = false;

            if (boardFinding.DecisionYN == "Y")
            {
                concurText = "";
                valid = true;
            }
            else
            {
                concurText = "Substituted Findings: ";

                if (boardFinding.Finding.HasValue)
                {
                    valid = true;
                    newFinding = PrintingUtil.PrintingUtil.GetFindingFormText(boardFinding.Finding.Value);
                }
            }

            PrintingUtil.PrintingUtil.SetFormField(doc, concurField, concurText + newFinding);
            PrintingUtil.PrintingUtil.SetFormField(doc, findingField, Server.HtmlDecode(boardFinding.FindingsText));

            return valid;
        }

        protected bool AddFormalSignatureToForm_v2(PDFForm doc, PersonnelData signature, DateTime? dateSigned, string dateField, string sigField, string nameField, string rankField, string branchField, string unitField, DBSignTemplateId template, PersonnelTypes ptype)
        {
            if (signature == null)
            {
                PrintingUtil.PrintingUtil.SetFormField(doc, dateField, string.Empty);
                PrintingUtil.PrintingUtil.SetFormField(doc, nameField, string.Empty);
                PrintingUtil.PrintingUtil.SetFormField(doc, sigField, string.Empty);
                PrintingUtil.PrintingUtil.SetFormField(doc, rankField, string.Empty);
                PrintingUtil.PrintingUtil.SetFormField(doc, branchField, string.Empty);
                PrintingUtil.PrintingUtil.SetFormField(doc, unitField, string.Empty);
                return false;
            }

            PrintingUtil.PrintingUtil.SetFormField(doc, nameField, signature.Name.ToUpper());
            PrintingUtil.PrintingUtil.SetFormField(doc, rankField, signature.Grade);

            PrintingUtil.PrintingUtil.SetFormField(doc, unitField, signature.PasCodeDescription);

            if (!string.IsNullOrEmpty(signature.Branch))
            {
                PrintingUtil.PrintingUtil.SetFormField(doc, branchField, signature.Branch);
            }
            else
            {
                if (SESSION_COMPO == "6")
                {
                    PrintingUtil.PrintingUtil.SetFormField(doc, branchField, BRANCH_AFRC);
                }
                else
                {
                    PrintingUtil.PrintingUtil.SetFormField(doc, branchField, BRANCH_ANG);
                }
            }

            if (dateSigned == null || !dateSigned.HasValue)
            {
                // no signature date, so don't add the signature, meaning we're done
                return false;
            }

            if (dateSigned.Value < EpochDate)
            {
                // use the old style signature
                PrintingUtil.PrintingUtil.SetFormField(doc, sigField, SIGNED_TEXT);

                // use the passed in date
                PrintingUtil.PrintingUtil.SetFormField(doc, dateField, dateSigned.Value.ToString(DATE_FORMAT));

                // and we're done
                return true;
            }

            // this signature occured after the epoch, so verify it
            VerifySource = new DBSignService(template, lodid, ptype);

            bool valid = false;

            DBSignResult signatureStatus = VerifySource.VerifySignature();

            if (signatureStatus == DBSignResult.SignatureValid)
            {
                // if it's valid, add the signing info to the form
                DigitalSignatureInfo signInfo = VerifySource.GetSignerInfo();

                string sigLine = "Digitally signed by "
                    + signInfo.Signature + Environment.NewLine
                    + "Date: " + signInfo.DateSigned.ToString(DIGITAL_SIGNATURE_DATE_FORMAT);

                PrintingUtil.PrintingUtil.SetFormField(doc, sigField, sigLine);
                PrintingUtil.PrintingUtil.SetFormField(doc, dateField, signInfo.DateSigned.ToString(DATE_FORMAT));
                valid = true;
            }
            else
            {
                // Check if the electronic signature should be used for the IO signature field
                if (ptype == PersonnelTypes.IO && (replaceIOSig == true || dateSigned.Value < Utility.ARCHIVE_DATE))
                {
                    // use the old style signature
                    PrintingUtil.PrintingUtil.SetFormField(doc, sigField, SIGNED_TEXT);

                    // use the passed in date
                    PrintingUtil.PrintingUtil.SetFormField(doc, dateField, dateSigned.Value.ToString(DATE_FORMAT));

                    valid = true;
                    replaceIOSig = false; // Reset flag
                }
                else
                {
                    // otherwise, clear those fields
                    PrintingUtil.PrintingUtil.SetFormField(doc, sigField, SIGNED_TEXT);
                    // SetFormField(doc, dateField, signature.DateSigned.ToString());
                    valid = false;
                }
            }

            return valid;
        }

        protected bool AddSignatureInformationToForm(int refId, PDFForm doc, SignatureMetaData signature, string dateField, string sigField, string nameField, DBSignTemplateId template, PersonnelTypes ptype)
        {
            if (signature == null)
            {
                PrintingUtil.PrintingUtil.SetFormField(doc, dateField, string.Empty);
                PrintingUtil.PrintingUtil.SetFormField(doc, nameField, string.Empty);
                PrintingUtil.PrintingUtil.SetFormField(doc, sigField, string.Empty);
                return false;
            }

            string strSig = "USAF";
            if (SESSION_COMPO == "5")
            {
                strSig = "ANG";
            }

            PrintingUtil.PrintingUtil.SetFormField(doc, nameField, signature.NameAndRank + ", " + strSig);

            DateTime dateSigned = signature.date;

            VerifySource = new DBSignService(template, refId, ptype);

            bool valid = false;

            DBSignResult signatureStatus = VerifySource.VerifySignature();

            if (signatureStatus == DBSignResult.SignatureValid)
            {
                DigitalSignatureInfo signInfo = VerifySource.GetSignerInfo();

                string sigLine = "Digitally signed by "
                    + signInfo.Signature + Environment.NewLine
                    + "Date: " + signInfo.DateSigned.ToString(DIGITAL_SIGNATURE_DATE_FORMAT);

                PrintingUtil.PrintingUtil.SetFormField(doc, sigField, sigLine);
                PrintingUtil.PrintingUtil.SetFormField(doc, dateField, signInfo.DateSigned.ToString("ddMMMyyyy"));
                valid = true;
            }
            else
            {
                // Use electronic signature
                PrintingUtil.PrintingUtil.SetFormField(doc, sigField, SIGNED_TEXT);
                PrintingUtil.PrintingUtil.SetFormField(doc, dateField, signature.date.ToString("ddMMMyyyy"));
                valid = false;
            }

            return valid;
        }
    }
}
