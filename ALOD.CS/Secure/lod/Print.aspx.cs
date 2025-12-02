using System;
using System.Collections.Generic;
using System.Web;
using ALOD.Core.Domain.Modules.Lod;
using ALOD.Core.Domain.Workflow;
using ALOD.Core.Interfaces.DAOInterfaces;
using ALOD.Data;
using ALODWebUtility.Common;
using ALODWebUtility.Printing;
using static ALODWebUtility.Common.SessionInfo;
using static ALODWebUtility.Common.Utility;

namespace ALOD.Web.LOD
{
    public partial class Secure_lod_Print : System.Web.UI.Page
    {
        public string url261;
        public string url348;

        // This is the date RCPHA was shutdown and operations moved to ALOD (Jan 29, 2010)
        // Signatures which occurred before this date use the old //signed// format
        // Signatures which occurred after this date use the new LAST.FIRST.MIDDLE.EDIPIN format
        protected const string EpochDate = "1/29/2010";

        private const string BRANCH_AFRC = "AFRC";
        private const string DIGITAL_SIGNATURE_DATE_FORMAT = "yyyy.MM.dd HH:mm:ss zz'00'";
        private const string SIGNED_TEXT = "//SIGNED//";
        private int lodid;
        private ModuleType type;

        protected void Page_Load(object sender, EventArgs e)
        {
            int Id = 0;
            int.TryParse(Request.QueryString["id"], out Id);

            if (Id == 0)
            {
                int.TryParse(Request.QueryString["refId"], out Id);
            }

            if (Id == 0)
            {
                return;
            }

            // Save our id for use in other methods
            lodid = Id;
            ILineOfDutyDao dao = new NHibernateDaoFactory().GetLineOfDutyDao();
            LineOfDuty LOD = dao.GetById(lodid, false);
            PageAccess.AccessLevel userAccess = dao.GetUserAccess(SESSION_USER_ID, Id);

            if (userAccess == PageAccess.AccessLevel.None && !UserHasPermission("lodViewAllCases"))
            {
                return;
            }

            bool is348only = false;
            bool is261only = false;

            switch (HttpContext.Current.Request.QueryString["form"])
            {
                case "348":
                    is348only = true;
                    break;
                case "261":
                    is261only = true;
                    break;
            }

            //********************************************
            // Resets Print button to view Final form 284/261
            PrintFinal docUrl348 = new PrintFinal();
            url348 = docUrl348.GetURL348(lodid.ToString(), HttpContext.Current.Session["UserName"].ToString(), dao);

            if (url348?.Length > 1)
            {
                url348 = this.ResolveClientUrl(url348);

                PrintFinal docUrl261 = new PrintFinal();
                url261 = docUrl261.GetURL261(lodid.ToString(), HttpContext.Current.Session["UserName"].ToString(), dao);
                if (url261?.Length > 1)
                {
                    url261 = this.ResolveClientUrl(url261);
                }
            }
            else
            {
                // If forms were not saved to database or not final
                PDFDocument doc;
                PDFCreateFactory create = new PDFCreateFactory();

                doc = create.GeneratePdf(Id, (int)ModuleType.LOD);

                List<int> numbers = new List<int> { 325, 326, 327, 328, 329, 330, 331, 332, 333, 334, 335, 336 };
                if (numbers.Contains(LOD.Status))
                {
                    doc.IncludeWSCompleteWaterMark = true;
                }

                if (doc != null)
                {
                    doc.Render(Page.Response);
                    doc.Close();
                }
            }
        }
    }
}
