using System;
using System.Configuration;
using System.Text;
using System.Text.RegularExpressions;
using ALOD.Core.Domain.DBSign;
using ALOD.Core.Domain.Modules.Lod;
using ALOD.Core.Domain.Modules.Reinvestigations;
using ALOD.Core.Domain.Modules.SARC;
using ALOD.Core.Domain.Workflow;
using ALOD.Core.Interfaces.DAOInterfaces;
using ALOD.Data;
using ALOD.Data.Services;
using ALOD.Logging;
using ALODWebUtility.Common;

namespace ALODWebUtility.Printing
{
    // Builds PDF of Forms 348 and 261 and stores in database when workflow status is Complete (isFinal)
    // This code was borrowed from lod/Print.aspx.vb and modified
    // GeneratePdf() procedure is called from the NextAction.aspx tab when user selects Complete or Cancel and Digitally Signs.

    public class PDFCreateFactory : System.Web.UI.Page
    {
        // this is the date RCPHA was shutdown and operations moved to ALOD (Jan 29, 2010)
        // signatures which occurred before this date use the old //signed// format
        // signatures which occurred after this date use the new LAST.FIRST.MIDDLE.EDIPIN format
        protected static readonly DateTime EpochDate = new DateTime(2010, 1, 29);

        private const string BRANCH_AFRC = "AFRC";
        private const string DIGITAL_SIGNATURE_DATE_FORMAT = "yyyy.MM.dd HH:mm:ss zz'00'";
        private const int ROTC_CADET_ID = 5;
        private const string SIGNED_TEXT = "//SIGNED//";
        private NHibernateDaoFactory _daoFactory;
        private ISARCDAO _sarcDao;
        private ISignatueMetaDateDao _sigDao;
        private IWorkStatusDao _workStatusDao;
        private int lodid;
        private string remarksField = "";
        private bool replaceIOSig = false;
        private DBSignService signatureService;
        ModuleType type;

