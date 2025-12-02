using System;
using System.Linq;
using System.Web.UI;
using ALOD.Core.Domain.Common;
using ALOD.Core.Domain.DBSign;
using ALOD.Core.Domain.Documents;
using ALOD.Core.Domain.Lookup;
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
    public class Form348 : Page, IDocumentCreate
    {
        protected static readonly DateTime EpochDate = new DateTime(2010, 1, 29);
        private const string DIGITAL_SIGNATURE_DATE_FORMAT = "yyyy.MM.dd HH:mm:ss zz'00'";
        private const string SIGNED_TEXT = "//SIGNED//";
        private ISignatueMetaDateDao _sigDao;
        private WorkStatusDao _wsdao;
        private Finding concurredFinding;
        private int lodid;
        private DBSignService signatureService;

        // Constants for formats that might be defined in Utility or elsewhere, assuming standard strings if not found
        private const string DATE_FORMAT = "ddMMMyyyy"; // Assuming standard format used in VB code
        private const string DATE_HOUR_FORMAT = "ddMMMyyyy HHmm"; // Assuming standard format

        // Assuming SESSION_COMPO is available via session or utility
        private string SESSION_COMPO
        {
            get { return SessionInfo.SESSION_COMPO; } // Using SessionInfo from Common
        }

        // Assuming PERMISSION_VIEW_SARC_CASES is available via session or utility
        private string PERMISSION_VIEW_SARC_CASES
        {
            get { return SessionInfo.PERMISSION_VIEW_SARC_CASES; } // Using SessionInfo constant
        }

        // Helper method to check permissions
        private bool UserHasPermission(string permission)
        {
            return SessionInfo.UserHasPermission(permission);
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

        protected WorkStatusDao wsdao
        {
            get
            {
                if (_wsdao == null)
                {
                    _wsdao = new WorkStatusDao();
                }

                return _wsdao;
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
            string strComments = "Generated Form 348 PDF";
            LineOfDuty lod = LodService.GetById(refId);
            lodid = lod.Id;
            bool isRLod = false;

            if (lod.SarcCase && !UserHasPermission(PERMISSION_VIEW_SARC_CASES))
            {
                if (lod.IsRestricted || (!lod.IsRestricted && !UserHasPermission("SARCUnrestricted")))
                {
                    throw new Exception("User is attempting to view SARC PDF without permission refId:" + lod.Id.ToString());
                }
            }

            PDFForm form348 = new PDFForm((int)PrintDocuments.FormARFC348);

            PrintingUtil.PrintingUtil.SetFormField(form348, "lodCaseNumberP1", lod.CaseId);
            PrintingUtil.PrintingUtil.SetFormField(form348, "lodCaseNumberP2", lod.CaseId);

            // set member/unit info
            PrintingUtil.PrintingUtil.SetFormField(form348, "to", lod.ToUnit);
            PrintingUtil.PrintingUtil.SetFormField(form348, "from", lod.FromUnit);
            PrintingUtil.PrintingUtil.SetFormField(form348, "memberName", lod.MemberName);
            PrintingUtil.PrintingUtil.SetFormField(form348, "memberSSN", Utility.FormatSSN(lod.MemberSSN));
            PrintingUtil.PrintingUtil.SetFormField(form348, "memberGrade", lod.MemberGrade);
            PrintingUtil.PrintingUtil.SetFormField(form348, "memberUnit", lod.MemberUnit);

            SetMedicalInfo(form348, lod);

            SetUnitInfo(form348, lod);

            SetWingJudgeAdvocateInfo(form348, lod);

            SetWingCommanderInfo(form348, lod);

            // *************************************
            // The board section only gets added
            // to the 348 for informal cases
            // *************************************

            // *************************************
            // 'If form 348 is not "Complete", we suppress the whole second page, Print-Out/PDF
            // '*************************************
            if (lod.WorkflowStatus.Id != (int)LodWorkStatus.Complete)
            {
                // Suppress the page
                form348.SuppressSecondPage();
            }
            else
            {
                //    'Continue normal execution

                if (lod.Formal)
                {
                    // this is a formal case, so add the 261 text
                    PrintingUtil.PrintingUtil.SetFormField(form348, "medicalReview", "(See DD Form 261 for Formal investigation )");
                }
                else
                {

                    SetBoardMedicalInfo(form348, lod);

                    SetBoardLegalInfo(form348, lod);

                    SetBoardTechInfo(form348, lod);

                    SetBoardApprovalInfo(form348, lod);

                    if (PrintingUtil.PrintingUtil.CheckForPriorStatus((short)lod.CurrentStatusCode, refId))
                    {
                        PrintingUtil.PrintingUtil.SetFormField(form348, "medicalReview", "This Block Not Used");
                        PrintingUtil.PrintingUtil.SetFormField(form348, "legalReview", "This Block Not Used");
                    }

                }

            }

            LogManager.LogAction((int)ModuleType.LOD, UserAction.ViewDocument, lodid, strComments);

            return form348;
        }

        protected bool AddBoardFinding(PDFForm doc, LineOfDutyFindings boardFinding, string concurField, string findingField)
        {
            if (boardFinding == null)
            {
                PrintingUtil.PrintingUtil.SetFormField(doc, concurField, string.Empty);
                PrintingUtil.PrintingUtil.SetFormField(doc, findingField, string.Empty);
                return false;
            }

            string concurText = "";
            string newFinding = "";

            if (boardFinding.DecisionYN == "Y")
            {
                concurText = "Concur with Appointing Authority";

                if (findingField == "medicalReview")
                {
                    string message = "Based on current authoritative medical literature combined with review of the provided medical records, the following conclusion assessing the pertinent injury/disease, pre-existing conditions and contributory factors for their  pathophysiology and prognosis, as related to causation, the following determination was found in this case: ";

                    PrintingUtil.PrintingUtil.SetFormField(doc, concurField, message + Environment.NewLine + concurText + newFinding);
                    PrintingUtil.PrintingUtil.SetFormField(doc, findingField, Server.HtmlDecode(boardFinding.FindingsText));
                    return true;
                }
                else
                {
                    PrintingUtil.PrintingUtil.SetFormField(doc, concurField, concurText + newFinding);
                    PrintingUtil.PrintingUtil.SetFormField(doc, findingField, Server.HtmlDecode(boardFinding.FindingsText));
                    return true;
                }
            }
            else
            {
                concurText = "Non Concur with Appointing Authority. Recommended new finding: ";

                if (boardFinding.Finding.HasValue)
                {
                    newFinding = PrintingUtil.PrintingUtil.GetFindingFormText((Finding)boardFinding.Finding.Value);

                    if (findingField == "medicalReview")
                    {
                        string message = "Based on current authoritative medical literature combined with review of the provided medical records, the following conclusion assessing the pertinent injury/disease, pre-existing conditions and contributory factors for their  pathophysiology and prognosis, as related to causation, the following determination was found in this case: ";

                        PrintingUtil.PrintingUtil.SetFormField(doc, concurField, message + Environment.NewLine + concurText + newFinding);
                        PrintingUtil.PrintingUtil.SetFormField(doc, findingField, Server.HtmlDecode(boardFinding.FindingsText));
                        return true;
                    }
                    else
                    {
                        PrintingUtil.PrintingUtil.SetFormField(doc, concurField, concurText + newFinding);
                        PrintingUtil.PrintingUtil.SetFormField(doc, findingField, Server.HtmlDecode(boardFinding.FindingsText));
                        return true;
                    }
                }
                else
                {
                    return false;
                }
            }
        }

        protected bool AddSignatureToForm(PDFForm doc, SignatureMetaData signature, string dateField, string sigField, string nameField, string titleField, DBSignTemplateId template, PersonnelTypes ptype)
        {
            if (signature == null)
            {
                PrintingUtil.PrintingUtil.SetFormField(doc, dateField, string.Empty);
                PrintingUtil.PrintingUtil.SetFormField(doc, nameField, string.Empty);
                PrintingUtil.PrintingUtil.SetFormField(doc, sigField, string.Empty);
                PrintingUtil.PrintingUtil.SetFormField(doc, titleField, string.Empty);
                return false;
            }

            PrintingUtil.PrintingUtil.SetFormField(doc, nameField, signature.NameAndRank);

            DateTime dateSigned = signature.date;

            if (dateSigned < EpochDate)
            {

                // use the old style signature
                PrintingUtil.PrintingUtil.SetFormField(doc, sigField, SIGNED_TEXT);

                // use the passed in date
                PrintingUtil.PrintingUtil.SetFormField(doc, dateField, dateSigned.ToString(DATE_FORMAT));

                // and we're done
                return true;

            }

            // this signature occured after the epoch, so verify it
            VerifySource = new DBSignService(template, lodid, (int)ptype);

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
                // otherwise, clear those fields
                PrintingUtil.PrintingUtil.SetFormField(doc, sigField, SIGNED_TEXT);
                PrintingUtil.PrintingUtil.SetFormField(doc, dateField, signature.date.ToString());
                valid = false;
            }

            // finally set the title field
            PrintingUtil.PrintingUtil.SetFormField(doc, titleField, signature.Title);

            return valid;
        }

        private void SetBoardApprovalInfo(PDFForm form348, LineOfDuty lod)
        {

            ALOD.Core.Domain.Workflow.WorkStatus lodCurrStatus = wsdao.GetById(lod.Status);

            if (lodCurrStatus.StatusCodeType.IsFinal)
            {

                SignatureMetaData sig = SigDao.GetByWorkStatus(lod.Id, lod.Workflow, (int)LodWorkStatus.ApprovingAuthorityAction);

                if (sig != null)
                {

                    LineOfDutyFindings approvingfinding;

                    if (lod.Formal)
                    {
                        approvingfinding = lod.FindByType((short)PersonnelTypes.FORMAL_APP_AUTH);
                    }
                    else
                    {
                        approvingfinding = lod.FindByType((short)PersonnelTypes.BOARD_AA);
                    }

                    if (approvingfinding != null)
                    {

                        if (approvingfinding.Finding != null)
                        {
                            switch ((Finding)approvingfinding.Finding)
                            {
                                case Finding.In_Line_Of_Duty:
                                    PrintingUtil.PrintingUtil.SetFormField(form348, "approvingILOD", "Yes");
                                    break;
                                case Finding.Recommend_Formal_Investigation:
                                    PrintingUtil.PrintingUtil.SetFormField(form348, "approvingRecommendFormal", "Yes");
                                    break;
                                case Finding.Epts_Service_Aggravated:
                                    PrintingUtil.PrintingUtil.SetFormField(form348, "approvingEptsServiceAggravated", "Yes");
                                    break;
                                case Finding.Nlod_Due_To_Own_Misconduct:
                                    PrintingUtil.PrintingUtil.SetFormField(form348, "approvingNotILodDom", "Yes");
                                    break;
                                case Finding.Epts_Lod_Not_Applicable:
                                    PrintingUtil.PrintingUtil.SetFormField(form348, "approvingEptsNotApplicable", "Yes");
                                    break;
                                case Finding.Nlod_Not_Due_To_OwnMisconduct:
                                    PrintingUtil.PrintingUtil.SetFormField(form348, "approvingNotILodNotDom", "Yes");
                                    break;
                            }

                        }

                        string approver = "";
                        // approval authority is slightly different

                        // we need to know if the board signed for the general
                        if (lod.BoardForGeneral == "Y")
                        {

                            SignatureMetaData Techsig = SigDao.GetByWorkStatus(lod.Id, lod.Workflow, (int)LodWorkStatus.BoardReview);

                            // add the board members name to the signature
                            if (Techsig != null)
                            {
                                approver += Techsig.NameAndRank + " for " + Environment.NewLine;
                            }
                        }
                        else
                        {

                            // if the General signed it, add their signature
                            AddSignatureToForm(form348, sig,
                                                        "approvingDate", "approvingSignature",
                                                        "approvingRankName", "",
                                                        DBSignTemplateId.Form348Findings,
                                                        PersonnelTypes.BOARD_AA);
                        }

                        approver += sig.NameAndRank
                                    + Environment.NewLine + sig.Title;

                        PrintingUtil.PrintingUtil.SetFormField(form348, "approvingRankName", approver);

                    }
                }

            }
        }

        private void SetBoardLegalInfo(PDFForm form348, LineOfDuty lod)
        {

            SignatureMetaData sig = SigDao.GetByWorkStatus(lod.Id, lod.Workflow, (int)LodWorkStatus.BoardLegalReview);

            if (sig != null)
            {

                LineOfDutyFindings legalfinding = lod.FindByType((short)PersonnelTypes.BOARD_JA);
                if (AddBoardFinding(form348, legalfinding, "legalSubstitutedFindings", "legalReview"))
                {

                    AddSignatureToForm(form348, sig,
                                                    "legalReviewDate", "legalReviewSignature",
                                                    "legalReviewName", "legalReviewRank",
                                                DBSignTemplateId.Form348Findings,
                                                PersonnelTypes.BOARD_JA);
                }

            }
        }

        private void SetBoardMedicalInfo(PDFForm form348, LineOfDuty lod)
        {

            SignatureMetaData sig = SigDao.GetByWorkStatus(lod.Id, lod.Workflow, (int)LodWorkStatus.BoardMedicalReview);

            if (sig != null)
            {

                LineOfDutyFindings medicalfinding = lod.FindByType((short)PersonnelTypes.BOARD_SG);
                if (AddBoardFinding(form348, medicalfinding, "medicalSubstitutedFindings", "medicalReview"))
                {

                    if (!AddSignatureToForm(form348, sig,
                                                    "medicalReviewlDate", "medicalReviewSignature",
                                                    "medicalReviewName", "medicalReviewRank",
                                                    DBSignTemplateId.Form348Findings,
                                                    PersonnelTypes.BOARD_SG))
                    {

                        // we failed to add a signature
                        // either it wasn't signed or the signature is not valid
                        // either way, clear the findings fields

                        PrintingUtil.PrintingUtil.ClearFormField(form348, "medicalSubstitutedFindings");
                        PrintingUtil.PrintingUtil.ClearFormField(form348, "medicalReview");
                        PrintingUtil.PrintingUtil.ClearFormField(form348, "medicalReviewlDate");
                        PrintingUtil.PrintingUtil.ClearFormField(form348, "medicalReviewSignature");
                        PrintingUtil.PrintingUtil.ClearFormField(form348, "medicalReviewName");
                        PrintingUtil.PrintingUtil.ClearFormField(form348, "medicalReviewRank");

                    }
                }

            }
        }

        private void SetBoardTechInfo(PDFForm form348, LineOfDuty lod)
        {

            ALOD.Core.Domain.Workflow.WorkStatus lodCurrStatus = wsdao.GetById(lod.Status);

            if (lodCurrStatus.StatusCodeType.IsFinal)
            {

                if (lod.FinalFindings != null && lod.FinalFindings.HasValue)
                {

                    SignatureMetaData sig = SigDao.GetByWorkStatus(lod.Id, lod.Workflow, (int)LodWorkStatus.BoardReview);

                    if (sig != null)
                    {

                        switch ((Finding)lod.FinalFindings.Value)
                        {
                            case Finding.In_Line_Of_Duty:
                                PrintingUtil.PrintingUtil.SetFormField(form348, "boardILod", "Yes");
                                break;
                            case Finding.Recommend_Formal_Investigation:
                                PrintingUtil.PrintingUtil.SetFormField(form348, "boardRecommendFormal", "Yes");
                                break;
                            case Finding.Epts_Service_Aggravated:
                                PrintingUtil.PrintingUtil.SetFormField(form348, "boardEptsServiceAggravated", "Yes");
                                break;
                        }

                        AddSignatureToForm(form348, sig,
                                        "boardReviewDate", "boardReviewSignature",
                                        "boardReviewRankName", "boardReviewRank",
                                        DBSignTemplateId.Form348Findings,
                                        PersonnelTypes.BOARD);

                        // if the board signed for the general, add the board findings and signature for the AA as well
                        if (lod.BoardForGeneral == "Y")
                        {

                            switch ((Finding)lod.FinalFindings.Value)
                            {
                                case Finding.In_Line_Of_Duty:
                                    PrintingUtil.PrintingUtil.SetFormField(form348, "approvingILOD", "Yes");
                                    break;
                                case Finding.Recommend_Formal_Investigation:
                                    PrintingUtil.PrintingUtil.SetFormField(form348, "approvingRecommendFormal", "Yes");
                                    break;
                                case Finding.Epts_Service_Aggravated:
                                    PrintingUtil.PrintingUtil.SetFormField(form348, "approvingEptsServiceAggravated", "Yes");
                                    break;
                                case Finding.Nlod_Due_To_Own_Misconduct:
                                    PrintingUtil.PrintingUtil.SetFormField(form348, "approvingNotILodDom", "Yes");
                                    break;
                                case Finding.Epts_Lod_Not_Applicable:
                                    PrintingUtil.PrintingUtil.SetFormField(form348, "approvingEptsNotApplicable", "Yes");
                                    break;
                                case Finding.Nlod_Not_Due_To_OwnMisconduct:
                                    PrintingUtil.PrintingUtil.SetFormField(form348, "approvingNotILodNotDom", "Yes");
                                    break;
                            }

                            SignatureMetaData Appsig = SigDao.GetByWorkStatus(lod.Id, lod.Workflow, (int)LodWorkStatus.ApprovingAuthorityAction);

                            AddSignatureToForm(form348, Appsig,
                                        "approvingDate", "approvingSignature",
                                        "approvingRankName", "",
                                        DBSignTemplateId.Form348Findings,
                                        PersonnelTypes.BOARD);

                            string approver = "";

                            approver += sig.NameAndRank + " for " + Environment.NewLine;

                            // Check if the Approval Authority actually signed the case...
                            if (Appsig != null)
                            {
                                // Use the approval authority signature information stored in the case record...
                                approver += Appsig.NameAndRank
                                            + Environment.NewLine + Appsig.Title;
                            }
                            else
                            {
                                // Use the Approval Authority selected by the Board Tech...
                                ALOD.Core.Domain.Users.AppUser approvalAuthority = null;
                                string title = string.Empty;

                                if (lod.ApprovingAuthorityUserId.HasValue)
                                {
                                    approvalAuthority = UserService.GetById(lod.ApprovingAuthorityUserId.Value);
                                }

                                if (approvalAuthority != null)
                                {
                                    title = UserService.GetUserAlternateTitle(approvalAuthority.Id, (int)ALOD.Core.Domain.Users.UserGroups.BoardApprovalAuthority);

                                    string strSig = "USAF";
                                    if (SESSION_COMPO == "5")
                                    {
                                        strSig = "ANG";
                                    }

                                    if (!string.IsNullOrEmpty(approvalAuthority.AlternateSignatureName))
                                    {
                                        approver += approvalAuthority.AlternateSignatureName + ", " + strSig + Environment.NewLine + title;
                                    }
                                    else
                                    {
                                        approver += approvalAuthority.SignatureName + ", " + strSig + Environment.NewLine + title;
                                    }
                                }
                                else
                                {
                                    approver += "UNKNOWN Approving Authority";
                                }
                            }

                            PrintingUtil.PrintingUtil.SetFormField(form348, "approvingRankName", approver);

                        }

                    }

                }

            }
        }

        private void SetMedicalInfo(PDFForm form348, LineOfDuty lod)
        {

            // medical info
            if (lod.LODMedical.TreatmentDate != null)
            {
                if (lod.LODMedical.TreatmentDate.HasValue)
                {
                    PrintingUtil.PrintingUtil.SetFormField(form348, "treatmentDate", Server.HtmlDecode(lod.LODMedical.TreatmentDate.Value.ToString(DATE_HOUR_FORMAT)));
                }
            }

            // start our diagnosis wit the nature of incident
            string natureOfIncident = lod.LODMedical.NatureOfIncidentDescription;

            // add EPTS
            if ((lod.LODMedical.Epts != null) && (lod.LODMedical.Epts.HasValue))
            {
                switch (lod.LODMedical.Epts.Value)
                {
                    case 0: // no EPTS
                        natureOfIncident += "    EPTS No";
                        break;
                    case 1: // EPTS Yes - Service Aggrivated
                        natureOfIncident += "    EPTS Yes/SA";
                        break;
                    case 2: // EPTS Yes - Not Service Aggrivated
                        natureOfIncident += "    EPTS Yes";
                        break;
                }
            }

            // add ICD9 info
            ICD9Code code = null;

            if ((lod.LODMedical.ICD9Id != null) && (lod.LODMedical.ICD9Id.HasValue))
            {
                code = LookupService.GetIcd9CodeById(lod.LODMedical.ICD9Id.Value);

                if (code != null)
                {
                    natureOfIncident += "    " + code.GetFullCode(lod.LODMedical.ICD7thCharacter) + " - " + code.Description;
                }

            }

            // the incidentType field holds: Incident type - EPTS - ICD9 code - ICD Diagnosis
            PrintingUtil.PrintingUtil.SetFormField(form348, "incidentType", natureOfIncident);

            // set the free-form diagnosis and physician info
            string diagnosis = lod.LODMedical.DiagnosisText;

            if (!string.IsNullOrEmpty(lod.LODMedical.ApprovalComments))
            {
                diagnosis += Environment.NewLine;
                diagnosis += Server.HtmlDecode(lod.LODMedical.ApprovalComments);
            }

            PrintingUtil.PrintingUtil.SetFormField(form348, "diagnosis", Server.HtmlDecode(diagnosis));
            PrintingUtil.PrintingUtil.SetFormField(form348, "treatmentInfo", Server.HtmlDecode(lod.LODMedical.MedicalFacility));

            SignatureMetaData sig = SigDao.GetByWorkStatus(lod.Id, lod.Workflow, (int)LodWorkStatus.MedicalOfficerReview);

            AddSignatureToForm(form348, sig,
                                "medicalDate", "medicalSignature",
                                "medicalName", "medicalRank",
                                DBSignTemplateId.Form348Medical,
                                PersonnelTypes.MED_OFF);
        }

        private void SetUnitInfo(PDFForm form348, LineOfDuty lod)
        {

            if (lod.SarcCase && lod.IsRestricted)
            {
                string message = "This Block not Used";

                PrintingUtil.PrintingUtil.SetFormField(form348, "unitName", message);
                PrintingUtil.PrintingUtil.SetFormField(form348, "judgeAdvocateRankName", message);
                PrintingUtil.PrintingUtil.SetFormField(form348, "appointingName", message);
            }
            else
            {

                // *************************************
                // Unit Commander
                // *************************************
                if (lod.LODUnit != null)
                {

                    SignatureMetaData sig = SigDao.GetByWorkStatus(lod.Id, lod.Workflow, (int)LodWorkStatus.UnitCommanderReview);

                    // we only display data if the UC has signed it
                    if (sig != null)
                    {

                        if (!string.IsNullOrEmpty(lod.LODUnit.DutyStatusDescription))
                        {
                            if (lod.LODUnit.DutyDetermination == DutyStatus.Active_Duty_Status)
                            {
                                PrintingUtil.PrintingUtil.SetFormField(form348, "unitActiveDutyStatus", "Yes");
                                if (lod.LODUnit.DutyFrom != null)
                                {
                                    if (lod.LODUnit.DutyFrom.HasValue)
                                    {
                                        PrintingUtil.PrintingUtil.SetFormField(form348, "unitActiveDutyStartDate", Server.HtmlDecode(lod.LODUnit.DutyFrom.Value.ToString(DATE_FORMAT)));
                                    }
                                }
                                if (lod.LODUnit.DutyTo != null)
                                {
                                    if (lod.LODUnit.DutyTo.HasValue)
                                    {
                                        PrintingUtil.PrintingUtil.SetFormField(form348, "unitActiveDutyEndDate", Server.HtmlDecode(lod.LODUnit.DutyTo.Value.ToString(DATE_FORMAT)));
                                    }
                                }
                            }
                            else if (lod.LODUnit.DutyDetermination != null)
                            {
                                PrintingUtil.PrintingUtil.SetFormField(form348, "unitInactiveDutyStatus", "Yes");
                                switch (lod.LODUnit.DutyDetermination)
                                {
                                    case DutyStatus.UTA:
                                        PrintingUtil.PrintingUtil.SetFormField(form348, "unitInactiveDutyStatusUTA", "Yes");
                                        break;
                                    case DutyStatus.AFTP:
                                        PrintingUtil.PrintingUtil.SetFormField(form348, "unitInactiveDutyStatusAFTP", "Yes");
                                        break;
                                    case DutyStatus.Saturday_night_rule:
                                        PrintingUtil.PrintingUtil.SetFormField(form348, "unitInactiveDutyStatusSaturdayNightRule", "Yes");
                                        break;
                                    case DutyStatus.Travel_to_from_duty:
                                        PrintingUtil.PrintingUtil.SetFormField(form348, "unitInactiveDutyStatusTravelDuty", "Yes");
                                        break;
                                    case DutyStatus.Unit_sponsored_event:
                                        PrintingUtil.PrintingUtil.SetFormField(form348, "unitInactiveDutyStatusUnitSponsoredEvent", "Yes");
                                        break;
                                    case DutyStatus.Other:
                                        PrintingUtil.PrintingUtil.SetFormField(form348, "unitInactiveDutyStatusOther", "Yes");
                                        PrintingUtil.PrintingUtil.SetFormField(form348, "unitOtherInfo", Server.HtmlDecode(lod.LODUnit.OtherDutyStatus));
                                        break;
                                }
                            }

                        }

                    } // end (if signed)

                    PrintingUtil.PrintingUtil.SetFormField(form348, "unitFindings", Server.HtmlDecode(lod.LODUnit.AccidentDetails));

                    LineOfDutyFindings unitFinding;
                    unitFinding = lod.FindByType((short)PersonnelTypes.UNIT_CMDR);
                    if (unitFinding != null)
                    {

                        if (unitFinding.Finding != null)
                        {
                            concurredFinding = (Finding)unitFinding.Finding.Value;

                            switch (concurredFinding)
                            {
                                case Finding.In_Line_Of_Duty:
                                    PrintingUtil.PrintingUtil.SetFormField(form348, "unitILod", "Yes");
                                    break;
                                case Finding.Recommend_Formal_Investigation:
                                    PrintingUtil.PrintingUtil.SetFormField(form348, "unitRecommendFormal", "Yes");
                                    break;
                                case Finding.Nlod_Due_To_Own_Misconduct:
                                    PrintingUtil.PrintingUtil.SetFormField(form348, "unitNotILodDom", "Yes");
                                    break;
                                case Finding.Epts_Lod_Not_Applicable:
                                    PrintingUtil.PrintingUtil.SetFormField(form348, "unitEptsNotApplicable", "Yes");
                                    break;
                                case Finding.Nlod_Not_Due_To_OwnMisconduct:
                                    PrintingUtil.PrintingUtil.SetFormField(form348, "unitNotILodNotDom", "Yes");
                                    break;
                                case Finding.Epts_Service_Aggravated:
                                    PrintingUtil.PrintingUtil.SetFormField(form348, "unitEptsServiceAggravated", "Yes");
                                    break;
                            }
                        }

                        // add the Unit Command signature
                        AddSignatureToForm(form348, sig,
                                    "unitDate", "unitSignature",
                                    "unitName", "unitRank",
                                    DBSignTemplateId.Form348Unit,
                                    PersonnelTypes.UNIT_CMDR);

                    } // unitFinding <> nothing

                }

            }
        }

        private void SetWingCommanderInfo(PDFForm form348, LineOfDuty lod)
        {

            SignatureMetaData sig = SigDao.GetByWorkStatus(lod.Id, lod.Workflow, (int)LodWorkStatus.AppointingAutorityReview);

            if (sig != null)
            {

                LineOfDutyFindings appointingfinding;
                appointingfinding = lod.FindByType((short)PersonnelTypes.APPOINT_AUTH);
                if (appointingfinding != null)
                {

                    if (appointingfinding.Finding != null)
                    {
                        if (appointingfinding.Finding.HasValue)
                        {
                            switch ((Finding)appointingfinding.Finding)
                            {
                                case Finding.In_Line_Of_Duty:
                                    PrintingUtil.PrintingUtil.SetFormField(form348, "appointingILod", "Yes");
                                    break;
                                case Finding.Recommend_Formal_Investigation:
                                    PrintingUtil.PrintingUtil.SetFormField(form348, "appointingRecommendFormal", "Yes");
                                    break;
                                case Finding.Nlod_Due_To_Own_Misconduct:
                                    PrintingUtil.PrintingUtil.SetFormField(form348, "appointingNotILodDom", "Yes");
                                    break;
                                case Finding.Epts_Lod_Not_Applicable:
                                    PrintingUtil.PrintingUtil.SetFormField(form348, "appointingEptsNotApplicable", "Yes");
                                    break;
                                case Finding.Nlod_Not_Due_To_OwnMisconduct:
                                    PrintingUtil.PrintingUtil.SetFormField(form348, "appointingNotILodNotDom", "Yes");
                                    break;
                                case Finding.Epts_Service_Aggravated:
                                    PrintingUtil.PrintingUtil.SetFormField(form348, "appointingEptsServiceAggravated", "Yes");
                                    break;

                            }
                        }
                    }

                    WorkStatus nextWorkStatus = wsdao.GetById((int)LodService.GetInitialNextStep(lod.Id, (int)LodWorkStatus.AppointingAutorityReview));

                    if (nextWorkStatus.StatusCodeType.Id == (int)LodStatusCode.BoardReview)
                    {
                        PrintingUtil.PrintingUtil.SetFormField(form348, "appointingForwardHQ", "Yes");
                    }

                    AddSignatureToForm(form348, sig,
                                    "appointingDate", "appointingSignature",
                                    "appointingName", "appointingRank",
                                    DBSignTemplateId.WingCC,
                                    PersonnelTypes.APPOINT_AUTH);

                }
            }
        }

        private void SetWingJudgeAdvocateInfo(PDFForm form348, LineOfDuty lod)
        {

            SignatureMetaData sig = SigDao.GetByWorkStatus(lod.Id, lod.Workflow, (int)LodWorkStatus.WingJAReview);

            // if not signed, we don't show anything
            if (sig != null)
            {

                LineOfDutyFindings jaFinding;
                jaFinding = lod.FindByType((short)PersonnelTypes.WING_JA);

                if (jaFinding != null)
                {

                    if (jaFinding.DecisionYN == "Y")
                    {
                        PrintingUtil.PrintingUtil.SetFormField(form348, "judgeAdvocateConcur", "Yes");
                    }
                    else if (jaFinding.DecisionYN == "N")
                    {
                        PrintingUtil.PrintingUtil.SetFormField(form348, "judgeAdvocateNonConcur", "Yes");
                        if (jaFinding.Finding != null)
                        {
                            if (jaFinding.Finding.HasValue)
                            {
                                concurredFinding = (Finding)jaFinding.Finding.Value;
                            }
                        }
                    }

                    if (jaFinding.DecisionYN == "N")
                    {
                        switch (concurredFinding)
                        {
                            case Finding.In_Line_Of_Duty:
                                PrintingUtil.PrintingUtil.SetFormField(form348, "judgeAdvocateILod", "Yes");
                                break;
                            case Finding.Recommend_Formal_Investigation:
                                PrintingUtil.PrintingUtil.SetFormField(form348, "judgeAdvocateRecommendFormal", "Yes");
                                break;
                            case Finding.Nlod_Due_To_Own_Misconduct:
                                PrintingUtil.PrintingUtil.SetFormField(form348, "judgeAdvocateNotILodDom", "Yes");
                                break;
                            case Finding.Epts_Lod_Not_Applicable:
                                PrintingUtil.PrintingUtil.SetFormField(form348, "judgeAdvocateEptsNotApplicable", "Yes");
                                break;
                            case Finding.Nlod_Not_Due_To_OwnMisconduct:
                                PrintingUtil.PrintingUtil.SetFormField(form348, "judgeAdvocateNotILodNotDom", "Yes");
                                break;
                            case Finding.Epts_Service_Aggravated:
                                PrintingUtil.PrintingUtil.SetFormField(form348, "judgeAdvocateEptsServiceAggravated", "Yes");
                                break;
                        }
                    }

                    AddSignatureToForm(form348, sig,
                                        "judgeAdvocateDate", "judgeAdvocateSignature",
                                        "judgeAdvocateName", "judgeAdvocateRank",
                                        DBSignTemplateId.Form348Findings,
                                        PersonnelTypes.WING_JA);

                }
            } // end (if signed)
        }
    }
}
