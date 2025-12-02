using System;
using System.Collections.Generic;
using ALOD.Core.Domain.Documents;
using ALOD.Core.Domain.Workflow;
using ALOD.Core.Interfaces.DAOInterfaces;
using ALOD.Data;
using ALOD.Data.Services;
using ALODWebUtility.Common;
using WebSupergoo.ABCpdf8;
using static ALODWebUtility.Common.SessionInfo;
using static ALODWebUtility.Common.Utility;

namespace ALOD.Web
{
    public class PrintCaseTracking1 : System.Web.UI.Page
    {
        private IDaoFactory _daoFactory;
        private IWorkflowDao _workflowDao;

        protected IDaoFactory DaoFactory
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

        protected IWorkflowDao WorkflowDao
        {
            get
            {
                if (_workflowDao == null)
                {
                    _workflowDao = DaoFactory.GetWorkflowDao();
                }
                return _workflowDao;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            byte case_module = byte.Parse(Request.QueryString["module"]);
            int refId = int.Parse(Request.QueryString["refId"]);
            bool showAll = bool.Parse(Request.QueryString["showAll"]);
            string CaseId = Request.QueryString["CaseId"];

            // get the data
            IEnumerable<WorkStatusTracking> data = LookupService.GetWorkStatusTracking(refId, case_module);

            // set up our pdf document
            Doc doc = new Doc();

            // apply a rotation transform
            double w = doc.MediaBox.Width;
            double h = doc.MediaBox.Height;
            double l = doc.MediaBox.Left;
            double b = doc.MediaBox.Bottom;
            doc.Transform.Rotate(90, l, b);
            doc.Transform.Translate(w, 0);

            // rotate our rectangle
            doc.Rect.Width = h;
            doc.Rect.Height = w;

            // rotate the page
            int docId = doc.GetInfoInt(doc.Root, "Pages");
            doc.SetInfo(docId, "/Rotate", "90");

            doc.FontSize = 10;
            doc.Rect.Inset(20, 40);

            PdfTable table = new PdfTable(doc, 5);
            table.CellPadding = 5;
            table.RepeatHeader = true;

            int i = 0;
            int page = 1;
            bool shade = false;

            // header row
            table.NextRow();
            string[] header = { "Process Name", "Start Date", "End Date", "Days in Process", "Completed By" };
            table.AddHtml(header);

            foreach (WorkStatusTracking row in data)
            {
                table.NextRow();

                string[] cols = new string[5];
                cols[0] = row.WorkflowStatus.Description;
                cols[1] = row.StartDate.ToString(DATE_HOUR_FORMAT);

                if (row.EndDate != null)
                {
                    cols[2] = row.EndDate.Value.ToString(DATE_HOUR_FORMAT);
                }
                else
                {
                    cols[2] = "";
                }

                cols[3] = row.DaysInStep.TotalDays.ToString("N2");

                if (row.CompletedBy != null)
                {
                    cols[4] = GetUserName(row.CompletedBy.Value);
                }
                else
                {
                    cols[4] = "";
                }
                table.AddHtml(cols);

                if (doc.PageNumber > page)
                {
                    page = doc.PageNumber;
                    shade = true;
                }

                if (shade)
                {
                    table.FillRow("216 216 255", table.Row);
                }

                shade = !shade;
                i = i + 1;
            }

            string title = WorkflowDao.GetCaseType(case_module);

            for (int ct = 1; ct <= doc.PageCount; ct++)
            {
                doc.PageNumber = ct;

                // left side
                doc.HPos = 0.0;
                doc.Rect.SetRect(20, 580, 280, 20);
                doc.AddText(CaseId + " - " + title);

                // middle
                doc.HPos = 0.5;
                doc.Rect.SetRect(20, 580, 750, 20);
                doc.AddText("Case Tracking");

                // right
                doc.HPos = 1.0;
                doc.Rect.SetRect(500, 580, 270, 20);
                doc.AddText("Generated: " + DateTime.Now.ToString(DATE_HOUR_FORMAT));

                // page number
                doc.HPos = 0.5;
                doc.Rect.SetRect(20, 20, 750, 20);
                doc.AddText("Page " + ct.ToString() + " of " + doc.PageCount.ToString());

                // table header
                doc.AddLine(20, 552, 772, 552);
            }

            doc.Flatten();

            // send the output to the client
            byte[] theData = doc.GetData();

            Response.ContentType = "application/pdf";
            Response.AddHeader("content-disposition", "inline; filename=Tracking.PDF");
            Response.AddHeader("content-length", theData.Length.ToString());
            Response.BinaryWrite(theData);
            Response.End();
        }

        protected string GetUserName(int userId)
        {
            // This helper method would typically look up the user name from the database
            var user = DaoFactory.GetUserDao().GetById(userId);
            return user != null ? user.FullName : "Unknown";
        }
    }
}