        ISignatueMetaDateDao SigDao
        {
            get
            {
                if (_sigDao == null)
                {
                    _sigDao = new NHibernateDaoFactory().GetSigMetaDataDao();
                }

                return _sigDao;
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

        protected IWorkStatusDao WorkStatusDao
        {
            get
            {
                if (_workStatusDao == null)
                {
                    _workStatusDao = DaoFactory.GetWorkStatusDao();
                }

                return _workStatusDao;
            }
        }

        private DBSignService VerifySource
        {
            get { return signatureService; }
            set { signatureService = value; }
        }

        public PDFDocument GenerateLOD(int refId)
        {
            bool isRLod = false;
            bool updatedRR = false;
            NHibernateDaoFactory factory = new NHibernateDaoFactory();
            LineOfDutyDao dao = factory.GetLineOfDutyDao();
            LineOfDuty lod = new LineOfDuty();
            LineOfDuty origLod = new LineOfDuty();
            PDFDocument doc = new PDFDocument();
            WorkStatusDao wsdao = new WorkStatusDao();
            ALOD.Core.Domain.Workflow.WorkStatus lodCurrStatus;

            if (dao.GetWorkflow(refId) == 27)
            {
                lod = new LineOfDuty_v2();
            }

            lod = LodService.GetById(refId);

            // Check if this LOD is a reinvestigation of another LOD case...
            isRLod = LookupService.GetIsReinvestigationLod(refId);

            if (isRLod)
            {
                // Check if the RR case has a case ID which matches the new ID format implemented when reinvestigation cases were changed to initialize as a formal case,
                // start in the Formal Appointing Authority review step, and not create a new Form348...
                LODReinvestigation rr = LodService.requestDao.GetReinvestigationRequestIdByRLod(refId) != 0 
                    ? LodService.requestDao.GetById(LodService.requestDao.GetReinvestigationRequestIdByRLod(refId)) 
                    : null;

                if (rr != null)
                {
                    Regex regex = new Regex(@"^\d{8}-\d{3}-RR-\d{3}?", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant | RegexOptions.IgnorePatternWhitespace | RegexOptions.Compiled);

                    if (regex.IsMatch(rr.CaseId))
                    {
                        // Load the original LOD case and use that to generate the 348 form...

                        if (dao.GetWorkflow(rr.InitialLodId) == 27)
                        {
                            origLod = new LineOfDuty_v2();
                        }

                        origLod = LodService.GetById(rr.InitialLodId);
                        updatedRR = true;
                    }
                }
            }

            Form261 form261 = new Form261();
            Form348 form348 = new Form348();
            Form348_v2 form348_v2 = new Form348_v2();

            if (isRLod && updatedRR)
            {
                if (origLod.Workflow == 27)
                {
                    doc.AddForm(form348_v2.GeneratePDFForm(origLod.Id, replaceIOSig));
                }
                else
                {
                    doc.AddForm(form348.GeneratePDFForm(origLod.Id, replaceIOSig));
                }

                if (lod.LODInvestigation != null)
                {
                    doc.AddForm(form261.GeneratePDFForm(lod.Id, replaceIOSig));
                }
            }
            else if (isRLod && !updatedRR)
            {
                doc.AddForm(form348.GeneratePDFForm(lod.Id, replaceIOSig));

                if (lod.LODInvestigation != null)
                {
                    doc.AddForm(form261.GeneratePDFForm(lod.Id, replaceIOSig));
                }
            }
            else
            {
                if (lod.Workflow == 27)
                {
                    doc.AddForm(form348_v2.GeneratePDFForm(lod.Id, replaceIOSig));
                }
                else
                {
                    doc.AddForm(form348.GeneratePDFForm(lod.Id, replaceIOSig));
                }

                if (lod.Formal && lod.LODInvestigation != null)
                {
                    doc.AddForm(form261.GeneratePDFForm(lod.Id, replaceIOSig));
                }
            }

            lodCurrStatus = wsdao.GetById(lod.Status);

            // If the case has been cancelled, then add a null watermark to the document
            if (lodCurrStatus.StatusCodeType.IsCancel)
            {
                AddNullWatermark(doc, lod);
            }

            return doc;
        }

        public PDFDocument GeneratePdf(int refId, int moduleId, bool replaceIOsig)
        {
            this.replaceIOSig = replaceIOsig;

            return GeneratePdf(refId, moduleId);
        }

        public PDFDocument GeneratePdf(int refId, int moduleId)
        {
            if (moduleId == (int)ModuleType.LOD)
            {
                return GenerateLOD(refId);
            }
            else
            {
                return GenerateRestrictedSARCDocument(refId);
            }
        }

        private bool IsValidSignature(SignatureEntry signature)
        {
            if (signature == null)
            {
                return false;
            }

            return signature.IsSigned;
        }

        #region WaterMark

        private void AddNullWatermark(PDFDocument doc, LineOfDuty lod)
        {
            string reason = string.Empty;
            string sigLine = string.Empty;
            int newLines = 2;

            // Get cancel reason...
            if (lod.Formal)
            {
                if (lod.AppointingCancelReasonId.HasValue && lod.AppointingCancelReasonId != 0)
                {
                    reason = LookupService.GetCancelReasonDescription(lod.AppointingCancelReasonId);
                }
                else if (lod.ApprovingCancelReasonId.HasValue && lod.AppointingCancelReasonId != 0)
                {
                    reason = LookupService.GetCancelReasonDescription(lod.ApprovingCancelReasonId);
                }
                else
                {
                    reason = "Unknown";
                }

                SignatureMetaData WingCCsig = ((SignatureMetaDataDao)SigDao).GetByWorkStatus(lod.Id, lod.Workflow, LodWorkStatus_v2.AppointingAutorityReview);

                sigLine = GetWatermarkSignature(WingCCsig, DBSignTemplateId.WingCC, lod.Id, PersonnelTypes.APPOINT_AUTH);
            }
            else
            {
                if (lod.LODMedical.PhysicianCancelReason != 0)
                {
                    reason = LookupService.GetCancelReasonDescription(lod.LODMedical.PhysicianCancelReason);
                }
                else
                {
                    reason = "Unknown";
                }

                SignatureMetaData Medsig = ((SignatureMetaDataDao)SigDao).GetByWorkStatus(lod.Id, lod.Workflow, LodWorkStatus_v2.MedicalOfficerReview);

                sigLine = GetWatermarkSignature(Medsig, DBSignTemplateId.Form348Medical, lod.Id, PersonnelTypes.MED_OFF);
            }

            if (reason.Length >= 36)
            {
                newLines = 1;
            }

            // Add watermark strings...
            doc.AddNullWatermarkString(new PDFString() { Text = "Case Cancelled", Linespacing = -20, FontSize = 36, FontWeight = "bold", PostNewLines = 1 });
            doc.AddNullWatermarkString(new PDFString() { Text = "Reason: " + reason, FontSize = 24, FontWeight = "bold", PostNewLines = newLines });
            doc.AddNullWatermarkString(new PDFString() { Text = sigLine, FontWeight = "bold" });
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
                sigLine = SIGNED_TEXT + "<BR>" + dateSigned.ToString(Utility.DATE_FORMAT);

                return sigLine;
            }

            // This signature occurred after the epoch, so verify it
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
                sigLine = SIGNED_TEXT + "<BR>" + dateSigned.ToString(Utility.DATE_FORMAT);

                return sigLine;
            }
        }

