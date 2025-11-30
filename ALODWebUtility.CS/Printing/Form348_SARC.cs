using System;
using System.Configuration;
using System.Text;
using System.Web.UI;
using ALOD.Core.Domain.DBSign;
using ALOD.Core.Domain.Documents;
using ALOD.Core.Domain.Modules.Lod;
using ALOD.Core.Domain.Modules.SARC;
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
    public class Form348_SARC : Page, IDocumentCreate
    {
        protected static readonly DateTime EpochDate = new DateTime(2010, 1, 29);
        private const string BRANCH_AFRC = "AFRC";
        private const string DIGITAL_SIGNATURE_DATE_FORMAT = "yyyy.MM.dd HH:mm:ss zz'00'";
        private const int ROTC_CADET_ID = 5;
        private const string SIGNED_TEXT = "//SIGNED//";
        private NHibernateDaoFactory _daoFactory;
        private ISARCDAO _sarcDao;
        private ISignatueMetaDateDao _sigDao;
        private int lodid;
        private string remarksField = "";
        private bool replaceIOSig = false;
        private DBSignService signatureService;

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

        protected NHibernateDaoFactory DaoFactory
        {
            get
            {
                if (_daoFactory == null)
                {
                    _daoFactory = new NHibernateDaoFactory();
                }

                return _daoFactory;
            }
        }

        protected ISARCDAO SARCDao
        {
            get
            {
                if (_sarcDao == null)
                {
                    _sarcDao = DaoFactory.GetSARCDao();
                }

                return _sarcDao;
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
            try
            {
                RestrictedSARC sarc = null;

                PDFForm form348R = new PDFForm(PrintDocuments.FormAFRC348R);

                sarc = SARCDao.GetById(refId);

                PrintingUtil.PrintingUtil.SetFormField(form348R, "sarcCaseNumberP1", Server.HtmlDecode(sarc.CaseId));
                PrintingUtil.PrintingUtil.SetDateTimeField(form348R, "ReportDateFill", sarc.CreatedDate, "ddMMMyyyy", true);
                PrintingUtil.PrintingUtil.SetDateTimeField(form348R, "IncidentDateFill", sarc.IncidentDate, "ddMMMyyyy", true);
                PrintingUtil.PrintingUtil.SetFormField(form348R, "DatabaseNumberFill", Server.HtmlDecode(sarc.DefenseSexualAssaultDBCaseNumber));
                PrintingUtil.PrintingUtil.SetDateTimeField(form348R, "OrdersStart", sarc.DurationStart, "ddMMMyyyy", true);
                PrintingUtil.PrintingUtil.SetDateTimeField(form348R, "d_StartTime", sarc.DurationStart, Utility.HOUR_FORMAT);
                PrintingUtil.PrintingUtil.SetDateTimeField(form348R, "OrdersEnd", sarc.DurationEnd, "ddMMMyyyy", true);
                PrintingUtil.PrintingUtil.SetDateTimeField(form348R, "d_EndTime", sarc.DurationEnd, Utility.HOUR_FORMAT);

                Set348RDutyStatusField(form348R, sarc);
                Set348RICDRelatedFields(form348R, sarc);

                SetInDutyStatusField(form348R, sarc);

                PrintingUtil.PrintingUtil.SetFormField(form348R, "Sec8Block", "BLOCK NOT USED");

                Set348RSignatureFields(form348R, sarc);

                SetRemarksField(form348R, sarc);

                return form348R;
            }
            catch (Exception ex)
            {
                LogManager.LogError(ex);
                throw new Exception("Error: Generate348RForm() in PDFCreateFactory.vb generated an exception.");
            }
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
                PrintingUtil.PrintingUtil.SetFormField(doc, dateField, signInfo.DateSigned.ToString("ddMMMyyyy").ToUpper());
                valid = true;
            }
            else
            {
                // Use electronic signature
                PrintingUtil.PrintingUtil.SetFormField(doc, sigField, SIGNED_TEXT);
                PrintingUtil.PrintingUtil.SetFormField(doc, dateField, signature.date.ToString("ddMMMyyyy").ToUpper());
                valid = false;
            }

            return valid;
        }

        protected string GetApprovingAuthorityRemarks(PDFForm form348R, RestrictedSARC sarc)
        {
            if (sarc.DutyStatus.HasValue && sarc.DutyStatus.Value != ROTC_CADET_ID)
            {
                return GetSARCApprovingAuthorityFindingsRemarksText(sarc);
            }

            return string.Empty;
        }

        protected string GetOtherICDCodesRemarksText(RestrictedSARC sarc)
        {
            try
            {
                if (!sarc.ICDOther.HasValue || sarc.ICDOther.Value == false || sarc.ICDList.Count == 0)
                {
                    return string.Empty;
                }

                string icdRemarks = "Other ICD Code(s):";

                foreach (RestrictedSARCOtherICDCode code in sarc.ICDList)
                {
                    icdRemarks += " [" + code.ICDCode.GetFullCode(code.ICD7thCharacter) + "] " + code.ICDCode.Description + ";";
                }

                icdRemarks = icdRemarks.Substring(0, icdRemarks.Length - 1) + Environment.NewLine + Environment.NewLine;

                return icdRemarks;
            }
            catch (Exception ex)
            {
                LogManager.LogError(ex);
                throw new Exception("Error: GetOtherICDCodesRemarksText() in PDFCreateFactory.vb generated an exception.");
            }
        }

        protected string GetSARCAdminRemarks(PDFForm form348R, RestrictedSARC sarc)
        {
            if (sarc.DutyStatus.HasValue && sarc.DutyStatus.Value != ROTC_CADET_ID)
            {
                return GetSARCAdminRemarksText(sarc);
            }

            return string.Empty;
        }

        protected string GetSARCAdminRemarksText(RestrictedSARC sarc)
        {
            try
            {
                if (SigDao.GetByWorkStatus(sarc.Id, sarc.Workflow, SARCRestrictedWorkStatus.SARCAdminReview) == null)
                {
                    return string.Empty;
                }

                string sarcAdminRemarksHeader = "SARC Administrator Remarks: ";
                RestrictedSARCFindings sarcAdminFindings = sarc.FindByType(PersonnelTypes.SARC_ADMIN);
                string newLines = Environment.NewLine + Environment.NewLine;

                if (sarcAdminFindings == null)
                {
                    return (sarcAdminRemarksHeader + "REMARKS NOT FOUND!" + newLines);
                }

                return (sarcAdminRemarksHeader + PrintingUtil.PrintingUtil.RemoveNewLinesFromString(Server.HtmlDecode(sarcAdminFindings.Remarks)) + newLines);
            }
            catch (Exception ex)
            {
                LogManager.LogError(ex);
                throw new Exception("Error: GetSARCAdminRemarksText() in PDFCreate.vb generated an exception.");
            }
        }

        protected string GetSARCApprovingAuthorityFindingsRemarksText(RestrictedSARC sarc)
        {
            try
            {
                if (SigDao.GetByWorkStatus(sarc.Id, sarc.Workflow, SARCRestrictedWorkStatus.SARCApprovingAuthorityReview) == null)
                {
                    return string.Empty;
                }

                string approvingAuthorityFindingsHeader = "Reviewing Authority (ARC/A1) Findings: ";
                RestrictedSARCFindings approvingAuthorityFindings = sarc.FindByType(PersonnelTypes.BOARD_AA);
                string newLines = Environment.NewLine + Environment.NewLine;

                if (approvingAuthorityFindings == null || approvingAuthorityFindings.Finding == null)
                {
                    return (approvingAuthorityFindingsHeader + "FINDINGS NOT FOUND!" + newLines);
                }

                if (approvingAuthorityFindings.Finding == Finding.Request_Consultation)
                {
                    return string.Empty;
                }

                return (approvingAuthorityFindingsHeader + PrintingUtil.PrintingUtil.GetFindingFormText(approvingAuthorityFindings.Finding.Value) + newLines);
            }
            catch (Exception ex)
            {
                LogManager.LogError(ex);
                throw new Exception("Error: GetSARCApprovingAuthorityFindingsRemarksText() in PDFCreate.vb generated an exception.");
            }
        }

        protected void LogSARCDeniedError(int refId)
        {
            try
            {
                StringBuilder msg = new StringBuilder();
                msg.Append("Access Denied" + System.Environment.NewLine);
                msg.Append("UserID: " + refId.ToString() + System.Environment.NewLine);
                msg.Append("Request: " + Request.Url.ToString() + System.Environment.NewLine);

                if (Request.UrlReferrer != null)
                {
                    msg.Append("Referrer: " + Request.UrlReferrer.ToString() + System.Environment.NewLine);
                }

                msg.Append("Reason: User is attempting to view a Restricted SARC PDF without permission");

                LogManager.LogError(msg.ToString());
                Response.Redirect(ConfigurationManager.AppSettings["AccessDeniedUrl"]);
            }
            catch (Exception ex)
            {
                LogManager.LogError(ex);
                throw new Exception("Error: LogSARCDeniedError() in PDFCreateFactory.vb generated an exception.");
            }
        }

        protected void Set348RDutyStatusField(PDFForm form348R, RestrictedSARC sarc)
        {
            try
            {
                if (!sarc.DutyStatus.HasValue)
                {
                    return;
                }

                string fieldName = string.Empty;

                switch (sarc.DutyStatus.Value)
                {
                    case 1:
                        fieldName = "AFRCheck";
                        break;
                    case 3:
                        fieldName = "ANGCheck";
                        break;
                    case ROTC_CADET_ID:
                        fieldName = "AFROTCCheck";
                        break;
                    default:
                        return;
                }

                PrintingUtil.PrintingUtil.SetFormField(form348R, fieldName, "1");
            }
            catch (Exception ex)
            {
                LogManager.LogError(ex);
                throw new Exception("Error: Set348RDutyStatusField() in PDFCreateFactory.vb generated an exception.");
            }
        }

        protected void Set348RICDRelatedFields(PDFForm form348R, RestrictedSARC sarc)
        {
            try
            {
                PrintingUtil.PrintingUtil.SetCheckboxField(form348R, "E968Check", sarc.ICDE968);
                PrintingUtil.PrintingUtil.SetCheckboxField(form348R, "E969Check", sarc.ICDE969);
                PrintingUtil.PrintingUtil.SetCheckboxField(form348R, "OtherCheck", sarc.ICDOther);

                if (sarc.ICDOther.HasValue && sarc.ICDOther.Value)
                {
                    PrintingUtil.PrintingUtil.SetFormField(form348R, "OtherICDText", "* See Section 10. REMARKS");
                }
            }
            catch (Exception ex)
            {
                LogManager.LogError(ex);
                throw new Exception("Error: Set348RICDRelatedFields() in PDFCreate.vb generated an exception.");
            }
        }

        protected void Set348RSignatureFields(PDFForm form348R, RestrictedSARC sarc)
        {
            SetWingSARCRSLSignatureFields(form348R, sarc);
            SetApprovingAuthoritySignatureFields(form348R, sarc);
        }

        protected void SetApprovingAuthoritySignatureFields(PDFForm form348R, RestrictedSARC sarc)
        {
            try
            {
                if (!sarc.DutyStatus.HasValue && sarc.DutyStatus.Value == ROTC_CADET_ID)
                {
                    return;
                }

                AddSignatureInformationToForm(sarc.Id, form348R, SigDao.GetByWorkStatus(sarc.Id, sarc.Workflow, SARCRestrictedWorkStatus.SARCApprovingAuthorityReview),
                                                  "Sec9DateFill", "LODReviewSign", "Sec9NameRank",
                                                  DBSignTemplateId.Form348SARCFindings, PersonnelTypes.BOARD_AA);
            }
            catch (Exception ex)
            {
                LogManager.LogError(ex);
                throw new Exception("Error: SetApprovingAuthoritySignatureFields() in PDFCreate.vb generated an exception.");
            }
        }

        protected void SetInDutyStatusField(PDFForm form348R, RestrictedSARC sarc)
        {
            if (!sarc.InDutyStatus.HasValue)
            {
                return;
            }

            if (sarc.InDutyStatus.Value)
            {
                PrintingUtil.PrintingUtil.SetCheckboxField(form348R, "YesCheck", true);
            }
            else
            {
                PrintingUtil.PrintingUtil.SetCheckboxField(form348R, "NoCheck", true);
            }
        }

        protected void SetRemarksField(PDFForm form348R, RestrictedSARC sarc)
        {
            string remarksValue = string.Empty;

            remarksValue += GetOtherICDCodesRemarksText(sarc);
            remarksValue += GetSARCAdminRemarks(form348R, sarc);
            remarksValue += GetApprovingAuthorityRemarks(form348R, sarc);

            PrintingUtil.PrintingUtil.SetFormField(form348R, "RemarksFill", remarksValue);
        }

        protected void SetWingSARCRSLSignatureFields(PDFForm form348R, RestrictedSARC sarc)
        {
            try
            {

                AddSignatureInformationToForm(sarc.Id, form348R, SigDao.GetByWorkStatus(sarc.Id, sarc.Workflow, SARCRestrictedWorkStatus.SARCInitiate),
                                                  "Sec7DateFill", "WingSARCSignAFROTC", "Sec7NameRank",
                                                  DBSignTemplateId.Form348SARCWing, PersonnelTypes.WING_SARC_RSL);
            }
            catch (Exception ex)
            {
                LogManager.LogError(ex);
                throw new Exception("Error: SetWingSARCRSLSignatureFields() in PDFCreate.vb generated an exception.");
            }
        }

        private void Add348RNullWatermark(PDFDocument doc, RestrictedSARC sarc)
        {
            try
            {
                string reason = "UNKNOWN";
                string sigLine = string.Empty;
                int newLines = 2;

                // Get cancel reason...
                if (sarc.Cancel_Reason.HasValue)
                {
                    reason = LookupService.GetCancelReasonDescription(sarc.Cancel_Reason.Value);
                }

                SignatureMetaData sig = SigDao.GetByWorkStatus(sarc.Id, sarc.Workflow, SARCRestrictedWorkStatus.SARCInitiate);

                sigLine = GetWatermarkSignature(sig, DBSignTemplateId.Form348SARC, sarc.Id, PersonnelTypes.WING_SARC_RSL);

                if (reason.Length > 36)
                {
                    newLines = 1;
                }

                // Add watermark strings...
                doc.AddNullWatermarkString(new PDFString() { Text = "Case Cancelled", Linespacing = -20, FontSize = 36, FontWeight = "bold", PostNewLines = 1 });
                doc.AddNullWatermarkString(new PDFString() { Text = "Reason: " + reason, FontSize = 24, FontWeight = "bold", PostNewLines = newLines });
                doc.AddNullWatermarkString(new PDFString() { Text = sigLine, FontWeight = "bold" });
            }
            catch (Exception ex)
            {
                LogManager.LogError(ex);
                throw new Exception("Error: Add348RNullWatermark() in PDFCreateFactory.vb generated an exception.");
            }
        }

        private string GetWatermarkSignature(SignatureMetaData signature, DBSignTemplateId template, int refId, PersonnelTypes ptype)
        {
            if (signature == null)
            {
                return string.Empty;
            }

            string sigLine = string.Empty;
            DateTime dateSigned = signature.date;

            // Check if the old style signature is needed...
            if (dateSigned < EpochDate)
            {
                sigLine = SIGNED_TEXT + "<BR>" + dateSigned.ToString(DATE_FORMAT);

                return sigLine;
            }

            // This signature occured after the epoch, so verify it
            VerifySource = new DBSignService(template, refId, ptype);

            DBSignResult signatureStatus = VerifySource.VerifySignature();

            // Check if valid signature...
            if (signatureStatus == DBSignResult.SignatureValid)
            {
                DigitalSignatureInfo signInfo = VerifySource.GetSignerInfo();

                if (signInfo == null)
                {
                    return string.Empty;
                }

                sigLine = "Digitally signed by "
                        + signInfo.Signature + "<BR>"
                        + "Date: " + signInfo.DateSigned.ToString(DIGITAL_SIGNATURE_DATE_FORMAT);

                return sigLine;
            }
            else
            {
                sigLine = SIGNED_TEXT + "<BR>" + dateSigned.ToString(DATE_FORMAT);

                return sigLine;
            }
        }
    }
}