        #endregion

        #region Restricted SARC - Form 348R

        public PDFDocument GenerateRestrictedSARCDocument(int refId)
        {
            try
            {
                if (!Utility.UserHasPermission(Utility.PERMISSION_VIEW_SARC_CASES))
                {
                    LogSARCDeniedError(refId);
                    return null;
                }

                Form348_SARC form348_SARC = new Form348_SARC();
                PDFDocument doc = new PDFDocument();
                RestrictedSARC sarc = null;
                WorkStatus currentWorkStatus = null;

                sarc = SARCDao.GetById(refId);

                doc.AddForm(form348_SARC.GeneratePDFForm(refId, replaceIOSig));

                currentWorkStatus = WorkStatusDao.GetById(sarc.WorkflowStatus.Id);

                if (currentWorkStatus.StatusCodeType.IsCancel)
                {
                    Add348RNullWatermark(doc, sarc);
                }

                return doc;
            }
            catch (Exception ex)
            {
                LogManager.LogError(ex);
                throw new Exception("Error: GenerateSARCRestrictedDocument() in PDFCreateFactory.cs generated an exception.");
            }
        }

        protected void LogSARCDeniedError(int refId)
        {
            try
            {
                StringBuilder msg = new StringBuilder();
                msg.Append("Access Denied" + Environment.NewLine);
                msg.Append("UserID: " + refId.ToString() + Environment.NewLine);
                msg.Append("Request: " + Request.Url.ToString() + Environment.NewLine);

                if (Request.UrlReferrer != null)
                {
                    msg.Append("Referrer: " + Request.UrlReferrer.ToString() + Environment.NewLine);
                }

                msg.Append("Reason: User is attempting to view a Restricted SARC PDF without permission");

                LogManager.LogError(msg.ToString());
                Response.Redirect(ConfigurationManager.AppSettings["AccessDeniedUrl"]);
            }
            catch (Exception ex)
            {
                LogManager.LogError(ex);
                throw new Exception("Error: LogSARCDeniedError() in PDFCreateFactory.cs generated an exception.");
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

                SignatureMetaData sig = ((SignatureMetaDataDao)SigDao).GetByWorkStatus(sarc.Id, sarc.Workflow, SARCRestrictedWorkStatus.SARCInitiate);

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
                throw new Exception("Error: Add348RNullWatermark() in PDFCreateFactory.cs generated an exception.");
            }
        }

        #endregion
    }
}
